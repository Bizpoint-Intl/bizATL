/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		bank.cs
 *	Description:    Bank Reconciliation
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
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizTools;
using BizRAD.BizAccounts;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace ATL.Bank
{
	public class BANK
	{
		protected DBAccess	dbAccess			= null;
		protected DataSet	ds					= null;
		protected Hashtable	selectsCollection	= null;
		protected string Filter1				= "";
		protected string Filter2				= "";
		protected string Filter3				= "";
        protected string Filter4                = "";
		protected string DataViewFilter			= "";
		protected DataGrid dataGrid				= null;

		protected string projectPath			= null;
		protected int setFlag					= 0;
		protected DataRow [] dr_csh				= null;

		protected string COY;

		public BANK()
		{
			COY = "";
			
			this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
			Form form = BizXmlReader.CurrentInstance.Load(this.projectPath+@"\BANK\Code\bank.xml", "bank", this, null) as Form;
			
			this.dbAccess			= new DBAccess();
			this.selectsCollection	= new Hashtable();

			this.dataGrid = (DataGrid)BizXmlReader.CurrentInstance.GetControl("bank", "dataGrid");
			string checking = "SELECT csh.[ID] as ID,csh.period as period,csh.trandate as trandate,csh.refnum as refnum,csh.coy as coy,csh.chknum as chknum,CASE WHEN APM.APNAME IS NULL THEN ARM.ARNAME ELSE APM.APNAME END AS DETAIL,csh.postamt AS withdrawal, csh.postamt AS deposit, csh.recondate as recondate,csh.reconuser as reconuser FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " csh LEFT OUTER JOIN arm ON csh.arnum  = arm.arnum LEFT OUTER JOIN apm ON csh.apnum = apm.apnum WHERE 1=1";
			this.selectsCollection.Add("SysLocker", "SELECT * FROM SysLocker WHERE ModuleName='BANK' AND [Key]='BANK'");
			//this.selectsCollection.Add("csh", "SELECT [ID],period,trandate,refnum,coy,chknum,detail,arnum,apnum,postamt AS withdrawal, postamt AS deposit, recondate,reconuser FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " WHERE coy = '" + COY + "' AND 1=1");
			this.selectsCollection.Add("csh", "SELECT csh.[ID] as ID,csh.period as period,csh.trandate as trandate,csh.refnum as refnum,csh.coy as coy,csh.chknum as chknum,CASE WHEN APM.APNAME IS NULL THEN ARM.ARNAME ELSE APM.APNAME END AS DETAIL,csh.postamt AS withdrawal, csh.postamt AS deposit, csh.recondate as recondate,csh.reconuser as reconuser FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " csh LEFT OUTER JOIN arm ON csh.arnum  = arm.arnum LEFT OUTER JOIN apm ON csh.apnum = apm.apnum WHERE 1=1");
			this.selectsCollection.Add("csh_pd", "SELECT DISTINCT period FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " WHERE 1=1 Order by period Desc");
			this.selectsCollection.Add("acm", "SELECT DISTINCT a.accnum, b.accname FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " a JOIN acm " + " b ON a.accnum=b.accnum and b.acctype=6");
			this.selectsCollection.Add("coy", "SELECT DISTINCT a.coy, b.coy FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " a LEFT JOIN coy" + " b ON a.coy=b.coy");
			this.selectsCollection.Add("search", "SELECT refnum,chknum AS chequenum,detail AS detail,trandate,postamt AS amount FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " WHERE 1=2");
			//this.selectsCollection.Add("search", "SELECT csh.refnum AS refnum,csh.chknum AS chequenum,CASE WHEN APM.APNAME IS NULL THEN ARM.ARNAME ELSE APM.APNAME END AS DETAIL,csh.trandate as trandate,csh.postamt AS amount FROM csh LEFT OUTER JOIN APM on csh.apnum = apm.apnum LEFT OUTER JOIN ARM on csh.arnum = ARM.arnum WHERE 1=2");
			this.dbAccess.ReadSQL(selectsCollection);

			if(this.dbAccess.DataSet.Tables["SysLocker"].Rows.Count != 0)
			{
				MessageBox.Show("Bank Reconciliation Module is locked/in use !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				if (this.dbAccess.DataSet.Tables["csh"].Rows.Count != 0)
				{
					this.dbAccess.RemoteStandardSQL.ExecuteNonQuery("INSERT INTO SysLocker (ModuleName, [Key], UserName) VALUES ('BANK-" + COY + "','BANK-" + COY + "','" + Common.DEFAULT_SYSTEM_USERNAME + "')");
					this.ds = this.dbAccess.DataSet;

					this.ds.Tables["csh"].Columns.Add("mark", Type.GetType("System.Boolean"));

                   

					BizGridInfo detailGridInfo = (BizGridInfo)BizXmlReader.CurrentInstance.Load(this.projectPath + @"\BANK\Code\DetailGridInfo_Bank.xml", "detailGridInfo", null, null);

					BizBinding.DataGridBinding(this.dataGrid, this.ds, detailGridInfo, this.dbAccess.ColumnsInfo, "", "", DataViewRowState.CurrentRows);

					CurrencyManager currencyManager = this.dataGrid.BindingContext[this.dataGrid.DataSource, this.dataGrid.DataMember] as CurrencyManager;
					(currencyManager.List as DataView).AllowNew = false;

					#region populate the comboBox Items - ACC

					ComboBox cb_accnum = (ComboBox)BizXmlReader.CurrentInstance.GetControl("bank", "accnum");
					cb_accnum.DataSource = this.dbAccess.DataSet.Tables["acm"];
					cb_accnum.DisplayMember = this.dbAccess.DataSet.Tables["acm"].Columns["accname"].ColumnName.ToString();
					cb_accnum.ValueMember = this.dbAccess.DataSet.Tables["acm"].Columns["accnum"].ColumnName.ToString();

					#endregion

					//DON20071027_REMOVE PRRIOD
					//#region populate the comboBox Items - PERIOD

					//ComboBox cb_period = (ComboBox)BizXmlReader.CurrentInstance.GetControl("bank", "period");
					//cb_period.DataSource = this.dbAccess.DataSet.Tables["csh_pd"];
					//cb_period.DisplayMember = this.dbAccess.DataSet.Tables["csh_pd"].Columns["period"].ColumnName.ToString();
					//cb_period.ValueMember = this.dbAccess.DataSet.Tables["csh_pd"].Columns["period"].ColumnName.ToString();

					//#endregion	

					#region populate the comboBox Items - BANK ACCOUNT

					ComboBox cb_coy = (ComboBox)BizXmlReader.CurrentInstance.GetControl("bank", "coy");
					cb_coy.DataSource = this.dbAccess.DataSet.Tables["coy"];
					cb_coy.DisplayMember = this.dbAccess.DataSet.Tables["coy"].Columns["coy"].ColumnName.ToString();
					cb_coy.ValueMember = this.dbAccess.DataSet.Tables["coy"].Columns["coy"].ColumnName.ToString();

					#endregion					

					#region set selDateFrom, showvouchers, viewtype, viewcoy, sortedby

					DateTimePicker dtp_DateFrom = BizXmlReader.CurrentInstance.GetControl("bank", "selDateFrom") as DateTimePicker;
					string dt_frm = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString("00") + "-" + "01";
					dtp_DateFrom.Value = DateTime.Parse(dt_frm);

					ComboBox cb_showvouchers = BizXmlReader.CurrentInstance.GetControl("bank", "showvouchers") as ComboBox;
					ComboBox cb_viewtype = BizXmlReader.CurrentInstance.GetControl("bank", "viewtype") as ComboBox;
					ComboBox cb_sortedby = BizXmlReader.CurrentInstance.GetControl("bank", "sortedby") as ComboBox;

					cb_showvouchers.SelectedIndex = 2;
					cb_viewtype.SelectedIndex = 0;
					cb_sortedby.SelectedIndex = 0;
					
					this.Filter1 = "recondate is null";
					this.Filter2 = "";
					this.Filter3 = "chknum,trandate";

					#endregion

					this.dbAccess.DataSet.Tables["search"].Rows.Add(new object[] { null, null, null, null, 0.00 });

					#region set alignment of "withdrawal" and "depost" to right
					foreach (DataGridTableStyle dataGridTableStyle in this.dataGrid.TableStyles)
					{
						foreach (DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
						{
							if (dataGridColumnStyle is BizDataGridTextBoxColumn)
							{
								BizDataGridTextBoxColumn bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;

								if (bizDataGridTextBoxColumn.TextBoxGrid.MappingName == "withdrawal" || bizDataGridTextBoxColumn.TextBoxGrid.MappingName == "deposit")
								{
									bizDataGridTextBoxColumn.Alignment = HorizontalAlignment.Right;
								}
							}
						}
					}
					#endregion

                    

					form.Closed += new EventHandler(form_Closed);
					form.FormBorderStyle = FormBorderStyle.FixedSingle;

                 
                    this.ds.Tables["csh"].ColumnChanged += new DataColumnChangeEventHandler(BANK_ColumnChanged);
					showGRID();
					
					form.ShowDialog();					
				}
				else
					MessageBox.Show("There is no Transaction found in the Bank Recon", "Bank Recon", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

        void BANK_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable csh = this.ds.Tables["csh"];
            switch (e.Column.ColumnName)
            {

                case "mark":
                    {
                        TextBox bankst = BizXmlReader.CurrentInstance.GetControl("bank", "bankst") as TextBox;
                        TextBox diff = BizXmlReader.CurrentInstance.GetControl("bank", "diff") as TextBox;

                        bankst.Text = GetCshSum("withdrawal").ToString();
                        diff.Text = GetCshSum("deposit").ToString();

                    }
                    break;
            }
        }

        private Decimal GetCshSum(string columnName)
        {
           

            decimal total = 0;
            string sql1 = "Select sum("+columnName+") as "+columnName+" from CSH where mark=0";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, sql1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (!BizFunctions.IsEmpty(dt1.Rows[0][columnName]))
                    {
                        total = Convert.ToDecimal(dt1.Rows[0][columnName]);
                    }
                }
            }

            return total;
        }

		#region ComboBox Events for Setting Filter1, Filter2, Filter3

		#region showvouchers_SelectionChange
		protected void showvouchers_SelectionChange(object sender, System.EventArgs e)
		{
			switch((sender as ComboBox).Text.ToUpper())
			{
				case "BOTH":
					this.Filter1 = "";
					break;
				case "RECONCILED":
					this.Filter1 = "recondate is not null";
					break;
				case "UNRECONCILED":
					this.Filter1 = "recondate is null";
					break;
			}
			this.setCshDataView();
		}
		#endregion

		#region viewtype_SelectionChange
		protected void viewtype_SelectionChange(object sender, System.EventArgs e)
		{
			switch((sender as ComboBox).Text.ToUpper())
			{
				case "BOTH":
					this.Filter2 = "";
					break;
				case "WITHDRAWALS":
					this.Filter2 = "withdrawal<>0";
					break;
				case "DEPOSITS":
					this.Filter2 = "deposit<>0";
					break;
			}
			this.setCshDataView();
		}
		#endregion

		#region sortedby_SelectionChange
		protected void sortedby_SelectionChange(object sender, System.EventArgs e)
		{
			switch((sender as ComboBox).Text.ToUpper())
			{
				case "DATE":
					this.Filter3 = "trandate";
					break;
				case "REFERENCE NO":
					this.Filter3 = "refnum";
					break;
				case "CHEQUE NO":
					this.Filter3 = "chknum";
					break;
				case "DETAIL":
					this.Filter3 = "detail";
					break;
				case "WITHDRAWAL":
					this.Filter3 = "withdrawal";
					break;
				case "DEPOSIT":
					this.Filter3 = "deposit";
					break;
				case "RECONCILED":
					this.Filter3 = "recondate";
					break;
			}
			this.setCshDataView();
		}
		#endregion

		#endregion

		#region setCshDataView (Set the dataview)

		protected void setCshDataView()
		{
			if(BizFunctions.IsEmpty(this.Filter1)) 
				this.DataViewFilter = this.Filter2;
			else if(BizFunctions.IsEmpty(this.Filter2)) 
				this.DataViewFilter = this.Filter1;
			else if(!BizFunctions.IsEmpty(this.Filter1) && !BizFunctions.IsEmpty(this.Filter2))
				this.DataViewFilter = this.Filter2 + " AND " + this.Filter1;
			else
				this.DataViewFilter = "";

			DataView dv_csh = (DataView) this.dataGrid.DataSource;
			dv_csh.RowFilter = this.DataViewFilter;
			dv_csh.Sort = this.Filter3;
			if(dv_csh.Count != 0)
				this.dataGrid.CurrentRowIndex = 0;
		}

		#endregion

		#region Search Clicked (Refresh the data - i.e. pull from server again)

		protected void btn_Search_Click(object sender, System.EventArgs e)
		{
			showGRID();
            DataTable csh = this.ds.Tables["csh"];

            foreach (DataRow dr1 in csh.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr1["mark"]))
                    {
                        dr1["mark"] = 0;
                    }
                }
            }
            GetSum();

		}

		public void showGRID()
		{
			DataTable csh = this.dbAccess.DataSet.Tables["csh"];
			if (csh.GetChanges(DataRowState.Modified) != null)
			{
				if (MessageBox.Show("Update changes to database before loading new search results?\n\n(NO: All changes will be lost)", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					this.UpdateToDB();
				}
			}

			DateTimePicker dtp_DateFrom = BizXmlReader.CurrentInstance.GetControl("bank", "selDateFrom") as DateTimePicker;
			DateTimePicker dtp_DateTo = BizXmlReader.CurrentInstance.GetControl("bank", "selDateTo") as DateTimePicker;
			string safeDateFrom = BizFunctions.GetSafeDateString(Convert.ToDateTime(dtp_DateFrom.Value.Day.ToString().Trim() + "/" + dtp_DateFrom.Value.Month.ToString().Trim() + "/" + dtp_DateFrom.Value.Year.ToString().Trim()));
			string safeDateTo = BizFunctions.GetSafeDateString(Convert.ToDateTime(dtp_DateTo.Value.Day.ToString().Trim() + "/" + dtp_DateTo.Value.Month.ToString().Trim() + "/" + dtp_DateTo.Value.Year.ToString().Trim()));
			ComboBox cb_accnum = BizXmlReader.CurrentInstance.GetControl("bank", "accnum") as ComboBox;
			ComboBox cb_coy = BizXmlReader.CurrentInstance.GetControl("bank", "coy") as ComboBox;
			ComboBox cb_period = BizXmlReader.CurrentInstance.GetControl("bank", "period") as ComboBox;

			BizFunctions.DeleteAllRows(this.dbAccess.DataSet.Tables["csh"]);
			this.dbAccess.DataSet.Tables["csh"].AcceptChanges();

			this.selectsCollection.Clear();
			//string command = "SELECT [ID],period,trandate,coy,refnum,chknum,detail," +
			//    "postamt*(CASE WHEN (postamt<0) THEN -1 ELSE 0 END) AS withdrawal," +
			//    "postamt*(CASE WHEN (postamt>0) THEN 1 ELSE 0 END) AS deposit,recondate,reconuser " +
			//    "FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " WHERE coy='" + cb_coy.SelectedValue +
			//    "' AND accnum='" + cb_accnum.SelectedValue +
			//    //"' AND period=" + cb_period.SelectedValue +
			//    "'  AND trandate>='" + safeDateFrom + "' AND trandate<='" + safeDateTo + "'";
			string command = "SELECT csh.[ID],csh.period,csh.trandate,csh.coy,csh.refnum,csh.chknum,"+
				"CASE WHEN APM.APNAME IS NOT NULL AND ARM.ARNAME IS NULL THEN APM.APNAME "+
				" WHEN APM.APNAME IS NULL AND ARM.ARNAME IS NOT NULL THEN ARM.ARNAME "+
				"ELSE DETAIL END AS DETAIL," +
				"csh.postamt*(CASE WHEN (csh.postamt<0) THEN -1 ELSE 0 END) AS withdrawal," +
				"csh.postamt*(CASE WHEN (csh.postamt>0) THEN 1 ELSE 0 END) AS deposit,csh.recondate,csh.reconuser " +
				"FROM csh" + Common.DEFAULT_SYSTEM_YEAR + " csh LEFT OUTER JOIN apm ON csh.apnum = apm.apnum LEFT OUTER JOIN arm ON csh.arnum = arm.arnum "+ 
				"WHERE csh.accnum='" + cb_accnum.SelectedValue +
				//"' AND period=" + cb_period.SelectedValue +
				"'  AND csh.trandate>='" + safeDateFrom + "' AND csh.trandate<='" + safeDateTo + "' ORDER BY chknum ";
			this.selectsCollection.Add("csh", command);
			this.dbAccess.ReadSQL(selectsCollection);

			this.setCshDataView();
		}

		#endregion

		#region Update Clicked

		protected void btn_Update_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("Confirm Update Changes to Database?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DataTable csh = this.dbAccess.DataSet.Tables["csh"];
				if(csh.GetChanges(DataRowState.Modified) != null)
				{
					this.UpdateToDB();
					csh.AcceptChanges();
                    GetSum();
				}
			}
		}

		protected void UpdateToDB()
		{
			DataTable csh = this.dbAccess.DataSet.Tables["csh"].GetChanges();
			foreach(DataRow dr in csh.Rows)
			{
				string setClause = null;
				if(Convert.IsDBNull(dr["recondate"]))
					setClause = " recondate=null ";
				else
					setClause = " recondate='"+BizFunctions.GetSafeDateString((DateTime)dr["recondate"])+"' ";

				string sqlCommand = "UPDATE csh" + Common.DEFAULT_SYSTEM_YEAR + 
					" SET reconuser='" + Common.DEFAULT_SYSTEM_USERNAME + "',"+setClause+
					" WHERE [ID]='" + dr["id"].ToString().Trim() + "'";
				this.dbAccess.RemoteStandardSQL.ExecuteNonQuery(sqlCommand);
			}
		}

		#endregion

		#region Cancel Clicked

		protected void btn_Cancel_Click(object sender, System.EventArgs e)
		{
			Form form = BizXmlReader.CurrentInstance.GetForm("bank") as Form;
			form.Close();
		}

		#endregion

		#region form_Closed

		protected void form_Closed(object sender, System.EventArgs e)
		{
			this.dbAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM SysLocker WHERE ModuleName='BANK-" + COY + "' AND [Key]='BANK-" + COY + "'");
		}

		#endregion

		#region Mark Clicked

		protected void btn_Mark_Click(object sender, System.EventArgs e)
		{
			foreach(DataRow dr in this.dbAccess.DataSet.Tables["csh"].Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					dr["mark"] = true;
				}
			}
		}

		#endregion

		#region Unmark Clicked

		protected void btn_Unmark_Click(object sender, System.EventArgs e)
		{
			foreach(DataRow dr in this.dbAccess.DataSet.Tables["csh"].Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					dr["mark"] = false;
				}
			}
		}

		#endregion

		#region Reconsile Clicked

		protected void btn_Reconsile_Click(object sender, System.EventArgs e)
		{
			foreach(DataRow dr in this.dbAccess.DataSet.Tables["csh"].Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					if(!Convert.IsDBNull(dr["mark"]))
					{
						if((bool)dr["mark"])
							dr["recondate"] = System.DBNull.Value;
					}
				}
			}
		}


        protected void btn_Reconsile_Click2(object sender, System.EventArgs e)
        {
            foreach (DataRow dr in this.dbAccess.DataSet.Tables["csh"].Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (!Convert.IsDBNull(dr["mark"]))
                    {
                        if ((bool)dr["mark"])
                            dr["recondate"] = dr["trandate"];
                    }
                }
            }
        }



		#endregion

		#region Report Clicked

		protected void btn_Report_Click(object sender, System.EventArgs e)
		{
			DateTimePicker dtp_DateFrom = BizXmlReader.CurrentInstance.GetControl("bank", "selDateFrom") as DateTimePicker;
			DateTimePicker dtp_DateTo = BizXmlReader.CurrentInstance.GetControl("bank", "selDateTo") as DateTimePicker;
			DateTime dateFrom = Convert.ToDateTime(dtp_DateFrom.Value.Day.ToString().Trim()+"/"+dtp_DateFrom.Value.Month.ToString().Trim()+"/"+dtp_DateFrom.Value.Year.ToString().Trim());
			DateTime dateTo = Convert.ToDateTime(dtp_DateTo.Value.Day.ToString().Trim()+"/"+dtp_DateTo.Value.Month.ToString().Trim()+"/"+dtp_DateTo.Value.Year.ToString().Trim());
			string safeDateFrom = BizFunctions.GetSafeDateString(dateFrom);
			string safeDateTo = BizFunctions.GetSafeDateString(dateTo);
			ComboBox cb_accnum = BizXmlReader.CurrentInstance.GetControl("bank", "accnum") as ComboBox;
			string accnum = cb_accnum.SelectedValue.ToString().Trim();
            ComboBox cb_coy = BizXmlReader.CurrentInstance.GetControl("bank", "coy") as ComboBox;
            string coy = cb_coy.SelectedValue.ToString().Trim();
			ComboBox cb_period = BizXmlReader.CurrentInstance.GetControl("bank", "period") as ComboBox;
			//string period = cb_period.SelectedValue.ToString().Trim();

			#region add parameter field for accnum

			ParameterFields [] pf = new ParameterFields[1];			

			ParameterFields paramFields = new ParameterFields();
		
			ParameterField paramField_1 = new ParameterField();
			paramField_1.ParameterFieldName = "accnum";
			ParameterDiscreteValue discreteVal_1 = new ParameterDiscreteValue();
			discreteVal_1.Value = accnum;
			ParameterValues val_1 = new ParameterValues();
			val_1.Add(discreteVal_1);
			paramField_1.CurrentValues = val_1;
					
			paramFields.Add(paramField_1);

			pf[0] = paramFields;

			#endregion

			Parameter[] parameters = new Parameter[6];
			parameters[0] = new Parameter("@datefrm", safeDateFrom);
			parameters[1] = new Parameter("@dateto", safeDateTo);
			parameters[2] = new Parameter("@tablename", "csh"+Common.DEFAULT_SYSTEM_YEAR);
			parameters[3] = new Parameter("@acm", "acm");
			parameters[4] = new Parameter("@accnum", accnum);
            parameters[5] = new Parameter("@coy", coy);
			//DON20071027_REMOVE PRRIOD
			//parameters[6] = new Parameter("@period", period);

			try
			{
				DataSet ds_csh = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult("BizERP_PrintBankRecon", ref parameters);
                ds_csh.Tables[0].TableName = "coy";
                ds_csh.Tables[1].TableName = "acm";
                ds_csh.Tables[2].TableName = "print_bankrecon";

				DataTable dsDateTo = new DataTable();
				dsDateTo.TableName = "dsDateTo";
				dsDateTo.Columns.Add("dateto");

				DataRow dsDateTo_dr = dsDateTo.NewRow();
				dsDateTo_dr["dateto"] = dtp_DateTo.Value.Day.ToString().Trim() + "/" + dtp_DateTo.Value.Month.ToString().Trim() + "/" + dtp_DateTo.Value.Year.ToString().Trim();
				dsDateTo.Rows.Add(dsDateTo_dr);

				ds_csh.Tables.Add(dsDateTo);

				Form form = BizXmlReader.CurrentInstance.Load(this.projectPath + @"\BANK\Report\FormPreview.xml", "formPreview", this, null) as Form;
				CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;

				ReportDocument crReportDocument = new ReportDocument();
				crReportDocument.Load(this.projectPath+@"\BANK\Report\Bankrecon.rpt");
				crReportDocument.SetDataSource(ds_csh);
				crystalReportViewer1.ReportSource = crReportDocument;
				crystalReportViewer1.ParameterFieldInfo = pf[0];

				form.ShowDialog();
				form.Dispose();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		#endregion

		#region Find Clicked

		protected void btn_Find_Click(object sender, System.EventArgs e)
		{
			Form form = BizXmlReader.CurrentInstance.Load(this.projectPath+@"\BANK\Code\search.xml", "search", this, null) as Form;
			
			TextBox tb_refnum = BizXmlReader.CurrentInstance.GetControl("search", "search_refnum") as TextBox;
			DateTimePicker dtp_trandate = BizXmlReader.CurrentInstance.GetControl("search", "search_trandate") as DateTimePicker;
			TextBox tb_detail = BizXmlReader.CurrentInstance.GetControl("search", "search_detail") as TextBox;
			TextBox tb_chequenum = BizXmlReader.CurrentInstance.GetControl("search", "search_chequenum") as TextBox;
			TextBox tb_amount = BizXmlReader.CurrentInstance.GetControl("search", "search_amount") as TextBox;

			Binding binding1 = new Binding("Text", this.dbAccess.DataSet.Tables["search"], "amount");
			tb_amount.DataBindings.Add(binding1);
			Binding binding2 = new Binding("Text", this.dbAccess.DataSet.Tables["search"], "trandate");
			dtp_trandate.DataBindings.Add(binding2);

			BizDataValidate bizDataValidate = new BizDataValidate(this.dbAccess.ColumnsInfo);
			bizDataValidate.RegisterValidateEvent(form);

			ComboBox cb_sortedby = BizXmlReader.CurrentInstance.GetControl("bank", "sortedby") as ComboBox;
			switch(cb_sortedby.SelectedItem.ToString().Trim().ToUpper())
			{
				case "CHEQUE NO":
					tb_chequenum.ReadOnly = false;
					tb_chequenum.TabStop = true;
					tb_chequenum.Focus();
					break;
				case "WITHDRAWAL":
					tb_amount.Text = (0*(decimal)this.dbAccess.DataSet.Tables["search"].Rows[0]["amount"]).ToString("#0.00");
					tb_amount.ReadOnly = false;
					tb_amount.TabStop = true;
					tb_amount.Focus();
					break;
				case "DEPOSIT":
					tb_amount.Text = (0*(decimal)this.dbAccess.DataSet.Tables["search"].Rows[0]["amount"]).ToString("#0.00");
					tb_amount.ReadOnly = false;
					tb_amount.TabStop = true;
					tb_amount.Focus();
					break;
				case "DETAIL":
					tb_detail.ReadOnly = false;
					tb_detail.TabStop = true;
				tb_detail.Focus();
					break;
				case "DATE":
					dtp_trandate.Enabled = true;
					dtp_trandate.TabStop = true;
					dtp_trandate.Focus();
					break;
				default: // reference no; reconciled
					tb_refnum.ReadOnly = false;
					tb_refnum.TabStop = true;
					tb_refnum.Focus();
					break;
			}

			form.FormBorderStyle = FormBorderStyle.FixedSingle;
			form.ShowDialog();
			form.Closed += new EventHandler(searchform_Closed);
		}

		#endregion

		#region search Search Click

		protected void btn_search_Search_Click(object sender, System.EventArgs e)
		{
			TextBox tb_refnum = BizXmlReader.CurrentInstance.GetControl("search", "search_refnum") as TextBox;
			DateTimePicker dtp_trandate = BizXmlReader.CurrentInstance.GetControl("search", "search_trandate") as DateTimePicker;
			TextBox tb_detail = BizXmlReader.CurrentInstance.GetControl("search", "search_detail") as TextBox;
			TextBox tb_chequenum = BizXmlReader.CurrentInstance.GetControl("search", "search_chequenum") as TextBox;
			TextBox tb_amount = BizXmlReader.CurrentInstance.GetControl("search", "search_amount") as TextBox;

			DataView dv_csh = (DataView) this.dataGrid.DataSource;
			string filterClause = "";
			string filterValue = "";

			ComboBox cb_sortedby = BizXmlReader.CurrentInstance.GetControl("bank", "sortedby") as ComboBox;
			switch(cb_sortedby.SelectedItem.ToString().Trim().ToUpper())
			{
				case "CHEQUE NO":
					dv_csh.Sort = "chknum,[ID]";
					filterClause = "chknum='"+tb_chequenum.Text+"'";
					filterValue = tb_chequenum.Text;
					break;
				case "WITHDRAWAL":
					dv_csh.Sort = "withdrawal,[ID]";
					filterClause = "withdrawal="+tb_amount.Text;
					filterValue = tb_amount.Text;
					break;
				case "DEPOSIT":
					dv_csh.Sort = "deposit,[ID]";
					filterClause = "deposit="+tb_amount.Text;
					filterValue = tb_amount.Text;
					break;
				case "DETAIL":
					dv_csh.Sort = "detail,[ID]";
					filterClause = "detail='"+tb_detail.Text.Replace("'","''")+"'";
					filterValue = tb_detail.Text;
					break;
				case "DATE":
					dv_csh.Sort = "trandate,[ID]";
					string safeTranDate = dtp_trandate.Value.Day.ToString().Trim()+"/"+dtp_trandate.Value.Month.ToString().Trim()+"/"+dtp_trandate.Value.Year.ToString().Trim();
					filterClause = "trandate='"+safeTranDate+"'";
					break;
				default:	// reference no; reconciled
					dv_csh.Sort = "refnum,[ID]";
					filterClause = "refnum='"+tb_refnum.Text+"'";
					filterValue = tb_refnum.Text;
					break;
			}
			
			#region set CurrentRowIndex

			if(this.setFlag == 0) // 1st time trigger search
			{
				tb_refnum.ReadOnly = true;
				dtp_trandate.Enabled = false;
				tb_detail.ReadOnly = true;
				tb_chequenum.ReadOnly = true;
				tb_amount.ReadOnly = true;

				this.dr_csh = this.dbAccess.DataSet.Tables["csh"].Select(filterClause);

				if(dr_csh.Length != 0)
				{
					if(cb_sortedby.SelectedItem.ToString().Trim().ToUpper() == "DATE")
						this.dataGrid.CurrentRowIndex = dv_csh.Find(new object [] {dtp_trandate.Value, dr_csh[0]["ID"]});
					else
						this.dataGrid.CurrentRowIndex = dv_csh.Find(new object [] {filterValue, dr_csh[0]["ID"]});
					
					this.setFlag++;
					
					Button btn_search = BizXmlReader.CurrentInstance.GetControl("search", "Btn_Search") as Button;
					btn_search.Text = "Next";
				}
				else
				{
					MessageBox.Show("No Record Found");
				}
			}
			else //subsequent time trigger search
			{
				if(this.setFlag<this.dr_csh.Length)
				{
					if(cb_sortedby.SelectedItem.ToString().Trim().ToUpper() == "DATE")
						this.dataGrid.CurrentRowIndex = dv_csh.Find(new object [] {dtp_trandate.Value, dr_csh[this.setFlag]["ID"]});
					else
						this.dataGrid.CurrentRowIndex = dv_csh.Find(new object [] {filterValue, dr_csh[this.setFlag]["ID"]});

					this.setFlag++;
				}
				else
				{
					MessageBox.Show("No Record Found");
				}
			}

			#endregion
		}

		#endregion

		#region search Done Click

		protected void btn_search_Done_Click(object sender, System.EventArgs e)
		{
			this.searchform_Closed(sender, e);
			Form form = BizXmlReader.CurrentInstance.GetForm("search") as Form;
			form.Close();
		}

		#endregion

		#region searchform_Closed

		protected void searchform_Closed(object sender, System.EventArgs e)
		{
			this.setFlag = 0;
			this.dr_csh = null;

			if(this.dbAccess.DataSet.Tables["search"].Rows.Count !=0)
			{
				DataRow dr = this.dbAccess.DataSet.Tables["search"].Rows[0];
				dr["refnum"] = "";
				dr["chequenum"] = "";
				dr["detail"] = "";
				dr["trandate"] = System.DBNull.Value;
				dr["amount"] = 0.00;
			}
		}

		#endregion

        private void GetSum()
        {
            DateTimePicker dtp_DateFrom = BizXmlReader.CurrentInstance.GetControl("bank", "selDateFrom") as DateTimePicker;
            DateTimePicker dtp_DateTo = BizXmlReader.CurrentInstance.GetControl("bank", "selDateTo") as DateTimePicker;
            DateTime dateFrom = Convert.ToDateTime(dtp_DateFrom.Value.Day.ToString().Trim() + "/" + dtp_DateFrom.Value.Month.ToString().Trim() + "/" + dtp_DateFrom.Value.Year.ToString().Trim());
            DateTime dateTo = Convert.ToDateTime(dtp_DateTo.Value.Day.ToString().Trim() + "/" + dtp_DateTo.Value.Month.ToString().Trim() + "/" + dtp_DateTo.Value.Year.ToString().Trim());
            string safeDateFrom = BizFunctions.GetSafeDateString(dateFrom);
            string safeDateTo = BizFunctions.GetSafeDateString(dateTo);
            ComboBox cb_accnum = BizXmlReader.CurrentInstance.GetControl("bank", "accnum") as ComboBox;
            string accnum = cb_accnum.SelectedValue.ToString().Trim();
            ComboBox cb_coy = BizXmlReader.CurrentInstance.GetControl("bank", "coy") as ComboBox;
            string coy = cb_coy.SelectedValue.ToString().Trim();
            ComboBox cb_period = BizXmlReader.CurrentInstance.GetControl("bank", "period") as ComboBox;
            //string period = cb_period.SelectedValue.ToString().Trim();

            Label lbl_bankamt = BizXmlReader.CurrentInstance.GetControl("bank", "lbl_bankamt") as Label;
            Label lbl_subtotal = BizXmlReader.CurrentInstance.GetControl("bank", "lbl_subtotal") as Label;

            TextBox bankamt = BizXmlReader.CurrentInstance.GetControl("bank", "bankamt") as TextBox;
            TextBox cshbkbal = BizXmlReader.CurrentInstance.GetControl("bank", "cshbkbal") as TextBox;
            TextBox bankst = BizXmlReader.CurrentInstance.GetControl("bank", "bankst") as TextBox;
            TextBox diff = BizXmlReader.CurrentInstance.GetControl("bank", "diff") as TextBox;

            #region add parameter field for accnum

            ParameterFields[] pf = new ParameterFields[1];

            ParameterFields paramFields = new ParameterFields();

            ParameterField paramField_1 = new ParameterField();
            paramField_1.ParameterFieldName = "accnum";
            ParameterDiscreteValue discreteVal_1 = new ParameterDiscreteValue();
            discreteVal_1.Value = accnum;
            ParameterValues val_1 = new ParameterValues();
            val_1.Add(discreteVal_1);
            paramField_1.CurrentValues = val_1;

            paramFields.Add(paramField_1);

            pf[0] = paramFields;

            #endregion

            Parameter[] parameters = new Parameter[6];
            parameters[0] = new Parameter("@datefrm", safeDateFrom);
            parameters[1] = new Parameter("@dateto", safeDateTo);
            parameters[2] = new Parameter("@tablename", "csh" + Common.DEFAULT_SYSTEM_YEAR);
            parameters[3] = new Parameter("@acm", "acm");
            parameters[4] = new Parameter("@accnum", accnum);
            parameters[5] = new Parameter("@coy", coy);
            //DON20071027_REMOVE PRRIOD
            //parameters[6] = new Parameter("@period", period);

            try
            {
                DataSet ds_csh = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult("BizERP_PrintBankRecon", ref parameters);
                ds_csh.Tables[0].TableName = "coy";
                ds_csh.Tables[1].TableName = "acm";
                ds_csh.Tables[2].TableName = "print_bankrecon";

                DataTable dsDateTo = new DataTable();
                dsDateTo.TableName = "dsDateTo";
                dsDateTo.Columns.Add("dateto");

                DataRow dsDateTo_dr = dsDateTo.NewRow();
                dsDateTo_dr["dateto"] = dtp_DateTo.Value.Day.ToString().Trim() + "/" + dtp_DateTo.Value.Month.ToString().Trim() + "/" + dtp_DateTo.Value.Year.ToString().Trim();
                dsDateTo.Rows.Add(dsDateTo_dr);

                ds_csh.Tables.Add(dsDateTo);

                //Form form = BizXmlReader.CurrentInstance.Load(this.projectPath + @"\BANK\Report\FormPreview.xml", "formPreview", this, null) as Form;
                //CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;

                //ReportDocument crReportDocument = new ReportDocument();
                //crReportDocument.Load(this.projectPath + @"\BANK\Report\Bankrecon.rpt");
                //crReportDocument.SetDataSource(ds_csh);
                //crystalReportViewer1.ReportSource = crReportDocument;
                //crystalReportViewer1.ParameterFieldInfo = pf[0];

                //form.ShowDialog();
                //form.Dispose();

                decimal subTotal = 0;
   
                if (ds_csh.Tables["print_bankrecon"] != null)
                {
                    if (ds_csh.Tables["print_bankrecon"].Rows.Count > 0)
                    {
                        DataRow d = ds_csh.Tables["print_bankrecon"].Rows[0];
                        cshbkbal.Text = d["bankamt"].ToString();
                        //foreach (DataRow dr1 in ds_csh.Tables["print_bankrecon"].Rows)
                        //{
                        //    if (dr1["printsection"].ToString().Trim() == "B")
                        //    {
                        //        //lbl_subtotal.Text = dr1["withdrawal"];

                        //        subTotal = subTotal + Convert.ToDecimal(dr1["withdrawal"]);
                        //    }
                        //    else if (dr1["printsection"].ToString().Trim() == "C")
                        //    {
                        //        //lbl_subtotal.Text = dr1["deposit"];
                        //        subTotal = subTotal + Convert.ToDecimal(dr1["deposit"]);
                        //    }
                        //    else
                        //    {
                        //        //lbl_subtotal.Text = 0;
                        //    }
                        //}
                        DataRow b = ds_csh.Tables["print_bankrecon"].Rows[ds_csh.Tables["print_bankrecon"].Rows.Count-1];

                        if (b["printsection"].ToString().Trim() == "B")
                        {
                            //lbl_subtotal.Text = b["withdrawal"];

                            subTotal = Convert.ToDecimal(b["withdrawal"]);
                        }
                        //else if (b["printsection"].ToString().Trim() == "C")
                        //{
                        //    //lbl_subtotal.Text = b["deposit"];
                        //    subTotal = Convert.ToDecimal(b["deposit"]);
                        //}
                        else
                        {
                            if (BizFunctions.IsEmpty(b["bankamt"]))
                            {
                                b["bankamt"] = 0;
                            }
                            subTotal = Convert.ToDecimal(b["bankamt"]);
                        }
                        bankst.Text = subTotal.ToString();

                        decimal value = 0;

                        if (bankamt.Text == string.Empty)
                        {
                            bankamt.Text = value.ToString();
                        }

                        if (bankst.Text == string.Empty)
                        {
                            bankamt.Text = value.ToString();
                        }


                        if (bankamt.Text != string.Empty)
                        {
                            if (bankamt.Text.Length > 0)
                            {
                                decimal n;
                                bool isNumeric = decimal.TryParse(bankamt.Text, out n);
                             
                                if (isNumeric)
                                {
                                    decimal a = Convert.ToDecimal(bankamt.Text) - Convert.ToDecimal(bankst.Text);
                                    diff.Text = a.ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
	}
}