using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizTools;
using BizRAD.BizAccounts;


namespace ATL.Schedule
{
    class ScheduleControl
    {
        DBAccess dbAccess = null;
        string Refnum,Refnum2,Type,Module,str1,str2,str3="";
        DataTable ScheduleTable,matnumCount;


        public ScheduleControl(string refnum,string type, string module)
        {
            this.Refnum = refnum;
            this.Type = type;
            this.Module=module;
            this.dbAccess = new DBAccess();
            
            getSchedule();
        }

        private void getSchedule()
        {
            if (Module == "SITMT")
            {
                if (Type == "CTR")
                {
                    SetQuery();
                    ExecuteQuery(str1);
                }
                if (Type == "ADH")
                {
                    SetQuery();
                    ExecuteQuery(str2);
                }
            }
            if (Module == "WRR")
            {
                Type = GetRefnum();
                if (Type != string.Empty)
                {
                    if (Type == "CTR")
                    {
                        SetQuery();
                        ExecuteQuery(str1);
                    }
                    if (Type == "ADH")
                    {
                        SetQuery();
                        ExecuteQuery(str2);
                    }
                }
            }

    
        }

        private string GetRefnum()
        {
            string TmpStr = "";
            str3=  "select docunum, SUBSTRING(docunum,1,3) as tablename from SITMT where sitenum in "+
                   "( "+		
	                    "select sitenum from wrr where refnum='" + Refnum + "' "+
                    ")";

            this.dbAccess.ReadSQL("tmpGetRefnum", str3);

            if (this.dbAccess.DataSet.Tables["tmpGetRefnum"].Rows.Count > 0)
            {
                DataRow dr1 = this.dbAccess.DataSet.Tables["tmpGetRefnum"].Rows[0];

                TmpStr = dr1["tablename"].ToString();
                Refnum = dr1["docunum"].ToString();

            }
            return TmpStr;
        }

