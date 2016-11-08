using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Data.SqlTypes;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Net.Mail;
using System.Threading;
using System.Globalization; 
using System.Text.RegularExpressions;
using System.Net;


namespace ATL.BizLogicTools
{
     
	/// <summary>
	/// Summary description for Tools.
	/// </summary>
	public class Tools
	{

        public Tools()
        {
          
        }

		#region CheckDigit
        /// <summary>
        /// 
        /// Used in Voucher_VIP.cs
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
		public static int CheckDigit(Int64 number)
		{
			char[] digits = number.ToString().ToCharArray();
			int oddSum = 0;
			int evenSum = 0;

			for (int i = 0; i < digits.Length; i += 2)
			{
				oddSum += int.Parse(digits[i].ToString());

				if (i + 1 < digits.Length)
				{
					evenSum += int.Parse(digits[i + 1].ToString());
				}
			}

			return (10 - ((3 * oddSum + evenSum) % 10)) % 10;
		}

		#endregion

		#region FixGldDetails

		public static void FixGldDetails(DBAccess dbaccess)
		{
			if (dbaccess.DataSet.Tables.Contains("gld"))
			{
				DataTable gld = dbaccess.DataSet.Tables["gld"];
				foreach (DataRow dr in gld.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						if (!Convert.IsDBNull(dr["lgr"]))
						{
							switch (dr["lgr"].ToString().Trim().ToUpper())
							{
								case "SIV1":
									if (dbaccess.DataSet.Tables.Contains("SIVH"))
									{
										//DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT MAX(detail) AS detail FROM [siv1] WHERE accnum='" + dr["accnum"].ToString().Trim() + "' AND oricur='" + dr["oricur"].ToString().Trim().Replace("'", "") + "' AND refnum='" + dr["refnum"].ToString().Trim() + "' GROUP BY accnum,oricur,refnum");
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [SIVH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}
									break;

								case "ARD":
									if (dbaccess.DataSet.Tables.Contains("SIVH"))
									{
										//DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT MAX(detail) AS detail FROM [siv1] WHERE accnum='" + dr["accnum"].ToString().Trim() + "' AND oricur='" + dr["oricur"].ToString().Trim().Replace("'", "") + "' AND refnum='" + dr["refnum"].ToString().Trim() + "' GROUP BY accnum,oricur,refnum");
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [SIVH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}

                                    // Payt contains both givh and payt. Remove the else. So that bottom overwrites the top.
                                    if (dbaccess.DataSet.Tables.Contains("RECP"))
                                    {
                                        DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT DETAIL FROM [RECP]");
                                        if (dtTmp.Rows.Count != 0)
                                        {
                                            dr["detail"] = dtTmp.Rows[0]["detail"];
                                        }
                                    }

									break;

								case "GLD":
									if (dbaccess.DataSet.Tables.Contains("SIV1"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT MAX(detail) AS detail FROM [siv1] WHERE accnum='" + dr["accnum"].ToString().Trim() + "' AND oricur='" + dr["oricur"].ToString().Trim().Replace("'", "") + "' AND refnum='" + dr["refnum"].ToString().Trim() + "' GROUP BY accnum,oricur,refnum");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["detail"];
										}
									}
									break;

								case "PIV1":
									if (dbaccess.DataSet.Tables.Contains("PIVH"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [PIVH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}
									else
									if (dbaccess.DataSet.Tables.Contains("PCSH"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [PCSH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}
									else
									if (dbaccess.DataSet.Tables.Contains("CIVH"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [CIVH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}

                                    // Payt bank charges post to giv1 and contains both givh and payt. This is so that bottom overwrites the top.
                                    if (dbaccess.DataSet.Tables.Contains("PAYT"))
                                    {
                                        DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT DETAIL FROM [PAYT]");
                                        if (dtTmp.Rows.Count != 0)
                                        {
                                            dr["detail"] = dtTmp.Rows[0]["detail"];
                                        }
                                    }
									break;

								case "CSH":
									if (dbaccess.DataSet.Tables.Contains("PCSH"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [PCSH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}

									if (dbaccess.DataSet.Tables.Contains("CIVH"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [CIVH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}

									if (dbaccess.DataSet.Tables.Contains("PAYT"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT DETAIL FROM [PAYT]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["detail"];
										}
									}
                                    else if (dbaccess.DataSet.Tables.Contains("RECP"))
                                    {
                                        DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT DETAIL FROM [RECP]");
                                        if (dtTmp.Rows.Count != 0)
                                        {
                                            dr["detail"] = dtTmp.Rows[0]["detail"];
                                        }
                                    }
                                    
									break;

								case "APD":
									if (dbaccess.DataSet.Tables.Contains("PIVH"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT REMARKS FROM [PIVH]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["remarks"];
										}
									}

                                    // Payt contains both givh and payt. Remove the else. So that bottom overwrites the top.
									if (dbaccess.DataSet.Tables.Contains("PAYT"))
									{
										DataTable dtTmp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT DETAIL FROM [PAYT]");
										if (dtTmp.Rows.Count != 0)
										{
											dr["detail"] = dtTmp.Rows[0]["detail"];
										}
									}
									break;


								default:
									break;
							}
						}
					}
				}
			}
		}

		#endregion
       
        #region setDefaults

        public static void setDefaults(DataSet dataSet, string tableNames)
		{
			string[] tables = tableNames.Split(new char[] {'/','\\'});
			if (ConfigurationManager.AppSettings.Get("AutoDefault").ToLower() == "true")
			{
				for (int i = 0; i < tables.Length; i++)
				{
					DataTable dt = dataSet.Tables[tables[i]];

                    if (dt.Rows.Count != 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    switch (dc.DataType.ToString())
                                    {
                                        // All decimals are 0 by default
                                        case "System.Decimal":
                                            if (dr[dc.ColumnName] == System.DBNull.Value)
                                                dr[dc.ColumnName] = 0;
                                            break;

                                        // All smallints are 0 by default
                                        case "System.Int16":
                                            if (dr[dc.ColumnName] == System.DBNull.Value)
                                                dr[dc.ColumnName] = 0;
                                            break;

                                        // All ints are 0 by default
                                        case "System.Int32":
                                            if (dr[dc.ColumnName] == System.DBNull.Value)
                                                dr[dc.ColumnName] = 0;
                                            break;

                                        // All bigints are 0 by default but do not touch ID
                                        case "System.Int64":
                                            if (dr[dc.ColumnName] == System.DBNull.Value && dc.ColumnName != "ID")
                                                dr[dc.ColumnName] = 0;
                                            break;

                                        // All bits are 0 by default
                                        case "System.Bit":
                                            if (dr[dc.ColumnName] == System.DBNull.Value)
                                                dr[dc.ColumnName] = 0;
                                            break;

                                        // All booleans are false by default
                                        case "System.Boolean":
                                            if (dr[dc.ColumnName] == System.DBNull.Value)
                                                dr[dc.ColumnName] = false;
                                            break;

                                        // Trim white spaces due to user entry
                                        case "System.String":
                                            if (dr[dc.ColumnName] != System.DBNull.Value)
                                                dr[dc.ColumnName] = (dr[dc.ColumnName] as String).Trim();
                                            break;
                                    }
                                }
                            }
                        }
                    }
				}
			}
        }

        #endregion

        #region getSupplierPrice

        public static decimal getSupplierPrice(DBAccess dbaccess, DateTime date, string matnum)
		{
			#region Retrieve supplier price

			// To retrieve supplier price 
			// Get the latest effective date for all suppliers.
			// If there is defaultapnum, then use price from latest effective date from that supplier.
			// Else just use the latest effective date amongst the suppliers.

			// Retrieve the effective supplier price and supplier for this item
			string command = "select appricelocal, apnum, effectivefrom from matm3 where matnum = '" + matnum + "' and " +
				"effectivefrom <= '" + BizFunctions.GetStandardDateString(date) + "' order by effectivefrom desc";
			dbaccess.ReadSQL("matm3tmp", command);

			decimal appricelocal = 0; 
			int rowsNum = dbaccess.DataSet.Tables["matm3tmp"].Rows.Count;
			if (rowsNum != 0)
			{
				DataTable matm3tmp = dbaccess.DataSet.Tables["matm3tmp"];
				if (rowsNum > 1)
				{
					if (((DateTime)matm3tmp.Rows[0]["effectivefrom"]).ToShortDateString() == ((DateTime)matm3tmp.Rows[1]["effectivefrom"]).ToShortDateString())
					{
						command = "select defaultapnum from matm where matnum = '" + matnum + "'";
						dbaccess.ReadSQL("matmtmp", command);

						DataRow matm = dbaccess.DataSet.Tables["matmtmp"].Rows[0];

						// If there is a defaultapnum
						if (!BizFunctions.IsEmpty(matm["defaultapnum"]))
						{
							string defApnum = matm["defaultapnum"].ToString();
							foreach (DataRow dataRow in matm3tmp.Select("apnum = '" + defApnum + "'"))
							{
								appricelocal = (decimal)dataRow["appricelocal"];
							}
						}
					}
					else
					{
						appricelocal = (decimal)matm3tmp.Rows[0]["appricelocal"];
					}
				}
				else
				{
					appricelocal = (decimal)matm3tmp.Rows[0]["appricelocal"];
				}
			#endregion Retrieve supplier price
			}

			return appricelocal;
        }

        #endregion

        #region GetGUID

        public static string getGUID()
		{
			System.Guid guid = System.Guid.NewGuid();

			String g = guid.ToString();
			g = g.Replace("-", "");
			return g;
            
		}
		#endregion

        #region Steph - Get Period From PD
        public static int GetPd(DBAccess dbaccess, DateTime trandate)
        {
            string strGetPd = "SELECT period, pdstart,pdend FROM pd " +
                " WHERE Convert(Datetime,Convert(nvarchar(8),pdstart,112))<= '" + Convert.ToDateTime(trandate).ToString("yyyyMMdd") + "' " +
                " AND Convert(Datetime,Convert(nvarchar(8),pdend,112)) >= '" + Convert.ToDateTime(trandate).ToString("yyyyMMdd") + "'";

            dbaccess.ReadSQL("getPd", strGetPd);

            int period = 0;

            if (dbaccess.DataSet.Tables["getPd"].Rows.Count > 0)
            {
                period = Convert.ToInt32(dbaccess.DataSet.Tables["getPd"].Rows[0]["period"]);
            }

            return period;

            //else
            //{
            //    MessageBox.Show("Please check with your administrator for the settings of Period!");
            //}
        }

        #endregion

        #region get from SYSID
        public static int getID(DBAccess dbaccess, string tableName)
        {
            string lastID = "SELECT lastID FROM SysID where TableName = '" + tableName + "'";
            int maxID = 0;
            dbaccess.ReadSQL("sysID", lastID);
            DataTable sysID = dbaccess.DataSet.Tables["sysID"];
            if (sysID.Rows.Count != 0)
                maxID = Convert.ToInt32(sysID.Rows[0][0].ToString());
            return maxID;
        }
        #endregion

        #region update sys ID
        public static void updateID(DBAccess dbaccess, string tableName, int lastID)
        {
            string updateID = "UPDATE sysID set LastID =" + lastID + " where TableName = '" + tableName + "' ";
            dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateID);
        }
        #endregion

		#region added by Vincent

		#region Clear

		public static void Clear(DataTable dt)
		{
			for (int i = dt.Rows.Count - 1; i >= 0; i--)
			{
				if (dt.Rows[i].RowState != DataRowState.Deleted)
				{
					dt.Rows[i].Delete();
				}
			}
		}

		#endregion

		#region is Frontend?
		public static bool isFrontEnd()
		{
			return ("HQ" != (ConfigurationManager.AppSettings.Get("POSID").ToString().Trim()));
		}

		#endregion

		public static bool isFrontEndVoucher(DBAccess dbaccess, string headerName)
		{
			DataRow header = dbaccess.DataSet.Tables[headerName].Rows[0];

			// Refnum does not contain -HQ- and contains -, then it is frontend.

			if (!header["refnum"].ToString().Trim().Contains("-HQ-") && header["refnum"].ToString().Trim().Contains("-"))
			{
				return true;
			}
			else
				return false;

		}

		public static void disableButtons(String formname)
		{
			Button btn_Document_Reopen = (Button)BizXmlReader.CurrentInstance.GetControl(formname, Common.DEFAULT_DOCUMENT_BTNREOPEN);
			btn_Document_Reopen.Visible = false;

			Button btn_Document_Void = (Button)BizXmlReader.CurrentInstance.GetControl(formname, Common.DEFAULT_DOCUMENT_BTNVOID);
			btn_Document_Void.Visible = false;
		}

		public static Boolean isHQ(string refnum, DBAccess e)
		{
			String strSQL = "select refnum,type from sitm where refnum = '" + refnum + "' and [type] in (Select refnum from sitetype where sitetype='HQ')";
			e.ReadSQL("HQ", strSQL);

			if (e.DataSet.Tables["HQ"].Rows.Count > 0)
				return true;
			else
				return false;
		}


		public static String getSysDefaultTrandate(String refnum, DBAccess dbaccess)
		{
			if (!isHQ(refnum, dbaccess))
			{
				Hashtable htSelColl2 = new Hashtable();
				String command2 = "select trandate from sysdefault";
				htSelColl2.Add("sysdefault", command2);

				dbaccess.ReadSQL(htSelColl2);

				if (dbaccess.DataSet.Tables["sysdefault"].Rows.Count > 0)
					return GetStandardDateString((DateTime)dbaccess.DataSet.Tables["sysdefault"].Rows[0]["trandate"]);
				else
					return null;
			}
			else
				return null;

        }

        #region GetStandardDateString
        //added by Yushu
        public static string GetStandardDateString(DateTime dateTime)
		{
			return dateTime.Year.ToString() + "-" + dateTime.Month.ToString("00") + "-" + dateTime.Day.ToString("00");
        }
        #endregion

        # region ClearDuplicatedRows
        //Added By Yushu - For HTMGH
        public static void ClearDuplicatedRows(DataTable dt, string key)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                i = Math.Min(dt.Rows.Count - 1, i);

                if (dt.Rows[i].RowState != DataRowState.Deleted)
                {
                    string filter = key + "='" + dt.Rows[i][key].ToString() + "'";
                    int count = (int)dt.Compute("COUNT(" + key + ")", filter);

                    for (int j = 1; j < count; j++)
                    {
                        dt.Select(filter)[0].Delete();
                    }
                }
            }
        }
        #endregion ClearDuplicatedRows

        #region Cent
        //Added By Yushu - For HTMGH Module
        public static decimal Cent(decimal dblNumber)
        {
            decimal dbsFinal = dblNumber * 1000;
            int dbsRem = (int)SqlInt32.Mod((int)dbsFinal, 10);
            decimal dblInt = dbsFinal - (decimal)dbsRem;

            if (dbsRem >= 5)
            {
                dbsFinal = (dblInt + 10) / 1000;
            }

            else
            {
                dbsFinal = dblInt / 1000;
            }

            return decimal.Parse(dbsFinal.ToString("#0.00"));
        }

        #endregion

        # region ClearInvalidRowsEMPNUM
        //added function for htmg - joel - 08.12.2006
        public static void ClearInvalidRowsEMPNUM(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i].RowState != DataRowState.Deleted)
                {
                    if (IsEmpty(dt.Rows[i]["empnum"]))
                    {
                        dt.Rows[i].Delete();
                    }
                }
            }
        }
        #endregion ClearInvalidRowsEMPNUM

        #region IsEmpty
        //Added By Yushu
        public static bool IsEmpty(object o)
        {
            return Convert.ToString((object)(o.ToString().Trim())) == "";
        }

        #endregion

        #region convertInttoStrMth

        public static string convertInttoStrMth(int intmth)
        {
            switch (intmth)
            {
                case 1:
                    return "JAN";
                case 2:
                    return "FEB";
                case 3:
                    return "MAR";
                case 4:
                    return "APR";
                case 5:
                    return "MAY";
                case 6:
                    return "JUN";
                case 7:
                    return "JUL";
                case 8:
                    return "AUG";
                case 9:
                    return "SEP";
                case 10:
                    return "OCT";
                case 11:
                    return "NOV";
                case 12:
                    return "DEC";
                default:
                    return "";
            }
        }

        public static int GetWENo(DateTime startdate, DateTime enddate)
        {
            int noofdays = ((TimeSpan)(enddate - startdate)).Days + 1;
            int dayofwk = 0;
            int totwedaysno = 0;

            for (int i = 0; i < noofdays; i++)
            {
                dayofwk = (int)(((DateTime)startdate).AddDays(i)).DayOfWeek;

                if (dayofwk == 0 || dayofwk == 6)
                    totwedaysno = totwedaysno + 1;
            }

            return totwedaysno;
        }

        public static int GetPBHNo(DBAccess dbAccess, DateTime startdate, DateTime enddate)
        {
            string command = "select count(*) as pbhcount from pbhm1 where pbhdate >= '" + Tools.GetSafeDateString((DateTime)startdate)
                + "' and pbhdate <= '" + Tools.GetSafeDateString((DateTime)enddate) + "' and wkend = 0";

            Hashtable selectsCollection = new Hashtable();
            selectsCollection.Add("tmp1pbhm1", command);
            dbAccess.ReadSQL(selectsCollection);

            if (dbAccess.DataSet.Tables["tmp1pbhm1"].Rows.Count != 0)
                return (int)dbAccess.DataSet.Tables["tmp1pbhm1"].Rows[0]["pbhcount"];

            else
                return 0;
        }

        public static int GetOFFDaysNo(DBAccess dbAccess, DateTime startdate, DateTime enddate, string empnum)
        {
            string command = "select count(*) as offdayscount from htmg3 where empnum = '" + empnum + "' and brchlvtype = '"
                + "OFF" + "' and trandate >= '" + Tools.GetSafeDateString(startdate) + "' and trandate <= '"
                + Tools.GetSafeDateString(enddate) + "'";

            Hashtable selectsCollection = new Hashtable();
            selectsCollection.Add("tmp1htmg3", command);
            dbAccess.ReadSQL(selectsCollection);

            if (dbAccess.DataSet.Tables["tmp1htmg3"].Rows.Count != 0)
                return (int)dbAccess.DataSet.Tables["tmp1htmg3"].Rows[0]["offdayscount"];

            else
                return 0;
        }

        public static void AppendErrMsg(DataSet dataSet, string refnum, string errorCode, string errorMessage)
        {
            if (dataSet.Tables.Contains("err"))
            {
                DataRow err_dr = dataSet.Tables["err"].NewRow();
                err_dr["refnum"] = refnum;
                err_dr["errcode"] = errorCode;
                err_dr["errmsg"] = errorMessage;
                err_dr["created"] = System.DateTime.Now;
                err_dr["modified"] = System.DateTime.Now;
                err_dr["year"] = System.DateTime.Now.Year;
                err_dr["user"] = Common.DEFAULT_SYSTEM_USERNAME;
                dataSet.Tables["err"].Rows.Add(err_dr);
            }

            else
            {
                throw new Exception("Table 'ERR' is not in dataset");
            }
        }


        public static string GetSitenum()
        {
            Hashtable selectedCollection = new Hashtable();
            DBAccess dbAccess = new DBAccess();
            string refnum = "";

            selectedCollection.Add("posm", "select refnum from posm where posnum = '" + ConfigurationManager.AppSettings.Get("POSID") + "'");
            dbAccess.ReadSQL(selectedCollection);

            if (dbAccess.DataSet.Tables["posm"].Rows.Count != 0)
            {
                DataRow posm = dbAccess.DataSet.Tables["posm"].Rows[0];
                refnum = posm["refnum"].ToString();
            }

            else
            {
                MessageBox.Show("Invalid POS ID.");
            }

            return refnum;
        }
        #endregion

        #region F2Condition
        /// <summary>
        /// Used for 'wildcard' searching. Returns a string to be used for the e.Condition 
        /// (i.e. where clause used in the select statement)
        /// </summary>
        /// <param name="fieldsToSearch">fields in the F2 query to search</param>
        /// <param name="textboxValue">the textfield value to search</param>
        public static string F2Condition(string fieldsToSearch, string textboxValue)
        {
            string delimStr = " ,/*";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = fieldsToSearch.Split(delimiter);

            string spChar = "|";
            char[] specialChar = spChar.ToCharArray();

            string result = "";
            int i = 0;

            textboxValue = textboxValue.Replace("'", "''");
            foreach (string s in split)
            {
                if (i == 0)
                {
                    if (textboxValue.EndsWith("|"))
                    {
                        result = s + " LIKE '" + textboxValue.TrimEnd(specialChar) + "%'";
                    }
                    else
                    {
                        result = s + " LIKE '%" + textboxValue + "%'";
                    }
                    i++;
                }
                else
                {
                    if (textboxValue.EndsWith("|"))
                    {
                        result = result + " OR " + s + " LIKE '" + textboxValue.TrimEnd(specialChar) + "%'";
                    }
                    else
                    {
                        result = result + " OR " + s + " LIKE '%" + textboxValue + "%'";
                    }
                }
            }

            return "(" + result + ")";
        }

        #endregion

        #region GetStandardDateString / GetSafeDateString / GetSafeTimeString

        //public static string GetStandardDateString(DateTime dateTime)
        //{
        //    return dateTime.Year.ToString() + "-" + dateTime.Month.ToString("00") + "-" + dateTime.Day.ToString("00");
        //}

        public static string GetSafeDateString(DateTime dateTime)
        {
            return dateTime.Year.ToString() + dateTime.Month.ToString("00") + dateTime.Day.ToString("00");
        }

        public static string GetSafeTimeString(DateTime dateTime)
        {
            return GetStandardDateString(dateTime) + "T" + dateTime.Hour.ToString("00") + ":" + dateTime.Minute.ToString("00") + ":" + dateTime.Second.ToString("00");
        }

        #endregion

        #region Round
        public static decimal Round(decimal numberToRound)
        {
            int decimalPlaces = 2;

            if (Convert.IsDBNull(numberToRound) || numberToRound == 0)
                return 0;
            else
                return Decimal.Round((decimal)System.Data.SqlTypes.SqlDecimal.Round((System.Data.SqlTypes.SqlDecimal)numberToRound, decimalPlaces), decimalPlaces);
        }
        #endregion    

	    #endregion End Vincent

        #region Get Datagrid TextBox (Pass Column Index No)

        public static TextBoxGrid DGTextBox(DataGrid dg, int CIndex)
        {
            DataGridTableStyle dgs = dg.TableStyles[0];
            DataGridColumnStyle dgc = dgs.GridColumnStyles[CIndex];
            BizDataGridTextBoxColumn dg_tb = dgc as BizDataGridTextBoxColumn;
            TextBoxGrid tbg = dg_tb.TextBoxGrid;
            return tbg;
        }

        public static TextBoxGrid DGTextBox(DataGrid dg, string Column)
        {
            DataGridTableStyle dgs = dg.TableStyles[0];
            DataGridColumnStyle dgc = dgs.GridColumnStyles[Column];
            BizDataGridTextBoxColumn dg_tb = dgc as BizDataGridTextBoxColumn;
            TextBoxGrid tbg = dg_tb.TextBoxGrid;
            return tbg;
        }

        #endregion

        #region Send Email

        public static void SendEmail(string Sender, string Receiver, string Subject, string BodyMessage)
        {

            try
            {
                MailMessage mail = new MailMessage();

                //set the addresses
                mail.From = new MailAddress(Sender);

                string[] mailto;
                char[] splitter = { ';' };
                mailto = Receiver.Split(splitter);

                if (mailto.Length > 1)
                {
                    for (int a = 0; a < mailto.Length; a++)
                    {
                        mail.To.Add(mailto[a]);
                    }
                }
                else
                {
                    mail.To.Add(Receiver);
                }

                //set the content
                mail.Subject = Subject;
                mail.Body = BodyMessage;

                //send the message
                SmtpClient smtp = new SmtpClient("mail.bizpoint.com.my");
                //to authenticate we set the username and password properites on the SmtpClient
                //smtp.Credentials = new NetworkCredential("username", "secret");
                smtp.Send(mail);
                //MessageBox.Show("Email Successfully Sent", "Successful Email", MessageBoxButtons.OK);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public static void SendEmail2(string Sender, string Receiver, string Subject, string BodyMessage, string username, string password)
        {

            try
            {
                MailMessage mail = new MailMessage();

                //set the addresses
                mail.From = new MailAddress(Sender);

                string[] mailto;
                char[] splitter = { ';' };
                mailto = Receiver.Split(splitter);

                if (mailto.Length > 1)
                {
                    for (int a = 0; a < mailto.Length; a++)
                    {
                        mail.To.Add(mailto[a]);
                    }
                }
                else
                {
                    mail.To.Add(Receiver);
                }

                //set the content
                mail.Subject = Subject;
                mail.Body = BodyMessage;

                //send the message
                SmtpClient smtp = new SmtpClient("pop.gmail.com");
                //to authenticate we set the username and password properites on the SmtpClient
                smtp.Credentials = new NetworkCredential(username, password);
                smtp.Send(mail);
                MessageBox.Show("Email Successfully Sent", "Successful Email", MessageBoxButtons.OK);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }


        public static Boolean SendEmailWithStatus(string Sender, string Receiver, string Subject, string BodyMessage)
        {

            try
            {

                BizRAD.DB.Client.DBAccess dbAccess = new DBAccess();
                dbAccess.ReadSQL("tmpDefault", "select * from emailm where isdefault = 1 and status <> 'V'");
                DataTable defaulttemp = dbAccess.DataSet.Tables["tmpDefault"];

                if (defaulttemp.Rows.Count != 1)
                {
                    MessageBox.Show("Error Detected In Email Setting", "Email Failed", MessageBoxButtons.OK);

                    return false;
                }
                else
                {
                    if (BizFunctions.IsEmpty(Sender))
                        Sender = defaulttemp.Rows[0]["sender"].ToString();

                    string SmtpServer = defaulttemp.Rows[0]["smtp"].ToString();

                    if (Convert.IsDBNull(defaulttemp.Rows[0]["port"]))
                    {
                        defaulttemp.Rows[0]["port"] = 0;
                    }


                    int Port = Convert.ToInt32(defaulttemp.Rows[0]["port"]);


                    MailMessage mail = new MailMessage();

                    //set the addresses
                    mail.From = new MailAddress(Sender);

                    string[] mailto;
                    char[] splitter = { ';' };
                    mailto = Receiver.Split(splitter);

                    if (mailto.Length > 1)
                    {
                        for (int a = 0; a < mailto.Length; a++)
                        {
                            mail.To.Add(mailto[a]);
                        }
                    }
                    else
                    {
                        mail.To.Add(Receiver);
                    }

                    //set the content
                    mail.Subject = Subject;
                    mail.Body = BodyMessage;

                    //send the message
                    SmtpClient smtp = new SmtpClient(SmtpServer, Port);

                    //if users tick on the credential checkbox (this box is required to tick if the users use port other than 25)
                    if (defaulttemp.Rows[0]["credential"].ToString() == "True")
                    {
                        //to authenticate we set the username and password properites on the SmtpClient
                        smtp.Credentials = new System.Net.NetworkCredential(defaulttemp.Rows[0]["username"].ToString(), defaulttemp.Rows[0]["password"].ToString());
                    }
                    smtp.Send(mail);
                    //MessageBox.Show("Email Successfully Sent", "Successful Email", MessageBoxButtons.OK);
                    return true;
                }
            }
            catch (SmtpException smex)
            {
                MessageBox.Show(smex.StackTrace);
                MessageBox.Show(smex.InnerException.ToString());
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        #endregion

        #region get the accounting period base on the trandate passed in
        // return -1 if there is no period set for the associate trandate
        public static int GetPeriod(DBAccess dbAccess, DateTime trandate)
        {
            int period = 0;

            DataSet ds = new DataSet();
            string tdate = trandate.ToString("yyyyMMdd");
            string displayDate = trandate.ToString("dd/MM/yyyy");
            string sql = "select * from pd" + BizRAD.BizCommon.Common.DEFAULT_SYSTEM_YEAR + " as pd where convert(varchar(8), pdstart, 112) <= '" + tdate + "' " +
                        " and convert(varchar(8), pdend, 112) >=  '" + tdate + "'";
            dbAccess.ReadSQL("pd", sql);

            if (dbAccess.DataSet.Tables["pd"].Rows.Count == 1)
            {
                period = (int)dbAccess.DataSet.Tables["pd"].Rows[0]["period"];
            }
            else
            {

                period = -1;
                //MessageBox.Show("Period is not set for " + displayDate + "!", "Invalid Period Setting", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }

            return period;
        }
        #endregion get the accounting period base on the trandate passed in

        #region Check Numeric

        public static Boolean isNumeric(string val)
        {
            try
            {
                Double result;
                return Double.TryParse(val, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out result);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }



        }
        #endregion

        #region Created by Jason

        public static string GetF2KeyDown(object s, System.Windows.Forms.KeyEventArgs a,string Filter,string Condition, string tablename, string rowname)
        {
            string detail = "";
            if (a.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_" + tablename + ".xml", s, "", Filter, Condition, F2Type.Sort);
                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    detail = f2BaseHelper.F2Base.CurrentRow[rowname].ToString();

                }


            }
            return detail;
        }

        public static string GetF2Clicked(object sender, System.EventArgs a, string tablename, string rowname)
        {
            string detail = "";
            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_" + tablename + ".xml", sender, "", null, null, F2Type.Sort);
            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                detail = f2BaseHelper.F2Base.CurrentRow[rowname].ToString();
            }
            return detail;
        }

        public static DataTable GetShiftSummary(string TableName, DataSet ds)
        {
            DataTable dt = new DataTable();
            string sql1 = "Select R1.matnum, " +
                         "R1.shiftcode, " +
                         "sum(R1.monday) as monday, " +
                         "sum(R1.tuesday) as tuesday, " +
                         "sum(R1.wednesday) as wednesday, " +
                         "sum(R1.thursday) as thursday, " +
                         "sum(R1.friday) as friday, " +
                         "sum(R1.saturday) as saturday, " +
                         "sum(R1.sunday) as sunday " +
                         "from " +
                         "( " +
                         "select " +
                         "[ID], " +
                         "matnum, " +
                         "shiftcode, " +
                         "case when ISNULL(monday,'') = 'X' then 1 else  0 end as monday, " +
                         "case when ISNULL(tuesday,'') = 'X' then 1 else  0 end as tuesday, " +
                         "case when ISNULL(wednesday,'') = 'X' then 1 else  0 end as wednesday, " +
                         "case when ISNULL(thursday,'') = 'X' then 1 else  0 end as thursday, " +
                         "case when ISNULL(friday,'') = 'X' then 1 else  0 end as friday, " +
                         "case when ISNULL(saturday,'') = 'X' then 1 else  0 end as saturday, " +
                         "case when ISNULL(sunday,'') = 'X' then 1 else  0 end as sunday " +
                         "from " + TableName + " " +
                         ")R1 " +
                         "Group by R1.matnum,R1.shiftcode";
            dt = BizFunctions.ExecuteQuery(ds, sql1);

            return dt;
        }

        public static DataTable GetShiftSummary2(string TableName, string refnum, DataSet ds, DBAccess dbAcc)
        {
            DataTable dt = new DataTable();


            string sql1 = "Select " +
                       "matnum, " +
                       "shiftcode, " +
                       "ISNULL(sum(monday),0) as monday, " +
                       "ISNULL(sum(tuesday),0) as tuesday, " +
                       "ISNULL(sum(wednesday),0) as wednesday, " +
                       "ISNULL(sum(thursday),0) as thursday, " +
                       "ISNULL(sum(friday),0) as friday, " +
                       "ISNULL(sum(saturday),0) as saturday, " +
                       "ISNULL(SUM(sunday),0) as sunday " +
                   "FROM " +
                   "( " +
                       "select R1.matnum, R1.[day], R1.shiftcode, case when sum(R1.isworkshift) = 0 then 1 else sum(R1.isworkshift) end as total  from " +
                       "( " +
                           "Select u.matnum,[day],u.shift AS ShiftCode,v.isWorkShift from " +
                           "(select empnum, matnum,  shiftcode, " +
                                	"CASE WHEN CHARINDEX('/',monday) > 0 then LEFT(ISNULL(monday,''),(CHARINDEX('/',ISNULL(monday,'')))-1) else monday end as monday, " +
	                                "CASE WHEN CHARINDEX('/',tuesday) > 0 then LEFT(ISNULL(tuesday,''),(CHARINDEX('/',ISNULL(tuesday,'')))-1) else tuesday end as tuesday," +
	                                "CASE WHEN CHARINDEX('/',wednesday) > 0 then LEFT(ISNULL(wednesday,''),(CHARINDEX('/',ISNULL(wednesday,'')))-1) else wednesday end as wednesday, " +
	                                "CASE WHEN CHARINDEX('/',thursday) > 0 then LEFT(ISNULL(thursday,''),(CHARINDEX('/',ISNULL(thursday,'')))-1) else thursday end as thursday, " +
	                                "CASE WHEN CHARINDEX('/',friday) > 0 then LEFT(ISNULL(friday,''),(CHARINDEX('/',ISNULL(friday,'')))-1) else friday end as friday, " +
	                                "CASE WHEN CHARINDEX('/',saturday) > 0 then LEFT(ISNULL(saturday,''),(CHARINDEX('/',ISNULL(saturday,'')))-1) else saturday end as saturday, " +
	                                "CASE WHEN CHARINDEX('/',sunday) > 0 then LEFT(ISNULL(sunday,''),(CHARINDEX('/',ISNULL(sunday,'')))-1) else sunday end as sunday  " +
                               "from " + TableName + " where refnum='" + refnum + "' " +
                               "AND MATNUM<>'' " +
                            ") AS p " +
                           "UNPIVOT " +
                               "(shift for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY))u " +
                           "left join vSHLV v on u.[shift] = v.SHIFTCODE " +
                       ") R1 where R1.ShiftCode<>'' " +
                       "GROUP BY R1.matnum, R1.[day], R1.shiftcode " +
                   ") as giv " +
                   "PIVOT " +
                   "( " +
                       "SUM(Total) for [day] IN ([MONDAY],[TUESDAY],[WEDNESDAY],[THURSDAY],[FRIDAY],[SATURDAY],[SUNDAY]) " +
                   ")AS child " +
                   "GROUP BY matnum,shiftcode " +
                   "ORDER BY matnum ";


            dbAcc.ReadSQL("tmpDT", sql1);

            if (dt.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(dt);
            }


            return dt = dbAcc.DataSet.Tables["tmpDT"];
        }

        #region Get Max ID of a Table
        public static int getMaxID(string tablename, DBAccess dbAcc)
        {
            int maxcount;
            dbAcc.ReadSQL("maxCountTmp", "Select ISNULL(max(id),0) as mid from " + tablename + "");
            object o = dbAcc.DataSet.Tables["maxCountTmp"].Rows[0]["mid"];
            maxcount = Convert.ToInt32(o);

            return maxcount;
        }
        #endregion

        public static string GetNRIC(string empnum)
        {
            string nric = "";
            string strGetNRIC = "";
            DBAccess dbAccess = new DBAccess();
            strGetNRIC = "Select nric from hemph where empnum='" + empnum + "' ";

            dbAccess.ReadSQL("dtNRIC", strGetNRIC);

            if (dbAccess.DataSet.Tables["dtNRIC"].Rows.Count > 0)
            {
                DataRow drNRIC = dbAccess.DataSet.Tables["dtNRIC"].Rows[0];
                nric = drNRIC["nric"].ToString();
            }

            dbAccess = null;
            return nric;
        }

        public static string GetNRIC2(string empnum, DBAccess dbAccess)
        {
            string nric = "";
            string strGetNRIC = "";
  
            strGetNRIC = "Select nric from hemph where empnum='" + empnum + "' ";

            dbAccess.ReadSQL("dtNRIC", strGetNRIC);

            if (dbAccess.DataSet.Tables["dtNRIC"].Rows.Count > 0)
            {
                DataRow drNRIC = dbAccess.DataSet.Tables["dtNRIC"].Rows[0];
                nric = drNRIC["nric"].ToString();
            }

            dbAccess = null;
            return nric;
        }


        #region Compare two DataTables and return a DataTable with DifferentRecords
        /// <summary>  
        /// Compare two DataTables and return a DataTable with DifferentRecords  
        /// </summary>  
        /// <param name="FirstDataTable">FirstDataTable</param>  
        /// <param name="SecondDataTable">SecondDataTable</param>  
        /// <returns>DifferentRecords</returns>  
        public static DataTable getDifferentRecords(DataTable FirstDataTable, DataTable SecondDataTable)
        {
            //Create Empty Table  
            DataTable ResultDataTable = new DataTable("ResultDataTable");

            //use a Dataset to make use of a DataRelation object  
            using (DataSet ds = new DataSet())
            {
                //Add tables  
                ds.Tables.AddRange(new DataTable[] { FirstDataTable.Copy(), SecondDataTable.Copy() });

                //Get Columns for DataRelation  
                DataColumn[] firstColumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstColumns.Length; i++)
                {
                    firstColumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondColumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondColumns.Length; i++)
                {
                    secondColumns[i] = ds.Tables[1].Columns[i];
                }

                //Create DataRelation  
                DataRelation r1 = new DataRelation(string.Empty, firstColumns, secondColumns, false);
                ds.Relations.Add(r1);

                DataRelation r2 = new DataRelation(string.Empty, secondColumns, firstColumns, false);
                ds.Relations.Add(r2);

                //Create columns for return table  
                for (int i = 0; i < FirstDataTable.Columns.Count; i++)
                {
                    ResultDataTable.Columns.Add(FirstDataTable.Columns[i].ColumnName, FirstDataTable.Columns[i].DataType);
                }

                //If FirstDataTable Row not in SecondDataTable, Add to ResultDataTable.  
                ResultDataTable.BeginLoadData();
                foreach (DataRow parentrow in ds.Tables[0].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r1);
                    if (childrows == null || childrows.Length == 0)
                        ResultDataTable.LoadDataRow(parentrow.ItemArray, true);
                }

                //If SecondDataTable Row not in FirstDataTable, Add to ResultDataTable.  
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r2);
                    if (childrows == null || childrows.Length == 0)
                        ResultDataTable.LoadDataRow(parentrow.ItemArray, true);
                }
                ResultDataTable.EndLoadData();
            }

            return ResultDataTable;
        }
        #endregion

        public static DataTable GetCommonEmpData(string empnum)
        {
            DBAccess dbAccess = new DBAccess();
            DataTable ReturnTable = null;

            string sql1 = "select h.nric, h.empnum, h.empname,h.gender,h.matnum,h.sitenum,h.statuscode,h.sectorcode,h.datejoined,a.rateamt as CurrentSalary, h.statuscode,h.daysperweek,h.paytypecode " +
                            "from hemph h " +
                            "left join " +
                            "(Select empnum,SUM(rateamt) as rateamt from PFMSR where empnum='" + empnum + "' group by empnum )a " +
                            "on h.empnum=a.empnum " +
                            "where H.empnum='" + empnum + "' and h.[status]<>'V'";
            dbAccess.ReadSQL("dtCommonEmpData", sql1);
            return ReturnTable = dbAccess.DataSet.Tables["dtCommonEmpData"];
        }

        public static DataRow GetCommonEmpDataRow(string empnum)
        {
            DBAccess dbAccess = new DBAccess();
            DataRow ReturnRow = null;

            string sql1 = "select h.empnum, h.empname,h.gender,h.matnum,h.sitenum,h.statuscode,h.sectorcode,h.datejoined,a.rateamt as CurrentSalary, h.statuscode,h.daysperweek " +
                            "from hemph h " +
                            "left join " +
                            "(Select empnum,SUM(rateamt) as rateamt from PFMSR where empnum='" + empnum + "' group by empnum )a " +
                            "on h.empnum=a.empnum " +
                            "where H.empnum='" + empnum + "' and h.[status]<>'V'";
            dbAccess.ReadSQL("dtCommonEmpData", sql1);

            if (dbAccess.DataSet.Tables["dtCommonEmpData"].Rows.Count > 0)
            {
                ReturnRow = dbAccess.DataSet.Tables["dtCommonEmpData"].Rows[0];
            }

            return ReturnRow;
        }


        public static DataRow GetCommonEmpDataRow2(string nric)
        {
            DBAccess dbAccess = new DBAccess();
            DataRow ReturnRow = null;

            string sql1 = "select top 1 h.nric, h.empnum, h.empname,h.gender,h.matnum,h.sitenum,h.statuscode,h.sectorcode,h.datejoined, h.statuscode " +
                            "FROM HEMPH H where H.nric='" + nric + "' and h.[status]<>'V'";

            dbAccess.ReadSQL("dtCommonEmpData", sql1);

            if (dbAccess.DataSet.Tables["dtCommonEmpData"].Rows.Count > 0)
            {
                ReturnRow = dbAccess.DataSet.Tables["dtCommonEmpData"].Rows[0];
            }

            return ReturnRow;
        }

        public static DataRow GetCommonEmpDataRowByDoc(string docunum)
        {
            DBAccess dbAccess = new DBAccess();
            DataRow ReturnRow = null;

            string sql1 = "Select empnum, empname from CTRH where refnum='"+docunum+"'";
            dbAccess.ReadSQL("dtCommonEmpData", sql1);

            return ReturnRow = dbAccess.DataSet.Tables["dtCommonEmpData"].Rows[0];
        }

        public static string GetEmpname(string empnum, DBAccess dbAccess)
        {
            string empname = "";

            string str1 = "Select empname from hemph where empnum='"+empnum+"' ";

            dbAccess.ReadSQL("TempEmpName1", str1);

            DataTable dt1 = dbAccess.DataSet.Tables["TempEmpName1"];

            if (dt1.Rows.Count > 0)
            {
                empname = dbAccess.DataSet.Tables["TempEmpName1"].Rows[0]["empname"].ToString();
            }

            return empname;
        }


        #endregion

        public static DataRow GetCommonDataRow(string tablename, string columname, string key)
        {
            DBAccess dbAccess = new DBAccess();
            DataRow ReturnRow = null;

            string sql1 = "Select top 1 * from " + tablename + " where " + columname + "='" + key + "'  ";
            dbAccess.ReadSQL("GetCommonDataRow", sql1);

            if (dbAccess.DataSet.Tables["GetCommonDataRow"].Rows.Count > 0)
            {
                ReturnRow = dbAccess.DataSet.Tables["GetCommonDataRow"].Rows[0];
            }

            return ReturnRow;
        }

        public static string ToTitleCase(string inputString)
        {
            CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(inputString.ToLower());  //inputString.Tolower() sometimes you need to use inputString directly here, it depends on yourself. 
        } 

        public static string ToUpperString(string source)
        {
            string letters = source;
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            //char[] letters = source.ToCharArray();
            //// upper case the first char
            //letters[0] = char.ToUpper(letters[0]);
            //// return the array made of the new char array
            return letters.ToUpper();
        }

        public static string GetSectorCode(string Site, DBAccess dbAccess)
        {
            string GetSector = "Select sectorcode from sitm where sitenum='"+ Site +"'";

            string SectorCode = "";

            dbAccess.ReadSQL("SectorTB", GetSector);

            DataTable SectorTB = dbAccess.DataSet.Tables["SectorTB"];

            if (SectorTB.Rows.Count > 0)
            {
                DataRow drSector = dbAccess.DataSet.Tables["SectorTB"].Rows[0];

                SectorCode = drSector["sectorcode"].ToString();

            }
            SectorTB.Dispose(); ;
            return SectorCode;
        }

        #region Jason: Display Crystal Report using (Dataset)
        public static void DispalyCR1(string rptfilelocation, DataSet source)
        {
            //BizERP.BizModules.Tools.CRForm crpt1 = new BizERP.BizModules.Tools.CRForm(source, rptfilelocation);
            //crpt1.Show();
            //crpt1.Focus();
        }
        #endregion

        public static string DefaultVarbinary()
        {
            string varBinary = "0x504B0304140002000800976C904258470FF02F020000B20400000B000000636F6E74656E742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB767B5A2D5BFA99D2CBCBE651D35E9739B5AE978FAAAC299A47CB6C91378FDAE9A36A952FCD4B8FBCC68FB823F9A0CDDFB5B77D196DFD77CFABDBBEF9AE29B7CF2B427CB1CADA6212A2D05C5EDC160E351D0032ABB3ABDB42415BA2B0FFFABBB258BEFDECA379DBAE1EDDBD7B757535BEBA37AEEA8BBBBB0F1F3EBCCBDF9AA65575651B02B8CC0E37DEDBD9D9BF7B55176D5E5BFA6693DBCF0E37F6D112D8B77D5DF9C4A7CAD4A2BA5AD72523399BDECDCB1CAF357777C7BB773FB20C764E7CB57D9E4DF3ED593E2D9BF4AEFD265BB7D582883EDD66066A8E1E0B23F1BFA9FC0ECC3EFBE835F52D7F9F678BA2BCFEECA3269FB6C4DE1FD977E4EFED554D43A8DB22E78EEE7A0087A1BFDCED425F6575765167ABB9F9823E808CF01FDBF2D68BAA5E64A569B0C81A9A9FED557661BE7FDD66CB5956CF3E62440687AC5F4CAAD9B5FD032271F4980543C795CA1F5EF720097FA87F4ABB9D8F8E1EF3A7ABFE1B344C03749545203ECDCFB375D9A62FCDE0D36734751F1D9D2E56EDF5E3BBF6CD23FD7D657E511CDD28057FF3970CCDFCD5D53747FF0F504B0304140002000800976C9042D5F05C2BBA020000F80500000A0000007374796C65732E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB76BB69AFCBBC49E9DD65F388FFA0C6F5F251953545F368992DF2E6513B7D54ADF2A579E791D7F811F7231F9C57B77DF55D536E9F57DBD36AB1CADA621282795716CBB79F7D346FDBD5A3BB77AFAEAEC657F7C6557D7177F7E1C38777F95BD3B4B9BCB86D97D474A03FA1C86DE128FDF0BE25E67945843CCFA6F9F62C9F964D7AD77E23D43D7A2CB4E27F53F91D7D7CF6D1D3FC3C5B976DFA32ABB38B3A5BCDD36704EB236D739E2D8AF2FAB38FDAFC1D7D767718CCF36299A72FD68B495EC7DF95CF56596DA77C7B33021B3AFBF6F52AAF6516225D99F7F0C7F6AA260AD66D410C765E3D9A5665557FF6D193729D9B57B9D57A396380B960F6D9474D5516B3812657C5AC9D7FF651B66E2BC6F2AE87E630CE2FAA7A91955D845766D00C091D3D421F4B2624890D31CCF2BCB858D7C434D532E506F2E536DA359F7D749E950D0D86BFA1296FF2F6B38F76C6F7A70BFD8C5A13A353DFF4F9AEFB0C10565553B42CAA657EDEEA77C5725AE7E0360263F8135FDC823B6D5BE16D90A6C384FA274847081553957D43B55576916F97D975B56E03DA7DB15AECD869F51A756697BFD1D93918DF2F961FD94FE7797131070576F1A9022202B7DB555DD06033A1C3AAAADB3A2B5A7E7191D517C572BBAD56A0E883834F0F14A27E31A95A1A46FC3B10141A70F760E7D3F09B5A10A1297AB8730F5F390EF246E648D7A7957EB1C89A962631A4A07E084801055FB7D97296D596A57D227A34F6E7AC03DF7CDC51DB47FF0F504B0304140002000800976C90425CC0E1810501000000010000150000004D4554412D494E462F6D616E69666573742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E8F1225B16E779D33E32BFA4F4DAB2B17F52D37AF9A8CA9AA279B4CC1679F3A89D3EAA56F972564DD78B7CD93E0ADB3F42471ED8F3A2CCB7A9597D9DBAAEF259916DB7D7ABFCB38FB2D5AA2CA6594B48DEBD5CCEC6DCD3D8EF60DCE6EFDA8FDCDBE7EBB2DC5E65EDFCB38FEE7E94DE3D7A7CB73786A3FF07504B01021400140002000800976C904258470FF02F020000B20400000B0000000000000000000000000000000000636F6E74656E742E786D6C504B01021400140002000800976C9042D5F05C2BBA020000F80500000A00000000000000000000000000580200007374796C65732E786D6C504B01021400140002000800976C90425CC0E181050100000001000015000000000000000000000000003A0500004D4554412D494E462F6D616E69666573742E786D6C504B05060000000003000300B4000000720600000000";
            return  varBinary;
        }


        public static bool ChangeDocStatus(string docstat)
        {
            bool handle = false;
            if (docstat == (string)Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (MessageBox.Show("Confirm this Document? \nYes or No?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    handle = false;
                }
                else
                {
                    handle = true;
                }
            }
            else if (docstat == (string)Common.DEFAULT_DOCUMENT_STATUSV)
            {
                if (MessageBox.Show("Void this Document? \nYes or No?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    handle = false;
                }
                else
                {
                    handle = true;
                }
            }

            return handle;
        }

        public static string GetSitenumFromTrq(string Refnum, DBAccess dbAccess)
        {
            string GetSitenum = "Select sitenum from trqh where refnum='" + Refnum + "'";

            string Sitenum = "";

            dbAccess.ReadSQL("SitenumTB", GetSitenum);

            DataTable SitenumTB = dbAccess.DataSet.Tables["SitenumTB"];

            if (SitenumTB.Rows.Count > 0)
            {
                DataRow drSitenum = dbAccess.DataSet.Tables["SitenumTB"].Rows[0];

                Sitenum = drSitenum["Sitenum"].ToString();

            }
            SitenumTB.Dispose(); ;
            return Sitenum;
        }


        public static string GetWhnum(string Site, DBAccess dbAccess)
        {
            string GetWhnum = "Select whnum from whm where sitenum='" + Site + "'";

            string Whnum = "";

            dbAccess.ReadSQL("WhnumTB", GetWhnum);

            DataTable WhnumTB = dbAccess.DataSet.Tables["WhnumTB"];

            if (WhnumTB.Rows.Count > 0)
            {
                DataRow drWhnum = dbAccess.DataSet.Tables["WhnumTB"].Rows[0];

                Whnum = drWhnum["whnum"].ToString();

            }
            WhnumTB.Dispose(); ;
            return Whnum;
        }


        public static string GetSitenum(string Whnum, DBAccess dbAccess)
        {
            string GetSitenum = "Select top 1 sitenum from whm where whnum='" + Whnum + "'";

            string Sitenum = "";

            dbAccess.ReadSQL("SitenumWHTB", GetSitenum);

            DataTable SitenumWHTB = dbAccess.DataSet.Tables["SitenumWHTB"];

            if (SitenumWHTB.Rows.Count > 0)
            {
                DataRow drSitenum = dbAccess.DataSet.Tables["SitenumWHTB"].Rows[0];

                Sitenum = drSitenum["Sitenum"].ToString();

            }
            SitenumWHTB.Dispose(); ;
            return Sitenum;
        }


        public static string GetSitenname(string sitenum, DBAccess dbAccess)
        {
            string GetSitename = "Select top 1 sitename from sitm where sitenum='" + sitenum + "'";

            string Sitename = "";

            dbAccess.ReadSQL("SitenameTB", GetSitename);

            DataTable SitenameTB = dbAccess.DataSet.Tables["SitenameTB"];

            if (SitenameTB.Rows.Count > 0)
            {
                DataRow drSitename = dbAccess.DataSet.Tables["SitenameTB"].Rows[0];

                Sitename = drSitename["Sitename"].ToString();

            }
            SitenameTB.Dispose(); ;
            return Sitename;
        }

        public static string Getarname(string arnum, DBAccess dbAccess)
        {
            string GetArname = "Select top 1 arname from arm where arnum='" + arnum + "'";

            string Arname = "";

            dbAccess.ReadSQL("ArnameTB", GetArname);

            DataTable ArnameTB = dbAccess.DataSet.Tables["ArnameTB"];

            if (ArnameTB.Rows.Count > 0)
            {
                DataRow drArname = dbAccess.DataSet.Tables["ArnameTB"].Rows[0];

                Arname = drArname["Arname"].ToString();

            }
            ArnameTB.Dispose(); ;

            return Arname;
        }

        public static decimal GetPriceByPeriodMATS(string matnum,DateTime trandate,DBAccess dbAccess,int year)
        {
            decimal value = 0;

            int period = 0;

            period = BizAccounts.GetPeriod(dbAccess, trandate);
                    //(DBAccess dbAccess, DateTime trandate)

            string query = "select top 1 std" + Convert.ToString(period) + " as price from mats"+Convert.ToString(year)+" where matnum='"+matnum+"'";

            dbAccess.ReadSQL("matnumPriceTB", query);

            DataTable matnumPriceTB = dbAccess.DataSet.Tables["matnumPriceTB"];

            if (matnumPriceTB.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(matnumPriceTB.Rows[0]["price"]))
                {
                    value = Convert.ToDecimal(matnumPriceTB.Rows[0]["price"]);
                }
            }

            return value;
        }


        public static string GetMatnumPUOM(string matnum,  DBAccess dbAccess)
        {
            string value = "";


            string query = "select puom from matm where matnum='"+matnum+"'";

            dbAccess.ReadSQL("matnumPUOMTB", query);

            DataTable matnumPUOMTB = dbAccess.DataSet.Tables["matnumPUOMTB"];

            if (matnumPUOMTB.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(matnumPUOMTB.Rows[0]["puom"]))
                {
                    value = Convert.ToString(matnumPUOMTB.Rows[0]["puom"]);
                }
            }

            return value;
        }


        public static decimal GetMatmStdCost(string matnum, DBAccess dbAccess)
        {
            decimal value = 0;


            string query = "select ISNULL(stdcost,0) as price from MATM where matnum='" + matnum + "'";

            dbAccess.ReadSQL("matnumPriceTB", query);

            DataTable matnumPriceTB = dbAccess.DataSet.Tables["matnumPriceTB"];

            if (matnumPriceTB.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(matnumPriceTB.Rows[0]["price"]))
                {
                    value = Convert.ToDecimal(matnumPriceTB.Rows[0]["price"]);
                }
            }

            return value;
        }


        public static decimal GetPeriodCost(string matnum, int period, DBAccess dbAccess)
        {
            decimal value = 0;


            string query = "select top 1 std" + Convert.ToString(period) + " as price from mats" + Common.DEFAULT_SYSTEM_YEAR + " where matnum='" + matnum + "'";

            dbAccess.ReadSQL("matnumPriceTB", query);

            DataTable matnumPriceTB = dbAccess.DataSet.Tables["matnumPriceTB"];

            if (matnumPriceTB.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(matnumPriceTB.Rows[0]["price"]))
                {
                    value = Convert.ToDecimal(matnumPriceTB.Rows[0]["price"]);
                }
            }

            return value;
        }


