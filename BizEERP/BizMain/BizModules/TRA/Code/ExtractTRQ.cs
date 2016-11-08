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

namespace ATL.TRA
{
    public partial class ExtractTRQ: Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;


        public ExtractTRQ(DBAccess dbAccess, DataTable originalTable)
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

            DataRow trah = this.dbAccess.DataSet.Tables["trah"].Rows[0];
            DataTable tra1 = this.dbAccess.DataSet.Tables["tra1"];

            //string sql1 = " Select b.refnum,b.matnum,b.qty,b.uom,b.detail,b.whnum,b.uniquekey from " +
            //                "( "+
            //                    "Select ISNULL(a.refnum,'') AS refnum,matnum,sum(a.qty) AS qty,a.uom,a.detail,a.whnum, a.uniquekey from " +
            //                    "( "+
            //                        "Select refnum,matnum,approveqty as qty,uomcode as uom,detail,whnum, ISNULL(matnum,'')+ISNULL(refnum,'')+ISNULL(whnum,'') as [uniquekey] from TRQ1   " +
            //                    ")a "+
            //                    "where uniquekey not in "+
            //                    "( "+
            //                       " select ISNULL(matnum,'')+ISNULL(trqnum,'')+ISNULL(whnum,'') from por1 where trqnum is not null  "+
            //                    ") "+
            //                    "and a.uniquekey not in "+
            //                    "( "+
            //                        "select ISNULL(ta.matnum,'')+ISNULL(th.trqnum,'')+ISNULL(ta.tositenum,'') from tra1 ta left join trah th on ta.refnum=th.refnum where th.trqnum is not null and th.[status]<>'V' "+
            //                    ") "+
            //                    "group by refnum,matnum,uom,detail,whnum,uniquekey " +
            //                ")b";

            string sql1 = "";

            if (!BizFunctions.IsEmpty(trah["trqnum"]))
            {
                sql1 = "	Select a.trqnum,a.matnum,MAX(a.detail) as detail,CASE WHEN sum((a.tbaseqty-a.traqty)-a.pbaseqty)<=0 THEN 0 ELSE sum((a.tbaseqty-a.traqty)-a.pbaseqty) END as Outstanding,sum(a.tbaseqty) as MRQqty,sum(a.pbaseqty) as PORqty,sum(a.traqty) as MISqty, " +
                                " uom,whnum " +
                                "from " +
                                "( " +
                                "Select tq1.refnum as trqnum,tq1.matnum,MAX(tq1.detail) as detail,sum(tq1.approveqty) as tbaseqty,0 as pbaseqty,0 as traqty,tq1.uomcode as uom,tq1.whnum from trq1 tq1 LEFT JOIN trqh tq on tq1.refnum=tq.refnum where tq.[status]<>'V'  and UPPER(tq.approvestatus)='YES'  " +
                                "group by tq1.refnum,tq1.matnum,tq1.detail,tq1.uomcode,tq1.whnum " +

                                "UNION ALL " +
                                "Select trqnum,matnum,'' as detail,0 as tbaseqty, sum(qty) as pbaseqty,0 as traqty,uom,whnum from POR1 where [status]<>'V' " +
                                "group by trqnum,matnum,detail,uom,whnum " +

                                "UNION " +

                                "Select th.trqnum,t1.matnum,'' as detail,0 as tbaseqty, 0 as pbaseqty, sum(qty) as traqty, t1.uom,t1.towhnum as whnum from tra1 t1 left join trah th on t1.refnum=th.refnum where th.[status]<>'V' " +
                                "group by th.trqnum,t1.matnum,t1.detail,t1.uom,t1.towhnum " +

                                ")a where trqnum='" + trah["trqnum"].ToString() + "' " +

                                "group by a.trqnum,a.matnum,a.uom,a.whnum " +
                                "having sum((a.tbaseqty-a.traqty)-a.pbaseqty)>0";
            }
            else
            {
                sql1 = "	Select a.trqnum,a.matnum,MAX(a.detail) AS detail,CASE WHEN sum((a.tbaseqty-a.traqty)-a.pbaseqty)<=0 THEN 0 ELSE sum((a.tbaseqty-a.traqty)-a.pbaseqty) END as Outstanding,sum(a.tbaseqty) as MRQqty,sum(a.pbaseqty) as PORqty,sum(a.traqty) as MISqty, " +
                                " uom,whnum " +
                                "from " +
                                "( " +
                                "Select tq1.refnum as trqnum,tq1.matnum,MAX(tq1.detail) as detail,sum(tq1.approveqty) as tbaseqty,0 as pbaseqty,0 as traqty,tq1.uomcode as uom,tq1.whnum from trq1 tq1 LEFT JOIN trqh tq on tq1.refnum=tq.refnum where tq.[status]<>'V'  and UPPER(tq.approvestatus)='YES'  " +
                                "group by tq1.refnum,tq1.matnum,tq1.detail,tq1.uomcode,tq1.whnum " +

                                "UNION ALL " +
                                "Select trqnum,matnum,'' as detail,0 as tbaseqty, sum(qty) as pbaseqty,0 as traqty,uom,whnum from POR1 where [status]<>'V' " +
                                "group by trqnum,matnum,detail,uom,whnum " +

                                "UNION " +

                                "Select th.trqnum,t1.matnum,'' as detail,0 as tbaseqty, 0 as pbaseqty, sum(qty) as traqty, t1.uom,t1.towhnum as whnum from tra1 t1 left join trah th on t1.refnum=th.refnum where th.[status]<>'V' " +
                                "group by th.trqnum,t1.matnum,t1.uom,t1.towhnum " +

                                ")a where trqnum is not null "+

                                "group by a.trqnum,a.matnum,a.uom,a.whnum " +
                                "having sum((a.tbaseqty-a.traqty)-a.pbaseqty)>0";
            }



