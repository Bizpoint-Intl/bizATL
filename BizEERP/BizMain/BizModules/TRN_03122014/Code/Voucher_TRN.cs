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
using ATL.TimeUtilites;
using ATL.BizModules.UserAuthorization;
#endregion

namespace ATL.TRN
{
    public class Voucher_TRN : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables

        UserAuthorization sa = null;
        protected DBAccess dbaccess = null;
        protected Hashtable selectsCollection = null;
        protected DataGrid datagrid1;
        protected TextBox txt_appraisalscore, txt_appraisaloverscore,txt_empname,
                          txt_currentsal, txt_reasonforlastincre, txt_newsal, TRNH_empnum, TRNH_appraisedcomments, TRNH_approvedcomments= null;
        protected GroupBox grb_TRNHapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        protected ComboBox cb, TRNH_hsgcode, TRNH_TRNmcode = null;
        protected string headerFormName,qaFormName, RecommendedBy, ApprovedBy = null;

        protected int TRN1RowNum = -1;
        bool FromPFMSR = false;
        protected bool opened = false;

        protected Button btnSave,btnConfirm = null;

        #endregion

        #region Construct

        public Voucher_TRN(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_TRN.xml", moduleName, voucherBaseHelpers)
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

            e.Condition = " [status]='O' ";

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
                    "To Edit/New Document, either close the document that's currently open for this module.",
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
                    "To Edit/New Document, either close the document that's currently open for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.qaFormName = (e.FormsCollection["request"] as Form).Name;
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            opened = true;
            //sa = new UserAuthorization(this.moduleName.ToString());

            Initialise();

            e.DBAccess.DataSet.Tables["TRNH"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRNH_ColumnChanged);
            e.DBAccess.DataSet.Tables["TRN1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRN1_ColumnChanged);
            //e.DBAccess.DataSet.Tables["TRN2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_TRN2_ColumnChanged);
            //btnSave = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Save") as Button;
            //btnConfirm = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Confirm") as Button;     


            //if (!BizFunctions.IsEmpty(TRNH["empnum"]))
            //{        
            //    string statuso = Common.DEFAULT_DOCUMENT_STATUSO;
            //    string statusp = Common.DEFAULT_DOCUMENT_STATUSP;
            //    string TRNHstatus = TRNH["status"].ToString();
            //    if (TRNHstatus == statuso || TRNHstatus == statusp)
            //    {
            //        TRNH_empnum.Enabled = false;
            //    }
            //}


        }
        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            DataTable TRN1 = this.dbaccess.DataSet.Tables["TRN1"];
            DataTable TRN2 = this.dbaccess.DataSet.Tables["TRN2"];
            //this.selectsCollection = new Hashtable();

      

            //string GetHQAM = "SELECT HQ.hqamcode,HQ.Question,a.maxScore FROM HQAM HQ "+
            //                    "LEFT JOIN "+
            //                    "(select hqamcode,MAX(score) as maxScore from HQAM1 group by hqamcode)a "+
            //                    "on HQ.hqamcode=a.hqamcode "+
            //                    "where isactive=1 and [status]<>'V'";

            //string GetHSAM = "Select * from HSAM where [status]<>'V'";


            //this.selectsCollection.Add("HQAM",GetHQAM);
            //this.selectsCollection.Add("HSAM", GetHSAM);
          

            //this.dbaccess.ReadSQL(selectsCollection);

            //if (TRNH["status"] == (string)Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    if (this.dbaccess.DataSet.Tables["HQAM"].Rows.Count > 0)
            //    {
            //        DataTable hqam = this.dbaccess.DataSet.Tables["HQAM"];

            //        foreach (DataRow dr1 in hqam.Rows)
            //        {
            //            if (dr1.RowState != DataRowState.Deleted)
            //            {
            //                DataRow InsertEar1 = TRN1.NewRow();
            //                InsertEar1["hqamcode"] = dr1["hqamcode"];
            //                InsertEar1["Question"] = dr1["Question"];
            //                InsertEar1["MaxScore"] = dr1["MaxScore"];
            //                TRN1.Rows.Add(InsertEar1);

            //            }
            //        }
            //    }
            //}

            //if (TRNH["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    if (this.dbaccess.DataSet.Tables["HSAM"].Rows.Count > 0)
            //    {
            //        DataTable hsam = this.dbaccess.DataSet.Tables["HSAM"];

            //        foreach (DataRow dr2 in hsam.Rows)
            //        {
            //            if (dr2.RowState != DataRowState.Deleted)
            //            {
            //                DataRow InsertEar2 = TRN2.NewRow();
            //                InsertEar2["hsamcode"] = dr2["hsamcode"];                            
            //                TRN2.Rows.Add(InsertEar2);

            //            }
            //        }
            //    }
            //}

            
            //txt_appraisalscore = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_appraisalscore") as TextBox;
            //txt_appraisaloverscore = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_appraisaloverscore") as TextBox;
            //txt_empname = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_empname") as TextBox;
            //TRNH_empnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "TRNH_empnum") as TextBox;
            //TRNH_empnum.Leave +=new EventHandler(TRNH_empnum_Leave);

            //TRNH_appraisedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "TRNH_appraisedcomments") as TextBox;           
            //TRNH_approvedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "TRNH_approvedcomments") as TextBox;              
            //txt_currentsal = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_currentsal") as TextBox;
            //txt_reasonforlastincre = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_reasonforlastincre") as TextBox;


           

            //TRNH_hsgcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "TRNH_hsgcode") as ComboBox;
            //TRNH_hsgcode.DropDown +=new EventHandler(TRNH_hsgcode_DropDown);

            //TRNH_TRNmcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "TRNH_TRNmcode") as ComboBox;
            //TRNH_TRNmcode.DropDown +=new EventHandler(TRNH_TRNmcode_DropDown);

            //grb_TRNHapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_TRNHapprinfo") as GroupBox;

            //rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            //rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            //rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            //rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            //rad_Recd.CheckedChanged +=new EventHandler(rad_Recd_CheckedChanged);
            //rad_NotRecd.CheckedChanged +=new EventHandler(rad_NotRecd_CheckedChanged);
            //rad_Appr.CheckedChanged +=new EventHandler(rad_Appr_CheckedChanged);
            //rad_NotAppr.CheckedChanged +=new EventHandler(rad_NotAppr_CheckedChanged);

        

            //TRNH_appraisedcomments.TextChanged += new EventHandler(TRNH_appraisedcomments_TextChanged);
            //TRNH_approvedcomments.TextChanged +=new EventHandler(TRNH_approvedcomments_TextChanged);


            //GetTotalScore();
            //GetOverallScore();
            //EmptyAppraisal();
            //LoadRadioButtonsData();

          
            //if (!sa.ApprovePermission)
            //{
            //    grb_TRNHapprinfo.Enabled = false;
            //}
            //else
            //{
            //    grb_TRNHapprinfo.Enabled = true;
            //}
     
                       
        }

