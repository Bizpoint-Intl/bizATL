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
//using PicoGuards.BizModules.UserAuthorization;
using PicoGuards.TimeUtilites;
using PicoGuards.GridResize;

#endregion

namespace PicoGuards.LADJ
{
    public class Voucher_LADJ : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        //UserAuthorization sa = null;
        protected string headerFormName,LeaveHistoryFormName,LeaveSummaryFormName,RecommendedBy,ApprovedBy = null;
        protected TextBox ladj_recmbyreason, ladj_recmby, ladj_apprdbyreason, ladj_apprdby, ladj_empnum, ladj_totaldays = null;
        protected GroupBox grb_ladjhapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        private string dateJoined = "";
        protected DateTimePicker ladj_leavefrom, ladj_leaveto = null;
        ComboBox ladj_lvmnum = null;
        DataGrid overallleavesummary,LeaveSummary = null;



        #endregion

        #region Construct

        public Voucher_LADJ(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_LADJ.xml", moduleName, voucherBaseHelpers)
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

            //sa = new UserAuthorization(this.moduleName.ToString());

            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];

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
        //            e.EventTarget = new Header_LADJ(e.DBAccess, e.FormsCollection, e.DocumentKey);
        //            break;
        //    }
        //}

        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            ladj_empnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_empnum") as TextBox;

            ladj_empnum.Leave +=new EventHandler(ladj_empnum_Leave);

            ladj_totaldays = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_totaldays") as TextBox;

            ladj_totaldays.Leave +=new EventHandler(ladj_totaldays_Leave);

            ladj_lvmnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_lvmnum") as ComboBox;
            ladj_lvmnum.DropDown += new EventHandler(ladj_lvmnum_DropDown);
            grb_ladjhapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_ladjapprinfo") as GroupBox;
            ladj_recmby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_recmby") as TextBox;
            ladj_apprdby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_apprdby") as TextBox;
            ladj_recmbyreason = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_recmbyreason") as TextBox;
            ladj_apprdbyreason = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_apprdbyreason") as TextBox;

           

            rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            ladj_leavefrom = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_leavefrom") as DateTimePicker;

            //ladj_leavefrom.Leave +=new EventHandler(ladj_leavefrom_Leave);

            ladj_leaveto = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ladj_leaveto") as DateTimePicker;

            ladj_leaveto.Leave +=new EventHandler(ladj_leaveto_Leave);


            //overallleavesummary = BizXmlReader.CurrentInstance.GetControl(LeaveHistoryFormName, "dg_leaverec") as DataGrid;

            //LeaveSummary = BizXmlReader.CurrentInstance.GetControl(LeaveSummaryFormName, "dg_leavesummary") as DataGrid;
           

            rad_Recd.CheckedChanged += new EventHandler(rad_Recd_CheckedChanged);
            rad_NotRecd.CheckedChanged += new EventHandler(rad_NotRecd_CheckedChanged);
            rad_Appr.CheckedChanged += new EventHandler(rad_Appr_CheckedChanged);
            rad_NotAppr.CheckedChanged += new EventHandler(rad_NotAppr_CheckedChanged);
            LoadRadioButtonsData();

            ladj_recmbyreason.TextChanged += new EventHandler(trqh_recommendedcomments_TextChanged);
            ladj_apprdbyreason.TextChanged += new EventHandler(trqh_approvedcomments_TextChanged);

            EmptyRecommend();

            //if (!sa.ApprovePermission)
            //{
            //    grb_ladjhapprinfo.Enabled = false;
            //}
            //else
            //{
            //    grb_ladjhapprinfo.Enabled = true;
            //}



            if (!BizFunctions.IsEmpty(ladj["empnum"]))
            {
                string statuso = Common.DEFAULT_DOCUMENT_STATUSO;
                string statusp = Common.DEFAULT_DOCUMENT_STATUSP;
                string ladjstatus = ladj["status"].ToString();

                if (ladjstatus == statuso || ladjstatus == statusp)
                {
                    ladj_empnum.Enabled = false;
                }
            }

