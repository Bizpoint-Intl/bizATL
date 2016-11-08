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

namespace ATL.SCR
{
    public partial class ExtractSIVForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable trr1;
        public ExtractSIVForm(DBAccess dbAccess, DataTable originalTable)
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

			DataRow sivh = this.dbAccess.DataSet.Tables["sivh"].Rows[0];
            string sql = "select * from "+
                         "( "+
                         "SELECT refnum,arnum,invnum,trandate,flag,(Select top 1 projectid from SIV1 where siv1.refnum=sivh.refnum and siv1.projectid is not null) as projectid FROM sivh where invnum not in (Select docunum as invnum from sivh where flag='SCR' and [status]<>'V') and [status]<>'V' and flag = 'SIV' " +
                         "UNION "+
                         "SELECT refnum,arnum,invnum,trandate,flag,projectid FROM ard"+Common.DEFAULT_SYSTEM_YEAR+" where invnum not in (Select docunum as invnum from sivh where flag='SCR' and [status]<>'V') and [status]<>'V' "+
                         ")A ";

            //string sql = "SELECT refnum,arnum,invnum,matnum,uom,discamt,detail,price,dqty FROM siv1 ";



            if (!BizFunctions.IsEmpty(sivh["arnum"]) && !BizFunctions.IsEmpty(sivh["projectid"]))
            {
                sql += "  where arnum ='" + sivh["arnum"].ToString() + "' and projectid='" + sivh["projectid"].ToString() + "' ";
                //sql += "  where flag = 'SIV' and arnum ='" + sivh["arnum"].ToString() + "'";
            }
            else if (!BizFunctions.IsEmpty(sivh["arnum"]) && BizFunctions.IsEmpty(sivh["projectid"]))
            {
                sql += "  where arnum ='" + sivh["arnum"].ToString() + "'  ";
                //sql += "  where flag = 'SIV' and arnum ='" + sivh["arnum"].ToString() + "'";
            }
          
            ds = dbAccess.ReadSQLTemp("tmp", sql);
            DataTable dt=ds.Tables["tmp"];

            dt_view = dt.Copy();
            dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
            dt_view.Columns[4].DefaultValue = 0;
            
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
                            DataRow siv1tmp = dbAccess.DataSet.Tables["siv1"].NewRow();
                            foreach (DataColumn dc in dt_view.Columns)
                            {

                                if (dbAccess.DataSet.Tables["siv1"].Columns.IndexOf(dc.ColumnName) != -1)
                                {
                                    siv1tmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
                            siv1tmp["Mark"] = 0;

                            //NewRow(row);

                            //string sql = "SELECT refnum,arnum,invnum,matnum,uom,discamt,detail,price,dqty FROM siv1 where refnum='" + row["refnum"].ToString() + "' ";

                            string sql = "SELECT refnum,arnum,invnum,MAX(matnum) as matnum,MAX(uom) as uom,SUM(discamt) as discamt,detail,SUM(price) as price,SUM(dqty) as dqty FROM "+
                                         "(  "+
                                         "SELECT refnum,arnum,invnum,matnum,uom,discamt,detail,price,dqty FROM siv1 where refnum='" + row["refnum"].ToString() + "'   and invnum='" + row["invnum"].ToString() + "' " +
                                         "UNION  "+
                                         "select refnum,arnum,invnum,'' as matnum,'' as uom,0 as discamt,detail,postamt as price,1 as qty from ard"+Common.DEFAULT_SYSTEM_YEAR+" where refnum='" + row["refnum"].ToString() + "' and invnum='" + row["invnum"].ToString() + "' " +
                                         ")A group by refnum,arnum,invnum, detail ";

                            this.dbAccess.ReadSQL("tmp2", sql);

                            DataTable dt = this.dbAccess.DataSet.Tables["tmp2"];

                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in dt.Rows)
                                {
                                    NewRow(dr1);
                                }
                            }
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
            DataTable siv1 = dbAccess.DataSet.Tables["siv1"];
            DataRow newRow = siv1.NewRow();
			newRow["invnum"] = dr["invnum"];
            newRow["matnum"] = dr["matnum"];
            newRow["detail"] = dr["detail"];
            newRow["uom"] = dr["uom"];
            //newRow["shiftcode"] = dr["shiftcode"];
            //newRow["timein"] = dr["timein"];
            //newRow["timeout"] = dr["timeout"];
            newRow["discamt"] = dr["discamt"];
            newRow["dqty"] = dr["dqty"];
            newRow["price"] = dr["price"];
            //newRow["remark"] = dr["remark"];
            siv1.Rows.Add(newRow);
        }
    }
}