        #endregion

        private void TRNH_empnum_Leave(object sender, EventArgs e)
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];

            if (TRNH_empnum.Text != String.Empty)
            {
                DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(TRNH_empnum.Text);
                if (tmpEmpData.Rows.Count > 0)
                {
                    foreach(DataRow dr1 in tmpEmpData.Rows)
                    {
                        TRNH["empnum"] = dr1["empnum"];
                        TRNH["empname"] = dr1["empname"];
                        TRNH["matnum"] = dr1["matnum"];
                        TRNH["sitenumsector"] = dr1["sitenum"] + " / " + dr1["sectorcode"];
                        TRNH["employmentdate"] = dr1["datejoined"];
                        TRNH["currentsal"] = dr1["currentsalary"];
                        TRNH["sectorcode"] = dr1["sectorcode"];
                        TRNH["newsitenum"] = dr1["sitenum"];

                    }

                    GetPreviousAppraisalInfo(TRNH["empnum"].ToString());
                }

            }
        }

        #region Appointment Code Dropdown

        protected void TRNH_hsgcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from HSGM where status<>'V'";
            this.dbaccess.ReadSQL("HSGM", sql1);
            TRNH_hsgcode.DataSource = this.dbaccess.DataSet.Tables["HSGM"];
            TRNH_hsgcode.DisplayMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
            TRNH_hsgcode.ValueMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
        }

        protected void TRNH_TRNmcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from TRNM where status<>'V'";
            this.dbaccess.ReadSQL("TRNM", sql1);
            TRNH_TRNmcode.DataSource = this.dbaccess.DataSet.Tables["TRNM"];
            TRNH_TRNmcode.DisplayMember = this.dbaccess.DataSet.Tables["TRNM"].Columns["TRNmcode"].ColumnName.ToString();
            TRNH_TRNmcode.ValueMember = this.dbaccess.DataSet.Tables["TRNM"].Columns["TRNmcode"].ColumnName.ToString();
        }

        #endregion

        #region Schedule Radio Button Methods
        private void rad_Recd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Recd.Checked)
            {
                DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];

                TRNH["isrecommended"] = 1;
            }
        
        }

        private void rad_NotRecd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotRecd.Checked)
            {
                DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];

                TRNH["isrecommended"] = 0;
            }
        }

        private void rad_Appr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Appr.Checked)
            {
                DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];

                TRNH["isapproved"] = 1;
            }
        }

        private void rad_NotAppr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotAppr.Checked)
            {
                DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];

                TRNH["isapproved"] = 0;
            }
        }
        #endregion

        #region TextChanged Events

        protected void TRNH_appraisedcomments_TextChanged(object sender, EventArgs e)
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];          

            if (TRNH_appraisedcomments.Text != "")
            {
                TRNH_approvedcomments.Enabled = true;
                rad_Recd.Enabled = true;
                rad_NotRecd.Enabled = true;    
                RecommendedBy = Common.DEFAULT_SYSTEM_USERNAME;
                
            }
            else
            {                
                TRNH_approvedcomments.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
                
            }
            
        }

        private void TRNH_approvedcomments_TextChanged(object sender, EventArgs e)
        {
            if (TRNH_approvedcomments.Text != "")
            {
                rad_Appr.Enabled = true;
                rad_NotAppr.Enabled = true;
                ApprovedBy = Common.DEFAULT_SYSTEM_USERNAME;
            }
            else
            {
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
            }
        }

        #endregion

        #region ColumnChanged Events

        private void Voucher_TRNH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable TRNH = this.dbaccess.DataSet.Tables["TRNH"];
            switch (e.Column.ColumnName)
            {
                case "apnum":
                    {
                        DataRow dr1 = BizLogicTools.Tools.GetCommonDataRow("APM", "apnum", e.Row["apnum"].ToString());

                        e.Row["apname"] = dr1["apname"];
                    }
                    break;

                case "coursecode":
                    {
                        DataRow dr1 = BizLogicTools.Tools.GetCommonDataRow("CSEH", "coursecode", e.Row["coursecode"].ToString());

                        e.Row["coursename"] = dr1["coursename"];
                        e.Row["coursevenue"] = dr1["coursevenue"];
                    }
                    break;

                //case "isPass":
                //    {
                //        if (BizFunctions.IsEmpty(e.Row["isPass"]))
                //        {
                //            e.Row["isPass"] = 0;
                //        }

                //        if ((bool)e.Row["isPassed"])
                //        {
                //            e.Row["isFailed"] = 0;
                //        }
                //    }
                //    break;

                //case "isfailed":
                //    {
                //        if (BizFunctions.IsEmpty(e.Row["isFailed"]))
                //        {
                //            e.Row["isFailed"] = 0;
                //        }

                //        if ((bool)e.Row["isFailed"])
                //        {
                //            e.Row["isPass"] = 0;
                //        }
                //    }
                //    break;
                //case "empnum":
                //    {
                //        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                //        {
                //            DataRow empDr = BizLogicTools.Tools.GetCommonEmpDataRow(e.Row["empnum"].ToString());

                //            e.Row["empname"] = empDr["empname"];
                //            e.Row["matnum"] = empDr["matnum"];
                //            e.Row["sitenum"] = empDr["sitenum"];
                //        }

                //    }
                //    break;

            }
        }

        private void Voucher_TRN1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable TRN1 = this.dbaccess.DataSet.Tables["TRN1"];
            switch (e.Column.ColumnName)
            {
                case "score":
                    {
                        GetTotalScore();
                        GetOverallScore();
                    }
                    break;

                case "isPass":
                    {
                        if (BizFunctions.IsEmpty(e.Row["isPass"]))
                        {
                            e.Row["isPass"] = 0;
                        }

                        if ((bool)e.Row["isPassed"])
                        {
                            e.Row["isFailed"] = 0;
                        }
                    }
                    break;

                case "isfailed":
                    {
                        if (BizFunctions.IsEmpty(e.Row["isFailed"]))
                        {
                            e.Row["isFailed"] = 0;
                        }

                        if ((bool)e.Row["isFailed"])
                        {
                            e.Row["isPass"] = 0;
                        }
                    }
                    break;
                case "empnum":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {

                            DataRow empDr = BizLogicTools.Tools.GetCommonEmpDataRow(e.Row["empnum"].ToString());

                            if (empDr == null)
                            {
                                e.Row["empnum"] = System.DBNull.Value;
                                e.Row["empname"] = System.DBNull.Value;
                                e.Row["matnum"] = System.DBNull.Value;
                                e.Row["sitenum"] = System.DBNull.Value;
                            }
                            else
                            {
                                e.Row["empname"] = empDr["empname"];
                                e.Row["matnum"] = empDr["matnum"];
                                e.Row["sitenum"] = empDr["sitenum"];
                            }
                        }
                      
                    }
                    break;


                case "nric":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["nric"]))
                        {

                            DataRow empDr = BizLogicTools.Tools.GetCommonEmpDataRow2(e.Row["nric"].ToString());

                            if (empDr == null)
                            {
                                e.Row["nric"] = System.DBNull.Value;
                                e.Row["empnum"] = System.DBNull.Value;
                                e.Row["empname"] = System.DBNull.Value;
                                e.Row["matnum"] = System.DBNull.Value;
                                e.Row["sitenum"] = System.DBNull.Value;
                            }
                            else
                            {
                                e.Row["empnum"] = empDr["empnum"];
                                e.Row["empname"] = empDr["empname"];
                                e.Row["matnum"] = empDr["matnum"];
                                e.Row["sitenum"] = empDr["sitenum"];
                            }
                        }

                    }
                    break;
          
            }
        }

        private void Voucher_TRN2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable TRN2 = this.dbaccess.DataSet.Tables["TRN2"];
            switch (e.Column.ColumnName)
            {
                case "rateamt":
                    {
                        GetTotalSalary();
                    }
                    break;


            }
        }

        #endregion

        #region GetTotalScore

        private void GetTotalScore()
        {
            DataTable TRN1 = this.dbaccess.DataSet.Tables["TRN1"];
            int score = 0;
            if (TRN1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TRN1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["score"]))
                        {
                            dr1["score"] = 0;
                        }
                        score += Convert.ToInt32(dr1["score"]);
                    }
                }
            }

            txt_appraisalscore.Text = score.ToString();

        }

        #endregion

        #region GetTotalSalary

        private void GetTotalSalary()
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            DataTable TRN2 = this.dbaccess.DataSet.Tables["TRN2"];
            decimal totalsal  = 0;
            if (TRN2.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TRN2.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["rateamt"]))
                        {
                            dr1["rateamt"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["isExempt"]))
                        {
                            dr1["isExempt"] = 0;
                        }

                        if (!(bool)dr1["isExempt"])
                        {
                            totalsal += Convert.ToDecimal(dr1["rateamt"]);
                        }
                    }
                }
            }

            TRNH["newsal"] = totalsal;
        }

        #endregion

        #region GetOverallScore

        private void GetOverallScore()
        {
            DataTable TRN1 = this.dbaccess.DataSet.Tables["TRN1"];
            int maxscore = 0;
            if (TRN1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TRN1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["maxscore"]))
                        {
                            dr1["maxscore"] = 0;
                        }
                        maxscore += Convert.ToInt32(dr1["maxscore"]);
                    }
                }
            }

            txt_appraisaloverscore.Text = maxscore.ToString();
        }

        #endregion

        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            DataTable TRN1 = this.dbaccess.DataSet.Tables["TRN1"];

            if (TRN1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TRN1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["staffstatus"]))
                        {
                            dr1["staffstatus"] = "P";
                        }
                    }
                }
            }

            //if (!sa.SavePermission)
            //{
            //    MessageBox.Show("Invalid Permission", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    e.Handle = false;
            //}
            //if(sa.RecommendPermission && BizFunctions.IsEmpty(TRNH["appraisedby"]))
            //{
            //    TRNH["appraisedby"] = RecommendedBy;
            //}
            //if (sa.ApprovePermission && BizFunctions.IsEmpty(TRNH["approvedby"]))
            //{
            //    TRNH["approvedby"] = ApprovedBy;
            //}

            if (BizFunctions.IsEmpty(TRNH["trandate"]))
            {
                TRNH["trandate"] = DateTime.Now;
            }

            if (!BizFunctions.IsEmpty(TRNH["trStartDate"]))
            {
                TRNH["courseyear"] = Convert.ToDateTime(TRNH["trStartDate"]).Year;
            }

            if (isDuplicateEmployees())
            {
                e.Handle = false;
            }

            //if(BizFunctions.IsEmpty(TRNH["newsaleffectivedate"]))
            //{
            //    MessageBox.Show("Please provide an effective date for the Salary Adjustment", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    e.Handle = false;
            //}

          
        }

        #endregion

        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            DataRow trnh = this.dbaccess.DataSet.Tables["trnh"].Rows[0];

            switch (e.ControlName)
            {
                case "TRNH_empnum":
                    {

                        e.Condition = BizFunctions.F2Condition("empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "empnum like '" + TRNH_empnum.Text.Trim() + "%' OR empname like '" + TRNH_empnum.Text.Trim() + "%'";

                    }
                    break;

              

                //case "TRNH_appraisedby":
                //    {
                //        if (!BizFunctions.IsEmpty(trnh["sectorcode"]))
                //        {
                //            e.DefaultCondition = "SECTORCODE like '%"+trnh["sectorcode"].ToString()+"%'";

                //        }

                //    }
                //    break;

                //case "TRNH_approvedby":
                //    {
                //        if (!BizFunctions.IsEmpty(trnh["sectorcode"]))
                //        {
                //            e.DefaultCondition = "sectorcode like '%" + trnh["sectorcode"].ToString() + "%'";
                //        }

                //    }
                //    break;
            }
        }
       

       

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            switch (e.MappingName)
            {
                case "monday":
                    {                       
                         e.DefaultCondition = "hqamcode='" + e.CurrentRow["hqamcode"].ToString() + "' ";                
                    }
                    break;

                case "empnum":
                    {
                        e.Condition = BizFunctions.F2Condition("empnum/empname", (sender as TextBox).Text);
                    }
                    break;
             
            }
        }
        #endregion

        #region F3

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            switch (e.MappingName)
            {
                case "answer":
                    e.CurrentRow["answer"] = e.F2CurrentRow["answer"];
                    e.CurrentRow["answercode"] = e.F2CurrentRow["answercode"];
                    e.CurrentRow["score"] = e.F2CurrentRow["score"];   
                    break;

                case "empnum":
                    e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                    break;


                case "nric":
                    e.CurrentRow["empnum"] = e.F2CurrentRow["empnum"];
                    e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                    break;
            }


        }


        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            switch (e.ControlName)
            {
                case "TRNH_empnum":
                    {
                        if (!BizFunctions.IsEmpty(TRNH["empnum"]))
                        {
                            GetHemphInfo(TRNH["empnum"].ToString());
                            GetPreviousAppraisalInfo(TRNH["empnum"].ToString());
                            e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                            e.CurrentRow["nric"] = e.F2CurrentRow["nric"];
                            e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                            e.CurrentRow["sitenumsector"] = e.F2CurrentRow["sitenum"].ToString()+"/"+e.F2CurrentRow["sectorcode"].ToString();

                            e.CurrentRow["newsitenum"] = e.F2CurrentRow["sitenum"];
                        }
                    }
                    break;

                case "trnh_coursecode":
                    {
                        e.CurrentRow["coursename"] = e.F2CurrentRow["coursename"];
                        e.CurrentRow["coursevenue"] = e.F2CurrentRow["coursevenue"];                
                    }
                    break;


                case "trnh_apnum":
                    {
                        e.CurrentRow["apname"] = e.F2CurrentRow["apname"];
                    }
                    break;
                           
            }
        }

        #endregion

        #region Get Hemph Info

        private void GetHemphInfo(string empnum)
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            DataTable TRN2 = this.dbaccess.DataSet.Tables["TRN2"];
            string sql = "select empname,matnum,sitenum,sectorcode,datejoined,statuscode from hemph where empnum='" + empnum + "'";
            this.dbaccess.ReadSQL("dtHEMPH", sql);
            if (this.dbaccess.DataSet.Tables["dtHEMPH"].Rows.Count > 0)
            {
                DataRow drHEMPH = this.dbaccess.DataSet.Tables["dtHEMPH"].Rows[0];
                TRNH["empname"] = drHEMPH["empname"].ToString();
                TRNH["matnum"] = drHEMPH["matnum"].ToString();
                TRNH["statuscode"] = drHEMPH["statuscode"].ToString();
                TRNH["sitenumsector"] = drHEMPH["sitenum"].ToString() + " / " + drHEMPH["sectorcode"].ToString();
                TRNH["sitenum"] = drHEMPH["sitenum"];

                if (BizFunctions.IsEmpty(TRNH["newsitenum"]))
                {
                    TRNH["newsitenum"] = drHEMPH["sitenum"];
                }
                if (!BizFunctions.IsEmpty(drHEMPH["datejoined"]))
                {

                    TRNH["employmentdate"] = Convert.ToDateTime(drHEMPH["datejoined"]).ToShortDateString();
                }
                
            }
            this.dbaccess.DataSet.Tables["dtHEMPH"].Dispose();
        }

        #endregion

        #region Get Previous Appraisal Data

        private void GetPreviousAppraisalInfo(string empnum)
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            DataTable TRN2 = this.dbaccess.DataSet.Tables["TRN2"];
            DataRow dr1,dr2=null;
            string sql = " Select * From  "+
                           "(  "+
                               "Select  "+
                               "ROW_NUMBER() OVER (Order BY newsaleffectivedate) as ForTop,ROW_NUMBER() OVER (Order BY newsaleffectivedate Desc) as ForBottom, "+
                               "refnum,empnum,newsaleffectivedate,newsal, approvedcomments,trandate,isapproved  " +
                               "from  "+
                               "TRNH where empnum='" + empnum + "' and status<>'V'" +
                           ")A  "+
                           "Where ForBottom <=2 ";

            this.dbaccess.ReadSQL("dtTRNH", sql);
            if (this.dbaccess.DataSet.Tables["dtTRNH"].Rows.Count > 0)
            {
                
                    dr1 = this.dbaccess.DataSet.Tables["dtTRNH"].Rows[0];
                    DateTime dt1 = DateTime.Now;
                    DateTime dt2 = TimeTools.GetSafeDate(BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["newsaleffectivedate"].ToString())));
                    int result = DateTime.Compare(dt1, dt2);


                    if (BizFunctions.IsEmpty(dr1["isapproved"]))
                    {
                        dr1["isapproved"] = 0;
                    }

                    if ((bool)dr1["isapproved"])
                    {
                        if (result >= 0)
                        {
                            TRNH["currentsal"] = Convert.ToDecimal(dr1["newsal"]);
                            TRNH["reasonforlastincre"] = dr1["approvedcomments"].ToString();                        
                            TRNH["lastincrementdate"] = Convert.ToDateTime(dr1["newsaleffectivedate"].ToString()).ToShortDateString();

                            string GetTRN2 = "Select * from TRN2 where refnum='" + dr1["refnum"].ToString() + "' order by myline";
                            this.dbaccess.ReadSQL("dtTRN2", GetTRN2);

                            if (this.dbaccess.DataSet.Tables["dtTRN2"].Rows.Count > 0)
                            {
                                BizFunctions.DeleteAllRows(TRN2);
                                foreach (DataRow dr3 in this.dbaccess.DataSet.Tables["dtTRN2"].Rows)
                                {
                                    if (dr3.RowState != DataRowState.Deleted)
                                    {
                                        DataRow InsertEar2 = TRN2.NewRow();
                                        InsertEar2["hsamcode"] = dr3["hsamcode"];
                                        InsertEar2["rateamt"] = dr3["rateamt"];
                                        InsertEar2["line"] = dr3["line"];
                                        TRN2.Rows.Add(InsertEar2);
                                    }
                                    
                                }
                                GetTotalSalary();
                            }
                            this.dbaccess.DataSet.Tables["dtTRN2"].Dispose();
                        }
                        else
                        {
                            MessageBox.Show("Employee '" + empnum + "' under refnum '" + dr1["refnum"].ToString() + "' has already been Appraised on '" + Convert.ToDateTime(dr1["trandate"]).ToShortDateString() + "'  and salary increment will take effect on '" + Convert.ToString(dt2.ToShortDateString()) + "' ");
                            btnSave.Enabled = false;
                            btnConfirm.Enabled = false;

                        }

                    }
                    else
                    {
                        MessageBox.Show("There is a pending Appraisal to be checked for Employee '" + empnum + "' under refnum '" + dr1["refnum"].ToString() + "' has already been Appraised on '" + Convert.ToDateTime(dr1["trandate"]).ToShortDateString() + "'.  Please confirm that refnum before re-creating");
                        btnSave.Enabled = false;
                        btnConfirm.Enabled = false;

                    }
              
            }
            else
            {

                string sql2 = "Select * from PFMSR where empnum='" + empnum + "' order by myline";

                this.dbaccess.ReadSQL("dtPFMSR", sql2);
                if (this.dbaccess.DataSet.Tables["dtPFMSR"].Rows.Count > 0)
                {

                    BizFunctions.DeleteAllRows(TRN2);


                    decimal TotalCurrentSal = 0;

                    foreach (DataRow dr3 in this.dbaccess.DataSet.Tables["dtPFMSR"].Rows)
                    {
                        if (dr3.RowState != DataRowState.Deleted)
                        {
                            DataRow InsertTRN2 = TRN2.NewRow();
                            InsertTRN2["hsamcode"] = dr3["hsamcode"];
                            InsertTRN2["rateamt"] = dr3["rateamt"];
                            InsertTRN2["line"] = dr3["line"];
                            TRN2.Rows.Add(InsertTRN2);
                            TotalCurrentSal += Convert.ToDecimal(InsertTRN2["rateamt"]);
                        }
                        
                    }
                    FromPFMSR = true;
                    GetTotalSalary();
                    TRNH["currentsal"] = TotalCurrentSal;
                    
                }
               
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
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];


            try
            {
                //string GetSalcode = "Select distinct salcode from TRN2 where refnum='" + TRNH["refnum"] + "'";

                //this.dbaccess.ReadSQL("dtSalcode", GetSalcode);

                //if (this.dbaccess.DataSet.Tables["dtSalcode"].Rows.Count > 0)
                //{
                    if (TRNH["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSO)
                    {
                        //DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                        // Delete this current refnum first.	
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + TRNH["refnum"].ToString() + "'");
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + TRNH["refnum"].ToString() + "'");

                        UpdatePFMSRonVoid();
                    }

                //}

                //this.dbaccess.DataSet.Tables["dtSalcode"].Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

          
        }

        protected override void Document_Void_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Void_Handle(sender, e);
          
        }

        protected override void Document_Void_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Void_OnClick(sender, e);
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];


            try
            {
                //string GetSalcode = "Select distinct salcode from TRN2 where refnum='" + TRNH["refnum"] + "'";

                //this.dbaccess.ReadSQL("dtSalcode", GetSalcode);

                //if (this.dbaccess.DataSet.Tables["dtSalcode"].Rows.Count > 0)
                //{
                if (TRNH["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSV)
                {
                    //DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                    // Delete this current refnum first.	
                    //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + TRNH["refnum"].ToString() + "'");
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + TRNH["refnum"].ToString() + "'");

                    UpdatePFMSRonVoid();
                }

                //}

                //this.dbaccess.DataSet.Tables["dtSalcode"].Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

       
        }

        #endregion

        #region Duplicate Handle
        protected override void Document_Duplicate_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Duplicate_Handle(sender, e);
            e.Handle = false;
        }
        #endregion
       
        #region Save Begin

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            DataTable TRN1 = this.dbaccess.DataSet.Tables["TRN1"];
            DataTable TRN2 = this.dbaccess.DataSet.Tables["TRN2"];



            //if (ApprovedBy != string.Empty)
            //{
            //    TRNH["appraisedby"] = RecommendedBy;
            //}
            //if (RecommendedBy != string.Empty)
            //{
            //    TRNH["approvedby"] = ApprovedBy;
            //}
         
            #region TRN1
            foreach (DataRow dr1 in TRN1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(TRNH, dr1, "refnum/user/flag/status/created/modified");

                    if (BizFunctions.IsEmpty(dr1["recommendreason"]))
                    {
                        dr1["recommendreason"] = TRNH["appraisedcomments"];
                    }
                }
            }
            #endregion

            #region TRN2
            //foreach (DataRow dr2 in TRN2.Rows)
            //{
            //    if (dr2.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(TRNH, dr2, "empnum/hsgcode/trnmcode/refnum/user/flag/status/created/modified");

            //        dr2["dateadjusted"] = TRNH["newsaleffectivedate"];

            //        if (TRNH["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSP)
            //        {
            //            dr2["salcode"] = TRNH["empnum"].ToString() + "-" + BizFunctions.GetSafeDateString(Convert.ToDateTime(TRNH["newsaleffectivedate"]));
            //            //dr2["salcode"] = TRNH["refnum"];
            //        }

            //        dr2["AdjustmentFlag"] = TRNH["flag"];
            //    }
            //}
            #endregion                                                     
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];


             
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            //GetTotalSalary();

        }
        #endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            //if (BizFunctions.IsEmpty(TRNH["newsaleffectivedate"]))
            //{                
            //    MessageBox.Show("Can't confirm, effective date is empty", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //}


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
     
        #region if Appraisal Comments is Empty

        private void EmptyAppraisal()
        {           
            if (TRNH_appraisedcomments.Text == "")
            {
                TRNH_approvedcomments.Enabled = false;
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
            }
        }

        #endregion

        #region Load Radio Button Data

        private void LoadRadioButtonsData()
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];

            if(!BizFunctions.IsEmpty(TRNH["isrecommended"]))
            {
                if((bool)TRNH["isrecommended"])
                {
                    rad_Recd.Checked = true;
                }
                else
                {
                    rad_Recd.Checked = false;
                }
            }

            if(!BizFunctions.IsEmpty(TRNH["isapproved"]))
            {
                if ((bool)TRNH["isapproved"])
                {
                    rad_Appr.Checked = true;
                }
                else
                {
                    rad_Appr.Checked = false;
                }
            }
        }

        #endregion    

        #region Update PFMSR on Void

        private void UpdatePFMSRonVoid()
        {
            DataRow TRNH = this.dbaccess.DataSet.Tables["TRNH"].Rows[0];
            string salcode = "";

            string GetLatestSALH = "Select * from "+
	                                "( "+
	                                "Select ROW_NUMBER() OVER (Order BY dateadjusted) as ForTop,ROW_NUMBER() OVER (Order BY dateadjusted Desc) as ForBottom, * "+
	                                "from "+
		                                "( "+
		                                "select refnum, salcode,empnum,matnum,SUM(rateamt) as TotalAmt,dateadjusted,adjustmentflag "+
		                                "from SALH "+
		                                "where empnum='" + TRNH["empnum"].ToString() + "' and status<>'V' "+
		                                "group by refnum, salcode,empnum,matnum,nric,dateadjusted,adjustmentflag "+
		                                ")a "+
	                                ")b "+
	                                "where ForBottom = 1";

            this.dbaccess.ReadSQL("dtGetLatestSALH", GetLatestSALH);

            DataTable dtGetLatestSALH = this.dbaccess.DataSet.Tables["dtGetLatestSALH"];


            string strNRIC = BizLogicTools.Tools.GetNRIC(TRNH["empnum"].ToString());


            if (dtGetLatestSALH.Rows.Count > 0)
            {
                DataRow drGetLatestSALH = this.dbaccess.DataSet.Tables["dtGetLatestSALH"].Rows[0];

                //create empty PFMSR datatable
                string strpfmsr = "Select * from pfmsr where 1=2 ";
                this.dbaccess.ReadSQL("PFMSR", strpfmsr);
                DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];

                string GetSALHdata = "Select * from salh where refnum='" + drGetLatestSALH["refnum"].ToString() + "'";

                this.dbaccess.ReadSQL("dtSALHdata", GetSALHdata);
                DataTable dtSALHdata = this.dbaccess.DataSet.Tables["dtSALHdata"];

                if (dtSALHdata.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in dtSALHdata.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            DataRow InsertPfmsr = pfmsr.NewRow();

                            InsertPfmsr["refnum"] = dr1["refnum"];
                            InsertPfmsr["empnum"] = dr1["empnum"];
                            InsertPfmsr["nric"] = strNRIC;
                            InsertPfmsr["rateamt"] = dr1["rateamt"];
                            InsertPfmsr["hsamcode"] = dr1["hsamcode"];
                            InsertPfmsr["remarks"] = dr1["remarks"];
                            InsertPfmsr["dateadjusted"] = dr1["dateadjusted"];
                            InsertPfmsr["hsgcode"] = dr1["hsgcode"];
                            InsertPfmsr["salcode"] = dr1["salcode"];
                            InsertPfmsr["flag"] = dr1["flag"];
                            InsertPfmsr["line"] = dr1["line"];
                            InsertPfmsr["AdjustmentFlag"] = dr1["AdjustmentFlag"];
                            InsertPfmsr["TRNmcode"] = dr1["TRNmcode"];                            
                            pfmsr.Rows.Add(InsertPfmsr);
                         
                        }
                    }
                }


                #region PFMSR
                foreach (DataRow dr2 in pfmsr.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(TRNH, dr2, "user/status/created/modified");
                    }

                }
                #endregion


                #region Assign ids to pfmsr table for saving

                string maxid = "SELECT ISNULL(MAX(id),0) AS 'id' FROM PFMSR";
                DataSet maxtmp = this.dbaccess.ReadSQLTemp("idpfmsr", maxid);

                int a = 0;
                if (maxtmp.Tables["idpfmsr"].Rows.Count > 0)
                {
                    if (maxtmp.Tables["idpfmsr"].Rows[0]["id"] == System.DBNull.Value)
                        a = 0;
                    else
                        a = Convert.ToInt32(maxtmp.Tables["idpfmsr"].Rows[0]["id"]) + 1;
                }
                else
                {
                    a = 0;
                }

                foreach (DataRow dr in pfmsr.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        dr["id"] = a;
                        a++;
                    }
                }

                #endregion


                Hashtable tablesCollections = new Hashtable();
                foreach (DataTable dataTable in this.dbaccess.DataSet.Tables)
                {
                    tablesCollections[dataTable.TableName] = dataTable.TableName;
                }

                DataTable[] dataTablestemp = new DataTable[1];
                dataTablestemp[0] = pfmsr;
                dataTablestemp[0].TableName = pfmsr.TableName.ToString();

                try
                {

                    // Delete this current refnum first.	
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM pfmsr WHERE empnum = '" + TRNH["empnum"].ToString() + "'");

                    this.dbaccess.Update(dataTablestemp);

                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from pfmsr) where tablename = 'pfmsr'");


                    //Dispose at end
                    pfmsr.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

            //Dispose at end
            this.dbaccess.DataSet.Tables["dtGetLatestSALH"].Dispose();


        }

        #endregion

        private bool isDuplicateEmployees()
        {
            bool duplicate = false;

            string message = string.Empty;

            DataTable TRN1Tmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select empnum,count(*) as total from TRN1 group by empnum");

            if(TRN1Tmp != null)
            {
                if(TRN1Tmp.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in TRN1Tmp.Rows)
                    {
                        if (Convert.ToInt16(dr1["total"]) > 1)
                        {
                            message = message + dr1["empnum"].ToString() + "\n";
                            duplicate = true;
                        }
                    }
                    
                    if (message != string.Empty)
                    {
                        MessageBox.Show("Duplicate Employee information\n\n" + message + " ", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

            return duplicate;
        }

    }
}
    

