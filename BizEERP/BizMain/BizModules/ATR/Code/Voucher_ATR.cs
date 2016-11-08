#region Namespaces
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

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
using ATL.PaintCellsOfGrid;

#endregion

namespace ATL.ATR
{
    public class Voucher_ATR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName, TimesheetForm, TimesheetSummaryForm = null;
        protected TextBox atr1_sectorcode, atr1_day,atr_wrrnum = null;
        protected Button Btn_Sort,Btn_Extract = null;
        protected ComboBox TableColumn = null;
        protected DateTimePicker atr1_atrdate = null;
        protected Button btnExtract1 = null;
        protected DataGrid Datagrid1, Datagrid2 = null;
        protected DataGridView Datagridview2 = null;
        protected string SectorCode = "";
        protected bool opened, isMouseClicked = false;
        protected TextBox txt_editEmpno = null;
        protected Button Btn_Show = null;
        #endregion

        #region Construct

        public Voucher_ATR(string moduleName, Hashtable voucherBaseHelpers, string Sector)
            : base("VoucherGridInfo_ATR.xml", moduleName, voucherBaseHelpers)
        {
            this.SectorCode = Sector;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            //e.Condition = "SectorCode='" + SectorCode + "'";
       

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            //e.Condition = "SectorCode='" + SectorCode + "' and [status]='O'";

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

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.TimesheetForm = (e.FormsCollection["timesheet"] as Form).Name;
            this.TimesheetSummaryForm = (e.FormsCollection["timesheetsummary"] as Form).Name;
            opened = true;
            e.DBAccess.DataSet.Tables["ATR1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ATR1_ColumnChanged);


            Initialise();

          

            string GetvSHLV = "select * from vshlv";
            this.dbaccess.ReadSQL("vSHLV", GetvSHLV);

            string GetSHM = "select * from shm where [status]<>'V'";
            this.dbaccess.ReadSQL("SHM", GetSHM);

            string GetvHEMPHtmp1 = "select * from vMainHEMPH where [status]<>'V'";
            this.dbaccess.ReadSQL("HEMPHtmp1", GetvHEMPHtmp1);

            


            Datagrid1 = BizXmlReader.CurrentInstance.GetControl(TimesheetSummaryForm, "dg_timesheet") as DataGrid;
           

            Datagrid2 = BizXmlReader.CurrentInstance.GetControl(TimesheetSummaryForm, "dg_timesheetsummary") as DataGrid;
            Datagrid2.MouseDoubleClick += new MouseEventHandler(Datagrid2_MouseDoubleClick);


            HighlightLateRows();
        }

        #endregion

     
        private void HighlightLateRows()
        {
            DataTable ATR1 = this.dbaccess.DataSet.Tables["ATR1"];

            //BindingSource nbindingSource = new BindingSource();
            //nbindingSource.DataSource = ATR1;

           

            //Datagridview2.DataSource = nbindingSource;

            //if (Datagridview2.Rows.Count > 0)
            //{

            //    foreach (DataGridViewRow r in Datagridview2.Rows)
            //    {
            //        if (!BizFunctions.IsEmpty(r.Cells["latemins"]) && !BizFunctions.IsEmpty(r.Cells["empnum"]))
            //        {
            //            decimal latemins = Convert.ToDecimal(r.Cells["latemins"]);
            //            if (latemins > 0)
            //            {
            //                string test = "";
            //            }
            //        }
            //    }
            //}
            //if (ATR1.Rows.Count > 0)
            //{
            //    BindingSource nbindingSource = new BindingSource();
            //    nbindingSource.DataSource = ATR1;

            //    Datagridview2 = new DataGridView();

            //    Datagridview2.DataSource = nbindingSource;

            //    Datagridview2.DataSource = ATR1;

            //    for (int i = 0; i < ATR1.Rows.Count; i++)
            //    {
            //        if (ATR1.Rows[i].RowState != DataRowState.Deleted)
            //        {
            //            if (!BizFunctions.IsEmpty(ATR1.Rows[i]["Empnum"]) && !BizFunctions.IsEmpty(ATR1.Rows[i]["latemins"]))
            //            {
            //                decimal latemins = Convert.ToDecimal(ATR1.Rows[i]["latemins"]);
            //                if (latemins >= 15)
            //                    for (int y = 0; y < Datagridview2.Rows[i].Cells.Count; y++)
            //                    {
            //                        Datagridview2.Rows[i].Cells[y].Style.BackColor = System.Drawing.Color.Red;
                            
            //                    }
            //            }
            //        }
            //    }

            //}

            //foreach (DataColumn dc1 in ATR1.Columns)
            //{
            //    if (ATR1.Columns.IndexOf(dc1.ColumnName) != -1)
            //    {

            //        if (ATR1.Rows.Count > 0)
            //        {
            //            foreach (DataRow dr1 in ATR1.Rows)
            //            {
            //                if (dr1.RowState != DataRowState.Deleted)
            //                {
            //                    foreach (DataGridTableStyle dataGridTableStyle in Datagrid2.TableStyles)
            //                    {

            //                        string test = "test";
            //                            foreach (DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
            //                            {

            //                                if (dataGridColumnStyle is BizDataGridFormColumn)
            //                                {

            //                                    if (!BizFunctions.IsEmpty(dr1["empnum"]) && !BizFunctions.IsEmpty(dr1["latemins"]))
            //                                    {
            //                                        decimal latemins = Convert.ToDecimal(dr1["latemins"]);
            //                                        if (latemins >= 15)
            //                                        {
            //                                            dataGridColumnStyle.DataGridTableStyle.BackColor = Color.Gold;
            //                                        }
            //                                    }
            //                                }


            //                        //        //if (dataGridColumnStyle is BizDataGridTextBoxColumn)
            //                        //        //{
            //                        //        //    BizDataGridTextBoxColumn bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;

            //                        //        //    if (bizDataGridTextBoxColumn.MappingName == dc1.ColumnName)
            //                        //        //    {

            //                        //                //bizDataGridTextBoxColumn.Alignment = HorizontalAlignment.Center;
            //                        //                //bizDataGridTextBoxColumn.TextBoxGrid.BackColor = Color.FromKnownColor(KnownColor.Control);
            //                        //                //Button columnButton = new Button();
            //                        //                //columnButton.Text = "Browse";
            //                        //                //columnButton.FlatStyle = FlatStyle.Standard;
            //                        //                //columnButton.BackColor = Color.FromKnownColor(KnownColor.Control);
            //                        //                //columnButton.Size = new Size(75, 17);
            //                        //                //columnButton.Click += new EventHandler(columnButton_Click);
            //                        //                //bizDataGridTextBoxColumn.TextBoxGrid.Controls.Add(columnButton);


            //                        //            //}
            //                        //        //}
            //                            }
            //                    }

            //                }
            //            }

            //        }
            //    }
            //}
        }
        
    

        private void Datagrid2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataRow atr = this.dbaccess.DataSet.Tables["atr"].Rows[0];
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

                    DataRow drCur = getcurrentrow(Datagrid2);

                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim(), "ATR1");

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
         

