/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_ACM.cs
 *	Description:    Good Receipt Note Module
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		17/04/07			Start 
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
using DEMO.MDT;
using ATL.BizLogicTools;
//Added By Yushu-To use in Barcode Print
using LabelGalleryPlus3WR;

namespace ATL.GRN
{
    public class Voucher_GRN : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region global variables
        protected bool opened = false;
        protected DBAccess dbaccess = null;
        protected string headerFormName = null;
        protected Hashtable formsCollection = null;
        protected string projectPath = null;
        protected string formName = null;
        protected bool save = false;
        protected string previous = null;

        protected decimal grn1_grosamt = 0;
        protected decimal grn1_discamt = 0;
        protected decimal grn1_oriamt = 0;

        protected int TabDetail;

        protected Button btn_Extract = null;
        protected Button btn_SIV = null;
        protected RadioButton rad_SIVN = null;
        protected RadioButton rad_SIVY = null;

        //added by Yushu-For Barcode Printing
        protected Button btn_PrintBarcode = null;
        protected DialogResult result = DialogResult.No;

        string posid = System.Configuration.ConfigurationManager.AppSettings.Get("POSID");
        #endregion

        public Voucher_GRN(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_GRN.xml", moduleName, voucherBaseHelpers)
        {

        }

        protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_TabControl_Handle(sender, e);

            TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
        }

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "grnh.flag='GRN' AND grnh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (grnh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " grnh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " grnh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND grnh.flag='GRN' AND grnh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

        #region Document Handle

