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

namespace ATL.BizModules.Tools
{
    public partial class InsertATRForm1 : Form
    {
        protected DBAccess dbaccess = null;
        protected string Empnum, TableName = "";


        public InsertATRForm1(DBAccess da, string empNo, string tableName)
        {
            InitializeComponent();

            this.dbaccess = da;
            this.Empnum = empNo;
            this.TableName = tableName;

        }


        private void InsertLine_Click(object sender, EventArgs e)
        {
            DataTable dt = this.dbaccess.DataSet.Tables[TableName];

            DataRow insertDt = dt.NewRow();

            insertDt["empnum"] = Empnum;

            

            insertDt["date"] = Convert.ToDateTime(dateTimePicker1.Text);
            insertDt["schedtimeout"] = Convert.ToDateTime(dateTimePicker1.Text);
            insertDt["scheddateout"] = Convert.ToDateTime(dateTimePicker1.Text);
            


            //insertDt["DTcolumn"] = Convert.ToDateTime(dateTimePicker1.Text);
            insertDt["sitenum"] = SiteNumTb.Text;
            insertDt["shiftcode"] = shiftcodeTb.Text;
            insertDt["DayAmt"] = Convert.ToDecimal(DayAmtTb.Text);
            if (DayAmtTb.Text == string.Empty)
            {
                insertDt["DayAmt"] = 0;
            }
            else
            {
                insertDt["DayAmt"] = Convert.ToDecimal(DayAmtTb.Text);
            }


            insertDt["schedtimein"] = schedtimeinTb.Text;
            insertDt["schedtimeout"] = schedtimeoutTb.Text;

            insertDt["confirmedtimein"] = schedtimeinTb.Text;
            insertDt["confirmedtimeout"] = schedtimeoutTb.Text;

            if (ActualTotalHrsTb.Text == string.Empty)
            {
                insertDt["ActualTotalHrs"] = 0;
            }
            else
            {
                insertDt["ActualTotalHrs"] = ActualTotalHrsTb.Text;

            }

            if (ActualOTHrsTb.Text == string.Empty)
            {
                insertDt["ActualOTHrs"] = 0;
            }
            else
            {
                insertDt["ActualOTHrs"] = Convert.ToDecimal(ActualOTHrsTb.Text);
            }
            


            if (actualLateMinsTb.Text == string.Empty)
            {
                insertDt["actualLateMins"] = 0;
            }
            else
            {
                insertDt["actualLateMins"] = Convert.ToDecimal(actualLateMinsTb.Text);
            }

            if (actualFixAllowTb.Text == string.Empty)
            {
                insertDt["actualFixAllow"] = 0;
            }
            else
            {
                insertDt["actualFixAllow"] = Convert.ToDecimal(actualFixAllowTb.Text);
            }
          
            insertDt["actualOT1"] = actualOT1cb.Checked;
            insertDt["actualOT15"] = actualOT15cb.Checked;
            insertDt["actualOT2"] = actualOT2cb.Checked;
            insertDt["actualAttnRemark"] = actualAttnRemarktb.Text;

            
//EMPNUM+SITENUM+SHIFTCODE+DATE
            insertDt["uniquekey"] = Empnum+SiteNumTb.Text.Trim()+shiftcodeTb.Text.Trim()+BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text));
            dt.Rows.Add(insertDt);

            this.Dispose();
            return;
            
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

