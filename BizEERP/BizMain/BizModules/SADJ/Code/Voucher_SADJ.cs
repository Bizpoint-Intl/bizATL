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
                          txt_currentsal, txt_reasonforlastincre, txt_newsal, SADJh_empnum, SADJh_appraisedcomments, SADJh_approvedcomments, qas_ttscore,
                          qas_ttfactor, qas_ttavgfactor, txt_psalary, txt_csalary = null;
        protected DateTimePicker SADJh_reqeffectivedate, SADJh_trandate = null;
        protected GroupBox grb_SADJhapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        protected ComboBox cb, SADJh_hsgcode, SADJh_SADJmcode, SADJh_oldpaytypecode, SADJh_paytypecode = null;
        protected string headerFormName, qaFormName, RecommendedBy, ApprovedBy, salaryFormName = null;

        protected int SADJ1RowNum = -1;
        bool FromPFMSR = false;
        protected bool opened = false;
        protected string flag = "";

        protected Button btnSave,btnConfirm = null;

        #endregion

        #region Construct

        public Voucher_SADJ(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_SADJ.xml", moduleName, voucherBaseHelpers)
        {
            this.flag = moduleName;
        }

        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);
    
            if (flag.ToUpper().Trim() == "APPH")
            {
                e.Condition = " sadjh.flag='" + flag + "' and (sadjh.[user]='" + Common.DEFAULT_SYSTEM_USERNAME + "' OR '" + Common.DEFAULT_SYSTEM_USERNAME + "' IN (Select UserName from SysUserGroup where GroupName='Administrator' )) ";
            }
            else
            {
                e.Condition = " sadjh.flag='" + flag + "'";
            }

       

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);


            if (flag.ToUpper().Trim() == "APPH")
            {
                e.Condition = " (sadjh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                              " sadjh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                              " sadjh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                              " AND sadjh.flag='" + flag + "' AND (sadjh.[user]='" + Common.DEFAULT_SYSTEM_USERNAME + "' OR '" + Common.DEFAULT_SYSTEM_USERNAME + "' IN (Select UserName from SysUserGroup where GroupName='Administrator' )) ";
            }
            else
            {
                e.Condition = "flag='" + flag + "' and [status]='O'";
            }

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
            this.salaryFormName = (e.FormsCollection["salaries"] as Form).Name;
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable sadj2 = this.dbaccess.DataSet.Tables["sadj2"];
            DataTable sadj3 = this.dbaccess.DataSet.Tables["sadj3"];
            opened = true;
            sa = new UserAuthorization(this.moduleName.ToString());


            if (SADJh["flag"].ToString().ToUpper().Trim() == "APPH")
            {
                if (BizFunctions.IsEmpty(SADJh["empnum"]))
                {
                    SADJh["empnum"] = Common.DEFAULT_SYSTEM_EMPNUM;
                }

                if (BizFunctions.IsEmpty(SADJh["approvedby"]))
                {
                    SADJh["approvedby"] = GetSupervisor(SADJh["empnum"].ToString());
                }
            }
            else
            {
                if (BizFunctions.IsEmpty(SADJh["approvedby"]))
                {
                    SADJh["approvedby"] = Common.DEFAULT_SYSTEM_USERNAME; ;
                }
            }

            Initialise();


            e.DBAccess.DataSet.Tables["SADJH"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SADJH_ColumnChanged);
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

                //GetTiming();

             
                    if (SADJh_empnum.Text != String.Empty)
                    {
                        DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(SADJh_empnum.Text);
                        if (tmpEmpData.Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in tmpEmpData.Rows)
                            {
                                if(BizFunctions.IsEmpty(SADJh["empname"]))
                                {
                                    SADJh["empname"] = dr1["empname"];
                                }
                                if (BizFunctions.IsEmpty(SADJh["matnum"]))
                                {
                                    SADJh["matnum"] = dr1["matnum"];

                                }
                                if (BizFunctions.IsEmpty(SADJh["sitenumsector"]))
                                {
                                    SADJh["sitenumsector"] = dr1["sitenum"] + " / " + dr1["sectorcode"];

                                }
                                if (BizFunctions.IsEmpty(SADJh["employmentdate"]))
                                {
                                    SADJh["employmentdate"] = dr1["datejoined"];

                                }
                                if (BizFunctions.IsEmpty(SADJh["currentsal"]))
                                {
                                   SADJh["currentsal"] = dr1["currentsalary"];

                                }
                                if (BizFunctions.IsEmpty(SADJh["sectorcode"]))
                                {
                                   SADJh["sectorcode"] = dr1["sectorcode"];

                                }
                                if (BizFunctions.IsEmpty(SADJh["newsitenum"]))
                                {
                                   SADJh["newsitenum"] = dr1["sitenum"];

                                }
                                if (BizFunctions.IsEmpty(SADJh["olddaysperweek"]))
                                {
                                    SADJh["olddaysperweek"] = dr1["daysperweek"];

                                }
                            

                            }

                        
                    }
                }
            }

            if (!BizFunctions.IsEmpty(SADJh["empnum"]))
            {
                DataTable EmpDT = BizLogicTools.Tools.GetCommonEmpData(SADJh["empnum"].ToString());
                if (EmpDT.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(SADJh["empname"]))
                    {
                        SADJh["empname"] = EmpDT.Rows[0]["empname"].ToString();
                    }
                    if (BizFunctions.IsEmpty(SADJh["nric"]))
                    {
                        SADJh["nric"] = BizLogicTools.Tools.GetNRIC2(SADJh["empnum"].ToString(), this.dbaccess);
                    }
                    if (BizFunctions.IsEmpty(SADJh["matnum"]))
                    {
                        SADJh["matnum"] = EmpDT.Rows[0]["matnum"].ToString();
                    }
                    if (BizFunctions.IsEmpty(SADJh["statuscode"]))
                    {
                        SADJh["statuscode"] = EmpDT.Rows[0]["statuscode"].ToString();
                    }

                    if (BizFunctions.IsEmpty(SADJh["sitenum"]))
                    {
                        SADJh["sitenum"] = EmpDT.Rows[0]["sitenum"].ToString();
                    }
                    if (BizFunctions.IsEmpty(SADJh["sectorcode"]))
                    {
                        SADJh["sectorcode"] = BizLogicTools.Tools.GetSectorCode(SADJh["sitenum"].ToString(), this.dbaccess);
                    }
                }
            }
            if (BizFunctions.IsEmpty(SADJh["trandate"]))
            {
                SADJh["trandate"] = DateTime.Now;
            }


           



            if (sadj3.Rows.Count == 0)
            {
                GetCurrentSalary();
            }

            if (flag.ToUpper().Trim() == "SADJ")
            {
                txt_psalary.Text = Convert.ToString(getTotalSalary(sadj2));
                txt_csalary.Text = Convert.ToString(getTotalSalary(sadj3));
            }

            if (SADJh["flag"].ToString().Trim() == "APPR" || SADJh["flag"].ToString().Trim() == "APPH")
            {

                if (BizFunctions.IsEmpty(SADJh["ratingother"]))
                {
                    SADJh["ratingother"] = "Annual Review";
                }
            }

            LoadCurrentTiming();

        }
        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];
            this.selectsCollection = new Hashtable();


            if(BizFunctions.IsEmpty(SADJh["t1"]))
            {
                SADJh["t1"] = 1;
            }
            if (BizFunctions.IsEmpty(SADJh["t2"]))
            {
                SADJh["t2"] = 1;
            }
            if (BizFunctions.IsEmpty(SADJh["t3"]))
            {
                SADJh["t3"] = 1;
            }
            if (BizFunctions.IsEmpty(SADJh["t4"]))
            {
                SADJh["t4"] = 1;
            }
            if (BizFunctions.IsEmpty(SADJh["t5"]))
            {
                SADJh["t5"] = 1;
            }
      

            //string GetHQAM = "SELECT HQ.hqamcode,HQ.Question,a.maxScore FROM HQAM HQ "+
            //                    "LEFT JOIN "+
            //                    "(select hqamcode,MAX(score) as maxScore from HQAM1 group by hqamcode)a "+
            //                    "on HQ.hqamcode=a.hqamcode "+
            //                    "where isactive=1 and [status]<>'V'";

            string GetHSAM = "Select * from HSAM where [status]<>'V'";


            //this.selectsCollection.Add("HQAM",GetHQAM);
            this.selectsCollection.Add("HSAM", GetHSAM);


            this.dbaccess.ReadSQL(selectsCollection);

            //if (SADJh["status"] == (string)Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    if (this.dbaccess.DataSet.Tables["HQAM"].Rows.Count > 0)
            //    {
            //        DataTable hqam = this.dbaccess.DataSet.Tables["HQAM"];

            //        foreach (DataRow dr1 in hqam.Rows)
            //        {
            //            if (dr1.RowState != DataRowState.Deleted)
            //            {
            //                DataRow InsertEar1 = SADJ1.NewRow();
            //                InsertEar1["hqamcode"] = dr1["hqamcode"];
            //                InsertEar1["Question"] = dr1["Question"];
            //                InsertEar1["MaxScore"] = dr1["MaxScore"];
            //                SADJ1.Rows.Add(InsertEar1);

            //            }
            //        }
            //    }
            //}

            //if (SADJh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    if (this.dbaccess.DataSet.Tables["HSAM"].Rows.Count > 0)
            //    {
            //        DataTable hsam = this.dbaccess.DataSet.Tables["HSAM"];

            //        foreach (DataRow dr2 in hsam.Rows)
            //        {
            //            if (dr2.RowState != DataRowState.Deleted)
            //            {
            //                DataRow InsertEar2 = SADJ2.NewRow();
            //                InsertEar2["hsamcode"] = dr2["hsamcode"];                            
            //                SADJ2.Rows.Add(InsertEar2);

            //            }
            //        }
            //    }
            //}

            
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


            SADJh_trandate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_trandate") as DateTimePicker;
            SADJh_reqeffectivedate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_reqeffectivedate") as DateTimePicker;
            SADJh_trandate.Enabled = false;
            SADJh_reqeffectivedate.Enabled = false;

            rad_Recd.CheckedChanged +=new EventHandler(rad_Recd_CheckedChanged);
            rad_NotRecd.CheckedChanged +=new EventHandler(rad_NotRecd_CheckedChanged);
            rad_Appr.CheckedChanged +=new EventHandler(rad_Appr_CheckedChanged);
            rad_NotAppr.CheckedChanged +=new EventHandler(rad_NotAppr_CheckedChanged);

        

            SADJh_appraisedcomments.TextChanged += new EventHandler(SADJh_appraisedcomments_TextChanged);
            SADJh_approvedcomments.TextChanged +=new EventHandler(SADJh_approvedcomments_TextChanged);


            qas_ttscore = BizXmlReader.CurrentInstance.GetControl(qaFormName, "qas_ttscore") as TextBox;
            qas_ttfactor = BizXmlReader.CurrentInstance.GetControl(qaFormName, "qas_ttfactor") as TextBox;
            qas_ttavgfactor = BizXmlReader.CurrentInstance.GetControl(qaFormName, "qas_ttavgfactor") as TextBox;

            if (flag.ToUpper().Trim() == "SADJ")
            {
                txt_psalary = BizXmlReader.CurrentInstance.GetControl(salaryFormName, "txt_psalary") as TextBox;
                txt_csalary = BizXmlReader.CurrentInstance.GetControl(salaryFormName, "txt_csalary") as TextBox;

                SADJh_oldpaytypecode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_oldpaytypecode") as ComboBox;
                SADJh_oldpaytypecode.DropDown += new EventHandler(SADJh_oldpaytypecode_DropDown);
                SADJh_oldpaytypecode.Enabled = false;

                SADJh_paytypecode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "SADJh_paytypecode") as ComboBox;
                SADJh_paytypecode.DropDown += new EventHandler(SADJh_paytypecode_DropDown);
            }
            



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

        void SADJh_paytypecode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from PAYTM where [status]<>'V'";
            this.dbaccess.ReadSQL("PAYTM", sql1);
            DataRow drInsertPAYTM = this.dbaccess.DataSet.Tables["PAYTM"].NewRow();

            drInsertPAYTM["paytypecode"] = "NIL";

            this.dbaccess.DataSet.Tables["PAYTM"].Rows.Add(drInsertPAYTM);

            SADJh_paytypecode.DataSource = this.dbaccess.DataSet.Tables["PAYTM"];
            SADJh_paytypecode.DisplayMember = this.dbaccess.DataSet.Tables["PAYTM"].Columns["Desc"].ColumnName.ToString();
            SADJh_paytypecode.ValueMember = this.dbaccess.DataSet.Tables["PAYTM"].Columns["paytypecode"].ColumnName.ToString();
        }

        void SADJh_oldpaytypecode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from PAYTM where [status]<>'V'";
            this.dbaccess.ReadSQL("PAYTM", sql1);
            DataRow drInsertPAYTM = this.dbaccess.DataSet.Tables["PAYTM"].NewRow();

            drInsertPAYTM["paytypecode"] = "NIL";

            this.dbaccess.DataSet.Tables["PAYTM"].Rows.Add(drInsertPAYTM);

            SADJh_oldpaytypecode.DataSource = this.dbaccess.DataSet.Tables["PAYTM"];
            SADJh_oldpaytypecode.DisplayMember = this.dbaccess.DataSet.Tables["PAYTM"].Columns["Desc"].ColumnName.ToString();
            SADJh_oldpaytypecode.ValueMember = this.dbaccess.DataSet.Tables["PAYTM"].Columns["paytypecode"].ColumnName.ToString();
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
                        SADJh["nric"] = dr1["nric"];
                        SADJh["empnum"] = dr1["empnum"];
                        SADJh["empname"] = dr1["empname"];
                        SADJh["matnum"] = dr1["matnum"];
                        SADJh["sitenumsector"] = dr1["sitenum"] + " / " + dr1["sectorcode"];
                        SADJh["employmentdate"] = dr1["datejoined"];
                        SADJh["currentsal"] = dr1["currentsalary"];
                        SADJh["sectorcode"] = dr1["sectorcode"];
                        SADJh["newsitenum"] = dr1["sitenum"];
                        SADJh["oldpaytypecode"] = dr1["paytypecode"];

                    }
                   

                        string getTiming = "SELECT " +
                                              "[MonToFriTimeFrom] " +
                                              ",[MonToFriTimeTo] " +
                                              ",[MonToFriLunchHr] " +
                                              ",[MonToFriTeaBrkHr] " +
                                              ",[SatTimeFrom] " +
                                              ",[SatTimeTo] " +
                                              ",[SatLunchHr] " +
                                              ",[SatTeaBrkHr] " +
                                              ",[SunTimeFrom] " +
                                              ",[SunTimeTo] " +
                                              ",[SunLunchHr] " +
                                              ",[SunTeaBrkHr] " +
                                              ",[RegularOffDay] " +
                                              ",[PHTimeFrom] " +
                                              ",[PHTimeTo] " +
                                              ",[PHLunchHr] " +
                                              ",[PHTeaBrkHr] " +
                                          "FROM HEMPH WHERE EMPNUM='" + SADJh["empnum"].ToString() + "'";

                        this.dbaccess.ReadSQL("getTimingTB", getTiming);

                        DataTable getTimingTB = this.dbaccess.DataSet.Tables["getTimingTB"];

                        if (getTimingTB != null)
                        {
                            if (getTimingTB.Rows.Count > 0)
                            {
                                DataRow dr1 = this.dbaccess.DataSet.Tables["getTimingTB"].Rows[0];

                                SADJh["oldMonToFriTimeFrom"] = dr1["MonToFriTimeFrom"];
                                SADJh["oldMonToFriTimeTo"] = dr1["MonToFriTimeTo"];
                                SADJh["oldMonToFriLunchHr"] = dr1["MonToFriLunchHr"];
                                SADJh["oldMonToFriTeaBrkHr"] = dr1["MonToFriTeaBrkHr"];
                                SADJh["oldSatTimeFrom"] = dr1["SatTimeFrom"];
                                SADJh["oldSatTimeTo"] = dr1["SatTimeTo"];
                                SADJh["oldSatLunchHr"] = dr1["SatLunchHr"];
                                SADJh["oldSatTeaBrkHr"] = dr1["SatTeaBrkHr"];
                                SADJh["oldSunTimeFrom"] = dr1["SunTimeFrom"];
                                SADJh["oldSunTimeTo"] = dr1["SunTimeTo"];
                                SADJh["oldSunLunchHr"] = dr1["SunLunchHr"];
                                SADJh["oldSunTeaBrkHr"] = dr1["SunTeaBrkHr"];
                                SADJh["oldRegularOffDay"] = dr1["RegularOffDay"];
                                SADJh["oldPHTimeFrom"] = dr1["PHTimeFrom"];
                                SADJh["oldPHTimeTo"] = dr1["PHTimeTo"];
                                SADJh["oldPHLunchHr"] = dr1["PHLunchHr"];
                                SADJh["oldPHTeaBrkHr"] = dr1["PHTeaBrkHr"];


                            }
                        }


                        GetCurrentSalary();


                        //if (SADJh["flag"].ToString().ToUpper().Trim() == "APPR")
                        //{
                            GetPreviousAppraisalInfo(SADJh["empnum"].ToString());
                        //}
                        //else
                        //{
                        //}
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
            //string sql1 = "Select * from SADJM where status<>'V'";
            //this.dbaccess.ReadSQL("SADJM", sql1);
            //SADJh_SADJmcode.DataSource = this.dbaccess.DataSet.Tables["SADJM"];
            //SADJh_SADJmcode.DisplayMember = this.dbaccess.DataSet.Tables["SADJM"].Columns["SADJmcode"].ColumnName.ToString();
            //SADJh_SADJmcode.ValueMember = this.dbaccess.DataSet.Tables["SADJM"].Columns["SADJmcode"].ColumnName.ToString();
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

        private void Voucher_SADJH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable SADJH = this.dbaccess.DataSet.Tables["SADJH"];
            switch (e.Column.ColumnName)
            {
               


            }
        }

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
                //case "rateamt":
                //    {
                //        GetTotalSalary();
                //    }
                //    break;


            }
        }

        #endregion

        #region GetTotalScore

        private void GetTotalScore()
        {
            DataRow dr = dbaccess.DataSet.Tables["SADJh"].Rows[0];
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            decimal ttscore=0,ttmaxscore = 0;
            int ttfactors = 0;
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
                        if (BizFunctions.IsEmpty(dr1["maxscore"]))
                        {
                            dr1["maxscore"] = 0;
                        }
                        ttscore += Convert.ToDecimal(dr1["score"]);
                        ttmaxscore += Convert.ToDecimal(dr1["maxscore"]);
                        ttfactors = ttfactors + 1;
                    }
                }

                qas_ttscore.Text = ttscore.ToString() + "/" + ttmaxscore.ToString();
                dr["ttscore"] = ttscore;

                qas_ttfactor.Text = ttfactors.ToString();
                dr["ttfactor"] = ttfactors;
                

                if (ttfactors > 0 && ttfactors > 0)
                {

                    decimal t = ttscore / ttfactors;
                    decimal s = Math.Round(t, 2, MidpointRounding.ToEven);

                    qas_ttavgfactor.Text = s.ToString();
                    dr["ttavgfactor"] = s;
                }

                dr["mtotalscore"] = ttscore;

                dr["ttmaxscore"] = ttmaxscore;
            }
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
            DataTable sadj1 = this.dbaccess.DataSet.Tables["SADJ1"];

            if (BizFunctions.IsEmpty(SADJh["paytypecode"]) && !BizFunctions.IsEmpty(SADJh["oldpaytypecode"]))
            {
                SADJh["paytypecode"] = SADJh["oldpaytypecode"];
            }

 

            if (SADJh["flag"].ToString().ToUpper().Trim() == "APPH")
            {
                if (BizFunctions.IsEmpty(SADJh["empnum"]))
                {
                    SADJh["empnum"] = Common.DEFAULT_SYSTEM_EMPNUM;
                }

                if (BizFunctions.IsEmpty(SADJh["approvedby"]))
                {
                    SADJh["approvedby"] = GetSupervisor(SADJh["empnum"].ToString());
                }
            }
            else
            {
                if (BizFunctions.IsEmpty(SADJh["approvedby"]))
                {
                    SADJh["approvedby"] = Common.DEFAULT_SYSTEM_USERNAME; ;
                }
            }

            //if (BizFunctions.IsEmpty(SADJh["reqeffectivedate"]))
            //{
            //    MessageBox.Show("Unable to Proceed.\n" +
            //          "Please Fill up the Final Effective Date.",
            //          "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    e.Handle = false;

            //}

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

            //if (BizFunctions.IsEmpty(SADJh["reqeffectivedate"]))
            //{
            //    SADJh["reqeffectivedate"] = SADJh["trandate"];
            //}

            if (!BizFunctions.IsEmpty(SADJh["reqeffectivedate"]))
            {
                if (BizFunctions.IsEmpty(SADJh["newsaleffectivedate"]))
                {
                    SADJh["newsaleffectivedate"] = SADJh["reqeffectivedate"];
                }
            }
        

            if(BizFunctions.IsEmpty(SADJh["newsaleffectivedate"]))
            {
                MessageBox.Show("Please provide the Final Effective Date for the Salary Adjustment", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handle = false;
            }
         
                if (!BizFunctions.IsEmpty(SADJh["MonToFriTimeFrom"]))
                {
                    if (SADJh["MonToFriTimeFrom"].ToString() == String.Empty)
                    {
                        SADJh["MonToFriTimeFrom"] = System.DBNull.Value;
                    }
                }
                else
                {
                    SADJh["MonToFriTimeFrom"] = System.DBNull.Value;
                }


                if (!BizFunctions.IsEmpty(SADJh["MonToFriTimeTo"]))
                {
                    if (SADJh["MonToFriTimeTo"].ToString() == String.Empty)
                    {
                        SADJh["MonToFriTimeTo"] = System.DBNull.Value;
                    }
                }
                else
                {
                    SADJh["MonToFriTimeTo"] = System.DBNull.Value;
                }


                if (!BizFunctions.IsEmpty(SADJh["MonToFriLunchHr"]))
                {
                    if (Convert.ToString(SADJh["MonToFriLunchHr"]) == "0")
                    {
                        SADJh["MonToFriLunchHr"] = System.DBNull.Value;
                    }
                }
                else
                {
                    SADJh["MonToFriLunchHr"] = System.DBNull.Value;
                }


                if (!BizFunctions.IsEmpty(SADJh["MonToFriTeaBrkHr"]))
                {
                    if (Convert.ToString(SADJh["MonToFriTeaBrkHr"]) == "0")
                    {
                        SADJh["MonToFriTeaBrkHr"] = System.DBNull.Value;
                    }
                }
                else
                {
                    SADJh["MonToFriTeaBrkHr"] = System.DBNull.Value;
                }
            

            ///

            if (!BizFunctions.IsEmpty(SADJh["SatTimeFrom"]))
            {
                if (SADJh["SatTimeFrom"].ToString() == String.Empty)
                {
                    SADJh["SatTimeFrom"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SatTimeFrom"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(SADJh["SatTimeTo"]))
            {
                if (SADJh["SatTimeTo"].ToString() == String.Empty)
                {
                    SADJh["SatTimeTo"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SatTimeTo"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(SADJh["SatLunchHr"]))
            {
                if (Convert.ToString(SADJh["SatLunchHr"]) == "0")
                {
                    SADJh["SatLunchHr"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SatLunchHr"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(SADJh["SatTeaBrkHr"]))
            {
                if (Convert.ToString(SADJh["SatTeaBrkHr"]) == "0")
                {
                    SADJh["SatTeaBrkHr"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SatTeaBrkHr"] = System.DBNull.Value;
            }

            ///

            if (!BizFunctions.IsEmpty(SADJh["SunTimeFrom"]))
            {
                if (SADJh["SunTimeFrom"].ToString() == String.Empty)
                {
                    SADJh["SunTimeFrom"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SunTimeFrom"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(SADJh["SunTimeTo"]))
            {
                if (SADJh["SunTimeTo"].ToString() == String.Empty)
                {
                    SADJh["SunTimeTo"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SunTimeTo"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(SADJh["SunLunchHr"]))
            {
                if (Convert.ToString(SADJh["SunLunchHr"]) == "0")
                {
                    SADJh["SunLunchHr"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SunLunchHr"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(SADJh["SunTeaBrkHr"]))
            {
                if (Convert.ToString(SADJh["SunTeaBrkHr"]) == "0")
                {
                    SADJh["SunTeaBrkHr"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["SunTeaBrkHr"] = System.DBNull.Value;
            }

            ///

            if (!BizFunctions.IsEmpty(SADJh["PHTimeFrom"]))
            {
                if (SADJh["PHTimeFrom"].ToString() == String.Empty)
                {
                    SADJh["PHTimeFrom"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["PHTimeFrom"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(SADJh["PHTimeTo"]))
            {
                if (SADJh["PHTimeTo"].ToString() == String.Empty)
                {
                    SADJh["PHTimeTo"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["PHTimeTo"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(SADJh["PHLunchHr"]))
            {
                if (Convert.ToString(SADJh["PHLunchHr"]) == "0")
                {
                    SADJh["PHLunchHr"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["PHLunchHr"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(SADJh["PHTeaBrkHr"]))
            {
                if (Convert.ToString(SADJh["PHTeaBrkHr"]) == "0")
                {
                    SADJh["PHTeaBrkHr"] = System.DBNull.Value;
                }
            }
            else
            {
                SADJh["PHTeaBrkHr"] = System.DBNull.Value;
            }

            ///


            if (BizFunctions.IsEmpty(SADJh["daysperweek"]))
            {
                SADJh["daysperweek"] = SADJh["olddaysperweek"];
            }


        


            //SADJh["oldMonToFriTimeFrom"] = dr1["MonToFriTimeFrom"];
            //SADJh["oldMonToFriTimeTo"] = dr1["MonToFriTimeTo"];
            //SADJh["oldMonToFriLunchHr"] = dr1["MonToFriLunchHr"];
            //SADJh["oldMonToFriTeaBrkHr"] = dr1["MonToFriTeaBrkHr"];
            //SADJh["oldSatTimeFrom"] = dr1["SatTimeFrom"];
            //SADJh["oldSatTimeTo"] = dr1["SatTimeTo"];
            //SADJh["oldSatLunchHr"] = dr1["SatLunchHr"];
            //SADJh["oldSatTeaBrkHr"] = dr1["SatTeaBrkHr"];
            //SADJh["oldSunTimeFrom"] = dr1["SunTimeFrom"];
            //SADJh["oldSunTimeTo"] = dr1["SunTimeTo"];
            //SADJh["oldSunLunchHr"] = dr1["SunLunchHr"];
            //SADJh["oldSunTeaBrkHr"] = dr1["SunTeaBrkHr"];
            //SADJh["oldRegularOffDay"] = dr1["RegularOffDay"];
            //SADJh["oldPHTimeFrom"] = dr1["PHTimeFrom"];
            //SADJh["oldPHTimeTo"] = dr1["PHTimeTo"];
            //SADJh["oldPHLunchHr"] = dr1["PHLunchHr"];
            //SADJh["oldPHTeaBrkHr"] = dr1["PHTeaBrkHr"];

            //Timing

            if ((bool)SADJh["t1"])
            {
                if (BizFunctions.IsEmpty(SADJh["MonToFriTimeFrom"]))
                {
                    SADJh["MonToFriTimeFrom"] = SADJh["oldMonToFriTimeFrom"];
                }

                if (BizFunctions.IsEmpty(SADJh["MonToFriTimeTo"]))
                {
                    SADJh["MonToFriTimeTo"] = SADJh["oldMonToFriTimeTo"];
                }

                if (BizFunctions.IsEmpty(SADJh["MonToFriLunchHr"]))
                {
                    SADJh["MonToFriLunchHr"] = SADJh["oldMonToFriLunchHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["MonToFriTeaBrkHr"]))
                {
                    SADJh["MonToFriTeaBrkHr"] = SADJh["oldMonToFriTeaBrkHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["newMonToFriShiftCode"]))
                {
                    SADJh["newMonToFriShiftCode"] = SADJh["MonToFriShiftCode"];
                }
            }

            if ((bool)SADJh["t2"])
            {

                if (BizFunctions.IsEmpty(SADJh["SatTimeFrom"]))
                {
                    SADJh["SatTimeFrom"] = SADJh["oldSatTimeFrom"];
                }

                if (BizFunctions.IsEmpty(SADJh["SatTimeTo"]))
                {
                    SADJh["SatTimeTo"] = SADJh["oldSatTimeTo"];
                }

                if (BizFunctions.IsEmpty(SADJh["SatLunchHr"]))
                {
                    SADJh["SatLunchHr"] = SADJh["oldSatLunchHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["SatTeaBrkHr"]))
                {
                    SADJh["SatTeaBrkHr"] = SADJh["oldSatTeaBrkHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["newSatShiftCode"]))
                {
                    SADJh["newSatShiftCode"] = SADJh["SatShiftCode"];
                }
            }

            if ((bool)SADJh["t3"])
            {
                if (BizFunctions.IsEmpty(SADJh["SunTimeFrom"]))
                {
                    SADJh["SunTimeFrom"] = SADJh["oldSunTimeFrom"];
                }

                if (BizFunctions.IsEmpty(SADJh["SunTimeTo"]))
                {
                    SADJh["SunTimeTo"] = SADJh["oldSunTimeTo"];
                }

                if (BizFunctions.IsEmpty(SADJh["SunLunchHr"]))
                {
                    SADJh["SunLunchHr"] = SADJh["oldSunLunchHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["SunTeaBrkHr"]))
                {
                    SADJh["SunTeaBrkHr"] = SADJh["oldSunTeaBrkHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["newSunShiftCode"]))
                {
                    SADJh["newSunShiftCode"] = SADJh["SunShiftCode"];
                }
            }

            if ((bool)SADJh["t5"])
            {
                if (BizFunctions.IsEmpty(SADJh["RegularOffDay"]))
                {
                    SADJh["RegularOffDay"] = SADJh["oldRegularOffDay"];
                }
            }

            if ((bool)SADJh["t4"])
            {

                if (BizFunctions.IsEmpty(SADJh["PHTimeFrom"]))
                {
                    SADJh["PHTimeFrom"] = SADJh["oldPHTimeFrom"];
                }

                if (BizFunctions.IsEmpty(SADJh["PHTimeTo"]))
                {
                    SADJh["PHTimeTo"] = SADJh["oldPHTimeTo"];
                }

                if (BizFunctions.IsEmpty(SADJh["PHLunchHr"]))
                {
                    SADJh["PHLunchHr"] = SADJh["oldPHLunchHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["PHTeaBrkHr"]))
                {
                    SADJh["PHTeaBrkHr"] = SADJh["oldPHTeaBrkHr"];
                }

                if (BizFunctions.IsEmpty(SADJh["newPHShiftCode"]))
                {
                    SADJh["newPHShiftCode"] = SADJh["PHShiftCode"];
                }
            }



            if (!BizFunctions.IsEmpty(SADJh["matnum"]))
            {
                if (BizFunctions.IsEmpty(SADJh["newmatnum"]))
                {
                    SADJh["newmatnum"] = SADJh["matnum"].ToString();
                }
            }

            if (sadj1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in sadj1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["scoregroup"]))
                        {
                            dr1["scoregroup"] = GetScoreGroup(dr1["gpfmcode"].ToString().Trim(), Convert.ToInt32(dr1["performFactorNo"]), dr1["groupcode"].ToString());
                        }
                    }
                }
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

                case "SADJh_newmatnum":
                    {

                        e.DefaultCondition = " flag ='MATM' ";

                    }
                    break;


                case "SADJh_sitenumt":
                    {
                        if (!BizFunctions.IsEmpty(sadjh["newsitenum"]))
                        {
                            e.DefaultCondition = " sitenum='" + sadjh["newsitenum"].ToString().Trim() + "' ";
                        }
                        else
                        {
                            e.Condition = BizFunctions.F2Condition("sitenumt", (sender as TextBox).Text);
                        }
                       

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
                            e.CurrentRow["daysperweek"] = e.F2CurrentRow["daysperweek"];
                            e.CurrentRow["oldpaytypecode"] = e.F2CurrentRow["paytypecode"];
                            
                            e.CurrentRow["sitenumsector"] = e.F2CurrentRow["sitenum"].ToString()+"/"+e.F2CurrentRow["sectorcode"].ToString();

                            e.CurrentRow["newsitenum"] = e.F2CurrentRow["sitenum"];

                            string getTiming = "SELECT "+
                                                  "[MonToFriTimeFrom] "+
                                                  ",[MonToFriTimeTo] "+
                                                  ",[MonToFriLunchHr] "+
                                                  ",[MonToFriTeaBrkHr] "+
                                                  ",[SatTimeFrom] "+
                                                  ",[SatTimeTo] "+
                                                  ",[SatLunchHr] "+
                                                  ",[SatTeaBrkHr] "+
                                                  ",[SunTimeFrom] "+
                                                  ",[SunTimeTo] "+
                                                  ",[SunLunchHr] "+
                                                  ",[SunTeaBrkHr] "+
                                                  ",[RegularOffDay] "+
                                                  ",[PHTimeFrom] "+
                                                  ",[PHTimeTo] "+
                                                  ",[PHLunchHr] "+
                                                  ",[PHTeaBrkHr] "+
                                                  ",[MonToFriShiftCode]" +
                                                  ",[SatShiftCode]" +
                                                  ",[SunShiftCode]" +
                                                  ",[PHShiftCode]" +
                                              "FROM HEMPH WHERE EMPNUM='" + SADJh["empnum"].ToString() + "'";

                            this.dbaccess.ReadSQL("getTimingTB", getTiming);

                            DataTable getTimingTB = this.dbaccess.DataSet.Tables["getTimingTB"];

                            if (getTimingTB != null)
                            {
                                if (getTimingTB.Rows.Count > 0)
                                {
                                    DataRow dr1 = this.dbaccess.DataSet.Tables["getTimingTB"].Rows[0];

                                    SADJh["oldMonToFriTimeFrom"] = dr1["MonToFriTimeFrom"];
                                    SADJh["oldMonToFriTimeTo"] = dr1["MonToFriTimeTo"];
                                    SADJh["oldMonToFriLunchHr"] = dr1["MonToFriLunchHr"];
                                    SADJh["oldMonToFriTeaBrkHr"] = dr1["MonToFriTeaBrkHr"];
                                    SADJh["oldSatTimeFrom"] = dr1["SatTimeFrom"];
                                    SADJh["oldSatTimeTo"] = dr1["SatTimeTo"];
                                    SADJh["oldSatLunchHr"] = dr1["SatLunchHr"];
                                    SADJh["oldSatTeaBrkHr"] = dr1["SatTeaBrkHr"];
                                    SADJh["oldSunTimeFrom"] = dr1["SunTimeFrom"];
                                    SADJh["oldSunTimeTo"] = dr1["SunTimeTo"];
                                    SADJh["oldSunLunchHr"] = dr1["SunLunchHr"];
                                    SADJh["oldSunTeaBrkHr"] = dr1["SunTeaBrkHr"];
                                    SADJh["oldRegularOffDay"] = dr1["RegularOffDay"];
                                    SADJh["oldPHTimeFrom"] = dr1["PHTimeFrom"];
                                    SADJh["oldPHTimeTo"] = dr1["PHTimeTo"];
                                    SADJh["oldPHLunchHr"] = dr1["PHLunchHr"];
                                    SADJh["oldPHTeaBrkHr"] = dr1["PHTeaBrkHr"];

                                    SADJh["MonToFriShiftCode"] = dr1["MonToFriShiftCode"];
                                    SADJh["SatShiftCode"] = dr1["SatShiftCode"];
                                    SADJh["SunShiftCode"] = dr1["SunShiftCode"];
                                    SADJh["PHShiftCode"] = dr1["PHShiftCode"];


                                }
                            }
                            GetCurrentSalary();

                        }
                    }
                    break;

                case "SADJh_gpfmcode":
                    {
                        if (!BizFunctions.IsEmpty(SADJh["empnum"]))
                        {
                            getAppraisalTemplate();
                        }
                    }
                    break;

                case "SADJh_newMonToFriShiftCode":
                    e.CurrentRow["MonToFriTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["MonToFriTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;

                case "SADJh_newSatShiftCode":
                    e.CurrentRow["SatTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["SatTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;

                case "SADJh_newSunShiftCode":
                    e.CurrentRow["SunTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["SunTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;

                case "SADJh_newPHShiftCode":
                    e.CurrentRow["PHTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["PHTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;

                case "SADJh_MonToFriShiftCode":
                    e.CurrentRow["oldMonToFriTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["oldMonToFriTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;

                case "SADJh_SatShiftCode":
                    e.CurrentRow["oldSatTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["oldSatTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;

                case "SADJh_SunShiftCode":
                    e.CurrentRow["oldSunTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["oldSunTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;

                case "SADJh_PHShiftCode":
                    e.CurrentRow["oldPHTimeFrom"] = e.F2CurrentRow["Timein"];
                    e.CurrentRow["oldPHTimeTo"] = e.F2CurrentRow["Timeout"];
                    break;
                           
            }
        }

        #endregion

        #region Get Hemph Info

        private void GetHemphInfo(string empnum)
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];
            DataTable SADJ3 = this.dbaccess.DataSet.Tables["SADJ3"];
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
            DataTable SADJ3 = this.dbaccess.DataSet.Tables["SADJ3"];
            
            DataRow dr1,dr2=null;
            string sql ="";
            if (SADJh["flag"].ToString().Trim() != "APPR")
            {

                sql = " Select * From  " +
                               "(  " +
                                   "Select  " +
                                   "ROW_NUMBER() OVER (Order BY newsaleffectivedate) as ForTop,ROW_NUMBER() OVER (Order BY newsaleffectivedate Desc) as ForBottom, " +
                                   "refnum,empnum,newsaleffectivedate,newsal, approvedcomments,trandate,isapproved  " +
                                   "from  " +
                                   "SADJH where empnum='" + empnum + "' and status='P' and flag<>'APPR' " +
                               ")A  " +
                               "Where ForBottom <=2 ";
            }
            else
            {
                sql = " Select * From  " +
                               "(  " +
                                   "Select  " +
                                   "ROW_NUMBER() OVER (Order BY newsaleffectivedate) as ForTop,ROW_NUMBER() OVER (Order BY newsaleffectivedate Desc) as ForBottom, " +
                                   "refnum,empnum,newsaleffectivedate,newsal, approvedcomments,trandate,isapproved  " +
                                   "from  " +
                                   "SADJH where empnum='" + empnum + "' and status='P' and flag='APPR'" +
                               ")A  " +
                               "Where ForBottom <=2 ";
            }

            this.dbaccess.ReadSQL("dtSADJH", sql);
            if (this.dbaccess.DataSet.Tables["dtSADJH"].Rows.Count > 0)
            {
                
                    dr1 = this.dbaccess.DataSet.Tables["dtSADJH"].Rows[0];
                    DateTime dt1 = DateTime.Now;
                    if(BizFunctions.IsEmpty(dr1["newsaleffectivedate"]))
                    {
                        dr1["newsaleffectivedate"] = dt1;
                    }
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
                            if (BizFunctions.IsEmpty(dr1["newsal"]))
                            {
                                dr1["newsal"] = 0;
                            }
                            //SADJh["currentsal"] = Convert.ToDecimal(dr1["newsal"]);
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
                            else
                            {
                                if (SADJ3.Rows.Count > 0)
                                {
                                    if (SADJ2.Rows.Count > 0)
                                    {
                                        BizFunctions.DeleteAllRows(SADJ2);
                                    }
                                    foreach (DataRow dr3 in SADJ3.Rows)
                                    {
                                        if (dr3.RowState != DataRowState.Deleted)
                                        {
                                            DataRow drInsertSADJ2 = SADJ2.NewRow();
                                            drInsertSADJ2["hsamcode"] = dr3["hsamcode"];
                                            drInsertSADJ2["rateamt"] = dr3["rateamt"];
                                            drInsertSADJ2["line"] = dr3["line"];
                                            SADJ2.Rows.Add(drInsertSADJ2);

                                        }
                                    }
                                }
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

        private void GetProposedSalary()
        {
            //DataRow sadjh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];

            //string GetSADJ2 = "Select * from SADJ2 where refnum='" + dr1["refnum"].ToString() + "' order by myline";
            //this.dbaccess.ReadSQL("dtSADJ2", GetSADJ2);

            //if (this.dbaccess.DataSet.Tables["dtSADJ2"].Rows.Count > 0)
            //{
            //    BizFunctions.DeleteAllRows(SADJ2);
            //    foreach (DataRow dr3 in this.dbaccess.DataSet.Tables["dtSADJ2"].Rows)
            //    {
            //        if (dr3.RowState != DataRowState.Deleted)
            //        {
            //            DataRow InsertEar2 = SADJ2.NewRow();
            //            InsertEar2["hsamcode"] = dr3["hsamcode"];
            //            InsertEar2["rateamt"] = dr3["rateamt"];
            //            InsertEar2["line"] = dr3["line"];
            //            SADJ2.Rows.Add(InsertEar2);
            //        }

            //    }
            //    GetTotalSalary();
            //}
        }

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
            DataTable SADJ3 = this.dbaccess.DataSet.Tables["SADJ3"];



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
            
            
            #region SADJ3
            foreach (DataRow dr3 in SADJ3.Rows)
            {
                if (dr3.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(SADJh, dr3, "empnum/refnum/user/flag/status/created/modified");
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
            string strsadj_temp = "Select * from sadjh_temp where 1=2";
            this.dbaccess.ReadSQL("SALH", strsalh);
            this.dbaccess.ReadSQL("PFMSR", strpfmsr);
            this.dbaccess.ReadSQL("sadjh_temp", strsadj_temp);

            

            DataTable salh = this.dbaccess.DataSet.Tables["SALH"];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];
            DataTable sadjh_temp = this.dbaccess.DataSet.Tables["sadjh_temp"];

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

                if (!BizFunctions.IsEmpty(SADJh["sadjmcode"]))
                {
                    if (SADJh["sadjmcode"].ToString().Trim().ToUpper() == "STAFFTRANSFER" || SADJh["sadjmcode"].ToString().Trim().ToUpper() == "TRANSFER")
                    {

                        DateTime dt1 = Convert.ToDateTime(Convert.ToDateTime(SADJh["newsaleffectivedate"]).ToShortDateString());
                        if (dt1 >= DateTime.Today)
                        {
                            //sadjh_temp.ImportRow(SADJh);
                            //refnum,nric,empnum,empname,sitenum,newsitenum, transferupdateStatus,newsaleffectivedate
                            DataRow sadjh_tempDr = sadjh_temp.NewRow();
                            sadjh_tempDr["refnum"] = SADJh["refnum"];
                            sadjh_tempDr["empnum"] = SADJh["empnum"];
                            sadjh_tempDr["empname"] = SADJh["empname"];
                            sadjh_tempDr["trandate"] = SADJh["trandate"];
                            sadjh_tempDr["newsaleffectivedate"] = SADJh["newsaleffectivedate"];
                            sadjh_tempDr["nric"] = SADJh["nric"];
                            sadjh_tempDr["sadjmcode"] = SADJh["sadjmcode"];
                            sadjh_tempDr["sitenum"] = SADJh["sitenum"];
                            sadjh_tempDr["newsitenum"] = SADJh["newsitenum"];
                            sadjh_tempDr["transferupdateStatus"] = SADJh["transferupdateStatus"];
                            sadjh_tempDr["sitenumt"] = SADJh["sitenumt"];
                            sadjh_tempDr["newMonToFriShiftCode"] = SADJh["newMonToFriShiftCode"];
                            sadjh_tempDr["newMonToFriShiftCode"] = SADJh["newMonToFriShiftCode"];
                            sadjh_tempDr["newMonToFriShiftCode"] = SADJh["newMonToFriShiftCode"];
                            sadjh_tempDr["newMonToFriShiftCode"] = SADJh["newMonToFriShiftCode"];
                            sadjh_tempDr["newMonToFriShiftCode"] = SADJh["newMonToFriShiftCode"];
                            sadjh_tempDr["newSatShiftCode"] = SADJh["newSatShiftCode"];
                            sadjh_tempDr["newSunShiftCode"] = SADJh["newSunShiftCode"];
                            sadjh_tempDr["newPHShiftCode"] = SADJh["newPHShiftCode"];

                            sadjh_temp.Rows.Add(sadjh_tempDr);



                        }
                    }
                }

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


                        #region Assign ids to sadjh_temp table for saving

                        string maxsadjh_tempid = "SELECT ISNULL(MAX(id),0) AS 'id' FROM sadjh_temp";
                        DataSet maxsadjh_tmp = this.dbaccess.ReadSQLTemp("idsadjh_temp", maxsadjh_tempid);

                        int c = 0;
                        if (maxsadjh_tmp.Tables["idsadjh_temp"].Rows.Count > 0)
                        {
                            if (maxsadjh_tmp.Tables["idsadjh_temp"].Rows[0]["id"] == System.DBNull.Value)
                                c = 0;
                            else
                                c = Convert.ToInt32(maxsadjh_tmp.Tables["idsadjh_temp"].Rows[0]["id"]) + 1;
                        }
                        else
                        {
                            c = 0;
                        }

                        foreach (DataRow dr in sadjh_temp.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                dr["id"] = c;
                                c++;
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

                                if (dataTable.TableName == "sadjh_temp")
                                {
                                    DataTable tempDataTable2 = dataTable.Clone();

                                    if (tempDataTable2.Columns.Contains("mark"))
                                        tempDataTable2.Columns.Remove("mark");
                                }
                            }


                            Hashtable tablesCollections = new Hashtable();
                            foreach (DataTable dataTable in this.dbaccess.DataSet.Tables)
                            {
                                tablesCollections[dataTable.TableName] = dataTable.TableName;
                            }

                            DataTable[] dataTablestemp = new DataTable[3];
                            dataTablestemp[0] = salh;
                            dataTablestemp[0].TableName = salh.TableName.ToString();
                            dataTablestemp[1] = pfmsr;
                            dataTablestemp[1].TableName = pfmsr.TableName.ToString();
                            dataTablestemp[2] = sadjh_temp;
                            dataTablestemp[2].TableName = sadjh_temp.TableName.ToString();



                            // Delete this current refnum first.	
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM salh WHERE refnum = '" + SADJh["refnum"].ToString() + "'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM pfmsr WHERE empnum = '" + SADJh["empnum"].ToString() + "'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM sadjh_temp WHERE refnum = '" + SADJh["refnum"].ToString() + "'");

                            this.dbaccess.Update(dataTablestemp);


                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from salh) where tablename = 'SALH'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from pfmsr) where tablename = 'pfmsr'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from sadjh_temp) where tablename = 'sadjh_temp'");

                            //if (!BizFunctions.IsEmpty(SADJh["SADJmcode"]))
                            {
                                if (SADJh["SADJmcode"].ToString().Trim().ToUpper() == "REJOIN")
                                {
                                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH SET [STATUSCODE]='REJOIN' WHERE EMPNUM='" + SADJh["empnum"].ToString() + "' ");
                                }
                            }

                            //if (!BizFunctions.IsEmpty(SADJh["newsitenum"]))
                            //{

                            //    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set sitenum = '" +
                            //                                                    SADJh["newsitenum"].ToString() +
                            //                                                    "' where empnum='" +
                            //                                                    SADJh["empnum"].ToString() + "' ");


                            //    string newSector = BizLogicTools.Tools.GetSectorCode(SADJh["newsitenum"].ToString(),
                            //                                                         this.dbaccess);

                            //    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set sectorcode = '" +
                            //                                                    newSector + "' where empnum='" +
                            //                                                    SADJh["empnum"].ToString() + "' ");


                            //    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set daysperweek = " +
                            //                                                 SADJh["daysperweek"].ToString() + " where empnum='" +
                            //                                                  SADJh["empnum"].ToString() + "' ");

                            //}

                            //if (!BizFunctions.IsEmpty(SADJh["newmatnum"]))
                            //{

                            //    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set matnum = '" +
                            //                                                    SADJh["newmatnum"].ToString() +
                            //                                                    "' where empnum='" +
                            //                                                    SADJh["empnum"].ToString() + "' ");


                            //}


                            //    string UpdateHemphTiming = "UPDATE HEMPH SET "+
                            //                                   "[MonToFriTimeFrom]='" + SADJh["MonToFriTimeFrom"].ToString() + "' " +
                            //                                  ",[MonToFriTimeTo]='" + SADJh["MonToFriTimeTo"].ToString() + "' " +
                            //                                  ",[MonToFriLunchHr]=" + SADJh["MonToFriLunchHr"].ToString() + " " +
                            //                                  ",[MonToFriTeaBrkHr]=" + SADJh["MonToFriTeaBrkHr"].ToString() + " " +
                            //                                  ",[SatTimeFrom]='" + SADJh["SatTimeFrom"].ToString() + "' " +
                            //                                  ",[SatTimeTo]='" + SADJh["SatTimeTo"].ToString() + "' " +
                            //                                  ",[SatLunchHr]=" + SADJh["SatLunchHr"].ToString() + " " +
                            //                                  ",[SatTeaBrkHr]=" + SADJh["SatTeaBrkHr"].ToString() + " " +
                            //                                  ",[SunTimeFrom]='" + SADJh["SunTimeFrom"].ToString() + "' " +
                            //                                  ",[SunTimeTo]='" + SADJh["SunTimeTo"].ToString() + "' " +
                            //                                  ",[SunLunchHr]=" + SADJh["SunLunchHr"].ToString() + " " +
                            //                                  ",[SunTeaBrkHr]=" + SADJh["SunTeaBrkHr"].ToString() + " " +
                            //                                  ",[PHTimeFrom]='" + SADJh["PHTimeFrom"].ToString() + "' " +
                            //                                  ",[PHTimeTo]='" + SADJh["PHTimeTo"].ToString() + "' " +
                            //                                  ",[PHLunchHr]=" + SADJh["PHLunchHr"].ToString() + " " +
                            //                                  ",[PHTeaBrkHr]=" + SADJh["PHTeaBrkHr"].ToString() + " " +
                            //                                  ",[RegularOffDay]='" + SADJh["RegularOffDay"].ToString() + "' " +
                            //                             "WHERE EMPNUM='" + SADJh["empnum"].ToString() + "' ";

                            //    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemphTiming);

                   


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


                if (!BizFunctions.IsEmpty(SADJh["sadjmcode"]))
                {
                    if (SADJh["sadjmcode"].ToString().Trim().ToUpper() == "STAFFTRANSFER" || SADJh["sadjmcode"].ToString().Trim().ToUpper() == "TRANSFER")
                    {

                        DateTime dt1 = Convert.ToDateTime(Convert.ToDateTime(SADJh["newsaleffectivedate"]).ToShortDateString());
                        if (dt1 <= DateTime.Today)
                        {


                            // Delete this current refnum first.	
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM SITMT8 WHERE empnum = '" + SADJh["empnum"].ToString() + "'");

                            int GetMaxID = BizLogicTools.Tools.getMaxID("SITMT8", this.dbaccess);

                            string getSiteTemplate = "";
                            if (!BizFunctions.IsEmpty(SADJh["sitenumt"]))
                            {
                                getSiteTemplate = "Select * from sitmt where sitenum='" + SADJh["newsitenum"].ToString() + "' and sitenumt='" + SADJh["sitenumt"].ToString().Trim() + "' and [status]<>'V' ";
                            }
                            else
                            {
                                getSiteTemplate = "Select * from sitmt where sitenum='" + SADJh["newsitenum"].ToString() + "' and [status]<>'V' ";
                            }


                            this.dbaccess.ReadSQL("TempSitmtMain", getSiteTemplate);

                            DataTable TempSitmtMain = this.dbaccess.DataSet.Tables["TempSitmtMain"];

                            if (TempSitmtMain.Rows.Count > 0)
                            {
                                string InsertEmp = " INSERT INTO SITMT8 " +
                                                   "( " +
                                                   "id " +
                                                   ",empnum " +
                                                   ",empname " +
                                                   ",matnum " +
                                                   ",sitenum " +
                                                   ",sitenumt " +
                                                   ",ctrnum " +
                                                   ",remark " +
                                                   ",xday1 " +
                                                   ",xday2 " +
                                                   ",xday3 " +
                                                   ",xday4 " +
                                                   ",xday5 " +
                                                   ",xday6 " +
                                                   ",xday7 " +
                                                   ",ispubhol " +
                                                   ",[guid] " +
                                                   ",[status] " +
                                                   ",created" +
                                                   ",modified " +
                                                   ",flag " +
                                                   ",[user] " +
                                                   ") " +
                                                   "VALUES " +
                                                   "( " +
                                                   " " + Convert.ToString(GetMaxID + 1) + " " +
                                                    ",'" + SADJh["empnum"].ToString() + "' " +
                                                    ",'" + BizLogicTools.Tools.GetEmpname(SADJh["empnum"].ToString(), this.dbaccess) + "' " +
                                                    ",'" + SADJh["newmatnum"].ToString() + "' " +
                                                   ",'" + TempSitmtMain.Rows[0]["sitenum"].ToString() + "' " +
                                                   ",'" + TempSitmtMain.Rows[0]["sitenumt"].ToString() + "' " +
                                                   ",'" + TempSitmtMain.Rows[0]["ctrnum"].ToString() + "' " +
                                                   ",'TRANFER - " + SADJh["refnum"].ToString() + " on " + SADJh["trandate"].ToString() + "' " +
                                                   ",'" + SADJh["newMonToFriShiftCode"].ToString() + "' " +
                                                   ",'" + SADJh["newMonToFriShiftCode"].ToString() + "' " +
                                                   ",'" + SADJh["newMonToFriShiftCode"].ToString() + "' " +
                                                   ",'" + SADJh["newMonToFriShiftCode"].ToString() + "' " +
                                                   ",'" + SADJh["newMonToFriShiftCode"].ToString() + "' " +
                                                   ",'" + SADJh["newSatShiftCode"].ToString() + "' " +
                                                   ",'" + SADJh["newSunShiftCode"].ToString() + "' " +
                                                   ",'" + SADJh["newPHShiftCode"].ToString() + "' " +
                                                   ",'" + BizLogicTools.Tools.getGUID() + "' " +
                                                   ",'O' " +
                                                   ",GETDATE() " +
                                                   ",GETDATE() " +
                                                   ",'SITMT' " +
                                                   ",'" + Common.DEFAULT_SYSTEM_USERNAME + "' " +
                                                   ") ";
                                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(InsertEmp);

                                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SYSID SET LASTID=(SELECT ISNULL(MAX(ID),0) FROM SITMT8) WHERE TABLENAME='SITMT8'");

                            }


                            ///
                        }
                    }

                }

            }
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow sadjh = e.DBAccess.DataSet.Tables["sadjh"].Rows[0];
            DataTable sadj2 = e.DBAccess.DataSet.Tables["sadj2"];
            DataTable sadj3 = e.DBAccess.DataSet.Tables["sadj3"];
            GetTotalSalary();


            if (flag.ToUpper().Trim() == "APPH" || flag.ToUpper().Trim() == "APPR")
            {
                GenerateGroupScore();
            }

            GetTotalScore();

            if (flag.ToUpper().Trim() == "SADJ")
            {
                txt_psalary.Text = Convert.ToString(getTotalSalary(sadj2));
                txt_csalary.Text = Convert.ToString(getTotalSalary(sadj3));
            }

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

            DataRow sadjh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];

            switch (e.ReportName)
            {

                //case "Giro Opt-Out Form":

                //    if (hemph["pmcode"].ToString().ToUpper() == "CASH")
                //    {
                //        e.DataSource = GiRods1();
                //    }
                //    else
                //    {

                //        MessageBox.Show("Unable to Preview unless you change the Payment Type to Cash Mode!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        e.ReportSource = null;
                //    }
                //    break;

                case "Salary Revision":
                    e.DataSource = SRds1();

                    if (sadjh["SADJmcode"].ToString().ToUpper() == "SALARYREVISION")
                    {
                        e.DataSource = SRds1();
                    }
                    else
                    {

                        MessageBox.Show("Unable to Preview unless you change the Adjustment Code to Salary Revision!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.ReportSource = null;
                    }
                    break;

                case "Promotion":
                  
                    if (sadjh["SADJmcode"].ToString().ToUpper() == "PROMOTION")
                    {
                        e.DataSource = SRds1();
                    }
                    else
                    {

                        MessageBox.Show("Unable to Preview unless you change the Adjustment Code to Promotion!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.ReportSource = null;
                    }
                    break;

                case "Transfer":
                    
                    if (sadjh["SADJmcode"].ToString().ToUpper() == "STAFFTRANSFER")
                    {
                        e.DataSource = SRds1();
                    }
                    else
                    {

                        MessageBox.Show("Unable to Preview unless you change the Adjustment Code to Staff Transfer!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.ReportSource = null;
                    }
                    break;
  
                case "Performance Appraisal 1":
                    if (sadjh["flag"].ToString().ToUpper().Trim() == "APPR")
                    {
                        e.DataSource = PFds1();
                    }
                    else
                    {

                        MessageBox.Show("Unable to Preview, this Form is for Cleaners only!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.ReportSource = null;
                    }
                    break;
                case "Performance Appraisal 2":
                    if (sadjh["flag"].ToString().ToUpper().Trim() == "APPH")
                    {
                        e.DataSource = PFds1();
                    }
                    else
                    {

                        MessageBox.Show("Unable to Preview, this for is for HQ Staff only!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.ReportSource = null;
                    }
                    break;

            }

        }

        #endregion

        private DataSet SRds1()
        {
            DataSet ds1 = new DataSet("LAds1");

            DataRow sadjh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];


            string str = "Select " +
                            "A.empname, " +
                            "A.nric, " +
                            "A.position, " +
                            "A.sitename, " +
                            "A.datejoined, " +
                            "A.bankacc, " +
                            "A.bankname, " +
                            "A.commencedate, " +
                            "A.contact, " +
                            "A.homecontactno, " +
                            "A.daysperweek, " +
                            "A.paytypecode, " +
                            "A.homeaddress, " +
                            "A.MonToFriTimeFrom, " +
                            "A.MonToFriTimeTo, " +
                            "A.MonToFriLunchHr, " +
                            "A.MonToFriTeaBrkHr, " +
                            "A.SatTimeFrom, " +
                            "A.SatTimeTo, " +
                            "A.SatLunchHr, " +
                            "A.SatTeaBrkHr, " +
                            "A.SunTimeFrom, " +
                            "A.SunTimeTo, " +
                            "A.SunLunchHr, " +
                            "A.SunTeaBrkHr, " +
                            "A.PHTimeFrom, " +
                            "A.PHTimeTo, " +
                            "A.PHLunchHr, " +
                            "A.PHTeaBrkHr, " +
                            "A.RegularOffDay, " +
                            "ISNULL(A.SalaryDeductionPerDay,0) AS SalaryDeductionPerDay, " +
                            "ISNULL(A.OtherDudction,0) AS OtherDudction, " +
                            "ISNULL(A.[Basic],0) AS [BasicSal], " +
                            "ISNULL(A.ATTNALLW,0) AS ATTNALLW, " +
                            "ISNULL(A.ACCOMALLW,0) AS ACCOMALLW, " +
                            "ISNULL(A.DRIVTRANSALLW,0) AS DRIVTRANSALLW, " +
                            "ISNULL(A.OtherAllowance,0) AS OtherAllowance, " +
                            "ISNULL(A.OTALLW,0) AS OTALLW, " +
                            "A.remark, " +
                            "A.NextOfKeen, " +
                            "A.FamilyContact, " +
                            "ISNULL(A.[Basic],0)+ISNULL(A.ATTNALLW,0)+ISNULL(A.ACCOMALLW,0)+ISNULL(A.DRIVTRANSALLW,0)+ISNULL(A.OtherAllowance,0) as TotalSalary " +
                        "from " +
                        "( " +
                            "Select  " +
                                "h.empnum, " +
                                "h.empname, " +
                                "h.nric, " +
                                "h.matnum, " +
                                "M.matname as position, " +
                                "CASE WHEN '" + sadjh["newsitenum"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["newsitenum"].ToString() + "' END as sitenum, " +
                                "CASE WHEN '" + sadjh["newsitenum"].ToString() + "' = '' THEN NULL ELSE dbo.GetSitename('" + sadjh["newsitenum"].ToString() + "') END as sitename, " +
                                //"h.sitenum, " +
                                //"S.sitename, " +
                                "h.datejoined, " +
                                "h.bankacc, " +
                                "h.bankname, " +
                                "h.commencedate, " +
                                "CASE WHEN h.contact IS NULL THEN '-' ELSE h.contact END AS contact,  " +
                                "CASE WHEN h.ContactHouse IS NULL THEN '-' ELSE h.ContactHouse END as homecontactno, " +
                                "h.daysperweek, " +
                            



                                "CASE WHEN '" + sadjh["MonToFriTimeFrom"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["MonToFriTimeFrom"].ToString() + "' END as MonToFriTimeFrom, " +
                                "CASE WHEN '" + sadjh["MonToFriTimeTo"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["MonToFriTimeTo"].ToString() + "' END as MonToFriTimeTo, " +
                                "CASE WHEN '" + sadjh["MonToFriLunchHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["MonToFriLunchHr"].ToString() + "' END as MonToFriLunchHr, " +
                                "CASE WHEN '" + sadjh["MonToFriTeaBrkHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["MonToFriTeaBrkHr"].ToString() + "' END as MonToFriTeaBrkHr, " +
                                "CASE WHEN '" + sadjh["SatTimeFrom"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SatTimeFrom"].ToString() + "' END as SatTimeFrom, " +
                                "CASE WHEN '" + sadjh["SatTimeTo"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SatTimeTo"].ToString() + "' END as SatTimeTo, " +
                                "CASE WHEN '" + sadjh["SatLunchHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SatLunchHr"].ToString() + "' END as SatLunchHr, " +
                                "CASE WHEN '" + sadjh["SatTeaBrkHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SatTeaBrkHr"].ToString() + "' END as SatTeaBrkHr, " +
                                "CASE WHEN '" + sadjh["SunTimeFrom"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SunTimeFrom"].ToString() + "' END as SunTimeFrom, " +
                                "CASE WHEN '" + sadjh["SunTimeTo"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SunTimeTo"].ToString() + "' END as SunTimeTo, " +
                                "CASE WHEN '" + sadjh["SunLunchHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SunLunchHr"].ToString() + "' END as SunLunchHr, " +
                                "CASE WHEN '" + sadjh["SunTeaBrkHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["SunTeaBrkHr"].ToString() + "' END as SunTeaBrkHr, " +
                                "CASE WHEN '" + sadjh["RegularOffDay"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["RegularOffDay"].ToString() + "' END as RegularOffDay, " +
                                "CASE WHEN '" + sadjh["PHTimeFrom"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["PHTimeFrom"].ToString() + "' END as PHTimeFrom, " +
                                "CASE WHEN '" + sadjh["PHTimeTo"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["PHTimeTo"].ToString() + "' END as PHTimeTo, " +
                                "CASE WHEN '" + sadjh["PHLunchHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["PHLunchHr"].ToString() + "' END as PHLunchHr, " +
                                "CASE WHEN '" + sadjh["PHTeaBrkHr"].ToString() + "' = '' THEN NULL ELSE '" + sadjh["PHTeaBrkHr"].ToString() + "' END as PHTeaBrkHr, " +
                                "(Select '" + sadjh["paytypecode"].ToString() + "' as paytypecode) as paytypecode, " +
                                //"CASE WHEN h.paytypecode LIKE 'M%' THEN 'Monthly' WHEN h.paytypecode LIKE 'W%' THEN 'Weekly' WHEN h.paytypecode LIKE 'D%' THEN 'Daily' end as paytypecode, " +
                                "ISNULL(h.cadd1,'')+' '+ISNULL(h.cadd2,'')+' '+ISNULL(h.cadd3,'') as homeaddress, " +
                                "0 as SalaryDeductionPerDay,  " + // TO GET THE VALUE
                                "0 as OtherDudction," + // TO GET THE VALUE
                                //"(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='BASIC' and PFMSR.empnum=h.empnum) as [Basic], " +
                                //"(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ATTNALLW' and PFMSR.empnum=h.empnum) as [ATTNALLW], " +
                                //"(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ACCOMALLW' and PFMSR.empnum=h.empnum) as [ACCOMALLW], " +
                                //"(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='DRIVTRANSALLW' and PFMSR.empnum=h.empnum) as [DRIVTRANSALLW], " +
                                //"(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='OTALLW' and PFMSR.empnum=h.empnum) as [OTALLW], " +
                                "(Select top 1 ISNULL(rateamt,0) from sadj2 where sadj2.hsamcode='BASIC' and sadj2.empnum=h.empnum and sadj2.refnum='" + sadjh["refnum"].ToString() + "') as [Basic], " +
                                "(Select top 1 ISNULL(rateamt,0) from sadj2 where sadj2.hsamcode='ATTNALLW' and sadj2.empnum=h.empnum and sadj2.refnum='" + sadjh["refnum"].ToString() + "') as [ATTNALLW], " +
                                "(Select top 1 ISNULL(rateamt,0) from sadj2 where sadj2.hsamcode='ACCOMALLW' and sadj2.empnum=h.empnum and sadj2.refnum='" + sadjh["refnum"].ToString() + "') as [ACCOMALLW], " +
                                "(Select top 1 ISNULL(rateamt,0) from sadj2 where sadj2.hsamcode='DRIVTRANSALLW' and sadj2.empnum=h.empnum and sadj2.refnum='" + sadjh["refnum"].ToString() + "') as [DRIVTRANSALLW], " +
                                "(Select top 1 ISNULL(rateamt,0) from sadj2 where sadj2.hsamcode='OTALLW' and sadj2.empnum=h.empnum and sadj2.refnum='" + sadjh["refnum"].ToString() + "') as [OTALLW], " +
                                //"(Select top 1 ISNULL(rateamt,0) from PFMSR where PFMSR.hsamcode='OTHER' and PFMSR.empnum=h.empnum) as OtherAllowance, " + // TO GET THE VALUE
                                "(SELECT SUM(rateamt) FROM sadj2 WHERE sadj2.empnum=h.empnum and sadj2.hsamcode NOT IN ('DRIVTRANSALLW','ACCOMALLW','ATTNALLW','BASIC') and (sadj2.hsamcode LIKE '%ALLW' OR sadj2.hsamcode='OTHER') and sadj2.refnum='" + sadjh["refnum"].ToString() + "') as OtherAllowance,  " +
                                "'"+sadjh["approvedcomments"].ToString()+"' as remark, " +
                                "CASE WHEN FM.name IS NULL THEN '-' ELSE FM.name END as NextOfKeen, " +
                                "CASE WHEN FM.contact IS NULL THEN '-' ELSE FM.contact END as FamilyContact " +
                            "from hemph h " +
                            "LEFT JOIN " +
                            "( " +
                            "Select top 1 empnum,name,contact from FAMR where empnum='" + sadjh["empnum"].ToString().Trim() + "' and ISNULL(isemergency,0)=1   " +
                            ")FM " +
                            "ON h.empnum=FM.empnum " +
                            "LEFT JOIN MATM M  " +
                            "ON h.matnum=M.matnum " +
                            "LEFT JOIN SITM S " +
                            "ON h.sitenum=S.sitenum " +
                        ")A where A.empnum='" + sadjh["empnum"].ToString().Trim() + "'";

            ds1 = this.dbaccess.RemoteStandardSQL.GetSQLResult(str);




            ds1.Tables[0].TableName = "LetterAppointment";


            string str2 = "Select * FROM hemph where empnum='" + sadjh["empnum"].ToString() + "' ";

            //string str2 = "SELECT [ID] "+
            //                  ,[refnum] "+
            //                  ,[empnum] "+
            //                  ,[nric] "+
            //                  ,[empname1] "+
            //                  ,[gender] "+
            //                  ,[nationality] "+
            //                  ,[maritalstatus]
            //                  ,[ethnicity]
            //                  ,[dob]
            //                  ,[bloodtype]
            //                  ,[etype]
            //                  ,[statusid]
            //                  ,[sectorcode]
            //                  ,[dateconfirmed]
            //                  ,[dateresigned]
            //                  ,[bankacc]
            //                  ,[basicsal]
            //                  ,[user]
            //                  ,[flag]
            //                  ,[status]
            //                  ,[created]
            //                  ,[modified]
            //                  ,[createdby]
            //                  ,[nextapprdate]
            //                  ,[remark]
            //                  ,[contact]
            //                  ,[empname]
            //                  ,[designation]
            //                  ,[cardid]
            //                  ,[recrempnum]
            //                  ,[hsgcode]
            //                  ,[photo]
            //                  ,[religion]
            //                  ,[force]
            //                  ,[rank]
            //                  ,[vocation]
            //                  ,[yearserved]
            //                  ,[padd1]
            //                  ,[padd2]
            //                  ,[padd3]
            //                  ,[ppostal]
            //                  ,[cadd1]
            //                  ,[cadd2]
            //                  ,[cadd3]
            //                  ,[cpostal]
            //                  ,[isNSmandatory]
            //                  ,[statuscode]
            //                  ,[datejoined]
            //                  ,[hramnum]
            //                  ,[regname]
            //                  ,[isHQstaff]
            //                  ,[confirmationdate]
            //                  ,[isretired]
            //                  ,[contractend]
            //                  ,[sitnumi]
            //                  ,'' as [paytypecode]
            //                  ,[matnum]
            //                  ,[matname]
            //                  ,[daysperweek]
            //                  ,[hramdesc]
            //                  ,[isblacklisted]
            //                  ,[BldgBlock]
            //                  ,[LevelNo]
            //                  ,[UnitNo]
            //                  ,[StreetName]
            //                  ,[CountryAddress]
            //                  ,[BankCode]
            //                  ,[BranchCode]
            //                  ,[ForeignAddress]
            //                  ,[bankname]
            //                  ,[COUNTRY]
            //                  ,[agencyfund]
            //                  ,[datePR]
            //                  ,[postal]
            //                  ,[pcountry]
            //                  ,[pmcode]
            //                  ,[emptype]
            //                  ,[photourl]
            //                  ,[sitenum]
            //                  ,[contractdoc]
            //                  ,[contractloc]
            //                  ,[contractsigned]
            //                  ,[signaturepicloc]
            //                  ,[interviewedby]
            //                  ,[isWebAdd]
            //                  ,[webguid]
            //                  ,[modifiedby]
            //                  ,[contactno]
            //                  ,[isApproved]
            //                  ,[approvedate]
            //                  ,[approveby]
            //                  ,[approvename]
            //                  ,[approvestatus]
            //                  ,[dateapplied]
            //                  ,[englishWritten]
            //                  ,[englishSpoken]
            //                  ,[chineseWritten]
            //                  ,[chineseSpoken]
            //                  ,[otherLanguage]
            //                  ,[otherLanguageWritten]
            //                  ,[otherLanguageSpoken]
            //                  ,[driverlicence]
            //                  ,[declaration1]
            //                  ,[declaration2]
            //                  ,[declaration3]
            //                  ,[declaration4]
            //                  ,[declaration5]
            //                  ,[declaration6]
            //                  ,[declaration6remark]
            //                  ,[giroReason]
            //                  ,[appliFormSigLoc]
            //                  ,[contactHouse]
            //                  ,[commencedate]
            //                  ,[homecontactno]
            //                  ,[MonToFriTimeFrom]
            //                  ,[MonToFriTimeTo]
            //                  ,[MonToFriLunchHr]
            //                  ,[MonToFriTeaBrkHr]
            //                  ,[SatTimeFrom]
            //                  ,[SatTimeTo]
            //                  ,[SatLunchHr]
            //                  ,[SatTeaBrkHr]
            //                  ,[SunTimeFrom]
            //                  ,[SunTimeTo]
            //                  ,[SunLunchHr]
            //                  ,[SunTeaBrkHr]
            //                  ,[RegularOffDay]
            //                  ,[PHTimeFrom]
            //                  ,[PHTimeTo]
            //                  ,[PHLunchHr]
            //                  ,[PHTeaBrkHr]
            //                  ,[jobgroup]
            //                  ,[RecommSigLoc]
            //                  ,[email]
            //                  ,[referby]
            //                  ,[giroSigLoc]
            //                  ,[supv]
            //                  ,[rejDesc]
            //                  ,[emgName]
            //                  ,[emgContact]
            //                  ,[emgRelationship]
            //                  ,[salaryRemark]
            //                  ,[resigndate]
            //                  ,[declaration7]
            //                  ,[declaration1Remark]
            //                  ,[declaration2Remark]
            //                  ,[declaration3Remark]
            //                  ,[declaration4Remark]
            //                  ,[declaration5Remark]
            //                  ,[accountHolderName]
            //                  ,[nricType]
            //                  ,[language1]
            //                  ,[language2]
            //                  ,[isConsent]
            //                  ,[interviewername]
            //                  ,[printLocalAdd]
            //                  ,[printForeignAdd]
            //                  ,[MonToFriShiftCode]
            //                  ,[SatShiftCode]
            //                  ,[SunShiftCode]
            //                  ,[PHShiftCode]
            //                  ,[salutationCode]
            //                  ,[sitenumt]
            //                  ,[oriempname]
            //                  ,[pemail]
            //              FROM [HEMPH]";

            this.dbaccess.ReadSQL("HEMPHTmp", str2);

            DataTable HEMPHTmp = this.dbaccess.DataSet.Tables["HEMPHTmp"];

            DataTable HEMPH1 = HEMPHTmp.Copy();

            HEMPH1.TableName = "HEMPH1";


            if (ds1.Tables.Contains("HEMPH1"))
            {
                ds1.Tables["HEMPH1"].Dispose();
                ds1.Tables.Remove("HEMPH1");
                HEMPH1.TableName = "HEMPH1";
                ds1.Tables.Add(HEMPH1);
            }
            else
            {
                HEMPH1.TableName = "HEMPH1";
                ds1.Tables.Add(HEMPH1);
            }


            /////


            string sadjhStr = "Select * FROM SADJH where refnum='" + sadjh["refnum"].ToString() + "' ";

            this.dbaccess.ReadSQL("SADJHTemp1", sadjhStr);

            DataTable SADJHTemp1 = this.dbaccess.DataSet.Tables["SADJHTemp1"];

            DataTable SADJH1 = SADJHTemp1.Copy();

            SADJH1.TableName = "SADJH1";


            if (ds1.Tables.Contains("SADJH1"))
            {
                ds1.Tables["SADJH1"].Dispose();
                ds1.Tables.Remove("SADJH1");
                SADJH1.TableName = "SADJH1";
                ds1.Tables.Add(SADJH1);
            }
            else
            {
                SADJH1.TableName = "SADJH1";
                ds1.Tables.Add(SADJH1);
            }


            //selectedCollection.Add("wlr1", "SELECT * FROM WLR where [guid]='" + wlr.Rows[dg_warning.CurrentCell.RowNumber]["guid"].ToString() + "'");
            //this.dbaccess.ReadSQL(selectedCollection);

            //GetWarningSignature(wlr.Rows[dg_warning.CurrentCell.RowNumber]["guid"].ToString());

            //DataTable SigTB1 = this.dbaccess.DataSet.Tables["SigTB"].Copy();

            //SigTB1.TableName = "SigTB1";

            //if (this.dbaccess.DataSet.Tables.Contains("SigTB1"))
            //{
            //    this.dbaccess.DataSet.Tables["SigTB1"].Dispose();
            //    this.dbaccess.DataSet.Tables.Remove("SigTB1");
            //    SigTB1.TableName = "SigTB1";
            //    this.dbaccess.DataSet.Tables.Add(SigTB1);
            //}
            //else
            //{
            //    SigTB1.TableName = "SigTB1";
            //    this.dbaccess.DataSet.Tables.Add(SigTB1);
            //}



            string strCoy = "select top 1 * from coy";

            this.dbaccess.ReadSQL("Coy", strCoy);

            DataTable Coy = this.dbaccess.DataSet.Tables["Coy"];

            DataTable Coy1 = Coy.Copy();

            Coy1.TableName = "Coy1";

            if (ds1.Tables.Contains("Coy1"))
            {
                ds1.Tables["Coy1"].Dispose();
                ds1.Tables.Remove("Coy1");
                Coy1.TableName = "Coy1";
                ds1.Tables.Add(Coy1);
            }
            else
            {
                Coy1.TableName = "Coy1";
                ds1.Tables.Add(Coy1);
            }

            GetSalaryHistory();

            DataTable dtGetSalH1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select top 1 * from dtGetSalH");

            dtGetSalH1.TableName = "dtGetSalH1";

            if (ds1.Tables.Contains("dtGetSalH1"))
            {
                ds1.Tables["dtGetSalH1"].Dispose();
                ds1.Tables.Remove("dtGetSalH1");
                dtGetSalH1.TableName = "dtGetSalH1";
                ds1.Tables.Add(dtGetSalH1);
            }
            else
            {
                dtGetSalH1.TableName = "dtGetSalH1";
                ds1.Tables.Add(dtGetSalH1);
            }

            GetSignature();

            DataTable SigTB1 = this.dbaccess.DataSet.Tables["SigTB"].Copy();

            SigTB1.TableName = "SigTB1";

            if (ds1.Tables.Contains("SigTB1"))
            {
                ds1.Tables["SigTB1"].Dispose();
                ds1.Tables.Remove("SigTB1");
                SigTB1.TableName = "SigTB1";
                ds1.Tables.Add(SigTB1);
            }
            else
            {
                SigTB1.TableName = "SigTB1";
                ds1.Tables.Add(SigTB1);
            }


            return ds1;

        }

        private DataSet PFds1()
        {
            DataTable sadjht = this.dbaccess.DataSet.Tables["sadjh"];
            DataRow sadjh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];
            DataTable sadj1 = this.dbaccess.DataSet.Tables["sadj1"];
            DataTable sadj2 = this.dbaccess.DataSet.Tables["sadj2"];
            DataSet ds1 = new DataSet("LAds1");



            string str = "Select sj.refnum, h.nric, sj.empnum, h.empname, m.matname, sj.ratingperiod, sj.appraisedby,sj.ratingother "+
                            "from SADJH sj left join hemph h on sj.empnum=h.empnum left join sitm sm on h.sitenum=sm.sitenum left join matm m on h.matnum=m.matnum "+
                            "where sj.refnum='"+sadjh["refnum"].ToString()+"'";

            ds1 = this.dbaccess.RemoteStandardSQL.GetSQLResult(str);

            ds1.Tables[0].TableName = "PerfAppr";


            string strPerfFactor = "Select a.refnum,a.gpfmcode, a.performfactorNo, a.performfactor,a.score,a.maxScore, a.groupcode,b.groupdesc,a.remark,a.scoregroup  from " +
                                    "( "+
                                    "select * from SADJ1  "+
                                    ")a  "+
                                    "left join GPFM1 b on a.gpfmcode=b.gpfmcode and a.groupcode=b.groupcode where a.refnum='" + sadjh["refnum"].ToString() + "' ";

            this.dbaccess.ReadSQL("PerfFactorTb", strPerfFactor);

            DataTable PerfFactorTb = this.dbaccess.DataSet.Tables["PerfFactorTb"];

            DataTable PerfFactor = PerfFactorTb.Copy();

            PerfFactor.TableName = "PerfFactor";


            string strCoy = "select top 1 * from coy";

            this.dbaccess.ReadSQL("Coy", strCoy);

            DataTable Coy = this.dbaccess.DataSet.Tables["Coy"];

            DataTable Coy1 = Coy.Copy();

            Coy1.TableName = "Coy1";

            string strGPFM1 = "select * from GPFM1";

            this.dbaccess.ReadSQL("GPFM1", strGPFM1);

            DataTable GPFM1 = this.dbaccess.DataSet.Tables["GPFM1"];

            DataTable GPFM11 = GPFM1.Copy();

            GPFM11.TableName = "GPFM11";


            if (ds1.Tables.Contains("PerfFactor"))
            {
                ds1.Tables["PerfFactor"].Dispose();
                ds1.Tables.Remove("PerfFactor");
                PerfFactor.TableName = "PerfFactor";
                ds1.Tables.Add(PerfFactor);
            }
            else
            {
                PerfFactor.TableName = "PerfFactor";
                ds1.Tables.Add(PerfFactor);
            }



            if (ds1.Tables.Contains("GPFM11"))
            {
                ds1.Tables["GPFM11"].Dispose();
                ds1.Tables.Remove("GPFM11");
                GPFM11.TableName = "GPFM11";
                ds1.Tables.Add(GPFM11);
            }
            else
            {
                GPFM11.TableName = "GPFM11";
                ds1.Tables.Add(GPFM11);
            }

            if (ds1.Tables.Contains("Coy1"))
            {
                ds1.Tables["Coy1"].Dispose();
                ds1.Tables.Remove("Coy1");
                Coy1.TableName = "Coy1";
                ds1.Tables.Add(Coy1);
            }
            else
            {
                Coy1.TableName = "Coy1";
                ds1.Tables.Add(Coy1);
            }

            DataTable SADJH1 = sadjht.Copy();
            DataTable SADJ11 = sadj1.Copy();
            DataTable SADJ21 = sadj2.Copy();

            SADJH1.TableName = "SADJH1";
            SADJ11.TableName = "SADJ11";
            SADJ21.TableName = "SADJ21";

            if (ds1.Tables.Contains("SADJH1"))
            {
                ds1.Tables["SADJH1"].Dispose();
                ds1.Tables.Remove("SADJH1");
                SADJH1.TableName = "SADJH1";
                ds1.Tables.Add(SADJH1);
            }
            else
            {
                SADJH1.TableName = "SADJH1";
                ds1.Tables.Add(SADJH1);
            }
            ///

            if (ds1.Tables.Contains("SADJ11"))
            {
                ds1.Tables["SADJ11"].Dispose();
                ds1.Tables.Remove("SADJ11");
                SADJ11.TableName = "SADJ11";
                ds1.Tables.Add(SADJ11);
            }
            else
            {
                SADJ11.TableName = "SADJ11";
                ds1.Tables.Add(SADJ11);
            }
            ///

            if (ds1.Tables.Contains("SADJ21"))
            {
                ds1.Tables["SADJ21"].Dispose();
                ds1.Tables.Remove("SADJ21");
                SADJ21.TableName = "SADJ21";
                ds1.Tables.Add(SADJ21);
            }
            else
            {
                SADJ21.TableName = "SADJ21";
                ds1.Tables.Add(SADJ21);
            }
            ///

            GetSignature();

            DataTable SigTB1 = this.dbaccess.DataSet.Tables["SigTB"].Copy();

            SigTB1.TableName = "SigTB1";

            if (ds1.Tables.Contains("SigTB1"))
            {
                ds1.Tables["SigTB1"].Dispose();
                ds1.Tables.Remove("SigTB1");
                SigTB1.TableName = "SigTB1";
                ds1.Tables.Add(SigTB1);
            }
            else
            {
                SigTB1.TableName = "SigTB1";
                ds1.Tables.Add(SigTB1);
            }


            return ds1;

        }

        private void GetSignature()
        {
            DataTable SigTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select empnum,userSignature as signaturepicloc,appraiserSignature as girosigLoc from sadjh");
            string projectPath = ConfigurationManager.AppSettings.Get("ImagePath");
            SigTB.TableName = "SigTB";

            SigTB.Columns.Add("Photo1", typeof(Byte[]));
            SigTB.Columns.Add("Photo2", typeof(Byte[]));

            if (SigTB.Rows.Count > 0)
            {
                foreach (DataRow dr1 in SigTB.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["signaturepicloc"]))
                        {
                            string test = dr1["signaturepicloc"].ToString().Trim();
                            if (dr1["signaturepicloc"].ToString().Trim() != "NULL")
                            {
                                if (dr1["signaturepicloc"].ToString().Trim() != "null")
                                {
                                    dr1["photo1"] = System.IO.File.ReadAllBytes(dr1["signaturepicloc"].ToString().Trim());
                                }
                                else
                                {

                                    dr1["photo1"] = System.IO.File.ReadAllBytes(projectPath + @"\BlankImage.JPG");
                                }
                            }
                            else
                            {

                                dr1["photo1"] = System.IO.File.ReadAllBytes(projectPath + @"\BlankImage.JPG");
                            }
                            
                        }
                        else
                        {

                            dr1["photo1"] = System.IO.File.ReadAllBytes(projectPath + @"\BlankImage.JPG");
                        }


                        if (!BizFunctions.IsEmpty(dr1["girosigLoc"]))
                        {
                            if (dr1["girosigLoc"].ToString().Trim() != "NULL")
                            {
                                if (dr1["girosigLoc"].ToString().Trim() != "null")
                                {
                                    dr1["photo2"] = System.IO.File.ReadAllBytes(dr1["girosigLoc"].ToString().Trim());
                                }
                                else
                                {

                                    dr1["photo2"] = System.IO.File.ReadAllBytes(projectPath + @"\BlankImage.JPG");
                                }
                            }
                            else
                            {

                                dr1["photo2"] = System.IO.File.ReadAllBytes(projectPath + @"\BlankImage.JPG");
                            }
                        }
                        else
                        {

                            dr1["photo2"] = System.IO.File.ReadAllBytes(projectPath + @"\BlankImage.JPG");
                        }
                    }
                }
            }

            if (this.dbaccess.DataSet.Tables.Contains("SigTB"))
            {
                this.dbaccess.DataSet.Tables["SigTB"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("SigTB");
                this.dbaccess.DataSet.Tables.Add(SigTB);

            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(SigTB);
            }

        }

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


        private void getAppraisalTemplate()
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJH"].Rows[0];
            DataTable SADJ1 = this.dbaccess.DataSet.Tables["SADJ1"];
            DataTable SADJ2 = this.dbaccess.DataSet.Tables["SADJ2"];

            string getAppraisal = "Select * from GPFM2 where gpfmcode='" + SADJh["gpfmcode"].ToString() + "'";

            this.dbaccess.ReadSQL("GPFM2", getAppraisal);

            DataTable GPFM2 = this.dbaccess.DataSet.Tables["GPFM2"];

            if (SADJ1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(SADJ1);
            }
            if (GPFM2.Rows.Count > 0)
            {
                foreach (DataRow dr2 in GPFM2.Rows)
                {
                    DataRow dr2Insert = SADJ1.NewRow();
                    dr2Insert["gpfmcode"] = dr2["gpfmcode"];
                    dr2Insert["groupcode"] = dr2["groupcode"];
                    dr2Insert["performfactorNo"] = dr2["performfactorNo"];
                    dr2Insert["performfactor"] = dr2["performfactor"];
                    dr2Insert["score"] = dr2["score"];
                    dr2Insert["scoregroup"] = dr2["scoregroup"];
                    dr2Insert["maxscore"] = dr2["maxscore"];

                    SADJ1.Rows.Add(dr2Insert);

                }
            }
            
        }

        private void GetSalaryHistory()
        {
            DataRow sadjh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];
            if (!BizFunctions.IsEmpty(sadjh["nric"]))
            {
               
                

                    string GetSalHistoryStr = "SELECT * FROM " +
                           "( " +
                               "select  " +
                                   "matnum, " +
                                   "SUM(rateamt) as TotalSalary, " +
                                   "dateadjusted as DateAdjusted, " +
                                   "ISNULL(refnum,'') as Refnum, " +
                                   "ISNULL(SADJmcode,'') as AdjustedCode, " +
                                   "sitenum  " +
                               "from SALH where empnum='" + sadjh["empnum"].ToString() + "' and status<>'V' group by matnum,nric,dateadjusted,refnum,SADJmcode,sitenum  " +
                               "UNION " +
                               "SELECT * FROM " +
                               "( " +
                               "select   " +
                                   "E.matnum, " +
                                   "E.currentsal as TotalSalary, " +
                                   "E.lastday as DateAdjusted, " +
                                   "E.refnum as Refnum, " +
                                   "'RESIGN' AS AdjustedCode, " +
                                   "E.Sitenum  " +
                               "from HEMPH H LEFT JOIN ERRH E ON H.empnum=E.empnum WHERE H.empnum='" + sadjh["empnum"].ToString() + "' and E.[status]<>'V' " +
                               ")A " +
                           ")B " +
                           "order by b.DateAdjusted desc";


                    this.dbaccess.ReadSQL("dtGetSalH", GetSalHistoryStr);

                  
             }
            
            //SalHistorydatagrid.CaptionText = "Salary History";
        }


        private void GetTiming()
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["SADJh"].Rows[0];
            string getTiming = "SELECT " +
                                  "[MonToFriTimeFrom] " +
                                  ",[MonToFriTimeTo] " +
                                  ",[MonToFriLunchHr] " +
                                  ",[MonToFriTeaBrkHr] " +
                                  ",[SatTimeFrom] " +
                                  ",[SatTimeTo] " +
                                  ",[SatLunchHr] " +
                                  ",[SatTeaBrkHr] " +
                                  ",[SunTimeFrom] " +
                                  ",[SunTimeTo] " +
                                  ",[SunLunchHr] " +
                                  ",[SunTeaBrkHr] " +
                                  ",[RegularOffDay] " +
                                  ",[PHTimeFrom] " +
                                  ",[PHTimeTo] " +
                                  ",[PHLunchHr] " +
                                  ",[PHTeaBrkHr] " +
                              "FROM HEMPH WHERE EMPNUM='" + SADJh["empnum"].ToString() + "'";

            this.dbaccess.ReadSQL("getTimingTB", getTiming);

            DataTable getTimingTB = this.dbaccess.DataSet.Tables["getTimingTB"];

            if (getTimingTB != null)
            {
                if (getTimingTB.Rows.Count > 0)
                {

                    if(BizFunctions.IsEmpty(SADJh["t1"]))
                    {
                        SADJh["t1"] = 0;
                    }
                    if (BizFunctions.IsEmpty(SADJh["t2"]))
                    {
                        SADJh["t2"] = 0;
                    }
                    if (BizFunctions.IsEmpty(SADJh["t3"]))
                    {
                        SADJh["t3"] = 0;
                    }
                    if (BizFunctions.IsEmpty(SADJh["t4"]))
                    {
                        SADJh["t4"] = 0;
                    }
                    if (BizFunctions.IsEmpty(SADJh["t5"]))
                    {
                        SADJh["t5"] = 0;
                    }


                    DataRow dr1 = this.dbaccess.DataSet.Tables["getTimingTB"].Rows[0];


                    if (BizFunctions.IsEmpty(SADJh["MonToFriTimeFrom"]) || SADJh["MonToFriTimeFrom"].ToString().Trim()=="0000")
                    {
                        SADJh["MonToFriTimeFrom"] = dr1["MonToFriTimeFrom"];
                    }

                    if (BizFunctions.IsEmpty(SADJh["MonToFriTimeTo"]) || SADJh["MonToFriTimeTo"].ToString().Trim() == "0000")
                    {
                        SADJh["MonToFriTimeTo"] = dr1["MonToFriTimeTo"];

                    }

                    if (BizFunctions.IsEmpty(SADJh["MonToFriLunchHr"]))
                    {
                        SADJh["MonToFriLunchHr"] = dr1["MonToFriLunchHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["MonToFriLunchHr"]) == "0")
                        {
                            SADJh["MonToFriLunchHr"] = dr1["MonToFriLunchHr"];
                        }
                    }

                    if (BizFunctions.IsEmpty(SADJh["MonToFriTeaBrkHr"]))
                    {
                        SADJh["MonToFriTeaBrkHr"] = dr1["MonToFriTeaBrkHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["MonToFriTeaBrkHr"]) == "0")
                        {
                            SADJh["MonToFriTeaBrkHr"] = dr1["MonToFriTeaBrkHr"];
                        }
                    }


                    if (BizFunctions.IsEmpty(SADJh["SatTimeFrom"]) || SADJh["SatTimeFrom"].ToString().Trim() == "0000")
                    {
                        SADJh["SatTimeFrom"] = dr1["SatTimeFrom"];

                    }

                    if (BizFunctions.IsEmpty(SADJh["SatTimeTo"]) || SADJh["SatTimeTo"].ToString().Trim() == "0000")
                    {
                        SADJh["SatTimeTo"] = dr1["SatTimeTo"];

                    }

                    if (BizFunctions.IsEmpty(SADJh["SatLunchHr"]))
                    {
                        SADJh["SatLunchHr"] = dr1["SatLunchHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["SatLunchHr"]) == "0")
                        {
                            SADJh["SatLunchHr"] = dr1["SatLunchHr"];
                        }
                    }

                    if (BizFunctions.IsEmpty(SADJh["SatTeaBrkHr"]))
                    {
                        SADJh["SatTeaBrkHr"] = dr1["SatTeaBrkHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["SatTeaBrkHr"]) == "0")
                        {
                            SADJh["SatTeaBrkHr"] = dr1["SatTeaBrkHr"];
                        }
                    }

                    if (BizFunctions.IsEmpty(SADJh["SunTimeFrom"]) || SADJh["SunTimeFrom"].ToString().Trim() == "0000")
                    {
                        SADJh["SunTimeFrom"] = dr1["SunTimeFrom"];

                    }

                    if (BizFunctions.IsEmpty(SADJh["SunTimeTo"]) || SADJh["SunTimeTo"].ToString().Trim() == "0000")
                    {
                        SADJh["SunTimeTo"] = dr1["SunTimeTo"];

                    }

                    if (BizFunctions.IsEmpty(SADJh["SunLunchHr"]))
                    {
                        SADJh["SunLunchHr"] = dr1["SunLunchHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["SunLunchHr"]) == "0")
                        {
                            SADJh["SunLunchHr"] = dr1["SunLunchHr"];
                        }
                    }

                    if (BizFunctions.IsEmpty(SADJh["SunTeaBrkHr"]))
                    {
                        SADJh["SunTeaBrkHr"] = dr1["SunTeaBrkHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["SunTeaBrkHr"]) == "0")
                        {
                            SADJh["SunTeaBrkHr"] = dr1["SunTeaBrkHr"];
                        }
                    }

                    if (BizFunctions.IsEmpty(SADJh["RegularOffDay"]))
                    {
                        SADJh["RegularOffDay"] = dr1["RegularOffDay"];

                    }

                    if (BizFunctions.IsEmpty(SADJh["PHTimeFrom"]) || SADJh["PHTimeFrom"].ToString().Trim() == "0000")
                    {
                        SADJh["PHTimeFrom"] = dr1["PHTimeFrom"];

                    }

                    if (BizFunctions.IsEmpty(SADJh["PHTimeTo"]) || SADJh["PHTimeTo"].ToString().Trim() == "0000")
                    {
                        SADJh["PHTimeTo"] = dr1["PHTimeTo"];

                    }
                  

                    if (BizFunctions.IsEmpty(SADJh["PHLunchHr"]))
                    {
                        SADJh["PHLunchHr"] = dr1["PHLunchHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["PHLunchHr"]) == "0")
                        {
                            SADJh["PHLunchHr"] = dr1["PHLunchHr"];
                        }
                    }

                    if (BizFunctions.IsEmpty(SADJh["PHTeaBrkHr"]))
                    {
                        SADJh["PHTeaBrkHr"] = dr1["PHTeaBrkHr"];

                    }
                    else
                    {
                        if (Convert.ToString(SADJh["PHTeaBrkHr"]) == "0")
                        {
                            SADJh["PHTeaBrkHr"] = dr1["PHTeaBrkHr"];
                        }
                    }


                }
            }
        }


        private void GetCurrentSalary()
        {
            DataRow sadjh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];
            DataTable sadj3 = this.dbaccess.DataSet.Tables["sadj3"];

            if (!BizFunctions.IsEmpty(sadjh["empnum"]))
            {
                if (sadj3.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(sadj3);                    
                }

                string str = "Select * from PFMSR where empnum='"+sadjh["empnum"].ToString()+"'";

                this.dbaccess.ReadSQL("TempPFMSR", str);

                DataTable TempPFMSR = this.dbaccess.DataSet.Tables["TempPFMSR"];

                if (TempPFMSR.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in TempPFMSR.Rows)
                    {
                        DataRow insertSadj3 = sadj3.NewRow();

                        insertSadj3["nric"] = dr1["nric"];
                        insertSadj3["empnum"] = dr1["empnum"];
                        insertSadj3["rateamt"] = dr1["rateamt"];
                        insertSadj3["hsamcode"] = dr1["hsamcode"];
                        insertSadj3["dateadjusted"] = dr1["dateadjusted"];
                        insertSadj3["salcode"] = dr1["salcode"];
                        insertSadj3["AdjustmentFlag"] = dr1["AdjustmentFlag"];
                        insertSadj3["sadjmcode"] = dr1["sadjmcode"];
                        insertSadj3["line"] = dr1["line"];
                        insertSadj3["isExempt"] = dr1["isExempt"];

                        sadj3.Rows.Add(insertSadj3);
                                                                   
                    }

                 
                }
            }
        }

        private decimal getTotalSalary(DataTable dt)
        {
            decimal totalsal = 0;

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr1 in dt.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["rateamt"]))
                        {
                            dr1["rateamt"] = 0;
                        }
                        totalsal = totalsal + Convert.ToDecimal(dr1["rateamt"]);
                    }
                }
            }

            return totalsal;
        }

        private int GetScoreGroup(string gpfmcode,int performfactorNo,string Groupcode)
        {
            int ScoreGroup = 1;

            string sql1 = "Select scoregroup from GPFM2 where gpfmcode='" + gpfmcode + "' and performfactorNo='" + performfactorNo.ToString() + "' and Groupcode='" + Groupcode + "' ";

            this.dbaccess.ReadSQL("TmpScoreGroupTb", sql1);

            DataTable dt1 = this.dbaccess.DataSet.Tables["TmpScoreGroupTb"];

            if (dt1.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(dt1.Rows[0]["scoregroup"]))
                {
                    if (Convert.ToInt32(dt1.Rows[0]["scoregroup"]) > 0)
                    {
                        ScoreGroup = Convert.ToInt32(dt1.Rows[0]["scoregroup"]);
                    }
                }
            }

            return ScoreGroup;
        }

        private string GetSupervisor(string empnum)
        {
            string SupervisorUserName = "";

            string sql1 = "select nric,empnum,empname,supv,[dbo].[GetEmpUsername](supv) as SupUserName from hemph where empnum='"+empnum+"' ";

            this.dbaccess.ReadSQL("TmpSupUserName", sql1);

            DataTable dt1 = this.dbaccess.DataSet.Tables["TmpSupUserName"];

            if (dt1.Rows.Count > 0)
            {
                if(!BizFunctions.IsEmpty(dt1.Rows[0]["SupUserName"]))
                {
                    SupervisorUserName = dt1.Rows[0]["SupUserName"].ToString();
                }
            }

            return SupervisorUserName;
        }


        private void GenerateGroupScore()
        {
            DataRow sadjh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];
            DataTable sadj1 = this.dbaccess.DataSet.Tables["sadj1"];
            DataTable sadj4 = this.dbaccess.DataSet.Tables["sadj4"];

            string sql1 = "Select "+
	                        "a.scoregroup,  "+
	                        "a.ttscore, "+
	                        "a.ttmaxscore,  "+
	                        "a.ttfactor "+
                            //"Case when ISNULL(a.ttscore,0)>0 AND ISNULL(a.ttfactor,0)>0 then Convert(decimal(16,2),a.ttscore)/Convert(decimal(16,2),a.ttfactor) else 0.00 end as ttavgfactor "+
                        "from "+
                        "( "+
                        "Select scoregroup, Convert(decimal(16,2),SUM(ISNULL(score,0))) as ttscore, Convert(decimal(16,2),SUM(ISNULL(maxScore,0))) as ttmaxscore, count(*) as ttfactor from SADJ1  "+
                        "group by scoregroup "+
                        ")a "+
                        "group by a.scoregroup,a.ttscore,a.ttmaxscore, a.ttfactor";

            if (sadj1.Rows.Count > 0)
            {
                if (sadj4.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(sadj4);
                }
                    DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, sql1);

                    if (dt1 != null)
                    {
                        if (dt1.Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in dt1.Rows)
                            {
                                DataRow insertSadj4 = sadj4.NewRow();

                                insertSadj4["scoregroup"] = dr1["scoregroup"];
                                insertSadj4["ttscore"] = dr1["ttscore"];
                                insertSadj4["ttmaxscore"] = dr1["ttmaxscore"];
                                insertSadj4["ttfactor"] = dr1["ttfactor"];

                                //decimal ttavgfactor = 0;

                                if (BizFunctions.IsEmpty(dr1["ttscore"]))
                                {
                                    dr1["ttscore"] = 0;
                                }

                                if (BizFunctions.IsEmpty(dr1["ttfactor"]))
                                {
                                    dr1["ttfactor"] = 0;
                                }

                                if (Convert.ToDecimal(dr1["ttfactor"]) > 0 && Convert.ToDecimal(dr1["ttscore"]) > 0)
                                {
                                    insertSadj4["ttavgfactor"] = Convert.ToDecimal(dr1["ttscore"]) / Convert.ToDecimal(dr1["ttfactor"]);
                                }
                                else
                                {
                                    insertSadj4["ttavgfactor"] = 0;
                                }

                                

                                sadj4.Rows.Add(insertSadj4);
                            }
                        }
                    }
                
            }
        }

        private void LoadCurrentTiming()
        {
            DataRow SADJh = this.dbaccess.DataSet.Tables["sadjh"].Rows[0];

            if(!BizFunctions.IsEmpty(SADJh["empnum"]))
            {
                if(BizFunctions.IsEmpty(SADJh["isMassGenerate"]))
                {
                    SADJh["isMassGenerate"] = 0;
                }

                if((bool)SADJh["isMassGenerate"])
                {
                     string getTiming = "SELECT "+
                                                  "[MonToFriTimeFrom] "+
                                                  ",[MonToFriTimeTo] "+
                                                  ",[MonToFriLunchHr] "+
                                                  ",[MonToFriTeaBrkHr] "+
                                                  ",[SatTimeFrom] "+
                                                  ",[SatTimeTo] "+
                                                  ",[SatLunchHr] "+
                                                  ",[SatTeaBrkHr] "+
                                                  ",[SunTimeFrom] "+
                                                  ",[SunTimeTo] "+
                                                  ",[SunLunchHr] "+
                                                  ",[SunTeaBrkHr] "+
                                                  ",[RegularOffDay] "+
                                                  ",[PHTimeFrom] "+
                                                  ",[PHTimeTo] "+
                                                  ",[PHLunchHr] "+
                                                  ",[PHTeaBrkHr] "+
                                                  ",[MonToFriShiftCode]" +
                                                  ",[SatShiftCode]" +
                                                  ",[SunShiftCode]" +
                                                  ",[PHShiftCode]" +
                                              "FROM HEMPH WHERE EMPNUM='" + SADJh["empnum"].ToString() + "'";

                            this.dbaccess.ReadSQL("getTimingTB", getTiming);

                            DataTable getTimingTB = this.dbaccess.DataSet.Tables["getTimingTB"];

                            if (getTimingTB != null)
                            {
                                if (getTimingTB.Rows.Count > 0)
                                {
                                    DataRow dr1 = this.dbaccess.DataSet.Tables["getTimingTB"].Rows[0];

                                    if (BizFunctions.IsEmpty(SADJh["oldMonToFriTimeFrom"]))
                                    {
                                        SADJh["oldMonToFriTimeFrom"] = dr1["MonToFriTimeFrom"];
                                    }

                                    if (BizFunctions.IsEmpty(SADJh["oldMonToFriTimeTo"]))
                                    {
                                        SADJh["oldMonToFriTimeTo"] = dr1["MonToFriTimeTo"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldMonToFriLunchHr"]))
                                    {
                                        SADJh["oldMonToFriLunchHr"] = dr1["MonToFriLunchHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldMonToFriTeaBrkHr"]))
                                    {
                                        SADJh["oldMonToFriTeaBrkHr"] = dr1["MonToFriTeaBrkHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSatTimeFrom"]))
                                    {
                                        SADJh["oldSatTimeFrom"] = dr1["SatTimeFrom"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSatTimeTo"]))
                                    {
                                        SADJh["oldSatTimeTo"] = dr1["SatTimeTo"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSatLunchHr"]))
                                    {
                                        SADJh["oldSatLunchHr"] = dr1["SatLunchHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSatTeaBrkHr"]))
                                    {
                                        SADJh["oldSatTeaBrkHr"] = dr1["SatTeaBrkHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSunTimeFrom"]))
                                    {
                                        SADJh["oldSunTimeFrom"] = dr1["SunTimeFrom"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSunTimeTo"]))
                                    {
                                        SADJh["oldSunTimeTo"] = dr1["SunTimeTo"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSunLunchHr"]))
                                    {
                                        SADJh["oldSunLunchHr"] = dr1["SunLunchHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldSunTeaBrkHr"]))
                                    {
                                        SADJh["oldSunTeaBrkHr"] = dr1["SunTeaBrkHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldRegularOffDay"]))
                                    {
                                        SADJh["oldRegularOffDay"] = dr1["RegularOffDay"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldPHTimeFrom"]))
                                    {
                                        SADJh["oldPHTimeFrom"] = dr1["PHTimeFrom"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldPHTimeTo"]))
                                    {
                                        SADJh["oldPHTimeTo"] = dr1["PHTimeTo"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldPHLunchHr"]))
                                    {
                                        SADJh["oldPHLunchHr"] = dr1["PHLunchHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["oldPHTeaBrkHr"]))
                                    {
                                        SADJh["oldPHTeaBrkHr"] = dr1["PHTeaBrkHr"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["MonToFriShiftCode"]))
                                    {
                                        SADJh["MonToFriShiftCode"] = dr1["MonToFriShiftCode"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["SatShiftCode"]))
                                    {
                                        SADJh["SatShiftCode"] = dr1["SatShiftCode"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["SunShiftCode"]))
                                    {
                                        SADJh["SunShiftCode"] = dr1["SunShiftCode"];
                                    }


                                    if (BizFunctions.IsEmpty(SADJh["PHShiftCode"]))
                                    {
                                        SADJh["PHShiftCode"] = dr1["PHShiftCode"];
                                    }
                                    


                                }
                            }
                }
            }
        }

    }
}
    

