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

namespace ATL.SITMD
{
    public class Voucher_SITMD : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName = null;
        protected CheckBox cb_all, sitmd_Mon, sitmd_Tue, sitmd_Wed, sitmd_Thu, sitmd_Fri, sitmd_Sat, sitmd_Sun = null;
        protected DateTimePicker sitmd_timein, sitmd_timeout = null;
        protected TextBox sitmd_ttlworkinghrs = null;
        protected DateTime dt = DateTime.Now;
        protected bool opened = false;
        protected bool isNewRecord = false;
       
        #endregion

        #region Construct

        public Voucher_SITMD(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_SITMD.xml", moduleName, voucherBaseHelpers)
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

            opened = false;

        }

        #endregion

        #region DocumentPage Event
        protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_Edit_Handle(sender, e);

            if (opened)
            {
                MessageBox.Show("You cannot open two documents at the same time.\n" +
                    "To Edit/New a Document, you have to close the document that's currently opened for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            e.Handle = !opened;
        }

        protected override void Voucher_New_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_New_Handle(sender, e);

            if (opened)
            {
                MessageBox.Show("You cannot open two documents at the same time.\n" +
                    "To Edit/New a Document, you have to close the document that's currently opened for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //core 'thought' it is input by user
                this.VoucherBase.SearchTextBox.Text = "NEW RECORD";
            }

            e.Handle = !opened;

        }
        #endregion

   

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;
            DataRow SITMD = e.DBAccess.DataSet.Tables["SITMD"].Rows[0];
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            opened = true;


            e.DBAccess.DataSet.Tables["SITMD"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITMD_ColumnChanged);
            //e.DBAccess.DataSet.Tables["SITMD2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITMD2_ColumnChanged);


            Initialise();
            //Checkcb();         
        }

        #endregion

        //private void isNew()
        //{
        //    DataRow sitmd = this.dbaccess.DataSet.Tables["sitmd"].Rows[0];
        //    if(!BizFunctions.IsEmpty(sitmd["status"]))
        //    {
        //        if (sitmd["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSO || sitmd["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSP || sitmd["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSV)
        //        {
        //            isNewRecord = true;
        //        }
        //    }
        //}

