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

using PicoGuards.BizLogicTools;

using PicoGuards.Schedule;

#endregion

namespace PicoGuards.SITMT
{
    public class Voucher_SITMT : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName, sitmt1FormName, sitmt2FormName, sitmt3FormName, sitmt4FormName = null;

        protected Label lbl_sitmt1Total, lbl_sitmt2Total, lbl_sitmt3Total = null;
        protected TextBox sitmt1Total_monday, sitmt1Total_tuesday, sitmt1Total_wednesday, sitmt1Total_thursday, sitmt1Total_friday,
                          sitmt1Total_saturday, sitmt1Total_sunday, sitmt2Total_monday, sitmt2Total_tuesday, sitmt2Total_wednesday, 
                          sitmt2Total_thursday,sitmt2Total_friday, sitmt2Total_saturday, sitmt2Total_sunday, sitmt3Total_monday, 
                          sitmt3Total_tuesday, sitmt3Total_wednesday, sitmt3Total_thursday, sitmt3Total_friday,sitmt3Total_saturday,
                          sitmt3Total_sunday, sitmt1Ctrh_monday, sitmt1Ctrh_tuesday, sitmt1Ctrh_wednesday, sitmt1Ctrh_thursday,
                          sitmt1Ctrh_friday, sitmt1Ctrh_saturday, sitmt1Ctrh_sunday, sitmt2Ctrh_monday, sitmt2Ctrh_tuesday, 
                          sitmt2Ctrh_wednesday, sitmt2Ctrh_thursday,sitmt2Ctrh_friday, sitmt2Ctrh_saturday, sitmt2Ctrh_sunday,
                          sitmt3Ctrh_monday, sitmt3Ctrh_tuesday, sitmt3Ctrh_wednesday, sitmt3Ctrh_thursday,sitmt3Ctrh_friday, 
                          sitmt3Ctrh_saturday, sitmt3Ctrh_sunday = null;



        Button BtnSummary = null;
        bool columnChanged;
        Schedule.ScheduleControl sc = null;
        DataTable AllowedDAys = null;
  

        #endregion
        
        #region Constructor

        public Voucher_SITMT(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_SITMT.xml", moduleName, voucherBaseHelpers)
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

            DataRow sitmt = e.DBAccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt1 = e.DBAccess.DataSet.Tables["sitmt1"];
            DataTable sitmt2 = e.DBAccess.DataSet.Tables["sitmt2"];
            DataTable sitmt3 = e.DBAccess.DataSet.Tables["sitmt3"];
            DataTable sitmt4 = e.DBAccess.DataSet.Tables["sitmt4"];
            DataTable sitmt5 = e.DBAccess.DataSet.Tables["sitmt5"];
            DataTable sitmt6 = e.DBAccess.DataSet.Tables["sitmt6"];
            DataTable hemph = e.DBAccess.DataSet.Tables["hemph"];
            string scheduleoption = sitmt["scheduleoption"].ToString();


            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.sitmt1FormName = (e.FormsCollection["dayshift"] as Form).Name;
            this.sitmt2FormName = (e.FormsCollection["nightshift"] as Form).Name;
            this.sitmt3FormName = (e.FormsCollection["concierge"] as Form).Name;
            this.sitmt4FormName = (e.FormsCollection["relief"] as Form).Name;
       
         

            #region Schedule Radio Buttons

            #endregion

            e.DBAccess.DataSet.Tables["sitmt1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM1_ColumnChanged);
            e.DBAccess.DataSet.Tables["sitmt2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM2_ColumnChanged);
            e.DBAccess.DataSet.Tables["sitmt3"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM3_ColumnChanged);
            e.DBAccess.DataSet.Tables["sitmt4"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM4_ColumnChanged);
            
            


            Initialise();

            
                GetSitmt1FooterTotals();                
                        
                GetSitmt2FooterTotals();
                      
                GetSitmt3FooterTotals();
            

            if (!BizFunctions.IsEmpty(sitmt["docunum"]))
            {
                GetSitmt1CtrhFooterTotals();
                GetSitmt2CtrhFooterTotals();
                GetSitmt3CtrhFooterTotals();
            }


           
        }

        #endregion

        #region Initialise


        private void Initialise()
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];

            sitmt1Total_monday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_monday") as TextBox;
            sitmt1Total_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_tuesday") as TextBox;
            sitmt1Total_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_wednesday") as TextBox;
            sitmt1Total_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_thursday") as TextBox;
            sitmt1Total_friday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_friday") as TextBox;
            sitmt1Total_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_saturday") as TextBox;
            sitmt1Total_sunday  = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_sunday") as TextBox;

            sitmt2Total_monday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_monday") as TextBox;
            sitmt2Total_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_tuesday") as TextBox;
            sitmt2Total_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_wednesday") as TextBox;
            sitmt2Total_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_thursday") as TextBox;
            sitmt2Total_friday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_friday") as TextBox;
            sitmt2Total_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_saturday") as TextBox;
            sitmt2Total_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_sunday") as TextBox;

            sitmt3Total_monday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_monday") as TextBox;
            sitmt3Total_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_tuesday") as TextBox;
            sitmt3Total_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_wednesday") as TextBox;
            sitmt3Total_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_thursday") as TextBox;
            sitmt3Total_friday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_friday") as TextBox;
            sitmt3Total_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_saturday") as TextBox;
            sitmt3Total_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_sunday") as TextBox;

            sitmt1Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_monday") as TextBox;
            sitmt1Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_tuesday") as TextBox;
            sitmt1Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_wednesday") as TextBox;
            sitmt1Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_thursday") as TextBox;
            sitmt1Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_friday") as TextBox;
            sitmt1Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_saturday") as TextBox;
            sitmt1Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_sunday") as TextBox;

            sitmt2Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_monday") as TextBox;
            sitmt2Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_tuesday") as TextBox;
            sitmt2Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_wednesday") as TextBox;
            sitmt2Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_thursday") as TextBox;
            sitmt2Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_friday") as TextBox;
            sitmt2Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_saturday") as TextBox;
            sitmt2Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_sunday") as TextBox;

            sitmt3Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_monday") as TextBox;
            sitmt3Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_tuesday") as TextBox;
            sitmt3Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_wednesday") as TextBox;
            sitmt3Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_thursday") as TextBox;
            sitmt3Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_friday") as TextBox;
            sitmt3Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_saturday") as TextBox;
            sitmt3Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_sunday") as TextBox;

            //sitmt1_dayshiftcode = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1_dayshiftcode") as TextBox;


            //sitmt2_nightshiftcode = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2_nightshiftcode") as TextBox;


            //sitmt3_concierge = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3_concierge") as TextBox;


            //BtnSummary = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Summary") as Button;
            //BtnSummary.Click +=new EventHandler(BtnSummary_Click);

            //sitmt1_dayshiftcode.Text = Getshiftcode("SITMT1", sitmt["sitenumt"].ToString());
            //sitmt2_nightshiftcode.Text = Getshiftcode("SITMT2", sitmt["sitenumt"].ToString());
            //sitmt3_concierge.Text = Getshiftcode("SITMT3", sitmt["sitenumt"].ToString());

            if (!BizFunctions.IsEmpty(sitmt["sitenumt"]))
            {
                if (sitmt["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSO || sitmt["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSP)
                {
                    sc = new ScheduleControl(sitmt["docunum"].ToString().Trim(), sitmt["docunum"].ToString().Trim().Substring(0, 3),"SITMT");
                }
            }

        }

        #endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

        }
        #endregion

        #region Other TextBox Click and KeyDown Events

        //protected void sitmt1_dayshiftcode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F2)
        //    {
        //        DataTable sitmt1 = this.dbaccess.DataSet.Tables["sitmt1"];
        //        sitmt1_dayshiftcode.Text = BizLogicTools.Tools.GetF2KeyDown(sender, e, null, null, "SHM", "shiftcode");
        //        foreach (DataRow dr1 in sitmt1.Rows)
        //        {
        //            if (dr1.RowState != DataRowState.Deleted)
        //            {
        //                dr1["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
        //            }
        //        }
        //    }
        //}
             
        //protected void sitmt1_dayshiftcode_Click(object sender, System.EventArgs e)
        //{
        //    DataTable sitmt1 = this.dbaccess.DataSet.Tables["sitmt1"];
        //    sitmt1_dayshiftcode.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "SHM", "shiftcode");
        //     foreach (DataRow dr1 in sitmt1.Rows)
        //    {
        //        if (dr1.RowState != DataRowState.Deleted)
        //        {
        //            dr1["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
        //        }
        //    }
        //}

        //protected void sitmt2_nightshiftcode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F2)
        //    {
        //        DataTable sitmt2 = this.dbaccess.DataSet.Tables["sitmt2"];
        //        sitmt2_nightshiftcode.Text = BizLogicTools.Tools.GetF2KeyDown(sender, e, null, null, "SHM", "shiftcode");
        //        foreach (DataRow dr1 in sitmt2.Rows)
        //        {
        //            if (dr1.RowState != DataRowState.Deleted)
        //            {
        //                dr1["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
        //            }
        //        }
        //    }

