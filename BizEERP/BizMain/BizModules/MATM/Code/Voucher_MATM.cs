/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_matm.cs
 *	Description:    H/R matnum Master
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

namespace ATL.MATM
{
	public class Voucher_MATM: BizRAD.BizApplication.VoucherBaseHelper
	{
        string mtype = "HR";
		public Voucher_MATM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_MATM.xml", moduleName, voucherBaseHelpers)
		{
            
		}

		#region Document Handle

        #region Voucher Condition
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {
            //For status 'O' & 'P'
            base.AddVoucherAllCondition(e);
            e.Condition = " MATM.mtype='"+mtype+"' ";
        }
        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {
            //For status 'O' & 'V'
            base.AddVoucherDefaultCondition(e);
            e.Condition = "MATM.mtype='"+mtype+"' and MATM.[status]='O'  ";

        }
        #endregion

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow matm = e.DBAccess.DataSet.Tables["matm"].Rows[0];

			if(matm["matname"].ToString().Trim() == String.Empty)
			{
				MessageBox.Show("Empty Field 'Description' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

            matm["mtype"] = mtype;
           
		}

		#endregion

	}
}
