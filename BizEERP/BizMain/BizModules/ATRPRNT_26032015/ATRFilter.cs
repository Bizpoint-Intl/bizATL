
using System;
using System.Data;
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


using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

//using NodaTime;
//using ATL.BizModules.TextValidator;
//using System.Drawing.Imaging;
//using ATL.BizModules.StaCompressFolders;
//using ATL.BizModules.FileAcc2;
//using ATL.Network;
//using ATL.BizModules.RichTextEdit;


namespace ATL.BizModules.ATRPRNT
{
    public partial class ATRRfilter : Form
    {
        string projectPath = "";
        DBAccess dbAccess = null;
        protected Hashtable selectsCollection = null;
        protected ATL.BizModules.Tools.CRForm crpt1,crpt2 = null;

        public ATRRfilter()
        {
            InitializeComponent();
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
            this.selectsCollection = new Hashtable();
            this.dbAccess = new DBAccess();

 

            siteTextBox.KeyDown += new KeyEventHandler(siteTextBox_KeyDown);
            siteTextBox.DoubleClick += new EventHandler(siteTextBox_DoubleClick);

            siteTextBox.BackColor = Color.Yellow;

            yearComboBox1.DropDown += new EventHandler(yearComboBox1_DropDown);

            yearComboBox1.Text = Common.DEFAULT_SYSTEM_YEAR;



            monthComboBox1.Text = DateTime.Now.Month.ToString();

            sitenameTextBox1.Visible = false;
  
            
        }

        void yearComboBox1_DropDown(object sender, EventArgs e)
        {
            string sql1 = "select SystemYear from SysYear";
            this.dbAccess.ReadSQL("SysYear", sql1);
            DataRow drInsertSysYear = this.dbAccess.DataSet.Tables["SysYear"].NewRow();

            

            this.dbAccess.DataSet.Tables["SysYear"].Rows.Add(drInsertSysYear);

            yearComboBox1.DataSource = this.dbAccess.DataSet.Tables["SysYear"];
            yearComboBox1.DisplayMember = this.dbAccess.DataSet.Tables["SysYear"].Columns["SystemYear"].ColumnName.ToString();
            yearComboBox1.ValueMember = this.dbAccess.DataSet.Tables["SysYear"].Columns["SystemYear"].ColumnName.ToString();
        }

        void siteTextBox_DoubleClick(object sender, EventArgs e)
        {
            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum like '" + siteTextBox.Text.Trim() + "%' ", null, F2Type.Sort);

            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                siteTextBox.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();

            }
        }

