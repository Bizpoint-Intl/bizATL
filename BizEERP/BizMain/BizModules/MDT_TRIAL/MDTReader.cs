using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizCommon;
using BizRAD.BizAccounts;
using BizRAD.BizControls.BizDateTimePicker;

using Za.Evaluator;

namespace DEMO.MDT
{
	public class MDTReader
	{

		public static void GetVoucherMDT(string MDTName, string moduleTable, string TDTNum, ref BizRAD.BizVoucher.VoucherEventArgs e)
		{
			try
			{
				DBAccess dbaccess = new DBAccess();

				//DataTable DT = e.DBAccess.DataSet.Tables[moduleTable + "h"];
				//DataTable tmpDT = e.DBAccess.DataSet.Tables[moduleTable + "h"].Copy();
				DataTable DT = e.DBAccess.DataSet.Tables[moduleTable];
				DataTable tmpDT = e.DBAccess.DataSet.Tables[moduleTable].Copy();

				//Get MDT Refnum
				DataSet DsMDTH = dbaccess.ReadSQLTemp("MDTH", "Select refnum from MDTH where mdtname='" + MDTName + "'");
				string refnum = DsMDTH.Tables["MDTH"].Rows[0]["refnum"].ToString().Trim();

				DataSet DsMDT1 = dbaccess.ReadSQLTemp("MDT1", "Select tdtnum from MDT1 where refnum='" + refnum + "' and tdtnum='" + TDTNum + "'");
				string TDT = DsMDT1.Tables["MDT1"].Rows[0][0].ToString().Trim();

				DataSet Ds = e.DBAccess.ReadSQLTemp("tdt4", "select collist,selectcond,orderby from tdt4 where userid='" + Common.DEFAULT_SYSTEM_USERNAME + "' and tdtnum='" + TDT + "'");

				#region collist

				Boolean found = false;
				string remove = string.Empty;

				string[] selectcond = Ds.Tables["tdt4"].Rows[0]["collist"].ToString().Split('|');

				for (int i = 0; i < tmpDT.Columns.Count; i++)
				{
					found = false;

					for (int j = 0; j < selectcond.Length; j++)
					{
						if (tmpDT.Columns[i].ToString().Trim() == selectcond[j].Trim())
						{
							found = true;
							break;
						}
					}

					if (!found)
					{
						DT.Columns.Remove(tmpDT.Columns[i].ColumnName);
					}
				}

				#endregion

				#region selectcond

				DataRow[] Dr = null;
				DataTable SelectDT;

				if (Ds.Tables["tdt4"].Rows.Count > 0)
				{
					//SelectDT = e.DBAccess.DataSet.Tables[moduleTable + "h"].Copy();
					SelectDT = e.DBAccess.DataSet.Tables[moduleTable].Copy();

					if (!BizFunctions.IsEmpty(Ds.Tables["tdt4"].Rows[0]["selectcond"]) && BizFunctions.IsEmpty(Ds.Tables["tdt4"].Rows[0]["orderby"]))
					{
						Dr = SelectDT.Select(Ds.Tables["tdt4"].Rows[0]["selectcond"].ToString());
					}
					else if (!BizFunctions.IsEmpty(Ds.Tables["tdt4"].Rows[0]["selectcond"]) && !BizFunctions.IsEmpty(Ds.Tables["tdt4"].Rows[0]["orderby"]))
					{
						Dr = SelectDT.Select(Ds.Tables["tdt4"].Rows[0]["selectcond"].ToString(), Ds.Tables["tdt4"].Rows[0]["orderby"].ToString());
					}
					else if (BizFunctions.IsEmpty(Ds.Tables["tdt4"].Rows[0]["selectcond"]) && !BizFunctions.IsEmpty(Ds.Tables["tdt4"].Rows[0]["orderby"]))
					{
						Dr = SelectDT.Select("", Ds.Tables["tdt4"].Rows[0]["orderby"].ToString());
					}
					else
					{
						Dr = SelectDT.Select("1=1");
					}

					//Remove all existing data in sivh
					//if (e.DBAccess.DataSet.Tables[moduleTable + "h"].Rows.Count > 0)
						//BizFunctions.DeleteAllRows(e.DBAccess.DataSet.Tables[moduleTable + "h"]);
					if (e.DBAccess.DataSet.Tables[moduleTable].Rows.Count > 0)
						BizFunctions.DeleteAllRows(e.DBAccess.DataSet.Tables[moduleTable]);
					
					if (Dr != null)
					{
						if (Dr.Length > 0)
						{
							for (int k = 0; k < Dr.Length; k++)
							{
								//e.DBAccess.DataSet.Tables[moduleTable + "h"].Rows.Add(Dr[k].ItemArray);
								e.DBAccess.DataSet.Tables[moduleTable].Rows.Add(Dr[k].ItemArray);
							}
						}
					}
				}
				#endregion
			}
			catch(Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
			}

		}

		public static void GetDefaultMDT(string MDTName, string moduleTable, string TDTName, ref DBAccess mainDS)
		{
			DBAccess dbaccess = new DBAccess();

			//Get MDT Refnum
			DataSet DsMDTH = dbaccess.ReadSQLTemp("MDTH", "Select refnum from MDTH where mdtname='" + MDTName + "'");
			string refnum = DsMDTH.Tables["MDTH"].Rows[0]["refnum"].ToString().Trim();

			DataSet DsMDT1 = dbaccess.ReadSQLTemp("MDT1", "Select tdtnum from MDT1 where refnum='" + refnum + "' and tdtname='" + TDTName + "'");
			string TDT = DsMDT1.Tables["MDT1"].Rows[0][0].ToString().Trim();

			DataSet DsTDT = dbaccess.ReadSQLTemp("TDT5", "Select tablename, fieldname, defaultvalue from TDT5 where tdtnum='" + TDT + "'");

			DataTable TDT5 = DsTDT.Tables["TDT5"];
			DataTable TDT5table = new DataTable();

			try
			{
				for (int i = 0; i < TDT5.Rows.Count; i++)
				{
					TDT5table = mainDS.DataSet.Tables[TDT5.Rows[i]["tablename"].ToString().Trim()];

					for (int j = 0; j < TDT5table.Rows.Count; j++)
					{
						if (mainDS.DataSet.Tables[TDT5.Rows[i]["tablename"].ToString().Trim()].Rows[j].RowState != DataRowState.Deleted)
						{
						if (mainDS.DataSet.Tables[TDT5.Rows[i]["tablename"].ToString().Trim()].Rows[j][TDT5.Rows[i]["fieldname"].ToString().Trim()].ToString().Trim() == String.Empty || mainDS.DataSet.Tables[TDT5.Rows[i]["tablename"].ToString().Trim()].Rows[j][TDT5.Rows[i]["fieldname"].ToString().Trim()] == System.DBNull.Value)
						{
						mainDS.DataSet.Tables[TDT5.Rows[i]["tablename"].ToString().Trim()].Rows[j][TDT5.Rows[i]["fieldname"].ToString().Trim()] = TDT5.Rows[i]["defaultvalue"].ToString().Trim();
						}
						}
					}
				}
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
			}


		}

		public static string[] GetComboMDT(string MDTName, string moduleTable)
		{
			DBAccess dbaccess = new DBAccess();
			string[] returnSTR;

			//Get MDT Refnum
			DataSet DsMDTH = dbaccess.ReadSQLTemp("MDTH", "Select refnum from MDTH where mdtname='" + MDTName + "'");
			string refnum = DsMDTH.Tables["MDTH"].Rows[0]["refnum"].ToString().Trim();

			DataSet DsMDT1 = dbaccess.ReadSQLTemp("MDT1", "Select tdtnum, tdtname from MDT1 where refnum='" + refnum + "'");

			returnSTR = new string[DsMDT1.Tables["MDT1"].Rows.Count];

			try
			{
			for (int i = 0; i < DsMDT1.Tables["MDT1"].Rows.Count; i++)
			{
				returnSTR[i] = DsMDT1.Tables["MDT1"].Rows[i]["tdtname"].ToString().Trim();
			}

			return returnSTR;
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
				return returnSTR;
			}
			
		}


