/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_CSEH.cs
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

namespace ATL.CSEH
{
	public class Voucher_CSEH : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_CSEH(string moduleName, Hashtable voucherBaseHelpers) : 
            base("VoucherGridInfo_CSEH.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow cseh =  e.DBAccess.DataSet.Tables["cseh"].Rows[0];
			/*************************************************
			*	Author:			Jer
			*	Description:	Force Users to key in Region Name
			*************************************************/
			if(cseh["coursename"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Course Name' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

            if (BizFunctions.IsEmpty(cseh["expire"]))
            {
                cseh["expire"] = 0;
            }
/*
			foreach(DataRow dataRow in e.DBAccess.DataSet.Tables["regd"].Rows)
			{
				if(dataRow.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(cseh, dataRow, "nationalitycode/status/user/modified");
				}
			}
*/
		}

		#endregion
	}
}
