/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		pd.cs
 *	Description:    Lock Financial Periods
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

namespace ATL.EXRATE
{
	public class EXRATE
	{
		protected DBAccess	dbAccess			= null;
		protected DataSet	ds					= null;
		protected Hashtable	selectsCollection	= null;
		
		protected string projectPath	= null;

		public EXRATE()
		{
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
			Form form = BizXmlReader.CurrentInstance.Load(this.projectPath+@"\EXRATE\exrate.xml", "exr", this, null) as Form;

			this.dbAccess			= new DBAccess();
			this.selectsCollection	= new Hashtable();

			DataGrid dataGrid = (DataGrid)BizXmlReader.CurrentInstance.GetControl("exr", "dataGrid");

			this.selectsCollection.Add("exr", "SELECT * FROM exr"+Common.DEFAULT_SYSTEM_YEAR + " WHERE ISNULL(status,'')<>'V' ");
			this.selectsCollection.Add("SysLocker", "SELECT * FROM SysLocker WHERE ModuleName='EXRATE' AND [Key]='EXRATE'");
			this.dbAccess.ReadSQL(selectsCollection);

			if(this.dbAccess.DataSet.Tables["SysLocker"].Rows.Count != 0)
			{
				MessageBox.Show("Exrate Settings is locked/in use !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				this.dbAccess.RemoteStandardSQL.ExecuteNonQuery("INSERT INTO SysLocker (ModuleName, [Key], UserName) VALUES ('EXRATE','EXRATE','"+Common.DEFAULT_SYSTEM_USERNAME+"')");
				this.ds = this.dbAccess.DataSet;

				BizGridInfo detailGridInfo = (BizGridInfo)BizXmlReader.CurrentInstance.Load(this.projectPath+@"\EXRATE\DetailGridInfo_EXRATE.xml", "detailGridInfo", null, null);
			
				BizBinding.DataGridBinding(dataGrid, this.ds, detailGridInfo, this.dbAccess.ColumnsInfo, "", "", DataViewRowState.CurrentRows);

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
				DataTable exr = this.dbAccess.DataSet.Tables["exr"];
				foreach(DataRow dr in exr.Rows)
				{
					if(dr.RowState == DataRowState.Modified)
					{
						dr["modified"] = System.DateTime.Now;
						dr["user"] = Common.DEFAULT_SYSTEM_USERNAME;
					}
				}
			
				DataTable[] savetable = new DataTable[1];
				savetable[0] = exr.GetChanges();
				if(savetable[0] != null)
				{
					savetable[0].TableName = "exr"+Common.DEFAULT_SYSTEM_YEAR;
					this.dbAccess.Update(savetable);
				}
			}
		}

		#endregion

		#region Cancel Clicked

		protected void btn_Cancel_Click(object sender, System.EventArgs e)
		{
			Form form = BizXmlReader.CurrentInstance.GetForm("exr") as Form;
			form.Close();
		}

		#endregion

		#region form_Closed

		protected void form_Closed(object sender, System.EventArgs e)
		{
			this.dbAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM SysLocker WHERE ModuleName='EXRATE' AND [Key]='EXRATE'");
		}

		#endregion
	}
}