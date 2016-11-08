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
using ATL.TR;

namespace ATL.TRA
{
    public class Voucher_TRA : BizRAD.BizApplication.VoucherBaseHelper
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
        bool trahColumnChange = true;
        bool trr1ColumnChange = true;
        bool materialFlag = false;
        string sitenum, flag = null;

        Hashtable selectsCollection = null;
        string command = null;
        DataGrid dg_detail = null;
        private string formName4SideBtn;
        protected DataTable tra1;

        TextBox txt_barcode = null;
        protected Button btn_ExtractMATM, btn_ExtractTemplate, btnPrint, btn_PreviousMATM = null;
        private ComboBox trah_trayear, trah_tramonth = null;
        protected string fromwhnum = null;
        protected Label lblDisplay = null;
        #endregion

        #region Construct
        public Voucher_TRA(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_TRA.xml", moduleName, voucherBaseHelpers)
        {
            this.flag = moduleName;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "trah.flag='" + flag + "' AND trah.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (trah.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " trah.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " trah.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND trah.flag='" + flag + "' AND trah.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

        #region Delete On Click

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);

            DataTable dtD = this.dbaccess.DataSet.Tables["tra1"];

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

        protected override void Document_Insert_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Insert_Handle(sender, e);

            DataTable tra1 = e.DBAccess.DataSet.Tables["tra1"];

            int line = 100;

            if (tra1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in tra1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["line"] = line;
                        line += 100;
                    }
                }
            }
        }

        protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Insert_OnClick(sender, e);

        }


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

            ATL.BizLogicTools.Tools.setDefaults(e.DBAccess.DataSet, "trah/tra1");

            this.currentYear = Common.DEFAULT_SYSTEM_YEAR;
            this.dbaccess = e.DBAccess;
            this.formName = (e.FormsCollection["header"] as Form).Name;
            this.detailName = (e.FormsCollection["detail"] as Form).Name;

            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];

            trah["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

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

            btnPrint = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Print") as Button;
            btnPrint.Enabled = true;
            btnPrint.Click += new EventHandler(btnPrint_Click);

            #region Set default values upon load
            if (trah["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                trah["agreedby"] = Common.DEFAULT_SYSTEM_USERNAME;
                string strHemph = "select * from hemph where empname= '" + Common.DEFAULT_SYSTEM_USERNAME + "'";
                e.DBAccess.ReadSQL("hemph", strHemph);
                DataTable dthemph = e.DBAccess.DataSet.Tables["hemph"];

                if (dthemph.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(trah["requestedby"]))
                    {
                        trah["requestedby"] = dthemph.Rows[0]["empnum"];
                    }
                }
            }

            if (Convert.IsDBNull(trah["trandate"]))
            {
                trah["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }
            if (Convert.IsDBNull(trah["senddate"]))
            {
                trah["senddate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);

            }

            // Default sitenum is always HQ for backend. Depends on POSID in appconfig.
            if (BizFunctions.IsEmpty(trah["sitenum"]))
            {
                string command = "select sitenum from posm where posnum = '" + ConfigurationManager.AppSettings.Get("POSID") + "'";
                e.DBAccess.ReadSQL("posm", command);
                DataRow posm = e.DBAccess.DataSet.Tables["posm"].Rows[0];
                trah["sitenum"] = posm["sitenum"].ToString();
                trah["fromsitenum"] = posm["sitenum"].ToString();

                if (posid != null)
                {
                    string command1 = "select sitenum from posm where posnum = '" + posid + "'";
                    e.DBAccess.ReadSQL("posm", command1);
                    DataRow posm1 = e.DBAccess.DataSet.Tables["posm"].Rows[0];
                    sitenum = posm1["sitenum"].ToString();
                }
                else
                    sitenum = posid;
            }
            if (BizFunctions.IsEmpty(trah["requestedby"]))
            {
                trah["requestedby"] = Common.DEFAULT_SYSTEM_EMPNUM;
            }
            #endregion

            #region initial controls
            e.DBAccess.DataSet.Tables["trah"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRAH_ColumnChanged);
            //e.DBAccess.DataSet.Tables["tra1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRA1_ColumnChanged);
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

            btn_PreviousMATM = BizXmlReader.CurrentInstance.GetControl(formName, "btn_PreviousMATM") as Button;
            btn_PreviousMATM.Click += new EventHandler(btn_PreviousMATM_Click);

            lblDisplay = BizXmlReader.CurrentInstance.GetControl(formName, "lbl_display") as Label;
            lblDisplay.Font = new Font(lblDisplay.Font, FontStyle.Bold);

            Button btn_Document_Void = (Button)BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, Common.DEFAULT_DOCUMENT_BTNVOID);
            btn_Document_Void.Visible = true;
            btn_Document_Void.Enabled = true;

            #endregion         

            if (BizFunctions.IsEmpty(trah["sitenum"]))
            {
                string command = "select sitenum from posm where posnum = '" + ConfigurationManager.AppSettings.Get("POSID") + "'";
                e.DBAccess.ReadSQL("posm", command);
                DataRow posm = e.DBAccess.DataSet.Tables["posm"].Rows[0];
                trah["sitenum"] = posm["sitenum"].ToString();
                trah["fromsitenum"] = posm["sitenum"].ToString();
            }

          


            TextBox trah_trqnum = BizXmlReader.CurrentInstance.GetControl(formName, "trah_trqnum") as TextBox;

            

            #region get fromwhnum
            if (trah["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                string from = "";
                if (Tools.isFrontEnd())
                {
                    from = "select whnum from whm where sitenum='" + trah["fromsitenum"].ToString() + "'";
                }
                else
                {
                    from = "select whnum from whm where sitenum='HQ'";
                }
                DataSet dt = this.dbaccess.ReadSQLTemp("from", from);
                if (dt.Tables[0] != null)
                {
                    fromwhnum = dt.Tables[0].Rows[0]["whnum"].ToString();
                }
            }
            #endregion

            #region disable void/reopen button for outlet user
            //allow HQ user to void and reopen TRA,but need to check whether tra extract this tra or not.
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

            Initialise();

            e.DBAccess.DataSet.Tables["TRA2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRA2_ColumnChanged);

            string GetSITMTB = "Select * from SITM WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("SITMTB", GetSITMTB);

            string GetDOCTB = "Select refnum,sitenum from CTRH WHERE [STATUS]<>'V' UNION Select refnum,sitenum from ADH WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("DOCTBALL", GetDOCTB);

        }

        void btn_PreviousMATM_Click(object sender, EventArgs e)
        {
            DataRow trah = this.dbaccess.DataSet.Tables["trah"].Rows[0];
            DataTable tra1 = this.dbaccess.DataSet.Tables["tra1"];
            DataTable tra2 = this.dbaccess.DataSet.Tables["tra2"];

            if (!BizFunctions.IsEmpty(trah["searchType"]))
            {
                if (trah["searchType"].ToString() == "SITM")
                {
                    DialogResult result = MessageBox.Show("This will clear the current Material Details, Click 'Yes' to Continue", "Bizpoint International", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {

                        BizFunctions.DeleteAllRows(tra1);
                        BizFunctions.DeleteAllRows(tra2);

                        string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where sitenum='" + trah["tositenum"].ToString().Trim() + "'";


                        this.dbaccess.ReadSQL("DocTB", getLocation);

                        DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

                        if (DocTB != null)
                        {
                            if (DocTB.Rows.Count > 0)
                            {
                                foreach (DataRow dr2 in DocTB.Rows)
                                {
                                    DataRow InsertTRA2 = tra2.NewRow();
                                    InsertTRA2["docunum"] = dr2["refnum"];
                                    InsertTRA2["fromsitenum"] = trah["fromsitenum"];
                                    InsertTRA2["tositenum"] = dr2["sitenum"];
                                    InsertTRA2["sectorcode"] = dr2["sectorcode"];
                                    tra2.Rows.Add(InsertTRA2);
                                }
                            }
                        }

                        string getPreviousIssue ="select refnum from "+
                                                    "( "+
                                                    "select  ROW_NUMBER() OVER (Order BY refnum) as ForTop,ROW_NUMBER() OVER (Order BY refnum Desc) as ForBottom, refnum from trah where tositenum='" + trah["tositenum"].ToString().Trim() + "' and [status]<>'V' " +
                                                    ")A "+
                                                    "where ForBottom=1";

                        //string getPreviousIssue = "select top 1 refnum from trah where tositenum='" + trah["tositenum"].ToString().Trim() + "'";

                        this.dbaccess.ReadSQL("TopRefnum", getPreviousIssue);

                        DataTable TopRefnum = this.dbaccess.DataSet.Tables["TopRefnum"];

                        if (TopRefnum.Rows.Count > 0)
                        {

                            DataRow drTopRefnum = TopRefnum.Rows[0];

                            string getPreviousDetails = "Select matnum,qty,line,sitenum,fromsitenum,tositenum,sectorcode from tra1 where refnum='" + drTopRefnum["refnum"].ToString() + "'";

                            this.dbaccess.ReadSQL("TopDetails", getPreviousDetails);

                            DataTable TopDetails = this.dbaccess.DataSet.Tables["TopDetails"];

                            if (TopDetails.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in TopDetails.Rows)
                                {
                                    DataRow insertTra1 = tra1.NewRow();
                                    insertTra1["matnum"] = dr1["matnum"];
                                    insertTra1["qty"] = dr1["qty"];
                                    insertTra1["traqty"] = dr1["qty"];
                                    insertTra1["line"] = dr1["line"];
                                    insertTra1["sitenum"] = dr1["sitenum"];
                                    insertTra1["fromsitenum"] = dr1["fromsitenum"];
                                    insertTra1["tositenum"] = dr1["tositenum"];
                                    insertTra1["sectorcode"] = dr1["sectorcode"];
                                    tra1.Rows.Add(insertTra1);
                                }
                            }
                        }

                    }
                }
            }

        }

       
        #endregion

        #endregion

    

        void Voucher_TRA2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
            }
        }


        #region Initialise

        private void Initialise()
        {
            trah_trayear = BizXmlReader.CurrentInstance.GetControl(formName, "trah_trayear") as ComboBox;
            trah_tramonth = BizXmlReader.CurrentInstance.GetControl(formName, "trah_tramonth") as ComboBox;
            setTraYear();
            setTraMonth();

        }

        #endregion

        private void setTraYear()
        {
            DataRow trah = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            int[] arr1 = new int[100];
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            int Year = dt.AddYears(-50).Year;

            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = Year;
                Year = Year + 1;
            }

            trah_trayear.DataSource = arr1;
            if (!BizFunctions.IsEmpty(trah["senddate"]))
            {
                trah["trayear"] = Convert.ToDateTime(trah["senddate"]).Year;
            }
            else
            {
                trah["trayear"] = dt.Year;
            }

        }


        private void setTraMonth()
        {
            DataRow trah = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            DateTime dt = new DateTime();
            dt = DateTime.Now;


            if (!BizFunctions.IsEmpty(trah["senddate"]))
            {
                trah["tramonth"] = Convert.ToDateTime(trah["senddate"]).Month;
            }
            else
            {
                trah["tramonth"] = dt.Month;
            }

        }

        private string GetZone(string sitenum)
        {
            string Zone = "";
            string strZone = "Select sectorcode from SITMTB where sitenum='"+sitenum+"'";

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

        # region Column Change Event
        private void Voucher_TRAH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (trahColumnChange)
            {
                DataRow trah = this.dbaccess.DataSet.Tables["trah"].Rows[0];
                DataTable tra1 = this.dbaccess.DataSet.Tables["tra1"];

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

                                foreach (DataRow dr in tra1.Rows)
                                {
                                    if (dr.RowState != DataRowState.Deleted)
                                    {
                                        /*
                                        string command = "SELECT matnum,sitenum,SUM(qty) AS qty FROM mwt" + this.currentYear +
                                              " WHERE matnum='" + dr["matnum"].ToString().Trim() +
                                              "' AND sitenum='" + e.Row["fromsitenum"].ToString().Trim() +
                                              "' AND trandate<='" + BizFunctions.GetSafeDateString((DateTime)trah["trandate"]) +
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
                                foreach (DataRow dr in tra1.Rows)
                                {
                                    if (dr.RowState != DataRowState.Deleted)
                                    {
                                        dr["fromsitenum"] = trah["fromsitenum"];
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
                                    foreach (DataRow dr in tra1.Rows)
                                    {
                                        if (dr.RowState != DataRowState.Deleted)
                                        {
                                            dr["tositenum"] = trah["tositenum"];
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
                            foreach (DataRow dr in tra1.Rows)
                            {
                                //dr["remarks"] = e.Row["remarks"];
                            }
                            break;
                        }

                    #endregion
                }
            }
        }

        private void Voucher_TRA1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (trr1ColumnChange)
            {
                DataRow trah = dbaccess.DataSet.Tables["trah"].Rows[0];
                DataTable tra1 = dbaccess.DataSet.Tables["tra1"];
                switch (e.Column.ColumnName)
                {
                    case "matnum":
                        #region Update product name and uom
                        string cmd = "select * from matm where matnum='" + e.Row["matnum"].ToString().Trim() + "'";
                        DataSet tmpds = this.dbaccess.ReadSQLTemp("matm", cmd);
                        if (tmpds.Tables["matm"].Rows.Count != 0)
                        {
                            e.Row["detail"] = tmpds.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                            e.Row["barcode"] = tmpds.Tables["matm"].Rows[0]["barcode"].ToString().Trim();
                            e.Row["uom"] = tmpds.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                            e.Row["pcatcode"] = tmpds.Tables["matm"].Rows[0]["pcatcode"];
                        }
                        #endregion

                        #region Set DefaultValues for Qty
                        if (Convert.IsDBNull(e.Row["qty"]))
                            e.Row["qty"] = 0;
                        #endregion

                        #region Copy header information into the detail rows
                        e.Row["refnum"] = trah["refnum"];
                        e.Row["trandate"] = trah["trandate"];
                        e.Row["sitenum"] = trah["sitenum"];
                        e.Row["status"] = trah["status"];
                        e.Row["agreedby"] = trah["agreedby"];
                        e.Row["sentby"] = trah["sentby"];
                        e.Row["senderremarks"] = trah["senderremarks"];
                        e.Row["requestedby"] = trah["requestedby"];
                        e.Row["fromsitenum"] = trah["fromsitenum"];
                        e.Row["tositenum"] = trah["tositenum"];
                        e.Row["year"] = trah["year"];
                        e.Row["flag"] = trah["flag"];
                        e.Row["user"] = trah["user"];
                        #endregion


                        #region Update stock on hand values
                        /*
                        string command = "select matnum,sitenum,SUM(qty) AS qty from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                              " where matnum='" + e.Row["matnum"].ToString().Trim() +
                              "' AND sitenum='" + e.Row["fromsitenum"].ToString().Trim() +
                              "' AND trandate<='" + BizFunctions.GetSafeDateString((DateTime)trah["trandate"]) +
                              //"' AND whnum='ADUI-MWH'"+
                              " GROUP BY bmatnum,sitenum";
                        this.dbaccess.ReadSQL("mwttemp", command);
                        DataTable mwttemp = this.dbaccess.DataSet.Tables["mwttemp"];
                        if (mwttemp.Rows.Count != 0)
                            e.Row["stockonhand"] = mwttemp.Rows[0]["qty"];
                        else*/
                            e.Row["stockonhand"] = 0;

                        if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        {
                            // If matnum invalid then prompt
                            if (!BizValidate.CheckTableIsValid(this.dbaccess, "matm", "matnum", e.Row["matnum"].ToString().Trim()))
                            {
                                e.Row.SetColumnError(e.Column.ColumnName, "Material Code not Valid");
                            }
                            # region Check Column Error
                            else
                            {
                                e.Row.SetColumnError(e.Column.ColumnName, "");
                                int check = 0;
                                check = checkdatagridError(e.Row);
                                if (check != 0)
                                {
                                    seterrorMessage(e.Row, check);
                                    break;
                                }
                            }
                        }
                        else
                            e.Row.SetColumnError(e.Column.ColumnName, "Material Code cannot be empty");
                            #endregion
                        break;
                        #endregion

                        
                    case "qty":
                        #region qty
                        if (e.Row["qty"] != System.DBNull.Value)
                        {
                            if ((int)e.Row["qty"] < 0)
                            {
                                MessageBox.Show("Invalid quantity");
                                e.Row.SetColumnError("qty", "Invalid quantity");
                            }
                            else
                            {
                                materialFlag = true;
                            }
                        }
                        #region update ttqty,ttamt in header
                        calcTotal();
                        #endregion
                        break;
                       #endregion

                }
            }
        }

        private void calcTotal()
        {
            DataTable tra1 = dbaccess.DataSet.Tables["tra1"];
            DataRow trah = dbaccess.DataSet.Tables["trah"].Rows[0];

            #region update ttqty,ttamt in header
            decimal ttqty = 0;
            foreach (DataRow dr1 in tra1.Rows)
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
            
            trah["ttqty"] = ttqty;
            #endregion
        }
        #endregion

        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];
            DataTable tra1 = e.DBAccess.DataSet.Tables["tra1"];

            

            //    # region Check for empty row and empty mat code
            if (tra1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in tra1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["matnum"]))
                        {
                            MessageBox.Show("Save Unsuccessful\nProduct Code cannot be empty !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Handle = false;
                            return;
                        }
                        if(BizFunctions.IsEmpty(dr1["fromsitenum"]))
                        {
                            dr1["fromsitenum"] = trah["fromsitenum"];
                        }

                        if(BizFunctions.IsEmpty(dr1["fromsitenum"]))
                        {
                            dr1["tositenum"] = trah["tositenum"];
                        }
                    }
                }
            }

            //    if (tra1.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Save Unsuccessful\nYou cannot save the file without any Product/Voucher!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }

            //    if (trah["fromsitenum"].ToString().Trim().CompareTo(trah["tositenum"].ToString().Trim()) == 0)
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom site and To site cannot be same !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }


            //    #endregion

            //    #region trah

            //    #region Check for fromsite and tosite duplication
            //    if (trah["fromsitenum"].ToString().Trim() == trah["tositenum"].ToString().Trim())
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom Site and To Site cannot be same", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //    }

            //    #endregion

            //    #region Validate sitenums
            //    if (BizFunctions.IsEmpty(trah["tositenum"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nSave Unsuccessful\nTo Site Code is empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", trah["tositenum"].ToString().Trim()))
            //        {
            //            MessageBox.Show("Save Unsuccessful\nSave Unsuccessful\nTo Site Code is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            e.Handle = false;
            //            return;
            //        }
            //    }

            //    if (BizFunctions.IsEmpty(trah["fromsitenum"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom Site Code is empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", trah["tositenum"].ToString().Trim()))
            //        {
            //            MessageBox.Show("Save Unsuccessful\nFrom Site Code is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            e.Handle = false;
            //            return;
            //        }
            //    }
            //    #endregion

            //    #region Validate dates
            //    if (BizFunctions.IsEmpty(trah["senddate"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nSend Date is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }

            //    #endregion

            //    #region Validate Empty Text Box
            //    if (BizFunctions.IsEmpty(trah["requestedby"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nRequested By cannot be empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    #endregion

            //    #endregion

            //    #region tra1
            //    foreach (DataRow dr in tra1.Rows)
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
            //    #endregion tra1

            //    //Backup Validation
            //    #region Final Backup Validation

            //    if (BizValidate.CheckColumnError(dbaccess.DataSet, "trah"))
            //    {
            //        MessageBox.Show("Invalid values detected in header", "Save unsuccessful");
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        //if (BizValidate.CheckColumnError(dbaccess.DataSet, "tra1"))
            //        //{
            //        //    MessageBox.Show("Invalid values detected in details", "Save unsuccessful");
            //        //    e.Handle = false;
            //        //    return;
            //        //}
            //    }
            //}
            //    #endregion
        }

        #region DocumentF2

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow trah = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];

            switch (e.ControlName)
            {
                case "trah_fromsitenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;
                case "trah_tositenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;
                case "trah_trqnum":
                    e.DefaultCondition = " refnum not in (Select ISNULL(trqnum,'') as refnum  from TRAH where [status]<>'V')   ";
                    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow trah = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            switch (e.ControlName)
            {
                case "trah_adhnum":
                    {
                        
                    }
                    break;

                case "trah_docunum":
                    e.CurrentRow["docunum"] = e.F2CurrentRow["refnum"];
                    e.CurrentRow["tositenum"] = e.F2CurrentRow["sitenum"];
                    e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    e.CurrentRow["contracttype"] = e.F2CurrentRow["TableName"];
                    {
                        trah["searchType"] = "DOC";
                    }
                    break;

                case "trah_tositenum":                   
                    {
                        trah["searchType"] = "SITM";
                        e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    }
                    break;

                case "trah_whnum":
                    {
                        trah["searchType"] = "WHM";
                    }
                    break;

                case "trah_sectorcode":
                    {
                        trah["searchType"] = "SEM";
                    }
                    break;

                case "trah_trqnum":
                    {
                        trah["searchType"] = "TRQ";
                        //trah["fromsitenum"] = e.F2CurrentRow["tositenum"];
                        trah["fromsitenum"] = e.F2CurrentRow["tositenum"];
                        trah["tositenum"] = e.F2CurrentRow["fromsitenum"];
                        trah["tositenum"] = e.F2CurrentRow["fromsitenum"];
                        trah["requestedby"] = e.F2CurrentRow["requestedby"];
                        trah["agreedby"] = e.F2CurrentRow["approveby"]; 
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
                case "matnum":
                    //e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    //e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    //e.CurrentRow["pcatcode"] = e.F2CurrentRow["pcatcode"];
                    //if (e.CurrentRow["barcode"] == System.DBNull.Value)
                    //{
                    //    e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    //}
                    break;
                case "barcode":
                    //e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    //e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    //e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    //e.CurrentRow["pcatcode"] = e.F2CurrentRow["pcatcode"];
                    //if (e.CurrentRow["matnum"] == System.DBNull.Value)
                    //{
                    //    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    //}
                    break;
            }
        }

        #endregion

        #region tra ReOpen/void

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);
            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];

            #region ori-not allow front end user to reopen and void
            //if (Tools.isFrontEnd() || Tools.isFrontEndVoucher(dbaccess, "TRAH"))
            //{
            //    // No reopen in outlet
            //    e.Handle = false;
            //    return;
            //}
            #endregion

            #region allow outlet user to reopen/void tra,but need to check whether this tra is extracted by tra or not
            //can only void/reopen own tra.Even HQ also can't void outlets
            //if (!Tools.isFrontEnd())
            //{
            //    if (Tools.isFrontEndVoucher(dbaccess, "TRAH"))
            //    {
            //        // No reopen in outlet
            //        e.Handle = false;
            //        MessageBox.Show("Not allowed to reopen " + trah["refnum"].ToString()+ ",which is created by outlet !", "Reopen not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    else
            //    {
            //        string strTRA = "select * from trah where tranum='" + trah["refnum"].ToString() + "'";
            //        DataSet dsTRA = this.dbaccess.ReadSQLTemp("tra", strTRA);
            //        if (dsTRA != null)
            //        {
            //            if (dsTRA.Tables[0].Rows.Count > 0)
            //            {
            //                e.Handle = false;
            //                MessageBox.Show(trah["refnum"].ToString().Trim() + " has been extracted by " + dsTRA.Tables["tra"].Rows[0]["refnum"].ToString() + " ,not allow to reopen!", "Reopen not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //    string strTRA = "select * from trah where tranum='" + trah["refnum"].ToString() + "'";
            //    DataSet dsTRA = this.remoteDBAccess.ReadSQLTemp("tra", strTRA);
            //    if (dsTRA != null || dsTRA.Tables[0]!=null)
            //    {
            //        if (dsTRA.Tables[0].Rows.Count > 0)
            //        {
            //            e.Handle = false;
            //            MessageBox.Show(trah["refnum"].ToString().Trim() + " has been extracted by " + dsTRA.Tables["tra"].Rows[0]["refnum"].ToString() + " !", "Reopen not allowed,not allow to reopen!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //    DataRow trah =this.dbaccess.DataSet.Tables["trah"].Rows[0];
            //    try
            //    {
            //        // Update the backend trah and tra1 status.
            //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trah set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum = '" + trah["refnum"].ToString().Trim() + "'");
            //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update tra1 set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum = '" + trah["refnum"].ToString().Trim() + "'");
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
            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];
            #region ori-not allow front end user to void
            //if (Tools.isFrontEnd())
            //{
            //    e.Handle = false;
            //}
            #endregion

            #region allow outlet user to void tra,but need to check whether this tra is extracted by tra or not
            //can only void/reopen own tra.Even HQ also can't void outlets
            //if (!Tools.isFrontEnd())
            //{
            //    if (Tools.isFrontEndVoucher(dbaccess, "TRAH"))
            //    {
            //        e.Handle = false;
            //        MessageBox.Show("Not allowed to void " + trah["refnum"].ToString() + ",which is created by outlet !", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    else
            //    {
            //        string strTRA = "select * from trah where tranum='" + trah["refnum"].ToString() + "'";
            //        DataSet dsTRA = this.dbaccess.ReadSQLTemp("tra", strTRA);
            //        if (dsTRA != null)
            //        {
            //            if (dsTRA.Tables[0].Rows.Count > 0)
            //            {
            //                e.Handle = false;
            //                MessageBox.Show(trah["refnum"].ToString().Trim() + " has been extracted by " + dsTRA.Tables["tra"].Rows[0]["refnum"].ToString() + " ,not allow to void!", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //    string strTRA = "select * from trah where tranum='" + trah["refnum"].ToString() + "'";
            //    DataSet dsTRA = this.remoteDBAccess.ReadSQLTemp("tra", strTRA);
            //    if (dsTRA != null || dsTRA.Tables[0] != null)
            //    {
            //        if (dsTRA.Tables[0].Rows.Count > 0)
            //        {
            //            e.Handle = false;
            //            MessageBox.Show(trah["refnum"].ToString().Trim() + " has been extracted by " + dsTRA.Tables["tra"].Rows[0]["refnum"].ToString() + ",not allow to void!", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];

            ////if (Tools.isFrontEnd())
            ////{
            ////    try
            ////    {
            ////        // Update the backend trah and tra1 status.
            ////        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trah set status='" + Common.DEFAULT_DOCUMENT_STATUSV + "' where refnum = '" + trah["refnum"].ToString().Trim() + "'");
            ////        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update tra1 set status='" + Common.DEFAULT_DOCUMENT_STATUSV + "' where refnum = '" + trah["refnum"].ToString().Trim() + "'");
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
            DataTable tra1 = e.DBAccess.DataSet.Tables["tra1"];
            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];

            #region assgin value for heaer
            if (trah["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                trah["confirms"] = 1;
            }
            else
            {
                trah["confirms"] = 0;
            }
            if (trah["year"] == System.DBNull.Value)
            {
                trah["year"] = ((DateTime)trah["trandate"]).Year;
            }
            #endregion

            #region Save important header information into details
            if (tra1.Rows.Count > 0)
            {
                foreach (DataRow dr in tra1.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        //Copies some of the trah's column data to tra1
                        BizFunctions.UpdateDataRow(trah, dr, "fromsitenum,tositenum,sentby,sitenum,senddate,trandate,year,flag,status,confirms,user,created,modified");
                    }
                }
            }
            # endregion  

            if (trah["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                #region update to local mwt
                foreach (DataRow dr in tra1.Rows)
                {
                    if (dr["matnum"] != System.DBNull.Value || dr["matnum"].ToString() != String.Empty)
                    {
                        if (dr.RowState != DataRowState.Deleted)
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
                                    dr_mwt["location"] = dr["fromsitenum"];
                                    dr_mwt["qty"] = -(int)dr["qty"];
                                    dr_mwt["uom"] = dr["uomcode"];
                                    //if (Tools.isFrontEnd())
                                    //{
                                    //dr_mwt["whnum"] = dr["fromsitenum"];//Decrease qty in main warehouse,here is the fromsitenum(local) if is outlets
                                    dr_mwt["whnum"] = fromwhnum;//Decrease qty in main warehouse,here is the fromsitenum(local) if is outlets
                                    //}
                                    //else
                                    //{
                                    //    dr_mwt["whnum"] = "MWH";//Decrease qty in main warehouse
                                    //}
                                    dr_mwt["guid"] = Tools.getGUID();
                                    dr_mwt["trandate"] = dr["trandate"];
                                    dr_mwt["year"] = dr["year"];
                                    dr_mwt["created"] = dr["created"];
                                    dr_mwt["modified"] = dr["modified"];
                                    dr_mwt["status"] = dr["status"].ToString().Trim();
                                    dr_mwt["user"] = dr["user"].ToString().Trim();
                                    dr_mwt["remarks"] = dr["remarks"].ToString().Trim();
                                    dr_mwt["flag"] = trah["flag"];
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
                                    dr_mwt["location"] = "HQ";
                                    dr_mwt["qty"] = (int)dr["qty"];
                                    dr_mwt["uom"] = dr["uomcode"];
                                    dr_mwt["whnum"] = "SITWH";//increase qty in Stock In Transit warehouse(Location HQ)
                                    dr_mwt["guid"] = Tools.getGUID();
                                    dr_mwt["trandate"] = dr["trandate"];
                                    dr_mwt["year"] = dr["year"];
                                    dr_mwt["created"] = dr["created"];
                                    dr_mwt["modified"] = dr["modified"];
                                    dr_mwt["status"] = dr["status"].ToString().Trim();
                                    dr_mwt["user"] = dr["user"].ToString().Trim();
                                    dr_mwt["remarks"] = dr["remarks"].ToString().Trim();
                                    dr_mwt["flag"] = trah["flag"];
                                    mwt.Rows.Add(dr_mwt);
                                }
                            }
                            #endregion
                        }
                    }

                }
                #endregion
            }
        }
        #endregion
        
        #region Save End Event

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];
            DataTable tra1 = e.DBAccess.DataSet.Tables["tra1"];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];

            string matnums = String.Empty;

            if (trah["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                //try
                //{
                //    if (Tools.isFrontEnd())
                //    {
                //        // Fill the backend dbaccess with required tables
                //        #region Save to trah,tra1 to backend
                //        // If USER only saves, then just save to front end and back end.
                //        // If USER confirms, then ensure that front end will get saved because
                //        // mwt will be pulled from there.
                //        foreach (DataTable dataTable in e.DBAccess.DataSet.Tables)
                //        {
                //            if (dataTable.TableName == "trah" || dataTable.TableName == "tra1")
                //            {
                //                DataTable tempDataTable = dataTable.Clone();

                //                // Remove the mark columns, these do not exist in the database
                //                if (dataTable.TableName == "tra1")
                //                {
                //                    if (tempDataTable.Columns.Contains("mark"))
                //                        tempDataTable.Columns.Remove("mark");

                //                    if (dataTable.Columns.Contains("mark"))
                //                        dataTable.Columns.Remove("mark");
                //                }

                //                int id = 0;
                //                DataSet trahmaxid = remoteDBAccess.ReadSQLTemp("traid", "Select max(id) as 'id' from " + dataTable.TableName);

                //                if (trahmaxid.Tables["traid"].Rows.Count > 0)
                //                {
                //                    if (trahmaxid.Tables["traid"].Rows[0]["id"] == System.DBNull.Value)
                //                        id = 0;
                //                    else
                //                        id = Convert.ToInt32(trahmaxid.Tables["traid"].Rows[0]["id"]) + 1;
                //                }
                //                else
                //                {
                //                    id = 0;
                //                }

                //                foreach (DataRow dr in dataTable.Rows)
                //                {
                //                    DataRow newrow = tempDataTable.NewRow();

                //                    if (dr.RowState != DataRowState.Deleted)
                //                    {
                //                        foreach (DataColumn dc in dataTable.Columns)
                //                        {
                //                            newrow[dc.ColumnName] = dr[dc.ColumnName];
                //                            if (dc.ColumnName == "ID")
                //                            {
                //                                newrow[dc.ColumnName] = id;
                //                                id++;
                //                            }
                //                        }
                //                    }
                //                    tempDataTable.Rows.Add(newrow);
                //                }

                //                if (remoteDBAccess.DataSet.Tables.Contains(tempDataTable.TableName))
                //                    remoteDBAccess.DataSet.Tables.Remove(tempDataTable.TableName);

                //                remoteDBAccess.DataSet.Tables.Add(tempDataTable);
                //            }
                //        }

                //        Hashtable tablesCollection = new Hashtable();
                //        foreach (DataTable dataTable in remoteDBAccess.DataSet.Tables)
                //        {
                //            tablesCollection[dataTable.TableName] = dataTable.TableName;
                //        }

                //        DataTable[] dataTables = new DataTable[2];
                //        dataTables[0] = remoteDBAccess.DataSet.Tables[0];
                //        dataTables[0].TableName = tablesCollection[dataTables[0].TableName].ToString();
                //        dataTables[1] = remoteDBAccess.DataSet.Tables[1];
                //        dataTables[1].TableName = tablesCollection[dataTables[1].TableName].ToString();

                //        // Delete this current refnum first.	
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM trah WHERE refnum = '" + trah["refnum"].ToString().Trim() + "'");
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM tra1 WHERE refnum = '" + trah["refnum"].ToString().Trim() + "'");

                //        remoteDBAccess.Update(dataTables); 
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select max(id) from trah) where tablename = 'TRAH'");
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select max(id) from tra1) where tablename = 'TRA1'");
                //        remoteDBAccess.DataSet.Tables.Clear();
                //        #endregion
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //    MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //    DataRow trih = dbaccess.DataSet.Tables["trah"].Rows[0];

                //    #region Front End Status Roll Back due to BackEnd Saving Failure
                //    //update the status of backend trah, tra1 to 'O' and update trah(tranum) to ' '
                //    string updateString0 =
                //    "Update trah set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum ='" + trah["refnum"].ToString().Trim() + "'";
                //    dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateString0);
                //    string updateString1 =
                //    "Update tra1 set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum ='" + trah["refnum"].ToString().Trim() + "'";
                //    dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateString1);
                   
                //    #endregion
                //}
            }

            if (!BizFunctions.IsEmpty(trah["trqnum"]))
            {
                string updateTRQH = "Update TRQH set tranum='" + trah["refnum"].ToString() + "' where refnum='" + trah["trqnum"].ToString() + "'";
                dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateTRQH);
            }
        }

        # endregion

        #region Document Button Events

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow trah = dbaccess.DataSet.Tables["trah"].Rows[0];
            DataTable tra1 = dbaccess.DataSet.Tables["tra1"];
            setDefaults(dbaccess.DataSet, "trah/tra1");

         

            #region update ttqty,ttamt in header and pull latest dct info
            decimal ttqty = 0;
            int line = 100;
            foreach (DataRow dr1 in tra1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    dr1["qty"] = dr1["traqty"];
                    #region update ttqty,ttamt in header
                    if (dr1["qty"] != System.DBNull.Value)
                    {
                        if ((int)dr1["qty"] < 0)
                        {
                            MessageBox.Show("Invalid quantity");
                            dr1.SetColumnError("qty", "Invalid quantity");
                        }
                    }
                    ttqty += (int)dr1["qty"];
                    #endregion

                    dr1["line"] = line;
                    line += 100;

                    if (BizFunctions.IsEmpty(dr1["sitenum"]) && !BizFunctions.IsEmpty(dr1["docunum"]))
                    {
                        dr1["sitenum"] = GetSitenum(dr1["docunum"].ToString());
                        if (!BizFunctions.IsEmpty(dr1["sitenum"]) && BizFunctions.IsEmpty(dr1["sectorcode"]))
                        {
                            dr1["sectorcode"] = GetZone(dr1["sitenum"].ToString());
                        }
                    }
                   
                    if (BizFunctions.IsEmpty(dr1["detail"]))
                    {
                        dr1["detail"] = dr1["matname"];
                    }

                    if (trah["searchType"] == "SITM")
                    {
                        if (!BizFunctions.IsEmpty(trah["tositenum"]))
                        {
                            dr1["sitenum"] = trah["tositenum"];
                            dr1["tositenum"] = trah["tositenum"];
                            if (BizFunctions.IsEmpty(dr1["sectorcode"]))
                            {
                                dr1["sectorcode"] = GetZone(dr1["sitenum"].ToString());
                            }
                        }

                    }
                }
            }
            trah["ttqty"] = ttqty;
            #endregion

            if (!BizFunctions.IsEmpty(trah["fromsitenum"]))
            {
                trah["fromsitename"] = GetSitename(trah["fromsitenum"].ToString().Trim());
            }

            if (!BizFunctions.IsEmpty(trah["tositenum"]))
            {
                trah["tositename"] = GetSitename(trah["tositenum"].ToString().Trim());
            }


          
        }
       
        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);

              DialogResult result = MessageBox.Show("Are you sure you want to confirm Transfer Request Order?", "Bizpoint International", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

              if (result == DialogResult.No)
              {
                  e.Handle = false;
                  return;
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

        //protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        //{
        //    base.Document_Preview_Handle(sender, e);
        //    DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];

        //    if (trah["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
        //    {
        //        if (BizValidate.CheckRowState(e.DBAccess.DataSet, "trah/tra1"))
        //        {
        //            MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            e.Handle = false;
        //        }
        //    }
        //}

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            //base.Document_Print_OnClick(sender, e);
            //DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];
            //DataTable tra1 = e.DBAccess.DataSet.Tables["tra1"];
          
            //Hashtable selectedCollection = new Hashtable();
            //String commandfrom = null;
            //String commandto = null;
            //String commandtrah = null;
            //String commandtra1 = null;

            //selectedCollection.Clear();
            //command = "select * from coy";
            //selectedCollection.Add("coy", command);

            //commandfrom = "select * from sitm where sitenum = '" + trah["fromsitenum"].ToString() + "'";
            //commandto = "select * from sitm where sitenum = '" + trah["tositenum"].ToString() + "'";
            //commandtrah = "select * from trah where refnum='" + trah["refnum"].ToString() + "'";
            //commandtra1 = "select * from tra1 where refnum='" + trah["refnum"].ToString() + "'";

            //string matnums = String.Empty;

            //foreach (DataRow dr in tra1.Rows)
            //{
            //    if (dr.RowState != DataRowState.Deleted)
            //    {
            //        if (matnums == String.Empty)
            //            matnums = "'" + dr["matnum"].ToString().Trim() + "'";
            //        else
            //            matnums = matnums + ",'" + dr["matnum"].ToString().Trim() + "'";
            //    }
            //}
            //selectedCollection.Add("matm", "Select matnum,matname from matm where matnum in (" + matnums + ")");
            //selectedCollection.Add("sitmFrom", commandfrom);
            //selectedCollection.Add("sitmTo", commandto);
            //selectedCollection.Add("trah", commandtrah);
            //selectedCollection.Add("tra1", commandtra1);
            //e.DBAccess.ReadSQL(selectedCollection);

            //BizFunctions.SetCoyForPrinting(e.DBAccess, "ID=1");

            //e.ReportSource.PrintOptions.PaperSize = PaperSize.PaperA4;
            //e.DataSource = e.DBAccess.DataSet;
        }

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);

            DataRow trah = e.DBAccess.DataSet.Tables["trah"].Rows[0];
            if (trah["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "trah"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

          
          
        }

        void btnPrint_Click(object sender, EventArgs e)
        {
            DataRow trah = this.dbaccess.DataSet.Tables["trah"].Rows[0];
            DataTable tra1 = this.dbaccess.DataSet.Tables["tra1"];
            Hashtable selectedCollection = new Hashtable();

            if (!BizValidate.CheckRowState(this.dbaccess.DataSet, "trah"))
            {
                string[] arr1;

                String commandfrom = null;
                String commandto = null;
                String commandtrah = null;
                String commandtra1 = null;


                selectedCollection.Clear();
                command = "select * from coy";
                selectedCollection.Add("coy", command);

                
                commandtrah = "select * from trah where refnum='" + trah["refnum"].ToString() + "'";
                commandtra1 = "select * from tra1 where refnum='" + trah["refnum"].ToString() + "'";
                commandfrom = "select * from sitm where sitenum = '" + trah["fromsitenum"].ToString() + "'";


                string matnums = String.Empty;

                foreach (DataRow dr in tra1.Rows)
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
                //selectedCollection.Add("sitmTo", commandto);
                selectedCollection.Add("trah", commandtrah);
                selectedCollection.Add("tra1", commandtra1);
      
            

                BizFunctions.SetCoyForPrinting(this.dbaccess, "ID=0");

                if (tra1.Rows.Count > 0)
                {
                    DataTable siteDataTable = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select distinct sitenum from TRA1");

                    if (siteDataTable.Rows.Count > 0)
                    {
                        arr1 = new string[siteDataTable.Rows.Count];

                        for (int i = 0; i < siteDataTable.Rows.Count; i++)
                        {
                            if (siteDataTable.Rows[i].RowState != DataRowState.Deleted)
                            {

                                if (siteDataTable.Rows.Count > 1 && i < siteDataTable.Rows.Count - 1)
                                {
                                    arr1[i] = "'" + siteDataTable.Rows[i]["sitenum"].ToString() + "',";
                                }
                                else
                                {
                                    arr1[i] = "'" + siteDataTable.Rows[i]["sitenum"].ToString() + "'";
                                }
                            }
                        }
                        string siteList = "";
                        for (int x = 0; x < arr1.Length; x++)
                        {
                            siteList = siteList + arr1[x].ToString();
                        }
                        selectedCollection.Add("sitm", "SELECT * FROM SITM where sitenum in (" + siteList + ")");

                        this.dbaccess.ReadSQL(selectedCollection);
                    }

                }

                string[, ,] arr2;
                arr2 = new string[,,] {
                                        { { "1", "Material Issue(Form) - Internal", @"\TRA\Report\TraFrm.rpt" }, { "2", "Material Issue(Form)", @"\TRA\Report\TraFrm2.rpt" }  }
                                      };


                TRfilter tr = new TRfilter(this.dbaccess, arr2);
                tr.Show();
                tr.Focus();
            }

        }

        #endregion

        #region Check DatagridError Funtion
        private int checkdatagridError(DataRow dr)
        {
            DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT (matnum) FROM [tra1] WHERE matnum = '" + dr["matnum"] + "'");

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

            DataRow[] rows = dbaccess.DataSet.Tables["tra1"].Select("matnum ='" + dr["matnum"].ToString().Trim() + "'");

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
                System.Data.DataRow dr = dbaccess.DataSet.Tables["tra1"].NewRow();

                try
                {
                    dt1 = getdata(2, "Product Code", openfile.FileName);
                    dt2 = getdata(2, "Total Quantity Shipped", openfile.FileName);

                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        dr = dbaccess.DataSet.Tables["tra1"].NewRow();
                        dr["matnum"] = dt1.Rows[i][0];
                        dr["qty"] = dt2.Rows[i][0];
                        dbaccess.DataSet.Tables["tra1"].Rows.Add(dr);
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
                DataTable tra1ToExcel = new DataTable();
                tra1ToExcel.Columns.Add("BarCode");
                tra1ToExcel.Columns.Add("ProductCode");
                tra1ToExcel.Columns.Add("ProductName");
                tra1ToExcel.Columns.Add("UOM");
                tra1ToExcel.Columns.Add("ProductCategory");
                tra1ToExcel.Columns.Add("QTY");
                foreach (DataRow dr in dbaccess.DataSet.Tables["tra1"].Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow newRow = tra1ToExcel.NewRow();
                        newRow["BarCode"] = dr["barcode"];
                        newRow["ProductCode"] = dr["matnum"];
                        newRow["ProductName"] = dr["detail"];
                        newRow["UOM"] = dr["uom"];
                        newRow["ProductCategory"] = dr["pcatcode"];
                        newRow["QTY"] = dr["qty"];
                        tra1ToExcel.Rows.Add(newRow);
                    }
                }

                System.Windows.Forms.SaveFileDialog savefile = new SaveFileDialog();
                savefile.DefaultExt = "xls";
                savefile.Filter = "XLS(*.xls)|*.xls|TXT(*.txt)|*.txt";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    DataTableToExcel(tra1ToExcel, savefile.FileName);
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
            DataTable tra1 = this.dbaccess.DataSet.Tables["tra1"];
            DataRow trah = this.dbaccess.DataSet.Tables["trah"].Rows[0];
            # endregion
            DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from [tra1]");
            DataTable tmp = null;
            string cmd1 = "select * from matm where barcode='" + txt_barcode.Text + "'";
            DataSet tmpds1 = this.dbaccess.ReadSQLTemp("matm", cmd1);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted)
                    {
                        tmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from [tra1] where barcode='" + txt_barcode.Text + "'");
                        if (tmp.Rows.Count == 0)
                        {
                            DataRow drTrr1 = tra1.NewRow();
                            drTrr1["barcode"] = txt_barcode.Text;
                            drTrr1["qty"] = 1;
                            if (tmpds1.Tables["matm"].Rows.Count != 0)
                            {
                                drTrr1["detail"] = tmpds1.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                                drTrr1["matnum"] = tmpds1.Tables["matm"].Rows[0]["matnum"].ToString().Trim();
                                drTrr1["uom"] = tmpds1.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                                drTrr1["pcatcode"] = tmpds1.Tables["matm"].Rows[0]["pcatcode"];
                            }
                            tra1.Rows.Add(drTrr1);
                        }
                        else
                        {
                            if (dt.Rows[i]["barcode"].ToString().Trim() == txt_barcode.Text)
                            {
                                tra1.Rows[i]["qty"] = Convert.ToInt32(tra1.Rows[i]["qty"]) + 1;
                            }
                        }
                    }
                }
            }
            else
            {
                DataRow drTrr1 = tra1.NewRow();
                drTrr1["barcode"] = txt_barcode.Text;
                drTrr1["qty"] = 1;
                if (tmpds1.Tables["matm"].Rows.Count != 0)
                {
                    drTrr1["detail"] = tmpds1.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                    drTrr1["matnum"] = tmpds1.Tables["matm"].Rows[0]["matnum"].ToString().Trim();
                    drTrr1["uom"] = tmpds1.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                    drTrr1["pcatcode"] = tmpds1.Tables["matm"].Rows[0]["pcatcode"];
                }
                tra1.Rows.Add(drTrr1);
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
            DataRow trah = dbaccess.DataSet.Tables["trah"].Rows[0];
            DataTable tra1 = dbaccess.DataSet.Tables["tra1"];

            Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            DataTable oriTable = tra1;
            try
            {
                // Open Extract Form
                ExtractMATM.ExtractMATM extract = new ExtractMATM.ExtractMATM(this.dbaccess, oriTable);
                extract.ShowDialog(frm);
                #region assign line number for pon1
                int line = 100;
                foreach (DataRow dr in tra1.Rows)
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

        #region Get Template by Contract

        private void GetByCtr(string ctrNum)
        {

            DataRow TRAH = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            DataTable TRA1 = this.dbaccess.DataSet.Tables["TRA1"];
            DataTable TRA2 = this.dbaccess.DataSet.Tables["TRA2"];

            if (TRA1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA1);
            }
            if (TRA2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA2);
            }

            string getCtr = "SELECT D.sitenum,D.sectorcode,D.refnum,D.matnum,D.matname,SUM(D.qty) as qty " +
                            "From " +
                            "( " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR3 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR7 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR5 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR8 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR9 WHERE refnum='" + ctrNum + "' AND [status]<>'V' " +
                            ")A1 LEFT JOIN ADH A ON A1.refnum=A.refnum where A.[status]<>'V' " +
                            "GROUP BY A.sitenum,A.sectorcode,A1.refnum,A1.matnum,A1.matname"; 




            this.dbaccess.ReadSQL("MatTB", getCtr);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];

            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRA1 = TRA1.NewRow();
                        InsertTRA1["docunum"] = dr1["refnum"];
                        InsertTRA1["matnum"] = dr1["matnum"];
                        InsertTRA1["detail"] = dr1["matname"];
                        InsertTRA1["traqty"] = dr1["qty"];
                        InsertTRA1["stdqty"] = dr1["qty"];
                        InsertTRA1["qty"] = dr1["qty"];
                        TRA1.Rows.Add(InsertTRA1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where refnum='"+TRAH["docunum"].ToString().Trim()+"'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRA2 = TRA2.NewRow();
                        InsertTRA2["docunum"] = dr2["refnum"];
                        InsertTRA2["tositenum"] = dr2["sitenum"];
                        InsertTRA2["sectorcode"] = dr2["sectorcode"];
                        TRA2.Rows.Add(InsertTRA2);
                    }
                }
            }


        }

        #endregion

        #region Get Template by Adhoc

        private void GetByAdHoc(string adhocNum)
        {

            DataRow TRAH = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            DataTable TRA1 = this.dbaccess.DataSet.Tables["TRA1"];
            DataTable TRA2 = this.dbaccess.DataSet.Tables["TRA2"];

            if (TRA1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA1);
            }
            if (TRA2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA2);
            }

            string getADH = "SELECT D.sitenum,D.sectorcode,D.refnum,D.matnum,D.matname,SUM(D.qty) as qty " +
                            "From "+
                            "( "+
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH3 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH7 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH5 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH8 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH9 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            ")A1 LEFT JOIN ADH A ON A1.refnum=A.refnum where A.[status]<>'V' "+
                            "GROUP BY A.sitenum,A.sectorcode,A1.refnum,A1.matnum,A1.matname";




            this.dbaccess.ReadSQL("MatTB", getADH);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];

            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRA1 = TRA1.NewRow();
                        InsertTRA1["docunum"] = dr1["refnum"];
                        InsertTRA1["sitenum"] = dr1["sitenum"]; ;
                        InsertTRA1["sectorcode"] = dr1["sectorcode"];  
                        InsertTRA1["matnum"] = dr1["matnum"].ToString().Trim();
                        InsertTRA1["detail"] = dr1["matname"];
                        InsertTRA1["traqty"] = dr1["qty"];
                        InsertTRA1["stdqty"] = dr1["qty"];
                        InsertTRA1["qty"] = dr1["qty"];
                        TRA1.Rows.Add(InsertTRA1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where refnum='" + TRAH["docunum"].ToString().Trim() + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRA2 = TRA2.NewRow();
                        InsertTRA2["docunum"] = dr2["refnum"];
                        InsertTRA2["tositenum"] = dr2["sitenum"];
                        InsertTRA2["sectorcode"] = dr2["sectorcode"];
                        TRA2.Rows.Add(InsertTRA2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by By Sitenum

        private void GetBySitm(string sitenum)
        {

            DataRow TRAH = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            DataTable TRA1 = this.dbaccess.DataSet.Tables["TRA1"];
            DataTable TRA2 = this.dbaccess.DataSet.Tables["TRA2"];

            if (TRA1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA1);
            }
            if (TRA2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA2);
            }

            string getSite = "SELECT D.sitenum,D.sectorcode,D.refnum,D.matnum,D.matname,SUM(D.qty) as qty " +
                                "From " +
                                "( " +
                                    "SELECT "+
                                        "C1.REFNUM, " +
	                                    "C1.SITENUM, "+
                                        "C1.SECTORCODE, "+ 
	                                    "A.MATNUM, "+
	                                    "A.MATNAME, "+
	                                    "A.QTY "+
                                    "FROM "+ 
                                    "( "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR3 WHERE   [status]<>'V' " +
                                        "UNION  "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR7 WHERE   [status]<>'V' " +
                                        "UNION  "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR5 WHERE   [status]<>'V' " +
                                        "UNION "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR8 WHERE   [status]<>'V' " +
                                        "UNION  "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR9 WHERE   [status]<>'V' " +
                                    ")A "+
                                    "LEFT JOIN CTRH C1 ON A.refnum= C1.refnum "+
                                    "WHERE C1.sitenum='"+sitenum+"' and C1.[STATUS]<>'V'"+
                                    "UNION "+
                                    "SELECT  "+
                                        "C2.REFNUM, " +
	                                    "C2.SITENUM, "+
                                        "C2.SECTORCODE, "+
	                                    "B.MATNUM, "+
	                                    "B.MATNAME, "+
	                                    "B.QTY "+
                                    "FROM "+
                                    "( "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH3 WHERE   [status]<>'V' " +
                                        "UNION "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH7 WHERE   [status]<>'V' " +
                                        "UNION  "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH5 WHERE   [status]<>'V' " +
                                        "UNION  "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH8 WHERE   [status]<>'V' " +
                                        "UNION  "+
                                        "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM ADH9 WHERE   [status]<>'V' " +
                                     ")B "+
                                        "LEFT JOIN CTRH C2 ON B.refnum= C2.refnum "+
                                    "WHERE C2.sitenum='" + sitenum + "' AND C2.[STATUS]<>'V' "+
                                ")D  " +
                            "GROUP BY D.sitenum,D.sectorcode,D.refnum,D.matnum,D.matname";




            this.dbaccess.ReadSQL("MatTB", getSite);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];


            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRA1 = TRA1.NewRow();
                        InsertTRA1["docunum"] = dr1["refnum"];
                        InsertTRA1["sitenum"] = dr1["sitenum"]; ;
                        InsertTRA1["sectorcode"] = dr1["sectorcode"];  
                        InsertTRA1["matnum"] = dr1["matnum"].ToString().Trim();
                        InsertTRA1["detail"] = dr1["matname"];
                        InsertTRA1["traqty"] = dr1["qty"];
                        InsertTRA1["stdqty"] = dr1["qty"];
                        InsertTRA1["qty"] = dr1["qty"];
                        TRA1.Rows.Add(InsertTRA1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where sitenum='" + TRAH["tositenum"].ToString().Trim() + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRA2 = TRA2.NewRow();
                        InsertTRA2["docunum"] = dr2["refnum"];
                        InsertTRA2["fromsitenum"] = TRAH["fromsitenum"];
                        InsertTRA2["tositenum"] = dr2["sitenum"];
                        InsertTRA2["sectorcode"] = dr2["sectorcode"];
                        TRA2.Rows.Add(InsertTRA2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by Sector

        private void GetBySectorCode(string sectorcode)
        {

            DataRow TRAH = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            DataTable TRA1 = this.dbaccess.DataSet.Tables["TRA1"];
            DataTable TRA2 = this.dbaccess.DataSet.Tables["TRA2"];

            if (TRA1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA1);
            }
            if (TRA2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA2);
            }

            string getSite = "SELECT D.sitenum,D.sectorcode,D.refnum,D.matnum,D.matname,SUM(D.qty) as qty " +
                            "From " +
                            "( " +
                                "SELECT " +
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
                                "WHERE C1.sectorcode='" + sectorcode + "' AND C1.[STATUS]<>'V' " +
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
                                "WHERE C2.sectorcode='" + sectorcode + "' AND C2.[STATUS]<>'V' "+
                                ")D  " +
                            "GROUP BY D.sitenum,D.sectorcode,D.refnum,D.matnum,D.matname"; 




            this.dbaccess.ReadSQL("MatTB", getSite);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];


            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRA1 = TRA1.NewRow();
                        InsertTRA1["docunum"] = dr1["refnum"];
                        InsertTRA1["sitenum"] = dr1["sitenum"]; ;
                        InsertTRA1["sectorcode"] = dr1["sectorcode"];                  
                        InsertTRA1["matnum"] = dr1["matnum"].ToString().Trim();
                        InsertTRA1["detail"] = dr1["matname"];
                        InsertTRA1["traqty"] = dr1["qty"];
                        InsertTRA1["stdqty"] = dr1["qty"];
                        InsertTRA1["qty"] = dr1["qty"];
                        TRA1.Rows.Add(InsertTRA1);
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
                        DataRow InsertTRA2 = TRA2.NewRow();
                        InsertTRA2["docunum"] = dr2["refnum"];
                        InsertTRA2["tositenum"] = dr2["sitenum"];
                        InsertTRA2["sectorcode"] = dr2["sectorcode"];
                        TRA2.Rows.Add(InsertTRA2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by TRQ

        private void GetByTRQ(string trqnum)
        {

            DataRow TRAH = this.dbaccess.DataSet.Tables["TRAH"].Rows[0];
            DataTable TRA1 = this.dbaccess.DataSet.Tables["TRA1"];
            DataTable TRA2 = this.dbaccess.DataSet.Tables["TRA2"];

            if (TRA1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA1);
            }
            if (TRA2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRA2);
            }

            string getTRQ = "select "+
                                "refnum, " +
	                            "sectorcode, "+
	                            "sitenum, "+
	                            "matnum, "+
                                 "barcode, sum(lasttotalqty) lasttotalqty," +
                                "SUM(qty) as qty, " +
                                "SUM(approveqty) as trqqty, " +
	                            "SUM(stdqty) as stdqty ,senddate"+
                            "from TRQ1 "+
                            "Where refnum='"+trqnum+"' "+
                            "GROUP BY refnum,sectorcode,sitenum,matnum,barcode,senddate";




            this.dbaccess.ReadSQL("MatTB", getTRQ);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];


            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRA1 = TRA1.NewRow();
                        InsertTRA1["docunum"] = dr1["refnum"];
                        InsertTRA1["sectorcode"] = dr1["sectorcode"];
                        InsertTRA1["sitenum"] = dr1["sitenum"];
                        InsertTRA1["matnum"] = dr1["matnum"];
                        InsertTRA1["trqqty"] = dr1["qty"];
                        InsertTRA1["traqty"] = dr1["trqqty"];
                        InsertTRA1["lastdate"] = dr1["senddate"];
                        InsertTRA1["stdqty"] = dr1["stdqty"];
                        InsertTRA1["qty"] = dr1["trqqty"];
                        InsertTRA1["barcode"] = dr1["barcode"];
                        InsertTRA1["lasttotalqty"] = dr1["lasttotalqty"];
                        TRA1.Rows.Add(InsertTRA1);
                    }
                }
            }

            string getLocation = "SELECT refnum,sectorcode,fromsitenum,tositenum,[address],docunum from TRQ2 where refnum='" + trqnum + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRA2 = TRA2.NewRow();
                        InsertTRA2["docunum"] = dr2["docunum"];
                        InsertTRA2["fromsitenum"] = dr2["fromsitenum"];
                        InsertTRA2["tositenum"] = dr2["tositenum"];
                        InsertTRA2["sectorcode"] = dr2["sectorcode"];
                        InsertTRA2["address"] = dr2["address"];
                        TRA2.Rows.Add(InsertTRA2);
                    }
                }
            }

        }

        #endregion

        #region Header Extract Template Button Event

        void btn_ExtractTemplate_Click(object sender, EventArgs e)
        {
            DataRow TRAH = dbaccess.DataSet.Tables["TRAH"].Rows[0];
            DataTable TRA1 = dbaccess.DataSet.Tables["TRA1"];
            if (!BizFunctions.IsEmpty(TRAH["searchType"]))
            {
                if (TRAH["searchType"].ToString() == "DOC")
                {
                    if (!BizFunctions.IsEmpty(TRAH["docunum"]))
                    {
                        if (TRAH["contracttype"].ToString() == "CTRH")
                        {
                            GetByCtr(TRAH["docunum"].ToString());
                        }
                        if (TRAH["contracttype"].ToString() == "ADH")
                        {
                            GetByAdHoc(TRAH["docunum"].ToString());
                        }
                    }
                }
                else if (TRAH["searchType"].ToString() == "SITM")
                {
                    GetBySitm(TRAH["tositenum"].ToString());
                }
                else if (TRAH["searchType"].ToString() == "WHM")
                {
                    GetBySitm(TRAH["tositenum"].ToString());
                }
                else if (TRAH["searchType"].ToString() == "SEM")
                {
                    GetBySectorCode(TRAH["sectorcode"].ToString().Trim());
                }
                else if (TRAH["searchType"].ToString() == "TRQ")
                {
                    if (!BizFunctions.IsEmpty(TRAH["trqnum"]))
                    {
                        GetByTRQ(TRAH["trqnum"].ToString().Trim());
                    }
                }
                if (TRA1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in TRA1.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                       
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
                            if (!BizFunctions.IsEmpty(dr1["traqty"]))
                            {
                                if (Convert.ToDecimal(dr1["traqty"]) < 1)
                                {
                                    dr1["traqty"] = 1;
                                }
                                {
                                    dr1["traqty"] = Math.Round(Convert.ToDecimal(dr1["traqty"]));
                                }
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
    }
}