        void siteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum like '" + siteTextBox.Text.Trim() + "%' ", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    siteTextBox.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();

                }

            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.crpt1 = new ATL.BizModules.Tools.CRForm();
            this.crpt2 = new ATL.BizModules.Tools.CRForm();

            string GetEmpDetails = "Select empnum,empname,datejoined,dateresigned from hemph";

            string EmployeeListStr = "SELECT DISTINCT EMPNUM FROM	"+	
		                            "( "+	
				                             "SELECT "+
					                             "ISNULL(C.shiftcode,'')+'/'+ISNULL(CONVERT(nvarchar(8),C.OTHrs),'0')+'/'+ISNULL(CONVERT(nvarchar(8),C.lateHrs),'0')+'/'+ISNULL(CONVERT(nvarchar(8),C.DailyAllow),'0') as detail, "+
					                             "*  "+
				                             "FROM  "+
				                             "(  "+
				                             "SELECT  "+
					                             "B.empnum,  "+
					                             "B.empname,  "+
					                             "B.[Date], "+
					                             "DAY(B.[Date]) as DayNo, "+
					                             "MONTH(B.[Date]) as MonthNo, "+
					                             "YEAR(B.[Date]) as YearNo, "+
					                             "[dbo].[GetWeekDayStr]([dbo].[GetWeekDay](B.[Date])) as [weekday], "+
					                             "B.sitenum, "+
					                             "B.sitename, "+
					                             "CASE WHEN ISNULL(B.isWorkShift,0)>0 THEN 'P' else  B.shiftcode END as shiftcode, "+
					                             "B.[timein], "+
					                             "B.[timeout], "+
					                             "B.ActualDateTimeIn, "+
					                             "B.ActualDateTimeOut, "+
					                             "CASE WHEN B.WorkHrs>0 THEN CONVERT(int,B.WorkHrs) ELSE 0 end as WorkHrs, "+
					                             "CASE WHEN B.OTHrs>0 THEN CONVERT(int,B.OTHrs) ELSE 0 end as OTHrs, "+
					                             "CASE WHEN B.lateHrs>0 THEN CONVERT(int,B.lateHrs) ELSE 0 end as lateHrs, "+
					                             "CASE WHEN B.DailyAllow>0 THEN CONVERT(int,B.DailyAllow) ELSE 0 end as DailyAllow, "+
					                             "B.isWorkShift "+
                            					
				                             "FROM "+
                            	
				                             "( "+
				                             "select  "+
					                             "al.empnum, "+
					                             "HP.empname, "+
					                             "al.[Date], "+
					                             "DAY(al.[Date]) as DayNo, "+
					                             "MONTH(al.[Date]) as MonthNo, "+
					                             "YEAR(al.[Date]) as YearNo, "+
					                             "[dbo].[GetWeekDayStr]([dbo].[GetWeekDay](al.[Date])) as weeday, "+
					                             "al.sitenum, "+
					                             "SM.sitename, "+
					                             "al.shiftcode, "+
					                             "al.[timein], "+
					                             "al.[timeout], "+
					                             "al.ActualDateTimeIn, "+
					                             "al.ActualDateTimeOut, "+
					                             "SUM(al.WorkHrs) AS WorkHrs, "+
					                             "SUM(ISNULL(al.OTHrs,0)) AS OTHrs, "+
					                             "SUM(ISNULL(al.latemins,0)) AS lateHrs, "+
					                             "SUM(ISNULL(al.FixAllowAmt,0)) AS DailyAllow, "+
					                             "SUM(ISNULL(vS.isWorkShift,0)) AS isWorkShift	 "+	
					                             "from ATMRLIVE al "+
						                             "LEFT JOIN HEMPH HP on al.empnum=HP.empnum "+
							                             "LEFT JOIN SITM SM on al.sitenum=SM.sitenum "+
								                             "LEFT JOIN vSHLV vS on al.shiftcode=vS.SHIFTCODE "+
                                                             "WHERE MONTH(al.[Date])=" + monthComboBox1.Text.Trim() + " and YEAR(al.[Date])="+yearComboBox1.Text.Trim()+" and al.sitenum='"+siteTextBox.Text.Trim()+"'  " + 
					                             "GROUP BY al.empnum,HP.empname,al.[Date],al.sitenum,SM.sitename,al.shiftcode,al.[timein],al.[timeout],al.ActualDateTimeIn,al.ActualDateTimeOut "+

				                             ")B "+
			                             ")C "+
		                             ")D";


            string EmployeeScheduleListStr = "SELECT " +
                                                 "ISNULL(C.shiftcode,'')+'/'+ISNULL(CONVERT(nvarchar(8),C.OTHrs),'0')+'/'+ISNULL(CONVERT(nvarchar(8),C.lateHrs),'0')+'/'+ISNULL(CONVERT(nvarchar(8),C.DailyAllow),'0') as detail, " +
                                                 "*  " +
                                             "FROM  " +
                                             "(  " +
                                             "SELECT  " +
                                                 "B.empnum,  " +
                                                 "B.empname,  " +
                                                 "B.datejoined, " +
                                                 "B.dateresigned, " +
                                                 "B.[Date], " +
                                                 "DAY(B.[Date]) as DayNo, " +
                                                 "MONTH(B.[Date]) as MonthNo, " +
                                                 "YEAR(B.[Date]) as YearNo, " +
                                                 "[dbo].[GetWeekDayStr]([dbo].[GetWeekDay](B.[Date])) as [weekday], " +
                                                 "B.sitenum, " +
                                                 "B.sitename, " +
                                                 "CASE WHEN ISNULL(B.isWorkShift,0)>0 THEN 'P' else  B.shiftcode END as shiftcode, " +
                                                 "B.[timein], " +
                                                 "B.[timeout], " +
                                                 "B.ActualDateTimeIn, " +
                                                 "B.ActualDateTimeOut, " +
                                                 "CASE WHEN B.WorkHrs>0 THEN CONVERT(int,B.WorkHrs) ELSE 0 end as WorkHrs, " +
                                                 "CASE WHEN B.OTHrs>0 THEN CONVERT(int,B.OTHrs) ELSE 0 end as OTHrs, " +
                                                 "CASE WHEN B.lateHrs>0 THEN CONVERT(int,B.lateHrs) ELSE 0 end as lateHrs, " +
                                                 "CASE WHEN B.DailyAllow>0 THEN CONVERT(int,B.DailyAllow) ELSE 0 end as DailyAllow, " +
                                                 "B.isWorkShift " +

                                             "FROM " +

                                             "( " +
                                             "select  " +
                                                 "al.empnum, " +
                                                 "HP.empname, " +
                                                 "HP.datejoined, " +
                                                 "HP.dateresigned, " +
                                                 "al.[Date], " +
                                                 "DAY(al.[Date]) as DayNo, " +
                                                 "MONTH(al.[Date]) as MonthNo, " +
                                                 "YEAR(al.[Date]) as YearNo, " +
                                                 "[dbo].[GetWeekDayStr]([dbo].[GetWeekDay](al.[Date])) as weeday, " +
                                                 "al.sitenum, " +
                                                 "SM.sitename, " +
                                                 "al.shiftcode, " +
                                                 "al.[timein], " +
                                                 "al.[timeout], " +
                                                 "al.ActualDateTimeIn, " +
                                                 "al.ActualDateTimeOut, " +
                                                 "SUM(al.WorkHrs) AS WorkHrs, " +
                                                 "SUM(ISNULL(al.OTHrs,0)) AS OTHrs, " +
                                                 "SUM(ISNULL(al.latemins,0)) AS lateHrs, " +
                                                 "SUM(ISNULL(al.FixAllowAmt,0)) AS DailyAllow, " +
                                                 "SUM(ISNULL(vS.isWorkShift,0)) AS isWorkShift	 " +
                                                 "from ATMRLIVE al " +
                                                     "LEFT JOIN HEMPH HP on al.empnum=HP.empnum " +
                                                         "LEFT JOIN SITM SM on al.sitenum=SM.sitenum " +
                                                             "LEFT JOIN vSHLV vS on al.shiftcode=vS.SHIFTCODE " +
                                                 "WHERE MONTH(al.[Date])=" + monthComboBox1.Text.Trim() + " and YEAR(al.[Date])=" + yearComboBox1.Text.Trim() + "  and al.sitenum='" + siteTextBox.Text.Trim() + "'   " +

                                                 "GROUP BY al.empnum,HP.empname,HP.datejoined,HP.dateresigned,al.[Date],al.sitenum,SM.sitename,al.shiftcode,al.[timein],al.[timeout],al.ActualDateTimeIn,al.ActualDateTimeOut " +

                                             ")B " +
                                         ")C ";


            this.dbAccess.ReadSQL("GetEmpDetails",GetEmpDetails);
            this.dbAccess.ReadSQL("EmployeeList", EmployeeListStr);
            this.dbAccess.ReadSQL("EmployeeScheduleList", EmployeeScheduleListStr);

            DataTable EmployeeList = this.dbAccess.DataSet.Tables["EmployeeList"];
            DataTable EmployeeScheduleList = this.dbAccess.DataSet.Tables["EmployeeScheduleList"];

            DataTable AttendanceTB1 = new DataTable("AttendanceTB1");

            AttendanceTB1.Columns.Add("empnum", typeof(string));
            AttendanceTB1.Columns.Add("empname", typeof(string));
            AttendanceTB1.Columns.Add("monthno", typeof(int));
            AttendanceTB1.Columns.Add("yearno", typeof(int));

            AttendanceTB1.Columns.Add("1", typeof(string));
            AttendanceTB1.Columns.Add("2", typeof(string));
            AttendanceTB1.Columns.Add("3", typeof(string));
            AttendanceTB1.Columns.Add("4", typeof(string));
            AttendanceTB1.Columns.Add("5", typeof(string));
            AttendanceTB1.Columns.Add("6", typeof(string));
            AttendanceTB1.Columns.Add("7", typeof(string));
            AttendanceTB1.Columns.Add("8", typeof(string));
            AttendanceTB1.Columns.Add("9", typeof(string));
            AttendanceTB1.Columns.Add("10", typeof(string));
            AttendanceTB1.Columns.Add("11", typeof(string));
            AttendanceTB1.Columns.Add("12", typeof(string));
            AttendanceTB1.Columns.Add("13", typeof(string));
            AttendanceTB1.Columns.Add("14", typeof(string));
            AttendanceTB1.Columns.Add("15", typeof(string));
            AttendanceTB1.Columns.Add("16", typeof(string));
            AttendanceTB1.Columns.Add("17", typeof(string));
            AttendanceTB1.Columns.Add("18", typeof(string));
            AttendanceTB1.Columns.Add("19", typeof(string));
            AttendanceTB1.Columns.Add("20", typeof(string));
            AttendanceTB1.Columns.Add("21", typeof(string));
            AttendanceTB1.Columns.Add("22", typeof(string));
            AttendanceTB1.Columns.Add("23", typeof(string));
            AttendanceTB1.Columns.Add("24", typeof(string));
            AttendanceTB1.Columns.Add("25", typeof(string));
            AttendanceTB1.Columns.Add("26", typeof(string));
            AttendanceTB1.Columns.Add("27", typeof(string));
            AttendanceTB1.Columns.Add("28", typeof(string));
            AttendanceTB1.Columns.Add("29", typeof(string));
            AttendanceTB1.Columns.Add("30", typeof(string));
            AttendanceTB1.Columns.Add("31", typeof(string));
            AttendanceTB1.Columns.Add("datejoined", typeof(DateTime));
            AttendanceTB1.Columns.Add("dateresigned", typeof(DateTime));


            DataTable AttendanceH1 = new DataTable("AttendanceH1");

            AttendanceH1.Columns.Add("sitenum", typeof(string));
            AttendanceH1.Columns.Add("sitename", typeof(string));
            AttendanceH1.Columns.Add("month", typeof(string));
            AttendanceH1.Columns.Add("year", typeof(string));

            DataRow InsertAttendanceH1 = AttendanceH1.NewRow();
            InsertAttendanceH1["sitenum"] = siteTextBox.Text.Trim();
            InsertAttendanceH1["sitename"] = getSitename(siteTextBox.Text.Trim());
            InsertAttendanceH1["month"] = monthComboBox1.Text.Trim();
            InsertAttendanceH1["year"] = yearComboBox1.Text.Trim(); 

            AttendanceH1.Rows.Add(InsertAttendanceH1);


            

            if(EmployeeList.Rows.Count > 0)
            {
                foreach(DataRow dr1 in EmployeeList.Rows)
                {
                    DataRow insertAttendanceTB1 = AttendanceTB1.NewRow();
                    insertAttendanceTB1["empnum"] = dr1["empnum"];
                    insertAttendanceTB1["empname"] = GetEmpDetail(dr1["empnum"].ToString().Trim(), "empname");
                    insertAttendanceTB1["yearno"] = yearComboBox1.Text.Trim();
                    insertAttendanceTB1["monthno"] = monthComboBox1.Text.Trim();
                    if (EmployeeScheduleList.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in EmployeeScheduleList.Rows)
                        {
                            if (dr1["empnum"].ToString().Trim() == dr2["empnum"].ToString().Trim())
                            {
                                string test1 = dr1["empnum"].ToString().Trim();
                                string test2 = dr2["detail"].ToString().Trim();
                                
                                for (int i = 1; i <= 31; i++)
                                {
                                    string test3 = i.ToString().Trim();
                                    string test4 = dr2["dayno"].ToString().Trim();

                                    if (i.ToString().Trim() == dr2["dayno"].ToString().Trim())
                                    {

                                        insertAttendanceTB1[i.ToString().Trim()] = dr2["detail"];

                                    }
                                    //else
                                    //{
                                    //    insertAttendanceTB1[i.ToString().Trim()] = "///";
                                    //}
                                }
                            }
                        }
                        AttendanceTB1.Rows.Add(insertAttendanceTB1);
                    }

                    //foreach (DataRow dr2 in EmployeeScheduleList.Rows)
                    //{
                    //    string empnum = dr1["empnum"].ToString();

                    //    if (dr1["empnum"].ToString().Trim() == dr2["empnum"].ToString().Trim())
                    //    {
                    //        DataRow insertAttendanceTB1 = AttendanceTB1.NewRow();
                    //        insertAttendanceTB1["empnum"] = dr1["empnum"];
                    //        insertAttendanceTB1["yearno"] = dr2["yearno"];
                    //        insertAttendanceTB1["monthno"] = dr2["monthno"];
                    //        for (int i = 0; i < 30; i++)
                    //        {
                    //            if (i.ToString() == dr2["dayno"].ToString())
                    //            {
                    //                insertAttendanceTB1[i.ToString()] = dr2["detail"].ToString();
                    //            }
                    //        }
                    //        AttendanceTB1.Rows.Add(insertAttendanceTB1);
                    //    }
                    //}
                }

                if (AttendanceTB1.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in AttendanceTB1.Rows)
                    {
                       
                        if (BizFunctions.IsEmpty(dr2["datejoined"]))
                        {
                            

                            if (GetEmpDetail(dr2["empnum"].ToString(), "datejoined") != "")
                            {
                                DateTime dt1 = Convert.ToDateTime(GetEmpDetail(dr2["empnum"].ToString(), "datejoined"));
                                dr2["datejoined"] = dt1;
                            }
                        }
                        if (BizFunctions.IsEmpty(dr2["dateresigned"]))
                        {
                            if (GetEmpDetail(dr2["empnum"].ToString(), "dateresigned") != "")
                            {
                                DateTime dt1 = Convert.ToDateTime(GetEmpDetail(dr2["empnum"].ToString(), "dateresigned"));
                                dr2["dateresigned"] = dt1;
                            }
                        }
                        for (int i = 1; i <= 31; i++)
                        {
                            if (BizFunctions.IsEmpty(dr2[i]) || dr2[i].ToString() == string.Empty)
                            {
                                dr2[i] = "///";
                            }
                        }

                       
                        
                    }
                }
            }

            //siteMaterials = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from tra1 where sitenum='" + dr1["sitenum"].ToString() + "'");

            //siteMaterials.TableName = "siteMaterials";

            //if (this.dbAccess.DataSet.Tables.Contains("siteMaterials"))
            //{
            //    this.dbAccess.DataSet.Tables["siteMaterials"].Dispose();
            //    this.dbAccess.DataSet.Tables.Remove("siteMaterials");
            //    this.dbAccess.DataSet.Tables.Add(siteMaterials);
            //}
            //else
            //{
            //    this.dbAccess.DataSet.Tables.Add(siteMaterials);
            //}


            if (this.dbAccess.DataSet.Tables.Contains("AttendanceTB1"))
            {
                this.dbAccess.DataSet.Tables["AttendanceTB1"].Dispose();
                this.dbAccess.DataSet.Tables.Remove("AttendanceTB1");
                this.dbAccess.DataSet.Tables.Add(AttendanceTB1);
            }
            else
            {
                this.dbAccess.DataSet.Tables.Add(AttendanceTB1);
            }

            if (this.dbAccess.DataSet.Tables.Contains("AttendanceH1"))
            {
                this.dbAccess.DataSet.Tables["AttendanceH1"].Dispose();
                this.dbAccess.DataSet.Tables.Remove("AttendanceH1");
                this.dbAccess.DataSet.Tables.Add(AttendanceH1);
            }
            else
            {
                this.dbAccess.DataSet.Tables.Add(AttendanceH1);
            }


            if (allRD1.Checked == true)
            {

                ReportDocument crReportDocument1 = new ReportDocument();
                crReportDocument1.Load(this.projectPath + @"\ATRPRNT\AttendanceReport1_1.rpt");

                crReportDocument1.SetDataSource(this.dbAccess.DataSet);

                crpt1.Crv1.ReportSource = crReportDocument1;
                crpt1.ShowDialog();

                ReportDocument crReportDocument2 = new ReportDocument();
                crReportDocument2.Load(this.projectPath + @"\ATRPRNT\AttendanceReport1_2.rpt");

                crReportDocument2.SetDataSource(this.dbAccess.DataSet);

                crpt2.Crv1.ReportSource = crReportDocument2;
                crpt2.ShowDialog();
            }

            else if (firstRD1.Checked == true)
            {
                ReportDocument crReportDocument1 = new ReportDocument();
                crReportDocument1.Load(this.projectPath + @"\ATRPRNT\AttendanceReport1_1.rpt");

                crReportDocument1.SetDataSource(this.dbAccess.DataSet);

                crpt1.Crv1.ReportSource = crReportDocument1;
                crpt1.ShowDialog();
            }

            else if (secondRD1.Checked == true)
            {
                ReportDocument crReportDocument2 = new ReportDocument();
                crReportDocument2.Load(this.projectPath + @"\ATRPRNT\AttendanceReport1_2.rpt");

                crReportDocument2.SetDataSource(this.dbAccess.DataSet);

                crpt2.Crv1.ReportSource = crReportDocument2;
                crpt2.ShowDialog();
            }
        }

        private string GetEmpDetail(string empnum, string returnColumn)
        {
            string value = "";

            string selectQuery = "Select " + returnColumn + " from GetEmpDetails where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, selectQuery);

            if (dt1.Rows.Count > 0)
            {
                value = dt1.Rows[0][returnColumn].ToString();
            }


            return value;
        }

        private string getSitename(string sitenum)
        {
            string str = "Select sitename from sitm where sitenum='"+sitenum+"'";
            string result = "";

            this.dbAccess.ReadSQL("tmpSitm",str);

            DataTable dt1 = this.dbAccess.DataSet.Tables["tmpSitm"];

            if(dt1.Rows.Count  > 0)
            {
                result = dt1.Rows[0]["sitename"].ToString();
            }
            return result;
        }
    }
}