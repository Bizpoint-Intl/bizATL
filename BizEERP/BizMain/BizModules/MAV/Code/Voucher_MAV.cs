/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_MAV.cs
 *	Description:    Stock Adjustment Note
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 *
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

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

namespace ATL.MAV
{
	public class Voucher_MAV : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Global variables

        protected string flag = "";
		protected DBAccess dbaccess = null;
		protected decimal mav1_cosamt = 0;
		protected DialogResult ok = DialogResult.No;
        protected bool opened = false;
		bool reminder = false;

		protected string strcreatedby = null;

        private string formName;
        private string detailFormName = null;

        private Button Btn_Insert;
        private Button Btn_Delete;
        private Button Btn_Mark;

		#endregion

		public Voucher_MAV(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_MAV.xml", moduleName, voucherBaseHelpers)
		{
            this.flag = moduleName;
		}

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "mavh.flag='" + flag + "' AND mavh.SystemYear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (mavh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " mavh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " mavh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND mavh.flag='" + flag + "' AND mavh.SystemYear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

		#region DocumentPage Event

		protected override void AddDocumentPageEventTarget(object sender, PageEventArgs e)
		{
			base.AddDocumentPageEventTarget (sender, e);
			switch(e.PageName)
			{
				case "header":
					e.EventTarget = new Header_MAV(e.DBAccess, e.FormsCollection, e.DocumentKey);
					break;
			}
		}

		#endregion

        #region Voucher New/Edit

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
        #endregion

        #region Document Cancel Click
        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);

