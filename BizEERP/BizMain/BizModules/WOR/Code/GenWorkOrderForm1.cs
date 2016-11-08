using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.ComponentModel;

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
using ATL.SortTable;
using ATL.TimeUtilites;
using ATL.BizModules.TextValidator;
using NodaTime;
using System.Diagnostics;
using System.Collections.Generic;

using ATL.BizModules.Tools;

using ATL.Network;
using System.Diagnostics;
using System.Net.Mail;
//using Outlook = Microsoft.Office.Interop.Outlook;

namespace ATL.BizModules.WOR
{
    public partial class GenWorkOrderForm1_02042015 : Form
    {
        protected DBAccess dbaccess = null;
        protected bool changedPersonel = false;
        protected DataTable OriTable;
        protected DataTable SiteListTb;
        protected DataTable DifferenceTable;
        protected string[] arrDifference;
        protected string ReAssignedInfo = "";

        public GenWorkOrderForm1_02042015()
        {
            InitializeComponent();

            this.dbaccess = new DBAccess();
            DifferenceTable = new DataTable("DifferenceTable");
            DifferenceTable.Columns.Add("orinric", typeof(string));
            DifferenceTable.Columns.Add("newnric", typeof(string));
            DifferenceTable.Columns.Add("orisite", typeof(string));
            DifferenceTable.Columns.Add("newsite", typeof(string));

        }
        private void selectAllcb_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < SiteDGV1.Rows.Count - 1; i++)
            {
                SiteDGV1.Rows[i].Cells[17].Value = true;
            }
        }


        private void ALLcb_CheckedChanged(object sender, EventArgs e)
        {
            if (ALLcb.Checked == true)
            {
                SVCcb.Checked = true;
                PWORcb.Checked = true;
            }
        }

        private void SVCcb_CheckedChanged(object sender, EventArgs e)
        {
            if (SVCcb.Checked == true && ALLcb.Checked == false)
            {
                PWORcb.Checked = false;
            }
        }

        private void PWORcb_CheckedChanged(object sender, EventArgs e)
        {
            if (PWORcb.Checked == true && ALLcb.Checked == false)
            {
                SVCcb.Checked = false;
            }
        }

        private void ConfirmBtn_Click(object sender, EventArgs e)
        {
            

            if (PWORcb.Checked == false && SVCcb.Checked == true)
            {
                GenerateWOR();
            }
            else if (PWORcb.Checked == true && SVCcb.Checked == false)
            {
                GeneratePWOR();
            }

        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            if (PWORcb.Checked == false && SVCcb.Checked == true)
            {
                GetWORSiteLists();
            }
            else if (PWORcb.Checked == true && SVCcb.Checked == false)
            {
                GetPWORSiteLists();
            }
         
        }

        private void GenerateBtn_Click(object sender, EventArgs e)
        {
            if (PWORcb.Checked == false && SVCcb.Checked == true)
            {
                GetWORSiteLists();
            }
            else if (PWORcb.Checked == true && SVCcb.Checked == false)
            {
                GetPWORSiteLists();
            }
        }

        void yearComboBox1_DropDown(object sender, EventArgs e)
        {
            string sql1 = "select SystemYear from SysYear";
            this.dbaccess.ReadSQL("SysYear", sql1);
            DataRow drInsertSysYear = this.dbaccess.DataSet.Tables["SysYear"].NewRow();



            this.dbaccess.DataSet.Tables["SysYear"].Rows.Add(drInsertSysYear);

            yearComboBox1.DataSource = this.dbaccess.DataSet.Tables["SysYear"];
            yearComboBox1.DisplayMember = this.dbaccess.DataSet.Tables["SysYear"].Columns["SystemYear"].ColumnName.ToString();
            yearComboBox1.ValueMember = this.dbaccess.DataSet.Tables["SysYear"].Columns["SystemYear"].ColumnName.ToString();
        }

        private void initialiseData()
        {
            
        }

        private void GetSiteLists()
        {
            //Jason : 31/03/2015 - Generate PWOR directly from PCTR
            //string Get = "  Select SM.sitenum,SM.sitename,ST.sitenumt,'' as refnum,'' as [date],'' as nric, '' as empname from sitm SM " +
            //                "LEFT JOIN SITMT ST  "+
            //                "ON SM.SITENUM=ST.SITENUM  "+
            //                "where  "+
            //                "SM.[status]<>'V'  "+
            //                "and  "+
            //                "ST.[status]<>'V'";

            string Get = "  Select "+
	                            "a.docunum, "+
	                            "a.sitenum, "+
	                            "a.sitename, "+
	                            "a.refnum, "+
	                            "a.[date], "+
	                            "a.empnum, "+
	                            "h.empname, "+
	                            "a.actualempnum,  "+
	                            "a.actualempname "+
                            "from "+
                            "( "+
                            "Select ch.refnum as docunum, c18.sitenum, ch.sitename, '' as refnum,'' as [date],c18.empnum,'' as actualempnum, '' as actualempname from ctr18 c18 left join ctrh ch on c18.refnum=ch.refnum "+
                            "where ch.[status]<>'V' and ch.commencedate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and ch.enddate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and c18.sitenum is not null " +
                            "group by ch.refnum,c18.sitenum,ch.sitename,c18.empnum "+
                            ")a "+
                            "left join hemph h on a.empnum=h.empnum";

            this.dbaccess.ReadSQL("SiteListTb", Get);



            DataTable SiteListTb = this.dbaccess.DataSet.Tables["SiteListTb"];

        

            BindingSource bs = new BindingSource();

            bs.DataSource = SiteListTb;

            this.SiteDGV1.DataSource = bs.DataSource;

            if (SiteDGV1.Columns.Contains("Mark"))
            {
                SiteDGV1.Columns["mark"].Dispose();
                DataGridViewCheckBoxColumn mark = new DataGridViewCheckBoxColumn(false);
                mark.Name = "Mark";
                SiteDGV1.Columns.Add(mark);
            }
            else
            {

                DataGridViewCheckBoxColumn mark = new DataGridViewCheckBoxColumn(false);
                mark.Name = "Mark";
                SiteDGV1.Columns.Add(mark);
            }
           

            DataGridViewColumn Mark = SiteDGV1.Columns["Mark"];
            Mark.Width = 60;

            DataGridViewColumn Sitenum = SiteDGV1.Columns["Sitenum"];
            Sitenum.Width = 120;

            SiteDGV1.Columns["mark"].DisplayIndex = 0;
            SiteDGV1.Columns["sitenum"].DisplayIndex = 1;
            SiteDGV1.Columns["sitename"].DisplayIndex = 2;
            SiteDGV1.Columns["docunum"].DisplayIndex = 3;
            SiteDGV1.Columns["refnum"].DisplayIndex = 4;
            SiteDGV1.Columns["Date"].DisplayIndex = 5;
            SiteDGV1.Columns["empnum"].DisplayIndex = 6;
            SiteDGV1.Columns["empname"].DisplayIndex = 7;
            SiteDGV1.Columns["actualempnum"].DisplayIndex = 8;
            SiteDGV1.Columns["actualempname"].DisplayIndex = 9;

            SiteDGV1.Columns["sitenum"].ReadOnly = true;
            SiteDGV1.Columns["sitenum"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["Sitename"].Visible = false;
            SiteDGV1.Columns["docunum"].Visible = false;
            SiteDGV1.Columns["refnum"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["Date"].DefaultCellStyle.BackColor = Color.LightBlue;

            SiteDGV1.Columns["empname"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["actualempname"].DefaultCellStyle.BackColor = Color.LightBlue;

        }

        private void GetPWORSiteLists()
        {


            DateTime dtX = TimeTools.GetFirstDayOfWeek(Convert.ToInt32(yearComboBox1.Text), Convert.ToInt32(weekComboBox1.Text)).AddDays(-1);
            DateTime dtY = dtX.AddDays(6);

            dateTimePicker1.Text = dtX.ToShortDateString();
            dateTimePicker2.Text = dtY.ToShortDateString();


            //string Get = "  Select SM.sitenum,SM.sitename,ST.sitenumt,'' as refnum,'' as [date],'' as nric, '' as empname from sitm SM " +
            //                "LEFT JOIN SITMT ST  " +
            //                "ON SM.SITENUM=ST.SITENUM  " +
            //                "where  " +
            //                "SM.[status]<>'V'  " +
            //                "and  " +
            //                "ST.[status]<>'V' and ST.flag='PSITM'";

            //Jason:01042015 - Get directly from PCTR
            //string Get = "   SELECT "+
            //                    "A.sitenum, "+
            //                    "A.sitename, "+
            //                    "A.sitenumt, "+
            //                    "CASE WHEN B.refnum IS NOT NULL OR B.refnum<>'' THEN B.refnum ELSE A.refnum END AS REFNUM, "+
            //                    "CASE WHEN B.[date] IS NOT NULL OR B.[date]<>'' THEN B.[date] ELSE A.[date] END AS [date], "+
            //                    "CASE WHEN B.nric IS NOT NULL OR B.nric<>'' THEN B.nric ELSE A.nric END AS NRIC, "+
            //                    "CASE WHEN B.empname IS NOT NULL OR B.empname<>'' THEN B.empname ELSE A.empname END AS empname "+
            //                    "FROM "+
            //                  " ( "+
            //                    "Select  "+
            //                        "SM.sitenum, "+
            //                        "SM.sitename, "+
            //                        "ST.sitenumt, "+
            //                        "'' as refnum, "+
            //                        "'"+BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text))+"' as [date], "+
            //                        "'' as nric,  "+
            //                        "'' as empname  "+
            //                        "from sitm SM LEFT JOIN SITMT ST  ON SM.SITENUM=ST.SITENUM  where  SM.[status]<>'V'  and  ST.[status]<>'V' and ST.flag='PSITM' "+
            //                    ")A "+
            //                    "LEFT JOIN  "+
            //                    "( "+
            //                    "Select "+
            //                        "sitenum, "+
            //                        "sitename, "+
            //                        "sitenumt, "+
            //                        "refnum, "+
            //                        "CONVERT(nvarchar,trandate,112) as [date], "+
            //                        "nric, "+
            //                        "'' as empname "+
            //                    "from worh where flag='PWOR' and trandate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and [status]<>'V' " +
            //                    ")B "+
            //                    "ON  A.sitenum=B.sitenum AND A.sitenumt = B.sitenumt AND A.[date]=B.[date]";

            //string Get = "  Select " +
            //                    "a.docunum, " +
            //                    "a.sitenum, " +
            //                    "a.sitename, " +
            //                    "a.refnum, " +
            //                   "'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' as [date], " +
            //                    "a.empnum, " +
            //                    "h.empname, " +
            //                    "a.actualempnum,  " +
            //                    "a.actualempname " +
            //                "from " +
            //                "( " +
            //                "Select ch.refnum as docunum, c18.sitenum, ch.sitename, '' as refnum,'' as [date],c18.empnum,'' as actualempnum, '' as actualempname from ctr18 c18 left join ctrh ch on c18.refnum=ch.refnum " +
            //                "where ch.[status]<>'V' and ch.commencedate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and ch.enddate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and c18.sitenum is not null " +
            //                "group by ch.refnum,c18.sitenum,ch.sitename,c18.empnum " +
            //                ")a " +
            //                "left join hemph h on a.empnum=h.empnum";


            //string Get = "  SELECT   "+
            //                   "p.SITENUM,   "+
            //                   "p.SITENAME,   "+
            //                    "CASE WHEN w.docunum IS NOT NULL OR w.docunum<>'' THEN w.docunum ELSE p.docunum END AS DOCUNUM,  " +
            //                   //"p.docunum,  "+
            //                   "CASE WHEN w.refnum IS NOT NULL OR w.refnum<>'' THEN w.refnum ELSE p.refnum END AS REFNUM,  "+
            //                   "CASE WHEN w.[date] IS NOT NULL OR w.[date]<>'' THEN w.[date] ELSE p.[date] END AS [DATE],   " +
            //                   "CASE WHEN w.empnum IS NOT NULL OR w.empnum<>'' THEN w.empnum ELSE p.empnum END AS EMPNUM,   "+
            //                   "CASE WHEN w.empnum IS NOT NULL OR w.empnum<>'' THEN [dbo].[GET_EMPNAME](w.empnum) ELSE [dbo].[GET_EMPNAME](p.empnum) END AS EMPNAME,   "+
            //                   "CASE WHEN w.actualempnum IS NOT NULL OR w.actualempnum<>'' THEN w.actualempnum ELSE p.actualempnum END AS actualempnum,   "+
            //                   "CASE WHEN w.actualempnum IS NOT NULL OR w.actualempnum<>'' THEN [dbo].[GET_EMPNAME](w.actualempnum) ELSE [dbo].[GET_EMPNAME](p.actualempnum) END AS actualempname,  "+
            //                   "CASE WHEN w.actualdate IS NOT NULL OR w.actualdate<>'' THEN w.actualdate ELSE p.actualdate END AS actualdate,  " +
            //                   "p.FREQUENCYCODE,  " +
            //                   "p.PDATES  " +
            //                "FROM  "+
            //                "(  "+
            //                   " Select   "+
            //                        "a.docunum,   "+
            //                       " a.sitenum,   "+
            //                        "a.sitename,   "+
            //                        "a.refnum,   "+
            //                     "'' as [date], " +
            //                        "a.empnum,   "+
            //                       " h.empname,   "+
            //                       " a.actualempnum,    "+
            //                        "a.actualempname,   "+
            //                        "'' as actualdate,   " +
            //                        "a.frequencycode,   " +
            //                        "a.pdates   " +
            //                    "from   "+
            //                    "(   "+
            //                    "Select ch.refnum as docunum, c18.sitenum, ch.sitename, '' as refnum,'' as [date],c18.empnum,'' as actualempnum, '' as actualempname, c18.frequencycode,c18.pdates from ctr18 c18 left join ctrh ch on c18.refnum=ch.refnum   " +
            //                    "where ch.[status]<>'V' and ch.commencedate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' and ch.enddate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and c18.sitenum is not null   " +
            //                    "group by ch.refnum,c18.sitenum,ch.sitename,c18.empnum, c18.frequencycode,c18.pdates   " +
            //                    ")a   "+
            //                    "left join hemph h on a.empnum=h.empnum  "+
            //                ")p  "+
            //                "left join  "+
            //                "(  "+
            //                 "Select  "+
            //                       "docunum,  "+
            //                       "sitenum,   "+
            //                       "sitename,   "+
            //                       "refnum,   "+
            //                       "CONVERT(nvarchar,trandate,112) as [date],   "+                    
            //                       "empnum,   "+
            //                       "empname,  "+
            //                       "actualempnum,  "+
            //                       "actualempname,  "+
            //                       "CONVERT(nvarchar,actualdate,112) as actualdate, '' as frequencycode, '' as pdates      " +
            //                   "from worh where flag='PWOR' and trandate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and trandate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' and [status]<>'V'   " +
            //                ")w  "+
            //                  " ON  p.sitenum=w.sitenum AND p.docunum = w.docunum AND p.[date]=w.[date]";


            string Get = "  select   " +
                                   "a.docunum,   " +
                                  " a.sitenum,   " +
                                   "a.sitename,   " +
                                   "a.refnum,   " +
                                "'' as [date], " +
                                   "a.empnum,   " +
                                  " h.empname,   " +
                                   "a.empnum2,   " +
                                   "'' as empname2,   " +
                                   "a.empnum3,   " +
                                   "'' as empname3,   " +
                                  " a.actualempnum,    " +
                                   "a.actualempname,   " +
                                   "'' as actualdate,   " +
                                   "a.frequencycode,   " +
                                   "a.pdates   " +
                               "from   " +
                               "(   " +
                               "Select ch.refnum as docunum, c18.sitenum, ch.sitename, '' as refnum,'' as [date],c18.empnum,c18.empnum2,c18.empnum3, '' as actualempnum, '' as actualempname, c18.frequencycode,c18.pdates from ctr18 c18 left join ctrh ch on c18.refnum=ch.refnum   " +
                               "where ch.[status]<>'V' and ch.commencedate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' and ch.enddate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and c18.sitenum is not null   " +
                               "group by ch.refnum,c18.sitenum,ch.sitename,c18.empnum,c18.empnum2,c18.empnum3, c18.frequencycode,c18.pdates   " +
                               ")a  left join hemph h on a.empnum=h.empnum  ";


            //string Get2 = " SELECT * FROM  (SELECT   " +
            //                 "p.SITENUM,   " +
            //                 "p.SITENAME,   " +
            //                 "p.DOCUNUM,  " +
            //                 "CASE WHEN w.refnum IS NOT NULL OR w.refnum<>'' THEN w.refnum ELSE p.refnum END AS REFNUM,  " +
            //                 "w.[DATE],   " +
            //                 "CASE WHEN w.empnum IS NOT NULL OR w.empnum<>'' THEN w.empnum ELSE p.empnum END AS EMPNUM,   " +
            //                 "CASE WHEN w.empnum IS NOT NULL OR w.empnum<>'' THEN [dbo].[GET_EMPNAME](w.empnum) ELSE [dbo].[GET_EMPNAME](p.empnum) END AS EMPNAME,   " +
            //                 "CASE WHEN w.actualempnum IS NOT NULL OR w.actualempnum<>'' THEN w.actualempnum ELSE p.actualempnum END AS actualempnum,   " +
            //                 "CASE WHEN w.actualempnum IS NOT NULL OR w.actualempnum<>'' THEN [dbo].[GET_EMPNAME](w.actualempnum) ELSE [dbo].[GET_EMPNAME](p.actualempnum) END AS actualempname,  " +
            //                 "CASE WHEN w.actualdate IS NOT NULL OR w.actualdate<>'' THEN w.actualdate ELSE p.actualdate END AS actualdate,  " +
            //                 "p.FREQUENCYCODE,  " +
            //                 "p.PDATES  " +
            //              "FROM  " +
            //              "(  " +
            //                 " Select   " +
            //                      "a.docunum,   " +
            //                     " a.sitenum,   " +
            //                      "a.sitename,   " +
            //                      "a.refnum,   " +
            //                   "'' as [date], " +
            //                      "a.empnum,   " +
            //                     " h.empname,   " +
            //                     " a.actualempnum,    " +
            //                      "a.actualempname,   " +
            //                      "'' as actualdate,   " +
            //                      "a.frequencycode,   " +
            //                      "a.pdates   " +
            //                  "from   " +
            //                  "(   " +
            //                  "Select ch.refnum as docunum, c18.sitenum, ch.sitename, '' as refnum,'' as [date],c18.empnum,'' as actualempnum, '' as actualempname, c18.frequencycode,c18.pdates from ctr18 c18 left join ctrh ch on c18.refnum=ch.refnum   " +
            //                  "where ch.[status]<>'V' and ch.commencedate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' and ch.enddate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and c18.sitenum is not null   " +
            //                  "group by ch.refnum,c18.sitenum,ch.sitename,c18.empnum, c18.frequencycode,c18.pdates   " +
            //                  ")a   " +
            //                  "left join hemph h on a.empnum=h.empnum  " +
            //              ")p  " +
            //              "left join  " +
            //              "(  " +
            //               "Select  " +
            //                     "docunum,  " +
            //                     "sitenum,   " +
            //                     "sitename,   " +
            //                     "refnum,   " +
            //                     "CONVERT(nvarchar,trandate,112) as [date],   " +
            //                     "empnum,   " +
            //                     "empname,  " +
            //                     "actualempnum,  " +
            //                     "actualempname,  " +
            //                     "CONVERT(nvarchar,actualdate,112) as actualdate, '' as frequencycode, '' as pdates      " +
            //                 "from worh where flag='PWOR' and trandate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and trandate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' and [status]<>'V'   " +
            //              ")w  " +
            //                " ON  p.sitenum=w.sitenum AND p.docunum = w.docunum AND p.[date]=w.[date])X WHERE 1=2";

            string Get2 =  " Select   " +
                                  "a.docunum,   " +
                                 " a.sitenum,   " +
                                  "a.sitename,   " +
                                  "a.refnum,   " +
                               "'' as [date], " +
                                  "a.empnum,   " +
                                 "'' as empname,   " +
                                  "a.empnum2,   " +
                                   "'' as empname2,   " +
                                   "a.empnum3,   " +
                                   "'' as empname3,   " +
                                 " a.actualempnum,    " +
                                  "a.actualempname,   " +
                                  "'' as actualdate,   " +
                                  "a.frequencycode,   " +
                                  "a.pdates   " +
                              "from   " +
                              "(   " +
                              "Select ch.refnum as docunum, c18.sitenum, ch.sitename, '' as refnum,'' as [date],c18.empnum, c18.empnum2,c18.empnum3,'' as actualempnum, '' as actualempname, c18.frequencycode,c18.pdates from ctr18 c18 left join ctrh ch on c18.refnum=ch.refnum   " +
                              "where ch.[status]<>'V' and ch.commencedate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' and ch.enddate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and c18.sitenum is not null   " +
                              "group by ch.refnum,c18.sitenum,ch.sitename,c18.empnum,c18.empnum2,c18.empnum3, c18.frequencycode,c18.pdates   " +
                              ")a WHERE 1=2";

            string Get3 = "Select  " +
                     "docunum,  " +
                     "sitenum,   " +
                     "sitename,   " +
                     "refnum,   " +
                     "CONVERT(nvarchar,trandate,112) as [date],   " +
                     "empnum,   " +
                     "empname,  " +
                     "empnum2,   " +
                     "empname2,  " +
                     "empnum3,   " +
                     "empname3,  " +
                     "actualempnum,  " +
                     "actualempname,  " +
                     "CONVERT(nvarchar,actualdate,112) as actualdate, '' as frequencycode, '' as pdates,wguid as [guid]      " +

                    "from worh where flag='PWOR' and trandate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and trandate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' and [status]<>'V'   ";



            this.dbaccess.ReadSQL("SiteListTb", Get); // Get Raw data from PCTR

            this.dbaccess.ReadSQL("SiteListTb2", Get2); //  Create and Empty Table base on SiteListTb

            this.dbaccess.ReadSQL("SiteListTb3", Get3); // Get Saved lists from WOR1

            this.dbaccess.ReadSQL("SiteListTb4", Get2); //  Create and Empty Table base on SiteListTb



            SiteListTb = this.dbaccess.DataSet.Tables["SiteListTb"];

            DataTable SiteListTb2 = this.dbaccess.DataSet.Tables["SiteListTb2"];

            DataTable SiteListTb3 = this.dbaccess.DataSet.Tables["SiteListTb3"];

            if (SiteListTb.Rows.Count > 0)
            {
                ///// Add rules for Frequency Codes

                if (SiteListTb.Columns.Contains("include"))
                {
                    SiteListTb.Columns["include"].Dispose();
                    SiteListTb.Columns.Remove("include");
                    DataColumn include = new DataColumn("include", typeof(int));

                    SiteListTb.Columns.Add(include);
                }
                else
                {

                    DataColumn include = new DataColumn("include", typeof(int));

                    SiteListTb.Columns.Add(include);
                }

                if (SiteListTb2.Columns.Contains("guid"))
                {
                    SiteListTb2.Columns["guid"].Dispose();
                    SiteListTb2.Columns.Remove("guid");
                    DataColumn guid = new DataColumn("guid", typeof(string));

                    SiteListTb2.Columns.Add(guid);
                }
                else
                {

                    DataColumn guid = new DataColumn("guid", typeof(string));

                    SiteListTb2.Columns.Add(guid);
                }


                foreach (DataRow dr1 in SiteListTb.Rows)
                {
                    dr1["include"] = 0;
                }

                foreach (DataRow dr1 in SiteListTb.Rows)
                {
                    string refnum = dr1["docunum"].ToString();
                    if (dr1["frequencycode"].ToString().Contains("MONTHLY") || dr1["frequencycode"].ToString().Contains("Monthly"))
                    {
                        string[] pdateStr = Convert.ToString(dr1["pdates"].ToString()).Split('/');

                        if (dr1["pdates"].ToString() != string.Empty)
                        {

                            if (dr1["frequencycode"].ToString().Trim().ToUpper() == "MONTHLYODD")
                            {
                                if (pdateStr.ToString().Trim() != string.Empty)
                                {
                                    if (BizLogicTools.Tools.IsOdd(Convert.ToDateTime(dateTimePicker2.Text).Month))
                                    {
                                        for (int i = 0; i < pdateStr.Length; i++)
                                        {
                                            string test = Convert.ToDateTime(dateTimePicker1.Text).Day.ToString();
                                            string test2 = pdateStr[i].ToString();
                                            if (
                                            (Convert.ToDateTime(dateTimePicker1.Text) <= TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))))
                                            &&
                                            (Convert.ToDateTime(dateTimePicker2.Text) >= TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))))
                                            )
                                            {
                                                DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                                insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                                insertSiteListTb2["sitename"] = dr1["sitename"];
                                                insertSiteListTb2["docunum"] = dr1["docunum"];
                                                insertSiteListTb2["refnum"] = dr1["refnum"];
                                                insertSiteListTb2["date"] = TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()));
                                                insertSiteListTb2["empnum"] = dr1["empnum"];
                                                insertSiteListTb2["empname"] = dr1["empname2"];
                                                insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                                insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                                insertSiteListTb2["empnum3"] = dr1["empnum3"];
                                                insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                                insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                                insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                                insertSiteListTb2["actualdate"] = TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()));
                                                insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                                insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                                insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();

                                                SiteListTb2.Rows.Add(insertSiteListTb2);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (dr1["frequencycode"].ToString().Trim().ToUpper() == "MONTHLYEVE")
                            {
                                if (dr1["pdates"].ToString() != string.Empty)
                                {
                                    if (BizLogicTools.Tools.IsEven(Convert.ToDateTime(dateTimePicker2.Text).Month))
                                    {
                                        for (int i = 0; i < pdateStr.Length; i++)
                                        {
                                            string test = Convert.ToDateTime(dateTimePicker1.Text).Day.ToString();
                                            string test2 = pdateStr[i].ToString();

                                            DateTime dt1 = TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())));


                                            if (
                                            (Convert.ToDateTime(dateTimePicker1.Text) <= TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))))
                                            &&
                                            (Convert.ToDateTime(dateTimePicker2.Text) >= TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))))
                                            )
                                            {
                                                DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                                insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                                insertSiteListTb2["sitename"] = dr1["sitename"];
                                                insertSiteListTb2["docunum"] = dr1["docunum"];
                                                insertSiteListTb2["refnum"] = dr1["refnum"];
                                                insertSiteListTb2["date"] = BizFunctions.GetSafeDateString(TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))));
                                                insertSiteListTb2["empnum"] = dr1["empnum"];
                                                insertSiteListTb2["empname"] = dr1["empname"];
                                                insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                                insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                                insertSiteListTb2["empnum3"] = dr1["empnum3"];

                                                insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                                insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                                insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                                insertSiteListTb2["actualdate"] = BizFunctions.GetSafeDateString(TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))));
                                                insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                                insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                                insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();


                                                SiteListTb2.Rows.Add(insertSiteListTb2);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (dr1["frequencycode"].ToString().Trim().ToUpper() == "MONTHLY")
                            {
                                if (dr1["pdates"].ToString() != string.Empty)
                                {
                                    for (int i = 0; i < pdateStr.Length; i++)
                                    {
                                        string test = Convert.ToDateTime(dateTimePicker1.Text).Day.ToString();
                                        string test2 = pdateStr[i].ToString();

                                        DateTime dt1 = TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())));




                                        if (
                                            (Convert.ToDateTime(dateTimePicker1.Text) <= TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))))
                                            &&
                                            (Convert.ToDateTime(dateTimePicker2.Text) >= TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))))
                                            )
                                        {
                                            DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                            insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                            insertSiteListTb2["sitename"] = dr1["sitename"];
                                            insertSiteListTb2["docunum"] = dr1["docunum"];
                                            insertSiteListTb2["refnum"] = dr1["refnum"];
                                            insertSiteListTb2["date"] = BizFunctions.GetSafeDateString(TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))));
                                            insertSiteListTb2["empnum"] = dr1["empnum"];
                                            insertSiteListTb2["empname"] = dr1["empname"];
                                            insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                            insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                            insertSiteListTb2["empnum3"] = dr1["empnum3"];

                                            insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                            insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                            insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                            insertSiteListTb2["actualdate"] = BizFunctions.GetSafeDateString(TimeTools.GetSafeDate(TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text).Month, Convert.ToDateTime(dateTimePicker2.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim()))));
                                            insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                            insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                            insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();


                                            SiteListTb2.Rows.Add(insertSiteListTb2);
                                        }
                                    }
                                }
                            }
                        }
                    }

                  
                        if (dr1["frequencycode"].ToString().Contains("WEEKLY") || dr1["frequencycode"].ToString().Contains("Weekly"))
                        {
                            string[] pdateStr = Convert.ToString(dr1["pdates"].ToString()).Split('/');
                            if (dr1["frequencycode"].ToString().Trim().ToUpper() == "WEEKLYODD")
                            {
                                if (dr1["pdates"].ToString() != string.Empty)
                                {

                                    if (BizLogicTools.Tools.IsOdd(TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text))))
                                    {
                                        for (int i = 0; i < pdateStr.Length; i++)
                                        {
                                            string test = TimeTools.GetDayOfWeekNo(Convert.ToDateTime(dateTimePicker1.Text).DayOfWeek.ToString().Trim()).ToString();
                                            string test2 = pdateStr[i].ToString();

                                            // old code
                                            DateTime dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                            DateTime dt2;

                                            //DateTime dt1;
                                            //DateTime dt2;

                                            //if (Convert.ToDateTime(dateTimePicker1.Text).Year < Convert.ToDateTime(dateTimePicker2.Text).Year)
                                            //{
                                            //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker1.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                            //}
                                            //else
                                            //{
                                            //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                            //}      

                                            if ((Convert.ToInt32(pdateStr[i].ToString()) - 1) > 0)
                                            {
                                                dt2 = dt1.AddDays(Convert.ToInt32(pdateStr[i].ToString()) - 1);
                                            }
                                            else
                                            {
                                                dt2 = dt1;
                                            }

                                            if (
                                                  (Convert.ToDateTime(dateTimePicker1.Text) <= dt2)
                                                  &&
                                                  (Convert.ToDateTime(dateTimePicker2.Text) >= dt2)
                                               )
                                            {
                                                DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                                insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                                insertSiteListTb2["sitename"] = dr1["sitename"];
                                                insertSiteListTb2["docunum"] = dr1["docunum"];
                                                insertSiteListTb2["refnum"] = dr1["refnum"];
                                                insertSiteListTb2["date"] = BizFunctions.GetSafeDateString(dt2);
                                                insertSiteListTb2["empnum"] = dr1["empnum"];
                                                insertSiteListTb2["empname"] = dr1["empname"];
                                                insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                                insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                                insertSiteListTb2["empnum3"] = dr1["empnum3"];

                                                insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                                insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                                insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                                insertSiteListTb2["actualdate"] = BizFunctions.GetSafeDateString(dt2);
                                                insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                                insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                                insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();


                                                SiteListTb2.Rows.Add(insertSiteListTb2);
                                            }
                                        }

                                    }
                                }
                            }
                            else if (dr1["frequencycode"].ToString().Trim().ToUpper() == "WEEKLYEVE")
                            {
                                if (dr1["pdates"].ToString() != string.Empty)
                                {
                                    if (BizLogicTools.Tools.IsEven(TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text))))
                                    {
                                        for (int i = 0; i < pdateStr.Length; i++)
                                        {
                                            string test = TimeTools.GetDayOfWeekNo(Convert.ToDateTime(dateTimePicker1.Text).DayOfWeek.ToString().Trim()).ToString();
                                            string test2 = pdateStr[i].ToString();

                                            // old code
                                            DateTime dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                            DateTime dt2;

                                            //DateTime dt1;
                                            //DateTime dt2;

                                            //if (Convert.ToDateTime(dateTimePicker1.Text).Year < Convert.ToDateTime(dateTimePicker2.Text).Year)
                                            //{
                                            //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker1.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                            //}
                                            //else
                                            //{
                                            //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                            //}      

                                            if ((Convert.ToInt32(pdateStr[i].ToString()) - 1) > 0)
                                            {
                                                dt2 = dt1.AddDays(Convert.ToInt32(pdateStr[i].ToString()) - 1);
                                            }
                                            else
                                            {
                                                dt2 = dt1;
                                            }

                                            if (
                                                  (Convert.ToDateTime(dateTimePicker1.Text) <= dt2)
                                                  &&
                                                  (Convert.ToDateTime(dateTimePicker2.Text) >= dt2)
                                               )
                                            {
                                                DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                                insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                                insertSiteListTb2["sitename"] = dr1["sitename"];
                                                insertSiteListTb2["docunum"] = dr1["docunum"];
                                                insertSiteListTb2["refnum"] = dr1["refnum"];
                                                insertSiteListTb2["date"] = BizFunctions.GetSafeDateString(dt2);
                                                insertSiteListTb2["empnum"] = dr1["empnum"];
                                                insertSiteListTb2["empname"] = dr1["empname"];
                                                insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                                insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                                insertSiteListTb2["empnum3"] = dr1["empnum3"];

                                                insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                                insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                                insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                                insertSiteListTb2["actualdate"] = BizFunctions.GetSafeDateString(dt2);
                                                insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                                insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                                insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();


                                                SiteListTb2.Rows.Add(insertSiteListTb2);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (dr1["frequencycode"].ToString().Trim().ToUpper() == "WEEKLY")
                            {
                                if (dr1["pdates"].ToString() != string.Empty)
                                {
                                    for (int i = 0; i < pdateStr.Length; i++)
                                    {
                                        string test = TimeTools.GetDayOfWeekNo(Convert.ToDateTime(dateTimePicker1.Text).DayOfWeek.ToString().Trim()).ToString();
                                        string test2 = pdateStr[i].ToString();

                                        // old code
                                        DateTime dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                        DateTime dt2;

                                        //DateTime dt1;
                                        //DateTime dt2;

                                        //if (Convert.ToDateTime(dateTimePicker1.Text).Year < Convert.ToDateTime(dateTimePicker2.Text).Year)
                                        //{
                                        //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker1.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                        //}
                                        //else
                                        //{
                                        //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                        //}       

                                        if ((Convert.ToInt32(pdateStr[i].ToString()) - 1) > 0)
                                        {
                                            dt2 = dt1.AddDays(Convert.ToInt32(pdateStr[i].ToString()) - 1);
                                        }
                                        else
                                        {
                                            dt2 = dt1;
                                        }

                                        if (
                                              (Convert.ToDateTime(dateTimePicker1.Text) <= dt2)
                                              &&
                                              (Convert.ToDateTime(dateTimePicker2.Text) >= dt2)
                                           )
                                        {
                                            DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                            insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                            insertSiteListTb2["sitename"] = dr1["sitename"];
                                            insertSiteListTb2["docunum"] = dr1["docunum"];
                                            insertSiteListTb2["refnum"] = dr1["refnum"];
                                            insertSiteListTb2["date"] = BizFunctions.GetSafeDateString(dt2);
                                            insertSiteListTb2["empnum"] = dr1["empnum"];
                                            insertSiteListTb2["empname"] = dr1["empname"];
                                            insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                            insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                            insertSiteListTb2["empnum3"] = dr1["empnum3"];

                                            insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                            insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                            insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                            insertSiteListTb2["actualdate"] = BizFunctions.GetSafeDateString(dt2);
                                            insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                            insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                            insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();


                                            SiteListTb2.Rows.Add(insertSiteListTb2);
                                        }
                                    }
                                }
                            }
                         }
                            
                           

     
                   
                        if (dr1["frequencycode"].ToString().Contains("YEARLY") || dr1["frequencycode"].ToString().Contains("Yearly"))
                        {
                            string[] pdateStr = Convert.ToString(dr1["pdates"].ToString()).Split('/');
                            if (dr1["pdates"].ToString() != string.Empty)
                            {
                                for (int i = 0; i < pdateStr.Length; i++)
                                {
                                    string test = Convert.ToDateTime(dateTimePicker1.Text).DayOfYear.ToString();
                                    string test2 = pdateStr[i].ToString();


                                    //DateTime dt = TimeTools.GetDateOfYear(Convert.ToInt32(pdateStr[i].ToString().Trim()),Convert.ToDateTime(dateTimePicker2.Text).Year);

                                    DateTime dt1 = Convert.ToDateTime(dateTimePicker1.Text);
                                    DateTime dt2 = Convert.ToDateTime(dateTimePicker2.Text);
                                    int year1 = Convert.ToDateTime(dateTimePicker2.Text).Year;
                                    int no1 = Convert.ToInt32(pdateStr[i].ToString().Trim());
                                    DateTime dt3 = TimeTools.GetDateOfYear(Convert.ToInt32(pdateStr[i].ToString().Trim()), Convert.ToDateTime(dateTimePicker2.Text).Year);

                                    if (
                                       (Convert.ToDateTime(dateTimePicker1.Text) <= TimeTools.GetDateOfYear(Convert.ToInt32(pdateStr[i].ToString().Trim()), Convert.ToDateTime(dateTimePicker2.Text).Year))
                                       &&
                                       (Convert.ToDateTime(dateTimePicker2.Text) >= TimeTools.GetDateOfYear(Convert.ToInt32(pdateStr[i].ToString().Trim()), Convert.ToDateTime(dateTimePicker2.Text).Year))
                                       )
                                    {
                                        DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                        insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                        insertSiteListTb2["sitename"] = dr1["sitename"];
                                        insertSiteListTb2["docunum"] = dr1["docunum"];
                                        insertSiteListTb2["refnum"] = dr1["refnum"];
                                        insertSiteListTb2["date"] = BizFunctions.GetSafeDateString(TimeTools.GetDateOfYear(Convert.ToInt32(pdateStr[i].ToString().Trim()), Convert.ToDateTime(dateTimePicker2.Text).Year));
                                        insertSiteListTb2["empnum"] = dr1["empnum"];
                                        insertSiteListTb2["empname"] = dr1["empname"];
                                        insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                        insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                        insertSiteListTb2["empnum3"] = dr1["empnum3"];

                                        insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                        insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                        insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                        insertSiteListTb2["actualdate"] = BizFunctions.GetSafeDateString(TimeTools.GetDateOfYear(Convert.ToInt32(pdateStr[i].ToString().Trim()), Convert.ToDateTime(dateTimePicker2.Text).Year));
                                        insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                        insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                        insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();


                                        SiteListTb2.Rows.Add(insertSiteListTb2);
                                    }
                                }
                            }
                        }



                        if (dr1["frequencycode"].ToString().Trim().ToUpper() == "DAILY" || dr1["frequencycode"].ToString().Contains("Daily"))
                        {
                            string[] pdateStr = Convert.ToString(dr1["pdates"].ToString()).Split('/');
                            if (dr1["pdates"].ToString() != string.Empty)
                            {
                                for (int i = 0; i < pdateStr.Length; i++)
                                {
                                    string test = TimeTools.GetDayOfWeekNo(Convert.ToDateTime(dateTimePicker1.Text).DayOfWeek.ToString().Trim()).ToString();
                                    string test2 = pdateStr[i].ToString();

                                    DateTime dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                    DateTime dt2;

                                    //DateTime dt1;
                                    //DateTime dt2;

                                    //if (Convert.ToDateTime(dateTimePicker1.Text).Year < Convert.ToDateTime(dateTimePicker2.Text).Year)
                                    //{
                                    //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker1.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                    //}
                                    //else
                                    //{
                                    //    dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker2.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker2.Text)));
                                    //}  

                                    if ((Convert.ToInt32(pdateStr[i].ToString()) - 1) > 0)
                                    {
                                        dt2 = dt1.AddDays(Convert.ToInt32(pdateStr[i].ToString()) - 1);
                                    }
                                    else
                                    {
                                        dt2 = dt1;
                                    }

                                    if (
                                          (Convert.ToDateTime(dateTimePicker1.Text) <= dt2)
                                          &&
                                          (Convert.ToDateTime(dateTimePicker2.Text) >= dt2)
                                       )
                                    {
                                        DataRow insertSiteListTb2 = SiteListTb2.NewRow();

                                        insertSiteListTb2["sitenum"] = dr1["sitenum"];
                                        insertSiteListTb2["sitename"] = dr1["sitename"];
                                        insertSiteListTb2["docunum"] = dr1["docunum"];
                                        insertSiteListTb2["refnum"] = dr1["refnum"];
                                        insertSiteListTb2["date"] = BizFunctions.GetSafeDateString(dt2);
                                        insertSiteListTb2["empnum"] = dr1["empnum"];
                                        insertSiteListTb2["empname"] = dr1["empname"];
                                        insertSiteListTb2["empnum2"] = dr1["empnum2"];
                                        insertSiteListTb2["empname2"] = BizLogicTools.Tools.GetEmpname(dr1["empnum2"].ToString(), this.dbaccess);
                                        insertSiteListTb2["empnum3"] = dr1["empnum3"];

                                        insertSiteListTb2["empname3"] = BizLogicTools.Tools.GetEmpname(dr1["empnum3"].ToString(), this.dbaccess);
                                        insertSiteListTb2["actualempnum"] = dr1["actualempnum"];
                                        insertSiteListTb2["actualempname"] = dr1["actualempname"];
                                        insertSiteListTb2["actualdate"] = BizFunctions.GetSafeDateString(dt2);
                                        insertSiteListTb2["frequencycode"] = dr1["frequencycode"];
                                        insertSiteListTb2["pdates"] = pdateStr[i].ToString();
                                        insertSiteListTb2["guid"] = dr1["sitenum"].ToString() + dr1["docunum"].ToString() + TimeTools.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text).Month, Convert.ToDateTime(dateTimePicker1.Text).Year, Convert.ToInt32(pdateStr[i].ToString().Trim())) + dr1["frequencycode"].ToString() + pdateStr[i].ToString();


                                        SiteListTb2.Rows.Add(insertSiteListTb2);
                                    }
                                }
                            }
                        }
                   
                    

                }
            }

            

            OriTable = SiteListTb.Copy();

            DataTable SiteListTb1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select * from SiteListTb where include=1");
            SiteListTb1.TableName = "SiteListTb1";
            BindingSource bs = new BindingSource();


            /////////////
            string filter1 = "";
            if (SiteListTb2.Rows.Count > 0)
            {
                foreach (DataRow dr2 in SiteListTb2.Rows)
                {
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from pworX1 where [guid]='" + dr2["sitenum"].ToString() + dr2["docunum"].ToString() + dr2["actualdate"].ToString() + dr2["pdates"].ToString()  + "' ");
                    Parameter[] parameters1 = new Parameter[11];
                    parameters1[0] = new Parameter("@sitenum", dr2["sitenum"].ToString());
                    parameters1[1] = new Parameter("@trandate", dr2["date"].ToString());
                    parameters1[2] = new Parameter("@actualdate", dr2["actualdate"].ToString());
                    parameters1[3] = new Parameter("@empnum", dr2["empnum"].ToString());
                    parameters1[4] = new Parameter("@empnum2", dr2["empnum2"].ToString());
                    parameters1[5] = new Parameter("@empnum3", dr2["empnum3"].ToString());
                    parameters1[6] = new Parameter("@actualempnum", dr2["actualempnum"].ToString());
                    parameters1[7] = new Parameter("@docunum", dr2["docunum"].ToString());
                    parameters1[8] = new Parameter("@refnum", dr2["refnum"].ToString());
                    parameters1[9] = new Parameter("@frequencycode", dr2["frequencycode"].ToString());
                    parameters1[10] = new Parameter("@guid", dr2["guid"].ToString());

                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_submit_Insert_PWOR_GenerateTemp1", ref parameters1);

                    filter1 = filter1 + "'" + dr2["guid"].ToString() +"',";
                }
               
            }
            if (filter1.Length > 0)
            {


                filter1 = filter1.Remove(filter1.Length - 1);
                string str1 = "Select docunum,sitenum,sitename,refnum,CONVERT(nvarchar,[date],112) as [date],empnum,empnum2,empnum3,empname,actualempnum,actualempname, CONVERT(nvarchar,actualdate,112) as actualdate,frequencycode,pdates,[guid] from pworX1 where [guid] in (" + filter1 + ") ";

                this.dbaccess.ReadSQL("TempPesList", str1);

                DataTable TempPesList = this.dbaccess.DataSet.Tables["TempPesList"];
                /////////////

                if (SiteListTb2.Rows.Count > 0)
                {
                    if (SiteListTb3.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in SiteListTb2.Rows)
                        {
                            foreach (DataRow dr3 in SiteListTb3.Rows)
                            {
                                if (dr2["guid"].ToString().Trim() == dr3["guid"].ToString().Trim())
                                {
                                    dr2["refnum"] = dr3["refnum"];
                                    if (!BizFunctions.IsEmpty(dr3["empnum"]))
                                    {
                                        dr2["empnum"] = dr3["empnum"];
                                        dr2["empname"] = BizLogicTools.Tools.GetEmpname(dr3["empnum"].ToString(), this.dbaccess);
                                    }
                                    else
                                    {
                                        dr2["empname"] = BizLogicTools.Tools.GetEmpname(dr2["empnum"].ToString(), this.dbaccess);
                                    }
                                   
                                    if (!BizFunctions.IsEmpty(dr3["empnum2"]))
                                    {
                                        dr2["empnum2"] = dr3["empnum2"];
                                        dr2["empname2"] = BizLogicTools.Tools.GetEmpname(dr3["empnum2"].ToString(), this.dbaccess);
                                    }
                                    else
                                    {
                                        dr2["empname2"] = BizLogicTools.Tools.GetEmpname(dr2["empnum2"].ToString(), this.dbaccess);
                                    }
                                   
                                    if (!BizFunctions.IsEmpty(dr3["empnum3"]))
                                    {
                                        dr2["empnum3"] = dr3["empnum3"];
                                        dr2["empname3"] = BizLogicTools.Tools.GetEmpname(dr3["empnum3"].ToString(), this.dbaccess);
                                    }
                                    else
                                    {
                                        dr2["empname3"] = BizLogicTools.Tools.GetEmpname(dr2["empnum3"].ToString(), this.dbaccess);
                                    }
                               
                                }
                            }
                        }
                    }
                }
            }

            bs.DataSource = SiteListTb2;

            this.SiteDGV1.DataSource = bs.DataSource;

            if (SiteDGV1.Columns.Contains("Mark"))
            {
                SiteDGV1.Columns["mark"].Dispose();
                SiteDGV1.Columns.Remove("Mark");
                DataGridViewCheckBoxColumn mark = new DataGridViewCheckBoxColumn(false);
                mark.Name = "Mark";
                SiteDGV1.Columns.Add(mark);
            }
            else
            {

                DataGridViewCheckBoxColumn mark = new DataGridViewCheckBoxColumn(false);
                mark.Name = "Mark";
                SiteDGV1.Columns.Add(mark);
            }


            DataGridViewColumn Mark = SiteDGV1.Columns["Mark"];
            Mark.Width = 60;

            DataGridViewColumn Sitenum = SiteDGV1.Columns["Sitenum"];
            Sitenum.Width = 120;

            SiteDGV1.Columns["mark"].DisplayIndex = 0;
            SiteDGV1.Columns["sitenum"].DisplayIndex = 1;
            SiteDGV1.Columns["sitename"].DisplayIndex = 2;
            SiteDGV1.Columns["docunum"].DisplayIndex = 3;
            SiteDGV1.Columns["refnum"].DisplayIndex = 4;
            SiteDGV1.Columns["Date"].DisplayIndex = 5;
            SiteDGV1.Columns["actualdate"].DisplayIndex = 6;
            SiteDGV1.Columns["empnum"].DisplayIndex = 7;
            SiteDGV1.Columns["empname"].DisplayIndex = 8;
            SiteDGV1.Columns["empnum2"].DisplayIndex = 9;
            SiteDGV1.Columns["empname2"].DisplayIndex = 10;
            SiteDGV1.Columns["empnum3"].DisplayIndex = 11;
            SiteDGV1.Columns["empname3"].DisplayIndex = 12;
            SiteDGV1.Columns["actualempnum"].DisplayIndex = 13;
            SiteDGV1.Columns["actualempname"].DisplayIndex = 14;

            SiteDGV1.Columns["sitenum"].ReadOnly = true;
            SiteDGV1.Columns["sitenum"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["Sitename"].Visible = false;
            SiteDGV1.Columns["pdates"].Visible = false;
            //SiteDGV1.Columns["docunum"].Visible = false;
            SiteDGV1.Columns["refnum"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["Date"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["empnum"].DefaultCellStyle.BackColor = Color.Yellow;
            SiteDGV1.Columns["actualempname"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["actualempnum"].DefaultCellStyle.BackColor = Color.Yellow;

            SiteDGV1.Columns["empnum2"].DefaultCellStyle.BackColor = Color.Yellow;
            SiteDGV1.Columns["empname2"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["empnum3"].DefaultCellStyle.BackColor = Color.Yellow;
            SiteDGV1.Columns["empname3"].DefaultCellStyle.BackColor = Color.LightBlue;


            SiteDGV1.Columns["empname"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["actualempname"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["frequencycode"].DefaultCellStyle.BackColor = Color.LightBlue;
            SiteDGV1.Columns["pdates"].DefaultCellStyle.BackColor = Color.LightBlue;


            SiteDGV1.Columns["empnum"].HeaderText = "Emp No 1";
            SiteDGV1.Columns["empnum2"].HeaderText = "Emp No 2";
            SiteDGV1.Columns["empnum3"].HeaderText = "Emp No 3";

            SiteDGV1.Columns["empname"].HeaderText = "Name";
            SiteDGV1.Columns["empname2"].HeaderText = "Name";
            SiteDGV1.Columns["empname3"].HeaderText = "Name";



            SiteDGV1.Columns["actualempnum"].Visible = false;
            SiteDGV1.Columns["actualempname"].Visible = false;
            SiteDGV1.Columns["actualdate"].Visible = false;
            SiteDGV1.Columns["guid"].Visible = false;

        }

        private void GetWORSiteLists()
        {
           

        }

        private void GenWorkOrderForm1_Load(object sender, EventArgs e)
        {

            yearComboBox1.Text = Common.DEFAULT_SYSTEM_YEAR;

            weekComboBox1.DropDown += new EventHandler(weekComboBox1_DropDown);
            //ALLcb.Checked = true;

            //if (ALLcb.Checked == true)
            //{
            //    GetSiteLists();
            //}
            SVCcb.Visible = false;
            ALLcb.Visible = false;
            PWORcb.Visible = false;

            PWORcb.Checked = true;

            //dateTimePicker1.Format = DateTimePickerFormat.Custom;
            //dateTimePicker1.CustomFormat = "dd-MM-yyyy";

            //dateTimePicker2.Format = DateTimePickerFormat.Custom;
            //dateTimePicker2.CustomFormat = "dd-MM-yyyy";


            //if (PWORcb.Checked == false && SVCcb.Checked == true)
            //{
            //    GetWORSiteLists();
            //}
            //else if (PWORcb.Checked == true && SVCcb.Checked == false)
            //{
            //    GetPWORSiteLists();
            //}
            weekComboBox1.Text = TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker1.Text)).ToString();
            DateTime dt1 = TimeTools.GetFirstDayOfWeek(Convert.ToDateTime(dateTimePicker1.Text).Year, TimeTools.GetYearWeekNumber(Convert.ToDateTime(dateTimePicker1.Text))).AddDays(-1);
            DateTime dt2 = dt1.AddDays(6);

            dateTimePicker1.Text = dt1.ToShortDateString();
            dateTimePicker2.Text = dt2.ToShortDateString();
            

         
        }

        void weekComboBox1_DropDown(object sender, EventArgs e)
        {
            int noOfWeeks = TimeTools.GetWeeksInYear(Convert.ToDateTime(dateTimePicker1.Text).Year);

            for (int i = 1; i <= noOfWeeks; i++)
            {
                 weekComboBox1.Items.Add(i.ToString());
            }
        }

        //Jason:01-04-2015: 
        // private void GeneratePWOR()
        // {

        //     // Header
        //     arrDifference = new string[OriTable.Rows.Count];



        //     if (DifferenceTable.Rows.Count > 0)
        //     {
        //         BizFunctions.DeleteAllRows(DifferenceTable);
        //     }

        //     for (int x = 0; x < OriTable.Rows.Count; x++)
        //     {
        //         if (SiteDGV1.Rows[x].Cells[5].Value != "")
        //         {
        //             if (OriTable.Rows[x]["nric"].ToString().Trim() != SiteDGV1.Rows[x].Cells[5].Value)
        //             {
        //                 DataRow Insert = DifferenceTable.NewRow();

        //                 Insert["orinric"] = OriTable.Rows[x]["nric"].ToString().Trim();
        //                 Insert["newnric"] = SiteDGV1.Rows[x].Cells[5].Value;
        //                 Insert["orisite"] = OriTable.Rows[x]["sitenum"].ToString().Trim();
        //                 Insert["newsite"] = SiteDGV1.Rows[x].Cells[0].Value;

        //                 DifferenceTable.Rows.Add(Insert);

        //             }
        //         }
        //     }

        //     for (int i = 0; i < SiteDGV1.Rows.Count-1; i++)
        //     {

        //         if (SiteDGV1.Rows[i].Cells[3].Value == "")
        //         {
        //             try
        //             {
        //                 Parameter[] parameters1 = new Parameter[5];
        //                 parameters1[0] = new Parameter("@sitenum", SiteDGV1.Rows[i].Cells[0].Value);
        //                 parameters1[1] = new Parameter("@sitenumt", SiteDGV1.Rows[i].Cells[2].Value);
        //                 parameters1[2] = new Parameter("@trandate", BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)));
        //                 parameters1[3] = new Parameter("@nric", SiteDGV1.Rows[i].Cells[5].Value);
        //                 parameters1[4] = new Parameter("@user", Common.DEFAULT_SYSTEM_USERNAME);

        //                 DataSet ds_Ref = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("sp_submit_Insert_PWOR_Header_Generate", ref parameters1);
        //                 ds_Ref.Tables[0].TableName = "RefnumTB";

        //                 DataTable ResultTB = ds_Ref.Tables[0];

        //                 if (ResultTB != null)
        //                 {
        //                     if (ResultTB.Rows.Count > 0)
        //                     {
        //                         SiteDGV1.Rows[i].Cells[3].Value = ResultTB.Rows[0]["refnum"].ToString();
        //                         SiteDGV1.Rows[i].Cells[4].Value = BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text));
        //                         try
        //                         {
        //                             // Detail
        //                             Parameter[] parameters2 = new Parameter[2];
        //                             parameters2[0] = new Parameter("@refnum", ResultTB.Rows[0]["refnum"].ToString());
        //                             parameters2[1] = new Parameter("@sitenum", SiteDGV1.Rows[i].Cells[0].Value);

        //                             this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_Insert_Pest_Tasks_Generate", ref parameters2);
        //                         }
        //                         catch (Exception ex)
        //                         {
        //                         }

        //                     }
        //                 }

        //             }
        //             catch (Exception ex)
        //             {

        //             }
        //         }
        //         else
        //         {


        //             //sp_submit_Update_PWOR_Header_Generate


        //                 try
        //                 {
        //                     Parameter[] parameters3 = new Parameter[2];
        //                     parameters3[0] = new Parameter("@refnum", SiteDGV1.Rows[i].Cells[3].Value);
        //                     parameters3[1] = new Parameter("@nric", SiteDGV1.Rows[i].Cells[5].Value);

        //                     this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_submit_Update_PWOR_Header_Generate", ref parameters3);
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                 }

        //         }

        //     }

        //     OriTable = SiteListTb.Copy();

        //     if (DifferenceTable.Rows.Count > 0)
        //     {
        //         foreach (DataRow dr1 in DifferenceTable.Rows)
        //         {
        //             if (dr1.RowState != DataRowState.Deleted)
        //             {
        //                 //ReAssignedInfo = ReAssignedInfo + dr1["newnric"].ToString().Trim() +" as been re-assigned to "+dr1["newsite"].ToString().Trim()+" From "+dr1["orisite"].ToString().Trim()+"  on "+BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text))+". \n\n ";
        //                 ReAssignedInfo = ReAssignedInfo + dr1["newnric"].ToString().Trim() + " as been re-assigned to " + dr1["newsite"].ToString().Trim() + " on " + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + ". \n\n ";
        //             }
        //         }

        //         SendEmail();
        //     }

        //}

        private void GeneratePWOR()
        {

            // Header
            arrDifference = new string[OriTable.Rows.Count];



            //if (DifferenceTable.Rows.Count > 0)
            //{
            //    BizFunctions.DeleteAllRows(DifferenceTable);
            //}

            //for (int x = 0; x < OriTable.Rows.Count; x++)
            //{
            //    if (SiteDGV1.Rows[x].Cells[5].Value != "")
            //    {
            //        if (OriTable.Rows[x]["nric"].ToString().Trim() != SiteDGV1.Rows[x].Cells[5].Value)
            //        {
            //            DataRow Insert = DifferenceTable.NewRow();

            //            Insert["orinric"] = OriTable.Rows[x]["nric"].ToString().Trim();
            //            Insert["newnric"] = SiteDGV1.Rows[x].Cells[5].Value;
            //            Insert["orisite"] = OriTable.Rows[x]["sitenum"].ToString().Trim();
            //            Insert["newsite"] = SiteDGV1.Rows[x].Cells[0].Value;

            //            DifferenceTable.Rows.Add(Insert);

            //        }
            //    }
            //}

            for (int i = 0; i < SiteDGV1.Rows.Count - 1; i++)
            {

                if (SiteDGV1.Rows[i].Cells[3].Value == "")
                {
                    if (SiteDGV1.Rows[i].Cells[17].Value == null)
                    {
                        SiteDGV1.Rows[i].Cells[17].Value = false;
                    }
                    if ((bool)SiteDGV1.Rows[i].Cells[17].Value == true)
                    {

                        try
                        {

                            if (SiteDGV1.Rows[i].Cells[4].Value == "")
                            {
                                SiteDGV1.Rows[i].Cells[4].Value = SiteDGV1.Rows[i].Cells[9].Value;
                            }
                            Parameter[] parameters1 = new Parameter[11];
                            parameters1[0] = new Parameter("@sitenum", SiteDGV1.Rows[i].Cells[1].Value);
                            parameters1[1] = new Parameter("@trandate", SiteDGV1.Rows[i].Cells[4].Value);
                            if (BizFunctions.IsEmpty(SiteDGV1.Rows[i].Cells[5].Value))
                            {
                                SiteDGV1.Rows[i].Cells[5].Value = "";
                            }
                            parameters1[2] = new Parameter("@empnum", SiteDGV1.Rows[i].Cells[5].Value);

                            if (BizFunctions.IsEmpty(SiteDGV1.Rows[i].Cells[7].Value))
                            {
                                SiteDGV1.Rows[i].Cells[7].Value = "";
                            }
                            parameters1[3] = new Parameter("@empnum2", SiteDGV1.Rows[i].Cells[7].Value);

                            if (BizFunctions.IsEmpty(SiteDGV1.Rows[i].Cells[9].Value))
                            {
                                SiteDGV1.Rows[i].Cells[9].Value = "";
                            }
                            parameters1[4] = new Parameter("@empnum3", SiteDGV1.Rows[i].Cells[9].Value);

                            parameters1[5] = new Parameter("@actualempnum", SiteDGV1.Rows[i].Cells[11].Value);
                            parameters1[6] = new Parameter("@user", Common.DEFAULT_SYSTEM_USERNAME);
                            parameters1[7] = new Parameter("@docunum", SiteDGV1.Rows[i].Cells[0].Value);
                            parameters1[8] = new Parameter("@actualdate", SiteDGV1.Rows[i].Cells[13].Value);
                            parameters1[9] = new Parameter("@guid", SiteDGV1.Rows[i].Cells[16].Value);
                            parameters1[10] = new Parameter("@systemyear", Common.DEFAULT_SYSTEM_YEAR);

                            DataSet ds_Ref = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("sp_submit_Insert_PWOR_Header_Generate", ref parameters1);
                            ds_Ref.Tables[0].TableName = "RefnumTB";

                            DataTable ResultTB = ds_Ref.Tables[0];

                            if (ResultTB != null)
                            {
                                if (ResultTB.Rows.Count > 0)
                                {
                                    SiteDGV1.Rows[i].Cells[3].Value = ResultTB.Rows[0]["refnum"].ToString();
                                    //SiteDGV1.Rows[i].Cells[4].Value = BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text));
                                    try
                                    {
                                        // Detail
                                        Parameter[] parameters2 = new Parameter[2];
                                        parameters2[0] = new Parameter("@refnum", ResultTB.Rows[0]["refnum"].ToString());
                                        parameters2[1] = new Parameter("@sitenum", SiteDGV1.Rows[i].Cells[1].Value);

                                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_Insert_Pest_Tasks_Generate", ref parameters2);
                                    }
                                    catch (Exception ex)
                                    {
                                    }

                                }
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {




                        try
                        {
                            Parameter[] parameters3 = new Parameter[5];

                            //if (SiteDGV1.Rows[i].Cells[5].Value == "00023")
                            //{
                            //    string test = "";
                            //}
                            parameters3[0] = new Parameter("@refnum", SiteDGV1.Rows[i].Cells[3].Value);
                            parameters3[1] = new Parameter("@empnum", SiteDGV1.Rows[i].Cells[5].Value);

                            if (BizFunctions.IsEmpty(SiteDGV1.Rows[i].Cells[7].Value))
                            {
                                SiteDGV1.Rows[i].Cells[7].Value = "";
                            }
                            parameters3[2] = new Parameter("@empnum2", SiteDGV1.Rows[i].Cells[7].Value);

                            if (BizFunctions.IsEmpty(SiteDGV1.Rows[i].Cells[9].Value))
                            {
                                SiteDGV1.Rows[i].Cells[9].Value = "";
                            }
                            parameters3[3] = new Parameter("@empnum3", SiteDGV1.Rows[i].Cells[9].Value);

                            //parameters3[2] = new Parameter("@empnum2", SiteDGV1.Rows[i].Cells[7].Value);
                            //parameters3[3] = new Parameter("@empnum3", SiteDGV1.Rows[i].Cells[9].Value);
                            parameters3[4] = new Parameter("@actualempnum", SiteDGV1.Rows[i].Cells[7].Value);

                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_submit_Update_PWOR_Header_Generate", ref parameters3);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                }

            }

            OriTable = SiteListTb.Copy();

            if (DifferenceTable.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DifferenceTable.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //ReAssignedInfo = ReAssignedInfo + dr1["newnric"].ToString().Trim() +" as been re-assigned to "+dr1["newsite"].ToString().Trim()+" From "+dr1["orisite"].ToString().Trim()+"  on "+BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text))+". \n\n ";
                        ReAssignedInfo = ReAssignedInfo + dr1["newnric"].ToString().Trim() + " as been re-assigned to " + dr1["newsite"].ToString().Trim() + " on " + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + ". \n\n ";
                    }
                }

                SendEmail();
            }

        }

        private void GenerateWOR()
        {

            // Header

            //for (int i = 0; i < SiteDGV1.Rows.Count; i++)
            //{

            //    if (SiteDGV1.Rows[i].Cells[3].Value == "")
            //    {
            //        try
            //        {
            //Parameter[] parameters1 = new Parameter[5];
            //parameters1[0] = new Parameter("@sitenum", SiteDGV1.Rows[i].Cells[0].Value);
            //parameters1[1] = new Parameter("@sitenumt", SiteDGV1.Rows[i].Cells[2].Value);
            //parameters1[2] = new Parameter("@trandate", BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)));
            //parameters1[3] = new Parameter("@nric", SiteDGV1.Rows[i].Cells[5].Value);
            //parameters1[4] = new Parameter("@user", Common.DEFAULT_SYSTEM_USERNAME);

            //            DataSet ds_Ref = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("sp_submit_Insert_PWOR_Header_Generate", ref parameters1);
            //            ds_Ref.Tables[0].TableName = "RefnumTB";

            //            DataTable ResultTB = ds_Ref.Tables[0];

            //            if (ResultTB != null)
            //            {
            //                if (ResultTB.Rows.Count > 0)
            //                {
            //                    SiteDGV1.Rows[i].Cells[3].Value = ResultTB.Rows[0]["refnum"].ToString();
            //                    SiteDGV1.Rows[i].Cells[4].Value = BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text));
            //                    try
            //                    {
            //                        // Detail
            //                        Parameter[] parameters2 = new Parameter[2];
            //                        parameters2[0] = new Parameter("@refnum", ResultTB.Rows[0]["refnum"].ToString());
            //                        parameters2[1] = new Parameter("@sitenum", SiteDGV1.Rows[i].Cells[0].Value);

            //                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_Insert_Pest_Tasks_Generate", ref parameters2);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                    }
            //                }
            //            }

            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //    }
            //    else
            //    {
            //        string test = "";

            //        //sp_submit_Update_PWOR_Header_Generate


            //        try
            //        {
            //            Parameter[] parameters3 = new Parameter[2];
            //            parameters3[0] = new Parameter("@refnum", SiteDGV1.Rows[i].Cells[3].Value);
            //            parameters3[1] = new Parameter("@nric", SiteDGV1.Rows[i].Cells[5].Value);

            //            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_submit_Update_PWOR_Header_Generate", ref parameters3);
            //        }
            //        catch (Exception ex)
            //        {
            //        }

            //    }

            //}

     
        }

        private void SendEmail()
        {
     

            //Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            //Outlook._MailItem oMailItem = (Outlook._MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
            //oMailItem.Subject = "Pest Control Notification";
            //oMailItem.Body = "Dear \n\nPlease noted that the following Employee's has been Re-Assigned. \n\n " + ReAssignedInfo + " .\n\n Thank You & Regards,\n\n";
            //int iPosition = (int)oMailItem.Body.Length + 1;
            //int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
            //String sDisplayName = "MyAttachment";

            //#region If Marked         
            //oMailItem.To = "pestcontrol@atlmaintenance.com.sg";
            //#endregion
            //oMailItem.Display(true);
        }

    }
}