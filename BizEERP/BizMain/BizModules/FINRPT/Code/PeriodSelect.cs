using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ATL.BizModules.FINRPT.Code
{
    public partial class PeriodSelect : Form
    {
        public string sPeriod = string.Empty;
        public Boolean execute = false;

        public PeriodSelect()
        {
            InitializeComponent();
        }

        private void PeriodSelect_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            
        }

        private void PeriodSelect_Closed(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            sPeriod = comboBox1.SelectedItem.ToString();
            execute = true;
            this.Close();
        }
    }
}