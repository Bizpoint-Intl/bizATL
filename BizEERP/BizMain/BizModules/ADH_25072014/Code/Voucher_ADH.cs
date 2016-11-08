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

namespace ATL.ADH
{
    public class Voucher_ADH : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName, budgetFormName, manpowerFormName, machineryFormName, equipmentcostsFormName, materialFormName, toiletriesFormName,
                         chemicalFormName, periodicFormName, archiveFormName, tacFormName, feedbackFormName, txt_guid = null;
        protected CheckBox adh_daily, adh_weekldays, adh_weekend = null;
        protected TextBox adh1_ttlamt, adh3_ttlamt, txt_ttlamt, adh5_ttlamt, adh6_ttlamt, adh7_ttlamt, adh8_ttlamt, adh9_ttlamt, adh_arnum,
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
        protected Button btn_Voucher_Reports, adh4_btnUpdate, adh4_btnAdd, adh10_btnBrowseEdu, adh10_btnDownloadEdu;
        protected DateTimePicker txt_Daterasied, adh_commencedate;
        protected bool opened, isMouseClicked, enableDocSave = false;
        string strFileName;
        bool headerFlag, budgetFlag, manpowerFlag, machineryFlag, equipmentFlag, materialFlag, toiletriesFlag, chemicalFlag, periodicFlag, archiveFlag, tncFlag, feedbackFlag = false;
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

