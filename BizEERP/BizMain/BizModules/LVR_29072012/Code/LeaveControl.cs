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
        protected string EmpNum, matnum, sitenumi, SectorCode, Gender, StatusCode = "";
        protected int YearsInService = 0;
        protected decimal RemainningLeaveLastYear, CurrentSalary = 0;
        DateTime joineddate = new DateTime();
        DataTable ALmisc = new DataTable();
        ArrayList al = new ArrayList();

        ArrayList alLeaveRecord = new ArrayList();
        DateTime dt = new DateTime();
        


        public LeaveControl(string empnum, DateTime DateTaken)
        {
            this.dbAccess = new DBAccess();
            this.EmpNum = empnum;
            this.dt = DateTaken;
            GetEmployeeData();    
            
        }

   



        private DataTable GetLeaveEntitlement(string year)
        {

            string str1 = "select lv.matnum,lv.lvmnum,lv.noOfdays,ISNULL(lm.isCombinable,0) AS isCombinable,ISNULL(lm.isCombinableWith,'') AS isCombinableWith,ISNULL(lm.isCapped,0) as isCapped,ISNULL(lm.iscalculable,0) as isCalculable, ISNULL(AllowBroughtForward,0) as AllowBroughtForward,BroughtForwardTo " +
                          "from lve1 lv left join lvm lm on lv.lvmnum = lm.lvmnum where lv.matnum='" + matnum + "' and lv.[status]<>'V'";

            decimal value = 0;

            dbAccess.ReadSQL("dtGetLeaveEntitlement", str1);
            DataTable dtGetLeaveEntitlement = dbAccess.DataSet.Tables["dtGetLeaveEntitlement"];

            if(dbAccess.DataSet.Tables.Contains("LeaveEntitlement" + year))
            {
                dbAccess.DataSet.Tables["LeaveEntitlement" + year].Dispose();
            }

            dtGetLeaveEntitlement.TableName = "LeaveEntitlement" + year;

            //To change
            //int AdditionalLeave = GetYearsService(Convert.ToInt32(year));
            int AdditionalLeave = GetYearsService(dt);



            string GetLvm1 = "Select * from LVM where [status]<>'V'";

            this.dbAccess.ReadSQL("tmpLVM1", GetLvm1);

            string GetLvm2 = "Select * from LVM where 1=2";
             this.dbAccess.ReadSQL("tmpLVM2", GetLvm2);
            DataTable tmpLvm2 = this.dbAccess.DataSet.Tables["tmpLVM2"];

            if (this.dbAccess.DataSet.Tables["tmpLVM1"].Rows.Count > 0)
            {
                foreach (DataRow dr1 in dbAccess.DataSet.Tables["tmpLVM1"].Rows)
                {
                    if (BizFunctions.IsEmpty(dr1["isincremental"]))
                    {
                        dr1["isincremental"] = 0;
                    }
                    if ((bool)dr1["isincremental"])
                    {
                        tmpLvm2.ImportRow(dr1);
                    }
                }
            }

            foreach (DataRow dr2 in dtGetLeaveEntitlement.Rows)
            {
                foreach (DataRow dr3 in tmpLvm2.Rows)
                {
                    if (dr2["lvmnum"].ToString() == dr3["lvmnum"].ToString())
                    {
                        decimal tmpTotal = Convert.ToDecimal(dr2["noofdays"]) + (Convert.ToDecimal(AdditionalLeave) * Convert.ToDecimal(dr3["yearlyincrementalvalue"]));

                        if(BizFunctions.IsEmpty(dr3["isCapped"]))
                        {
                            dr3["isCapped"] = 0;
                        }
                        if((bool)dr3["isCapped"])
                        {
                             if(tmpTotal > Convert.ToDecimal(dr3["isCappedValue"]))
                             {
                                 tmpTotal = Convert.ToDecimal(dr3["isCappedValue"]);
                             }
                        }

                        dr2["noofdays"] = tmpTotal;
                    }
                }
            }

            return dtGetLeaveEntitlement;
        }

       


        public DataTable GetLeaveTally()
        {
            string strLvr1 = "Select * from lvr1 where 1=2";
            this.dbAccess.ReadSQL("LVR1", strLvr1);

            DataTable lvr1 = this.dbAccess.DataSet.Tables["LVR1"];

            if (StatusCode.Trim() == "ACTIVE")
            {

            

                DataTable LeaveCount = GetYearLeaveCount(Convert.ToString(dt.Year)); // Get Current Leave Utilized
                DataTable LeaveEntitlement = GetLeaveEntitlement(Convert.ToString(dt.Year));  //Get Current Leave Entitlement
                DataTable RemainningLeave = GetUnusedLeave();
                string lvnum1, lvnum2;
                bool combinable, iscapped;
                DataTable TmpCombinableLists = new DataTable();
                TmpCombinableLists.Columns.Add("lvenum", typeof(string));
                TmpCombinableLists.Columns.Add("CombinableWith", typeof(string));
                TmpCombinableLists.Columns.Add("DaysToDeduct", typeof(decimal));


                if (lvr1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(lvr1);
                }

                foreach (DataRow dr1 in LeaveEntitlement.Rows)
                {
                    foreach (DataRow dr2 in LeaveCount.Rows)
                    {
                        if (BizFunctions.IsEmpty(dr1["isCombinable"]))
                        {
                            dr1["isCombinable"] = 0;
                        }
                        combinable = (bool)dr1["isCombinable"];
                        if (BizFunctions.IsEmpty(dr1["iscapped"]))
                        {
                            dr1["iscapped"] = 0;
                        }
                        iscapped = (bool)dr1["iscapped"];

                        lvnum1 = dr1["lvmnum"].ToString();

                        lvnum2 = dr2["lvmnum"].ToString();

                        if (lvnum1 == lvnum2)
                        {

                            if (BizFunctions.IsEmpty(dr1["noOfdays"]))
                            {
                                dr1["noOfdays"] = 0;
                            }
                            if (BizFunctions.IsEmpty(dr2["Total"]))
                            {
                                dr2["Total"] = 0;
                            }


                            dr1["noOfdays"] = (Convert.ToDecimal(dr1["noOfdays"]) - Convert.ToDecimal(dr2["Total"]));



                            if (combinable)
                            {

                                TmpCombinableLists.Rows.Add(dr2["lvmnum"].ToString(), dr1["isCombinableWith"].ToString(), Convert.ToDecimal(dr2["Total"]));
                            }

                        }
                    }
                }

                foreach (DataRow dr5 in LeaveEntitlement.Rows)
                {
                    DataRow InsertLvr1 = lvr1.NewRow();
                    InsertLvr1["lvmnum"] = dr5["lvmnum"];
                    InsertLvr1["daysleft"] = dr5["noofdays"];
                    lvr1.Rows.Add(InsertLvr1);
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

                if (RemainningLeave.Rows.Count > 0) 
                {
                    foreach (DataRow dr5 in RemainningLeave.Rows)
                    {
                        foreach (DataRow dr6 in lvr1.Rows)
                        {
                            if (dr6.RowState != DataRowState.Deleted)
                            {
                                if (dr5["lvmnum"].ToString() == dr6["lvmnum"].ToString())
                                {
                                    if (BizFunctions.IsEmpty(dr6["daysleft"]))
                                    {
                                        dr6["daysleft"] = 0;
                                    }
                                    if (BizFunctions.IsEmpty(dr5["daysleft"]))
                                    {
                                        dr5["daysleft"] = 0;
                                    }
                                    dr6["daysleft"] = Convert.ToDecimal(dr6["daysleft"]) + Convert.ToDecimal(dr5["daysleft"]);
                                }
                            }
                        }
                    }

                }

                TmpCombinableLists.Dispose();
                LeaveCount.Dispose();
                LeaveEntitlement.Dispose();

            }
            else if (StatusCode.Trim() == "PROBATION")
            {
              lvr1 =  ProbationLeave();
            }
            if (ALmisc != null)
            {
                ALmisc.Clear();
            }
            if (ALmisc.Columns.Contains("lvmnum"))
            {
                ALmisc.Columns.Remove("lvmnum");
            }
            if (ALmisc.Columns.Contains("daysleft"))
            {
                ALmisc.Columns.Remove("daysleft");
            }
            if (ALmisc.Columns.Contains("year"))
            {
                ALmisc.Columns.Remove("year");
            }
            if (ALmisc.Columns.Contains("remark"))
            {
                ALmisc.Columns.Remove("remark");
            }
            ALmisc.Columns.Add("lvmnum", typeof(string));
            ALmisc.Columns.Add("daysleft", typeof(int));
            ALmisc.Columns.Add("year", typeof(string));
            ALmisc.Columns.Add("remark", typeof(string));

            foreach (DataRow drlv in lvr1.Rows) // continue here
            {
                if (drlv.RowState != DataRowState.Deleted)
                {
                    if (drlv["lvmnum"].ToString() == "AL")
                    {
                        DataRow insertTmp = ALmisc.NewRow();
                        insertTmp["lvmnum"] = drlv["lvmnum"];

                        decimal test = Convert.ToDecimal(9 + 14 / 12 * dt.Month);

                        insertTmp["daysleft"] = Convert.ToString(Convert.ToInt32(drlv["daysleft"]) / 12 * dt.Month);
                        insertTmp["year"] = dt.Year.ToString();
                        insertTmp["remark"] = "LEAVE ENTITLEMENT FOR CURRENT MONTH " + dt.Month.ToString() + " ";
                        ALmisc.Rows.Add(insertTmp);
                    }
                }
            }

            //foreach (DataRow drlv in lvr1.Rows)
            //{
            //    if (drlv.RowState != DataRowState.Deleted)
            //    {
            //        if (drlv["lvmnum"].ToString() == "AL")
            //        {
            //            DataRow insertTmp = ALmisc.NewRow();
            //            insertTmp["lvmnum"] = drlv["lvmnum"];
            //            insertTmp["daysleft"] = drlv["daysleft"];
            //            insertTmp["year"] = Yr;
            //            insertTmp["remark"] = "LEAVE ENTITLEMENT FOR YEAR " + Yr + " ";
            //            ALmisc.Rows.Add(insertTmp);
            //        }
            //    }
            //}



            return lvr1;
       
        }

        private void GetEmployeeData()
        {
            DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(EmpNum);

            if (tmpEmpData.Rows.Count > 0)
            {
                DataRow dr1 = tmpEmpData.Rows[0];
                matnum = dr1["matnum"].ToString();
                sitenumi =  dr1["sitenumi"].ToString();
                SectorCode = dr1["sectorcode"].ToString();
                if (BizFunctions.IsEmpty(dr1["CurrentSalary"]))
                {
                    dr1["CurrentSalary"] = 0;
                }
                CurrentSalary = Convert.ToDecimal(dr1["CurrentSalary"]);
                joineddate = Convert.ToDateTime(dr1["datejoined"]);
                Gender = dr1["gender"].ToString();
                StatusCode = dr1["statuscode"].ToString();
                //YearsInService = GetYearsService();

            }
        }


        //private int GetYearsService(int year)
        //{           
        //    int YearsServed = year - joineddate.Year;
   
        //    return YearsServed;
        //}

        private int GetYearsService(DateTime CurrentDateTime)
        {
            int YearServed = TimeTools.CalculateYears(joineddate,CurrentDateTime);

            return YearServed;
        }

    


        private DataTable GetUnusedLeave()
        {
            DateTime GetYear = dt;
            GetYear = GetYear.AddYears(-1);

            DataTable YearLeaveTable = GetPreviousYearLeaveTally(GetYear.Year.ToString());

            YearLeaveTable.TableName = "dt" + GetYear.Year.ToString();

            return YearLeaveTable;
        }

        //Get previous Year Leave
        private DataTable GetPreviousYearLeaveTally(string Yr)
        {

            string strLvr = "Select * from lvr1 where 1=2";
            this.dbAccess.ReadSQL("lvr1x", strLvr);
            DataTable lvr1x = this.dbAccess.DataSet.Tables["lvr1x"];

                          
                lvr1x.TableName = "LVR1X";


                if(!lvr1x.Columns.Contains("isCapped"))
                {
                    lvr1x.Columns.Add("isCapped", typeof(bool));
                }

                if (!lvr1x.Columns.Contains("iscalculable"))
                {
                    lvr1x.Columns.Add("iscalculable", typeof(bool));
                }
                if (!lvr1x.Columns.Contains("AllowBroughtForward"))
                {
                    lvr1x.Columns.Add("AllowBroughtForward", typeof(bool));
                }

                if (!lvr1x.Columns.Contains("AllowBroughtForwardTo"))
                {
                    lvr1x.Columns.Add("AllowBroughtForwardTo", typeof(string));
                }

                if (!lvr1x.Columns.Contains("iscombinable"))
                {
                    lvr1x.Columns.Add("iscombinable", typeof(bool));
                }

                if (!lvr1x.Columns.Contains("iscombinablewith"))
                {
                    lvr1x.Columns.Add("iscombinablewith", typeof(string));
                }

                if (!lvr1x.Columns.Contains("remark"))
                {
                    lvr1x.Columns.Add("remark", typeof(string));
                }

       


            DataTable LeaveCount = GetYearLeaveCount(Yr);
            DataTable LeaveEntitlement = GetLeaveEntitlement(Yr);
            string lvnum1, lvnum2, BroughtForwardTo = "";
            decimal BroughtFowardDays = 0;
            bool combinable, allowBroughtForward, iscapped;
            DataTable TmpCombinableLists = new DataTable();
            TmpCombinableLists.Columns.Add("lvenum", typeof(string));
            TmpCombinableLists.Columns.Add("CombinableWith", typeof(string));
            TmpCombinableLists.Columns.Add("DaysToDeduct", typeof(decimal));
            TmpCombinableLists.Columns.Add("isCapped", typeof(bool));
            TmpCombinableLists.Columns.Add("iscalculable", typeof(bool));



            DataTable TmpBroughtForwardLists = new DataTable();
            TmpBroughtForwardLists.Columns.Add("lvenum", typeof(string));
            TmpBroughtForwardLists.Columns.Add("BroughtForwardTo", typeof(string));
            TmpBroughtForwardLists.Columns.Add("DaysLeft", typeof(decimal));
            TmpBroughtForwardLists.Columns.Add("AllowBroughtForward", typeof(bool));


            if (lvr1x.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(lvr1x);
            }

            
                foreach (DataRow dr1 in LeaveEntitlement.Rows)
                {
                    if (LeaveCount.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in LeaveCount.Rows)
                        {

                            if (BizFunctions.IsEmpty(dr1["iscapped"]))
                            {
                                dr1["iscapped"] = 0;
                            }
                            iscapped = (bool)dr1["iscapped"];


                            if (BizFunctions.IsEmpty(dr1["isCombinable"]))
                            {
                                dr1["isCombinable"] = 0;
                            }
                            combinable = (bool)dr1["isCombinable"];

                            if (BizFunctions.IsEmpty(dr1["AllowBroughtForward"]))
                            {
                                dr1["AllowBroughtForward"] = 0;
                            }
                            allowBroughtForward = (bool)dr1["AllowBroughtForward"];

                            lvnum1 = dr1["lvmnum"].ToString();

                            lvnum2 = dr2["lvmnum"].ToString();

                            if (lvnum1 == lvnum2)
                            {
                                if (BizFunctions.IsEmpty(dr1["noofdays"]))
                                {
                                    dr1["noofdays"] = 0;
                                }
                                if (BizFunctions.IsEmpty(dr2["total"]))
                                {
                                    dr2["total"] = 0;
                                }
                                dr1["noofdays"] = (Convert.ToDecimal(dr1["noofdays"]) - Convert.ToDecimal(dr2["total"]));

                                if (combinable)
                                {

                                    TmpCombinableLists.Rows.Add(dr2["lvmnum"].ToString(), dr1["isCombinableWith"].ToString(), Convert.ToDecimal(dr2["Total"]));
                                }

                                if (allowBroughtForward)
                                {
                                    TmpBroughtForwardLists.Rows.Add(dr1["lvmnum"].ToString(), dr1["BroughtForwardTo"].ToString(), Convert.ToDecimal(dr1["noofdays"]), (bool)dr1["AllowBroughtForward"]);

                                }
                            }

                        }
                    }
                  

                }

                foreach (DataRow dr5 in LeaveEntitlement.Rows)
                {
                    DataRow InsertLvr1 = lvr1x.NewRow();
                    InsertLvr1["lvmnum"] = dr5["lvmnum"];
                    InsertLvr1["daysleft"] = dr5["noofdays"];
                    InsertLvr1["AllowBroughtForward"] = dr5["AllowBroughtForward"];
                    InsertLvr1["AllowBroughtForwardTo"] = dr5["BroughtForwardTo"];
                    InsertLvr1["iscombinable"] = dr5["iscombinable"];
                    InsertLvr1["iscombinablewith"] = dr5["iscombinablewith"];
                    lvr1x.Rows.Add(InsertLvr1);
                }

                if (TmpCombinableLists.Rows.Count > 0) 
                {
                    foreach (DataRow dr3 in TmpCombinableLists.Rows)
                    {

                        foreach (DataRow dr4 in lvr1x.Rows)
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


                //if (TmpBroughtForwardLists.Rows.Count > 0) // here
                //{

                //    foreach (DataRow dr5 in TmpBroughtForwardLists.Rows)
                //    {
                //        foreach (DataRow dr6 in lvr1x.Rows)
                //        {
                //            if (dr6.RowState != DataRowState.Deleted)
                //            {
                //                if (dr5["BroughtForwardTo"].ToString() == dr6["lvmnum"].ToString())                                   
                //                {
                //                    if (BizFunctions.IsEmpty(dr5["daysleft"]))
                //                    {
                //                        dr5["daysleft"] = 0;
                //                    }
                //                    if (Convert.ToDecimal(dr5["daysleft"]) < 0)
                //                    {
                //                        dr5["daysleft"] = 0;
                //                    }

                //                    if (BizFunctions.IsEmpty(dr6["daysleft"]))
                //                    {
                //                        dr6["daysleft"] = 0;
                //                    }
                //                    if (Convert.ToDecimal(dr6["daysleft"]) < 0)
                //                    {
                //                        dr6["daysleft"] = 0;
                //                    }
                                  
                //                    dr6["daysleft"] = Convert.ToDecimal(dr6["daysleft"]) + Convert.ToDecimal(dr5["daysleft"]);
                //                }
                //            }
                //        }
                //    }
                //}
            


                DataTable tmpLvrx = lvr1x.Copy();
                
                tmpLvrx.TableName = "tmpLvrx";

                if (TmpBroughtForwardLists.Rows.Count > 0) // here
                {
                    BizFunctions.DeleteAllRows(tmpLvrx);
                    foreach (DataRow dr7 in TmpBroughtForwardLists.Rows)
                    {

                        foreach (DataRow dr8 in lvr1x.Rows)
                        {
                            if (dr8.RowState != DataRowState.Deleted)
                            {
                                if (dr7["BroughtForwardTo"].ToString() == dr8["lvmnum"].ToString())
                                {

                                    DataRow InsertTmpLvrx = tmpLvrx.NewRow();
                                    InsertTmpLvrx["lvmnum"] = dr8["lvmnum"];
                                    InsertTmpLvrx["daysleft"] = dr8["daysleft"];
                                    tmpLvrx.Rows.Add(InsertTmpLvrx);

                                }
                            }
                        }
                    }
                }
                else
                {
                    DataTable xTemp = tmpLvrx.Copy();
                    BizFunctions.DeleteAllRows(xTemp);
                    foreach (DataRow dr9 in tmpLvrx.Rows)
                    {
                        if (dr9.RowState != DataRowState.Deleted)
                        {
                            if(BizFunctions.IsEmpty(dr9["AllowBroughtForwardTo"]))
                            {
                                dr9["AllowBroughtForward"] = 0;
                            }
                            if ((bool)dr9["AllowBroughtForward"])
                            {
                                DataRow InsertxTemp = xTemp.NewRow();

                                InsertxTemp["lvmnum"] = dr9["AllowBroughtForwardTo"];
                                InsertxTemp["daysleft"]  = dr9["daysleft"];

                                xTemp.Rows.Add(InsertxTemp);
                            }
                        }
                    }
                    BizFunctions.DeleteAllRows(tmpLvrx);
                    tmpLvrx = xTemp.Copy();

                }

                if (tmpLvrx.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(lvr1x);
                    lvr1x = tmpLvrx.Copy();
                }
                if (lvr1x.Rows.Count > 0)
                {
                    //if (ALmisc.Columns.Contains("lvmnum"))
                    //{
                    //    ALmisc.Columns.Remove("lvmnum");
                    //}
                    //if (ALmisc.Columns.Contains("daysleft"))
                    //{
                    //    ALmisc.Columns.Remove("daysleft");
                    //}
                    //if (ALmisc.Columns.Contains("year"))
                    //{
                    //    ALmisc.Columns.Remove("year");
                    //}
                    //if (ALmisc.Columns.Contains("remark"))
                    //{
                    //    ALmisc.Columns.Remove("remark");
                    //}
                    //ALmisc.Columns.Add("lvmnum", typeof(string));
                    //ALmisc.Columns.Add("daysleft", typeof(int));
                    //ALmisc.Columns.Add("year", typeof(string));
                    //ALmisc.Columns.Add("remark", typeof(string));

                    //foreach (DataRow drlv in lvr1x.Rows)
                    //{
                    //    if (drlv.RowState != DataRowState.Deleted)
                    //    {
                    //        if (drlv["lvmnum"].ToString() == "AL")
                    //        {
                    //            DataRow insertTmp = ALmisc.NewRow();
                    //            insertTmp["lvmnum"] = drlv["lvmnum"];
                    //            insertTmp["daysleft"] = drlv["daysleft"];
                    //            insertTmp["year"] = Yr;
                    //            insertTmp["remark"] = "LEAVE ENTITLEMENT FOR YEAR "+Yr+" ";
                    //            ALmisc.Rows.Add(insertTmp);
                    //        }
                    //    }
                    //}
                }
                
            return lvr1x;

        }

        private DataTable GetYearLeaveCount(string Year)
        {
            

            string str1 = "select matnum,lvmnum, SUM(totaldays) as Total from lvr " +
                            "where [status]<>'V' and empnum='" + EmpNum + "' and matnum='" + matnum + "' and LeaveYear='" + Year + "' " +
                            "group by matnum,lvmnum";


            dbAccess.ReadSQL("dtGetLeaveHistory", str1);

            DataTable dtLeaveCount = dbAccess.DataSet.Tables["dtGetLeaveHistory"];
            if (dbAccess.DataSet.Tables.Contains(dtLeaveCount.ToString() + Year))
            {
                this.dbAccess.DataSet.Tables[dtLeaveCount.ToString() + Year].Dispose();
            }
           
                dtLeaveCount.TableName = dtLeaveCount.ToString() + Year;
            
            return dtLeaveCount;
        }


        public DataTable ProbationLeave()
        {
            string strLvr = "Select * from lvr1 where 1=2";
            this.dbAccess.ReadSQL("ProbationTable", strLvr);
            DataTable ProbationTable = this.dbAccess.DataSet.Tables["ProbationTable"];



            int days = TimeUtilites.TimeTools.daysTaken(BizFunctions.GetSafeDateString(joineddate), BizFunctions.GetSafeDateString(dt));

            if(days <= 365)
            {
                DataTable ProbationLeave = GetYearLeaveCount(Convert.ToString(dt.Year));
                decimal result1 = Convert.ToDecimal(days) / Convert.ToDecimal(365);
                decimal finalresult = Math.Round((result1 * 7),0);



                if (ProbationLeave.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ProbationLeave.Rows)
                    {
                        if (dr1["lvmnum"].ToString() == "REST")
                        {
                            dr1["total"] = finalresult - Convert.ToDecimal(dr1["total"]);
                        }
                    }


                    foreach (DataRow dr3 in ProbationLeave.Rows)
                    {
                        DataRow InsertProbationTable = ProbationTable.NewRow();
                        InsertProbationTable["lvmnum"] = dr3["lvmnum"];
                        InsertProbationTable["daysleft"] = dr3["total"];
                        ProbationTable.Rows.Add(InsertProbationTable);
                    }
                }
                else
                {
                    DataRow InsertProbation = ProbationLeave.NewRow();

                    InsertProbation["lvmnum"] = "REST";
                    InsertProbation["total"] = finalresult;

                    ProbationLeave.Rows.Add(InsertProbation);

                    foreach (DataRow dr2 in ProbationLeave.Rows)
                    {
                        DataRow InsertProbationTable = ProbationTable.NewRow();
                        InsertProbationTable["lvmnum"] = dr2["lvmnum"];
                        InsertProbationTable["daysleft"] = dr2["total"];
                        ProbationTable.Rows.Add(InsertProbationTable);
                    }
                }

                
            }






            return ProbationTable;
        }


        public int YearsServiceInfo
        {
            get
            {
                return YearsInService;
            }
            set
            {
                YearsInService = value;
            }
        }

        public string StatusCodeInfo
        {
            get
            {
                return StatusCode;
            }
            set
            {
                StatusCode = value;
            }
        }

        public string matnumInfo
        {
            get
            {
                return matnum;
            }
            set
            {
                matnum = value;
            }
        }

        public string sitenumiInfo
        {
            get
            {
                return sitenumi;
            }
            set
            {
                sitenumi = value;
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
