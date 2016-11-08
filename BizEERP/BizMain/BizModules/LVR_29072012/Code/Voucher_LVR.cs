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

using System.Text.RegularExpressions;
using PicoGuards.BizModules.UserAuthorization;
using PicoGuards.TimeUtilites;

#endregion

namespace PicoGuards.LVR
{
    public class Voucher_LVR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        UserAuthorization sa = null;
        protected string headerFormName,LeaveHistoryFormName,LeaveSummaryFormName,RecommendedBy,ApprovedBy = null;
        protected TextBox lvr_recmbyreason, lvr_recmby, lvr_apprdbyreason, lvr_apprdby, lvr_empnum, lvr_totaldays = null;
        protected GroupBox grb_lvrhapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        private string dateJoined = "";
        protected DateTimePicker lvr_leavefrom, lvr_leaveto = null;
        ComboBox lvr_lvmnum = null;
        DataGrid overallleavesummary,LeaveSummary = null;

        LeaveControl lv = null;

        #endregion

        #region Construct

        public Voucher_LVR(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_LVR.xml", moduleName, voucherBaseHelpers)
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
            this.LeaveHistoryFormName = (e.FormsCollection["leaverec"] as Form).Name;
            this.LeaveSummaryFormName = (e.FormsCollection["leavesum"] as Form).Name;

            sa = new UserAuthorization(this.moduleName.ToString());

            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];

