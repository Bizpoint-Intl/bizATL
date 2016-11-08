#region Namespaces
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
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using NodaTime;
using ATL.BizModules.TextValidator;
#endregion

namespace ATL.SITM
{
    public class Voucher_SITM : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName,flag= null;

     
        #endregion

        #region Construct

        public Voucher_SITM(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_SITM.xml", moduleName, voucherBaseHelpers)
        {
            
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);


        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);


        }
        #endregion

        #region Delete On Click

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);

        }
        #endregion

        #region Cancel on Click

        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);

        }

        #endregion

        #region DocumentPage Event
        protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_Edit_Handle(sender, e);
        }

        protected override void Voucher_New_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_New_Handle(sender, e);

        }
        #endregion

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;
            DataRow sitm = e.DBAccess.DataSet.Tables["sitm"].Rows[0];
           
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;


            if (Convert.IsDBNull(sitm["trandate"]))
            {
                sitm["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }

       

        }

        private void Initialise()
        {
            DataRow sitm = dbaccess.DataSet.Tables["sitm"].Rows[0];

           
    


        }


        #endregion
   
        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow sitm = this.dbaccess.DataSet.Tables["sitm"].Rows[0];

            if (BizFunctions.IsEmpty(sitm["terminalid"]))
            {
                sitm["terminalid"] = 0;
            }
            if (Convert.ToInt32(sitm["terminalid"]) > 0)
            {
                string CheckTerminalID = "Select * from sitm where terminalid='" + sitm["terminalID"].ToString() + "' and sitenum<>'" + sitm["sitenum"].ToString() + "'";

                this.dbaccess.ReadSQL("CheckTerminalIDTb", CheckTerminalID);

                DataTable CheckTerminalIDTb = this.dbaccess.DataSet.Tables["CheckTerminalIDTb"];

                if (CheckTerminalIDTb.Rows.Count > 0)
                {
                    MessageBox.Show("Please choose another Terminal ID", "Terminal ID '" + sitm["terminalID"].ToString() + "' already Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    sitm["terminalid"] = 0;
                    e.Handle = false;
                }
            }


          
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            DataRow sitm = this.dbaccess.DataSet.Tables["SITM"].Rows[0];
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "sitm_arnum":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                    e.DefaultCondition = "(arm.arnum like '" + sitm["arnum"].ToString() + "%' OR arm.arname like '" + sitm["arnum"].ToString() + "%') and arm.[status]<>'V'";
                    break;

                case "sitm_sectorcode":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                    e.DefaultCondition = "(sem.sectorcode like '" + sitm["sectorcode"].ToString() + "%' OR sem.[desc] like '" + sitm["sectorcode"].ToString() + "%') and sem.[status]<>'V'";
                    break;


                case "sitm_prmcode":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                    e.DefaultCondition = " [status]<>'V' ";
                    break;

            }
        }
        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow sitm = dbaccess.DataSet.Tables["sitm"].Rows[0];
            switch (e.ControlName)
            {

                case "sitm_qctnum":
                    if (!BizFunctions.IsEmpty(e.CurrentRow["qctnum"]))
                    {
                        GetLatesArmInfo(e.CurrentRow["qctnum"].ToString());
                        e.CurrentRow["sitename"] = e.F2CurrentRow["sitename"];
                        e.CurrentRow["addr1"] = e.F2CurrentRow["addr1"];
                        e.CurrentRow["addr2"] = e.F2CurrentRow["addr2"];
                        e.CurrentRow["addr3"] = e.F2CurrentRow["addr3"];
                        e.CurrentRow["postalcode"] = e.F2CurrentRow["postalcode"];
                        e.CurrentRow["tel1"] = e.F2CurrentRow["tel1"];
                        e.CurrentRow["fax"] = e.F2CurrentRow["sfax"];
                        e.CurrentRow["rep1"] = e.F2CurrentRow["rep1"];
                        e.CurrentRow["rep1tel"] = e.F2CurrentRow["rep1tel"];
                        e.CurrentRow["rep2"] = e.F2CurrentRow["rep2"];
                        e.CurrentRow["rep2tel"] = e.F2CurrentRow["rep2tel"];
                        e.CurrentRow["prmcode"] = e.F2CurrentRow["prmcode"];
                    }
                    break;

                case "sitm_sempnum1":
                    {                       
                        e.CurrentRow["sempname1"] = e.F2CurrentRow["empname"];                        
                    }
                    break;

                case "sitm_sempnum2":
                    {
                        e.CurrentRow["sempname2"] = e.F2CurrentRow["empname"];
                    }
                    break;

                case "sitm_sempnum3":
                    {
                        e.CurrentRow["sempname3"] = e.F2CurrentRow["empname"];
                    }
                    break;


            }
        }

        #endregion

        #region DetailF2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            //switch (e.MappingName)
            //{
               
            //}
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow sitm = dbaccess.DataSet.Tables["sitm"].Rows[0];

            switch (e.MappingName)
            {
          

            }
        }

        #endregion

        #region  ReOpen/void

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);

        }

        protected override void Document_Reopen_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Reopen_OnClick(sender, e);

          
        }

        protected override void Document_Void_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Void_Handle(sender, e);
          
        }

        protected override void Document_Void_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Void_OnClick(sender, e);
       
        }

        #endregion

        #region Duplicate Handle
        protected override void Document_Duplicate_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Duplicate_Handle(sender, e);
            e.Handle = false;


        }
        #endregion

        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow sitm = this.dbaccess.DataSet.Tables["sitm"].Rows[0];
      
           
                                  
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataRow sitm = this.dbaccess.DataSet.Tables["sitm"].Rows[0];

            // Update SITMT
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SITMT SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            // Update Roster
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE WRR SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE WRR SET FLAG='WRR'+'" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            // Update Contract
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE CTRH SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            // Update Ad-hoc
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE CTRH SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sectorcode"].ToString() + "'");

            ////----------

            // Update 
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE ATMRLive SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            // Update Roster
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE ATMR SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE ATR SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");


            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE ATR SET FLAG='ATR'+'" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            // Update Roster
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE ATR1 SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

     

            // Update Roster
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE WKCH SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");


            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE WKCH SET FLAG='WKC'+'" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            // Update Roster
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE WKC1 SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            // Update Roster
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE MTAH SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");

            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE MTAH SET FLAG='MTA'+'" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");


            // Update Roster
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE MTA1 SET sectorcode='" + sitm["sectorcode"].ToString() + "' WHERE sitenum='" + sitm["sitenum"].ToString() + "'");


            //SELECT sectorcode FROM ATMRLive
            //SELECT sectorcode FROM ATMR
            //SELECT sectorcode FROM ATR1
            //SELECT sectorcode FROM WKCH
            //SELECT sectorcode FROM WKC1
            //SELECT sectorcode FROM MTAH
            //SELECT sectorcode FROM MTA1

    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow sitm = this.dbaccess.DataSet.Tables["SITM"].Rows[0];

            if (!BizFunctions.IsEmpty(sitm["sectorcode"]))
            {
                string[] arr1 = new string[2];

                arr1 = GetZoneSupervisor(sitm["sectorcode"].ToString());

                sitm["empnum"] = arr1[0];
                sitm["empname"] = arr1[1];

            }


            if (BizFunctions.IsEmpty(sitm["sempnum1"]))
            {
                sitm["sempname1"] = System.DBNull.Value;
            }
            if (BizFunctions.IsEmpty(sitm["sempnum2"]))
            {
                sitm["sempname2"] = System.DBNull.Value;
            }
            if (BizFunctions.IsEmpty(sitm["sempnum3"]))
            {
                sitm["sempname3"] = System.DBNull.Value;
            }

        }

        #endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);


        }

        #endregion

        #region Preview on Click

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);

        }

        #endregion

        #region Print on Click

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

        }

        #endregion

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);

        }

        #endregion
       
        #region Vouchers ColumnChanged Events
        private void Voucher_sitm_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow sitm = this.dbaccess.DataSet.Tables["sitm"].Rows[0];

            switch (e.Column.ColumnName)
            {
            
            }
        }
        #endregion

        private void GetLatesArmInfo(string qctNum)
        {
            DataRow SITM = this.dbaccess.DataSet.Tables["SITM"].Rows[0];
            string getArmLatest = "Select * from arm where qctnum='" + qctNum + "' and ISNULL(isPending,0)=0";
            this.dbaccess.ReadSQL("ArmLatest", getArmLatest);

            DataTable ArmLatest = this.dbaccess.DataSet.Tables["ArmLatest"];

            if (ArmLatest.Rows.Count > 0)
            {
                DataRow drArm = this.dbaccess.DataSet.Tables["ArmLatest"].Rows[0];

                SITM["arnum"] = drArm["arnum"];          
            }
        }

        private string[] GetZoneSupervisor(string sectorcode)
        {
            string getSup = "select s.opmgr,h.empname from sem s LEFT JOIN HEMPH h on s.opmgr=h.empnum " +
                            "where s.sectorcode='" + sectorcode.Trim() + "'";

            string[] arr1 = new string[2];

            this.dbaccess.ReadSQL("SupTB", getSup);

            DataTable SupTB = this.dbaccess.DataSet.Tables["SupTB"];

            if (SupTB != null)
            {
                if (SupTB.Rows.Count > 0)
                {
                    arr1[0] = this.dbaccess.DataSet.Tables["SupTB"].Rows[0]["opmgr"].ToString();
                    arr1[1] = this.dbaccess.DataSet.Tables["SupTB"].Rows[0]["empname"].ToString();
                }
            }
            return arr1;
        }

        private void InsertSiteMaster()
        {
            DataRow arm = this.dbaccess.DataSet.Tables["arm"].Rows[0];

            string checkSiteExist = "Select * from SITM where arnum='" + arm["arnum"].ToString() + "'";

            this.dbaccess.ReadSQL("CheckSiteTB", checkSiteExist);

            DataTable CheckSiteTB = this.dbaccess.DataSet.Tables["CheckSiteTB"];

            if (CheckSiteTB.Rows.Count > 0)
            {

            }
            else
            {
                int MaxID = BizLogicTools.Tools.getMaxID("SITM", this.dbaccess);

                string InsertSITM = "INSERT INTO [SITM] " +
                                   "([ID] " +
                                   ",[sectorcode] " +
                                   ",[sitename] " +
                                   ",[addr1] " +
                                   ",[addr2] " +
                                   ",[addr3] " +
                    //",[country] "+
                    //",[postalcode] "+
                    //",[officerqty] "+
                    //",[discamt] "+
                                   ",[billadd1] " +
                                   ",[billadd2] " +
                                   ",[billadd3] " +
                                   ",[rep1] " +
                                   ",[email] " +
                                   ",[tel1] " +
                                   ",[fax] " +
                                   ",[created] " +
                    //",[trandate] "+
                                   ",[modified] " +
                    //",[year] "+
                                   ",[status] " +
                    //",[period] "+
                                   ",[flag] " +
                                   ",[user] " +
                                   ",[guid] " +
                    //",[remark] "+
                    //",[rep2] "+
                    //",[tel2] "+
                    //",[rep1tel] "+
                    //",[rep2tel] "+
                                   ",[createdby] " +
                                   ",[arnum] " +
                    //",[terminalid] "+
                    //",[prmcode] "+
                    //",[qctnum] "+
                    //",[sitenumt] "+
                                   ",[sitenum] " +
                    //",[empnum] "+
                    //",[empname]
                                    ") " +
                    //" + arm[""].ToString() + "
                             "VALUES " +
                                   "(" + Convert.ToString(MaxID + 1) + " " +
                                   ",'" + arm["sectorcode"].ToString() + "' " +
                                   ",'" + arm["arname"].ToString() + "' " +
                                   ",'" + arm["addr1"].ToString() + "' " +
                                   ",'" + arm["addr2"].ToString() + "' " +
                                   ",'" + arm["addr3"].ToString() + "' " +
                    //",<country, nvarchar(20),> "+
                    //",<postalcode, nvarchar(20),> "+
                    //",<officerqty, int,> "+
                    //",<discamt, decimal(16,2),> "+
                                   ",'" + arm["baddr1"].ToString() + "' " +
                                   ",'" + arm["baddr2"].ToString() + "' " +
                                   ",'" + arm["baddr3"].ToString() + "' " +
                                   ",'" + arm["ptc"].ToString() + "' " +
                                   ",'" + arm["email"].ToString() + "' " +
                                   ",'" + arm["phone"].ToString() + "' " +
                                   ",'" + arm["fax"].ToString() + "' " +
                                   ",GETDATE() " +
                    //"," + arm["trandate"].ToString() + "' " +
                                   ",GETDATE() " +
                    //",<year, int,> "+
                                   ",'O' " +
                    //",<period, int,> "+
                                   ",'SITM' " +
                                   ",'" + arm["user"].ToString() + "' " +
                                   ",LOWER(REPLACE(NEWID(),'-','')) " +
                    //"," + arm["remark"].ToString() + " "+
                    //",<rep2, nvarchar(200),> "+
                    //",<tel2, nvarchar(30),> "+
                    //",<rep1tel, nvarchar(100),> "+
                    //",<rep2tel, nvarchar(100),> "+
                                   ",'" + arm["createdby"].ToString() + "' " +
                                   ",'" + arm["arnum"].ToString() + "' " +
                    //",<terminalid, nvarchar(3),> "+
                    //",<prmcode, nvarchar(20),> "+
                    //",<qctnum, nvarchar(20),> "+
                    //",<sitenumt, nvarchar(20),> "+
                                   ",'" + arm["arnum"].ToString() + "1' " +
                    //",<empnum, nvarchar(20),> "+
                    //",<empname, nvarchar(100),>
                                   ") ";

                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(InsertSITM);
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SYSID SET LASTID=(SELECT MAX(ID) FROM SITM) WHERE TABLENAME='SITM'");
            }

        }

    }
}
    