        //select sum(qty) as total from POR1 where refnum='' and matnum='' and [status]='P'

        public static decimal GetIssueDifference(string matnum, string refnum, DBAccess dbAccess)
        {
            decimal value = 0;


            string query = "select sum(qty) as total from POR1 where refnum='"+refnum+"' and matnum='"+matnum+"' and [status]='P' ";

            dbAccess.ReadSQL("matnumtotalTB", query);

            DataTable matnumPriceTB = dbAccess.DataSet.Tables["matnumtotalTB"];

            if (matnumPriceTB.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(matnumPriceTB.Rows[0]["total"]))
                {
                    value = Convert.ToDecimal(matnumPriceTB.Rows[0]["total"]);
                }
            }

            return value;
        }

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        public static bool IsEven(int value)
        {
            return value % 2 == 0;
        }

        public static string GetApnumAccnum(string apnum, DBAccess dbAccess)
        {
            string accnum = "";

            string str = "Select * from apm where apnum='"+apnum+"'";

            dbAccess.ReadSQL("TempApnumAccnum", str);

            DataTable dt = dbAccess.DataSet.Tables["TempApnumAccnum"];

            if (dt.Rows.Count > 0)
            {
                accnum = dt.Rows[0]["accnum"].ToString().Trim();
            }

            return accnum;
        }


