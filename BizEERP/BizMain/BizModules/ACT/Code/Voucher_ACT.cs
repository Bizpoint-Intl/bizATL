/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_CATM.cs
 *	Description:    Category Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		20/04/07			Start 
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
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
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizAccounts;

namespace ATL.ACT
{
	public class Voucher_ACT : BizRAD.BizApplication.VoucherBaseHelper
	{

		#region Constructor

		public Voucher_ACT(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_ACT.xml", moduleName, voucherBaseHelpers)
		{
		}
		
		#endregion

		#region Voucher Default/ALL

		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);
		}

		#endregion

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
            DataRow acctype = e.DBAccess.DataSet.Tables["acctype"].Rows[0];

			/*************************************************
			*	Author:			Chze Keong
			*	Description:	Force user to key catname
			*************************************************/

            if (acctype["acctypename"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Description' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

		}

		#endregion

		#region Document Event

		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);
		}

		#endregion

		#endregion

	}
}

