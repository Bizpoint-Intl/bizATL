/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_PIV.cs
 *	Description:    Purchase Invoice Voucher
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
using DEMO.MDT;
using ATL.PIV;



namespace ATL.GIV
{
	public class Voucher_GIV : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables	
		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;
		protected bool opened = false;
		protected decimal giv1_grosamt = 0;
		protected decimal giv1_discamt = 0;
		protected decimal giv1_oriamt = 0;
		protected decimal giv1_origstamt = 0;
		protected decimal giv1_postamt = 0;
		protected decimal giv1_gstamt = 0;
		protected decimal giv1_roriamt = 0;
		protected decimal giv1_rpostamt = 0;
	    protected string detailFormName = null;
		public string documentKey = null;
		protected int TabDetail;
		protected string strAccNotOverwritten = String.Empty;
        public string flag = "";
		protected Button getIncomeExpense;
        protected Button btn_Extract_Mat, btn_ExtractGRN;

		#endregion

		public Voucher_GIV(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_GIV.xml", moduleName, voucherBaseHelpers)
		{
            this.flag = moduleName;
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
			e.Condition = "givh.flag='"+flag+"' AND givh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (givh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" givh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" givh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
                " AND givh.flag='" + flag + "' AND givh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];
			DataTable giv1 = dbaccess.DataSet.Tables["giv1"];
			DataTable givc = dbaccess.DataSet.Tables["givc"];
			DataTable giv10 = dbaccess.DataSet.Tables["giv10"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			setDefaults(dbaccess.DataSet, "giv1/givc/giv10/apd/gld");

			AutoCalc();

			#region Steph - Error Checking!
			string DataEntryErrors = "Please check the following:";

			dbaccess.ReadSQL("checkAcm", "SELECT accnum,accname FROM acm");

			DataTable getEmptyAcPivh = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum from [givh] WHERE isnull(accnum,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcPivh.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. (Credit A/C) in Header";
			}

			DataTable getEmptyAcPiv1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [giv1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
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
                //MDTReader.updateAccount(ref this.dbaccess, "refnum|trandate|" +
                //"accnum|apnum|exrate|detail|flag|lgr|gstamt|invdate|dpostamt|postamt|doriamt|oriamt|" +
                //"oricur|period|oricredit|oridebit|invnum|gstamt|exramt|supinvnum",
                //"PIVH", "PIV", "apd", "giv10", "GRN-PIV EXTRACT");

                MDTReader.updateAccount(ref this.dbaccess, "refnum|trandate|" +
                "accnum|apnum|exrate|detail|flag|lgr|gstamt|invdate|dpostamt|postamt|doriamt|oriamt|" +
                "oricur|period|oricredit|oridebit|invnum|gstamt|exramt|supinvnum",
                "GIVH", "GIV", "apd", "giv10", "GRN-PIV EXTRACT");

				#region Steph - To overwrite the posting if there is CC Distribution entries
				// summation of the oriamt and postamt by accnum from the Cost Centre Distribution tab
				DataTable GroupPivcAcc = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,sum(oriamt) as oriamt, sum(postamt) as postamt " +
					" from [givc] group by accnum");

				strAccNotOverwritten = "Please check Account No. for Cost Centre below will not be overwritten:";

				foreach (DataRow dr in GroupPivcAcc.Rows)
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
								DataTable givcGrpWCC = BizFunctions.ExecuteQuery(dbaccess.DataSet, " SELECT accnum,ccnum,oricur,exrate,SUM(oriamt) AS oriamt, " +
									" SUM(postamt) AS postamt FROM [givc] WHERE accnum ='" + dr["accnum"].ToString().Trim() + "' GROUP BY accnum,ccnum,oricur,exrate");
								foreach (DataRow drCC in givcGrpWCC.Rows)
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
										addGL["lgr"] = "PIVC";
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

					//Steph - If this is not overwritten, must delete all from giv10, the temp table.
					BizFunctions.DeleteAllRows(giv10);

					//Steph - Take out the posting for GL and AR since overwrite of the Cost Center is not allowed!
					BizFunctions.DeleteAllRows(apd);
					BizFunctions.DeleteAllRows(gld);
				}
				#endregion

				#region steph - Need to post the header's remark into GLD.

				foreach (DataRow dr in gld.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["detail"] = givh["remark"];
						if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
							dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.
					}
				}
				#endregion

                BizFunctions.DeleteAllRows(apd);// wenyue added Apr 2016,because Eunice say GIV no bussniss w AP
                foreach (DataRow dr in apd.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(givh, dr, "user/coy/flag/status/created/modified");
                    }
                }
				#endregion
			}
			#endregion
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow givh = e.DBAccess.DataSet.Tables["givh"].Rows[0];
           
            if(BizFunctions.IsEmpty(givh["oricur"]))
            {
                givh["oricur"] = "SGD";
            }
			
			if(BizValidate.ChkPeriodLocked(e.DBAccess, givh["period"].ToString()))
			{
				MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

            if (BizFunctions.IsEmpty(givh["accnum"]))
            {
                givh["accnum"] = "14001";
            }
		}

		protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Preview_Handle (sender, e);
			DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];
			if (givh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
			{
				if (BizValidate.CheckRowState(e.DBAccess.DataSet, "givh"))
				{
					MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					e.Handle = false;
				}
			}
		}
		#endregion

		protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
		{
			base.AddDocumentF2Condition(sender, e);

			DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];

			switch (e.ControlName)
			{
				case "givh_grnum":
					e.Condition = BizFunctions.F2Condition("grnh.refnum", (sender as TextBox).Text);
					e.DefaultCondition = "grnh.status = 'P' and grnh.apnum = '" + givh["apnum"].ToString().Trim() + "' "+
						" AND grnh.refnum IN " +
						" (SELECT refnum FROM (SELECT ISNULL(refnum,'') AS refnum,matnum,sum(qty) AS qty FROM " +
						" (SELECT refnum,matnum,qty FROM grn1 " +
						" UNION ALL " +
						" SELECT grnum as refnum,matnum,-qty as qty FROM giv1) GrnVsPiv " +
						" GROUP BY refnum,matnum  " +
						" HAVING SUM(qty) > 0)result)";
					break;
			}
		}

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            switch (e.MappingName)
            {
                case "matnum":
                    e.Condition =" mtype='STK' and [status]<>'V' ";
                    break;

            }
        }


		#region F3
		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);

			DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];
			DataTable giv1 = dbaccess.DataSet.Tables["giv1"];

			switch (e.ControlName)
			{
				case "givh_grnum":
					
                    //#region Extraction from grnh to givh
                    //string grnum = givh["grnum"].ToString().Trim();
                    //if (!grnum.Equals(""))
                    //{
                    //    // 29/05/2015 : Jason - Changed due to that the PIV is grabbing the base qty not the Purchase Qty
                    //    //string selectGrnh = "SELECT * FROM grnh "+
                    //    //        " WHERE refnum in (SELECT refnum FROM (SELECT ISNULL(refnum,'') AS refnum,matnum,"+
                    //    //        " sum(qty) AS qty,uom,discamt,price,detail FROM " +
                    //    //        " (SELECT refnum,matnum,qty,uomcode uom,discamt,price,detail FROM grn1 " +
                    //    //        " UNION ALL " +
                    //    //        " SELECT grnum as refnum,matnum,-qty as qty,uom,discamt,price,detail FROM giv1) GrnVsPiv " +
                    //    //        " GROUP BY refnum,matnum,uom,discamt,price,detail " +
                    //    //        " HAVING SUM(qty) > 0)result) AND refnum = '" + givh["grnum"].ToString().Trim() + "'";

                    //    string selectGrnh = "SELECT * FROM grnh " +
                    //            " WHERE refnum in (SELECT refnum FROM (SELECT ISNULL(refnum,'') AS refnum,matnum," +
                    //            " sum(pqty) AS qty,uom,discamt,price,detail FROM " +
                    //            " (SELECT refnum,matnum,pqty,uomcode uom,discamt,price,detail FROM grn1 " +
                    //            " UNION ALL " +
                    //            " SELECT grnum as refnum,matnum,-qty as pqty,uom,discamt,price,detail FROM giv1) GrnVsPiv " +
                    //            " GROUP BY refnum,matnum,uom,discamt,price,detail " +
                    //            " HAVING SUM(pqty) > 0)result) AND refnum = '" + givh["grnum"].ToString().Trim() + "'";
                    //    this.dbaccess.ReadSQL("grnhTmp", selectGrnh);
                    //    DataTable grnhTmp = this.dbaccess.DataSet.Tables["grnhTmp"];

                    //    if (grnhTmp.Select().Length > 0)
                    //    {
                    //        givh["apnum"] = grnhTmp.Rows[0]["apnum"];
                    //        //givh["ponum"] = grnhTmp.Rows[0]["ponum"];
                    //        givh["oricur"] = grnhTmp.Rows[0]["oricur"];
                    //        givh["remark"] = grnhTmp.Rows[0]["remark"];
                    //        givh["gstgrpnum"] = grnhTmp.Rows[0]["gstgrpnum"];
                    //        givh["supinvnum"] = grnhTmp.Rows[0]["ssivnum"];
                    //    }

                    //    else
                    //    {
                    //        givh["apnum"] = "";
                    //        givh["oricur"] = "";
                    //        givh["remark"] = "";
                    //        givh["gstgrpnum"] = "";
                    //        givh["supinvnum"] = "";
                    //    }
                    //#endregion

                    //#region Steph - Copy the detail page

                    //    BizFunctions.DeleteAllRows(giv1);

                    //    if (grnhTmp.Rows.Count != 0)
                    //    {
                    //        #region Steph - Import grn1 to giv1
                    //        // 29/05/2015 : Jason - Changed due to that the PIV is grabbing the base qty not the Purchase Qty
                    //    //    string selectDor1 = "SELECT ISNULL(refnum,'') AS refnum,matnum,sum(qty) AS qty,uom,discamt,price,detail FROM " +
                    //    //" (SELECT refnum,matnum,qty,uomcode uom,discamt,price,detail FROM grn1 " +
                    //    //" UNION ALL " +
                    //    //" SELECT grnum as refnum,matnum,-qty as qty,uom,discamt,price,detail FROM giv1) GrnVsPiv "+
                    //    //" WHERE refnum = '" + givh["grnum"].ToString().Trim() + "' " +
                    //    //" GROUP BY refnum,matnum,uom,discamt,price,detail  " +
                    //    //" HAVING SUM(qty) > 0";


                    //        string selectDor1 = "SELECT ISNULL(refnum,'') AS refnum,matnum,sum(pqty) AS qty,uom,discamt,price,detail FROM " +
                    //                            " (SELECT refnum,matnum,pqty,uomcode uom,discamt,price,detail FROM grn1 " +
                    //                            " UNION ALL " +
                    //                            " SELECT grnum as refnum,matnum,-qty as pqty,uom,discamt,price,detail FROM giv1) GrnVsPiv " +
                    //                            " WHERE refnum = '" + givh["grnum"].ToString().Trim() + "' " +
                    //                            " GROUP BY refnum,matnum,uom,discamt,price,detail  " +
                    //                            " HAVING SUM(pqty) > 0";


                    //        this.dbaccess.ReadSQL("grn1Tmp", selectDor1);
                    //        DataTable grn1Tmp = this.dbaccess.DataSet.Tables["grn1Tmp"];

                    //        BizFunctions.DeleteAllRows(giv1);
                    //        foreach (DataRow dr in grn1Tmp.Select())
                    //        {
                    //            dr.SetAdded();
                    //            giv1.ImportRow(dr);
                    //        }
                    //        foreach (DataRow dr2 in giv1.Select())
                    //        {
                    //            dr2["refnum"] = givh["refnum"].ToString().Trim();
                    //            dr2["dqty"] = dr2["qty"];
                    //        }
                    //        #endregion
                    //    }
                    //}

                    //AutoCalc();
                    //    #endregion
					break;
                case "givh_apnum":
                    e.CurrentRow["contact"] = e.F2CurrentRow["ptc"];
                    //e.CurrentRow["accnum1"] = BizLogicTools.Tools.GetApnumAccnum(givh["apnum"].ToString().Trim(), this.dbaccess);
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
                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    //AutoCalc();
                    break;
            }
        }
        //San_End

		#endregion

		#region Document Event

		#region Form Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad (sender, e);
			opened = true;
			documentKey = e.DocumentKey;
			
			DataRow givh = e.DBAccess.DataSet.Tables["givh"].Rows[0];
            DataTable giv1 = e.DBAccess.DataSet.Tables["giv1"];
			string headerFormName = (e.FormsCollection["header"] as Form).Name;
			string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;

            
				
			if (givh["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "givh_trandate", givh);
			}

			givh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			this.dbaccess = e.DBAccess;

			getIncomeExpense = BizXmlReader.CurrentInstance.GetControl(ccDistFormName, "btn_getIncomeExpense") as Button;
			getIncomeExpense.Click += new EventHandler(getIncomeExpense_Click);

            btn_Extract_Mat = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Extract_Mat") as Button;
            btn_Extract_Mat.Click += new EventHandler(btn_Extract_Mat_Click);

            btn_ExtractGRN = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_ExtractGRN") as Button;
            btn_ExtractGRN.Click += new EventHandler(btn_ExtractGRN_Click);

			e.DBAccess.DataSet.Tables["givh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PIVH_ColumnChanged);
			e.DBAccess.DataSet.Tables["giv1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PIV1_ColumnChanged);
		}

        void btn_ExtractGRN_Click(object sender, EventArgs e)
        {
            DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];
            DataTable giv1 = dbaccess.DataSet.Tables["giv1"];

            if (givh["apnum"].ToString() != string.Empty)
            {
                #region Import pon1 to giv1
          
                #endregion
                Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
                DataTable oriTable = giv1;

                try
                {


                    ATL.PIV.ExtractGRN ExtractGRN = new ATL.PIV.ExtractGRN(this.dbaccess, oriTable);
                    ExtractGRN.ShowDialog();
                    ExtractGRN.Focus();

                    #region assign line number for giv1
                    int line = 100;
                    foreach (DataRow dr in giv1.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            dr["line"] = line;
                            line += 100;
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Please selet supplier No. first");
                return;
            }
        }

        void btn_Extract_Mat_Click(object sender, EventArgs e)
        {
            DataRow givh = this.dbaccess.DataSet.Tables["givh"].Rows[0];
            DataTable giv1 = this.dbaccess.DataSet.Tables["giv1"];

            if (!BizFunctions.IsEmpty(givh["dtrandateFrom"]) && !BizFunctions.IsEmpty(givh["dtrandateTo"]))
            {
                Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
                DataTable oriTable = giv1;
                try
                {
                    ATL.PIV.ExtractMIVitems2 ExtractMIV = new ExtractMIVitems2(this.dbaccess, oriTable);
                    ExtractMIV.ShowDialog(frm);
 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Can't Extract if Trandate From / To is Empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

		void getIncomeExpense_Click(object sender, EventArgs e)
		{
			DataTable givc = dbaccess.DataSet.Tables["givc"];
			DataTable getIE = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,sum(oriamt) AS oriamt,oricur,exrate FROM [giv1] GROUP BY accnum,oricur,exrate");

			DialogResult result = MessageBox.Show("The entries in this page will be reset! Continue?", "Clear Data?", MessageBoxButtons.YesNo);

			if (result == DialogResult.Yes)
			{
				BizFunctions.DeleteAllRows(givc);

				foreach (DataRow dr in getIE.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						DataRow addPivc = givc.Rows.Add(new object[] { });
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
			DataRow givh = this.dbaccess.DataSet.Tables["givh"].Rows[0];

			if (givh["flag"].ToString().Trim() != "GIV")
			{
				e.Handle = false;
			}
            else if (BizValidate.ChkPeriodLocked(e.DBAccess, givh["period"].ToString()))
            {
                MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
            }                  
			else
			{
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM APD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + givh["refnum"].ToString().Trim() + "'");
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + givh["refnum"].ToString().Trim() + "'");
			}
		}

		#endregion

		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);
			DataRow givh = e.DBAccess.DataSet.Tables["givh"].Rows[0];
			DataTable giv1 = e.DBAccess.DataSet.Tables["giv1"];
			DataTable apd = e.DBAccess.DataSet.Tables["apd"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable gld = e.DBAccess.DataSet.Tables["gld"];
			setDefaults(dbaccess.DataSet, "givh/giv1");


            string getMatmTemp1 = "select matnum,expacc from matm";

            this.dbaccess.ReadSQL("getMatmTemp1", getMatmTemp1);


            if (giv1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in giv1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["accnum"] = GetExpacc(dr1["matnum"].ToString());
                    }
                }
            }

			//if (givh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSN)
			//{
			//    if (givh["docunum"].ToString().Trim() == String.Empty || givh["docunum"] == System.DBNull.Value)
			//        givh["docunum"] = givh["refnum"];
			//}

            givh["invnum"] = givh["supinvnum"];



            //if (givh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    if (!BizFunctions.IsEmpty(givh["invdate"]))
            //    {
            //        givh["invdate"] = givh["trandate"];
            //    }
            //}
            //if (BizFunctions.IsEmpty(givh["invdate"]))
            //{
            //    givh["invdate"] = givh["trandate"];
            //}

            if (!BizFunctions.IsEmpty(givh["detail"]) && BizFunctions.IsEmpty(givh["remark"]))
            {
                givh["remark"] = givh["detail"];
            }

            AutoCalc();
			
			MDTReader.SetCorrectValue(ref this.dbaccess, "GIV1", "GIV");
		}

		#endregion
       
		 #region TabControl Handle

		protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_TabControl_Handle(sender, e);

			TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
		}

		#endregion


		#region Steph - Extraction Time!!
		protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Extract_Handle(sender, e);
			DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];
			#region Steph - MDT Extraction

			Hashtable HsExtract = MDTReader.GetExtraction("giv", "GRN-PIV Extract", TabDetail, this.dbaccess);

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
		#endregion

		#region SaveBegin
		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow givh = e.DBAccess.DataSet.Tables["givh"].Rows[0];
            DataTable giv1 = e.DBAccess.DataSet.Tables["giv1"];
            DataTable apd = e.DBAccess.DataSet.Tables["apd"];
            DataTable csh = e.DBAccess.DataSet.Tables["csh"];
            DataTable gld = e.DBAccess.DataSet.Tables["gld"];

            if (!BizFunctions.IsEmpty(givh["trandate"]))
            {
                givh["invdate"] = givh["trandate"];
            }

            foreach (DataRow dr1 in giv1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    dr1["invdate"] = givh["invdate"];
                }
            }


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
            DataRow givh = e.DBAccess.DataSet.Tables["givh"].Rows[0];
            Hashtable selectedCollection = new Hashtable();
			
			switch (e.ReportName)
            {
				case "Purchase Invoice":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("apm", "SELECT top 1 * FROM apm where apnum='" + givh["apnum"].ToString() + "'");
					selectedCollection.Add("matm", "SELECT * FROM matm");
					selectedCollection.Add("hemph", "SELECT * FROM hemph where empnum = '" + givh["salesman"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;                   
                    break;				                                
            }            
		}

		#endregion

		#endregion
	
		#region ColumnChangedEvents

		#region givh

        private void Voucher_PIVH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
			DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];
			DataTable giv1 = dbaccess.DataSet.Tables["giv1"];

            switch (e.Column.ColumnName)
            {
				case "apnum":
					#region Steph - Pull info from APM
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
					string exrStr = "Select  * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
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

					givh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(givh["trandate"]));

					e.Row.EndEdit();
					break;

					#endregion				
				case "grnum":
					break;
					
			}
        }            
		#endregion

		#region giv1 - Column Changed	
		private void Voucher_PIV1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{
				case "matnum":
					#region Steph - Pull Info from MATM
                    dbaccess.ReadSQL("getMatm", "SELECT matname, puom umocode,purcAcc,expacc FROM matm where matnum ='" + e.Row["matnum"].ToString().Trim() + "'");
					if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
					{
						if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
							e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
                        //if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
                        //    e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
						if (e.Row["accnum"].ToString().Trim() == "" || e.Row["accnum"] == System.DBNull.Value)
                            e.Row["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["expacc"];
					}
					break;

					#endregion		

                //case "ccnum":
                //    if (e.Row["ccnum"].ToString().Trim() == "PSC")
                //    {
                      
                //            e.Row["accnum"] = "50002";
                        
                //    }
                //    if (e.Row["ccnum"].ToString().Trim() == "CLN")
                //    {
                     
                //            e.Row["accnum"] = "50001";
                        
                //    }
                //    break;
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

		#region Steph - To set the Auto Calc to be used in various events
		private void AutoCalc()
		{
			DataRow givh = dbaccess.DataSet.Tables["givh"].Rows[0];
			DataTable giv1 = dbaccess.DataSet.Tables["giv1"];

			setDefaults(dbaccess.DataSet, "giv1");

			if (givh["refnum"].ToString().Trim().Contains("PIV"))
			{
				if (givh["docunum"] == System.DBNull.Value || givh["docunum"].ToString().Trim() == String.Empty)
				{
					givh["docunum"] = givh["refnum"];
				}
			}			


            #region san - Check contact textbox
            dbaccess.ReadSQL("getContactInfo", "Select ptc from arm where arnum = '" + givh["apnum"] + "' ");
            if (dbaccess.DataSet.Tables["getContactInfo"].Rows.Count > 0)
            {
                if (givh["contact"].ToString() == String.Empty || givh["contact"] == System.DBNull.Value)
                {
                    givh["contact"] = dbaccess.DataSet.Tables["getContactInfo"].Rows[0]["ptc"];
                }
            }
            #endregion

			#region Steph -  To get pd from pd (nonYear) table.
			
			givh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(givh["trandate"]));

			#endregion

			#region giv1

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum2 FROM apm WHERE apnum = '" + givh["apnum"].ToString().Trim() + "'");

			dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
				" WHERE gstgrpnum ='" + givh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

			if ((bool)givh["inclgst"])
			{
				#region Steph - Inclusive GST calculations
				#region initialise values
				giv1_discamt = 0;
				giv1_oriamt = 0;
				giv1_origstamt = 0;
				giv1_postamt = 0;
				giv1_gstamt = 0;
				giv1_grosamt = 0;
				giv1_roriamt = 0;
				giv1_rpostamt = 0;
				#endregion

				foreach (DataRow dr in giv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(givh, dr);
						BizFunctions.UpdateDataRow(givh, dr, "refnum/apnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

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
							dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(givh["gstper"]) / (100 + Convert.ToDecimal(givh["gstper"])));
						}
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
						dr["dgrosamt"] = dr["doriamt"];

						giv1_discamt += (decimal)dr["discamt"];
						giv1_oriamt += (decimal)dr["doriamt"];
						giv1_origstamt += (decimal)dr["dorigstamt"];
						giv1_postamt += (decimal)dr["dpostamt"];
						giv1_gstamt += (decimal)dr["dgstamt"];
						giv1_grosamt += (decimal)dr["dgrosamt"];
						giv1_roriamt += (decimal)dr["roriamt"];
						giv1_rpostamt += (decimal)dr["rpostamt"];
					}
				}
				#region Steph - Check the gst amt differences and add/deduct from the first entry of giv1
				if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
				{
					//steph  - do not have to calculate the gst, allow user to key in manually.
				}
				else
				{
					if (giv1.Rows.Count > 0)
					{
						decimal headerGst = BizFunctions.Round((giv1_oriamt + giv1_origstamt) * Convert.ToDecimal(givh["gstper"]) / (100 + Convert.ToDecimal(givh["gstper"])));
						if (headerGst != giv1_origstamt)
						{
								giv1.Rows[0]["dorigstamt"] = Convert.ToDecimal(giv1.Rows[0]["dorigstamt"]) + headerGst - giv1_origstamt;
						}
					}

					#region initialise values
					giv1_discamt = 0;
					giv1_oriamt = 0;
					giv1_origstamt = 0;
					giv1_postamt = 0;
					giv1_gstamt = 0;
					giv1_grosamt = 0;
					giv1_roriamt = 0;
					giv1_rpostamt = 0;
					#endregion

					foreach (DataRow dr in giv1.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
							dr["dgrosamt"] = dr["doriamt"];
							dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
							dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

							giv1_discamt += (decimal)dr["discamt"];
							giv1_oriamt += (decimal)dr["doriamt"];
							giv1_origstamt += (decimal)dr["dorigstamt"];
							giv1_postamt += (decimal)dr["dpostamt"];
							giv1_gstamt += (decimal)dr["dgstamt"];
							giv1_grosamt += (decimal)dr["dgrosamt"];
							giv1_roriamt += (decimal)dr["roriamt"];
							giv1_rpostamt += (decimal)dr["rpostamt"];
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
				giv1_grosamt = 0;
				giv1_discamt = 0;
				giv1_oriamt = 0;
				giv1_origstamt = 0;
				giv1_postamt = 0;
				giv1_gstamt = 0;
				giv1_roriamt = 0;
				giv1_rpostamt = 0;
				#endregion

				dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
						" WHERE gstgrpnum ='" + givh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in giv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(givh, dr);
						BizFunctions.UpdateDataRow(givh, dr, "refnum/apnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

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
							dr["dorigstamt"] = BizFunctions.Round((decimal)dr["doriamt"] * ((decimal)dr["gstper"] / 100));
						}
						dr["dpostamt"] = BizFunctions.Round((decimal)dr["doriamt"] * (decimal)dr["exrate"]);
						dr["dgstamt"] = BizFunctions.Round((decimal)dr["dorigstamt"] * (decimal)dr["exrate"]);
						dr["roriamt"] = BizFunctions.Round((decimal)dr["doriamt"] + (decimal)dr["dorigstamt"]);
						dr["rpostamt"] = BizFunctions.Round((decimal)dr["dpostamt"] + (decimal)dr["dgstamt"]);

						giv1_discamt += (decimal)dr["discamt"];
						giv1_oriamt += (decimal)dr["doriamt"];
						giv1_origstamt += (decimal)dr["dorigstamt"];
						giv1_postamt += (decimal)dr["dpostamt"];
						giv1_gstamt += (decimal)dr["dgstamt"];
						giv1_grosamt += (decimal)dr["dgrosamt"];
						giv1_roriamt += (decimal)dr["roriamt"];
						giv1_rpostamt += (decimal)dr["rpostamt"];
					}
				}

				#endregion
			}

			givh["discamt"] = giv1_discamt;
			givh["grosamt"] = giv1_grosamt;
			givh["origstamt"] = giv1_origstamt;
			givh["gstamt"] = giv1_gstamt;
			givh["oriamt"] = giv1_oriamt + giv1_origstamt;
			givh["postamt"] = giv1_postamt + giv1_gstamt;
			#endregion
		}
		#endregion


        private string GetExpacc(string matnum)
        {
            string expacc = "";

            string get1 = "Select expacc from getMatmTemp1 where matnum='"+matnum+"'";

            DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, get1);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!BizFunctions.IsEmpty(dt.Rows[0]["expacc"]))
                    {
                        expacc = dt.Rows[0]["expacc"].ToString().Trim();
                    }
                }
            }

            return expacc;
        }

	}
}