            opened = false;
        }
        #endregion

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);

            opened = true;
            this.formName = (sender as Form).Name;
            this.detailFormName = (e.FormsCollection["header"] as Form).Name;


            DataTable MAVH = e.DBAccess.DataSet.Tables["mavh"];
            DataRow mavh = e.DBAccess.DataSet.Tables["mavh"].Rows[0];
            DataTable mav1 = e.DBAccess.DataSet.Tables["mav1"];
            setDefaults(e.DBAccess.DataSet, "MAVH/mav1");

            string headerFormName = (e.FormsCollection["header"] as Form).Name;
            if (mavh["status"].ToString() == "N")
            {
                BizFunctions.GetTrandate(headerFormName, "mavh_trandate", mavh);
            }

            this.dbaccess = e.DBAccess;

            e.DBAccess.DataSet.Tables["mavh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_MAVH_ColumnChanged);
            e.DBAccess.DataSet.Tables["mav1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_MAV1_ColumnChanged);

            Btn_Insert = BizXmlReader.CurrentInstance.GetControl(this.formName, Common.DEFAULT_DOCUMENT_BTNINSERT) as Button;
            //Btn_Insert.Enabled = true;
            Btn_Delete = BizXmlReader.CurrentInstance.GetControl(this.formName, Common.DEFAULT_DOCUMENT_BTNDELETE) as Button;
            //Btn_Delete.Enabled = true;
            Btn_Mark = BizXmlReader.CurrentInstance.GetControl(this.formName, Common.DEFAULT_DOCUMENT_BTNMARK) as Button;
            //Btn_Mark.Enabled = true;
        }

        #endregion

        #region Document F2/F3

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            DataRow mavh = this.dbaccess.DataSet.Tables["mavh"].Rows[0];

            switch (e.ControlName)
            {
                case "mavh_whnum":
                    if (!BizFunctions.IsEmpty(mavh["sitenum"]))
                    {
                        e.DefaultCondition = " sitenum='" + mavh["sitenum"].ToString() + "' ";
                    }
                    else
                    {
                        e.DefaultCondition = "1=1";
                    }
                    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow mavh = dbaccess.DataSet.Tables["mavh"].Rows[0];
            switch (e.ControlName)
            {
                // later...            
                case "mavh_fromsitenum":
                    {
                        mavh["fromsitenum"] = e.F2CurrentRow["sitenum"];
                        mavh["fromsitename"] = e.F2CurrentRow["sitename"];   
                    }
                    break;

                case "mavh_tositenum":
                    {
                        mavh["tositenum"] = e.F2CurrentRow["sitenum"];
                        mavh["tositename"] = e.F2CurrentRow["sitename"]; 
                    }
                    break;      

            }
        }

        #endregion

        #region Detail F2/F3

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);

            switch (e.MappingName)
            {
                case "matnum":
                    e.Condition = BizFunctions.F2Condition("matnum/matname", (sender as TextBox).Text);
                    break;
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);

            switch (e.MappingName)
            {
                case "matnum":
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    e.CurrentRow["matname"] = e.F2CurrentRow["matname"];
                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    break;
            }
        }

        #endregion

        #region ColumnChangedEvents

        #region mavh

        private void Voucher_MAVH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            DataRow mavh = dbaccess.DataSet.Tables["mavh"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "trandate":
                    #region set period
                    e.Row.BeginEdit();
                    //YushuEdited-23Jul2010-For WAC to group mwt data by period
                    //if ((bool)mavh["opbal"] != true)
                    //if (mavh["mavtype"].ToString() == "Stock Take")
                    //{
                        e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row[e.Column.ColumnName]);
                        e.Row["pd"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row[e.Column.ColumnName]);
                    //}
                    //else if (mavh["mavtype"].ToString() == "Stock Adjustment")
                    //{
                    //    e.Row["period"] = 0;
                    //    e.Row["pd"] = 0;
                    //}
                    e.Row.EndEdit();
                    break;
                    #endregion

                case "mavtype":
                    #region set period

                    #region Get DataGridView MAV1
                    //Added By Yushu20100331 - Open Up different columns for different mav type selected

                    DataGrid dgMAV1 = BizXmlReader.CurrentInstance.GetControl(detailFormName, "dg_Detail") as DataGrid;

                    foreach (DataGridTableStyle dgts in dgMAV1.TableStyles)
                    {
                        foreach (DataGridColumnStyle dgcs in dgts.GridColumnStyles)
                        {
                            if (dgcs is BizDataGridTextBoxColumn)
                            {
                                BizDataGridTextBoxColumn bizcombo = null;
                                bizcombo = dgcs as BizDataGridTextBoxColumn;

                                #region Stock Adjustment
                               if (mavh["mavtype"].ToString() == "Stock Adjustment")
                                {
                                    //e.Row["period"] = 0;
                                    //e.Row["pd"] = 0;
                                   //YushuEdited-23Jul2010-For WAC to group mwt data by period
                                    e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)mavh["trandate"]);
                                    e.Row["pd"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)mavh["trandate"]);


                                    if (bizcombo.MappingName == "phyqty")
                                    {
                                        bizcombo.TextBoxGrid.Enabled = false;
                                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.LightGray;
                                    }

                                    if (bizcombo.MappingName == "qty")
                                    {
                                        bizcombo.TextBoxGrid.Enabled = true;
                                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.White;
                                    }
                                }
                                #endregion

                                #region Stock Take
                                else if (mavh["mavtype"].ToString() == "Stock Take")
                                {
                                    e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)mavh["trandate"]);
                                    e.Row["pd"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)mavh["trandate"]);

                                    if (bizcombo.MappingName == "qty")
                                    {
                                        bizcombo.TextBoxGrid.Enabled = false;
                                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.LightGray;
                                    }

                                    if (bizcombo.MappingName == "phyqty")
                                    {
                                        bizcombo.TextBoxGrid.Enabled = true;
                                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.White;
                                    }

                                }
                                #endregion
                            }
                        }
                    }

                    #endregion

                    break;

                    #endregion

                #region opbal-Commented
                //case "opbal":
                //    #region set period

                //    if ((bool)mavh["opbal"] != true)
                //    {
                //        e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)mavh["trandate"]);
                //    }
                //    else
                //    {
                //        e.Row["period"] = 0;
                //    }
                //    break;
                //    #endregion
                #endregion
            }
        }

        #endregion

        #region mav1

        private void Voucher_MAV1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow mavh = this.dbaccess.DataSet.Tables["mavh"].Rows[0];
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    #region uom

                    if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                    {
                        string sql = "Select matname,uomcode,barcode from matm where matnum='" + e.Row["matnum"].ToString() + "' and isnull(status,'')<>'V'";
                        this.dbaccess.ReadSQL("tmpMATM", sql);
                        DataTable tmpMATM = this.dbaccess.DataSet.Tables["tmpMATM"];
                        foreach (DataRow drMATM in tmpMATM.Rows)
                        {
                            if (drMATM.RowState != DataRowState.Deleted)
                            {
                                e.Row["matname"] = drMATM["matname"];
                                e.Row["uom"] = drMATM["uomcode"];
                                e.Row["barcode"] = drMATM["barcode"];
                            }
                        }
                    }

                    #endregion
                    break;
            }
        }

        #endregion

        #endregion

        #region Refresh

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

            #region initialise values

            // Initialise the accounting defaults.
            mav1_cosamt = 0;
            this.ok = DialogResult.OK;
            DataRow mavh = e.DBAccess.DataSet.Tables["mavh"].Rows[0];
            DataTable MAVH = dbaccess.DataSet.Tables["mavh"];
            DataTable mav1 = e.DBAccess.DataSet.Tables["mav1"];

            #endregion

            #region mavh

            mavh["trandate"] = BizFunctions.GetStandardDateString(Convert.ToDateTime(mavh["trandate"]));
            //mavh["stkdate"] = BizFunctions.GetStandardDateString(Convert.ToDateTime(mavh["stkdate"]));
            if (BizFunctions.IsEmpty(mavh["stkdate"]))
            {
                mavh["stkdate"] = mavh["trandate"];
            }
            setDefaults(dbaccess.DataSet, "MAVH/mav1");

            #endregion

            if (mavh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP) return;

            #region mav1


     

            foreach (DataRow drMAV1 in mav1.Rows)
            {
                if (drMAV1.RowState != DataRowState.Deleted)
                {
                    drMAV1["phyqty"] = drMAV1["bkqty"];
                    BizFunctions.UpdateDataRow(mavh, drMAV1);
                    //#region Update matname & uom if empty
                    //if (BizFunctions.IsEmpty(drMAV1["matname"]) || BizFunctions.IsEmpty(drMAV1["uom"]))
                    //{
                    //    if (!BizFunctions.IsEmpty(drMAV1["matnum"]))
                    //    {
                    //        string sqlUOM = "Select matname,uomcode from matm where matnum='" + drMAV1["matnum"].ToString() + "' and isnull(status,'')<>'V'";
                    //        this.dbaccess.ReadSQL("tmpMATM", sqlUOM);
                    //        DataTable tmpUOM = this.dbaccess.DataSet.Tables["tmpMATM"];
                    //        foreach (DataRow drUOM in tmpUOM.Rows)
                    //        {
                    //            if (drUOM.RowState != DataRowState.Deleted)
                    //            {
                    //                if (BizFunctions.IsEmpty(drMAV1["matname"]))
                    //                {
                    //                    drMAV1["matname"] = drUOM["matname"].ToString();
                    //                }
                    //                if (BizFunctions.IsEmpty(drMAV1["uom"]))
                    //                {
                    //                    drMAV1["uom"] = drUOM["uomcode"].ToString();
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //#endregion
                }
            }

            #endregion

            //#region UpdateDataRow mav1

            //foreach (DataRow dr in mav1.Rows)
            //{
            //    if (dr.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(mavh, dr);
            //    }
            //}

            //#endregion

            Btn_Insert.Enabled = true;
            Btn_Delete.Enabled = true;
            Btn_Mark.Enabled = true;
        }

        #endregion

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);

			DataRow mavh = e.DBAccess.DataSet.Tables["mavh"].Rows[0];

            mavh.BeginEdit();
            mavh["SystemYear"] = Common.DEFAULT_SYSTEM_YEAR.ToString();
            mavh["period"] = BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(mavh["trandate"]));
            mavh["pd"] = BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(mavh["trandate"]));

            if (mavh["flag"].ToString().Trim() == "LMAV")
            {

                mavh["sitenum"] = mavh["fromsitenum"];
                mavh["whnum"] = getWhnum(mavh["fromsitenum"].ToString());

                if (BizFunctions.IsEmpty(mavh["whnumfrm"]))
                {
                    if (!BizFunctions.IsEmpty(mavh["fromsitenum"]))
                    {
                        mavh["whnumfrm"] = getWhnum(mavh["fromsitenum"].ToString());
                    }
                }
                if (BizFunctions.IsEmpty(mavh["whnumto"]))
                {
                    if (!BizFunctions.IsEmpty(mavh["tositenum"]))
                    {
                        mavh["whnumto"] = getWhnum(mavh["tositenum"].ToString());
                    }
                }
            }

			if (mavh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP) return;

			Document_Refresh_OnClick(sender, new DocumentEventArgs(e.PageName, e.DBAccess, e.DocumentKey, e.FormsCollection, e.CurrentUser, e.TableYear, e.CurrentRow));

            if (BizValidate.ChkPeriodLocked(e.DBAccess, mavh["period"].ToString()) || BizValidate.ChkPeriodLocked(e.DBAccess, mavh["pd"].ToString()))
			{
				MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
                return;
			}

            if (BizFunctions.IsEmpty(mavh["sitenum"]))
            {
                MessageBox.Show("Please select Site No. !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }

            if (BizFunctions.IsEmpty(mavh["whnum"]))
            {
                MessageBox.Show("Please select Warehouse !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }
		}

		#endregion

        #region SaveBegin

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            //DataTable dtTmp = null;
            //string sqlCommand = null;
            DataRow mavh = e.DBAccess.DataSet.Tables["mavh"].Rows[0];
            DataTable mav1 = e.DBAccess.DataSet.Tables["mav1"];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];

            if (mavh["flag"].ToString() == "MAV")
            {
                if (mavh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
                {
                    #region stock costing adjustment update MWT

                    foreach (DataRow dr in mav1.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            if ((decimal)dr["qty"] != 0)
                            {
                                DataRow mwt_dr = mwt.Rows.Add(new object[] { });

                                mwt_dr["location"] = mavh["sitenum"].ToString();
                                mwt_dr["whnum"] = mavh["whnum"].ToString();

                                BizFunctions.UpdateDataRow(mavh, mwt_dr);

                                if (mavh["mavtype"].ToString() == "Opening Balance")
                                {
                                    mavh["opbal"] = 1;
                                    BizFunctions.UpdateDataRow(mavh, mwt_dr, "opbal");
                                }
                                else
                                {
                                    mavh["opbal"] = 0;
                                    BizFunctions.UpdateDataRow(mavh, mwt_dr, "opbal");
                                }

                                BizFunctions.UpdateDataRow(dr, mwt_dr, "coy/stkdate");
                                BizFunctions.UpdateDataRow(dr, mwt_dr, "matnum/uom/stdcost/cosamt");

                                mwt_dr["detail"] = dr["matname"].ToString();
                                mwt_dr["year"] = mavh["systemyear"].ToString();
                                mwt_dr["created"] = mavh["created"].ToString();
                                mwt_dr["modified"] = mavh["modified"].ToString();
                                mwt_dr["docunum"] = mavh["refnum"].ToString();
                                mwt_dr["pd"] = mavh["pd"].ToString();
                                mwt_dr["qty"] = Convert.ToInt32(dr["qty"]);
                                mwt_dr["barcode"] = dr["barcode"].ToString();
                                mwt_dr["guid"] = BizLogicTools.Tools.getGUID();
                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {
                if (mavh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
                {
                    foreach (DataRow dr in mav1.Rows)
                    {
                        if (dr["matnum"] != System.DBNull.Value || dr["matnum"].ToString() != String.Empty)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                if ((decimal)dr["qty"] != 0)
                                {

                                    #region assigning rows to local mwt
                                    for (int i = 0; i <= 1; i++)
                                    {
                                        if (i == 0)
                                        {
                                            DataRow dr_mwt = mwt.NewRow();
                                            dr_mwt["refnum"] = dr["refnum"];
                                            dr_mwt["barcode"] = dr["barcode"];
                                            dr_mwt["matnum"] = dr["matnum"];
                                            dr_mwt["detail"] = dr["detail"];
                                            //dr_mwt["pcatcode"] = dr["pcatcode"];
                                            dr_mwt["docunum"] = dr["refnum"];
                                            dr_mwt["location"] = mavh["fromsitenum"];
                                            dr_mwt["qty"] = -(decimal)dr["qty"];
                                            dr_mwt["uom"] = dr["uom"];
                                            //if (Tools.isFrontEnd())
                                            //{
                                            //dr_mwt["whnum"] = dr["fromsitenum"];//Decrease qty in main warehouse,here is the fromsitenum(local) if is outlets
                                            dr_mwt["whnum"] = mavh["whnumfrm"];//Decrease qty in main warehouse,here is the fromsitenum(local) if is outlets
                                            //}
                                            //else
                                            //{
                                            //    dr_mwt["whnum"] = "MWH";//Decrease qty in main warehouse
                                            //}
                                            dr_mwt["guid"] = BizLogicTools.Tools.getGUID();
                                            dr_mwt["trandate"] = dr["trandate"];
                                            //dr_mwt["year"] = dr["year"];
                                            dr_mwt["created"] = dr["created"];
                                            dr_mwt["modified"] = dr["modified"];
                                            dr_mwt["status"] = dr["status"].ToString().Trim();
                                            dr_mwt["user"] = dr["user"].ToString().Trim();
                                            dr_mwt["barcode"] = dr["barcode"].ToString();
                                            //dr_mwt["remarks"] = dr["remarks"].ToString().Trim();
                                            dr_mwt["flag"] = mavh["flag"];
                                            mwt.Rows.Add(dr_mwt);
                                        }
                                        if (i == 1)
                                        {
                                            DataRow dr_mwt = mwt.NewRow();
                                            dr_mwt["refnum"] = dr["refnum"];
                                            dr_mwt["barcode"] = dr["barcode"];
                                            dr_mwt["matnum"] = dr["matnum"];
                                            dr_mwt["detail"] = dr["detail"];
                                            //dr_mwt["pcatcode"] = dr["pcatcode"];
                                            dr_mwt["docunum"] = dr["refnum"];
                                            dr_mwt["location"] = mavh["tositenum"];
                                            dr_mwt["qty"] = (decimal)dr["qty"];
                                            dr_mwt["uom"] = dr["uom"];
                                            dr_mwt["whnum"] = mavh["whnumto"];//increase qty in Stock In Transit warehouse(Location HQ)
                                            dr_mwt["guid"] = BizLogicTools.Tools.getGUID();
                                            dr_mwt["trandate"] = dr["trandate"];
                                            //dr_mwt["year"] = dr["year"];
                                            dr_mwt["created"] = dr["created"];
                                            dr_mwt["modified"] = dr["modified"];
                                            dr_mwt["status"] = dr["status"].ToString().Trim();
                                            dr_mwt["user"] = dr["user"].ToString().Trim();
                                            dr_mwt["barcode"] = dr["barcode"].ToString();
                                            //dr_mwt["remarks"] = dr["remarks"].ToString().Trim();
                                            dr_mwt["flag"] = mavh["flag"];
                                            mwt.Rows.Add(dr_mwt);
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }

                }
            }
            foreach (DataRow dr in mav1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    dr["detail"] = dr["matname"].ToString();
                    BizFunctions.UpdateDataRow(mavh, dr, "trandate/period/user/flag/status/created/modified/SystemYear/pd");
                }
            }

            
        }

        #endregion

        #region Reopen

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);

			DataRow mavh = dbaccess.DataSet.Tables["mavh"].Rows[0];

            if (BizValidate.ChkPeriodLocked(e.DBAccess, mavh["period"].ToString()) || BizValidate.ChkPeriodLocked(e.DBAccess, mavh["pd"].ToString()))
			{
                MessageBox.Show("You are not allowed to reopen this voucher !", "Period has been Closed !", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
        }

        #endregion

        #region Preview Handle

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);

            DataRow mavh = this.dbaccess.DataSet.Tables["mavh"].Rows[0];

            // If allow print even after confirm by checking the status
            if (mavh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "mavh/mav1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }

        #endregion

        #region Print OnClick

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

            DataRow mavh=this.dbaccess.DataSet.Tables["mavh"].Rows[0];
            Hashtable selectedCollection = new Hashtable();

            selectedCollection.Add("MATM", "SELECT m.matnum,m.matname,m.uomcode FROM MATM m left join MAV1 v on m.matnum=v.matnum where v.refnum='" + mavh["refnum"].ToString() + "'");

            e.DBAccess.ReadSQL(selectedCollection);
            e.DataSource = e.DBAccess.DataSet;
        }

        #endregion

        #region Private Functions

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
								case "System.DateTime":
									if (dr[dc.ColumnName] == System.DBNull.Value)
										dr[dc.ColumnName] = DateTime.Now;
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

        //private void calculatePhyQty()
        //{
        //    DataTable mav1 = this.dbaccess.DataSet.Tables["mav1"];

        //    foreach (DataRow drV1 in mav1.Rows)
        //    {
        //        if (drV1.RowState != DataRowState.Deleted)
        //        {
        //            drV1["phyqty"] = BizFunctions.Round(Convert.ToDecimal(drV1["qty"]) + Convert.ToDecimal(drV1["bkqty"]), 4);
        //        }
        //    }
        //}

        //private void calculateAdjQty()
        //{
        //    DataTable mav1 = this.dbaccess.DataSet.Tables["mav1"];

        //    foreach (DataRow drV1 in mav1.Rows)
        //    {
        //        if (drV1.RowState != DataRowState.Deleted)
        //        {
        //            drV1["qty"] = BizFunctions.Round(Convert.ToDecimal(drV1["phyqty"]) - Convert.ToDecimal(drV1["bkqty"]), 4);
        //        }
        //    }
        //}

        //private void calculateTotalQty()
        //{
        //    DataTable mav1 = this.dbaccess.DataSet.Tables["mav1"];
        //    DataRow mavh=this.dbaccess.DataSet.Tables["mavh"].Rows[0];

        //    //decimal totalbkqty = Convert.ToDecimal(mavh["totalbkqty"]);
        //    //decimal totalphyqty = Convert.ToDecimal(mavh["totalphyqty"]);
        //    //decimal totalqty = Convert.ToDecimal(mavh["totalqty"]);
        //    decimal totalbkqty = 0;
        //    decimal totalphyqty = 0;
        //    decimal totalqty = 0;

        //    foreach (DataRow drV1 in mav1.Rows)
        //    {
        //        if (drV1.RowState != DataRowState.Deleted)
        //        {
        //            if (BizFunctions.IsEmpty(drV1["bkqty"]))
        //            {
        //                drV1["bkqty"] = 0;
        //            }
        //            if (BizFunctions.IsEmpty(drV1["phyqty"]))
        //            {
        //                drV1["phyqty"] = 0;
        //            }
        //            if (BizFunctions.IsEmpty(drV1["qty"]))
        //            {
        //                drV1["qty"] = 0;
        //            }
        //            totalbkqty += Convert.ToDecimal(drV1["bkqty"]);
        //            totalphyqty += Convert.ToDecimal(drV1["phyqty"]);
        //            totalqty += Convert.ToDecimal(drV1["qty"]);
        //        }
        //    }

        //    mavh["totalbkqty"] = totalbkqty;
        //    mavh["totalphyqty"] = totalphyqty;
        //    mavh["totalqty"] = totalqty;
        //}

        #endregion


        private string getWhnum(string sitenum)
        {
            string whnum = "";
            string str = "Select whnum from whm where sitenum='"+sitenum+"'";

            this.dbaccess.ReadSQL("whmTmp", str);

            DataTable dt = this.dbaccess.DataSet.Tables["whmTmp"];

            //DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    whnum = dt.Rows[0]["whnum"].ToString();
                }
                else
                {
                    whnum = sitenum;
                }
            }

            return whnum;
        }
    }
}
