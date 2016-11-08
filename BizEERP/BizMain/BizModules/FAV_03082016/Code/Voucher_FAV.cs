/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_FAV.cs
 *	Description:   Journal Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Jer				2006-08-04			Add paste_handle, paste_onclick to enable/disable header columnchanged event
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Configuration;
using System.ComponentModel;
using System.Drawing;


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
using ATL.GeneralTools;
using ATL.FilterOP;
using ATL.MultiColumnComboBox;
using System.IO;
using System.Data.OleDb;

using ATL.BizLogicTools;

namespace ATL.FAV
{
	public class Voucher_FAV : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables
		protected DBAccess dbaccess = null;
		string formdDetailName;

		//FOR DETAILS
		protected GroupBox grpHeaderVoucherInfo;
		protected GroupBox grpHeaderTransInfo;

		protected DataGrid dgDetail;
        protected string currentDir = Environment.CurrentDirectory;
		protected TextBox txtVouchernum;
		protected TextBox txtDesc;
		protected ColumnComboBox cboApname;
		protected ColumnComboBox cboArname;
		protected ColumnComboBox cboAccname;
		protected DateTimePicker dtTrandate;
		protected DateTimePicker dtInvdate;
		protected TextBox txtAccnum;
		protected TextBox txtAccname;
		protected TextBox txtArname;
		protected TextBox txtApname;
		protected TextBox txtChequeno;
		protected TextBox txtOridebit;
		protected TextBox txtExrate;
		protected TextBox txtOricredit;
		protected TextBox txtOricur;
		//protected ComboBox cboOricur;
		protected Button btnAdd;
		protected Button btnUpdate;
		protected Button btnInsertTrans;
		protected Button btnNextTrans;
		protected Label txtCurrentAction;
		protected Button btnGetOB;
		protected DialogResult Save = DialogResult.No;
		protected string prevJour = null;
        protected ComboBox cboOricur;
		protected bool skipValidate;

		protected Button btnInsert;
		protected Button btnDelete;
		protected Button btnUp;
		protected Button btnDown;
		protected Button btnMark;
		protected Button btnDuplicate;
		protected Button btnExtract;
		protected Button btnClose;
        protected Button btn_Extract;
        protected string Flag ="";

		protected int TabDetail;
		
		public string documentKey = null;
		public string vouchernum = "";
		public string lastYear = "";
		
		GenTools genFunctions = new GenTools();
		getFilterOP FilterOP = new getFilterOP();
        int blnStandard;
		bool blnNew = true;
        protected DialogResult result = DialogResult.No;
		protected bool opened = false;

		#endregion

		public Voucher_FAV(string moduleName, Hashtable voucherBaseHelpers,string type) : base("VoucherGridInfo_FAV.xml", moduleName, voucherBaseHelpers)
		{
            this.Flag = type;
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
			base.AddVoucherAllCondition(e);
			e.Condition = "bfavh.flag='"+Flag+"' AND bfavh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (bfavh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" bfavh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" bfavh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND bfavh.flag='"+Flag+"' AND bfavh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

		protected override void Voucher_Form_OnLoad(object sender, VoucherEventArgs e)
		{
			base.Voucher_Form_OnLoad(sender, e);
		}


		protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
		{
			base.Document_TabControl_OnSelectionChanged(sender, e);
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			btnInsert = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Insert") as Button;
			btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
			btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
			btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
			btnClose = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Close") as Button;

				btnInsert.Enabled = false;
				btnDelete.Enabled = true;
				btnMark.Enabled = true;
				btnDuplicate.Enabled = false;

		}

		protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_TabControl_Handle(sender, e);

			TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
		}

		#region Document Event

		#region Form Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad (sender, e);
			opened = true;
			dbaccess = e.DBAccess;
			this.formdDetailName = (e.FormsCollection["header"] as Form).Name;
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			documentKey = e.DocumentKey;
			skipValidate = false;

			bfavh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			bfavh["oricur"] = "SGD";

