using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ATL.BizModules.FINRPT.Code
{
    public partial class Period_Selection : Form
    {
        public Boolean execute = false;
        public string pdFrom = "";
        public string pdTo = "";

        public Period_Selection()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            pdFrom = cmb_pdFrom.SelectedItem.ToString();
            pdTo = cmb_pdTo.SelectedItem.ToString();
            execute = true;
            this.Close();

        }

        private void Period_Selection_Load(object sender, EventArgs e)
        {
            cmb_pdFrom.SelectedIndex = 0;
            cmb_pdTo.SelectedIndex = 0;
        }

        private void Period_Selection_Closed(object sender, EventArgs e)
        {
            cmb_pdFrom.SelectedIndex = 0;
            cmb_pdTo.SelectedIndex = 0;
        }
    }
}