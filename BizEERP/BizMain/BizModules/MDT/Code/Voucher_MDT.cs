/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_MDT.cs
 *	Description:    Module Definition Table
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * ChzeKeong        071211              Start
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

namespace DEMO.MDT
{
	public class Voucher_MDT : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Global Variables

		bool	isNew		 = false;
	
		// Do these really need to be static?
		// It is reset to 0 on refresh everytime. Leave for safety.
		static decimal appricelocal = 0;
		static decimal apfpricelocal = 0;

		protected DBAccess dbaccess  = null;
		protected Hashtable formsCollection = null;

		#endregion

        public Voucher_MDT(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_MDT.xml", moduleName, voucherBaseHelpers)
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
		}

		#region Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);

            Hashtable selectsCollection = new Hashtable();
			this.dbaccess	 = e.DBAccess;
			this.formsCollection = e.FormsCollection;

			e.DBAccess.DataSet.Tables["mdt1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_MDT1_ColumnChanged);

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

		protected void Voucher_MDT1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{

			switch (e.Column.ColumnName)
			{
				case "tdtnum":
					DataSet Ds = dbaccess.ReadSQLTemp("TDTH", "Select tdtname from TDTH where tdtnum='" + e.Row["tdtnum"].ToString().Trim() + "'");
					e.Row["tdtname"] = Ds.Tables["TDTH"].Rows[0][0];
					break;
			}
		}

		#endregion

		#endregion	
	}
}