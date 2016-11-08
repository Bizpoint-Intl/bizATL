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
using BizRAD.BizBase;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;
#endregion

namespace ATL.GPFM
{
    public class Voucher_GPFM : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName,factorsFormName = null;
        protected TextBox gpfm2_ttscore = null;
        protected bool opened;
        #endregion
        
        #region Constructor

        public Voucher_GPFM(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_GPFM.xml", moduleName, voucherBaseHelpers)
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
            opened = false;
        }

        #endregion

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

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;
            opened = true;

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.factorsFormName = (e.FormsCollection["factor"] as Form).Name;

            gpfm2_ttscore = BizXmlReader.CurrentInstance.GetControl(factorsFormName, "gpfm2_ttscore") as TextBox;

        }

        #endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

            DataRow gpfm = this.dbaccess.DataSet.Tables["gpfm"].Rows[0];
            DataTable gpfm1 = this.dbaccess.DataSet.Tables["gpfm1"];
            DataTable gpfm2 = this.dbaccess.DataSet.Tables["gpfm2"];


            if (gpfm2.Rows.Count > 0)
            {
                int i = 0;
                decimal total = 0;
                foreach (DataRow dr2 in gpfm2.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr2["score"]))
                        {
                            dr2["score"] = 3.00;
                        }
                        if (BizFunctions.IsEmpty(dr2["maxscore"]))
                        {
                            dr2["maxscore"] = 5.00;
                        }

                        if (BizFunctions.IsEmpty(dr2["performfactorNo"]))
                        {
                            i = i + 1;
                            dr2["performfactorNo"] = Convert.ToString(i);
                        }

                        total = total + Convert.ToDecimal(dr2["maxscore"]);

                    }
                }

                gpfm2_ttscore.Text = total.ToString();
            }

        }
        #endregion
   
        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow gpfm = this.dbaccess.DataSet.Tables["gpfm"].Rows[0];
            DataTable gpfm1 = this.dbaccess.DataSet.Tables["gpfm1"];
            DataTable gpfm2 = this.dbaccess.DataSet.Tables["gpfm2"];
            decimal line = 0;
            if (gpfm2.Rows.Count > 0)
            {
                int i = 0;
                decimal total = 0;
                foreach (DataRow dr2 in gpfm2.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr2["score"]))
                        {
                            dr2["score"] = 3.00;
                        }
                        if (BizFunctions.IsEmpty(dr2["maxscore"]))
                        {
                            dr2["maxscore"] = 5.00;
                        }

                        if (BizFunctions.IsEmpty(dr2["performfactorNo"]))
                        {
                            i = i + 1;
                            dr2["performfactorNo"] = Convert.ToString(i);
                        }

                        //if (BizFunctions.IsEmpty(dr2["scoregroup"]))
                        //{
                        //    dr2["scoregroup"] = 1;
                        //}

                        total = total + Convert.ToDecimal(dr2["maxscore"]);
                        line = line + Convert.ToDecimal(1.00);
                        dr2["line"] = line;


                    }
                }
                gpfm2_ttscore.Text = total.ToString();
            }


            if (gpfm1.Rows.Count > 0)
            {
             
                foreach (DataRow dr1 in gpfm1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["scoregroup"]))
                        {
                            dr1["scoregroup"] = 1;
                        }
                        else if (Convert.ToInt32(dr1["scoregroup"]) == 0)
                        {
                            dr1["scoregroup"] = 1;
                        }
                      

                        foreach (DataRow dr2 in gpfm2.Rows)
                        {
                            if (dr2.RowState != DataRowState.Deleted)
                            {
                                if (dr1["groupcode"].ToString().Trim().ToUpper() == dr2["groupcode"].ToString().Trim().ToUpper())
                                {
                                    //if (BizFunctions.IsEmpty(dr2["scoregroup"]))
                                    //{
                                    //    ////dr2["scoregroup"] = dr1["scoregroup"];
                                    //    dr2["scoregroup"] = dr1["scoregroup"];
                                    //}
                                    //else if (Convert.ToInt32(dr2["scoregroup"])==0)
                                    //{
                                    //    dr2["scoregroup"] = dr1["scoregroup"];
                                    //}
                                    //else                                        
                                    //{
                                    //    dr2["scoregroup"] = 1;
                                    //}
                                    dr2["scoregroup"] = dr1["scoregroup"];
                                }
                            }
                        }
                    }
                }
            }


        }

        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

            DataRow gpfm = this.dbaccess.DataSet.Tables["gpfm"].Rows[0];
            DataTable gpfm1 = this.dbaccess.DataSet.Tables["gpfm1"];
           


        }
        #endregion

        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

        }

        # endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);


        }

        #endregion

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

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
        
            switch (e.ControlName)
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

    
    }
}
    

