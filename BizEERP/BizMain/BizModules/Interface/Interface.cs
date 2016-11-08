using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ATL.ATLInterfaceUI.SqlHelper;
using System.Data.Odbc;
using ATL.ATLInterfaceUI.SqlHelper2;
using System.Text.RegularExpressions;


using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizDetail;
using BizRAD.BizApplication;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizTools;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;


namespace ATL.ATLInterfaceUI
{
    public partial class Interface : Form
    {
        #region Class Variables
        string FromDb, ToDb, FromTable, ToTable, SqlString, FromColum, ToColumn, SyBaseConnectionString, DefaultConnectionString,whereClause = "";
        protected DBAccess dbaccess = null;
        protected bool isLiveSageLink = false;
        protected DataTable dt1 = null;
        #endregion

        #region Constructor
        public Interface()
        {
             this.dbaccess = new DBAccess();
            InitializeComponent();

            isLiveSageLink = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isLiveSageLink"]);


            DefaultConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ToString();

            if (isLiveSageLink)
            {
                if (BizLogicTools.Tools.Platform == "x86")
                {
                    SyBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SyBaseConnectionStringLive"].ToString();
                    lbl_SystemStat.Text = "LIVE";
                }
                else
                {
                    SyBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SyBaseConnectionStringLive64"].ToString();
                    lbl_SystemStat.Text = "LIVE";
                }
               
            }
            else
            {
                SyBaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SyBaseConnectionString"].ToString();
                lbl_SystemStat.Text = "TEST";
            }
           
            
            
        }
        #endregion

        #region Button Execute Click
        private void button1_Click(object sender, EventArgs e)
        {
            if (dt1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(dt1);
            }
            GenerateWhereClause();
            //return;
                //Done:Insert/Update
                if (comboBox1.Text.Trim() == "Staff Info")
                {
                    InsertQueryiPersonalAll();             
                }

                //Insert/Update
                if (comboBox1.Text.Trim() == "- Personal")
                {
                    InsertQueryiPersonal();
                }

                //Done:Insert/Update
                if (comboBox1.Text.Trim() == "- Employee Info")
                {
                    InsertQueryiEmployee();
                }

                //Done:Insert/Update
                if (comboBox1.Text.Trim() == "- Residence Status")
                {
                    InsertQueryiResidenceStatusRecord();
                }

                if (comboBox1.Text.Trim() == "- Bank Info")
                {
                    InsertQueryiPaymentBankInfo();
                }


                //Done:Insert/Update
                if (comboBox1.Text.Trim() == "- Contact")
                {
                    InsertQueryiPersonalContact();
                }

                //Done:Insert/Update
                if (comboBox1.Text.Trim() == "- Family")
                {
                    InserQueryiFamily();
                }

                // Done:Insert/Update           
                if (comboBox1.Text.Trim() == "Career Progression")
                {
                    InsertQueryiCareerProgression();
                }

                // Done:Insert/Update
                if (comboBox1.Text.Trim() == "Basic Rate Progression")
                {
                    InsertQueryiBasicRateProgression();    
                }

                if (comboBox1.Text.Trim() == "Fixed Allowance Progression")
                {
                    InsertQueryiEmployeeRecurAllowance();
                }

                // Done:Insert/Update
                if (comboBox1.Text.Trim() == "Work Days/Hrs")
                {
                    InsertQueryiDetailRecord();
                }


                // Done:Insert/Update
                if (comboBox1.Text.Trim() == "Leave")
                {
                    InserQueryiLeaveRecord();
                }

                // Done:Insert/Update
                if (comboBox1.Text.Trim() == "OT Record")
                {
                    InsertQueryiOTRecord();
                }

                // Done 
                if (comboBox1.Text.Trim() == "Allowances")
                {
                    InsertQueryiAllowanceRecord();
                }

                int Inserted = 0;
                int Updated = 0;

                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in dt1.Rows)
                    {
                        if (dr1["status"].ToString().Trim().ToUpper() == "I")
                        {
                            Inserted = Inserted + 1;
                        }
                        if (dr1["status"].ToString().Trim().ToUpper() == "U")
                        {
                            Updated = Updated + 1;
                        }
                    }
                }

            lblResult.Text = "Inserted: "+Inserted.ToString()+"       Updated: "+Updated.ToString()+" ";

