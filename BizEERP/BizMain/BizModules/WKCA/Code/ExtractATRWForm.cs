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

namespace ATL.WKC
{
    public partial class ExtractATRWForm : Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable siv1;
        protected string SectorCode = "";

        public ExtractATRWForm(DBAccess dbAccess, DataTable originalTable, string Sector)
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
          
            DataRow wkch = this.dbAccess.DataSet.Tables["wkch"].Rows[0];
            DataTable wkc1 = this.dbAccess.DataSet.Tables["wkc1"];

         
            //string sql1 = "SELECT C.refnum,C.arnum,A.arname, C.sitenum, ISNULL(convert(varchar,C.commencedate, 103),'') as commencedate, ISNULL(convert(varchar,C.enddate, 103),'') as enddate FROM " +
            //                "( "+
            //                "select refnum,arnum,sitenum,commencedate,enddate from CTRH WHERE [status]<>'V' AND arnum='" + sivh["arnum"].ToString() + "'  " +
            //                "union all "+
            //                "select refnum,arnum,sitenum,commencedate,enddate from ADH WHERE [status]<>'V' AND arnum='" + sivh["arnum"].ToString() + "' " +
            //                ")C  "+
            //                "LEFT JOIN ARM A on C.arnum=A.arnum"; 

            //string sql1 = "";

            /////////////////////////

            string GetATR1 = "";
            if ((!BizFunctions.IsEmpty(wkch["fromempnum"]) && BizFunctions.IsEmpty(wkch["toempnum"])) || (BizFunctions.IsEmpty(wkch["fromempnum"]) && !BizFunctions.IsEmpty(wkch["toempnum"])))
            {
                string empnum = "";

                if (!BizFunctions.IsEmpty(wkch["fromempnum"]))
                {
                    empnum = wkch["fromempnum"].ToString().Trim();
                }
                if (!BizFunctions.IsEmpty(wkch["toempnum"]))
                {
                    empnum = wkch["toempnum"].ToString().Trim();
                }

                GetATR1 = "Select empnum,sitenum,sectorcode,shiftcode,[timein],[timeout],scheddatein,schedtimein,scheddateout,schedtimeout,confirmedtimein,confirmedtimeout,TotalHrs,lateMins, " +
                                "rempnum,rempname,rnric,paytypecode,isadhoc,isOT,OTrate,isRD,isOffset,isDRE,isUS,RepRefKey,uniquekey,nric,[day],[Date],[dayofweek],refnum,[guid] "+
                           "from ATR1 where [date]>=CONVERT(DATETIME, '" + BizFunctions.GetSafeDateString(Convert.ToDateTime(wkch["WeekDateFrom"])) + "', 103) and [date]<=CONVERT(DATETIME, '" + BizFunctions.GetSafeDateString(Convert.ToDateTime(wkch["WeekDateTo"])) + "', 103)   and status<>'V' and sectorcode='"+SectorCode+"' " +
                               "and empnum='" + empnum + "' and [guid] not in (Select [guid] FROM wkc1 where [status]<>'V') and paytypecode='W'";

            }
            else if (!BizFunctions.IsEmpty(wkch["fromempnum"]) || !BizFunctions.IsEmpty(wkch["toempnum"]))
            {
                string empnum1, empnum2 = "";


                empnum1 = wkch["fromempnum"].ToString().Trim();
                empnum2 = wkch["toempnum"].ToString().Trim();

                GetATR1 = "Select empnum,sitenum,sectorcode,shiftcode,[timein],[timeout],scheddatein,schedtimein,scheddateout,schedtimeout,confirmedtimein,confirmedtimeout,TotalHrs,lateMins, " +
                                "rempnum,rempname,rnric,paytypecode,isadhoc,isOT,OTrate,isRD,isDRE,isOffset,isUS,RepRefKey,uniquekey,nric,[day],[Date],[dayofweek],refnum,[guid] " +
                            "from ATR1 where [date]>=CONVERT(DATETIME, '" + BizFunctions.GetSafeDateString(Convert.ToDateTime(wkch["WeekDateFrom"])) + "', 103) and [date]<=CONVERT(DATETIME, '" + BizFunctions.GetSafeDateString(Convert.ToDateTime(wkch["WeekDateTo"])) + "', 103)   and status<>'V' and sectorcode='" + SectorCode + "' " +
                               "and empnum>='" + empnum1 + "' and empnum<='" + empnum2 + "' and [guid] not in (Select [guid] FROM wkc1 where [status]<>'V') and paytypecode='W'";

            }
            else
            {

                GetATR1 = "Select empnum,sitenum,sectorcode,[timein],[timeout],shiftcode,scheddatein,schedtimein,scheddateout,schedtimeout,confirmedtimein,confirmedtimeout,TotalHrs,lateMins, " +
                                "rempnum,rempname,rnric,paytypecode,isadhoc,isOT,OTrate,isRD,isDRE,isOffset,isUS,RepRefKey,uniquekey,nric,[day],[Date],[dayofweek],refnum,[guid] " +
                            "from ATR1 where [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(wkch["WeekDateFrom"])) + "' and [date]<= '" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(wkch["WeekDateTo"])) + "'  and status<>'V' and sectorcode='" + SectorCode + "' and [guid] not in (Select [guid] FROM wkc1 where [status]<>'V') and paytypecode='W' ORDER BY empnum,[date]";

            }

            this.dbAccess.ReadSQL("ATRliveALL", GetATR1);

            DataTable ATRliveALL = this.dbAccess.DataSet.Tables["ATRliveALL"];

            string InsertIntowkc1 = "SELECT * FROM ATRliveALL WHERE uniquekey not in (Select uniquekey from wkc1)";