			e.DBAccess.DataSet.Tables["bfavh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_FAV_ColumnChanged);

			lastYear = Convert.ToString(GetNumeric(Common.DEFAULT_SYSTEM_YEAR) - 1);
			if (bfavh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
			{
				bfavh["aropen"] = false;
				bfavh["apopen"] = false;
				bfavh["cshopen"] = false;
				bfavh["glopen"] = false;
			}

			InitializeControls();

			#region Steph - Set the current status of users action
			txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";
			#endregion
			
			grpHeaderVoucherInfo.Enabled = true; 
			grpHeaderTransInfo.Enabled = false;

			txtOridebit.Text = "0.00";
			txtOricredit.Text = "0.00";
			//txtExrate.Text = "0.00";

			MakeEnterEvent();
			MakeLostFocusEvent();

			InitialComboAccnum();
			InitialComboArnum();
			InitialComboApnum();

//			cboOricur.Text = "SGD";

			cboAccname.SelectedIndexChanged += new System.EventHandler(this.AccnumChanged_Combo);
			cboArname.SelectedIndexChanged += new EventHandler(cboArname_SelectedIndexChanged);
			cboApname.SelectedIndexChanged += new EventHandler(cboApname_SelectedIndexChanged);

//			cboOricur.SelectedIndexChanged += new EventHandler(cboOricur_SelectedIndexChanged);

			MakeF3DblClickEventsOnGrid();
			calcTotalDebitCredit();
//			setOricur();
			btnUpdate.Enabled = false;

			btnAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			btnInsertTrans.Click += new System.EventHandler(this.cmdInsert_Click);
			btnNextTrans.Click += new System.EventHandler(this.cmdNextTrans_Click);
			btnUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
			btnGetOB.Click += new EventHandler(this.cmdGetOB_Click);


            
            btn_Extract = BizXmlReader.CurrentInstance.GetControl(formdDetailName, "btn_Extract") as Button;

            btn_Extract.Click += new EventHandler(btn_Extract_Click);

			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(bfavh, dr, "user/flag/status/created/modified");
				}
			}
		}

        void btn_Extract_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
                DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
                decimal lineNo = 0;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
                openFileDialog.Filter = "XLS(*.XLS;*.XLSX;)|*.xls;*.xlsx;|All Files|*.*";
                openFileDialog.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
                DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
                string Path = (sender as OpenFileDialog).FileName;
                //Read data from Excel,and return the dataset
                DataSet ds = ExcelToDS(Path, "XSL", 0);



                if (bfav1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(bfav1);
                }

            

                for (int i = 0; i < ds.Tables["XSL"].Rows.Count; i++)
                {
                    DataRow insertBfav1 = bfav1.NewRow();

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][0]))
                    {
                        if (ds.Tables["XSL"].Rows[i][0].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["vouchernum"] = ds.Tables["XSL"].Rows[i][0].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][1]))
                    {
                        if (ds.Tables["XSL"].Rows[i][1].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["accnum"] = ds.Tables["XSL"].Rows[i][1].ToString();
                        }
                    }

                    if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][2]))
                    {
                        if (ds.Tables["XSL"].Rows[i][2].ToString().ToUpper().Trim() != string.Empty)
                        {
                            ds.Tables["XSL"].Rows[i][2] = 0;
                        }
                    }
                    insertBfav1["oridebit"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][2]);

                    if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][3]))
                    {
                        if (ds.Tables["XSL"].Rows[i][3].ToString().ToUpper().Trim() != string.Empty)
                        {
                            ds.Tables["XSL"].Rows[i][3] = 0;
                        }
                    }
                    insertBfav1["oricredit"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][3]);

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][4]))
                    {
                        if (ds.Tables["XSL"].Rows[i][4].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["detail"] = ds.Tables["XSL"].Rows[i][4].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][5]))
                    {
                        if (ds.Tables["XSL"].Rows[i][5].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["apnum"] = ds.Tables["XSL"].Rows[i][5].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][6]))
                    {
                        if (ds.Tables["XSL"].Rows[i][6].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["arnum"] = ds.Tables["XSL"].Rows[i][6].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][7]))
                    {
                        if (ds.Tables["XSL"].Rows[i][7].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["invnum"] = ds.Tables["XSL"].Rows[i][7].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][8]))
                    {
                        if (ds.Tables["XSL"].Rows[i][8].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["invdate"] = Convert.ToDateTime(ds.Tables["XSL"].Rows[i][8]);
                        }
                    }


                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][9]))
                    {
                        if (ds.Tables["XSL"].Rows[i][9].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["chknum"] = ds.Tables["XSL"].Rows[i][9].ToString();
                        }
                    }





                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][10]))
                    {
                        if (ds.Tables["XSL"].Rows[i][10].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["oricur"] = ds.Tables["XSL"].Rows[i][10].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][11]))
                    {
                        if (ds.Tables["XSL"].Rows[i][11].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["ccnum"] = ds.Tables["XSL"].Rows[i][11].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][12]))
                    {
                        if (ds.Tables["XSL"].Rows[i][12].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["projectid"] = ds.Tables["XSL"].Rows[i][12].ToString();
                        }
                    }

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][13]))
                    {
                        if (ds.Tables["XSL"].Rows[i][13].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["sitenum"] = ds.Tables["XSL"].Rows[i][13].ToString();
                        }
                    }


                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][14]))
                    {
                        if (ds.Tables["XSL"].Rows[i][14].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertBfav1["jobid"] = ds.Tables["XSL"].Rows[i][14].ToString();
                        }
                    }




                    bfav1.Rows.Add(insertBfav1);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }   
        }

        #region ExcelToDS
        public DataSet ExcelToDS(string Path, string tablename, int sheetIndex)
        {
            string os_platform = System.Environment.OSVersion.Platform.ToString();
            string strConn = "";

            if (BizLogicTools.Tools.Platform == "x86")
            {
                strConn = "Provider = Microsoft.Jet.OLEDB.4.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            }
            else
            {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            }
     
           
          
            OleDbConnection conn = new OleDbConnection(strConn);

            conn.Open();
            System.Data.DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dbSchema == null || dbSchema.Rows.Count < 1)
            {
                throw new Exception("Error: Could not get Excel schema table.");
            }
            string sheetName = "[" + dbSchema.Rows[sheetIndex]["TABLE_NAME"].ToString() + "]";
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from " + sheetName;
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, tablename);
            conn.Close();
            return ds;
        }
        #endregion

		void Voucher_FAV_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			switch (e.Column.ColumnName)
			{
                // Replaced by Wen Yue: 20-05-2016
				case "jvtemplate":
					#region Steph - Copy from JV Template
					this.Save = MessageBox.Show("Overwrite existing Journal details?", "Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

					DataSet dstempfavt1 = this.dbaccess.ReadSQLTemp("favt1", "SELECT * FROM favt1 WHERE refnum='" + bfavh["jvtemplate"].ToString().Trim()+ "'");
					DataTable dttempfavt1 = dstempfavt1.Tables["favt1"];

					if (this.Save == DialogResult.Yes)
					{
						DataRow tempbfav1;
						BizFunctions.DeleteAllRows(bfav1);
						//Get bug1 data
						for (int i = 0; i < dttempfavt1.Rows.Count; i++)
						{

							tempbfav1 = bfav1.NewRow();

							if (dttempfavt1.Rows[i].RowState != DataRowState.Deleted)
							{
								tempbfav1["vouchernum"] = dttempfavt1.Rows[i]["vouchernum"];
								tempbfav1["accnum"] = dttempfavt1.Rows[i]["accnum"];
								tempbfav1["desc"] = dttempfavt1.Rows[i]["desc"];
								tempbfav1["trandate"] = bfavh["trandate"];
                                tempbfav1["oridebit"] = dttempfavt1.Rows[i]["oridebit"];
                                tempbfav1["oricredit"] = dttempfavt1.Rows[i]["oricredit"];
								tempbfav1["detail"] = dttempfavt1.Rows[i]["detail"];
								tempbfav1["apnum"] = dttempfavt1.Rows[i]["apnum"];
								tempbfav1["arnum"] = dttempfavt1.Rows[i]["arnum"];
								tempbfav1["oricur"] = dttempfavt1.Rows[i]["oricur"];
								tempbfav1["exrate"] = dttempfavt1.Rows[i]["exrate"];
								tempbfav1["oricur"] = dttempfavt1.Rows[i]["oricur"];

								bfav1.Rows.Add(tempbfav1.ItemArray);
							}
						}
					}
					else
					{
						bfavh["refnum"] = this.prevJour;
					}
					refreshBfav1();
					#endregion
					break;	
				case "oricur":
					#region Steph - get latest exrate from exr table
					bfavh["exrate"] = BizAccounts.GetExRate(dbaccess, bfavh["oricur"].ToString().Trim(), BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(bfavh["trandate"])));
					#endregion
					break;
			}

		}

		void cboApname_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtApname.Text = "";
			if (cboApname.Text != "")
			{
				txtApname.Text = cboApname["apnum"].ToString().Trim();
			}
		}

		void cboArname_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtArname.Text = "";
			if (cboArname.Text != "")
			{
				txtArname.Text = cboArname["arnum"].ToString().Trim();
			}
		}

	
			
		void txtOricredit_LostFocus(object sender, EventArgs e)
		{
			try
			{
				decimal getCreditValue = Convert.ToDecimal(txtOricredit.Text);
				if (txtOricredit.Text != "")
				{
					txtOricredit.Text= string.Format("{0:0.00}", getCreditValue);
				}
			}
			catch
			{
				MessageBox.Show("Invalid value keyed in Credit textfield! Please check");
				txtOricredit.Focus();
				return;
			}
			
		}

		void txtOridebit_LostFocus(object sender, EventArgs e)
		{
			try
			{
				decimal getDebitValue = Convert.ToDecimal(txtOridebit.Text);
				if (txtOridebit.Text != "")
				{
					txtOridebit.Text = string.Format("{0:0.00}", getDebitValue);
				}
			}
			catch
			{
				MessageBox.Show("Invalid value keyed in Debit textfield! Please check");
				txtOridebit.Focus();
				return;
			}
		}

		#endregion

		#region Save & Confirm
		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

            foreach (DataRow dr1 in bfav1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(bfavh, dr1, "refnum/user/coy/flag/status/created/modified/systemyear");
                }
            }
           
		}

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick(sender, e);
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

		}

		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			e.Handle = true;

			if ((bool)bfavh["aropen"] != true)
			{
				if ((bool)bfavh["apopen"] != true)
				{
					if ((bool)bfavh["cshopen"] != true)
					{
						DataTable checkBalance = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT SUM(postamt) FROM [bfav1] HAVING SUM(postamt)<>0");
						if (checkBalance.Rows.Count > 0)
						{
							MessageBox.Show("The Voucher Posting Is Not Balance!", "Voucher Is Not Confirmed!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

							e.Handle = false;
						}
					}
				}
			}

			dbaccess.ReadSQL("acm", "select * from acm");

			if(e.Handle != false)
			{
				#region Steph - Posting Time!

				if ((bool)bfavh["aropen"] == true)
				{
					#region Steph - Saving into ARD
					string saveArd = "select refnum,vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,"+
						" sum(oricredit) as oricredit,arnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period "+
						" from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' "+
						" AND accnum in (select accnum from [acm] where acctype = '7') "+
						" GROUP BY refnum,vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate,arnum";
					DataTable getArd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveArd);

					foreach (DataRow dr in getArd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addArd = ard.Rows.Add(new object[] { });
							addArd["refnum"] = dr["refnum"];
							addArd["docunum"] = dr["vouchernum"];
							addArd["trandate"] = dr["trandate"];
							addArd["chknum"] = dr["chknum"];
							addArd["accnum"] = dr["accnum"];
							addArd["locno"] = dr["locno"];
							addArd["deptno"] = dr["deptno"];
							addArd["detail"] = dr["detail"];
							addArd["oridebit"] = dr["oridebit"];
							addArd["oricredit"] = dr["oricredit"];
							addArd["arnum"] = dr["arnum"];
							addArd["invnum"] = dr["invnum"];
							addArd["oldref"] = dr["oldref"];
							addArd["invdate"] = dr["invdate"];
							addArd["lgr"] = "ARD";
							addArd["coy"] = "SAF";
							addArd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addArd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addArd["period"] = dr["period"];
							addArd["oricur"] = dr["oricur"];
							addArd["exrate"] = dr["exrate"];
							addArd["gstamt"] = 0;
							addArd["exramt"] = 0;
							addArd["oriamt"] = addArd["doriamt"];
							addArd["postamt"] = addArd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in ard.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}


					#endregion
				}
				else if ((bool)bfavh["apopen"] == true)
				{
					#region Steph - Saving into APD
					string saveApd = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,"+
						" sum(oricredit) as oricredit,apnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period "+
						" from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "' "+
						" AND accnum in (select accnum from [acm] where acctype = '12') "+
						" GROUP BY vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate,apnum";

					DataTable getApd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveApd);

					foreach (DataRow dr in getApd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addApd = apd.Rows.Add(new object[] { });
							addApd["docunum"] = dr["vouchernum"];
							addApd["trandate"] = dr["trandate"];
							addApd["chknum"] = dr["chknum"];
							addApd["accnum"] = dr["accnum"];
							addApd["locno"] = dr["locno"];
							addApd["deptno"] = dr["deptno"];
							addApd["detail"] = dr["detail"];
							addApd["oridebit"] = dr["oridebit"];
							addApd["oricredit"] = dr["oricredit"];
							addApd["apnum"] = dr["apnum"];
							addApd["invnum"] = dr["invnum"];
							addApd["oldref"] = dr["oldref"];
							addApd["invdate"] = dr["invdate"];
							addApd["lgr"] = "APD";
							addApd["coy"] = "SAF";
							addApd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addApd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addApd["period"] = dr["period"];
							addApd["oricur"] = dr["oricur"];
							addApd["exrate"] = dr["exrate"];
							addApd["gstamt"] = 0;
							addApd["exramt"] = 0;
							addApd["oriamt"] = addApd["doriamt"];
							addApd["postamt"] = addApd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in apd.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion
				}
				else if ((bool)bfavh["cshopen"] == true)
				{
					#region Steph - Saving into Csh
					string saveCsh = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '5' OR acctype = '6') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getCsh = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveCsh);
                    DataTable getCsh = this.dbaccess.ReadSQLTemp("getCsh", saveCsh).Tables[0];
					foreach (DataRow dr in getCsh.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							DataRow addCsh = csh.Rows.Add(new object[] { });
							addCsh["docunum"] = dr["vouchernum"];
							addCsh["trandate"] = dr["trandate"];
							addCsh["chknum"] = dr["chknum"];
							addCsh["accnum"] = dr["accnum"];
							addCsh["locno"] = dr["locno"];
							addCsh["deptno"] = dr["deptno"];
							addCsh["detail"] = dr["detail"];
							addCsh["oridebit"] = dr["oridebit"];
							addCsh["oricredit"] = dr["oricredit"];
							addCsh["apnum"] = dr["apnum"];
							addCsh["lgr"] = "CSH";
							addCsh["coy"] = "SAF";
							addCsh["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addCsh["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addCsh["period"] = dr["period"];
							addCsh["oricur"] = dr["oricur"];
							addCsh["exrate"] = dr["exrate"];
						}
					}

					foreach (DataRow dr2 in csh.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}
					#endregion
				}
				else if ((bool)bfavh["glopen"] == true)
				{
					#region Steph - Saving all into GLD table.
					string saveGld = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getGld = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveGld);
                    DataTable getGld = this.dbaccess.ReadSQLTemp("getGld",saveGld).Tables[0];
					foreach (DataRow dr in getGld.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addGld = gld.Rows.Add(new object[] { });
							addGld["docunum"] = dr["vouchernum"];
							addGld["trandate"] = dr["trandate"];
							addGld["accnum"] = dr["accnum"];
							addGld["locno"] = dr["locno"];
							addGld["deptno"] = dr["deptno"];
							addGld["detail"] = dr["detail"];
							addGld["oridebit"] = dr["oridebit"];
							addGld["oricredit"] = dr["oricredit"];
							addGld["arnum"] = dr["arnum"];
							addGld["lgr"] = "GLD";
							addGld["coy"] = "SAF";
							addGld["period"] = dr["period"];
							addGld["oricur"] = dr["oricur"];
							addGld["exrate"] = dr["exrate"];
							addGld["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addGld["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
						}
					}

					foreach (DataRow dr2 in gld.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion
				}
				else
				{
					#region Steph - Saving into ARD
					dbaccess.ReadSQL("acm", "select * from acm");
					string saveArd = "select refnum,vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '7') GROUP BY refnum,vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate";
                    DataSet ds=this.dbaccess.ReadSQLTemp("getArd",saveArd);
                    DataTable getArd=ds.Tables["getArd"];
                    //DataTable getArd = BizFunctions..ExecuteQuery(dbaccess.DataSet, saveArd);

					foreach (DataRow dr in getArd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addArd = ard.Rows.Add(new object[] { });
							addArd["refnum"] = dr["refnum"];
							addArd["docunum"] = dr["vouchernum"];
							addArd["trandate"] = dr["trandate"];
							addArd["chknum"] = dr["chknum"];
							addArd["accnum"] = dr["accnum"];
							addArd["locno"] = dr["locno"];
							addArd["deptno"] = dr["deptno"];
							addArd["detail"] = dr["detail"];
							addArd["oridebit"] = dr["oridebit"];
							addArd["oricredit"] = dr["oricredit"];
							addArd["arnum"] = dr["arnum"];
							addArd["invnum"] = dr["invnum"];
							addArd["oldref"] = dr["oldref"];
							addArd["invdate"] = dr["invdate"];
							addArd["lgr"] = "ARD";
							addArd["coy"] = "SAF";
							addArd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addArd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addArd["period"] = dr["period"];
							addArd["oricur"] = dr["oricur"];
							addArd["exrate"] = dr["exrate"];
							addArd["gstamt"] = 0;
							addArd["exramt"] = 0;
							addArd["oriamt"] = addArd["doriamt"];
							addArd["postamt"] = addArd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in ard.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}


					#endregion

					#region Steph - Saving into APD
					string saveApd = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '12') GROUP BY vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getApd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveApd);
                    DataTable getApd = this.dbaccess.ReadSQLTemp("getApd", saveApd).Tables[0];

					foreach (DataRow dr in getApd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addApd = apd.Rows.Add(new object[] { });
							addApd["docunum"] = dr["vouchernum"];
							addApd["trandate"] = dr["trandate"];
							addApd["chknum"] = dr["chknum"];
							addApd["accnum"] = dr["accnum"];
							addApd["locno"] = dr["locno"];
							addApd["deptno"] = dr["deptno"];
							addApd["detail"] = dr["detail"];
							addApd["oridebit"] = dr["oridebit"];
							addApd["oricredit"] = dr["oricredit"];
							addApd["apnum"] = dr["apnum"];
							addApd["invnum"] = dr["invnum"];
							addApd["oldref"] = dr["oldref"];
							addApd["invdate"] = dr["invdate"];
							addApd["lgr"] = "APD";
							addApd["coy"] = "SAF";
							addApd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addApd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addApd["period"] = dr["period"];
							addApd["oricur"] = dr["oricur"];
							addApd["exrate"] = dr["exrate"];
							addApd["gstamt"] = 0;
							addApd["exramt"] = 0;
							addApd["oriamt"] = addApd["doriamt"];
							addApd["postamt"] = addApd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in apd.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion

					#region Steph - Saving into Csh
					string saveCsh = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '5' OR acctype = '6') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getCsh = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveCsh);
                    DataTable getCsh = this.dbaccess.ReadSQLTemp("getCsh", saveCsh).Tables[0];
					foreach (DataRow dr in getCsh.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							DataRow addCsh = csh.Rows.Add(new object[] { });
							addCsh["docunum"] = dr["vouchernum"];
							addCsh["trandate"] = dr["trandate"];
							addCsh["chknum"] = dr["chknum"];
							addCsh["accnum"] = dr["accnum"];
							addCsh["locno"] = dr["locno"];
							addCsh["deptno"] = dr["deptno"];
							addCsh["detail"] = dr["detail"];
							addCsh["oridebit"] = dr["oridebit"];
							addCsh["oricredit"] = dr["oricredit"];
							addCsh["apnum"] = dr["apnum"];
							addCsh["lgr"] = "CSH";
							addCsh["coy"] = "SAF";
							addCsh["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addCsh["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addCsh["period"] = dr["period"];
							addCsh["oricur"] = dr["oricur"];
							addCsh["exrate"] = dr["exrate"];
						}
					}

					foreach (DataRow dr2 in csh.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}
					#endregion

					#region Steph - Saving into SIV
					string saveSiv = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '0' OR acctype = '1') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getSiv = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveSiv);
                    DataTable getSiv = this.dbaccess.ReadSQLTemp("getSiv", saveSiv).Tables[0];

					foreach (DataRow dr in getSiv.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addSiv = siv1.Rows.Add(new object[] { });
							addSiv["docunum"] = dr["vouchernum"];
							addSiv["trandate"] = dr["trandate"];
							addSiv["accnum"] = dr["accnum"];
							addSiv["locno"] = dr["locno"];
							addSiv["deptno"] = dr["deptno"];
							addSiv["detail"] = dr["detail"];
							addSiv["oridebit"] = dr["oridebit"];
							addSiv["oricredit"] = dr["oricredit"];
							addSiv["arnum"] = dr["arnum"];
							addSiv["invnum"] = dr["invnum"];
							addSiv["invdate"] = dr["invdate"];
							addSiv["coy"] = "SAF";
							addSiv["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addSiv["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addSiv["period"] = dr["period"];
							addSiv["oricur"] = dr["oricur"];
							addSiv["exrate"] = dr["exrate"];
							addSiv["gstamt"] = 0;
							addSiv["gstper"] = 0;
							addSiv["oriamt"] = addSiv["doriamt"];
							addSiv["postamt"] = addSiv["dpostamt"];
						}
					}

					foreach (DataRow dr2 in siv1.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}

					#endregion

					#region Steph - Saving into PIV
					string savePiv = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '2' OR acctype = '3') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getPiv = BizFunctions.ExecuteQuery(dbaccess.DataSet, savePiv);
                    DataTable getPiv =this.dbaccess.ReadSQLTemp("getPiv",savePiv).Tables[0];

					foreach (DataRow dr in getPiv.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addPiv = piv1.Rows.Add(new object[] { });
							addPiv["docunum"] = dr["vouchernum"];
							addPiv["trandate"] = dr["trandate"];
							//addPiv["chknum"] = dr["chknum"];
							addPiv["accnum"] = dr["accnum"];
							addPiv["locno"] = dr["locno"];
							addPiv["deptno"] = dr["deptno"];
							addPiv["detail"] = dr["detail"];
							addPiv["oridebit"] = dr["oridebit"];
							addPiv["oricredit"] = dr["oricredit"];
							addPiv["apnum"] = dr["apnum"];
							addPiv["invnum"] = dr["invnum"];
							addPiv["invdate"] = dr["invdate"];
							addPiv["lgr"] = "PIV";
							addPiv["coy"] = "SAF";
							addPiv["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addPiv["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addPiv["period"] = dr["period"];
							addPiv["oricur"] = dr["oricur"];
							addPiv["exrate"] = dr["exrate"];
							addPiv["gstamt"] = 0;
							addPiv["gstper"] = 0;
							addPiv["oriamt"] = addPiv["doriamt"];
							addPiv["postamt"] = addPiv["dpostamt"];
						}
					}

					foreach (DataRow dr2 in piv1.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}
					#endregion

					#region Steph - Saving all into GLD table.
                    string saveGld = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period,ccnum,projectid,sitenum from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate,ccnum,projectid,sitenum";

                    //DataTable getGld = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveGld);
                    DataTable getGld = this.dbaccess.ReadSQLTemp("getGld", saveGld).Tables[0];
					foreach (DataRow dr in getGld.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addGld = gld.Rows.Add(new object[] { });
							addGld["docunum"] = dr["vouchernum"];
							addGld["trandate"] = dr["trandate"];
							addGld["accnum"] = dr["accnum"];
							addGld["locno"] = dr["locno"];
							addGld["deptno"] = dr["deptno"];
							addGld["detail"] = dr["detail"];
							addGld["oridebit"] = dr["oridebit"];
							addGld["oricredit"] = dr["oricredit"];
							addGld["arnum"] = dr["arnum"];
							addGld["lgr"] = "GLD";
							addGld["coy"] = "SAF";
							addGld["period"] = dr["period"];
							addGld["oricur"] = dr["oricur"];
							addGld["exrate"] = dr["exrate"];

                            addGld["ccnum"] = dr["ccnum"];
                            addGld["projectid"] = dr["projectid"];
                            addGld["sitenum"] = dr["sitenum"];

							addGld["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addGld["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
						}
					}

					foreach (DataRow dr2 in gld.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion
				}


				#endregion
			}
		}

		#endregion

		protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick(sender, e);

			DataRow bfavh = e.DBAccess.DataSet.Tables["bfavh"].Rows[0];
			Hashtable selectedCollection = new Hashtable();

			switch (e.ReportName)
			{
				case "Journal":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + bfavh["arnum"].ToString().Trim() + "'");
					selectedCollection.Add("bfav1det", "SELECT bfav1.accnum AS accnum,acm.accname AS accname,"+
						" bfav1.detail AS detail,bfav1.oricur AS oricur,bfav1.oricredit AS oricredit,bfav1.oridebit AS oridebit FROM bfav1 "+
						" LEFT OUTER JOIN acm on acm.accnum = bfav1.accnum "+
						" where bfav1.refnum='" + bfavh["refnum"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;
			}            
		}

		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable acm = dbaccess.DataSet.Tables["acm"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			if (bfavh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
			{
				txtCurrentAction.Text = "Voucher is closed!";
				return;
			}

			bfavh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(bfavh["trandate"]));
			bfavh["coy"] = "SAF";

			refreshBfav1();





            

		}
		#endregion

		#region Reopen
		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
				
			DataRow bfavh = this.dbaccess.DataSet.Tables["bfavh"].Rows[0];

            if (BizValidate.ChkPeriodLocked(e.DBAccess, bfavh["period"].ToString()))
            {
                MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
            }


			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM SIV1 WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM ARD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM PIV1 WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM APD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM CSH" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");

		}
		#endregion



		#endregion

		#region Steph - Events for Batch Payment Entry into detail

		private void InitializeControls()
		{
			grpHeaderVoucherInfo = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "grp_VoucherInfo") as GroupBox;
			grpHeaderTransInfo = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "grp_TransactionInfo") as GroupBox;

			dgDetail = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "dg_detail") as DataGrid;

			txtVouchernum = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_vouchernum") as TextBox;
			txtDesc = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_desc") as TextBox;
			cboArname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_arname") as ColumnComboBox;
			cboApname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_apname") as ColumnComboBox;
			cboAccname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_accname") as ColumnComboBox;
			txtAccname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_accname") as TextBox;
			txtArname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_arname") as TextBox;
			txtApname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_apname") as TextBox;
			txtChequeno = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_chequeno") as TextBox;
			dtTrandate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_trandate") as DateTimePicker;
			dtInvdate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_invdate") as DateTimePicker;
			txtAccnum = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_accnum") as TextBox;
			txtOridebit = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_oridebit") as TextBox;
			txtOricredit = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_oricredit") as TextBox;
			txtOricur = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "bfavh_oricur") as TextBox;
			//txtExrate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_exrate") as TextBox;
			//cboOricur = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_oricur") as  ComboBox;
			txtCurrentAction = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_currentAction") as Label;

			btnAdd= BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Add") as Button;
			btnGetOB = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_getOB") as Button;
			btnUpdate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Update") as Button;
			btnInsertTrans = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Insert") as Button;
			btnNextTrans = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_NextTrans") as Button;
		}

		private void calcTotalDebitCredit ()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			setDefaults(dbaccess.DataSet, "bfav1");
	
			decimal countTotalDebit = 0;
			decimal countTotalCredit = 0;
			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					//countTotalDebit += (decimal)dr["oridebit"];
					//countTotalCredit += (decimal)dr["oricredit"];

					if (Convert.ToDecimal(dr["postamt"])> 0)
					{
						countTotalDebit += (decimal)dr["postamt"];
					}
					else
					{
						countTotalCredit += Math.Abs((decimal)dr["postamt"]);
					}

				}
			}

			bfavh["debit"] = countTotalDebit;
			bfavh["credit"] = countTotalCredit;
			bfavh["oriamt"] = countTotalDebit - countTotalCredit;
		}
		
		private void MakeEnterEvent()
		{
			foreach (Control crlControl in grpHeaderVoucherInfo.Controls)
			{
				crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
				crlControl.Validating += new CancelEventHandler(crlControl_Validating);
			}

			foreach (Control crlControl in grpHeaderTransInfo.Controls)
			{
				crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
			}

		}

		void crlControl_Validating(object sender, CancelEventArgs e)
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0]; 
			string strExistVoucher = "select vouchernum from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "'";
			DataTable existVoucher = BizFunctions.ExecuteQuery(dbaccess.DataSet, strExistVoucher);
			bool checkOnce = true;
			foreach (DataRow dr in existVoucher.Rows)
			{
				if (checkOnce == true)
				{
					if(skipValidate == false)
					if (txtVouchernum.Text.Trim() == dr["vouchernum"].ToString().Trim())
					{
						MessageBox.Show("This Journal No. exist in this voucher! Please proceed to key in the Transactions Info!");
						#region Steph - Copied from F11 in order to get the values for all the field in Voucher Info for Existing Data
						vouchernum = txtVouchernum.Text.ToString();
						txtVouchernum.Focus();
						DataTable editHeader = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select vouchernum,apname,arname,detail,trandate,chknum,invdate from bfav1 where vouchernum = '" + vouchernum + "'");
						if (editHeader.Rows.Count > 0)
						{
							txtVouchernum.Text = editHeader.Rows[0]["vouchernum"].ToString();
							cboApname.Text = editHeader.Rows[0]["apname"].ToString();
							cboArname.Text = editHeader.Rows[0]["arname"].ToString();
							//txtDesc.Text = editHeader.Rows[0]["detail"].ToString();
							dtTrandate.Text = editHeader.Rows[0]["trandate"].ToString();
							dtInvdate.Text = editHeader.Rows[0]["invdate"].ToString();
							txtChequeno.Text = editHeader.Rows[0]["chknum"].ToString();
						}
						#endregion
						add_Click();
						checkOnce = false;
					}
				}
			}
			skipValidate = true;
			
		}		
		
		private void SendTabForEnter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			if (e.KeyCode == Keys.Enter)
			{
				if (txtVouchernum.Text.ToString().Trim() == String.Empty)
				{
					MessageBox.Show("Voucher No. Cannot Be Empty!");
				}
				if (txtVouchernum.Text.ToString().Trim() != String.Empty)
				{
					SendKeys.Send("{Tab}");
				}
			}

			if (e.KeyCode == Keys.F5)
			{
				btnUpdate.Focus();
				SendKeys.Send("{Enter}");
			}

			if (e.KeyCode == Keys.F8)
			{
				btnNextTrans.Focus();
				SendKeys.Send("{Enter}");
			}

			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			if (e.KeyCode == Keys.F11)
			{
				F11Event();
			}

			if (e.KeyCode == Keys.F12)
			{
				F12Event();
			}
			
		}

		private void F12Event()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			if (vouchernum != "")
			{
				foreach (DataRow dr in bfav1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						if (dr["vouchernum"].ToString() == vouchernum)
						{
							dr["vouchernum"] = txtVouchernum.Text;
							dr["trandate"] = dtTrandate.Value;
							dr["invdate"] = dtInvdate.Value;
							dr["apnum"] = cboApname["apnum"];
							dr["apname"] = cboApname.Text;
							dr["arnum"] = cboArname["arnum"];
							dr["arname"] = cboArname["arname"];
							//dr["detail"] = txtDesc.Text;
							dr["chknum"] = txtChequeno.Text;
						}
					}
				}
				txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";
				skipValidate = false;
			}

			dgDetail.Enabled = true;
			grpHeaderVoucherInfo.Enabled = true;
			grpHeaderTransInfo.Enabled = false;
			btnAdd.Enabled = true;

			ClearTransaction();
			txtVouchernum.Focus();
		}

		private void F11Event()
		{
			if (txtVouchernum.Text.ToString().Trim() != "")
			{
				vouchernum = txtVouchernum.Text.ToString();
				txtVouchernum.Focus();
				DataTable editHeader = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select vouchernum,apname,arname,detail,trandate,chknum,invdate from bfav1 where vouchernum = '" + vouchernum + "'");
				if (editHeader.Rows.Count > 0)
				{
					txtVouchernum.Text = editHeader.Rows[0]["vouchernum"].ToString();
					cboApname.Text = editHeader.Rows[0]["apname"].ToString();
					cboArname.Text = editHeader.Rows[0]["arname"].ToString();
					//txtDesc.Text = editHeader.Rows[0]["detail"].ToString();
					dtTrandate.Text = editHeader.Rows[0]["trandate"].ToString();
					dtInvdate.Text = editHeader.Rows[0]["invdate"].ToString();
					txtChequeno.Text = editHeader.Rows[0]["chknum"].ToString();

					txtCurrentAction.Text = "Updating Voucher Info, press F12 after updating is done!";

					//System.Drawing.Color.RoyalBlue
					dgDetail.Enabled = false;
					grpHeaderTransInfo.Enabled = false;
					grpHeaderVoucherInfo.Enabled = true;

					btnAdd.Enabled = false;
					btnNextTrans.Enabled = false;
					btnUpdate.Enabled = true;
					btnInsertTrans.Enabled = false;
					blnNew = false;
					skipValidate = true;
				}
				else
				{
					MessageBox.Show("The Voucher No. keyed in is not found! Please check!");
				}
			}
		}
		private void cmdNextTrans_Click(object sender, EventArgs e)
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			grpHeaderVoucherInfo.Enabled = true;
			grpHeaderTransInfo.Enabled = false;

			txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";

			ClearTransaction();
			txtVouchernum.Focus();
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			add_Click();
		}

		private void cmdGetOB_Click(object sender, EventArgs e)
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			
			
			if ((bool)bfavh["aropen"] == true)
			{
				FilterOP.ShowDialog();
				getAROpening();

			}
			if ((bool)bfavh["apopen"] == true)
			{
				FilterOP.ShowDialog();
				getAPOpening();

			}
			if ((bool)bfavh["cshopen"] == true)
			{
				FilterOP.ShowDialog();
				getCSHOpening();
			}
			if ((bool)bfavh["glopen"] == true)
			{
				FilterOP.ShowDialog();
				getGLOpening();
			}

		}

		private void add_Click()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			if (txtVouchernum.Text.ToString().Trim() == "")
			{
				MessageBox.Show("Voucher No. Cannot Be Empty!");
				txtVouchernum.Focus();
			}

			//steph_29 Mar 2010_1433 - to take from exr table if the exrate is zero

			bfavh["exrate"] = BizAccounts.GetExRate(dbaccess, bfavh["oricur"].ToString().Trim(), BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(bfavh["trandate"])));

			if (txtVouchernum.Text.ToString().Trim() != "")
			{
				grpHeaderVoucherInfo.Enabled = false;
				grpHeaderTransInfo.Enabled = true;

				txtCurrentAction.Text = "OPEN For Transactions Info Data Entry";
				btnInsertTrans.Enabled = true;
				btnNextTrans.Enabled = true;


				calcTotalDebitCredit();

				cboAccname.Focus();
			}
		}

		private DataRow getcurrentrow(DataGrid datagrid)
		{
			CurrencyManager cm = (CurrencyManager)datagrid.BindingContext[datagrid.DataSource, datagrid.DataMember];
			DataRowView drv = cm.Current as DataRowView;
			DataRow dr = drv.Row;

			return dr;
		}

		private void cmdUpdate_Click(object sender, EventArgs e)
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			dgDetail.Enabled = true;
			

			if (blnNew)
			{
				DataRow addRow = bfav1.Rows.Add(new object[] { });
				addRow["line"] = intGetLine();
				addRow["vouchernum"] = txtVouchernum.Text;
				addRow["detail"] = txtDesc.Text;
				addRow["apnum"] = cboApname["apnum"];
				addRow["apname"] = cboApname.Text;
				addRow["arnum"] = cboArname["arnum"];
				addRow["arname"] = cboArname["arname"];
				addRow["trandate"] = dtTrandate.Value;
				addRow["accnum"] = cboAccname["accnum"];
				addRow["accname"] = cboAccname["accname"];
				addRow["locno"] = bfavh["locno"];
				addRow["deptno"] = bfavh["deptno"];
				addRow["chknum"] = txtChequeno.Text;
				addRow["oridebit"] = GetNumeric(txtOridebit.Text);
				addRow["oricredit"] = GetNumeric(txtOricredit.Text);
				addRow["exrate"] =bfavh["exrate"];
				addRow["oricur"] = bfavh["oricur"];
				addRow["invnum"] = txtVouchernum.Text;
				addRow["invdate"] = dtInvdate.Value;
			}
			else
			{
                //Jason :18062015: Error after adding some columns
                //int intRow = dgDetail.CurrentRowIndex;
                //DataRow currentRow = getcurrentrow(dgDetail);
                //dgDetail[intRow, 2] = txtVouchernum.Text;
                //dgDetail[intRow, 3] = dtTrandate.Value;
                //if (cboAccname.Text != "")
                //{
                //    dgDetail[intRow, 4] = cboAccname["accnum"];
                //    dgDetail[intRow, 5] = cboAccname["accname"];
                //}
                //dgDetail[intRow, 6] = GetNumeric(txtOridebit.Text);
                //dgDetail[intRow, 7] = GetNumeric(txtOricredit.Text);			
                //dgDetail[intRow, 8] = txtDesc.Text;
                //if (cboApname.Text != "")
                //{
                //    dgDetail[intRow, 9] = cboApname["apnum"];
                //    dgDetail[intRow, 10] = cboApname["apname"];
                //}
                //if (cboArname.Text != "")
                //{
                //    dgDetail[intRow, 11] = cboArname["arnum"];
                //    dgDetail[intRow, 12] = cboArname["arname"];
                //}
                //dgDetail[intRow, 13] = txtVouchernum.Text;
                //dgDetail[intRow, 14] = dtInvdate.Value;
                //dgDetail[intRow, 15] = txtChequeno.Text;
                //dgDetail[intRow, 16] = bfavh["oricur"];
                //dgDetail[intRow, 17] = bfavh["exrate"];
                //dgDetail[intRow, 18] = bfavh["locno"];
                //dgDetail[intRow, 19] = bfavh["deptno"];
				
                //currentRow["vouchernum"] = txtVouchernum.Text;


                int intRow = dgDetail.CurrentRowIndex;
                DataRow currentRow = getcurrentrow(dgDetail);
                dgDetail[intRow, 1] = txtVouchernum.Text;
                dgDetail[intRow, 2] = dtTrandate.Value;
                if (cboAccname.Text != "")
                {
                    dgDetail[intRow, 3] = cboAccname["accnum"];
                    dgDetail[intRow, 4] = cboAccname["accname"];
                }
                dgDetail[intRow, 5] = GetNumeric(txtOridebit.Text);
                dgDetail[intRow, 6] = GetNumeric(txtOricredit.Text);
                dgDetail[intRow, 7] = txtDesc.Text;
                if (cboApname.Text != "")
                {
                    dgDetail[intRow, 8] = cboApname["apnum"];
                    dgDetail[intRow, 9] = cboApname["apname"];
                }
                if (cboArname.Text != "")
                {
                    dgDetail[intRow, 10] = cboArname["arnum"];
                    dgDetail[intRow, 11] = cboArname["arname"];
                }
                dgDetail[intRow, 12] = txtVouchernum.Text;
                dgDetail[intRow, 13] = dtInvdate.Value;
                dgDetail[intRow, 14] = txtChequeno.Text;
                dgDetail[intRow, 15] = bfavh["oricur"];
                dgDetail[intRow, 16] = bfavh["exrate"];
                dgDetail[intRow, 17] = bfavh["locno"];
                dgDetail[intRow, 18] = bfavh["deptno"];

                currentRow["vouchernum"] = txtVouchernum.Text;


                    //txtVouchernum.Text = dgDetail[intRow, 1].ToString();
                    //dtTrandate.Text = dgDetail[intRow, 2].ToString();
                    //txtAccname.Text = dgDetail[intRow, 3].ToString();
                    //cboAccname.Text = dgDetail[intRow, 4].ToString();
                    //txtOridebit.Text = dgDetail[intRow, 5].ToString();
                    //txtOricredit.Text = dgDetail[intRow, 6].ToString();
                    //txtDesc.Text = dgDetail[intRow, 7].ToString();
                    //txtApname.Text = dgDetail[intRow, 8].ToString();
                    //cboApname.Text = dgDetail[intRow, 9].ToString();
                    //txtArname.Text = dgDetail[intRow, 10].ToString();
                    //cboArname.Text = dgDetail[intRow, 11].ToString();
                    //dtInvdate.Text = dgDetail[intRow, 13].ToString();
                    //txtChequeno.Text = dgDetail[intRow, 14].ToString();
                    //bfavh["oricur"] = dgDetail[intRow, 15].ToString();
                    //bfavh["exrate"] = GetNumeric(dgDetail[intRow, 16].ToString());
                    //bfavh["locno"] = dgDetail[intRow, 17].ToString();
                    //bfavh["deptno"] = dgDetail[intRow, 18].ToString();
			}

			skipValidate = false;

			calcTotalDebitCredit();
			ClearTransaction();
			txtVouchernum.Focus();
			grpHeaderVoucherInfo.Enabled = true;
			grpHeaderTransInfo.Enabled = false;

			txtVouchernum.Enabled = true;
			cboApname.Enabled = true;
			cboArname.Enabled = true;
			txtChequeno.Enabled = true;
			dtTrandate.Enabled = true;
			//txtDesc.Enabled = true;
			dtInvdate.Enabled = true;


			btnUpdate.Enabled = false;
			btnAdd.Enabled = true;
			blnNew = true;
			dgDetail.Enabled = true;

			refreshBfav1();

			txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";
			
		}
		private void cmdInsert_Click(object sender, EventArgs e)
		{
			DataTable bfav1 =  dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			dgDetail.Enabled = true;
	
			if (blnNew)
			{
				DataRow addRow = bfav1.Rows.Add(new object[] { });
				addRow["line"] = intGetLine();
				addRow["vouchernum"] = txtVouchernum.Text;
				addRow["detail"] = txtDesc.Text;
				addRow["apnum"] = cboApname["apnum"];
				addRow["apname"] = cboApname["apname"];
				addRow["arnum"] = cboArname["arnum"];
				addRow["arname"] = cboArname["arname"];
				addRow["trandate"] = dtTrandate.Value;
				addRow["accnum"] = cboAccname["accnum"];
				addRow["accname"] = cboAccname["accname"];
				addRow["locno"] = bfavh["locno"];
				addRow["deptno"] = bfavh["deptno"];
				addRow["chknum"] = txtChequeno.Text;
				addRow["oridebit"] = GetNumeric(txtOridebit.Text);
				addRow["oricredit"] = GetNumeric(txtOricredit.Text);
				addRow["exrate"] = bfavh["exrate"];
				addRow["oricur"] = bfavh["oricur"] ;
				addRow["invnum"] = txtVouchernum.Text;
				addRow["invdate"] = dtInvdate.Value;

			}
			else
			{
				int intRow = dgDetail.CurrentRowIndex;

				dgDetail[intRow, 2] = txtVouchernum.Text;
				dgDetail[intRow, 3] = dtTrandate.Value;
				dgDetail[intRow, 4] = cboAccname["accnum"];
				dgDetail[intRow, 5] = cboAccname["accname"];
				dgDetail[intRow, 6] = GetNumeric(txtOridebit.Text);
				dgDetail[intRow, 7] = GetNumeric(txtOricredit.Text);
				dgDetail[intRow, 8] = txtDesc.Text;
				dgDetail[intRow, 9] = cboApname["apnum"];
				dgDetail[intRow, 10] = cboApname["apname"];
				dgDetail[intRow, 11] = cboArname["arnum"];
				dgDetail[intRow, 12] = cboArname["arname"];
				dgDetail[intRow, 13] = txtVouchernum.Text;
				dgDetail[intRow, 14] = dtInvdate.Value;
				dgDetail[intRow, 15] = txtChequeno.Text;
				dgDetail[intRow, 16] = bfavh["oricur"];
				dgDetail[intRow, 17] = bfavh["exrate"];
				dgDetail[intRow, 18] = bfavh["locno"];
				dgDetail[intRow, 19] = bfavh["deptno"];
			}
			calcTotalDebitCredit();
			ClearEntry();
			cboAccname.Focus();
			blnNew = true;
			dgDetail.Enabled = true;

			refreshBfav1();
		}
		#endregion

		private int intGetLine()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			int intValue = 0;

			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					intValue = intValue + 1;
			}

			return intValue;
		}

	
		private decimal GetNumeric(object Numeric)
		{
			try
			{
				return decimal.Parse(Numeric.ToString());
			}
			catch
			{
				return 0;
			}
		}


		private void ClearTransaction()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			txtVouchernum.Text = "";
			txtChequeno.Text = "";
			txtDesc.Text = "";
			cboApname.Text = "";
			cboArname.Text = "";
			cboAccname.Text = "";
			txtAccname.Text = "";
			txtArname.Text = "";
			txtApname.Text = "";
			txtOridebit.Text = "0.00";
			txtOricredit.Text = "0.00";

			blnNew = true;
		}

		private void ClearEntry()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			cboAccname.Text= "";
			txtDesc.Text = "";
			txtAccname.Text = "";
			txtOridebit.Text = "0.00";
			txtOricredit.Text = "0.00";
			bfavh["oricur"] = "SGD";
			bfavh["exrate"] = 0;
			blnNew = true;
		}
	
		private void Addrow_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				int intRow;
				DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
				intRow = dgDetail.CurrentRowIndex + 1;
				if (dgDetail.CurrentRowIndex == getTotalRows() - 1)
				{
					DataRow addRow = bfav1.Rows.Add(new object[] { });
					addRow["deliverydate"] = dbaccess.DataSet.Tables["bfavh"].Rows[0]["deliverydate"];
				}
				dgDetail.CurrentCell = new DataGridCell(intRow, 1);
			}
		}

		private int getTotalRows()
		{
			int intRow = 0;
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					intRow = intRow + 1;
				}
			}
			return intRow;
		}

		private void DeleteCheckItemsOnBFAV1()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			BizFunctions.DeleteRow(bfav1, " mark=true");
		}

		private void MakeLostFocusEvent()
		{
			txtOridebit.LostFocus+=new EventHandler(txtOridebit_LostFocus);
			txtOricredit.LostFocus += new EventHandler(txtOricredit_LostFocus);
		}

		void txtOridebit_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{

			}
		}

		protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Delete_OnClick(sender, e);

			calcTotalDebitCredit();
		}

		private void MakeF3DblClickEventsOnGrid()
		{
			foreach (DataGridTableStyle dataGridTableStyle in dgDetail.TableStyles)
			{
				foreach (DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
				{
					if (dataGridColumnStyle is BizDataGridTextBoxColumn)
					{
						BizDataGridTextBoxColumn bizDataGridTextBoxColumn = null;

						bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;
						bizDataGridTextBoxColumn.TextBoxGrid.MouseDoubleClick += new MouseEventHandler(dgDetail_MouseDoubleClick);

					}
				}
			}
		}

		private void dgDetail_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			int intRow = dgDetail.CurrentRowIndex;

			txtVouchernum.Focus();

            //Jason 18062015 : Error after adding new columns
            //txtVouchernum.Text			= dgDetail[intRow, 2].ToString();
            //dtTrandate.Text				= dgDetail[intRow, 3].ToString();
            //txtAccname.Text				= dgDetail[intRow, 4].ToString();
            //cboAccname.Text				= dgDetail[intRow, 5].ToString();
            //txtOridebit.Text				= dgDetail[intRow, 6].ToString();
            //txtOricredit.Text				= dgDetail[intRow, 7].ToString();
            //txtDesc.Text					= dgDetail[intRow, 8].ToString();
            //txtApname.Text				= dgDetail[intRow, 9].ToString();
            //cboApname.Text				= dgDetail[intRow, 10].ToString();
            //txtArname.Text				= dgDetail[intRow, 11].ToString();
            //cboArname.Text				= dgDetail[intRow, 12].ToString();
            //dtInvdate.Text				= dgDetail[intRow, 14].ToString();
            //txtChequeno.Text				= dgDetail[intRow, 15].ToString();
            //bfavh["oricur"]					= dgDetail[intRow, 16].ToString();
            //bfavh["exrate"]				= GetNumeric(dgDetail[intRow, 17].ToString());
            //bfavh["locno"]					= dgDetail[intRow, 18].ToString();
            //bfavh["deptno"]				= dgDetail[intRow, 19].ToString();


            txtVouchernum.Text = dgDetail[intRow, 1].ToString();
            dtTrandate.Text = dgDetail[intRow, 2].ToString();
            txtAccname.Text = dgDetail[intRow, 3].ToString();
            cboAccname.Text = dgDetail[intRow, 4].ToString();
            txtOridebit.Text = dgDetail[intRow, 5].ToString();
            txtOricredit.Text = dgDetail[intRow, 6].ToString();
            txtDesc.Text = dgDetail[intRow, 7].ToString();
            txtApname.Text = dgDetail[intRow, 8].ToString();
            cboApname.Text = dgDetail[intRow, 9].ToString();
            txtArname.Text = dgDetail[intRow, 10].ToString();
            cboArname.Text = dgDetail[intRow, 11].ToString();
            dtInvdate.Text = dgDetail[intRow, 13].ToString();
            txtChequeno.Text = dgDetail[intRow, 14].ToString();
            bfavh["oricur"] = dgDetail[intRow, 15].ToString();
            bfavh["exrate"] = GetNumeric(dgDetail[intRow, 16].ToString());
            bfavh["locno"] = dgDetail[intRow, 17].ToString();
            bfavh["deptno"] = dgDetail[intRow, 18].ToString();


			dgDetail.Enabled = false;
			grpHeaderTransInfo.Enabled = true;
			grpHeaderVoucherInfo.Enabled = true;

			txtVouchernum.Enabled = false;
			cboApname.Enabled = false;
			cboArname.Enabled = false;
			txtChequeno.Enabled = false;
			dtTrandate.Enabled = false;
			txtDesc.Enabled = true;
			dtInvdate.Enabled = false;

			btnAdd.Enabled = false;
			btnNextTrans.Enabled = false;
			btnUpdate.Enabled = true;
			btnInsertTrans.Enabled = false;
			blnNew = false;
			txtCurrentAction.Text = "Updating Transactions Info, hit the Update button or press F5 once updating is done!";
		}
		private void AccnumChanged_Combo(object sender, EventArgs e)
		{
			txtAccname.Text = "";
			if (cboAccname.Text != "")
			{
				txtAccname.Text = cboAccname["accnum"].ToString().Trim();
			}
		}

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
		
		private void refreshBfav1()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
					
			#region Refresh for detail transaction BFAV1
			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
                    if (BizFunctions.IsEmpty(dr["trandate"]))
                    {
                        if (!BizFunctions.IsEmpty(bfavh["trandate"])||!BizFunctions.IsEmpty(bfavh["jvtemplate"]))  //wenyue add jvtemplate Jul 2016
                        {
                            dr["trandate"] = bfavh["trandate"];
                        }
                    }

					#region Steph - To get AP/ AR name in case user brought over the openings from last year
					dbaccess.ReadSQL("getApname", "SELECT apname from apm where apnum = '" + dr["apnum"].ToString().Trim() + "'");
					if (dbaccess.DataSet.Tables["getApname"].Rows.Count > 0)
					{
						if (dr["apname"].ToString().Trim() == "" || dr["apname"] == System.DBNull.Value)
						{
							dr["apname"] = dbaccess.DataSet.Tables["getApname"].Rows[0]["apname"].ToString().Trim();
						}
					}

					dbaccess.ReadSQL("getArname", "SELECT arname from arm where arnum = '" + dr["arnum"].ToString().Trim() + "'");
					if (dbaccess.DataSet.Tables["getArname"].Rows.Count > 0)
					{
						if (dr["arname"].ToString().Trim() == "" || dr["arname"] == System.DBNull.Value)
						{
							dr["arname"] = dbaccess.DataSet.Tables["getArname"].Rows[0]["arname"].ToString().Trim();
						}
					}
					dbaccess.ReadSQL("getAccname", "SELECT accname from acm where accnum = '" + dr["accnum"].ToString().Trim() + "'");
					if (dbaccess.DataSet.Tables["getAccname"].Rows.Count > 0)
					{
						if (dr["accname"].ToString().Trim() == "" || dr["accname"] == System.DBNull.Value)
						{
							dr["accname"] = dbaccess.DataSet.Tables["getAccname"].Rows[0]["accname"].ToString().Trim();
						}
					}
					#endregion

					#region Steph -  To get pd from pd (nonYear) table.
					dr["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(dr["trandate"]));
					#endregion

                   
					
					//steph_29 Mar 2010_1433 - to take from exr table if the exrate is zero.
					if (dr["exrate"] == System.DBNull.Value || Convert.ToDecimal(dr["exrate"]) == 0)
					{
						dr["exrate"] = BizAccounts.GetExRate(dbaccess, dr["oricur"].ToString(), BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(dr["trandate"])));
					}
                    if (dr["oridebit"]==System.DBNull.Value)
                    {
                        dr["oridebit"] = 0.00;
                    }
                    if (dr["oricredit"] == System.DBNull.Value)
                    {
                        dr["oricredit"] = 0.00;
                    }

					dr["oriamt"] = Convert.ToDecimal(dr["oridebit"]) - Convert.ToDecimal(dr["oricredit"]);
					dr["postamt"] = BizFunctions.Round(Convert.ToDecimal(dr["oriamt"]) * Convert.ToDecimal(dr["exrate"]));
				}
			}
			calcTotalDebitCredit();
			#endregion
		}

		public void getAROpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
		

			dbaccess.ReadSQL("getAR", "SELECT arnum,invnum,invdate,locno,deptno,oricur,exrate,accnum,sum(oriamt) as oriamt,"+
				" sum(postamt) as postamt "+
				" from ard" + lastYear +
				" WHERE arnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and arnum<= '" + FilterOP.CodeToValue.ToString().Trim() +"' GROUP BY arnum,invnum,invdate,oricur,exrate,accnum,locno,deptno");
			DataTable getAR = dbaccess.DataSet.Tables["getAR"];
			foreach (DataRow dr in getAR.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addArd = bfav1.Rows.Add(new object[] { });
					addArd["vouchernum"] = dr["invnum"];
					addArd["trandate"] = bfavh["trandate"];
					addArd["coy"] = "SAF";
					addArd["arnum"] = dr["arnum"];
					addArd["invnum"] = dr["invnum"];
					addArd["invdate"] = dr["invdate"];
					addArd["locno"] = dr["locno"];
					addArd["deptno"] = dr["deptno"];
					addArd["oricur"] = dr["oricur"];
					addArd["accnum"] = dr["accnum"];
					addArd["detail"] = bfavh["detail"];
					if ((decimal)dr["oriamt"] > 0)
					{
						addArd["oridebit"] = dr["oriamt"];
					}
					else
					{
						addArd["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
					}
					addArd["exrate"] = dr["exrate"];

				}
			}

			refreshBfav1();

		}

		public void getAPOpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];


			dbaccess.ReadSQL("getAP", "SELECT apnum,invnum,invdate,locno,deptno,oricur,exrate,accnum,sum(oriamt) as oriamt,"+
				" sum(postamt) as postamt from apd" + lastYear + 
				" WHERE apnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and apnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' "+
				" GROUP BY apnum,invnum,invdate,oricur,exrate,accnum,locno,deptno");
			DataTable getAP = dbaccess.DataSet.Tables["getAP"];
			foreach (DataRow dr in getAP.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addApd = bfav1.Rows.Add(new object[] { });
					addApd["vouchernum"] = dr["invnum"];
					addApd["trandate"] = bfavh["trandate"];
					addApd["coy"] = "SAF";
					addApd["apnum"] = dr["apnum"];
					addApd["invnum"] = dr["invnum"];
					addApd["invdate"] = dr["invdate"];
					addApd["locno"] = dr["locno"];
					addApd["deptno"] = dr["deptno"];
					addApd["oricur"] = dr["oricur"];
					addApd["exrate"] = dr["exrate"];
					addApd["accnum"] = dr["accnum"];
					addApd["detail"] = bfavh["detail"];
					if ((decimal)dr["oriamt"] > 0)
					{
						addApd["oridebit"] = dr["oriamt"];
					}
					else
					{
						addApd["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
					}

				}
			}

			refreshBfav1();

		}

		public void getCSHOpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];


			dbaccess.ReadSQL("getCSH", "SELECT chknum,trandate,oricur,exrate,accnum,locno,deptno,sum(oriamt) as oriamt,"+
				" sum(postamt) as postamt from csh" + lastYear + 
				" WHERE accnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and accnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' "+
				" AND (RECONDATE='' OR RECONDATE IS NULL) GROUP BY refnum,chknum," +
				"trandate,accnum,locno,deptno,oricur,exrate "+
				" UNION ALL "+
				" SELECT max(chknum) as chknum,max(trandate) as trandate,oricur,exrate,accnum,locno,deptno,"+
				" sum(oriamt) as oriamt,sum(postamt) as postamt from csh" + lastYear + " WHERE "+
				" accnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and accnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' "+
				" AND (RECONDATE <>'' and RECONDATE IS NOT NULL) GROUP BY accnum,locno,deptno,oricur,exrate");
			DataTable getCSH = dbaccess.DataSet.Tables["getCSH"];
			foreach (DataRow dr in getCSH.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addCsh = bfav1.Rows.Add(new object[] { });
					addCsh["vouchernum"] = dr["chknum"];
					addCsh["trandate"] = bfavh["trandate"];
					addCsh["coy"] = "SAF";
					addCsh["chknum"] = dr["chknum"];
					addCsh["oricur"] = dr["oricur"];
					addCsh["exrate"] = dr["exrate"];
					addCsh["accnum"] = dr["accnum"];
					addCsh["locno"] = dr["locno"];
					addCsh["deptno"] = dr["deptno"];
					addCsh["detail"] = bfavh["detail"];
					if ((decimal)dr["oriamt"] > 0)
					{
						addCsh["oridebit"] = dr["oriamt"];
					}
					else
					{
						addCsh["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
					}

				}
			}

			refreshBfav1();

		}

		public void getGLOpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

            //ori
            //string strlGLD = "SELECT trandate,oricur,gld.exrate,accnum,locno,deptno,acm.acctype as acctype,sum(oriamt) as oriamt,"+
            //    " sum(postamt) as postamt from gld" + lastYear + " gld "+
            //    " LEFT OUTER JOIN acm ON accnum =  acm.accnum "+
            //    " WHERE acm.acctype in ('5','6','7','8','9','10','11','12','13','14') "+
            //    " GROUP BY gld.accnum,gld.locno,gld.deptno,gld.oricur,gld.exrate";
            string strlGLD = "SELECT gld.oricur,gld.exrate,gld.accnum,locno,deptno,sum(oriamt) as oriamt," +
                    " sum(postamt) as postamt from gld" + lastYear + " gld " +
                    " LEFT OUTER JOIN acm ON gld.accnum =  acm.accnum " +
                    " WHERE acm.acctype in ('5','6','7','8','9','10','11','12','13','14') " +
                    " GROUP BY gld.accnum,gld.locno,gld.deptno,gld.oricur,gld.exrate";
            dbaccess.ReadSQL("getGL", strlGLD);
			DataTable getGL = dbaccess.DataSet.Tables["getGL"];
			foreach (DataRow dr in getGL.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addGl = bfav1.Rows.Add(new object[] { });
					addGl["vouchernum"] = bfavh["refnum"];
					addGl["trandate"] = bfavh["trandate"];
					addGl["coy"] = "SAF";
					addGl["oricur"] = dr["oricur"];
					addGl["exrate"] = dr["exrate"];
					addGl["accnum"] = dr["accnum"];
					addGl["locno"] = dr["locno"];
					addGl["deptno"] = dr["deptno"];
					addGl["detail"] = bfavh["detail"];

					if ((decimal)dr["oriamt"] > 0)
					{
						addGl["oridebit"] = dr["oriamt"];
					}
					else
					{
						addGl["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
					}

				}
			}

			refreshBfav1();

		}

		private void InitialComboAccnum()
		{
			this.cboAccname = new ATL.MultiColumnComboBox.ColumnComboBox();

			this.cboAccname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboAccname.DropDownWidth = 17;
			this.cboAccname.Location = new System.Drawing.Point(60, 15);
			this.cboAccname.Name = "cboAccname";
			this.cboAccname.Size = new System.Drawing.Size(240, 21);
			this.cboAccname.TabIndex = 1;
			cboAccname.Visible = true;
			this.cboAccname.KeyDown += new KeyEventHandler(cboAccname_KeyDown);

			this.grpHeaderTransInfo.Controls.Add(cboAccname);

			string strSQL = "SELECT ACCNAME,ACCNUM FROM ACM WHERE ACTIVE='1' ORDER BY ACCNAME";
			DataSet dsACM = this.dbaccess.ReadSQLTemp("ACM", strSQL);
			DataTable dtACM = dsACM.Tables["ACM"];

			cboAccname.Data = dtACM;
			//Set which row will be displayed in the text box
			//If you set this to a column that isn't displayed then the suggesting functionality won't work.
			cboAccname.ViewColumn = 0;
			//Set a few columns to not be shown
			cboAccname.ColWidthNew(new int[] { 300, 100 });
			cboAccname.UpdateIndex();			
		}

		void cboAccname_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				SendKeys.Send("{Tab}");
			}
		}

		private string getAccName(string strAccnum)
		{
			string strSQL = "SELECT TOP 1 * FROM ACM WHERE ACCNUM='" + cboAccname["accnum"] + "'";
			DataSet dsACM = this.dbaccess.ReadSQLTemp("ACM", strSQL);
			DataTable dtACM = dsACM.Tables["ACM"];

			if (dtACM.Rows.Count > 0)
				return dtACM.Rows[0]["ACCNAME"].ToString();
			else
				return "";
		}

		private void InitialComboArnum()
		{
			this.cboArname = new ATL.MultiColumnComboBox.ColumnComboBox();

			this.cboArname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboArname.DropDownWidth = 17;
			this.cboArname.Location = new System.Drawing.Point(100, 40);
			this.cboArname.Name = "cboArname";
			this.cboArname.Size = new System.Drawing.Size(200, 21);
			this.cboArname.TabIndex = 1;
			cboArname.Visible = true;
			this.cboArname.KeyDown += new KeyEventHandler(cboArname_KeyDown);

			this.grpHeaderVoucherInfo.Controls.Add(cboArname);

			//this.formName.Controls.Add(cboApname);

			string strSQL = "SELECT ARNAME,ARNUM FROM ARM WHERE ACTIVE='1' ORDER BY ARNAME";
			DataSet dsARM = this.dbaccess.ReadSQLTemp("ARM", strSQL);
			DataTable dtARM = dsARM.Tables["ARM"];

			cboArname.Data = dtARM;
			//Set which row will be displayed in the text box
			//If you set this to a column that isn't displayed then the suggesting functionality won't work.
			cboArname.ViewColumn = 0;
			//Set a few columns to not be shown
			cboArname.ColWidthNew(new int[] { 300, 100 });
			cboArname.UpdateIndex();
		}

		void cboArname_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				SendKeys.Send("{Tab}");
			}
			if (e.KeyCode == Keys.F11)
			{
				F11Event();
			}

			if (e.KeyCode == Keys.F12)
			{
				F12Event();
			}
			
		}

		private string getArName(string strArnum)
		{
			string strSQL = "SELECT TOP 1 * FROM ARM WHERE ARNUM='" + cboArname["arnum"] + "'";
			DataSet dsARM = this.dbaccess.ReadSQLTemp("ARM", strSQL);
			DataTable dtARM = dsARM.Tables["ARM"];

			if (dtARM.Rows.Count > 0)
				return dtARM.Rows[0]["ARNAME"].ToString();
			else
				return "";
		}


		private void InitialComboApnum()
		{
			this.cboApname = new ATL.MultiColumnComboBox.ColumnComboBox();

			this.cboApname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboApname.DropDownWidth = 17;
			this.cboApname.Location = new System.Drawing.Point(450, 40);
			this.cboApname.Name = "cboApname";
			this.cboApname.Size = new System.Drawing.Size(180, 21);
			this.cboApname.TabIndex = 1;
			cboApname.Visible = true;
			this.cboApname.KeyDown += new KeyEventHandler(cboApname_KeyDown);

			this.grpHeaderVoucherInfo.Controls.Add(cboApname);

			//this.formName.Controls.Add(cboApname);

			string strSQL = "SELECT APNAME,APNUM FROM APM WHERE ACTIVE='1' ORDER BY APNAME";
			DataSet dsAPM = this.dbaccess.ReadSQLTemp("APM", strSQL);
			DataTable dtAPM = dsAPM.Tables["APM"];

			cboApname.Data = dtAPM;
			//Set which row will be displayed in the text box
			//If you set this to a column that isn't displayed then the suggesting functionality won't work.
			cboApname.ViewColumn = 0;
			//Set a few columns to not be shown
			cboApname.ColWidthNew(new int[] { 300, 100 });
			cboApname.UpdateIndex();
		}

		void cboApname_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				SendKeys.Send("{Tab}");
			}

			if (e.KeyCode == Keys.F11)
			{
				F11Event();
			}

			if (e.KeyCode == Keys.F12)
			{
				F12Event();
			}
		}

		private string getApName(string strApnum)
		{
			string strSQL = "SELECT TOP 1 * FROM APM WHERE APNUM='" + cboApname["apnum"] + "'";
			DataSet dsAPM = this.dbaccess.ReadSQLTemp("APM", strSQL);
			DataTable dtAPM = dsAPM.Tables["APM"];

			if (dtAPM.Rows.Count > 0)
				return dtAPM.Rows[0]["APNAME"].ToString();
			else
				return "";
		}
	}
}
