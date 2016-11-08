using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizDetail;
using BizRAD.BizApplication;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizTools;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizAccounts;



namespace ATL.PCAT
{
	public class Voucher_PCAT : BizRAD.BizApplication.VoucherBaseHelper
	{
		protected bool opened = false;
		protected DataRow pcat;


		public Voucher_PCAT(string moduleName, Hashtable voucherBaseHelpers)
			: base("VoucherGridInfo_PCAT.xml", moduleName, voucherBaseHelpers)
		{
		}

		protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
		{
			base.Voucher_Edit_Handle(sender, e);

			if (opened)
			{
				MessageBox.Show("You cannot open two documents at the same time.\n" +
					"To Edit/New a Document, you have to close the document that's currently opened for this module.",
					"System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			e.Handle = !opened;
		}

		protected override void AddVoucherSelectClause(VoucherSelectEventArgs e)
		{
			base.AddVoucherSelectClause(e);

			//sepcify the sorting order of the voucher list
			e.OrderClause = "active desc,pcatcode";
		}


		protected override void Voucher_New_Handle(object sender, VoucherHandleEventArgs e)
		{
			base.Voucher_New_Handle(sender, e);
			if (opened)
			{
				MessageBox.Show("You cannot open two documents at the same time.\n" +
					"To Edit/New a Document, you have to close the document that's currently opened for this module.",
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

			this.pcat = e.DBAccess.DataSet.Tables["pcatm"].Rows[0];
			if (this.pcat["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
			{
				this.pcat["active"] = true;		//set default to Active!
			}
		}


		protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Cancel_OnClick(sender, e);

			opened = false;
		}


		#region Document Save Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
			

			if (BizFunctions.IsEmpty(this.pcat["pcatname"]))
			{
				MessageBox.Show("Empty Field 'Category Description' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}

            //if (BizFunctions.IsEmpty(this.pcat["ptypecode"]))
            //{
            //    MessageBox.Show("Empty Field 'Category Type' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //}
            //else
            //{
            //    //CHECK for F2 TextBox Invalid value!
            //    if (!BizValidate.CheckTableIsValid(e.DBAccess, "ptypem", "ptypecode", this.pcat["ptypecode"].ToString()))
            //    {
            //        MessageBox.Show("Invalid 'Category Type' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //    }
            //}

            //if (BizFunctions.IsEmpty(this.pcat["hscode"]))
            //{
            //    MessageBox.Show("Empty Field 'HS Code' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //}
		}

		#endregion
	}
}