        public static string ReplaceAt(string input, int index, char newChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            char[] chars = input.ToCharArray();
            chars[index] = newChar;
            return new string(chars);
        }

        public static string GetProjectID(string key, DBAccess dbAccess)
        {
            string Key = "";

            string str = "Select top 1 projectid from ctrh where arnum='" + key + "' and [status]<>'V' ";

            dbAccess.ReadSQL("TempProjectID", str);

            DataTable dt = dbAccess.DataSet.Tables["TempProjectID"];

            if (dt.Rows.Count > 0)
            {
                Key = dt.Rows[0]["projectid"].ToString().Trim();
            }

            return Key;
        }



        public static string GetProjectIDFromSitenum(string key, DBAccess dbAccess)
        {
            string Key = "";

            string str = "Select top 1 projectid from ctrh where sitenum='" + key + "' and [status]<>'V' ";

            dbAccess.ReadSQL("TempProjectIDFomSitm", str);

            DataTable dt = dbAccess.DataSet.Tables["TempProjectIDFomSitm"];

            if (dt.Rows.Count > 0)
            {
                Key = dt.Rows[0]["projectid"].ToString().Trim();
            }

            return Key;
        }




        public static string GetSitenumFromDoc(string arnum, DBAccess dbAccess)
        {
            string GetSitenum = "Select MAX(sitenum) as sitenum from ctrh where arnum='" + arnum + "' and [status]<>'V' ";

            string Sitenum = "";

            dbAccess.ReadSQL("SitenumTBDoc", GetSitenum);

            DataTable SitenumTBDoc = dbAccess.DataSet.Tables["SitenumTBDoc"];

            if (SitenumTBDoc.Rows.Count > 0)
            {

                if (!BizFunctions.IsEmpty(dbAccess.DataSet.Tables["SitenumTBDoc"].Rows[0]))
                {
                    DataRow drSitenum = dbAccess.DataSet.Tables["SitenumTBDoc"].Rows[0];

                    Sitenum = drSitenum["Sitenum"].ToString();
                }

            }
            SitenumTBDoc.Dispose(); ;
            return Sitenum;
        }


