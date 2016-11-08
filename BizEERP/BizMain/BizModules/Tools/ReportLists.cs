using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

using BizRAD.BizXml;
using BizRAD.DB.Client;
using BizRAD.BizDocument;
using BizRAD.DB.Interface;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizCommon;


using BizRAD.BizControls.DataGridColumns;
using System.Net.Mail;
using System.Threading;
using System.Text.RegularExpressions;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;


namespace ATL.ReportLists
{
    public partial class Reports : Form
    {
        #region Class Variables
        Boolean checkcsoReport;
        protected string reportType;
        protected string reportName;
        protected DBAccess dbAccess;
        protected DataTable standardReports;
        private string xsdFileName;
        private string returnedTableName;
        private string filterColumnNames;
        private string rptFileName;
        private string projectPath;
        private string FilterColumnNames;
        private string Filterkey;
        #endregion

        #region Constructor
        public Reports(Boolean CSOReport,string Module,string ReportName,string key)
        {
            checkcsoReport = CSOReport;
            this.reportType = Module;
            this.reportName = ReportName;
            this.Filterkey = key;
            InitializeComponent();
            projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
        }
        #endregion

        #region Form On load
        private void Report_Load(object sender, EventArgs e)
        {
           
            bool parameterValue = false;
            this.dbAccess = new DBAccess();
            Parameter[] parameters = new Parameter[] { new Parameter("@ReportType", this.reportType), new Parameter("@IsError", parameterValue) };
            parameters[1].Output = true;
            parameters[1].SqlDbType = SqlDbType.Bit;
            
            try
            {
                DataSet storedProcedureResult = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_VoucherReports", ref parameters);
                this.standardReports = storedProcedureResult.Tables[0];
                if (standardReports.Rows.Count > 0)
                {
                    dataGridView1.Columns.Add("reportdescription", "Report Description");
                    dataGridView1.Columns["reportdescription"].Width = 150;

                    dataGridView1.Rows.Add(standardReports.Rows.Count);

                    for (int i = 0; i <= standardReports.Rows.Count - 1; i++)
                    {
                        dataGridView1.Rows[i].Cells[0].Value = standardReports.Rows[i]["reportdescription"].ToString();
                    }
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }


        }
        #endregion

        #region Execute Stored Procedure
        protected DataSet ExecuteReportSP(string sp_Name)
        {
            #region for use of multiple clauses
            //string rowFilter = this.GetRowFilter();
            //rowFilter = (rowFilter == string.Empty) ? rowFilter : (" WHERE " + rowFilter +"='" + Filterkey + "'");
            #endregion

            string rowFilter;
            rowFilter = " WHERE " + filterColumnNames + "='" + Filterkey + "'";
            Parameter[] parameters = new Parameter[] { new Parameter("LoginSystemYear", Common.DEFAULT_SYSTEM_YEAR),new Parameter("LoginUserName", Common.DEFAULT_SYSTEM_USERNAME), new Parameter("WhereClause", rowFilter),new Parameter("refnum",Filterkey)};
            try
            {
                DataSet storedProcedureResult = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult(sp_Name, ref parameters);
                if (storedProcedureResult == null)
                {
                    MessageBox.Show("Criteria did not return any result set!", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
                storedProcedureResult.DataSetName = this.xsdFileName;

                string[] sTemp = this.returnedTableName.Split('|');

                for (int i = 0; i <= sTemp.Length - 1; i++)
                {
                    string test = sTemp[i].ToString();
                    storedProcedureResult.Tables[i].TableName = sTemp[i].ToString();
                }
                //storedProcedureResult.Tables[0].TableName = this.returnedTableName;


                return storedProcedureResult;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return null;
            }
        }
        #endregion

        #region Could be used for muliple where clauses
        //protected string GetRowFilter()
        //{
            //string str = string.Empty;
            //if (((this.cmb_ColumnName1.SelectedItem != null) && (this.cmb_Filter1.SelectedItem != null)) && (this.tb_Value1.Text != string.Empty))
            //{
            //    str = this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName1.SelectedItem).GetValue(), this.cmb_Filter1.SelectedItem.ToString(), this.tb_Value1.Text);
            //}
            //if (((this.cmb_ColumnName2.SelectedItem != null) && (this.cmb_Filter2.SelectedItem != null)) && (this.tb_Value2.Text != string.Empty))
            //{
            //    if (str != string.Empty)
            //    {
            //        str = str + " AND ";
            //    }
            //    str = str + this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName2.SelectedItem).GetValue(), this.cmb_Filter2.SelectedItem.ToString(), this.tb_Value2.Text);
            //}
            //if (((this.cmb_ColumnName3.SelectedItem != null) && (this.cmb_Filter3.SelectedItem != null)) && (this.tb_Value3.Text != string.Empty))
            //{
            //    if (str != string.Empty)
            //    {
            //        str = str + " AND ";
            //    }
            //    str = str + this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName3.SelectedItem).GetValue(), this.cmb_Filter3.SelectedItem.ToString(), this.tb_Value3.Text);
            //}
            //if (((this.cmb_ColumnName4.SelectedItem != null) && (this.cmb_Filter4.SelectedItem != null)) && (this.tb_Value4.Text != string.Empty))
            //{
            //    if (str != string.Empty)
            //    {
            //        str = str + " AND ";
            //    }
            //    str = str + this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName4.SelectedItem).GetValue(), this.cmb_Filter4.SelectedItem.ToString(), this.tb_Value4.Text);
            //}
            //if (((this.cmb_ColumnName5.SelectedItem == null) || (this.cmb_Filter5.SelectedItem == null)) || !(this.tb_Value5.Text != string.Empty))
            //{
            //    return str;
            //}
            //if (str != string.Empty)
            //{
            //    str = str + " AND ";
            //}
            //return (str + this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName5.SelectedItem).GetValue(), this.cmb_Filter5.SelectedItem.ToString(), this.tb_Value5.Text));

        //    return str;
        //}
        #endregion

        #region Open Clicked
        private void btnOpen_Click(object sender, EventArgs e)
        {         
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (dr.Selected)
                {
                    int selected = dr.Index;
                    for (int x = 0; x <= standardReports.Rows.Count - 1; x++)
                    {
                        if (x == selected)
                        {
                            foreach (DataRow dr1 in standardReports.Rows)
                            {
                                if (dr1["reportdescription"].ToString().Trim() == dataGridView1.Rows[x].Cells["reportdescription"].Value.ToString().Trim())
                                {
                                    xsdFileName = dr1["xsdFileName"].ToString();
                                    returnedTableName = dr1["ReturnedTableName"].ToString();
                                    filterColumnNames = dr1["FilterColumnNames"].ToString();
                                    rptFileName = dr1["RptFileName"].ToString();
                                    filterColumnNames = dr1["FilterColumnNames"].ToString();
                                    DataSet dt = ExecuteReportSP(dr1["StoredProcedureName"].ToString());
                                    if(dt != null)
                                    {
                                    DispalyCR(projectPath, reportName, rptFileName, sender, dt);
                                    dt.Dispose();
                                    }
                                    else
                                    {                                   
                                        return;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    
                }
           }
       }
        #endregion

        #region Display Crystal Report
       private static void DispalyCR(string Ppath,string FolderName, string ReportName,object targetEvent, DataSet source)
        {

            Form form = BizXmlReader.CurrentInstance.Load(Ppath + @"\\Tools\\FormPreviewWithCancel.xml", "formPreview", targetEvent, null) as Form;

            CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;
            ReportDocument crReportDocument = new ReportDocument();

            crReportDocument.Load(Ppath + @"\\" + FolderName + "\\REPORT\\" + ReportName +".rpt");
            crReportDocument.SetDataSource(source);

            crystalReportViewer1.ReportSource = crReportDocument;
            form.ShowDialog();
            form.Dispose();

        }
        #endregion

        #region Cancel Clicked
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

    }
}