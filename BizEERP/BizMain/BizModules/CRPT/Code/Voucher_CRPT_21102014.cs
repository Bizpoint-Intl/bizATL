/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_CRPT.cs
 *	Description:   Custom Reports
 *	Function List:	
 * 
 * 
 * ---------------------------------------------------------
 * Author			Time				Description
 * Wern Sern      20070917            Custom Reports
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using System.Data.Sql;
using System.Data.SqlClient;


using BizRAD.BizReport;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;

using BizRAD.BizAccounts;
using BizRAD.BizTools;

using ATL.BizLogicTools;
using System.Threading;
using ATL.TimeUtilites;


namespace ATL.CRPT
{
	public class Voucher_CRPT : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Variable
		string posid = ConfigurationManager.AppSettings.Get("POSID");
		protected DBAccess dbaccess = null;
		protected string currentYear = null;
		protected string formName = null;
		protected string formName4SideBtn = null;
		protected bool crpthColumnChange = false;
		protected bool opened = false;
		protected int selectClauseDropLocation = 0;
        protected DataTable SysLocker = null;
        protected DataRow drSysLocker = null;
		#endregion

		#region Construct
		public Voucher_CRPT(string moduleName, Hashtable voucherBaseHelpers)
			: base("VoucherGridInfo_CRPT.xml", moduleName, voucherBaseHelpers)
		{
		}
		#endregion Construct

		#region Voucher Default/All Condition
		protected override void AddVoucherDefaultCondition(BizRAD.BizVoucher.VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);

