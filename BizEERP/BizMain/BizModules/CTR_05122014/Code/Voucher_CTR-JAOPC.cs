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
using ATL.BizModules.TextValidator;
using System.Drawing.Imaging;
using ATL.BizModules.StaCompressFolders;
using ATL.BizModules.FileAcc2;
using ATL.Network;
using ATL.BizModules.RichTextEdit;

#endregion

namespace ATL.CTR
{
    public class Voucher_CTR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName,budgetFormName, manpowerFormName,machineryFormName, equipmentcostsFormName,materialFormName,toiletriesFormName,
                         chemicalFormName,periodicFormName,archiveFormName,tacFormName,feedbackFormName,txt_guid = null;
        protected CheckBox ctrh_daily, ctrh_weekldays, ctrh_weekend = null;
        protected TextBox ctr1_ttlamt, ctr3_ttlamt, txt_ttlamt, ctr5_ttlamt, ctr6_ttlamt, ctr7_ttlamt, ctr8_ttlamt, ctr9_ttlamt, ctrh_arnum, 
                          txt_grossProfit1, txt_grandTotal1,
                          txt_grossProfit2, txt_grandTotal2,
                          txt_grossProfit3, txt_grandTotal3,
                          txt_grossProfit4, txt_grandTotal4,
                          txt_grossProfit5, txt_grandTotal5,
                          txt_grossProfitLsdp1, txt_grandTotalLsdp1,
                          txt_grossProfitLsdp2, txt_grandTotalLsdp2,
                          txt_grossProfitLsdp3, txt_grandTotalLsdp3,
                          txt_grossProfitLsdp4, txt_grandTotalLsdp4,
                          txt_grossProfitLsdp5, txt_grandTotalLsdp5,
                           txt_Raisedby, txt_desc, txt_followup = null;
        protected Button btn_Voucher_Reports, ctr4_btnUpdate, ctr4_btnAdd, ctr10_btnBrowseEdu, ctr10_btnDownloadEdu;
        protected DateTimePicker txt_Daterasied, ctrh_commencedate;
        protected bool opened, isMouseClicked, enableDocSave = false;
        string strFileName;
        bool headerFlag, budgetFlag, manpowerFlag, machineryFlag, equipmentFlag, materialFlag, toiletriesFlag,chemicalFlag, periodicFlag, archiveFlag, tncFlag, feedbackFlag = false;
        protected DataGrid dgCtr4 = null;       
        #region Components Misc Scan
        private TextBox txtMiscDesc;
        private TextBox txtMiscFileName;
        protected Button btnMiscBrowse;
        protected Button BtnTerms;
        protected Button btnMiscAddPic;
        protected Button btnMiscDelete;
        protected Button btnMiscUpdate;
        protected Button btnMiscExpand;
        protected PictureBox picMiscImg;
        protected FlowLayoutPanel picMiscBg1;
        protected Button btnMiscFirst;
        protected Button btnMiscPrevious;
        protected Button btnMiscNext;
        protected Button btnMiscLast;
        protected TextBox txtMiscPage;
        protected TextBox txtMiscTotalPix;
        protected Form frmThisMisc;
        private bool msgfilter;
        int tempImgAmt = 0;// store the previous img total amount while adding /deleting
        int imgCurrent; // Store current image No.
        int rowPTR7 = 0;//Store the number of existing row in PTR7
        int imgMod;//Store the value after conduct (%)
        static int imgTotal = 18;//Total No of image per page       
        int gQuotient = 0;//Store Quotient after perform (/)
        Boolean inTabHandle = false;//True: In Tab_Handle()
        Boolean atBrowse = false;
        Boolean atFormOnload = false;
        Boolean gInModImg = false;
        int[] idArray;//Store the 'ID' for the pictures in Misc of each page
        string[] idArrayXRay;//Store the 'ID' for pictures in XRAY
        Hashtable Hashmisc;
        string tabName = "Misc";
        protected Form frmXRayThis;
        string[] alFilenames;
        int intAge;
        private int picnumber = 0;
        #endregion
        #endregion

        #region Construct

        public Voucher_CTR(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_CTR.xml", moduleName, voucherBaseHelpers)
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
            e.Handle = !opened;

        }
        #endregion

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;
            DataRow CTRH = e.DBAccess.DataSet.Tables["CTRH"].Rows[0];
            string scheduleoption = CTRH["scheduleoption"].ToString();
           
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.budgetFormName = (e.FormsCollection["budget"] as Form).Name;
            this.manpowerFormName = (e.FormsCollection["manpower"] as Form).Name;
            this.machineryFormName = (e.FormsCollection["machinerycost"] as Form).Name;           
            this.equipmentcostsFormName = (e.FormsCollection["equipmentcost"] as Form).Name;
            this.materialFormName = (e.FormsCollection["materialcost"] as Form).Name;
            this.toiletriesFormName = (e.FormsCollection["toiletriescost"] as Form).Name;
            this.chemicalFormName = (e.FormsCollection["chemicalcost"] as Form).Name;
            this.periodicFormName = (e.FormsCollection["scope"] as Form).Name;
            this.archiveFormName = (e.FormsCollection["archive"] as Form).Name;            
            this.feedbackFormName = (e.FormsCollection["feedback"] as Form).Name;


            if (Convert.IsDBNull(CTRH["trandate"]))
            {
                CTRH["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }

            Initialise();

            initiValues();

            #region ColumnChanged Events
            e.DBAccess.DataSet.Tables["CTRH"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTRH_ColumnChanged);
            e.DBAccess.DataSet.Tables["CTR1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTR1_ColumnChanged);
            e.DBAccess.DataSet.Tables["CTR3"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTR3_ColumnChanged);
            e.DBAccess.DataSet.Tables["CTR5"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTR5_ColumnChanged);
            e.DBAccess.DataSet.Tables["CTR6"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTR6_ColumnChanged);
            e.DBAccess.DataSet.Tables["CTR7"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTR7_ColumnChanged);
            e.DBAccess.DataSet.Tables["CTR8"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTR8_ColumnChanged);
            e.DBAccess.DataSet.Tables["CTR9"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_CTR9_ColumnChanged);
            #endregion


            string GetShiftInfo = "Select * from vShlv";

            this.dbaccess.ReadSQL("vSHLV", GetShiftInfo);

            opened = true;

            if (BizFunctions.IsEmpty(CTRH["tnc"]))
            {
                string varBinary = "0x504B0304140002000800976C904258470FF02F020000B20400000B000000636F6E74656E742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB767B5A2D5BFA99D2CBCBE651D35E9739B5AE978FAAAC299A47CB6C91378FDAE9A36A952FCD4B8FBCC68FB823F9A0CDDFB5B77D196DFD77CFABDBBEF9AE29B7CF2B427CB1CADA6212A2D05C5EDC160E351D0032ABB3ABDB42415BA2B0FFFABBB258BEFDECA379DBAE1EDDBD7B757535BEBA37AEEA8BBBBB0F1F3EBCCBDF9AA65575651B02B8CC0E37DEDBD9D9BF7B55176D5E5BFA6693DBCF0E37F6D112D8B77D5DF9C4A7CAD4A2BA5AD72523399BDECDCB1CAF357777C7BB773FB20C764E7CB57D9E4DF3ED593E2D9BF4AEFD265BB7D582883EDD66066A8E1E0B23F1BFA9FC0ECC3EFBE835F52D7F9F678BA2BCFEECA3269FB6C4DE1FD977E4EFED554D43A8DB22E78EEE7A0087A1BFDCED425F6575765167ABB9F9823E808CF01FDBF2D68BAA5E64A569B0C81A9A9FED557661BE7FDD66CB5956CF3E62440687AC5F4CAAD9B5FD032271F4980543C795CA1F5EF720097FA87F4ABB9D8F8E1EF3A7ABFE1B344C03749545203ECDCFB375D9A62FCDE0D36734751F1D9D2E56EDF5E3BBF6CD23FD7D657E511CDD28057FF3970CCDFCD5D53747FF0F504B0304140002000800976C9042D5F05C2BBA020000F80500000A0000007374796C65732E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB76BB69AFCBBC49E9DD65F388FFA0C6F5F251953545F368992DF2E6513B7D54ADF2A579E791D7F811F7231F9C57B77DF55D536E9F57DBD36AB1CADA621282795716CBB79F7D346FDBD5A3BB77AFAEAEC657F7C6557D7177F7E1C38777F95BD3B4B9BCB86D97D474A03FA1C86DE128FDF0BE25E67945843CCFA6F9F62C9F964D7AD77E23D43D7A2CB4E27F53F91D7D7CF6D1D3FC3C5B976DFA32ABB38B3A5BCDD36704EB236D739E2D8AF2FAB38FDAFC1D7D767718CCF36299A72FD68B495EC7DF95CF56596DA77C7B33021B3AFBF6F52AAF6516225D99F7F0C7F6AA260AD66D410C765E3D9A5665557FF6D193729D9B57B9D57A396380B960F6D9474D5516B3812657C5AC9D7FF651B66E2BC6F2AE87E630CE2FAA7A91955D845766D00C091D3D421F4B2624890D31CCF2BCB858D7C434D532E506F2E536DA359F7D749E950D0D86BFA1296FF2F6B38F76C6F7A70BFD8C5A13A353DFF4F9AEFB0C10565553B42CAA657EDEEA77C5725AE7E0360263F8135FDC823B6D5BE16D90A6C384FA274847081553957D43B55576916F97D975B56E03DA7DB15AECD869F51A756697BFD1D93918DF2F961FD94FE7797131070576F1A9022202B7DB555DD06033A1C3AAAADB3A2B5A7E7191D517C572BBAD56A0E883834F0F14A27E31A95A1A46FC3B10141A70F760E7D3F09B5A10A1297AB8730F5F390EF246E648D7A7957EB1C89A962631A4A07E084801055FB7D97296D596A57D227A34F6E7AC03DF7CDC51DB47FF0F504B0304140002000800976C90425CC0E1810501000000010000150000004D4554412D494E462F6D616E69666573742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E8F1225B16E779D33E32BFA4F4DAB2B17F52D37AF9A8CA9AA279B4CC1679F3A89D3EAA56F972564DD78B7CD93E0ADB3F42471ED8F3A2CCB7A9597D9DBAAEF259916DB7D7ABFCB38FB2D5AA2CA6594B48DEBD5CCEC6DCD3D8EF60DCE6EFDA8FDCDBE7EBB2DC5E65EDFCB38FEE7E94DE3D7A7CB73786A3FF07504B01021400140002000800976C904258470FF02F020000B20400000B0000000000000000000000000000000000636F6E74656E742E786D6C504B01021400140002000800976C9042D5F05C2BBA020000F80500000A00000000000000000000000000580200007374796C65732E786D6C504B01021400140002000800976C90425CC0E181050100000001000015000000000000000000000000003A0500004D4554412D494E462F6D616E69666573742E786D6C504B05060000000003000300B4000000720600000000";
                char[] charArray = varBinary.ToCharArray();
                byte[] byteArray = new byte[charArray.Length];

                CTRH["tnc"] = byteArray;
            }
   
            enableDocSave = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("EnableDocSave"));

          
        }


        private void btn_Voucher_Reports_Click(object sender, EventArgs e)
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (ctrh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO)
            {
                if (BizValidate.CheckRowState(dbaccess.DataSet, "ctrh/ctr1/ctr3"))
                {
                 
                    ReportLists.Reports ReportForm = new ATL.ReportLists.Reports(false, "CTRH", "CTRH", ctrh["refnum"].ToString());

                    ReportForm.ShowDialog();
                }
            }
        }

        private void Initialise()
        {
            DataTable xctr10 = this.dbaccess.DataSet.Tables["ctr10"].Copy();

            if (!this.dbaccess.DataSet.Tables.Contains("xctr10"))
            {
                xctr10.TableName = "xctr10";
                this.dbaccess.DataSet.Tables.Add(xctr10.Copy());
            }

            ctrh_commencedate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ctrh_commencedate") as DateTimePicker;
            ctrh_commencedate.TextChanged += new EventHandler(ctrh_commencedate_TextChanged);

            ctr1_ttlamt = BizXmlReader.CurrentInstance.GetControl(manpowerFormName,"ctr1_ttlamt") as TextBox;
            ctr3_ttlamt = BizXmlReader.CurrentInstance.GetControl(equipmentcostsFormName, "ctr3_ttlamt") as TextBox;           
            ctr5_ttlamt = BizXmlReader.CurrentInstance.GetControl(materialFormName, "ctr5_ttlamt") as TextBox;
            ctr6_ttlamt = BizXmlReader.CurrentInstance.GetControl(periodicFormName, "ctr6_ttlamt") as TextBox;
            ctr7_ttlamt = BizXmlReader.CurrentInstance.GetControl(machineryFormName, "ctr7_ttlamt") as TextBox;
            ctr8_ttlamt = BizXmlReader.CurrentInstance.GetControl(toiletriesFormName, "ctr8_ttlamt") as TextBox;
            ctr9_ttlamt = BizXmlReader.CurrentInstance.GetControl(chemicalFormName, "ctr9_ttlamt") as TextBox;
            ctrh_arnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "ctrh_arnum") as TextBox;

            txt_grandTotal1 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotal1") as TextBox;
            txt_grossProfit1 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfit1") as TextBox;

            txt_grandTotal2 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotal2") as TextBox;
            txt_grossProfit2 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfit2") as TextBox;

            txt_grandTotal3 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotal3") as TextBox;
            txt_grossProfit3 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfit3") as TextBox;

            txt_grandTotal4 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotal4") as TextBox;
            txt_grossProfit4 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfit4") as TextBox;

            txt_grandTotal5 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotal5") as TextBox;
            txt_grossProfit5 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfit5") as TextBox;

            txt_grandTotalLsdp1 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotalLsdp1") as TextBox;
            txt_grossProfitLsdp1 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfitLsdp1") as TextBox;

            txt_grandTotalLsdp2 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotalLsdp2") as TextBox;
            txt_grossProfitLsdp2 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfitLsdp2") as TextBox;

            txt_grandTotalLsdp3 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotalLsdp3") as TextBox;
            txt_grossProfitLsdp3 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfitLsdp3") as TextBox;

            txt_grandTotalLsdp4 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotalLsdp4") as TextBox;
            txt_grossProfitLsdp4 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfitLsdp4") as TextBox;