            dgv1.Refresh();
          
        }

        private void InsertQueryiPaymentBankInfo()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";
            string InsertQuery3 = "";
            string UpdateQuery3 = "";
            string SQLQuery = "";

            string str1 = "Select  * from HEMPH where [status]<>'V'   ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("HEMPH", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];

            string str4 = "Select  * from iPaymentBankInfo";
            DataTable iPaymentBankInfo = sbConn.GetDataTableByCommandSp("iPaymentBankInfo", str4);

            if (this.dbaccess.DataSet.Tables.Contains("iPaymentBankInfo"))
            {
                this.dbaccess.DataSet.Tables["iPaymentBankInfo"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iPaymentBankInfo");
                this.dbaccess.DataSet.Tables.Add(iPaymentBankInfo);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iPaymentBankInfo);
            }

            ///           

            if (DT.Rows.Count > 0)
            {
                #region Loop through HEMPH lists
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dob"]))
                    {
                        #region iEmployee
                        docInfo = "iEmployee - " + dr1["empnum"].ToString();
                        try
                        {
                            if (iPaymentBankInfoExist(dr1["empnum"].ToString()) == 0)
                            {
                                //BankCode,BranchCode,bankacc
                                InsertQuery3 = "Insert into iPaymentBankInfo " +
                                                     "( " +
                                                         "PayBankEmployeeId " +
                                                         ",PayBankId " +
                                                         ",PayBankBranchId " +
                                                         ",BankAccountNo " +   
                                                         ",PaymentType "+
                                                         ",PaymentMode "+
                                                      ") " +
                                                     "VALUES " +
                                                     "('" +
                                                         dr1["empnum"].ToString() + "' " +
                                                         ",'" + dr1["BankCode"].ToString() + "' " +
                                                         ",'" + dr1["BranchCode"].ToString() + "' " +
                                                         ",'" + dr1["bankacc"].ToString() + "' " +
                                                         ",'Percentage' " +
                                                         ",'Telegraphic' "+
                                                     ")";
                                SQLQuery = InsertQuery3;
                                sbConn.GetExecuteNonQueryByCommand(InsertQuery3);
                                InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "I");
                            }
                            else if (iPaymentBankInfoExist(dr1["empnum"].ToString()) == 1)
                            {
                                if (!BizFunctions.IsEmpty(dr1["dateresigned"]))
                                {
                                    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                    {
                                        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                        {
                                            UpdateQuery3 = "Update iPaymentBankInfo Set " +
                                                                     ",PayBankId='" + dr1["BankCode"].ToString() + "' " +
                                                                     ",PayBankBranchId'" + dr1["BranchCode"].ToString() + "' " +
                                                                     ",BankAccountNo'" + dr1["bankacc"].ToString() + "' " +    
                                                                     ",Processed=0 " +
                                                                     "Where PayBankEmployeeId='" + dr1["empnum"].ToString() + "' ";
                                            SQLQuery = UpdateQuery3;
                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                }
                                //else
                                //{
                                //    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                //    {
                                //        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                //        {
                                //            UpdateQuery3 = "Update iPaymentBankInfo Set " +
                                //                                     ",PayBankId='" + dr1["BankCode"].ToString() + "' " +
                                //                                     ",PayBankBranchId'" + dr1["BranchCode"].ToString() + "' " +
                                //                                     ",BankAccountNo'" + dr1["bankacc"].ToString() + "' " +    
                                //                                     ",Processed=0 " +
                                //                                    "Where PayBankEmployeeId='" + dr1["empnum"].ToString() + "' ";
                                //            SQLQuery = UpdateQuery3;
                                //            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                //            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                //        }
                                //    }
                                //}
                            }
                            else if (iPaymentBankInfoExist(dr1["empnum"].ToString()) == 2)
                            {
                                if (!BizFunctions.IsEmpty(dr1["dateresigned"]))
                                {
                                    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                    {
                                        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                        {
                                            UpdateQuery3 = "Update iPaymentBankInfo Set " +
                                                                      ",PayBankId='" + dr1["BankCode"].ToString() + "' " +
                                                                     ",PayBankBranchId'" + dr1["BranchCode"].ToString() + "' " +
                                                                     ",BankAccountNo'" + dr1["bankacc"].ToString() + "' " +   
                                                                     ",Processed=0 " +
                                                                     "Where PayBankEmployeeId='" + dr1["empnum"].ToString() + "' ";
                                            SQLQuery = UpdateQuery3;
                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                }
                                //else
                                //{
                                //    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                //    {
                                //        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                //        {
                                //            UpdateQuery3 = "Update iPaymentBankInfo Set " +
                                //                                     ",PayBankId='" + dr1["BankCode"].ToString() + "' " +
                                //                                     ",PayBankBranchId'" + dr1["BranchCode"].ToString() + "' " +
                                //                                     ",BankAccountNo'" + dr1["bankacc"].ToString() + "' " +   
                                //                                    ",Processed=0 " +
                                //                                    "Where PayBankEmployeeId='" + dr1["empnum"].ToString() + "' ";
                                //            SQLQuery = UpdateQuery3;
                                //            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                //            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                //        }
                                //    }
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(docInfo, SQLQuery + " : " + ex.Message);
                        }
                        #endregion
                    }

                }
                #endregion

            }
        }

        private void GenerateWhereClause()
        {
            if (whereClause != string.Empty)
            {
                whereClause = string.Empty;
            }
            
            if (
                    !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                    && !BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                    && checkBox1.Checked == true 
                    && comboBox1.Text != string.Empty
                )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + ToEmpnumTb.Text + "' and empnum in (Select empnum from hemph where datejoined>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and datejoined<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ) ";
            }
            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && !BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        &&  checkBox1.Checked == true
                        && checkBox2.Checked == true 
                        && comboBox1.Text != string.Empty
                    )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + ToEmpnumTb.Text + "'  ";
            }
            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox1.Checked == true 
                        && comboBox1.Text != string.Empty
                    )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + FromEmpnumTb.Text + "' and empnum in (Select empnum from hemph where datejoined>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and datejoined<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ) ";
            }
            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox1.Checked == true 
                        && comboBox1.Text != string.Empty
                    )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + FromEmpnumTb.Text + "'  ";
            }
            else if (
                        BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        &&  checkBox1.Checked == true 
                        && comboBox1.Text != string.Empty)
            {
                whereClause = " empnum in (Select empnum from hemph where datejoined>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and datejoined<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ) ";
            }
            ////
            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && !BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox2.Checked == true 
                        && (comboBox1.Text.Trim() == "Career Progression" || comboBox1.Text.Trim() == "Basic Rate Progression")
                    )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + ToEmpnumTb.Text + "' and dateadjusted>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and dateadjusted<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "'  ";
            }

            else if (
                   !BizFunctions.IsEmpty(FromEmpnumTb.Text)
                   && BizFunctions.IsEmpty(ToEmpnumTb.Text)
                   && checkBox2.Checked == true
                   && (comboBox1.Text.Trim() == "Career Progression" || comboBox1.Text.Trim() == "Basic Rate Progression")
               )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + FromEmpnumTb.Text + "' and dateadjusted>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and dateadjusted<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "'  ";
            }

            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && !BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox1.Checked == false && checkBox2.Checked == false 
                        && comboBox1.Text != string.Empty
                    )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + ToEmpnumTb.Text + "'  ";
            }
            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox2.Checked == true && (comboBox1.Text.Trim() == "Career Progression" || comboBox1.Text.Trim() == "Basic Rate Progression")
                    )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + FromEmpnumTb.Text + "' and dateadjusted>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and dateadjusted<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ";
            }

            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox2.Checked == true && (comboBox1.Text.Trim() == "Work Days/Hrs" || comboBox1.Text.Trim() == "OT Record" || comboBox1.Text.Trim() == "Allowances")
                    )//
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + FromEmpnumTb.Text + "' and [date]>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and [date]<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ";
            }
            else if (
                   !BizFunctions.IsEmpty(FromEmpnumTb.Text)
                   && !BizFunctions.IsEmpty(ToEmpnumTb.Text)
                   && checkBox2.Checked == true && (comboBox1.Text.Trim() == "Work Days/Hrs" || comboBox1.Text.Trim() == "OT Record" || comboBox1.Text.Trim() == "Allowances")
               )//
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + ToEmpnumTb.Text + "' and [date]>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and [date]<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ";
            }

            else if (
                        !BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox1.Checked == true 
                        && comboBox1.Text != string.Empty
                    )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + FromEmpnumTb.Text + "'  ";
            }
            else if (
                        BizFunctions.IsEmpty(FromEmpnumTb.Text) 
                        && BizFunctions.IsEmpty(ToEmpnumTb.Text) 
                        && checkBox1.Checked == true 
                        && comboBox1.Text != string.Empty
                    )
            {
                whereClause = " empnum in (Select empnum from hemph where datejoined>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and datejoined<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ) ";
            }

                //
            else if (
              !BizFunctions.IsEmpty(FromEmpnumTb.Text)
              && !BizFunctions.IsEmpty(ToEmpnumTb.Text)
              && checkBox2.Checked == true
              && (comboBox1.Text.Trim() == "Leave")
          )//
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + ToEmpnumTb.Text + "' and leavefrom>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and leaveto<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ";
            }

            else if (
                 !BizFunctions.IsEmpty(FromEmpnumTb.Text)
                 && BizFunctions.IsEmpty(ToEmpnumTb.Text)
                 && checkBox2.Checked == true
                 && (comboBox1.Text.Trim() == "Leave")
             )
            {
                whereClause = "  empnum>='" + FromEmpnumTb.Text + "' and empnum<='" + FromEmpnumTb.Text + "' and leavefrom>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and leaveto<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "'  ";
            }

            else if (
                   BizFunctions.IsEmpty(FromEmpnumTb.Text)
                   && BizFunctions.IsEmpty(ToEmpnumTb.Text)
                   && checkBox2.Checked == true
                   && (comboBox1.Text.Trim() == "Leave")
               )
            {
                whereClause = " leavefrom>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker1.Text)) + "' and leaveto<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateTimePicker2.Text)) + "' ";
            }
            
            
            string test = "";


    
           
            //// Done:Insert/Update           
            //if (comboBox1.Text.Trim() == "Career Progression")
            //{
            //    InsertQueryiCareerProgression();
            //}

            //// Done:Insert/Update
            //if (comboBox1.Text.Trim() == "Basic Rate Progression")
            //{
            //    InsertQueryiBasicRateProgression();
            //}

            //// Done:Insert/Update
            //if (comboBox1.Text.Trim() == "Work Days/Hrs")
            //{
            //    InsertQueryiDetailRecord();
            //}


            //// Done
            //if (comboBox1.Text.Trim() == "Leave")
            //{
            //    InserQueryiLeaveRecord();
            //}

            //// Done
            //if (comboBox1.Text.Trim() == "OT Record")
            //{
            //    InsertQueryiOTRecord();
            //}

            //// Done 
            //if (comboBox1.Text.Trim() == "Allowances")
            //{
            //    InsertQueryiAllowanceRecord();
            //}
        }

        private void InsertQueryiOTRecord()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select * from ATR1 where [status]<>'V' and ISNULL(ActualOTHrs,0)>0 and ISNULL(isomit,0)=0 ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("ATR1_OT", str1);

            string str2 = "Select  * from iOTRecord ";            
            DataTable iOTRecord = sbConn.GetDataTableByCommandSp("iOTRecord", str2);


            if (this.dbaccess.DataSet.Tables.Contains("iOTRecord"))
            {
                this.dbaccess.DataSet.Tables["iOTRecord"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iOTRecord");
                this.dbaccess.DataSet.Tables.Add(iOTRecord);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iOTRecord);
            }



            DataTable DT = this.dbaccess.DataSet.Tables["ATR1_OT"];

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["trandate"]))
                    {
                        docInfo = "iOTRecord - " + dr1["refnum"].ToString() + " - " + dr1["empnum"].ToString();
                        try
                        {
                            string InsertQuery = "";
                            if (BizFunctions.IsEmpty(dr1["actualOT1"]))
                            {
                                dr1["actualOT1"] = 0;
                            }
                            if (BizFunctions.IsEmpty(dr1["actualOT15"]))
                            {
                                dr1["actualOT15"] = 0;
                            }
                            if (BizFunctions.IsEmpty(dr1["actualOT2"]))
                            {
                                dr1["actualOT2"] = 0;
                            }

                            if ((bool)dr1["actualOT1"])
                            {

                                if (iOTRecordExists(dr1["empnum"].ToString(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())), getSageMasterValue("ATR1", "OT", "OT1")) == 0)
                                {
                                    InsertQuery = "Insert into iOTRecord (OTEmployeeID,OTID,OTDate,PayRecID,CurrentOTFreq) " +
                                                 "VALUES " +
                                                 "('" + dr1["empnum"].ToString() + "','" + getSageMasterValue("ATR1", "OT", "OT1") + "','" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())) + "','Normal'," + dr1["ActualOTHrs"].ToString() + ") ";
                                }

                            }
                            else if ((bool)dr1["actualOT15"])
                            {
                                if (iOTRecordExists(dr1["empnum"].ToString(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())), getSageMasterValue("ATR1", "OT", "OT1")) == 0)
                                {
                                    InsertQuery = "Insert into iOTRecord (OTEmployeeID,OTID,OTDate,PayRecID,CurrentOTFreq) " +
                                                 "VALUES " +
                                                 "('" + dr1["empnum"].ToString() + "', '" + getSageMasterValue("ATR1", "OT", "OT15") + "','" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())) + "','Normal'," + dr1["ActualOTHrs"].ToString() + ") ";
                                }

                            }
                            else if ((bool)dr1["actualOT2"])
                            {
                                if (iOTRecordExists(dr1["empnum"].ToString(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())), getSageMasterValue("ATR1", "OT", "OT1")) == 0)
                                {
                                    InsertQuery = "Insert into iOTRecord (OTEmployeeID,OTID,OTDate,PayRecID,CurrentOTFreq) " +
                                                 "VALUES " +
                                                 "('" + dr1["empnum"].ToString() + "','" + getSageMasterValue("ATR1", "OT", "OT2") + "','" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())) + "','Normal'," + dr1["ActualOTHrs"].ToString() + ") ";
                                }

                            }

                            if (InsertQuery != string.Empty)
                            {
                                sbConn.GetExecuteNonQueryByCommand(InsertQuery);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(),this.dbaccess), "I");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(docInfo, ex.Message);
                        }
                    }
                }
            }

        }

        private void InsertQueryiCareerProgression()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "";
            if (whereClause != string.Empty)
            {
                str1 = "	SELECT * FROM " +
                           "( " +
                           "Select " +
                               "salcode, " +
                               "refnum, " +
                               "empnum, " +
                               "dateadjusted , " +
                               "'' as approvedcomments, " +
                               "CASE WHEN refnum=empnum THEN 'FirstRecord' ELSE sadjmcode end as sadjmcode, " +
                               "1 as CareerCurrent, " +
                               "'' as BranchID, " +
                               "'' as CategoryId, " +
                               "sitenum, " +
                               "'' as SectionID, " +
                               "'' as ClassificationCode, " +
                               "hsgcode, " +
                               "matnum " +
                           "from SALH  " +
                           ")B " +
                           "Where "+ whereClause + " "+
                           "GROUP BY salcode,refnum,empnum,dateadjusted,approvedcomments,sadjmcode,CareerCurrent,BranchID,CategoryId,sitenum,SectionID,ClassificationCode,hsgcode,matnum";

            }
            else
            {
                str1 = "	SELECT * FROM " +
                           "( " +
                           "Select " +
                               "salcode, " +
                               "refnum, " +
                               "empnum, " +
                               "dateadjusted , " +
                               "'' as approvedcomments, " +
                               "CASE WHEN refnum=empnum THEN 'FirstRecord' ELSE sadjmcode end as sadjmcode, " +
                               "1 as CareerCurrent, " +
                               "'' as BranchID, " +
                               "'' as CategoryId, " +
                               "sitenum, " +
                               "'' as SectionID, " +
                               "'' as ClassificationCode, " +
                               "hsgcode, " +
                               "matnum " +
                           "from SALH  " +
                           ")B " +
                           "GROUP BY salcode,refnum,empnum,dateadjusted,approvedcomments,sadjmcode,CareerCurrent,BranchID,CategoryId,sitenum,SectionID,ClassificationCode,hsgcode,matnum";

            }


            this.dbaccess.ReadSQL("SALH_2", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["SALH_2"];



            ///

            string str2 = "Select  * from iCareerProgression ";
            DataTable iCareerProgression = sbConn.GetDataTableByCommandSp("iCareerProgression", str2);

            if (this.dbaccess.DataSet.Tables.Contains("iCareerProgression"))
            {
                this.dbaccess.DataSet.Tables["iCareerProgression"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iCareerProgression");
                this.dbaccess.DataSet.Tables.Add(iCareerProgression);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iCareerProgression);
            }

            int recordNo = 0;
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dateadjusted"]))
                    {
                        docInfo = "iCareerProgression - " + dr1["salcode"].ToString() + " - " + dr1["empnum"].ToString();

                        if (iCareerProgressionExist(dr1["empnum"].ToString().Trim(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])), getSageMasterValue("SADJH", "sadjmcode", dr1["sadjmcode"].ToString())) == 0)
                        {
                            try
                            {
                                string InsertQuery1 = "Insert Into iCareerProgression " +
                                                        "( " +
                                                        "CareerEmployeeId  " +
                                                        ",CareerEffectiveDate " +
                                                        ",CareerRemarks " +
                                                        ",CareerCareerId " +
                                                        ",CareerCurrent " +
                                                        ",BranchID " +
                                                        ",CategoryId " +
                                                        ",DepartmentId " +
                                                        ",SectionID " +
                                                        ",ClassificationCode " +
                                                        ",SalaryGradeId " +
                                                        ",PositionID " +
                                                        ") " +
                                                        "VALUES " +
                                                         "( '" +
                                                         dr1["empnum"].ToString() + "' " +
                                                         ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])) + "' " +
                                                         ", '" + dr1["approvedcomments"].ToString() + "' " +
                                                         ",'" + getSageMasterValue("SADJH", "sadjmcode", dr1["sadjmcode"].ToString()) + "' " +
                                    //",1 " +
                                                         ", " + isCurrentProgression(dr1["empnum"].ToString(), dr1["salcode"].ToString()) + " " +
                                                         ", 'None'" +
                                                         ", 'None'" +
                                                         ", '" + dr1["sitenum"].ToString() + "' " +
                                                         ", 'None'" +
                                                         ", 'None'" +
                                                         ", '" + dr1["hsgcode"].ToString() + "'" +
                                                         ", '" + dr1["matnum"].ToString() + "'" +
                                                         ") ";

                                sbConn.GetExecuteNonQueryByCommand(InsertQuery1);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(),this.dbaccess), "I");

                                recordNo = recordNo + 1;
                            }
                            catch (Exception ex)
                            {
                                LogError(docInfo, ex.Message);
                            }
                        }

                    }
                }
            }
        }

        private void InsertQueryiBasicRateProgression()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select * from salh where empnum is not null and hsamcode='BASIC' and ISNULL(rateamt,0)>0  ";

            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }

            this.dbaccess.ReadSQL("SALH", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["SALH"];


            string str2 = "Select  * from iBasicRateProgression ";
            DataTable iBasicRateProgression = sbConn.GetDataTableByCommandSp("iBasicRateProgression", str2);

            if (this.dbaccess.DataSet.Tables.Contains("iBasicRateProgression"))
            {
                this.dbaccess.DataSet.Tables["iBasicRateProgression"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iBasicRateProgression");
                this.dbaccess.DataSet.Tables.Add(iBasicRateProgression);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iBasicRateProgression);
            }


            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dateadjusted"]))
                    {
                        docInfo = "iBasicRateProgression - " + dr1["salcode"].ToString() + " - " + dr1["empnum"].ToString();

                        if (iBasicRateProgressionExist(dr1["empnum"].ToString().Trim(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])), "FirstRecord") == 0 && dr1["empnum"].ToString().Trim() == dr1["refnum"].ToString().Trim())
                        //if (dr1["empnum"].ToString().Trim() == dr1["refnum"].ToString().Trim())
                        {
                            try
                            {
                                string InsertQuery2 = "Insert Into iBasicRateProgression " +
                                                       "( " +
                                                        "BREmployeeId, " +
                                                        "BRProgDate, " +
                                                        "BRProgEffectiveDate, " +
                                                        "BRProgCareerId,  " +
                                                        "BRProgNewBasicRate,  " +
                                                        "BRProgRemarks " +
                                                       ") " +
                                                       "VALUES " +
                                                        "( '" +
                                                        dr1["empnum"].ToString() + "' " +
                                                        ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])) + "' " +
                                                        ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])) + "' " +
                                                         ",'FirstRecord' " +
                                                         ", " + dr1["rateamt"].ToString() + " " +
                                                        ", '' " +
                                                        ") ";

                                sbConn.GetExecuteNonQueryByCommand(InsertQuery2);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(),this.dbaccess), "I");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else if (iBasicRateProgressionExist(dr1["empnum"].ToString().Trim(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])), getSageMasterValue("SADJH", "sadjmcode", dr1["sadjmcode"].ToString())) == 0 && dr1["empnum"].ToString().Trim() != dr1["refnum"].ToString().Trim())
                        {
                            try
                            {
                                string InsertQuery2 = "Insert Into iBasicRateProgression " +
                                                       "( " +
                                                        "BREmployeeId, " +
                                                        "BRProgDate, " +
                                                        "BRProgEffectiveDate, " +
                                                        "BRProgCareerId,  " +
                                                        "BRProgNewBasicRate,  " +
                                                        "BRProgRemarks " +
                                                       ") " +
                                                       "VALUES " +
                                                        "( '" +
                                                        dr1["empnum"].ToString() + "' " +
                                                        ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])) + "' " +
                                                        ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"])) + "' " +
                                                         ",'" + getSageMasterValue("SADJH", "sadjmcode", dr1["sadjmcode"].ToString()) + "' " +
                                                         ", " + dr1["rateamt"].ToString() + " " +
                                                        ", '' " +
                                                        ") ";

                                sbConn.GetExecuteNonQueryByCommand(InsertQuery2);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(),this.dbaccess), "I");
                            }
                            catch (Exception ex)
                            {
                                LogError(docInfo, ex.Message);
                            }
                        }                 
                    }
                }
            }
        }

        private void InsertQueryiDetailRecord()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select * from atr1 where [status]<>'V' and ISNULL(isomit,0)=0  and shiftcode in "+
                            "( "+
                            "select shiftcode from vshlv where isworkshift=1 "+
                            ") ";

            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }

            this.dbaccess.ReadSQL("ATRiDetailRecord", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["ATRiDetailRecord"];

            string str2 = "Select  * from iDetailRecord ";
            DataTable iDetailRecord = sbConn.GetDataTableByCommandSp("iDetailRecord", str2);

            if (this.dbaccess.DataSet.Tables.Contains("iDetailRecord"))
            {
                this.dbaccess.DataSet.Tables["iDetailRecord"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iDetailRecord");
                this.dbaccess.DataSet.Tables.Add(iDetailRecord);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iDetailRecord);
            }

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["trandate"]))
                    {
                        docInfo = "IDetailRecord - " + dr1["refnum"].ToString() + " - " + dr1["empnum"].ToString();
                        if (iDetailRecordExists(dr1["empnum"].ToString(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString()))) == 0)
                        {
                            try
                            {
                                string InsertQuery = "Insert into IDetailRecord " +
                                                     "( " +
                                                        "DetailEmployeeID " +
                                                        ",DetailDate " +
                                                        ",PayRecID " +
                                                        ",CurrentHrDays " +
                                                        ") " +
                                                 "VALUES " +
                                                 "('" +
                                                     dr1["empnum"].ToString() + "' " +
                                                     ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())) + "' " +
                                    //",'" + dr1["paytypecode"].ToString() + "' "+
                                                     ",'Normal' " +
                                                     "," + Convert.ToInt32(dr1["attnmark"]).ToString() + " " +
                                                 ")";

                                sbConn.GetExecuteNonQueryByCommand(InsertQuery);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(),this.dbaccess), "I");
                            }
                            catch (Exception ex)
                            {
                                LogError(docInfo, ex.Message);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Select Query
        private void SelectQuery()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);

            SqlString = "Select * from hemph";

            DataTable ReturnTable = null;
            FromTable = "HEMPH";
            ToTable = "";
            FromDb = "";
            ToDb = "";
            string QueryToTb, QueryFromTb = "";
            string columnLink1 = "";
            string[] columnSeparated, columnLink2, FromColumn,InsertValueColumn;
            string[,] columnMultiArray1;

            DataTable LinkTable = dfConn.GetDataTableByCommandTxt("Tb", "select * from InterfaceLink where FromTable='" + FromTable + "' ");


            if (LinkTable != null)
            {
                if (LinkTable.Rows.Count > 0)
                {

                    string sqlScript = LinkTable.Rows[0]["sqlScript"].ToString();

                    ReturnTable = sbConn.GetDataTableByCommandTxt(FromTable, sqlScript);

                    //ToTable = LinkTable.Rows[0]["ToTable"].ToString();

                    //columnLink1 = LinkTable.Rows[0]["columnLink"].ToString();

                    //columnSeparated = columnLink1.Split('|');

                    //columnMultiArray1 = new string[1, columnSeparated.Length];

                    //QueryToTb = "Select";



                    //if (SqlString.Contains("*"))
                    //{
                    //    for (int i = 0; i < columnSeparated.Length; i++)
                    //    {
                    //        columnLink2 = columnSeparated[i].ToString().ToUpper().Split('*');

                    //        if (i == 0)
                    //        {
                    //            QueryToTb = QueryToTb + " " + columnLink2[1].ToString() + " AS " + columnLink2[0].ToString().Trim() + " ";
                    //        }
                    //        else
                    //        {
                    //            QueryToTb = QueryToTb + ", " + columnLink2[1].ToString() + " AS " + columnLink2[0].ToString().Trim() + " ";
                    //        }

                    //    }

                    //    QueryToTb = QueryToTb + "From " + ToTable + " ";


                    //    }
                    //    else
                    //    {
                    //        FromColumn = GetStringBetween(SqlString.ToUpper(), "SELECT", "FROM").Split(',');
                    //        for (int i = 0; i < columnSeparated.Length; i++)
                    //        {
                    //            columnLink2 = columnSeparated[i].ToString().ToUpper().Split('*');

                    //            for (int x = 0; x < FromColumn.Length; x++)
                    //            {
                    //                if (columnLink2[0].ToString().Trim() == FromColumn[x].ToString().Trim())
                    //                {
                    //                    if (x == 0)
                    //                    {
                    //                        QueryToTb = QueryToTb + " " + columnLink2[1].ToString() + " AS " + columnLink2[0].ToString().Trim() + " ";
                    //                    }
                    //                    else
                    //                    {
                    //                        QueryToTb = QueryToTb + ", " + columnLink2[1].ToString() + " AS " + columnLink2[0].ToString().Trim() + " ";
                    //                    }
                    //                }
                    //            }

                    //        }

                    //        QueryToTb = QueryToTb + "From " + ToTable + " ";

                    //        ReturnTable = sbConn.GetDataTableByCommandTxt(FromTable, QueryToTb);
                    //    }

                    //}
                }
            }

        }
        #endregion

        #region Insert Query
        //private void InsertQuery()
        //{
        //    Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
        //    Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);

        //    SqlString = "Insert into HEMPH (nric,nationality,empname) VALUES ('NRIC','NATIONALITY','EMPNAME')";

        //    DataTable ReturnTable = null;
        //    FromTable = "HEMPH";
        //    ToTable = "";
        //    FromDb = "";
        //    ToDb = "";
        //    string QueryToTb, QueryFromTb = "";
        //    string columnLink1 = "";
        //    string[] columnSeparated, columnLink2, FromColumn, InsertValueColumn1;
        //    string[,] columnMultiArray1;

        //    DataTable LinkTable = dfConn.GetDataTableByCommandTxt("Tb", "select * from InterfaceLink where FromTable='" + FromTable + "' ");


        //    if (LinkTable != null)
        //    {
        //        if (LinkTable.Rows.Count > 0)
        //        {
        //            ToTable = LinkTable.Rows[0]["ToTable"].ToString();

        //            columnLink1 = LinkTable.Rows[0]["columnLink"].ToString();

        //            columnSeparated = columnLink1.Split('|');

        //            columnMultiArray1 = new string[1, columnSeparated.Length];


        //            QueryToTb = "Insert Into " +ToTable+" (";

        //            for (int i = 0; i < columnSeparated.Length; i++)
        //            {
        //                columnLink2 = columnSeparated[i].ToString().ToUpper().Split('*');

        //                if (i == 0)
        //                {
        //                    QueryToTb = QueryToTb + " " + columnLink2[1].ToString() + "  ";
        //                }
        //                else
        //                {
        //                    QueryToTb = QueryToTb + ", " + columnLink2[1].ToString() +" ";
        //                }

        //                if (i == columnSeparated.Length - 1)
        //                {
        //                    QueryToTb = QueryToTb + " ) VALUES ";
        //                }
        //            }

        //            int frontIndex1 = SqlString.IndexOf("VALUES")+6;
        //            string InsertValueTemp1 = SqlString.Substring(frontIndex1, SqlString.Length - frontIndex1);


        //            int frontIndex2 = InsertValueTemp1.IndexOf("(");
        //            InsertValueColumn1 = InsertValueTemp1.Substring(frontIndex2 + 1, InsertValueTemp1.Length - (frontIndex2 + 2)).Split(',');


        //            for (int y = 0; y < InsertValueColumn1.Length; y++)
        //            {
        //                columnLink2 = columnSeparated[y].ToString().ToUpper().Split('*');

        //                InsertValueColumn1[y] = InsertValueColumn1[y].ToString().Replace('(', ' ').Replace(')', ' ');

        //                if (y == 0)
        //                {
        //                    QueryToTb = QueryToTb + " ( " + InsertValueColumn1[y].ToString().Trim() + "  ";
        //                }
        //                else
        //                {
        //                    QueryToTb = QueryToTb + ", " + InsertValueColumn1[y].ToString().Trim() + " ";
        //                }

        //                if (y == columnSeparated.Length - 1)
        //                {
        //                    QueryToTb = QueryToTb + " )  ";
        //                }
        //            }


        //            sbConn.GetExecuteNonQueryByCommand(QueryToTb);
        //        }
        //    }

        //}


        private void InsertQuery()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);

         
            DataTable ReturnTable = null;
            FromTable = "HEMPH";
            ToTable = "";
            FromDb = "";
            ToDb = "";
            string QueryToTb, QueryFromTb = "";
            string columnLink1 = "";
            string[] columnSeparated, columnLink2, FromColumn, InsertValueColumn1;
            string[,] columnMultiArray1;

            DataTable LinkTable = dfConn.GetDataTableByCommandTxt("Tb", "select * from InterfaceLink where FromTable='" + FromTable + "' ");
            string sqlString = "";
            if (LinkTable != null)
            {
                if (LinkTable.Rows.Count > 0)
                {
                    sqlString = "Insert into " + LinkTable.Rows[0]["ToTable"].ToString() + " ( ";

                    columnLink1 = LinkTable.Rows[0]["columnLink"].ToString();

                    columnSeparated = columnLink1.Split('|');

                    columnMultiArray1 = new string[1, columnSeparated.Length];


                     for (int i = 0; i < columnSeparated.Length; i++)
                     {
                         columnLink2 = columnSeparated[i].ToString().ToUpper().Split('*');

                         if (i == 0)
                         {
                             SqlString = sqlString + " " + columnLink2[1].ToString() + "  ";
                         }
                         else
                         {
                             SqlString = sqlString + " " + columnLink2[1].ToString() + "  ";
                         }

                         if (i == columnSeparated.Length - 1)
                         {
                             SqlString = SqlString + " ) VALUES ";
                         }
                     }
                }
            }

           
           
            

        }


        private void InsertQueryiPersonalAll()
        {

            #region Old Code: 23/06/2015
            //Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            //Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);

            //string str1 = "Select  * from HEMPH where [status]<>'V' AND flag='HEMP' and empnum='12767' ";            
            //this.dbaccess.ReadSQL("HEMPH", str1);

            /////

            //string str2 = "Select  * from iPersonal ";
            //DataTable iPersonal = sbConn.GetDataTableByCommandSp("iPersonal", str2);

            //DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];

            //if (this.dbaccess.DataSet.Tables.Contains("iPersonal"))
            //{
            //    this.dbaccess.DataSet.Tables["iPersonal"].Dispose();
            //    this.dbaccess.DataSet.Tables.Remove("iPersonal");
            //    this.dbaccess.DataSet.Tables.Add(iPersonal);
            //}
            //else
            //{
            //    this.dbaccess.DataSet.Tables.Add(iPersonal);
            //}

            /////

            //string str3 = "Select  * from iResidenceStatusRecord ";
            //DataTable iResidenceStatusRecord = sbConn.GetDataTableByCommandSp("iResidenceStatusRecord", str3);

            //if (this.dbaccess.DataSet.Tables.Contains("iResidenceStatusRecord"))
            //{
            //    this.dbaccess.DataSet.Tables["iResidenceStatusRecord"].Dispose();
            //    this.dbaccess.DataSet.Tables.Remove("iResidenceStatusRecord");
            //    this.dbaccess.DataSet.Tables.Add(iResidenceStatusRecord);
            //}
            //else
            //{
            //    this.dbaccess.DataSet.Tables.Add(iResidenceStatusRecord);
            //}

            /////

            ////iEmployee


            //string str4 = "Select  * from iEmployee";
            //DataTable iEmployee= sbConn.GetDataTableByCommandSp("iEmployee", str4);

            //if (this.dbaccess.DataSet.Tables.Contains("iEmployee"))
            //{
            //    this.dbaccess.DataSet.Tables["iEmployee"].Dispose();
            //    this.dbaccess.DataSet.Tables.Remove("iEmployee");
            //    this.dbaccess.DataSet.Tables.Add(iEmployee);
            //}
            //else
            //{
            //    this.dbaccess.DataSet.Tables.Add(iEmployee);
            //}

            //if (DT.Rows.Count > 0)
            //{
            //    #region Loop through HEMPH lists
            //    foreach (DataRow dr1 in DT.Rows)
            //    {
            //        if (!BizFunctions.IsEmpty(dr1["dob"]))
            //        {
            //            #region iPersonal

            //            try
            //            {
            //                if (iPersonalExist(dr1["nric"].ToString()) == 0)
            //                {
            //                    string InsertQuery1 = "Insert into iPersonal " +
            //                                         "( " +
            //                                             "IdentityNo " +
            //                                             ",NewIdentityNo " +
            //                                             ",IdentityTypeId " +
            //                                             ",PersonalTypeid " +
            //                                             ",MaritalStatusCode " +
            //                                             ",TitleId " +
            //                                             ",RaceId " +
            //                                             ",ReligionID " +
            //                                             ",CountryOfBirth " +
            //                                             ",DateOfBirth " +
            //                                             ",Gender " +
            //                                             ",Nationality " +
            //                                             ",PersonalName " +
            //                                             ",BloodGroupId " +
            //                                         ") " +
            //                                         "VALUES " +
            //                                         "('" +
            //                                             dr1["nric"].ToString() + "' " +
            //                                             ",'" + dr1["nric"].ToString() + "' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "nrictype", dr1["nrictype"].ToString()) + "' " +
            //                                             ",'Staff' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString()) + "' " +
            //                                             ",'" + GetTitleCode(Convert.ToInt32(getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString())), getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString())) + "' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "hramdesc", dr1["hramdesc"].ToString()) + "' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "religion", dr1["religion"].ToString()) + "' " +
            //                                             ",'" + UppercaseFirst(dr1["country"].ToString()) + "' " +
            //                                             ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dob"])) + "' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString()) + "' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "nationality", GetNationality(dr1["nric"].ToString())) + "' " +
            //                                             ",'" + dr1["empname"].ToString() + "' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "bloodtype", dr1["bloodtype"].ToString()) + "' " +
            //                                         ")";

            //                    sbConn.GetExecuteNonQueryByCommand(InsertQuery1);
            //                }
            //                else if (iPersonalExist(dr1["nric"].ToString()) == 1)
            //                {
            //                    string UpdateQuery1 = "Update iPersonal Set "+
            //                                             "IdentityTypeId='" + getSageMasterValue("HEMPH", "nrictype", dr1["nrictype"].ToString()) + "' " +
            //                                             ",MaritalStatusCode='" + getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString()) + "' " +
            //                                             ",TitleId='" + GetTitleCode(Convert.ToInt32(getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString())), getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString())) + "' " +
            //                                             ",RaceId='" + getSageMasterValue("HEMPH", "hramdesc", dr1["hramdesc"].ToString()) + "' " +
            //                                             ",ReligionID='" + getSageMasterValue("HEMPH", "religion", dr1["religion"].ToString()) + "' " +
            //                                             ",CountryOfBirth='" + UppercaseFirst(dr1["country"].ToString()) + "' " +
            //                                             ",DateOfBirth='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dob"])) + "' " +
            //                                             ",Gender='" + getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString()) + "' " +
            //                                             ",Nationality='" + getSageMasterValue("HEMPH", "nationality", GetNationality(dr1["nric"].ToString())) + "' " +
            //                                             ",PersonalName='" + dr1["empname"].ToString() + "' " +
            //                                             ",BloodGroupId='" + getSageMasterValue("HEMPH", "bloodtype", dr1["bloodtype"].ToString()) + "' " +
            //                                           "Where IdentityNo='" + dr1["nric"].ToString() + "' ";
            //                    sbConn.GetExecuteNonQueryByCommand(UpdateQuery1);
            //                }
            //                else if (iPersonalExist(dr1["nric"].ToString()) == 2)
            //                {
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }

            //            #endregion

            //            #region iResidenceStatusRecord
            //            DataTable ResidenceDT = GetResidenceDT(dr1["nric"].ToString());

            //            if (ResidenceDT != null)
            //            {
            //                if (ResidenceDT.Rows.Count > 0)
            //                {
            //                    foreach (DataRow drResidence in ResidenceDT.Rows)
            //                    {
            //                        try
            //                        {
            //                            if (iResidenceStatusRecordExist(dr1["nric"].ToString(), getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()), BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"]))) == 0)
            //                            {

            //                                //iResidenceStatusRecordExist
            //                                string InsertQuery2 = "";
            //                                if (!BizFunctions.IsEmpty(drResidence["effectivedate"]))
            //                                {
            //                                    InsertQuery2 = "Insert into iResidenceStatusRecord " +
            //                                                    "( " +
            //                                                        "ResIdentityNo " +
            //                                                        ",ResStatusEffectiveDate " +
            //                                                        ",ResidenceTypeId " +
            //                                                        ",ResStatusRemarks " +
            //                                                    ") " +
            //                                                    "VALUES " +
            //                                                    "('" +
            //                                                         dr1["nric"].ToString() + "' " +
            //                                                         ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"])) + "' " +
            //                                                         ",'" + getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()) + "' " +
            //                                                         ",'" + drResidence["remark"].ToString() + "' " +
            //                                                    ")";

            //                                    sbConn.GetExecuteNonQueryByCommand(InsertQuery2);
            //                                }
                                            
                                           
            //                            }
            //                            else if (iResidenceStatusRecordExist(dr1["nric"].ToString(), getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()), BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"]))) == 1)
            //                            {
            //                                string UpdateQuery2 = "Update iResidenceStatusRecord Set " +
            //                                                        "ResStatusEffectiveDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"])) + "' " +
            //                                                        ",ResidenceTypeId='" + getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()) + "' " +
            //                                                        ",ResStatusRemarks='" + drResidence["remark"].ToString() + "' " +
            //                                                        "Where ResIdentityNo='" + dr1["nric"].ToString() + "' and ResidenceTypeId='" + getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()) + "' and ResStatusEffectiveDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"])) + "'  ";

            //                                sbConn.GetExecuteNonQueryByCommand(UpdateQuery2);
            //                            }
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            MessageBox.Show(ex.Message);
            //                        }

            //                    }

            //                }
            //            }
            //            #endregion

            //            #region iEmployee

            //            try
            //            {
            //                if (iEmployeeExist(dr1["empnum"].ToString()) == 0)
            //                {
            //                    string InsertQuery3 = "Insert into iEmployee " +
            //                                         "( " +
            //                                             "EmployeeId " +
            //                                             ",NewEmployeeId " +
            //                                             ",EIdentityNo " +
            //                                             ",HireDate " +
            //                                             ",BranchId " +
            //                                             ",PositionId " +
            //                                             ",CessationCode " +
            //                                             ",CategoryId " +
            //                                             ",DepartmentId " +
            //                                             ",SectionId " +
            //                                             ",CurrentBasicRateType " +
            //                                             ",ClassificationCode " +
            //                                             ",SalaryGradeId " +
            //                                             ",CostCentreId " +
            //                                          ") " +
            //                                         "VALUES " +
            //                                         "('" +
            //                                             dr1["empnum"].ToString() + "' " +
            //                                             ",'" + dr1["empnum"].ToString() + "' " +
            //                                             ",'" + dr1["nric"].ToString() + "' " +
            //                                             ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
            //                                             ",'None' " +
            //                                             ",'None' " +
            //                                             ",'None' " +
            //                                             ",'None' " +
            //                                            ",'" + dr1["sitenum"].ToString() + "' " +
            //                                             ",'None' " +
            //                                             ",'" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "' " +
            //                                             ",'Permanent' " +
            //                                             ",'" + dr1["hsgcode"].ToString() + "' " +
            //                                             ",'CLN' " +
            //                                         ")";
            //                    sbConn.GetExecuteNonQueryByCommand(InsertQuery3);
            //                }
            //                else if (iEmployeeExist(dr1["empnum"].ToString()) == 1)
            //                {
            //                    string UpdateQuery3 = "Update iEmployee Set "+
            //                                             ",EIdentityNo='" + dr1["nric"].ToString() + "' " +
            //                                             ",HireDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
            //                                             ",BranchId='None' " +
            //                                             ",PositionId='None' " +
            //                                             ",CessationCode='None' " +
            //                                             ",CategoryId='None' " +
            //                                             ",DepartmentId='" + dr1["sitenum"].ToString() + "'  " +
            //                                             ",SectionId='None' " +
            //                                             ",CurrentBasicRateType='" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "'  " +
            //                                             ",ClassificationCode='Permanent' " +
            //                                             ",SalaryGradeId='" + dr1["hsgcode"].ToString() + "' " +
            //                                             ",CostCentreId='CLN' " +
            //                                             "Where EmployeeId='" + dr1["empnum"].ToString() + "' ";

            //                    sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }
            //            #endregion

            //            #region iPersonalAddress

            //            try
            //            {
            //                string InsertQuery4 = "Insert into iPersonalAddress " +
            //                                     "( " +
            //                                         "PerAddIdentityNo " +
            //                                         ",PerAddAddress1 " +
            //                                         ",PerAddAddress2 " +
            //                                         ",PerAddAddress3 " +
            //                                         ",PerAddCountry " +
            //                                         ",PerAddConLocId " +
            //                                     ") " +
            //                                     "VALUES " +
            //                                     "('" +
            //                                           dr1["nric"].ToString() + "' " +
            //                                           ",'" + dr1["cadd1"].ToString() + "' " +
            //                                           ",'" + dr1["cadd2"].ToString() + "' " +
            //                                           ",'" + dr1["cadd3"].ToString() + "' " +
            //                                           ",'" + UppercaseFirst(dr1["country"].ToString()) + "' " +
            //                                            ",'Others' " +
            //                                       ")";
            //                //sbConn.GetExecuteNonQueryByCommand(InsertQuery4);
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }

            //            #endregion

            //            #region iPersonalContact
            //            try
            //            {
            //                string InsertQuery5_1 = "";
            //                string InsertQuery5_2 = "";
            //                if (!BizFunctions.IsEmpty(dr1["contact"]))
            //                {
            //                    if (dr1["contact"].ToString().Trim() != "-")
            //                    {
            //                        InsertQuery5_1 = "Insert into iPersonalContact " +
            //                                             "( " +
            //                                                 "PerConIdentityNo " +
            //                                                 ",PerContactConLocId " +
            //                                                 ",ContactNumber " +
            //                                             ") " +
            //                                             "VALUES " +
            //                                             "('" +
            //                                                 dr1["nric"].ToString() + "' " +
            //                                                 ",'Handphone' " +
            //                                                 ",'" + dr1["contact"].ToString() + "' " +
            //                                             ")";
            //                        //sbConn.GetExecuteNonQueryByCommand(InsertQuery5_1);
            //                    }
            //                }
            //                if (!BizFunctions.IsEmpty(dr1["homecontactno"]))
            //                {
            //                    if (dr1["homecontactno"].ToString().Trim() != "-")
            //                    {
            //                        InsertQuery5_2 = "Insert into iPersonalContact " +
            //                                         "( " +
            //                                             "PerConIdentityNo " +
            //                                             ",PerContactConLocId " +
            //                                             ",ContactNumber " +
            //                                         ") " +
            //                                         "VALUES " +
            //                                         "('" +
            //                                             dr1["nric"].ToString() + "' " +
            //                                             ",'Home' " +
            //                                             ",'" + dr1["homecontactno"].ToString() + "' " +
            //                                         ")";
            //                        //sbConn.GetExecuteNonQueryByCommand(InsertQuery5_2);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }


            //            #endregion

            //            #region iPersonalEmail - Cleaners don't use email
            //            //try
            //            //{
            //            //    if (!BizFunctions.IsEmpty(dr1["email"]))
            //            //    {
            //            //        string InsertQuery6 = "Insert into iPersonalEmail " +
            //            //                             "( " +
            //            //                                 "PerEmailIdentityNo " +
            //            //                                 ",PerEmailConLocId " +
            //            //                                 ",PerEmail " +
            //            //                             "VALUES " +
            //            //                             "('" +
            //            //                                 dr1["nric"].ToString() + "' " +
            //            //                                 ",'" + dr1["email"].ToString() + "' " +
            //            //                                 ",'Others' " +
            //            //                             ")";

            //            //        sbConn.GetExecuteNonQueryByCommand(InsertQuery6);
            //            //    }
            //            //}
            //            //catch (Exception ex)
            //            //{
            //            //}
            //            #endregion

            //            #region iFamily

            //            DataTable FamilyDT = GetFamilyDT(dr1["empnum"].ToString());

            //            if (FamilyDT != null)
            //            {
            //                if (FamilyDT.Rows.Count > 0)
            //                {
            //                    foreach (DataRow drFamily in FamilyDT.Rows)
            //                    {
            //                        try
            //                        {
            //                            if (BizFunctions.IsEmpty(drFamily["relationship"]))
            //                            {
            //                                drFamily["relationship"] = "None";
            //                            }
            //                            if (BizFunctions.IsEmpty(drFamily["residencetype"]))
            //                            {
            //                                drFamily["residencetype"] = "Others";
            //                            }
            //                            string InsertQuery7 = "Insert into iFamily " +
            //                                                 "( " +
            //                                                     "FamilyIdentityNo " +
            //                                                     ",FamilyMemIdentityNo " +
            //                                                     ",ResidenceTypeId " +
            //                                                     ",RelationshipId " +
            //                                                     ",PersonName " +
            //                                                     ",Gender " +
            //                                                     ",ContactNo1 " +
            //                                                 ") " +
            //                                                 "VALUES " +
            //                                                 "('" +
            //                                                 dr1["nric"].ToString() + "' " +
            //                                                 ",'" + drFamily["fnric"].ToString() + "' " +
            //                                                     ",'" + drFamily["residencetype"].ToString() + "' " +
            //                                //",'" + getSageMasterValue("FAMR", "relationship", drFamily["relationship"].ToString()) + "' " +
            //                                                     ",'" + drFamily["relationship"].ToString() + "' " +
            //                                                     ",'" + drFamily["name"].ToString() + "' " +
            //                                //",'" + getSageMasterValue("FAMR", "gender", drFamily["gender"].ToString()) + "' " +
            //                                                     ",'" + drFamily["gender"].ToString() + "' " +
            //                                                     ",'" + drFamily["contact"].ToString() + "' " +
            //                                                 ")";
            //                            //sbConn.GetExecuteNonQueryByCommand(InsertQuery7);
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            MessageBox.Show(ex.Message);
            //                        }
            //                    }
            //                }
            //            }
            //            #endregion

            //        }

            //    }
            //    #endregion

            //}
            #endregion

            InsertQueryiPersonal();
            InsertQueryiEmployee();
            InsertQueryiResidenceStatusRecord();
            InsertQueryiPersonalContact();
            InserQueryiFamily();

        }

        private void InsertQueryiPersonal()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";
            string InsertQuery1 = "";
            string  UpdateQuery1 = "";
            string SQLQuery = "";

            string str1 = "Select  * from HEMPH where [status]<>'V'    ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("HEMPH", str1);

        

            ///

            string str2 = "Select  * from iPersonal ";
            DataTable iPersonal = sbConn.GetDataTableByCommandSp("iPersonal", str2);

            DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];

            if (this.dbaccess.DataSet.Tables.Contains("iPersonal"))
            {
                this.dbaccess.DataSet.Tables["iPersonal"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iPersonal");
                this.dbaccess.DataSet.Tables.Add(iPersonal);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iPersonal);
            }

            if (DT.Rows.Count > 0)
            {
                int count = 0;
                #region Loop through HEMPH lists
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dob"]))
                    {
                        docInfo = "Insert: iPersonal - " + dr1["empnum"].ToString();
                        #region iPersonal

                        try
                        {
                            count = count + 1;
                            if (iPersonalExist(dr1["nric"].ToString()) == 0)
                            {
                                InsertQuery1 = "Insert into iPersonal " +
                                                     "( " +
                                                         "IdentityNo " +
                                                         ",NewIdentityNo " +
                                                         ",IdentityTypeId " +
                                                         ",PersonalTypeid " +
                                                         ",MaritalStatusCode " +
                                                         ",TitleId " +
                                                         ",RaceId " +
                                                         ",ReligionID " +
                                                         ",CountryOfBirth " +
                                                         ",DateOfBirth " +
                                                         ",Gender " +
                                                         ",Nationality " +
                                                         ",PersonalName " +
                                                         ",BloodGroupId " +
                                                     ") " +
                                                     "VALUES " +
                                                     "('" +
                                                         dr1["nric"].ToString() + "' " +
                                                         ",'" + dr1["nric"].ToString() + "' " +
                                                         ",'" + getSageMasterValue("HEMPH", "nrictype", dr1["nrictype"].ToString()) + "' " +
                                                         ",'Staff' " +
                                                         ",'" + getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString()) + "' " +
                                                         ",'" + GetTitleCode(Convert.ToInt32(getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString())), getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString())) + "' " +
                                                         ",'" + getSageMasterValue("HEMPH", "hramdesc", dr1["hramdesc"].ToString()) + "' " +
                                                         ",'" + getSageMasterValue("HEMPH", "religion", dr1["religion"].ToString()) + "' " +
                                                         ",'" + UppercaseFirst(dr1["country"].ToString()) + "' " +
                                                         ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dob"])) + "' " +
                                                         ",'" + getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString()) + "' " +
                                                         ",'" + getSageMasterValue("HEMPH", "nationality", GetNationality(dr1["nric"].ToString())) + "' " +
                                                         ",'" + dr1["empname"].ToString() + "' " +
                                                         ",'" + getSageMasterValue("HEMPH", "bloodtype", dr1["bloodtype"].ToString()) + "' " +
                                                     ")";

                                SQLQuery = InsertQuery1;

                                sbConn.GetExecuteNonQueryByCommand(InsertQuery1);

                                InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "I");
                            }
                            else if (iPersonalExist(dr1["nric"].ToString()) == 1)
                            {
                                if (iPersonalProccessedDateExist(dr1["nric"].ToString()))
                                {
                                    if (Convert.ToDateTime(dr1["modified"]) > iPersonalProccessedDate(dr1["nric"].ToString()))
                                    {
                                        DateTime dt1 = Convert.ToDateTime(dr1["modified"]);
                                        DateTime dt2 = iPersonalProccessedDate(dr1["nric"].ToString());
                                        UpdateQuery1 = "Update iPersonal Set " +
                                                                 " NewIdentityNo=NULL " +
                                                                 ",IdentityTypeId='" + getSageMasterValue("HEMPH", "nrictype", dr1["nrictype"].ToString()) + "' " +
                                                                 ",MaritalStatusCode='" + getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString()) + "' " +
                                                                 ",TitleId='" + GetTitleCode(Convert.ToInt32(getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString())), getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString())) + "' " +
                                                                 ",RaceId='" + getSageMasterValue("HEMPH", "hramdesc", dr1["hramdesc"].ToString()) + "' " +
                                                                 ",ReligionID='" + getSageMasterValue("HEMPH", "religion", dr1["religion"].ToString()) + "' " +
                                                                 ",CountryOfBirth='" + UppercaseFirst(dr1["country"].ToString()) + "' " +
                                                                 ",DateOfBirth='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dob"])) + "' " +
                                                                 ",Gender='" + getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString()) + "' " +
                                                                 ",Nationality='" + getSageMasterValue("HEMPH", "nationality", GetNationality(dr1["nric"].ToString())) + "' " +
                                                                 ",PersonalName='" + dr1["empname"].ToString() + "' " +
                                                                 ",BloodGroupId='" + getSageMasterValue("HEMPH", "bloodtype", dr1["bloodtype"].ToString()) + "' " +
                                                                 ",Processed=0 " +
                                                               "Where IdentityNo='" + dr1["nric"].ToString() + "' ";
                                        SQLQuery = UpdateQuery1;
                                        sbConn.GetExecuteNonQueryByCommand(UpdateQuery1);
                                        InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                    }
                                }
                            }
                            else if (iPersonalExist(dr1["nric"].ToString()) == 2)
                            {
                                if (iPersonalProccessedDateExist(dr1["nric"].ToString()))
                                {
                                   

                                    if (Convert.ToDateTime(dr1["modified"]) > iPersonalProccessedDate(dr1["nric"].ToString()))
                                    {
                                        
                                        UpdateQuery1 = "Update iPersonal Set " +
                                                                " NewIdentityNo=NULL " +
                                                                ",IdentityTypeId='" + getSageMasterValue("HEMPH", "nrictype", dr1["nrictype"].ToString()) + "' " +
                                                                ",MaritalStatusCode='" + getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString()) + "' " +
                                                                ",TitleId='" + GetTitleCode(Convert.ToInt32(getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString())), getSageMasterValue("HEMPH", "maritalstatus", dr1["maritalstatus"].ToString())) + "' " +
                                                                ",RaceId='" + getSageMasterValue("HEMPH", "hramdesc", dr1["hramdesc"].ToString()) + "' " +
                                                                ",ReligionID='" + getSageMasterValue("HEMPH", "religion", dr1["religion"].ToString()) + "' " +
                                                                ",CountryOfBirth='" + UppercaseFirst(dr1["country"].ToString()) + "' " +
                                                                ",DateOfBirth='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dob"])) + "' " +
                                                                ",Gender='" + getSageMasterValue("HEMPH", "gender", dr1["gender"].ToString()) + "' " +
                                                                ",Nationality='" + getSageMasterValue("HEMPH", "nationality", GetNationality(dr1["nric"].ToString())) + "' " +
                                                                ",PersonalName='" + dr1["empname"].ToString() + "' " +
                                                                ",BloodGroupId='" + getSageMasterValue("HEMPH", "bloodtype", dr1["bloodtype"].ToString()) + "' " +
                                                                ",Processed=0 " +                 
                                                              "Where IdentityNo='" + dr1["nric"].ToString() + "' ";
                                        SQLQuery = UpdateQuery1;
                                        sbConn.GetExecuteNonQueryByCommand(UpdateQuery1);
                                        InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                    }
                                }
                            }
                        }
                        //catch (Exception ex)
                        //{
                        //    //MessageBox.Show(ex.Message);

                        //    LogError(docInfo, ex.Message);
                        //}

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);

                            LogError(docInfo, SQLQuery +" : "+ ex.Message);
                        }

                        #endregion
                    }

                }
                #endregion

            }
        }
        ///
        private void InsertQueryiResidenceStatusRecord()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select  * from HEMPH where [status]<>'V' ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("HEMPH", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];

            string str3 = "Select  * from iResidenceStatusRecord ";
            DataTable iResidenceStatusRecord = sbConn.GetDataTableByCommandSp("iResidenceStatusRecord", str3);

            if (this.dbaccess.DataSet.Tables.Contains("iResidenceStatusRecord"))
            {
                this.dbaccess.DataSet.Tables["iResidenceStatusRecord"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iResidenceStatusRecord");
                this.dbaccess.DataSet.Tables.Add(iResidenceStatusRecord);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iResidenceStatusRecord);
            }

            if (DT.Rows.Count > 0)
            {
                #region Loop through HEMPH lists
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dob"]))
                    {
                        docInfo = "iResidenceStatusRecord - " + dr1["empnum"].ToString();
                        #region iResidenceStatusRecord
                        DataTable ResidenceDT = GetResidenceDT(dr1["nric"].ToString());

                        if (ResidenceDT != null)
                        {
                            if (ResidenceDT.Rows.Count > 0)
                            {
                                foreach (DataRow drResidence in ResidenceDT.Rows)
                                {
                                    try
                                    {
                                        if (iResidenceStatusRecordExist(dr1["nric"].ToString(), getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()), BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"]))) == 0)
                                        {

                                            //iResidenceStatusRecordExist
                                            string InsertQuery2 = "";
                                            if (!BizFunctions.IsEmpty(drResidence["effectivedate"]))
                                            {
                                                InsertQuery2 = "Insert into iResidenceStatusRecord " +
                                                                "( " +
                                                                    "ResIdentityNo " +
                                                                    ",ResStatusEffectiveDate " +
                                                                    ",ResidenceTypeId " +
                                                                    ",ResStatusRemarks " +
                                                                ") " +
                                                                "VALUES " +
                                                                "('" +
                                                                     dr1["nric"].ToString() + "' " +
                                                                     ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"])) + "' " +
                                                                     ",'" + getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()) + "' " +
                                                                     ",'" + drResidence["remark"].ToString() + "' " +
                                                                ")";

                                                sbConn.GetExecuteNonQueryByCommand(InsertQuery2);
                                                InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "I");
                                            }


                                        }
                                        else if (iResidenceStatusRecordExist(dr1["nric"].ToString(), getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()), BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"]))) == 1)
                                        {
                                            string UpdateQuery2 = "Update iResidenceStatusRecord Set " +
                                                                    "ResStatusEffectiveDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"])) + "' " +
                                                                    ",ResidenceTypeId='" + getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()) + "' " +
                                                                    ",ResStatusRemarks='" + drResidence["remark"].ToString() + "' " +
                                                                    "Where ResIdentityNo='" + dr1["nric"].ToString() + "' and ResidenceTypeId='" + getSageMasterValue("HEMP2", "nationality", drResidence["nationality"].ToString()) + "' and ResStatusEffectiveDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(drResidence["effectivedate"])) + "'  ";

                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery2);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogError(docInfo, ex.Message);
                                        MessageBox.Show(ex.Message);
                                    }

                                }

                            }
                        }
                        #endregion
                    }

                }
                #endregion

            }
        }

        private void InsertQueryiEmployee()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";
            string InsertQuery3 = "";
            string UpdateQuery3 = "";
            string SQLQuery = "";

            string str1 = "Select  * from HEMPH where [status]<>'V'      ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("HEMPH", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];

            string str4 = "Select  * from iEmployee";
            DataTable iEmployee = sbConn.GetDataTableByCommandSp("iEmployee", str4);

            if (this.dbaccess.DataSet.Tables.Contains("iEmployee"))
            {
                this.dbaccess.DataSet.Tables["iEmployee"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iEmployee");
                this.dbaccess.DataSet.Tables.Add(iEmployee);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iEmployee);
            }

            ///           

            if (DT.Rows.Count > 0)
            {
                #region Loop through HEMPH lists
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dob"]))
                    {
                        #region iEmployee
                        docInfo = "iEmployee - " + dr1["empnum"].ToString();
                        try
                        {
                            if (iEmployeeExist(dr1["empnum"].ToString()) == 0)
                            {
                                InsertQuery3 = "Insert into iEmployee " +
                                                     "( " +
                                                         "EmployeeId " +
                                                         ",NewEmployeeId " +
                                                         ",EIdentityNo " +
                                                         ",HireDate " +
                                                         ",BranchId " +
                                                         ",PositionId " +
                                                         ",CessationCode " +
                                                         ",CategoryId " +
                                                         ",DepartmentId " +
                                                         ",SectionId " +
                                                         ",CurrentBasicRateType " +
                                                         ",ClassificationCode " +
                                                         ",SalaryGradeId " +
                                                         ",CostCentreId " +
                                                         ",ProbationPeriod " +
                                                         ",ProbationUnit "+
                                                         ",CalendarId " +
                                                      ") " +
                                                     "VALUES " +
                                                     "('" +
                                                         dr1["empnum"].ToString() + "' " +
                                                         ",'" + dr1["empnum"].ToString() + "' " +
                                                         ",'" + dr1["nric"].ToString() + "' " +
                                                         ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
                                                         ",'None' " +
                                                         ",'None' " +
                                                         ",'None' " +
                                                         ",'None' " +
                                                         ",'" + dr1["sitenum"].ToString() + "' " +
                                                         ",'None' " +
                                                         ",'" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "' " +
                                                         ",'Permanent' " +
                                                         ",'" + dr1["hsgcode"].ToString() + "' " +
                                                         ",'CLN' " +
                                                         ",3 " +
                                                         ",'Mth' "+
                                                         ",'" + getSageMasterValue("HEMPH", "daysperweek", dr1["daysperweek"].ToString()) + "' " +
                                                     ")";
                                SQLQuery = InsertQuery3;
                                sbConn.GetExecuteNonQueryByCommand(InsertQuery3);
                                InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "I");
                            }
                            else if (iEmployeeExist(dr1["empnum"].ToString()) == 1)
                            {
                                if (!BizFunctions.IsEmpty(dr1["dateresigned"]))
                                {
                                    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                    {
                                        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                        {
                                            UpdateQuery3 = "Update iEmployee Set " +
                                                                     "EIdentityNo='" + dr1["nric"].ToString() + "' " +
                                                                     ",NewEmployeeId=NULL " +
                                                                     ",HireDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
                                                                     ",CessationDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateresigned"])) + "' " +
                                                                     ",BranchId='None' " +
                                                                     ",PositionId='None' " +
                                                                     ",CessationCode='None' " +
                                                                     ",CategoryId='None' " +
                                                                     ",DepartmentId='" + dr1["sitenum"].ToString() + "'  " +
                                                                     ",SectionId='None' " +
                                                                     ",CurrentBasicRateType='" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "'  " +
                                                                     ",ClassificationCode='Permanent' " +
                                                                     ",SalaryGradeId='" + dr1["hsgcode"].ToString() + "' " +
                                                                     ",CostCentreId='CLN' " +
                                                                     ",CalendarId='" + getSageMasterValue("HEMPH", "daysperweek", dr1["daysperweek"].ToString()) + "' " +
                                                                     ",Processed=0 " +
                                                                     "Where EmployeeId='" + dr1["empnum"].ToString() + "' ";
                                            SQLQuery = UpdateQuery3;
                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                }
                                else if (BizFunctions.IsEmpty(dr1["dateresigned"]))
                                {
                                    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                    {
                                        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                        {
                                            UpdateQuery3 = "Update iEmployee Set " +
                                                                     "EIdentityNo='" + dr1["nric"].ToString() + "' " +
                                                                     ",NewEmployeeId=NULL " +
                                                                     ",HireDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
                                                                     ",CessationDate=NULL " +
                                                                     ",BranchId='None' " +
                                                                     ",PositionId='None' " +
                                                                     ",CessationCode='None' " +
                                                                     ",CategoryId='None' " +
                                                                     ",DepartmentId='" + dr1["sitenum"].ToString() + "'  " +
                                                                     ",SectionId='None' " +
                                                                     ",CurrentBasicRateType='" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "'  " +
                                                                     ",ClassificationCode='Permanent' " +
                                                                     ",SalaryGradeId='" + dr1["hsgcode"].ToString() + "' " +
                                                                     ",CostCentreId='CLN' " +
                                                                     ",CalendarId='" + getSageMasterValue("HEMPH", "daysperweek", dr1["daysperweek"].ToString()) + "' " +
                                                                     ",Processed=0 " +
                                                                     "Where EmployeeId='" + dr1["empnum"].ToString() + "' ";
                                            SQLQuery = UpdateQuery3;
                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                }

                                else
                                {
                                    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                    {
                                        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                        {
                                            UpdateQuery3 = "Update iEmployee Set " +
                                                                    "EIdentityNo='" + dr1["nric"].ToString() + "' " +
                                                                    ",NewEmployeeId=NULL " +
                                                                    ",HireDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
                                                                    ",BranchId='None' " +
                                                                    ",PositionId='None' " +
                                                                    ",CessationCode='None' " +
                                                                    ",CategoryId='None' " +
                                                                    ",DepartmentId='" + dr1["sitenum"].ToString() + "'  " +
                                                                    ",SectionId='None' " +
                                                                    ",CurrentBasicRateType='" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "'  " +
                                                                    ",ClassificationCode='Permanent' " +
                                                                    ",SalaryGradeId='" + dr1["hsgcode"].ToString() + "' " +
                                                                    ",CostCentreId='CLN' " +
                                                                    ",CalendarId='" + getSageMasterValue("HEMPH", "daysperweek", dr1["daysperweek"].ToString()) + "' " +
                                                                    ",Processed=0 " +
                                                                    "Where EmployeeId='" + dr1["empnum"].ToString() + "' ";
                                            SQLQuery = UpdateQuery3;
                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                }
                            }
                            else if (iEmployeeExist(dr1["empnum"].ToString()) == 2)
                            {
                                if (!BizFunctions.IsEmpty(dr1["dateresigned"]))
                                {
                                    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                    {
                                        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                        {
                                            UpdateQuery3 = "Update iEmployee Set " +
                                                                     "EIdentityNo='" + dr1["nric"].ToString() + "' " +
                                                                     ",NewEmployeeId=NULL " +
                                                                     ",HireDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
                                                                     ",CessationDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateresigned"])) + "' " +
                                                                     ",BranchId='None' " +
                                                                     ",PositionId='None' " +
                                                                     ",CessationCode='None' " +
                                                                     ",CategoryId='None' " +
                                                                     ",DepartmentId='" + dr1["sitenum"].ToString() + "'  " +
                                                                     ",SectionId='None' " +
                                                                     ",CurrentBasicRateType='" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "'  " +
                                                                     ",ClassificationCode='Permanent' " +
                                                                     ",SalaryGradeId='" + dr1["hsgcode"].ToString() + "' " +
                                                                     ",CostCentreId='CLN' " +
                                                                     ",CalendarId='" + getSageMasterValue("HEMPH", "daysperweek", dr1["daysperweek"].ToString()) + "' " +
                                                                     ",Processed=0 " +
                                                                     "Where EmployeeId='" + dr1["empnum"].ToString() + "' ";
                                            SQLQuery = UpdateQuery3;
                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                }
                                else
                                {
                                    if (iEmployeeProccessedDateExist(dr1["empnum"].ToString()))
                                    {
                                        if (Convert.ToDateTime(dr1["modified"]) > iEmployeeProccessedDate(dr1["empnum"].ToString()))
                                        {
                                            UpdateQuery3 = "Update iEmployee Set " +
                                                                    "EIdentityNo='" + dr1["nric"].ToString() + "' " +
                                                                    ",NewEmployeeId=NULL " +
                                                                    ",HireDate='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["datejoined"])) + "' " +
                                                                    ",BranchId='None' " +
                                                                    ",PositionId='None' " +
                                                                    ",CessationCode='None' " +
                                                                    ",CategoryId='None' " +
                                                                    ",DepartmentId='" + dr1["sitenum"].ToString() + "'  " +
                                                                    ",SectionId='None' " +
                                                                    ",CurrentBasicRateType='" + getSageMasterValue("HEMPH", "paytypecode", dr1["paytypecode"].ToString()) + "'  " +
                                                                    ",ClassificationCode='Permanent' " +
                                                                    ",SalaryGradeId='" + dr1["hsgcode"].ToString() + "' " +
                                                                    ",CostCentreId='CLN' " +
                                                                    ",CalendarId='" + getSageMasterValue("HEMPH", "daysperweek", dr1["daysperweek"].ToString()) + "' " +
                                                                    ",Processed=0 " +
                                                                    "Where EmployeeId='" + dr1["empnum"].ToString() + "' ";
                                            SQLQuery = UpdateQuery3;
                                            sbConn.GetExecuteNonQueryByCommand(UpdateQuery3);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "U");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(docInfo, SQLQuery +" : " + ex.Message);
                        }
                        #endregion
                    }

                }
                #endregion

            }
        }

        private void InsertQueryiPersonalAddress()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);


            string str1 = "Select  * from HEMPH where [status]<>'V' ";
            this.dbaccess.ReadSQL("HEMPH", str1);

            string str4 = "Select  * from iPersonalAddress";
            DataTable iPersonalAddress = sbConn.GetDataTableByCommandSp("iPersonalAddress", str4);

            if (this.dbaccess.DataSet.Tables.Contains("iPersonalAddress"))
            {
                this.dbaccess.DataSet.Tables["iPersonalAddress"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iPersonalAddress");
                this.dbaccess.DataSet.Tables.Add(iPersonalAddress);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iPersonalAddress);
            }

            ///    
            DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];

            if (DT.Rows.Count > 0)
            {
                #region Loop through HEMPH lists
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dob"]))
                    {
                        #region iPersonalAddress

                        try
                        {
                            string InsertQuery4 = "Insert into iPersonalAddress " +
                                                 "( " +
                                                     "PerAddIdentityNo " +
                                                     ",PerAddAddress1 " +
                                                     ",PerAddAddress2 " +
                                                     ",PerAddAddress3 " +
                                                     ",PerAddCountry " +
                                                     ",PerAddConLocId " +
                                                 ") " +
                                                 "VALUES " +
                                                 "('" +
                                                       dr1["nric"].ToString() + "' " +
                                                       ",'" + dr1["cadd1"].ToString() + "' " +
                                                       ",'" + dr1["cadd2"].ToString() + "' " +
                                                       ",'" + dr1["cadd3"].ToString() + "' " +
                                                       ",'" + UppercaseFirst(dr1["country"].ToString()) + "' " +
                                                        ",'Others' " +
                                                   ")";
                            //sbConn.GetExecuteNonQueryByCommand(InsertQuery4);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        #endregion
                    }

                }
                #endregion
            }
        }
        //
        private void InsertQueryiPersonalContact()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select  * from HEMPH where [status]<>'V'   ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("HEMPH", str1);

            ///    
            DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];


            string str4 = "Select  * from iPersonalContact";
            DataTable iPersonalContact = sbConn.GetDataTableByCommandSp("iPersonalContact", str4);

            if (this.dbaccess.DataSet.Tables.Contains("iPersonalContact"))
            {
                this.dbaccess.DataSet.Tables["iPersonalContact"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iPersonalContact");
                this.dbaccess.DataSet.Tables.Add(iPersonalContact);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iPersonalContact);
            }

            if (DT.Rows.Count > 0)
            {
                #region Loop through HEMPH lists
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dob"]))
                    {
                        docInfo = "iPersonalContact - " + dr1["empnum"].ToString();
                        #region iPersonalContact
                        try
                        {
                            string InsertQuery5_1 = "";
                            string InsertQuery5_2 = "";
                            if (!BizFunctions.IsEmpty(dr1["contact"]))
                            {
                                if (iPersonalContactExist(dr1["nric"].ToString(), "Handphone", dr1["contact"].ToString()) == 0)
                                {
                                    if (dr1["contact"].ToString().Trim() != "-")
                                    {
                                        InsertQuery5_1 = "Insert into iPersonalContact " +
                                                             "( " +
                                                                 "PerConIdentityNo " +
                                                                 ",PerContactConLocId " +
                                                                 ",ContactNumber " +
                                                             ") " +
                                                             "VALUES " +
                                                             "('" +
                                                                 dr1["nric"].ToString() + "' " +
                                                                 ",'Handphone' " +
                                                                 ",'" + dr1["contact"].ToString() + "' " +
                                                             ")";
                                        sbConn.GetExecuteNonQueryByCommand(InsertQuery5_1);
                                    }
                                }
                              
                            }
                            if (!BizFunctions.IsEmpty(dr1["homecontactno"]))
                            {
                                if (iPersonalContactExist(dr1["nric"].ToString(), "Home", dr1["contact"].ToString()) == 0)
                                {
                                    if (dr1["homecontactno"].ToString().Trim() != "-")
                                    {
                                        InsertQuery5_2 = "Insert into iPersonalContact " +
                                                         "( " +
                                                             "PerConIdentityNo " +
                                                             ",PerContactConLocId " +
                                                             ",ContactNumber " +
                                                         ") " +
                                                         "VALUES " +
                                                         "('" +
                                                             dr1["nric"].ToString() + "' " +
                                                             ",'Home' " +
                                                             ",'" + dr1["homecontactno"].ToString() + "' " +
                                                         ")";
                                        //sbConn.GetExecuteNonQueryByCommand(InsertQuery5_2);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                        #endregion
                    }

                }
                #endregion

            }
        }
       
    
        private void InsertQueryiAllowanceRecord()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "select * from atr1 where ISNULL(actualFixAllow,0)>0 and [status]<>'V'";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }

            this.dbaccess.ReadSQL("ATR1_ALLOW", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["ATR1_ALLOW"];


            string str4 = "Select  * from iAllowanceRecord";
            DataTable iAllowanceRecord = sbConn.GetDataTableByCommandSp("iAllowanceRecord", str4);

            if (this.dbaccess.DataSet.Tables.Contains("iAllowanceRecord"))
            {
                this.dbaccess.DataSet.Tables["iAllowanceRecord"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iAllowanceRecord");
                this.dbaccess.DataSet.Tables.Add(iAllowanceRecord);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iAllowanceRecord);
            }

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["trandate"]))
                    {
                        docInfo = "iAllowanceRecord - " + dr1["refnum"].ToString() + " - " + dr1["empnum"].ToString();
                        try
                        {
                            //string InsertQuery = "Insert into iAllowanceRecord (AllowanceEmployeeID,AllowanceID,AllowanceDate,PayRecID,AllowanceAmount,AllowanceRemarks,AllowanceDeclaredDate) " +
                            //                 "VALUES " +
                            //                 "('" + dr1["empnum"].ToString() + "','AWS','" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["trandate"].ToString())) + "','Normal'," + dr1["FixAllowAmt"].ToString() + ",'" + dr1["remark"].ToString() + "','" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["trandate"].ToString())) + "')";
                            if (iAllowanceRecordExists(dr1["empnum"].ToString().Trim(), "ADHOC ALLOW", BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["trandate"].ToString()))) == 0)
                            {
                                string InsertQuery = "Insert into iAllowanceRecord " +
                                                        "( " +
                                                            "AllowanceEmployeeID " +
                                                            ",AllowanceID " +
                                                            ",AllowanceDate " +
                                                            ",PayRecID " +
                                                            ",AllowanceAmount " +
                                                            ",AllowanceRemarks " +
                                                            ",AllowanceDeclaredDate " +
                                                         ") " +
                                                     "VALUES " +
                                                     "('" +
                                                         dr1["empnum"].ToString() + "' " +
                                                         ",'ADHOC ALLOW' " +
                                                         ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["trandate"].ToString())) + "' " +
                                                         ",'Normal' " +
                                                         "," + dr1["FixAllowAmt"].ToString() + " " +
                                                         ",'" + dr1["remark"].ToString() + "' " +
                                                         ",'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["trandate"].ToString())) + "' " +
                                                     ")";


                                sbConn.GetExecuteNonQueryByCommand(InsertQuery);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(),this.dbaccess), "I");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(docInfo, ex.Message);
                        }



                    }
                }
            }
        }


        private void InsertQueryiEmployeeRecurAllowance()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select * from pfmsr where rateamt>0 and hsamcode not in ('BASIC','DAILYRATED') and [status]<>'V' and hsamcode in "+
                            "(Select ERPvalue as hsamcode from SageColumnMasterLink where TableName='PFMSR' and erpcolumn='hsamcode' and SageValue is not null) ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }

            this.dbaccess.ReadSQL("PFMSR_ALLOW", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["PFMSR_ALLOW"];


            string str4 = "Select  * from iEmployeeRecurAllowance";
            DataTable iEmployeeRecurAllowance = sbConn.GetDataTableByCommandSp("iEmployeeRecurAllowance", str4);

            if (this.dbaccess.DataSet.Tables.Contains("iEmployeeRecurAllowance"))
            {
                this.dbaccess.DataSet.Tables["iEmployeeRecurAllowance"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iEmployeeRecurAllowance");
                this.dbaccess.DataSet.Tables.Add(iEmployeeRecurAllowance);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iEmployeeRecurAllowance);
            }

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dateadjusted"]))
                    {
                        docInfo = "iEmployeeRecurAllowance - " + dr1["salcode"].ToString() + " - " + dr1["empnum"].ToString();
                        try
                        {        
                            //'" + getSageMasterValue("HEMPH", "daysperweek", dr1["daysperweek"].ToString()) + "' " +
                            if (iEmployeeRecurAllowanceExists(dr1["empnum"].ToString().Trim(), getSageMasterValue("PFMSR", "hsamcode", dr1["hsamcode"].ToString()), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["dateadjusted"].ToString()))) == 0)
                            {
                                string InsertQuery = "Insert into iEmployeeRecurAllowance " +
                                                        "( " +
                                                            "RecurEmployeeID " +
                                                            ",RecurFormulaId " +
                                                            ",RecurAlloFullAmount " + // put as 0
                                                            ",RecurAlloSubPeriod " +  // put as 1 - How many times a month
                                                            ",RecurAlloStartPeriod " +  //Starting Month of the allowance
                                                            ",RecurAlloEndPeriod " + 
                                                            ",RecurAlloAmount " +
                                                            ",RecurAlloEndCounter " +
                                                            ",RecurAlloStartYear " +   
                                                            ",RecurAlloEndYear "+
                                                            ",RecurAlloPayRecId " +
                                                            ",RecurAlloDesc " +
                                                            ",CreatedBy " +
                                                         ") " +
                                                     "VALUES " +
                                                     "('" +
                                                         dr1["empnum"].ToString() + "' " +
                                                         ",'" + getSageMasterValue("PFMSR", "hsamcode", dr1["hsamcode"].ToString()) + "'" +  
                                                         ",0 "+
                                                         ",1 "+
                                                         ",1 " +
                                                         ",12 " +
                                                        "," + dr1["rateamt"].ToString() + " " +
                                                         ",0 " +
                                                         "," + Convert.ToDateTime(dr1["dateadjusted"].ToString()).Year.ToString() + " " +
                                                         "," + Convert.ToDateTime(dr1["dateadjusted"].ToString()).AddYears(3).Year.ToString() + " " +  
                                                         ",'Normal' "+
                                                         ", '" + dr1["salcode"].ToString() + " : " + dr1["docunum"].ToString() + "'" +
                                                         ",'" + Common.DEFAULT_SYSTEM_USERNAME + "' " +
                                                     ")";


                                sbConn.GetExecuteNonQueryByCommand(InsertQuery);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(), this.dbaccess), "I");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(docInfo, ex.Message);
                        }



                    }
                }
            }
        }




        #region InsertQueryiLeave Record: 28-12-2015 @ Getting data from the wrong location
        //private void InserQueryiLeaveRecord()
        //{
        //    Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
        //    Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
        //    string docInfo = "";

        //    string str1 = "Select * from lvr where [status]<>'V'";
        //    if (whereClause != string.Empty)
        //    {
        //        str1 = str1 + " AND " + whereClause;
        //    }
        //    this.dbaccess.ReadSQL("LVR",str1);

        //    DataTable DT = this.dbaccess.DataSet.Tables["LVR"];

        //    string str2 = "Select * from iLeaveRecord";

        //    DataTable iLeaveRecord = sbConn.GetDataTableByCommandSp("iLeaveRecord", str2);
        //    if (this.dbaccess.DataSet.Tables.Contains("iLeaveRecord"))
        //    {
        //        this.dbaccess.DataSet.Tables["iLeaveRecord"].Dispose();
        //        this.dbaccess.DataSet.Tables.Remove("iLeaveRecord");
        //        this.dbaccess.DataSet.Tables.Add(iLeaveRecord);
        //    }
        //    else
        //    {
        //        this.dbaccess.DataSet.Tables.Add(iLeaveRecord);
        //    }

        //    if(DT.Rows.Count > 0)
        //    {
        //        foreach(DataRow dr1 in DT.Rows)
        //        {
        //            if (!BizFunctions.IsEmpty(dr1["trandate"]))
        //            {
        //                docInfo = "iLeaveRecord - " + dr1["refnum"].ToString() + " - "+dr1["empnum"].ToString();
        //                try
        //                {
        //                    if (iLeaveRecordExists(dr1["empnum"].ToString(), getSageMasterValue("LVR", "lvmnum", dr1["lvmnum"].ToString()).Trim(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["leavefrom"].ToString()))) == 0)
        //                    {
        //                        string InsertQuery = "Insert into iLeaveRecord (LveEmployeeID,LveID,LeaveDate,CurrentLveDays) " +
        //                                         "VALUES " +
        //                                         "('" + dr1["empnum"].ToString() + "', '" + getSageMasterValue("LVR", "lvmnum", dr1["lvmnum"].ToString()).Trim() + "' ,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["leavefrom"].ToString())) + "'," + dr1["totaldays"].ToString() + ")";

        //                        sbConn.GetExecuteNonQueryByCommand(InsertQuery);
        //                        InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(),this.dbaccess), "I");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogError(docInfo, ex.Message);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        private void InserQueryiLeaveRecord()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select * from atr1 where [status]<>'V' and ISNULL(isomit,0)=0 and shiftcode in (Select SHIFTCODE from vSHLV where isWorkShift=0 and SHIFTCODE<>'V' ) and shiftcode in "+
                            "(  "+
                                "select ERPValue as shiftcode from SageColumnMasterLink where tablename='LVR'  " +
                            ")";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("ATR_LVR", str1);

            DataTable DT = this.dbaccess.DataSet.Tables["ATR_LVR"];

            string str2 = "Select * from iLeaveRecord";

            DataTable iLeaveRecord = sbConn.GetDataTableByCommandSp("iLeaveRecord", str2);
            if (this.dbaccess.DataSet.Tables.Contains("iLeaveRecord"))
            {
                this.dbaccess.DataSet.Tables["iLeaveRecord"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iLeaveRecord");
                this.dbaccess.DataSet.Tables.Add(iLeaveRecord);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iLeaveRecord);
            }

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["trandate"]))
                    {
                        docInfo = "iLeaveRecord - " + dr1["refnum"].ToString() + " - " + dr1["empnum"].ToString();
                        try
                        {
                            if (iLeaveRecordExists(dr1["empnum"].ToString(), getSageMasterValue("LVR", "lvmnum", dr1["shiftcode"].ToString()).Trim(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString()))) == 0)
                            {
                                string InsertQuery = "Insert into iLeaveRecord (LveEmployeeID,LveID,LeaveDate,CurrentLveDays) " +
                                                 "VALUES " +
                                                 "('" + dr1["empnum"].ToString() + "', '" + getSageMasterValue("LVR", "lvmnum", dr1["shiftcode"].ToString()).Trim() + "' ,'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())) + "',1)";

                                sbConn.GetExecuteNonQueryByCommand(InsertQuery);
                                InsertRowStatus(BizLogicTools.Tools.GetNRIC(dr1["empnum"].ToString()), dr1["empnum"].ToString(), BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(), this.dbaccess), "I");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(docInfo, ex.Message);
                        }
                    }
                }
            }
        }

        private void InserQueryiFamily()
        {
            Sqlhelper1 dfConn = new Sqlhelper1(DefaultConnectionString);
            Sqlhelper2 sbConn = new Sqlhelper2(SyBaseConnectionString);
            string docInfo = "";

            string str1 = "Select  * from HEMPH where [status]<>'V'   ";
            if (whereClause != string.Empty)
            {
                str1 = str1 + " AND " + whereClause;
            }
            this.dbaccess.ReadSQL("HEMPH", str1);

            ///    
            DataTable DT = this.dbaccess.DataSet.Tables["HEMPH"];

            string str2 = "Select  * from iFamily ";
            DataTable iFamily = sbConn.GetDataTableByCommandSp("iFamily", str2);

            if (this.dbaccess.DataSet.Tables.Contains("iFamily"))
            {
                this.dbaccess.DataSet.Tables["iFamily"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("iFamily");
                this.dbaccess.DataSet.Tables.Add(iFamily);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(iFamily);
            }

            if (DT.Rows.Count > 0)
            {
                #region Loop through HEMPH lists
                foreach (DataRow dr1 in DT.Rows)
                {
                    if (!BizFunctions.IsEmpty(dr1["dob"]))
                    {
                        docInfo = "iFamily - " + dr1["empnum"].ToString();
                        #region iFamily

                        DataTable FamilyDT = GetFamilyDT(dr1["empnum"].ToString());

                        if (FamilyDT != null)
                        {
                            if (FamilyDT.Rows.Count > 0)
                            {
                                foreach (DataRow drFamily in FamilyDT.Rows)
                                {
                                    try
                                    {
                                        if (BizFunctions.IsEmpty(drFamily["relationship"]))
                                        {
                                            drFamily["relationship"] = "None";
                                        }
                                        if (BizFunctions.IsEmpty(drFamily["residencetype"]))
                                        {
                                            drFamily["residencetype"] = "Others";
                                        }

                                        if (iFamilyExist(drFamily["fnric"].ToString()) == 0)
                                        {
                                            string InsertQuery7 = "Insert into iFamily " +
                                                                 "( " +
                                                                     "FamilyIdentityNo " +
                                                                     ",FamilyMemIdentityNo " +
                                                                     ",ResidenceTypeId " +
                                                                     ",RelationshipId " +
                                                                     ",PersonName " +
                                                                     ",Gender " +
                                                                     ",ContactNo1 " +
                                                                 ") " +
                                                                 "VALUES " +
                                                                 "('" +
                                                                 dr1["nric"].ToString() + "' " +
                                                                 ",'" + drFamily["fnric"].ToString() + "' " +
                                                                     ",'" + drFamily["residencetype"].ToString() + "' " +
                                                //",'" + getSageMasterValue("FAMR", "relationship", drFamily["relationship"].ToString()) + "' " +
                                                                     ",'" + drFamily["relationship"].ToString() + "' " +
                                                                     ",'" + drFamily["name"].ToString() + "' " +
                                                //",'" + getSageMasterValue("FAMR", "gender", drFamily["gender"].ToString()) + "' " +
                                                                     ",'" + drFamily["gender"].ToString() + "' " +
                                                                     ",'" + drFamily["contact"].ToString() + "' " +
                                                                 ")";
                                            sbConn.GetExecuteNonQueryByCommand(InsertQuery7);
                                            InsertRowStatus(dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), "I");
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                }
                #endregion

            }
        }


        
        #endregion


        #region Exist Checkers
        private int iPersonalExist(string nric)
        {
            int status = 0;

            string str1 = "Select * from iPersonal where IdentityNo='" + nric + "'";



            //this.dbaccess.ReadSQL("TmpIpersonal", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        // Processed
                        status = 2;
                    }
                    else
                    {
                        // Not yet Processed
                        status = 1;
                    }
                }
            }

            return status;
        }

        private DateTime iPersonalProccessedDate(string nric)
        {
            DateTime dt = new DateTime();

            string str1 = "Select * from iPersonal where IdentityNo='" + nric + "'";



            //this.dbaccess.ReadSQL("TmpIpersonal", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {

                    dt = Convert.ToDateTime(dt1.Rows[0]["ProcessedDateTime"]);
                    //if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    //{
                    //    dt1.Rows[0]["processed"] = 0;
                    //}
                    //if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    //{
                    //    // Processed
                    //    status = 2;
                    //}
                    //else
                    //{
                    //    // Not yet Processed
                    //    status = 1;
                    //}
                }
            }

            return dt;
        }

        private bool iPersonalProccessedDateExist(string nric)
        {
            DateTime dt = new DateTime();
            bool Exists = false;
            string str1 = "Select * from iPersonal where IdentityNo='" + nric + "'";



            //this.dbaccess.ReadSQL("TmpIpersonal", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (!BizFunctions.IsEmpty(dt1.Rows[0]["ProcessedDateTime"]))
                    {
                        Exists = true;
                    }
                    else
                    {
                        Exists = false;
                    }    
                }
            }

            return Exists;
        }

        private bool iEmployeeProccessedDateExist(string empnum)
        {
            DateTime dt = new DateTime();
            bool Exists = false;
            string str1 = "Select * from iEmployee where EmployeeId='" + empnum + "'";



            //this.dbaccess.ReadSQL("TmpIpersonal", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (!BizFunctions.IsEmpty(dt1.Rows[0]["ProcessedDateTime"]))
                    {
                        Exists = true;
                    }
                    else
                    {
                        Exists = false;
                    }
                }
            }

            return Exists;
        }



        //private bool iPersonalIsUpdated(string nric, DateTime modified)
        //{
        //    bool isUpdated = false;

        //    string str1 = "Select * from iPersonal where IdentityNo='" + nric + "'";

        //    DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

        //    if (dt1 != null)
        //    {
        //        if (dt1.Rows.Count > 0)
        //        {
        //            if (!BizFunctions.IsEmpty(dt1.Rows[0]["ProcessedDateTime"]))
        //            {
        //                dt = Convert.ToDateTime(dt1.Rows[0]["ProcessedDateTime"]);

        //                if (modified > dt1)
        //                {
        //                    isUpdated = false;
        //                }
        //                else
        //                {
        //                    isUpdated = true;
        //                }
        //            }
                                   
        //        }
        //    }

        //    return isUpdated;
        //}

        private DateTime iEmployeeProccessedDate(string empnum)
        {
            DateTime dt = new DateTime();

            string str1 = "Select * from iEmployee where EmployeeId='" + empnum + "'";



            //this.dbaccess.ReadSQL("TmpIpersonal", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {

                    dt = Convert.ToDateTime(dt1.Rows[0]["ProcessedDateTime"]);
                    //if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    //{
                    //    dt1.Rows[0]["processed"] = 0;
                    //}
                    //if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    //{
                    //    // Processed
                    //    status = 2;
                    //}
                    //else
                    //{
                    //    // Not yet Processed
                    //    status = 1;
                    //}
                }
            }

            return dt;
        }

        //private bool iEmployeeIsUpdated(string empnum, DateTime modified)
        //{
        //    bool isUpdated = false;

        //    string str1 = "Select * from iEmployee where EmployeeId='" + empnum + "'";

        //    DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

        //    if (dt1 != null)
        //    {
        //        if (dt1.Rows.Count > 0)
        //        {
        //            if (!BizFunctions.IsEmpty(dt1.Rows[0]["ProcessedDateTime"]))
        //            {
        //                dt = Convert.ToDateTime(dt1.Rows[0]["ProcessedDateTime"]);

        //                if (modified > dt1)
        //                {
        //                    isUpdated = false;
        //                }
        //                else
        //                {
        //                    isUpdated = true;
        //                }
        //            }

        //        }
        //    }

        //    return isUpdated;
        //}


        private int iResidenceStatusRecordExist(string key, string ResidenceTypeId, string effectiveDate)
        {
            int status = 0;


            string str1 = "select * from iResidenceStatusRecord where ResIdentityNo='" + key + "' and ResidenceTypeId='" + ResidenceTypeId + "' and ResStatusEffectiveDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(effectiveDate) + "'  ";


            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iPersonalContactExist(string key, string PerContactConLocId, string ContactNumber)
        {
            int status = 0;


            string str1 = "select * from iPersonalContact where PerConIdentityNo='" + key + "' and PerContactConLocId='" + PerContactConLocId + "' and ContactNumber='" + ContactNumber + "'  ";


            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iEmployeeExist(string key)
        {
            int status = 0;

            string str1 = "Select * from iEmployee where EmployeeID='" + key + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }


        private int iPaymentBankInfoExist(string key)
        {
            int status = 0;

            string str1 = "Select * from iPaymentBankInfo where PayBankEmployeeId='" + key + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iEmployeeIsResigned(string key)
        {
            int status = 0;

            string str1 = "Select * from iEmployee where EmployeeId='" + key + "' and cessationDate<>NULL and processed=1";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iPersonalAddressExist(string key)
        {
            int status = 0;

            //string str1 = "Select * from iPersonal where IdentityNo='" + nric + "'";



            ////this.dbaccess.ReadSQL("TmpIpersonal", str1);

            //DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            //if (dt1 != null)
            //{
            //    if (dt1.Rows.Count > 0)
            //    {
            //        if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
            //        {
            //            dt1.Rows[0]["processed"] = 0;
            //        }
            //        if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
            //        {
            //            status = 2;
            //        }
            //        else
            //        {
            //            status = 1;
            //        }
            //    }
            //}

            return status;
        }

        private int iBasicRateProgressionExist(string key, string Date, string BRProgCareerId)
        {
            int status = 0;

            string str1 = "Select * from iBasicRateProgression where BREmployeeId='" + key + "' and BRProgEffectiveDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(Date).ToShortDateString() + "' and BRProgCareerId='" + BRProgCareerId + "' ";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iCareerProgressionExist(string key, string Date, string CareerCareerId)
        {
            int status = 0;

            string str1 = "Select * from iCareerProgression where CareerEmployeeId='" + key + "' and CareerEffectiveDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(Date).ToShortDateString() + "' and CareerCareerId='" + CareerCareerId + "' ";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iPersonalContactExist(string key)
        {
            int status = 0;

            //string str1 = "Select * from iPersonal where IdentityNo='" + nric + "'";



            ////this.dbaccess.ReadSQL("TmpIpersonal", str1);

            //DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            //if (dt1 != null)
            //{
            //    if (dt1.Rows.Count > 0)
            //    {
            //        if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
            //        {
            //            dt1.Rows[0]["processed"] = 0;
            //        }
            //        if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
            //        {
            //            status = 2;
            //        }
            //        else
            //        {
            //            status = 1;
            //        }
            //    }
            //}

            return status;
        }

        private int iFamilyExist(string key)
        {
            int status = 0;

            string str1 = "Select * from iFamily where FamilyIdentityNo='" + key + "'";



            //this.dbaccess.ReadSQL("TmpiFamily", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iDetailRecordExists(string key,string Date)
        {
            int status = 0;

            string str1 = "Select * from iDetailRecord where DetailEmployeeID='" + key + "' and DetailDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(Date).ToShortDateString() + "' ";

            //this.dbaccess.ReadSQL("TmpiDetailRecord", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iLeaveRecordExists(string key,string LveID, string Date)
        {
            int status = 0;
            //LveEmployeeID,LveID,LeaveDat
            string str1 = "Select * from iLeaveRecord where LveEmployeeID='" + key + "' and LeaveDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(Date).ToShortDateString() + "' and LveID='" + LveID + "' ";

            //this.dbaccess.ReadSQL("TmpiLeaveRecord", str1);

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iOTRecordExists(string key, string Date, string OTID)
        {
            int status = 0;


            //InsertQuery = "Insert into iOTRecord (OTEmployeeID,OTID,OTDate,PayRecID,CurrentOTFreq) " +
            //             "VALUES " +
            //             "('" + dr1["empnum"].ToString() + "','" + getSageMasterValue("ATR1", "OT", "OT1") + "','" + BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["date"].ToString())) + "','Normal'," + dr1["ActualOTHrs"].ToString() + ") ";


            string str1 = "Select * from iOTRecord where OTEmployeeID='" + key + "' and OTDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(Date).ToShortDateString() + "' and OTID='" + OTID + "' ";

         

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iAllowanceRecordExists(string key, string AllowanceID, string Date)
        {
            int status = 0;


            string str1 = "Select * from iAllowanceRecord where RecurEmployeeID='" + key + "' and AllowanceDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(Date).ToShortDateString() + "' ";

       

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }

        private int iEmployeeRecurAllowanceExists(string key,string AllowanceID, string Date)
        {
            int status = 0;

            //string str1 = "Select * from iEmployeeRecurAllowance where AllowanceEmployeeID='" + key + "' and AllowanceDate='" + ATL.TimeUtilites.TimeTools.GetSafeDate(Date).ToShortDateString() + "' and RecurFormulaId='" + AllowanceID + "' ";
            string str1 = "Select * from iEmployeeRecurAllowance where AllowanceEmployeeID='" + key + "' and RecurFormulaId='" + AllowanceID + "' ";
       
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt1.Rows[0]["processed"]))
                    {
                        dt1.Rows[0]["processed"] = 0;
                    }
                    if (Convert.ToInt32(dt1.Rows[0]["processed"]) == 1)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }

            return status;
        }
        

        #endregion

        #region Misc Methods
        #region Update Query
        private void UpdateQuery()
        {
        }
        #endregion

        #region Delete Query
        private void DeleteQuery()
        {
        }
        #endregion

        #region Get String Between
        static string GetStringBetween(string source, string left, string right)
        {
            return Regex.Match(source, string.Format("{0}(.*){1}", left, right)).Groups[1].Value;
        }
        #endregion

        #region Radio Button Events

        private void insertRb_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void updateRb_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void deleteRb_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void selectRb_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

       
        private string getSageMasterValue(string TableName, string ErpcolumnName, string ErpcolumnValue)
        {
            string value = "";

            string getValue = "Select * from SageColumnMasterLink where TableName='" + TableName + "' and erpcolumn='" + ErpcolumnName + "' and erpvalue='" + ErpcolumnValue.Trim() + "'  ";

            this.dbaccess.ReadSQL("TempSageColumnValue", getValue);

            DataTable DT = this.dbaccess.DataSet.Tables["TempSageColumnValue"];

            if (DT != null)
            {
                if (DT.Rows.Count > 0)
                {
                    value = DT.Rows[0]["sagevalue"].ToString();
                }
            }

            return value;
        }

        private void Interface_Load(object sender, EventArgs e)
        {
            FromEmpnumTb.KeyDown += new KeyEventHandler(FromEmpnumTb_KeyDown);
            FromEmpnumTb.DoubleClick += new EventHandler(FromEmpnumTb_DoubleClick);
            ToEmpnumTb.KeyDown += new KeyEventHandler(ToEmpnumTb_KeyDown);
            ToEmpnumTb.DoubleClick += new EventHandler(ToEmpnumTb_DoubleClick);

            dt1 = new DataTable("dt1");

            dt1.Columns.Add("nric",typeof(string));
            dt1.Columns.Add("empnum", typeof(string));
            dt1.Columns.Add("empname", typeof(string));
            dt1.Columns.Add("status", typeof(string));

            BindingSource nbindingSource = new BindingSource();


            dt1.ColumnChanged += new DataColumnChangeEventHandler(dt1_ColumnChanged);

            nbindingSource.DataSource = dt1;

            dgv1.DataSource = nbindingSource;

        }

        void dt1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
          
        }

        void ToEmpnumTb_DoubleClick(object sender, EventArgs e)
        {
            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + ToEmpnumTb.Text + "%'  ", null, F2Type.Sort);



            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {

                ToEmpnumTb.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();

            }
        }

        void ToEmpnumTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + ToEmpnumTb.Text + "%'  ", null, F2Type.Sort);



                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {

                    ToEmpnumTb.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();                   

                }

            }
        }

        void FromEmpnumTb_DoubleClick(object sender, EventArgs e)
        {
            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + FromEmpnumTb.Text + "%'  ", null, F2Type.Sort);



            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {

                FromEmpnumTb.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();

            }
        }

        void FromEmpnumTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + FromEmpnumTb.Text + "%'  ", null, F2Type.Sort);



                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {

                    FromEmpnumTb.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();

                }

            }
        }

        private string GetTitleCode(int gender, string maritalStatus)
        {
            string titleCode = "";

            if (gender == 1)
            {
                titleCode = "Mr";
            }
            else if (gender == 0)
            {
                if (maritalStatus.Trim() == "Married")
                {
                    titleCode = "Mrs";
                }
                else
                {
                    titleCode = "Ms";
                }
            }

            return titleCode;
        }

        private DataTable GetResidenceDT(string nric)
        {
            DataTable dt;

            string str1 = "Select * from HEMP2 where empnum=[dbo].GetEmpnumNRIC('" + nric + "') and ISNULL(isLatest,0)=1 and nationality like 'SINGA%'";

            this.dbaccess.ReadSQL("ResidenceDT", str1);

            dt = this.dbaccess.DataSet.Tables["ResidenceDT"];


            return dt;
        }

        private DataTable GetFamilyDT(string empnum)
        {
            DataTable dt;

            string str1 = "Select * from FAMR where empnum='"+empnum+"'";

            this.dbaccess.ReadSQL("FamilyDT", str1);

            dt = this.dbaccess.DataSet.Tables["FamilyDT"];


            return dt;
        }

        private string GetNationality(string nric)
        {
            string Nationality = ""; ;
            DataTable dt;

            string str1 = "Select Top 1 * from HEMP2 where empnum=[dbo].GetEmpnumNRIC('" + nric + "') and ISNULL(isMainNationality,0)=1 ";

            this.dbaccess.ReadSQL("NationalityDT", str1);

            dt = this.dbaccess.DataSet.Tables["NationalityDT"];

            if(dt != null)
            {
                if(dt.Rows.Count > 0)
                {
                    Nationality = dt.Rows[0]["nationality"].ToString();
                }
            }

            return Nationality;
        }

        private string UppercaseFirst(string s)
        {
            s = s.ToLower();
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        private decimal GetBasicSalary(string empnum)
        {
            decimal basicSal = 0;

            string str1 = "SELECT * FROM "+
                            "( "+
                            "SELECT "+
	                            "ROW_NUMBER() OVER (Order BY dateadjusted) as ForTop,ROW_NUMBER() OVER (Order BY dateadjusted Desc) as ForBottom,empnum,rateamt as basicsal,dateadjusted "+
                            "from SALH where hsamcode='BASIC' and empnum='"+empnum+"' "+
                            ")A WHERE ForBottom=1";

            this.dbaccess.ReadSQL("TmpSalhEmp",str1);

            DataTable Dt1 = this.dbaccess.DataSet.Tables["TmpSalhEmp"];

            if (Dt1 != null)
            {
                if (Dt1.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(Dt1.Rows[0]["basicsal"]))
                    {
                        Dt1.Rows[0]["basicsal"] = 0;
                    }
                    basicSal = Convert.ToDecimal(Dt1.Rows[0]["basicsal"]);
                }
            }

            return basicSal;
        }

        private int isCurrentProgression(string empnum,string salcode)
        {
            int current = 0;

            string str1 = "SELECT * FROM " +
                            "( " +
                            "SELECT " +
                                "ROW_NUMBER() OVER (Order BY dateadjusted) as ForTop,ROW_NUMBER() OVER (Order BY dateadjusted Desc) as ForBottom,empnum,salcode,dateadjusted " +
                            "from SALH where empnum='" + empnum + "' " +
                            "group by empnum,salcode,dateadjusted "+
                            ")A  WHERE salcode='"+salcode+"'";

            this.dbaccess.ReadSQL("TmpSalhProgEmp", str1);

            DataTable Dt1 = this.dbaccess.DataSet.Tables["TmpSalhProgEmp"];

            if (Dt1 != null)
            {
                if (Dt1.Rows.Count > 0)
                {

                    if (!BizFunctions.IsEmpty(Dt1.Rows[0]["ForBottom"]))
                    {
                        if (Convert.ToInt32(Dt1.Rows[0]["ForBottom"]) == 1)
                        {
                            current = 1;
                        }
                        else
                        {
                            current = 0;
                        }
                    }
                }
            }

            return current;
        }

        #endregion

        private void LogError(string docinfo, string logmessage)
        {
            Parameter[] parameters = new Parameter[3];
            parameters[0] = new Parameter("@docinfo", @docinfo);
            parameters[1] = new Parameter("@logmessage", @logmessage);
            parameters[2] = new Parameter("@user", Common.DEFAULT_SYSTEM_USERNAME);
    

            try
            {
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_Insert_SageMigrateErrorLog", ref parameters);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertRowStatus(string nric,string empnum,string empname,string status)
        {
            DataRow dr1 = dt1.NewRow();

            dr1["nric"] = nric;
            dr1["empnum"] = empnum;
            dr1["empname"] = empname;
            dr1["status"] = status;

            dt1.Rows.Add(dr1);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
            }
        }



    }
}