        #region Initialise Components
        private void Initialise()
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];

            //if (BizFunctions.IsEmpty(SITMD["timein"]))
            //{
            //    SITMD["timein"] = "0000";
            //}
            //if (BizFunctions.IsEmpty(SITMD["timeout"]))
            //{
            //    SITMD["timeout"] = "0000";
            //}

            ////sitmd_timein = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_timein") as DateTimePicker;
            ////sitmd_timeout = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_timeout") as DateTimePicker;

            ////sitmd_timein.Format = DateTimePickerFormat.Time;
            ////sitmd_timein.ShowUpDown = true;

            ////sitmd_timeout.Format = DateTimePickerFormat.Time;
            ////sitmd_timeout.ShowUpDown = true;
       

            //cb_all = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cb_all") as CheckBox;
            //cb_all.CheckedChanged += new EventHandler(cb_all_CheckedChanged);

            //sitmd_Mon = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_Monday") as CheckBox;
            //sitmd_Mon.CheckedChanged += new EventHandler(sitmd_Mon_CheckedChanged);

            //sitmd_Tue = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_Tuesday") as CheckBox;
            //sitmd_Tue.CheckedChanged += new EventHandler(sitmd_Tue_CheckedChanged);

            //sitmd_Wed = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_Wednesday") as CheckBox;
            //sitmd_Wed.CheckedChanged += new EventHandler(sitmd_Wed_CheckedChanged);

            //sitmd_Thu = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_Thursday") as CheckBox;
            //sitmd_Thu.CheckedChanged += new EventHandler(sitmd_Thu_CheckedChanged);

            //sitmd_Fri = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_Friday") as CheckBox;
            //sitmd_Fri.CheckedChanged += new EventHandler(sitmd_Fri_CheckedChanged);

            //sitmd_Sat = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_Saturday") as CheckBox;
            //sitmd_Sat.CheckedChanged += new EventHandler(sitmd_Sat_CheckedChanged);

            //sitmd_Sun = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_Sunday") as CheckBox;
            //sitmd_Sun.CheckedChanged += new EventHandler(sitmd_Sun_CheckedChanged);

            //sitmd_ttlworkinghrs = BizXmlReader.CurrentInstance.GetControl(headerFormName, "sitmd_ttlworkinghrs") as TextBox;

            

            

        }
        #endregion

        #region Check Check Boxes Status
        private void Checkcb()
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            int mon, tue, wed, thu, fri, sat, sun;

            if (BizFunctions.IsEmpty(SITMD["monday"]))
            {
                SITMD["monday"] = 0;
            }
            if (BizFunctions.IsEmpty(SITMD["tuesday"]))
            {
                SITMD["tuesday"] = 0;
            }
            if (BizFunctions.IsEmpty(SITMD["wednesday"]))
            {
                SITMD["wednesday"] = 0;
            }
            if (BizFunctions.IsEmpty(SITMD["thursday"]))
            {
                SITMD["thursday"] = 0;
            }
            if (BizFunctions.IsEmpty(SITMD["friday"]))
            {
                SITMD["friday"] = 0;
            }
            if (BizFunctions.IsEmpty(SITMD["saturday"]))
            {
                SITMD["saturday"] = 0;
            }
            if (BizFunctions.IsEmpty(SITMD["sunday"]))
            {
                SITMD["sunday"] = 0;
            }

            mon = Convert.ToInt16(SITMD["monday"]);
            tue = Convert.ToInt16(SITMD["tuesday"]);
            wed = Convert.ToInt16(SITMD["wednesday"]);
            thu = Convert.ToInt16(SITMD["thursday"]);
            fri = Convert.ToInt16(SITMD["friday"]);
            sat = Convert.ToInt16(SITMD["saturday"]);
            sun = Convert.ToInt16(SITMD["sunday"]);

            if (mon == 1)
            {
                sitmd_Mon.Checked = true;
            }
            if (tue == 1)
            {
                sitmd_Tue.Checked = true;
            }
            if (wed == 1)
            {
                sitmd_Wed.Checked = true;
            }
            if (thu == 1)
            {
                sitmd_Thu.Checked = true;
            }
            if (fri == 1)
            {
                sitmd_Fri.Checked = true;
            }
            if (sat == 1)
            {
                sitmd_Sat.Checked = true;
            }
            if (sun == 1)
            {
                sitmd_Sun.Checked = true;
            }

            if (sitmd_Mon.Checked == true && sitmd_Tue.Checked == true && sitmd_Wed.Checked == true && sitmd_Thu.Checked == true && sitmd_Fri.Checked == true && sitmd_Sat.Checked == true && sitmd_Sun.Checked == true)
            {
                cb_all.Checked = true;
            }
        }
        #endregion


        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            DataTable sitmd1 = dbaccess.DataSet.Tables["SITMD1"];
            base.Document_Save_Handle(sender, e);

            if (BizFunctions.IsEmpty(SITMD["sitmdnum"]))
            {
                MessageBox.Show("You cannot save it without an Employee No.\n" +
                   "Please Assign an Employee Number for this Record.",
                   "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Handle = false;
            }

            if (e.Handle)
            {
                if (SITMD["sitmdnum"].ToString() == "NEW RECORD")
                {
                    string CheckEmp = "Select empnum from sitmd where empnum='" + SITMD["empnum"].ToString() + "'";

                    this.dbaccess.ReadSQL("CheckEmpTB", CheckEmp);

                    DataTable checkEmptb = this.dbaccess.DataSet.Tables["CheckEmpTB"];

                    if (checkEmptb.Rows.Count <= 0)
                    {
                        SITMD["sitmdnum"] = SITMD["empnum"].ToString() + "-" + BizFunctions.GetSafeDateString(DateTime.Today);
                    }
                    else
                    {
                        MessageBox.Show("There is a Record of this Employee\n" +
                                          "You cannot create a new Record for this Employee.",
                                          "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Handle = false;
                    }
                }
            }


            //if (!BizFunctions.IsEmpty(SITMD["timein"]) && !BizFunctions.IsEmpty(SITMD["timeout"]))
            //{
            //    if (Convert.ToDouble(SITMD["timein"]) <= Convert.ToDouble(SITMD["timeout"]))
            //    {
            //        SITMD["ttlworkinghrs"] = Math.Round(Math.Abs(GetHours(SITMD["timein"].ToString(), SITMD["timeout"].ToString())), 1, MidpointRounding.AwayFromZero);
            //    }
            //    else if (Convert.ToDouble(SITMD["timein"]) == Convert.ToDouble(SITMD["timeout"]))
            //    {
            //        SITMD["ttlworkinghrs"] = 24;
            //    }
            //    else
            //    {
            //        decimal tmpTTL1, tmpTTL2 = 0;
            //        tmpTTL1 = (decimal)Math.Abs(GetHours(SITMD["timein"].ToString(), "2359"));
            //        tmpTTL2 = (decimal)Math.Abs(GetHours("0001", SITMD["timeout"].ToString()));
            //        SITMD["ttlworkinghrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);
            //    }
            //}

            //if (sitmd2.Rows.Count > 0)
            //{
            //    foreach (DataRow dr2 in sitmd2.Rows)
            //    {
            //        if (dr2.RowState != DataRowState.Deleted)
            //        {
            //            if (!BizFunctions.IsEmpty(dr2["timein"]) && !BizFunctions.IsEmpty(dr2["timeout"]))
            //            {
            //                if (Convert.ToDouble(dr2["timein"]) <= Convert.ToDouble(dr2["timeout"]))
            //                {
            //                    dr2["totalhours"] = Math.Round(Math.Abs(GetHours(dr2["timein"].ToString(), dr2["timeout"].ToString())), 1, MidpointRounding.AwayFromZero);
            //                }
            //                else
            //                {
            //                    decimal tmpTTL12, tmpTTL22 = 0;
            //                    tmpTTL12 = (decimal)Math.Abs(GetHours(dr2["timein"].ToString(), "2359"));
            //                    tmpTTL22 = (decimal)Math.Abs(GetHours("0001", dr2["timeout"].ToString()));
            //                    dr2["totalhours"] = Math.Round((tmpTTL12 + tmpTTL22),1,MidpointRounding.AwayFromZero);
                                
            //                }
            //            }

            //        }
            //    }
            //}

            
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);

            switch (e.ControlName)
            {
                case "sitmd_empnum":
                    {
                        e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
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
                case "sitenum":
                    {
                        e.CurrentRow["sitename"] = e.F2CurrentRow["sitename"];
                    }
                    break;
          
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
            DataRow SITMD = e.DBAccess.DataSet.Tables["SITMD"].Rows[0];
            DataTable SITMD1 = e.DBAccess.DataSet.Tables["SITMD1"];
            //DataTable SITMD2 = e.DBAccess.DataSet.Tables["SITMD2"];
           
			foreach (DataRow dr in SITMD1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					
					BizFunctions.UpdateDataRow(SITMD, dr, "sitmdnum/status/modified");
				}
			}
            //foreach (DataRow dr2 in SITMD2.Rows)
            //{
            //    if (dr2.RowState != DataRowState.Deleted)
            //    {

            //        BizFunctions.UpdateDataRow(SITMD, dr2, "shiftcode/status/modified");
            //    }
            //}

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

        private void cb_all_CheckedChanged(object sender, EventArgs e)
        {
            DataTable sitmd2 = dbaccess.DataSet.Tables["SITMD2"];
            if (cb_all.Checked)
            {
                sitmd_Mon.Checked = true; 
                sitmd_Tue.Checked = true; 
                sitmd_Wed.Checked = true; 
                sitmd_Thu.Checked = true; 
                sitmd_Fri.Checked = true; 
                sitmd_Sat.Checked = true;
                sitmd_Sun.Checked = true;

               
            }
            if (cb_all.Checked == false)
            {
                sitmd_Mon.Checked = false;
                sitmd_Tue.Checked = false;
                sitmd_Wed.Checked = false;
                sitmd_Thu.Checked = false;
                sitmd_Fri.Checked = false;
                sitmd_Sat.Checked = false;
                sitmd_Sun.Checked = false;
                BizFunctions.DeleteAllRows(sitmd2);
            }
        }

        private void sitmd_Mon_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];  
            if (sitmd_Mon.Checked)
            {
                SITMD["monday"] = 1;
                InsertSITMD2("monday");
            }
            else if (!sitmd_Mon.Checked)
            {
                SITMD["monday"] = 0;
                removeSITMD2("monday");
                
            }
        }

        private void sitmd_Tue_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            if (sitmd_Tue.Checked)
            {
                SITMD["tuesday"] = 1;
                InsertSITMD2("tuesday");
            }
            else if (!sitmd_Tue.Checked)
            {
                SITMD["tuesday"] = 0;
                removeSITMD2("tuesday");
            }
        }

        private void sitmd_Wed_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            if (sitmd_Wed.Checked)
            {
                SITMD["wednesday"] = 1;
                InsertSITMD2("wednesday");
            }
            else if (!sitmd_Wed.Checked)
            {
                SITMD["wednesday"] = 0;
                removeSITMD2("wednesday");
            }
        }

        private void sitmd_Thu_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            if (sitmd_Thu.Checked)
            {
                SITMD["thursday"] = 1;
                InsertSITMD2("thursday");
            }
            else if (!sitmd_Thu.Checked)
            {
                SITMD["thursday"] = 0;
                removeSITMD2("thursday");
            }
        }

        private void sitmd_Fri_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            if (sitmd_Fri.Checked)
            {
                SITMD["friday"] = 1;
                InsertSITMD2("friday");
            }
            else if (!sitmd_Fri.Checked)
            {
                SITMD["friday"] = 0;
                removeSITMD2("friday");
            }
        }

        private void sitmd_Sat_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            if (sitmd_Sat.Checked)
            {
                SITMD["saturday"] = 1;
                InsertSITMD2("saturday");
            }
            else if (!sitmd_Sat.Checked)
            {
                SITMD["saturday"] = 0;
                removeSITMD2("saturday");
            }
        }

        private void sitmd_Sun_CheckedChanged(object sender, EventArgs e)
        {
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
            if (sitmd_Sun.Checked)
            {
                SITMD["sunday"] = 1;
                InsertSITMD2("sunday");
            }
            else if (!sitmd_Sun.Checked)
            {
                SITMD["sunday"] = 0;
                removeSITMD2("sunday");
            }
        }

        private void Voucher_SITMD_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
         
            DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "timein":
                    {                      
                        if (!BizFunctions.IsEmpty(SITMD["timein"]))
                        {
                            if (!BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(SITMD["timein"].ToString()))
                            {
                                MessageBox.Show("Invalid Time Format!", "BizERP Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                SITMD["timein"] = "";

                            }              
                        }

                       
                    }
                    break;

                case "timeout":

                    {
                        if (!BizFunctions.IsEmpty(SITMD["timeout"]))
                        {
                            if (!BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(SITMD["timeout"].ToString()))
                            {
                                MessageBox.Show("Invalid Time Format!", "BizERP Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                SITMD["timeout"] = "";

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

        private void Voucher_SITMD2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
             DataRow SITMD = this.dbaccess.DataSet.Tables["SITMD"].Rows[0];
             DataTable SITMD2 = this.dbaccess.DataSet.Tables["SITMD2"];

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

        private void InsertSITMD2(string day)
        {
            DataRow sitmd = dbaccess.DataSet.Tables["SITMD"].Rows[0];
            DataTable sitmd2 = dbaccess.DataSet.Tables["SITMD2"];

            if (sitmd2.Rows.Count > 0)
            {
                for (int i = 0; i <= sitmd2.Rows.Count - 1; i++)
                {
                    if (sitmd2.Rows[i].RowState != DataRowState.Deleted)
                    {
                        if (sitmd2.Rows[i]["day"].ToString() == day)
                        {
                            sitmd2.Rows[i].Delete();
                        }
                    }
                }
            }              


            DataTable tmpSITMD2 = new DataTable();
            tmpSITMD2.Columns.Add("DAY",typeof(string));
            tmpSITMD2.Columns.Add("timein", typeof(string));
            tmpSITMD2.Columns.Add("timeout", typeof(string));
            tmpSITMD2.Columns.Add("totalhours", typeof(decimal));            
            tmpSITMD2.Columns.Add("sequence", typeof(int));

            DataRow insertShm2 = tmpSITMD2.NewRow();
            insertShm2["day"] = day;
            insertShm2["timein"] = sitmd["timein"].ToString().Trim();
            insertShm2["timeout"] = sitmd["timeout"].ToString().Trim();
            insertShm2["sequence"] = TimeTools.GetDayOfWeekNo(day);
            tmpSITMD2.Rows.Add(insertShm2);

            tmpSITMD2.TableName = "tmpSITMD2";

            SortDT sdt = new SortDT(tmpSITMD2, "sequence");
            DataTable tmpSortedsitmd2 = sdt.SortedTable();

        

            foreach (DataRow dr1 in tmpSortedsitmd2.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    sitmd2.ImportRow(dr1);
                }
            }

            tmpSITMD2.Dispose();
        }

        private void removeSITMD2(string day)
        {
            DataTable sitmd2 = dbaccess.DataSet.Tables["SITMD2"];
            if (sitmd2.Rows.Count > 0)
            {
                for (int i = 0; i <= sitmd2.Rows.Count - 1; i++)
                {
                    if (sitmd2.Rows[i].RowState != DataRowState.Deleted)
                    {
                        if (sitmd2.Rows[i]["day"].ToString() == day)
                        {
                            sitmd2.Rows[i].Delete();
                        }
                    }
                }
            }              
        }

    }
}
    