		public static string[] GetComboContent(string TableName, string MatchField, string MatchValue, string GetField)
		{
			DBAccess dbaccess = new DBAccess();
			string[] returnCombo;

			//Get MDT Refnum
			DataSet DsMDTH = dbaccess.ReadSQLTemp("Combo", "Select " + MatchField.ToString().Trim() + "," + GetField.ToString().Trim() + " from "+ TableName.ToString().Trim() + " where " + MatchField.ToString().Trim() + "='" + MatchValue.ToString().Trim() + "'");
			//string refnum = DsMDTH.Tables["MDTH"].Rows[0]["refnum"].ToString().Trim();

			//DataSet DsMDT1 = dbaccess.ReadSQLTemp("MDT1", "Select tdtnum, tdtname from MDT1 where refnum='" + refnum + "'");

			returnCombo = new string[DsMDTH.Tables["Combo"].Rows.Count];

			try
			{
				for (int i = 0; i < DsMDTH.Tables["Combo"].Rows.Count; i++)
				{
					returnCombo[i] = DsMDTH.Tables["Combo"].Rows[i][GetField].ToString().Trim();
				}

				return returnCombo;
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
				return returnCombo;
			}

		}

		public static string GetaccnumMDT(string MDTName, string TDTName, string subledger)
		{
			DBAccess dbaccess = new DBAccess();

			//Get MDT Refnum
			DataSet DsMDTH = dbaccess.ReadSQLTemp("MDTH", "Select refnum from MDTH where mdtname='" + MDTName + "'");
			string refnum = DsMDTH.Tables["MDTH"].Rows[0]["refnum"].ToString().Trim();

			DataSet DsMDT1 = dbaccess.ReadSQLTemp("MDT1", "Select tdtnum from MDT1 where refnum='" + refnum + "' and tdtname='" + TDTName + "'");
			string TDT = DsMDT1.Tables["MDT1"].Rows[0][0].ToString().Trim();

			DataSet DsTDT = dbaccess.ReadSQLTemp("TDT1", "Select accnum from TDT1 where tdtnum='" + TDT + "' and lgr='" + subledger + "'");
			DataTable TDT1 = DsTDT.Tables["TDT1"];

			try
			{
			if (TDT1.Rows.Count > 0)
				return TDT1.Rows[0][0].ToString().Trim();
			else
				return string.Empty;
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
				return string.Empty;
			}

		}

		public static Variant Getformula(string MDTName, string moduleTable, string TDTNum, DBAccess dbAccess)
		{
			DBAccess dbaccess = new DBAccess();

			//Get MDT Refnum
			DataSet DsMDTH = dbaccess.ReadSQLTemp("MDTH", "Select refnum from MDTH where mdtname='" + MDTName + "'");
			string refnum = DsMDTH.Tables["MDTH"].Rows[0]["refnum"].ToString().Trim();

			DataSet DsMDT1 = dbaccess.ReadSQLTemp("MDT1", "Select tdtnum from MDT1 where refnum='" + refnum + "' and tdtnum='" + TDTNum + "'");
			string TDT = DsMDT1.Tables["MDT1"].Rows[0][0].ToString().Trim();

			DataSet DsTDT = dbaccess.ReadSQLTemp("TDT7", "Select formula from TDT7 where tdtnum='" + TDT + "'");
			DataTable TDT7 = DsTDT.Tables["TDT7"];

			if (TDT7.Rows.Count > 0)
			{
				return compute(moduleTable, TDT7.Rows[0][0].ToString().Trim(), dbAccess);
				 //TDT7.Rows[0][0].ToString().Trim();
			}
			else
				return string.Empty;

		}

		protected static Variant compute(string moduleTable, string formula, DBAccess dbAccess)
		{
			string[] Formula = formula.Split('[');
			string heading = moduleTable.ToLower() + "h";

			Hashtable hstformula = new Hashtable();

			for (int i = 1; i < Formula.Length; i++)
			{
				int count = Formula[i].IndexOf(']');

				string str = Formula[i].Substring(0,count);

				string[] table = str.Split('_');
				if (table[0].Trim() == heading)
					hstformula.Add(str, dbAccess.DataSet.Tables[table[0]].Rows[0][table[1]]);
				else
					hstformula.Add(str, dbAccess.DataSet.Tables[table[0]].Rows[dbAccess.DataSet.Tables[table[0]].Rows.Count - 1][table[1]]);

			}

			Evaluator.Variables = hstformula;

			return Evaluator.Evaluate(formula);

		}

		public static void updateAccount(ref DBAccess dbaccess, string columns, string moduleTable, string module, string AxDTable, string XIV10Table, string TDTName)
		{
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			string table = null;
			string get3Char = null;
			if (moduleTable == "SIVH" && module == "PIV")
			{
				table = module;
				get3Char = module + "1";
			}
			else
			{
				table = moduleTable.Substring(0, 3);
				get3Char = moduleTable.Substring(0, 3) + "1";
			}
			DataRow xIVH = dbaccess.DataSet.Tables[moduleTable].Rows[0];

			#region steph - Get the SIV10

			DataTable grpSiv1Query = null;

			if (module == "SAJ")
			{
				string strGrpSiv1Query = "SELECT sum(oriamt) as oriamt, "+
					" sum(postamt) as postamt,sum(gstamt) as gstamt,sum(oridebit) as oridebit,sum(oricredit) as oricredit, "+
					" invnum,invdate,exrate FROM "+
					" (SELECT (aftdeposit * -1) AS oriamt,0 postamt,0 AS gstamt,oridebit,oricredit,invnum,invdate,exrate FROM SAJ1 "+
					" WHERE ISNULL(detail2,'') <>'Voucher' "+
					" UNION ALL "+
					" SELECT oriamt,postamt,gstamt,oridebit,oricredit,invnum,invdate,exrate FROM DARD WHERE ISNULL(chkReverse,'')<>'Y' "+
					" AND ISNULL(detail2,'') <>'Voucher' " +
					" ) "+
					" group by invnum,invdate,exrate";
				grpSiv1Query = BizFunctions.ExecuteQuery(dbaccess.DataSet, strGrpSiv1Query);
			}

			else if (module == "MCR")
			{
				grpSiv1Query = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT sum(oriamt) as oriamt, " +
					" sum(postamt) as postamt,sum(gstamt) as gstamt,sum(oridebit) as oridebit,sum(oricredit) as oricredit, " +
					" invnum,trandate as invdate,exrate FROM " +
					" MCR1 " +
					" group by invnum,invdate,exrate");
			}
            else if (module == "ANP")
            {
                grpSiv1Query = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT sum(oriamt) as oriamt, " +
                    " sum(postamt) as postamt,sum(gstamt) as gstamt,sum(oridebit) as oridebit,sum(oricredit) as oricredit, " +
                    " invnum,trandate as invdate,exrate FROM " +
                    " ANP1 " +
                    " group by invnum,invdate,exrate");
            }
			else
			{
				grpSiv1Query = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT sum(oriamt) as oriamt,sum(postamt) as postamt,sum(gstamt) as gstamt,sum(oridebit) as oridebit,sum(oricredit) as oricredit,invnum,invdate,exrate FROM [" + get3Char + "] group by invnum,invdate,exrate");
			}

			DataTable xiv10 = dbaccess.DataSet.Tables[table + "10"];
			if (grpSiv1Query.Rows.Count > 0)
			{
				int county = grpSiv1Query.Rows.Count;
				int xiv10Count = 0;
				foreach (DataRow dr in grpSiv1Query.Rows)
				{
					xiv10.Rows.Add(new object[] { });

					xiv10.Rows[xiv10Count]["refnum"] = xIVH["refnum"].ToString().Trim();
					xiv10.Rows[xiv10Count]["coy"] = xIVH["coy"].ToString().Trim();
					xiv10.Rows[xiv10Count]["oriamt"] = dr["oriamt"];
					//xiv10.Rows[xiv10Count]["postamt"] = dr["postamt"];  // steph - amended for saj as the postamt the transaction could be of different currency
					xiv10.Rows[xiv10Count]["postamt"] = Convert.ToDecimal(dr["oriamt"]) * Convert.ToDecimal(dr["exrate"]);
					xiv10.Rows[xiv10Count]["invnum"] = dr["invnum"];
					xiv10.Rows[xiv10Count]["invdate"] = dr["invdate"];
					xiv10.Rows[xiv10Count]["gstamt"] = dr["gstamt"];
					xiv10.Rows[xiv10Count]["exrate"] = dr["exrate"].ToString().Trim();
					xiv10.Rows[xiv10Count]["oridebit"] = dr["oridebit"];
					xiv10.Rows[xiv10Count]["oricredit"] = dr["oricredit"];
					xiv10.Rows[xiv10Count]["gstper"] = xIVH["gstper"].ToString().Trim();
					xiv10.Rows[xiv10Count]["oricur"] = xIVH["oricur"].ToString().Trim();
					xiv10Count++;
				}

				xiv10Count = 0;

				if (AxDTable.ToString().Trim().ToUpper() == "ARD")
				{
					foreach (DataRow drAr in xiv10.Rows)
					{
						if (drAr.RowState != DataRowState.Deleted)
						{
								drAr["arnum"] = xIVH["arnum"].ToString().Trim();
						}
					}
				}

				if (AxDTable.ToString().Trim().ToUpper() == "APD")
				{
					foreach (DataRow drAr in xiv10.Rows)
					{
						if (drAr.RowState != DataRowState.Deleted)
						{
							drAr["apnum"] = xIVH["apnum"].ToString().Trim();
						}
					}
				}
			}
			#endregion

