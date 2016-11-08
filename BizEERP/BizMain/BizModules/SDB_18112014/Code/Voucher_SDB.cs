/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_SDB.cs
 *	Description:    Sales Debit Note Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 
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
using BizRAD.BizAccounts;
using BizRAD.BizReport;
//using BizMDT;
using DEMO.MDT;


namespace ATL.SDB
{
	public class Voucher_SDB : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables
		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;
		protected bool opened = false;
		protected decimal siv1_grosamt = 0;
		protected decimal siv1_oriamt = 0;
		protected decimal siv1_origstamt = 0;
		protected decimal siv1_postamt = 0;
		protected decimal siv1_gstamt = 0;
		protected decimal siv1_discamt = 0;
		protected string detailFormName = null;
		public string documentKey = null;
		protected int TabDetail;
		protected string strAccNotOverwritten = String.Empty;

		protected Button btnExtract = null;
		protected Button getIncomeExpense;

		#endregion

		public Voucher_SDB(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_SDB.xml", moduleName, voucherBaseHelpers)
		{
		}

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
			e.Condition = "sivh.flag='SDB' AND sivh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (sivh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" sivh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" sivh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND sivh.flag='SDB' AND sivh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}			
		#endregion

		protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
		{
			base.AddDocumentF2Condition(sender, e);

			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];

			switch (e.ControlName)
			{
				case "sivh_invnum":
					e.Condition = BizFunctions.F2Condition("sivhView.refnum", (sender as TextBox).Text);
					e.DefaultCondition = "sivhView.status = 'P' and sivhView.arnum = '" + sivh["arnum"].ToString().Trim() + "'";
					break;
			}
		}

        private string geTimeIn(string shiftcode)
        {
            string Timein = "";

            string GetvSHLV = "Select timein from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timein = vSHLVTmp.Rows[0]["timein"].ToString();
            }

