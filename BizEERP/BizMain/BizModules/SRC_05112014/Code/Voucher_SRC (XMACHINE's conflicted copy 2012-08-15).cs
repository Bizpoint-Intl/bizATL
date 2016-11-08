/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_SRC.cs
 *	Description:    Sales Receipt Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Xinyi			2006-08-17          Edit Form
 * Jer				2006-08-04			Add paste_handle, paste_onclick to enable/disable header columnchanged event
 * Jer				2006-07-10			ard extract, csh posting, gld posting
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
using PicoGuards.MDT;
using PicoGuards.CustomerSearch;
using PicoGuards.GeneralTools;


using Za.Evaluator;


namespace PicoGuards.SRC
{
	public class Voucher_SRC : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables

		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;

		protected DataGrid dgOthers;
		protected bool opened = false;
		protected decimal ard_oriamt = 0;
		protected decimal ard_postamt = 0;
		protected decimal ard_doriamt = 0;
		protected decimal ard_dpostamt = 0;
		protected decimal siv1_doriamt = 0;
		protected decimal siv1_dorigstamt = 0;
		protected decimal siv1_postamt = 0;
		protected decimal siv1_gstamt = 0;
		protected decimal siv1_roriamt = 0;
		protected decimal siv1_rpostamt = 0;

		protected decimal saj1_discamt = 0;
		protected decimal saj1_oriamt = 0;
		protected decimal saj1_origstamt = 0;
		protected decimal saj1_postamt = 0;
		protected decimal saj1_gstamt = 0;
		protected decimal saj1_grosamt = 0;
		protected decimal saj1_roriamt = 0;
		protected decimal saj1_rpostamt = 0;

		protected decimal dard_oriamt = 0;

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
		protected TextBox txtCustomer;
		protected Label txtRecStatus;
		protected TextBox txtCsh;
		protected Button getIncomeExpense;

		protected Button getPOS;

        protected string flag = "";

		GenTools genFunctions = new GenTools();
        protected bool check;
		#endregion

		public Voucher_SRC(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_SRC.xml", moduleName, voucherBaseHelpers)
		{
            this.flag = moduleName;
		}

		#region Voucher Default/ALL
		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);
			e.Condition = "recp.flag='"+flag+"' AND recp.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (recp.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" recp.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" recp.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND recp.flag='"+flag+"' AND recp.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

		#region Steph - To stop users from accessing more than one voucher from Sales Receipts at the same time
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

		#region F3 - San*
		protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
		{
			base.AddDetailF3Condition(sender, e);

			switch (e.MappingName)
			{
				case "matnum":
					e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
					break;
			}
		}

		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);

			//DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];

			//DataTable ard = dbaccess.DataSet.Tables["ard"];

			switch (e.ControlName)
			{
				case "recp_arnum":
					e.CurrentRow["contact"] = e.F2CurrentRow["ptc"];
					AutoCalc();
					break;
			}

		}

		#endregion - San_End
		
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

		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			dbaccess.ReadSQL("acm", "SELECT * FROM acm");

			setDefaults(dbaccess.DataSet, "siv1/piv1/ard/csh/gld");

			AutoCalc();

			#region Steph - Error Checking!
			string DataEntryErrors = "Please check the following:";

			dbaccess.ReadSQL("checkAcm", "SELECT accnum,accname FROM acm");

			if ((decimal)recp["amtrcv"] == 0)
			{
				if ((bool)recp["contra"] == false)
				{
					DataEntryErrors = DataEntryErrors + "\n Please key in Cheque Amount!";
				}
			}

			if (recp["chknum"].ToString().Trim() == string.Empty || recp["chknum"] == System.DBNull.Value)
			{
				DataEntryErrors = DataEntryErrors + "\n Please key in Cheque No.!";
			}
            if (check == false)
            {
                DataEntryErrors = DataEntryErrors + "\n Bank in Amount not tally with GrandTotal";
            }
			DataTable getEmptyAcRecp = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum1 from [recp] WHERE isnull(accnum1,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcRecp.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Bank A/C No. in Header";
			}

			DataTable getEmptyAcRecp2 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum2 from [recp] WHERE isnull(accnum2,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcRecp2.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Bank Charges A/C No. in Header";
			}

			DataTable getEmptyAcRecp3 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum3 from [recp] WHERE isnull(accnum3,'') not in (SELECT accnum from [checkAcm]) AND discamt<>0");
			if (getEmptyAcRecp3.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Discount I A/C No. in Header";
			}

			DataTable getEmptyAcRecp4 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum4 from [recp] WHERE isnull(accnum4,'') not in (SELECT accnum from [checkAcm]) AND flexamt<>0");
			if (getEmptyAcRecp4.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Discount II A/C No. in Header";
			}

			DataTable getEmptyAcArd = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [ard]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcArd.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Receivables A/C No. in Detail";
			}

			DataTable getEmptyAcSiv1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [siv1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcSiv1.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Sales (Others) A/C No. in Detail";
			}

			DataTable getEmptyAcSaj1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [saj1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcSaj1.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Sales POS A/C No. in Detail";
			}

			DataTable getEmptyAcDard = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [dard]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcDard.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Deposits A/C No. in Detail";
			}

			if (DataEntryErrors != "Please check the following:")
			{
				MessageBox.Show(DataEntryErrors, "Confirm Unsuccessful");
				e.Handle = false;
			}
			else
			{
				#region Steph - Posting time!

				#region steph - piv / siv posting for accnum3 - discamt
				if (dbaccess.DataSet.Tables["recp"].Select().Length > 0)
				{
					DataTable sumRecp = BizFunctions.ExecuteQuery(e.DBAccess.DataSet, "Select  recp.refnum as refnum,  recp.arnum as arnum, recp.flag as flag, recp.trandate as trandate,  recp.period as period," +
						" recp.gstper as gstper,  recp.exrate as exrate," +
						" recp.oricur3 as oricur,  recp.discamt as oriamt, recp.accnum3 as accnum, acm.acctype as acctype from [recp]" +
						"  recp  LEFT OUTER JOIN [acm] acm ON recp.accnum3 = acm.accnum where recp.refnum = '" + recp["refnum"].ToString().Trim() + "' and recp.discamt <> 0");

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
										case "arnum":
											tmppiv["arnum"] = sumSivRow["arnum"];
											break;
										case "locno":
											tmppiv["locno"] = recp["locno"];
											break;
										case "deptno":
											tmppiv["deptno"] = recp["deptno"];
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
										case "arnum":
											tmpsiv["arnum"] = sumSivRow["arnum"];
											break;
										case "locno":
											tmpsiv["locno"] = recp["locno"];
											break;
										case "deptno":
											tmpsiv["deptno"] = recp["deptno"];
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
										case "arnum":
											tmpcsh["arnum"] = sumSivRow["arnum"];
											break;
										case "locno":
											tmpcsh["locno"] = recp["locno"];
											break;
										case "deptno":
											tmpcsh["deptno"] = recp["deptno"];
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
				if (dbaccess.DataSet.Tables["recp"].Select().Length > 0)
				{
					DataTable sumFlex = BizFunctions.ExecuteQuery(e.DBAccess.DataSet, "Select  recp.refnum as refnum,  recp.arnum as arnum, recp.flag as flag, recp.trandate as trandate,  recp.period as period," +
						" recp.gstper as gstper,  recp.exrate as exrate," +
						" recp.oricur4 as oricur,  recp.flexamt as oriamt, recp.accnum4 as accnum, acm.acctype as acctype from  [recp]" +
						"  recp  LEFT OUTER JOIN [acm] acm ON recp.accnum4 = acm.accnum where recp.refnum = '" + recp["refnum"].ToString().Trim() + "' and recp.flexamt <> 0");

					if (sumFlex.Rows.Count > 0)
					{
						DataRow sumFlexRow = sumFlex.Rows[0];

						string discColumn = "refnum|discallow|accnum|exrate|flag|postamt|oriamt|oricur|period|oricredit|oridebit|gstamt|gstper|locno|deptno";
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
										case "arnum":
											tmppiv["arnum"] = sumFlexRow["arnum"];
											break;
										case "locno":
											tmppiv["locno"] = recp["locno"];
											break;
										case "deptno":
											tmppiv["deptno"] = recp["deptno"];
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
										case "arnum":
											tmpsiv["arnum"] = sumFlexRow["arnum"];
											break;
										case "locno":
											tmpsiv["locno"] = recp["locno"];
											break;
										case "deptno":
											tmpsiv["deptno"] = recp["deptno"];
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
											if ((decimal)sumFlexRow["oriamt"] < 0)
											{
												tmpsiv["oricredit"] = 0;
											}
											else
											{
												tmpsiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
											}
											break;
										case "oricredit":
											if ((decimal)sumFlexRow["oriamt"] < 0)
											{
												tmpsiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
											}
											else
											{
												tmpsiv["oricredit"] = 0;
											}
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
											//steph 16 Apr 2010 - as piv and siv does not contain exramt column, 
											//and the exramt cannot be null value (postgld), assign a zero value to exramt column.
											tmpcsh["exramt"] = 0;
											break;
										case "arnum":
											tmpcsh["arnum"] = sumFlexRow["arnum"];
											break;
										case "locno":
											tmpcsh["locno"] = recp["locno"];
											break;
										case "deptno":
											tmpcsh["deptno"] = recp["deptno"];
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
				string refnum = this.dbaccess.DataSet.Tables["recp"].Rows[0]["refnum"].ToString().Trim();
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
										dr_Csh["locno"] = recp["locno"];
										dr_Csh["deptno"] = recp["deptno"];
									}
								}
							}
						}
						this.dbaccess.DataSet.Tables["csh"].Rows.Add(dr_Csh);
					}
				}
				#endregion

				#region Steph - The MDT posting statement
				MDTReader.updateAccountCsh(ref this.dbaccess, "refnum|trandate|" +
					"accnum|arnum|exrate|detail|flag|lgr|postamt|oriamt|bankamt|oricur|period|chknum|"+
					"oricredit|remark|oridebit|locno|deptno|exramt|status",
									"REC", "ARD", "RECP", "csh", "siv1", "SALES RECEIPT - EXTRACTION");
				#endregion

				#region steph - Need to post the header's remark into GLD.
				foreach (DataRow dr in gld.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["detail"] = recp["remark"];
						if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
							dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.

					}
				}
				#endregion

				#endregion
			}
			#endregion
		}

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];

			//if ((decimal)recp["amtrcv"] == 0)
			//{
			//    MessageBox.Show("Please key in Cheque Amount!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    e.Handle = false;
			//}
			//if (recp["chknum"].ToString().Trim() == string.Empty || recp["chknum"] == System.DBNull.Value)
			//{
			//    MessageBox.Show("Please key in Cheque No.!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    e.Handle = false;
			//}
		}

		protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
		{
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			base.Document_Extract_Handle (sender, e);
			DataRow recp = e.DBAccess.DataSet.Tables["recp"].Rows[0];

			#region Extraction Checking
			if(recp["arnum"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Please select customer code before Extracting !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				e.Handle = false;
			}
			#endregion

			#region Steph - MDT Extraction
			Hashtable HsExtract = MDTReader.GetExtraction("src", "ARD EXTRACT", TabDetail, this.dbaccess);

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
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			if (recp["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
			{
				if (BizValidate.CheckRowState(e.DBAccess.DataSet, "recp/ard/siv1"))
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

			DataRow recp = e.DBAccess.DataSet.Tables["recp"].Rows[0];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];

			#region Grab the items from the form
			this.formdDetailName = (e.FormsCollection["header"] as Form).Name;
			this.formOthers = (e.FormsCollection["others"] as Form).Name;
			this.formExcsh = (e.FormsCollection["ExCsh"] as Form).Name;
			dgOthers = BizXmlReader.CurrentInstance.GetControl(this.formOthers, "dg_Others") as DataGrid;
			txtRecStatus = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_recStatus") as Label; //To show whether this has been cleared.
			txtCsh = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_csh") as TextBox;  //To show the difference of amount paid and AR amt.
			string headerFormName = (e.FormsCollection["header"] as Form).Name;
			string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;
            #endregion


            if (recp["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "recp_trandate", recp);
				recp["advance"] = false;
				recp["contra"] = false;
			}

			recp["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			if (recp["posdatefrom"] == System.DBNull.Value)
			{
				recp["posdatefrom"] = recp["trandate"];
			}

			if (recp["posdateto"] == System.DBNull.Value)
			{
				recp["posdateto"] = recp["trandate"];
			}

			this.dbaccess = e.DBAccess;

			getIncomeExpense = BizXmlReader.CurrentInstance.GetControl(ccDistFormName, "btn_getIncomeExpense") as Button;
			getIncomeExpense.Click += new EventHandler(getIncomeExpense_Click);

			getPOS = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_getPOS") as Button;
			getPOS.Click += new EventHandler(getPOS_Click);

			e.DBAccess.DataSet.Tables["recp"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_RECP_ColumnChanged);
			e.DBAccess.DataSet.Tables["ard"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ARD_ColumnChanged);

            #region check
            if (recp["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                check = false;
            }
            else
            {
                check = true;
            }
            #endregion
        }


		void getPOS_Click(object sender, EventArgs e)
		{
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			DataTable saj1 = dbaccess.DataSet.Tables["saj1"];
			DataTable sajpos = dbaccess.DataSet.Tables["sajpos"];
			DataTable dard = dbaccess.DataSet.Tables["dard"];

			#region Steph - Check header AR Code to see if it's invalid
			string strCheckArnum = "SELECT arnum FROM arm where arnum = '" + recp["arnum"].ToString().Trim() + "'";
			dbaccess.ReadSQL("checkArnum", strCheckArnum);
			if (dbaccess.DataSet.Tables["checkArnum"].Rows.Count == 0)
			{
				MessageBox.Show("Empty or Invalid A/R Code", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			#endregion

			#region Steph -  To check if the date range selected by users conflict with trandate selected earlier, if yes, prompt users the refnum list
			string strGetTrandateRange = "SELECT refnum,posdatefrom, posdateto " +
				" FROM RECP " +
				" WHERE outletid = '" + recp["outletid"].ToString().Trim() + "' " +
				" AND arnum ='" + recp["arnum"].ToString().Trim() + "' " +
				" AND flag ='" + recp["flag"].ToString().Trim() + "' " +
				" AND refnum <>'" + recp["refnum"].ToString().Trim() + "' " +
				" AND " +
				" ( " +
				" ( " +
				" Convert(nvarchar(8),posdatefrom,112) <= '" + Convert.ToDateTime(recp["posdatefrom"]).ToString("yyyyMMdd").Trim() + "' " +
				" AND Convert(nvarchar(8),posdateto,112) >= '" + Convert.ToDateTime(recp["posdatefrom"]).ToString("yyyyMMdd").Trim() + "' " +
				" ) " +
				" OR " +
				" ( " +
				" Convert(nvarchar(8),posdatefrom,112) <=	'" + Convert.ToDateTime(recp["posdateto"]).ToString("yyyyMMdd").Trim() + "' " +
				" AND Convert(nvarchar(8),posdateto,112) >= '" + Convert.ToDateTime(recp["posdateto"]).ToString("yyyyMMdd").Trim() + "' " +
				" ) " +
                " ) and status<>'V' ";
			dbaccess.ReadSQL("getTrandateRange", strGetTrandateRange);

			string oriString = "Please check entry below with POS date range conflict with this transaction's date range selected :";
			string listOfConflictVouchers = oriString;

			DataTable getTrandateRange = dbaccess.DataSet.Tables["getTrandateRange"];

			if (getTrandateRange.Rows.Count > 0)
			{
				foreach (DataRow dr in getTrandateRange.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						listOfConflictVouchers = listOfConflictVouchers + "\n" + dr["refnum"].ToString().Trim();
					}
				}
			}

			if (listOfConflictVouchers != oriString)
			{
				MessageBox.Show(listOfConflictVouchers, "POS Date Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			#endregion

			dbaccess.ReadSQL("readPosInfoFromArm", "SELECT outletid, isCoach FROM arm " +
				" WHERE arnum = '" + recp["arnum"].ToString().Trim() + "'");

			string strGetPOS = String.Empty;

			if (dbaccess.DataSet.Tables["readPosInfoFromArm"].Rows.Count > 0)
			{
				DataRow readPosInfoFromArm = dbaccess.DataSet.Tables["readPosInfoFromArm"].Rows[0];

				if (readPosInfoFromArm["outletid"].ToString().Trim() != String.Empty)
				{
					recp["outletid"] = readPosInfoFromArm["outletid"];

					if (readPosInfoFromArm["isCoach"] == System.DBNull.Value)
						readPosInfoFromArm["isCoach"] = false;

					if ((bool)readPosInfoFromArm["isCoach"] == true)
					{
						#region Steph - for AR as coach  --commented away as users may select the pos transaction based on range, instead of day by day_18Mar2010
						//strGetPOS = "SELECT sitenumi,paytype,SUM(tenderamt) AS tenderamt,reason,OrderSNo  " +
						//    " FROM ( " +
						//    " SELECT trandate,sitenumi,paytype,reason,OrderSNo,tenderamt FROM pos2 " +
						//    " UNION ALL " +
						//    " SELECT trandate,sitenumi,detail2 as paytype,reason,OrderSNo,-aftdeposit AS tenderamt FROM saj1" +
						//    " UNION ALL " +
						//    " SELECT trandate,sitenumi,detail2 as paytype,reason,OrderSNo,oriamt AS tenderamt FROM dard " +
						//    " WHERE invnum = 'POS' AND ISNULL(chkReverse,'')<>'Y'" +
						//    " ) posSaj " +
						//    " WHERE reason like '%COACH%' AND sitenumi = '" + recp["outletid"].ToString().Trim() + "' " +
						//    " AND convert(nvarchar(12),trandate,112) = '" + Convert.ToDateTime(recp["trandate"]).ToString("yyyyMMdd") + "' " +
						//    " AND ISNULL(paytype,'') in (SELECT ISNULL(paymentmode,'') FROM armpos WHERE arnum='" + recp["arnum"].ToString().Trim() + "')" +
						//    " GROUP BY sitenumi,paytype,reason,OrderSNo HAVING SUM(tenderamt)<>0";
						#endregion

						#region Steph - for AR as coach
						strGetPOS = "SELECT posdate,sitenumi,paytype,SUM(tenderamt) AS tenderamt,reason,OrderSNo  " +
							" FROM ( " +
							" SELECT CONVERT(datetime,CONVERT(nvarchar(8),pos2.trandate,112)) AS posdate,pos2.sitenumi,pos2.paytype,pos2.reason, "+
							" pos2.OrderSNo,(pos2.tenderamt - pos2.change) as tenderamt FROM pos2 " +
							" LEFT OUTER JOIN posh ON posh.refnum = pos2.refnum " +
							" LEFT OUTER JOIN memoh ON memoh.refnum = pos2.refnum " +
							//steph - amended 09June2010_to take the status from pos2 as there is possibility of one live transactions contain voided items.
							" WHERE (ISNULL(posh.status,'') <>  'V'  AND ISNULL(posh.status,'') <>  'X' AND ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
							" AND (ISNULL(memoh.status,'') <>  'V'  AND ISNULL(memoh.status,'') <>  'X' AND ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
							//" WHERE (ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
							//" WHERE (ISNULL(posh.status,'') <>  'V'  AND ISNULL(posh.status,'') <>  'X') " +
							//"  OR (ISNULL(memoh.status,'') <>  'V'  AND ISNULL(memoh.status,'') <>  'X') " +
							" UNION ALL " +
							" SELECT CONVERT(datetime,CONVERT(nvarchar(8),posdate,112)) AS posdate,sitenumi,detail2 as paytype,reason,OrderSNo,-aftdeposit AS tenderamt FROM saj1" +
							" UNION ALL " +
							" SELECT CONVERT(datetime,CONVERT(nvarchar(8),posdate,112)) AS posdate,sitenumi,detail2 as paytype,reason,OrderSNo,oriamt AS tenderamt FROM dard " +
							" WHERE invnum = 'POS' AND ISNULL(chkReverse,'')<>'Y'" +
							" ) posSaj " +
							" WHERE reason like '%COACH%' " +
							//" AND sitenumi = '" + recp["outletid"].ToString().Trim() + "' " +
							" AND orderSNo = '" + recp["arnum"].ToString().Trim() + "' " +
							" AND convert(nvarchar(12),posdate,112) >= '" + Convert.ToDateTime(recp["posdatefrom"]).ToString("yyyyMMdd") + "' " +
							" AND convert(nvarchar(12),posdate,112) <= '" + Convert.ToDateTime(recp["posdateto"]).ToString("yyyyMMdd") + "' " +
							" AND ISNULL(paytype,'') in (SELECT ISNULL(paymentmode,'') FROM armpos WHERE arnum='" + recp["arnum"].ToString().Trim() + "' " +
							" AND module = '"+flag+"')" +
							" GROUP BY posdate,sitenumi,paytype,reason,OrderSNo HAVING SUM(tenderamt)<>0";
						#endregion
					}
					else
					{
						#region Steph - for AR as outlet --commented away as users may select the pos transaction based on range, instead of day by day_18Mar2010
						//strGetPOS = "SELECT sitenumi,paytype,SUM(tenderamt) AS tenderamt,reason,OrderSNo  " +
						//    " FROM ( " +
						//    " SELECT trandate,sitenumi,paytype,reason,OrderSNo,tenderamt FROM pos2 " +
						//    " UNION ALL " +
						//    " SELECT trandate,sitenumi,detail2 as paytype,reason,OrderSNo,-aftdeposit AS tenderamt FROM saj1" +
						//    " UNION ALL " +
						//    " SELECT trandate,sitenumi,detail2 as paytype,reason,OrderSNo,oriamt AS tenderamt FROM dard " +
						//    " WHERE invnum = 'POS' AND ISNULL(chkReverse,'')<>'Y'" +
						//    " ) posSaj " +
						//    " WHERE reason not like '%COACH%' AND sitenumi = '" + recp["outletid"].ToString().Trim() + "' " +
						//    " AND convert(nvarchar(12),trandate,112) = '" + Convert.ToDateTime(recp["trandate"]).ToString("yyyyMMdd") + "' " +
						//    " AND ISNULL(paytype,'') in (SELECT ISNULL(paymentmode,'') FROM armpos WHERE arnum='" + recp["arnum"].ToString().Trim() + "')" +
						//    " GROUP BY sitenumi,paytype,reason,OrderSNo HAVING SUM(tenderamt)<>0";
						#endregion

						#region Steph - for AR as outlet
						strGetPOS = "SELECT posdate,sitenumi,paytype,SUM(tenderamt) AS tenderamt,reason,OrderSNo  " +
							" FROM ( " +
							" SELECT CONVERT(datetime,CONVERT(nvarchar(8),pos2.trandate,112)) AS posdate,pos2.sitenumi,pos2.paytype,pos2.reason, "+
							" pos2.OrderSNo,(pos2.tenderamt - pos2.change) as tenderamt FROM pos2 " +
							" LEFT OUTER JOIN posh ON posh.refnum = pos2.refnum " +
							" LEFT OUTER JOIN memoh ON memoh.refnum = pos2.refnum " +
							//steph - amended 09June2010_to take the status from pos2 as there is possibility of one live transactions contain voided items.
							" WHERE (ISNULL(posh.status,'') <>  'V'  AND ISNULL(posh.status,'') <>  'X' AND ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
							"  AND (ISNULL(memoh.status,'') <>  'V'  AND ISNULL(memoh.status,'') <>  'X' AND ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
							//" WHERE (ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
							//" WHERE (ISNULL(posh.status,'') <>  'V'  AND ISNULL(posh.status,'') <>  'X') " +
							//"  OR (ISNULL(memoh.status,'') <>  'V'  AND ISNULL(memoh.status,'') <>  'X') " +
							" UNION ALL " +
							" SELECT CONVERT(datetime,CONVERT(nvarchar(8),posdate,112)) AS posdate,sitenumi,detail2 as paytype,reason,OrderSNo,-aftdeposit AS tenderamt FROM saj1" +
							" UNION ALL " +
							" SELECT CONVERT(datetime,CONVERT(nvarchar(8),posdate,112)) AS posdate,sitenumi,detail2 as paytype,reason,OrderSNo,oriamt AS tenderamt FROM dard " +
							" WHERE invnum = 'POS' AND ISNULL(chkReverse,'')<>'Y'" +
							" ) posSaj " +
							" WHERE reason not like '%COACH%' AND sitenumi = '" + recp["outletid"].ToString().Trim() + "' " +
							" AND convert(nvarchar(12),posdate,112) >= '" + Convert.ToDateTime(recp["posdatefrom"]).ToString("yyyyMMdd") + "' " +
							" AND convert(nvarchar(12),posdate,112) <= '" + Convert.ToDateTime(recp["posdateto"]).ToString("yyyyMMdd") + "' " +
							" AND ISNULL(paytype,'') in (SELECT ISNULL(paymentmode,'') FROM armpos WHERE arnum='" + recp["arnum"].ToString().Trim() + "' " +
							" AND module = '"+flag+"')" +
							" GROUP BY posdate,sitenumi,paytype,reason,OrderSNo HAVING SUM(tenderamt)<>0";
						#endregion
					}
				}
				else
				{
					MessageBox.Show("This is not a POS AR. Please check settings in Customer Master!");
					recp["outletid"] = String.Empty;
					return;
				}


				dbaccess.ReadSQL("getPOS", strGetPOS);
				DataTable getPos = dbaccess.DataSet.Tables["getPOS"];
				if (getPos.Rows.Count > 0)
				{
					if (saj1.Rows.Count > 0) // show the message only if there is data in Others tab.
					{
						DialogResult result = MessageBox.Show("This will clear all existing data in Sales and Deposits page! Continue?", "Clear data?", MessageBoxButtons.YesNo);
						if (result == DialogResult.Yes)
						{
							BizFunctions.DeleteAllRows(saj1);
							BizFunctions.DeleteAllRows(dard);
							BizFunctions.DeleteAllRows(sajpos);
							AddRecord(ref this.dbaccess);
						}
					}
					else
					{
						BizFunctions.DeleteAllRows(sajpos);
						AddRecord(ref this.dbaccess);
					}

					DataTable getDeposits = null;

					#region Steph - To loop through all the Sales and add in the deposit paid earlier, while reversing out the deposits
					foreach (DataRow dr in saj1.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							//dbaccess.ReadSQL("getDeposits", "SELECT * FROM dard");

							//DataTable AllDeposits = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,oricur,exrate, "+
							//" oricredit AS oridebit,oridebit AS oricredit, " + // steph -  to swap the 2 fields value
							//" -oriamt AS oriamt,-postamt AS postamt,invnum,OrderSNo FROM [DARD] " +
							//" WHERE ISNULL(OrderSNo,'')<>'' AND ISNULL(OrderSNo,'') IN (SELECT ISNULL(OrderSNo,'') FROM [SAJ1])");

							//string strGetDeposits = "SELECT accnum,oricur,exrate, " +
							//" oricredit AS oridebit,oridebit AS oricredit, " + // steph -  to swap the 2 fields value
							//" -oriamt AS oriamt,-postamt AS postamt,invnum,OrderSNo,invdate FROM DARD " +
							//" WHERE ISNULL(OrderSNo,'')<>'' AND ISNULL(OrderSNo,'') = '" + dr["OrderSNo"].ToString().Trim() + "'";

							string strGetDeposits = "SELECT accnum,oricur,exrate, " +
									" oricredit AS oridebit,oridebit AS oricredit, " + // steph -  to swap the 2 fields value
									" -oriamt AS oriamt,-postamt AS postamt,invnum,OrderSNo,invdate FROM DARD " +
									" WHERE ISNULL(OrderSNo,'')<>'' AND ISNULL(OrderSNo,'') = '" + dr["OrderSNo"].ToString().Trim() + "'";

							dbaccess.ReadSQL("getDeposits", strGetDeposits);

							getDeposits = dbaccess.DataSet.Tables["getDeposits"];

							DataRow drGetDeposits = null;

							if (dbaccess.DataSet.Tables["getDeposits"].Rows.Count > 0)
							{
								drGetDeposits = dbaccess.DataSet.Tables["getDeposits"].Rows[0];

								//steph - add the deposit to saj1 for calculation
								dr["deposit"] = drGetDeposits["oriamt"];

								DataRow addDard = dard.Rows.Add(new object[] { });
                                addDard["accnum"] = drGetDeposits["accnum"];
								addDard["invnum"] = drGetDeposits["invnum"];
								addDard["invdate"] = drGetDeposits["invdate"];
								addDard["oricur"] = drGetDeposits["oricur"];

								string strExr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + addDard["oricur"].ToString().Trim() + "'";
								this.dbaccess.ReadSQL("exrate", strExr);
								if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
								{
									decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(addDard["invdate"])) + ""]);
									addDard["exrate"] = exrate;
								}

								addDard["oricredit"] = drGetDeposits["oricredit"];
								addDard["oridebit"] = drGetDeposits["oridebit"];
								addDard["oriamt"] = drGetDeposits["oriamt"];
								addDard["postamt"] = BizFunctions.Round(Convert.ToDecimal(drGetDeposits["oriamt"]) * Convert.ToDecimal(addDard["exrate"]));
								addDard["gstamt"] = 0;
								addDard["chkReverse"] = "Y";  // to check if this is a reverse entry. if yes
								addDard["OrderSNo"] = drGetDeposits["OrderSNo"];
								addDard["exramt"] = (Convert.ToDecimal(addDard["oriamt"]) * Convert.ToDecimal(recp["exrate"])) -
															(Convert.ToDecimal(addDard["oriamt"]) * Convert.ToDecimal(addDard["exrate"]));
							}
						}
					}
					#endregion
				}
				else
				{
					dbaccess.ReadSQL("checkOutlet", "SELECT sitenumi FROM sitmi WHERE isnull(sitenumi,'') = '" + recp["outletid"].ToString().Trim() + "'");

					if (recp["outletid"].ToString().Trim() == String.Empty || recp["outletid"] == System.DBNull.Value)
					{
						MessageBox.Show("No POS outlet selected in Header!");
					}
					else
					{
						if (dbaccess.DataSet.Tables["checkOutlet"].Rows.Count > 0)
						{
							if (listOfConflictVouchers == oriString)
							{
								MessageBox.Show("There is no POS transaction for " + recp["outletid"].ToString().Trim() +
									" from " + Convert.ToDateTime(recp["posdatefrom"]).ToString("dd-MMM-yyyy") +
									" to " + Convert.ToDateTime(recp["posdateto"]).ToString("dd-MMM-yyyy"), "No Transaction", MessageBoxButtons.OK);
							}
						}
						else
						{
							MessageBox.Show("Invalid POS outlet selected in Header!");
						}
					}
				}
			}

			AutoCalc();
		}

		private void AddRecord(ref DBAccess dbaccess)
		{
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			DataTable getPos = dbaccess.DataSet.Tables["getPos"];
			DataTable saj1 = dbaccess.DataSet.Tables["saj1"];
			DataTable sajpos = dbaccess.DataSet.Tables["sajpos"];
			DataTable dard = dbaccess.DataSet.Tables["dard"];

			foreach (DataRow dr in getPos.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					//Steph - If this is a deposit, the record will be inserted into DARD
					if (dr["reason"].ToString().Trim().Contains("Deposit"))
					{
						DataRow newDard = dard.Rows.Add(new object[] { });
						BizFunctions.UpdateDataRow(recp, newDard, "oricur/exrate");
						newDard["trandate"] = recp["trandate"];
						newDard["posdate"] = dr["posdate"];
						newDard["invnum"] = "POS";
						newDard["invdate"] = recp["trandate"];
						newDard["detail"] = "POS - " + dr["paytype"].ToString().Trim();
						newDard["detail2"] = dr["paytype"].ToString().Trim();  //steph - added this to check the paymode extracted before
						newDard["sitenumi"] = dr["sitenumi"].ToString().Trim();  //steph - added this to check the paymode extracted before
						newDard["oriamt"] = Convert.ToDecimal(dr["tenderamt"]) * -1;
						newDard["gstamt"] = 0;  //Steph - GST is not calculated for deposit, and the gst will only be calculated when the transaction is posted to sales.
						newDard["OrderSNo"] = dr["OrderSNo"];
						newDard["reason"] = dr["reason"];

						newDard["oricredit"] = dr["tenderamt"];
						newDard["oridebit"] = 0;
						newDard["postamt"] = Convert.ToDecimal(newDard["oriamt"]) * Convert.ToDecimal(newDard["exrate"]);

					}
					else
					{
						DataRow newSaj1 = saj1.Rows.Add(new object[] { });
						newSaj1["trandate"] = recp["trandate"];
						newSaj1["posdate"] = dr["posdate"];
						newSaj1["invnum"] = "POS";
						newSaj1["detail"] = "POS - " + dr["paytype"].ToString().Trim();
						newSaj1["detail2"] = dr["paytype"].ToString().Trim();  //steph - added this to check the paymode extracted before
						newSaj1["sitenumi"] = dr["sitenumi"].ToString().Trim();  //steph - added this to check the paymode extracted before
						newSaj1["aftdeposit"] = Convert.ToDecimal(dr["tenderamt"]);
						//newSaj1["doriamt"] = (Convert.ToDecimal(dr["tenderamt"]) * 100) / (100 + Convert.ToDecimal(recp["gstper"]));
						newSaj1["OrderSNo"] = dr["OrderSNo"];
						newSaj1["reason"] = dr["reason"];

						#region Steph - Adding of Reference No. created at POS
						//string strGetPosRefnum = "SELECT POSResult.refnum,SUM(POSResult.tenderamt - POSResult.change) AS tenderamt,POSResult.sitenumi,POSResult.reason " +
						//    " FROM " +
						//    " (SELECT * FROM POS2 " +
						//    " WHERE CONVERT(datetime,Convert(nvarchar(8),trandate,112)) = '" + Convert.ToDateTime(dr["posdate"]).ToString("yyyyMMdd").Trim() + "' " +
						//    " AND sitenumi = '" + dr["sitenumi"].ToString().Trim() + "' AND paytype = '" + dr["paytype"].ToString().Trim() + "' " +
						//    " ) POSResult " +
						//    " LEFT OUTER JOIN posh ON posh.refnum = POSResult.refnum " +
						//    " GROUP BY POSResult.refnum,POSResult.sitenumi,POSResult.reason";


						string strGetPosRefnum = "SELECT POSResult.refnum,SUM(POSResult.tenderamt - POSResult.change) AS tenderamt, "+
							" POSResult.sitenumi,POSResult.reason " +
							" FROM " +
							" (SELECT pos2.refnum,pos2.tenderamt,pos2.change,pos2.sitenumi,pos2.reason,"+
							" CASE WHEN pos2.reason like 'COACH%' THEN MEMOCOACH.cchnum ELSE arm.arnum END AS arnum FROM POS2 " +
							" LEFT OUTER JOIN memocoach ON memocoach.refnum = pos2.refnum "+
							" LEFT OUTER JOIN (SELECT * FROM arm WHERE ISNULL(iscoach,0)=0) arm ON ISNULL(arm.outletid,'') = ISNULL(pos2.sitenumi,'') "+
							" WHERE CONVERT(datetime,Convert(nvarchar(8),POS2.trandate,112)) = '" + Convert.ToDateTime(dr["posdate"]).ToString("yyyyMMdd").Trim() + "' " +
							" AND POS2.sitenumi = '" + dr["sitenumi"].ToString().Trim() + "' "+
							" AND POS2.paytype = '" + dr["paytype"].ToString().Trim() + "' " +
                            " AND POS2.reason='" + dr["reason"].ToString().Trim() + "'" +
							" ) POSResult " +
							" LEFT OUTER JOIN posh ON posh.refnum = POSResult.refnum " +
							" WHERE POSResult.arnum ='" + recp["arnum"].ToString().Trim() + "' "+ 
							" GROUP BY POSResult.refnum,POSResult.sitenumi,POSResult.reason";

						dbaccess.ReadSQL("getPosRefnum", strGetPosRefnum);

						DataTable getPosRefnum = dbaccess.DataSet.Tables["getPosRefnum"];

						foreach (DataRow dr2 in getPosRefnum.Rows)
						{
							if (dr2.RowState != DataRowState.Deleted)
							{
								DataRow newSajPos = sajpos.Rows.Add(new object[] { });
								BizFunctions.UpdateDataRow(recp, newSajPos, "refnum/arnum");
								newSajPos["debtoracc"] = recp["accnum1"];
								newSajPos["salestype"] = "Credit";
								newSajPos["posdate"] = dr["posdate"];
								newSajPos["paymode"] = dr["paytype"];

								newSajPos["posrefnum"] = dr2["refnum"];
								newSajPos["sitenumi"] = dr2["sitenumi"];
								newSajPos["reason"] = dr2["reason"];
								newSajPos["tenderamt"] = dr2["tenderamt"];
							}
						}
						#endregion
					}
				}
			}
		}

		void getIncomeExpense_Click(object sender, EventArgs e)
		{
			DataTable sivc = dbaccess.DataSet.Tables["sivc"];
			string strGetIE = "SELECT accnum,SUM(oriamt) AS oriamt,oricur,exrate FROM [siv1] GROUP BY accnum,oricur,exrate HAVING SUM(oriamt)<>0 "+
				" UNION ALL SELECT accnum2,bankchg,oricur,exrate FROM recp WHERE ISNULL(bankchg,0) <>0 "+
				" UNION ALL SELECT accnum3,discamt,oricur,exrate FROM recp WHERE ISNULL(discamt,0) <>0 " +
				" UNION ALL SELECT accnum4,flexamt,oricur,exrate FROM recp WHERE ISNULL(flexamt,0) <>0";
			DataTable getIE = BizFunctions.ExecuteQuery(dbaccess.DataSet, strGetIE);

			DialogResult result = MessageBox.Show("The entries in this page will be reset! Continue?", "Clear Data?", MessageBoxButtons.YesNo);

			if (result == DialogResult.Yes)
			{
				BizFunctions.DeleteAllRows(sivc);

				foreach (DataRow dr in getIE.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						DataRow addSivc = sivc.Rows.Add(new object[] { });
						addSivc["accnum"] = dr["accnum"];
						addSivc["oriamt"] = dr["oriamt"];
						addSivc["oricur"] = dr["oricur"];
						addSivc["exrate"] = dr["exrate"];
					}
				}
			}
			AutoCalc();
		}


	    #region Reopen Handle
		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
			DataRow RECP = this.dbaccess.DataSet.Tables["recp"].Rows[0];
			DataTable siv1  =this.dbaccess.DataSet.Tables["siv1"];

			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM CSH" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + RECP["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + RECP["refnum"].ToString().Trim() + "'");
			BizFunctions.DeleteRow(siv1, "discallow = 'Y'");
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
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			DataView dtvView = new DataView(siv1);
			dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			//dtvView.Sort = "MATNAME";
			//cpm1.Rows.Add(new object[] { });
			dtvView.AllowNew = false;
			dgOthers.DataSource = dtvView;
		}

		protected override void Document_Delete_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Delete_Handle(sender, e);
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			DataView dtvView = new DataView(siv1);
			dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			//dtvView.Sort = "MATNAME";
			//cpm1.Rows.Add(new object[] { });
			dtvView.AllowNew = false;
			dgOthers.DataSource = dtvView;
		}

		protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveEnd_OnClick(sender, e);
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			DataView dtvView = new DataView(siv1);
			dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			//dtvView.Sort = "MATNAME";
			//cpm1.Rows.Add(new object[] { });
			dtvView.AllowNew = false;
			dgOthers.DataSource = dtvView;
		}

		protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Insert_OnClick (sender, e);
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			if (e.CurrentRow.Table.TableName.ToUpper() == "CSH")
			{
				e.CurrentRow["lgr"] = "CSH";
			}
			//int intRow = 0;

			//foreach (DataRow dr in siv1.Rows)
			//{
			//    if (dr.RowState != DataRowState.Deleted)
			//    {
			//        intRow = intRow + 1;
			//    }
			//}

			//dgOthers.CurrentCell = new DataGridCell(intRow, 3);		


			//if (e.CurrentRow.Table.TableName.ToUpper() == "PIV1")
			//{
			//}
		}

		#endregion

		#region Refresh
        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow recp = e.DBAccess.DataSet.Tables["recp"].Rows[0];
            DataTable ard = e.DBAccess.DataSet.Tables["ard"];
            DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
            DataTable csh = e.DBAccess.DataSet.Tables["csh"];
            DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
			DataTable excsh = e.DBAccess.DataSet.Tables["excsh"];
			setDefaults(dbaccess.DataSet, "recp/ard/siv1/excsh");



			DataView dtvView = new DataView(siv1);
			dtvView.RowFilter = "DISCALLOW is null or discallow <>'Y'";
			//dtvView.Sort = "MATNAME";
			//cpm1.Rows.Add(new object[] { });
			dtvView.AllowNew = false;
			dgOthers.DataSource = dtvView;

			AutoCalc();

			AutoSetDefaultValueNVCHeader();
			AutoSetDefaultValueNVCDetails();
		}
		#endregion
		#endregion

		#region SaveBegin

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick (sender, e);
            DataRow recp = e.DBAccess.DataSet.Tables["recp"].Rows[0];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
            DataTable csh = e.DBAccess.DataSet.Tables["csh"];
            DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
			DataTable excsh = e.DBAccess.DataSet.Tables["excsh"];
            DataTable dard=e.DBAccess.DataSet.Tables["dard"];
			dbaccess.ReadSQL("acm", "SELECT * FROM acm");

			DataTable acm = dbaccess.DataSet.Tables["acm"];

			#region Steph - Posting Time!!! - commented away - moved to Confirm_Handle
			//if (recp["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
			//{

			//    #region steph - piv / siv posting for accnum3 - discamt
			//    if (dbaccess.DataSet.Tables["recp"].Select().Length > 0)
			//    {
			//        DataTable sumRecp = BizFunctions.ExecuteQuery(e.DBAccess.DataSet, "Select  recp.refnum as refnum,  recp.arnum as arnum, recp.flag as flag, recp.trandate as trandate,  recp.period as period," +
			//            " recp.gstper as gstper,  recp.exrate as exrate," +
			//            " recp.oricur3 as oricur,  recp.discamt as oriamt, recp.accnum3 as accnum, acm.acctype as acctype from [recp]" +
			//            "  recp  LEFT OUTER JOIN [acm] acm ON recp.accnum3 = acm.accnum where recp.refnum = '" + recp["refnum"].ToString().Trim() + "' and recp.discamt <> 0");
					
			//        if (sumRecp.Rows.Count > 0)
			//        {
			//            DataRow sumSivRow = sumRecp.	Rows[0];

			//            string discColumn = "refnum|discallow|accnum|exrate|flag|postamt|oriamt|oricur|period|oricredit|oridebit|gstamt|gstper|locno|deptno";
			//            string[] bankchgStrColumn = discColumn.Split('|');

			//            if (bankchgStrColumn.Length > 0)
			//            {

			//                #region Steph -  To post to PIV
			//                if (Convert.ToString(sumSivRow["acctype"]).Trim() == "2" || Convert.ToString(sumSivRow["acctype"]).Trim() == "3")
			//                {
			//                    DataRow tmppiv = piv1.NewRow();

			//                    for (int i = 0; i < bankchgStrColumn.Length; i++)
			//                    {
			//                        switch (bankchgStrColumn[i])
			//                        {
			//                            case "refnum":
			//                                tmppiv["refnum"] = sumSivRow["refnum"];
			//                                break;
			//                            case "arnum":
			//                                tmppiv["arnum"] = sumSivRow["arnum"];
			//                                break;
			//                            case "locno":
			//                                tmppiv["locno"] = recp["locno"];
			//                                break;
			//                            case "deptno":
			//                                tmppiv["deptno"] = recp["deptno"];
			//                                break;
			//                            case "period":
			//                                tmppiv["period"] = sumSivRow["period"];
			//                                break;
			//                            case "accnum":
			//                                tmppiv["accnum"] = sumSivRow["accnum"];
			//                                break;
			//                            case "trandate":
			//                                tmppiv["trandate"] = sumSivRow["trandate"];
			//                                break;
			//                            case "lgr":
			//                                tmppiv["lgr"] = "PIV";
			//                                break;
			//                            case "discallow":
			//                                tmppiv["discallow"] = "Y";
			//                                break;
			//                            case "flag":
			//                                tmppiv["flag"] = sumSivRow["flag"];
			//                                break;
			//                            case "exrate":
			//                                if (sumSivRow["exrate"] == System.DBNull.Value)
			//                                {
			//                                    sumSivRow["exrate"] = 1;
			//                                }
			//                                tmppiv["exrate"] = Convert.ToDecimal(sumSivRow["exrate"]);
			//                                break;
			//                            case "oricur":
			//                                tmppiv["oricur"] = sumSivRow["oricur"];
			//                                break;
			//                            case "oriamt":
			//                                #region To replace the null value
			//                                if (sumSivRow["oriamt"] == System.DBNull.Value)
			//                                {
			//                                    sumSivRow["oriamt"] = 0;
			//                                }
			//                                #endregion
			//                                tmppiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])));
			//                                break;
			//                            case "postamt":
			//                                #region To replace the null value
			//                                if (sumSivRow["exrate"] == System.DBNull.Value)
			//                                {
			//                                    sumSivRow["exrate"] = 1;
			//                                }
			//                                if (sumSivRow["oriamt"] == System.DBNull.Value)
			//                                {
			//                                    sumSivRow["oriamt"] = 0;
			//                                }
			//                                #endregion
			//                                tmppiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])) * (Convert.ToDecimal(sumSivRow["exrate"])));
			//                                break;
			//                            case "gstper":
			//                                tmppiv["gstper"] = Convert.ToDecimal(sumSivRow["gstper"]);
			//                                break;
			//                            case "gstamt":
			//                                tmppiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]) - Convert.ToDecimal(tmppiv["oriamt"]));
			//                                break;
			//                            case "oridebit":
			//                                if ((decimal)sumSivRow["oriamt"] > 0)
			//                                {
			//                                    tmppiv["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmppiv["oridebit"] = 0;
			//                                }
			//                                break;
			//                            case "oricredit":
			//                                if ((decimal)sumSivRow["oriamt"] < 0)
			//                                {
			//                                    tmppiv["oricredit"] = 0;
			//                                }
			//                                else
			//                                {
			//                                    tmppiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
			//                                }
			//                                break;
			//                            default:
			//                                tmppiv[bankchgStrColumn[i]] = sumSivRow[bankchgStrColumn[i]];
			//                                break;
			//                        }
			//                    }

			//                    piv1.Rows.Add(tmppiv.ItemArray);
			//                }
			//                #endregion

			//                #region Steph -To post to SIV
			//                if (Convert.ToString(sumSivRow["acctype"]).Trim() == "0" || Convert.ToString(sumSivRow["acctype"]).Trim() == "1")
			//                {
			//                    DataRow tmpsiv = siv1.NewRow();

			//                    for (int i = 0; i < bankchgStrColumn.Length; i++)
			//                    {
			//                        switch (bankchgStrColumn[i])
			//                        {
			//                            case "refnum":
			//                                tmpsiv["refnum"] = sumSivRow["refnum"];
			//                                break;
			//                            case "arnum":
			//                                tmpsiv["arnum"] = sumSivRow["arnum"];
			//                                break;
			//                            case "locno":
			//                                tmpsiv["locno"] = recp["locno"];
			//                                break;
			//                            case "deptno":
			//                                tmpsiv["deptno"] = recp["deptno"];
			//                                break;
			//                            case "period":
			//                                tmpsiv["period"] = sumSivRow["period"];
			//                                break;
			//                            case "accnum":
			//                                tmpsiv["accnum"] = sumSivRow["accnum"];
			//                                break;
			//                            case "trandate":
			//                                tmpsiv["trandate"] = sumSivRow["trandate"];
			//                                break;
			//                            case "lgr":
			//                                tmpsiv["lgr"] = "SIV";
			//                                break;
			//                            case "discallow":
			//                                tmpsiv["discallow"] = "Y";
			//                                break;
			//                            case "flag":
			//                                tmpsiv["flag"] = sumSivRow["flag"];
			//                                break;
			//                            case "exrate":
			//                                tmpsiv["exrate"] = Convert.ToDecimal(sumSivRow["exrate"]);
			//                                break;
			//                            case "oricur":
			//                                tmpsiv["oricur"] = sumSivRow["oricur"];
			//                                break;
			//                            case "oriamt":
			//                                tmpsiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])));
			//                                break;
			//                            case "postamt":
			//                                tmpsiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumSivRow["gstper"])) * (Convert.ToDecimal(sumSivRow["exrate"])));
			//                                break;
			//                            case "gstper":
			//                                tmpsiv["gstper"] = Convert.ToDecimal(sumSivRow["gstper"]);
			//                                break;
			//                            case "gstamt":
			//                                tmpsiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]) - Convert.ToDecimal(tmpsiv["oriamt"]));
			//                                break;
			//                            case "oridebit":
			//                                if ((decimal)sumSivRow["oriamt"] > 0)
			//                                {
			//                                    tmpsiv["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmpsiv["oridebit"] = 0;
			//                                }
			//                                break;
			//                            case "oricredit":
			//                                if ((decimal)sumSivRow["oriamt"] < 0)
			//                                {
			//                                    tmpsiv["oricredit"] = 0;
			//                                }
			//                                else
			//                                {
			//                                    tmpsiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
			//                                }
			//                                break;
			//                            default:
			//                                tmpsiv[bankchgStrColumn[i]] = sumSivRow[bankchgStrColumn[i]];
			//                                break;
			//                        }
			//                    }
			//                    siv1.Rows.Add(tmpsiv.ItemArray);
			//                }
			//                #endregion

			//                #region Steph -Contra Account - To post to Cash table
			//                if (Convert.ToString(sumSivRow["acctype"]).Trim() != "0" && Convert.ToString(sumSivRow["acctype"]).Trim() != "1" && Convert.ToString(sumSivRow["acctype"]).Trim() != "2" && Convert.ToString(sumSivRow["acctype"]).Trim() != "3")
			//                {
			//                    DataRow tmpcsh = csh.NewRow();

			//                    for (int i = 0; i < bankchgStrColumn.Length; i++)
			//                    {
			//                        switch (bankchgStrColumn[i])
			//                        {
			//                            case "refnum":
			//                                tmpcsh["refnum"] = sumSivRow["refnum"];
			//                                break;
			//                            case "arnum":
			//                                tmpcsh["arnum"] = sumSivRow["arnum"];
			//                                break;
			//                            case "locno":
			//                                tmpcsh["locno"] = recp["locno"];
			//                                break;
			//                            case "deptno":
			//                                tmpcsh["deptno"] = recp["deptno"];
			//                                break;
			//                            case "period":
			//                                tmpcsh["period"] = sumSivRow["period"];
			//                                break;
			//                            case "accnum":
			//                                tmpcsh["accnum"] = sumSivRow["accnum"];
			//                                break;
			//                            case "trandate":
			//                                tmpcsh["trandate"] = sumSivRow["trandate"];
			//                                break;
			//                            case "lgr":
			//                                tmpcsh["lgr"] = "SIV";
			//                                break;
			//                            case "discallow":
			//                                tmpcsh["discallow"] = "Y";
			//                                break;
			//                            case "flag":
			//                                tmpcsh["flag"] = sumSivRow["flag"];
			//                                break;
			//                            case "exrate":
			//                                if (sumSivRow["exrate"] == System.DBNull.Value)
			//                                {
			//                                    sumSivRow["exrate"] = 1;
			//                                }
			//                                tmpcsh["exrate"] = Convert.ToDecimal(sumSivRow["exrate"]);
			//                                break;
			//                            case "oricur":
			//                                tmpcsh["oricur"] = sumSivRow["oricur"];
			//                                break;
			//                            case "oriamt":
			//                                tmpcsh["oriamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]));
			//                                break;
			//                            case "postamt":
			//                                tmpcsh["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumSivRow["oriamt"])) * (Convert.ToDecimal(sumSivRow["exrate"])));
			//                                break;
			//                            case "gstper":
			//                                tmpcsh["gstper"] = Convert.ToDecimal(sumSivRow["gstper"]);
			//                                break;
			//                            case "gstamt":
			//                                tmpcsh["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumSivRow["oriamt"]) - Convert.ToDecimal(tmpcsh["oriamt"]));
			//                                break;
			//                            case "oridebit":
			//                                if ((decimal)sumSivRow["oriamt"] > 0)
			//                                {
			//                                    tmpcsh["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmpcsh["oridebit"] = 0;
			//                                }
			//                                break;
			//                            case "oricredit":
			//                                if ((decimal)sumSivRow["oriamt"] < 0)
			//                                {
			//                                    tmpcsh["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumSivRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmpcsh["oricredit"] = 0;
			//                                }
			//                                break;
			//                            default:
			//                                tmpcsh[bankchgStrColumn[i]] = sumSivRow[bankchgStrColumn[i]];
			//                                break;
			//                        }
			//                    }
			//                    csh.Rows.Add(tmpcsh.ItemArray);
			//                }
			//                #endregion
			//            }
			//        }
			//    }

			//    #endregion

			//    #region steph - piv / siv posting for accnum4 - flexamt
			//    if (dbaccess.DataSet.Tables["recp"].Select().Length > 0)
			//    {
			//        DataTable sumFlex = BizFunctions.ExecuteQuery(e.DBAccess.DataSet, "Select  recp.refnum as refnum,  recp.arnum as arnum, recp.flag as flag, recp.trandate as trandate,  recp.period as period," +
			//            " recp.gstper as gstper,  recp.exrate as exrate," +
			//            " recp.oricur4 as oricur,  recp.flexamt as oriamt, recp.accnum4 as accnum, acm.acctype as acctype from  [recp]" +
			//            "  recp  LEFT OUTER JOIN [acm] acm ON recp.accnum4 = acm.accnum where recp.refnum = '" + recp["refnum"].ToString().Trim() + "' and recp.flexamt <> 0");
					
			//        if (sumFlex.Rows.Count> 0)
			//        {
			//            DataRow sumFlexRow = sumFlex.Rows[0];

			//            string discColumn = "refnum|discallow|accnum|exrate|flag|postamt|oriamt|oricur|period|oricredit|oridebit|gstamt|gstper|locno|deptno";
			//            string[] bankchgStrColumn = discColumn.Split('|');

			//            if (bankchgStrColumn.Length > 0)
			//            {
			//                #region Steph - To post the flexamt to PIV
			//                if (Convert.ToString(sumFlexRow["acctype"]).Trim() == "2" || Convert.ToString(sumFlexRow["acctype"]).Trim() == "3")
			//                {
			//                    DataRow tmppiv = piv1.NewRow();

			//                    for (int i = 0; i < bankchgStrColumn.Length; i++)
			//                    {
			//                        switch (bankchgStrColumn[i])
			//                        {
			//                            case "refnum":
			//                                tmppiv["refnum"] = sumFlexRow["refnum"];
			//                                break;
			//                            case "arnum":
			//                                tmppiv["arnum"] = sumFlexRow["arnum"];
			//                                break;
			//                            case "locno":
			//                                tmppiv["locno"] = recp["locno"];
			//                                break;
			//                            case "deptno":
			//                                tmppiv["deptno"] = recp["deptno"];
			//                                break;
			//                            case "period":
			//                                tmppiv["period"] = sumFlexRow["period"];
			//                                break;
			//                            case "accnum":
			//                                tmppiv["accnum"] = sumFlexRow["accnum"];
			//                                break;
			//                            case "trandate":
			//                                tmppiv["trandate"] = sumFlexRow["trandate"];
			//                                break;
			//                            case "lgr":
			//                                tmppiv["lgr"] = "PIV";
			//                                break;
			//                            case "discallow":
			//                                tmppiv["discallow"] = "Y";
			//                                break;
			//                            case "flag":
			//                                tmppiv["flag"] = sumFlexRow["flag"];
			//                                break;
			//                            case "exrate":
			//                                if (sumFlexRow["exrate"] == System.DBNull.Value)
			//                                {
			//                                    sumFlexRow["exrate"] = 1;
			//                                }
			//                                tmppiv["exrate"] = Convert.ToDecimal(sumFlexRow["exrate"]);
			//                                break;
			//                            case "oricur":
			//                                tmppiv["oricur"] = sumFlexRow["oricur"];
			//                                break;
			//                            case "oriamt":
			//                                tmppiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])));
			//                                break;
			//                            case "postamt":
			//                                tmppiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])) * (Convert.ToDecimal(sumFlexRow["exrate"])));
			//                                break;
			//                            case "gstper":
			//                                tmppiv["gstper"] = Convert.ToDecimal(sumFlexRow["gstper"]);
			//                                break;
			//                            case "gstamt":
			//                                tmppiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]) - Convert.ToDecimal(tmppiv["oriamt"]));
			//                                break;
			//                            case "oridebit":
			//                                if ((decimal)sumFlexRow["oriamt"] < 0)
			//                                {
			//                                    tmppiv["oricredit"] = 0;
			//                                }
			//                                else
			//                                {
			//                                    tmppiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
			//                                }
			//                                break;
			//                            case "oricredit":
			//                                if ((decimal)sumFlexRow["oriamt"] < 0)
			//                                {
			//                                    tmppiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmppiv["oricredit"] = 0;
			//                                }
			//                                break;
			//                            default:
			//                                tmppiv[bankchgStrColumn[i]] = sumFlexRow[bankchgStrColumn[i]];
			//                                break;
			//                        }
			//                    }

			//                    piv1.Rows.Add(tmppiv.ItemArray);
			//                }

			//                #endregion

			//                #region Steph  - To post the flexamt to SIV
			//                if (Convert.ToString(sumFlexRow["acctype"]).Trim() == "0" || Convert.ToString(sumFlexRow["acctype"]).Trim() == "1")
			//                {
			//                    DataRow tmpsiv = siv1.NewRow();

			//                    for (int i = 0; i < bankchgStrColumn.Length; i++)
			//                    {
			//                        switch (bankchgStrColumn[i])
			//                        {
			//                            case "refnum":
			//                                tmpsiv["refnum"] = sumFlexRow["refnum"];
			//                                break;
			//                            case "arnum":
			//                                tmpsiv["arnum"] = sumFlexRow["arnum"];
			//                                break;
			//                            case "locno":
			//                                tmpsiv["locno"] = recp["locno"];
			//                                break;
			//                            case "deptno":
			//                                tmpsiv["deptno"] = recp["deptno"];
			//                                break;
			//                            case "period":
			//                                tmpsiv["period"] = sumFlexRow["period"];
			//                                break;
			//                            case "accnum":
			//                                tmpsiv["accnum"] = sumFlexRow["accnum"];
			//                                break;
			//                            case "trandate":
			//                                tmpsiv["trandate"] = sumFlexRow["trandate"];
			//                                break;
			//                            case "lgr":
			//                                tmpsiv["lgr"] = "SIV";
			//                                break;
			//                            case "discallow":
			//                                tmpsiv["discallow"] = "Y";
			//                                break;
			//                            case "flag":
			//                                tmpsiv["flag"] = sumFlexRow["flag"];
			//                                break;
			//                            case "exrate":
			//                                if (sumFlexRow["exrate"] == System.DBNull.Value)
			//                                {
			//                                    sumFlexRow["exrate"] = 1;
			//                                }
			//                                tmpsiv["exrate"] = Convert.ToDecimal(sumFlexRow["exrate"]);
			//                                break;
			//                            case "oricur":
			//                                tmpsiv["oricur"] = sumFlexRow["oricur"];
			//                                break;
			//                            case "oriamt":
			//                                tmpsiv["oriamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])));
			//                                break;
			//                            case "postamt":
			//                                tmpsiv["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * 100 / (100 + Convert.ToDecimal(sumFlexRow["gstper"])) * (Convert.ToDecimal(sumFlexRow["exrate"])));
			//                                break;
			//                            case "gstper":
			//                                tmpsiv["gstper"] = Convert.ToDecimal(sumFlexRow["gstper"]);
			//                                break;
			//                            case "gstamt":
			//                                tmpsiv["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]) - Convert.ToDecimal(tmpsiv["oriamt"]));
			//                                break;
			//                            case "oridebit":
			//                                if ((decimal)sumFlexRow["oriamt"] < 0)
			//                                {
			//                                    tmpsiv["oricredit"] = 0;
			//                                }
			//                                else
			//                                {
			//                                    tmpsiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
			//                                }
			//                                break;
			//                            case "oricredit":
			//                                if ((decimal)sumFlexRow["oriamt"] < 0)
			//                                {
			//                                    tmpsiv["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmpsiv["oricredit"] = 0;
			//                                }
			//                                break;
			//                            default:
			//                                tmpsiv[bankchgStrColumn[i]] = sumFlexRow[bankchgStrColumn[i]];
			//                                break;
			//                        }
			//                    }

			//                    siv1.Rows.Add(tmpsiv.ItemArray);
			//                }
			//                #endregion

			//                #region Steph - contra account - To post to Cash Table
			//                if (Convert.ToString(sumFlexRow["acctype"]).Trim() != "0" && Convert.ToString(sumFlexRow["acctype"]).Trim() != "1" && Convert.ToString(sumFlexRow["acctype"]).Trim() != "2" && Convert.ToString(sumFlexRow["acctype"]).Trim() != "3")
			//                {
			//                    DataRow tmpcsh = csh.NewRow();

			//                    for (int i = 0; i < bankchgStrColumn.Length; i++)
			//                    {
			//                        switch (bankchgStrColumn[i])
			//                        {
			//                            case "refnum":
			//                                tmpcsh["refnum"] = sumFlexRow["refnum"];
			//                                break;
			//                            case "arnum":
			//                                tmpcsh["arnum"] = sumFlexRow["arnum"];
			//                                break;
			//                            case "locno":
			//                                tmpcsh["locno"] = recp["locno"];
			//                                break;
			//                            case "deptno":
			//                                tmpcsh["deptno"] = recp["deptno"];
			//                                break;
			//                            case "period":
			//                                tmpcsh["period"] = sumFlexRow["period"];
			//                                break;
			//                            case "accnum":
			//                                tmpcsh["accnum"] = sumFlexRow["accnum"];
			//                                break;
			//                            case "trandate":
			//                                tmpcsh["trandate"] = sumFlexRow["trandate"];
			//                                break;
			//                            case "lgr":
			//                                tmpcsh["lgr"] = "SIV";
			//                                break;
			//                            case "discallow":
			//                                tmpcsh["discallow"] = "Y";
			//                                break;
			//                            case "flag":
			//                                tmpcsh["flag"] = sumFlexRow["flag"];
			//                                break;
			//                            case "exrate":
			//                                if (sumFlexRow["exrate"] == System.DBNull.Value)
			//                                {
			//                                    sumFlexRow["exrate"] = 1;
			//                                }
			//                                tmpcsh["exrate"] = Convert.ToDecimal(sumFlexRow["exrate"]);
			//                                break;
			//                            case "oricur":
			//                                tmpcsh["oricur"] = sumFlexRow["oricur"];
			//                                break;
			//                            case "oriamt":
			//                                tmpcsh["oriamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]));
			//                                break;
			//                            case "postamt":
			//                                tmpcsh["postamt"] = BizFunctions.Round((Convert.ToDecimal(sumFlexRow["oriamt"])) * (Convert.ToDecimal(sumFlexRow["exrate"])));
			//                                break;
			//                            case "gstper":
			//                                tmpcsh["gstper"] = Convert.ToDecimal(sumFlexRow["gstper"]);
			//                                break;
			//                            case "gstamt":
			//                                tmpcsh["gstamt"] = BizFunctions.Round(Convert.ToDecimal(sumFlexRow["oriamt"]) - Convert.ToDecimal(tmpcsh["oriamt"]));
			//                                break;
			//                            case "oridebit":
			//                                if ((decimal)sumFlexRow["oriamt"] > 0)
			//                                {
			//                                    tmpcsh["oridebit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmpcsh["oridebit"] = 0;
			//                                }
			//                                break;
			//                            case "oricredit":
			//                                if ((decimal)sumFlexRow["oriamt"] < 0)
			//                                {
			//                                    tmpcsh["oricredit"] = System.Math.Abs(Convert.ToDecimal(sumFlexRow["oriamt"]));
			//                                }
			//                                else
			//                                {
			//                                    tmpcsh["oricredit"] = 0.00;
			//                                }
			//                                break;
			//                            default:
			//                                tmpcsh[bankchgStrColumn[i]] = sumFlexRow[bankchgStrColumn[i]];
			//                                break;
			//                        }
			//                    }
			//                    csh.Rows.Add(tmpcsh.ItemArray);
			//                }
			//                #endregion
			//            }
			//        }
			//    }

			//                    #endregion

			//    #region Steph - To copy from ExCsh to Csh for the changing of currency receipts.
			//    string refnum = this.dbaccess.DataSet.Tables["recp"].Rows[0]["refnum"].ToString().Trim();
			//    DataTable ExCsh = dbaccess.DataSet.Tables["ExCsh"];
			//    if (!refnum.Equals(""))
			//    {
			//        for (int j = 0; j < ExCsh.Rows.Count; j++)
			//        {
			//            DataRow dr_Csh = this.dbaccess.DataSet.Tables["csh"].NewRow();
			//            for (int i = 0; i < ExCsh.Columns.Count; i++)
			//            {
			//                if (dr_Csh.RowState != DataRowState.Deleted)
			//                {
			//                    if (ExCsh.Columns[i].ColumnName != "ID")
			//                    {
			//                        if (this.dbaccess.DataSet.Tables["csh"].Columns.IndexOf(ExCsh.Columns[i].ColumnName) != -1)
			//                        {
			//                            dr_Csh[ExCsh.Columns[i].ColumnName] = ExCsh.Rows[j][i];
			//                            dr_Csh["exramt"] = 0;
			//                            dr_Csh["locno"] = recp["locno"];
			//                            dr_Csh["deptno"] = recp["deptno"];
			//                        }
			//                    }
			//                }
			//            }
			//            this.dbaccess.DataSet.Tables["csh"].Rows.Add(dr_Csh);
			//        }
			//    }
			//    #endregion

			//    #region Steph - The MDT posting statement
			//    MDTReader.updateAccountCsh(ref this.dbaccess, "refnum|trandate|" +
			//        "accnum|arnum|exrate|detail|flag|lgr|postamt|oriamt|bankamt|oricur|period|chknum|oricredit|remark|oridebit|locno|deptno",
			//                        "REC", "ARD", "RECP", "csh", "siv1", "SALES RECEIPT - EXTRACTION");
			//    #endregion

			//    #region steph - Need to post the header's remark into GLD.
			//    DataTable gld = dbaccess.DataSet.Tables["gld"];
			//    foreach (DataRow dr in gld.Rows)
			//    {
			//        if (dr.RowState != DataRowState.Deleted)
			//        {
			//            dr["detail"] = recp["remark"];
			//            if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
			//                dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.

			//        }
			//    }
			//    #endregion
			//}
			#endregion

			#region Steph - This is to update the detail tables
			foreach (DataRow dr in ard.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					BizFunctions.UpdateDataRow(recp, dr, "user/flag/status/created/modified");
			}

			foreach (DataRow dr in siv1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					BizFunctions.UpdateDataRow(recp, dr, "user/flag/status/created/modified");
			}
			foreach (DataRow dr in excsh.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					BizFunctions.UpdateDataRow(recp, dr, "user/flag/status/created/modified");
			}
            foreach (DataRow dr in dard.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                    BizFunctions.UpdateDataRow(recp, dr, "user/flag/status/created/modified");
            }
			#endregion
		}

		#endregion

		#endregion

		protected override void Document_Paste_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Paste_OnClick (sender, e);
			e.DBAccess.DataSet.Tables["recp"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_RECP_ColumnChanged);
		}	

		#region ColumnChangedEvents

		#region recp

		private void Voucher_RECP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];

			switch(e.Column.ColumnName)
			{
				case "arnum":
					#region Steph - Pull info from ARM
					dbaccess.ReadSQL("getArmInfo", "SELECT arnum,arname,ptc,addr1,addr2,addr3,addr4,phone,hp,fax,ptnum,gstgrpnum,oricur,accnum FROM arm where arnum ='" + e.Row["arnum"].ToString().Trim() + "'");

					if (dbaccess.DataSet.Tables["getArmInfo"].Rows.Count > 0)
					{
						DataRow getArmInfo = dbaccess.DataSet.Tables["getArmInfo"].Rows[0];
						e.Row["detail"] = getArmInfo["arname"];
						e.Row["addr1"] = getArmInfo["addr1"];
						e.Row["addr2"] = getArmInfo["addr2"];
						e.Row["addr3"] = getArmInfo["addr3"];
						e.Row["addr4"] = getArmInfo["addr4"];
						e.Row["phone"] = getArmInfo["phone"];
						e.Row["hp"] = getArmInfo["hp"];
						e.Row["fax"] = getArmInfo["fax"];

						if (e.Row["payterms"].ToString().Trim() == "" || e.Row["payterms"] == System.DBNull.Value)
							e.Row["payterms"] = getArmInfo["ptnum"];
						if (e.Row["gstgrpnum"].ToString().Trim() == "" || e.Row["gstgrpnum"] == System.DBNull.Value)
							e.Row["gstgrpnum"] = getArmInfo["gstgrpnum"];
						if (e.Row["oricur"].ToString().Trim() == "" || e.Row["oricur"] == System.DBNull.Value)
							e.Row["oricur"] = getArmInfo["oricur"];
					}
					else
					{
						e.Row["detail"] = "";
						e.Row["addr1"] = "";
						e.Row["addr2"] = "";
						e.Row["addr3"] = "";
						e.Row["addr4"] = "";
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
					//    decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
					//    e.Row["exrate"] = exrate;
					//}

					e.Row["exrate"] = getExrate(e.Row["oricur"].ToString().Trim(), e.Row["trandate"]);

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
							e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)recp["trandate"]);
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
					string strexr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					this.dbaccess.ReadSQL("exrate", strexr);
					if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					{
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
						e.Row["exrate"] = exrate;
					}

					recp["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(recp["trandate"]));

					e.Row.EndEdit();
					break;
					#endregion				
			}

		}

		#endregion

		#region ard

		private void Voucher_ARD_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{
				case "oricur":
			#region set exrate
					if (e.Row["invdate"] != System.DBNull.Value)
					{
						string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
						this.dbaccess.ReadSQL("exrate", exrStr);
						if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
						{
                            if(Convert.ToDecimal(e.Row["exrate"])==0)
                            {
							decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["invdate"])) + ""]);
							e.Row["exrate"] = exrate;
                            }
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
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			DataTable RECP = dbaccess.DataSet.Tables["recp"];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable dard = dbaccess.DataSet.Tables["dard"];
			DataTable excsh = dbaccess.DataSet.Tables["excsh"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable saj1 = dbaccess.DataSet.Tables["saj1"];
			DataTable gsttable = dbaccess.DataSet.Tables["gsttable"];

            setDefaults(dbaccess.DataSet, "RECP/saj1/ard/siv1");

			#region recp
			recp["oricur2"] = recp["oricur"];
			recp["dbankamt"] = (decimal)recp["amtrcv"];
			#endregion

			if(recp["refnum"].ToString().Trim().Contains("SRC"))
			{
				if (recp["docunum"] == System.DBNull.Value || recp["docunum"].ToString().Trim() == String.Empty)
				{
					recp["docunum"] = recp["refnum"];
				}
			}
			
            #region San* - Check contact textbox
			dbaccess.ReadSQL("getContactInfo", "Select ptc,outletid from arm where arnum = '" + recp["arnum"] + "' ");
            if (dbaccess.DataSet.Tables["getContactInfo"].Rows.Count > 0)
            {
                if (recp["contact"].ToString() == String.Empty || recp["contact"] == System.DBNull.Value)
                {
                    recp["contact"] = dbaccess.DataSet.Tables["getContactInfo"].Rows[0]["ptc"];
                }
				recp["outletid"] = dbaccess.DataSet.Tables["getContactinfo"].Rows[0]["outletid"];
            }
            #endregion - San_End

			#region Steph -  To get pd from pd (nonYear) table.
			
			recp["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(recp["trandate"]));

			#endregion

			#region initialise values
			ard_oriamt = 0;
			ard_postamt = 0;
			ard_doriamt = 0;
			ard_dpostamt = 0;
			siv1_doriamt = 0;
			siv1_dorigstamt = 0;
			siv1_postamt = 0;
			siv1_roriamt = 0;
			siv1_rpostamt = 0;
			siv1_gstamt = 0;
			#endregion

			#region ard



			#region Steph - Advance payment
			//steph - 19 April 2010 - Add in Advance Payment function.
			bool checkExists = false;

			dbaccess.ReadSQL("checkDebtorsAccnum", "SELECT accnum FROM arm WHERE arnum = '" + recp["arnum"].ToString().Trim() + "'");

			if ((bool)recp["advance"] == true)
			{
				foreach (DataRow dr in ard.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						if (dr["locno"].ToString().Trim() == "ZZZ")
						{
							checkExists = true;

							dr["invnum"] = recp["refnum"];
							dr["sivamt"] = 0;
							if (dr["adjamt"] == System.DBNull.Value || Convert.ToDecimal(dr["adjamt"]) == 0)
							{
								dr["adjamt"] = recp["dbankamt"];
							}
							dr["invdate"] = recp["trandate"];
							dr["lgr"] = "ARD";
							dr["dorigstamt"] = 0;
							dr["dgstamt"] = 0;
							dr["exramt"] = 0;

							if (dr["oricur"] == System.DBNull.Value || dr["oricur"].ToString().Trim() == String.Empty)
							{
								dr["oricur"] = recp["oricur"];
							}
							if (dr["exrate"] == System.DBNull.Value || Convert.ToDecimal(dr["exrate"]) == 0)
							{
								dr["exrate"] = getExrate(dr["oricur"].ToString().Trim(), dr["invdate"]);
							}

							if (dbaccess.DataSet.Tables["checkDebtorsAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkDebtorsAccnum"].Rows[0]["accnum"];
							}
						}
					}
				}
				if ((bool)checkExists == false)
				{
					DataRow addArd = ard.Rows.Add(new object[] { });

					BizFunctions.UpdateDataRow(recp, addArd);

					addArd["invnum"] = recp["refnum"];
					addArd["sivamt"] = 0;
					if (addArd["adjamt"] == System.DBNull.Value || Convert.ToDecimal(addArd["adjamt"]) == 0)
					{
						addArd["adjamt"] = recp["dbankamt"];
					}
					addArd["invdate"] = recp["trandate"];
					addArd["lgr"] = "ARD";
					addArd["dorigstamt"] = 0;
					addArd["dgstamt"] = 0;
					addArd["exramt"] = 0;
					addArd["locno"] = "ZZZ";  //steph - used as a key to check if the entry already exists.

					if (addArd["oricur"] == System.DBNull.Value || addArd["oricur"].ToString().Trim() == String.Empty)
					{
						addArd["oricur"] = recp["oricur"];
					}
					if (addArd["exrate"] == System.DBNull.Value || Convert.ToDecimal(addArd["exrate"]) == 0)
					{
						addArd["exrate"] = getExrate(addArd["oricur"].ToString().Trim(), addArd["invdate"]);
					}

					if (dbaccess.DataSet.Tables["checkDebtorsAccnum"].Rows.Count > 0)
					{
						addArd["accnum"] = dbaccess.DataSet.Tables["checkDebtorsAccnum"].Rows[0]["accnum"];
					}
				}
			}
			#endregion

			foreach (DataRow dr in ard.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(recp, dr);
					BizFunctions.UpdateDataRow(recp, dr, "arnum/docunum/status/chknum");

					if (dr["detail"] == System.DBNull.Value|| dr["detail"].ToString().Trim() == String.Empty )
					{
						dr["detail"] = recp["remark"];
					}
					if (dr["oricur"] == System.DBNull.Value || dr["oricur"].ToString().Trim() == String.Empty)
					{
						dr["oricur"] = recp["oricur"];
                    }
                    #region TT,exrate base on historical rate not the current rate.21 Sep 2010
                    if (dr["exrate"] == System.DBNull.Value || dr["exrate"].ToString().Trim() == String.Empty)
                    {
                        dr["exrate"] = getExrate(dr["oricur"].ToString().Trim(), dr["invdate"]);
                    }
                    //ori
                    //dr["exrate"] = getExrate(dr["oricur"].ToString().Trim(), dr["invdate"]);
                    #endregion

                    if (dr["adjamt"] == System.DBNull.Value || (decimal)dr["adjamt"] == 0)
						dr["adjamt"] = dr["sivamt"];
					dr["doriamt"] = dr["adjamt"];
					dr["dpostamt"] = BizFunctions.Round((decimal)dr["exrate"] * (decimal)dr["doriamt"]);

					if ((decimal)dr["doriamt"] > 0)
					{
						dr["oricredit"] = System.Math.Abs((decimal)dr["doriamt"]);
						dr["oridebit"] = 0;					
					}
					if ((decimal)dr["doriamt"] < 0)
					{
						dr["oricredit"] = 0;
						dr["oridebit"] = System.Math.Abs((decimal)dr["doriamt"]);
					}

					ard_oriamt += (decimal)dr["doriamt"] * -1;		//amount is negative
					ard_postamt += (decimal)dr["dpostamt"] * -1; //amount is negative
					ard_doriamt += (decimal)dr["doriamt"];
					ard_dpostamt += (decimal)dr["dpostamt"];
				}
			}

			#endregion

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum,accnum2 FROM arm WHERE arnum = '" + recp["arnum"].ToString().Trim() + "'");

			#region saj1

			#region Steph - Inclusive GST calculations
			#region initialise values
			saj1_discamt = 0;
			saj1_oriamt = 0;
			saj1_origstamt = 0;
			saj1_postamt = 0;
			saj1_gstamt = 0;
			saj1_grosamt = 0;
			saj1_roriamt = 0;
			saj1_rpostamt = 0;
			#endregion

			dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
				" WHERE gstgrpnum ='" + recp["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

			foreach (DataRow dr in saj1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
				{
					BizFunctions.UpdateDataRow(recp, dr);
					BizFunctions.UpdateDataRow(recp, dr, "refnum/arnum/docunum/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");


					//if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
					//{
					//    if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
					//    {
					//        dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
					//    }
					//}

					if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
					{
						dr["accnum"] = getSaleAcc(dr);
					}

					dr["roriamt"] = Convert.ToDecimal(dr["aftdeposit"]) + Convert.ToDecimal(dr["deposit"]);

					dr["rpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(dr["exrate"]));

					if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
					{
						//steph  - do not have to calculate the gst, allow user to key in manually.
					}
					else
					{
						dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(recp["gstper"]) / (100 + Convert.ToDecimal(recp["gstper"])));
					}
					dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
					dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
					dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
					dr["dgrosamt"] = dr["doriamt"];

					saj1_discamt += (decimal)dr["discamt"];
					saj1_oriamt += (decimal)dr["doriamt"];
					saj1_origstamt += (decimal)dr["dorigstamt"];
					saj1_postamt += (decimal)dr["dpostamt"];
					saj1_gstamt += (decimal)dr["dgstamt"];
					saj1_grosamt += (decimal)dr["dgrosamt"];
					saj1_roriamt += (decimal)dr["roriamt"];
					saj1_rpostamt += (decimal)dr["rpostamt"];

					#region Steph - Pull Info from MATM
					dbaccess.ReadSQL("getMatm", "SELECT matname,saleAcc FROM matm where matnum ='" + dr["matnum"].ToString().Trim() + "'");
					if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
					{
						if (dr["detail"].ToString().Trim() == "" || dr["detail"] == System.DBNull.Value)
							dr["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
						if (dr["accnum"].ToString().Trim() == "" || dr["accnum"] == System.DBNull.Value)
							dr["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
					}
					#endregion
				}
			}
			#region Steph - Check the gst amt differences and add/deduct from the first entry of saj1
			if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
			{
				//steph  - do not have to calculate the gst, allow user to key in manually.
			}
			else
			{
				if (saj1.Rows.Count > 0)
				{
					decimal headerGst = BizFunctions.Round((saj1_oriamt + saj1_origstamt) * Convert.ToDecimal(recp["gstper"]) / (100 + Convert.ToDecimal(recp["gstper"])));
					if (headerGst != saj1_origstamt)
					{
						if (saj1.Rows[0].RowState != DataRowState.Deleted)
						{
							saj1.Rows[0]["dorigstamt"] = Convert.ToDecimal(saj1.Rows[0]["dorigstamt"]) + headerGst - saj1_origstamt;
						}
					}
				}

				#region initialise values
				saj1_discamt = 0;
				saj1_oriamt = 0;
				saj1_origstamt = 0;
				saj1_postamt = 0;
				saj1_gstamt = 0;
				saj1_grosamt = 0;
				saj1_roriamt = 0;
				saj1_rpostamt = 0;
				#endregion

				foreach (DataRow dr in saj1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgrosamt"] = dr["doriamt"];
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

						saj1_discamt += (decimal)dr["discamt"];
						saj1_oriamt += (decimal)dr["doriamt"];
						saj1_origstamt += (decimal)dr["dorigstamt"];
						saj1_postamt += (decimal)dr["dpostamt"];
						saj1_gstamt += (decimal)dr["dgstamt"];
						saj1_grosamt += (decimal)dr["dgrosamt"];
						saj1_roriamt += (decimal)dr["roriamt"];
						saj1_rpostamt += (decimal)dr["rpostamt"];
					}
				}
			}

			#endregion
			#endregion

			#endregion

			#region dard

			//Initializing values
			dard_oriamt = 0;

            #region get default account code for deposit
            dbaccess.ReadSQL("dardacc", "SELECT accnum FROM acc WHERE refnum= 'DARD'");
            DataTable dardacc = dbaccess.DataSet.Tables["dardacc"];
            #endregion
			foreach (DataRow dr in dard.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
					{
                        if (dardacc.Rows.Count > 0)
                        {
                            dr["accnum"] = dardacc.Rows[0]["accnum"];
                        }
                        //ORI
                        //if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
                        //{
                        //    dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum"];
                        //}
					}
                    if (BizFunctions.IsEmpty(dr["oriamt"]))
                    {
                        dr["oriamt"] = 0;
                    }
					dard_oriamt = dard_oriamt + (Convert.ToDecimal(dr["oriamt"])*-1);
                    BizFunctions.UpdateDataRow(recp, dr, "user/created/modified/status");
				}
			}
			#endregion

			#region ExCsh
			foreach (DataRow dr in excsh.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(recp, dr);
					BizFunctions.UpdateDataRow(recp, dr, "arnum/docunum/trandate/chknum");

					dr["oricur"] = dr["oricur"]; // reset oricur to trigger column changed event				
					dr["lgr"] = "csh";
					dr["oriamt"] = (decimal)dr["bankamt"];
					if ((decimal)dr["oriamt"] > 0)
					{
						dr["oricredit"] = 0;
						dr["oridebit"] = System.Math.Abs((decimal)dr["oriamt"]);
					}
					if ((decimal)dr["oriamt"] < 0)
					{
						dr["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
						dr["oridebit"] = 0;
					}
					dr["postamt"] = BizFunctions.Round((decimal)dr["exrate"] * (decimal)dr["oriamt"]);
				}
			}
			#endregion

			#region siv1

			//dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum2 FROM arm WHERE arnum = '" + recp["arnum"].ToString().Trim() + "'");

			if ((bool)recp["inclgst"])
			{
				#region Steph - Inclusive GST calculations

				dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
						" WHERE gstgrpnum ='" + recp["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in siv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(recp, dr, "createdby/arnum/docunum/oricur/exrate/gstgrpnum/gstper");

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
							dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(recp["gstper"]) / (100 + Convert.ToDecimal(recp["gstper"])));
						}
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
						dr["dgrosamt"] = dr["doriamt"];

						siv1_doriamt += (decimal)dr["doriamt"];
						siv1_dorigstamt += (decimal)dr["dorigstamt"];
						siv1_postamt += (decimal)dr["dpostamt"];
						siv1_gstamt += (decimal)dr["dgstamt"];
					}
				}
				#region Steph - Check the gst amt differences and add/deduct from the first entry of siv1
				if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
				{
					//steph  - do not have to calculate the gst, allow user to key in manually.
				}
				else
				{
					if (siv1.Rows.Count > 0)
					{
						decimal headerGst = BizFunctions.Round((siv1_doriamt + siv1_dorigstamt) * Convert.ToDecimal(recp["gstper"]) / (100 + Convert.ToDecimal(recp["gstper"])));
						if (headerGst != siv1_dorigstamt)
						{
							siv1.Rows[0]["dorigstamt"] = Convert.ToDecimal(siv1.Rows[0]["dorigstamt"]) + headerGst - siv1_dorigstamt;
						}
					}

					#region initialise values
					siv1_doriamt = 0;
					siv1_dorigstamt = 0;
					siv1_postamt = 0;
					siv1_gstamt = 0;
					#endregion

					foreach (DataRow dr in siv1.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
							dr["dgrosamt"] = dr["doriamt"];
							dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
							dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

							siv1_doriamt += (decimal)dr["doriamt"];
							siv1_dorigstamt += (decimal)dr["dorigstamt"];
							siv1_postamt += (decimal)dr["dpostamt"];
							siv1_gstamt += (decimal)dr["dgstamt"];
							siv1_roriamt += (decimal)dr["roriamt"];
							siv1_rpostamt += (decimal)dr["rpostamt"];

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
						" WHERE gstgrpnum ='" + recp["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in siv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						BizFunctions.UpdateDataRow(recp, dr);
						BizFunctions.UpdateDataRow(recp, dr, "createdby/arnum/docunum/oricur/exrate/gstgrpnum/gstper");

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

						siv1_doriamt += (decimal)dr["doriamt"];
						siv1_dorigstamt += (decimal)dr["dorigstamt"];
						siv1_postamt += (decimal)dr["dpostamt"];
						siv1_gstamt += (decimal)dr["dgstamt"];
						siv1_roriamt += (decimal)dr["roriamt"];
						siv1_rpostamt += (decimal)dr["rpostamt"];

					}
				}
				#endregion
			}

			#endregion

			#region Gst table
			BizFunctions.DeleteAllRows(gsttable);
			
			dbaccess.ReadSQL("gstInput","SELECT accnum FROM acc WHERE refnum = 'GST2'");

			string gstInput = String.Empty;

			if (dbaccess.DataSet.Tables["gstInput"].Rows.Count > 0)
			{
				gstInput = dbaccess.DataSet.Tables["gstInput"].Rows[0]["accnum"].ToString().Trim();
			}

			foreach (DataRow dr in saj1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addGst = gsttable.Rows.Add(new object[] { });
					BizFunctions.UpdateDataRow(dr,addGst);  // steph - copy all table into this table.
					BizFunctions.UpdateDataRow(dr,addGst,"oriamt/postamt/gstamt");  // steph - copy all table into this table.
					addGst["oriamt"] = addGst["gstamt"]; // steph - this table is to force the postgld to post the gstamt from saj1 table.
					addGst["postamt"] = addGst["gstamt"];
					addGst["accnum"] = gstInput;
					addGst["oricur"] = "SGD";
					addGst["exrate"] = 1;					
				}
			}
			#endregion

			#region set Header Page oriamt/origstamt/oritotalamt/postamt/gstamt/totalamt

			recp["artotal"] = ard_doriamt;
			recp["sivtotal"] = siv1_doriamt + siv1_dorigstamt;
			recp["saj1Total"] = saj1_oriamt + saj1_origstamt;
			recp["dardTotal"] = dard_oriamt;			
			recp["origstamt"] = siv1_dorigstamt;
			recp["oritotalamt"] = (decimal)recp["oriamt"] + (decimal)recp["origstamt"];
			recp["oriamt"] = (decimal)siv1_roriamt + (decimal)ard_doriamt + (decimal)recp["saj1Total"] + (decimal)recp["dardTotal"];
			recp["postamt"] = (decimal)siv1_rpostamt + (decimal)ard_dpostamt;


			recp["gstamt"] = BizFunctions.Round((decimal)recp["postamt"] * ((decimal)recp["gstper"] / 100));
			recp["totalamt"] = (decimal)recp["postamt"] + (decimal)recp["gstamt"];

			#endregion

			#region Calculation of differences
			decimal steDiff = 0;
            //decimal ArTotal = 0;
			decimal CshRcv = 0;
            decimal Grandtotal = 0;
            //ArTotal = ard_postamt * -1;
            Grandtotal = Convert.ToDecimal(recp["oriamt"]);

			if (recp["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSP) // if confim only post into ard ,csh and gld
			{
				CshRcv = BizFunctions.Round((Convert.ToDecimal(recp["amtrcv"]) + Convert.ToDecimal(recp["flexamt"])
					+ Convert.ToDecimal(recp["flexamt"])) * Convert.ToDecimal(recp["exrate"]));

                //steDiff = CshRcv - ArTotal;
                steDiff = CshRcv - Grandtotal;
				txtRecStatus.Text = "AR Rounding = "+ steDiff.ToString().Trim();
                if (steDiff != 0)
                {
                    check = false;
                }
                else
                {
                    check = true;
                }
			}
			#endregion

			#region Steph - To set the correct value for oriamt, postamt using mdt
			MDTReader.SetCorrectValue(ref this.dbaccess, "SIV1", "SRC");
			MDTReader.SetCorrectValue(ref this.dbaccess, "ARD", "SRC");
			MDTReader.SetCorrectValue(ref this.dbaccess, "RECP", "SRC");
			MDTReader.SetCorrectValue(ref this.dbaccess, "SAJ1", "SAJ");
			#endregion
		}
		#endregion
		
		#region Steph - Set Default For Header RECP
		private void AutoSetDefaultValueNVCHeader()
		{
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			#region Steph - Setting of Default Value
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, recp["flag"].ToString().Trim(), "recp", "oricur1");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, recp["flag"].ToString().Trim(), "recp", "accnum1");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, recp["flag"].ToString().Trim(), "recp", "accnum2");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, recp["flag"].ToString().Trim(), "recp", "accnum3");
			MDTReader.SetDefaultValueNVC(ref this.dbaccess, recp["flag"].ToString().Trim(), "recp", "accnum4");
			#endregion

		}
		#endregion

		#region Steph - Set Default For Details ARD,SIV1,EXCSH
		private void AutoSetDefaultValueNVCDetails()
		{
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			//MDTReader.SetDefaultValueNVC(ref this.dbaccess, recp["flag"].ToString().Trim(), "ard", "accnum");
			//MDTReader.SetDefaultValueNVC(ref this.dbaccess, recp["flag"].ToString().Trim(), "siv1", "accnum");
		}
		#endregion

		protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick(sender, e);
			DataRow recp = e.DBAccess.DataSet.Tables["recp"].Rows[0];
			Hashtable selectedCollection = new Hashtable();

			switch (e.ReportName)
			{
				case "Receipt (Debtors)":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					if (e.DBAccess.DataSet.Tables.Contains("arm"))
						e.DBAccess.DataSet.Tables.Remove("arm");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + recp["arnum"].ToString().Trim() + "'");
					selectedCollection.Add("ard", "SELECT * FROM ard" + Common.DEFAULT_SYSTEM_YEAR + " where refnum = '" + recp["refnum"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;
				case "Receipt (Others)":
					selectedCollection.Add("coy", "SELECT * FROM coy");
                    selectedCollection.Add("acm", "SELECT * FROM acm");
                    selectedCollection.Add("ccm", "SELECT * FROM ccm");
					if (e.DBAccess.DataSet.Tables.Contains("arm"))
						e.DBAccess.DataSet.Tables.Remove("arm");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + recp["arnum"].ToString().Trim() + "'");
					selectedCollection.Add("siv1", "SELECT * FROM siv1 where refnum = '" + recp["refnum"].ToString().Trim() + "'");
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

		private string getSaleAcc(DataRow drSaj1)
		{
			DataRow recp = dbaccess.DataSet.Tables["recp"].Rows[0];

			dbaccess.ReadSQL("checkMainSalesAccnum", "SELECT accnum2 FROM arm " +
				" WHERE arnum = '" + recp["arnum"].ToString().Trim() + "'");

			switch (drSaj1["reason"].ToString().Trim())
			{

				case "Coaching":
					dbaccess.ReadSQL("coachSales", "SELECT DISTINCT saleacc FROM armpos2 " +
							" WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 1 ");
					if (dbaccess.DataSet.Tables["coachSales"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["coachSales"].Rows[0]["saleacc"].ToString().Trim());
					}
					else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					}
					else
					{
						return String.Empty;
					}
					break;

				case "POS":
					dbaccess.ReadSQL("itemSales", "SELECT DISTINCT saleacc FROM armpos2 " +
							" WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 2 ");
					if (dbaccess.DataSet.Tables["itemSales"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["itemSales"].Rows[0]["saleacc"].ToString().Trim());
					}
					else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					}
					else
					{
						return String.Empty;
					}
					break;

				case "SpecialCollection":
					dbaccess.ReadSQL("SpecialSales", "SELECT DISTINCT saleacc FROM armpos2 " +
							" WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 2 ");
					if (dbaccess.DataSet.Tables["SpecialSales"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["SpecialSales"].Rows[0]["saleacc"].ToString().Trim());
					}
					else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					}
					else
					{
						return String.Empty;
					}
					break;

				case "SpecialDeposit":
					dbaccess.ReadSQL("SpecialSales", "SELECT DISTINCT saleacc FROM armpos2 " +
							" WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 2 ");
					if (dbaccess.DataSet.Tables["SpecialSales"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["SpecialSales"].Rows[0]["saleacc"].ToString().Trim());
					}
					else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					}
					else
					{
						return String.Empty;
					}
					break;

				case "Rental":
					dbaccess.ReadSQL("rentalSales", "SELECT DISTINCT saleacc FROM armpos2 " +
							" WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 3 ");
					if (dbaccess.DataSet.Tables["rentalSales"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["rentalSales"].Rows[0]["saleacc"].ToString().Trim());
					}
					else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					}
					else
					{
						return String.Empty;
					}
					break;

				case "RepairCollection":
					dbaccess.ReadSQL("repairSales", "SELECT DISTINCT saleacc FROM armpos2 " +
							" WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 4 ");
					if (dbaccess.DataSet.Tables["repairSales"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["repairSales"].Rows[0]["saleacc"].ToString().Trim());
					}
					else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					}
					else
					{
						return String.Empty;
					}
					break;

				case "RepairDeposit":
					dbaccess.ReadSQL("repairSales", "SELECT DISTINCT saleacc FROM armpos2 " +
							" WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 4 ");
					if (dbaccess.DataSet.Tables["repairSales"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["repairSales"].Rows[0]["saleacc"].ToString().Trim());
					}
					else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
					{
						return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					}
					else
					{
						return String.Empty;
					}
					break;

				case "VoucherSale": 
				    dbaccess.ReadSQL("voucherSales", "SELECT DISTINCT saleacc FROM armpos2 " +
				            " WHERE arnum ='" + recp["arnum"].ToString().Trim() + "' AND postype = 6 ");
				    if (dbaccess.DataSet.Tables["voucherSales"].Rows.Count > 0)
				    {
				        return (dbaccess.DataSet.Tables["voucherSales"].Rows[0]["saleacc"].ToString().Trim());
				    }
				    else if (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows.Count > 0)
				    {
				        return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
				    }
				    else
				    {
				        return String.Empty;
				    }
				    break;

				default:
					return (dbaccess.DataSet.Tables["checkMainSalesAccnum"].Rows[0]["accnum2"].ToString().Trim());
					break;
			}
		}
	}
}