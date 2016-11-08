/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_view.cs
 *	Description:    View Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Wern Sern        20070917            Creation
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.VIEWM
{
	public class Voucher_VIEW : BizRAD.BizApplication.VoucherBaseHelper
	{
		DBAccess dbaccess = null;
		string formName = null;

		public Voucher_VIEW(string moduleName, Hashtable voucherBaseHelpers)
			: base("VoucherGridInfo_VIEWM.xml", moduleName, voucherBaseHelpers)
		{
		}

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);
						
			this.formName = (e.FormsCollection["header"] as Form).Name;
			dbaccess = e.DBAccess;

			Button Btn_TestView = BizXmlReader.CurrentInstance.GetControl(formName, "Btn_TestView") as Button;
			Btn_TestView.Click += new EventHandler(Btn_TestView_Click);
		}

		void Btn_TestView_Click(object sender, EventArgs e)
		{
			if (testView())
			{
				MessageBox.Show("View is ok!");
			}
			else
			{
				MessageBox.Show("View did not return any rows");
			}
		}

		private bool testView()
		{
			DataRow viewm = dbaccess.DataSet.Tables["viewm"].Rows[0];

			string dropviewsql = "DROP VIEW " + viewm["viewcode"].ToString().Trim();

			//steph - to allow go thru if it's !YEAR!
			string checkViewStatement = viewm["viewstatement"].ToString().Trim().Replace("!YEAR!", Common.DEFAULT_SYSTEM_YEAR);

			string createviewsql = "CREATE VIEW " + viewm["viewcode"].ToString().Trim() + " AS " + checkViewStatement;

			//string createviewsql = "CREATE VIEW " + viewm["viewcode"].ToString().Trim() + " AS " + viewm["viewstatement"].ToString().Trim();

			dbaccess.RemoteStandardSQL.ExecuteNonQuery(dropviewsql);
			dbaccess.RemoteStandardSQL.ExecuteNonQuery(createviewsql);
			try
			{
				dbaccess.ReadSQL("test", "select * from " + viewm["viewcode"].ToString().Trim() +" where 1=2");
			}
			catch
			{
				return false;
			}

			return true;
		}

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
			DataRow viewm = e.DBAccess.DataSet.Tables["viewm"].Rows[0];

			if (BizFunctions.IsEmpty(viewm["viewdescription"]))
			{
				MessageBox.Show("Empty Field 'View Description' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

			if (BizFunctions.IsEmpty(viewm["viewstatement"]))
			{
				MessageBox.Show("Empty Field 'View Statement' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

			if (!testView())
			{
				MessageBox.Show("View did not return any rows");
				e.Handle = false;
				return;
			}

		}

		#endregion


		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick(sender, e);

			DataRow viewm = e.DBAccess.DataSet.Tables["viewm"].Rows[0];

			string dropviewsql = "DROP VIEW " + viewm["viewcode"].ToString().Trim();

			string createviewsql = "CREATE VIEW " + viewm["viewcode"].ToString().Trim() + " AS " + viewm["viewstatement"].ToString().Trim();

			dbaccess.RemoteStandardSQL.ExecuteNonQuery(dropviewsql);
			dbaccess.RemoteStandardSQL.ExecuteNonQuery(createviewsql);

		}


	}
}
