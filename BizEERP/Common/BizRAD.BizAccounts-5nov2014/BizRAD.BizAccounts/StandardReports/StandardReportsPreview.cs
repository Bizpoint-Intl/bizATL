using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Microsoft.Reporting.WinForms;

namespace BizRAD.BizAccounts
{
    public partial class StandardReportsPreview : Form
    {
        public ReportViewer reportViewer = null;

        public StandardReportsPreview()
        {
            InitializeComponent();
            this.reportViewer = this.reportViewer1;
        }

        private void StandardReportsPreview_Load(object sender, EventArgs e)
        {
            this.reportViewer.ProcessingMode = ProcessingMode.Local;
            this.reportViewer.RefreshReport();
        }

    }
}