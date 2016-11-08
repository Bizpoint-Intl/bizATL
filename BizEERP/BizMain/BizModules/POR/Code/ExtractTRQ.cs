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

namespace ATL.POR
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

            DataRow porh = this.dbAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = this.dbAccess.DataSet.Tables["por1"];

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

            if (!BizFunctions.IsEmpty(porh["ponum"]))
            {
                sql1 = "	Select a.trqnum,a.matnum,MAX(a.detail) as detail,CASE WHEN sum((a.tbaseqty-a.traqty)-a.pbaseqty)<=0 THEN 0 ELSE sum((a.tbaseqty-a.traqty)-a.pbaseqty) END as Outstanding,sum(a.tbaseqty) as MRQqty,sum(a.pbaseqty) as PORqty,sum(a.traqty) as MISqty, " +
                                " uom,whnum " +
                                "from " +
                                "( " +
                                "Select tq1.refnum as trqnum,tq1.matnum,MAX(tq1.detail) as detail,sum(tq1.approveqty) as tbaseqty,0 as pbaseqty,0 as traqty,tq1.uomcode as uom,tq1.whnum from trq1 tq1 LEFT JOIN trqh tq on tq1.refnum=tq.refnum where tq.[status]<>'V' and UPPER(tq.approvestatus)='YES' " +
                                "group by tq1.refnum,tq1.matnum,tq1.detail,tq1.uomcode,tq1.whnum " +

                                "UNION ALL " +
                                "Select trqnum,matnum,'' as detail,0 as tbaseqty, sum(qty) as pbaseqty,0 as traqty,uom,whnum from POR1 where [status]<>'V' " +
                                "group by trqnum,matnum,detail,uom,whnum " +

                                "UNION " +

                                "Select th.trqnum,t1.matnum,'' as detail,0 as tbaseqty, 0 as pbaseqty, sum(qty) as traqty, t1.uom,t1.towhnum as whnum from tra1 t1 left join trah th on t1.refnum=th.refnum where th.[status]<>'V' " +
                                "group by th.trqnum,t1.matnum,detail,t1.uom,t1.towhnum " +

                                ")a where trqnum='" + porh["ponum"].ToString() + "' " +

                                "group by a.trqnum,a.matnum,a.uom,a.whnum " +
                                "having sum((a.tbaseqty-a.traqty)-a.pbaseqty)>0";
            }
            else
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
                                "group by th.trqnum,t1.matnum,detail,t1.uom,t1.towhnum " +

                                ")a where trqnum is not null "+

                                "group by a.trqnum,a.matnum,a.uom,a.whnum " +
                                "having sum((a.tbaseqty-a.traqty)-a.pbaseqty)>0";
            }



            this.dbAccess.ReadSQL("POR1Lists", sql1);

            DataTable POR1Lists = this.dbAccess.DataSet.Tables["POR1Lists"];

            //string InsertIntoPor1 = "SELECT * FROM POR1Lists WHERE uniquekey not in (Select matnum+refnum+whnum as uniquekey from POR1)";
            //string InsertIntoPor2 = "Select refnum+matnum+whnum as uniquekey from POR1";

            //DataTable dtPOR1tmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntoPor1);
            //DataTable dtPOR1tmp2 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntoPor2);

            //if (this.dbAccess.DataSet.Tables.Contains("dtPOR1tmp"))
            //{
            //    this.dbAccess.DataSet.Tables["dtPOR1tmp"].Dispose();
            //    this.dbAccess.DataSet.Tables["dtPOR1tmp"].Clear();
            //}
            //else
            //{
            //    dtPOR1tmp.TableName = "dtPOR1tmp";
            //    this.dbAccess.DataSet.Tables.Add(dtPOR1tmp);
            //}



                dt_view = POR1Lists.Copy();
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
            DataRow porh = dbAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = dbAccess.DataSet.Tables["por1"];
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
                                DataRow por1tmp = dbAccess.DataSet.Tables["por1"].NewRow();
                                por1tmp["Mark"] = 0;
                                por1tmp["trqnum"] = row["trqnum"];
                                por1tmp["ponum"] = row["trqnum"];
                                por1tmp["matnum"] = row["matnum"];
                                por1tmp["detail"] = row["detail"];
                                por1tmp["uom"] = row["uom"];
                                por1tmp["qty"] = row["Outstanding"];
                                //por1tmp["pqty"] = Math.Abs(GetUomQty(row["matnum"].ToString()) / Convert.ToDecimal(row["Outstanding"]));
                                por1tmp["approveqty"] = row["mrqqty"];
                                por1tmp["whnum"] = row["whnum"];
                                por1tmp["trqnum"] = row["trqnum"];

                                bool takedefault = true;

                                if(!BizFunctions.IsEmpty(porh["apnum"]))
                                {
                                    string str1 = "Select top 1 * from matm2 where apnum='"+porh["apnum"].ToString() + "' and matnum='"+row["matnum"].ToString()+"' ";

                                    this.dbAccess.ReadSQL("TempMATM2", str1);

                                    DataTable TempMATM2 = this.dbAccess.DataSet.Tables["TempMATM2"];

                                    if (TempMATM2.Rows.Count < 1)
                                    {
                                        takedefault = true;
                                    } else
                                    {
                                        takedefault = false;
                                        por1tmp["uomqty"] = TempMATM2.Rows[0]["uomqty"];
                                        por1tmp["uomcode"] = TempMATM2.Rows[0]["uomcode"];
                                        por1tmp["pqty"] = Math.Abs(Convert.ToDecimal(row["Outstanding"]) / Convert.ToDecimal(por1tmp["uomqty"]));
                                    }
                                  
                                }

                                if (takedefault)
                                {
                                    string str2 = "select ISNULL(ploftcode,1) AS convertqty,uomcode,puom   from matm where matnum='" + row["matnum"].ToString() + "'";

                                    this.dbAccess.ReadSQL("TempMatm22", str2);

                                    DataTable TempMatm22 = this.dbAccess.DataSet.Tables["TempMatm22"];

                                    if (TempMatm22.Rows.Count > 0)
                                    {
                                        por1tmp["uomqty"] = TempMatm22.Rows[0]["convertqty"];
                                        por1tmp["uomcode"] = TempMatm22.Rows[0]["puom"];
                                        //por1tmp["pqty"] = Math.Abs(Convert.ToDecimal(por1tmp["uomqty"]) / Convert.ToDecimal(row["Outstanding"]));
                                        por1tmp["pqty"] = Math.Abs(Convert.ToDecimal(row["Outstanding"]) / Convert.ToDecimal(por1tmp["uomqty"]));
                                    }


                                }
                                //get the conversion factor
                                //if apnum is NotFiniteNumberException blank
                                //    select top 1 conversion, uom, purchaseprice From ChildHandle matm1 where apnum = porh.apnum and matnum
                                //    if no record found
                                //        takedefault = true


                                //if take default = true then
                                //    select From matm when matnum

                                //put convcersion qty
                                //calculate ourchase qty



                                por1.Rows.Add(por1tmp);

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


        private decimal GetUomQty(string matnum)
        {
            decimal UomQty = 0;

            string str = "Select uomqty from vAPMATM where matnum='"+matnum+"'";

            this.dbAccess.ReadSQL("TempVAPMATM", str);

            DataTable TempVAPMATM = this.dbAccess.DataSet.Tables["TempVAPMATM"];

            if (TempVAPMATM.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(TempVAPMATM.Rows[0]["uomqty"]))
                {
                    UomQty = Convert.ToDecimal(TempVAPMATM.Rows[0]["uomqty"]);
                }
            }

            return UomQty;
        }


     
    }
}