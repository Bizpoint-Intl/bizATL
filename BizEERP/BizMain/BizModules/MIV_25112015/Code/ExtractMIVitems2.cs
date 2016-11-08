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

            DataRow pivh = this.dbAccess.DataSet.Tables["pivh"].Rows[0];
            DataTable piv1 = this.dbAccess.DataSet.Tables["piv1"];

            //string sql1 = " select ponum,matnum,uom,detail,discamt,SUM(pqty) as pqty,SUM(gqty) as gqty, sum(pqty-gqty) as OutstandingQty,whnum "+
            //                "from "+
            //                "( "+
            //                "select refnum as ponum,matnum,uom,detail,discamt,sum(qty) as pqty,0 as gqty,price,whnum from por1  "+
            //                                //"--where --apnum = ' ' and  " +
            //                                //    "--type='LOCAL'  " +
            //                                    "group by refnum,matnum,uom,detail,discamt,price,whnum " +
            //                "UNION ALL "+
            //                "select ponum,matnum,uom,detail,discamt,0 as pqty,sum(qty) as gqty,price,whnum from piv1  " +
            //                                //"--where --apnum = ' ' and  "+
            //                                //    "--type='LOCAL'  "+
            //                                    "group by ponum,matnum,uom,detail,discamt,price,whnum) a " +
            //                "group by ponum,matnum,uom,detail,discamt,price,whnum " +
            //                "having SUM(pqty-gqty)>0";

            string sql1 = "select docunum,matnum,uom,uomcode,MAX(detail) as detail,discamt,SUM(pqty) as pqty,SUM(gqty) as gqty,max(uomqty) as uomqty, "+
                            "max(price) as price,sum(gqty-pqty) as OutstandingQty,whnum,trandate   " +
                           "from    "+
                            "(  "+
                                "select refnum as docunum,matnum,uom,uomcode,detail,discamt,sum(pqty) as gqty,0 as pqty, uomqty,  price, whnum,trandate from grn1 where [status]<>'V' and trandate>='"+BizFunctions.GetSafeDateString(Convert.ToDateTime(pivh["dtrandateFrom"]))+"' and trandate<='"+BizFunctions.GetSafeDateString(Convert.ToDateTime(pivh["dtrandateTo"]))+"'    " +
                                "group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum,price,uomqty,trandate    " +
			                    "UNION ALL    "+
                                "select docunum,matnum,uom,uomcode,'' as detail,discamt,0 as gqty,sum(qty) as pqty, 0 as uomqty, 0 as price, whnum,trandate from piv1 where [status]<>'V' and trandate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(pivh["dtrandateFrom"])) + "' and trandate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(pivh["dtrandateTo"])) + "'     " +
                                "group by docunum,matnum,uom,uomcode,discamt,price,whnum,trandate " +
                            ") a    "+
                            "group by docunum,matnum,uom,uomcode,discamt,whnum,trandate  " +
                            "having SUM(gqty-pqty)>0;";
                
                
                
                //"select ponum,matnum,uom,uomcode,detail,discamt,SUM(pqty) as pqty,SUM(gqty) as gqty,SUM(puomqty) as puomqty,SUM(guomqty) as guomqty,SUM(pprice) as pprice,SUM(gprice) as gprice, sum(pqty-gqty) as OutstandingQty,whnum "+
                //            "from  "+
                //            "(  "+



                //                "select refnum as ponum,matnum,uom,uomcode,detail,discamt,sum(pqty) as pqty,0 as gqty, SUM(uomqty) as puomqty,0 as guomqty, sum(price) as pprice, 0 as gprice,whnum from por1  "+
                //                "group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum  "+
                                
	                  							
                //                "UNION ALL  "+

                //                "select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,sum(pqty) as gqty, 0 as puomqty,sum(qty) as guomqty, 0 as pprice, sum(price) as gprice, whnum from piv1    " +
                //                "group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum   "+


                //                ////-- get qty
                //                //"select refnum as ponum,matnum,uom,uomcode,detail,discamt,sum(pqty) as pqty,0 as gqty, 0 as puomqty,0 as guomqty, 0 as pprice, 0 as gprice,whnum from por1  "+
                //                //"group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum  " +
                    							
                //                //"UNION ALL  "+
                //                ////-- get uomqty
                //                //"select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,sum(pqty) as gqty, 0 as puomqty,0 as guomqty, 0 as pprice, 0 as gprice,whnum from piv1  "+
                //                //"group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum "+
                    			
                //                //"UNION ALL "+
                    			
                //                ////-- get qty
                //                //"select refnum as ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, SUM(uomqty) as puomqty,0 as guomqty, 0 as pprice, 0 as gprice ,whnum from por1   "+
                //                //"group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum  "+ 
                    							
                //                //"UNION ALL  "+
                //                ////-- get uomqty
                //                //"select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, 0 as puomqty, sum(uomqty) as guomqty, 0 as pprice, 0 as gprice,whnum from piv1  "+
                //                //"group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum "+
                    			
                //                //"UNION ALL "+
                    			
                //                ////-- get por1 price
                //                //"select refnum as ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, SUM(uomqty) as puomqty,0 as guomqty, sum(price) as pprice, 0 as gprice,whnum from por1   "+
                //                //"group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum   "+
                    							
                //                //"UNION ALL  "+
                //                ////-- get piv1 price
                //                //"select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, 0 as puomqty, sum(uomqty) as guomqty, 0 as pprice, sum(price) as gprice,whnum from piv1   "+
                //                //"group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum "+
                    			
                //            ") a  "+ 
                //            "group by ponum,matnum,uom,uomcode,detail,discamt,whnum   "+
                //            "having SUM(pqty-gqty)>0";

            this.dbAccess.ReadSQL("piv1Lists", sql1);

            DataTable piv1Lists = this.dbAccess.DataSet.Tables["piv1Lists"];

            //string InsertIntoPor1 = "SELECT * FROM piv1Lists WHERE uniquekey not in (Select refnum+matnum+whnum as uniquekey from piv1)";

            //DataTable dtpiv1tmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntoPor1);

            //if (this.dbAccess.DataSet.Tables.Contains("dtpiv1tmp"))
            //{
            //    this.dbAccess.DataSet.Tables["dtpiv1tmp"].Dispose();
            //    this.dbAccess.DataSet.Tables["dtpiv1tmp"].Clear();
            //}
            //else
            //{
            //    dtpiv1tmp.TableName = "dtpiv1tmp";
            //    this.dbAccess.DataSet.Tables.Add(dtpiv1tmp);
            //}

          

            //    dt_view = dtpiv1tmp.Copy();




                dt_view = piv1Lists.Copy();
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
            DataTable piv1 = dbAccess.DataSet.Tables["piv1"];
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
                                DataRow piv1tmp = dbAccess.DataSet.Tables["piv1"].NewRow();
                                piv1tmp["Mark"] = 0;
                                piv1tmp["docunum"] = row["docunum"];
                                piv1tmp["docunum3"] = row["docunum"];
                                piv1tmp["matnum"] = row["matnum"];
                                piv1tmp["detail"] = row["detail"];
                                piv1tmp["uom"] = row["uom"];
                                piv1tmp["uomcode"] = row["uomcode"];
                                piv1tmp["qty"] = row["OutstandingQty"];
                                piv1tmp["dqty"] = row["OutstandingQty"];
                                piv1tmp["whnum"] = row["whnum"];
                                //piv1tmp["sitenum"] = row["sitenum"];
                                //piv1tmp["grnum"] = row["grnum"];
                                piv1tmp["price"] = row["price"];

                                piv1.Rows.Add(piv1tmp);

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