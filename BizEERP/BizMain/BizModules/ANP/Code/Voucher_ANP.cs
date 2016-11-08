/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_ANP.cs
 *	Description:    Sales Invoice Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;

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
using BizRAD.BizReport;
using DEMO.MDT;

using ATL.ExtractTools;



namespace ATL.ANP
{
	public class Voucher_ANP : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables	
		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;
		protected bool opened = false;
		protected decimal anp1_grosamt = 0;
		protected decimal anp1_discamt = 0;
		protected decimal anp1_oriamt = 0;
		protected decimal anp1_origstamt = 0;
		protected decimal anp1_postamt = 0;
		protected decimal anp1_gstamt = 0;
		protected decimal anp1_roriamt = 0;
		protected decimal anp1_rpostamt = 0;
	    protected string detailFormName = null;
		protected bool anphColumnChange = true;
		protected bool anp1ColumnChange = true;		

		public string documentKey = null;
		protected int TabDetail;

		protected Button getIncomeExpense;
		protected string strAccNotOverwritten = String.Empty;

		protected Button btnDelete;
		protected Button btnUp;
		protected Button btnDown;
		protected Button btnMark;
		protected Button btnDuplicate;
		protected Button btnExtract;

		string headerFormName;

		#endregion

		public Voucher_ANP(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_ANP.xml", moduleName, voucherBaseHelpers)
		{
		}

		#region Steph - To stop users from open more than 1 voucher from the same module  as this is causing the saving error.
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
		#endregion

		#region Voucher Default/ALL

		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition (e);
			e.Condition = "anph.flag='ANP' AND anph.systemyear = "+Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (anph.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or "+
				" anph.status = '" + Common.DEFAULT_DOCUMENT_STATUSO+ "' or "+
				" anph.status = '" +Common.DEFAULT_DOCUMENT_STATUSE +"')  "+
				" AND anph.flag='ANP' AND anph.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);
			DataRow anph = e.DBAccess.DataSet.Tables["anph"].Rows[0];
			
			if(BizValidate.ChkPeriodLocked(e.DBAccess, anph["period"].ToString()))
			{
				MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
			}
		}

		protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Preview_Handle (sender, e);
			DataRow anph = dbaccess.DataSet.Tables["anph"].Rows[0];
			if (anph["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
			{
				if (BizValidate.CheckRowState(e.DBAccess.DataSet, "anph"))
				{
					MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					e.Handle = false;
				}
			}
		}
		#endregion

		#region Document Event

		#region Form Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad (sender, e);
			opened = true;
			documentKey = e.DocumentKey;
			
			DataRow anph = e.DBAccess.DataSet.Tables["anph"].Rows[0];
            DataTable anp1 = e.DBAccess.DataSet.Tables["anp1"];
			string headerFormName = (e.FormsCollection["header"] as Form).Name;
		//	string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;

			this.headerFormName = (e.FormsCollection["header"] as Form).Name;
				
			if (anph["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "anph_trandate", anph);
			}

			anph["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			this.dbaccess = e.DBAccess;

			setDefaults(this.dbaccess.DataSet, "anph");

			#region Steph - To show the decimal amount with thousand separator

			ReBindsTextBox(headerFormName, "anph_qty", e.DBAccess.DataSet.Tables["anph"], "qty", DecimalToCurrencyString);
			//ReBindsTextBox(headerFormName, "anph_exrate", e.DBAccess.DataSet.Tables["anph"], "exrate", DecimalToCurrencyString);
			//ReBindsTextBox(headerFormName, "anph_grosamt", e.DBAccess.DataSet.Tables["anph"], "grosamt", DecimalToCurrencyString);
			//ReBindsTextBox(headerFormName, "anph_discamt", e.DBAccess.DataSet.Tables["anph"], "discamt", DecimalToCurrencyString);
			//ReBindsTextBox(headerFormName, "anph_origstamt", e.DBAccess.DataSet.Tables["anph"], "origstamt", DecimalToCurrencyString);
			//ReBindsTextBox(headerFormName, "anph_oriamt", e.DBAccess.DataSet.Tables["anph"], "oriamt", DecimalToCurrencyString);
			#endregion
			
			// Set link to database
			e.DBAccess.DataSet.Tables["anph"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ANPH_ColumnChanged);
			e.DBAccess.DataSet.Tables["anp1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ANP1_ColumnChanged);
		}

		#endregion


		#region Reopen Handle

		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
			DataRow anph = this.dbaccess.DataSet.Tables["anph"].Rows[0];

			#region Steph - Do not allow reopen if voucher has been created for this Sales Invoice - Ex: Sales Receipt
			//dbaccess.ReadSQL("checkArdReopen", "SELECT refnum FROM ard" + Common.DEFAULT_SYSTEM_YEAR + " WHERE invnum = '" + anph["refnum"].ToString().Trim() + "' AND refnum <>'" + anph["refnum"].ToString().Trim() + "' AND flag<>'ANP'");
			//DataTable checkArdReopen = dbaccess.DataSet.Tables["checkArdReopen"];
			//string RefList = "Please check entries below which has been created for this Sales Invoice:";
			//foreach (DataRow dr in checkArdReopen.Rows)
			//{
			//    if (dr.RowState != DataRowState.Deleted)
			//    {
			//        RefList = RefList + "\n " + dr["refnum"].ToString().Trim();
			//    }
			//}

			//if (RefList != "Please check entries below which has been created for this Sales Invoice:")
			//{
			//    MessageBox.Show(RefList, "Reopen Unsuccessful");
			//    e.Handle = false;
			//}
			#endregion

			//else if (anph["flag"].ToString().Trim() != "ANP")
			//{
			//    e.Handle = false;
			//}
			//else
			//{
				e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM MWT" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + anph["refnum"].ToString().Trim() + "'");
			//}
		}

		#endregion

		protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
		{
			base.AddDetailF3Condition(sender, e);

			switch (e.MappingName)
			{
				case "matnum":
					e.CurrentRow["detail"] = e.F2CurrentRow["matname"];
					e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
					AutoCalc();					
					break;
			}
		}

		protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Insert_OnClick(sender, e);
			DataTable anp1 = dbaccess.DataSet.Tables["anp1"];

			btnUp.Enabled = true;
			btnDown.Enabled = true;
		}

		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);
			DataRow anph = e.DBAccess.DataSet.Tables["anph"].Rows[0];
			DataTable anp1 = e.DBAccess.DataSet.Tables["anp1"];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable gld = e.DBAccess.DataSet.Tables["gld"];
			setDefaults(dbaccess.DataSet, "anp1");

			anph["invnum"] = anph["refnum"];
			anph["invdate"] = anph["trandate"];

			setColumnChange("all", false);

			AutoCalc();

			setColumnChange("all", true);			
			
		}