        private void SetQuery()
        {
            /////////////////  ///////////////// For Query of Contract Refnums  /////////////////  /////////////////
            str1 = "Select shiftcode, matnum,Total,[day] from " +
                               "( " +
                                   "Select shiftcode, matnum, sum(monday) as monday, sum(tuesday) as tuesday, sum(wednesday) as wednesday, sum(thursday) as thursday, " +
                                    "sum(friday) as friday, sum(saturday) as saturday, sum(sunday) as sunday " +
                                       "from ( " +
                                               "Select shiftcode,matnum, " +
                                                   "CASE when monday=1 then (monday*qty) else 0 end as monday, " +
                                                   "CASE when tuesday=1 then (tuesday*qty) else 0 end as tuesday, " +
                                                   "CASE when wednesday=1 then (wednesday*qty) else 0 end as wednesday, " +
                                                   "CASE when thursday=1 then (thursday*qty) else 0 end as thursday, " +
                                                   "CASE when friday=1 then (friday*qty) else 0 end as friday, " +
                                                   "CASE when saturday=1 then (saturday*qty) else 0 end as saturday, " +
                                                   "CASE when sunday=1 then (sunday*qty) else 0 end as sunday " +
                                                       "from ( " +
                                                               "select " +
                                                                     "shiftcode, " +
                                                                     "matnum, " +
                                                                     "ISNULL(officerqty,0) as qty, " +
                                                                     "ISNULL(monday,0) as monday, " +
                                                                     "ISNULL(tuesday,0) as tuesday, " +
                                                                     "ISNULL(wednesday,0) as wednesday, " +
                                                                     "ISNULL(thursday,0) as thursday, " +
                                                                     "ISNULL(friday,0) as friday, " +
                                                                     "ISNULL(saturday,0)as saturday, " +
                                                                     "ISNULL(sunday,0) as sunday " +
                                                               "from ctr1 where refnum='" + Refnum + "' and [status]<>'V' " +

                                                               "UNION ALL " +

                                                               "select " +
                                                                   "shiftcode, " +
                                                                   "matnum, " +
                                                                   "ISNULL(officerqty,0) as qty, " +
                                                                   "ISNULL(monday,0) as monday, " +
                                                                   "ISNULL(tuesday,0) as tuesday, " +
                                                                   "ISNULL(wednesday,0) as wednesday, " +
                                                                   "ISNULL(thursday,0) as thursday, " +
                                                                   "ISNULL(friday,0) as friday, " +
                                                                   "ISNULL(saturday,0)as saturday,ISNULL(sunday,0) as sunday " +
                                                                "from adh1 where ctrnum='" + Refnum + "' and [status]<>'V' " +
                                                ")a  " +
                                   ")b " +
                                   "group by shiftcode,matnum " +
                               ") as p " +
                               "UNPIVOT " +
                               "( TOTAL for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) )unP";


            /////////////////  ///////////////// For Query of Adhocs Only - Not an Adhoc of any Contract  /////////////////  /////////////////
            str2 = "Select shiftcode, matnum,Total,[day] from " +
                                 "( " +
                                     "Select shiftcode, matnum, sum(monday) as monday, sum(tuesday) as tuesday, sum(wednesday) as wednesday, sum(thursday) as thursday, " +
                                      "sum(friday) as friday, sum(saturday) as saturday, sum(sunday) as sunday " +
                                         "from ( " +
                                                 "Select shiftcode,matnum, " +
                                                     "CASE when monday=1 then (monday*qty) else 0 end as monday, " +
                                                     "CASE when tuesday=1 then (tuesday*qty) else 0 end as tuesday, " +
                                                     "CASE when wednesday=1 then (wednesday*qty) else 0 end as wednesday, " +
                                                     "CASE when thursday=1 then (thursday*qty) else 0 end as thursday, " +
                                                     "CASE when friday=1 then (friday*qty) else 0 end as friday, " +
                                                     "CASE when saturday=1 then (saturday*qty) else 0 end as saturday, " +
                                                     "CASE when sunday=1 then (sunday*qty) else 0 end as sunday " +
                                                         "from ( " +
                                                                 "select " +
                                                                     "shiftcode, " +
                                                                     "matnum, " +
                                                                     "ISNULL(officerqty,0) as qty, " +
                                                                     "ISNULL(monday,0) as monday, " +
                                                                     "ISNULL(tuesday,0) as tuesday, " +
                                                                     "ISNULL(wednesday,0) as wednesday, " +
                                                                     "ISNULL(thursday,0) as thursday, " +
                                                                     "ISNULL(friday,0) as friday, " +
                                                                     "ISNULL(saturday,0)as saturday,ISNULL(sunday,0) as sunday " +
                                                                  "from adh1 where refnum='" + Refnum + "' " +
                                                  ")a  " +
                                     ")b " +
                                     "group by shiftcode,matnum " +
                                 ") as p " +
                                 "UNPIVOT " +
                                 "( TOTAL for [day] in (MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY) )unP";


        }

        private void ExecuteQuery(string strQuery)
        {
            this.dbAccess.ReadSQL("SCHEDULE", strQuery);

            if (this.dbAccess.DataSet.Tables["SCHEDULE"].Rows.Count > 0)
            {
                ScheduleTable = this.dbAccess.DataSet.Tables["SCHEDULE"].Copy();
					
				 string Countmatnum = "Select matnum,SUM(qty) as Total From "+
								            "( "+
									            "select "+											 
											            "matnum, "+
											            "ISNULL(officerqty,0) as qty "+
                                                "from ctr1 where refnum='" + Refnum + "' and [status]<>'V'  " +
            										
										            "UNION ALL  "+
            										
										            "select "+
											            "matnum, "+
											            "ISNULL(officerqty,0) as qty	"+									
										         "from adh1 where ctrnum='" + Refnum + "' and [status]<>'V' "+
								            ")A Group by matnum";


                 this.dbAccess.ReadSQL("matnumCount", Countmatnum);
                 matnumCount = this.dbAccess.DataSet.Tables["matnumCount"];

            }

            this.dbAccess.DataSet.Tables["SCHEDULE"].Dispose();
        }


        public DataTable ScheduleInfo
        {
            get
            {
                return ScheduleTable;
            }
            set
            {
                ScheduleTable = value;
            }
        }

        public DataTable matnumCountInfo
        {
            get
            {
                return matnumCount;
            }
            set
            {
                matnumCount = value;
            }
        } 
    }
}
