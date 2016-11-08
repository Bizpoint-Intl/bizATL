/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_TDT.cs
 *	Description:    Table Definition Table
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * ChzeKeong        071122              Start
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizCommon;
using BizRAD.BizAccounts;
using BizRAD.BizControls.BizDateTimePicker;

namespace ATL.TDT
{
	public class Voucher_TDT : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Global Variables

		// Do these really need to be static?
		// It is reset to 0 on refresh everytime. Leave for safety.
	

		protected DBAccess dbaccess  = null;
		protected Hashtable formsCollection = null;

		#endregion

        public Voucher_TDT(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_TDT.xml", moduleName, voucherBaseHelpers)
		{
		}

        #region Voucher Default/All Condition

        protected override void AddVoucherDefaultCondition(BizRAD.BizVoucher.VoucherConditionEventArgs e)
        {
            base.AddVoucherDefaultCondition (e);
        }

        #endregion

        #region Voucher Handle
        protected override void Voucher_Delete_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_Delete_Handle(sender, e);
            e.Handle = false;
        }
        #endregion


        #region Document Handles

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

        }

        #endregion

		#region Document Events

		protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
		{
			base.AddDetailF3Condition(sender, e);

			switch (e.MappingName)
			{
				case "accnum":
					e.CurrentRow["accname"] = e.F2CurrentRow["accname"];
					e.CurrentRow["acctype"] = e.F2CurrentRow["acctype"];
					break;
			}
		}

		#region Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);

            Hashtable selectsCollection = new Hashtable();
			this.dbaccess	 = e.DBAccess;
			this.formsCollection = e.FormsCollection;

        }

        #endregion    

		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);

        }


		#endregion

        #region SaveBegin

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);            
        }

        #endregion

        #region SaveEnd

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
           
        }

        #endregion

        #region Delete

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);
        }

        #endregion

		#region Column Change events


		#endregion

		#endregion	
	}
}