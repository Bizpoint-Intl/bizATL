#region Namespaces
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;

using BizRAD.BizReport;
using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizCommon;
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;
using PicoGuards.BizModules.UserAuthorization;
using PicoGuards.TimeUtilites;
#endregion

namespace PicoGuards.LVR
{
    class LeaveControl
    {
        protected DBAccess dbAccess = null;
        protected string Designation,Sitenum,SectorCode,Gender = "";
        protected int YearsInService = 0;
        protected decimal RemainningLeaveLastYear, CurrentSalary = 0;
        DateTime joineddate = new DateTime();
        ArrayList al = new ArrayList();

        ArrayList alLeaveRecord = new ArrayList();
        DateTime dt = DateTime.Now;
        


        public LeaveControl(DBAccess dbAccess)
        {
            this.dbAccess = dbAccess;
            
            GetEmployeeData();
            GetYearsLists();                  
            
        }

        private DataTable GetLeaveCount()
        {
            DataRow lvr = dbAccess.DataSet.Tables["lvr"].Rows[0];

            string str1 = "select designation,lvmnum, SUM(totaldays) as Total from lvr " +
                            "where status='P' and empnum='" + lvr["empnum"].ToString() + "' and designation='" + lvr["designation"].ToString() + "' and LeaveYear='" + Convert.ToString(dt.Year) + "' " +
                            "group by designation,lvmnum";

            dbAccess.ReadSQL("dtGetLeaveHistory", str1);

            return dbAccess.DataSet.Tables["dtGetLeaveHistory"];
        }



        private DataTable GetLeaveEntitlement()
        {
            DataRow lvr = dbAccess.DataSet.Tables["lvr"].Rows[0];
            string str1 = "select lv.designation,lv.lvmnum,lv.noOfdays,ISNULL(lm.isCombinable,0) AS isCombinable,ISNULL(lm.isCombinableWith,'') AS isCombinableWith,ISNULL(lm.isCapped,0) as isCapped,ISNULL(lm.iscalculable,0) as isCalculable " +
                          "from lve1 lv left join lvm lm on lv.lvmnum = lm.lvmnum where lv.designation='" + lvr["designation"].ToString().Trim() + "' and lv.[status]<>'V'";

            dbAccess.ReadSQL("dtGetLeaveEntitlement", str1);
            return dbAccess.DataSet.Tables["dtGetLeaveEntitlement"];
        }

        public void GetLeaveTally()
        {
            GetUnusedLeave();

            DataRow lvr = dbAccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = dbAccess.DataSet.Tables["lvr1"];

            DataTable LeaveCount = GetLeaveCount();
            DataTable LeaveEntitlement = GetLeaveEntitlement();
            string lvnum1, lvnum2;
            bool combinable;
            DataTable TmpCombinableLists = new DataTable();
            TmpCombinableLists.Columns.Add("lvenum", typeof(string));
            TmpCombinableLists.Columns.Add("CombinableWith", typeof(string));
            TmpCombinableLists.Columns.Add("DaysToDeduct", typeof(decimal));


            if (lvr1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(lvr1);
            }

            if (LeaveCount.Rows.Count > 0)
            {
                foreach (DataRow dr1 in LeaveCount.Rows)
                {
                    foreach (DataRow dr2 in LeaveEntitlement.Rows)
                    {

                        combinable = (bool)dr2["isCombinable"];

                        lvnum1 = dr1["lvmnum"].ToString();

                        lvnum2 = dr2["lvmnum"].ToString();

                        DataRow Insertlve1 = lvr1.NewRow();
                        if (lvnum1 == lvnum2)
                        {


                            Insertlve1["lvmnum"] = dr1["lvmnum"];

                            if (BizFunctions.IsEmpty(dr2["noofdays"]))
                            {
                                dr2["noofdays"] = 0;
                            }
                            Insertlve1["daysleft"] = Math.Abs(Convert.ToDecimal(dr2["noofdays"]) - Convert.ToDecimal(dr1["Total"]));

                         


                            if (combinable)
                            {

                                TmpCombinableLists.Rows.Add(dr1["lvmnum"].ToString(), dr2["isCombinableWith"].ToString(), Convert.ToDecimal(dr1["Total"]));
                            }
                        }
                        else
                        {
                            Insertlve1["lvmnum"] = dr2["lvmnum"];
                            Insertlve1["daysleft"] = Convert.ToDecimal(dr2["noOfdays"]);
                        }
                        
                        lvr1.Rows.Add(Insertlve1);
                        //DataRow lvr1_lastdr = lvr1.Rows.Add(new object[] { });
                        //lvr1_lastdr["lvmnum"] = "OIL";
                        //lvr1_lastdr["daysleft"] = RemainningLeaveLastYear;

                    }

                }

                if (TmpCombinableLists.Rows.Count > 0)
                {
                    foreach (DataRow dr3 in TmpCombinableLists.Rows)
                    {

                        foreach (DataRow dr4 in lvr1.Rows)
                        {
                            if (dr4.RowState != DataRowState.Deleted)
                            {
                                if (dr3["combinablewith"].ToString() == dr4["lvmnum"].ToString())
                                {
                                    dr4["daysleft"] = Math.Abs(Convert.ToDecimal(dr4["daysleft"]) - Convert.ToDecimal(dr3["daystodeduct"]));
                                }                               
                            }
                        }
                    }                    
                }
            }

            else
            {
                if (LeaveEntitlement.Rows.Count > 0)
                {
                    foreach (DataRow dr5 in LeaveEntitlement.Rows)
                    {
                        DataRow Insertlve1 = lvr1.NewRow();

                        Insertlve1["lvmnum"] = dr5["lvmnum"];
                        Insertlve1["daysleft"] = dr5["noOfdays"];
                        lvr1.Rows.Add(Insertlve1);


                    }

                    DataRow lvr1_lastdr = lvr1.Rows.Add(new object[] { });
                    lvr1_lastdr["lvmnum"] = "OIL";
                    lvr1_lastdr["daysleft"] = RemainningLeaveLastYear;

                }
            }

            TmpCombinableLists.Dispose();
            LeaveCount.Dispose();
            LeaveEntitlement.Dispose();
            
            

       
        }

