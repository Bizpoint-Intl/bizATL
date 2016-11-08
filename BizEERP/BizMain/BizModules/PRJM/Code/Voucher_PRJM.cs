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

namespace ATL.PRJM
{
    public class Voucher_PRJM : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName,flag= null;

     
        #endregion

        #region Construct

        public Voucher_PRJM(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_PRJM.xml", moduleName, voucherBaseHelpers)
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
            DataRow prjm = e.DBAccess.DataSet.Tables["prjm"].Rows[0];
           
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;


            if (Convert.IsDBNull(prjm["trandate"]))
            {
                prjm["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }

       

        }

        private void Initialise()
        {
            DataRow prjm = dbaccess.DataSet.Tables["prjm"].Rows[0];

           
    


        }


        #endregion
   
        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow prjm = this.dbaccess.DataSet.Tables["prjm"].Rows[0];

        
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            DataRow prjm = this.dbaccess.DataSet.Tables["PRJM"].Rows[0];
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "prjm_arnum":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                    e.DefaultCondition = "(arm.arnum like '" + prjm["arnum"].ToString() + "%' OR arm.arname like '" + prjm["arnum"].ToString() + "%') and arm.[status]<>'V'";
                    break;

                case "prjm_sectorcode":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                    e.DefaultCondition = "(sem.sectorcode like '" + prjm["sectorcode"].ToString() + "%' OR sem.[desc] like '" + prjm["sectorcode"].ToString() + "%') and sem.[status]<>'V'";
                    break;


                case "prjm_prmcode":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                    e.DefaultCondition = " [status]<>'V' ";
                    break;

            }
        }
        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow prjm = dbaccess.DataSet.Tables["prjm"].Rows[0];
            switch (e.ControlName)
            {

                case "prjm_qctnum":
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
            DataRow prjm = dbaccess.DataSet.Tables["prjm"].Rows[0];

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
            DataRow prjm = this.dbaccess.DataSet.Tables["prjm"].Rows[0];
      
           
                                  
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataRow prjm = this.dbaccess.DataSet.Tables["prjm"].Rows[0];

            

    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow prjm = this.dbaccess.DataSet.Tables["PRJM"].Rows[0];

            if (!BizFunctions.IsEmpty(prjm["sectorcode"]))
            {
                string[] arr1 = new string[2];

                arr1 = GetZoneSupervisor(prjm["sectorcode"].ToString());

                prjm["empnum"] = arr1[0];
                prjm["empname"] = arr1[1];

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
        private void Voucher_prjm_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow prjm = this.dbaccess.DataSet.Tables["prjm"].Rows[0];

            switch (e.Column.ColumnName)
            {
            
            }
        }
        #endregion

        private void GetLatesArmInfo(string qctNum)
        {
            DataRow PRJM = this.dbaccess.DataSet.Tables["PRJM"].Rows[0];
            string getArmLatest = "Select * from arm where qctnum='" + qctNum + "' and ISNULL(isPending,0)=0";
            this.dbaccess.ReadSQL("ArmLatest", getArmLatest);

            DataTable ArmLatest = this.dbaccess.DataSet.Tables["ArmLatest"];

            if (ArmLatest.Rows.Count > 0)
            {
                DataRow drArm = this.dbaccess.DataSet.Tables["ArmLatest"].Rows[0];

                PRJM["arnum"] = drArm["arnum"];          
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

    }
}
    

