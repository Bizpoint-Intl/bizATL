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

namespace ATL.GRN
{
    public partial class ExtractPOR: Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;


        public ExtractPOR(DBAccess dbAccess, DataTable originalTable)
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

            DataRow grnh = this.dbAccess.DataSet.Tables["grnh"].Rows[0];
            DataTable grn1 = this.dbAccess.DataSet.Tables["grn1"];

            //string sql1 = " select ponum,matnum,uom,detail,discamt,SUM(pqty) as pqty,SUM(gqty) as gqty, sum(pqty-gqty) as OutstandingQty,whnum "+
            //                "from "+
            //                "( "+
            //                "select refnum as ponum,matnum,uom,detail,discamt,sum(qty) as pqty,0 as gqty,price,whnum from por1  "+
            //                                //"--where --apnum = ' ' and  " +
            //                                //    "--type='LOCAL'  " +
            //                                    "group by refnum,matnum,uom,detail,discamt,price,whnum " +
            //                "UNION ALL "+
            //                "select ponum,matnum,uom,detail,discamt,0 as pqty,sum(qty) as gqty,price,whnum from grn1  " +
            //                                //"--where --apnum = ' ' and  "+
            //                                //    "--type='LOCAL'  "+
            //                                    "group by ponum,matnum,uom,detail,discamt,price,whnum) a " +
            //                "group by ponum,matnum,uom,detail,discamt,price,whnum " +
            //                "having SUM(pqty-gqty)>0";

            string sql1 = "select ponum,matnum,uom,uomcode,MAX(detail) as detail,discamt,SUM(pqty) as pqty,SUM(gqty) as gqty,max(uomqty) as uomqty, "+
                "max(price) as price,sum(pqty-gqty) as OutstandingQty,whnum  "+
                "           from   "+
                "            (   "+
		    	"                    select refnum as ponum,matnum,uom,uomcode,detail,discamt,sum(pqty) as pqty,0 as gqty, uomqty,  price, whnum from por1 "+
			    "                group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum,price,uomqty   "+
			    "                UNION ALL   "+
			    "                select ponum,matnum,uom,uomcode,'' as detail,discamt,0 as pqty,sum(pqty) as gqty, 0 as uomqty, 0 as price, whnum from grn1   "+
			    "                group by ponum,matnum,uom,uomcode,discamt,price,whnum"+
                "            ) a  where ponum='" + grnh["ponum"].ToString().Trim() + "'  " +
                "            group by ponum,matnum,uom,uomcode,discamt,whnum "+
                "            having SUM(pqty-gqty)>0";
                
                
                
                //"select ponum,matnum,uom,uomcode,detail,discamt,SUM(pqty) as pqty,SUM(gqty) as gqty,SUM(puomqty) as puomqty,SUM(guomqty) as guomqty,SUM(pprice) as pprice,SUM(gprice) as gprice, sum(pqty-gqty) as OutstandingQty,whnum "+
                //            "from  "+
                //            "(  "+



                //                "select refnum as ponum,matnum,uom,uomcode,detail,discamt,sum(pqty) as pqty,0 as gqty, SUM(uomqty) as puomqty,0 as guomqty, sum(price) as pprice, 0 as gprice,whnum from por1  "+
                //                "group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum  "+
                                
	                  							
                //                "UNION ALL  "+

                //                "select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,sum(pqty) as gqty, 0 as puomqty,sum(qty) as guomqty, 0 as pprice, sum(price) as gprice, whnum from grn1    " +
                //                "group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum   "+


                //                ////-- get qty
                //                //"select refnum as ponum,matnum,uom,uomcode,detail,discamt,sum(pqty) as pqty,0 as gqty, 0 as puomqty,0 as guomqty, 0 as pprice, 0 as gprice,whnum from por1  "+
                //                //"group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum  " +
                    							
