/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		ACC.cs
 *	Description:    Set System Default Accounts
 *	Function List:	
 * 
 * History			��ʷ�޸���Ӽ�¼
 * ---------------------------------------------------------
 * Author			Time				Description
 *
***********************************************************/

using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizTools;
using BizRAD.BizAccounts;
using BizRAD.BizControls.DataGridColumns;

namespace ATL.ACC
{
	public class ACC
	{
		protected DBAccess	dbAccess			= null;
		protected DataSet	ds					= null;
		protected Hashtable	selectsCollection	= null;
		protected BizDataGridTextBoxColumn bizDataGridTextBoxColumn = null;
		
		protected string projectPath	= null;

		public ACC()
		{
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
			Form form = BizXmlReader.CurrentInstance.Load(this.projectPath+@"\ACC\acc.xml", "acc", this, null) as Form;

			this.dbAccess			= new DBAccess();
			this.selectsCollection	= new Hashtable();

			DataGrid dataGrid = (DataGrid)BizXmlReader.CurrentInstance.GetControl("acc", "dataGrid");

			this.selectsCollection.Add("acc", "SELECT * FROM acc ORDER BY refnum");
			this.selectsCollection.Add("SysLocker", "SELECT * FROM SysLocker WHERE ModuleName='ACC' AND [Key]='ACC'");
			this.dbAccess.ReadSQL(selectsCollection);

			if(this.dbAccess.DataSet.Tables["SysLocker"].Rows.Count != 0)
			{
				MessageBox.Show("Default Account Settings is locked/in use !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				this.dbAccess.RemoteStandardSQL.ExecuteNonQuery("INSERT INTO SysLocker (ModuleName, [Key], UserName) VALUES ('ACC','ACC','"+Common.DEFAULT_SYSTEM_USERNAME+"')");
				this.ds = this.dbAccess.DataSet;

				BizGridInfo detailGridInfo = (BizGridInfo)BizXmlReader.CurrentInstance.Load(this.projectPath+@"\ACC\DetailGridInfo_Acc.xml", "detailGridInfo", null, null);
			
				BizBinding.DataGridBinding(dataGrid, this.ds, detailGridInfo, this.dbAccess.ColumnsInfo, "", "", DataViewRowState.CurrentRows);

				foreach(DataGridTableStyle dataGridTableStyle in dataGrid.TableStyles)
				{
					foreach(DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
					{
						if(dataGridColumnStyle is BizDataGridTextBoxColumn)
						{
							BizDataGridTextBoxColumn bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;

//							if(bizDataGridTextBoxColumn.TextBoxGrid.MappingName == "accnum")
//							{
								bizDataGridTextBoxColumn.TextBoxGrid.DataGridKeyDownF2 += new KeyDownF2EventHandler(TextBoxGrid_DataGridKeyDownF2);
//							}
						}
					}
				}

				CurrencyManager currencyManager = dataGrid.BindingContext[dataGrid.DataSource, dataGrid.DataMember] as CurrencyManager;
				(currencyManager.List as DataView).AllowNew = false;

				form.Closed += new EventHandler(form_Closed);

				form.FormBorderStyle = FormBorderStyle.FixedSingle;
				form.ShowDialog();	
			}
		}

		#region Update Clicked

		protected void btn_Update_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("Confirm Update Changes to Database?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DataTable acc = this.dbAccess.DataSet.Tables["acc"];
				foreach(DataRow dr in acc.Rows)
				{
					if(dr.RowState == DataRowState.Modified)
					{
						dr["modified"] = System.DateTime.Now;
						dr["user"] = Common.DEFAULT_SYSTEM_USERNAME;
					}
				}
			
				DataTable[] savetable = new DataTable[1];
				savetable[0] = acc.GetChanges();
				if(savetable[0] != null)
				{
					savetable[0].TableName = "acc";
					this.dbAccess.Update(savetable);
				}
			}
		}

		#endregion

		#region Cancel Clicked

		protected void btn_Cancel_Click(object sender, System.EventArgs e)
		{
			Form form = BizXmlReader.CurrentInstance.GetForm("acc") as Form;
			form.Close();
		}

		#endregion

		#region form_Closed

		protected void form_Closed(object sender, System.EventArgs e)
		{
			this.dbAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM SysLocker WHERE ModuleName='ACC' AND [Key]='ACC'");
		}

		#endregion

		private void TextBoxGrid_DataGridKeyDownF2(object sender, DataGridEventArgs e)
		{
			MessageBox.Show("hi","hi");
			// ???
		}

	}
}