        //}

        //protected void sitmt2_nightshiftcode_Click(object sender, System.EventArgs e)
        //{
        //    DataTable sitmt2 = this.dbaccess.DataSet.Tables["sitmt2"];
        //    sitmt2_nightshiftcode.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "SHM", "shiftcode");
        //     foreach (DataRow dr1 in sitmt2.Rows)
        //    {
        //        if (dr1.RowState != DataRowState.Deleted)
        //        {
        //            dr1["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
        //        }
        //    }

        //}

        //protected void sitmt3_concierge_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F2)
        //    {
        //        DataTable sitmt3 = this.dbaccess.DataSet.Tables["sitmt3"];
        //        sitmt3_concierge.Text = BizLogicTools.Tools.GetF2KeyDown(sender, e,null,null, "SHM", "shiftcode");
        //        foreach (DataRow dr1 in sitmt3.Rows)
        //        {
        //            if (dr1.RowState != DataRowState.Deleted)
        //            {
        //                dr1["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
        //            }
        //        }
        //    }


        //}

        //protected void sitmt3_concierge_Click(object sender, System.EventArgs e)
        //{
        //    DataTable sitmt3 = this.dbaccess.DataSet.Tables["sitmt3"];
        //    sitmt3_concierge.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "SHM", "shiftcode");
        //     foreach (DataRow dr1 in sitmt3.Rows)
        //    {
        //        if (dr1.RowState != DataRowState.Deleted)
        //        {
        //            dr1["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
        //        } 
        //    }
        //}

