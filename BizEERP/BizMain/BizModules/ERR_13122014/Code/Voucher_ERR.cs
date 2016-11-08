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
//using BizERP.BizModules.UserAuthorization;
#endregion

namespace ATL.ERR
{
    public class Voucher_ERR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        //UserAuthorization sa = null;
        protected TextBox errh_approvedby, 
                          errh_approvedcomments, txt_employmentdate, errh_confirmedcomments,
                          errh_confirmedby,errh_empnum = null;
        protected GroupBox grb_errhapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        protected string headerFormName, RecommendedBy, ApprovedBy = null;

        #endregion

        #region Construct

        public Voucher_ERR(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_ERR.xml", moduleName, voucherBaseHelpers)
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
            DataRow errh = e.DBAccess.DataSet.Tables["errh"].Rows[0];
            //sa = new UserAuthorization(this.moduleName.ToString());
            Initialise();


            if (!BizFunctions.IsEmpty(errh["empnum"]))
            {
                DataTable EmpDT = BizLogicTools.Tools.GetCommonEmpData(errh["empnum"].ToString());
                if (EmpDT.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(errh["empname"]))
                    {
                        errh["empname"] = EmpDT.Rows[0]["empname"].ToString();
                    }
                    if (BizFunctions.IsEmpty(errh["matnum"]))
                    {
                        errh["matnum"] = EmpDT.Rows[0]["matnum"].ToString();
                    }
                    if (BizFunctions.IsEmpty(errh["statuscode"]))
                    {
                        errh["statuscode"] = EmpDT.Rows[0]["statuscode"].ToString();
                    }

                    if (BizFunctions.IsEmpty(errh["sitenum"]))
                    {
                        errh["sitenum"] = EmpDT.Rows[0]["sitenum"].ToString();
                    }
                    if (BizFunctions.IsEmpty(errh["sectorcode"]))
                    {
                        errh["sectorcode"] = BizLogicTools.Tools.GetSectorCode(errh["sitenum"].ToString(), this.dbaccess);
                    }
                }
            }
            if (BizFunctions.IsEmpty(errh["trandate"]))
            {
                errh["trandate"] = DateTime.Now;
            }
        }

        #endregion

        #region Initialise Components

        private void Initialise()
        {
            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
            errh_confirmedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "errh_confirmedcomments") as TextBox;
            errh_confirmedby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "errh_confirmedby") as TextBox;

            errh_approvedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "errh_approvedcomments") as TextBox;
            errh_approvedby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "errh_approvedby") as TextBox;

            errh_empnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "errh_empnum") as TextBox;
            errh_empnum.Leave +=new EventHandler(errh_empnum_Leave);

            txt_employmentdate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_employmentdate") as TextBox;

            grb_errhapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_errhapprinfo") as GroupBox;

            rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            rad_Recd.CheckedChanged += new EventHandler(rad_Recd_CheckedChanged);
            rad_NotRecd.CheckedChanged += new EventHandler(rad_NotRecd_CheckedChanged);
            rad_Appr.CheckedChanged += new EventHandler(rad_Appr_CheckedChanged);
            rad_NotAppr.CheckedChanged += new EventHandler(rad_NotAppr_CheckedChanged);

            errh_confirmedcomments.TextChanged +=new EventHandler(errh_confirmedcomments_TextChanged);
            errh_approvedcomments.TextChanged += new EventHandler(errh_approvedcomments_TextChanged);

            LoadRadioButtonsData();
            //if (!sa.ApprovePermission)
            //{
            //    grb_errhapprinfo.Enabled = false;
            //}
            //else
            //{
            //    grb_errhapprinfo.Enabled = true;
            //}

            GetHiredDate();
            GetCurrentSal();
            EmptyAConfirmation();

            if (!BizFunctions.IsEmpty(errh["empnum"]))
            {
                string statuso = Common.DEFAULT_DOCUMENT_STATUSO;
                string statusp = Common.DEFAULT_DOCUMENT_STATUSP;
                string errhstatus = errh["status"].ToString();

                if (errhstatus == statuso || errhstatus == statusp)
                {
                    errh_empnum.Enabled = false;
                }
            }
            
        }

        #endregion

        private void errh_empnum_Leave(object sender, EventArgs e)
        {
            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
            if (errh_empnum.Text != String.Empty)
            {
                DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(errh_empnum.Text);
                if (tmpEmpData.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in tmpEmpData.Rows)
                    {
                        errh["empnum"] = dr1["empnum"];
                        errh["empname"] = dr1["empname"];
                        errh["matnum"] = dr1["matnum"];
                        errh["sitenum"] = dr1["sitenum"];
                        errh["sectorcode"] = dr1["sectorcode"];
                        errh["employmentdate"] = dr1["datejoined"];
                        errh["currentsal"] = dr1["currentsalary"];
                        GetHiredDate();
                    }
                }

            }
        }

        private void EmptyAConfirmation()
        {
            if (errh_confirmedcomments.Text == "")
            {
                errh_approvedcomments.Enabled = false;
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
            }
        }

        #region Load Radio Button Data

        private void LoadRadioButtonsData()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["errH"].Rows[0];

            if (!BizFunctions.IsEmpty(trqh["isrecommended"]))
            {
                if ((bool)trqh["isrecommended"])
                {
                    rad_Recd.Checked = true;
                }
                else
                {
                    rad_NotRecd.Checked = true;
                }
            }

            if (!BizFunctions.IsEmpty(trqh["isapproved"]))
            {
                if ((bool)trqh["isapproved"])
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

        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "err_empnum":
                    {
                        
                        e.Condition = BizFunctions.F2Condition("empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "empnum like '" + errh_empnum.Text.Trim() + "%' OR empname like '" + errh_empnum.Text.Trim() + "%'";

                    }
                    break;
          
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
            switch (e.ControlName)
            {
                case "errh_empnum":
                    {
                        if (!BizFunctions.IsEmpty(errh["empnum"]))
                        {
                            e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                            e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                            e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                            e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            e.CurrentRow["currentsal"] = GetCurrentSal();
                            GetHiredDate();
                        }
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

            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];

            //if (ApprovedBy != string.Empty)
            //{
            //    errh["confirmedby"] = RecommendedBy;
            //}
            //if (RecommendedBy != string.Empty)
            //{
            //    errh["approvedby"] = ApprovedBy;
            //}
           
                                  
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];

            if (errh["status"].ToString() == (string)Common.DEFAULT_DOCUMENT_STATUSP)
            {
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set statuscode = 'RESIGNED' where empnum='" + errh["empnum"].ToString() + "' ");
            }
            if (errh["status"].ToString() == (string)Common.DEFAULT_DOCUMENT_STATUSV)
            {
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set statuscode = 'ACTIVE' where empnum='" + errh["empnum"].ToString() + "' ");
                MessageBox.Show("Emp No " + errh["empnum"].ToString() + "'s Status has been set to 'ACTIVE'", "Pico Guards Ltd. Pte.", MessageBoxButtons.OK);
            }

            
    
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


            switch (e.ReportName)
            {

                case "Notice Of Resignation":
                    e.DataSource = NRds1();
                    break;          

            }

        }

        #endregion

        private void GetSignature()
        {
            DataTable SigTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select empnum,appliFormSigLoc as signaturepicloc,girosigLoc from hemph");

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
                            dr1["photo1"] = System.IO.File.ReadAllBytes(dr1["signaturepicloc"].ToString().Trim());
                        }

                        if (!BizFunctions.IsEmpty(dr1["girosigLoc"]))
                        {
                            dr1["photo2"] = System.IO.File.ReadAllBytes(dr1["girosigLoc"].ToString().Trim());
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

        private DataSet NRds1()
        {
            DataSet ds1 = new DataSet("NRds1");

            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];

            string str = "select e.empnum,h.nric, h.empname,e.lastday,e.futurecontact,e.confirmedcomments from errh e left join hemph h on e.empnum=h.empnum "+
                         "where e.refnum='"+errh["refnum"].ToString()+"' ";

            ds1 = this.dbaccess.RemoteStandardSQL.GetSQLResult(str);

            ds1.Tables[0].TableName = "NR";
         

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



            //DataTable dtGetSalH = this.dbaccess.DataSet.Tables["dtGetSalH"];

            DataTable ERRH1 = this.dbaccess.DataSet.Tables["errh"].Copy();
            DataTable ERR11 = this.dbaccess.DataSet.Tables["err1"].Copy();

            ERRH1.TableName = "ERRH1";
            ERR11.TableName = "ERR11";

            if (ds1.Tables.Contains("ERRH1"))
            {
                ds1.Tables["ERRH1"].Dispose();
                ds1.Tables.Remove("ERRH1");
                ERRH1.TableName = "ERRH1";
                ds1.Tables.Add(ERRH1);
            }
            else
            {
                ERRH1.TableName = "ERRH1";
                ds1.Tables.Add(ERRH1);
            }

            if (ds1.Tables.Contains("ERR11"))
            {
                ds1.Tables["ERR11"].Dispose();
                ds1.Tables.Remove("ERR11");
                ERR11.TableName = "ERR11";
                ds1.Tables.Add(ERR11);
            }
            else
            {
                ERR11.TableName = "ERR11";
                ds1.Tables.Add(ERR11);
            }




            return ds1;

        }

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);

        }

        #endregion

        #region Schedule Radio Button Methods
        private void rad_Recd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Recd.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["errH"].Rows[0];

                trqh["isrecommended"] = 1;
            }

        }

        private void rad_NotRecd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotRecd.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["errH"].Rows[0];

                trqh["isrecommended"] = 0;
            }
        }

        private void rad_Appr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Appr.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["errH"].Rows[0];

                trqh["isapproved"] = 1;
            }
        }

        private void rad_NotAppr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotAppr.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["errH"].Rows[0];

                trqh["isapproved"] = 0;
            }
        }
        #endregion

        #region TextChanged Events

        //protected void errh_confirmedcomments_TextChanged(object sender, EventArgs e)
        //{
        //    DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
        //    if (BizFunctions.IsEmpty(errh["confirmedby"]))
        //    {
        //        errh["confirmedby"] = Common.DEFAULT_SYSTEM_USERNAME;
        //    }
        //}

        protected void errh_confirmedcomments_TextChanged(object sender, EventArgs e)
        {

            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
            if (errh_confirmedcomments.Text != "")
            {
                errh_approvedcomments.Enabled = true;
                rad_Recd.Enabled = true;
                rad_NotRecd.Enabled = true;
                RecommendedBy = Common.DEFAULT_SYSTEM_USERNAME;
            }
            else
            {
                errh_approvedcomments.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;

            }
            //if (BizFunctions.IsEmpty(errh["confirmedby"]))
            //{
            //    errh["confirmedby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}

        }

        private void errh_approvedcomments_TextChanged(object sender, EventArgs e)
        {

            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
            if (errh_approvedcomments.Text != "")
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
            //if (errh_approvedby.Text == "")
            //{
            //    errh["approvedby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}

        }

        #endregion

        private void GetHiredDate()
        {
            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
            if (!BizFunctions.IsEmpty(errh["empnum"]))
            {
                string str1 = "Select datejoined from hemph where empnum='" + errh["empnum"].ToString() + "'";
                this.dbaccess.ReadSQL("dtCHECKdatejoined", str1);

                if (this.dbaccess.DataSet.Tables["dtCHECKdatejoined"].Rows.Count > 0)
                {
                    DataRow drCHECKdatejoined = this.dbaccess.DataSet.Tables["dtCHECKdatejoined"].Rows[0];
                    txt_employmentdate.Text = Convert.ToDateTime(drCHECKdatejoined["datejoined"]).ToShortDateString();
                }

            }

        }

        private decimal GetCurrentSal()
        {
            decimal TotalS = 0;
            DataRow errh = this.dbaccess.DataSet.Tables["errh"].Rows[0];
            if (!BizFunctions.IsEmpty(errh["empnum"]))
            {
                string str1 = "Select sum(rateamt) as TotalSalary from pfmsr where empnum='" + errh["empnum"].ToString() + "'";
                this.dbaccess.ReadSQL("dtTotalSalary", str1);

                if (this.dbaccess.DataSet.Tables["dtTotalSalary"].Rows.Count > 0)
                {
                    DataRow drTotalSalary = this.dbaccess.DataSet.Tables["dtTotalSalary"].Rows[0];
                    if (!BizFunctions.IsEmpty(drTotalSalary["TotalSalary"]))
                    {
                        TotalS = Convert.ToDecimal(drTotalSalary["TotalSalary"]);
                    }
                    else
                    {
                        TotalS = 0;
                    }
                }
            }

            return TotalS;
        }        
    }
}
    

