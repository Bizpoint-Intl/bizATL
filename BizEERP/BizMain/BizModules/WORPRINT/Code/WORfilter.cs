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
    public partial class WORfilter : Form
    {
        private DBAccess dbAccess = null;
        private DataTable siteDataTable, siteRange, sitePeriodic, sigTB2,WOR42 = null;
        private string[] arr1;
        protected string projectPath;
        protected ATL.BizModules.Tools.CRForm crpt1 = null; 

        public WORfilter(DBAccess dbAcc)
        {
            InitializeComponent();
            this.dbAccess = dbAcc;
            GetSites();
            txt_SiteFrom.DoubleClick += new EventHandler(txt_SiteFrom_DoubleClick);
            txt_SiteFrom.KeyDown += new KeyEventHandler(txt_SiteFrom_KeyDown);
            txt_SiteTo.DoubleClick += new EventHandler(txt_SiteTo_DoubleClick);
            txt_SiteTo.KeyDown += new KeyEventHandler(txt_SiteTo_KeyDown);
            cb_AllLocation.Checked = true;
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
            
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
                if (isCompleted.Checked)
                {
                    string siteList = "";
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        siteList = siteList + arr1[i].ToString();
                    }
                    DataTable dtTemp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where worstatus='COMPLETED' and sitenum in (" + siteList + ")");
                    
                
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
                else if (txt_SiteFrom.Text.Trim() != string.Empty && txt_SiteTo.Text != string.Empty)
                {
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");

                }

                if (validsiteRange())
                {
                    foreach (DataRow dr1 in siteRange.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {

           
                            //sitePeriodic = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from dtAllPdTask where [Project Site]='" + dr1["sitenum"].ToString() + "'");               

                            //sitePeriodic.TableName = "sitePeriodic";

                            //sigTB2 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from SigTB where sitenum='" + dr1["sitenum"].ToString() + "'");

                            //sigTB2.TableName = "sigTB2";

                            //WOR42 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from WOR4 where sitenum='" + dr1["sitenum"].ToString() + "'");

                            //WOR42.TableName = "wor42";

               
                            //string getSiteInfo = "select a.arname,s.sitename,s.rep1,s.rep1tel,s.empname from sitm s "+
                            //                        "left join arm a on a.arnum=s.arnum "+
                            //                        "where s.sitenum='" + dr1["sitenum"].ToString() + "' and s.[status]<>'V'";

                            //this.dbAccess.ReadSQL("Header", getSiteInfo);

                            //DataTable Header = this.dbAccess.DataSet.Tables["Header"];

                            //if (this.dbAccess.DataSet.Tables.Contains("Header"))
                            //{
                            //    this.dbAccess.DataSet.Tables["Header"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("Header");
                            //    this.dbAccess.DataSet.Tables.Add(Header);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(Header);
                            //}


                            //if (this.dbAccess.DataSet.Tables.Contains("sitePeriodic"))
                            //{
                            //    this.dbAccess.DataSet.Tables["sitePeriodic"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("sitePeriodic");
                            //    this.dbAccess.DataSet.Tables.Add(sitePeriodic);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(sitePeriodic);
                            //}

                            //if (this.dbAccess.DataSet.Tables.Contains("sigTB2"))
                            //{
                            //    this.dbAccess.DataSet.Tables["sigTB2"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("sigTB2");
                            //    this.dbAccess.DataSet.Tables.Add(sigTB2);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(sigTB2);
                            //}

                            //if (this.dbAccess.DataSet.Tables.Contains("wor42"))
                            //{
                            //    this.dbAccess.DataSet.Tables["wor42"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("wor42");
                            //    this.dbAccess.DataSet.Tables.Add(WOR42);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(WOR42);
                            //}



                            ReportDocument crReportDocument = new ReportDocument();
                            crReportDocument.Load(this.projectPath + @"\SVC\Report\PestControlReportForm1.rpt");

                            crReportDocument.SetDataSource(this.dbAccess.DataSet);

                            crpt1.Crv1.ReportSource = crReportDocument;
                            crpt1.ShowDialog();
                                              
                        }
                    }
             
                }

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
                else if (txt_SiteFrom.Text.Trim() != string.Empty && txt_SiteTo.Text != string.Empty)
                {
                    siteRange = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select sitenum from siteDataTable where sitenum>='" + txt_SiteFrom.Text.Trim() + "' and sitenum<='" + txt_SiteTo.Text.Trim() + "'");

                }

                if (validsiteRange())
                {
                    foreach (DataRow dr1 in siteRange.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            

                            //sitePeriodic = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from dtAllPdTask where [Project Site]='" + dr1["sitenum"].ToString() + "'");

                            //sitePeriodic.TableName = "sitePeriodic";

                            //sigTB2 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from SigTB where sitenum='" + dr1["sitenum"].ToString() + "'");

                            //sigTB2.TableName = "sigTB2";

                            //WOR42 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select * from WOR4 where sitenum='" + dr1["sitenum"].ToString() + "'");

                            //WOR42.TableName = "wor42";


                            //string getSiteInfo = "select a.arname,s.sitename,s.rep1,s.rep1tel,s.empname from sitm s " +
                            //                        "left join arm a on a.arnum=s.arnum " +
                            //                        "where s.sitenum='" + dr1["sitenum"].ToString() + "' and s.[status]<>'V'";

                            //this.dbAccess.ReadSQL("Header", getSiteInfo);

                            //DataTable Header = this.dbAccess.DataSet.Tables["Header"];

                            //if (this.dbAccess.DataSet.Tables.Contains("Header"))
                            //{
                            //    this.dbAccess.DataSet.Tables["Header"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("Header");
                            //    this.dbAccess.DataSet.Tables.Add(Header);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(Header);
                            //}


                            //if (this.dbAccess.DataSet.Tables.Contains("sitePeriodic"))
                            //{
                            //    this.dbAccess.DataSet.Tables["sitePeriodic"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("sitePeriodic");
                            //    this.dbAccess.DataSet.Tables.Add(sitePeriodic);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(sitePeriodic);
                            //}

                            //if (this.dbAccess.DataSet.Tables.Contains("sigTB2"))
                            //{
                            //    this.dbAccess.DataSet.Tables["sigTB2"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("sigTB2");
                            //    this.dbAccess.DataSet.Tables.Add(sigTB2);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(sigTB2);
                            //}

                            //if (this.dbAccess.DataSet.Tables.Contains("wor42"))
                            //{
                            //    this.dbAccess.DataSet.Tables["wor42"].Dispose();
                            //    this.dbAccess.DataSet.Tables.Remove("wor42");
                            //    this.dbAccess.DataSet.Tables.Add(WOR42);
                            //}
                            //else
                            //{
                            //    this.dbAccess.DataSet.Tables.Add(WOR42);
                            //}



                            ReportDocument crReportDocument = new ReportDocument();
                            crReportDocument.Load(this.projectPath + @"\WOR\Report\WorFrm.rpt");

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
                isCompleted.Checked = false;
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
            siteDataTable = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select distinct sitenum from WOR1");
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

        private void isCompleted_CheckedChanged(object sender, EventArgs e)
        {
            if (isCompleted.Checked == true)
            {
                cb_AllLocation.Checked = false;
                groupBox1.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
            }
           
        }


    }
}