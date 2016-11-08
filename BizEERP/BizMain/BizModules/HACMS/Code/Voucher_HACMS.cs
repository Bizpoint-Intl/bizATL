/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_HACMS.cs
 *	Description:    H/R Appraisal Code Main Section Master
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
using BizRAD.DB.Client;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizAccounts;

namespace ATL.HACMS
{
	public class Voucher_HACMS : BizRAD.BizApplication.VoucherBaseHelper
	{
		protected DBAccess dbAccess = null;

		public Voucher_HACMS(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_HACMS.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region DocumentPageEvent

		protected override void AddDocumentPageEventTarget(object sender, PageEventArgs e)
		{
			base.AddDocumentPageEventTarget (sender, e);
			switch(e.PageName)
			{
				case "header":
					e.EventTarget = new Header_HACMS(e.DBAccess, e.FormsCollection, e.DocumentKey);
					break;
			}
		}

		#endregion

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow hacms = e.DBAccess.DataSet.Tables["hacms"].Rows[0];
			DataTable hacss = e.DBAccess.DataSet.Tables["hacss"];
			DataTable hacm = e.DBAccess.DataSet.Tables["hacm"];

			if(hacms["acmsname"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Description' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

			foreach(DataRow dr in hacss.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					if(dr["acssname"].ToString().Trim() == String.Empty)
					{
						MessageBox.Show("Empty Field 'Description' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						e.Handle = false;
					}
					else if(BizFunctions.IsEmpty(dr["acssnum"]))
					{
						MessageBox.Show("No Code Defined for "+dr["acssname"].ToString().Trim()+" !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						e.Handle = false;
					}
					else if(BizFunctions.IsEmpty(dr["acsscode"]))
					{
						MessageBox.Show("Appraisal Code Category cannot be Empty !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						e.Handle = false;
					}
				}
			}

			foreach(DataRow dr in hacm.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					if(dr["apccode"].ToString().Trim() == String.Empty)
					{
						MessageBox.Show("Empty Field 'Appraisal Code' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						e.Handle = false;
					}
				}
			}
		}

		#endregion

		#region Document Event

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad (sender, e);
			DataTable hacm = e.DBAccess.DataSet.Tables["hacm"];

			DataRow [] hacm_array = hacm.Select("","apccode");
			for(int i=0; i<hacm_array.Length; i++)
			{
				hacm_array[i]["line"] = i+1;
			}									 
			
			this.dbAccess = e.DBAccess;

			e.DBAccess.DataSet.Tables["hacss"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_HACSS_ColumnChanged);
		}

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick (sender, e);
			DataRow hacms = e.DBAccess.DataSet.Tables["hacms"].Rows[0];
			DataTable hacss = e.DBAccess.DataSet.Tables["hacss"];
			DataTable hacm = e.DBAccess.DataSet.Tables["hacm"];

			foreach(DataRow dr in hacm.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					dr["apccode"] = dr["acsscode"].ToString().Trim() + "-" + dr["apcnum"].ToString().Trim();
				}
			}
		}

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick (sender, e);
			DataRow hacms = e.DBAccess.DataSet.Tables["hacms"].Rows[0];
			DataTable hacss = e.DBAccess.DataSet.Tables["hacss"];
			DataTable hacm = e.DBAccess.DataSet.Tables["hacm"];
			
			foreach(DataRow dr in hacss.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(hacms, dr, "acmsnum/user/flag/status/created/modified");
				}
			}

			foreach(DataRow dr in hacm.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(hacms, dr, "acmsnum/user/flag/status/created/modified");
				}
			}
		}

		#endregion
		
		#region ColumnChangedEvents

		#region hacss

		private void Voucher_HACSS_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{
				case "acssnum":
					#region set acsscode

					DataTable dtTmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "SELECT COUNT(*) FROM [hacss] WHERE acssnum='"+e.Row["acssnum"].ToString().Trim()+"'  HAVING COUNT(*)>1");
					if(dtTmp.Rows.Count != 0)
					{
						MessageBox.Show("Repeated Code !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						e.Row["acsscode"] = "";
					}
					else
					{
						e.Row["acsscode"] = this.dbAccess.DataSet.Tables["hacms"].Rows[0]["acmsnum"].ToString().Trim() + e.Row["acssnum"].ToString().Trim();
					}
					break;

					#endregion
			}
		}

		#endregion

		#endregion
	}
}
