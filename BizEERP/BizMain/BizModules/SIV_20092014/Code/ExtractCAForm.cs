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

namespace ATL.SIV
{
    public partial class ExtractCAForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable siv1;
        //string posid = System.Configuration.ConfigurationManager.AppSettings.Get("POSID");

        string sitenum = null;
        Hashtable remoteHash = null;
        DBAccess remoteDBAccess = null;

        public ExtractCAForm(DBAccess dbAccess, DataTable originalTable)
        {
            this.dbAccess = dbAccess;
            this.oriTable = originalTable;

           // #region get sitenum
           // string command = "select sitenum from posm where posnum = '" + posid + "'";
           //this.dbAccess.ReadSQL("posm", command);
           // DataRow posm =this.dbAccess.DataSet.Tables["posm"].Rows[0];
           // sitenum = posm["sitenum"].ToString();
           // #endregion

           // #region Get connections
           // string standardstring = ConfigurationManager.AppSettings.Get("StandardSQLString");
           // string specialstring = ConfigurationManager.AppSettings.Get("SpecialSQLString");
           // string dataaccessstring = ConfigurationManager.AppSettings.Get("DataAccessString");

           // if (Tools.isFrontEnd())
           // {
           //     remoteHash = new Hashtable();
           //     remoteHash.Add("StandardSQL", standardstring);
           //     remoteHash.Add("SpecialSQL", specialstring);
           //     remoteHash.Add("DataAccess", dataaccessstring);
           //     remoteDBAccess = new DBAccess(remoteHash);
           // }
           // #endregion

            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshDatagrid()
        {
          
            DataRow sivh = this.dbAccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = this.dbAccess.DataSet.Tables["siv1"];


            string sql1 = "SELECT C.refnum,C.arnum,A.arname, C.sitenum, ISNULL(convert(varchar,C.commencedate, 103),'') as commencedate, ISNULL(convert(varchar,C.enddate, 103),'') as enddate,totalamt FROM " +
                            "( "+
                            "select refnum,arnum,sitenum,commencedate,enddate,ttlbillingamt as totalamt from CTRH WHERE [status]<>'V' AND arnum='" + sivh["arnum"].ToString() + "'  " +
                            "union all "+
                            "select refnum,arnum,sitenum,commencedate,enddate,ttlbillingamt as totalamt from ADH WHERE [status]<>'V' AND arnum='" + sivh["arnum"].ToString() + "' " +
                            ")C  "+
                            "LEFT JOIN ARM A on C.arnum=A.arnum"; 

            this.dbAccess.ReadSQL("ContractALL", sql1);

            DataTable ContractAll = this.dbAccess.DataSet.Tables["ContractALL"];

            if (ContractAll.Rows.Count > 0)
            {               

                    dt_view = ContractAll.Copy();
                    dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
                    dt_view.Columns[5].DefaultValue = 0;

                    this.dgView1.DataSource = dt_view;
                
            }
                   
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
            DataRow sivh = this.dbAccess.DataSet.Tables["sivh"].Rows[0];

            try
            {
                foreach (DataRow row in this.dt_view.Rows)
                {
                    if (row["Mark"] != System.DBNull.Value)
                    {
                        if ((bool)row["Mark"] == true)
                        {
                            sivh["sitenum"] = row["sitenum"];
                            DataRow tra1tmp = dbAccess.DataSet.Tables["siv1"].NewRow();
                            foreach (DataColumn dc in dt_view.Columns)
                            {

                                if (dbAccess.DataSet.Tables["siv1"].Columns.IndexOf(dc.ColumnName) != -1)
                                {
                                    tra1tmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
                            tra1tmp["Mark"] = 0;
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
            DataRow sivh = dbAccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = dbAccess.DataSet.Tables["siv1"];
            string sql1 = "";
            if (dr["refnum"].ToString().Contains("ADH"))
            {
                sql1 = " SELECT uniquekey,docunum,arnum,matnum,matname,shiftcode,sitenum,sectorcode,sum(officerqty)as officerqty,rate,remark   FROM " +
                               "( " +
                               "select  " +
                                   "REPLACE(c1.refnum+''+ch.arnum+''+c1.matnum+''+c1.shiftcode+''+c1.sectorcode+''+ " +
                                   "CONVERT(nvarchar(1),(CONVERT(int,ISNULL(c1.monday,0))+CONVERT(int,ISNULL(c1.tuesday,0))+CONVERT(int,ISNULL(c1.wednesday,0))+CONVERT(int,ISNULL(c1.thursday,0))+CONVERT(int,ISNULL(c1.friday,0))+CONVERT(int,ISNULL(c1.saturday,0))+CONVERT(int,ISNULL(c1.sunday,0))+CONVERT(int,ISNULL(c1.ispubhol,0)))),' ','') as uniquekey,  " +
                                   "c1.refnum as docunum,ch.arnum, c1.matnum ,c1.matname,c1.shiftcode,c1.sitenum,c1.sectorcode,c1.officerqty,c1.rate,c1.remark  " +
                                    "from CTR1 c1 LEFT JOIN CTRH ch on c1.refnum=ch.refnum where ch.[status]<>'V' and ch.refnum='" + dr["refnum"].ToString() + "'  " +

                                   "union all " +

                               "select " +
                                   "REPLACE(a1.refnum+''+ah.arnum+''+a1.matnum+''+a1.shiftcode+''+a1.sectorcode+''+ " +
                                   "CONVERT(nvarchar(1),(CONVERT(int,ISNULL(a1.monday,0))+CONVERT(int,ISNULL(a1.tuesday,0))+CONVERT(int,ISNULL(a1.wednesday,0))+CONVERT(int,ISNULL(a1.thursday,0))+CONVERT(int,ISNULL(a1.friday,0))+CONVERT(int,ISNULL(a1.saturday,0))+CONVERT(int,ISNULL(a1.sunday,0))+CONVERT(int,ISNULL(a1.ispubhol,0)))),' ','') as uniquekey, " +
                                   "a1.refnum as docnum,ah.arnum, a1.matnum ,a1.matname,a1.shiftcode,a1.sitenum,a1.sectorcode,a1.officerqty,(a1.noofdays*a1.rate) as rate,a1.remark " +
                                   "from ADH1 a1 LEFT JOIN ADH ah on a1.refnum=ah.refnum where ah.[status]<>'V' and ah.refnum='" + dr["refnum"].ToString() + "' " +
                               ")A " +
                               "GROUP BY " +
                               "uniquekey,docunum,arnum,matnum,matname,shiftcode,sitenum,sectorcode,rate,remark ";
            }
            else
            {
                sql1 = " SELECT uniquekey,docunum,arnum,matnum,matname,shiftcode,sitenum,sectorcode,sum(officerqty)as officerqty,rate,remark   FROM " +
                                "( " +
                                "select  " +
                                    "REPLACE(c1.refnum+''+ch.arnum+''+c1.matnum+''+c1.shiftcode+''+c1.sectorcode+''+ " +
                                    "CONVERT(nvarchar(1),(CONVERT(int,ISNULL(c1.monday,0))+CONVERT(int,ISNULL(c1.tuesday,0))+CONVERT(int,ISNULL(c1.wednesday,0))+CONVERT(int,ISNULL(c1.thursday,0))+CONVERT(int,ISNULL(c1.friday,0))+CONVERT(int,ISNULL(c1.saturday,0))+CONVERT(int,ISNULL(c1.sunday,0))+CONVERT(int,ISNULL(c1.ispubhol,0)))),' ','') as uniquekey,  " +
                                    "c1.refnum as docunum,ch.arnum, c1.matnum ,c1.matname,c1.shiftcode,c1.sitenum,c1.sectorcode,c1.officerqty,c1.rate,c1.remark  " +
                                     "from CTR1 c1 LEFT JOIN CTRH ch on c1.refnum=ch.refnum where ch.[status]<>'V' and ch.refnum='" + dr["refnum"].ToString() + "'  " +

                                    "union all " +

                                "select " +
                                    "REPLACE(a1.refnum+''+ah.arnum+''+a1.matnum+''+a1.shiftcode+''+a1.sectorcode+''+ " +
                                    "CONVERT(nvarchar(1),(CONVERT(int,ISNULL(a1.monday,0))+CONVERT(int,ISNULL(a1.tuesday,0))+CONVERT(int,ISNULL(a1.wednesday,0))+CONVERT(int,ISNULL(a1.thursday,0))+CONVERT(int,ISNULL(a1.friday,0))+CONVERT(int,ISNULL(a1.saturday,0))+CONVERT(int,ISNULL(a1.sunday,0))+CONVERT(int,ISNULL(a1.ispubhol,0)))),' ','') as uniquekey, " +
                                    "a1.refnum as docnum,ah.arnum, a1.matnum ,a1.matname,a1.shiftcode,a1.sitenum,a1.sectorcode,a1.officerqty,a1.rate,a1.remark " +
                                    "from ADH1 a1 LEFT JOIN ADH ah on a1.refnum=ah.refnum where ah.[status]<>'V' and ah.refnum='" + dr["refnum"].ToString() + "' " +
                                ")A " +
                                "GROUP BY " +
                                "uniquekey,docunum,arnum,matnum,matname,shiftcode,sitenum,sectorcode,rate,remark ";
            }

            this.dbAccess.ReadSQL("ContractDetails", sql1);

            DataTable ContractDetails = this.dbAccess.DataSet.Tables["ContractDetails"];

            //Find which does not exists in the Details
            if (ContractDetails.Rows.Count > 0)
            {


                foreach (DataRow dr1 in ContractDetails.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow newRow = siv1.NewRow();
                        newRow["ctrnum"] = dr1["docunum"];
                        newRow["matnum"] = dr1["matnum"].ToString().Trim();
                        newRow["dqty"] = dr1["officerqty"];
                        newRow["price"] = dr1["rate"];
                        newRow["oricur"] = sivh["oricur"];
                        newRow["hscode"] = dr1["sectorcode"];
                        newRow["detail"] = dr1["matname"];
                        newRow["uniquekey"] = dr1["uniquekey"];
                        newRow["shiftcode"] = dr1["shiftcode"];
                        newRow["remark"] = dr1["remark"];
                        newRow["ccnum"] = dr1["sitenum"];
                        siv1.Rows.Add(newRow);
                    }
                }
                //DataTable tmpContractDetails = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select uniquekey from ContractDetails");
                //DataTable tmpSIV1 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select uniquekey from siv1");

                //tmpContractDetails.TableName = "tmpContractDetails";
                //tmpSIV1.TableName = "tmpSIV1";

              
                //DataTable tmpDT = BizLogicTools.Tools.getDifferentRecords(tmpContractDetails, tmpSIV1);

                //if (tmpDT.Rows.Count > 0)
                //{
                //    foreach (DataRow dr1 in tmpDT.Rows)
                //    {
                //        if (dr1.RowState != DataRowState.Deleted)
                //        {
                //            foreach (DataRow dr2 in ContractDetails.Rows)
                //            {
                //                if (dr2.RowState != DataRowState.Deleted)
                //                {
                //                    if (dr1["uniquekey"] == dr2["uniquekey"])
                //                    {

                //                        DataRow newRow = siv1.NewRow();
                //                        newRow["ctrnum"] = dr2["docunum"];
                //                        newRow["matnum"] = dr2["matnum"].ToString().Trim();
                //                        newRow["dqty"] = dr2["officerqty"];
                //                        newRow["price"] = dr2["rate"];
                //                        newRow["oricur"] = sivh["oricur"];
                //                        newRow["hscode"] = dr2["sectorcode"];
                //                        newRow["detail"] = dr2["matname"];
                //                        newRow["uniquekey"] = dr2["uniquekey"];
                //                        newRow["shiftcode"] = dr2["shiftcode"];
                //                        newRow["remark"] = dr2["remark"];
                //                        newRow["ccnum"] = dr2["sitenum"];
                //                        siv1.Rows.Add(newRow);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

            }
            //end of finding which is not in the details

        }
    }
}