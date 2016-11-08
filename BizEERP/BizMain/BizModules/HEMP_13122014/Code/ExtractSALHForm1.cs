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
           

           
         
        }



        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
            return;
        }


   
       
    }
}