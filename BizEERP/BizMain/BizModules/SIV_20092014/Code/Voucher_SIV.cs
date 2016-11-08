/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_SIV.cs
 *	Description:    Sales Invoice Vouchers
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

using ATL.ExtractTools;



namespace ATL.SIV
{
	public class Voucher_SIV : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables	
		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;
		protected bool opened = false;
		protected decimal siv1_grosamt = 0;
		protected decimal siv1_discamt = 0;
		protected decimal siv1_oriamt = 0;
		protected decimal siv1_origstamt = 0;
		protected decimal siv1_postamt = 0;
		protected decimal siv1_gstamt = 0;
		protected decimal siv1_roriamt = 0;
		protected decimal siv1_rpostamt = 0;
	    protected string detailFormName = null;
		protected bool sivhColumnChange = true;
		protected bool siv1ColumnChange = true;

        protected string headerFormName, detailsFormName = "";
		public string documentKey = null;
		protected int TabDetail;

		protected Button getIncomeExpense;
		protected string strAccNotOverwritten = String.Empty;

		protected Button btnDelete;
		protected Button btnUp;
		protected Button btnDown;
		protected Button btnMark;
		protected Button btnDuplicate;
		protected Button btnExtract;
        protected Button btn_Extract;
        protected CheckBox sivh_autodisc;

		

		#endregion

