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

namespace ATL.ATR
{
    public partial class ExtractALiveForm2: Form
    {
        private DataTable oriTable = null;
        private DBAccess dbAccess = null;
        private DataTable dt_view = null;
        protected DataTable siv1;
        //string posid = System.Configuration.ConfigurationManager.AppSettings.Get("POSID");
        //string sitenum = null;
        //Hashtable remoteHash = null;
        //DBAccess remoteDBAccess = null;

        public ExtractALiveForm2(DBAccess dbAccess, DataTable originalTable)
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
          
            DataRow atr = this.dbAccess.DataSet.Tables["atr"].Rows[0];
            DataTable atr1 = this.dbAccess.DataSet.Tables["atr1"];
        
            //string sql1 = "SELECT empnum,[date],sitenum,sectorcode,[day],shiftcode,[timein],[timeout],ActualDateTimeIN,ActualTimeIn2,ActualDateTimeOut, "+
            //               "ActualTimeOut2,rnric,rempnum,rempname,isTR,TRsitenum,remark,isAdhoc,isOT,OTrate,isDRE,isOffset,isUS,isRD,refnum,uniquekey,Refkey,[guid]  FROM " +
            //                    "( " +
            //                    "Select * from atmrlive A LEFT JOIN (SELECT shiftcode as shift, isworkshift from vSHLV) v on A.shiftcode=v.shift " +
            //                        "where " +
            //                        //"( " +
            //                        //    "A.refnum='" + atr["wrrnum"].ToString().Trim() + "' and (ISNULL(A.ClockInMark,0)=1 OR ISNULL(v.isworkshift,0)=0)  and  uniquekey not in (Select uniquekey from ATR1 where wrrnum='" + atr["wrrnum"].ToString().Trim() + "' and [status]<>'V') " +
            //                        //") " +
            //                        //"or " +
            //                        "( " +
            //                            "(A.refnum is null or A.refnum='') and sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' and (ISNULL(A.ClockInMark,0)=1 OR ISNULL(v.isworkshift,0)=0)  " +
            //                            "and " +
            //                            "[guid] not in (Select [guid] from ATR1 where [status]<>'V' and sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' )) and A.shiftcode<>'V' " +

            //                     ")B " +
            //                      "order by empnum,[date]";

            string sql1 = "";

