/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_HETM.cs
 *	Description:    H/R matnum Master
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

namespace ATL.HETM
{
	public class Voucher_HETM : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_HETM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_HETM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			
		}

		#endregion

	}
}