		public Voucher_SIV(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_SIV.xml", moduleName, voucherBaseHelpers)
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
			base.AddVoucherAllCondition (e);
			e.Condition = "sivh.flag='SIV' AND sivh.systemyear = "+Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (sivh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or "+
				" sivh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO+ "' or "+
				" sivh.status = '" +Common.DEFAULT_DOCUMENT_STATUSE +"')  "+
				" AND sivh.flag='SIV' AND sivh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

		#region Document Handle

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
			base.Document_Form_OnLoad (sender, e);
			opened = true;
			documentKey = e.DocumentKey;
			
			DataRow sivh = e.DBAccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
			headerFormName = (e.FormsCollection["header"] as Form).Name;
      
            //string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;

			this.headerFormName = (e.FormsCollection["header"] as Form).Name;

            btn_Extract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Extract") as Button;

            sivh_autodisc = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sivh_autodisc") as CheckBox;

            btn_Extract.Click +=new EventHandler(btn_Extract_Click);
				
			if (sivh["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "sivh_trandate", sivh);
			}

			sivh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			this.dbaccess = e.DBAccess;
            // Initialise event handlers for button clicked in detail page.

            //getIncomeExpense = BizXmlReader.CurrentInstance.GetControl(ccDistFormName, "btn_getIncomeExpense") as Button;
            //getIncomeExpense.Click += new EventHandler(getIncomeExpense_Click);

			#region Steph - To show the decimal amount with thousand separator
			DataTable SIVH = e.DBAccess.DataSet.Tables["sivh"];
			setDefaults(e.DBAccess.DataSet, "SIVH");

			ReBindsTextBox(headerFormName, "sivh_exrate", e.DBAccess.DataSet.Tables["sivh"], "exrate", DecimalToCurrencyString);
			ReBindsTextBox(headerFormName, "sivh_grosamt", e.DBAccess.DataSet.Tables["sivh"], "grosamt", DecimalToCurrencyString);
			ReBindsTextBox(headerFormName, "sivh_discamt", e.DBAccess.DataSet.Tables["sivh"], "discamt", DecimalToCurrencyString);
			ReBindsTextBox(headerFormName, "sivh_origstamt", e.DBAccess.DataSet.Tables["sivh"], "origstamt", DecimalToCurrencyString);
			ReBindsTextBox(headerFormName, "sivh_oriamt", e.DBAccess.DataSet.Tables["sivh"], "oriamt", DecimalToCurrencyString);
			#endregion
			
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


		}




        #region Extract
        private void btn_Extract_Click(object sender, EventArgs e)
        {
            DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

            Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            DataTable oriTable = siv1;
            try
            {
                // Open Extract Form
                //ExtractTRQForm ExtractTRQ = new ExtractTRQForm(this.dbaccess, oriTable);
                //ExtractTRQ.ShowDialog(frm);

                ExtractCAForm ExtractCA = new ExtractCAForm(this.dbaccess, oriTable);
                ExtractCA.ShowDialog(frm);

                #region assign line number for tra1
                int line = 100;
                foreach (DataRow dr in siv1.Rows)
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
        #endregion

       
		void getIncomeExpense_Click(object sender, EventArgs e)
		{
			DataTable sivc = dbaccess.DataSet.Tables["sivc"];
			DataTable getIE =  BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum,sum(oriamt) AS oriamt,oricur,exrate FROM [siv1] GROUP BY accnum,oricur,exrate");

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

			#region Steph - Do not allow reopen if voucher has been created for this Sales Invoice - Ex: Sales Receipt
			dbaccess.ReadSQL("checkArdReopen", "SELECT refnum FROM ard" + Common.DEFAULT_SYSTEM_YEAR + " WHERE invnum = '" + sivh["refnum"].ToString().Trim() + "' AND refnum <>'" + sivh["refnum"].ToString().Trim() + "' AND flag<>'SIV'");
			DataTable checkArdReopen = dbaccess.DataSet.Tables["checkArdReopen"];
			string RefList = "Please check entries below which has been created for this Sales Invoice:";
			foreach (DataRow dr in checkArdReopen.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					RefList = RefList + "\n " + dr["refnum"].ToString().Trim();
				}
			}

			if (RefList != "Please check entries below which has been created for this Sales Invoice:")
			{
				MessageBox.Show(RefList, "Reopen Unsuccessful");
				e.Handle = false;
			}
			#endregion

			else if (sivh["flag"].ToString().Trim() != "SIV")
			{
				e.Handle = false;
			}
			else
			{
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM ARD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");
			}

            e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM ARD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");
            e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");

		}

		#endregion


		protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
		{
			base.AddDocumentF2Condition(sender, e);

			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];

			switch (e.ControlName)
			{
				case "sivh_donum":
					e.Condition = BizFunctions.F2Condition("dorh.refnum", (sender as TextBox).Text);
					//e.DefaultCondition = "dorh.status = 'P' and dorh.arnum = '" + sivh["arnum"].ToString().Trim() + "' " +
					//" AND dorh.refnum IN " +
					//" (SELECT refnum FROM " +
					//" (SELECT ISNULL(refnum,'') AS refnum,matnum,sum(qty) AS qty FROM " +
					//" (SELECT refnum,matnum,qty FROM dor1 " +
					//" UNION ALL " +
					//" SELECT donum as refnum,matnum,qty FROM siv1" +
					//" ) DorVsSiv " +
					//" GROUP BY refnum,matnum  " +
					//" HAVING SUM(qty) > 0" +
					//" )" +
					//" result)";
					e.DefaultCondition = "dorh.status = 'P' and dorh.arnum = '" + sivh["arnum"].ToString().Trim() + "' " +
						" AND dorh.refnum IN " +
						" (SELECT refnum FROM " +
						" (SELECT ISNULL(refnum,'') AS refnum,matnum,sum(qty) AS qty FROM " +
						" (SELECT dor1.refnum,dor1.matnum,dor1.qty FROM dor1 " +
						" LEFT OUTER JOIN dorh ON dorh.refnum = dor1.refnum " +
						" WHERE dorh.status<>'V' "+
						" UNION ALL " +
						" SELECT siv1.donum as refnum,siv1.matnum,siv1.qty FROM siv1 " +
						" LEFT OUTER JOIN sivh ON sivh.refnum = siv1.refnum "+
						" WHERE sivh.status<>'V' " +
						" ) DorVsSiv " +
						" GROUP BY refnum,matnum  " +
						" HAVING SUM(qty) > 0" +
						" )" +
						" result)";
					break;
				case "sivh_mcrnum":
					e.Condition = BizFunctions.F2Condition("mcrh.refnum", (sender as TextBox).Text);
					e.DefaultCondition = "mcrh.status = 'P' AND mcrh.arnum = '" + sivh["arnum"].ToString().Trim() + "' " +
						" AND (ISNULL(mcrh.refnum,'')) NOT IN (SELECT ISNULL(refnum,'') from sivh WHERE status<>'P')";
					break;

                case "sivh_ctrnum":
                    e.Condition = "ctrh.arnum='" + sivh["arnum"].ToString() + "'";
                    break;

                case "sivh_adhnum":
                    e.Condition = "adh.arnum='" + sivh["arnum"].ToString() + "'";
                    break;

             
			}
		}

		#region F3 

		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);

			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

            switch (e.ControlName)
            {
                case "sivh_donum":
                    #region Extraction from dorh to sivh
                    string donum = sivh["donum"].ToString().Trim();
                    if (!donum.Equals(""))
                    {
                        //string selectDorh = "SELECT * FROM dorh "+
                        //        " WHERE refnum in "+
                        //        " (select donum from "+
                        //        " (SELECT donum,matnum,uom,discamt,sum(qty) as qty,price,detail from " +
                        //        "(SELECT refnum as donum, matnum,uom,discamt, qty,price,matname as detail from dor1 UNION ALL" +
                        //        " SELECT donum, matnum,uom,discamt,qty,price,detail from siv1 "+
                        //        " WHERE refnum <>'" + sivh["refnum"].ToString().Trim() + "')a" +
                        //        "  GROUP BY donum,matnum,price,uom,discamt,detail" +
                        //        " HAVING SUM(qty) >0)frmSelectDetailBelow) AND refnum = '"+sivh["donum"].ToString().Trim()+"'";

                        string selectDorh = "SELECT * FROM dorh " +
                                " WHERE status<>'V' AND refnum in " +
                                " (select donum from " +
                                " (SELECT donum,matnum,uom,discamt,sum(qty) as qty,price,detail from " +
                                "(SELECT dor1.refnum as donum, dor1.matnum,dor1.uom,dor1.discamt, dor1.qty,dor1.price,dor1.matname AS detail FROM dor1 " +
                                " LEFT OUTER JOIN dorh ON dorh.refnum = dor1.refnum " +
                                " WHERE dorh.status <>'V' " +
                                " UNION ALL" +
                                " SELECT siv1.donum, siv1.matnum,siv1.uom,siv1.discamt,siv1.qty,siv1.price,siv1.detail FROM siv1 " +
                                " LEFT OUTER JOIN sivh ON sivh.refnum = siv1.refnum " +
                                " WHERE sivh.status<>'V' " +
                                " AND sivh.refnum <>'" + sivh["refnum"].ToString().Trim() + "')a" +
                                "  GROUP BY donum,matnum,price,uom,discamt,detail" +
                                " HAVING SUM(qty) >0)frmSelectDetailBelow) AND refnum = '" + sivh["donum"].ToString().Trim() + "'";



                        this.dbaccess.ReadSQL("dorhTmp", selectDorh);
                        DataTable dorhTmp = this.dbaccess.DataSet.Tables["dorhTmp"];

                        if (dorhTmp.Select().Length > 0)
                        {
                            sivh["arnum"] = dorhTmp.Rows[0]["arnum"];
                            sivh["custpo"] = dorhTmp.Rows[0]["custpo"];
                            sivh["oricur"] = dorhTmp.Rows[0]["oricur"];
                            sivh["remark"] = dorhTmp.Rows[0]["remark"];
                            sivh["gstgrpnum"] = dorhTmp.Rows[0]["gstgrpnum"];
                            sivh["inclgst"] = dorhTmp.Rows[0]["inclgst"];

                            //update shipping address base on dorh
                            sivh["saddr1"] = dorhTmp.Rows[0]["saddr1"];
                            sivh["saddr2"] = dorhTmp.Rows[0]["saddr2"];
                            sivh["saddr3"] = dorhTmp.Rows[0]["saddr3"];
                            sivh["saddr4"] = dorhTmp.Rows[0]["saddr4"];

                        }

                        else
                        {
                            sivh["arnum"] = "";
                            sivh["custpo"] = "";
                            sivh["oricur"] = "";
                            sivh["remark"] = "";
                            sivh["gstgrpnum"] = "";
                            sivh["inclgst"] = 0;
                            sivh["saddr1"] ="";
                            sivh["saddr2"] ="";
                            sivh["saddr3"] = "";
                            sivh["saddr4"] = "";
                        }
                    #endregion
                  
                    #region Steph - Copy the detail page

                        BizFunctions.DeleteAllRows(siv1);

                        if (dorhTmp.Rows.Count != 0)
                        {
                            #region Steph - Import dor1 to siv1

                            //string selectDor1 = "SELECT donum,matnum,uom,discamt,sum(qty) as qty,price,detail from " +
                            //    "(SELECT refnum as donum, matnum,uom,discamt,qty,price,matname as detail from dor1 UNION ALL" +
                            //    " SELECT donum, matnum,uom,discamt,qty,price,detail from siv1 "+
                            //    " WHERE refnum <>'" + sivh["refnum"].ToString().Trim() + "')a " +
                            //    " WHERE donum = '"+sivh["donum"].ToString().Trim()+"'" +
                            //    " GROUP BY donum,matnum,price,uom,discamt,detail" +
                            //    " HAVING SUM(qty) >0 ";

                            //string selectDor1 = "SELECT donum,matnum,uom,discamt,sum(qty) as qty,price,detail from " +
                            //        "(SELECT dor1.refnum as donum, dor1.matnum,dor1.uom,dor1.discamt, dor1.qty,dor1.price,dor1.matname AS detail FROM dor1 " +
                            //        " LEFT OUTER JOIN dorh ON dorh.refnum = dor1.refnum " +
                            //        " WHERE dorh.status <>'V' " +
                            //        " UNION ALL" +
                            //        " SELECT siv1.donum, siv1.matnum,siv1.uom,siv1.discamt,siv1.qty,siv1.price,siv1.detail FROM siv1 " +
                            //        " LEFT OUTER JOIN sivh ON sivh.refnum = siv1.refnum " +
                            //        " WHERE sivh.status<>'V' " +
                            //        " AND sivh.refnum <>'" + sivh["refnum"].ToString().Trim() + "')a" +
                            //        " WHERE donum = '" + sivh["donum"].ToString().Trim() + "' "+
                            //        "  GROUP BY donum,matnum,price,uom,discamt,detail" +
                            //        " HAVING SUM(qty) >0";

                            string selectDor1 = " select * from(select dor1.refnum as donum,dor1.matnum,dor1.uom,dor1.discamt,dor1.detail,dor1.price," +
                                                     "case when (ISNULL(dor1.qty,0) + ISNULL(siv1.qty,0))< 0 then 0 else (ISNULL(dor1.qty,0) + ISNULL(siv1.qty,0)) end as qty,line " +
                                                     "from (select refnum,matnum,uom,matname as detail,discamt,sum(qty) as qty,price,line from dor1 " +
                                                     "where status<>'V' and refnum = '" + sivh["donum"].ToString().Trim() + "' " +
                                                     "group by refnum,matnum,uom,matname,discamt,price,line)dor1 " +
                                                     "left join (select donum,matnum,sum(qty) as qty,price from siv1 where isnull(status,'')<>'V' AND refnum <>'" + sivh["refnum"].ToString().Trim() + "' " +
                                                     "group by donum,matnum,price)siv1 " +
                                                     "on dor1.refnum=siv1.donum and dor1.matnum= siv1.matnum and dor1.price=siv1.price " +
                                                     "where (dor1.qty+ISNULL(siv1.qty,0)) > 0)tmp1 order by line";

                            this.dbaccess.ReadSQL("dor1Tmp", selectDor1);
                            DataTable dor1Tmp = this.dbaccess.DataSet.Tables["dor1Tmp"];

                            BizFunctions.DeleteAllRows(siv1);
                            foreach (DataRow dr in dor1Tmp.Select())
                            {
                                if (dr.RowState != DataRowState.Deleted)
                                {
                                    dr.SetAdded();
                                    siv1.ImportRow(dr);
                                }
                            }

                            decimal lineNo = 0;
                            foreach (DataRow dr2 in siv1.Select())
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    lineNo = lineNo + 100;
                                    dr2["line"] = lineNo;
                                    dr2["refnum"] = sivh["refnum"].ToString().Trim();
                                    #region Steph - DOR is not using the dqty
                                    dr2["dqty"] = (decimal)dr2["qty"];
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }

                    AutoCalc();
                        #endregion
                    break;
                case "sivh_arnum":
                    e.CurrentRow["contact"] = e.F2CurrentRow["bptc"];


                    if (BizFunctions.IsEmpty(e.CurrentRow["addr1"]))
                    {
                        e.CurrentRow["addr1"] = e.F2CurrentRow["baddr1"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["addr2"]))
                    {
                        e.CurrentRow["addr2"] = e.F2CurrentRow["baddr2"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["addr3"]))
                    {
                        e.CurrentRow["addr3"] = e.F2CurrentRow["baddr3"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["addr4"]))
                    {
                        e.CurrentRow["addr4"] = e.F2CurrentRow["baddr4"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["phone"]))
                    {
                        e.CurrentRow["phone"] = e.F2CurrentRow["phone"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["hp"]))
                    {
                        e.CurrentRow["hp"] = e.F2CurrentRow["hp"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["fax"]))
                    {
                        e.CurrentRow["fax"] = e.F2CurrentRow["fax"].ToString();
                    }

                    //////
                    if (BizFunctions.IsEmpty(e.CurrentRow["saddr1"]))
                    {
                        e.CurrentRow["saddr1"] = e.F2CurrentRow["baddr1"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["saddr2"]))
                    {
                        e.CurrentRow["saddr2"] = e.F2CurrentRow["baddr2"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["saddr3"]))
                    {
                        e.CurrentRow["saddr3"] = e.F2CurrentRow["baddr3"].ToString();
                    }
                    if (BizFunctions.IsEmpty(e.CurrentRow["saddr4"]))
                    {
                        e.CurrentRow["saddr4"] = e.F2CurrentRow["baddr4"].ToString();
                    }

                    break;
                case "sivh_mcrnum":
                    #region Extraction from mcrh to sivh
                    string mcrnum = sivh["mcrnum"].ToString().Trim();
                    if (!mcrnum.Equals(""))
                    {
                        string selectMcrh = "SELECT * FROM mcrh WHERE status <>'V' AND ISNULL(refnum,'') NOT IN " +
                            " (SELECT ISNULL(refnum,'') FROM SIVH) AND refnum = '" + sivh["mcrnum"].ToString().Trim() + "'";
                        this.dbaccess.ReadSQL("mcrhTmp", selectMcrh);
                        DataTable mcrhTmp = this.dbaccess.DataSet.Tables["mcrhTmp"];

                        if (mcrhTmp.Select().Length > 0)
                        {
                            sivh["arnum"] = mcrhTmp.Rows[0]["arnum"];
                            sivh["custpo"] = mcrhTmp.Rows[0]["custpo"];
                            sivh["oricur"] = mcrhTmp.Rows[0]["oricur"];
                            sivh["remark"] = mcrhTmp.Rows[0]["remark"];
                            sivh["gstgrpnum"] = mcrhTmp.Rows[0]["gstgrpnum"];
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

                        if (mcrhTmp.Rows.Count != 0)
                        {
                            #region Steph - Import mcr1 to siv1

                            //string selectMcr1 = "SELECT detail, 1 as qty, price FROM mcr1 WHERE ISNULL(refnum,'') NOT IN " +
                            //" (SELECT ISNULL(refnum,'') FROM SIVH) AND refnum = '" + sivh["mcrnum"].ToString().Trim() + "'";

                            string selectMcr1 = "SELECT mcr1.detail, 1 as qty, mcr1.price FROM mcr1 " +
                                " LEFT OUTER JOIN mcrh ON mcrh.refnum = mcr1.refnum " +
                                " WHERE mcrh.status<>'V' ISNULL(mcrh.refnum,'') NOT IN " +
                            " (SELECT ISNULL(refnum,'') FROM SIVH) AND mcrh.refnum = '" + sivh["mcrnum"].ToString().Trim() + "'";


                            this.dbaccess.ReadSQL("mcr1Tmp", selectMcr1);
                            DataTable mcr1Tmp = this.dbaccess.DataSet.Tables["mcr1Tmp"];

                            BizFunctions.DeleteAllRows(siv1);
                            foreach (DataRow dr in mcr1Tmp.Select())
                            {
                                if (dr.RowState != DataRowState.Deleted)
                                {
                                    dr.SetAdded();
                                    siv1.ImportRow(dr);
                                }
                            }
                            foreach (DataRow dr2 in siv1.Select())
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    dr2["refnum"] = sivh["refnum"].ToString().Trim();
                                    #region Steph - MCR is not using the dqty
                                    dr2["dqty"] = (decimal)dr2["qty"];
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }

                    AutoCalc();
                        #endregion
                    break;

                case "sivh_ctrnum":
                    if (!BizFunctions.IsEmpty(sivh["ctrnum"]))
                    {

                         string strExtractCTR1 = "Select * from CTR1 where refnum= '" + sivh["ctrnum"].ToString().Trim() + "' and [status]<>'V' ";
                        this.dbaccess.ReadSQL("ctrTmp", strExtractCTR1);
                        DataTable ctrTmp = this.dbaccess.DataSet.Tables["ctrTmp"] ;
                        //ClearHeader();

                        if (ctrTmp.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(siv1);

                            foreach (DataRow dr1 in ctrTmp.Rows)
                            {
  
                                DataRow InsertSIV1 = siv1.NewRow();
                                InsertSIV1["docunum"] = dr1["refnum"];
                                InsertSIV1["matnum"] = dr1["matnum"];
                                InsertSIV1["arnum"] = sivh["arnum"];
                                InsertSIV1["dqty"] = dr1["officerqty"];
                                InsertSIV1["price"] = dr1["rate"];
                                InsertSIV1["hscode"] = dr1["sectorcode"];
                                InsertSIV1["detail"] = dr1["remark"];
                                InsertSIV1["shiftcode"] = dr1["shiftcode"];


                                siv1.Rows.Add(InsertSIV1);
                                
                            }


                        }

                        ctrTmp.Dispose();
                    }
                    break;

                case "sivh_adhnum":
                    if (!BizFunctions.IsEmpty(sivh["adhnum"]))
                    {

                        string strExtractADH1 = "Select * from ADH1 where refnum= '" + sivh["adhnum"].ToString().Trim() + "' and [status]<>'V' ";
                        this.dbaccess.ReadSQL("adhTmp", strExtractADH1);
                        DataTable adhTmp = this.dbaccess.DataSet.Tables["adhTmp"];
                        //ClearHeader();

                        if (adhTmp.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(siv1);

                            foreach (DataRow dr1 in adhTmp.Rows)
                            {

                                DataRow InsertSIV1 = siv1.NewRow();
                                InsertSIV1["docunum"] = dr1["refnum"];
                                InsertSIV1["matnum"] = dr1["matnum"];
                                InsertSIV1["arnum"] = sivh["arnum"];
                                InsertSIV1["dqty"] = dr1["officerqty"];
                                InsertSIV1["price"] = dr1["rate"];
                                InsertSIV1["hscode"] = dr1["sectorcode"];
                                InsertSIV1["detail"] = dr1["remark"];


                                siv1.Rows.Add(InsertSIV1);

                            }

                            adhTmp.Dispose();
                        }
                    }
                    break;



            }

		}

		#endregion

		protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
		{
			base.AddDetailF3Condition(sender, e);

			switch (e.MappingName)
			{
				case "matnum":
					e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    //e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
					AutoCalc();					
					break;

                case "shiftcode":
                    e.CurrentRow["timein"] = e.F2CurrentRow["timein"];
                    e.CurrentRow["timeout"] = e.F2CurrentRow["timeout"];
                    //e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    AutoCalc();
                    break;
			}
		}


		protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Insert_OnClick(sender, e);
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			btnUp.Enabled = true;
			btnDown.Enabled = true;

			//DataGrid dataGrid_SIV= BizXmlReader.CurrentInstance.GetControl(headerFormName, "dg_SIV1") as DataGrid;

			//DataView test = dataGrid_SIV.DataSource as DataView;

			//MessageBox.Show(dataGrid_SIV.CurrentRowIndex.ToString());

			//e.CurrentRow["line"] = (decimal)test[dataGrid_SIV.CurrentRowIndex - 1]["line"] + 1;
		}

		protected override void Document_Sort_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Sort_Handle(sender, e);

			//DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
		
			//decimal line = 0;

			//foreach (DataRow dr in siv1.Rows)
			//{
			//    if (dr.RowState != DataRowState.Deleted)
			//    {
			//        line = line + 100;
			//        dr["line"] = line;
			//    }
			//}

		}


		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);
			DataRow sivh = e.DBAccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = e.DBAccess.DataSet.Tables["siv1"];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable gld = e.DBAccess.DataSet.Tables["gld"];
			setDefaults(dbaccess.DataSet, "siv1");

