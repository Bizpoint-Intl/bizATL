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

namespace ATL.PON
{
    public partial class ExtractPORForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable trr1;
        public ExtractPORForm(DBAccess dbAccess, DataTable originalTable)
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

            DataRow ponh = this.dbAccess.DataSet.Tables["ponh"].Rows[0];
            //string sql = "select *from(SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
            //         "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 where apnum='" + ponh["apnum"].ToString() + "' and type='OVERSEA' UNION ALL" +
            //         " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price from pon1" +
            //         " WHERE refnum <>'" + ponh["refnum"].ToString().Trim() + "' and ponum!=pon1.ponum)a" +
            //         " GROUP BY ponum,matnum,uom,discamt,detail" +
            //         " HAVING SUM(qty) >0)tmp1";

            string sql = "select * from(select por1.refnum as ponum,por1.matnum,por1.uom,por1.discamt,por1.detail,por1.price," +       
                        "case when (ISNULL(por1.qty,0) - ISNULL(pon1.qty,0))< 0 then 0 else (ISNULL(por1.qty,0) - ISNULL(pon1.qty,0)) end as qty "+
                        "from (select refnum,matnum,uom,detail,discamt,sum(qty) as qty,price from por1 " +
                        "where status='P' and apnum = '" + ponh["apnum"].ToString() + "' and type='OVERSEA' group by refnum,matnum,uom,detail,discamt,price)por1 " +
                        "left join (select ponum,matnum,sum(qty) as qty,price from pon1 where isnull(status,'')<>'V' group by ponum,matnum,price)pon1 " +
                        "on por1.refnum=pon1.ponum and por1.matnum= pon1.matnum and por1.price=pon1.price where (por1.qty-ISNULL(pon1.qty,0)) > 0)tmp1";
            if (!BizFunctions.IsEmpty(ponh["ponum"]))
            {
                sql += " where tmp1.ponum ='" + ponh["ponum"].ToString() + "'";
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
                            DataRow pon1tmp = dbAccess.DataSet.Tables["pon1"].NewRow();
                            foreach (DataColumn dc in dt_view.Columns)
                            {

                                if (dbAccess.DataSet.Tables["pon1"].Columns.IndexOf(dc.ColumnName) != -1)
                                {
                                    pon1tmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
                            pon1tmp["Mark"] = 0;
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
            DataTable pon1 = dbAccess.DataSet.Tables["pon1"];
            DataRow newRow = pon1.NewRow();
            newRow["ponum"] = dr["ponum"];
            newRow["matnum"] = dr["matnum"];
            newRow["detail"] = dr["detail"];
            newRow["uom"] = dr["uom"];
            newRow["discamt"] = dr["discamt"];
            newRow["qty"] = dr["qty"];
            newRow["price"] = dr["price"];
            pon1.Rows.Add(newRow);
        }
    }
}