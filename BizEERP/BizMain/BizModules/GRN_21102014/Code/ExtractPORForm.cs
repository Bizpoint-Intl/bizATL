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

namespace ATL.GRN
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

            DataRow grnh = this.dbAccess.DataSet.Tables["grnh"].Rows[0];
            //string sql = "select *from(SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
            //         "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 where apnum='" + grnh["apnum"].ToString() + "' and type='LOCAL' UNION ALL" +
            //         " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price from grn1" +
            //         " WHERE refnum <>'" + grnh["refnum"].ToString().Trim() + "' and ponum!=grn1.ponum)a" +
            //         " GROUP BY ponum,matnum,uom,discamt,detail" +
            //         " HAVING SUM(qty) >0)tmp1";

            //string sql ="select * from(select por1.refnum as ponum,por1.matnum,por1.uom,por1.discamt,por1.detail,por1.price,"+       
            //            "case when (ISNULL(por1.qty,0) - ISNULL(grn1.qty,0))< 0 then 0 else (ISNULL(por1.qty,0) - ISNULL(grn1.qty,0)) end as qty "+
            //            "from (select refnum,matnum,uom,detail,discamt,sum(qty) as qty,sum(price)as price from por1 where status='P' and apnum = '" + grnh["apnum"].ToString() + "' and type='LOCAL' group by refnum,matnum,uom,detail,discamt)por1 " +
            //            "left join (select ponum,matnum,sum(qty) as qty,sum(price)as price from grn1 where isnull(status,'')<>'V' group by ponum,matnum)grn1 "+
            //            "on por1.refnum=grn1.ponum and por1.matnum= grn1.matnum where (por1.qty-ISNULL(grn1.qty,0)) > 0)tmp1";

            //Yushu Modified20100329-Do not sum price because there is special scenarios in ATL where there will be same items of different pricing
            string sql = "select * from(select por1.refnum as ponum,por1.matnum,por1.uom,por1.discamt,por1.detail,por1.price," +
            "case when (ISNULL(por1.qty,0) - ISNULL(grn1.qty,0))< 0 then 0 else (ISNULL(por1.qty,0) - ISNULL(grn1.qty,0)) end as qty " +
            "from (select refnum,matnum,uom,detail,discamt,sum(qty) as qty,price from por1 where status='P' and apnum = '" + grnh["apnum"].ToString() 
            + "' and type='LOCAL' group by refnum,matnum,uom,detail,discamt,price)por1 " +
            "left join (select ponum,matnum,sum(qty) as qty,detail,sum(price)as price from grn1 where isnull(status,'')<>'V' group by ponum,matnum,detail)grn1 " +
            "on por1.refnum=grn1.ponum and por1.matnum= grn1.matnum and por1.detail=grn1.detail where (por1.qty-ISNULL(grn1.qty,0)) > 0)tmp1";

            if (!BizFunctions.IsEmpty(grnh["ponum"]))
            {
                sql += " where tmp1.ponum ='" + grnh["ponum"].ToString() + "'";
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
                            DataRow grn1tmp = dbAccess.DataSet.Tables["grn1"].NewRow();
                            foreach (DataColumn dc in dt_view.Columns)
                            {

                                if (dbAccess.DataSet.Tables["grn1"].Columns.IndexOf(dc.ColumnName) != -1)
                                {
                                    grn1tmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
                            grn1tmp["Mark"] = 0;
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
            DataTable grn1 = dbAccess.DataSet.Tables["grn1"];
            DataRow newRow = grn1.NewRow();
            newRow["ponum"] = dr["ponum"];
            newRow["matnum"] = dr["matnum"];
            newRow["detail"] = dr["detail"];
            newRow["uom"] = dr["uom"];
            newRow["discamt"] = dr["discamt"];
            newRow["qty"] = dr["qty"];
            newRow["price"] = dr["price"];
            grn1.Rows.Add(newRow);
        }
    }
}