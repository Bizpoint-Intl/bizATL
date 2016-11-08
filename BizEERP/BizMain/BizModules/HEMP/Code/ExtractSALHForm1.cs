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

using ATL.BizModules.Tools;

namespace ATL.ExtractSALHForm1
{
    public partial class ExtractSALHForm1 : Form
    {
        protected DBAccess dbaccess = null;
        protected string Empnum,Reference = "";
        protected DataView EmpDV1 = null;
        

        public ExtractSALHForm1(DBAccess da, string empNo,string refnum)
        {
            InitializeComponent();

            this.dbaccess = da;
            this.Empnum = empNo;
            this.Reference = refnum;

           
        }



      

      

        private void ExtractATR1Form1_Load(object sender, EventArgs e)
        {


            string GetSALH = "Select hsamcode,rateamt from salh where empnum='" + Empnum + "' and refnum='" + Reference + "' ORDER BY Line";

            this.dbaccess.ReadSQL("TmpSalh", GetSALH);

            DataTable TmpSalh = this.dbaccess.DataSet.Tables["TmpSalh"];

            if (TmpSalh.Rows.Count > 0)
            {
                DataView dvData = new DataView(TmpSalh);

                BindingSource nbindingSource = new BindingSource();
                nbindingSource.DataSource = dvData;
                EmpDGV1.DataSource = nbindingSource;



                EmpDGV1.Columns["hsamcode"].DefaultCellStyle.BackColor = Color.LightBlue;
                EmpDGV1.Columns["rateamt"].DefaultCellStyle.BackColor = Color.LightBlue;

                EmpDGV1.Columns["hsamcode"].HeaderText = "Desc";
                EmpDGV1.Columns["rateamt"].HeaderText = "Amount($)";

                EmpDGV1.Columns["hsamcode"].ReadOnly = true;
                EmpDGV1.Columns["rateamt"].ReadOnly = true;

                string GetSALHtotal = "Select SUM(rateamt) as Total from salh where empnum='" + Empnum + "' and refnum='" + Reference + "'";
                this.dbaccess.ReadSQL("TotalSalh", GetSALHtotal);

                DataTable TotalSalh = this.dbaccess.DataSet.Tables["TotalSalh"];

                if (TotalSalh.Rows.Count > 0)
                {
                    DataRow dr1 = this.dbaccess.DataSet.Tables["TotalSalh"].Rows[0];

                    if (BizFunctions.IsEmpty(dr1["Total"]))
                    {
                        dr1["Total"] = 0;
                    }
                    TotalTB.Text = "S$ "+Convert.ToString(dr1["Total"]);

                }

            }

            string strOtherData = "select * from sadjh where refnum='"+Reference+"'";

            this.dbaccess.ReadSQL("SalhTmp", strOtherData);

            DataTable SalhTmp = this.dbaccess.DataSet.Tables["SalhTmp"];

            if (SalhTmp.Rows.Count > 0)
            {
                DataRow dr1 = this.dbaccess.DataSet.Tables["SalhTmp"].Rows[0];

                Sitenum.Text = BizLogicTools.Tools.GetSitenname(dr1["newsitenum"].ToString(),this.dbaccess);
                matname.Text = BizLogicTools.Tools.GetEmpname(dr1["newmatnum"].ToString(),this.dbaccess);
                m1.Text = dr1["MonToFriTimeFrom"].ToString();
                m2.Text = dr1["MonToFriTimeTo"].ToString();
                m3.Text = dr1["MonToFriLunchHr"].ToString();
                m4.Text = dr1["MonToFriTeaBrkHr"].ToString();
                m5.Text = dr1["SatTimeFrom"].ToString();
                m6.Text = dr1["SatTimeTo"].ToString();
                m7.Text = dr1["SatLunchHr"].ToString();
                m8.Text = dr1["SatTeaBrkHr"].ToString();
                m9.Text = dr1["SunTimeFrom"].ToString();
                m10.Text = dr1["SunTimeTo"].ToString();
                m11.Text = dr1["SunLunchHr"].ToString();
                m12.Text = dr1["SunTeaBrkHr"].ToString();
                m13.Text = dr1["PHTimeFrom"].ToString();
                m14.Text = dr1["PHTimeTo"].ToString();
                m15.Text = dr1["PHLunchHr"].ToString();
                m16.Text = dr1["PHTeaBrkHr"].ToString();
                m1.Text = dr1["RegularOffDay"].ToString();
                daysperweek.Text = dr1["daysperweek"].ToString();
                recommendtb.Text = dr1["appraisedcomments"].ToString();
                approvetb.Text = dr1["approvedcomments"].ToString();
            }
            else
            {
                DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

                //string getTiming = "SELECT " +
                //                             "[MonToFriTimeFrom] " +
                //                             ",[MonToFriTimeTo] " +
                //                             ",[MonToFriLunchHr] " +
                //                             ",[MonToFriTeaBrkHr] " +
                //                             ",[SatTimeFrom] " +
                //                             ",[SatTimeTo] " +
                //                             ",[SatLunchHr] " +
                //                             ",[SatTeaBrkHr] " +
                //                             ",[SunTimeFrom] " +
                //                             ",[SunTimeTo] " +
                //                             ",[SunLunchHr] " +
                //                             ",[SunTeaBrkHr] " +
                //                             ",[RegularOffDay] " +
                //                             ",[PHTimeFrom] " +
                //                             ",[PHTimeTo] " +
                //                             ",[PHLunchHr] " +
                //                             ",[PHTeaBrkHr] " +
                //                         "FROM HEMPH WHERE EMPNUM='" + SADJh["empnum"].ToString() + "'";


                Sitenum.Text = hemph["sitenum"].ToString();
                matname.Text = hemph["matname"].ToString();
                m1.Text = hemph["MonToFriTimeFrom"].ToString();
                m2.Text = hemph["MonToFriTimeTo"].ToString();
                m3.Text = hemph["MonToFriLunchHr"].ToString();
                m4.Text = hemph["MonToFriTeaBrkHr"].ToString();
                m5.Text = hemph["SatTimeFrom"].ToString();
                m6.Text = hemph["SatTimeTo"].ToString();
                m7.Text = hemph["SatLunchHr"].ToString();
                m8.Text = hemph["SatTeaBrkHr"].ToString();
                m9.Text = hemph["SunTimeFrom"].ToString();
                m10.Text = hemph["SunTimeTo"].ToString();
                m11.Text = hemph["SunLunchHr"].ToString();
                m12.Text = hemph["SunTeaBrkHr"].ToString();
                m13.Text = hemph["PHTimeFrom"].ToString();
                m14.Text = hemph["PHTimeTo"].ToString();
                m15.Text = hemph["PHLunchHr"].ToString();
                m16.Text = hemph["PHTeaBrkHr"].ToString();
                m1.Text = hemph["RegularOffDay"].ToString();
                daysperweek.Text = hemph["daysperweek"].ToString();

            }
           

           
         
        }



        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
            return;
        }


   
       
    }
}