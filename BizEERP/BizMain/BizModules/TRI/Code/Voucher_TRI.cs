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

namespace ATL.TRI
{
    public class Voucher_TRI : BizRAD.BizApplication.VoucherBaseHelper
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
        bool trihColumnChange = true;
        bool trr1ColumnChange = true;
        bool materialFlag = false;
        string sitenum, flag = null;

        Hashtable selectsCollection = null;
        string command = null;
        DataGrid dg_detail = null;
        private string formName4SideBtn;
        protected DataTable tri1;

        TextBox txt_barcode = null;
        protected Button btn_ExtrictMATM, btn_ExtrictTemplate = null;
        private ComboBox trih_triyear, trih_trimonth = null;
        protected string towhnum = null;
        protected Label lblDisplay = null;
        #endregion

        #region Construct

        public Voucher_TRI(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_TRI.xml", moduleName, voucherBaseHelpers)
        {
            this.flag = moduleName;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "trih.flag='" + flag + "' AND trih.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (trih.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " trih.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " trih.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND trih.flag='" + flag + "' AND trih.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

        #region Delete On Click

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);

            DataTable dtD = this.dbaccess.DataSet.Tables["tri1"];

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

            ATL.BizLogicTools.Tools.setDefaults(e.DBAccess.DataSet, "trih/tri1");

            this.currentYear = Common.DEFAULT_SYSTEM_YEAR;
            this.dbaccess = e.DBAccess;
            this.formName = (e.FormsCollection["header"] as Form).Name;
            this.detailName = (e.FormsCollection["detail"] as Form).Name;

            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];

