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

namespace ATL.CustomerSearch
{
	public partial class ARMSearch : Form
	{
		string strCustomerCode;
		string[] strReturnCustCode;
		protected DBAccess dbAccess = null;
		string strPage;

		public ARMSearch()
		{
			InitializeComponent();
		}

		private void ARMSearch_Load(object sender, EventArgs e)
		{
			txtArNum.Text = strCustomerCode;
			txtPage.Text = "1";
			strPage = "1";
			MakeSearch();

			txtArNum.KeyDown += new KeyEventHandler(txtArNum_KeyDown);
			txtArName.KeyDown += new KeyEventHandler(txtArName_KeyDown);
			dgCustomer.KeyDown += new KeyEventHandler(dgCustomer_KeyDown);
		}

		private void dgCustomer_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FinalizeSearch();
			}
		}

		private void txtArNum_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FinalizeSearch();
			}
		}

		private void txtArName_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FinalizeSearch();
			}
		}

		private void FinalizeSearch()
		{
			if (dgCustomer.Rows.Count > 0)
			{
				int intCurrentRow = dgCustomer.SelectedCells[0].RowIndex;
				strReturnCustCode = new string[] { dgCustomer.Rows[intCurrentRow].Cells[0].Value.ToString(), //arcode
													dgCustomer.Rows[intCurrentRow].Cells[1].Value.ToString(),//arname
													dgCustomer.Rows[intCurrentRow].Cells[2].Value.ToString(),//address
													dgCustomer.Rows[intCurrentRow].Cells[3].Value.ToString(),//postal code
													dgCustomer.Rows[intCurrentRow].Cells[4].Value.ToString(),//region
													dgCustomer.Rows[intCurrentRow].Cells[5].Value.ToString(),//ptc
													dgCustomer.Rows[intCurrentRow].Cells[6].Value.ToString(),//phone
													dgCustomer.Rows[intCurrentRow].Cells[7].Value.ToString(),//fax
													dgCustomer.Rows[intCurrentRow].Cells[8].Value.ToString(),//hp
													dgCustomer.Rows[intCurrentRow].Cells[9].Value.ToString(),//email
													dgCustomer.Rows[intCurrentRow].Cells[10].Value.ToString(),//currency
													dgCustomer.Rows[intCurrentRow].Cells[11].Value.ToString(),//gst
													dgCustomer.Rows[intCurrentRow].Cells[12].Value.ToString(),//ptnum
													dgCustomer.Rows[intCurrentRow].Cells[13].Value.ToString(),//ctnum
													dgCustomer.Rows[intCurrentRow].Cells[14].Value.ToString(),//credit
													dgCustomer.Rows[intCurrentRow].Cells[15].Value.ToString(),//salesman
													dgCustomer.Rows[intCurrentRow].Cells[16].Value.ToString(),//includegst
													dgCustomer.Rows[intCurrentRow].Cells[17].Value.ToString() //allow partial
													};
			}
			else
				strReturnCustCode = new string[] { "", "", "", "", "", "", "", "", "", "","","","","","","","","" };

			this.Close();
		}

		public string[] GetCustomer(string strCustCode, DBAccess dbaccess)
		{
			this.dbAccess = dbaccess;
			strCustomerCode = strCustCode;
			//initialize value to empty string
			strReturnCustCode = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
			this.ShowDialog();
			return strReturnCustCode;
		}

		private void MakeSearch()
		{
			SetupPage();

			int intStartRow = 0;
			int intEndRow = 0;

			intEndRow = Convert.ToInt32(txtPage.Text) * 10;
			intStartRow = intEndRow - 9;

			string strSQL = "WITH [ARM ORDERED BY ROWID] AS " +
				"(SELECT ROW_NUMBER() OVER (ORDER BY ARNUM ASC) AS ROWID, " +
				" * FROM ARM WHERE ACTIVE=1 AND ARNUM LIKE '" + txtArNum.Text + "%' " +
                " AND ARNAME LIKE '" + txtArName.Text.Replace("'", "''") + "%') " +
						" SELECT ARNUM,ARNAME,ADDRESS,POSTCODE,REGION, " +
						" PTC,PHONE,FAX,HP,EMAIL,ORICUR,GSTGRPNUM, " +
						" PTNUM, CTNUM, CREDIT, SALESMANEMPNUM, " +
						" INCLUDEGST, ALLOWPARTIAL " +
						" FROM [ARM ORDERED BY ROWID] " +
						" WHERE ROWID BETWEEN " + intStartRow + " AND " + intEndRow +
						" ORDER BY ARNUM ";

			DataSet DS = dbAccess.ReadSQLTemp("tmpARM", strSQL);
			BindingSource BindSourceARM = new BindingSource();
			BindSourceARM.DataSource = DS.Tables["tmpARM"];

			dgCustomer.DataSource = BindSourceARM;
			dgCustomer.Refresh();
		}

		private void SetupPage()
		{
			string strSQL = "SElECT Count(*) TotalPage FROM ARM " +
				" WHERE ACTIVE=1 AND ARNUM LIKE '" + txtArNum.Text + "%' " +
				" AND ARNAME LIKE '" + txtArName.Text.Replace("'","''") + "%'";

			DataSet dsARM = dbAccess.ReadSQLTemp("ARMTemp", strSQL);
			DataTable dtARM = dsARM.Tables["ARMTemp"];

			DataRow DR = dtARM.Rows[0];
			decimal decTotalPage = Convert.ToDecimal(DR["TotalPage"].ToString());
			int intPage = (int)(decTotalPage / 10);
			if ((decTotalPage % 10) != 0)
			{
				intPage = intPage + 1;
			}
			txtTotalPage.Text = intPage.ToString();
		}

		private void txtArNum_TextChanged(object sender, EventArgs e)
		{
			txtPage.Text = "1";
			MakeSearch();
		}

		private void txtArName_TextChanged(object sender, EventArgs e)
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

		private void dgCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			FinalizeSearch();
		}
	}
}