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
using System.Web.UI;
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
        protected Button btn_ExtractMATM, btn_ExtractTemplate, btn_Refresh_Price = null;
        private ComboBox trqh_trqyear, trqh_trqmonth = null;

        protected Label lblDisplay = null;


        bool isLiveEmail = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("IsLiveEmail"));

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

            btn_Refresh_Price = BizXmlReader.CurrentInstance.GetControl(detailName, "btn_Refresh_Price") as Button;
            btn_Refresh_Price.Click += new EventHandler(btn_Refresh_Price_Click);

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

            if (BizFunctions.IsEmpty(trqh["whnum"]))
            {
                trqh["whnum"] = BizLogicTools.Tools.GetWhnum(trqh["fromsitenum"].ToString(), this.dbaccess);
            }
       
        }

        void btn_Refresh_Price_Click(object sender, EventArgs e)
        {
            DataTable trq1 = this.dbaccess.DataSet.Tables["trq1"];

            if (trq1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in trq1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["retail"] = BizLogicTools.Tools.GetMatmStdCost(dr1["matnum"].ToString(), this.dbaccess);
                    }
                }
            }
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
                if (BizFunctions.IsEmpty(trqh["trqyear"]))
                {
                    trqh["trqyear"] = Convert.ToDateTime(trqh["senddate"]).Year;
                }
                else  if (Convert.ToInt16(trqh["trqyear"]) == 0)
                {
                    trqh["trqyear"] = Convert.ToDateTime(trqh["senddate"]).Year;
                }
            }
            else
            {

                if (Convert.ToInt16(trqh["trqyear"]) == 0)
                {
                    trqh["trqyear"] = Convert.ToDateTime(trqh["senddate"]).Year;
                }
                
            }

        }


        private void setTrqMonth()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["TRQH"].Rows[0];
            DateTime dt = (DateTime)trqh["trandate"];
            //   dt = (DateTime)trqh["trandate"];


            if (!BizFunctions.IsEmpty(trqh["senddate"]))
            {
                if (BizFunctions.IsEmpty(trqh["trqmonth"]))
                {
                    trqh["trqmonth"] = Convert.ToDateTime(trqh["senddate"]).Month;
                }
                else
                {
                    if (Convert.ToInt16(trqh["trqmonth"]) == 0)
                    {
                        trqh["trqmonth"] = Convert.ToDateTime(trqh["senddate"]).Month;
                    }
                }
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

            string getPreviousQty = "select (t1.matnum) , traqty as previousQtyRequest,trandate from tra1  t1 where t1.tositenum='" + trqh["fromsitenum"] + "' " +
" and t1.matnum='" + matnum + "'  and docunum<>'" + trqh["refnum"] + "' and trandate<'" + BizFunctions.GetSafeDateString((DateTime)trqh["trandate"]) + "' order by trandate desc";

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

            string getPreviousQty = "select (t1.matnum) , traqty as previousQtyRequest,trandate from tra1  t1 where t1.tositenum='" + trqh["fromsitenum"] + "' " +
" and t1.matnum='" + matnum + "'  and docunum<>'" + trqh["refnum"] + "' and trandate<'" + BizFunctions.GetSafeDateString((DateTime)trqh["trandate"]) + "' order by trandate desc";

            this.dbaccess.ReadSQL("TempMatnumQty", getPreviousQty);

            DataTable TempMatnumQty = this.dbaccess.DataSet.Tables["TempMatnumQty"];

            if (TempMatnumQty.Rows.Count > 0)
            {
                previousdate = (DateTime)(TempMatnumQty.Rows[0]["trandate"]);
            }
            return previousdate;
        }

        private decimal previousPrice(string matnum)
        {
            decimal previousQty = 0;
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];

            string getPreviousQty = "select top 1 price  from por1 where matnum='"+matnum+"' and status<>'V' order by trandate desc";
            this.dbaccess.ReadSQL("TempMatnumprice", getPreviousQty);
            DataTable TempMatnumPr = this.dbaccess.DataSet.Tables["TempMatnumprice"];

            string getPreviousQty1 = "select MAX( dpriceConvert) dpriceConvert  from matm2 where matnum ='"+matnum+"'";
            this.dbaccess.ReadSQL("TempMatnumprice1", getPreviousQty1);
            DataTable TempMatnumPr1 = this.dbaccess.DataSet.Tables["TempMatnumprice1"];

            if (TempMatnumPr.Rows.Count > 0)
            {
                previousQty = Convert.ToDecimal(TempMatnumPr.Rows[0]["price"]);
            }
            else if (TempMatnumPr1.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(TempMatnumPr1.Rows[0]["dpriceConvert"]))
                previousQty = Convert.ToDecimal(TempMatnumPr1.Rows[0]["dpriceConvert"]);
                        
            }
            return previousQty;
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

                        if(!BizFunctions.IsEmpty(e.Row["matnum"]))
                        {
                            if (BizFunctions.IsEmpty(e.Row["whnum"]))
                            {
                                e.Row["whnum"] = trqh["fromsitenum"];
                            }
                        }
                        #endregion
                        break;


                    case "sitenum":
                        {
                            if (!BizFunctions.IsEmpty(e.Row["sitenum"]))
                            {
                                if (BizFunctions.IsEmpty(e.Row["whnum"]))
                                {
                                    e.Row["whnum"] = trqh["sitenum"];
                                }
                            }
                        }
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

            if(BizFunctions.IsEmpty(trqh["ccnum"]))
            {
                trqh["ccnum"] = "CLN";
            }
            foreach (DataRow dr1 in trq1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr1["ccnum"]))
                    {
                        dr1["ccnum"] = trqh["ccnum"];
                    }
                }
            }

            if (BizFunctions.IsEmpty(trqh["projectid"]))
            {
                MessageBox.Show("Please state the ProjectID ", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                e.Handle = false;
            }


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

        private void CalculateCost()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = this.dbaccess.DataSet.Tables["trq1"];

            if (trq1.Rows.Count > 0)
            {
                decimal totalretailcost = 0;
                decimal month1cost = 0;
                decimal month2cost = 0;
                decimal month3cost = 0;
                decimal totalAllMonthCost = 0;
                decimal totalApproveCost = 0;

                foreach (DataRow dr1 in trq1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["month0cost"] = PreviousMonthCost(dr1["matnum"].ToString(), 0);
                        dr1["month1cost"] = PreviousMonthCost(dr1["matnum"].ToString(), -1);
                        dr1["month2cost"] = PreviousMonthCost(dr1["matnum"].ToString(), -2);
                        dr1["month3cost"] = PreviousMonthCost(dr1["matnum"].ToString(), -3);

                        int[,] arrayDatesQty0 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]),  0);
                        int[,] arrayDatesQty1 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]), -1);
                        int[,] arrayDatesQty2 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]), -2);
                        int[,] arrayDatesQty3 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]), -3);

                        dr1["month1qty"] = PreviousMonthQty(dr1["matnum"].ToString(), arrayDatesQty0[0, 0], arrayDatesQty0[0, 1]);
                        dr1["month1qty"] = PreviousMonthQty(dr1["matnum"].ToString(), arrayDatesQty1[0, 0], arrayDatesQty1[0, 1]);
                        dr1["month2qty"] = PreviousMonthQty(dr1["matnum"].ToString(), arrayDatesQty2[0, 0], arrayDatesQty2[0, 1]);
                        dr1["month3qty"] = PreviousMonthQty(dr1["matnum"].ToString(), arrayDatesQty3[0, 0], arrayDatesQty3[0, 1]);

                        if (BizFunctions.IsEmpty(dr1["retail"]))
                        {
                            dr1["retail"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["month1cost"]))
                        {
                            dr1["month1cost"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["month2cost"]))
                        {
                            dr1["month2cost"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["month3cost"]))
                        {
                            dr1["month3cost"] = 0;
                        }

                        dr1["totalAllMonthCost"] = Convert.ToDecimal(dr1["retail"]) + Convert.ToDecimal(dr1["month1cost"]) + Convert.ToDecimal(dr1["month2cost"]) + Convert.ToDecimal(dr1["month3cost"]);

                        if (BizFunctions.IsEmpty(dr1["month1cost"]))
                        {
                            dr1["retail"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["month1cost"]))
                        {
                            dr1["month1cost"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["month2cost"]))
                        {
                            dr1["month2cost"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["month3cost"]))
                        {
                            dr1["month3cost"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["totalAllMonthCost"]))
                        {
                            dr1["totalAllMonthCost"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["approveqty"]))
                        {
                            dr1["approveqty"] = 0;
                        }
                        totalretailcost = totalretailcost + Convert.ToDecimal(dr1["retail"]);
                        month1cost = month1cost + Convert.ToDecimal(dr1["month1cost"]);
                        month2cost = month2cost + Convert.ToDecimal(dr1["month2cost"]);
                        month3cost = month3cost + Convert.ToDecimal(dr1["month3cost"]);
                        totalAllMonthCost = totalAllMonthCost + Convert.ToDecimal(dr1["totalAllMonthCost"]);
                        totalApproveCost = totalApproveCost + (Convert.ToDecimal(dr1["retail"]) * Convert.ToDecimal(dr1["approveqty"]));

                        dr1["sitenum"] = trqh["fromsitenum"];
                        if (BizFunctions.IsEmpty(dr1["whnum"]) && !BizFunctions.IsEmpty(trqh["whnum"]))
                        {
                            dr1["whnum"] = trqh["whnum"];
                        }
                        if (!BizFunctions.IsEmpty(dr1["sitenum"]) && BizFunctions.IsEmpty(dr1["whnum"]))
                        {
                            dr1["whnum"] = trqh["sitenum"];
                        }


                    }
                }

                int[,] arrayDates0 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]), 0);
                int[,] arrayDates1 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]), -1);
                int[,] arrayDates2 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]), -2);
                int[,] arrayDates3 = ATL.TimeUtilites.TimeTools.GetMonthYear(Convert.ToInt32(trqh["trqmonth"]), Convert.ToInt32(trqh["trqyear"]), -3);

     
                trqh["totalmonth0cost"] = PreviousMonthCost2(arrayDates0[0, 0], arrayDates0[0, 1]);
                trqh["totalmonth1cost"] = PreviousMonthCost2(arrayDates1[0, 0], arrayDates1[0, 1]);
                trqh["totalmonth2cost"] = PreviousMonthCost2(arrayDates2[0, 0], arrayDates2[0, 1]);
                trqh["totalmonth3cost"] = PreviousMonthCost2(arrayDates3[0, 0], arrayDates3[0, 1]);

                trqh["totalmonth0appvcost"] = PreviousMonthCost3(arrayDates0[0, 0], arrayDates0[0, 1]);
                trqh["totalmonth1appvcost"] = PreviousMonthCost3(arrayDates1[0, 0], arrayDates1[0, 1]);
                trqh["totalmonth2appvcost"] = PreviousMonthCost3(arrayDates2[0, 0], arrayDates2[0, 1]);
                trqh["totalmonth3appvcost"] = PreviousMonthCost3(arrayDates3[0, 0], arrayDates3[0, 1]); 


                trqh["totalmonthAllcost"] = totalAllMonthCost;
                //trqh["totalApproveCost"] = totalApproveCost;
                trqh["totalApproveCost"] = totalApproveCost;
            }
        }

        #region DocumentF2

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow trqh = e.DBAccess.DataSet.Tables["trqh"].Rows[0];
            switch (e.ControlName)
            {
                case "trqh_fromsitenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;
                case "trqh_tositenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;
                case "trqh_projectid":
                    if (!BizFunctions.IsEmpty(trqh["fromsitenum"]))
                    {
                        e.DefaultCondition = " Projectid in (Select projectid from CTRH where sitenum='" + e.CurrentRow["fromsitenum"].ToString().Trim() + "' and [status]<>'V') ";
                    }
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
                        trqh["whnum"] = BizLogicTools.Tools.GetWhnum(trqh["tositenum"].ToString(),this.dbaccess);
                    }
                    break;

                case "trqh_fromsitenum":
                    {
                        trqh["whnum"] = BizLogicTools.Tools.GetWhnum(trqh["fromsitenum"].ToString(), this.dbaccess);
                        trqh["projectid"] = BizLogicTools.Tools.GetProjectID(e.F2CurrentRow["arnum"].ToString(),this.dbaccess);
                    }
                    break;

                case "trqh_whnum":
                    {
                        //trqh["searchType"] = "WHM";
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
                    e.CurrentRow["retail"] = e.F2CurrentRow["stdcost"];
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

            foreach (DataRow dr in trq1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {

                    BizFunctions.UpdateDataRow(trqh, dr, "trandate/user/flag/status/created/modified");
                }
            }


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
                if (MessageBox.Show("Do you Want to Send an Email Notification?", "Notification", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string ApproveStatus = "";
                    string EmpName = "";
                    string Body = "";
                    string itemList = "";

                    DataTable EmpDataTb = BizLogicTools.Tools.GetCommonEmpData(trqh["requestedby"].ToString());

                    if (EmpDataTb.Rows.Count > 0)
                    {
                        EmpName = EmpDataTb.Rows[0]["empname"].ToString();
                    }
                   

                    StringWriter stringwriter = new StringWriter();

                    HtmlTextWriter writer = new HtmlTextWriter(stringwriter);

                    if (trqh["approvestatus"].ToString().Trim() == "Yes")
                    {
                        if (trq1.Rows.Count > 0)
                        {


                            ApproveStatus = "Approved";
                          
                            writer.WriteBeginTag("p");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("Dear " + EmpName + ", ");
                            writer.WriteEndTag("p");

                            writer.WriteBeginTag("p");
                            writer.Write(HtmlTextWriter.TagRightChar);

                            writer.Write("Please note that your Order : " + trqh["refnum"].ToString().Trim() + " has been " + ApproveStatus + " by " + trqh["approveby"].ToString() + " on " + Convert.ToDateTime(trqh["approvedate"]).ToShortDateString());
                            writer.WriteEndTag("p");

     
                            writer.WriteBeginTag("table Border=1");
         
                            writer.Write(HtmlTextWriter.TagRightChar);
           

                            string test1 = stringwriter.ToString();

                            writer.WriteBeginTag("tr");
                            writer.Write(HtmlTextWriter.TagRightChar);

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("S/N   ");
                            writer.WriteEndTag("td");

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("Description   ");
                            writer.WriteEndTag("td");

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("UOM   ");
                            writer.WriteEndTag("td");

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("Request Qty  ");
                            writer.WriteEndTag("td");

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("Approved Qty  ");
                            writer.WriteEndTag("td");

                   


                            writer.WriteEndTag("tr");

       
                           

                            int count = 1;
                            decimal totalqty = 0;
                            decimal totalamt = 0;

                            foreach (DataRow dr1 in trq1.Rows)
                            {
                                if (dr1.RowState != DataRowState.Deleted)
                                {
    
                                    writer.WriteBeginTag("tr");
                                    writer.Write(HtmlTextWriter.TagRightChar);
                                    
                                    writer.WriteBeginTag("td");
                                    writer.Write(HtmlTextWriter.TagRightChar);
                                    writer.Write(count.ToString());
                                    writer.WriteEndTag("td");

                                    writer.WriteBeginTag("td");
                                    writer.Write(HtmlTextWriter.TagRightChar);
                                    writer.Write(dr1["detail"].ToString());
                                    writer.WriteEndTag("td");

                                    writer.WriteBeginTag("td");
                                    writer.Write(HtmlTextWriter.TagRightChar);
                                    writer.Write(dr1["uomcode"].ToString());
                                    writer.WriteEndTag("td");

                                    writer.WriteBeginTag("td");
                                    writer.Write(HtmlTextWriter.TagRightChar);
                                    writer.Write(dr1["trqqty"].ToString());
                                    writer.WriteEndTag("td");

                                    writer.WriteBeginTag("td");
                                    writer.Write(HtmlTextWriter.TagRightChar);
                                    writer.Write(dr1["approveqty"].ToString());
                                    writer.WriteEndTag("td");


                                    writer.WriteEndTag("tr");


                                    count = count + 1;

                                    totalqty = totalqty + Convert.ToDecimal(dr1["approveqty"]);

                                    totalamt = totalamt + (Convert.ToDecimal(dr1["retail"]) * Convert.ToDecimal(dr1["approveqty"]));
                                }
                            }

                            //////
                            writer.WriteBeginTag("tr");
                            writer.Write(HtmlTextWriter.TagRightChar);

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.WriteEndTag("td");

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.WriteEndTag("td");

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.WriteEndTag("td");

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("Total Cost:");
                            writer.WriteEndTag("td");      

                            writer.WriteBeginTag("td");
                            writer.Write(HtmlTextWriter.TagRightChar);
                            writer.Write("$" + totalamt.ToString("0.##"));
                            writer.WriteEndTag("td");

                            writer.WriteEndTag("tr");


                            /////
                        }

                    

   
                        writer.WriteEndTag("table");

                        writer.WriteBeginTag("br");
                        writer.Write(HtmlTextWriter.TagRightChar);

                        writer.WriteBeginTag("br");
                        writer.Write(HtmlTextWriter.TagRightChar);
               
                        
    
                        Body = stringwriter.ToString() + "\n\n Thank You & Regards,\n\n";

                    }
                    else if (trqh["approvestatus"].ToString().Trim() == "No")
                    {
                        ApproveStatus = "Rejected";
                        //Body = "Dear " + EmpName + " \n\nPlease note that your Order : " + trqh["refnum"].ToString().Trim() + " has been " + ApproveStatus + " by " + trqh["approveby"].ToString() + " on " + Convert.ToDateTime(trqh["approvedate"]).ToShortDateString()  + " \n\n Thank You & Regards,\n\n";

                        writer.WriteBeginTag("p");
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Write("Dear " + EmpName + ", ");
                        writer.WriteEndTag("p");

                        writer.WriteBeginTag("p");
                        writer.Write(HtmlTextWriter.TagRightChar);

                        writer.Write("Please note that your Order : " + trqh["refnum"].ToString().Trim() + " has been " + ApproveStatus + " by " + trqh["approveby"].ToString() + " on " + Convert.ToDateTime(trqh["approvedate"]).ToShortDateString());
                        writer.WriteEndTag("p");

                        Body = stringwriter.ToString() + "\n\n Thank You & Regards,\n\n";

                    }


                    string Subject = trqh["refnum"].ToString().Trim() + " - " + trqh["fromsitename"].ToString() + " - " + ApproveStatus;
                    SendEmail2(Subject, Body);

                    
                }
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
                        //dr1["retail"] = GetLatestMatnumCost(dr1["matnum"].ToString(), Convert.ToDateTime(trqh["trandate"])); // Jason: 26/01/2015
                        if (!BizFunctions.IsEmpty(dr1["matnum"]))
                        {
                            if (BizFunctions.IsEmpty(dr1["retail"]))
                            {
                                //24/03/2015: Jason:  Users decided to get cost from the stdcost column of MATM
                                //dr1["retail"] = BizLogicTools.Tools.GetPeriodCost(dr1["matnum"].ToString(), BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(trqh["trandate"])), this.dbaccess);
                                dr1["retail"] = BizLogicTools.Tools.GetMatmStdCost(dr1["matnum"].ToString(), this.dbaccess);
                            }
                            else if (Convert.ToDecimal(dr1["retail"]) == 0)
                            {
                                dr1["retail"] = BizLogicTools.Tools.GetMatmStdCost(dr1["matnum"].ToString(), this.dbaccess);
                            }
                        }

                        dr1["itemRetailTotal"] = Convert.ToDecimal(dr1["approveqty"]) * Convert.ToDecimal(dr1["retail"]);

                        dr1["detail"] = dr1["matname"];
                        if ((int)dr1["qty"] < 0)
                        {
                            MessageBox.Show("Invalid quantity");
                            dr1.SetColumnError("qty", "Invalid quantity");
                        }
                    }
                    ttqty += (int)dr1["qty"];
                    #endregion

                    GetPonum2(dr1);

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
                        //No longer gets the period cost
                        //dr1["retail"] = BizLogicTools.Tools.GetPeriodCost(dr1["matnum"].ToString(), BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(trqh["trandate"])), this.dbaccess);
                        dr1["senddate"] = previousQtyRequest(dr1["matnum"].ToString().Trim(), (DateTime)trqh["trandate"]);

                        if (BizFunctions.IsEmpty(dr1["approveqty"]))
                        {
                            if (!BizFunctions.IsEmpty(dr1["trqqty"]))
                            {
                                dr1["approveqty"] = dr1["trqqty"];
                            }
                            else
                            {
                                dr1["approveqty"] = 0;
                            }
                        }

                        //GetPonum(dr1["matnum"].ToString());
                        //else if (Convert.ToDecimal(dr1["approveqty"]) == 0 && Convert.ToDecimal(dr1["trqqty"])>0)
                        //{
                        //    dr1["approveqty"] = dr1["trqqty"];
                        //}
                        


                        //if (BizFunctions.IsEmpty(dr1["approveqty"]))
                        ////if (BizFunctions.IsEmpty(dr1["approveqty"]) && !BizFunctions.IsEmpty(dr1["trqqty"]))
                        //{
                        //    if (BizFunctions.IsEmpty(dr1["approveqty"]))
                        //    {
                        //        if (Convert.ToDecimal(dr1["trqqty"]) > 0)
                        //        {
                        //            dr1["approveqty"] = dr1["trqqty"];
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    if (Convert.ToDecimal(dr1["trqqty"]) > 0)
                        //    {
                        //        dr1["approveqty"] = dr1["trqqty"];
                        //    }
                        //    else
                        //    {
                        //        dr1["approveqty"] = 0;
                               
                        //    }
                        //}

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

            // Jason : 18/03/2015 disable if user keyed in 0 qty then let it be 0
            //if (trq1.Rows.Count > 0)
            //{
            //    foreach (DataRow dr1 in trq1.Rows)
            //    {
            //        if (dr1.RowState != DataRowState.Deleted)
            //        {
            //            if (BizFunctions.IsEmpty(dr1["approveqty"]))
            //            {
            //                dr1["approveqty"] = dr1["trqqty"];
            //            }
            //            else if (Convert.ToDecimal(dr1["approveqty"]) == 0 && !BizFunctions.IsEmpty(dr1["trqqty"]))
            //            {
            //                if (Convert.ToDecimal(dr1["trqqty"]) > 0)
            //                {
            //                    dr1["approveqty"] = dr1["trqqty"];
            //                }
            //            }
            //        }
            //    }
            //}

            CalculateCost();

            if (BizFunctions.IsEmpty(trqh["whnum"]))
            {
                trqh["whnum"] = BizLogicTools.Tools.GetWhnum(trqh["fromsitenum"].ToString(), this.dbaccess);
            }

            if (BizFunctions.IsEmpty(trqh["empname"]) && !BizFunctions.IsEmpty(trqh["requestedby"]))
            {
                trqh["empname"] = BizLogicTools.Tools.GetEmpname(trqh["requestedby"].ToString().Trim(), this.dbaccess);
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
            if (BizFunctions.IsEmpty(trqh["requesttype"]) == true || trqh["requesttype"].ToString() == "")
            {
                if(trqh["flag"].ToString()=="TRQ")
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

        private decimal PreviousMonthCost(string matnum,int monthNo)
        {
            decimal cost = 0;
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];

            string query = "SELECT ISNULL(SUM(ISNULL(retail,0)*t1.approveqty),0) as totalcost from " +
                            "TRQ1 t1 left join TRQH th ON T1.refnum=TH.refnum "+
                            "where th.approvestatus='YES' AND (th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM,"+monthNo.ToString()+",'"+BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"]))+"')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM,"+monthNo.ToString()+",'"+BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"]))+"'))) "+
                            "and t1.matnum='" + matnum + "' and th.[status]<>'V' and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' ";

            //string query = "SELECT ISNULL(SUM(ISNULL(retail,0)*t1.approveqty),0) as totalcost from " +
            //                "TRQ1 t1 left join TRQH th ON T1.refnum=TH.refnum " +
            //                "where "+
            //                "th.trqmonth=" + monthNo.ToString() + " " +
            //                "and th.trqyear=" + yearNo.ToString() + " " +
            //                "and t1.matnum='" + matnum + "' and th.[status]<>'V' and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' ";

            this.dbaccess.ReadSQL("TempMatnumCost",query);

            DataTable dt = this.dbaccess.DataSet.Tables["TempMatnumCost"];

            if(dt.Rows.Count>0)
            {
                if (BizFunctions.IsEmpty(dt.Rows[0]["totalcost"]))
                {
                    dt.Rows[0]["totalcost"] = 0;
                }
                cost = Convert.ToDecimal(dt.Rows[0]["totalcost"]);
            }

            return cost;
        }

        #region - Get cost from Approved MRQ qty
        private decimal PreviousMonthCost2(int monthNo, int yearNo)
        {
            decimal cost = 0;
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];

            // 20/03/2015: Jason - Clarence has asked to calculate all items instead of excluding and item with a code of M011
            //string query = "SELECT ISNULL(SUM(ISNULL(retail,0)*t1.approveqty),0) as totalcost from " +
            //     "TRQ1 t1 left join TRQH th ON T1.refnum=TH.refnum " +
            //     "where th.approvestatus='YES' AND (th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "'))) " +
            //     "and th.[status]<>'V' and t1.matnum not like 'M011%' and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' ";


            string query = "SELECT ISNULL(SUM(ISNULL(retail,0)*t1.approveqty),0) as totalcost from " +
                 "TRQ1 t1 left join TRQH th ON T1.refnum=TH.refnum " +
                 "where "+
                 "th.trqmonth=" + monthNo.ToString() + " " +
                 "and th.trqyear=" + yearNo.ToString() + " " +
                 "and th.refnum<>'"+trqh["refnum"].ToString()+"' "+
                 "and th.[status]<>'V' and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' ";

            this.dbaccess.ReadSQL("TempMatnumCost", query);

            DataTable dt = this.dbaccess.DataSet.Tables["TempMatnumCost"];

            if (dt.Rows.Count > 0)
            {
                if (BizFunctions.IsEmpty(dt.Rows[0]["totalcost"]))
                {
                    dt.Rows[0]["totalcost"] = 0;
                }
                cost = Convert.ToDecimal(dt.Rows[0]["totalcost"]);
            }

            return cost;
        }
        #endregion

        #region - Get cost from Issued Items and Items with PO

        private decimal PreviousMonthCost3(int monthNo, int yearNo)
        {
            decimal cost = 0;
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];

            // 20/03/2015: Jason - Clarence has asked to calculate all items instead of excluding and item with a code of M011
            //string query = "SELECT "+
            //                    "SUM(totalcost) as totalcost  "+
            //                "FROM "+
            //                "( " +
            //                    "SELECT "+
            //                        "tah.trqnum,  "+
            //                        "ISNULL(SUM(ISNULL(ta1.retail,0)*ta1.qty),0) as totalcost  "+
            //                        "from TRA1 ta1 left join TRAH tah ON ta1.refnum=tah.refnum  "+
            //                        "where ta1.matnum not like 'M011%'  "+
            //                        "and tah.trqnum in "+
            //                        "( "+
            //                            "SELECT  "+
            //                            "th.refnum "+
            //                            "from TRQH th  "+
            //                            "where th.approvestatus='YES'  "+
            //                            "AND (th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "'))) " +
            //                            "and th.[status]<>'V'  "+
            //                            "and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' "+
            //                        ") "+
            //                        "group by tah.trqnum "+
            //                    "UNION "+
            //                    "Select por1.ponum as trqnum, SUM(por1.baseunitprice*por1.qty) as totalcost from por1 where por1.matnum not like 'M011%' and  por1.ponum in " +
            //                    "( "+
            //                        "SELECT  "+
            //                            "th.refnum "+
            //                            "from TRQH th  "+
            //                            "where th.approvestatus='YES'  "+
            //                            "AND (th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "'))) " +
            //                            "and th.[status]<>'V'  "+
            //                            "and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' "+
            //                    ") and por1.[status]='P' "+
            //                    "group by por1.ponum "+
            //                 " )A";

            string query = "SELECT " +
                               "SUM(totalcost) as totalcost  " +
                           "FROM " +
                           "( " +
                               "SELECT " +
                                   "tah.trqnum,  " +
                                   "ISNULL(SUM(ISNULL(ta1.retail,0)*ta1.qty),0) as totalcost  " +
                                   "from TRA1 ta1 left join TRAH tah ON ta1.refnum=tah.refnum  " +
                                   "where tah.trqnum in " +
                                   "( " +
                                       "SELECT  " +
                                       "th.refnum " +
                                       "from TRQH th  " +
                                       "where th.approvestatus='YES'  " +
                                       //"AND (th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "'))) " +
                                       "and th.trqmonth="+monthNo.ToString()+" "+
				                       "and th.trqyear="+yearNo.ToString()+" "+
                                       "and th.[status]<>'V'  " +
                                       "and th.refnum<>'"+trqh["refnum"].ToString()+"' "+
                                       "and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' " +
                                   ") " +
                                   "and tah.[status]<>'V' " +
                                   "group by tah.trqnum " +
                               "UNION " +
                               "Select por1.ponum as trqnum, SUM(por1.baseunitprice*por1.qty) as totalcost from por1 where  por1.ponum in " +
                               "( " +
                                   "SELECT  " +
                                       "th.refnum " +
                                       "from TRQH th  " +
                                       "where th.approvestatus='YES'  " +
                                       //"AND (th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "'))) " +
                                       "and th.trqmonth=" + monthNo.ToString() + " " +
                                       "and th.trqyear=" + yearNo.ToString() + " " +
                                       "and th.[status]<>'V'  " +
                                       "and th.refnum<>'" + trqh["refnum"].ToString() + "' " +
                                       "and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' " +
                               ") and por1.[status]<>'V' " +
                               "group by por1.ponum " +
                            " )A";


            this.dbaccess.ReadSQL("TempMatnumCost", query);

            DataTable dt = this.dbaccess.DataSet.Tables["TempMatnumCost"];

            if (dt.Rows.Count > 0)
            {
                if (BizFunctions.IsEmpty(dt.Rows[0]["totalcost"]))
                {
                    dt.Rows[0]["totalcost"] = 0;
                }
                cost = Convert.ToDecimal(dt.Rows[0]["totalcost"]);
            }

            return cost;
        }

        #endregion

        private decimal PreviousMonthQty(string matnum, int monthNo, int yearNo)
        {
            decimal cost = 0;
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];

            // Jason :23/03/2015 Change in order to calculate the proper month
            //string query = "SELECT SUM(t1.approveqty) as totalqty from " +
            //                "TRQ1 t1 left join TRQH th ON T1.refnum=TH.refnum " +
            //                "where th.approvestatus='YES' AND (th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "'))) " +
            //                "and t1.matnum='" + matnum + "' and th.[status]<>'V' and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' ";

            string query = "SELECT SUM(t1.approveqty) as totalqty from " +
                     "TRQ1 t1 left join TRQH th ON T1.refnum=TH.refnum " +
                     "where th.approvestatus='YES' AND "+
                     //"(th.trandate>=dbo.GetFirstDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "')) and th.trandate<=dbo.GetLastDateofMonth(DATEADD(MM," + monthNo.ToString() + ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(trqh["trandate"])) + "'))) " +
                     "th.trqmonth=" + monthNo.ToString() + " " +
                     "and th.trqyear=" + yearNo.ToString() + " " +
                     "and th.refnum<>'"+trqh["refnum"].ToString()+"' "+
                     "and t1.matnum='" + matnum + "' and th.[status]<>'V' and th.fromsitenum='" + trqh["fromsitenum"].ToString() + "' ";

            this.dbaccess.ReadSQL("TempMaterialQty", query);

            DataTable dt = this.dbaccess.DataSet.Tables["TempMaterialQty"];

            if (dt.Rows.Count > 0)
            {
                if (BizFunctions.IsEmpty(dt.Rows[0]["totalqty"]))
                {
                    dt.Rows[0]["totalqty"] = 0;
                }
                cost = Convert.ToDecimal(dt.Rows[0]["totalqty"]);
            }

            return cost;
        }


        private void SendEmail(string Subject, string BodyMessage)
        {

            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];
            string email = "";
            string GetEmail = "Select [dbo].[GET_EMAIL2]('"+trqh["requestedby"].ToString().Trim()+"') as email";
            this.dbaccess.ReadSQL("EmailTB", GetEmail);

            DataTable dt = this.dbaccess.DataSet.Tables["EmailTB"];

            if (isLiveEmail)
            {
                if (dt.Rows.Count > 0)
                {
                    email = dt.Rows[0]["email"].ToString().Trim();

                    if (!BizFunctions.IsEmpty(email))
                    {
                        string cc = "";
                        string bcc = "";
                        cc = "tatlee@atlmaintenance.com.sg, clarence@atlmaintenance.com.sg";
                        ATL.BizModules.Tools.SendEmail2 send = new ATL.BizModules.Tools.SendEmail2(Subject, BodyMessage, email, cc, bcc, Common.DEFAULT_SYSTEM_USERNAME, this.dbaccess);

                        send.SendEmail();
                    }
                    else
                    {
                        MessageBox.Show("Cannot send email notification\n" +
                      "The user " + BizLogicTools.Tools.GetEmpname(trqh["requestedby"].ToString().Trim(), this.dbaccess) + " has no email set in his profile",
                      "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else
                {
                    MessageBox.Show("Cannot send email notification\n" +
                  "The user "+ BizLogicTools.Tools.GetEmpname(trqh["requestedby"].ToString().Trim(),this.dbaccess) +" has no email set in his profile",
                  "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
              
            }
            else
            {

                email = "jason.obina@outlook.com";
                string cc = "";
                string bcc = "";
                cc = "jason.obina@bizpoint-intl.com, jayobina@gmail.com";
                bcc = "jason.obina@bizpoint-intl.com, jayobina@gmail.com";
     
                ATL.BizModules.Tools.SendEmail2 send = new ATL.BizModules.Tools.SendEmail2(Subject, BodyMessage, email, cc, bcc, Common.DEFAULT_SYSTEM_USERNAME, this.dbaccess);

                send.SendEmail();

            }

       
        }


        private void SendEmail2(string Subject, string BodyMessage)
        {

            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];
            string email = "";
            string GetEmail = "Select [dbo].[GET_EMAIL2]('" + trqh["requestedby"].ToString().Trim() + "') as email";
            this.dbaccess.ReadSQL("EmailTB", GetEmail);

            string GetEmail2 = "select * from enm1 where modulecode='MRQ' and [status]<>'V' AND ISNULL(recommend,0)=1";
            this.dbaccess.ReadSQL("EmailTB2", GetEmail2);

            DataTable dt = this.dbaccess.DataSet.Tables["EmailTB"];
            DataTable dt2 = this.dbaccess.DataSet.Tables["EmailTB2"];

            if (isLiveEmail)
            {
                if (dt.Rows.Count > 0)
                {
                    email = dt.Rows[0]["email"].ToString().Trim();

                    if (!BizFunctions.IsEmpty(email))
                    {
                        string cc = "";
                        string bcc = "";
                        //cc = "tatlee@atlmaintenance.com.sg, clarence@atlmaintenance.com.sg";

                        if (dt2.Rows.Count > 0)
                        {
                            foreach (DataRow dr2 in dt2.Rows)
                            {
                                if (dr2["sendtype"].ToString().Trim().ToUpper() == "CC")
                                {
                                    cc = cc + dr2["email"].ToString() + ",";
                                }
                                else if (dr2["sendtype"].ToString().Trim().ToUpper() == "BCC")
                                {
                                    bcc = bcc + dr2["email"].ToString() + ",";
                                }
                            }
                            string strCC = "";
                            string strBCC = "";
                            if (cc.Length > 0)
                            {
                                strCC = cc.Substring(cc.Length - 1);
                            }

                            if (bcc.Length > 0)
                            {
                                strBCC = bcc.Substring(bcc.Length - 1);
                            }

                            if (strCC.Trim() == ",")
                            {
                                cc = BizLogicTools.Tools.ReplaceAt(cc, cc.Length - 1, ' ');
                            }

                            if (strBCC.Trim() == ",")
                            {
                                bcc = BizLogicTools.Tools.ReplaceAt(bcc, bcc.Length - 1, ' ');
                            }


                        }

                        ATL.BizModules.Tools.SendEmail2 send = new ATL.BizModules.Tools.SendEmail2(Subject, BodyMessage, email, cc, bcc, Common.DEFAULT_SYSTEM_USERNAME, this.dbaccess);

                        send.SendEmail();
                    }
                    else
                    {
                        MessageBox.Show("Cannot send email notification\n" +
                      "The user " + BizLogicTools.Tools.GetEmpname(trqh["requestedby"].ToString().Trim(), this.dbaccess) + " has no email set in his profile",
                      "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else
                {
                    MessageBox.Show("Cannot send email notification\n" +
                  "The user " + BizLogicTools.Tools.GetEmpname(trqh["requestedby"].ToString().Trim(), this.dbaccess) + " has no email set in his profile",
                  "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else
            {

                email = "jason.obina@outlook.com";
                string cc = "";
                string bcc = "";
                //cc = "jason.obina@bizpoint-intl.com, jayobina@gmail.com";
                //bcc = "jason.obina@bizpoint-intl.com, jayobina@gmail.com";

                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        if (dr2["sendtype"].ToString().Trim().ToUpper() == "CC")
                        {
                            cc = cc + dr2["email"].ToString() + ",";
                        }
                        else if (dr2["sendtype"].ToString().Trim().ToUpper() == "BCC")
                        {
                            bcc = bcc + dr2["email"].ToString() + ",";
                        }
                    }
                  string strCC = "";
                  string strBCC = "";
                  if (cc.Length > 0)
                  {
                      strCC = cc.Substring(cc.Length - 1);
                  }

                  if (bcc.Length > 0)
                  {
                      strBCC = bcc.Substring(bcc.Length - 1);
                  }
                                  
                  if (strCC.Trim() == ",")
                  {
                      cc = BizLogicTools.Tools.ReplaceAt(cc, cc.Length - 1, ' ');
                  }

                  if (strBCC.Trim() == ",")
                  {
                      bcc = BizLogicTools.Tools.ReplaceAt(bcc, bcc.Length - 1, ' ');
                  }

                    
                }

                ATL.BizModules.Tools.SendEmail2 send = new ATL.BizModules.Tools.SendEmail2(Subject, BodyMessage, email, cc, bcc, Common.DEFAULT_SYSTEM_USERNAME, this.dbaccess);

                send.SendEmail();

            }


        }




        private void GetPonum(string matnum)
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = this.dbaccess.DataSet.Tables["trq1"];
            string[,] Ponum = new string[1,4] ;


            string str = "Select refnum,matnum,ponum as trqnum from por1 where ponum='" + trqh["refnum"].ToString() + "' and matnum='" + matnum + "' and [status]<>'V' group by refnum,matnum,ponum";

            this.dbaccess.ReadSQL("TempPORTRQList", str);


            DataTable TempPORTRQList = this.dbaccess.DataSet.Tables["TempPORTRQList"];

            if(trq1.Rows.Count > 0)
            {
                foreach(DataRow dr1 in trq1.Rows)
                {
                    if(dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["ponum1"] = "";
                        dr1["ponum2"] = "";
                        dr1["ponum3"] = "";
                        dr1["ponum4"] = "";

                        //////////////////////////////////
                        if (TempPORTRQList.Rows.Count > 0)
                        {
                            int i = 1;
                            foreach (DataRow dr in TempPORTRQList.Rows)
                            {
                                if (dr.RowState != DataRowState.Deleted)
                                {
                                    if (dr1["matnum"].ToString().Trim() == dr["matnum"].ToString().Trim())
                                    {
                                        if (!BizFunctions.IsEmpty(dr["trqnum"]))
                                        {
                                            if (i <= 4)
                                            {
                                                dr1["ponum" + i.ToString()] = dr["refnum"];
                                            }
                                            i = i + 1;
                                        }
                                    }
                                }
                            }
                        }
                        /////////////////////////////////
                    }
                }
            }

           




           

            //int[,] MonthYear;

            //string mmStr = "";

            //if (mm >= 10)
            //{
            //    mmStr = mm.ToString();
            //}
            //else
            //{
            //    mmStr = "0" + mm.ToString();
            //}

            //string DateString = Convert.ToString(yyyy) + Convert.ToString(mmStr) + "01";

            //DateTime dt = GetSafeDate(DateString).AddMonths(monthno);



            //YYYY = dt.Year;
            //MM = dt.Month;

            //MonthYear = new int[1, 2];

            //MonthYear[0, 0] = MM;
            //MonthYear[0, 1] = YYYY;



        }

        private void GetPonum2(DataRow dr)
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["trqh"].Rows[0];
            DataTable trq1 = this.dbaccess.DataSet.Tables["trq1"];

            string str = "Select refnum,matnum,ponum as trqnum from por1 where ponum='" + trqh["refnum"].ToString() + "' and matnum='" + dr["matnum"].ToString() + "' and [status]<>'V' group by refnum,matnum,ponum";

            this.dbaccess.ReadSQL("TempPORTRQList", str);


            DataTable TempPORTRQList = this.dbaccess.DataSet.Tables["TempPORTRQList"];

                        

                        //////////////////////////////////

                        if (TempPORTRQList.Rows.Count > 0)
                        {
                            dr["ponum1"] = "";
                            dr["ponum2"] = "";
                            dr["ponum3"] = "";
                            dr["ponum4"] = "";
                            
                                //for (int i = 0; i < TempPORTRQList.Rows.Count; i++)
                                //{
                                //    for (int y = 1; y <= TempPORTRQList.Rows.Count; y++)
                                //    {
                                //        if (y <= 4)
                                //        {
                                //            dr["ponum" + y.ToString()] = TempPORTRQList.Rows[i]["refnum"].ToString();
                                //        }
                                //    }
                                    
                                //}

                            for (int i = 0; i < TempPORTRQList.Rows.Count; i++)
                            {
                                int y = i + 1;
                                    if (y <= 4)
                                    {
                                        dr["ponum" + y.ToString()] = TempPORTRQList.Rows[i]["refnum"].ToString();
                                    }
                                

                            }
                         }
                        
          








            //int[,] MonthYear;

            //string mmStr = "";

            //if (mm >= 10)
            //{
            //    mmStr = mm.ToString();
            //}
            //else
            //{
            //    mmStr = "0" + mm.ToString();
            //}

            //string DateString = Convert.ToString(yyyy) + Convert.ToString(mmStr) + "01";

            //DateTime dt = GetSafeDate(DateString).AddMonths(monthno);



            //YYYY = dt.Year;
            //MM = dt.Month;

            //MonthYear = new int[1, 2];

            //MonthYear[0, 0] = MM;
            //MonthYear[0, 1] = YYYY;



        }



    }
}

