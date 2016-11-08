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

namespace ATL.CFR
{
    public class Voucher_CFR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        //UserAuthorization sa = null;
        protected TextBox cfrh_confirmedcomments, cfrh_confirmedby, cfrh_apprdbyreason, cfrh_approvedby, txt_employmentdate, cfrh_empnum = null;
        protected GroupBox grb_cfrhapprinfo;
        protected RadioButton rad_Recd, rad_NotRecd, rad_Appr, rad_NotAppr = null;
        protected CheckBox hasConfirmationDate = null;
        protected DateTimePicker cfrh_confirmationdate = null;
        protected string headerFormName, RecommendedBy, ApprovedBy = null;

        #endregion

        #region Construct

        public Voucher_CFR(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_CFR.xml", moduleName, voucherBaseHelpers)
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
            //sa = new UserAuthorization(this.moduleName.ToString());
            Initialise();

            e.DBAccess.DataSet.Tables["cfr1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CFR1_ColumnChanged);
        }

        #endregion

        #region Initialise Components
        private void Initialise()
        {
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
            cfrh_confirmedcomments = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cfrh_confirmedcomments") as TextBox;
            cfrh_confirmedby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cfrh_confirmedby") as TextBox;
            cfrh_apprdbyreason = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cfrh_apprdbyreason") as TextBox;
            cfrh_approvedby = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cfrh_approvedby") as TextBox;
            cfrh_empnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cfrh_empnum") as TextBox;
            cfrh_empnum.Leave +=new EventHandler(cfrh_empnum_Leave);

            txt_employmentdate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_employmentdate") as TextBox;

            grb_cfrhapprinfo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "grb_cfrhapprinfo") as GroupBox;

            rad_Recd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Recd") as RadioButton;
            rad_NotRecd = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotRecd") as RadioButton;
            rad_Appr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_Appr") as RadioButton;
            rad_NotAppr = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_NotAppr") as RadioButton;

            hasConfirmationDate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cfrh_hasConfirmationDate") as CheckBox;

            hasConfirmationDate.CheckedChanged +=new EventHandler(hasConfirmationDate_CheckedChanged);


            cfrh_confirmationdate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "cfrh_confirmationdate") as DateTimePicker;

            rad_Recd.CheckedChanged += new EventHandler(rad_Recd_CheckedChanged);
            rad_NotRecd.CheckedChanged += new EventHandler(rad_NotRecd_CheckedChanged);
            rad_Appr.CheckedChanged += new EventHandler(rad_Appr_CheckedChanged);
            rad_NotAppr.CheckedChanged += new EventHandler(rad_NotAppr_CheckedChanged);

            cfrh_confirmedcomments.TextChanged += new EventHandler(trqh_recommendedcomments_TextChanged);
            cfrh_apprdbyreason.TextChanged += new EventHandler(trqh_approvedcomments_TextChanged);

            LoadRadioButtonsData();
            EmptyRecommendation();
            //if (!sa.ApprovePermission)
            //{
            //    grb_cfrhapprinfo.Enabled = false;
            //}
            //else
            //{
            //    grb_cfrhapprinfo.Enabled = true;
            //}

            GetHiredDate();

            if (BizFunctions.IsEmpty(cfrh["hasConfirmationDate"]))
            {
                cfrh["hasConfirmationDate"] = 0;
                cfrh_confirmationdate.Enabled = false;

            }
            else
            {
                if (!(bool)cfrh["hasConfirmationDate"])
                {
                    cfrh_confirmationdate.Enabled = false;
                }
            }

