using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizDetail;
using BizRAD.BizApplication;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizTools;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizAccounts;

namespace ATL.WAC
{
    public partial class WAC_Main : Form
    {
		DBAccess dbAccess = new DBAccess();
		protected string company = "";

		public WAC_Main(string moduleName)
        {
            InitializeComponent();
			this.company = moduleName;
        }

		private void WAC_Main_Load(object sender, EventArgs e)
		{
			LoadMATS();
		}

		private void LoadMATS()
		{
			string strSQL = "SELECT " +
							" mats.matnum, matm.matname, mats.std0, " +
							" mats.std1, mats.std2, mats.std3, mats.std4," +
							" mats.std5, mats.std6, mats.std7, mats.std8," +
							" mats.std9, mats.std10, mats.std11, " +
							" mats.std12 " +
					" FROM " +
							" MATS" + Common.DEFAULT_SYSTEM_YEAR +
							" AS mats INNER JOIN " +
							" matm ON mats.matnum = matm.matnum " +
					" WHERE " +
							" ISNULL(mats.MATNUM,'') LIKE '%" + txtMatnum.Text.Replace("'","''") + "%'" +
							" AND ISNULL(MATNAME,'') LIKE '%" + txtMatname.Text.Replace("'","''") + "%'" +
							" AND ISNULL(mats.coy,'') = '" + company + "'" +
					" ORDER BY " +
							" MATNAME ";

			DataSet DS = dbAccess.ReadSQLTemp("wac", strSQL);
			BindingSource BindSourceWAC = new BindingSource();
			BindSourceWAC.DataSource = DS.Tables["wac"];

			dgWac.DataSource = BindSourceWAC;
			dgWac.Refresh();
		}

		private void cmdClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cmdNew_Click(object sender, EventArgs e)
		{
			Form frmDocs = new WAC_Document(company, true,-1);

			frmDocs.ShowDialog();
			LoadMATS();
		}

		private void txtMatnum_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				LoadMATS();
			}
		}

		private void View_Click(object sender, EventArgs e)
		{
			if (cboPeriod.SelectedIndex == -1)
			{
				MessageBox.Show("Please select period.", "System Message");
				return;
			}
			if (isValidPeriod())
			{
				Form frmDocs = new WAC_Document(company,false, Convert.ToInt32(cboPeriod.Text));

				frmDocs.ShowDialog();
			}
			else
			{
				MessageBox.Show("No Record for selected period.", "System Message",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
		}

		private bool isValidPeriod()
		{
			string strSQL = "SELECT TOP 1 * FROM WAC" + Common.DEFAULT_SYSTEM_YEAR +
				" WHERE PERIOD=" + cboPeriod.Text;

			DataSet dsWAC = this.dbAccess.ReadSQLTemp("WAC", strSQL);
			DataTable dtWAC = dsWAC.Tables["WAC"];

			if (dtWAC.Rows.Count > 0)
				return true;
			else
				return false;
		}
    }
}