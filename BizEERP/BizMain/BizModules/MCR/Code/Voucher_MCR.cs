/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_MCR.cs
 *	Description:    Monthly Recon
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		20070103			Start 
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using DEMO.MDT;

namespace ATL.MCR
{
	public class Voucher_MCR : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables
		protected DBAccess dbaccess = null;
		protected bool mcrhColumnChange = true;
		protected bool mcr1ColumnChange = true;

		protected Button getPOS;

		protected bool opened = false;

		protected decimal mcr1_oriamt = 0;
		protected decimal mcr1_origstamt = 0;
		protected decimal mcr1_postamt = 0;
		protected decimal mcr1_gstamt = 0;
		protected decimal mcr1_grosamt = 0;
		protected decimal mcr1_roriamt = 0;
		protected decimal mcr1_rpostamt = 0;

		protected Button btnDelete;

		protected string strAccNotOverwritten = String.Empty;

		#endregion

		#region Constructor

		public Voucher_MCR(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_MCR.xml", moduleName, voucherBaseHelpers)
		{
		}

		#endregion
		
		#region Steph - To stop users from open more than 1 voucher from the same module  as this is causing the saving error.
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
			e.Condition = "mcrh.flag='MCR' AND mcrh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (mcrh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" mcrh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" mcrh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND mcrh.flag='MCR' AND mcrh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}

		#endregion

		#region Tab Control

		protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
		{
			base.Document_TabControl_OnSelectionChanged(sender, e);
			btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
			Button btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
			Button btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;
			Button btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
			Button btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
			Button btnExtract = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Extract") as Button;

			switch ((sender as LXFClassLibrary.Controls.TabControl).SelectedIndex)
			{
				case 0:
					btnDelete.Enabled = true;
					break;
			}

		}
	
		#endregion

		#region Document Form On Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);
			DataRow mcrh = e.DBAccess.DataSet.Tables["mcrh"].Rows[0];

			opened = true;

			string headerForm = (e.FormsCollection["header"] as Form).Name;

			dbaccess = e.DBAccess;

			getPOS = BizXmlReader.CurrentInstance.GetControl(headerForm, "btn_getPOS") as Button;
			getPOS.Click += new EventHandler(getPOS_Click);

			if (mcrh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
			{
				mcrh["trandate"] = System.DateTime.Now;
				mcrh["posdatefrom"] = mcrh["trandate"];
				mcrh["posdateto"] = mcrh["trandate"];
			}

			mcrh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			e.DBAccess.DataSet.Tables["mcrh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_MCRH_ColumnChanged);
		}

		#endregion

		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow mcrh = dbaccess.DataSet.Tables["mcrh"].Rows[0];
			DataTable mcr1 = e.DBAccess.DataSet.Tables["mcr1"];
			DataTable mcr10 = e.DBAccess.DataSet.Tables["mcr10"];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];


			#region Steph - Error Checking!

			string DataEntryErrors = "Please check the following:";

			dbaccess.ReadSQL("checkAcm", "SELECT accnum,accname FROM acm");

			DataTable getEmptyAcMcrh = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum from [mcrh] WHERE isnull(accnum,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcMcrh.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. (Debit A/C) in Header";
			}

			DataTable getEmptyAcMcr1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [mcr1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcMcr1.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. in Sales Adjustments Detail";
			}

			if (DataEntryErrors != "Please check the following:")
			{
				MessageBox.Show(DataEntryErrors, "Confirm Unsuccessful");
				e.Handle = false;
			}
			else
			{
				#region Voucher Confirmed - Posting Time!

				MDTReader.updateAccount(ref this.dbaccess, "refnum|trandate|" +
					"accnum|arnum|exrate|detail|flag|lgr|gstamt|invdate|dpostamt|postamt|doriamt|oriamt|oricur|period|oricredit|oridebit|invnum|gstamt|exramt|locno|deptno",
					"MCRH", "MCR", "ard", "siv10", "DOR-SIV EXTRACT");


				#region Steph - To overwrite the posting if there is CC Distribution entries
				// summation of the oriamt and postamt by accnum from the Cost Centre Distribution tab
				DataTable GroupSivcAcc = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,sum(oriamt) as oriamt, sum(postamt) as postamt " +
					" from [sivc] group by accnum");

				strAccNotOverwritten = "Please check Account No. for Cost Centre below will not be overwritten:";

				foreach (DataRow dr in GroupSivcAcc.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						DataTable GroupGldAcc = BizFunctions.ExecuteQuery(dbaccess.DataSet, "Select accnum,sum(oriamt) as oriamt,sum(postamt) as postamt " +
							" from [gld] where accnum = '" + dr["accnum"].ToString().Trim() + "' group by accnum");
						if (GroupGldAcc.Rows.Count > 0)
						{
							if (dr["accnum"].ToString().Trim() == GroupGldAcc.Rows[0]["accnum"].ToString().Trim() & Convert.ToDecimal(dr["postamt"]) == Convert.ToDecimal(GroupGldAcc.Rows[0]["postamt"]))
							{
								#region GLD posting
								BizFunctions.DeleteRow(gld, "accnum='" + dr["accnum"].ToString().Trim() + "'");
								DataTable SivcGrpWCC = BizFunctions.ExecuteQuery(dbaccess.DataSet, " SELECT accnum,ccnum,oricur,exrate,SUM(oriamt) AS oriamt, " +
									" SUM(postamt) AS postamt FROM [sivc] WHERE accnum ='" + dr["accnum"].ToString().Trim() + "' GROUP BY accnum,ccnum,oricur,exrate");
								foreach (DataRow drCC in SivcGrpWCC.Rows)
								{
									if (drCC.RowState != DataRowState.Deleted)
									{
										DataRow addGL = gld.Rows.Add(new object[] { });
										addGL["accnum"] = drCC["accnum"];
										addGL["ccnum"] = drCC["ccnum"];
										addGL["oricur"] = drCC["oricur"];
										addGL["exrate"] = drCC["exrate"];
										if (Convert.ToDecimal(drCC["oriamt"]) > 0)
										{
											addGL["oricredit"] = 0;
											addGL["oridebit"] = Math.Abs(Convert.ToDecimal(drCC["oriamt"]));
										}
										else
										{
											addGL["oricredit"] = Math.Abs(Convert.ToDecimal(drCC["oriamt"]));
											addGL["oridebit"] = 0;
										}
										addGL["oriamt"] = drCC["oriamt"];
										addGL["postamt"] = drCC["postamt"];
										addGL["lgr"] = "SIVC";
									}
								}
								#endregion
							}
							else
							{
								strAccNotOverwritten = strAccNotOverwritten + "\n Amount Does Not Tally: " + dr["accnum"].ToString().Trim();
							}
						}
						else
						{
							strAccNotOverwritten = strAccNotOverwritten + "\n Account No. does not exists: " + dr["accnum"].ToString().Trim();
						}
					}
				}
				if (strAccNotOverwritten != "Please check Account No. for Cost Centre below will not be overwritten:")
				{
					MessageBox.Show(strAccNotOverwritten);
					e.Handle = false;

					//Steph - If this is not overwritten, must delete all from saj10, the temp table.
					BizFunctions.DeleteAllRows(mcr10);

					//Steph - Take out the posting for GL and AR since overwrite of the Cost Center is not allowed!
					BizFunctions.DeleteAllRows(ard);
					BizFunctions.DeleteAllRows(gld);
				}
				#endregion


