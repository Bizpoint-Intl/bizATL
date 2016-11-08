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
//using BizERP.BizModules.UserAuthorization;
using ATL.TimeUtilites;
using ATL.GridResize;
using System.Net.Mail;
//using Outlook = Microsoft.Office.Interop.Outlook;
using System.Web.UI;
using System.Text.RegularExpressions;
#endregion

namespace ATL.LVR
{
    public class Voucher_LVR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        //UserAuthorization sa = null;
        protected string headerFormName, LeaveHistoryFormName, LeaveSummaryFormName, RecommendedBy, ApprovedBy = null;
        protected TextBox lvr_recmbyreason, lvr_recmby, lvr_apprdbyreason, lvr_apprdby, lvr_empnum, lvr_totaldays = null;
        protected GroupBox grb_lvrhapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        private string dateJoined = "";
        protected DateTimePicker lvr_leavefrom, lvr_leaveto = null;
        ComboBox lvr_lvmnum = null;
        DataGrid overallleavesummary, LeaveSummary = null;
        protected string flag = "";
        

        Button lv = null;

        #endregion

        #region Construct

        public Voucher_LVR(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_LVR.xml", moduleName, voucherBaseHelpers)
        {
            this.flag = moduleName;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);
            if (flag.ToUpper().Trim() != "LVRH")
            {
                e.Condition = "lvr.flag='" + flag + "' AND lvr.leaveyear = " + Common.DEFAULT_SYSTEM_YEAR;
            }
            else
            {
                e.Condition = "lvr.flag='" + flag + "' AND lvr.leaveyear = " + Common.DEFAULT_SYSTEM_YEAR + " and (lvr.[user]='" + Common.DEFAULT_SYSTEM_USERNAME + "' OR '" + Common.DEFAULT_SYSTEM_USERNAME + "' IN (Select UserName from SysUserGroup where GroupName='Administrator' ) OR empnum in (Select empnum from hemph where supv='" + Common.DEFAULT_SYSTEM_EMPNUM + "')) ";
            }

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);
            if (flag.ToUpper().Trim() != "LVRH")
            {
                e.Condition = " (lvr.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                       " lvr.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                       " lvr.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                       " AND lvr.flag='" + flag + "' AND lvr.leaveyear = " + Common.DEFAULT_SYSTEM_YEAR;
            }
            else
            {
                e.Condition = " (lvr.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                      " lvr.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                      " lvr.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                      " AND lvr.flag='" + flag + "' AND lvr.leaveyear = " + Common.DEFAULT_SYSTEM_YEAR + " and (lvr.[user]='" + Common.DEFAULT_SYSTEM_USERNAME + "' OR '" + Common.DEFAULT_SYSTEM_USERNAME + "' IN (Select UserName from SysUserGroup where GroupName='Administrator' ) OR empnum in (Select empnum from hemph where supv='" + Common.DEFAULT_SYSTEM_EMPNUM + "')) ";
            }

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
            DataRow lvr = e.DBAccess.DataSet.Tables["lvr"].Rows[0];
            this.dbaccess = e.DBAccess;
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.LeaveHistoryFormName = (e.FormsCollection["leaverec"] as Form).Name;
            //this.LeaveSummaryFormName = (e.FormsCollection["leavesum"] as Form).Name;

            //sa = new UserAuthorization(this.moduleName.ToString());

            if (lvr["flag"].ToString().ToUpper().Trim() == "LVRH")
            {
                if (BizFunctions.IsEmpty(lvr["empnum"]))
                {
                    lvr["empnum"] = Common.DEFAULT_SYSTEM_EMPNUM;
                    
                }

                //if (lvr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO)
                //{
                //    lvr["apprdby"] = Common.DEFAULT_SYSTEM_USERNAME;
                //}

                if (BizFunctions.IsEmpty(lvr["apprdby"]))
                {
                    lvr["apprdby"] = GetSupervisor(lvr["empnum"].ToString());
                }
            }
            


            e.DBAccess.DataSet.Tables["lvr"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_LVR_ColumnChanged);