			//if (sivh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSN)
			//{
			//    if (sivh["docunum"].ToString().Trim() == String.Empty || sivh["docunum"] == System.DBNull.Value)
			//        sivh["docunum"] = sivh["refnum"];
			//}

			sivh["invnum"] = sivh["refnum"];
			sivh["invdate"] = sivh["trandate"];

			setColumnChange("all", false);

			AutoCalc();

			setColumnChange("all", true);			
			
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
			
			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			#region Steph - MDT Extraction

			Hashtable HsExtract = MDTReader.GetExtraction("siv", "DOR-SIV Extract", TabDetail, this.dbaccess);

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
			
			#region Steph - New Extraction - Testing
			//e.Handle = false;
			
			//DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			
			//if (BizFunctions.IsEmpty(sivh["arnum"]))
			//{
			//    MessageBox.Show("Invalid Customer Code", "Extraction Failed!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//    return;
			//}
		
			//    string strSql = "select refnum,max(arnum) as arnum,matnum,detail,sum(qty) as qty,uom,price, "+
			//        " sum(grosamt) as grosamt,sum(discamt) as discamt,sum(oriamt) as oriamt,max(remark) as remark "+ 
			//        " from  (select refnum,arnum,matnum,detail,qty,uom,price,grosamt,discamt,oriamt,remark "+
			//        " from sor1 union all select sonum as refnum,arnum,matnum,detail,qty,uom,price,grosamt, "+
			//        " -discamt,oriamt,'' as remark from dor1)result "+
			//            " where arnum = '"+sivh["arnum"]+"' group by refnum,matnum,detail,uom,price having sum(qty)>0 ";
				
