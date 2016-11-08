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

namespace ATL.TRI
{
    public partial class ExtrictGRNForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable tri1;
        public ExtrictGRNForm(DBAccess dbAccess, DataTable originalTable)
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

            DataRow trih = this.dbAccess.DataSet.Tables["trih"].Rows[0];
            string sql = "select * from(select grn1.refnum as grnrefnum,grn1.matnum,grn1.uom,grn1.detail,grn1.price," +
            "case when (ISNULL(grn1.qty,0) - ISNULL(tri1.qty,0))< 0 then 0 else (ISNULL(grn1.qty,0) - ISNULL(tri1.qty,0)) end as qty " +
            "from (select refnum,matnum,uom,detail,sum(qty) as qty,sum(price)as price from grn1 where status='P' and apnum ='" + trih["apnum"].ToString() + "' group by refnum,matnum,uom,detail)grn1 " +
            "left join(select grnrefnum,matnum,sum(qty) as qty,sum(price)as price from tri1 where isnull(status,'')<>'V' group by grnum,matnum)grt1 " +
            "on grn1.refnum=tri1.grnrefnum and grn1.matnum= tri1.matnum where (grn1.qty-ISNULL(tri1.qty,0)) > 0)tmp1";

            if (!BizFunctions.IsEmpty(trih["grnrefnum"]))
            {
                sql += " where tmp1.grnrefnum ='" + trih["grnrefnum"].ToString() + "'";
            }

            ds = dbAccess.ReadSQLTemp("tmp", sql);
            DataTable dt = ds.Tables["tmp"];

            dt_view = dt.Copy();
            dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
            dt_view.Columns[14].DefaultValue = 0;

            this.dgView1.DataSource = dt_view;
        }

        private void ExtrictForm_Load(object sender, EventArgs e)
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

        private void btnExtrict_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow row in this.dt_view.Rows)
                {
                    if (row["Mark"] != System.DBNull.Value)
                    {
                        if ((bool)row["Mark"] == true)
                        {
                            DataRow trr1tmp = dbAccess.DataSet.Tables["tri1"].NewRow();
                            string size = null;
                            Int32 qty = 0;
                            foreach (DataColumn dc in dt_view.Columns)
                            {
                                if (dbAccess.DataSet.Tables["tri1"].Columns.IndexOf(dc.ColumnName) != -1)
                                {
                                    trr1tmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
                            trr1tmp["Mark"] = 0;
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
            DataTable tri1 = dbAccess.DataSet.Tables["tri1"];
            DataRow newRow = tri1.NewRow();
            newRow["grnrefnum"] = dr["grnrefnum"];
            newRow["matnum"] = dr["matnum"];
            newRow["detail"] = dr["detail"];
            newRow["uom"] = dr["uom"];
            //newRow["pcatcode"] = dr["pcatcode"];
            newRow["qty"] = dr["qty"];
            newRow["price"] = dr["price"];            
            tri1.Rows.Add(newRow);
        }
    }
}