            if (xiv10.Rows.Count > 0)
            {
                int xiv10Count2 = 0;

                DataTable AxD = dbaccess.DataSet.Tables[AxDTable];

                DataRow xIV10 = xiv10.Rows[0];

                foreach (DataRow dr2 in xiv10.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        string[] StrColumn = columns.Split('|');

                        if (StrColumn.Length > 0)
                        {
                            DataRow tmpAxD = AxD.NewRow();

                            for (int i = 0; i < StrColumn.Length; i++)
                            {
                                switch (StrColumn[i])
                                {
                                    case "arnum":
                                        tmpAxD["arnum"] = dr2["arnum"];
                                        break;
                                    case "apnum":
                                        tmpAxD["apnum"] = dr2["apnum"];
                                        break;
                                    case "period":
                                        tmpAxD["period"] = xIVH["period"];
                                        break;
                                    case "coy":
                                        tmpAxD["coy"] = dr2["coy"];
                                        break;
                                    case "exramt":
                                        tmpAxD["exramt"] = 0;
                                        break;
                                    case "locno":
                                        tmpAxD["locno"] = xIVH["locno"];
                                        break;
                                    case "deptno":
                                        tmpAxD["deptno"] = xIVH["deptno"];
                                        break;
                                    case "accnum":
                                        tmpAxD["accnum"] = xIVH["accnum"].ToString().Trim();
                                        break;
                                    case "trandate":
                                        tmpAxD["trandate"] = xIVH["trandate"];
                                        break;
                                    case "lgr":
                                        tmpAxD["lgr"] = AxDTable;
                                        break;
                                    #region Steph - Changed for case oriamt and postamt to cater for cases where there are exrate
                                    case "exrate":
                                        tmpAxD["exrate"] = Convert.ToDecimal(dr2["exrate"]);
                                        break;
                                    case "invdate":
                                        tmpAxD["invdate"] = Convert.ToDateTime(dr2["invdate"]);
                                        break;
                                    case "detail":
                                        tmpAxD["detail"] = xIVH["remark"].ToString().Trim();
                                        break;
                                    case "oricur":
                                        tmpAxD["oricur"] = dr2["oricur"];
                                        break;
                                    case "oriamt":
                                        if (dr2["exrate"] == System.DBNull.Value || Convert.ToDecimal(dr2["exrate"]) == 0)
                                            dr2["exrate"] = 1;
                                        tmpAxD["oriamt"] = BizFunctions.Round(((Convert.ToDecimal(dr2["oriamt"])) + ((Convert.ToDecimal(dr2["gstamt"])) / (Convert.ToDecimal(dr2["exrate"])))) * -1);
                                        break;
                                    case "doriamt":
                                        if (dr2["exrate"] == System.DBNull.Value || Convert.ToDecimal(dr2["exrate"]) == 0)
                                            dr2["exrate"] = 1;
                                        tmpAxD["doriamt"] = System.Math.Abs(BizFunctions.Round(((Convert.ToDecimal(dr2["oriamt"])) + ((Convert.ToDecimal(dr2["gstamt"])) / (Convert.ToDecimal(dr2["exrate"])))) * -1));
                                        break;
                                    case "postamt":
                                        tmpAxD["postamt"] = BizFunctions.Round(((Convert.ToDecimal(dr2["postamt"])) + ((Convert.ToDecimal(dr2["gstamt"])))) * -1);
                                        break;
                                    case "dpostamt":
                                        tmpAxD["dpostamt"] = System.Math.Abs(BizFunctions.Round(((Convert.ToDecimal(dr2["postamt"])) + ((Convert.ToDecimal(dr2["gstamt"])))) * -1));
                                        break;
                                    case "flag":
                                        tmpAxD["flag"] = xIVH["flag"];
                                        break;
                                    case "gstamt":
                                        tmpAxD["gstamt"] = BizFunctions.Round(Convert.ToDecimal(dr2["postamt"]) * Convert.ToDecimal(dr2["gstper"]) / 100 * -1);
                                        break;

                                    case "oridebit":
                                        if (BizFunctions.Round((Convert.ToDecimal(dr2["oriamt"]) + (Convert.ToDecimal(dr2["gstamt"]) / (Convert.ToDecimal(dr2["exrate"]))))) < 0)
                                        {
                                            tmpAxD["oridebit"] = System.Math.Abs(BizFunctions.Round((Convert.ToDecimal(dr2["oriamt"]) + (Convert.ToDecimal(dr2["gstamt"]) / (Convert.ToDecimal(dr2["exrate"]))))));

                                        }
                                        else
                                        {
                                            tmpAxD["oridebit"] = 0;
                                        }
                                        break;

                                    case "oricredit":
                                        if (BizFunctions.Round(((Convert.ToDecimal(dr2["oriamt"])) + ((Convert.ToDecimal(dr2["gstamt"])) / (Convert.ToDecimal(dr2["exrate"]))))) > 0)
                                        {
                                            tmpAxD["oricredit"] = System.Math.Abs(BizFunctions.Round(((Convert.ToDecimal(dr2["oriamt"])) + ((Convert.ToDecimal(dr2["gstamt"])) / (Convert.ToDecimal(dr2["exrate"]))))));
                                        }
                                        else
                                        {
                                            tmpAxD["oricredit"] = 0;
                                        }
                                        break;
                                    #endregion Steph - Changed for case oriamt and postamt to cater for cases where there are exrate

                                    case "invnum":
                                        tmpAxD["invnum"] = grpSiv1Query.Rows[xiv10Count2]["invnum"];
                                        break;
                                    case "supinvnum":
                                        tmpAxD["supinvnum"] = xIVH["supinvnum"];
                                        break;

                                    default:
                                        tmpAxD[StrColumn[i]] = dr2[StrColumn[i]];
                                        break;
                                }
                            }

                            AxD.Rows.Add(tmpAxD.ItemArray);
                        }
                    }
                    xiv10Count2++;
                }

                if (module.ToString().Trim() == "SAJ")
                {
                    DataTable POSGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT sum(gstamt) AS oriamt FROM [saj1]");

                    BizAccounts.PostGLD(dbaccess, "SAJVOUCHER/DARD/" + AxDTable + "/" + table.ToLower() + "1", moduleTable, "sitenum,projectid");

                    #region Steph -  To add the exchange gain/loss from deposits

                    DataTable dard = dbaccess.DataSet.Tables["dard"];

                    decimal totExramt = 0;

                    foreach (DataRow dr in dard.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            if (dr["exramt"] != System.DBNull.Value && Convert.ToDecimal(dr["exramt"]) != 0)
                            {
                                DataRow addDardIntoGld = gld.Rows.Add(new object[] { });
                                BizFunctions.UpdateDataRow(xIVH, addDardIntoGld, "refnum/trandate/modified/created/oricur/flag");

                                dbaccess.ReadSQL("getExrAcc", "SELECT accnum FROM acc WHERE refnum = 'EXR1'");
                                if (dbaccess.DataSet.Tables["getExrAcc"].Rows.Count > 0)
                                {
                                    addDardIntoGld["accnum"] = dbaccess.DataSet.Tables["getExrAcc"].Rows[0]["accnum"];
                                }

                                addDardIntoGld["oricur"] = "SGD";
                                addDardIntoGld["oriamt"] = dr["exramt"];

                                if (Convert.ToDecimal(addDardIntoGld["oriamt"]) > 0)
                                {
                                    addDardIntoGld["oridebit"] = addDardIntoGld["oriamt"];
                                }
                                else
                                {
                                    addDardIntoGld["oricredit"] = System.Math.Abs(Convert.ToDecimal(addDardIntoGld["oriamt"]));
                                }
                                addDardIntoGld["postamt"] = dr["exramt"];
                                addDardIntoGld["lgr"] = "EXR";

                                totExramt = totExramt + Convert.ToDecimal(dr["exramt"]);
                            }
                        }
                    }

