/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_STM.cs
 *	Description:    Payment Term Master
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

namespace ATL.WHM
{
	public class Voucher_WHM : BizRAD.BizApplication.VoucherBaseHelper
	{
        protected bool opened = false;
		public Voucher_WHM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_WHM.xml", moduleName, voucherBaseHelpers)
		{
		}

        protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_Edit_Handle(sender, e);

            if (opened)
            {
                MessageBox.Show("You cannot open two documents at the same time.\n" +
                    "To Edit/New Document, either close the document that's currently open for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            e.Handle = !opened;
        }


        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);

            opened = false;
        }

        protected override void Voucher_New_Handle(object sender, VoucherHandleEventArgs e)
        {
            base.Voucher_New_Handle(sender, e);
            if (opened)
            {
                MessageBox.Show("You cannot open two documents at the same time.\n" +
                    "To Edit/New Document, either close the document that's currently open for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            e.Handle = !opened;
        }

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);
            opened = true;
			(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Confirm") as Button).Enabled = false;
			(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Void") as Button).Enabled = false;
		}
		
		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);

			DataRow whm = e.DBAccess.DataSet.Tables["whm"].Rows[0];

			if (BizFunctions.IsEmpty(whm["whname"]))
			{
				MessageBox.Show("Empty Field 'Warehouse Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}


            if (BizFunctions.IsEmpty(whm["pempnum"]))
            {
                whm["ptc"] = System.DBNull.Value;
                whm["hp"] = System.DBNull.Value;
                whm["fulladdress"] = System.DBNull.Value;
            }
		}

		#endregion


        #region DetailF2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            switch (e.MappingName)
            {
                case "matnum":
                    e.Condition = BizFunctions.F2Condition("matnum,matname", (sender as TextBox).Text);
                    break;
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            switch (e.MappingName)
            {
                
            }
        }
        #endregion

        #region DocumentF2

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
               
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
  
            switch (e.ControlName)
            {
                case "whm_pempnum":
                    {
                        e.CurrentRow["ptc"] = e.F2CurrentRow["empname"];
                        e.CurrentRow["hp"] = e.F2CurrentRow["contact"];
                    }
                    break;

               

            }
        }

        #endregion


	}
}