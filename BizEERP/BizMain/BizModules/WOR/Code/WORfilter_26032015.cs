using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing.Printing;


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


namespace ATL.WOR
{
    public partial class WORfilter1 : Form
    {
        private DBAccess dbAccess = null;
        private DataTable siteDataTable, siteRange,siteMaterials = null;
        private string[] arr1;
        private string[,,] arr2;
        protected string projectPath;
        protected ATL.BizModules.Tools.CRForm crpt1 = null;
        protected int selectReportID;
        protected string ReportLocation;

        public WORfilter1(DBAccess dbAcc, string[,,] Array)
        {
            InitializeComponent();
            this.dbAccess = dbAcc;
            this.arr2 = Array;
            GetSites();
            SetListBoxValues();
            txt_SiteFrom.DoubleClick += new EventHandler(txt_SiteFrom_DoubleClick);
            txt_SiteFrom.KeyDown += new KeyEventHandler(txt_SiteFrom_KeyDown);
            txt_SiteTo.DoubleClick += new EventHandler(txt_SiteTo_DoubleClick);
            txt_SiteTo.KeyDown += new KeyEventHandler(txt_SiteTo_KeyDown);
            cb_AllLocation.Checked = true;
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
            rptListBox.SelectedIndex = 0;
            
        }

