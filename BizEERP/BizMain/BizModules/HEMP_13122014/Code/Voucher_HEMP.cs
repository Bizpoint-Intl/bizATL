#region Namespaces
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Data.SqlTypes;
using System.Diagnostics;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;
using ATL.BizModules.CompressFolders;
using ATL.TimeUtilites;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
using ATL.BizModules.FileAcc;
using ATL.Network;
using ATL.ReportLists;
using ATL.ExtractSALHForm1;
using ATL.BizModules.RichTextEdit;
using ATL.BizModules.FileAcc2;

#endregion

namespace ATL.HEMP
{
    public class Voucher_HEMP : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        //protected DataGrid dg_wlr = null;
        DataTable xPFMSR = null;
        protected TextBox hemph_name, hemph_nric, hemph_regname, hemph_ethnicity, hemph_status,
                          hemph_padd1, hemph_padd2, hemph_padd3, hemph_ppostal, age,
                          hemph_cadd1, hemph_cadd2, hemph_cadd3, hemph_cpostal, recrempname,
                          hemph_force, hemph_rank, hemph_vocation, hemph_yearserved, txt_photourl, hemph_Country,
                          txt_TotalSal, txt_dateconfirmed, txt_dateresigned, hemph_contractend, hemph_nationality = null;
        protected CheckBox hemph_isNSmandatory, hemph_isRetired = null;
        protected Button btn_Voucher_Reports, Btn_Contract, columnButton1, columnButton2 = null;
        protected DataGrid SalHistorydatagrid, EmpHistorydatagrid, SalRecordDG, dg_warning = null;

        protected Label lbl_SalaryStatus = null;

        protected string SalCode = "";

        protected string EmpFlag = "";

        ComboBox hemph_bloodtype, hemph_maritalstatus, hemph_hsgcode, hemph_pmcode, hemph_hramdesc, hemph_paytypecode = null;
        protected DateTimePicker wpr_wprenewal, wpr_wpissuedate, wpr_wpexpiry, hemph_dob, hemph_datejoined, hemph_datejPR = null;
        protected RadioButton rad_male, rad_female = null;

        protected PictureBox pb = new PictureBox();
        protected PictureBox pb1 = new PictureBox();

        Button BtnBrowse, BtnBrowseEDU, BtnDownloadEdu, btnBrowsePermit, btnDownloadPermit, btnInsert, btnDelete = null;

        protected Hashtable selectsCollection = null;

        protected string gender, tabName, headerFormName, eduFormName, salFormname, permitsFormName, employmentrecFormName, contactsinfoFormName,
                          experienceFormName, familyFormName, incetiveFormName, cidclearancesFormName, archiveFormName, warningFormName, leaveFormName, trainingFormName = null;

        DataGrid dg_education, dg_salary, dg_permits, dg_employment, dg_contacts, dg_experience, dg_family, dg_incentive, dg_cidclearances, dg_request, dg_leaverec = null;

        protected int pfprRowNum = -1;

        protected bool isRefreshed, fromSADJ;
        protected bool opened, isMouseClicked, enableDocSave = false;

        protected Button btnUp, pffa_btnBrowse, pffa_btnDownload;
        protected Button btnDown;
        protected DataGrid pfmsr2DG = null;

        protected string projectPath;
        protected ATL.BizModules.Tools.CRForm crpt1 = null;


        #endregion

        #region Construct

        public Voucher_HEMP(string moduleName, Hashtable voucherBaseHelpers, string hFlag)
            : base("VoucherGridInfo_HEMP.xml", moduleName, voucherBaseHelpers)
        {
            this.EmpFlag = hFlag;
        }
        #endregion Construct