                    foreach (DataRow dr in gld.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            if (dr["lgr"] == "GLD")
                            {
                                dr["oriamt"] = Convert.ToDecimal(dr["oriamt"]) - totExramt;
                            }
                        }
                    }
                    #endregion

                    #region Steph - Core postgld does not take in the gst in saj1, thus writing codes to force gld take in the figure.
                    dbaccess.ReadSQL("gstout", "SELECT accnum FROM acc WHERE refnum= 'GST2'");
                    DataTable chkGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,oriamt FROM [gld] WHERE lgr = 'GLD'");
                    if (POSGST.Rows.Count > 0)
                    {
                        if (chkGST.Rows.Count > 0)
                        {
                            if (POSGST.Rows[0]["oriamt"] != System.DBNull.Value)
                            {
                                if (Convert.ToDecimal(chkGST.Rows[0]["oriamt"]) == Convert.ToDecimal(POSGST.Rows[0]["oriamt"]))
                                {
                                    BizFunctions.DeleteRow(gld, "lgr='GLD'");
                                    DataRow addSaj1Gst = gld.Rows.Add(new object[] { });
                                    BizFunctions.UpdateDataRow(xIVH, addSaj1Gst, "refnum/trandate/modified/created/oricur/flag/period");
                                    if (dbaccess.DataSet.Tables["gstout"].Rows.Count > 0)
                                    {
                                        addSaj1Gst["accnum"] = dbaccess.DataSet.Tables["gstout"].Rows[0]["accnum"];
                                    }

                                    addSaj1Gst["oriamt"] = POSGST.Rows[0]["oriamt"];

                                    if (Convert.ToDecimal(addSaj1Gst["oriamt"]) > 0)
                                    {
                                        addSaj1Gst["oridebit"] = addSaj1Gst["oriamt"];
                                    }
                                    else
                                    {
                                        addSaj1Gst["oricredit"] = System.Math.Abs(Convert.ToDecimal(addSaj1Gst["oriamt"]));
                                    }

                                    addSaj1Gst["postamt"] = POSGST.Rows[0]["oriamt"];
                                    addSaj1Gst["lgr"] = "SAJ1";
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (module.ToString().Trim() == "MCR")
                {
                    DataTable POSGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT sum(gstamt) AS oriamt FROM [mcr1]");

                    BizAccounts.PostGLD(dbaccess, AxDTable + "/" + table.ToLower() + "1", moduleTable, "sitenum,projectid");

                    #region Steph - Core postgld does not take in the gst in mcr1, thus writing codes to force gld take in the figure.
                    dbaccess.ReadSQL("gstout", "SELECT accnum FROM acc WHERE refnum= 'GST2'");
                    DataTable chkGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,oriamt FROM [gld] WHERE lgr = 'GLD'");
                    if (POSGST.Rows.Count > 0)
                    {
                        if (chkGST.Rows.Count > 0)
                        {
                            if (POSGST.Rows[0]["oriamt"] != System.DBNull.Value)
                            {
                                if (Convert.ToDecimal(chkGST.Rows[0]["oriamt"]) == Convert.ToDecimal(POSGST.Rows[0]["oriamt"]))
                                {
                                    BizFunctions.DeleteRow(gld, "lgr='GLD'");
                                    DataRow addMcr1Gst = gld.Rows.Add(new object[] { });
                                    BizFunctions.UpdateDataRow(xIVH, addMcr1Gst, "refnum/trandate/modified/created/oricur/flag");
                                    if (dbaccess.DataSet.Tables["gstout"].Rows.Count > 0)
                                    {
                                        addMcr1Gst["accnum"] = dbaccess.DataSet.Tables["gstout"].Rows[0]["accnum"];
                                    }

                                    addMcr1Gst["oriamt"] = POSGST.Rows[0]["oriamt"];

                                    if (Convert.ToDecimal(addMcr1Gst["oriamt"]) > 0)
                                    {
                                        addMcr1Gst["oridebit"] = addMcr1Gst["oriamt"];
                                    }
                                    else
                                    {
                                        addMcr1Gst["oricredit"] = System.Math.Abs(Convert.ToDecimal(addMcr1Gst["oriamt"]));
                                    }

                                    addMcr1Gst["postamt"] = POSGST.Rows[0]["oriamt"];
                                    addMcr1Gst["lgr"] = "MCR1";
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    BizAccounts.PostGLD(dbaccess, AxDTable + "/" + table.ToLower() + "1", moduleTable, "sitenum,projectid");
                }
            }
            #region TT 7 Sep 2010.special for SAJ voucher(only voucher payment in saj1)
            else
            {
                if (module.ToString().Trim() == "SAJ")
                {
                  string strGrpSiv1Query = "SELECT sum(oriamt) as oriamt, " +
                                                    " sum(postamt) as postamt,sum(gstamt) as gstamt,sum(oridebit) as oridebit,sum(oricredit) as oricredit, " +
                                                    " invnum,invdate,exrate FROM " +
                                                    " (SELECT (aftdeposit * -1) AS oriamt,0 postamt,0 AS gstamt,oridebit,oricredit,invnum,invdate,exrate FROM SAJ1 " +
                                                    " WHERE ISNULL(detail2,'') ='Voucher' " +
                                                    " UNION ALL " +
                                                    " SELECT oriamt,postamt,gstamt,oridebit,oricredit,invnum,invdate,exrate FROM DARD WHERE ISNULL(chkReverse,'')<>'Y' " +
                                                    " AND ISNULL(detail2,'') ='Voucher' " +
                                                    " )tmp " +
                                                    " group by invnum,invdate,exrate";
                    grpSiv1Query = BizFunctions.ExecuteQuery(dbaccess.DataSet, strGrpSiv1Query);
                    if (grpSiv1Query.Rows.Count > 0)
                    {


                        DataTable POSGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT sum(gstamt) AS oriamt FROM [saj1]");

                        BizAccounts.PostGLD(dbaccess, "SAJVOUCHER/DARD/" + table.ToLower() + "1", moduleTable, "sitenum,projectid");

                        #region Steph -  To add the exchange gain/loss from deposits

                        DataTable dard = dbaccess.DataSet.Tables["dard"];

                        decimal totExramt = 0;

                        foreach (DataRow dr in dard.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                if (dr["exramt"] != System.DBNull.Value && Convert.ToDecimal(dr["exramt"]) != 0)
                                {
                                    DataRow addDardIntoGld = gld.Rows.Add(new object[] { });
                                    BizFunctions.UpdateDataRow(xIVH, addDardIntoGld, "refnum/trandate/modified/created/oricur/flag");

                                    dbaccess.ReadSQL("getExrAcc", "SELECT accnum FROM acc WHERE refnum = 'EXR1'");
                                    if (dbaccess.DataSet.Tables["getExrAcc"].Rows.Count > 0)
                                    {
                                        addDardIntoGld["accnum"] = dbaccess.DataSet.Tables["getExrAcc"].Rows[0]["accnum"];
                                    }

                                    addDardIntoGld["oricur"] = "SGD";
                                    addDardIntoGld["oriamt"] = dr["exramt"];

                                    if (Convert.ToDecimal(addDardIntoGld["oriamt"]) > 0)
                                    {
                                        addDardIntoGld["oridebit"] = addDardIntoGld["oriamt"];
                                    }
                                    else
                                    {
                                        addDardIntoGld["oricredit"] = System.Math.Abs(Convert.ToDecimal(addDardIntoGld["oriamt"]));
                                    }
                                    addDardIntoGld["postamt"] = dr["exramt"];
                                    addDardIntoGld["lgr"] = "EXR";

                                    totExramt = totExramt + Convert.ToDecimal(dr["exramt"]);
                                }
                            }
                        }

                        foreach (DataRow dr in gld.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                if (dr["lgr"] == "GLD")
                                {
                                    dr["oriamt"] = Convert.ToDecimal(dr["oriamt"]) - totExramt;
                                }
                            }
                        }
                        #endregion

                        #region Steph - Core postgld does not take in the gst in saj1, thus writing codes to force gld take in the figure.
                        dbaccess.ReadSQL("gstout", "SELECT accnum FROM acc WHERE refnum= 'GST2'");
                        DataTable chkGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,oriamt FROM [gld] WHERE lgr = 'GLD'");
                        if (POSGST.Rows.Count > 0)
                        {
                            if (chkGST.Rows.Count > 0)
                            {
                                if (POSGST.Rows[0]["oriamt"] != System.DBNull.Value)
                                {
                                    if (Convert.ToDecimal(chkGST.Rows[0]["oriamt"]) == Convert.ToDecimal(POSGST.Rows[0]["oriamt"]))
                                    {
                                        BizFunctions.DeleteRow(gld, "lgr='GLD'");
                                        DataRow addSaj1Gst = gld.Rows.Add(new object[] { });
                                        BizFunctions.UpdateDataRow(xIVH, addSaj1Gst, "refnum/trandate/modified/created/oricur/flag/period");
                                        if (dbaccess.DataSet.Tables["gstout"].Rows.Count > 0)
                                        {
                                            addSaj1Gst["accnum"] = dbaccess.DataSet.Tables["gstout"].Rows[0]["accnum"];
                                        }

                                        addSaj1Gst["oriamt"] = POSGST.Rows[0]["oriamt"];

                                        if (Convert.ToDecimal(addSaj1Gst["oriamt"]) > 0)
                                        {
                                            addSaj1Gst["oridebit"] = addSaj1Gst["oriamt"];
                                        }
                                        else
                                        {
                                            addSaj1Gst["oricredit"] = System.Math.Abs(Convert.ToDecimal(addSaj1Gst["oriamt"]));
                                        }

                                        addSaj1Gst["postamt"] = POSGST.Rows[0]["oriamt"];
                                        addSaj1Gst["lgr"] = "SAJ1";
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion
        }

		public static void SetCorrectValue(ref DBAccess dbaccess, string moduleTable, string module)
		{

			DataTable table = dbaccess.DataSet.Tables[moduleTable];

			//if (module.ToString() == "SIV" || module.ToString() == "SDB" || module.ToString() == "SRC")
			if (module.ToString() == "SIV" || module.ToString() == "ANP" || module.ToString() == "SAJ" || module.ToString() == "MCR" || module.ToString() == "SDB" || module.ToString() == "SRC")
			{
				if (moduleTable.ToString() == "SIV1")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["qty"] = (decimal)drTable["dqty"] * -1;
							drTable["grosamt"] = (decimal)drTable["dgrosamt"] * -1;
							drTable["oriamt"] = (decimal)drTable["doriamt"] * -1;
							drTable["origstamt"] = (decimal)drTable["dorigstamt"] * -1;
							drTable["postamt"] = (decimal)drTable["dpostamt"] * -1;
							drTable["gstamt"] = (decimal)drTable["dgstamt"] * -1;
						}
					}
				}

				if (moduleTable.ToString() == "ANP1")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["qty"] = (decimal)drTable["dqty"] * -1;
							drTable["grosamt"] = (decimal)drTable["dgrosamt"] * -1;
							drTable["oriamt"] = (decimal)drTable["doriamt"] * -1;
							drTable["origstamt"] = (decimal)drTable["dorigstamt"] * -1;
							drTable["postamt"] = (decimal)drTable["dpostamt"] * -1;
							drTable["gstamt"] = (decimal)drTable["dgstamt"] * -1;
						}
					}
				}

				//Steph - For BizERP POS Sales Adjustment module.
				if (moduleTable.ToString() == "SAJ1")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							//drTable["qty"] = (decimal)drTable["dqty"] * -1;
							drTable["grosamt"] = (decimal)drTable["dgrosamt"] * -1;
							drTable["oriamt"] = (decimal)drTable["doriamt"] * -1;
							drTable["origstamt"] = (decimal)drTable["dorigstamt"] * -1;
							drTable["postamt"] = (decimal)drTable["dpostamt"] * -1;
							drTable["gstamt"] = (decimal)drTable["dgstamt"] * -1;
						}
					}
				}

				//Steph - For BizERP POS Sales Adjustment module.
				if (moduleTable.ToString() == "MCR1")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							//drTable["qty"] = (decimal)drTable["dqty"] * -1;
							drTable["grosamt"] = (decimal)drTable["dgrosamt"] * -1;
							drTable["oriamt"] = (decimal)drTable["doriamt"] * -1;
							drTable["origstamt"] = (decimal)drTable["dorigstamt"] * -1;
							drTable["postamt"] = (decimal)drTable["dpostamt"] * -1;
							drTable["gstamt"] = (decimal)drTable["dgstamt"] * -1;
						}
					}
				}

				if (moduleTable.ToString() == "ARD")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["oriamt"] = (decimal)drTable["doriamt"] * -1;
							drTable["origstamt"] = (decimal)drTable["dorigstamt"] * -1;
							drTable["postamt"] = (decimal)drTable["dpostamt"] * -1;
							drTable["gstamt"] = (decimal)drTable["dgstamt"] * -1;

							if ((decimal)drTable["oriamt"] > 0)
							{
								drTable["oridebit"] = System.Math.Abs((decimal)drTable["oriamt"]);
								drTable["oricredit"] = 0;
							}
							else
							{
								drTable["oridebit"] = 0;
								drTable["oricredit"] = System.Math.Abs((decimal)drTable["oriamt"]);
							}
						}
					}
				}

				if (moduleTable.ToString() == "RECP")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["bankamt"] = (decimal)drTable["dbankamt"];
							drTable["bankchg"] = (decimal)drTable["dbankchg"] * -1;
						}
					}
				}
			}

			if (module.ToString() == "SCR")
			{
				if (moduleTable.ToString() == "SIV1")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["qty"] = (decimal)drTable["dqty"];
							drTable["grosamt"] = (decimal)drTable["dgrosamt"];
							drTable["oriamt"] = (decimal)drTable["doriamt"];
							drTable["origstamt"] = (decimal)drTable["dorigstamt"];
							drTable["postamt"] = (decimal)drTable["dpostamt"];
							drTable["gstamt"] = (decimal)drTable["dgstamt"];
						}
					}
				}
			}


			if (module.ToString() == "PIV" || module.ToString() == "PDB" || module.ToString() == "PAY" || module.ToString() == "GIV")
			{
				if (moduleTable.ToString() == "PIV1")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["qty"] = (decimal)drTable["dqty"];
							drTable["grosamt"] = (decimal)drTable["dgrosamt"];
							drTable["oriamt"] = (decimal)drTable["doriamt"];
							drTable["origstamt"] = (decimal)drTable["dorigstamt"];
							drTable["postamt"] = (decimal)drTable["dpostamt"];
							drTable["gstamt"] = (decimal)drTable["dgstamt"];
						}
					}
				}

                if (moduleTable.ToString() == "GIV1")
                {
                    foreach (DataRow drTable in table.Rows)
                    {
                        if (drTable.RowState != DataRowState.Deleted)
                        {
                            drTable["qty"] = (decimal)drTable["dqty"];
                            drTable["grosamt"] = (decimal)drTable["dgrosamt"];
                            drTable["oriamt"] = (decimal)drTable["doriamt"];
                            drTable["origstamt"] = (decimal)drTable["dorigstamt"];
                            drTable["postamt"] = (decimal)drTable["dpostamt"];
                            drTable["gstamt"] = (decimal)drTable["dgstamt"];
                        }
                    }
                }
				if (moduleTable.ToString() == "APD")
				{
						foreach (DataRow drTable in table.Rows)
						{
							if (drTable.RowState != DataRowState.Deleted)
							{
								drTable["oriamt"] = (decimal)drTable["doriamt"];
								drTable["origstamt"] = (decimal)drTable["dorigstamt"];
								drTable["postamt"] = (decimal)drTable["dpostamt"];
								drTable["gstamt"] = (decimal)drTable["dgstamt"];
							}
						}
				}
				if (moduleTable.ToString() == "PAYT")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["bankamt"] = (decimal)drTable["dbankamt"] * -1;
							drTable["bankchg"] = (decimal)drTable["dbankchg"] * -1;
						}
					}
				}
			}

			if (module.ToString() == "PCR")
			{
				if (moduleTable.ToString() == "PIV1")
				{
					foreach (DataRow drTable in table.Rows)
					{
						if (drTable.RowState != DataRowState.Deleted)
						{
							drTable["qty"] = (decimal)drTable["dqty"] * -1;
							drTable["grosamt"] = (decimal)drTable["dgrosamt"] * -1;
							drTable["oriamt"] = (decimal)drTable["doriamt"] * -1;
							drTable["origstamt"] = (decimal)drTable["dorigstamt"] * -1;
							drTable["postamt"] = (decimal)drTable["dpostamt"] * -1;
							drTable["gstamt"] = (decimal)drTable["dgstamt"] * -1;
						}
					}
				}
			}



		}

		public static void SetDefaultValueNVC(ref DBAccess dbaccess, string flag, string page, string field)
		{
			DataTable table = dbaccess.DataSet.Tables[page];
			dbaccess.ReadSQL("getDefault", "select flag,page,field,value from defaultvalue where flag = '" + flag + "' and page = '" + page + "' and field ='" + field + "'");
			if (dbaccess.DataSet.Tables["getDefault"].Select().Length > 0)
			{
				foreach (DataRow drTable in table.Rows)
				{
					if (drTable.RowState != DataRowState.Deleted)
					{
						if (drTable[field] == System.DBNull.Value || drTable[field].ToString().Trim() == string.Empty)
						{
							drTable[field] = dbaccess.DataSet.Tables["getDefault"].Rows[0]["value"];
						}
					}
				}				
			}		
		}

		public static void updateAccountCsh(ref DBAccess dbaccess, string columns, string moduleTable, string module, string headerTable, string CshTable, string xIVTable, string TDTName)
		{
			DataTable Csh = dbaccess.DataSet.Tables[CshTable];
			DataRow xRecPay = dbaccess.DataSet.Tables[headerTable].Rows[0];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			#region Steph - To post the correct amount to Csh db first.
											
			string[] StrColumn = columns.Split('|');

				if (StrColumn.Length > 0)
				{
					DataRow tmpCsh = Csh.NewRow();

					for (int i = 0; i < StrColumn.Length; i++)
					{
						switch (StrColumn[i])
						{
							case "apnum":
								tmpCsh["apnum"] = xRecPay["apnum"];
								break;
							case "exramt":
								tmpCsh["exramt"] = 0;
								break;
							case "arnum":
								tmpCsh["arnum"] = xRecPay["arnum"];
								break;
							case "chequeno":
								tmpCsh["chequeno"] = xRecPay["chequeno"];
								break;
							case "chknum":
								tmpCsh["chknum"] = xRecPay["chknum"];
								break;
							case "locno":
								tmpCsh["locno"] = xRecPay["locno"];
								break;
							case "deptno":
								tmpCsh["deptno"] = xRecPay["deptno"];
								break;
							case "coy":
								tmpCsh["coy"] = xRecPay["coy"];
								break;
							case "period":
								tmpCsh["period"] = xRecPay["period"];
								break;
							case "accnum":
								tmpCsh["accnum"] = xRecPay["accnum1"];
								break;
							case "bankamt":
								tmpCsh["bankamt"] = xRecPay["bankamt"];
								break;
							case "trandate":
								tmpCsh["trandate"] = xRecPay["trandate"];
								break;
							case "lgr":
								tmpCsh["lgr"] = CshTable.ToString().Trim().ToUpper();
								break;
							case "exrate":
								if (xRecPay["exrate"] == System.DBNull.Value) 
								{
									MessageBox.Show("Please check the Ex-Rate in Header page!");
									xRecPay["exrate"] = 0;
								}
								tmpCsh["exrate"] = Convert.ToDecimal(xRecPay["exrate"]);
								break;
							case "oricur":
								tmpCsh["oricur"] = xRecPay["oricur"];
								break;
							case "oriamt":
								if (module.ToString().Trim() == "ARD")
								{
									//if (xRecPay["bankamt"] == System.DBNull.Value)
									//{
									//    MessageBox.Show("Please check the Bank-Amt in Header page!");
									//    xRecPay["bankamt"] = 0;
									//}
									//if (xRecPay["bankchg"] == System.DBNull.Value)
									//{
									//    MessageBox.Show("Please check the Bank-Charge in Header page!");
									//    xRecPay["bankchg"] = 0;
									//}
									tmpCsh["oriamt"] = (Convert.ToDecimal(xRecPay["dbankamt"]) - Convert.ToDecimal(xRecPay["dbankchg"]));
								}
								if (module.ToString().Trim() == "APD")
								{
									//if (xRecPay["bankamt"] == System.DBNull.Value)
									//{
									//    MessageBox.Show("Please check the Bank-Amt in Header page!");
									//    xRecPay["bankamt"] = 0;
									//}
									tmpCsh["oriamt"] = (Convert.ToDecimal(xRecPay["dbankamt"]) + Convert.ToDecimal(xRecPay["dbankchg"])) * -1;
								}
								break;
							case "postamt":
								if (module.ToString().Trim() == "ARD")
								{
									//if (xRecPay["bankamt"] == System.DBNull.Value)
									//{
									//    MessageBox.Show("Please check the Bank-Amt in Header page!");
									//    xRecPay["bankamt"] = 0;
									//}
									tmpCsh["postamt"] = BizFunctions.Round((Convert.ToDecimal(xRecPay["dbankamt"]) - Convert.ToDecimal(xRecPay["dbankchg"])) * (Convert.ToDecimal(xRecPay["exrate"])));
								}
								if (module.ToString().Trim() == "APD")
								{
									//if (xRecPay["bankamt"] == System.DBNull.Value)
									//{
									//    MessageBox.Show("Please check the Bank-Amt in Header page!");
									//    xRecPay["bankamt"] = 0;
									//}
									tmpCsh["postamt"] = BizFunctions.Round((Convert.ToDecimal(xRecPay["dbankamt"]) + Convert.ToDecimal(xRecPay["dbankchg"]) ) * (Convert.ToDecimal(xRecPay["exrate"])) * -1);
								}
								break;
							case "oridebit":
									if ((Convert.ToDecimal(xRecPay["bankamt"]) + Convert.ToDecimal(xRecPay["bankchg"]) > 0))
									{
										tmpCsh["oridebit"] = System.Math.Abs(Convert.ToDecimal(xRecPay["bankamt"]) + Convert.ToDecimal(xRecPay["bankchg"]));
									}
									if ((Convert.ToDecimal(xRecPay["bankamt"]) + Convert.ToDecimal(xRecPay["bankchg"]) < 0))
									{
										tmpCsh["oridebit"] = 0;
									}							
								break;
							case "oricredit":
									if ((Convert.ToDecimal(xRecPay["bankamt"]) - Convert.ToDecimal(xRecPay["bankchg"]) > 0))
									{
										tmpCsh["oricredit"] = 0.00;
									}
									if ((Convert.ToDecimal(xRecPay["bankamt"]) - Convert.ToDecimal(xRecPay["bankchg"]) < 0))
									{
										tmpCsh["oricredit"] = System.Math.Abs(Convert.ToDecimal(xRecPay["bankamt"]) + Convert.ToDecimal(xRecPay["bankchg"]));
									}							
								break;
							case "detail":
								tmpCsh["detail"] = xRecPay["remark"];
								break;

							default:
								tmpCsh[StrColumn[i]] = xRecPay[StrColumn[i]];
								break;
						}
					}
				
					Csh.Rows.Add(tmpCsh.ItemArray);
				}
				#region steph - giv posting
				DataTable Piv = dbaccess.DataSet.Tables["giv1"];

				string bankchgColumn = "refnum|accnum|exrate|lgr|postamt|coy|oriamt|oricur|period|oricredit|oridebit|discallow|locno|deptno";
				string[] bankchgStrColumn = bankchgColumn.Split('|');

					if ((Convert.ToDecimal(xRecPay["dbankchg"])) != 0)
						if (bankchgStrColumn.Length > 0)
						{
							DataRow tmpPiv = Piv.NewRow();

							for (int i = 0; i < bankchgStrColumn.Length; i++)
							{
								switch (bankchgStrColumn[i])
								{
									case "apnum":
										tmpPiv["apnum"] = xRecPay["apnum"];
										break;
									case "arnum":
										tmpPiv["arnum"] = xRecPay["arnum"];
										break;
									case "locno":
										tmpPiv["locno"] = xRecPay["locno"];
										break;
									case "deptno":
										tmpPiv["deptno"] = xRecPay["deptno"];
										break;
									case "discallow":
										tmpPiv["discallow"] = "Y";
										break;
									case "coy":
										tmpPiv["coy"] = xRecPay["coy"];
										break;
									case "period":
										tmpPiv["period"] = xRecPay["period"];
										break;
									case "accnum":
										tmpPiv["accnum"] = xRecPay["accnum2"];
										break;
									case "trandate":
										tmpPiv["trandate"] = DateTime.Today;
										break;
									case "lgr":
										tmpPiv["lgr"] = "PIV";
										break;
									case "exrate":
										if (xRecPay["exrate"] == System.DBNull.Value)
										{
											MessageBox.Show("Please check the Ex-Rate in Header page!");
											xRecPay["exrate"] = 0;
										}
										tmpPiv["exrate"] = Convert.ToDecimal(xRecPay["exrate"]);
										break;
									case "oricur":
										tmpPiv["oricur"] = xRecPay["oricur"];
										break;
									case "oriamt":
										if (module.ToString().Trim() == "ARD")
										{
											if (xRecPay["bankchg"] == System.DBNull.Value)
											{
												MessageBox.Show("Please check the Bank-Amt in Header page!");
												xRecPay["bankchg"] = 0;
											}
											tmpPiv["oriamt"] = (Convert.ToDecimal(xRecPay["dbankchg"]));
										}
										if (module.ToString().Trim() == "APD")
										{
											if (xRecPay["bankchg"] == System.DBNull.Value)
											{
												MessageBox.Show("Please check the Bank-Amt in Header page!");
												xRecPay["bankchg"] = 0;
											}
											tmpPiv["oriamt"] = (Convert.ToDecimal(xRecPay["dbankchg"]));
										}
										break;
									case "postamt":
										if (module.ToString().Trim() == "ARD")
										{
											if (xRecPay["bankchg"] == System.DBNull.Value)
											{
												MessageBox.Show("Please check the Bank-Amt in Header page!");
												xRecPay["bankchg"] = 0;
											}
											tmpPiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(xRecPay["dbankchg"])) * (Convert.ToDecimal(xRecPay["exrate"])));
										}
										if (module.ToString().Trim() == "APD")
										{
											if (xRecPay["bankchg"] == System.DBNull.Value)
											{
												MessageBox.Show("Please check the Bank-Amt in Header page!");
												xRecPay["bankchg"] = 0;
											}
											tmpPiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(xRecPay["dbankchg"])) * (Convert.ToDecimal(xRecPay["exrate"])));
										}
										break;
									case "oridebit":
										tmpPiv["oridebit"] = (Convert.ToDecimal(xRecPay["dbankchg"]));
										break;
									case "oricredit":
										tmpPiv["oricredit"] = 0.00;
										break;

									default:
										tmpPiv[StrColumn[i]] = xRecPay[bankchgStrColumn[i]];
										break;
								}
							}
							Piv.Rows.Add(tmpPiv.ItemArray);
						}
				#endregion

			#endregion Steph - To post the correct amount to Csh db first.
				if (headerTable.ToString().Trim() == "RECP")
				{
					BizAccounts.PostGLD(dbaccess, "GSTTABLE/DARD/SAJ1/"+CshTable + "/" + module.ToLower() + "/" + xIVTable.ToLower() + "/giv1", headerTable, "projectid,sitenum");

					DataTable POSGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT sum(gstamt) AS oriamt FROM [saj1]");

					#region Steph - Core postgld does not take in the gst in saj1, thus writing codes to force gld take in the figure.
					dbaccess.ReadSQL("gstout", "SELECT accnum FROM acc WHERE refnum= 'GST2'");
					DataTable chkGST = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,oriamt FROM [gld] WHERE lgr = 'GLD'");
					if (POSGST.Rows.Count > 0)
					{
						if (chkGST.Rows.Count > 0)
						{
							if (POSGST.Rows[0]["oriamt"] != System.DBNull.Value)
							{
								if (Convert.ToDecimal(chkGST.Rows[0]["oriamt"]) == Convert.ToDecimal(POSGST.Rows[0]["oriamt"]))
								{
									BizFunctions.DeleteRow(gld, "lgr='GLD'");
									DataRow addSaj1Gst = gld.Rows.Add(new object[] { });
									BizFunctions.UpdateDataRow(xRecPay, addSaj1Gst, "refnum/trandate/modified/created/oricur/flag");
									if (dbaccess.DataSet.Tables["gstout"].Rows.Count > 0)
									{
										addSaj1Gst["accnum"] = dbaccess.DataSet.Tables["gstout"].Rows[0]["accnum"];
									}

									addSaj1Gst["oriamt"] = POSGST.Rows[0]["oriamt"];

									if (Convert.ToDecimal(addSaj1Gst["oriamt"]) > 0)
									{
										addSaj1Gst["oridebit"] = addSaj1Gst["oriamt"];
									}
									else
									{
										addSaj1Gst["oricredit"] = System.Math.Abs(Convert.ToDecimal(addSaj1Gst["oriamt"]));
									}

									addSaj1Gst["postamt"] = POSGST.Rows[0]["oriamt"];
									addSaj1Gst["lgr"] = "SAJ1";
								}
							}
						}
					}
					#endregion
				}
				if (headerTable.ToString().Trim() == "PAYT")
				{
                    BizAccounts.PostGLD(dbaccess, CshTable + "/" + module.ToLower() + "/" + xIVTable.ToLower() + "/siv1", headerTable, "projectid,sitenum");
				}
		}

		public static string[] GetInvoicetype(DBAccess dbaccess, string[] str, string module, string moduleTable, Hashtable Formscollection)
		{
			DataRow xIVH = dbaccess.DataSet.Tables[module + "H"].Rows[0];
			ComboBox xIVType = BizXmlReader.CurrentInstance.GetControl((Formscollection["header"] as Form).Name, moduleTable + "h_" + module.ToLower() + "type") as ComboBox;
			
			DataSet Ds = dbaccess.ReadSQLTemp("TDTH", "select selectClaus from tdth where tdtname='" + xIVType.Text + "'");

			try
			{
				if (str != null)
				{
					for (int i = 0; i < str.Length; i++)
					{
						Control[] Scntrl = (Formscollection["header"] as Form).Controls.Find(moduleTable + "h_" + str[i].ToLower(), true);
						if (Scntrl.Length > 0)
						{
							Scntrl[0].Enabled = false;
						}
					}

					str = null;
				}

				if (Ds.Tables["TDTH"].Rows.Count > 0)
				{
					string[] selectClaus = Ds.Tables["TDTH"].Rows[0][0].ToString().Split('|');
					for (int i = 0; i < selectClaus.Length; i++)
					{
						Control[] cntrl = (Formscollection["header"] as Form).Controls.Find(moduleTable + "h_" + selectClaus[i].ToLower(), true);
						if (cntrl.Length > 0)
						{
							cntrl[0].Enabled = true;
						}
					}
					str = selectClaus;
				}

				return str;
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
				return str;
			}

		}

		public static DataRow updateDataRow(DBAccess dbaccess, string updateColumns, DataRow FromTable, DataRow ToTable)
		{
			string[] str = updateColumns.Split('|');
			
			try
			{
				if (str.Length > 0)
				{
					for (int i = 0; i < str.Length; i++)
					{
						FromTable[str[i]] = ToTable[str[i]];
					}

				}

				return FromTable;
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.ToString());
				return FromTable;
			}
		}

		public static Hashtable GetExtraction(string MDTName, string TDTName, int detailTab, DBAccess dbAccess)
		{
			DBAccess dbaccess = new DBAccess();
			Hashtable Hs = new Hashtable();

			//Get MDT Refnum
			DataSet DsMDTH = dbaccess.ReadSQLTemp("MDTH", "Select refnum from MDTH where mdtname='" + MDTName + "'");
			string refnum = DsMDTH.Tables["MDTH"].Rows[0]["refnum"].ToString().Trim();

			DataSet DsMDT1 = dbaccess.ReadSQLTemp("MDT1", "Select tdtnum from MDT1 where refnum='" + refnum + "' and tdtname='" + TDTName + "'");
			string TDT = DsMDT1.Tables["MDT1"].Rows[0][0].ToString().Trim();
			string stephTest = "Select DetailGridnum,DestinationTable,colDisplay,colCopy,sqlDisplay,sqlCopy,extractkey,inclextracted from TDT6 where DetailGridnum='" + detailTab + "' and tdtnum='" + TDT + "'";
			DataSet DsTDT6 = dbaccess.ReadSQLTemp("TDT6", "Select DetailGridnum,DestinationTable,colDisplay,colCopy,sqlDisplay,sqlCopy,extractkey,inclextracted from TDT6 where DetailGridnum='" + detailTab + "' and tdtnum='" + TDT + "'");

				if (DsTDT6.Tables["TDT6"].Rows.Count > 0)
				{

					Hs.Add("DetailGridnum", DsTDT6.Tables["TDT6"].Rows[0][0].ToString().Trim());
					Hs.Add("DestinationTable", DsTDT6.Tables["TDT6"].Rows[0][1].ToString().Trim());

					Hs.Add("colDisplay", DsTDT6.Tables["TDT6"].Rows[0][2].ToString().Trim());
					Hs.Add("colCopy", DsTDT6.Tables["TDT6"].Rows[0][3].ToString().Trim());
					string sql;
					string subStr;
					string[] split;
					string sql0;
					string sql1;

					//STEPH-TAKE SYSTEM YEAR - 14 Apr 2010
					DsTDT6.Tables["TDT6"].Rows[0][4] = DsTDT6.Tables["TDT6"].Rows[0][4].ToString().Replace("!YEAR!", Common.DEFAULT_SYSTEM_YEAR.ToString().Trim());

					if (DsTDT6.Tables["TDT6"].Rows[0][4].ToString().Contains("{"))
					{
						subStr = DsTDT6.Tables["TDT6"].Rows[0][4].ToString().Substring(DsTDT6.Tables["TDT6"].Rows[0][4].ToString().IndexOf('{'), DsTDT6.Tables["TDT6"].Rows[0][4].ToString().IndexOf('}') - DsTDT6.Tables["TDT6"].Rows[0][4].ToString().IndexOf('{') + 1);
						split = subStr.Split('.');

						sql0 = split[0].Substring(1, split[0].Length - 1);
						sql1 = split[1].Substring(0, split[1].Length - 1);

						sql = DsTDT6.Tables["TDT6"].Rows[0][4].ToString().Substring(0,DsTDT6.Tables["TDT6"].Rows[0][4].ToString().IndexOf('{') - 1) + " '" + 
							dbAccess.DataSet.Tables[sql0].Rows[0][sql1].ToString().Trim() + "' " + DsTDT6.Tables["TDT6"].Rows[0][4].ToString().Substring(DsTDT6.Tables["TDT6"].Rows[0][4].ToString().IndexOf('}') + 1);

                        #region TT.20100916.SRC extract,display detail for S/R Order of POS
                        if (sql.Contains("{"))
                        {
                            string subStr1 = sql.Substring(sql.IndexOf('{'), sql.IndexOf('}') - sql.IndexOf('{') + 1);
                            string[] split1 = subStr1.Split('.');

                            string sql2 = split1[0].Substring(1, split[0].Length - 1);
                            string sql3 = split1[1].Substring(0, split[1].Length - 1);

                            sql = sql.Substring(0, sql.IndexOf('{') - 1) + " '" +
                                  dbAccess.DataSet.Tables[sql0].Rows[0][sql1].ToString().Trim() + "' " + sql.Substring(sql.IndexOf('}') + 1);

                        }
                        #endregion
                        //MessageBox.Show(sql);
					}
					else
					{
						sql = DsTDT6.Tables["TDT6"].Rows[0][4].ToString().Trim();
					}

					Hs.Add("sqlDisplay", sql);

					//STEPH - TAKE SYSTEM YEAR - 14 Apr 2010
					DsTDT6.Tables["TDT6"].Rows[0][5] = DsTDT6.Tables["TDT6"].Rows[0][5].ToString().Replace("!YEAR!", Common.DEFAULT_SYSTEM_YEAR.ToString().Trim());

					if (DsTDT6.Tables["TDT6"].Rows[0][5].ToString().Contains("{"))
					{
						subStr = DsTDT6.Tables["TDT6"].Rows[0][5].ToString().Substring(DsTDT6.Tables["TDT6"].Rows[0][5].ToString().IndexOf('{'), DsTDT6.Tables["TDT6"].Rows[0][5].ToString().IndexOf('}') - DsTDT6.Tables["TDT6"].Rows[0][5].ToString().IndexOf('{') + 1);
						split = subStr.Split('.');

						sql0 = split[0].Substring(1, split[0].Length - 1);
						sql1 = split[1].Substring(0, split[1].Length - 1);

						sql = DsTDT6.Tables["TDT6"].Rows[0][5].ToString().Substring(0, DsTDT6.Tables["TDT6"].Rows[0][5].ToString().IndexOf('{') - 1) + " '" +
							dbAccess.DataSet.Tables[sql0].Rows[0][sql1].ToString().Trim() + "' " + DsTDT6.Tables["TDT6"].Rows[0][5].ToString().Substring(DsTDT6.Tables["TDT6"].Rows[0][5].ToString().IndexOf('}') + 1);

                        #region TT.20100916.SRC extract,display detail for S/R Order of POS
                        if (sql.Contains("{"))
                        {
                            string subStr1 = sql.Substring(sql.IndexOf('{'), sql.IndexOf('}') - sql.IndexOf('{') + 1);
                            string[] split1 = subStr1.Split('.');

                            string sql2 = split1[0].Substring(1, split[0].Length - 1);
                            string sql3 = split1[1].Substring(0, split[1].Length - 1);

                            sql = sql.Substring(0, sql.IndexOf('{') - 1) + " '" +
                                  dbAccess.DataSet.Tables[sql0].Rows[0][sql1].ToString().Trim() + "' " + sql.Substring(sql.IndexOf('}') + 1);

                        }
                        #endregion

						//MessageBox.Show(sql);
					}
					else
					{
						sql = DsTDT6.Tables["TDT6"].Rows[0][5].ToString().Trim();
					}

					Hs.Add("sqlCopy", sql);

					Hs.Add("extractkey", DsTDT6.Tables["TDT6"].Rows[0][6].ToString().Trim());
					Hs.Add("inclextracted", DsTDT6.Tables["TDT6"].Rows[0][7].ToString().Trim());
				}
				return Hs;


		}

		public static void updateMWT(ref DBAccess dbaccess, string columns, string detailPage, string warehouse)
		{
			
			DataTable det = dbaccess.DataSet.Tables[detailPage];
			DataTable whouse = dbaccess.DataSet.Tables[warehouse];

				foreach (DataRow dr in det.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						string[] StrColumn = columns.Split('|');

						if (StrColumn.Length > 0)
						{
							DataRow wh= whouse.NewRow();

							for (int i = 0; i < StrColumn.Length; i++)
							{
								switch (StrColumn[i])
								{
									case "refnum":
										wh["refnum"] = dr["refnum"];
										break;
                                    case "location":
                                        wh["location"] = dr["location"];
										break;
                                    case "whnum":
                                        wh["whnum"] = dr["whnum"];
                                        break;
									case "matnum":
										wh["matnum"] = dr["matnum"];
										break;
									case "docunum":
										wh["docunum"] = dr["docunum"];
										break;
									case "donum":
										wh["donum"] = dr["refnum"];
										break;
									case "grnum":
										wh["grnum"] = dr["refnum"];
										break;
									case "whcode":
										wh["whcode"] = dr["whcode"];
										break;
									case "qty":
										wh["qty"] = dr["qty"];
										break;
									case "stdcost":
										wh["stdcost"] = dr["stdcost"];
										break;
                                    case "discamt":
                                        wh["discamt"] = dr["discamt"];
                                        break;
									case "costamt":
										wh["costamt"] = dr["costamt"];
										break;
									case "uom":
										wh["uom"] = dr["uom"];
										break;
									case "apnum":
										wh["apnum"] = dr["apnum"];
										break;
									case "stkdate":
										if(detailPage.ToUpper() == "GRN1")
										wh["stkdate"] = dr["trandate"];
									break;
									case "period":
										wh["period"] = dr["period"];
										break;
									case "user":
										wh["user"] = dr["user"];
										break;
									case "flag":
										wh["flag"] = dr["flag"];
										break;
									case "status":
										wh["status"] = "P";
										break;
									case "created":
										wh["created"] = dr["created"];
										break;
									case "modified":
										wh["modified"] = dr["modified"];
										break;
									case "trandate":
										wh["trandate"] = dr["trandate"];
										break;
                                    case "price":
                                        wh["price"] = dr["price"];
                                        break;
                                    case "projectid":
                                        wh["projectid"] = dr["projectid"];
                                        break;
									default:
										wh[StrColumn[i]] = dr[StrColumn[i]];
										break;
								}
							}
							whouse.Rows.Add(wh.ItemArray);
						}
					}
					
				}
			}

	}
}