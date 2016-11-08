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
using BizRAD.BizBase;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;

using ASOMS.BizLogicTools;

using ASOMS.Schedule;
using ASOMS.TimeUtilites;
using ASOMS.BizModules.RichTextEdit2;

#endregion

namespace ASOMS.SITMT
{
    public class Voucher_SITMT : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName, sitmt1FormName, sitmt2FormName, sitmt3FormName, sitmt4FormName, sitmt8FormName, sitmt9FormName, sitmt10FormName, 
                         txt_guid,varBinary = null;

        protected Label lbl_sitmt1Total, lbl_sitmt2Total, lbl_sitmt3Total = null;
        protected TextBox sitmt1Total_monday, sitmt1Total_tuesday, sitmt1Total_wednesday, sitmt1Total_thursday, sitmt1Total_friday,
                          sitmt1Total_saturday, sitmt1Total_sunday, sitmt2Total_monday, sitmt2Total_tuesday, sitmt2Total_wednesday, 
                          sitmt2Total_thursday,sitmt2Total_friday, sitmt2Total_saturday, sitmt2Total_sunday, sitmt3Total_monday, 
                          sitmt3Total_tuesday, sitmt3Total_wednesday, sitmt3Total_thursday, sitmt3Total_friday,sitmt3Total_saturday,
                          sitmt3Total_sunday, sitmt1Ctrh_monday, sitmt1Ctrh_tuesday, sitmt1Ctrh_wednesday, sitmt1Ctrh_thursday,
                          sitmt1Ctrh_friday, sitmt1Ctrh_saturday, sitmt1Ctrh_sunday, sitmt2Ctrh_monday, sitmt2Ctrh_tuesday, 
                          sitmt2Ctrh_wednesday, sitmt2Ctrh_thursday,sitmt2Ctrh_friday, sitmt2Ctrh_saturday, sitmt2Ctrh_sunday,
                          sitmt3Ctrh_monday, sitmt3Ctrh_tuesday, sitmt3Ctrh_wednesday, sitmt3Ctrh_thursday,sitmt3Ctrh_friday,
                          sitmt3Ctrh_saturday, sitmt3Ctrh_sunday, sitmt8_ttlamt, sitmt9_ttlamt, txt_projectsite, txt_address,
                          txt_designation, txt_issueby, txt_deployment, txt_locdesc = null;

        DateTimePicker dtp_issuedate = null;

        Button BtnSummary, sitmt10_btnAdd, BtnDesc, sitmt10_btnUpdate, sitmt10_btnNew = null;
        bool headerFlag, columnChanged, manpowerFlag, periodicFlag, isMouseClicked;
        Schedule.ScheduleControl sc = null;
        DataTable AllowedDAys = null;
        DataGrid dgSitmt10 = null;
        Byte[] array = null;

        WordForm2 wf = null;
  

        #endregion
        
        #region Constructor

        public Voucher_SITMT(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_SITMT.xml", moduleName, voucherBaseHelpers)
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

            DataRow sitmt = e.DBAccess.DataSet.Tables["sitmt"].Rows[0];
            //DataTable sitmt1 = e.DBAccess.DataSet.Tables["sitmt1"];
            //DataTable sitmt2 = e.DBAccess.DataSet.Tables["sitmt2"];
            //DataTable sitmt3 = e.DBAccess.DataSet.Tables["sitmt3"];
            //DataTable sitmt4 = e.DBAccess.DataSet.Tables["sitmt4"];
            //DataTable sitmt5 = e.DBAccess.DataSet.Tables["sitmt5"];
            //DataTable sitmt6 = e.DBAccess.DataSet.Tables["sitmt6"];
            //DataTable hemph = e.DBAccess.DataSet.Tables["hemph"];
            string scheduleoption = sitmt["scheduleoption"].ToString();


            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            //this.sitmt1FormName = (e.FormsCollection["dayshift"] as Form).Name;
            //this.sitmt2FormName = (e.FormsCollection["nightshift"] as Form).Name;
            //this.sitmt3FormName = (e.FormsCollection["concierge"] as Form).Name;
            //this.sitmt4FormName = (e.FormsCollection["relief"] as Form).Name;

            this.sitmt8FormName = (e.FormsCollection["manpower"] as Form).Name;
            this.sitmt9FormName = (e.FormsCollection["scope"] as Form).Name;
            this.sitmt10FormName = (e.FormsCollection["archive"] as Form).Name;
       
         

            #region Schedule Radio Buttons

            #endregion

            //e.DBAccess.DataSet.Tables["sitmt1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM1_ColumnChanged);
            //e.DBAccess.DataSet.Tables["sitmt2"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM2_ColumnChanged);
            //e.DBAccess.DataSet.Tables["sitmt3"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM3_ColumnChanged);
            //e.DBAccess.DataSet.Tables["sitmt4"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM4_ColumnChanged);
            e.DBAccess.DataSet.Tables["sitmt"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITMT_ColumnChanged);
            e.DBAccess.DataSet.Tables["sitmt8"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM8_ColumnChanged);
            e.DBAccess.DataSet.Tables["sitmt9"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_SITM9_ColumnChanged);




            Initialise();

            
                //GetSitmt1FooterTotals();                
                        
                //GetSitmt2FooterTotals();
                      
                //GetSitmt3FooterTotals();
            

            //if (!BizFunctions.IsEmpty(sitmt["docunum"]))
            //{
            //    GetSitmt1CtrhFooterTotals();
            //    GetSitmt2CtrhFooterTotals();
            //    GetSitmt3CtrhFooterTotals();
            //}


           
        }

        #endregion

        #region Initialise


