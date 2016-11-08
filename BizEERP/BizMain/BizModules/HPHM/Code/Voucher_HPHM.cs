/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_HPHM.cs
 *	Description:    H/R Public Holiday Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 *
***********************************************************/

using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizAccounts;

namespace ATL.HPHM
{
	public class Voucher_HPHM : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_HPHM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_HPHM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow hphm = e.DBAccess.DataSet.Tables["hphm"].Rows[0];
			DataTable hphd = e.DBAccess.DataSet.Tables["hphd"];

			if(hphm["year"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Year' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

			DataSet hphmTmp = e.DBAccess.ReadSQLTemp("hphmTmp","SELECT * FROM hphmTmp WHERE refnum<>'"+hphm["refnum"].ToString().Trim()+"' AND year='"+hphm["year"].ToString().Trim()+"'");
			if(hphmTmp.Tables["hphmTmp"].Rows.Count != 0)
			{
				MessageBox.Show("Duplicate Field 'Year' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

			foreach(DataRow dr in hphd.Rows)
			{
				if(Convert.IsDBNull(dr["hdate"]))
				{
					MessageBox.Show("Empty Field 'Date' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Handle = false;
				}
			}

			string sqlcommand = "SELECT COUNT(hdate) AS cnt FROM [hphd] GROUP BY hdate HAVING COUNT(hdate)>1";
			DataTable dt = BizFunctions.ExecuteQuery(e.DBAccess.DataSet, sqlcommand);
			if(dt.Rows.Count!=0)
			{
				MessageBox.Show("Duplicate Dates Selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
		}

		#endregion
		
		#region Document Event

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick (sender, e);
			DataRow hphm = e.DBAccess.DataSet.Tables["hphm"].Rows[0];
			DataTable hphd = e.DBAccess.DataSet.Tables["hphd"];

			foreach(DataRow dr in hphd.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(hphm, dr, "refnum/year/status/flag/user/modified");
					dr["hdate"] = BizFunctions.GetStandardDateString((DateTime)dr["hdate"]);
				}
			}
		}

		#endregion
	}
}