			if (!Common.DEFAULT_SYSTEM_ISADMINISTRATORGROUP && !Common.DEFAULT_SYSTEM_ISADMINISTRATORUSER)
				e.Condition = "code in (select refnum from syscustomreportsecurity where username ='" + Common.DEFAULT_SYSTEM_USERNAME + "')";
		}

		protected override void AddVoucherAllCondition(BizRAD.BizVoucher.VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);

			if (!Common.DEFAULT_SYSTEM_ISADMINISTRATORGROUP && !Common.DEFAULT_SYSTEM_ISADMINISTRATORUSER)
				e.Condition = "code in (select refnum from syscustomreportsecurity where username ='" + Common.DEFAULT_SYSTEM_USERNAME + "')";
		}
		#endregion

		#region Voucher New/Edit/Cancel	
		protected override void Voucher_New_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
		{
			base.Voucher_New_Handle(sender, e);
			e.Handle = !opened;
		}

		protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
		{
			base.Voucher_Edit_Handle(sender, e);
			e.Handle = !opened;
		}

		protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Cancel_OnClick(sender, e);

			opened = false;
		}
		#endregion
		
		#region DocumentF2
		protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
		{
			base.AddDocumentF2Condition(sender, e);

			switch (e.ControlName)
			{
				// VIEWCODE
				case "crpth_viewcode":
					if (!Common.DEFAULT_SYSTEM_ISADMINISTRATORUSER && !Common.DEFAULT_SYSTEM_ISADMINISTRATORGROUP)
						e.DefaultCondition = "1=2";
					break;
			}
		}
		#endregion 

		#region Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);

			opened = true;

			dbaccess = e.DBAccess;

			this.formName4SideBtn = (sender as Form).Name;
			this.formName = (e.FormsCollection["header"] as Form).Name;

			DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];

			// If this is confirmed or voided voucher. Do not allow column change anymore
			if (crpth["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP || crpth["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSV)
			{
				crpthColumnChange = false;
			}
			else
			{
				crpthColumnChange = true;
			}

			if (crpth["cutoffdate"] == System.DBNull.Value)
			{
				crpth["cutoffdate"] = DateTime.Now; //Steph -  in case users did not key in the Cut Off Date
			}

			ListBox lsbox_columnlist = BizXmlReader.CurrentInstance.GetControl(formName, "lsbox_columnlist") as ListBox;
			Button Btn_Generate = BizXmlReader.CurrentInstance.GetControl(formName, "Btn_Generate") as Button;
			Button Btn_Generate2 = BizXmlReader.CurrentInstance.GetControl(formName, "Btn_Generate2") as Button;
			Button Btn_AddUser = BizXmlReader.CurrentInstance.GetControl(formName, "Btn_AddUser") as Button;
			Button Btn_RemoveUser = BizXmlReader.CurrentInstance.GetControl(formName, "Btn_RemoveUser") as Button;
			TextBox crpth_selectclause = BizXmlReader.CurrentInstance.GetControl(formName, "crpth_selectclause") as TextBox;
			GroupBox grb_userpermissions = BizXmlReader.CurrentInstance.GetControl(formName, "grb_userpermissions") as GroupBox;
			

			// Only administrator is allowed to see user permissions and add and remove permissions.
			if (!Common.DEFAULT_SYSTEM_ISADMINISTRATORGROUP && !Common.DEFAULT_SYSTEM_ISADMINISTRATORUSER)
			{
				grb_userpermissions.Enabled = false;
				grb_userpermissions.Visible = false;
			}

			Btn_Generate.Click += new EventHandler(Btn_Generate_Click);
			Btn_Generate2.Click += new EventHandler(Btn_Generate2_Click);
			Btn_AddUser.Click += new EventHandler(Btn_AddUser_Click);
			Btn_RemoveUser.Click += new EventHandler(Btn_RemoveUser_Click);

			// Populate the list box for the column list
			refreshUsers();
			dbaccess.DataSet.Tables["crpth"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CRPTH_ColumnChanged);
            if (crpth["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSO || crpth["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                CopySysLocker(crpth["code"].ToString());
            }
            

            if (crpth["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSO || crpth["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                EnableMultipleOpenDocs(crpth["code"].ToString());
            }

            

		}

		void Btn_RemoveUser_Click(object sender, EventArgs e)
		{
			DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];
			ListBox lsbox_users = BizXmlReader.CurrentInstance.GetControl(formName, "lsbox_users") as ListBox;

			dbaccess.RemoteStandardSQL.ExecuteNonQuery("delete from syscustomreportsecurity where refnum ='" + crpth["code"].ToString().Trim() + "' and username ='" + lsbox_users.SelectedValue.ToString().Trim() + "'");
			refreshUsers();
		}

		void Btn_AddUser_Click(object sender, EventArgs e)
		{
			DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];
			TextBox username = BizXmlReader.CurrentInstance.GetControl(formName, "username") as TextBox;
			ListBox lsbox_users = BizXmlReader.CurrentInstance.GetControl(formName, "lsbox_users") as ListBox;

			if (BizFunctions.IsEmpty(username.Text.Trim()))
			{
				MessageBox.Show("Please enter a username to add", "Missing username");
				username.Focus();
				return;
			}

			foreach (DataRowView drv in lsbox_users.Items)
			{
				if (drv[lsbox_users.ValueMember].ToString().Trim().ToLower() == username.Text.ToLower().Trim())
				{
					MessageBox.Show("This user already exists", "Existing user");
					return;
				}
			}

			BizRAD.DB.Interface.Parameter[] parameters = new BizRAD.DB.Interface.Parameter[2];
			parameters[0] = new BizRAD.DB.Interface.Parameter("@refnum", crpth["code"].ToString().Trim());
			parameters[1] = new BizRAD.DB.Interface.Parameter("@username", username.Text.Trim());

			dbaccess.RemoteStandardSQL.GetStoredProcedureResult("sp_savecustomreportsecurity", ref parameters);


			refreshUsers();
		}

		void refreshUsers()
		{
			DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];
			ListBox lsbox_users = BizXmlReader.CurrentInstance.GetControl(formName, "lsbox_users") as ListBox;
			DataSet dstmp = dbaccess.ReadSQLTemp("users", "select * from syscustomreportsecurity where refnum ='" + crpth["code"].ToString().Trim() + "'");


			lsbox_users.ValueMember = "username";
			lsbox_users.DisplayMember = "username";
			lsbox_users.DataSource = dstmp.Tables["users"];
		}

		private bool testView()
		{
			DataRow viewm = dbaccess.DataSet.Tables["viewm"].Rows[0];

			string dropviewsql = "DROP VIEW " + viewm["viewcode"].ToString().Trim();


			string createviewsql = "CREATE VIEW " + viewm["viewcode"].ToString().Trim() + " AS " + yearParser((viewm["viewstatement"].ToString().Trim()),Common.DEFAULT_SYSTEM_YEAR);

			dbaccess.RemoteStandardSQL.ExecuteNonQuery(dropviewsql);
			dbaccess.RemoteStandardSQL.ExecuteNonQuery(createviewsql);
			try
			{
				dbaccess.ReadSQL("test", "select * from " + viewm["viewcode"].ToString().Trim() + " where 1=2");
			}
			catch
			{
				return false;
			}

			return true;
		}


		void Btn_Generate2_Click(object sender, EventArgs e)
		{
			try
			{
                
				DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];
                
                InsertSysLocker(drSysLocker, crpth["code"].ToString());

				// Must enter report code first. So can map layout to the code when user saves layout.
				if (!BizFunctions.IsEmpty(crpth["code"]))
				{
					TextBox cprth_selectclause = BizXmlReader.CurrentInstance.GetControl(formName, "crpth_selectclause") as TextBox;

					if (BizFunctions.IsEmpty(crpth["viewstatement"]))
					{
						MessageBox.Show("View must not be empty");
						return;
					}
					else
					{
						// Recreate the view. Just in case.
						DataSet dsview = dbaccess.ReadSQLTemp("view", "select viewstatement from viewm where viewcode = '" + crpth["viewcode"].ToString().Trim() + "'");
						if (dsview.Tables["view"].Rows.Count > 0)
						{
							bool result = testView();
						}
					}

					String sql = formSql("FULL");


					#region Use seperate connection. Dbserver cannot handle too much querying

					DataTable tmptable = new DataTable();
					string connectionString = null;

					DataSet dsconnection = dbaccess.ReadSQLTemp("reportconnection", "select connectionstring from syscustomreportconnection");
					if (dsconnection.Tables["reportConnection"].Rows.Count > 0)
					{
						connectionString = dsconnection.Tables["reportConnection"].Rows[0]["connectionstring"].ToString().Trim();
					}

					SqlConnection myConnection = new SqlConnection(connectionString);

					SqlDataAdapter myAdapter = new SqlDataAdapter(sql, myConnection);

					SqlCommandBuilder myCmdBuilder = new SqlCommandBuilder(myAdapter);

					myAdapter.TableMappings.Add("Table", "report");

					DataSet reportds = new DataSet();

					myAdapter.Fill(reportds);
					
					ReportObject reportOb = new ReportObject(reportds, String.Empty
																  , dbaccess.DataSet.Tables["crpth"].Rows[0]["description"].ToString().Trim()
																  , crpth["code"].ToString().Trim(),crpth["whereclause"].ToString().Trim());

					//Button document_close = ((Button)BizXmlReader.CurrentInstance.GetControl(formName4SideBtn, Common.DEFAULT_DOCUMENT_BTNCLOSE));
					((Button)BizXmlReader.CurrentInstance.GetControl(formName4SideBtn, Common.DEFAULT_DOCUMENT_BTNCLOSE)).PerformClick();
					NormalCustom NormalCustom = new NormalCustom(reportOb);
					//this.voucherBase.Cancel_Click();

					#endregion

				}
				else
					MessageBox.Show("Please enter the report code first!");

			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error");
				return;
			}
		}

		void crpth_selectclause_DragOver(object sender, DragEventArgs e)
		{
			TextBox txt_selectClause = (sender as TextBox);
			Point currentPoint = new Point(e.X, e.Y);

			selectClauseDropLocation = txt_selectClause.GetCharIndexFromPosition(txt_selectClause.PointToClient(currentPoint));
		}

		void crpth_selectclause_DragDrop(object sender, DragEventArgs e)
		{
			DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];

			if (e.Data.GetDataPresent(DataFormats.StringFormat))
			{
				if ((sender as TextBox).Text.Length != 0)
				{
					// If adding to the front of an existing string
					if (selectClauseDropLocation==0)
					{
						(sender as TextBox).Text = (sender as TextBox).Text.Insert(selectClauseDropLocation, (e.Data.GetData(DataFormats.Text)).ToString().Trim()+",");
					}
					else
					// If adding to the end of an existing string
					if ((sender as TextBox).Text.Trim().Length - 1 == selectClauseDropLocation)
					{

						if ((sender as TextBox).Text[selectClauseDropLocation] == '(')
							(sender as TextBox).Text = (sender as TextBox).Text.Insert(selectClauseDropLocation + 1, (e.Data.GetData(DataFormats.Text)).ToString().Trim() + ")");
						else
							(sender as TextBox).Text = (sender as TextBox).Text.Insert(selectClauseDropLocation + 1, "," + (e.Data.GetData(DataFormats.Text)).ToString().Trim());
	

						//(sender as TextBox).Text = (sender as TextBox).Text.Insert(selectClauseDropLocation + 1, "," + (e.Data.GetData(DataFormats.Text)).ToString().Trim());
					}
					else
					// If adding in middle of a string
					{
						// Find the nearest comma
						int nearestcommaafter = (sender as TextBox).Text.Trim().Length;
						int nearestcommabefore = 0;
						int insertlocation = 0;

						string originalstring = (sender as TextBox).Text.Trim();

						char[] str = originalstring.ToCharArray();
						Array.Reverse(str);
						string reversestring = new string(str);

						nearestcommaafter = originalstring.IndexOf(',', selectClauseDropLocation);


						for (int i = selectClauseDropLocation; i >= 0; i--)
						{
							if (originalstring[i] == ',')
							{
								nearestcommabefore = i;
								break;
							}
						}

						// Get nearest comma location
						insertlocation = (nearestcommaafter > nearestcommabefore) ? nearestcommaafter : nearestcommabefore;

						// Try to insert before the nearest comma.
						if (insertlocation != 0)
						{
							if ((sender as TextBox).Text[insertlocation] == '(')
									(sender as TextBox).Text = (sender as TextBox).Text.Insert(insertlocation, (e.Data.GetData(DataFormats.Text)).ToString().Trim() +")");
							else
								(sender as TextBox).Text = (sender as TextBox).Text.Insert(insertlocation, "," + (e.Data.GetData(DataFormats.Text)).ToString().Trim());
						}
						else
							(sender as TextBox).Text = (sender as TextBox).Text.Insert(insertlocation, (e.Data.GetData(DataFormats.Text)).ToString().Trim() + ",");

					}

				}
				else
					// If nothing in the textbox
				(sender as TextBox).Text = (sender as TextBox).Text.Insert(selectClauseDropLocation, e.Data.GetData(DataFormats.Text).ToString().Trim());
			}

			crpth["selectclause"] = (sender as TextBox).Text;
			(sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;
			(sender as TextBox).SelectionLength = 0;
			(sender as TextBox).ScrollToCaret();
			(sender as TextBox).Focus();
		}

		void lsbox_columnlist_MouseDown(object sender, MouseEventArgs e)
		{
			int indexOfItem = (sender as ListBox).IndexFromPoint(e.X, e.Y);

			if (indexOfItem >= 0 && indexOfItem < (sender as ListBox).Items.Count)  // check we clicked down on a string
			{
				(sender as ListBox).DoDragDrop((sender as ListBox).Items[indexOfItem], DragDropEffects.Copy);
			}
		}

		void crpth_selectclause_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.StringFormat) && (e.AllowedEffect == DragDropEffects.Copy))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.Move;
		}

		void Btn_Generate_Click(object sender, EventArgs e)
		{
			try
			{
				DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];

				ListBox lsbox_columnlist = BizXmlReader.CurrentInstance.GetControl(formName, "lsbox_columnlist") as ListBox;
				TextBox cprth_selectclause = BizXmlReader.CurrentInstance.GetControl(formName, "crpth_selectclause") as TextBox;

				if (BizFunctions.IsEmpty(crpth["viewstatement"]))
				{
					MessageBox.Show("View must not be empty");
					return;
				}
				else
				{
					// Recreate the view. Just in case.
					DataSet dsview = dbaccess.ReadSQLTemp("view", "select viewstatement from viewm where viewcode = '" + crpth["viewcode"].ToString().Trim() + "'");
					if (dsview.Tables["view"].Rows.Count > 0)
					{
						bool result = testView();
					}
				}

				string sql = formSql("FULL");
				string connectionString = null;

				DataTable tmptable = new DataTable();

				DataSet dsconnection = dbaccess.ReadSQLTemp("reportconnection", "select connectionstring from syscustomreportconnection");
				if (dsconnection.Tables["reportConnection"].Rows.Count > 0)
				{
					connectionString = dsconnection.Tables["reportConnection"].Rows[0]["connectionstring"].ToString().Trim();
				}

				SqlConnection myConnection = new SqlConnection(connectionString);

				SqlDataAdapter myAdapter = new SqlDataAdapter(sql, myConnection);

				SqlCommandBuilder myCmdBuilder = new SqlCommandBuilder(myAdapter);

				myAdapter.TableMappings.Add("Table", "report");

				DataSet reportds = new DataSet();

				myAdapter.Fill(reportds);

				ReportObject reportOb = new ReportObject(reportds, String.Empty
															  , dbaccess.DataSet.Tables["crpth"].Rows[0]["description"].ToString().Trim()
															  , dbaccess.DataSet.Tables["crpth"].Rows[0]["code"].ToString().Trim()
															  , crpth["whereclause"].ToString().Trim());

				ReportTable reportTable = new ReportTable(reportOb);
			}
			catch(Exception exp)
			{
				MessageBox.Show(exp.Message, "Error");
				return;
			}
		}

		#endregion

        #region ColumnChangedEvents

        #region CRPTH

		void Voucher_CRPTH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			if (crpthColumnChange)
			{
				DataRow crpth = this.dbaccess.DataSet.Tables["crpth"].Rows[0];

				switch (e.Column.ColumnName)
				{
					case "viewcode":

						dbaccess.ReadSQL("viewm", "Select * from viewm where viewcode ='" + e.Row["viewcode"].ToString().Trim() + "'");
						e.Row["viewdescription"] = dbaccess.DataSet.Tables["viewm"].Rows[0]["viewdescription"].ToString().Trim();
						e.Row["viewstatement"] = dbaccess.DataSet.Tables["viewm"].Rows[0]["viewstatement"].ToString().Trim();
						//populateColumnList(dbaccess);
						break;
				}
			}
		}

		#endregion CRPTH


		#endregion ColumnChangedEvents

		#region Document Handles

		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
			e.Handle = false;
		}

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
		    base.Document_Save_Handle(sender, e);
		    DataRow crpth = e.DBAccess.DataSet.Tables["crpth"].Rows[0];

			// Check if report code is duplicate
			//DataSet dscodevalidate = dbaccess.ReadSQLTemp("reportcode", "Select code from crpth where code = '" + crpth["code"].ToString().Trim() + "' and refnum <>'" + crpth["code"].ToString() + "' and status<>'" + Common.DEFAULT_DOCUMENT_STATUSV + "'");
			//if (dscodevalidate.Tables["reportcode"].Rows.Count > 0)
			//{
			//    MessageBox.Show("The report code is already used, please enter another code");
			//    e.Handle = false;
			//    return;
			//}
            //InsertSysLocker(drSysLocker,crpth["code"].ToString());

		}

        protected override void Document_Paste_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Paste_Handle(sender, e);
        }

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);
			e.Handle = false;
		}

		#endregion

		#region Document OnClicks

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);


			e.Handle = false;
        }

		#endregion

		#region SaveBegin
		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick(sender, e);

		}
		#endregion SaveBegin

        #region Private Functions

		// This function forms the sql from the view and filters entered by the user.
		private string formSql(string mode)
		{

			DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];

			if (!BizFunctions.IsEmpty(crpth["viewcode"]))
			{
				string view = " FROM " + crpth["viewcode"].ToString().Trim();

				string selectclause = "select *";
				//string selectclause = String.Empty ;

				if (!BizFunctions.IsEmpty(crpth["selectclause"]) && crpth["selectclause"].ToString().Trim() != "*")
					selectclause = "select " + crpth["selectclause"].ToString().Trim();

				string whereclause = String.Empty;

				switch (mode.ToUpper().Trim())
				{

					case "FULL":

						if (!BizFunctions.IsEmpty(crpth["whereclause"]))
							whereclause = " where " + crpth["whereclause"].ToString().Trim();

						break;

					case "QUICK":

						whereclause = " where 1=2";

						break;
				}

				string groupbyclause = String.Empty;

				if (!BizFunctions.IsEmpty(crpth["groupbyclause"]))
					groupbyclause = " group by " + crpth["groupbyclause"].ToString().Trim();

				string havingclause = String.Empty;

				if (!BizFunctions.IsEmpty(crpth["havingclause"]))
					havingclause = " having " + crpth["havingclause"].ToString().Trim();

				string orderbyclause = String.Empty;

				if (!BizFunctions.IsEmpty(crpth["orderbyclause"]))
					orderbyclause = " order by " + crpth["orderbyclause"].ToString().Trim();

				//string selectsql = selectclause
				//                    + view
				//                    + whereclause
				//                    + groupbyclause
				//                    + havingclause
				//                    + orderbyclause;

				string selectsql = "select * " + view + yearParser(whereclause,Common.DEFAULT_SYSTEM_YEAR);

				return selectsql;
			}

			return String.Empty;
		}

		private string yearParser(string input, string currentyear)
		{
			DataRow crpth = dbaccess.DataSet.Tables["crpth"].Rows[0];

			input = input.ToLower();
			// Quick code. Just forward and backwards 10 years. Anything that looks for more than 20 years will not be supported
			input = input.Replace("!year+1!", Convert.ToString((int.Parse(currentyear) + 1)));
			input = input.Replace("!year+2!", Convert.ToString((int.Parse(currentyear) + 2)));
			input = input.Replace("!year+3!", Convert.ToString((int.Parse(currentyear) + 3)));
			input = input.Replace("!year+4!", Convert.ToString((int.Parse(currentyear) + 4)));
			input = input.Replace("!year+5!", Convert.ToString((int.Parse(currentyear) + 5)));
			input = input.Replace("!year+6!", Convert.ToString((int.Parse(currentyear) + 6)));
			input = input.Replace("!year+7!", Convert.ToString((int.Parse(currentyear) + 7)));
			input = input.Replace("!year+8!", Convert.ToString((int.Parse(currentyear) + 8)));
			input = input.Replace("!year+9!", Convert.ToString((int.Parse(currentyear) + 9)));
			input = input.Replace("!year+10!", Convert.ToString((int.Parse(currentyear) + 10)));
			input = input.Replace("!year!", Convert.ToString((int.Parse(currentyear))));
			input = input.Replace("!year-1!", Convert.ToString((int.Parse(currentyear) - 1)));
			input = input.Replace("!year-2!", Convert.ToString((int.Parse(currentyear) - 2)));
			input = input.Replace("!year-3!", Convert.ToString((int.Parse(currentyear) - 3)));
			input = input.Replace("!year-4!", Convert.ToString((int.Parse(currentyear) - 4)));
			input = input.Replace("!year-5!", Convert.ToString((int.Parse(currentyear) - 5)));
			input = input.Replace("!year-6!", Convert.ToString((int.Parse(currentyear) - 6)));
			input = input.Replace("!year-7!", Convert.ToString((int.Parse(currentyear) - 7)));
			input = input.Replace("!year-8!", Convert.ToString((int.Parse(currentyear) - 8)));
			input = input.Replace("!year-9!", Convert.ToString((int.Parse(currentyear) - 9)));
			input = input.Replace("!year-10!", Convert.ToString((int.Parse(currentyear) - 10)));

			input = input.Replace("!date!", Convert.ToString(BizFunctions.GetSafeDateString((DateTime)crpth["cutoffdate"])));
		
			
			return input;

		}

        private void InsertSysLocker(DataRow dr1,string key)
        {
            string GetSyslocker = "Select * from syslocker where [key]='" + key + "'";
            this.dbaccess.ReadSQL("TmpSysLocker", GetSyslocker);
            string StrInsertSyslocker = "";

            DataTable TmpSysLocker = this.dbaccess.DataSet.Tables["TmpSysLocker"];
            if (TmpSysLocker.Rows.Count <= 0)
            {
                StrInsertSyslocker = "Insert Into Syslocker (ModuleName,[Key],UserName,LockTime) VALUES ('" + dr1["ModuleName"].ToString() + "','" + dr1["Key"].ToString() + "','" + dr1["UserName"].ToString() + "','" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(dr1["LockTime"].ToString())) + "')";
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(StrInsertSyslocker);
            }
        }

        private void CopySysLocker(string key)
        {
            string GetSyslocker = "Select * from syslocker where [key]='" + key + "'";
            this.dbaccess.ReadSQL("SysLocker", GetSyslocker);

            SysLocker = this.dbaccess.DataSet.Tables["SysLocker"];
            drSysLocker = this.dbaccess.DataSet.Tables["SysLocker"].Rows[0];

        }

        private void EnableMultipleOpenDocs(string key)
        {
            string DelSyslocker = "Delete from syslocker where [key]='" + key + "'";

            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(DelSyslocker);
        }

        #endregion Private Functions
    }
}
