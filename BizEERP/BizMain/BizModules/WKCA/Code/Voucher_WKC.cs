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
using ATL.SortTable;
using ATL.TimeUtilites;
using ATL.BizModules.TextValidator;
using NodaTime;
using ATL.DateTimeExt;

using ATL.ExtractATR1Form1;

#endregion

namespace ATL.WKC
{
    public class Voucher_WKC : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName, TimesheetForm,TimesheetSummaryForm = null;
        protected TextBox wkch_sectorcode, wkch_day = null;
        protected Button Btn_Sort,Btn_Extract = null;
        protected ComboBox TableColumn = null;
        protected DateTimePicker wkch_wkchdate = null;
        protected Button btnExtract1 = null;
        protected DataGrid Datagrid1, Datagrid2 = null;
        protected DataGridView DatagridView1 = null;
        protected bool isMouseClicked = false;
        protected string SectorCode = "";
        protected bool opened = false;

        protected TextBox txt_editEmpno = null;
        protected Button Btn_Show = null;
        #endregion

        #region Construct

        public Voucher_WKC(string moduleName, Hashtable voucherBaseHelpers,string Sector)
            : base("VoucherGridInfo_WKC.xml", moduleName, voucherBaseHelpers)
        {
            this.SectorCode = Sector;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);
            e.Condition = "SectorCode='" + SectorCode + "' ";

       

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = "SectorCode='" + SectorCode + "' and [status]='O'";


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

            DataTable wkc1 = dbaccess.DataSet.Tables["wck1"];

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.TimesheetForm = (e.FormsCollection["timesheet"] as Form).Name;
            this.TimesheetSummaryForm = (e.FormsCollection["timesheetsummary"] as Form).Name;

            e.DBAccess.DataSet.Tables["WKC1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_WKC1_ColumnChanged);
            opened = true;

            Initialise();

            string GetPAYTM = "SELECT * FROM PAYTM WHERE [status]<>'V'";
            this.dbaccess.ReadSQL("PAYTM", GetPAYTM);


            string GetvSHLV = "Select * from vSHLV";
            this.dbaccess.ReadSQL("vSHLV", GetvSHLV);

         

            Datagrid1 = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "dg_timesheet") as DataGrid;           
            Datagrid1.MouseDoubleClick +=new MouseEventHandler(Datagrid1_MouseDoubleClick);

            //Datagrid1.MouseDown +=new MouseEventHandler(Datagrid1_MouseDown);

            string GetvHEMPHtmp1 = "select * from vMainHEMPH where [status]<>'V'";
            this.dbaccess.ReadSQL("HEMPHtmp1", GetvHEMPHtmp1);

