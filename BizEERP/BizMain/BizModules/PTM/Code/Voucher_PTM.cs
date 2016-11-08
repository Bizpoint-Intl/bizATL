/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_PTM.cs
 *	Description:    Payment Term Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Xinyi	        20070213		    Start
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.PTM
{
	public class Voucher_PTM : BizRAD.BizApplication.VoucherBaseHelper
	{

        public Voucher_PTM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_PTM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
            DataRow ptm = e.DBAccess.DataSet.Tables["ptm"].Rows[0];

            if (ptm["ptname"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Payment Term Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
		}

		#endregion
	}
}