				#region steph - Need to post the header's remark into GLD.
				foreach (DataRow dr in gld.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["detail"] = mcrh["remark"];
						if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
							dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.
					}
				}
				#endregion

				#endregion
			}

			#endregion
		}

		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);

			DataRow mcrh = this.dbaccess.DataSet.Tables["mcrh"].Rows[0];

			#region Steph - Do not allow reopen if voucher has been created for this Sales Invoice - Ex: Sales Receipt
			dbaccess.ReadSQL("checkArdReopen", "SELECT refnum FROM ard" + Common.DEFAULT_SYSTEM_YEAR + 
				" WHERE invnum = '" + mcrh["refnum"].ToString().Trim() + "' "+
				" AND refnum <>'" + mcrh["refnum"].ToString().Trim() + "' AND flag<>'MCR'");
			DataTable checkArdReopen = dbaccess.DataSet.Tables["checkArdReopen"];
			string RefList = "Please check entries below which has been created for this Monthly Reconciliation Voucher:";
			foreach (DataRow dr in checkArdReopen.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					RefList = RefList + "\n " + dr["refnum"].ToString().Trim();
				}
			}

			if (RefList != "Please check entries below which has been created for this Monthly Reconciliation Voucher:")
			{
				MessageBox.Show(RefList, "Reopen Unsuccessful");
				e.Handle = false;
			}
			#endregion

			else if (mcrh["flag"].ToString().Trim() != "MCR")
			{
				e.Handle = false;
			}
			else
			{
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM ARD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + mcrh["refnum"].ToString().Trim() + "'");
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + mcrh["refnum"].ToString().Trim() + "'");
			}

		}

		void getPOS_Click(object sender, EventArgs e)
		{
			DataRow mcrh = dbaccess.DataSet.Tables["mcrh"].Rows[0];
			DataTable mcr1 = dbaccess.DataSet.Tables["mcr1"];

			#region Steph - Check header AR Code to see if it's invalid
			string strCheckArnum = "SELECT arnum FROM arm where arnum = '" + mcrh["arnum"].ToString().Trim() + "'";
			dbaccess.ReadSQL("checkArnum", strCheckArnum);
			if (dbaccess.DataSet.Tables["checkArnum"].Rows.Count == 0)
			{
				MessageBox.Show("Empty or Invalid A/R Code", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			#endregion

			dbaccess.ReadSQL("readPosInfoFromArm", "SELECT outletid, isCoach FROM arm "+
				" WHERE arnum = '"+mcrh["arnum"].ToString().Trim()+"' "+
				" AND ISNULL(outletid,'')<>''");
			
			string strGetPOS = String.Empty;

			if (dbaccess.DataSet.Tables["readPosInfoFromArm"].Rows.Count > 0)
			{
				DataRow readPosInfoFromArm = dbaccess.DataSet.Tables["readPosInfoFromArm"].Rows[0];

				mcrh["outletid"] = readPosInfoFromArm["outletid"];

				#region Steph -  To check if the date range selected by users conflict with trandate selected earlier, if yes, prompt users the refnum list
				string strGetTrandateRange = "SELECT refnum,posdatefrom, posdateto " +
					" FROM mcrh " +
					" WHERE status<>'V' AND outletid = '" + mcrh["outletid"].ToString().Trim() + "' " +
					" AND arnum ='" + mcrh["arnum"].ToString().Trim() + "' " +
					" AND flag ='" + mcrh["flag"].ToString().Trim() + "' " +
					" AND refnum <>'" + mcrh["refnum"].ToString().Trim() + "' " +
					" AND " +
					" ( " +
					" ( " +
					" Convert(nvarchar(8),posdatefrom,112) <= '" + Convert.ToDateTime(mcrh["posdatefrom"]).ToString("yyyyMMdd").Trim() + "' " +
					" AND Convert(nvarchar(8),posdateto,112) >= '" + Convert.ToDateTime(mcrh["posdatefrom"]).ToString("yyyyMMdd").Trim() + "' " +
					" ) " +
					" OR " +
					" ( " +
					" Convert(nvarchar(8),posdatefrom,112) <=	'" + Convert.ToDateTime(mcrh["posdateto"]).ToString("yyyyMMdd").Trim() + "' " +
					" AND Convert(nvarchar(8),posdateto,112) >= '" + Convert.ToDateTime(mcrh["posdateto"]).ToString("yyyyMMdd").Trim() + "' " +
					" ) " +
					" ) ";
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

				else
				{

					#region Steph - for AR as outlet
					//strGetPOS = "SELECT posdate,sitenum,paytype,SUM(tenderamt) AS tenderamt,reason,OrderSNo  " +
					//    " FROM ( " +
					//    " SELECT CONVERT(datetime,CONVERT(nvarchar(8),trandate,112)) AS posdate,sitenum,paytype,reason,OrderSNo,tenderamt FROM pos2 " +
					//    " UNION ALL " +
					//    " SELECT CONVERT(datetime,CONVERT(nvarchar(8),posdate,112)) AS posdate,sitenum,detail2 as paytype,reason,OrderSNo,-oriamt AS tenderamt FROM mcr1" +
					//    " ) posMcr " +
					//    " WHERE reason not like '%COACH%' " +
					//    " AND sitenum = '" + mcrh["outletid"].ToString().Trim() + "' " +
					//    " AND convert(nvarchar(12),posdate,112) >= '" + Convert.ToDateTime(mcrh["posdatefrom"]).ToString("yyyyMMdd") + "' " +
					//    " AND convert(nvarchar(12),posdate,112) <= '" + Convert.ToDateTime(mcrh["posdateto"]).ToString("yyyyMMdd") + "' " +
					//    " AND ISNULL(paytype,'') in (SELECT ISNULL(paymentmode,'') FROM armpos WHERE arnum='" + mcrh["arnum"].ToString().Trim() + "' " +
					//    " AND module = 'SAJ')" +
					//    " GROUP BY posdate,sitenum,paytype,reason,OrderSNo HAVING SUM(tenderamt)<>0";


					strGetPOS = "SELECT posdate,sitenum,paytype,SUM(tenderamt) AS tenderamt,reason,OrderSNo  " +
						" FROM ( " +
						" SELECT CONVERT(datetime,CONVERT(nvarchar(8),pos2.trandate,112)) AS posdate,pos2.sitenum,"+
						" pos2.paytype,pos2.reason,pos2.OrderSNo,(pos2.tenderamt - pos2.change) as tenderamt FROM pos2 " +
						" LEFT OUTER JOIN posh ON posh.refnum = pos2.refnum " +
						" LEFT OUTER JOIN memoh ON memoh.refnum = pos2.refnum " +
						//steph - amended 09June2010_to take the status from pos2 as there is possibility of one live transactions contain voided items.
						" WHERE (ISNULL(posh.status,'') <>  'V'  AND ISNULL(posh.status,'') <>  'X' AND ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
						"  AND (ISNULL(memoh.status,'') <>  'V'  AND ISNULL(memoh.status,'') <>  'X' AND ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
						//" WHERE (ISNULL(pos2.status,'') <>  'V'  AND ISNULL(pos2.status,'') <>  'X') " +
						//" WHERE (ISNULL(posh.status,'') <>  'V'  AND ISNULL(posh.status,'') <>  'X') " +
						//"  OR (ISNULL(memoh.status,'') <>  'V'  AND ISNULL(memoh.status,'') <>  'X') " +
						" UNION ALL " +
						" SELECT CONVERT(datetime,CONVERT(nvarchar(8),posdate,112)) AS posdate,sitenum, " + 
						" detail2 as paytype,reason,OrderSNo,-oriamt AS tenderamt FROM mcr1" +
						" WHERE status<>'V' "+
						" ) posMcr " +
						" WHERE reason not like '%COACH%' " +
						" AND sitenum = '" + mcrh["outletid"].ToString().Trim() + "' " +
						" AND convert(nvarchar(12),posdate,112) >= '" + Convert.ToDateTime(mcrh["posdatefrom"]).ToString("yyyyMMdd") + "' " +
						" AND convert(nvarchar(12),posdate,112) <= '" + Convert.ToDateTime(mcrh["posdateto"]).ToString("yyyyMMdd") + "' " +
						" AND ISNULL(paytype,'') in (SELECT ISNULL(paymentmode,'') FROM armpos WHERE arnum='" + mcrh["arnum"].ToString().Trim() + "' " +
						" AND module = 'SAJ')" +
						" GROUP BY posdate,sitenum,paytype,reason,OrderSNo HAVING SUM(tenderamt)<>0";


					#endregion


					dbaccess.ReadSQL("getPOS", strGetPOS);
					DataTable getPos = dbaccess.DataSet.Tables["getPOS"];
					if (getPos.Rows.Count > 0)
					{
						if (mcr1.Rows.Count > 0) // show the message only if there is data in Others tab.
						{
							DialogResult result = MessageBox.Show("This will clear all existing data in this page! Continue?", "Clear data?", MessageBoxButtons.YesNo);
							if (result == DialogResult.Yes)
							{
								BizFunctions.DeleteAllRows(mcr1);

								foreach (DataRow dr in getPos.Rows)
								{
									if (dr.RowState != DataRowState.Deleted)
									{
										DataRow newMcr1 = mcr1.Rows.Add(new object[] { });
										newMcr1["posdate"] = Convert.ToDateTime(dr["posdate"]);
										newMcr1["detail"] = "POS - " + Convert.ToDateTime(dr["posdate"]).ToString("dd/MMM/yyyy").Trim();
										newMcr1["sysprice"] = Convert.ToDecimal(dr["tenderamt"]);
										newMcr1["dqty"] = 1;
									}
								}
							}
						}
						else
						{
							foreach (DataRow dr in getPos.Rows)
							{
								if (dr.RowState != DataRowState.Deleted)
								{
									DataRow newMcr1 = mcr1.Rows.Add(new object[] { });
									newMcr1["posdate"] = Convert.ToDateTime(dr["posdate"]);
									newMcr1["detail"] = "POS - " + Convert.ToDateTime(dr["posdate"]).ToString("dd/MMM/yyyy").Trim();
									newMcr1["sysprice"] = Convert.ToDecimal(dr["tenderamt"]);
									newMcr1["dqty"] = 1;
								}
							}
						}
					}
					else
					{
						MessageBox.Show("There is no POS transaction for " + mcrh["outletid"].ToString().Trim() +
								" FROM " + Convert.ToDateTime(mcrh["posdatefrom"]).ToString("dd-MMM-yyyy") +
								" TO " + Convert.ToDateTime(mcrh["posdateto"]).ToString("dd-MMM-yyyy"), "No Transaction", MessageBoxButtons.OK);

					}
				}
			}
			else
			{
				MessageBox.Show("This is not a POS AR. Please check settings in Customer Master!");
				mcrh["outletid"] = String.Empty;
				return;
			}

			setColumnChange("all", false);
			AutoCalc();
			setColumnChange("all", true);

			btnDelete.Enabled = true;
		}

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);

			DataRow mcrh = dbaccess.DataSet.Tables["mcrh"].Rows[0];
			DataTable MCRH = dbaccess.DataSet.Tables["mcrh"];
			DataTable mcr1 = dbaccess.DataSet.Tables["mcr1"];

			setDefaults(dbaccess.DataSet,"mcr1");
			
			setColumnChange("all", false);

			AutoCalc();

			setColumnChange("all", true);
		}

		void Voucher_MCRH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			DataRow mcrh = dbaccess.DataSet.Tables["mcrh"].Rows[0];
			DataTable mcr1 = dbaccess.DataSet.Tables["mcr1"];

			if (mcrhColumnChange)
			{
				switch (e.Column.ColumnName)
				{
					case "arnum":
						setColumnChange("all", false);
						#region Steph - Pull info from ARM
						e.Row.BeginEdit();
						dbaccess.ReadSQL("getArmInfo", "SELECT arnum,arname,ptc,addr1,addr2,addr3,addr4,phone,hp,fax,ptnum,gstgrpnum,oricur,accnum,outletid FROM arm where arnum ='" + e.Row["arnum"].ToString().Trim() + "'");

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
							if (e.Row["accnum"].ToString().Trim() == String.Empty || e.Row["accnum"] == System.DBNull.Value)
								e.Row["accnum"] = getArmInfo["accnum"];

							e.Row["outletid"] = getArmInfo["outletid"];

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
							e.Row["accnum"] = "";
							e.Row["outletid"] = string.Empty;
						}

						AutoCalc();

						setColumnChange("all", true);
						e.Row.EndEdit();
						break;
						#endregion
					case "trandate":
						setColumnChange("all", false);
						e.Row.BeginEdit();

						mcrh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(mcrh["trandate"]));

						setColumnChange("all", true);
						e.Row.EndEdit();
						break;
				}
			}
		}

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick(sender, e);

			DataRow mcrh = dbaccess.DataSet.Tables["mcrh"].Rows[0];
			DataTable mcr1 = dbaccess.DataSet.Tables["mcr1"];

			foreach (DataRow dr in mcr1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(mcrh, dr, "user/created/modified/status");
				}
			}
		}

		#region Column Change Issue
		private void setColumnChange(string type, bool value)
		{
			type = type.ToLower();

			if (type == "header")
			{
				mcrhColumnChange = value;
			}
			if (type == "detail")
			{
				mcr1ColumnChange = value;
			}
			if (type == "all")
			{
				mcrhColumnChange = value;
				mcr1ColumnChange = value;
			}

		}
		#endregion

		#region Steph - To set the Auto Calc to be used in various events
		private void AutoCalc()
		{
			setColumnChange("all", false);
			DataRow mcrh = dbaccess.DataSet.Tables["mcrh"].Rows[0];
			DataTable MCRH = dbaccess.DataSet.Tables["mcrh"];
			DataTable mcr1 = dbaccess.DataSet.Tables["mcr1"];

			setDefaults(dbaccess.DataSet, "mcr1");

			#region mcrh

			#region Steph -  To get pd from pd (nonYear) table.

			mcrh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(mcrh["trandate"]));
            if (mcrh["refnum"].ToString().Trim().Contains("MCR"))
            {
                if (mcrh["docunum"] == System.DBNull.Value || mcrh["docunum"].ToString().Trim() == String.Empty)
                {
                    mcrh["docunum"] = mcrh["refnum"];
                }
            }
			#endregion

			this.dbaccess.ReadSQL("gstm", "SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='" + mcrh["gstgrpnum"].ToString() + "'");
			if (this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
			{
				if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
				{
					if (mcrh["gstper"] == System.DBNull.Value||(decimal)mcrh["gstper"] == 0)
					{
						mcrh["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)mcrh["trandate"]);
					}
				}
				else
				{
					mcrh["gstper"] = 0;
				}
			}

			string strexr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + mcrh["oricur"].ToString().Trim() + "'";
			dbaccess.ReadSQL("exrate", strexr);
			if (dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
			{
				decimal exrate = Convert.ToDecimal(dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)mcrh["trandate"]) + ""]);
				mcrh["exrate"] = exrate;
			}
			#endregion

			#region initialise values
			mcr1_oriamt = 0;
			mcr1_origstamt = 0;
			mcr1_postamt = 0;
			mcr1_gstamt = 0;
			mcr1_grosamt = 0;
			mcr1_roriamt = 0;
			mcr1_rpostamt = 0;
			#endregion

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum,accnum2 FROM arm WHERE arnum = '" + mcrh["arnum"].ToString().Trim() + "'");

			dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
						" WHERE gstgrpnum ='" + mcrh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

			#region mcr1
			foreach (DataRow dr in mcr1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(mcrh, dr);
					BizFunctions.UpdateDataRow(mcrh, dr, "refnum/arnum/docunum/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

					#region get default Sales A/C from arm
					if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
					{
						if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
						{
							dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
						}
					}
					#endregion

					if (dr["price"] == System.DBNull.Value || Convert.ToDecimal(dr["price"]) == 0)
					{
						dr["price"] = dr["sysprice"];
					}

					dr["roriamt"] = BizFunctions.Round((Convert.ToDecimal(dr["dqty"]) * Convert.ToDecimal(dr["price"])) - Convert.ToDecimal(dr["discamt"]));
					dr["rpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(dr["exrate"]));
					if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
					{
						//steph  - do not have to calculate the gst, allow user to key in manually.
					}
					else
					{
						dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(mcrh["gstper"]) / (100 + Convert.ToDecimal(mcrh["gstper"])));
					}
					dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
					dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
					dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
					dr["dgrosamt"] = dr["doriamt"];

					mcr1_oriamt += (decimal)dr["doriamt"];
					mcr1_origstamt += (decimal)dr["dorigstamt"];
					mcr1_postamt += (decimal)dr["dpostamt"];
					mcr1_gstamt += (decimal)dr["dgstamt"];
					mcr1_grosamt += (decimal)dr["dgrosamt"];
					mcr1_roriamt += (decimal)dr["roriamt"];
					mcr1_rpostamt += (decimal)dr["rpostamt"];
				}
			}


			#region Steph - Check the gst amt differences and add/deduct from the first entry of mcr1
			if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
			{
				//steph  - do not have to calculate the gst, allow user to key in manually.
			}
			else
			{
				if (mcr1.Rows.Count > 0)
				{
					decimal headerGst = BizFunctions.Round((mcr1_oriamt + mcr1_origstamt) * Convert.ToDecimal(mcrh["gstper"]) / (100 + Convert.ToDecimal(mcrh["gstper"])));
					if (headerGst != mcr1_origstamt)
					{
						mcr1.Rows[0]["dorigstamt"] = Convert.ToDecimal(mcr1.Rows[0]["dorigstamt"]) + headerGst - mcr1_origstamt;
					}
				}

				#region initialise values
				mcr1_oriamt = 0;
				mcr1_origstamt = 0;
				mcr1_postamt = 0;
				mcr1_gstamt = 0;
				mcr1_grosamt = 0;
				mcr1_roriamt = 0;
				mcr1_rpostamt = 0;
				#endregion

				foreach (DataRow dr in mcr1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgrosamt"] = dr["doriamt"];
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

						mcr1_oriamt += (decimal)dr["doriamt"];
						mcr1_origstamt += (decimal)dr["dorigstamt"];
						mcr1_postamt += (decimal)dr["dpostamt"];
						mcr1_gstamt += (decimal)dr["dgstamt"];
						mcr1_grosamt += (decimal)dr["dgrosamt"];
						mcr1_roriamt += (decimal)dr["roriamt"];
						mcr1_rpostamt += (decimal)dr["rpostamt"];
					}
				}
			}

			#endregion

			mcrh["grosamt"] = mcr1_grosamt;
			mcrh["origstamt"] = mcr1_origstamt;
			mcrh["oriamt"] = mcr1_grosamt + mcr1_origstamt;
			#endregion
			setColumnChange("all", true);

			MDTReader.SetCorrectValue(ref this.dbaccess, "MCR1", "MCR");
		}
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
	}
}