            if (!BizFunctions.IsEmpty(lvr["empnum"]))
            {
                DataTable EmpDT = BizLogicTools.Tools.GetCommonEmpData(lvr["empnum"].ToString());
                if (EmpDT.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(lvr["empname"]))
                    {
                        lvr["empname"] = EmpDT.Rows[0]["empname"].ToString();
                    }
                    if (BizFunctions.IsEmpty(lvr["matnum"]))
                    {
                        lvr["matnum"] = EmpDT.Rows[0]["matnum"].ToString();
                    }
                    if (BizFunctions.IsEmpty(lvr["statuscode"]))
                    {
                        lvr["statuscode"] = EmpDT.Rows[0]["statuscode"].ToString();
                    }

                    if (BizFunctions.IsEmpty(lvr["sitenum"]))
                    {
                        lvr["sitenum"] = EmpDT.Rows[0]["sitenum"].ToString();
                    }
                    //if (BizFunctions.IsEmpty(lvr["sectorcode"]))
                    //{
                    //    lvr["sectorcode"] = BizLogicTools.Tools.GetSectorCode(lvr["sitenum"].ToString(), this.dbaccess);
                    //}
                }
            }
            if (BizFunctions.IsEmpty(lvr["trandate"]))
            {
                lvr["trandate"] = DateTime.Now;
            }

            Initialise();
            GetHiredDate();

            //


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

            lvr_empnum.Leave += new EventHandler(lvr_empnum_Leave);

            lvr_totaldays = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_totaldays") as TextBox;

            lvr_totaldays.Leave += new EventHandler(lvr_totaldays_Leave);

            lvr_lvmnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_lvmnum") as ComboBox;
            lvr_lvmnum.DropDown += new EventHandler(lvr_lvmnum_DropDown);
            grb_lvrhapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_lvrapprinfo") as GroupBox;
            lvr_recmby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_recmby") as TextBox;
            lvr_apprdby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_apprdby") as TextBox;
            lvr_recmbyreason = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_recmbyreason") as TextBox;
            lvr_apprdbyreason = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_apprdbyreason") as TextBox;

            lv = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_send") as Button;
            lv.Click += new EventHandler(lv_click);
            rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            lvr_leavefrom = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_leavefrom") as DateTimePicker;

            //lvr_leavefrom.Leave +=new EventHandler(lvr_leavefrom_Leave);

            lvr_leaveto = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvr_leaveto") as DateTimePicker;

            lvr_leaveto.Leave += new EventHandler(lvr_leaveto_Leave);


            overallleavesummary = BizXmlReader.CurrentInstance.GetControl(LeaveHistoryFormName, "dg_leaverec") as DataGrid;

            //LeaveSummary = BizXmlReader.CurrentInstance.GetControl(LeaveSummaryFormName, "dg_leavesummary") as DataGrid;


            rad_Recd.CheckedChanged += new EventHandler(rad_Recd_CheckedChanged);
            rad_NotRecd.CheckedChanged += new EventHandler(rad_NotRecd_CheckedChanged);
            rad_Appr.CheckedChanged += new EventHandler(rad_Appr_CheckedChanged);
            rad_NotAppr.CheckedChanged += new EventHandler(rad_NotAppr_CheckedChanged);
            LoadRadioButtonsData();

            lvr_recmbyreason.TextChanged += new EventHandler(trqh_recommendedcomments_TextChanged);
            lvr_apprdbyreason.TextChanged += new EventHandler(trqh_approvedcomments_TextChanged);

            EmptyRecommend();

            //if (!sa.ApprovePermission)
            //{
            //    grb_lvrhapprinfo.Enabled = false;
            //}
            //else
            //{
            //    grb_lvrhapprinfo.Enabled = true;
            //}



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

            if (BizFunctions.IsEmpty(lvr["leavefrom"]))
            {
                lvr["leavefrom"] = DateTime.Now;
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
                    MessageBox.Show("Date From Can't be later than Date To", "BizERP Ltd. Pte.", MessageBoxButtons.OK);

                    lvr["leavefrom"] = System.DBNull.Value;
                    lvr["leaveto"] = System.DBNull.Value;
                    lvr["totaldays"] = System.DBNull.Value;
                }
            }
            else
            {
                MessageBox.Show("Date From Can't be Empty before selecting Date To", "BizERP Ltd. Pte.", MessageBoxButtons.OK);
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

                //if (this.dbaccess.DataSet.Tables["dtCheckStaffType"].Rows.Count > 0)
                //{
                //    DataRow dr1CheckStaffType = this.dbaccess.DataSet.Tables["dtCheckStaffType"].Rows[0];

                //    if ((bool)dr1CheckStaffType["isHQstaff"])
                //    {
                //        if (tmpTotalNodays >= Convert.ToDecimal(lvr_totaldays.Text.Trim()) && (tmpTotalNodays - 1) < Convert.ToDecimal(lvr_totaldays.Text.Trim()))
                //        {
                //            lvr["totaldays"] = Convert.ToDecimal(lvr_totaldays.Text.Trim());
                //        }
                //        else
                //        {
                //            MessageBox.Show("Can't key in value which is more or less than a  Day", "BizERP", MessageBoxButtons.OK);
                //            lvr["totaldays"] = GetNoDaysLeave();
                //        }
                //    }
                //    else
                //    {
                //        if (tmpTotalNodays != Convert.ToDecimal(lvr_totaldays.Text.Trim()))
                //        {
                //            MessageBox.Show("Can't key in value which is More/Less than a  Day or Non-HQ Staff can't apply for Half-Day Leave", "BizERP", MessageBoxButtons.OK);
                //            lvr["totaldays"] = GetNoDaysLeave();
                //        }
                //        else
                //        {

                //            lvr["totaldays"] = GetNoDaysLeave();
                //        }

                //    }

                //}

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
                DataTable tmpEmpData = null;
                tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(lvr_empnum.Text);

                if (lvr["flag"].ToString().ToUpper().Trim() == "LVRH")
                {
                    if (!BizFunctions.IsEmpty(lvr["empnum"]))
                    {
                        if (lvr["empnum"].ToString().Trim() != Common.DEFAULT_SYSTEM_EMPNUM && !isAllowedToChangeEmpnum(Common.DEFAULT_SYSTEM_USERNAME))
                        {
                            MessageBox.Show("You can only choose your own Employee Number", "Not Allowed");
                            tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(Common.DEFAULT_SYSTEM_EMPNUM);
                            if (tmpEmpData.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in tmpEmpData.Rows)
                                {
                                    lvr["empnum"] = dr1["empnum"];
                                    lvr["empname"] = dr1["empname"];
                                    lvr["statuscode"] = dr1["statuscode"];
                                    lvr["dateconfirmed"] = GetConfirmationDate(dr1["empnum"].ToString());
                                    lvr["matnum"] = dr1["matnum"];
                                    lvr["sitenum"] = dr1["sitenum"];
                                    if (BizFunctions.IsEmpty(lvr["trandate"]))
                                    {
                                        lvr["trandate"] = DateTime.Now;
                                    }                       
                                }
                            }
                        }
                        else if(lvr["empnum"].ToString().Trim() != Common.DEFAULT_SYSTEM_EMPNUM && isAllowedToChangeEmpnum(Common.DEFAULT_SYSTEM_USERNAME))
                        {
                            tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(lvr_empnum.Text);
                            if (tmpEmpData.Rows.Count > 0)
                            {
                                    foreach (DataRow dr1 in tmpEmpData.Rows)
                                    {
                                        lvr["empnum"] = dr1["empnum"];
                                        lvr["empname"] = dr1["empname"];
                                        lvr["statuscode"] = dr1["statuscode"];
                                        lvr["dateconfirmed"] = GetConfirmationDate(dr1["empnum"].ToString());
                                        lvr["matnum"] = dr1["matnum"];
                                        lvr["sitenum"] = dr1["sitenum"];
                                        if (BizFunctions.IsEmpty(lvr["trandate"]))
                                        {
                                            lvr["trandate"] = DateTime.Now;
                                        }                       
                                    }
                            }
                        }

                        else
                        {
                                if (tmpEmpData.Rows.Count > 0)
                                {
                                    foreach (DataRow dr1 in tmpEmpData.Rows)
                                    {
                                        lvr["empnum"] = dr1["empnum"];
                                        lvr["empname"] = dr1["empname"];
                                        lvr["statuscode"] = dr1["statuscode"];
                                        lvr["dateconfirmed"] = GetConfirmationDate(dr1["empnum"].ToString());
                                        lvr["matnum"] = dr1["matnum"];
                                        lvr["sitenum"] = dr1["sitenum"];
                                        if (BizFunctions.IsEmpty(lvr["trandate"]))
                                        {
                                            lvr["trandate"] = DateTime.Now;
                                        }                
                                    }
                                }
                        }
                    }
                }
                else
                {
                    if (tmpEmpData.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in tmpEmpData.Rows)
                        {
                            lvr["empnum"] = dr1["empnum"];
                            lvr["empname"] = dr1["empname"];
                            lvr["statuscode"] = dr1["statuscode"];
                            lvr["dateconfirmed"] = GetConfirmationDate(dr1["empnum"].ToString());
                            lvr["matnum"] = dr1["matnum"];
                            lvr["sitenum"] = dr1["sitenum"];
                            if (BizFunctions.IsEmpty(lvr["trandate"]))
                            {
                                lvr["trandate"] = DateTime.Now;
                            }
                        }
                    }
                }

               

                GetLeaveHistory();
                //GetLeaveSummary();
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

            if (BizFunctions.IsEmpty(lvr["lvmnum"]))
            {
                MessageBox.Show("You must choose the type of Leave", "Not Saved");
                e.Handle = false;
            }

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
                            e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                      //      e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            GetLeaveHistory();
                            if (BizFunctions.IsEmpty(lvr["trandate"]))
                            {
                                lvr["trandate"] = DateTime.Now;
                            }
                            //LeaveControl lv = new LeaveControl(lvr["empnum"].ToString(),Convert.ToDateTime(lvr["trandate"]));

                            //DataTable tmp = lv.GetLeaveTally();

                            //if (tmp.Rows.Count > 0)
                            //{
                            //    if (lvr1.Rows.Count > 0)
                            //    {
                            //        BizFunctions.DeleteAllRows(lvr1);
                            //    }
                            //    foreach (DataRow dr2 in tmp.Rows)
                            //    {
                            //        lvr1.ImportRow(dr2);
                            //    }
                            //}
                            GetLeaveHistory();
                            //GetLeaveSummary();

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

            string GetConfirmation = "select CF.empnum,CF.confirmationdate from CFRH CF " +
                                        "LEFT JOIN HEMPH H ON CF.empnum=H.empnum  " +
                                        "where CF.[status]<>'V' and CF.empnum='" + empnum + "'";

            string GetDateJoined = "SELECT datejoined from HEMPH where [status]<>'V' and empnum='" + empnum + "' ";

            this.dbaccess.ReadSQL("dtCFR", GetConfirmation);

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
            DataTable lwt = e.DBAccess.DataSet.Tables["lwt"];

            if (BizFunctions.IsEmpty(lvr["trandate"]))
            {
                lvr["trandate"] = DateTime.Now;
            }

            lvr["LeaveYear"] = Convert.ToDateTime(lvr["leavefrom"]).Year;

            //if (ApprovedBy != string.Empty)
            //{
            //    lvr["recmby"] = RecommendedBy;
            //}
            //if (RecommendedBy != string.Empty)
            //{
            //    lvr["apprdby"] = ApprovedBy;
            //}

            foreach (DataRow dr1 in lvr1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(lvr, dr1, "status/flag/user/trandate/created/modified/leaveyear");
                }
            }


            if (!BizFunctions.IsEmpty(lvr["empnum"]) && !BizFunctions.IsEmpty(lvr["trandate"]))
            {


                if (lvr["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
                {
                    #region stock costing adjustment update lwt

                    //foreach (DataRow dr in lvr1.Rows)
                    //{
                    //    if (dr.RowState != DataRowState.Deleted)
                    //    {
                    if ((decimal)lvr["totaldays"] != 0)
                    {
                        DataRow lwt_dr = lwt.Rows.Add(new object[] { });


                        lwt_dr["refnum"] = lvr["refnum"];
                        lwt_dr["lvmnum"] = lvr["lvmnum"];
                        lwt_dr["empnum"] = lvr["empnum"];
                        lwt_dr["aladjdate"] = lvr["trandate"];
                        //lwt_lvr["empname"] = lvr["empname"];
                        lwt_dr["matnum"] = lvr["matnum"];
                        //lwt_lvr["datejoined"] = lvr["datejoined"];
                        //lwt_lvr["yearsofservice"] = lvr["yearsofservice"];
                        //lwt_lvr["monthsofservice"] = lvr["monthsofservice"];
                        //lwt_lvr["albf"] = lvr["albf"];
                        //lwt_lvr["algiven"] = lvr["algiven"];
                        //lwt_lvr["alcurr"] = lvr["alcurr"];
                        //lwt_lvr["ladjust"] = lvr["ladjust"];
                        //lwt_lvr["initialentitlement"] = lvr["initialentitlement"];
                        //lwt_lvr["proposedleavcurryear"] = lvr["proposedleavcurryear"];
                        //lwt_lvr["proposedaddon"] = lvr["proposedaddon"];
                        //lwt_lvr["actualaddon"] = lvr["actualaddon"];
                        //lwt_lvr["actualbflyr"] = lvr["actualbflyr"];
                        //lwt_lvr["proposedbflyr"] = lvr["proposedbflyr"];
                        //lwt_lvr["actualbf"] = lvr["actualbf"];
                        //lwt_lvr["totalal"] = lvr["totalal"];
                        //lwt_lvr["aladjdate"] = lvr["aladjdate"];
                        //lwt_lvr["remark"] = lvr["remark"];
                        //lwt_lvr["refnum"] = lvr["refnum"];
                        //lwt_lvr["totalal"] = lvr["totalal"];
                        //lwt_lvr["adjbalqty"] = lvr["adjbalqty"];
                        lwt_dr["actualtotalal"] = Convert.ToDecimal(lvr["totaldays"])*-1;
                        lwt_dr["adjbalqty"] = Convert.ToDecimal(lvr["totaldays"]) * -1;
                        
                        //lwt_lvr["currlveqty"] = lvr["currlveqty"];

                        lwt_dr["guid"] = BizLogicTools.Tools.getGUID();
                        //    }
                        //}
                    }
                    #endregion
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
            

        

            GetLeaveHistory();
            //GetLeaveSummary();

            if (lvr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (lvr["flag"].ToString().Trim().ToUpper() == "LVRH")
                {
                    if (MessageBox.Show("Do you Want to Send an Email Notification?", "Notification", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        string ApproveStatus = "";
                        string EmpName = "";
                        string Body = "";
                        string itemList = "";

                        DataTable EmpDataTb = BizLogicTools.Tools.GetCommonEmpData(lvr["empnum"].ToString());

                        if (EmpDataTb.Rows.Count > 0)
                        {
                            EmpName = EmpDataTb.Rows[0]["empname"].ToString();
                        }


                        if (BizFunctions.IsEmpty(lvr["isapproved"]))
                        {
                            lvr["isapproved"] = 0;
                        }

                        if ((bool)lvr["isapproved"])
                        {
                            ApproveStatus = "Approved";
                        }
                        else
                        {
                            ApproveStatus = "Rejected";
                        }




                        Body = "Dear " + EmpName + ",<br><br> " +

                                "Your leave application which was submitted on (" + Convert.ToDateTime(lvr["trandate"]).ToShortDateString() + ") has been " + ApproveStatus + "  <br/><br/> " +

                                "Date From: " + Convert.ToDateTime(lvr["leavefrom"]).ToShortDateString() + " To " + Convert.ToDateTime(lvr["leaveto"]).ToShortDateString() + " <br/><br/> " +

                                "Leave Type - " + lvr["lvmnum"].ToString() + "<br/> <br/> " +

                                "No. of Days - " + lvr["totaldays"].ToString() + "<br/> <br/>  " +

                                "Thanks & Best Regards. <br/> <br/><br/> " +
                                "" + Common.DEFAULT_SYSTEM_USERNAME + "";

                        string Subject = lvr["refnum"].ToString().Trim() + " - " + ApproveStatus;
                        BizLogicTools.Tools.SendEmailByModule(this.dbaccess, Subject, Body, "LEAVEHQ", lvr["empnum"].ToString());


                    }
                }
            }
       }


        

        # endregion


       


        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];


            if (lvr["flag"].ToString().ToUpper().Trim() == "LVRH")
            {
                if (BizFunctions.IsEmpty(lvr["empnum"]))
                {
                    lvr["empnum"] = Common.DEFAULT_SYSTEM_EMPNUM;
                }
            }
            

            GetLeaveHistory();

            if (!BizFunctions.IsEmpty(lvr["empnum"]))
            {
                lvr["dateconfirmed"] = GetConfirmationDate(lvr["empnum"].ToString());
                GetStatus();
            }

            if (!BizFunctions.IsEmpty(lvr["empnum"]) && !BizFunctions.IsEmpty(lvr["trandate"]))
            {
                //LeaveControl lv = new LeaveControl(lvr["empnum"].ToString(), Convert.ToDateTime(lvr["trandate"]));

                //DataTable tmp = lv.GetLeaveTally();

                //if (tmp.Rows.Count > 0)
                //{
                //    if (lvr1.Rows.Count > 0)
                //    {
                //        BizFunctions.DeleteAllRows(lvr1);
                //    }
                //    foreach (DataRow dr2 in tmp.Rows)
                //    {
                //        lvr1.ImportRow(dr2);
                //    }
                //}
            }

            GetLeaveHistory();
            //GetLeaveSummary();


            if (BizFunctions.IsEmpty(lvr["apprdby"]))
            {
                lvr["apprdby"] = GetSupervisor(lvr["empnum"].ToString());
            }

        }
        #endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);

            DataRow lvr = this.dbaccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbaccess.DataSet.Tables["lvr1"];


            if (lvr["flag"].ToString().Trim().ToUpper() == "LVRH")
            {
                if (GetSupervisor(lvr["empnum"].ToString()) != Common.DEFAULT_SYSTEM_USERNAME)
                {
                    MessageBox.Show("You can't confirm this Voucher, only the Employee's immediate is allowed", "Not Allowed");
                    e.Handle = false;
                }
            }


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

        private void Voucher_LVR_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            string supv = "";
            switch (e.Column.ColumnName)
            {
                case "empnum":
                    #region Steph - Pull info from ARM
                    dbaccess.ReadSQL("getArmInfo", "SELECT * FROM hemph where refnum ='" + e.Row["empnum"].ToString().Trim() + "'");

                    if (dbaccess.DataSet.Tables["getArmInfo"].Rows.Count > 0)
                    {
                        DataRow getArmInfo = dbaccess.DataSet.Tables["getArmInfo"].Rows[0];
                        supv = getArmInfo["supv"].ToString();
                        e.Row["sectorcode"] = getArmInfo["email"];

                        dbaccess.ReadSQL("getArmInfo1", "SELECT * FROM hemph where refnum ='" + supv + "'");

                        if (dbaccess.DataSet.Tables["getArmInfo1"].Rows.Count > 0)
                        {
                            DataRow getArmInfo1 = dbaccess.DataSet.Tables["getArmInfo1"].Rows[0];
                            e.Row["sendto"] = getArmInfo1["email"];
                        }
                    }
                    else
                    {
                        e.Row["sendto"] = "";
                    }
                    break;
                    #endregion
            }
        }

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
                string str1 = "SELECT Lv.refnum as Refnum, Lv.LeaveYear as [Year], Lv.leavefrom as [From] ,Lv.leaveto as [To],Lm.lvmnum as [Desc],Lv.totaldays as Days,Lv.apprdby as ApprovedBy " +
                                "FROM LVR Lv left join LVM Lm on Lv.lvmnum=Lm.lvmnum " +
                                "where empnum='" + lvr["empnum"].ToString() + "' and lv.[status]<>'V' AND isapproved=1 AND Lv.totaldays>0" +
                                "order by lv.leavefrom";

                this.dbaccess.ReadSQL("dtLeaveHistory", str1);
                if (this.dbaccess.DataSet.Tables["dtLeaveHistory"].Rows.Count > 0)
                {
                    overallleavesummary.DataSource = this.dbaccess.DataSet.Tables["dtLeaveHistory"];

                }

                this.dbaccess.DataSet.Tables["dtLeaveHistory"].Dispose();
            }

            //GetLeaveSummary();
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

            DateTime dt1 = Convert.ToDateTime(lvr["leavefrom"]);
            DateTime dt2 = dt1.AddYears(-1);

            int CurrYear = dt1.Year;
            int PrevYear = dt2.Year;

            decimal LeaveBalRemain = 0;

            if (!BizFunctions.IsEmpty(lvr["empnum"]) && !BizFunctions.IsEmpty(lvr["leavefrom"]))
            {
                try
                {
                    Parameter[] parameters = new Parameter[2];
                    parameters[0] = new Parameter("@Empnum", lvr["empnum"].ToString());
                    parameters[1] = new Parameter("@Date", BizFunctions.GetSafeDateString(Convert.ToDateTime(lvr["leavefrom"])));
                    DataSet ds_LvBal = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_GetLeaveBal", ref parameters);
                    ds_LvBal.Tables[0].TableName = "LeaveBalTb";

                    if (ds_LvBal != null)
                    {
                        DataTable lvBal = ds_LvBal.Tables[0];
                        if (lvBal.Rows.Count > 0)
                        {

                            for (int i = 0; i <= lvBal.Rows.Count - 1; i++)
                            {
                                if (lvBal.Rows[i].RowState != DataRowState.Deleted)
                                {
                                    if ((i + 1) <= lvBal.Rows.Count - 1)
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


                            decimal test1 = Convert.ToDecimal(lvBal.Rows[maxrow]["LveEntitlement"]) / Convert.ToDecimal(12.00);
                            decimal test2 = Convert.ToDateTime(lvr["leavefrom"]).Month;
                            decimal test3 = Convert.ToDecimal(lvBal.Rows[maxrow]["ALBFprevYr"]);

                            decimal test4 = (test1 * test2) + test3;


                            lvBal.Rows[maxrow]["AL_ENT_CURR_MTH"] = Math.Round((((Convert.ToDecimal(lvBal.Rows[maxrow]["LveEntitlement"]) / 12) * Convert.ToDateTime(lvr["leavefrom"]).Month) + Convert.ToDecimal(lvBal.Rows[maxrow]["ALBFprevYr"])), 0, MidpointRounding.AwayFromZero);
                            lvBal.Rows[maxrow]["AL_BAL_CURR_MTH"] = Convert.ToDecimal(lvBal.Rows[maxrow]["AL_ENT_CURR_MTH"]) - Convert.ToDecimal(lvBal.Rows[maxrow]["ALutil"]);


                            lvBal.Rows[maxrow]["ALbal"] = Math.Round(Convert.ToDecimal(lvBal.Rows[maxrow]["ALbal"]), 0, MidpointRounding.AwayFromZero);
                            lvBal.Rows[maxrow]["ALBFprevYr"] = Math.Round(Convert.ToDecimal(lvBal.Rows[maxrow]["ALBFprevYr"]), 0, MidpointRounding.AwayFromZero);

                            //string GetAdjustedDays = "select sum(adjdays) as adjdays from LVR where empnum='"+lvr["empnum"].ToString()+"' and LeaveYear="+lvr["leaveyear"].ToString()+" and [status]<>'V'";

                            //this.dbaccess.ReadSQL("GetAdjustedDaysTB", GetAdjustedDays);

                            //DataTable GetAdjustedDaysTB = this.dbaccess.DataSet.Tables["GetAdjustedDaysTB"];

                            //if (GetAdjustedDaysTB.Rows.Count > 0)
                            //{
                            //    DataRow drGGetAdjustedDays = GetAdjustedDaysTB
                            //}
                        }
                    }

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
                                             "FROM LeaveBalTb " +
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


        void lv_click(object sender, EventArgs e)  // Send Email
        {
            //DataRow lvr = dbaccess.DataSet.Tables["lvr"].Rows[0];

            //Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            //Outlook._MailItem oMailItem = (Outlook._MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
            //oMailItem.Subject = "Leave Application";
            //oMailItem.Body = "Dear \n\nPlease look through my application for Reference No. " + lvr["refnum"].ToString() + "";
            //int iPosition = (int)oMailItem.Body.Length + 1;



            //if (lvr["sendto"].ToString() != "")
            //{
            //    oMailItem.To = lvr["sendto"].ToString();
            //    oMailItem.Display(false);
            //    oMailItem.Send();
            //}
            //else
            //{
            //    MessageBox.Show("Please check Email address is not vaild!");

            //}
        }


        private string GetSupervisor(string empnum)
        {
            string SupervisorUserName = "";

            string sql1 = "select nric,empnum,empname,supv,[dbo].[GetEmpUsername](supv) as SupUserName from hemph where empnum='" + empnum + "' ";

            this.dbaccess.ReadSQL("TmpSupUserName", sql1);

            DataTable dt1 = this.dbaccess.DataSet.Tables["TmpSupUserName"];

            if (dt1.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(dt1.Rows[0]["SupUserName"]))
                {
                    SupervisorUserName = dt1.Rows[0]["SupUserName"].ToString();
                }
            }

            return SupervisorUserName;
        }


        private bool isAllowedToChangeEmpnum(string username)
        {
            bool isAllowed = false;

            string sql1 = "Select * from SysUserGroup where UserName='"+username+"'";

            this.dbaccess.ReadSQL("TempGroupInfo", sql1);

            DataTable dt1 = this.dbaccess.DataSet.Tables["TempGroupInfo"];

            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in dt1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (dr1["GroupName"].ToString().Trim().ToUpper() == "ADMINISTRATOR" || dr1["GroupName"].ToString().Trim().ToUpper() == "PAYROLL")
                        {
                            isAllowed = true;
                        }
                    }
                }
            }

            return isAllowed;
        }

        //protected void SizeColumns(DataGrid grid)
        //{
        //    Graphics g = CreateGraphics();

        //    DataTable dataTable = (DataTable)grid.DataSource;

        //    DataGridTableStyle dataGridTableStyle = new DataGridTableStyle();

        //    dataGridTableStyle.MappingName = dataTable.TableName;

        //    foreach (DataColumn dataColumn in dataTable.Columns)
        //    {
        //        int maxSize = 0;

        //        SizeF size = g.MeasureString(
        //                        dataColumn.ColumnName,
        //                        grid.Font
        //                     );

        //        if (size.Width > maxSize)
        //            maxSize = (int)size.Width;

        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            size = g.MeasureString(
        //                      row[dataColumn.ColumnName].ToString(),
        //                      grid.Font
        //                );

        //            if (size.Width > maxSize)
        //                maxSize = (int)size.Width;
        //        }

        //        DataGridColumnStyle dataGridColumnStyle = new DataGridTextBoxColumn();
        //        dataGridColumnStyle.MappingName = dataColumn.ColumnName;
        //        dataGridColumnStyle.HeaderText = dataColumn.ColumnName;
        //        dataGridColumnStyle.Width = maxSize + 5;
        //        dataGridTableStyle.GridColumnStyles.Add(dataGridColumnStyle);
        //    }
        //    grid.TableStyles.Add(dataGridTableStyle);

        //    g.Dispose();
        //}
    }
}


