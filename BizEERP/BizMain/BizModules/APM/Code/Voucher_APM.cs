/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_APM.cs
 *	Description:    Supplier Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		20070122			Start 
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.BizVoucher;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.APM
{
	public class Voucher_APM : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Variables
		protected DBAccess dbaccess;
		#endregion
		#region Constructor

		public Voucher_APM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_APM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#endregion

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);
			dbaccess = e.DBAccess;
			DataRow apm = dbaccess.DataSet.Tables["apm"].Rows[0];

			if (Convert.IsDBNull(apm["active"])) apm["active"] = 1;
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
			DataRow apm = e.DBAccess.DataSet.Tables["APM"].Rows[0];

			//if (BizFunctions.IsEmpty(apm["apname"]))
			//{
			//    MessageBox.Show("Empty Field 'Supplier Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    e.Handle = false;
			//    return;
			//}

			//if (!BizFunctions.IsEmpty(apm["accnum"]))
			//{
			//    if (!BizValidate.CheckTableIsValid(e.DBAccess, "acm", "accnum", apm["accnum"].ToString()))
			//    {
			//        MessageBox.Show("Invalid 'A/C Code' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//        e.Handle = false;
			//        return;
			//    }
			//}

			//if (!BizFunctions.IsEmpty(apm["oricur"]))
			//{
			//    if (!BizValidate.CheckTableIsValid(e.DBAccess, "exr", "oricur", apm["oricur"].ToString()))
			//    {
			//        MessageBox.Show("Invalid 'Currency Code' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//        e.Handle = false;
			//        return;
			//    }
			//}
		}
		
		#endregion

		#region Document F2

		protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
		{
			base.AddDocumentF2Condition(sender, e);

			switch (e.ControlName)
			{
				case "apm_catnum":
					e.Condition = BizFunctions.F2Condition("catnum", (sender as TextBox).Text);
					break;
			}
		}

		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);

			switch (e.ControlName)
			{
				case "apm_catnum":
					e.CurrentRow["catname"] = e.F2CurrentRow["catname"];
					break;
			}
		}

		#endregion

	}
}

