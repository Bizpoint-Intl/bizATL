using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ATL.FAV
{
    public partial class frmImport : Form
    {
        //steve amended on 19-May-2010 for cater import txt changes
        //bool blnSflag = false;
        int blnSflag;
        public bool isClose = true;
        public frmImport(string flag)
        {            
            InitializeComponent();
            if (flag != "FAV")
                btn_Opera.Visible = false;
        }
        public int isStandard()
        {
            ShowDialog();
            return blnSflag;
        }

        private void btn_Standard_Click(object sender, EventArgs e)
        {
            blnSflag = 1;
            isClose = false;
            Close();
        }

        private void btn_Opera_Click(object sender, EventArgs e)
        {
            blnSflag = 2;
            isClose = false;
            Close();
        }

        private void frmImport_FormClosed(object sender, FormClosedEventArgs e)
        {
            //isClose;
        }

        private void btn_txt_Click(object sender, EventArgs e)
        {
            blnSflag = 3;
            isClose = false;
            Close();

        }
        
    }
}