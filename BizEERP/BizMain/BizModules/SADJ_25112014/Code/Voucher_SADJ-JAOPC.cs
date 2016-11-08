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

namespace ATL.SADJ
{
    public class Voucher_SADJ : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables

        UserAuthorization sa = null;
        protected DBAccess dbaccess = null;
        protected Hashtable selectsCollection = null;
        protected DataGrid datagrid1;
        protected TextBox txt_appraisalscore, txt_appraisaloverscore,txt_empname,
                          txt_currentsal, txt_reasonforlastincre, txt_newsal, SADJh_empnum, SADJh_appraisedcomments, SADJh_approvedcomments= null;
        protected GroupBox grb_SADJhapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        protected ComboBox cb, SADJh_hsgcode, SADJh_SADJmcode = null;
        protected string headerFormName,qaFormName, RecommendedBy, ApprovedBy = null;

        protected int SADJ1RowNum = -1;
        bool FromPFMSR = false;
        protected bool opened = false;

        protected Button btnSave,btnConfirm = null;

        #endregion

        #region Construct

        public Voucher_SADJ(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_SADJ.xml", moduleName, voucherBaseHelpers)
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
            this.qaFormName = (e.FormsCollection["qas"] as Form).Name;
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            opened = true;
            sa = new UserAuthorization(this.moduleName.ToString());

            Initialise();

    
            e.DBAccess.DataSet.Tables["SADJ1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SADJ1_ColumnChanged);
            e.DBAccess.DataSet.Tables["SADJ2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SADJ2_ColumnChanged);
            btnSave = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Save") as Button;
            btnConfirm = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Confirm") as Button;     


            if (!BizFunctions.IsEmpty(SADJh["empnum"]))
            {        
                string statuso = Common.DEFAULT_DOCUMENT_STATUSO;
                string statusp = Common.DEFAULT_DOCUMENT_STATUSP;
                string SADJhstatus = SADJh["status"].ToString();
                if (SADJhstatus == statuso || SADJhstatus == statusp)
                {
                    SADJh_empnum.Enabled = false;
                }
            }


        }
        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];
            this.selectsCollection = new Hashtable();

      

            string GetHQAM = "SELECT HQ.hqamcode,HQ.Question,a.maxScore FROM HQAM HQ "+
                                "LEFT JOIN "+
                                "(select hqamcode,MAX(score) as maxScore from HQAM1 group by hqamcode)a "+
                                "on HQ.hqamcode=a.hqamcode "+
                                "where isactive=1 and [status]<>'V'";

            string GetHSAM = "Select * from HSAM where [status]<>'V'";


            this.selectsCollection.Add("HQAM",GetHQAM);
            this.selectsCollection.Add("HSAM", GetHSAM);
          

            this.dbaccess.ReadSQL(selectsCollection);

            if (SADJh["status"] == (string)Common.DEFAULT_DOCUMENT_STATUSN)
            {
                if (this.dbaccess.DataSet.Tables["HQAM"].Rows.Count > 0)
                {
                    DataTable hqam = this.dbaccess.DataSet.Tables["HQAM"];

                    foreach (DataRow dr1 in hqam.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            DataRow InsertEar1 = SADJ1.NewRow();
                            InsertEar1["hqamcode"] = dr1["hqamcode"];
                            InsertEar1["Question"] = dr1["Question"];
                            InsertEar1["MaxScore"] = dr1["MaxScore"];
                            SADJ1.Rows.Add(InsertEar1);

                        }
                    }
                }
            }

            if (SADJh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                if (this.dbaccess.DataSet.Tables["HSAM"].Rows.Count > 0)
                {
                    DataTable hsam = this.dbaccess.DataSet.Tables["HSAM"];

                    foreach (DataRow dr2 in hsam.Rows)
                    {
                        if (dr2.RowState != DataRowState.Deleted)
                        {
                            DataRow InsertEar2 = SADJ2.NewRow();
                            InsertEar2["hsamcode"] = dr2["hsamcode"];                            
                            SADJ2.Rows.Add(InsertEar2);

                        }
                    }
                }
            }

            
            txt_appraisalscore = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_appraisalscore") as TextBox;
            txt_appraisaloverscore = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_appraisaloverscore") as TextBox;
            txt_empname = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_empname") as TextBox;
            SADJh_empnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_empnum") as TextBox;
            SADJh_empnum.Leave +=new EventHandler(SADJh_empnum_Leave);