			//    string[][] searchcolumns = new string[][] 
			//    { 
			//        new string[] { Tool.GetLanguageBindingName("DORNo"), "AAAAAAAAAA"},
			//        new string[] { "DORNo", "ProductCode" ,"ProductName"}
			//    };
				
			//    string[][] copycolumns = new string[][] 
			//    { 
			//        new string[] { "DORNo", "ProductCode","DORAmount","Price","SIVAmount","Amount"},
			//        new string[] { "DORNo", "ProductCode" ,"DORAmount","Price","SIVAmount","Amount"}
			//    };
				
			//    string[][] comparecolumns = new string[][] 
			//    { 
			//        new string[] { "DORNo", "ProductCode"},
			//        new string[] { "DORNo", "ProductCode" }
			//    };
			//    ExtractClass extract = new ExtractClass(e.DBAccess, "SIV", "DetailGridInfo_Extract.xml", "BBBBBBBBBBB,", searchcolumns, copycolumns, comparecolumns, strSql, "SIV1");
			//    extract.Form_load();        
			#endregion
		}
		#endregion

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
					"SIVH", "SIV", "ard", "siv10", "DOR-SIV EXTRACT");

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

			DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];

			foreach (DataRow dr in siv1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(sivh, dr, "user/created/modified/status");
				}
			}
		}
         
		#endregion	

        #region Tab Control

        protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
        {
            base.Document_TabControl_OnSelectionChanged(sender, e);
            btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
            btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
            btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;
            btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
            btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
            btnExtract = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Extract") as Button;

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
				case "Local Invoice - Inclusive GST":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
					selectedCollection.Add("ptm", "SELECT top 1 * FROM ptm where ptnum='" + sivh["payterms"].ToString() + "'");
                    selectedCollection.Add("delarm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
					selectedCollection.Add("matm", "SELECT * FROM matm");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;                   
                    break;

				case "Local Invoice - Exclusive GST":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
					selectedCollection.Add("ptm", "SELECT top 1 * FROM ptm where ptnum='" + sivh["payterms"].ToString() + "'");
                    selectedCollection.Add("delarm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
					selectedCollection.Add("matm", "SELECT * FROM matm");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;

				case "Oversea Invoice":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
					selectedCollection.Add("ptm", "SELECT top 1 * FROM ptm where ptnum='" + sivh["payterms"].ToString() + "'");
                    selectedCollection.Add("delarm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
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
			if (sivhColumnChange)
			{
				switch (e.Column.ColumnName)
				{
					case "arnum":
						setColumnChange("all", false);
						#region Steph - Pull info from ARM
						dbaccess.ReadSQL("getArmInfo", "SELECT arnum,arname,ptc,addr1,addr2,addr3,addr4,phone,hp,fax,ptnum,gstgrpnum,oricur,accnum FROM arm where arnum ='" + e.Row["arnum"].ToString().Trim() + "'");

						if (dbaccess.DataSet.Tables["getArmInfo"].Rows.Count > 0)
						{
							DataRow getArmInfo = dbaccess.DataSet.Tables["getArmInfo"].Rows[0];
							e.Row["detail"] = getArmInfo["arname"];
							//e.Row["contact"] = getArmInfo["ptc"];
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
						}
						else
						{
							e.Row["detail"] = "";
							e.Row["contact"] = "";
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
						setColumnChange("all", true);
						break;
						#endregion
					case "oricur":
						setColumnChange("all", false);
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
						setColumnChange("all", true);
						break;
						#endregion
					case "gstgrpnum":
						setColumnChange("all", false);
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
						setColumnChange("all", true);
						break;

						#endregion
					case "trandate":
						setColumnChange("all", false);
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

						sivh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(sivh["trandate"]));

						e.Row.EndEdit();
						setColumnChange("all", true);
						break;

						#endregion			
				}
			}
        }            
		#endregion

		#region siv1
		private void Voucher_SIV1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{
				//case "matnum":
					//#region Steph - Pull Info from matm
					//dbaccess.ReadSQL("getMatm", "SELECT matname, uom,saleAcc FROM matm where matnum ='" + e.Row["matnum"].ToString().Trim() + "'");
					//if(dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
					//{
					//    if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
					//        e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
					//    if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
					//        e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uom"];
					//    if (e.Row["accnum"].ToString().Trim() == "" || e.Row["accnum"] == System.DBNull.Value)
					//        e.Row["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
					//}
					//break;

					//#endregion		
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

        private void GetMainDiscAmout()
        {
            
            DataRow sivh = this.dbaccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = this.dbaccess.DataSet.Tables["siv1"];

            decimal WholeDiscount = 0;
           

           

            if (siv1.Rows.Count > 0)
            {
                string GetRefnum = "Select Distinct ctrnum from siv1 where ctrnum is not null";
                DataTable TmpCtrnum = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetRefnum);

                if (TmpCtrnum.Rows.Count > 0)
                {



                    foreach (DataRow dr1 in TmpCtrnum.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            if (!BizFunctions.IsEmpty(dr1["ctrnum"]))
                            {
                                decimal discamt = 0;
                                decimal discamtperline = 0;
                                decimal totaldiscamtline = 0;
                                int totalMatnumShifts = 0;

                                string table = "";

                                if (dr1["ctrnum"].ToString().Trim().Substring(0, 3) == "CTR")
                                {
                                    table = "CTRH";
                                }
                                if (dr1["ctrnum"].ToString().Trim().Substring(0, 3) == "ADH")
                                {
                                    table = "ADH";
                                }
                                string GetDisc = "Select refnum, discamt from " + table + " where refnum='" + dr1["ctrnum"].ToString() + "'";



                                this.dbaccess.ReadSQL("tmpCtr", GetDisc);

                                DataTable tmpCtr = this.dbaccess.DataSet.Tables["tmpCtr"];

                                if (tmpCtr.Rows.Count > 0)
                                {
                                    DataRow dr2 = this.dbaccess.DataSet.Tables["tmpCtr"].Rows[0];

                                    if (BizFunctions.IsEmpty(dr2["discamt"]))
                                    {
                                        dr2["discamt"] = 0;
                                    }

                                    //Get whole discount amount each ctrnum
                                    discamt = Convert.ToDecimal(dr2["discamt"]);

                                }

                                string CountLists = "Select refnum,count(*) as Total from  " + dr1["ctrnum"].ToString().Substring(0, 3) + "1" + " where refnum='" + dr1["ctrnum"].ToString() + "' and matnum is not null and shiftcode is not null group by refnum";

                                this.dbaccess.ReadSQL("TotalCount", CountLists);

                                DataTable TotalCount = this.dbaccess.DataSet.Tables["TotalCount"];

                                if (TotalCount.Rows.Count > 0)
                                {
                                    DataRow dr3 = this.dbaccess.DataSet.Tables["TotalCount"].Rows[0];

                                    if (BizFunctions.IsEmpty(dr3["total"]))
                                    {
                                        dr3["total"] = 1;
                                    }

                                    totalMatnumShifts = Convert.ToInt32(dr3["total"]);

                                }

                                if (discamt > 0 && totalMatnumShifts > 0)
                                {

                                    discamtperline = discamt / totalMatnumShifts;
                                }

                                int count = 0;

                                string TEST = dr1["ctrnum"].ToString();
                                foreach (DataRow dr4 in siv1.Rows)
                                {
                                    if (dr4.RowState != DataRowState.Deleted)
                                    {
                                        if (dr4["ctrnum"] == dr1["ctrnum"])
                                        {
                                            count = count + 1;

                                            if (count == totalMatnumShifts)
                                            {
                                                dr4["discamt"] = discamt - totaldiscamtline;
                                            }
                                            else
                                            {
                                                dr4["discamt"] = discamtperline;
                                                totaldiscamtline = totaldiscamtline + discamtperline;
                                            }
                                        }
                                    }
                                }


                            }

                            //WholeDiscount = WholeDiscount + discamt;

                        }
                    }
                }
            }
            //sivh["discamt"] = WholeDiscount;

          
        }




        #region Steph - To set the Auto Calc to be used in various events
        private void AutoCalc()
        {
            setColumnChange("all", false);
            DataRow sivh = dbaccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
            DataTable sivc = dbaccess.DataSet.Tables["sivc"];

            setDefaults(dbaccess.DataSet, "sivh/siv1/sivc");

            if (sivh["refnum"].ToString().Trim().Contains("SIV"))
            {
                if (sivh["docunum"] == System.DBNull.Value || sivh["docunum"].ToString().Trim() == String.Empty)
                {
                    sivh["docunum"] = sivh["refnum"];
                }
            }


            #region Steph - Get ptc from ARM
            dbaccess.ReadSQL("getArmContact", "SELECT ptc FROM arm WHERE arnum ='" + sivh["arnum"].ToString().Trim() + "'");
            if (dbaccess.DataSet.Tables["getArmContact"].Rows.Count > 0)
            {
                if (sivh["contact"].ToString().Trim() == String.Empty || sivh["contact"] == System.DBNull.Value)
                {
                    sivh["contact"] = dbaccess.DataSet.Tables["getArmContact"].Rows[0]["ptc"];
                }
            }
            #endregion

            #region Steph -  To get pd from pd (nonYear) table.
            sivh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(sivh["trandate"]));
            #endregion

            #region siv1

            dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum2 FROM arm WHERE arnum = '" + sivh["arnum"].ToString().Trim() + "'");

            decimal myline = 0;//x
            decimal line = 0;
            decimal lineFocStart = 99999;  //assuming one transaction does not have 99999 records!

            if (BizFunctions.IsEmpty(sivh["autodisc"]) || sivh["status"]==Common.DEFAULT_DOCUMENT_STATUSN)
            {
                sivh["autodisc"] = 1;
            }

            if ((bool)sivh["autodisc"])
            {
                GetMainDiscAmout();
            }

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
                siv1_roriamt = 0;
                siv1_rpostamt = 0;
                myline = 0;
                line = 0;
                #endregion

                dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
                        " WHERE gstgrpnum ='" + sivh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

                //GetMainDiscAmout();

                foreach (DataRow dr in siv1.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
                    {
                        BizFunctions.UpdateDataRow(sivh, dr);
                        BizFunctions.UpdateDataRow(sivh, dr, "refnum/arnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

                        #region Steph - Adding of hscode and country of origin: 101109_1

                        //reading hscode from pcat, instead matm after mh amended matm and pcat_24Nov2009_0939
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

                        #region Steph -  To replace the rest of the lines after the foc line as foc.
                        //if (dr["foc"] != System.DBNull.Value && (bool)dr["foc"] != false)
                        //{
                        //    lineFocStart = Convert.ToDecimal(dr["line"]);
                        //}
                        #endregion

                        //line = line + 100;
                        //dr["line"] = line;

                        siv1_discamt += (decimal)dr["discamt"];
                        siv1_oriamt += (decimal)dr["doriamt"];
                        siv1_origstamt += (decimal)dr["dorigstamt"];
                        siv1_postamt += (decimal)dr["dpostamt"];
                        siv1_gstamt += (decimal)dr["dgstamt"];
                        siv1_grosamt += (decimal)dr["dgrosamt"];
                        siv1_roriamt += (decimal)dr["roriamt"];
                        siv1_rpostamt += (decimal)dr["rpostamt"];

                        #region Steph - Pull Info from MATM
                        dbaccess.ReadSQL("getMatm", "SELECT matname,uomcode,saleAcc FROM matm where matnum ='" + dr["matnum"].ToString().Trim() + "'");
                        if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
                        {
                            if (dr["detail"].ToString().Trim() == "" || dr["detail"] == System.DBNull.Value)
                                dr["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
                            if (dr["accnum"].ToString().Trim() == "" || dr["accnum"] == System.DBNull.Value)
                                dr["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
                            if (dr["uom"].ToString().Trim() == "" || dr["uom"] == System.DBNull.Value)
                                dr["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
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
                    siv1_roriamt = 0;
                    siv1_rpostamt = 0;
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
                #region initialise values
                siv1_grosamt = 0;
                siv1_discamt = 0;
                siv1_oriamt = 0;
                siv1_origstamt = 0;
                siv1_postamt = 0;
                siv1_gstamt = 0;
                siv1_roriamt = 0;
                siv1_rpostamt = 0;
                myline = 0;
                line = 0;
                #endregion

                dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
                        " WHERE gstgrpnum ='" + sivh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

                //GetMainDiscAmout();

                foreach (DataRow dr in siv1.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
                    {
                        BizFunctions.UpdateDataRow(sivh, dr);
                        BizFunctions.UpdateDataRow(sivh, dr, "refnum/arnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

                        #region Steph - Adding of hscode and country of origin: 101109_1
                        //reading hscode from pcat, instead matm after mh amended matm and pcat_24Nov2009_0934
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
                        //dr["dgstamt"] = BizFunctions.Round((decimal)dr["dpostamt"] * ((decimal)sivh["gstper"] / 100));
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

                        #region Steph -  To replace the rest of the lines after the foc line as foc.
                        //if (dr["foc"] != System.DBNull.Value && (bool)dr["foc"] != false)
                        //{
                        //    lineFocStart = Convert.ToDecimal(dr["line"]);
                        //}
                        #endregion

                        //line = line + 100;
                        //dr["line"] = line;

                        siv1_discamt += (decimal)dr["discamt"];
                        siv1_oriamt += (decimal)dr["doriamt"];
                        siv1_origstamt += (decimal)dr["dorigstamt"];
                        siv1_postamt += (decimal)dr["dpostamt"];
                        siv1_gstamt += (decimal)dr["dgstamt"];
                        siv1_grosamt += (decimal)dr["dgrosamt"];
                        siv1_roriamt += (decimal)dr["roriamt"];
                        siv1_rpostamt += (decimal)dr["rpostamt"];

                        #region Steph - Pull Info from MATM
                        dbaccess.ReadSQL("getMatm", "SELECT matname,uomcode,saleAcc FROM matm where matnum ='" + dr["matnum"].ToString().Trim() + "'");
                        if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
                        {
                            if (dr["detail"].ToString().Trim() == "" || dr["detail"] == System.DBNull.Value)
                                dr["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
                            if (dr["accnum"].ToString().Trim() == "" || dr["accnum"] == System.DBNull.Value)
                                dr["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
                            if (dr["uom"].ToString().Trim() == "" || dr["uom"] == System.DBNull.Value)
                                dr["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
                        }
                        #endregion
                    }
                }
                #endregion
            }

            #region Steph - Replacing line with foc if larger than the line users ticked
            //BizFunctions.ExecuteQuery(dbaccess.DataSet, "UPDATE siv1 SET foc = 1 WHERE line>= " + lineFocStart);
            #endregion

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

            setColumnChange("all", true);
            MDTReader.SetCorrectValue(ref this.dbaccess, "SIV1", "SIV");
        }
        #endregion

		#region Column Change Issue
		private void setColumnChange(string type, bool value)
		{
			type = type.ToLower();

			if (type == "header")
			{
				sivhColumnChange = value;
			}
			if (type == "detail")
			{
				siv1ColumnChange = value;
			}
			if (type == "all")
			{
				sivhColumnChange = value;
				siv1ColumnChange = value;
			}

		}
		#endregion

		#region Thousand Separator
		public static void ReBindsTextBox(string formName, string controlName, object datasource, string column, ConvertEventHandler formathandler)
		{
			TextBox txtBox = BizXmlReader.CurrentInstance.GetControl(formName, controlName) as TextBox;
			txtBox.DataBindings.Clear();

			Binding binding = new Binding("Text", datasource, column);
			binding.Format += new ConvertEventHandler(formathandler);

			txtBox.DataBindings.Add(binding);
		}


		private void DecimalToCurrencyString(object sender, ConvertEventArgs cevent)
		{
			/* This method is the Format event handler. Whenever the 
			   control displays a new value, the value is converted from 
			   its native Decimal type to a string. The ToString method 
			   then formats the value as a Currency, by using the 
			   formatting character "c". */

			// The application can only convert to string type. 
			if (cevent.DesiredType != typeof(string)) return;

			cevent.Value = ((decimal)cevent.Value).ToString("#,##0.00########");

			//IF you want to format the display to be some other format instead of currency format, use the one below
			//			cevent.Value = ((decimal)cevent.Value).ToString("#,##0.000");
		}
		#endregion

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
	}
}
