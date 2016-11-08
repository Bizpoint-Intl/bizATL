/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_HNAM.cs
 *	Description:    Region Master
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

namespace ATL.HNAM
{
	public class Voucher_HNAM : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_HNAM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_HNAM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow hnam =  e.DBAccess.DataSet.Tables["hnam"].Rows[0];
			/*************************************************
			*	Author:			Jer
			*	Description:	Force Users to key in Region Name
			*************************************************/
			if(hnam["nationality"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Region Name' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

            if (BizFunctions.IsEmpty(hnam["expire"]))
            {
                hnam["expire"] = 0;
            }
/*
			foreach(DataRow dataRow in e.DBAccess.DataSet.Tables["regd"].Rows)
			{
				if(dataRow.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(hnam, dataRow, "nationalitycode/status/user/modified");
				}
			}
*/
		}

		#endregion
	}
}
