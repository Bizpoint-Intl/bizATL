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
using NodaTime;
using System.Text.RegularExpressions;
using ATL.TimeUtilites;
using ATL.SortTable;
#endregion

namespace ATL.SHM
{
    public class Voucher_SHM : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName = null;
        protected CheckBox cb_all, shm_Mon, shm_Tue, shm_Wed, shm_Thu, shm_Fri, shm_Sat, shm_Sun = null;
        protected DateTimePicker shm_timein, shm_timeout = null;
        protected TextBox shm_ttlworkinghrs = null;
        protected DateTime dt = DateTime.Now;
        #endregion

        #region Construct

        public Voucher_SHM(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_SHM.xml", moduleName, voucherBaseHelpers)
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
            DataRow SHM = e.DBAccess.DataSet.Tables["SHM"].Rows[0];
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;



            e.DBAccess.DataSet.Tables["SHM"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SHM_ColumnChanged);
            e.DBAccess.DataSet.Tables["SHM2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SHM2_ColumnChanged);


            Initialise();
            Checkcb();         
        }

        #endregion

        #region Initialise Components
        private void Initialise()
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];

            if (BizFunctions.IsEmpty(SHM["timein"]))
            {
                SHM["timein"] = "0000";
            }
            if (BizFunctions.IsEmpty(SHM["timeout"]))
            {
                SHM["timeout"] = "0000";
            }

            //shm_timein = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_timein") as DateTimePicker;
            //shm_timeout = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_timeout") as DateTimePicker;

            //shm_timein.Format = DateTimePickerFormat.Time;
            //shm_timein.ShowUpDown = true;

            //shm_timeout.Format = DateTimePickerFormat.Time;
            //shm_timeout.ShowUpDown = true;
       

            cb_all = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cb_all") as CheckBox;
            cb_all.CheckedChanged += new EventHandler(cb_all_CheckedChanged);

            shm_Mon = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_Monday") as CheckBox;
            shm_Mon.CheckedChanged += new EventHandler(shm_Mon_CheckedChanged);

            shm_Tue = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_Tuesday") as CheckBox;
            shm_Tue.CheckedChanged += new EventHandler(shm_Tue_CheckedChanged);

            shm_Wed = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_Wednesday") as CheckBox;
            shm_Wed.CheckedChanged += new EventHandler(shm_Wed_CheckedChanged);

            shm_Thu = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_Thursday") as CheckBox;
            shm_Thu.CheckedChanged += new EventHandler(shm_Thu_CheckedChanged);

            shm_Fri = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_Friday") as CheckBox;
            shm_Fri.CheckedChanged += new EventHandler(shm_Fri_CheckedChanged);

            shm_Sat = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_Saturday") as CheckBox;
            shm_Sat.CheckedChanged += new EventHandler(shm_Sat_CheckedChanged);

            shm_Sun = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_Sunday") as CheckBox;
            shm_Sun.CheckedChanged += new EventHandler(shm_Sun_CheckedChanged);