            if (!BizFunctions.IsEmpty(cfrh["empnum"]))
            {
                string statuso = Common.DEFAULT_DOCUMENT_STATUSO;
                string statusp = Common.DEFAULT_DOCUMENT_STATUSP;
                string cfrhstatus = cfrh["status"].ToString();

                if (cfrhstatus == statuso || cfrhstatus == statusp)
                {
                    cfrh_empnum.Enabled = false;
                }
            }
        }

        #endregion

        private void cfrh_empnum_Leave(object sender, EventArgs e)
        {
            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            if (cfrh_empnum.Text != String.Empty)
            {
                DataTable tmpEmpData = BizLogicTools.Tools.GetCommonEmpData(cfrh_empnum.Text);
                if (tmpEmpData.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in tmpEmpData.Rows)
                    {
                        cfrh["empnum"] = dr1["empnum"];
                        cfrh["empname"] = dr1["empname"];
                        cfrh["matnum"] = dr1["matnum"];
                        cfrh["sitenum"] = dr1["sitenum"];
                        cfrh["sectorcode"] = dr1["sectorcode"];
                        cfrh["employmentdate"] = dr1["datejoined"];
                        cfrh["currentsal"] = dr1["currentsalary"];
                        cfrh["statuscode"] = dr1["statuscode"];
                        cfrh["newsitenum"] = dr1["sitenum"];
                        GetHiredDate();
                    }

                    GetCurrentSalLists();
                    TotalCFR1SalaryLists();
                }

            }
            
        }

        private void hasConfirmationDate_CheckedChanged(object sender, EventArgs e)
        {
            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            if (hasConfirmationDate.Checked)
            {
                cfrh_confirmationdate.Enabled = true;
            }
            else
            {
                //cfrh_confirmationdate.Text = String.Empty;
                //cfrh["confirmationdate"] = System.DBNull.Value;
                cfrh_confirmationdate.Enabled = false;
                
                
            }
        }

        #region Load Radio Button Data

        private void LoadRadioButtonsData()
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];

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

            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];

            if (CheckIFhasExistingCFR())
            {
                MessageBox.Show("Emp No. '" + cfrh["empnum"].ToString() + "' has already a CFR record.", "Pico Guards Ltd. Pte.",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                e.Handle = false;
            }

            if (!(bool)cfrh["hasConfirmationDate"])
            {
                cfrh["confirmationdate"] = System.DBNull.Value;
              

            }

            if (BizFunctions.IsEmpty(cfrh["confirmationdate"]))
            {
                MessageBox.Show("Confirmation Date is Empty", "Pico Guards Ltd. Pte.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handle = false;
            }
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
            switch (e.ControlName)
            {
                    
                case "cfrh_empnum":
                    {

                        e.Condition = BizFunctions.F2Condition("empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "empnum like '" + cfrh_empnum.Text.Trim() + "%' OR empname like '" + cfrh_empnum.Text.Trim() + "%'";

                    }
                    break;

                case "cfrh_confirmedby":
                    {
                        if (!BizFunctions.IsEmpty(cfrh["sectorcode"]))
                        {
                            e.DefaultCondition = "SECTORCODE like '%" + cfrh["sectorcode"].ToString() + "%'";

                        }

                    }
                    break;

                case "cfrh_approvedby":
                    {
                        if (!BizFunctions.IsEmpty(cfrh["sectorcode"]))
                        {
                            e.DefaultCondition = "sectorcode like '%" + cfrh["sectorcode"].ToString() + "%'";
                        }

                    }
                    break;
          
            }
        }


        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            switch (e.ControlName)
            {
                case "cfrh_empnum":
                    {
                        if (!BizFunctions.IsEmpty(cfrh["empnum"]))
                        {
                            e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                            e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                            e.CurrentRow["sitenum"] = e.F2CurrentRow["sitenum"];
                            e.CurrentRow["newsitenum"] = e.F2CurrentRow["sitenum"];
                            e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                            e.CurrentRow["currentsal"] = GetCurrentSal();
                            GetHiredDate();
                            GetCurrentSalLists();
                            TotalCFR1SalaryLists();
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

            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            try
            {

                if (cfrh["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSO)
                {
                    //DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                    // Delete this current refnum first.	
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + cfrh["refnum"].ToString() + "'");
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + cfrh["refnum"].ToString() + "'");

                    UpdatePFMSRonVoid();
                }



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
            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];

            try
            {
               
                    if (cfrh["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSV)
                    {
                        DataRow drSalcode = this.dbaccess.DataSet.Tables["dtSalcode"].Rows[0];
                        // Delete this current refnum first.	
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from PFMSR where refnum='" + cfrh["refnum"].ToString() + "'");
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from SALH  where refnum='" + cfrh["refnum"].ToString() + "'");

                        UpdatePFMSRonVoid();
                    }

                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
       
        }

        #endregion

        private void UpdatePFMSRonVoid()
        {
            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            string salcode = "";

            string GetLatestSALH = "Select * from " +
                                    "( " +
                                    "Select ROW_NUMBER() OVER (Order BY dateadjusted) as ForTop,ROW_NUMBER() OVER (Order BY dateadjusted Desc) as ForBottom, * " +
                                    "from " +
                                        "( " +
                                        "select refnum, salcode,empnum,matnum,SUM(rateamt) as TotalAmt,dateadjusted,adjustmentflag " +
                                        "from SALH " +
                                        "where empnum='" + cfrh["empnum"].ToString() + "' and status<>'V' " +
                                        "group by refnum, salcode,empnum,matnum,nric,dateadjusted,adjustmentflag " +
                                        ")a " +
                                    ")b " +
                                    "where ForBottom = 1";

            this.dbaccess.ReadSQL("dtGetLatestSALH", GetLatestSALH);

            DataTable dtGetLatestSALH = this.dbaccess.DataSet.Tables["dtGetLatestSALH"];


            string strNRIC = BizLogicTools.Tools.GetNRIC(cfrh["empnum"].ToString());


            if (dtGetLatestSALH.Rows.Count > 0)
            {
                DataRow drGetLatestSALH = this.dbaccess.DataSet.Tables["dtGetLatestSALH"].Rows[0];

                //create empty PFMSR datatable
                string strpfmsr = "Select * from pfmsr where 1=2 ";
                this.dbaccess.ReadSQL("PFMSR", strpfmsr);
                DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];

                //string GetSALHdata = "Select * from salh where salcode='" + drGetLatestSALH["salcode"].ToString() + "'";

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
                            InsertPfmsr["AdjustmentFlag"] = dr1["AdjustmentFlag"];
                            InsertPfmsr["SADJmcode"] = dr1["SADJmcode"];
                            InsertPfmsr["line"] = dr1["line"];
                            InsertPfmsr["sitenum"] = dr1["sitenum"];
                            pfmsr.Rows.Add(InsertPfmsr);

                        }
                    }
                }


                #region PFMSR
                decimal Line1 = 0;
                foreach (DataRow dr2 in pfmsr.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(cfrh, dr2, "user/status/created/modified");
                        
                        //Line1 = Line1 + 100;
                        //dr2["line"] = Line1;
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
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM pfmsr WHERE empnum = '" + cfrh["empnum"].ToString() + "'");

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

            DataRow cfrh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            DataTable cfr1 = this.dbaccess.DataSet.Tables["CFR1"];




            //if (ApprovedBy != string.Empty)
            //{
            //    cfrh["approvedby"] = RecommendedBy;
            //}
            //if (RecommendedBy != string.Empty)
            //{
            //    cfrh["confirmedby"] = ApprovedBy;
            //}
           
                                  
        }
        #endregion

        #region if Appraisal Comments is Empty

        private void EmptyRecommendation()
        {
            if (cfrh_confirmedcomments.Text == "")
            {
                cfrh_apprdbyreason.Enabled = false;
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;
            }
        }

        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
            DataTable cfr1 = this.dbaccess.DataSet.Tables["CFR1"];

            if (!BizFunctions.IsEmpty(cfrh["statuscode"]))
            {
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set statuscode = '" + cfrh["statuscode"].ToString() + "' where empnum='" + cfrh["empnum"].ToString() + "' ");
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set sitenum = '" + cfrh["newsitenum"].ToString() + "' where empnum='" + cfrh["empnum"].ToString() + "' ");
            }

            string strsalh = "Select * from salh where 1=2";
            string strpfmsr = "Select * from pfmsr where 1=2 ";
            this.dbaccess.ReadSQL("SALH", strsalh);
            this.dbaccess.ReadSQL("PFMSR", strpfmsr);

            DataTable salh = this.dbaccess.DataSet.Tables["SALH"];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];

            string strNRIC = BizLogicTools.Tools.GetNRIC(cfrh["empnum"].ToString());

            if (cfrh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                BizFunctions.DeleteAllRows(salh);
                foreach (DataRow dr12 in cfr1.Rows)
                {
                    if (dr12.RowState != DataRowState.Deleted)
                    {

                        DataRow InsertSalh = salh.NewRow();
                        InsertSalh["refnum"] = cfrh["refnum"];
                        InsertSalh["empnum"] = cfrh["empnum"];
                        InsertSalh["nric"] = strNRIC;
                        InsertSalh["matnum"] = cfrh["matnum"];
                        InsertSalh["rateamt"] = dr12["rateamt"];
                        InsertSalh["hsamcode"] = dr12["hsamcode"];
                        InsertSalh["remarks"] = dr12["remarks"];
                        InsertSalh["dateadjusted"] = cfrh["confirmationdate"];
                        InsertSalh["hsgcode"] = dr12["hsgcode"];
                        InsertSalh["AdjustmentFlag"] = dr12["Flag"];
                        InsertSalh["SADJmcode"] = "CFR";
                        InsertSalh["salcode"] = cfrh["refnum"];
                        InsertSalh["line"] = dr12["line"];
                        InsertSalh["sitenum"] = cfrh["newsitenum"];
                        InsertSalh["status"] = Common.DEFAULT_DOCUMENT_STATUSP;
                        salh.Rows.Add(InsertSalh);
                    }
                }


                #region SALH
                decimal lineNo1 = 0;
                foreach (DataRow dr13 in salh.Rows)
                {
                    if (dr13.RowState != DataRowState.Deleted)
                    {
                        BizFunctions.UpdateDataRow(cfrh, dr13, "empnum/refnum/user/flag/status/created/modified");

                        //if (BizFunctions.IsEmpty(dr13["line"]) || (decimal)dr13["line"] <= 0)
                        //{
                        //    lineNo1 = lineNo1 + 100;
                        //    dr13["line"] = lineNo1;
                        //}
                    }

                }
                #endregion


                foreach (DataRow dr14 in cfr1.Rows)
                {
                    if (dr14.RowState != DataRowState.Deleted)
                    {
                        DataRow Insertpfmsr = pfmsr.NewRow();
                        Insertpfmsr["refnum"] = dr14["refnum"];
                        Insertpfmsr["empnum"] = cfrh["empnum"];
                        Insertpfmsr["nric"] = strNRIC;
                        Insertpfmsr["rateamt"] = dr14["rateamt"];
                        Insertpfmsr["hsamcode"] = dr14["hsamcode"];
                        Insertpfmsr["remarks"] = dr14["remarks"];
                        Insertpfmsr["dateadjusted"] = cfrh["confirmationdate"];
                        Insertpfmsr["hsgcode"] = dr14["hsgcode"];
                        Insertpfmsr["salcode"] = cfrh["refnum"];
                        Insertpfmsr["flag"] = cfrh["flag"];
                        Insertpfmsr["AdjustmentFlag"] = dr14["Flag"];
                        Insertpfmsr["SADJmcode"] = "CFR";
                        Insertpfmsr["myline"] = dr14["myline"];
                        Insertpfmsr["line"] = dr14["line"];
                        Insertpfmsr["sitenum"] = cfrh["newsitenum"];
                        Insertpfmsr["docunum"] = cfrh["refnum"];
                        pfmsr.Rows.Add(Insertpfmsr);
                    }
                }

                //#region PFMSR
                //decimal lineNo2 = 0;
                //foreach (DataRow dr15 in pfmsr.Rows)
                //{
                //    if (dr15.RowState != DataRowState.Deleted)
                //    {
                //        BizFunctions.UpdateDataRow(cfrh, dr15, "user/status/created/modified");

                //        if (BizFunctions.IsEmpty(dr15["line"]) || (decimal)dr15["line"] <= 0)
                //        {
                //            lineNo2 = lineNo2 + 100;
                //            dr15["line"] = lineNo2;
                //        }

                //    }

                //}
                //#endregion

               
                    if (!BizFunctions.IsEmpty(cfrh["isapproved"]) && (bool)cfrh["isapproved"])
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
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM salh WHERE refnum = '" + cfrh["refnum"].ToString() + "'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM pfmsr WHERE empnum = '" + cfrh["empnum"].ToString() + "'");

                            this.dbaccess.Update(dataTablestemp);


                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from salh) where tablename = 'SALH'");
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from pfmsr) where tablename = 'pfmsr'");
                            //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE HEMPH set statuscode = '" + cfrh["statuscode"].ToString() + "' where empnum='" + cfrh["empnum"].ToString() + "' ");


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

        #region Schedule Radio Button Methods
        private void rad_Recd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Recd.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];

                trqh["isrecommended"] = 1;
            }

        }

        private void rad_NotRecd_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotRecd.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];

                trqh["isrecommended"] = 0;
            }
        }

        private void rad_Appr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Appr.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];

                trqh["isapproved"] = 1;
            }
        }

        private void rad_NotAppr_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_NotAppr.Checked)
            {
                DataRow trqh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];

                trqh["isapproved"] = 0;
            }
        }
        #endregion

        #region TextChanged Events

        protected void trqh_recommendedcomments_TextChanged(object sender, EventArgs e)
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            if (cfrh_confirmedcomments.Text != "")
            {
                cfrh_apprdbyreason.Enabled = true;
                rad_Recd.Enabled = true;
                rad_NotRecd.Enabled = true;

                RecommendedBy =  Common.DEFAULT_SYSTEM_USERNAME;
            }
            else
            {
                cfrh_apprdbyreason.Enabled = false;
                rad_Recd.Enabled = false;
                rad_NotRecd.Enabled = false;

            }
            //if (cfrh_confirmedby.Text == "")
            //{
            //    trqh["confirmedby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}
        }

        private void trqh_approvedcomments_TextChanged(object sender, EventArgs e)
        {
            DataRow trqh = this.dbaccess.DataSet.Tables["cfrh"].Rows[0];
            if (cfrh_apprdbyreason.Text != "")
            {
                rad_Appr.Enabled = true;
                rad_NotAppr.Enabled = true;
                ApprovedBy =  Common.DEFAULT_SYSTEM_USERNAME;
            }
            else
            {
                rad_Appr.Enabled = false;
                rad_NotAppr.Enabled = false;
            }
            //if (cfrh_approvedby.Text == "")
            //{
            //    trqh["approvedby"] = Common.DEFAULT_SYSTEM_USERNAME;
            //}
        }

        #endregion

        private bool CheckIFhasExistingCFR()
        {
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
            bool hasRecord = false;

            if (!BizFunctions.IsEmpty(cfrh["empnum"]))
            {
                string str1 = "Select * from cfrh where empnum='" + cfrh["empnum"].ToString() + "' and refnum<>'" + cfrh["refnum"].ToString() + "'";
                this.dbaccess.ReadSQL("dtCHECKcfr", str1);

                if (this.dbaccess.DataSet.Tables["dtCHECKcfr"].Rows.Count > 0)
                {
                    hasRecord = true;
                }
                else
                {
                    hasRecord = false;
                }
            }
            this.dbaccess.DataSet.Tables["dtCHECKcfr"].Dispose();
            return hasRecord;
        }

        private void GetHiredDate()
        {
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
            if (!BizFunctions.IsEmpty(cfrh["empnum"]))
            {
                string str1 = "Select datejoined from hemph where empnum='" + cfrh["empnum"].ToString() + "'";
                this.dbaccess.ReadSQL("dtCHECKdatejoined", str1);

                if (this.dbaccess.DataSet.Tables["dtCHECKdatejoined"].Rows.Count > 0)
                {
                    DataRow  drCHECKdatejoined = this.dbaccess.DataSet.Tables["dtCHECKdatejoined"].Rows[0];
                    if (!BizFunctions.IsEmpty(drCHECKdatejoined["datejoined"]))
                    {
                        txt_employmentdate.Text = Convert.ToDateTime(drCHECKdatejoined["datejoined"]).ToShortDateString();
                    }
                }

            }

        }

        private decimal GetCurrentSal()
        {
            decimal TotalS = 0;
             DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
             if (!BizFunctions.IsEmpty(cfrh["empnum"]))
             {
                 string str1 = "Select sum(rateamt) as TotalSalary from pfmsr where empnum='" + cfrh["empnum"].ToString() + "'";
                 this.dbaccess.ReadSQL("dtTotalSalary", str1);

                 if (this.dbaccess.DataSet.Tables["dtTotalSalary"].Rows.Count > 0)
                 {
                     DataRow drTotalSalary = this.dbaccess.DataSet.Tables["dtTotalSalary"].Rows[0];
                     if(BizFunctions.IsEmpty(drTotalSalary["TotalSalary"]))
                     {
                         drTotalSalary["TotalSalary"]=0;
                     }
                     TotalS = Convert.ToDecimal(drTotalSalary["TotalSalary"]);
                 }
             }

             return TotalS;
        }

        private void TotalCFR1SalaryLists()
        {
            decimal TotalS = 0;
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
            if (!BizFunctions.IsEmpty(cfrh["empnum"]))
            {
                string str1 = "Select sum(rateamt) as TotalSalary from cfr1";
                //this.dbaccess.ReadSQL("dtTotalSalary", str1);
                DataTable dtTotalCFR1SalaryLists = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, str1);

                if (dtTotalCFR1SalaryLists.Rows.Count > 0)
                {
                    DataRow drTotalSalary = dtTotalCFR1SalaryLists.Rows[0];
                    if (BizFunctions.IsEmpty(drTotalSalary["TotalSalary"]))
                    {
                        drTotalSalary["TotalSalary"] = 0;
                    }
                    cfrh["newsal"] = Convert.ToDecimal(drTotalSalary["TotalSalary"]);
                }
            }

       
        }

        private void GetCurrentSalLists()
        {
            
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];
            DataTable ctr1 = this.dbaccess.DataSet.Tables["CFR1"];
            if (!BizFunctions.IsEmpty(cfrh["empnum"]))
            {
                string str1 = "Select * from pfmsr where empnum='" + cfrh["empnum"].ToString() + "' and [status]<>'V'";
                this.dbaccess.ReadSQL("dtSalaryLists", str1);

                if (this.dbaccess.DataSet.Tables["dtSalaryLists"].Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(ctr1);

                    foreach (DataRow dr1 in this.dbaccess.DataSet.Tables["dtSalaryLists"].Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            DataRow InsertCTR1 = ctr1.NewRow();
                            InsertCTR1["nric"] = dr1["nric"];
                            InsertCTR1["empnum"] = dr1["empnum"];
                            InsertCTR1["saldesc"] = dr1["saldesc"];
                            InsertCTR1["rateamt"] = dr1["rateamt"];
                            InsertCTR1["hsamcode"] = dr1["hsamcode"];
                            //InsertCTR1["dateadjusted"] = dr1[""];
                            InsertCTR1["remarks"] = dr1["remarks"];
    
                            //InsertCTR1["salcode"] = dr1[""];
                            //InsertCTR1["sadjmcode"] = dr1[""];
                            //InsertCTR1["adjustmentflag"] = dr1[""];
                            InsertCTR1["line"] = dr1["line"];

                            ctr1.Rows.Add(InsertCTR1);

                        }
                    }

                }

               
            }

          
        }

        private void Voucher_CFR1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow cfrh = this.dbaccess.DataSet.Tables["CFRH"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "rateamt":
                    {
                        TotalCFR1SalaryLists();
                    }
                    break;

                case "hsamcode":
                    {
                        e.Row["adjustmentflag"] = cfrh["flag"];
                    }
                    break;
            }
        }
    }
}
    