            return Timein;
        }

        private string geTimeOut(string shiftcode)
        {
            string Timeout = "";

            string GetvSHLV = "Select [timeout] from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timeout = vSHLVTmp.Rows[0]["timeout"].ToString();
            }

            return Timeout;
        }

		#region F3

		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);

			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			switch (e.ControlName)
			{
				case "sivh_invnum":
					    #region Extraction from sivh(SIV) to sivh(SCR)
					string invnum = sivh["invnum"].ToString().Trim();
					if (!invnum.Equals(""))
					{
						string selectSivh = "SELECT * FROM sivh WHERE refnum ='" + sivh["invnum"].ToString().Trim() + "'";
						this.dbaccess.ReadSQL("sivhTmp", selectSivh);
						DataTable sivhTmp = this.dbaccess.DataSet.Tables["sivhTmp"];

						if (sivhTmp.Select().Length > 0)
						{
							sivh["arnum"] = sivhTmp.Rows[0]["arnum"];
							sivh["custpo"] = sivhTmp.Rows[0]["custpo"];
							sivh["oricur"] = sivhTmp.Rows[0]["oricur"];
							sivh["remark"] = sivhTmp.Rows[0]["remark"];
							sivh["gstgrpnum"] = sivhTmp.Rows[0]["gstgrpnum"];
						}

						else
						{
							sivh["arnum"] = "";
							sivh["custpo"] = "";
							sivh["oricur"] = "";
							sivh["remark"] = "";
							sivh["gstgrpnum"] = "";
						}
					#endregion
						#region Steph - Copy the detail page

						BizFunctions.DeleteAllRows(siv1);

						if (sivhTmp.Rows.Count != 0)
						{
							#region Steph - Import siv1 to siv1

							string selectSiv1 = "SELECT invnum,matnum,uom,discamt,detail,sum(qty) as qty,price from siv1" +
								" WHERE refnum = '" + sivh["invnum"].ToString().Trim() + "'" +
								"  GROUP BY invnum,matnum,uom,price,discamt,detail";

							this.dbaccess.ReadSQL("siv1Tmp", selectSiv1);
							DataTable siv1Tmp = this.dbaccess.DataSet.Tables["siv1Tmp"];

							BizFunctions.DeleteAllRows(siv1);
							foreach (DataRow dr in siv1Tmp.Select())
							{
								dr.SetAdded();
								siv1.ImportRow(dr);
							}
							foreach (DataRow dr2 in siv1.Select())
							{
								dr2["refnum"] = sivh["refnum"].ToString().Trim();

								#region Steph - SOR is not using the dqty
								dr2["dqty"] = (decimal)dr2["qty"] * -1;
								#endregion
							}
							#endregion
						}
					}

					AutoCalc();
						#endregion

					break;

                case "sivh_arnum":
                    //e.CurrentRow["contact"] = e.F2CurrentRow["ptc"];
                    if (!BizFunctions.IsEmpty(e.CurrentRow["arnum"]))
                    {
                        e.CurrentRow["contact"] = e.F2CurrentRow["bptc"];



                        e.CurrentRow["addr1"] = e.F2CurrentRow["baddr1"].ToString();


                        e.CurrentRow["addr2"] = e.F2CurrentRow["baddr2"].ToString();


                        e.CurrentRow["addr3"] = e.F2CurrentRow["baddr3"].ToString();


                        e.CurrentRow["addr4"] = e.F2CurrentRow["baddr4"].ToString();


                        e.CurrentRow["phone"] = e.F2CurrentRow["phone"].ToString();


                        e.CurrentRow["hp"] = e.F2CurrentRow["hp"].ToString();


                        e.CurrentRow["fax"] = e.F2CurrentRow["fax"].ToString();


                        //////

                        e.CurrentRow["saddr1"] = e.F2CurrentRow["baddr1"].ToString();


                        e.CurrentRow["saddr2"] = e.F2CurrentRow["baddr2"].ToString();


                        e.CurrentRow["saddr3"] = e.F2CurrentRow["baddr3"].ToString();


                        e.CurrentRow["saddr4"] = e.F2CurrentRow["baddr4"].ToString();

                        string GetArm = "SELECT * FROM ARM WHERE ARNUM='" + e.CurrentRow["arnum"].ToString() + "'";

                        this.dbaccess.ReadSQL("TempARM", GetArm);
                    }
                    break;                

			}
		}

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);

            switch (e.MappingName)
            {
                case "matnum":
                    e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    AutoCalc();
                    break;

                //San*
                case "sivh_arnum":
                    e.CurrentRow["contact"] = e.F2CurrentRow["ptc"];
                    break;
                //San_End
            }
        }

		#endregion
			
		#region TabControl Handle

		protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_TabControl_Handle(sender, e);

			TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
		}

		#endregion

		#region Document Handle
		protected override void Document_Paste_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Paste_Handle (sender, e);
			e.DBAccess.DataSet.Tables["sivh"].ColumnChanged -= new DataColumnChangeEventHandler(Voucher_SIVH_ColumnChanged);
		}

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow sivh = e.DBAccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];

			if(BizValidate.ChkPeriodLocked(e.DBAccess, sivh["period"].ToString()))
			{
				MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

            foreach (DataRow dr1 in siv1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr1["shiftcode"]))
                    {
                        dr1["timein"] = geTimeIn(dr1["shiftcode"].ToString());
                        dr1["timeout"] = geTimeOut(dr1["shiftcode"].ToString());
                    }
                }
            }
		}

		protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Preview_Handle (sender, e);
			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			if (sivh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
			{
				if (BizValidate.CheckRowState(e.DBAccess.DataSet, "sivh"))
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
			base.Document_Form_OnLoad(sender, e);
			opened = true;
			documentKey = e.DocumentKey;

			DataRow sivh = e.DBAccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
			string headerFormName = (e.FormsCollection["header"] as Form).Name;
			string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;

			if (sivh["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "sivh_trandate", sivh);
			}

			sivh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			this.dbaccess = e.DBAccess;
			// Initialise event handlers for button clicked in detail page.

			getIncomeExpense = BizXmlReader.CurrentInstance.GetControl(ccDistFormName, "btn_getIncomeExpense") as Button;
			getIncomeExpense.Click += new EventHandler(getIncomeExpense_Click);

			btnExtract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Extract") as Button;
			btnExtract.Click += new EventHandler(btnExtract_Click);

			// Set link to database
			e.DBAccess.DataSet.Tables["sivh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SIVH_ColumnChanged);
			e.DBAccess.DataSet.Tables["siv1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SIV1_ColumnChanged);

            string GetvSHLV = "select * from vshlv";
            this.dbaccess.ReadSQL("vSHLV", GetvSHLV);

            if (BizFunctions.IsEmpty(sivh["gstgrpnum"]))
            {
                sivh["gstgrpnum"] = "SGST";
            }

            if (BizFunctions.IsEmpty(sivh["oricur"]))
            {
                sivh["oricur"] = "SGD";
            }

            if (!BizFunctions.IsEmpty(sivh["arnum"]))
            {
                string GetArm = "SELECT * FROM ARM WHERE ARNUM='" + sivh["arnum"].ToString() + "'";

                this.dbaccess.ReadSQL("TempARM", GetArm);
            }
		}

		void btnExtract_Click(object sender, EventArgs e)
		{
			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			if (sivh["arnum"].ToString() != string.Empty)
			{
				Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
				DataTable oriTable = siv1;

				try
				{
					// Open Extract Form
					ExtractSIVForm ExtractSIV = new ExtractSIVForm(this.dbaccess, oriTable);
					ExtractSIV.ShowDialog(frm);
				}
				catch (Exception ex)
				{
					MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			else
			{
				MessageBox.Show("Invalid Customer Code!");
				return;
			}
			AutoCalc();
		}

		void getIncomeExpense_Click(object sender, EventArgs e)
		{
			DataTable sivc = dbaccess.DataSet.Tables["sivc"];
			DataTable getIE = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,sum(oriamt) AS oriamt,oricur,exrate FROM [siv1] GROUP BY accnum,oricur,exrate");

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

		#endregion

		#region Reopen Handle

		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
			DataRow sivh = this.dbaccess.DataSet.Tables["sivh"].Rows[0];

			#region Steph - Do not allow reopen if voucher has been created for this Sales Credit Note - Ex: Sales Receipt
			string strCheckArdReopen = "SELECT refnum FROM ard" + Common.DEFAULT_SYSTEM_YEAR + 
				" WHERE ISNULL(invnum,'') in (SELECT ISNULL(invnum,'') AS invnum "+
				" FROM siv1 WHERE refnum = '" + sivh["refnum"].ToString().Trim() + "') "+
				" AND refnum <>'" + sivh["refnum"].ToString().Trim() + "' and flag<>'SIV' and flag<>'SCR'";

			dbaccess.ReadSQL("checkArdReopen", strCheckArdReopen);

			DataTable checkArdReopen = dbaccess.DataSet.Tables["checkArdReopen"];
			string RefList = "Please check entries below which has been created for this Sales Debit Note:";
			foreach (DataRow dr in checkArdReopen.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					RefList = RefList + "\n " + dr["refnum"].ToString().Trim();
				}
			}
			#endregion

			if (RefList != "Please check entries below which has been created for this Sales Debit Note:")
			{
				MessageBox.Show(RefList, "Reopen Unsuccessful");
				e.Handle = false;
			}

			if (sivh["flag"].ToString().Trim() != "SDB")
			{
				e.Handle = false;
			}
			else
			{
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM ARD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");
			}
		}

		#endregion
        
		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{          
			base.Document_Refresh_OnClick (sender, e);
			DataRow sivh = e.DBAccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
            DataTable gld = e.DBAccess.DataSet.Tables["gld"];

			setDefaults(dbaccess.DataSet, "sivh/siv1");


            if (!BizFunctions.IsEmpty(sivh["arnum"]) && this.dbaccess.DataSet.Tables["TempARM"] != null && (sivh["arnum"].ToString() != (string)Common.DEFAULT_DOCUMENT_STATUSP || sivh["arnum"].ToString() != (string)Common.DEFAULT_DOCUMENT_STATUSV))
            {
                DataRow TempARM = e.DBAccess.DataSet.Tables["TempARM"].Rows[0];

                sivh["contact"] = string.Empty;
                sivh["addr1"] = string.Empty;
                sivh["addr2"] = string.Empty;
                sivh["addr3"] = string.Empty;
                sivh["addr4"] = string.Empty;
                sivh["phone"] = string.Empty;
                sivh["hp"] = string.Empty;
                sivh["fax"] = string.Empty;
                sivh["saddr1"] = string.Empty;
                sivh["saddr2"] = string.Empty;
                sivh["saddr3"] = string.Empty;
                sivh["saddr4"] = string.Empty;

                sivh["contact"] = TempARM["bptc"];
                sivh["addr1"] = TempARM["baddr1"].ToString();
                sivh["addr2"] = TempARM["baddr2"].ToString();
                sivh["addr3"] = TempARM["baddr3"].ToString();
                sivh["addr4"] = TempARM["baddr4"].ToString();
                sivh["phone"] = TempARM["phone"].ToString();
                sivh["hp"] = TempARM["hp"].ToString();
                sivh["fax"] = TempARM["fax"].ToString();
                sivh["saddr1"] = TempARM["baddr1"].ToString();
                sivh["saddr2"] = TempARM["baddr2"].ToString();
                sivh["saddr3"] = TempARM["baddr3"].ToString();
                sivh["saddr4"] = TempARM["baddr4"].ToString();
            }

			//if (sivh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSN)
			//{
			//    if (sivh["docunum"].ToString().Trim() == String.Empty || sivh["docunum"] == System.DBNull.Value)
			//        sivh["docunum"] = sivh["refnum"];
			//}

			AutoCalc();			
        }

		#endregion
       
        #region Extract Handle
        protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Extract_Handle(sender, e);
			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			#region Steph - MDT Extraction

			Hashtable HsExtract = MDTReader.GetExtraction("SDB", "SIV-SDB Extract", TabDetail, this.dbaccess);

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

			e.Handle = false;			         
				
        }
        #endregion Extract Handle


		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);
			DataRow sivh = e.DBAccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
			DataTable siv10 = e.DBAccess.DataSet.Tables["siv10"];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable gld = e.DBAccess.DataSet.Tables["gld"];

			setDefaults(dbaccess.DataSet, "siv1/siv10/ard/gld");

			AutoCalc();	

			#region Steph - Error Checking!
			string DataEntryErrors = "Please check the following:";

			dbaccess.ReadSQL("checkAcm", "SELECT accnum,accname FROM acm");

			DataTable getEmptyAcSivh = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum from [sivh] WHERE isnull(accnum,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcSivh.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. (Debit A/C) in Header";
			}
		
			DataTable getEmptyAcSiv1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [siv1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcSiv1.Rows.Count > 0)
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
				#region Voucher Confirmed - Posting Time!
				MDTReader.updateAccount(ref this.dbaccess, "refnum|trandate|status|" +
				"accnum|arnum|exrate|detail|flag|lgr|gstamt|invdate|dpostamt|postamt|doriamt|oriamt|oricur|period|oricredit|oridebit|invnum|gstamt|exramt|locno|deptno",
				"SIVH", "SDB", "ard", "siv10", "SIV-SDB EXTRACT");

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
									DataTable sivcGrpWCC = BizFunctions.ExecuteQuery(dbaccess.DataSet, " SELECT accnum,ccnum,oricur,exrate,SUM(oriamt) AS oriamt, " +
										" SUM(postamt) AS postamt FROM [sivc] WHERE accnum ='" + dr["accnum"].ToString().Trim() + "' GROUP BY accnum,ccnum,oricur,exrate");
									foreach (DataRow drCC in sivcGrpWCC.Rows)
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

						//Steph - If this is not overwritten, must delete all from siv10, the temp table.
						BizFunctions.DeleteAllRows(siv10);

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
							dr["detail"] = sivh["remark"];
							if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
								dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.
						}
					}
					#endregion

                    #region update status to ard
                    foreach (DataRow dr in ard.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            dr["status"] = sivh["status"];
                        }
                    }
                    #endregion

				#endregion
			}
			#endregion
		}
        #region SaveBegin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
        }         
		#endregion	

        #region Tab Control
        protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
        {
            base.Document_TabControl_OnSelectionChanged(sender, e);
            Button btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
			Button btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
			Button btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;
			Button btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
			Button btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
			Button btnExtract = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Extract") as Button;

			btnExtract.Enabled = false;
           
            switch ((sender as LXFClassLibrary.Controls.TabControl).SelectedIndex)
            {
				case 0:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnMark.Enabled = true;
					break;           
            }
        }
        #endregion
        
        #region Preview
        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick (sender, e);
            
            DataRow sivh = e.DBAccess.DataSet.Tables["sivh"].Rows[0];
            Hashtable selectedCollection = new Hashtable();
           
            switch (e.ReportName)
            {
				case "Sales Debit Note":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
					selectedCollection.Add("matm", "SELECT * FROM matm");					
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;
			}            
		}
		#endregion

		#endregion

		#region ColumnChangedEvents

		#region sivh

        private void Voucher_SIVH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

            switch (e.Column.ColumnName)
            {
				case "arnum":
					#region Steph - Pull info from ARM
					dbaccess.ReadSQL("getArmInfo", "SELECT arnum,arname,bptc,baddr1,baddr2,baddr3,baddr4,bphone,bhp,bfax,ptnum,gstgrpnum,oricur,accnum FROM arm where arnum ='" + e.Row["arnum"].ToString().Trim() + "'");

					if (dbaccess.DataSet.Tables["getArmInfo"].Rows.Count > 0)
					{
						DataRow getArmInfo = dbaccess.DataSet.Tables["getArmInfo"].Rows[0];
						e.Row["detail"] = getArmInfo["arname"];
						//e.Row["contact"] = getArmInfo["ptc"];
						e.Row["addr1"] = getArmInfo["baddr1"];
						e.Row["addr2"] = getArmInfo["baddr2"];
						e.Row["addr3"] = getArmInfo["baddr3"];
						e.Row["addr4"] = getArmInfo["baddr4"];
						e.Row["phone"] = getArmInfo["bphone"];
						e.Row["hp"] = getArmInfo["bhp"];
						e.Row["fax"] = getArmInfo["bfax"];

						if (e.Row["payterms"].ToString().Trim() == "" || e.Row["payterms"] == System.DBNull.Value)
							e.Row["payterms"] = getArmInfo["ptnum"];
						if (e.Row["gstgrpnum"].ToString().Trim() == "" || e.Row["gstgrpnum"] == System.DBNull.Value)
							e.Row["gstgrpnum"] = getArmInfo["gstgrpnum"];
						if (e.Row["oricur"].ToString().Trim() == "" || e.Row["oricur"] == System.DBNull.Value)
							e.Row["oricur"] = getArmInfo["oricur"];
						if (e.Row["accnum"].ToString().Trim() == String.Empty || e.Row["accnum"] == System.DBNull.Value)
							e.Row["accnum"] = getArmInfo["accnum"];

					}
					else
					{
						e.Row["detail"] = "";
						//e.Row["contact"] = "";
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
						e.Row["accnum"] = "";
					}
					break;
					#endregion
				case "oricur":
					#region set exrate
					e.Row.BeginEdit();
					string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					this.dbaccess.ReadSQL("exrate", exrStr);
					if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					{
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
						e.Row["exrate"] = exrate;
					}
					e.Row.EndEdit();
					break;
					#endregion
				case "gstgrpnum":
					#region set gstper

					e.Row.BeginEdit();
					this.dbaccess.ReadSQL("gstm", "SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='" + e.Row[e.Column.ColumnName].ToString() + "'");
					if (this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
					{
						if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
						{
							if ((decimal)e.Row["gstper"] == 0 || e.Row["gstper"] == System.DBNull.Value)
							{
								e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)e.Row["trandate"]);
							}
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
					//e.Row["exrate"] = BizAccounts.GetExRate(this.dbaccess, e.Row["oricur"].ToString(), (DateTime)e.Row[e.Column.ColumnName]);
					string strexr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					this.dbaccess.ReadSQL("exrate", strexr);
					if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					{
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" +ATL.BizLogicTools.Tools.GetPd(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
						e.Row["exrate"] = exrate;
					}

					sivh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(sivh["trandate"]));

					e.Row.EndEdit();
					break;

					#endregion				
				case "invnum":
					break;
						
			}
        }            
		#endregion

		#region siv1
		private void Voucher_SIV1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{
				case "matnum":
					#region Steph - Pull Info from MATM
					//dbaccess.ReadSQL("getMatm", "SELECT matname, uom,saleAcc FROM matm where matnum ='" + e.Row["matnum"].ToString().Trim() + "'");
					//if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
					//{
					//    if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
					//        e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
					//    if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
					//        e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uom"];
					//    if (e.Row["accnum"].ToString().Trim() == "" || e.Row["accnum"] == System.DBNull.Value)
					//        e.Row["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
					//}
					AutoCalc();
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
								//case "System.String":
								//        if (dr[dc.ColumnName] != System.DBNull.Value)
								//            dr[dc.ColumnName] = (dr[dc.ColumnName] as String).Trim();
									//break;
							}
						}
					}
				}
			}

		}
		#endregion 

		#region Steph - To set the Auto Calc to be used in various events
		private void AutoCalc()
		{
			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable sivc = dbaccess.DataSet.Tables["sivc"];

			sivh["invdate"] = sivh["trandate"];

			if (sivh["refnum"].ToString().Trim().Contains("SDB"))
			{
				if (sivh["docunum"] == System.DBNull.Value || sivh["docunum"].ToString().Trim() == String.Empty)
				{
					sivh["docunum"] = sivh["refnum"];
				}
			}			
			
            #region sivh Contact textbox
            dbaccess.ReadSQL("getContactInfo", "Select bptc from arm where arnum = '" + sivh["arnum"] + "' ");
            if (dbaccess.DataSet.Tables["getContactInfo"].Rows.Count > 0)
            {
                if (sivh["contact"].ToString() == String.Empty || sivh["contact"] == System.DBNull.Value)
                {
                    sivh["contact"] = dbaccess.DataSet.Tables["getContactInfo"].Rows[0]["bptc"];
                }
            }
            #endregion

			#region Steph -  To get pd from pd (nonYear) table.

			sivh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(sivh["trandate"]));

			#endregion

			#region initialise values
			siv1_grosamt = 0;
			siv1_discamt = 0;
			siv1_oriamt = 0;
			siv1_origstamt = 0;
			siv1_postamt = 0;
			siv1_gstamt = 0;
			#endregion

			#region siv1

			decimal myline = 0;//x
			decimal line = 0;

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum2 FROM arm WHERE arnum = '" + sivh["arnum"].ToString().Trim() + "'");

			dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
					" WHERE gstgrpnum ='" + sivh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

			if ((bool)sivh["inclgst"])
			{
				#region Steph - Inclusive GST calculations
				#region initialise values
				siv1_discamt = 0;
				siv1_oriamt = 0;
				siv1_origstamt = 0;
				siv1_postamt = 0;
				siv1_gstamt = 0;
				siv1_grosamt = 0;
				myline = 0;
				line = 0;
				//siv1_roriamt = 0;
				//siv1_rpostamt = 0;
				#endregion

				foreach (DataRow dr in siv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(sivh, dr);
						BizFunctions.UpdateDataRow(sivh, dr, "refnum/arnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

						#region Steph - Adding of hscode and country of origin: 101109_1
						//read hscode from pcatm, instead of matm_24Nov2009_0941
						//dbaccess.ReadSQL("getHsCountry", "SELECT hscode, oricountry FROM matm WHERE matnum = '" + dr["matnum"].ToString().Trim() + "'");
						dbaccess.ReadSQL("getHsCountry", "SELECT pcatm.hscode AS hscode, matm.oricountry AS oricountry FROM matm " +
							" LEFT OUTER JOIN pcatm ON matm.pcatcode = pcatm.pcatcode " +
							" WHERE matm.matnum = '" + dr["matnum"].ToString().Trim() + "'");

						if (dbaccess.DataSet.Tables["getHsCountry"].Rows.Count > 0)
						{
							dr["hscode"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["hscode"];
							dr["oricountry"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["oricountry"];
						}
						#endregion

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
							dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(sivh["gstper"]) / (100 + Convert.ToDecimal(sivh["gstper"])));
						}
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
						dr["dgrosamt"] = dr["doriamt"];

						//steph - to assign myline for print purpose
						if (Convert.ToDecimal(dr["dqty"]) > 0)
						{
							myline = myline + 1;
							dr["myline"] = myline;
						}
						else
						{
							dr["myline"] = 0;
						}

						siv1_discamt += (decimal)dr["discamt"];
						siv1_oriamt += (decimal)dr["doriamt"];
						siv1_origstamt += (decimal)dr["dorigstamt"];
						siv1_postamt += (decimal)dr["dpostamt"];
						siv1_gstamt += (decimal)dr["dgstamt"];
						siv1_grosamt += (decimal)dr["dgrosamt"];
						//siv1_roriamt += (decimal)dr["roriamt"];
						//siv1_rpostamt += (decimal)dr["rpostamt"];

						#region Steph - Pull Info from MATM
						dbaccess.ReadSQL("getMatm", "SELECT matname,uomcode,saleAcc FROM matm where matnum ='" + dr["matnum"].ToString().Trim() + "'");
						if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
						{
							if (dr["detail"].ToString().Trim() == "" || dr["detail"] == System.DBNull.Value)
								dr["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
							if (dr["uom"].ToString().Trim() == "" || dr["detail"] == System.DBNull.Value)
								dr["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
							if (dr["accnum"].ToString().Trim() == "" || dr["accnum"] == System.DBNull.Value)
								dr["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
						}
						#endregion
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
						decimal headerGst = BizFunctions.Round((siv1_oriamt + siv1_origstamt) * Convert.ToDecimal(sivh["gstper"]) / (100 + Convert.ToDecimal(sivh["gstper"])));
						if (headerGst != siv1_origstamt)
						{
							siv1.Rows[0]["dorigstamt"] = Convert.ToDecimal(siv1.Rows[0]["dorigstamt"]) + headerGst - siv1_origstamt;
						}
					}

					#region initialise values
					siv1_discamt = 0;
					siv1_oriamt = 0;
					siv1_origstamt = 0;
					siv1_postamt = 0;
					siv1_gstamt = 0;
					siv1_grosamt = 0;
					//siv1_roriamt = 0;
					//siv1_rpostamt = 0;
					#endregion

					foreach (DataRow dr in siv1.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
							dr["dgrosamt"] = dr["doriamt"];
							dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
							dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

							siv1_discamt += (decimal)dr["discamt"];
							siv1_oriamt += (decimal)dr["doriamt"];
							siv1_origstamt += (decimal)dr["dorigstamt"];
							siv1_postamt += (decimal)dr["dpostamt"];
							siv1_gstamt += (decimal)dr["dgstamt"];
							siv1_grosamt += (decimal)dr["dgrosamt"];
							//siv1_roriamt += (decimal)dr["roriamt"];
							//siv1_rpostamt += (decimal)dr["rpostamt"];
						}
					}
				}

				#endregion
				#endregion
			}
			else
			{
				#region Steph - Exclusive GST calculations

				#region initialise values
				siv1_discamt = 0;
				siv1_oriamt = 0;
				siv1_origstamt = 0;
				siv1_postamt = 0;
				siv1_gstamt = 0;
				siv1_grosamt = 0;
				//siv1_roriamt = 0;
				//siv1_rpostamt = 0;
				myline = 0;
				line = 0;
				#endregion

				foreach (DataRow dr in siv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(sivh, dr);
						BizFunctions.UpdateDataRow(sivh, dr, "refnum/arnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

						#region Steph - Adding of hscode and country of origin: 101109_1
						//reading hscode from pcatm, instead of matm_24Nov2009_0944
						//dbaccess.ReadSQL("getHsCountry", "SELECT hscode, oricountry FROM matm WHERE matnum = '" + dr["matnum"].ToString().Trim() + "'");
						dbaccess.ReadSQL("getHsCountry", "SELECT pcatm.hscode AS hscode, matm.oricountry AS oricountry FROM matm " +
							" LEFT OUTER JOIN pcatm ON matm.pcatcode = pcatm.pcatcode " +
							" WHERE matm.matnum = '" + dr["matnum"].ToString().Trim() + "'");

						if (dbaccess.DataSet.Tables["getHsCountry"].Rows.Count > 0)
						{
							dr["hscode"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["hscode"];
							dr["oricountry"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["oricountry"];
						}
						#endregion

						if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
						{
							if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
							}
						}

						dr["dgrosamt"] = BizFunctions.Round((decimal)dr["dqty"] * (decimal)dr["price"]);
						dr["doriamt"] = (decimal)dr["dgrosamt"] - (decimal)dr["discamt"];

						if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
						{
							//steph  - do not have to calculate the gst, allow user to key in manually.
						}
						else
						{
							dr["dorigstamt"] = BizFunctions.Round((decimal)dr["doriamt"] * ((decimal)sivh["gstper"] / 100));
						}
						dr["dpostamt"] = BizFunctions.Round((decimal)dr["doriamt"] * (decimal)dr["exrate"]);
						dr["dgstamt"] = BizFunctions.Round((decimal)dr["dorigstamt"] * (decimal)dr["exrate"]);
						dr["roriamt"] = BizFunctions.Round((decimal)dr["doriamt"] + (decimal)dr["dorigstamt"]);
						dr["rpostamt"] = BizFunctions.Round((decimal)dr["dpostamt"] + (decimal)dr["dgstamt"]);

						//steph - to assign myline for print purpose
						if (Convert.ToDecimal(dr["dqty"]) > 0)
						{
							myline = myline + 1;
							dr["myline"] = myline;
						}
						else
						{
							dr["myline"] = 0;
						}

						siv1_grosamt += (decimal)dr["dgrosamt"];
						siv1_oriamt += (decimal)dr["doriamt"];
						siv1_origstamt += (decimal)dr["dorigstamt"];
						siv1_postamt += (decimal)dr["dpostamt"];
						siv1_gstamt += (decimal)dr["dgstamt"];
						siv1_discamt += (decimal)dr["discamt"];

						if (dr["invnum"].ToString().Trim() == String.Empty || dr["invnum"] == System.DBNull.Value)
							dr["invnum"] = dr["refnum"];
						if (dr["invdate"] == System.DBNull.Value)
							dr["invdate"] = dr["trandate"];
					}
				}
				#endregion
			}

			sivh["discamt"] = siv1_discamt;
			sivh["grosamt"] = siv1_grosamt;
			sivh["origstamt"] = siv1_origstamt;
			sivh["gstamt"] = siv1_gstamt;
			sivh["oriamt"] = siv1_oriamt + siv1_origstamt;
			sivh["postamt"] = siv1_postamt + siv1_gstamt;

			#endregion

			#region sivc
			foreach (DataRow dr in sivc.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(sivh, dr, "oricur/exrate");

					dr["postamt"] = Convert.ToDecimal(dr["oriamt"]) * Convert.ToDecimal(dr["exrate"]);
				}
			}
			#endregion

			MDTReader.SetCorrectValue(ref this.dbaccess, "SIV1", "SDB");
		}
		#endregion
	}
}