            SADJh_appraisedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_appraisedcomments") as TextBox;           
            SADJh_approvedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_approvedcomments") as TextBox;              
            txt_currentsal = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_currentsal") as TextBox;
            txt_reasonforlastincre = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_reasonforlastincre") as TextBox;


           

            SADJh_hsgcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_hsgcode") as ComboBox;
            SADJh_hsgcode.DropDown +=new EventHandler(SADJh_hsgcode_DropDown);

            SADJh_SADJmcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_SADJmcode") as ComboBox;
            SADJh_SADJmcode.DropDown +=new EventHandler(SADJh_SADJmcode_DropDown);

            grb_SADJhapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_SADJhapprinfo") as GroupBox;

            rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            rad_Recd.CheckedChanged +=new EventHandler(rad_Recd_CheckedChanged);
            rad_NotRecd.CheckedChanged +=new EventHandler(rad_NotRecd_CheckedChanged);
            rad_Appr.CheckedChanged +=new EventHandler(rad_Appr_CheckedChanged);
            rad_NotAppr.CheckedChanged +=new EventHandler(rad_NotAppr_CheckedChanged);

        

            SADJh_appraisedcomments.TextChanged += new EventHandler(SADJh_appraisedcomments_TextChanged);
            SADJh_approvedcomments.TextChanged +=new EventHandler(SADJh_approvedcomments_TextChanged);


