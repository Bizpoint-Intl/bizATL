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

namespace ATL.HRPM
{
    public class Voucher_HRPM : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName = null;
        #endregion

        #region Construct

        public Voucher_HRPM(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_HRPM.xml", moduleName, voucherBaseHelpers)
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



            e.DBAccess.DataSet.Tables["HRPM"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_HRPM_ColumnChanged);

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
            DataRow hrpm = this.dbaccess.DataSet.Tables["hrpm"].Rows[0];
            UserAuthorization sa = new UserAuthorization(this.moduleName.ToString());
        

            if (e.Handle && CheckIFhasExistingModuleName())
            {
                MessageBox.Show("Module Name. '" + hrpm["Modulename"].ToString() + "' has already set and can't be duplicated.", "Pico Guards Ltd. Pte.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                e.Handle = false;
            }

            if (e.Handle && BizFunctions.IsEmpty(hrpm["modulename"]))
            {
                MessageBox.Show("Module Name must not be Blank", "Pico Guards Ltd. Pte.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                e.Handle = false;
            }

            if(e.Handle && !BizFunctions.IsEmpty(hrpm["modulename"]))
            {
                 hrpm["modulecode"] = hrpm["modulename"];
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
            DataRow hrpm = this.dbaccess.DataSet.Tables["HRPM"].Rows[0];
            DataTable hrpm1 = this.dbaccess.DataSet.Tables["hrpm1"];

           

            foreach (DataRow dr1 in hrpm1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hrpm, dr1, "Modulecode/ModuleName/user/flag/status/created/modified");
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


        private void Voucher_HRPM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
         
            switch (e.Column.ColumnName)
            {
               
                   
          
            }
        }

        private bool CheckIFhasExistingModuleName()
        {
            DataRow hrpm = this.dbaccess.DataSet.Tables["hrpm"].Rows[0];
            bool hasRecord = false;

            if (!BizFunctions.IsEmpty(hrpm["modulename"]))
            {
                string str1 = "Select * from hrpm where modulename='" + hrpm["modulename"].ToString() + "' and modulecode<>'" + hrpm["modulecode"].ToString() + "'";
                this.dbaccess.ReadSQL("dtCHECKhrpm", str1);

                if (this.dbaccess.DataSet.Tables["dtCHECKhrpm"].Rows.Count > 0)
                {
                    hasRecord = true;
                }
            }
            this.dbaccess.DataSet.Tables["dtCHECKhrpm"].Dispose();
            return hasRecord;
        }

   
    }
}
    