            DataTable dtwkc1tmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntowkc1);

            if (this.dbAccess.DataSet.Tables.Contains("dtwkc1tmp"))
            {
                this.dbAccess.DataSet.Tables["dtwkc1tmp"].Dispose();
                this.dbAccess.DataSet.Tables["dtwkc1tmp"].Clear();
            }
            else
            {
                dtwkc1tmp.TableName = "dtwkc1tmp";
                this.dbAccess.DataSet.Tables.Add(dtwkc1tmp);
            }




            ////////////////////////



            if (dtwkc1tmp != null)
            {
                if (dtwkc1tmp.Rows.Count > 0)
                {

                    dt_view = dtwkc1tmp.Copy();
                    dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
                    //dt_view.Columns[5].DefaultValue = 0;

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
            DataTable wkc1 = dbAccess.DataSet.Tables["wkc1"];
            try
            {
                foreach (DataRow row in this.dt_view.Rows)
                {
                    if (row["Mark"] != System.DBNull.Value)
                    {
                        if ((bool)row["Mark"] == true)
                        {
                            DataRow wkc1tmp = dbAccess.DataSet.Tables["wkc1"].NewRow();

                            //foreach (DataColumn dc in dt_view.Columns)
                            //{

                            //    if (dbAccess.DataSet.Tables["wkc1"].Columns.IndexOf(dc.ColumnName) != -1)
                            //    {
                            //        wkc1tmp[dc.ColumnName] = row[dc.ColumnName];
                            //    }
                            //}

                            wkc1tmp["Mark"] = 0;
                            //NewRow(row);

                            ////////////////////////////////////////////////////////////////
                            wkc1tmp["uniquekey"] = row["uniquekey"];

                             wkc1tmp["atrnum"] = row["refnum"];
                             wkc1tmp["nric"] = row["nric"];
                             wkc1tmp["empnum"] = row["empnum"];
                             wkc1tmp["empname"] = GetEmpname(row["empnum"].ToString());
                             wkc1tmp["empnum2"] = row["empnum"];
                             wkc1tmp["empnum3"] = row["empnum"];
                             wkc1tmp["shiftcode"] = row["shiftcode"];
                             wkc1tmp["sitenum"] = row["sitenum"];
                             wkc1tmp["sectorcode"] = row["sectorcode"];
                             wkc1tmp["confirmedtimein"] = row["confirmedtimein"];
                             wkc1tmp["confirmedtimeout"] = row["confirmedtimeout"];
                             wkc1tmp["scheddatein"] = row["scheddatein"];
                             wkc1tmp["scheddateout"] = row["scheddateout"];
                             //wkc1tmp["timein"] = row["timein"];
                             //wkc1tmp["timeout"] = row["timeout"];
                             wkc1tmp["schedtimein"] = row["schedtimein"];
                             wkc1tmp["schedtimeout"] = row["schedtimeout"];
                             wkc1tmp["rempnum"] = row["rempnum"];
                             wkc1tmp["rempname"] = row["rempname"];
                             wkc1tmp["rnric"] = row["rnric"];
                             wkc1tmp["day"] = row["day"];
                             wkc1tmp["date"] = row["date"];
                             wkc1tmp["dayofweek"] = row["dayofweek"];
                             wkc1tmp["RepRefKey"] = row["RepRefKey"];
                             wkc1tmp["paytypecode"] = row["paytypecode"];
                             wkc1tmp["daysperweek"] = GetDaysPerWeek(row["empnum"].ToString());
                             wkc1tmp["LateMins"] = row["LateMins"];
                            if (!BizFunctions.IsEmpty(row["paytypecode"]))
                            {
                                 wkc1tmp["value"] = GetPayTypeValue(row["paytypecode"].ToString());
                            }

                             wkc1tmp["isadhoc"] = row["isadhoc"];


                            if (BizFunctions.IsEmpty(row["isadhoc"]))
                            {
                                 wkc1tmp["isadhoc"] = 0;
                            }
                            else
                            {
                                 wkc1tmp["isadhoc"] = row["isadhoc"];
                            }

                            if (BizFunctions.IsEmpty(row["isOT"]))
                            {
                                 wkc1tmp["isOT"] = 0;
                            }
                            else
                            {
                                 wkc1tmp["isOT"] = row["isOT"];
                            }


                            if (BizFunctions.IsEmpty(row["OTrate"]))
                            {
                                 wkc1tmp["OTrate"] = 0;
                            }
                            else
                            {
                                 wkc1tmp["OTrate"] = row["OTrate"];
                            }


                            if (BizFunctions.IsEmpty(row["isRD"]))
                            {
                                 wkc1tmp["isRD"] = 0;
                            }
                            else
                            {
                                 wkc1tmp["isRD"] = row["isRD"];
                            }


                            if (BizFunctions.IsEmpty(row["isDRE"]))
                            {
                                 wkc1tmp["isDRE"] = 0;
                            }
                            else
                            {
                                 wkc1tmp["isDRE"] = row["isDRE"];
                            }

                            if (BizFunctions.IsEmpty(row["isOffset"]))
                            {
                                wkc1tmp["isOffset"] = 0;
                            }
                            else
                            {
                                wkc1tmp["isOffset"] = row["isOffset"];
                            }

                            if (BizFunctions.IsEmpty(row["isUS"]))
                            {
                                 wkc1tmp["isUS"] = 0;
                            }

                            else
                            {
                                 wkc1tmp["isUS"] = row["isUS"];
                            }

                             wkc1tmp["TotalHrs"] = row["TotalHrs"];
                             wkc1tmp["guid"] = row["guid"];



                            dbAccess.DataSet.Tables["wkc1"].Rows.Add(wkc1tmp);
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