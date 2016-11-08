using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;

using BizRAD.BizReport;
using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizCommon;
using ATL.BizLogicTools;
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;

namespace ATL.TRQ
{
    public class Voucher_TRQ : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Variable
        protected bool opened = false;
        string posid = System.Configuration.ConfigurationManager.AppSettings.Get("POSID");
        string currentYear = null;
        Hashtable remoteHash = null;
        DBAccess remoteDBAccess = null;
        DBAccess dbaccess = null;
        protected string formName = null;
        protected string detailName = null;
        bool trqhColumnChange = true;
        bool trr1ColumnChange = true;
        bool materialFlag = false;
        string sitenum, flag = null;

        Hashtable selectsCollection = null;
        string command = null;
        DataGrid dg_detail = null;
        private string formName4SideBtn;
        protected DataTable trq1;

        TextBox txt_barcode = null;
        protected Button btn_ExtractMATM, btn_ExtractTemplate = null;
        private ComboBox trqh_trqyear, trqh_trqmonth = null;

        protected Label lblDisplay = null;
        #endregion

        #region Construct

        public Voucher_TRQ(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_TRQ.xml", moduleName, voucherBaseHelpers)
        {
            this.flag = moduleName;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "trqh.flag='" + flag + "' AND trqh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (trqh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " trqh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " trqh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND trqh.flag='" + flag + "' AND trqh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

        #region Delete On Click

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);

            DataTable dtD = this.dbaccess.DataSet.Tables["trq1"];

            foreach (DataRow dr in dtD.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    int check = 0;

                    check = checkdatagridError(dr);
                    seterrorMessage(dr, check);

                }
            }
        }
        #endregion

        #region Document Cancel

        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);
            opened = false;

        }

        #endregion

        #region Document Page Event

        #region DocumentPage Event
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

        protected override void Voucher_New_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
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

        #region Form On Load Event

        #region Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            selectsCollection = new Hashtable();
            opened = true;

            ATL.BizLogicTools.Tools.setDefaults(e.DBAccess.DataSet, "trqh/trq1");

            this.currentYear = Common.DEFAULT_SYSTEM_YEAR;
            this.dbaccess = e.DBAccess;
            this.formName = (e.FormsCollection["header"] as Form).Name;
            this.detailName = (e.FormsCollection["detail"] as Form).Name;

            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];

            trqh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

            #region Get connections
            string standardstring = ConfigurationManager.AppSettings.Get("StandardSQLString");
            string specialstring = ConfigurationManager.AppSettings.Get("SpecialSQLString");
            string dataaccessstring = ConfigurationManager.AppSettings.Get("DataAccessString");

            if (Tools.isFrontEnd())
            {
                remoteHash = new Hashtable();
                remoteHash.Add("StandardSQL", standardstring);
                remoteHash.Add("SpecialSQL", specialstring);
                remoteHash.Add("DataAccess", dataaccessstring);
                remoteDBAccess = new DBAccess(remoteHash);
            }
            dbaccess = e.DBAccess;
            #endregion

            #region Set default values upon load
            if (trqh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                trqh["agreedby"] = Common.DEFAULT_SYSTEM_USERNAME;
                string strHemph = "select * from hemph where empname= '" + Common.DEFAULT_SYSTEM_USERNAME + "'";
                e.DBAccess.ReadSQL("hemph", strHemph);
                DataTable dthemph = e.DBAccess.DataSet.Tables["hemph"];

                if (dthemph.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(trqh["requestedby"]))
                    {
                        trqh["requestedby"] = dthemph.Rows[0]["empnum"];
                    }
                }
            }

            if (Convert.IsDBNull(trqh["trandate"]))
            {
                trqh["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }
            if (Convert.IsDBNull(trqh["senddate"]))
            {
                trqh["senddate"] = trqh["trandate"];
            }
            //   if (Convert.IsDBNull(trqh["approveby"]))
            {
                trqh["approveby"] = Common.DEFAULT_SYSTEM_USERNAME;
            }
            if (Convert.IsDBNull(trqh["approvedate"]))
            {
                trqh["approvedate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);

            }

            string GetSITMTB = "Select * from SITM WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("SITMTB", GetSITMTB);

            string GetDOCTB = "Select refnum,sitenum from CTRH WHERE [STATUS]<>'V' UNION Select refnum,sitenum from ADH WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("DOCTBALL", GetDOCTB);

            // Default sitenum is always HQ for backend. Depends on POSID in appconfig.
            //if (BizFunctions.IsEmpty(trqh["sitenum"]))
            //{
            //    string command = "select sitenum from posm where posnum = '" + ConfigurationManager.AppSettings.Get("POSID") + "'";
            //    e.DBAccess.ReadSQL("posm", command);
            //    DataRow posm = e.DBAccess.DataSet.Tables["posm"].Rows[0];
            //    trqh["sitenum"] = posm["sitenum"].ToString();
            //    trqh["fromsitenum"] = posm["sitenum"].ToString();

            //    if (posid != null)
            //    {
            //        string command1 = "select sitenum from posm where posnum = '" + posid + "'";
            //        e.DBAccess.ReadSQL("posm", command1);
            //        DataRow posm1 = e.DBAccess.DataSet.Tables["posm"].Rows[0];
            //        sitenum = posm1["sitenum"].ToString();
            //    }
            //    else
            //        sitenum = posid;
            //}

            trqh["tositenum"] = "HQ";

            if (BizFunctions.IsEmpty(trqh["requestedby"]))
            {
                trqh["requestedby"] = Common.DEFAULT_SYSTEM_EMPNUM;
            }
            #endregion

            #region initial controls
            e.DBAccess.DataSet.Tables["trqh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRQH_ColumnChanged);
            e.DBAccess.DataSet.Tables["trq1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRQ1_ColumnChanged);
            //Button btn_import = BizXmlReader.CurrentInstance.GetControl(formName, "btn_Import") as Button;
            ////Button btn_ExtractGRN = BizXmlReader.CurrentInstance.GetControl(formName, "btn_ExtractGRN") as Button;
            //btn_import.Click += new EventHandler(btn_import_Click);
            ////btn_ExtractGRN.Click += new EventHandler(btn_ExtractGRN_Click);
            //Button btnExport = BizXmlReader.CurrentInstance.GetControl(formName, "btn_Export") as Button;
            //btnExport.Click += new EventHandler(btnExport_Click);
            dg_detail = BizXmlReader.CurrentInstance.GetControl((e.FormsCollection["detail"] as Form).Name, "dg_detail") as DataGrid;
            this.formName4SideBtn = (sender as Form).Name;

            btn_ExtractMATM = BizXmlReader.CurrentInstance.GetControl(formName, "btn_ExtractMATM") as Button;
            btn_ExtractMATM.Click += new EventHandler(btn_ExtractMATM_Click);

            btn_ExtractTemplate = BizXmlReader.CurrentInstance.GetControl(formName, "btn_ExtractTemplate") as Button;
            btn_ExtractTemplate.Click += new EventHandler(btn_ExtractTemplate_Click);

            lblDisplay = BizXmlReader.CurrentInstance.GetControl(formName, "lbl_display") as Label;
            lblDisplay.Font = new Font(lblDisplay.Font, FontStyle.Bold);

            Button btn_Document_Void = (Button)BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, Common.DEFAULT_DOCUMENT_BTNVOID);
            btn_Document_Void.Visible = true;
            btn_Document_Void.Enabled = true;

            #endregion

            #region disable void/reopen button for outlet user
            //allow HQ user to void and reopen TRQ,but need to check whether tri Extract this trq or not.
            //if yes,can't void and reopen            
            //if (!Tools.isHQ(this.sitenum, this.dbaccess))
            //{
            //    Tools.disableButtons((sender as Form).Name);
            //}
            #endregion

            #region Scan
            //txt_barcode = BizXmlReader.CurrentInstance.GetControl((e.FormsCollection["detail"] as Form).Name, "txt_barcode") as TextBox;
            //txt_barcode.KeyDown += new KeyEventHandler(txt_barcode_KeyDown);
            #endregion

            e.DBAccess.DataSet.Tables["TRQ2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRQ2_ColumnChanged);

            Initialise();
        }




        #endregion

        #endregion

        void Voucher_TRQ2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
                //case "officerqty":
                //    {
                //        manpowerFlag = true;
                //    }
                //    break;
            }
        }

        #region Initialise

        private void Initialise()
        {
            trqh_trqyear = BizXmlReader.CurrentInstance.GetControl(formName, "trqh_trqyear") as ComboBox;
            trqh_trqmonth = BizXmlReader.CurrentInstance.GetControl(formName, "trqh_trqmonth") as ComboBox;
            setTrqYear();
            setTrqMonth();

        }

        #endregion

        private void setTrqYear()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            int[] arr1 = new int[100];
            DateTime dt = (DateTime)trqh["trandate"];
            //     dt = (DateTime)trqh["trandate"];

            int Year = dt.AddYears(-50).Year;

            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = Year;
                Year = Year + 1;
            }

            trqh_trqyear.DataSource = arr1;
            if (!BizFunctions.IsEmpty(trqh["senddate"]))
            {
                trqh["trqyear"] = Convert.ToDateTime(trqh["senddate"]).Year;
            }
            else
            {
                trqh["trqyear"] = dt.Year;
            }

        }


        private void setTrqMonth()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            DateTime dt = (DateTime)trqh["trandate"];
            //   dt = (DateTime)trqh["trandate"];


            if (!BizFunctions.IsEmpty(trqh["senddate"]))
            {
                trqh["trqmonth"] = Convert.ToDateTime(trqh["senddate"]).Month;
            }
            else
            {
                trqh["trqmonth"] = dt.Month;
            }

        }

        private decimal previousQtyRequest(string matnum)
        {
            decimal previousQty = 0;
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];

            string getPreviousQty = "select (t1.matnum) , qty as previousQtyRequest,trandate from trq1  t1 where t1.fromsitenum='" + trqh["fromsitenum"] + "' " +