        private void InsertATRForm1_Load(object sender, EventArgs e)
        {          
            SiteNumTb.KeyDown += new KeyEventHandler(SiteNumTb_KeyDown);
            SiteNumTb.DoubleClick += new EventHandler(SiteNumTb_DoubleClick);
            SiteNumTb.BackColor = Color.Yellow;

            shiftcodeTb.KeyDown += new KeyEventHandler(shiftcodeTb_KeyDown);
            shiftcodeTb.DoubleClick += new EventHandler(shiftcodeTb_DoubleClick);
            shiftcodeTb.BackColor = Color.Yellow;

            shiftcodeTb.TextChanged += new EventHandler(shiftcodeTb_TextChanged);
            schedtimeinTb.TextChanged += new EventHandler(schedtimeinTb_TextChanged);
            schedtimeoutTb.TextChanged += new EventHandler(schedtimeoutTb_TextChanged);
            ActualTotalHrsTb.TextChanged += new EventHandler(ActualTotalHrsTb_TextChanged);
            DayAmtTb.TextChanged += new EventHandler(DayAmtTb_TextChanged);
            ActualOTHrsTb.TextChanged += new EventHandler(ActualOTHrsTb_TextChanged);
            actualLateMinsTb.TextChanged += new EventHandler(actualLateMinsTb_TextChanged);
            actualFixAllowTb.TextChanged += new EventHandler(actualFixAllowTb_TextChanged);
            SiteNumTb.TextChanged += new EventHandler(SiteNumTb_TextChanged);

            shiftcodeTb.Leave += new EventHandler(shiftcodeTb_Leave);
            schedtimeinTb.Leave += new EventHandler(schedtimeinTb_Leave);
            schedtimeoutTb.Leave += new EventHandler(schedtimeoutTb_Leave);
            ActualTotalHrsTb.Leave += new EventHandler(ActualTotalHrsTb_Leave);
            DayAmtTb.Leave += new EventHandler(DayAmtTb_Leave);
            ActualOTHrsTb.Leave += new EventHandler(ActualOTHrsTb_Leave);
            actualLateMinsTb.Leave += new EventHandler(actualLateMinsTb_Leave);
            actualFixAllowTb.Leave += new EventHandler(actualFixAllowTb_Leave);
            SiteNumTb.Leave += new EventHandler(SiteNumTb_Leave);


        }

