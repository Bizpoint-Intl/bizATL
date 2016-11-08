/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_CCM.cs
 *	Description:    Cost Center Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		20070103			Start 
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

namespace ATL.CCM
{
	public class Voucher_CCM : BizRAD.BizApplication.VoucherBaseHelper
	{

		#region Constructor

		public Voucher_CCM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_CCM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#endregion

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
			DataRow ccm = e.DBAccess.DataSet.Tables["CCM"].Rows[0];

			if (BizFunctions.IsEmpty(ccm["ccname"]))
			{
				MessageBox.Show("Empty Field 'Cost Center Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
                return;
			}

            //if (ccm["cctype"].ToString().Trim() == String.Empty)
            //{
            //    MessageBox.Show("Empty Field 'Cost Center Type' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //    return;
            //}
		}

		#endregion
	}
}

