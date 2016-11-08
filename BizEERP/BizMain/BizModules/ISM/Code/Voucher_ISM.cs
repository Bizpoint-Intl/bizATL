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

namespace ATL.ISM
{
    public class Voucher_ISM : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables

        UserAuthorization sa = null;
        protected DBAccess dbaccess = null;
        protected Hashtable selectsCollection = null;
        protected DataGrid datagrid1;
        protected TextBox txt_appraisalscore, txt_appraisaloverscore,txt_empname,
                          txt_currentsal, txt_reasonforlastincre, txt_newsal, ISM_empnum, ISM_appraisedcomments, ISM_approvedcomments= null;
        protected GroupBox grb_ISMapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        protected ComboBox cb, ISM_hsgcode, ISM_ISMmcode = null;
        protected string headerFormName,qaFormName, RecommendedBy, ApprovedBy = null;

        protected int ISM1RowNum = -1;
        bool FromPFMSR = false;
        protected bool opened = false;

        protected Button btnSave,btnConfirm = null;

        #endregion

        #region Construct

        public Voucher_ISM(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_ISM.xml", moduleName, voucherBaseHelpers)
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
            //this.qaFormName = (e.FormsCollection["request"] as Form).Name;
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            opened = true;
            //sa = new UserAuthorization(this.moduleName.ToString());

            //Initialise();

            e.DBAccess.DataSet.Tables["ISM"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ISM_ColumnChanged);


        }
        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            DataTable ISM1 = this.dbaccess.DataSet.Tables["ISM1"];
            DataTable ISM2 = this.dbaccess.DataSet.Tables["ISM2"];
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

            //if (ISM["status"] == (string)Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    if (this.dbaccess.DataSet.Tables["HQAM"].Rows.Count > 0)
            //    {
            //        DataTable hqam = this.dbaccess.DataSet.Tables["HQAM"];

            //        foreach (DataRow dr1 in hqam.Rows)
            //        {
            //            if (dr1.RowState != DataRowState.Deleted)
            //            {
            //                DataRow InsertEar1 = ISM1.NewRow();
            //                InsertEar1["hqamcode"] = dr1["hqamcode"];
            //                InsertEar1["Question"] = dr1["Question"];
            //                InsertEar1["MaxScore"] = dr1["MaxScore"];
            //                ISM1.Rows.Add(InsertEar1);

            //            }
            //        }
            //    }
            //}

            //if (ISM["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    if (this.dbaccess.DataSet.Tables["HSAM"].Rows.Count > 0)
            //    {
            //        DataTable hsam = this.dbaccess.DataSet.Tables["HSAM"];

            //        foreach (DataRow dr2 in hsam.Rows)
            //        {
            //            if (dr2.RowState != DataRowState.Deleted)
            //            {
            //                DataRow InsertEar2 = ISM2.NewRow();
            //                InsertEar2["hsamcode"] = dr2["hsamcode"];                            
            //                ISM2.Rows.Add(InsertEar2);

            //            }
            //        }
            //    }
            //}

            
            //txt_appraisalscore = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_appraisalscore") as TextBox;
            //txt_appraisaloverscore = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_appraisaloverscore") as TextBox;
            //txt_empname = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_empname") as TextBox;
            //ISM_empnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ISM_empnum") as TextBox;
            //ISM_empnum.Leave +=new EventHandler(ISM_empnum_Leave);

            //ISM_appraisedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ISM_appraisedcomments") as TextBox;           
            //ISM_approvedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ISM_approvedcomments") as TextBox;              
            //txt_currentsal = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_currentsal") as TextBox;
            //txt_reasonforlastincre = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_reasonforlastincre") as TextBox;


           

            //ISM_hsgcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ISM_hsgcode") as ComboBox;
            //ISM_hsgcode.DropDown +=new EventHandler(ISM_hsgcode_DropDown);

            //ISM_ISMmcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ISM_ISMmcode") as ComboBox;
            //ISM_ISMmcode.DropDown +=new EventHandler(ISM_ISMmcode_DropDown);

            //grb_ISMapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_ISMapprinfo") as GroupBox;

            //rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            //rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            //rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            //rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            //rad_Recd.CheckedChanged +=new EventHandler(rad_Recd_CheckedChanged);
            //rad_NotRecd.CheckedChanged +=new EventHandler(rad_NotRecd_CheckedChanged);
            //rad_Appr.CheckedChanged +=new EventHandler(rad_Appr_CheckedChanged);
            //rad_NotAppr.CheckedChanged +=new EventHandler(rad_NotAppr_CheckedChanged);

        

