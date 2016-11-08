/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		accsp.cs
 *	Description:    Modify Default Accounts
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Jer              070222              1st version [TODO: error check for manual entries, pull accname for manual entries and solve redrawing of value after F3 and mouse hasn't left cell glitch]
***********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizBase;
using BizRAD.BizApplication;
using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;


namespace ATL.BizModules.FINRPT.Code
{
    public partial class finrptlocal : Form
    {
        protected DBAccess dbAccess = null;
        protected SystemDBAccess systemDBAccess = null;
        protected Form frmThis = null;
        protected string projectPath = null;
        protected DataRow currentRow = null;
        protected TextBox textBox = null;

        public finrptlocal(DBAccess dbAccess, TextBox textBox, DataRow currentRow)
        {
            InitializeComponent();
            this.dbAccess = dbAccess;
            this.currentRow = currentRow;
            this.textBox = textBox;
            this.frmThis = this;
            this.systemDBAccess = new SystemDBAccess();
        }

        private void LOCAL_Load(object sender, EventArgs e)
        {						
            //user can double click on cell, then select it
            this.dgv_finrptlocal.CellDoubleClick += new DataGridViewCellEventHandler(CellDoubleClick);

			this.dgv_finrptlocal.KeyDown += new KeyEventHandler(dgv_finrptlocal_KeyDown);

            try
            {
                DataRow finrpth = this.dbAccess.DataSet.Tables["finrpth"].Rows[0];
                DataTable finrpt1 = this.dbAccess.DataSet.Tables["finrpt1"];
                DataTable finrpt2 = this.dbAccess.DataSet.Tables["finrpt2"];
                this.dgv_finrptlocal.AutoGenerateColumns = true;


                string sqlcommand = "SELECT gnum AS 'Group Num', MAX(isnull(gname, '')) AS 'Group Name', "+
					" remark AS 'Remarks' FROM [finrpt1mirror] WHERE refnum = '" + finrpth["refnum"].ToString() + "' "+
					" AND gnum LIKE '%"+this.textBox.Text.Trim()+"%' "+
					" AND ISNULL(gnum,'') NOT IN (SELECT ISNULL(gnum,'') FROM [finrpt2])" + 
					" GROUP BY gnum,remark";

                DataTable dt = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, sqlcommand);
                this.dgv_finrptlocal.DataSource = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, sqlcommand);

                this.dgv_finrptlocal.Columns[0].ReadOnly = true;
                this.dgv_finrptlocal.Columns[1].ReadOnly = true;
				this.dgv_finrptlocal.Columns[1].Width = 280;
                this.dgv_finrptlocal.Columns[2].ReadOnly = true;                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

		void dgv_finrptlocal_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F9)
			{
				try
				{
					int currRow = this.dgv_finrptlocal.CurrentCell.RowIndex;
					DataGridViewRow dr = this.dgv_finrptlocal.Rows[currRow];
					this.currentRow.BeginEdit();
					this.currentRow["gnum"] = dr.Cells[0].Value.ToString();
					this.currentRow["gname"] = dr.Cells[1].Value.ToString();
					this.currentRow.EndEdit();
					this.frmThis.Close();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			else if (e.KeyCode == Keys.Enter)
			{
				//steph - allow the system to move to next record
			}
			else
			{
				MessageBox.Show("Press F9 to select!");
			}
		}
				
        protected void CellDoubleClick(object sender, System.EventArgs e)
        {
            try
            {
                int currRow = this.dgv_finrptlocal.CurrentCell.RowIndex;
                DataGridViewRow dr = this.dgv_finrptlocal.Rows[currRow];
                this.currentRow.BeginEdit();
                this.currentRow["gnum"] = dr.Cells[0].Value.ToString();
                this.currentRow["gname"] = dr.Cells[1].Value.ToString();  
                this.currentRow.EndEdit();
                this.frmThis.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int currRow = this.dgv_finrptlocal.CurrentCell.RowIndex;
                DataGridViewRow dr = this.dgv_finrptlocal.Rows[currRow];

                this.currentRow["gnum"] = dr.Cells[0].Value.ToString();
                this.currentRow["gname"] = dr.Cells[1].Value.ToString();  
               
                this.frmThis.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.frmThis.Close();
        }

        private void dgv_uomc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}