        #region Document_Save_Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow grnh = e.DBAccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = e.DBAccess.DataSet.Tables["grn1"];
            DataTable grn2 = e.DBAccess.DataSet.Tables["grn2"];
            # region Check for empty row and empty mat code
            if (grn1.Rows.Count < 1)
            {
                //MessageBox.Show("Details can't be empty", "Bizpoint International");
                //e.Handle = false;
            }
            if (grnh["whnum"].ToString() == string.Empty)
            {
                MessageBox.Show("Warehouse can't be empty", "Bizpoint International");
                e.Handle = false;
            }
            foreach (DataRow dr in grn1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr["matnum"]))
                    {
                        MessageBox.Show("Save Unsuccessful\nProduct Code cannot be empty !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Handle = false;
                        return;
                    }
                }
            }
            #endregion
        }

        #endregion

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);
            DataRow sivh = this.dbaccess.DataSet.Tables["grnh"].Rows[0];

   
            if (sivh["flag"].ToString().Trim() != "GRN")
            {
                e.Handle = false;
            }
            else if (BizValidate.ChkPeriodLocked(e.DBAccess, sivh["period"].ToString()))
            {
                MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
            }
            else
            {
                e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM mwt" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");
                //e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + sivh["refnum"].ToString().Trim() + "'");
            }
        }
        #region Document_SaveBegin_OnClick

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow grnh = e.DBAccess.DataSet.Tables["grnh"].Rows[0];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];
            DataTable grn1 = e.DBAccess.DataSet.Tables["grn1"];
            DataTable grn2 = e.DBAccess.DataSet.Tables["grn2"];
            #region set type to LOCAL
            grnh["type"] = "LOCAL";
            #endregion
            #region update detail columns
            foreach (DataRow dr in grn1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(grnh, dr, "status/whnum");
                }
            }

            foreach (DataRow dr in grn2.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(grnh, dr, "status/whnum");
                }
            }
            #endregion

            #region confirm:update to mwt
            #region get location for mwt
            string location = "";
            string outlet = "select * from posm where posnum='" + posid + "'";
            DataSet dt = this.dbaccess.ReadSQLTemp("outlet", outlet);

            if (!Tools.isFrontEnd())
            {
                location = "HQ";
            }
            else
            {
                if (dt.Tables[0] != null)
                {
                    location = dt.Tables[0].Rows[0]["sitenum"].ToString();
                }
            }
            #endregion

            if (grnh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                // MDTReader.updateMWT(ref this.dbaccess, "refnum|matnum|detail|docunum|grnum|uom|apnum|qty|stdcost|cosamt|discamt|period|user|flag|status|created|modified|stkdate|trandate|remark|whnum|price", "grn1", "mwt");
                //foreach (DataRow dr in grn1.Rows)
                //{
                //    if (dr.RowState != DataRowState.Deleted)
                //    {
                        DataTable GroupGldAcc = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT matnum,detail,qty,uom,'' barcode FROM GRN1 where refnum ='"+grnh["refnum"]+"' and ISNULL(sn,0)=0"
+" union all SELECT matnum,detail,1 qty,uom,barcode FROM GRN2 where refnum ='"+grnh["refnum"]+"'");
                        if (GroupGldAcc.Rows.Count > 0)
                        {
                            foreach (DataRow drCC in GroupGldAcc.Rows)
                            {
                                if (drCC.RowState != DataRowState.Deleted)
                                {
                                    DataRow addGL = mwt.Rows.Add(new object[] { });
                                    addGL["refnum"] = grnh["refnum"];
                                    addGL["matnum"] = drCC["matnum"];
                                    addGL["detail"] = drCC["detail"];
                                    addGL["qty"] = drCC["qty"];
                                    addGL["status"] ="P";
                                    addGL["uom"] = drCC["uom"];
                                    addGL["barcode"] = drCC["barcode"];
                                    addGL["user"] = Common.DEFAULT_SYSTEM_USERNAME;
                                    addGL["flag"] = "GRN";
                                    addGL["year"] = Common.DEFAULT_SYSTEM_YEAR;
                                    addGL["trandate"] = grnh["trandate"];
                                    addGL["created"] =System.DateTime.Today;
                                    addGL["modified"] = System.DateTime.Today;
                                    addGL["whnum"] = grnh["whnum"];

                                    addGL["guid"] = Tools.getGUID();
                                    addGL["location"] = location;
                                }
                            }
                  
                }
            }
            #endregion
        }

        #endregion

        #region Document_Print_Handle
        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow grnh = e.DBAccess.DataSet.Tables["grnh"].Rows[0];

            if (grnh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "grnh/grn1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }
        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

            BizFunctions.SetCoyForPrinting(dbaccess);

            e.DataSource = e.DBAccess.DataSet;
        }

        #endregion

        #region Document_Extract_Handle

        protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Extract_Handle(sender, e);

            DataRow grnh = dbaccess.DataSet.Tables["grnh"].Rows[0];
            #region Steph - MDT Extraction

            Hashtable HsExtract = MDTReader.GetExtraction("grn", "POR-GRN Extract", TabDetail, this.dbaccess);

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

        #region DocumentPage Event
        protected override void Voucher_Edit_Handle(object sender, VoucherHandleEventArgs e)
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
        #endregion

        #region F3
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow grnh = dbaccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = dbaccess.DataSet.Tables["grn1"];
            switch (e.ControlName)
            {
                case "grnh_ponum":
                    //e.DefaultCondition = "type='LOCAL'";
                    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);

            DataRow grnh = dbaccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = dbaccess.DataSet.Tables["grn1"];
            DataTable grn2 = dbaccess.DataSet.Tables["grn2"];
            switch (e.ControlName)
            {
                case "grnh_ponum":
                    string ponum = grnh["ponum"].ToString().Trim();
                    if (!ponum.Equals(""))
                    {
                        #region Extraction one porh to grnh
                        string selectPorh = "SELECT * FROM porh WHERE refnum in (select ponum from (SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price,sn from " +
                                "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price,sn from por1 UNION ALL" +
                                " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price,sn from grn1 WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "')a" +
                                "  WHERE ponum = '" + grnh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,discamt,detail,sn" +
                                " HAVING SUM(qty) >0)frmSelectDetailBelow) ";
                        this.dbaccess.ReadSQL("porhTmp", selectPorh);
                        DataTable porhTmp = this.dbaccess.DataSet.Tables["porhTmp"];

                        if (porhTmp.Select().Length > 0)
                        {
                            grnh["apnum"] = porhTmp.Rows[0]["apnum"];
                            //   grnh["sn"] = porhTmp.Rows[0]["sn"];
                            grnh["oricur"] = porhTmp.Rows[0]["oricur"];
                            grnh["remark"] = porhTmp.Rows[0]["remark"];
                            grnh["gstgrpnum"] = porhTmp.Rows[0]["gstgrpnum"];
                        }
                        else
                        {
                            grnh["apnum"] = "";
                            grnh["docunum"] = "";
                            //     grnh["sn"] = "";
                            grnh["oricur"] = "";
                            grnh["remark"] = "";
                            grnh["gstgrpnum"] = "";
                        }
                        #endregion

                        #region Copy the detail page
                        BizFunctions.DeleteAllRows(grn1);

                        if (porhTmp.Rows.Count != 0)
                        {
                            #region Steph - Import por1 to grn1

                            //string selectPor1 = "SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
                            //    "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 UNION ALL" +
                            //    " SELECT ponum, matnum,uom,discamt,detail,-qty as qty,0 as price from grn1 WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "')a" +
                            //    "  WHERE ponum = '" + grnh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,discamt,detail" +
                            //    " HAVING SUM(qty) >0";

                            //Yushu Modified20100329-Do not sum price because there is special scenarios in ATL where there will be same items of different pricing
                            string selectPor1 = "SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,price,sn from " +
                                "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price,sn from por1 UNION ALL" +
                                " SELECT ponum, matnum,uom,discamt,detail,-qty as qty,0 as price,sn from grn1 WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "')a" +
                                "  WHERE ponum = '" + grnh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,discamt,detail,price,sn" +
                                " HAVING SUM(qty) >0";


                            this.dbaccess.ReadSQL("por1Tmp", selectPor1);
                            DataTable por1Tmp = this.dbaccess.DataSet.Tables["por1Tmp"];

                            BizFunctions.DeleteAllRows(grn1);
                            foreach (DataRow dr in por1Tmp.Select())
                            {
                                dr.SetAdded();
                                grn1.ImportRow(dr);
                            }
                            foreach (DataRow dr2 in grn1.Select())
                            {
                                dr2["refnum"] = grnh["refnum"].ToString().Trim();
                                //#region Steph - SOR is not using the dqty
                                //dr2["dqty"] = dr2["qty"];
                                //#endregion
                            }
                            #endregion
                        }

                        if (porhTmp.Rows.Count != 0)
                        {
                            #region Steph - Import por1 to grn1
                            //Yushu Modified20100329-Do not sum price because there is special scenarios in ATL where there will be same items of different pricing
                            string selectPor1 = "SELECT ponum,matnum,uom,barcode,detail from " +
                                "(SELECT refnum as ponum, matnum,uom,barcode,detail from por2 UNION ALL" +
                                " SELECT ponum, matnum,uom,barcode,detail from grn2 WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "')a" +
                                "  WHERE ponum = '" + grnh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,barcode,detail";


                            this.dbaccess.ReadSQL("por2Tmp", selectPor1);
                            DataTable por2Tmp = this.dbaccess.DataSet.Tables["por2Tmp"];

                            BizFunctions.DeleteAllRows(grn2);
                            foreach (DataRow dr in por2Tmp.Select())
                            {
                                dr.SetAdded();
                                grn2.ImportRow(dr);
                            }
                            foreach (DataRow dr2 in grn2.Select())
                            {
                                dr2["refnum"] = grnh["refnum"].ToString().Trim();
                                //#region Steph - SOR is not using the dqty
                                //dr2["dqty"] = dr2["qty"];
                                //#endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    AutoCalc();
                    break;
            }
        }
        #endregion

        #region Document Event

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);

            opened = true;
            this.dbaccess = e.DBAccess;
            this.formsCollection = e.FormsCollection;
            this.formName = (sender as Form).Name;
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            DataRow grnh = dbaccess.DataSet.Tables["grnh"].Rows[0];

            grnh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

            #region initial controls
            btn_Extract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Extract") as Button;
            btn_SIV = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_autoSIV") as Button;
            rad_SIVN = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_SIVN") as RadioButton;
            rad_SIVY = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_SIVY") as RadioButton;
            rad_SIVN.CheckedChanged += new EventHandler(rad_SIVN_CheckedChanged);
            rad_SIVY.CheckedChanged += new EventHandler(rad_SIVY_CheckedChanged);
            btn_Extract.Click += new EventHandler(btn_Extract_Click);
            btn_SIV.Click += new EventHandler(btn_SIV_Click);

            //Added By Yushu
            btn_PrintBarcode = BizXmlReader.CurrentInstance.GetControl((e.FormsCollection["detail"] as Form).Name, "btnPrintBarcode") as Button;
            btn_PrintBarcode.Click += new EventHandler(btn_PrintBarcode_Click);
            #endregion

            e.DBAccess.DataSet.Tables["grnh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_GRNH_ColumnChanged);
            e.DBAccess.DataSet.Tables["grn1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_GRN1_ColumnChanged);

            #region set controls
            if (grnh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                btn_SIV.Enabled = false;
            }
            else
            {
                if (rad_SIVY.Checked)
                {
                    btn_SIV.Enabled = true;
                }
                else
                {
                    btn_SIV.Enabled = false;
                }
            }
            #endregion
        }
        #endregion

        #region Private Functions
        private void btn_SIV_Click(object sender, EventArgs e)
        {
            GRN.AutoSIV AutoSiv = new AutoSIV();
            AutoSiv.ShowDialog();
        }

        private void btn_Extract_Click(object sender, EventArgs e)
        {
            DataRow grnh = dbaccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = dbaccess.DataSet.Tables["grn1"];

            if (grnh["apnum"].ToString() != string.Empty)
            {
                #region Import pon1 to grn1
                /*
                BizFunctions.DeleteAllRows(grn1);
                string selectPor1 = "SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
                     "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from pon1 where apnum='" + grnh["apnum"].ToString() + "' UNION ALL" +
                     " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price from grn1" +
                     " WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "' and ponum!=grn1.ponum)a" +
                     " GROUP BY ponum,matnum,uom,discamt,detail" +
                     " HAVING SUM(qty) >0";

                this.dbaccess.ReadSQL("por1Tmp", selectPor1);
                DataTable por1Tmp = this.dbaccess.DataSet.Tables["por1Tmp"];

                BizFunctions.DeleteAllRows(grn1);
                foreach (DataRow dr in por1Tmp.Select())
                {
                    dr.SetAdded();
                    grn1.ImportRow(dr);
                }
                foreach (DataRow dr2 in grn1.Select())
                {
                    dr2["refnum"] = grnh["refnum"].ToString().Trim();
                    //#region Steph - SOR is not using the dqty
                    //dr2["dqty"] = dr2["qty"];
                    //#endregion
                }
                 */
                #endregion
                Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
                DataTable oriTable = grn1;

                try
                {
                    // Open Extract Form
                    ExtractPORForm ExtractPOR = new ExtractPORForm(this.dbaccess, oriTable);
                    ExtractPOR.ShowDialog(frm);

                    #region assign line number for grn1
                    int line = 100;
                    foreach (DataRow dr in grn1.Rows)
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
            AutoCalc();
        }

        private void rad_SIVY_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_SIVY.Checked)
            {
                btn_SIV.Enabled = true;
            }
        }

        private void rad_SIVN_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_SIVN.Checked)
            {
                btn_SIV.Enabled = false;
            }
        }
        #endregion

        #region Refresh

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow grnh = e.DBAccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = dbaccess.DataSet.Tables["grn1"];

            setDefaults(dbaccess.DataSet, "grnh/grn1");

            if (grnh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSN)
            {
                if (grnh["docunum"].ToString().Trim() == String.Empty || grnh["docunum"] == System.DBNull.Value)
                    grnh["docunum"] = grnh["refnum"];
            }

            grnh["grnum"] = grnh["refnum"];

            AutoCalc();
        }

        #endregion

        #region ColumnChangedEvents

        #region grnh
        private void Voucher_GRNH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow grnh = this.dbaccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = this.dbaccess.DataSet.Tables["grn1"];

            switch (e.Column.ColumnName)
            {
                case "apnum":
                    #region apnum
                    dbaccess.ReadSQL("getApmInfo", "SELECT apnum,apname,ptc,address,phone,hp,fax,ptnum,gstgrpnum,oricur FROM apm where apnum ='" + e.Row["apnum"].ToString().Trim() + "'");
                    if (dbaccess.DataSet.Tables["getApmInfo"].Rows.Count > 0)
                    {
                        DataRow getApmInfo = dbaccess.DataSet.Tables["getApmInfo"].Rows[0];
                        e.Row["apname"] = getApmInfo["apname"];
                        e.Row["contact"] = getApmInfo["ptc"];
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
                        e.Row["apname"] = "";
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
                case "ponum":
                    //#region Extraction one porh to grnh
                    //string selectPorh = "SELECT * FROM porh WHERE refnum in (select ponum from (SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
                    //        "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 UNION ALL" +
                    //        " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price from grn1 WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "')a" +
                    //        "  WHERE ponum = '" + grnh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,discamt,detail" +
                    //        " HAVING SUM(qty) >0)frmSelectDetailBelow) ";
                    //this.dbaccess.ReadSQL("porhTmp", selectPorh);
                    //DataTable porhTmp = this.dbaccess.DataSet.Tables["porhTmp"];

                    //if (porhTmp.Select().Length > 0)
                    //{
                    //    e.Row["apnum"] = porhTmp.Rows[0]["apnum"];
                    //    e.Row["docunum"] = porhTmp.Rows[0]["docunum"];
                    //    e.Row["oricur"] = porhTmp.Rows[0]["oricur"];
                    //    e.Row["remark"] = porhTmp.Rows[0]["remark"];
                    //    e.Row["gstgrpnum"] = porhTmp.Rows[0]["gstgrpnum"];
                    //}
                    //else
                    //{
                    //    e.Row["apnum"] = "";
                    //    e.Row["docunum"] = "";
                    //    e.Row["oricur"] = "";
                    //    e.Row["remark"] = "";
                    //    e.Row["gstgrpnum"] = "";
                    //}
                    //#endregion

                    //#region Copy the detail page
                    //BizFunctions.DeleteAllRows(grn1);

                    //if (porhTmp.Rows.Count != 0)
                    //{
                    //    #region Steph - Import por1 to grn1

                    //    string selectPor1 = "SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
                    //        "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 UNION ALL" +
                    //        " SELECT ponum, matnum,uom,discamt,detail,-qty as qty,0 as price from grn1 WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "')a" +
                    //        "  WHERE ponum = '" + grnh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,discamt,detail" +
                    //        " HAVING SUM(qty) >0";


                    //    this.dbaccess.ReadSQL("por1Tmp", selectPor1);
                    //    DataTable por1Tmp = this.dbaccess.DataSet.Tables["por1Tmp"];

                    //    BizFunctions.DeleteAllRows(grn1);
                    //    foreach (DataRow dr in por1Tmp.Select())
                    //    {
                    //        dr.SetAdded();
                    //        grn1.ImportRow(dr);
                    //    }
                    //    foreach (DataRow dr2 in grn1.Select())
                    //    {
                    //        dr2["refnum"] = grnh["refnum"].ToString().Trim();
                    //        //#region Steph - SOR is not using the dqty
                    //        //dr2["dqty"] = dr2["qty"];
                    //        //#endregion
                    //    }
                    //    #endregion
                    //}
                    //#endregion
                    break;
                case "oricur":
                    #region set exrate
                    e.Row.BeginEdit();
                    string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
                    this.dbaccess.ReadSQL("exrate", exrStr);
                    if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
                    {
                        decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + ""]);
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
                            e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)e.Row["trandate"]);
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
                    #region set grnh exrate

                    e.Row.BeginEdit();
                    string strexr = "Select rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + " FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
                    this.dbaccess.ReadSQL("exrate", strexr);
                    if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
                    {
                        decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + ""]);
                        e.Row["exrate"] = exrate;
                    }
                    e.Row.EndEdit();
                    break;
                    #endregion
            }
        }
        #endregion

        #region grn1
        private void Voucher_GRN1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow grnh = this.dbaccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = this.dbaccess.DataSet.Tables["grn1"];
            if (e.Row["discamt"] == System.DBNull.Value)
            {
                e.Row["discamt"] = 0;
            }

            switch (e.Column.ColumnName)
            {
                case "matnum":
                    dbaccess.ReadSQL("getMatm", "SELECT matname, uomcode FROM matm where matnum ='" + e.Row["matnum"].ToString().Trim() + "'");
                    if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
                    {
                        if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
                            e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
                        if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
                            e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
                    }
                    break;
                case "qty":
                    if (e.Row["price"] != System.DBNull.Value && Convert.ToDecimal(e.Row["qty"]) > 0)
                    {
                        #region set deafult discamt=0
                        if (e.Row["discamt"] == System.DBNull.Value)
                        {
                            e.Row["discamt"] = 0.00;
                        }
                        #endregion
                        e.Row["grosamt"] = (decimal)e.Row["price"] * (decimal)e.Row["qty"];
                        e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                    }
                    break;
                case "price":
                    if (e.Row["qty"] != System.DBNull.Value && Convert.ToDecimal(e.Row["price"]) > 0)
                    {
                        #region set deafult discamt=0
                        if (e.Row["discamt"] == System.DBNull.Value)
                        {
                            e.Row["discamt"] = 0.00;
                        }
                        #endregion
                        e.Row["grosamt"] = (decimal)e.Row["price"] * (decimal)e.Row["qty"];
                        e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                    }
                    break;
                case "discamt":
                    #region set deafult discamt=0
                    if (e.Row["discamt"] == System.DBNull.Value)
                    {
                        e.Row["discamt"] = 0.00;
                    }
                    #endregion
                    if (e.Row["grosamt"] != System.DBNull.Value && Convert.ToDecimal(e.Row["discamt"]) > 0)
                    {
                        e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                    }
                    break;
            }
        }
        #endregion

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

        #region Steph - To set the Auto Calculation to be use in various events
        private void AutoCalc()
        {
            DataRow grnh = dbaccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = dbaccess.DataSet.Tables["grn1"];
            DataTable grn2 = dbaccess.DataSet.Tables["grn2"];
            setDefaults(dbaccess.DataSet, "grn1");

            grnh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(grnh["trandate"]));

            #region grn1

            grn1_grosamt = 0;
            grn1_discamt = 0;
            grn1_oriamt = 0;
         //   BizFunctions.DeleteAllRows(grn2);

            foreach (DataRow dr in grn1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (Convert.ToDecimal(dr["qty"]) > 0)
                    {
                        BizFunctions.UpdateDataRow(grnh, dr, "refnum/apnum/docunum/grnum/trandate/period/user/status/flag/expire/created/modified");
                        if ((decimal)dr["grosamt"] == 0 || dr["grosamt"] == System.DBNull.Value)
                            dr["grosamt"] = (decimal)dr["qty"] * (decimal)dr["price"];

                        dr["oriamt"] = (decimal)dr["grosamt"] - (decimal)dr["discamt"];

                        dr["cosamt"] = dr["oriamt"]; // Reason is to get the cosamt after deduct discamt given by supplier.
                        dr["stdcost"] = (decimal)dr["oriamt"] / (decimal)dr["qty"] * (decimal)grnh["exrate"];

                        grn1_grosamt += (decimal)dr["grosamt"];
                        grn1_discamt += (decimal)dr["discamt"];
                        grn1_oriamt += (decimal)dr["oriamt"];
                        if (dr["sn"].ToString() == "True") { insertGRN2(dr["matnum"].ToString(), dr["qty"].ToString(), dr["detail"].ToString(), dr["uom"].ToString()); }
                    }
                }
            }
            grnh["grosamt"] = grn1_grosamt;
            grnh["discamt"] = grn1_discamt;
            grnh["oriamt"] = grn1_oriamt;
            #endregion
        }

        private void insertGRN2(string p,string co,string uom,string detail)
        {
            DataRow grnh = dbaccess.DataSet.Tables["GRNH"].Rows[0];
            DataTable grn2 = dbaccess.DataSet.Tables["GRN2"];
            int coo = Convert.ToInt16(co);

            string find = "matnum = '"+p+"'";
 DataRow[] foundRows = grn2.Select(find);

            if (grn2.Rows.Count > 0)
            {
                foreach (DataRow dr in grn2.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        if (foundRows.Length<=0)
                        {
                            for (int i = 0; i < (int)coo; i++)
                            {
                                DataRow dr_siv1 = grn2.NewRow();
                                dr_siv1["uom"] = uom;
                                dr_siv1["matnum"] = p;
                                dr_siv1["qty"] = 1;
                                dr_siv1["detail"] = detail;
                                grn2.Rows.Add(dr_siv1);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < (int)coo; i++)
                {
                    DataRow dr_siv1 = grn2.NewRow();
                    dr_siv1["uom"] = uom;
                    dr_siv1["matnum"] = p;
                    dr_siv1["qty"] = 1;
                   dr_siv1["detail"] = detail;
                    grn2.Rows.Add(dr_siv1);
                }
             //   grn2.AcceptChanges();
            }
        }
        #endregion

        #region Yushu- btn_PrintBarcode_Click

        private void btn_PrintBarcode_Click(object sender, EventArgs e)
        {
            #region Variables

            DataRow grnh = dbaccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = dbaccess.DataSet.Tables["grn1"];
            DataTable dtBarcode;
            DataRow drBarcode = null;

            string matname = "";
            string matnum = "";
            string parameter = "";
            string barcode = "";
            int qty = 0;
            decimal price = -1;
            string sql = "";
            bool isEmpty = false;

            #endregion

            this.result = MessageBox.Show("Start Barcode Printing?", "Print Barcode?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            #region Start Print

            if (this.result == DialogResult.Yes)
            {
                #region Save Before Printing

                //if (grnh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
                //{
                if (BizValidate.CheckRowState(this.dbaccess.DataSet, "grnh/grn1"))
                {
                    MessageBox.Show("Please save before Printing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    foreach (DataRow dr1 in grn1.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            if (!BizFunctions.IsEmpty(dr1["qty"]))
                            {
                                qty = Convert.ToInt32(dr1["qty"]);
                            }

                            if (!BizFunctions.IsEmpty(dr1["price"]))
                            {
                                price = Convert.ToDecimal(dr1["price"]);
                            }

                            #region Select Barcode data and assign to dtBarcode table

                            sql = "select matnum,matname,ploftcode+pflexcode+pshfcode+pcolcode+pszcode as parameter,barcode,price from "
                                + "(select g.matnum,m.matname,case when isnull(m.ploftcode,'')='' or isnull(m.ploftcode,'')='***' then ''+ SPACE(1) "
                                + "else m.ploftcode+ SPACE(1) end as ploftcode,"
                                + "case when isnull(m.pflexcode,'')='' or isnull(m.pflexcode,'')='**' then ''+ SPACE(1) "
                                + "else m.pflexcode+ SPACE(1) end as pflexcode,"
                                + "case when isnull(m.pshfcode,'')='' or isnull(m.pshfcode,'')='***' then ''+ SPACE(1) "
                                + "else m.pshfcode+ SPACE(1) end as pshfcode,"
                                + "case when isnull(m.pcolcode,'')='' or isnull(m.pcolcode,'')='****' then ''+ SPACE(1) "
                                + "else m.pcolcode+ SPACE(1) end as pcolcode,"
                                + "case when isnull(m.pszcode,'')='' or isnull(m.pszcode,'')='****' then ''+ SPACE(1) "
                                + "else m.pszcode+ SPACE(1) end as pszcode,"
                                + "m.barcode,g.price from grn1 g "
                                + "left join matm m on g.matnum=m.matnum where g.refnum='" + grnh["refnum"].ToString()
                                + "' and m.matnum='" + dr1["matnum"].ToString() + "') t";

                            this.dbaccess.ReadSQL("tmpBarcode", sql);
                            dtBarcode = this.dbaccess.DataSet.Tables["tmpBarcode"];

                            #endregion

                            if (dtBarcode.Rows.Count > 0)
                            {
                                drBarcode = dtBarcode.Rows[0];

                                if (drBarcode["matnum"].ToString().ToUpper().Trim() == dr1["matnum"].ToString().ToUpper().Trim())
                                {
                                    #region Create New Label, point to Label Path and Barcode Printer

                                    LGApp nice = new LGApp();
                                    LGLabel labelintf = new LGLabel();

                                    bool setprintx;
                                    if (BizFunctions.IsEmpty(grnh["printPrice"]))
                                    {
                                        grnh["printPrice"] = false;
                                    }
                                    long labelID = 0;
                                    if ((bool)grnh["printPrice"] == true)
                                    {
                                        labelID = nice.LabelOpen(@"C:\TVBarcode\TVBarcode.lbl");
                                    }
                                    else
                                    {
                                        labelID = nice.LabelOpen(@"C:\TVBarcode\TVBarcode(NoPrice).lbl");
                                    }

                                    // get printer
                                    setprintx = nice.LabelSetPrinter((int)labelID, ConfigurationManager.AppSettings.Get("BarcodePrinter"));

                                    #endregion

                                    #region Assign Label Data

                                    barcode = drBarcode["barcode"].ToString();
                                    matname = drBarcode["matname"].ToString();
                                    matnum = drBarcode["matnum"].ToString();
                                    parameter = drBarcode["parameter"].ToString();
                                    //if (!BizFunctions.IsEmpty(drBarcode["price"]))
                                    //{
                                    //    price = Convert.ToDecimal(drBarcode["price"]);
                                    //}

                                    #endregion

                                    #region Call PrintBarcode Method

                                    if ((bool)grnh["printPrice"] == true)
                                    {
                                        if (!(BizFunctions.IsEmpty(matnum) && BizFunctions.IsEmpty(matname)
                                            && BizFunctions.IsEmpty(barcode)) && price > -1 && qty > 0)
                                        {
                                            PrintBarcode(drBarcode, nice, labelintf, setprintx, labelID, matname, matnum, parameter, barcode, qty, price);
                                        }
                                        else
                                        {
                                            isEmpty = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!(BizFunctions.IsEmpty(matnum) && BizFunctions.IsEmpty(matname)
                                            && BizFunctions.IsEmpty(barcode)) && qty > 0)
                                        {
                                            PrintBarcode2(drBarcode, nice, labelintf, setprintx, labelID, matname, matnum, parameter, barcode, qty);
                                        }
                                        else
                                        {
                                            isEmpty = true;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #region If Empty or Invalid Data detected, Prompt Error

                    if (isEmpty)
                    {
                        MessageBox.Show("Invalid Material Code/Material Name/Barcode,Negative Price or 0 Qty detected!", "No Label Printed For Some Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion
                }
                //}

                #endregion
            }
            #endregion
        }

        #region Print Barcode

        private void PrintBarcode(DataRow drBarcode, LGApp nice, LGLabel labelintf, bool setprintx, long labelID, string matname, string matnum, string parameter, string barcode, int qty, decimal price)
        {
            #region Split Parameter into Multiline

            if (parameter.Length > 51)
                parameter = parameter.Substring(0, 51);

            if (matname.Length > 45)
                //matname = matname.Substring(0, 45);
                matname = matname.Replace(" ", "");

            //if (parameter.Length > 17 && parameter.Substring(17, 1) != " ")
            //{
            //    parameter = matname.Substring(0, 17) + " " + parameter.Substring(17);
            //}

            #endregion

            int multiples = qty / 5;
            int remainder = qty % 5;

            #region set label
            setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "price", price.ToString(), -9999, -9999);

            //setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "price", price.ToString(), 5000, 5000);
            #endregion

            for (int i = 0; i < multiples; i++)
            {
                setprintx = nice.LabelPrint((int)labelID, "5");
            }

            setprintx = nice.LabelPrint((int)labelID, remainder.ToString());

            labelintf.Free();
        }

        #endregion

        #region Print Barcode2

        private void PrintBarcode2(DataRow drBarcode, LGApp nice, LGLabel labelintf, bool setprintx, long labelID, string matname, string matnum, string parameter, string barcode, int qty)
        {
            #region Split Parameter into Multiline

            if (parameter.Length > 51)
                parameter = parameter.Substring(0, 51);

            if (matname.Length > 45)
                //matname = matname.Substring(0, 45);
                matname = matname.Replace(" ", "");

            //if (parameter.Length > 17 && parameter.Substring(17, 1) != " ")
            //{
            //    parameter = matname.Substring(0, 17) + " " + parameter.Substring(17);
            //}

            #endregion

            int multiples = qty / 5;
            int remainder = qty % 5;

            #region set label
            setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, -9999, -9999);

            //setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, 5000, 5000);
            #endregion

            for (int i = 0; i < multiples; i++)
            {
                setprintx = nice.LabelPrint((int)labelID, "5");
            }

            setprintx = nice.LabelPrint((int)labelID, remainder.ToString());

            labelintf.Free();
        }

        #endregion

        #endregion
    }
}