        void SiteNumTb_Leave(object sender, EventArgs e)
        {
            if (!isValidSite(SiteNumTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Site Code");
                SiteNumTb.Text = "";
            }
        }

        void actualFixAllowTb_Leave(object sender, EventArgs e)
        {
            if (!TextValidator.TextValidator.IsvalidDecimal(actualFixAllowTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Decimal Format");
                actualFixAllowTb.Text = "0";
            }      
        }

        void actualLateMinsTb_Leave(object sender, EventArgs e)
        {
            if (!TextValidator.TextValidator.IsvalidDecimal(actualLateMinsTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Decimal Format");
                actualLateMinsTb.Text = "0";
            }  
        }

        void ActualOTHrsTb_Leave(object sender, EventArgs e)
        {
            if (!TextValidator.TextValidator.IsvalidDecimal(ActualOTHrsTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Decimal Format");
                ActualOTHrsTb.Text = "0";
            }  
        }

        void DayAmtTb_Leave(object sender, EventArgs e)
        {
            if (!TextValidator.TextValidator.IsvalidDecimal(DayAmtTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Decimal Format");
                DayAmtTb.Text = "0";
            }  
        }

        void ActualTotalHrsTb_Leave(object sender, EventArgs e)
        {
            if (!TextValidator.TextValidator.IsvalidDecimal(ActualTotalHrsTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Decimal Format");
                ActualTotalHrsTb.Text = "0";
            } 
        }

        void schedtimeoutTb_Leave(object sender, EventArgs e)
        {
            if (!TextValidator.TextValidator.IsvalidMilitaryTime(schedtimeoutTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Military Time Format");
                schedtimeoutTb.Text = string.Empty;
            }  
        }

        void schedtimeinTb_Leave(object sender, EventArgs e)
        {
            if (!TextValidator.TextValidator.IsvalidMilitaryTime(schedtimeinTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Military Time Format");
                schedtimeinTb.Text = string.Empty;
            }  
        }

        void shiftcodeTb_Leave(object sender, EventArgs e)
        {
            if (!isValidShift(shiftcodeTb.Text.Trim()))
            {
                MessageBox.Show("Invalid Shift Code");
                shiftcodeTb.Text = "";
            }
        }

        void SiteNumTb_TextChanged(object sender, EventArgs e)
        {
            
        }

        void actualFixAllowTb_TextChanged(object sender, EventArgs e)
        {
              
        }

        void actualLateMinsTb_TextChanged(object sender, EventArgs e)
        {
           
        }

        void ActualOTHrsTb_TextChanged(object sender, EventArgs e)
        {
           
        }

        void DayAmtTb_TextChanged(object sender, EventArgs e)
        {
          
        }

        void ActualTotalHrsTb_TextChanged(object sender, EventArgs e)
        {
            
        }

        void schedtimeoutTb_TextChanged(object sender, EventArgs e)
        {
           
        }

        void schedtimeinTb_TextChanged(object sender, EventArgs e)
        {
            
        }

        void shiftcodeTb_TextChanged(object sender, EventArgs e)
        {
           
        }

        private bool isValidShift(string shiftcode)
        {
            bool valid = false;

            string GetvSHLV = "select isWorkShift from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetvSHLV);
            if (vSHLVTmp != null)
            {
                if (vSHLVTmp.Rows.Count > 0)
                {
                    valid = true;
                }
            }

            return valid;
        }

        private bool isValidSite(string sitenum)
        {
            bool valid = false;

            string GetSiteNum = "select sitenum from SITM where sitenum='" + sitenum + "'";

            this.dbaccess.ReadSQL("TmpSitnum", GetSiteNum);

            DataTable SiteNumTmp = this.dbaccess.DataSet.Tables["TmpSitnum"];

            //DataTable SiteNumTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetSiteNum);
            if (SiteNumTmp != null)
            {
                if (SiteNumTmp.Rows.Count > 0)
                {
                    valid = true;
                }
            }

            return valid;
        }

        void shiftcodeTb_DoubleClick(object sender, EventArgs e)
        {
            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_vSHLv.xml", e, "shiftcode", "shiftcode like '" + shiftcodeTb.Text.Trim() + "%' ", null, F2Type.Sort);

            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                shiftcodeTb.Text = f2BaseHelper.F2Base.CurrentRow["shiftcode"].ToString();
                schedtimeinTb.Text = f2BaseHelper.F2Base.CurrentRow["timein"].ToString();
                schedtimeoutTb.Text = f2BaseHelper.F2Base.CurrentRow["timeout"].ToString();
                ActualTotalHrsTb.Text = f2BaseHelper.F2Base.CurrentRow["ttlworkinghrs"].ToString();
                DayAmtTb.Text = GetDayAmtShift(shiftcodeTb.Text).ToString();
            }
        }

        void shiftcodeTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_vSHLv.xml", e, "shiftcode", "shiftcode like '" + shiftcodeTb.Text.Trim() + "%' ", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    shiftcodeTb.Text = f2BaseHelper.F2Base.CurrentRow["shiftcode"].ToString();
                    schedtimeinTb.Text = f2BaseHelper.F2Base.CurrentRow["timein"].ToString();
                    schedtimeoutTb.Text = f2BaseHelper.F2Base.CurrentRow["timeout"].ToString();
                    ActualTotalHrsTb.Text = f2BaseHelper.F2Base.CurrentRow["ttlworkinghrs"].ToString();
                    DayAmtTb.Text = GetDayAmtShift(shiftcodeTb.Text).ToString();
                }
            }
        }

        void SiteNumTb_DoubleClick(object sender, EventArgs e)
        {
            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum like '" + SiteNumTb.Text.Trim() + "%' ", null, F2Type.Sort);

            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                SiteNumTb.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
            }
        }

        void SiteNumTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum like '" + SiteNumTb.Text.Trim() + "%' ", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    SiteNumTb.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
                }
            }
        }
    }
}