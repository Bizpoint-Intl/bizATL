/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		gstsp.cs
 *	Description:    Modify Tax Rates
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Jer              070121              1st version [TODO: DateTimePicker instead]
***********************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;

namespace ATL.GSTSP
{
	public partial class GSTSP : Form
	{
		protected Form myForm = null;
		protected DBAccess dbAccess = null;
        protected SystemDBAccess systemDBAccess = null;
		protected Hashtable selectsCollection = null;
        
		public GSTSP()
        {
            InitializeComponent();

            this.myForm = this;
            this.dbAccess = new DBAccess();
            this.systemDBAccess = new SystemDBAccess();
            this.selectsCollection = new Hashtable();
			
            // lock and prevent other users from using same module
			bool isLocked = systemDBAccess.IsLocked("TAXRATE", "TAX RATES", Common.DEFAULT_SYSTEM_USERNAME);
			
            if (isLocked == true)
            {
                this.myForm.Close();
                MessageBox.Show("System Tax Rates Settings is locked/in use !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.myForm.ShowDialog();
            } 
        }

		private void GSTSP_Load(object sender, EventArgs e)
		{
			// Load the tax rates table
            string cmd = "SELECT effectivedate,taxpercent FROM systaxrates ORDER BY effectivedate ASC";
			selectsCollection.Clear();
			selectsCollection.Add("localgst", cmd);
			this.dbAccess.ReadSQL(selectsCollection);

			this.dg_gstsp.DataSource = this.dbAccess.DataSet.Tables["localgst"];
			
			#region Set the display of the dataGridView

			foreach (DataGridViewColumn dgvc in this.dg_gstsp.Columns)
			{
				switch (dgvc.Name)
				{
					case "taxpercent":
						dgvc.HeaderText = "Tax Rate";
                        dgvc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvc.DefaultCellStyle.Format = "f2";
						dgvc.Width = 120;
                        dgvc.Resizable = DataGridViewTriState.False;
						break;
                    case "effectivedate":
                        dgvc.HeaderText = "Effective Date";
                        dgvc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvc.DefaultCellStyle.Format = "d";
                        dgvc.Width = 150;
                        dgvc.Resizable = DataGridViewTriState.False;
                        break;
				}
			}

			#endregion	
			 
		}

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            // Remove entry from SysLocker
            bool isUnlock = systemDBAccess.UnLock("TAXRATE", "TAX RATES");
            this.myForm.Dispose();
        }

		private void btn_save_Click(object sender, EventArgs e)
		{
			try
			{
                #region Check no empty dates and taxpercents
                foreach (DataRow dr in this.dbAccess.DataSet.Tables["localgst"].Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        if (Convert.IsDBNull(dr["taxpercent"]))
                        {
                            MessageBox.Show("TaxPercent cannot be empty!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            if ((decimal)dr["taxpercent"] < 0)
                            {
                                MessageBox.Show("TaxPercent cannot be negative!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        if (Convert.IsDBNull(dr["effectivedate"]))
                        {
                            MessageBox.Show("Effective Date cannot be empty!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                DataTable dtTMp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "SELECT COUNT(effectivedate) as cnt FROM [localgst] GROUP BY effectivedate HAVING COUNT(effectivedate)>1");
                if (dtTMp.Rows.Count > 0)
                {
                    MessageBox.Show("Duplicate Effective Dates entered!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                #endregion

                // For simplicity, drop the whole tax table, and reinsert it from the datagrid
				string clearString = "DELETE FROM systaxrates";
				this.dbAccess.RemoteStandardSQL.ExecuteNonQuery(clearString);
				int count=0;

				foreach (DataRow dr in this.dbAccess.DataSet.Tables["localgst"].Rows)
				{
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        string updateString = "INSERT INTO systaxrates(id,effectivedate,taxpercent,[user]) VALUES " +
                                              "(" + count + ",'" + BizFunctions.GetStandardDateString((DateTime)dr["effectivedate"]) + "'," + dr["taxpercent"] + ",'" + Common.DEFAULT_SYSTEM_USERNAME + "')";

                        this.dbAccess.RemoteStandardSQL.ExecuteNonQuery(updateString);
                        count++;
                    }
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.dbAccess.DataSet.Tables["localgst"].AcceptChanges();
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}
	}
}