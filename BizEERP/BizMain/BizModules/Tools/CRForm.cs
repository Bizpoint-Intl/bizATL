using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;
using BizRAD.BizApplication;
using BizRAD.BizBase;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace ATL.BizModules.Tools
{
    public partial class CRForm : Form
    {
        protected string projectPath,reportPath = null;
        protected DataSet dataSet = null;

        public CRForm()
        {
            InitializeComponent();
            //this.dataSet = ds;
            //this.reportPath = ReportPath;
            //this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
        }

        private void CRForm_Load(object sender, EventArgs e)
        {
            //showReport();
        }

        private void crViewer1_Load(object sender, EventArgs e)
        {

        }

        private void showReport()
        {
            //ReportDocument crReportDocument = new ReportDocument();
            //crReportDocument.Load(projectPath + reportPath);

            //crReportDocument.SetDataSource(dataSet);

            //crViewer1.ReportSource = crReportDocument;

        }

        public CrystalReportViewer Crv1
        {
            get
            {
                return crViewer1;
            }
            set
            {
                crViewer1 = value;
            }
        }
    }
}