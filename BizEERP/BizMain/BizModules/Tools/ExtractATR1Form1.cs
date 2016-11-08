//using System;
//using System.Data;
//using System.Collections;
//using System.Windows.Forms;
//using System.Configuration;
//using System.IO;
//using System.Drawing;
//using System.ComponentModel;

//using BizRAD.BizReport;
//using BizRAD.BizXml;
//using BizRAD.BizDocument;
//using BizRAD.DB.Client;
//using BizRAD.DB.Interface;
//using BizRAD.BizApplication;
//using BizRAD.BizControls.OutLookBar;
//using BizRAD.BizControls.BizDateTimePicker;
//using BizRAD.BizControls.DataGridColumns;
//using BizRAD.BizCommon;
//using BizRAD.BizAccounts;
//using BizRAD.BizVoucher;

//using CrystalDecisions.Windows.Forms;
//using CrystalDecisions.Shared;
//using CrystalDecisions.CrystalReports.Engine;

//using System.Text.RegularExpressions;
//using ATL.SortTable;
//using ATL.TimeUtilites;
//using ATL.BizModules.TextValidator;
//using NodaTime;

//using ATL.BizModules.Tools;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;

using ATL.BizModules.TimeSync;
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
using ATL.TimeUtilites;
using ATL.BizModules.TextValidator;
using NodaTime;
using ATL.BizModules.Tools;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;
using ATL.SortTable;



namespace ATL.ExtractATR1Form1
{
    public partial class ExtractATR1Form1 : Form
    {
        protected DBAccess dbaccess = null;
        protected string Empnum,TableName = "";
        protected DataView EmpDV1 = null;
        

        public ExtractATR1Form1(DBAccess da, string empNo,string tableName)
        {
            InitializeComponent();

            this.dbaccess = da;
            this.Empnum = empNo;
            this.TableName = tableName;

           
        }


        private void table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            switch (e.Column.ColumnName)
            {
               

                case "CONFIRMEDTIMEIN":
                    if (!BizFunctions.IsEmpty(e.Row["CONFIRMEDTIMEIN"]))
                    {
                        if (!TextValidator.IsvalidMilitaryTime(e.Row["CONFIRMEDTIMEIN"].ToString().Trim()))
                        {
                      
                            MessageBox.Show("Invalid Time In Format");
                            e.Row["CONFIRMEDTIMEIN"] = string.Empty;
                        }
                    }
                    break;

                case "CONFIRMEDTIMEOUT":
                    if (!BizFunctions.IsEmpty(e.Row["CONFIRMEDTIMEOUT"]))
                    {
                        if (TextValidator.IsvalidMilitaryTime(e.Row["CONFIRMEDTIMEOUT"].ToString().Trim()))
                        {                      
                            MessageBox.Show("Invalid Time Out Format");
                            e.Row["CONFIRMEDTIMEOUT"] = string.Empty;
                        }
                    }
                    break;

                //case "DTcolumn":
                //    if(!BizFunctions.IsEmpty("DTcolumn"))
                //    {
                //        e.Row["Date"] = e.Row["DTcolumn"];
                //    }
                //    break;
            }


            EmpDGV1.Refresh();
        }

      

