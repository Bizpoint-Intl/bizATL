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
using ATL.SortTable;
using ATL.TimeUtilites;
using ATL.BizModules.TextValidator;
using NodaTime;
#endregion

namespace ATL.DLA
{
    public class Voucher_DLA : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName, TimesheetForm = null;
        protected TextBox dla_sectorcode, dla_day = null;
        protected Button Btn_Sort,Btn_Extract = null;
        protected ComboBox TableColumn = null;
        protected DateTimePicker dla_dladate = null;
        protected Button btnExtract1 = null;
        #endregion

        #region Construct

        public Voucher_DLA(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_DLA.xml", moduleName, voucherBaseHelpers)
        {
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

       

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

        }
        #endregion

        #region Delete On Click

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);

        }
        #endregion

        #region Cancel on Click

        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);

        }

        #endregion

        #region DocumentPage Event
        protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_Edit_Handle(sender, e);
        }

        protected override void Voucher_New_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_New_Handle(sender, e);

        }
        #endregion

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.TimesheetForm = (e.FormsCollection["timesheet"] as Form).Name;

            e.DBAccess.DataSet.Tables["DLA1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_DLA1_ColumnChanged);


            Initialise();

            string GetPAYTM = "SELECT * FROM PAYTM WHERE [status]<>'V'";
            this.dbaccess.ReadSQL("PAYTM",GetPAYTM);

        }

        #endregion

        private void Voucher_DLA1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {           

            switch (e.Column.ColumnName)
            {
                case "confirmedtimein":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]))
                        {
                            if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                            {
                                e.Row["confirmedtimein"] = System.DBNull.Value;
                            }
                        
                        }
                    }
                    break;
                case "confirmedtimeout":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["confirmedtimeout"]))
                        {
                            if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                            {
                                e.Row["confirmedtimeout"] = System.DBNull.Value;
                            }
                        }
                    }
                    break;
            }
        }

        private void Initialise()
        {

            Btn_Sort = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "Btn_Sort") as Button;
            Btn_Extract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Extract") as Button;

            TableColumn = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "TableColumn") as ComboBox;

            Btn_Sort.Click +=new EventHandler(Btn_Sort_Click);
            Btn_Extract.Click +=new EventHandler(Btn_Extract_Click);
            
            //dla_sectorcode = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "dla_sectorcode") as TextBox;
        
            //dla_day = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "dla_day") as TextBox;


            //dla_dladate = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "dla_dladate") as DateTimePicker;

            //btnExtract1 = BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "Btn_Extract") as Button;
            //btnExtract1.Click += new EventHandler(btnExtract1_Click);
            

        }


        private void Btn_Extract_Click(object sender, EventArgs e)
        {
            DataRow dla = this.dbaccess.DataSet.Tables["DLA"].Rows[0];
            DataTable dla1 = this.dbaccess.DataSet.Tables["DLA1"];

            if (!BizFunctions.IsEmpty(dla["Date"]))
            {
                if (MessageBox.Show("This will Reload the Detail Page, any data extracted will be cleared\nContinue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (dla1.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(dla1);
                    }

                    string GetATR1 = "";
                    if ((!BizFunctions.IsEmpty(dla["fromempnum"]) && BizFunctions.IsEmpty(dla["toempnum"])) || (BizFunctions.IsEmpty(dla["fromempnum"]) && !BizFunctions.IsEmpty(dla["toempnum"])))
                    {
                        string empnum = "";

                        if (!BizFunctions.IsEmpty(dla["fromempnum"]))
                        {
                            empnum = dla["fromempnum"].ToString().Trim();
                        }
                        if (!BizFunctions.IsEmpty(dla["toempnum"]))
                        {
                            empnum = dla["toempnum"].ToString().Trim();
                        }

                        GetATR1 = "Select * from ATR1 where [date]='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dla["Date"])) + "'  and status<>'V' and empnum='" + empnum + "' and uniquekey not in (Select uniquekey from DLA1 where [status]<>'V')";

                    }
                    else if (!BizFunctions.IsEmpty(dla["fromempnum"]) || !BizFunctions.IsEmpty(dla["toempnum"]))
                    {
                        string empnum1, empnum2 = "";


                        empnum1 = dla["fromempnum"].ToString().Trim();
                        empnum2 = dla["toempnum"].ToString().Trim();

                        GetATR1 = "Select * from ATR1 where [date]='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dla["Date"])) + "'  and status<>'V' and empnum>='" + empnum1 + "' and empnum<='" + empnum2 + "'  and uniquekey not in (Select uniquekey from DLA1 where [status]<>'V')";


                    }
                    else
                    {
                        GetATR1 = "Select * from ATR1 where [date]='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dla["Date"])) + "'  and status<>'V' and (SUBSTRING(empnum,1,1)='P' or ISNULL(isdre,0)=1 or paytypecode='D') and uniquekey not in (Select uniquekey from DLA1 where [status]<>'V')";
                    }
                    this.dbaccess.ReadSQL("TbATR1", GetATR1);
                    DataTable TbATR1 = this.dbaccess.DataSet.Tables["TbATR1"];
                    if (TbATR1.Rows.Count > 0)
                    {
                        
                        foreach (DataRow dr1 in TbATR1.Select())
                        {
                            if (dr1.RowState != DataRowState.Deleted)
                            {
                                DataRow InsertWkch1 = dla1.NewRow();

                                InsertWkch1["atrnum"] = dr1["refnum"];
                                InsertWkch1["nric"] = dr1["nric"];
                                InsertWkch1["empnum"] = dr1["empnum"];
                                InsertWkch1["empnum2"] = dr1["empnum"];
                                InsertWkch1["empnum3"] = dr1["empnum"];
                                InsertWkch1["shiftcode"] = dr1["shiftcode"];
                                InsertWkch1["sectorcode"] = dr1["sectorcode"];
                                InsertWkch1["sitenum"] = dr1["sitenum"];
                                InsertWkch1["confirmedtimein"] = dr1["confirmedtimein"];
                                InsertWkch1["confirmedtimeout"] = dr1["confirmedtimeout"];
                                InsertWkch1["scheddatein"] = dr1["scheddatein"];
                                InsertWkch1["scheddateout"] = dr1["scheddateout"];
                                InsertWkch1["timein"] = dr1["timein"];
                                InsertWkch1["timeout"] = dr1["timeout"];
                                InsertWkch1["schedtimein"] = dr1["schedtimein"];
                                InsertWkch1["schedtimeout"] = dr1["schedtimeout"];
                                InsertWkch1["rempnum"] = dr1["rempnum"];
                                InsertWkch1["rempname"] = dr1["rempname"];
                                InsertWkch1["rnric"] = dr1["rnric"];
                                InsertWkch1["day"] = dr1["day"];
                                InsertWkch1["date"] = dr1["date"];
                                InsertWkch1["dayofweek"] = dr1["dayofweek"];
                                InsertWkch1["RepRefKey"] = dr1["RepRefKey"];
                                InsertWkch1["paytypecode"] = dr1["paytypecode"];
                                InsertWkch1["LateMins"] = dr1["LateMins"];
                                if (!BizFunctions.IsEmpty(dr1["paytypecode"]))
                                {
                                    InsertWkch1["value"] = GetPayTypeValue(dr1["paytypecode"].ToString());
                                }

                                InsertWkch1["isadhoc"] = dr1["isadhoc"];


                                if (BizFunctions.IsEmpty(dr1["isadhoc"]))
                                {
                                    InsertWkch1["isadhoc"] = 0;
                                }
                                else
                                {
                                    InsertWkch1["isadhoc"] = dr1["isadhoc"];
                                }

                                if (BizFunctions.IsEmpty(dr1["isOT"]))
                                {
                                    InsertWkch1["isOT"] = 0;
                                }
                                else
                                {
                                    InsertWkch1["isOT"] = dr1["isOT"];
                                }


                                if (BizFunctions.IsEmpty(dr1["OTrate"]))
                                {
                                    InsertWkch1["OTrate"] = 0;
                                }
                                else
                                {
                                    InsertWkch1["OTrate"] = dr1["OTrate"];
                                }


                                if (BizFunctions.IsEmpty(dr1["isRD"]))
                                {
                                    InsertWkch1["isRD"] = 0;
                                }
                                else
                                {
                                    InsertWkch1["isRD"] = dr1["isRD"];
                                }


                                if (BizFunctions.IsEmpty(dr1["isDRE"]))
                                {
                                    InsertWkch1["isDRE"] = 0;
                                }
                                else
                                {
                                    InsertWkch1["isDRE"] = dr1["isDRE"];
                                }

                                if (BizFunctions.IsEmpty(dr1["isUS"]))
                                {
                                    InsertWkch1["isUS"] = 0;
                                }

                                else
                                {
                                    InsertWkch1["isUS"] = dr1["isUS"];
                                }

                                InsertWkch1["TotalHrs"] = dr1["TotalHrs"];
                                dla1.Rows.Add(InsertWkch1);
                            }
                        }
                    }
                }

            }
            
        }

   
   
        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
  
        }


        #region DocumentF2

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {           
            base.AddDocumentF3Condition(sender, e);
            DataRow dla = this.dbaccess.DataSet.Tables["DLA"].Rows[0];
            switch (e.ControlName)
            {
                case "dla_dlanum":
                    {
                        //if (!BizFunctions.IsEmpty(dla["dlanum"].ToString()))
                        //{
                        //    GetWrr();
                        //    GetATMR();
                        //}
                    }
                    break;


            }
        }
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "dla_dlanum":
                    {                      
                        e.DefaultCondition = "refnum not in (Select dlanum from dla where status<>'V') and status<>'V'";               
                    }
                    break;

                case "dla_fromempnum":
                    {
                        e.DefaultCondition = "paytypecode='D' and status<>'V'";
                    }
                    break;

                case "dla_toempnum":
                    {
                        e.DefaultCondition = "paytypecode='D' and status<>'V'";
                    }
                    break;
          
            }
        }
        #endregion

        #region DetailF2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            switch (e.MappingName)
            {
               
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            switch (e.MappingName)
            {
          
            }
        }

        #endregion

        #region  btn_Sort_Click
        protected void Btn_Sort_Click(object sender, System.EventArgs e)
        {
            DataTable wkc1 = this.dbaccess.DataSet.Tables["wkc1"];
            TableColumn = (ComboBox)BizXmlReader.CurrentInstance.GetControl(TimesheetForm, "TableColumn");


            string cname = TableColumn.Text.ToString();
            if (cname != "")
            {
                if (wkc1.Rows.Count > 0)
                {
                    SortDT sort = new SortDT(wkc1, cname + " ASC");
                    DataTable returnedfinalextraction = sort.SortedTable();

                    BizFunctions.DeleteAllRows(wkc1);

                    foreach (DataRow dr in returnedfinalextraction.Select())
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            wkc1.ImportRow(dr);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Choose Column To Sort !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

        }
        #endregion


        #region trq ReOpen/void

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);

        }

        protected override void Document_Reopen_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Reopen_OnClick(sender, e);

          
        }

        protected override void Document_Void_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Void_Handle(sender, e);
          
        }

        protected override void Document_Void_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Void_OnClick(sender, e);
       
        }

        #endregion

        #region Duplicate Handle
        protected override void Document_Duplicate_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Duplicate_Handle(sender, e);
            e.Handle = false;
        }
        #endregion

        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            DataRow dla = this.dbaccess.DataSet.Tables["DLA"].Rows[0];
            DataTable wkc1 = this.dbaccess.DataSet.Tables["DLA1"];
            base.Document_SaveBegin_OnClick(sender, e);

            if (BizFunctions.IsEmpty(dla["trandate"]))
            {
                dla["trandate"] = DateTime.Now;
            }

            #region DLA1
            foreach (DataRow dr1 in wkc1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(dla, dr1, "refnum/user/flag/status/trandate/createdby/created/modified");
                }

            }
            #endregion

            foreach (DataRow dr1 in wkc1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if(BizFunctions.IsEmpty(dr1["confirmedtimein"]))
                    {
                        if (!BizFunctions.IsEmpty(dr1["schedtimein"]))
                        {
                            dr1["confirmedtimein"] = dr1["schedtimein"];
                        }                     
                    }

                     if(BizFunctions.IsEmpty(dr1["confirmedtimeout"]))
                    {
                        if (!BizFunctions.IsEmpty(dr1["schedtimeout"]))
                        {
                            dr1["confirmedtimein"] = dr1["schedtimein"];
                        }                       
                    }
                }
            }
           
                                  
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

        }
        #endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);


        }

        #endregion

        #region Preview on Click

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);

        }

        #endregion

        #region Print on Click

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

        }

        #endregion

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);

        }

        #endregion

        private void GetWrr()
        {
            DataRow dla = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            string sqlGetDLA = "Select * from dla where refnum='" + dla["dlanum"].ToString().Trim() + "'";
            this.dbaccess.ReadSQL("TBGetDLA", sqlGetDLA);
            DataTable TBGetDLA = this.dbaccess.DataSet.Tables["TBGetDLA"];

            if (TBGetDLA.Rows.Count > 0)
            {
                DataRow drGetDLA = this.dbaccess.DataSet.Tables["TBGetDLA"].Rows[0];
                dla["sectorcode"] = drGetDLA["sectorcode"]; 
                dla["sitenum"] = drGetDLA["sitenum"];
                dla["dlaFromDate"] = Convert.ToDateTime(drGetDLA["commencedate"]).ToShortDateString();
                dla["dlaToDate"] = Convert.ToDateTime(drGetDLA["enddate"]).ToShortDateString();
           
            }
        }

        private void GetATMR()
        {
            DataRow dla = this.dbaccess.DataSet.Tables["DLA"].Rows[0];
            DataTable wkc1 = this.dbaccess.DataSet.Tables["DLA1"];
            if (wkc1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wkc1);
            }
            string sqlGetATR = "Select * from atmr where refnum='" + dla["dlanum"].ToString().Trim() + "'";

            this.dbaccess.ReadSQL("TBGetATMR", sqlGetATR);
            DataTable TBGetATMR = this.dbaccess.DataSet.Tables["TBGetATMR"];
            if (TBGetATMR.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TBGetATMR.Select())
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertAtr1 = wkc1.NewRow();

                        InsertAtr1["nric"] = dr1["nric"];
                        InsertAtr1["empnum"] = dr1["empnum"];                       
                        InsertAtr1["shiftcode"] = dr1["shiftcode"];
                      
                        // timein
                        if (BizFunctions.IsEmpty(dr1["timein"]))
                        {
                            InsertAtr1["timein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["timein"] = dr1["timein"];
                        }
                        //timeout
                        if (BizFunctions.IsEmpty(dr1["timeout"]))
                        {
                            InsertAtr1["timeout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["timeout"] = dr1["timeout"];
                        }
                        //scheddatetiin
                        if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            InsertAtr1["scheddatein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["scheddatein"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimein"]));

                        }
                        //scheddateout



                        if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            InsertAtr1["scheddateout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimeout"]));
                        }

                        //schedtimein
                        if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            InsertAtr1["schedtimein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                        }

                        ////////////////////////////

                        if (!BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            LocalTime timein = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timein"].ToString());
                            LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"])));

                            if (timein.TickOfDay < actualtimeout.TickOfDay)
                            {
                                InsertAtr1["confirmedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                            }
                            else
                            {
                                InsertAtr1["confirmedtimein"] = dr1["timein"].ToString();
                            }
                        }


                        ////////////////////////////

                        //schedtimeout
                        if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            InsertAtr1["schedtimeout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"]));

                        }

                        if (!BizFunctions.IsEmpty(dr1["timeout"]) && !BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            LocalTime timeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timeout"].ToString());
                            LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"])));

                            if (timeout.TickOfDay < actualtimeout.TickOfDay)
                            {
                                InsertAtr1["confirmedtimeout"] = dr1["timeout"].ToString();
                            }
                            else
                            {
                                InsertAtr1["confirmedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"]));
                            }
                        }


                        //
                                                                                                    
                        InsertAtr1["replacedby"] = dr1["rempnum"];
                        InsertAtr1["rempname"] = dr1["rempname"];
                        InsertAtr1["rnric"] = dr1["rnric"];
                        InsertAtr1["day"] = dr1["day"];
                        InsertAtr1["date"] = dr1["date"];
                        InsertAtr1["TotalHrs"] = dr1["TotalHrs"];
                        
                        InsertAtr1["dayofweek"] = TimeTools.GetDayOfWeekNo(dr1["day"].ToString().Trim());
                        InsertAtr1["RepRefKey"] = dr1["RefKey"];
                        wkc1.Rows.Add(InsertAtr1);
                    }
                }

            }
        }


        private string GetPayTypeCode(string empnum)
        {
            string paytype = "";

            string getPayType = "Select paytypecode from hemph where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getPayType);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                paytype = dr1["paytypecode"].ToString();
            }

            return paytype;
        }

        private decimal GetPayTypeValue(string paytypecode)
        {
            decimal value = 0;

            string GetPayTypeValue = "Select value from PAYTM where paytypecode='" + paytypecode + "'";

                DataTable dt2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetPayTypeValue);

                if (dt2.Rows.Count > 0)
                {
                    DataRow dr2 = dt2.Rows[0];

                    if(BizFunctions.IsEmpty(dr2["value"]))
                    {
                        dr2["value"] = 0;
                    }

                    value = Convert.ToDecimal(dr2["value"]);
                }

            

            return value;
        }
    }
}
    