        private void GetEmployeeData()
        {
            DataRow lvr = dbAccess.DataSet.Tables["lvr"].Rows[0];

            DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(lvr["empnum"].ToString());

            if (tmpEmpData.Rows.Count > 0)
            {
                DataRow dr1 = tmpEmpData.Rows[0];
                Designation = dr1["designation"].ToString();
                Sitenum =  dr1["sitenum"].ToString();
                SectorCode = dr1["sectorcode"].ToString();
                CurrentSalary = Convert.ToDecimal(dr1["CurrentSalary"]);
                joineddate = Convert.ToDateTime(dr1["datejoined"]);
                Gender = dr1["gender"].ToString();
                YearsInService = GetYearsService();

            }
        }

        private int GetYearsService()
        {
            DataRow lvr = dbAccess.DataSet.Tables["lvr"].Rows[0];

            int YearsServed = 0;

            string str1 = "select datejoined from hemph where empnum='" + lvr["empnum"].ToString() + "'";

            dbAccess.ReadSQL("dtDateJoined", str1);

            if (dbAccess.DataSet.Tables["dtDateJoined"].Rows.Count > 0)
            {
                DataRow drDataJoined = dbAccess.DataSet.Tables["dtDateJoined"].Rows[0];

                YearsServed = TimeTools.CalculateYears(Convert.ToDateTime(drDataJoined["datejoined"]), DateTime.Today);

            }

            return YearsServed;
        }

    

        private void GetYearsLists()
        {
            DateTime GetYear = dt;
            GetYear = GetYear.AddYears(-1);      
            //for (int i = 0; i < 1; i++)
            //{

            //    GetYear = GetYear.AddYears(1);
                al.Add(GetYear.Year);                            
            //}
        }



        private void GetUnusedLeave()
        {
            if (al.Count > 0)
            {
                
                for (int y = 0; y < al.Count; y++)
                {
                  
                    ///
                    DataTable YearLeaveTable = GetYearlyLeaveTally(al[y].ToString());

                    YearLeaveTable.TableName = "dt"+al[y].ToString();

                    foreach (DataRow dr1 in YearLeaveTable.Rows)
                    {
                        if (BizFunctions.IsEmpty(dr1["iscapped"]))
                        {
                            dr1["iscapped"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["iscalculable"]))
                        {
                            dr1["iscalculable"] = 0;
                        }

                        if (dr1["lvmnum"].ToString() =="REST")
                        {
                            RemainningLeaveLastYear = RemainningLeaveLastYear + Convert.ToDecimal(dr1["daysleft"]);
                        }
                    }
                  
                }
            }
        }