        void txt_SiteTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                string siteList = "";
                for (int i = 0; i < arr1.Length; i++)
                {
                    siteList = siteList + arr1[i].ToString();
                }

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum in (" + siteList + ") ", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    txt_SiteTo.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();     
                }

            }
        }

        void txt_SiteTo_DoubleClick(object sender, EventArgs e)
        {
            string siteList = "";
            for (int i = 0; i < arr1.Length; i++)
            {
                siteList = siteList + arr1[i].ToString();
            }

            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum in (" + siteList + ") ", null, F2Type.Sort);

            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                txt_SiteTo.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
            }

        }

        void txt_SiteFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                string siteList = "";
                for (int i = 0; i < arr1.Length; i++)
                {
                    siteList = siteList + arr1[i];
                }

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum in (" + siteList + ") ", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    txt_SiteFrom.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
                }

            }

        }

        void txt_SiteFrom_DoubleClick(object sender, EventArgs e)
        {
            string siteList = "";
            for (int i = 0; i < arr1.Length; i++)
            {
                siteList = siteList + arr1[i].ToString();
            }

            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITM.xml", e, "sitenum", "sitenum in (" + siteList + ") ", null, F2Type.Sort);

            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                txt_SiteFrom.Text = f2BaseHelper.F2Base.CurrentRow["sitenum"].ToString();
            }

        }

     

        private bool validsiteRange()
        {
            bool isValid = false;

            if (siteRange.Rows.Count > 0)
            {
                isValid = true;
            }

            return isValid;
        }

        private void btn_Preview_Click(object sender, EventArgs e)
        {
            if (siteDataTable.Rows.Count > 0)
            {
                this.crpt1 = new ATL.BizModules.Tools.CRForm();
                if (cb_AllLocation.Checked)
                {
                    string siteList = "";
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        siteList = siteList + arr1[i].ToString();
                    }
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum in (" + siteList + ")");

                }
                else if (txt_SiteFrom.Text.Trim() != string.Empty && txt_SiteTo.Text == string.Empty)
                {
                    txt_SiteTo.Text = txt_SiteFrom.Text;
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");
                }
                else if (txt_SiteFrom.Text.Trim() == string.Empty && txt_SiteTo.Text != string.Empty)
                {
                    txt_SiteFrom.Text = txt_SiteTo.Text;
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");
                }
                else
                {
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");

                }

                if (validsiteRange())
                {
                    foreach (DataRow dr1 in siteRange.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {

                            siteMaterials = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from tra1 where sitenum='" + dr1["sitenum"].ToString() + "'");

                            siteMaterials.TableName = "siteMaterials";

                            if (this.dbAccess.DataSet.Tables.Contains("siteMaterials"))
                            {
                                this.dbAccess.DataSet.Tables["siteMaterials"].Dispose();
                                this.dbAccess.DataSet.Tables.Remove("siteMaterials");
                                this.dbAccess.DataSet.Tables.Add(siteMaterials);
                            }
                            else
                            {
                                this.dbAccess.DataSet.Tables.Add(siteMaterials);
                            }



                            ReportDocument crReportDocument = new ReportDocument();
                            crReportDocument.Load(this.projectPath + ReportLocation);

                            crReportDocument.SetDataSource(this.dbAccess.DataSet);

                            crpt1.Crv1.ReportSource = crReportDocument;
                            crpt1.ShowDialog();

                        }
                    }

                }

            }

        }

        private void btn_Print_Click(object sender, EventArgs e)
        {
            if (siteDataTable.Rows.Count > 0)
            {
                this.crpt1 = new ATL.BizModules.Tools.CRForm();
                if (cb_AllLocation.Checked)
                {
                    string siteList = "";
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        siteList = siteList + arr1[i].ToString();
                    }
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum in (" + siteList + ")");

                }
                else if (txt_SiteFrom.Text.Trim() != string.Empty && txt_SiteTo.Text == string.Empty)
                {
                    txt_SiteTo.Text = txt_SiteFrom.Text;
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");
                }
                else if (txt_SiteFrom.Text.Trim() == string.Empty && txt_SiteTo.Text != string.Empty)
                {
                    txt_SiteFrom.Text = txt_SiteTo.Text;
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");
                }
                else
                {
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");

                }

                if (validsiteRange())
                {
                    foreach (DataRow dr1 in siteRange.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            siteMaterials = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from tra1 where sitenum='" + dr1["sitenum"].ToString() + "'");
                            //siteMaterials = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from dtAllPdTask");

                            siteMaterials.TableName = "siteMaterials";

                            if (this.dbAccess.DataSet.Tables.Contains("siteMaterials"))
                            {
                                this.dbAccess.DataSet.Tables["siteMaterials"].Dispose();
                                this.dbAccess.DataSet.Tables.Remove("siteMaterials");
                                this.dbAccess.DataSet.Tables.Add(siteMaterials);
                            }
                            else
                            {
                                this.dbAccess.DataSet.Tables.Add(siteMaterials);
                            }

                            ReportDocument crReportDocument = new ReportDocument();
                            crReportDocument.Load(this.projectPath + ReportLocation);

                            crReportDocument.SetDataSource(this.dbAccess.DataSet);

                            PrintDocument printDocument = new PrintDocument();
                            crReportDocument.PrintOptions.PrinterName = printDocument.PrinterSettings.PrinterName;               

                            crReportDocument.PrintToPrinter(1, true, 0, 0);
                            printDocument.Dispose();

                        }
                    }

                }

            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cb_AllLocation_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_AllLocation.Checked == true)
            {
                groupBox1.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
            }
        }

        private void txt_SiteFrom_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_SiteTo_TextChanged(object sender, EventArgs e)
        {

        }

        private void GetSites()
        {
            siteDataTable = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select distinct sitenum from TRA1");
            siteDataTable.TableName = "siteDataTable";

            if (this.dbAccess.DataSet.Tables.Contains("siteDataTable"))
            {
                this.dbAccess.DataSet.Tables["siteDataTable"].Dispose();
                this.dbAccess.DataSet.Tables.Remove("siteDataTable");
                this.dbAccess.DataSet.Tables.Add(siteDataTable);
            }
            else
            {
                this.dbAccess.DataSet.Tables.Add(siteDataTable);
            }

            if (siteDataTable.Rows.Count > 0)
            {
                arr1 = new string[siteDataTable.Rows.Count];

                for (int i = 0; i < siteDataTable.Rows.Count; i++)
                {
                    if (siteDataTable.Rows[i].RowState != DataRowState.Deleted)
                    {

                        if (siteDataTable.Rows.Count > 1 && i < siteDataTable.Rows.Count-1)
                        {
                            arr1[i] = "'" + siteDataTable.Rows[i]["sitenum"].ToString() + "',";
                        }
                        else
                        {
                            arr1[i] = "'" + siteDataTable.Rows[i]["sitenum"].ToString() + "'";
                        }
                    }
                }               
            }
        }

        private void SetListBoxValues()
        {
            string[] reportName = new string[arr2.GetLength(1)];
            this.rptListBox.FormattingEnabled = true;
            this.rptListBox.HorizontalScrollbar = true;           

            for (int a = 0; a < arr2.GetLength(1); a++)
            {
                int stpoint = 0;
                for (int b = 0; b < arr2.GetLength(2)-1; b++)
                {
                    if (stpoint == 0)
                    {
                        reportName[a] = "(" + arr2[0, a, b] + ") - ";
                    }
                    else
                    {
                        reportName[a] = reportName[a] + arr2[0, a, b];  
                    }
                    stpoint++;
                }

            }

            rptListBox.Items.AddRange(reportName);

            this.rptListBox.ScrollAlwaysVisible = true;
            this.rptListBox.TabIndex = 0;
            this.rptListBox.ColumnWidth = 85;

         
        }

        private void rptListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int x = (rptListBox.SelectedIndex); x >= 0; x--)
            {
                selectReportID = rptListBox.SelectedIndex;
            }
                                 
            ReportLocation = arr2[0,selectReportID,2];
                           
        }


    }
}