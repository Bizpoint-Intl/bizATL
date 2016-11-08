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

using MAXVALUE.BizModules.TimeSync;
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
using MAXVALUE.TimeUtilites;
using MAXVALUE.BizModules.TextValidator;

namespace MAXVALUE.BizModules.ATR.TimeControlForm1
{
    public partial class TimeControlForm1 : Form
    {

        #region local variables

        protected DBAccess dbaccess = null;
        NTPClient client;
        string TimeServer, nric, empnum, uniquekey, shiftcode, docunum;
        bool normalflag, CheckedIn, CheckedOut, isConfirmed, ClockINbtnClicked, ClockOUTbtnClicked, yesterday, today, isReplaced;
        bool isAdhoc = false;
        DataTable ATMRdatasource, xATMRdatasource, yATMRdatasource = null;
        protected Hashtable selectsCollection = null;
        protected bool opened = false;

        #endregion
        public TimeControlForm1()
        {
            this.dbaccess = new DBAccess();
            InitializeComponent();
            SiteNameTb.ReadOnly = true;
            WRRrefNoTb.ReadOnly = true;
            SectorTb.ReadOnly = true;
            EmpNameTb.ReadOnly = true;
            GetHemph();
            GetVshlv();
            GetSITMI();
            SiteCodeTb.KeyDown +=new KeyEventHandler(SiteCodeTb_KeyDown);
            SiteCodeTb.Leave +=new EventHandler(SiteCodeTb_Leave);
            EmpTb.KeyDown +=new KeyEventHandler(EmpTb_KeyDown);
            EmpTb.Leave +=new EventHandler(EmpTb_Leave);
            ATMRdg1.CellDoubleClick +=new DataGridViewCellEventHandler(ATMRdg1_CellDoubleClick);
            //ATMRdg1.CellFormatting += new DataGridViewCellFormattingEventHandler(ATMRdg1_CellFormatting);

            ATMRdg1.CellValueChanged +=new DataGridViewCellEventHandler(ATMRdg1_CellValueChanged);
            //ATMRdg1.KeyDown +=new KeyEventHandler(ATMRdg1_KeyDown);

            

            DateTime cdt = Convert.ToDateTime("#17:20:35#");
            //Some custom start time for the clock.
            DateTime utcDt = DateTime.UtcNow;
            //The current UTC dateTime. 
            //This is needed because the clock internal works with UTC dateTime.
            this.Clock1.UtcOffset = new TimeSpan(0, cdt.Hour - utcDt.Hour, cdt.Minute - utcDt.Minute, cdt.Second - utcDt.Second);


            //Set UTC offset to the system utc offset when the application loads
            this.Clock1.UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

            
            
        }

        private void SiteCodeTb_Leave(object sender, EventArgs e)
        {
            if (!BizFunctions.IsEmpty(SiteCodeTb.Text) || SiteCodeTb.Text != string.Empty)
            {
                getvSITMI();
                GetWRRInfo();

                EmpTb.Text = string.Empty;
                EmpNameTb.Text = string.Empty;
            }
        }

        private void EmpTb_Leave(object sender, EventArgs e)
        {
            if (!BizFunctions.IsEmpty(EmpTb.Text) || EmpTb.Text != string.Empty)
            {

                SiteCodeTb.Text = string.Empty;
                SectorTb.Text = string.Empty;
                WRRrefNoTb.Text = string.Empty;
                EmpNameTb.Text = string.Empty;
                GetEmpScheduleDetails();
                //GetEMPInfo();
                GetWRRInfo();

                

                if (ATMRdatasource.Rows.Count > 0)
                {                    
                    for (int i = 0; i < ATMRdatasource.Rows.Count; i++)
                    {
                        if (ATMRdatasource.Rows[i].RowState != DataRowState.Deleted)
                        {
                            if (!BizFunctions.IsEmpty(ATMRdatasource.Rows[i]["Empnum"]))
                            {
                                if (ATMRdatasource.Rows[i]["Empnum"].ToString() == EmpTb.Text)
                                {
                                    this.ATMRdg1.Rows[i].DefaultCellStyle.BackColor = Color.Gold;
                                }
                            }
                        }
                    }
                }


            }
        }

        private void GetEmpScheduleDetails()
        {
            string GetEmpSchedule = "select * from v_ATMRhemph2 where empnum='" + EmpTb.Text + "' and  [date] = '" + TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dateTimePicker1.Text)) + "' ";

            this.dbaccess.ReadSQL("GetEmpSchedule", GetEmpSchedule);