            trih["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

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
            if (trih["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                trih["agreedby"] = Common.DEFAULT_SYSTEM_USERNAME;
                string strHemph = "select * from hemph where empname= '" + Common.DEFAULT_SYSTEM_USERNAME + "'";
                e.DBAccess.ReadSQL("hemph", strHemph);
                DataTable dthemph = e.DBAccess.DataSet.Tables["hemph"];

                if (dthemph.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(trih["requestedby"]))
                    {
                        trih["requestedby"] = dthemph.Rows[0]["empnum"];
                    }
                }
            }

            if (Convert.IsDBNull(trih["trandate"]))
            {
                trih["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }
            if (Convert.IsDBNull(trih["senddate"]))
            {
                trih["senddate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);

            }

            string GetSITMTB = "Select * from SITM WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("SITMTB", GetSITMTB);

            string GetDOCTB = "Select refnum,sitenum from CTRH WHERE [STATUS]<>'V' UNION Select refnum,sitenum from ADH WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("DOCTBALL", GetDOCTB);

            // Default sitenum is always HQ for backend. Depends on POSID in appconfig.
            if (BizFunctions.IsEmpty(trih["sitenum"]))
            {
                string command = "select sitenum from posm where posnum = '" + ConfigurationManager.AppSettings.Get("POSID") + "'";
                e.DBAccess.ReadSQL("posm", command);
                DataRow posm = e.DBAccess.DataSet.Tables["posm"].Rows[0];
                trih["sitenum"] = posm["sitenum"].ToString();
                trih["tositenum"] = posm["sitenum"].ToString();

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
            if (BizFunctions.IsEmpty(trih["requestedby"]))
            {
                trih["requestedby"] = Common.DEFAULT_SYSTEM_EMPNUM;
            }
            #endregion

            #region initial controls
            e.DBAccess.DataSet.Tables["trih"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRIH_ColumnChanged);
            //e.DBAccess.DataSet.Tables["tri1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRI1_ColumnChanged);
            //Button btn_import = BizXmlReader.CurrentInstance.GetControl(formName, "btn_Import") as Button;
            ////Button btn_ExtrictGRN = BizXmlReader.CurrentInstance.GetControl(formName, "btn_ExtrictGRN") as Button;
            //btn_import.Click += new EventHandler(btn_import_Click);
            ////btn_ExtrictGRN.Click += new EventHandler(btn_ExtrictGRN_Click);
            //Button btnExport = BizXmlReader.CurrentInstance.GetControl(formName, "btn_Export") as Button;
            //btnExport.Click += new EventHandler(btnExport_Click);
            dg_detail = BizXmlReader.CurrentInstance.GetControl((e.FormsCollection["detail"] as Form).Name, "dg_detail") as DataGrid;
            this.formName4SideBtn = (sender as Form).Name;

            btn_ExtrictMATM = BizXmlReader.CurrentInstance.GetControl(formName, "btn_ExtrictMATM") as Button;
            btn_ExtrictMATM.Click += new EventHandler(btn_ExtrictMATM_Click);

            btn_ExtrictTemplate = BizXmlReader.CurrentInstance.GetControl(formName, "btn_ExtrictTemplate") as Button;
            btn_ExtrictTemplate.Click += new EventHandler(btn_ExtrictTemplate_Click);

            lblDisplay = BizXmlReader.CurrentInstance.GetControl(formName, "lbl_display") as Label;
            lblDisplay.Font = new Font(lblDisplay.Font, FontStyle.Bold);

            Button btn_Document_Void = (Button)BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, Common.DEFAULT_DOCUMENT_BTNVOID);
            btn_Document_Void.Visible = true;
            btn_Document_Void.Enabled = true;

            #endregion         

            #region disable void/reopen button for outlet user
            //allow HQ user to void and reopen TRI,but need to check whether tri extrict this tri or not.
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

            #region get towhnum
            string to = "";
            //if (Tools.isFrontEnd())
            //{
            //    to = "select whnum from whm where sitenum='" + trih["tositenum"].ToString() + "'";
            //}
            //else
            //{
            //    to = "select whnum from whm where sitenum='HQ'";
            //}
            //DataSet dt = this.dbaccess.ReadSQLTemp("to", to);
            //if (dt.Tables[0] != null)
            //{
            //    towhnum = dt.Tables[0].Rows[0]["whnum"].ToString();
            //}
            #endregion

            Initialise();

            e.DBAccess.DataSet.Tables["TRI2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRI2_ColumnChanged);

        }

       
        #endregion

        #endregion


        void Voucher_TRI2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
            trih_triyear = BizXmlReader.CurrentInstance.GetControl(formName, "trih_triyear") as ComboBox;
            trih_trimonth = BizXmlReader.CurrentInstance.GetControl(formName, "trih_trimonth") as ComboBox;
            setTriYear();
            setTriMonth();
        }

        #endregion


        private void setTriYear()
        {
            DataRow trih = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            int[] arr1 = new int[100];
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            int Year = dt.AddYears(-50).Year;

            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = Year;
                Year = Year + 1;
            }

            trih_triyear.DataSource = arr1;
            if (!BizFunctions.IsEmpty(trih["senddate"]))
            {
                trih["triyear"] = Convert.ToDateTime(trih["senddate"]).Year;
            }
            else
            {
                trih["triyear"] = dt.Year;
            }

        }


        private void setTriMonth()
        {
            DataRow trih = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            DateTime dt = new DateTime();
            dt = DateTime.Now;


            if (!BizFunctions.IsEmpty(trih["senddate"]))
            {
                if (BizFunctions.IsEmpty(trih["trimonth"]))
                {
                    trih["trimonth"] = Convert.ToDateTime(trih["senddate"]).Month;
                }
                else
                {
                    if (Convert.ToInt16(trih["trimonth"]) == 0)
                    {
                        trih["trimonth"] = Convert.ToDateTime(trih["senddate"]).Month;
                    }
                }
            }
            else
            {

                trih["trimonth"] = dt.Month;

            }


        }

        # region Column Change Event
        private void Voucher_TRIH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (trihColumnChange)
            {
                DataRow trih = this.dbaccess.DataSet.Tables["trih"].Rows[0];
                DataTable tri1 = this.dbaccess.DataSet.Tables["tri1"];

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

                                foreach (DataRow dr in tri1.Rows)
                                {
                                    if (dr.RowState != DataRowState.Deleted)
                                    {
                                        /*
                                        string command = "SELECT matnum,sitenum,SUM(qty) AS qty FROM mwt" + this.currentYear +
                                              " WHERE matnum='" + dr["matnum"].ToString().Trim() +
                                              "' AND sitenum='" + e.Row["fromsitenum"].ToString().Trim() +
                                              "' AND trandate<='" + BizFunctions.GetSafeDateString((DateTime)trih["trandate"]) +
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
                                foreach (DataRow dr in tri1.Rows)
                                {
                                    if (dr.RowState != DataRowState.Deleted)
                                    {
                                        dr["fromsitenum"] = trih["fromsitenum"];
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
                                    foreach (DataRow dr in tri1.Rows)
                                    {
                                        if (dr.RowState != DataRowState.Deleted)
                                        {
                                            dr["tositenum"] = trih["tositenum"];
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
                            foreach (DataRow dr in tri1.Rows)
                            {
                                //dr["remarks"] = e.Row["remarks"];
                            }
                            break;
                        }

                    #endregion
                }
            }
        }

        private void Voucher_TRI1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (trr1ColumnChange)
            {
                DataRow trih = dbaccess.DataSet.Tables["trih"].Rows[0];
                DataTable tri1 = dbaccess.DataSet.Tables["tri1"];
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
                        e.Row["refnum"] = trih["refnum"];
                        e.Row["trandate"] = trih["trandate"];
                        e.Row["sitenum"] = trih["sitenum"];
                        e.Row["status"] = trih["status"];
                        e.Row["agreedby"] = trih["agreedby"];
                        e.Row["sentby"] = trih["sentby"];
                        e.Row["senderremarks"] = trih["senderremarks"];
                        e.Row["requestedby"] = trih["requestedby"];
                        e.Row["fromsitenum"] = trih["fromsitenum"];
                        e.Row["tositenum"] = trih["tositenum"];
                        e.Row["year"] = trih["year"];
                        e.Row["flag"] = trih["flag"];
                        e.Row["user"] = trih["user"];
                        #endregion


                        #region Update stock on hand values
                        /*
                        string command = "select matnum,sitenum,SUM(qty) AS qty from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                              " where matnum='" + e.Row["matnum"].ToString().Trim() +
                              "' AND sitenum='" + e.Row["fromsitenum"].ToString().Trim() +
                              "' AND trandate<='" + BizFunctions.GetSafeDateString((DateTime)trih["trandate"]) +
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
            DataTable tri1 = dbaccess.DataSet.Tables["tri1"];
            DataRow trih = dbaccess.DataSet.Tables["trih"].Rows[0];

            #region update ttqty,ttamt in header
            decimal ttqty = 0;
            foreach (DataRow dr1 in tri1.Rows)
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
            
            trih["ttqty"] = ttqty;
            #endregion
        }
        #endregion

        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];
            DataTable tri1 = e.DBAccess.DataSet.Tables["tri1"];

            if (BizFunctions.IsEmpty(trih["ccnum"]))
            {
                trih["ccnum"] = "CLN";
            }
            foreach (DataRow dr1 in tri1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr1["ccnum"]))
                    {
                        dr1["ccnum"] = trih["ccnum"];
                    }
                }
            }
           

            //    # region Check for empty row and empty mat code
            //    foreach (DataRow dr1 in tri1.Rows)
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

            //    if (tri1.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Save Unsuccessful\nYou cannot save the file without any Product/Voucher!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }

            //    if (trih["fromsitenum"].ToString().Trim().CompareTo(trih["tositenum"].ToString().Trim()) == 0)
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom site and To site cannot be same !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }


            //    #endregion

            //    #region trih

            //    #region Check for fromsite and tosite duplication
            //    if (trih["fromsitenum"].ToString().Trim() == trih["tositenum"].ToString().Trim())
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom Site and To Site cannot be same", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //    }

            //    #endregion

            //    #region Validate sitenums
            //    if (BizFunctions.IsEmpty(trih["tositenum"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nSave Unsuccessful\nTo Site Code is empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", trih["tositenum"].ToString().Trim()))
            //        {
            //            MessageBox.Show("Save Unsuccessful\nSave Unsuccessful\nTo Site Code is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            e.Handle = false;
            //            return;
            //        }
            //    }

            //    if (BizFunctions.IsEmpty(trih["fromsitenum"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nFrom Site Code is empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        if (!BizValidate.CheckTableIsValid(dbaccess, "sitm", "sitenum", trih["tositenum"].ToString().Trim()))
            //        {
            //            MessageBox.Show("Save Unsuccessful\nFrom Site Code is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            e.Handle = false;
            //            return;
            //        }
            //    }
            //    #endregion

            //    #region Validate dates
            //    if (BizFunctions.IsEmpty(trih["senddate"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nSend Date is Invalid!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }

            //    #endregion

            //    #region Validate Empty Text Box
            //    if (BizFunctions.IsEmpty(trih["requestedby"]))
            //    {
            //        MessageBox.Show("Save Unsuccessful\nRequested By cannot be empty!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //    #endregion

            //    #endregion

            //    #region tri1
            //    foreach (DataRow dr in tri1.Rows)
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
            //    #endregion tri1

            //    //Backup Validation
            //    #region Final Backup Validation

            //    if (BizValidate.CheckColumnError(dbaccess.DataSet, "trih"))
            //    {
            //        MessageBox.Show("Invalid values detected in header", "Save unsuccessful");
            //        e.Handle = false;
            //        return;
            //    }
            //    else
            //    {
            //        //if (BizValidate.CheckColumnError(dbaccess.DataSet, "tri1"))
            //        //{
            //        //    MessageBox.Show("Invalid values detected in details", "Save unsuccessful");
            //        //    e.Handle = false;
            //        //    return;
            //        //}
            //    }
            //}
            //    #endregion

            if (BizFunctions.IsEmpty(trih["tositenum"]))
            {
                MessageBox.Show("Receive Site is Empty", "Save unsuccessful");
                e.Handle = false;
            }

            towhnum = trih["tositenum"].ToString();
            trih["sitenum"] = trih["tositenum"];
        }

        #region DocumentF2

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow trih = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            switch (e.ControlName)
            {
            
                case "trih_fromsitenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;
                case "trih_tositenum":
                    e.Condition = BizFunctions.F2Condition("sitenum/sitename", (sender as TextBox).Text);
                    break;

                case "trah_tranum":
                    if (trih["flag"].ToString().Trim().ToUpper() == "TRI")
                    {
                        e.DefaultCondition = " refnum not in (Select ISNULL(tranum,'') as refnum  from TRIH where [status]<>'V') and flag='TRA'  ";
                    }
                    else
                    {
                        e.DefaultCondition = " refnum not in (Select ISNULL(tranum,'') as refnum  from TRIH where [status]<>'V') and flag='LTRA'    ";
                    }
                    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow trih = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            switch (e.ControlName)
            {
                case "trih_adhnum":
                    {
                        
                    }
                    break;

                case "trih_docunum":
                    e.CurrentRow["docunum"] = e.F2CurrentRow["refnum"];
                    e.CurrentRow["tositenum"] = e.F2CurrentRow["sitenum"];
                    e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    e.CurrentRow["contracttype"] = e.F2CurrentRow["TableName"];
                    {
                        trih["searchType"] = "DOC";
                    }
                    break;

                case "trih_tositenum":                   
                    {
                        trih["searchType"] = "SITM";
                        e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    }
                    break;

                case "trih_whnum":
                    {
                        trih["searchType"] = "WHM";
                    }
                    break;

                case "trih_sectorcode":
                    {
                        trih["searchType"] = "SEM";
                    }
                    break;

                case "trih_tranum":
                    {
                        trih["searchType"] = "TRI";
                        //e.CurrentRow["tositenum"] = e.F2CurrentRow["tositenum"];
                        e.CurrentRow["fromsitenum"] = e.F2CurrentRow["fromsitenum"];
                        e.CurrentRow["tositenum"] = e.F2CurrentRow["tositenum"];
                        e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                        e.CurrentRow["ccnum"] = e.F2CurrentRow["ccnum"];
                        if (!BizFunctions.IsEmpty(trih["tranum"]))
                        {
                            trih["ccnum"] = GetCcnumFromTRA(trih["tranum"].ToString().Trim());
                            trih["projectid"] = GetProjectIDFromTRA(trih["tranum"].ToString().Trim());
                        }
                        GetByTRA(trih["tranum"].ToString().Trim());
                    
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
                    e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    e.CurrentRow["pcatcode"] = e.F2CurrentRow["pcatcode"];
                    e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    //if (e.CurrentRow["barcode"] == System.DBNull.Value)
                    //{
                    //    e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    //}
                    e.CurrentRow["retail"] = e.F2CurrentRow["stdcost"];
                    break;
                case "barcode":
                    e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    e.CurrentRow["barcode"] = e.F2CurrentRow["barcode"];
                    e.CurrentRow["pcatcode"] = e.F2CurrentRow["pcatcode"];
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    //if (e.CurrentRow["matnum"] == System.DBNull.Value)
                    //{
                    //    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    //}
                    break;
            }
        }

        #endregion

        #region tri ReOpen/void

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];

            #region ori-not allow front end user to reopen and void
            //if (Tools.isFrontEnd() || Tools.isFrontEndVoucher(dbaccess, "TRIH"))
            //{
            //    // No reopen in outlet
            //    e.Handle = false;
            //    return;
            //}
            #endregion

            #region allow outlet user to reopen/void tri,but need to check whether this tri is extricted by tri or not
            //can only void/reopen own tri.Even HQ also can't void outlets
            //if (!Tools.isFrontEnd())
            //{
            //    if (Tools.isFrontEndVoucher(dbaccess, "TRIH"))
            //    {
            //        // No reopen in outlet
            //        e.Handle = false;
            //        MessageBox.Show("Not allowed to reopen " + trih["refnum"].ToString()+ ",which is created by outlet !", "Reopen not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    else
            //    {
            //        string strTRI = "select * from trih where trinum='" + trih["refnum"].ToString() + "'";
            //        DataSet dsTRI = this.dbaccess.ReadSQLTemp("tri", strTRI);
            //        if (dsTRI != null)
            //        {
            //            if (dsTRI.Tables[0].Rows.Count > 0)
            //            {
            //                e.Handle = false;
            //                MessageBox.Show(trih["refnum"].ToString().Trim() + " has been extricted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + " ,not allow to reopen!", "Reopen not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //    string strTRI = "select * from trih where trinum='" + trih["refnum"].ToString() + "'";
            //    DataSet dsTRI = this.remoteDBAccess.ReadSQLTemp("tri", strTRI);
            //    if (dsTRI != null || dsTRI.Tables[0]!=null)
            //    {
            //        if (dsTRI.Tables[0].Rows.Count > 0)
            //        {
            //            e.Handle = false;
            //            MessageBox.Show(trih["refnum"].ToString().Trim() + " has been extricted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + " !", "Reopen not allowed,not allow to reopen!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //    DataRow trih =this.dbaccess.DataSet.Tables["trih"].Rows[0];
            //    try
            //    {
            //        // Update the backend trih and tri1 status.
            //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trih set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum = '" + trih["refnum"].ToString().Trim() + "'");
            //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update tri1 set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum = '" + trih["refnum"].ToString().Trim() + "'");
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
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];
            #region ori-not allow front end user to void
            //if (Tools.isFrontEnd())
            //{
            //    e.Handle = false;
            //}
            #endregion

            #region allow outlet user to void tri,but need to check whether this tri is extricted by tri or not
            //can only void/reopen own tri.Even HQ also can't void outlets
            //if (!Tools.isFrontEnd())
            //{
            //    if (Tools.isFrontEndVoucher(dbaccess, "TRIH"))
            //    {
            //        e.Handle = false;
            //        MessageBox.Show("Not allowed to void " + trih["refnum"].ToString() + ",which is created by outlet !", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    else
            //    {
            //        string strTRI = "select * from trih where trinum='" + trih["refnum"].ToString() + "'";
            //        DataSet dsTRI = this.dbaccess.ReadSQLTemp("tri", strTRI);
            //        if (dsTRI != null)
            //        {
            //            if (dsTRI.Tables[0].Rows.Count > 0)
            //            {
            //                e.Handle = false;
            //                MessageBox.Show(trih["refnum"].ToString().Trim() + " has been extricted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + " ,not allow to void!", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //    string strTRI = "select * from trih where trinum='" + trih["refnum"].ToString() + "'";
            //    DataSet dsTRI = this.remoteDBAccess.ReadSQLTemp("tri", strTRI);
            //    if (dsTRI != null || dsTRI.Tables[0] != null)
            //    {
            //        if (dsTRI.Tables[0].Rows.Count > 0)
            //        {
            //            e.Handle = false;
            //            MessageBox.Show(trih["refnum"].ToString().Trim() + " has been extricted by " + dsTRI.Tables["tri"].Rows[0]["refnum"].ToString() + ",not allow to void!", "Void not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];

            ////if (Tools.isFrontEnd())
            ////{
            ////    try
            ////    {
            ////        // Update the backend trih and tri1 status.
            ////        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update trih set status='" + Common.DEFAULT_DOCUMENT_STATUSV + "' where refnum = '" + trih["refnum"].ToString().Trim() + "'");
            ////        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("update tri1 set status='" + Common.DEFAULT_DOCUMENT_STATUSV + "' where refnum = '" + trih["refnum"].ToString().Trim() + "'");
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
            DataTable tri1 = e.DBAccess.DataSet.Tables["tri1"];
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];


            foreach (DataRow dr in tri1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {

                    BizFunctions.UpdateDataRow(trih, dr, "trandate/user/flag/status/created/modified");
                }
            }

            #region assgin value for heaer
            if (trih["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                trih["confirms"] = 1;
            }
            else
            {
                trih["confirms"] = 0;
            }
            if (trih["year"] == System.DBNull.Value)
            {
                trih["year"] = ((DateTime)trih["trandate"]).Year;
            }
            #endregion

            //#region Save important header information into details
            //if (tri1.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in tri1.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {
            //            //Copies some of the trih's column data to tri1
            //            BizFunctions.UpdateDataRow(trih, dr, "fromsitenum,tositenum,sentby,sitenum,senddate,trandate,year,flag,status,confirms,user,modified");
            //        }
            //    }
            //}
            //# endregion  

            #region Save into mwt
            #region tri1
            foreach (DataRow dr in tri1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(trih, dr, "refnum,fromsitenum,trandate,year,tositenum,status,year,flag,user,confirms,sitenum,created,modified");

                    if (Convert.IsDBNull(dr["acceptedqty"]))
                        dr["acceptedqty"] = 0;

                    dr["qty"] = dr["acceptedqty"];
                    dr["triqty"] = dr["acceptedqty"];

                    if (trih["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
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
                                dr_mwt["uom"] = dr["uomcode"];
                                dr_mwt["docunum"] = dr["refnum"];
                                dr_mwt["location"] = dr["tositenum"];
                                dr_mwt["qty"] = (decimal)dr["acceptedqty"];
                                //if (Tools.isFrontEnd())
                                //{
                                //dr_mwt["whnum"] = dr["tositenum"];//increase qty in main warehouse,here is the fromsitenum(local) if is outlets
                                dr_mwt["whnum"] = towhnum;//increase qty in main warehouse,here is the fromsitenum(local) if is outlets
                                //}
                                //else
                                //{
                                //    dr_mwt["whnum"] = "MWH";//increase qty in main warehouse,here is 'ADUI-MWH' if is HQ
                                //}
                                dr_mwt["guid"] = Tools.getGUID();
                                dr_mwt["trandate"] = dr["trandate"];
                                dr_mwt["year"] = dr["year"];
                                dr_mwt["created"] = dr["created"];
                                dr_mwt["modified"] = dr["modified"];
                                dr_mwt["status"] = dr["status"].ToString().Trim();
                                dr_mwt["user"] = dr["user"].ToString().Trim();
                                dr_mwt["remarks"] = dr["remarks"].ToString().Trim();
                                dr_mwt["flag"] = trih["flag"];
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
                                dr_mwt["uom"] = dr["uomcode"];
                                dr_mwt["docunum"] = dr["refnum"];
                                dr_mwt["location"] = dr["fromsitenum"];
                                dr_mwt["qty"] = -(decimal)dr["acceptedqty"];
                                dr_mwt["whnum"] = "SITWH";//Decrease qty in Stock In Transit warehouse(Location HQ)
                                dr_mwt["guid"] = Tools.getGUID();
                                dr_mwt["trandate"] = dr["trandate"];
                                dr_mwt["year"] = dr["year"];
                                dr_mwt["created"] = dr["created"];
                                dr_mwt["modified"] = dr["modified"];
                                dr_mwt["status"] = dr["status"].ToString().Trim();
                                dr_mwt["user"] = dr["user"].ToString().Trim();
                                dr_mwt["remarks"] = dr["remarks"].ToString().Trim();
                                dr_mwt["flag"] = trih["flag"];
                                mwt.Rows.Add(dr_mwt);
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion
            #endregion


        }
        #endregion
        
        #region Save End Event

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];
            DataTable tri1 = e.DBAccess.DataSet.Tables["tri1"];
            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];

            string matnums = String.Empty;

            if (trih["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                //try
                //{
                //    if (Tools.isFrontEnd())
                //    {
                //        // Fill the backend dbaccess with required tables
                //        #region Save to trih,tri1 to backend
                //        // If USER only saves, then just save to front end and back end.
                //        // If USER confirms, then ensure that front end will get saved because
                //        // mwt will be pulled from there.
                //        foreach (DataTable dataTable in e.DBAccess.DataSet.Tables)
                //        {
                //            if (dataTable.TableName == "trih" || dataTable.TableName == "tri1")
                //            {
                //                DataTable tempDataTable = dataTable.Clone();

                //                // Remove the mark columns, these do not exist in the database
                //                if (dataTable.TableName == "tri1")
                //                {
                //                    if (tempDataTable.Columns.Contains("mark"))
                //                        tempDataTable.Columns.Remove("mark");

                //                    if (dataTable.Columns.Contains("mark"))
                //                        dataTable.Columns.Remove("mark");
                //                }

                //                int id = 0;
                //                DataSet trihmaxid = remoteDBAccess.ReadSQLTemp("triid", "Select max(id) as 'id' from " + dataTable.TableName);

                //                if (trihmaxid.Tables["triid"].Rows.Count > 0)
                //                {
                //                    if (trihmaxid.Tables["triid"].Rows[0]["id"] == System.DBNull.Value)
                //                        id = 0;
                //                    else
                //                        id = Convert.ToInt32(trihmaxid.Tables["triid"].Rows[0]["id"]) + 1;
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
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM trih WHERE refnum = '" + trih["refnum"].ToString().Trim() + "'");
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM tri1 WHERE refnum = '" + trih["refnum"].ToString().Trim() + "'");

                //        remoteDBAccess.Update(dataTables); 
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select max(id) from trih) where tablename = 'TRIH'");
                //        remoteDBAccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select max(id) from tri1) where tablename = 'TRI1'");
                //        remoteDBAccess.DataSet.Tables.Clear();
                //        #endregion
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //    MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //    DataRow trih = dbaccess.DataSet.Tables["trih"].Rows[0];

                //    #region Front End Status Roll Back due to BackEnd Saving Failure
                //    //update the status of backend trih, tri1 to 'O' and update trih(trinum) to ' '
                //    string updateString0 =
                //    "Update trih set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum ='" + trih["refnum"].ToString().Trim() + "'";
                //    dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateString0);
                //    string updateString1 =
                //    "Update tri1 set status='" + Common.DEFAULT_DOCUMENT_STATUSO + "' where refnum ='" + trih["refnum"].ToString().Trim() + "'";
                //    dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateString1);
                   
                //    #endregion
                //}
            }

            if (!BizFunctions.IsEmpty(trih["tranum"]))
            {
                string updateTRIH = "Update TRAH set trinum='" + trih["refnum"].ToString() + "' where refnum='" + trih["tranum"].ToString() + "'";
                dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateTRIH);
            }
        }

        # endregion

        #region Document Button Events

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow trih = dbaccess.DataSet.Tables["trih"].Rows[0];
            DataTable tri1 = dbaccess.DataSet.Tables["tri1"];
            setDefaults(dbaccess.DataSet, "trih/tri1");

            #region update ttqty,ttamt in header and pull latest dct info
            decimal ttqty = 0;
            foreach (DataRow dr1 in tri1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    dr1["qty"] = dr1["triqty"];
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
                }
            }
            trih["ttqty"] = ttqty;
            #endregion

            if (!BizFunctions.IsEmpty(trih["fromsitenum"]))
            {
                trih["fromsitename"] = GetSitename(trih["fromsitenum"].ToString().Trim());
            }

            if (!BizFunctions.IsEmpty(trih["tositenum"]))
            {
                trih["tositename"] = GetSitename(trih["tositenum"].ToString().Trim());
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

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];

            if (trih["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "trih/tri1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];
            DataTable tri1 = e.DBAccess.DataSet.Tables["tri1"];
          
            Hashtable selectedCollection = new Hashtable();
            String commandfrom = null;
            String commandto = null;
            String commandtrih = null;
            String commandtri1 = null;

            selectedCollection.Clear();
            command = "select * from coy";
            selectedCollection.Add("coy", command);

            commandfrom = "select * from sitm where sitenum = '" + trih["fromsitenum"].ToString() + "'";
            commandto = "select * from sitm where sitenum = '" + trih["tositenum"].ToString() + "'";
            commandtrih = "select * from trih where refnum='" + trih["refnum"].ToString() + "'";
            commandtri1 = "select * from tri1 where refnum='" + trih["refnum"].ToString() + "'";

            string matnums = String.Empty;

            foreach (DataRow dr in tri1.Rows)
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
            selectedCollection.Add("trih", commandtrih);
            selectedCollection.Add("tri1", commandtri1);
            e.DBAccess.ReadSQL(selectedCollection);

            BizFunctions.SetCoyForPrinting(e.DBAccess, "ID=1");

            e.ReportSource.PrintOptions.PaperSize = PaperSize.PaperA4;
            e.DataSource = e.DBAccess.DataSet;
        }

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);
            DataRow trih = e.DBAccess.DataSet.Tables["trih"].Rows[0];
            DataTable tri1 = e.DBAccess.DataSet.Tables["tri1"];
            if (trih["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "trih/tri1"))
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
            DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT (matnum) FROM [tri1] WHERE matnum = '" + dr["matnum"] + "'");

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

            DataRow[] rows = dbaccess.DataSet.Tables["tri1"].Select("matnum ='" + dr["matnum"].ToString().Trim() + "'");

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
                System.Data.DataRow dr = dbaccess.DataSet.Tables["tri1"].NewRow();

                try
                {
                    dt1 = getdata(2, "Product Code", openfile.FileName);
                    dt2 = getdata(2, "Total Quantity Shipped", openfile.FileName);

                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        dr = dbaccess.DataSet.Tables["tri1"].NewRow();
                        dr["matnum"] = dt1.Rows[i][0];
                        dr["qty"] = dt2.Rows[i][0];
                        dbaccess.DataSet.Tables["tri1"].Rows.Add(dr);
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
                DataTable tri1ToExcel = new DataTable();
                tri1ToExcel.Columns.Add("BarCode");
                tri1ToExcel.Columns.Add("ProductCode");
                tri1ToExcel.Columns.Add("ProductName");
                tri1ToExcel.Columns.Add("UOM");
                tri1ToExcel.Columns.Add("ProductCategory");
                tri1ToExcel.Columns.Add("QTY");
                foreach (DataRow dr in dbaccess.DataSet.Tables["tri1"].Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow newRow = tri1ToExcel.NewRow();
                        newRow["BarCode"] = dr["barcode"];
                        newRow["ProductCode"] = dr["matnum"];
                        newRow["ProductName"] = dr["detail"];
                        newRow["UOM"] = dr["uom"];
                        newRow["ProductCategory"] = dr["pcatcode"];
                        newRow["QTY"] = dr["qty"];
                        tri1ToExcel.Rows.Add(newRow);
                    }
                }

                System.Windows.Forms.SaveFileDialog savefile = new SaveFileDialog();
                savefile.DefaultExt = "xls";
                savefile.Filter = "XLS(*.xls)|*.xls|TXT(*.txt)|*.txt";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    DataTableToExcel(tri1ToExcel, savefile.FileName);
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
            DataTable tri1 = this.dbaccess.DataSet.Tables["tri1"];
            DataRow trih = this.dbaccess.DataSet.Tables["trih"].Rows[0];
            # endregion
            DataTable dt = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from [tri1]");
            DataTable tmp = null;
            string cmd1 = "select * from matm where barcode='" + txt_barcode.Text + "'";
            DataSet tmpds1 = this.dbaccess.ReadSQLTemp("matm", cmd1);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted)
                    {
                        tmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from [tri1] where barcode='" + txt_barcode.Text + "'");
                        if (tmp.Rows.Count == 0)
                        {
                            DataRow drTrr1 = tri1.NewRow();
                            drTrr1["barcode"] = txt_barcode.Text;
                            drTrr1["qty"] = 1;
                            if (tmpds1.Tables["matm"].Rows.Count != 0)
                            {
                                drTrr1["detail"] = tmpds1.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                                drTrr1["matnum"] = tmpds1.Tables["matm"].Rows[0]["matnum"].ToString().Trim();
                                drTrr1["uom"] = tmpds1.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                                drTrr1["pcatcode"] = tmpds1.Tables["matm"].Rows[0]["pcatcode"];
                            }
                            tri1.Rows.Add(drTrr1);
                        }
                        else
                        {
                            if (dt.Rows[i]["barcode"].ToString().Trim() == txt_barcode.Text)
                            {
                                tri1.Rows[i]["qty"] = Convert.ToInt32(tri1.Rows[i]["qty"]) + 1;
                            }
                        }
                    }
                }
            }
            else
            {
                DataRow drTrr1 = tri1.NewRow();
                drTrr1["barcode"] = txt_barcode.Text;
                drTrr1["qty"] = 1;
                if (tmpds1.Tables["matm"].Rows.Count != 0)
                {
                    drTrr1["detail"] = tmpds1.Tables["matm"].Rows[0]["matname"].ToString().Trim();
                    drTrr1["matnum"] = tmpds1.Tables["matm"].Rows[0]["matnum"].ToString().Trim();
                    drTrr1["uom"] = tmpds1.Tables["matm"].Rows[0]["uomcode"].ToString().Trim();
                    drTrr1["pcatcode"] = tmpds1.Tables["matm"].Rows[0]["pcatcode"];
                }
                tri1.Rows.Add(drTrr1);
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

        #region Extrict
        private void btn_ExtrictMATM_Click(object sender, EventArgs e)
        {
            DataRow trih = dbaccess.DataSet.Tables["trih"].Rows[0];
            DataTable tri1 = dbaccess.DataSet.Tables["tri1"];

            Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            DataTable oriTable = tri1;
            try
            {
                // Open Extrict Form
                ExtractMATM.ExtractMATM extrict = new ExtractMATM.ExtractMATM(this.dbaccess, oriTable);
                extrict.ShowDialog(frm);
                #region assign line number for pon1
                int line = 100;
                foreach (DataRow dr in tri1.Rows)
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

            DataRow TRIH = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            DataTable TRI1 = this.dbaccess.DataSet.Tables["TRI1"];
            DataTable TRI2 = this.dbaccess.DataSet.Tables["TRI2"];

            if (TRI1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI1);
            }
            if (TRI2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI2);
            }
            
            string getCtr = "SELECT * FROM CTR3 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT * FROM CTR7 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT * FROM CTR5 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT * FROM CTR8 WHERE refnum='" + ctrNum + "' AND [status]<>'V'" +
                            "UNION "+
                            "SELECT * FROM CTR9 WHERE refnum='"+ctrNum+"' ";




            this.dbaccess.ReadSQL("MatTB", getCtr);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];

            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRI1 = TRI1.NewRow();
                        InsertTRI1["docunum"] = dr1["refnum"];
                        InsertTRI1["matnum"] = dr1["matnum"];
                        InsertTRI1["detail"] = dr1["matname"];
                        InsertTRI1["triqty"] = dr1["qty"];
                        InsertTRI1["stdqty"] = dr1["qty"];
                        InsertTRI1["qty"] = dr1["qty"];
                        TRI1.Rows.Add(InsertTRI1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where refnum='"+TRIH["docunum"].ToString().Trim()+"'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRI2 = TRI2.NewRow();
                        InsertTRI2["docunum"] = dr2["refnum"];
                        InsertTRI2["tositenum"] = dr2["sitenum"];
                        InsertTRI2["sectorcode"] = dr2["sectorcode"];
                        TRI2.Rows.Add(InsertTRI2);
                    }
                }
            }


        }

        #endregion

        #region Get Template by Adhoc

        private void GetByAdHoc(string adhocNum)
        {

            DataRow TRIH = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            DataTable TRI1 = this.dbaccess.DataSet.Tables["TRI1"];
            DataTable TRI2 = this.dbaccess.DataSet.Tables["TRI2"];

            if (TRI1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI1);
            }
            if (TRI2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI2);
            }

            string getADH = "SELECT * FROM ADH3 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT * FROM ADH7 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT * FROM ADH5 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT * FROM ADH8 WHERE refnum='" + adhocNum + "' AND [status]<>'V' " +
                            "UNION " +
                            "SELECT * FROM ADH9 WHERE refnum='" + adhocNum + "' AND [status]<>'V' ";




            this.dbaccess.ReadSQL("MatTB", getADH);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];

            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRI1 = TRI1.NewRow();
                        InsertTRI1["docunum"] = dr1["refnum"];
                        InsertTRI1["matnum"] = dr1["matnum"];
                        InsertTRI1["detail"] = dr1["matname"];
                        InsertTRI1["triqty"] = dr1["qty"];
                        InsertTRI1["stdqty"] = dr1["qty"];
                        InsertTRI1["qty"] = dr1["qty"];
                        TRI1.Rows.Add(InsertTRI1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where refnum='" + TRIH["docunum"].ToString().Trim() + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRI2 = TRI2.NewRow();
                        InsertTRI2["docunum"] = dr2["refnum"];
                        InsertTRI2["tositenum"] = dr2["sitenum"];
                        InsertTRI2["sectorcode"] = dr2["sectorcode"];
                        TRI2.Rows.Add(InsertTRI2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by By Sitenum

        private void GetBySitm(string sitenum)
        {

            DataRow TRIH = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            DataTable TRI1 = this.dbaccess.DataSet.Tables["TRI1"];
            DataTable TRI2 = this.dbaccess.DataSet.Tables["TRI2"];

            if (TRI1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI1);
            }
            if (TRI2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI2);
            }

            string getSite =    "SELECT "+
                                    "C1.REFNUM, "+
	                                "C1.SITENUM, "+
	                                "A.MATNUM, "+
	                                "A.MATNAME, "+
	                                "A.QTY "+
                                "FROM "+
                                "( "+
                                    "SELECT * FROM CTR3 WHERE   [status]<>'V' "+
                                    "UNION  "+
                                    "SELECT * FROM CTR7 WHERE   [status]<>'V' "+
                                    "UNION  "+
                                    "SELECT * FROM CTR5 WHERE   [status]<>'V' "+
                                    "UNION "+
                                    "SELECT * FROM CTR8 WHERE   [status]<>'V' "+
                                    "UNION  "+
                                    "SELECT * FROM CTR9 WHERE   [status]<>'V' "+
                                ")A "+
                                "LEFT JOIN CTRH C1 ON A.refnum= C1.refnum "+
                                "WHERE C1.sitenum='"+sitenum+"' "+
                                "UNION "+
                                "SELECT  "+
                                    "C2.REFNUM, "+
	                                "C2.SITENUM, "+
	                                "B.MATNUM, "+
	                                "B.MATNAME, "+
	                                "B.QTY "+
                                "FROM "+
                                "( "+
                                    "SELECT * FROM ADH3 WHERE   [status]<>'V' "+
                                    "UNION "+
                                    "SELECT * FROM ADH7 WHERE   [status]<>'V' "+
                                    "UNION  "+
                                    "SELECT * FROM ADH5 WHERE   [status]<>'V' "+
                                    "UNION  "+
                                    "SELECT * FROM ADH8 WHERE   [status]<>'V' "+
                                    "UNION  "+
                                    "SELECT * FROM ADH9 WHERE   [status]<>'V' "+
                                 ")B "+
                                    "LEFT JOIN CTRH C2 ON B.refnum= C2.refnum "+
                                "WHERE C2.sitenum='" + sitenum + "'";




            this.dbaccess.ReadSQL("MatTB", getSite);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];


            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRI1 = TRI1.NewRow();
                        InsertTRI1["docunum"] = dr1["refnum"];
                        InsertTRI1["matnum"] = dr1["matnum"];
                        InsertTRI1["detail"] = dr1["matname"];
                        InsertTRI1["triqty"] = dr1["qty"];
                        InsertTRI1["stdqty"] = dr1["qty"];
                        InsertTRI1["qty"] = dr1["qty"];
                        TRI1.Rows.Add(InsertTRI1);
                    }
                }
            }

            string getLocation = "select refnum,sitenum,sectorcode from vCTRH_ADH where sitenum='" + TRIH["tositenum"].ToString().Trim() + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRI2 = TRI2.NewRow();
                        InsertTRI2["docunum"] = dr2["refnum"];
                        InsertTRI2["tositenum"] = dr2["sitenum"];
                        InsertTRI2["sectorcode"] = dr2["sectorcode"];
                        TRI2.Rows.Add(InsertTRI2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by Sector

        private void GetBySectorCode(string sectorcode)
        {

            DataRow TRIH = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            DataTable TRI1 = this.dbaccess.DataSet.Tables["TRI1"];
            DataTable TRI2 = this.dbaccess.DataSet.Tables["TRI2"];

            if (TRI1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI1);
            }
            if (TRI2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI2);
            }

            string getSite = "SELECT " +
                                    "C1.REFNUM, "+
                                    "C1.SITENUM, " +
                                    "C1.SECTORCODE, " +
                                    "A.MATNUM, " +
                                    "A.MATNAME, " +
                                    "A.QTY " +
                                "FROM " +
                                "( " +
                                    "SELECT * FROM CTR3 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT * FROM CTR7 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT * FROM CTR5 WHERE   [status]<>'V' " +
                                    "UNION " +
                                    "SELECT * FROM CTR8 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT * FROM CTR9 WHERE   [status]<>'V' " +
                                ")A " +
                                "LEFT JOIN CTRH C1 ON A.refnum= C1.refnum " +
                                "WHERE C1.sectorcode='" + sectorcode + "' " +
                                "UNION " +
                                "SELECT  " +
                                    "C2.REFNUM, "+
                                    "C2.SITENUM, " +
                                    "C2.SECTORCODE, " +
                                    "B.MATNUM, " +
                                    "B.MATNAME, " +
                                    "B.QTY " +
                                "FROM " +
                                "( " +
                                    "SELECT * FROM ADH3 WHERE   [status]<>'V' " +
                                    "UNION " +
                                    "SELECT * FROM ADH7 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT * FROM ADH5 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT * FROM ADH8 WHERE   [status]<>'V' " +
                                    "UNION  " +
                                    "SELECT * FROM ADH9 WHERE   [status]<>'V' " +
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
                        DataRow InsertTRI1 = TRI1.NewRow();
                        InsertTRI1["docunum"] = dr1["refnum"];
                        InsertTRI1["sectorcode"] = dr1["sectorcode"];
                        InsertTRI1["sitenum"] = dr1["sitenum"];
                        InsertTRI1["matnum"] = dr1["matnum"];
                        InsertTRI1["detail"] = dr1["matname"];
                        InsertTRI1["triqty"] = dr1["qty"];
                        InsertTRI1["stdqty"] = dr1["qty"];
                        InsertTRI1["qty"] = dr1["qty"];
                        TRI1.Rows.Add(InsertTRI1);
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
                        DataRow InsertTRI2 = TRI2.NewRow();
                        InsertTRI2["docunum"] = dr2["refnum"];
                        InsertTRI2["tositenum"] = dr2["sitenum"];
                        InsertTRI2["sectorcode"] = dr2["sectorcode"];
                        TRI2.Rows.Add(InsertTRI2);
                    }
                }
            }

        }

        #endregion

        #region Get Template by TRA

        private void GetByTRA(string tranum)
        {

            DataRow TRIH = this.dbaccess.DataSet.Tables["TRIH"].Rows[0];
            DataTable TRI1 = this.dbaccess.DataSet.Tables["TRI1"];
            DataTable TRI2 = this.dbaccess.DataSet.Tables["TRI2"];

            if (TRI1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI1);
            }
            if (TRI2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(TRI2);
            }

            string getTRA = "select "+
	                            "sectorcode, "+
	                            "sitenum, "+
	                            "matnum, "+
	                            "trqqty, "+
                                "traqty, " +
	                            "stdqty, "+
                                "barcode, " +
                                             "retail, " +
	                            "remarks "+
                            "from TRA1 "+
                            "Where refnum='"+tranum+"'";




            this.dbaccess.ReadSQL("MatTB", getTRA);

            DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];


            if (MatTB != null)
            {
                if (MatTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in MatTB.Rows)
                    {
                        DataRow InsertTRI1 = TRI1.NewRow();
                        InsertTRI1["sectorcode"] = dr1["sectorcode"];
                        InsertTRI1["sitenum"] = dr1["sitenum"];
                        InsertTRI1["matnum"] = dr1["matnum"];
                        InsertTRI1["trqqty"] = dr1["trqqty"];
                        InsertTRI1["traqty"] = dr1["traqty"];
                        InsertTRI1["triqty"] = dr1["traqty"];
                        InsertTRI1["stdqty"] = dr1["stdqty"];
                        InsertTRI1["acceptedqty"] = dr1["traqty"];
                        InsertTRI1["remarks"] = dr1["remarks"];
                        InsertTRI1["retail"] = dr1["retail"];
                        InsertTRI1["barcode"] = dr1["barcode"];
                        TRI1.Rows.Add(InsertTRI1);
                    }
                }
            }

            string getLocation = "SELECT refnum,sectorcode,fromsitenum,tositenum,[address],docunum from TRA2 where refnum='" + tranum + "'";


            this.dbaccess.ReadSQL("DocTB", getLocation);

            DataTable DocTB = this.dbaccess.DataSet.Tables["DocTB"];

            if (DocTB != null)
            {
                if (DocTB.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in DocTB.Rows)
                    {
                        DataRow InsertTRI2 = TRI2.NewRow();
                        InsertTRI2["docunum"] = dr2["docunum"];
                        InsertTRI2["fromsitenum"] = dr2["fromsitenum"];
                        InsertTRI2["tositenum"] = dr2["tositenum"];
                        InsertTRI2["sectorcode"] = dr2["sectorcode"];
                        InsertTRI2["address"] = dr2["address"];
                        TRI2.Rows.Add(InsertTRI2);
                    }
                }
            }

        }

        #endregion

        #region Header Extrict Template Button Event

        void btn_ExtrictTemplate_Click(object sender, EventArgs e)
        {
            DataRow TRIH = dbaccess.DataSet.Tables["TRIH"].Rows[0];
            if (!BizFunctions.IsEmpty(TRIH["searchType"]))
            {
                if (TRIH["searchType"].ToString() == "DOC")
                {
                    if (!BizFunctions.IsEmpty(TRIH["docunum"]))
                    {
                        if (TRIH["contracttype"].ToString() == "CTRH")
                        {
                            GetByCtr(TRIH["docunum"].ToString());
                        }
                        if (TRIH["contracttype"].ToString() == "ADH")
                        {
                            GetByAdHoc(TRIH["docunum"].ToString());
                        }
                    }
                }
                else if (TRIH["searchType"].ToString() == "SITM")
                {
                    GetBySitm(TRIH["tositenum"].ToString());
                }
                else if (TRIH["searchType"].ToString() == "WHM")
                {
                    GetBySitm(TRIH["tositenum"].ToString());
                }
                else if (TRIH["searchType"].ToString() == "SEM")
                {
                    GetBySectorCode(TRIH["sectorcode"].ToString().Trim());
                }
                else if (TRIH["searchType"].ToString() == "TRI")
                {
                    if (!BizFunctions.IsEmpty(TRIH["tranum"]))
                    {
                        GetByTRA(TRIH["tranum"].ToString().Trim());
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

        private string GetCcnumFromTRA(string tranum)
        {
            string ccnum = "";
            string get1 = "Select ccnum from trah where refnum='" + tranum + "' ";

            this.dbaccess.ReadSQL("TempCcnutra", get1);

            DataTable dt = this.dbaccess.DataSet.Tables["TempCcnutra"];

            if (dt.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(dt.Rows[0]["ccnum"]))
                {
                    ccnum = dt.Rows[0]["ccnum"].ToString().Trim().ToUpper();
                }
            }

            return ccnum;
        }

        private string GetProjectIDFromTRA(string tranum)
        {
            string projectid = "";
            string get1 = "Select projectid from trah where refnum='" + tranum + "' ";

            this.dbaccess.ReadSQL("TempProjectIDtra", get1);

            DataTable dt = this.dbaccess.DataSet.Tables["TempProjectIDtra"];

            if (dt.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(dt.Rows[0]["projectid"]))
                {
                    projectid = dt.Rows[0]["projectid"].ToString().Trim().ToUpper();
                }
            }

            return projectid;
        }
    }
}

