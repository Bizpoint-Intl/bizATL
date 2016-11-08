using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

namespace ATL.AccDefaults
{
	public partial class AccountDefaults : Form
	{
		protected DBAccess dbaccess = null;

		public AccountDefaults()
		{
			InitializeComponent();

			this.dbaccess = new DBAccess();
		}

		private void AccountDefaults_Load(object sender, EventArgs e)
		{
			LoadAssignmentToGrid();

		}

		private void LoadAssignmentToGrid()
		{
			string strSQL;
			strSQL = "SELECT * FROM defaultvalue ORDER BY flag,page,field";
			DataSet dsAcc = this.dbaccess.ReadSQLTemp("ACC", strSQL);
			DataTable dtAcc = dsAcc.Tables["ACC"];
			BindingSource BindSourceACC = new BindingSource();
			BindSourceACC.DataSource = dtAcc;

			dg_accDef.AutoGenerateColumns = false;
			dg_accDef.DataSource = BindSourceACC;
			dg_accDef.Refresh();
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cmdUpdate_Click(object sender, EventArgs e)
		{
			string strFlag;
			string strPage;
			string strField;
			string strValue;

			foreach (DataGridViewRow DR in dg_accDef.Rows)
			{
				strFlag = DR.Cells["flag"].Value.ToString();
				strPage = DR.Cells["page"].Value.ToString();
				strField = DR.Cells["field"].Value.ToString();
				strValue = DR.Cells["value"].Value.ToString();

				//string strSQLUpdate = "UPDATE defaultvalue " +
				//        " SET field='" + strField + "'," +
				//        " value='" + strValue + "'" +
				//        " WHERE flag='" + strFlag + "'" +
				//        " AND page ='" + strPage + "'";

				string strSQLUpdate = "UPDATE defaultvalue " +
						" SET field='" + strField + "'," +
						" value='" + strValue + "'" +
						" WHERE flag='" + strFlag + "'" +
						" AND page ='" + strPage + "'" +
						" AND field ='" + strField + "'";

				if (strFlag == "GST1")
				{
					string strSQLUpdate2 = "UPDATE ACC " +
							"SET ACCNUM='" + strValue + "'" +
							"WHERE REFNUM ='GST1'";
					this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(strSQLUpdate2);
				}

				if (strFlag == "GST2")
				{
					string strSQLUpdate3 = "UPDATE ACC " +
							"SET ACCNUM='" + strValue + "'" +
							"WHERE REFNUM ='GST2'";
					this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(strSQLUpdate3);
				}
				//MessageBox.Show(strSQLUpdate);

				this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(strSQLUpdate);


			}
			MessageBox.Show("Update Successful!");
			this.Close();
		}
	}
}