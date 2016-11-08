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

namespace ATL.MTA
{
    public partial class ExtractATRMForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable mta1;
        protected string SectorCode ="";

        public ExtractATRMForm(DBAccess dbAccess, DataTable originalTable, string Sector)
        {
            this.dbAccess = dbAccess;
            this.oriTable = originalTable;    
            this.SectorCode = Sector;

            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshDatagrid()
        {
          
            DataRow mtah = this.dbAccess.DataSet.Tables["mtah"].Rows[0];
            DataTable mta1 = this.dbAccess.DataSet.Tables["mta1"];

            string GetATR1 = "";
            if ((!BizFunctions.IsEmpty(mtah["fromempnum"]) && BizFunctions.IsEmpty(mtah["toempnum"])) || (BizFunctions.IsEmpty(mtah["fromempnum"]) && !BizFunctions.IsEmpty(mtah["toempnum"])))
            {
                string empnum = "";

                if (!BizFunctions.IsEmpty(mtah["fromempnum"]))
                {
                    empnum = mtah["fromempnum"].ToString().Trim();
                }
                if (!BizFunctions.IsEmpty(mtah["toempnum"]))
                {
                    empnum = mtah["toempnum"].ToString().Trim();
                }

                GetATR1 = "Select empnum,sitenum,sectorcode,shiftcode,[timein],[timeout],scheddatein,schedtimein,scheddateout,schedtimeout,confirmedtimein,confirmedtimeout,TotalHrs,lateMins, " +
                                "rempnum,rempname,rnric,paytypecode,isadhoc,isOT,OTrate,isRD,isDRE,isOffset,isUS,RepRefKey,uniquekey,nric,[day],[Date],[dayofweek],refnum,[guid] " +
                            "from ATR1 where MONTH([date])=" + mtah["monthno"] + " AND YEAR([date])=" + mtah["monthyear"] + "  and status<>'V' and sectorcode='" + SectorCode + "' and empnum='" + empnum + "' and [guid] not in (Select [guid] from mta1 where [status]<>'V') order by empnum,[date]";

            }
            else if (!BizFunctions.IsEmpty(mtah["fromempnum"]) || !BizFunctions.IsEmpty(mtah["toempnum"]))
            {
                string empnum1, empnum2 = "";


                empnum1 = mtah["fromempnum"].ToString().Trim();
                empnum2 = mtah["toempnum"].ToString().Trim();

                GetATR1 = "Select empnum,sitenum,sectorcode,shiftcode,[timein],[timeout],scheddatein,schedtimein,scheddateout,schedtimeout,confirmedtimein,confirmedtimeout,TotalHrs,lateMins, " +
                                "rempnum,rempname,rnric,paytypecode,isadhoc,isOT,OTrate,isRD,isDRE,isOffset,isUS,RepRefKey,uniquekey,nric,[day],[Date],[dayofweek],refnum,[guid] " +
                            "from ATR1 where MONTH([date])=" + mtah["monthno"] + " AND YEAR([date])=" + mtah["monthyear"] + "  and status<>'V' and sectorcode='" + SectorCode + "' and empnum>='" + empnum1 + "' and empnum<='" + empnum2 + "' and [guid] not in (Select [guid] from mta1 where [status]<>'V') order by empnum,[date]";


            }
            else
            {
                GetATR1 = "Select empnum,sitenum,sectorcode,shiftcode,[timein],[timeout],scheddatein,schedtimein,scheddateout,schedtimeout,confirmedtimein,confirmedtimeout,TotalHrs,lateMins, " +
                                "rempnum,rempname,rnric,paytypecode,isadhoc,isOT,OTrate,isRD,isDRE,isOffset,isUS,RepRefKey,uniquekey,nric,[day],[Date],[dayofweek],refnum,[guid] " +
                            "from ATR1 where MONTH([date])=" + mtah["monthno"] + " AND YEAR([date])=" + mtah["monthyear"] + "  and status<>'V' and sectorcode='" + SectorCode + "' and paytypecode='M' and [guid] not in (Select [guid] from mta1 where [status]<>'V') order by empnum,[date]";
            }

            this.dbAccess.ReadSQL("ATRliveALL", GetATR1);

            DataTable ATRliveALL = this.dbAccess.DataSet.Tables["ATRliveALL"];

            string InsertIntomta1 = "SELECT * FROM ATRliveALL WHERE uniquekey not in (Select uniquekey from mta1)";

             DataTable dtmta1tmp = null;

            if (ATRliveALL.Rows.Count > 0)
            {

                dtmta1tmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntomta1);

                if (this.dbAccess.DataSet.Tables.Contains("dtmta1tmp"))
                {
                    this.dbAccess.DataSet.Tables["dtmta1tmp"].Dispose();
                    this.dbAccess.DataSet.Tables["dtmta1tmp"].Clear();
                }
                else
                {
                    dtmta1tmp.TableName = "dtmta1tmp";
                    this.dbAccess.DataSet.Tables.Add(dtmta1tmp);
                }

            }


            ////////////////////////


            if (dtmta1tmp != null)
            {

                if (dtmta1tmp.Rows.Count > 0)
                {

                    dt_view = dtmta1tmp.Copy();
                    dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
                    this.dgView1.DataSource = dt_view;

                }
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

        private string GetEmpname(string empnum)
        {
            string empName = "";

            string Get = "Select Empname From HEMPHtmp1 where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                empName = dt1.Rows[0]["empname"].ToString();
            }

            dt1.Dispose();

            return empName;

        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            DataTable mta1 = dbAccess.DataSet.Tables["mta1"];
            try
            {
                foreach (DataRow row in this.dt_view.Rows)
                {
                    if (row["Mark"] != System.DBNull.Value)
                    {
                        if ((bool)row["Mark"] == true)
                        {
                            DataRow mta1tmp = dbAccess.DataSet.Tables["mta1"].NewRow();

                            //foreach (DataColumn dc in dt_view.Columns)
                            //{

                            //    if (dbAccess.DataSet.Tables["mta1"].Columns.IndexOf(dc.ColumnName) != -1)
                            //    {
                            //        mta1tmp[dc.ColumnName] = row[dc.ColumnName];
                            //    }
                            //}

                            mta1tmp["Mark"] = 0;
                            //NewRow(row);

                            ////////////////////////////////////////////////////////////////
                            mta1tmp["uniquekey"] = row["uniquekey"];

                             mta1tmp["atrnum"] = row["refnum"];
                             mta1tmp["nric"] = row["nric"];
                             mta1tmp["empnum"] = row["empnum"];
                             mta1tmp["empname"] = GetEmpname(row["empnum"].ToString());
                             mta1tmp["empnum2"] = row["empnum"];
                             mta1tmp["empnum3"] = row["empnum"];
                             mta1tmp["shiftcode"] = row["shiftcode"];
                             mta1tmp["sitenum"] = row["sitenum"];
                             mta1tmp["sectorcode"] = row["sectorcode"];
                             mta1tmp["confirmedtimein"] = row["confirmedtimein"];
                             mta1tmp["confirmedtimeout"] = row["confirmedtimeout"];
                             mta1tmp["scheddatein"] = row["scheddatein"];
                             mta1tmp["scheddateout"] = row["scheddateout"];
                             //mta1tmp["timein"] = row["timein"];
                             //mta1tmp["timeout"] = row["timeout"];
                             mta1tmp["schedtimein"] = row["schedtimein"];
                             mta1tmp["schedtimeout"] = row["schedtimeout"];
                             mta1tmp["rempnum"] = row["rempnum"];
                             mta1tmp["rempname"] = row["rempname"];
                             mta1tmp["rnric"] = row["rnric"];
                             mta1tmp["day"] = row["day"];
                             mta1tmp["date"] = row["date"];
                             mta1tmp["dayofweek"] = row["dayofweek"];
                             mta1tmp["RepRefKey"] = row["RepRefKey"];
                             mta1tmp["paytypecode"] = row["paytypecode"];
                             mta1tmp["daysperweek"] = GetDaysPerWeek(row["empnum"].ToString());
                             mta1tmp["LateMins"] = row["LateMins"];
                            if (!BizFunctions.IsEmpty(row["paytypecode"]))
                            {
                                 mta1tmp["value"] = GetPayTypeValue(row["paytypecode"].ToString());
                            }

                             mta1tmp["isadhoc"] = row["isadhoc"];


                            if (BizFunctions.IsEmpty(row["isadhoc"]))
                            {
                                 mta1tmp["isadhoc"] = 0;
                            }
                            else
                            {
                                 mta1tmp["isadhoc"] = row["isadhoc"];
                            }

                            if (BizFunctions.IsEmpty(row["isOT"]))
                            {
                                 mta1tmp["isOT"] = 0;
                            }
                            else
                            {
                                 mta1tmp["isOT"] = row["isOT"];
                            }


                            if (BizFunctions.IsEmpty(row["OTrate"]))
                            {
                                 mta1tmp["OTrate"] = 0;
                            }
                            else
                            {
                                 mta1tmp["OTrate"] = row["OTrate"];
                            }


                            if (BizFunctions.IsEmpty(row["isRD"]))
                            {
                                 mta1tmp["isRD"] = 0;
                            }
                            else
                            {
                                 mta1tmp["isRD"] = row["isRD"];
                            }


                            if (BizFunctions.IsEmpty(row["isDRE"]))
                            {
                                 mta1tmp["isDRE"] = 0;
                            }
                            else
                            {
                                 mta1tmp["isDRE"] = row["isDRE"];
                            }

                            if (BizFunctions.IsEmpty(row["isOffset"]))
                            {
                                mta1tmp["isOffset"] = 0;
                            }
                            else
                            {
                                mta1tmp["isOffset"] = row["isOffset"];
                            }


                            if (BizFunctions.IsEmpty(row["isUS"]))
                            {
                                 mta1tmp["isUS"] = 0;
                            }

                            else
                            {
                                 mta1tmp["isUS"] = row["isUS"];
                            }
                            mta1tmp["guid"] = row["guid"];

                             mta1tmp["TotalHrs"] = row["TotalHrs"];



                            dbAccess.DataSet.Tables["mta1"].Rows.Add(mta1tmp);
                           ///////////////////////////////////////////////////////////////////
                           
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

        private decimal GetDaysPerWeek(string empnum)
        {
            decimal daysPerWeek = 0;

            string getDaysPerWK = "Select daysperweek from HEMPHtmp1 where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, getDaysPerWK);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                if (!BizFunctions.IsEmpty(dr1["daysperweek"]))
                {
                    daysPerWeek = Convert.ToDecimal(dr1["daysperweek"]);
                }

            }

            return daysPerWeek;
        }

        private decimal GetPayTypeValue(string paytypecode)
        {
            decimal value = 0;

            string GetPayTypeValue = "Select ISNULL(value,0) as value from PAYTM where paytypecode='" + paytypecode + "'";

            DataTable dt2 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, GetPayTypeValue);

            if (dt2.Rows.Count > 0)
            {
                DataRow dr2 = dt2.Rows[0];

                value = Convert.ToDecimal(dr2["value"]);
            }



            return value;
        }


        private string GetPayTypeCode(string empnum)
        {
            string paytype = "";

            string getPayType = "Select paytypecode from vMainHEMPH where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, getPayType);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                paytype = dr1["paytypecode"].ToString();
            }

            return paytype;
        }

        protected void NewRow(DataRow dr)
        {
            DataRow sivh = dbAccess.DataSet.Tables["sivh"].Rows[0];
            DataTable siv1 = dbAccess.DataSet.Tables["siv1"];


            string sql1 = "SELECT * FROM " +
                            "( " +
                            "select  " +
                                "REPLACE(c1.refnum+''+ch.arnum+''+c1.matnum+''+c1.shiftcode+''+c1.sectorcode+''+ " +
                                "CONVERT(nvarchar(1),(CONVERT(int,ISNULL(c1.monday,0))+CONVERT(int,ISNULL(c1.tuesday,0))+CONVERT(int,ISNULL(c1.wednesday,0))+CONVERT(int,ISNULL(c1.thursday,0))+CONVERT(int,ISNULL(c1.friday,0))+CONVERT(int,ISNULL(c1.saturday,0))+CONVERT(int,ISNULL(c1.sunday,0)))),' ','') as uniquekey,  " +
                                "c1.refnum as docunum,ch.arnum, c1.matnum ,c1.matname,c1.shiftcode,c1.sitenum,c1.sectorcode,c1.officerqty,c1.rate,c1.remark  " +
                                 "from CTR1 c1 LEFT JOIN CTRH ch on c1.refnum=ch.refnum where ch.[status]<>'V' and ch.refnum='" + dr["refnum"].ToString() + "'  " +

                                "union all " +

                            "select " +
                                "REPLACE(a1.refnum+''+ah.arnum+''+a1.matnum+''+a1.shiftcode+''+a1.sectorcode+''+ " +
                                "CONVERT(nvarchar(1),(CONVERT(int,ISNULL(a1.monday,0))+CONVERT(int,ISNULL(a1.tuesday,0))+CONVERT(int,ISNULL(a1.wednesday,0))+CONVERT(int,ISNULL(a1.thursday,0))+CONVERT(int,ISNULL(a1.friday,0))+CONVERT(int,ISNULL(a1.saturday,0))+CONVERT(int,ISNULL(a1.sunday,0)))),' ','') as uniquekey, " +
                                "a1.refnum as docnum,ah.arnum, a1.matnum ,a1.matname,a1.shiftcode,a1.sitenum,a1.sectorcode,a1.officerqty,a1.rate,a1.remark " +
                                "from ADH1 a1 LEFT JOIN ADH ah on a1.refnum=ah.refnum where ah.[status]<>'V' and ah.refnum='" + dr["refnum"].ToString() + "' " +
                            ")A ";

            this.dbAccess.ReadSQL("ContractDetails", sql1);

            DataTable ContractDetails = this.dbAccess.DataSet.Tables["ContractDetails"];

            if (ContractDetails.Rows.Count > 0)
            {

                DataTable tmpContractDetails = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select uniquekey from ContractDetails");
                DataTable tmpSIV1 = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, "Select uniquekey from siv1");

                tmpContractDetails.TableName = "tmpContractDetails";
                tmpSIV1.TableName = "tmpSIV1";

              
                DataTable tmpDT = BizLogicTools.Tools.getDifferentRecords(tmpContractDetails, tmpSIV1);

                if (tmpDT.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in tmpDT.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            foreach (DataRow dr2 in ContractDetails.Rows)
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    if (dr1["uniquekey"] == dr2["uniquekey"])
                                    {

                                        DataRow newRow = siv1.NewRow();
                                        newRow["docunum"] = dr2["docunum"];
                                        newRow["matnum"] = dr2["matnum"].ToString().Trim();
                                        newRow["dqty"] = dr2["officerqty"];
                                        newRow["price"] = dr2["rate"];
                                        newRow["oricur"] = sivh["oricur"];
                                        newRow["hscode"] = dr2["sectorcode"];
                                        newRow["detail"] = dr2["remark"];
                                        newRow["uniquekey"] = dr2["uniquekey"];
                                        newRow["shiftcode"] = dr2["shiftcode"];
                                        siv1.Rows.Add(newRow);
                                    }
                                }
                            }
                        }
                    }
                }

            }

        }
    }
}