            if (BizFunctions.IsEmpty(ladj["leavefrom"]))
            {
                ladj["leavefrom"] = DateTime.Now;
            }
         
            
        }

        #endregion


        #region if Appraisal Comments is Empty

        private void EmptyRecommend()
        {
            if (ladj_recmbyreason.Text == "")
            {
                ladj_apprdbyreason.Enabled = false;
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
            }
        }

        #endregion
     



        //private void ladj_leaveto_Leave(object sender, EventArgs e)
        //{
        //    DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];

        //    if (ladj_leavefrom.Text.Trim() != string.Empty)
        //    {

        //        if (!isWrongLeaveDate())
        //        {
        //            ladj["totaldays"] = GetNoDaysLeave();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Date From Can't be later than Date To", "PicoGuards Ltd. Pte.", MessageBoxButtons.OK);

        //            ladj["leavefrom"] = System.DBNull.Value;
        //            ladj["leaveto"] = System.DBNull.Value;
        //            ladj["totaldays"] = System.DBNull.Value;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Date From Can't be Empty before selecting Date To", "PicoGuards Ltd. Pte.", MessageBoxButtons.OK);
        //        ladj["leavefrom"] = System.DBNull.Value;
        //        ladj["leaveto"] = System.DBNull.Value;
        //        ladj["totaldays"] = System.DBNull.Value;

        //    }
        //}

        //private void ladj_totaldays_Leave(object sender, EventArgs e)
        //{
        //    DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];

        //    if (ladj_leavefrom.Text.Trim() != string.Empty && ladj_leaveto.Text.Trim() != string.Empty)
        //    {
        //        decimal tmpTotalNodays = GetNoDaysLeave();

        //        string str1 = "select matnum,ISNULL(isHQstaff,0) as isHQstaff from matm where matnum='" + ladj["matnum"].ToString() + "' and status<>'V'";
        //        this.dbaccess.ReadSQL("dtCheckStaffType", str1);
        //        decimal dec = Convert.ToDecimal(0.50);

        //        if (this.dbaccess.DataSet.Tables["dtCheckStaffType"].Rows.Count > 0)
        //        {
        //            DataRow dr1CheckStaffType = this.dbaccess.DataSet.Tables["dtCheckStaffType"].Rows[0];

        //            if ((bool)dr1CheckStaffType["isHQstaff"])
        //            {
        //                if (tmpTotalNodays >= Convert.ToDecimal(ladj_totaldays.Text.Trim()) && (tmpTotalNodays - 1) < Convert.ToDecimal(ladj_totaldays.Text.Trim()))
        //                {
        //                    ladj["totaldays"] = Convert.ToDecimal(ladj_totaldays.Text.Trim());
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Can't key in value which is more or less than a  Day", "Pico Guards Pte. Ltd", MessageBoxButtons.OK);
        //                    ladj["totaldays"] = GetNoDaysLeave();
        //                }
        //            }
        //            else
        //            {
        //                if (tmpTotalNodays != Convert.ToDecimal(ladj_totaldays.Text.Trim()))
        //                {
        //                    MessageBox.Show("Can't key in value which is More/Less than a  Day or Non-HQ Staff can't apply for Half-Day Leave", "Pico Guards Pte. Ltd", MessageBoxButtons.OK);
        //                    ladj["totaldays"] = GetNoDaysLeave();
        //                }
        //                else
        //                {

        //                    ladj["totaldays"] = GetNoDaysLeave();
        //                }
                      
                        
                        
        //            }

        //        }

        //    }
        //    else
        //    {
        //        if (!isWrongLeaveDate())
        //        {
        //            ladj["totaldays"] = GetNoDaysLeave();
        //        }
        //    }
        //}

        private void ladj_empnum_Leave(object sender, EventArgs e)
        {
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            DataTable ladj1 = this.dbaccess.DataSet.Tables["ladj1"];
            if (ladj_empnum.Text != String.Empty)
            {
                DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(ladj_empnum.Text);
                if (tmpEmpData.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in tmpEmpData.Rows)
                    {
                        ladj["empnum"] = dr1["empnum"];
                        ladj["empname"] = dr1["empname"];
                        ladj["statuscode"] = dr1["statuscode"];
                        ladj["dateconfirmed"] = GetConfirmationDate(dr1["empnum"].ToString());
                        ladj["matnum"] = dr1["matnum"];
                        ladj["sitenumi"] = dr1["sitenumi"];
                        ladj["sectorcode"] = dr1["sectorcode"];
                        if (BizFunctions.IsEmpty(ladj["trandate"]))
                        {
                            ladj["trandate"] = DateTime.Now;
                        }
               
                    }
                }
                //GetLeaveHistory();
                //GetLeaveSummary();
            }
        }
       
        #region Load Radio Button Data

        private void LoadRadioButtonsData()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["ladj"].Rows[0];

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

        protected void ladj_lvmnum_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from LVM WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("LVM", sql1);
            ladj_lvmnum.DataSource = this.dbaccess.DataSet.Tables["LVM"];
            ladj_lvmnum.DisplayMember = this.dbaccess.DataSet.Tables["LVM"].Columns["lvmnum"].ColumnName.ToString();
            ladj_lvmnum.ValueMember = this.dbaccess.DataSet.Tables["LVM"].Columns["lvmnum"].ColumnName.ToString();
        }

        #endregion


        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            DataTable ladj1 = this.dbaccess.DataSet.Tables["ladj1"];

            if (BizFunctions.IsEmpty(ladj["lvmnum"]))
            {
                MessageBox.Show("You must choose the type of Leave", "Not Saved");
                e.Handle = false;
            }

            //if(BizFunctions.IsEmpty(ladj["empnum"]))
            //{
            //    e.Handle=false;
            //}

            //if(e.Handle && BizFunctions.IsEmpty(ladj[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(ladj[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(ladj[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(ladj[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(ladj[""]))
            //{
            //    e.Handle=false;
            //}
            // if(e.Handle && BizFunctions.IsEmpty(ladj[""]))
            //{
            //    e.Handle=false;
            //}

           

            
            if (ladj1.Rows.Count > 0 && e.Handle)
            {
                foreach (DataRow dr2 in ladj1.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (ladj["lvmnum"].ToString() == dr2["lvmnum"].ToString())
                        {

                            decimal tmpDaysLeft = Convert.ToDecimal(dr2["daysleft"]);
                            decimal tmpDaysLeaveApplied = GetNoDaysLeave();
                            decimal total;
                            total = tmpDaysLeft - tmpDaysLeaveApplied;

                            if (total <= 0)
                            {
                                if (MessageBox.Show("You have applied for " + Convert.ToString(tmpDaysLeaveApplied) + " Days '" + ladj["lvmnum"].ToString() + "' Leave as the Balance is '" + Convert.ToString(tmpDaysLeft) + "' Days, It's over the balance  \nYes or No?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
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
                case "ladj_empnum":
                    {
                        
                        e.Condition = BizFunctions.F2Condition("empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "empnum like '" + ladj_empnum.Text.Trim() + "%' OR empname like '" + ladj_empnum.Text.Trim() + "%'";

                    }
                    break;
          
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            DataTable ladj1 = this.dbaccess.DataSet.Tables["ladj1"];
            switch (e.ControlName)
            {
                case "ladj_empnum":
                    {
                        if (!BizFunctions.IsEmpty(ladj["empnum"]))
                        {
                            e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                            e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                            e.CurrentRow["statuscode"] = e.F2CurrentRow["statuscode"];
                            e.CurrentRow["dateconfirmed"] = GetConfirmationDate(ladj["empnum"].ToString());
                            e.CurrentRow["sitenumi"] = e.F2CurrentRow["sitenumi"];
                            e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            GetLeaveHistory();
                            if (BizFunctions.IsEmpty(ladj["trandate"]))
                            {
                                ladj["trandate"] = DateTime.Now;
                            }
                            //LeaveControl lv = new LeaveControl(ladj["empnum"].ToString(),Convert.ToDateTime(ladj["trandate"]));

                            //DataTable tmp = lv.GetLeaveTally();

                            //if (tmp.Rows.Count > 0)
                            //{
                            //    if (ladj1.Rows.Count > 0)
                            //    {
                            //        BizFunctions.DeleteAllRows(ladj1);
                            //    }
                            //    foreach (DataRow dr2 in tmp.Rows)
                            //    {
                            //        ladj1.ImportRow(dr2);
                            //    }
                            //}
                            GetLeaveHistory();
                            GetLeaveSummary();
                           
                        }
                    }
                    break;
            }
        }
        #endregion

        #region GetStatus

        private void GetStatus()
        {
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];


            string GetStatus = "Select statuscode from hemph where empnum='" + ladj["empnum"].ToString() + "'";

            this.dbaccess.ReadSQL("dtGetStatus", GetStatus);

            DataTable dtGetStatus = this.dbaccess.DataSet.Tables["dtGetStatus"];

            if (dtGetStatus.Rows.Count > 0)
            {
                ladj["statuscode"] = this.dbaccess.DataSet.Tables["dtGetStatus"].Rows[0]["statuscode"];
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
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            DataTable ladj1 = this.dbaccess.DataSet.Tables["ladj1"];

            if(BizFunctions.IsEmpty(ladj["trandate"]))
            {
                ladj["trandate"] = DateTime.Now;
            }

            ladj["LeaveYear"] = Convert.ToDateTime(ladj["trandate"]).Year;

            //if (ApprovedBy != string.Empty)
            //{
            //    ladj["recmby"] = RecommendedBy;
            //}
            //if (RecommendedBy != string.Empty)
            //{
            //    ladj["apprdby"] = ApprovedBy;
            //}

            foreach (DataRow dr1 in ladj1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ladj, dr1, "status/flag/user/trandate/created/modified/leaveyear");
                }
            }

          
           
                                  
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataRow ladj = this.dbaccess.DataSet.Tables["LADJ"].Rows[0];
            DataTable ladj1 = this.dbaccess.DataSet.Tables["ladj1"];

            if (!BizFunctions.IsEmpty(ladj["empnum"]) && !BizFunctions.IsEmpty(ladj["trandate"]))
            {
                //LeaveControl lv = new LeaveControl(ladj["empnum"].ToString(), Convert.ToDateTime(ladj["trandate"]));

                //DataTable tmp = lv.GetLeaveTally();

                //if (tmp.Rows.Count > 0)
                //{
                //    if (ladj1.Rows.Count > 0)
                //    {
                //        BizFunctions.DeleteAllRows(ladj1);
                //    }
                //    foreach (DataRow dr2 in tmp.Rows)
                //    {
                //        ladj1.ImportRow(dr2);
                //    }
                //}
            }

            GetLeaveHistory();
            GetLeaveSummary();
    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            DataTable ladj1 = this.dbaccess.DataSet.Tables["ladj1"];
            GetLeaveHistory();

            if (ladj["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO && !BizFunctions.IsEmpty(ladj["empnum"]) || ladj["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP && !BizFunctions.IsEmpty(ladj["empnum"]))
            {
                ladj["dateconfirmed"] = GetConfirmationDate(ladj["empnum"].ToString());
                GetStatus();
            }

            if (!BizFunctions.IsEmpty(ladj["empnum"]) && !BizFunctions.IsEmpty(ladj["trandate"]))
            {
                //LeaveControl lv = new LeaveControl(ladj["empnum"].ToString(), Convert.ToDateTime(ladj["trandate"]));

                //DataTable tmp = lv.GetLeaveTally();

                //if (tmp.Rows.Count > 0)
                //{
                //    if (ladj1.Rows.Count > 0)
                //    {
                //        BizFunctions.DeleteAllRows(ladj1);
                //    }
                //    foreach (DataRow dr2 in tmp.Rows)
                //    {
                //        ladj1.ImportRow(dr2);
                //    }
                //}
            }

            GetLeaveHistory();
            GetLeaveSummary();

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
                DataRow lvm = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
                lvm["isrecommended"] = 1;
            }

        }

        private void rad_NotRecd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotRecd.Checked)
            {
                DataRow lvm = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
                lvm["isrecommended"] = 0;
            }
        }

        private void rad_Appr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Appr.Checked)
            {
                DataRow lvm = this.dbaccess.DataSet.Tables["ladj"].Rows[0];

                lvm["isapproved"] = 1;
            }
        }

        private void rad_NotAppr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotAppr.Checked)
            {
                DataRow lvm = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
                lvm["isapproved"] = 0;
            }
        }
        #endregion

        protected void trqh_recommendedcomments_TextChanged(object sender, EventArgs e)
        {
           DataRow lvm = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
           if (ladj_recmbyreason.Text != "")
            {
                ladj_apprdbyreason.Enabled = true;
                rad_Recd.Enabled = true;
                rad_NotRecd.Enabled = true;
                RecommendedBy = Common.DEFAULT_SYSTEM_USERNAME;

          
            }
            else
            {
                ladj_apprdbyreason.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;

            }

        
            //if (ladj_recmby.Text == "")
            //{
            //    lvm["recmby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}
        }

        private void trqh_approvedcomments_TextChanged(object sender, EventArgs e)
        {
            DataRow lvm = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            if (ladj_apprdbyreason.Text != "")
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
            //if (ladj_apprdby.Text == "")
            //{
            //    lvm["apprdby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}
        }

        private void GetHiredDate()
        {
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            if (!BizFunctions.IsEmpty(ladj["empnum"]))
            {
                string str1 = "Select datejoined from hemph where empnum='" + ladj["empnum"].ToString() + "'";
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
            DataRow ladj = this.dbaccess.DataSet.Tables["LADJ"].Rows[0];
            if (!BizFunctions.IsEmpty(ladj["leavefrom"]) && !BizFunctions.IsEmpty(ladj["leaveto"]))
            {
                if (Convert.ToDateTime(ladj["leavefrom"]).ToShortDateString() == Convert.ToDateTime(ladj["leaveto"]).ToShortDateString())
                {
                    Days = 1;
                }
                else
                {
                    Days = Convert.ToDecimal(TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])), BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leaveto"]))));
                    Days = Days + 1;

                }
            }

            return Days;
        }

        private void GetLeaveHistory()
        {
            DataRow ladj = this.dbaccess.DataSet.Tables["LADJ"].Rows[0];

            if (!BizFunctions.IsEmpty(ladj["empnum"]))
            {
                string str1 = "SELECT Lv.refnum as Refnum, Lv.LeaveYear as [Year], Lv.leavefrom as [From] ,Lv.leaveto as [To],Lm.lvmnum as [Desc],Lv.totaldays as Days,Lv.apprdby as ApprovedBy " +
                                "FROM LADJ Lv left join LVM Lm on Lv.lvmnum=Lm.lvmnum " +
                                "where empnum='" + ladj["empnum"].ToString() + "' and lv.[status]<>'V' AND isapproved=1 AND Lv.totaldays>0" +
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
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];
            bool isWrongDate = false;
            int result = 0;
            string datetime1 = ladj_leavefrom.Text;
            string datetime2 = ladj_leaveto.Text;

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

                        ladj["leavefrom"] = dt1;
                        ladj["leaveto"] = dt2;
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
            DataRow ladj = this.dbaccess.DataSet.Tables["ladj"].Rows[0];

            decimal Days;

            Days = Convert.ToDecimal(TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])), BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leaveto"]))));
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
            DataRow ladj = this.dbaccess.DataSet.Tables["LADJ"].Rows[0];

            DateTime dt1 = Convert.ToDateTime(ladj["leavefrom"]);
            DateTime dt2 = dt1.AddYears(-1);

            int CurrYear = dt1.Year;
            int PrevYear = dt2.Year;

            decimal LeaveBalRemain = 0;

            if (!BizFunctions.IsEmpty(ladj["empnum"]) && !BizFunctions.IsEmpty(ladj["leavefrom"]))
            {

               

              

                try
                {
                    Parameter[] parameters = new Parameter[2];
                    parameters[0] = new Parameter("@Empnum", ladj["empnum"].ToString());
                    parameters[1] = new Parameter("@Date", BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])));
                    DataSet ds_LvBal = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_GetLeaveBal", ref parameters);
                    ds_LvBal.Tables[0].TableName = "LeaveBalTb";

                    if (ds_LvBal != null)
                    {
                        DataTable lvBal = ds_LvBal.Tables[0];
                        if (lvBal.Rows.Count > 0)
                        {

                            for (int i = 0; i <= lvBal.Rows.Count-1; i++)
                            {
                                if (lvBal.Rows[i].RowState != DataRowState.Deleted)
                                {
                                    if ((i + 1) <= lvBal.Rows.Count-1)
                                    {
                                        //lvBal.Rows[i]["ALbal"] = (Convert.ToDecimal(lvBal.Rows[i]["ALBFprevYr"]) + Convert.ToDecimal(lvBal.Rows[i]["LveEntitlement"])) - Convert.ToDecimal(lvBal.Rows[i]["ALutil"]);
                                        lvBal.Rows[i]["ALbal"] = (Convert.ToDecimal(lvBal.Rows[i]["ALBFprevYr"]) + Convert.ToDecimal(lvBal.Rows[i]["ALbal"]));
                                        lvBal.Rows[i + 1]["ALBFprevYr"] = Convert.ToDecimal(lvBal.Rows[i]["ALbal"]);
                                    }
                                    else
                                    {
                                        lvBal.Rows[i]["ALbal"] = (Convert.ToDecimal(lvBal.Rows[i]["ALBFprevYr"]) + Convert.ToDecimal(lvBal.Rows[i]["LveEntitlement"])) - Convert.ToDecimal(lvBal.Rows[i]["ALutil"]);

                                    }
                                    
                                    
                                }
                            }


                            int maxrow = lvBal.Rows.Count - 1;

                            lvBal.Rows[maxrow]["AL_ENT_CURR_MTH"] = Math.Round((((Convert.ToDecimal(lvBal.Rows[maxrow]["LveEntitlement"]) / 12) * Convert.ToDateTime(ladj["leavefrom"]).Month) + Convert.ToInt32(lvBal.Rows[maxrow]["ALBFprevYr"])), 0, MidpointRounding.AwayFromZero);
                            lvBal.Rows[maxrow]["AL_BAL_CURR_MTH"] = Convert.ToDecimal(lvBal.Rows[maxrow]["AL_ENT_CURR_MTH"]) - Convert.ToDecimal(lvBal.Rows[maxrow]["ALutil"]);
                        }
                    }


          

                     

                    //string GetLeaveSummary = "SELECT " +
                    //                            "ROUND(C.ALbfPrevYear,0.5) as [AL B/F FROM " + PrevYear.ToString() + "], " +
                    //                            "ROUND(C.ALentCurrYear,0.5) as [AL ENT " + CurrYear.ToString() + "], " +
                    //                            "ROUND((C.ALentCurrYear+C.ALbfPrevYear)-C.ALcountCurrYear,0.5) AS [AL BAL " + CurrYear.ToString() + "], " +
                    //                            "ROUND((C.ALbfPrevYear+C.ALentCurrMonthF1),0.5) AS [AL ENT CURR MTH], " +
                    //                            "ROUND((C.ALbfPrevYear+C.ALentCurrMonthF1) - C.ALcountCurrYear,0.5) AS [AL BAL CURR MTH], " +
                    //                            "(SELECT noOfdays FROM LVE1 WHERE LVE1.matnum=C.matnum AND lvmnum IN ('ML','MED','MC')) AS [MC ENT " + CurrYear.ToString() + "], " +
                    //                            "(SELECT noOfdays FROM LVE1 WHERE LVE1.matnum=C.matnum AND lvmnum IN ('ML','MED','MC'))-C.MCcountCurrYear as [MC REMAIN], " +
                    //                            "ROUND(C.MCcountCurrYear,0.5) AS [MC UTILIZED], " +
                    //                            "ROUND(C.ALcountCurrYear,0.5) AS [AL UTILIZED], " +
                    //                            "ROUND(C.HPLcountCurrYear,0.5) AS [HPL UTILIZED]  " +
                    //                            "FROM ( " +
                    //                                    "SELECT " +
                    //                                        "B.matnum, " +
                    //                                        "B.empnum, " +
                    //                                        "B.empname, " +
                    //                                        "B.DateJoined, " +
                    //                                        "MONTH(CONVERT(DATETIME,B.DateJoined,103)) as MonthJoined, " +
                    //                                        "YEAR(CONVERT(DATETIME,B.DateJoined,103)) as YearJoined,	" +
                    //                                        "B.DateConfirmed, " +
                    //                                        "B.ALcountPrevYear, " +
                    //                                        "B.ALentPrevYear, " +
                    //                                        " " + Convert.ToString(LeaveBalRemain) + " AS ALbfPrevYear, " +
                    //                                        "B.ALentCurrYear, " +
                    //                                        "ROUND(((B.ALentCurrYear/12.00)*MONTH(CONVERT(DATETIME,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "',103))),1) AS ALentCurrMonthF1, " +
                    //                                        "B.MCcountCurrYear, " +
                    //                                        "B.ALcountCurrYear, " +
                    //                                        "B.HPLcountCurrYear " +
                    //                                        "FROM ( " +
                    //                                                "SELECT " +
                    //                                                    "A.matnum, " +
                    //                                                    "A.empnum, " +
                    //                                                    "A.empname, " +
                    //                                                    "A.DateJoined, " +
                    //                                                    "A.DateConfirmed, " +
                    //                                                    "A.ALcountPrevYear, " +
                    //                                                    "CASE WHEN A.YrServedPrev + 7 > 14 THEN 14 ELSE A.YrServedPrev + 7  END AS ALentPrevYear, " +
                    //                                                    "CASE WHEN A.YrServedCurr + 7 > 14 THEN 14 ELSE A.YrServedCurr + 7  END AS ALentCurrYear, " +
                    //                                                    "ISNULL(A.ALcountCurrYear,0) AS ALcountCurrYear, " +
                    //                                                    "ISNULL(A.MCcountCurrYear,0) AS MCcountCurrYear, " +
                    //                                                    "ISNULL(A.HPLcountCurrYear,0) AS HPLcountCurrYear, " +
                    //                                                    "ISNULL(A.UPLcountCurrYear,0) as UPLcountCurrYear " +
                    //                                                    "FROM ( " +
                    //                                                            "SELECT " +
                    //                                                                "H.matnum, " +
                    //                                                                "H.empnum, " +
                    //                                                                "H.empname, " +
                    //                                                                "CASE " +
                    //                                                                    "WHEN H.datejoined IS NOT NULL THEN convert(varchar, H.datejoined, 103) " +
                    //                                                                "END AS DateJoined, " +
                    //                                                                "CASE " +
                    //                                                                    "WHEN H.dateconfirmed IS NOT NULL THEN convert(varchar, H.dateconfirmed, 103) " +
                    //                                                                "END AS DateConfirmed, " +
                    //                                                                "CASE " +
                    //                                                                    "WHEN H.DateJoined is not null then DBO.fn_GetYears(DATEJOINED,CONVERT(DATETIME,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "',103)) " +
                    //                                                                "END AS YrServedCurr,  " +
                    //                                                                "CASE " +
                    //                                                                    "WHEN dbo.fn_GetYears(h.datejoined,DATEADD(MILLISECOND, -3, DATEADD(YEAR,DATEDIFF(YEAR, 0, DATEADD(YEAR, -1, '" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "')) + 1, 0)))<0 THEN 0 " +
                    //                                                                    "ELSE dbo.fn_GetYears(h.datejoined,DATEADD(MILLISECOND, -3, DATEADD(YEAR,DATEDIFF(YEAR, 0, DATEADD(YEAR, -1, '" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "')) + 1, 0))) " +
                    //                                                                "END as YrServedPrev, " +
                    //                                                                "( " +
                    //                                                                    "Select sum(totaldays)  from LADJ WHERE lvmnum IN ('AL','A/L') AND LeaveYear=YEAR(DATEADD(YYYY,-1,CONVERT(DATETIME,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "',103))) " +
                    //                                                                    "and " +
                    //                                                                    "LADJ.empnum=H.empnum  " +
                    //                                                                ") as ALcountPrevYear, " +
                    //                                                                "( " +
                    //                                                                    "Select sum(totaldays)  from LADJ WHERE lvmnum IN ('AL','A/L') AND LeaveYear=(YEAR(CONVERT(DATETIME,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "',103))) " +
                    //                                                                    "and " +
                    //                                                                    "LADJ.empnum=H.empnum  and [status]<>'V' " +
                    //                                                                ") as ALcountCurrYear, " +
                    //                                                                "( " +
                    //                                                                    "Select sum(totaldays)  from LADJ WHERE lvmnum IN ('MED','ML') AND LeaveYear=(YEAR(CONVERT(DATETIME,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "',103))) " +
                    //                                                                    "and " +
                    //                                                                    "LADJ.empnum=H.empnum  and [status]<>'V' " +
                    //                                                                ") as MCcountCurrYear, " +
                    //                                                                "( " +
                    //                                                                    "Select sum(totaldays)  from LADJ WHERE lvmnum IN ('HOS','HPL') " +
                    //                                                                    "AND LeaveYear=(YEAR(CONVERT(DATETIME,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "',103))) and LADJ.empnum=H.empnum  and [status]<>'V' " +
                    //                                                                ") as HPLcountCurrYear, " +
                    //                                                                "( " +
                    //                                                                    "Select sum(totaldays)  from LADJ WHERE lvmnum LIKE 'UPL%' " +
                    //                                                                    "AND LeaveYear=(YEAR(CONVERT(DATETIME,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ladj["leavefrom"])) + "',103))) and LADJ.empnum=H.empnum  and [status]<>'V' " +
                    //                                                                ") as UPLcountCurrYear " +
                    //                                                                "FROM HEMPH H  " +
                    //                                                                "where H.EMPNUM='" + ladj["empnum"].ToString() + "' " +
                    //                                                            ")A  " +
                    //                                                ")B " +
                    //                                        ")C";


                    string GetLeaveSummary = "SELECT " +
                                                 "ALBFprevYr as [AL B/F FRM " + PrevYear.ToString() + "], " +
                                                 "LveEntitlement [AL ENT " + CurrYear.ToString() + "], " +
                                                 "ALbal AS [AL BAL " + CurrYear.ToString() + "], " +
                                                 "AL_ENT_CURR_MTH AS [AL ENT CURR MTH], " +
                                                 "AL_BAL_CURR_MTH AS [AL BAL CURR MTH], " +
                                                 "MlEnt AS [MC ENT " + CurrYear.ToString() + "], " +
                                                 "MlRemm as [MC REMAIN], " +
                                                 "MLutil AS [MC UTIL], " +
                                                 "ALutil AS [AL UTIL], " +
                                                 "HPutil AS [HPL UTIL],  " +
                                                 "CHCutil AS [CHDC UTIL],  " +
                                                 "MTLutil AS [MTL UTIL],  " +
                                                 "CPLutil AS [CPL UTIL]  " +
                                             "FROM LeaveBalTb "+
                                             "WHERE ForBottom=1";

                    DataTable GetLeaveSummaryDT = BizFunctions.ExecuteQuery(ds_LvBal, GetLeaveSummary);

                    //this.dbaccess.ReadSQL("tmpLeaveSum", GetLeaveSummary);

                    //DataTable GetLeaveSummaryDT = this.dbaccess.DataSet.Tables["tmpLeaveSum"];

                    if (GetLeaveSummaryDT.Rows.Count > 0)
                    {

                        LeaveSummary.DataSource = GetLeaveSummaryDT;

                        AutoResizeDataGridTableStyle style = new AutoResizeDataGridTableStyle();
                        LeaveSummary.DataSourceChanged += new EventHandler(style.OnDataSourceChanged);
                        LeaveSummary.Resize += new EventHandler(style.OnDataGridResize);
                        style.MappingName = ds_LvBal.Tables[0].TableName;
                        LeaveSummary.TableStyles.Add(style);
  

                    }
                }
                catch (Exception ex)
                {
                }

            }
        }

       


      

    }
}
    

