using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ATL.GeneralTools;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;

namespace ATL.FSWRefnum
{
	public partial class Refnum : Form
	{
		protected DBAccess dbAccess;

		public Refnum(DBAccess dbaccess)
		{
			InitializeComponent();			
			dbAccess = dbaccess;
		}

		GenTools genFunctions = new GenTools();

		private void OK_Click(object sender, EventArgs e)
		{
			this.Close();

			DataRow finrpth = dbAccess.DataSet.Tables["finrpth"].Rows[0];
			//DataTable finrpt1 = dbAccess.DataSet.Tables["finrpt1"];
			DataTable finrpt1mirror = dbAccess.DataSet.Tables["finrpt1mirror"];
			DataTable finrpt2 = dbAccess.DataSet.Tables["finrpt2"];

			string strGetFswh = "SELECT * FROM finrpth WHERE refnum ='" + comboBox1.Text + "'";
			//string strGetFsw1 = "SELECT * FROM finrpt1 WHERE refnum ='" + comboBox1.Text + "'";
			string strGetFsw1mirror =  "SELECT * FROM finrpt1mirror WHERE refnum ='" + comboBox1.Text + "'";
			string strGetFsw2 = "SELECT * FROM finrpt2 WHERE refnum ='" + comboBox1.Text + "'";

			dbAccess.ReadSQL("getFswh", strGetFswh);
			//dbAccess.ReadSQL("getFsw1", strGetFsw1);
			dbAccess.ReadSQL("getFsw1Mirror", strGetFsw1mirror);
			dbAccess.ReadSQL("getFsw2", strGetFsw2);

			DataRow getFswh = dbAccess.DataSet.Tables["getFswh"].Rows[0];
			DataTable getFsw1 = dbAccess.DataSet.Tables["getFsw1"];
			DataTable getFsw1Mirror = dbAccess.DataSet.Tables["getFsw1Mirror"];
			DataTable getFsw2 = dbAccess.DataSet.Tables["getFsw2"];

			BizFunctions.UpdateDataRow(getFswh, finrpth,"title/remarks/selectstatus");

			BizFunctions.DeleteAllRows(finrpt1mirror);
			foreach (DataRow dr in getFsw1Mirror.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addFsw1mirror = finrpt1mirror.Rows.Add(new object[] { });
					BizFunctions.UpdateDataRow(dr,addFsw1mirror, "gnum/gname/accnumfrom/accnamefrom/accnumto/accnameto/remark");
					dr["refnum"] = finrpth["refnum"];					
				}
			}

			BizFunctions.DeleteAllRows(finrpt2);
			foreach (DataRow dr in getFsw2.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addFsw2 = finrpt2.Rows.Add(new object[] { });
					BizFunctions.UpdateDataRow(dr, addFsw2,"line/linenum/underline/bold/header/hide/gnum/gname/formula/pd0/pd1/pd2/pd3/pd4/pd5/pd6/pd7/pd8/pd9/pd10/pd11/pd12");
					dr["refnum"] = finrpth["refnum"];
				}
			}
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Refnum_Load(object sender, EventArgs e)
		{
			DataRow finrpth = dbAccess.DataSet.Tables["finrpth"].Rows[0];

			string strGetRefnum = "SELECT refnum FROM finrpth WHERE isnull(refnum,'')<>'"+finrpth["refnum"].ToString().Trim()+"'";
			genFunctions.BindComboBox(comboBox1, strGetRefnum, "REFNUM", "REFNUM");
		}
	}
}