            if (this.dbaccess.DataSet.Tables["GetEmpSchedule"].Rows.Count > 0)
            {
                DataRow dr1 = this.dbaccess.DataSet.Tables["GetEmpSchedule"].Rows[0];
                EmpTb.Text =  dr1["empnum"].ToString();
                SiteCodeTb.Text = dr1["sitenum"].ToString();
                SiteNameTb.Text = GetSiteName(dr1["sitenum"].ToString());
                SectorTb.Text = dr1["sectorcode"].ToString();
                WRRrefNoTb.Text = dr1["refnum"].ToString();
                EmpTb.Text = dr1["empnum"].ToString();
                EmpNameTb.Text = dr1["empname"].ToString();

            }
            else
            {
                string GetEmpSchedule2 = "select * from v_ATMRhemph3 where empnum='" + EmpTb.Text + "' and  [date] = '" + TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dateTimePicker1.Text)) + "' ";

                this.dbaccess.ReadSQL("GetEmpSchedule2", GetEmpSchedule2);

                if (this.dbaccess.DataSet.Tables["GetEmpSchedule2"].Rows.Count > 0)
                {

                    DataRow dr1 = this.dbaccess.DataSet.Tables["GetEmpSchedule2"].Rows[0];
                    SiteCodeTb.Text = dr1["sitenum"].ToString();
                    SectorTb.Text = dr1["sectorcode"].ToString();
                    WRRrefNoTb.Text = dr1["refnum"].ToString();
                    EmpTb.Text = dr1["empnum"].ToString();
                    EmpNameTb.Text = dr1["empname"].ToString();
                }

            }
        }

        private void ATMRdg1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        
        }

        private void ATMRdatasource_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            

            switch (e.Column.ColumnName)
            {
                case "ActualTimeIn":
                    if (!BizFunctions.IsEmpty(e.Row["ActualTimeIn"]))
                    {
                        if (TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["ActualTimeIn"].ToString().Trim()))
                        {
                            e.Row["ActualDateTimeIn"] = TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dateTimePicker1.Text));
                        }
                        else
                        {
                            MessageBox.Show("Invalid Time In Format");
                            e.Row["Clock In"] = 0;
                            

                        }
                    }
                    break;

                case "ActualTimeOut":
                    if (!BizFunctions.IsEmpty(e.Row["ActualTimeOut"]))
                    {
                        if (TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["ActualTimeOut"].ToString().Trim()))
                        {
                            e.Row["ActualDateTimeOut"] = TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dateTimePicker1.Text));
                        }
                        else
                        {
                            MessageBox.Show("Invalid Time Out Format");
                            e.Row["Clock Out"] = 0;
                            
                        }
                    }
                    break;

                //case "Rep Empno":
                //    if (!BizFunctions.IsEmpty(e.Row["Rep Empno"]))
                //    {

                //        if (isValidEmpno(e.Row["Rep Empno"].ToString()))
                //        {
                //            DataRow InsertATMRtmp = this.dbaccess.DataSet.Tables["ATMRtmp"].NewRow();

                //            //////////// Add columns to insert
                //            InsertATMRtmp["empnum"] = e.Row["Rep Empno"].ToString();
                //            InsertATMRtmp["Name"] = getEmpName(e.Row["Rep Empno"].ToString());
                //            InsertATMRtmp["shiftcode"] = e.Row["shiftcode"].ToString();
                //            InsertATMRtmp["timein"] = e.Row["timein"].ToString();
                //            InsertATMRtmp["timeout"] = e.Row["timeout"].ToString();
                //            InsertATMRtmp["Date"] = e.Row["Date"].ToString();
                //            this.dbaccess.DataSet.Tables["ATMRtmp"].Rows.Add(InsertATMRtmp);

                //            e.Row["Refkey"] = e.Row["Rep Empno"].ToString() + '-' + e.Row["shiftcode"].ToString() + '-' + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text));
                //        }
                //        else
                //        {
                //            e.Row["Rep Empno"] = string.Empty;
                //            e.Row["Rep Name"] = string.Empty;
                //            e.Row["RefKey"] = string.Empty;
                //        }

                //    }
                //    else
                //    {
                //        e.Row["Rep Name"] = string.Empty;
                //        e.Row["RefKey"] = string.Empty;
                //    }
                //    break;
                  
                case "empnum":
                    if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                    {                      
                        if (isValidEmpno(e.Row["empnum"].ToString()))
                        {
                            e.Row["Name"] = getEmpName(e.Row["empnum"].ToString());                          
                        }
                        else
                        {
                            e.Row["empnum"] = string.Empty;
                            e.Row["Name"] = string.Empty;                          
                        }
                    }
                    else
                    {
                        e.Row["Name"] = string.Empty;
                    }
                    break;

                    ///////////

                case "shiftcode":
                    if (!BizFunctions.IsEmpty(e.Row["shiftcode"]))
                    {
                        e.Row["timein"] = geTimeIn(e.Row["shiftcode"].ToString());
                        e.Row["timeout"] = geTimeOut(e.Row["shiftcode"].ToString());
                    }
                    else
                    {
                        e.Row["timein"] = string.Empty;
                        e.Row["timeout"] = string.Empty;
                    }
                    break;

                case "Clock In":
                    if (!BizFunctions.IsEmpty(e.Row["Clock In"]))
                    {
                        
                            if ((bool)e.Row["Clock In"])
                            {
                                if (BizFunctions.IsEmpty(e.Row["ActualTimeIn"]) || e.Row["ActualTimeIn"].ToString() == string.Empty)
                                {
                                    e.Row["ActualTimeIn"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(DateTime.Now);
                                }
                            }
                            
                            else
                            {
                                e.Row["ActualTimeIn"] = System.DBNull.Value;
                                e.Row["ActualDateTimeIn"] = System.DBNull.Value;
                            }
                        
                    }
                    else
                    {
                      e.Row["ActualTimeIn"] = System.DBNull.Value;
                      e.Row["ActualDateTimeIn"] = System.DBNull.Value;
                    }
                    break;

                case "Clock Out":
                    if (!BizFunctions.IsEmpty(e.Row["Clock Out"]))
                    {
                       
                            if ((bool)e.Row["Clock Out"])
                            {
                                if (BizFunctions.IsEmpty(e.Row["ActualTimeOut"]) || e.Row["ActualTimeOut"].ToString() == string.Empty)
                                {
                                     e.Row["ActualTimeOut"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(DateTime.Now);
                                }
                            }
                            else
                            {
                                e.Row["ActualTimeOut"] = System.DBNull.Value;
                                e.Row["ActualDateTimeOut"] = System.DBNull.Value;
                            }
                        //}
                    }
                    else
                    {
                         e.Row["ActualTimeOut"] = System.DBNull.Value;
                         e.Row["ActualDateTimeOut"] = System.DBNull.Value;
                    }
                    break;




            }
        }

        private bool isValidEmpno(string empno)
        {
            bool valid = false;

            string GetEmpno = "Select empnum from HEMPH where empnum='" + empno.ToUpper().Trim() + "'";

            DataTable HemphTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetEmpno);

            if (HemphTmp.Rows.Count > 0)
            {
                valid = true;
            }

            return valid;
        }


        private string getEmpName(string empno)
        {
            string EmpName="";

            string GetEmpno = "Select empname from HEMPH where empnum='" + empno + "'";

            DataTable HemphTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetEmpno);

            if (HemphTmp.Rows.Count > 0)
            {
                EmpName = HemphTmp.Rows[0]["empname"].ToString();
            }

            return EmpName;
        }

        private string getNric(string empno)
        {
            string nric = "";

            string GetEmpno = "Select nric from HEMPH where empnum='" + empno + "'";

            DataTable HemphTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetEmpno);

            if (HemphTmp.Rows.Count > 0)
            {
                nric = HemphTmp.Rows[0]["nric"].ToString();
            }

            return nric;
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

        private void getvSITMI()
        {
            string GetvSITMI = "SELECT * FROM vSITMI where sitenum ='" + SiteCodeTb.Text.Trim() + "' and commencedate <= CONVERT(DateTime, '" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "') and enddate >= CONVERT(DateTime, '" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "')";

            this.dbaccess.ReadSQL("vSITMI", GetvSITMI);

            if (this.dbaccess.DataSet.Tables["vSITMI"].Rows.Count > 0)
            {
                DataRow dr1 = this.dbaccess.DataSet.Tables["vSITMI"].Rows[0];
                SiteCodeTb.Text = dr1["Sitenumi"].ToString();
                SiteNameTb.Text = dr1["SiteName"].ToString();
                SectorTb.Text = dr1["SectorCode"].ToString();
                WRRrefNoTb.Text = dr1["refnum"].ToString();
            }
        }



        void ATMRdg1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            if (e.RowIndex >= 0 && e.ColumnIndex == this.ATMRdg1.Columns["RealSchedule"].Index)
            {
                if (e.Value != null)
                {
                    if (!(bool)e.Value)
                    {

                        //string CNumColour = e.Value.ToString();



                        //if (CNumColour == "Pool" || CNumColour == "Fair")
                        //{

                        this.ATMRdg1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;

                        //}
                    }
                }

            }

        }
 


     

        private string GetSectorcode(string sitenum)
        {
            string sectorcode = "";
            string get = "Select sectorcode from sitm where sitenum='" + sitenum + "'";
            this.dbaccess.ReadSQL("GetSectorcode", get);
            if (this.dbaccess.DataSet.Tables["GetSectorcode"].Rows.Count > 0)
            {
                DataRow dr1 = this.dbaccess.DataSet.Tables["GetSectorcode"].Rows[0];
                sectorcode = dr1["sectorcode"].ToString();
            }
            this.dbaccess.DataSet.Tables["GetSectorcode"].Dispose();
            return sectorcode;

        }

      

        protected void SiteCodeTb_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_vSITMI.xml", e, "sitenum", "sitenum like '" + SiteCodeTb.Text.Trim() + "%' and commencedate <= CONVERT(DateTime, '" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "') and enddate >= CONVERT(DateTime, '" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "')", null, F2Type.Sort);
  
                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    SiteCodeTb.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
                    SiteNameTb.Text = f2BaseHelper.F2Base.CurrentRow["sitename"].ToString();
                    WRRrefNoTb.Text = f2BaseHelper.F2Base.CurrentRow["refnum"].ToString();
                    SectorTb.Text = f2BaseHelper.F2Base.CurrentRow["sectorcode"].ToString();

                   

                    if (SiteCodeTb.Text != string.Empty && WRRrefNoTb.Text != String.Empty)
                    {
                        GetWRRInfo();
                    }

                }
              
            }
        }

        private void GetHemph()
        {
            string GetHemph ="Select refnum,nric,empnum,empname,matnum,sectorcode from hemph";

            this.dbaccess.ReadSQL("HEMPH",GetHemph);
        }

        private void GetVshlv()
        {

            string GetvSHLV = "Select * from vSHLV";

            this.dbaccess.ReadSQL("vSHLV", GetvSHLV);
        }

        protected void EmpTb_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_vATMRHEMPH.xml", e, "empnum", "empnum like '" + EmpTb.Text + "%' and  [date] = '" + TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dateTimePicker1.Text)) + "' ", null, F2Type.Sort);

                

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {

                    EmpTb.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                    SiteCodeTb.Text = string.Empty;
                    SectorTb.Text = string.Empty;
                    WRRrefNoTb.Text = string.Empty;
                    EmpNameTb.Text = string.Empty;
                    GetEmpScheduleDetails();
                    GetWRRInfo();

                    if (ATMRdatasource.Rows.Count > 0)
                    {
                        for (int i = 0; i < ATMRdatasource.Rows.Count; i++)
                        {
                            if (ATMRdatasource.Rows[i].RowState != DataRowState.Deleted)
                            {
                                if (!BizFunctions.IsEmpty(ATMRdatasource.Rows[i]["Empnum"]))
                                {
                                    if (ATMRdatasource.Rows[i]["Empnum"].ToString() == EmpTb.Text)
                                    {
                                        this.ATMRdg1.Rows[i].DefaultCellStyle.BackColor = Color.Gold;
                                    }
                                }
                            }
                        }
                    }




                    //EmpTb.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                    //EmpNameTb.Text = f2BaseHelper.F2Base.CurrentRow["empname"].ToString();
                    //SiteCodeTb.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
                    ////SiteNameTb.Text = f2BaseHelper.F2Base.CurrentRow["sitename"].ToString();
                    //WRRrefNoTb.Text = f2BaseHelper.F2Base.CurrentRow["refnum"].ToString();
                    //SectorTb.Text = GetSectorcode(f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString());                  
                    //if (EmpTb.Text != string.Empty && SiteCodeTb.Text != string.Empty && WRRrefNoTb.Text != String.Empty)
                    //{
                    //    GetEMPInfo();
                       
                    //}

                }

            }
        }
        private void GetEMPInfo()
        {


            string GetATMR = "SELECT * FROM " +
                                "( " +
                                "Select ISNULL(markdelete,0) as Mark, a.ID, a.nric,a.rnric,REPLACE(a.rempname,char(39),'') as rempname,a.[day],a.Refkey, a.empnum,REPLACE(h.empname,char(39),'') as Name,a.shiftcode,a.[timein],a.ClockInMark as [Clock In],a.ActualDateTimeIn,a.actualtimein2 as ActualTimeIn,a.[timeout],a.ClockOutMark as [Clock Out], a.ActualDateTimeOut,a.actualtimeout2 as ActualTimeOut,ISNULL(a.isreplaced,0) as [Is Replaced],a.rempnum as [Rep Empno],REPLACE(a.rempname,char(39),'')  as [Rep Name],ISNULL(a.isAdhoc,0) as Adhoc, ISNULL(a.isRealSchedule,0) as [RealSchedule], " +
                                "ISNULL(a.isOT,0) as [OT], ISNULL(a.OTrate,0) as [OTrate],ISNULL(a.isDRE,0) as [DRE],ISNULL(a.isOffset,0) as [OS], ISNULL(a.isUS,0) AS [US], ISNULL(a.isRD,0) as [RD],SUBSTRING(a.shiftcode,1,1) as shifttype,a.[Date],a.remark,a.uniquekey from ATMR a LEFT JOIN HEMPH h on a.empnum=h.empnum " +
                                "where a.refnum='" + WRRrefNoTb.Text.Trim() + "' and a.sitenum='" + SiteCodeTb.Text.Trim() + "' and a.[date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' and a.empnum='" + EmpTb.Text + "' " +
                                ")A " +
                                "ORDER BY [RealSchedule]  DESC,shifttype,empnum";

            string GetATMRLive = "SELECT * FROM " +
                                "( " +
                                "Select ISNULL(markdelete,0) as Mark, a.ID, a.nric,a.rnric,REPLACE(a.rempname,char(39),'') as rempname,a.[day],a.Refkey, a.empnum,REPLACE(h.empname,char(39),'') as Name,a.shiftcode,a.[timein],a.ClockInMark as [Clock In],a.ActualDateTimeIn,a.actualtimein2 as ActualTimeIn,a.[timeout],a.ClockOutMark as [Clock Out], a.ActualDateTimeOut,a.actualtimeout2 as ActualTimeOut,ISNULL(a.isreplaced,0) as [Is Replaced],a.rempnum as [Rep Empno],REPLACE(a.rempname,char(39),'') as [Rep Name],ISNULL(a.isAdhoc,0) as Adhoc, ISNULL(a.isRealSchedule,0) as [RealSchedule], " +
                                "ISNULL(a.isOT,0) as [OT],ISNULL(a.OTrate,0) as [OTrate],ISNULL(a.isDRE,0) as [DRE],ISNULL(a.isOffset,0) as [OS], ISNULL(a.isUS,0) AS [US], ISNULL(a.isRD,0) as [RD],SUBSTRING(a.shiftcode,1,1) as shifttype,a.[Date],a.remark,a.uniquekey from ATMRLive a LEFT JOIN HEMPH h on a.empnum=h.empnum " +
                                //"where a.refnum='" + WRRrefNoTb.Text.Trim() + "' and a.sitenum='" + SiteCodeTb.Text.Trim() + "' and a.[day]='" + TimeTools.GetDay(TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)))) + "' and a.empnum='" + EmpTb.Text + "' " +
                                "where a.refnum='" + WRRrefNoTb.Text.Trim() + "' and a.sitenum='" + SiteCodeTb.Text.Trim() + "' and a.[date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' and a.empnum='" + EmpTb.Text + "' " +
                                ")A " +
                                "ORDER BY [RealSchedule],shifttype,empnum DESC";

            if (this.dbaccess.DataSet.Tables.Contains("ATMRtmp"))
            {
                this.dbaccess.DataSet.Tables["ATMRtmp"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("ATMRtmp");


            }

            if (this.dbaccess.DataSet.Tables.Contains("ATMRLiveTmp"))
            {
                this.dbaccess.DataSet.Tables["ATMRLiveTmp"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("ATMRLiveTmp");
            }

            this.dbaccess.ReadSQL("ATMRtmp", GetATMR);
            this.dbaccess.ReadSQL("ATMRLiveTmp", GetATMRLive);

            if (this.dbaccess.DataSet.Tables["ATMRtmp"].Rows.Count > 0)
            {
                foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["ATMRtmp"].Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (this.dbaccess.DataSet.Tables["ATMRLiveTmp"].Rows.Count > 0)
                        {
                            foreach (DataRow dr2 in this.dbaccess.DataSet.Tables["ATMRLiveTmp"].Rows)
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    if (!BizFunctions.IsEmpty(dr2["uniquekey"]))
                                    {
                                        if (dr1["uniquekey"].ToString() == dr2["uniquekey"].ToString())
                                        {

                                            dr1["actualtimein"] = dr2["actualtimein"];
                                            dr1["actualtimeout"] = dr2["actualtimeout"];
                                            dr1["shiftcode"] = dr2["shiftcode"];

                                            dr1["timein"] = dr2["timein"];
                                            dr1["timeout"] = dr2["timeout"];
                                            dr1["nric"] = dr2["nric"];
                                            dr1["rnric"] = dr2["rnric"];
                                            dr1["rempname"] = dr2["rempname"];
                                            dr1["Refkey"] = dr2["Refkey"];
                                            dr1["shiftcode"] = dr2["shiftcode"];
                                            dr1["ActualDateTimeIn"] = dr2["ActualDateTimeIn"];
                                            dr1["ActualDateTimeOut"] = dr2["ActualDateTimeOut"];
                                            dr1["ActualTimeIn"] = dr2["ActualTimein"];
                                            dr1["ActualTimeOut"] = dr2["ActualTimeOut"];
                                            dr1["Is Replaced"] = dr1["Is Replaced"];
                                            dr1["Rep Empno"] = dr1["Rep Empno"];
                                            dr1["Rep Name"] = dr1["Rep Name"];
                                            dr1["Adhoc"] = dr1["Adhoc"];
                                            dr1["RealSchedule"] = dr1["RealSchedule"];
                                            dr1["OT"] = dr1["OT"];
                                            dr1["OTrate"] = dr1["OTrate"];
                                            dr1["DRE"] = dr1["DRE"];
                                            dr1["OS"] = dr1["OS"];
                                            dr1["US"] = dr1["US"];
                                            dr1["RD"] = dr1["RD"];
                                            dr1["shifttype"] = dr1["shifttype"];
                                            dr1["Date"] = dr1["Date"];
                                            dr1["remark"] = dr1["remark"];



                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            
            string GetATMRLiveOther = "SELECT * FROM " +
                               "( " +
                               "Select ISNULL(markdelete,0) as Mark, a.ID, a.nric,a.rnric,a.rempname,a.[day],a.Refkey, a.empnum,h.empname as Name,a.shiftcode,a.[timein],a.ClockInMark as [Clock In],a.ActualDateTimeIn,a.actualtimein2 as ActualTimeIn,a.[timeout],a.ClockOutMark as [Clock Out], a.ActualDateTimeOut,a.actualtimeout2 as ActualTimeOut,ISNULL(a.isreplaced,0) as [Is Replaced],a.rempnum as [Rep Empno],a.rempname as [Rep Name],ISNULL(a.isAdhoc,0) as Adhoc, ISNULL(a.isRealSchedule,0) as [RealSchedule], " +
                               "ISNULL(a.isOT,0) as [OT],ISNULL(a.OTrate,0) as [OTrate],ISNULL(a.isDRE,0) as [DRE],ISNULL(a.isOffset,0) as [OS], ISNULL(a.isUS,0) AS [US], ISNULL(a.isRD,0) as [RD],SUBSTRING(a.shiftcode,1,1) as shifttype,a.[Date],a.remark,a.uniquekey from ATMRLive a LEFT JOIN HEMPH h on a.empnum=h.empnum " +
                               "where a.refnum='" + WRRrefNoTb.Text + "' and a.sitenum='" + SiteCodeTb.Text + "' and a.[date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' and a.empnum='"+EmpTb.Text+"'  " +
                               "and a.uniquekey not in (Select uniquekey from ATMR where refnum='" + WRRrefNoTb.Text + "' and sitenum='" + SiteCodeTb.Text + "' and [date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "'  and a.empnum='" + EmpTb.Text + "' ) " +
                               ")A " +
                               "ORDER BY [RealSchedule],shifttype,empnum DESC";


         
            this.dbaccess.ReadSQL("ATMRLiveTmpOther", GetATMRLiveOther);

       

            if (this.dbaccess.DataSet.Tables["ATMRLiveTmpOther"].Rows.Count > 0)
            {
                foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["ATMRLiveTmpOther"].Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertATMRtmp = this.dbaccess.DataSet.Tables["ATMRtmp"].NewRow();

                        //////////// Add columns to insert
                        InsertATMRtmp["uniquekey"] = dr1["uniquekey"].ToString();
                        InsertATMRtmp["empnum"] = dr1["empnum"].ToString();
                        InsertATMRtmp["Name"] = dr1["Name"].ToString();
                        InsertATMRtmp["shiftcode"] = dr1["shiftcode"].ToString();
                        InsertATMRtmp["timein"] = dr1["timein"].ToString();
                        InsertATMRtmp["timeout"] = dr1["timeout"].ToString();
                        InsertATMRtmp["Date"] = dr1["Date"].ToString();

                        InsertATMRtmp["ActualDateTimeIn"] = dr1["ActualDateTimeIn"];
                        InsertATMRtmp["ActualTimeIn"] = dr1["ActualTimeIn"].ToString();
                        InsertATMRtmp["ActualDateTimeOut"] = dr1["ActualDateTimeOut"];
                        InsertATMRtmp["ActualTimeOut"] = dr1["ActualTimeOut"].ToString();

                        InsertATMRtmp["Clock In"] = dr1["Clock In"];
                        InsertATMRtmp["Clock Out"] = dr1["Clock In"];

                        InsertATMRtmp["Is Replaced"] = dr1["Is Replaced"].ToString();
                        InsertATMRtmp["Rep Name"] = dr1["Rep Name"].ToString();

                        InsertATMRtmp["Adhoc"] = dr1["Adhoc"].ToString();
                        InsertATMRtmp["RealSchedule"] = dr1["RealSchedule"].ToString();
                        InsertATMRtmp["nric"] = dr1["nric"].ToString();

                        InsertATMRtmp["Rep Empno"] = dr1["Rep Empno"].ToString();
                        InsertATMRtmp["rnric"] = dr1["rnric"].ToString();
                        InsertATMRtmp["rempname"] = dr1["rempname"].ToString();

                        InsertATMRtmp["Refkey"] = dr1["Refkey"].ToString();

                        InsertATMRtmp["OT"] = dr1["OT"].ToString();
                        InsertATMRtmp["OTrate"] = dr1["OTrate"].ToString();
                        InsertATMRtmp["DRE"] = dr1["DRE"].ToString();
                        InsertATMRtmp["OS"] = dr1["OS"].ToString();
                        InsertATMRtmp["US"] = dr1["US"].ToString();
                        InsertATMRtmp["RD"] = dr1["RD"].ToString();
                        InsertATMRtmp["shifttype"] = dr1["shifttype"].ToString();
                        InsertATMRtmp["remark"] = dr1["remark"].ToString();


                        this.dbaccess.DataSet.Tables["ATMRtmp"].Rows.Add(InsertATMRtmp);

                    }
                }
            }

            ///////////////////////////////////////


            //string GetATMRLiveEmpOther = "SELECT * FROM " +
            //                   "( " +
            //                   "Select ISNULL(markdelete,0) as Mark, a.ID, a.nric,a.rnric,a.rempname,a.[day],a.Refkey, a.empnum,h.empname as Name,a.shiftcode,a.[timein],a.ClockInMark as [Clock In],a.ActualDateTimeIn,a.actualtimein2 as ActualTimeIn,a.[timeout],a.ClockOutMark as [Clock Out], a.ActualDateTimeOut,a.actualtimeout2 as ActualTimeOut,ISNULL(a.isreplaced,0) as [Is Replaced],a.rempnum as [Rep Empno],a.rempname as [Rep Name],ISNULL(a.isAdhoc,0) as Adhoc, ISNULL(a.isRealSchedule,0) as [RealSchedule], " +
            //                   "ISNULL(a.isOT,0) as [OT],ISNULL(a.OTrate,0) as [OTrate],ISNULL(a.isDRE,0) as [DRE], ISNULL(a.isUS,0) AS [US], ISNULL(a.isRD,0) as [RD],SUBSTRING(a.shiftcode,1,1) as shifttype,a.[Date],a.remark,a.uniquekey from ATMRLive a LEFT JOIN HEMPH h on a.empnum=h.empnum " +
            //                   "where a.refnum='" + WRRrefNoTb.Text + "' and a.sitenum='" + SiteCodeTb.Text + "' and a.[date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' and empnum='" + EmpTb.Text + "' " +
            //                   "and a.uniquekey not in (Select uniquekey from ATMR where refnum='" + WRRrefNoTb.Text + "' and sitenum='" + SiteCodeTb.Text + "' and [date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' and empnum='"+ EmpTb.Text +"' ) " +
            //                   ")A " +
            //                   "ORDER BY [RealSchedule],shifttype,empnum DESC";



            //this.dbaccess.ReadSQL("ATMRLiveTmpOther", GetATMRLiveOther);



            //if (this.dbaccess.DataSet.Tables["ATMRLiveTmpOther"].Rows.Count > 0)
            //{
            //    foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["ATMRLiveTmpOther"].Rows)
            //    {
            //        if (dr1.RowState != DataRowState.Deleted)
            //        {
            //            DataRow InsertATMRtmp = this.dbaccess.DataSet.Tables["ATMRtmp"].NewRow();

            //            //////////// Add columns to insert
            //            InsertATMRtmp["uniquekey"] = dr1["uniquekey"].ToString();
            //            InsertATMRtmp["empnum"] = dr1["empnum"].ToString();
            //            InsertATMRtmp["Name"] = dr1["Name"].ToString();
            //            InsertATMRtmp["shiftcode"] = dr1["shiftcode"].ToString();
            //            InsertATMRtmp["timein"] = dr1["timein"].ToString();
            //            InsertATMRtmp["timeout"] = dr1["timeout"].ToString();
            //            InsertATMRtmp["Date"] = dr1["Date"].ToString();

            //            InsertATMRtmp["ActualDateTimeIn"] = dr1["ActualDateTimeIn"];
            //            InsertATMRtmp["ActualTimeIn"] = dr1["ActualTimeIn"].ToString();
            //            InsertATMRtmp["ActualDateTimeOut"] = dr1["ActualDateTimeOut"];
            //            InsertATMRtmp["ActualTimeOut"] = dr1["ActualTimeOut"].ToString();

            //            InsertATMRtmp["Clock In"] = dr1["Clock In"];
            //            InsertATMRtmp["Clock Out"] = dr1["Clock In"];

            //            InsertATMRtmp["Is Replaced"] = dr1["Is Replaced"].ToString();
            //            InsertATMRtmp["Rep Name"] = dr1["Rep Name"].ToString();

            //            InsertATMRtmp["Adhoc"] = dr1["Adhoc"].ToString();
            //            InsertATMRtmp["RealSchedule"] = dr1["RealSchedule"].ToString();
            //            InsertATMRtmp["nric"] = dr1["nric"].ToString();

            //            InsertATMRtmp["Rep Empno"] = dr1["Rep Empno"].ToString();
            //            InsertATMRtmp["rnric"] = dr1["rnric"].ToString();
            //            InsertATMRtmp["rempname"] = dr1["rempname"].ToString();

            //            InsertATMRtmp["Refkey"] = dr1["Refkey"].ToString();

            //            InsertATMRtmp["OT"] = dr1["OT"].ToString();
            //            InsertATMRtmp["OTrate"] = dr1["OTrate"].ToString();
            //            InsertATMRtmp["DRE"] = dr1["DRE"].ToString();
            //            InsertATMRtmp["US"] = dr1["US"].ToString();
            //            InsertATMRtmp["RD"] = dr1["RD"].ToString();
            //            InsertATMRtmp["shifttype"] = dr1["shifttype"].ToString();
            //            InsertATMRtmp["remark"] = dr1["remark"].ToString();


            //            this.dbaccess.DataSet.Tables["ATMRtmp"].Rows.Add(InsertATMRtmp);

            //        }
            //    }
            //}

            ///////////////////////////////////////


            BindingSource nbindingSource = new BindingSource();

            ATMRdatasource = this.dbaccess.DataSet.Tables["ATMRtmp"];


            ATMRdatasource.ColumnChanged += new DataColumnChangeEventHandler(ATMRdatasource_ColumnChanged);
            nbindingSource.DataSource = ATMRdatasource;


            ATMRdg1.DataSource = nbindingSource;



            ATMRdg1.Columns["empnum"].DefaultCellStyle.BackColor = Color.Yellow;
            ATMRdg1.Columns["shiftcode"].DefaultCellStyle.BackColor = Color.Yellow;
            ATMRdg1.Columns["Rep Empno"].DefaultCellStyle.BackColor = Color.Yellow;
            ATMRdg1.Columns["actualtimein"].DefaultCellStyle.BackColor = Color.Orange;
            ATMRdg1.Columns["actualtimeout"].DefaultCellStyle.BackColor = Color.Orange;
            ATMRdg1.Columns["timein"].DefaultCellStyle.BackColor = Color.LightGreen;
            ATMRdg1.Columns["timeout"].DefaultCellStyle.BackColor = Color.LightGreen;
            ATMRdg1.Columns["Name"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Rep Name"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Date"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Uniquekey"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Name"].ReadOnly = true;
            ATMRdg1.Columns["Date"].ReadOnly = true;
            ATMRdg1.Columns["uniquekey"].ReadOnly = true;
            ATMRdg1.Columns["Rep Name"].ReadOnly = true;
            ATMRdg1.Columns["refkey"].ReadOnly = true;
            ATMRdg1.Columns["OT"].HeaderText = "OT2.0";
            ATMRdg1.Columns["OS"].HeaderText = "OFFSET";

            //Invisible Colums

            ATMRdg1.Columns["nric"].Visible = false;
            ATMRdg1.Columns["day"].Visible = false;
            ATMRdg1.Columns["uniquekey"].Visible = false;
            ATMRdg1.Columns["refkey"].Visible = false;
            ATMRdg1.Columns["rnric"].Visible = false;
            ATMRdg1.Columns["rempname"].Visible = false;
            ATMRdg1.Columns["ID"].Visible = false;

            ATMRdg1.Columns["OTrate"].Visible = false;



            //////////////////

            ATMRdg1.Columns["ActualDateTimeIn"].Visible = false;
            ATMRdg1.Columns["ActualDateTimeoUT"].Visible = false;

            DataGridViewColumn Mark = ATMRdg1.Columns["Mark"];
            Mark.Width = 40;

            DataGridViewColumn ClockIn = ATMRdg1.Columns["Clock In"];
            Mark.Width = 40;

            DataGridViewColumn ClockOut = ATMRdg1.Columns["Clock Out"];
            Mark.Width = 40;

            DataGridViewColumn OS = ATMRdg1.Columns["OS"];
            OS.Width = 50;

            if (ATMRdatasource.Rows.Count > 0)
            {
                StatusLB.Text = string.Empty;

            }
            else
            {
                StatusLB.Text = "Invalid Employee Number / Site No";
                EmpTb.Text = string.Empty;
                EmpNameTb.Text = string.Empty;
                SiteCodeTb.Text = string.Empty;
                SiteNameTb.Text = string.Empty;
                WRRrefNoTb.Text = string.Empty;
            }
        }

        private void GetWRRInfo()
        {
            DataTable WRR1 = this.dbaccess.DataSet.Tables["WRR1"];
            DataTable WRR2 = this.dbaccess.DataSet.Tables["WRR2"];
            DataTable WRR3 = this.dbaccess.DataSet.Tables["WRR3"];
            DataTable WRR4 = this.dbaccess.DataSet.Tables["WRR4"];

            if (ATMRdatasource != null)
            {
                if (ATMRdatasource.Rows.Count > 0)
                {
                    ATMRdatasource.Clear();
                }
            }

            if (WRR1 != null)
            {
                if (WRR1.Rows.Count > 0)
                {
                    WRR1.Clear();
                }
            }

            if (WRR2 != null)
            {
                if (WRR2.Rows.Count > 0)
                {
                    WRR2.Clear();
                }
            }

            if (WRR3 != null)
            {
                if (WRR3.Rows.Count > 0)
                {
                    WRR3.Clear();
                }
            }

            if (WRR4 != null)
            {
                if (WRR4.Rows.Count > 0)
                {
                    WRR4.Clear();
                }
            }

            
            //    BizFunctions.DeleteAllRows(ATMRdatasource);

            //    BindingSource nbindingSource2 = new BindingSource();

            //    ATMRdatasource = this.dbaccess.DataSet.Tables["ATMRtmp"];


              
            //    nbindingSource2.DataSource = ATMRdatasource;


            //    ATMRdg1.DataSource = nbindingSource2;   
    
            
           
            string GetATMR = "SELECT * FROM "+
                                "( "+
                                "Select ISNULL(markdelete,0) as Mark, a.ID, a.nric,a.rnric,REPLACE(a.rempname,char(39),'') as rempname,a.[day],a.Refkey, a.empnum,REPLACE(h.empname,char(39),'') as Name,a.shiftcode,a.[timein],ISNULL(a.ClockInMark,0) as [Clock In],a.ActualDateTimeIn,a.actualtimein2 as ActualTimeIn,a.[timeout],ISNULL(a.ClockOutMark,0) as [Clock Out], a.ActualDateTimeOut,a.actualtimeout2 as ActualTimeOut,ISNULL(a.isreplaced,0) as [Is Replaced],a.rempnum as [Rep Empno],REPLACE(a.rempname,char(39),'')  as [Rep Name],ISNULL(a.isAdhoc,0) as Adhoc, ISNULL(a.isRealSchedule,0) as [RealSchedule], " +
                                "ISNULL(a.isOT,0) as [OT], ISNULL(a.OTrate,0) as [OTrate],ISNULL(a.isDRE,0) as [DRE],ISNULL(a.isOffset,0) as [OS], ISNULL(a.isUS,0) AS [US], ISNULL(a.isRD,0) as [RD],ISNULL(a.isTR,0) as [TR],'' as [Site TR],SUBSTRING(a.shiftcode,1,1) as shifttype,a.[Date],a.remark,a.uniquekey from ATMR a LEFT JOIN HEMPH h on a.empnum=h.empnum " +
                                "where a.refnum='" + WRRrefNoTb.Text.Trim() + "' and a.sitenum='" + SiteCodeTb.Text.Trim() + "' and a.[date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' " +
                                ")A "+
                                "ORDER BY [RealSchedule]  DESC,shifttype,empnum";

            string GetATMRLive = "SELECT * FROM " +
                                "( " +
                                "Select ISNULL(markdelete,0) as Mark, a.ID, a.nric,a.rnric,REPLACE(a.rempname,char(39),'') as rempname,a.[day],a.Refkey, a.empnum,REPLACE(h.empname,char(39),'') as Name,a.shiftcode,a.[timein],ISNULL(a.ClockInMark,0) as [Clock In],a.ActualDateTimeIn,a.actualtimein2 as ActualTimeIn,a.[timeout],ISNULL(a.ClockOutMark,0) as [Clock Out], a.ActualDateTimeOut,a.actualtimeout2 as ActualTimeOut,ISNULL(a.isreplaced,0) as [Is Replaced],a.rempnum as [Rep Empno],REPLACE(a.rempname,char(39),'') as [Rep Name],ISNULL(a.isAdhoc,0) as Adhoc, ISNULL(a.isRealSchedule,0) as [RealSchedule], " +
                                "ISNULL(a.isOT,0) as [OT],ISNULL(a.OTrate,0) as [OTrate],ISNULL(a.isDRE,0) as [DRE],ISNULL(a.isOffset,0) as [OS], ISNULL(a.isUS,0) AS [US], ISNULL(a.isRD,0) as [RD], ISNULL(a.isTR,0) as [TR],a.TRsitenum as [Site TR], SUBSTRING(a.shiftcode,1,1) as shifttype,a.[Date],a.remark,a.uniquekey from ATMRLive a LEFT JOIN HEMPH h on a.empnum=h.empnum " +
                                //"where a.refnum='" + WRRrefNoTb.Text.Trim() + "' and a.sitenum='" + SiteCodeTb.Text.Trim() + "' and a.[day]='" + TimeTools.GetDay(TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)))) + "' " +
                                "where a.refnum='" + WRRrefNoTb.Text.Trim() + "' and a.sitenum='" + SiteCodeTb.Text.Trim() + "' and a.[date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' " +
                                ")A " +
                                "ORDER BY [RealSchedule],shifttype,empnum DESC";

            //if(this.dbaccess.DataSet.Tables.Contains("ATMRtmp"))
            //{
            //    this.dbaccess.DataSet.Tables["ATMRtmp"].Dispose();
            //    this.dbaccess.DataSet.Tables.Remove("ATMRtmp");
                

            //}

            //if (this.dbaccess.DataSet.Tables.Contains("ATMRLiveTmp"))
            //{
            //    this.dbaccess.DataSet.Tables["ATMRLiveTmp"].Dispose();
            //    this.dbaccess.DataSet.Tables.Remove("ATMRLiveTmp");
            //}
         
            dbaccess.ReadSQL("ATMRtmp", GetATMR);
            dbaccess.ReadSQL("ATMRLiveTmp", GetATMRLive);
            

            if(this.dbaccess.DataSet.Tables["ATMRtmp"].Rows.Count > 0)
            {
                foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["ATMRtmp"].Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["uniquekey"]) || dr1["uniquekey"].ToString() == string.Empty)
                        {
                            dr1["uniquekey"] = dr1["empnum"].ToString() + "-" + dr1["shiftcode"].ToString() + "-" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["Date"]));
                        }
                        if (this.dbaccess.DataSet.Tables["ATMRLiveTmp"].Rows.Count > 0)
                        {
                            foreach (DataRow dr2 in this.dbaccess.DataSet.Tables["ATMRLiveTmp"].Rows)
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    if (!BizFunctions.IsEmpty(dr2["uniquekey"]))
                                    {
                                        if (dr1["uniquekey"].ToString() == dr2["uniquekey"].ToString())
                                        {
                                            dr1["Clock In"] = dr2["Clock In"];
                                            dr1["Clock Out"] = dr2["Clock Out"];
                                            dr1["actualtimein"] = dr2["actualtimein"];
                                            dr1["actualtimeout"] = dr2["actualtimeout"];
                                            dr1["shiftcode"] = dr2["shiftcode"];

                                            dr1["timein"] = dr2["timein"];
                                            dr1["timeout"] = dr2["timeout"];
                                            dr1["nric"] = dr2["nric"];
                                            dr1["rnric"] = dr2["rnric"];
                                            dr1["rempname"] = dr2["rempname"];
                                            dr1["Refkey"] = dr2["Refkey"];
                                            dr1["shiftcode"] = dr2["shiftcode"];
                                           

                                            if (!BizFunctions.IsEmpty(dr2["ActualDateTimeIn"]))
                                            {
                                                dr1["ActualDateTimeIn"] = TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dr2["ActualDateTimeIn"]));
                                            }
                                            if (!BizFunctions.IsEmpty(dr2["ActualDateTimeOut"]))
                                            {
                                                dr1["ActualDateTimeOut"] = TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dr2["ActualDateTimeOut"]));
                                            }
                       

                                     
                                            dr1["ActualTimeIn"] = dr2["ActualTimein"];
                                            dr1["ActualTimeOut"] = dr2["ActualTimeOut"];
                                            dr1["Is Replaced"] = dr2["Is Replaced"];
                                            dr1["Rep Empno"] = dr2["Rep Empno"];
                                            dr1["Rep Name"] = dr2["Rep Name"];
                                            dr1["Adhoc"] = dr2["Adhoc"];
                                            dr1["RealSchedule"] = dr1["RealSchedule"];
                                            dr1["OT"] = dr2["OT"];
                                            dr1["OTrate"] = dr2["OTrate"];
                                            dr1["DRE"] = dr2["DRE"];
                                            dr1["OS"] = dr2["OS"];
                                            dr1["US"] = dr2["US"];
                                            dr1["RD"] = dr2["RD"];
                                            dr1["TR"] = dr2["TR"];
                                            dr1["Site TR"] = dr2["Site TR"];
                                            dr1["shifttype"] = dr1["shifttype"];
                                            dr1["Date"] = dr2["Date"];
                                            dr1["remark"] = dr2["remark"];

                                            




                                        }
                                    }
                                }
                            }
                        }
                        //else
                        //{
                        //    DataRow InsertDr1 = 
                        //}
                    }
                }
            }

            string GetATMRLiveOther = "SELECT * FROM " +
                               "( " +
                               "Select ISNULL(markdelete,0) as Mark, a.ID, a.nric,a.rnric,a.rempname,a.[day],a.Refkey, a.empnum,h.empname as Name,a.shiftcode,a.[timein],ISNULL(a.ClockInMark,0) as [Clock In],a.ActualDateTimeIn,a.actualtimein2 as ActualTimeIn,a.[timeout],ISNULL(a.ClockOutMark,0) as [Clock Out], a.ActualDateTimeOut,a.actualtimeout2 as ActualTimeOut,ISNULL(a.isreplaced,0) as [Is Replaced],a.rempnum as [Rep Empno],a.rempname as [Rep Name],ISNULL(a.isAdhoc,0) as Adhoc, ISNULL(a.isRealSchedule,0) as [RealSchedule], " +
                               "ISNULL(a.isOT,0) as [OT],ISNULL(a.OTrate,0) as [OTrate],ISNULL(a.isDRE,0) as [DRE],ISNULL(a.isOffset,0) as [OS],  ISNULL(a.isUS,0) AS [US], ISNULL(a.isRD,0) as [RD], ISNULL(a.isTR,0) as [TR],a.TRsitenum as [Site TR],SUBSTRING(a.shiftcode,1,1) as shifttype,a.[Date],a.remark,a.uniquekey from ATMRLive a LEFT JOIN HEMPH h on a.empnum=h.empnum " +
                               "where a.refnum='" + WRRrefNoTb.Text + "' and a.sitenum='" + SiteCodeTb.Text + "' and a.[date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' " +
                               "and a.uniquekey not in (Select uniquekey from ATMR where refnum='" + WRRrefNoTb.Text + "' and sitenum='" + SiteCodeTb.Text + "' and [date]='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "' ) " +
                               ")A " +
                               "ORDER BY [RealSchedule],shifttype,empnum DESC";


         
            this.dbaccess.ReadSQL("ATMRLiveTmpOther", GetATMRLiveOther);

       

            if (this.dbaccess.DataSet.Tables["ATMRLiveTmpOther"].Rows.Count > 0)
            {
                foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["ATMRLiveTmpOther"].Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertATMRtmp = this.dbaccess.DataSet.Tables["ATMRtmp"].NewRow();

                        //////////// Add columns to insert
                        InsertATMRtmp["uniquekey"] = dr1["uniquekey"].ToString();
                        InsertATMRtmp["empnum"] = dr1["empnum"].ToString();
                        InsertATMRtmp["Name"] = dr1["Name"].ToString();
                        InsertATMRtmp["shiftcode"] = dr1["shiftcode"].ToString();
                        InsertATMRtmp["timein"] = dr1["timein"].ToString();
                        InsertATMRtmp["timeout"] = dr1["timeout"].ToString();
                        InsertATMRtmp["Date"] = dr1["Date"].ToString();

                        if (!BizFunctions.IsEmpty(dr1["ActualDateTimeIn"]))
                        {
                            InsertATMRtmp["ActualDateTimeIn"] = TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dr1["ActualDateTimeIn"]));
                        }
                        if (!BizFunctions.IsEmpty(dr1["ActualDateTimeOut"]))
                        {
                            InsertATMRtmp["ActualDateTimeOut"] = TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dr1["ActualDateTimeOut"]));
                        }
                       
                        
                    
                        InsertATMRtmp["ActualTimeIn"] = dr1["ActualTimeIn"].ToString();
          
                        InsertATMRtmp["ActualTimeOut"] = dr1["ActualTimeOut"].ToString();

                        InsertATMRtmp["Clock In"] = dr1["Clock In"];
                        InsertATMRtmp["Clock Out"] = dr1["Clock In"];

                        InsertATMRtmp["Is Replaced"] = dr1["Is Replaced"].ToString();
                        InsertATMRtmp["Rep Name"] = dr1["Rep Name"].ToString();

                        InsertATMRtmp["Adhoc"] = dr1["Adhoc"].ToString();
                        InsertATMRtmp["RealSchedule"] = dr1["RealSchedule"].ToString();
                        InsertATMRtmp["nric"] = dr1["nric"].ToString();

                        InsertATMRtmp["Rep Empno"] = dr1["Rep Empno"].ToString();
                        InsertATMRtmp["rnric"] = dr1["rnric"].ToString();
                        InsertATMRtmp["rempname"] = dr1["rempname"].ToString();

                        InsertATMRtmp["Refkey"] = dr1["Refkey"].ToString();

                        InsertATMRtmp["OT"] = dr1["OT"];
                        InsertATMRtmp["OTrate"] = dr1["OTrate"].ToString();
                        InsertATMRtmp["DRE"] = dr1["DRE"];
                        InsertATMRtmp["OS"] = dr1["OS"];
                        InsertATMRtmp["US"] = dr1["US"];
                        InsertATMRtmp["RD"] = dr1["RD"];
                        InsertATMRtmp["TR"] = dr1["TR"];
                        InsertATMRtmp["Site TR"] = dr1["Site TR"];
                        InsertATMRtmp["shifttype"] = dr1["shifttype"];
                        InsertATMRtmp["remark"] = dr1["remark"];

                        InsertATMRtmp["Clock In"] = dr1["Clock In"];
                        InsertATMRtmp["Clock Out"] = dr1["Clock Out"];


                        this.dbaccess.DataSet.Tables["ATMRtmp"].Rows.Add(InsertATMRtmp);

                    }
                }
            }




            BindingSource nbindingSource = new BindingSource();

            ATMRdatasource = this.dbaccess.DataSet.Tables["ATMRtmp"];

            
            ATMRdatasource.ColumnChanged += new DataColumnChangeEventHandler(ATMRdatasource_ColumnChanged);
            nbindingSource.DataSource = ATMRdatasource;


            ATMRdg1.DataSource = nbindingSource;

            

            ATMRdg1.Columns["empnum"].DefaultCellStyle.BackColor = Color.Yellow;
            ATMRdg1.Columns["shiftcode"].DefaultCellStyle.BackColor = Color.Yellow;
            ATMRdg1.Columns["Rep Empno"].DefaultCellStyle.BackColor = Color.Yellow;
            ATMRdg1.Columns["Site TR"].DefaultCellStyle.BackColor = Color.Yellow;
            ATMRdg1.Columns["actualtimein"].DefaultCellStyle.BackColor = Color.Orange;
            ATMRdg1.Columns["actualtimeout"].DefaultCellStyle.BackColor = Color.Orange;
            ATMRdg1.Columns["timein"].DefaultCellStyle.BackColor = Color.LightGreen;
            ATMRdg1.Columns["timeout"].DefaultCellStyle.BackColor = Color.LightGreen;
            ATMRdg1.Columns["Name"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Rep Name"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Date"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Uniquekey"].DefaultCellStyle.BackColor = Color.LightBlue;
            ATMRdg1.Columns["Name"].ReadOnly = true;
            ATMRdg1.Columns["Date"].ReadOnly = true;
            ATMRdg1.Columns["uniquekey"].ReadOnly = true;
            ATMRdg1.Columns["Rep Name"].ReadOnly = true;
            ATMRdg1.Columns["refkey"].ReadOnly = true;
            ATMRdg1.Columns["OT"].HeaderText = "OT2.0";
            ATMRdg1.Columns["OS"].HeaderText = "OFFSET";
            

            //Invisible Colums

            ATMRdg1.Columns["nric"].Visible = false;
            ATMRdg1.Columns["day"].Visible = false;
            ATMRdg1.Columns["uniquekey"].Visible = false;
            ATMRdg1.Columns["refkey"].Visible = false;
            ATMRdg1.Columns["rnric"].Visible = false;
            ATMRdg1.Columns["Rep Empno"].Visible = false;
            ATMRdg1.Columns["Rep Name"].Visible = false;
            ATMRdg1.Columns["rempname"].Visible = false;
            ATMRdg1.Columns["ID"].Visible = false;
            ATMRdg1.Columns["RealSchedule"].Visible = false;
             ATMRdg1.Columns["OTrate"].Visible = false;
             ATMRdg1.Columns["shifttype"].Visible = false;

       
          
            //////////////////

            //ATMRdg1.Columns["ActualDateTimeIn"].Visible = false;
            //ATMRdg1.Columns["ActualDateTimeoUT"].Visible = false;

            DataGridViewColumn Mark = ATMRdg1.Columns["Mark"];
            Mark.Width = 40;

            DataGridViewColumn OS = ATMRdg1.Columns["OS"];
            OS.Width = 50;


            DataGridViewColumn ClockIn = ATMRdg1.Columns["Clock In"];
            ClockIn.Width = 60;

            DataGridViewColumn ClockOut = ATMRdg1.Columns["Clock Out"];
            ClockOut.Width = 60;

            //////////////////////////////////////////////////////////

            DataGridViewColumn OT = ATMRdg1.Columns["OT"];
            OT.Width = 40;

            DataGridViewColumn DRE = ATMRdg1.Columns["DRE"];
            DRE.Width = 40;

            DataGridViewColumn US = ATMRdg1.Columns["US"];
            US.Width = 40;

            DataGridViewColumn RD = ATMRdg1.Columns["RD"];
            RD.Width = 40;

            DataGridViewColumn isRep = ATMRdg1.Columns["Is Replaced"];
            isRep.Width = 80;

            DataGridViewColumn isTR = ATMRdg1.Columns["TR"];
            isTR.Width = 40;

            DataGridViewColumn SiteTR = ATMRdg1.Columns["Site TR"];
            SiteTR.Width = 80;

            //

            DataGridViewColumn EMPNUM = ATMRdg1.Columns["empnum"];
            EMPNUM.Width = 60;

            DataGridViewColumn NAME = ATMRdg1.Columns["NAME"];
            NAME.Width = 150;

            DataGridViewColumn SHIFTCODE = ATMRdg1.Columns["shiftcode"];
            SHIFTCODE.Width = 75;

            DataGridViewColumn TIMEIN = ATMRdg1.Columns["timein"];
            TIMEIN.Width = 75;

            DataGridViewColumn ATIMEIN = ATMRdg1.Columns["actualtimein"];
            ATIMEIN.Width = 80;

            DataGridViewColumn TIMEOUT = ATMRdg1.Columns["timeout"];
            TIMEOUT.Width = 75;


            DataGridViewColumn ATIMEOUT = ATMRdg1.Columns["actualtimeout"];
            ATIMEOUT.Width = 80;

            DataGridViewColumn ADHOC = ATMRdg1.Columns["adhoc"];
            ADHOC.Width = 55;


            DataGridViewColumn REMARK = ATMRdg1.Columns["remark"];
            REMARK.Width = 200;

            //

 

            if (ATMRdatasource.Rows.Count > 0)
            {
                StatusLB.Text = string.Empty;

                for (int i = 0; i < ATMRdatasource.Rows.Count; i++)
                {
                    if (ATMRdatasource.Rows[i].RowState != DataRowState.Deleted)
                    {
                        if(BizFunctions.IsEmpty(ATMRdatasource.Rows[i]["RealSchedule"]))
                        {
                            ATMRdatasource.Rows[i]["RealSchedule"] = 0;
                        }
                        if (!(bool)ATMRdatasource.Rows[i]["RealSchedule"])
                        {
                            this.ATMRdg1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                        }
                        if ((BizFunctions.IsEmpty(ATMRdatasource.Rows[i]["TimeIn"]) && BizFunctions.IsEmpty(ATMRdatasource.Rows[i]["TimeOut"])) || (ATMRdatasource.Rows[i]["TimeIn"].ToString() != string.Empty && ATMRdatasource.Rows[i]["TimeOut"].ToString() == string.Empty))
                        {
                            this.ATMRdg1.Rows[i].DefaultCellStyle.ForeColor = Color.DarkGreen;
                        }
                    }
                }
                

              
            }
            else
            {
                StatusLB.Text = "Invalid Site Number / Employee No";
                SiteCodeTb.Text = string.Empty;
                SiteNameTb.Text = string.Empty;
                WRRrefNoTb.Text = string.Empty;

                EmpTb.Text = string.Empty;
                EmpNameTb.Text = string.Empty;
            }

            if (WRRrefNoTb.Text != string.Empty)
            {
                GetRoster();
            }
            
        }

        

        private void ATMRdg1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            

            if (e.RowIndex >= 0 && e.ColumnIndex == this.ATMRdg1.Columns[e.ColumnIndex].Index)
            {
                if (e.ColumnIndex == 7)
                {
                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + Convert.ToString(ATMRdg1.CurrentRow.Cells["empnum"].Value) + "%' ", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {

                        ATMRdg1.Rows[e.RowIndex].Cells["empnum"].Value = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                        ATMRdg1.Rows[e.RowIndex].Cells["name"].Value = f2BaseHelper.F2Base.CurrentRow["empname"].ToString();
                        ATMRdg1.Rows[e.RowIndex].Cells["nric"].Value = f2BaseHelper.F2Base.CurrentRow["nric"].ToString();

                        ATMRdg1.Refresh();
                        //ATMRdg1.CurrentRow.Cells["shiftcode"] = s;
                        //ATMRdg1.CurrentRow.Cells["timein"]
                        //WRRrefNoTb.Text = f2BaseHelper.F2Base.CurrentRow["refnum"].ToString();



                    }

                }
                if (e.ColumnIndex == 9)
                {
            
                    
                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_vSHLV.xml", e, "shiftcode", "shiftcode like '"+Convert.ToString(ATMRdg1.CurrentRow.Cells["shiftcode"].Value)+"%' ", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {
                       
                        ATMRdg1.Rows[e.RowIndex].Cells["shiftcode"].Value = f2BaseHelper.F2Base.CurrentRow["shiftcode"].ToString();
                        ATMRdg1.Rows[e.RowIndex].Cells["timein"].Value = f2BaseHelper.F2Base.CurrentRow["timein"].ToString();
                        ATMRdg1.Rows[e.RowIndex].Cells["timeout"].Value = f2BaseHelper.F2Base.CurrentRow["timeout"].ToString();

                        ATMRdg1.Refresh();

                    }
                }

                if (e.ColumnIndex == 19)
                {
                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + Convert.ToString(ATMRdg1.CurrentRow.Cells["Rep Empno"].Value) + "%' ", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {

                        ATMRdg1.Rows[e.RowIndex].Cells["Rep Empno"].Value = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                        ATMRdg1.Rows[e.RowIndex].Cells["Rep Name"].Value = f2BaseHelper.F2Base.CurrentRow["empname"].ToString();

                        ATMRdg1.Refresh();
            
                    }

                }

                if (e.ColumnIndex == 29)
                {
                    String TEST = Convert.ToString(ATMRdg1.CurrentRow.Cells["Site TR"].Value);
                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum like '" + Convert.ToString(ATMRdg1.CurrentRow.Cells["Site TR"].Value) + "%' ", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {

                        ATMRdg1.Rows[e.RowIndex].Cells["Site TR"].Value = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
                       

                        ATMRdg1.Refresh();
                        //ATMRdg1.CurrentRow.Cells["shiftcode"] = s;
                        //ATMRdg1.CurrentRow.Cells["timein"]
                        //WRRrefNoTb.Text = f2BaseHelper.F2Base.CurrentRow["refnum"].ToString();



                    }

                }
                
            }
        }

        private void confirmbtn_Click(object sender, EventArgs e)
        {
            string UpdateRow = "";
            string InsertRow = "";
            DataTable xATMRdatasource = ATMRdatasource.GetChanges(DataRowState.Deleted);
            bool TimeOutEmpty = false;
            if (ATMRdatasource.Rows.Count > 0)
            {
                GetSchedule();
                foreach (DataRow dr1 in ATMRdatasource.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["actualtimein"]) && (Convert.ToInt16(dr1["Clock In"]) == 0 || BizFunctions.IsEmpty(dr1["Clock In"])))
                        {
                            if (TextValidator.TextValidator.IsvalidMilitaryTime(dr1["actualtimein"].ToString().Trim()))
                            {
                                dr1["Clock In"] = 1;
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["actualtimeout"]) && (Convert.ToInt16(dr1["Clock Out"]) == 0 || BizFunctions.IsEmpty(dr1["Clock In"])))
                        {
                            if (TextValidator.TextValidator.IsvalidMilitaryTime(dr1["actualtimeout"].ToString().Trim()))
                            {
                                dr1["Clock Out"] = 1;
                            }
                        }

                        #region If Empty

                        if (BizFunctions.IsEmpty(dr1["Day"]))
                        {
                            dr1["Day"] = TimeTools.GetDay(TimeTools.GetDayNoOfWeek(BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text))));
                        }

                        if (BizFunctions.IsEmpty(dr1["shifttype"]) || dr1["shifttype"].ToString() == string.Empty)
                        {
                            if (!BizFunctions.IsEmpty(dr1["shiftcode"]) || dr1["shiftcode"].ToString() != string.Empty)
                            {
                                dr1["shifttype"] = dr1["shiftcode"].ToString().Substring(0, 1);
                            }

                        }

                        if (BizFunctions.IsEmpty(dr1["nric"]))
                        {
                            dr1["nric"] = getNric(dr1["empnum"].ToString());
                        }

                        if (BizFunctions.IsEmpty(dr1["Is Replaced"]))
                        {
                            dr1["Is Replaced"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["Adhoc"]))
                        {
                            dr1["Adhoc"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["TR"]))
                        {
                            dr1["TR"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["OT"]))
                        {
                            dr1["OT"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["DRE"]))
                        {
                            dr1["DRE"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["OS"]))
                        {
                            dr1["OS"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["RD"]))
                        {
                            dr1["RD"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["US"]))
                        {
                            dr1["US"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["RealSchedule"]))
                        {
                            dr1["RealSchedule"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["Date"]) || dr1["Date"].ToString() == string.Empty)
                        {
                            dr1["Date"] = Convert.ToDateTime(dateTimePicker1.Text).ToShortDateString();
                        }

                        if (BizFunctions.IsEmpty(dr1["OTrate"]) || dr1["OTrate"].ToString()== string.Empty)
                        {
                            dr1["OTrate"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["Clock In"]))
                        {
                            dr1["Clock In"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["Clock Out"]))
                        {
                            dr1["Clock Out"] = 0;
                        }
                       
                        

                        if(BizFunctions.IsEmpty(dr1["uniquekey"]))
                        {
                            dr1["uniquekey"] = dr1["empnum"].ToString() + "-" + dr1["shiftcode"].ToString() + "-" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["Date"]));
                        }

                        #endregion

                        #region if Empnum is not Empty
                        if ((!BizFunctions.IsEmpty(dr1["empnum"]) || dr1["empnum"].ToString() != string.Empty))
                        {
                            #region If Employee doesn't have conflicting Schedule

                            if (!isScheduled(dr1["uniquekey"].ToString()))
                            {
                                #region If ActualDateTimeOut is Empty
                                if (BizFunctions.IsEmpty(dr1["ActualDateTimeOut"]))
                                {
                                    TimeOutEmpty = true;
                                }
                                #endregion

                                #region Get Max ID of AtmrLive

                                int maxCount = BizLogicTools.Tools.getMaxID("ATMRLive", this.dbaccess);

                                #endregion

                                #region if Actual Date Time OUT is Empty

                                if ((BizFunctions.IsEmpty(dr1["actualdateTimeOut"]) || dr1["actualdateTimeOut"].ToString() == string.Empty) && (!BizFunctions.IsEmpty(dr1["actualdateTimeIn"]) || dr1["actualdateTimeIn"].ToString() != string.Empty))
                                {
                                    InsertRow = "Insert Into ATMRLIVE " +
                                                           "(ID,uniquekey,refnum,sectorcode,shiftcode,nric,empnum,isreplaced,rnric,rempnum,rempname,timein,[timeout],actualtimein2,actualtimeout2,ActualDateTimeIn,ActualDateTimeOut, " +
                                                           "created,[day],[Date],[status],remark,[user],trandate,Refkey,sitenum,isAdhoc,isOT,OTrate,isDRE,isOffset,isRD,isUS,isRealSchedule,ClockInMark,ClockOutMark,isTR,TRsitenum,[guid]) " +
                                                           "VALUES " +
                                                           "(" + Convert.ToString(maxCount + 1) + ", '" + dr1["uniquekey"].ToString() + "','" + WRRrefNoTb.Text + "','" + SectorTb.Text + "', " +
                                                           "'" + dr1["shiftcode"].ToString() + "','" + dr1["nric"].ToString() + "','" + dr1["empnum"].ToString() + "'," + Convert.ToInt16(dr1["Is Replaced"]).ToString() + ",'" + dr1["rnric"].ToString() + "', " +
                                                           "'" + dr1["Rep Empno"].ToString() + "','" + dr1["Rep Name"].ToString() + "','" + dr1["timein"].ToString() + "','" + dr1["timeout"].ToString() + "','" + dr1["actualtimein"].ToString() + "', " +
                                                           "'" + dr1["actualtimeout"].ToString() + "','" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["actualdateTimeIn"])) + "',NULL,'" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "', " +
                                                           "'" + dr1["day"].ToString() + "','" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["Date"])) + "','" + Common.DEFAULT_DOCUMENT_STATUSO + "','" + dr1["remark"].ToString() + "','" + Common.DEFAULT_SYSTEM_USERNAME + "','" + BizFunctions.GetSafeDateString(DateTime.Now) + "', " +
                                                           "'" + dr1["Refkey"].ToString() + "','" + SiteCodeTb.Text + "'," + Convert.ToInt16(dr1["Adhoc"]).ToString() + "," + Convert.ToInt16(dr1["OT"]).ToString() + "," + dr1["OTrate"].ToString() + "," + Convert.ToInt16(dr1["DRE"]).ToString() + "," + Convert.ToInt16(dr1["OS"]).ToString() + "," +
                                                           Convert.ToInt16(dr1["RD"]).ToString() + "," + Convert.ToInt16(dr1["US"]).ToString() + "," + Convert.ToInt16(dr1["RealSchedule"]).ToString() + "," + Convert.ToInt16(dr1["Clock In"]).ToString() + "," + Convert.ToInt16(dr1["Clock Out"]).ToString() + "," + Convert.ToInt16(dr1["TR"]).ToString() + ",'" + dr1["Site TR"].ToString() + "','" + BizLogicTools.Tools.getGUID() + "' )";
                                }

                                #endregion

                                #region If Actual Date Time IN is Emtpy

                                else if ((BizFunctions.IsEmpty(dr1["actualdateTimeIn"]) || dr1["actualdateTimeIn"].ToString() == string.Empty) && (!BizFunctions.IsEmpty(dr1["actualdateTimeOut"]) || dr1["actualdateTimeOut"].ToString() != string.Empty))
                                {
                                    InsertRow = "Insert Into ATMRLIVE " +
                                                          "(ID,uniquekey,refnum,sectorcode,shiftcode,nric,empnum,isreplaced,rnric,rempnum,rempname,timein,[timeout],actualtimein2,actualtimeout2,ActualDateTimeIn,ActualDateTimeOut, " +
                                                          "created,[day],[Date],[status],remark,[user],trandate,Refkey,sitenum,isAdhoc,isOT,OTrate,isDRE,isOffset,isRD,isUS,isRealSchedule,ClockInMark,ClockOutMark,isTR,TRsitenum,[guid]) " +
                                                          "VALUES " +
                                                          "(" + Convert.ToString(maxCount + 1) + ", '" + dr1["uniquekey"].ToString() + "','" + WRRrefNoTb.Text + "','" + SectorTb.Text + "', " +
                                                          "'" + dr1["shiftcode"].ToString() + "','" + dr1["nric"].ToString() + "','" + dr1["empnum"].ToString() + "'," + Convert.ToInt16(dr1["Is Replaced"]).ToString() + ",'" + dr1["rnric"].ToString() + "', " +
                                                          "'" + dr1["Rep Empno"].ToString() + "','" + dr1["Rep Name"].ToString() + "','" + dr1["timein"].ToString() + "','" + dr1["timeout"].ToString() + "','" + dr1["actualtimein"].ToString() + "', " +
                                                          "'" + dr1["actualtimeout"].ToString() + "',NULL,NULL,'" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "', " +
                                                          "'" + dr1["day"].ToString() + "','" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["Date"])) + "','" + Common.DEFAULT_DOCUMENT_STATUSO + "','" + dr1["remark"].ToString() + "','" + Common.DEFAULT_SYSTEM_USERNAME + "','" + BizFunctions.GetSafeDateString(DateTime.Now) + "', " +
                                                          "'" + dr1["Refkey"].ToString() + "','" + SiteCodeTb.Text + "'," + Convert.ToInt16(dr1["Adhoc"]).ToString() + "," + Convert.ToInt16(dr1["OT"]).ToString() + "," + dr1["OTrate"].ToString() + "," + Convert.ToInt16(dr1["DRE"]).ToString() + "," + Convert.ToInt16(dr1["OS"]).ToString() + "," +
                                                          Convert.ToInt16(dr1["RD"]).ToString() + "," + Convert.ToInt16(dr1["US"]).ToString() + "," + Convert.ToInt16(dr1["RealSchedule"]).ToString() + "," + Convert.ToInt16(dr1["Clock In"]).ToString() + "," + Convert.ToInt16(dr1["Clock Out"]).ToString() + "," + Convert.ToInt16(dr1["TR"]).ToString() + ",'" + dr1["Site TR"].ToString() + "','" + BizLogicTools.Tools.getGUID() + "' )";
                                }

                                #endregion

                                #region If BOTH Actual Date Time IN & ctual Date Time IN are Emtpy

                                else if ((BizFunctions.IsEmpty(dr1["actualdateTimeOut"]) || dr1["actualdateTimeOut"].ToString() == string.Empty) && (BizFunctions.IsEmpty(dr1["actualdateTimeIn"]) || dr1["actualdateTimeIn"].ToString() == string.Empty))
                                {
                                    InsertRow = "Insert Into ATMRLIVE " +
                                                         "(ID,uniquekey,refnum,sectorcode,shiftcode,nric,empnum,isreplaced,rnric,rempnum,rempname,timein,[timeout],actualtimein2,actualtimeout2,ActualDateTimeIn,ActualDateTimeOut, " +
                                                         "created,[day],[Date],[status],remark,[user],trandate,Refkey,sitenum,isAdhoc,isOT,OTrate,isDRE,isOffset,isRD,isUS,isRealSchedule,ClockInMark,ClockOutMark,isTR,TRsitenum,[guid]) " +
                                                         "VALUES " +
                                                         "(" + Convert.ToString(maxCount + 1) + ", '" + dr1["uniquekey"].ToString() + "','" + WRRrefNoTb.Text + "','" + SectorTb.Text + "', " +
                                                         "'" + dr1["shiftcode"].ToString() + "','" + dr1["nric"].ToString() + "','" + dr1["empnum"].ToString() + "'," + Convert.ToInt16(dr1["Is Replaced"]).ToString() + ",'" + dr1["rnric"].ToString() + "', " +
                                                         "'" + dr1["Rep Empno"].ToString() + "','" + dr1["Rep Name"].ToString() + "','" + dr1["timein"].ToString() + "','" + dr1["timeout"].ToString() + "','" + dr1["actualtimein"].ToString() + "', " +
                                                         "'" + dr1["actualtimeout"].ToString() + "',NULL,NULL,'" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "', " +
                                                         "'" + dr1["day"].ToString() + "','" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["Date"])) + "','" + Common.DEFAULT_DOCUMENT_STATUSO + "','" + dr1["remark"].ToString() + "','" + Common.DEFAULT_SYSTEM_USERNAME + "','" + BizFunctions.GetSafeDateString(DateTime.Now) + "', " +
                                                         "'" + dr1["Refkey"].ToString() + "','" + SiteCodeTb.Text + "'," + Convert.ToInt16(dr1["Adhoc"]).ToString() + "," + Convert.ToInt16(dr1["OT"]).ToString() + "," + dr1["OTrate"].ToString() + "," + Convert.ToInt16(dr1["DRE"]).ToString() + "," + Convert.ToInt16(dr1["OS"]).ToString() + "," +
                                                         Convert.ToInt16(dr1["RD"]).ToString() + "," + Convert.ToInt16(dr1["US"]).ToString() + "," + Convert.ToInt16(dr1["RealSchedule"]).ToString() + "," + Convert.ToInt16(dr1["Clock In"]).ToString() + "," + Convert.ToInt16(dr1["Clock Out"]).ToString() + "," + Convert.ToInt16(dr1["TR"]).ToString() + ",'" + dr1["Site TR"].ToString() + "','" + BizLogicTools.Tools.getGUID() + "' )";
                                }

                                #endregion

                                #region Else BOTH Actual Date Time IN & ctual Date Time IN are NOT Emtpy

                                else
                                {
                                    InsertRow = "Insert Into ATMRLIVE " +
                                                         "(ID,uniquekey,refnum,sectorcode,shiftcode,nric,empnum,isreplaced,rnric,rempnum,rempname,timein,[timeout],actualtimein2,actualtimeout2,ActualDateTimeIn,ActualDateTimeOut, " +
                                                         "created,[day],[Date],[status],remark,[user],trandate,Refkey,sitenum,isAdhoc,isOT,OTrate,isDRE,isOffset,isRD,isUS,isRealSchedule,ClockInMark,ClockOutMark,isTR,TRsitenum,[guid]) " +
                                                         "VALUES " +
                                                         "(" + Convert.ToString(maxCount + 1) + ", '" + dr1["uniquekey"].ToString() + "','" + WRRrefNoTb.Text + "','" + SectorTb.Text + "', " +
                                                         "'" + dr1["shiftcode"].ToString() + "','" + dr1["nric"].ToString() + "','" + dr1["empnum"].ToString() + "'," + Convert.ToInt16(dr1["Is Replaced"]).ToString() + ",'" + dr1["rnric"].ToString() + "', " +
                                                         "'" + dr1["Rep Empno"].ToString() + "','" + dr1["Rep Name"].ToString() + "','" + dr1["timein"].ToString() + "','" + dr1["timeout"].ToString() + "','" + dr1["actualtimein"].ToString() + "', " +
                                                         "'" + dr1["actualtimeout"].ToString() + "',NULL,'" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["actualdateTimeOut"])) + "','" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text)) + "', " +
                                                         "'" + dr1["day"].ToString() + "','" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["Date"])) + "','" + Common.DEFAULT_DOCUMENT_STATUSO + "','" + dr1["remark"].ToString() + "','" + Common.DEFAULT_SYSTEM_USERNAME + "','" + BizFunctions.GetSafeDateString(DateTime.Now) + "', " +
                                                         "'" + dr1["Refkey"].ToString() + "','" + SiteCodeTb.Text + "'," + Convert.ToInt16(dr1["Adhoc"]).ToString() + "," + Convert.ToInt16(dr1["OT"]).ToString() + "," + dr1["OTrate"].ToString() + "," + Convert.ToInt16(dr1["DRE"]).ToString() + "," + Convert.ToInt16(dr1["OS"]).ToString() + "," +
                                                         Convert.ToInt16(dr1["RD"]).ToString() + "," + Convert.ToInt16(dr1["US"]).ToString() + "," + Convert.ToInt16(dr1["RealSchedule"]).ToString() + "," + Convert.ToInt16(dr1["Clock In"]).ToString() + "," + Convert.ToInt16(dr1["Clock Out"]).ToString() + "," + Convert.ToInt16(dr1["TR"]).ToString() + ",'" + dr1["Site TR"].ToString() + "','" + BizLogicTools.Tools.getGUID() + "' )";
                                }

                                #endregion

                                #region Execute Non Query Insert

                                try
                                {
                                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(InsertRow);

                                    if (TimeOutEmpty)
                                    {
                                        string UpdateActualTimeOut = "Update ATMRlive set ActualDateTimeOut=null where uniquekey='" + dr1["uniquekey"].ToString() + "' ";
                                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateActualTimeOut);
                                        TimeOutEmpty = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                                #endregion

                            }

                            #endregion

                            #region Empnum has Conflicting Schedule so Update Only
                            else
                            {
                                #region If ActualDateTimeOut is Empty

                                if (BizFunctions.IsEmpty(dr1["ActualDateTimeOut"]))
                                {
                                    TimeOutEmpty = true;
                                }

                                #endregion

                                #region if Actual Date Time OUT is Empty

                                if ((BizFunctions.IsEmpty(dr1["actualdateTimeIn"]) || dr1["actualdateTimeIn"].ToString() == string.Empty) && (!BizFunctions.IsEmpty(dr1["actualdateTimeOut"]) || dr1["actualdateTimeOut"].ToString() != string.Empty))
                                {
                                    UpdateRow = "UPDATE ATMRLIVE SET " +
                                             "Sectorcode='" + SectorTb.Text + "', " +
                                             "shiftcode='" + dr1["shiftcode"].ToString() + "', " +
                                             "nric='" + dr1["nric"].ToString() + "', " +
                                             "isreplaced='" + Convert.ToInt16(dr1["Is Replaced"]).ToString() + "', " +
                                             "rnric='" + dr1["rnric"].ToString() + "', " +
                                             "rempnum='" + dr1["Rep Empno"].ToString() + "', " +
                                             "rempname='" + dr1["Rep Name"].ToString() + "', " +
                                             "actualtimein2='" + dr1["actualtimein"].ToString() + "', " +
                                             "actualtimeout2='" + dr1["actualtimeout"].ToString() + "', " +
                                             "ActualDateTimein=null, " +
                                             "ActualDateTimeOut='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["actualdateTimeOut"])) + "', " +
                                             "Refkey='" + dr1["Refkey"].ToString() + "', " +
                                             "isAdhoc=" + Convert.ToInt16(dr1["Adhoc"]).ToString() + ", " +
                                             "isOT=" + Convert.ToInt16(dr1["OT"]).ToString() + ", " +
                                             "isDre=" + Convert.ToInt16(dr1["DRE"]).ToString() + ", " +
                                             "isRD=" + Convert.ToInt16(dr1["RD"]).ToString() + ", " +
                                             "isUS=" + Convert.ToInt16(dr1["US"]).ToString() + ", " +
                                             "ClockInMark=" + Convert.ToInt16(dr1["Clock In"]).ToString() + ", " +
                                             "ClockOutMark=" + Convert.ToInt16(dr1["Clock Out"]).ToString() + ", " +
                                             "isTR=" + Convert.ToInt16(dr1["TR"]).ToString() + " ," +
                                             "TRsitenum='" + dr1["Site TR"].ToString() + "' " +
                                             "where uniquekey='" + dr1["uniquekey"].ToString() + "'";

                                }

                                #endregion

                                #region If Actual Date Time IN is Emtpy

                                else if ((BizFunctions.IsEmpty(dr1["actualdateTimeOut"]) || dr1["actualdateTimeOut"].ToString() == string.Empty) && (!BizFunctions.IsEmpty(dr1["actualdateTimeIn"]) || dr1["actualdateTimeIn"].ToString() != string.Empty))
                                {
                                    UpdateRow = "UPDATE ATMRLIVE SET " +
                                           "Sectorcode='" + SectorTb.Text + "', " +
                                           "shiftcode='" + dr1["shiftcode"].ToString() + "', " +
                                           "nric='" + dr1["nric"].ToString() + "', " +
                                           "isreplaced='" + Convert.ToInt16(dr1["Is Replaced"]).ToString() + "', " +
                                           "rnric='" + dr1["rnric"].ToString() + "', " +
                                           "rempnum='" + dr1["Rep Empno"].ToString() + "', " +
                                           "rempname='" + dr1["Rep Name"].ToString() + "', " +
                                           "actualtimein2='" + dr1["actualtimein"].ToString() + "', " +
                                           "actualtimeout2='" + dr1["actualtimeout"].ToString() + "', " +
                                           "ActualDateTimein='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["actualdateTimeIn"])) + "', " +
                                           "ActualDateTimeOut=null, " +
                                           "Refkey='" + dr1["Refkey"].ToString() + "', " +
                                           "isAdhoc=" + Convert.ToInt16(dr1["Adhoc"]).ToString() + ", " +
                                           "isOT=" + Convert.ToInt16(dr1["OT"]).ToString() + ", " +
                                           "isDre=" + Convert.ToInt16(dr1["DRE"]).ToString() + ", " +
                                           "isOffset=" + Convert.ToInt16(dr1["OS"]).ToString() + ", " +
                                           "isRD=" + Convert.ToInt16(dr1["RD"]).ToString() + ", " +
                                           "isUS=" + Convert.ToInt16(dr1["US"]).ToString() + ", " +
                                           "ClockInMark=" + Convert.ToInt16(dr1["Clock In"]).ToString() + ", " +
                                           "ClockOutMark=" + Convert.ToInt16(dr1["Clock Out"]).ToString() + ", " +
                                           "isTR=" + Convert.ToInt16(dr1["TR"]).ToString() + " ," +
                                           "TRsitenum='" + dr1["Site TR"].ToString() + "' " +
                                           "where uniquekey='" + dr1["uniquekey"].ToString() + "'";
                                }

                                #endregion

                                #region If BOTH Actual Date Time IN & ctual Date Time IN are Emtpy

                                else if ((BizFunctions.IsEmpty(dr1["actualdateTimeOut"]) || dr1["actualdateTimeOut"].ToString() == string.Empty) && (BizFunctions.IsEmpty(dr1["actualdateTimeIn"]) || dr1["actualdateTimeIn"].ToString() == string.Empty))
                                {
                                    UpdateRow = "UPDATE ATMRLIVE SET " +
                                         "Sectorcode='" + SectorTb.Text + "', " +
                                         "shiftcode='" + dr1["shiftcode"].ToString() + "', " +
                                         "nric='" + dr1["nric"].ToString() + "', " +
                                         "isreplaced='" + Convert.ToInt16(dr1["Is Replaced"]).ToString() + "', " +
                                         "rnric='" + dr1["rnric"].ToString() + "', " +
                                         "rempnum='" + dr1["Rep Empno"].ToString() + "', " +
                                         "rempname='" + dr1["Rep Name"].ToString() + "', " +
                                         "actualtimein2='" + dr1["actualtimein"].ToString() + "', " +
                                         "actualtimeout2='" + dr1["actualtimeout"].ToString() + "', " +
                                         "ActualDateTimein=null, " +
                                         "ActualDateTimeOut=null, " +
                                         "Refkey='" + dr1["Refkey"].ToString() + "', " +
                                         "isAdhoc=" + Convert.ToInt16(dr1["Adhoc"]).ToString() + ", " +
                                         "isOT=" + Convert.ToInt16(dr1["OT"]).ToString() + ", " +
                                         "isDre=" + Convert.ToInt16(dr1["DRE"]).ToString() + ", " +
                                         "isOffset=" + Convert.ToInt16(dr1["OS"]).ToString() + ", " +
                                         "isRD=" + Convert.ToInt16(dr1["RD"]).ToString() + ", " +
                                         "isUS=" + Convert.ToInt16(dr1["US"]).ToString() + ", " +
                                         "ClockInMark=" + Convert.ToInt16(dr1["Clock In"]).ToString() + ", " +
                                         "ClockOutMark=" + Convert.ToInt16(dr1["Clock Out"]).ToString() + ", " +
                                         "isTR=" + Convert.ToInt16(dr1["TR"]).ToString() + " ," +
                                         "TRsitenum='" + dr1["Site TR"].ToString() + "' " +
                                         "where uniquekey='" + dr1["uniquekey"].ToString() + "'";
                                }

                                #endregion

                                #region Else BOTH Actual Date Time IN & ctual Date Time IN are NOT Emtpy

                                else
                                {
                                    UpdateRow = "UPDATE ATMRLIVE SET " +
                                              "Sectorcode='" + SectorTb.Text + "', " +
                                              "shiftcode='" + dr1["shiftcode"].ToString() + "', " +
                                              "nric='" + dr1["nric"].ToString() + "', " +
                                              "isreplaced='" + dr1["Is Replaced"].ToString() + "', " +
                                              "rnric='" + dr1["rnric"].ToString() + "', " +
                                              "rempnum='" + dr1["Rep Empno"].ToString() + "', " +
                                              "rempname='" + dr1["Rep Name"].ToString() + "', " +
                                              "actualtimein2='" + dr1["actualtimein"].ToString() + "', " +
                                              "actualtimeout2='" + dr1["actualtimeout"].ToString() + "', " +
                                              "ActualDateTimein='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["actualdateTimeIn"])) + "', " +
                                              "ActualDateTimeOut='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["actualdateTimeOut"])) + "', " +
                                              "Refkey='" + dr1["Refkey"].ToString() + "', " +
                                              "isAdhoc=" + Convert.ToInt16(dr1["Adhoc"]).ToString() + ", " +
                                              "isOT=" + Convert.ToInt16(dr1["OT"]).ToString() + ", " +
                                              "isDre=" + Convert.ToInt16(dr1["DRE"]).ToString() + ", " +
                                              "isOffset=" + Convert.ToInt16(dr1["OS"]).ToString() + ", " +
                                              "isRD=" + Convert.ToInt16(dr1["RD"]).ToString() + ", " +
                                              "isUS=" + Convert.ToInt16(dr1["US"]).ToString() + ", " +
                                              "ClockInMark=" + Convert.ToInt16(dr1["Clock In"]).ToString() + ", " +
                                              "ClockOutMark=" + Convert.ToInt16(dr1["Clock Out"]).ToString() + ", " +
                                              "isTR=" + Convert.ToInt16(dr1["TR"]).ToString() + " ," +
                                              "TRsitenum='" + dr1["Site TR"].ToString() + "' " +
                                              "where uniquekey='" + dr1["uniquekey"].ToString() + "'";
                                }

                                #endregion

                                #region Execute Non Query Update

                                try
                                {

                                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateRow);

                                    if (TimeOutEmpty)
                                    {
                                        string UpdateActualTimeOut = "Update ATMRlive set ActualDateTimeOut=null where uniquekey='" + dr1["uniquekey"].ToString() + "' ";
                                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateActualTimeOut);
                                        TimeOutEmpty = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                                #endregion

                            }

                            #endregion
                        }
                        #endregion

                        #region Old Code
                        //#region Else if Empnum is Empty
                        //else
                        //{


                        //    UpdateRow = "UPDATE ATMRLIVE SET " +
                        //              "Sectorcode='" + SectorTb.Text + "', " +
                        //              "shiftcode='" + dr1["shiftcode"].ToString() + "', " +
                        //              "nric='" + dr1["nric"].ToString() + "', " +
                        //              "isreplaced='" + dr1["Is Replaced"].ToString() + "', " +
                        //              "rnric='" + dr1["rnric"].ToString() + "', " +
                        //              "rempnum='" + dr1["Rep Empno"].ToString() + "', " +
                        //              "rempname='" + dr1["Rep Name"].ToString() + "', " +
                        //              "actualtimein2='" + dr1["actualtimein"].ToString() + "', " +
                        //              "actualtimeout2='" + dr1["actualtimeout"].ToString() + "', " +
                        //              "ActualDateTimein=null, " +
                        //              "ActualDateTimeOut=null, " +
                        //              "Refkey='" + dr1["Refkey"].ToString() + "', " +
                        //              "isAdhoc=" + Convert.ToInt16(dr1["Adhoc"]).ToString() + ", " +
                        //              "isOT=" + Convert.ToInt16(dr1["OT"]).ToString() + ", " +
                        //              "isDre=" + Convert.ToInt16(dr1["DRE"]).ToString() + ", " +
                        //              "isRD=" + Convert.ToInt16(dr1["RD"]).ToString() + ", " +
                        //              "isUS=" + Convert.ToInt16(dr1["US"]).ToString() + ", " +
                        //              "ClockInMark=" + Convert.ToInt16(dr1["Clock In"]).ToString() + ", " +
                        //              "ClockOutMark=" + Convert.ToInt16(dr1["Clock Out"]).ToString() + ", " +
                        //              "isTR=" + Convert.ToInt16(dr1["TR"]).ToString() + " ," +
                        //              "TRsitenum='" + dr1["Site TR"].ToString() + "' " +
                        //              "where uniquekey='" + dr1["uniquekey"].ToString() + "'";
                        //}

                        //#endregion

                        //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateRow);


                        //if (TimeOutEmpty)
                        //{
                        //    string UpdateActualTimeOut = "Update ATMRlive set ActualDateTimeOut=null where uniquekey='" + dr1["uniquekey"].ToString() + "' ";
                        //    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateActualTimeOut);
                        //    TimeOutEmpty = false;
                        //}
                        #endregion
                    }
                }
                //Parameter[] parameters = new Parameter[1];

                //foreach (DataRow drC in ATMRdatasource.Rows)
                //{
                //    drC.AcceptChanges();
                //}
                //this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("DeleteDuplicatesATMRLIVE", ref parameters);
                GetWRRInfo();
            }
            //add nric to select statement earlier
          
            
        }

        private bool isScheduled(string uniquekey)
        {
            bool hasSchedule = false;
            string Lists = "";
            int count = 0;
        

            DataTable Schedule = this.dbaccess.DataSet.Tables["CurrentSchedule"];
            if (Schedule.Rows.Count > 0)
            {
                foreach (DataRow dr1 in Schedule.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (dr1["uniquekey"].ToString() == uniquekey)
                        {
                            hasSchedule = true;
                            break;
                        }
                    }
                }
            }

         
            return hasSchedule;

        }

        private void GetSITMI()
        {
            string GetSITMI = "SELECT * FROM SITM WHERE [STATUS]<>'V'";
            this.dbaccess.ReadSQL("SITM", GetSITMI);
        }

        private string GetSiteName(string sitenum)
        {
            string sitename = "";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select sitename from SITM where sitenum='"+sitenum+"'");

            if(dt1.Rows.Count > 0)
            {
                sitename = dt1.Rows[0]["sitename"].ToString();
            }
            return sitename;
        }

        private bool UniqueKeyExists(string uniquekey)
        {
            bool hasSchedule = false;
            string Lists = "";
            int count = 0;


            DataTable Schedule = this.dbaccess.DataSet.Tables["CurrentSchedule"];
            if (Schedule.Rows.Count > 0)
            {
                foreach (DataRow dr1 in Schedule.Rows)
                {
                    if (dr1["uniquekey"].ToString() == uniquekey)
                    {
                        Lists = Lists + "\n Employee " + dr1["empnum"].ToString() + " on " + Convert.ToDateTime(dr1["Date"]).ToShortDateString() + " (" + dr1["shiftcode"].ToString() + ") ";

                        break;
                    }
                }
            }

            if (count > 0)
            {
                MessageBox.Show(Lists, "Schedule has Conflicts,Save Unsuccessful");
                hasSchedule = true;
            }
            return hasSchedule;

        }

        private void GetSchedule()
        {

            string GetCurrentSchedule = "Select * from atmrlive where  refnum='" + WRRrefNoTb.Text + "' AND [date]='"+TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dateTimePicker1.Text))+"'";

            if (this.dbaccess.DataSet.Tables.Contains("CurrentSchedule"))
            {
                this.dbaccess.DataSet.Tables["CurrentSchedule"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("CurrentSchedule");
            }

            this.dbaccess.ReadSQL("CurrentSchedule", GetCurrentSchedule);
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
          
            xATMRdatasource = BizFunctions.ExecuteQuery(this.dbaccess.DataSet,"SELECT * FROM ATMRtmp where Mark=1");
            yATMRdatasource = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM ATMRtmp where ISNULL(Mark,0)=0");

            if (yATMRdatasource.Rows.Count > 0)
            {
                DataTable tmpyATMRdatasource = yATMRdatasource.Copy();
               
            }

            if(xATMRdatasource.Rows.Count > 0)
            {
                foreach (DataRow dr3 in xATMRdatasource.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        string DeleteUniqueKey = "Delete from ATMRLIVE WHERE UNIQUEKEY='" + dr3["uniquekey"].ToString() + "'";
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(DeleteUniqueKey);
                    }
                }
            }


            for (int i = 0; i < ATMRdatasource.Rows.Count - 1; i++)
            {
                if (ATMRdatasource.Rows[i].RowState != DataRowState.Deleted)
                {
                    if(BizFunctions.IsEmpty(ATMRdatasource.Rows[i]["Mark"]))
                    {
                        ATMRdatasource.Rows[i]["Mark"] =0;
                    }
                    if ((bool)ATMRdatasource.Rows[i]["Mark"])
                    {
                        ATMRdatasource.Rows[i]["empnum"].ToString();
                        ATMRdatasource.Rows[i].Delete();
                    }
                }
            }

            if (yATMRdatasource.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ATMRdatasource);

                foreach (DataRow dr1 in yATMRdatasource.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {                       
                        ATMRdatasource.ImportRow(dr1);
                    }
                }
               
            }

            //GetWRRInfo();
            
        }

        private void TimeControlForm1_Load_1(object sender, EventArgs e)
        {

        }

        //private DataTable GetPOrest()
        //{
        //    string GetCurrentREST = "Select * from atmr where ISNULL(isSchedule,0)=0 and [status]<>'V' and shiftcode='REST'";
        //}

        private void GetRoster()
        {
            this.selectsCollection = new Hashtable();

            string GetWRR1 = "SELECT MATNUM AS APPT,EMPNUM AS EMPNO,EMPNAME AS NAME, MONDAY AS MON,TUESDAY AS TUE, WEDNESDAY AS WED,THURSDAY AS THU,FRIDAY AS FRI,SATURDAY AS SAT,SUNDAY AS SUN  FROM WRR1 WHERE REFNUM='"+ WRRrefNoTb.Text +"'";
            string GetWRR2 = "SELECT MATNUM AS APPT,EMPNUM AS EMPNO,EMPNAME AS NAME, MONDAY AS MON,TUESDAY AS TUE, WEDNESDAY AS WED,THURSDAY AS THU,FRIDAY AS FRI,SATURDAY AS SAT,SUNDAY AS SUN  FROM WRR2 WHERE REFNUM='" + WRRrefNoTb.Text + "'";
            string GetWRR3 = "SELECT MATNUM AS APPT,EMPNUM AS EMPNO,EMPNAME AS NAME, MONDAY AS MON,TUESDAY AS TUE, WEDNESDAY AS WED,THURSDAY AS THU,FRIDAY AS FRI,SATURDAY AS SAT,SUNDAY AS SUN  FROM WRR3 WHERE REFNUM='" + WRRrefNoTb.Text + "'";
            string GetWRR4 = "SELECT MATNUM AS APPT,EMPNUM AS EMPNO,EMPNAME AS NAME, MONDAY AS MON,TUESDAY AS TUE, WEDNESDAY AS WED,THURSDAY AS THU,FRIDAY AS FRI,SATURDAY AS SAT,SUNDAY AS SUN  FROM WRR4 WHERE REFNUM='" + WRRrefNoTb.Text + "'";

            this.selectsCollection.Add("WRR1",GetWRR1);
            this.selectsCollection.Add("WRR2",GetWRR2);
            this.selectsCollection.Add("WRR3",GetWRR3);
            this.selectsCollection.Add("WRR4",GetWRR4);

            this.dbaccess.ReadSQL(selectsCollection);
           


            BindingSource WRR1bindingSource = new BindingSource();

            WRR1bindingSource.DataSource = this.dbaccess.DataSet.Tables["WRR1"];

            DayDGv1.DataSource = WRR1bindingSource;

            int ColumnSize = 65;

            DataGridViewColumn MONwrr1 = DayDGv1.Columns["MON"];
            MONwrr1.Width = ColumnSize;

            DataGridViewColumn TUEwrr1 = DayDGv1.Columns["TUE"];
            TUEwrr1.Width = ColumnSize;

            DataGridViewColumn WEDwrr1 = DayDGv1.Columns["WED"];
            WEDwrr1.Width = ColumnSize;

            DataGridViewColumn THUwrr1 = DayDGv1.Columns["THU"];
            THUwrr1.Width = ColumnSize;

            DataGridViewColumn FRIwrr1 = DayDGv1.Columns["FRI"];
            FRIwrr1.Width = ColumnSize;

            DataGridViewColumn SATwrr1 = DayDGv1.Columns["SAT"];
            SATwrr1.Width = ColumnSize;

            DataGridViewColumn SUNwrr1 = DayDGv1.Columns["SUN"];
            SUNwrr1.Width = ColumnSize;

            //////////////////////////////////////////

            BindingSource WRR2bindingSource = new BindingSource();

            WRR2bindingSource.DataSource = this.dbaccess.DataSet.Tables["WRR2"];

            NightDGv1.DataSource = WRR2bindingSource;

            DataGridViewColumn MONwrr2 = NightDGv1.Columns["MON"];
            MONwrr2.Width = ColumnSize;

            DataGridViewColumn TUEwrr2 = NightDGv1.Columns["TUE"];
            TUEwrr2.Width = ColumnSize;

            DataGridViewColumn WEDwrr2 = NightDGv1.Columns["WED"];
            WEDwrr2.Width = ColumnSize;

            DataGridViewColumn THUwrr2 = NightDGv1.Columns["THU"];
            THUwrr2.Width = ColumnSize;

            DataGridViewColumn FRIwrr2 = NightDGv1.Columns["FRI"];
            FRIwrr2.Width = ColumnSize;

            DataGridViewColumn SATwrr2 = NightDGv1.Columns["SAT"];
            SATwrr2.Width = ColumnSize;

            DataGridViewColumn SUNwrr2 = NightDGv1.Columns["SUN"];
            SUNwrr2.Width = ColumnSize;


            /////////////////////////////////////////

            BindingSource WRR3bindingSource = new BindingSource();

            WRR3bindingSource.DataSource = this.dbaccess.DataSet.Tables["WRR3"];

            ConciergeDGv1.DataSource = WRR3bindingSource;

            DataGridViewColumn MONwrr3 = ConciergeDGv1.Columns["MON"];
            MONwrr3.Width = ColumnSize;

            DataGridViewColumn TUEwrr3 = ConciergeDGv1.Columns["TUE"];
            TUEwrr3.Width = ColumnSize;

            DataGridViewColumn WEDwrr3 = ConciergeDGv1.Columns["WED"];
            WEDwrr3.Width = ColumnSize;

            DataGridViewColumn THUwrr3 = ConciergeDGv1.Columns["THU"];
            THUwrr3.Width = ColumnSize;

            DataGridViewColumn FRIwrr3 = ConciergeDGv1.Columns["FRI"];
            FRIwrr3.Width = ColumnSize;

            DataGridViewColumn SATwrr3 = ConciergeDGv1.Columns["SAT"];
            SATwrr3.Width = ColumnSize;

            DataGridViewColumn SUNwrr3 = ConciergeDGv1.Columns["SUN"];
            SUNwrr3.Width = ColumnSize;

            /////////////////////////////////////////

            BindingSource WRR4bindingSource = new BindingSource();

            WRR4bindingSource.DataSource = this.dbaccess.DataSet.Tables["WRR4"];

            ReliefDGv1.DataSource = WRR4bindingSource;


            DataGridViewColumn MONwrr4 = ReliefDGv1.Columns["MON"];
            MONwrr4.Width = ColumnSize;

            DataGridViewColumn TUEwrr4 = ReliefDGv1.Columns["TUE"];
            TUEwrr4.Width = ColumnSize;

            DataGridViewColumn WEDwrr4 = ReliefDGv1.Columns["WED"];
            WEDwrr4.Width = ColumnSize;

            DataGridViewColumn THUwrr4 = ReliefDGv1.Columns["THU"];
            THUwrr4.Width = ColumnSize;

            DataGridViewColumn FRIwrr4 = ReliefDGv1.Columns["FRI"];
            FRIwrr4.Width = ColumnSize;

            DataGridViewColumn SATwrr4 = ReliefDGv1.Columns["SAT"];
            SATwrr4.Width = ColumnSize;

            DataGridViewColumn SUNwrr4 = ReliefDGv1.Columns["SUN"];
            SUNwrr4.Width = ColumnSize;
            


           



            
        }

        private void cancelbtn_Click(object sender, EventArgs e)
        {
            DataTable WRR1 = this.dbaccess.DataSet.Tables["WRR1"];
            DataTable WRR2 = this.dbaccess.DataSet.Tables["WRR2"];
            DataTable WRR3 = this.dbaccess.DataSet.Tables["WRR3"];
            DataTable WRR4 = this.dbaccess.DataSet.Tables["WRR4"];

            if (ATMRdatasource != null)
            {
                if (ATMRdatasource.Rows.Count > 0)
                {
                    ATMRdatasource.Clear();
                }
            }

            if (WRR1 != null)
            {
                if (WRR1.Rows.Count > 0)
                {
                    WRR1.Clear();
                }
            }

            if (WRR2 != null)
            {
                if (WRR2.Rows.Count > 0)
                {
                    WRR2.Clear();
                }
            }

            if (WRR3 != null)
            {
                if (WRR3.Rows.Count > 0)
                {
                    WRR3.Clear();
                }
            }

            if (WRR4 != null)
            {
                if (WRR4.Rows.Count > 0)
                {
                    WRR4.Clear();
                }
            }

            SiteCodeTb.Text = string.Empty;
            SiteNameTb.Text = string.Empty;
            SectorTb.Text = string.Empty;
            WRRrefNoTb.Text = string.Empty;

            EmpTb.Text = string.Empty;
            EmpNameTb.Text = string.Empty;
        }

    }
}