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



namespace ATL.PBRD
{
	public class Voucher_PBRD : BizRAD.BizApplication.VoucherBaseHelper
	{
		protected bool opened = false;
		protected DataRow pbrd;


		public Voucher_PBRD(string moduleName, Hashtable voucherBaseHelpers)
			: base("VoucherGridInfo_PBRD.xml", moduleName, voucherBaseHelpers)
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
			e.OrderClause = "active desc,pbrdcode";
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

			this.pbrd = e.DBAccess.DataSet.Tables["pbrdm"].Rows[0];
			if (this.pbrd["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
			{
				this.pbrd["active"] = true;		//set default to Active!
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

			if (BizFunctions.IsEmpty(this.pbrd["pbrdname"]))
			{
				MessageBox.Show("Empty Field 'Brand Description' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
		}

		#endregion

	}
}