            //ISM_appraisedcomments.TextChanged += new EventHandler(ISM_appraisedcomments_TextChanged);
            //ISM_approvedcomments.TextChanged +=new EventHandler(ISM_approvedcomments_TextChanged);


            //GetTotalScore();
            //GetOverallScore();
            //EmptyAppraisal();
            //LoadRadioButtonsData();

          
            //if (!sa.ApprovePermission)
            //{
            //    grb_ISMapprinfo.Enabled = false;
            //}
            //else
            //{
            //    grb_ISMapprinfo.Enabled = true;
            //}
     
                       
        }

        #endregion

        private void ISM_empnum_Leave(object sender, EventArgs e)
        {
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];

            if (ISM_empnum.Text != String.Empty)
            {
                DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(ISM_empnum.Text);
                if (tmpEmpData.Rows.Count > 0)
                {
                    foreach(DataRow dr1 in tmpEmpData.Rows)
                    {
                        ISM["empnum"] = dr1["empnum"];
                        ISM["empname"] = dr1["empname"];
                        ISM["matnum"] = dr1["matnum"];
                        ISM["sitenumsector"] = dr1["sitenum"] + " / " + dr1["sectorcode"];
                        ISM["employmentdate"] = dr1["datejoined"];
                        ISM["currentsal"] = dr1["currentsalary"];
                        ISM["sectorcode"] = dr1["sectorcode"];
                        ISM["newsitenum"] = dr1["sitenum"];

                    }

                    GetPreviousAppraisalInfo(ISM["empnum"].ToString());
                }

            }
        }

        #region Appointment Code Dropdown

        protected void ISM_hsgcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from HSGM where status<>'V'";
            this.dbaccess.ReadSQL("HSGM", sql1);
            ISM_hsgcode.DataSource = this.dbaccess.DataSet.Tables["HSGM"];
            ISM_hsgcode.DisplayMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
            ISM_hsgcode.ValueMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
        }

        protected void ISM_ISMmcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from ISMM where status<>'V'";
            this.dbaccess.ReadSQL("ISMM", sql1);
            ISM_ISMmcode.DataSource = this.dbaccess.DataSet.Tables["ISMM"];
            ISM_ISMmcode.DisplayMember = this.dbaccess.DataSet.Tables["ISMM"].Columns["ISMmcode"].ColumnName.ToString();
            ISM_ISMmcode.ValueMember = this.dbaccess.DataSet.Tables["ISMM"].Columns["ISMmcode"].ColumnName.ToString();
        }

        #endregion

        #region Schedule Radio Button Methods
        private void rad_Recd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Recd.Checked)
            {
                DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];

                ISM["isrecommended"] = 1;
            }
        
        }

        private void rad_NotRecd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotRecd.Checked)
            {
                DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];

                ISM["isrecommended"] = 0;
            }
        }

        private void rad_Appr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Appr.Checked)
            {
                DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];

                ISM["isapproved"] = 1;
            }
        }

        private void rad_NotAppr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotAppr.Checked)
            {
                DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];

                ISM["isapproved"] = 0;
            }
        }
        #endregion

        #region TextChanged Events

        protected void ISM_appraisedcomments_TextChanged(object sender, EventArgs e)
        {
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];          

            if (ISM_appraisedcomments.Text != "")
            {
                ISM_approvedcomments.Enabled = true;
                rad_Recd.Enabled = true;
                rad_NotRecd.Enabled = true;    
                RecommendedBy = Common.DEFAULT_SYSTEM_USERNAME;
                
            }
            else
            {                
                ISM_approvedcomments.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
                
            }
            
        }

        private void ISM_approvedcomments_TextChanged(object sender, EventArgs e)
        {
            if (ISM_approvedcomments.Text != "")
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

        private void Voucher_ISM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ISM = this.dbaccess.DataSet.Tables["ISM"];
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

        private void Voucher_ISM1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ISM1 = this.dbaccess.DataSet.Tables["ISM1"];
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
          
            }
        }

        private void Voucher_ISM2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ISM2 = this.dbaccess.DataSet.Tables["ISM2"];
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
            DataTable ISM1 = this.dbaccess.DataSet.Tables["ISM1"];
            int score = 0;
            if (ISM1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ISM1.Rows)
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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            DataTable ISM2 = this.dbaccess.DataSet.Tables["ISM2"];
            decimal totalsal  = 0;
            if (ISM2.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ISM2.Rows)
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

            ISM["newsal"] = totalsal;
        }

        #endregion

        #region GetOverallScore

        private void GetOverallScore()
        {
            DataTable ISM1 = this.dbaccess.DataSet.Tables["ISM1"];
            int maxscore = 0;
            if (ISM1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ISM1.Rows)
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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            //DataTable ISM1 = this.dbaccess.DataSet.Tables["ISM1"];

            //if (ISM1.Rows.Count > 0)
            //{
            //    foreach (DataRow dr1 in ISM1.Rows)
            //    {
            //        if (dr1.RowState != DataRowState.Deleted)
            //        {
            //            if (BizFunctions.IsEmpty(dr1["staffstatus"]))
            //            {
            //                dr1["staffstatus"] = "P";
            //            }
            //        }
            //    }
            //}

            ////if (!sa.SavePermission)
            ////{
            ////    MessageBox.Show("Invalid Permission", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ////    e.Handle = false;
            ////}
            ////if(sa.RecommendPermission && BizFunctions.IsEmpty(ISM["appraisedby"]))
            ////{
            ////    ISM["appraisedby"] = RecommendedBy;
            ////}
            ////if (sa.ApprovePermission && BizFunctions.IsEmpty(ISM["approvedby"]))
            ////{
            ////    ISM["approvedby"] = ApprovedBy;
            ////}

            //if (BizFunctions.IsEmpty(ISM["trandate"]))
            //{
            //    ISM["trandate"] = DateTime.Now;
            //}
        

            //if(BizFunctions.IsEmpty(ISM["newsaleffectivedate"]))
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

            DataRow ism = this.dbaccess.DataSet.Tables["ism"].Rows[0];

            switch (e.ControlName)
            {
                case "ISM_empnum":
                    {

                        e.Condition = BizFunctions.F2Condition("empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "empnum like '" + ISM_empnum.Text.Trim() + "%' OR empname like '" + ISM_empnum.Text.Trim() + "%'";

                    }
                    break;

              

                //case "ISM_appraisedby":
                //    {
                //        if (!BizFunctions.IsEmpty(ism["sectorcode"]))
                //        {
                //            e.DefaultCondition = "SECTORCODE like '%"+ism["sectorcode"].ToString()+"%'";

                //        }

                //    }
                //    break;

                //case "ISM_approvedby":
                //    {
                //        if (!BizFunctions.IsEmpty(ism["sectorcode"]))
                //        {
                //            e.DefaultCondition = "sectorcode like '%" + ism["sectorcode"].ToString() + "%'";
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
            }


        }


        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            switch (e.ControlName)
            {
                case "ISM_empnum":
                    {
                        if (!BizFunctions.IsEmpty(ISM["empnum"]))
                        {
                            GetHemphInfo(ISM["empnum"].ToString());
                            GetPreviousAppraisalInfo(ISM["empnum"].ToString());
                            e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                            e.CurrentRow["nric"] = e.F2CurrentRow["nric"];
                            e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                            e.CurrentRow["sitenumsector"] = e.F2CurrentRow["sitenum"].ToString()+"/"+e.F2CurrentRow["sectorcode"].ToString();

                            e.CurrentRow["newsitenum"] = e.F2CurrentRow["sitenum"];
                        }
                    }
                    break;

                case "ism_coursecode":
                    {
                        e.CurrentRow["coursename"] = e.F2CurrentRow["coursename"];
                        e.CurrentRow["coursevenue"] = e.F2CurrentRow["coursevenue"];                
                    }
                    break;


                case "ism_apnum":
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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            DataTable ISM2 = this.dbaccess.DataSet.Tables["ISM2"];
            string sql = "select empname,matnum,sitenum,sectorcode,datejoined,statuscode from hemph where empnum='" + empnum + "'";
            this.dbaccess.ReadSQL("dtHEMPH", sql);
            if (this.dbaccess.DataSet.Tables["dtHEMPH"].Rows.Count > 0)
            {
                DataRow drHEMPH = this.dbaccess.DataSet.Tables["dtHEMPH"].Rows[0];
                ISM["empname"] = drHEMPH["empname"].ToString();
                ISM["matnum"] = drHEMPH["matnum"].ToString();
                ISM["statuscode"] = drHEMPH["statuscode"].ToString();
                ISM["sitenumsector"] = drHEMPH["sitenum"].ToString() + " / " + drHEMPH["sectorcode"].ToString();
                ISM["sitenum"] = drHEMPH["sitenum"];

                if (BizFunctions.IsEmpty(ISM["newsitenum"]))
                {
                    ISM["newsitenum"] = drHEMPH["sitenum"];
                }
                if (!BizFunctions.IsEmpty(drHEMPH["datejoined"]))
                {

                    ISM["employmentdate"] = Convert.ToDateTime(drHEMPH["datejoined"]).ToShortDateString();
                }
                
            }
            this.dbaccess.DataSet.Tables["dtHEMPH"].Dispose();
        }

        #endregion

        #region Get Previous Appraisal Data

        private void GetPreviousAppraisalInfo(string empnum)
        {
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            DataTable ISM2 = this.dbaccess.DataSet.Tables["ISM2"];
            DataRow dr1,dr2=null;
            string sql = " Select * From  "+
                           "(  "+
                               "Select  "+
                               "ROW_NUMBER() OVER (Order BY newsaleffectivedate) as ForTop,ROW_NUMBER() OVER (Order BY newsaleffectivedate Desc) as ForBottom, "+
                               "refnum,empnum,newsaleffectivedate,newsal, approvedcomments,trandate,isapproved  " +
                               "from  "+
                               "ISM where empnum='" + empnum + "' and status<>'V'" +
                           ")A  "+
                           "Where ForBottom <=2 ";

            this.dbaccess.ReadSQL("dtISM", sql);
            if (this.dbaccess.DataSet.Tables["dtISM"].Rows.Count > 0)
            {
                
                    dr1 = this.dbaccess.DataSet.Tables["dtISM"].Rows[0];
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
                            ISM["currentsal"] = Convert.ToDecimal(dr1["newsal"]);
                            ISM["reasonforlastincre"] = dr1["approvedcomments"].ToString();                        
                            ISM["lastincrementdate"] = Convert.ToDateTime(dr1["newsaleffectivedate"].ToString()).ToShortDateString();

                            string GetISM2 = "Select * from ISM2 where refnum='" + dr1["refnum"].ToString() + "' order by myline";
                            this.dbaccess.ReadSQL("dtISM2", GetISM2);

                            if (this.dbaccess.DataSet.Tables["dtISM2"].Rows.Count > 0)
                            {
                                BizFunctions.DeleteAllRows(ISM2);
                                foreach (DataRow dr3 in this.dbaccess.DataSet.Tables["dtISM2"].Rows)
                                {
                                    if (dr3.RowState != DataRowState.Deleted)
                                    {
                                        DataRow InsertEar2 = ISM2.NewRow();
                                        InsertEar2["hsamcode"] = dr3["hsamcode"];
                                        InsertEar2["rateamt"] = dr3["rateamt"];
                                        InsertEar2["line"] = dr3["line"];
                                        ISM2.Rows.Add(InsertEar2);
                                    }
                                    
                                }
                                GetTotalSalary();
                            }
                            this.dbaccess.DataSet.Tables["dtISM2"].Dispose();
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

                    BizFunctions.DeleteAllRows(ISM2);


                    decimal TotalCurrentSal = 0;

                    foreach (DataRow dr3 in this.dbaccess.DataSet.Tables["dtPFMSR"].Rows)
                    {
                        if (dr3.RowState != DataRowState.Deleted)
                        {
                            DataRow InsertISM2 = ISM2.NewRow();
                            InsertISM2["hsamcode"] = dr3["hsamcode"];
                            InsertISM2["rateamt"] = dr3["rateamt"];
                            InsertISM2["line"] = dr3["line"];
                            ISM2.Rows.Add(InsertISM2);
                            TotalCurrentSal += Convert.ToDecimal(InsertISM2["rateamt"]);
                        }
                        
                    }
                    FromPFMSR = true;
                    GetTotalSalary();
                    ISM["currentsal"] = TotalCurrentSal;
                    
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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];


            try
            {
                //string GetSalcode = "Select distinct salcode from ISM2 where refnum='" + ISM["refnum"] + "'";

                //this.dbaccess.ReadSQL("dtSalcode", GetSalcode);

                //if (this.dbaccess.DataSet.Tables["dtSalcode"].Rows.Count > 0)
                //{
                    if (ISM["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSO)
                    {
                        //DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                        // Delete this current refnum first.	
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + ISM["refnum"].ToString() + "'");
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + ISM["refnum"].ToString() + "'");

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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];


            try
            {
                //string GetSalcode = "Select distinct salcode from ISM2 where refnum='" + ISM["refnum"] + "'";

                //this.dbaccess.ReadSQL("dtSalcode", GetSalcode);

                //if (this.dbaccess.DataSet.Tables["dtSalcode"].Rows.Count > 0)
                //{
                if (ISM["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSV)
                {
                    //DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                    // Delete this current refnum first.	
                    //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + ISM["refnum"].ToString() + "'");
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + ISM["refnum"].ToString() + "'");

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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            DataTable ISM1 = this.dbaccess.DataSet.Tables["ISM1"];
            DataTable ISM2 = this.dbaccess.DataSet.Tables["ISM2"];



            //if (ApprovedBy != string.Empty)
            //{
            //    ISM["appraisedby"] = RecommendedBy;
            //}
            //if (RecommendedBy != string.Empty)
            //{
            //    ISM["approvedby"] = ApprovedBy;
            //}
         
            //#region ISM1
            //foreach (DataRow dr1 in ISM1.Rows)
            //{
            //    if (dr1.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(ISM, dr1, "refnum/user/flag/status/created/modified");

            //        if (BizFunctions.IsEmpty(dr1["recommendreason"]))
            //        {
            //            dr1["recommendreason"] = ISM["appraisedcomments"];
            //        }
            //    }
            //}
            //#endregion

            //#region ISM2
            ////foreach (DataRow dr2 in ISM2.Rows)
            ////{
            ////    if (dr2.RowState != DataRowState.Deleted)
            ////    {
            ////        BizFunctions.UpdateDataRow(ISM, dr2, "empnum/hsgcode/ismmcode/refnum/user/flag/status/created/modified");

            ////        dr2["dateadjusted"] = ISM["newsaleffectivedate"];

            ////        if (ISM["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSP)
            ////        {
            ////            dr2["salcode"] = ISM["empnum"].ToString() + "-" + BizFunctions.GetSafeDateString(Convert.ToDateTime(ISM["newsaleffectivedate"]));
            ////            //dr2["salcode"] = ISM["refnum"];
            ////        }

            ////        dr2["AdjustmentFlag"] = ISM["flag"];
            ////    }
            ////}
            //#endregion                                                     
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
           
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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            if (BizFunctions.IsEmpty(ISM["newsaleffectivedate"]))
            {                
                MessageBox.Show("Can't confirm, effective date is empty", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
            }


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
            if (ISM_appraisedcomments.Text == "")
            {
                ISM_approvedcomments.Enabled = false;
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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];

            if(!BizFunctions.IsEmpty(ISM["isrecommended"]))
            {
                if((bool)ISM["isrecommended"])
                {
                    rad_Recd.Checked = true;
                }
                else
                {
                    rad_Recd.Checked = false;
                }
            }

            if(!BizFunctions.IsEmpty(ISM["isapproved"]))
            {
                if ((bool)ISM["isapproved"])
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
            DataRow ISM = this.dbaccess.DataSet.Tables["ISM"].Rows[0];
            string salcode = "";

            string GetLatestSALH = "Select * from "+
	                                "( "+
	                                "Select ROW_NUMBER() OVER (Order BY dateadjusted) as ForTop,ROW_NUMBER() OVER (Order BY dateadjusted Desc) as ForBottom, * "+
	                                "from "+
		                                "( "+
		                                "select refnum, salcode,empnum,matnum,SUM(rateamt) as TotalAmt,dateadjusted,adjustmentflag "+
		                                "from SALH "+
		                                "where empnum='" + ISM["empnum"].ToString() + "' and status<>'V' "+
		                                "group by refnum, salcode,empnum,matnum,nric,dateadjusted,adjustmentflag "+
		                                ")a "+
	                                ")b "+
	                                "where ForBottom = 1";

            this.dbaccess.ReadSQL("dtGetLatestSALH", GetLatestSALH);

            DataTable dtGetLatestSALH = this.dbaccess.DataSet.Tables["dtGetLatestSALH"];


            string strNRIC = BizLogicTools.Tools.GetNRIC(ISM["empnum"].ToString());


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
                            InsertPfmsr["ISMmcode"] = dr1["ISMmcode"];                            
                            pfmsr.Rows.Add(InsertPfmsr);
                         
                        }
                    }
                }


                #region PFMSR
                foreach (DataRow dr2 in pfmsr.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(ISM, dr2, "user/status/created/modified");
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
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM pfmsr WHERE empnum = '" + ISM["empnum"].ToString() + "'");

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

    }
}
    

