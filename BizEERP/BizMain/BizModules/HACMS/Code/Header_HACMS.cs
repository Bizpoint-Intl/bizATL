/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Header_HACMS.cs
 *	Description:    H/R Appraisal Code Main Section Header
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
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizControls.DataGridColumns;

namespace ATL.HACMS
{
	public class Header_HACMS
	{
		protected DBAccess	dbAccess		= null;
		protected DataSet	dataSet			= null;
		protected Hashtable	formsCollection	= null;
		protected string documentKey		= null;

		protected string acmsnum			= null;
		protected string acsscode			= null;
		protected string acssnum			= null;
		protected string acssname			= null;

		protected string projectPath		= null;

		public Header_HACMS(DBAccess dbAccess, Hashtable formsCollection, string DocumentKey)
		{
			this.dbAccess			= dbAccess;
			this.dataSet			= this.dbAccess.DataSet;
			this.formsCollection	= formsCollection;
			this.documentKey		= DocumentKey;
            this.projectPath        = ConfigurationManager.AppSettings.Get("ProjectPath");
		}

		#region detail_Load

		protected void detail_Load(object sender, System.EventArgs e)
		{
			DataGrid dataGrid = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "dg_Detail") as DataGrid;

			foreach(DataGridTableStyle dataGridTableStyle in dataGrid.TableStyles)
			{
				foreach(DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
				{
					if(dataGridColumnStyle is BizDataGridTextBoxColumn)
					{
						BizDataGridTextBoxColumn bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;
				
						if(bizDataGridTextBoxColumn.TextBoxGrid.MappingName == "acssname")
						{
							bizDataGridTextBoxColumn.TextBoxGrid.DoubleClick += new EventHandler(TextBoxGrid_DoubleClick);
						}
					}
				}
			}
		}

		#endregion

		#region Double Click

		protected void TextBoxGrid_DoubleClick(object sender, System.EventArgs e)
		{
			Form form = BizXmlReader.CurrentInstance.Load(this.projectPath+@"\HACMS\UIFile\Apc.xml", "apc", this, null) as Form;

			string form_detail = (this.formsCollection["header"] as Form).Name;
			DataGrid dg_detail = BizXmlReader.CurrentInstance.GetControl(form_detail,"dg_Detail") as DataGrid;
			DataRowView dv_currentRow = dg_detail.BindingContext[dg_detail.DataSource as DataView].Current as DataRowView;

			this.acmsnum = dv_currentRow["acmsnum"].ToString();
			this.acsscode = dv_currentRow["acsscode"].ToString();
			this.acssnum = dv_currentRow["acssnum"].ToString();
			this.acssname = dv_currentRow["acssname"].ToString();

			if(this.acsscode == String.Empty)
			{
				MessageBox.Show("Empty Field 'Appraisal Code Category' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else if (this.acssname == String.Empty)
			{
				MessageBox.Show("Empty Field 'Description' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				form.ShowDialog();
			}
		}

		#endregion

		#region apc_Load

		protected void apc_Load(object sender, System.EventArgs e)
		{
			DataTable	hacm		= this.dbAccess.DataSet.Tables["hacm"];
			BizGridInfo bizGridInfo	= BizXmlReader.CurrentInstance.Load(this.projectPath+@"\HACMS\InfoFile\BizGridInfo_HACM.xml", "detailGridInfo", null, null) as BizGridInfo;
			DataGrid	dataGrid	= BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "dg_Apc") as DataGrid;

			Hashtable selectsCollection = new Hashtable();
			selectsCollection.Add("hacmTmp","SELECT * FROM hacm WHERE 1=2");
			this.dbAccess.ReadSQL(selectsCollection);

			DataTable hacmTmp = this.dbAccess.DataSet.Tables["hacmTmp"];
			DataRow [] hacmCurrent = hacm.Select("acsscode='"+this.acsscode+"'");
			for(int i=0; i<hacmCurrent.Length; i++)
			{
				DataRow new_dr = hacmTmp.Rows.Add(new object[] {});
				new_dr["acmsnum"] = this.acmsnum;
				new_dr["acsscode"] = this.acsscode;
				new_dr["apcnum"] = hacmCurrent[i]["apcnum"];
				new_dr["apccode"] = hacmCurrent[i]["apccode"];
				new_dr["apcname"] = hacmCurrent[i]["apcname"];
				new_dr["mark"] = 0;
			}

			bizGridInfo.MainTable = "hacmTmp";

			BizBinding.DataGridBinding(dataGrid, this.dbAccess.DataSet, bizGridInfo, null, null, null, DataViewRowState.CurrentRows);
			 
			CurrencyManager currencyManager = dataGrid.BindingContext[dataGrid.DataSource, dataGrid.DataMember] as CurrencyManager;
			(currencyManager.List as DataView).AllowNew = false;

			TextBox tb_acsscode = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "CurrentAcsscode") as TextBox;
			tb_acsscode.Text = this.acsscode;
			TextBox tb_acssname = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "CurrentAcssname") as TextBox;
			tb_acssname.Text = this.acssname;
		}

		#endregion

		#region btn_Insert_Click

		protected void btn_Insert_Click(object sender, System.EventArgs e)
		{
			DataTable hacmTmp = this.dbAccess.DataSet.Tables["hacmTmp"];
			DataRow new_dr = hacmTmp.Rows.Add(new object[] {});
			new_dr["acmsnum"] = this.acmsnum;
			new_dr["acssnum"] = this.acssnum;
			new_dr["acsscode"] = this.acsscode;
			new_dr["mark"] = 0;
		}

		#endregion

		#region btn_Delete_Click

		protected void btn_Delete_Click(object sender, System.EventArgs e)
		{
			DataTable hacmTmp = this.dbAccess.DataSet.Tables["hacmTmp"];

			int rowCount = hacmTmp.Rows.Count;

			for(int i=rowCount; i>0; i--)
			{
				if(hacmTmp.Rows[i-1].RowState != DataRowState.Deleted)
				{
					if(Convert.ToBoolean(hacmTmp.Rows[i-1]["mark"]))
						hacmTmp.Rows[i-1].Delete();
				}
			}
		}

		#endregion

		#region btn_Save_Click

		protected void btn_Save_Click(object sender, System.EventArgs e)
		{
			DataTable hacmTmp = this.dbAccess.DataSet.Tables["hacmTmp"];
			DataTable hacm = this.dbAccess.DataSet.Tables["hacm"];
			
			BizFunctions.DeleteRow(hacm, "acsscode='"+this.acsscode+"'");
			
			foreach(DataRow dr in hacmTmp.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					dr["apccode"] = this.acsscode + "-" + dr["apcnum"].ToString().Trim();

					DataRow new_dr = hacm.Rows.Add(new object[] {});
					new_dr["acmsnum"] = this.acmsnum;
					new_dr["acsscode"] = this.acsscode;
					new_dr["apcnum"] = dr["apcnum"];
					new_dr["apccode"] = dr["apccode"];
					new_dr["apcname"] = dr["apcname"];
					new_dr["mark"] = dr["mark"];
				}
			}
		}

		#endregion

		#region apc_Closing

		protected void apc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			BizXmlReader.CurrentInstance.RemoveForm((sender as Form).Name);
			DataTable hacmTmp = this.dbAccess.DataSet.Tables["hacmTmp"];
			BizFunctions.DeleteAllRows(hacmTmp);
			this.dbAccess.DataSet.Tables.Remove("hacmTmp");
		}

		#endregion
	}
}
