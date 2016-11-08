/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_EXRATE.cs
 *	Description:    Currency Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * 
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.EXRATE
{
	public class Voucher_EXRATE : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_EXRATE(string moduleName, Hashtable voucherBaseHelpers): base("VoucherGridInfo_EXRATE.xml", moduleName, voucherBaseHelpers)
		{
		}

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);

			(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Confirm") as Button).Enabled = false;
			(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Void") as Button).Enabled = false;
		}

        #region Document Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow exr = e.DBAccess.DataSet.Tables["exr"].Rows[0];

            if (BizFunctions.IsEmpty(exr["curname"]))
            {
				MessageBox.Show("Empty Field 'Currency Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }
        }

		#endregion 
	}
}