        public Voucher_ADH(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_ADH.xml", moduleName, voucherBaseHelpers)
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
            DataRow ADH = e.DBAccess.DataSet.Tables["ADH"].Rows[0];
            string scheduleoption = ADH["scheduleoption"].ToString();

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


            if (Convert.IsDBNull(ADH["trandate"]))
            {
                ADH["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }

            Initialise();

            initiValues();

            #region ColumnChanged Events
            e.DBAccess.DataSet.Tables["ADH"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH_ColumnChanged);
            e.DBAccess.DataSet.Tables["ADH1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH1_ColumnChanged);
            e.DBAccess.DataSet.Tables["ADH3"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH3_ColumnChanged);
            e.DBAccess.DataSet.Tables["ADH5"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH5_ColumnChanged);
            e.DBAccess.DataSet.Tables["ADH6"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH6_ColumnChanged);
            e.DBAccess.DataSet.Tables["ADH7"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH7_ColumnChanged);
            e.DBAccess.DataSet.Tables["ADH8"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH8_ColumnChanged);
            e.DBAccess.DataSet.Tables["ADH9"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ADH9_ColumnChanged);
            #endregion


            string GetShiftInfo = "Select * from vShlv";

            this.dbaccess.ReadSQL("vSHLV", GetShiftInfo);

            opened = true;

            if (BizFunctions.IsEmpty(ADH["tnc"]))
            {
                string varBinary = "0x504B0304140002000800976C904258470FF02F020000B20400000B000000636F6E74656E742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB767B5A2D5BFA99D2CBCBE651D35E9739B5AE978FAAAC299A47CB6C91378FDAE9A36A952FCD4B8FBCC68FB823F9A0CDDFB5B77D196DFD77CFABDBBEF9AE29B7CF2B427CB1CADA6212A2D05C5EDC160E351D0032ABB3ABDB42415BA2B0FFFABBB258BEFDECA379DBAE1EDDBD7B757535BEBA37AEEA8BBBBB0F1F3EBCCBDF9AA65575651B02B8CC0E37DEDBD9D9BF7B55176D5E5BFA6693DBCF0E37F6D112D8B77D5DF9C4A7CAD4A2BA5AD72523399BDECDCB1CAF357777C7BB773FB20C764E7CB57D9E4DF3ED593E2D9BF4AEFD265BB7D582883EDD66066A8E1E0B23F1BFA9FC0ECC3EFBE835F52D7F9F678BA2BCFEECA3269FB6C4DE1FD977E4EFED554D43A8DB22E78EEE7A0087A1BFDCED425F6575765167ABB9F9823E808CF01FDBF2D68BAA5E64A569B0C81A9A9FED557661BE7FDD66CB5956CF3E62440687AC5F4CAAD9B5FD032271F4980543C795CA1F5EF720097FA87F4ABB9D8F8E1EF3A7ABFE1B344C03749545203ECDCFB375D9A62FCDE0D36734751F1D9D2E56EDF5E3BBF6CD23FD7D657E511CDD28057FF3970CCDFCD5D53747FF0F504B0304140002000800976C9042D5F05C2BBA020000F80500000A0000007374796C65732E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB76BB69AFCBBC49E9DD65F388FFA0C6F5F251953545F368992DF2E6513B7D54ADF2A579E791D7F811F7231F9C57B77DF55D536E9F57DBD36AB1CADA621282795716CBB79F7D346FDBD5A3BB77AFAEAEC657F7C6557D7177F7E1C38777F95BD3B4B9BCB86D97D474A03FA1C86DE128FDF0BE25E67945843CCFA6F9F62C9F964D7AD77E23D43D7A2CB4E27F53F91D7D7CF6D1D3FC3C5B976DFA32ABB38B3A5BCDD36704EB236D739E2D8AF2FAB38FDAFC1D7D767718CCF36299A72FD68B495EC7DF95CF56596DA77C7B33021B3AFBF6F52AAF6516225D99F7F0C7F6AA260AD66D410C765E3D9A5665557FF6D193729D9B57B9D57A396380B960F6D9474D5516B3812657C5AC9D7FF651B66E2BC6F2AE87E630CE2FAA7A91955D845766D00C091D3D421F4B2624890D31CCF2BCB858D7C434D532E506F2E536DA359F7D749E950D0D86BFA1296FF2F6B38F76C6F7A70BFD8C5A13A353DFF4F9AEFB0C10565553B42CAA657EDEEA77C5725AE7E0360263F8135FDC823B6D5BE16D90A6C384FA274847081553957D43B55576916F97D975B56E03DA7DB15AECD869F51A756697BFD1D93918DF2F961FD94FE7797131070576F1A9022202B7DB555DD06033A1C3AAAADB3A2B5A7E7191D517C572BBAD56A0E883834F0F14A27E31A95A1A46FC3B10141A70F760E7D3F09B5A10A1297AB8730F5F390EF246E648D7A7957EB1C89A962631A4A07E084801055FB7D97296D596A57D227A34F6E7AC03DF7CDC51DB47FF0F504B0304140002000800976C90425CC0E1810501000000010000150000004D4554412D494E462F6D616E69666573742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E8F1225B16E779D33E32BFA4F4DAB2B17F52D37AF9A8CA9AA279B4CC1679F3A89D3EAA56F972564DD78B7CD93E0ADB3F42471ED8F3A2CCB7A9597D9DBAAEF259916DB7D7ABFCB38FB2D5AA2CA6594B48DEBD5CCEC6DCD3D8EF60DCE6EFDA8FDCDBE7EBB2DC5E65EDFCB38FEE7E94DE3D7A7CB73786A3FF07504B01021400140002000800976C904258470FF02F020000B20400000B0000000000000000000000000000000000636F6E74656E742E786D6C504B01021400140002000800976C9042D5F05C2BBA020000F80500000A00000000000000000000000000580200007374796C65732E786D6C504B01021400140002000800976C90425CC0E181050100000001000015000000000000000000000000003A0500004D4554412D494E462F6D616E69666573742E786D6C504B05060000000003000300B4000000720600000000";
                char[] charArray = varBinary.ToCharArray();
                byte[] byteArray = new byte[charArray.Length];

                ADH["tnc"] = byteArray;
            }

            enableDocSave = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("EnableDocSave"));

        }


        private void btn_Voucher_Reports_Click(object sender, EventArgs e)
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            if (adh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO)
            {
                if (BizValidate.CheckRowState(dbaccess.DataSet, "adh/adh1/adh3"))
                {

                    ReportLists.Reports ReportForm = new ATL.ReportLists.Reports(false, "ADH", "ADH", adh["refnum"].ToString());

                    ReportForm.ShowDialog();
                }
            }
        }

        private void Initialise()
        {
            DataTable xadh10 = this.dbaccess.DataSet.Tables["adh10"].Copy();

            if (!this.dbaccess.DataSet.Tables.Contains("xadh10"))
            {
                xadh10.TableName = "xadh10";
                this.dbaccess.DataSet.Tables.Add(xadh10.Copy());
            }

            adh_commencedate = BizXmlReader.CurrentInstance.GetControl(headerFormName, "adh_commencedate") as DateTimePicker;
            adh_commencedate.TextChanged += new EventHandler(adh_commencedate_TextChanged);

            adh1_ttlamt = BizXmlReader.CurrentInstance.GetControl(manpowerFormName, "adh1_ttlamt") as TextBox;
            adh3_ttlamt = BizXmlReader.CurrentInstance.GetControl(equipmentcostsFormName, "adh3_ttlamt") as TextBox;
            adh5_ttlamt = BizXmlReader.CurrentInstance.GetControl(materialFormName, "adh5_ttlamt") as TextBox;
            adh6_ttlamt = BizXmlReader.CurrentInstance.GetControl(periodicFormName, "adh6_ttlamt") as TextBox;
            adh7_ttlamt = BizXmlReader.CurrentInstance.GetControl(machineryFormName, "adh7_ttlamt") as TextBox;
            adh8_ttlamt = BizXmlReader.CurrentInstance.GetControl(toiletriesFormName, "adh8_ttlamt") as TextBox;
            adh9_ttlamt = BizXmlReader.CurrentInstance.GetControl(chemicalFormName, "adh9_ttlamt") as TextBox;
            adh_arnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "adh_arnum") as TextBox;

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


            adh4_btnAdd = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "adh4_btnAdd") as Button;
            adh4_btnAdd.Click += new EventHandler(adh4_btnAdd_Click);
            adh4_btnUpdate = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "adh4_btnUpdate") as Button;
            adh4_btnUpdate.Click += new EventHandler(adh4_btnUpdate_Click);

            dgCtr4 = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "dg_feedback") as DataGrid;
            dgCtr4.MouseDoubleClick += new MouseEventHandler(dgtxt_MouseDoubleClick);

            adh10_btnBrowseEdu = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "adh10_btnBrowseEdu") as Button;
            adh10_btnBrowseEdu.Click += new EventHandler(adh10_btnBrowseEdu_Click);

            adh10_btnDownloadEdu = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "adh10_btnDownloadEdu") as Button;
            adh10_btnDownloadEdu.Click += new EventHandler(adh10_btnDownloadEdu_Click);

            BtnTerms = BizXmlReader.CurrentInstance.GetControl(headerFormName, "BtnTerms") as Button;
            BtnTerms.Click += new EventHandler(BtnTerms_Click);

            GetManPowerAmt();
            GetMaterialCost();
            GetChemicalCost();
            GetMachineryCost();
            GetToiletryCost();
            GetEquipmentCost();
            GetPeriodScheduleCost();
        }




        #endregion

        void adh_commencedate_TextChanged(object sender, EventArgs e)
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            if (!refnumExist(adh["refnum"].ToString()))
            {
                if (!BizFunctions.IsEmpty(adh["commencedate"]))
                {
                    adh["enddate"] = Convert.ToDateTime(adh["commencedate"]).AddYears(1);
                }
            }
        }

        private bool refnumExist(string refnum)
        {
            bool exist = false;

            string check = "Select refnum from adh where refnum='" + refnum + "'";

            this.dbaccess.ReadSQL("checkADHRefnum", check);

            DataTable checkADHRefnum = this.dbaccess.DataSet.Tables["checkADHRefnum"];

            if (checkADHRefnum != null)
            {
                if (checkADHRefnum.Rows.Count > 0)
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
                string[] files = Directory.GetFiles(ServerProjDir, "*.jpg", SearchOption.TopDirectoryOnly);
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
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];
            DataTable adh1 = this.dbaccess.DataSet.Tables["adh1"];
            DataTable ADH3 = this.dbaccess.DataSet.Tables["ADH3"];
            DataTable ADH4 = this.dbaccess.DataSet.Tables["ADH4"];
            DataTable ADH5 = this.dbaccess.DataSet.Tables["ADH5"];
            DataTable ADH6 = this.dbaccess.DataSet.Tables["ADH6"];
            DataTable ADH7 = this.dbaccess.DataSet.Tables["ADH7"];
            DataTable ADH8 = this.dbaccess.DataSet.Tables["ADH8"];
            DataTable ADH9 = this.dbaccess.DataSet.Tables["ADH9"];
            DataTable adh10 = this.dbaccess.DataSet.Tables["adh10"];

        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            DataRow adh = dbaccess.DataSet.Tables["adh"].Rows[0];
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "adh_arnum":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);

                    if (adh_arnum.Text != string.Empty)
                    {
                        //e.DefaultCondition = "(arm.arnum like '%" + adh["arnum"].ToString().Trim() + "%' OR arm.arname like '" + adh["arnum"].ToString().Trim() + "%') and arm.[status]<>'V'";
                        e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);

                    }
                    break;

                case "adh_sitenum":
                    if (!BizFunctions.IsEmpty(adh["arnum"]))
                    {
                        e.Condition = BizFunctions.F2Condition("sitenum,sitename", (sender as TextBox).Text);
                        //e.DefaultCondition = "(sitm.sitenum like '" + adh["sitenum"].ToString() + "%' OR sitm.sitename like '" + adh["sitenum"].ToString() + "%') and sitm.[status]<>'V'";
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
            DataRow adh = dbaccess.DataSet.Tables["adh"].Rows[0];
            switch (e.ControlName)
            {
                // later...
                case "adh_sitenum":
                    if (!BizFunctions.IsEmpty(adh["sitenum"].ToString()))
                    {
                        string GetSitenumI = "Select * from sitm where sitenum='" + adh["sitenum"] + "'";
                        this.dbaccess.ReadSQL("SITM", GetSitenumI);
                        DataTable sitm = this.dbaccess.DataSet.Tables["SITM"];
                        if (sitm.Rows.Count > 0)
                        {
                            DataRow drSitmi = sitm.Rows[0];
                            adh["sitename"] = drSitmi["sitename"];
                            adh["sectorcode"] = drSitmi["sectorcode"];
                            adh["addr1"] = drSitmi["addr1"];
                            adh["addr2"] = drSitmi["addr2"];
                            adh["addr3"] = drSitmi["addr3"];
                            adh["country"] = drSitmi["country"];
                            adh["postalcode"] = drSitmi["postalcode"];
                            adh["billadd1"] = drSitmi["billadd1"];
                            adh["billadd2"] = drSitmi["billadd2"];
                            adh["billadd3"] = drSitmi["billadd3"];
                            adh["rep1"] = drSitmi["rep1"];
                            adh["tel1"] = drSitmi["tel1"];
                            adh["fax"] = drSitmi["fax"];
                            adh["rep2"] = drSitmi["rep2"];
                            adh["tel2"] = drSitmi["tel2"];
                            adh["rep1tel"] = drSitmi["rep1tel"];
                            adh["rep2tel"] = drSitmi["rep2tel"];
                            adh["prmcode"] = drSitmi["prmcode"];

                        }
                    }
                    break;

                case "adh_arnum":
                    {
                        adh["arname"] = e.F2CurrentRow["arname"];
                        adh["billadd1"] = e.F2CurrentRow["baddr1"];
                        adh["billadd2"] = e.F2CurrentRow["baddr2"];
                        adh["billadd3"] = e.F2CurrentRow["baddr3"];
                        adh["billadd4"] = e.F2CurrentRow["baddr4"];
                    }
                    break;

                case "adh_qctnum":
                    {
                        if (!BizFunctions.IsEmpty(adh["qctnum"].ToString()))
                        {
                            GetQuotation(adh["qctnum"].ToString());
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
            DataRow adh = dbaccess.DataSet.Tables["adh"].Rows[0];
            DataTable adh1 = dbaccess.DataSet.Tables["adh1"];

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
                        if (e.TableName != "adh1")
                        {
                            e.CurrentRow["estmamt"] = GetLatestMatnumCost(e.F2CurrentRow["matnum"].ToString());
                        }

                    }
                    break;

                case "frequencycode":
                    {
                        if (e.TableName == "adh6")
                        {
                            e.CurrentRow["qty"] = e.F2CurrentRow["qty"];
                        }

                    }
                    break;

                case "svccode":
                    if (e.TableName == "adh6")
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
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            adh["refnum"] = System.DBNull.Value;
        }

        protected override void Document_Paste_OnClick(object sender, BizRAD.BizDocument.DocumentEventArgs e)
        {
            base.Document_Paste_OnClick(sender, e);
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable ADH1 = this.dbaccess.DataSet.Tables["adh1"];
            DataTable ADH3 = this.dbaccess.DataSet.Tables["ADH3"];
            DataTable ADH4 = this.dbaccess.DataSet.Tables["ADH4"];
            DataTable ADH5 = this.dbaccess.DataSet.Tables["ADH5"];
            DataTable ADH6 = this.dbaccess.DataSet.Tables["ADH6"];
            DataTable ADH7 = this.dbaccess.DataSet.Tables["ADH7"];
            DataTable ADH8 = this.dbaccess.DataSet.Tables["ADH8"];
            DataTable ADH9 = this.dbaccess.DataSet.Tables["ADH9"];
            DataTable ADH10 = this.dbaccess.DataSet.Tables["adh10"];

            adh["refnum"] = System.DBNull.Value;
            adh["createdby"] = System.DBNull.Value;
            adh["issuedby"] = System.DBNull.Value;

            if (ADH1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ADH1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["refnum"] = System.DBNull.Value;
                    }
                }
            }

            if (ADH3.Rows.Count > 0)
            {
                foreach (DataRow dr3 in ADH3.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        dr3["refnum"] = System.DBNull.Value;
                    }
                }
            }

            if (ADH4.Rows.Count > 0)
            {
                foreach (DataRow dr4 in ADH4.Rows)
                {
                    if (dr4.RowState != DataRowState.Deleted)
                    {
                        dr4["refnum"] = System.DBNull.Value;
                    }
                }
            }

            if (ADH5.Rows.Count > 0)
            {
                foreach (DataRow dr5 in ADH5.Rows)
                {
                    if (dr5.RowState != DataRowState.Deleted)
                    {
                        dr5["refnum"] = System.DBNull.Value;
                    }
                }
            }


            if (ADH6.Rows.Count > 0)
            {
                foreach (DataRow dr6 in ADH6.Rows)
                {
                    if (dr6.RowState != DataRowState.Deleted)
                    {
                        dr6["refnum"] = System.DBNull.Value;
                    }
                }
            }


            if (ADH7.Rows.Count > 0)
            {
                foreach (DataRow dr7 in ADH7.Rows)
                {
                    if (dr7.RowState != DataRowState.Deleted)
                    {
                        dr7["refnum"] = System.DBNull.Value;
                    }
                }
            }


            if (ADH8.Rows.Count > 0)
            {
                foreach (DataRow dr8 in ADH8.Rows)
                {
                    if (dr8.RowState != DataRowState.Deleted)
                    {
                        dr8["refnum"] = System.DBNull.Value;
                    }
                }
            }


            if (ADH9.Rows.Count > 0)
            {
                foreach (DataRow dr9 in ADH9.Rows)
                {
                    if (dr9.RowState != DataRowState.Deleted)
                    {
                        dr9["refnum"] = System.DBNull.Value;
                    }
                }
            }


            if (ADH10.Rows.Count > 0)
            {
                foreach (DataRow dr10 in ADH10.Rows)
                {
                    if (dr10.RowState != DataRowState.Deleted)
                    {
                        dr10["refnum"] = System.DBNull.Value;
                    }
                }
            }
        }
        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];
            DataTable adh1 = this.dbaccess.DataSet.Tables["adh1"];
            DataTable ADH3 = this.dbaccess.DataSet.Tables["ADH3"];
            DataTable ADH4 = this.dbaccess.DataSet.Tables["ADH4"];
            DataTable ADH5 = this.dbaccess.DataSet.Tables["ADH5"];
            DataTable ADH6 = this.dbaccess.DataSet.Tables["ADH6"];
            DataTable ADH7 = this.dbaccess.DataSet.Tables["ADH7"];
            DataTable ADH8 = this.dbaccess.DataSet.Tables["ADH8"];
            DataTable ADH9 = this.dbaccess.DataSet.Tables["ADH9"];
            DataTable adh10 = this.dbaccess.DataSet.Tables["adh10"];

            foreach (DataRow dr1 in adh1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr1, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr3 in ADH3.Rows)
            {
                if (dr3.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr3, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr4 in ADH4.Rows)
            {
                if (dr4.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr4, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr5 in ADH5.Rows)
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr5, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr6 in ADH6.Rows)
            {
                if (dr6.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr6, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr7 in ADH7.Rows)
            {
                if (dr7.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr7, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr8 in ADH8.Rows)
            {
                if (dr8.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr8, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr9 in ADH9.Rows)
            {
                if (dr9.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr9, "refnum/user/flag/status/created/modified");
                }
            }
            foreach (DataRow dr10 in adh10.Rows)
            {
                if (dr10.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(adh, dr10, "refnum/user/flag/status/created/modified");
                }
            }


            DataTable xadh10 = adh10.GetChanges(DataRowState.Deleted);

            if (BizFunctions.IsEmpty(adh["trandate"]))
            {
                adh["trandate"] = DateTime.Now;
            }


            #region Personal Files/Docs Record
            if (enableDocSave)
            {

                if (adh10.Rows.Count > 0)
                {
                    try
                    {

                        string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("ASOMSDocsRepository"));

                        if (DriveLetter.Trim() != "")
                        {

                            foreach (DataRow dr11 in adh10.Rows)
                            {

                                if (dr11.RowState != DataRowState.Deleted)
                                {
                                    BizFunctions.UpdateDataRow(adh, dr11, "refnum/user/flag/status/created/modified");

                                    if (BizFunctions.IsEmpty(dr11["physicalserverlocation"]))
                                    {
                                        FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), adh["arnum"].ToString(), Convert.ToDateTime(adh["commencedate"]), dr11["flname"].ToString());
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
                                            FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), adh["arnum"].ToString(), Convert.ToDateTime(adh["commencedate"]), dr11["flname"].ToString());
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
                            MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (xadh10 != null)
                {

                    if (this.dbaccess.DataSet.Tables.Contains("xadh10"))
                    {
                        this.dbaccess.DataSet.Tables.Remove("xadh10");
                        xadh10 = adh10.GetChanges(DataRowState.Deleted);

                        xadh10.TableName = "xadh10";

                        this.dbaccess.DataSet.Tables.Add(xadh10);
                    }
                    else
                    {
                        xadh10.TableName = "xadh10";

                        this.dbaccess.DataSet.Tables.Add(xadh10);
                    }
                }



                if (adh["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSV)
                {
                    if (!BizFunctions.IsEmpty(adh["tnc"]))
                    {
                        WordForm1 wf = new WordForm1(this.dbaccess, "ADH", "tnc", "ADH");

                        MemoryStream ms = new MemoryStream();

                        if (!wf.SaveToFile())
                        {
                            MessageBox.Show("Unable to Save Service Agreement Document, Please try again later.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            adh["tncLoc"] = wf.FileInServerLocation;
                        }
                    }
                }
            }


        }
        #endregion

        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataTable xadh10 = this.dbaccess.DataSet.Tables["xadh10"];

            #region  Make Save Changes in Education Doc Files
            if (enableDocSave)
            {
                if (xadh10 != null)
                {
                    try
                    {

                        string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("ASOMSDocsRepository"));

                        if (DriveLetter1.Trim() != "")
                        {
                            if (xadh10.Rows.Count > 0)
                            {

                                foreach (DataRow dr1 in xadh10.Rows)
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
                        BizFunctions.DeleteAllRows(xadh10);

                        if (this.dbaccess.DataSet.Tables.Contains("xadh10"))
                        {
                            this.dbaccess.DataSet.Tables.Remove("xadh10");
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
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];

            #region Refresh IF Flag value=True

            Refresh_Header();

            Refresh_Manpower();

            Refresh_Machinery();

            Refresh_Equipment();

            Refresh_Material();

            Refresh_Toiletries();

            Refresh_Chemical();

            Refresh_Schedule();

            Refresh_Budget();


            Refresh_Budget();

            #endregion

            if (!BizFunctions.IsEmpty(adh["sectorcode"]))
            {
                string[] arr1 = new string[2];

                arr1 = GetZoneSupervisor(adh["sectorcode"].ToString());

                adh["empnum"] = arr1[0];
                adh["empname"] = arr1[1];

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

            DataRow adh = dbaccess.DataSet.Tables["adh"].Rows[0];
            if (adh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "adh/adh1"))
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

            DataRow adh = e.DBAccess.DataSet.Tables["adh"].Rows[0];
            Hashtable selectedCollection = new Hashtable();

            switch (e.ReportName)
            {

                case "Contract Form 1":
                    e.DataSource = adhds1();
                    break;

                case "Contract Form 2":
                    e.DataSource = adhds1();
                    break;

            }

        }

        #endregion

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow adh = dbaccess.DataSet.Tables["adh"].Rows[0];
            if (adh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "adh"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion

        #region Vouchers ColumnChanged Events
        private void Voucher_ADH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow ADH = this.dbaccess.DataSet.Tables["ADH"].Rows[0];

            switch (e.Column.ColumnName)
            {

                case "adhnum":
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
        private void Voucher_ADH1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable adh1 = dbaccess.DataSet.Tables["adh1"];
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

            }
        }
        #endregion

        #region Equipment
        private void Voucher_ADH3_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable adh3 = dbaccess.DataSet.Tables["adh3"];
            switch (e.Column.ColumnName)
            {
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
        private void Voucher_ADH5_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
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

        private void Voucher_ADH6_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable adh6 = dbaccess.DataSet.Tables["adh6"];
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
                    }
                    break;

            }
        }

        #region Machinery
        private void Voucher_ADH7_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
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
        private void Voucher_ADH8_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
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
        private void Voucher_ADH9_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
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
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh1 = dbaccess.DataSet.Tables["adh1"];
            foreach (DataRow dr1 in adh1.Rows)
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
            adh["bgtsactsal"] = totalamt;
            adh1_ttlamt.Text = totalamt.ToString();
        }

        private void GetMaterialCost()
        {
            decimal totalamt = 0;
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh5 = dbaccess.DataSet.Tables["adh5"];
            foreach (DataRow dr1 in adh5.Rows)
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
            adh["bgtsactmat"] = totalamt;
            adh5_ttlamt.Text = totalamt.ToString();
        }

        private void GetChemicalCost()
        {
            decimal totalamt = 0;
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh9 = dbaccess.DataSet.Tables["adh9"];
            if (adh9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
                adh["bgtsactchem"] = totalamt;
                adh9_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetMachineryCost()
        {
            decimal totalamt = 0;
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh7 = dbaccess.DataSet.Tables["adh7"];
            if (adh7.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh7.Rows)
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
                adh["bgtsactmach"] = totalamt;
                adh7_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetToiletryCost()
        {
            decimal totalamt = 0;
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh8 = dbaccess.DataSet.Tables["adh8"];
            if (adh8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
                adh["bgtsactToi"] = totalamt;
                adh8_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetEquipmentCost()
        {
            decimal totalamt = 0;
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh3 = dbaccess.DataSet.Tables["adh3"];
            if (adh3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh3.Rows)
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
                adh["bgtsactequip"] = totalamt;
                adh3_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetPeriodScheduleCost()
        {
            decimal totalamt = 0;
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh6 = dbaccess.DataSet.Tables["adh6"];
            if (adh6.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh6.Rows)
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
                adh["bgtsactPrd"] = totalamt;
                adh6_ttlamt.Text = totalamt.ToString();
            }
        }

        private decimal GetTotalMatnumCost(DataTable dt, string columnname)
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

            string GetAmout = "SELECT " +
                                    "retail " +
                                "FROM " +
                                "( " +
                                "select  " +
                                    "retail, " +
                                    "ROW_NUMBER() OVER (Order BY effectivedate) as ForTop,ROW_NUMBER() OVER (Order BY effectivedate Desc) as ForBottom  " +
                                "from matm1  " +
                                "where effectivedate<=GETDATE() " +
                                "and matnum ='" + matnum + "'  " +
                                ")A  " +
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
            DataRow ADH = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh1 = dbaccess.DataSet.Tables["adh1"];
            string sqlCount = "select SUM(officerqty) as Oqty from [adh1]";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, sqlCount);
            DataRow dr1 = dt1.Rows[0];

            if (dr1["Oqty"] != System.DBNull.Value)
            {
                ADH["officerqty"] = dr1["Oqty"];
            }
            else
            {
                ADH["officerqty"] = 0;
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

        private DataSet adhds1()
        {
            DataSet ds1 = new DataSet("ADHds1");
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];
            DataTable adh1 = this.dbaccess.DataSet.Tables["adh1"];
            DataTable adh3 = this.dbaccess.DataSet.Tables["adh1"];


            DataTable vSHLV = this.dbaccess.DataSet.Tables["vshlv"];


            string GetADH1 = " SELECT " +
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
                                            "from ADH1 group by matnum, monday,tuesday,wednesday,thursday,friday,saturday,sunday,ispubhol " +
                                            ")A1 " +
                                        "group by A1.matnum, A1.monday,A1.tuesday,A1.wednesday,A1.thursday,A1.friday,A1.saturday,A1.sunday,A1.ispubhol " +
                                   ")A2 " +
                                "LEFT JOIN " +
                                "( " +
                                "SELECT matnum,MAX(officerqty) as officerqty from " +
                                "ADH1  " +
                                "GROUP BY matnum " +
                                ") " +
                                "B ON A2.MATNUM=B.MATNUM";

            DataTable ADH = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM ADH");
            DataTable ADH1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetADH1);
            DataTable ADH12 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select matnum,officerqty,shiftcode,monday,tuesday,wednesday,thursday,friday,saturday,sunday,ispubhol from ADH1");

            ADH12.Columns.Add("TimeDetails", typeof(string));

            if (ADH12.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ADH12.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["timedetails"] = GetTimeDetails(dr1["shiftcode"].ToString());
                    }
                }
            }


            DataTable ADH3 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM ADH3");

            dbaccess.ReadSQL("MATMtmp", "SELECT * FROM MATM");

            DataTable MATM1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM MATMtmp");

            //string getARM = "SELECT * FROM ARM WHERE arnum='" + adh["arnum"].ToString() + "'";

            //this.dbaccess.ReadSQL("ARMtmp", getARM);

            //DataTable ARMtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getARM);


            //////////////////////////////////////

            string getARM = "SELECT * FROM ARM WHERE arnum='" + adh["arnum"].ToString() + "'";
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


            if (ds1.Tables.Contains("ADH"))
            {
                ds1.Tables["ADH"].Dispose();
                ds1.Tables.Remove("ADH");
                ADH.TableName = "ADH";
                ds1.Tables.Add(ADH);
            }
            else
            {
                ADH.TableName = "ADH";
                ds1.Tables.Add(ADH);
            }

            if (ds1.Tables.Contains("ADH1"))
            {
                ds1.Tables["ADH1"].Dispose();
                ds1.Tables.Remove("ADH1");
                ADH1.TableName = "ADH1";
                ds1.Tables.Add(ADH1);
            }
            else
            {
                ADH1.TableName = "ADH1";
                ds1.Tables.Add(ADH1);
            }

            if (ds1.Tables.Contains("ADH12"))
            {
                ds1.Tables["ADH12"].Dispose();
                ds1.Tables.Remove("ADH12");
                ADH12.TableName = "ADH12";
                ds1.Tables.Add(ADH12);
            }
            else
            {
                ADH12.TableName = "ADH12";
                ds1.Tables.Add(ADH12);
            }

            if (ds1.Tables.Contains("ADH3"))
            {
                ds1.Tables["ADH3"].Dispose();
                ds1.Tables.Remove("ADH3");
                ADH3.TableName = "ADH3";
                ds1.Tables.Add(ADH3);
            }
            else
            {
                ADH3.TableName = "ADH3";
                ds1.Tables.Add(ADH3);
            }


            return ds1;

        }

        #region Refresh Functions

        #region Refresh Header

        private void Refresh_Header()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            if (BizFunctions.IsEmpty(adh["commencedate"]) || BizFunctions.IsEmpty(adh["enddate"]))
            {
                adh["commencedate"] = DateTime.Now;
                adh["enddate"] = DateTime.Now;
            }
            adh["totalmonths"] = ATL.TimeUtilites.TimeTools.MonthDiff(Convert.ToDateTime(adh["commencedate"]), Convert.ToDateTime(adh["enddate"]));
            RefreshMonthNo();
            headerFlag = false;
        }

        #endregion

        #region Refresh Budget

        private void Refresh_Budget()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh1 = this.dbaccess.DataSet.Tables["ADH1"];
            DataTable adh3 = this.dbaccess.DataSet.Tables["ADH3"];
            DataTable adh5 = this.dbaccess.DataSet.Tables["ADH5"];
            DataTable adh6 = this.dbaccess.DataSet.Tables["ADH6"];
            DataTable adh7 = this.dbaccess.DataSet.Tables["ADH7"];
            DataTable adh8 = this.dbaccess.DataSet.Tables["ADH8"];
            DataTable adh9 = this.dbaccess.DataSet.Tables["ADH9"];


            #region Manual Actual
            if (Convert.ToDecimal(adh["bgtActMctr"]) <= 0)
            {
                adh["bgtActMctr"] = adh["bgtestctr"];
            }
            if (Convert.ToDecimal(adh["bgtActMctrldsp"]) <= 0)
            {
                adh["bgtActMctrldsp"] = adh["bgtestctrldsp"];
            }
            if (Convert.ToDecimal(adh["bgtActMLandSub"]) <= 0)
            {
                adh["bgtActMLandSub"] = adh["bgtestLandSub"];
            }
            if (Convert.ToDecimal(adh["bgtActMsal"]) <= 0)
            {
                adh["bgtActMsal"] = adh["bgtestsal"];
            }
            if (Convert.ToDecimal(adh["bgtActMmat"]) <= 0)
            {
                adh["bgtActMmat"] = adh["bgtestmat"];
            }
            if (Convert.ToDecimal(adh["bgtActMchem"]) <= 0)
            {
                adh["bgtActMchem"] = adh["bgtestchem"];
            }
            if (Convert.ToDecimal(adh["bgtActMmach"]) <= 0)
            {
                adh["bgtActMmach"] = adh["bgtestmach"];
            }
            if (Convert.ToDecimal(adh["bgtActMequip"]) <= 0)
            {
                adh["bgtActMequip"] = adh["bgtestequip"];
            }
            if (Convert.ToDecimal(adh["bgtActMToi"]) <= 0)
            {
                adh["bgtActMToi"] = adh["bgtestToi"];
            }
            if (Convert.ToDecimal(adh["bgtActMPrd"]) <= 0)
            {
                adh["bgtActMPrd"] = adh["bgtestPrd"];
            }
            if (Convert.ToDecimal(adh["bgtActMLand"]) <= 0)
            {
                adh["bgtActMLand"] = adh["bgtestLand"];
            }
            if (Convert.ToDecimal(adh["bgtActMSub"]) <= 0)
            {
                adh["bgtActMSub"] = adh["bgtestSub"];
            }
            if (Convert.ToDecimal(adh["bgtActMOther"]) <= 0)
            {
                adh["bgtActMOther"] = adh["bgtEstOther"];
            }
            #endregion End Manual Actual

            #region System Actual
            //if (Convert.ToDecimal(adh["bgtsactctr"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMctr"]) > 0)
            {
                adh["bgtsactctr"] = adh["bgtActMctr"];
            }
            else
            {
                adh["bgtsactctr"] = adh["bgtestctr"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtsactctrldsp"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMctrldsp"]) > 0)
            {
                adh["bgtsactctrldsp"] = adh["bgtActMctrldsp"];
            }
            else
            {
                adh["bgtsactctrldsp"] = adh["bgtestctrldsp"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtsactLandSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLandSub"]) > 0)
            {
                adh["bgtsactLandSub"] = adh["bgtActMLandSub"];
            }
            else
            {
                adh["bgtsactLandSub"] = adh["bgtestLandSub"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtsactLand"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLand"]) > 0)
            {
                adh["bgtsactLand"] = adh["bgtActMLand"];
            }
            else
            {
                adh["bgtsactLand"] = adh["bgtestLand"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtsactSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMSub"]) > 0)
            {
                adh["bgtsactSub"] = adh["bgtActMSub"];
            }
            else
            {
                adh["bgtsactSub"] = adh["bgtestSub"];
            }

            //}
            //if (Convert.ToDecimal(adh["bgtsactOther"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMOther"]) > 0)
            {
                adh["bgtsactOther"] = adh["bgtActMOther"];
            }
            else
            {
                adh["bgtsactOther"] = adh["bgtestOther"];
            }
            //}
            #endregion End System Actual

            #region Actual
            //if (Convert.ToDecimal(adh["bgtactctr"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMctr"]) > 0)
            {
                adh["bgtactctr"] = adh["bgtActMctr"];
            }
            else
            {
                adh["bgtactctr"] = adh["bgtestctr"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactctrldsp"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMctrldsp"]) > 0)
            {
                adh["bgtactctrldsp"] = adh["bgtActMctrldsp"];
            }
            else
            {
                adh["bgtactctrldsp"] = adh["bgtestctrldsp"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactsal"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMsal"]) > 0)
            {
                adh["bgtactsal"] = adh["bgtActMsal"];
            }
            else
            {
                adh["bgtactsal"] = adh["bgtestsal"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactmat"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMmat"]) > 0)
            {
                adh["bgtactmat"] = adh["bgtActMmat"];
            }
            else
            {
                adh["bgtactmat"] = adh["bgtestmat"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactchem"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMchem"]) > 0)
            {
                adh["bgtactchem"] = adh["bgtActMchem"];
            }
            else
            {
                adh["bgtactchem"] = adh["bgtestchem"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactmach"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMmach"]) > 0)
            {
                adh["bgtactmach"] = adh["bgtActMmach"];
            }
            else
            {
                adh["bgtactmach"] = adh["bgtestmach"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactequip"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMequip"]) > 0)
            {
                adh["bgtactequip"] = adh["bgtActMequip"];
            }
            else
            {
                adh["bgtactequip"] = adh["bgtestequip"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactToi"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMToi"]) > 0)
            {
                adh["bgtactToi"] = adh["bgtActMToi"];
            }
            else
            {
                adh["bgtactToi"] = adh["bgtestToi"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactPrd"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMPrd"]) > 0)
            {
                adh["bgtactPrd"] = adh["bgtActMPrd"];
            }
            else
            {
                adh["bgtactPrd"] = adh["bgtestPrd"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactOther"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMOther"]) > 0)
            {
                adh["bgtactOther"] = adh["bgtActMOther"];
            }
            else
            {
                adh["bgtactOther"] = adh["bgtestOther"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMSub"]) > 0)
            {
                adh["bgtactSub"] = adh["bgtActMSub"];
            }
            else
            {
                adh["bgtactSub"] = adh["bgtestSub"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactLand"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLand"]) > 0)
            {
                adh["bgtactLand"] = adh["bgtActMLand"];
            }
            else
            {
                adh["bgtactLand"] = adh["bgtestLand"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactLandSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLandSub"]) > 0)
            {
                adh["bgtactLandSub"] = adh["bgtActMLandSub"];
            }
            else
            {
                adh["bgtactLandSub"] = adh["bgtestLandSub"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactLandSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLandSub"]) > 0)
            {
                adh["bgtactLandSub"] = adh["bgtActMLandSub"];
            }
            else
            {
                adh["bgtactLandSub"] = adh["bgtestLandSub"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactLand"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLand"]) > 0)
            {
                adh["bgtactLand"] = adh["bgtActMLand"];
            }
            else
            {
                adh["bgtactLand"] = adh["bgtestLand"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtactSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMSub"]) > 0)
            {
                adh["bgtactSub"] = adh["bgtActMSub"];
            }
            else
            {
                adh["bgtactSub"] = adh["bgtestSub"];
            }

            //}
            //if (Convert.ToDecimal(adh["bgtactOther"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMOther"]) > 0)
            {
                adh["bgtactOther"] = adh["bgtActMOther"];
            }
            else
            {
                adh["bgtactOther"] = adh["bgtestOther"];
            }
            //}
            #endregion End Actual

            #region Latest Actual
            //if (Convert.ToDecimal(adh["bgtlactctr"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMctr"]) > 0)
            {
                adh["bgtlactctr"] = adh["bgtActMctr"];
            }
            else
            {
                adh["bgtlactctr"] = adh["bgtestctr"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtlactctrldsp"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMctrldsp"]) > 0)
            {
                adh["bgtlactctrldsp"] = adh["bgtActMctrldsp"];
            }
            else
            {
                adh["bgtlactctrldsp"] = adh["bgtestctrldsp"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtlactsal"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMsal"]) > 0)
            {
                adh["bgtlactsal"] = adh["bgtActMsal"];
            }
            else
            {
                adh["bgtlactsal"] = adh["bgtestsal"];
            }
            //}

            #region Take the latest cost from matm for each item from the Detailed Costing

            if (Convert.ToDecimal(adh["totalmonths"]) > 0)
            {
                adh["bgtlactmat"] = GetTotalMatnumCost(adh5, "matnum") / Convert.ToDecimal(adh["totalmonths"]);
                adh["bgtlactchem"] = GetTotalMatnumCost(adh9, "matnum") / Convert.ToDecimal(adh["totalmonths"]);
                adh["bgtlactmach"] = GetTotalMatnumCost(adh7, "matnum") / Convert.ToDecimal(adh["totalmonths"]);
                adh["bgtlactequip"] = GetTotalMatnumCost(adh3, "matnum") / Convert.ToDecimal(adh["totalmonths"]);
                adh["bgtlactToi"] = GetTotalMatnumCost(adh8, "matnum") / Convert.ToDecimal(adh["totalmonths"]);
            }

            #endregion

            //if (Convert.ToDecimal(adh["bgtlactLandSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLandSub"]) > 0)
            {
                adh["bgtlactLandSub"] = adh["bgtActMLandSub"];
            }
            else
            {
                adh["bgtlactLandSub"] = adh["bgtestLandSub"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtlactLand"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMLand"]) > 0)
            {
                adh["bgtlactLand"] = adh["bgtActMLand"];
            }
            else
            {
                adh["bgtlactLand"] = adh["bgtestLand"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtlactSub"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMSub"]) > 0)
            {
                adh["bgtlactSub"] = adh["bgtActMSub"];
            }
            else
            {
                adh["bgtlactSub"] = adh["bgtestSub"];
            }

            //}
            //if (Convert.ToDecimal(adh["bgtlactPrd"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMPrd"]) > 0)
            {
                adh["bgtlactPrd"] = adh["bgtActMPrd"];
            }
            else
            {
                adh["bgtlactPrd"] = adh["bgtestPrd"];
            }
            //}
            //if (Convert.ToDecimal(adh["bgtlactOther"]) <= 0)
            //{
            if (Convert.ToDecimal(adh["bgtActMOther"]) > 0)
            {
                adh["bgtlactOther"] = adh["bgtActMOther"];
            }
            else
            {
                adh["bgtlactOther"] = adh["bgtestOther"];
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
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh1 = this.dbaccess.DataSet.Tables["ADH1"];
            decimal totalamt = 0;

            if (BizFunctions.IsEmpty(adh["wkdaysmth"]))
            {
                adh["wkdaysmth"] = 22;
            }

            if (Convert.ToDecimal(adh["wkdaysmth"]) > 0)
            {

                if (adh1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in adh1.Rows)
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

                            if (!BizFunctions.IsEmpty(dr1["xday1"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday1"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday1"].ToString().Trim());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday2"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday2"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday2"].ToString().Trim());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday3"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday3"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday3"].ToString().Trim());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday4"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday4"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday4"].ToString().Trim());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday5"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday5"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday5"].ToString().Trim());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday6"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday6"].ToString().Trim());
                                days = days + isWorkShift(dr1["xday6"].ToString().Trim());
                            }
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

                            if (Convert.ToDecimal(dr1["oriamt"]) == 0 || Convert.ToDecimal(adh["wkdaysmth"]) == 0 || totalHrs == 0 || days == 0)
                            {
                                dr1["hcost"] = 0;
                            }
                            else
                            {
                                dr1["hcost"] = Convert.ToDecimal(dr1["oriamt"]) / (Convert.ToDecimal(adh["wkdaysmth"]) * (totalHrs / Convert.ToDecimal(days)));
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
                                    remark = "MON-SUN(Including PH) " + GetShiftTimeToTime(dr1["xday1"].ToString()) + " ";
                                }
                                else if (monToSunExcPH)
                                {
                                    remark = "MON-SUNExcluding PH) " + GetShiftTimeToTime(dr1["xday1"].ToString()) + "";
                                }
                                else if (monToSatIncPH)
                                {
                                    remark = "MON-SAT(Including PH) " + GetShiftTimeToTime(dr1["xday1"].ToString()) + "";
                                }
                                else if (monToSatExcPH)
                                {
                                    remark = "MON-SAT(Excluding PH) " + GetShiftTimeToTime(dr1["xday1"].ToString()) + "";
                                }
                                else if (monToFriIncPH)
                                {
                                    remark = "MON-FRI(Including PH) " + GetShiftTimeToTime(dr1["xday1"].ToString()) + "";
                                }
                                else if (monToFriExcPH)
                                {
                                    remark = "MON-FRI(Excluding PH) " + GetShiftTimeToTime(dr1["xday1"].ToString()) + "";
                                }
                                else if (friToSunIncPH)
                                {
                                    remark = "FRI-SUN(Including PH) " + GetShiftTimeToTime(dr1["xday5"].ToString()) + "";
                                }
                                else if (friToSunExcPH)
                                {
                                    remark = "FRI-SUN(Excluding PH) " + GetShiftTimeToTime(dr1["xday5"].ToString()) + "";
                                }

                                else if (satToSunIncPH)
                                {
                                    remark = "SAT to SUN, Including PH) " + GetShiftTimeToTime(dr1["xday6"].ToString()) + "";
                                }
                                else if (satTosunExcPH)
                                {
                                    remark = "SAT to SUN, Excluding PH " + GetShiftTimeToTime(dr1["xday6"].ToString()) + "";
                                }

                                dr1["remark"] = remark;


                            }
                        }
                    }
                }


                adh["bgtsactsal"] = totalamt;
                adh1_ttlamt.Text = totalamt.ToString();
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
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh7 = this.dbaccess.DataSet.Tables["ADH7"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (adh7.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh7.Rows)
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
            //adh["bgtsactmach"] = totalamt;
            adh["bgtsactmach"] = monthlyamt;
            adh7_ttlamt.Text = totalamt.ToString();
            manpowerFlag = false;
        }

        #endregion

        #region Refresh Equipment

        private void Refresh_Equipment()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh3 = this.dbaccess.DataSet.Tables["ADH3"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (adh3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh3.Rows)
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
            adh["bgtsactequip"] = monthlyamt;
            adh3_ttlamt.Text = totalamt.ToString();
            equipmentFlag = false;
        }

        #endregion

        #region Refresh Material

        private void Refresh_Material()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh5 = this.dbaccess.DataSet.Tables["ADH5"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (adh5.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh5.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = adh["totalmonths"];
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
            //adh["bgtsactmat"] = totalamt;
            adh["bgtsactmat"] = monthlyamt;
            adh5_ttlamt.Text = totalamt.ToString();
            materialFlag = false;
        }

        #endregion

        #region Refresh Toiletries

        private void Refresh_Toiletries()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh8 = this.dbaccess.DataSet.Tables["ADH8"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (adh8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = adh["totalmonths"];
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
            //adh["bgtsactToi"] = totalamt;
            adh["bgtsactToi"] = monthlyamt;
            adh8_ttlamt.Text = totalamt.ToString();
            toiletriesFlag = false;
        }

        #endregion

        #region Refresh Chemical

        private void Refresh_Chemical()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh9 = this.dbaccess.DataSet.Tables["ADH9"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (adh9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = adh["totalmonths"];
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
            //adh["bgtsactchem"] = totalamt;
            adh["bgtsactchem"] = monthlyamt;
            adh9_ttlamt.Text = totalamt.ToString();
            chemicalFlag = false;
        }

        #endregion

        #region Refresh Periodic Schedule

        private void Refresh_Schedule()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh6 = this.dbaccess.DataSet.Tables["ADH6"];
            decimal totalamt = 0;
            decimal monthlyamt = 0;

            if (adh6.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh6.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        ////dr1["mthnum"] = adh["totalmonths"];
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

            if (!BizFunctions.IsEmpty(adh["totalmonths"]) || totalamt == 0)
            {
                if (Convert.ToDecimal(adh["totalmonths"]) > 0)
                {
                    monthlyamt = totalamt / Convert.ToDecimal(adh["totalmonths"]);
                }
            }

            adh["bgtsactPrd"] = monthlyamt;
            adh6_ttlamt.Text = totalamt.ToString();
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
            DataRow adh = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable adh1 = this.dbaccess.DataSet.Tables["ADH1"];
            DataTable adh3 = this.dbaccess.DataSet.Tables["ADH3"];
            DataTable adh5 = this.dbaccess.DataSet.Tables["ADH5"];
            DataTable adh6 = this.dbaccess.DataSet.Tables["ADH6"];
            DataTable adh7 = this.dbaccess.DataSet.Tables["ADH7"];
            DataTable adh8 = this.dbaccess.DataSet.Tables["ADH8"];
            DataTable adh9 = this.dbaccess.DataSet.Tables["ADH9"];

            if (adh1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in adh1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(adh["totalmonths"]))
                        {
                            dr1["mthnum"] = adh["totalmonths"];
                        }
                    }
                }
            }

            if (adh3.Rows.Count > 0)
            {
                foreach (DataRow dr3 in adh3.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(adh["totalmonths"]))
                        {
                            dr3["mthnum"] = adh["totalmonths"];
                        }
                    }
                }
            }

            if (adh5.Rows.Count > 0)
            {
                foreach (DataRow dr5 in adh5.Rows)
                {
                    if (dr5.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(adh["totalmonths"]))
                        {
                            dr5["mthnum"] = adh["totalmonths"];
                        }
                    }
                }
            }


            if (adh6.Rows.Count > 0)
            {
                foreach (DataRow dr6 in adh6.Rows)
                {
                    if (dr6.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(adh["totalmonths"]))
                        {
                            dr6["mthnum"] = adh["totalmonths"];
                        }
                    }
                }
            }

            if (adh7.Rows.Count > 0)
            {
                foreach (DataRow dr7 in adh7.Rows)
                {
                    if (dr7.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(adh["totalmonths"]))
                        {
                            dr7["mthnum"] = adh["totalmonths"];
                        }
                    }
                }
            }

            if (adh8.Rows.Count > 0)
            {
                foreach (DataRow dr8 in adh8.Rows)
                {
                    if (dr8.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(adh["totalmonths"]))
                        {
                            dr8["mthnum"] = adh["totalmonths"];
                        }
                    }
                }
            }

            if (adh9.Rows.Count > 0)
            {
                foreach (DataRow dr9 in adh9.Rows)
                {
                    if (dr9.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(adh["totalmonths"]))
                        {
                            dr9["mthnum"] = adh["totalmonths"];
                        }
                    }
                }
            }


        }

        #endregion

        #endregion

        #region Feedback Detail Buttons

        #region Feedback Button Events

        private void adh4_btnAdd_Click(object sender, EventArgs e)
        {
            DataTable adh4 = this.dbaccess.DataSet.Tables["ADH4"];
            DataRow InsertCtr4 = adh4.NewRow();
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
                adh4.Rows.Add(InsertCtr4);
                txt_guid = string.Empty;
            }


        }

        private void adh4_btnUpdate_Click(object sender, EventArgs e)
        {
            DataTable adh4 = this.dbaccess.DataSet.Tables["ADH4"];

            txt_Daterasied = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Daterasied") as DateTimePicker;
            txt_Raisedby = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Raisedby") as TextBox;
            txt_desc = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_desc") as TextBox;
            txt_followup = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_followup") as TextBox;

            if (adh4.Rows.Count > 0)
            {
                foreach (DataRow dr4 in adh4.Rows)
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

                    string GetCtr4Row = "Select * from adh4 where [guid]='" + drCur["guid"].ToString() + "'";

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

        protected void adh10_btnBrowseEdu_Click(object sender, System.EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaCompress form = new ATL.BizModules.StaCompressFolders.StaCompress(dbaccess, "ADH", "ADH10", "refnum");
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

        protected void adh10_btnDownloadEdu_Click(object sender, System.EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaDecommpress form = new ATL.BizModules.StaCompressFolders.StaDecommpress(dbaccess, "ADH", "ADH10", "refnum");
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
            WordForm1 wf = new WordForm1(this.dbaccess, "ADH", "tnc", "ADH");
            wf.Show();
            wf.Focus();
        }
        #endregion

        private void getBgtEstTotal()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(adh["bgtestsal"]) + Convert.ToDecimal(adh["bgtestmat"]) + Convert.ToDecimal(adh["bgtestchem"]) +
                                 Convert.ToDecimal(adh["bgtestmach"]) + Convert.ToDecimal(adh["bgtestequip"]) + Convert.ToDecimal(adh["bgtestToi"]) +
                                 Convert.ToDecimal(adh["bgtestPrd"]) + Convert.ToDecimal(adh["bgtestSub"]) + Convert.ToDecimal(adh["bgtestOther"]);
            txt_grandTotal1.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(adh["bgtestctr"]))
            {
                adh["bgtestctr"] = 0;
            }
            txt_grossProfit1.Text = Convert.ToString(Math.Round((Convert.ToDecimal(adh["bgtestctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(adh["bgtestLand"]) + Convert.ToDecimal(adh["bgtestLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(adh["bgtestctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp1.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp1.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtManualActTotal()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(adh["bgtActMsal"]) + Convert.ToDecimal(adh["bgtActMmat"]) + Convert.ToDecimal(adh["bgtActMchem"]) +
                                 Convert.ToDecimal(adh["bgtActMmach"]) + Convert.ToDecimal(adh["bgtActMequip"]) + Convert.ToDecimal(adh["bgtActMToi"]) +
                                 Convert.ToDecimal(adh["bgtActMPrd"]) + Convert.ToDecimal(adh["bgtActMSub"]) + Convert.ToDecimal(adh["bgtActMOther"]);
            txt_grandTotal2.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(adh["bgtActMctr"]))
            {
                adh["bgtActMctr"] = 0;
            }
            txt_grossProfit2.Text = Convert.ToString(Math.Round((Convert.ToDecimal(adh["bgtActMctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(adh["bgtActMLand"]) + Convert.ToDecimal(adh["bgtActMLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(adh["bgtActMctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp2.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp2.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtSysActTotal()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(adh["bgtsactsal"]) + Convert.ToDecimal(adh["bgtsactmat"]) + Convert.ToDecimal(adh["bgtsactchem"]) +
                                 Convert.ToDecimal(adh["bgtsactmach"]) + Convert.ToDecimal(adh["bgtsactequip"]) + Convert.ToDecimal(adh["bgtsactToi"]) +
                                 Convert.ToDecimal(adh["bgtsactPrd"]) + Convert.ToDecimal(adh["bgtsactSub"]) + Convert.ToDecimal(adh["bgtsactOther"]);
            txt_grandTotal3.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(adh["bgtsactctr"]))
            {
                adh["bgtsactctr"] = 0;
            }

            txt_grossProfit3.Text = Convert.ToString(Math.Round((Convert.ToDecimal(adh["bgtsactctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(adh["bgtsactLand"]) + Convert.ToDecimal(adh["bgtsactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(adh["bgtsactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp3.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp3.Text = Convert.ToString(grosProfitLsdp);

        }

        private void getBgtActualTotal()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(adh["bgtactsal"]) + Convert.ToDecimal(adh["bgtactmat"]) + Convert.ToDecimal(adh["bgtactchem"]) +
                                 Convert.ToDecimal(adh["bgtactmach"]) + Convert.ToDecimal(adh["bgtactequip"]) + Convert.ToDecimal(adh["bgtactToi"]) +
                                 Convert.ToDecimal(adh["bgtactPrd"]) + Convert.ToDecimal(adh["bgtactSub"]) + Convert.ToDecimal(adh["bgtactOther"]);
            txt_grandTotal4.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(adh["bgtactctr"]))
            {
                adh["bgtactctr"] = 0;
            }
            txt_grossProfit4.Text = Convert.ToString(Math.Round((Convert.ToDecimal(adh["bgtactctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(adh["bgtactLand"]) + Convert.ToDecimal(adh["bgtactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(adh["bgtactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp4.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp4.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtLatestActTotal()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(adh["bgtlactsal"]) + Convert.ToDecimal(adh["bgtlactmat"]) + Convert.ToDecimal(adh["bgtlactchem"]) +
                                 Convert.ToDecimal(adh["bgtlactmach"]) + Convert.ToDecimal(adh["bgtlactequip"]) + Convert.ToDecimal(adh["bgtlactToi"]) +
                                 Convert.ToDecimal(adh["bgtlactPrd"]) + Convert.ToDecimal(adh["bgtlactSub"]) + Convert.ToDecimal(adh["bgtlactOther"]);
            txt_grandTotal5.Text = Convert.ToString(Math.Round(grandTotal, 2));
            if (BizFunctions.IsEmpty(adh["bgtlactctr"]))
            {
                adh["bgtlactctr"] = 0;
            }
            txt_grossProfit5.Text = Convert.ToString(Math.Round((Convert.ToDecimal(adh["bgtlactctr"]) - grandTotal), 2));

            decimal grandTotalLsdp = Convert.ToDecimal(adh["bgtlactLand"]) + Convert.ToDecimal(adh["bgtlactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(adh["bgtlactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp5.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp5.Text = Convert.ToString(grosProfitLsdp);
        }

        private void initiValues()
        {
            DataRow adh = this.dbaccess.DataSet.Tables["adh"].Rows[0];

            if (BizFunctions.IsEmpty(adh["bgtestctr"]))
            {
                adh["bgtestctr"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestctrldsp"]))
            {
                adh["bgtestctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestLandSub"]))
            {
                adh["bgtestLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestsal"]))
            {
                adh["bgtestsal"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestmat"]))
            {
                adh["bgtestmat"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestchem"]))
            {
                adh["bgtestchem"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestmach"]))
            {
                adh["bgtestmach"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestequip"]))
            {
                adh["bgtestequip"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestToi"]))
            {
                adh["bgtestToi"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestPrd"]))
            {
                adh["bgtestPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestLand"]))
            {
                adh["bgtestLand"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestSub"]))
            {
                adh["bgtestSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtestOther"]))
            {
                adh["bgtestOther"] = 0;
            }

            ///////

            if (BizFunctions.IsEmpty(adh["bgtActMctr"]))
            {
                adh["bgtActMctr"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMctrldsp"]))
            {
                adh["bgtActMctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMLandSub"]))
            {
                adh["bgtActMLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMsal"]))
            {
                adh["bgtActMsal"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMmat"]))
            {
                adh["bgtActMmat"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMchem"]))
            {
                adh["bgtActMchem"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMmach"]))
            {
                adh["bgtActMmach"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMequip"]))
            {
                adh["bgtActMequip"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMToi"]))
            {
                adh["bgtActMToi"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMPrd"]))
            {
                adh["bgtActMPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMLand"]))
            {
                adh["bgtActMLand"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMSub"]))
            {
                adh["bgtActMSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtActMOther"]))
            {
                adh["bgtActMOther"] = 0;
            }

            /////////

            if (BizFunctions.IsEmpty(adh["bgtsactctr"]))
            {
                adh["bgtsactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactctrldsp"]))
            {
                adh["bgtsactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactLandSub"]))
            {
                adh["bgtsactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactsal"]))
            {
                adh["bgtsactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactmat"]))
            {
                adh["bgtsactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactchem"]))
            {
                adh["bgtsactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactmach"]))
            {
                adh["bgtsactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactequip"]))
            {
                adh["bgtsactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactToi"]))
            {
                adh["bgtsactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactPrd"]))
            {
                adh["bgtsactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactLand"]))
            {
                adh["bgtsactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactSub"]))
            {
                adh["bgtsactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtsactOther"]))
            {
                adh["bgtsactOther"] = 0;
            }

            ////////

            if (BizFunctions.IsEmpty(adh["bgtactctr"]))
            {
                adh["bgtactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactctrldsp"]))
            {
                adh["bgtactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactLandSub"]))
            {
                adh["bgtactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactsal"]))
            {
                adh["bgtactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactmat"]))
            {
                adh["bgtactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactchem"]))
            {
                adh["bgtactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactmach"]))
            {
                adh["bgtactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactequip"]))
            {
                adh["bgtactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactToi"]))
            {
                adh["bgtactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactPrd"]))
            {
                adh["bgtactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactLand"]))
            {
                adh["bgtactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactSub"]))
            {
                adh["bgtactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtactOther"]))
            {
                adh["bgtactOther"] = 0;
            }

            /////  

            if (BizFunctions.IsEmpty(adh["bgtlactctr"]))
            {
                adh["bgtlactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactctrldsp"]))
            {
                adh["bgtlactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactLandSub"]))
            {
                adh["bgtlactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactsal"]))
            {
                adh["bgtlactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactmat"]))
            {
                adh["bgtlactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactchem"]))
            {
                adh["bgtlactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactmach"]))
            {
                adh["bgtlactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactequip"]))
            {
                adh["bgtlactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactToi"]))
            {
                adh["bgtlactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactPrd"]))
            {
                adh["bgtlactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactLand"]))
            {
                adh["bgtlactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactSub"]))
            {
                adh["bgtlactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(adh["bgtlactOther"]))
            {
                adh["bgtlactOther"] = 0;
            }


        }

        private void GetQuotation(string qctNum)
        {

            DataRow ADH = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            DataTable ADH1 = this.dbaccess.DataSet.Tables["ADH1"];
            DataTable ADH3 = this.dbaccess.DataSet.Tables["ADH3"];
            DataTable ADH4 = this.dbaccess.DataSet.Tables["ADH4"];
            DataTable ADH5 = this.dbaccess.DataSet.Tables["ADH5"];
            DataTable ADH6 = this.dbaccess.DataSet.Tables["ADH6"];
            DataTable ADH7 = this.dbaccess.DataSet.Tables["ADH7"];
            DataTable ADH8 = this.dbaccess.DataSet.Tables["ADH8"];
            DataTable ADH9 = this.dbaccess.DataSet.Tables["ADH9"];
            DataTable ADH10 = this.dbaccess.DataSet.Tables["ADH10"];

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
            DataTable QCT4 = this.dbaccess.DataSet.Tables["QCT4"];
            DataTable QCT5 = this.dbaccess.DataSet.Tables["QCT5"];
            DataTable QCT6 = this.dbaccess.DataSet.Tables["QCT6"];
            DataTable QCT7 = this.dbaccess.DataSet.Tables["QCT7"];
            DataTable QCT8 = this.dbaccess.DataSet.Tables["QCT8"];
            DataTable QCT9 = this.dbaccess.DataSet.Tables["QCT9"];
            DataTable QCT10 = this.dbaccess.DataSet.Tables["QCT10"];


            if (QCTH != null)
            {
                ADH["adhnum"] = QCTH["adhnum"];
                ADH["coy"] = QCTH["coy"];
                ADH["coyname"] = QCTH["coyname"];
                ADH["officerqty"] = QCTH["officerqty"];
                ADH["discamt"] = QCTH["discamt"];
                ADH["payid"] = QCTH["payid"];
                ADH["duty"] = QCTH["duty"];
                ADH["event"] = QCTH["event"];
                ADH["schedule"] = QCTH["schedule"];
                ADH["scheduleoption"] = QCTH["scheduleoption"];
                ADH["isweekdays"] = QCTH["isweekdays"];
                ADH["sinstruction"] = QCTH["sinstruction"];
                ADH["created"] = QCTH["created"];
                ADH["trandate"] = QCTH["trandate"];
                ADH["modified"] = QCTH["modified"];
                ADH["year"] = QCTH["year"];
                ADH["period"] = QCTH["period"];
                ADH["flag"] = QCTH["flag"];
                ADH["user"] = QCTH["user"];
                ADH["guid"] = QCTH["guid"];
                ADH["remark"] = QCTH["remark"];
                ADH["contractdate"] = QCTH["contractdate"];
                ADH["commencedate"] = QCTH["commencedate"];
                ADH["enddate"] = QCTH["enddate"];
                ADH["issuedby"] = QCTH["issuedby"];
                ADH["ttlbillingamt"] = QCTH["ttlbillingamt"];
                ADH["discount"] = QCTH["discount"];
                ADH["subtotal"] = QCTH["subtotal"];
                ADH["othercost"] = QCTH["othercost"];
                ADH["createdby"] = QCTH["createdby"];
                ADH["arnum"] = QCTH["arnum"];
                ADH["additionalcosts"] = QCTH["additionalcosts"];
                ADH["equipmentcosts"] = QCTH["equipmentcosts"];
                ADH["cstarttime"] = QCTH["cstarttime"];
                ADH["cendtime"] = QCTH["cendtime"];
                ADH["cc1"] = QCTH["cc1"];
                ADH["cc2"] = QCTH["cc2"];
                ADH["payinfo"] = QCTH["payinfo"];
                ADH["billadd4"] = QCTH["billadd4"];
                ADH["oldrefnum"] = QCTH["oldrefnum"];
                ADH["padhnum"] = QCTH["padhnum"];
                ADH["totalmonths"] = QCTH["totalmonths"];
                ADH["bgtestsal"] = QCTH["bgtestsal"];
                ADH["bgtestmat"] = QCTH["bgtestmat"];
                ADH["bgtestchem"] = QCTH["bgtestchem"];
                ADH["bgtestmach"] = QCTH["bgtestmach"];
                ADH["bgtestPrd"] = QCTH["bgtestPrd"];
                ADH["bgtestLand"] = QCTH["bgtestLand"];
                ADH["bgtestSub"] = QCTH["bgtestSub"];
                ADH["bgtestOther"] = QCTH["bgtestOther"];
                ADH["bgtestToi"] = QCTH["bgtestToi"];
                ADH["bgtActMLand"] = QCTH["bgtActMLand"];
                ADH["bgtActMSub"] = QCTH["bgtActMSub"];
                ADH["bgtActMOther"] = QCTH["bgtActMOther"];
                ADH["bgtsactmat"] = QCTH["bgtsactmat"];
                ADH["bgtsactchem"] = QCTH["bgtsactchem"];
                ADH["bgtsactmach"] = QCTH["bgtsactmach"];
                ADH["bgtsactPrd"] = QCTH["bgtsactPrd"];
                ADH["bgtsactLand"] = QCTH["bgtsactLand"];
                ADH["bgtsactSub"] = QCTH["bgtsactSub"];
                ADH["bgtsactOther"] = QCTH["bgtsactOther"];
                ADH["bgtsactToi"] = QCTH["bgtsactToi"];
                ADH["bgtsactsal"] = QCTH["bgtsactsal"];
                ADH["bgtactsal"] = QCTH["bgtactsal"];
                ADH["bgtactmat"] = QCTH["bgtactmat"];
                ADH["bgtactchem"] = QCTH["bgtactchem"];
                ADH["bgtactmach"] = QCTH["bgtactmach"];
                ADH["bgtactToi"] = QCTH["bgtactToi"];
                ADH["bgtactPrd"] = QCTH["bgtactPrd"];
                ADH["bgtactLand"] = QCTH["bgtactLand"];
                ADH["bgtactSub"] = QCTH["bgtactSub"];
                ADH["bgtactOther"] = QCTH["bgtactOther"];
                ADH["bgtlactsal"] = QCTH["bgtlactsal"];
                ADH["bgtlactmat"] = QCTH["bgtlactmat"];
                ADH["bgtlactchem"] = QCTH["bgtlactchem"];
                ADH["bgtlactmach"] = QCTH["bgtlactmach"];
                ADH["bgtlactToi"] = QCTH["bgtlactToi"];
                ADH["bgtlactPrd"] = QCTH["bgtlactPrd"];
                ADH["bgtlactLand"] = QCTH["bgtlactLand"];
                ADH["bgtlactSub"] = QCTH["bgtlactSub"];
                ADH["bgtlactOther"] = QCTH["bgtlactOther"];
                ADH["bgtActMsal"] = QCTH["bgtActMsal"];
                ADH["bgtActMmat"] = QCTH["bgtActMmat"];
                ADH["bgtActMchem"] = QCTH["bgtActMchem"];
                ADH["bgtActMmach"] = QCTH["bgtActMmach"];
                ADH["bgtActMToi"] = QCTH["bgtActMToi"];
                ADH["bgtActMPrd"] = QCTH["bgtActMPrd"];
                ADH["bgtestequip"] = QCTH["bgtestequip"];
                ADH["bgtActMequip"] = QCTH["bgtActMequip"];
                ADH["bgtsactequip"] = QCTH["bgtsactequip"];
                ADH["bgtactequip"] = QCTH["bgtactequip"];
                ADH["bgtlactequip"] = QCTH["bgtlactequip"];
                ADH["bgtestctr"] = QCTH["bgtestctr"];
                ADH["bgtActMctr"] = QCTH["bgtActMctr"];
                ADH["bgtsactctr"] = QCTH["bgtsactctr"];
                ADH["bgtactctr"] = QCTH["bgtactctr"];
                ADH["bgtlactctr"] = QCTH["bgtlactctr"];
                ADH["tnc"] = QCTH["tnc"];
                ADH["wkdaysmth"] = QCTH["wkdaysmth"];
                ADH["bgtestctrldsp"] = QCTH["bgtestctrldsp"];
                ADH["bgtestLandSub"] = QCTH["bgtestLandSub"];
                ADH["bgtActMctrldsp"] = QCTH["bgtActMctrldsp"];
                ADH["bgtActMLandSub"] = QCTH["bgtActMLandSub"];
                ADH["bgtsactctrldsp"] = QCTH["bgtsactctrldsp"];
                ADH["bgtsactLandSub"] = QCTH["bgtsactLandSub"];
                ADH["bgtactctrldsp"] = QCTH["bgtactctrldsp"];
                ADH["bgtactLandSub"] = QCTH["bgtactLandSub"];
                ADH["bgtlactctrldsp"] = QCTH["bgtlactctrldsp"];
                ADH["bgtlactLandSub"] = QCTH["bgtlactLandSub"];

            }





            if (QCT1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH1);

                foreach (DataRow dr1 in QCT1.Rows)
                {
                    DataRow InsertADH1 = ADH1.NewRow();

                    InsertADH1["matnum"] = dr1["matnum"];
                    InsertADH1["estmamt"] = dr1["estmamt"];
                    InsertADH1["actmamt"] = dr1["actmamt"];
                    InsertADH1["officerqty"] = dr1["officerqty"];
                    InsertADH1["mthnum"] = dr1["mthnum"];
                    InsertADH1["rate"] = dr1["rate"];
                    InsertADH1["oriamt"] = dr1["oriamt"];
                    InsertADH1["hperd"] = dr1["hperd"];
                    InsertADH1["dperw"] = dr1["dperw"];
                    InsertADH1["wperm"] = dr1["wperm"];
                    InsertADH1["hcost"] = dr1["hcost"];
                    InsertADH1["xday1"] = dr1["xday1"];
                    InsertADH1["xday2"] = dr1["xday2"];
                    InsertADH1["xday3"] = dr1["xday3"];
                    InsertADH1["xday4"] = dr1["xday4"];
                    InsertADH1["xday5"] = dr1["xday5"];
                    InsertADH1["xday6"] = dr1["xday6"];
                    InsertADH1["xday7"] = dr1["xday7"];
                    InsertADH1["perappoitamt"] = dr1["perappoitamt"];
                    InsertADH1["ispubhol"] = dr1["ispubhol"];
                    InsertADH1["perhr"] = dr1["perhr"];
                    InsertADH1["remark"] = dr1["remark"];

                    ADH1.Rows.Add(InsertADH1);
                }
            }

            if (QCT3.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH3);

                foreach (DataRow dr3 in QCT3.Rows)
                {
                    DataRow InsertADH3 = ADH3.NewRow();

                    InsertADH3["matnum"] = dr3["matnum"];
                    InsertADH3["itemdesc"] = dr3["itemdesc"];
                    InsertADH3["estmamt"] = dr3["estmamt"];
                    InsertADH3["actmamt"] = dr3["actmamt"];
                    InsertADH3["qty"] = dr3["qty"];
                    InsertADH3["totalmths"] = dr3["totalmths"];
                    InsertADH3["mthnum"] = dr3["mthnum"];
                    InsertADH3["price"] = dr3["price"];
                    InsertADH3["oriamt"] = dr3["oriamt"];
                    InsertADH3["moriamt"] = dr3["moriamt"];
                    InsertADH3["amount"] = dr3["amount"];
                    InsertADH3["remark"] = dr3["remark"];


                    ADH3.Rows.Add(InsertADH3);
                }
            }

            if (QCT4.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH4);

                foreach (DataRow dr4 in QCT4.Rows)
                {

                    DataRow InsertADH4 = ADH4.NewRow();

                    InsertADH4["raisedby"] = dr4["raisedby"];
                    InsertADH4["desc"] = dr4["desc"];
                    InsertADH4["followup"] = dr4["followup"];

                    ADH4.Rows.Add(InsertADH4);
                }

            }

            if (QCT5.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH5);

                foreach (DataRow dr5 in QCT5.Rows)
                {
                    DataRow InsertADH3 = ADH3.NewRow();

                    InsertADH3["matnum"] = dr5["matnum"];
                    InsertADH3["itemdesc"] = dr5["itemdesc"];
                    InsertADH3["estmamt"] = dr5["estmamt"];
                    InsertADH3["actmamt"] = dr5["actmamt"];
                    InsertADH3["qty"] = dr5["qty"];
                    InsertADH3["mthnum"] = dr5["mthnum"];
                    InsertADH3["price"] = dr5["price"];
                    InsertADH3["oriamt"] = dr5["oriamt"];
                    InsertADH3["moriamt"] = dr5["moriamt"];
                    InsertADH3["amount"] = dr5["amount"];
                    InsertADH3["remark"] = dr5["remark"];


                    ADH3.Rows.Add(InsertADH3);
                }
            }

            if (QCT6.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH6);

                foreach (DataRow dr6 in QCT6.Rows)
                {
                    DataRow InsertADH6 = ADH6.NewRow();

                    InsertADH6["svccode"] = dr6["svccode"];
                    InsertADH6["svcdesc"] = dr6["svcdesc"];
                    InsertADH6["estmamt"] = dr6["estmamt"];
                    InsertADH6["actmamt"] = dr6["actmamt"];
                    InsertADH6["qty"] = dr6["qty"];
                    InsertADH6["oriamt"] = dr6["oriamt"];
                    InsertADH6["frequencycode"] = dr6["frequencycode"];
                    InsertADH6["xmonth1"] = dr6["xmonth1"];
                    InsertADH6["xmonth2"] = dr6["xmonth2"];
                    InsertADH6["xmonth3"] = dr6["xmonth3"];
                    InsertADH6["xmonth4"] = dr6["xmonth4"];
                    InsertADH6["xmonth5"] = dr6["xmonth5"];
                    InsertADH6["xmonth6"] = dr6["xmonth6"];
                    InsertADH6["xmonth7"] = dr6["xmonth7"];
                    InsertADH6["xmonth8"] = dr6["xmonth8"];
                    InsertADH6["xmonth9"] = dr6["xmonth9"];
                    InsertADH6["xmonth10"] = dr6["xmonth10"];
                    InsertADH6["xmonth11"] = dr6["xmonth11"];
                    InsertADH6["xmonth12"] = dr6["xmonth12"];
                    InsertADH6["remark"] = dr6["remark"];
                    InsertADH6["workscope"] = dr6["workscope"];
                    InsertADH6["sitenum"] = dr6["sitenum"];

                    ADH6.Rows.Add(InsertADH6);
                }
            }

            if (QCT7.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH7);

                foreach (DataRow dr7 in QCT7.Rows)
                {
                    DataRow InsertADH7 = ADH7.NewRow();

                    InsertADH7["matnum"] = dr7["matnum"];
                    InsertADH7["itemdesc"] = dr7["itemdesc"];
                    InsertADH7["estmamt"] = dr7["estmamt"];
                    InsertADH7["actmamt"] = dr7["actmamt"];
                    InsertADH7["qty"] = dr7["qty"];
                    InsertADH7["mthnum"] = dr7["mthnum"];
                    InsertADH7["price"] = dr7["price"];
                    InsertADH7["oriamt"] = dr7["oriamt"];
                    InsertADH7["moriamt"] = dr7["moriamt"];
                    InsertADH7["amount"] = dr7["amount"];
                    InsertADH7["remark"] = dr7["remark"];


                    ADH7.Rows.Add(InsertADH7);
                }
            }

            if (QCT8.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH8);

                foreach (DataRow dr8 in QCT8.Rows)
                {
                    DataRow InsertADH8 = ADH8.NewRow();

                    InsertADH8["matnum"] = dr8["matnum"];
                    InsertADH8["itemdesc"] = dr8["itemdesc"];
                    InsertADH8["estmamt"] = dr8["estmamt"];
                    InsertADH8["actmamt"] = dr8["actmamt"];
                    InsertADH8["qty"] = dr8["qty"];
                    InsertADH8["mthnum"] = dr8["mthnum"];
                    InsertADH8["price"] = dr8["price"];
                    InsertADH8["oriamt"] = dr8["oriamt"];
                    InsertADH8["moriamt"] = dr8["moriamt"];
                    InsertADH8["amount"] = dr8["amount"];
                    InsertADH8["remark"] = dr8["remark"];


                    ADH8.Rows.Add(InsertADH8);
                }
            }

            if (QCT9.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH9);

                foreach (DataRow dr9 in QCT9.Rows)
                {
                    DataRow InsertADH9 = ADH9.NewRow();

                    InsertADH9["matnum"] = dr9["matnum"];
                    InsertADH9["itemdesc"] = dr9["itemdesc"];
                    InsertADH9["estmamt"] = dr9["estmamt"];
                    InsertADH9["actmamt"] = dr9["actmamt"];
                    InsertADH9["qty"] = dr9["qty"];
                    InsertADH9["mthnum"] = dr9["mthnum"];
                    InsertADH9["price"] = dr9["price"];
                    InsertADH9["oriamt"] = dr9["oriamt"];
                    InsertADH9["moriamt"] = dr9["moriamt"];
                    InsertADH9["amount"] = dr9["amount"];
                    InsertADH9["remark"] = dr9["remark"];

                    ADH9.Rows.Add(InsertADH9);
                }
            }

            if (QCT10.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(ADH10);

                foreach (DataRow dr10 in QCT10.Rows)
                {
                    DataRow InsertADH10 = ADH10.NewRow();

                    InsertADH10["remark"] = dr10["remark"];
                    InsertADH10["filedesc"] = dr10["filedesc"];
                    InsertADH10["filename"] = dr10["filename"];
                    InsertADH10["physicalserverlocation"] = dr10["physicalserverlocation"];
                    InsertADH10["trackingno"] = dr10["trackingno"];
                    InsertADH10["mappedDrivelocation"] = dr10["mappedDrivelocation"];
                    InsertADH10["created"] = dr10["created"];

                    ADH10.Rows.Add(InsertADH10);
                }
            }

            GetLatesArmInfo(qctNum);
            GetLatestSiteInfo(qctNum);


        }

        private void GetLatesArmInfo(string qctNum)
        {
            DataRow ADH = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            string getArmLatest = "Select * from arm where qctnum='" + qctNum + "' and ISNULL(isPending,0)=0";
            this.dbaccess.ReadSQL("ArmLatest", getArmLatest);

            DataTable ArmLatest = this.dbaccess.DataSet.Tables["ArmLatest"];

            if (ArmLatest.Rows.Count > 0)
            {
                DataRow drArm = this.dbaccess.DataSet.Tables["ArmLatest"].Rows[0];

                ADH["arnum"] = drArm["arnum"];
                ADH["billadd1"] = drArm["baddr1"];
                ADH["billadd2"] = drArm["baddr2"];
                ADH["billadd3"] = drArm["baddr3"];
                ADH["billadd4"] = drArm["baddr4"];

            }
        }

        private void GetLatestSiteInfo(string qctNum)
        {
            DataRow ADH = this.dbaccess.DataSet.Tables["ADH"].Rows[0];
            string getSiteLatest = "Select * from sitm where qctnum='" + qctNum + "'";
            this.dbaccess.ReadSQL("SitmiLatest", getSiteLatest);

            DataTable SitmiLatest = this.dbaccess.DataSet.Tables["SitmiLatest"];

            if (SitmiLatest.Rows.Count > 0)
            {
                DataRow drSitmi = this.dbaccess.DataSet.Tables["SitmiLatest"].Rows[0];

                ADH["sitenum"] = drSitmi["sitenum"];
                ADH["sitename"] = drSitmi["sitename"];
                ADH["sectorcode"] = drSitmi["sectorcode"];
                ADH["sitename"] = drSitmi["sitename"];
                ADH["addr1"] = drSitmi["addr1"];
                ADH["addr2"] = drSitmi["addr2"];
                ADH["addr3"] = drSitmi["addr3"];
                ADH["country"] = drSitmi["country"];
                ADH["postalcode"] = drSitmi["postalcode"];
                ADH["rep1"] = drSitmi["rep1"];
                ADH["email"] = drSitmi["email"];
                ADH["sfax"] = drSitmi["fax"];
                ADH["tel1"] = drSitmi["tel1"];
                ADH["rep2"] = drSitmi["rep2"];
                ADH["tel2"] = drSitmi["tel2"];
                ADH["rep1tel"] = drSitmi["rep1tel"];
                ADH["rep2tel"] = drSitmi["rep2tel"];
                ADH["prmcode"] = drSitmi["prmcode"];

            }
        }

        #endregion

        private string[] GetZoneSupervisor(string sectorcode)
        {
            string getSup = "select s.opmgr,h.empname from sem s LEFT JOIN HEMPH h on s.opmgr=h.empnum " +
                            "where s.sectorcode='" + sectorcode.Trim() + "'";

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