            Initialise();
            GetHiredDate();

        
            
        }

        #endregion

        #region DocumentPage Event

        //protected override void AddDocumentPageEventTarget(object sender, PageEventArgs e)
        //{
        //    base.AddDocumentPageEventTarget(sender, e);
        //    switch (e.PageName)
        //    {
        //        case "header":
        //            e.EventTarget = new Header_LVR(e.DBAccess, e.FormsCollection, e.DocumentKey);
        //            break;
        //    }
        //}

        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            lvr_empnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_empnum") as TextBox;

            lvr_empnum.Leave +=new EventHandler(lvr_empnum_Leave);

            lvr_totaldays = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_totaldays") as TextBox;

            lvr_totaldays.Leave +=new EventHandler(lvr_totaldays_Leave);

            lvr_lvmnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_lvmnum") as ComboBox;
            lvr_lvmnum.DropDown += new EventHandler(lvr_lvmnum_DropDown);
            grb_lvrhapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_lvrapprinfo") as GroupBox;
            lvr_recmby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_recmby") as TextBox;
            lvr_apprdby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_apprdby") as TextBox;
            lvr_recmbyreason = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_recmbyreason") as TextBox;
            lvr_apprdbyreason = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_apprdbyreason") as TextBox;

           

            rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            lvr_leavefrom = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_leavefrom") as DateTimePicker;

            //lvr_leavefrom.Leave +=new EventHandler(lvr_leavefrom_Leave);

            lvr_leaveto = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_leaveto") as DateTimePicker;

            lvr_leaveto.Leave +=new EventHandler(lvr_leaveto_Leave);


            overallleavesummary = BizXmlReader.CurrentInstance.GetControl(LeaveHistoryFormName, "dg_leaverec") as DataGrid;

            LeaveSummary = BizXmlReader.CurrentInstance.GetControl(LeaveSummaryFormName, "dg_leavesummary") as DataGrid;
           

            rad_Recd.CheckedChanged += new EventHandler(rad_Recd_CheckedChanged);
            rad_NotRecd.CheckedChanged += new EventHandler(rad_NotRecd_CheckedChanged);
            rad_Appr.CheckedChanged += new EventHandler(rad_Appr_CheckedChanged);
            rad_NotAppr.CheckedChanged += new EventHandler(rad_NotAppr_CheckedChanged);
            LoadRadioButtonsData();

            lvr_recmbyreason.TextChanged += new EventHandler(trqh_recommendedcomments_TextChanged);
            lvr_apprdbyreason.TextChanged += new EventHandler(trqh_approvedcomments_TextChanged);

            EmptyRecommend();

            if (!sa.ApprovePermission)
            {
                grb_lvrhapprinfo.Enabled = false;
            }
            else
            {
                grb_lvrhapprinfo.Enabled = true;
            }



            if (!BizFunctions.IsEmpty(lvr["empnum"]))
            {
                string statuso = Common.DEFAULT_DOCUMENT_STATUSO;
                string statusp = Common.DEFAULT_DOCUMENT_STATUSP;
                string lvrstatus = lvr["status"].ToString();

                if (lvrstatus == statuso || lvrstatus == statusp)
                {
                    lvr_empnum.Enabled = false;
                }
            }
         
            
        }

        #endregion


        #region if Appraisal Comments is Empty

        private void EmptyRecommend()
        {
            if (lvr_recmbyreason.Text == "")
            {
                lvr_apprdbyreason.Enabled = false;
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
            }
        }

        #endregion
     



        private void lvr_leaveto_Leave(object sender, EventArgs e)
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];

            if (lvr_leavefrom.Text.Trim() != string.Empty)
            {

                if (!isWrongLeaveDate())
                {
                    lvr["totaldays"] = GetNoDaysLeave();
                }
                else
                {
                    MessageBox.Show("Date From Can't be later than Date To", "PicoGuards Ltd. Pte.", MessageBoxButtons.OK);

                    lvr["leavefrom"] = System.DBNull.Value;
                    lvr["leaveto"] = System.DBNull.Value;
                    lvr["totaldays"] = System.DBNull.Value;
                }
            }
            else
            {
                MessageBox.Show("Date From Can't be Empty before selecting Date To", "PicoGuards Ltd. Pte.", MessageBoxButtons.OK);
                lvr["leavefrom"] = System.DBNull.Value;
                lvr["leaveto"] = System.DBNull.Value;
                lvr["totaldays"] = System.DBNull.Value;

            }
        }

        private void lvr_totaldays_Leave(object sender, EventArgs e)
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];

            if (lvr_leavefrom.Text.Trim() != string.Empty && lvr_leaveto.Text.Trim() != string.Empty)
            {
                decimal tmpTotalNodays = GetNoDaysLeave();

                string str1 = "select matnum,ISNULL(isHQstaff,0) as isHQstaff from matm where matnum='" + lvr["matnum"].ToString() + "' and status<>'V'";
                this.dbaccess.ReadSQL("dtCheckStaffType", str1);
                decimal dec = Convert.ToDecimal(0.50);

                if (this.dbaccess.DataSet.Tables["dtCheckStaffType"].Rows.Count > 0)
                {
                    DataRow dr1CheckStaffType = this.dbaccess.DataSet.Tables["dtCheckStaffType"].Rows[0];

                    if ((bool)dr1CheckStaffType["isHQstaff"])
                    {
                        if (tmpTotalNodays >= Convert.ToDecimal(lvr_totaldays.Text.Trim()) && (tmpTotalNodays - 1) < Convert.ToDecimal(lvr_totaldays.Text.Trim()))
                        {
                            lvr["totaldays"] = Convert.ToDecimal(lvr_totaldays.Text.Trim());
                        }
                        else
                        {
                            MessageBox.Show("Can't key in value which is more or less than a  Day", "Pico Guards Pte. Ltd", MessageBoxButtons.OK);
                            lvr["totaldays"] = GetNoDaysLeave();
                        }
                    }
                    else
                    {
                        if (tmpTotalNodays != Convert.ToDecimal(lvr_totaldays.Text.Trim()))
                        {
                            MessageBox.Show("Can't key in value which is More/Less than a  Day or Non-HQ Staff can't apply for Half-Day Leave", "Pico Guards Pte. Ltd", MessageBoxButtons.OK);
                            lvr["totaldays"] = GetNoDaysLeave();
                        }
                        else
                        {

                            lvr["totaldays"] = GetNoDaysLeave();
                        }
                      
                        
                        
                    }

                }

            }
            else
            {
                if (!isWrongLeaveDate())
                {
                    lvr["totaldays"] = GetNoDaysLeave();
                }
            }
        }

        private void lvr_empnum_Leave(object sender, EventArgs e)
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];
            if (lvr_empnum.Text != String.Empty)
            {
                DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(lvr_empnum.Text);
                if (tmpEmpData.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in tmpEmpData.Rows)
                    {
                        lvr["empnum"] = dr1["empnum"];
                        lvr["empname"] = dr1["empname"];
                        lvr["statuscode"] = dr1["statuscode"];
                        lvr["dateconfirmed"] = GetConfirmationDate(dr1["empnum"].ToString());
                        lvr["matnum"] = dr1["matnum"];
                        lvr["sitenumi"] = dr1["sitenumi"];
                        lvr["sectorcode"] = dr1["sectorcode"];
                        if (BizFunctions.IsEmpty(lvr["trandate"]))
                        {
                            lvr["trandate"] = DateTime.Now;
                        }
                        LeaveControl lv = new LeaveControl(dr1["empnum"].ToString(),Convert.ToDateTime(lvr["trandate"]));

                       
                        DataTable tmp = lv.GetLeaveTally();

                        if (tmp.Rows.Count > 0)
                        {
                         
                            if (lvr1.Rows.Count > 0)
                            {
                                BizFunctions.DeleteAllRows(lvr1);
                            }
                            foreach (DataRow dr2 in tmp.Rows)
                            {
                                lvr1.ImportRow(dr2);
                            }
                        }

                        lv = null;
                    }
                }
            }
        }
       
        #region Load Radio Button Data

        private void LoadRadioButtonsData()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["lvr"].Rows[0];

            if (!BizFunctions.IsEmpty(trqh["isrecommended"]))
            {
                if ((bool)trqh["isrecommended"])
                {
                    rad_Recd.Checked = true;
                }
                else
                {
                    rad_NotRecd.Checked = true;
                }
            }

            if (!BizFunctions.IsEmpty(trqh["isapproved"]))
            {
                if ((bool)trqh["isapproved"])
                {
                    rad_Appr.Checked = true;
                }
                else
                {
                    rad_Appr.Checked = false;
                }
            }
        }

           #endregion

        #region Appointment Code Dropdown

        protected void lvr_lvmnum_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from LVM WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("LVM", sql1);
            lvr_lvmnum.DataSource = this.dbaccess.DataSet.Tables["LVM"];
            lvr_lvmnum.DisplayMember = this.dbaccess.DataSet.Tables["LVM"].Columns["lvmnum"].ColumnName.ToString();
            lvr_lvmnum.ValueMember = this.dbaccess.DataSet.Tables["LVM"].Columns["lvmnum"].ColumnName.ToString();
        }

        #endregion


        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];



            //if(BizFunctions.IsEmpty(lvr["empnum"]))
            //{
            //    e.Handle=false;
            //}

            //if(e.Handle && BizFunctions.IsEmpty(lvr[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(lvr[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(lvr[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(lvr[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(lvr[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(lvr[""]))
            //{
            //    e.Handle=false;
            //}

           

            
            if (lvr1.Rows.Count > 0 && e.Handle)
            {
                foreach (DataRow dr2 in lvr1.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (lvr["lvmnum"].ToString() == dr2["lvmnum"].ToString())
                        {

                            decimal tmpDaysLeft = Convert.ToDecimal(dr2["daysleft"]);
                            decimal tmpDaysLeaveApplied = GetNoDaysLeave();
                            decimal total;
                            total = tmpDaysLeft - tmpDaysLeaveApplied;

                            if (total <= 0)
                            {
                                if (MessageBox.Show("You have applied for " + Convert.ToString(tmpDaysLeaveApplied) + " Days '" + lvr["lvmnum"].ToString() + "' Leave as the Balance is '" + Convert.ToString(tmpDaysLeft) + "' Days, It's over the balance  \nYes or No?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    e.Handle = false;
                                }                        
                            }
                        }
                    }
                }
            }


            
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "lvr_empnum":
                    {
                        
                        e.Condition = BizFunctions.F2Condition("empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "empnum like '" + lvr_empnum.Text.Trim() + "%' OR empname like '" + lvr_empnum.Text.Trim() + "%'";

                    }
                    break;
          
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];
            switch (e.ControlName)
            {
                case "lvr_empnum":
                    {
                        if (!BizFunctions.IsEmpty(lvr["empnum"]))
                        {
                            e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                            e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                            e.CurrentRow["statuscode"] = e.F2CurrentRow["statuscode"];
                            e.CurrentRow["dateconfirmed"] = GetConfirmationDate(lvr["empnum"].ToString());
                            e.CurrentRow["sitenumi"] = e.F2CurrentRow["sitenumi"];
                            e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            GetLeaveHistory();
                            if (BizFunctions.IsEmpty(lvr["trandate"]))
                            {
                                lvr["trandate"] = DateTime.Now;
                            }
                            LeaveControl lv = new LeaveControl(lvr["empnum"].ToString(),Convert.ToDateTime(lvr["trandate"]));

                            DataTable tmp = lv.GetLeaveTally();

                            if (tmp.Rows.Count > 0)
                            {
                                if (lvr1.Rows.Count > 0)
                                {
                                    BizFunctions.DeleteAllRows(lvr1);
                                }
                                foreach (DataRow dr2 in tmp.Rows)
                                {
                                    lvr1.ImportRow(dr2);
                                }
                            }
         
                           
                        }
                    }
                    break;
            }
        }
        #endregion

        #region GetStatus

        private void GetStatus()
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];


            string GetStatus = "Select statuscode from hemph where empnum='" + lvr["empnum"].ToString() + "'";

            this.dbaccess.ReadSQL("dtGetStatus", GetStatus);

            DataTable dtGetStatus = this.dbaccess.DataSet.Tables["dtGetStatus"];

            if (dtGetStatus.Rows.Count > 0)
            {
                lvr["statuscode"] = this.dbaccess.DataSet.Tables["dtGetStatus"].Rows[0]["statuscode"];
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

        #region Get Confirmation Date

        private DateTime GetConfirmationDate(string empnum)
        {
            DateTime dt = new DateTime();

            string GetConfirmation = "select CF.empnum,CF.confirmationdate from CFRH CF "+
                                        "LEFT JOIN HEMPH H ON CF.empnum=H.empnum  "+
                                        "where CF.[status]<>'V' and CF.empnum='" + empnum + "'";

            string GetDateJoined = "SELECT datejoined from HEMPH where [status]<>'V' and empnum='" + empnum + "' ";

            this.dbaccess.ReadSQL("dtCFR",GetConfirmation);

            DataTable dtCFR = this.dbaccess.DataSet.Tables["dtCFR"];

            if (dtCFR.Rows.Count > 0)
            {
                DataRow drCFR = this.dbaccess.DataSet.Tables["dtCFR"].Rows[0];
                dt = Convert.ToDateTime(drCFR["confirmationdate"]);
            }
            else
            {
                this.dbaccess.ReadSQL("dtDateJoined", GetDateJoined);

                DataTable dtDateJoined = this.dbaccess.DataSet.Tables["dtDateJoined"];

                if (dtDateJoined.Rows.Count > 0)
                {
                    DataRow drDateJoined = this.dbaccess.DataSet.Tables["dtDateJoined"].Rows[0];

                    if (!BizFunctions.IsEmpty(drDateJoined["datejoined"]))
                    {
                        dt = Convert.ToDateTime(drDateJoined["datejoined"]).AddMonths(3);
                    }
                }

            }

            return dt;
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
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];

            if(BizFunctions.IsEmpty(lvr["trandate"]))
            {
                lvr["trandate"] = DateTime.Now;
            }

            lvr["LeaveYear"] = Convert.ToDateTime(lvr["trandate"]).Year;

            if (ApprovedBy != string.Empty)
            {
                lvr["recmby"] = RecommendedBy;
            }
            if (RecommendedBy != string.Empty)
            {
                lvr["apprdby"] = ApprovedBy;
            }

            foreach (DataRow dr1 in lvr1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(lvr, dr1, "status/flag/user/trandate/created/modified/leaveyear");
                }
            }

          
           
                                  
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataRow lvr = this.dbaccess.DataSet.Tables["LVR"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];

            if (!BizFunctions.IsEmpty(lvr["empnum"]) && !BizFunctions.IsEmpty(lvr["trandate"]))
            {
                LeaveControl lv = new LeaveControl(lvr["empnum"].ToString(), Convert.ToDateTime(lvr["trandate"]));

                DataTable tmp = lv.GetLeaveTally();

                if (tmp.Rows.Count > 0)
                {
                    if (lvr1.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(lvr1);
                    }
                    foreach (DataRow dr2 in tmp.Rows)
                    {
                        lvr1.ImportRow(dr2);
                    }
                }
            }

            GetLeaveSummary();
    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];
            GetLeaveHistory();

            if (lvr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO && !BizFunctions.IsEmpty(lvr["empnum"]) || lvr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP && !BizFunctions.IsEmpty(lvr["empnum"]))
            {
                lvr["dateconfirmed"] = GetConfirmationDate(lvr["empnum"].ToString());
                GetStatus();
            }

            if (!BizFunctions.IsEmpty(lvr["empnum"]) && !BizFunctions.IsEmpty(lvr["trandate"]))
            {
                LeaveControl lv = new LeaveControl(lvr["empnum"].ToString(), Convert.ToDateTime(lvr["trandate"]));

                DataTable tmp = lv.GetLeaveTally();

                if (tmp.Rows.Count > 0)
                {
                    if (lvr1.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(lvr1);
                    }
                    foreach (DataRow dr2 in tmp.Rows)
                    {
                        lvr1.ImportRow(dr2);
                    }
                }
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

        #region Schedule Radio Button Methods
        private void rad_Recd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Recd.Checked)
            {
                DataRow lvm = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
                lvm["isrecommended"] = 1;
            }

        }

        private void rad_NotRecd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotRecd.Checked)
            {
                DataRow lvm = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
                lvm["isrecommended"] = 0;
            }
        }

        private void rad_Appr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Appr.Checked)
            {
                DataRow lvm = this.dbaccess.DataSet.Tables["lvr"].Rows[0];

                lvm["isapproved"] = 1;
            }
        }

        private void rad_NotAppr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotAppr.Checked)
            {
                DataRow lvm = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
                lvm["isapproved"] = 0;
            }
        }
        #endregion

        protected void trqh_recommendedcomments_TextChanged(object sender, EventArgs e)
        {
           DataRow lvm = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
           if (lvr_recmbyreason.Text != "")
            {
                lvr_apprdbyreason.Enabled = true;
                rad_Recd.Enabled = true;
                rad_NotRecd.Enabled = true;
                RecommendedBy = Common.DEFAULT_SYSTEM_USERNAME;

          
            }
            else
            {
                lvr_apprdbyreason.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;

            }

        
            //if (lvr_recmby.Text == "")
            //{
            //    lvm["recmby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}
        }

        private void trqh_approvedcomments_TextChanged(object sender, EventArgs e)
        {
            DataRow lvm = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            if (lvr_apprdbyreason.Text != "")
            {
                rad_Appr.Enabled = true;
                rad_NotAppr.Enabled = true;
                ApprovedBy = Common.DEFAULT_SYSTEM_USERNAME;
              
            }
            else
            {
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
            }
            //if (lvr_apprdby.Text == "")
            //{
            //    lvm["apprdby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}
        }

        private void GetHiredDate()
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            if (!BizFunctions.IsEmpty(lvr["empnum"]))
            {
                string str1 = "Select datejoined from hemph where empnum='" + lvr["empnum"].ToString() + "'";
                this.dbaccess.ReadSQL("dtCHECKdatejoined", str1);

                if (this.dbaccess.DataSet.Tables["dtCHECKdatejoined"].Rows.Count > 0)
                {
                    DataRow drCHECKdatejoined = this.dbaccess.DataSet.Tables["dtCHECKdatejoined"].Rows[0];
                    dateJoined = Convert.ToDateTime(drCHECKdatejoined["datejoined"]).ToShortDateString();
                }

            }

        }

        private decimal GetNoDaysLeave()
        {
            decimal Days = 0;
            DataRow lvr = this.dbaccess.DataSet.Tables["LVR"].Rows[0];
            if (!BizFunctions.IsEmpty(lvr["leavefrom"]) && !BizFunctions.IsEmpty(lvr["leaveto"]))
            {
                if (Convert.ToDateTime(lvr["leavefrom"]).ToShortDateString() == Convert.ToDateTime(lvr["leaveto"]).ToShortDateString())
                {
                    Days = 1;
                }
                else
                {
                    Days = Convert.ToDecimal(TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(lvr["leavefrom"])), BizFunctions.GetSafeDateString(Convert.ToDateTime(lvr["leaveto"]))));
                    Days = Days + 1;

                }
            }

            return Days;
        }

        private void GetLeaveHistory()
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["LVR"].Rows[0];

            if (!BizFunctions.IsEmpty(lvr["empnum"]))
            {
                string str1 = "SELECT Lv.refnum as Refnum, Lv.leavefrom as [From] ,Lv.leaveto as [To],Lm.lvmnum as [Desc],Lv.totaldays as Days,Lv.apprdby as ApprovedBy " +
                                "FROM LVR Lv left join LVM Lm on Lv.lvmnum=Lm.lvmnum " +
                                "where empnum='" + lvr["empnum"].ToString() + "' and lv.[status]<>'V' AND isapproved=1 " +
                                "order by lv.leavefrom";

                this.dbaccess.ReadSQL("dtLeaveHistory", str1);
                if (this.dbaccess.DataSet.Tables["dtLeaveHistory"].Rows.Count > 0)
                {
                    overallleavesummary.DataSource = this.dbaccess.DataSet.Tables["dtLeaveHistory"];

                }

                this.dbaccess.DataSet.Tables["dtLeaveHistory"].Dispose();
            }

            GetLeaveSummary();
        }

        private bool isWrongLeaveDate()
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            bool isWrongDate = false;
            int result = 0;
            string datetime1 = lvr_leavefrom.Text;
            string datetime2 = lvr_leaveto.Text;

            if (datetime1.Trim() != string.Empty && datetime2.Trim() != string.Empty)
            {
                

                if (datetime1 != string.Empty && datetime2 != string.Empty)
                {

                    DateTime dt1 = Convert.ToDateTime(datetime1);
                    DateTime dt2 = Convert.ToDateTime(datetime2);

                    result = DateTime.Compare(dt1, dt2);

                    if (result <= 0)
                    {
                        isWrongDate = false;

                        lvr["leavefrom"] = dt1;
                        lvr["leaveto"] = dt2;
                    }
                    else
                    {
                        isWrongDate = true;
                    }

                }
            }

            return isWrongDate;
        }


        //continue here
        private decimal GetProbationLeave()
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];

            decimal Days;

            Days = Convert.ToDecimal(TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(lvr["leavefrom"])), BizFunctions.GetSafeDateString(Convert.ToDateTime(lvr["leaveto"]))));
            Days = Days + 1;

            return Days;
        }


        static DateTime LastDayOfYear(DateTime d)
        {
            // 1
            // Get first of next year
            DateTime n = new DateTime(d.Year + 1, 1, 1);
            // 2
            // Subtract 1 from it
            return n.AddDays(-1);
        }

        private void GetLeaveSummary()
        {
            DataRow lvr = this.dbaccess.DataSet.Tables["LVR"].Rows[0];

            DateTime dt1 = DateTime.Now;
            DateTime dt2 = dt1.AddYears(-1);

            int CurrYear = dt1.Year;
            int PrevYear = dt2.Year;
           

            string GetLeaveSummary = " SELECT "+		   
                                            "B.ALcountPrevYear-B.ALentPrevYear AS [AL B/F FROM " + PrevYear.ToString()+ "], " +
		                                    "(B.ALentCurrYear+(B.ALcountPrevYear-B.ALentPrevYear))-B.ALcountCurrYear AS [YR "+CurrYear.ToString()+" BAL], "+
                                            "CASE  "+
			                                    "WHEN ((B.ALcountPrevYear-B.ALentPrevYear) + B.ALentCurrYear / 12 * MONTH(GETDATE()))<0 THEN 0  "+
			                                    "ELSE ((B.ALcountPrevYear-B.ALentPrevYear) + B.ALentCurrYear / 12 * MONTH(GETDATE())) "+
		                                    "END AS  [AL ENT CURR MTH], "+
		                                    "CASE "+
			                                    "WHEN (((B.ALcountPrevYear-B.ALentPrevYear) + B.ALentCurrYear / 12 * MONTH(GETDATE()))-B.ALcountCurrYear)< 0 THEN 0 "+
			                                    "ELSE (((B.ALcountPrevYear-B.ALentPrevYear) + B.ALentCurrYear / 12 * MONTH(GETDATE()))-B.ALcountCurrYear) "+
                                            "END AS [AL BAL CURR MTH], " +
		                                    "B.MCcountCurrYear AS [MC UTILIZED], "+
		                                    "B.ALcountCurrYear AS [AL UTILIZED], "+
		                                    "B.HPLcountCurrYear AS [HPL UTILIZED]"+
	                                     "FROM "+
		                                    "( "+
		                                    "SELECT "+
			                                    "A.empnum, "+
			                                    "A.DateJoined, "+
			                                    "A.DateConfirmed, "+
			                                    "A.YrServed, "+
			                                    "A.ALcountPrevYear, "+
			                                    "CASE WHEN ((ISNULL(YrServed,0)-1) + 7)>14 THEN 14 ELSE ((ISNULL(YrServed,0)-1) + 7) END AS ALentPrevYear, "+
			                                    "CASE WHEN (ISNULL(YrServed,0) + 7)>14 THEN 14 ELSE (ISNULL(YrServed,0) + 7) END AS ALentCurrYear, "+
			                                    "A.ALcountCurrYear, "+
			                                    "A.MCcountCurrYear, "+
			                                    "A.HPLcountCurrYear, "+
			                                    "A.UPLcountCurrYear "+
		                                    "FROM "+
			                                    "( "+
				                                    "SELECT "+
					                                    "H.empnum, "+
					                                    "CASE WHEN H.datejoined IS NOT NULL THEN convert(varchar, H.datejoined, 103) END AS DateJoined, "+
					                                    "CASE WHEN H.dateconfirmed IS NOT NULL THEN convert(varchar, H.dateconfirmed, 103) END AS DateConfirmed, "+
                                                        "DATEDIFF(yy,DATEJOINED,GETDATE()) AS YrServed, " +
					                                    "(Select sum(totaldays)  from LVR WHERE lvmnum IN ('AL','A/L') AND LeaveYear=YEAR(DATEADD(YYYY,-1,GETDATE())) and LVR.empnum=H.empnum  ) as ALcountPrevYear, "+
					                                    "(Select sum(totaldays)  from LVR WHERE lvmnum IN ('AL','A/L') AND LeaveYear=(YEAR(GETDATE())) and LVR.empnum=H.empnum  ) as ALcountCurrYear, "+
					                                    "(Select sum(totaldays)  from LVR WHERE lvmnum IN ('MED','ML') AND LeaveYear=(YEAR(GETDATE())) and LVR.empnum=H.empnum  ) as MCcountCurrYear, "+
					                                    "(Select sum(totaldays)  from LVR WHERE lvmnum IN ('HOS','HPL') AND LeaveYear=(YEAR(GETDATE())) and LVR.empnum=H.empnum  ) as HPLcountCurrYear, "+
                                                        "(Select sum(totaldays)  from LVR WHERE lvmnum LIKE 'UPL%' AND LeaveYear=(YEAR(GETDATE())) and LVR.empnum=H.empnum  ) as UPLcountCurrYear " +
				                                    "FROM HEMPH H "+
                                                    "Where H.empnum='"+ lvr["empnum"].ToString() +"' "+
			                                    ")A "+
                                            ")B";

            this.dbaccess.ReadSQL("tmpLeaveSum", GetLeaveSummary);

            DataTable GetLeaveSummaryDT = this.dbaccess.DataSet.Tables["tmpLeaveSum"];

            if (GetLeaveSummaryDT.Rows.Count > 0)
            {
             
                LeaveSummary.DataSource = GetLeaveSummaryDT;
            }

        }

      

    }
}
    