            this.dbAccess.ReadSQL("TRA1Lists", sql1);

            DataTable TRA1Lists = this.dbAccess.DataSet.Tables["TRA1Lists"];

            //string InsertIntoTRA1 = "SELECT * FROM TRA1Lists WHERE uniquekey not in (Select matnum+refnum+whnum as uniquekey from TRA1)";
            //string InsertIntoTRA2 = "Select refnum+matnum+whnum as uniquekey from TRA1";

            //DataTable dtTRA1tmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntoTRA1);
            //DataTable dtTRA1tmp2 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntoTRA2);

            //if (this.dbAccess.DataSet.Tables.Contains("dtTRA1tmp"))
            //{
            //    this.dbAccess.DataSet.Tables["dtTRA1tmp"].Dispose();
            //    this.dbAccess.DataSet.Tables["dtTRA1tmp"].Clear();
            //}
            //else
            //{
            //    dtTRA1tmp.TableName = "dtTRA1tmp";
            //    this.dbAccess.DataSet.Tables.Add(dtTRA1tmp);
            //}



                dt_view = TRA1Lists.Copy();
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
            DataTable tra1 = dbAccess.DataSet.Tables["tra1"];
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
                                DataRow tra1tmp = dbAccess.DataSet.Tables["tra1"].NewRow();
                                tra1tmp["Mark"] = 0;
                                tra1tmp["matnum"] = row["matnum"];
                                tra1tmp["detail"] = row["detail"];
                                tra1tmp["uom"] = row["uom"];
                                tra1tmp["qty"] = row["Outstanding"];
                                tra1tmp["traqty"] = row["MRQQty"]; //Approved qty
                                tra1tmp["whnum"] = row["whnum"];
                                tra1tmp["trqnum"] = row["trqnum"];

                                tra1.Rows.Add(tra1tmp);

                            }
                        }
                    }
                }

                MessageBox.Show("Items Extracted", "Extraction Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
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