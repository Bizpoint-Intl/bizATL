#region Namespaces
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;

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

using ATL.BizLogicTools;
using ATL.TimeUtilites;
using NodaTime;
using ATL.SortTable;
using ATL.Schedule;

#endregion

namespace ATL.WRR
{
    public class Voucher_WRR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName, wrr1FormName, wrr2FormName, wrr3FormName, wrr4FormName, atrFormName = null;
        protected DateTimePicker wrr_commencedate, wrr_enddate = null;
        //TextBox wrr1_dayshiftcode, wrr2_nightshiftcode, wrr3_concierge = null;
        Button BtnSummary = null;
        protected Button btn_Voucher_Reports = null;
        bool columnChanged, isSummarized,scheduleChanged;
        protected Button Btn_Sort = null;
        protected bool reOpen,confirmed,voidClicked = false;
        protected ComboBox TableColumn = null;
        DataTable vSHLV = null;
        DataTable AllowedDAys = null;
        string GetvSHLV = "Select * from vSHLV";
        Schedule.ScheduleControl sc = null;
        protected string SectorCode="";
        protected bool opened = false;



        protected Label lbl_wrr1Total, lbl_wrr2Total, lbl_wrr3Total = null;
        protected TextBox wrr_sitenumt,wrr1Total_monday, wrr1Total_tuesday, wrr1Total_wednesday, wrr1Total_thursday, wrr1Total_friday,
                          wrr1Total_saturday, wrr1Total_sunday, wrr2Total_monday, wrr2Total_tuesday, wrr2Total_wednesday,
                          wrr2Total_thursday, wrr2Total_friday, wrr2Total_saturday, wrr2Total_sunday, wrr3Total_monday,
                          wrr3Total_tuesday, wrr3Total_wednesday, wrr3Total_thursday, wrr3Total_friday, wrr3Total_saturday,
                          wrr3Total_sunday, wrr1Ctrh_monday, wrr1Ctrh_tuesday, wrr1Ctrh_wednesday, wrr1Ctrh_thursday,
                          wrr1Ctrh_friday, wrr1Ctrh_saturday, wrr1Ctrh_sunday, wrr2Ctrh_monday, wrr2Ctrh_tuesday,
                          wrr2Ctrh_wednesday, wrr2Ctrh_thursday, wrr2Ctrh_friday, wrr2Ctrh_saturday, wrr2Ctrh_sunday,
                          wrr3Ctrh_monday, wrr3Ctrh_tuesday, wrr3Ctrh_wednesday, wrr3Ctrh_thursday, wrr3Ctrh_friday,
                          wrr3Ctrh_saturday, wrr3Ctrh_sunday = null;


        #endregion

        #region Constructor

        public Voucher_WRR(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_WRR.xml", moduleName, voucherBaseHelpers)
        {
            //this.SectorCode = Sector;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            //string CheckSysuserInfo = "Select empnum from sysuserinfo where Username='" + Common.DEFAULT_SYSTEM_USERNAME + "'";
            //string CheckSITMD = "Select empnum from SITMD where empnum in " +
            //                    "(select sy.empnum " +
            //                               "from " +
            //                                   "SysUserInfo sy " +
            //                                   "where sy.UserName='" + Common.DEFAULT_SYSTEM_USERNAME + "'" +
            //                    ") and [status]<>'V'";


            //e.DBAccess.ReadSQL("checkSIinfo", CheckSysuserInfo);
            //e.DBAccess.ReadSQL("checkSITMD", CheckSITMD);

            //DataTable checkSIinfo = e.DBAccess.DataSet.Tables["checkSIinfo"];
            //DataTable CheckSITMDtb = e.DBAccess.DataSet.Tables["checkSITMD"];



            //if (checkSIinfo.Rows.Count > 0 && CheckSITMDtb.Rows.Count > 0)
            //{
            //    e.Condition = "SectorCode='" + SectorCode + "' and sitenum in " +
            //                  "(select s1.sitenum " +
            //                    "from sitmd1 s1 " +
            //                    "left join ( " +
            //                                "select sd.sitmdnum,sy.empnum,sd.[status] " +
            //                                "from " +
            //                                    "sitmd sd left join SysUserInfo sy on sd.empnum=sy.Empnum " +
            //                                    "where sy.UserName='" + Common.DEFAULT_SYSTEM_USERNAME + "'" +
            //                                "group  by sd.sitmdnum,sy.empnum,sd.[status]) sd " +
            //                    "on s1.sitmdnum=sd.sitmdnum " +
            //                    "where sd.[status]<>'V' " +
            //                    "group by sd.empnum,  s1.sitenum)";
            //}
            //else
            //{
            //    e.Condition = "SectorCode='" + SectorCode + "'";
            //}

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

             //string CheckSysuserInfo = "Select empnum from sysuserinfo where Username='" + Common.DEFAULT_SYSTEM_USERNAME + "'";
             //string CheckSITMD = "Select empnum from SITMD where empnum in " +
             //                    "(select sy.empnum " +
             //                               "from " +
             //                                   "SysUserInfo sy " +
             //                                   "where sy.UserName='" + Common.DEFAULT_SYSTEM_USERNAME + "'" +
             //                    ") and [status]<>'V'";


             //e.DBAccess.ReadSQL("checkSIinfo", CheckSysuserInfo);
             //e.DBAccess.ReadSQL("checkSITMD", CheckSITMD);

             //DataTable checkSIinfo = e.DBAccess.DataSet.Tables["checkSIinfo"];
             //DataTable CheckSITMDtb = e.DBAccess.DataSet.Tables["checkSITMD"];

            

             //if (checkSIinfo.Rows.Count > 0 && CheckSITMDtb.Rows.Count > 0)
             //{
             //    e.Condition = "SectorCode='" + SectorCode + "' and [status]='O' and sitenum in " +
             //                  "(select s1.sitenum " +
             //                    "from sitmd1 s1 " +
             //                    "left join ( " +
             //                                "select sd.sitmdnum,sy.empnum,sd.[status] " +
             //                                "from " +
             //                                    "sitmd sd left join SysUserInfo sy on sd.empnum=sy.Empnum " +
             //                                    "where sy.UserName='" + Common.DEFAULT_SYSTEM_USERNAME + "'" +
             //                                "group  by sd.sitmdnum,sy.empnum,sd.[status]) sd " +
             //                    "on s1.sitmdnum=sd.sitmdnum " +
             //                    "where sd.[status]<>'V' " +
             //                    "group by sd.empnum,  s1.sitenum)";
             //}
             
             //else
             //{
             //    e.Condition = "SectorCode='" + SectorCode + "' and [status]='O'";
             //}

            e.Condition = "[status]='O'";
            
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

            DataRow wrr = e.DBAccess.DataSet.Tables["WRR"].Rows[0];
            DataTable wrr1 = this.dbaccess.DataSet.Tables["WRR1"];
            DataTable wrr2 = this.dbaccess.DataSet.Tables["WRR2"];
            DataTable wrr3 = this.dbaccess.DataSet.Tables["WRR3"];
            opened = true;
            string scheduleoption = wrr["scheduleoption"].ToString();

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.wrr1FormName = (e.FormsCollection["dayshift"] as Form).Name;
            this.wrr2FormName = (e.FormsCollection["nightshift"] as Form).Name;
            this.wrr3FormName = (e.FormsCollection["concierge"] as Form).Name;
            this.wrr4FormName = (e.FormsCollection["relief"] as Form).Name;
            this.atrFormName = (e.FormsCollection["detailedschedule"] as Form).Name;


            this.dbaccess.ReadSQL("vSHLV", GetvSHLV);

            vSHLV = this.dbaccess.DataSet.Tables["vSHLV"];

            e.DBAccess.DataSet.Tables["wrr1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_WRR1_ColumnChanged);
            e.DBAccess.DataSet.Tables["wrr2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_WRR2_ColumnChanged);
            e.DBAccess.DataSet.Tables["wrr3"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_WRR3_ColumnChanged);
            e.DBAccess.DataSet.Tables["wrr4"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_WRR4_ColumnChanged);

            Initialise();

          
                Getwrr1FooterTotals();
            
                Getwrr2FooterTotals();
            

           
                Getwrr3FooterTotals();
            

            if (!BizFunctions.IsEmpty(wrr["docunum"]))
            {
                Getwrr1CtrhFooterTotals();
                Getwrr2CtrhFooterTotals();
                Getwrr3CtrhFooterTotals();
            }

            if (wrr["createdby"].ToString().Trim() == String.Empty)
            {
                wrr["createdby"] = Common.DEFAULT_SYSTEM_USERNAME;
            }

     
            confirmed = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("EnableSaveRosterConflicts").ToString());
        }

        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];

            Btn_Sort = BizXmlReader.CurrentInstance.GetControl(atrFormName, "Btn_Sort") as Button;

            Btn_Sort.Click += new EventHandler(Btn_Sort_Click);

            BtnSummary = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Summary") as Button;

            //BtnSummary.Click +=new EventHandler(BtnSummary_Click);

            TableColumn = BizXmlReader.CurrentInstance.GetControl(atrFormName, "TableColumn") as ComboBox;

            wrr_commencedate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "wrr_commencedate") as DateTimePicker;
            wrr_commencedate.Leave += new EventHandler(wrr_commencedate_Leave);

