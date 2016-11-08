/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_EXR.cs
 *	Description:    Currency Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Jer		        20070209		    Start
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
using BizRAD.BizCommon;

namespace ATL.EXR
{
	public class Voucher_EXR : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Constructor

		public Voucher_EXR(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_EXR.xml", moduleName, voucherBaseHelpers)
		{
		}

		#endregion

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
            DataRow exr = e.DBAccess.DataSet.Tables["exr"].Rows[0];

			if (exr["curname"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Currency Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
		}

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);

			DataRow exr = e.DBAccess.DataSet.Tables["exr"].Rows[0];

			if (exr["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
			{
				exr["expire"] = false;
			}
		}

		protected override void Document_Void_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Void_Handle(sender, e);

			DataRow exr = e.DBAccess.DataSet.Tables["exr"].Rows[0];

			exr["expire"] = true;

		}
		#endregion
	}
}

