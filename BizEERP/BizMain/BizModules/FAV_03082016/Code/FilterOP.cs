using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ATL.CustomerSearch;

namespace ATL.FilterOP
{
	public partial class getFilterOP : Form
	{
		
		public string CodeFromValue = "";
		public string CodeToValue = "";
		
		public getFilterOP()
		{
			InitializeComponent();

			CodeFrom.MouseDoubleClick += new MouseEventHandler(CodeFrom_MouseDoubleClick);
			CodeFrom.KeyDown += new KeyEventHandler(CodeFrom_KeyDown);

		}

		void CodeFrom_KeyDown(object sender, KeyEventArgs e)
		{
			SearchARM();
		}

		void CodeFrom_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			SearchARM();
		}

		private void SearchARM()
		{
			//ARMSearch frmCustSearch = new ARMSearch();
			//string[] strARM = frmCustSearch.GetCustomer(CodeFrom.Text.Trim(),);

			//if (strARM[0].ToString() == "") return;

			//CodeFrom.Text = strARM[0].ToString();
			//CodeFrom.Text = strARM[0].ToString();
			//CodeTo.Text= strARM[1].ToString();
		}

		
		private void btnExtract_Click(object sender, EventArgs e)
		{
			//getAROpening();
			this.Close();
			CodeFromValue = CodeFrom.Text.Trim();
			CodeFromValue = CodeTo.Text.Trim();
			if (CodeTo.Text.Trim() != "")
			{
				CodeToValue = CodeTo.Text.Trim();
			}
			else
			{
				CodeToValue = "ZZZZZZZZZZZZZZZZ";
			}
		
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

			
	}
}