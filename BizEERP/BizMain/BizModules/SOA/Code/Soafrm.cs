using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CrystalDecisions.Windows.Forms;

namespace ATL.SOA
{
    public partial class Soafrm : Form
    {
        public Label labelDescription = null; 
        public Button buttonQuit = null;
        public CrystalReportViewer crystalReportViewer = null;

        public Soafrm()
        {
            InitializeComponent();

            this.labelDescription = lbl_description;
            this.buttonQuit = btnQuit;
            this.crystalReportViewer = this.statementpreview;
        }
    }
}