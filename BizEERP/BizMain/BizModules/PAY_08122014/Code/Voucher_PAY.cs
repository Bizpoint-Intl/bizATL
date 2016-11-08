/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_PAY.cs
 *	Description:    Purchase Payment Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Xinyi			2006-08-17          Edit Form
 * Jer				2006-08-04			Add paste_handle, paste_onclick to enable/disable header columnchanged event
 * Jer				2006-07-10			apd extract, csh posting, gld posting
 * Jer				2006-07-08			converted xml to new core
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizDetail;
using BizRAD.BizApplication;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizTools;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;
using DEMO.MDT;
using ATL.SupplierSearch;
using ATL.GeneralTools;


using Za.Evaluator;


namespace ATL.PAY
{
	public class Voucher_PAY : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables

		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;

		protected DataGrid dgOthers;
		protected bool opened = false;
		protected decimal apd_oriamt = 0;
		protected decimal apd_postamt = 0;
		protected decimal apd_doriamt = 0;
		protected decimal apd_dpostamt = 0;
		protected decimal piv1_oriamt = 0;
		protected decimal piv1_origstamt = 0;
		protected decimal piv1_postamt = 0;
		protected decimal piv1_gstamt = 0;
		protected decimal piv1_roriamt = 0;
		protected decimal piv1_rpostamt = 0;

		protected Hashtable formsCollection = null;
		protected int TabDetail;

		protected Button btnDelete;
		protected Button btnUp;
		protected Button btnDown;
		protected Button btnMark;
		protected Button btnDuplicate;
		protected Button btnExtract;

		string formdDetailName;
		string formOthers;
		string formExcsh;
		protected TextBox txtSupplier;
		protected Label txtPayStatus;
		protected TextBox txtCsh;
        protected string flag = "";

		protected Button getIncomeExpense;
		
		GenTools genFunctions = new GenTools();			
		#endregion

		public Voucher_PAY(string moduleName, Hashtable voucherBaseHelpers,string Flag) : base("VoucherGridInfo_PAY.xml", moduleName, voucherBaseHelpers)
		{
            this.flag = moduleName;
        }

		#region Steph - To stop users from accessing more than one voucher from Purchase Payments at the same time
		protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
		{
			base.Voucher_Edit_Handle(sender, e);

			if (opened)
			{
				MessageBox.Show("You cannot open two documents at the same time.\n" +
					"To Edit/New Document, either close the document that's currently open for this module.",
					"System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			e.Handle = !opened;
		}

		protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Cancel_OnClick(sender, e);

			opened = false;
		}

		protected override void Voucher_New_Handle(object sender, VoucherHandleEventArgs e)
		{
			base.Voucher_New_Handle(sender, e);
			if (opened)
			{
				MessageBox.Show("You cannot open two documents at the same time.\n" +
					"To Edit/New Document, either close the document that's currently open for this module.",
					"System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}


			e.Handle = !opened;
		}
		#endregion

		#region Voucher Default/ALL
		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);
            e.Condition = "payt.flag='" + flag + "' AND payt.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (payt.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" payt.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" payt.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND payt.flag='"+flag+"' AND payt.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

        #region F3

		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);
			switch (e.ControlName)
			{
				case "payt_apnum":
					e.CurrentRow["contact"] = e.F2CurrentRow["ptc"];
					AutoCalc();
					break;
			}
		}


