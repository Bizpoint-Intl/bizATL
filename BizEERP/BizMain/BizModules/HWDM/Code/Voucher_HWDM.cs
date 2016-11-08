/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_HWDM.cs
 *	Description:    Leave Scheme Master
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

namespace ATL.HWDM
{
	public class Voucher_HWDM : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_HWDM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_HWDM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow hwdm = e.DBAccess.DataSet.Tables["hwdm"].Rows[0];

            if (hwdm["nwdname"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Description' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
		}

		#endregion

		#region Document Event

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick (sender, e);
			DataRow hwdm = e.DBAccess.DataSet.Tables["hwdm"].Rows[0];

			hwdm.BeginEdit();

            hwdm["cmth1"] = (decimal) hwdm["pmth1"] + (decimal)hwdm["mth1"];
			hwdm["cmth2"] = (decimal) hwdm["pmth2"] + (decimal) hwdm["mth2"];
			hwdm["cmth3"] = (decimal) hwdm["pmth3"] + (decimal) hwdm["mth3"];
			hwdm["cmth4"] = (decimal) hwdm["pmth4"] + (decimal) hwdm["mth4"];
			hwdm["cmth5"] = (decimal) hwdm["pmth5"] + (decimal) hwdm["mth5"];
			hwdm["cmth6"] = (decimal) hwdm["pmth6"] + (decimal) hwdm["mth6"];
			hwdm["cmth7"] = (decimal) hwdm["pmth7"] + (decimal) hwdm["mth7"];
			hwdm["cmth8"] = (decimal) hwdm["pmth8"] + (decimal) hwdm["mth8"];
			hwdm["cmth9"] = (decimal) hwdm["pmth9"] + (decimal) hwdm["mth9"];
			hwdm["cmth10"] = (decimal) hwdm["pmth10"] + (decimal) hwdm["mth10"];
			hwdm["cmth11"] = (decimal) hwdm["pmth11"] + (decimal) hwdm["mth11"];
			hwdm["cmth12"] = (decimal) hwdm["pmth12"] + (decimal) hwdm["mth12"];

			hwdm.EndEdit();
		}

		#endregion
	}
}