		#endregion
       
      #region TabControl Handle

		protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_TabControl_Handle(sender, e);

			TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
		}

		#endregion


		#region Steph - Extraction Time!!
		protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Extract_Handle(sender, e);
			
			DataRow anph = dbaccess.DataSet.Tables["anph"].Rows[0];
			#region Steph - MDT Extraction

			Hashtable HsExtract = MDTReader.GetExtraction("anp", "DOR-ANP Extract", TabDetail, this.dbaccess);

			if (HsExtract.Count > 0)
			{
				ExtractGrid extract = new ExtractGrid(this.dbaccess, "extract", HsExtract["DestinationTable"].ToString().Trim(), HsExtract["colDisplay"].ToString().Trim(), HsExtract["colCopy"].ToString().Trim(),
									HsExtract["sqlDisplay"].ToString().Trim(), HsExtract["sqlCopy"].ToString().Trim(), HsExtract["extractkey"].ToString().Trim(), Convert.ToBoolean(HsExtract["inclextracted"]));
				extract.showGrid();
			}
			else
			{
				MessageBox.Show("Error in data extraction");
			}
			#endregion

			AutoCalc();

			e.Handle = false;			         
		
		}
		#endregion

		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow anph = e.DBAccess.DataSet.Tables["anph"].Rows[0];
			DataTable anp1 = e.DBAccess.DataSet.Tables["anp1"];
			DataTable ard = e.DBAccess.DataSet.Tables["ard"];
			DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			DataTable gld = e.DBAccess.DataSet.Tables["gld"];
			DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];

			setDefaults(dbaccess.DataSet, "anp1");

			AutoCalc();	

			#region Steph - Error Checking!
			string orimessage = "Please check the following:";
			string DataEntryErrors = orimessage;

			dbaccess.ReadSQL("checkAcm", "SELECT accnum,accname FROM acm");

			//DataTable getEmptyAcAnph = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum from [anph] WHERE isnull(accnum,'') not in (SELECT accnum from [checkAcm])");
			//if (getEmptyAcAnph.Rows.Count > 0)
			//{
			//    DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. (Debit A/C) in Header";
			//}
		
			//DataTable getEmptyAcAnp1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [anp1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			//if (getEmptyAcAnp1.Rows.Count > 0)
			//{
			//    DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. in Detail";
			//}

			if (DataEntryErrors != orimessage)
			{
				MessageBox.Show(DataEntryErrors, "Confirm Unsuccessful");
				e.Handle = false;
			}
			else
			{
				#region Voucher Confirmed - Posting Time!
				
				DataTable detail2Update = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT * FROM anp1 WHERE dqty<>0 ");

				detail2Update.TableName = "detail2Update";

				dbaccess.DataSet.Tables.Add(detail2Update);

				MDTReader.updateMWT(ref this.dbaccess, "refnum|matnum|docunum|uom|arnum|qty|period|" +
					"user|flag|status|created|modified|stkdate|trandate|remark|whnum|location", "detail2Update", "mwt");	

				#endregion
			}

			#endregion
			}

		#region SaveBegin
		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

			DataRow anph = dbaccess.DataSet.Tables["anph"].Rows[0];
			DataTable anp1 = dbaccess.DataSet.Tables["anp1"];

			foreach (DataRow dr in anp1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(anph, dr, "user/created/modified/status");
                    dr["location"]="HQ";
				}
			}
		}
         
		#endregion	

        #region Tab Control

        protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
        {
            base.Document_TabControl_OnSelectionChanged(sender, e);
            btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
            btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
            btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;
            btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
            btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
            btnExtract = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Extract") as Button;

			btnExtract.Enabled = false;

            switch ((sender as LXFClassLibrary.Controls.TabControl).SelectedIndex)
            {
                case 0:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnMark.Enabled = true;
					break;            
            }
        }

        #endregion
        
       #region Preview

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick (sender, e);
            DataRow anph = e.DBAccess.DataSet.Tables["anph"].Rows[0];
            Hashtable selectedCollection = new Hashtable();
			
			switch (e.ReportName)
            {
				case "Advertisement and Promotion":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + anph["arnum"].ToString() + "'");
					selectedCollection.Add("matm", "SELECT * FROM matm");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;
            }            
		}

		#endregion

		#endregion
	
		#region ColumnChangedEvents

		#region anph

        private void Voucher_ANPH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
			DataRow anph = dbaccess.DataSet.Tables["anph"].Rows[0];
			DataTable anp1 = dbaccess.DataSet.Tables["anp1"];
			if (anphColumnChange)
			{
				switch (e.Column.ColumnName)
				{
					case "arnum":
						setColumnChange("all", false);
						#region Steph - Pull info from ARM
						dbaccess.ReadSQL("getArmInfo", "SELECT arnum,arname,ptc,addr1,addr2,addr3,addr4,phone,hp,fax,ptnum,"+
							" gstgrpnum,oricur,accnum FROM arm "+
							" WHERE arnum ='" + e.Row["arnum"].ToString().Trim() + "'");

						if (dbaccess.DataSet.Tables["getArmInfo"].Rows.Count > 0)
						{
							DataRow getArmInfo = dbaccess.DataSet.Tables["getArmInfo"].Rows[0];
							e.Row["detail"] = getArmInfo["arname"];
							//e.Row["contact"] = getArmInfo["ptc"];
							e.Row["addr1"] = getArmInfo["addr1"];
							e.Row["addr2"] = getArmInfo["addr2"];
							e.Row["addr3"] = getArmInfo["addr3"];
							e.Row["addr4"] = getArmInfo["addr4"];
							e.Row["phone"] = getArmInfo["phone"];
							e.Row["hp"] = getArmInfo["hp"];
							e.Row["fax"] = getArmInfo["fax"];

							if (e.Row["payterms"].ToString().Trim() == "" || e.Row["payterms"] == System.DBNull.Value)
								e.Row["payterms"] = getArmInfo["ptnum"];
							if (e.Row["gstgrpnum"].ToString().Trim() == "" || e.Row["gstgrpnum"] == System.DBNull.Value)
								e.Row["gstgrpnum"] = getArmInfo["gstgrpnum"];
							if (e.Row["oricur"].ToString().Trim() == "" || e.Row["oricur"] == System.DBNull.Value)
								e.Row["oricur"] = getArmInfo["oricur"];
							if (e.Row["accnum"].ToString().Trim() == String.Empty || e.Row["accnum"] == System.DBNull.Value)
								e.Row["accnum"] = getArmInfo["accnum"];
						}
						else
						{
							e.Row["detail"] = "";
							e.Row["contact"] = "";
							e.Row["addr1"] = "";
							e.Row["addr2"] = "";
							e.Row["addr3"] = "";
							e.Row["addr4"] = "";
							e.Row["phone"] = "";
							e.Row["hp"] = "";
							e.Row["fax"] = "";
							e.Row["payterms"] = "";
							e.Row["gstgrpnum"] = "";
							e.Row["oricur"] = "";
							e.Row["accnum"] = "";
						}
						setColumnChange("all", true);
						break;
						#endregion
					case "oricur":
						setColumnChange("all", false);
						#region set exrate
						e.Row.BeginEdit();
						string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
						this.dbaccess.ReadSQL("exrate", exrStr);
						if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
						{
							decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
							e.Row["exrate"] = exrate;
						}
						e.Row.EndEdit();
						setColumnChange("all", true);
						break;
						#endregion
					case "gstgrpnum":
						setColumnChange("all", false);
						#region set gstper

						e.Row.BeginEdit();
						this.dbaccess.ReadSQL("gstm", "SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='" + e.Row[e.Column.ColumnName].ToString() + "'");
						if (this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
						{
							if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
							{
								if ((decimal)e.Row["gstper"] == 0 || e.Row["gstper"] == System.DBNull.Value)
								{
									e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)e.Row["trandate"]);
								}
							}
							else
							{
								e.Row["gstper"] = 0;
							}
						}
						e.Row.EndEdit();
						setColumnChange("all", true);
						break;

						#endregion
					case "trandate":
						setColumnChange("all", false);
						#region set dorh exrate

						e.Row.BeginEdit();
						//e.Row["exrate"] = BizAccounts.GetExRate(this.dbaccess, e.Row["oricur"].ToString(), (DateTime)e.Row[e.Column.ColumnName]);
						string strexr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
						this.dbaccess.ReadSQL("exrate", strexr);
						if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
						{
							decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
							e.Row["exrate"] = exrate;
						}

						anph["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(anph["trandate"]));

						e.Row.EndEdit();
						setColumnChange("all", true);
						break;

						#endregion			
				}
			}
        }            
		#endregion

		#region anp1
		private void Voucher_ANP1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch(e.Column.ColumnName)
			{

				case "dqty":
					AutoCalc();
					break;

				//case "matnum":
					//#region Steph - Pull Info from MATM
					//dbaccess.ReadSQL("getMatm", "SELECT matname, uom,saleAcc FROM matm where matnum ='" + e.Row["matnum"].ToString().Trim() + "'");
					//if(dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
					//{
					//    if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
					//        e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
					//    if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
					//        e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uom"];
					//    if (e.Row["accnum"].ToString().Trim() == "" || e.Row["accnum"] == System.DBNull.Value)
					//        e.Row["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
					//}
					//break;

					//#endregion		
			}
		}

		#endregion

		#endregion

		#region fun fun - To set default values

		public static void setDefaults(DataSet dataSet, string tableNames)
		{
			string[] tables = tableNames.Split(new char[] { '/', '\\' });

			for (int i = 0; i < tables.Length; i++)
			{
				DataTable dt = dataSet.Tables[tables[i]];

				foreach (DataRow dr in dt.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						foreach (DataColumn dc in dt.Columns)
						{
							switch (dc.DataType.ToString())
							{
								// All decimals are 0 by default
								case "System.Decimal":
									if (dr[dc.ColumnName] == System.DBNull.Value)
										dr[dc.ColumnName] = 0;
									break;

								// All smallints are 0 by default
								case "System.Int16":
									if (dr[dc.ColumnName] == System.DBNull.Value)
										dr[dc.ColumnName] = 0;
									break;

								// All ints are 0 by default
								case "System.Int32":
									if (dr[dc.ColumnName] == System.DBNull.Value)
										dr[dc.ColumnName] = 0;
									break;

								// All bigints are 0 by default but do not touch ID
								case "System.Int64":
									if (dr[dc.ColumnName] == System.DBNull.Value && dc.ColumnName != "ID")
										dr[dc.ColumnName] = 0;
									break;

								// All bits are 0 by default
								case "System.Bit":
									if (dr[dc.ColumnName] == System.DBNull.Value)
										dr[dc.ColumnName] = 0;
									break;

								// All booleans are false by default
								case "System.Boolean":
									if (dr[dc.ColumnName] == System.DBNull.Value)
										dr[dc.ColumnName] = false;
									break;

								// Trim white spaces due to user entry
								case "System.String":
									if (dr[dc.ColumnName] != System.DBNull.Value)
										dr[dc.ColumnName] = (dr[dc.ColumnName] as String).Trim();
									break;
							}
						}
					}
				}
			}

		}
		#endregion

		#region Steph - To set the Auto Calc to be used in various events
		private void AutoCalc()
		{
			setColumnChange("all", false);
			DataRow anph = dbaccess.DataSet.Tables["anph"].Rows[0];
			DataTable anp1 = dbaccess.DataSet.Tables["anp1"];
			DataTable anpc = dbaccess.DataSet.Tables["anpc"];

			setDefaults(dbaccess.DataSet,"anph/anp1");

			if (anph["refnum"].ToString().Trim().Contains("ANP"))
			{
				if (anph["docunum"] == System.DBNull.Value || anph["docunum"].ToString().Trim() == String.Empty)
				{
					anph["docunum"] = anph["refnum"];
				}
			}			


			#region Steph - Get ptc from ARM
			dbaccess.ReadSQL("getArmContact", "SELECT ptc FROM arm WHERE arnum ='"+anph["arnum"].ToString().Trim()+"'");
			if (dbaccess.DataSet.Tables["getArmContact"].Rows.Count > 0)
			{
				if (anph["contact"].ToString().Trim() == String.Empty || anph["contact"] == System.DBNull.Value)
				{
					anph["contact"] = dbaccess.DataSet.Tables["getArmContact"].Rows[0]["ptc"];
				}
			}
			#endregion

			#region Steph -  To get pd from pd (nonYear) table.
			anph["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(anph["trandate"]));
			#endregion

			#region anp1

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum2 FROM arm WHERE arnum = '" + anph["arnum"].ToString().Trim() + "'");

			decimal myline = 0;//x
			decimal line = 0;
			decimal lineFocStart = 99999;  //assuming one transaction does not have 99999 records!
			decimal anp1_totalQty = 0;
			decimal myLine = 0;


			#region  Steph  - inclusive/exclusive gst not applicable for this module. Commented away
			//if ((bool)anph["inclgst"])
			//{
			//    #region Steph - Inclusive GST calculations
			//    #region initialise values
			//    anp1_discamt = 0;
			//    anp1_oriamt = 0;
			//    anp1_origstamt = 0;
			//    anp1_postamt = 0;
			//    anp1_gstamt = 0;
			//    anp1_grosamt = 0;
			//    anp1_roriamt = 0;
			//    anp1_rpostamt = 0;
			//    myline = 0;
			//    line = 0;
			//    #endregion

			//    dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
			//            " WHERE gstgrpnum ='" + anph["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

			//    foreach (DataRow dr in anp1.Rows)
			//    {
			//        if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
			//        {
			//            BizFunctions.UpdateDataRow(anph, dr);
			//            BizFunctions.UpdateDataRow(anph, dr, "refnum/arnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

			//            #region Steph - Adding of hscode and country of origin: 101109_1
						
			//            //reading hscode from pcat, instead matm after mh amended matm and pcat_24Nov2009_0939
			//            //dbaccess.ReadSQL("getHsCountry", "SELECT hscode, oricountry FROM matm WHERE matnum = '" + dr["matnum"].ToString().Trim() + "'");
			//            dbaccess.ReadSQL("getHsCountry", "SELECT pcatm.hscode AS hscode, matm.oricountry AS oricountry FROM matm " +
			//                " LEFT OUTER JOIN pcatm ON matm.pcatcode = pcatm.pcatcode " +
			//                " WHERE matm.matnum = '" + dr["matnum"].ToString().Trim() + "'");

			//            if (dbaccess.DataSet.Tables["getHsCountry"].Rows.Count > 0)
			//            {
			//                dr["hscode"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["hscode"];
			//                dr["oricountry"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["oricountry"];
			//            }
			//            #endregion

			//            if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
			//            {
			//                if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
			//                {
			//                    dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
			//                }
			//            }

			//            dr["roriamt"] = BizFunctions.Round((Convert.ToDecimal(dr["dqty"]) * Convert.ToDecimal(dr["price"])) - Convert.ToDecimal(dr["discamt"]));
			//            dr["rpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(dr["exrate"]));

			//            if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
			//            {
			//                //steph  - do not have to calculate the gst, allow user to key in manually.
			//            }
			//            else
			//            {
			//                dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(anph["gstper"]) / (100 + Convert.ToDecimal(anph["gstper"])));
			//            }
			//            dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
			//            dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
			//            dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
			//            dr["dgrosamt"] = dr["doriamt"];
						

			//            //steph - to assign myline for print purpose
			//            if (Convert.ToDecimal(dr["dqty"]) > 0)
			//            {
			//                myline = myline + 1;
			//                dr["myline"] = myline;
			//            }
			//            else
			//            {
			//                dr["myline"] = 0;
			//            }

			//            #region Steph -  To replace the rest of the lines after the foc line as foc.
			//            //if (dr["foc"] != System.DBNull.Value && (bool)dr["foc"] != false)
			//            //{
			//            //    lineFocStart = Convert.ToDecimal(dr["line"]);
			//            //}
			//            #endregion

			//            //line = line + 100;
			//            //dr["line"] = line;
						
			//            anp1_discamt += (decimal)dr["discamt"];
			//            anp1_oriamt += (decimal)dr["doriamt"];
			//            anp1_origstamt += (decimal)dr["dorigstamt"];
			//            anp1_postamt += (decimal)dr["dpostamt"];
			//            anp1_gstamt += (decimal)dr["dgstamt"];
			//            anp1_grosamt += (decimal)dr["dgrosamt"];
			//            anp1_roriamt += (decimal)dr["roriamt"];
			//            anp1_rpostamt += (decimal)dr["rpostamt"];

			//            #region Steph - Pull Info from MATM
			//            dbaccess.ReadSQL("getMatm", "SELECT matname,uomcode,saleAcc FROM matm where matnum ='" + dr["matnum"].ToString().Trim() + "'");
			//            if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
			//            {
			//                if (dr["detail"].ToString().Trim() == "" || dr["detail"] == System.DBNull.Value)
			//                    dr["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
			//                if (dr["accnum"].ToString().Trim() == "" || dr["accnum"] == System.DBNull.Value)
			//                    dr["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
			//                if (dr["uom"].ToString().Trim() == "" || dr["uom"] == System.DBNull.Value)
			//                    dr["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
			//            }
			//            #endregion
			//        }
			//    }				
			//#region Steph - Check the gst amt differences and add/deduct from the first entry of anp1
			//    if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
			//    {
			//        //steph  - do not have to calculate the gst, allow user to key in manually.
			//    }
			//    else
			//    {
			//        if (anp1.Rows.Count > 0)
			//        {
			//            decimal headerGst = BizFunctions.Round((anp1_oriamt + anp1_origstamt) * Convert.ToDecimal(anph["gstper"]) / (100 + Convert.ToDecimal(anph["gstper"])));
			//            if (headerGst != anp1_origstamt)
			//            {
			//                anp1.Rows[0]["dorigstamt"] = Convert.ToDecimal(anp1.Rows[0]["dorigstamt"]) + headerGst - anp1_origstamt;
			//            }
			//        }

			//        #region initialise values
			//        anp1_discamt = 0;
			//        anp1_oriamt = 0;
			//        anp1_origstamt = 0;
			//        anp1_postamt = 0;
			//        anp1_gstamt = 0;
			//        anp1_grosamt = 0;
			//        anp1_roriamt = 0;
			//        anp1_rpostamt = 0;
			//        #endregion

			//        foreach (DataRow dr in anp1.Rows)
			//        {
			//            if (dr.RowState != DataRowState.Deleted)
			//            {
			//                dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
			//                dr["dgrosamt"] = dr["doriamt"];
			//                dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
			//                dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

			//                anp1_discamt += (decimal)dr["discamt"];
			//                anp1_oriamt += (decimal)dr["doriamt"];
			//                anp1_origstamt += (decimal)dr["dorigstamt"];
			//                anp1_postamt += (decimal)dr["dpostamt"];
			//                anp1_gstamt += (decimal)dr["dgstamt"];
			//                anp1_grosamt += (decimal)dr["dgrosamt"];
			//                anp1_roriamt += (decimal)dr["roriamt"];
			//                anp1_rpostamt += (decimal)dr["rpostamt"];
			//            }
			//        }
			//    }

			//    #endregion
			//    #endregion
			//}
			//else
			//{
			//    #region Steph - Exclusive GST calculations
			//    #region initialise values
			//    anp1_grosamt = 0;
			//    anp1_discamt = 0;
			//    anp1_oriamt = 0;
			//    anp1_origstamt = 0;
			//    anp1_postamt = 0;
			//    anp1_gstamt = 0;
			//    anp1_roriamt = 0;
			//    anp1_rpostamt = 0;
			//    myline = 0;
			//    line = 0;
			//    #endregion

			//    dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
			//            " WHERE gstgrpnum ='" + anph["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

			//    foreach (DataRow dr in anp1.Rows)
			//    {
			//        if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
			//        {
			//            BizFunctions.UpdateDataRow(anph, dr);
			//            BizFunctions.UpdateDataRow(anph, dr, "refnum/arnum/docunum/invnum/invdate/flag/trandate/period/status/user/modified/oricur/exrate/gstgrpnum/gstper");

			//            #region Steph - Adding of hscode and country of origin: 101109_1
			//            //reading hscode from pcat, instead matm after mh amended matm and pcat_24Nov2009_0934
			//            //dbaccess.ReadSQL("getHsCountry", "SELECT hscode, oricountry FROM matm WHERE matnum = '" + dr["matnum"].ToString().Trim() + "'");
			//            dbaccess.ReadSQL("getHsCountry", "SELECT pcatm.hscode AS hscode, matm.oricountry AS oricountry FROM matm "+
			//                " LEFT OUTER JOIN pcatm ON matm.pcatcode = pcatm.pcatcode "+
			//                " WHERE matm.matnum = '" + dr["matnum"].ToString().Trim() + "'");


			//            if (dbaccess.DataSet.Tables["getHsCountry"].Rows.Count > 0)
			//            {
			//                dr["hscode"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["hscode"];
			//                dr["oricountry"] = dbaccess.DataSet.Tables["getHsCountry"].Rows[0]["oricountry"];
			//            }
			//            #endregion


			//            if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
			//            {
			//                if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
			//                {
			//                    dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
			//                }
			//            }

			//            dr["dgrosamt"] = BizFunctions.Round((decimal)dr["dqty"] * (decimal)dr["price"]);
			//            dr["doriamt"] = (decimal)dr["dgrosamt"] - (decimal)dr["discamt"];
			//            //dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm "+
			//            //    " WHERE gstgrpnum ='"+dr["gstgrpnum"].ToString().Trim()+"' AND gsttype=3");
			//            if (dbaccess.DataSet.Tables["checkGST2"].Rows.Count > 0)
			//            {
			//                //steph  - do not have to calculate the gst, allow user to key in manually.
			//            }
			//            else
			//            {
			//                dr["dorigstamt"] = BizFunctions.Round((decimal)dr["doriamt"] * ((decimal)dr["gstper"] / 100));
			//            }
			//            dr["dpostamt"] = BizFunctions.Round((decimal)dr["doriamt"] * (decimal)dr["exrate"]);
			//            //dr["dgstamt"] = BizFunctions.Round((decimal)dr["dpostamt"] * ((decimal)anph["gstper"] / 100));
			//            dr["dgstamt"] = BizFunctions.Round((decimal)dr["dorigstamt"] * (decimal)dr["exrate"]);
			//            dr["roriamt"] = BizFunctions.Round((decimal)dr["doriamt"] + (decimal)dr["dorigstamt"]);
			//            dr["rpostamt"] = BizFunctions.Round((decimal)dr["dpostamt"] + (decimal)dr["dgstamt"]);


			//            //steph - to assign myline for print purpose
			//            if (Convert.ToDecimal(dr["dqty"]) > 0)
			//            {
			//                myline = myline + 1;
			//                dr["myline"] = myline;
			//            }
			//            else
			//            {
			//                dr["myline"] = 0;
			//            }

			//            #region Steph -  To replace the rest of the lines after the foc line as foc.
			//            //if (dr["foc"] != System.DBNull.Value && (bool)dr["foc"] != false)
			//            //{
			//            //    lineFocStart = Convert.ToDecimal(dr["line"]);
			//            //}
			//            #endregion

			//            //line = line + 100;
			//            //dr["line"] = line;

			//            anp1_discamt += (decimal)dr["discamt"];
			//            anp1_oriamt += (decimal)dr["doriamt"];
			//            anp1_origstamt += (decimal)dr["dorigstamt"];
			//            anp1_postamt += (decimal)dr["dpostamt"];
			//            anp1_gstamt += (decimal)dr["dgstamt"];
			//            anp1_grosamt += (decimal)dr["dgrosamt"];
			//            anp1_roriamt += (decimal)dr["roriamt"];
			//            anp1_rpostamt += (decimal)dr["rpostamt"];

			//            #region Steph - Pull Info from MATM
			//            dbaccess.ReadSQL("getMatm", "SELECT matname,uomcode,saleAcc FROM matm where matnum ='" + dr["matnum"].ToString().Trim() + "'");
			//            if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
			//            {
			//                if (dr["detail"].ToString().Trim() == "" || dr["detail"] == System.DBNull.Value)
			//                    dr["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
			//                if (dr["accnum"].ToString().Trim() == "" || dr["accnum"] == System.DBNull.Value)
			//                    dr["accnum"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["saleAcc"];
			//                if (dr["uom"].ToString().Trim() == "" || dr["uom"] == System.DBNull.Value)
			//                    dr["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
			//            }
			//            #endregion
			//        }
			//    }
			//    #endregion
			//}
			#endregion

			foreach (DataRow dr in anp1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(anph, dr, "status/user/trandate/period/docunum/created/modified/whnum/flag");

					if (Convert.ToDecimal(dr["dqty"]) > 0)
					{
						myline = myline + 1;
						dr["myline"] = myline;
					}
					else
					{
						dr["myline"] = 0;
					}

					anp1_totalQty = anp1_totalQty + Convert.ToDecimal(dr["dqty"]);
				}
			}

			anph["qty"] = anp1_totalQty;
			anph["discamt"] = anp1_discamt;
			anph["grosamt"] = anp1_grosamt;
			anph["origstamt"] = anp1_origstamt;
			anph["gstamt"] = anp1_gstamt;
			anph["oriamt"] = anp1_oriamt + anp1_origstamt;
			anph["postamt"] = anp1_postamt + anp1_gstamt;
			#endregion			

			setColumnChange("all", true);
			MDTReader.SetCorrectValue(ref this.dbaccess, "ANP1", "ANP");
		}
		#endregion

		#region Column Change Issue
		private void setColumnChange(string type, bool value)
		{
			type = type.ToLower();

			if (type == "header")
			{
				anphColumnChange = value;
			}
			if (type == "detail")
			{
				anp1ColumnChange = value;
			}
			if (type == "all")
			{
				anphColumnChange = value;
				anp1ColumnChange = value;
			}

		}
		#endregion

		#region Thousand Separator
		public static void ReBindsTextBox(string formName, string controlName, object datasource, string column, ConvertEventHandler formathandler)
		{
			TextBox txtBox = BizXmlReader.CurrentInstance.GetControl(formName, controlName) as TextBox;
			txtBox.DataBindings.Clear();

			Binding binding = new Binding("Text", datasource, column);
			binding.Format += new ConvertEventHandler(formathandler);

			txtBox.DataBindings.Add(binding);
		}


		private void DecimalToCurrencyString(object sender, ConvertEventArgs cevent)
		{
			/* This method is the Format event handler. Whenever the 
			   control displays a new value, the value is converted from 
			   its native Decimal type to a string. The ToString method 
			   then formats the value as a Currency, by using the 
			   formatting character "c". */

			// The application can only convert to string type. 
			if (cevent.DesiredType != typeof(string)) return;

			cevent.Value = ((decimal)cevent.Value).ToString("#,##0.00########");

			//IF you want to format the display to be some other format instead of currency format, use the one below
			//			cevent.Value = ((decimal)cevent.Value).ToString("#,##0.000");
		}
		#endregion
	}
}