        private void Initialise()
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];


            txt_projectsite = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_projectsite") as TextBox;
            txt_projectsite.BackColor = Color.Yellow;
            txt_issueby = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_issueby") as TextBox;
            txt_issueby.BackColor = Color.Yellow;

            txt_projectsite.KeyDown += new KeyEventHandler(txt_projectsite_KeyDown);
            txt_projectsite.DoubleClick += new EventHandler(txt_projectsite_DoubleClick);

            txt_issueby.KeyDown += new KeyEventHandler(txt_issueby_KeyDown);
            txt_issueby.DoubleClick += new EventHandler(txt_issueby_DoubleClick);


            dgSitmt10 = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "dg_archive") as DataGrid;
            dgSitmt10.MouseDoubleClick += new MouseEventHandler(dgSitmt10_MouseDoubleClick);

            sitmt10_btnNew = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "sitmt10_btnNew") as Button; 
            sitmt10_btnAdd = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "sitmt10_btnAdd") as Button; 
            sitmt10_btnUpdate = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "sitmt10_btnUpdate") as Button;
            BtnDesc = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "BtnDesc") as Button;

            BtnDesc.Click += new EventHandler(BtnDesc_Click);
            sitmt10_btnAdd.Click += new EventHandler(sitmt10_btnAdd_Click);
            sitmt10_btnUpdate.Click += new EventHandler(sitmt10_btnUpdate_Click);
            sitmt10_btnNew.Click += new EventHandler(sitmt10_btnNew_Click);



            //sitmt1Total_monday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_monday") as TextBox;
            //sitmt1Total_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_tuesday") as TextBox;
            //sitmt1Total_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_wednesday") as TextBox;
            //sitmt1Total_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_thursday") as TextBox;
            //sitmt1Total_friday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_friday") as TextBox;
            //sitmt1Total_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_saturday") as TextBox;
            //sitmt1Total_sunday  = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Total_sunday") as TextBox;

            //sitmt2Total_monday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_monday") as TextBox;
            //sitmt2Total_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_tuesday") as TextBox;
            //sitmt2Total_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_wednesday") as TextBox;
            //sitmt2Total_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_thursday") as TextBox;
            //sitmt2Total_friday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_friday") as TextBox;
            //sitmt2Total_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_saturday") as TextBox;
            //sitmt2Total_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Total_sunday") as TextBox;

            //sitmt3Total_monday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_monday") as TextBox;
            //sitmt3Total_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_tuesday") as TextBox;
            //sitmt3Total_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_wednesday") as TextBox;
            //sitmt3Total_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_thursday") as TextBox;
            //sitmt3Total_friday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_friday") as TextBox;
            //sitmt3Total_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_saturday") as TextBox;
            //sitmt3Total_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Total_sunday") as TextBox;

            //sitmt1Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_monday") as TextBox;
            //sitmt1Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_tuesday") as TextBox;
            //sitmt1Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_wednesday") as TextBox;
            //sitmt1Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_thursday") as TextBox;
            //sitmt1Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_friday") as TextBox;
            //sitmt1Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_saturday") as TextBox;
            //sitmt1Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt1FormName, "sitmt1Ctrh_sunday") as TextBox;

            //sitmt2Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_monday") as TextBox;
            //sitmt2Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_tuesday") as TextBox;
            //sitmt2Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_wednesday") as TextBox;
            //sitmt2Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_thursday") as TextBox;
            //sitmt2Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_friday") as TextBox;
            //sitmt2Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_saturday") as TextBox;
            //sitmt2Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt2FormName, "sitmt2Ctrh_sunday") as TextBox;

            //sitmt3Ctrh_monday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_monday") as TextBox;
            //sitmt3Ctrh_tuesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_tuesday") as TextBox;
            //sitmt3Ctrh_wednesday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_wednesday") as TextBox;
            //sitmt3Ctrh_thursday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_thursday") as TextBox;
            //sitmt3Ctrh_friday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_friday") as TextBox;
            //sitmt3Ctrh_saturday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_saturday") as TextBox;
            //sitmt3Ctrh_sunday = BizXmlReader.CurrentInstance.GetControl(sitmt3FormName, "sitmt3Ctrh_sunday") as TextBox;

            sitmt8_ttlamt = BizXmlReader.CurrentInstance.GetControl(sitmt8FormName, "sitmt8_ttlamt") as TextBox;
            sitmt9_ttlamt = BizXmlReader.CurrentInstance.GetControl(sitmt9FormName, "sitmt9_ttlamt") as TextBox;

            varBinary = "0x504B0304140002000800976C904258470FF02F020000B20400000B000000636F6E74656E742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB767B5A2D5BFA99D2CBCBE651D35E9739B5AE978FAAAC299A47CB6C91378FDAE9A36A952FCD4B8FBCC68FB823F9A0CDDFB5B77D196DFD77CFABDBBEF9AE29B7CF2B427CB1CADA6212A2D05C5EDC160E351D0032ABB3ABDB42415BA2B0FFFABBB258BEFDECA379DBAE1EDDBD7B757535BEBA37AEEA8BBBBB0F1F3EBCCBDF9AA65575651B02B8CC0E37DEDBD9D9BF7B55176D5E5BFA6693DBCF0E37F6D112D8B77D5DF9C4A7CAD4A2BA5AD72523399BDECDCB1CAF357777C7BB773FB20C764E7CB57D9E4DF3ED593E2D9BF4AEFD265BB7D582883EDD66066A8E1E0B23F1BFA9FC0ECC3EFBE835F52D7F9F678BA2BCFEECA3269FB6C4DE1FD977E4EFED554D43A8DB22E78EEE7A0087A1BFDCED425F6575765167ABB9F9823E808CF01FDBF2D68BAA5E64A569B0C81A9A9FED557661BE7FDD66CB5956CF3E62440687AC5F4CAAD9B5FD032271F4980543C795CA1F5EF720097FA87F4ABB9D8F8E1EF3A7ABFE1B344C03749545203ECDCFB375D9A62FCDE0D36734751F1D9D2E56EDF5E3BBF6CD23FD7D657E511CDD28057FF3970CCDFCD5D53747FF0F504B0304140002000800976C9042D5F05C2BBA020000F80500000A0000007374796C65732E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB76BB69AFCBBC49E9DD65F388FFA0C6F5F251953545F368992DF2E6513B7D54ADF2A579E791D7F811F7231F9C57B77DF55D536E9F57DBD36AB1CADA621282795716CBB79F7D346FDBD5A3BB77AFAEAEC657F7C6557D7177F7E1C38777F95BD3B4B9BCB86D97D474A03FA1C86DE128FDF0BE25E67945843CCFA6F9F62C9F964D7AD77E23D43D7A2CB4E27F53F91D7D7CF6D1D3FC3C5B976DFA32ABB38B3A5BCDD36704EB236D739E2D8AF2FAB38FDAFC1D7D767718CCF36299A72FD68B495EC7DF95CF56596DA77C7B33021B3AFBF6F52AAF6516225D99F7F0C7F6AA260AD66D410C765E3D9A5665557FF6D193729D9B57B9D57A396380B960F6D9474D5516B3812657C5AC9D7FF651B66E2BC6F2AE87E630CE2FAA7A91955D845766D00C091D3D421F4B2624890D31CCF2BCB858D7C434D532E506F2E536DA359F7D749E950D0D86BFA1296FF2F6B38F76C6F7A70BFD8C5A13A353DFF4F9AEFB0C10565553B42CAA657EDEEA77C5725AE7E0360263F8135FDC823B6D5BE16D90A6C384FA274847081553957D43B55576916F97D975B56E03DA7DB15AECD869F51A756697BFD1D93918DF2F961FD94FE7797131070576F1A9022202B7DB555DD06033A1C3AAAADB3A2B5A7E7191D517C572BBAD56A0E883834F0F14A27E31A95A1A46FC3B10141A70F760E7D3F09B5A10A1297AB8730F5F390EF246E648D7A7957EB1C89A962631A4A07E084801055FB7D97296D596A57D227A34F6E7AC03DF7CDC51DB47FF0F504B0304140002000800976C90425CC0E1810501000000010000150000004D4554412D494E462F6D616E69666573742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E8F1225B16E779D33E32BFA4F4DAB2B17F52D37AF9A8CA9AA279B4CC1679F3A89D3EAA56F972564DD78B7CD93E0ADB3F42471ED8F3A2CCB7A9597D9DBAAEF259916DB7D7ABFCB38FB2D5AA2CA6594B48DEBD5CCEC6DCD3D8EF60DCE6EFDA8FDCDBE7EBB2DC5E65EDFCB38FEE7E94DE3D7A7CB73786A3FF07504B01021400140002000800976C904258470FF02F020000B20400000B0000000000000000000000000000000000636F6E74656E742E786D6C504B01021400140002000800976C9042D5F05C2BBA020000F80500000A00000000000000000000000000580200007374796C65732E786D6C504B01021400140002000800976C90425CC0E181050100000001000015000000000000000000000000003A0500004D4554412D494E462F6D616E69666573742E786D6C504B05060000000003000300B4000000720600000000";

            GetManPowerAmt();
            GetPeriodScheduleCost();
            

            

            //if (!BizFunctions.IsEmpty(sitmt["sitenumt"]))
            //{
            //    if (sitmt["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSO || sitmt["status"].ToString().Trim() == (string)Common.DEFAULT_DOCUMENT_STATUSP)
            //    {
            //        if (!BizFunctions.IsEmpty(sitmt["docunum"]))
            //        {
            //            sc = new ScheduleControl(sitmt["docunum"].ToString().Trim(), sitmt["docunum"].ToString().Trim().Substring(0, 3), "SITMT");
            //        }
            //    }
            //}

        }

        void txt_issueby_DoubleClick(object sender, EventArgs e)
        {
            txt_issueby = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_issueby") as TextBox;
            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empname", "empname like '" + txt_issueby.Text + "%' ", null, F2Type.Sort);

            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                txt_issueby.Text = f2BaseHelper.F2Base.CurrentRow["empname"].ToString();

            }
        }

        void txt_issueby_KeyDown(object sender, KeyEventArgs e)
        {
            txt_issueby = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_issueby") as TextBox;
            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empname", "empname like '" + txt_issueby.Text + "%' ", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    txt_issueby.Text = f2BaseHelper.F2Base.CurrentRow["empname"].ToString();

                }

            }
        }

        void txt_projectsite_DoubleClick(object sender, EventArgs e)
        {
            txt_projectsite = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_projectsite") as TextBox;
            txt_address = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_address") as TextBox;

            F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITMI.xml", e, "sitenumi", "sitenumi like '" + txt_projectsite.Text + "%' ", null, F2Type.Sort);

            f2BaseHelper.F2_Load();

            if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
            {
                txt_projectsite.Text = f2BaseHelper.F2Base.CurrentRow["sitenumi"].ToString();
                txt_address.Text = f2BaseHelper.F2Base.CurrentRow["sitename"].ToString();
            }

        }

        void txt_projectsite_KeyDown(object sender, KeyEventArgs e)
        {
            txt_projectsite = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_projectsite") as TextBox;
            txt_address = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_address") as TextBox;

            if (e.KeyCode == Keys.F2)
            {

                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_SITMI.xml", e, "sitenumi", "sitenumi like '" + txt_projectsite.Text + "%' ", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    txt_projectsite.Text = f2BaseHelper.F2Base.CurrentRow["sitenumi"].ToString();
                    txt_address.Text = f2BaseHelper.F2Base.CurrentRow["sitename"].ToString();

                }

            }
        }

        void sitmt10_btnNew_Click(object sender, EventArgs e)
        {
            txt_projectsite = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_projectsite") as TextBox;
            txt_address = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_address") as TextBox;
            txt_designation = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_designation") as TextBox;
            txt_issueby = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_issueby") as TextBox;
            txt_deployment = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_deployment") as TextBox;
            txt_locdesc = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_locdesc") as TextBox;
            dtp_issuedate = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "dtp_issuedate") as DateTimePicker;

            txt_projectsite.Text = string.Empty;
            txt_address.Text = string.Empty;
            txt_designation.Text = string.Empty;
            txt_issueby.Text = string.Empty;
            txt_deployment.Text = string.Empty;
            txt_locdesc.Text = string.Empty;
            dtp_issuedate.Text = string.Empty;

            txt_guid = string.Empty;
            array = null;
        }

        void BtnDesc_Click(object sender, EventArgs e)
        {
            wf = new WordForm2(array);
            wf.Show();
            wf.Focus();
                        
        }

        void sitmt10_btnUpdate_Click(object sender, EventArgs e)
        {
            DataTable sitmt10 = this.dbaccess.DataSet.Tables["sitmt10"];

            txt_projectsite = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_projectsite") as TextBox;
            txt_address = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_address") as TextBox;
            txt_designation = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_designation") as TextBox;
            txt_issueby = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_issueby") as TextBox;
            txt_deployment = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_deployment") as TextBox;
            txt_locdesc = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_locdesc") as TextBox;
            dtp_issuedate = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "dtp_issuedate") as DateTimePicker;

            if (sitmt10.Rows.Count > 0)
            {
                foreach (DataRow dr10 in sitmt10.Rows)
                {
                    if (dr10.RowState != DataRowState.Deleted)
                    {
                        if (dr10["guid"].ToString() == txt_guid)
                        {
                            
                            dr10["sitenumi"] = txt_projectsite.Text;
                            dr10["locaddr"] = txt_address.Text;
                            dr10["designation"] = txt_designation.Text;
                            dr10["issuedby"] = txt_issueby.Text;
                            dr10["deployment"] = txt_deployment.Text;
                            dr10["locdesc"] = txt_locdesc.Text;
                            dr10["issueddate"] = Convert.ToDateTime(dtp_issuedate.Text);

                            array = wf.array;
                            if (array != null && array.Length > 0)
                            {
                                dr10["pddesc"] = array;
                            }
                            else
                            {
                                char[] charArray = varBinary.ToCharArray();
                                byte[] byteArray = new byte[charArray.Length];

                                dr10["pddesc"] = byteArray;
                            }
                        }
                    }
                }
            }
        }

        void sitmt10_btnAdd_Click(object sender, EventArgs e)
        {
            DataTable sitmt10 = this.dbaccess.DataSet.Tables["sitmt10"];
            DataRow Insertsitmt10 = sitmt10.NewRow();

            txt_projectsite = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_projectsite") as TextBox;
            txt_address = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_address") as TextBox;
            txt_designation = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_designation") as TextBox;
            txt_issueby = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_issueby") as TextBox;
            txt_deployment = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_deployment") as TextBox;
            txt_locdesc = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_locdesc") as TextBox;
            dtp_issuedate = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "dtp_issuedate") as DateTimePicker;

            if (txt_projectsite.Text != string.Empty || txt_address.Text != string.Empty || txt_designation.Text != string.Empty || txt_issueby.Text != string.Empty || txt_deployment.Text != string.Empty || txt_locdesc.Text != string.Empty || dtp_issuedate.Text != string.Empty)
            {
                
                Insertsitmt10["sitenumi"] = txt_projectsite.Text;
                Insertsitmt10["locaddr"] = txt_address.Text;
                Insertsitmt10["designation"] = txt_designation.Text;
                Insertsitmt10["issuedby"] = txt_issueby.Text;
                Insertsitmt10["deployment"] = txt_deployment.Text;
                Insertsitmt10["locdesc"] = txt_locdesc.Text;
                Insertsitmt10["issueddate"] = Convert.ToDateTime(dtp_issuedate.Text);
                Insertsitmt10["guid"] = Tools.getGUID();

                if (wf != null)
                {
                    array = wf.array;
                }
                if (array != null && array.Length > 0)
                {
                    Insertsitmt10["pddesc"] = array;
                }
                else
                {
                    char[] charArray = varBinary.ToCharArray();
                    byte[] byteArray = new byte[charArray.Length];

                    Insertsitmt10["pddesc"] = byteArray;
                }

                sitmt10.Rows.Add(Insertsitmt10);
                txt_guid = string.Empty;
           
            }
        }

        void dgSitmt10_MouseDoubleClick(object sender, MouseEventArgs e)
        {

          
            try
            {

                txt_projectsite = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_projectsite") as TextBox;
                txt_address = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_address") as TextBox;
                txt_designation = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_designation") as TextBox;
                txt_issueby = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_issueby") as TextBox;
                txt_deployment = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_deployment") as TextBox;
                txt_locdesc = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "txt_locdesc") as TextBox;
                dtp_issuedate = BizXmlReader.CurrentInstance.GetControl(sitmt10FormName, "dtp_issuedate") as DateTimePicker;

                txt_projectsite.Text = string.Empty;
                txt_address.Text = string.Empty;
                txt_designation.Text = string.Empty;
                txt_issueby.Text = string.Empty;
                txt_deployment.Text = string.Empty;
                dtp_issuedate.Text = string.Empty;

                isMouseClicked = true;
                #region Get and Set Row

                DataGrid dataGrid = sender as DataGrid;

                int i = dataGrid.CurrentRowIndex;
                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

                if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
                {// if user double click Row Header or Cell, the selected row will be added to CRQ2.
                    dataGrid.Select(i);

                    DataRow drCur = getcurrentrow(dgSitmt10);

                    string Getsitmt10Row = "Select * from sitmt10 where [guid]='" + drCur["guid"].ToString() + "'";

                    DataTable tempsitmt10 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Getsitmt10Row);

                    if (tempsitmt10 != null)
                    {
                        if (tempsitmt10.Rows.Count > 0)
                        {
                            DataRow dr1 = tempsitmt10.Rows[0];

                            txt_projectsite.Text = dr1["sitenumi"].ToString();

                            txt_designation.Text = dr1["designation"].ToString();
                            txt_address.Text = dr1["locaddr"].ToString();
                            txt_issueby.Text = dr1["issuedby"].ToString();
                            txt_deployment.Text = dr1["deployment"].ToString();
                            txt_locdesc.Text = dr1["locdesc"].ToString();
                            dtp_issuedate.Text = Convert.ToDateTime(dr1["issueddate"]).ToShortDateString();
                            txt_guid = drCur["guid"].ToString();
                            array = (byte[])dr1["pddesc"];
                    
                        }
                    }

                }

                #endregion
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        private DataRow getcurrentrow(DataGrid datagrid)
        {
            CurrencyManager cm = (CurrencyManager)datagrid.BindingContext[datagrid.DataSource, datagrid.DataMember];
            DataRowView drv = cm.Current as DataRowView;
            DataRow dr = drv.Row;

            return dr;
        }


        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataTable SITMT11 = e.DBAccess.DataSet.Tables["SITMT11"];

            Refresh_Header();


            if (manpowerFlag)
            {
                Refresh_Manpower();
            }           
            if (periodicFlag)
            {
                Refresh_Schedule();
            }

           

        }
        #endregion

        #region Other TextBox Click and KeyDown Events

        //protected void sitmt1_dayshiftcode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F2)
        //    {
        //        DataTable sitmt1 = this.dbaccess.DataSet.Tables["sitmt1"];
        //        sitmt1_dayshiftcode.Text = BizLogicTools.Tools.GetF2KeyDown(sender, e, null, null, "SHM", "shiftcode");
        //        foreach (DataRow dr1 in sitmt1.Rows)
        //        {
        //            if (dr1.RowState != DataRowState.Deleted)
        //            {
        //                dr1["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
        //            }
        //        }
        //    }
        //}
             
        //protected void sitmt1_dayshiftcode_Click(object sender, System.EventArgs e)
        //{
        //    DataTable sitmt1 = this.dbaccess.DataSet.Tables["sitmt1"];
        //    sitmt1_dayshiftcode.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "SHM", "shiftcode");
        //     foreach (DataRow dr1 in sitmt1.Rows)
        //    {
        //        if (dr1.RowState != DataRowState.Deleted)
        //        {
        //            dr1["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
        //        }
        //    }
        //}

        //protected void sitmt2_nightshiftcode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F2)
        //    {
        //        DataTable sitmt2 = this.dbaccess.DataSet.Tables["sitmt2"];
        //        sitmt2_nightshiftcode.Text = BizLogicTools.Tools.GetF2KeyDown(sender, e, null, null, "SHM", "shiftcode");
        //        foreach (DataRow dr1 in sitmt2.Rows)
        //        {
        //            if (dr1.RowState != DataRowState.Deleted)
        //            {
        //                dr1["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
        //            }
        //        }
        //    }

        //}

        //protected void sitmt2_nightshiftcode_Click(object sender, System.EventArgs e)
        //{
        //    DataTable sitmt2 = this.dbaccess.DataSet.Tables["sitmt2"];
        //    sitmt2_nightshiftcode.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "SHM", "shiftcode");
        //     foreach (DataRow dr1 in sitmt2.Rows)
        //    {
        //        if (dr1.RowState != DataRowState.Deleted)
        //        {
        //            dr1["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
        //        }
        //    }

        //}

        //protected void sitmt3_concierge_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.F2)
        //    {
        //        DataTable sitmt3 = this.dbaccess.DataSet.Tables["sitmt3"];
        //        sitmt3_concierge.Text = BizLogicTools.Tools.GetF2KeyDown(sender, e,null,null, "SHM", "shiftcode");
        //        foreach (DataRow dr1 in sitmt3.Rows)
        //        {
        //            if (dr1.RowState != DataRowState.Deleted)
        //            {
        //                dr1["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
        //            }
        //        }
        //    }


        //}

        //protected void sitmt3_concierge_Click(object sender, System.EventArgs e)
        //{
        //    DataTable sitmt3 = this.dbaccess.DataSet.Tables["sitmt3"];
        //    sitmt3_concierge.Text = BizLogicTools.Tools.GetF2Clicked(sender, e, "SHM", "shiftcode");
        //     foreach (DataRow dr1 in sitmt3.Rows)
        //    {
        //        if (dr1.RowState != DataRowState.Deleted)
        //        {
        //            dr1["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
        //        } 
        //    }
        //}

        #endregion

        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow sitmt = e.DBAccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt11 = e.DBAccess.DataSet.Tables["SITMT11"];
            DataTable hemph = e.DBAccess.DataSet.Tables["hemph"];
           

        
         
        }

        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

            DataRow sitmt = e.DBAccess.DataSet.Tables["sitmt"].Rows[0];
            //DataTable sitmt1sum = e.DBAccess.DataSet.Tables["sitmt1sum"];
            //DataTable sitmt2 = e.DBAccess.DataSet.Tables["sitmt2"];
            //DataTable sitmt2sum = e.DBAccess.DataSet.Tables["sitmt2sum"];
            //DataTable sitmt3 = e.DBAccess.DataSet.Tables["sitmt3"];
            //DataTable sitmt3sum = e.DBAccess.DataSet.Tables["sitmt3sum"];
            //DataTable sitmt4 = e.DBAccess.DataSet.Tables["sitmt4"];
            DataTable sitmt8 = e.DBAccess.DataSet.Tables["sitmt8"];
            DataTable sitmt9 = e.DBAccess.DataSet.Tables["sitmt9"];
            DataTable sitmt10 = e.DBAccess.DataSet.Tables["sitmt10"];
            DataTable sitmt11 = e.DBAccess.DataSet.Tables["sitmt11"];


            foreach (DataRow dr8 in sitmt8.Rows)
            {
                if (dr8.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr8, "sitenumt/user/flag/status/created/modified");        
                }

            }

            foreach (DataRow dr9 in sitmt9.Rows)
            {                
                if (dr9.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr9, "sitenumt/user/flag/status/created/modified");
                    
                }
            }


            foreach (DataRow dr10 in sitmt10.Rows)
            {
                if (dr10.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr10, "sitenumt/user/flag/status/created/modified");   
                }
            }

            foreach (DataRow dr11 in sitmt11.Rows)
            {
                if (dr11.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(sitmt, dr11, "sitenumt/user/flag/status/created/modified");
                }
            }
            


        
            
            //#region FAMR

            //foreach (DataRow dr1 in sitmt1.Rows)
            //{
            //    if (dr1.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr1, "sitenumt/user/flag/status/created/modified");
        
            //    }

            //}
            //foreach (DataRow dr1s in sitmt1sum.Rows)
            //{
                
            //    if (dr1s.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr1s, "sitenumt/user/flag/status/created/modified");
                    
            //    }

            //}

            //#endregion

            //#region HCNR
            //foreach (DataRow dr2 in sitmt2.Rows)
            //{
            //    if (dr2.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr2, "sitenumt/user/flag/status/created/modified");
    

            //    }

            //}

            //foreach (DataRow dr2s in sitmt2sum.Rows)
            //{
            //    if (dr2s.RowState != DataRowState.Deleted)
            //    {

            //        BizFunctions.UpdateDataRow(sitmt, dr2s, "sitenumt/user/flag/status/created/modified");

            //    }

            //}

            //#endregion

            //#region PFMEDU
            //foreach (DataRow dr3 in sitmt3.Rows)
            //{
            //    if (dr3.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr3, "sitenumt/user/flag/status/created/modified");

            //    }

            //}

            //foreach (DataRow dr3s in sitmt3sum.Rows)
            //{
            //    if (dr3s.RowState != DataRowState.Deleted)
            //    {

            //        BizFunctions.UpdateDataRow(sitmt, dr3s, "sitenumt/user/flag/status/created/modified");

            //    }

            //}

            //#endregion

            //#region PFMER
            //foreach (DataRow dr4 in sitmt4.Rows)
            //{
            //    if (dr4.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr4, "sitenumt/user/flag/status/created/modified");
            //    }
            //}

            //foreach (DataRow dr4s in sitmt4sum.Rows)
            //{
            //    if (dr4s.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr4s, "sitenumt/user/flag/status/created/modified");

            //    }
            //}

            //#endregion

            //#region PFMER
            //foreach (DataRow dr5 in sitmt5.Rows)
            //{
            //    if (dr5.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr5, "sitenumt/user/flag/status/created/modified");
            //    }
            //}
            //#endregion

            //#region PFMER
            //foreach (DataRow dr6 in sitmt6.Rows)
            //{
            //    if (dr6.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(sitmt, dr6, "sitenumt/user/flag/status/created/modified");
            //    }

            //}
            //#endregion

            //#region PFMER
            ////foreach (DataRow dr7 in sitmtall.Rows)
            ////{
            ////    if (dr7.RowState != DataRowState.Deleted)
            ////    {
            ////        BizFunctions.UpdateDataRow(sitmt, dr7, "refnum/user/flag/status/created/modified");
            ////    }

            ////}
            //#endregion


            //Summary();
        }
        #endregion

        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataTable sitmt11 = e.DBAccess.DataSet.Tables["sitmt11"];
            //DataTable sitmt2 = e.DBAccess.DataSet.Tables["sitmt2"];
            //DataTable sitmt3 = e.DBAccess.DataSet.Tables["sitmt3"];
            //DataTable sitmt4 = e.DBAccess.DataSet.Tables["sitmt4"];
            //DataTable sitmt5 = e.DBAccess.DataSet.Tables["sitmt5"];
            //DataTable sitmt6 = e.DBAccess.DataSet.Tables["sitmt6"];                    

            //#region FAMR

            //foreach (DataRow dr1 in sitmt1.Rows)
            //{
            //    if (dr1.RowState != DataRowState.Deleted)
            //    {
            //        if(!BizFunctions.IsEmpty(dr1["empnum"].ToString()))
            //        {
            //            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr1["empnum"].ToString()));
            //        }

            //    }

            //}
            //#endregion
           
            //#region HCNR
            //foreach (DataRow dr2 in sitmt2.Rows)
            //{
            //    if (dr2.RowState != DataRowState.Deleted)
            //    {
            //        if (!BizFunctions.IsEmpty(dr2["empnum"].ToString()))
            //        {
            //            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr2["empnum"].ToString()));
            //        }

            //    }

            //}

            //#endregion

            //#region PFMEDU
            //foreach (DataRow dr3 in sitmt3.Rows)
            //{
            //    if (dr3.RowState != DataRowState.Deleted)
            //    {
            //        if (!BizFunctions.IsEmpty(dr3["empnum"].ToString()))
            //        {
            //            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr3["empnum"].ToString()));
            //        }
            //    }

            //}
            //#endregion

            //#region PFMER
            //foreach (DataRow dr4 in sitmt4.Rows)
            //{
            //    if (dr4.RowState != DataRowState.Deleted)
            //    {
            //        if (!BizFunctions.IsEmpty(dr4["empnum"].ToString()))
            //        {
            //            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr4["empnum"].ToString()));
            //        }
            //    }
            //}

            //#endregion

            //#region PFMER
            //foreach (DataRow dr5 in sitmt5.Rows)
            //{
            //    if (dr5.RowState != DataRowState.Deleted)
            //    {
            //        if (!BizFunctions.IsEmpty(dr5["empnum"].ToString()))
            //        {
            //            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(UpdateHemph(dr5["empnum"].ToString()));
            //        }
            //    }
            //}
            //#endregion

         
        }

        # endregion

        #region Update Employee Table Sitenumi

        private string UpdateHemph(string empno)
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];
            string sql1 = "Update hemph set sitenumi='" + sitmt["sitenumi"].ToString() + "', sectorcode='" + sitmt["sectorcode"].ToString() + "' where empnum='" + empno + "' ";

            return sql1;
        }
        #endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);


        }

        #endregion

        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "sitmt_docunum":
                    {
                        e.Condition = "vCTRH_ADH.refnum not in(Select docunum as refnum from sitmt where status<>'V')";
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
                    
                case "empnum":
                    if (e.CurrentRow.Table.TableName == "sitmt1")
                    {                                               
                        e.Condition = BizFunctions.F2Condition("empnum/empname", (sender as TextBox).Text);                       
                    }
                    if (e.CurrentRow.Table.TableName == "sitmt2")
                    {
                        e.Condition = BizFunctions.F2Condition("empnum/empname", (sender as TextBox).Text);                       
                    }
                    if (e.CurrentRow.Table.TableName == "sitmt3")
                    {
                        e.Condition = BizFunctions.F2Condition("empnum/empname", (sender as TextBox).Text);                      
                    }
                    break;
                
                    
               
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow sitmt = dbaccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt1 = dbaccess.DataSet.Tables["sitmt1"];
            DataTable sitmt2 = dbaccess.DataSet.Tables["sitmt2"];
            DataTable sitmt3 = dbaccess.DataSet.Tables["sitmt3"];
            DataTable sitmt9 = dbaccess.DataSet.Tables["sitmt9"];

            switch (e.ControlName)
            {
                case "sitmt_docunum":
                    e.CurrentRow["docunum"] = e.F2CurrentRow["refnum"];
                    e.CurrentRow["contracttype"] = e.F2CurrentRow["TableName"];
                    if (!BizFunctions.IsEmpty(sitmt["docunum"].ToString().Trim()) || sitmt["docunum"].ToString().Trim() != string.Empty)
                    {
                        if (e.CurrentRow["contracttype"].ToString() == "CTRH")
                        {
                            GetContract(sitmt["docunum"].ToString().Trim());
                        }
                        if (e.CurrentRow["contracttype"].ToString() == "ADH")
                        {
                            GetAdhoc(sitmt["docunum"].ToString().Trim());
                        }

                        e.CurrentRow["arnum"] = e.F2CurrentRow["arnum"];
                               
                    }
                    break;

                case "sitmt_sectorcode":
                    e.CurrentRow["sectorcode"] = e.F2CurrentRow["sectorcode"];
                    break;

          
            }
        }

        private int Countmatnum(string matnum,string shift,string Table)
        {
            int Total = 0;
            string SelectCount = "SELECT matnum,shiftcode,COUNT(*) as total from " + Table + " where matnum='" + matnum + "' and shiftcode='" + shift + "' group by matnum,shiftcode";
            DataTable matnumCount = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, SelectCount);
            if (matnumCount.Rows.Count > 0)
            {
                DataRow dr1 = matnumCount.Rows[0];
                if (BizFunctions.IsEmpty(dr1["total"]))
                {
                    dr1["total"] = 0;
                }
                else
                {
                    Total = Convert.ToInt32(dr1["total"]);
                }
            }
            return Total;
        }

        private string GetTable(char Code)
        {
            string Table = "";

            if (Code == 'D')
            {
                Table = "SITMT1";
            }
            if (Code == 'N')
            {
                Table = "SITMT2";
            }
            if (Code == 'C')
            {
                Table = "SITMT3";
            }

            return Table;
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow sitmt = dbaccess.DataSet.Tables["sitmt"].Rows[0];
            switch (e.MappingName)
            {
                case "empnum":
                    e.CurrentRow["empnum"] = e.F2CurrentRow["empnum"];
                    e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    //if (!AllowmatnumInsert(e.F2CurrentRow["matnum"].ToString(), e.TableName.ToString()))
                    //{

                    //    e.CurrentRow.Delete();
                        
                       
                    //}
                 
                    break;

                case "eqmnum":                                   
                    break;

                case "matnum":
                    {
                        //if(!AllowmatnumInsert(e.F2CurrentRow["matnum"].ToString(),e.TableName.ToString()))
                        //{
                          
                        //    e.CurrentRow.Delete();
                       
                        //}
                    }
                    break;


            
            }
        }

        #endregion

        #region tra ReOpen/void

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

        #region Get ShiftCode

        private string Getshiftcode(string tablename, string sitenumi)
        {
            string result = "";
            string sql1 = "Select distinct shiftcode from " + tablename + " where sitenumt='" + sitenumi + "'";
            this.dbaccess.ReadSQL("ResultShiftcode", sql1);
            DataTable ResultShiftcode = this.dbaccess.DataSet.Tables["ResultShiftcode"];

            if (ResultShiftcode.Rows.Count > 0)
            {
                DataRow drRS = this.dbaccess.DataSet.Tables["ResultShiftcode"].Rows[0];
                result = drRS["shiftcode"].ToString();
            }
            else
            {
                result = "";
            }
            return result;
        }

        #endregion

        #region Allow To Insert Schedule

        private bool AllowScheduleInsert(string matnum, string shiftcode, string day,string Tablename)
        {

            string GetInfo = "Select shiftcode,matnum,[day],ISNULL(sum(total),0) as Total from "+
                                "( "+
                                "SELECT shiftcode,matnum,'" + day + "' as [day], "+
	                                "CASE "+
                                        "WHEN ISNULL(" + day + ",'')='X' THEN 1 " +
                                            " WHEN " + day + "='O' OR " + day + "='' THEN 0 "+
                                        "ELSE 0 " +
		                                "END as  Total "+ 
	                                "from " + Tablename + " where shiftcode='" + shiftcode + "' and matnum='" + matnum + "' "+
                                ")A "+
                                "group by shiftcode,matnum,[day]";
            int total=0;
            bool allow, hasmatnum = false;
            DataTable tmpGetInfo = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetInfo);

            if (tmpGetInfo.Rows.Count > 0)
            {
                DataRow drTmpGetInfo = tmpGetInfo.Rows[0];

                foreach (DataRow dr1 in sc.ScheduleInfo.Rows)
                {
                    if (dr1["day"].ToString().Trim() == drTmpGetInfo["day"].ToString().Trim() && dr1["shiftcode"].ToString().Trim() == drTmpGetInfo["shiftcode"].ToString().Trim() && dr1["matnum"].ToString().Trim() == drTmpGetInfo["matnum"].ToString().Trim())
                    {
                        total = Convert.ToInt32(dr1["total"]) - Convert.ToInt32(drTmpGetInfo["total"]);
                        hasmatnum = true;

                        break;
                    }
                    else
                    {
                        hasmatnum = false;
                    }
                }

            }


            if (hasmatnum)
            {
                if (total < 0)
                {
                    allow = false;
                }
                else
                {
                    allow = true;
                }
            }
            else
            {
                allow = false;
            }

           

            return allow;

        }

        #endregion

        #region Allow matnum Insert

        private bool AllowmatnumInsert(string Tablename)
        {
            string GetmatnumInfo = "Select matnum,COUNT(*) as total from " + Tablename + " " +
                                        "group by matnum";

            int total = 0;
            bool allow,flag = false;
            DataTable tmpGetmatnumInfo = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetmatnumInfo);

            if (tmpGetmatnumInfo.Rows.Count > 0)
            {
                DataRow drTmptmpGetDgInfo = tmpGetmatnumInfo.Rows[0];

                foreach (DataRow dr1 in sc.matnumCountInfo.Rows)
                {
                    if (dr1["matnum"].ToString().Trim() == drTmptmpGetDgInfo["matnum"].ToString().Trim())
                    {
                        total = Convert.ToInt32(dr1["total"]) - Convert.ToInt32(drTmptmpGetDgInfo["total"]);
                        flag = true;
                        break;
                    }
                }

            }

            if (total < 0 || flag==false)
            {
                allow = false;
            }
            else
            {
                allow = true;
            }
            return allow;
        }

        #endregion

        #region Column Changed Events

        private void Voucher_SITMT_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable sitmt = this.dbaccess.DataSet.Tables["SITMTT"];

            switch (e.Column.ColumnName)
            {
                case "commencedate":
                    {
                        headerFlag = true;
                        break;
                    }
                case "enddate":
                    {
                        headerFlag = true;
                        break;
                    }
            }
        }

        private void Voucher_SITM1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable sitmt1 = this.dbaccess.DataSet.Tables["SITMT1"];

             switch (e.Column.ColumnName)
             {
                 case "empnum":
                     {
                         if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                         {
                             e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                             e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                         }
                                                                    
                     }
                     break;

                 case "monday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}                  
                         
                     }
                     break;

                 case "tuesday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}

                     }
                     break;

                 case "wednesday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "thursday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "friday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "saturday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;

                 case "sunday":
                     {
                         //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                         //{
                         //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                         //    {
                         //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt1_dayshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                         //        {
                         //            e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //        }
                         //        else
                         //        {
                         //            e.Row.BeginEdit();
                         //            e.Row[e.Column.ToString()] = string.Empty;
                         //            e.Row.EndEdit();
                         //            ShowMsgAllowScheduleInsert();
                         //        }
                         //    }
                         //}
                     }
                     break;
                 case "shiftcode":
                     {
                         
                     }
                     break;
                 case "matnum":
                     {
                         //if (AllowmatnumInsert(e.Column.Table.ToString()))
                         //{
                         //    e.Row["shiftcode"] = sitmt1_dayshiftcode.Text.ToString().Trim();
                         //}
                         //else
                         //{
                         //    if (e.Row[e.Column.ToString()].ToString() != string.Empty)
                         //    {
                         //        e.Row.BeginEdit();
                         //        e.Row.Delete();
                         //        e.Row.EndEdit();
                         //    }
                         //}
                     }
                     break;
                     e.Row.EndEdit();



             }
        }

        private void Voucher_SITM2_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {


            e.Row.BeginEdit();
            
            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {
                        
                         // if(AllowScheduleInsert(e.Row["matnum"].ToString(),e.Row["shiftcode"].ToString(),e.Column.ToString(),e.Column.Table.ToString()))
                         //{
                         //    e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                         //}
                         //else
                         //{
                         //    e.Row.BeginEdit();
                         //    e.Row[e.Column.ToString()] = string.Empty;
                         //    e.Row.EndEdit();
                         //    ShowMsgAllowScheduleInsert();
                         //}

                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                        }
                                              

                    }
                    break;

                case "monday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}

                    }
                    break;

                case "tuesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "wednesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "thursday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "friday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "saturday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "sunday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt2_nightshiftcode.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt2_nightshiftcode.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;
                case "shiftcode":
                    {
                        
                    }
                    break;
                    e.Row.EndEdit();

                    
            }
        }

        private void Voucher_SITM3_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
        

            e.Row.BeginEdit();

            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {

                        //if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //{
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //}
                        //else
                        //{
                        //    e.Row.BeginEdit();
                        //    e.Row[e.Column.ToString()] = string.Empty;
                        //    e.Row.EndEdit();
                        //    ShowMsgAllowScheduleInsert();
                        //}

                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                        }
                                              

                    }
                    break;

                case "monday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                        
                    }
                    break;

                case "tuesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "wednesday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "thursday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "friday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "saturday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;

                case "sunday":
                    {
                        //if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                        //{
                        //    if (!BizFunctions.IsEmpty(e.Row[e.Column.ToString()]))
                        //    {
                        //        if (AllowScheduleInsert(e.Row["matnum"].ToString(), sitmt3_concierge.Text, e.Column.ToString(), e.Column.Table.ToString()))
                        //        {
                        //e.Row["shiftcode"] = sitmt3_concierge.Text.ToString().Trim();
                        //        }
                        //        else
                        //        {
                        //            e.Row.BeginEdit();
                        //            e.Row[e.Column.ToString()] = string.Empty;
                        //            e.Row.EndEdit();
                        //            ShowMsgAllowScheduleInsert();
                        //        }
                        //    }
                        //}
                    }
                    break;
                case "shiftcode":
                    {
                        
                    }
                    break;
                    e.Row.EndEdit();
         


            }
        }

        private void Voucher_SITM4_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
    

            e.Row.BeginEdit();

            switch (e.Column.ColumnName)
            {
                case "empnum":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["empname"] = GetEmpname(e.Row["empnum"].ToString());
                            e.Row["matnum"] = GetMatnum(e.Row["empnum"].ToString());
                        }
                                   
                    }
                    break;

                case "monday":
                    {

                        
                    }
                    break;

                case "tuesday":
                    {
                        
                    }
                    break;

                case "wednesday":
                    {
                        

                    }
                    break;

                case "thursday":
                    {
                        
                    }
                    break;

                case "friday":
                    {
                        
                    }
                    break;

                case "saturday":
                    {
                        
                    }
                    break;

                case "sunday":
                    {
                        
                    }
                    break;
                case "shiftcode":
                    {
                        
                    }
                    break;
                    e.Row.EndEdit();



            }


        }


        private void Voucher_SITM8_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable sitmt8 = this.dbaccess.DataSet.Tables["SITMT8"];

            switch (e.Column.ColumnName)
            {
                case "officerqty":
                    {
                        manpowerFlag = true;
                    }
                    break;

                case "estmamt":
                    {
                        manpowerFlag = true;
                    }
                    break;

                case "actmamt":
                    {
                        manpowerFlag = true;
                    }
                    break;

                case "rate":
                    {
                        manpowerFlag = true;
                    }
                    break;
                case "subtotal":
                    {
                        manpowerFlag = true;
                    }
                    break;
            }
        }

        private void Voucher_SITM9_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable sitmt9 = this.dbaccess.DataSet.Tables["SITMT9"];

            switch (e.Column.ColumnName)
            {
                case "actmamt":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "estmamt":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth1":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth2":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth3":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth4":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth5":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth6":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth7":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth8":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth9":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth10":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth11":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "xmonth12":
                    {
                        periodicFlag = true;
                    }
                    break;
            }
        }

        #endregion

        #region Get Final Summary Count of Schedule
        
        private void Summary()
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt1 = this.dbaccess.DataSet.Tables["sitmt1"];
            DataTable sitmt1sum = this.dbaccess.DataSet.Tables["sitmt1sum"];

            DataTable sitmt2 = this.dbaccess.DataSet.Tables["sitmt2"];
            DataTable sitmt2sum = this.dbaccess.DataSet.Tables["sitmt2sum"];

            DataTable sitmt3 = this.dbaccess.DataSet.Tables["sitmt3"];
            DataTable sitmt3sum = this.dbaccess.DataSet.Tables["sitmt3sum"];

            DataTable sitmt4 = this.dbaccess.DataSet.Tables["sitmt4"];
            DataTable sitmt4sum = this.dbaccess.DataSet.Tables["sitmt4sum"];

            DataTable sitmtall = this.dbaccess.DataSet.Tables["sitmtall"];

          

           #region

           foreach (DataRow dr1 in sitmt1.Rows)
           {
               if (dr1.RowState != DataRowState.Deleted)
               {
                   if (BizFunctions.IsEmpty(dr1["monday"].ToString().Trim()))
                   {
                       dr1["monday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["tuesday"].ToString().Trim()))
                   {
                       dr1["tuesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["wednesday"].ToString().Trim()))
                   {
                       dr1["wednesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["thursday"].ToString().Trim()))
                   {
                       dr1["thursday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["friday"].ToString().Trim()))
                   {
                       dr1["friday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["saturday"].ToString().Trim()))
                   {
                       dr1["saturday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr1["sunday"].ToString().Trim()))
                   {
                       dr1["sunday"] = "O";
                   }
               }

           }

           #endregion

           #region HCNR
           foreach (DataRow dr2 in sitmt2.Rows)
           {
               if (dr2.RowState != DataRowState.Deleted)
               {
                   if (BizFunctions.IsEmpty(dr2["monday"].ToString().Trim()))
                   {
                       dr2["monday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["tuesday"].ToString().Trim()))
                   {
                       dr2["tuesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["wednesday"].ToString().Trim()))
                   {
                       dr2["wednesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["thursday"].ToString().Trim()))
                   {
                       dr2["thursday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["friday"].ToString().Trim()))
                   {
                       dr2["friday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["saturday"].ToString().Trim()))
                   {
                       dr2["saturday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr2["sunday"].ToString().Trim()))
                   {
                       dr2["sunday"] = "O";
                   }

               }

           }

           #endregion

           #region PFMEDU
           foreach (DataRow dr3 in sitmt3.Rows)
           {
               if (dr3.RowState != DataRowState.Deleted)
               {
                   if (BizFunctions.IsEmpty(dr3["monday"].ToString().Trim()))
                   {
                       dr3["monday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["tuesday"].ToString().Trim()))
                   {
                       dr3["tuesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["wednesday"].ToString().Trim()))
                   {
                       dr3["wednesday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["thursday"].ToString().Trim()))
                   {
                       dr3["thursday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["friday"].ToString().Trim()))
                   {
                       dr3["friday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["saturday"].ToString().Trim()))
                   {
                       dr3["saturday"] = "O";
                   }

                   if (BizFunctions.IsEmpty(dr3["sunday"].ToString().Trim()))
                   {
                       dr3["sunday"] = "O";
                   }
               }

           }

           #endregion


           
           if (sitmt1sum.Rows.Count > 0)
           {
               BizFunctions.DeleteAllRows(sitmt1sum);
           }
           DataTable sitmt1sumTmp = BizLogicTools.Tools.GetShiftSummary("SITMT1", this.dbaccess.DataSet);
            foreach (DataRow dr4 in sitmt1sumTmp.Select())
            {       
                if (dr4.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm1Sum = sitmt1sum.NewRow();
                    drSitm1Sum["matnum"] = dr4["matnum"];
                    drSitm1Sum["shiftcode"] = dr4["shiftcode"];
                    drSitm1Sum["monday"] = dr4["monday"];
                    drSitm1Sum["tuesday"] = dr4["tuesday"];
                    drSitm1Sum["wednesday"] = dr4["wednesday"];
                    drSitm1Sum["thursday"] = dr4["thursday"];
                    drSitm1Sum["friday"] = dr4["friday"];
                    drSitm1Sum["saturday"] = dr4["saturday"];
                    drSitm1Sum["sunday"] = dr4["sunday"]; 
                    sitmt1sum.Rows.Add(drSitm1Sum);
                }
            }

            if (sitmt2sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmt2sum);
            }
            DataTable sitmt2sumTmp = BizLogicTools.Tools.GetShiftSummary("SITMT2", this.dbaccess.DataSet);
            foreach (DataRow dr5 in sitmt2sumTmp.Select())
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm2Sum = sitmt2sum.NewRow();
                    drSitm2Sum["matnum"] = dr5["matnum"];
                    drSitm2Sum["shiftcode"] = dr5["shiftcode"];
                    drSitm2Sum["monday"] = dr5["monday"];
                    drSitm2Sum["tuesday"] = dr5["tuesday"];
                    drSitm2Sum["wednesday"] = dr5["wednesday"];
                    drSitm2Sum["thursday"] = dr5["thursday"];
                    drSitm2Sum["friday"] = dr5["friday"];
                    drSitm2Sum["saturday"] = dr5["saturday"];
                    drSitm2Sum["sunday"] = dr5["sunday"];
                    sitmt2sum.Rows.Add(drSitm2Sum);
                }
            }

            if (sitmt3sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmt3sum);
            }
            DataTable sitmt3sumTmp = BizLogicTools.Tools.GetShiftSummary("SITMT3", this.dbaccess.DataSet);
            foreach (DataRow dr6 in sitmt3sumTmp.Select())
            {
                if (dr6.RowState != DataRowState.Deleted)
                {

                    DataRow drSitm3Sum = sitmt3sum.NewRow();
                    drSitm3Sum["matnum"] = dr6["matnum"];
                    drSitm3Sum["shiftcode"] = dr6["shiftcode"];
                    drSitm3Sum["monday"] = dr6["monday"];
                    drSitm3Sum["tuesday"] = dr6["tuesday"];
                    drSitm3Sum["wednesday"] = dr6["wednesday"];
                    drSitm3Sum["thursday"] = dr6["thursday"];
                    drSitm3Sum["friday"] = dr6["friday"];
                    drSitm3Sum["saturday"] = dr6["saturday"];
                    drSitm3Sum["sunday"] = dr6["sunday"];
                    sitmt3sum.Rows.Add(drSitm3Sum);
                }
            }

            string relief = "select matnum,COUNT(*) as total from [sitmt4] group by matnum";
            DataTable sitmt4sumTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, relief);
            if (sitmt4sum.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmt4sum);
            }

            foreach (DataRow dr7 in sitmt4sumTmp.Select())
            {

                if (dr7.RowState != DataRowState.Deleted)
                {
                    DataRow drSitm4Sum = sitmt4sum.NewRow();
                    drSitm4Sum["matnum"] = dr7["matnum"];
                    drSitm4Sum["total"] = dr7["total"];           
                    sitmt4sum.Rows.Add(drSitm4Sum);
                }

            }


            string overallSum = "Select R1.shiftcode, " +
                               "sum(R1.monday) as monday, " +
                               "sum(R1.tuesday) as tuesday, " +
                               "sum(R1.wednesday) as wednesday, " +
                               "sum(R1.thursday) as thursday, " +
                               "sum(R1.friday) as friday, " +
                               "sum(R1.saturday) as saturday, " +
                               "sum(R1.sunday) as sunday " +
                               "From " +
                               "(" +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from sitmt1sum " +

                                "union all " +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from sitmt2sum " +

                                "union all " +
                                "select shiftcode, monday,tuesday, wednesday,thursday,friday,saturday,sunday from sitmt3sum " +

                                ") R1 " +
                                "Group by R1.shiftcode";

            DataTable sitmtallsumTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, overallSum);

            if (sitmtall.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(sitmtall);
            }

            foreach (DataRow dr8 in sitmtallsumTmp.Select())
            {
                if (dr8.RowState != DataRowState.Deleted)
                {

                    DataRow drSitmall = sitmtall.NewRow();
                    drSitmall["shiftcode"] = dr8["shiftcode"];
                    drSitmall["monday"] = dr8["monday"];
                    drSitmall["tuesday"] = dr8["tuesday"];
                    drSitmall["wednesday"] = dr8["wednesday"];
                    drSitmall["thursday"] = dr8["thursday"];
                    drSitmall["friday"] = dr8["friday"];
                    drSitmall["saturday"] = dr8["saturday"];
                    drSitmall["sunday"] = dr8["sunday"];
                    sitmtall.Rows.Add(drSitmall);
                }
            }


            sitmt1sumTmp.Dispose();
            sitmt2sumTmp.Dispose();
            sitmt3sumTmp.Dispose();
            sitmt4sumTmp.Dispose();
            sitmtallsumTmp.Dispose();
            columnChanged = false;

        }

        #endregion

        #region Get Table Max ID

        private int GetTableMaxID(string Tablename)
        {
            string sql1 = "Select ISNULL(MAX(id),0) as maxid from " + Tablename + "";

            this.dbaccess.ReadSQL("Result1", sql1);
            DataRow dr = this.dbaccess.DataSet.Tables["Result1"].Rows[0];

            return  Convert.ToInt32(dr["maxid"]);

        }

        #endregion

        #region Get Min ID

        private int GetTableMinID(string Tablename, string sitenumi)
        {
            string sql1 = "Select ISNULL(Min(id),0) as minid from " + Tablename + " where refnum='"+sitenumi+"'";

            this.dbaccess.ReadSQL("Result2", sql1);
            DataRow dr = this.dbaccess.DataSet.Tables["Result2"].Rows[0];

            return Convert.ToInt32(dr["minid"]);

        }

        #endregion

        #region isAllowedDay

        private bool isAllowedDay(string Day)
        {
            bool allowed = false;
            foreach (DataRow dr1 in AllowedDAys.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {

                    if (dr1["Day"].ToString() == Day || dr1["Day"].ToString() == BizLogicTools.Tools.ToTitleCase(Day))
                    {
                        allowed = true;
                        break;
                    }
                }
            }
            return allowed;
        }

        #endregion

        #region Show Message

        private void ShowMsgAllowScheduleInsert()
        {
            MessageBox.Show("You can't enter Schedule in this Row, Please Check your Contract", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        #region Calculate Day of the Week
        private int EmpTotalofDay(string day, string tablename)
        {
            int total = 0;
            int value = 0;

            DataTable Table = dbaccess.DataSet.Tables[tablename];
            foreach (DataRow dr1 in Table.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr1[day]))
                    {
                        if (dr1[day].ToString().Trim() == "X")
                        {
                            value = 1;
                        }
                        else
                        {
                            value = 0;
                        }                       
                    }
                    else
                    {
                        value = 0;
                    }
                    total = total + value;
                }
            }
            return total;
        }      
        #endregion

        #region Get Footer Totals

        private void GetSitmt1FooterTotals()
        {
            sitmt1Total_monday.Text = Convert.ToString(EmpTotalofDay("monday","sitmt1"));
            sitmt1Total_tuesday.Text = Convert.ToString(EmpTotalofDay("tuesday","sitmt1"));
            sitmt1Total_wednesday.Text = Convert.ToString(EmpTotalofDay("wednesday","sitmt1"));
            sitmt1Total_thursday.Text = Convert.ToString(EmpTotalofDay("thursday","sitmt1"));
            sitmt1Total_friday.Text = Convert.ToString(EmpTotalofDay("friday","sitmt1"));
            sitmt1Total_saturday.Text = Convert.ToString(EmpTotalofDay("saturday","sitmt1"));
            sitmt1Total_sunday.Text = Convert.ToString(EmpTotalofDay("sunday","sitmt1")); 
        }

        private void GetSitmt2FooterTotals()
        {
            sitmt2Total_monday.Text = Convert.ToString(EmpTotalofDay("monday", "sitmt2"));
            sitmt2Total_tuesday.Text = Convert.ToString(EmpTotalofDay("tuesday", "sitmt2"));
            sitmt2Total_wednesday.Text = Convert.ToString(EmpTotalofDay("wednesday", "sitmt2"));
            sitmt2Total_thursday.Text = Convert.ToString(EmpTotalofDay("thursday", "sitmt2"));
            sitmt2Total_friday.Text = Convert.ToString(EmpTotalofDay("friday", "sitmt2"));
            sitmt2Total_saturday.Text = Convert.ToString(EmpTotalofDay("saturday", "sitmt2"));
            sitmt2Total_sunday.Text = Convert.ToString(EmpTotalofDay("sunday", "sitmt2"));
        }

        private void GetSitmt3FooterTotals()
        {
            sitmt3Total_monday.Text = Convert.ToString(EmpTotalofDay("monday", "sitmt3"));
            sitmt3Total_tuesday.Text = Convert.ToString(EmpTotalofDay("tuesday", "sitmt3"));
            sitmt3Total_wednesday.Text = Convert.ToString(EmpTotalofDay("wednesday", "sitmt3"));
            sitmt3Total_thursday.Text = Convert.ToString(EmpTotalofDay("thursday", "sitmt3"));
            sitmt3Total_friday.Text = Convert.ToString(EmpTotalofDay("friday", "sitmt3"));
            sitmt3Total_saturday.Text = Convert.ToString(EmpTotalofDay("saturday", "sitmt3"));
            sitmt3Total_sunday.Text = Convert.ToString(EmpTotalofDay("sunday", "sitmt3"));
        }

        #endregion

        #region Get Contract Footer Totals

        private void GetSitmt1CtrhFooterTotals()
        {
            this.dbaccess.ReadSQL("CTRday",GetQuery("D"));
            DataTable CTRday = this.dbaccess.DataSet.Tables["CTRday"];
            if (CTRday.Rows.Count > 0)
            {
                DataRow drDay = this.dbaccess.DataSet.Tables["CTRday"].Rows[0];


                if (BizFunctions.IsEmpty(drDay["monday"]))
                {
                    drDay["monday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["tuesday"]))
                {
                    drDay["tuesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["wednesday"]))
                {
                    drDay["wednesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["thursday"]))
                {
                    drDay["thursday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["friday"]))
                {
                    drDay["friday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["saturday"]))
                {
                    drDay["saturday"] = 0;
                }
                if (BizFunctions.IsEmpty(drDay["sunday"]))
                {
                    drDay["sunday"] = 0;
                }

                sitmt1Ctrh_monday.Text = drDay["monday"].ToString();
                sitmt1Ctrh_tuesday.Text = drDay["tuesday"].ToString();
                sitmt1Ctrh_wednesday.Text = drDay["wednesday"].ToString();
                sitmt1Ctrh_thursday.Text = drDay["thursday"].ToString();
                sitmt1Ctrh_friday.Text = drDay["friday"].ToString();
                sitmt1Ctrh_saturday.Text = drDay["saturday"].ToString();
                sitmt1Ctrh_sunday.Text = drDay["sunday"].ToString();
            }
           
            CTRday.Dispose();

        }

        private void GetSitmt2CtrhFooterTotals()
        {
            this.dbaccess.ReadSQL("CTRnight", GetQuery("N"));
            DataTable CTRnight = this.dbaccess.DataSet.Tables["CTRnight"];
            if (CTRnight.Rows.Count > 0)
            {
                DataRow drNight = this.dbaccess.DataSet.Tables["CTRnight"].Rows[0];


                if (BizFunctions.IsEmpty(drNight["monday"]))
                {
                    drNight["monday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["tuesday"]))
                {
                    drNight["tuesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["wednesday"]))
                {
                    drNight["wednesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["thursday"]))
                {
                    drNight["thursday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["friday"]))
                {
                    drNight["friday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["saturday"]))
                {
                    drNight["saturday"] = 0;
                }
                if (BizFunctions.IsEmpty(drNight["sunday"]))
                {
                    drNight["sunday"] = 0;
                }

                sitmt2Ctrh_monday.Text = drNight["monday"].ToString();
                sitmt2Ctrh_tuesday.Text = drNight["tuesday"].ToString();
                sitmt2Ctrh_wednesday.Text = drNight["wednesday"].ToString();
                sitmt2Ctrh_thursday.Text = drNight["thursday"].ToString();
                sitmt2Ctrh_friday.Text = drNight["friday"].ToString();
                sitmt2Ctrh_saturday.Text = drNight["saturday"].ToString();
                sitmt2Ctrh_sunday.Text = drNight["sunday"].ToString();
            }
           
            CTRnight.Dispose();

        }

        private void GetSitmt3CtrhFooterTotals()
        {

            this.dbaccess.ReadSQL("CTRconcierge", GetQuery("C"));
            DataTable CTRconcierge = this.dbaccess.DataSet.Tables["CTRconcierge"];
            if (CTRconcierge.Rows.Count > 0)
            {
                DataRow drconcierge = this.dbaccess.DataSet.Tables["CTRconcierge"].Rows[0];

                if (BizFunctions.IsEmpty(drconcierge["monday"]))
                {
                    drconcierge["monday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["tuesday"]))
                {
                    drconcierge["tuesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["wednesday"]))
                {
                    drconcierge["wednesday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["thursday"]))
                {
                    drconcierge["thursday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["friday"]))
                {
                    drconcierge["friday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["saturday"]))
                {
                    drconcierge["saturday"] = 0;
                }
                if (BizFunctions.IsEmpty(drconcierge["sunday"]))
                {
                    drconcierge["sunday"] = 0;
                }

                sitmt3Ctrh_monday.Text = drconcierge["monday"].ToString();
                sitmt3Ctrh_tuesday.Text = drconcierge["tuesday"].ToString();
                sitmt3Ctrh_wednesday.Text = drconcierge["wednesday"].ToString();
                sitmt3Ctrh_thursday.Text = drconcierge["thursday"].ToString();
                sitmt3Ctrh_friday.Text = drconcierge["friday"].ToString();
                sitmt3Ctrh_saturday.Text = drconcierge["saturday"].ToString();
                sitmt3Ctrh_sunday.Text = drconcierge["sunday"].ToString();
            }
           
            CTRconcierge.Dispose();
            
        }

        private string GetQuery(string ShiftType)
        {
            DataRow sitmt = dbaccess.DataSet.Tables["SITMT"].Rows[0];
            string Query = "";
            if (sitmt["contracttype"].ToString() == "ADH")
            {
                Query = "Select "+
	                            "SUM(monday) as monday, "+
	                            "SUM(tuesday) as tuesday, "+
	                            "SUM(wednesday) as wednesday, "+
	                            "SUM(thurday) as thursday, "+
	                            "SUM(friday) as friday, "+
	                            "SUM(saturday) as saturday, "+
                                "SUM(sunday) as sunday " +
                            "from "+
                            "( "+
	                            "select "+
		                            "officerqty, "+
		                            "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, "+
		                            "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, "+
		                            "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, "+
		                            "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, "+
		                            "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday "+
		                            "from CTR1 "+
                                "where refnum='" + sitmt["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +	
                            	
	                            "union all "+
                            	
	                            "select "+
		                            "officerqty, "+
		                            "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, "+
		                            "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, "+
		                            "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, "+
		                            "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, "+
		                            "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, "+
		                            "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday "+
                                    "from ADH1 " +
                                "where adhnum='" + sitmt["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +
                            ")a";
            }
            if (sitmt["contracttype"].ToString() == "ADH")
            {
                Query = "Select " +
                                "SUM(monday) as monday, " +
                                "SUM(tuesday) as tuesday, " +
                                "SUM(wednesday) as wednesday, " +
                                "SUM(thurday) as thursday, " +
                                "SUM(friday) as friday, " +
                                "SUM(saturday) as saturday, " +
                                "SUM(sunday) as sunday " +
                            "from " +
                            "( " +                               
                                "select " +
                                    "officerqty, " +
                                    "ISNULL(officerqty,0) * ISNULL(monday,0) as monday, " +
                                    "ISNULL(officerqty,0) * ISNULL(tuesday,0)as tuesday, " +
                                    "ISNULL(officerqty,0) * ISNULL(wednesday,0) as wednesday, " +
                                    "ISNULL(officerqty,0) * ISNULL(thursday,0)as thurday, " +
                                    "ISNULL(officerqty,0) * ISNULL(friday,0) as friday, " +
                                    "ISNULL(officerqty,0) * ISNULL(saturday,0) as saturday, " +
                                    "ISNULL(officerqty,0) * ISNULL(sunday,0) as sunday " +
                                    "from ADH1 " +
                                "where refnum='" + sitmt["docunum"].ToString().Trim() + "' and  shifttype='" + ShiftType + "' " +
                            ")a";
            }

            return Query;

        }

        #endregion

        #region Check if Each shift is Balanced

        private bool isDayBalanced()
        {
            bool correct = true;
            if (sitmt1Total_monday.Text != sitmt1Ctrh_monday.Text || sitmt1Total_tuesday.Text != sitmt1Ctrh_tuesday.Text
                || sitmt1Total_wednesday.Text != sitmt1Ctrh_wednesday.Text || sitmt1Total_thursday.Text != sitmt1Ctrh_thursday.Text
                || sitmt1Total_friday.Text != sitmt1Ctrh_friday.Text || sitmt1Total_saturday.Text != sitmt1Ctrh_saturday.Text
                || sitmt1Total_sunday.Text != sitmt1Ctrh_sunday.Text)
            {
                correct = false;
            }          
          
            return correct;
        }

        private bool isNightBalanced()
        {
            bool correct = true;
            if (sitmt2Total_monday.Text != sitmt2Ctrh_monday.Text || sitmt2Total_tuesday.Text != sitmt2Ctrh_tuesday.Text
                || sitmt2Total_wednesday.Text != sitmt2Ctrh_wednesday.Text || sitmt2Total_thursday.Text != sitmt2Ctrh_thursday.Text
                || sitmt2Total_friday.Text != sitmt2Ctrh_friday.Text || sitmt2Total_saturday.Text != sitmt2Ctrh_saturday.Text
                || sitmt2Total_sunday.Text != sitmt2Ctrh_sunday.Text)
            {
                correct = false;
            }

            return correct;
        }

        private bool isConciergeBalanced()
        {
            bool correct = true;
            if (sitmt3Total_monday.Text != sitmt3Ctrh_monday.Text || sitmt3Total_tuesday.Text != sitmt3Ctrh_tuesday.Text
                || sitmt3Total_wednesday.Text != sitmt3Ctrh_wednesday.Text || sitmt3Total_thursday.Text != sitmt3Ctrh_thursday.Text
                || sitmt3Total_friday.Text != sitmt3Ctrh_friday.Text || sitmt3Total_saturday.Text != sitmt3Ctrh_saturday.Text
                || sitmt3Total_sunday.Text != sitmt3Ctrh_sunday.Text)
            {
                correct = false;
            }

            return correct;
        }

        #endregion

        #region Check if an Employee is Assigned more than once in the same shift
        private bool isDuplicateInShift(string table1, string table2, string table3, string column1, string column2)
        {
            bool rowsEqual = true; 
            DataTable dataTable1 = this.dbaccess.DataSet.Tables[table1];
           
            for (Int32 r0 = 0; r0 < dataTable1.Rows.Count; r0++)
            {
                

                if (dataTable1.Rows[r0].RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dataTable1.Rows[r0][column1]))
                    {
                        for (Int32 r1 = r0 + 1; r1 < dataTable1.Rows.Count; r1++)
                        {
                            
                            if (dataTable1.Rows[r1].RowState != DataRowState.Deleted)
                            {
                                if (!BizFunctions.IsEmpty(dataTable1.Rows[r1][column1]))
                                {
                                    string EMPNUM1 = dataTable1.Rows[r0][column1].ToString().Trim();
                                    string EMPNUM2 = dataTable1.Rows[r1][column1].ToString().Trim();
                                    string DAY1 = dataTable1.Rows[r0][column2].ToString().Trim();
                                    string DAY2 = dataTable1.Rows[r1][column2].ToString().Trim();
                                   
                                    if (dataTable1.Rows[r0][column1].ToString().Trim() == dataTable1.Rows[r1][column1].ToString().Trim() && dataTable1.Rows[r0][column2].ToString().Trim() == dataTable1.Rows[r1][column2].ToString().Trim())
                                    {
                                        MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column1].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        rowsEqual = false;
                                        break;

                                    }                                  
                                  
                                    if (rowsEqual == false)
                                    {
                                        break;
                                    }
                                }

                                if (rowsEqual && !isExistinOtherTable(dataTable1.Rows[r0][column1].ToString().Trim(), dataTable1.Rows[r0][column2].ToString().Trim(), table2, column1, column2))
                                {
                                    MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column1].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    rowsEqual = false;
                                    break;

                                }

                                if (rowsEqual && !isExistinOtherTable(dataTable1.Rows[r0][column1].ToString().Trim(), dataTable1.Rows[r0][column2].ToString().Trim(), table3, column1, column2))
                                {
                                    MessageBox.Show("Duplicates found for " + dataTable1.Rows[r0][column1].ToString().Trim() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    rowsEqual = false;
                                    break;

                                }
                                //if (rowsEqual && !isExistinOtherTemplates(dataTable1.Rows[r0][column1].ToString().Trim()))
                                //{
                                //    rowsEqual = false;
                                //}
                            }
                        }

                    }

                   

                   

                }

               

            } 

            return rowsEqual;
        }
        #endregion

        #region Check if Employee is Duplicated across Shifts

        private bool isExistinOtherTable(string empnum,string wDay,string datatable,string column1,string column2)
        {
            DataTable dataTable = this.dbaccess.DataSet.Tables[datatable];

            bool rowsEqual = true;

            foreach (DataRow dr1 in dataTable.Rows)
            {
                  if (dr1.RowState != DataRowState.Deleted)
                  {
                      if (!BizFunctions.IsEmpty(dr1[column1]))
                      {
                          if (dr1[column1].ToString().Trim() == empnum && dr1[column2].ToString().Trim() == wDay)
                          {
                              rowsEqual = false;
                              break;
                          }
                      }
                  }
            }
          
            return rowsEqual;
        }

        #endregion

        #region Check if Employee is Duplicated across Shifts

        private bool isExistinOtherTemplates(string empnum)
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];
            bool rowsEqual = true;

            string DayShift = "Select Empnum,sitenumt from sitmt1 where empnum='" + empnum + "' and sitenumt<>'" + sitmt["sitenumt"] + "' and [status]<>'V'";
            string NightShift = "Select Empnum,sitenumt from sitmt2 where empnum='" + empnum + "' and sitenumt<>'" + sitmt["sitenumt"] + "' and [status]<>'V'";
            string Concierge = "Select Empnum,sitenumt from sitmt3 where empnum='" + empnum + "' and sitenumt<>'" + sitmt["sitenumt"] + "' and [status]<>'V'";

            this.dbaccess.ReadSQL("OtherDayShift", DayShift);
            this.dbaccess.ReadSQL("OtherNightShift", NightShift);
            this.dbaccess.ReadSQL("OtherConciergeShift", Concierge);

            if (this.dbaccess.DataSet.Tables["OtherDayShift"].Rows.Count > 0)
            {
                MessageBox.Show("Emp No " + empnum + " already exists in " + this.dbaccess.DataSet.Tables["OtherDayShift"].Rows[0]["sitenumt"].ToString() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rowsEqual = false;
            }

            if (this.dbaccess.DataSet.Tables["OtherNightShift"].Rows.Count > 0)
            {
                MessageBox.Show("Emp No " + empnum + " already exists in " + this.dbaccess.DataSet.Tables["OtherNightShift"].Rows[0]["sitenumt"].ToString() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rowsEqual = false;
            }

            if (this.dbaccess.DataSet.Tables["OtherConciergeShift"].Rows.Count > 0)
            {
                MessageBox.Show("Emp No " + empnum + " already exists in " + this.dbaccess.DataSet.Tables["OtherConciergeShift"].Rows[0]["sitenumt"].ToString() + "", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rowsEqual = false;
            }
            this.dbaccess.DataSet.Tables["OtherDayShift"].Dispose();
            this.dbaccess.DataSet.Tables["OtherNightShift"].Dispose();
            this.dbaccess.DataSet.Tables["OtherConciergeShift"].Dispose();

            return rowsEqual;
        }

        #endregion

        private string GetEmpname(string empnum)
        {
            string empName = "";

            string Get = "Select Empname From hemph where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                empName = dt1.Rows[0]["empname"].ToString();
            }

            dt1.Dispose();

            return empName;

        }

        private string GetMatnum(string empnum)
        {

            string matnum = "";

            string Get = "Select matnum From hemph where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                matnum = dt1.Rows[0]["matnum"].ToString();
            }

            dt1.Dispose();

            return matnum;
        }

        private int GetMaxDayOfWeek(int[] a)
        {
            int max = a[0];

            for (int i = 0; i < a.Length - 1; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                }
            }

            return max;
        }

        private void GetManPowerAmt()
        {
            decimal totalamt = 0;
            DataTable sitmt8 = dbaccess.DataSet.Tables["sitmt8"];
            if (sitmt8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in sitmt8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
            }

            sitmt8_ttlamt.Text = totalamt.ToString();
        }

        private void GetPeriodScheduleCost()
        {
            decimal totalamt = 0;

            DataTable sitmt9 = dbaccess.DataSet.Tables["sitmt9"];
            if (sitmt9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in sitmt9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["actmamt"]))
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }
                        totalamt = totalamt + (decimal)dr1["actmamt"];
                    }
                }
             
               sitmt9_ttlamt.Text = totalamt.ToString();
            }
        }

        #region Refresh Header

        private void Refresh_Header()
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];

            if (BizFunctions.IsEmpty(sitmt["commencedate"]) || BizFunctions.IsEmpty(sitmt["enddate"]))
            {
                sitmt["commencedate"] = DateTime.Now;
                sitmt["enddate"] = DateTime.Now;
            }

            sitmt["totalmonths"] = ASOMS.TimeUtilites.TimeTools.TotelMonthDifference(Convert.ToDateTime(sitmt["commencedate"]), Convert.ToDateTime(sitmt["enddate"]));
            RefreshMonthNo();
            headerFlag = false;
        }

        #endregion

        #region Man Power

        private void Refresh_Manpower()
        {
            DataTable sitmt8 = this.dbaccess.DataSet.Tables["sitmt8"];
            decimal totalamt = 0;

            if (sitmt8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in sitmt8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["actmamt"]))
                        {
                            dr1["actmamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["estmamt"] = 0;
                        }
                        if ((BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0) && !BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }

                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || BizFunctions.IsEmpty(dr1["officerqty"]) || BizFunctions.IsEmpty(dr1["mthnum"]))
                        {
                            dr1["rate"] = 0;
                        }
                        else
                        {
                            dr1["rate"] = (Convert.ToDecimal(dr1["actmamt"]) / Convert.ToDecimal(dr1["officerqty"]));
                        }

                        dr1["oriamt"] = Convert.ToDecimal(dr1["officerqty"]) * Convert.ToDecimal(dr1["actmamt"]);
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
            }
            sitmt8_ttlamt.Text = totalamt.ToString();
            manpowerFlag = false;
        }

        #endregion

        #region Periodic Schedule

        private void Refresh_Schedule()
        {
   
            DataTable sitmt9 = this.dbaccess.DataSet.Tables["sitmt9"];
            decimal totalamt = 0;

            UpdateScheduleList();


            if (sitmt9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in sitmt9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["actmamt"]))
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }
                        dr1["oriamt"] = Convert.ToDecimal(dr1["actmamt"]) * Convert.ToDecimal(dr1["qty"]);
                        //totalamt = totalamt + Convert.ToDecimal(dr1["actmamt"]);

                        totalamt = totalamt + Convert.ToDecimal(dr1["oriamt"]);
                    }
                }
            }     
            sitmt9_ttlamt.Text = totalamt.ToString();
            periodicFlag = false;

        }

        #endregion

        #region Refresh Month No

        private void RefreshMonthNo()
        {
            DataRow sitmt = this.dbaccess.DataSet.Tables["sitmt"].Rows[0];
            DataTable sitmt8 = this.dbaccess.DataSet.Tables["sitmt8"];
            //DataTable sitmt9 = this.dbaccess.DataSet.Tables["sitmt9"];

            if (sitmt8.Rows.Count > 0)
            {
                foreach (DataRow dr8 in sitmt8.Rows)
                {
                    if (dr8.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr8["mthnum"]))
                        {
                            dr8["mthnum"] = sitmt["totalmonths"];
                        }
                    }
                }
            }

            //if (sitmt9.Rows.Count > 0)
            //{
            //    foreach (DataRow dr9 in sitmt9.Rows)
            //    {
            //        if (dr9.RowState != DataRowState.Deleted)
            //        {
            //            if (BizFunctions.IsEmpty(dr9["mthnum"]))
            //            {
            //                dr9["mthnum"] = sitmt["totalmonths"];
            //            }
            //        }
            //    }
            //}


        }

        #endregion

        private void GetContract(string adhocNum)
        {

            DataRow SITMT = this.dbaccess.DataSet.Tables["SITMT"].Rows[0];
            DataTable SITMT8 = this.dbaccess.DataSet.Tables["SITMT8"];
            DataTable SITMT9 = this.dbaccess.DataSet.Tables["SITMT9"];


            Hashtable qctCollection = new Hashtable();

            string getCtrh = "Select * from CTRH where refnum='" + adhocNum + "'";
            string getCtr1 = "Select * from CTR1 where refnum='" + adhocNum + "'";
            string getCtr6 = "Select * from CTR6 where refnum='" + adhocNum + "'";

            qctCollection.Add("CTRH", getCtrh);
            qctCollection.Add("CTR1", getCtr1);
            qctCollection.Add("CTR6", getCtr6);

            this.dbaccess.ReadSQL(qctCollection);

            DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable CTR1 = this.dbaccess.DataSet.Tables["CTR1"];
            DataTable CTR6 = this.dbaccess.DataSet.Tables["CTR6"];

            if (CTRH != null)
            {
                SITMT["contractdate"] = CTRH["trandate"];
                SITMT["docunum"] = CTRH["refnum"];
                SITMT["sitenumi"] = CTRH["sitenumi"];
                SITMT["addr1"] = CTRH["addr1"];
                SITMT["addr2"] = CTRH["addr2"];
                SITMT["addr3"] = CTRH["addr3"];
                SITMT["postalcode"] = CTRH["postalcode"];
                SITMT["sinstruction"] = CTRH["remark"];
                SITMT["contractdate"] = CTRH["contractdate"];
                SITMT["commencedate"] = CTRH["commencedate"];
                SITMT["enddate"] = CTRH["enddate"];
                SITMT["tel1"] = CTRH["tel1"];
                SITMT["tel2"] = CTRH["tel2"];
                SITMT["fax"] = CTRH["sfax"];
                SITMT["rep1"] = CTRH["rep1"];
                SITMT["rep1"] = CTRH["rep1"];
                SITMT["rep1tel"] = CTRH["createdby"];
                SITMT["rep2tel"] = CTRH["rep2tel"];
                SITMT["sectorcode"] = CTRH["sectorcode"];
                

            }





            if (CTR1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(SITMT8);

                foreach (DataRow dr1 in CTR1.Rows)
                {
                    DataRow InsertSITMT8 = SITMT8.NewRow();

                    InsertSITMT8["matnum"] = dr1["matnum"];
                    InsertSITMT8["estmamt"] = dr1["estmamt"];
                    InsertSITMT8["actmamt"] = dr1["actmamt"];
                    InsertSITMT8["officerqty"] = dr1["officerqty"];
                    InsertSITMT8["mthnum"] = dr1["mthnum"];
                    InsertSITMT8["rate"] = dr1["rate"];
                    InsertSITMT8["oriamt"] = dr1["oriamt"];
                    InsertSITMT8["hperd"] = dr1["hperd"];
                    InsertSITMT8["dperw"] = dr1["dperw"];
                    InsertSITMT8["wperm"] = dr1["wperm"];
                    InsertSITMT8["hcost"] = dr1["hcost"];
                    InsertSITMT8["xday1"] = dr1["xday1"];
                    InsertSITMT8["xday2"] = dr1["xday2"];
                    InsertSITMT8["xday3"] = dr1["xday3"];
                    InsertSITMT8["xday4"] = dr1["xday4"];
                    InsertSITMT8["xday5"] = dr1["xday5"];
                    InsertSITMT8["xday6"] = dr1["xday6"];
                    InsertSITMT8["xday7"] = dr1["xday7"];
                    InsertSITMT8["perappoitamt"] = dr1["perappoitamt"];
                    InsertSITMT8["ispubhol"] = dr1["ispubhol"];
                    InsertSITMT8["perhr"] = dr1["perhr"];
                    InsertSITMT8["remark"] = dr1["remark"];

                    SITMT8.Rows.Add(InsertSITMT8);
                }
            }






            if (CTR6.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(SITMT9);

                foreach (DataRow dr6 in CTR6.Rows)
                {
                    DataRow InsertSITMT9 = SITMT9.NewRow();

                    InsertSITMT9["svccode"] = dr6["svccode"];
                    InsertSITMT9["svcdesc"] = dr6["svcdesc"];
                    InsertSITMT9["estmamt"] = dr6["estmamt"];
                    InsertSITMT9["actmamt"] = dr6["actmamt"];
                    InsertSITMT9["qty"] = dr6["qty"];
                    InsertSITMT9["oriamt"] = dr6["oriamt"];
                    InsertSITMT9["frequencycode"] = dr6["frequencycode"];
                    InsertSITMT9["xmonth1"] = dr6["xmonth1"];
                    InsertSITMT9["xmonth2"] = dr6["xmonth2"];
                    InsertSITMT9["xmonth3"] = dr6["xmonth3"];
                    InsertSITMT9["xmonth4"] = dr6["xmonth4"];
                    InsertSITMT9["xmonth5"] = dr6["xmonth5"];
                    InsertSITMT9["xmonth6"] = dr6["xmonth6"];
                    InsertSITMT9["xmonth7"] = dr6["xmonth7"];
                    InsertSITMT9["xmonth8"] = dr6["xmonth8"];
                    InsertSITMT9["xmonth9"] = dr6["xmonth9"];
                    InsertSITMT9["xmonth10"] = dr6["xmonth10"];
                    InsertSITMT9["xmonth11"] = dr6["xmonth11"];
                    InsertSITMT9["xmonth12"] = dr6["xmonth12"];
                    InsertSITMT9["remark"] = dr6["remark"];
                    InsertSITMT9["workscope"] = dr6["workscope"];
                    InsertSITMT9["sitenumi"] = dr6["sitenumi"];

                    SITMT9.Rows.Add(InsertSITMT9);
                }
            }



        }

        private void GetAdhoc(string adhocNum)
        {

            DataRow SITMT = this.dbaccess.DataSet.Tables["SITMT"].Rows[0];
            DataTable SITMT8 = this.dbaccess.DataSet.Tables["SITMT8"];
            DataTable SITMT9 = this.dbaccess.DataSet.Tables["SITMT9"];


            Hashtable qctCollection = new Hashtable();

            string getAdh = "Select * from ADH where refnum='" + adhocNum + "'";
            string getAdh1 = "Select * from ADH1 where refnum='" + adhocNum + "'";
            string getAdh6 = "Select * from ADH6 where refnum='" + adhocNum + "'";

            qctCollection.Add("ADH", getAdh);
            qctCollection.Add("ADH1", getAdh1);
            qctCollection.Add("ADH6", getAdh6);

            this.dbaccess.ReadSQL(qctCollection);

            DataRow ADH = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable ADH1 = this.dbaccess.DataSet.Tables["ADH1"];
            DataTable ADH6 = this.dbaccess.DataSet.Tables["ADH6"];

            if (ADH != null)
            {
                SITMT["contractdate"] = ADH["trandate"];
                SITMT["docunum"] = ADH["refnum"];
                SITMT["sitenumi"] = ADH["sitenumi"];
                SITMT["addr1"] = ADH["addr1"];
                SITMT["addr2"] = ADH["addr2"];
                SITMT["addr3"] = ADH["addr3"];
                SITMT["postalcode"] = ADH["postalcode"];
                SITMT["sinstruction"] = ADH["remark"];
                SITMT["contractdate"] = ADH["contractdate"];
                SITMT["commencedate"] = ADH["commencedate"];
                SITMT["enddate"] = ADH["enddate"];
                SITMT["tel1"] = ADH["tel1"];
                SITMT["tel2"] = ADH["tel2"];
                SITMT["fax"] = ADH["sfax"];
                SITMT["rep1"] = ADH["rep1"];
                SITMT["rep1"] = ADH["rep1"];
                SITMT["rep1tel"] = ADH["createdby"];
                SITMT["rep2tel"] = ADH["rep2tel"];
                SITMT["sectorcode"] = ADH["sectorcode"];

            }


            if (ADH1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(SITMT8);

                foreach (DataRow dr1 in ADH1.Rows)
                {
                    DataRow InsertSITMT8 = SITMT8.NewRow();

                    InsertSITMT8["matnum"] = dr1["matnum"];
                    InsertSITMT8["estmamt"] = dr1["estmamt"];
                    InsertSITMT8["actmamt"] = dr1["actmamt"];
                    InsertSITMT8["officerqty"] = dr1["officerqty"];
                    InsertSITMT8["mthnum"] = dr1["mthnum"];
                    InsertSITMT8["rate"] = dr1["rate"];
                    InsertSITMT8["oriamt"] = dr1["oriamt"];
                    InsertSITMT8["hperd"] = dr1["hperd"];
                    InsertSITMT8["dperw"] = dr1["dperw"];
                    InsertSITMT8["wperm"] = dr1["wperm"];
                    InsertSITMT8["hcost"] = dr1["hcost"];
                    InsertSITMT8["xday1"] = dr1["xday1"];
                    InsertSITMT8["xday2"] = dr1["xday2"];
                    InsertSITMT8["xday3"] = dr1["xday3"];
                    InsertSITMT8["xday4"] = dr1["xday4"];
                    InsertSITMT8["xday5"] = dr1["xday5"];
                    InsertSITMT8["xday6"] = dr1["xday6"];
                    InsertSITMT8["xday7"] = dr1["xday7"];
                    InsertSITMT8["perappoitamt"] = dr1["perappoitamt"];
                    InsertSITMT8["ispubhol"] = dr1["ispubhol"];
                    InsertSITMT8["perhr"] = dr1["perhr"];
                    InsertSITMT8["remark"] = dr1["remark"];

                    SITMT8.Rows.Add(InsertSITMT8);
                }
            }


            if (ADH6.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(SITMT9);

                foreach (DataRow dr6 in ADH6.Rows)
                {
                    DataRow InsertSITMT9 = SITMT9.NewRow();

                    InsertSITMT9["svccode"] = dr6["svccode"];
                    InsertSITMT9["svcdesc"] = dr6["svcdesc"];
                    InsertSITMT9["estmamt"] = dr6["estmamt"];
                    InsertSITMT9["actmamt"] = dr6["actmamt"];
                    InsertSITMT9["qty"] = dr6["qty"];
                    InsertSITMT9["oriamt"] = dr6["oriamt"];
                    InsertSITMT9["frequencycode"] = dr6["frequencycode"];
                    InsertSITMT9["xmonth1"] = dr6["xmonth1"];
                    InsertSITMT9["xmonth2"] = dr6["xmonth2"];
                    InsertSITMT9["xmonth3"] = dr6["xmonth3"];
                    InsertSITMT9["xmonth4"] = dr6["xmonth4"];
                    InsertSITMT9["xmonth5"] = dr6["xmonth5"];
                    InsertSITMT9["xmonth6"] = dr6["xmonth6"];
                    InsertSITMT9["xmonth7"] = dr6["xmonth7"];
                    InsertSITMT9["xmonth8"] = dr6["xmonth8"];
                    InsertSITMT9["xmonth9"] = dr6["xmonth9"];
                    InsertSITMT9["xmonth10"] = dr6["xmonth10"];
                    InsertSITMT9["xmonth11"] = dr6["xmonth11"];
                    InsertSITMT9["xmonth12"] = dr6["xmonth12"];
                    InsertSITMT9["remark"] = dr6["remark"];
                    InsertSITMT9["workscope"] = dr6["workscope"];
                    InsertSITMT9["sitenumi"] = dr6["sitenumi"];

                    SITMT9.Rows.Add(InsertSITMT9);
                }
            }

        }

        private void UpdateScheduleList()
        {
            DataRow SITMT = this.dbaccess.DataSet.Tables["SITMT"].Rows[0];
            DataTable SITMT9 = this.dbaccess.DataSet.Tables["SITMT9"];
            DataTable SITMT11 = this.dbaccess.DataSet.Tables["SITMT11"];



            if (SITMT9.Rows.Count > 0)
            {
                if (SITMT11.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(SITMT11);
                }
                foreach (DataRow dr9 in SITMT9.Rows)
                {
                    if (dr9.RowState != DataRowState.Deleted)
                    {
                        dr9["qty"] = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            if (!BizFunctions.IsEmpty(dr9["xmonth" + i.ToString() + ""]) && !BizFunctions.IsEmpty(dr9["year"]))
                            {
                                dr9["qty"] = Convert.ToDecimal(dr9["qty"]) + Convert.ToDecimal(CountDays(i, Convert.ToInt32(dr9["year"]), dr9["xmonth" + i.ToString() + ""].ToString()));
                                UpdateSchedule(dr9["svccode"].ToString(),dr9["frequencycode"].ToString(), i.ToString(), dr9["year"].ToString(), dr9["xmonth" + i.ToString() + ""].ToString());
                            }
                        }
                    }
                }
            }
        }

        private int CountDays(int month,int year,string days)
        {
            int totalDays = 0;
            string[] sTemp = days.Split(',');
            DateTime dt = new DateTime(year, month, 1);
            
            if (days != string.Empty)
            {
                if (!days.Contains("99"))
                {
                    for (int i = 0; i < sTemp.Length; i++)
                    {
                        totalDays = totalDays + 1;
                    }                   
                }
                else
                {
                    totalDays = totalDays + TimeTools.GetTotalDayMonth(dt);
                }
            }
            return totalDays;
        }

        private void UpdateSchedule(string svcCode,string frequencyCode, string month, string year, string days)
        {
            int totalDays = 0;
            string[] sTemp = days.Split(',');
            DateTime dt = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);

            if (days != string.Empty)
            {
                if (!days.Contains("99"))
                {
                    for (int i = 0; i < sTemp.Length; i++)
                    {
                        InsertSchedule(svcCode,frequencyCode, month, sTemp[i].ToString(), year);
                    }
                }
                else
                {
                    totalDays = TimeTools.GetTotalDayMonth(dt);

                    for (int i = 0; i < totalDays; i++)
                    {
                        InsertSchedule(svcCode,frequencyCode, month, sTemp[i].ToString(), year);
                    }
                    
                }
            }
        }

        private void InsertSchedule(string svcCode, string frequencyCode, string xMonth, string dayNo, string year)
        {
            DataRow SITMT = this.dbaccess.DataSet.Tables["SITMT"].Rows[0];
            DataTable SITMT11 = this.dbaccess.DataSet.Tables["SITMT11"];

            DataRow InsertSITMT11 = SITMT11.NewRow();

            InsertSITMT11["sitenumt"] = SITMT["sitenumt"];
            InsertSITMT11["frequencycode"] = frequencyCode;
            InsertSITMT11["svccode"] = svcCode;
            InsertSITMT11["xmonth"] = Convert.ToInt32(xMonth);
            InsertSITMT11["dayno"] = Convert.ToInt32(dayNo);
            InsertSITMT11["year"] = Convert.ToInt32(year);

            SITMT11.Rows.Add(InsertSITMT11);
        }

    }

}


