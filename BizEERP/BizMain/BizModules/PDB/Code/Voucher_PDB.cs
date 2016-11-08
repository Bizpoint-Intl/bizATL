/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_PDB.cs
 *	Depdbiption:    Purchase Debit Note Vouchers
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


namespace ATL.PDB
{
	public class Voucher_PDB : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables
		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;
		protected bool opened = false;
		protected decimal piv1_grosamt = 0;
		protected decimal piv1_oriamt = 0;
		protected decimal piv1_origstamt = 0;
		protected decimal piv1_postamt = 0;
		protected decimal piv1_gstamt = 0;
		protected string detailFormName = null;
		public string documentKey = null;
		protected int TabDetail;

		protected Button getIncomeExpense;
		protected Button btnExtract = null;

		#endregion

		public Voucher_PDB(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_PDB.xml", moduleName, voucherBaseHelpers)
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

		protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Cancel_OnClick(sender, e);

			opened = false;
		}
		#endregion

		#region Voucher Default/ALL
		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);
			e.Condition = "pivh.flag='PDB' AND pivh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (pivh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" pivh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" pivh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND pivh.flag='PDB' AND pivh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion


		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);
			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable pivc = dbaccess.DataSet.Tables["pivc"];
			DataTable piv10 = dbaccess.DataSet.Tables["piv10"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			setDefaults(dbaccess.DataSet, "piv1/pivc/piv10/apd/gld");

			AutoCalc();

			#region Steph - Error Checking!
			string DataEntryErrors = "Please check the following:";

			dbaccess.ReadSQL("checkAcm", "SELECT accnum,accname FROM acm");

			DataTable getEmptyAcPivh = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum from [pivh] WHERE isnull(accnum,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcPivh.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. (Credit A/C) in Header";
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
				#region Steph - Posting Time!!
				MDTReader.updateAccount(ref this.dbaccess, "refnum|trandate|" +
				"accnum|apnum|exrate|detail|flag|lgr|gstamt|invdate|dpostamt|postamt|doriamt|oriamt|" +
				"oricur|period|oricredit|oridebit|invnum|gstamt|exramt|supinvnum",
				"PIVH", "PDB", "apd", "piv10", "PIV-PDB EXTRACT");

				#region steph - Need to post the header's remark into GLD.
				foreach (DataRow dr in gld.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["detail"] = pivh["remark"];
                        dr["docunum"] = pivh["docunum"];
						if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
							dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.
					}
				}
				#endregion
				#endregion
			}
			#endregion
		}

		protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
		{
			base.AddDocumentF2Condition(sender, e);

			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];

			switch (e.ControlName)
			{
				case "pivh_invnum":
					e.Condition = BizFunctions.F2Condition("pivhView.refnum", (sender as TextBox).Text);
					e.DefaultCondition = "pivhView.status = 'P' and pivhView.apnum = '" + pivh["apnum"].ToString().Trim() + "'";
					break;
			}
		}

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            switch (e.MappingName)
            {
                case "matnum":
                    e.Condition = " mtype='STK' and [status]<>'V' ";
                    break;

            }
        }

		#region F3 

		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);

			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			
			switch (e.ControlName)
			{
				case "pivh_invnum":
					#region Extraction from pivh(PIV) to pivh(SCR)
					string invnum = pivh["invnum"].ToString().Trim();
					if (!invnum.Equals(""))
					{
						string selectPivh = "SELECT * FROM pivh WHERE refnum ='" + pivh["invnum"].ToString().Trim() + "'";
						this.dbaccess.ReadSQL("pivhTmp", selectPivh);
						DataTable pivhTmp = this.dbaccess.DataSet.Tables["pivhTmp"];

						if (pivhTmp.Select().Length > 0)
						{
							pivh["apnum"] = pivhTmp.Rows[0]["apnum"];
							//pivh["ponum"] = pivhTmp.Rows[0]["ponum"];
							pivh["oricur"] = pivhTmp.Rows[0]["oricur"];
							pivh["remark"] = pivhTmp.Rows[0]["remark"];
							pivh["gstgrpnum"] = pivhTmp.Rows[0]["gstgrpnum"];
						}

						else
						{
							pivh["apnum"] = "";
							//pivh["ponum"] = "";
							pivh["oricur"] = "";
							pivh["remark"] = "";
							pivh["gstgrpnum"] = "";
						}
					#endregion
						#region Steph - Copy the detail page

						BizFunctions.DeleteAllRows(piv1);

						if (pivhTmp.Rows.Count != 0)
						{
							#region Steph - Import piv1 to piv1

                            string selectPiv1 = "SELECT invnum,matnum,uom,discamt,detail,sum(qty) as qty,price,ccnum,projectid,sitenum from piv1" +
								" WHERE refnum = '" + pivh["invnum"].ToString().Trim() + "'" +
                                "  GROUP BY invnum,matnum,uom,price,discamt,detail,ccnum,projectid,sitenum";

							this.dbaccess.ReadSQL("piv1Tmp", selectPiv1);
							DataTable piv1Tmp = this.dbaccess.DataSet.Tables["piv1Tmp"];

							BizFunctions.DeleteAllRows(piv1);
							foreach (DataRow dr in piv1Tmp.Select())
							{
								dr.SetAdded();
								piv1.ImportRow(dr);
							}
							foreach (DataRow dr2 in piv1.Select())
							{
								dr2["refnum"] = pivh["refnum"].ToString().Trim();
								dr2["dqty"] = dr2["qty"];
							}
							#endregion
						}
					}

					AutoCalc();
						#endregion
					break;
                //San*
                case "pivh_apnum":
                    e.CurrentRow["contact"] = e.F2CurrentRow["ptc"];
                    break;
                //San_End

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
                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    //AutoCalc();
                    break;
            }
        }
        //San_End


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
			e.DBAccess.DataSet.Tables["pivh"].ColumnChanged -= new DataColumnChangeEventHandler(Voucher_PIVH_ColumnChanged);
		}

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow pivh = e.DBAccess.DataSet.Tables["pivh"].Rows[0];
			
			if(BizValidate.ChkPeriodLocked(e.DBAccess, pivh["period"].ToString()))
			{
				MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
		}

		protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Preview_Handle (sender, e);
			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
			if (pivh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
			{
				if (BizValidate.CheckRowState(e.DBAccess.DataSet, "pivh"))
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

			DataRow pivh = e.DBAccess.DataSet.Tables["pivh"].Rows[0];
			DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
			string headerFormName = (e.FormsCollection["header"] as Form).Name;
			string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;

			if (pivh["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "pivh_trandate", pivh);
			}

			pivh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			this.dbaccess = e.DBAccess;
			// Initialise event handlers for button clicked in detail page.

			getIncomeExpense = BizXmlReader.CurrentInstance.GetControl(ccDistFormName, "btn_getIncomeExpense") as Button;
			getIncomeExpense.Click += new EventHandler(getIncomeExpense_Click);

			btnExtract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Extract") as Button;
			btnExtract.Click += new EventHandler(btnExtract_Click);

			// Set link to database
			e.DBAccess.DataSet.Tables["pivh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PIVH_ColumnChanged);
			e.DBAccess.DataSet.Tables["piv1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PIV1_ColumnChanged);
		}

		void btnExtract_Click(object sender, EventArgs e)
		{
			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];

			if (pivh["apnum"].ToString() != string.Empty)
			{
				Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
				DataTable oriTable = piv1;

				try
				{
					// Open Extract Form
					ExtractPIVForm ExtractPIV = new ExtractPIVForm(this.dbaccess, oriTable);
					ExtractPIV.ShowDialog(frm);
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
			DataTable getIE = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,sum(oriamt) AS oriamt,oricur,exrate FROM [piv1] GROUP BY accnum,oricur,exrate");

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

		#region Reopen Handle

		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
			DataRow pivh = this.dbaccess.DataSet.Tables["pivh"].Rows[0];

			if (pivh["flag"].ToString().Trim() != "PDB")
			{
				e.Handle = false;
			}
            else if (BizValidate.ChkPeriodLocked(e.DBAccess, pivh["period"].ToString()))
            {
                MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
            }
			else
			{
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM APD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + pivh["refnum"].ToString().Trim() + "'");
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + pivh["refnum"].ToString().Trim() + "'");
			}
		}

		#endregion
        
		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{          
			base.Document_Refresh_OnClick (sender, e);
			DataRow pivh = e.DBAccess.DataSet.Tables["pivh"].Rows[0];
			DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
			DataTable apd = e.DBAccess.DataSet.Tables["apd"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
            DataTable gld = e.DBAccess.DataSet.Tables["gld"];
			setDefaults(dbaccess.DataSet, "pivh/piv1");

			//if (pivh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSN)
			//{
			//    if (pivh["docunum"].ToString().Trim() == String.Empty || pivh["docunum"] == System.DBNull.Value)
			//        pivh["docunum"] = pivh["refnum"];
			//}

            if (BizFunctions.IsEmpty(pivh["invdate"]))
            {
                pivh["invdate"] = pivh["trandate"];
            }

            if (!BizFunctions.IsEmpty(pivh["detail"]) && BizFunctions.IsEmpty(pivh["remark"]))
            {
                pivh["remark"] = pivh["detail"];
            }


			AutoCalc();

			MDTReader.SetCorrectValue(ref this.dbaccess, "PIV1", "PDB");
        }

		#endregion
       
        #region Extract Handle
        protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Extract_Handle(sender, e);
			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
			#region Steph - MDT Extraction

			Hashtable HsExtract = MDTReader.GetExtraction("pdb", "PIV-PDB Extract", TabDetail, this.dbaccess);

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

      
        #region SaveBegin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
			DataRow pivh = e.DBAccess.DataSet.Tables["pivh"].Rows[0];
			DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
			DataTable apd = e.DBAccess.DataSet.Tables["apd"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable gld = e.DBAccess.DataSet.Tables["gld"];

			#region Voucher Confirmed - Posting Time!
			if (pivh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP) // if confim only post into apd ,csh and gld
			{
				//MDTReader.updateAccount(ref this.dbaccess, "refnum|trandate|" +
				//"accnum|apnum|exrate|detail|flag|lgr|gstamt|invdate|dpostamt|postamt|doriamt|oriamt|oricur|period|oricredit|oridebit|invnum|gstamt|exramt|locno|deptno",
				//"PIVH", "PDB", "apd", "piv10", "PIV-PDB EXTRACT");

				//#region steph - Need to post the header's remark into GLD.
				//foreach (DataRow dr in gld.Rows)
				//{
				//    if (dr.RowState != DataRowState.Deleted)
				//    {
				//        dr["detail"] = pivh["remark"];
				//        if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
				//            dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.
				//    }
				//}
				//#endregion
			}
			#endregion
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
            
            DataRow pivh = e.DBAccess.DataSet.Tables["pivh"].Rows[0];
            DataRow piv1 = e.DBAccess.DataSet.Tables["piv1"].Rows[0];
            Hashtable selectedCollection = new Hashtable();
           
            switch (e.ReportName)
            {
                case "Purchase Debit Note":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("apm", "SELECT top 1 * FROM apm where apnum='" + pivh["apnum"].ToString() + "'");
					selectedCollection.Add("matm", "SELECT * FROM matm");
					selectedCollection.Add("hemph", "SELECT * FROM hemph where empnum = '" + pivh["salesman"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;            
                    break;
               
            }            
		}
		#endregion

		#endregion

		#region ColumnChangedEvents

		#region pivh

        private void Voucher_PIVH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];

            switch (e.Column.ColumnName)
            {
				case "apnum":
					#region Steph - Pull info from ARM
					dbaccess.ReadSQL("getApmInfo", "SELECT apnum,apname,ptc,address,phone,hp,fax,ptnum,gstgrpnum,oricur,accnum FROM apm where apnum ='" + e.Row["apnum"].ToString().Trim() + "'");

					if (dbaccess.DataSet.Tables["getApmInfo"].Rows.Count > 0)
					{
						DataRow getApmInfo = dbaccess.DataSet.Tables["getApmInfo"].Rows[0];
						e.Row["detail"] = getApmInfo["apname"];
						//e.Row["contact"] = getApmInfo["ptc"];
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
						if (e.Row["accnum"].ToString().Trim() == String.Empty || e.Row["accnum"] == System.DBNull.Value)
							e.Row["accnum"] = getApmInfo["accnum"];

					}
					else
					{
						e.Row["detail"] = "";
						//e.Row["contact"] = "";
						e.Row["address"] = "";
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
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
						e.Row["exrate"] = exrate;
					}

					pivh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(pivh["trandate"]));

					e.Row.EndEdit();
					break;

					#endregion				
			}
        }            
		#endregion

		#region piv1
		private void Voucher_PIV1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{
				case "matnum":
					#region Steph - Pull Info from MATM
					dbaccess.ReadSQL("getMatm", "SELECT matname, uomcode,purcAcc FROM matm where matnum ='" + e.Row["matnum"].ToString().Trim() + "'");
					if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
					{
						if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
							e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
						if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
							e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
						if (e.Row["accnum"].ToString().Trim() == "" || e.Row["accnum"] == System.DBNull.Value)
							e.Row["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["purcAcc"];
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
			DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];

			setDefaults(dbaccess.DataSet, "piv1");


			if (pivh["refnum"].ToString().Trim().Contains("PDB"))
			{
				if (pivh["docunum"] == System.DBNull.Value || pivh["docunum"].ToString().Trim() == String.Empty)
				{
					pivh["docunum"] = pivh["refnum"];
				}
			}			

            #region San* - Check contact textbox
            dbaccess.ReadSQL("getContactInfo", "Select ptc from apm where apnum = '" + pivh["apnum"] + "' ");
            if (dbaccess.DataSet.Tables["getContactInfo"].Rows.Count > 0)
            {
                if (pivh["contact"].ToString() == String.Empty || pivh["contact"] == System.DBNull.Value)
                {
                    pivh["contact"] = dbaccess.DataSet.Tables["getContactInfo"].Rows[0]["ptc"];
                }
            }
            #endregion San_End

			#region Steph -  To get pd from pd (nonYear) table.
		
			pivh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(pivh["trandate"]));

			#endregion

			#region initialise values
			piv1_grosamt = 0;
			piv1_oriamt = 0;
			piv1_origstamt = 0;
			piv1_postamt = 0;
			piv1_gstamt = 0;
			#endregion

			#region piv1

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum2 FROM apm WHERE apnum = '" + pivh["apnum"].ToString().Trim() + "'");

			if ((bool)pivh["inclgst"])
			{
				#region Steph - Inclusive GST calculations

				#region initialise values
				piv1_oriamt = 0;
				piv1_origstamt = 0;
				piv1_postamt = 0;
				piv1_gstamt = 0;
				piv1_grosamt = 0;
				#endregion

				dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
					" WHERE gstgrpnum ='" + pivh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in piv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(pivh, dr);
						BizFunctions.UpdateDataRow(pivh, dr, "refnum/apnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

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
							dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(pivh["gstper"]) / (100 + Convert.ToDecimal(pivh["gstper"])));
						}
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
						dr["dgrosamt"] = dr["doriamt"];

						piv1_oriamt += (decimal)dr["doriamt"];
						piv1_origstamt += (decimal)dr["dorigstamt"];
						piv1_postamt += (decimal)dr["dpostamt"];
						piv1_gstamt += (decimal)dr["dgstamt"];
						piv1_grosamt += (decimal)dr["dgrosamt"];
					}
				}
				#region Steph - Check the gst amt differences and add/deduct from the first entry of 
				if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
				{
					//steph  - do not have to calculate the gst, allow user to key in manually.
				}
				else
				{
					if (piv1.Rows.Count > 0)
					{
						decimal headerGst = BizFunctions.Round((piv1_oriamt + piv1_origstamt) * Convert.ToDecimal(pivh["gstper"]) / (100 + Convert.ToDecimal(pivh["gstper"])));
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
					piv1_grosamt = 0;
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
							piv1_grosamt += (decimal)dr["dgrosamt"];
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
				piv1_oriamt = 0;
				piv1_origstamt = 0;
				piv1_postamt = 0;
				piv1_gstamt = 0;
				piv1_grosamt = 0;
				#endregion

				dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
					" WHERE gstgrpnum ='" + pivh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in piv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(pivh, dr);
						BizFunctions.UpdateDataRow(pivh, dr, "refnum/apnum/docunum/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

						if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
						{
							if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
							}
						}

						dr["dgrosamt"] = BizFunctions.Round((decimal)dr["dqty"] * (decimal)dr["price"]);
						dr["doriamt"] = dr["dgrosamt"];
						//dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm "+
						//    " WHERE gstgrpnum ='"+dr["gstgrpnum"].ToString().Trim()+"' AND gsttype=3");
						if (dbaccess.DataSet.Tables["checkGST2"].Rows.Count > 0)
						{
							//steph  - do not have to calculate the gst, allow user to key in manually.
						}
						else
						{
							dr["dorigstamt"] = BizFunctions.Round((decimal)dr["doriamt"] * ((decimal)pivh["gstper"] / 100));
						}
						dr["dpostamt"] = BizFunctions.Round((decimal)dr["doriamt"] * (decimal)dr["exrate"]);
						dr["dgstamt"] = BizFunctions.Round((decimal)dr["dorigstamt"] * (decimal)dr["exrate"]);
						dr["roriamt"] = BizFunctions.Round((decimal)dr["doriamt"] + (decimal)dr["dorigstamt"]);
						dr["rpostamt"] = BizFunctions.Round((decimal)dr["dpostamt"] + (decimal)dr["dgstamt"]);

						piv1_grosamt += (decimal)dr["dgrosamt"];
						piv1_oriamt += (decimal)dr["doriamt"];
						piv1_origstamt += (decimal)dr["dorigstamt"];
						piv1_postamt += (decimal)dr["dpostamt"];
						piv1_gstamt += (decimal)dr["dgstamt"];

						//if(dr["invnum"].ToString().Trim() == String.Empty || dr["invnum"] == System.DBNull.Value)
						dr["invnum"] = dr["refnum"];
						//if (dr["invdate"] == System.DBNull.Value)
						dr["invdate"] = dr["trandate"];
					}
				}
				#endregion
			}

			pivh["grosamt"] = piv1_grosamt;
			pivh["origstamt"] = piv1_origstamt;
			pivh["gstamt"] = piv1_gstamt;
			pivh["oriamt"] = piv1_oriamt + piv1_origstamt;
			pivh["postamt"] = piv1_postamt + piv1_gstamt;

			#endregion
		}
		#endregion
	}
}
