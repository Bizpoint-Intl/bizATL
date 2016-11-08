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

namespace ATL.PAY
{
    public partial class ExtractAPForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable trr1;
        public ExtractAPForm(DBAccess dbAccess, DataTable originalTable)
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

			DataRow payt = this.dbAccess.DataSet.Tables["payt"].Rows[0];
			//string sql = "select MAX(refnum) as refnum,max(invdate) as invdate,invnum,max(detail) as detail, " +
			//    " max(oricur) as oricur,max(exrate) as exrate,sum(-oriamt) as oriamt,sum(dpostamt) as dpostamt," +
			//    " max(accnum) as accnum from apd" + Common.DEFAULT_SYSTEM_YEAR;
			string sql = "SELECT MIN(trandate) as invdate,invnum,supinvnum,detail, " +
				" oricur,exrate,sum(-oriamt) as oriamt,sum(dpostamt) as dpostamt," +
				" accnum FROM apd" + Common.DEFAULT_SYSTEM_YEAR;


            if (!BizFunctions.IsEmpty(payt["apnum"]))
            {
				sql += "  WHERE  apnum ='" + payt["apnum"].ToString() + "'";
            }

			sql += " GROUP BY invnum,supinvnum,detail,oricur,exrate,accnum HAVING SUM(-oriamt)<>0 ";
          
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
							DataRow apdtmp = dbAccess.DataSet.Tables["apd"].NewRow();
                            foreach (DataColumn dc in dt_view.Columns)
                            {

								if (dbAccess.DataSet.Tables["apd"].Columns.IndexOf(dc.ColumnName) != -1)
                                {
									apdtmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
							apdtmp["Mark"] = 0;
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
            DataTable apd = dbAccess.DataSet.Tables["apd"];
			DataRow newRow = apd.NewRow();
			newRow["invnum"] = dr["invnum"];
			newRow["invdate"] = dr["invdate"];
			newRow["supinvnum"] = dr["supinvnum"];
            newRow["detail"] = dr["detail"];
			newRow["pivamt"] = dr["oriamt"];
            apd.Rows.Add(newRow);
        }
    }
}