        private DataTable GetYearlyLeaveTally(string Yr)
        {
            DataRow lvr = dbAccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = dbAccess.DataSet.Tables["lvr1"];
            DataTable lvrx = lvr1.Copy();


            if (!dbAccess.DataSet.Tables.Contains("lvr1x"))
            {
                lvrx.TableName = "LVRX";
                //lvr1x.TableName = "xPFMSR";
                //this.dbaccess.DataSet.Tables.Add(xlvr1x.Copy());

                if(!lvrx.Columns.Contains("isCapped"))
                {
                    lvrx.Columns.Add("isCapped", typeof(bool));
                }

                if (!lvrx.Columns.Contains("iscalculable"))
                {
                    lvrx.Columns.Add("iscalculable", typeof(bool));
                }
            }
            

            DataTable LeaveCount = GetYearLeaveCountAvailable(Yr);
            DataTable LeaveEntitlement = GetLeaveEntitlement();
            string lvnum1, lvnum2;
            bool combinable;
            DataTable TmpCombinableLists = new DataTable();
            TmpCombinableLists.Columns.Add("lvenum", typeof(string));
            TmpCombinableLists.Columns.Add("CombinableWith", typeof(string));
            TmpCombinableLists.Columns.Add("DaysToDeduct", typeof(decimal));
            TmpCombinableLists.Columns.Add("isCapped", typeof(bool));
            TmpCombinableLists.Columns.Add("iscalculable", typeof(bool));


            if (lvrx.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(lvrx);
            }

            if (LeaveCount.Rows.Count > 0)
            {
                foreach (DataRow dr1 in LeaveCount.Rows)
                {
                    foreach (DataRow dr2 in LeaveEntitlement.Rows)
                    {

                        combinable = (bool)dr2["isCombinable"];

                        lvnum1 = dr1["lvmnum"].ToString();

                        lvnum2 = dr2["lvmnum"].ToString();

                        DataRow Insertlve1 = lvrx.NewRow();
                        if (lvnum1 == lvnum2)
                        {


                            Insertlve1["lvmnum"] = dr1["lvmnum"];
                            Insertlve1["daysleft"] = Math.Abs(Convert.ToDecimal(dr2["noofdays"]) - Convert.ToDecimal(dr1["Total"]));


                            if (combinable)
                            {

                                TmpCombinableLists.Rows.Add(dr1["lvmnum"].ToString(), dr2["isCombinableWith"].ToString(), Convert.ToDecimal(dr1["Total"]));
                            }
                        }
                        else
                        {
                            Insertlve1["lvmnum"] = dr2["lvmnum"];
                            if(BizFunctions.IsEmpty(dr2["noOfdays"]))
                            {
                                dr2["noOfdays"] = 0;
                            }
                            Insertlve1["daysleft"] = Convert.ToDecimal(dr2["noOfdays"]);
                        }
                        Insertlve1["isCapped"] = dr2["isCapped"];
                        Insertlve1["iscalculable"] = dr2["iscalculable"];
                        lvrx.Rows.Add(Insertlve1);

                    }

                }

                if (TmpCombinableLists.Rows.Count > 0)
                {
                    foreach (DataRow dr3 in TmpCombinableLists.Rows)
                    {

                        foreach (DataRow dr4 in lvrx.Rows)
                        {
                            if (dr4.RowState != DataRowState.Deleted)
                            {
                                if (dr3["combinablewith"].ToString() == dr4["lvmnum"].ToString())
                                {
                                    dr4["daysleft"] = Math.Abs(Convert.ToDecimal(dr4["daysleft"]) - Convert.ToDecimal(dr3["daystodeduct"]));
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                if (LeaveEntitlement.Rows.Count > 0)
                {
                    foreach (DataRow dr5 in LeaveEntitlement.Rows)
                    {
                        DataRow Insertlve1 = lvrx.NewRow();

                        Insertlve1["lvmnum"] = dr5["lvmnum"];
                        Insertlve1["daysleft"] = dr5["noOfdays"];
                        Insertlve1["isCapped"] = dr5["isCapped"];
                        Insertlve1["iscalculable"] = dr5["iscalculable"];
                        lvrx.Rows.Add(Insertlve1);
                    }

                }
            }


            return lvrx;

        }

        private DataTable GetYearLeaveCountAvailable(string Year)
        {
            DataRow lvr = dbAccess.DataSet.Tables["lvr"].Rows[0];

            string str1 = "select designation,lvmnum, SUM(totaldays) as Total from lvr " +
                            "where status='P' and empnum='" + lvr["empnum"].ToString() + "' and designation='" + lvr["designation"].ToString() + "' and LeaveYear='" + Year + "' " +
                            "group by designation,lvmnum";

            dbAccess.ReadSQL("dtGetLeaveHistory", str1);

            return dbAccess.DataSet.Tables["dtGetLeaveHistory"];
        }





        public int YearsServiceInfo
        {
            get
            {
                return YearsInService;
            }
            set
            {
                YearsInService = value; ;
            }
        }

        public string DesignationInfo
        {
            get
            {
                return Designation;
            }
            set
            {
                Designation = value;
            }
        }

        public string SitenumInfo
        {
            get
            {
                return Sitenum;
            }
            set
            {
                Sitenum = value;
            }
        }

        public string SectorCodeInfo
        {
            get
            {
                return SectorCode;
            }
            set
            {
                SectorCode = value;
            }
        }

        public decimal CurrentSalaryInfo
        {
            get
            {
                return CurrentSalary;
            }
            set
            {
                CurrentSalary = value;
            }
        }

        public string GenderInfo
        {
            get
            {
                return Gender;
            }
            set
            {
               Gender  = value;
            }
        }



     
    }
}