        #region Delete On Click

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);

        }
        #endregion

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.eduFormName = (e.FormsCollection["education"] as Form).Name;
            this.salFormname = (e.FormsCollection["salaryrec"] as Form).Name;
            this.permitsFormName = (e.FormsCollection["permitslicences"] as Form).Name;
            //this.employmentrecFormName = (e.FormsCollection["employmentrec"] as Form).Name;

            //this.contactsinfoFormName = (e.FormsCollection["contactinfos"] as Form).Name;
            this.experienceFormName = (e.FormsCollection["experience"] as Form).Name;
            this.familyFormName = (e.FormsCollection["family"] as Form).Name;
            //this.incetiveFormName = (e.FormsCollection["incentive"] as Form).Name;
            //this.cidclearancesFormName =  (e.FormsCollection["cidclearances"] as Form).Name;
            this.archiveFormName = (e.FormsCollection["archive"] as Form).Name;
            this.warningFormName = (e.FormsCollection["warning"] as Form).Name;

            this.leaveFormName = (e.FormsCollection["leaverec"] as Form).Name;
            this.trainingFormName = (e.FormsCollection["request"] as Form).Name;


            DataGrid dg_education = (DataGrid)BizXmlReader.CurrentInstance.GetControl(eduFormName, "dg_Education");

            foreach (DataGridTableStyle dataGridTableStyle1 in dg_education.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle1 in dataGridTableStyle1.GridColumnStyles)
                {
                    if (dataGridColumnStyle1 is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn1 = dataGridColumnStyle1 as BizDataGridTextBoxColumn;
                        bizDataGridTextBoxColumn1.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
                    }
                }
            }

            //////////////////////////////////////////////
            DataGrid dg_salary = (DataGrid)BizXmlReader.CurrentInstance.GetControl(salFormname, "dg_Salaryrec");

            foreach (DataGridTableStyle dataGridTableStyle2 in dg_salary.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle2 in dataGridTableStyle2.GridColumnStyles)
                {
                    if (dataGridColumnStyle2 is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn2 = dataGridColumnStyle2 as BizDataGridTextBoxColumn;
                        bizDataGridTextBoxColumn2.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
                    }
                }
            }

            //////////////////////////////////////////////
            DataGrid dg_permits = (DataGrid)BizXmlReader.CurrentInstance.GetControl(permitsFormName, "dg_Permitslicences");

            foreach (DataGridTableStyle dataGridTableStyle3 in dg_permits.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle3 in dataGridTableStyle3.GridColumnStyles)
                {
                    if (dataGridColumnStyle3 is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn3 = dataGridColumnStyle3 as BizDataGridTextBoxColumn;
                        bizDataGridTextBoxColumn3.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
                    }
                }
            }

            //////////////////////////////////////////////
            //DataGrid dg_employment = (DataGrid)BizXmlReader.CurrentInstance.GetControl(eduFormName, "dg_Education");

            //foreach (DataGridTableStyle dataGridTableStyle1 in dg_education.TableStyles)
            //{
            //    foreach (DataGridColumnStyle dataGridColumnStyle1 in dataGridTableStyle1.GridColumnStyles)
            //    {
            //        if (dataGridColumnStyle1 is BizDataGridTextBoxColumn)
            //        {
            //            BizDataGridTextBoxColumn bizDataGridTextBoxColumn1 = dataGridColumnStyle1 as BizDataGridTextBoxColumn;
            //            bizDataGridTextBoxColumn1.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
            //        }
            //    }
            //}

            //////////////////////////////////////////////
            //DataGrid dg_contacts = (DataGrid)BizXmlReader.CurrentInstance.GetControl(contactsinfoFormName, "dg_contactinfos");

            //foreach (DataGridTableStyle dataGridTableStyle4 in dg_contacts.TableStyles)
            //{
            //    foreach (DataGridColumnStyle dataGridColumnStyle4 in dataGridTableStyle4.GridColumnStyles)
            //    {
            //        if (dataGridColumnStyle4 is BizDataGridTextBoxColumn)
            //        {
            //            BizDataGridTextBoxColumn bizDataGridTextBoxColumn4 = dataGridColumnStyle4 as BizDataGridTextBoxColumn;
            //            bizDataGridTextBoxColumn4.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
            //        }
            //    }
            //}

            //////////////////////////////////////////////
            DataGrid dg_experience = (DataGrid)BizXmlReader.CurrentInstance.GetControl(experienceFormName, "dg_Experience");

            foreach (DataGridTableStyle dataGridTableStyle5 in dg_experience.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle5 in dataGridTableStyle5.GridColumnStyles)
                {
                    if (dataGridColumnStyle5 is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn5 = dataGridColumnStyle5 as BizDataGridTextBoxColumn;
                        bizDataGridTextBoxColumn5.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
                    }
                }
            }

            //////////////////////////////////////////////
            DataGrid dg_family = (DataGrid)BizXmlReader.CurrentInstance.GetControl(familyFormName, "dg_Family");

            foreach (DataGridTableStyle dataGridTableStyle6 in dg_family.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle6 in dataGridTableStyle6.GridColumnStyles)
                {
                    if (dataGridColumnStyle6 is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn6 = dataGridColumnStyle6 as BizDataGridTextBoxColumn;
                        bizDataGridTextBoxColumn6.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
                    }
                }
            }

            //////////////////////////////////////////////
            //DataGrid dg_incentive = (DataGrid)BizXmlReader.CurrentInstance.GetControl(incetiveFormName, "dg_Incentive");

            //foreach (DataGridTableStyle dataGridTableStyle7 in dg_incentive.TableStyles)
            //{
            //    foreach (DataGridColumnStyle dataGridColumnStyle7 in dataGridTableStyle7.GridColumnStyles)
            //    {
            //        if (dataGridColumnStyle7 is BizDataGridTextBoxColumn)
            //        {
            //            BizDataGridTextBoxColumn bizDataGridTextBoxColumn7 = dataGridColumnStyle7 as BizDataGridTextBoxColumn;
            //            bizDataGridTextBoxColumn7.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
            //        }
            //    }
            //}


            //DataGrid dg_cidclearances = (DataGrid)BizXmlReader.CurrentInstance.GetControl(cidclearancesFormName, "dg_cidclearances");

            //foreach (DataGridTableStyle dataGridTableStyle8 in dg_cidclearances.TableStyles)
            //{
            //    foreach (DataGridColumnStyle dataGridColumnStyle8 in dataGridTableStyle8.GridColumnStyles)
            //    {
            //        if (dataGridColumnStyle8 is BizDataGridTextBoxColumn)
            //        {
            //            BizDataGridTextBoxColumn bizDataGridTextBoxColumn8 = dataGridColumnStyle8 as BizDataGridTextBoxColumn;
            //            bizDataGridTextBoxColumn8.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
            //        }
            //    }
            //}


            /////////



            //foreach (DataGridTableStyle dataGridTableStyle8 in dg_leaverec.TableStyles)
            //{
            //    foreach (DataGridColumnStyle dataGridColumnStyle8 in dataGridTableStyle8.GridColumnStyles)
            //    {
            //        if (dataGridColumnStyle8 is BizDataGridTextBoxColumn)
            //        {
            //            BizDataGridTextBoxColumn bizDataGridTextBoxColumn8 = dataGridColumnStyle8 as BizDataGridTextBoxColumn;
            //            bizDataGridTextBoxColumn8.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
            //        }
            //    }
            //}



            //foreach (DataGridTableStyle dataGridTableStyle8 in dg_request.TableStyles)
            //{
            //    foreach (DataGridColumnStyle dataGridColumnStyle8 in dataGridTableStyle8.GridColumnStyles)
            //    {
            //        if (dataGridColumnStyle8 is BizDataGridTextBoxColumn)
            //        {
            //            BizDataGridTextBoxColumn bizDataGridTextBoxColumn8 = dataGridColumnStyle8 as BizDataGridTextBoxColumn;
            //            bizDataGridTextBoxColumn8.TextBoxGrid.CharacterCasing = CharacterCasing.Upper;
            //        }
            //    }
            //}


            opened = true;

            this.selectsCollection = new Hashtable();

            DataRow hemph = e.DBAccess.DataSet.Tables["hemph"].Rows[0];
            DataTable pfmedu = e.DBAccess.DataSet.Tables["pfmedu"];
            DataTable recr = e.DBAccess.DataSet.Tables["recr"];
            DataTable wpr = e.DBAccess.DataSet.Tables["wpr"];
            DataTable pfmwe = e.DBAccess.DataSet.Tables["pfmwe"];
            DataTable pfmer = e.DBAccess.DataSet.Tables["pfmer"];
            DataTable pfmsr = e.DBAccess.DataSet.Tables["pfmsr"];
            DataTable famr = e.DBAccess.DataSet.Tables["famr"];
            DataTable plr = e.DBAccess.DataSet.Tables["plr"];
            DataTable hcnr = e.DBAccess.DataSet.Tables["hcnr"];
            DataTable pfpr = e.DBAccess.DataSet.Tables["pfpr"];

            if (BizFunctions.IsEmpty(hemph["datejoined"]))
            {
                hemph["datejoined"] = System.DBNull.Value;
            }

            string GetHETM = "Select * from HETM where status<>'V'";
            this.dbaccess.ReadSQL("HETM", GetHETM);

            e.DBAccess.DataSet.Tables["pfmsr"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PFSR_ColumnChanged);
            e.DBAccess.DataSet.Tables["famr"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_FAMR_ColumnChanged);
            e.DBAccess.DataSet.Tables["pfmedu"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PFMEDU_ColumnChanged);



            Initialise();
            this.ShowImage();

            if (pfmsr.Rows.Count <= 0)
            {
                string GetHSAM = "";

                if (!BizFunctions.IsEmpty(hemph["empnum"]))
                {
                    if (hemph["empnum"].ToString().Contains("F"))
                    {
                        GetHSAM = "Select * from HSAM where [status]<>'V' AND hsamcode in ('BASIC','OT(1.5)','ATTNINCENTIVE','MEALREIMB','LAUNDRY','SUPALLW') order by line";
                    }
                    else if (hemph["empnum"].ToString().Contains("SP"))
                    {
                        GetHSAM = "Select * from HSAM where [status]<>'V' AND hsamcode in ('BASIC','OT(1.5)','ATTNALLW','MEALREIMB','LAUNDRY','TPTREIMB','HARDSHIPALLW') order by line";
                    }
                    else if (hemph["empnum"].ToString().Contains("P"))
                    {
                        GetHSAM = "Select * from HSAM where [status]<>'V' AND hsamcode in ('BASIC','OT(1.5)','ATTNALLW','MEALREIMB','LAUNDRY','TPTREIMB','HARDSHIPALLW') order by line";
                    }
                    else
                    {
                        GetHSAM = "Select * from HSAM where [status]<>'V' order by line";
                    }

                    this.selectsCollection.Add("HSAM", GetHSAM);
                    this.dbaccess.ReadSQL(selectsCollection);

                    if (this.dbaccess.DataSet.Tables["HSAM"].Rows.Count > 0)
                    {
                        DataTable hsam = this.dbaccess.DataSet.Tables["HSAM"];

                        foreach (DataRow dr2 in hsam.Rows)
                        {
                            if (dr2.RowState != DataRowState.Deleted)
                            {
                                DataRow Insertpfmsr = pfmsr.NewRow();
                                Insertpfmsr["hsamcode"] = dr2["hsamcode"];
                                Insertpfmsr["myline"] = dr2["line"];
                                Insertpfmsr["line"] = dr2["line"];
                                pfmsr.Rows.Add(Insertpfmsr);

                            }
                        }
                    }
                }

            }

            if (pfmsr.Rows.Count > 0)
            {
                GetTotalSalary();
            }


            SalHistorydatagrid = BizXmlReader.CurrentInstance.GetControl(salFormname, "dg_SalaryHistory") as DataGrid;

            //SalHistorydatagrid = BizXmlReader.CurrentInstance.GetControl(salFormname, "dg_timesheetsummary") as DataGrid;
            SalHistorydatagrid.MouseDoubleClick += new MouseEventHandler(SalHistorydatagrid_MouseDoubleClick);


            //EmpHistorydatagrid = BizXmlReader.CurrentInstance.GetControl(employmentrecFormName, "dg_Employmentrec") as DataGrid;


            dg_leaverec = (DataGrid)BizXmlReader.CurrentInstance.GetControl(leaveFormName, "dg_leaverec");
            dg_request = (DataGrid)BizXmlReader.CurrentInstance.GetControl(trainingFormName, "dg_request");


            btnInsert = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Insert") as Button;
            btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Delete") as Button;

            GetSalaryHistory();
            //GetEmploymentHistory();
            GetLeavetHistory();
            if (!BizFunctions.IsEmpty(hemph["datejoined"]))
            {
                if (!GetConfirmationDate())
                {
                    DateTime dt = Convert.ToDateTime(hemph["datejoined"]).AddMonths(3);
                    if (!hemph["empnum"].ToString().Contains("P"))
                    {
                        txt_dateconfirmed.Text = dt.ToShortDateString();
                    }
                    else
                    {
                        txt_dateconfirmed.Text = string.Empty;
                    }
                }
            }

            if (BizFunctions.IsEmpty(hemph["isretired"]))
            {
                hemph["isretired"] = 0;
            }

            //(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, Common.DEFAULT_DOCUMENT_BTNPREVIEW) as Button).Enabled = false;
            //(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, Common.DEFAULT_DOCUMENT_BTNPRINT) as Button).Enabled = true;
            ////(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Report") as Button).Enabled = true;
            ////(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Report") as Button).Visible = true;
            //btn_Voucher_Reports = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Report") as Button;


            //btn_Voucher_Reports.Click+=new EventHandler(btn_Voucher_Reports_Click);



            decimal lineNo1 = 0;


            foreach (DataRow dr1 in pfmsr.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr1["Line"]))
                    {
                        lineNo1 = lineNo1 + 100;
                        dr1["line"] = lineNo1;
                    }

                }
            }

            if (BizFunctions.IsEmpty(hemph["Country"]))
            {
                hemph["country"] = "SINGAPORE";
            }

            if (BizFunctions.IsEmpty(hemph["contractdoc"]))
            {
                string varBinary = "0x504B0304140002000800976C904258470FF02F020000B20400000B000000636F6E74656E742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB767B5A2D5BFA99D2CBCBE651D35E9739B5AE978FAAAC299A47CB6C91378FDAE9A36A952FCD4B8FBCC68FB823F9A0CDDFB5B77D196DFD77CFABDBBEF9AE29B7CF2B427CB1CADA6212A2D05C5EDC160E351D0032ABB3ABDB42415BA2B0FFFABBB258BEFDECA379DBAE1EDDBD7B757535BEBA37AEEA8BBBBB0F1F3EBCCBDF9AA65575651B02B8CC0E37DEDBD9D9BF7B55176D5E5BFA6693DBCF0E37F6D112D8B77D5DF9C4A7CAD4A2BA5AD72523399BDECDCB1CAF357777C7BB773FB20C764E7CB57D9E4DF3ED593E2D9BF4AEFD265BB7D582883EDD66066A8E1E0B23F1BFA9FC0ECC3EFBE835F52D7F9F678BA2BCFEECA3269FB6C4DE1FD977E4EFED554D43A8DB22E78EEE7A0087A1BFDCED425F6575765167ABB9F9823E808CF01FDBF2D68BAA5E64A569B0C81A9A9FED557661BE7FDD66CB5956CF3E62440687AC5F4CAAD9B5FD032271F4980543C795CA1F5EF720097FA87F4ABB9D8F8E1EF3A7ABFE1B344C03749545203ECDCFB375D9A62FCDE0D36734751F1D9D2E56EDF5E3BBF6CD23FD7D657E511CDD28057FF3970CCDFCD5D53747FF0F504B0304140002000800976C9042D5F05C2BBA020000F80500000A0000007374796C65732E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB76BB69AFCBBC49E9DD65F388FFA0C6F5F251953545F368992DF2E6513B7D54ADF2A579E791D7F811F7231F9C57B77DF55D536E9F57DBD36AB1CADA621282795716CBB79F7D346FDBD5A3BB77AFAEAEC657F7C6557D7177F7E1C38777F95BD3B4B9BCB86D97D474A03FA1C86DE128FDF0BE25E67945843CCFA6F9F62C9F964D7AD77E23D43D7A2CB4E27F53F91D7D7CF6D1D3FC3C5B976DFA32ABB38B3A5BCDD36704EB236D739E2D8AF2FAB38FDAFC1D7D767718CCF36299A72FD68B495EC7DF95CF56596DA77C7B33021B3AFBF6F52AAF6516225D99F7F0C7F6AA260AD66D410C765E3D9A5665557FF6D193729D9B57B9D57A396380B960F6D9474D5516B3812657C5AC9D7FF651B66E2BC6F2AE87E630CE2FAA7A91955D845766D00C091D3D421F4B2624890D31CCF2BCB858D7C434D532E506F2E536DA359F7D749E950D0D86BFA1296FF2F6B38F76C6F7A70BFD8C5A13A353DFF4F9AEFB0C10565553B42CAA657EDEEA77C5725AE7E0360263F8135FDC823B6D5BE16D90A6C384FA274847081553957D43B55576916F97D975B56E03DA7DB15AECD869F51A756697BFD1D93918DF2F961FD94FE7797131070576F1A9022202B7DB555DD06033A1C3AAAADB3A2B5A7E7191D517C572BBAD56A0E883834F0F14A27E31A95A1A46FC3B10141A70F760E7D3F09B5A10A1297AB8730F5F390EF246E648D7A7957EB1C89A962631A4A07E084801055FB7D97296D596A57D227A34F6E7AC03DF7CDC51DB47FF0F504B0304140002000800976C90425CC0E1810501000000010000150000004D4554412D494E462F6D616E69666573742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E8F1225B16E779D33E32BFA4F4DAB2B17F52D37AF9A8CA9AA279B4CC1679F3A89D3EAA56F972564DD78B7CD93E0ADB3F42471ED8F3A2CCB7A9597D9DBAAEF259916DB7D7ABFCB38FB2D5AA2CA6594B48DEBD5CCEC6DCD3D8EF60DCE6EFDA8FDCDBE7EBB2DC5E65EDFCB38FEE7E94DE3D7A7CB73786A3FF07504B01021400140002000800976C904258470FF02F020000B20400000B0000000000000000000000000000000000636F6E74656E742E786D6C504B01021400140002000800976C9042D5F05C2BBA020000F80500000A00000000000000000000000000580200007374796C65732E786D6C504B01021400140002000800976C90425CC0E181050100000001000015000000000000000000000000003A0500004D4554412D494E462F6D616E69666573742E786D6C504B05060000000003000300B4000000720600000000";
                char[] charArray = varBinary.ToCharArray();
                byte[] byteArray = new byte[charArray.Length];

                hemph["contractdoc"] = byteArray;
            }




        }

        #endregion

        #region Salary History Mouse DoubleClick

        private void SalHistorydatagrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            DataTable EmpDGV1 = this.dbaccess.DataSet.Tables["EmpDGV1"];

            try
            {
                isMouseClicked = true;
                #region Get and Set Row

                DataGrid dataGrid = sender as DataGrid;

                int i = dataGrid.CurrentRowIndex;
                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

                if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
                {
                    dataGrid.Select(i);

                    DataRow drCur = getcurrentrow(SalHistorydatagrid);

                    ATL.ExtractSALHForm1.ExtractSALHForm1 eATR1 = new ATL.ExtractSALHForm1.ExtractSALHForm1(this.dbaccess, drCur["empnum"].ToString().Trim(), drCur["refnum"].ToString());

                    eATR1.Show();
                    eATR1.Focus();


                }
                #endregion

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region get Current Row

        private DataRow getcurrentrow(DataGrid datagrid)
        {
            CurrencyManager cm = (CurrencyManager)datagrid.BindingContext[datagrid.DataSource, datagrid.DataMember];
            DataRowView drv = cm.Current as DataRowView;
            DataRow dr = drv.Row;

            return dr;
        }

        #endregion

        #region Prevent openning of two docs
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

        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);

            opened = false;
        }

        protected override void Voucher_New_Handle(object sender, VoucherHandleEventArgs e)
        {
            base.Voucher_New_Handle(sender, e);
            //DataRow hemph = e.DBAccess.DataSet.Tables["hemph"].Rows[0];
            if (opened)
            {
                MessageBox.Show("You cannot open two documents at the same time.\n" +
                    "To Edit/New Document, either close the document that's currently open for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (BizFunctions.IsEmpty(this.VoucherBase.SearchTextBox.Text))
                {
                    //core 'thought' it is input by user
                    this.VoucherBase.SearchTextBox.Text = "NEW EMPLOYEE";
                }
            }


            e.Handle = !opened;
        }
        #endregion

        #region Document Insert Onclick

        protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Insert_OnClick(sender, e);
            DataTable pfmsr = dbaccess.DataSet.Tables["pfmsr"];

            btnUp.Enabled = true;
            btnDown.Enabled = true;
        }

        #endregion

        #region btn Voucher Reports

        private void btn_Voucher_Reports_Click(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

            if (hemph["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO || hemph["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(dbaccess.DataSet, "hemph"))
                {

                    ReportLists.Reports ReportForm = new ATL.ReportLists.Reports(false, "HEMPH", "HEMP", hemph["empnum"].ToString());

                    ReportForm.ShowDialog();
                }
            }
        }

        #endregion

        #region Column Changed Events

        private void Voucher_PFSR_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            switch (e.Column.ColumnName)
            {
                case "rateamt":
                    {
                        GetTotalSalary();
                    }
                    break;

                case "hsamcode":
                    {
                        e.Row["adjustmentflag"] = "HEMP";
                    }
                    break;
            }
        }


        private void Voucher_FAMR_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable famr = this.dbaccess.DataSet.Tables["famr"];
            switch (e.Column.ColumnName)
            {

            }
        }

        private void Voucher_PFMEDU_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable PFMEDU = this.dbaccess.DataSet.Tables["PFMEDU"];

            switch (e.Column.ColumnName)
            {
                case "isHighest":
                    {
                        if (PFMEDU.Rows.Count > 0)
                        {
                            DataTable tmpPFMEDU = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select * from PFMEDU where ISNULL(isHighest,0)=1");

                            if (tmpPFMEDU.Rows.Count > 1)
                            {
                                e.Row["isHighest"] = 0;
                            }
                        }
                    }
                    break;
            }
        }


        #endregion

        #region Initialise Components

        private void Initialise()
        {

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            DataTable xPFMSR = this.dbaccess.DataSet.Tables["pfmsr"].Copy();
            DataTable xPFPLREC = this.dbaccess.DataSet.Tables["pfplrec"].Copy();
            DataTable xpffa = this.dbaccess.DataSet.Tables["pffa"].Copy();

            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");


            if (!this.dbaccess.DataSet.Tables.Contains("xPFMSR"))
            {
                xPFMSR.TableName = "xPFMSR";
                this.dbaccess.DataSet.Tables.Add(xPFMSR.Copy());
            }

            if (!this.dbaccess.DataSet.Tables.Contains("xPFPLREC"))
            {
                xPFPLREC.TableName = "xPFPLREC";
                this.dbaccess.DataSet.Tables.Add(xPFPLREC.Copy());
            }



            if (!this.dbaccess.DataSet.Tables.Contains("xpffa"))
            {
                xpffa.TableName = "xpffa";
                this.dbaccess.DataSet.Tables.Add(xpffa.Copy());
            }

            Btn_Contract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "Btn_Contract") as Button;
            Btn_Contract.Click += new EventHandler(Btn_Contract_Click);

            lbl_SalaryStatus = BizXmlReader.CurrentInstance.GetControl(salFormname, "lbl_SalaryStatus") as Label;
            recrempname = BizXmlReader.CurrentInstance.GetControl(headerFormName, "recrempname") as TextBox;
            hemph_datejoined = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_datejoined") as DateTimePicker;
            hemph_datejoined.AllowDrop = false;

            txt_dateconfirmed = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_dateconfirmed") as TextBox;
            txt_dateresigned = BizXmlReader.CurrentInstance.GetControl(headerFormName, "txt_dateresigned") as TextBox;



            hemph_nric = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_nric") as TextBox;
            //hemph_nric.Leave +=new EventHandler(hemph_nric_Leave);

            age = BizXmlReader.CurrentInstance.GetControl(headerFormName, "age") as TextBox;
            recrempname = BizXmlReader.CurrentInstance.GetControl(headerFormName, "recrempname") as TextBox;
            hemph_nationality = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_nationality") as TextBox;

            hemph_nationality.Leave += new EventHandler(hemph_nationality_Leave);

            hemph_datejPR = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_datePR") as DateTimePicker;

            if (hemph_nationality.Text != string.Empty)
            {
                if (hemph_nationality.Text.Contains(" PR"))
                {
                    hemph_datejPR.Enabled = true;
                }
                else
                {
                    hemph_datejPR.Enabled = false;
                }
            }
            else
            {
                hemph_datejPR.Enabled = false;
            }


            //hemph_nric.DoubleClick +=new EventHandler(hemph_nric_DoubleClick);
            hemph_dob = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_dob") as DateTimePicker;
            //hemph_dob.TextChanged+=new EventHandler(hemph_dob_TextChanged);
            hemph_dob.Leave += new EventHandler(hemph_dob_Leave);
            hemph_datejoined = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_datejoined") as DateTimePicker;
            hemph_datejoined.Leave += new EventHandler(hemph_datejoined_Leave);
            hemph_name = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_name") as TextBox;
            hemph_isNSmandatory = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_isNSmandatory") as CheckBox;
            hemph_isNSmandatory.CheckStateChanged += new EventHandler(hemph_isNSmandatory_CheckStateChanged);
            hemph_regname = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_regname") as TextBox;
            hemph_maritalstatus = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_maritalstatus") as ComboBox;
            hemph_ethnicity = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_ethnicity") as TextBox;
            hemph_bloodtype = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_bloodtype") as ComboBox;
            hemph_isNSmandatory = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_isNSmandatory") as CheckBox;
            hemph_isRetired = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_isRetired") as CheckBox;
            hemph_isRetired.CheckStateChanged += new EventHandler(hemph_isRetired_CheckStateChanged);

            hemph_Country = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_Country") as TextBox;
            hemph_Country.ReadOnly = true;

            hemph_padd1 = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_padd1") as TextBox;
            hemph_padd2 = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_padd2") as TextBox;
            hemph_padd3 = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_padd3") as TextBox;
            hemph_ppostal = BizXmlReader.CurrentInstance.GetControl(headerFormName, "pfadd_ppostal") as TextBox;
            hemph_cadd1 = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_cadd1") as TextBox;
            hemph_cadd2 = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_cadd2") as TextBox;
            hemph_cadd3 = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_cadd3") as TextBox;
            hemph_cpostal = BizXmlReader.CurrentInstance.GetControl(headerFormName, "pfadd_cpostal") as TextBox;
            hemph_force = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_force") as TextBox;
            hemph_rank = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_rank") as TextBox;
            hemph_contractend = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_contractend") as TextBox;
            hemph_vocation = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_vocation") as TextBox;
            hemph_yearserved = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_yearserved") as TextBox;
            rad_male = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_male") as RadioButton;
            rad_male.CheckedChanged += new EventHandler(rad_male_CheckedChanged);
            rad_female = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_female") as RadioButton;
            rad_female.CheckedChanged += new EventHandler(rad_female_CheckedChanged);

            hemph_hsgcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_hsgcode") as ComboBox;
            hemph_hsgcode.DropDown += new EventHandler(hemph_hsgcode_DropDown);

            hemph_paytypecode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_paytypecode") as ComboBox;
            hemph_paytypecode.DropDown += new EventHandler(hemph_paytypecode_DropDown);

            hemph_pmcode = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_pmcode") as ComboBox;
            hemph_pmcode.DropDown += new EventHandler(hemph_pmcode_DropDown);

            hemph_hramdesc = BizXmlReader.CurrentInstance.GetControl(headerFormName, "hemph_hramdesc") as ComboBox;
            hemph_hramdesc.DropDown += new EventHandler(hemph_hramdesc_DropDown);

            //hemph_pmcode, hemph_hramdesc, hemph_paytypecode

            BtnBrowse = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btnBrowse") as Button;
            BtnBrowse.Click += new EventHandler(BtnBrowse_Click);
            BtnBrowseEDU = BizXmlReader.CurrentInstance.GetControl(eduFormName, "btnBrowseEdu") as Button;
            BtnBrowseEDU.Click += new EventHandler(BtnBrowseEDU_Click);
            BtnDownloadEdu = BizXmlReader.CurrentInstance.GetControl(eduFormName, "btnDownloadEdu") as Button;
            BtnDownloadEdu.Click += new EventHandler(BtnDownloadEdu_Click);

            btnBrowsePermit = BizXmlReader.CurrentInstance.GetControl(permitsFormName, "btnBrowsePermit") as Button;
            btnBrowsePermit.Click += new EventHandler(btnBrowsePermit_Click);

            btnDownloadPermit = BizXmlReader.CurrentInstance.GetControl(permitsFormName, "btnDownloadPermit") as Button;
            btnDownloadPermit.Click += new EventHandler(btnDownloadPermit_Click);

            pffa_btnBrowse = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "pffa_btnBrowse") as Button;
            pffa_btnBrowse.Click += new EventHandler(pffa_btnBrowse_Click);

            pffa_btnDownload = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "pffa_btnDownload") as Button;
            pffa_btnDownload.Click += new EventHandler(pffa_btnDownload_Click);


            pb = BizXmlReader.CurrentInstance.GetControl(headerFormName, "pictbox") as PictureBox;
            txt_photourl = (TextBox)BizXmlReader.CurrentInstance.GetControl(headerFormName, "photourl");



            txt_TotalSal = BizXmlReader.CurrentInstance.GetControl(salFormname, "txt_TotalSal") as TextBox;

            gender = hemph["Gender"].ToString().Trim();
            if (gender == "M")
            {
                rad_male.Checked = true;
                rad_female.Checked = false;
            }
            if (gender == "F")
            {
                rad_male.Checked = false;
                rad_female.Checked = true;
            }

            if (BizFunctions.IsEmpty(hemph["isNSmandatory"]))
            {
                hemph["isNSmandatory"] = 0;
            }

            if ((bool)hemph["isNSmandatory"])
            {
                hemph_isNSmandatory.Checked = true;
                //hemph["force"] = "";
                //hemph["rank"] = "";
                //hemph["vocation"] = "";
                //hemph["yearserved"] = System.DBNull.Value;

                hemph_force.Enabled = true;
                hemph_rank.Enabled = true;
                hemph_vocation.Enabled = true;
                hemph_yearserved.Enabled = true;

            }
            else
            {
                hemph["force"] = "";
                hemph["rank"] = "";
                hemph["vocation"] = "";
                hemph["yearserved"] = System.DBNull.Value;
                hemph_isNSmandatory.Checked = false;

                hemph_force.Enabled = false;
                hemph_rank.Enabled = false;
                hemph_vocation.Enabled = false;
                hemph_yearserved.Enabled = false;


            }

            //hemph_dob.Leave +=new EventHandler(hemph_dob_Leave);

            if (!BizFunctions.IsEmpty(hemph["DOB"]))
            {
                age.Text = Convert.ToString(GetAge());
            }
            hemph_maritalstatus.Text = hemph["maritalstatus"].ToString().Trim();
            hemph_hsgcode.Text = hemph["hsgcode"].ToString().Trim(); ;
            GetRecr();
            GetResignationDate();

            dg_warning = BizXmlReader.CurrentInstance.GetControl(warningFormName, "dg_warning") as DataGrid;

            foreach (DataGridTableStyle dataGridTableStyle in dg_warning.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
                {

                    if (dataGridColumnStyle is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;

                        if (bizDataGridTextBoxColumn.MappingName == "button1")
                        {
                            bizDataGridTextBoxColumn.Alignment = HorizontalAlignment.Center;
                            bizDataGridTextBoxColumn.TextBoxGrid.BackColor = Color.FromKnownColor(KnownColor.Control);
                            columnButton1 = new Button();
                            columnButton1.Text = "Preview";
                            columnButton1.FlatStyle = FlatStyle.Standard;
                            columnButton1.BackColor = Color.FromKnownColor(KnownColor.Control);
                            columnButton1.Size = new Size(75, 14);
                            columnButton1.Click += new EventHandler(columnButton1_Click);

                            bizDataGridTextBoxColumn.TextBoxGrid.Controls.Add(columnButton1);
                        }
                    }
                }
            }

            foreach (DataGridTableStyle dataGridTableStyle in dg_warning.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
                {

                    if (dataGridColumnStyle is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;

                        if (bizDataGridTextBoxColumn.MappingName == "button2")
                        {
                            bizDataGridTextBoxColumn.Alignment = HorizontalAlignment.Center;
                            bizDataGridTextBoxColumn.TextBoxGrid.BackColor = Color.FromKnownColor(KnownColor.Control);
                            columnButton2 = new Button();
                            columnButton2.Text = "View";
                            columnButton2.FlatStyle = FlatStyle.Standard;
                            columnButton2.BackColor = Color.FromKnownColor(KnownColor.Control);
                            columnButton2.Size = new Size(75, 14);
                            columnButton2.Click += new EventHandler(columnButton2_Click);

                            bizDataGridTextBoxColumn.TextBoxGrid.Controls.Add(columnButton2);
                        }
                    }
                }
            }

            enableDocSave = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("EnableDocSave"));


        }

        void hemph_hramdesc_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from HRAM where [status]<>'V'";
            this.dbaccess.ReadSQL("HRAM2", sql1);
            DataRow drInsertHRAM = this.dbaccess.DataSet.Tables["HRAM2"].NewRow();

            drInsertHRAM["hramnum"] = "NIL";

            this.dbaccess.DataSet.Tables["HRAM2"].Rows.Add(drInsertHRAM);

            hemph_hramdesc.DataSource = this.dbaccess.DataSet.Tables["HRAM2"];
            hemph_hramdesc.DisplayMember = this.dbaccess.DataSet.Tables["HRAM2"].Columns["hramnum"].ColumnName.ToString();
            hemph_hramdesc.ValueMember = this.dbaccess.DataSet.Tables["HRAM2"].Columns["hramnum"].ColumnName.ToString();
        }

        void hemph_pmcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from PMM where [status]<>'V'";
            this.dbaccess.ReadSQL("PMM2", sql1);
            DataRow drInsertPMM2 = this.dbaccess.DataSet.Tables["PMM2"].NewRow();

            drInsertPMM2["pmcode"] = "NIL";

            this.dbaccess.DataSet.Tables["PMM2"].Rows.Add(drInsertPMM2);

            hemph_pmcode.DataSource = this.dbaccess.DataSet.Tables["PMM2"];
            hemph_pmcode.DisplayMember = this.dbaccess.DataSet.Tables["PMM2"].Columns["pmcode"].ColumnName.ToString();
            hemph_pmcode.ValueMember = this.dbaccess.DataSet.Tables["PMM2"].Columns["pmcode"].ColumnName.ToString();
        }

        void hemph_paytypecode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from PAYTM where [status]<>'V'";
            this.dbaccess.ReadSQL("PAYTM", sql1);
            DataRow drInsertPAYTM = this.dbaccess.DataSet.Tables["PAYTM"].NewRow();

            drInsertPAYTM["paytypecode"] = "NIL";

            this.dbaccess.DataSet.Tables["PAYTM"].Rows.Add(drInsertPAYTM);

            hemph_paytypecode.DataSource = this.dbaccess.DataSet.Tables["PAYTM"];
            hemph_paytypecode.DisplayMember = this.dbaccess.DataSet.Tables["PAYTM"].Columns["Desc"].ColumnName.ToString();
            hemph_paytypecode.ValueMember = this.dbaccess.DataSet.Tables["PAYTM"].Columns["paytypecode"].ColumnName.ToString();
        }

        void columnButton2_Click(object sender, EventArgs e)
        {
            DataTable wlr = this.dbaccess.DataSet.Tables["wlr"];

            if (wlr.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(wlr.Rows[dg_warning.CurrentCell.RowNumber]["wlsigLoc"]))
                {
                    Process.Start(wlr.Rows[dg_warning.CurrentCell.RowNumber]["wlsigLoc"].ToString());
                }
                else
                {
                    MessageBox.Show("This Document has No Signed Picture", "No Pictures", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }



        void columnButton1_Click(object sender, EventArgs e)
        {
            DataRow HEMPH = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            DataTable wlr = this.dbaccess.DataSet.Tables["WLR"];

            if (wlr.Rows.Count > 0)
            {
                //string test1 = "SELECT top 1 * FROM sitm where sitenum='" + HEMPH["sitenum"].ToString() + "'";
                //string test2 = "SELECT top 1 * FROM matm where matnum='" + HEMPH["matnum"].ToString() + "'";
                //string test3 = "SELECT * FROM WLR where [guid]='" + wlr.Rows[dg_warning.CurrentCell.RowNumber]["guid"].ToString() + "'";


                string str = "Select " +
                                "A.empname, " +
                                "A.nric, " +
                                "A.position, " +
                                "A.sitename, " +
                                "A.datejoined, " +
                                "A.bankacc, " +
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
                                "A.FamilyContact " +
                            "from " +
                            "( " +
                                "Select  " +
                                    "h.empnum, " +
                                    "h.empname, " +
                                    "h.nric, " +
                                    "h.matnum, " +
                                    "M.matname as position, " +
                                    "h.sitenum, " +
                                    "S.sitename, " +
                                    "h.datejoined, " +
                                    "h.bankacc, " +
                                    "h.commencedate, " +
                                    "h.contact,  " +
                                    "h.homecontactno, " +
                                    "h.daysperweek, " +
                                    "h.MonToFriTimeFrom, " +
                                    "h.MonToFriTimeTo, " +
                                    "h.MonToFriLunchHr, " +
                                    "h.MonToFriTeaBrkHr, " +
                                    "h.SatTimeFrom, " +
                                    "h.SatTimeTo, " +
                                    "h.SatLunchHr, " +
                                    "h.SatTeaBrkHr, " +
                                    "h.SunTimeFrom, " +
                                    "h.SunTimeTo, " +
                                    "h.SunLunchHr, " +
                                    "h.SunTeaBrkHr, " +
                                    "h.RegularOffDay, " +
                                    "h.PHTimeFrom, " +
                                    "h.PHTimeTo, " +
                                    "h.PHLunchHr, " +
                                    "h.PHTeaBrkHr, " +
                                    "CASE WHEN h.paytypecode LIKE 'M%' THEN 'Monthly' WHEN h.paytypecode LIKE 'W%' THEN 'Weekly' WHEN h.paytypecode LIKE 'D%' THEN 'Daily' end as paytypecode, " +
                                    "ISNULL(h.cadd1,'')+' '+ISNULL(h.cadd2,'')+' '+ISNULL(h.cadd3,'') as homeaddress, " +
                                    "0 as SalaryDeductionPerDay,  " + // TO GET THE VALUE
                                    "0 as OtherDudction," + // TO GET THE VALUE
                                    "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='BASIC' and PFMSR.empnum=h.empnum) as [Basic], " +
                                    "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ATTNALLW' and PFMSR.empnum=h.empnum) as [ATTNALLW], " +
                                    "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ACCOMALLW' and PFMSR.empnum=h.empnum) as [ACCOMALLW], " +
                                    "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='DRIVTRANSALLW' and PFMSR.empnum=h.empnum) as [DRIVTRANSALLW], " +
                                    "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='OTALLW' and PFMSR.empnum=h.empnum) as [OTALLW], " +
                                    "0 as OtherAllowance, " + // TO GET THE VALUE
                                    "h.remark, " +
                                    "FM.name as NextOfKeen, " +
                                    "FM.contact as FamilyContact " +
                                "from hemph h " +
                                "LEFT JOIN " +
                                "( " +
                                "Select top 1 empnum,name,contact from FAMR where [status]<>'V' " +
                                ")FM " +
                                "ON h.empnum=FM.empnum " +
                                "LEFT JOIN MATM M  " +
                                "ON h.matnum=M.matnum " +
                                "LEFT JOIN SITM S " +
                                "ON h.sitenum=S.sitenum " +
                            ")A where A.empnum='" + HEMPH["empnum"].ToString().Trim() + "'";


                Hashtable selectedCollection = new Hashtable();
                selectedCollection.Add("LetterAppointment", str);
                selectedCollection.Add("COY1", "SELECT * FROM coy");
                selectedCollection.Add("SITM1", "SELECT top 1 * FROM sitm where sitenum='" + HEMPH["sitenum"].ToString() + "'");
                selectedCollection.Add("MATM1", "SELECT top 1 * FROM matm where matnum='" + HEMPH["matnum"].ToString() + "'");
                selectedCollection.Add("wlr1", "SELECT * FROM WLR where [guid]='" + wlr.Rows[dg_warning.CurrentCell.RowNumber]["guid"].ToString() + "'");
                this.dbaccess.ReadSQL(selectedCollection);


                GetSignatureWarning();

                DataTable SigTB1 = this.dbaccess.DataSet.Tables["SigTB"].Copy();

                SigTB1.TableName = "SigTB1";

                if (this.dbaccess.DataSet.Tables.Contains("SigTB1"))
                {
                    this.dbaccess.DataSet.Tables["SigTB1"].Dispose();
                    this.dbaccess.DataSet.Tables.Remove("SigTB1");
                    SigTB1.TableName = "SigTB1";
                    this.dbaccess.DataSet.Tables.Add(SigTB1);
                }
                else
                {
                    SigTB1.TableName = "SigTB1";
                    this.dbaccess.DataSet.Tables.Add(SigTB1);
                }


                this.crpt1 = new ATL.BizModules.Tools.CRForm();

                string ReportLocation = @"\HEMP\Report\WarningLettertForm1.rpt";

                ReportDocument crReportDocument = new ReportDocument();
                crReportDocument.Load(this.projectPath + ReportLocation);

                crReportDocument.SetDataSource(this.dbaccess.DataSet);

                crpt1.Crv1.ReportSource = crReportDocument;
                crpt1.ShowDialog();


            }
        }

        void pffa_btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaDecommpress form = new ATL.BizModules.StaCompressFolders.StaDecommpress(dbaccess, "HEMPH", "PFFA", "empnum");
                form.ShowDialog();
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        void pffa_btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaCompress form = new ATL.BizModules.StaCompressFolders.StaCompress(dbaccess, "HEMPH", "PFFA", "empnum");
                form.ShowDialog();
                form.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        void Btn_Contract_Click(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

            WordForm3 wf = new WordForm3(this.dbaccess, "HEMPH", "contractdoc", "CONTRACT");
            wf.ShowDialog();


        }

        #endregion

        #region hemph_hramdesc_Leave Event

        private void hemph_nationality_Leave(object sender, EventArgs e)
        {
            if (hemph_nationality.Text != string.Empty)
            {
                if (hemph_nationality.Text.Contains(" PR"))
                {
                    hemph_datejPR.Enabled = true;
                }
                else
                {
                    hemph_datejPR.Enabled = false;
                }
            }
            else
            {
                hemph_datejPR.Enabled = false;
            }
        }

        #endregion

        #region dob DateTimePicker Leave Event

        private void hemph_dob_Leave(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            age.Text = Convert.ToString(GetAge());

        }

        private void hemph_dob_TextChanged(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            age.Text = Convert.ToString(GetAge());
        }

        #endregion

        #region hemph datejoined Leave

        private void hemph_datejoined_Leave(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

            if (hemph_datejoined.Text.ToString() != string.Empty)
            {
                if (!GetConfirmationDate())
                {
                    DateTime dt = Convert.ToDateTime(hemph_datejoined.Text).AddMonths(3);

                    if (!hemph["empnum"].ToString().Contains("P"))
                    {
                        txt_dateconfirmed.Text = dt.ToShortDateString();
                    }
                    else
                        txt_dateconfirmed.Text = string.Empty;
                    {
                    }
                }
            }


        }

        #endregion

        #region Get Age

        private int GetAge()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            int age = 0;
            if (hemph_dob.Text.ToString().Trim() != string.Empty)
            {
                age = TimeTools.CalculateAge(Convert.ToDateTime(hemph_dob.Text), DateTime.Today);
            }
            return age;
        }

        #endregion

        #region GetEmployeeData

        protected void GetEmployeeData()
        {

            //hemph_nric.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "vHemphNRIC", "nric");
            string empnum;
            DataTable pfmedu = this.dbaccess.DataSet.Tables["pfmedu"];
            DataTable recr = this.dbaccess.DataSet.Tables["recr"];
            DataTable wpr = this.dbaccess.DataSet.Tables["wpr"];
            DataTable pfmwe = this.dbaccess.DataSet.Tables["pfmwe"];
            //DataTable pfmer = this.dbaccess.DataSet.Tables["pfmer"];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["pfmsr"];
            DataTable famr = this.dbaccess.DataSet.Tables["famr"];
            DataTable plr = this.dbaccess.DataSet.Tables["plr"];
            DataTable hcnr = this.dbaccess.DataSet.Tables["hcnr"];
            DataTable pfdrec = this.dbaccess.DataSet.Tables["pfdrec"];
            DataTable pfplrec = this.dbaccess.DataSet.Tables["pfplrec"];
            //DataTable cidc = this.dbaccess.DataSet.Tables["cidc"];

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

            if (hemph_nric.Text != "")
            {

                string sql1 = "Select * From " +
                            "( " +
                                "Select " +
                                "ROW_NUMBER() OVER (Order BY datejoined) as ForTop,ROW_NUMBER() OVER (Order BY datejoined Desc) as ForBottom, * " +
                                "from " +
                                "HEMPH where nric='" + hemph_nric.Text.ToString().Trim() + "' and [status]<>'V' and empnum<>'" + hemph["empnum"].ToString() + "' " +
                            ")A " +
                            "Where ForBottom =1 ";

                this.dbaccess.ReadSQL("TmpEmpInfo", sql1);
                DataTable TmpEmpInfo = this.dbaccess.DataSet.Tables["TmpEmpInfo"];
                if (TmpEmpInfo.Rows.Count > 0)
                {
                    DataRow dr1 = TmpEmpInfo.Rows[0];
                    //hemph["nric"] = dr1["nric"];

                    if (!BizFunctions.IsEmpty(dr1["gender"]))
                    {
                        hemph["gender"] = dr1["gender"];

                        if (dr1["gender"].ToString().Trim() == "M")
                        {
                            rad_male.Checked = true;
                            rad_female.Checked = false;
                        }
                        if (dr1["gender"].ToString().Trim() == "F")
                        {
                            rad_male.Checked = false;
                            rad_female.Checked = true;
                        }
                    }


                    hemph["regname"] = dr1["regname"];
                    hemph["maritalstatus"] = dr1["maritalstatus"];
                    hemph["ethnicity"] = dr1["ethnicity"];
                    hemph["dob"] = dr1["dob"];
                    hemph["bloodtype"] = dr1["bloodtype"];
                    hemph["etype"] = dr1["etype"];
                    hemph["statusid"] = dr1["statusid"];
                    hemph["sitenum"] = dr1["sitenum"];
                    hemph["sectorcode"] = dr1["sectorcode"];
                    hemph["dateconfirmed"] = dr1["dateconfirmed"];
                    hemph["datejoined"] = dr1["datejoined"];
                    hemph["dateresigned"] = dr1["dateresigned"];

                    hemph["basicsal"] = dr1["basicsal"];
                    hemph["nextapprdate"] = dr1["nextapprdate"];
                    hemph["remark"] = dr1["remark"];
                    hemph["contact"] = dr1["contact"];
                    hemph["empname"] = dr1["empname"];
                    hemph["matnum"] = dr1["matnum"];
                    //hemph["cardid"] = dr1["cardid"];
                    hemph["recrempnum"] = dr1["recrempnum"];
                    hemph["hsgcode"] = dr1["hsgcode"];
                    hemph["photo"] = dr1["photo"];
                    hemph["religion"] = dr1["religion"];
                    hemph["force"] = dr1["force"];
                    hemph["rank"] = dr1["rank"];
                    hemph["vocation"] = dr1["vocation"];
                    hemph["yearserved"] = dr1["yearserved"];
                    hemph["padd1"] = dr1["padd1"];
                    hemph["padd2"] = dr1["padd2"];
                    hemph["padd3"] = dr1["padd3"];
                    hemph["ppostal"] = dr1["ppostal"];
                    hemph["cadd1"] = dr1["cadd1"];
                    hemph["cadd2"] = dr1["cadd2"];
                    hemph["cadd3"] = dr1["cadd3"];
                    hemph["postal"] = dr1["postal"];
                    hemph["isNSmandatory"] = dr1["isNSmandatory"];
                    hemph["paytypecode"] = dr1["paytypecode"];
                    hemph["daysperweek"] = dr1["daysperweek"];
                    hemph["hramdesc"] = dr1["hramdesc"];
                    hemph["nationality"] = dr1["nationality"];

                    hemph["isRetired"] = dr1["isRetired"];
                    hemph["isNSmandatory"] = dr1["isNSmandatory"];

                    hemph["bankacc"] = dr1["bankacc"];
                    hemph["bankname"] = dr1["bankname"];
                    hemph["BankCode"] = dr1["BankCode"];
                    hemph["BranchCode"] = dr1["BankCode"];
                    hemph["BldgBlock"] = dr1["BldgBlock"];
                    hemph["LevelNo"] = dr1["LevelNo"];
                    hemph["UnitNo"] = dr1["UnitNo"];
                    hemph["StreetName"] = dr1["StreetName"];
                    hemph["COUNTRY"] = dr1["COUNTRY"];
                    hemph["contact"] = dr1["contact"];
                    if (!BizFunctions.IsEmpty(dr1["datePR"]))
                    {
                        hemph["datePR"] = dr1["datePR"];
                    }
                    else
                    {
                        hemph["datePR"] = System.DBNull.Value;
                    }


                    this.ShowImage();

                    ////////////////////////////////////////////////

                    if (!BizFunctions.IsEmpty(dr1["empnum"].ToString().Trim()))
                    {

                        if (pfmedu.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfmedu);
                        }
                        if (recr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(recr);
                        }
                        if (wpr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(wpr);
                        }
                        if (pfmwe.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfmwe);
                        }
                        if (pfmsr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfmsr);
                        }
                        if (famr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(famr);
                        }
                        if (plr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(plr);
                        }
                        if (hcnr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(hcnr);
                        }
                        if (pfdrec.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfdrec);
                        }
                        if (pfplrec.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfplrec);
                        }

                        //if (cidc.Rows.Count > 0)
                        //{
                        //    BizFunctions.DeleteAllRows(cidc);
                        //}

                        string sql3 = "Select * from Recr where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpRecr", sql3);
                        DataTable TmpEmpRecr = this.dbaccess.DataSet.Tables["TmpEmpRecr"];

                        if (TmpEmpRecr.Rows.Count > 0)
                        {
                            foreach (DataRow dr2 in TmpEmpRecr.Select())
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertRecr = recr.NewRow();
                                    InsertRecr["incentivedsc"] = dr2["incentivedsc"];
                                    InsertRecr["incentivecr"] = dr2["incentivecr"];
                                    InsertRecr["incentivedate"] = dr2["incentivedate"];
                                    InsertRecr["nricrecomm"] = dr2["nricrecomm"];
                                    recr.Rows.Add(InsertRecr);
                                }
                            }
                        }

                        string sql4 = "Select * from WPR where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpWpr", sql4);
                        DataTable TmpEmpWpr = this.dbaccess.DataSet.Tables["TmpEmpWpr"];

                        if (TmpEmpWpr.Rows.Count > 0)
                        {
                            foreach (DataRow dr3 in TmpEmpWpr.Select())
                            {
                                if (dr3.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertWpr = wpr.NewRow();
                                    InsertWpr["wpnum"] = dr3["wpnum"];
                                    InsertWpr["wpissuedate "] = dr3["wpissuedate"];
                                    InsertWpr["wpexpiry"] = dr3["wpexpiry"];
                                    InsertWpr["wprenewal"] = dr3["wprenewal"];
                                    wpr.Rows.Add(InsertWpr);
                                }
                            }
                        }

                        string sql5 = "Select * from PFMWE where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfmwe", sql5);
                        DataTable TmpEmpPfmwe = this.dbaccess.DataSet.Tables["TmpEmpPfmwe"];

                        if (TmpEmpPfmwe.Rows.Count > 0)
                        {
                            foreach (DataRow dr4 in TmpEmpPfmwe.Select())
                            {
                                if (dr4.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertPfmwe = pfmwe.NewRow();
                                    InsertPfmwe["matnum"] = dr4["matnum"];
                                    InsertPfmwe["coy"] = dr4["coy"];
                                    InsertPfmwe["coyname"] = dr4["coyname"];
                                    InsertPfmwe["yearfrom"] = dr4["yearfrom"];
                                    InsertPfmwe["yearto"] = dr4["yearto"];
                                    InsertPfmwe["issecurityrelated"] = dr4["issecurityrelated"];
                                    pfmwe.Rows.Add(InsertPfmwe);
                                }
                            }
                        }


                        string sql7 = "Select * from PFMSR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfmsr", sql7);
                        DataTable TmpEmpPfmsr = this.dbaccess.DataSet.Tables["TmpEmpPfmsr"];

                        if (TmpEmpPfmsr.Rows.Count > 0)
                        {
                            foreach (DataRow dr6 in TmpEmpPfmsr.Select())
                            {
                                if (dr6.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertpfmsr = pfmsr.NewRow();
                                    Insertpfmsr["nric"] = dr6["nric"];
                                    Insertpfmsr["saldesc"] = dr6["saldesc"];
                                    Insertpfmsr["rateamt"] = dr6["rateamt"];
                                    Insertpfmsr["hsamcode"] = dr6["hsamcode"];
                                    Insertpfmsr["dateadjusted"] = dr6["dateadjusted"];
                                    Insertpfmsr["line"] = dr6["line"];
                                    Insertpfmsr["remarks"] = dr6["remarks"];
                                    pfmsr.Rows.Add(Insertpfmsr);
                                }
                            }
                        }

                        string sql8 = "Select * from FAMR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpFamr", sql8);
                        DataTable TmpEmpFamr = this.dbaccess.DataSet.Tables["TmpEmpFamr"];

                        if (TmpEmpFamr.Rows.Count > 0)
                        {
                            foreach (DataRow dr7 in TmpEmpFamr.Select())
                            {
                                if (dr7.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertfamr = famr.NewRow();
                                    Insertfamr["nric"] = dr7["nric"];
                                    Insertfamr["name"] = dr7["name"];
                                    Insertfamr["relationship"] = dr7["relationship"];
                                    Insertfamr["gender"] = dr7["gender"];
                                    Insertfamr["contact"] = dr7["contact"];
                                    Insertfamr["remark"] = dr7["remark"];
                                    famr.Rows.Add(Insertfamr);
                                }
                            }
                        }

                        string sql9 = "Select * from PLR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPlr", sql9);
                        DataTable TmpEmpPlr = this.dbaccess.DataSet.Tables["TmpEmpPlr"];

                        if (TmpEmpPlr.Rows.Count > 0)
                        {
                            foreach (DataRow dr8 in TmpEmpPlr.Select())
                            {
                                if (dr8.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertplr = plr.NewRow();
                                    Insertplr["nric"] = dr8["nric"];
                                    Insertplr["pldesc"] = dr8["pldesc"];
                                    Insertplr["permitno"] = dr8["permitno"];
                                    Insertplr["applydate"] = dr8["applydate"];
                                    Insertplr["expirydate"] = dr8["expirydate"];
                                    Insertplr["remark"] = dr8["remark"];
                                    Insertplr["cancelddate"] = dr8["cancelddate"];
                                    plr.Rows.Add(Insertplr);
                                }
                            }
                        }

                        string sql10 = "Select * from HCNR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpHncr", sql10);
                        DataTable TmpEmpHncr = this.dbaccess.DataSet.Tables["TmpEmpHncr"];

                        if (TmpEmpHncr.Rows.Count > 0)
                        {
                            foreach (DataRow dr9 in TmpEmpHncr.Select())
                            {
                                if (dr9.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertHcnr = hcnr.NewRow();
                                    InsertHcnr["nric"] = dr9["nric"];
                                    InsertHcnr["contactno"] = dr9["contactno"];
                                    InsertHcnr["contype"] = dr9["contype"];
                                    InsertHcnr["isactive"] = dr9["isactive"];
                                    hcnr.Rows.Add(InsertHcnr);
                                }
                            }
                        }


                        string sql2 = "Select * from pfmedu where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfmedu", sql2);
                        DataTable TmpEmpPfmedu = this.dbaccess.DataSet.Tables["TmpEmpPfmedu"];

                        if (TmpEmpPfmedu.Rows.Count > 0)
                        {
                            foreach (DataRow dr11 in TmpEmpPfmedu.Select())
                            {
                                if (dr1.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertPfmedu = pfmedu.NewRow();
                                    InsertPfmedu["coursedesc"] = dr11["coursedesc"];
                                    InsertPfmedu["Institution"] = dr11["Institution"];
                                    InsertPfmedu["datefinished"] = dr11["datefinished"];
                                    InsertPfmedu["certification"] = dr11["certification"];
                                    InsertPfmedu["yearfinished"] = dr11["yearfinished"];
                                    InsertPfmedu["ishighest"] = dr11["ishighest"];
                                    InsertPfmedu["issecurityrelated"] = dr11["issecurityrelated"];
                                    InsertPfmedu["remark"] = dr11["remark"];
                                    pfmedu.Rows.Add(InsertPfmedu);
                                }
                            }
                        }

                        string sql13 = "Select * from PFDREC where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfdrec", sql13);

                        DataTable TmpEmpPfdrec = this.dbaccess.DataSet.Tables["TmpEmpPfdrec"];

                        if (TmpEmpPfdrec.Rows.Count > 0)
                        {
                            foreach (DataRow dr12 in TmpEmpPfdrec.Select())
                            {
                                if (dr12.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertPfdrec = pfdrec.NewRow();
                                    InsertPfdrec["nric"] = dr12["nric"];
                                    InsertPfdrec["trackingno"] = dr12["trackingno"];
                                    InsertPfdrec["filename"] = dr12["filename"];
                                    InsertPfdrec["templocation"] = dr12["templocation"];
                                    InsertPfdrec["physicalserverlocation"] = dr12["physicalserverlocation"];
                                    InsertPfdrec["remark"] = dr12["remark"];
                                    pfdrec.Rows.Add(InsertPfdrec);
                                }
                            }
                        }


                        string sql14 = "Select * from PFPLREC where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmppfplrec", sql14);

                        DataTable TmpEmppfplrec = this.dbaccess.DataSet.Tables["TmpEmppfplrec"];

                        if (TmpEmppfplrec.Rows.Count > 0)
                        {
                            foreach (DataRow dr13 in TmpEmppfplrec.Select())
                            {
                                if (dr13.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertpfplrec = pfplrec.NewRow();
                                    Insertpfplrec["nric"] = dr13["nric"];
                                    Insertpfplrec["trackingno"] = dr13["trackingno"];
                                    Insertpfplrec["filename"] = dr13["filename"];
                                    Insertpfplrec["templocation"] = dr13["templocation"];
                                    Insertpfplrec["physicalserverlocation"] = dr13["physicalserverlocation"];
                                    Insertpfplrec["remark"] = dr13["remark"];
                                    pfplrec.Rows.Add(Insertpfplrec);
                                }
                            }
                        }

                        /////////////////

                        //string sql15 = "Select * from CIDC where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        //this.dbaccess.ReadSQL("TmpEmpcidc", sql15);

                        //DataTable TmpEmpcidc = this.dbaccess.DataSet.Tables["TmpEmpcidc"];

                        //if (TmpEmpcidc.Rows.Count > 0)
                        //{
                        //    foreach (DataRow dr14 in TmpEmpcidc.Select())
                        //    {
                        //        if (dr14.RowState != DataRowState.Deleted)
                        //        {
                        //            DataRow Insertcidc = cidc.NewRow();
                        //            Insertcidc["nric"] = dr14["nric"];
                        //            Insertcidc["cidstatusofindividual"] = dr14["cidstatusofindividual"];
                        //            Insertcidc["cidClearanceNo"] = dr14["cidClearanceNo"];
                        //            Insertcidc["cidissuedate"] = dr14["cidissuedate"];
                        //            Insertcidc["licencsestatus"] = dr14["licencsestatus"];
                        //            Insertcidc["trainningstatus"] = dr14["trainningstatus"];
                        //            cidc.Rows.Add(Insertcidc);
                        //        }
                        //    }
                        //}

                        TmpEmpFamr.Dispose();
                        TmpEmpHncr.Dispose();
                        TmpEmpInfo.Dispose();
                        TmpEmpPfmsr.Dispose();
                        TmpEmpPfmwe.Dispose();
                        TmpEmpPlr.Dispose();
                        TmpEmpRecr.Dispose();
                        TmpEmpWpr.Dispose();
                        TmpEmpPfdrec.Dispose();
                        TmpEmppfplrec.Dispose();
                        //TmpEmpcidc.Dispose();

                        //GetEmploymentHistory();
                        GetLeavetHistory();
                        GetSalaryHistory();
                        GetTotalSalary();

                    }

                }

            }

        }

        protected void GetEmployeeData2()
        {

            //hemph_nric.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "vHemphNRIC", "nric");
            string empnum;
            DataTable pfmedu = this.dbaccess.DataSet.Tables["pfmedu"];
            DataTable recr = this.dbaccess.DataSet.Tables["recr"];
            DataTable wpr = this.dbaccess.DataSet.Tables["wpr"];
            DataTable pfmwe = this.dbaccess.DataSet.Tables["pfmwe"];
            //DataTable pfmer = this.dbaccess.DataSet.Tables["pfmer"];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["pfmsr"];
            DataTable famr = this.dbaccess.DataSet.Tables["famr"];
            DataTable plr = this.dbaccess.DataSet.Tables["plr"];
            DataTable hcnr = this.dbaccess.DataSet.Tables["hcnr"];
            DataTable pfdrec = this.dbaccess.DataSet.Tables["pfdrec"];
            DataTable pfplrec = this.dbaccess.DataSet.Tables["pfplrec"];
            //DataTable cidc = this.dbaccess.DataSet.Tables["cidc"];

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

            if (hemph_nric.Text != "")
            {

                string sql1 = "Select * From " +
                            "( " +
                                "Select " +
                                "ROW_NUMBER() OVER (Order BY datejoined) as ForTop,ROW_NUMBER() OVER (Order BY datejoined Desc) as ForBottom, * " +
                                "from " +
                                "HEMPH where nric='" + hemph_nric.Text.ToString().Trim() + "' and [status]<>'V'    " +
                            ")A " +
                            "Where ForBottom =1 ";

                this.dbaccess.ReadSQL("TmpEmpInfo", sql1);
                DataTable TmpEmpInfo = this.dbaccess.DataSet.Tables["TmpEmpInfo"];
                if (TmpEmpInfo.Rows.Count > 0)
                {
                    DataRow dr1 = TmpEmpInfo.Rows[0];
                    //hemph["nric"] = dr1["nric"];

                    if (!BizFunctions.IsEmpty(dr1["gender"]))
                    {
                        hemph["gender"] = dr1["gender"];

                        if (dr1["gender"].ToString().Trim() == "M")
                        {
                            rad_male.Checked = true;
                            rad_female.Checked = false;
                        }
                        if (dr1["gender"].ToString().Trim() == "F")
                        {
                            rad_male.Checked = false;
                            rad_female.Checked = true;
                        }
                    }


                    hemph["regname"] = dr1["regname"];
                    hemph["maritalstatus"] = dr1["maritalstatus"];
                    hemph["ethnicity"] = dr1["ethnicity"];
                    hemph["dob"] = dr1["dob"];
                    hemph["bloodtype"] = dr1["bloodtype"];
                    hemph["etype"] = dr1["etype"];
                    hemph["statusid"] = dr1["statusid"];
                    hemph["sitenum"] = dr1["sitenum"];
                    hemph["sectorcode"] = dr1["sectorcode"];
                    hemph["dateconfirmed"] = dr1["dateconfirmed"];
                    hemph["datejoined"] = dr1["datejoined"];
                    hemph["dateresigned"] = dr1["dateresigned"];

                    hemph["basicsal"] = dr1["basicsal"];
                    hemph["nextapprdate"] = dr1["nextapprdate"];
                    hemph["remark"] = dr1["remark"];
                    hemph["contact"] = dr1["contact"];
                    hemph["empname"] = dr1["empname"];
                    hemph["matnum"] = dr1["matnum"];
                    //hemph["cardid"] = dr1["cardid"];
                    hemph["recrempnum"] = dr1["recrempnum"];
                    hemph["hsgcode"] = dr1["hsgcode"];
                    hemph["photo"] = dr1["photo"];
                    hemph["religion"] = dr1["religion"];
                    hemph["force"] = dr1["force"];
                    hemph["rank"] = dr1["rank"];
                    hemph["vocation"] = dr1["vocation"];
                    hemph["yearserved"] = dr1["yearserved"];
                    hemph["padd1"] = dr1["padd1"];
                    hemph["padd2"] = dr1["padd2"];
                    hemph["padd3"] = dr1["padd3"];
                    hemph["ppostal"] = dr1["ppostal"];
                    hemph["cadd1"] = dr1["cadd1"];
                    hemph["cadd2"] = dr1["cadd2"];
                    hemph["cadd3"] = dr1["cadd3"];
                    hemph["postal"] = dr1["postal"];
                    hemph["isNSmandatory"] = dr1["isNSmandatory"];
                    hemph["paytypecode"] = dr1["paytypecode"];
                    hemph["daysperweek"] = dr1["daysperweek"];
                    hemph["hramdesc"] = dr1["hramdesc"];
                    hemph["nationality"] = dr1["nationality"];

                    hemph["isRetired"] = dr1["isRetired"];
                    hemph["isNSmandatory"] = dr1["isNSmandatory"];

                    hemph["bankacc"] = dr1["bankacc"];
                    hemph["bankname"] = dr1["bankname"];
                    hemph["BankCode"] = dr1["BankCode"];
                    hemph["BranchCode"] = dr1["BankCode"];
                    hemph["BldgBlock"] = dr1["BldgBlock"];
                    hemph["LevelNo"] = dr1["LevelNo"];
                    hemph["UnitNo"] = dr1["UnitNo"];
                    hemph["StreetName"] = dr1["StreetName"];
                    hemph["COUNTRY"] = dr1["COUNTRY"];
                    hemph["contact"] = dr1["contact"];
                    if (!BizFunctions.IsEmpty(dr1["datePR"]))
                    {
                        hemph["datePR"] = dr1["datePR"];
                    }
                    else
                    {
                        hemph["datePR"] = System.DBNull.Value;
                    }


                    this.ShowImage();

                    ////////////////////////////////////////////////

                    if (!BizFunctions.IsEmpty(dr1["empnum"].ToString().Trim()))
                    {

                        if (pfmedu.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfmedu);
                        }
                        if (recr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(recr);
                        }
                        if (wpr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(wpr);
                        }
                        if (pfmwe.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfmwe);
                        }
                        if (pfmsr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfmsr);
                        }
                        if (famr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(famr);
                        }
                        if (plr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(plr);
                        }
                        if (hcnr.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(hcnr);
                        }
                        if (pfdrec.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfdrec);
                        }
                        if (pfplrec.Rows.Count > 0)
                        {
                            BizFunctions.DeleteAllRows(pfplrec);
                        }

                        //if (cidc.Rows.Count > 0)
                        //{
                        //    BizFunctions.DeleteAllRows(cidc);
                        //}

                        string sql3 = "Select * from Recr where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpRecr", sql3);
                        DataTable TmpEmpRecr = this.dbaccess.DataSet.Tables["TmpEmpRecr"];

                        if (TmpEmpRecr.Rows.Count > 0)
                        {
                            foreach (DataRow dr2 in TmpEmpRecr.Select())
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertRecr = recr.NewRow();
                                    InsertRecr["incentivedsc"] = dr2["incentivedsc"];
                                    InsertRecr["incentivecr"] = dr2["incentivecr"];
                                    InsertRecr["incentivedate"] = dr2["incentivedate"];
                                    InsertRecr["nricrecomm"] = dr2["nricrecomm"];
                                    recr.Rows.Add(InsertRecr);
                                }
                            }
                        }

                        string sql4 = "Select * from WPR where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpWpr", sql4);
                        DataTable TmpEmpWpr = this.dbaccess.DataSet.Tables["TmpEmpWpr"];

                        if (TmpEmpWpr.Rows.Count > 0)
                        {
                            foreach (DataRow dr3 in TmpEmpWpr.Select())
                            {
                                if (dr3.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertWpr = wpr.NewRow();
                                    InsertWpr["wpnum"] = dr3["wpnum"];
                                    InsertWpr["wpissuedate "] = dr3["wpissuedate"];
                                    InsertWpr["wpexpiry"] = dr3["wpexpiry"];
                                    InsertWpr["wprenewal"] = dr3["wprenewal"];
                                    wpr.Rows.Add(InsertWpr);
                                }
                            }
                        }

                        string sql5 = "Select * from PFMWE where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfmwe", sql5);
                        DataTable TmpEmpPfmwe = this.dbaccess.DataSet.Tables["TmpEmpPfmwe"];

                        if (TmpEmpPfmwe.Rows.Count > 0)
                        {
                            foreach (DataRow dr4 in TmpEmpPfmwe.Select())
                            {
                                if (dr4.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertPfmwe = pfmwe.NewRow();
                                    InsertPfmwe["matnum"] = dr4["matnum"];
                                    InsertPfmwe["coy"] = dr4["coy"];
                                    InsertPfmwe["coyname"] = dr4["coyname"];
                                    InsertPfmwe["yearfrom"] = dr4["yearfrom"];
                                    InsertPfmwe["yearto"] = dr4["yearto"];
                                    InsertPfmwe["issecurityrelated"] = dr4["issecurityrelated"];
                                    pfmwe.Rows.Add(InsertPfmwe);
                                }
                            }
                        }


                        string sql7 = "Select * from PFMSR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfmsr", sql7);
                        DataTable TmpEmpPfmsr = this.dbaccess.DataSet.Tables["TmpEmpPfmsr"];

                        if (TmpEmpPfmsr.Rows.Count > 0)
                        {
                            foreach (DataRow dr6 in TmpEmpPfmsr.Select())
                            {
                                if (dr6.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertpfmsr = pfmsr.NewRow();
                                    Insertpfmsr["nric"] = dr6["nric"];
                                    Insertpfmsr["saldesc"] = dr6["saldesc"];
                                    Insertpfmsr["rateamt"] = dr6["rateamt"];
                                    Insertpfmsr["hsamcode"] = dr6["hsamcode"];
                                    Insertpfmsr["dateadjusted"] = dr6["dateadjusted"];
                                    Insertpfmsr["line"] = dr6["line"];
                                    Insertpfmsr["remarks"] = dr6["remarks"];
                                    pfmsr.Rows.Add(Insertpfmsr);
                                }
                            }
                        }

                        string sql8 = "Select * from FAMR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpFamr", sql8);
                        DataTable TmpEmpFamr = this.dbaccess.DataSet.Tables["TmpEmpFamr"];

                        if (TmpEmpFamr.Rows.Count > 0)
                        {
                            foreach (DataRow dr7 in TmpEmpFamr.Select())
                            {
                                if (dr7.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertfamr = famr.NewRow();
                                    Insertfamr["nric"] = dr7["nric"];
                                    Insertfamr["name"] = dr7["name"];
                                    Insertfamr["relationship"] = dr7["relationship"];
                                    Insertfamr["gender"] = dr7["gender"];
                                    Insertfamr["contact"] = dr7["contact"];
                                    Insertfamr["remark"] = dr7["remark"];
                                    famr.Rows.Add(Insertfamr);
                                }
                            }
                        }

                        string sql9 = "Select * from PLR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPlr", sql9);
                        DataTable TmpEmpPlr = this.dbaccess.DataSet.Tables["TmpEmpPlr"];

                        if (TmpEmpPlr.Rows.Count > 0)
                        {
                            foreach (DataRow dr8 in TmpEmpPlr.Select())
                            {
                                if (dr8.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertplr = plr.NewRow();
                                    Insertplr["nric"] = dr8["nric"];
                                    Insertplr["pldesc"] = dr8["pldesc"];
                                    Insertplr["permitno"] = dr8["permitno"];
                                    Insertplr["applydate"] = dr8["applydate"];
                                    Insertplr["expirydate"] = dr8["expirydate"];
                                    Insertplr["remark"] = dr8["remark"];
                                    Insertplr["cancelddate"] = dr8["cancelddate"];
                                    plr.Rows.Add(Insertplr);
                                }
                            }
                        }

                        string sql10 = "Select * from HCNR where empnum='" + dr1["empnum"].ToString().Trim() + "' and nric='" + dr1["nric"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpHncr", sql10);
                        DataTable TmpEmpHncr = this.dbaccess.DataSet.Tables["TmpEmpHncr"];

                        if (TmpEmpHncr.Rows.Count > 0)
                        {
                            foreach (DataRow dr9 in TmpEmpHncr.Select())
                            {
                                if (dr9.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertHcnr = hcnr.NewRow();
                                    InsertHcnr["nric"] = dr9["nric"];
                                    InsertHcnr["contactno"] = dr9["contactno"];
                                    InsertHcnr["contype"] = dr9["contype"];
                                    InsertHcnr["isactive"] = dr9["isactive"];
                                    hcnr.Rows.Add(InsertHcnr);
                                }
                            }
                        }


                        string sql2 = "Select * from pfmedu where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfmedu", sql2);
                        DataTable TmpEmpPfmedu = this.dbaccess.DataSet.Tables["TmpEmpPfmedu"];

                        if (TmpEmpPfmedu.Rows.Count > 0)
                        {
                            foreach (DataRow dr11 in TmpEmpPfmedu.Select())
                            {
                                if (dr1.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertPfmedu = pfmedu.NewRow();
                                    InsertPfmedu["coursedesc"] = dr11["coursedesc"];
                                    InsertPfmedu["Institution"] = dr11["Institution"];
                                    InsertPfmedu["datefinished"] = dr11["datefinished"];
                                    InsertPfmedu["certification"] = dr11["certification"];
                                    InsertPfmedu["yearfinished"] = dr11["yearfinished"];
                                    InsertPfmedu["ishighest"] = dr11["ishighest"];
                                    InsertPfmedu["issecurityrelated"] = dr11["issecurityrelated"];
                                    InsertPfmedu["remark"] = dr11["remark"];
                                    pfmedu.Rows.Add(InsertPfmedu);
                                }
                            }
                        }

                        string sql13 = "Select * from PFDREC where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmpPfdrec", sql13);

                        DataTable TmpEmpPfdrec = this.dbaccess.DataSet.Tables["TmpEmpPfdrec"];

                        if (TmpEmpPfdrec.Rows.Count > 0)
                        {
                            foreach (DataRow dr12 in TmpEmpPfdrec.Select())
                            {
                                if (dr12.RowState != DataRowState.Deleted)
                                {
                                    DataRow InsertPfdrec = pfdrec.NewRow();
                                    InsertPfdrec["nric"] = dr12["nric"];
                                    InsertPfdrec["trackingno"] = dr12["trackingno"];
                                    InsertPfdrec["filename"] = dr12["filename"];
                                    InsertPfdrec["templocation"] = dr12["templocation"];
                                    InsertPfdrec["physicalserverlocation"] = dr12["physicalserverlocation"];
                                    InsertPfdrec["remark"] = dr12["remark"];
                                    pfdrec.Rows.Add(InsertPfdrec);
                                }
                            }
                        }


                        string sql14 = "Select * from PFPLREC where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        this.dbaccess.ReadSQL("TmpEmppfplrec", sql14);

                        DataTable TmpEmppfplrec = this.dbaccess.DataSet.Tables["TmpEmppfplrec"];

                        if (TmpEmppfplrec.Rows.Count > 0)
                        {
                            foreach (DataRow dr13 in TmpEmppfplrec.Select())
                            {
                                if (dr13.RowState != DataRowState.Deleted)
                                {
                                    DataRow Insertpfplrec = pfplrec.NewRow();
                                    Insertpfplrec["nric"] = dr13["nric"];
                                    Insertpfplrec["trackingno"] = dr13["trackingno"];
                                    Insertpfplrec["filename"] = dr13["filename"];
                                    Insertpfplrec["templocation"] = dr13["templocation"];
                                    Insertpfplrec["physicalserverlocation"] = dr13["physicalserverlocation"];
                                    Insertpfplrec["remark"] = dr13["remark"];
                                    pfplrec.Rows.Add(Insertpfplrec);
                                }
                            }
                        }

                        /////////////////

                        //string sql15 = "Select * from CIDC where empnum='" + dr1["empnum"].ToString().Trim() + "' and [status]<>'V'";
                        //this.dbaccess.ReadSQL("TmpEmpcidc", sql15);

                        //DataTable TmpEmpcidc = this.dbaccess.DataSet.Tables["TmpEmpcidc"];

                        //if (TmpEmpcidc.Rows.Count > 0)
                        //{
                        //    foreach (DataRow dr14 in TmpEmpcidc.Select())
                        //    {
                        //        if (dr14.RowState != DataRowState.Deleted)
                        //        {
                        //            DataRow Insertcidc = cidc.NewRow();
                        //            Insertcidc["nric"] = dr14["nric"];
                        //            Insertcidc["cidstatusofindividual"] = dr14["cidstatusofindividual"];
                        //            Insertcidc["cidClearanceNo"] = dr14["cidClearanceNo"];
                        //            Insertcidc["cidissuedate"] = dr14["cidissuedate"];
                        //            Insertcidc["licencsestatus"] = dr14["licencsestatus"];
                        //            Insertcidc["trainningstatus"] = dr14["trainningstatus"];
                        //            cidc.Rows.Add(Insertcidc);
                        //        }
                        //    }
                        //}

                        TmpEmpFamr.Dispose();
                        TmpEmpHncr.Dispose();
                        TmpEmpInfo.Dispose();
                        TmpEmpPfmsr.Dispose();
                        TmpEmpPfmwe.Dispose();
                        TmpEmpPlr.Dispose();
                        TmpEmpRecr.Dispose();
                        TmpEmpWpr.Dispose();
                        TmpEmpPfdrec.Dispose();
                        TmpEmppfplrec.Dispose();
                        //TmpEmpcidc.Dispose();

                        //GetEmploymentHistory();
                        GetLeavetHistory();
                        GetSalaryHistory();
                        GetTotalSalary();

                    }

                }

            }

        }

        #endregion

        #region Appointment Code ComboBox Dropdown Event

        protected void hemph_hsgcode_DropDown(object sender, EventArgs e)
        {
            string sql1 = "Select * from HSGM where [status]<>'V'";
            this.dbaccess.ReadSQL("HSGM", sql1);
            DataRow drInsertHSGM = this.dbaccess.DataSet.Tables["HSGM"].NewRow();

            drInsertHSGM["hsgcode"] = "NIL";

            this.dbaccess.DataSet.Tables["HSGM"].Rows.Add(drInsertHSGM);

            hemph_hsgcode.DataSource = this.dbaccess.DataSet.Tables["HSGM"];
            hemph_hsgcode.DisplayMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
            hemph_hsgcode.ValueMember = this.dbaccess.DataSet.Tables["HSGM"].Columns["hsgcode"].ColumnName.ToString();
        }

        #endregion

        #region Save Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {

            base.Document_Save_Handle(sender, e);

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];
            DataTable wlr = this.dbaccess.DataSet.Tables["WLR"];

            //if (BizValidate.CheckRowState(e.DBAccess.DataSet, "hemph"))
            //{
            //    if (MessageBox.Show("Save this Document? \nYes or No?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
            //    {
            //        e.Handle = false;
            //    }
            //}

            if (!BizFunctions.IsEmpty(hemph["MonToFriTimeFrom"]))
            {
                if (hemph["MonToFriTimeFrom"].ToString() == String.Empty)
                {
                    hemph["MonToFriTimeFrom"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["MonToFriTimeFrom"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(hemph["MonToFriTimeTo"]))
            {
                if (hemph["MonToFriTimeTo"].ToString() == String.Empty)
                {
                    hemph["MonToFriTimeTo"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["MonToFriTimeTo"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(hemph["MonToFriLunchHr"]))
            {
                if (Convert.ToDecimal(hemph["MonToFriLunchHr"]) == 0)
                {
                    hemph["MonToFriLunchHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["MonToFriLunchHr"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(hemph["MonToFriTeaBrkHr"]))
            {
                if (Convert.ToDecimal(hemph["MonToFriTeaBrkHr"]) == 0)
                {
                    hemph["MonToFriTeaBrkHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["MonToFriTeaBrkHr"] = System.DBNull.Value;
            }

            ///

            if (!BizFunctions.IsEmpty(hemph["SatTimeFrom"]))
            {
                if (hemph["SatTimeFrom"].ToString() == String.Empty)
                {
                    hemph["SatTimeFrom"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SatTimeFrom"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(hemph["SatTimeTo"]))
            {
                if (hemph["SatTimeTo"].ToString() == String.Empty)
                {
                    hemph["SatTimeTo"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SatTimeTo"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(hemph["SatLunchHr"]))
            {
                if (Convert.ToDecimal(hemph["SatLunchHr"]) == 0)
                {
                    hemph["SatLunchHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SatLunchHr"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(hemph["SatTeaBrkHr"]))
            {
                if (Convert.ToDecimal(hemph["SatTeaBrkHr"]) == 0)
                {
                    hemph["SatTeaBrkHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SatTeaBrkHr"] = System.DBNull.Value;
            }

            ///

            if (!BizFunctions.IsEmpty(hemph["SunTimeFrom"]))
            {
                if (hemph["SunTimeFrom"].ToString() == String.Empty)
                {
                    hemph["SunTimeFrom"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SunTimeFrom"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(hemph["SunTimeTo"]))
            {
                if (hemph["SunTimeTo"].ToString() == String.Empty)
                {
                    hemph["SunTimeTo"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SunTimeTo"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(hemph["SunLunchHr"]))
            {
                if (Convert.ToDecimal(hemph["SunLunchHr"]) == 0)
                {
                    hemph["SunLunchHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SunLunchHr"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(hemph["SunTeaBrkHr"]))
            {
                if (Convert.ToDecimal(hemph["SunTeaBrkHr"]) == 0)
                {
                    hemph["SunTeaBrkHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["SunTeaBrkHr"] = System.DBNull.Value;
            }

            ///

            if (!BizFunctions.IsEmpty(hemph["PHTimeFrom"]))
            {
                if (hemph["PHTimeFrom"].ToString() == String.Empty)
                {
                    hemph["PHTimeFrom"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["PHTimeFrom"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(hemph["PHTimeTo"]))
            {
                if (hemph["PHTimeTo"].ToString() == String.Empty)
                {
                    hemph["PHTimeTo"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["PHTimeTo"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(hemph["PHLunchHr"]))
            {
                if (Convert.ToDecimal(hemph["PHLunchHr"]) == 0)
                {
                    hemph["PHLunchHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["PHLunchHr"] = System.DBNull.Value;
            }


            if (!BizFunctions.IsEmpty(hemph["PHTeaBrkHr"]))
            {
                if (Convert.ToDecimal(hemph["PHTeaBrkHr"]) == 0)
                {
                    hemph["PHTeaBrkHr"] = System.DBNull.Value;
                }
            }
            else
            {
                hemph["PHTeaBrkHr"] = System.DBNull.Value;
            }

            ///


            if (BizFunctions.IsEmpty(hemph["hsgcode"]))
            {
                hemph["hsgcode"] = "L3";
            }

            if (e.Handle && BizFunctions.IsEmpty(hemph["nric"]))
            {
                MessageBox.Show("NRIC Can't be blank.", "BizERP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handle = false;
            }

            if (e.Handle && BizFunctions.IsEmpty(hemph["datejoined"]))
            {
                MessageBox.Show("Date Joined Can't be blank.", "BizERP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handle = false;
            }

            if (e.Handle && BizFunctions.IsEmpty(hemph["empname"]))
            {
                MessageBox.Show("Name Can't be blank.", "BizERP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handle = false;
            }
            if (e.Handle && BizFunctions.IsEmpty(hemph["matnum"]))
            {
                MessageBox.Show("Appointment Can't be blank.", "BizERP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handle = false;
            }

            if (e.Handle && pfmsr.Rows.Count > 0)
            {
                foreach (DataRow dr5 in pfmsr.Rows)
                {
                    if (dr5.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr5["salcode"]))
                        {
                            SalCode = dr5["salcode"].ToString();

                        }
                    }
                }
            }

            decimal lineNo1 = 0;
            foreach (DataRow dr6 in pfmsr.Rows)
            {
                if (dr6.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr6["line"]) || (decimal)dr6["line"] <= 0)
                    {
                        lineNo1 = lineNo1 + 100;
                        dr6["line"] = lineNo1;
                    }
                }
            }


            if (e.Handle)
            {
                if (!CheckSALH())
                {
                    fromSADJ = true;
                    SetSalaryStatus();
                }
                else
                {
                    fromSADJ = false;

                }
            }


            if (e.Handle && pfmsr.Rows.Count > 0)
            {
                foreach (DataRow dr5 in pfmsr.Rows)
                {

                    if (dr5.RowState != DataRowState.Deleted)
                    {
                        if (fromSADJ)
                        {
                            BizFunctions.UpdateDataRow(hemph, dr5, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                        }
                        else
                        {
                            BizFunctions.UpdateDataRow(hemph, dr5, "empnum/nric/user/status/createdby/created/modified");
                        }
                    }
                }
            }

            //if (!BizFunctions.IsEmpty(hemph["empnum"]))
            //{
            //    if (hemph["empnum"].ToString().Contains("HQ"))
            //    {
            //        hemph["cardid"] = "900" + hemph["empnum"].ToString().Replace("HQ", "");
            //    }
            //    else if (hemph["empnum"].ToString().Contains("F"))
            //    {
            //        hemph["cardid"] = "10" + hemph["empnum"].ToString().Replace("F", "");
            //    }

            //    else if (hemph["empnum"].ToString().Contains("SP"))
            //    {
            //        hemph["cardid"] = "300" + hemph["empnum"].ToString().Replace("SP", "").Replace("-", "");
            //    }
            //    else if (hemph["empnum"].ToString().Contains("P"))
            //    {
            //        hemph["cardid"] = "200" + hemph["empnum"].ToString().Replace("P", "");
            //    }
            //    else
            //    {
            //        hemph["cardid"] = hemph["empnum"];
            //    }

            //    DataTable TableCardID = new DataTable();
            //    string checkCardID = "SELECT EMPNUM FROM HEMPH WHERE CARDID='" + hemph["cardid"].ToString() + "' and empnum<>'" + hemph["empnum"].ToString() + "'";

            //    this.dbaccess.ReadSQL("TableCardID", checkCardID);

            //    TableCardID = this.dbaccess.DataSet.Tables["TableCardID"];

            //    if (TableCardID.Rows.Count > 0)
            //    {
            //        MessageBox.Show("Card ID for Biometrics already Exists, Please choose a different Card ID", "BizERP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        hemph["cardid"] = string.Empty;
            //        e.Handle = false;
            //    }
            //}

            decimal wlrlineNo1 = 0;
            if (wlr.Rows.Count > 0)
            {
                foreach (DataRow drWlr in wlr.Rows)
                {
                    if (drWlr.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(drWlr["dateissued"]))
                        {
                            drWlr["dateissued"] = DateTime.Now;
                        }

                        if (BizFunctions.IsEmpty(drWlr["line"]) || (decimal)drWlr["line"] <= 0)
                        {
                            wlrlineNo1 = wlrlineNo1 + 100;
                            drWlr["line"] = wlrlineNo1;
                        }

                        if (BizFunctions.IsEmpty(drWlr["typeofwarning1"]))
                        {
                            drWlr["typeofwarning1"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["typeofwarning2"]))
                        {
                            drWlr["typeofwarning2"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["typeofwarning3"]))
                        {
                            drWlr["typeofwarning3"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["o1"]))
                        {
                            drWlr["o1"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["o2"]))
                        {
                            drWlr["o2"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["o3"]))
                        {
                            drWlr["o3"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["o4"]))
                        {
                            drWlr["o4"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["o5"]))
                        {
                            drWlr["o5"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["o6"]))
                        {
                            drWlr["o6"] = 0;
                        }

                        if (BizFunctions.IsEmpty(drWlr["o7"]))
                        {
                            drWlr["o7"] = 0;
                        }

                        //if (BizFunctions.IsEmpty(drWlr["wldoc"]))
                        //{
                        //    drWlr["wldoc"] = BizLogicTools.Tools.DefaultVarbinary();
                        //}
                    }
                }
            }

        }

        #endregion

        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            switch (e.ControlName)
            {
                case "hemph_nric":
                    {

                        e.Condition = BizFunctions.F2Condition("nric,empnum,empname", (sender as TextBox).Text);
                        e.DefaultCondition = "isapproved<>0 ";

                    }
                    break;
                case "hemph_hranum":
                    {

                        e.Condition = BizFunctions.F2Condition("hramnum,hramdesc", (sender as TextBox).Text);
                        //e.DefaultCondition = "hramnum like '" + hemph["hramnum"].ToString() + "%' OR hramdesc like '" + hemph["hramnum"].ToString() + "%'";

                    }
                    break;

                case "hemph_religion":
                    {

                        e.Condition = BizFunctions.F2Condition("religion,remark", (sender as TextBox).Text);
                        //e.DefaultCondition = "religion like '" + hemph["religion"].ToString() + "%' OR remark like '" + hemph["religion"].ToString() + "%'";

                    }
                    break;

                case "hemph_regname":
                    {

                        e.Condition = BizFunctions.F2Condition("regnum,regname", (sender as TextBox).Text);
                        //e.DefaultCondition = "regname like '" + hemph["regname"].ToString() + "%' OR regnum like '" + hemph["regname"].ToString() + "%'";

                    }
                    break;

                case "hemph_sitenum":
                    {

                        e.Condition = BizFunctions.F2Condition("sitenum,sitename", (sender as TextBox).Text);
                        //e.DefaultCondition = "sitenum like '" + hemph["sitenum"].ToString() + "%' OR sitename like '" + hemph["sitenum"].ToString() + "%'";

                    }
                    break;
                case "hemph_statuscode":
                    {

                        e.Condition = BizFunctions.F2Condition("statuscode,remark", (sender as TextBox).Text);
                        //e.DefaultCondition = "statuscode like '" + hemph["statuscode"].ToString() + "%' OR remark like '" + hemph["statuscode"].ToString() + "%'";

                    }
                    break;

                case "hemph_matnum":
                    {

                        e.Condition = BizFunctions.F2Condition("matnum,matname", (sender as TextBox).Text);
                        //e.DefaultCondition = "matnum like '" + hemph["matnum"].ToString() + "%' OR matname like '" + hemph["matnum"].ToString() + "%'";

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
                case "matnum":
                    {
                        e.Condition = BizFunctions.F2Condition("matnum,matname", (sender as TextBox).Text);
                    }
                    break;
                case "hsamcode":
                    {
                        e.Condition = BizFunctions.F2Condition("hsamcode,[desc]", (sender as TextBox).Text);
                    }
                    break;

            }
        }
        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            switch (e.ControlName)
            {
                case "hemph_nric":
                    e.CurrentRow["nric"] = e.F2CurrentRow["nric"];
                    if (!BizFunctions.IsEmpty(e.CurrentRow["nric"]))
                    {
                        if (!BizFunctions.IsEmpty(e.F2CurrentRow["empnum"]))
                        {
                            if (!BizFunctions.IsEmpty(e.F2CurrentRow["flag"]))
                            {
                                if (e.F2CurrentRow["flag"].ToString() == "HEMPN")
                                {
                                    e.CurrentRow["empnum"] = e.F2CurrentRow["empnum"];
                                    GetEmployeeData();
                                }
                            }
                            else
                            {
                                GetEmployeeData();
                            }
                        }
                        else
                        {
                            GetEmployeeData2();
                        }
                    }
                    break;

                case "hemph_matnum":
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    break;

                case "hemph_ethnicity":
                    e.CurrentRow["ethnicity"] = e.F2CurrentRow["ethnicity"];
                    break;

                case "hemph_regname":
                    e.CurrentRow["regname"] = e.F2CurrentRow["regname"];
                    break;

                case "hemph_statuscode":
                    e.CurrentRow["statuscode"] = e.F2CurrentRow["statuscode"];
                    break;

                case "hemph_religion":
                    e.CurrentRow["religion"] = e.F2CurrentRow["religion"];
                    break;
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            switch (e.MappingName)
            {
                case "nricrecomm":
                    ////e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    break;

                case "coursecode":
                    e.CurrentRow["coursedesc"] = e.F2CurrentRow["coursename"];
                    break;

                case "apnum":
                    e.CurrentRow["Institution"] = e.F2CurrentRow["apname"];
                    break;

            }
        }

        #endregion

        #region ReOpen/void

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

        #region Voucher Condition
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {
            //For status 'O' & 'P'
            base.AddVoucherAllCondition(e);
            //e.Condition = " HEMPH.isHQstaff=0 ";
            e.Condition = " HEMPH.FLAG='" + EmpFlag + "' and HEMPH.isApproved=1 ";
        }
        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {
            //For status 'O' & 'V'
            base.AddVoucherDefaultCondition(e);
            e.Condition = " HEMPH.FLAG='" + EmpFlag + "' and HEMPH.status='O' and HEMPH.isApproved=1  ";

        }
        #endregion

        #region Save Begin

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

            DataRow hemph = e.DBAccess.DataSet.Tables["hemph"].Rows[0];
            DataTable pfmedu = e.DBAccess.DataSet.Tables["pfmedu"];
            DataTable recr = e.DBAccess.DataSet.Tables["recr"];
            DataTable wpr = e.DBAccess.DataSet.Tables["wpr"];
            DataTable pfmwe = e.DBAccess.DataSet.Tables["pfmwe"];
            DataTable pfmsr = e.DBAccess.DataSet.Tables["pfmsr"];
            DataTable famr = e.DBAccess.DataSet.Tables["famr"];
            DataTable plr = e.DBAccess.DataSet.Tables["plr"];
            DataTable hcnr = e.DBAccess.DataSet.Tables["hcnr"];
            DataTable pfdrec = e.DBAccess.DataSet.Tables["pfdrec"];
            DataTable pfpr = e.DBAccess.DataSet.Tables["pfpr"];
            DataTable pfplrec = e.DBAccess.DataSet.Tables["pfplrec"];
            //DataTable cidc = e.DBAccess.DataSet.Tables["cidc"];
            DataTable pffa = e.DBAccess.DataSet.Tables["pffa"];
            DataTable wlr = e.DBAccess.DataSet.Tables["wlr"];
            DataTable xpfdrec = pfdrec.GetChanges(DataRowState.Deleted);

            #region Generate barcode

            if (hemph["empnum"].ToString() == "NEW EMPLOYEE")
            {
                if (hemph["hsgcode"].ToString().Trim() == "L3")
                {
                    DataTable dtempnum = dbaccess.ReadSQLTemp("dtempnum", "select isnull(max(empnum),0) as empnum from HEMPH").Tables["dtempnum"];
                    int code = Int32.Parse(dtempnum.Rows[0]["empnum"].ToString()) + 1;
                    hemph["empnum"] = code.ToString("D5");
                }


                //this.VoucherBase.SearchTextBox.Text = hemph["empnum"].ToString();
            }
            //if (matm["empnum"]. )
            //{
            //    //Jason:(27/06/2014) Changed due to error on very long int after the importation of 2nd hand items
            //    //DataTable dtempnum = dbaccess.ReadSQLTemp("dtempnum", "select isnull(max(empnum),0) as empnum from matm ").Tables["dtempnum"];
            //    DataTable dtempnum = dbaccess.ReadSQLTemp("dtempnum", "select isnull(max(empnum),0) as empnum from HEMPH").Tables["dtempnum"];
            //    int code = Int32.Parse(dtempnum.Rows[0]["empnum"].ToString()) + 1;
            //    matm["empnum"] = code.ToString("D8");
            //}
            //else if (!BizFunctions.IsEmpty(matm["empnum"]))
            //{
            //    if (matm["empnum"].ToString().Length <= 8)
            //    {
            //        int empnum = Int32.Parse(matm["empnum"].ToString());
            //        matm["empnum"] = empnum.ToString("D8");
            //    }
            //}
            #endregion

            #region Validations
            if (hemph_datejPR.Enabled == false)
            {
                hemph["datePR"] = System.DBNull.Value;
            }

            if (!BizFunctions.IsEmpty(hemph["datejoined"]))
            {
                if (!GetConfirmationDate())
                {
                    DateTime dt = Convert.ToDateTime(hemph["datejoined"]).AddMonths(3);
                    if (!hemph["empnum"].ToString().Contains("P"))
                    {
                        txt_dateconfirmed.Text = dt.ToShortDateString();
                    }
                }
            }

            if (!BizFunctions.IsEmpty(hemph["empnum"]))
            {
                hemph["empnum"] = hemph["empnum"].ToString().Trim();
            }

            if (BizFunctions.IsEmpty(hemph["statuscode"]) || hemph["statuscode"].ToString() == "")
            {

                string partOfempnum = hemph["empnum"].ToString();
                partOfempnum = partOfempnum.Substring(0, 2);
                partOfempnum = partOfempnum + "%";

                string GetStatusCodeHETM = "Select statuscode from HETM where hetmnum like '" + partOfempnum + "' ";

                DataTable tmpGetStatus = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetStatusCodeHETM);

                if (tmpGetStatus.Rows.Count > 0)
                {

                    DataRow drTmpGetStatus = tmpGetStatus.Rows[0];

                    hemph["statuscode"] = drTmpGetStatus["statuscode"].ToString();
                }
            }

            if (!BizFunctions.IsEmpty(hemph["dob"]))
            {
                age.Text = Convert.ToString(GetAge());
                int IntAge = Convert.ToInt32(age.Text.ToString().Trim());
                if (IntAge <= 13)
                {
                    MessageBox.Show("Age can't be 13 years old and below.", "BizERP", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }

            this.dbaccess.ReadSQL("MaxID", "Select ISNULL(MAX(SystemNumber),0) + 1 AS sysno FROM DOCFILES");
            DataRow MaxID = dbaccess.DataSet.Tables["MaxID"].Rows[0];
            int count = Convert.ToInt32(MaxID["sysno"]);

            if (!BizFunctions.IsEmpty(hemph["empnum"]))
            {
                hemph["refnum"] = hemph["empnum"];
            }
            #endregion

            #region Family Records

            foreach (DataRow dr1 in famr.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hemph, dr1, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                }

            }

            #endregion

            #region Contact Records

            foreach (DataRow dr2 in hcnr.Rows)
            {
                if (dr2.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hemph, dr2, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                }

            }

            #endregion

            #region Education Records

            foreach (DataRow dr3 in pfmedu.Rows)
            {
                if (dr3.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hemph, dr3, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                }

            }

            #endregion

            #region Salary Record

            decimal lineNo1 = 0;

            foreach (DataRow dr5 in pfmsr.Rows)
            {


                if (dr5.RowState != DataRowState.Deleted)
                {

                    BizFunctions.UpdateDataRow(hemph, dr5, "empnum/nric/sitenum/refnum/user/flag/status/createdby/created/modified");

                    if (BizFunctions.IsEmpty(dr5["adjustmentflag"]))
                    {
                        dr5["adjustmentflag"] = hemph["flag"];
                    }

                    if (hemph["flag"].ToString().Trim() == (string)dr5["adjustmentflag"])
                    {
                        dr5["dateadjusted"] = hemph["datejoined"];
                    }
                    if (!BizFunctions.IsEmpty(hemph["datejoined"]))
                    {
                        if (BizFunctions.IsEmpty(dr5["salcode"]))
                        {
                            dr5["salcode"] = hemph["empnum"].ToString() + "-" + BizFunctions.GetSafeDateString(Convert.ToDateTime(hemph["datejoined"]));
                        }
                    }
                    if (BizFunctions.IsEmpty(dr5["hsgcode"]))
                    {
                        dr5["hsgcode"] = hemph["hsgcode"];
                    }
                    if (!BizFunctions.IsEmpty(dr5["salcode"]))
                    {
                        SalCode = dr5["salcode"].ToString();
                    }
                }
            }

            #endregion

            #region Work Experience

            foreach (DataRow dr6 in pfmwe.Rows)
            {
                if (dr6.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hemph, dr6, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                }

            }
            #endregion

            #region Permits and Licences Record

            foreach (DataRow dr8 in plr.Rows)
            {
                if (dr8.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hemph, dr8, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                }

            }
            #endregion

            #region Recommendation Incentives Record

            foreach (DataRow dr9 in recr.Rows)
            {
                if (dr9.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hemph, dr9, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                }

            }

            #endregion

            #region Work Permit Records

            foreach (DataRow dr10 in wpr.Rows)
            {
                if (dr10.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(hemph, dr10, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
                }

            }
            #endregion

            #region Personal Files/Docs Record
            if (enableDocSave)
            {
                if (pfdrec.Rows.Count > 0)
                {
                    try
                    {

                        string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                        if (DriveLetter.Trim() != "")
                        {

                            foreach (DataRow dr11 in pfdrec.Rows)
                            {

                                if (dr11.RowState != DataRowState.Deleted)
                                {
                                    BizFunctions.UpdateDataRow(hemph, dr11, "empnum/nric/refnum/user/flag/status/created/modified");

                                    if (BizFunctions.IsEmpty(dr11["physicalserverlocation"]))
                                    {
                                        FileSendGet fsg1 = new FileSendGet(DriveLetter.Trim(), dr11["templocation"].ToString(), hemph["empnum"].ToString(), "EDU");
                                        if (!fsg1.FileUploadSuccess)
                                        {
                                            try
                                            {
                                                dr11.Delete();
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                        else
                                        {
                                            dr11["physicalserverlocation"] = fsg1.FileInServerLocation;
                                            dr11.SetAdded();
                                        }

                                    }
                                    else
                                    {
                                        if (!File.Exists(dr11["physicalserverlocation"].ToString()))
                                        {
                                            FileSendGet fsg2 = new FileSendGet(DriveLetter.Trim(), dr11["templocation"].ToString(), hemph["empnum"].ToString(), "EDU");
                                            if (!fsg2.FileUploadSuccess)
                                            {
                                                try
                                                {
                                                    dr11.Delete();
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                            }
                                            else
                                            {
                                                dr11["physicalserverlocation"] = fsg2.FileInServerLocation;
                                                dr11.SetAdded();
                                            }
                                        }
                                    }

                                }

                            }

                        }
                        else
                        {
                            MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        NetworkDrive.DisconnectNetworkDrive(true);
                    }
                }
            }

            #endregion

            #region Permits / Docs Record
            if (enableDocSave)
            {
                if (pfplrec.Rows.Count > 0)
                {
                    try
                    {
                        string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                        if (DriveLetter.Trim() != "")
                        {

                            foreach (DataRow dr12 in pfplrec.Rows)
                            {

                                if (dr12.RowState != DataRowState.Deleted)
                                {
                                    BizFunctions.UpdateDataRow(hemph, dr12, "empnum/nric/refnum/user/flag/status/created/modified");

                                    if (BizFunctions.IsEmpty(dr12["physicalserverlocation"]))
                                    {
                                        FileSendGet fsg1 = new FileSendGet(DriveLetter.Trim(), dr12["templocation"].ToString(), hemph["empnum"].ToString(), "PMT");
                                        if (!fsg1.FileUploadSuccess)
                                        {
                                            dr12.Delete();
                                        }
                                        else
                                        {
                                            dr12["physicalserverlocation"] = fsg1.FileInServerLocation;
                                            dr12.SetAdded();
                                        }

                                    }
                                    else
                                    {
                                        if (!File.Exists(dr12["physicalserverlocation"].ToString()))
                                        {
                                            FileSendGet fsg2 = new FileSendGet(DriveLetter.Trim(), dr12["templocation"].ToString(), hemph["empnum"].ToString(), "PMT");
                                            if (!fsg2.FileUploadSuccess)
                                            {
                                                dr12.Delete();
                                            }
                                            else
                                            {
                                                dr12["physicalserverlocation"] = fsg2.FileInServerLocation;
                                                dr12.SetAdded();
                                            }
                                        }
                                    }

                                }

                            }

                        }
                        else
                        {
                            MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        NetworkDrive.DisconnectNetworkDrive(true);
                    }
                }
            }
            #endregion

            #region Save the picture to database
            MemoryStream ms = new MemoryStream();

            // Jason
            //if (pb.Image == null)
            //{
            //    pb.SizeMode = PictureBoxSizeMode.StretchImage;
            //    string projectPath = ConfigurationManager.AppSettings.Get("ImagePath");
            //    Image img = Image.FromFile(projectPath + @"\noimage.JPG");
            //    pb.Image = img;
            //}


            if (pb.Image == null && !BizFunctions.IsEmpty(hemph["photourl"]) && BizFunctions.IsEmpty(hemph["photo"]))
            {
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                Image img = Image.FromFile(hemph["photourl"].ToString());
                pb.Image = img;
            }
            else if (pb.Image == null && BizFunctions.IsEmpty(hemph["photourl"]) && BizFunctions.IsEmpty(hemph["photo"]))
            {
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                string projectPath = ConfigurationManager.AppSettings.Get("ImagePath");
                Image img = Image.FromFile(projectPath + @"\noimage.JPG");
                pb.Image = img;
            }

            pb.Image.Save(ms, pb.Image.RawFormat);
            //Read from MemoryStream into Byte array.
            Byte[] bytBLOBData = new Byte[ms.Length];
            ms.Position = 0;
            ms.Read(bytBLOBData, 0, Convert.ToInt32(ms.Length));
            hemph["photo"] = bytBLOBData;

            if (enableDocSave)
            {
                string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                try
                {
                    string SaveLoc = DriveLetter1 + ":";
                    System.IO.DirectoryInfo fl = new DirectoryInfo(SaveLoc + @"\\Photos\\" + hemph["empnum"].ToString().Trim() + "\\");

                    if (!fl.Exists)
                    {
                        System.IO.Directory.CreateDirectory(fl.FullName);
                    }

                    string ServerLocation = System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository") + "\\Photos\\" + hemph["empnum"].ToString().Trim() + "\\" + hemph["empnum"].ToString().Trim() + ".jpg";
                    pb.Image.Save(fl.FullName + "\\" + hemph["empnum"].ToString().Trim() + ".jpg");
                    hemph["photourl"] = ServerLocation;

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    NetworkDrive.DisconnectNetworkDrive(true);
                }
            }


            #endregion

            #region Add xPFDRECT - pfdrec.GetChanges(DataRowState.Deleted)

            if (xpfdrec != null)
            {

                if (this.dbaccess.DataSet.Tables.Contains("xpfdrec"))
                {
                    this.dbaccess.DataSet.Tables.Remove("xpfdrec");
                    xpfdrec = pfdrec.GetChanges();

                    xpfdrec.TableName = "xpfdrec";

                    this.dbaccess.DataSet.Tables.Add(xpfdrec);
                }
                else
                {
                    xpfdrec.TableName = "xpfdrec";

                    this.dbaccess.DataSet.Tables.Add(xpfdrec);
                }
            }

            #endregion

            #region Emp Flag
            if (EmpFlag != string.Empty)
            {
                if (EmpFlag == "HEMP")
                {
                    hemph["isHQstaff"] = 1;
                }
                else
                {
                    hemph["isHQstaff"] = 0;
                }

                if (BizFunctions.IsEmpty(hemph["isHQstaff"]))
                {
                    hemph["isHQstaff"] = 0;
                }
            }
            #endregion

            #region CIDC

            //foreach (DataRow dr20 in cidc.Rows)
            //{
            //    if (dr20.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(hemph, dr20, "empnum/nric/refnum/user/flag/status/createdby/created/modified");
            //    }

            //}

            #endregion

            #region Others

            DataTable xpffa = pffa.GetChanges(DataRowState.Deleted);


            #region Folder Archive
            if (enableDocSave)
            {
                if (pffa.Rows.Count > 0)
                {
                    try
                    {

                        string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                        if (DriveLetter.Trim() != "")
                        {

                            foreach (DataRow dr11 in pffa.Rows)
                            {

                                if (dr11.RowState != DataRowState.Deleted)
                                {
                                    BizFunctions.UpdateDataRow(hemph, dr11, "refnum/user/flag/status/created/modified");

                                    if (BizFunctions.IsEmpty(dr11["physicalserverlocation"]))
                                    {
                                        FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), hemph["empnum"].ToString(), Convert.ToDateTime(hemph["datejoined"]), dr11["flname"].ToString());
                                        if (!fsg1.FileUploadSuccess)
                                        {
                                            try
                                            {
                                                dr11.Delete();
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        else
                                        {
                                            dr11["physicalserverlocation"] = fsg1.FileInServerLocation;
                                            dr11["filedesc"] = fsg1.finalSzipFileName;

                                        }

                                    }
                                    else
                                    {
                                        if (!File.Exists(dr11["physicalserverlocation"].ToString()))
                                        {
                                            FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), hemph["empnum"].ToString(), Convert.ToDateTime(hemph["datejoined"]), dr11["flname"].ToString());
                                            if (!fsg1.FileUploadSuccess)
                                            {
                                                try
                                                {
                                                    dr11.Delete();
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                            }
                                            else
                                            {
                                                dr11["physicalserverlocation"] = fsg1.FileInServerLocation;
                                                dr11["filedesc"] = fsg1.finalSzipFileName;

                                            }
                                        }
                                    }


                                }

                            }

                        }
                        else
                        {
                            MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        NetworkDrive.DisconnectNetworkDrive(true);
                    }
                }
            }
            #endregion

            if (xpffa != null)
            {

                if (this.dbaccess.DataSet.Tables.Contains("xpffa"))
                {
                    this.dbaccess.DataSet.Tables.Remove("xpffa");
                    xpffa = pffa.GetChanges(DataRowState.Deleted);

                    xpffa.TableName = "xpffa";

                    this.dbaccess.DataSet.Tables.Add(xpffa);
                }
                else
                {
                    xpffa.TableName = "xpffa";

                    this.dbaccess.DataSet.Tables.Add(xpffa);
                }
            }
            #endregion

            #region Write Document
            if (enableDocSave)
            {

                WordForm3 wf1 = new WordForm3(this.dbaccess, "HEMPH", "contractdoc", "CONTRACT");

                if (!wf1.SaveToFile())
                {
                    MessageBox.Show("Unable to Save Service Agreement Document, Please try again later.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    hemph["contractloc"] = wf1.FileInServerLocation;
                }

                //if (wlr.Rows.Count > 0)
                //{

                //    for (int i = 0; i < wlr.Rows.Count; i++)
                //    {
                //        if (wlr.Rows[i].RowState != DataRowState.Deleted)
                //        {

                //                WordForm4 wf2 = new WordForm4(this.dbaccess, "WLR", "wldoc", "WARNING", i);
                //                if (!wf2.SaveToFile())
                //                {
                //                    MessageBox.Show("Unable to Save Warning Letter, Please try again later.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //                }
                //                else
                //                {
                //                    wlr.Rows[i]["wldocloc"] = wf2.FileInServerLocation;
                //                }

                //        }
                //    }
                //}
            }
            #endregion

        }

        #endregion

        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataRow hemph = e.DBAccess.DataSet.Tables["hemph"].Rows[0];
            DataTable pfmsr = e.DBAccess.DataSet.Tables["pfmsr"];
            DataTable xpfdrec = e.DBAccess.DataSet.Tables["xpfdrec"];
            DataTable xpfplrec = e.DBAccess.DataSet.Tables["xpfplrec"];

            string strsalh = "Select * from salh where 1=2";

            this.dbaccess.ReadSQL("SALH", strsalh);

            DataTable salh = e.DBAccess.DataSet.Tables["salh"];

            if (enableDocSave)
            {
                #region  Make Save Changes in Education Doc Files

                if (xpfdrec != null)
                {
                    try
                    {

                        string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                        if (DriveLetter1.Trim() != "")
                        {
                            if (xpfdrec.Rows.Count > 0)
                            {

                                foreach (DataRow dr1 in xpfdrec.Rows)
                                {

                                    if (dr1.RowState == DataRowState.Deleted)
                                    {
                                        System.IO.File.Delete(dr1["physicalserverlocation", DataRowVersion.Original].ToString());
                                    }

                                }


                            }
                        }
                    }

                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        NetworkDrive.DisconnectNetworkDrive(true);
                        BizFunctions.DeleteAllRows(xpfdrec);
                    }


                    NetworkDrive.DisconnectNetworkDrive(true);
                }
                #endregion

                #region  Make Save Changes in Education Doc Files

                if (xpfplrec != null)
                {
                    try
                    {
                        string DriveLetter2 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                        if (DriveLetter2.Trim() != "")
                        {
                            if (xpfplrec.Rows.Count > 0)
                            {
                                foreach (DataRow dr2 in xpfplrec.Rows)
                                {

                                    if (dr2.RowState == DataRowState.Deleted)
                                    {
                                        System.IO.File.Delete(dr2["physicalserverlocation", DataRowVersion.Original].ToString());
                                    }

                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {

                        NetworkDrive.DisconnectNetworkDrive(true);
                        BizFunctions.DeleteAllRows(xpfplrec);
                    }
                }


                #endregion

                DataTable xpffa = this.dbaccess.DataSet.Tables["xpffa"];

                #region  Make Save Changes in Education Doc Files

                if (xpffa != null)
                {
                    try
                    {

                        string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                        if (DriveLetter1.Trim() != "")
                        {
                            if (xpffa.Rows.Count > 0)
                            {

                                foreach (DataRow dr1 in xpffa.Rows)
                                {

                                    if (dr1.RowState == DataRowState.Deleted)
                                    {
                                        System.IO.File.Delete(dr1["physicalserverlocation", DataRowVersion.Original].ToString());
                                    }

                                }


                            }
                        }
                    }

                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        NetworkDrive.DisconnectNetworkDrive(true);
                        BizFunctions.DeleteAllRows(xpffa);

                        if (this.dbaccess.DataSet.Tables.Contains("xpffa"))
                        {
                            this.dbaccess.DataSet.Tables.Remove("xpffa");
                        }
                    }


                    NetworkDrive.DisconnectNetworkDrive(true);
                }
                #endregion
            }

            if (BizFunctions.IsEmpty(hemph["cardid"]))
            {

            }

            if (!fromSADJ)
            {

                GetTotalSalary();
                decimal TmpSal = Convert.ToDecimal(txt_TotalSal.Text);
                //if (TmpSal > 0)
                //{

                #region Personal Salary Record --> Salary Record History

                if (hemph["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO)
                {
                    BizFunctions.DeleteAllRows(salh);
                    foreach (DataRow dr12 in pfmsr.Rows)
                    {
                        if (dr12.RowState != DataRowState.Deleted)
                        {

                            DataRow InsertSalh = salh.NewRow();
                            InsertSalh["refnum"] = dr12["refnum"];
                            InsertSalh["empnum"] = dr12["empnum"];
                            InsertSalh["nric"] = dr12["nric"];
                            InsertSalh["matnum"] = hemph["matnum"];
                            InsertSalh["rateamt"] = dr12["rateamt"];
                            InsertSalh["hsamcode"] = dr12["hsamcode"];
                            InsertSalh["remarks"] = dr12["remarks"];
                            InsertSalh["adjustmentflag"] = dr12["adjustmentflag"];
                            InsertSalh["dateadjusted"] = dr12["dateadjusted"];
                            InsertSalh["hsgcode"] = dr12["hsgcode"];
                            InsertSalh["salcode"] = dr12["salcode"];
                            InsertSalh["sitenum"] = dr12["sitenum"];
                            InsertSalh["isExempt"] = dr12["isExempt"];
                            InsertSalh["line"] = dr12["line"];
                            salh.Rows.Add(InsertSalh);
                        }
                    }
                    //}

                #endregion

                    #region Update rows for SALH - Salary Record History

                    foreach (DataRow dr13 in salh.Rows)
                    {
                        if (dr13.RowState != DataRowState.Deleted)
                        {
                            BizFunctions.UpdateDataRow(hemph, dr13, "empnum/nric/sitenum/refnum/user/status/createdby/created/modified");


                        }

                    }
                    #endregion

                    #region Assign ids to salh table for saving

                    string maxid = "SELECT ISNULL(MAX(id),0) AS 'id' FROM salh";
                    DataSet maxtmp = this.dbaccess.ReadSQLTemp("idsalh", maxid);

                    int a = 0;
                    if (maxtmp.Tables["idsalh"].Rows.Count > 0)
                    {
                        if (maxtmp.Tables["idsalh"].Rows[0]["id"] == System.DBNull.Value)
                            a = 0;
                        else
                            a = Convert.ToInt32(maxtmp.Tables["idsalh"].Rows[0]["id"]) + 1;
                    }
                    else
                    {
                        a = 0;
                    }

                    foreach (DataRow dr in salh.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            dr["id"] = a;
                            a++;
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

                        DataTable[] dataTablestemp = new DataTable[1];
                        dataTablestemp[0] = salh;
                        dataTablestemp[0].TableName = salh.TableName.ToString();



                        // Delete this current refnum first.	
                        //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM salh WHERE salcode = '" + SalCode + "'");
                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM salh WHERE refnum = '" + hemph["empnum"].ToString() + "'");

                        this.dbaccess.Update(dataTablestemp);
                        //this.dbaccess.Update(e.SessionID, "SALH", "SALH");

                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE sysid set lastid = (Select ISNULL(max(id),0) from salh) where tablename = 'SALH'");
                        //remoteDBAccess.DataSet.Tables.Clear();

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }
            }
            GetSalaryHistory();
            GetTotalSalary();

            //string deleteWebNRIC = "Delete from hemph where nric='"+hemph["nric"].ToString().Trim()+"' and ISNULL(isWebAdd,0)=1";
            //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(deleteWebNRIC);
        }

        # endregion

        #region GetStatus

        private void GetStatus()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];


            string GetStatus = "Select statuscode from hemph where empnum='" + hemph["empnum"].ToString() + "'";

            this.dbaccess.ReadSQL("dtGetStatus", GetStatus);

            DataTable dtGetStatus = this.dbaccess.DataSet.Tables["dtGetStatus"];

            if (dtGetStatus.Rows.Count > 0)
            {
                hemph["statuscode"] = this.dbaccess.DataSet.Tables["dtGetStatus"].Rows[0]["statuscode"];
            }



        }

        #endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow hemph = e.DBAccess.DataSet.Tables["hemph"].Rows[0];

            if (hemph["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO || hemph["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                GetSalaryHistory();
                GetConfirmationDate();
                GetResignationDate();
                GetTotalSalary();
                GetLeavetHistory();
                //GetEmploymentHistory();
                GetTrainingtHistory();
                //GetStatus();
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

        //protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        //{
        //    base.Document_Preview_Handle(sender, e);

        //}

        #endregion

        #region Print on Click

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

            switch (e.ReportName)
            {

                case "Curriculum Vitae 1":
                    e.DataSource = hemphds1();
                    break;

                case "Curriculum Vitae 2":
                    e.DataSource = hemphds1();
                    break;

                case "Letter Of Appointment":
                    e.DataSource = LAds1();
                    break;

                case "Giro Opt-Out Form":
                    e.DataSource = GiRods1();
                    break;

                case "Recommendation Form":
                    e.DataSource = Recods1();
                    break; 
                default :
                      e.DataSource = dbaccess.DataSet;
                    break;
            }


        }

        #endregion

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);
            DataRow hemph = dbaccess.DataSet.Tables["hemph"].Rows[0];

            if (hemph["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "hemph"))
                {
                    MessageBox.Show("Please Summarize then Save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
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

        private void GetSignatureWarning()
        {
            DataTable wlr = this.dbaccess.DataSet.Tables["wlr"];

            DataTable SigTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select empnum,userSignature as signaturepicloc,appraiserSignature as girosigLoc from wlr where [guid]='" + wlr.Rows[dg_warning.CurrentCell.RowNumber]["guid"].ToString() + "'");

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

        #region HEMPH DS Report

        private DataSet hemphds1()
        {
            DataSet ds1 = new DataSet("HEMPHds1");

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            DataTable pfmedu = this.dbaccess.DataSet.Tables["pfmedu"];
            DataTable recr = this.dbaccess.DataSet.Tables["recr"];
            DataTable wpr = this.dbaccess.DataSet.Tables["wpr"];
            DataTable PFMWE = this.dbaccess.DataSet.Tables["PFMWE"];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["pfmsr"];
            DataTable famr = this.dbaccess.DataSet.Tables["famr"];
            DataTable plr = this.dbaccess.DataSet.Tables["plr"];
            DataTable hcnr = this.dbaccess.DataSet.Tables["hcnr"];
            DataTable pfdrec = this.dbaccess.DataSet.Tables["pfdrec"];
            DataTable pfpr = this.dbaccess.DataSet.Tables["pfpr"];
            DataTable pfplrec = this.dbaccess.DataSet.Tables["pfplrec"];

            foreach (DataRow drWE in PFMWE.Rows)
            {
                if (drWE.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(drWE["issecurityrelated"]))
                    {
                        drWE["issecurityrelated"] = 0;
                    }

                }
            }

            DataTable HEMPH1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM HEMPH");
            DataTable PFMEDU1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM PFMEDU where isHighest=1");
            DataTable PFMEDU2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM PFMEDU where issecurityrelated=1");
            DataTable RECR1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM RECR");
            DataTable WPR1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM WPR");
            DataTable PFMWE1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM PFMWE where issecurityrelated=1 order by yearfrom");
            DataTable PFMWE2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM PFMWE where issecurityrelated=0 order by yearfrom");

            PFMWE1.Columns.Add("Period", typeof(string));
            PFMWE2.Columns.Add("Period", typeof(string));

            if (PFMWE1.Rows.Count > 0)
            {


                foreach (DataRow dr1 in PFMWE1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["yearfrom"]) && !BizFunctions.IsEmpty(dr1["yearto"]))
                        {
                            dr1["period"] = "From " + dr1["yearfrom"].ToString() + " To " + dr1["yearto"].ToString();
                        }
                    }
                }
            }

            if (PFMWE2.Rows.Count > 0)
            {


                foreach (DataRow dr2 in PFMWE2.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr2["yearfrom"]) && !BizFunctions.IsEmpty(dr2["yearto"]))
                        {
                            dr2["period"] = "From " + dr2["yearfrom"].ToString() + " To " + dr2["yearto"].ToString();
                        }
                    }
                }
            }

            DataTable FAMR1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM FAMR");
            DataTable PLR1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM PLR");
            DataTable HCNR1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM HCNR");

            string GetCDC = "SELECT A.empnum,A.[Desc],A.[EStatus] " +
                                "FROM ( " +
                                        "Select unP.empnum, 'TRAINING STATUS' as [Desc],unP.estatus2 as [EStatus] " +
                                        "from " +
                                        "(  " +
                                            "select empnum, trainningstatus from CIDC where empnum='" + hemph["empnum"].ToString() + "' " +
                                        ") as p UNPIVOT " +
                                        "( estatus2 for [estatus3] in (trainningstatus) )unP " +
                                ")A  " +
                                "UNION ALL " +
                                "SELECT B.empnum,B.[Desc],B.[EStatus] " +
                                "FROM ( " +
                                        "Select unP.empnum, 'LICENCE STATUS' as [Desc],unP.estatus3 as [EStatus]  " +
                                        "from " +
                                        "(  " +
                                            "select empnum, licencsestatus from CIDC where empnum='" + hemph["empnum"].ToString() + "' " +
                                        ") as p UNPIVOT " +
                                        "( estatus3 for [estatus4] in (licencsestatus) )unP " +
                                ")B  ";

            this.dbaccess.ReadSQL("tmpCDC", GetCDC);

            DataTable tmpCDC = this.dbaccess.DataSet.Tables["tmpCDC"];

            DataTable CIDC1 = tmpCDC.Copy();

            CIDC1.TableName = "CIDC1";

            this.dbaccess.ReadSQL("MATMtmp", "SELECT * FROM MATM WHERE MATNUM='" + hemph["matnum"].ToString() + "'");

            DataTable MATM1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select * from MATMtmp");

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

            if (ds1.Tables.Contains("MATM1"))
            {
                ds1.Tables["MATM1"].Dispose();
                ds1.Tables.Remove("MATM1");
                MATM1.TableName = "MATM1";
                ds1.Tables.Add(MATM1);
            }
            else
            {
                MATM1.TableName = "MATM1";
                ds1.Tables.Add(MATM1);
            }


            if (ds1.Tables.Contains("PFMEDU1"))
            {
                ds1.Tables["PFMEDU1"].Dispose();
                ds1.Tables.Remove("PFMEDU1");
                PFMEDU1.TableName = "PFMEDU1";
                ds1.Tables.Add(PFMEDU1);
            }
            else
            {
                PFMEDU1.TableName = "PFMEDU1";
                ds1.Tables.Add(PFMEDU1);
            }

            if (ds1.Tables.Contains("PFMEDU2"))
            {
                ds1.Tables["PFMEDU2"].Dispose();
                ds1.Tables.Remove("PFMEDU2");
                PFMEDU2.TableName = "PFMEDU2";
                ds1.Tables.Add(PFMEDU2);
            }
            else
            {
                PFMEDU2.TableName = "PFMEDU2";
                ds1.Tables.Add(PFMEDU2);
            }

            if (ds1.Tables.Contains("RECR1"))
            {
                ds1.Tables["RECR1"].Dispose();
                ds1.Tables.Remove("RECR1");
                RECR1.TableName = "RECR1";
                ds1.Tables.Add(RECR1);
            }
            else
            {
                RECR1.TableName = "RECR1";
                ds1.Tables.Add(RECR1);
            }

            if (ds1.Tables.Contains("WPR1"))
            {
                ds1.Tables["WPR1"].Dispose();
                ds1.Tables.Remove("WPR1");
                WPR1.TableName = "WPR1";
                ds1.Tables.Add(WPR1);
            }
            else
            {
                WPR1.TableName = "WPR1";
                ds1.Tables.Add(WPR1);
            }


            if (ds1.Tables.Contains("PFMWE1"))
            {
                ds1.Tables["PFMWE1"].Dispose();
                ds1.Tables.Remove("PFMWE1");
                PFMWE1.TableName = "PFMWE1";
                ds1.Tables.Add(PFMWE1);
            }
            else
            {
                PFMWE1.TableName = "PFMWE1";
                ds1.Tables.Add(PFMWE1);
            }

            if (ds1.Tables.Contains("PFMWE2"))
            {
                ds1.Tables["PFMWE2"].Dispose();
                ds1.Tables.Remove("PFMWE2");
                PFMWE2.TableName = "PFMWE2";
                ds1.Tables.Add(PFMWE2);
            }
            else
            {
                PFMWE2.TableName = "PFMWE2";
                ds1.Tables.Add(PFMWE2);
            }


            if (ds1.Tables.Contains("FAMR1"))
            {
                ds1.Tables["FAMR1"].Dispose();
                ds1.Tables.Remove("FAMR1");
                FAMR1.TableName = "FAMR1";
                ds1.Tables.Add(FAMR1);
            }
            else
            {
                FAMR1.TableName = "FAMR1";
                ds1.Tables.Add(FAMR1);
            }

            if (ds1.Tables.Contains("PLR1"))
            {
                ds1.Tables["PLR1"].Dispose();
                ds1.Tables.Remove("PLR1");
                PLR1.TableName = "PLR1";
                ds1.Tables.Add(PLR1);
            }
            else
            {
                PLR1.TableName = "PLR1";
                ds1.Tables.Add(PLR1);
            }

            if (ds1.Tables.Contains("HCNR1"))
            {
                ds1.Tables["HCNR1"].Dispose();
                ds1.Tables.Remove("HCNR1");
                HCNR1.TableName = "HCNR1";
                ds1.Tables.Add(HCNR1);
            }
            else
            {
                HCNR1.TableName = "HCNR1";
                ds1.Tables.Add(HCNR1);
            }

            if (ds1.Tables.Contains("CIDC1"))
            {
                ds1.Tables["CIDC1"].Dispose();
                ds1.Tables.Remove("CIDC1");
                CIDC1.TableName = "CIDC1";
                ds1.Tables.Add(CIDC1);
            }
            else
            {
                CIDC1.TableName = "CIDC1";
                ds1.Tables.Add(CIDC1);
            }

            //if (ds1.Tables.Contains("PFPR1"))
            //{
            //    ds1.Tables["PFPR1"].Dispose();
            //    ds1.Tables.Remove("PFPR1");
            //    PFPR1.TableName = "PFPR1";
            //    ds1.Tables.Add(PFPR1);
            //}
            //else
            //{
            //    PFPR1.TableName = "PFPR1";
            //    ds1.Tables.Add(PFPR1);
            //}


            return ds1;

        }

        private DataSet LAds1()
        {
            DataSet ds1 = new DataSet("LAds1");

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];


            string str = "Select " +
                            "A.empname, " +
                            "A.nric, " +
                            "A.position, " +
                            "A.sitename, " +
                            "A.datejoined, " +
                            "A.bankacc, " +
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
                            "A.FamilyContact " +
                        "from " +
                        "( " +
                            "Select  " +
                                "h.empnum, " +
                                "h.empname, " +
                                "h.nric, " +
                                "h.matnum, " +
                                "M.matname as position, " +
                                "h.sitenum, " +
                                "S.sitename, " +
                                "h.datejoined, " +
                                "h.bankacc, " +
                                "h.commencedate, " +
                                "h.contact,  " +
                                "h.homecontactno, " +
                                "h.daysperweek, " +
                                "h.MonToFriTimeFrom, " +
                                "h.MonToFriTimeTo, " +
                                "h.MonToFriLunchHr, " +
                                "h.MonToFriTeaBrkHr, " +
                                "h.SatTimeFrom, " +
                                "h.SatTimeTo, " +
                                "h.SatLunchHr, " +
                                "h.SatTeaBrkHr, " +
                                "h.SunTimeFrom, " +
                                "h.SunTimeTo, " +
                                "h.SunLunchHr, " +
                                "h.SunTeaBrkHr, " +
                                "h.RegularOffDay, " +
                                "h.PHTimeFrom, " +
                                "h.PHTimeTo, " +
                                "h.PHLunchHr, " +
                                "h.PHTeaBrkHr, " +
                                "CASE WHEN h.paytypecode LIKE 'M%' THEN 'Monthly' WHEN h.paytypecode LIKE 'W%' THEN 'Weekly' WHEN h.paytypecode LIKE 'D%' THEN 'Daily' end as paytypecode, " +
                                "ISNULL(h.cadd1,'')+' '+ISNULL(h.cadd2,'')+' '+ISNULL(h.cadd3,'') as homeaddress, " +
                                "0 as SalaryDeductionPerDay,  " + // TO GET THE VALUE
                                "0 as OtherDudction," + // TO GET THE VALUE
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='BASIC' and PFMSR.empnum=h.empnum) as [Basic], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ATTNALLW' and PFMSR.empnum=h.empnum) as [ATTNALLW], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ACCOMALLW' and PFMSR.empnum=h.empnum) as [ACCOMALLW], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='DRIVTRANSALLW' and PFMSR.empnum=h.empnum) as [DRIVTRANSALLW], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='OTALLW' and PFMSR.empnum=h.empnum) as [OTALLW], " +
                                "0 as OtherAllowance, " + // TO GET THE VALUE
                                "h.remark, " +
                                "FM.name as NextOfKeen, " +
                                "FM.contact as FamilyContact " +
                            "from hemph h " +
                            "LEFT JOIN " +
                            "( " +
                            "Select top 1 empnum,name,contact from FAMR where [status]<>'V' " +
                            ")FM " +
                            "ON h.empnum=FM.empnum " +
                            "LEFT JOIN MATM M  " +
                            "ON h.matnum=M.matnum " +
                            "LEFT JOIN SITM S " +
                            "ON h.sitenum=S.sitenum " +
                        ")A where A.empnum='" + hemph["empnum"].ToString().Trim() + "'";

            ds1 = this.dbaccess.RemoteStandardSQL.GetSQLResult(str);




            ds1.Tables[0].TableName = "LetterAppointment";


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


            //DataTable dtGetSalH = this.dbaccess.DataSet.Tables["dtGetSalH"];

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




            return ds1;

        }

        private DataSet Recods1()
        {
            DataSet ds1 = new DataSet("LAds1");

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];


            string str = "Select " +
                            "A.empname, " +
                            "A.nric, " +
                            "A.position, " +
                            "A.sitename, " +
                            "A.datejoined, " +
                            "A.bankacc, " +
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
                            "A.FamilyContact " +
                        "from " +
                        "( " +
                            "Select  " +
                                "h.empnum, " +
                                "h.empname, " +
                                "h.nric, " +
                                "h.matnum, " +
                                "M.matname as position, " +
                                "h.sitenum, " +
                                "S.sitename, " +
                                "h.datejoined, " +
                                "h.bankacc, " +
                                "h.commencedate, " +
                                "h.contact,  " +
                                "h.homecontactno, " +
                                "h.daysperweek, " +
                                "h.MonToFriTimeFrom, " +
                                "h.MonToFriTimeTo, " +
                                "h.MonToFriLunchHr, " +
                                "h.MonToFriTeaBrkHr, " +
                                "h.SatTimeFrom, " +
                                "h.SatTimeTo, " +
                                "h.SatLunchHr, " +
                                "h.SatTeaBrkHr, " +
                                "h.SunTimeFrom, " +
                                "h.SunTimeTo, " +
                                "h.SunLunchHr, " +
                                "h.SunTeaBrkHr, " +
                                "h.RegularOffDay, " +
                                "h.PHTimeFrom, " +
                                "h.PHTimeTo, " +
                                "h.PHLunchHr, " +
                                "h.PHTeaBrkHr, " +
                                "CASE WHEN h.paytypecode LIKE 'M%' THEN 'Monthly' WHEN h.paytypecode LIKE 'W%' THEN 'Weekly' WHEN h.paytypecode LIKE 'D%' THEN 'Daily' end as paytypecode, " +
                                "ISNULL(h.cadd1,'')+' '+ISNULL(h.cadd2,'')+' '+ISNULL(h.cadd3,'') as homeaddress, " +
                                "0 as SalaryDeductionPerDay,  " + // TO GET THE VALUE
                                "0 as OtherDudction," + // TO GET THE VALUE
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='BASIC' and PFMSR.empnum=h.empnum) as [Basic], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ATTNALLW' and PFMSR.empnum=h.empnum) as [ATTNALLW], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='ACCOMALLW' and PFMSR.empnum=h.empnum) as [ACCOMALLW], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='DRIVTRANSALLW' and PFMSR.empnum=h.empnum) as [DRIVTRANSALLW], " +
                                "(Select top 1 rateamt from PFMSR where PFMSR.hsamcode='OTALLW' and PFMSR.empnum=h.empnum) as [OTALLW], " +
                                "0 as OtherAllowance, " + // TO GET THE VALUE
                                "h.remark, " +
                                "FM.name as NextOfKeen, " +
                                "FM.contact as FamilyContact " +
                            "from hemph h " +
                            "LEFT JOIN " +
                            "( " +
                            "Select top 1 empnum,name,contact from FAMR where [status]<>'V' " +
                            ")FM " +
                            "ON h.empnum=FM.empnum " +
                            "LEFT JOIN MATM M  " +
                            "ON h.matnum=M.matnum " +
                            "LEFT JOIN SITM S " +
                            "ON h.sitenum=S.sitenum " +
                        ")A where A.empnum='" + hemph["empnum"].ToString().Trim() + "'";

            ds1 = this.dbaccess.RemoteStandardSQL.GetSQLResult(str);




            ds1.Tables[0].TableName = "LetterAppointment";


            DataTable HEMPH1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select * from HEMPH where empnum='" + hemph["empnum"].ToString() + "'");

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

        private DataSet GiRods1()
        {
            DataSet ds1 = new DataSet("GiRoDS1");

            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];

            string str = "Select h.empname,h.nric, m.matname, h.giroReason from hemph h left join matm m on h.matnum=m.matnum where h.empnum='" + hemph["empnum"].ToString() + "' ";

            ds1 = this.dbaccess.RemoteStandardSQL.GetSQLResult(str);

            ds1.Tables[0].TableName = "GiroOptOut";

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


        #endregion

        #region Male & Female Radio Buttons CheckedChanged Events

        private void rad_male_CheckedChanged(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            if (rad_male.Checked)
            {
                hemph["Gender"] = "M";
            }
        }

        private void rad_female_CheckedChanged(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            if (rad_female.Checked)
            {
                hemph["Gender"] = "F";
            }
        }
        #endregion

        #region is Retired checkbox

        private void hemph_isRetired_CheckStateChanged(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            if (hemph_isRetired.Checked == true)
            {
                hemph["isretired"] = 1;
            }
            else
            {
                hemph["isretired"] = 0;
            }

            if ((bool)hemph["isretired"])
            {
                DateTime dtTmp = Convert.ToDateTime(hemph["datejoined"]).AddYears(1);

                hemph["contractend"] = TimeTools.GetStandardSafeDateOnly(dtTmp.AddDays(-1));
            }
            else
            {
                hemph["contractend"] = System.DBNull.Value;
            }



        }

        #endregion

        #region isNSmandatory Checkbox CheckStateChanged Event

        private void hemph_isNSmandatory_CheckStateChanged(object sender, EventArgs e)
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            if (hemph_isNSmandatory.Checked == true)
            {

                hemph["isNSmandatory"] = true;

                hemph_force.Enabled = true;
                hemph_rank.Enabled = true;
                hemph_vocation.Enabled = true;
                hemph_yearserved.Enabled = true;

            }
            if (hemph_isNSmandatory.Checked == false)
            {
                hemph_force.Enabled = false;
                hemph_rank.Enabled = false;
                hemph_vocation.Enabled = false;
                hemph_yearserved.Enabled = false;
                hemph["isNSmandatory"] = false;

                hemph["force"] = "";
                hemph["rank"] = "";
                hemph["vocation"] = "";
                hemph["yearserved"] = System.DBNull.Value;
            }
        }

        #endregion

        #region Browse Botton (Header) Click Event

        protected void BtnBrowse_Click(object sender, System.EventArgs e)
        {
            Form frm = BizXmlReader.CurrentInstance.GetForm(headerFormName) as Form;
            try
            {
                DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
                //TextBox tb_image = (TextBox)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "hemph_empphoto");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
                openFileDialog.Filter = "JPEG(*.JPG;*.JPEG;*.JPE;*.JFIF)|*.jpg;*.jpeg;*.jpe;*.jfif|BMP Files|*.bmp|GIF Files|*.gif|TIFF(*.TIF;*.TIFF)|*.tif;*.tiff|PNG Files|*.png|All Picture Files|*.bmp;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png|All Files|*.*";
                openFileDialog.ShowDialog(frm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void btnBrowsePermit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Compress form = new Compress(dbaccess, "PFPLREC");
                form.ShowDialog();
                form.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void BtnDownloadPermit_Click(object sender, System.EventArgs e)
        {

            try
            {
                Decommpress form = new Decommpress(dbaccess, "PFPLREC");
                form.ShowDialog();
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataTable pfpr = this.dbaccess.DataSet.Tables["pfpr"];
            try
            {
                if (tabName == "pfpr")
                {
                    pb1.SizeMode = PictureBoxSizeMode.StretchImage;  /** Makes the image the size of the picturebox **/
                    pb1.ImageLocation = (sender as OpenFileDialog).FileName;

                    pfpr.Rows[pfprRowNum]["photourl"] = (sender as OpenFileDialog).FileName;
                    pfpr.Rows[pfprRowNum]["trandate"] = (DateTime)System.DateTime.Now.ToLocalTime();

                }
                else
                {
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;  /** Makes the image the size of the picturebox **/
                    pb.ImageLocation = (sender as OpenFileDialog).FileName;
                    txt_photourl.Text = (sender as OpenFileDialog).FileName;
                }

                Environment.CurrentDirectory = Application.StartupPath;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //}

            pb.Refresh();
        }


        #endregion

        #region Browse Botton (Education) Click Event

        protected void BtnBrowseEDU_Click(object sender, System.EventArgs e)
        {
            try
            {
                Compress form = new Compress(dbaccess, "PFDREC");
                form.ShowDialog();
                form.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region Download Botton (Education) Click Event

        protected void BtnDownloadEdu_Click(object sender, System.EventArgs e)
        {
            try
            {
                Decommpress form = new Decommpress(dbaccess, "PFDREC");
                form.ShowDialog();
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region Download Botton (Permit) Click Event

        protected void btnDownloadPermit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Decommpress form = new Decommpress(dbaccess, "PFPLREC");
                form.ShowDialog();
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region TabControl OnSelectionChanged Event

        protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
        {
            base.Document_TabControl_OnSelectionChanged(sender, e);

            btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
            btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;

            switch ((sender as LXFClassLibrary.Controls.TabControl).SelectedIndex)
            {
                case 0:
                    {
                        tabName = "header";

                    }
                    break;
                case 1:
                    {
                        tabName = "contactinfos";
                    }
                    break;
                case 2:
                    {
                        tabName = "experience";
                    }
                    break;
                case 4:
                    {
                        tabName = "education";
                    }
                    break;
                case 5:
                    {
                        tabName = "employmentrec";
                    }
                    break;
                case 6:
                    {
                        tabName = "salaryrec";
                        btnUp.Enabled = true;
                        btnDown.Enabled = true;
                    }
                    break;
                case 7:
                    {
                        tabName = "incentive";
                    }
                    break;
                case 8:
                    {
                        tabName = "family";
                    }
                    break;
                case 9:
                    {
                        tabName = "permitslicences";
                    }
                    break;
                case 10:
                    {
                        tabName = "photos";
                    }
                    break;
            }
        }

        #endregion

        #region Get Image Employee Photo

        private void ShowImage()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            if (hemph["photo"].ToString().Length > 0)
            {
                Byte[] byteBLOBData = (Byte[])hemph["photo"];

                if (byteBLOBData != null && byteBLOBData.Length > 0)
                {
                    MemoryStream stmBLOBData = new MemoryStream(byteBLOBData);
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;
                    pb.Image = Image.FromStream(stmBLOBData);
                }
            }
        }

        #endregion

        #region Get Incentive Data Record

        private void GetRecr()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["hemph"].Rows[0];
            string sql1 = "select r.empnum, h.nric,h.empname,r.nricrecomm from RECR r left join HEMPH h on r.nric = h.nric where r.nricrecomm='" + hemph["nric"] + "'";
            this.dbaccess.ReadSQL("TmpR", sql1);
            DataTable TmpR = this.dbaccess.DataSet.Tables["TmpR"];
            if (TmpR.Rows.Count > 0)
            {
                DataRow drTmpR = TmpR.Rows[0];
                hemph["recrempnum"] = drTmpR["nric"];
                recrempname.Text = drTmpR["empname"].ToString();
            }

        }

        #endregion

        #region Get Resignation Date

        private void GetResignationDate()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];
            string sql1 = "select lastday from ERRH where empnum='" + hemph["empnum"].ToString() + "' and status<>'V'";
            if (!BizFunctions.IsEmpty(hemph["empnum"]))
            {
                this.dbaccess.ReadSQL("dtERRH", sql1);
                DataTable dtERRH = this.dbaccess.DataSet.Tables["dtERRH"];
                if (dtERRH.Rows.Count > 0)
                {
                    DataRow drERRH = this.dbaccess.DataSet.Tables["dtERRH"].Rows[0];
                    txt_dateresigned.Text = Convert.ToDateTime(drERRH["lastday"]).ToShortDateString();
                    hemph["dateresigned"] = Convert.ToDateTime(drERRH["lastday"]).ToShortDateString();
                }
            }
        }

        #endregion

        #region Get Confirmation Date

        private bool GetConfirmationDate()
        {
            bool hasConfimredRecord = false;
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];
            string sql1 = "select confirmationdate from cfrh where empnum ='" + hemph["empnum"].ToString() + "' and status<>'V'";
            if (!BizFunctions.IsEmpty(hemph["empnum"]))
            {
                this.dbaccess.ReadSQL("dtCFRH", sql1);
                DataTable dtCFRH = this.dbaccess.DataSet.Tables["dtCFRH"];
                if (dtCFRH.Rows.Count > 0)
                {
                    DataRow drCFRH = this.dbaccess.DataSet.Tables["dtCFRH"].Rows[0];

                    if (drCFRH["confirmationdate"] == System.DBNull.Value)
                    {
                        //drCFRH["confirmationdate"] = string.Empty;

                        txt_dateconfirmed.Text = "No Confirmed Date Yet";
                        hasConfimredRecord = true;
                    }
                    else
                    {
                        txt_dateconfirmed.Text = Convert.ToDateTime(drCFRH["confirmationdate"]).ToShortDateString();
                        hasConfimredRecord = true;
                    }

                }
            }
            return hasConfimredRecord;
        }

        #endregion

        #region Get Total Salary

        private void GetTotalSalary()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];
            decimal totalsal = 0;
            if (pfmsr.Rows.Count > 0)
            {
                foreach (DataRow dr1 in pfmsr.Rows)
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

            txt_TotalSal.Text = Convert.ToString(totalsal);
        }

        #endregion

        #region Get Salary History

        private void GetSalaryHistory()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];
            DataTable pfmsr2 = this.dbaccess.DataSet.Tables["PFMSR2"];
            bool resigned = false;
            if (!BizFunctions.IsEmpty(hemph["nric"]))
            {
                if (txt_dateresigned.Text != string.Empty)
                {
                    resigned = true;
                }
                else
                {
                    resigned = false;
                }
                if (!resigned)
                {

                    //Jason: 04092014 to combine Employment History and Salary History.  Requested by Gan of ATL
                    //string GetSalHistoryStr = "select matnum,SUM(rateamt) as TotalSalary,dateadjusted as DateAdjusted,ISNULL(refnum,'') as Refnum,ISNULL(SADJmcode,'') as AdjustedCode,sitenum from SALH " +
                    //                            "where nric='" + hemph["nric"] + "' and status<>'V' " +
                    //                            "group by matnum,nric,dateadjusted,refnum,SADJmcode,sitenum " +
                    //                            "order by dateadjusted desc";

                    string GetSalHistoryStr = "SELECT * FROM " +
                           "( " +
                               "select  " +
                                   "matnum, " +
                                   "SUM(rateamt) as TotalSalary, " +
                                   "dateadjusted as DateAdjusted, " +
                                   "ISNULL(refnum,'') as Refnum, " +
                                   "ISNULL(SADJmcode,'') as AdjustedCode, " +
                                   "sitenum  " +
                               "from SALH where empnum='" + hemph["empnum"].ToString() + "' and status<>'V' group by matnum,nric,dateadjusted,refnum,SADJmcode,sitenum  " +
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
                               "from HEMPH H LEFT JOIN ERRH E ON H.empnum=E.empnum WHERE H.empnum='" + hemph["empnum"].ToString() + "' and E.[status]<>'V' " +
                               ")A " +
                           ")B " +
                           "order by b.DateAdjusted desc";


                    this.dbaccess.ReadSQL("dtGetSalH", GetSalHistoryStr);

                    DataTable dtGetSalH = this.dbaccess.DataSet.Tables["dtGetSalH"];

                    if (dtGetSalH.Rows.Count > 0)
                    {
                        //SalHistorydatagrid.Dispose();

                        //SalHistorydatagrid.DataSource = this.dbaccess.DataSet.Tables["dtGetSalH"];

                        BizFunctions.DeleteAllRows(pfmsr2);

                        foreach (DataRow dr1 in dtGetSalH.Rows)
                        {
                            DataRow InsertPfmsr2 = pfmsr2.NewRow();
                            InsertPfmsr2["matnum"] = dr1["matnum"];
                            InsertPfmsr2["TotalSalary"] = dr1["TotalSalary"];
                            InsertPfmsr2["DateAdjusted"] = dr1["DateAdjusted"];
                            InsertPfmsr2["Refnum"] = dr1["Refnum"];
                            InsertPfmsr2["AdjustedCode"] = dr1["AdjustedCode"];
                            InsertPfmsr2["sitenum"] = dr1["sitenum"];

                            pfmsr2.Rows.Add(InsertPfmsr2);
                        }

                    }


                }
                else
                {
                    //Jason: 04092014 to combine Employment History and Salary History.  Requested by Gan of ATL
                    //string GetSalHistoryStr = "select matnum,SUM(rateamt) as TotalSalary,dateadjusted as DateAdjusted,ISNULL(refnum,'') as Refnum,ISNULL(SADJmcode,'') as AdjustedCode,sitenum from SALH " +
                    //                            "where nric='" + hemph["nric"] + "' and status<>'V' " +
                    //                            "group by matnum,nric,dateadjusted,refnum,SADJmcode,sitenum " +
                    //                            "order by dateadjusted desc";

                    string GetSalHistoryStr = "SELECT * FROM " +
                         "( " +
                             "select  " +
                                 "matnum, " +
                                 "SUM(rateamt) as TotalSalary, " +
                                 "dateadjusted as DateAdjusted, " +
                                 "ISNULL(refnum,'') as Refnum, " +
                                 "ISNULL(SADJmcode,'') as AdjustedCode, " +
                                 "sitenum  " +
                             "from SALH where empnum='" + hemph["empnum"].ToString() + "' and status<>'V' group by matnum,nric,dateadjusted,refnum,SADJmcode,sitenum  " +
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
                             "from HEMPH H LEFT JOIN ERRH E ON H.empnum=E.empnum WHERE H.empnum='" + hemph["empnum"].ToString() + "' and E.[status]<>'V' " +
                             ")A " +
                         ")B " +
                         "order by b.DateAdjusted desc";

                    this.dbaccess.ReadSQL("dtGetSalH", GetSalHistoryStr);

                    DataTable dtGetSalH = this.dbaccess.DataSet.Tables["dtGetSalH"];

                    if (dtGetSalH.Rows.Count > 0)
                    {
                        //SalHistorydatagrid.Dispose();

                        //SalHistorydatagrid.DataSource = this.dbaccess.DataSet.Tables["dtGetSalH"];

                        BizFunctions.DeleteAllRows(pfmsr2);

                        foreach (DataRow dr1 in dtGetSalH.Rows)
                        {
                            DataRow InsertPfmsr2 = pfmsr2.NewRow();
                            InsertPfmsr2["matnum"] = dr1["matnum"];
                            InsertPfmsr2["TotalSalary"] = dr1["TotalSalary"];
                            InsertPfmsr2["DateAdjusted"] = dr1["DateAdjusted"];
                            InsertPfmsr2["Refnum"] = dr1["Refnum"];
                            InsertPfmsr2["AdjustedCode"] = dr1["AdjustedCode"];
                            InsertPfmsr2["sitenum"] = dr1["sitenum"];

                            pfmsr2.Rows.Add(InsertPfmsr2);
                        }

                    }

                }
            }
            //SalHistorydatagrid.CaptionText = "Salary History";
        }

        #endregion

        #region Get Employment History

        private void GetEmploymentHistory()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];
            if (!BizFunctions.IsEmpty(hemph["nric"]))
            {
                string GetEmpHistoryStr = "select H.empnum as EmpNo, H.matnum AS Appointment,H.datejoined AS Joined,E.lastday as Resigned,E.refnum as Refnum,E.EmpWillConvert as Is_Transfered " +
                                          "from HEMPH H LEFT JOIN ERRH E " +
                                          "ON H.empnum=E.empnum " +
                                          "WHERE H.nric='" + hemph["nric"] + "' and E.status<>'V' " +
                                          "ORDER BY H.datejoined";

                this.dbaccess.ReadSQL("dtGetEmpH", GetEmpHistoryStr);
                DataTable dtGetEmpH = this.dbaccess.DataSet.Tables["dtGetEmpH"];
                if (dtGetEmpH.Rows.Count > 0)
                {
                    EmpHistorydatagrid.DataSource = this.dbaccess.DataSet.Tables["dtGetEmpH"];


                }
            }
            EmpHistorydatagrid.CaptionText = "Employment History";

        }

        #endregion


        #region Get Leave History

        private void GetLeavetHistory()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];

            if (!BizFunctions.IsEmpty(hemph["empnum"]))
            {

                string str1 = "SELECT Lv.refnum as Refnum, Lv.LeaveYear as [Year], Lv.leavefrom as [From] ,Lv.leaveto as [To],Lm.lvmnum as [Desc],Lv.totaldays as Days,Lv.apprdby as ApprovedBy " +
                                "FROM LVR Lv left join LVM Lm on Lv.lvmnum=Lm.lvmnum " +
                                "where empnum='" + hemph["empnum"].ToString() + "' and lv.[status]<>'V' AND isapproved=1 AND Lv.totaldays>0" +
                                "order by lv.leavefrom";

                this.dbaccess.ReadSQL("dtLeaveHistory", str1);
                if (this.dbaccess.DataSet.Tables["dtLeaveHistory"].Rows.Count > 0)
                {
                    //overallleavesummary.DataSource = this.dbaccess.DataSet.Tables["dtLeaveHistory"];

                    dg_leaverec.DataSource = this.dbaccess.DataSet.Tables["dtLeaveHistory"];

                }

                //this.dbaccess.DataSet.Tables["dtLeaveHistory"].Dispose();
            }


            dg_leaverec.CaptionText = "Leave History";

        }

        #endregion


        #region Get Training History

        private void GetTrainingtHistory()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];
            if (!BizFunctions.IsEmpty(hemph["nric"]))
            {
                // Jason: Changed due to that the user wants to record any course taken when applying for work.
                //string GetEmpHistoryStr = "select t1.refnum,th.coursecode,th.coursename as Course, t1.isFullTime as FullTime,t1.ispass as Passed,t1.isRejected as Rejected,t1.funding as Funding "+
                //                           "from trn1 t1 left join trnh th on t1.refnum=th.refnum "+
                //                            "where t1.empnum='" + hemph["empnum"].ToString() + "' and th.[status]<>'V' " +
                //                            "order by th.trandate";

                string GetEmpHistoryStr = "Select * from " +
                                            "( " +
                                            "select  " +
                                                    "t1.refnum, " +
                                                    "th.coursecode, " +
                                                    "th.coursename as Course,  " +
                                                    "th.coursetype as FullTime, " +
                                                    "CASE WHEN t1.staffstatus='P' THEN 'PASSED' WHEN t1.staffstatus='F' THEN 'FAILED' ELSE 'PENDING' END as Passed, " +
                                                    "t1.isRejected as Rejected, " +
                                                    "t1.funding as Funding  " +
                                            "from trn1 t1 left join trnh th on t1.refnum=th.refnum where t1.empnum='" + hemph["empnum"].ToString() + "' and th.[status]<>'V'  " +
                                            ")A " +
                                            "UNION " +
                                            "Select * from " +
                                            "( " +
                                            "Select " +
                                                    "'' as refnum, " +
                                                    "coursecode, " +
                                                    "coursedesc as coursename, " +
                                                    "'' as FullTime, " +
                                                    "'' as Passed, " +
                                                    "0 as Rejected, " +
                                                    "0 as Funding " +
                                            "from PFMEDU where empnum='" + hemph["empnum"].ToString() + "' and coursecode is not null " +
                                            ")A";

                this.dbaccess.ReadSQL("dtGetEmpTrn", GetEmpHistoryStr);
                DataTable dtGetEmpTrn = this.dbaccess.DataSet.Tables["dtGetEmpTrn"];
                if (dtGetEmpTrn.Rows.Count > 0)
                {
                    DataTable dt = this.dbaccess.DataSet.Tables["dtGetEmpTrn"];

                    dg_request.DataSource = dt;

                    // Get the width of Longest Field
                    int newwidth1 = LongestField(this.dbaccess.DataSet, "dtGetEmpTrn", "Refnum");
                    int newwidth2 = LongestField(this.dbaccess.DataSet, "dtGetEmpTrn", "coursecode");
                    int newwidth3 = LongestField(this.dbaccess.DataSet, "dtGetEmpTrn", "Course");
                    int newwidth4 = LongestField(this.dbaccess.DataSet, "dtGetEmpTrn", "FullTime");
                    int newwidth5 = LongestField(this.dbaccess.DataSet, "dtGetEmpTrn", "Passed");
                    int newwidth6 = LongestField(this.dbaccess.DataSet, "dtGetEmpTrn", "Rejected");
                    int newwidth7 = LongestField(this.dbaccess.DataSet, "dtGetEmpTrn", "Funding");




                    // Create new Table Style
                    DataGridTableStyle ts1 = new DataGridTableStyle();
                    DataGridTableStyle ts2 = new DataGridTableStyle();
                    DataGridTableStyle ts3 = new DataGridTableStyle();
                    DataGridTableStyle ts4 = new DataGridTableStyle();
                    DataGridTableStyle ts5 = new DataGridTableStyle();
                    DataGridTableStyle ts6 = new DataGridTableStyle();
                    DataGridTableStyle ts7 = new DataGridTableStyle();


                    ts1.MappingName = "dtGetEmpTrn";
                    ts2.MappingName = "dtGetEmpTrn";
                    ts3.MappingName = "dtGetEmpTrn";
                    ts4.MappingName = "dtGetEmpTrn";
                    ts5.MappingName = "dtGetEmpTrn";
                    ts6.MappingName = "dtGetEmpTrn";
                    ts7.MappingName = "dtGetEmpTrn";


                    this.dg_request.TableStyles.Clear();
                    this.dg_request.TableStyles.Add(ts1);
                    //this.dg_request.TableStyles.Add(ts2);
                    //this.dg_request.TableStyles.Add(ts3);
                    //this.dg_request.TableStyles.Add(ts4);
                    //this.dg_request.TableStyles.Add(ts5);
                    //this.dg_request.TableStyles.Add(ts6);
                    //this.dg_request.TableStyles.Add(ts7);

                    // Assign New Width to DataGrid column

                    if (!BizFunctions.IsEmpty(dt.Rows[0]["refnum"]))
                    {
                        this.dg_request.TableStyles["dtGetEmpTrn"].GridColumnStyles["refnum"].Width = newwidth1;
                    }

                    if (!BizFunctions.IsEmpty(dt.Rows[0]["coursecode"]))
                    {
                        this.dg_request.TableStyles["dtGetEmpTrn"].GridColumnStyles["coursecode"].Width = newwidth2;
                    }

                    if (!BizFunctions.IsEmpty(dt.Rows[0]["Course"]))
                    {
                        this.dg_request.TableStyles["dtGetEmpTrn"].GridColumnStyles["Course"].Width = newwidth3;
                    }

                    if (!BizFunctions.IsEmpty(dt.Rows[0]["FullTime"]))
                    {
                        this.dg_request.TableStyles["dtGetEmpTrn"].GridColumnStyles["FullTime"].Width = newwidth4;
                    }

                    if (!BizFunctions.IsEmpty(dt.Rows[0]["Passed"]))
                    {
                        this.dg_request.TableStyles["dtGetEmpTrn"].GridColumnStyles["Passed"].Width = newwidth5;
                    }

                    if (!BizFunctions.IsEmpty(dt.Rows[0]["Rejected"]))
                    {
                        this.dg_request.TableStyles["dtGetEmpTrn"].GridColumnStyles["Rejected"].Width = newwidth6;
                    }

                    if (!BizFunctions.IsEmpty(dt.Rows[0]["Funding"]))
                    {
                        this.dg_request.TableStyles["dtGetEmpTrn"].GridColumnStyles["Funding"].Width = newwidth7;
                    }











                }
            }
            dg_request.CaptionText = "Training History";

        }


        private int LongestField(DataSet ds, string TableName, string ColumnName)
        {
            int maxlength = 0;
            int tot = ds.Tables[TableName].Rows.Count;
            string straux = "";
            int intaux = 0;

            Graphics g = dg_request.CreateGraphics();

            // Take width one balnk space to add to the new width to the Column   
            int offset = Convert.ToInt32(Math.Ceiling(g.MeasureString(" ", dg_request.Font).Width));

            for (int i = 0; i < tot; ++i)
            {
                straux = ds.Tables[TableName].Rows[i][ColumnName].ToString();

                // Get the width of Current Field String according to the Font
                intaux = Convert.ToInt32(Math.Ceiling(g.MeasureString(straux, dg_request.Font).Width));
                if (intaux > maxlength)
                {
                    maxlength = intaux;
                }
            }// End of For Loop

            return maxlength + offset;
        }


        #endregion

        #region Check Salary History

        private bool CheckSALH()
        {
            DataRow hemph = this.dbaccess.DataSet.Tables["HEMPH"].Rows[0];
            DataTable pfmsr = this.dbaccess.DataSet.Tables["PFMSR"];
            DataTable xPFMSR = this.dbaccess.DataSet.Tables["xPFMSR"];

            bool equal = true;


            if (xPFMSR.Rows.Count > 0)
            {
                string GetPFMR, GetXPFMR = "";
                GetPFMR = "SELECT hsgcode,hsamcode,salcode,rateamt,dateadjusted,adjustmentflag FROM PFMSR";
                GetXPFMR = "SELECT hsgcode,hsamcode,salcode,rateamt,dateadjusted,adjustmentflag FROM xPFMSR";

                //GetPFMR = "SELECT sum(rateamt) as totalrateamt FROM PFMSR";
                //GetXPFMR = "SELECT sum(rateamt) as totalrateamt FROM xPFMSR";

                DataTable dtTmpPFMR = BizFunctions.ExecuteQuery(dbaccess.DataSet, GetPFMR);
                DataTable dtTmpXPFMR = BizFunctions.ExecuteQuery(dbaccess.DataSet, GetXPFMR);

                //this.dbaccess.ReadSQL("tmpSalh","Select * from salh w"

                DataTable dt1, dt2 = null;
                dt1 = dtTmpPFMR.Copy();
                dt1.TableName = "dt1";
                dt2 = dtTmpXPFMR.Copy();
                dt2.TableName = "dt2";


                DataTable dt;
                dt = BizLogicTools.Tools.getDifferentRecords(dt1, dt2);

                //decimal totalrateamt1 = 0;
                //decimal  totalrateamt2 = 0;
                //if (!BizFunctions.IsEmpty(dt1.Rows[0]["totalrateamt"]))
                //{
                //    totalrateamt1 = Convert.ToDecimal(dt1.Rows[0]["totalrateamt"]);
                //}

                //if(!BizFunctions.IsEmpty(dt1.Rows[0]["totalrateamt"]))
                //{
                //    totalrateamt2 = Convert.ToDecimal(dt1.Rows[0]["totalrateamt"]);
                //}

                //if (totalrateamt1 == totalrateamt2)
                //{
                //    equal = true;
                //}
                //else
                //{
                //    equal = false;
                //}


                foreach (DataRow drTmpPFMR in dtTmpPFMR.Rows)
                {
                    if (drTmpPFMR["adjustmentflag"].ToString() == (string)hemph["flag"])
                    {
                        equal = true;
                    }
                    else
                    {
                        equal = false;
                        break;
                    }

                }

                // Once saved.. can only be changed from sajh
                if (!equal)
                {
                    string checkSalhTable = "Select * from salh where empnum='" + hemph["empnum"].ToString() + "' and [status]<>'V'";

                    this.dbaccess.ReadSQL("tmpSALH1", checkSalhTable);

                    DataTable tmpSALH1 = this.dbaccess.DataSet.Tables["tmpSALH1"];

                    if (tmpSALH1.Rows.Count > 0)
                    {

                        BizFunctions.DeleteAllRows(pfmsr);

                        foreach (DataRow dr1 in xPFMSR.Rows)
                        {
                            if (dr1.RowState != DataRowState.Deleted)
                            {
                                DataRow InsertPFMSR = pfmsr.NewRow();
                                InsertPFMSR["nric"] = dr1["nric"];
                                InsertPFMSR["refnum"] = dr1["refnum"];
                                InsertPFMSR["empnum"] = dr1["empnum"];
                                InsertPFMSR["saldesc"] = dr1["saldesc"];
                                InsertPFMSR["rateamt"] = dr1["rateamt"];
                                InsertPFMSR["dateadjusted"] = dr1["dateadjusted"];
                                InsertPFMSR["remarks"] = dr1["remarks"];
                                InsertPFMSR["user"] = dr1["user"];
                                InsertPFMSR["flag"] = dr1["flag"];
                                InsertPFMSR["status"] = dr1["status"];
                                InsertPFMSR["created"] = dr1["created"];
                                InsertPFMSR["modified"] = dr1["modified"];
                                InsertPFMSR["createdby"] = dr1["createdby"];
                                InsertPFMSR["hsgcode"] = dr1["hsgcode"];
                                InsertPFMSR["hsamcode"] = dr1["hsamcode"];
                                InsertPFMSR["salcode"] = dr1["salcode"];
                                InsertPFMSR["adjustmentflag"] = dr1["adjustmentflag"];
                                InsertPFMSR["mark"] = dr1["mark"];
                                pfmsr.Rows.Add(InsertPFMSR);
                            }
                        }

                    }
                    else
                    {
                        equal = true;
                    }

                    tmpSALH1.Dispose();
                }
                else
                {
                    string deleteFromSalhTable = "Delete from salh where empnum='" + hemph["empnum"].ToString() + "' and flag='HEMP'";

                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(deleteFromSalhTable);
                }

                dt.Dispose();

                dtTmpPFMR.Dispose();
                dtTmpXPFMR.Dispose();
            }

            return equal;

        }

        #endregion

        #region Set Salary Status

        private void SetSalaryStatus()
        {
            lbl_SalaryStatus.Text = "LOCKED";
            lbl_SalaryStatus.ForeColor = Color.Red;

        }

        #endregion

    }
}


