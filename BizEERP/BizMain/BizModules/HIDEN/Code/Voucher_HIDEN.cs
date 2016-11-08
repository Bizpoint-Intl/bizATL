/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_HIDEN.cs
 *	Description:    H/R Identify Type Master
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

namespace ATL.HIDEN
{
	public class Voucher_HIDEN : BizRAD.BizApplication.VoucherBaseHelper
	{
		public Voucher_HIDEN(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_HIDEN.xml", moduleName, voucherBaseHelpers)
		{
        }

        #region Voucher Handle (prevent Delete/New)

        protected override void Voucher_Delete_Handle(object sender, VoucherHandleEventArgs e)
        {
            base.Voucher_Delete_Handle(sender, e);
            MessageBox.Show("System File - Delete has been disabled");
            e.Handle = false;
        }

        protected override void Voucher_New_Handle(object sender, VoucherHandleEventArgs e)
        {
            base.Voucher_New_Handle(sender, e);
            MessageBox.Show("System File - New has been disabled");
            e.Handle = false;
        }

        #endregion

        #region Document Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow hiden = e.DBAccess.DataSet.Tables["hiden"].Rows[0];

			if(hiden["identiname"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Description' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
        }

        #endregion

        #region Document Event

        #region TabControl SelectionChanged

        protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
        {
            base.Document_TabControl_OnSelectionChanged(sender, e);
            #region disable Insert,Delete,Mark,Up,Down,Duplicate buttons

            (this.documentBaseHelpers[e.DocumentKey] as DocumentBaseHelper).DocumentBase.InsertButton.Enabled = false;
            (this.documentBaseHelpers[e.DocumentKey] as DocumentBaseHelper).DocumentBase.DeleteButton.Enabled = false;
            (this.documentBaseHelpers[e.DocumentKey] as DocumentBaseHelper).DocumentBase.MarkButton.Enabled = false;
            (this.documentBaseHelpers[e.DocumentKey] as DocumentBaseHelper).DocumentBase.UpButton.Enabled = false;
            (this.documentBaseHelpers[e.DocumentKey] as DocumentBaseHelper).DocumentBase.DownButton.Enabled = false;
            (this.documentBaseHelpers[e.DocumentKey] as DocumentBaseHelper).DocumentBase.DuplicateButton.Enabled = false;

            #endregion
        }

        #endregion

        #endregion
    }
}