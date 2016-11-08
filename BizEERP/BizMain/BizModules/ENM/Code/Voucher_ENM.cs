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
using System.Text.RegularExpressions;
using ATL.BizModules.UserAuthorization;
#endregion

namespace ATL.ENM
{
    public class Voucher_ENM : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName = null;
        #endregion

        #region Construct

        public Voucher_ENM(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_ENM.xml", moduleName, voucherBaseHelpers)
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
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;



            e.DBAccess.DataSet.Tables["ENM"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ENM_ColumnChanged);
            e.DBAccess.DataSet.Tables["ENM1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ENM1_ColumnChanged);

            Initialise();
                   
        }

        #endregion

        #region Initialise Components
        private void Initialise()
        {

       


            

        }
        #endregion

       


        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow ENM = this.dbaccess.DataSet.Tables["ENM"].Rows[0];
            DataTable ENM1 = this.dbaccess.DataSet.Tables["ENM1"];
            int toCount = 0;
            if (ENM1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ENM1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["email"]) && BizFunctions.IsEmpty(dr1["sendtype"]))
                        {
                            dr1["sendtype"] = "TO";
                        }
                        if (dr1["sendtype"].ToString().Trim().ToUpper() == "TO")
                        {
                            toCount = toCount + 1;
                        }
                    }
                }
            }

            if (toCount <= 0)
            {
                MessageBox.Show("Please state at least one email as 'TO'", "ATL Maintenance Pte. Ltd.", MessageBoxButtons.OK);
                e.Handle = false;
            }
        

          

  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
          
            }
        }
        #endregion

        #region DetailF2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            switch (e.MappingName)
            {
               
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            switch (e.MappingName)
            {
                case "username":
                    {
                        e.CurrentRow["empnum"] = e.F2CurrentRow["empnum"];
                    }
                    break;
            }
        }

        #endregion

        #region trq ReOpen/void

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
            DataRow ENM = this.dbaccess.DataSet.Tables["ENM"].Rows[0];
            DataTable ENM1 = this.dbaccess.DataSet.Tables["ENM1"];

           

            foreach (DataRow dr1 in ENM1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ENM, dr1, "Modulecode/ModuleName/user/flag/status/created/modified");
                }
            }

		}
                                  
        
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

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


        private void Voucher_ENM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
         
            switch (e.Column.ColumnName)
            {
               
                   
          
            }
        }


        private void Voucher_ENM1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            switch (e.Column.ColumnName)
            {           
                case "empnum":
                    {
                        e.Row["email"] = GetEmail(e.Row["empnum"].ToString());
                    }
                    break;
            }
        }

        private bool CheckIFhasExistingModuleName()
        {
            DataRow ENM = this.dbaccess.DataSet.Tables["ENM"].Rows[0];
            bool hasRecord = false;

            if (!BizFunctions.IsEmpty(ENM["modulename"]))
            {
                string str1 = "Select * from ENM where modulename='" + ENM["modulename"].ToString() + "' and modulecode<>'" + ENM["modulecode"].ToString() + "'";
                this.dbaccess.ReadSQL("dtCHECKENM", str1);

                if (this.dbaccess.DataSet.Tables["dtCHECKENM"].Rows.Count > 0)
                {
                    hasRecord = true;
                }
            }
            this.dbaccess.DataSet.Tables["dtCHECKENM"].Dispose();
            return hasRecord;
        }


        private string GetEmail(string empnum)
        {
            string email = "";
            string str1 = "Select email from hemph where empnum='"+empnum+"'";

            this.dbaccess.ReadSQL("TmpEmail", str1);

            DataTable dt1 = this.dbaccess.DataSet.Tables["TmpEmail"];

            if (dt1.Rows.Count > 0)
            {
                email = dt1.Rows[0]["email"].ToString();
            }

            return email;
        }

   
    }
}
    