        private void GetSummary()
        {
            DataRow atr = this.dbaccess.DataSet.Tables["atr"].Rows[0];
            DataTable atr2 = this.dbaccess.DataSet.Tables["atr2"];

            // Jason : 18-05-2015 - Changed for ATL requirements
            //string sql1 = "SELECT " +
            //                        "b.empnum, " +
            //                        "h.empname, " +
            //                        "b.wd as totalpayabledays, " +
            //                        "b.tothrs as TotalwkHrs, " +
            //                        "b.rcount as totalRest, " +
            //                        "b.dre as totaldre, " +
            //                                 "b.ot2 as totalot, " +
            //                        "b.ot15hrs, " +
            //                        "b.latecount as TotalLatecount, " +
            //                        "(b.latemins/60) as TotalLateHrs, " +
            //                        "b.pl as totalAleave, " +
            //                        "b.mc as totalmc, " +
            //                        "b.npl as totalupl, " +
            //                        "b.awol totalawol  " +
            //                        "from " +
            //                        "( " +
            //                             "select empnum,SUM(wdcount) as wd,SUM(mccount) as mc, SUM (nplcount) as npl, SUM(awolcount) as awol, SUM (plcount) as pl, SUM (hpcount) as hp, " +
            //                                  "SUM(phcount) as ph, SUM(workhrs) as tothrs,SUM(latecount) as latecount,sum(latemins) as latemins, sum(standbyhrs) as standbyhrs,sum(ot15hrs) as ot15hrs, " +
            //                                  //"SUM(dre) as dre, SUM(rcount) as rcount,SUM(ot2) as ot2  " +
            //                             "from " +
            //                             "( " +
            //                                  "select empnum,[Date] as WorkDate,[DAY],paytypecode,shiftcode,sectorcode,sitenum,timein,confirmedtimein,[timeout],confirmedtimeout, " +
            //                                       "case when shiftcode in ('MED','MC') then 1 else 0 end as mccount, " +
            //                                       "case when shiftcode in ('HPL','HOS') then 1 else 0 end as hpcount, " +
            //                                       "case when shiftcode like '%UPL%' then 1 else 0 end as nplcount, " +
            //                                       "case when shiftcode like 'AWO%' then 1 else 0 end as awolcount, " +
            //                                       "case when shiftcode in ('AL','A/L') then 1 else 0 end as plcount, " +
            //                                       "case when shiftcode like 'RES%' then 1 else 0 end as rcount, " +
            //                                       "case when shiftcode in ('PH') then 1 else 0 end as phcount, " +
            //                                       "case when ISNULL(TotalHrs,0)>0 then 1 else 0 end as wdcount,  " +
            //                                       "case when isnull(isDRE,0)=1 then 1 else 0 end as dre, " +
            //                                       "case when isnull(isRD,0)=1 then 1 else 0 end as rd, " +
            //                                       "case when isnull(isOT,0)=1 then 1 else 0 end as ot2, " +
            //                                       "ISNULL(TotalHrs,0) as workhrs, " +
            //                                       "case when ISNULL(LateMins,0)>10 then 1 else 0 end as latecount, " +
            //                                       "CASE WHEN ISNULL(latemins,0)<=10 THEN 0 ELSE ISNULL(latemins,0) end as latemins, " +
            //                                       "CASE WHEN TotalHrs>7.33 THEN TotalHrs-7.33 else 0 end as ot15hrs, " +
            //                                       "CASE WHEN ISNULL(otmins,0)>15 THEN ISNULL(otmins,0)/60 ELSE 0 end as standbyhrs, " +
            //                                       "refnum as reference  " +
            //                                  "from atr1 " +
            //                                  "where refnum='" + atr["refnum"].ToString() + "' " +
            //                             ") a " +
            //                             "group by empnum " +
            //                        ") b " +
            //                        "left join  " +
            //                        "( " +
            //                        "select empnum,empname " +
            //                        "from hemph " +
            //                        ") h on h.empnum = b.empnum";


            string sql1 = "SELECT  "+
	                           " b.empnum, "+  
	                            "h.empname,   "+
	                           " b.wd as totalpayabledays,   "+
	                            "b.workHrs as TotalwkHrs,   "+
	                            "b.latecount as TotalLatecount,   "+
                                "b.latemins as TotalLateHrs, " +
                                "b.actualothrs,  "+
                                "b.ot1hrs,  "+
                                "b.ot15hrs,  "+
                                "b.ot2hrs,  "+
                                "b.actualfixallow,  "+
	                            "b.rcount as totalRest,   "+
	                            "b.pl as totalAleave,   "+
	                            "b.mc as totalmc,   "+
	                            "b.npl as totalupl,   "+
                                "b.ab as totalab,   " +
	                            "b.awol totalawol    "+
	                            "from   "+
	                            "(   "+

                                  "select   "+
                                        "empnum,  "+
                                        "SUM(wd) as wd,  "+
                                        "SUM(mc) as mc,   "+
                                        "SUM(rcount) as rcount,  "+
                                        "SUM (npl) as npl,   "+
                                        "SUM(awol) as awol, "+
                                        "SUM (pl) as pl,   "+
                                        "SUM (hp) as hp, "+
                                        "SUM(ph) as ph,   "+
                                        "SUM(ab) as ab,   " +
                                        "SUM(workhrs) as workHrs,  "+
                                        "SUM(latecount) as latecount,  "+
                                        "sum(latemins) as latemins,   "+
                                        "sum(ot1hrs) as ot1hrs,   "+
                                        "sum(ot15hrs) as ot15hrs,   "+
                                        "sum(ot2hrs) as ot2hrs,   "+
                                        "sum(actualFixAllow) as actualfixallow,   "+
                                        "sum(actualothrs) as actualothrs  "+
                                        "from   "+
                                           " ( "+

		                                       " select   "+
				                                       " empnum,  "+
                                                        "WorkDate,  "+
                                                        "SUM(wdcount) as wd,   " +
				                                        "SUM(mccount) as mc,   "+
				                                        "SUM(rcount) as rcount,  "+
				                                        "SUM (nplcount) as npl,   "+
				                                        "SUM(awolcount) as awol,   "+
				                                        "SUM (plcount) as pl,   "+
				                                        "SUM (hpcount) as hp,   "+
				                                        "SUM(phcount) as ph,   "+
                                                        "SUM(abcount) as ab,   " +
				                                        "SUM(workhrs) as workHrs,  "+
				                                        "SUM(latecount) as latecount, "+
				                                        "sum(latemins) as latemins,   "+
				                                        "sum(ot1hrs) as ot1hrs,   "+
				                                        "sum(ot15hrs) as ot15hrs,   "+
				                                        "sum(ot2hrs) as ot2hrs,   "+
				                                        "sum(actualFixAllow) as actualfixallow,   "+
				                                        "sum(actualothrs) as actualothrs  "+
				                                        "from   "+
                                                            "(   "+
						                                        "select   "+
							                                        "empnum,  "+
							                                        "[Date] as WorkDate,  "+
							                                        "[DAY],  "+
							                                        "paytypecode,  "+
							                                        "shiftcode,  "+
							                                        "sectorcode,  "+
							                                        "sitenum,  "+
							                                        "timein,  "+
							                                        "confirmedtimein,  "+
							                                        "[timeout],  "+
							                                        "confirmedtimeout,   "+
							                                        "case when shiftcode in ('MED','MC') then 1 else 0 end as mccount, case when shiftcode in ('HPL','HOS') then 1 else 0 end as hpcount,   "+
							                                        "case when shiftcode like '%UPL%' then 1 else 0 end as nplcount, case when shiftcode like 'AWO%' then 1 else 0 end as awolcount,  "+
							                                        "case when shiftcode in ('AL','A/L') then 1 else 0 end as plcount, case when shiftcode like 'RES%' then 1 else 0 end as rcount,   "+
                                                                    "case when shiftcode in ('PH') then 1 else 0 end as phcount, case when ISNULL(attnmark,0)>0 then 1 else 0 end as wdcount,    "+
                                                                    "case when shiftcode in ('AB') then 1 else 0 end as abcount, "+
							                                        "case when isnull(isOT,0)=1 then 1 else 0 end as ot2, ISNULL(ACTUALTOTALHRS,0) as workhrs,   "+
							                                        "case when ISNULL(ActualLateMins,0)>0 then 1 else 0 end as latecount,   "+
							                                        "CASE WHEN ISNULL(Actuallatemins,0)<=0 THEN 0 ELSE ISNULL(latemins,0) end as latemins,   "+
                                                                    "CASE WHEN ISNULL(actualOT1,0)=1 THEN actualothrs else 0 end as ot1hrs,   "+
                                                                    "CASE WHEN ISNULL(actualOT15,0)=1 THEN actualothrs else 0 end as ot15hrs,   "+
                                                                    "CASE WHEN ISNULL(actualOT2,0)=1 THEN actualothrs else 0 end as ot2hrs,   "+
							                                        "CASE WHEN ISNULL(actualFixAllow,0)>0 THEN actualFixAllow else 0 end as actualFixAllow,   "+
							                                        "CASE WHEN ISNULL(actualothrs,0)>0 THEN actualothrs else 0 end as actualothrs,   "+
							                                        "refnum as reference    "+
                                                                "from atr1 where refnum='" + atr["refnum"].ToString() + "'  "+
                                                            ") a group by empnum,WorkDate   "+
                                            ") a1 group by empnum  "+
                                       ")b left join  ( select empnum,empname from hemph ) h on h.empnum = b.empnum order by b.empnum ";

            this.dbaccess.ReadSQL("tmpATR2", sql1);

            DataTable tmpATR2 = this.dbaccess.DataSet.Tables["tmpATR2"];

            BizFunctions.DeleteAllRows(atr2);

            if (tmpATR2.Rows.Count > 0)
            {
                foreach (DataRow dr1 in tmpATR2.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertATR2 = atr2.NewRow();
                        InsertATR2["empnum"] = dr1["empnum"];
                        InsertATR2["empname"] = dr1["empname"];
                        InsertATR2["totalpayabledays"] = dr1["totalpayabledays"];
                        InsertATR2["TotalwkHrs"] = dr1["TotalwkHrs"];
                        InsertATR2["totalRest"] = dr1["totalRest"];
                        //InsertATR2["totaldre"] = dr1["totaldre"];
                        //InsertATR2["totalot"] = dr1["totalot"];

                  
                        InsertATR2["actualothrs"] = dr1["actualothrs"];
                        InsertATR2["ot1hrs"] = dr1["ot1hrs"];
                        InsertATR2["ot15hrs"] = dr1["ot15hrs"];
                        InsertATR2["ot2hrs"] = dr1["ot2hrs"];
                        InsertATR2["actualfixallow"] = dr1["actualfixallow"];
               
                        

                        InsertATR2["TotalLatecount"] = dr1["TotalLatecount"];
                        InsertATR2["TotalLateHrs"] = dr1["TotalLateHrs"];
                        InsertATR2["totalAleave"] = dr1["totalAleave"];
                        InsertATR2["totalmc"] = dr1["totalmc"];
                        InsertATR2["totalupl"] = dr1["totalupl"];
                        InsertATR2["totalawol"] = dr1["totalawol"];
                        InsertATR2["totalab"] = dr1["totalab"];
                        atr2.Rows.Add(InsertATR2);
                    }
                }
            }

        }

        private void GetSummary2()
        {
            DataRow atr = this.dbaccess.DataSet.Tables["atr"].Rows[0];
            DataTable atr1 = this.dbaccess.DataSet.Tables["atr1"];
            DataTable atr2 = this.dbaccess.DataSet.Tables["atr2"];

            string getHempTmp = "select empnum,empname from hemph";

            this.dbaccess.ReadSQL("HempTmp", getHempTmp);

            //if (atr1.Rows.Count > 0)
            //{
            //    foreach (DataRow dr1 in atr1.Rows)
            //    {
            //        if (dr1.RowState != DataRowState.Deleted)
            //        {
            //            if(!BizFunctions.IsEmpty(dr1["shiftcode"]))
            //            {
            //                if (!(bool)isWorkShift(dr1["shiftcode"].ToString().Trim()))
            //                {
            //                    if (BizFunctions.IsEmpty(dr1["DayAmt"]))
            //                    {
            //                        dr1["DayAmt"] = 0;
            //                    }

            //                    if (Convert.ToDecimal(dr1["DayAmt"]) > 0)
            //                    {
            //                        if (dr1["shiftcode"].ToString().Trim().ToUpper() == "R")
            //                        {
            //                            dr1["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(dr1["DayAmt"]);
            //                        }
            //                        else if (dr1["shiftcode"].ToString().Trim().ToUpper() == "AL")
            //                        {
            //                            dr1["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(dr1["DayAmt"]);
            //                        }
            //                        else if (dr1["shiftcode"].ToString().Trim().ToUpper() == "MC")
            //                        {
            //                            dr1["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(dr1["DayAmt"]);
            //                        }
            //                        else if (dr1["shiftcode"].ToString().Trim().ToUpper() == "UPL")
            //                        {
            //                            dr1["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(dr1["DayAmt"]);
            //                        }
            //                        else if (dr1["shiftcode"].ToString().Trim().ToUpper() == "AWOL")
            //                        {
            //                            dr1["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(dr1["DayAmt"]);
            //                        }
            //                        else if (dr1["shiftcode"].ToString().Trim().ToUpper() == "HL")
            //                        {
            //                            dr1["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(dr1["DayAmt"]);
            //                        }
            //                        else
            //                        {
            //                            dr1["DayOffset"] = 0;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            decimal totalpayabledays = 0;
            decimal TotalwkHrs = 0;
            decimal TotalLatecount = 0;
            decimal TotalLateHrs = 0;
            decimal actualothrs = 0;
            decimal ot1hrs = 0;
            decimal ot15hrs = 0;
            decimal ot2hrs = 0;
            decimal actualfixallow = 0;
            decimal totalRest = 0;
            decimal totalAleave = 0;
            decimal totalmc = 0;
            decimal totalupl = 0;
            decimal totalab = 0;
            decimal totalawol = 0;

            //Jason: 29-12-2015
            //string sql1 = "SELECT  " +
            //              " b.empnum, " +
            //               "h.empname,   " +
            //              " b.wd as totalpayabledays,   " +
            //               "b.workHrs as TotalwkHrs,   " +
            //               "b.latecount as TotalLatecount,   " +
            //               "b.latemins as TotalLateHrs, " +
            //               "b.actualothrs,  " +
            //               "b.ot1hrs,  " +
            //               "b.ot15hrs,  " +
            //               "b.ot2hrs,  " +
            //               "b.actualfixallow,  " +
            //               "b.rcount as totalRest,   " +
            //               "b.pl as totalAleave,   " +
            //               "b.mc as totalmc,   " +
            //               "b.npl as totalupl,   " +
            //               "b.ab as totalab,   " +
            //               "b.awol totalawol    " +
            //               "from   " +
            //               "(   " +

            //                 "select   " +
            //                       "empnum,  " +
            //                       "SUM(wd) as wd,  " +
            //                       "SUM(mc) as mc,   " +
            //                       "SUM(rcount) as rcount,  " +
            //                       "SUM (npl) as npl,   " +
            //                       "SUM(awol) as awol, " +
            //                       "SUM (pl) as pl,   " +
            //                       "SUM (hp) as hp, " +
            //                       "SUM(ph) as ph,   " +
            //                       "SUM(ab) as ab,   " +
            //                       "SUM(workhrs) as workHrs,  " +
            //                       "SUM(latecount) as latecount,  " +
            //                       "sum(latemins) as latemins,   " +
            //                       "sum(ot1hrs) as ot1hrs,   " +
            //                       "sum(ot15hrs) as ot15hrs,   " +
            //                       "sum(ot2hrs) as ot2hrs,   " +
            //                       "sum(actualFixAllow) as actualfixallow,   " +
            //                       "sum(actualothrs) as actualothrs  " +
            //                       "from   " +
            //                          " ( " +

            //                              " select   " +
            //                                      " empnum,  " +
            //                                       "WorkDate,  " +
            //                                       "SUM(wdcount) as wd,   " +
            //                                       "SUM(mccount) as mc,   " +
            //                                       "SUM(rcount) as rcount,  " +
            //                                       "SUM (nplcount) as npl,   " +
            //                                       "SUM(awolcount) as awol,   " +
            //                                       "SUM (plcount) as pl,   " +
            //                                       "SUM (hpcount) as hp,   " +
            //                                       "SUM(phcount) as ph,   " +
            //                                       "SUM(abcount) as ab,   " +
            //                                       "SUM(workhrs) as workHrs,  " +
            //                                       "SUM(latecount) as latecount, " +
            //                                       "sum(latemins) as latemins,   " +
            //                                       "sum(ot1hrs) as ot1hrs,   " +
            //                                       "sum(ot15hrs) as ot15hrs,   " +
            //                                       "sum(ot2hrs) as ot2hrs,   " +
            //                                       "sum(actualFixAllow) as actualfixallow,   " +
            //                                       "sum(actualothrs) as actualothrs  " +
            //                                       "from   " +
            //                                           "(   " +
            //                                               "select   " +
            //                                                   "empnum,  " +
            //                                                   "[Date] as WorkDate,  " +
            //                                                   "[DAY],  " +
            //                                                   "paytypecode,  " +
            //                                                   "shiftcode,  " +
            //                                                   "sectorcode,  " +
            //                                                   "sitenum,  " +
            //                                                   "timein,  " +
            //                                                   "confirmedtimein,  " +
            //                                                   "[timeout],  " +
            //                                                   "confirmedtimeout,   " +
            //                                                    "case when shiftcode in ('MED','MC') then ISNULL(DayAmt,0) else 0 end as mccount, "+
            //                                                    "case when shiftcode in ('HPL','HOS') then ISNULL(DayAmt,0) else 0 end as hpcount,    "+
            //                                                    "case when shiftcode like '%UPL%' then ISNULL(DayAmt,0) else 0 end as nplcount,  "+
            //                                                    "case when shiftcode like 'AWO%' then ISNULL(DayAmt,0) else 0 end as awolcount,   "+
            //                                                    "case when shiftcode in ('AL','A/L') then ISNULL(DayAmt,0) else 0 end as plcount,  "+
            //                                                    "case when shiftcode like 'RES%' then ISNULL(DayAmt,0) else 0 end as rcount,    "+
            //                                                    "case when shiftcode in ('PH') then ISNULL(DayAmt,0) else 0 end as phcount,  "+
            //                                                    "case when shiftcode NOT in ('AWOL','AB','UPL') then ISNULL(DayAmt,0) else 0 end wdcount,  "+
            //                                                    "case when shiftcode in ('AB') then ISNULL(DayAmt,0) else 0 end as abcount,  "+
            //                                                    "case when isnull(isOT,0)=1 then 1 else 0 end as ot2, ISNULL(ACTUALTOTALHRS,0) as workhrs,    "+
            //                                                    "case when ISNULL(ActualLateMins,0)>0 then 1 else 0 end as latecount,    "+
            //                                                    "CASE WHEN ISNULL(Actuallatemins,0)<=0 THEN 0 ELSE ISNULL(Actuallatemins,0) end as latemins,   "+ 
            //                                                    "CASE WHEN ISNULL(actualOT1,0)=1 THEN actualothrs else 0 end as ot1hrs,    "+
            //                                                    "CASE WHEN ISNULL(actualOT15,0)=1 THEN actualothrs else 0 end as ot15hrs,    "+
            //                                                    "CASE WHEN ISNULL(actualOT2,0)=1 THEN actualothrs else 0 end as ot2hrs,    "+
            //                                                    "CASE WHEN ISNULL(actualFixAllow,0)>0 THEN actualFixAllow else 0 end as actualFixAllow,    "+
            //                                                    "CASE WHEN ISNULL(actualothrs,0)>0 THEN actualothrs else 0 end as actualothrs,    "+
            //                                                    "refnum as reference     " +
            //                                               "from atr1 where refnum='" + atr["refnum"].ToString() + "' AND ISNULL(isOmit,0)=0  " +
            //                                           ") a group by empnum,WorkDate   " +
            //                           ") a1 group by empnum  " +
            //                      ")b left join  ( select empnum,empname from HempTmp ) h on h.empnum = b.empnum order by b.empnum ";

            // Jason: 05-01-2016
            string sql1 = "SELECT    " +
                                "b.empnum,  " +
                                "h.empname,     " +
                                "b.wd as totalpayabledays,    " +
                                "b.workHrs as TotalwkHrs,    " +
                                "b.latecount as TotalLatecount, " +
                                "b.latemins as TotalLateHrs,  " +
                                "b.actualothrs,   " +
                                "b.ot1hrs,   " +
                                "b.ot15hrs,   " +
                                "b.ot2hrs,   " +
                                "b.actualfixallow,   " +
                                "b.rcount as totalRest,    " +
                                "b.pl as totalAleave,    " +
                                "b.mc as totalmc,    " +
                                "b.hp as totalhp,     " +
                                "b.npl as totalupl,    " +
                                "b.ab as totalab,    " +
                                "b.awol totalawol     " +
                                "from    " +
                                "(    " +
                                    "select    " +
                                        "empnum,   " +
                                        "SUM(wd) as wd,   " +
                                        "SUM(mc) as mc,    " +
                                        "SUM(rcount) as rcount,   " +
                                        "SUM (npl) as npl,    " +
                                        "SUM(awol) as awol,  " +
                                        "SUM (pl) as pl,    " +
                                        "SUM (hp) as hp,  " +
                                        "SUM(ph) as ph,    " +
                                        "SUM(ab) as ab,    " +
                                        "SUM(workhrs) as workHrs,   " +
                                        "SUM(latecount) as latecount,   " +
                                        "sum(latemins) as latemins,    " +
                                        "sum(ot1hrs) as ot1hrs,    " +
                                        "sum(ot15hrs) as ot15hrs,    " +
                                        "sum(ot2hrs) as ot2hrs,    " +
                                        "sum(actualFixAllow) as actualfixallow,  " +
                                        "sum(actualothrs) as actualothrs   " +
                                        "from     " +
                                        "(   " +
                                            "select     " +
                                                "empnum,  " +
                                                "WorkDate,   " +
                                                "SUM(wdcount) as wd,    " +
                                                "SUM(mccount) as mc,   " +
                                                "SUM(rcount) as rcount,   " +
                                                "SUM (nplcount) as npl,    " +
                                                "SUM(awolcount) as awol,    " +
                                                "SUM (plcount) as pl,    " +
                                                "SUM (hpcount) as hp,   " +
                                                "SUM(phcount) as ph,    " +
                                                "SUM(abcount) as ab,    " +
                                                "SUM(workhrs) as workHrs, " +
                                                "SUM(latecount) as latecount,  " +
                                                "sum(latemins) as latemins,    " +
                                                "sum(ot1hrs) as ot1hrs,    " +
                                                "sum(ot15hrs) as ot15hrs,    " +
                                                "sum(ot2hrs) as ot2hrs,    " +
                                                "sum(actualFixAllow) as actualfixallow,    " +
                                                "sum(actualothrs) as actualothrs   " +
                                                "from    " +
                                                "(    " +
                                                    "select    " +
                                                        "empnum,   " +
                                                        "[Date] as WorkDate,   " +
                                                        "[DAY],   " +
                                                        "paytypecode,  " +
                                                        "shiftcode,   " +
                                                        "sectorcode,   " +
                                                        "sitenum,   " +
                                                        "timein,   " +
                                                        "confirmedtimein,   " +
                                                        "[timeout],   " +
                                                        "case when shiftcode in ('MED','MC') then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end as mccount,    " +
                                                        "case when shiftcode in ('HPL','HOS') then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end as hpcount,      " +
                                                        "case when shiftcode like '%UPL%' then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end as nplcount,    " +
                                                        "case when shiftcode like 'AWO%' then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end as awolcount,     " +
                                                        "case when shiftcode in ('AL','A/L') then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end as plcount,    " +
                                                        "case when shiftcode like 'RES%' then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end as rcount,      " +
                                                        "case when shiftcode in ('PH') then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end as phcount,    " +
                                                        "case when shiftcode NOT in ('AWOL','AB','UPL') then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) else CONVERT(decimal(16,2),0) end wdcount,    " +
                                                        "case when (shiftcode in ('AB') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('AB') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0)  end as abcount,   " +
                                                        "case when isnull(isOT,0)=1 then 1 else 0 end as ot2,    " +
                                                        "CONVERT(decimal(16,2),ISNULL(ACTUALTOTALHRS,0)) as workhrs,      " +
                                                        "case when ISNULL(ActualLateMins,0)>0 then 1 else 0 end as latecount,      " +
                                                        "CASE WHEN ISNULL(Actuallatemins,0)<=0 THEN CONVERT(decimal(16,2),0) ELSE CONVERT(decimal(16,2),ISNULL(Actuallatemins,0)) end as latemins,      " +
                                                        "CASE WHEN ISNULL(actualOT1,0)=1 THEN actualothrs else 0 end as ot1hrs,      " +
                                                        "CASE WHEN ISNULL(actualOT15,0)=1 THEN actualothrs else 0 end as ot15hrs,      " +
                                                        "CASE WHEN ISNULL(actualOT2,0)=1 THEN actualothrs else 0 end as ot2hrs,      " +
                                                        "CASE WHEN ISNULL(actualFixAllow,0)>0 THEN CONVERT(decimal(16,2),actualFixAllow) else CONVERT(decimal(16,2),0) end as actualFixAllow,      " +
                                                        "CASE WHEN ISNULL(actualothrs,0)>0 THEN CONVERT(decimal(16,2),actualothrs) else CONVERT(decimal(16,2),0) end as actualothrs,      " +
                                                        "refnum as reference       " +
                                                        "from atr1 " +
                                                        "where refnum='" + atr["refnum"].ToString() + "' AND ISNULL(isOmit,0)=0  ) a group by empnum,WorkDate  " +
                                                 ") a1 group by empnum   " +
                                        ")b left join  ( select empnum,empname from HempTmp ) h on h.empnum = b.empnum order by b.empnum ";


            //string sql1 = "SELECT    " +
            //                    "b.empnum,  " +
            //                    "h.empname,     " +
            //                    "b.wd as totalpayabledays,    " +
            //                    "b.workHrs as TotalwkHrs,    " +
            //                    "b.latecount as TotalLatecount, " +
            //                    "b.latemins as TotalLateHrs,  " +
            //                    "b.actualothrs,   " +
            //                    "b.ot1hrs,   " +
            //                    "b.ot15hrs,   " +
            //                    "b.ot2hrs,   " +
            //                    "b.actualfixallow,   " +
            //                    "b.rcount as totalRest,    " +
            //                    "b.pl as totalAleave,    " +
            //                    "b.mc as totalmc,    " +
            //                    "b.hp as totalhp,     "+
            //                    "b.npl as totalupl,    " +
            //                    "b.ab as totalab,    " +
            //                    "b.awol totalawol     " +
            //                    "from    " +
            //                    "(    " +
            //                        "select    " +
            //                            "empnum,   " +
            //                            "SUM(wd) as wd,   " +
            //                            "SUM(mc) as mc,    " +
            //                            "SUM(rcount) as rcount,   " +
            //                            "SUM (npl) as npl,    " +
            //                            "SUM(awol) as awol,  " +
            //                            "SUM (pl) as pl,    " +
            //                            "SUM (hp) as hp,  " +
            //                            "SUM(ph) as ph,    " +
            //                            "SUM(ab) as ab,    " +
            //                            "SUM(workhrs) as workHrs,   " +
            //                            "SUM(latecount) as latecount,   " +
            //                            "sum(latemins) as latemins,    " +
            //                            "sum(ot1hrs) as ot1hrs,    " +
            //                            "sum(ot15hrs) as ot15hrs,    " +
            //                            "sum(ot2hrs) as ot2hrs,    " +
            //                            "sum(actualFixAllow) as actualfixallow,  " +
            //                            "sum(actualothrs) as actualothrs   " +
            //                            "from     " +
            //                            "(   " +
            //                                "select     " +
            //                                    "empnum,  " +
            //                                    "WorkDate,   " +
            //                                    "SUM(wdcount) as wd,    " +
            //                                    "SUM(mccount) as mc,   " +
            //                                    "SUM(rcount) as rcount,   " +
            //                                    "SUM (nplcount) as npl,    " +
            //                                    "SUM(awolcount) as awol,    " +
            //                                    "SUM (plcount) as pl,    " +
            //                                    "SUM (hpcount) as hp,   " +
            //                                    "SUM(phcount) as ph,    " +
            //                                    "SUM(abcount) as ab,    " +
            //                                    "SUM(workhrs) as workHrs, " +
            //                                    "SUM(latecount) as latecount,  " +
            //                                    "sum(latemins) as latemins,    " +
            //                                    "sum(ot1hrs) as ot1hrs,    " +
            //                                    "sum(ot15hrs) as ot15hrs,    " +
            //                                    "sum(ot2hrs) as ot2hrs,    " +
            //                                    "sum(actualFixAllow) as actualfixallow,    " +
            //                                    "sum(actualothrs) as actualothrs   " +
            //                                    "from    " +
            //                                    "(    " +
            //                                        "select    " +
            //                                            "empnum,   " +
            //                                            "[Date] as WorkDate,   " +
            //                                            "[DAY],   " +
            //                                            "paytypecode,  " +
            //                                            "shiftcode,   " +
            //                                            "sectorcode,   " +
            //                                            "sitenum,   " +
            //                                            "timein,   " +
            //                                            "confirmedtimein,   " +
            //                                            "[timeout],   " +
            //                                            "case when (shiftcode in ('MED','MC') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('MED','MC') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end as mccount,      " +
            //                                            "case when (shiftcode in ('HPL','HOS','HL') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('HPL','HOS','HL') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end as hpcount,        " +
            //                                            "case when (shiftcode in ('UPL') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('UPL') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end as nplcount,      " +
            //                                            "case when (shiftcode in ('AWOL') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('AWOL') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end as awolcount,       " +
            //                                            "case when (shiftcode in ('AL','A/L') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('AL','A/L') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end as plcount,      " +
            //                                            "case when (shiftcode in ('R','REST') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('R','REST')and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end as rcount,        " +
            //                                            "case when (shiftcode in ('PH') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('PH') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end as phcount,      " +
            //                                            "case when (shiftcode not in ('AWOL','AB','UPL') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('AWOL','AB','UPL') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0) end wdcount,      " +
            //                                            "case when (shiftcode in ('AB') and ISNULL(DayAmt,0)>0) then CONVERT(decimal(16,2),ISNULL(DayAmt,0)) when (shiftcode in ('AB') and ISNULL(DayAmt,0)=0) then CONVERT(decimal(16,2),1) else CONVERT(decimal(16,2),0)  end as abcount,     " +
            //                                            "case when isnull(isOT,0)=1 then 1 else 0 end as ot2,    " +
            //                                            "CONVERT(decimal(16,2),ISNULL(ACTUALTOTALHRS,0)) as workhrs,      " +
            //                                            "case when ISNULL(ActualLateMins,0)>0 then 1 else 0 end as latecount,      " +
            //                                            "CASE WHEN ISNULL(Actuallatemins,0)<=0 THEN CONVERT(decimal(16,2),0) ELSE CONVERT(decimal(16,2),ISNULL(Actuallatemins,0)) end as latemins,      " +
            //                                            "CASE WHEN ISNULL(actualOT1,0)=1 THEN actualothrs else 0 end as ot1hrs,      " +
            //                                            "CASE WHEN ISNULL(actualOT15,0)=1 THEN actualothrs else 0 end as ot15hrs,      " +
            //                                            "CASE WHEN ISNULL(actualOT2,0)=1 THEN actualothrs else 0 end as ot2hrs,      " +
            //                                            "CASE WHEN ISNULL(actualFixAllow,0)>0 THEN CONVERT(decimal(16,2),actualFixAllow) else CONVERT(decimal(16,2),0) end as actualFixAllow,      " +
            //                                            "CASE WHEN ISNULL(actualothrs,0)>0 THEN CONVERT(decimal(16,2),actualothrs) else CONVERT(decimal(16,2),0) end as actualothrs,      " +
            //                                            "refnum as reference       " +
            //                                            "from atr1 " +
            //                                            "where refnum='" + atr["refnum"].ToString() + "' AND ISNULL(isOmit,0)=0  ) a group by empnum,WorkDate  " +
            //                                     ") a1 group by empnum   " +
            //                            ")b left join  ( select empnum,empname from HempTmp ) h on h.empnum = b.empnum order by b.empnum ";
                            			

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, sql1);

            if (dt1.Rows.Count > 0)
            {
                if (atr2.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(atr2);
                }
                foreach (DataRow dr1 in dt1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertATR2 = atr2.NewRow();
                        InsertATR2["empnum"] = dr1["empnum"];
                        InsertATR2["empname"] = dr1["empname"];
                        InsertATR2["totalpayabledays"] = dr1["totalpayabledays"];
                        InsertATR2["TotalwkHrs"] = dr1["TotalwkHrs"];
                        InsertATR2["totalRest"] = dr1["totalRest"];
                        InsertATR2["actualothrs"] = dr1["actualothrs"];
                        InsertATR2["ot1hrs"] = dr1["ot1hrs"];
                        InsertATR2["ot15hrs"] = dr1["ot15hrs"];
                        InsertATR2["ot2hrs"] = dr1["ot2hrs"];
                        InsertATR2["actualfixallow"] = dr1["actualfixallow"];
                        InsertATR2["TotalLatecount"] = dr1["TotalLatecount"];
                        InsertATR2["TotalLateHrs"] = dr1["TotalLateHrs"];
                        InsertATR2["totalAleave"] = dr1["totalAleave"];
                        InsertATR2["totalmc"] = dr1["totalmc"];
                        InsertATR2["totalupl"] = dr1["totalupl"];
                        InsertATR2["totalawol"] = dr1["totalawol"];
                        InsertATR2["totalab"] = dr1["totalab"];
                        InsertATR2["totalhl"] = dr1["totalhp"];
                        atr2.Rows.Add(InsertATR2);
                    }
                }
            }

       
          

        }


        #region Column Changed

        private void Voucher_ATR1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {           

            switch (e.Column.ColumnName)
            {
                   

                case "actualOT1":
                    {
                        if (BizFunctions.IsEmpty(e.Row["actualOT1"]))
                        {
                            e.Row["actualOT1"] = 0;
                        }
                        if ((bool)e.Row["actualOT1"])
                        {
                            e.Row["actualOT15"] = 0;
                            e.Row["actualOT2"] = 0;
                        }
                    }
                    break;

                case "actualOT15":
                    {
                        if (BizFunctions.IsEmpty(e.Row["actualOT15"]))
                        {
                            e.Row["actualOT15"] = 0;
                        }
                        if ((bool)e.Row["actualOT15"])
                        {
                            e.Row["actualOT1"] = 0;
                            e.Row["actualOT2"] = 0;
                        }
                    }
                    break;

                case "actualOT2":
                    {
                        if (BizFunctions.IsEmpty(e.Row["actualOT2"]))
                        {
                            e.Row["actualOT2"] = 0;
                        }
                        if ((bool)e.Row["actualOT2"])
                        {
                            e.Row["actualOT1"] = 0;
                            e.Row["actualOT15"] = 0;
                        }
                    }
                    break;

                case "shiftcode":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["shiftcode"]))
                        {
                            e.Row["timein"] = geTimeIn(e.Row["shiftcode"].ToString());
                            e.Row["timeout"] = geTimeOut(e.Row["shiftcode"].ToString());

                            if (isWorkShift(e.Row["shiftcode"].ToString()) && !BizFunctions.IsEmpty(e.Row["Date"]))
                            {
                                e.Row["Scheddatein"] = Convert.ToDateTime(e.Row["Date"]).ToShortDateString();
                            }
                            else
                            {
                                //e.Row["Scheddatein"] = System.DBNull.Value;
                            }
                        }

                        if (!BizFunctions.IsEmpty(e.Row["DayAmt"]))
                        {
                            if (Convert.ToDecimal(e.Row["DayAmt"]) == 0)
                            {
                                e.Row["DayAmt"] = GetDayAmtShift(e.Row["shiftcode"].ToString());
                            }
                        }
                        else
                        {
                            e.Row["DayAmt"] = GetDayAmtShift(e.Row["shiftcode"].ToString());
                        }
                        
                    }
                    break;

                case "DayAmt":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["shiftcode"]))
                        {
                            if (!(bool)isWorkShift(e.Row["shiftcode"].ToString().Trim()))
                            {
                                if (BizFunctions.IsEmpty(e.Row["DayAmt"]))
                                {
                                    e.Row["DayAmt"] = 0;
                                }

                                if (Convert.ToDecimal(e.Row["DayAmt"]) > 0)
                                {
                                    if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "R")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "AL")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "MC")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "UPL")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "AWOL")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "HL")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "AB")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "OFF")
                                    {
                                        e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    }
                                    //else if (e.Row["shiftcode"].ToString().Trim().ToUpper() == "PH")
                                    //{
                                    //    e.Row["DayOffset"] = Convert.ToDecimal(1.00) - Convert.ToDecimal(e.Row["DayAmt"]);
                                    //}
                                    else
                                    {
                                        e.Row["DayOffset"] = 0;
                                    }
                                }
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

                                        //if (ftime2 > ftime1)
                                        //{
                                        //    e.Row["LateMins"] = ftime2 - ftime1;
                                        //}
                                        //else
                                        //{
                                        //    e.Row["LateMins"] = 0;
                                        //}

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

                                        //if (ftime2 > ftime1)
                                        //{
                                        //    e.Row["LateMins"] = ftime2 - ftime1;
                                        //}
                                        //else
                                        //{
                                        //    e.Row["LateMins"] = 0;
                                        //}

                                    }


                                    // GET OT MINS
                                    if (!BizFunctions.IsEmpty(e.Row["totalhrs"]))
                                    {
                                        int TotalHrs = Convert.ToInt32(e.Row["totalhrs"]);
                                        if (TotalHrs > 0)
                                        {
                                            if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]) && !BizFunctions.IsEmpty(e.Row["confirmedtimeout"]) && !BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                            {
                                                decimal tmpTTL1, tmpTTL2= 0;

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

                //case "latemins":
                //    {
                //        if (!BizFunctions.IsEmpty(e.Row["latemins"]))
                //        {
                //            decimal latemins = Convert.ToDecimal(e.Row["latemins"]);

                //            if (latemins >= 15)
                //            {
                //                e.Row.set
                //            }

                //            e.Row["rempname"] = getEmpName(e.Row["rempnum"].ToString());
                //        }
                //    }
                //    break;


              
            }
        }

        #endregion

        #region Initialise

        private void Initialise()
        {

            Btn_Sort = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "Btn_Sort") as Button;
            Btn_Extract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Extract") as Button;
            TableColumn = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "TableColumn") as ComboBox;
            atr_wrrnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "atr_wrrnum") as TextBox;

            Btn_Sort.Click +=new EventHandler(Btn_Sort_Click);
            Btn_Extract.Click +=new EventHandler(Btn_Extract_Click);

            //txt_editEmpno = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_editEmpno") as TextBox;
            //txt_editEmpno.KeyDown += new KeyEventHandler(txt_editEmpno_KeyDown);
            //txt_editEmpno.DoubleClick += new EventHandler(txt_editEmpno_DoubleClick);

            //Btn_Show = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Show") as Button;
            //Btn_Show.Click += new EventHandler(Btn_Show_Click);



            //atr1_sectorcode = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "atr1_sectorcode") as TextBox;
        
            //atr1_day = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "atr1_day") as TextBox;


            //atr1_atrdate = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "atr1_atrdate") as DateTimePicker;

            //btnExtract1 = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "Btn_Extract") as Button;
            //btnExtract1.Click += new EventHandler(btnExtract1_Click);


        }

        #endregion

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

        #region Button Extract Event

        private void Btn_Extract_Click(object sender, EventArgs e)
        {
            DataRow atr = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            DataTable atr1 = this.dbaccess.DataSet.Tables["atr1"];
            if (!BizFunctions.IsEmpty(atr["wrrFromDate"]) && !BizFunctions.IsEmpty(atr["wrrToDate"]))
            {
                Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
                DataTable oriTable = atr1;
                try
                {

                    ExtractALiveForm2 ExtractAL = new ExtractALiveForm2(this.dbaccess, oriTable);
                    ExtractAL.ShowDialog(frm);
                    HighlightLateRows(); ;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Can't Extract if WRR refnum is Empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }

        #endregion

        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataTable atr1 = this.dbaccess.DataSet.Tables["atr1"];
            string getLeaveRecord = "Select * from lvr  where leaveyear='" + Common.DEFAULT_SYSTEM_YEAR + "' and  [status]<>'V'";

            this.dbaccess.ReadSQL("LVRtmp", getLeaveRecord);

            if (atr1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in atr1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["shiftcode"]))
                        {
                            dr1["timein"] = geTimeIn(dr1["shiftcode"].ToString());
                            dr1["timeout"] = geTimeOut(dr1["shiftcode"].ToString());
                        }

                        if (!BizFunctions.IsEmpty(dr1["empnum"]))
                        {
                            dr1["empname"] = getEmpName(dr1["empnum"].ToString());
                         
                        }

                        if (BizFunctions.IsEmpty(dr1["actualFixAllow"]))
                        {
                            dr1["actualFixAllow"] = dr1["FixAllowAmt"];
                        }

                        if (BizFunctions.IsEmpty(dr1["actualLateMins"]))
                        {
                            dr1["actualLateMins"] = dr1["LateMins"];
                        }

                        if (BizFunctions.IsEmpty(dr1["actualWorkHrs"]))
                        {
                            dr1["actualWorkHrs"] = dr1["TotalHrs"];
                        }

                        if (BizFunctions.IsEmpty(dr1["actualOT1"]))
                        {
                            dr1["actualOT1"] = dr1["actualOT1"];
                        }
                        if (BizFunctions.IsEmpty(dr1["actualOT15"]))
                        {
                            dr1["actualOT15"] = dr1["OT15"];
                        }

                        if (BizFunctions.IsEmpty(dr1["ACTUALTOTALHRS"]))
                        {
                            dr1["ACTUALTOTALHRS"] = dr1["TotalHrs"];
                        }


                        if (BizFunctions.IsEmpty(dr1["ActualOTHrs"]))
                        {
                            dr1["ActualOTHrs"] = dr1["OTHrs"];
                        }


                        if (BizFunctions.IsEmpty(dr1["actualAttnRemark"]))
                        {
                            dr1["actualAttnRemark"] = dr1["AttnRemarks"];
                        }


                        if (BizFunctions.IsEmpty(dr1["actualFixAllow"]))
                        {
                            dr1["actualFixAllow"] = dr1["FixAllow"];
                        }


                        if (BizFunctions.IsEmpty(dr1["lvrnum"]))
                        {
                            if(!BizFunctions.IsEmpty(dr1["empnum"]) && !BizFunctions.IsEmpty(dr1["date"]))
                            {
                                dr1["lvrnum"] = GetLeaveRefnum(dr1["empnum"].ToString(), Convert.ToDateTime(dr1["date"]));

                                
                                
                            }
                        }
                        if (!BizFunctions.IsEmpty(dr1["lvrnum"]))
                        {
                            if (BizFunctions.IsEmpty(dr1["sitenum"]))
                            {
                                dr1["sitenum"] = GetLeaveSitenum(dr1["lvrnum"].ToString());
                            }
                        }



                    }
                }
            }
            
  
        }


        #region DocumentF2

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {           
            base.AddDocumentF3Condition(sender, e);
            DataRow atr = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            switch (e.ControlName)
            {
                case "atr_wrrnum":
                    {
                       atr["Sitenum"] = e.F2CurrentRow["sitenum"];
                       //e.CurrentRow["Sitename"] = e.F2CurrentRow["sitename"];
                       atr["sectorcode"] = e.F2CurrentRow["sectorcode"];
                       atr["wrrFromDate"] = e.F2CurrentRow["commencedate"];
                       atr["wrrToDate"] = e.F2CurrentRow["enddate"];

                       if (!BizFunctions.IsEmpty(atr["wrrnum"].ToString()))
                       {
                           GetWrr();
                           //GetATMR();
                       }
                       else
                       {
                           MessageBox.Show("Can't Extract if WRR refnum is Empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                       }
                  
                    }
                    break;


            }
        }
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow atr = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            switch (e.ControlName)
            {
                   
                case "atr_wrrnum":
                    {
                        if (atr_wrrnum.Text != string.Empty)
                        {
                            //NO Sector code
                            //e.Condition = "(sitenum like '%" + atr_wrrnum.Text.Trim() + "%' OR refnum like '%" + atr_wrrnum.Text.Trim() + "') and SectorCode='" + SectorCode + "'";
                            e.Condition = "(sitenum like '%" + atr_wrrnum.Text.Trim() + "%' OR refnum like '%" + atr_wrrnum.Text.Trim() + "') ";
                        }
                        //else
                        //{
                        //    e.Condition = "SectorCode='" + SectorCode + "'";
                        //}
                        //e.Condition = "SectorCode='" + SectorCode + "'";   
                        //e.Condition = BizFunctions.F2Condition("sitenum,refnum", (sender as TextBox).Text);
                        
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
            DataTable atr1 = this.dbaccess.DataSet.Tables["atr1"];
            TableColumn = (ComboBox)BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "TableColumn");


            string cname = TableColumn.Text.ToString();
            if (cname != "")
            {
                if (atr1.Rows.Count > 0)
                {
                    SortDT sort = new SortDT(atr1, cname + " ASC");
                    DataTable returnedfinalextraction = sort.SortedTable();

                    BizFunctions.DeleteAllRows(atr1);

                    foreach (DataRow dr in returnedfinalextraction.Select())
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            atr1.ImportRow(dr);
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

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            DataRow atr = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            DataTable atr1 = this.dbaccess.DataSet.Tables["ATR1"];
            DataTable atr2 = this.dbaccess.DataSet.Tables["ATR2"];

            DataTable wkc = e.DBAccess.DataSet.Tables["wkch"];
            DataTable wkc1 = e.DBAccess.DataSet.Tables["wkc1"];
            DataTable wkc2 = e.DBAccess.DataSet.Tables["wkc2"];

            base.Document_SaveBegin_OnClick(sender, e);

            atr["user"] = Common.DEFAULT_DOCUMENT_USER;

            if(BizFunctions.IsEmpty(atr["trandate"]))
            {
                atr["trandate"] = DateTime.Now;
            }

            #region ATR1
            foreach (DataRow dr1 in atr1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(atr, dr1, "refnum/sectorcode/wrrnum/user/flag/status/trandate/created/modified");

                    if (BizFunctions.IsEmpty(dr1["sitenum"]))
                    {
                        dr1["sitenum"] = atr["sitenum"];
                    }

                    if (BizFunctions.IsEmpty(dr1["timein"]))
                    {
                        dr1["timein"] = geTimeIn(dr1["shiftcode"].ToString());
                    }

                    if (BizFunctions.IsEmpty(dr1["timeout"]))
                    {
                        dr1["timeout"] = geTimeOut(dr1["shiftcode"].ToString());

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

                    if (!BizFunctions.IsEmpty(dr1["confirmedtimein"]))
                    {
                        if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(dr1["confirmedtimein"].ToString()))
                        {
                            dr1["confirmedtimein"] = System.DBNull.Value;
                        }
                        else
                        {
                            if (!BizFunctions.IsEmpty(dr1["confirmedtimeout"]))
                            {
                                if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(dr1["confirmedtimein"].ToString()))
                                {
                                    dr1["confirmedtimeout"] = System.DBNull.Value;
                                }
                                else
                                {

                                    if (Convert.ToDouble(dr1["confirmedtimein"]) <= Convert.ToDouble(dr1["confirmedtimeout"]))
                                    {
                                        dr1["totalhrs"] = Math.Round(Math.Abs(GetHours(dr1["confirmedtimein"].ToString(), dr1["confirmedtimeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                                        //dr1["totalhrs"] =Math.Abs(GetHours(dr1["confirmedtimein"].ToString(), dr1["confirmedtimeout"].ToString()));
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

                                    //if (ftime2 > ftime1)
                                    //{
                                    //    dr1["LateMins"] = ftime2 - ftime1;
                                    //}
                                    //else
                                    //{
                                    //    dr1["LateMins"] = 0;
                                    //}

                                }
                            }
                        }

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

                                    //if (ftime2 > ftime1)
                                    //{
                                    //    dr1["LateMins"] = ftime2 - ftime1;
                                    //}
                                    //else
                                    //{
                                    //    dr1["LateMins"] = 0;
                                    //}

                                }


                                // GET OT MINS
                                if (!BizFunctions.IsEmpty(dr1["totalhrs"]))
                                {
                                    int TotalHrs = Convert.ToInt32(dr1["totalhrs"]);
                                    if (TotalHrs > 0)
                                    {
                                        if (!BizFunctions.IsEmpty(dr1["confirmedtimein"]) && !BizFunctions.IsEmpty(dr1["confirmedtimeout"]) && !BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["timeout"]))
                                        {
                                            decimal tmpTTL1, tmpTTL2 = 0;

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
                        dr1["empnum4"] = dr1["empnum"];
                    }

                    if (!BizFunctions.IsEmpty(dr1["date"]) && BizFunctions.IsEmpty(dr1["Day"]))
                    {
                        dr1["Day"] = TimeTools.GetDay(TimeTools.GetDayOfWeekNo(Convert.ToDateTime(dr1["Date"]).DayOfWeek.ToString()));
                    }


                   
                }

            }
            #endregion

            foreach (DataRow dr2 in atr2.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(atr, dr2, "refnum/user/flag/status/trandate/createdby/created/modified");
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


        private decimal GetDayAmtShift(string shiftcode)
        {
            decimal dayAmt = 0;

            string GetWorkDayAmt = "Select DayAmt from SHM where shiftcode='" + shiftcode + "'";

            DataTable GetWorkDayAmtTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetWorkDayAmt);

            if (GetWorkDayAmtTmp.Rows.Count > 0)
            {
                if (BizFunctions.IsEmpty(GetWorkDayAmtTmp.Rows[0]["DayAmt"]))
                {
                    GetWorkDayAmtTmp.Rows[0]["DayAmt"] = 0;
                }
                dayAmt = Convert.ToDecimal(GetWorkDayAmtTmp.Rows[0]["DayAmt"]);
            }

            return dayAmt;
        }

        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow atr = e.DBAccess.DataSet.Tables["ATR"].Rows[0];
            DataTable atr1 = e.DBAccess.DataSet.Tables["ATR1"];
            DataTable atr2 = e.DBAccess.DataSet.Tables["ATR2"];
            
            
            DataTable wkch = e.DBAccess.DataSet.Tables["wkch"];
            DataTable wkc1 = e.DBAccess.DataSet.Tables["wkc1"];
            DataTable wkc2 = e.DBAccess.DataSet.Tables["wkc2"];

            if (atr2.Rows.Count > 0)
            {
                this.dbaccess.Update(e.SessionID, "ATR2", "ATR2");
            }


            if (wkch != null)
            {
                if (wkch.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(wkch);
                }
            }
            if (wkc1 != null)
            {
                if (wkc1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(wkc1);
                }
            }
            if (wkc2 != null)
            {
                if (wkc2.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(wkc2);
                }
            }

            DataRow newWkch = wkch.NewRow();

            newWkch["refnum"] = atr["refnum"];
            newWkch["sectorcode"] = atr["sectorcode"];
            newWkch["Day"] = atr["Day"];
            newWkch["Controller"] = atr["Controller"];
            newWkch["DutyOpsExec1"] = atr["DutyOpsExec1"];
            newWkch["DutyOpsExec2"] = atr["DutyOpsExec2"];
            newWkch["DutyOpsExec3"] = atr["DutyOpsExec3"];
            newWkch["DutyOpsExec4"] = atr["DutyOpsExec4"];
            newWkch["remark"] = atr["remark"];
            newWkch["created"] = atr["created"];
            newWkch["modified"] = atr["modified"];
            newWkch["year"] = atr["year"];
            newWkch["status"] = atr["status"];
            newWkch["period"] = atr["period"];
            newWkch["flag"] = "WKC" + atr["sectorcode"].ToString().Trim();
            newWkch["user"] = atr["user"];
            newWkch["guid"] = atr["guid"];
            newWkch["docunum"] = atr["docunum"];
            newWkch["wrrdate"] = atr["wrrdate"];
            newWkch["wrrday"] = atr["wrrday"];
            newWkch["trandate"] = atr["trandate"];
            newWkch["wrrFromDate"] = atr["wrrFromDate"];
            newWkch["wrrToDate"] = atr["wrrToDate"];
            newWkch["weekDateFrom"] = atr["wrrFromDate"];
            newWkch["weekDateTo"] = atr["wrrToDate"];
            newWkch["wrrnum"] = atr["wrrnum"];
            newWkch["sitenum"] = atr["sitenum"];
            newWkch["createdby"] = atr["createdby"];

            wkch.Rows.Add(newWkch);

            if (atr1.Rows.Count > 0)
            {
                foreach (DataRow a1 in atr1.Rows)
                {
                    if (a1.RowState != DataRowState.Deleted)
                    {
                        if (a1["paytypecode"].ToString().Trim() == "W")
                        {
                            DataRow newWkc1 = wkc1.NewRow();
                            newWkc1["refnum"] = a1["refnum"];
                            newWkc1["empnum"] = a1["empnum"];
                            newWkc1["timein"] = a1["timein"];
                            newWkc1["schedtimein"] = a1["schedtimein"];
                            newWkc1["scheddatein"] = a1["scheddatein"];
                            newWkc1["timeout"] = a1["timeout"];
                            newWkc1["schedtimeout"] = a1["schedtimeout"];
                            newWkc1["scheddateout"] = a1["scheddateout"];
                            newWkc1["remark"] = a1["remark"];
                            newWkc1["created"] = a1["created"];
                            newWkc1["modified"] = a1["modified"];
                            newWkc1["year"] = a1["year"];
                            newWkc1["status"] = a1["status"];
                            newWkc1["period"] = a1["period"];
                            newWkc1["flag"] = wkch.Rows[0]["flag"];
                            newWkc1["user"] = a1["user"];
                            newWkc1["guid"] = a1["guid"];
                            newWkc1["adhocremark"] = a1["adhocremark"];
                            newWkc1["lvmnum"] = a1["lvmnum"];
                            newWkc1["shiftcode"] = a1["shiftcode"];
                            newWkc1["replacedby"] = a1["replacedby"];
                            newWkc1["trandate"] = a1["trandate"];
                            newWkc1["nric"] = a1["nric"];
                            newWkc1["rnric"] = a1["rnric"];
                            newWkc1["rempname"] = a1["rempname"];
                            newWkc1["reprefkey"] = a1["reprefkey"];
                            newWkc1["day"] = a1["day"];
                            newWkc1["Date"] = a1["Date"];
                            newWkc1["dayofweek"] = a1["dayofweek"];
                            newWkc1["confirmedtimein"] = a1["confirmedtimein"];
                            newWkc1["confirmedtimeout"] = a1["confirmedtimeout"];
                            newWkc1["rempnum"] = a1["rempnum"];
                            newWkc1["atrnum"] = a1["atrnum"];
                            newWkc1["createdby"] = a1["createdby"];
                            newWkc1["sitenum"] = a1["sitenum"];
                            newWkc1["sectorcode"] = a1["sectorcode"];
                            newWkc1["finaltimein"] = a1["finaltimein"];
                            newWkc1["finaltimout"] = a1["finaltimout"];
                            newWkc1["empnum2"] = a1["empnum2"];
                            newWkc1["paytypecode"] = a1["paytypecode"];
                            newWkc1["isAdhoc"] = a1["isAdhoc"];
                            newWkc1["isOT"] = a1["isOT"];
                            newWkc1["OTrate"] = a1["OTrate"];
                            newWkc1["isRD"] = a1["isRD"];
                            newWkc1["isDre"] = a1["isDre"];
                            newWkc1["isUS"] = a1["isUS"];
                            newWkc1["uniquekey"] = a1["uniquekey"];
                            newWkc1["TotalHrs"] = a1["TotalHrs"];
                            newWkc1["empnum3"] = a1["empnum3"];
                            newWkc1["isTR"] = a1["isTR"];
                            newWkc1["TRsitenum"] = a1["TRsitenum"];
                            newWkc1["empnum4"] = a1["empnum4"];
                            newWkc1["Latemins"] = a1["Latemins"];
                            newWkc1["daysperweek"] = a1["daysperweek"];
                            newWkc1["isph"] = a1["isph"];
                            newWkc1["empname"] = a1["empname"];
                            newWkc1["otmins"] = a1["otmins"];
                            if (!BizFunctions.IsEmpty(a1["Date"]))
                            {
                                newWkc1["weekno"] = ATL.DateTimeExt.DateTimeExtensions.weekNumber(Convert.ToDateTime(a1["Date"]));
                            }
                            newWkc1["isBioMetrics"] = a1["isBioMetrics"];
                            newWkc1["value"] = a1["value"];
                            newWkc1["isoffset"] = a1["isoffset"];
                            newWkc1["offsetremark"] = a1["offsetremark"];
                            wkc1.Rows.Add(newWkc1);

                        }
                    }
                }
            }

            
           

            if (wkch.Rows.Count > 0)
            {

                DataRow wkcDr = e.DBAccess.DataSet.Tables["wkch"].Rows[0];

                if (wkc1.Rows.Count > 0)
                {
                    foreach (DataRow w1 in wkc1.Rows)
                    {
                        if (w1.RowState != DataRowState.Deleted)
                        {
                            w1["flag"] = wkcDr["flag"];
                        }
                    }
                }

             

               

                Hashtable tablesCollection = new Hashtable();
                foreach (DataTable dataTable in e.DBAccess.DataSet.Tables)
                {
                    tablesCollection[dataTable.TableName] = dataTable.TableName;
                }


                DataTable[] dataTables = new DataTable[2];
                dataTables[0] = e.DBAccess.DataSet.Tables["WKCH"];
                dataTables[0].TableName = tablesCollection[dataTables[0].TableName].ToString();

                dataTables[1] = e.DBAccess.DataSet.Tables["WKC1"];
                dataTables[1].TableName = tablesCollection[dataTables[1].TableName].ToString();

           


                foreach (DataTable dt in dataTables)
                {
                    int maxID = BizLogicTools.Tools.getID(e.DBAccess, dt.TableName);

                    if (maxID > 0)
                    {
                        maxID = maxID + 1;
                    }
                   
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            dr["id"] = maxID;
                            maxID++;
                        }
                    }                    
                }

                if (wkc1.Rows.Count > 0)
                {
                    e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM wkch WHERE refnum = '" + atr["refnum"].ToString().Trim() + "'");
                    e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM wkc1 WHERE refnum = '" + atr["refnum"].ToString().Trim() + "'");


                    e.DBAccess.Update(dataTables);


                    e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SysID SET LastID=(SELECT MAX(ID) FROM WKCH) WHERE TableName='WKCH'");
                    e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SysID SET LastID=(SELECT MAX(ID) FROM WKC1) WHERE TableName='WKC1'");

                }

               
            }


            if (atr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                string UpdateWRR = "UPDATE WRR set [status]='" + Common.DEFAULT_DOCUMENT_STATUSP + "' where refnum='"+atr["wrrnum"]+"'";

                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateWRR);
            }


   
           
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            HighlightLateRows();
            GetSummary2();

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

        #region Other Methods

        private void GetWrr()
        {
            DataRow atr = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            string sqlGetWRR = "Select * from wrr where refnum='" + atr["wrrnum"].ToString().Trim() + "'";
            this.dbaccess.ReadSQL("TBGetWRR", sqlGetWRR);
            DataTable TBGetWRR = this.dbaccess.DataSet.Tables["TBGetWRR"];

            if (TBGetWRR.Rows.Count > 0)
            {
                DataRow drGetWRR = this.dbaccess.DataSet.Tables["TBGetWRR"].Rows[0];
                atr["sectorcode"] = drGetWRR["sectorcode"]; 
                atr["sitenum"] = drGetWRR["sitenum"];
                atr["wrrFromDate"] = Convert.ToDateTime(drGetWRR["commencedate"]).ToShortDateString();
                atr["wrrToDate"] = Convert.ToDateTime(drGetWRR["enddate"]).ToShortDateString();
           
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

            return Timeout;
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

        private decimal GetPayTypeValue(string empnum)
        {
            decimal value = 0;

            string getPayType = "Select paytypecode from HEMPHtmp1 where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getPayType);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                string GetPayTypeValue = "Select value from PAYTM where paytypecode='" + dr1["paytypecode"].ToString() + "'";

                DataTable dt2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetPayTypeValue);

                if (dt2.Rows.Count > 0)
                {
                    DataRow dr2 = dt2.Rows[0];

                    value = Convert.ToDecimal(dr2["value"]);
                }

            }

            return value;
        }

        private void GetATMR()
        {
            DataRow atr = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            DataTable atr1 = this.dbaccess.DataSet.Tables["ATR1"];
            if (atr1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(atr1);
            }
            //string sqlGetATR = "Select * from atmrlive where refnum='" + atr["wrrnum"].ToString().Trim() + "' or (sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "' and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "') and uniquekey not in (Select uniquekey from ATR1 where [status]<>'V') order by empnum,[date]";

            string sqlGetATR = "SELECT * FROM "+
                                "( "+
                                "Select * from atmrlive "+
	                                "where "+
	                                "( "+
                                        "refnum='" + atr["wrrnum"].ToString().Trim() + "' and ISNULL(ClockInMark,0)=1 and  uniquekey not in (Select uniquekey from ATR1 where wrrnum='" + atr["wrrnum"].ToString().Trim() + "' and [status]<>'V') " +
	                                ") "+
	                                "or "+
	                                "( "+
                                        "sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' and ISNULL(ClockInMark,0)=1  " +
		                                "and "+
                                        "uniquekey not in (Select uniquekey from ATR1 where [status]<>'V' and sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' )) " +

                                 ")A "+
                                 
                                "UNION "+

                                "SELECT * FROM "+
                                 "( "+
                                 "Select * from atmrlive "+
	                                "where "+
	                                "( "+
                                        "refnum='" + atr["wrrnum"].ToString().Trim() + "' and shiftcode in (Select shiftcode from vshlv where ISNULL(isWorkShift,0)=0) " +
		                                "and "+
                                        "uniquekey not in (Select uniquekey from ATR1 where wrrnum='" + atr["wrrnum"].ToString().Trim() + "' and [status]<>'V')) " +
	                                "or "+
	                                "( "+
                                        "sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' " +
		                                "and "+
		                                "shiftcode in (Select shiftcode from vshlv where ISNULL(isWorkShift,0)=0)  "+
		                                "and "+
                                        "uniquekey not in (Select uniquekey from ATR1 where [status]<>'V' and sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' )) " +
                                 ")B "+
                                  "order by empnum,[date]";

            this.dbaccess.ReadSQL("TBGetATMR", sqlGetATR);
            DataTable TBGetATMR = this.dbaccess.DataSet.Tables["TBGetATMR"];
            if (TBGetATMR.Rows.Count > 0)
            {
                if (atr1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(atr1);
                }
                foreach (DataRow dr1 in TBGetATMR.Select())
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertAtr1 = atr1.NewRow();

                        InsertAtr1["uniquekey"] = dr1["uniquekey"];
                        InsertAtr1["nric"] = dr1["nric"];
                        InsertAtr1["empnum"] = dr1["empnum"];
                        InsertAtr1["empnum2"] = dr1["empnum"];
                        InsertAtr1["shiftcode"] = dr1["shiftcode"];
                        InsertAtr1["sectorcode"] = dr1["sectorcode"];

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
                        //if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        //{
                        //    InsertAtr1["scheddatein"] = System.DBNull.Value;
                        //}
                        //else
                        //{
                        //    InsertAtr1["scheddatein"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimein"]));

                        //}

                        if (BizFunctions.IsEmpty(dr1["ActualDateTimeIn"]))
                        {
                            InsertAtr1["scheddatein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["scheddatein"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["ActualDateTimeIn"]));

                        }

                        //scheddateout
                        //if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        //{
                        //    InsertAtr1["scheddateout"] = System.DBNull.Value;
                        //}
                        //else
                        //{
                        //    InsertAtr1["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimeout"]));

                        //}

                     




                        //schedtimein
                        //if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        //{
                        //    InsertAtr1["schedtimein"] = System.DBNull.Value;
                        //}
                        //else
                        //{
                        //    InsertAtr1["schedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                        //}

                        if (BizFunctions.IsEmpty(dr1["actualtimein2"]))
                        {
                            InsertAtr1["schedtimein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimein"] = dr1["actualtimein2"].ToString();
                        }

                        if (!BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["actualtimein2"]) || (dr1["timein"].ToString() != string.Empty && dr1["actualtimein2"] != string.Empty))
                        {
                           
                                LocalTime timein = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timein"].ToString());
                                //LocalTime actualtimeout = MAXVALUE.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"])));
                                LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["actualtimein2"].ToString());

                                if (timein.TickOfDay < actualtimeout.TickOfDay)
                                {
                                    //InsertAtr1["confirmedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                                    InsertAtr1["confirmedtimein"] = dr1["actualtimein2"].ToString();
                                }
                                else
                                {
                                    InsertAtr1["confirmedtimein"] = dr1["timein"].ToString();
                                }
                            
                        }

                        //schedtimeout
                        //if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        //{
                        //    InsertAtr1["schedtimeout"] = System.DBNull.Value;
                        //}
                        //else
                        //{
                        //    InsertAtr1["schedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"]));

                        //}

                        if (BizFunctions.IsEmpty(dr1["actualtimeout2"]))
                        {
                            InsertAtr1["schedtimeout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimeout"] = dr1["actualtimeout2"].ToString();

                        }


                        if (!BizFunctions.IsEmpty(dr1["timeout"]) && !BizFunctions.IsEmpty(dr1["actualtimeout2"]) || (dr1["timeout"].ToString() != string.Empty && dr1["actualtimeout2"].ToString() != string.Empty))
                        {
                           
                                LocalTime timeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timeout"].ToString());
                                //LocalTime actualtimeout = MAXVALUE.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"])));
                                LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["actualtimeout2"].ToString());


                                if (timeout.TickOfDay < actualtimeout.TickOfDay)
                                {
                                    InsertAtr1["confirmedtimeout"] = dr1["timeout"].ToString();
                                }
                                else
                                {
                                    //InsertAtr1["confirmedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout2"]));
                                    InsertAtr1["confirmedtimeout"] = dr1["actualtimeout2"].ToString();
                                }
                          
                        }
                        else if (!BizFunctions.IsEmpty(dr1["timeout"]) && !BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["actualtimein2"]) && BizFunctions.IsEmpty(dr1["actualtimeout2"]))
                        {
                            InsertAtr1["confirmedtimeout"] = dr1["timeout"];
                        }


                        if (BizFunctions.IsEmpty(dr1["ActualDateTimeOut"]))
                        {
                            if (!BizFunctions.IsEmpty(dr1["ActualDateTimeIn"]) || !BizFunctions.IsEmpty(dr1["actualtimein2"]))
                            {
                                if (!BizFunctions.IsEmpty(InsertAtr1["confirmedtimein"]) && !BizFunctions.IsEmpty(InsertAtr1["confirmedtimeout"]))
                                {

                                    LocalTime schedtimein = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(InsertAtr1["confirmedtimein"].ToString());
                                    LocalTime schedtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(InsertAtr1["confirmedtimeout"].ToString());

                                    if (!(schedtimein.TickOfDay < schedtimeout.TickOfDay))
                                    {
                                        InsertAtr1["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["ActualDateTimeIn"]).AddDays(+1));
                                    }
                                    else
                                    {
                                        InsertAtr1["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["ActualDateTimeIn"]));
                                    }
                                }
                                else
                                {
                                    InsertAtr1["scheddateout"] = System.DBNull.Value;
                                }
                            }
                             
                            else
                            {
                                InsertAtr1["scheddateout"] = System.DBNull.Value;
                            }

                        }
                        else
                        {
                            InsertAtr1["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["ActualDateTimeOut"]));

                        }


                        //

                        if (BizFunctions.IsEmpty(dr1["isadhoc"]))
                        {
                            InsertAtr1["isadhoc"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["isOT"]))
                        {
                            InsertAtr1["isOT"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["OTrate"]))
                        {
                            InsertAtr1["OTrate"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["isRD"]))
                        {
                            InsertAtr1["isRD"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["isDRE"]))
                        {
                            InsertAtr1["isDRE"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["isUS"]))
                        {
                            InsertAtr1["isUS"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["isTR"]))
                        {
                            InsertAtr1["isTR"] = 0;
                        }


                        InsertAtr1["isadhoc"] = dr1["isadhoc"];
                        InsertAtr1["isOT"] = dr1["isOT"];
                        InsertAtr1["isRD"] = dr1["isRD"];
                        InsertAtr1["isDRE"] = dr1["isDRE"];
                        InsertAtr1["isUS"] = dr1["isUS"];
                        InsertAtr1["isTR"] = dr1["isTR"];
                        InsertAtr1["TRsitenum"] = dr1["TRsitenum"];

                        InsertAtr1["rempnum"] = dr1["rempnum"];
                        InsertAtr1["rempname"] = dr1["rempname"];
                        InsertAtr1["rnric"] = dr1["rnric"];
                        InsertAtr1["day"] = dr1["day"];
                        InsertAtr1["date"] = dr1["date"];
                        InsertAtr1["dayofweek"] = TimeTools.GetDayOfWeekNo(dr1["day"].ToString().Trim());
                        InsertAtr1["RepRefKey"] = dr1["RefKey"];
                        InsertAtr1["empnum3"] = dr1["empnum"];
                        InsertAtr1["empnum4"] = dr1["empnum"];

                        InsertAtr1["paytypecode"] = GetPayTypeCode(dr1["empnum"].ToString());

                        //if(GetPayTypeCode(dr1["empnum"].ToString()) != string.Empty)
                        //{
                        //     InsertAtr1["value"] = GetPayTypeValue(GetPayTypeCode(dr1["empnum"].ToString()));
                        //}

                       
                        atr1.Rows.Add(InsertAtr1);
                    }
                }
            }
            else
            {
                MessageBox.Show("It's either you have Extracted all Schedules for this Refnum or NO Clock In/Out data is available yet", "No Records Found!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private string GetLeaveRefnum(string empnum, DateTime dt)
        {
            string LeaveRefnum = "";

            //string Get = "Select refnum from LVRtmp where empnum='" + empnum + "' and leavefrom>='" + BizFunctions.GetSafeDateString(dt) + "' and leaveto<='" + BizFunctions.GetSafeDateString(dt) + "'  ";
            string Get = "Select refnum from LVRtmp where empnum='" + empnum + "' and leavefrom>='" + dt.ToString() + "' and leaveto<='" + dt.ToString() + "'  ";
            //this.dbaccess.ReadSQL("GetLeaveTb", Get);

            //DataTable dt1 = this.dbaccess.DataSet.Tables["GetLeaveTb"];

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                LeaveRefnum = dt1.Rows[0]["refnum"].ToString();
            }

            return LeaveRefnum;
        }

        private string GetLeaveSitenum(string refnum)
        {
            string LeaveSitenum = "";

            string Get = "Select sitenum from LVRtmp where refnum='"+refnum+"'   ";

            //this.dbaccess.ReadSQL("GetLeaveTb2", Get);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                LeaveSitenum = dt1.Rows[0]["sitenum"].ToString();
            }

            return LeaveSitenum;
        }

        #endregion
    }
}
    

