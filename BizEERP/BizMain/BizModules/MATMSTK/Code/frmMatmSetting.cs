using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace ATL.MATMSTK
{
	public partial class frmMatmSetting : Form
	{
		public int Nett = 0;
		public int Floor = 0;


		public frmMatmSetting()
		{
			InitializeComponent();
		}


		private void frmMatmSetting_Load(object sender, EventArgs e)
		{
			this.txtNett.Text = this.Nett.ToString();
			this.txtFloor.Text = this.Floor.ToString();
		}



		private int ValidateValue(string InputString)
		{
			int Value = 0;
			bool convert = false;


			try
			{
				convert = Int32.TryParse(InputString, out Value);
				if (convert == false)
				{
					MessageBox.Show("Invalid Percentage Value!", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					if (Value <= 0)
					{
						MessageBox.Show("Please enter a value > 0!", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			catch
			{ }


			return Value;
		}


		private void btnSave_Click(object sender, EventArgs e)
		{
			this.Nett = this.ValidateValue(this.txtNett.Text);
			if (this.Nett <= 0)
				return;

			this.Floor = this.ValidateValue(this.txtFloor.Text);
			if (this.Floor <= 0)
				return;

			if (this.Nett < this.Floor)
			{
				MessageBox.Show("", "Please enter a value > 0!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}


			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}