            wrr_enddate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "wrr_enddate") as DateTimePicker;
            wrr_enddate.Leave += new EventHandler(wrr_enddate_Leave);

            wrr_sitenumt = BizXmlReader.CurrentInstance.GetControl(headerFormName, "wrr_sitenumt") as TextBox;
            wrr1Total_monday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Total_monday") as TextBox;
            wrr1Total_tuesday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Total_tuesday") as TextBox;
            wrr1Total_wednesday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Total_wednesday") as TextBox;
            wrr1Total_thursday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Total_thursday") as TextBox;
            wrr1Total_friday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Total_friday") as TextBox;
            wrr1Total_saturday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Total_saturday") as TextBox;
            wrr1Total_sunday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Total_sunday") as TextBox;

            wrr2Total_monday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Total_monday") as TextBox;
            wrr2Total_tuesday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Total_tuesday") as TextBox;
            wrr2Total_wednesday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Total_wednesday") as TextBox;
            wrr2Total_thursday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Total_thursday") as TextBox;
            wrr2Total_friday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Total_friday") as TextBox;
            wrr2Total_saturday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Total_saturday") as TextBox;
            wrr2Total_sunday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Total_sunday") as TextBox;

            wrr3Total_monday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Total_monday") as TextBox;
            wrr3Total_tuesday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Total_tuesday") as TextBox;
            wrr3Total_wednesday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Total_wednesday") as TextBox;
            wrr3Total_thursday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Total_thursday") as TextBox;
            wrr3Total_friday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Total_friday") as TextBox;
            wrr3Total_saturday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Total_saturday") as TextBox;
            wrr3Total_sunday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Total_sunday") as TextBox;

            wrr1Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Ctrh_monday") as TextBox;
            wrr1Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Ctrh_tuesday") as TextBox;
            wrr1Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Ctrh_wednesday") as TextBox;
            wrr1Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Ctrh_thursday") as TextBox;
            wrr1Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Ctrh_friday") as TextBox;
            wrr1Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Ctrh_saturday") as TextBox;
            wrr1Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(wrr1FormName, "wrr1Ctrh_sunday") as TextBox;

            wrr2Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Ctrh_monday") as TextBox;
            wrr2Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Ctrh_tuesday") as TextBox;
            wrr2Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Ctrh_wednesday") as TextBox;
            wrr2Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Ctrh_thursday") as TextBox;
            wrr2Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Ctrh_friday") as TextBox;
            wrr2Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Ctrh_saturday") as TextBox;
            wrr2Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(wrr2FormName, "wrr2Ctrh_sunday") as TextBox;

            wrr3Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Ctrh_monday") as TextBox;
            wrr3Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Ctrh_tuesday") as TextBox;
            wrr3Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Ctrh_wednesday") as TextBox;
            wrr3Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Ctrh_thursday") as TextBox;
            wrr3Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Ctrh_friday") as TextBox;
            wrr3Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Ctrh_saturday") as TextBox;
            wrr3Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(wrr3FormName, "wrr3Ctrh_sunday") as TextBox;


            if (!BizFunctions.IsEmpty(wrr["refnum"]))
            {
                if (wrr["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSO || wrr["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSP)
                {
                    sc = new ScheduleControl(wrr["refnum"].ToString().Trim(), wrr["refnum"].ToString().Trim().Substring(0, 3), "WRR");
                }
            }

            if (!BizFunctions.IsEmpty(wrr["commencedate"]) && !BizFunctions.IsEmpty(wrr["enddate"]))
            {
                SetAllowedDays();
            }

        }

        #endregion

        #region  Reports Click

        private void btn_Voucher_Reports_Click(object sender, EventArgs e)
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];

            if (wrr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO || wrr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {

                ReportLists.Reports ReportForm = new ATL.ReportLists.Reports(false, "WRR", "WRR", wrr["refnum"].ToString());

                ReportForm.ShowDialog();

            }
        }

        #endregion

        #region Btn Summary(Header)
        private void BtnSummary_Click(object sender, EventArgs e)
        {

            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = this.dbaccess.DataSet.Tables["wrr1"];
            DataTable wrr1sum = this.dbaccess.DataSet.Tables["wrr1sum"];
            DataTable wrr2 = this.dbaccess.DataSet.Tables["wrr2"];
            DataTable wrr2sum = this.dbaccess.DataSet.Tables["wrr2sum"];
            DataTable wrr3 = this.dbaccess.DataSet.Tables["wrr3"];
            DataTable wrr3sum = this.dbaccess.DataSet.Tables["wrr3sum"];
            DataTable wrr4 = this.dbaccess.DataSet.Tables["wrr4"];
            DataTable wrr4sum = this.dbaccess.DataSet.Tables["wrr4sum"];
            DataTable wrrall = this.dbaccess.DataSet.Tables["wrrall"];
            DataTable atmr = this.dbaccess.DataSet.Tables["atmr"];

            if ((!BizFunctions.IsEmpty(wrr["enddate"]) && !BizFunctions.IsEmpty(wrr["commencedate"])) || (wrr["enddate"] != System.DBNull.Value && wrr["commencedate"] != System.DBNull.Value))
            {

                if (MessageBox.Show("This will Re Create the Schedules, any remarks will be cleared\nContinue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {


                    #region Delete All Summary Rows

                    if (wrr1sum.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(wrr1sum);
                    }

                    if (wrr2sum.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(wrr2sum);
                    }

                    if (wrr3sum.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(wrr3sum);
                    }

                    if (wrr4sum.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(wrr4sum);
                    }

                    if (wrrall.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(wrrall);
                    }

                    if (atmr.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(atmr);
                    }

                    #endregion


                    Summary();
                }
            }
            else
            {
                MessageBox.Show("Commence and End Date are Empty, Unable to Summarize", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }
        #endregion

        #region Set Allowed Days

        protected void SetAllowedDays()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["WRR"].Rows[0];
            if (!BizFunctions.IsEmpty(wrr["commencedate"]) && !illegalWeek())
            {
                int Count = TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_commencedate.Text)), BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_enddate.Text)));

                ArrayList DateLists = new ArrayList();


                DateTime beginDate = new DateTime();
                DateTime endDate = new DateTime();

                beginDate = Convert.ToDateTime(wrr_commencedate.Text.ToString());
                endDate = Convert.ToDateTime(wrr_enddate.Text.ToString());


                while (beginDate <= endDate)
                {
                    DateLists.Add(beginDate);
                    beginDate = beginDate.AddDays(1);
                }


                if (this.dbaccess.DataSet.Tables.Contains("AlloweSchedule"))
                {
                    this.dbaccess.DataSet.Tables["AlloweSchedule"].Dispose();
                }
                DataTable dtTable = new DataTable();
                dtTable.TableName = "AlloweSchedule";
                dtTable.Columns.Add("Date", typeof(DateTime));
                dtTable.Columns.Add("Day", typeof(string));
                dtTable.Columns.Add("sequence", typeof(int));

                for (int i = 0; i <= DateLists.Count - 1; i++)
                {
                    DataRow InsertdtTable = dtTable.NewRow();
                    InsertdtTable["Date"] = DateLists[i].ToString();
                    InsertdtTable["Day"] = TimeTools.GetDay(TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString()))));
                    InsertdtTable["sequence"] = TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString())));
                    dtTable.Rows.Add(InsertdtTable);
                }


                AllowedDAys = dtTable.Copy();
                if (this.dbaccess.DataSet.Tables.Contains("AlloweSchedule"))
                {
                    this.dbaccess.DataSet.Tables.Remove("AlloweSchedule");
                    //this.dbaccess.DataSet.Tables["AlloweSchedule"].Dispose();
                }
                else
                {
                    AllowedDAys.TableName = "AlloweSchedule";
                    this.dbaccess.DataSet.Tables.Add(AllowedDAys);
                }

            }
        }

        #endregion

        #region Enddate TextBox Leave Event

        protected void wrr_enddate_Leave(object sender, System.EventArgs e)
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["WRR"].Rows[0];
            if (!BizFunctions.IsEmpty(wrr["commencedate"]) && !illegalWeek())
            {
                int Count = TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_commencedate.Text)), BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_enddate.Text)));
                //string GetDates = "Select dateadd(day," + Count.ToString() + ",commencedate) as [Days],'' as sequence from wrr";

                //DataTable TmpWrr = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetDates);
                //if (TmpWrr.Rows.Count > 0)
                //{
                //    foreach (DataRow dr1 in TmpWrr.Rows)
                //    {
                //        dr1["sequence"] = TimeTools.GetDayOfWeekNo(dr1["Days"].ToString());
                //    }
                //}

                ////////////////////////////////////////////////////

                //List<DateTime[]> weeks = new List<DateTime[]>();

                ArrayList DateLists = new ArrayList();

                //DateTime beginDate = new Convert.ToDateTime(wrr_commencedate.Text);
                //DateTime endDate = new Convert.ToDateTime(wrr_enddate.Text);

                DateTime beginDate = new DateTime();
                DateTime endDate = new DateTime();

                beginDate = Convert.ToDateTime(wrr_commencedate.Text.ToString());
                endDate = Convert.ToDateTime(wrr_enddate.Text.ToString());


                while (beginDate <= endDate)
                {
                    DateLists.Add(beginDate);
                    beginDate = beginDate.AddDays(1);
                }

                //for (int x = 0; x < weeks.Count; x++)
                //{
                //    Console.WriteLine(weeks[x][0].Date.ToShortDateString() + " - " + weeks[x][1].Date.ToShortDateString());
                //} 


                if (this.dbaccess.DataSet.Tables.Contains("AlloweSchedule"))
                {
                    this.dbaccess.DataSet.Tables["AlloweSchedule"].Dispose();
                }
                DataTable dtTable = new DataTable();
                dtTable.TableName = "AlloweSchedule";
                dtTable.Columns.Add("Date", typeof(DateTime));
                dtTable.Columns.Add("Day", typeof(string));
                dtTable.Columns.Add("sequence", typeof(int));

                for (int i = 0; i <= DateLists.Count - 1; i++)
                {
                    DataRow InsertdtTable = dtTable.NewRow();
                    InsertdtTable["Date"] = DateLists[i].ToString();
                    InsertdtTable["Day"] = TimeTools.GetDay(TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString()))));
                    InsertdtTable["sequence"] = TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString())));
                    dtTable.Rows.Add(InsertdtTable);
                }


                AllowedDAys = dtTable.Copy();
                if (this.dbaccess.DataSet.Tables.Contains("AllowedDAys"))
                {
                    this.dbaccess.DataSet.Tables["AllowedDAys"].Dispose();
                }
                else
                {
                    AllowedDAys.TableName = "AllowedDAys";
                    this.dbaccess.DataSet.Tables.Add(AllowedDAys);
                }



            }
        }

        #endregion

        #region Commencedate TextBox Leave Event

        protected void wrr_commencedate_Leave(object sender, System.EventArgs e)
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["WRR"].Rows[0];
            if (!BizFunctions.IsEmpty(wrr["commencedate"]) && !illegalWeek())
            {
                int Count = TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_commencedate.Text)), BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_enddate.Text)));
                //string GetDates = "Select dateadd(day," + Count.ToString() + ",commencedate) as [Days],'' as sequence from wrr";

                //DataTable TmpWrr = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetDates);
                //if (TmpWrr.Rows.Count > 0)
                //{
                //    foreach (DataRow dr1 in TmpWrr.Rows)
                //    {
                //        dr1["sequence"] = TimeTools.GetDayOfWeekNo(dr1["Days"].ToString());
                //    }
                //}

                ////////////////////////////////////////////////////

                //List<DateTime[]> weeks = new List<DateTime[]>();

                ArrayList DateLists = new ArrayList();

                //DateTime beginDate = new Convert.ToDateTime(wrr_commencedate.Text);
                //DateTime endDate = new Convert.ToDateTime(wrr_enddate.Text);

                DateTime beginDate = new DateTime();
                DateTime endDate = new DateTime();

                beginDate = Convert.ToDateTime(wrr_commencedate.Text.ToString());
                endDate = Convert.ToDateTime(wrr_enddate.Text.ToString());


                while (beginDate <= endDate)
                {
                    DateLists.Add(beginDate);
                    beginDate = beginDate.AddDays(1);
                }

                //for (int x = 0; x < weeks.Count; x++)
                //{
                //    Console.WriteLine(weeks[x][0].Date.ToShortDateString() + " - " + weeks[x][1].Date.ToShortDateString());
                //} 


                if (this.dbaccess.DataSet.Tables.Contains("AlloweSchedule"))
                {
                    this.dbaccess.DataSet.Tables["AlloweSchedule"].Dispose();
                }
                DataTable dtTable = new DataTable();
                dtTable.TableName = "AlloweSchedule";
                dtTable.Columns.Add("Date", typeof(DateTime));
                dtTable.Columns.Add("Day", typeof(string));
                dtTable.Columns.Add("sequence", typeof(int));

                for (int i = 0; i <= DateLists.Count - 1; i++)
                {
                    DataRow InsertdtTable = dtTable.NewRow();
                    InsertdtTable["Date"] = DateLists[i].ToString();
                    InsertdtTable["Day"] = TimeTools.GetDay(TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString()))));
                    InsertdtTable["sequence"] = TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(DateLists[i].ToString())));
                    dtTable.Rows.Add(InsertdtTable);
                }


                AllowedDAys = dtTable.Copy();
                if (this.dbaccess.DataSet.Tables.Contains("AllowedDAys"))
                {
                    this.dbaccess.DataSet.Tables["AllowedDAys"].Dispose();
                }
                else
                {
                    AllowedDAys.TableName = "AllowedDAys";
                    this.dbaccess.DataSet.Tables.Add(AllowedDAys);
                }

            }
        }

        #endregion

        #region Clear Days in Header Summary

        private void ClearDays()
        {
            DataTable wrr1 = this.dbaccess.DataSet.Tables["WRR1"];
            DataTable wrr2 = this.dbaccess.DataSet.Tables["WRR2"];
            DataTable wrr3 = this.dbaccess.DataSet.Tables["WRR3"];

            SortedDictionary<string, object> WeekDays = new SortedDictionary<string, object>();
            SortedDictionary<string, object> WeekDaysAvailable = new SortedDictionary<string, object>();

            WeekDays.Add("Monday", null);
            WeekDays.Add("Tuesday", null);
            WeekDays.Add("Wednesday", null);
            WeekDays.Add("Thursday", null);
            WeekDays.Add("Friday", null);
            WeekDays.Add("Saturday", null);
            WeekDays.Add("Sunday", null);

            foreach (DataRow dr1 in AllowedDAys.Rows)
            {
                WeekDaysAvailable.Add(dr1["Day"].ToString(), null);
            }

            DataTable DeleteColumns = new DataTable();

            DeleteColumns = GeUnavailableDay(WeekDays, WeekDaysAvailable).Copy();

            foreach (DataRow dr1 in DeleteColumns.Rows)
            {

                foreach (DataRow dr2 in wrr1.Rows)
                {

                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        dr2[dr1["Day"].ToString()] = string.Empty;
                    }
                }

                foreach (DataRow dr3 in wrr2.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        dr3[dr1["Day"].ToString()] = string.Empty;
                    }
                }

                foreach (DataRow dr4 in wrr3.Rows)
                {
                    if (dr4.RowState != DataRowState.Deleted)
                    {
                        dr4[dr1["Day"].ToString()] = string.Empty;
                    }
                }
            }

        }

        #endregion

        #region Get Unavailable Day

        static DataTable GeUnavailableDay(SortedDictionary<string, object> list1, SortedDictionary<string, object> list2)
        {
            DataTable tmpTable = new DataTable();

            tmpTable.Columns.Add("Day", typeof(string));

            foreach (KeyValuePair<string, object> item in list1)
            {
                if (!list2.ContainsKey(item.Key))
                {
                    DataRow InsertTb = tmpTable.NewRow();

                    InsertTb["Day"] = item.Key.ToString();
                    tmpTable.Rows.Add(InsertTb);
                }
            }

            // Must be the same
            return tmpTable;
        }

        #endregion

        #region Is Allowed Day

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

        #region  btn_Sort_Click
        protected void Btn_Sort_Click(object sender, System.EventArgs e)
        {
            DataTable atmr = this.dbaccess.DataSet.Tables["atmr"];
            TableColumn = (ComboBox)BizXmlReader.CurrentInstance.GetControl(atrFormName, "TableColumn");


            string cname = TableColumn.Text.ToString();
            if (cname != "")
            {
                if (atmr.Rows.Count > 0)
                {
                    SortDT sort = new SortDT(atmr, cname + " ASC");
                    DataTable returnedfinalextraction = sort.SortedTable();

                    BizFunctions.DeleteAllRows(atmr);

                    foreach (DataRow dr in returnedfinalextraction.Select())
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            atmr.ImportRow(dr);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Choose Column To Sort !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

        }
        #endregion

        #region Check Illegal Week

        protected bool illegalWeek()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            bool checkWeek = false;
            if (!BizFunctions.IsEmpty(wrr["commencedate"]) && !BizFunctions.IsEmpty(wrr["enddate"]))
            {
                DateTime dt1 = (DateTime)wrr["commencedate"];
                DateTime dt2 = (DateTime)wrr["enddate"];
                int noOfdays = TimeTools.daysTaken(BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_commencedate.Text)), BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr_enddate.Text)));
                if (noOfdays > 6)
                {
                    checkWeek = true;
                }
            }
            return checkWeek;
        }

        #endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

        }
        #endregion

        #region Paste Handle

        protected override void Document_Paste_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Paste_Handle(sender, e);
        }

        #endregion 

        #region Paste Onclick

        protected override void Document_Paste_OnClick(object sender, BizRAD.BizDocument.DocumentEventArgs e)
        {
            base.Document_Paste_OnClick(sender, e);
            DataRow wrr = e.DBAccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = e.DBAccess.DataSet.Tables["wrr1"];
            DataTable wrr2 = e.DBAccess.DataSet.Tables["wrr2"];
            DataTable wrr3 = e.DBAccess.DataSet.Tables["wrr3"];
            DataTable wrr4 = e.DBAccess.DataSet.Tables["wrr4"];
            DataTable wrr5 = e.DBAccess.DataSet.Tables["wrr5"];
            DataTable wrr6 = e.DBAccess.DataSet.Tables["wrr6"];
            
            
         
                    wrr["refnum"] = System.DBNull.Value;
                    wrr["createdby"] = System.DBNull.Value;
                    wrr["issuedby"] = System.DBNull.Value;

                    if (wrr1.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in wrr1.Rows)
                        {
                            if (dr1.RowState != DataRowState.Deleted)
                            {
                                dr1["refnum"] = System.DBNull.Value;
                            }
                        }
                    }

                    if (wrr2.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in wrr2.Rows)
                        {
                            if (dr1.RowState != DataRowState.Deleted)
                            {
                                dr1["refnum"] = System.DBNull.Value;
                            }
                        }
                    }

                    if (wrr3.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in wrr3.Rows)
                        {
                            if (dr1.RowState != DataRowState.Deleted)
                            {
                                dr1["refnum"] = System.DBNull.Value;
                            }
                        }
                    }

                    if (wrr4.Rows.Count > 0)
                    {
                        foreach (DataRow dr4 in wrr4.Rows)
                        {
                            if (dr4.RowState != DataRowState.Deleted)
                            {
                                dr4["refnum"] = System.DBNull.Value;
                            }
                        }
                    }



                }

        #endregion

        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow wrr = e.DBAccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = e.DBAccess.DataSet.Tables["wrr1"];
            DataTable wrr2 = e.DBAccess.DataSet.Tables["wrr2"];
            DataTable wrr3 = e.DBAccess.DataSet.Tables["wrr3"];
            DataTable wrr4 = e.DBAccess.DataSet.Tables["wrr4"];
            DataTable wrr5 = e.DBAccess.DataSet.Tables["wrr5"];
            DataTable wrr6 = e.DBAccess.DataSet.Tables["wrr6"];
            DataTable vMainHEMPH = e.DBAccess.DataSet.Tables["vMainHEMPH"];

            wrr["user"] = Common.DEFAULT_SYSTEM_USERNAME;

            if (!this.dbaccess.DataSet.Tables.Contains("vMainHEMPH"))
            {
                string Get = "Select nric,empnum,empname,matnum,sitenum,sectorcode from vMainHEMPH where [status]<>'V'";
                this.dbaccess.ReadSQL("HEMPH",Get);
            }
            else if (vMainHEMPH.Rows.Count <= 0)
            {
                string Get = "Select * from vMainHEMPH where [status]<>'V'";
                this.dbaccess.ReadSQL("HEMPHtmp", Get);
                if (this.dbaccess.DataSet.Tables["HEMPHtmp"].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["HEMPHtmp"].Rows)
                    {
                        vMainHEMPH.ImportRow(dr1);
                    }
                }
                this.dbaccess.DataSet.Tables["HEMPHtmp"].Dispose();
            }


            if (e.Handle && BizFunctions.IsEmpty(wrr["enddate"]) || BizFunctions.IsEmpty(wrr["commencedate"]) || wrr["enddate"] == System.DBNull.Value || wrr["commencedate"] == System.DBNull.Value)
            {
                MessageBox.Show("Commence and End Date are Empty, Unable to proceed", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handle = false;
            }

            if (e.Handle)
            {
                if (BizFunctions.IsEmpty(wrr["adhocrefnum"]) || wrr["adhocrefnum"].ToString() == string.Empty)
                {
                    string GetDuplicateSitMI = "Select sitenum,refnum from WRR where sitenum='" + wrr["sitenum"].ToString() + "' and [commencedate]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(wrr["commencedate"])) + "' and [status]<>'V' and refnum<>'" + wrr["refnum"].ToString() + "'";

                    this.dbaccess.ReadSQL("TmpDuplicateSched", GetDuplicateSitMI);

                    DataTable TmpDuplicateSched = this.dbaccess.DataSet.Tables["TmpDuplicateSched"];

                    if (!voidClicked || !reOpen)
                    {

                        if (TmpDuplicateSched.Rows.Count > 0)
                        {
                            MessageBox.Show("This Site already has an Existing Schedule from \n " + TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(wrr["commencedate"])) + " to " + TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(wrr["commencedate"]).AddDays(6)) + " - " + TmpDuplicateSched.Rows[0]["refnum"].ToString() + " \n Save Unsuccessful  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;

                        }
                    }
                }



            }

            if (e.Handle && illegalWeek())
            {
                MessageBox.Show("Weekly Schedule more than 7 days", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                wrr_commencedate.Text = DateTime.Today.ToString();
                wrr_enddate.Text = DateTime.Today.ToString();
                e.Handle = false;
            }


            if (e.Handle && wrr1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in wrr1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["empnum"]))
                        {
                            
                            MessageBox.Show("One of the rows in Day Shift has an empty Empno, Unable to Save", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                        }

                        if (!BizFunctions.IsEmpty(dr1["empnum"]))
                        {
                            dr1["empname"] = GetEmpname(dr1["empnum"].ToString());
                            dr1["matnum"] = GetMatnum(dr1["empnum"].ToString());
                        }
                        if (!e.Handle)
                        {
                            break;
                        }
                        dr1["isRealSchedule"] = 1;
                    }
                }
            }

            if (e.Handle && wrr2.Rows.Count > 0)
            {
                foreach (DataRow dr1 in wrr2.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["empnum"]))
                        {

                            MessageBox.Show("One of the rows in Night Shift has an empty Empno, Unable to Save", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                        }

                        if (!BizFunctions.IsEmpty(dr1["empnum"]))
                        {
                            dr1["empname"] = GetEmpname(dr1["empnum"].ToString());
                            dr1["matnum"] = GetMatnum(dr1["empnum"].ToString());
                        }
                        if (!e.Handle)
                        {
                            break;
                        }
                        dr1["isRealSchedule"] = 1;
                    }
                }
            }

            if (e.Handle && wrr3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in wrr3.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["empnum"]))
                        {

                            MessageBox.Show("One of the rows in Concierge Shift has an empty Empno, Unable to Save", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                        }

                        if (!BizFunctions.IsEmpty(dr1["empnum"]))
                        {                           
                            dr1["empname"] = GetEmpname(dr1["empnum"].ToString());
                            dr1["matnum"] = GetMatnum(dr1["empnum"].ToString());
                        }

                        if (!e.Handle)
                        {
                            break;
                        }

                        dr1["isRealSchedule"] = 1;
                    }
                }
            }

            if (e.Handle && wrr4.Rows.Count > 0)
            {
                foreach (DataRow dr4 in wrr4.Rows)
                {
                    if (dr4.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr4["empnum"]))
                        {

                            MessageBox.Show("One of the rows in Relief has an empty Empno, Unable to Save", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                        }

                        if (!BizFunctions.IsEmpty(dr4["empnum"]))
                        {                          
                            dr4["empname"] = GetEmpname(dr4["empnum"].ToString());
                            dr4["matnum"] = GetMatnum(dr4["empnum"].ToString());
                        }
                        if (!e.Handle)
                        {
                            break;
                        }
                        dr4["isRealSchedule"] = 0;
                    }
                }
            }
           

            if (e.Handle)
            {
                ClearDays(); //Later
            }

           
                Getwrr1FooterTotals();
            
            
                Getwrr2FooterTotals();
            

           
                Getwrr3FooterTotals();
            

            if (!BizFunctions.IsEmpty(wrr["docunum"]))
            {
                Getwrr1CtrhFooterTotals();
                Getwrr2CtrhFooterTotals();
                Getwrr3CtrhFooterTotals();
            }

            if (!voidClicked || !reOpen)
            {

                if (e.Handle && !isDayBalanced())
                {
                    if (MessageBox.Show("Day Shift is not Tally with the Contract\nContinue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        e.Handle = false;
                    }
                }

                //if (e.Handle && !isNightBalanced())
                //{
                //    if (MessageBox.Show("Night Shift is not Tally with the Contract\nContinue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                //    {
                //        e.Handle = false;
                //    }
                //}

                //if (e.Handle && !isConciergeBalanced())
                //{
                //    if (MessageBox.Show("Concierge Shift is not Tally with the Contract\nContinue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                //    {
                //        e.Handle = false;
                //    }
                //}


                string[] week ={ "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };



                for (int i = 0; i < week.Length - 1; i++)
                {
                    if (e.Handle && !isDuplicateInShift("wrr1", "wrr2", "wrr3", "empnum", week[i].ToString()))
                    {
                        e.Handle = false;
                    }
                }
                for (int i = 0; i < week.Length - 1; i++)
                {
                    if (e.Handle && !isDuplicateInShift("wrr2", "wrr1", "wrr3", "empnum", week[i].ToString()))
                    {
                        e.Handle = false;
                    }
                }

                for (int i = 0; i < week.Length - 1; i++)
                {
                    if (e.Handle && !isDuplicateInShift("wrr3", "wrr2", "wrr1", "empnum", week[i].ToString()))
                    {
                        e.Handle = false;
                    }

                }
            }

            if (!BizFunctions.IsEmpty(wrr["sitenum"]))
            {
                wrr["sectorcode"] = Tools.GetSectorCode(wrr["sitenum"].ToString(), this.dbaccess);

            }
        }

        #endregion     

        #region Check if Each shift is Balanced

        private bool isDayBalanced()
        {
            bool correct = true;
            if (wrr1Total_monday.Text != wrr1Ctrh_monday.Text || wrr1Total_tuesday.Text != wrr1Ctrh_tuesday.Text
                || wrr1Total_wednesday.Text != wrr1Ctrh_wednesday.Text || wrr1Total_thursday.Text != wrr1Ctrh_thursday.Text
                || wrr1Total_friday.Text != wrr1Ctrh_friday.Text || wrr1Total_saturday.Text != wrr1Ctrh_saturday.Text
                || wrr1Total_sunday.Text != wrr1Ctrh_sunday.Text)
            {
                correct = false;
            }

            return correct;
        }

        private bool isNightBalanced()
        {
            bool correct = true;
            if (wrr2Total_monday.Text != wrr2Ctrh_monday.Text || wrr2Total_tuesday.Text != wrr2Ctrh_tuesday.Text
                || wrr2Total_wednesday.Text != wrr2Ctrh_wednesday.Text || wrr2Total_thursday.Text != wrr2Ctrh_thursday.Text
                || wrr2Total_friday.Text != wrr2Ctrh_friday.Text || wrr2Total_saturday.Text != wrr2Ctrh_saturday.Text
                || wrr2Total_sunday.Text != wrr2Ctrh_sunday.Text)
            {
                correct = false;
            }

            return correct;
        }

        private bool isConciergeBalanced()
        {
            bool correct = true;
            if (wrr3Total_monday.Text != wrr3Ctrh_monday.Text || wrr3Total_tuesday.Text != wrr3Ctrh_tuesday.Text
                || wrr3Total_wednesday.Text != wrr3Ctrh_wednesday.Text || wrr3Total_thursday.Text != wrr3Ctrh_thursday.Text
                || wrr3Total_friday.Text != wrr3Ctrh_friday.Text || wrr3Total_saturday.Text != wrr3Ctrh_saturday.Text
                || wrr3Total_sunday.Text != wrr3Ctrh_sunday.Text)
            {
                correct = false;
            }

            return correct;
        }

        #endregion

        #region Save Begin

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

            DataRow wrr = e.DBAccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = e.DBAccess.DataSet.Tables["wrr1"];
            DataTable wrr1sum = e.DBAccess.DataSet.Tables["wrr1sum"];
            DataTable wrr2 = e.DBAccess.DataSet.Tables["wrr2"];
            DataTable wrr2sum = e.DBAccess.DataSet.Tables["wrr2sum"];
            DataTable wrr3 = e.DBAccess.DataSet.Tables["wrr3"];
            DataTable wrr3sum = e.DBAccess.DataSet.Tables["wrr3sum"];
            DataTable wrr4 = e.DBAccess.DataSet.Tables["wrr4"];
            DataTable wrr4sum = e.DBAccess.DataSet.Tables["wrr4sum"];
            DataTable wrrall = e.DBAccess.DataSet.Tables["wrrall"];
            DataTable atmr = e.DBAccess.DataSet.Tables["atmr"];

            if (BizFunctions.IsEmpty(wrr["issuedby"]))
            {
                wrr["issuedby"] = wrr["user"].ToString();
            }


            if (BizFunctions.IsEmpty(wrr["trandate"]))
            {
                wrr["trandate"] = DateTime.Now;
            }

            #region WRR1

            foreach (DataRow dr1 in wrr1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(wrr, dr1, "refnum/weeknum/user/flag/status/created/modified");

                    if (BizFunctions.IsEmpty(dr1["sitenum"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenum"]))
                        {
                            dr1["sitenum"] = wrr["sitenum"];
                        }
                    }

                    if (BizFunctions.IsEmpty(dr1["sitenumt"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenumt"]))
                        {
                            dr1["sitenumt"] = wrr["sitenumt"];
                        }
                    }
                }

            }

            #endregion

            #region WRR2

            foreach (DataRow dr2 in wrr2.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(wrr, dr2, "refnum/weeknum/user/flag/status/created/modified");

                    if (BizFunctions.IsEmpty(dr2["sitenum"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenum"]))
                        {
                            dr2["sitenum"] = wrr["sitenum"];
                        }
                    }

                    if (BizFunctions.IsEmpty(dr2["sitenumt"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenumt"]))
                        {
                            dr2["sitenumt"] = wrr["sitenumt"];
                        }
                    }

                }

            }

            #endregion

            #region WRR3

            foreach (DataRow dr3 in wrr3.Rows)
            {
                if (dr3.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(wrr, dr3, "refnum/weeknum/user/flag/status/created/modified");

                    if (BizFunctions.IsEmpty(dr3["sitenum"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenum"]))
                        {
                            dr3["sitenum"] = wrr["sitenum"];
                        }
                    }

                    if (BizFunctions.IsEmpty(dr3["sitenumt"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenumt"]))
                        {
                            dr3["sitenumt"] = wrr["sitenumt"];
                        }
                    }
                }

            }

            #endregion

            #region WRR4

            foreach (DataRow dr4 in wrr4.Rows)
            {
                if (dr4.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(wrr, dr4, "refnum/weeknum/user/flag/status/created/modified");

                    if (BizFunctions.IsEmpty(dr4["sitenum"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenum"]))
                        {
                            dr4["sitenum"] = wrr["sitenum"];
                        }
                    }

                    if (BizFunctions.IsEmpty(dr4["sitenumt"]))
                    {
                        if (!BizFunctions.IsEmpty(wrr["sitenumt"]))
                        {
                            dr4["sitenumt"] = wrr["sitenumt"];
                        }
                    }
                }
            }


            #endregion

     

        }
        #endregion

        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1sum = this.dbaccess.DataSet.Tables["wrr1sum"];
            DataTable wrr2sum = this.dbaccess.DataSet.Tables["wrr2sum"];
            DataTable wrr3sum = this.dbaccess.DataSet.Tables["wrr3sum"];
            DataTable wrr4sum = this.dbaccess.DataSet.Tables["wrr4sum"];
            DataTable wrrall = this.dbaccess.DataSet.Tables["wrrall"];
            DataTable atmr = this.dbaccess.DataSet.Tables["atmr"];
            DataTable wrr1 = this.dbaccess.DataSet.Tables["wrr1"];
            DataTable wrr4 = this.dbaccess.DataSet.Tables["wrr4"];


            
                //if (scheduleChanged)
                //{
                    Summary();
                //}
            
           

            foreach (DataRow dr1 in atmr.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(wrr, dr1, "refnum/sitenumt/user/flag/status/created/modified");

                    dr1["isConfirmed"] = 1;
                    dr1["ClockInMark"] = 1;

                }

            }

            foreach (DataRow dr4 in wrr4.Rows)
            {
                if (dr4.RowState != DataRowState.Deleted)
                {
                    foreach (DataRow dr1 in atmr.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {

                            if (dr4["empnum"].ToString().Trim() == dr1["empnum"].ToString().Trim())
                            {
                                dr1["isConfirmed"] = 0;
                                dr1["ClockInMark"] = 0;
                            }

                        }

                    }                   
                }

            }


            //foreach (DataRow dr1 in atmr.Rows)
            //{
            //    if (dr1.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(wrr, dr1, "refnum/sitenumt/user/flag/status/created/modified");

            //        dr1["isConfirmed"] = 1;
            //        dr1["ClockInMark"] = 1;

            //    }

            //}
        

            if (!CheckScheduleConflicts())
            {

                this.dbaccess.Update(e.SessionID, "WRR1SUM", "WRR1SUM");
                this.dbaccess.Update(e.SessionID, "WRR2SUM", "WRR2SUM");
                this.dbaccess.Update(e.SessionID, "WRR3SUM", "WRR3SUM");
                this.dbaccess.Update(e.SessionID, "WRR4SUM", "WRR4SUM");
                this.dbaccess.Update(e.SessionID, "WRRall", "WRRall");
                this.dbaccess.Update(e.SessionID, "ATMR", "ATMR");
            }

            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE WRR SET commencedate=CONVERT(DateTime, convert(varchar,COMMENCEDATE, 101)), enddate=CONVERT(DateTime, convert(varchar,ENDDATE, 101)) where refnum='"+wrr["refnum"].ToString()+"'");
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("EXEC [dbo].[DeleteDuplicatedATMR]");
        }

        # endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);

            //confirmed = true;


        }

        #endregion

        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "wrr_sitenumt":

                    //e.Condition = "SectorCode='" + SectorCode + "' AND status<>'V' AND sitenumt LIKE '"+wrr_sitenumt.Text+"%'";
                    e.Condition = "[status]<>'V' AND sitenumt LIKE '" + wrr_sitenumt.Text + "%'";
                       
                       
                    
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
                        e.Condition = BizFunctions.F2Condition("empnum/empname", (sender as TextBox).Text);
                    }
                    if (e.CurrentRow.Table.TableName == "sitmt2")
                    {
                        e.Condition = BizFunctions.F2Condition("empnum/empname", (sender as TextBox).Text);
                    }
                    if (e.CurrentRow.Table.TableName == "sitmt3")
                    {
                        e.Condition = BizFunctions.F2Condition("empnum/empname", (sender as TextBox).Text);
                    }
                    break;

                case "monday":
                    {

                        if (e.TableName == "wrr1")
                        {
                        }
                        if (e.TableName == "wrr2")
                        {
                        }
                        if (e.TableName == "wrr3")
                        {
                        }
                        if (e.TableName == "wrr4")
                        {
                        }

                    }
                    break;
                case "tuesday":
                    {
                        if (e.TableName == "wrr1")
                        {
                        }
                        if (e.TableName == "wrr2")
                        {
                        }
                        if (e.TableName == "wrr3")
                        {
                        }
                        if (e.TableName == "wrr4")
                        {
                        }
                    }
                    break;
                case "wednesday":
                    {
                        if (e.TableName == "wrr1")
                        {
                        }
                        if (e.TableName == "wrr2")
                        {
                        }
                        if (e.TableName == "wrr3")
                        {
                        }
                        if (e.TableName == "wrr4")
                        {
                        }
                    }
                    break;
                case "thursday":
                    {
                        if (e.TableName == "wrr1")
                        {
                        }
                        if (e.TableName == "wrr2")
                        {
                        }
                        if (e.TableName == "wrr3")
                        {
                        }
                        if (e.TableName == "wrr4")
                        {
                        }
                    }
                    break;
                case "friday":
                    {
                        if (e.TableName == "wrr1")
                        {
                        }
                        if (e.TableName == "wrr2")
                        {
                        }
                        if (e.TableName == "wrr3")
                        {
                        }
                        if (e.TableName == "wrr4")
                        {
                        }
                    }
                    break;
                case "saturday":
                    {
                        if (e.TableName == "wrr1")
                        {
                        }
                        if (e.TableName == "wrr2")
                        {
                        }
                        if (e.TableName == "wrr3")
                        {
                        }
                        if (e.TableName == "wrr4")
                        {
                        }
                    }
                    break;
                case "sunday":
                    {
                        if (e.TableName == "wrr1")
                        {
                        }
                        if (e.TableName == "wrr2")
                        {
                        }
                        if (e.TableName == "wrr3")
                        {
                        }
                        if (e.TableName == "wrr4")
                        {
                        }
                    }
                    break;


            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow wrr = dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = this.dbaccess.DataSet.Tables["WRR1"];
            DataTable wrr2 = this.dbaccess.DataSet.Tables["WRR2"];
            DataTable wrr3 = this.dbaccess.DataSet.Tables["WRR3"];
            switch (e.ControlName)
            {
                // later...
                case "wrr_sitenumt":
                    e.CurrentRow["sitename"] = e.F2CurrentRow["sitename"];
                    if (!BizFunctions.IsEmpty(wrr["sitenumt"].ToString()))
                    {
                        if (wrr["sitenumt"].ToString() == "ALLOUTLETS")
                        {
                            GetAllOutlets();
                            GetALLSitm4();
                        }
                        else
                        {
                            e.CurrentRow["contracttype"] = e.F2CurrentRow["contracttype"];
                            e.CurrentRow["docunum"] = e.F2CurrentRow["docunum"];

                            DataRow dr = BizLogicTools.Tools.GetCommonEmpDataRowByDoc(e.CurrentRow["docunum"].ToString());

                            e.CurrentRow["empnum"] = dr["empnum"].ToString();
                            e.CurrentRow["empname"] = dr["empname"].ToString();
   
                            GetSitm();
                            GetSitm1();
                            //GetSitm2();
                            //GetSitm3();
                            GetSitm4();

                            if (!BizFunctions.IsEmpty(wrr["sitenumt"]))
                            {
                                sc = new ScheduleControl(wrr["refnum"].ToString().Trim(), wrr["refnum"].ToString().Trim().Substring(0, 3), "WRR");
                            }


                            if (!BizFunctions.IsEmpty(wrr["docunum"]))
                            {
                                Getwrr1CtrhFooterTotals();
                                Getwrr2CtrhFooterTotals();
                                Getwrr3CtrhFooterTotals();
                            }
                        }

                    }
                    break;

                case "wrr_adhocrefnum":
                    if (!BizFunctions.IsEmpty(wrr["adhocrefnum"]))
                    {

                        if (BizFunctions.IsEmpty(wrr["commencedate"]))
                        {
                            wrr["commencedate"] = e.F2CurrentRow["adhocFrom"];
                        }
                        if (BizFunctions.IsEmpty(wrr["enddate"]))
                        {
                            wrr["enddate"] = e.F2CurrentRow["adhocTo"];
                        }

                        SetAllowedDays();

                        if (Convert.ToDateTime(wrr["commencedate"]).ToShortDateString() == Convert.ToDateTime(e.F2CurrentRow["adhocFrom"]).ToShortDateString() && Convert.ToDateTime(wrr["enddate"]).ToShortDateString() == Convert.ToDateTime(e.F2CurrentRow["adhocTo"]).ToShortDateString())
                        {

                            if (!BizFunctions.IsEmpty(wrr["adhocrefnum"].ToString().Trim()) || wrr["adhocrefnum"].ToString().Trim() != string.Empty)
                            {
                                string[] week ={ "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
                                #region Extraction of Information From Contract Information(CTRH)
                                string strExtractCTR = "Select * from adh where refnum= '" + wrr["adhocrefnum"].ToString().Trim() + "' ";
                                this.dbaccess.ReadSQL("ctrTmp", strExtractCTR);
                                DataTable ctrTmp = this.dbaccess.DataSet.Tables["ctrTmp"];
                                //ClearHeader();
                                int[] QtyOfDaysInWeek = new int[7];
                                if (ctrTmp.Rows.Count > 0)
                                {
                                    wrr["contracttype"] = "ADH";

                                    DataRow ctrTmpDR = dbaccess.DataSet.Tables["ctrTmp"].Rows[0];
                                    wrr["docunum"] = ctrTmpDR["refnum"].ToString();
                                    wrr["docunum"] = ctrTmpDR["refnum"].ToString();
                                    wrr["sitenum"] = ctrTmpDR["sitenum"].ToString();
                                    //wrr["sitename"] = ctrTmpDR["sitename"].ToString();
                                    wrr["coy"] = ctrTmpDR["coy"].ToString();
                                    wrr["coyname"] = ctrTmpDR["coyname"].ToString();
                                    wrr["sectorcode"] = ctrTmpDR["sectorcode"].ToString();
                                    wrr["addr1"] = ctrTmpDR["addr1"].ToString();
                                    wrr["addr2"] = ctrTmpDR["addr2"].ToString();
                                    wrr["addr3"] = ctrTmpDR["addr3"].ToString();
                                    wrr["postalcode"] = ctrTmpDR["postalcode"].ToString();
                                    //if (BizFunctions.IsEmpty(ctrTmpDR["officerqty"]))
                                    //{
                                    //    ctrTmpDR["officerqty"] = 0;
                                    //}
                                    //wrr["officerqty"] = ctrTmpDR["officerqty"];
                                    wrr["tel1"] = ctrTmpDR["tel1"].ToString();
                                    wrr["fax"] = ctrTmpDR["fax"].ToString();
                                    wrr["rep1"] = ctrTmpDR["rep1"].ToString();
                                    wrr["rep1tel"] = ctrTmpDR["rep1tel"].ToString();
                                    wrr["rep2"] = ctrTmpDR["rep2"].ToString();
                                    wrr["rep2tel"] = ctrTmpDR["rep2tel"].ToString();
                                    wrr["duty"] = ctrTmpDR["duty"].ToString();
                                    wrr["event"] = ctrTmpDR["event"].ToString();
                                    wrr["schedule"] = ctrTmpDR["schedule"].ToString();


                                    //wrr["isdaily"] = ctrTmpDR["isdaily"].ToString();
                                    //wrr["isweekdays"] = ctrTmpDR["isweekdays"].ToString();
                                    //wrr["isweekend"] = ctrTmpDR["isweekend"].ToString();

                                    //wrr["ispubhol"] = ctrTmpDR["ispubhol"].ToString();
                                    wrr["sinstruction"] = ctrTmpDR["sinstruction"].ToString();
                                    wrr["remark"] = ctrTmpDR["remark"].ToString();
                                    wrr["contractdate"] = ctrTmpDR["trandate"];






                                    //BizFunctions.DeleteAllRows(wrr1);
                                    //BizFunctions.DeleteAllRows(wrr2);
                                    //BizFunctions.DeleteAllRows(wrr3);

                                    //sc = new ScheduleControl(wrr["docunum"].ToString().Trim(), wrr["docunum"].ToString().Trim().Substring(0, 3), "wrr");

                                    string ManPowerSchedule = "";

                                    ManPowerSchedule = "select " +
                                                            "shiftcode, " +
                                                            "matnum, " +
                                                            "sum(monday*officerqty) as monday, " +
                                                            "sum(tuesday*officerqty) as tuesday, " +
                                                            "sum(wednesday*officerqty) as wednesday, " +
                                                            "sum(thursday*officerqty) as thursday, " +
                                                            "sum(friday*officerqty) as friday, " +
                                                            "sum(saturday*officerqty) as saturday, " +
                                                            "sum(sunday*officerqty) as sunday, " +
                                                            "shifttype " +
                                                        "from ADH1 where [status]<>'V' and refnum='" + wrr["adhocrefnum"].ToString().Trim() + "' and [guid]='"+ e.F2CurrentRow["guid"].ToString() +"' " +
                                                        "group by shiftcode,matnum,shifttype";
                                    //}
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



                                                #endregion

                                                #region Day Shift



                                                if (dr1["ShiftType"].ToString() == "D")
                                                {

                                                    for (int i = 0; i <= week.Length - 1; i++)
                                                    {
                                                        QtyOfDaysInWeek[i] = Convert.ToInt32(dr1[week[i].ToString()]);
                                                    }

                                                    int max = GetMaxDayOfWeek(QtyOfDaysInWeek);

                                                    DataTable tmpSitm1 = wrr1.Copy();

                                                    tmpSitm1.TableName = "tmpSitm1";

                                                    BizFunctions.DeleteAllRows(tmpSitm1);

                                                    for (int i = 0; i <= max - 1; i++)
                                                    {
                                                        DataRow InsertSitm1 = tmpSitm1.NewRow();
                                                        InsertSitm1["matnum"] = dr1["matnum"].ToString();
                                                        InsertSitm1["shiftcode"] = dr1["shiftcode"].ToString();
                                                        tmpSitm1.Rows.Add(InsertSitm1);
                                                    }

                                                    int count = 0;
                                                    int maxCount = 0;
                                                    for (int y = 0; y <= week.Length - 1; y++)
                                                    {
                                                        maxCount = Convert.ToInt32(dr1[week[y].ToString()]);
                                                        count = 0;
                                                        foreach (DataRow dr3 in tmpSitm1.Rows)
                                                        {
                                                            if (dr3.RowState != DataRowState.Deleted)
                                                            {
                                                                string DayTest = week[y];
                                                                if (maxCount <= count)
                                                                {
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    //dr3[week[y]] = "X";
                                                                    dr3[week[y]] = e.F2CurrentRow["shiftcode"];
                                                                    count = count + 1;
                                                                }




                                                            }
                                                        }
                                                    }



                                                    foreach (DataRow dr3 in tmpSitm1.Rows)
                                                    {
                                                        if (dr3.RowState != DataRowState.Deleted)
                                                        {
                                                            wrr1.ImportRow(dr3);
                                                        }
                                                    }

                                                }




                                                #endregion

                                                #region Night Shift

                                                if (dr1["ShiftType"].ToString() == "N")
                                                {
                                                    for (int i = 0; i <= week.Length - 1; i++)
                                                    {
                                                        QtyOfDaysInWeek[i] = Convert.ToInt32(dr1[week[i].ToString()]);
                                                    }

                                                    int max = GetMaxDayOfWeek(QtyOfDaysInWeek);

                                                    DataTable tmpSitm1 = wrr1.Copy();

                                                    tmpSitm1.TableName = "tmpSitm1";

                                                    BizFunctions.DeleteAllRows(tmpSitm1);

                                                    for (int i = 0; i <= max - 1; i++)
                                                    {
                                                        DataRow InsertSitm1 = tmpSitm1.NewRow();
                                                        InsertSitm1["matnum"] = dr1["matnum"].ToString();
                                                        InsertSitm1["shiftcode"] = dr1["shiftcode"].ToString();
                                                        tmpSitm1.Rows.Add(InsertSitm1);
                                                    }

                                                    int count = 0;
                                                    int maxCount = 0;
                                                    for (int y = 0; y <= week.Length - 1; y++)
                                                    {
                                                        maxCount = Convert.ToInt32(dr1[week[y].ToString()]);
                                                        count = 0;
                                                        foreach (DataRow dr3 in tmpSitm1.Rows)
                                                        {
                                                            if (dr3.RowState != DataRowState.Deleted)
                                                            {
                                                                string DayTest = week[y];
                                                                if (maxCount <= count)
                                                                {
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    dr3[week[y]] = e.F2CurrentRow["shiftcode"];
                                                                    count = count + 1;
                                                                }




                                                            }
                                                        }
                                                    }



                                                    foreach (DataRow dr3 in tmpSitm1.Rows)
                                                    {
                                                        if (dr3.RowState != DataRowState.Deleted)
                                                        {
                                                            wrr2.ImportRow(dr3);
                                                        }
                                                    }
                                                }

                                                #endregion

                                                #region ConciergeShift

                                                if (dr1["ShiftType"].ToString() == "C")
                                                {

                                                    for (int i = 0; i <= week.Length - 1; i++)
                                                    {
                                                        QtyOfDaysInWeek[i] = Convert.ToInt32(dr1[week[i].ToString()]);
                                                    }

                                                    int max = GetMaxDayOfWeek(QtyOfDaysInWeek);

                                                    DataTable tmpSitm1 = wrr1.Copy();

                                                    tmpSitm1.TableName = "tmpSitm1";

                                                    BizFunctions.DeleteAllRows(tmpSitm1);

                                                    for (int i = 0; i <= max - 1; i++)
                                                    {
                                                        DataRow InsertSitm1 = tmpSitm1.NewRow();
                                                        InsertSitm1["matnum"] = dr1["matnum"].ToString();
                                                        InsertSitm1["shiftcode"] = dr1["shiftcode"].ToString();
                                                        tmpSitm1.Rows.Add(InsertSitm1);
                                                    }

                                                    int count = 0;
                                                    int maxCount = 0;
                                                    for (int y = 0; y <= week.Length - 1; y++)
                                                    {
                                                        maxCount = Convert.ToInt32(dr1[week[y].ToString()]);
                                                        count = 0;
                                                        foreach (DataRow dr3 in tmpSitm1.Rows)
                                                        {
                                                            if (dr3.RowState != DataRowState.Deleted)
                                                            {
                                                                string DayTest = week[y];
                                                                if (maxCount <= count)
                                                                {
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    dr3[week[y]] = e.F2CurrentRow["shiftcode"];
                                                                    count = count + 1;
                                                                }




                                                            }
                                                        }
                                                    }



                                                    foreach (DataRow dr3 in tmpSitm1.Rows)
                                                    {
                                                        if (dr3.RowState != DataRowState.Deleted)
                                                        {
                                                            wrr3.ImportRow(dr3);
                                                        }
                                                    }


                                                }

                                                #endregion

                                                if (!BizFunctions.IsEmpty(wrr["docunum"]))
                                                {
                                                    Getwrr1CtrhFooterTotals();
                                                    Getwrr2CtrhFooterTotals();
                                                    Getwrr3CtrhFooterTotals();
                                                }

                                            }
                                        }
                                    }
                                    this.dbaccess.DataSet.Tables["ctrTmp"].Dispose();
                                    this.dbaccess.DataSet.Tables["ManPowerSchedule"].Dispose();

                                }
                                else
                                {
                                    MessageBox.Show("Contract No. Doesn't Exist!", "MAXVALUE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }


                                #endregion

                            }
                        }
                        else
                        {
                            MessageBox.Show("Can't Add Schedule which is different from the Header Schedule!", "MAXVALUE", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                    }
                    break;

                case "wrr_docunum":
                    {
                        DataRow dr = BizLogicTools.Tools.GetCommonEmpDataRowByDoc(e.CurrentRow["docunum"].ToString());
                        e.CurrentRow["sitenumt"] = System.DBNull.Value;
                        e.CurrentRow["empnum"] = dr["empnum"].ToString();
                        e.CurrentRow["empname"] = dr["empname"].ToString();
                    }
                    break;


            }
        }

        private void GetAllOutlets()
        {
            GetSitm1All();
        }

        private int GetMaxDayOfWeek(int[] a)
        {
            int max = a[0];

            for (int i = 0; i < a.Length - 1; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                }
            }

            return max;
        }

        #region Get Contract Footer Totals

        private void Getwrr1CtrhFooterTotals()
        {
            this.dbaccess.ReadSQL("CTRday", GetQuery("D"));
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


                wrr1Ctrh_monday.Text = drDay["monday"].ToString();
                wrr1Ctrh_tuesday.Text = drDay["tuesday"].ToString();
                wrr1Ctrh_wednesday.Text = drDay["wednesday"].ToString();
                wrr1Ctrh_thursday.Text = drDay["thursday"].ToString();
                wrr1Ctrh_friday.Text = drDay["friday"].ToString();
                wrr1Ctrh_saturday.Text = drDay["saturday"].ToString();
                wrr1Ctrh_sunday.Text = drDay["sunday"].ToString();
            }
            CTRday.Dispose();

        }

        private void Getwrr2CtrhFooterTotals()
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


                wrr2Ctrh_monday.Text = drNight["monday"].ToString();
                wrr2Ctrh_tuesday.Text = drNight["tuesday"].ToString();
                wrr2Ctrh_wednesday.Text = drNight["wednesday"].ToString();
                wrr2Ctrh_thursday.Text = drNight["thursday"].ToString();
                wrr2Ctrh_friday.Text = drNight["friday"].ToString();
                wrr2Ctrh_saturday.Text = drNight["saturday"].ToString();
                wrr2Ctrh_sunday.Text = drNight["sunday"].ToString();
            }
            CTRnight.Dispose();

        }

        private void Getwrr3CtrhFooterTotals()
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


                wrr3Ctrh_monday.Text = drconcierge["monday"].ToString();
                wrr3Ctrh_tuesday.Text = drconcierge["tuesday"].ToString();
                wrr3Ctrh_wednesday.Text = drconcierge["wednesday"].ToString();
                wrr3Ctrh_thursday.Text = drconcierge["thursday"].ToString();
                wrr3Ctrh_friday.Text = drconcierge["friday"].ToString();
                wrr3Ctrh_saturday.Text = drconcierge["saturday"].ToString();
                wrr3Ctrh_sunday.Text = drconcierge["sunday"].ToString();
            }
            CTRconcierge.Dispose();

        }

        private string GetQuery(string ShiftType)
        {
            DataRow wrr = dbaccess.DataSet.Tables["wrr"].Rows[0];
            string Query = "";
            if (wrr["contracttype"].ToString() == "CTRH")
            {
                //Query = "Select " +
                //                "SUM(monday) as monday, " +
                //                "SUM(tuesday) as tuesday, " +
                //                "SUM(wednesday) as wednesday, " +
                //                "SUM(thurday) as thursday, " +
                //                "SUM(friday) as friday, " +
                //                "SUM(saturday) as saturday, " +
                //                "SUM(sunday) as sunday " +
                //            "from " +
                //            "( " +
                //                "select " +
                //                    "officerqty, " +
                //                    "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday " +
                //                    "from CTR1 " +
                //                "where refnum='" + wrr["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +

                //                "union all " +

                //                "select " +
                //                    "officerqty, " +
                //                    "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, " +
                //                    "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday " +
                //                    "from ADH1 " +
                //                "where ctrnum='" + wrr["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +
                //            ")a";

                Query = "Select " +
                           "SUM(monday) as monday, " +
                           "SUM(tuesday) as tuesday, " +
                           "SUM(wednesday) as wednesday, " +
                           "SUM(thursday) as thursday, " +
                           "SUM(friday) as friday, " +
                           "SUM(saturday) as saturday, " +
                           "SUM(sunday) as sunday " +
                       "from " +
                       "( " +
                           "select " +
                               "officerqty, " +
                             	"CASE WHEN xday1 is not null then ISNULL(officerqty,0)*1 else 0 end as monday, " +
				                "CASE WHEN xday2 is not null then ISNULL(officerqty,0)*1 else 0 end as tuesday, " +
				                "CASE WHEN xday3 is not null then ISNULL(officerqty,0)*1 else 0 end as wednesday, " +
				                "CASE WHEN xday4 is not null then ISNULL(officerqty,0)*1 else 0 end as thursday, " +
				                "CASE WHEN xday5 is not null then ISNULL(officerqty,0)*1 else 0 end as friday, " +
				                "CASE WHEN xday6 is not null then ISNULL(officerqty,0)*1 else 0 end as saturday, " +
				                "CASE WHEN xday7 is not null then ISNULL(officerqty,0)*1 else 0 end as sunday " +
                               "from CTR1 " +
                           "where refnum='" + wrr["docunum"].ToString().Trim() + "'  " +

                           "union all " +

                           "select " +
                               "officerqty, " +
                             	"CASE WHEN xday1 is not null then ISNULL(officerqty,0)*1 else 0 end as monday, " +
				                "CASE WHEN xday2 is not null then ISNULL(officerqty,0)*1 else 0 end as tuesday, " +
				                "CASE WHEN xday3 is not null then ISNULL(officerqty,0)*1 else 0 end as wednesday, " +
				                "CASE WHEN xday4 is not null then ISNULL(officerqty,0)*1 else 0 end as thursday, " +
				                "CASE WHEN xday5 is not null then ISNULL(officerqty,0)*1 else 0 end as friday, " +
				                "CASE WHEN xday6 is not null then ISNULL(officerqty,0)*1 else 0 end as saturday, " +
                                "CASE WHEN xday7 is not null then ISNULL(officerqty,0)*1 else 0 end as sunday " +
                               "from ADH1 " +
                           "where ctrnum='" + wrr["docunum"].ToString().Trim() + "'  " +
                       ")a";
            }
            if (wrr["contracttype"].ToString() == "ADH")
            {
                Query = "Select " +
                                "SUM(monday) as monday, " +
                                "SUM(tuesday) as tuesday, " +
                                "SUM(wednesday) as wednesday, " +
                                "SUM(thursday) as thursday, " +
                                "SUM(friday) as friday, " +
                                "SUM(saturday) as saturday, " +
                                "SUM(sunday) as sunday " +
                            "from " +
                            "( " +
                                "select " +
                                    "officerqty, " +
                                    "CASE WHEN xday1 is not null then ISNULL(officerqty,0)*1 else 0 end as monday, " +
                                    "CASE WHEN xday2 is not null then ISNULL(officerqty,0)*1 else 0 end as tuesday, " +
                                    "CASE WHEN xday3 is not null then ISNULL(officerqty,0)*1 else 0 end as wednesday, " +
                                    "CASE WHEN xday4 is not null then ISNULL(officerqty,0)*1 else 0 end as thursday, " +
                                    "CASE WHEN xday5 is not null then ISNULL(officerqty,0)*1 else 0 end as friday, " +
                                    "CASE WHEN xday6 is not null then ISNULL(officerqty,0)*1 else 0 end as saturday, " +
                                    "CASE WHEN xday7 is not null then ISNULL(officerqty,0)*1 else 0 end as sunday " +
                                    "from ADH1 " +
                                    "where refnum='" + wrr["adhocrefnum"].ToString().Trim() + "'  and adhocFrom>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr["commencedate"])) + "' and adhocTo<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr["enddate"])) + "' " +
                            ")a";
            }

            return Query;

        }

        #endregion

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow wrr = dbaccess.DataSet.Tables["wrr"].Rows[0];
            switch (e.MappingName)
            {
                case "empnum":
                    e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    e.CurrentRow["nric"] = e.F2CurrentRow["nric"];
                    break;

                case "monday":
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

            reOpen = true;
           


        }

        protected override void Document_Void_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Void_Handle(sender, e);
            voidClicked = true;

        }

        protected override void Document_Void_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Void_OnClick(sender, e);
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            string DeleteFromATMR = "Delete from ATMR where refnum='" + wrr["refnum"].ToString()  + "'";
            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(DeleteFromATMR);
            

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
                              
            switch (e.ReportName)
            {
       
                case "Weekly Rooster 1":                                                                     
                    e.DataSource = WR1();
                    break;
            }

        }

        #endregion

        #region Dataset DS1

        private DataSet WR1()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            Hashtable selectedCollection = new Hashtable();
            string from, to = "";

            DataSet ds1 = new DataSet("ds1WRR");

            if (ds1.Tables.Contains("WRRcomplete"))
            {
                ds1.Tables["WRRcomplete"].Dispose();
                ds1.Tables.Remove("WRRcomplete");

            }

            

            string wrrComplete = "select matnum,empnum,empname,monday,tuesday,wednesday,thursday,friday,saturday,sunday,remark, 'DAY SHIFT' as ShiftType from wrr1 where refnum='" + wrr["refnum"].ToString() + "' " +
                                    "union " +
                                 "select matnum,empnum,empname,monday,tuesday,wednesday,thursday,friday,saturday,sunday,remark, 'NIGHT SHIFT' as ShiftType from wrr2 where refnum='" + wrr["refnum"].ToString() + "' " +
                                    "union " +
                                 "select matnum,empnum,empname,monday,tuesday,wednesday,thursday,friday,saturday,sunday,remark, 'RELIEF' as ShiftType from wrr4 where refnum='" + wrr["refnum"].ToString() + "' ";

            selectedCollection.Add("WRRcomplete", wrrComplete);
            selectedCollection.Add("COY", "Select * from coy where coy='" + wrr["coy"].ToString() + "'");
            selectedCollection.Add("SITM", "Select * from SITM where sitenum='" + wrr["sitenum"].ToString() + "'");
            this.dbaccess.ReadSQL(selectedCollection);

            ds1 = this.dbaccess.ReadSQLTemp(selectedCollection);

            DataTable dtWeek = new DataTable("WEEKDATES");
            dtWeek.Columns.Add("Monday", typeof(string));
            dtWeek.Columns.Add("Tuesday", typeof(string));
            dtWeek.Columns.Add("Wednesday", typeof(string));
            dtWeek.Columns.Add("Thursday", typeof(string));
            dtWeek.Columns.Add("Friday", typeof(string));
            dtWeek.Columns.Add("Saturday", typeof(string));
            dtWeek.Columns.Add("Sunday", typeof(string));

            DataTable dtWRR1total = new DataTable("WRR1TOTAL");
            dtWRR1total.Columns.Add("Monday", typeof(int));
            dtWRR1total.Columns.Add("Tuesday", typeof(int));
            dtWRR1total.Columns.Add("Wednesday", typeof(int));
            dtWRR1total.Columns.Add("Thursday", typeof(int));
            dtWRR1total.Columns.Add("Friday", typeof(int));
            dtWRR1total.Columns.Add("Saturday", typeof(int));
            dtWRR1total.Columns.Add("Sunday", typeof(int));
            dtWRR1total.Columns.Add("ShiftTypeRemark", typeof(string));

            DataRow IndrdtWRR1total = dtWRR1total.NewRow();
            IndrdtWRR1total["Monday"] = Convert.ToInt32(wrr1Total_monday.Text);
            IndrdtWRR1total["Tuesday"] = Convert.ToInt32(wrr1Total_tuesday.Text);
            IndrdtWRR1total["Wednesday"] = Convert.ToInt32(wrr1Total_wednesday.Text);
            IndrdtWRR1total["Thursday"] = Convert.ToInt32(wrr1Total_thursday.Text);
            IndrdtWRR1total["Friday"] = Convert.ToInt32(wrr1Total_friday.Text);
            IndrdtWRR1total["Saturday"] = Convert.ToInt32(wrr1Total_saturday.Text);
            IndrdtWRR1total["Sunday"] = Convert.ToInt32(wrr1Total_sunday.Text);
            IndrdtWRR1total["ShiftTypeRemark"] = "TOTAL STRENGHT (DAY)";
            dtWRR1total.Rows.Add(IndrdtWRR1total);

            if (ds1.Tables.Contains("WRR1TOTAL"))
            {
                ds1.Tables["WRR1TOTAL"].Dispose();
                ds1.Tables.Remove("WRR1TOTAL");
                dtWRR1total.TableName = "WRR1TOTAL";
                ds1.Tables.Add(dtWRR1total);
            }
            else
            {
                dtWRR1total.TableName = "WRR1TOTAL";
                ds1.Tables.Add(dtWRR1total);
            }

            DataTable dtWRR2total = new DataTable("WRR2TOTAL");
            dtWRR2total.Columns.Add("Monday", typeof(int));
            dtWRR2total.Columns.Add("Tuesday", typeof(int));
            dtWRR2total.Columns.Add("Wednesday", typeof(int));
            dtWRR2total.Columns.Add("Thursday", typeof(int));
            dtWRR2total.Columns.Add("Friday", typeof(int));
            dtWRR2total.Columns.Add("Saturday", typeof(int));
            dtWRR2total.Columns.Add("Sunday", typeof(int));
            dtWRR2total.Columns.Add("ShiftTypeRemark", typeof(string));

            DataRow IndrdtWRR2total = dtWRR2total.NewRow();
            IndrdtWRR2total["Monday"] = Convert.ToInt32(wrr2Total_monday.Text);
            IndrdtWRR2total["Tuesday"] = Convert.ToInt32(wrr2Total_tuesday.Text);
            IndrdtWRR2total["Wednesday"] = Convert.ToInt32(wrr2Total_wednesday.Text);
            IndrdtWRR2total["Thursday"] = Convert.ToInt32(wrr2Total_thursday.Text);
            IndrdtWRR2total["Friday"] = Convert.ToInt32(wrr2Total_friday.Text);
            IndrdtWRR2total["Saturday"] = Convert.ToInt32(wrr2Total_saturday.Text);
            IndrdtWRR2total["Sunday"] = Convert.ToInt32(wrr2Total_sunday.Text);
            IndrdtWRR2total["ShiftTypeRemark"] = "TOTAL STRENGHT (NIGHT)";

            dtWRR2total.Rows.Add(IndrdtWRR2total);

            if (ds1.Tables.Contains("WRR2TOTAL"))
            {
                ds1.Tables["WRR2TOTAL"].Dispose();
                ds1.Tables.Remove("WRR2TOTAL");
                dtWRR2total.TableName = "WRR2TOTAL";
                ds1.Tables.Add(dtWRR2total);
            }
            else
            {
                dtWRR2total.TableName = "WRR2TOTAL";
                ds1.Tables.Add(dtWRR2total);
            }

            //

            DataTable dtWRR3total = new DataTable("WRR2TOTAL");
            dtWRR3total.Columns.Add("Monday", typeof(int));
            dtWRR3total.Columns.Add("Tuesday", typeof(int));
            dtWRR3total.Columns.Add("Wednesday", typeof(int));
            dtWRR3total.Columns.Add("Thursday", typeof(int));
            dtWRR3total.Columns.Add("Friday", typeof(int));
            dtWRR3total.Columns.Add("Saturday", typeof(int));
            dtWRR3total.Columns.Add("Sunday", typeof(int));
            dtWRR3total.Columns.Add("ShiftTypeRemark", typeof(string));

            DataRow IndrdtWRR3total = dtWRR3total.NewRow();
            IndrdtWRR3total["Monday"] = Convert.ToInt32(wrr3Total_monday.Text);
            IndrdtWRR3total["Tuesday"] = Convert.ToInt32(wrr3Total_tuesday.Text);
            IndrdtWRR3total["Wednesday"] = Convert.ToInt32(wrr3Total_wednesday.Text);
            IndrdtWRR3total["Thursday"] = Convert.ToInt32(wrr3Total_thursday.Text);
            IndrdtWRR3total["Friday"] = Convert.ToInt32(wrr3Total_friday.Text);
            IndrdtWRR3total["Saturday"] = Convert.ToInt32(wrr3Total_saturday.Text);
            IndrdtWRR3total["Sunday"] = Convert.ToInt32(wrr3Total_sunday.Text);
            IndrdtWRR3total["ShiftTypeRemark"] = "TOTAL STRENGHT (CONCIERGE)";

            dtWRR3total.Rows.Add(IndrdtWRR3total);

            if (ds1.Tables.Contains("WRR3TOTAL"))
            {
                ds1.Tables["WRR3TOTAL"].Dispose();
                ds1.Tables.Remove("WRR3TOTAL");
                dtWRR3total.TableName = "WRR3TOTAL";
                ds1.Tables.Add(dtWRR3total);
            }
            else
            {
                dtWRR3total.TableName = "WRR3TOTAL";
                ds1.Tables.Add(dtWRR3total);
            }


            string getFinalTotal = "Select * from WRR1TOTAL UNION Select * from WRR2TOTAL UNION  Select * from WRR3TOTAL";
            DataTable WRRFINALTOTAL = BizFunctions.ExecuteQuery(ds1, getFinalTotal);

            if (ds1.Tables.Contains("WRRFINALTOTAL"))
            {
                ds1.Tables["WRRFINALTOTAL"].Dispose();
                ds1.Tables.Remove("WRRFINALTOTALS");
                WRRFINALTOTAL.TableName = "WRRFINALTOTAL";
                ds1.Tables.Add(WRRFINALTOTAL);
            }
            else
            {
                WRRFINALTOTAL.TableName = "WRRFINALTOTAL";
                ds1.Tables.Add(WRRFINALTOTAL);
            }


            from = BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr["commencedate"]));
            to = BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr["commencedate"]).AddDays(6));
            DataTable tmpWeek = TimeTools.WeekTable(from, to);
            if (tmpWeek.Rows.Count > 0)
            {




                DataRow IndrWeek = dtWeek.NewRow();
                dtWeek.Rows.Add(IndrWeek);
                DataRow drWeek = dtWeek.Rows[0];

                foreach (DataRow dr1 in tmpWeek.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (dr1["Day"].ToString() == "Monday")
                        {
                            drWeek["Monday"] = Convert.ToDateTime(dr1["Date"]).ToShortDateString();
                        }
                        if (dr1["Day"].ToString() == "Tuesday")
                        {
                            drWeek["Tuesday"] = Convert.ToDateTime(dr1["Date"]).ToShortDateString();
                        }
                        if (dr1["Day"].ToString() == "Wednesday")
                        {
                            drWeek["Wednesday"] = Convert.ToDateTime(dr1["Date"]).ToShortDateString();
                        }
                        if (dr1["Day"].ToString() == "Thursday")
                        {
                            drWeek["Thursday"] = Convert.ToDateTime(dr1["Date"]).ToShortDateString();
                        }
                        if (dr1["Day"].ToString() == "Friday")
                        {
                            drWeek["Friday"] = Convert.ToDateTime(dr1["Date"]).ToShortDateString();
                        }
                        if (dr1["Day"].ToString() == "Saturday")
                        {
                            drWeek["Saturday"] = Convert.ToDateTime(dr1["Date"]).ToShortDateString();
                        }
                        if (dr1["Day"].ToString() == "Sunday")
                        {
                            drWeek["Sunday"] = Convert.ToDateTime(dr1["Date"]).ToShortDateString();
                        }
                    }
                }

                if (ds1.Tables.Contains("WEEKDATES"))
                {
                    ds1.Tables["WEEKDATES"].Dispose();
                    ds1.Tables.Remove("WEEKDATES");
                    dtWeek.TableName = "WEEKDATES";
                    ds1.Tables.Add(dtWeek);
                }
                else
                {
                    dtWeek.TableName = "WEEKDATES";
                    ds1.Tables.Add(dtWeek);
                }


            }

            DataTable WRR = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM WRR");
            if (ds1.Tables.Contains("WRR"))
            {
                ds1.Tables["WRR"].Dispose();
                ds1.Tables.Remove("WRR");
                WRR.TableName = "WRR";
                ds1.Tables.Add(WRR);

            }
            else
            {
                WRR.TableName = "WRR";
                ds1.Tables.Add(WRR);
            }

            return ds1;

        }

        #endregion

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {        
            base.Document_Preview_Handle(sender, e);
            DataRow wrr = dbaccess.DataSet.Tables["wrr"].Rows[0];
            if (wrr["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "wrr"))
                {
                    MessageBox.Show("Please Summarize then Save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion

        #region Get Shift Code

        private string Getshiftcode(string tablename, string refnum)
        {
            string result = "";
            string sql1 = "Select distinct shiftcode from " + tablename + " where refnum='" + refnum + "'";
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

        #region Voucher_WRR1_ColumnChanged

        private void Voucher_WRR1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable wrr1 = this.dbaccess.DataSet.Tables["WRR1"];

            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                            scheduleChanged = true;
                        }                                   
                    }
                    break;



                case "monday":
                    {
                        scheduleChanged = true;                                                 
                    }
                    break;

                case "tuesday":
                    {
                        scheduleChanged = true;                         
                    }
                    break;

                case "wednesday":
                    {
                        scheduleChanged = true;               
                    }
                    break;

                case "thursday":
                    {
                        scheduleChanged = true;                 
                    }
                    break;

                case "friday":
                    {
                        scheduleChanged = true;                 
                    }
                    break;

                case "saturday":
                    {
                        scheduleChanged = true;                            
                    }
                    break;

                case "sunday":
                    {
                        scheduleChanged = true;                                
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

        #region Voucher_WRR2_ColumnChanged

        private void Voucher_WRR2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable wrr2 = this.dbaccess.DataSet.Tables["WRR2"];

            e.Row.BeginEdit();

            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {
  
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                            scheduleChanged = true;
                        }
                                   
                    }
                    break;

                case "monday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "tuesday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "wednesday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "thursday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "friday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "saturday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "sunday":
                    {
                        scheduleChanged = true;          
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

        #region Voucher_WRR3_ColumnChanged

        private void Voucher_WRR3_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable wrr3 = this.dbaccess.DataSet.Tables["WRR3"];

            e.Row.BeginEdit();

            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {                        
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                            scheduleChanged = true;
                        }                                   
                    }
                    break;

                case "monday":
                    {
                        scheduleChanged = true;                       
                    }
                    break;

                case "tuesday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "wednesday":
                    {
                        scheduleChanged = true;                        
                    }
                    break;

                case "thursday":
                    {
                        scheduleChanged = true;         
                    }
                    break;

                case "friday":
                    {
                        scheduleChanged = true;                   
                    }
                    break;

                case "saturday":
                    {
                        scheduleChanged = true;                       

                    }
                    break;

                case "sunday":
                    {
                        scheduleChanged = true;
                      
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

        #region Voucher_WRR4_ColumnChanged

        private void Voucher_WRR4_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
                            scheduleChanged = true;
                        }
                                   
                    }
                    break;

                case "monday":
                    {
                        scheduleChanged = true;

                    }
                    break;

                case "tuesday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "wednesday":
                    {

                        scheduleChanged = true;
                    }
                    break;

                case "thursday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "friday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "saturday":
                    {
                        scheduleChanged = true;
                    }
                    break;

                case "sunday":
                    {
                        scheduleChanged = true;
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

        #region Summarize All

        private void Summary()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = this.dbaccess.DataSet.Tables["wrr1"];
            DataTable wrr1sum = this.dbaccess.DataSet.Tables["wrr1sum"];

            DataTable wrr2 = this.dbaccess.DataSet.Tables["wrr2"];
            DataTable wrr2sum = this.dbaccess.DataSet.Tables["wrr2sum"];

            DataTable wrr3 = this.dbaccess.DataSet.Tables["wrr3"];
            DataTable wrr3sum = this.dbaccess.DataSet.Tables["wrr3sum"];

            DataTable wrr4 = this.dbaccess.DataSet.Tables["wrr4"];
            DataTable wrr4sum = this.dbaccess.DataSet.Tables["wrr4sum"];

            DataTable wrrall = this.dbaccess.DataSet.Tables["wrrall"];



            if (wrr1sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr1sum);
            }
            DataTable wrr1sumTmp = BizLogicTools.Tools.GetShiftSummary2("WRR1", wrr["refnum"].ToString(), this.dbaccess.DataSet, this.dbaccess);

            //if(this.dbaccess.DataSet.Tables.Contains("wrr1sumTmp"))
            //{
            //    this.dbaccess.DataSet.Tables["wrr1sumTmp"].Dispose();
            //}
            //wrr1sumTmp.TableName = "wrr1sumTmp";
            //this.dbaccess.DataSet.Tables.Add(wrr1sumTmp);

            foreach (DataRow dr4 in wrr1sumTmp.Select())
            {
                if (dr4.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm1Sum = wrr1sum.NewRow();
                    drSitm1Sum["matnum"] = dr4["matnum"];
                    //drSitm1Sum["shiftcode"] = wrr1_dayshiftcode.Text.ToString();
                    drSitm1Sum["shiftcode"] = dr4["shiftcode"]; ;
                    drSitm1Sum["monday"] = dr4["monday"];
                    drSitm1Sum["tuesday"] = dr4["tuesday"];
                    drSitm1Sum["wednesday"] = dr4["wednesday"];
                    drSitm1Sum["thursday"] = dr4["thursday"];
                    drSitm1Sum["friday"] = dr4["friday"];
                    drSitm1Sum["saturday"] = dr4["saturday"];
                    drSitm1Sum["sunday"] = dr4["sunday"];
                    wrr1sum.Rows.Add(drSitm1Sum);
                }
            }

            if (wrr2sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr2sum);
            }
            DataTable wrr2sumTmp = BizLogicTools.Tools.GetShiftSummary2("WRR2", wrr["refnum"].ToString(), this.dbaccess.DataSet, this.dbaccess);
            foreach (DataRow dr5 in wrr2sumTmp.Select())
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm2Sum = wrr2sum.NewRow();
                    drSitm2Sum["matnum"] = dr5["matnum"];
                    drSitm2Sum["shiftcode"] = dr5["shiftcode"]; ;
                    drSitm2Sum["monday"] = dr5["monday"];
                    drSitm2Sum["tuesday"] = dr5["tuesday"];
                    drSitm2Sum["wednesday"] = dr5["wednesday"];
                    drSitm2Sum["thursday"] = dr5["thursday"];
                    drSitm2Sum["friday"] = dr5["friday"];
                    drSitm2Sum["saturday"] = dr5["saturday"];
                    drSitm2Sum["sunday"] = dr5["sunday"];
                    wrr2sum.Rows.Add(drSitm2Sum);
                }
            }

            if (wrr3sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr3sum);
            }
            DataTable wrr3sumTmp = BizLogicTools.Tools.GetShiftSummary2("WRR3", wrr["refnum"].ToString(), this.dbaccess.DataSet, this.dbaccess);
            foreach (DataRow dr6 in wrr3sumTmp.Select())
            {
                if (dr6.RowState != DataRowState.Deleted)
                {

                    DataRow drSitm3Sum = wrr3sum.NewRow();
                    drSitm3Sum["matnum"] = dr6["matnum"];
                    drSitm3Sum["shiftcode"] = dr6["shiftcode"]; ;
                    drSitm3Sum["monday"] = dr6["monday"];
                    drSitm3Sum["tuesday"] = dr6["tuesday"];
                    drSitm3Sum["wednesday"] = dr6["wednesday"];
                    drSitm3Sum["thursday"] = dr6["thursday"];
                    drSitm3Sum["friday"] = dr6["friday"];
                    drSitm3Sum["saturday"] = dr6["saturday"];
                    drSitm3Sum["sunday"] = dr6["sunday"];
                    wrr3sum.Rows.Add(drSitm3Sum);
                }
            }




            string reliefTmp = "Select " +
                                   "R1.matnum, " +
                                   "R1.shiftcode, " +
                                   "SUM(R1.monday) AS monday, " +
                                   "SUM(R1.tuesday) AS tuesday, " +
                                   "SUM(R1.wednesday) AS wednesday, " +
                                   "SUM(R1.thursday) AS thursday, " +
                                   "SUM(R1.friday) AS friday, " +
                                   "SUM(R1.saturday) AS saturday, " +
                                   "SUM(R1.sunday) AS sunday " +
                                 "FROM " +
                                       "( " +
                //monday
                                           "Select matnum,shiftcode,sum(monday) as monday,sum(tuesday) as tuesday,sum(wednesday) as wednesday,sum(thursday) as thursday,sum(friday) as friday,sum(saturday) as saturday,sum(sunday) as sunday " +
                                           "from " +
                                           "( " +
                                               "select matnum,monday as shiftcode,COUNT(monday) as monday,COUNT(tuesday) * 0 as tuesday,COUNT(wednesday) * 0 as wednesday,COUNT(thursday) * 0 as thursday,COUNT(friday) * 0 as friday,COUNT(saturday) * 0 as saturday,COUNT(sunday) * 0 as sunday " +
                                               "from WRR4 where refnum='" + wrr["refnum"].ToString() + "' " +
                                               "group by monday,matnum " +
                                           ")a " +
                                           "where shiftcode is not null " +
                                           "group by shiftcode,matnum " +

                                           "UNION " +

                                       //tuesday
                                           "Select matnum,shiftcode,sum(monday) as monday,sum(tuesday) as tuesday,sum(wednesday) as wednesday, sum(thursday) as thursday,sum(friday) as friday,sum(saturday) as saturday,sum(sunday) as sunday " +
                                           "from " +
                                           "( " +
                                               "select matnum,tuesday as shiftcode,COUNT(monday) * 0 as monday,COUNT(tuesday) as tuesday,COUNT(wednesday) * 0 as wednesday,COUNT(thursday) * 0 as thursday,COUNT(friday) * 0 as friday,COUNT(saturday) * 0 as saturday,COUNT(sunday) * 0 as sunday " +
                                               "from WRR4 where refnum='" + wrr["refnum"].ToString() + "' " +
                                               "group by tuesday,matnum " +
                                           ")b " +
                                           "where shiftcode is not null " +
                                           "group by shiftcode,matnum " +

                                           "UNION " +

                                       //wednesday
                                           "Select matnum,shiftcode,sum(monday) as monday, sum(tuesday) as tuesday,sum(wednesday) as wednesday,sum(thursday) as thursday,sum(friday) as friday,sum(saturday) as saturday,sum(sunday) as sunday " +
                                           "from " +
                                           "( " +
                                               "select matnum,wednesday as shiftcode,COUNT(monday) * 0  as monday,COUNT(tuesday) * 0 as tuesday,COUNT(wednesday)as wednesday,COUNT(thursday) * 0 as thursday,COUNT(friday) * 0 as friday,COUNT(saturday) * 0 as saturday,COUNT(sunday) * 0 as sunday " +
                                               "from WRR4 where refnum='" + wrr["refnum"].ToString() + "' " +
                                               "group by wednesday,matnum " +
                                           ")c " +
                                           "where shiftcode is not null " +
                                           "group by shiftcode,matnum " +

                                           "UNION " +

                                       //thursday	
                                           "Select matnum,shiftcode,sum(monday) as monday,sum(tuesday) as tuesday,sum(wednesday) as wednesday, sum(thursday) as thursday,sum(friday) as friday,sum(saturday) as saturday,sum(sunday) as sunday " +
                                           "from " +
                                           "( " +
                                               "select matnum,thursday as shiftcode,COUNT(monday) * 0 as monday,COUNT(tuesday) * 0 as tuesday,COUNT(wednesday) * 0 as wednesday,COUNT(thursday) as thursday,COUNT(friday) * 0 as friday,COUNT(saturday) * 0 as saturday,COUNT(sunday) * 0 as sunday " +
                                               "from WRR4 where refnum='" + wrr["refnum"].ToString() + "' " +
                                               "group by thursday,matnum " +
                                           ")d " +
                                           "where shiftcode is not null " +
                                           "group by shiftcode,matnum " +

                                           "UNION " +

                                       //friday	
                                           "Select matnum,shiftcode,sum(monday) as monday,sum(tuesday) as tuesday,sum(wednesday) as wednesday,sum(thursday) as thursday,sum(friday) as friday,sum(saturday) as saturday,sum(sunday) as sunday " +
                                            "from " +
                                           "( " +
                                               "select matnum,friday as shiftcode,COUNT(monday) * 0  as monday,COUNT(tuesday) * 0 as tuesday,COUNT(wednesday) * 0 as wednesday,COUNT(thursday) * 0 as thursday,COUNT(friday) as friday,COUNT(saturday) * 0 as saturday,COUNT(sunday) * 0 as sunday " +
                                               "from WRR4 where refnum='" + wrr["refnum"].ToString() + "' " +
                                               "group by friday,matnum " +
                                           ")e " +
                                           "where shiftcode is not null " +
                                           "group by shiftcode,matnum " +

                                           "UNION " +

                                       //saturday	
                                           "Select matnum,shiftcode, sum(monday) as monday,sum(tuesday) as tuesday,sum(wednesday) as wednesday,sum(thursday) as thursday, sum(friday) as friday,sum(saturday) as saturday, sum(sunday) as sunday " +
                                           "from " +
                                           "( " +
                                               "select matnum,saturday as shiftcode,COUNT(monday)  * 0  as monday,COUNT(tuesday) * 0 as tuesday,COUNT(wednesday) * 0 as wednesday,COUNT(thursday) * 0 as thursday,COUNT(friday) * 0 as friday,COUNT(saturday) as saturday,COUNT(sunday) * 0 as sunday " +
                                               "from WRR4 where refnum='" + wrr["refnum"].ToString() + "' " +
                                               "group by saturday,matnum " +
                                           ")f " +
                                           "where shiftcode is not null " +
                                           "group by shiftcode,matnum " +

                                           "UNION " +

                                       //sunday
                                           "Select matnum,shiftcode,sum(monday) as monday,sum(tuesday) as tuesday,sum(wednesday) as wednesday, sum(thursday) as thursday, sum(friday) as friday,sum(saturday) as saturday,sum(sunday) as sunday " +
                                           "from " +
                                           "( " +
                                               "select matnum,sunday as shiftcode,COUNT(monday) * 0  as monday,COUNT(tuesday) * 0 as tuesday,COUNT(wednesday) * 0 as wednesday,COUNT(thursday) * 0 as thursday,COUNT(friday) * 0 as friday,COUNT(saturday) * 0 as saturday,COUNT(sunday) as sunday " +
                                               "from WRR4 where refnum='" + wrr["refnum"].ToString() + "' " +
                                               "group by sunday,matnum " +
                                           ")g " +
                                           "where shiftcode is not null " +
                                           "group by shiftcode,matnum " +
                                       ") R1 " +
                               "GROUP BY R1.matnum,R1.SHIFTCODE";


            //DataTable wrr4sumTmp = BizLogicTools.Tools.GetShiftSummary2("WRR4", wrr["refnum"].ToString(), this.dbaccess.DataSet, this.dbaccess);

            DataTable wrr4sumTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, reliefTmp);

            if (wrr4sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr4sum);
            }

            foreach (DataRow dr7 in wrr4sumTmp.Select())
            {

                if (dr7.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm4Sum = wrr4sum.NewRow();
                    drSitm4Sum["matnum"] = dr7["matnum"];
                    drSitm4Sum["shiftcode"] = dr7["shiftcode"];
                    drSitm4Sum["monday"] = dr7["monday"];
                    drSitm4Sum["tuesday"] = dr7["tuesday"];
                    drSitm4Sum["wednesday"] = dr7["wednesday"];
                    drSitm4Sum["thursday"] = dr7["thursday"];
                    drSitm4Sum["friday"] = dr7["friday"];
                    drSitm4Sum["saturday"] = dr7["saturday"];
                    drSitm4Sum["sunday"] = dr7["sunday"];
                    wrr4sum.Rows.Add(drSitm4Sum);
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
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from wrr1sum " +
                                "union all " +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from wrr2sum " +
                                "union all " +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from wrr3sum " +
                                "union all " +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from wrr4sum " +
                                ") R1 where R1.shiftcode<>'' " +
                                "Group by R1.shiftcode";

            DataTable wrrallsumTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, overallSum);

            if (wrrall.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrrall);
            }

            foreach (DataRow dr8 in wrrallsumTmp.Select())
            {
                if (dr8.RowState != DataRowState.Deleted)
                {

                    DataRow drSitmall = wrrall.NewRow();
                    drSitmall["shiftcode"] = dr8["shiftcode"];
                    drSitmall["monday"] = dr8["monday"];
                    drSitmall["tuesday"] = dr8["tuesday"];
                    drSitmall["wednesday"] = dr8["wednesday"];
                    drSitmall["thursday"] = dr8["thursday"];
                    drSitmall["friday"] = dr8["friday"];
                    drSitmall["saturday"] = dr8["saturday"];
                    drSitmall["sunday"] = dr8["sunday"];
                    wrrall.Rows.Add(drSitmall);
                }
            }


            wrr1sumTmp.Dispose();
            wrr2sumTmp.Dispose();
            wrr3sumTmp.Dispose();
            wrr4sumTmp.Dispose();
            wrrallsumTmp.Dispose();
            updateATMR();
            columnChanged = false;
            scheduleChanged = false;

        }

        #endregion

        #region GetTableMaxID

        private int GetTableMaxID(string Tablename)
        {
            string sql1 = "Select ISNULL(MAX(id),0) as maxid from " + Tablename + "";

            this.dbaccess.ReadSQL("Result1", sql1);
            DataRow dr = this.dbaccess.DataSet.Tables["Result1"].Rows[0];

            return Convert.ToInt32(dr["maxid"]);

        }

        #endregion

        #region GetTableMinID

        private int GetTableMinID(string Tablename, string sitenumt)
        {
            string sql1 = "Select ISNULL(Min(id),0) as minid from " + Tablename + " where sitmt='" + sitenumt + "'";

            this.dbaccess.ReadSQL("Result2", sql1);
            DataRow dr = this.dbaccess.DataSet.Tables["Result2"].Rows[0];

            return Convert.ToInt32(dr["minid"]);

        }

        #endregion

        #region Get Sitm Data

        private void GetSitm()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];

            string GetSitmSQL = "Select sitenum, sectorcode,addr1,addr2,addr3,postalcode,tel1,fax, rep1, rep1tel,rep2, rep2tel, " +
                                "sectorcode,[event],schedule, " +
                                "officerqty,contractdate,commencedate,enddate,issuedby " +
                                "from sitmt where sitenumt ='" + wrr["sitenumt"].ToString() + "'";

            this.dbaccess.ReadSQL("TmpSitm", GetSitmSQL);
            DataTable TmpSitm = this.dbaccess.DataSet.Tables["TmpSitm"];

            if (TmpSitm.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TmpSitm.Select())
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        wrr["sitenum"] = dr1["sitenum"].ToString();
                        wrr["sectorcode"] = dr1["sectorcode"].ToString();
                        wrr["addr1"] = dr1["addr1"].ToString();
                        wrr["addr2"] = dr1["addr2"].ToString();
                        wrr["addr3"] = dr1["addr3"].ToString();
                        wrr["postalcode"] = dr1["postalcode"].ToString();
                        wrr["tel1"] = dr1["tel1"].ToString();
                        wrr["fax"] = dr1["fax"].ToString();
                        wrr["rep1"] = dr1["rep1"].ToString();
                        wrr["rep1tel"] = dr1["rep1tel"].ToString();
                        wrr["rep2"] = dr1["rep2"].ToString();
                        wrr["rep2tel"] = dr1["rep2tel"].ToString();
                        wrr["sectorcode"] = dr1["sectorcode"].ToString();
                        wrr["event"] = dr1["event"].ToString();
                        wrr["schedule"] = dr1["schedule"].ToString();
                        //wrr["isdaily"] = dr1["isdaily"].ToString();
                        //wrr["isweekdays"] = dr1["isweekdays"].ToString();
                        //wrr["isweekend"] = dr1["isweekend"].ToString();
                        //wrr["ispubhol"] = dr1["ispubhol"].ToString();
                        //wrr["monday"] = dr1["monday"].ToString();
                        //wrr["tuesday"] = dr1["tuesday"].ToString();
                        //wrr["wednesday"] = dr1["wednesday"].ToString();
                        //wrr["thursday"] = dr1["thursday"].ToString();
                        //wrr["friday"] = dr1["friday"].ToString();
                        //wrr["saturday"] = dr1["saturday"].ToString();
                        //wrr["sunday"] = dr1["sunday"].ToString();
                        wrr["officerqty"] = dr1["officerqty"];
                        wrr["contractdate"] = dr1["contractdate"];
                        wrr["commencedate"] = System.DBNull.Value;
                        wrr["enddate"] = System.DBNull.Value;
                        wrr["issuedby"] = dr1["issuedby"].ToString();

                    }
                }
            }

        }

        #endregion

        #region Get Sitm1 Data

        private void GetSitm1()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = this.dbaccess.DataSet.Tables["wrr1"];

            // Change
            //string GetSitm1SQL = "Select sitenumt ,empnum,empname,matnum,shiftcode, " +
            //                     "CASE WHEN isnull(monday,'') = 'X' then shiftcode else '' end as monday, " +
            //                     "CASE WHEN isnull(tuesday,'') = 'X' then shiftcode else '' end as tuesday, " +
            //                     "CASE WHEN isnull(wednesday,'') = 'X' then shiftcode else '' end as wednesday, " +
            //                     "CASE WHEN isnull(thursday,'') = 'X' then shiftcode else '' end as thursday, " +
            //                     "CASE WHEN isnull(friday,'') = 'X' then shiftcode else '' end as friday, " +
            //                     "CASE WHEN isnull(saturday,'') = 'X' then shiftcode else '' end as saturday, " +
            //                     "CASE WHEN isnull(sunday,'') = 'X' then shiftcode else '' end as sunday " +
            //                     "from sitmt1 where sitenumt ='" + wrr["sitenumt"].ToString() + "'";

            string GetSitm1SQL = "Select sitenumt ,empnum,empname,matnum, " +
                              "xday1 as monday, " +
                              "xday2 as tuesday, " +
                              "xday3 as wednesday, " +
                              "xday4 as thursday, " +
                              "xday5 as friday, " +
                              "xday6 as saturday, " +
                              "xday7 as sunday " +
                              "from sitmt8 where sitenumt ='" + wrr["sitenumt"].ToString() + "'";

            this.dbaccess.ReadSQL("TmpSitm1", GetSitm1SQL);
            DataTable TmpSitm1 = this.dbaccess.DataSet.Tables["TmpSitm1"];

            if (TmpSitm1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr1);

                foreach (DataRow dr in TmpSitm1.Select())
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWRR1 = wrr1.NewRow();
                        InsertWRR1["empnum"] = dr["empnum"];
                        InsertWRR1["empname"] = dr["empname"];
                        InsertWRR1["matnum"] = dr["matnum"];
                        //InsertWRR1["shiftcode"] = dr["shiftcode"];
                        InsertWRR1["monday"] = dr["monday"];
                        InsertWRR1["tuesday"] = dr["tuesday"];
                        InsertWRR1["wednesday"] = dr["wednesday"];
                        InsertWRR1["thursday"] = dr["thursday"];
                        InsertWRR1["friday"] = dr["friday"];
                        InsertWRR1["saturday"] = dr["saturday"];
                        InsertWRR1["sunday"] = dr["sunday"];
                        wrr1.Rows.Add(InsertWRR1);
                        //if (!BizFunctions.IsEmpty(dr["monday"]))
                        //{
                        //    wrr1_dayshiftcode.Text = dr["monday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["tuesday"]))
                        //{
                        //    wrr1_dayshiftcode.Text = dr["tuesday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["wednesday"]))
                        //{
                        //    wrr1_dayshiftcode.Text = dr["wednesday"].ToString();
                        //}
                    }

                }


            }
        }


        private void GetSitm1All()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr1 = this.dbaccess.DataSet.Tables["wrr1"];

            // Change
            //string GetSitm1SQL = "Select sitenumt ,empnum,empname,matnum,shiftcode, " +
            //                     "CASE WHEN isnull(monday,'') = 'X' then shiftcode else '' end as monday, " +
            //                     "CASE WHEN isnull(tuesday,'') = 'X' then shiftcode else '' end as tuesday, " +
            //                     "CASE WHEN isnull(wednesday,'') = 'X' then shiftcode else '' end as wednesday, " +
            //                     "CASE WHEN isnull(thursday,'') = 'X' then shiftcode else '' end as thursday, " +
            //                     "CASE WHEN isnull(friday,'') = 'X' then shiftcode else '' end as friday, " +
            //                     "CASE WHEN isnull(saturday,'') = 'X' then shiftcode else '' end as saturday, " +
            //                     "CASE WHEN isnull(sunday,'') = 'X' then shiftcode else '' end as sunday " +
            //                     "from sitmt1 where sitenumt ='" + wrr["sitenumt"].ToString() + "'";

            string GetSitm1SQL = "Select sitenumt ,empnum,empname,matnum,ctrnum,sitenum, " +
                              "xday1 as monday, " +
                              "xday2 as tuesday, " +
                              "xday3 as wednesday, " +
                              "xday4 as thursday, " +
                              "xday5 as friday, " +
                              "xday6 as saturday, " +
                              "xday7 as sunday " +
                              "from sitmt8 where [status]<>'V' and sitenumt<>'ALLOUTLETS'";

            this.dbaccess.ReadSQL("TmpSitm1", GetSitm1SQL);
            DataTable TmpSitm1 = this.dbaccess.DataSet.Tables["TmpSitm1"];

            if (TmpSitm1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr1);

                foreach (DataRow dr in TmpSitm1.Select())
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWRR1 = wrr1.NewRow();
                        InsertWRR1["empnum"] = dr["empnum"];
                        InsertWRR1["empname"] = dr["empname"];
                        InsertWRR1["matnum"] = dr["matnum"];
                        //InsertWRR1["shiftcode"] = dr["shiftcode"];
                        InsertWRR1["monday"] = dr["monday"];
                        InsertWRR1["tuesday"] = dr["tuesday"];
                        InsertWRR1["wednesday"] = dr["wednesday"];
                        InsertWRR1["thursday"] = dr["thursday"];
                        InsertWRR1["friday"] = dr["friday"];
                        InsertWRR1["saturday"] = dr["saturday"];
                        InsertWRR1["sunday"] = dr["sunday"];
                        InsertWRR1["sitenumt"] = dr["sitenumt"];
                        InsertWRR1["docunum"] = dr["ctrnum"];
                        InsertWRR1["sitenum"] = dr["sitenum"];
                        wrr1.Rows.Add(InsertWRR1);
                        //if (!BizFunctions.IsEmpty(dr["monday"]))
                        //{
                        //    wrr1_dayshiftcode.Text = dr["monday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["tuesday"]))
                        //{
                        //    wrr1_dayshiftcode.Text = dr["tuesday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["wednesday"]))
                        //{
                        //    wrr1_dayshiftcode.Text = dr["wednesday"].ToString();
                        //}
                    }

                }


            }
        }

        #endregion

        #region Get Sitm2 Data

        private void GetSitm2()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr2 = this.dbaccess.DataSet.Tables["wrr2"];


            string GetSitm2SQL = "Select sitenumt ,empnum,empname,matnum,shiftcode, " +
                                   "CASE WHEN isnull(monday,'') = 'X' then shiftcode else '' end as monday, " +
                                   "CASE WHEN isnull(tuesday,'') = 'X' then shiftcode else '' end as tuesday, " +
                                   "CASE WHEN isnull(wednesday,'') = 'X' then shiftcode else '' end as wednesday, " +
                                   "CASE WHEN isnull(thursday,'') = 'X' then shiftcode else '' end as thursday, " +
                                   "CASE WHEN isnull(friday,'') = 'X' then shiftcode else '' end as friday, " +
                                   "CASE WHEN isnull(saturday,'') = 'X' then shiftcode else '' end as saturday, " +
                                   "CASE WHEN isnull(sunday,'') = 'X' then shiftcode else '' end as sunday " +
                                   "from sitmt2 where sitenumt ='" + wrr["sitenumt"].ToString() + "'";

            this.dbaccess.ReadSQL("TmpSitm2", GetSitm2SQL);
            DataTable TmpSitm2 = this.dbaccess.DataSet.Tables["TmpSitm2"];

            if (TmpSitm2.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr2);

                foreach (DataRow dr in TmpSitm2.Select())
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWRR2 = wrr2.NewRow();
                        InsertWRR2["empnum"] = dr["empnum"];
                        InsertWRR2["empname"] = dr["empname"];
                        InsertWRR2["matnum"] = dr["matnum"];
                        InsertWRR2["shiftcode"] = dr["shiftcode"];
                        InsertWRR2["monday"] = dr["monday"];
                        InsertWRR2["tuesday"] = dr["tuesday"];
                        InsertWRR2["wednesday"] = dr["wednesday"];
                        InsertWRR2["thursday"] = dr["thursday"];
                        InsertWRR2["friday"] = dr["friday"];
                        InsertWRR2["saturday"] = dr["saturday"];
                        InsertWRR2["sunday"] = dr["sunday"];
                        wrr2.Rows.Add(InsertWRR2);


                        //if (!BizFunctions.IsEmpty(dr["monday"]))
                        //{
                        //    wrr2_nightshiftcode.Text = dr["monday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["tuesday"]))
                        //{
                        //    wrr2_nightshiftcode.Text = dr["tuesday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["wednesday"]))
                        //{
                        //    wrr2_nightshiftcode.Text = dr["wednesday"].ToString();
                        //}
                    }

                }
            }
        }

        #endregion

        #region Get Sitm3 Data

        private void GetSitm3()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr3 = this.dbaccess.DataSet.Tables["wrr3"];

            string GetSitm3SQL = "Select sitenumt ,empnum,empname,matnum,shiftcode, " +
                                  "CASE WHEN isnull(monday,'') = 'X' then shiftcode else '' end as monday, " +
                                  "CASE WHEN isnull(tuesday,'') = 'X' then shiftcode else '' end as tuesday, " +
                                  "CASE WHEN isnull(wednesday,'') = 'X' then shiftcode else '' end as wednesday, " +
                                  "CASE WHEN isnull(thursday,'') = 'X' then shiftcode else '' end as thursday, " +
                                  "CASE WHEN isnull(friday,'') = 'X' then shiftcode else '' end as friday, " +
                                  "CASE WHEN isnull(saturday,'') = 'X' then shiftcode else '' end as saturday, " +
                                  "CASE WHEN isnull(sunday,'') = 'X' then shiftcode else '' end as sunday " +
                                  "from sitmt3 where sitenumt ='" + wrr["sitenumt"].ToString() + "'";

            this.dbaccess.ReadSQL("TmpSitm3", GetSitm3SQL);
            DataTable TmpSitm3 = this.dbaccess.DataSet.Tables["TmpSitm3"];

            if (TmpSitm3.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr3);

                foreach (DataRow dr in TmpSitm3.Select())
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWRR3 = wrr3.NewRow();
                        InsertWRR3["empnum"] = dr["empnum"];
                        InsertWRR3["empname"] = dr["empname"];
                        InsertWRR3["matnum"] = dr["matnum"];
                        InsertWRR3["shiftcode"] = dr["shiftcode"];
                        InsertWRR3["monday"] = dr["monday"];
                        InsertWRR3["tuesday"] = dr["tuesday"];
                        InsertWRR3["wednesday"] = dr["wednesday"];
                        InsertWRR3["thursday"] = dr["thursday"];
                        InsertWRR3["friday"] = dr["friday"];
                        InsertWRR3["saturday"] = dr["saturday"];
                        InsertWRR3["sunday"] = dr["sunday"];
                        wrr3.Rows.Add(InsertWRR3);

                        //if (!BizFunctions.IsEmpty(dr["monday"]))
                        //{
                        //    wrr3_concierge.Text = dr["monday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["tuesday"]))
                        //{
                        //    wrr3_concierge.Text = dr["tuesday"].ToString();
                        //}
                        //if (!BizFunctions.IsEmpty(dr["wednesday"]))
                        //{
                        //    wrr3_concierge.Text = dr["wednesday"].ToString();
                        //}
                    }

                }
            }

        }

        #endregion

        #region Get Sitm4 Data

        private void GetSitm4()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr4 = this.dbaccess.DataSet.Tables["wrr4"];


            string GetSitm4SQL = "select empnum, empname, matnum from sitmt4 where sitenumt ='" + wrr["sitenumt"].ToString() + "'";

            this.dbaccess.ReadSQL("TmpSitm4", GetSitm4SQL);
            DataTable TmpSitm4 = this.dbaccess.DataSet.Tables["TmpSitm4"];

            if (TmpSitm4.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr4);

                foreach (DataRow dr in TmpSitm4.Select())
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWRR4 = wrr4.NewRow();
                        InsertWRR4["empnum"] = dr["empnum"];
                        InsertWRR4["empname"] = dr["empname"];
                        InsertWRR4["matnum"] = dr["matnum"];
                        wrr4.Rows.Add(InsertWRR4);
                    }

                }
            }

        }


        private void GetALLSitm4()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            DataTable wrr4 = this.dbaccess.DataSet.Tables["wrr4"];


            string GetSitm4SQL = "select empnum, empname, matnum,sitenumt,sitenum from sitmt4 where sitenumt in "+
                                    "( "+
                                    "select sitenumt from wrr where [status]<>'V' "+
                                    ") ";

            this.dbaccess.ReadSQL("TmpSitm4", GetSitm4SQL);
            DataTable TmpSitm4 = this.dbaccess.DataSet.Tables["TmpSitm4"];

            if (TmpSitm4.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wrr4);

                foreach (DataRow dr in TmpSitm4.Select())
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWRR4 = wrr4.NewRow();
                        InsertWRR4["empnum"] = dr["empnum"];
                        InsertWRR4["empname"] = dr["empname"];
                        InsertWRR4["matnum"] = dr["matnum"];
                        InsertWRR4["sitenum"] = dr["sitenum"];
                        InsertWRR4["sitenumt"] = dr["sitenumt"];
                        InsertWRR4["sitename"] = GetSitename(dr["sitenum"].ToString());
                        wrr4.Rows.Add(InsertWRR4);
                    }

                }
            }

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

        #region Check WRR Shift

        //private bool CheckWRRShift(DataRow dr, string rowname, string tablename)
        //{
        //    bool check;

        //    if (tablename == "wrr1")
        //    {

        //        if (dr[rowname] != wrr1_dayshiftcode.Text.ToString().Trim()) ;
        //        {
        //            check = true;
        //        }

        //    }

        //    else if (tablename == "wrr2")
        //    {
        //        if (dr[rowname] != wrr2_nightshiftcode.Text.ToString().Trim()) ;
        //        {
        //            check = true;
        //        }

        //    }

        //    else if (tablename == "wrr3")
        //    {
        //        if (dr[rowname] != wrr3_concierge.Text.ToString().Trim()) ;
        //        {
        //            check = true;
        //        }

        //    }
        //    else
        //    {
        //        check = false;
        //    }



        //    return check;


        //}

        #endregion

        #region Default Message

        private void Msg()
        {
            MessageBox.Show("Shift Code is not the same as The Main Shiftcode specified", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        #region Update ATMR Table

        private void updateATMR()
        {
            string from, to = "";

            DataRow wrr = dbaccess.DataSet.Tables["WRR"].Rows[0];
            DataTable wrr1 = dbaccess.DataSet.Tables["wrr1"];
            DataTable wrr2 = dbaccess.DataSet.Tables["wrr2"];
            DataTable wrr3 = dbaccess.DataSet.Tables["wrr3"];
            DataTable wrr4 = dbaccess.DataSet.Tables["wrr4"];
            DataTable atmr = dbaccess.DataSet.Tables["atmr"];


            from = BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr["commencedate"]));
            to = BizFunctions.GetSafeDateString(Convert.ToDateTime(wrr["enddate"]));



            DataTable ATMRtmp = GetATMRpivot(wrr["refnum"].ToString());

            DataTable tmpWeek = TimeTools.WeekTable(from, to);

            if (this.dbaccess.DataSet.Tables.Contains("weekTB"))
            {
                DataTable WeekTB = this.dbaccess.DataSet.Tables["WeekTB"];
                this.dbaccess.DataSet.Tables.Remove(WeekTB);
                tmpWeek.TableName = "weekTB";
                this.dbaccess.DataSet.Tables.Add(tmpWeek);
            }
            else
            {
                tmpWeek.TableName = "weekTB";
                this.dbaccess.DataSet.Tables.Add(tmpWeek);
            }

            if (!ATMRtmp.Columns.Contains("date"))
            {
                ATMRtmp.Columns.Add("Date", typeof(DateTime));
            }


            string GetAtmr = "Select * from tmpAtmr where shiftcode<>''";

            DataTable FinalAMTR = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetAtmr);

            if (this.dbaccess.DataSet.Tables.Contains("FinalAMTR"))
            {

                this.dbaccess.DataSet.Tables.Remove("FinalAMTR");
                FinalAMTR.TableName = "FinalAMTR";
                this.dbaccess.DataSet.Tables.Add(FinalAMTR);
            }
            else
            {
                FinalAMTR.TableName = "FinalAMTR";
                this.dbaccess.DataSet.Tables.Add(FinalAMTR);
            }


            string GetWeekTB = "Select * from weekTB where [date] <='" + wrr["enddate"].ToString() + "'";

            DataTable FinalWeekTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetWeekTB);

            if (this.dbaccess.DataSet.Tables.Contains("FinalWeekTB"))
            {

                this.dbaccess.DataSet.Tables.Remove("FinalWeekTB");
                FinalWeekTB.TableName = "FinalWeekTB";
                this.dbaccess.DataSet.Tables.Add(FinalWeekTB);
            }
            else
            {
                FinalWeekTB.TableName = "FinalWeekTB";
                this.dbaccess.DataSet.Tables.Add(FinalWeekTB);
            }



            string FinalSchedule = "Select FW.REFNUM,FW.TB,FW.EMPNUM,FW.NRIC,FW.SHIFTCODE,FW.[DAY],FW.TIMEIN,FW.[TIMEOUT],FB.[DATE], FW.ISOT,FW.OTRATE,FW.ISDRE,FW.ISOFFSET,FW.ISUS,FW.ISRD,FW.REMARK,FW.isRealSchedule,FW.sitenum,FW.docunum FROM FinalAMTR FW " +
                                    "RIGHT JOIN  FinalWeekTB FB on FW.[day]=FB.[day]";

            DataTable FinalSched = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, FinalSchedule);

            if (FinalSched.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(atmr);

                foreach (DataRow dr1 in FinalSched.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {

                        if (!BizFunctions.IsEmpty(dr1["refnum"]) || !BizFunctions.IsEmpty(dr1["empnum"]))
                        {
                            DataRow DRnew = atmr.NewRow();
                            DRnew["uniquekey"] = dr1["empnum"].ToString().Trim() + "-" + dr1["shiftcode"].ToString().Trim() +"-"+ BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["Date"]));
                            DRnew["empnum"] = dr1["empnum"];
                            DRnew["nric"] = Tools.GetNRIC(dr1["empnum"].ToString());
                            DRnew["shiftcode"] = dr1["shiftcode"];
                            if (wrr["sitenumt"].ToString().Contains("ALL"))
                            {
                                DRnew["sitenum"] = dr1["sitenum"];
                            }
                            else
                            {
                                DRnew["sitenum"] = wrr["sitenum"];  // MUst change
                            }
                            DRnew["sectorcode"] = wrr["sectorcode"];
                            DRnew["status"] = wrr["status"];
                            DRnew["day"] = dr1["day"];
                            DRnew["timein"] = ShiftTimein(dr1["day"].ToString(), dr1["shiftcode"].ToString());
                            DRnew["timeout"] = ShiftTimeOut(dr1["day"].ToString(), dr1["shiftcode"].ToString());
                            DRnew["date"] = dr1["date"];
                            DRnew["isot"] = dr1["isot"];
                            DRnew["otrate"] = dr1["otrate"];
                            DRnew["isRealSchedule"] = dr1["isRealSchedule"];
                            DRnew["isDRE"] = dr1["isDRE"];
                            DRnew["isOffSet"] = dr1["isOffSet"];
                            DRnew["isUS"] = dr1["isUS"];
                            DRnew["isRD"] = dr1["isRD"];
                            DRnew["docunum"] = dr1["docunum"];
                           
                            atmr.Rows.Add(DRnew);
                        }

                    }
                }
            }
        }

        #endregion

        #region If Contains Schedule

        private bool hasSchedule(string empnum)
        {
            bool isTaken = false;

            return isTaken;
        }

        #endregion

        #region Get ATMR pivot
        // Must check this
        private DataTable GetATMRpivot(string refnum)
        {
            string sql1 = "";

            //sql1 = "SELECT "+ 
            //            "M3.refnum,M3.TB,M3.EMPNUM,M3.NRIC, M3.shiftcode, "+
            //            "M3.[day],vS.TIMEIN,vS.[TIMEOUT],M3.remark,M3.isRealSchedule ,M3.isOT,M3.OTrate,M3.isDRE,M3.isRD,M3.isOffSet,M3.isUS " +
            //        "FROM "+
            //        "( "+
            //               " SELECT "+
            //                 "M2.refnum,M2.TB,M2.EMPNUM,M2.NRIC, "+
            //                 "CASE WHEN CHARINDEX('/',M2.shiftcode) > 0 then LEFT(ISNULL(M2.shiftcode,''),(CHARINDEX('/',ISNULL(M2.shiftcode,'')))-1) else M2.shiftcode end as shiftcode, "+
            //                 "M2.[day], "+
            //                 "M2.remark, "+
            //                 "M2.isRealSchedule, "+
            //                 "CASE WHEN CHARINDEX('OT',M2.shiftcode) > 0 then 1 else 0 end as isOT, "+
            //                 "CASE WHEN ((CHARINDEX('OT',M2.shiftcode) > 0) AND (REPLACE(REPLACE(SUBSTRING(M2.shiftcode,CHARINDEX('/',ISNULL(M2.shiftcode,''))+1,LEN(M2.shiftcode)),'OT',''),'X',''))<>'') THEN REPLACE(REPLACE(SUBSTRING(M2.shiftcode,CHARINDEX('/',ISNULL(M2.shiftcode,''))+1,LEN(M2.shiftcode)),'OT',''),'X','') ELSE null end as OTrate, " +
            //                 "CASE WHEN CHARINDEX('DRE',M2.shiftcode) > 0 then 1 else 0 end as isDRE, "+
            //                 "CASE WHEN CHARINDEX('RD',M2.shiftcode) > 0 then 1 else 0 end as isRD, "+
            //                 "CASE WHEN (CHARINDEX('OS',M2.shiftcode) > 0 OR CHARINDEX('OFFSET',M2.shiftcode) > 0) then 1 else 0 end as isOffSet, " +
            //                 "CASE WHEN (CHARINDEX('US',M2.shiftcode) > 0 OR CHARINDEX('UD',M2.shiftcode) > 0)  then 1 else 0 end as isUS " +
            //            "FROM "+
            //            "( "+
            //                "SELECT M1.refnum,M1.TB,M1.EMPNUM,M1.NRIC, M1.shiftcode, "+
            //                    "M1.[day],M1.remark,M1.isRealSchedule ,NULL as isOT, NULL as OTrate "+
            //                    "FROM "+
            //                    "( "+
            //                    "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule from  "+
            //                    "( "+
            //                       "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,'wrr1' as TB from wrr1 where refnum='" + refnum + "'" + 
            //                    ") as p UNPIVOT  "+
            //                    "(  "+
            //                        "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) "+
            //                    ")unP  "+
                    			
            //                    "UNION ALL "+
                    			
            //                    "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule  from "+
            //                    "( "+
            //                        "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,'wrr2' as TB  from wrr2 where refnum='" + refnum + "'" +
            //                    ") as p UNPIVOT "+
            //                    "( "+
            //                        "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY)  "+
            //                    ")unP "+
                    			
            //                    "UNION ALL "+
                    			
            //                    "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule  from "+
            //                    "( "+
            //                        "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,'wrr3' as TB  from wrr3 where refnum='" + refnum + "'" +
            //                    ") as p UNPIVOT "+
            //                    "( "+
            //                        "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) "+
            //                    ")unP "+
                    			
            //                    "UNION ALL "+
                    			
            //                    "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule  from "+
            //                    "( "+
            //                        "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,'wrr4' as TB  from wrr4 where refnum='" + refnum + "'" +
            //                    ") as p UNPIVOT "+
            //                    "( "+
            //                    "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) "+
            //                    ")unP "+
            //                ")M1 "+
            //            ")M2 "+
            //        ")M3 "+
            //        "LEFT JOIN vSHLV vS on M3.shiftcode=vS.SHIFTCODE";

            sql1 = "SELECT " +
                       "M3.refnum,M3.TB,M3.EMPNUM,M3.NRIC, M3.shiftcode, " +
                       "M3.[day],vS.TIMEIN,vS.[TIMEOUT],M3.remark,M3.isRealSchedule,M3.sitenum,M3.docunum ,M3.isOT,M3.OTrate,M3.isDRE,M3.isRD,M3.isOffSet,M3.isUS " +
                   "FROM " +
                   "( " +
                          " SELECT " +
                            "M2.refnum,M2.TB,M2.EMPNUM,M2.NRIC, " +
                            "CASE WHEN CHARINDEX('/',M2.shiftcode) > 0 then LEFT(ISNULL(M2.shiftcode,''),(CHARINDEX('/',ISNULL(M2.shiftcode,'')))-1) else M2.shiftcode end as shiftcode, " +
                            "M2.[day], " +
                            "M2.remark, " +
                            "M2.isRealSchedule, " +
                            "M2.sitenum, " +
                            "M2.docunum, " +
                            "CASE WHEN CHARINDEX('OT',M2.shiftcode) > 0 then 1 else 0 end as isOT, " +
                            "CASE WHEN ((CHARINDEX('OT',M2.shiftcode) > 0) AND (REPLACE(REPLACE(SUBSTRING(M2.shiftcode,CHARINDEX('/',ISNULL(M2.shiftcode,''))+1,LEN(M2.shiftcode)),'OT',''),'X',''))<>'') THEN REPLACE(REPLACE(SUBSTRING(M2.shiftcode,CHARINDEX('/',ISNULL(M2.shiftcode,''))+1,LEN(M2.shiftcode)),'OT',''),'X','') ELSE null end as OTrate, " +
                            "CASE WHEN CHARINDEX('DRE',M2.shiftcode) > 0 then 1 else 0 end as isDRE, " +
                            "CASE WHEN CHARINDEX('RD',M2.shiftcode) > 0 then 1 else 0 end as isRD, " +
                            "CASE WHEN (CHARINDEX('OS',M2.shiftcode) > 0 OR CHARINDEX('OFFSET',M2.shiftcode) > 0) then 1 else 0 end as isOffSet, " +
                            "CASE WHEN (CHARINDEX('US',M2.shiftcode) > 0 OR CHARINDEX('UD',M2.shiftcode) > 0)  then 1 else 0 end as isUS " +
                       "FROM " +
                       "( " +
                           "SELECT M1.refnum,M1.TB,M1.EMPNUM,M1.NRIC, M1.shiftcode, " +
                               "M1.[day],M1.remark,M1.isRealSchedule,sitenum,docunum ,NULL as isOT, NULL as OTrate " +
                               "FROM " +
                               "( " +
                               "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule,sitenum,docunum from  " +
                               "( " +
                                  "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,sitenum,docunum, 'wrr1' as TB from wrr1 where refnum='" + refnum + "'" +
                               ") as p UNPIVOT  " +
                               "(  " +
                                   "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) " +
                               ")unP  " +

                               "UNION ALL " +

                               "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule,sitenum,docunum  from " +
                               "( " +
                                   "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,sitenum,docunum,'wrr2' as TB  from wrr2 where refnum='" + refnum + "'" +
                               ") as p UNPIVOT " +
                               "( " +
                                   "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY)  " +
                               ")unP " +

                               "UNION ALL " +

                               "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule,sitenum,docunum  from " +
                               "( " +
                                   "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,sitenum,docunum,'wrr3' as TB  from wrr3 where refnum='" + refnum + "'" +
                               ") as p UNPIVOT " +
                               "( " +
                                   "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) " +
                               ")unP " +

                               "UNION ALL " +

                               "Select unP.refnum,TB,unP.empnum,unP.nric,shiftcode,[day],remark,isRealSchedule,sitenum,docunum  from " +
                               "( " +
                                   "select refnum,empnum,nric,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,remark,isRealSchedule,sitenum,docunum,'wrr4' as TB  from wrr4 where refnum='" + refnum + "'" +
                               ") as p UNPIVOT " +
                               "( " +
                               "shiftcode for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) " +
                               ")unP " +
                           ")M1 " +
                       ")M2 " +
                   ")M3 " +
                   "LEFT JOIN vSHLV vS on M3.shiftcode=vS.SHIFTCODE";

            this.dbaccess.ReadSQL("tmpAtmr", sql1);

            DataTable tmpAtmr = this.dbaccess.DataSet.Tables["tmpAtmr"];

            return tmpAtmr;
        }

        #endregion

        #region Allow Schedule Insert

        private bool AllowScheduleInsert(string matnum, string shiftcode, string day, string Tablename)
        {

            string GetInfo = "Select shiftcode,matnum,[day],ISNULL(sum(total),0) as Total from " +
                                "( " +
                                "SELECT shiftcode,matnum,'" + day + "' as [day], " +
                                    "CASE " +
                                        "WHEN ISNULL(" + day + ",'')='X' THEN 1 " +
                                            " WHEN " + day + "='O' OR " + day + "='' THEN 0 " +
                                        "ELSE 0 " +
                                        "END as  Total " +
                                    "from " + Tablename + " where shiftcode='" + shiftcode + "' and matnum='" + matnum + "' " +
                                ")A " +
                                "group by shiftcode,matnum,[day]";
            int total = 0;
            bool allow = false;
            DataTable tmpGetInfo = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetInfo);

            if (tmpGetInfo.Rows.Count > 0)
            {
                DataRow drTmpGetInfo = tmpGetInfo.Rows[0];

                foreach (DataRow dr1 in sc.ScheduleInfo.Rows)
                {
                    if (dr1["day"].ToString().Trim() == drTmpGetInfo["day"].ToString().Trim() && dr1["shiftcode"].ToString().Trim() == drTmpGetInfo["shiftcode"].ToString().Trim() && dr1["matnum"].ToString().Trim() == drTmpGetInfo["matnum"].ToString().Trim())
                    {
                        total = Convert.ToInt32(dr1["total"]) - Convert.ToInt32(drTmpGetInfo["total"]);

                        break;
                    }
                }

            }

            if (total < 0)
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

        #region Calculate Day of the Week
        private int EmpTotalofDay(string day, string tablename)
        {
            int total = 0;
            int value = 0;

            DataTable Table = dbaccess.DataSet.Tables[tablename];
            if (Table.Rows.Count > 0)
            {
                string sql = "Select shiftcode,isWorkShift from vSHLV";
                this.dbaccess.ReadSQL("vSHLVtmp", sql);

                foreach (DataRow dr1 in Table.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1[day]))
                        {
                            //DataTable dtVshlvTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select shiftcode from vSHLVtmp  where shiftcode='" + dr1[day].ToString() + "' and isWorkShift=1");
                            DataTable dtVshlvTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select shiftcode,isWorkShift from vSHLVtmp  where shiftcode='" + GetShiftCode(dr1[day].ToString()) + "' and isWorkShift=1");


                            if(dtVshlvTmp.Rows.Count > 0)
                            {
                                value = Convert.ToInt32(dtVshlvTmp.Rows[0]["isWorkShift"]);
                                //value = 1;
                            }
                            else
                            {
                                value = 0;
                            }

                            dtVshlvTmp.Dispose();
                        }
                        else
                        {
                            value = 0;
                        }

                        total = total + value;

                        if (isUnderStudy(dr1[day].ToString()))
                        {
                            total = total - 1;
                        }

                        
                    }
                }
                this.dbaccess.DataSet.Tables["vSHLVtmp"].Dispose();
            }

            
            return total;
        }
        #endregion

        #region Is UnderStudy

        private bool isUnderStudy(string shiftcode)
        {
            bool isUS = false;
            string str_Tmp1 = shiftcode.Trim();
            int str_Tmp1MaxLength = shiftcode.Length;
            string newShiftcode = "";
            int newLastDigit = 0;
            int lastcharcount = 0;
            string newLastDigitStr = "";
            int target = 0;
            int str_Index = 0;
            bool hasSlash = false;
            if (str_Tmp1.Trim().Length != 0)
            {

                foreach (char c in str_Tmp1.Trim())
                {
                    str_Index = str_Index + 1;
                    if (c == '/')
                    {
                        hasSlash = true;
                       str_Tmp1MaxLength = str_Tmp1MaxLength - str_Index;
                        break;
                    }
                }

                if (hasSlash)
                {
                    newShiftcode = str_Tmp1.Substring((str_Index), str_Tmp1MaxLength);
                }
            }

            int SearchForUstudy1 = newShiftcode.IndexOf("US");
            int SearchForUstudy2 = newShiftcode.IndexOf("UD");
            int SearchForOJT = newShiftcode.IndexOf("OJT");

            if (SearchForUstudy1 >= 0 || SearchForUstudy2 >= 0 || SearchForOJT >= 0)
            {
                isUS = true;
            }

            return isUS;
        }

        #endregion

        #region Get ShiftCode

        private string GetShiftCode(string shiftcode)
        {

            string str_Tmp1 = shiftcode.Trim();
            string newShiftcode = "";
            int newLastDigit = 0;
            int lastcharcount = 0;
            string newLastDigitStr = "";
            int target = 0;
            int str_Index = 0;
            bool hasSlash = false;
            if (str_Tmp1.Trim().Length != 0)
            {
               
                foreach (char c in str_Tmp1.Trim())
                {
                    str_Index = str_Index + 1;
                    if (c == '/')
                    {
                        hasSlash = true;
                        break;
                            //newLastDigit = Convert.ToInt32(str_Tmp1.Substring(str_Index)) + 1;
                            //lastcharcount = str_Index - 2;

                            //if (newLastDigit < 10)
                            //{
                            //    newLastDigitStr = "00" + Convert.ToString(newLastDigit);
                            //}
                            //else
                            //{
                            //    newLastDigitStr = "0" + Convert.ToString(newLastDigit);
                            //}


                        
                    }
                }
                newShiftcode = str_Tmp1.Substring(0, str_Index-1);
            }
            if (!hasSlash)
            {
                newShiftcode = shiftcode;
            }

            return newShiftcode;
        }

        #endregion

        #region Get Footer Totals

        private void Getwrr1FooterTotals()
        {
            wrr1Total_monday.Text = Convert.ToString((EmpTotalofDay("monday", "wrr1") + EmpTotalReliefofDay("monday", "wrr4","D")) - EmpTotalBufferofDay("monday", "wrr1", "isoptional"));
            wrr1Total_tuesday.Text = Convert.ToString((EmpTotalofDay("tuesday", "wrr1") + EmpTotalReliefofDay("tuesday", "wrr4", "D")) - EmpTotalBufferofDay("tuesday", "wrr1", "isoptional"));
            wrr1Total_wednesday.Text = Convert.ToString((EmpTotalofDay("wednesday", "wrr1") + EmpTotalReliefofDay("wednesday", "wrr4", "D")) - EmpTotalBufferofDay("wednesday", "wrr1", "isoptional"));
            wrr1Total_thursday.Text = Convert.ToString((EmpTotalofDay("thursday", "wrr1") + EmpTotalReliefofDay("thursday", "wrr4", "D")) - EmpTotalBufferofDay("thursday", "wrr1", "isoptional"));
            wrr1Total_friday.Text = Convert.ToString((EmpTotalofDay("friday", "wrr1") + EmpTotalReliefofDay("friday", "wrr4", "D")) - EmpTotalBufferofDay("friday", "wrr1", "isoptional"));
            wrr1Total_saturday.Text = Convert.ToString((EmpTotalofDay("saturday", "wrr1") + EmpTotalReliefofDay("saturday", "wrr4", "D")) - EmpTotalBufferofDay("saturday", "wrr1", "isoptional"));
            wrr1Total_sunday.Text = Convert.ToString((EmpTotalofDay("sunday", "wrr1") + EmpTotalReliefofDay("sunday", "wrr4", "D")) - EmpTotalBufferofDay("sunday", "wrr1", "isoptional"));
        }

        private void Getwrr2FooterTotals()
        {
            wrr2Total_monday.Text = Convert.ToString((EmpTotalofDay("monday", "wrr2") + EmpTotalReliefofDay("monday", "wrr4","N")) - EmpTotalBufferofDay("monday", "wrr2", "isoptional"));
            wrr2Total_tuesday.Text = Convert.ToString((EmpTotalofDay("tuesday", "wrr2") + EmpTotalReliefofDay("tuesday", "wrr4", "N")) - EmpTotalBufferofDay("tuesday", "wrr2", "isoptional"));
            wrr2Total_wednesday.Text = Convert.ToString((EmpTotalofDay("wednesday", "wrr2") + EmpTotalReliefofDay("wednesday", "wrr4", "N")) - EmpTotalBufferofDay("wednesday", "wrr2", "isoptional"));
            wrr2Total_thursday.Text = Convert.ToString((EmpTotalofDay("thursday", "wrr2") + EmpTotalReliefofDay("thursday", "wrr4", "N")) - EmpTotalBufferofDay("thursday", "wrr2", "isoptional"));
            wrr2Total_friday.Text = Convert.ToString((EmpTotalofDay("friday", "wrr2")  + EmpTotalReliefofDay("friday", "wrr4", "N"))  - EmpTotalBufferofDay("friday", "wrr2", "isoptional"));
            wrr2Total_saturday.Text = Convert.ToString((EmpTotalofDay("saturday", "wrr2")  + EmpTotalReliefofDay("saturday", "wrr4", "N")) - EmpTotalBufferofDay("saturday", "wrr2", "isoptional"));
            wrr2Total_sunday.Text = Convert.ToString((EmpTotalofDay("sunday", "wrr2") + EmpTotalReliefofDay("sunday", "wrr4", "N")) - EmpTotalBufferofDay("sunday", "wrr2", "isoptional"));
        }

        private void Getwrr3FooterTotals()
        {
            wrr3Total_monday.Text = Convert.ToString((EmpTotalofDay("monday", "wrr3") + EmpTotalReliefofDay("monday", "wrr4","C"))  - EmpTotalBufferofDay("monday", "wrr3", "isoptional"));
            wrr3Total_tuesday.Text = Convert.ToString((EmpTotalofDay("tuesday", "wrr3") + EmpTotalReliefofDay("tuesday", "wrr4", "C")) - EmpTotalBufferofDay("tuesday", "wrr3", "isoptional"));
            wrr3Total_wednesday.Text = Convert.ToString((EmpTotalofDay("wednesday", "wrr3")  + EmpTotalReliefofDay("wednesday", "wrr4", "C")) - EmpTotalBufferofDay("wednesday", "wrr3", "isoptional"));
            wrr3Total_thursday.Text = Convert.ToString((EmpTotalofDay("thursday", "wrr3")  + EmpTotalReliefofDay("thursday", "wrr4", "C")) - EmpTotalBufferofDay("thursday", "wrr3", "isoptional"));
            wrr3Total_friday.Text = Convert.ToString((EmpTotalofDay("friday", "wrr3")  + EmpTotalReliefofDay("friday", "wrr4", "C"))  - EmpTotalBufferofDay("friday", "wrr3", "isoptional"));
            wrr3Total_saturday.Text = Convert.ToString((EmpTotalofDay("saturday", "wrr3")  + EmpTotalReliefofDay("saturday", "wrr4", "C")) - EmpTotalBufferofDay("saturday", "wrr3", "isoptional"));
            wrr3Total_sunday.Text = Convert.ToString((EmpTotalofDay("sunday", "wrr3") + EmpTotalReliefofDay("sunday", "wrr4", "C")) - EmpTotalBufferofDay("sunday", "wrr3", "isoptional"));
        }

        #endregion

        #region Check if an Employee is Assigned more than once in the same shift
        private bool isDuplicateInShift(string table1, string table2, string table3, string column1, string column2)
        {
            bool rowsEqual = true;
            DataTable dataTable1 = this.dbaccess.DataSet.Tables[table1];

            for (Int32 r0 = 0; r0 < dataTable1.Rows.Count; r0++)
            {


                if (dataTable1.Rows[r0].RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dataTable1.Rows[r0][column1]))
                    {
                        for (Int32 r1 = r0 + 1; r1 < dataTable1.Rows.Count; r1++)
                        {

                            if (dataTable1.Rows[r1].RowState != DataRowState.Deleted)
                            {
                                if (!BizFunctions.IsEmpty(dataTable1.Rows[r1][column1]))
                                {
                                    

                                    if (dataTable1.Rows[r0][column1].ToString().Trim() == dataTable1.Rows[r1][column1].ToString().Trim() && dataTable1.Rows[r0][column2].ToString().Trim() == dataTable1.Rows[r1][column2].ToString().Trim())
                                    {
                                        MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column1].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        rowsEqual = false;
                                        break;

                                    }

                                    if (rowsEqual == false)
                                    {
                                        break;
                                    }
                                }

                                if (rowsEqual && !isExistinOtherTable(dataTable1.Rows[r0][column1].ToString().Trim(), dataTable1.Rows[r0][column2].ToString().Trim(), table2, column1, column2))
                                {
                                    MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column1].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    rowsEqual = false;
                                    break;

                                }

                                if (rowsEqual && !isExistinOtherTable(dataTable1.Rows[r0][column1].ToString().Trim(), dataTable1.Rows[r0][column2].ToString().Trim(), table3, column1, column2))
                                {
                                    MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column1].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    rowsEqual = false;
                                    break;

                                }
                                
                            }
                        }

                    }

                }



            }

            return rowsEqual;
        }
        #endregion

        #region Check if Employee is Duplicated across Shifts

        private bool isExistinOtherTable(string empnum, string wDay, string datatable, string column1, string column2)
        {
            DataTable dataTable = this.dbaccess.DataSet.Tables[datatable];

            bool rowsEqual = true;

            foreach (DataRow dr1 in dataTable.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr1[column1]) && !BizFunctions.IsEmpty(dr1[column2]))
                    {
                        if (dr1[column1].ToString().Trim() == empnum && dr1[column2].ToString().Trim() == wDay)
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

        private bool isExistinOtherActiveSchedules(string empnum)
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            bool rowsEqual = true;

            string CheckExistingSchedule = "Select empnum,refnum from atmr where empnum='" + empnum + "' and refnum<>'" + wrr["refnum"] + "' and [status]<>'V'";

            this.dbaccess.ReadSQL("CheckExistingSchedule", CheckExistingSchedule);

            if (this.dbaccess.DataSet.Tables["CheckExistingSchedule"].Rows.Count > 0)
            {
                MessageBox.Show("Emp No " + empnum + " already exists in " + this.dbaccess.DataSet.Tables["CheckExistingSchedule"].Rows[0]["refnum"].ToString() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rowsEqual = false;
            }

            this.dbaccess.DataSet.Tables["OtherDayShift"].Dispose();

            return rowsEqual;
        }

        #endregion

        #region Calculate Buffer Day of the Week
        private int EmpTotalBufferofDay(string day, string tablename, string column)
        {
            int total = 0;
            int value = 0;

            DataTable Table = dbaccess.DataSet.Tables[tablename];
            if (Table.Rows.Count > 0)
            {
                
                foreach (DataRow dr1 in Table.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1[day]))
                        {
                            if (BizFunctions.IsEmpty(dr1[column]))
                            {
                                dr1[column] = 0;
                            }

                            if ((bool)dr1[column])
                            {
                                value = 1;
                            }                         
                        }
                        total = total + value;
                    }
                }
            }

            return total;
        }
        #endregion

        #region Get Shift Timein / Timout

        private string ShiftTimein(string Day, string shiftcode)
        {
            string TimeIn = "";
            string get = "select [timein] from shm2 where [day]='" + Day + "' and shiftcode='" + shiftcode + "'";

            this.dbaccess.ReadSQL("TbTimein", get);
            if (this.dbaccess.DataSet.Tables["TbTimein"].Rows.Count > 0)
            {
                TimeIn = this.dbaccess.DataSet.Tables["TbTimein"].Rows[0]["timein"].ToString();
            }
            this.dbaccess.DataSet.Tables["TbTimein"].Dispose();
            return TimeIn;
        }

        private string ShiftTimeOut(string Day, string shiftcode)
        {
            string TimeOut = "";
            string get = "select [timeout] from shm2 where [day]='" + Day + "' and shiftcode='" + shiftcode + "'";

            this.dbaccess.ReadSQL("Tbtimeout", get);
            if (this.dbaccess.DataSet.Tables["Tbtimeout"].Rows.Count > 0)
            {
                TimeOut = this.dbaccess.DataSet.Tables["Tbtimeout"].Rows[0]["timeout"].ToString();
            }
            this.dbaccess.DataSet.Tables["Tbtimeout"].Dispose();

            return TimeOut;
        }

        #endregion

        #region Check Schedule Conflicts

        private bool CheckScheduleConflicts()
        {
            DataRow wrr = this.dbaccess.DataSet.Tables["wrr"].Rows[0];
            bool hasConflict = false;
            if (wrr["status"] != (string)Common.DEFAULT_DOCUMENT_STATUSV)
            {
                
                string getWeeklySchedule = "select * from ATMR where [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(wrr["commencedate"])) + "' and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(wrr["commencedate"])) + "' and refnum<>'" + wrr["refnum"] + "' and [status]<>'V'";
                string GetCurrentSchedules = " Select * from ATMR";
                

                string Lists = "";
                int count = 0;

                DataTable CurrentSchedule = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetCurrentSchedules);

                this.dbaccess.ReadSQL("tbScheduleConfilicts", getWeeklySchedule);

                DataTable tbScheduleConfilicts = this.dbaccess.DataSet.Tables["tbScheduleConfilicts"];

                if (this.dbaccess.DataSet.Tables["tbScheduleConfilicts"].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["tbScheduleConfilicts"].Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            foreach (DataRow dr2 in CurrentSchedule.Rows)
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    string empnumtest1 = dr1["empnum"].ToString();
                                    string empnumtest2 = dr2["empnum"].ToString();

                                    string shiftcodetest1 = dr1["shiftcode"].ToString();
                                    string shiftcodetest2 = dr2["shiftcode"].ToString();

                                    string datetest1 = BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["Date"]));
                                    string datetest2 = BizFunctions.GetSafeDateString(Convert.ToDateTime(dr2["Date"]));

                                    if (BizFunctions.IsEmpty(dr1["isRealSchedule"]))
                                    {
                                        dr1["isRealSchedule"] = 0;
                                    }

                                    if (BizFunctions.IsEmpty(dr2["isRealSchedule"]))
                                    {
                                        dr2["isRealSchedule"] = 0;
                                    }


                                    if (empnumtest1 == empnumtest2 && datetest1 == datetest2)
                                    {
                                        string test = "BINGO";
                                    }


                                    if (dr1["empnum"].ToString() == dr2["empnum"].ToString() && dr1["shiftcode"].ToString() == dr2["shiftcode"].ToString() && BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["Date"])) == BizFunctions.GetSafeDateString(Convert.ToDateTime(dr2["Date"])) && (bool)dr1["isRealSchedule"] == (bool)dr2["isRealSchedule"])
                                    {
                                        Lists = Lists + "\n Employee " + dr1["empnum"].ToString() + " on " + Convert.ToDateTime(dr2["Date"]).ToShortDateString() + " (" + dr1["refnum"].ToString() + ") ";
                                        count = count + 1;
                                    }
                                }
                            }
                        }
                    }
                }



                if (count > 0)
                {
                    MessageBox.Show(Lists, "Schedule has Conflicts,Save Unsuccessful");
                    hasConflict = true;
                }
                else
                {
                    hasConflict = false;
                }
                this.dbaccess.DataSet.Tables["tbScheduleConfilicts"].Dispose();
                CurrentSchedule.Dispose();
            }

            if (confirmed)
            {
                hasConflict = false;
            }

            return hasConflict;
        }

        #endregion

        #region Get Empname

        private string GetEmpname(string empnum)
        {
            string empName = "";

            string Get = "Select Empname From vMainHEMPH where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                empName = dt1.Rows[0]["empname"].ToString();
            }

            dt1.Dispose();

            return empName;

        }

        #endregion

        #region Get Matnum

        private string GetMatnum(string empnum)
        {

            string matnum = "";

            string Get = "Select matnum From vMainHEMPH where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                matnum = dt1.Rows[0]["matnum"].ToString();
            }

            dt1.Dispose();

            return matnum;
        }

        #endregion

        #region Get Total Relief

        private int EmpTotalReliefofDay(string day, string tablename, string ShiftType)
        {
            int total = 0;

            int value = 0;

            DataTable Table = dbaccess.DataSet.Tables[tablename];
            if (Table.Rows.Count > 0)
            {
                string sql = "Select shiftcode,isWorkShift,shifttype from vSHLV";
                this.dbaccess.ReadSQL("vSHLVtmp2", sql);

                foreach (DataRow dr1 in Table.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1[day]))
                        {
                            DataTable dtVshlvTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select shiftcode from vSHLVtmp2  where shiftcode='" + dr1[day].ToString() + "' and isWorkShift=1 and shifttype='" + ShiftType + "'");

                            if (dtVshlvTmp.Rows.Count > 0)
                            {
                                value = 1;
                            }
                            else
                            {
                                value = 0;
                            }

                            dtVshlvTmp.Dispose();
                        }
                        else
                        {
                            value = 0;
                        }
                        total = total + value;
                    }
                }
                this.dbaccess.DataSet.Tables["vSHLVtmp2"].Dispose();
            }


            

            return total;
        }

        #endregion

    }
}