        //San*
        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);

            switch (e.MappingName)
            {
                case "matnum":
                    e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    //AutoCalc();
                    break;
            }
        }
        //San_End
        #endregion

		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			setDefaults(dbaccess.DataSet, "apd/piv1/csh/gld");

			AutoCalc();

			#region Steph - Error Checking!
			string DataEntryErrors = "Please check the following:";

			dbaccess.ReadSQL("checkAcm", "SELECT * FROM acm");

			if ((decimal)payt["amtpaid"] == 0)
			{
				if ((bool)payt["contra"] == false)
				{
					DataEntryErrors = DataEntryErrors + "\n Please key in Cheque Amount!";
				}
			}

			if (payt["chknum"].ToString().Trim() == string.Empty || payt["chknum"] == System.DBNull.Value)
			{
				DataEntryErrors = DataEntryErrors + "\n Please key in Cheque No.!";
			}

			DataTable getEmptyAcPayt = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum1 from [payt] WHERE isnull(accnum1,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcPayt.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Bank A/C No. in Header";
			}

			DataTable getEmptyAcPayt2 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum2 from [Payt] WHERE isnull(accnum2,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcPayt2.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Bank Charges A/C No. in Header";
			}

			DataTable getEmptyAcPayt3 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum3 from [Payt] WHERE isnull(accnum3,'') not in (SELECT accnum from [checkAcm]) AND discamt<>0");
			if (getEmptyAcPayt3.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Discount I A/C No. in Header";
			}

			DataTable getEmptyAcPayt4 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum4 from [Payt] WHERE isnull(accnum4,'') not in (SELECT accnum from [checkAcm]) AND flexamt<>0");
			if (getEmptyAcPayt4.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Discount II A/C No. in Header";
			}

			DataTable getEmptyAcApd = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [apd]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcApd.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. in Detail";
			}

			DataTable getEmptyAcPiv1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [piv1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcPiv1.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. in Detail";
			}

			if (DataEntryErrors != "Please check the following:")
			{
				MessageBox.Show(DataEntryErrors, "Confirm Unsuccessful");
				e.Handle = false;
			}
			else
			{

				#region Steph - Posting Time!!!

				#region steph - piv / siv posting for accnum3 - discamt
				if (dbaccess.DataSet.Tables["payt"].Select().Length > 0)
				{
					DataTable sumRecp = BizFunctions.ExecuteQuery(e.DBAccess.DataSet, "Select  payt.refnum as refnum,  payt.apnum as apnum, payt.flag as flag, payt.trandate as trandate,  payt.period as period," +
						" payt.gstper as gstper,  payt.exrate as exrate," +
						" payt.oricur3 as oricur,  payt.discamt as oriamt, payt.accnum3 as accnum, acm.acctype as acctype from [payt]" +
						"  payt  LEFT OUTER JOIN [checkAcm] acm ON payt.accnum3 = acm.accnum where payt.refnum = '" + payt["refnum"].ToString().Trim() + "' and payt.discamt <> 0");

					if (sumRecp.Rows.Count > 0)
					{
						DataRow sumSivRow = sumRecp.Rows[0];

						string discColumn = "refnum|discallow|accnum|exrate|flag|postamt|oriamt|oricur|period|oricredit|oridebit|gstamt|gstper|locno|deptno";
						string[] bankchgStrColumn = discColumn.Split('|');

						if (bankchgStrColumn.Length > 0)
						{
							#region Steph -  To post to PIV
							if (Convert.ToString(sumSivRow["acctype"]).Trim() == "2" || Convert.ToString(sumSivRow["acctype"]).Trim() == "3" || Convert.ToString(sumSivRow["acctype"]).Trim() == "4")
							{
								DataRow tmppiv = piv1.NewRow();

								for (int i = 0; i < bankchgStrColumn.Length; i++)
								{
									switch (bankchgStrColumn[i])
									{
										case "refnum":
											tmppiv["refnum"] = sumSivRow["refnum"];
											break;
										case "apnum":
											tmppiv["apnum"] = sumSivRow["apnum"];
											break;

										case "locno":
											tmppiv["locno"] = payt["locno"];
											break;
										case "deptno":
											tmppiv["deptno"] = payt["deptno"];
											break;

										case "period":
											tmppiv["period"] = sumSivRow["period"];
											break;
										case "accnum":
											tmppiv["accnum"] = sumSivRow["accnum"];
											break;
										case "trandate":
											tmppiv["trandate"] = sumSivRow["trandate"];
											break;
										case "lgr":
											tmppiv["lgr"] = "PIV";
											break;
										case "discallow":
											tmppiv["discallow"] = "Y";
											break;
										case "flag":
											tmppiv["flag"] = sumSivRow["flag"];
											break;
										case "exrate":
											if (sumSivRow["exrate"] == System.DBNull.Value)
											{
												sumSivRow["exrate"] = 1;
											}
											tmppiv["exrate"] = Convert.ToDecimal(sumSivRow["exrate"]);
											break;
										case "oricur":
											tmppiv["oricur"] = sumSivRow["oricur"];
											break;
										case "oriamt":
											#region To replace the null value
											if (sumSivRow["oriamt"] == System.DBNull.Value)
											{
												sumSivRow["oriamt"] = 0;
											}
											#endregion
											tmppiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])));
											break;
										case "postamt":
											#region To replace the null value
											if (sumSivRow["exrate"] == System.DBNull.Value)
											{
												sumSivRow["exrate"] = 1;
											}
											if (sumSivRow["oriamt"] == System.DBNull.Value)
											{
												sumSivRow["oriamt"] = 0;
											}
											#endregion
											tmppiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])) * (Convert.ToDecimal(sumSivRow["exrate"])));
											break;
										case "gstper":
											tmppiv["gstper"] = Convert.ToDecimal(sumSivRow["gstper"]);
											break;
										case "gstamt":
											tmppiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]) - Convert.ToDecimal(tmppiv["oriamt"]));
											break;
										case "oridebit":
											if ((decimal)sumSivRow["oriamt"] > 0)
											{
												tmppiv["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
											}
											else
											{
												tmppiv["oridebit"] = 0;
											}
											break;
										case "oricredit":
											if ((decimal)sumSivRow["oriamt"] < 0)
											{
												tmppiv["oricredit"] = 0;
											}
											else
											{
												tmppiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
											}
											break;
										default:
											tmppiv[bankchgStrColumn[i]] = sumSivRow[bankchgStrColumn[i]];
											break;
									}
								}

								piv1.Rows.Add(tmppiv.ItemArray);
							}
							#endregion

							#region Steph -To post to SIV
							if (Convert.ToString(sumSivRow["acctype"]).Trim() == "0" || Convert.ToString(sumSivRow["acctype"]).Trim() == "1")
							{
								DataRow tmpsiv = siv1.NewRow();

								for (int i = 0; i < bankchgStrColumn.Length; i++)
								{
									switch (bankchgStrColumn[i])
									{
										case "refnum":
											tmpsiv["refnum"] = sumSivRow["refnum"];
											break;
										case "apnum":
											tmpsiv["apnum"] = sumSivRow["apnum"];
											break;
										case "locno":
											tmpsiv["locno"] = payt["locno"];
											break;
										case "deptno":
											tmpsiv["deptno"] = payt["deptno"];
											break;
										case "period":
											tmpsiv["period"] = sumSivRow["period"];
											break;
										case "accnum":
											tmpsiv["accnum"] = sumSivRow["accnum"];
											break;
										case "trandate":
											tmpsiv["trandate"] = sumSivRow["trandate"];
											break;
										case "lgr":
											tmpsiv["lgr"] = "SIV";
											break;
										case "discallow":
											tmpsiv["discallow"] = "Y";
											break;
										case "flag":
											tmpsiv["flag"] = sumSivRow["flag"];
											break;
										case "exrate":
											tmpsiv["exrate"] = Convert.ToDecimal(sumSivRow["exrate"]);
											break;
										case "oricur":
											tmpsiv["oricur"] = sumSivRow["oricur"];
											break;
										case "oriamt":
											tmpsiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])));
											break;
										case "postamt":
											tmpsiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])) * (Convert.ToDecimal(sumSivRow["exrate"])));
											break;
										case "gstper":
											tmpsiv["gstper"] = Convert.ToDecimal(sumSivRow["gstper"]);
											break;
										case "gstamt":
											tmpsiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]) - Convert.ToDecimal(tmpsiv["oriamt"]));
											break;
										case "oridebit":
											if ((decimal)sumSivRow["oriamt"] > 0)
											{
												tmpsiv["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
											}
											else
											{
												tmpsiv["oridebit"] = 0;
											}
											break;
										case "oricredit":
											if ((decimal)sumSivRow["oriamt"] < 0)
											{
												tmpsiv["oricredit"] = 0;
											}
											else
											{
												tmpsiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
											}
											break;
										default:
											tmpsiv[bankchgStrColumn[i]] = sumSivRow[bankchgStrColumn[i]];
											break;
									}
								}
								siv1.Rows.Add(tmpsiv.ItemArray);
							}
							#endregion

							#region Steph -Contra Account - To post to Cash table
							if (Convert.ToString(sumSivRow["acctype"]).Trim() != "0" && Convert.ToString(sumSivRow["acctype"]).Trim() != "1" && Convert.ToString(sumSivRow["acctype"]).Trim() != "2" && Convert.ToString(sumSivRow["acctype"]).Trim() != "3" && Convert.ToString(sumSivRow["acctype"]).Trim() != "4")
							{
								DataRow tmpcsh = csh.NewRow();

								for (int i = 0; i < bankchgStrColumn.Length; i++)
								{
									switch (bankchgStrColumn[i])
									{
										case "refnum":
											tmpcsh["refnum"] = sumSivRow["refnum"];
											//steph 16 Apr 2010 - as piv and siv does not contain exramt column, 
											//and the exramt cannot be null value (postgld), assign a zero value to exramt column.
											tmpcsh["exramt"] = 0;
											break;
										case "apnum":
											tmpcsh["apnum"] = sumSivRow["apnum"];
											break;
										case "locno":
											tmpcsh["locno"] = payt["locno"];
											break;
										case "deptno":
											tmpcsh["deptno"] = payt["deptno"];
											break;
										case "period":
											tmpcsh["period"] = sumSivRow["period"];
											break;
										case "accnum":
											tmpcsh["accnum"] = sumSivRow["accnum"];
											break;
										case "trandate":
											tmpcsh["trandate"] = sumSivRow["trandate"];
											break;
										case "lgr":
											tmpcsh["lgr"] = "SIV";
											break;
										case "discallow":
											tmpcsh["discallow"] = "Y";
											break;
										case "flag":
											tmpcsh["flag"] = sumSivRow["flag"];
											break;
										case "exrate":
											if (sumSivRow["exrate"] == System.DBNull.Value)
											{
												sumSivRow["exrate"] = 1;
											}
											tmpcsh["exrate"] = Convert.ToDecimal(sumSivRow["exrate"]);
											break;
										case "oricur":
											tmpcsh["oricur"] = sumSivRow["oricur"];
											break;
										case "oriamt":
											tmpcsh["oriamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]));
											break;
										case "postamt":
											tmpcsh["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * (Convert.ToDecimal(sumSivRow["exrate"])));
											break;
										case "gstper":
											tmpcsh["gstper"] = Convert.ToDecimal(sumSivRow["gstper"]);
											break;
										case "gstamt":
											tmpcsh["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]) - Convert.ToDecimal(tmpcsh["oriamt"]));
											break;
										case "oridebit":
											if ((decimal)sumSivRow["oriamt"] > 0)
											{
												tmpcsh["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
											}
											else
											{
												tmpcsh["oridebit"] = 0;
											}
											break;
										case "oricredit":
											if ((decimal)sumSivRow["oriamt"] < 0)
											{
												tmpcsh["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
											}
											else
											{
												tmpcsh["oricredit"] = 0;
											}
											break;
										default:
											tmpcsh[bankchgStrColumn[i]] = sumSivRow[bankchgStrColumn[i]];
											break;
									}
								}
								csh.Rows.Add(tmpcsh.ItemArray);
							}
							#endregion
						}
					}
				}

				#endregion

				#region steph - piv / siv posting for accnum4 - flexamt
				if (dbaccess.DataSet.Tables["payt"].Select().Length > 0)
				{

					dbaccess.ReadSQL("sumFlex", " Select  payt.refnum as refnum,  payt.apnum as apnum, payt.flag as flag, payt.trandate as trandate,  payt.period as period," +
						" payt.gstper as gstper,  payt.exrate as exrate," +
						" payt.oricur4 as oricur,  payt.flexamt as oriamt, payt.accnum4 as accnum, acm.acctype as acctype from  " + dbaccess.DataSet.Tables["payt"] +
						"  payt  LEFT OUTER JOIN acm ON payt.accnum4 = acm.accnum where payt.refnum = '" + payt["refnum"].ToString().Trim() + "' and payt.flexamt <> 0");

					if (dbaccess.DataSet.Tables["sumFlex"].Select().Length > 0)
					{
						DataRow sumFlexRow = dbaccess.DataSet.Tables["sumFlex"].Rows[0];

						string discColumn = "refnum|discallow|accnum|exrate|flag|postamt|oriamt|oricur|period|oricredit|oridebit|gstamt|gstper";
						string[] bankchgStrColumn = discColumn.Split('|');

						if (bankchgStrColumn.Length > 0)
						{
							#region Steph - To post the flexamt to PIV
							if (Convert.ToString(sumFlexRow["acctype"]).Trim() == "2" || Convert.ToString(sumFlexRow["acctype"]).Trim() == "3" || Convert.ToString(sumFlexRow["acctype"]).Trim() == "4")
							{
								DataRow tmppiv = piv1.NewRow();

								for (int i = 0; i < bankchgStrColumn.Length; i++)
								{
									switch (bankchgStrColumn[i])
									{
										case "refnum":
											tmppiv["refnum"] = sumFlexRow["refnum"];
											break;
										case "apnum":
											tmppiv["apnum"] = sumFlexRow["apnum"];
											break;
										case "locno":
											tmppiv["locno"] = payt["locno"];
											break;
										case "deptno":
											tmppiv["deptno"] = payt["deptno"];
											break;
										case "period":
											tmppiv["period"] = sumFlexRow["period"];
											break;
										case "accnum":
											tmppiv["accnum"] = sumFlexRow["accnum"];
											break;
										case "trandate":
											tmppiv["trandate"] = sumFlexRow["trandate"];
											break;
										case "lgr":
											tmppiv["lgr"] = "PIV";
											break;
										case "discallow":
											tmppiv["discallow"] = "Y";
											break;
										case "flag":
											tmppiv["flag"] = sumFlexRow["flag"];
											break;
										case "exrate":
											if (sumFlexRow["exrate"] == System.DBNull.Value)
											{
												sumFlexRow["exrate"] = 1;
											}
											tmppiv["exrate"] = Convert.ToDecimal(sumFlexRow["exrate"]);
											break;
										case "oricur":
											tmppiv["oricur"] = sumFlexRow["oricur"];
											break;
										case "oriamt":
											tmppiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])));
											break;
										case "postamt":
											tmppiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])) * (Convert.ToDecimal(sumFlexRow["exrate"])));
											break;
										case "gstper":
											tmppiv["gstper"] = Convert.ToDecimal(sumFlexRow["gstper"]);
											break;
										case "gstamt":
											tmppiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]) - Convert.ToDecimal(tmppiv["oriamt"]));
											break;
										case "oridebit":
											if ((decimal)sumFlexRow["oriamt"] < 0)
											{
												tmppiv["oricredit"] = 0;
											}
											else
											{
												tmppiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
											}
											break;
										case "oricredit":
											if ((decimal)sumFlexRow["oriamt"] < 0)
											{
												tmppiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
											}
											else
											{
												tmppiv["oricredit"] = 0;
											}
											break;
										default:
											tmppiv[bankchgStrColumn[i]] = sumFlexRow[bankchgStrColumn[i]];
											break;
									}
								}

								piv1.Rows.Add(tmppiv.ItemArray);
							}

							#endregion

							#region Steph  - To post the flexamt to SIV
							if (Convert.ToString(sumFlexRow["acctype"]).Trim() == "0" || Convert.ToString(sumFlexRow["acctype"]).Trim() == "1")
							{
								DataRow tmpsiv = siv1.NewRow();

								for (int i = 0; i < bankchgStrColumn.Length; i++)
								{
									switch (bankchgStrColumn[i])
									{
										case "refnum":
											tmpsiv["refnum"] = sumFlexRow["refnum"];
											break;
										case "apnum":
											tmpsiv["apnum"] = sumFlexRow["apnum"];
											break;
										case "locno":
											tmpsiv["locno"] = payt["locno"];
											break;
										case "deptno":
											tmpsiv["deptno"] = payt["deptno"];
											break;
										case "period":
											tmpsiv["period"] = sumFlexRow["period"];
											break;
										case "accnum":
											tmpsiv["accnum"] = sumFlexRow["accnum"];
											break;
										case "trandate":
											tmpsiv["trandate"] = sumFlexRow["trandate"];
											break;
										case "lgr":
											tmpsiv["lgr"] = "SIV";
											break;
										case "discallow":
											tmpsiv["discallow"] = "Y";
											break;
										case "flag":
											tmpsiv["flag"] = sumFlexRow["flag"];
											break;
										case "exrate":
											if (sumFlexRow["exrate"] == System.DBNull.Value)
											{
												sumFlexRow["exrate"] = 1;
											}
											tmpsiv["exrate"] = Convert.ToDecimal(sumFlexRow["exrate"]);
											break;
										case "oricur":
											tmpsiv["oricur"] = sumFlexRow["oricur"];
											break;
										case "oriamt":
											tmpsiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])));
											break;
										case "postamt":
											tmpsiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])) * (Convert.ToDecimal(sumFlexRow["exrate"])));
											break;
										case "gstper":
											tmpsiv["gstper"] = Convert.ToDecimal(sumFlexRow["gstper"]);
											break;
										case "gstamt":
											tmpsiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]) - Convert.ToDecimal(tmpsiv["oriamt"]));
											break;
										case "oridebit":
											tmpsiv["oridebit"] = (Convert.ToDecimal(sumFlexRow["oriamt"]));
											break;
										case "oricredit":
											tmpsiv["oricredit"] = 0.00;
											break;
										default:
											tmpsiv[bankchgStrColumn[i]] = sumFlexRow[bankchgStrColumn[i]];
											break;
									}
								}

								siv1.Rows.Add(tmpsiv.ItemArray);
							}
							#endregion

							#region Steph - contra account - To post to Cash Table
							if (Convert.ToString(sumFlexRow["acctype"]).Trim() != "0" && Convert.ToString(sumFlexRow["acctype"]).Trim() != "1" && Convert.ToString(sumFlexRow["acctype"]).Trim() != "2" && Convert.ToString(sumFlexRow["acctype"]).Trim() != "3" && Convert.ToString(sumFlexRow["acctype"]).Trim() != "4")
							{
								DataRow tmpcsh = csh.NewRow();

								for (int i = 0; i < bankchgStrColumn.Length; i++)
								{
									switch (bankchgStrColumn[i])
									{
										case "refnum":
											tmpcsh["refnum"] = sumFlexRow["refnum"];
											//steph 16 Apr 2010- as piv and siv does not contain exramt column, 
											//and the exramt cannot be null value (postgld), assign a zero value to exramt column.
											tmpcsh["exramt"] = 0;

											break;
										case "apnum":
											tmpcsh["apnum"] = sumFlexRow["apnum"];
											break;
										case "locno":
											tmpcsh["locno"] = payt["locno"];
											break;
										case "deptno":
											tmpcsh["deptno"] = payt["deptno"];
											break;
										case "period":
											tmpcsh["period"] = sumFlexRow["period"];
											break;
										case "accnum":
											tmpcsh["accnum"] = sumFlexRow["accnum"];
											break;
										case "trandate":
											tmpcsh["trandate"] = sumFlexRow["trandate"];
											break;
										case "lgr":
											tmpcsh["lgr"] = "SIV";
											break;
										case "discallow":
											tmpcsh["discallow"] = "Y";
											break;
										case "flag":
											tmpcsh["flag"] = sumFlexRow["flag"];
											break;
										case "exrate":
											if (sumFlexRow["exrate"] == System.DBNull.Value)
											{
												sumFlexRow["exrate"] = 1;
											}
											tmpcsh["exrate"] = Convert.ToDecimal(sumFlexRow["exrate"]);
											break;
										case "oricur":
											tmpcsh["oricur"] = sumFlexRow["oricur"];
											break;
										case "oriamt":
											tmpcsh["oriamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]));
											break;
										case "postamt":
											tmpcsh["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * (Convert.ToDecimal(sumFlexRow["exrate"])));
											break;
										case "gstper":
											tmpcsh["gstper"] = Convert.ToDecimal(sumFlexRow["gstper"]);
											break;
										case "gstamt":
											tmpcsh["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]) - Convert.ToDecimal(tmpcsh["oriamt"]));
											break;
										case "oridebit":
											if ((decimal)sumFlexRow["oriamt"] > 0)
											{
												tmpcsh["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
											}
											else
											{
												tmpcsh["oridebit"] = 0;
											}
											break;
										case "oricredit":
											if ((decimal)sumFlexRow["oriamt"] < 0)
											{
												tmpcsh["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
											}
											else
											{
												tmpcsh["oricredit"] = 0.00;
											}
											break;
										default:
											tmpcsh[bankchgStrColumn[i]] = sumFlexRow[bankchgStrColumn[i]];
											break;
									}
								}
								csh.Rows.Add(tmpcsh.ItemArray);
							}
							#endregion
						}
					}
				}

				#endregion

				#region Steph - To copy from ExCsh to Csh for the changing of currency receipts.
				string refnum = this.dbaccess.DataSet.Tables["payt"].Rows[0]["refnum"].ToString().Trim();
				DataTable ExCsh = dbaccess.DataSet.Tables["ExCsh"];
				if (!refnum.Equals(""))
				{
					for (int j = 0; j < ExCsh.Rows.Count; j++)
					{
						DataRow dr_Csh = this.dbaccess.DataSet.Tables["csh"].NewRow();
						for (int i = 0; i < ExCsh.Columns.Count; i++)
						{
							if (dr_Csh.RowState != DataRowState.Deleted)
							{
								if (ExCsh.Columns[i].ColumnName != "ID")
								{
									if (this.dbaccess.DataSet.Tables["csh"].Columns.IndexOf(ExCsh.Columns[i].ColumnName) != -1)
									{
										dr_Csh[ExCsh.Columns[i].ColumnName] = ExCsh.Rows[j][i];
										dr_Csh["exramt"] = 0;
										dr_Csh["locno"] = payt["locno"];
										dr_Csh["deptno"] = payt["deptno"];
									}
								}
							}
						}
						this.dbaccess.DataSet.Tables["csh"].Rows.Add(dr_Csh);
					}
				}
				#endregion

				#region Steph - The MDT posting statement
				MDTReader.updateAccountCsh(ref this.dbaccess, "refnum|trandate|status|" +
					"accnum|apnum|exrate|detail|flag|lgr|postamt|oriamt|bankamt|oricur|period|"+
					"chknum|oricredit|remark|oridebit|locno|deptno|exramt",
									"PAY", "APD", "PAYT", "csh", "piv1", "Purchase Payment - Extraction");
				#endregion

				#region steph - Need to post the header's remark into GLD.
				foreach (DataRow dr in gld.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["detail"] = payt["remark"];
						if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
							dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.
					}
				}
				#endregion

				#endregion
			}
			#endregion

		}

		#region Tab Control
		protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_TabControl_Handle(sender, e);

			TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
		}
		protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
		{
			base.Document_TabControl_OnSelectionChanged(sender, e);
			btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
			btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
			btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;
			btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
			btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
			btnExtract = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Extract") as Button;

			switch ((sender as LXFClassLibrary.Controls.TabControl).SelectedIndex)
			{
				case 0:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnExtract.Enabled = false;
					btnMark.Enabled = true;
					break;
				case 1:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnExtract.Enabled = true;
					btnMark.Enabled = true;
					break;
				case 2:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnExtract.Enabled = false;
					btnMark.Enabled = true;
					break;
			}

		}
		#endregion
		
		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];

			//if ((decimal)payt["amtpaid"] == 0)
			//{
			//    MessageBox.Show("Please key in Cheque Amount!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    e.Handle = false;
			//}
			//if (payt["chknum"].ToString().Trim() == string.Empty || payt["chknum"] == System.DBNull.Value)
			//{
			//    MessageBox.Show("Please key in Cheque No.!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    e.Handle = false;
			//}
		}

		protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
		{
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			base.Document_Extract_Handle (sender, e);
			DataRow payt = e.DBAccess.DataSet.Tables["payt"].Rows[0];

			#region Extraction Checking
			if(payt["apnum"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Please select supplier code before Extracting !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				e.Handle = false;
			}
			#endregion

			#region Steph - MDT Extraction
			Hashtable HsExtract = MDTReader.GetExtraction("pay", "APD EXTRACT", TabDetail, this.dbaccess);

			if (HsExtract.Count > 0)
			{
				ExtractGrid extract = new ExtractGrid(this.dbaccess, "extract", HsExtract["DestinationTable"].ToString().Trim(), HsExtract["colDisplay"].ToString().Trim(), HsExtract["colCopy"].ToString().Trim(),
									HsExtract["sqlDisplay"].ToString().Trim(), HsExtract["sqlCopy"].ToString().Trim(), HsExtract["extractkey"].ToString().Trim(), Convert.ToBoolean(HsExtract["inclextracted"]));
				extract.showGrid();
			}
			else
			{
				MessageBox.Show("Error in data extraction");
			}
			#endregion

			AutoCalc();

			#region Steph - Re-activate  the buttons
			btnDelete.Enabled = true;
			btnUp.Enabled = true;
			btnDown.Enabled = true;
			btnDuplicate.Enabled = true;
			btnExtract.Enabled = true;
			btnMark.Enabled = true;
			#endregion

			e.Handle = false;
		}
		
		protected override void Document_Extract_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Extract_OnClick(sender, e);
		}

		protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Preview_Handle (sender, e);
			DataRow payt = e.DBAccess.DataSet.Tables["payt"].Rows[0];

			if (payt["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
			{
				if (BizValidate.CheckRowState(e.DBAccess.DataSet, "payt/apd/piv1"))
				{
					MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					e.Handle = false;
				}
			}
		}

		#endregion

		#region Document Event

		#region Form Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad (sender, e);
			opened = true;

			DataRow payt = e.DBAccess.DataSet.Tables["payt"].Rows[0];
			DataTable apd = e.DBAccess.DataSet.Tables["apd"];
			DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];

			#region Grab the items from the form
			this.formdDetailName = (e.FormsCollection["header"] as Form).Name;
			this.formOthers = (e.FormsCollection["others"] as Form).Name;
			this.formExcsh = (e.FormsCollection["ExCsh"] as Form).Name;
			dgOthers = BizXmlReader.CurrentInstance.GetControl(this.formOthers, "dg_Others") as DataGrid;
			txtPayStatus = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_payStatus") as Label; //To show whether this has been cleared.
			txtCsh = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_csh") as TextBox;  //To show the difference of amount paid and AR amt.
			#endregion	

			string headerFormName = (e.FormsCollection["header"] as Form).Name;
			string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;

			if (payt["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "payt_trandate", payt);
				payt["advance"] = false;
				payt["contra"] = false;
			}

			payt["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			this.dbaccess = e.DBAccess;
			getIncomeExpense = BizXmlReader.CurrentInstance.GetControl(ccDistFormName, "btn_getIncomeExpense") as Button;
			getIncomeExpense.Click += new EventHandler(getIncomeExpense_Click);

			//btnExtract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Extract") as Button;
			//btnExtract.Click += new EventHandler(btnExtract_Click);

			e.DBAccess.DataSet.Tables["payt"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PAYT_ColumnChanged);
			e.DBAccess.DataSet.Tables["apd"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_APD_ColumnChanged);
		}

		void btnExtract_Click(object sender, EventArgs e)
		{
			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];
			DataTable apd = dbaccess.DataSet.Tables["apd"];

			if (payt["apnum"].ToString() != string.Empty)
			{
				Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
				DataTable oriTable = apd;

				try
				{
					// Open Extract Form
					ExtractAPForm ExtractAP = new ExtractAPForm(this.dbaccess, oriTable);
					ExtractAP.ShowDialog(frm);
				}
				catch (Exception ex)
				{
					MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			else
			{
				MessageBox.Show("Invalid Supplier Code!");
				return;
			}
			AutoCalc();
		}

		void getIncomeExpense_Click(object sender, EventArgs e)
		{
			DataTable pivc = dbaccess.DataSet.Tables["pivc"];

			string strGetIE = "SELECT accnum,SUM(oriamt) AS oriamt,oricur,exrate FROM [piv1] GROUP BY accnum,oricur,exrate HAVING SUM(oriamt)<>0 " +
				" UNION ALL SELECT accnum2,bankchg,oricur,exrate FROM payt WHERE ISNULL(bankchg,0) <>0 " +
				" UNION ALL SELECT accnum3,discamt,oricur,exrate FROM payt WHERE ISNULL(discamt,0) <>0 " +
				" UNION ALL SELECT accnum4,flexamt,oricur,exrate FROM payt WHERE ISNULL(flexamt,0) <>0";

			DataTable getIE = BizFunctions.ExecuteQuery(dbaccess.DataSet,strGetIE);

			DialogResult result = MessageBox.Show("The entries in this page will be reset! Continue?", "Clear Data?", MessageBoxButtons.YesNo);

			if (result == DialogResult.Yes)
			{
				BizFunctions.DeleteAllRows(pivc);

				foreach (DataRow dr in getIE.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						DataRow addPivc = pivc.Rows.Add(new object[] { });
						addPivc["accnum"] = dr["accnum"];
						addPivc["oriamt"] = dr["oriamt"];
						addPivc["oricur"] = dr["oricur"];
						addPivc["exrate"] = dr["exrate"];
					}
				}
			}
			AutoCalc();
		}
		#endregion

		//void cboApname_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//    DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];
		//    payt["apnum"] = cboApname.SelectedValue;
		//}

		#region Reopen Handle

		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
			DataRow PAYT = this.dbaccess.DataSet.Tables["payt"].Rows[0];
			DataTable piv1  =this.dbaccess.DataSet.Tables["piv1"];

			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM CSH" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + PAYT["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + PAYT["refnum"].ToString().Trim() + "'");
			BizFunctions.DeleteRow(piv1, "discallow = 'Y'");
		}
		#endregion

		protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Delete_OnClick(sender, e);
			AutoCalc();
		}

		#region Insert


		protected override void Document_Insert_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Insert_Handle(sender, e);
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];

			DataView dtvView = new DataView(piv1);
			dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			//dtvView.Sort = "MATNAME";
			//cpm1.Rows.Add(new object[] { });
			dtvView.AllowNew = false;
			dgOthers.DataSource = dtvView;
		}

		protected override void Document_Delete_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Delete_Handle(sender, e);
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];

			DataView dtvView = new DataView(piv1);
			dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			//dtvView.Sort = "MATNAME";
			//cpm1.Rows.Add(new object[] { });
			dtvView.AllowNew = false;
			dgOthers.DataSource = dtvView;
		}

		protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveEnd_OnClick(sender, e);
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];

			DataView dtvView = new DataView(piv1);
			dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			//dtvView.Sort = "MATNAME";
			//cpm1.Rows.Add(new object[] { });
			dtvView.AllowNew = false;
			dgOthers.DataSource = dtvView;
		}

		protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Insert_OnClick (sender, e);
			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];			
			if (e.CurrentRow.Table.TableName.ToUpper() == "CSH")
			{
				e.CurrentRow["lgr"] = "CSH";
			}

			if (e.CurrentRow.Table.TableName.ToUpper() == "APD")
			{
				//e.CurrentRow["invnum"] = payt["refnum"];
				e.CurrentRow["invdate"] = payt["trandate"];
			}
		}

		#endregion

		#region Refresh
        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow payt = e.DBAccess.DataSet.Tables["payt"].Rows[0];
            DataTable apd = e.DBAccess.DataSet.Tables["apd"];
            DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
            DataTable csh = e.DBAccess.DataSet.Tables["csh"];
            DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
			DataTable excsh = e.DBAccess.DataSet.Tables["excsh"];
			setDefaults(dbaccess.DataSet, "payt/apd/piv1/excsh");
			 
            #region payt
			payt["oricur2"] = payt["oricur"];
			payt["dbankamt"] = (decimal)payt["amtpaid"];
			//payt["apnum"] = cboApname.SelectedValue;
            #endregion

			//DataView dtvView = new DataView(piv1);
			//dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			////dtvView.Sort = "MATNAME";
			////cpm1.Rows.Add(new object[] { });
			//dtvView.AllowNew = false;
			//dgOthers.DataSource = dtvView;

			AutoCalc();

			#region Steph - To set the correct value for oriamt, postamt using mdt
			MDTReader.SetCorrectValue(ref this.dbaccess, "PIV1", "PAY");
			MDTReader.SetCorrectValue(ref this.dbaccess, "APD", "PAY");
			MDTReader.SetCorrectValue(ref this.dbaccess, "PAYT", "PAY");
			#endregion
			//AutoSetDefaultValueNVCHeader();
			//AutoSetDefaultValueNVCDetails();
		}
		#endregion
		#endregion

		#region SaveBegin

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick (sender, e);
            DataRow payt = e.DBAccess.DataSet.Tables["payt"].Rows[0];
			DataTable apd = e.DBAccess.DataSet.Tables["apd"];
			DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
            DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable excsh = e.DBAccess.DataSet.Tables["excsh"];

			#region Steph - This is to update the detail tables
			foreach (DataRow dr in apd.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					BizFunctions.UpdateDataRow(payt, dr, "user/flag/status/created/modified");
			}

			foreach (DataRow dr in piv1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					BizFunctions.UpdateDataRow(payt, dr, "user/flag/status/created/modified");
			}
			foreach (DataRow dr in excsh.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					BizFunctions.UpdateDataRow(payt, dr, "user/flag/status/created/modified");
			}
			#endregion

            if(!BizFunctions.IsEmpty(payt["apnum"]) && BizFunctions.IsEmpty(payt["remark"]))
            {
                dbaccess.ReadSQL("getApmName", "SELECT apname FROM apm where apnum ='" + payt["apnum"].ToString().Trim() + "'");

                if (dbaccess.DataSet.Tables["getApmName"].Rows.Count > 0)
                {
                    DataRow getApmName = dbaccess.DataSet.Tables["getApmName"].Rows[0];
                    payt["remark"] = getApmName["apname"];

                }

            }
		}

		#endregion

		protected override void Document_Paste_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Paste_OnClick (sender, e);
			e.DBAccess.DataSet.Tables["payt"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PAYT_ColumnChanged);
		}	

		#region ColumnChangedEvents

		#region payt

		private void Voucher_PAYT_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];

			switch(e.Column.ColumnName)
			{
				case "apnum":
					#region Steph - Pull info from ARM
					dbaccess.ReadSQL("getApmInfo", "SELECT apnum,apname,ptc,address,phone,hp,fax,ptnum,gstgrpnum,oricur,accnum FROM apm where apnum ='" + e.Row["apnum"].ToString().Trim() + "'");

					if (dbaccess.DataSet.Tables["getApmInfo"].Rows.Count > 0)
					{
						DataRow getApmInfo = dbaccess.DataSet.Tables["getApmInfo"].Rows[0];
						e.Row["detail"] = getApmInfo["apname"];

                        #region san* - Save contact info if not empty
                        //San*
                        if (e.Row["contact"].ToString().Trim() != "")
                        {
                            payt["ptc"] = e.Row["contact"];
                        }
						//e.Row["contact"] = getApmInfo["ptc"];
                        #endregion - San_End

						e.Row["address"] = getApmInfo["address"];
						e.Row["phone"] = getApmInfo["phone"];
						e.Row["hp"] = getApmInfo["hp"];
						e.Row["fax"] = getApmInfo["fax"];

						if (e.Row["payterms"].ToString().Trim() == "" || e.Row["payterms"] == System.DBNull.Value)
							e.Row["payterms"] = getApmInfo["ptnum"];
						if (e.Row["gstgrpnum"].ToString().Trim() == "" || e.Row["gstgrpnum"] == System.DBNull.Value)
							e.Row["gstgrpnum"] = getApmInfo["gstgrpnum"];
						if (e.Row["oricur"].ToString().Trim() == "" || e.Row["oricur"] == System.DBNull.Value)
							e.Row["oricur"] = getApmInfo["oricur"];
					}
					else
					{
						e.Row["detail"] = "";
						e.Row["contact"] = "";
						e.Row["address"] = "";
						e.Row["phone"] = "";
						e.Row["hp"] = "";
						e.Row["fax"] = "";
						e.Row["payterms"] = "";
						e.Row["gstgrpnum"] = "";
						e.Row["oricur"] = "";
					}
				    break;
                    #endregion
				case "oricur":
					#region set exrate
					e.Row.BeginEdit();
					//string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					//this.dbaccess.ReadSQL("exrate", exrStr);
					//if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					//{
						//decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
					if(e.Row["exrate"] == System.DBNull.Value || Convert.ToDecimal(e.Row["exrate"]) == 0)
					{
						e.Row["exrate"] = getExrate(e.Row["oricur"].ToString(),e.Row["trandate"]);
					}
					//}
					e.Row.EndEdit();
					break;
					#endregion
				case "gstgrpnum":
					#region set gstper
					
					e.Row.BeginEdit();
					this.dbaccess.ReadSQL("gstm","SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='"+e.Row[e.Column.ColumnName].ToString()+"'");
					if(this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
					{
						if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
						{
							e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)payt["trandate"]);
						}
						else
						{
							e.Row["gstper"] = 0;
						}

					}
					e.Row.EndEdit();
					break;

					#endregion
				case "trandate":
					#region set dorh exrate
					e.Row.BeginEdit();
					if(e.Row["exrate"] == System.DBNull.Value || Convert.ToDecimal(e.Row["exrate"]) == 0)
					{
						e.Row["exrate"] = getExrate(e.Row["oricur"].ToString(),e.Row["trandate"]);
					}

					//payt["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(payt["trandate"]));
					payt["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(payt["trandate"]));

					e.Row.EndEdit();
					break;
					#endregion				
			}

		}

		#endregion

		#region apd

		private void Voucher_APD_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{
				case "oricur":
			#region set exrate
					string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					this.dbaccess.ReadSQL("exrate", exrStr);
					if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					{
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["invdate"])) + ""]);
						if (e.Row["exrate"] == System.DBNull.Value || Convert.ToDecimal(e.Row["exrate"]) == 0)
						{
							e.Row["exrate"] = exrate;
						}
					}
					break;
			#endregion
			}
		}

		#endregion

		#endregion

		#region fun fun - To set default values

		public static void setDefaults(DataSet dataSet, string tableNames)
		{
			string[] tables = tableNames.Split(new char[] { '/', '\\' });

			for (int i = 0; i < tables.Length; i++)
			{
				DataTable dt = dataSet.Tables[tables[i]];

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
		#endregion 
				
		#region Auto Calculate
		private void AutoCalc()
		{
			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];
			DataTable PAYT = dbaccess.DataSet.Tables["payt"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable excsh = dbaccess.DataSet.Tables["excsh"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];


			setDefaults(dbaccess.DataSet, "PAYT/apd/excsh/piv1");

			#region payt
			if (payt["refnum"].ToString().Trim().Contains("PAY"))
			{
				if (payt["docunum"] == System.DBNull.Value || payt["docunum"].ToString().Trim() == String.Empty)
				{
					payt["docunum"] = payt["refnum"];
				}
			}			

             dbaccess.ReadSQL("getContactInfo", "Select ptc from apm where apnum = '" + payt["apnum"] + "' ");
            if (dbaccess.DataSet.Tables["getContactInfo"].Rows.Count > 0)
            {
                if (payt["contact"].ToString() == String.Empty || payt["contact"] == System.DBNull.Value)
                {
                    payt["contact"] = dbaccess.DataSet.Tables["getContactInfo"].Rows[0]["ptc"];
                }
            }

			payt["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(payt["trandate"]));

			dbaccess.ReadSQL("readAcc", "SELECT accnum FROM acc WHERE refnum = 'BANK2'");
			if (dbaccess.DataSet.Tables["readAcc"].Rows.Count > 0)
			{
				if (payt["accnum2"] == System.DBNull.Value || payt["accnum2"].ToString().Trim() == String.Empty)
				{
					payt["accnum2"] = dbaccess.DataSet.Tables["readAcc"].Rows[0]["accnum"];
				}
			}

			#endregion

			#region initialise values
			apd_oriamt = 0;
			apd_postamt = 0;
			apd_doriamt = 0;
			apd_dpostamt = 0;
			piv1_oriamt = 0;
			piv1_origstamt = 0;
			piv1_postamt = 0;
			piv1_roriamt = 0;
			piv1_rpostamt = 0;
			piv1_gstamt = 0;
			#endregion

			#region apd


			//steph - 19 April 2010 - Add in Advance Payment function.
			bool checkExists = false;

			dbaccess.ReadSQL("checkCreditorsAccnum", "SELECT accnum FROM apm WHERE apnum = '" + payt["apnum"].ToString().Trim() + "'");

			if ((bool)payt["advance"] == true)
			{
				foreach (DataRow dr in apd.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						if (dr["locno"].ToString().Trim() == "ZZZ")
						{
							checkExists = true;

							dr["invnum"] = payt["refnum"];
							dr["pivamt"] = 0;
							if (dr["adjamt"] == System.DBNull.Value || Convert.ToDecimal(dr["adjamt"]) == 0)
							{
								dr["adjamt"] = payt["dbankamt"];
							}
							dr["invdate"] = payt["trandate"];
							dr["lgr"] = "APD";
							dr["dorigstamt"] = 0;
							dr["dgstamt"] = 0;
							dr["exramt"] = 0;

							if (dr["oricur"] == System.DBNull.Value || dr["oricur"].ToString().Trim() == String.Empty)
							{
								dr["oricur"] = payt["oricur"];
							}
							if (dr["exrate"] == System.DBNull.Value || Convert.ToDecimal(dr["exrate"]) == 0)
							{
								dr["exrate"] = getExrate(dr["oricur"].ToString().Trim(), dr["invdate"]);
							}

							if (dbaccess.DataSet.Tables["checkCreditorsAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkCreditorsAccnum"].Rows[0]["accnum"];
							}
						}
					}
				}
				if ((bool)checkExists == false)
				{
					DataRow addApd = apd.Rows.Add(new object[] { });

					BizFunctions.UpdateDataRow(payt, addApd);

					addApd["invnum"] = payt["refnum"];
					addApd["pivamt"] = 0;
					if (addApd["adjamt"] == System.DBNull.Value || Convert.ToDecimal(addApd["adjamt"]) == 0)
					{
						addApd["adjamt"] = payt["dbankamt"];
					}
					addApd["invdate"] = payt["trandate"];
					addApd["lgr"] = "APD";
					addApd["dorigstamt"] = 0;
					addApd["dgstamt"] = 0;
					addApd["exramt"] = 0;
					addApd["locno"] = "ZZZ";  //steph - used as a key to check if the entry already exists.

					if (addApd["oricur"] == System.DBNull.Value || addApd["oricur"].ToString().Trim() == String.Empty)
					{
						addApd["oricur"] = payt["oricur"];
					}
					if (addApd["exrate"] == System.DBNull.Value || Convert.ToDecimal(addApd["exrate"]) == 0)
					{
						addApd["exrate"] = getExrate(addApd["oricur"].ToString().Trim(), addApd["invdate"]);
					}

					if (dbaccess.DataSet.Tables["checkCreditorsAccnum"].Rows.Count > 0)
					{
						addApd["accnum"] = dbaccess.DataSet.Tables["checkCreditorsAccnum"].Rows[0]["accnum"];
					}
				}
			}

				

			foreach (DataRow dr in apd.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(payt, dr);
					BizFunctions.UpdateDataRow(payt, dr, "apnum/docunum/status");

					if (dr["exrate"] == System.DBNull.Value)
					{
						dr["exrate"] = payt["exrate"];
					}

					dr["chknum"] = payt["chknum"];
					
					if (dr["detail"] == System.DBNull.Value || dr["detail"].ToString().Trim() == String.Empty)
					{
						dr["detail"] = payt["remark"];
					}

					if (dr["oricur"] == System.DBNull.Value || dr["oricur"].ToString().Trim() == String.Empty)
					{
						dr["oricur"] = payt["oricur"];
					}

					if (dr["supinvnum"] == System.DBNull.Value || dr["supinvnum"].ToString().Trim() == String.Empty)
					{
						dr["supinvnum"] = dr["invnum"];
					}


					if (dr["adjamt"] == System.DBNull.Value || Convert.ToDecimal(dr["adjamt"]) == 0)
					{
						dr["adjamt"] = dr["pivamt"];
					}

					dr["doriamt"] = dr["adjamt"];

					dr["dpostamt"] = BizFunctions.Round((decimal)dr["exrate"] * (decimal)dr["doriamt"]);

					if ((decimal)dr["doriamt"] > 0)
					{
						dr["oricredit"] = 0;
						dr["oridebit"] = System.Math.Abs((decimal)dr["doriamt"]);
					}
					if ((decimal)dr["doriamt"] < 0)
					{
						dr["oricredit"] = System.Math.Abs((decimal)dr["doriamt"]);
						dr["oridebit"] = 0;
					}
					
					apd_oriamt += (decimal)dr["doriamt"];
					apd_postamt += (decimal)dr["dpostamt"];
					apd_doriamt += (decimal)dr["doriamt"];
					apd_dpostamt += (decimal)dr["dpostamt"];
				}
			}

			#endregion

			#region ExCsh

			foreach (DataRow dr in excsh.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(payt, dr);
					BizFunctions.UpdateDataRow(payt, dr, "apnum/docunum/trandate");

					dr["chknum"] = payt["chknum"];
					dr["oricur"] = dr["oricur"]; // reset oricur to trigger column changed event				
					dr["lgr"] = "csh";
					dr["oriamt"] = (decimal)dr["bankamt"];
					if ((decimal)dr["oriamt"] > 0)
					{
						dr["oricredit"] = 0;
						dr["oridebit"] = (decimal)dr["oriamt"];
					}
					if ((decimal)dr["oriamt"] < 0)
					{
						dr["oricredit"] = (decimal)dr["oriamt"];
						dr["oridebit"] = 0;
					}
					dr["postamt"] = BizFunctions.Round((decimal)dr["exrate"] * (decimal)dr["oriamt"]);
				}
			}

			#endregion

			#region piv1

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum,accnum2 FROM apm WHERE apnum = '" + payt["apnum"].ToString().Trim() + "'");

			if ((bool)payt["inclgst"])
			{
				#region Steph - Inclusive GST calculations

				dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
					" WHERE gstgrpnum ='" + payt["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in piv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(payt, dr, "createdby/apnum/docunum/oricur/exrate/gstgrpnum/gstper");

						if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
						{
							if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
							}
						}

						dr["roriamt"] = BizFunctions.Round((Convert.ToDecimal(dr["dqty"]) * Convert.ToDecimal(dr["price"])) - Convert.ToDecimal(dr["discamt"]));
						dr["rpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(dr["exrate"]));
						//dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm "+
						//    " WHERE gstgrpnum ='"+dr["gstgrpnum"].ToString().Trim()+"' AND gsttype=3");
						if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
						{
							//steph  - do not have to calculate the gst, allow user to key in manually.
						}
						else
						{
							dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(payt["gstper"]) / (100 + Convert.ToDecimal(payt["gstper"])));
						}
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
						dr["dgrosamt"] = dr["doriamt"];

						piv1_oriamt += (decimal)dr["doriamt"];
						piv1_origstamt += (decimal)dr["dorigstamt"];
						piv1_postamt += (decimal)dr["dpostamt"];
						piv1_gstamt += (decimal)dr["dgstamt"];
					}
				}
				#region Steph - Check the gst amt differences and add/deduct from the first entry of piv1
				if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
				{
					//steph  - do not have to calculate the gst, allow user to key in manually.
				}
				else
				{
					if (piv1.Rows.Count > 0)
					{
						decimal headerGst = BizFunctions.Round((piv1_oriamt + piv1_origstamt) * Convert.ToDecimal(payt["gstper"]) / (100 + Convert.ToDecimal(payt["gstper"])));
						if (headerGst != piv1_origstamt)
						{
							piv1.Rows[0]["dorigstamt"] = Convert.ToDecimal(piv1.Rows[0]["dorigstamt"]) + headerGst - piv1_origstamt;
						}
					}

					#region initialise values
					piv1_oriamt = 0;
					piv1_origstamt = 0;
					piv1_postamt = 0;
					piv1_gstamt = 0;
					#endregion

					foreach (DataRow dr in piv1.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
							dr["dgrosamt"] = dr["doriamt"];
							dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
							dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

							piv1_oriamt += (decimal)dr["doriamt"];
							piv1_origstamt += (decimal)dr["dorigstamt"];
							piv1_postamt += (decimal)dr["dpostamt"];
							piv1_gstamt += (decimal)dr["dgstamt"];
							piv1_roriamt += (decimal)dr["roriamt"];
							piv1_rpostamt += (decimal)dr["rpostamt"];

						}
					}
				}
				#endregion
				#endregion
			}
			else
			{
				#region Steph - Exclusive GST calculations

				dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
					" WHERE gstgrpnum ='" + payt["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in piv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						BizFunctions.UpdateDataRow(payt, dr);
						BizFunctions.UpdateDataRow(payt, dr, "createdby/apnum/docunum/oricur/exrate/gstgrpnum/gstper");

						if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
						{
							if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
							}
						}

						dr["dgrosamt"] = BizFunctions.Round((decimal)dr["dqty"] * (decimal)dr["price"]);
						dr["doriamt"] = (decimal)dr["dgrosamt"];
						//dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm "+
						//    " WHERE gstgrpnum ='"+dr["gstgrpnum"].ToString().Trim()+"' AND gsttype=3");
						if (dbaccess.DataSet.Tables["checkGST2"].Rows.Count > 0)
						{
							//steph  - do not have to calculate the gst, allow user to key in manually.
						}
						else
						{
							dr["dorigstamt"] = BizFunctions.Round((decimal)dr["doriamt"] * ((decimal)dr["gstper"] / 100));
						}
						dr["dpostamt"] = BizFunctions.Round((decimal)dr["doriamt"] * (decimal)dr["exrate"]);
						dr["dgstamt"] = BizFunctions.Round((decimal)dr["dorigstamt"] * (decimal)dr["exrate"]);
						dr["roriamt"] = BizFunctions.Round((decimal)dr["doriamt"] + (decimal)dr["dorigstamt"]);
						dr["rpostamt"] = BizFunctions.Round((decimal)dr["dpostamt"] + (decimal)dr["dgstamt"]);

						piv1_oriamt += (decimal)dr["doriamt"];
						piv1_origstamt += (decimal)dr["dorigstamt"];
						piv1_postamt += (decimal)dr["dpostamt"];
						piv1_gstamt += (decimal)dr["dgstamt"];
						piv1_roriamt += (decimal)dr["roriamt"];
						piv1_rpostamt += (decimal)dr["rpostamt"];

					}
				}
				#endregion
			}

			#region set Header Page oriamt/origstamt/oritotalamt/postamt/gstamt/totalamt
			payt["aptotal"]= apd_doriamt;
			payt["pivtotal"] = piv1_oriamt+piv1_origstamt;
			payt["origstamt"] = piv1_origstamt;
			payt["oritotalamt"] = (decimal)payt["oriamt"] + (decimal)payt["origstamt"];
			payt["oriamt"] = (decimal)piv1_roriamt + (decimal)apd_doriamt;
			payt["postamt"] = (decimal)piv1_rpostamt + (decimal)apd_dpostamt;
			payt["gstamt"] = BizFunctions.Round((decimal)payt["postamt"] * ((decimal)payt["gstper"] / 100));
			payt["totalamt"] = (decimal)payt["postamt"] + (decimal)payt["gstamt"];
			#endregion

			#region Calculation of differences
			decimal steDiff = 0;
			decimal ApTotal = 0;
			decimal Piv1Total = 0;
			decimal CshPaid = 0;

			ApTotal = apd_postamt;
			Piv1Total = piv1_postamt;

			if (payt["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSP) // if confim only post into ard ,csh and gld
			{
				CshPaid = BizFunctions.Round((Convert.ToDecimal(payt["amtpaid"]) + Convert.ToDecimal(payt["flexamt"])
					+ Convert.ToDecimal(payt["flexamt"])) * Convert.ToDecimal(payt["exrate"]));

				steDiff =	ApTotal+Piv1Total - CshPaid;

				txtPayStatus.Text = "AP Rounding = " + steDiff.ToString().Trim();

			}
			#endregion


			#endregion
		
		}
		#endregion
		
		#region Steph - Set Default For Header PAYT
		private void AutoSetDefaultValueNVCHeader()
		{
			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];
			#region Steph - Setting of Default Value
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "oricur1");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "oricur2");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "oricur3");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "oricur4");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "accnum1");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "accnum2");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "accnum3");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "payt", "accnum4");
			#endregion

		}
		#endregion

		#region Steph - Set Default For Details APD,PIV1,EXCSH
		private void AutoSetDefaultValueNVCDetails()
		{
			DataRow payt = dbaccess.DataSet.Tables["payt"].Rows[0];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "apd", "accnum");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, payt["flag"].ToString().Trim(), "piv1", "accnum");
		}
		#endregion

		protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick(sender, e);
			DataRow payt = e.DBAccess.DataSet.Tables["payt"].Rows[0];
			Hashtable selectedCollection = new Hashtable();
			switch (e.ReportName)
			{
				case "Payment (Creditors)":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("apm", "SELECT top 1 * FROM apm WHERE apnum='" + payt["apnum"].ToString().Trim() + "'");
					selectedCollection.Add("apd", "SELECT * FROM apd" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum = '" + payt["refnum"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);                    
					e.DataSource = e.DBAccess.DataSet;
					break;
				case "Payment (Others)":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("apm", "SELECT top 1 * FROM apm where apnum='" + payt["apnum"].ToString().Trim() + "'");
					selectedCollection.Add("piv1", "SELECT * FROM piv1 where refnum = '" + payt["refnum"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;			
			}
		}

		private decimal getExrate(string Currency, object date1)
		{
			string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + Currency + "'";
			this.dbaccess.ReadSQL("exrate", exrStr);
			if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
			{
				decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(date1))]);
				return exrate;
			}
			else
			{
				return 0;
			}
		}		
	}
}