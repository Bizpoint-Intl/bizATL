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

    class Header_LVR
    {
        protected DBAccess dbAccess = null;
        protected DataSet dataSet = null;
        protected Hashtable formsCollection = null;
        protected Hashtable selectsCollection = null;
        protected string documentKey = null;


        public Header_LVR(DBAccess dbAccess, Hashtable formsCollection, string DocumentKey)
		{
			this.dbAccess			= dbAccess;
			this.dataSet			= this.dbAccess.DataSet;
			this.formsCollection	= formsCollection;
			this.documentKey		= DocumentKey;
			this.selectsCollection  = new Hashtable();


        }

        private DataTable GetLeaveCount()
        {
            DataRow lvr = this.dbAccess.DataSet.Tables["lvr"].Rows[0];

            string str1 = "select designation,lvmnum, SUM(totaldays) as Total from lvr " +
                            "where status='P' and empnum='" + lvr["empnum"].ToString() + "' and designation='" + lvr["designation"].ToString() + "' " +
                            "group by designation,lvmnum";

            this.dbAccess.ReadSQL("dtGetLeaveHistory", str1);

            return this.dbAccess.DataSet.Tables["dtGetLeaveHistory"];
        }



        private DataTable GetLeaveEntitlement()
        {
            DataRow lvr = this.dbAccess.DataSet.Tables["lvr"].Rows[0];
            string str1 = "select lv.designation,lv.lvmnum,lv.noOfdays,ISNULL(lm.isCombinable,0) AS isCombinable,ISNULL(lm.isCombinableWith,'') AS isCombinableWith " +
                          "from lve1 lv left join lvm lm on lv.lvmnum = lm.lvmnum where lv.designation='" + lvr["designation"].ToString().Trim() + "' and lv.[status]<>'V'";

            this.dbAccess.ReadSQL("dtGetLeaveEntitlement", str1);
            return this.dbAccess.DataSet.Tables["dtGetLeaveEntitlement"];
        }

        private void GetLeaveTally()
        {
            DataRow lvr = this.dbAccess.DataSet.Tables["lvr"].Rows[0];
            DataTable lvr1 = this.dbAccess.DataSet.Tables["lvr1"];

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

                }
            }

            TmpCombinableLists.Dispose();
            LeaveCount.Dispose();
            LeaveEntitlement.Dispose();
        }

    }
}