            txt_grandTotalLsdp5 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grandTotalLsdp5") as TextBox;
            txt_grossProfitLsdp5 = BizXmlReader.CurrentInstance.GetControl(budgetFormName, "txt_grossProfitLsdp5") as TextBox;

           
            ctr4_btnAdd = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "ctr4_btnAdd") as Button;
            ctr4_btnAdd.Click +=new EventHandler(ctr4_btnAdd_Click);
            ctr4_btnUpdate = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "ctr4_btnUpdate") as Button;
            ctr4_btnUpdate.Click +=new EventHandler(ctr4_btnUpdate_Click);

            dgCtr4 = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "dg_feedback") as DataGrid;
            dgCtr4.MouseDoubleClick +=new MouseEventHandler(dgtxt_MouseDoubleClick);

            ctr10_btnBrowseEdu = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "ctr10_btnBrowseEdu") as Button;
            ctr10_btnBrowseEdu.Click +=new EventHandler(ctr10_btnBrowseEdu_Click);
            
            ctr10_btnDownloadEdu = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "ctr10_btnDownloadEdu") as Button;
            ctr10_btnDownloadEdu.Click += new EventHandler(ctr10_btnDownloadEdu_Click);

            BtnTerms = BizXmlReader.CurrentInstance.GetControl(headerFormName, "BtnTerms") as Button;
            BtnTerms.Click +=new EventHandler(BtnTerms_Click);
                       
            GetManPowerAmt();
            GetMaterialCost();
            GetChemicalCost();
            GetMachineryCost();
            GetToiletryCost();
            GetEquipmentCost();
            GetPeriodScheduleCost();        
        }

      


        #endregion

        void ctrh_commencedate_TextChanged(object sender, EventArgs e)
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (!refnumExist(ctrh["refnum"].ToString()))
            {
                if (!BizFunctions.IsEmpty(ctrh["commencedate"]))
                {
                    ctrh["enddate"] = Convert.ToDateTime(ctrh["commencedate"]).AddYears(1);
                }
            }
        }

        private bool refnumExist(string refnum)
        {
            bool exist = false;

            string check = "Select refnum from ctrh where refnum='"+refnum+"'";

            this.dbaccess.ReadSQL("checkCTRRefnum", check);

            DataTable checkCTRRefnum = this.dbaccess.DataSet.Tables["checkCTRRefnum"];

            if (checkCTRRefnum != null)
            {
                if (checkCTRRefnum.Rows.Count > 0)
                {
                    exist = true;
                }
            }
            return exist;
        }

        #region Folder Achive Methonds / Functions

        #region Drawing Controls Events
        private void setEventsForDrawingControls()
        {
            btnMiscExpand.Click += new System.EventHandler(this.btnMiscExpand_Click);
            btnMiscUpdate.Click += new System.EventHandler(this.btnMiscUpdate_Click);
            btnMiscBrowse.Click += new System.EventHandler(btnMiscBrowse_Click);
            btnMiscAddPic.Click += new System.EventHandler(this.btnMiscAddPic_Click);
            btnMiscDelete.Click += new System.EventHandler(this.btnMiscDelete_Click);
            picMiscImg.MouseDown += new MouseEventHandler(this.PicBox_MouseEnter);
        }
        #endregion

        #region Pic Box Mouse Event
        void PicBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (picMiscImg != null)
                    picMiscImg.BorderStyle = BorderStyle.None;
                picMiscImg = (PictureBox)sender;
                picMiscImg.BorderStyle = BorderStyle.Fixed3D;
                string na = picMiscImg.Name;
                txtMiscDesc.Text = Hashmisc[na].ToString();
            }
        }
        #endregion

        #region Misc Browse Button
        private void btnMiscBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                atFormOnload = false;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JPEG(*.JPG;*.JPEG;*.JPE;*.JFIF)|*.jpg;*.jpeg;*.jpe;*.jfif|BMP Files|*.bmp|GIF Files|*.gif|TIFF(*.TIF;*.TIFF)|*.tif;*.tiff|PNG Files|*.png|All Picture Files|*.bmp;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png|All Files|*.*";
                openFileDialog.Multiselect = true;

                if (DialogResult.OK == openFileDialog.ShowDialog(frmThisMisc))
                {
                    int i = openFileDialog.FileNames.Length;
                    alFilenames = new String[i];

                    foreach (string fname in openFileDialog.FileNames)
                    {
                        txtMiscFileName.Text = fname;
                        i--;
                        alFilenames[i] = fname;
                        picMiscImg = new PictureBox();
                        picMiscImg.SizeMode = PictureBoxSizeMode.StretchImage;
                        picMiscImg.Size = new Size(100, 100);
                        picMiscImg.Image = Image.FromFile(txtMiscFileName.Text);
                        picMiscImg.Parent = picMiscBg1;
                        picMiscBg1.Controls.Add(picMiscImg);
                    }
                    Environment.CurrentDirectory = Application.StartupPath;
                }
            }//try
            catch (Exception ex)
            {
                MessageBox.Show(frmThisMisc, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region format date- eg. 5 covert to 05 .
        private string formatDate(string str)
        {
            string value = str.Length > 1 ? str : "0" + str;
            return value;
        }
        #endregion

        #region Misc Add Pic Button
        private void btnMiscAddPic_Click(object sender, EventArgs e)
        {
            try
            {
                #region Save To Picture Box

                //Local Test PC
                string ServerProjDir = "C:\\temp";

                string year = Common.DEFAULT_SYSTEM_YEAR.ToString();
                for (int i = 0; i < alFilenames.Length; i++)
                {
                    Image imgPix = Image.FromFile(alFilenames[i]);
                    picMiscImg.Image = imgPix;

                    DateTime dateNow = DateTime.Now;
                    string mth = formatDate(dateNow.Month.ToString());
                    string day = formatDate(dateNow.Day.ToString());
                    string hr = formatDate(dateNow.Hour.ToString());
                    string mn = formatDate(dateNow.Minute.ToString());
                    string sc = formatDate(dateNow.Second.ToString());
                    string picname = "-P-" + year + mth + day +
                               hr + mn + i + ".JPG";

                    if (!Directory.Exists(ServerProjDir))
                    {
                        //Create a new subfolder under the current active folder
                        string newFolder = System.IO.Path.Combine(ServerProjDir, "");
                        // Create the subfolder
                        System.IO.Directory.CreateDirectory(newFolder);
                    }
                #endregion
      
                    imgPix.Save(ServerProjDir + "\\" + picname, System.Drawing.Imaging.ImageFormat.Jpeg);
                    EncoderParameters parameters = new EncoderParameters(1);
                    parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 20L);
                    ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
                    MemoryStream ms = new MemoryStream();
                    #region Read from MemoryStream into Byte array.
                    Byte[] bytBLOBData = new Byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(bytBLOBData, 0, Convert.ToInt32(ms.Length));

                    if (!MiscAllowToSavePic(bytBLOBData.Length))
                    {
                        MessageBox.Show("Image already exist, Please select another picture!",
                           "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    #endregion
                    strFileName = txtMiscFileName.Text.ToString();
                    picMiscImg.Name = ServerProjDir + "\\" + picname;
                    Hashmisc.Add(ServerProjDir + "\\" + picname, ServerProjDir + "\\" + picname);
                }
                MessageBox.Show("Picture successfully added!", "System Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(frmXRayThis, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Misc Allow To Save Pic
        private bool MiscAllowToSavePic(long lngLength)
        {
            return true;
        }
        #endregion

        #region  Get Encoder 
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {

            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();

            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)

                    return encoders[j];

            }
            return null;
        }
        #endregion

        #region Misc Delete Button
        private void btnMiscDelete_Click(object sender, EventArgs e)
        {
            try
            {
              
                        try
                        {
                            DeletePicMisc(txtMiscDesc.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Delecte Failed!", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        RefreshPagesMisc();
                        MessageBox.Show("Picture Delected!", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                   
                  
               
            }
            catch (Exception ex)
            {
                //FileInfo imgInfo = new FileInfo(picdesc);
            }
        }
        #endregion

        #region delete selected pic from Folder
        private void DeletePicMisc(string picinfo)
        {
            try
            {
                File.Delete(picinfo);
                if (picMiscImg != null)
                {
                    picMiscBg1.Controls.Remove(picMiscImg);
                    picMiscBg1.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                throw ex;
            }
        }

        #endregion

        #region Refresh Page Misc
        private void RefreshPagesMisc()
        {
            try
            {
               
                Hashmisc = new Hashtable();
                picMiscBg1.Controls.Clear();

                tabName = "Misc";
                string ServerProjDir = "C:\\TEMP";
                if (!Directory.Exists(ServerProjDir))
                {
                    //Create a new subfolder under the current active folder
                    string newFolder = System.IO.Path.Combine(ServerProjDir, "");
                    // Create the subfolder
                    System.IO.Directory.CreateDirectory(newFolder);
                }
                string[] files = Directory.GetFiles(ServerProjDir,  "*.jpg", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    strFileName = files[i];
                    PictureBox pb = new PictureBox();
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;
                    pb.Size = new Size(100, 100);

                    using (Image img = Image.FromFile(strFileName))
                    {
                        pb.Image = img.GetThumbnailImage(100, 100, null, System.IntPtr.Zero);
                    }
                    pb.MouseDown += new MouseEventHandler(this.PicBox_MouseEnter);
                    picMiscBg1.Controls.Add(pb);
                    pb.Name = files[i];
                    Hashmisc.Add(pb.Name, files[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Misc Update Button
        private void btnMiscUpdate_Click(object sender, EventArgs e)
        {
            //string fileLoc = "";
            //string newDesc = "";
            //String sql = "select * from PTR4" + Common.DEFAULT_SYSTEM_YEAR
            //           + " Where PatientID = '" + PRGH.Table.Rows[0]["PatientID"].ToString() + "' ";
            //DataTable PTR4 = this.dbaccess.ReadSQLTemp("PTR4" + Common.DEFAULT_SYSTEM_YEAR.ToString(), sql).Tables["PTR4" + Common.DEFAULT_SYSTEM_YEAR.ToString()];
            //fileLoc = txtMiscFileName.Text;
            //newDesc = txtMiscDesc.Text;

            //if (newDesc == "")
            //{
            //    MessageBox.Show("'Description' field cannot be empty!", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //foreach (DataRow drPTR4 in PTR4.Rows)
            //{
            //    if (newDesc != "" && drPTR4["ID"].ToString() == idNo.Trim())
            //    {
            //        #region Update sql
            //        drPTR4["description"] = newDesc;
            //        MessageBox.Show("Update Completed!", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        #endregion

            //        #region Set Visible False for PicBg for selected PB
            //        txtMiscFileName.Text = "";
            //        txtMiscDesc.Text = "";
            //        if (redPBClick == 1)
            //        {
            //            picMiscBg1.Visible = false;
            //        }
            //        #endregion

            //        adjustDescMisc(newDesc, redPBClick);//Refresh 'Description' in Page
            //        clickBefore = false;//Set to default.Indicate the click for each PB.
            //        pbNoClick = 0;//Reset to default
            //    }//if
            //}//foreach
            ////Update Table PTR4 in database
            //DataTable[] savetable = new DataTable[1];
            //savetable[0] = PTR4.GetChanges();
            //this.dbaccess.Update(savetable);
        }
        #endregion

        #region Misc Expand Button
        private void btnMiscExpand_Click(object sender, EventArgs e)
        {
            Form form = BizXmlReader.CurrentInstance.GetForm("Detail3") as Form;
            try
            {
            //    string strfn = Convert.ToString(DateTime.Now.ToFileTime());
            //    Image i2 = Image.FromFile(txtMiscDesc.Text);
            //    ImageConverter converter = new ImageConverter();
            //    byte[] barrImg = (byte[])converter.ConvertTo(i2, typeof(byte[]));
            //    FileStream fs = new FileStream(strfn, FileMode.CreateNew, FileAccess.Write);
            //    fs.Write(barrImg, 0, barrImg.Length);
            //    fs.Flush();
            //    fs.Close();

            //    PictureViewer picViewer = new PictureViewer();
            //    Bitmap bm = new Bitmap(strfn);
            //    picViewer.Image = (Image)bm.Clone();
            //    picViewer.ZoomEnabled = true;
            //    FullScreenPictureForm fullForm = new FullScreenPictureForm();
            //    fullForm.SetPictureViewer(picViewer);
            //    fullForm.ShowDialog(form);
            }//try
            catch (Exception ex)
            {
                MessageBox.Show(form, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #endregion

        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
            {
                if (Convert.ToDecimal(ctrh["totalmonths"]) <= 0)
                {
                    MessageBox.Show("Invalid Number of Months!. Only greater than 0 Monhts is valid", "AO ServicePro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Handle = false;
                }
            }

                        
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            DataRow ctrh = dbaccess.DataSet.Tables["ctrh"].Rows[0];
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "ctrh_arnum":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);

                    if (ctrh_arnum.Text != string.Empty)
                    {
                        //e.DefaultCondition = "(arm.arnum like '%" + ctrh["arnum"].ToString().Trim() + "%' OR arm.arname like '" + ctrh["arnum"].ToString().Trim() + "%') and arm.[status]<>'V'";
                        e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                        
                    }
                    break;

                case "ctrh_sitenum":
                    if (!BizFunctions.IsEmpty(ctrh["arnum"]))
                    {
                        e.Condition = BizFunctions.F2Condition("sitenum,sitename", (sender as TextBox).Text);
                        //e.DefaultCondition = "(sitm.sitenum like '" + ctrh["sitenum"].ToString() + "%' OR sitm.sitename like '" + ctrh["sitenum"].ToString() + "%') and sitm.[status]<>'V'";
                    }
                    else
                    {
                        MessageBox.Show("Please Choose Your Customer first", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        e.Handle = false;
                    }
                    break;              

            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow ctrh = dbaccess.DataSet.Tables["ctrh"].Rows[0];
            switch (e.ControlName)
            {
                // later...
                case "ctrh_sitenum":
                    if (!BizFunctions.IsEmpty(ctrh["sitenum"].ToString()))
                    {
                        string GetSitenumI = "Select * from sitm where sitenum='" + ctrh["sitenum"] + "'";
                        this.dbaccess.ReadSQL("SITM", GetSitenumI);
                        DataTable sitm = this.dbaccess.DataSet.Tables["SITM"];
                        if (sitm.Rows.Count > 0)
                        {
                            DataRow drSitmi = sitm.Rows[0];
                            ctrh["sitename"] = drSitmi["sitename"];
                            ctrh["sectorcode"] = drSitmi["sectorcode"];
                            ctrh["addr1"] = drSitmi["addr1"];
                            ctrh["addr2"] = drSitmi["addr2"];
                            ctrh["addr3"] = drSitmi["addr3"];
                            ctrh["country"] = drSitmi["country"];
                            ctrh["postalcode"] = drSitmi["postalcode"];
                            ctrh["billadd1"] = drSitmi["billadd1"];
                            ctrh["billadd2"] = drSitmi["billadd2"];
                            ctrh["billadd3"] = drSitmi["billadd3"];
                            ctrh["rep1"] = drSitmi["rep1"];
                            ctrh["tel1"] = drSitmi["tel1"];
                            ctrh["fax"] = drSitmi["fax"];
                            ctrh["rep2"] = drSitmi["rep2"];
                            ctrh["tel2"] = drSitmi["tel2"];
                            ctrh["rep1tel"] = drSitmi["rep1tel"];
                            ctrh["rep2tel"] = drSitmi["rep2tel"];
                            ctrh["prmcode"] = drSitmi["prmcode"];
                        
                        }
                    }
                    break;

                case "ctrh_arnum":
                    {
                        ctrh["arname"] = e.F2CurrentRow["arname"];
                        ctrh["billadd1"] = e.F2CurrentRow["baddr1"];
                        ctrh["billadd2"] = e.F2CurrentRow["baddr2"];
                        ctrh["billadd3"] = e.F2CurrentRow["baddr3"];
                        ctrh["billadd4"] = e.F2CurrentRow["baddr4"];
                    }
                    break;

                case "ctrh_qctnum":
                    {
                        if (!BizFunctions.IsEmpty(ctrh["qctnum"].ToString()))
                        {
                            GetQuotation(ctrh["qctnum"].ToString());
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
                case "xday1":
                    e.Condition = BizFunctions.F2Condition("shiftcode", (sender as TextBox).Text);
                    break;

                case "xday2":
                    e.Condition = BizFunctions.F2Condition("shiftcode", (sender as TextBox).Text);
                    break;

                case "xday3":
                    e.Condition = BizFunctions.F2Condition("shiftcode", (sender as TextBox).Text);
                    break;

                case "xday4":
                    e.Condition = BizFunctions.F2Condition("shiftcode", (sender as TextBox).Text);
                    break;

                case "xday5":
                    e.Condition = BizFunctions.F2Condition("shiftcode", (sender as TextBox).Text);
                    break;

                case "xday6":
                    e.Condition = BizFunctions.F2Condition("shiftcode", (sender as TextBox).Text);
                    break;

                case "xday7":
                    e.Condition = BizFunctions.F2Condition("shiftcode", (sender as TextBox).Text);
                    break;


                case "frequencycode":
                    e.Condition = BizFunctions.F2Condition("frequencycode", (sender as TextBox).Text);
                    break;

                case "matnum":
                    e.Condition = BizFunctions.F2Condition("matnum,matname", (sender as TextBox).Text);
                    break;
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow ctrh = dbaccess.DataSet.Tables["ctrh"].Rows[0];
            DataTable ctr1 = dbaccess.DataSet.Tables["ctr1"];

            switch (e.MappingName)
            {
                case "shiftcode":
                    {
                        e.CurrentRow["monday"] = e.F2CurrentRow["monday"];
                        e.CurrentRow["tuesday"] = e.F2CurrentRow["tuesday"];
                        e.CurrentRow["wednesday"] = e.F2CurrentRow["wednesday"];
                        e.CurrentRow["thursday"] = e.F2CurrentRow["thursday"];
                        e.CurrentRow["friday"] = e.F2CurrentRow["friday"];
                        e.CurrentRow["saturday"] = e.F2CurrentRow["saturday"];
                        e.CurrentRow["sunday"] = e.F2CurrentRow["sunday"];
                        e.CurrentRow["shifttype"] = e.F2CurrentRow["shifttype"];
                        
                    }
                    break;

                case "matnum":
                    {
                        if (e.TableName != "ctr1")
                        {
                            e.CurrentRow["estmamt"] = GetLatestMatnumCost(e.F2CurrentRow["matnum"].ToString());
                        }
                                             
                    }
                    break;

                case "frequencycode":
                    {
                        if (e.TableName == "ctr6")
                        {
                            e.CurrentRow["qty"] = e.F2CurrentRow["qty"];
                        }

                    }
                    break;

                case "svccode":
                    if (e.TableName == "ctr6")
                    {
                        if (BizFunctions.IsEmpty(e.CurrentRow["qty"]))
                        {
                            if (!BizFunctions.IsEmpty(e.F2CurrentRow["qty"]))
                            {
                                if (Convert.ToDecimal(e.F2CurrentRow["qty"]) > 0)
                                {
                                    e.CurrentRow["qty"] = e.F2CurrentRow["qty"];
                                }
                                else
                                {
                                    e.CurrentRow["qty"] = 1;
                                }
                            }
                            else
                            {
                                e.CurrentRow["qty"] = 1;
                            }
                        }
                    }
                    break;

            }
        }

        #endregion

        #region  ReOpen/void

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

        }
        #endregion

        #endregion

        #region Paste Handle
        protected override void Document_Paste_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Paste_Handle(sender, e);
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            ctrh["guid"] = System.DBNull.Value;
        }

        protected override void Document_Paste_OnClick(object sender, BizRAD.BizDocument.DocumentEventArgs e)
        {
            base.Document_Paste_OnClick(sender, e);
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable CTR1 = this.dbaccess.DataSet.Tables["ctr1"];
            DataTable CTR3 = this.dbaccess.DataSet.Tables["CTR3"];
            DataTable CTR4 = this.dbaccess.DataSet.Tables["CTR4"];
            DataTable CTR5 = this.dbaccess.DataSet.Tables["CTR5"];
            DataTable CTR6 = this.dbaccess.DataSet.Tables["CTR6"];
            DataTable CTR7 = this.dbaccess.DataSet.Tables["CTR7"];
            DataTable CTR8 = this.dbaccess.DataSet.Tables["CTR8"];
            DataTable CTR9 = this.dbaccess.DataSet.Tables["CTR9"];
            DataTable CTR10 = this.dbaccess.DataSet.Tables["ctr10"];

            ctrh["guid"] = System.DBNull.Value;
            ctrh["createdby"] = System.DBNull.Value;
            ctrh["issuedby"] = System.DBNull.Value;

            if (CTR1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in CTR1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["guid"] = System.DBNull.Value;
                    }
                }
            }

            if (CTR3.Rows.Count > 0)
            {
                foreach (DataRow dr3 in CTR3.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        dr3["guid"] = System.DBNull.Value;
                    }
                }
            }

            if (CTR4.Rows.Count > 0)
            {
                foreach (DataRow dr4 in CTR4.Rows)
                {
                    if (dr4.RowState != DataRowState.Deleted)
                    {
                        dr4["guid"] = System.DBNull.Value;
                    }
                }
            }
            
            if (CTR5.Rows.Count > 0)
            {
                foreach (DataRow dr5 in CTR5.Rows)
                {
                    if (dr5.RowState != DataRowState.Deleted)
                    {
                        dr5["guid"] = System.DBNull.Value;
                    }
                }
            }


            if (CTR6.Rows.Count > 0)
            {
                foreach (DataRow dr6 in CTR6.Rows)
                {
                    if (dr6.RowState != DataRowState.Deleted)
                    {
                        dr6["guid"] = System.DBNull.Value;
                    }
                }
            }


            if (CTR7.Rows.Count > 0)
            {
                foreach (DataRow dr7 in CTR7.Rows)
                {
                    if (dr7.RowState != DataRowState.Deleted)
                    {
                        dr7["guid"] = System.DBNull.Value;
                    }
                }
            }


            if (CTR8.Rows.Count > 0)
            {
                foreach (DataRow dr8 in CTR8.Rows)
                {
                    if (dr8.RowState != DataRowState.Deleted)
                    {
                        dr8["guid"] = System.DBNull.Value;
                    }
                }
            }


            if (CTR9.Rows.Count > 0)
            {
                foreach (DataRow dr9 in CTR9.Rows)
                {
                    if (dr9.RowState != DataRowState.Deleted)
                    {
                        dr9["guid"] = System.DBNull.Value;
                    }
                }
            }


            if (CTR10.Rows.Count > 0)
            {
                foreach (DataRow dr10 in CTR10.Rows)
                {
                    if (dr10.RowState != DataRowState.Deleted)
                    {
                        dr10["guid"] = System.DBNull.Value;
                    }
                }
            }
        }
        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];
            DataTable ctr1 = this.dbaccess.DataSet.Tables["ctr1"];
            DataTable CTR3 = this.dbaccess.DataSet.Tables["CTR3"];
            DataTable CTR4 = this.dbaccess.DataSet.Tables["CTR4"];
            DataTable CTR5 = this.dbaccess.DataSet.Tables["CTR5"];
            DataTable CTR6 = this.dbaccess.DataSet.Tables["CTR6"];
            DataTable CTR7 = this.dbaccess.DataSet.Tables["CTR7"];
            DataTable CTR8 = this.dbaccess.DataSet.Tables["CTR8"];
            DataTable CTR9 = this.dbaccess.DataSet.Tables["CTR9"];
            DataTable ctr10 = this.dbaccess.DataSet.Tables["ctr10"];

            foreach (DataRow dr1 in ctr1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr1, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr3 in CTR3.Rows)
            {
                if (dr3.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr3, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr4 in CTR4.Rows)
            {
                if (dr4.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr4, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr5 in CTR5.Rows)
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr5, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr6 in CTR6.Rows)
            {
                if (dr6.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr6, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr7 in CTR7.Rows)
            {
                if (dr7.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr7, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr8 in CTR8.Rows)
            {
                if (dr8.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr8, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr9 in CTR9.Rows)
            {
                if (dr9.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr9, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr10 in ctr10.Rows)
            {
                if (dr10.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ctrh, dr10, "refnum/user/flag/status/created/modified");
                }
            }


            DataTable xctr10 = ctr10.GetChanges(DataRowState.Deleted);

            if (BizFunctions.IsEmpty(ctrh["trandate"]))
            {
                ctrh["trandate"] = DateTime.Now;
            }


            #region Personal Files/Docs Record
            if (enableDocSave)
            {
                if (ctr10.Rows.Count > 0)
                {
                    try
                    {

                        string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("ASOMSDocsRepository"));

                        if (DriveLetter.Trim() != "")
                        {

                            foreach (DataRow dr11 in ctr10.Rows)
                            {

                                if (dr11.RowState != DataRowState.Deleted)
                                {
                                    BizFunctions.UpdateDataRow(ctrh, dr11, "refnum/user/flag/status/created/modified");

                                    if (BizFunctions.IsEmpty(dr11["physicalserverlocation"]))
                                    {
                                        FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), ctrh["arnum"].ToString(), Convert.ToDateTime(ctrh["commencedate"]), dr11["flname"].ToString());
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

                                        fsg1 = null;

                                    }
                                    else
                                    {
                                        if (!File.Exists(dr11["physicalserverlocation"].ToString()))
                                        {
                                            FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), ctrh["arnum"].ToString(), Convert.ToDateTime(ctrh["commencedate"]), dr11["flname"].ToString());
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
                                            fsg1 = null;
                                        }
                                    }


                                }

                            }

                        }
                        else
                        {
                            MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "AO ServicePro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            #endregion

                if (xctr10 != null)
                {

                    if (this.dbaccess.DataSet.Tables.Contains("xctr10"))
                    {
                        this.dbaccess.DataSet.Tables.Remove("xctr10");
                        xctr10 = ctr10.GetChanges(DataRowState.Deleted);

                        xctr10.TableName = "xctr10";

                        this.dbaccess.DataSet.Tables.Add(xctr10);
                    }
                    else
                    {
                        xctr10.TableName = "xctr10";

                        this.dbaccess.DataSet.Tables.Add(xctr10);
                    }
                }



                if (ctrh["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSV)
                {
                    if (!BizFunctions.IsEmpty(ctrh["tnc"]))
                    {
                        WordForm1 wf = new WordForm1(this.dbaccess, "CTRH", "tnc", "CTRH");

                        //MemoryStream ms = new MemoryStream();

                        if (!wf.SaveToFile())
                        {
                            MessageBox.Show("Unable to Save Service Agreement Document, Please try again later.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            ctrh["tncLoc"] = wf.FileInServerLocation;
                        }

                        wf.Dispose();
                    }
                }
            }
            


        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataTable xctr10 = this.dbaccess.DataSet.Tables["xctr10"];

            #region  Make Save Changes in Education Doc Files
            if (enableDocSave)
            {
                if (xctr10 != null)
                {
                    try
                    {

                        string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("ASOMSDocsRepository"));

                        if (DriveLetter1.Trim() != "")
                        {
                            if (xctr10.Rows.Count > 0)
                            {

                                foreach (DataRow dr1 in xctr10.Rows)
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
                        BizFunctions.DeleteAllRows(xctr10);

                        if (this.dbaccess.DataSet.Tables.Contains("xctr10"))
                        {
                            this.dbaccess.DataSet.Tables.Remove("xctr10");
                        }
                    }


                    NetworkDrive.DisconnectNetworkDrive(true);
                }
            }
            #endregion
            ATL.BizModules.Tools.MemoryManagement.FlushMemory();
    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
           
            #region Refresh IF Flag value=True
           
            Refresh_Header();

            if (manpowerFlag)
            {
                Refresh_Manpower();
            }
            if (machineryFlag)
            {
                Refresh_Machinery();
            }
            if (equipmentFlag)
            {
                Refresh_Equipment();
            }
            if (materialFlag)
            {
                Refresh_Material();
            }
            if (toiletriesFlag)
            {
                Refresh_Toiletries();
            }
            if (chemicalFlag)
            {
                Refresh_Chemical();
            }
            if (periodicFlag)
            {
                Refresh_Schedule();
            }
          

            Refresh_Budget();
            
            #endregion

            if (!BizFunctions.IsEmpty(ctrh["sectorcode"]))
            {
                string[] arr1 = new string[2];

                arr1 = GetZoneSupervisor(ctrh["sectorcode"].ToString());

                ctrh["empnum"] = arr1[0];
                ctrh["empname"] = arr1[1];

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

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);

            DataRow ctrh = dbaccess.DataSet.Tables["ctrh"].Rows[0];
            if (ctrh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "ctrh/ctr1"))
                {
                    MessageBox.Show("Please Summarize then Save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion

        #region Print on Click

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

            DataRow ctrh = e.DBAccess.DataSet.Tables["ctrh"].Rows[0];
            Hashtable selectedCollection = new Hashtable();

            switch (e.ReportName)
            {

                case "Contract Form 1":
                    e.DataSource = ctrds1();
                    break;

                case "Contract Form 2":
                    e.DataSource = ctrds1();
                    break;

            }

        }

        #endregion
       
        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow ctrh = dbaccess.DataSet.Tables["ctrh"].Rows[0];
            if (ctrh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "ctrh"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion
      
        #region Vouchers ColumnChanged Events
        private void Voucher_CTRH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];

            switch (e.Column.ColumnName)
            {

                case "ctrnum":
                    {
                        headerFlag = true;
                     break;
                    }
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
                case "subtotal":
                    {
                        headerFlag = true;
                    }
                    break;

                case "additionalcosts":
                    {
                        headerFlag = true;
                    }
                    break;
                case "discamt":
                    {
                        headerFlag = true;
                    }
                    break;

                case "equipmentcosts":
                    {
                        headerFlag = true;
                    }
                    break;
            }
        }

        #region Manpower
        private void Voucher_CTR1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ctr1 = dbaccess.DataSet.Tables["ctr1"];
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    {
                        manpowerFlag = true;
                    }
                    break;

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
                case "xday1":
                    {
                        manpowerFlag = true;
                    }
                    break;
                case "xday2":
                    {
                        manpowerFlag = true;
                    }
                    break;
                case "xday3":
                    {
                        manpowerFlag = true;
                    }
                    break;
                case "xday4":
                    {
                        manpowerFlag = true;
                    }
                    break;
                case "xday5":
                    {
                        manpowerFlag = true;
                    }
                    break;
                case "xday6":
                    {
                        manpowerFlag = true;
                    }
                    break;
                case "xday7":
                    {
                        manpowerFlag = true;
                    }
                    break;

                case "ispubhol":
                    {
                        manpowerFlag = true;
                    }
                    break;

            }
        }
        #endregion

        #region Equipment
        private void Voucher_CTR3_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ctr3 = dbaccess.DataSet.Tables["ctr3"];
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    {
                        equipmentFlag = true;
                    }
                    break;

                case "mthnum":
                    {
                        equipmentFlag = true;
                    }
                    break;

                case "qty":
                    {
                        equipmentFlag = true;
                    }
                    break;

                case "actmamt":
                    {
                        equipmentFlag = true;
                    }
                    break;

                case "estmamt":
                    {
                        equipmentFlag = true;
                    }
                    break;
             
                #region old
              
                #endregion
            }
        }
        #endregion

        #region Materials
        private void Voucher_CTR5_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {            
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    {
                        materialFlag = true;
                    }
                    break;

                case "mthnum":
                    {
                        materialFlag = true;
                    }
                    break;

                case "qty":
                    {
                        materialFlag = true;
                    }
                    break;

                case "estmamt":
                    {
                        materialFlag = true;
                    }
                    break;
                case "actmamt":
                    {
                        materialFlag = true;
                    }
                    break;     
            }
        }
        #endregion

        private void Voucher_CTR6_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ctr6 = dbaccess.DataSet.Tables["ctr6"];
            switch (e.Column.ColumnName)
            {
                case "svccode":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "mthnum":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "actmamt":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "oriamt":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "frequencycode":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "estmamt":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "qty":
                    {
                        periodicFlag = true;
                    }
                    break;
                case "allmonth":
                    {
                        if ((bool)e.Row["allmonth"])
                        {
                            e.Row["xmonth1"] = true;
                            e.Row["xmonth2"] = true;
                            e.Row["xmonth3"] = true;
                            e.Row["xmonth4"] = true;
                            e.Row["xmonth5"] = true;
                            e.Row["xmonth6"] = true;
                            e.Row["xmonth7"] = true;
                            e.Row["xmonth8"] = true;
                            e.Row["xmonth9"] = true;
                            e.Row["xmonth10"] = true;
                            e.Row["xmonth11"] = true;
                            e.Row["xmonth12"] = true;
                        }
                        else
                        {
                            e.Row["xmonth1"] = false;
                            e.Row["xmonth2"] = false;
                            e.Row["xmonth3"] = false;
                            e.Row["xmonth4"] = false;
                            e.Row["xmonth5"] = false;
                            e.Row["xmonth6"] = false;
                            e.Row["xmonth7"] = false;
                            e.Row["xmonth8"] = false;
                            e.Row["xmonth9"] = false;
                            e.Row["xmonth10"] = false;
                            e.Row["xmonth11"] = false;
                            e.Row["xmonth12"] = false;
                        }
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

        #region Machinery
        private void Voucher_CTR7_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {         
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    {
                        machineryFlag = true;
                    }
                    break;

                case "mthnum":
                    {
                        machineryFlag = true;
                    }
                    break;

                case "qty":
                    {
                        machineryFlag = true;
                    }
                    break;

                case "estmamt":
                    {
                        machineryFlag = true;
                    }
                    break;
                case "actmamt":
                    {
                        machineryFlag = true;
                    }
                    break;                                       
            }
        }
        #endregion

        #region Toiletries
        private void Voucher_CTR8_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    {
                        toiletriesFlag = true;
                    }
                    break;

                case "mthnum":
                    {
                        toiletriesFlag = true;
                    }
                    break;

                case "qty":
                    {
                        toiletriesFlag = true;
                    }
                    break;

                case "estmamt":
                    {
                        toiletriesFlag = true;
                    }
                    break;
                case "actmamt":
                    {
                        toiletriesFlag = true;
                    }
                    break;     
            }
        }
        #endregion

        #region Chemicals
        private void Voucher_CTR9_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    {
                        chemicalFlag = true;
                    }
                    break;

                case "mthnum":
                    {
                        chemicalFlag = true;
                    }
                    break;

                case "qty":
                    {
                        chemicalFlag = true;
                    }
                    break;

                case "estmamt":
                    {
                        chemicalFlag = true;
                    }
                    break;
                case "actmamt":
                    {
                        chemicalFlag = true;
                    }
                    break;
            }
        }
        #endregion

        #endregion

        #region Other Functions / Methods

        private void GetManPowerAmt()
        {
            decimal totalamt = 0;
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr1 = dbaccess.DataSet.Tables["ctr1"];
            foreach (DataRow dr1 in ctr1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr1["oriamt"]))
                    {
                        dr1["oriamt"] = 0;
                    }
                    totalamt = totalamt + (decimal)dr1["oriamt"];
                }
            }
            ctrh["bgtsactsal"] = totalamt;
            ctr1_ttlamt.Text = totalamt.ToString();            
        }

        private void GetMaterialCost()
        {
            decimal totalamt = 0;
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr5 = dbaccess.DataSet.Tables["ctr5"];
            foreach (DataRow dr1 in ctr5.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr1["oriamt"]))
                    {
                        dr1["oriamt"] = 0;
                    }
                    totalamt = totalamt + (decimal)dr1["oriamt"];
                }
            }
            ctrh["bgtsactmat"] = totalamt;
            ctr5_ttlamt.Text = totalamt.ToString();
        }

        private void GetChemicalCost()
        {
            decimal totalamt = 0;
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr9 = dbaccess.DataSet.Tables["ctr9"];
            if (ctr9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
                ctrh["bgtsactchem"] = totalamt;
                ctr9_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetMachineryCost()
        {
            decimal totalamt = 0;
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr7 = dbaccess.DataSet.Tables["ctr7"];
            if (ctr7.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr7.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["oriamt"]))
                        {
                            dr1["oriamt"] = 0;
                        }
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
                ctrh["bgtsactmach"] = totalamt;
                ctr7_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetToiletryCost()
        {
            decimal totalamt = 0;
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr8 = dbaccess.DataSet.Tables["ctr8"];
            if (ctr8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
                ctrh["bgtsactToi"] = totalamt;
                ctr8_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetEquipmentCost()
        {
            decimal totalamt = 0;
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr3 = dbaccess.DataSet.Tables["ctr3"];
            if (ctr3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr3.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["oriamt"]))
                        {
                            dr1["oriamt"] = 0;
                        }
                        totalamt = totalamt + Convert.ToDecimal(dr1["oriamt"]);
                    }
                }
                ctrh["bgtsactequip"] = totalamt;
                ctr3_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetPeriodScheduleCost()
        {
            decimal totalamt = 0;
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr6 = dbaccess.DataSet.Tables["ctr6"];
            if (ctr6.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr6.Rows)
                {                  
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["actmamt"]))
                        {
                            if (BizFunctions.IsEmpty(dr1["estmamt"]))
                            {
                                dr1["estmamt"] = 0;
                            }
                            dr1["actmamt"] = dr1["estmamt"];
                        }
                        totalamt = totalamt + (decimal)dr1["actmamt"];
                    }
                }
                ctrh["bgtsactPrd"] = totalamt;
                ctr6_ttlamt.Text = totalamt.ToString();
            }
        }

        private decimal GetTotalMatnumCost(DataTable dt,string columnname)
        {
            decimal totalAmout = 0;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            if (BizFunctions.IsEmpty(dr1["qty"]))
                            {
                                dr1["qty"] = 0;
                            }

                            totalAmout = totalAmout + (Convert.ToDecimal(dr1["qty"]) * GetLatestMatnumCost(dr1[columnname].ToString()));
                        }
                    }
                }
            }
            return totalAmout;
        }

        private decimal GetLatestMatnumCost(string matnum)
        {
            decimal amount = 0;

            string GetAmout = "SELECT "+
                                    "retail "+
                                "FROM "+
                                "( "+
                                "select  "+
	                                "retail, "+
	                                "ROW_NUMBER() OVER (Order BY effectivedate) as ForTop,ROW_NUMBER() OVER (Order BY effectivedate Desc) as ForBottom  "+
                                "from matm1  "+
                                "where effectivedate<=GETDATE() "+
                                "and matnum ='"+ matnum +"'  "+
                                ")A  "+
                                "WHERE ForBottom=1";

            this.dbaccess.ReadSQL("tempPrice", GetAmout);

            DataTable tempPrice = this.dbaccess.DataSet.Tables["tempPrice"];
            if (tempPrice != null)
            {
                if (tempPrice.Rows.Count > 0)
                {
                    DataRow dr1 = tempPrice.Rows[0];
                    if (BizFunctions.IsEmpty(dr1["retail"]))
                    {
                        dr1["retail"] = 0;
                    }
                    amount = Convert.ToDecimal(dr1["retail"]);
                }
            }
            tempPrice.Dispose();

            return amount;
        }
    
        private void CountOfficers()
        {
            DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr1 = dbaccess.DataSet.Tables["ctr1"];
            string sqlCount = "select SUM(officerqty) as Oqty from [ctr1]";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, sqlCount);
            DataRow dr1 = dt1.Rows[0];

            if (dr1["Oqty"] != System.DBNull.Value) 
            {
                CTRH["officerqty"] = dr1["Oqty"];
            }
            else
            {
                CTRH["officerqty"] = 0;
            }

        }

        private string GetTimeDetails(string shiftcode)
        {
            string TimeDetails = "";

            string getTimeDetails = "Select timein,[timeout] from vshlv where shiftcode='" + shiftcode + "'";

            DataTable vSHLVtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getTimeDetails);

            if (vSHLVtmp.Rows.Count > 0)
            {
                TimeDetails = vSHLVtmp.Rows[0]["timein"].ToString() + "-" + vSHLVtmp.Rows[0]["timeout"].ToString();
            }

            return TimeDetails;
        }

        private decimal GetShiftHrs(string shiftcode)
        {
            decimal Hrs = 0;

            string getTimeDetails = "Select TTLWORKINGHRS from vshlv where shiftcode='" + shiftcode + "'";

            DataTable vSHLVtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getTimeDetails);

            if (vSHLVtmp.Rows.Count > 0)
            {
                Hrs = Convert.ToDecimal(vSHLVtmp.Rows[0]["TTLWORKINGHRS"]);
            }

            return Hrs;
        }

        private string GetShiftTimeToTime(string shiftcode)
        {
            string timein = ""; 
            string timeout = "";

            string getTimeDetails = "Select TIMEIN,[TIMEOUT] from vshlv where shiftcode='" + shiftcode + "'";

            DataTable vSHLVtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getTimeDetails);

            if (vSHLVtmp.Rows.Count > 0)
            {
                timein = Convert.ToString(vSHLVtmp.Rows[0]["TIMEIN"]);
                timeout = Convert.ToString(vSHLVtmp.Rows[0]["TIMEOUT"]);
            }

            return timein + "-" + timeout;
        }

        private int isWorkShift(string shiftcode)
        {
            int isWorkSh = 0;

            string getTimeDetails = "Select Convert(int,ISNULL(isWorkShift,0)) as sWorkShift from vshlv where shiftcode='" + shiftcode + "'";

            DataTable vSHLVtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getTimeDetails);

            if (vSHLVtmp.Rows.Count > 0)
            {
                isWorkSh = Convert.ToInt32(vSHLVtmp.Rows[0]["sWorkShift"]);
            }

            return isWorkSh;
        }

        private DataSet ctrds1()
        {
            DataSet ds1 = new DataSet("CTRds1");
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];
            DataTable ctr1 = this.dbaccess.DataSet.Tables["ctr1"];
            DataTable ctr3 = this.dbaccess.DataSet.Tables["ctr1"];


            DataTable vSHLV = this.dbaccess.DataSet.Tables["vshlv"];


            string GetCTR1 = " SELECT " +
                                    "A2.matnum, " +
                                    "B.officerqty, " +
                                    "CONVERT(bit,A2.monday) AS monday, " +
                                    "CONVERT(bit,A2.tuesday) AS tuesday, " +
                                    "CONVERT(bit,A2.wednesday) AS wednesday, " +
                                    "CONVERT(bit,A2.thursday) AS thursday, " +
                                    "CONVERT(bit,A2.friday) AS friday, " +
                                    "CONVERT(bit,A2.saturday) AS saturday, " +
                                    "CONVERT(bit,A2.sunday) AS sunday, " +
                                    "CONVERT(bit,A2.ispubhol) AS ispubhol " +
                                    "from " +
                                    "( " +
                                        "SELECT  " +
                                            "A1.matnum, " +
                                            "A1.monday, " +
                                            "A1.tuesday, " +
                                            "A1.wednesday, " +
                                            "A1.thursday, " +
                                            "A1.friday, " +
                                            "A1.saturday, " +
                                            "A1.sunday, " +
                                            "A1.ispubhol " +
                                            "from " +
                                        "( " +
                                            "select  " +
                                                "matnum, " +
                                                "CASE WHEN SUM(CONVERT(int,monday))>=1 THEN 1 ELSE 0 END AS monday, " +
                                                "CASE WHEN SUM(CONVERT(int,tuesday))>=1 THEN 1 ELSE 0 END AS tuesday, " +
                                                "CASE WHEN SUM(CONVERT(int,wednesday))>=1 THEN 1 ELSE 0 END AS wednesday, " +
                                                "CASE WHEN SUM(CONVERT(int,thursday))>=1 THEN 1 ELSE 0 END AS thursday, " +
                                                "CASE WHEN SUM(CONVERT(int,friday))>=1 THEN 1 ELSE 0 END AS friday, " +
                                                "CASE WHEN SUM(CONVERT(int,saturday))>=1 THEN 1 ELSE 0 END AS saturday, " +
                                                "CASE WHEN SUM(CONVERT(int,sunday))>=1 THEN 1 ELSE 0 END AS sunday, " +
                                                "CASE WHEN SUM(CONVERT(int,sunday))>=1 THEN 1 ELSE 0 END AS ispubhol " +
                                            "from CTR1 group by matnum, monday,tuesday,wednesday,thursday,friday,saturday,sunday,ispubhol " +
                                            ")A1 " +
                                        "group by A1.matnum, A1.monday,A1.tuesday,A1.wednesday,A1.thursday,A1.friday,A1.saturday,A1.sunday,A1.ispubhol " +
                                   ")A2 " +
                                "LEFT JOIN " +
                                "( " +
                                "SELECT matnum,MAX(officerqty) as officerqty from " +
                                "CTR1  " +
                                "GROUP BY matnum " +
                                ") " +
                                "B ON A2.MATNUM=B.MATNUM";

            DataTable CTRH = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM CTRH");
            DataTable CTR1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetCTR1);
            DataTable CTR12 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select matnum,officerqty,shiftcode,monday,tuesday,wednesday,thursday,friday,saturday,sunday,ispubhol from CTR1");

            CTR12.Columns.Add("TimeDetails", typeof(string));

            if (CTR12.Rows.Count > 0)
            {
                foreach (DataRow dr1 in CTR12.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["timedetails"] = GetTimeDetails(dr1["shiftcode"].ToString());
                    }
                }
            }


            DataTable CTR3 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM CTR3");

            dbaccess.ReadSQL("MATMtmp", "SELECT * FROM MATM");

            DataTable MATM1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM MATMtmp");

            //string getARM = "SELECT * FROM ARM WHERE arnum='" + ctrh["arnum"].ToString() + "'";

            //this.dbaccess.ReadSQL("ARMtmp", getARM);

            //DataTable ARMtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getARM);


            //////////////////////////////////////

            string getARM = "SELECT * FROM ARM WHERE arnum='" + ctrh["arnum"].ToString() + "'";
            this.dbaccess.ReadSQL("ARMtmp", getARM);
            DataTable ARMtmp = this.dbaccess.DataSet.Tables["ARMtmp"];
            //this.dbaccess.ReadSQL("ARMtmp", getARM);

            //////////////////////////////////////

            if (ds1.Tables.Contains("ARMtmp1"))
            {
                ds1.Tables["ARMtmp1"].Dispose();
                ds1.Tables.Remove("ARMtmp1");
                DataTable ARMtmp1 = ARMtmp.Copy();
                ARMtmp1.TableName = "ARMtmp1";
                ds1.Tables.Add(ARMtmp1);
            }
            else
            {
                DataTable ARMtmp1 = ARMtmp.Copy();
                ARMtmp1.TableName = "ARMtmp1";
                ds1.Tables.Add(ARMtmp1);
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


            if (ds1.Tables.Contains("CTRH"))
            {
                ds1.Tables["CTRH"].Dispose();
                ds1.Tables.Remove("CTRH");
                CTRH.TableName = "CTRH";
                ds1.Tables.Add(CTRH);
            }
            else
            {
                CTRH.TableName = "CTRH";
                ds1.Tables.Add(CTRH);
            }

            if (ds1.Tables.Contains("CTR1"))
            {
                ds1.Tables["CTR1"].Dispose();
                ds1.Tables.Remove("CTR1");
                CTR1.TableName = "CTR1";
                ds1.Tables.Add(CTR1);
            }
            else
            {
                CTR1.TableName = "CTR1";
                ds1.Tables.Add(CTR1);
            }

            if (ds1.Tables.Contains("CTR12"))
            {
                ds1.Tables["CTR12"].Dispose();
                ds1.Tables.Remove("CTR12");
                CTR12.TableName = "CTR12";
                ds1.Tables.Add(CTR12);
            }
            else
            {
                CTR12.TableName = "CTR12";
                ds1.Tables.Add(CTR12);
            }

            if (ds1.Tables.Contains("CTR3"))
            {
                ds1.Tables["CTR3"].Dispose();
                ds1.Tables.Remove("CTR3");
                CTR3.TableName = "CTR3";
                ds1.Tables.Add(CTR3);
            }
            else
            {
                CTR3.TableName = "CTR3";
                ds1.Tables.Add(CTR3);
            }


            return ds1;

        }

        #region Refresh Functions

        #region Refresh Header

        private void Refresh_Header()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            if (BizFunctions.IsEmpty(ctrh["commencedate"]) || BizFunctions.IsEmpty(ctrh["enddate"]))
            {
                ctrh["commencedate"] = DateTime.Now;
                ctrh["enddate"] = DateTime.Now;
            }
            ctrh["totalmonths"] = ATL.TimeUtilites.TimeTools.MonthDiff(Convert.ToDateTime(ctrh["commencedate"]), Convert.ToDateTime(ctrh["enddate"]));
            RefreshMonthNo();
            headerFlag = false;
        }

        #endregion

        #region Refresh Budget

        private void Refresh_Budget()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr1 = this.dbaccess.DataSet.Tables["CTR1"];
            DataTable ctr3 = this.dbaccess.DataSet.Tables["CTR3"];
            DataTable ctr5 = this.dbaccess.DataSet.Tables["CTR5"];
            DataTable ctr6 = this.dbaccess.DataSet.Tables["CTR6"];
            DataTable ctr7 = this.dbaccess.DataSet.Tables["CTR7"];
            DataTable ctr8 = this.dbaccess.DataSet.Tables["CTR8"];
            DataTable ctr9 = this.dbaccess.DataSet.Tables["CTR9"];

            
            #region Manual Actual
            if (Convert.ToDecimal(ctrh["bgtActMctr"]) <= 0)
            {
                ctrh["bgtActMctr"] = ctrh["bgtestctr"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMctrldsp"]) <= 0)
            {
                ctrh["bgtActMctrldsp"] = ctrh["bgtestctrldsp"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMLandSub"]) <= 0)
            {
                ctrh["bgtActMLandSub"] = ctrh["bgtestLandSub"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMsal"]) <= 0)
            {
                ctrh["bgtActMsal"] = ctrh["bgtestsal"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMmat"]) <= 0)
            {
                ctrh["bgtActMmat"] = ctrh["bgtestmat"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMchem"]) <= 0)
            {
                ctrh["bgtActMchem"] = ctrh["bgtestchem"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMmach"]) <= 0)
            {
                ctrh["bgtActMmach"] = ctrh["bgtestmach"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMequip"]) <= 0)
            {
                ctrh["bgtActMequip"] = ctrh["bgtestequip"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMToi"]) <= 0)
            {
                ctrh["bgtActMToi"] = ctrh["bgtestToi"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMPrd"]) <= 0)
            {
                ctrh["bgtActMPrd"] = ctrh["bgtestPrd"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMLand"]) <= 0)
            {
                ctrh["bgtActMLand"] = ctrh["bgtestLand"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMSub"]) <= 0)
            {
                ctrh["bgtActMSub"] = ctrh["bgtestSub"];
            }
            if (Convert.ToDecimal(ctrh["bgtActMOther"]) <= 0)
            {
                ctrh["bgtActMOther"] = ctrh["bgtEstOther"];
            }
            #endregion End Manual Actual

            #region System Actual
            //if (Convert.ToDecimal(ctrh["bgtsactctr"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMctr"]) > 0)
                {
                    ctrh["bgtsactctr"] = ctrh["bgtActMctr"];
                }
                else
                {
                    ctrh["bgtsactctr"] = ctrh["bgtestctr"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtsactctrldsp"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMctrldsp"]) > 0)
                {
                    ctrh["bgtsactctrldsp"] = ctrh["bgtActMctrldsp"];
                }
                else
                {
                    ctrh["bgtsactctrldsp"] = ctrh["bgtestctrldsp"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtsactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLandSub"]) > 0)
                {
                    ctrh["bgtsactLandSub"] = ctrh["bgtActMLandSub"];
                }
                else
                {
                    ctrh["bgtsactLandSub"] = ctrh["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtsactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLand"]) > 0)
                {
                ctrh["bgtsactLand"] = ctrh["bgtActMLand"];
                }
                else
                {
                    ctrh["bgtsactLand"] = ctrh["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtsactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMSub"]) > 0)
                {
                    ctrh["bgtsactSub"] = ctrh["bgtActMSub"];
                }
                else
                {
                    ctrh["bgtsactSub"] = ctrh["bgtestSub"];
                }

            //}
            //if (Convert.ToDecimal(ctrh["bgtsactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMOther"]) > 0)
                {
                ctrh["bgtsactOther"] = ctrh["bgtActMOther"];
                }
                else
                {
                    ctrh["bgtsactOther"] = ctrh["bgtestOther"];
                }
            //}
            #endregion End System Actual

            #region Actual
            //if (Convert.ToDecimal(ctrh["bgtactctr"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMctr"]) > 0)
                {
                    ctrh["bgtactctr"] = ctrh["bgtActMctr"];
                }
                else
                {
                    ctrh["bgtactctr"] = ctrh["bgtestctr"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactctrldsp"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMctrldsp"]) > 0)
                {
                    ctrh["bgtactctrldsp"] = ctrh["bgtActMctrldsp"];
                }
                else
                {
                    ctrh["bgtactctrldsp"] = ctrh["bgtestctrldsp"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactsal"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMsal"]) > 0)
                {
                    ctrh["bgtactsal"] = ctrh["bgtActMsal"];
                }
                else
                {
                    ctrh["bgtactsal"] = ctrh["bgtestsal"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactmat"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMmat"]) > 0)
                {
                    ctrh["bgtactmat"] = ctrh["bgtActMmat"];
                }
                else
                {
                    ctrh["bgtactmat"] = ctrh["bgtestmat"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactchem"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMchem"]) > 0)
                {
                    ctrh["bgtactchem"] = ctrh["bgtActMchem"];
                }
                else
                {
                    ctrh["bgtactchem"] = ctrh["bgtestchem"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactmach"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMmach"]) > 0)
                {
                    ctrh["bgtactmach"] = ctrh["bgtActMmach"];
                }
                else
                {
                    ctrh["bgtactmach"] = ctrh["bgtestmach"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactequip"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMequip"]) > 0)
                {
                    ctrh["bgtactequip"] = ctrh["bgtActMequip"];
                }
                else
                {
                    ctrh["bgtactequip"] = ctrh["bgtestequip"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactToi"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMToi"]) > 0)
                {
                    ctrh["bgtactToi"] = ctrh["bgtActMToi"];
                }
                else
                {
                    ctrh["bgtactToi"] = ctrh["bgtestToi"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactPrd"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMPrd"]) > 0)
                {
                    ctrh["bgtactPrd"] = ctrh["bgtActMPrd"];
                }
                else
                {
                    ctrh["bgtactPrd"] = ctrh["bgtestPrd"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMOther"]) > 0)
                {
                    ctrh["bgtactOther"] = ctrh["bgtActMOther"];
                }
                else
                {
                    ctrh["bgtactOther"] = ctrh["bgtestOther"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMSub"]) > 0)
                {
                    ctrh["bgtactSub"] = ctrh["bgtActMSub"];
                }
                else
                {
                    ctrh["bgtactSub"] = ctrh["bgtestSub"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLand"]) > 0)
                {
                    ctrh["bgtactLand"] = ctrh["bgtActMLand"];
                }
                else
                {
                    ctrh["bgtactLand"] = ctrh["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLandSub"]) > 0)
                {
                    ctrh["bgtactLandSub"] = ctrh["bgtActMLandSub"];
                }
                else
                {
                    ctrh["bgtactLandSub"] = ctrh["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLandSub"]) > 0)
                {
                    ctrh["bgtactLandSub"] = ctrh["bgtActMLandSub"];
                }
                else
                {
                    ctrh["bgtactLandSub"] = ctrh["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLand"]) > 0)
                {
                    ctrh["bgtactLand"] = ctrh["bgtActMLand"];
                }
                else
                {
                    ctrh["bgtactLand"] = ctrh["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMSub"]) > 0)
                {
                    ctrh["bgtactSub"] = ctrh["bgtActMSub"];
                }
                else
                {
                    ctrh["bgtactSub"] = ctrh["bgtestSub"];
                }

            //}
            //if (Convert.ToDecimal(ctrh["bgtactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMOther"]) > 0)
                {
                    ctrh["bgtactOther"] = ctrh["bgtActMOther"];
                }
                else
                {
                    ctrh["bgtactOther"] = ctrh["bgtestOther"];
                }
            //}
            #endregion End Actual

            #region Latest Actual
            //if (Convert.ToDecimal(ctrh["bgtlactctr"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMctr"]) > 0)
                {
                    ctrh["bgtlactctr"] = ctrh["bgtActMctr"];
                }
                else
                {
                    ctrh["bgtlactctr"] = ctrh["bgtestctr"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtlactctrldsp"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMctrldsp"]) > 0)
                {
                    ctrh["bgtlactctrldsp"] = ctrh["bgtActMctrldsp"];
                }
                else
                {
                    ctrh["bgtlactctrldsp"] = ctrh["bgtestctrldsp"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtlactsal"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMsal"]) > 0)
                {
                    ctrh["bgtlactsal"] = ctrh["bgtActMsal"];
                }
                else
                {
                    ctrh["bgtlactsal"] = ctrh["bgtestsal"];
                }
            //}

                #region Take the latest cost from matm for each item from the Detailed Costing

                if (Convert.ToDecimal(ctrh["totalmonths"]) > 0)
                {
                    ctrh["bgtlactmat"] = GetTotalMatnumCost(ctr5, "matnum") / Convert.ToDecimal(ctrh["totalmonths"]);
                    ctrh["bgtlactchem"] = GetTotalMatnumCost(ctr9, "matnum") / Convert.ToDecimal(ctrh["totalmonths"]);
                    ctrh["bgtlactmach"] = GetTotalMatnumCost(ctr7, "matnum") / Convert.ToDecimal(ctrh["totalmonths"]);
                    ctrh["bgtlactequip"] = GetTotalMatnumCost(ctr3, "matnum") / Convert.ToDecimal(ctrh["totalmonths"]);
                    ctrh["bgtlactToi"] = GetTotalMatnumCost(ctr8, "matnum") / Convert.ToDecimal(ctrh["totalmonths"]);
                }

                #endregion

            //if (Convert.ToDecimal(ctrh["bgtlactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLandSub"]) > 0)
                {
                    ctrh["bgtlactLandSub"] = ctrh["bgtActMLandSub"];
                }
                else
                {
                    ctrh["bgtlactLandSub"] = ctrh["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtlactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMLand"]) > 0)
                {
                    ctrh["bgtlactLand"] = ctrh["bgtActMLand"];
                }
                else
                {
                    ctrh["bgtlactLand"] = ctrh["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtlactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMSub"]) > 0)
                {
                    ctrh["bgtlactSub"] = ctrh["bgtActMSub"];
                }
                else
                {
                    ctrh["bgtlactSub"] = ctrh["bgtestSub"];
                }

            //}
            //if (Convert.ToDecimal(ctrh["bgtlactPrd"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMPrd"]) > 0)
                {
                    ctrh["bgtlactPrd"] = ctrh["bgtActMPrd"];
                }
                else
                {
                    ctrh["bgtlactPrd"] = ctrh["bgtestPrd"];
                }
            //}
            //if (Convert.ToDecimal(ctrh["bgtlactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(ctrh["bgtActMOther"]) > 0)
                {
                    ctrh["bgtlactOther"] = ctrh["bgtActMOther"];
                }
                else
                {
                    ctrh["bgtlactOther"] = ctrh["bgtestOther"];
                }
            //}
            #endregion Latest Actual

            #region Get Grand Total and Gross Profit

            getBgtEstTotal();
            getBgtManualActTotal();
            getBgtSysActTotal();
            getBgtActualTotal();
            getBgtLatestActTotal();
            
            #endregion

        }

        #endregion

        #region Refresh Man Power

        private void Refresh_Manpower()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr1 = this.dbaccess.DataSet.Tables["CTR1"];
            decimal totalamt = 0;

            if (BizFunctions.IsEmpty(ctrh["wkdaysmth"]))
            {
                ctrh["wkdaysmth"] = 22;
            }

            if (Convert.ToDecimal(ctrh["wkdaysmth"]) > 0)
            {

                if (ctr1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ctr1.Rows)
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

                            if (Convert.ToDecimal(dr1["actmamt"]) <= 0 && Convert.ToDecimal(dr1["estmamt"]) > 0)
                            {
                                dr1["actmamt"] = dr1["estmamt"];
                            }

                            if (BizFunctions.IsEmpty(dr1["actmamt"]) || BizFunctions.IsEmpty(dr1["officerqty"]) || BizFunctions.IsEmpty(dr1["mthnum"]))
                            {
                                dr1["rate"] = 0;
                            }
                            else if (Convert.ToDecimal(dr1["actmamt"]) == 0 || Convert.ToDecimal(dr1["officerqty"]) == 0)
                            {
                                dr1["rate"] = 0;
                            }
                            else
                            {
                                dr1["rate"] = (Convert.ToDecimal(dr1["actmamt"]) / Convert.ToDecimal(dr1["officerqty"]));
                            }

                            decimal totalHrs = 0;
                            int days = 0;
                            // Monday
                            if (!BizFunctions.IsEmpty(dr1["xday1"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday1"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday1"].ToString().Trim());
                            }

                            // Tuesday
                            if (!BizFunctions.IsEmpty(dr1["xday2"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday2"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday2"].ToString().Trim());
                            }

                            // Wednesday
                            if (!BizFunctions.IsEmpty(dr1["xday3"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday3"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday3"].ToString().Trim());
                            }

                            // Thursday
                            if (!BizFunctions.IsEmpty(dr1["xday4"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday4"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday4"].ToString().Trim());
                            }

                            // Friday
                            if (!BizFunctions.IsEmpty(dr1["xday5"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday5"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday5"].ToString().Trim());
                            }

                            // Saturday
                            if (!BizFunctions.IsEmpty(dr1["xday6"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday6"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday6"].ToString().Trim());
                            }

                            // Sunday
                            if (!BizFunctions.IsEmpty(dr1["xday7"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday7"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday7"].ToString().Trim());
                            }

                            dr1["hperd"] = totalHrs;
                            dr1["dperw"] = days;


                            if (BizFunctions.IsEmpty(dr1["officerqty"]))
                            {
                                dr1["officerqty"] = 0;
                            }



                            dr1["oriamt"] = Convert.ToDecimal(dr1["officerqty"]) * Convert.ToDecimal(dr1["actmamt"]);

                            if (Convert.ToDecimal(dr1["oriamt"]) == 0 || Convert.ToDecimal(ctrh["wkdaysmth"]) == 0 || totalHrs == 0 || days == 0)
                            {
                                dr1["hcost"] = 0;
                            }
                            else
                            {
                                dr1["hcost"] = Convert.ToDecimal(dr1["oriamt"]) / (Convert.ToDecimal(ctrh["wkdaysmth"]) * (totalHrs / Convert.ToDecimal(days)));
                            }


                            totalamt = totalamt + Convert.ToDecimal(dr1["oriamt"]);


                            if (BizFunctions.IsEmpty(dr1["remark"]))
                            {
                                bool monToSunIncPH = false;
                                bool monToSunExcPH = false;
                                bool monToSatIncPH = false;
                                bool monToSatExcPH = false;
                                bool monToFriIncPH = false;
                                bool monToFriExcPH = false;
                                bool friToSunIncPH = false;
                                bool friToSunExcPH = false;
                                bool satToSunIncPH = false;
                                bool satTosunExcPH = false;

                               

                                if (!BizFunctions.IsEmpty(dr1["xday1"]) && !BizFunctions.IsEmpty(dr1["xday2"]) && !BizFunctions.IsEmpty(dr1["xday3"]) && !BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !BizFunctions.IsEmpty(dr1["xday7"]) && !BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    monToSunIncPH = true;
                                }
                                else if (!BizFunctions.IsEmpty(dr1["xday1"]) && !BizFunctions.IsEmpty(dr1["xday2"]) && !BizFunctions.IsEmpty(dr1["xday3"]) && !BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !BizFunctions.IsEmpty(dr1["xday7"]) && !!BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    monToSunExcPH = true;
                                }
                                else if (!BizFunctions.IsEmpty(dr1["xday1"]) && !BizFunctions.IsEmpty(dr1["xday2"]) && !BizFunctions.IsEmpty(dr1["xday3"]) && !BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !!BizFunctions.IsEmpty(dr1["xday7"]) && !BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    monToSatIncPH = true;
                                }

                                else if (!BizFunctions.IsEmpty(dr1["xday1"]) && !BizFunctions.IsEmpty(dr1["xday2"]) && !BizFunctions.IsEmpty(dr1["xday3"]) && !BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !!BizFunctions.IsEmpty(dr1["xday7"]) && !!BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    monToSunExcPH = true;
                                }
                                else if (!BizFunctions.IsEmpty(dr1["xday1"]) && !BizFunctions.IsEmpty(dr1["xday2"]) && !BizFunctions.IsEmpty(dr1["xday3"]) && !BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !!BizFunctions.IsEmpty(dr1["xday6"]) && !!BizFunctions.IsEmpty(dr1["xday7"]) && !BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    monToFriIncPH = true;
                                }

                                else if (!BizFunctions.IsEmpty(dr1["xday1"]) && !BizFunctions.IsEmpty(dr1["xday2"]) && !BizFunctions.IsEmpty(dr1["xday3"]) && !BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !!BizFunctions.IsEmpty(dr1["xday6"]) && !!BizFunctions.IsEmpty(dr1["xday7"]) && !!BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    monToFriExcPH = true;
                                }
                                ////
                                else if (!!BizFunctions.IsEmpty(dr1["xday1"]) && !!BizFunctions.IsEmpty(dr1["xday2"]) && !!BizFunctions.IsEmpty(dr1["xday3"]) && !!BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !BizFunctions.IsEmpty(dr1["xday7"]) && !BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    friToSunIncPH = true;
                                }

                                else if (!!BizFunctions.IsEmpty(dr1["xday1"]) && !!BizFunctions.IsEmpty(dr1["xday2"]) && !!BizFunctions.IsEmpty(dr1["xday3"]) && !!BizFunctions.IsEmpty(dr1["xday4"]) && !BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !BizFunctions.IsEmpty(dr1["xday7"]) && !!BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    friToSunExcPH = true;
                                }

                                else if (!!BizFunctions.IsEmpty(dr1["xday1"]) && !!BizFunctions.IsEmpty(dr1["xday2"]) && !!BizFunctions.IsEmpty(dr1["xday3"]) && !!BizFunctions.IsEmpty(dr1["xday4"]) && !!BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !BizFunctions.IsEmpty(dr1["xday7"]) && !BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    satToSunIncPH = true;
                                }

                                else if (!!BizFunctions.IsEmpty(dr1["xday1"]) && !!BizFunctions.IsEmpty(dr1["xday2"]) && !!BizFunctions.IsEmpty(dr1["xday3"]) && !!BizFunctions.IsEmpty(dr1["xday4"]) && !!BizFunctions.IsEmpty(dr1["xday5"]) && !BizFunctions.IsEmpty(dr1["xday6"]) && !BizFunctions.IsEmpty(dr1["xday7"]) && !!BizFunctions.IsEmpty(dr1["ispubhol"]))
                                {
                                    satTosunExcPH = true;
                                }

                                string remark = "";


                                if (monToSunIncPH)
                                {
                                    remark = "MON-SUN(Including PH) "+GetShiftTimeToTime(dr1["xday1"].ToString())+" ";
                                }
                                else if (monToSunExcPH)
                                {
                                    remark = "MON-SUNExcluding PH) "+GetShiftTimeToTime(dr1["xday1"].ToString())+"";
                                }
                                else if (monToSatIncPH)
                                {
                                    remark = "MON-SAT(Including PH) "+GetShiftTimeToTime(dr1["xday1"].ToString())+"";
                                }
                                else if (monToSatExcPH)
                                {
                                    remark = "MON-SAT(Excluding PH) "+GetShiftTimeToTime(dr1["xday1"].ToString())+"";
                                }
                                else if (monToFriIncPH)
                                {
                                    remark = "MON-FRI(Including PH) "+GetShiftTimeToTime(dr1["xday1"].ToString())+"";
                                }
                                else if (monToFriExcPH)
                                {
                                    remark = "MON-FRI(Excluding PH) "+GetShiftTimeToTime(dr1["xday1"].ToString())+"";
                                }
                                else if (friToSunIncPH)
                                {
                                    remark = "FRI-SUN(Including PH) "+GetShiftTimeToTime(dr1["xday5"].ToString())+"";
                                }
                                else if (friToSunExcPH)
                                {
                                    remark = "FRI-SUN(Excluding PH) "+GetShiftTimeToTime(dr1["xday5"].ToString())+"";
                                }

                                else if (satToSunIncPH)
                                {
                                    remark = "SAT to SUN, Including PH) "+GetShiftTimeToTime(dr1["xday6"].ToString())+"";
                                }
                                else if (satTosunExcPH)
                                {
                                    remark = "SAT to SUN, Excluding PH "+GetShiftTimeToTime(dr1["xday6"].ToString())+"";
                                }

                                dr1["remark"] = remark;


                            }
                        }
                    }
                }


                ctrh["bgtsactsal"] = totalamt;
                ctr1_ttlamt.Text = totalamt.ToString();
                manpowerFlag = false;
            }
            else
            {
                MessageBox.Show("Unable to Get Hourly Cost" +
                                "Please keyin the Number of working days per month under Manpower tab",
                                "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Refresh Machinery

        private void Refresh_Machinery()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr7 = this.dbaccess.DataSet.Tables["CTR7"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (ctr7.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr7.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        //{
                        //    dr1["estmamt"] = 0;
                        //}
                        //if (BizFunctions.IsEmpty(dr1["qty"]))
                        //{
                        //    dr1["qty"] = 0;
                        //}
                        //if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        //{
                        //    dr1["actmamt"] = dr1["estmamt"];
                        //}
                        //if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) == 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) == 0)
                        //{
                        //    dr1["price"] = 0;
                        //}
                        //else
                        //{
                        //    dr1["price"] = Convert.ToDecimal(dr1["actmamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        //}
                        //dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["price"]);

                        //totalamt = totalamt + (decimal)dr1["oriamt"];
                        //else
                        //{
                        //    dr1["price"] = Convert.ToDecimal(dr1["oriamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        //}
                        if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["estmamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            dr1["qty"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }


                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

                        if (BizFunctions.IsEmpty(dr1["oriamt"]) || Convert.ToDecimal(dr1["oriamt"]) <= 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) <= 0)
                        {
                            dr1["price"] = 0;
                        }
                        else
                        {
                            dr1["price"] = Convert.ToDecimal(dr1["oriamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        }

                        totalamt = totalamt + (decimal)dr1["oriamt"];
                        monthlyamt = monthlyamt + (decimal)dr1["price"];
                    }
                }
            }
            //ctrh["bgtsactmach"] = totalamt;
            ctrh["bgtsactmach"] = monthlyamt;
            ctr7_ttlamt.Text = totalamt.ToString();
            manpowerFlag = false;
        }

        #endregion

        #region Refresh Equipment

        private void Refresh_Equipment()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr3 = this.dbaccess.DataSet.Tables["CTR3"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (ctr3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr3.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                      
                        //if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        //{
                        //    dr1["estmamt"] = 0;
                        //}
                        //if (BizFunctions.IsEmpty(dr1["qty"]))
                        //{
                        //    dr1["qty"] = 0;
                        //}
                        //if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        //{
                        //    dr1["actmamt"] = dr1["estmamt"];
                        //}
                    

                        //dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

                        //if (BizFunctions.IsEmpty(dr1["oriamt"]) || Convert.ToDecimal(dr1["oriamt"]) <= 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) <= 0)
                        //{
                        //    dr1["price"] = 0;
                        //}
                        //else
                        //{
                        //    dr1["price"] = Convert.ToDecimal(dr1["oriamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        //}

                        if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["estmamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            dr1["qty"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }


                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

                        if (BizFunctions.IsEmpty(dr1["oriamt"]) || Convert.ToDecimal(dr1["oriamt"]) <= 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) <= 0)
                        {
                            dr1["price"] = 0;
                        }
                        else
                        {
                            dr1["price"] = Convert.ToDecimal(dr1["oriamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        }

                        totalamt = totalamt + (decimal)dr1["oriamt"];
                        monthlyamt = monthlyamt + (decimal)dr1["price"];
                    }
                }
            }
            ctrh["bgtsactequip"] = monthlyamt;
            ctr3_ttlamt.Text = totalamt.ToString();
            equipmentFlag = false;
        }

        #endregion

        #region Refresh Material

        private void Refresh_Material()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr5 = this.dbaccess.DataSet.Tables["CTR5"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (ctr5.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr5.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
                        if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["estmamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            dr1["qty"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }
                        
                        //dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["price"]);

                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

                        if (BizFunctions.IsEmpty(dr1["oriamt"]) || Convert.ToDecimal(dr1["oriamt"]) <= 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) <= 0)
                        {
                            dr1["price"] = 0;
                        }
                        else
                        {
                            dr1["price"] = Convert.ToDecimal(dr1["oriamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        }

                        totalamt = totalamt + (decimal)dr1["oriamt"];
                        monthlyamt = monthlyamt + (decimal)dr1["price"];
                    }
                }
            }
            //ctrh["bgtsactmat"] = totalamt;
            ctrh["bgtsactmat"] = monthlyamt;
            ctr5_ttlamt.Text = totalamt.ToString();
            materialFlag = false;
        }

        #endregion

        #region Refresh Toiletries

        private void Refresh_Toiletries()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr8 = this.dbaccess.DataSet.Tables["CTR8"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (ctr8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
                        if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["estmamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            dr1["qty"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }

                    
                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

                        if (BizFunctions.IsEmpty(dr1["oriamt"]) || Convert.ToDecimal(dr1["oriamt"]) <= 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) <= 0)
                        {
                            dr1["price"] = 0;
                        }
                        else
                        {
                            dr1["price"] = Convert.ToDecimal(dr1["oriamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        }

                        totalamt = totalamt + (decimal)dr1["oriamt"];
                        monthlyamt = monthlyamt + (decimal)dr1["price"];
                    }
                }
            }
            //ctrh["bgtsactToi"] = totalamt;
            ctrh["bgtsactToi"] = monthlyamt;
            ctr8_ttlamt.Text = totalamt.ToString();
            toiletriesFlag = false;
        }

        #endregion

        #region Refresh Chemical

        private void Refresh_Chemical()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr9 = this.dbaccess.DataSet.Tables["CTR9"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (ctr9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
                        if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["estmamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            dr1["qty"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }

                        //if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) == 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) == 0)
                        //{
                        //    dr1["price"] = 0;
                        //}
                        //else
                        //{
                        //    dr1["price"] = Convert.ToDecimal(dr1["actmamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        //}
                        //dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["price"]);
                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

                        if (BizFunctions.IsEmpty(dr1["oriamt"]) || Convert.ToDecimal(dr1["oriamt"]) <= 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) <= 0)
                        {
                            dr1["price"] = 0;
                        }
                        else
                        {
                            dr1["price"] = Convert.ToDecimal(dr1["oriamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        }

                        totalamt = totalamt + (decimal)dr1["oriamt"];
                        monthlyamt = monthlyamt + (decimal)dr1["price"];
                    }
                }
            }
            //ctrh["bgtsactchem"] = totalamt;
            ctrh["bgtsactchem"] = monthlyamt;
            ctr9_ttlamt.Text = totalamt.ToString();
            chemicalFlag = false;
        }

        #endregion

        #region Refresh Periodic Schedule

        private void Refresh_Schedule()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr6 = this.dbaccess.DataSet.Tables["CTR6"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (ctr6.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr6.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        ////dr1["mthnum"] = ctrh["totalmonths"];
                        //if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        //{
                        //    dr1["estmamt"] = 0;
                        //}
                        //if (BizFunctions.IsEmpty(dr1["qty"]))
                        //{
                        //    dr1["qty"] = 0;
                        //}
                        //if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        //{
                        //    dr1["actmamt"] = dr1["estmamt"];
                        //}

                        //dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

                        //totalamt = totalamt + Convert.ToDecimal(dr1["oriamt"]);

                        if (BizFunctions.IsEmpty(dr1["estmamt"]))
                        {
                            dr1["estmamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            dr1["qty"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }


                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["actmamt"]);

            

                        if (Convert.ToDecimal(dr1["qty"]) == 12)
                        {
                            dr1["allmonth"] = true;

                        }


                        totalamt = totalamt + (decimal)dr1["oriamt"];
                        
                    }
                   
                }
            }

            if (!BizFunctions.IsEmpty(ctrh["totalmonths"]) || totalamt == 0)
            {
                if (Convert.ToDecimal(ctrh["totalmonths"]) > 0)
                {
                    monthlyamt = totalamt / Convert.ToDecimal(ctrh["totalmonths"]);
                }
            }

            ctrh["bgtsactPrd"] = monthlyamt;
            ctr6_ttlamt.Text = totalamt.ToString();
            periodicFlag = false;

        }

        #endregion

        #region Archive

        private void Refresh_Archive()
        {
        }

        #endregion

        #region Terms and Condition

        private void Refresh_TnC()
        {
        }

        #endregion

        #region Feedback

        private void Refresh_Feedback()
        {
        }

        #endregion

        #region Refresh Month No

        private void RefreshMonthNo()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr1 = this.dbaccess.DataSet.Tables["CTR1"];
            DataTable ctr3 = this.dbaccess.DataSet.Tables["CTR3"];
            DataTable ctr5 = this.dbaccess.DataSet.Tables["CTR5"];
            DataTable ctr6 = this.dbaccess.DataSet.Tables["CTR6"];
            DataTable ctr7 = this.dbaccess.DataSet.Tables["CTR7"];
            DataTable ctr8 = this.dbaccess.DataSet.Tables["CTR8"];
            DataTable ctr9 = this.dbaccess.DataSet.Tables["CTR9"];

            if (ctr1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
                        {
                            dr1["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }

            if (ctr3.Rows.Count > 0)
            {
                foreach (DataRow dr3 in ctr3.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
                        {
                            dr3["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }

            if (ctr5.Rows.Count > 0)
            {
                foreach (DataRow dr5 in ctr5.Rows)
                {
                    if (dr5.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
                        {
                            dr5["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }


            if (ctr6.Rows.Count > 0)
            {
                foreach (DataRow dr6 in ctr6.Rows)
                {
                    if (dr6.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
                        {
                            dr6["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }          

            if (ctr7.Rows.Count > 0)
            {
                foreach (DataRow dr7 in ctr7.Rows)
                {
                    if (dr7.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
                        {
                            dr7["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }

            if (ctr8.Rows.Count > 0)
            {
                foreach (DataRow dr8 in ctr8.Rows)
                {
                    if (dr8.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
                        {
                            dr8["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }

            if (ctr9.Rows.Count > 0)
            {
                foreach (DataRow dr9 in ctr9.Rows)
                {
                    if (dr9.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(ctrh["totalmonths"]))
                        {
                            dr9["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }


        }

        #endregion

        #endregion

        #region Feedback Detail Buttons

        #region Feedback Button Events

        private void ctr4_btnAdd_Click(object sender, EventArgs e)
        {
            DataTable ctr4 = this.dbaccess.DataSet.Tables["CTR4"];
            DataRow InsertCtr4 = ctr4.NewRow();
            txt_Daterasied = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Daterasied") as DateTimePicker;
            txt_Raisedby = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Raisedby") as TextBox;
            txt_desc = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_desc") as TextBox;
            txt_followup = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_followup") as TextBox;

            if (txt_Daterasied.Text != string.Empty || txt_Raisedby.Text != string.Empty || txt_desc.Text != string.Empty || txt_followup.Text != string.Empty)
            {
                InsertCtr4["dateraised"] = Convert.ToDateTime(txt_Daterasied.Text);
                InsertCtr4["Raisedby"] = txt_Raisedby.Text;
                InsertCtr4["desc"] = txt_desc.Text;
                InsertCtr4["followup"] = txt_followup.Text;
                ctr4.Rows.Add(InsertCtr4);
                txt_guid = string.Empty;
            }


        }

        private void ctr4_btnUpdate_Click(object sender, EventArgs e)
        {
            DataTable ctr4 = this.dbaccess.DataSet.Tables["CTR4"];

            txt_Daterasied = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Daterasied") as DateTimePicker;
            txt_Raisedby = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Raisedby") as TextBox;
            txt_desc = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_desc") as TextBox;
            txt_followup = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_followup") as TextBox;

            if (ctr4.Rows.Count > 0)
            {
                foreach (DataRow dr4 in ctr4.Rows)
                {
                    if (dr4.RowState != DataRowState.Deleted)
                    {
                        if (dr4["guid"].ToString() == txt_guid)
                        {
                            dr4["dateraised"] = Convert.ToDateTime(txt_Daterasied.Text);
                            dr4["Raisedby"] = txt_Raisedby.Text;
                            dr4["desc"] = txt_desc.Text;
                            dr4["followup"] = txt_followup.Text;
                        }
                    }
                }
            }

        }

        #endregion

        #region Datagrid Mouse Down and Double Click

        private void dgtxt_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                isMouseClicked = true;
                DataGrid dataGrid = sender as DataGrid;

                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);
                dataGrid.CurrentCell = new DataGridCell(hti.Row, hti.Column);
                if (hti.Row >= 0)
                {
                    dataGrid.Select(hti.Row);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgtxt_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {

                txt_Daterasied = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Daterasied") as DateTimePicker;
                txt_Raisedby = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Raisedby") as TextBox;
                txt_desc = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_desc") as TextBox;
                txt_followup = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_followup") as TextBox;

                txt_Raisedby.Text = string.Empty;
                txt_desc.Text = string.Empty;
                txt_followup.Text = string.Empty;

                isMouseClicked = true;
                #region Get and Set Row

                DataGrid dataGrid = sender as DataGrid;

                int i = dataGrid.CurrentRowIndex;
                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

                if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
                {// if user double click Row Header or Cell, the selected row will be added to CRQ2.
                    dataGrid.Select(i);

                    DataRow drCur = getcurrentrow(dgCtr4);

                    string GetCtr4Row = "Select * from ctr4 where [guid]='" + drCur["guid"].ToString() + "'";

                    DataTable tempCtr4 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetCtr4Row);

                    if (tempCtr4 != null)
                    {
                        if (tempCtr4.Rows.Count > 0)
                        {
                            DataRow dr1 = tempCtr4.Rows[0];
                            txt_Daterasied.Text = Convert.ToDateTime(dr1["dateraised"]).ToShortDateString();
                            txt_Raisedby.Text = dr1["raisedby"].ToString();
                            txt_desc.Text = dr1["desc"].ToString();
                            txt_followup.Text = dr1["followup"].ToString();
                            txt_guid = drCur["guid"].ToString();
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

        private DataRow getcurrentrow(DataGrid datagrid)
        {
            CurrencyManager cm = (CurrencyManager)datagrid.BindingContext[datagrid.DataSource, datagrid.DataMember];
            DataRowView drv = cm.Current as DataRowView;
            DataRow dr = drv.Row;

            return dr;
        }

        #endregion

        #endregion

        #region Folder Archive Buttons

        #region Browse Botton Folder Click Event

        protected void ctr10_btnBrowseEdu_Click(object sender, System.EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaCompress form = new ATL.BizModules.StaCompressFolders.StaCompress(dbaccess, "CTRH", "CTR10", "refnum");
                form.ShowDialog();
                form.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region Download Botton Click Event

        protected void ctr10_btnDownloadEdu_Click(object sender, System.EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaDecommpress form = new ATL.BizModules.StaCompressFolders.StaDecommpress(dbaccess, "CTRH", "CTR10", "refnum");
                form.ShowDialog();
                form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #endregion

        #region Terms & Condition Buttons
        private void BtnTerms_Click(object sender, EventArgs e)
        {
            WordForm1 wf = new WordForm1(this.dbaccess,"CTRH","tnc","CTRH");
            wf.Show();
            wf.Focus();
        }
        #endregion

        private void getBgtEstTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];
           
            decimal grandTotal = Convert.ToDecimal(ctrh["bgtestsal"]) + Convert.ToDecimal(ctrh["bgtestmat"]) + Convert.ToDecimal(ctrh["bgtestchem"]) +
                                 Convert.ToDecimal(ctrh["bgtestmach"]) + Convert.ToDecimal(ctrh["bgtestequip"]) + Convert.ToDecimal(ctrh["bgtestToi"]) + 
                                 Convert.ToDecimal(ctrh["bgtestPrd"]) +  Convert.ToDecimal(ctrh["bgtestSub"]) + Convert.ToDecimal(ctrh["bgtestOther"]);
            txt_grandTotal1.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(ctrh["bgtestctr"]))
            {
                ctrh["bgtestctr"] = 0;
            }
            txt_grossProfit1.Text = Convert.ToString(Math.Round((Convert.ToDecimal(ctrh["bgtestctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(ctrh["bgtestLand"]) + Convert.ToDecimal(ctrh["bgtestLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(ctrh["bgtestctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp1.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp1.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtManualActTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(ctrh["bgtActMsal"]) + Convert.ToDecimal(ctrh["bgtActMmat"]) + Convert.ToDecimal(ctrh["bgtActMchem"]) +
                                 Convert.ToDecimal(ctrh["bgtActMmach"]) + Convert.ToDecimal(ctrh["bgtActMequip"]) + Convert.ToDecimal(ctrh["bgtActMToi"]) +
                                 Convert.ToDecimal(ctrh["bgtActMPrd"]) + Convert.ToDecimal(ctrh["bgtActMSub"]) + Convert.ToDecimal(ctrh["bgtActMOther"]);
            txt_grandTotal2.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(ctrh["bgtActMctr"]))
            {
                ctrh["bgtActMctr"] = 0;
            }
            txt_grossProfit2.Text = Convert.ToString(Math.Round((Convert.ToDecimal(ctrh["bgtActMctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(ctrh["bgtActMLand"]) + Convert.ToDecimal(ctrh["bgtActMLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(ctrh["bgtActMctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp2.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp2.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtSysActTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(ctrh["bgtsactsal"]) + Convert.ToDecimal(ctrh["bgtsactmat"]) + Convert.ToDecimal(ctrh["bgtsactchem"]) +
                                 Convert.ToDecimal(ctrh["bgtsactmach"]) + Convert.ToDecimal(ctrh["bgtsactequip"]) + Convert.ToDecimal(ctrh["bgtsactToi"]) +
                                 Convert.ToDecimal(ctrh["bgtsactPrd"]) + Convert.ToDecimal(ctrh["bgtsactSub"]) + Convert.ToDecimal(ctrh["bgtsactOther"]);
            txt_grandTotal3.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(ctrh["bgtsactctr"]))
            {
                ctrh["bgtsactctr"] = 0;
            }
 
            txt_grossProfit3.Text = Convert.ToString(Math.Round((Convert.ToDecimal(ctrh["bgtsactctr"]) - grandTotal),2));

            decimal grandTotalLsdp = Convert.ToDecimal(ctrh["bgtsactLand"]) + Convert.ToDecimal(ctrh["bgtsactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(ctrh["bgtsactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp3.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp3.Text = Convert.ToString(grosProfitLsdp);
        
        }

        private void getBgtActualTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(ctrh["bgtactsal"]) + Convert.ToDecimal(ctrh["bgtactmat"]) + Convert.ToDecimal(ctrh["bgtactchem"]) +
                                 Convert.ToDecimal(ctrh["bgtactmach"]) + Convert.ToDecimal(ctrh["bgtactequip"]) + Convert.ToDecimal(ctrh["bgtactToi"]) +
                                 Convert.ToDecimal(ctrh["bgtactPrd"]) + Convert.ToDecimal(ctrh["bgtactSub"]) + Convert.ToDecimal(ctrh["bgtactOther"]);
            txt_grandTotal4.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(ctrh["bgtactctr"]))
            {
                ctrh["bgtactctr"] = 0;
            }
            txt_grossProfit4.Text = Convert.ToString(Math.Round((Convert.ToDecimal(ctrh["bgtactctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(ctrh["bgtactLand"]) + Convert.ToDecimal(ctrh["bgtactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(ctrh["bgtactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp4.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp4.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtLatestActTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(ctrh["bgtlactsal"]) + Convert.ToDecimal(ctrh["bgtlactmat"]) + Convert.ToDecimal(ctrh["bgtlactchem"]) +
                                 Convert.ToDecimal(ctrh["bgtlactmach"]) + Convert.ToDecimal(ctrh["bgtlactequip"]) + Convert.ToDecimal(ctrh["bgtlactToi"]) +
                                 Convert.ToDecimal(ctrh["bgtlactPrd"]) + Convert.ToDecimal(ctrh["bgtlactSub"]) + Convert.ToDecimal(ctrh["bgtlactOther"]);
            txt_grandTotal5.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(ctrh["bgtlactctr"]))
            {
                ctrh["bgtlactctr"] = 0;
            }
            txt_grossProfit5.Text = Convert.ToString(Math.Round((Convert.ToDecimal(ctrh["bgtlactctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(ctrh["bgtlactLand"]) + Convert.ToDecimal(ctrh["bgtlactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(ctrh["bgtlactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp5.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp5.Text = Convert.ToString(grosProfitLsdp);
        }

        private void initiValues()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (BizFunctions.IsEmpty(ctrh["bgtestctr"]))
            {
                ctrh["bgtestctr"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestctrldsp"]))
            {
                ctrh["bgtestctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestLandSub"]))
            {
                ctrh["bgtestLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestsal"]))
            {
                ctrh["bgtestsal"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestmat"]))
            {
                ctrh["bgtestmat"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestchem"]))
            {
                ctrh["bgtestchem"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestmach"]))
            {
                ctrh["bgtestmach"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestequip"]))
            {
                ctrh["bgtestequip"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestToi"]))
            {
                ctrh["bgtestToi"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestPrd"]))
            {
                ctrh["bgtestPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestLand"]))
            {
                ctrh["bgtestLand"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestSub"]))
            {
                ctrh["bgtestSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtestOther"]))
            {
                ctrh["bgtestOther"] = 0;
            }

            ///////

            if (BizFunctions.IsEmpty(ctrh["bgtActMctr"]))
            {
                ctrh["bgtActMctr"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMctrldsp"]))
            {
                ctrh["bgtActMctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMLandSub"]))
            {
                ctrh["bgtActMLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMsal"]))
            {
                ctrh["bgtActMsal"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMmat"]))
            {
                ctrh["bgtActMmat"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMchem"]))
            {
                ctrh["bgtActMchem"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMmach"]))
            {
                ctrh["bgtActMmach"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMequip"]))
            {
                ctrh["bgtActMequip"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMToi"]))
            {
                ctrh["bgtActMToi"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMPrd"]))
            {
                ctrh["bgtActMPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMLand"]))
            {
                ctrh["bgtActMLand"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMSub"]))
            {
                ctrh["bgtActMSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMOther"]))
            {
                ctrh["bgtActMOther"] = 0;
            }

            /////////

            if (BizFunctions.IsEmpty(ctrh["bgtsactctr"]))
            {
                ctrh["bgtsactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactctrldsp"]))
            {
                ctrh["bgtsactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactLandSub"]))
            {
                ctrh["bgtsactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactsal"]))
            {
                ctrh["bgtsactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactmat"]))
            {
                ctrh["bgtsactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactchem"]))
            {
                ctrh["bgtsactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactmach"]))
            {
                ctrh["bgtsactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactequip"]))
            {
                ctrh["bgtsactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactToi"]))
            {
                ctrh["bgtsactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactPrd"]))
            {
                ctrh["bgtsactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactLand"]))
            {
                ctrh["bgtsactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactSub"]))
            {
                ctrh["bgtsactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactOther"]))
            {
                ctrh["bgtsactOther"] = 0;
            }

            ////////

            if (BizFunctions.IsEmpty(ctrh["bgtactctr"]))
            {
                ctrh["bgtactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactctrldsp"]))
            {
                ctrh["bgtactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactLandSub"]))
            {
                ctrh["bgtactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactsal"]))
            {
                ctrh["bgtactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactmat"]))
            {
                ctrh["bgtactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactchem"]))
            {
                ctrh["bgtactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactmach"]))
            {
                ctrh["bgtactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactequip"]))
            {
                ctrh["bgtactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactToi"]))
            {
                ctrh["bgtactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactPrd"]))
            {
                ctrh["bgtactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactLand"]))
            {
                ctrh["bgtactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactSub"]))
            {
                ctrh["bgtactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtactOther"]))
            {
                ctrh["bgtactOther"] = 0;
            }

            /////  

            if (BizFunctions.IsEmpty(ctrh["bgtlactctr"]))
            {
                ctrh["bgtlactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactctrldsp"]))
            {
                ctrh["bgtlactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactLandSub"]))
            {
                ctrh["bgtlactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactsal"]))
            {
                ctrh["bgtlactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactmat"]))
            {
                ctrh["bgtlactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactchem"]))
            {
                ctrh["bgtlactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactmach"]))
            {
                ctrh["bgtlactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactequip"]))
            {
                ctrh["bgtlactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactToi"]))
            {
                ctrh["bgtlactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactPrd"]))
            {
                ctrh["bgtlactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactLand"]))
            {
                ctrh["bgtlactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactSub"]))
            {
                ctrh["bgtlactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtlactOther"]))
            {
                ctrh["bgtlactOther"] = 0;
            }


        }

        private void GetQuotation(string qctNum)
        {

            DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable CTR1 = this.dbaccess.DataSet.Tables["CTR1"];
            DataTable CTR3 = this.dbaccess.DataSet.Tables["CTR3"];
            DataTable CTR4 = this.dbaccess.DataSet.Tables["CTR4"];
            DataTable CTR5 = this.dbaccess.DataSet.Tables["CTR5"];
            DataTable CTR6 = this.dbaccess.DataSet.Tables["CTR6"];
            DataTable CTR7 = this.dbaccess.DataSet.Tables["CTR7"];
            DataTable CTR8 = this.dbaccess.DataSet.Tables["CTR8"];
            DataTable CTR9 = this.dbaccess.DataSet.Tables["CTR9"];
            DataTable CTR10 = this.dbaccess.DataSet.Tables["CTR10"];

            Hashtable qctCollection = new Hashtable();

            string getQcth = "Select * from QCTH where refnum='" + qctNum + "'";
            string getQct1 = "Select * from QCT1 where refnum='" + qctNum + "'";
            string getQct3 = "Select * from QCT3 where refnum='" + qctNum + "'";
            string getQct4 = "Select * from QCT4 where refnum='" + qctNum + "'";
            string getQct5 = "Select * from QCT5 where refnum='" + qctNum + "'";
            string getQct6 = "Select * from QCT6 where refnum='" + qctNum + "'";
            string getQct7 = "Select * from QCT7 where refnum='" + qctNum + "'";
            string getQct8 = "Select * from QCT8 where refnum='" + qctNum + "'";
            string getQct9 = "Select * from QCT9 where refnum='" + qctNum + "'";
            string getQct10 = "Select * from QCT10 where refnum='" + qctNum + "'";

            qctCollection.Add("QCTH", getQcth);
            qctCollection.Add("QCT1", getQct1);
            qctCollection.Add("QCT3", getQct3);
            qctCollection.Add("QCT4", getQct4);
            qctCollection.Add("QCT5", getQct5);
            qctCollection.Add("QCT6", getQct6);
            qctCollection.Add("QCT7", getQct7);
            qctCollection.Add("QCT8", getQct8);
            qctCollection.Add("QCT9", getQct9);
            qctCollection.Add("QCT10", getQct10);

            this.dbaccess.ReadSQL(qctCollection);

            DataRow QCTH = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable QCT1 = this.dbaccess.DataSet.Tables["QCT1"];
            DataTable QCT3 = this.dbaccess.DataSet.Tables["QCT3"];
            DataTable QCT4= this.dbaccess.DataSet.Tables["QCT4"];
            DataTable QCT5 = this.dbaccess.DataSet.Tables["QCT5"];
            DataTable QCT6 = this.dbaccess.DataSet.Tables["QCT6"];
            DataTable QCT7 = this.dbaccess.DataSet.Tables["QCT7"];
            DataTable QCT8 = this.dbaccess.DataSet.Tables["QCT8"];
            DataTable QCT9 = this.dbaccess.DataSet.Tables["QCT9"];
            DataTable QCT10 = this.dbaccess.DataSet.Tables["QCT10"];


            if(QCTH != null)
            {
                CTRH["ctrnum"] = QCTH["ctrnum"];
                CTRH["coy"] = QCTH["coy"];
                CTRH["coyname"] = QCTH["coyname"];
                CTRH["officerqty"] = QCTH["officerqty"];
                CTRH["discamt"] = QCTH["discamt"];
                CTRH["payid"] = QCTH["payid"];
                CTRH["duty"] = QCTH["duty"];
                CTRH["event"] = QCTH["event"];
                CTRH["schedule"] = QCTH["schedule"];
                CTRH["scheduleoption"] = QCTH["scheduleoption"];
                CTRH["isweekdays"] = QCTH["isweekdays"];
                CTRH["sinstruction"] = QCTH["sinstruction"];
                CTRH["created"] = QCTH["created"];
                CTRH["trandate"] = QCTH["trandate"];
                CTRH["modified"] = QCTH["modified"];
                CTRH["year"] = QCTH["year"];                
                CTRH["period"] = QCTH["period"];
                CTRH["flag"] = QCTH["flag"];
                CTRH["user"] = QCTH["user"];
                CTRH["guid"] = QCTH["guid"];
                CTRH["remark"] = QCTH["remark"];
                CTRH["contractdate"] = QCTH["contractdate"];
                CTRH["commencedate"] = QCTH["commencedate"];
                CTRH["enddate"] = QCTH["enddate"];
                CTRH["issuedby"] = QCTH["issuedby"];
                CTRH["ttlbillingamt"] = QCTH["ttlbillingamt"];
                CTRH["discount"] = QCTH["discount"];
                CTRH["subtotal"] = QCTH["subtotal"];
                CTRH["othercost"] = QCTH["othercost"];
                CTRH["createdby"] = QCTH["createdby"];
                CTRH["arnum"] = QCTH["arnum"];
                CTRH["additionalcosts"] = QCTH["additionalcosts"];
                CTRH["equipmentcosts"] = QCTH["equipmentcosts"];
                CTRH["cstarttime"] = QCTH["cstarttime"];
                CTRH["cendtime"] = QCTH["cendtime"];
                CTRH["cc1"] = QCTH["cc1"];
                CTRH["cc2"] = QCTH["cc2"];
                CTRH["payinfo"] = QCTH["payinfo"];
                CTRH["billadd4"] = QCTH["billadd4"];
                CTRH["oldrefnum"] = QCTH["oldrefnum"];
                CTRH["pctrnum"] = QCTH["pctrnum"];
                CTRH["totalmonths"] = QCTH["totalmonths"];
                CTRH["bgtestsal"] = QCTH["bgtestsal"];
                CTRH["bgtestmat"] = QCTH["bgtestmat"];
                CTRH["bgtestchem"] = QCTH["bgtestchem"];
                CTRH["bgtestmach"] = QCTH["bgtestmach"];
                CTRH["bgtestPrd"] = QCTH["bgtestPrd"];
                CTRH["bgtestLand"] = QCTH["bgtestLand"];
                CTRH["bgtestSub"] = QCTH["bgtestSub"];
                CTRH["bgtestOther"] = QCTH["bgtestOther"];
                CTRH["bgtestToi"] = QCTH["bgtestToi"];
                CTRH["bgtActMLand"] = QCTH["bgtActMLand"];
                CTRH["bgtActMSub"] = QCTH["bgtActMSub"];
                CTRH["bgtActMOther"] = QCTH["bgtActMOther"];
                CTRH["bgtsactmat"] = QCTH["bgtsactmat"];
                CTRH["bgtsactchem"] = QCTH["bgtsactchem"];
                CTRH["bgtsactmach"] = QCTH["bgtsactmach"];
                CTRH["bgtsactPrd"] = QCTH["bgtsactPrd"];
                CTRH["bgtsactLand"] = QCTH["bgtsactLand"];
                CTRH["bgtsactSub"] = QCTH["bgtsactSub"];
                CTRH["bgtsactOther"] = QCTH["bgtsactOther"];
                CTRH["bgtsactToi"] = QCTH["bgtsactToi"];
                CTRH["bgtsactsal"] = QCTH["bgtsactsal"];
                CTRH["bgtactsal"] = QCTH["bgtactsal"];
                CTRH["bgtactmat"] = QCTH["bgtactmat"];
                CTRH["bgtactchem"] = QCTH["bgtactchem"];
                CTRH["bgtactmach"] = QCTH["bgtactmach"];
                CTRH["bgtactToi"] = QCTH["bgtactToi"];
                CTRH["bgtactPrd"] = QCTH["bgtactPrd"];
                CTRH["bgtactLand"] = QCTH["bgtactLand"];
                CTRH["bgtactSub"] = QCTH["bgtactSub"];
                CTRH["bgtactOther"] = QCTH["bgtactOther"];
                CTRH["bgtlactsal"] = QCTH["bgtlactsal"];
                CTRH["bgtlactmat"] = QCTH["bgtlactmat"];
                CTRH["bgtlactchem"] = QCTH["bgtlactchem"];
                CTRH["bgtlactmach"] = QCTH["bgtlactmach"];
                CTRH["bgtlactToi"] = QCTH["bgtlactToi"];
                CTRH["bgtlactPrd"] = QCTH["bgtlactPrd"];
                CTRH["bgtlactLand"] = QCTH["bgtlactLand"];
                CTRH["bgtlactSub"] = QCTH["bgtlactSub"];
                CTRH["bgtlactOther"] = QCTH["bgtlactOther"];
                CTRH["bgtActMsal"] = QCTH["bgtActMsal"];
                CTRH["bgtActMmat"] = QCTH["bgtActMmat"];
                CTRH["bgtActMchem"] = QCTH["bgtActMchem"];
                CTRH["bgtActMmach"] = QCTH["bgtActMmach"];
                CTRH["bgtActMToi"] = QCTH["bgtActMToi"];
                CTRH["bgtActMPrd"] = QCTH["bgtActMPrd"];
                CTRH["bgtestequip"] = QCTH["bgtestequip"];
                CTRH["bgtActMequip"] = QCTH["bgtActMequip"];
                CTRH["bgtsactequip"] = QCTH["bgtsactequip"];
                CTRH["bgtactequip"] = QCTH["bgtactequip"];
                CTRH["bgtlactequip"] = QCTH["bgtlactequip"];
                CTRH["bgtestctr"] = QCTH["bgtestctr"];
                CTRH["bgtActMctr"] = QCTH["bgtActMctr"];
                CTRH["bgtsactctr"] = QCTH["bgtsactctr"];
                CTRH["bgtactctr"] = QCTH["bgtactctr"];
                CTRH["bgtlactctr"] = QCTH["bgtlactctr"];
                CTRH["tnc"] = QCTH["tnc"];
                CTRH["wkdaysmth"] = QCTH["wkdaysmth"];
                CTRH["bgtestctrldsp"] = QCTH["bgtestctrldsp"];
                CTRH["bgtestLandSub"] = QCTH["bgtestLandSub"];
                CTRH["bgtActMctrldsp"] = QCTH["bgtActMctrldsp"];
                CTRH["bgtActMLandSub"] = QCTH["bgtActMLandSub"];
                CTRH["bgtsactctrldsp"] = QCTH["bgtsactctrldsp"];
                CTRH["bgtsactLandSub"] = QCTH["bgtsactLandSub"];
                CTRH["bgtactctrldsp"] = QCTH["bgtactctrldsp"];
                CTRH["bgtactLandSub"] = QCTH["bgtactLandSub"];
                CTRH["bgtlactctrldsp"] = QCTH["bgtlactctrldsp"];
                CTRH["bgtlactLandSub"] = QCTH["bgtlactLandSub"];
                
            }



          

            if (QCT1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR1);

                foreach (DataRow dr1 in QCT1.Rows)
                {
                    DataRow InsertCTR1 = CTR1.NewRow();

                    InsertCTR1["matnum"] = dr1["matnum"];
                    InsertCTR1["estmamt"] = dr1["estmamt"];
                    InsertCTR1["actmamt"] = dr1["actmamt"];
                    InsertCTR1["officerqty"] = dr1["officerqty"];
                    InsertCTR1["mthnum"] = dr1["mthnum"];
                    InsertCTR1["rate"] = dr1["rate"];
                    InsertCTR1["oriamt"] = dr1["oriamt"];
                    InsertCTR1["hperd"] = dr1["hperd"];
                    InsertCTR1["dperw"] = dr1["dperw"];
                    InsertCTR1["wperm"] = dr1["wperm"];
                    InsertCTR1["hcost"] = dr1["hcost"];
                    InsertCTR1["xday1"] = dr1["xday1"];
                    InsertCTR1["xday2"] = dr1["xday2"];
                    InsertCTR1["xday3"] = dr1["xday3"];
                    InsertCTR1["xday4"] = dr1["xday4"];
                    InsertCTR1["xday5"] = dr1["xday5"];
                    InsertCTR1["xday6"] = dr1["xday6"];
                    InsertCTR1["xday7"] = dr1["xday7"];
                    InsertCTR1["perappoitamt"] = dr1["perappoitamt"];
                    InsertCTR1["ispubhol"] = dr1["ispubhol"];
                    InsertCTR1["perhr"] = dr1["perhr"];
                    InsertCTR1["remark"] = dr1["remark"];

                    CTR1.Rows.Add(InsertCTR1);
                }
            }

            if (QCT3.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR3);

                foreach (DataRow dr3 in QCT3.Rows)
                {
                    DataRow InsertCTR3 = CTR3.NewRow();

                    InsertCTR3["matnum"] = dr3["matnum"];
                    InsertCTR3["itemdesc"] = dr3["itemdesc"];
                    InsertCTR3["estmamt"] = dr3["estmamt"];
                    InsertCTR3["actmamt"] = dr3["actmamt"];
                    InsertCTR3["qty"] = dr3["qty"];
                    InsertCTR3["totalmths"] = dr3["totalmths"];
                    InsertCTR3["mthnum"] = dr3["mthnum"];
                    InsertCTR3["price"] = dr3["price"];
                    InsertCTR3["oriamt"] = dr3["oriamt"];
                    InsertCTR3["moriamt"] = dr3["moriamt"];
                    InsertCTR3["amount"] = dr3["amount"];
                    InsertCTR3["remark"] = dr3["remark"];
                   

                    CTR3.Rows.Add(InsertCTR3);
                }
            }

            if (QCT4.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR4);

                foreach (DataRow dr4 in QCT4.Rows)
                {

                    DataRow InsertCTR4 = CTR4.NewRow();

                    InsertCTR4["raisedby"] = dr4["raisedby"];
                    InsertCTR4["desc"] = dr4["desc"];
                    InsertCTR4["followup"] = dr4["followup"];                  

                    CTR4.Rows.Add(InsertCTR4);
                }
                   
            }

            if (QCT5.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR5);

                foreach (DataRow dr5 in QCT5.Rows)
                {
                    DataRow InsertCTR3 = CTR3.NewRow();

                    InsertCTR3["matnum"] = dr5["matnum"];
                    InsertCTR3["itemdesc"] = dr5["itemdesc"];
                    InsertCTR3["estmamt"] = dr5["estmamt"];
                    InsertCTR3["actmamt"] = dr5["actmamt"];
                    InsertCTR3["qty"] = dr5["qty"];
                    InsertCTR3["mthnum"] = dr5["mthnum"];
                    InsertCTR3["price"] = dr5["price"];
                    InsertCTR3["oriamt"] = dr5["oriamt"];
                    InsertCTR3["moriamt"] = dr5["moriamt"];
                    InsertCTR3["amount"] = dr5["amount"];
                    InsertCTR3["remark"] = dr5["remark"];
                   

                    CTR3.Rows.Add(InsertCTR3);
                }
            }

            if (QCT6.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR6);

                foreach (DataRow dr6 in QCT6.Rows)
                {
                    DataRow InsertCTR6 = CTR6.NewRow();

                    InsertCTR6["svccode"] = dr6["svccode"];
                    InsertCTR6["svcdesc"] = dr6["svcdesc"];
                    InsertCTR6["estmamt"] = dr6["estmamt"];
                    InsertCTR6["actmamt"] = dr6["actmamt"];
                    InsertCTR6["qty"] = dr6["qty"];
                    InsertCTR6["oriamt"] = dr6["oriamt"];
                    InsertCTR6["frequencycode"] = dr6["frequencycode"];
                    InsertCTR6["xmonth1"] = dr6["xmonth1"];
                    InsertCTR6["xmonth2"] = dr6["xmonth2"];
                    InsertCTR6["xmonth3"] = dr6["xmonth3"];
                    InsertCTR6["xmonth4"] = dr6["xmonth4"];
                    InsertCTR6["xmonth5"] = dr6["xmonth5"];
                    InsertCTR6["xmonth6"] = dr6["xmonth6"];
                    InsertCTR6["xmonth7"] = dr6["xmonth7"];
                    InsertCTR6["xmonth8"] = dr6["xmonth8"];
                    InsertCTR6["xmonth9"] = dr6["xmonth9"];
                    InsertCTR6["xmonth10"] = dr6["xmonth10"];
                    InsertCTR6["xmonth11"] = dr6["xmonth11"];
                    InsertCTR6["xmonth12"] = dr6["xmonth12"];
                    InsertCTR6["remark"] = dr6["remark"];
                    InsertCTR6["workscope"] = dr6["workscope"];
                    InsertCTR6["sitenum"] = dr6["sitenum"];

                    CTR6.Rows.Add(InsertCTR6);
                }
            }

            if (QCT7.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR7);

                foreach (DataRow dr7 in QCT7.Rows)
                {
                    DataRow InsertCTR7 = CTR7.NewRow();

                    InsertCTR7["matnum"] = dr7["matnum"];
                    InsertCTR7["itemdesc"] = dr7["itemdesc"];
                    InsertCTR7["estmamt"] = dr7["estmamt"];
                    InsertCTR7["actmamt"] = dr7["actmamt"];
                    InsertCTR7["qty"] = dr7["qty"];
                    InsertCTR7["mthnum"] = dr7["mthnum"];
                    InsertCTR7["price"] = dr7["price"];
                    InsertCTR7["oriamt"] = dr7["oriamt"];
                    InsertCTR7["moriamt"] = dr7["moriamt"];
                    InsertCTR7["amount"] = dr7["amount"];
                    InsertCTR7["remark"] = dr7["remark"];
                   

                    CTR7.Rows.Add(InsertCTR7);
                }
            }

            if (QCT8.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR8);

                foreach (DataRow dr8 in QCT8.Rows)
                {
                    DataRow InsertCTR8 = CTR8.NewRow();

                    InsertCTR8["matnum"] = dr8["matnum"];
                    InsertCTR8["itemdesc"] = dr8["itemdesc"];
                    InsertCTR8["estmamt"] = dr8["estmamt"];
                    InsertCTR8["actmamt"] = dr8["actmamt"];
                    InsertCTR8["qty"] = dr8["qty"];
                    InsertCTR8["mthnum"] = dr8["mthnum"];
                    InsertCTR8["price"] = dr8["price"];
                    InsertCTR8["oriamt"] = dr8["oriamt"];
                    InsertCTR8["moriamt"] = dr8["moriamt"];
                    InsertCTR8["amount"] = dr8["amount"];
                    InsertCTR8["remark"] = dr8["remark"];
                   

                    CTR8.Rows.Add(InsertCTR8);
                }
            }

            if (QCT9.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR9);

                foreach (DataRow dr9 in QCT9.Rows)
                {
                    DataRow InsertCTR9 = CTR9.NewRow();

                    InsertCTR9["matnum"] = dr9["matnum"];
                    InsertCTR9["itemdesc"] = dr9["itemdesc"];
                    InsertCTR9["estmamt"] = dr9["estmamt"];
                    InsertCTR9["actmamt"] = dr9["actmamt"];
                    InsertCTR9["qty"] = dr9["qty"];
                    InsertCTR9["mthnum"] = dr9["mthnum"];
                    InsertCTR9["price"] = dr9["price"];
                    InsertCTR9["oriamt"] = dr9["oriamt"];
                    InsertCTR9["moriamt"] = dr9["moriamt"];
                    InsertCTR9["amount"] = dr9["amount"];
                    InsertCTR9["remark"] = dr9["remark"];
                   
                    CTR9.Rows.Add(InsertCTR9);
                }
            }

            if (QCT10.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(CTR10);

                foreach (DataRow dr10 in QCT10.Rows)
                {
                    DataRow InsertCTR10 = CTR10.NewRow();

                    InsertCTR10["remark"] = dr10["remark"];
                    InsertCTR10["filedesc"] = dr10["filedesc"];
                    InsertCTR10["filename"] = dr10["filename"];
                    InsertCTR10["physicalserverlocation"] = dr10["physicalserverlocation"];
                    InsertCTR10["trackingno"] = dr10["trackingno"];
                    InsertCTR10["mappedDrivelocation"] = dr10["mappedDrivelocation"];
                    InsertCTR10["created"] = dr10["created"];

                    CTR10.Rows.Add(InsertCTR10);
                }
            }

            GetLatesArmInfo(qctNum);
            GetLatestSiteInfo(qctNum);


        }

        private void GetLatesArmInfo(string qctNum)
        {
            DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            string getArmLatest = "Select * from arm where qctnum='" + qctNum + "' and ISNULL(isPending,0)=0";
            this.dbaccess.ReadSQL("ArmLatest", getArmLatest);

            DataTable ArmLatest = this.dbaccess.DataSet.Tables["ArmLatest"];

            if (ArmLatest.Rows.Count > 0)
            {
                DataRow drArm = this.dbaccess.DataSet.Tables["ArmLatest"].Rows[0];

                CTRH["arnum"] = drArm["arnum"];
                CTRH["billadd1"] = drArm["baddr1"];
                CTRH["billadd2"] = drArm["baddr2"];
                CTRH["billadd3"] = drArm["baddr3"];
                CTRH["billadd4"] = drArm["baddr4"];

            }
        }

        private void GetLatestSiteInfo(string qctNum)
        {
            DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            string getSiteLatest = "Select * from sitm where qctnum='" + qctNum + "'";
            this.dbaccess.ReadSQL("SitmiLatest", getSiteLatest);

            DataTable SitmiLatest = this.dbaccess.DataSet.Tables["SitmiLatest"];

            if (SitmiLatest.Rows.Count > 0)
            {
                DataRow drSitmi = this.dbaccess.DataSet.Tables["SitmiLatest"].Rows[0];

                CTRH["sitenum"] = drSitmi["sitenum"];
                CTRH["sitename"] = drSitmi["sitename"];
                CTRH["sectorcode"] = drSitmi["sectorcode"];
                CTRH["sitename"] = drSitmi["sitename"];
                CTRH["addr1"] = drSitmi["addr1"];
                CTRH["addr2"] = drSitmi["addr2"];
                CTRH["addr3"] = drSitmi["addr3"];
                CTRH["country"] = drSitmi["country"];
                CTRH["postalcode"] = drSitmi["postalcode"];
                CTRH["rep1"] = drSitmi["rep1"];
                CTRH["email"] = drSitmi["email"];
                CTRH["sfax"] = drSitmi["fax"];
                CTRH["tel1"] = drSitmi["tel1"];
                CTRH["rep2"] = drSitmi["rep2"];
                CTRH["tel2"] = drSitmi["tel2"];
                CTRH["rep1tel"] = drSitmi["rep1tel"];
                CTRH["rep2tel"] = drSitmi["rep2tel"];
                CTRH["prmcode"] = drSitmi["prmcode"];

            }
        }

        #endregion

        private string[] GetZoneSupervisor(string sectorcode)
        {
            string getSup = "select s.opmgr,h.empname from sem s LEFT JOIN HEMPH h on s.opmgr=h.empnum "+
                            "where s.sectorcode='"+sectorcode.Trim()+"'";

            string[] arr1 = new string[2];

            this.dbaccess.ReadSQL("SupTB", getSup);

            DataTable SupTB = this.dbaccess.DataSet.Tables["SupTB"];

            if (SupTB != null)
            {
                if (SupTB.Rows.Count > 0)
                {
                    arr1[0] = this.dbaccess.DataSet.Tables["SupTB"].Rows[0]["opmgr"].ToString();
                    arr1[1] = this.dbaccess.DataSet.Tables["SupTB"].Rows[0]["empname"].ToString();
                }
            }
            return arr1;
        }

    }
        
}
    