            if (!BizFunctions.IsEmpty(atr["fromEmpnum"]) && !BizFunctions.IsEmpty(atr["toEmpnum"]))
            {
                //Jason:27072015- Changed filter
                //sql1 = "SELECT *  FROM " +
                //                        "( " +
                //                        "Select * from atmrlive A LEFT JOIN (SELECT shiftcode as shift, isworkshift from vSHLV) v on A.shiftcode=v.shift " +
                //                            "where " +
                //                            "( " +
                //                                " empnum>='" + atr["fromEmpnum"].ToString() + "' and empnum<='" + atr["toEmpnum"].ToString() + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' and (ISNULL(A.ClockInMark,0)=1 OR ISNULL(v.isworkshift,0)=0)  " +
                //                                "and " +
                //                                "[guid] not in (Select pguid as [guid] from ATR1 where [status]<>'V' and (empnum>='" + atr["fromEmpnum"].ToString() + "' and empnum<='" + atr["toEmpnum"].ToString() + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' ))) and A.shiftcode<>'V' " +

                //                         ")B " +
                //                          "order by empnum,[date]";

                sql1 = "SELECT *  FROM " +
                               "( " +
                               "Select * from atmrlive A LEFT JOIN (SELECT shiftcode as shift, isworkshift from vSHLV) v on A.shiftcode=v.shift " +
                                   "where " +
                                   "( " +
                                       " empnum>='" + atr["fromEmpnum"].ToString() + "' and empnum<='" + atr["toEmpnum"].ToString() + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "'  " +
                                       "and " +
                                       "[guid] not in (Select pguid as [guid] from ATR1 where [status]<>'V' and (empnum>='" + atr["fromEmpnum"].ToString() + "' and empnum<='" + atr["toEmpnum"].ToString() + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' ))) and A.shiftcode<>'V' and (ISNULL(ClockInMark,0)=1 OR ISNULL(v.isWorkShift,0)=0) " +

                                ")B " +
                                 "order by empnum,[date]";
            }
            else if (BizFunctions.IsEmpty(atr["fromEmpnum"]) && BizFunctions.IsEmpty(atr["toEmpnum"]) && !BizFunctions.IsEmpty(atr["sitenum"]))
            {
                //Jason:27072015- Changed filter
                //sql1 = "SELECT *  FROM " +
                //            "( " +
                //            "Select * from atmrlive A LEFT JOIN (SELECT shiftcode as shift, isworkshift from vSHLV) v on A.shiftcode=v.shift " +
                //                "where " +
                //                "( " +
                //                    " sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' and (ISNULL(A.ClockInMark,0)=1 OR ISNULL(v.isworkshift,0)=0)  " +
                //                    "and " +
                //                    "[guid] not in (Select pguid as [guid] from ATR1 where [status]<>'V' and (sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' ))) and A.shiftcode<>'V' " +

                //             ")B " +
                //              "order by empnum,[date]";

                sql1 = "SELECT *  FROM " +
                          "( " +
                          "Select * from atmrlive A LEFT JOIN (SELECT shiftcode as shift, isworkshift from vSHLV) v on A.shiftcode=v.shift " +
                              "where " +
                              "( " +
                                  " sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "'   " +
                                  "and " +
                                  "[guid] not in (Select pguid as [guid] from ATR1 where [status]<>'V' and (sitenum='" + atr["sitenum"] + "' and [date]>='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrFromDate"])) + "'  and [date]<='" + TimeTools.GetStandardSafeDateOnly2(Convert.ToDateTime(atr["wrrToDate"])) + "' ))) and A.shiftcode<>'V' and (ISNULL(ClockInMark,0)=1 OR ISNULL(v.isWorkShift,0)=0) " +

                           ")B " +
                            "order by empnum,[date]";
            }


            this.dbAccess.ReadSQL("ATMRliveALL", sql1);

            DataTable ATMRliveALL = this.dbAccess.DataSet.Tables["ATMRliveALL"];

            string InsertIntoAtr1 = "SELECT * FROM ATMRliveALL WHERE uniquekey not in (Select uniquekey from ATR1)";

            DataTable dtATR1tmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, InsertIntoAtr1);

            if (this.dbAccess.DataSet.Tables.Contains("dtATR1tmp"))
            {
                this.dbAccess.DataSet.Tables["dtATR1tmp"].Dispose();
                this.dbAccess.DataSet.Tables["dtATR1tmp"].Clear();
            }
            else
            {
                dtATR1tmp.TableName ="dtATR1tmp";
                this.dbAccess.DataSet.Tables.Add(dtATR1tmp);
            }

            if (dtATR1tmp.Rows.Count > 0)
            {
                foreach (DataRow dr1 in dtATR1tmp.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["timein"]))
                        {
                            if (!BizFunctions.IsEmpty(dr1["shiftcode"]))
                            {
                                dr1["timein"] = geTimeIn(dr1["shiftcode"].ToString());
                            }
                            else
                            {
                                dr1["timein"] = System.DBNull.Value;
                            }
                        }
                        else
                        {
                            dr1["timein"] = dr1["timein"];
                        }

                        //timeout
                        if (BizFunctions.IsEmpty(dr1["timeout"]))
                        {
                            if (!BizFunctions.IsEmpty(dr1["shiftcode"]))
                            {
                                dr1["timeout"] = geTimeOut(dr1["shiftcode"].ToString());
                            }
                            else
                            {
                                dr1["timeout"] = System.DBNull.Value;
                            }
                        }
                        else
                        {
                            dr1["timeout"] = dr1["timeout"];
                        }

                    }
                }

                dt_view = dtATR1tmp.Copy();
                dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));

       

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

        private string geTimeIn(string shiftcode)
        {
            string Timein = "";

            string GetvSHLV = "Select timein from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timein = vSHLVTmp.Rows[0]["timein"].ToString();
            }

            return Timein;
        }

        private string geTimeOut(string shiftcode)
        {
            string Timeout = "";

            string GetvSHLV = "Select [timeout] from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timeout = vSHLVTmp.Rows[0]["timeout"].ToString();
            }

            return Timeout;
        }


        private bool isWork(string shiftcode)
        {
            bool isWorkShift = false;

            string GetvSHLV = "Select isWorkShift from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, GetvSHLV);
            if (vSHLVTmp != null)
            {
                if (vSHLVTmp.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(vSHLVTmp.Rows[0]["isWorkShift"]))
                    {
                        vSHLVTmp.Rows[0]["isWorkShift"] = 0;
                    }
                    isWorkShift = Convert.ToBoolean(vSHLVTmp.Rows[0]["isWorkShift"]);
                }
            }

            return isWorkShift;
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            DataTable atr1 = dbAccess.DataSet.Tables["atr1"];
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
                                DataRow atr1tmp = dbAccess.DataSet.Tables["atr1"].NewRow();

                                //foreach (DataColumn dc in dt_view.Columns)
                                //{

                                //    if (dbAccess.DataSet.Tables["atr1"].Columns.IndexOf(dc.ColumnName) != -1)
                                //    {
                                //        atr1tmp[dc.ColumnName] = row[dc.ColumnName];
                                //    }
                                //}


                                atr1tmp["OTHrs"] = row["OTHrs"];
                                atr1tmp["FixAllowAmt"] = row["FixAllowAmt"];
                                atr1tmp["AttnRemarks"] = row["AttnRemarks"];

                                atr1tmp["Mark"] = 0;
                                atr1tmp["uniquekey"] = row["uniquekey"];
                                atr1tmp["empnum"] = row["empnum"].ToString().Trim();
                                atr1tmp["empname"] = GetEmpname(row["empnum"].ToString().Trim());
                                atr1tmp["empnum2"] = row["empnum"];
                                atr1tmp["shiftcode"] = row["shiftcode"];

                                atr1tmp["latemins"] = row["latemins"];


                                atr1tmp["sectorcode"] = row["sectorcode"];

                                // timein
                                if (BizFunctions.IsEmpty(row["timein"]))
                                {
                                    if (!BizFunctions.IsEmpty(row["shiftcode"]))
                                    {
                                        atr1tmp["timein"] = geTimeIn(row["shiftcode"].ToString());
                                    }
                                    else
                                    {
                                        atr1tmp["timein"] = System.DBNull.Value;
                                    }
                                }
                                else
                                {
                                    atr1tmp["timein"] = row["timein"];
                                }

                                //timeout
                                if (BizFunctions.IsEmpty(row["timeout"]))
                                {
                                    if (!BizFunctions.IsEmpty(row["shiftcode"]))
                                    {
                                        atr1tmp["timeout"] = geTimeOut(row["shiftcode"].ToString());
                                    }
                                    else
                                    {
                                        atr1tmp["timeout"] = System.DBNull.Value;
                                    }
                                }
                                else
                                {
                                    atr1tmp["timeout"] = row["timeout"];
                                }

                                if (BizFunctions.IsEmpty(row["ActualDateTimeIn"]))
                                {
                                    atr1tmp["scheddatein"] = System.DBNull.Value;
                                }
                                else
                                {
                                    atr1tmp["scheddatein"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(row["ActualDateTimeIn"]));

                                }

                                if (BizFunctions.IsEmpty(row["actualtimein2"]))
                                {
                                    atr1tmp["schedtimein"] = System.DBNull.Value;
                                }
                                else
                                {
                                    atr1tmp["schedtimein"] = row["actualtimein2"].ToString();
                                }

                                if (!BizFunctions.IsEmpty(row["timein"]) && !BizFunctions.IsEmpty(row["actualtimein2"]) || (row["timein"].ToString() != string.Empty && row["actualtimein2"] != string.Empty))
                                {

                                    LocalTime timein = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(row["timein"].ToString());
                                    LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(row["actualtimein2"].ToString());

                                    if (timein.TickOfDay < actualtimeout.TickOfDay)
                                    {
                                        atr1tmp["confirmedtimein"] = row["actualtimein2"].ToString();
                                    }
                                    else
                                    {
                                        atr1tmp["confirmedtimein"] = row["timein"].ToString();
                                    }

                                }

                                if (BizFunctions.IsEmpty(row["actualtimeout2"]))
                                {
                                    atr1tmp["schedtimeout"] = System.DBNull.Value;
                                }
                                else
                                {
                                    atr1tmp["schedtimeout"] = row["actualtimeout2"].ToString();

                                }

                                if (!BizFunctions.IsEmpty(row["timeout"]) && !BizFunctions.IsEmpty(row["actualtimeout2"]) || (row["timeout"].ToString() != string.Empty && row["actualtimeout2"].ToString() != string.Empty))
                                {

                                    LocalTime timeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(row["timeout"].ToString());
                                    LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(row["actualtimeout2"].ToString());

                                    atr1tmp["confirmedtimeout"] = row["actualtimeout2"].ToString();
                                  

                                }
                                else if (!BizFunctions.IsEmpty(row["timeout"]) && !BizFunctions.IsEmpty(row["timein"]) && !BizFunctions.IsEmpty(row["actualtimein2"]) && BizFunctions.IsEmpty(row["actualtimeout2"]))
                                {
                                    atr1tmp["confirmedtimeout"] = row["timeout"];
                                }


                                if (BizFunctions.IsEmpty(row["ActualDateTimeOut"]))
                                {
                                    if (!BizFunctions.IsEmpty(row["ActualDateTimeIn"]) || !BizFunctions.IsEmpty(row["actualtimein2"]))
                                    {
                                        if (!BizFunctions.IsEmpty(atr1tmp["confirmedtimein"]) && !BizFunctions.IsEmpty(atr1tmp["confirmedtimeout"]))
                                        {

                                            LocalTime schedtimein = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(atr1tmp["confirmedtimein"].ToString());
                                            LocalTime schedtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(atr1tmp["confirmedtimeout"].ToString());

                                            if (!(schedtimein.TickOfDay < schedtimeout.TickOfDay))
                                            {
                                                atr1tmp["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(row["ActualDateTimeIn"]).AddDays(+1));
                                            }
                                            else
                                            {
                                                atr1tmp["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(row["ActualDateTimeIn"]));
                                            }
                                        }
                                        else
                                        {
                                            atr1tmp["scheddateout"] = System.DBNull.Value;
                                        }
                                    }
                                    else
                                    {
                                        atr1tmp["scheddateout"] = System.DBNull.Value;
                                    }

                                }
                                else
                                {
                                    atr1tmp["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(row["ActualDateTimeOut"]));

                                }

                                if (BizFunctions.IsEmpty(row["isadhoc"]))
                                {
                                    atr1tmp["isadhoc"] = 0;
                                }

                                if (BizFunctions.IsEmpty(row["isOT"]))
                                {
                                    atr1tmp["isOT"] = 0;
                                }

                                if (BizFunctions.IsEmpty(row["OTrate"]))
                                {
                                    atr1tmp["OTrate"] = 0;
                                }

                                if (BizFunctions.IsEmpty(row["isRD"]))
                                {
                                    atr1tmp["isRD"] = 0;
                                }

                                if (BizFunctions.IsEmpty(row["isDRE"]))
                                {
                                    atr1tmp["isDRE"] = 0;
                                }

                                if (BizFunctions.IsEmpty(row["isOffset"]))
                                {
                                    atr1tmp["isOffset"] = 0;
                                }

                                if (BizFunctions.IsEmpty(row["isUS"]))
                                {
                                    atr1tmp["isUS"] = 0;
                                }

                                if (BizFunctions.IsEmpty(row["isTR"]))
                                {
                                    atr1tmp["isTR"] = 0;
                                }


                                atr1tmp["isadhoc"] = row["isadhoc"];
                                atr1tmp["isOT"] = row["isOT"];
                                atr1tmp["isRD"] = row["isRD"];
                                atr1tmp["isDRE"] = row["isDRE"];
                                atr1tmp["isOffset"] = row["isOffset"];
                                atr1tmp["isUS"] = row["isUS"];
                                atr1tmp["isTR"] = row["isTR"];
                                atr1tmp["TRsitenum"] = row["TRsitenum"];

                                if (row["sitenum"].ToString() != string.Empty)
                                {
                                    atr1tmp["sitenum"] = row["sitenum"];
                                }

                                atr1tmp["rempnum"] = row["rempnum"];
                                atr1tmp["rempname"] = row["rempname"];
                                atr1tmp["rnric"] = row["rnric"];
                                atr1tmp["day"] = row["day"];
                                atr1tmp["date"] = row["date"];
                                atr1tmp["dayofweek"] = TimeTools.GetDayOfWeekNo(row["day"].ToString().Trim());
                                atr1tmp["RepRefKey"] = row["RefKey"];
                                atr1tmp["empnum3"] = row["empnum"];
                                atr1tmp["empnum4"] = row["empnum"];
                                atr1tmp["pguid"] = row["guid"];
                                atr1tmp["paytypecode"] = GetPayTypeCode(row["empnum"].ToString());


                                atr1tmp["remark"] = row["remark"];

                                atr1tmp["attnmark"] = isWork(row["shiftcode"].ToString());


                                atr1tmp["WorkHrs"] = row["WorkHrs"];
                                atr1tmp["OTHrs"] = row["OTHrs"];
                                atr1tmp["OT1"] = row["OT1"];
                                atr1tmp["OT15"] = row["OT15"];
                                atr1tmp["OT2"] = row["OT2"];
                                atr1tmp["FixAllowAmt"] = row["FixAllowAmt"];
                                atr1tmp["AttnRemarks"] = row["AttnRemarks"];
                                atr1tmp["isOmit"] = 0;

                                count = count + 1;
                                atr1.Rows.Add(atr1tmp);

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


        private string GetPayTypeCode(string empnum)
        {
            string paytype = "";

            string getPayType = "Select paytypecode from HEMPHtmp1 where empnum='" + empnum + "'";
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