                //                //"UNION ALL  "+
                //                ////-- get uomqty
                //                //"select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,sum(pqty) as gqty, 0 as puomqty,0 as guomqty, 0 as pprice, 0 as gprice,whnum from grn1  "+
                //                //"group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum "+
                    			
                //                //"UNION ALL "+
                    			
                //                ////-- get qty
                //                //"select refnum as ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, SUM(uomqty) as puomqty,0 as guomqty, 0 as pprice, 0 as gprice ,whnum from por1   "+
                //                //"group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum  "+ 
                    							
                //                //"UNION ALL  "+
                //                ////-- get uomqty
                //                //"select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, 0 as puomqty, sum(uomqty) as guomqty, 0 as pprice, 0 as gprice,whnum from grn1  "+
                //                //"group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum "+
                    			
                //                //"UNION ALL "+
                    			
                //                ////-- get por1 price
                //                //"select refnum as ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, SUM(uomqty) as puomqty,0 as guomqty, sum(price) as pprice, 0 as gprice,whnum from por1   "+
                //                //"group by refnum,matnum,uom,uomcode,detail,discamt,price,whnum   "+
                    							
                //                //"UNION ALL  "+
                //                ////-- get grn1 price
                //                //"select ponum,matnum,uom,uomcode,detail,discamt,0 as pqty,0 as gqty, 0 as puomqty, sum(uomqty) as guomqty, 0 as pprice, sum(price) as gprice,whnum from grn1   "+
                //                //"group by ponum,matnum,uom,uomcode,detail,discamt,price,whnum "+
                    			
                //            ") a  "+ 
                //            "group by ponum,matnum,uom,uomcode,detail,discamt,whnum   "+
                //            "having SUM(pqty-gqty)>0";

            this.dbAccess.ReadSQL("GRN1Lists", sql1);

            DataTable GRN1Lists = this.dbAccess.DataSet.Tables["GRN1Lists"];

            //string InsertIntoPor1 = "SELECT * FROM GRN1Lists WHERE uniquekey not in (Select refnum+matnum+whnum as uniquekey from GRN1)";

            //DataTable dtGRN1tmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntoPor1);

            //if (this.dbAccess.DataSet.Tables.Contains("dtGRN1tmp"))
            //{
            //    this.dbAccess.DataSet.Tables["dtGRN1tmp"].Dispose();
            //    this.dbAccess.DataSet.Tables["dtGRN1tmp"].Clear();
            //}
            //else
            //{
            //    dtGRN1tmp.TableName = "dtGRN1tmp";
            //    this.dbAccess.DataSet.Tables.Add(dtGRN1tmp);
            //}

          

            //    dt_view = dtGRN1tmp.Copy();




                dt_view = GRN1Lists.Copy();
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
            DataTable grn1 = dbAccess.DataSet.Tables["grn1"];
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
                                //Select b.refnum,b.matnum,b.qty,b.uom,b.detail from "+
                                DataRow grn1tmp = dbAccess.DataSet.Tables["grn1"].NewRow();
                                grn1tmp["Mark"] = 0;
                                grn1tmp["matnum"] = row["matnum"];
                                grn1tmp["detail"] = row["detail"];
                                grn1tmp["uom"] = row["uom"];
                                //grn1tmp["uomcode"] = BizLogicTools.Tools.GetMatnumPUOM(row["matnum"].ToString(),this.dbAccess);
                                grn1tmp["uomcode"] = row["uomcode"];
                                grn1tmp["pqty"] = row["OutstandingQty"];
                                grn1tmp["uomqty"] = row["uomqty"];
                                grn1tmp["qty"] = Convert.ToDecimal(row["uomqty"]) * Convert.ToDecimal(row["OutstandingQty"]);
                                grn1tmp["whnum"] = row["whnum"];
                                grn1tmp["ponum"] = row["ponum"];
                                grn1tmp["price"] = row["price"];

                                grn1.Rows.Add(grn1tmp);

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