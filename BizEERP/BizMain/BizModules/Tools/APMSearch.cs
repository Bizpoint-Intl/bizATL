using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;

namespace ATL.SupplierSearch
{
    public partial class APMSearch : Form
    {
        string strSupplierCode;
        string[] strReturnSupCode;
        protected DBAccess dbAccess = null;
        string strPage;

        public APMSearch()
        {
            InitializeComponent();
        }

        private void APMSearch_Load(object sender, EventArgs e)
        {
            txtApNum.Text = strSupplierCode;
            txtPage.Text = "1";
            strPage = "1";
            MakeSearch();

            txtApNum.KeyDown += new KeyEventHandler(txtApNum_KeyDown);
            txtApName.KeyDown += new KeyEventHandler(txtApName_KeyDown);
            dgSupplier.KeyDown += new KeyEventHandler(dgSupplier_KeyDown);
        }

        private void dgSupplier_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FinalizeSearch();
            }
        }

        private void txtApNum_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FinalizeSearch();
            }
        }

        private void txtApName_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FinalizeSearch();
            }
        }

        private void FinalizeSearch()
        {
            if (dgSupplier.Rows.Count > 0)
            {
                int intCurrentRow = dgSupplier.SelectedCells[0].RowIndex;
                strReturnSupCode = new string[] { dgSupplier.Rows[intCurrentRow].Cells[0].Value.ToString(), //apcode
													dgSupplier.Rows[intCurrentRow].Cells[1].Value.ToString()//apname
													};
            }
            else
                strReturnSupCode = new string[] { "", "" };

            this.Close();
        }

        public string[] GetSupplier(string strSupCode, DBAccess dbaccess)
        {
            this.dbAccess = dbaccess;
            strSupplierCode = strSupCode;
            //initialize value to empty string
            strReturnSupCode = new string[] { "", ""};
            this.ShowDialog();
            return strReturnSupCode;
        }

        private void MakeSearch()
        {
            SetupPage();

            int intStartRow = 0;
            int intEndRow = 0;

            intEndRow = Convert.ToInt16(txtPage.Text) * 10;
            intStartRow = intEndRow - 9;

            string strSQL = "WITH [APM ORDERED BY ROWID] AS " +
                "(SELECT ROW_NUMBER() OVER (ORDER BY APNUM ASC) AS ROWID, " +
                " * FROM APM WHERE ACTIVE=1 AND APNUM LIKE '" + txtApNum.Text + "%' " +
                " AND APNAME LIKE '" + txtApName.Text + "%') " +
                        " SELECT APNUM,APNAME " +
                        " FROM [APM ORDERED BY ROWID] " +
                        " WHERE ROWID BETWEEN " + intStartRow + " AND " + intEndRow +
                        " ORDER BY APNUM ";

            DataSet DS = dbAccess.ReadSQLTemp("tmpAPM", strSQL);
            BindingSource BindSourceAPM = new BindingSource();
            BindSourceAPM.DataSource = DS.Tables["tmpAPM"];
            
            dgSupplier.DataSource = BindSourceAPM;
            dgSupplier.Refresh();
        }

        private void SetupPage()
        {
            string strSQL = "SElECT Count(*) TotalPage FROM APM " +
                " WHERE ACTIVE=1 AND APNUM LIKE '" + txtApNum.Text + "%' " +
                " AND APNAME LIKE '" + txtApName.Text + "%'";

            DataSet dsAPM = dbAccess.ReadSQLTemp("APMTemp", strSQL);
            DataTable dtAPM = dsAPM.Tables["APMTemp"];

            DataRow DR = dtAPM.Rows[0];
            decimal decTotalPage = Convert.ToDecimal(DR["TotalPage"].ToString());
            int intPage = (int)(decTotalPage / 10);
            if ((decTotalPage % 10) != 0)
            {
                intPage = intPage + 1;
            }
            txtTotalPage.Text = intPage.ToString();
        }

        private void txtApNum_TextChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            MakeSearch();
        }

        private void txtApName_TextChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            MakeSearch();
        }

        private bool isValidPage()
        {
            if (IsNumeric(txtPage.Text))
            {
                if ((Convert.ToInt16(txtPage.Text) > Convert.ToInt16(txtTotalPage.Text)) || (Convert.ToInt16(txtPage.Text) <= 0))
                {
                    MessageBox.Show("Invalid Page Number.", "System Message");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Invalid Page Number.", "System Message");
                return false;
            }

            return true;
        }

        private void cmdGo_Click(object sender, EventArgs e)
        {
            if (isValidPage())
            {
                MakeSearch();
                strPage = txtPage.Text;
            }
            else
                txtPage.Text = strPage;
        }

        internal static bool IsNumeric(object ObjectToTest)
        {
            if (ObjectToTest == null)
            {
                return false;
            }
            else
            {
                double OutValue;
                return double.TryParse(ObjectToTest.ToString().Trim(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.CurrentCulture,
                    out OutValue);
            }
        }

        private void cmdFirst_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            strPage = "1";
            MakeSearch();
        }

        private void cmdPrevious_Click(object sender, EventArgs e)
        {
            if (txtPage.Text == "1") return;
            txtPage.Text = Convert.ToString(Convert.ToInt16(txtPage.Text) - 1);
            strPage = txtPage.Text;
            MakeSearch();
        }

        private void cmdNext_Click(object sender, EventArgs e)
        {
            if (txtPage.Text == txtTotalPage.Text) return;
            txtPage.Text = Convert.ToString(Convert.ToInt16(txtPage.Text) + 1);
            strPage = txtPage.Text;
            MakeSearch();
        }

        private void cmdLast_Click(object sender, EventArgs e)
        {
            txtPage.Text = txtTotalPage.Text;
            strPage = txtPage.Text;
            MakeSearch();
        }

        private void dgSupplier_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            FinalizeSearch();
        }

           

    }
}