        #endregion

        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);



            DataRow sitmt = e.DBAccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt1 = e.DBAccess.DataSet.Tables["sitmt1"];        
            DataTable sitmt2 = e.DBAccess.DataSet.Tables["sitmt2"];        
            DataTable sitmt3 = e.DBAccess.DataSet.Tables["sitmt3"];           
            DataTable sitmt4 = e.DBAccess.DataSet.Tables["sitmt4"];        
            DataTable sitmt5 = e.DBAccess.DataSet.Tables["sitmt5"];
            DataTable sitmt6 = e.DBAccess.DataSet.Tables["sitmt6"];

            if (e.Handle && sitmt1.Rows.Count > 0)
            {
                foreach(DataRow dr1 in sitmt1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["empnum"]))
                        {
                            dr1["empname"] = GetEmpname(dr1["empnum"].ToString());
                            dr1["matnum"] = GetMatnum(dr1["empnum"].ToString());
                        }
                        if (BizFunctions.IsEmpty(dr1["shiftcode"]))
                        {
                            MessageBox.Show("Day Shift has Rows But Shift Code is Empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                        }
                    }
                }
            }

            if (e.Handle && sitmt2.Rows.Count > 0)
            {
                foreach (DataRow dr2 in sitmt2.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr2["empnum"]))
                        {                          
                            dr2["empname"] = GetEmpname(dr2["empnum"].ToString());
                            dr2["matnum"] = GetMatnum(dr2["empnum"].ToString());
                        }
                        if (BizFunctions.IsEmpty(dr2["shiftcode"]))
                        {
                            MessageBox.Show("Night Shift has Rows But Shift Code is Empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                        }
                    }
                }
            }

            if (e.Handle && sitmt3.Rows.Count > 0)
            {
                foreach (DataRow dr3 in sitmt3.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr3["empnum"]))
                        {
                            dr3["empname"] = GetEmpname(dr3["empnum"].ToString());
                            dr3["matnum"] = GetMatnum(dr3["empnum"].ToString());
                        }
                        if (BizFunctions.IsEmpty(dr3["shiftcode"]))
                        {
                            MessageBox.Show("Concierge Shift has Rows But Shift Code is Empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                        }
                    }
                }
            }

            if (e.Handle && sitmt4.Rows.Count > 0)
            {
                foreach (DataRow dr4 in sitmt4.Rows)
                {
                    if (dr4.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr4["empnum"]))
                        {
                            dr4["empname"] = GetEmpname(dr4["empnum"].ToString());
                            dr4["matnum"] = GetMatnum(dr4["empnum"].ToString());
                        }                       
                    }
                }
            }

            
                GetSitmt1FooterTotals();
          

         
                GetSitmt2FooterTotals();
          

           
                GetSitmt3FooterTotals();
          
    
            if (e.Handle && !isDayBalanced())
            {
                MessageBox.Show("Day Shift is not Tally with the Contract", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handle = false;
            }

            if (e.Handle && !isNightBalanced())
            {
                MessageBox.Show("Night Shift is not Tally with the Contract", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handle = false;
            }

            if (e.Handle && !isConciergeBalanced())
            {
                MessageBox.Show("Concierge Shift is not Tally with the Contract", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handle = false;
            }

            if (e.Handle && !isDuplicateInShift("sitmt1", "sitmt2", "sitmt3", "empnum"))
            {
                e.Handle = false;
            }

            if (e.Handle && !isDuplicateInShift("sitmt2", "sitmt1", "sitmt3", "empnum"))
            {
                e.Handle = false;
            }

            if (e.Handle && !isDuplicateInShift("sitmt3", "sitmt2", "sitmt1", "empnum"))
            {
                e.Handle = false;
            }

        }

        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

            DataRow sitmt = e.DBAccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt1 = e.DBAccess.DataSet.Tables["sitmt1"];
            DataTable sitmt1sum = e.DBAccess.DataSet.Tables["sitmt1sum"];
            DataTable sitmt2 = e.DBAccess.DataSet.Tables["sitmt2"];
            DataTable sitmt2sum = e.DBAccess.DataSet.Tables["sitmt2sum"];
            DataTable sitmt3 = e.DBAccess.DataSet.Tables["sitmt3"];
            DataTable sitmt3sum = e.DBAccess.DataSet.Tables["sitmt3sum"];
            DataTable sitmt4 = e.DBAccess.DataSet.Tables["sitmt4"];
            DataTable sitmt4sum = e.DBAccess.DataSet.Tables["sitmt4sum"];
            DataTable sitmt5 = e.DBAccess.DataSet.Tables["sitmt5"];
            DataTable sitmt6 = e.DBAccess.DataSet.Tables["sitmt6"];
            DataTable sitmtall = e.DBAccess.DataSet.Tables["sitmtall"];

            
        
            
            #region FAMR

            foreach (DataRow dr1 in sitmt1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr1, "sitenumt/user/flag/status/created/modified");
        
                }

            }
            foreach (DataRow dr1s in sitmt1sum.Rows)
            {
                
                if (dr1s.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr1s, "sitenumt/user/flag/status/created/modified");
                    
                }

            }

            #endregion

            #region HCNR
            foreach (DataRow dr2 in sitmt2.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr2, "sitenumt/user/flag/status/created/modified");
    

                }

            }

            foreach (DataRow dr2s in sitmt2sum.Rows)
            {
                if (dr2s.RowState != DataRowState.Deleted)
                {

                    BizFunctions.UpdateDataRow(sitmt, dr2s, "sitenumt/user/flag/status/created/modified");

                }

            }

            #endregion

            #region PFMEDU
            foreach (DataRow dr3 in sitmt3.Rows)
            {
                if (dr3.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr3, "sitenumt/user/flag/status/created/modified");

                }

            }

            foreach (DataRow dr3s in sitmt3sum.Rows)
            {
                if (dr3s.RowState != DataRowState.Deleted)
                {

                    BizFunctions.UpdateDataRow(sitmt, dr3s, "sitenumt/user/flag/status/created/modified");

                }

            }

            #endregion

            #region PFMER
            foreach (DataRow dr4 in sitmt4.Rows)
            {
                if (dr4.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr4, "sitenumt/user/flag/status/created/modified");
                }
            }

            foreach (DataRow dr4s in sitmt4sum.Rows)
            {
                if (dr4s.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr4s, "sitenumt/user/flag/status/created/modified");

                }
            }

            #endregion

            #region PFMER
            foreach (DataRow dr5 in sitmt5.Rows)
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr5, "sitenumt/user/flag/status/created/modified");
                }
            }
            #endregion

            #region PFMER
            foreach (DataRow dr6 in sitmt6.Rows)
            {
                if (dr6.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr6, "sitenumt/user/flag/status/created/modified");
                }

            }
            #endregion

            #region PFMER
            //foreach (DataRow dr7 in sitmtall.Rows)
            //{
            //    if (dr7.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr7, "refnum/user/flag/status/created/modified");
            //    }

            //}
            #endregion


            Summary();
        }
        #endregion

        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataTable sitmt1 = e.DBAccess.DataSet.Tables["sitmt1"];
            DataTable sitmt2 = e.DBAccess.DataSet.Tables["sitmt2"];
            DataTable sitmt3 = e.DBAccess.DataSet.Tables["sitmt3"];
            DataTable sitmt4 = e.DBAccess.DataSet.Tables["sitmt4"];
            DataTable sitmt5 = e.DBAccess.DataSet.Tables["sitmt5"];
            DataTable sitmt6 = e.DBAccess.DataSet.Tables["sitmt6"];                    

            #region FAMR

            foreach (DataRow dr1 in sitmt1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if(!BizFunctions.IsEmpty(dr1["empnum"].ToString()))
                    {
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr1["empnum"].ToString()));
                    }

                }

            }
            #endregion

            

            #region HCNR
            foreach (DataRow dr2 in sitmt2.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr2["empnum"].ToString()))
                    {
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr2["empnum"].ToString()));
                    }

                }

            }

            #endregion

            #region PFMEDU
            foreach (DataRow dr3 in sitmt3.Rows)
            {
                if (dr3.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr3["empnum"].ToString()))
                    {
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr3["empnum"].ToString()));
                    }
                }

            }
            #endregion

            #region PFMER
            foreach (DataRow dr4 in sitmt4.Rows)
            {
                if (dr4.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr4["empnum"].ToString()))
                    {
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr4["empnum"].ToString()));
                    }
                }
            }

            #endregion

            #region PFMER
            foreach (DataRow dr5 in sitmt5.Rows)
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr5["empnum"].ToString()))
                    {
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr5["empnum"].ToString()));
                    }
                }
            }
            #endregion

         
        }

        # endregion

        #region Update Employee Table Sitenumi

        private string UpdateHemph(string empno)
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];
            string sql1 = "Update hemph set sitenumi='" + sitmt["sitenumi"].ToString() + "', sectorcode='" + sitmt["sectorcode"].ToString() + "' where empnum='" + empno + "' ";

            return sql1;
        }
        #endregion

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
                case "sitmt_docunum":
                    {
                        e.Condition = "vCTRH_ADH.refnum not in(Select docunum as refnum from sitmt where status<>'V')";
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
                    
                case "empnum":
                    if (e.CurrentRow.Table.TableName == "sitmt1")
                    {
                        e.Condition = "vw_hemph.matnum ='" + e.CurrentRow["matnum"].ToString() + "'";
                        //if (sitmt1_dayshiftcode.Text == string.Empty)
                        //{
                        //    MessageBox.Show("Day ShiftCode Can't be empty", "Pico Guards Ltd. Pte.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //    e.Handle = false;
                        //}
                    }
                    if (e.CurrentRow.Table.TableName == "sitmt2")
                    {
                        e.Condition = "vw_hemph.matnum ='" + e.CurrentRow["matnum"].ToString() + "'";
                        //if (sitmt2_nightshiftcode.Text == string.Empty)
                        //{
                        //    MessageBox.Show("Night ShiftCode Can't be empty", "Pico Guards Ltd. Pte.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //    e.Handle = false;
                        //}
                    }
                    if (e.CurrentRow.Table.TableName == "sitmt3")
                    {
                        e.Condition = "vw_hemph.matnum ='" + e.CurrentRow["matnum"].ToString() + "'";
                        //if (sitmt3_concierge.Text == string.Empty)
                        //{
                        //    MessageBox.Show("Concierge ShiftCode Can't be empty", "Pico Guards Ltd. Pte.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //    e.Handle = false;
                        //}
                    }
                    break;
                
                    
               
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow sitmt = dbaccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt1 = dbaccess.DataSet.Tables["sitmt1"];
            DataTable sitmt2 = dbaccess.DataSet.Tables["sitmt2"];
            DataTable sitmt3 = dbaccess.DataSet.Tables["sitmt3"];

            switch (e.ControlName)
            {
                case "sitmt_docunum":
                    e.CurrentRow["docunum"] = e.F2CurrentRow["refnum"];
                    e.CurrentRow["contracttype"] = e.F2CurrentRow["TableName"];
                    if (!BizFunctions.IsEmpty(sitmt["docunum"].ToString().Trim()) || sitmt["docunum"].ToString().Trim() != string.Empty)
                    {
                        #region Extraction of Information From Contract Information(CTRH)
                        string strExtractCTR = "Select * from "+ e.F2CurrentRow["TableName"].ToString() +" where refnum= '" + sitmt["docunum"].ToString().Trim() + "' ";
                        this.dbaccess.ReadSQL("ctrTmp", strExtractCTR);
                        DataTable ctrTmp = this.dbaccess.DataSet.Tables["ctrTmp"] ;
                        //ClearHeader();

                        if (ctrTmp.Rows.Count > 0)
                        {
                           

                            DataRow ctrTmpDR = dbaccess.DataSet.Tables["ctrTmp"].Rows[0];
                            sitmt["docunum"] = ctrTmpDR["refnum"].ToString();
                            sitmt["docunum"] = ctrTmpDR["refnum"].ToString();
                            sitmt["sitenumi"] = ctrTmpDR["sitenumi"].ToString();
                            sitmt["sitename"] = ctrTmpDR["sitename"].ToString();
                            sitmt["coy"] = ctrTmpDR["coy"].ToString();
                            sitmt["coyname"] = ctrTmpDR["coyname"].ToString();
                            sitmt["sectorcode"] = ctrTmpDR["sectorcode"].ToString();
                            sitmt["addr1"] = ctrTmpDR["addr1"].ToString();
                            sitmt["addr2"] = ctrTmpDR["addr2"].ToString();
                            sitmt["addr3"] = ctrTmpDR["addr3"].ToString();
                            sitmt["postalcode"] = ctrTmpDR["postalcode"].ToString();
                            sitmt["officerqty"] = ctrTmpDR["officerqty"].ToString();
                            sitmt["tel1"] = ctrTmpDR["tel1"].ToString();
                            sitmt["fax"] = ctrTmpDR["fax"].ToString();
                            sitmt["rep1"] = ctrTmpDR["rep1"].ToString();
                            sitmt["rep1tel"] = ctrTmpDR["rep1tel"].ToString();
                            sitmt["rep2"] = ctrTmpDR["rep2"].ToString();
                            sitmt["rep2tel"] = ctrTmpDR["rep2tel"].ToString();
                            sitmt["duty"] = ctrTmpDR["duty"].ToString();
                            sitmt["event"] = ctrTmpDR["event"].ToString();
                            sitmt["schedule"] = ctrTmpDR["schedule"].ToString();
      
                            
                            //sitmt["isdaily"] = ctrTmpDR["isdaily"].ToString();
                            //sitmt["isweekdays"] = ctrTmpDR["isweekdays"].ToString();
                            //sitmt["isweekend"] = ctrTmpDR["isweekend"].ToString();

                            //sitmt["ispubhol"] = ctrTmpDR["ispubhol"].ToString();
                            sitmt["sinstruction"] = ctrTmpDR["sinstruction"].ToString();
                            sitmt["remark"] = ctrTmpDR["remark"].ToString();
                            sitmt["contractdate"] = ctrTmpDR["trandate"];
                            sitmt["commencedate"] = ctrTmpDR["commencedate"];
                            sitmt["enddate"] = ctrTmpDR["enddate"];

                            BizFunctions.DeleteAllRows(sitmt1);
                            BizFunctions.DeleteAllRows(sitmt2);
                            BizFunctions.DeleteAllRows(sitmt3);

                             sc = new ScheduleControl(sitmt["docunum"].ToString().Trim(), sitmt["docunum"].ToString().Trim().Substring(0, 3),"SITMT");

                             string ManPowerSchedule = "";

                             if (e.CurrentRow["contracttype"].ToString().Trim() == "CTRH")
                             {
                                 ManPowerSchedule = "select * from CTR1 where [status]<>'V' and refnum='" + sitmt["docunum"].ToString().Trim() + "' UNION ALL select * from ADH1 where [status]<>'V' and ctrnum='" + sitmt["docunum"].ToString().Trim() + "'";
                             }
                             else
                             {
                                 ManPowerSchedule = "select * from ADH1 where [status]<>'V' and refnum='" + sitmt["docunum"].ToString().Trim() + "'";
                             }
                             this.dbaccess.ReadSQL("ManPowerSchedule", ManPowerSchedule);

                             DataTable DtManPowerSchedule = this.dbaccess.DataSet.Tables["ManPowerSchedule"];

                             if (DtManPowerSchedule.Rows.Count > 0)
                             {
                                 
                                 foreach (DataRow dr1 in DtManPowerSchedule.Rows)
                                 {
                                     if (dr1.RowState != DataRowState.Deleted)
                                     {
                                         #region if days are null then

                                         if (BizFunctions.IsEmpty(dr1["monday"]))
                                         {
                                             dr1["monday"] = 0;
                                         }
                                         if (BizFunctions.IsEmpty(dr1["tuesday"]))
                                         {
                                             dr1["tuesday"] = 0;
                                         }
                                         if (BizFunctions.IsEmpty(dr1["wednesday"]))
                                         {
                                             dr1["wednesday"] = 0;
                                         }
                                         if (BizFunctions.IsEmpty(dr1["thursday"]))
                                         {
                                             dr1["thursday"] = 0;
                                         }
                                         if (BizFunctions.IsEmpty(dr1["friday"]))
                                         {
                                             dr1["friday"] = 0;
                                         }
                                         if (BizFunctions.IsEmpty(dr1["saturday"]))
                                         {
                                             dr1["saturday"] = 0;
                                         }
                                         if (BizFunctions.IsEmpty(dr1["sunday"]))
                                         {
                                             dr1["sunday"] = 0;
                                         }

                                         if (BizFunctions.IsEmpty(dr1["officerqty"]))
                                         {
                                             dr1["officerqty"] = 0;
                                         }

                                         #endregion

                                         #region Day Shift

                                         if (dr1["ShiftType"].ToString() == "D")
                                         {
                                             for(int i=0;i <= Convert.ToInt32(dr1["officerqty"])-1; i++)
                                             {
                                                 DataRow InsertSitm1 = sitmt1.NewRow();
                                                 InsertSitm1["shiftcode"] = dr1["shiftcode"];
                                                 InsertSitm1["matnum"] = dr1["matnum"];

                                                 if ((bool)dr1["monday"])
                                                 {
                                                     InsertSitm1["monday"] = "X";
                                                 }
                                                 if ((bool)dr1["tuesday"])
                                                 {
                                                     InsertSitm1["tuesday"] = "X";
                                                 }
                                                 if ((bool)dr1["wednesday"])
                                                 {
                                                     InsertSitm1["wednesday"] = "X";
                                                 }
                                                 if ((bool)dr1["thursday"])
                                                 {
                                                     InsertSitm1["thursday"] = "X";
                                                 }
                                                 if ((bool)dr1["friday"])
                                                 {
                                                     InsertSitm1["friday"] = "X";
                                                 }
                                                 if ((bool)dr1["saturday"])
                                                 {
                                                     InsertSitm1["saturday"] = "X";
                                                 }
                                                 if ((bool)dr1["sunday"])
                                                 {
                                                     InsertSitm1["sunday"] = "X";
                                                 }

                                                 sitmt1.Rows.Add(InsertSitm1);
                                             }                                                                                                                                      
                                         }

                                         #endregion
                                     
                                         #region Night Shift

                                         if (dr1["ShiftType"].ToString() == "N")
                                         {
                                             for (int i = 0; i <= Convert.ToInt32(dr1["officerqty"]) - 1; i++)
                                             {
                                                 
                                                 if (dr1["ShiftType"].ToString() == "N")
                                                 {
                                                     DataRow InsertSitm2 = sitmt2.NewRow();
                                                     InsertSitm2["shiftcode"] = dr1["shiftcode"];

                                                     InsertSitm2["shiftcode"] = dr1["shiftcode"];
                                                     InsertSitm2["matnum"] = dr1["matnum"];

                                                     if ((bool)dr1["monday"])
                                                     {
                                                         InsertSitm2["monday"] = "X";
                                                     }
                                                     if ((bool)dr1["tuesday"])
                                                     {
                                                         InsertSitm2["tuesday"] = "X";
                                                     }
                                                     if ((bool)dr1["wednesday"])
                                                     {
                                                         InsertSitm2["wednesday"] = "X";
                                                     }
                                                     if ((bool)dr1["thursday"])
                                                     {
                                                         InsertSitm2["thursday"] = "X";
                                                     }
                                                     if ((bool)dr1["friday"])
                                                     {
                                                         InsertSitm2["friday"] = "X";
                                                     }
                                                     if ((bool)dr1["saturday"])
                                                     {
                                                         InsertSitm2["saturday"] = "X";
                                                     }
                                                     if ((bool)dr1["sunday"])
                                                     {
                                                         InsertSitm2["sunday"] = "X";
                                                     }

                                                     sitmt2.Rows.Add(InsertSitm2);
                                                 }

                                                
                                             }
                                         }

                                         #endregion

                                         #region ConciergeShift

                                         if (dr1["ShiftType"].ToString() == "C")
                                         {
                                             for (int i = 0; i <= Convert.ToInt32(dr1["officerqty"]) - 1; i++)
                                             {

                                                 if (dr1["ShiftType"].ToString() == "C")
                                                 {
                                                     DataRow InsertSitm3 = sitmt3.NewRow();
                                                     InsertSitm3["shiftcode"] = dr1["shiftcode"];

                                                     InsertSitm3["shiftcode"] = dr1["shiftcode"];
                                                     InsertSitm3["matnum"] = dr1["matnum"];

                                                     if ((bool)dr1["monday"])
                                                     {
                                                         InsertSitm3["monday"] = "X";
                                                     }
                                                     if ((bool)dr1["tuesday"])
                                                     {
                                                         InsertSitm3["tuesday"] = "X";
                                                     }
                                                     if ((bool)dr1["wednesday"])
                                                     {
                                                         InsertSitm3["wednesday"] = "X";
                                                     }
                                                     if ((bool)dr1["thursday"])
                                                     {
                                                         InsertSitm3["thursday"] = "X";
                                                     }
                                                     if ((bool)dr1["friday"])
                                                     {
                                                         InsertSitm3["friday"] = "X";
                                                     }
                                                     if ((bool)dr1["saturday"])
                                                     {
                                                         InsertSitm3["saturday"] = "X";
                                                     }
                                                     if ((bool)dr1["sunday"])
                                                     {
                                                         InsertSitm3["sunday"] = "X";
                                                     }

                                                     sitmt3.Rows.Add(InsertSitm3);
                                                 }

                                                 
                                             }
                                         }

                                         #endregion

                                         if (!BizFunctions.IsEmpty(sitmt["docunum"]))
                                         {
                                             GetSitmt1CtrhFooterTotals();
                                             GetSitmt2CtrhFooterTotals();
                                             GetSitmt3CtrhFooterTotals();
                                         }

                                     }
                                 }
                             }
                             this.dbaccess.DataSet.Tables["ctrTmp"].Dispose();
                             this.dbaccess.DataSet.Tables["ManPowerSchedule"].Dispose();
                           
                        }
                        else
                        {
                            MessageBox.Show("Contract No. Doesn't Exist!", "PicoGuards", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }


                        #endregion
                    }
                    break;

                case "sitmt_sectorcode":
                    e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    break;

          
            }
        }

        private int Countmatnum(string matnum,string shift,string Table)
        {
            int Total = 0;
            string SelectCount = "SELECT matnum,shiftcode,COUNT(*) as total from " + Table + " where matnum='" + matnum + "' and shiftcode='" + shift + "' group by matnum,shiftcode";
            DataTable matnumCount = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, SelectCount);
            if (matnumCount.Rows.Count > 0)
            {
                DataRow dr1 = matnumCount.Rows[0];
                if (BizFunctions.IsEmpty(dr1["total"]))
                {
                    dr1["total"] = 0;
                }
                else
                {
                    Total = Convert.ToInt32(dr1["total"]);
                }
            }
            return Total;
        }

        private string GetTable(char Code)
        {
            string Table = "";

            if (Code == 'D')
            {
                Table = "SITMT1";
            }
            if (Code == 'N')
            {
                Table = "SITMT2";
            }
            if (Code == 'C')
            {
                Table = "SITMT3";
            }

            return Table;
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow sitmt = dbaccess.DataSet.Tables["sitmt"].Rows[0];
            switch (e.MappingName)
            {
                case "empnum":
                    e.CurrentRow["empnum"] = e.F2CurrentRow["empnum"];
                    e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    //if (!AllowmatnumInsert(e.F2CurrentRow["matnum"].ToString(), e.TableName.ToString()))
                    //{

                    //    e.CurrentRow.Delete();
                        
                       
                    //}
                 
                    break;

                case "eqmnum":                                   
                    break;

                case "matnum":
                    {
                        //if(!AllowmatnumInsert(e.F2CurrentRow["matnum"].ToString(),e.TableName.ToString()))
                        //{
                          
                        //    e.CurrentRow.Delete();
                       
                        //}
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

        #region Get ShiftCode

        private string Getshiftcode(string tablename, string sitenum)
        {
            string result = "";
            string sql1 = "Select distinct shiftcode from " + tablename + " where sitenumt='" + sitenum + "'";
            this.dbaccess.ReadSQL("ResultShiftcode", sql1);
            DataTable ResultShiftcode = this.dbaccess.DataSet.Tables["ResultShiftcode"];

            if (ResultShiftcode.Rows.Count > 0)
            {
                DataRow drRS = this.dbaccess.DataSet.Tables["ResultShiftcode"].Rows[0];
                result = drRS["shiftcode"].ToString();
            }
            else
            {
                result = "";
            }
            return result;
        }

        #endregion

        #region Allow To Insert Schedule

        private bool AllowScheduleInsert(string matnum, string shiftcode, string day,string Tablename)
        {

            string GetInfo = "Select shiftcode,matnum,[day],ISNULL(sum(total),0) as Total from "+
                                "( "+
                                "SELECT shiftcode,matnum,'" + day + "' as [day], "+
	                                "CASE "+
                                        "WHEN ISNULL(" + day + ",'')='X' THEN 1 " +
                                            " WHEN " + day + "='O' OR " + day + "='' THEN 0 "+
                                        "ELSE 0 " +
		                                "END as  Total "+ 
	                                "from " + Tablename + " where shiftcode='" + shiftcode + "' and matnum='" + matnum + "' "+
                                ")A "+
                                "group by shiftcode,matnum,[day]";
            int total=0;
            bool allow, hasmatnum = false;
            DataTable tmpGetInfo = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetInfo);

            if (tmpGetInfo.Rows.Count > 0)
            {
                DataRow drTmpGetInfo = tmpGetInfo.Rows[0];

                foreach (DataRow dr1 in sc.ScheduleInfo.Rows)
                {
                    if (dr1["day"].ToString().Trim() == drTmpGetInfo["day"].ToString().Trim() && dr1["shiftcode"].ToString().Trim() == drTmpGetInfo["shiftcode"].ToString().Trim() && dr1["matnum"].ToString().Trim() == drTmpGetInfo["matnum"].ToString().Trim())
                    {
                        total = Convert.ToInt32(dr1["total"]) - Convert.ToInt32(drTmpGetInfo["total"]);
                        hasmatnum = true;

                        break;
                    }
                    else
                    {
                        hasmatnum = false;
                    }
                }

            }


            if (hasmatnum)
            {
                if (total < 0)
                {
                    allow = false;
                }
                else
                {
                    allow = true;
                }
            }
            else
            {
                allow = false;
            }

           

            return allow;

        }

        #endregion

        #region Allow matnum Insert

        private bool AllowmatnumInsert(string Tablename)
        {
            string GetmatnumInfo = "Select matnum,COUNT(*) as total from " + Tablename + " " +
                                        "group by matnum";

            int total = 0;
            bool allow,flag = false;
            DataTable tmpGetmatnumInfo = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetmatnumInfo);

            if (tmpGetmatnumInfo.Rows.Count > 0)
            {
                DataRow drTmptmpGetDgInfo = tmpGetmatnumInfo.Rows[0];

                foreach (DataRow dr1 in sc.matnumCountInfo.Rows)
                {
                    if (dr1["matnum"].ToString().Trim() == drTmptmpGetDgInfo["matnum"].ToString().Trim())
                    {
                        total = Convert.ToInt32(dr1["total"]) - Convert.ToInt32(drTmptmpGetDgInfo["total"]);
                        flag = true;
                        break;
                    }
                }

            }

            if (total < 0 || flag==false)
            {
                allow = false;
            }
            else
            {
                allow = true;
            }
            return allow;
        }

        #endregion

        #region Column Changed Events

        private void Voucher_SITM1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable sitmt1 = this.dbaccess.DataSet.Tables["SITMT1"];

             switch (e.Column.ColumnName)
             {
                 case "empnum":
                     {
                         if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                         {
                             e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                             e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                         }
                                                                    
                     }
                     break;

                 case "monday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}                  
                         
                     }
                     break;

                 case "tuesday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}

                     }
                     break;

                 case "wednesday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "thursday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "friday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "saturday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "sunday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;
                 case "shiftcode":
                     {
                         
                     }
                     break;
                 case "matnum":
                     {
                         //if (AllowmatnumInsert(e.Column.Table.ToString()))
                         //{
                         //    e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //}
                         //else
                         //{
                         //    if (e.Row[e.Column.ToString()].ToString() != string.Empty)
                         //    {
                         //        e.Row.BeginEdit();
                         //        e.Row.Delete();
                         //        e.Row.EndEdit();
                         //    }
                         //}
                     }
                     break;
                     e.Row.EndEdit();



             }
        }

        private void Voucher_SITM2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {


            e.Row.BeginEdit();
            
            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {
                        
                         // if(AllowScheduleInsert(e.Row["matnum"].ToString(),e.Row["shiftcode"].ToString(),e.Column.ToString(),e.Column.Table.ToString()))
                         //{
                         //    e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                         //}
                         //else
                         //{
                         //    e.Row.BeginEdit();
                         //    e.Row[e.Column.ToString()] = string.Empty;
                         //    e.Row.EndEdit();
                         //    ShowMsgAllowScheduleInsert();
                         //}

                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                        }
                                              

                    }
                    break;

                case "monday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}

                    }
                    break;

                case "tuesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "wednesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "thursday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "friday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "saturday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "sunday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;
                case "shiftcode":
                    {
                        
                    }
                    break;
                    e.Row.EndEdit();

                    
            }
        }

        private void Voucher_SITM3_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
        

            e.Row.BeginEdit();

            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {

                        //if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //{
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //}
                        //else
                        //{
                        //    e.Row.BeginEdit();
                        //    e.Row[e.Column.ToString()] = string.Empty;
                        //    e.Row.EndEdit();
                        //    ShowMsgAllowScheduleInsert();
                        //}

                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                        }
                                              

                    }
                    break;

                case "monday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                        
                    }
                    break;

                case "tuesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "wednesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "thursday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "friday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "saturday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "sunday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;
                case "shiftcode":
                    {
                        
                    }
                    break;
                    e.Row.EndEdit();
         


            }
        }

        private void Voucher_SITM4_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
    

            e.Row.BeginEdit();

            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                        }
                                   
                    }
                    break;

                case "monday":
                    {

                        
                    }
                    break;

                case "tuesday":
                    {
                        
                    }
                    break;

                case "wednesday":
                    {
                        

                    }
                    break;

                case "thursday":
                    {
                        
                    }
                    break;

                case "friday":
                    {
                        
                    }
                    break;

                case "saturday":
                    {
                        
                    }
                    break;

                case "sunday":
                    {
                        
                    }
                    break;
                case "shiftcode":
                    {
                        
                    }
                    break;
                    e.Row.EndEdit();



            }


        }

        #endregion

        #region Get Final Summary Count of Schedule
        
        private void Summary()
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt1 = this.dbaccess.DataSet.Tables["sitmt1"];
            DataTable sitmt1sum = this.dbaccess.DataSet.Tables["sitmt1sum"];

            DataTable sitmt2 = this.dbaccess.DataSet.Tables["sitmt2"];
            DataTable sitmt2sum = this.dbaccess.DataSet.Tables["sitmt2sum"];

            DataTable sitmt3 = this.dbaccess.DataSet.Tables["sitmt3"];
            DataTable sitmt3sum = this.dbaccess.DataSet.Tables["sitmt3sum"];

            DataTable sitmt4 = this.dbaccess.DataSet.Tables["sitmt4"];
            DataTable sitmt4sum = this.dbaccess.DataSet.Tables["sitmt4sum"];

            DataTable sitmtall = this.dbaccess.DataSet.Tables["sitmtall"];

          

           #region

           foreach (DataRow dr1 in sitmt1.Rows)
           {
               if (dr1.RowState != DataRowState.Deleted)
               {
                   if (BizFunctions.IsEmpty(dr1["monday"].ToString().Trim()))
                   {
                       dr1["monday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["tuesday"].ToString().Trim()))
                   {
                       dr1["tuesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["wednesday"].ToString().Trim()))
                   {
                       dr1["wednesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["thursday"].ToString().Trim()))
                   {
                       dr1["thursday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["friday"].ToString().Trim()))
                   {
                       dr1["friday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["saturday"].ToString().Trim()))
                   {
                       dr1["saturday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["sunday"].ToString().Trim()))
                   {
                       dr1["sunday"] = "O";
                   }
               }

           }

           #endregion

           #region HCNR
           foreach (DataRow dr2 in sitmt2.Rows)
           {
               if (dr2.RowState != DataRowState.Deleted)
               {
                   if (BizFunctions.IsEmpty(dr2["monday"].ToString().Trim()))
                   {
                       dr2["monday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["tuesday"].ToString().Trim()))
                   {
                       dr2["tuesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["wednesday"].ToString().Trim()))
                   {
                       dr2["wednesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["thursday"].ToString().Trim()))
                   {
                       dr2["thursday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["friday"].ToString().Trim()))
                   {
                       dr2["friday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["saturday"].ToString().Trim()))
                   {
                       dr2["saturday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["sunday"].ToString().Trim()))
                   {
                       dr2["sunday"] = "O";
                   }

               }

           }

           #endregion

           #region PFMEDU
           foreach (DataRow dr3 in sitmt3.Rows)
           {
               if (dr3.RowState != DataRowState.Deleted)
               {
                   if (BizFunctions.IsEmpty(dr3["monday"].ToString().Trim()))
                   {
                       dr3["monday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["tuesday"].ToString().Trim()))
                   {
                       dr3["tuesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["wednesday"].ToString().Trim()))
                   {
                       dr3["wednesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["thursday"].ToString().Trim()))
                   {
                       dr3["thursday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["friday"].ToString().Trim()))
                   {
                       dr3["friday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["saturday"].ToString().Trim()))
                   {
                       dr3["saturday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["sunday"].ToString().Trim()))
                   {
                       dr3["sunday"] = "O";
                   }
               }

           }

           #endregion


           
           if (sitmt1sum.Rows.Count > 0)
           {
               BizFunctions.DeleteAllRows(sitmt1sum);
           }
           DataTable sitmt1sumTmp = BizLogicTools.Tools.GetShiftSummary("SITMT1", this.dbaccess.DataSet);
            foreach (DataRow dr4 in sitmt1sumTmp.Select())
            {       
                if (dr4.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm1Sum = sitmt1sum.NewRow();
                    drSitm1Sum["matnum"] = dr4["matnum"];
                    drSitm1Sum["shiftcode"] = dr4["shiftcode"];
                    drSitm1Sum["monday"] = dr4["monday"];
                    drSitm1Sum["tuesday"] = dr4["tuesday"];
                    drSitm1Sum["wednesday"] = dr4["wednesday"];
                    drSitm1Sum["thursday"] = dr4["thursday"];
                    drSitm1Sum["friday"] = dr4["friday"];
                    drSitm1Sum["saturday"] = dr4["saturday"];
                    drSitm1Sum["sunday"] = dr4["sunday"]; 
                    sitmt1sum.Rows.Add(drSitm1Sum);
                }
            }

            if (sitmt2sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmt2sum);
            }
            DataTable sitmt2sumTmp = BizLogicTools.Tools.GetShiftSummary("SITMT2", this.dbaccess.DataSet);
            foreach (DataRow dr5 in sitmt2sumTmp.Select())
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm2Sum = sitmt2sum.NewRow();
                    drSitm2Sum["matnum"] = dr5["matnum"];
                    drSitm2Sum["shiftcode"] = dr5["shiftcode"];
                    drSitm2Sum["monday"] = dr5["monday"];
                    drSitm2Sum["tuesday"] = dr5["tuesday"];
                    drSitm2Sum["wednesday"] = dr5["wednesday"];
                    drSitm2Sum["thursday"] = dr5["thursday"];
                    drSitm2Sum["friday"] = dr5["friday"];
                    drSitm2Sum["saturday"] = dr5["saturday"];
                    drSitm2Sum["sunday"] = dr5["sunday"];
                    sitmt2sum.Rows.Add(drSitm2Sum);
                }
            }

            if (sitmt3sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmt3sum);
            }
            DataTable sitmt3sumTmp = BizLogicTools.Tools.GetShiftSummary("SITMT3", this.dbaccess.DataSet);
            foreach (DataRow dr6 in sitmt3sumTmp.Select())
            {
                if (dr6.RowState != DataRowState.Deleted)
                {

                    DataRow drSitm3Sum = sitmt3sum.NewRow();
                    drSitm3Sum["matnum"] = dr6["matnum"];
                    drSitm3Sum["shiftcode"] = dr6["shiftcode"];
                    drSitm3Sum["monday"] = dr6["monday"];
                    drSitm3Sum["tuesday"] = dr6["tuesday"];
                    drSitm3Sum["wednesday"] = dr6["wednesday"];
                    drSitm3Sum["thursday"] = dr6["thursday"];
                    drSitm3Sum["friday"] = dr6["friday"];
                    drSitm3Sum["saturday"] = dr6["saturday"];
                    drSitm3Sum["sunday"] = dr6["sunday"];
                    sitmt3sum.Rows.Add(drSitm3Sum);
                }
            }

            string relief = "select matnum,COUNT(*) as total from [sitmt4] group by matnum";
            DataTable sitmt4sumTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, relief);
            if (sitmt4sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmt4sum);
            }

            foreach (DataRow dr7 in sitmt4sumTmp.Select())
            {

                if (dr7.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm4Sum = sitmt4sum.NewRow();
                    drSitm4Sum["matnum"] = dr7["matnum"];
                    drSitm4Sum["total"] = dr7["total"];           
                    sitmt4sum.Rows.Add(drSitm4Sum);
                }

            }


            string overallSum = "Select R1.shiftcode, " +
                               "sum(R1.monday) as monday, " +
                               "sum(R1.tuesday) as tuesday, " +
                               "sum(R1.wednesday) as wednesday, " +
                               "sum(R1.thursday) as thursday, " +
                               "sum(R1.friday) as friday, " +
                               "sum(R1.saturday) as saturday, " +
                               "sum(R1.sunday) as sunday " +
                               "From " +
                               "(" +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from sitmt1sum " +

                                "union all " +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from sitmt2sum " +

                                "union all " +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from sitmt3sum " +

                                ") R1 " +
                                "Group by R1.shiftcode";

            DataTable sitmtallsumTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, overallSum);

            if (sitmtall.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmtall);
            }

            foreach (DataRow dr8 in sitmtallsumTmp.Select())
            {
                if (dr8.RowState != DataRowState.Deleted)
                {

                    DataRow drSitmall = sitmtall.NewRow();
                    drSitmall["shiftcode"] = dr8["shiftcode"];
                    drSitmall["monday"] = dr8["monday"];
                    drSitmall["tuesday"] = dr8["tuesday"];
                    drSitmall["wednesday"] = dr8["wednesday"];
                    drSitmall["thursday"] = dr8["thursday"];
                    drSitmall["friday"] = dr8["friday"];
                    drSitmall["saturday"] = dr8["saturday"];
                    drSitmall["sunday"] = dr8["sunday"];
                    sitmtall.Rows.Add(drSitmall);
                }
            }


            sitmt1sumTmp.Dispose();
            sitmt2sumTmp.Dispose();
            sitmt3sumTmp.Dispose();
            sitmt4sumTmp.Dispose();
            sitmtallsumTmp.Dispose();
            columnChanged = false;

        }

        #endregion

        #region Get Table Max ID

        private int GetTableMaxID(string Tablename)
        {
            string sql1 = "Select ISNULL(MAX(id),0) as maxid from " + Tablename + "";

            this.dbaccess.ReadSQL("Result1", sql1);
            DataRow dr = this.dbaccess.DataSet.Tables["Result1"].Rows[0];

            return  Convert.ToInt32(dr["maxid"]);

        }

        #endregion

        #region Get Min ID

        private int GetTableMinID(string Tablename, string Sitenum)
        {
            string sql1 = "Select ISNULL(Min(id),0) as minid from " + Tablename + " where refnum='"+Sitenum+"'";

            this.dbaccess.ReadSQL("Result2", sql1);
            DataRow dr = this.dbaccess.DataSet.Tables["Result2"].Rows[0];

            return Convert.ToInt32(dr["minid"]);

        }

        #endregion

        #region isAllowedDay

        private bool isAllowedDay(string Day)
        {
            bool allowed = false;
            foreach (DataRow dr1 in AllowedDAys.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {

                    if (dr1["Day"].ToString() == Day || dr1["Day"].ToString() == BizLogicTools.Tools.ToTitleCase(Day))
                    {
                        allowed = true;
                        break;
                    }
                }
            }
            return allowed;
        }

        #endregion

        #region Show Message

        private void ShowMsgAllowScheduleInsert()
        {
            MessageBox.Show("You can't enter Schedule in this Row, Please Check your Contract", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        #region Calculate Day of the Week
        private int EmpTotalofDay(string day, string tablename)
        {
            int total = 0;
            int value = 0;

            DataTable Table = dbaccess.DataSet.Tables[tablename];
            foreach (DataRow dr1 in Table.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr1[day]))
                    {
                        if (dr1[day].ToString().Trim() == "X")
                        {
                            value = 1;
                        }
                        else
                        {
                            value = 0;
                        }

                        //if (dr1[day].ToString().Trim() != "O")
                        //{
                        //    value = 1;
                        //}
                        //else
                        //{
                        //    value = 0;
                        //}
                    }
                    else
                    {
                        value = 0;
                    }
                    total = total + value;
                }
            }
            return total;
        }      
        #endregion

        #region Get Footer Totals

        private void GetSitmt1FooterTotals()
        {
            sitmt1Total_monday.Text = Convert.ToString(EmpTotalofDay("monday","sitmt1"));
            sitmt1Total_tuesday.Text = Convert.ToString(EmpTotalofDay("tuesday","sitmt1"));
            sitmt1Total_wednesday.Text = Convert.ToString(EmpTotalofDay("wednesday","sitmt1"));
            sitmt1Total_thursday.Text = Convert.ToString(EmpTotalofDay("thursday","sitmt1"));
            sitmt1Total_friday.Text = Convert.ToString(EmpTotalofDay("friday","sitmt1"));
            sitmt1Total_saturday.Text = Convert.ToString(EmpTotalofDay("saturday","sitmt1"));
            sitmt1Total_sunday.Text = Convert.ToString(EmpTotalofDay("sunday","sitmt1")); 
        }

        private void GetSitmt2FooterTotals()
        {
            sitmt2Total_monday.Text = Convert.ToString(EmpTotalofDay("monday", "sitmt2"));
            sitmt2Total_tuesday.Text = Convert.ToString(EmpTotalofDay("tuesday", "sitmt2"));
            sitmt2Total_wednesday.Text = Convert.ToString(EmpTotalofDay("wednesday", "sitmt2"));
            sitmt2Total_thursday.Text = Convert.ToString(EmpTotalofDay("thursday", "sitmt2"));
            sitmt2Total_friday.Text = Convert.ToString(EmpTotalofDay("friday", "sitmt2"));
            sitmt2Total_saturday.Text = Convert.ToString(EmpTotalofDay("saturday", "sitmt2"));
            sitmt2Total_sunday.Text = Convert.ToString(EmpTotalofDay("sunday", "sitmt2"));
        }

        private void GetSitmt3FooterTotals()
        {
            sitmt3Total_monday.Text = Convert.ToString(EmpTotalofDay("monday", "sitmt3"));
            sitmt3Total_tuesday.Text = Convert.ToString(EmpTotalofDay("tuesday", "sitmt3"));
            sitmt3Total_wednesday.Text = Convert.ToString(EmpTotalofDay("wednesday", "sitmt3"));
            sitmt3Total_thursday.Text = Convert.ToString(EmpTotalofDay("thursday", "sitmt3"));
            sitmt3Total_friday.Text = Convert.ToString(EmpTotalofDay("friday", "sitmt3"));
            sitmt3Total_saturday.Text = Convert.ToString(EmpTotalofDay("saturday", "sitmt3"));
            sitmt3Total_sunday.Text = Convert.ToString(EmpTotalofDay("sunday", "sitmt3"));
        }

        #endregion

        #region Get Contract Footer Totals

        private void GetSitmt1CtrhFooterTotals()
        {
            this.dbaccess.ReadSQL("CTRday",GetQuery("D"));
            DataTable CTRday = this.dbaccess.DataSet.Tables["CTRday"];
            if (CTRday.Rows.Count > 0)
            {
                DataRow drDay = this.dbaccess.DataSet.Tables["CTRday"].Rows[0];


                if (BizFunctions.IsEmpty(drDay["monday"]))
                {
                    drDay["monday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["tuesday"]))
                {
                    drDay["tuesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["wednesday"]))
                {
                    drDay["wednesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["thursday"]))
                {
                    drDay["thursday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["friday"]))
                {
                    drDay["friday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["saturday"]))
                {
                    drDay["saturday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["sunday"]))
                {
                    drDay["sunday"] = 0;
                }

                sitmt1Ctrh_monday.Text = drDay["monday"].ToString();
                sitmt1Ctrh_tuesday.Text = drDay["tuesday"].ToString();
                sitmt1Ctrh_wednesday.Text = drDay["wednesday"].ToString();
                sitmt1Ctrh_thursday.Text = drDay["thursday"].ToString();
                sitmt1Ctrh_friday.Text = drDay["friday"].ToString();
                sitmt1Ctrh_saturday.Text = drDay["saturday"].ToString();
                sitmt1Ctrh_sunday.Text = drDay["sunday"].ToString();
            }
           
            CTRday.Dispose();

        }

        private void GetSitmt2CtrhFooterTotals()
        {
            this.dbaccess.ReadSQL("CTRnight", GetQuery("N"));
            DataTable CTRnight = this.dbaccess.DataSet.Tables["CTRnight"];
            if (CTRnight.Rows.Count > 0)
            {
                DataRow drNight = this.dbaccess.DataSet.Tables["CTRnight"].Rows[0];


                if (BizFunctions.IsEmpty(drNight["monday"]))
                {
                    drNight["monday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["tuesday"]))
                {
                    drNight["tuesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["wednesday"]))
                {
                    drNight["wednesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["thursday"]))
                {
                    drNight["thursday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["friday"]))
                {
                    drNight["friday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["saturday"]))
                {
                    drNight["saturday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["sunday"]))
                {
                    drNight["sunday"] = 0;
                }

                sitmt2Ctrh_monday.Text = drNight["monday"].ToString();
                sitmt2Ctrh_tuesday.Text = drNight["tuesday"].ToString();
                sitmt2Ctrh_wednesday.Text = drNight["wednesday"].ToString();
                sitmt2Ctrh_thursday.Text = drNight["thursday"].ToString();
                sitmt2Ctrh_friday.Text = drNight["friday"].ToString();
                sitmt2Ctrh_saturday.Text = drNight["saturday"].ToString();
                sitmt2Ctrh_sunday.Text = drNight["sunday"].ToString();
            }
           
            CTRnight.Dispose();

        }

        private void GetSitmt3CtrhFooterTotals()
        {

            this.dbaccess.ReadSQL("CTRconcierge", GetQuery("C"));
            DataTable CTRconcierge = this.dbaccess.DataSet.Tables["CTRconcierge"];
            if (CTRconcierge.Rows.Count > 0)
            {
                DataRow drconcierge = this.dbaccess.DataSet.Tables["CTRconcierge"].Rows[0];

                if (BizFunctions.IsEmpty(drconcierge["monday"]))
                {
                    drconcierge["monday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["tuesday"]))
                {
                    drconcierge["tuesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["wednesday"]))
                {
                    drconcierge["wednesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["thursday"]))
                {
                    drconcierge["thursday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["friday"]))
                {
                    drconcierge["friday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["saturday"]))
                {
                    drconcierge["saturday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["sunday"]))
                {
                    drconcierge["sunday"] = 0;
                }

                sitmt3Ctrh_monday.Text = drconcierge["monday"].ToString();
                sitmt3Ctrh_tuesday.Text = drconcierge["tuesday"].ToString();
                sitmt3Ctrh_wednesday.Text = drconcierge["wednesday"].ToString();
                sitmt3Ctrh_thursday.Text = drconcierge["thursday"].ToString();
                sitmt3Ctrh_friday.Text = drconcierge["friday"].ToString();
                sitmt3Ctrh_saturday.Text = drconcierge["saturday"].ToString();
                sitmt3Ctrh_sunday.Text = drconcierge["sunday"].ToString();
            }
           
            CTRconcierge.Dispose();
            
        }

        private string GetQuery(string ShiftType)
        {
            DataRow sitmt = dbaccess.DataSet.Tables["SITMT"].Rows[0];
            string Query = "";
            if (sitmt["contracttype"].ToString() == "CTRH")
            {
                Query = "Select "+
	                            "SUM(monday) as monday, "+
	                            "SUM(tuesday) as tuesday, "+
	                            "SUM(wednesday) as wednesday, "+
	                            "SUM(thurday) as thursday, "+
	                            "SUM(friday) as friday, "+
	                            "SUM(saturday) as saturday, "+
                                "SUM(sunday) as sunday " +
                            "from "+
                            "( "+
	                            "select "+
		                            "officerqty, "+
		                            "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, "+
		                            "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, "+
		                            "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, "+
		                            "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, "+
		                            "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday "+
		                            "from CTR1 "+
                                "where refnum='" + sitmt["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +	
                            	
	                            "union all "+
                            	
	                            "select "+
		                            "officerqty, "+
		                            "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, "+
		                            "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, "+
		                            "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, "+
		                            "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, "+
		                            "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday "+
                                    "from ADH1 " +
                                "where ctrnum='" + sitmt["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +
                            ")a";
            }
            if (sitmt["contracttype"].ToString() == "ADH")
            {
                Query = "Select " +
                                "SUM(monday) as monday, " +
                                "SUM(tuesday) as tuesday, " +
                                "SUM(wednesday) as wednesday, " +
                                "SUM(thurday) as thursday, " +
                                "SUM(friday) as friday, " +
                                "SUM(saturday) as saturday, " +
                                "SUM(sunday) as sunday " +
                            "from " +
                            "( " +                               
                                "select " +
                                    "officerqty, " +
                                    "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, " +
                                    "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, " +
                                    "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, " +
                                    "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, " +
                                    "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, " +
                                    "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, " +
                                    "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday " +
                                    "from ADH1 " +
                                "where refnum='" + sitmt["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +
                            ")a";
            }

            return Query;

        }

        #endregion

        #region Check if Each shift is Balanced

        private bool isDayBalanced()
        {
            bool correct = true;
            if (sitmt1Total_monday.Text != sitmt1Ctrh_monday.Text || sitmt1Total_tuesday.Text != sitmt1Ctrh_tuesday.Text
                || sitmt1Total_wednesday.Text != sitmt1Ctrh_wednesday.Text || sitmt1Total_thursday.Text != sitmt1Ctrh_thursday.Text
                || sitmt1Total_friday.Text != sitmt1Ctrh_friday.Text || sitmt1Total_saturday.Text != sitmt1Ctrh_saturday.Text
                || sitmt1Total_sunday.Text != sitmt1Ctrh_sunday.Text)
            {
                correct = false;
            }          
          
            return correct;
        }

        private bool isNightBalanced()
        {
            bool correct = true;
            if (sitmt2Total_monday.Text != sitmt2Ctrh_monday.Text || sitmt2Total_tuesday.Text != sitmt2Ctrh_tuesday.Text
                || sitmt2Total_wednesday.Text != sitmt2Ctrh_wednesday.Text || sitmt2Total_thursday.Text != sitmt2Ctrh_thursday.Text
                || sitmt2Total_friday.Text != sitmt2Ctrh_friday.Text || sitmt2Total_saturday.Text != sitmt2Ctrh_saturday.Text
                || sitmt2Total_sunday.Text != sitmt2Ctrh_sunday.Text)
            {
                correct = false;
            }

            return correct;
        }

        private bool isConciergeBalanced()
        {
            bool correct = true;
            if (sitmt3Total_monday.Text != sitmt3Ctrh_monday.Text || sitmt3Total_tuesday.Text != sitmt3Ctrh_tuesday.Text
                || sitmt3Total_wednesday.Text != sitmt3Ctrh_wednesday.Text || sitmt3Total_thursday.Text != sitmt3Ctrh_thursday.Text
                || sitmt3Total_friday.Text != sitmt3Ctrh_friday.Text || sitmt3Total_saturday.Text != sitmt3Ctrh_saturday.Text
                || sitmt3Total_sunday.Text != sitmt3Ctrh_sunday.Text)
            {
                correct = false;
            }

            return correct;
        }

        #endregion

        #region Check if an Employee is Assigned more than once in the same shift
        private bool isDuplicateInShift(string table1, string table2, string table3, string column)
        {
            bool rowsEqual = true; 
            DataTable dataTable1 = this.dbaccess.DataSet.Tables[table1];
           
            for (Int32 r0 = 0; r0 < dataTable1.Rows.Count; r0++)
            {
                

                if (dataTable1.Rows[r0].RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dataTable1.Rows[r0][column]))
                    {
                        for (Int32 r1 = r0 + 1; r1 < dataTable1.Rows.Count; r1++)
                        {
                            
                            if (dataTable1.Rows[r1].RowState != DataRowState.Deleted)
                            {
                                if (!BizFunctions.IsEmpty(dataTable1.Rows[r1][column]))
                                {
                                    string tests2 = dataTable1.Rows[r1][column].ToString().Trim();
                                    if (dataTable1.Rows[r0][column].ToString().Trim() == dataTable1.Rows[r1][column].ToString().Trim())
                                    {
                                        MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        rowsEqual = false;
                                        break;

                                    }                                  
                                  
                                    if (rowsEqual == false)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                    }

                    if (rowsEqual && !isExistinOtherTable(dataTable1.Rows[r0][column].ToString().Trim(), table2, column))
                    {
                        MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rowsEqual = false;
                        break;

                    }

                    if (rowsEqual && !isExistinOtherTable(dataTable1.Rows[r0][column].ToString().Trim(), table3, column))
                    {
                        MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rowsEqual = false;
                        break;

                    }
                    if (rowsEqual && !isExistinOtherTemplates(dataTable1.Rows[r0][column].ToString().Trim()))
                    {
                        rowsEqual = false;
                    }

                }

               

            } 

            return rowsEqual;
        }
        #endregion

        #region Check if Employee is Duplicated across Shifts

        private bool isExistinOtherTable(string empnum,string datatable,string column)
        {
            DataTable dataTable = this.dbaccess.DataSet.Tables[datatable];

            bool rowsEqual = true;

            foreach (DataRow dr1 in dataTable.Rows)
            {
                  if (dr1.RowState != DataRowState.Deleted)
                  {
                      if (!BizFunctions.IsEmpty(dr1[column]))
                      {
                          if (dr1[column].ToString().Trim() == empnum)
                          {
                              rowsEqual = false;
                              break;
                          }
                      }
                  }
            }
          
            return rowsEqual;
        }

        #endregion

        #region Check if Employee is Duplicated across Shifts

        private bool isExistinOtherTemplates(string empnum)
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];
            bool rowsEqual = true;

            string DayShift = "Select Empnum,sitenumt from sitmt1 where empnum='" + empnum + "' and sitenumt<>'" + sitmt["sitenumt"] + "' and [status]<>'V'";
            string NightShift = "Select Empnum,sitenumt from sitmt2 where empnum='" + empnum + "' and sitenumt<>'" + sitmt["sitenumt"] + "' and [status]<>'V'";
            string Concierge = "Select Empnum,sitenumt from sitmt3 where empnum='" + empnum + "' and sitenumt<>'" + sitmt["sitenumt"] + "' and [status]<>'V'";

            this.dbaccess.ReadSQL("OtherDayShift", DayShift);
            this.dbaccess.ReadSQL("OtherNightShift", NightShift);
            this.dbaccess.ReadSQL("OtherConciergeShift", Concierge);

            if (this.dbaccess.DataSet.Tables["OtherDayShift"].Rows.Count > 0)
            {
                MessageBox.Show("Emp No " + empnum + " already exists in " + this.dbaccess.DataSet.Tables["OtherDayShift"].Rows[0]["sitenumt"].ToString() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rowsEqual = false;
            }

            if (this.dbaccess.DataSet.Tables["OtherNightShift"].Rows.Count > 0)
            {
                MessageBox.Show("Emp No " + empnum + " already exists in " + this.dbaccess.DataSet.Tables["OtherNightShift"].Rows[0]["sitenumt"].ToString() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rowsEqual = false;
            }

            if (this.dbaccess.DataSet.Tables["OtherConciergeShift"].Rows.Count > 0)
            {
                MessageBox.Show("Emp No " + empnum + " already exists in " + this.dbaccess.DataSet.Tables["OtherConciergeShift"].Rows[0]["sitenumt"].ToString() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rowsEqual = false;
            }
            this.dbaccess.DataSet.Tables["OtherDayShift"].Dispose();
            this.dbaccess.DataSet.Tables["OtherNightShift"].Dispose();
            this.dbaccess.DataSet.Tables["OtherConciergeShift"].Dispose();

            return rowsEqual;
        }

        #endregion

        private string GetEmpname(string empnum)
        {
            string empName = "";

            string Get = "Select Empname From hemph where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                empName = dt1.Rows[0]["empname"].ToString();
            }

            dt1.Dispose();

            return empName;

        }

        private string GetMatnum(string empnum)
        {

            string matnum = "";

            string Get = "Select matnum From hemph where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                matnum = dt1.Rows[0]["matnum"].ToString();
            }

            dt1.Dispose();

            return matnum;
        }




    }

}


//protected void SetAllowedDays()
//{
//    DataRow sitmt = this.dbaccess.DataSet.Tables["SITMT"].Rows[0];
//    if (!BizFunctions.IsEmpty(wrr["commencedate"]) && !illegalWeek())
//    {
//        int Count = TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_commencedate.Text)), BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_enddate.Text)));

//        ArrayList DateLists = new ArrayList();


//        DateTime beginDate = new DateTime();
//        DateTime endDate = new DateTime();

//        beginDate = Convert.ToDateTime(wrr_commencedate.Text.ToString());
//        endDate = Convert.ToDateTime(wrr_enddate.Text.ToString());


//        while (beginDate <= endDate)
//        {
//            DateLists.Add(beginDate);
//            beginDate = beginDate.AddDays(1);
//        }


//        if (this.dbaccess.DataSet.Tables.Contains("AlloweSchedule"))
//        {
//            this.dbaccess.DataSet.Tables["AlloweSchedule"].Dispose();
//        }
//        DataTable dtTable = new DataTable();
//        dtTable.TableName = "AlloweSchedule";
//        dtTable.Columns.Add("Date", typeof(DateTime));
//        dtTable.Columns.Add("Day", typeof(string));
//        dtTable.Columns.Add("sequence", typeof(int));

//        for (int i = 0; i <= DateLists.Count - 1; i++)
//        {
//            DataRow InsertdtTable = dtTable.NewRow();
//            InsertdtTable["Date"] = DateLists[i].ToString();
//            InsertdtTable["Day"] = TimeTools.GetDay(TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString()))));
//            InsertdtTable["sequence"] = TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString())));
//            dtTable.Rows.Add(InsertdtTable);
//        }


//        AllowedDAys = dtTable.Copy();
//        if (this.dbaccess.DataSet.Tables.Contains("AlloweSchedule"))
//        {
//            this.dbaccess.DataSet.Tables["AlloweSchedule"].Dispose();
//        }
//        else
//        {
//            AllowedDAys.TableName = "AllowedDAys";
//            this.dbaccess.DataSet.Tables.Add(AllowedDAys);
//        }

//    }
//}