        public static string Platform
        {
            get
            {
                if (IntPtr.Size == 8)
                    return "x64";
                else
                    return "x86";
            }
        }


        public static void SendEmailByModule(DBAccess dbaccess, string Subject, string BodyMessage, string ModuleName, string Requestor)
        {

            bool isLiveEmail = true;
            string email = "";
            string GetEmail = "Select [dbo].[GET_EMAIL2]('" + Requestor + "') as email";
            dbaccess.ReadSQL("EmailTB", GetEmail);

            string GetEmail2 = "select * from enm1 where modulecode='" + ModuleName + "' and [status]<>'V' AND ISNULL(recommend,0)=1";
            dbaccess.ReadSQL("EmailTB2", GetEmail2);

            DataTable dt = dbaccess.DataSet.Tables["EmailTB"];
            DataTable dt2 = dbaccess.DataSet.Tables["EmailTB2"];

            if (isLiveEmail)
            {
            if (dt.Rows.Count > 0)
            {
                email = dt.Rows[0]["email"].ToString().Trim();

                if (!BizFunctions.IsEmpty(email))
                {
                    string cc = "";
                    string bcc = "";
                    //cc = "tatlee@atlmaintenance.com.sg, clarence@atlmaintenance.com.sg";

                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            if (dr2["sendtype"].ToString().Trim().ToUpper() == "CC")
                            {
                                cc = cc + dr2["email"].ToString() + ",";
                            }
                            else if (dr2["sendtype"].ToString().Trim().ToUpper() == "BCC")
                            {
                                bcc = bcc + dr2["email"].ToString() + ",";
                            }
                        }
                        string strCC = "";
                        string strBCC = "";
                        if (cc.Length > 0)
                        {
                            strCC = cc.Substring(cc.Length - 1);
                        }

                        if (bcc.Length > 0)
                        {
                            strBCC = bcc.Substring(bcc.Length - 1);
                        }

                        if (strCC.Trim() == ",")
                        {
                            cc = BizLogicTools.Tools.ReplaceAt(cc, cc.Length - 1, ' ');
                        }

                        if (strBCC.Trim() == ",")
                        {
                            bcc = BizLogicTools.Tools.ReplaceAt(bcc, bcc.Length - 1, ' ');
                        }


                    }

                    ATL.BizModules.Tools.SendEmail2 send = new ATL.BizModules.Tools.SendEmail2(Subject, BodyMessage, email, cc, bcc, Common.DEFAULT_SYSTEM_USERNAME, dbaccess);

                    send.SendEmail();
                }
                else
                {
                    MessageBox.Show("Cannot send email notification\n" +
                  "The user " + Requestor + " has no email set in his profile",
                  "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else
            {
                MessageBox.Show("Cannot send email notification\n" +
              "The user " + Requestor + " has no email set in his profile",
              "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        else
        {

            email = "jason.obina@outlook.com";
            string cc = "";
            string bcc = "";
            cc = "jason.obina@bizpoint-intl.com, jayobina@gmail.com";
            bcc = "jason.obina@bizpoint-intl.com, jayobina@gmail.com";

            if (dt2.Rows.Count > 0)
            {
                //foreach (DataRow dr2 in dt2.Rows)
                //{
                //    if (dr2["sendtype"].ToString().Trim().ToUpper() == "CC")
                //    {
                //        cc = cc + dr2["email"].ToString() + ",";
                //    }
                //    else if (dr2["sendtype"].ToString().Trim().ToUpper() == "BCC")
                //    {
                //        bcc = bcc + dr2["email"].ToString() + ",";
                //    }
                //}
                string strCC = "";
                string strBCC = "";
                if (cc.Length > 0)
                {
                    strCC = cc.Substring(cc.Length - 1);
                }

                if (bcc.Length > 0)
                {
                    strBCC = bcc.Substring(bcc.Length - 1);
                }

                if (strCC.Trim() == ",")
                {
                    cc = BizLogicTools.Tools.ReplaceAt(cc, cc.Length - 1, ' ');
                }

                if (strBCC.Trim() == ",")
                {
                    bcc = BizLogicTools.Tools.ReplaceAt(bcc, bcc.Length - 1, ' ');
                }


            }

            ATL.BizModules.Tools.SendEmail2 send = new ATL.BizModules.Tools.SendEmail2(Subject, BodyMessage, email, cc, bcc, Common.DEFAULT_SYSTEM_USERNAME, dbaccess);

            send.SendEmail();

        }


        }


        public static string GetEmailByEmpnum(DBAccess dbaccess, string empnum)
        {
            string email = "";
            string GetEmail = "Select [dbo].[GET_EMAIL2]('" + empnum + "') as email";
            dbaccess.ReadSQL("UserEmail", GetEmail);

            DataTable dt = dbaccess.DataSet.Tables["UserEmail"];

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!BizFunctions.IsEmpty(dt.Rows[0]["email"]))
                    {
                        email = dt.Rows[0]["email"].ToString();
                    }
                }
            }

            return email;
        }

        public static void GeneratePdf(string ReportName, string FolderName, DataSet source, object targetEvent,string refnum)
        {
            try
            {
                string RepositoryLocation = System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository");
                string projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");

                Form form = BizXmlReader.CurrentInstance.Load(projectPath + @"\\Tools\\FormPreviewWithCancel.xml", "formPreview", targetEvent, null) as Form;

                CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;
                ReportDocument crReportDocument = new ReportDocument();
                
                crReportDocument.Load(projectPath + @"\\" + FolderName + "\\Report\\" + ReportName);
                crReportDocument.SetDataSource(source);

                crystalReportViewer1.ReportSource = crReportDocument;

                crystalReportViewer1.Refresh();

                ExportOptions CrExportOptions;
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                string ServerProjDir = RepositoryLocation;
                CrDiskFileDestinationOptions.DiskFileName = RepositoryLocation + @"\\" + FolderName + "\\" + Common.DEFAULT_SYSTEM_YEAR + "\\" + refnum + ".pdf";
                if (!Directory.Exists(ServerProjDir))
                {
                    //Create a new subfolder under the current active folder
                    string newFolder = System.IO.Path.Combine(ServerProjDir, "");
                    // Create the subfolder
                    System.IO.Directory.CreateDirectory(newFolder);
                }

                CrExportOptions = crReportDocument.ExportOptions;
                {
                    CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;
                }
                crReportDocument.Export();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        //private static void DispalyCR(string Ppath, string FolderName, string ReportName, object targetEvent, DataSet source)
        //{

        //    Form form = BizXmlReader.CurrentInstance.Load(Ppath + @"\\Tools\\FormPreviewWithCancel.xml", "formPreview", targetEvent, null) as Form;

        //    CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;
        //    ReportDocument crReportDocument = new ReportDocument();

        //    crReportDocument.Load(Ppath + @"\\" + FolderName + "\\REPORT\\" + ReportName + ".rpt");
        //    crReportDocument.SetDataSource(source);

        //    crystalReportViewer1.ReportSource = crReportDocument;
        //    form.ShowDialog();
        //    form.Dispose();

        //}


    }
}