            Datagrid2 = BizXmlReader.CurrentInstance.GetControl(TimesheetSummaryForm, "dg_timesheetsummary") as DataGrid;
            Datagrid2.MouseDoubleClick += new MouseEventHandler(Datagrid2_MouseDoubleClick);
            Datagrid2.MouseClick+=new MouseEventHandler(Datagrid2_MouseClick);
            

        }

        private void Datagrid2_MouseClick(object sender, MouseEventArgs e)
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["wkch"].Rows[0];
            DataTable EmpDGV1 = this.dbaccess.DataSet.Tables["EmpDGV1"];

            try
            {
                isMouseClicked = true;
                #region Get and Set Row

                DataGrid dataGrid = sender as DataGrid;

                int i = dataGrid.CurrentRowIndex;
                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

                if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
                {
                    dataGrid.Select(i);

                    DataRow drCur = getcurrentrow(Datagrid1);

                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim(), "WKC1");

                    eATR1.Show();
                    eATR1.Focus();


                }
                #endregion

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Datagrid2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["wkch"].Rows[0];
            DataTable EmpDGV1 = this.dbaccess.DataSet.Tables["EmpDGV1"];

            try
            {
                isMouseClicked = true;
                #region Get and Set Row

                DataGrid dataGrid = sender as DataGrid;

                int i = dataGrid.CurrentRowIndex;
                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

                if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
                {
                    dataGrid.Select(i);

                    DataRow drCur = getcurrentrow(Datagrid1);

                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim(), "WKC1");

                    eATR1.Show();
                    eATR1.Focus();


                }
                #endregion

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion
        //private void Datagrid1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    DataRow wkch = this.dbaccess.DataSet.Tables["wkch"].Rows[0];
        //    DataTable wkc1 = this.dbaccess.DataSet.Tables["wkc1"];

        //    try
        //    {
        //        isMouseClicked = true;
        //        #region Get and Set Row

        //        DataGrid dataGrid = sender as DataGrid;

        //        int i = dataGrid.CurrentRowIndex;
        //        System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
        //        System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

        //        if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
        //        {// if user double click Row Header or Cell, the selected row will be added to CRQ2.
        //            dataGrid.Select(i);

        //            DataRow drCur = getcurrentrow(Datagrid1);

        //            ExtractATR1Form1.ExtractATR1Form1 eATR1 = new BizERP.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim());

        //            eATR1.Show();
        //            eATR1.Focus();

        //            //refresh here..
        //            //
        //        }
        //        #endregion

        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}


        private void Datagrid1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["wkch"].Rows[0];
            DataTable wkc1 = this.dbaccess.DataSet.Tables["wkc1"];
              
            try
            {
                isMouseClicked = true;
                #region Get and Set Row

                DataGrid dataGrid = sender as DataGrid;

                int i = dataGrid.CurrentRowIndex;
                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

                if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
                {// if user double click Row Header or Cell, the selected row will be added to CRQ2.
                    dataGrid.Select(i);

                    DataRow drCur = getcurrentrow(Datagrid2);
                                            
                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim(),"WKC1");

                    eATR1.Show();
                    eATR1.Focus();

                   
                }
                #endregion

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private DataRow getcurrentrow(DataGrid datagrid)
        {
            CurrencyManager cm = (CurrencyManager)datagrid.BindingContext[datagrid.DataSource, datagrid.DataMember];
            DataRowView drv = cm.Current as DataRowView;
            DataRow dr = drv.Row;

            return dr;
        }



        private void Voucher_WKC1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {           

            switch (e.Column.ColumnName)
            {
                //case "confirmedtimein":
                //    {
                //        if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]))
                //        {
                //            if (!BizERP.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                //            {
                //                e.Row["confirmedtimein"] = System.DBNull.Value;
                //            }
                        
                //        }
                //    }
                //    break;
                //case "confirmedtimeout":
                //    {
                //        if (!BizFunctions.IsEmpty(e.Row["confirmedtimeout"]))
                //        {
                //            if (!BizERP.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                //            {
                //                e.Row["confirmedtimeout"] = System.DBNull.Value;
                //            }
                //        }
                //    }
                //    break;

                case "shiftcode":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["shiftcode"]))
                        {
                            if (BizFunctions.IsEmpty(e.Row["timein"]))
                            {
                                e.Row["timein"] = geTimeIn(e.Row["shiftcode"].ToString());
                            }
                            if (BizFunctions.IsEmpty(e.Row["timeout"]))
                            {
                                e.Row["timeout"] = geTimeOut(e.Row["shiftcode"].ToString());
                            }
                            if (isWorkShift(e.Row["shiftcode"].ToString()) && !BizFunctions.IsEmpty(e.Row["Date"]))
                            {
                                e.Row["Scheddatein"] = Convert.ToDateTime(e.Row["Date"]).ToShortDateString();
                            }
                            else
                            {
                                e.Row["Scheddatein"] = System.DBNull.Value;
                            }
                        }
                    }
                    break;


                case "Date":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["date"]) && BizFunctions.IsEmpty(e.Row["Day"]))
                        {
                            e.Row["Day"] = TimeTools.GetDay(TimeTools.GetDayOfWeekNo(Convert.ToDateTime(e.Row["Date"]).DayOfWeek.ToString()));
                        }
                    }
                    break;

                        

                case "confirmedtimein":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]))
                        {
                            if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                            {
                                e.Row["confirmedtimein"] = System.DBNull.Value;
                            }
                            else
                            {
                                if (!BizFunctions.IsEmpty(e.Row["confirmedtimeout"]))
                                {
                                    if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                                    {
                                        e.Row["confirmedtimeout"] = System.DBNull.Value;
                                    }
                                    else
                                    {

                                        if (Convert.ToDouble(e.Row["confirmedtimein"]) <= Convert.ToDouble(e.Row["confirmedtimeout"]))
                                        {
                                            e.Row["totalhrs"] = Math.Round(Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), e.Row["confirmedtimeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                                            //e.Row["totalhrs"] =Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), e.Row["confirmedtimeout"].ToString()));
                                        }
                                        else
                                        {
                                            decimal tmpTTL1, tmpTTL2 = 0;
                                            tmpTTL1 = (decimal)Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), "2359"));
                                            tmpTTL2 = (decimal)Math.Abs(GetHours("0001", e.Row["confirmedtimeout"].ToString()));
                                            e.Row["totalhrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        }

                                    }

                                    if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]) && !BizFunctions.IsEmpty(e.Row["confirmedtimeout"]) && !BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                    {
                                        decimal tmpTTL1, tmpTTL2, tmpTTL3, tmpTTL4, ftime1, ftime2 = 0;

                                        tmpTTL1 = (decimal)Math.Abs(GetMins(e.Row["confirmedtimein"].ToString(), "2359"));
                                        //tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["confirmedtimeout"].ToString()));
                                        tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime1 = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        tmpTTL3 = (decimal)Math.Abs(GetMins(e.Row["timein"].ToString(), "2359"));
                                        tmpTTL4 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime2 = Math.Round((tmpTTL3 + tmpTTL4), 1, MidpointRounding.AwayFromZero);

                                        if (ftime2 > ftime1)
                                        {
                                            e.Row["LateMins"] = ftime2 - ftime1;
                                        }
                                        else
                                        {
                                            e.Row["LateMins"] = 0;
                                        }

                                    }
                                }
                            }

                        }
                    }
                    break;
                case "confirmedtimeout":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["confirmedtimeout"]))
                        {
                            if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimeout"].ToString()))
                            {
                                e.Row["confirmedtimeout"] = System.DBNull.Value;
                            }

                            else
                            {
                                if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]))
                                {
                                    if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                                    {
                                        e.Row["confirmedtimein"] = System.DBNull.Value;
                                    }
                                    else
                                    {

                                        if (Convert.ToDouble(e.Row["confirmedtimein"]) <= Convert.ToDouble(e.Row["confirmedtimeout"]))
                                        {
                                            e.Row["totalhrs"] = Math.Round(Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), e.Row["confirmedtimeout"].ToString())), 1, MidpointRounding.AwayFromZero);

                                        }
                                        else
                                        {
                                            decimal tmpTTL1, tmpTTL2 = 0;

                                            tmpTTL1 = (decimal)Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), "2359"));
                                            tmpTTL2 = (decimal)Math.Abs(GetHours("0001", e.Row["confirmedtimeout"].ToString()));
                                            e.Row["totalhrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        }

                                    }

                                    if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]) && !BizFunctions.IsEmpty(e.Row["confirmedtimeout"]) && !BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                    {
                                        decimal tmpTTL1, tmpTTL2, tmpTTL3, tmpTTL4, ftime1, ftime2 = 0;

                                        tmpTTL1 = (decimal)Math.Abs(GetMins(e.Row["confirmedtimein"].ToString(), "2359"));
                                        //tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["confirmedtimeout"].ToString()));
                                        tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime1 = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        tmpTTL3 = (decimal)Math.Abs(GetMins(e.Row["timein"].ToString(), "2359"));
                                        tmpTTL4 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime2 = Math.Round((tmpTTL3 + tmpTTL4), 1, MidpointRounding.AwayFromZero);

                                        if (ftime2 > ftime1)
                                        {
                                            e.Row["LateMins"] = ftime2 - ftime1;
                                        }
                                        else
                                        {
                                            e.Row["LateMins"] = 0;
                                        }

                                    }


                                    // GET OT MINS
                                    if (!BizFunctions.IsEmpty(e.Row["totalhrs"]))
                                    {
                                        int TotalHrs = Convert.ToInt32(e.Row["totalhrs"]);
                                        if (TotalHrs > 0)
                                        {
                                            if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]) && !BizFunctions.IsEmpty(e.Row["confirmedtimeout"]) && !BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                            {
                                                decimal tmpTTL1 = 0;

                                                tmpTTL1 = (decimal)Math.Abs(GetMins(e.Row["confirmedtimeout"].ToString(), e.Row["timeout"].ToString()));


                                                if (tmpTTL1 > 0)
                                                {
                                                    e.Row["OtMins"] = tmpTTL1;
                                                }
                                                else
                                                {
                                                    e.Row["OtMins"] = 0;
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "empnum":
                    {
                        e.Row["empnum2"] = e.Row["empnum"].ToString();
                        e.Row["empnum3"] = e.Row["empnum"].ToString();
                        e.Row["empnum4"] = e.Row["empnum"].ToString();
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["paytypecode"] = GetPayTypeCode(e.Row["empnum"].ToString());
                        }
                    }
                    break;



                case "rempnum":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["rempnum"]))
                        {
                            e.Row["rempname"] = getEmpName(e.Row["rempnum"].ToString());
                        }
                    }
                    break;
            }
        }


        private double GetHours(string start, string end)
        {
            double hourstaken;
            LocalTime dt1 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(start);
            LocalTime dt2 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(end);

            Duration dr1 = new Duration(dt1.TickOfDay);
            Duration dr2 = new Duration(dt2.TickOfDay);

            Duration dr3 = Duration.Subtract(dr1, dr2);

            TimeSpan elapsedSpan1 = new TimeSpan(dr3.Ticks);

            return hourstaken = elapsedSpan1.TotalHours;

        }

        private double GetMins(string start, string end)
        {
            double hourstaken;
            LocalTime dt1 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(start);
            LocalTime dt2 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(end);

            Duration dr1 = new Duration(dt1.TickOfDay);
            Duration dr2 = new Duration(dt2.TickOfDay);

            Duration dr3 = Duration.Subtract(dr1, dr2);

            TimeSpan elapsedSpan1 = new TimeSpan(dr3.Ticks);

            return hourstaken = elapsedSpan1.TotalMinutes;

        }

        private string getEmpName(string empno)
        {
            string EmpName = "";

            string GetEmpno = "Select empname from HEMPHtmp1 where empnum='" + empno + "'";

            DataTable HemphTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetEmpno);

            if (HemphTmp.Rows.Count > 0)
            {
                EmpName = HemphTmp.Rows[0]["empname"].ToString();
            }

            return EmpName;
        }

        private string geTimeIn(string shiftcode)
        {
            string Timein = "";

            string GetvSHLV = "Select timein from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timein = vSHLVTmp.Rows[0]["timein"].ToString();
            }
            vSHLVTmp.Dispose();
            return Timein;
        }

        private string geTimeOut(string shiftcode)
        {
            string Timeout = "";

            string GetvSHLV = "Select [timeout] from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timeout = vSHLVTmp.Rows[0]["timeout"].ToString();
            }
            vSHLVTmp.Dispose();
            return Timeout;
        }

        private void Initialise()
        {

            Btn_Sort = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "Btn_Sort") as Button;
            Btn_Extract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Extract") as Button;

            TableColumn = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "TableColumn") as ComboBox;

            Btn_Sort.Click +=new EventHandler(Btn_Sort_Click);
            Btn_Extract.Click +=new EventHandler(Btn_Extract_Click);

            txt_editEmpno = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_editEmpno") as TextBox;
            txt_editEmpno.KeyDown += new KeyEventHandler(txt_editEmpno_KeyDown);
            txt_editEmpno.DoubleClick += new EventHandler(txt_editEmpno_DoubleClick);

            Btn_Show = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Show") as Button;
            Btn_Show.Click += new EventHandler(Btn_Show_Click);
            
            //wkch_sectorcode = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "wkch_sectorcode") as TextBox;
        
            //wkch_day = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "wkch_day") as TextBox;


            //wkch_wkchdate = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "wkch_wkchdate") as DateTimePicker;

            //btnExtract1 = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "Btn_Extract") as Button;
            //btnExtract1.Click += new EventHandler(btnExtract1_Click);
            

        }

        private void Btn_Show_Click(object sender, EventArgs e)
        {
            if (txt_editEmpno.Text != string.Empty)
            {
                try
                {

                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, txt_editEmpno.Text, "WKC1");

                    eATR1.Show();
                    eATR1.Focus();

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        private void txt_editEmpno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                if (txt_editEmpno.Text != string.Empty)
                {
                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + txt_editEmpno.Text + "%' and sectorcode='" + SectorCode + "'", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {

                        txt_editEmpno.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                    }
                }
            }
        }
        private void txt_editEmpno_DoubleClick(object sender, EventArgs e)
        {
            if (txt_editEmpno.Text != string.Empty)
            {
                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + txt_editEmpno.Text + "%' and sectorcode='" + SectorCode + "'", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {

                    txt_editEmpno.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                }
            }

        }


        private void Btn_Extract_Click(object sender, EventArgs e)
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["WKCH"].Rows[0];
            DataTable wkc1 = this.dbaccess.DataSet.Tables["WKC1"];
            //if (!BizFunctions.IsEmpty(wkch["weekno"]) && !BizFunctions.IsEmpty(wkch["weekyear"]))
            //{
            //    DateTime FirstDay = TimeTools.GetFirstDayOfWeek(Convert.ToInt32(wkch["weekyear"]), Convert.ToInt32(wkch["weekno"]));

            //    DateTime EndDay = FirstDay.AddDays(6);

            //    wkch["WeekDateFrom"] = FirstDay;
            //    wkch["WeekDateTo"] = EndDay;

            //    Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            //    DataTable oriTable = wkc1;
            //    try
            //    {
            //        ExtractATRWForm ExtractATR1 = new ExtractATRWForm(this.dbaccess, oriTable,SectorCode);
            //        ExtractATR1.ShowDialog(frm);

            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please Fill up both Week and Year", "Empty Week / Year No", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            if (!BizFunctions.IsEmpty(wkch["WeekDateFrom"]) && !BizFunctions.IsEmpty(wkch["WeekDateTo"]))
            {
                if (Convert.ToDateTime(wkch["WeekDateFrom"]) <= Convert.ToDateTime(wkch["WeekDateTo"]))
                {


                    Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
                    DataTable oriTable = wkc1;
                    try
                    {
                        ExtractATRWForm ExtractATR1 = new ExtractATRWForm(this.dbaccess, oriTable, SectorCode);
                        ExtractATR1.ShowDialog(frm);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Date To can't be earlier than Date From", "Wrong Date", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Either Date From / To Can't Be Empty", "Empty Dates", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
        }

        private string GetPayTypeCode(string empnum)
        {
            string paytype = "";

            string getPayType = "Select paytypecode from HEMPHtmp1 where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getPayType);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                paytype = dr1["paytypecode"].ToString();
            }

            return paytype;
        }

        private decimal GetDaysPerWeek(string empnum)
        {
            decimal daysPerWeek = 0;

            string getDaysPerWK = "Select daysperweek from vMainHEMPH where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getDaysPerWK);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                if (!BizFunctions.IsEmpty(dr1["daysperweek"]))
                {
                    daysPerWeek = Convert.ToDecimal(dr1["daysperweek"]);
                }
               
            }

            return daysPerWeek;
        }

        private decimal GetPayTypeValue(string paytypecode)
        {
            decimal value = 0;

            string GetPayTypeValue = "Select ISNULL(value,0) as value from PAYTM where paytypecode='" + paytypecode + "'";

            DataTable dt2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetPayTypeValue);

            if (dt2.Rows.Count > 0)
            {
                DataRow dr2 = dt2.Rows[0];

                value = Convert.ToDecimal(dr2["value"]);
            }



            return value;
        }

   
   
        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow wkch = this.dbaccess.DataSet.Tables["wkch"].Rows[0];

            //wkch["sectorcode"] = SectorCode;

            //string CheckExists = "Select * from wkch where weekno='"+wkch["weekno"].ToString()+"' and weekyear='"+wkch["weekyear"].ToString()+"' and sectorcode='"+SectorCode+"' and refnum<>'"+wkch["refnum"].ToString()+"' and [status]<>'V' ";

            //this.dbaccess.ReadSQL("tmpCheck", CheckExists);

            //DataTable tmpCheck = this.dbaccess.DataSet.Tables["tmpCheck"];

            //if (tmpCheck.Rows.Count > 0)
            //{
            //    MessageBox.Show("There is already a Refnum(" + tmpCheck.Rows[0]["refnum"].ToString() + ") for this Week of the Year ", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    e.Handle = false;
            //}
  
        }


        #region DocumentF2

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {           
            base.AddDocumentF3Condition(sender, e);
            DataRow wkch = this.dbaccess.DataSet.Tables["WKCH"].Rows[0];
            switch (e.ControlName)
            {
                case "wkch_wkchnum":
                    {
                        //if (!BizFunctions.IsEmpty(wkch["wkchnum"].ToString()))
                        //{
                        //    GetWrr();
                        //    GetATMR();
                        //}
                    }
                    break;


            }
        }
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "wkch_wkchnum":
                    {                      
                        e.DefaultCondition = "refnum not in (Select wkchnum from wkch where status<>'V') and status<>'V'";               
                    }
                    break;

                case "wkch_fromempnum":
                    {
                        e.DefaultCondition = "paytypecode='W' and status<>'V' and sectorcode='"+SectorCode+"' ";
                    }
                    break;

                case "wkch_toempnum":
                    {
                        e.DefaultCondition = "paytypecode='W' and status<>'V' and sectorcode='" + SectorCode + "' ";
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

        #region  btn_Sort_Click
        protected void Btn_Sort_Click(object sender, System.EventArgs e)
        {
            DataTable wkc1 = this.dbaccess.DataSet.Tables["wkc1"];
            TableColumn = (ComboBox)BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "TableColumn");


            string cname = TableColumn.Text.ToString();
            if (cname != "")
            {
                if (wkc1.Rows.Count > 0)
                {
                    SortDT sort = new SortDT(wkc1, cname + " ASC");
                    DataTable returnedfinalextraction = sort.SortedTable();

                    BizFunctions.DeleteAllRows(wkc1);

                    foreach (DataRow dr in returnedfinalextraction.Select())
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            wkc1.ImportRow(dr);
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

        private string GetEmpname(string empnum)
        {
            string empName = "";

            string Get = "Select Empname From HEMPHtmp1 where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                empName = dt1.Rows[0]["empname"].ToString();
            }

            dt1.Dispose();

            return empName;

        }

        private void GetSummary()
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["wkch"].Rows[0];
            DataTable wkc2 = this.dbaccess.DataSet.Tables["wkc2"];

            string sql1 = "SELECT " +
                                    "b.empnum, " +
                                    "h.empname, " +
                                    "b.wd as totalpayabledays, " +
                                    "b.tothrs as TotalwkHrs, " +
                                    "b.rcount as totalRest, " +
                                    "b.dre as totaldre, " +
                                             "b.ot2 as totalot, " +
                                    "b.ot15hrs, " +
                                    "b.latecount as TotalLatecount, " +
                                    "(b.latemins/60) as TotalLateHrs, " +
                                    "b.pl as totalAleave, " +
                                    "b.mc as totalmc, " +
                                    "b.npl as totalupl, " +
                                    "b.awol totalawol  " +
                                    "from " +
                                    "( " +
                                         "select empnum,SUM(wdcount) as wd,SUM(mccount) as mc, SUM (nplcount) as npl, SUM(awolcount) as awol, SUM (plcount) as pl, SUM (hpcount) as hp, " +
                                              "SUM(phcount) as ph, SUM(workhrs) as tothrs,SUM(latecount) as latecount,sum(latemins) as latemins, sum(standbyhrs) as standbyhrs,sum(ot15hrs) as ot15hrs, " +
                                              "SUM(dre) as dre, SUM(rcount) as rcount,SUM(ot2) as ot2  " +
                                         "from " +
                                         "( " +
                                              "select empnum,[Date] as WorkDate,[DAY],paytypecode,shiftcode,sectorcode,sitenum,timein,confirmedtimein,[timeout],confirmedtimeout, " +
                                                   "case when shiftcode in ('MED','MC') then 1 else 0 end as mccount, " +
                                                   "case when shiftcode in ('HPL','HOS') then 1 else 0 end as hpcount, " +
                                                   "case when shiftcode like '%UPL%' then 1 else 0 end as nplcount, " +
                                                   "case when shiftcode like 'AWO%' then 1 else 0 end as awolcount, " +
                                                   "case when shiftcode in ('AL','A/L') then 1 else 0 end as plcount, " +
                                                   "case when shiftcode like 'RES%' then 1 else 0 end as rcount, " +
                                                   "case when shiftcode in ('PH') then 1 else 0 end as phcount, " +
                                                   "case when ISNULL(TotalHrs,0)>0 then 1 else 0 end as wdcount,  " +
                                                   "case when isnull(isDRE,0)=1 then 1 else 0 end as dre, " +
                                                   "case when isnull(isRD,0)=1 then 1 else 0 end as rd, " +
                                                   "case when isnull(isOT,0)=1 then 1 else 0 end as ot2, " +
                                                   "ISNULL(TotalHrs,0) as workhrs, " +
                                                   "case when ISNULL(LateMins,0)>10 then 1 else 0 end as latecount, " +
                                                   "CASE WHEN ISNULL(latemins,0)<=10 THEN 0 ELSE ISNULL(latemins,0) end as latemins, " +
                                                   "CASE WHEN TotalHrs>7.33 THEN TotalHrs-7.33 else 0 end as ot15hrs, " +
                                                   "CASE WHEN ISNULL(otmins,0)>15 THEN ISNULL(otmins,0)/60 ELSE 0 end as standbyhrs, " +
                                                   "atrnum as reference  " +
                                              "from wkc1 " +
                                              "where refnum='" + wkch["refnum"].ToString() + "' " +
                                         ") a " +
                                         "group by empnum " +
                                    ") b " +
                                    "left join  " +
                                    "( " +
                                    "select empnum,empname " +
                                    "from hemph " +
                                    ") h on h.empnum = b.empnum";


            this.dbaccess.ReadSQL("tmpWKC2", sql1);

            DataTable tmpWKC2 = this.dbaccess.DataSet.Tables["tmpWKC2"];

            BizFunctions.DeleteAllRows(wkc2);

            if (tmpWKC2.Rows.Count > 0)
            {
                foreach (DataRow dr1 in tmpWKC2.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWKC2 = wkc2.NewRow();
                        InsertWKC2["empnum"] = dr1["empnum"];
                        InsertWKC2["empname"] = dr1["empname"];
                        InsertWKC2["totalpayabledays"] = dr1["totalpayabledays"];
                        InsertWKC2["TotalwkHrs"] = dr1["TotalwkHrs"];
                        InsertWKC2["totalRest"] = dr1["totalRest"];
                        InsertWKC2["totaldre"] = dr1["totaldre"];
                        InsertWKC2["totalot"] = dr1["totalot"];
                        InsertWKC2["ot15hrs"] = dr1["ot15hrs"];
                        InsertWKC2["TotalLatecount"] = dr1["TotalLatecount"];
                        InsertWKC2["TotalLateHrs"] = dr1["TotalLateHrs"];
                        InsertWKC2["totalAleave"] = dr1["totalAleave"];
                        InsertWKC2["totalmc"] = dr1["totalmc"];
                        InsertWKC2["totalupl"] = dr1["totalupl"];
                        InsertWKC2["totalawol"] = dr1["totalawol"];
                        wkc2.Rows.Add(InsertWKC2);
                    }
                }
            }






        }


     

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["WKCH"].Rows[0];
            DataTable wkc1 = this.dbaccess.DataSet.Tables["WKC1"];
            DataTable wkc2 = this.dbaccess.DataSet.Tables["WKC2"];
            base.Document_SaveBegin_OnClick(sender, e);

            if (BizFunctions.IsEmpty(wkch["trandate"]))
            {
                wkch["trandate"] = DateTime.Now;
            }

            #region WKCH1
            foreach (DataRow dr1 in wkc1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(wkch, dr1, "refnum/user/flag/sectorcode/status/trandate/createdby/created/modified");

                    if (!BizFunctions.IsEmpty(dr1["shiftcode"]))
                    {
               
                        if (BizFunctions.IsEmpty(dr1["timein"]))
                        {
                            dr1["timein"] = geTimeIn(dr1["shiftcode"].ToString());
                        }
                        if (BizFunctions.IsEmpty(dr1["timeout"]))
                        {
                            dr1["timein"] = geTimeOut(dr1["shiftcode"].ToString());
                        }
                    }

                    if (BizFunctions.IsEmpty(dr1["confirmedtimein"]))
                    {
                        if (!BizFunctions.IsEmpty(dr1["schedtimein"]))
                        {
                            dr1["confirmedtimein"] = dr1["schedtimein"];
                        }
                    }

                    if (BizFunctions.IsEmpty(dr1["confirmedtimeout"]))
                    {
                        if (!BizFunctions.IsEmpty(dr1["schedtimeout"]))
                        {
                            dr1["confirmedtimeout"] = dr1["schedtimeout"];
                        }
                        else
                        {
                            dr1["confirmedtimeout"] = dr1["timeout"];
                        }
                    }

                    if (!BizFunctions.IsEmpty(dr1["empnum"]))
                    {
                        dr1["empname"] = GetEmpname(dr1["empnum"].ToString());
                    }

                    //if (!BizFunctions.IsEmpty(wkch["weekno"]) && BizFunctions.IsEmpty(dr1["weekno"]))
                    //{
                        
                       

                    //}

                    if (!BizFunctions.IsEmpty(dr1["Date"]))
                    {
                        dr1["weekno"] = DateTimeExt.DateTimeExtensions.weekNumber(Convert.ToDateTime(dr1["Date"]));
                    }

                    if (!BizFunctions.IsEmpty(dr1["confirmedtimeout"]))
                    {
                        if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(dr1["confirmedtimeout"].ToString()))
                        {
                            dr1["confirmedtimeout"] = System.DBNull.Value;
                        }

                        else
                        {
                            if (!BizFunctions.IsEmpty(dr1["confirmedtimein"]))
                            {
                                if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(dr1["confirmedtimein"].ToString()))
                                {
                                    dr1["confirmedtimein"] = System.DBNull.Value;
                                }
                                else
                                {

                                    if (Convert.ToDouble(dr1["confirmedtimein"]) <= Convert.ToDouble(dr1["confirmedtimeout"]))
                                    {
                                        dr1["totalhrs"] = Math.Round(Math.Abs(GetHours(dr1["confirmedtimein"].ToString(), dr1["confirmedtimeout"].ToString())), 1, MidpointRounding.AwayFromZero);

                                    }
                                    else
                                    {
                                        decimal tmpTTL1, tmpTTL2 = 0;

                                        tmpTTL1 = (decimal)Math.Abs(GetHours(dr1["confirmedtimein"].ToString(), "2359"));
                                        tmpTTL2 = (decimal)Math.Abs(GetHours("0001", dr1["confirmedtimeout"].ToString()));
                                        dr1["totalhrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                    }

                                }

                                if (!BizFunctions.IsEmpty(dr1["confirmedtimein"]) && !BizFunctions.IsEmpty(dr1["confirmedtimeout"]) && !BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["timeout"]))
                                {
                                    decimal tmpTTL1, tmpTTL2, tmpTTL3, tmpTTL4, ftime1, ftime2 = 0;

                                    tmpTTL1 = (decimal)Math.Abs(GetMins(dr1["confirmedtimein"].ToString(), "2359"));
                                    //tmpTTL2 = (decimal)Math.Abs(GetMins("0001", dr1["confirmedtimeout"].ToString()));
                                    tmpTTL2 = (decimal)Math.Abs(GetMins("0001", dr1["timeout"].ToString()));

                                    ftime1 = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                    tmpTTL3 = (decimal)Math.Abs(GetMins(dr1["timein"].ToString(), "2359"));
                                    tmpTTL4 = (decimal)Math.Abs(GetMins("0001", dr1["timeout"].ToString()));

                                    ftime2 = Math.Round((tmpTTL3 + tmpTTL4), 1, MidpointRounding.AwayFromZero);

                                    if (ftime2 > ftime1)
                                    {
                                        dr1["LateMins"] = ftime2 - ftime1;
                                    }
                                    else
                                    {
                                        dr1["LateMins"] = 0;
                                    }

                                }


                                // GET OT MINS
                                if (!BizFunctions.IsEmpty(dr1["totalhrs"]))
                                {
                                    int TotalHrs = Convert.ToInt32(dr1["totalhrs"]);
                                    if (TotalHrs > 0)
                                    {
                                        if (!BizFunctions.IsEmpty(dr1["confirmedtimein"]) && !BizFunctions.IsEmpty(dr1["confirmedtimeout"]) && !BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["timeout"]))
                                        {
                                            decimal tmpTTL1 = 0;

                                            tmpTTL1 = (decimal)Math.Abs(GetMins(dr1["confirmedtimeout"].ToString(), dr1["timeout"].ToString()));


                                            if (tmpTTL1 > 0)
                                            {
                                                dr1["OtMins"] = tmpTTL1;
                                            }
                                            else
                                            {
                                                dr1["OtMins"] = 0;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (BizFunctions.IsEmpty(dr1["scheddateout"]))
                    {
                        if (isWorkShift(dr1["shiftcode"].ToString()))
                        {
                            if (!BizFunctions.IsEmpty(dr1["confirmedtimeout"]) && !BizFunctions.IsEmpty(dr1["TotalHrs"]))
                            {
                                decimal tmpTTL3, tmpTTL4, TThrs, ftime2 = 0;
                                decimal twenty4hrs = 24 * 60;

                                TThrs = Convert.ToDecimal(dr1["TotalHrs"]);
                                tmpTTL4 = (decimal)Math.Abs(GetMins("0001", dr1["timein"].ToString()));


                                ftime2 = Math.Round(((TThrs * 60) + tmpTTL4), 1, MidpointRounding.AwayFromZero);


                                if (ftime2 > twenty4hrs)
                                {
                                    DateTime dt1 = Convert.ToDateTime(dr1["scheddatein"]);
                                    DateTime dt2 = dt1.AddDays(1);
                                    dr1["scheddateout"] = dt2.ToShortDateString();
                                }
                                else
                                {
                                    DateTime dt1 = Convert.ToDateTime(dr1["scheddatein"]);
                                    DateTime dt2 = dt1;
                                    dr1["scheddateout"] = dt2.ToShortDateString();
                                }

                            }
                        }
                    }

                    if (BizFunctions.IsEmpty(dr1["empnum2"]) && !BizFunctions.IsEmpty(dr1["empnum"]))
                    {
                        dr1["empnum2"] = dr1["empnum"];
                    }
                    if (BizFunctions.IsEmpty(dr1["empnum3"]) && !BizFunctions.IsEmpty(dr1["empnum"]))
                    {
                        dr1["empnum3"] = dr1["empnum"];
                    }
                    if (BizFunctions.IsEmpty(dr1["empnum4"]) && !BizFunctions.IsEmpty(dr1["empnum"]))
                    {
                        dr1["empnum3"] = dr1["empnum"];
                    }

                    if (!BizFunctions.IsEmpty(dr1["date"]) && BizFunctions.IsEmpty(dr1["Day"]))
                    {

                        dr1["Day"] = TimeTools.GetDay(TimeTools.GetDayOfWeekNo(Convert.ToDateTime(dr1["Date"]).DayOfWeek.ToString()));
                    }



                }

            }
            #endregion      
            
  
            foreach (DataRow dr2 in wkc2.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(wkch, dr2, "refnum/user/flag/status/trandate/createdby/created/modified");
                }
            }
                                  
        }
        #endregion

        private bool isWorkShift(string shiftcode)
        {
            bool isWorkShift = false;

            string GetIsWorkShift = "Select isWorkShift from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable GetIsWorkShiftTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetIsWorkShift);

            if (GetIsWorkShiftTmp.Rows.Count > 0)
            {
                if (BizFunctions.IsEmpty(GetIsWorkShiftTmp.Rows[0]["isWorkShift"]))
                {
                    GetIsWorkShiftTmp.Rows[0]["isWorkShift"] = 0;
                }
                isWorkShift = (bool)Convert.ToBoolean(GetIsWorkShiftTmp.Rows[0]["isWorkShift"]);
            }

            return isWorkShift;
        }
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataTable wkc2 = this.dbaccess.DataSet.Tables["wkc2"];

            GetSummary();

            if (wkc2.Rows.Count > 0)
            {
                this.dbaccess.Update(e.SessionID, "WKC2", "WKC2");
            }

    
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
            DataRow wkch = dbaccess.DataSet.Tables["wkch"].Rows[0];
            if (wkch["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "sivh"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion

        #region Print on Click

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

            DataRow wkch = e.DBAccess.DataSet.Tables["wkch"].Rows[0];
            Hashtable selectedCollection = new Hashtable();

            switch (e.ReportName)
            {
                //case "Local Invoice - Inclusive GST":
                //    selectedCollection.Add("coy", "SELECT * FROM coy");
                //    selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
                //    selectedCollection.Add("ptm", "SELECT top 1 * FROM ptm where ptnum='" + sivh["payterms"].ToString() + "'");
                //    selectedCollection.Add("delarm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
                //    selectedCollection.Add("matm", "SELECT * FROM matm");
                //    e.DBAccess.ReadSQL(selectedCollection);
                //    e.DataSource = e.DBAccess.DataSet;
                //    break;

                case "Weekly Summary Report":
                    //selectedCollection.Add("coy", "SELECT * FROM coy");
                    //selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
                    //selectedCollection.Add("ptm", "SELECT top 1 * FROM ptm where ptnum='" + sivh["payterms"].ToString() + "'");
                    //selectedCollection.Add("delarm", "SELECT top 1 * FROM arm where arnum='" + sivh["arnum"].ToString() + "'");
                    //selectedCollection.Add("matm", "SELECT * FROM matm");
                    //e.DBAccess.ReadSQL(selectedCollection);
                    e.DataSource = e.DBAccess.DataSet;
                    break;

             
            }            

        }

        #endregion

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);

            DataRow wkch = dbaccess.DataSet.Tables["wkch"].Rows[0];
            if (wkch["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "wkch"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion

        private void GetWrr()
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            string sqlGetWKCH = "Select * from wkch where refnum='" + wkch["wkchnum"].ToString().Trim() + "'";
            this.dbaccess.ReadSQL("TBGetWKCH", sqlGetWKCH);
            DataTable TBGetWKCH = this.dbaccess.DataSet.Tables["TBGetWKCH"];

            if (TBGetWKCH.Rows.Count > 0)
            {
                DataRow drGetWKCH = this.dbaccess.DataSet.Tables["TBGetWKCH"].Rows[0];
                wkch["sectorcode"] = drGetWKCH["sectorcode"]; 
                wkch["sitenum"] = drGetWKCH["sitenum"];
                wkch["wkchFromDate"] = Convert.ToDateTime(drGetWKCH["commencedate"]).ToShortDateString();
                wkch["wkchToDate"] = Convert.ToDateTime(drGetWKCH["enddate"]).ToShortDateString();
           
            }
        }

        private void GetATMR()
        {
            DataRow wkch = this.dbaccess.DataSet.Tables["WKCH"].Rows[0];
            DataTable wkc1 = this.dbaccess.DataSet.Tables["WKC1"];
            if (wkc1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wkc1);
            }
            string sqlGetATR = "Select * from atmr where refnum='" + wkch["wkchnum"].ToString().Trim() + "'";

            this.dbaccess.ReadSQL("TBGetATMR", sqlGetATR);
            DataTable TBGetATMR = this.dbaccess.DataSet.Tables["TBGetATMR"];
            if (TBGetATMR.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TBGetATMR.Select())
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertAtr1 = wkc1.NewRow();

                        InsertAtr1["nric"] = dr1["nric"];
                        InsertAtr1["empnum"] = dr1["empnum"];                       
                        InsertAtr1["shiftcode"] = dr1["shiftcode"];
                      
                        // timein
                        if (BizFunctions.IsEmpty(dr1["timein"]))
                        {
                            InsertAtr1["timein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["timein"] = dr1["timein"];
                        }
                        //timeout
                        if (BizFunctions.IsEmpty(dr1["timeout"]))
                        {
                            InsertAtr1["timeout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["timeout"] = dr1["timeout"];
                        }
                        //scheddatetiin
                        if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            InsertAtr1["scheddatein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["scheddatein"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimein"]));

                        }
                        //scheddateout



                        if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            InsertAtr1["scheddateout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimeout"]));
                        }

                        //schedtimein
                        if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            InsertAtr1["schedtimein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                        }

                        ////////////////////////////

                        if (!BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            LocalTime timein = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timein"].ToString());
                            LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"])));

                            if (timein.TickOfDay < actualtimeout.TickOfDay)
                            {
                                InsertAtr1["confirmedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                            }
                            else
                            {
                                InsertAtr1["confirmedtimein"] = dr1["timein"].ToString();
                            }
                        }


                        ////////////////////////////

                        //schedtimeout
                        if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            InsertAtr1["schedtimeout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"]));

                        }

                        if (!BizFunctions.IsEmpty(dr1["timeout"]) && !BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            LocalTime timeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timeout"].ToString());
                            LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"])));

                            if (timeout.TickOfDay < actualtimeout.TickOfDay)
                            {
                                InsertAtr1["confirmedtimeout"] = dr1["timeout"].ToString();
                            }
                            else
                            {
                                InsertAtr1["confirmedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"]));
                            }
                        }


                        //
                                                                                                    
                        InsertAtr1["replacedby"] = dr1["rempnum"];
                        InsertAtr1["rempname"] = dr1["rempname"];
                        InsertAtr1["rnric"] = dr1["rnric"];
                        InsertAtr1["day"] = dr1["day"];
                        InsertAtr1["date"] = dr1["date"];
                        InsertAtr1["dayofweek"] = TimeTools.GetDayOfWeekNo(dr1["day"].ToString().Trim());
                        InsertAtr1["RepRefKey"] = dr1["RefKey"];
                        wkc1.Rows.Add(InsertAtr1);
                    }
                }

            }
        }

      
    }
}
    