" and t1.matnum='" + matnum + "'  and refnum<>'" + trqh["refnum"] + "' and trandate<'" + BizFunctions.GetSafeDateString((DateTime)trqh["trandate"]) + "' order by trandate desc";

            this.dbaccess.ReadSQL("TempMatnumQty", getPreviousQty);

            DataTable TempMatnumQty = this.dbaccess.DataSet.Tables["TempMatnumQty"];

            if (TempMatnumQty.Rows.Count > 0)
            {
                previousQty = Convert.ToDecimal(TempMatnumQty.Rows[0]["previousQtyRequest"]);
            }
            return previousQty;
        }

        private DateTime previousQtyRequest(string matnum, DateTime bb)
        {
            DateTime previousdate = bb;
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];

            string getPreviousQty = "select (t1.matnum) , qty as previousQtyRequest,trandate from trq1  t1 where t1.fromsitenum='" + trqh["fromsitenum"] + "' " +
" and t1.matnum='" + matnum + "'  and refnum<>'" + trqh["refnum"] + "' and trandate<'" + BizFunctions.GetSafeDateString((DateTime)trqh["trandate"]) + "' order by trandate desc";

            this.dbaccess.ReadSQL("TempMatnumQty", getPreviousQty);

            DataTable TempMatnumQty = this.dbaccess.DataSet.Tables["TempMatnumQty"];

            if (TempMatnumQty.Rows.Count > 0)
            {
                previousdate = (DateTime)(TempMatnumQty.Rows[0]["trandate"]);
            }
            return previousdate;
        }


        # region Column Change Event
        private void Voucher_TRQH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (trqhColumnChange)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];
                DataTable trq1 = this.dbaccess.DataSet.Tables["trq1"];

                switch (e.Column.ColumnName)
                {
                    #region Case fromsitenum

                    case "fromsitenum":

                        if (!BizFunctions.IsEmpty(e.Row["fromsitenum"]))
                        {
                            // If sitenum invalid then prompt
                            if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", e.Row["fromsitenum"].ToString().Trim()))
                            {
                                MessageBox.Show("Source sitenum entered is invalid");
                                e.Row.SetColumnError("fromsitenum", "Invalid sitenum");
                            }
                            else
                            {
                                e.Row.SetColumnError("fromsitenum", "");

                                #region Update stock on hand values

                                foreach (DataRow dr in trq1.Rows)
                                {
                                    if (dr.RowState != DataRowState.Deleted)
                                    {
                                        /*
                                        string command = "SELECT matnum,sitenum,SUM(qty) AS qty FROM mwt" + this.currentYear +
                                              " WHERE matnum='" + dr["matnum"].ToString().Trim() +
                                              "' AND sitenum='" + e.Row["fromsitenum"].ToString().Trim() +
                                              "' AND trandate<='" + BizFunctions.GetSafeDateString((DateTime)trqh["trandate"]) +
                                              "' GROUP BY matnum,sitenum";
                                        this.dbaccess.ReadSQL("mwttemp", command);
                                        DataTable mwttemp = this.dbaccess.DataSet.Tables["mwttemp"];
                                        if (mwttemp.Rows.Count != 0)
                                            dr["stockonhand"] = mwttemp.Rows[0]["qty"];
                                        else*/
                                        dr["stockonhand"] = 0;
                                    }
                                }
                                #endregion

                                #region Update sitenum in detail
                                foreach (DataRow dr in trq1.Rows)
                                {
                                    if (dr.RowState != DataRowState.Deleted)
                                    {
                                        dr["fromsitenum"] = trqh["fromsitenum"];
                                    }
                                }
                                #endregion
                            }
                        }
                        break;

                    #endregion

                    #region Case tositenum

                    case "tositenum":
                        {
                            if (!BizFunctions.IsEmpty(e.Row["tositenum"]))
                            {
                                // If sitenum invalid then prompt
                                if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", e.Row["tositenum"].ToString().Trim()))
                                {
                                    MessageBox.Show("Destination sitenum entered is invalid");
                                    e.Row.SetColumnError("tositenum", "Invalid sitenum");
                                }
                                else
                                {
                                    e.Row.SetColumnError("tositenum", "");
                                    #region Update sitenum in detail
                                    foreach (DataRow dr in trq1.Rows)
                                    {
                                        if (dr.RowState != DataRowState.Deleted)
                                        {
                                            dr["tositenum"] = trqh["tositenum"];
                                        }
                                    }
                                    #endregion
                                }
                            }
                            break;
                        }

                    #endregion

                    #region Case requestedby

                    case "requestedby":
                        {
                            if (!BizFunctions.IsEmpty(e.Row["requestedby"]))
                            {
                                // If sitenum invalid then prompt
                                if (!BizValidate.CheckTableIsValid(dbaccess, "hemph", "empnum", e.Row["requestedby"].ToString().Trim()))
                                {
                                    MessageBox.Show("Invalid employee number entered");
                                    e.Row.SetColumnError("requestedby", "Invalid employee number");
                                }
                                else
                                {
                                    e.Row.SetColumnError("requestedby", "");
                                }
                            }
                            break;
                        }

                    case "remarks":
                        {
                            foreach (DataRow dr in trq1.Rows)
                            {
                                //dr["remarks"] = e.Row["remarks"];
                            }
                            break;
                        }

                    #endregion
                }
            }
        }

        private void Voucher_TRQ1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (trr1ColumnChange)
            {
                DataRow trqh = dbaccess.DataSet.Tables["trqh"].Rows[0];
                DataTable trq1 = dbaccess.DataSet.Tables["trq1"];
                switch (e.Column.ColumnName)
                {
                    case "matnum":
                        #region Update product name and uom
                        string cmd = "select * from matm where matnum='" + e.Row["matnum"].ToString().Trim() + "'";
                        DataSet tmpds = this.dbaccess.ReadSQLTemp("matm", cmd);
                        if (tmpds.Tables["matm"].Rows.Count != 0)
                        {
                            //e.Row["detail"] = tmpds.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                            //e.Row["barcode"] = tmpds.Tables["matm"].Rows[0]["barcode"].ToString().Trim();
                            //e.Row["uom"] = tmpds.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                            //e.Row["pcatcode"] = tmpds.Tables["matm"].Rows[0]["pcatcode"];

                            e.Row["lasttotalqty"] = previousQtyRequest(e.Row["matnum"].ToString().Trim());
                        }
                        #endregion
                        break;


                    #region Set DefaultValues for Qty   no use
                    //if (Convert.IsDBNull(e.Row["qty"]))
                    //    e.Row["qty"] = 0;
                    //#endregion

                    //#region Copy header information into the detail rows
                    //e.Row["refnum"] = trqh["refnum"];
                    //e.Row["trandate"] = trqh["trandate"];
                    //e.Row["sitenum"] = trqh["sitenum"];
                    //e.Row["status"] = trqh["status"];
                    //e.Row["agreedby"] = trqh["agreedby"];
                    //e.Row["sentby"] = trqh["sentby"];
                    //e.Row["senderremarks"] = trqh["senderremarks"];
                    //e.Row["requestedby"] = trqh["requestedby"];
                    //e.Row["fromsitenum"] = trqh["fromsitenum"];
                    //e.Row["tositenum"] = trqh["tositenum"];
                    //e.Row["year"] = trqh["year"];
                    //e.Row["flag"] = trqh["flag"];
                    //e.Row["user"] = trqh["user"];
                    //#endregion


                    //#region Update stock on hand values
                    ///*
                    //string command = "select matnum,sitenum,SUM(qty) AS qty from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                    //      " where matnum='" + e.Row["matnum"].ToString().Trim() +
                    //      "' AND sitenum='" + e.Row["fromsitenum"].ToString().Trim() +
                    //      "' AND trandate<='" + BizFunctions.GetSafeDateString((DateTime)trqh["trandate"]) +
                    //      //"' AND whnum='ADUI-MWH'"+
                    //      " GROUP BY bmatnum,sitenum";
                    //this.dbaccess.ReadSQL("mwttemp", command);
                    //DataTable mwttemp = this.dbaccess.DataSet.Tables["mwttemp"];
                    //if (mwttemp.Rows.Count != 0)
                    //    e.Row["stockonhand"] = mwttemp.Rows[0]["qty"];
                    //else*/
                    //    e.Row["stockonhand"] = 0;

                    //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                    //{
                    //    // If matnum invalid then prompt
                    //    if (!BizValidate.CheckTableIsValid(this.dbaccess, "matm", "matnum", e.Row["matnum"].ToString().Trim()))
                    //    {
                    //        e.Row.SetColumnError(e.Column.ColumnName, "Material Code not Valid");
                    //    }
                    //    # region Check Column Error
                    //    else
                    //    {
                    //        e.Row.SetColumnError(e.Column.ColumnName, "");
                    //        int check = 0;
                    //        check = checkdatagridError(e.Row);
                    //        if (check != 0)
                    //        {
                    //            seterrorMessage(e.Row, check);
                    //            break;
                    //        }
                    //    }
                    //}
                    //else
                    //    e.Row.SetColumnError(e.Column.ColumnName, "Material Code cannot be empty");
                    //    #endregion
                    //break;
                    #endregion


                    //case "qty":
                    //    #region qty
                    //    if (e.Row["qty"] != System.DBNull.Value)
                    //    {
                    //        if ((int)e.Row["qty"] < 0)
                    //        {
                    //            MessageBox.Show("Invalid quantity");
                    //            e.Row.SetColumnError("qty", "Invalid quantity");
                    //        }
                    //        else
                    //        {
                    //            materialFlag = true;
                    //        }
                    //    }
                    #region update ttqty,ttamt in header
                    //    calcTotal();
                    //    #endregion
                    //    break;
                    #endregion
                }
            }
        }

        private void calcTotal()
        {
            DataTable trq1 = dbaccess.DataSet.Tables["trq1"];
            DataRow trqh = dbaccess.DataSet.Tables["trqh"].Rows[0];

            #region update ttqty,ttamt in header
            decimal ttqty = 0;
            foreach (DataRow dr1 in trq1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (dr1["qty"] != System.DBNull.Value)
                    {
                        if ((int)dr1["qty"] < 0)
                        {
                            MessageBox.Show("Invalid quantity");
                            dr1.SetColumnError("qty", "Invalid quantity");
                        }
                        ttqty += (int)dr1["qty"];
                    }
                }
            }

            trqh["ttqty"] = ttqty;
            #endregion
        }
        #endregion

        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = e.DBAccess.DataSet.Tables["trq1"];

            if (BizFunctions.IsEmpty(trqh["fromsitenum"]))
            {
                MessageBox.Show("Please state the Request Site ", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                e.Handle = false;
            }

            if (e.Handle)
            {
                if (BizFunctions.IsEmpty(trqh["sitenum"]))
                {

                    trqh["sitenum"] = trqh["fromsitenum"].ToString();
                    sitenum = trqh["fromsitenum"].ToString();
                }
            }




            # region Check for empty row and empty mat code  no use
            //    foreach (DataRow dr1 in trq1.Rows)
            //    {
            //        if (dr1.RowState != DataRowState.Deleted)
            //        {
            //            if (BizFunctions.IsEmpty(dr1["matnum"]))
            //            {
            //                MessageBox.Show("Save Unsuccessful\nProduct Code cannot be empty !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                e.Handle = false;
            //                return;
            //            }
            //        }
            //    }

            //    if (trq1.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Save Unsuccessful\nYou cannot save the file without any Product/Voucher!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }

            //    if (trqh["fromsitenum"].ToString().Trim().CompareTo(trqh["tositenum"].ToString().Trim()) == 0)
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom site and To site cannot be same !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }


            //    #endregion

            //    #region trqh

            //    #region Check for fromsite and tosite duplication
            //    if (trqh["fromsitenum"].ToString().Trim() == trqh["tositenum"].ToString().Trim())
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom Site and To Site cannot be same", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //    }

            //    #endregion

            //    #region Validate sitenums
            //    if (BizFunctions.IsEmpty(trqh["tositenum"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nSave Unsuccessful\nTo Site Code is empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", trqh["tositenum"].ToString().Trim()))
            //        {
            //            MessageBox.Show("Save Unsuccessful\nSave Unsuccessful\nTo Site Code is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            e.Handle = false;
            //            return;
            //        }
            //    }

            //    if (BizFunctions.IsEmpty(trqh["fromsitenum"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom Site Code is empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", trqh["tositenum"].ToString().Trim()))
            //        {
            //            MessageBox.Show("Save Unsuccessful\nFrom Site Code is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            e.Handle = false;
            //            return;
            //        }
            //    }
            //    #endregion

            //    #region Validate dates
            //    if (BizFunctions.IsEmpty(trqh["senddate"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nSend Date is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }

            //    #endregion

            //    #region Validate Empty Text Box
            //    if (BizFunctions.IsEmpty(trqh["requestedby"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nRequested By cannot be empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    #endregion

            //    #endregion

            //    #region trq1
            //    foreach (DataRow dr in trq1.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {
            //            if ((int)dr["qty"] <= 0)
            //            {
            //                MessageBox.Show("Save Unsuccessful\nInvalid quantity in details!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                e.Handle = false;
            //                return;
            //            }
            //        }
            //    }
            //    #endregion trq1

            //    //Backup Validation
            //    #region Final Backup Validation

            //    if (BizValidate.CheckColumnError(dbaccess.DataSet, "trqh"))
            //    {
            //        MessageBox.Show("Invalid values detected in header", "Save unsuccessful");
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        //if (BizValidate.CheckColumnError(dbaccess.DataSet, "trq1"))
            //        //{
            //        //    MessageBox.Show("Invalid values detected in details", "Save unsuccessful");
            //        //    e.Handle = false;
            //        //    return;
            //        //}
            //    }
            //}
            #endregion
        }

        #region DocumentF2

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "trqh_fromsitenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;
                case "trqh_tositenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow trqh = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            switch (e.ControlName)
            {
                case "trqh_adhnum":
                    {

                    }
                    break;

                case "trqh_docunum":
                    e.CurrentRow["docunum"] = e.F2CurrentRow["refnum"];
                    e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                    e.CurrentRow["fromsitenum"] = e.F2CurrentRow["sitenum"];
                    e.CurrentRow["tositenum"] = "HQ";
                    e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    e.CurrentRow["contracttype"] = e.F2CurrentRow["TableName"];
                    {
                        trqh["searchType"] = "DOC";
                    }
                    break;

                case "trqh_tositenum":
                    {
                        trqh["searchType"] = "SITM";
                        e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    }
                    break;

                case "trqh_whnum":
                    {
                        trqh["searchType"] = "WHM";
                    }
                    break;

                case "trqh_sectorcode":
                    {
                        trqh["searchType"] = "SEM";
                    }
                    break;

            }
        }

        #endregion

        #region DetailF2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            switch (e.MappingName)
            {
                case "matnum":
                    e.Condition = BizFunctions.F2Condition("matnum,matname", (sender as TextBox).Text);
                    break;
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            switch (e.MappingName)
            {
                case "matname":
                    //e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    e.CurrentRow["uomcode"] = e.F2CurrentRow["uomcode"];
                    //e.CurrentRow["pcatcode"] = e.F2CurrentRow["pcatcode"];
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    //if (e.CurrentRow["barcode"] == System.DBNull.Value)
                    //{
                    //    e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    //}
                    break;
                case "barcode":
                    e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    e.CurrentRow["pcatcode"] = e.F2CurrentRow["pcatcode"];
                    if (e.CurrentRow["matnum"] == System.DBNull.Value)
                    {
                        e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    }
                    break;
            }
        }

        #endregion

        #region trq ReOpen/void

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];

            #region ori-not allow front end user to reopen and void
            //if (Tools.isFrontEnd() || Tools.isFrontEndVoucher(dbaccess, "TRQH"))
            //{
            //    // No reopen in outlet
            //    e.Handle = false;
            //    return;
            //}
            #endregion

            #region allow outlet user to reopen/void trq,but need to check whether this trq is Extracted by tri or not
            //can only void/reopen own trq.Even HQ also can't void outlets
            //if (!Tools.isFrontEnd())
            //{
            //    if (Tools.isFrontEndVoucher(dbaccess, "TRQH"))
            //    {
            //        // No reopen in outlet
            //        e.Handle = false;
            //        MessageBox.Show("Not allowed to reopen " + trqh["refnum"].ToString()+ ",which is created by outlet !", "Reopen not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    else
            //    {
            //        string strTRI = "select * from trih where trqnum='" + trqh["refnum"].ToString() + "'";
            //        DataSet dsTRI = this.dbaccess.ReadSQLTemp("tri", strTRI);
            //        if (dsTRI != null)
            //        {
            //            if (dsTRI.Tables[0].Rows.Count > 0)
            //            {
            //                e.Handle = false;
            //                MessageBox.Show(trqh["refnum"].ToString().Trim() + " has been Extracted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + " ,not allow to reopen!", "Reopen not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            e.Handle = true;
            //        }
            //    }
            //}
            ////outlet user
            //else
            //{
            //    string strTRI = "select * from trih where trqnum='" + trqh["refnum"].ToString() + "'";
            //    DataSet dsTRI = this.remoteDBAccess.ReadSQLTemp("tri", strTRI);
            //    if (dsTRI != null || dsTRI.Tables[0]!=null)
            //    {
            //        if (dsTRI.Tables[0].Rows.Count > 0)
            //        {
            //            e.Handle = false;
            //            MessageBox.Show(trqh["refnum"].ToString().Trim() + " has been Extracted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + " !", "Reopen not allowed,not allow to reopen!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        e.Handle = true;
            //    }
            //}
            #endregion
        }

        protected override void Document_Reopen_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Reopen_OnClick(sender, e);

            //if (Tools.isFrontEnd())
            //{
            //    DataRow trqh =this.dbaccess.DataSet.Tables["trqh"].Rows[0];
            //    try
            //    {
            //        // Update the backend trqh and trq1 status.
            //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trqh set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum = '" + trqh["refnum"].ToString().Trim() + "'");
            //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trq1 set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum = '" + trqh["refnum"].ToString().Trim() + "'");
            //    }
            //    catch (Exception xception)
            //    {
            //        MessageBox.Show(xception.Message, "Reopen_OnClick");
            //    }
            //}
        }

        protected override void Document_Void_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Void_Handle(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];
            #region ori-not allow front end user to void
            //if (Tools.isFrontEnd())
            //{
            //    e.Handle = false;
            //}
            #endregion

            #region allow outlet user to void trq,but need to check whether this trq is Extracted by tri or not
            //can only void/reopen own trq.Even HQ also can't void outlets
            //if (!Tools.isFrontEnd())
            //{
            //    if (Tools.isFrontEndVoucher(dbaccess, "TRQH"))
            //    {
            //        e.Handle = false;
            //        MessageBox.Show("Not allowed to void " + trqh["refnum"].ToString() + ",which is created by outlet !", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    else
            //    {
            //        string strTRI = "select * from trih where trqnum='" + trqh["refnum"].ToString() + "'";
            //        DataSet dsTRI = this.dbaccess.ReadSQLTemp("tri", strTRI);
            //        if (dsTRI != null)
            //        {
            //            if (dsTRI.Tables[0].Rows.Count > 0)
            //            {
            //                e.Handle = false;
            //                MessageBox.Show(trqh["refnum"].ToString().Trim() + " has been Extracted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + " ,not allow to void!", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            e.Handle = true;
            //        }
            //    }
            //}
            //outlet user
            //else
            //{
            //    string strTRI = "select * from trih where trqnum='" + trqh["refnum"].ToString() + "'";
            //    DataSet dsTRI = this.remoteDBAccess.ReadSQLTemp("tri", strTRI);
            //    if (dsTRI != null || dsTRI.Tables[0] != null)
            //    {
            //        if (dsTRI.Tables[0].Rows.Count > 0)
            //        {
            //            e.Handle = false;
            //            MessageBox.Show(trqh["refnum"].ToString().Trim() + " has been Extracted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + ",not allow to void!", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        e.Handle = true;
            //    }
            //}
            #endregion
        }

        protected override void Document_Void_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Void_OnClick(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];

            ////if (Tools.isFrontEnd())
            ////{
            ////    try
            ////    {
            ////        // Update the backend trqh and trq1 status.
            ////        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trqh set status='" + Common.DEFAULT_DOCUMENT_STATUSV + "' where refnum = '" + trqh["refnum"].ToString().Trim() + "'");
            ////        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trq1 set status='" + Common.DEFAULT_DOCUMENT_STATUSV + "' where refnum = '" + trqh["refnum"].ToString().Trim() + "'");
            ////    }
            ////    catch (Exception xception)
            ////    {
            ////        MessageBox.Show(xception.Message, "Void_OnClick");
            ////    }
            ////}
        }

        #endregion

        #region Duplicate Handle
        protected override void Document_Duplicate_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Duplicate_Handle(sender, e);
            //e.Handle = false;
        }
        #endregion

        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataTable trq1 = e.DBAccess.DataSet.Tables["trq1"];
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];

            #region assgin value for heaer
            if (trqh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                trqh["confirms"] = 1;
            }
            else
            {
                trqh["confirms"] = 0;
            }
            if (trqh["year"] == System.DBNull.Value)
            {
                trqh["year"] = ((DateTime)trqh["trandate"]).Year;
            }
            #endregion


        }
        #endregion

        #region Save End Event

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = e.DBAccess.DataSet.Tables["trq1"];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];

            string matnums = String.Empty;

            if (trqh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {

            }
        }

        # endregion

        #region Document Button Events

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow trqh = dbaccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = dbaccess.DataSet.Tables["trq1"];
            setDefaults(dbaccess.DataSet, "trqh/trq1");

            #region update ttqty,ttamt in header and pull latest dct info
            decimal ttqty = 0;
            foreach (DataRow dr1 in trq1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    #region update ttqty,ttamt in header
                    dr1["qty"] = dr1["trqqty"];
                    if (dr1["qty"] != System.DBNull.Value)
                    {
                        dr1["retail"] = GetLatestMatnumCost(dr1["matnum"].ToString(), Convert.ToDateTime(trqh["trandate"]));
                        dr1["itemRetailTotal"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["retail"]);

                        dr1["detail"] = dr1["matname"];
                        if ((int)dr1["qty"] < 0)
                        {
                            MessageBox.Show("Invalid quantity");
                            dr1.SetColumnError("qty", "Invalid quantity");
                        }

                    }
                    ttqty += (int)dr1["qty"];
                    #endregion

                    if (BizFunctions.IsEmpty(dr1["sitenum"]) && !BizFunctions.IsEmpty(dr1["docunum"]))
                    {
                        dr1["sitenum"] = GetSitenum(dr1["docunum"].ToString());
                        if (!BizFunctions.IsEmpty(dr1["sitenum"]) && BizFunctions.IsEmpty(dr1["sectorcode"]))
                        {
                            dr1["sectorcode"] = GetZone(dr1["sitenum"].ToString());
                        }
                    }




                    if (!BizFunctions.IsEmpty(dr1["matnum"]))
                    {
                        dr1["lasttotalqty"] = previousQtyRequest(dr1["matnum"].ToString().Trim());
                        dr1["senddate"] = previousQtyRequest(dr1["matnum"].ToString().Trim(), (DateTime)trqh["trandate"]);
                        if (BizFunctions.IsEmpty(dr1["approveqty"]) || (Convert.ToDecimal(dr1["approveqty"]) == 0 && Convert.ToDecimal(dr1["trqqty"]) > 0))
                        {
                            dr1["approveqty"] = dr1["trqqty"];
                        }
                    }
                    dr1["trandate"] = trqh["trandate"];
                }
            }
            trqh["ttqty"] = ttqty;
            #endregion

            if (!BizFunctions.IsEmpty(trqh["fromsitenum"]))
            {
                trqh["fromsitename"] = GetSitename(trqh["fromsitenum"].ToString().Trim());
            }

            if (!BizFunctions.IsEmpty(trqh["tositenum"]))
            {
                trqh["tositename"] = GetSitename(trqh["tositenum"].ToString().Trim());
            }

            if (trq1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in trq1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["approveqty"]))
                        {
                            dr1["approveqty"] = dr1["trqqty"];
                        }
                        else if (Convert.ToDecimal(dr1["approveqty"]) == 0 && !BizFunctions.IsEmpty(dr1["trqqty"]))
                        {
                            if (Convert.ToDecimal(dr1["trqqty"]) > 0)
                            {
                                dr1["approveqty"] = dr1["trqqty"];
                            }
                        }
                    }
                }
            }
        }

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);
            DataRow trqh = dbaccess.DataSet.Tables["trqh"].Rows[0];
            string result = "Please check the following:";

            if (BizFunctions.IsEmpty(trqh["approvestatus"]) == true || trqh["approvestatus"].ToString() == "")
            {
                result = result + "\n Not Approve Yet!";
            }
            if (BizFunctions.IsEmpty(trqh["ApplicationType"]) == true || trqh["ApplicationType"].ToString() == "")
            {
                result = result + "\n Please Choose Req Type!";
            }

            if (result != "Please check the following:")
            {
                MessageBox.Show(result, "Confirm Unsuccessful");
                e.Handle = false;
            }
            else
            {
                if (Tools.isFrontEnd())
                {
                    try
                    {
                        //System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US",false);

                        DataSet dstmp = remoteDBAccess.ReadSQLTemp("ping", "select top 1 id from mwt" + Common.DEFAULT_SYSTEM_YEAR);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cannot confirm because connection to HQ is down. " + System.Environment.NewLine +
                                        "Try again when connection is up" + System.Environment.NewLine + ex.Message, "Connection Down");
                        e.Handle = false;
                        return;
                    }
                }
            }
        }

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];

            if (trqh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "trqh/trq1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = e.DBAccess.DataSet.Tables["trq1"];

            Hashtable selectedCollection = new Hashtable();
            String commandfrom = null;
            String commandto = null;
            String commandtrqh = null;
            String commandtrq1 = null;

            selectedCollection.Clear();
            command = "select * from coy";
            selectedCollection.Add("coy", command);

            commandfrom = "select * from sitm where sitenum = '" + trqh["fromsitenum"].ToString() + "'";
            commandto = "select * from sitm where sitenum = '" + trqh["tositenum"].ToString() + "'";
            commandtrqh = "select * from trqh where refnum='" + trqh["refnum"].ToString() + "'";
            commandtrq1 = "select * from trq1 where refnum='" + trqh["refnum"].ToString() + "'";

            string matnums = String.Empty;

            foreach (DataRow dr in trq1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (matnums == String.Empty)
                        matnums = "'" + dr["matnum"].ToString().Trim() + "'";
                    else
                        matnums = matnums + ",'" + dr["matnum"].ToString().Trim() + "'";
                }
            }
            selectedCollection.Add("matm", "Select matnum,matname from matm where matnum in (" + matnums + ")");
            selectedCollection.Add("sitmFrom", commandfrom);
            selectedCollection.Add("sitmTo", commandto);
            selectedCollection.Add("trqh", commandtrqh);
            selectedCollection.Add("trq1", commandtrq1);
            e.DBAccess.ReadSQL(selectedCollection);

            BizFunctions.SetCoyForPrinting(e.DBAccess, "ID=1");

            e.ReportSource.PrintOptions.PaperSize = PaperSize.PaperA4;
            e.DataSource = e.DBAccess.DataSet;
        }

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = e.DBAccess.DataSet.Tables["trq1"];
            if (trqh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "trqh/trq1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }

        #endregion

        #region Check DatagridError Funtion
        private int checkdatagridError(DataRow dr)
        {
            DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT (matnum) FROM [trq1] WHERE matnum = '" + dr["matnum"] + "'");

            if (dt.Rows.Count > 1)
            {
                return 1;
            }
            else
                return 0;
        }
        #endregion

        #region Check stockonhand negative error
        private int checkstockonhanderror(DataRow dr)
        {
            if (Convert.ToDecimal(dr["stockonhand"].ToString().Trim()) <= 0)
            {
                return 2;
            }
            else
                return 0;
        }
        #endregion

        #region Set Error Message
        private void seterrorMessage(DataRow dr, int check)
        {
            if (check == 0)
            {
                dr.SetColumnError("matnum", "");
            }

            if (check == 1)
            {
                dr.SetColumnError("matnum", "Duplicate item ");
            }

            if (check == 2)
            {
                dr.SetColumnError("qty", "Stock on hand is negative or equal to zero balance");
            }

            DataRow[] rows = dbaccess.DataSet.Tables["trq1"].Select("matnum ='" + dr["matnum"].ToString().Trim() + "'");

            if (rows.Length > 0)
                rows[0].SetColumnError("matnum", "");

        }
        #endregion

        #region Import/Export Excel
        private DataTable getdata(int linenum, string target, string filename)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataRow dr;
            dt.Columns.Add("column0");
            dr = dt.NewRow();
            TextReader sr = File.OpenText(filename);
            string newtext = null;
            for (int i = 1; i <= linenum; i++)
            {
                newtext = sr.ReadLine();
            }
            Regex re = new Regex(target);
            Match m = re.Match(newtext);
            if (m.Success)
            {
                newtext = newtext.Substring(0, m.Index);
                MatchCollection mc;
                MatchCollection msc;
                // Delimiter - \t means tab

                Regex r = new Regex(",");
                Regex rs = new Regex(",");
                mc = r.Matches(newtext);
                int num = mc.Count - 1; //get column postion

                while ((newtext = sr.ReadLine()) != null)
                {
                    dr = dt.NewRow();

                    msc = rs.Matches(newtext);
                    if (msc[num].Index - msc[num + 1].Index == 1)
                    {
                        dr[0] = "";
                    }
                    else
                    {
                        dr[0] = newtext.Substring(msc[num].Index + 1, msc[num + 1].Index - msc[num].Index - 1);
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        protected void btn_import_Click(object sender, System.EventArgs e)
        {
            string defaultPath = Environment.CurrentDirectory;
            System.Windows.Forms.OpenFileDialog openfile = new OpenFileDialog();
            openfile.InitialDirectory = @"c:\";
            openfile.DefaultExt = "TXT";
            openfile.Filter = "XLS(*.xls)|*.xls|TXT(*.txt)|*.txt";
            DataTable dt = new DataTable();

            if (openfile.ShowDialog() == DialogResult.OK)
            {
                System.Data.DataColumn cfirst = new System.Data.DataColumn("matnum");
                System.Data.DataColumn csecond = new System.Data.DataColumn("qty");
                System.Data.DataTable dt1 = new System.Data.DataTable();
                System.Data.DataTable dt2 = new System.Data.DataTable();
                System.Data.DataRow dr = dbaccess.DataSet.Tables["trq1"].NewRow();

                try
                {
                    dt1 = getdata(2, "Product Code", openfile.FileName);
                    dt2 = getdata(2, "Total Quantity Shipped", openfile.FileName);

                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        dr = dbaccess.DataSet.Tables["trq1"].NewRow();
                        dr["matnum"] = dt1.Rows[i][0];
                        dr["qty"] = dt2.Rows[i][0];
                        dbaccess.DataSet.Tables["trq1"].Rows.Add(dr);
                    }
                    MessageBox.Show("Import process is complete !");
                }
                catch
                {
                    MessageBox.Show("Error style in the txt, import process is not complete !");
                }
                Environment.CurrentDirectory = defaultPath;
            }
        }

        void btnExport_Click(object sender, EventArgs e)
        {
            string defaultPath = Environment.CurrentDirectory;
            try
            {
                DataTable trq1ToExcel = new DataTable();
                trq1ToExcel.Columns.Add("BarCode");
                trq1ToExcel.Columns.Add("ProductCode");
                trq1ToExcel.Columns.Add("ProductName");
                trq1ToExcel.Columns.Add("UOM");
                trq1ToExcel.Columns.Add("ProductCategory");
                trq1ToExcel.Columns.Add("QTY");
                foreach (DataRow dr in dbaccess.DataSet.Tables["trq1"].Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow newRow = trq1ToExcel.NewRow();
                        newRow["BarCode"] = dr["barcode"];
                        newRow["ProductCode"] = dr["matnum"];
                        newRow["ProductName"] = dr["detail"];
                        newRow["UOM"] = dr["uom"];
                        newRow["ProductCategory"] = dr["pcatcode"];
                        newRow["QTY"] = dr["qty"];
                        trq1ToExcel.Rows.Add(newRow);
                    }
                }

                System.Windows.Forms.SaveFileDialog savefile = new SaveFileDialog();
                savefile.DefaultExt = "xls";
                savefile.Filter = "XLS(*.xls)|*.xls|TXT(*.txt)|*.txt";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    DataTableToExcel(trq1ToExcel, savefile.FileName);
                    MessageBox.Show("The data has been exported successfully!", "ADU - Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Environment.CurrentDirectory = defaultPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void DataTableToExcel(DataTable tmpDataTable, string fileName)
        {
            if (fileName == null)
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
            xlApp.DefaultFilePath = "";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
            xlApp.Cells.NumberFormat = "@";

            for (int i = 0; i < tmpDataTable.Columns.Count; i++)
            {
                xlApp.Cells[1, i + 1] = tmpDataTable.Columns[i].ColumnName.ToString();
            }

            for (int i = 0; i < tmpDataTable.Rows.Count; i++)
            {
                for (int j = 0; j < tmpDataTable.Columns.Count; j++)
                {
                    xlApp.Cells[i + 2, j + 1] = tmpDataTable.Rows[i][j].ToString();
                }
            }
            xlBook.SaveCopyAs(fileName);
        }
        #endregion

        #region Scan function
        private void txt_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (txt_barcode.Text != "")
            {
                if (e.KeyCode == Keys.Enter)
                {
                    AppendBarcode();
                    txt_barcode.Text = "";
                }
            }
        }

        private void AppendBarcode()
        {
            # region Variables
            DataTable trq1 = this.dbaccess.DataSet.Tables["trq1"];
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];
            # endregion
            DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from [trq1]");
            DataTable tmp = null;
            string cmd1 = "select * from matm where barcode='" + txt_barcode.Text + "'";
            DataSet tmpds1 = this.dbaccess.ReadSQLTemp("matm", cmd1);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted)
                    {
                        tmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from [trq1] where barcode='" + txt_barcode.Text + "'");
                        if (tmp.Rows.Count == 0)
                        {
                            DataRow drTrr1 = trq1.NewRow();
                            drTrr1["barcode"] = txt_barcode.Text;
                            drTrr1["qty"] = 1;
                            if (tmpds1.Tables["matm"].Rows.Count != 0)
                            {
                                drTrr1["detail"] = tmpds1.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                                drTrr1["matnum"] = tmpds1.Tables["matm"].Rows[0]["matnum"].ToString().Trim();
                                drTrr1["uom"] = tmpds1.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                                drTrr1["pcatcode"] = tmpds1.Tables["matm"].Rows[0]["pcatcode"];
                            }
                            trq1.Rows.Add(drTrr1);
                        }
                        else
                        {
                            if (dt.Rows[i]["barcode"].ToString().Trim() == txt_barcode.Text)
                            {
                                trq1.Rows[i]["qty"] = Convert.ToInt32(trq1.Rows[i]["qty"]) + 1;
                            }
                        }
                    }
                }
            }
            else
            {
                DataRow drTrr1 = trq1.NewRow();
                drTrr1["barcode"] = txt_barcode.Text;
                drTrr1["qty"] = 1;
                if (tmpds1.Tables["matm"].Rows.Count != 0)
                {
                    drTrr1["detail"] = tmpds1.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                    drTrr1["matnum"] = tmpds1.Tables["matm"].Rows[0]["matnum"].ToString().Trim();
                    drTrr1["uom"] = tmpds1.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                    drTrr1["pcatcode"] = tmpds1.Tables["matm"].Rows[0]["pcatcode"];
                }
                trq1.Rows.Add(drTrr1);
            }
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

        #region Extract
        private void btn_ExtractMATM_Click(object sender, EventArgs e)
        {
            DataRow trqh = dbaccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = dbaccess.DataSet.Tables["trq1"];

            Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            DataTable oriTable = trq1;
            try
            {
                // Open Extract Form

                ExtractMATM.ExtractMATM Extract = new ExtractMATM.ExtractMATM(this.dbaccess, oriTable);
                Extract.ShowDialog(frm);
                #region assign line number for pon1
                int line = 100;
                foreach (DataRow dr in trq1.Rows)
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

        #region Get Template by Contrict

        private void GetByCtr(string ctrNum)
        {

            DataRow TRQH = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            DataTable TRQ1 = this.dbaccess.DataSet.Tables["TRQ1"];
            DataTable TRQ2 = this.dbaccess.DataSet.Tables["TRQ2"];

            if (TRQ1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ1);
            }
            if (TRQ2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ2);
            }

            string getCtr = "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR3 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR7 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR5 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR8 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR9 WHERE refnum='" + ctrNum + "' AND [status]<>'V'";




            this.dbaccess.ReadSQL("MatTB", getCtr);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];

            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRQ1 = TRQ1.NewRow();
                        InsertTRQ1["docunum"] = dr1["refnum"];
                        InsertTRQ1["matnum"] = dr1["matnum"];
                        InsertTRQ1["detail"] = dr1["matname"];
                        InsertTRQ1["trqqty"] = dr1["qty"];
                        InsertTRQ1["stdqty"] = dr1["qty"];
                        InsertTRQ1["qty"] = dr1["qty"];
                        TRQ1.Rows.Add(InsertTRQ1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where refnum='" + TRQH["docunum"].ToString().Trim() + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRQ2 = TRQ2.NewRow();
                        InsertTRQ2["docunum"] = dr2["refnum"];
                        InsertTRQ2["tositenum"] = dr2["sitenum"];
                        InsertTRQ2["sectorcode"] = dr2["sectorcode"];
                        TRQ2.Rows.Add(InsertTRQ2);
                    }
                }
            }


        }

        #endregion

        #region Get Template by Adhoc

        private void GetByAdHoc(string adhocNum)
        {

            DataRow TRQH = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            DataTable TRQ1 = this.dbaccess.DataSet.Tables["TRQ1"];
            DataTable TRQ2 = this.dbaccess.DataSet.Tables["TRQ2"];

            if (TRQ1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ1);
            }
            if (TRQ2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ2);
            }

            string getADH = "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH3 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH7 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH5 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH8 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH9 WHERE refnum='" + adhocNum + "' AND [status]<>'V' ";




            this.dbaccess.ReadSQL("MatTB", getADH);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];

            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRQ1 = TRQ1.NewRow();
                        InsertTRQ1["docunum"] = dr1["refnum"];
                        InsertTRQ1["matnum"] = dr1["matnum"];
                        InsertTRQ1["detail"] = dr1["matname"];
                        InsertTRQ1["trqqty"] = dr1["qty"];
                        InsertTRQ1["stdqty"] = dr1["qty"];
                        InsertTRQ1["qty"] = dr1["qty"];
                        TRQ1.Rows.Add(InsertTRQ1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where refnum='" + TRQH["docunum"].ToString().Trim() + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRQ2 = TRQ2.NewRow();
                        InsertTRQ2["docunum"] = dr2["refnum"];
                        InsertTRQ2["tositenum"] = dr2["sitenum"];
                        InsertTRQ2["sectorcode"] = dr2["sectorcode"];
                        TRQ2.Rows.Add(InsertTRQ2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by By Sitenum

        private void GetBySitm(string sitenum)
        {

            DataRow TRQH = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            DataTable TRQ1 = this.dbaccess.DataSet.Tables["TRQ1"];
            DataTable TRQ2 = this.dbaccess.DataSet.Tables["TRQ2"];

            if (TRQ1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ1);
            }
            if (TRQ2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ2);
            }

            string getSite = "SELECT " +
                                    "C1.REFNUM, " +
                                    "C1.SITENUM, " +
                                    "A.MATNUM, " +
                                    "A.MATNAME, " +
                                    "A.QTY " +
                                "FROM " +
                                "( " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR3 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR7 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR5 WHERE   [status]<>'V' " +
                                    "UNION " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR8 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR9 WHERE   [status]<>'V' " +
                                ")A " +
                                "LEFT JOIN CTRH C1 ON A.refnum= C1.refnum " +
                                "WHERE C1.sitenum='" + sitenum + "' " +
                                "UNION " +
                                "SELECT  " +
                                    "A1.REFNUM, " +
                                    "A1.SITENUM, " +
                                    "B.MATNUM, " +
                                    "B.MATNAME, " +
                                    "B.QTY " +
                                "FROM " +
                                "( " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH3 WHERE   [status]<>'V' " +
                                    "UNION " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH7 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH5 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH8 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH9 WHERE   [status]<>'V' " +
                                 ")B " +
                                    "LEFT JOIN ADH A1  ON B.refnum= A1.refnum " +
                                "WHERE A1.sitenum='" + sitenum + "'";




            this.dbaccess.ReadSQL("MatTB", getSite);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];


            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRQ1 = TRQ1.NewRow();
                        InsertTRQ1["docunum"] = dr1["refnum"];
                        InsertTRQ1["matnum"] = dr1["matnum"];
                        InsertTRQ1["detail"] = dr1["matname"];
                        InsertTRQ1["trqqty"] = dr1["qty"];
                        InsertTRQ1["stdqty"] = dr1["qty"];
                        InsertTRQ1["qty"] = dr1["qty"];
                        TRQ1.Rows.Add(InsertTRQ1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where sitenum='" + TRQH["tositenum"].ToString().Trim() + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRQ2 = TRQ2.NewRow();
                        InsertTRQ2["docunum"] = dr2["refnum"];
                        InsertTRQ2["tositenum"] = dr2["sitenum"];
                        InsertTRQ2["sectorcode"] = dr2["sectorcode"];
                        TRQ2.Rows.Add(InsertTRQ2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by Sector

        private void GetBySectorCode(string sectorcode)
        {

            DataRow TRQH = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            DataTable TRQ1 = this.dbaccess.DataSet.Tables["TRQ1"];
            DataTable TRQ2 = this.dbaccess.DataSet.Tables["TRQ2"];

            if (TRQ1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ1);
            }
            if (TRQ2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRQ2);
            }

            string getSite = "SELECT " +
                                    "C1.REFNUM, " +
                                    "C1.SITENUM, " +
                                    "C1.SECTORCODE, " +
                                    "A.MATNUM, " +
                                    "A.MATNAME, " +
                                    "A.QTY " +
                                "FROM " +
                                "( " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR3 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR7 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR5 WHERE   [status]<>'V' " +
                                    "UNION " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR8 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR9 WHERE   [status]<>'V' " +
                                ")A " +
                                "LEFT JOIN CTRH C1 ON A.refnum= C1.refnum " +
                                "WHERE C1.sectorcode='" + sectorcode + "' " +
                                "UNION " +
                                "SELECT  " +
                                    "C2.REFNUM, " +
                                    "C2.SITENUM, " +
                                    "C2.SECTORCODE, " +
                                    "B.MATNUM, " +
                                    "B.MATNAME, " +
                                    "B.QTY " +
                                "FROM " +
                                "( " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH3 WHERE   [status]<>'V' " +
                                    "UNION " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH7 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH5 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH8 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH9 WHERE   [status]<>'V' " +
                                 ")B " +
                                    "LEFT JOIN CTRH C2 ON B.refnum= C2.refnum " +
                                "WHERE C2.sectorcode='" + sectorcode + "'";




            this.dbaccess.ReadSQL("MatTB", getSite);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];


            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRQ1 = TRQ1.NewRow();
                        InsertTRQ1["docunum"] = dr1["refnum"];
                        InsertTRQ1["sectorcode"] = dr1["sectorcode"];
                        InsertTRQ1["sitenum"] = dr1["sitenum"];
                        InsertTRQ1["matnum"] = dr1["matnum"];
                        InsertTRQ1["detail"] = dr1["matname"];
                        InsertTRQ1["trqqty"] = dr1["qty"];
                        InsertTRQ1["stdqty"] = dr1["qty"];
                        InsertTRQ1["qty"] = dr1["qty"];
                        TRQ1.Rows.Add(InsertTRQ1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where sectorcode='" + sectorcode + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRQ2 = TRQ2.NewRow();
                        InsertTRQ2["docunum"] = dr2["refnum"];
                        InsertTRQ2["tositenum"] = dr2["sitenum"];
                        InsertTRQ2["sectorcode"] = dr2["sectorcode"];
                        TRQ2.Rows.Add(InsertTRQ2);
                    }
                }
            }

        }

        #endregion

        #region Header Extract Template Button Event

        void btn_ExtractTemplate_Click(object sender, EventArgs e)
        {
            DataRow TRQH = dbaccess.DataSet.Tables["TRQH"].Rows[0];
            DataTable TRQ1 = dbaccess.DataSet.Tables["TRQ1"];
            if (!BizFunctions.IsEmpty(TRQH["searchType"]))
            {
                if (TRQH["searchType"].ToString() == "DOC")
                {
                    if (!BizFunctions.IsEmpty(TRQH["docunum"]))
                    {
                        if (TRQH["contracttype"].ToString() == "CTRH")
                        {
                            GetByCtr(TRQH["docunum"].ToString());
                        }
                        if (TRQH["contracttype"].ToString() == "ADH")
                        {
                            GetByAdHoc(TRQH["docunum"].ToString());
                        }
                    }
                }
                else if (TRQH["searchType"].ToString() == "SITM")
                {
                    GetBySitm(TRQH["fromsitenum"].ToString());
                }
                else if (TRQH["searchType"].ToString() == "WHM")
                {
                    GetBySitm(TRQH["fromsitenum"].ToString());
                }
                else if (TRQH["searchType"].ToString() == "SEM")
                {
                    GetBySectorCode(TRQH["sectorcode"].ToString().Trim());
                }
            }

            if (TRQ1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TRQ1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["matnum"]))
                        {
                            dr1["lasttotalqty"] = previousQtyRequest(dr1["matnum"].ToString().Trim());
                        }
                        if (!BizFunctions.IsEmpty(dr1["stdqty"]))
                        {
                            if (Convert.ToDecimal(dr1["stdqty"]) < 1)
                            {
                                dr1["stdqty"] = 1;
                            }
                            else
                            {
                                dr1["stdqty"] = Math.Round(Convert.ToDecimal(dr1["stdqty"]));
                            }
                        }
                        if (!BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            if (Convert.ToDecimal(dr1["qty"]) < 1)
                            {
                                dr1["qty"] = 1;
                            }
                            else
                            {
                                dr1["qty"] = Math.Round(Convert.ToDecimal(dr1["qty"]));
                            }
                        }
                        if (!BizFunctions.IsEmpty(dr1["trqqty"]))
                        {
                            if (Convert.ToDecimal(dr1["trqqty"]) < 1)
                            {
                                dr1["trqqty"] = 1;
                            }
                            {
                                dr1["trqqty"] = Math.Round(Convert.ToDecimal(dr1["trqqty"]));
                            }
                        }
                    }
                }
            }

        }

        #endregion

        #region Query Outlet List
        // Query each and every outlet and display stock remaining.
        void btn_queryoutlets_Click(object sender, EventArgs e)
        {
            queryoutlets queryWindow = new queryoutlets(dbaccess);
        }

        #endregion


        private decimal GetLatestMatnumCost(string matnum, DateTime dt)
        {
            decimal amount = 0;

            string GetAmout = "SELECT " +
                                    "retail " +
                                "FROM " +
                                "( " +
                                "select  " +
                                    "retail, " +
                                    "ROW_NUMBER() OVER (Order BY effectivedate) as ForTop,ROW_NUMBER() OVER (Order BY effectivedate Desc) as ForBottom  " +
                                "from matm1  " +
                                "where effectivedate<='" + BizFunctions.GetSafeDateString(dt) + "' " +
                                "and matnum ='" + matnum + "'  " +
                                ")A  " +
                                "WHERE ForBottom=1";

            this.dbaccess.ReadSQL("tempPrice", GetAmout);

            DataTable tempPrice = this.dbaccess.DataSet.Tables["tempPrice"];
            if (tempPrice != null)
            {
                if (tempPrice.Rows.Count > 0)
                {
                    DataRow dr1 = tempPrice.Rows[0];
                    if (BizFunctions.IsEmpty(dr1["retail"]))
                    {
                        dr1["retail"] = 0;
                    }
                    amount = Convert.ToDecimal(dr1["retail"]);
                }
            }
            tempPrice.Dispose();

            return amount;
        }

        private string GetSitename(string sitenum)
        {
            string siteName = "";
            string strSitename = "Select sitename from SITM where sitenum='" + sitenum + "'";

            this.dbaccess.ReadSQL("tmpSITM", strSitename);

            DataTable sitm = this.dbaccess.DataSet.Tables["tmpSITM"];

            if (sitm != null)
            {
                if (sitm.Rows.Count > 0)
                {
                    siteName = sitm.Rows[0]["sitename"].ToString();
                }
            }

            return siteName;
        }

        private string GetZone(string sitenum)
        {
            string Zone = "";
            string strZone = "Select sectorcode from SITMTB where sitenum='" + sitenum + "'";

            DataTable tbZone = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, strZone);

            if (tbZone != null)
            {
                if (tbZone.Rows.Count > 0)
                {
                    Zone = tbZone.Rows[0]["sectorcode"].ToString();
                }
            }
            return Zone;

        }

        private string GetSitenum(string refnum)
        {
            string Site = "";
            string strSite = "Select sitenum from DOCTBALL where refnum='" + refnum + "'";

            DataTable tbSite = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, strSite);

            if (tbSite != null)
            {
                if (tbSite.Rows.Count > 0)
                {
                    Site = tbSite.Rows[0]["sitenum"].ToString();
                }
            }
            return Site;
        }
    }
}