            GetTotalScore();
            GetOverallScore();
            EmptyAppraisal();
            LoadRadioButtonsData();

          
            if (!sa.ApprovePermission)
            {
                grb_SADJhapprinfo.Enabled = false;
            }
            else
            {
                grb_SADJhapprinfo.Enabled = true;
            }
     
                       
        }

        #endregion

        private void SADJh_empnum_Leave(object sender, EventArgs e)
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJh"].Rows[0];

            if (SADJh_empnum.Text != String.Empty)
            {
                DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(SADJh_empnum.Text);
                if (tmpEmpData.Rows.Count > 0)
                {
                    foreach(DataRow dr1 in tmpEmpData.Rows)
                    {
                        SADJh["empnum"] = dr1["empnum"];
                        SADJh["empname"] = dr1["empname"];
                        SADJh["matnum"] = dr1["matnum"];
                        SADJh["sitenumsector"] = dr1["sitenum"] + " / " + dr1["sectorcode"];
                        SADJh["employmentdate"] = dr1["datejoined"];
                        SADJh["currentsal"] = dr1["currentsalary"];
                        SADJh["sectorcode"] = dr1["sectorcode"];
                        SADJh["newsitenum"] = dr1["sitenum"];

                    }

                    GetPreviousAppraisalInfo(SADJh["empnum"].ToString());
                }

            }
        }

        #region Appointment Code Dropdown

        protected void SADJh_hsgcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from HSGM where status<>'V'";
            this.dbaccess.ReadSQL("HSGM", sql1);
            SADJh_hsgcode.DataSource = this.dbaccess.DataSet.Tables["HSGM"];
            SADJh_hsgcode.DisplayMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
            SADJh_hsgcode.ValueMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
        }

        protected void SADJh_SADJmcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from SADJM where status<>'V'";
            this.dbaccess.ReadSQL("SADJM", sql1);
            SADJh_SADJmcode.DataSource = this.dbaccess.DataSet.Tables["SADJM"];
            SADJh_SADJmcode.DisplayMember = this.dbaccess.DataSet.Tables["SADJM"].Columns["SADJmcode"].ColumnName.ToString();
            SADJh_SADJmcode.ValueMember = this.dbaccess.DataSet.Tables["SADJM"].Columns["SADJmcode"].ColumnName.ToString();
        }

        #endregion

        #region Schedule Radio Button Methods
        private void rad_Recd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Recd.Checked)
            {
                DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];

                SADJh["isrecommended"] = 1;
            }
        
        }

        private void rad_NotRecd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotRecd.Checked)
            {
                DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];

                SADJh["isrecommended"] = 0;
            }
        }

        private void rad_Appr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Appr.Checked)
            {
                DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];

                SADJh["isapproved"] = 1;
            }
        }

        private void rad_NotAppr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotAppr.Checked)
            {
                DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];

                SADJh["isapproved"] = 0;
            }
        }
        #endregion

        #region TextChanged Events

        protected void SADJh_appraisedcomments_TextChanged(object sender, EventArgs e)
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];          

            if (SADJh_appraisedcomments.Text != "")
            {
                SADJh_approvedcomments.Enabled = true;
                rad_Recd.Enabled = true;
                rad_NotRecd.Enabled = true;    
                RecommendedBy = Common.DEFAULT_SYSTEM_USERNAME;
                
            }
            else
            {                
                SADJh_approvedcomments.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
                
            }
            
        }

        private void SADJh_approvedcomments_TextChanged(object sender, EventArgs e)
        {
            if (SADJh_approvedcomments.Text != "")
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

        private void Voucher_SADJ1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            switch (e.Column.ColumnName)
            {
                case "score":
                    {
                        GetTotalScore();
                        GetOverallScore();
                    }
                    break;

          
            }
        }

        private void Voucher_SADJ2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];
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
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            int score = 0;
            if (SADJ1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in SADJ1.Rows)
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
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];
            decimal totalsal  = 0;
            if (SADJ2.Rows.Count > 0)
            {
                foreach (DataRow dr1 in SADJ2.Rows)
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

            SADJh["newsal"] = totalsal;
        }

        #endregion

        #region GetOverallScore

        private void GetOverallScore()
        {
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            int maxscore = 0;
            if (SADJ1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in SADJ1.Rows)
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
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];


            //if (!sa.SavePermission)
            //{
            //    MessageBox.Show("Invalid Permission", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    e.Handle = false;
            //}
            //if(sa.RecommendPermission && BizFunctions.IsEmpty(SADJh["appraisedby"]))
            //{
            //    SADJh["appraisedby"] = RecommendedBy;
            //}
            //if (sa.ApprovePermission && BizFunctions.IsEmpty(SADJh["approvedby"]))
            //{
            //    SADJh["approvedby"] = ApprovedBy;
            //}

            if (BizFunctions.IsEmpty(SADJh["trandate"]))
            {
                SADJh["trandate"] = DateTime.Now;
            }
        

            if(BizFunctions.IsEmpty(SADJh["newsaleffectivedate"]))
            {
                MessageBox.Show("Please provide an effective date for the Salary Adjustment", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handle = false;
            }

          
        }

        #endregion

        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            DataRow sadjh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];

            switch (e.ControlName)
            {
                case "SADJh_empnum":
                    {

                        e.Condition = BizFunctions.F2Condition("empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "empnum like '" + SADJh_empnum.Text.Trim() + "%' OR empname like '" + SADJh_empnum.Text.Trim() + "%'";

                    }
                    break;

                //case "SADJh_appraisedby":
                //    {
                //        if (!BizFunctions.IsEmpty(sadjh["sectorcode"]))
                //        {
                //            e.DefaultCondition = "SECTORCODE like '%"+sadjh["sectorcode"].ToString()+"%'";

                //        }

                //    }
                //    break;

                //case "SADJh_approvedby":
                //    {
                //        if (!BizFunctions.IsEmpty(sadjh["sectorcode"]))
                //        {
                //            e.DefaultCondition = "sectorcode like '%" + sadjh["sectorcode"].ToString() + "%'";
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
            }
        }


        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            switch (e.ControlName)
            {
                case "SADJh_empnum":
                    {
                        if (!BizFunctions.IsEmpty(SADJh["empnum"]))
                        {
                            GetHemphInfo(SADJh["empnum"].ToString());
                            GetPreviousAppraisalInfo(SADJh["empnum"].ToString());
                            e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                            e.CurrentRow["nric"] = e.F2CurrentRow["nric"];
                            e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                            e.CurrentRow["sitenumsector"] = e.F2CurrentRow["sitenum"].ToString()+"/"+e.F2CurrentRow["sectorcode"].ToString();

                            e.CurrentRow["newsitenum"] = e.F2CurrentRow["sitenum"];
                        }
                    }
                    break;
                           
            }
        }

        #endregion

        #region Get Hemph Info

        private void GetHemphInfo(string empnum)
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];
            string sql = "select empname,matnum,sitenum,sectorcode,datejoined,statuscode from hemph where empnum='" + empnum + "'";
            this.dbaccess.ReadSQL("dtHEMPH", sql);
            if (this.dbaccess.DataSet.Tables["dtHEMPH"].Rows.Count > 0)
            {
                DataRow drHEMPH = this.dbaccess.DataSet.Tables["dtHEMPH"].Rows[0];
                SADJh["empname"] = drHEMPH["empname"].ToString();
                SADJh["matnum"] = drHEMPH["matnum"].ToString();
                SADJh["statuscode"] = drHEMPH["statuscode"].ToString();
                SADJh["sitenumsector"] = drHEMPH["sitenum"].ToString() + " / " + drHEMPH["sectorcode"].ToString();
                SADJh["sitenum"] = drHEMPH["sitenum"];

                if (BizFunctions.IsEmpty(SADJh["newsitenum"]))
                {
                    SADJh["newsitenum"] = drHEMPH["sitenum"];
                }
                if (!BizFunctions.IsEmpty(drHEMPH["datejoined"]))
                {

                    SADJh["employmentdate"] = Convert.ToDateTime(drHEMPH["datejoined"]).ToShortDateString();
                }
                
            }
            this.dbaccess.DataSet.Tables["dtHEMPH"].Dispose();
        }

        #endregion

        #region Get Previous Appraisal Data

        private void GetPreviousAppraisalInfo(string empnum)
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];
            DataRow dr1,dr2=null;
            string sql = " Select * From  "+
                           "(  "+
                               "Select  "+
                               "ROW_NUMBER() OVER (Order BY newsaleffectivedate) as ForTop,ROW_NUMBER() OVER (Order BY newsaleffectivedate Desc) as ForBottom, "+
                               "refnum,empnum,newsaleffectivedate,newsal, approvedcomments,trandate,isapproved  " +
                               "from  "+
                               "SADJH where empnum='" + empnum + "' and status<>'V'" +
                           ")A  "+
                           "Where ForBottom <=2 ";

            this.dbaccess.ReadSQL("dtSADJH", sql);
            if (this.dbaccess.DataSet.Tables["dtSADJH"].Rows.Count > 0)
            {
                
                    dr1 = this.dbaccess.DataSet.Tables["dtSADJH"].Rows[0];
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
                            SADJh["currentsal"] = Convert.ToDecimal(dr1["newsal"]);
                            SADJh["reasonforlastincre"] = dr1["approvedcomments"].ToString();                        
                            SADJh["lastincrementdate"] = Convert.ToDateTime(dr1["newsaleffectivedate"].ToString()).ToShortDateString();

                            string GetSADJ2 = "Select * from SADJ2 where refnum='" + dr1["refnum"].ToString() + "' order by myline";
                            this.dbaccess.ReadSQL("dtSADJ2", GetSADJ2);

                            if (this.dbaccess.DataSet.Tables["dtSADJ2"].Rows.Count > 0)
                            {
                                BizFunctions.DeleteAllRows(SADJ2);
                                foreach (DataRow dr3 in this.dbaccess.DataSet.Tables["dtSADJ2"].Rows)
                                {
                                    if (dr3.RowState != DataRowState.Deleted)
                                    {
                                        DataRow InsertEar2 = SADJ2.NewRow();
                                        InsertEar2["hsamcode"] = dr3["hsamcode"];
                                        InsertEar2["rateamt"] = dr3["rateamt"];
                                        InsertEar2["line"] = dr3["line"];
                                        SADJ2.Rows.Add(InsertEar2);
                                    }
                                    
                                }
                                GetTotalSalary();
                            }
                            this.dbaccess.DataSet.Tables["dtSADJ2"].Dispose();
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

                    BizFunctions.DeleteAllRows(SADJ2);


                    decimal TotalCurrentSal = 0;

                    foreach (DataRow dr3 in this.dbaccess.DataSet.Tables["dtPFMSR"].Rows)
                    {
                        if (dr3.RowState != DataRowState.Deleted)
                        {
                            DataRow InsertSADJ2 = SADJ2.NewRow();
                            InsertSADJ2["hsamcode"] = dr3["hsamcode"];
                            InsertSADJ2["rateamt"] = dr3["rateamt"];
                            InsertSADJ2["line"] = dr3["line"];
                            SADJ2.Rows.Add(InsertSADJ2);
                            TotalCurrentSal += Convert.ToDecimal(InsertSADJ2["rateamt"]);
                        }
                        
                    }
                    FromPFMSR = true;
                    GetTotalSalary();
                    SADJh["currentsal"] = TotalCurrentSal;
                    
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
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];


            try
            {
                //string GetSalcode = "Select distinct salcode from SADJ2 where refnum='" + SADJh["refnum"] + "'";

                //this.dbaccess.ReadSQL("dtSalcode", GetSalcode);

                //if (this.dbaccess.DataSet.Tables["dtSalcode"].Rows.Count > 0)
                //{
                    if (SADJh["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSO)
                    {
                        //DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                        // Delete this current refnum first.	
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + SADJh["refnum"].ToString() + "'");
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + SADJh["refnum"].ToString() + "'");

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
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];


            try
            {
                //string GetSalcode = "Select distinct salcode from SADJ2 where refnum='" + SADJh["refnum"] + "'";

                //this.dbaccess.ReadSQL("dtSalcode", GetSalcode);

                //if (this.dbaccess.DataSet.Tables["dtSalcode"].Rows.Count > 0)
                //{
                if (SADJh["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSV)
                {
                    //DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                    // Delete this current refnum first.	
                    //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + SADJh["refnum"].ToString() + "'");
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + SADJh["refnum"].ToString() + "'");

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
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];



            //if (ApprovedBy != string.Empty)
            //{
            //    SADJh["appraisedby"] = RecommendedBy;
            //}
            //if (RecommendedBy != string.Empty)
            //{
            //    SADJh["approvedby"] = ApprovedBy;
            //}
         
            #region SADJ1
            foreach (DataRow dr1 in SADJ1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(SADJh, dr1, "refnum/user/flag/status/created/modified");
                }
            }
            #endregion

            #region SADJ2
            foreach (DataRow dr2 in SADJ2.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(SADJh, dr2, "empnum/hsgcode/sadjmcode/refnum/user/flag/status/created/modified");

                    dr2["dateadjusted"] = SADJh["newsaleffectivedate"];

                    if (SADJh["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSP)
                    {
                        dr2["salcode"] = SADJh["empnum"].ToString() + "-" + BizFunctions.GetSafeDateString(Convert.ToDateTime(SADJh["newsaleffectivedate"]));
                        //dr2["salcode"] = SADJh["refnum"];
                    }

                    dr2["AdjustmentFlag"] = SADJh["flag"];
                }
            }
            #endregion                                                     
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];

            string strsalh = "Select * from salh where 1=2";
            string strpfmsr = "Select * from pfmsr where 1=2 ";
            this.dbaccess.ReadSQL("SALH", strsalh);
            this.dbaccess.ReadSQL("PFMSR", strpfmsr);

            DataTable salh = this.dbaccess.DataSet.Tables["SALH"];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];

            string strNRIC = BizLogicTools.Tools.GetNRIC(SADJh["empnum"].ToString());

            if (SADJh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                BizFunctions.DeleteAllRows(salh);
                foreach (DataRow dr12 in SADJ2.Rows)
                {
                    if (dr12.RowState != DataRowState.Deleted)
                    {

                        DataRow InsertSalh = salh.NewRow();
                        InsertSalh["refnum"] = SADJh["refnum"];
                        InsertSalh["empnum"] = dr12["empnum"];
                        InsertSalh["nric"] = strNRIC;
                        InsertSalh["matnum"] = SADJh["matnum"];
                        InsertSalh["rateamt"] = dr12["rateamt"];
                        InsertSalh["hsamcode"] = dr12["hsamcode"];
                        InsertSalh["remarks"] = dr12["remarks"];
                        InsertSalh["dateadjusted"] = dr12["dateadjusted"];
                        InsertSalh["hsgcode"] = dr12["hsgcode"];
                        InsertSalh["AdjustmentFlag"] = dr12["Flag"];
                        InsertSalh["SADJmcode"] = dr12["SADJmcode"];
                        InsertSalh["salcode"] = dr12["salcode"];
                        InsertSalh["sitenum"] = SADJh["newsitenum"];
                        InsertSalh["isExempt"] = SADJh["isExempt"];
                        InsertSalh["line"] = dr12["line"];
                        InsertSalh["status"] = Common.DEFAULT_DOCUMENT_STATUSP;
                        salh.Rows.Add(InsertSalh);
                    }
                }


                #region SALH
                decimal Line1 = 0;
                foreach (DataRow dr13 in salh.Rows)
                {
                    if (dr13.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(SADJh, dr13, "empnum/refnum/user/flag/status/created/modified");
                        if (BizFunctions.IsEmpty(dr13["line"]))
                        {
                            Line1 = Line1 + 100;
                            dr13["line"] = Line1;
                        }
                    }

                }
                #endregion


                foreach (DataRow dr14 in SADJ2.Rows)
                {
                    if (dr14.RowState != DataRowState.Deleted)
                    {
                        DataRow Insertpfmsr = pfmsr.NewRow();
                        Insertpfmsr["refnum"] = dr14["empnum"];
                        Insertpfmsr["empnum"] = dr14["empnum"];
                        Insertpfmsr["nric"] = strNRIC;
                        Insertpfmsr["rateamt"] = dr14["rateamt"];
                        Insertpfmsr["hsamcode"] = dr14["hsamcode"];
                        Insertpfmsr["remarks"] = dr14["remarks"];
                        Insertpfmsr["dateadjusted"] = dr14["dateadjusted"];
                        Insertpfmsr["hsgcode"] = dr14["hsgcode"];
                        Insertpfmsr["salcode"] = dr14["salcode"];
                        Insertpfmsr["flag"] = "HEMP";
                        Insertpfmsr["AdjustmentFlag"] = dr14["Flag"];
                        Insertpfmsr["SADJmcode"] = dr14["SADJmcode"];
                        Insertpfmsr["line"] = dr14["line"];
                        Insertpfmsr["sitenum"] = SADJh["newsitenum"];
                        Insertpfmsr["docunum"] = SADJh["refnum"];
                        pfmsr.Rows.Add(Insertpfmsr);
                    }
                }

                #region PFMSR
                decimal Line2 = 0;
                foreach (DataRow dr15 in pfmsr.Rows)
                {
                    if (dr15.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(SADJh, dr15, "user/status/created/modified");
                        if (BizFunctions.IsEmpty(dr15["line"]))
                        {
                            Line2 = Line1 + 100;
                            dr15["line"] = Line1;
                        }
                    }

                }
                #endregion

                if (SADJh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
                {
                    if (!BizFunctions.IsEmpty(SADJh["isapproved"]) && (bool)SADJh["isapproved"])
                    {
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


                        #region Assign ids to salh table for saving

                        string maxsalhid = "SELECT ISNULL(MAX(id),0) AS 'id' FROM salh";
                        DataSet maxsaltmp = this.dbaccess.ReadSQLTemp("idsalh", maxsalhid);

                        int b = 0;
                        if (maxsaltmp.Tables["idsalh"].Rows.Count > 0)
                        {
                            if (maxsaltmp.Tables["idsalh"].Rows[0]["id"] == System.DBNull.Value)
                                b = 0;
                            else
                                b = Convert.ToInt32(maxsaltmp.Tables["idsalh"].Rows[0]["id"]) + 1;
                        }
                        else
                        {
                            b = 0;
                        }

                        foreach (DataRow dr in salh.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                dr["id"] = b;
                                b++;
                            }
                        }

                        #endregion

                        try
                        {

                            #region Save to SALH

                            foreach (DataTable dataTable in e.DBAccess.DataSet.Tables)
                            {
                                if (dataTable.TableName == "salh")
                                {
                                    DataTable tempDataTable = dataTable.Clone();

                                    if (tempDataTable.Columns.Contains("mark"))
                                        tempDataTable.Columns.Remove("mark");
                                }
                            }


                            Hashtable tablesCollections = new Hashtable();
                            foreach (DataTable dataTable in this.dbaccess.DataSet.Tables)
                            {
                                tablesCollections[dataTable.TableName] = dataTable.TableName;
                            }

                            DataTable[] dataTablestemp = new DataTable[2];
                            dataTablestemp[0] = salh;
                            dataTablestemp[0].TableName = salh.TableName.ToString();
                            dataTablestemp[1] = pfmsr;
                            dataTablestemp[1].TableName = pfmsr.TableName.ToString();



                            // Delete this current refnum first.	
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM salh WHERE refnum = '" + SADJh["refnum"].ToString() + "'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM pfmsr WHERE empnum = '" + SADJh["empnum"].ToString() + "'");

                            this.dbaccess.Update(dataTablestemp);


                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from salh) where tablename = 'SALH'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from pfmsr) where tablename = 'pfmsr'");

                            if (!BizFunctions.IsEmpty(SADJh["newsitenum"]))
                            {

                                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set sitenum = '" +
                                                                                SADJh["newsitenum"].ToString() +
                                                                                "' where empnum='" +
                                                                                SADJh["empnum"].ToString() + "' ");


                                string newSector = BizLogicTools.Tools.GetSectorCode(SADJh["newsitenum"].ToString(),
                                                                                     this.dbaccess);

                                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set sectorcode = '" +
                                                                                newSector + "' where empnum='" +
                                                                                SADJh["empnum"].ToString() + "' ");

                            }

                            //remoteDBAccess.DataSet.Tables.ClSADJ();

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                }
                salh.Dispose();
            }
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            GetTotalSalary();

        }
        #endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            if (BizFunctions.IsEmpty(SADJh["newsaleffectivedate"]))
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
            if (SADJh_appraisedcomments.Text == "")
            {
                SADJh_approvedcomments.Enabled = false;
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
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];

            if(!BizFunctions.IsEmpty(SADJh["isrecommended"]))
            {
                if((bool)SADJh["isrecommended"])
                {
                    rad_Recd.Checked = true;
                }
                else
                {
                    rad_Recd.Checked = false;
                }
            }

            if(!BizFunctions.IsEmpty(SADJh["isapproved"]))
            {
                if ((bool)SADJh["isapproved"])
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
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            string salcode = "";

            string GetLatestSALH = "Select * from "+
	                                "( "+
	                                "Select ROW_NUMBER() OVER (Order BY dateadjusted) as ForTop,ROW_NUMBER() OVER (Order BY dateadjusted Desc) as ForBottom, * "+
	                                "from "+
		                                "( "+
		                                "select refnum, salcode,empnum,matnum,SUM(rateamt) as TotalAmt,dateadjusted,adjustmentflag "+
		                                "from SALH "+
		                                "where empnum='" + SADJh["empnum"].ToString() + "' and status<>'V' "+
		                                "group by refnum, salcode,empnum,matnum,nric,dateadjusted,adjustmentflag "+
		                                ")a "+
	                                ")b "+
	                                "where ForBottom = 1";

            this.dbaccess.ReadSQL("dtGetLatestSALH", GetLatestSALH);

            DataTable dtGetLatestSALH = this.dbaccess.DataSet.Tables["dtGetLatestSALH"];


            string strNRIC = BizLogicTools.Tools.GetNRIC(SADJh["empnum"].ToString());


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
                            InsertPfmsr["SADJmcode"] = dr1["SADJmcode"];                            
                            pfmsr.Rows.Add(InsertPfmsr);
                         
                        }
                    }
                }


                #region PFMSR
                foreach (DataRow dr2 in pfmsr.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(SADJh, dr2, "user/status/created/modified");
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
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM pfmsr WHERE empnum = '" + SADJh["empnum"].ToString() + "'");

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
    

