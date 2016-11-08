using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;
using BizRAD.BizCommon;

namespace ATL.PCR
{
    public partial class ExtractPIVForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable trr1;
        public ExtractPIVForm(DBAccess dbAccess, DataTable originalTable)
        {
            this.dbAccess = dbAccess;
            this.oriTable = originalTable;

            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshDatagrid()
        {
            DataSet ds = new DataSet();

			DataRow pivh = this.dbAccess.DataSet.Tables["pivh"].Rows[0];
			string sql = "SELECT refnum,apnum,invnum,matnum,uom,discamt,detail,price,dqty FROM piv1 ";

            if (!BizFunctions.IsEmpty(pivh["apnum"]))
            {
				sql += "  WHERE flag = 'PIV' and apnum ='" + pivh["apnum"].ToString() + "'";
            }
          
            ds = dbAccess.ReadSQLTemp("tmp", sql);
            DataTable dt=ds.Tables["tmp"];

            dt_view = dt.Copy();
            dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
            dt_view.Columns[7].DefaultValue = 0;
            
            this.dgView1.DataSource = dt_view;
        }

        private void ExtractForm_Load(object sender, EventArgs e)
        {
            try
            {              
                this.RefreshDatagrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnMarkAll_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in this.dt_view.Rows)
            {
                row["Mark"] = 1;
            }
        }

        private void btnUnMark_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in this.dt_view.Rows)
            {
                row["Mark"] = 0;
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow row in this.dt_view.Rows)
                {
                    if (row["Mark"] != System.DBNull.Value)
                    {
                        if ((bool)row["Mark"] == true)
                        {
                            DataRow piv1tmp = dbAccess.DataSet.Tables["piv1"].NewRow();
                            foreach (DataColumn dc in dt_view.Columns)
                            {

                                if (dbAccess.DataSet.Tables["piv1"].Columns.IndexOf(dc.ColumnName) != -1)
                                {
                                    piv1tmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
                            piv1tmp["Mark"] = 0;
                            NewRow(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.Close();
            }
        }

        protected void NewRow(DataRow dr)
        {
            DataTable piv1 = dbAccess.DataSet.Tables["piv1"];
            DataRow newRow = piv1.NewRow();
			newRow["invnum"] = dr["invnum"];
            newRow["matnum"] = dr["matnum"];
            newRow["detail"] = dr["detail"];
            newRow["uom"] = dr["uom"];
            newRow["discamt"] = dr["discamt"];
            newRow["dqty"] = dr["dqty"];
            newRow["price"] = dr["price"];
            piv1.Rows.Add(newRow);
        }
    }
}