using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;

namespace BizRAD.BizAccounts
{
    public partial class StandardReports : Form
    {
        protected DBAccess dbAccess = null;
        protected Form frmThis = null;
        protected DataTable standardReports = null;
        
        protected string reportType = null;
        protected DataRow selectedRow = null;

        public StandardReports(string reportType)
        {
            InitializeComponent();

            this.frmThis = this;
            this.dbAccess = new DBAccess();
            this.reportType = reportType;

            bool isError = false;
            Parameter[] parameters = new Parameter[2];
            parameters[0] = new Parameter("@ReportType", this.reportType);
            parameters[1] = new Parameter("@IsError", isError);
            
            parameters[1].Output = true;
            parameters[1].SqlDbType = SqlDbType.Bit;

            try
            {
                DataSet ds = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_StandardReports", ref parameters);

                this.standardReports = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.frmThis.Close();
            }
        }

        private void StandardReports_Load(object sender, EventArgs e)
        {
            this.dtp_CutOffDate.Value = System.DateTime.Now;

            foreach (DataRow dr in this.standardReports.Rows)
            {
                this.lb_Reports.Items.Add(dr["ReportName"].ToString());
            }

            this.lb_Reports.SelectedIndexChanged += new EventHandler(lb_Reports_SelectedIndexChanged);

            if(this.standardReports.Rows.Count > 0)
                this.lb_Reports.SelectedIndex = 0;
        }

        protected void lb_Reports_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRow [] selectedReport = this.standardReports.Select("ReportName = '" + this.lb_Reports.SelectedItem.ToString()+"'");
            this.lbl_description.Text = selectedReport[0]["ReportDescription"].ToString();
            this.selectedRow = selectedReport[0];
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.frmThis.Dispose();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            StandardReportsFilter standardReportsFilter = new StandardReportsFilter(this.dbAccess, this.selectedRow, this.reportType, this.dtp_CutOffDate.Value, false);
            standardReportsFilter.ShowDialog();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            StandardReportsFilter standardReportsFilter = new StandardReportsFilter(this.dbAccess, this.selectedRow, this.reportType, this.dtp_CutOffDate.Value, true);
            standardReportsFilter.ShowDialog();
        }
    }
}