        private void EmpDGV1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == this.EmpDGV1.Columns[e.ColumnIndex].Index)
            {
                if (e.ColumnIndex == 63) // DTcolumn
                {
                    EmpDGV1.Rows[e.RowIndex].Cells["Date"].Value = EmpDGV1.Rows[e.RowIndex].Cells["DTcolumn"].Value;
                    EmpDGV1.Rows[e.RowIndex].Cells["Empnum"].Value = Empnum;
                }


                if (e.ColumnIndex == 29) // DTcolumn
                {
                    if (!BizFunctions.IsEmpty(EmpDGV1.Rows[e.RowIndex].Cells["DTcolumn"].Value))
                    {
                        EmpDGV1.Rows[e.RowIndex].Cells["DTcolumn"].Value = EmpDGV1.Rows[e.RowIndex].Cells["Date"].Value;
                    }
                }
            }

             //case "sitenum":
                //    {
                //        //e.Row["empnum"] = Empnum;
                //    }
                //    break;
                //case "shiftcode":
                //    {
                //        //e.Row["empnum"] = Empnum;
                //    }
                //    break;
                //case "Date":
                //    {
                //        //e.Row["empnum"] = Empnum;
                //        e.Row["SCHEDDATEIN"] = e.Row["Date"];
                //    }
                //    break;
        }


        private void ExtractATR1Form1_Load(object sender, EventArgs e)
        {
            DataTable table = this.dbaccess.DataSet.Tables[TableName];

            

            DataView dvData = new DataView(table);

            dvData.RowFilter = "empnum = '" + Empnum + "'";

             
            BindingSource nbindingSource = new BindingSource();
            nbindingSource.DataSource = dvData;
            EmpDGV1.DataSource = nbindingSource;


            CalendarColumn cc = new CalendarColumn();
            cc.Name = "DTcolumn";
            cc.HeaderText = "Date";
           
            this.EmpDGV1.Columns.Add(cc);
                     
            EmpDGV1.Columns["EMPNUM"].DisplayIndex = 0;
	        EmpDGV1.Columns["sitenum"].DisplayIndex = 1;
	        EmpDGV1.Columns["DAY"].DisplayIndex = 2;
            EmpDGV1.Columns["DTcolumn"].DisplayIndex = 3;
            EmpDGV1.Columns["isomit"].DisplayIndex = 4;
            EmpDGV1.Columns["attnmark"].DisplayIndex = 5;          
	        EmpDGV1.Columns["shiftcode"].DisplayIndex = 6;
            EmpDGV1.Columns["DayAmt"].DisplayIndex = 7;
            EmpDGV1.Columns["lvrnum"].DisplayIndex = 8;
            //lvrnum

	        EmpDGV1.Columns["actualshiftcode"].DisplayIndex = 9;
	        EmpDGV1.Columns["scheddatein"].DisplayIndex = 10;
	        EmpDGV1.Columns["timein"].DisplayIndex = 11;
	        EmpDGV1.Columns["confirmedtimein"].DisplayIndex = 12;
	        EmpDGV1.Columns["scheddateout"].DisplayIndex = 13;
	        EmpDGV1.Columns["timeout"].DisplayIndex = 14;
	        EmpDGV1.Columns["confirmedtimeout"].DisplayIndex = 15;
	        EmpDGV1.Columns["TotalHrs"].DisplayIndex = 16;
	        EmpDGV1.Columns["ActualTotalHrs"].DisplayIndex = 17;


            EmpDGV1.Columns["OTHrs"].DisplayIndex = 18;
            EmpDGV1.Columns["ActualOTHrs"].DisplayIndex = 19;

	                 

       
            EmpDGV1.Columns["LateMins"].DisplayIndex = 20;
            EmpDGV1.Columns["actualLateMins"].DisplayIndex = 21;
            EmpDGV1.Columns["FixAllowAmt"].DisplayIndex = 22;
            EmpDGV1.Columns["actualFixAllow"].DisplayIndex = 23;

            EmpDGV1.Columns["AttnRemarks"].DisplayIndex = 24;
            EmpDGV1.Columns["actualAttnRemark"].DisplayIndex = 25;
            EmpDGV1.Columns["remark"].DisplayIndex = 26;
            EmpDGV1.Columns["OT1"].DisplayIndex = 27;
            EmpDGV1.Columns["actualOT1"].DisplayIndex = 28;
            EmpDGV1.Columns["OT15"].DisplayIndex = 30;
            EmpDGV1.Columns["actualOT15"].DisplayIndex = 31;
            EmpDGV1.Columns["OT2"].DisplayIndex = 32;
            EmpDGV1.Columns["actualOT2"].DisplayIndex = 33;




       

          



            //EmpDGV1.Columns["DATE"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["attnmark"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["AttnRemarks"].DefaultCellStyle.BackColor = Color.LightBlue;
            //EmpDGV1.Columns["DTcolumn"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["FixAllowAmt"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["EMPNUM"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["SITENUM"].DefaultCellStyle.BackColor = Color.Yellow;
            EmpDGV1.Columns["SCHEDDATEIN"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["OTHrs"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["TIMEIN"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["SCHEDDATEOUT"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["TIMEOUT"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["DAY"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["SHIFTCODE"].DefaultCellStyle.BackColor = Color.Yellow;
            EmpDGV1.Columns["LVRNUM"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["SECTORCODE"].DefaultCellStyle.BackColor = Color.Yellow;
            EmpDGV1.Columns["REMPNUM"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["EMPNAME"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["ISADHOC"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["ISOT"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["ISUS"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["ISRD"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["ISTR"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["ISDRE"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["ISOFFSET"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["ISPH"].DefaultCellStyle.BackColor = Color.LightGray;
            EmpDGV1.Columns["DAYSPERWEEK"].DefaultCellStyle.BackColor = Color.LightGreen;
            EmpDGV1.Columns["TOTALHRS"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["LATEMINS"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["OTMINS"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["ISFIXALLOW"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["OT1"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["OT15"].DefaultCellStyle.BackColor = Color.LightBlue;
            EmpDGV1.Columns["OT2"].DefaultCellStyle.BackColor = Color.LightBlue;

            //EmpDGV1.Columns["SHIFTCODE"].ReadOnly = true;
            EmpDGV1.Columns["attnmark"].ReadOnly = true;
            //EmpDGV1.Columns["DATE"].ReadOnly = true;
            EmpDGV1.Columns["SCHEDDATEIN"].ReadOnly = true;
            EmpDGV1.Columns["TIMEIN"].ReadOnly = true;
            EmpDGV1.Columns["SCHEDDATEOUT"].ReadOnly = true;
            EmpDGV1.Columns["TIMEOUT"].ReadOnly = true;
            //EmpDGV1.Columns["DTcolumn"].ReadOnly = true;              
            EmpDGV1.Columns["TOTALHRS"].ReadOnly = true;
            EmpDGV1.Columns["LATEMINS"].ReadOnly = true;
            EmpDGV1.Columns["OTMINS"].ReadOnly = true;
            EmpDGV1.Columns["ISFIXALLOW"].ReadOnly = true;
            EmpDGV1.Columns["OT1"].ReadOnly = true;
            EmpDGV1.Columns["OT15"].ReadOnly = true;
            EmpDGV1.Columns["OT2"].ReadOnly = true;
            EmpDGV1.Columns["EMPNUM"].ReadOnly = true;
            EmpDGV1.Columns["EMPNAME"].ReadOnly = true;
            EmpDGV1.Columns["DAY"].ReadOnly = true;
            EmpDGV1.Columns["TIMEIN"].ReadOnly = true;
            EmpDGV1.Columns["TIMEOUT"].ReadOnly = true;;
            EmpDGV1.Columns["OTHrs"].ReadOnly = true;
            EmpDGV1.Columns["FixAllowAmt"].ReadOnly = true;
            EmpDGV1.Columns["AttnRemarks"].ReadOnly = true;

            ////Invisible Colums

            EmpDGV1.Columns["otmins"].Visible = false;
            EmpDGV1.Columns["isfixallow"].Visible = false;
            EmpDGV1.Columns["actualworkhrs"].Visible = false;
            EmpDGV1.Columns["date"].Visible = false;


            EmpDGV1.Columns["actualshiftcode"].Visible = false;
            EmpDGV1.Columns["MARK"].Visible = false;            
            EmpDGV1.Columns["EMPNUM"].Visible = false;
            EmpDGV1.Columns["EMPNAME"].Visible = false;
            EmpDGV1.Columns["ID"].Visible = false;
            EmpDGV1.Columns["LVMNUM"].Visible = false;
            EmpDGV1.Columns["ISPH"].Visible = false;
            EmpDGV1.Columns["GUID"].Visible = false;
            EmpDGV1.Columns["uniquekey"].Visible = false;

            EmpDGV1.Columns["REFNUM"].Visible = false;
            EmpDGV1.Columns["CREATED"].Visible = false;
            EmpDGV1.Columns["MODIFIED"].Visible = false;

            EmpDGV1.Columns["YEAR"].Visible = false;
            EmpDGV1.Columns["STATUS"].Visible = false;
            EmpDGV1.Columns["PERIOD"].Visible = false;
         
            EmpDGV1.Columns["WRRNUM"].Visible = false;


            EmpDGV1.Columns["FLAG"].Visible = false;
            EmpDGV1.Columns["USER"].Visible = false;
            EmpDGV1.Columns["TRANDATE"].Visible = false;

            EmpDGV1.Columns["NRIC"].Visible = false;
            EmpDGV1.Columns["DAYOFWEEK"].Visible = false;
            EmpDGV1.Columns["ATRNUM"].Visible = false;

            EmpDGV1.Columns["CREATEDBY"].Visible = false;
            EmpDGV1.Columns["EMPNUM2"].Visible = false;
            EmpDGV1.Columns["EMPNUM3"].Visible = false;

            EmpDGV1.Columns["EMPNUM4"].Visible = false;
            EmpDGV1.Columns["PAYTYPECODE"].Visible = false;
            EmpDGV1.Columns["uniquekey"].Visible = false;



            ///////


            EmpDGV1.Columns["REPLACEDBY"].Visible = false;
            EmpDGV1.Columns["RNRIC"].Visible = false;
            EmpDGV1.Columns["REMPNAME"].Visible = false;

            EmpDGV1.Columns["REPREFKEY"].Visible = false;
            EmpDGV1.Columns["FINALTIMEIN"].Visible = false;
            EmpDGV1.Columns["FINALTIMOUT"].Visible = false;

            ////////

            EmpDGV1.Columns["OTRATE"].Visible = false;
            EmpDGV1.Columns["ISDRE"].Visible = false;
            EmpDGV1.Columns["ISOFFSET"].DisplayIndex = 17;
            EmpDGV1.Columns["ISUS"].Visible = false;
            EmpDGV1.Columns["ISRD"].Visible = false;
            EmpDGV1.Columns["ISADHOC"].Visible = false;
            EmpDGV1.Columns["ADHOCREMARK"].Visible = false;
            EmpDGV1.Columns["DAYSPERWEEK"].Visible = false;
            EmpDGV1.Columns["ISTR"].Visible = false;
            EmpDGV1.Columns["TRSITENUM"].Visible = false;


            EmpDGV1.Columns["ISPH"].HeaderText = "PH";
            EmpDGV1.Columns["ISOMIT"].HeaderText = "OMIT";
            EmpDGV1.Columns["ISOT"].Visible = false;
            EmpDGV1.Columns["ISDRE"].Visible = false;
            EmpDGV1.Columns["ISOFFSET"].Visible = false;
            EmpDGV1.Columns["ISUS"].Visible = false;
            EmpDGV1.Columns["ISRD"].Visible = false;
            EmpDGV1.Columns["ISADHOC"].Visible = false;
            EmpDGV1.Columns["isBioMetrics"].Visible = false;
            EmpDGV1.Columns["docunum"].Visible = false;
            EmpDGV1.Columns["lvename"].Visible = false;
            EmpDGV1.Columns["OFFSETREMARK"].Visible = false;


            EmpDGV1.Columns["SCHEDTIMEOUT"].Visible = false;
            EmpDGV1.Columns["SCHEDTIMEIN"].Visible = false;


            EmpDGV1.Columns["SECTORCODE"].Visible = false;
            EmpDGV1.Columns["REMPNUM"].Visible = false;
            EmpDGV1.Columns["PGUID"].Visible = false;
           
            EmpDGV1.Columns["VALUE"].Visible = false;


            EmpDGV1.Columns["EMPNUM"].Frozen = true;
            EmpDGV1.Columns["SITENUM"].Frozen = true;
            EmpDGV1.Columns["DAY"].Frozen = true;
            EmpDGV1.Columns["DTcolumn"].Frozen = true;



     






            DataGridViewColumn OT = EmpDGV1.Columns["ISOT"];
            OT.Width = 40;


            DataGridViewColumn attnmark = EmpDGV1.Columns["attnmark"];
            attnmark.Width = 40;

            DataGridViewColumn isOmit = EmpDGV1.Columns["isOmit"];
            isOmit.Width = 40;


            DataGridViewColumn PH = EmpDGV1.Columns["ISPH"];
            PH.Width = 40;

            DataGridViewColumn DRE = EmpDGV1.Columns["ISDRE"];
            DRE.Width = 40;

            DataGridViewColumn US = EmpDGV1.Columns["ISUS"];
            US.Width = 40;

            DataGridViewColumn RD = EmpDGV1.Columns["ISRD"];
            RD.Width = 40;

            DataGridViewColumn AD = EmpDGV1.Columns["ISADHOC"];
            AD.Width = 40;

            DataGridViewColumn DPW = EmpDGV1.Columns["DAYSPERWEEK"];
            DPW.Width = 60;


            DataGridViewColumn isTR = EmpDGV1.Columns["ISTR"];
            isTR.Width = 40;

            DataGridViewColumn SiteTR = EmpDGV1.Columns["TRSITENUM"];
            SiteTR.Width = 80;

            DataGridViewColumn TOTALHRS = EmpDGV1.Columns["TOTALHRS"];
            TOTALHRS.Width = 60;


            DataGridViewColumn LATEMINS = EmpDGV1.Columns["LATEMINS"];
            LATEMINS.Width = 60;

            DataGridViewColumn OTRATE = EmpDGV1.Columns["OTRATE"];
            OTRATE.Width = 50;


            ///

            //EmpDGV1.Columns["OT1"].DisplayIndex = 16;
            //EmpDGV1.Columns["actualOT1"].DisplayIndex = 17;
            //EmpDGV1.Columns["OT15"].DisplayIndex = 18;
            //EmpDGV1.Columns["actualOT15"].DisplayIndex = 19;
            //EmpDGV1.Columns["OT2"].DisplayIndex = 20;
            //EmpDGV1.Columns["actualOT2"].DisplayIndex = 21;


            DataGridViewColumn ActualOTHrs = EmpDGV1.Columns["ActualOTHrs"];
            ActualOTHrs.Width = 80;

            DataGridViewColumn ActualLateMins = EmpDGV1.Columns["ActualLateMins"];
            ActualLateMins.Width = 80;


            DataGridViewColumn FixAllowamt = EmpDGV1.Columns["FixAllowamt"];
            FixAllowamt.Width = 70;


            DataGridViewColumn ActualFixAllow = EmpDGV1.Columns["ActualFixAllow"];
            ActualFixAllow.Width = 70;



            DataGridViewColumn OTHrs = EmpDGV1.Columns["OTHrs"];
            OTHrs.Width = 50;


            DataGridViewColumn ActualTotalHrs = EmpDGV1.Columns["ActualTotalHrs"];
            ActualTotalHrs.Width = 80;


            DataGridViewColumn OT1 = EmpDGV1.Columns["OT1"];
            OT1.Width = 50;

            DataGridViewColumn actualOT1 = EmpDGV1.Columns["actualOT1"];
            actualOT1.Width = 70;

            DataGridViewColumn OT15 = EmpDGV1.Columns["OT15"];
            OT15.Width = 50;

            DataGridViewColumn actualOT15 = EmpDGV1.Columns["actualOT15"];
            actualOT15.Width = 70;


            DataGridViewColumn OT2 = EmpDGV1.Columns["OT2"];
            OT2.Width = 50;

            DataGridViewColumn actualOT2 = EmpDGV1.Columns["actualOT2"];
            actualOT2.Width = 70;





            EmpDGV1.CellValueChanged += new DataGridViewCellEventHandler(EmpDGV1_CellValueChanged);

            table.ColumnChanged += new DataColumnChangeEventHandler(table_ColumnChanged);


            for (int i = 0; i < EmpDGV1.Rows.Count - 1; i++)
            {
                EmpDGV1.Rows[i].Cells["DTcolumn"].Value = EmpDGV1.Rows[i].Cells["Date"].Value;
            }

            currentBasicTb.Text = GetBasic(Empnum).ToString();



            EmpDGV1.CellDoubleClick += new DataGridViewCellEventHandler(EmpDGV1_CellDoubleClick);
         
        }

        void EmpDGV1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == this.EmpDGV1.Columns[e.ColumnIndex].Index)
            {
                if (e.ColumnIndex == 61)
                {
                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum like '" + Convert.ToString(EmpDGV1.CurrentRow.Cells["sitenum"].Value) + "%' ", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {

                        EmpDGV1.Rows[e.RowIndex].Cells["sitenum"].Value = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
                        EmpDGV1.Rows[e.RowIndex].Cells["empnum"].Value = Empnum;
                        EmpDGV1.Refresh();

                    }

                }
                if (e.ColumnIndex == 20)
                {


                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_vSHLV.xml", e, "shiftcode", "shiftcode like '" + Convert.ToString(EmpDGV1.CurrentRow.Cells["shiftcode"].Value) + "%' ", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {

                        EmpDGV1.Rows[e.RowIndex].Cells["shiftcode"].Value = f2BaseHelper.F2Base.CurrentRow["shiftcode"].ToString();
                        //EmpDGV1.Rows[e.RowIndex].Cells["timein"].Value = f2BaseHelper.F2Base.CurrentRow["timein"].ToString();
                        //EmpDGV1.Rows[e.RowIndex].Cells["timeout"].Value = f2BaseHelper.F2Base.CurrentRow["timeout"].ToString();
                        EmpDGV1.Rows[e.RowIndex].Cells["empnum"].Value = Empnum;
                        EmpDGV1.Refresh();

                    }
                }

               

            }
        }



        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
            return;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void calculatebtn_Click(object sender, EventArgs e)
        {

        }

        private decimal GetBasic(string empnum)
        {
            decimal basic = 0;

            string getBasic = "select ISNULL(rateamt,0) as rateamt from PFMSR where empnum='"+empnum+"' and hsamcode='BASIC'";

            this.dbaccess.ReadSQL("TmpBasicSal", getBasic);

            DataTable TmpBasicSal = this.dbaccess.DataSet.Tables["TmpBasicSal"];

            if (TmpBasicSal != null)
            {
                if (TmpBasicSal.Rows.Count > 0)
                {
                    basic = Convert.ToDecimal(TmpBasicSal.Rows[0]["rateamt"]);
                }
            }

            return basic;
        }



        private void InsertLine_Click(object sender, EventArgs e)
        {
            ATL.BizModules.Tools.InsertATRForm1 eATR1Insert = new ATL.BizModules.Tools.InsertATRForm1(this.dbaccess, Empnum, "ATR1");

            eATR1Insert.Show();
            eATR1Insert.Focus();

            EmpDGV1.Refresh();

            //for (int i = 0; i < EmpDGV1.Rows.Count; i++)
            //{
            //    if (BizFunctions.IsEmpty(EmpDGV1.Rows[i].Cells["DTColumn"]))
            //    {
            //        if (!BizFunctions.IsEmpty(EmpDGV1.Rows[i].Cells["Date"]))
            //        {
            //            EmpDGV1.Rows[i].Cells["DTColumn"].Value = EmpDGV1.Rows[i].Cells["Date"].Value;
            //        }
            //    }
            //}

        }

    }
}