            shm_ttlworkinghrs = BizXmlReader.CurrentInstance.GetControl(headerFormName, "shm_ttlworkinghrs") as TextBox;

            

            

        }
        #endregion

        #region Check Check Boxes Status
        private void Checkcb()
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            int mon, tue, wed, thu, fri, sat, sun;

            if (BizFunctions.IsEmpty(SHM["monday"]))
            {
                SHM["monday"] = 0;
            }
            if (BizFunctions.IsEmpty(SHM["tuesday"]))
            {
                SHM["tuesday"] = 0;
            }
            if (BizFunctions.IsEmpty(SHM["wednesday"]))
            {
                SHM["wednesday"] = 0;
            }
            if (BizFunctions.IsEmpty(SHM["thursday"]))
            {
                SHM["thursday"] = 0;
            }
            if (BizFunctions.IsEmpty(SHM["friday"]))
            {
                SHM["friday"] = 0;
            }
            if (BizFunctions.IsEmpty(SHM["saturday"]))
            {
                SHM["saturday"] = 0;
            }
            if (BizFunctions.IsEmpty(SHM["sunday"]))
            {
                SHM["sunday"] = 0;
            }

            mon = Convert.ToInt16(SHM["monday"]);
            tue = Convert.ToInt16(SHM["tuesday"]);
            wed = Convert.ToInt16(SHM["wednesday"]);
            thu = Convert.ToInt16(SHM["thursday"]);
            fri = Convert.ToInt16(SHM["friday"]);
            sat = Convert.ToInt16(SHM["saturday"]);
            sun = Convert.ToInt16(SHM["sunday"]);

            if (mon == 1)
            {
                shm_Mon.Checked = true;
            }
            if (tue == 1)
            {
                shm_Tue.Checked = true;
            }
            if (wed == 1)
            {
                shm_Wed.Checked = true;
            }
            if (thu == 1)
            {
                shm_Thu.Checked = true;
            }
            if (fri == 1)
            {
                shm_Fri.Checked = true;
            }
            if (sat == 1)
            {
                shm_Sat.Checked = true;
            }
            if (sun == 1)
            {
                shm_Sun.Checked = true;
            }

            if (shm_Mon.Checked == true && shm_Tue.Checked == true && shm_Wed.Checked == true && shm_Thu.Checked == true && shm_Fri.Checked == true && shm_Sat.Checked == true && shm_Sun.Checked == true)
            {
                cb_all.Checked = true;
            }
        }
        #endregion


        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            DataTable shm2 = dbaccess.DataSet.Tables["SHM2"];
            base.Document_Save_Handle(sender, e);

            decimal totalhours = 0;

            if (!BizFunctions.IsEmpty(SHM["timein"]) && !BizFunctions.IsEmpty(SHM["timeout"]))
            {
                if (Convert.ToDouble(SHM["timein"]) <= Convert.ToDouble(SHM["timeout"]))
                {
                    //SHM["ttlworkinghrs"] = Math.Round(Math.Abs(GetHours(SHM["timein"].ToString(), SHM["timeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                    totalhours = Convert.ToDecimal(Math.Round(Math.Abs(GetHours(SHM["timein"].ToString(), SHM["timeout"].ToString())), 1, MidpointRounding.AwayFromZero));
                    totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                    SHM["ttlworkinghrs"] = totalhours;
                }
                else if (Convert.ToDouble(SHM["timein"]) == Convert.ToDouble(SHM["timeout"]))
                {
                    SHM["ttlworkinghrs"] = 24;
                }
                else
                {
                    decimal tmpTTL1, tmpTTL2 = 0;
                    tmpTTL1 = (decimal)Math.Abs(GetHours(SHM["timein"].ToString(), "2359"));
                    tmpTTL2 = (decimal)Math.Abs(GetHours("0001", SHM["timeout"].ToString()));
                    //SHM["ttlworkinghrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);
                    totalhours = Convert.ToDecimal(Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero));
                    totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                    SHM["ttlworkinghrs"] = totalhours;
                  
                }
            }

            if (shm2.Rows.Count > 0)
            {
                foreach (DataRow dr2 in shm2.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr2["timein"]) && !BizFunctions.IsEmpty(dr2["timeout"]))
                        {
                            if (Convert.ToDouble(dr2["timein"]) <= Convert.ToDouble(dr2["timeout"]))
                            {
                                //dr2["totalhours"] = Math.Round(Math.Abs(GetHours(dr2["timein"].ToString(), dr2["timeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                                totalhours = Convert.ToDecimal(Math.Round(Math.Abs(GetHours(dr2["timein"].ToString(), dr2["timeout"].ToString())), 1, MidpointRounding.AwayFromZero));

                                totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                                SHM["ttlworkinghrs"] = totalhours;
                            }
                            else
                            {
                                decimal tmpTTL12, tmpTTL22 = 0;
                                tmpTTL12 = (decimal)Math.Abs(GetHours(dr2["timein"].ToString(), "2359"));
                                tmpTTL22 = (decimal)Math.Abs(GetHours("0001", dr2["timeout"].ToString()));
                                //dr2["totalhours"] = Math.Round((tmpTTL12 + tmpTTL22), 1, MidpointRounding.AwayFromZero);
                                totalhours = Convert.ToDecimal(Math.Round((tmpTTL12 + tmpTTL22),1,MidpointRounding.AwayFromZero));

                                totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                                SHM["ttlworkinghrs"] = totalhours;
                                
                            }
                        }

                    }
                }
            }

            
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
          
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
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow SHM = e.DBAccess.DataSet.Tables["SHM"].Rows[0];
            DataTable SHM1 = e.DBAccess.DataSet.Tables["SHM1"];
            DataTable SHM2 = e.DBAccess.DataSet.Tables["SHM2"];
           
			foreach (DataRow dr in SHM1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					
					BizFunctions.UpdateDataRow(SHM, dr, "shiftcode/status/modified");
				}
			}
            foreach (DataRow dr2 in SHM2.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {

                    BizFunctions.UpdateDataRow(SHM, dr2, "shiftcode/status/modified");
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

            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            DataTable shm2 = dbaccess.DataSet.Tables["SHM2"];

            decimal totalhours = 0;

            if (!BizFunctions.IsEmpty(SHM["timein"]) && !BizFunctions.IsEmpty(SHM["timeout"]))
            {
                if (BizFunctions.IsEmpty(SHM["mealbreak"]))
                {
                    SHM["mealbreak"] = 0;
                }
                if (BizFunctions.IsEmpty(SHM["teabreak"]))
                {
                    SHM["teabreak"] = 0;
                }
                if (Convert.ToDouble(SHM["timein"]) <= Convert.ToDouble(SHM["timeout"]))
                {
                    //SHM["ttlworkinghrs"] = Math.Round(Math.Abs(GetHours(SHM["timein"].ToString(), SHM["timeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                    totalhours = Convert.ToDecimal(Math.Round(Math.Abs(GetHours(SHM["timein"].ToString(), SHM["timeout"].ToString())), 1, MidpointRounding.AwayFromZero));
                    totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                    SHM["ttlworkinghrs"] = totalhours;
                }
                else if (Convert.ToDouble(SHM["timein"]) == Convert.ToDouble(SHM["timeout"]))
                {
                    SHM["ttlworkinghrs"] = 24;
                }
                else
                {
                    decimal tmpTTL1, tmpTTL2 = 0;
                    tmpTTL1 = (decimal)Math.Abs(GetHours(SHM["timein"].ToString(), "2359"));
                    tmpTTL2 = (decimal)Math.Abs(GetHours("0001", SHM["timeout"].ToString()));
                    //SHM["ttlworkinghrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);
                    totalhours = Convert.ToDecimal(Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero));
                    totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                    SHM["ttlworkinghrs"] = totalhours;

                }
            }

            if (shm2.Rows.Count > 0)
            {
                foreach (DataRow dr2 in shm2.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr2["timein"]) && !BizFunctions.IsEmpty(dr2["timeout"]))
                        {
                            if (Convert.ToDouble(dr2["timein"]) <= Convert.ToDouble(dr2["timeout"]))
                            {
                                //dr2["totalhours"] = Math.Round(Math.Abs(GetHours(dr2["timein"].ToString(), dr2["timeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                                totalhours = Convert.ToDecimal(Math.Round(Math.Abs(GetHours(dr2["timein"].ToString(), dr2["timeout"].ToString())), 1, MidpointRounding.AwayFromZero));

                                totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                                SHM["ttlworkinghrs"] = totalhours;
                            }
                            else
                            {
                                decimal tmpTTL12, tmpTTL22 = 0;
                                tmpTTL12 = (decimal)Math.Abs(GetHours(dr2["timein"].ToString(), "2359"));
                                tmpTTL22 = (decimal)Math.Abs(GetHours("0001", dr2["timeout"].ToString()));
                                //dr2["totalhours"] = Math.Round((tmpTTL12 + tmpTTL22), 1, MidpointRounding.AwayFromZero);
                                totalhours = Convert.ToDecimal(Math.Round((tmpTTL12 + tmpTTL22), 1, MidpointRounding.AwayFromZero));

                                totalhours = totalhours - (Convert.ToDecimal(SHM["teabreak"]) + Convert.ToDecimal(SHM["mealbreak"]));
                                SHM["ttlworkinghrs"] = totalhours;

                            }
                        }

                    }
                }
            }

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

        private void cb_all_CheckedChanged(object sender, EventArgs e)
        {
            DataTable shm2 = dbaccess.DataSet.Tables["SHM2"];
            if (cb_all.Checked)
            {
                shm_Mon.Checked = true; 
                shm_Tue.Checked = true; 
                shm_Wed.Checked = true; 
                shm_Thu.Checked = true; 
                shm_Fri.Checked = true; 
                shm_Sat.Checked = true;
                shm_Sun.Checked = true;

               
            }
            if (cb_all.Checked == false)
            {
                shm_Mon.Checked = false;
                shm_Tue.Checked = false;
                shm_Wed.Checked = false;
                shm_Thu.Checked = false;
                shm_Fri.Checked = false;
                shm_Sat.Checked = false;
                shm_Sun.Checked = false;
                BizFunctions.DeleteAllRows(shm2);
            }
        }

        private void shm_Mon_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];  
            if (shm_Mon.Checked)
            {
                SHM["monday"] = 1;
                InsertSHM2("monday");
            }
            else if (!shm_Mon.Checked)
            {
                SHM["monday"] = 0;
                removeSHM2("monday");
                
            }
        }

        private void shm_Tue_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            if (shm_Tue.Checked)
            {
                SHM["tuesday"] = 1;
                InsertSHM2("tuesday");
            }
            else if (!shm_Tue.Checked)
            {
                SHM["tuesday"] = 0;
                removeSHM2("tuesday");
            }
        }

        private void shm_Wed_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            if (shm_Wed.Checked)
            {
                SHM["wednesday"] = 1;
                InsertSHM2("wednesday");
            }
            else if (!shm_Wed.Checked)
            {
                SHM["wednesday"] = 0;
                removeSHM2("wednesday");
            }
        }

        private void shm_Thu_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            if (shm_Thu.Checked)
            {
                SHM["thursday"] = 1;
                InsertSHM2("thursday");
            }
            else if (!shm_Thu.Checked)
            {
                SHM["thursday"] = 0;
                removeSHM2("thursday");
            }
        }

        private void shm_Fri_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            if (shm_Fri.Checked)
            {
                SHM["friday"] = 1;
                InsertSHM2("friday");
            }
            else if (!shm_Fri.Checked)
            {
                SHM["friday"] = 0;
                removeSHM2("friday");
            }
        }

        private void shm_Sat_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            if (shm_Sat.Checked)
            {
                SHM["saturday"] = 1;
                InsertSHM2("saturday");
            }
            else if (!shm_Sat.Checked)
            {
                SHM["saturday"] = 0;
                removeSHM2("saturday");
            }
        }

        private void shm_Sun_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
            if (shm_Sun.Checked)
            {
                SHM["sunday"] = 1;
                InsertSHM2("sunday");
            }
            else if (!shm_Sun.Checked)
            {
                SHM["sunday"] = 0;
                removeSHM2("sunday");
            }
        }

        private void Voucher_SHM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
         
            DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "timein":
                    {                      
                        if (!BizFunctions.IsEmpty(SHM["timein"]))
                        {
                            if (!BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(SHM["timein"].ToString()))
                            {
                                MessageBox.Show("Invalid Time Format!", "BizERP Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                SHM["timein"] = "";

                            }              
                        }

                       
                    }
                    break;

                case "timeout":

                    {
                        if (!BizFunctions.IsEmpty(SHM["timeout"]))
                        {
                            if (!BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(SHM["timeout"].ToString()))
                            {
                                MessageBox.Show("Invalid Time Format!", "BizERP Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                SHM["timeout"] = "";

                            }           
                        }
                    }
                    break;

                case "ttlworkinghrs":
                    {
                        
                    }
                    break;
                   
          
            }
        }

        private void Voucher_SHM2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
             DataRow SHM = this.dbaccess.DataSet.Tables["SHM"].Rows[0];
             DataTable SHM2 = this.dbaccess.DataSet.Tables["SHM2"];

             switch (e.Column.ColumnName)
             {
                 case "timein":
                     {
 
                         if (!BizFunctions.IsEmpty(e.Row["timein"]))
                         {
                             if (!BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["timein"].ToString()))
                             {
                                 MessageBox.Show("Invalid Time Format!", "BizERP Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                 e.Row.BeginEdit();
                                 e.Row["timein"] = string.Empty;
                                 e.Row.EndEdit();
                             }
                         }
                         else
                         {
                             if (!BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                             {
                                 if (Convert.ToDouble(e.Row["timein"]) <= Convert.ToDouble(e.Row["timeout"]))
                                 {
                                     e.Row["totalhours"] = Math.Round(Math.Abs(GetHours(e.Row["timein"].ToString(), e.Row["timeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                                 }
                                 else
                                 {
                                     decimal tmpTTL1, tmpTTL2 = 0;
                                     tmpTTL1 = (decimal)Math.Abs(GetHours(e.Row["timein"].ToString(), "2359"));
                                     tmpTTL2 = (decimal)Math.Abs(GetHours("0001", e.Row["timeout"].ToString()));
                                     e.Row["totalhours"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);
                                 }
                             }

                         }                            


                     }
                     break;

                 case "timeout":
                     {
                         if (!BizFunctions.IsEmpty(e.Row["timeout"]))
                         {
                             if (!BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["timeout"].ToString()))
                             {
                                 MessageBox.Show("Invalid Time Format!", "BizERP Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                 e.Row.BeginEdit();
                                 e.Row["timeout"] = string.Empty;
                                 e.Row.EndEdit();
                             }
                             else
                             {
                                 if (!BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                 {
                                     if (Convert.ToDouble(e.Row["timein"]) <= Convert.ToDouble(e.Row["timeout"]))
                                     {
                                         e.Row["totalhours"] = Math.Round(Math.Abs(GetHours(e.Row["timein"].ToString(), e.Row["timeout"].ToString())), 1, MidpointRounding.AwayFromZero); 
                                     }
                                     else
                                     {
                                         decimal tmpTTL1, tmpTTL2 = 0;
                                         tmpTTL1 = (decimal)Math.Abs(GetHours(e.Row["timein"].ToString(), "2359"));
                                         tmpTTL2 = (decimal)Math.Abs(GetHours("0001", e.Row["timeout"].ToString()));
                                         e.Row["totalhours"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);
                                     }
                                 }
                                 
                             }                            
                         }
                     }
                     break;
             }
        }

        private double GetHours(string start, string end)
        {
            double hourstaken;
            LocalTime dt1 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(start);
            LocalTime dt2 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(end);
            
            Duration dr1 = new Duration(dt1.TickOfDay);
            Duration dr2 = new Duration(dt2.TickOfDay);

            Duration dr3 = Duration.Subtract(dr1, dr2);

            TimeSpan elapsedSpan1 = new TimeSpan(dr3.Ticks);

            return hourstaken = elapsedSpan1.TotalHours;

        }

        private void InsertSHM2(string day)
        {
            DataRow shm = dbaccess.DataSet.Tables["SHM"].Rows[0];
            DataTable shm2 = dbaccess.DataSet.Tables["SHM2"];

            if (shm2.Rows.Count > 0)
            {
                for (int i = 0; i <= shm2.Rows.Count - 1; i++)
                {
                    if (shm2.Rows[i].RowState != DataRowState.Deleted)
                    {
                        if (shm2.Rows[i]["day"].ToString() == day)
                        {
                            shm2.Rows[i].Delete();
                        }
                    }
                }
            }              


            DataTable tmpSHM2 = new DataTable();
            tmpSHM2.Columns.Add("DAY",typeof(string));
            tmpSHM2.Columns.Add("timein", typeof(string));
            tmpSHM2.Columns.Add("timeout", typeof(string));
            tmpSHM2.Columns.Add("totalhours", typeof(decimal));            
            tmpSHM2.Columns.Add("sequence", typeof(int));

            DataRow insertShm2 = tmpSHM2.NewRow();
            insertShm2["day"] = day;
            insertShm2["timein"] = shm["timein"].ToString().Trim();
            insertShm2["timeout"] = shm["timeout"].ToString().Trim();
            insertShm2["sequence"] = TimeTools.GetDayOfWeekNo(day);
            tmpSHM2.Rows.Add(insertShm2);

            tmpSHM2.TableName = "tmpSHM2";

            SortDT sdt = new SortDT(tmpSHM2, "sequence");
            DataTable tmpSortedshm2 = sdt.SortedTable();

        

            foreach (DataRow dr1 in tmpSortedshm2.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    shm2.ImportRow(dr1);
                }
            }

            tmpSHM2.Dispose();
        }

        private void removeSHM2(string day)
        {
            DataTable shm2 = dbaccess.DataSet.Tables["SHM2"];
            if (shm2.Rows.Count > 0)
            {
                for (int i = 0; i <= shm2.Rows.Count - 1; i++)
                {
                    if (shm2.Rows[i].RowState != DataRowState.Deleted)
                    {
                        if (shm2.Rows[i]["day"].ToString() == day)
                        {
                            shm2.Rows[i].Delete();
                        }
                    }
                }
            }              
        }

    }
}
    

