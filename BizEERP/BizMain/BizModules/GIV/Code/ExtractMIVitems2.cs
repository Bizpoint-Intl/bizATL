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
using ATL.BizLogicTools;
using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;
using BizRAD.BizCommon;
using System.Configuration;
using ATL.TimeUtilites;
using NodaTime;

namespace ATL.PIV
{
    public partial class ExtractMIVitems2: Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;


        public ExtractMIVitems2(DBAccess dbAccess, DataTable originalTable)
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

            DataRow givh = this.dbAccess.DataSet.Tables["givh"].Rows[0];
            DataTable giv1 = this.dbAccess.DataSet.Tables["giv1"];          

            string sql1 = "select docunum,matnum,uom,uomcode,MAX(detail) as detail,discamt,SUM(pqty) as pqty,SUM(gqty) as gqty,max(uomqty) as uomqty, "+
                            "max(price) as price,sum(gqty-pqty) as OutstandingQty,whnum,ccnum,projectid   " +
                           "from    "+
                            "(  "+
                                "select g1.refnum as docunum,g1.matnum,g1.uom,g1.uomcode,g1.detail,g1.discamt,sum(g1.pqty) as gqty,0 as pqty, g1.uomqty,  g1.price, g1.whnum,g1.ccnum,g1.projectid  from grn1 g1 left join grnh gh on g1.refnum=gh.refnum " +
                                "where g1.[status]<>'V' and g1.trandate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(givh["dtrandateFrom"])) + "' and g1.trandate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(givh["dtrandateTo"])) + "'  and gh.[type]='SITE'   " +
                                "group by g1.refnum,g1.matnum,g1.uom,g1.uomcode,g1.detail,g1.discamt,g1.pqty,g1.whnum,g1.ccnum,g1.projectid,g1.price,g1.uomqty   " +
			                    "UNION ALL    "+
                                "select docunum3 as docunum,matnum,uom,uomcode,'' as detail,discamt,0 as gqty,sum(qty) as pqty, 0 as uomqty, 0 as price, whnum,ccnum,projectid " +
                                "from giv1 where [status]<>'V' and trandate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(givh["dtrandateFrom"])) + "' and trandate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(givh["dtrandateTo"])) + "' " +
                                "group by docunum3,matnum,uom,uomcode,discamt,price,whnum,ccnum,projectid " +
                                "UNION ALL " +       
                                  "select docunum,matnum,uom,uomcode,detail,discamt, gqty as qqty,0 pqty,uomqty,price, whnum,ccnum,projectid      "+
		                            "from      "+
		                            "(      "+
			                            "select refnum as docunum,matnum,uom,uomcode,detail,0 as discamt,sum(qty) as gqty, 0 as pqty, 0 as uomqty, SUM(ISNULL(retail,0)) as price, whnum,ccnum,projectid from tri1      "+
                                        "where [status]='P' and trandate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(givh["dtrandateFrom"])) + "' and trandate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(givh["dtrandateTo"])) + "'         " +
			                            "group by refnum,matnum,uom,uomcode,detail,retail,whnum,ccnum,projectid       "+
                                    ")c      " +
                            ") a    "+
                            "group by docunum,matnum,uom,uomcode,discamt,whnum,ccnum,projectid " +
                            "having SUM(gqty-pqty)>0;";
                
           

            this.dbAccess.ReadSQL("giv1Lists", sql1);

            DataTable giv1Lists = this.dbAccess.DataSet.Tables["giv1Lists"];
                dt_view = giv1Lists.Copy();
                dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));

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
            DataTable giv1 = dbAccess.DataSet.Tables["giv1"];
            int count = 0;
            try
            {
                foreach (DataRow row in this.dt_view.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        if (row["Mark"] != System.DBNull.Value)
                        {
                            if ((bool)row["Mark"] == true)
                            {
                                DataRow giv1tmp = dbAccess.DataSet.Tables["giv1"].NewRow();
                                giv1tmp["Mark"] = 0;
                                giv1tmp["docunum"] = row["docunum"];
                                giv1tmp["docunum3"] = row["docunum"];
                                giv1tmp["matnum"] = row["matnum"];
                                giv1tmp["detail"] = row["detail"];
                                giv1tmp["uom"] = row["uom"];
                                giv1tmp["uomcode"] = row["uomcode"];
                                giv1tmp["qty"] = row["OutstandingQty"];
                                giv1tmp["dqty"] = row["OutstandingQty"];
                                giv1tmp["whnum"] = row["whnum"];
                                //giv1tmp["sitenum"] = row["sitenum"];
                                //giv1tmp["grnum"] = row["grnum"];
                                giv1tmp["price"] = row["price"];
                                giv1tmp["ccnum"] = row["ccnum"];

                                if (row["ccnum"].ToString().Trim().ToUpper() == "CLN")
                                {
                                    //giv1tmp["accnum"] = "50001";
                                }
                                else if (row["ccnum"].ToString().Trim().ToUpper() == "PSC")
                                {
                                    //giv1tmp["accnum"] = "50002";
                                }
                                giv1tmp["sitenum"] = BizLogicTools.Tools.GetSitenum(row["whnum"].ToString(), this.dbAccess);
                                giv1tmp["projectid"] = row["projectid"];
                              

                                giv1.Rows.Add(giv1tmp);

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


     
    }
}