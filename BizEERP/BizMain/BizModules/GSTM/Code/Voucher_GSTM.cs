/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_UOMM.cs
 *	Description:    Unit Of Measurement Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Joseph           080124              Unit Of Measurement Master
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizTools;
using BizRAD.BizAccounts;

namespace ATL.GSTM
{
	public class Voucher_GSTM : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_GSTM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_GSTM.xml", moduleName, voucherBaseHelpers)
        {
        }

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);

			ComboBox gsttype = BizXmlReader.CurrentInstance.GetControl((e.FormsCollection["header"] as Form).Name, "GSTM_gsttype") as ComboBox;
			gsttype.SelectedValue = "Taxable";

			(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Confirm") as Button).Enabled = false;
			(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Void") as Button).Enabled = false;

		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
			DataRow gstm = e.DBAccess.DataSet.Tables["gstm"].Rows[0];

			if (BizFunctions.IsEmpty(gstm["gstgrpname"]))
			{
				MessageBox.Show("Empty Field 'GST Grp Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

			if (BizFunctions.IsEmpty(gstm["gsttype"]))
			{
				MessageBox.Show("Empty Field 'GST Type' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}


		}

		#endregion
	}
}
