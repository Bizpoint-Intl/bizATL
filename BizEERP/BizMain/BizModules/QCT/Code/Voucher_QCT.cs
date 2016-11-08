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

namespace ATL.QCT
{
    public class Voucher_QCT : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName,budgetFormName, manpowerFormName,machineryFormName, equipmentcostsFormName,materialFormName,toiletriesFormName,
                         chemicalFormName,periodicFormName,archiveFormName,tacFormName,feedbackFormName,txt_guid = null;
        protected CheckBox qct_daily, qct_weekldays, qct_weekend = null;
        protected TextBox qct1_ttlamt, qct3_ttlamt, txt_ttlamt, qct5_ttlamt, qct6_ttlamt, qct7_ttlamt, qct8_ttlamt, qct9_ttlamt, qcth_arnum, 
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
        protected Button btn_Voucher_Reports, qct4_btnUpdate, qct4_btnAdd, qct10_btnBrowseEdu, qct10_btnDownloadEdu;
        protected DateTimePicker txt_Daterasied;
        protected bool opened, isMouseClicked = false;
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

        public Voucher_QCT(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_QCT.xml", moduleName, voucherBaseHelpers)
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
            DataRow QCTH = e.DBAccess.DataSet.Tables["QCTH"].Rows[0];
            string scheduleoption = QCTH["scheduleoption"].ToString();
           
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


            if (Convert.IsDBNull(QCTH["trandate"]))
            {
                QCTH["trandate"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
            }

            Initialise();

            initiValues();

            #region ColumnChanged Events
            e.DBAccess.DataSet.Tables["QCTH"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCTH_ColumnChanged);
            e.DBAccess.DataSet.Tables["QCT1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCT1_ColumnChanged);
            e.DBAccess.DataSet.Tables["QCT3"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCT3_ColumnChanged);
            e.DBAccess.DataSet.Tables["QCT5"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCT5_ColumnChanged);
            e.DBAccess.DataSet.Tables["QCT6"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCT6_ColumnChanged);
            e.DBAccess.DataSet.Tables["QCT7"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCT7_ColumnChanged);
            e.DBAccess.DataSet.Tables["QCT8"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCT8_ColumnChanged);
            e.DBAccess.DataSet.Tables["QCT9"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_QCT9_ColumnChanged);
            #endregion


            string GetShiftInfo = "Select * from vShlv";

            this.dbaccess.ReadSQL("vSHLV", GetShiftInfo);

            opened = true;

            if (BizFunctions.IsEmpty(QCTH["tnc"]))
            {
                string varBinary = "0x504B0304140002000800976C904258470FF02F020000B20400000B000000636F6E74656E742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB767B5A2D5BFA99D2CBCBE651D35E9739B5AE978FAAAC299A47CB6C91378FDAE9A36A952FCD4B8FBCC68FB823F9A0CDDFB5B77D196DFD77CFABDBBEF9AE29B7CF2B427CB1CADA6212A2D05C5EDC160E351D0032ABB3ABDB42415BA2B0FFFABBB258BEFDECA379DBAE1EDDBD7B757535BEBA37AEEA8BBBBB0F1F3EBCCBDF9AA65575651B02B8CC0E37DEDBD9D9BF7B55176D5E5BFA6693DBCF0E37F6D112D8B77D5DF9C4A7CAD4A2BA5AD72523399BDECDCB1CAF357777C7BB773FB20C764E7CB57D9E4DF3ED593E2D9BF4AEFD265BB7D582883EDD66066A8E1E0B23F1BFA9FC0ECC3EFBE835F52D7F9F678BA2BCFEECA3269FB6C4DE1FD977E4EFED554D43A8DB22E78EEE7A0087A1BFDCED425F6575765167ABB9F9823E808CF01FDBF2D68BAA5E64A569B0C81A9A9FED557661BE7FDD66CB5956CF3E62440687AC5F4CAAD9B5FD032271F4980543C795CA1F5EF720097FA87F4ABB9D8F8E1EF3A7ABFE1B344C03749545203ECDCFB375D9A62FCDE0D36734751F1D9D2E56EDF5E3BBF6CD23FD7D657E511CDD28057FF3970CCDFCD5D53747FF0F504B0304140002000800976C9042D5F05C2BBA020000F80500000A0000007374796C65732E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E871757E5E4CF347B36ABA5EE4CB76BB69AFCBBC49E9DD65F388FFA0C6F5F251953545F368992DF2E6513B7D54ADF2A579E791D7F811F7231F9C57B77DF55D536E9F57DBD36AB1CADA621282795716CBB79F7D346FDBD5A3BB77AFAEAEC657F7C6557D7177F7E1C38777F95BD3B4B9BCB86D97D474A03FA1C86DE128FDF0BE25E67945843CCFA6F9F62C9F964D7AD77E23D43D7A2CB4E27F53F91D7D7CF6D1D3FC3C5B976DFA32ABB38B3A5BCDD36704EB236D739E2D8AF2FAB38FDAFC1D7D767718CCF36299A72FD68B495EC7DF95CF56596DA77C7B33021B3AFBF6F52AAF6516225D99F7F0C7F6AA260AD66D410C765E3D9A5665557FF6D193729D9B57B9D57A396380B960F6D9474D5516B3812657C5AC9D7FF651B66E2BC6F2AE87E630CE2FAA7A91955D845766D00C091D3D421F4B2624890D31CCF2BCB858D7C434D532E506F2E536DA359F7D749E950D0D86BFA1296FF2F6B38F76C6F7A70BFD8C5A13A353DFF4F9AEFB0C10565553B42CAA657EDEEA77C5725AE7E0360263F8135FDC823B6D5BE16D90A6C384FA274847081553957D43B55576916F97D975B56E03DA7DB15AECD869F51A756697BFD1D93918DF2F961FD94FE7797131070576F1A9022202B7DB555DD06033A1C3AAAADB3A2B5A7E7191D517C572BBAD56A0E883834F0F14A27E31A95A1A46FC3B10141A70F760E7D3F09B5A10A1297AB8730F5F390EF246E648D7A7957EB1C89A962631A4A07E084801055FB7D97296D596A57D227A34F6E7AC03DF7CDC51DB47FF0F504B0304140002000800976C90425CC0E1810501000000010000150000004D4554412D494E462F6D616E69666573742E786D6CEDBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E8F1225B16E779D33E32BFA4F4DAB2B17F52D37AF9A8CA9AA279B4CC1679F3A89D3EAA56F972564DD78B7CD93E0ADB3F42471ED8F3A2CCB7A9597D9DBAAEF259916DB7D7ABFCB38FB2D5AA2CA6594B48DEBD5CCEC6DCD3D8EF60DCE6EFDA8FDCDBE7EBB2DC5E65EDFCB38FEE7E94DE3D7A7CB73786A3FF07504B01021400140002000800976C904258470FF02F020000B20400000B0000000000000000000000000000000000636F6E74656E742E786D6C504B01021400140002000800976C9042D5F05C2BBA020000F80500000A00000000000000000000000000580200007374796C65732E786D6C504B01021400140002000800976C90425CC0E181050100000001000015000000000000000000000000003A0500004D4554412D494E462F6D616E69666573742E786D6C504B05060000000003000300B4000000720600000000";
                char[] charArray = varBinary.ToCharArray();
                byte[] byteArray = new byte[charArray.Length];

                QCTH["tnc"] = byteArray;
            }

          
        }


        private void btn_Voucher_Reports_Click(object sender, EventArgs e)
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];

            if (qcth["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO)
            {
                if (BizValidate.CheckRowState(dbaccess.DataSet, "qcth//qct1/qct3"))
                {
                 
                    ReportLists.Reports ReportForm = new ATL.ReportLists.Reports(false, "QCT", "QCT", qcth["refnum"].ToString());

                    ReportForm.ShowDialog();
                }
            }
        }

        private void Initialise()
        {
            DataTable xqct10 = this.dbaccess.DataSet.Tables["qct10"].Copy();

            if (!this.dbaccess.DataSet.Tables.Contains("xqct10"))
            {
                xqct10.TableName = "xqct10";
                this.dbaccess.DataSet.Tables.Add(xqct10.Copy());
            }

            qct1_ttlamt = BizXmlReader.CurrentInstance.GetControl(manpowerFormName,"qct1_ttlamt") as TextBox;
            qct3_ttlamt = BizXmlReader.CurrentInstance.GetControl(equipmentcostsFormName, "qct3_ttlamt") as TextBox;           
            qct5_ttlamt = BizXmlReader.CurrentInstance.GetControl(materialFormName, "qct5_ttlamt") as TextBox;
            qct6_ttlamt = BizXmlReader.CurrentInstance.GetControl(periodicFormName, "qct6_ttlamt") as TextBox;
            qct7_ttlamt = BizXmlReader.CurrentInstance.GetControl(machineryFormName, "qct7_ttlamt") as TextBox;
            qct8_ttlamt = BizXmlReader.CurrentInstance.GetControl(toiletriesFormName, "qct8_ttlamt") as TextBox;
            qct9_ttlamt = BizXmlReader.CurrentInstance.GetControl(chemicalFormName, "qct9_ttlamt") as TextBox;
            qcth_arnum = BizXmlReader.CurrentInstance.GetControl(headerFormName, "qcth_arnum") as TextBox;

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

           
            qct4_btnAdd = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "qct4_btnAdd") as Button;
            qct4_btnAdd.Click +=new EventHandler(qct4_btnAdd_Click);
            qct4_btnUpdate = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "qct4_btnUpdate") as Button;
            qct4_btnUpdate.Click +=new EventHandler(qct4_btnUpdate_Click);

            dgCtr4 = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "dg_feedback") as DataGrid;
            dgCtr4.MouseDoubleClick +=new MouseEventHandler(dgtxt_MouseDoubleClick);

            qct10_btnBrowseEdu = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "qct10_btnBrowseEdu") as Button;
            qct10_btnBrowseEdu.Click +=new EventHandler(qct10_btnBrowseEdu_Click);
            
            qct10_btnDownloadEdu = BizXmlReader.CurrentInstance.GetControl(archiveFormName, "qct10_btnDownloadEdu") as Button;
            qct10_btnDownloadEdu.Click += new EventHandler(qct10_btnDownloadEdu_Click);

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
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];
            DataTable qct1 = this.dbaccess.DataSet.Tables["qct1"];

            //foreach (DataRow dr1 in qct1.Rows)
            //{
            //    if (dr1.RowState != DataRowState.Deleted)
            //    {
            //        if (BizFunctions.IsEmpty(dr1["monday"]))
            //        {
            //            dr1["monday"] = 0;
            //        }

            //        if (BizFunctions.IsEmpty(dr1["tuesday"]))
            //        {
            //            dr1["tuesday"] = 0;
            //        }

            //        if (BizFunctions.IsEmpty(dr1["wednesday"]))
            //        {
            //            dr1["wednesday"] = 0;
            //        }

            //        if (BizFunctions.IsEmpty(dr1["thursday"]))
            //        {
            //            dr1["thursday"] = 0;
            //        }

            //        if (BizFunctions.IsEmpty(dr1["friday"]))
            //        {
            //            dr1["friday"] = 0;
            //        }

            //        if (BizFunctions.IsEmpty(dr1["saturday"]))
            //        {
            //            dr1["saturday"] = 0;
            //        }

            //        if (BizFunctions.IsEmpty(dr1["sunday"]))
            //        {
            //            dr1["sunday"] = 0;
            //        }

            //        if (BizFunctions.IsEmpty(dr1["ispubhol"]))
            //        {
            //            dr1["ispubhol"] = 0;
            //        }
            //    }
            //}

            //GetManPowerAmt();
  
            //GetTotalBill();
      
           
  
        }


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            DataRow qcth = dbaccess.DataSet.Tables["qcth"].Rows[0];
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "qcth_arnum":
                    //e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);

                    if (qcth_arnum.Text != string.Empty)
                    {
                        //e.DefaultCondition = "(arm.arnum like '%" + qcth["arnum"].ToString().Trim() + "%' OR arm.arname like '" + qcth["arnum"].ToString().Trim() + "%') and arm.[status]<>'V'";
                        e.Condition = BizFunctions.F2Condition("arnum,arname", (sender as TextBox).Text);
                    }
                    break;

                case "qcth_sitenum":
                    if (!BizFunctions.IsEmpty(qcth["arnum"]))
                    {
                        e.Condition = BizFunctions.F2Condition("sitenum,sitename", (sender as TextBox).Text);
                        //e.DefaultCondition = "(sitm.sitenum like '" + qcth["sitenum"].ToString() + "%' OR sitm.sitename like '" + qcth["sitenum"].ToString() + "%') and sitm.[status]<>'V'";
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
            DataRow qcth = dbaccess.DataSet.Tables["qcth"].Rows[0];
            switch (e.ControlName)
            {
                // later...
                case "qcth_sitenum":
                    if (!BizFunctions.IsEmpty(qcth["sitenum"].ToString()))
                    {
                        string GetSitenumI = "Select * from sitm where sitenum='" + qcth["sitenum"] + "'";
                        this.dbaccess.ReadSQL("SITM", GetSitenumI);
                        DataTable sitm = this.dbaccess.DataSet.Tables["SITM"];
                        if (sitm.Rows.Count > 0)
                        {
                            DataRow drSitmi = sitm.Rows[0];
                            qcth["sitename"] = drSitmi["sitename"];
                            qcth["sectorcode"] = drSitmi["sectorcode"];
                            qcth["addr1"] = drSitmi["addr1"];
                            qcth["addr2"] = drSitmi["addr2"];
                            qcth["addr3"] = drSitmi["addr3"];
                            qcth["country"] = drSitmi["country"];
                            qcth["postalcode"] = drSitmi["postalcode"];
                            qcth["billadd1"] = drSitmi["billadd1"];
                            qcth["billadd2"] = drSitmi["billadd2"];
                            qcth["billadd3"] = drSitmi["billadd3"];
                            qcth["rep1"] = drSitmi["rep1"];
                            qcth["tel1"] = drSitmi["tel1"];
                            qcth["fax"] = drSitmi["fax"];
                            qcth["rep2"] = drSitmi["rep2"];
                            qcth["tel2"] = drSitmi["tel2"];
                            qcth["rep1tel"] = drSitmi["rep1tel"];
                            qcth["rep2tel"] = drSitmi["rep2tel"];
                            qcth["prmcode"] = drSitmi["prmcode"];
                        
                        }
                    }
                    break;

                case "qcth_arnum":
                    {
                        //qcth["arname"] = e.F2CurrentRow["arname"];
                        //qcth["phone"] = e.F2CurrentRow["phone"];
                        //qcth["hp"] = e.F2CurrentRow["hp"];
                        //qcth["fax"] = e.F2CurrentRow["fax"];
                        //qcth["email"] = e.F2CurrentRow["email"];
                        //qcth["ptc"] = e.F2CurrentRow["ptc"];

                        qcth["billadd1"] = e.F2CurrentRow["baddr1"];
                        qcth["billadd2"] = e.F2CurrentRow["baddr2"];
                        qcth["billadd3"] = e.F2CurrentRow["baddr3"];
                        qcth["billadd4"] = e.F2CurrentRow["baddr4"];
                    }
                    break;



            }
        }

        #endregion

        #region DetailF2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            //switch (e.MappingName)
            //{
               
            //}
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow qcth = dbaccess.DataSet.Tables["qcth"].Rows[0];
            DataTable qct1 = dbaccess.DataSet.Tables["qct1"];

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
                        if (e.TableName != "qct1")
                        {
                            e.CurrentRow["estmamt"] = GetLatestMatnumCost(e.F2CurrentRow["matnum"].ToString());
                        }
                                             
                    }
                    break;

                case "frequencycode":
                    {
                        if (e.TableName == "qct6")
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
            e.Handle = false;


        }
        #endregion

        #endregion

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];
            DataTable qct1 = this.dbaccess.DataSet.Tables["qct1"];
            DataTable qct10 = this.dbaccess.DataSet.Tables["qct10"];
            //DataTable xqct10 = null;

            DataTable xqct10 = qct10.GetChanges(DataRowState.Deleted);

            if (BizFunctions.IsEmpty(qcth["trandate"]))
            {
                qcth["trandate"] = DateTime.Now;
            }


            #region Personal Files/Docs Record

            if (qct10.Rows.Count > 0)
            {
                try
                {

                    string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                    if (DriveLetter.Trim() != "")
                    {

                        foreach (DataRow dr11 in qct10.Rows)
                        {

                            if (dr11.RowState != DataRowState.Deleted)
                            {
                                BizFunctions.UpdateDataRow(qcth, dr11, "refnum/user/flag/status/created/modified");

                                if (BizFunctions.IsEmpty(dr11["physicalserverlocation"]))
                                {
                                    FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), qcth["arnum"].ToString(), Convert.ToDateTime(qcth["commencedate"]), dr11["flname"].ToString());
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
                                        FileSendGet2 fsg1 = new FileSendGet2(DriveLetter.Trim(), dr11["templocation"].ToString(), qcth["arnum"].ToString(), Convert.ToDateTime(qcth["commencedate"]), dr11["flname"].ToString());
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

            if (xqct10 != null)
            {

                if (this.dbaccess.DataSet.Tables.Contains("xqct10"))
                {
                    this.dbaccess.DataSet.Tables.Remove("xqct10");
                    xqct10 = qct10.GetChanges(DataRowState.Deleted);

                    xqct10.TableName = "xqct10";

                    this.dbaccess.DataSet.Tables.Add(xqct10);
                }
                else
                {
                    xqct10.TableName = "xqct10";

                    this.dbaccess.DataSet.Tables.Add(xqct10);
                }
            }


        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataTable xqct10 = this.dbaccess.DataSet.Tables["xqct10"];

            #region  Make Save Changes in Education Doc Files

            if (xqct10 != null)
            {
                try
                {

                    string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                    if (DriveLetter1.Trim() != "")
                    {
                        if (xqct10.Rows.Count > 0)
                        {

                            foreach (DataRow dr1 in xqct10.Rows)
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
                    BizFunctions.DeleteAllRows(xqct10);

                    if (this.dbaccess.DataSet.Tables.Contains("xqct10"))
                    {
                        this.dbaccess.DataSet.Tables.Remove("xqct10");                       
                    }
                }


                NetworkDrive.DisconnectNetworkDrive(true);
            }
            #endregion
    
        }

        # endregion

        #region Refresh on Click

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
           
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
            if (budgetFlag)
            {
                Refresh_Budget();
            }
            
            Refresh_Budget();
            
            #endregion
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

            DataRow qcth = dbaccess.DataSet.Tables["qcth"].Rows[0];
            if (qcth["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "qcth//qct1"))
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

            DataRow qcth = e.DBAccess.DataSet.Tables["adh"].Rows[0];
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
            DataRow qcth = dbaccess.DataSet.Tables["qcth"].Rows[0];
            if (qcth["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
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
        private void Voucher_QCTH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow QCTH = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];

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
        private void Voucher_QCT1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable qct1 = dbaccess.DataSet.Tables["qct1"];
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
        private void Voucher_QCT3_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable qct3 = dbaccess.DataSet.Tables["qct3"];
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
        private void Voucher_QCT5_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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

        private void Voucher_QCT6_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable qct6 = dbaccess.DataSet.Tables["qct6"];
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
        private void Voucher_QCT7_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
        private void Voucher_QCT8_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
        private void Voucher_QCT9_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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

        #region Other Methods / Functions

        //private void GetTotalBill()
        //{
         
        //    DataRow QCTH = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];

        //    if (BizFunctions.IsEmpty(QCTH["subtotal"]))
        //    {
        //        QCTH["subtotal"] = 0;
        //    }
        //    //if (BizFunctions.IsEmpty(QCTH["additionalcosts"]))
        //    //{
        //    //    QCTH["additionalcosts"] = 0;
        //    //}
        //    if (BizFunctions.IsEmpty(QCTH["discamt"]))
        //    {
        //        QCTH["discamt"] = 0;
        //    }
        //    //if (BizFunctions.IsEmpty(QCTH["equipmentcosts"]))
        //    //{
        //    //    QCTH["equipmentcosts"] = 0;
        //    //}

        //    //QCTH["ttlbillingamt"] = (Convert.ToDecimal(QCTH["subtotal"]) + Convert.ToDecimal(QCTH["additionalcosts"]) + Convert.ToDecimal(QCTH["equipmentcosts"])) - Convert.ToDecimal(QCTH["discamt"]);
        //    QCTH["ttlbillingamt"] = Convert.ToDecimal(QCTH["subtotal"]) - Convert.ToDecimal(QCTH["discamt"]);

            
        //}

        private void GetManPowerAmt()
        {
            decimal totalamt = 0;
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct1 = dbaccess.DataSet.Tables["qct1"];
            foreach (DataRow dr1 in qct1.Rows)
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
            qcth["bgtsactsal"] = totalamt;
            qct1_ttlamt.Text = totalamt.ToString();            
        }

        private void GetMaterialCost()
        {
            decimal totalamt = 0;
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct5 = dbaccess.DataSet.Tables["qct5"];
            foreach (DataRow dr1 in qct5.Rows)
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
            qcth["bgtsactmat"] = totalamt;
            qct5_ttlamt.Text = totalamt.ToString();
        }

        private void GetChemicalCost()
        {
            decimal totalamt = 0;
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct9 = dbaccess.DataSet.Tables["qct9"];
            if (qct9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
                qcth["bgtsactchem"] = totalamt;
                qct9_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetMachineryCost()
        {
            decimal totalamt = 0;
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct7 = dbaccess.DataSet.Tables["qct7"];
            if (qct7.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct7.Rows)
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
                qcth["bgtsactmach"] = totalamt;
                qct7_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetToiletryCost()
        {
            decimal totalamt = 0;
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct8 = dbaccess.DataSet.Tables["qct8"];
            if (qct8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
                qcth["bgtsactToi"] = totalamt;
                qct8_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetEquipmentCost()
        {
            decimal totalamt = 0;
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct3 = dbaccess.DataSet.Tables["qct3"];
            if (qct3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct3.Rows)
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
                qcth["bgtsactequip"] = totalamt;
                qct3_ttlamt.Text = totalamt.ToString();
            }
        }

        private void GetPeriodScheduleCost()
        {
            decimal totalamt = 0;
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct6 = dbaccess.DataSet.Tables["qct6"];
            if (qct6.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct6.Rows)
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
                qcth["bgtsactPrd"] = totalamt;
                qct6_ttlamt.Text = totalamt.ToString();
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
            DataRow QCTH = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct1 = dbaccess.DataSet.Tables["qct1"];
            string sqlCount = "select SUM(officerqty) as Oqty from [qct1]";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, sqlCount);
            DataRow dr1 = dt1.Rows[0];

            if (dr1["Oqty"] != System.DBNull.Value) 
            {
                QCTH["officerqty"] = dr1["Oqty"];
            }
            else
            {
                QCTH["officerqty"] = 0;
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
            DataSet ds1 = new DataSet("QCTds1");
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];
            DataTable qct1 = this.dbaccess.DataSet.Tables["qct1"];
            DataTable qct3 = this.dbaccess.DataSet.Tables["qct1"];


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
                                "QCT1  " +
                                "GROUP BY matnum " +
                                ") " +
                                "B ON A2.MATNUM=B.MATNUM";

            DataTable QCTH = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "SELECT * FROM QCTH");
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

            //string getARM = "SELECT * FROM ARM WHERE arnum='" + qcth["arnum"].ToString() + "'";

            //this.dbaccess.ReadSQL("ARMtmp", getARM);

            //DataTable ARMtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getARM);


            //////////////////////////////////////

            string getARM = "SELECT * FROM ARM WHERE arnum='" + qcth["arnum"].ToString() + "'";
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


            if (ds1.Tables.Contains("QCTH"))
            {
                ds1.Tables["QCTH"].Dispose();
                ds1.Tables.Remove("QCTH");
                QCTH.TableName = "QCTH";
                ds1.Tables.Add(QCTH);
            }
            else
            {
                QCTH.TableName = "QCTH";
                ds1.Tables.Add(QCTH);
            }

            if (ds1.Tables.Contains("QCT1"))
            {
                ds1.Tables["QCT1"].Dispose();
                ds1.Tables.Remove("QCT1");
                CTR1.TableName = "QCT1";
                ds1.Tables.Add(CTR1);
            }
            else
            {
                CTR1.TableName = "QCT1";
                ds1.Tables.Add(CTR1);
            }

            if (ds1.Tables.Contains("QCT12"))
            {
                ds1.Tables["QCT12"].Dispose();
                ds1.Tables.Remove("QCT12");
                CTR12.TableName = "QCT12";
                ds1.Tables.Add(CTR12);
            }
            else
            {
                CTR12.TableName = "QCT12";
                ds1.Tables.Add(CTR12);
            }

            if (ds1.Tables.Contains("QCT3"))
            {
                ds1.Tables["QCT3"].Dispose();
                ds1.Tables.Remove("QCT3");
                CTR3.TableName = "QCT3";
                ds1.Tables.Add(CTR3);
            }
            else
            {
                CTR3.TableName = "QCT3";
                ds1.Tables.Add(CTR3);
            }


            return ds1;

        }

        #region Refresh Functions

        #region Refresh Header

        private void Refresh_Header()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            if (BizFunctions.IsEmpty(qcth["commencedate"]) || BizFunctions.IsEmpty(qcth["enddate"]))
            {
                qcth["commencedate"] = DateTime.Now;
                qcth["enddate"] = DateTime.Now;
            }
            qcth["totalmonths"] = ATL.TimeUtilites.TimeTools.MonthDiff(Convert.ToDateTime(qcth["commencedate"]), Convert.ToDateTime(qcth["enddate"]));
            RefreshMonthNo();
            headerFlag = false;
        }

        #endregion

        #region Refresh Budget

        private void Refresh_Budget()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct1 = this.dbaccess.DataSet.Tables["QCT1"];
            DataTable qct3 = this.dbaccess.DataSet.Tables["QCT3"];
            DataTable qct5 = this.dbaccess.DataSet.Tables["QCT5"];
            DataTable qct6 = this.dbaccess.DataSet.Tables["QCT6"];
            DataTable qct7 = this.dbaccess.DataSet.Tables["QCT7"];
            DataTable qct8 = this.dbaccess.DataSet.Tables["QCT8"];
            DataTable qct9 = this.dbaccess.DataSet.Tables["QCT9"];

            
            #region Manual Actual
            if (Convert.ToDecimal(qcth["bgtActMctr"]) <= 0)
            {
                qcth["bgtActMctr"] = qcth["bgtestctr"];
            }
            if (Convert.ToDecimal(qcth["bgtActMctrldsp"]) <= 0)
            {
                qcth["bgtActMctrldsp"] = qcth["bgtestctrldsp"];
            }
            if (Convert.ToDecimal(qcth["bgtActMLandSub"]) <= 0)
            {
                qcth["bgtActMLandSub"] = qcth["bgtestLandSub"];
            }
            if (Convert.ToDecimal(qcth["bgtActMsal"]) <= 0)
            {
                qcth["bgtActMsal"] = qcth["bgtestsal"];
            }
            if (Convert.ToDecimal(qcth["bgtActMmat"]) <= 0)
            {
                qcth["bgtActMmat"] = qcth["bgtestmat"];
            }
            if (Convert.ToDecimal(qcth["bgtActMchem"]) <= 0)
            {
                qcth["bgtActMchem"] = qcth["bgtestchem"];
            }
            if (Convert.ToDecimal(qcth["bgtActMmach"]) <= 0)
            {
                qcth["bgtActMmach"] = qcth["bgtestmach"];
            }
            if (Convert.ToDecimal(qcth["bgtActMequip"]) <= 0)
            {
                qcth["bgtActMequip"] = qcth["bgtestequip"];
            }
            if (Convert.ToDecimal(qcth["bgtActMToi"]) <= 0)
            {
                qcth["bgtActMToi"] = qcth["bgtestToi"];
            }
            if (Convert.ToDecimal(qcth["bgtActMPrd"]) <= 0)
            {
                qcth["bgtActMPrd"] = qcth["bgtestPrd"];
            }
            if (Convert.ToDecimal(qcth["bgtActMLand"]) <= 0)
            {
                qcth["bgtActMLand"] = qcth["bgtestLand"];
            }
            if (Convert.ToDecimal(qcth["bgtActMSub"]) <= 0)
            {
                qcth["bgtActMSub"] = qcth["bgtestSub"];
            }
            if (Convert.ToDecimal(qcth["bgtActMOther"]) <= 0)
            {
                qcth["bgtActMOther"] = qcth["bgtEstOther"];
            }
            #endregion End Manual Actual

            #region System Actual
            //if (Convert.ToDecimal(qcth["bgtsactctr"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMctr"]) > 0)
                {
                    qcth["bgtsactctr"] = qcth["bgtActMctr"];
                }
                else
                {
                    qcth["bgtsactctr"] = qcth["bgtestctr"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtsactctrldsp"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMctrldsp"]) > 0)
                {
                    qcth["bgtsactctrldsp"] = qcth["bgtActMctrldsp"];
                }
                else
                {
                    qcth["bgtsactctrldsp"] = qcth["bgtestctrldsp"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtsactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLandSub"]) > 0)
                {
                    qcth["bgtsactLandSub"] = qcth["bgtActMLandSub"];
                }
                else
                {
                    qcth["bgtsactLandSub"] = qcth["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtsactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLand"]) > 0)
                {
                qcth["bgtsactLand"] = qcth["bgtActMLand"];
                }
                else
                {
                    qcth["bgtsactLand"] = qcth["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtsactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMSub"]) > 0)
                {
                    qcth["bgtsactSub"] = qcth["bgtActMSub"];
                }
                else
                {
                    qcth["bgtsactSub"] = qcth["bgtestSub"];
                }

            //}
            //if (Convert.ToDecimal(qcth["bgtsactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMOther"]) > 0)
                {
                qcth["bgtsactOther"] = qcth["bgtActMOther"];
                }
                else
                {
                    qcth["bgtsactOther"] = qcth["bgtestOther"];
                }
            //}
            #endregion End System Actual

            #region Actual
            //if (Convert.ToDecimal(qcth["bgtactctr"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMctr"]) > 0)
                {
                    qcth["bgtactctr"] = qcth["bgtActMctr"];
                }
                else
                {
                    qcth["bgtactctr"] = qcth["bgtestctr"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactctrldsp"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMctrldsp"]) > 0)
                {
                    qcth["bgtactctrldsp"] = qcth["bgtActMctrldsp"];
                }
                else
                {
                    qcth["bgtactctrldsp"] = qcth["bgtestctrldsp"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactsal"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMsal"]) > 0)
                {
                    qcth["bgtactsal"] = qcth["bgtActMsal"];
                }
                else
                {
                    qcth["bgtactsal"] = qcth["bgtestsal"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactmat"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMmat"]) > 0)
                {
                    qcth["bgtactmat"] = qcth["bgtActMmat"];
                }
                else
                {
                    qcth["bgtactmat"] = qcth["bgtestmat"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactchem"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMchem"]) > 0)
                {
                    qcth["bgtactchem"] = qcth["bgtActMchem"];
                }
                else
                {
                    qcth["bgtactchem"] = qcth["bgtestchem"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactmach"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMmach"]) > 0)
                {
                    qcth["bgtactmach"] = qcth["bgtActMmach"];
                }
                else
                {
                    qcth["bgtactmach"] = qcth["bgtestmach"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactequip"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMequip"]) > 0)
                {
                    qcth["bgtactequip"] = qcth["bgtActMequip"];
                }
                else
                {
                    qcth["bgtactequip"] = qcth["bgtestequip"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactToi"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMToi"]) > 0)
                {
                    qcth["bgtactToi"] = qcth["bgtActMToi"];
                }
                else
                {
                    qcth["bgtactToi"] = qcth["bgtestToi"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactPrd"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMPrd"]) > 0)
                {
                    qcth["bgtactPrd"] = qcth["bgtActMPrd"];
                }
                else
                {
                    qcth["bgtactPrd"] = qcth["bgtestPrd"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMOther"]) > 0)
                {
                    qcth["bgtactOther"] = qcth["bgtActMOther"];
                }
                else
                {
                    qcth["bgtactOther"] = qcth["bgtestOther"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMSub"]) > 0)
                {
                    qcth["bgtactSub"] = qcth["bgtActMSub"];
                }
                else
                {
                    qcth["bgtactSub"] = qcth["bgtestSub"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLand"]) > 0)
                {
                    qcth["bgtactLand"] = qcth["bgtActMLand"];
                }
                else
                {
                    qcth["bgtactLand"] = qcth["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLandSub"]) > 0)
                {
                    qcth["bgtactLandSub"] = qcth["bgtActMLandSub"];
                }
                else
                {
                    qcth["bgtactLandSub"] = qcth["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLandSub"]) > 0)
                {
                    qcth["bgtactLandSub"] = qcth["bgtActMLandSub"];
                }
                else
                {
                    qcth["bgtactLandSub"] = qcth["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLand"]) > 0)
                {
                    qcth["bgtactLand"] = qcth["bgtActMLand"];
                }
                else
                {
                    qcth["bgtactLand"] = qcth["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMSub"]) > 0)
                {
                    qcth["bgtactSub"] = qcth["bgtActMSub"];
                }
                else
                {
                    qcth["bgtactSub"] = qcth["bgtestSub"];
                }

            //}
            //if (Convert.ToDecimal(qcth["bgtactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMOther"]) > 0)
                {
                    qcth["bgtactOther"] = qcth["bgtActMOther"];
                }
                else
                {
                    qcth["bgtactOther"] = qcth["bgtestOther"];
                }
            //}
            #endregion End Actual

            #region Latest Actual
            //if (Convert.ToDecimal(qcth["bgtlactctr"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMctr"]) > 0)
                {
                    qcth["bgtlactctr"] = qcth["bgtActMctr"];
                }
                else
                {
                    qcth["bgtlactctr"] = qcth["bgtestctr"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtlactctrldsp"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMctrldsp"]) > 0)
                {
                    qcth["bgtlactctrldsp"] = qcth["bgtActMctrldsp"];
                }
                else
                {
                    qcth["bgtlactctrldsp"] = qcth["bgtestctrldsp"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtlactsal"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMsal"]) > 0)
                {
                    qcth["bgtlactsal"] = qcth["bgtActMsal"];
                }
                else
                {
                    qcth["bgtlactsal"] = qcth["bgtestsal"];
                }
            //}

                #region Take the latest cost from matm for each item from the Detailed Costing

                if (Convert.ToDecimal(qcth["totalmonths"]) > 0)
                {
                    qcth["bgtlactmat"] = GetTotalMatnumCost(qct5, "matnum") / Convert.ToDecimal(qcth["totalmonths"]);
                    qcth["bgtlactchem"] = GetTotalMatnumCost(qct9, "matnum") / Convert.ToDecimal(qcth["totalmonths"]);
                    qcth["bgtlactmach"] = GetTotalMatnumCost(qct7, "matnum") / Convert.ToDecimal(qcth["totalmonths"]);
                    qcth["bgtlactequip"] = GetTotalMatnumCost(qct3, "matnum") / Convert.ToDecimal(qcth["totalmonths"]);
                    qcth["bgtlactToi"] = GetTotalMatnumCost(qct8, "matnum") / Convert.ToDecimal(qcth["totalmonths"]);
                }

                #endregion

            //if (Convert.ToDecimal(qcth["bgtlactLandSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLandSub"]) > 0)
                {
                    qcth["bgtlactLandSub"] = qcth["bgtActMLandSub"];
                }
                else
                {
                    qcth["bgtlactLandSub"] = qcth["bgtestLandSub"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtlactLand"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMLand"]) > 0)
                {
                    qcth["bgtlactLand"] = qcth["bgtActMLand"];
                }
                else
                {
                    qcth["bgtlactLand"] = qcth["bgtestLand"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtlactSub"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMSub"]) > 0)
                {
                    qcth["bgtlactSub"] = qcth["bgtActMSub"];
                }
                else
                {
                    qcth["bgtlactSub"] = qcth["bgtestSub"];
                }

            //}
            //if (Convert.ToDecimal(qcth["bgtlactPrd"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMPrd"]) > 0)
                {
                    qcth["bgtlactPrd"] = qcth["bgtActMPrd"];
                }
                else
                {
                    qcth["bgtlactPrd"] = qcth["bgtestPrd"];
                }
            //}
            //if (Convert.ToDecimal(qcth["bgtlactOther"]) <= 0)
            //{
                if (Convert.ToDecimal(qcth["bgtActMOther"]) > 0)
                {
                    qcth["bgtlactOther"] = qcth["bgtActMOther"];
                }
                else
                {
                    qcth["bgtlactOther"] = qcth["bgtestOther"];
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
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct1 = this.dbaccess.DataSet.Tables["QCT1"];
            decimal totalamt = 0;

            if (BizFunctions.IsEmpty(qcth["wkdaysmth"]))
            {
                qcth["wkdaysmth"] = 0;
            }

            if (Convert.ToDecimal(qcth["wkdaysmth"]) > 0)
            {

                if (qct1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in qct1.Rows)
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
                            else
                            {
                                dr1["rate"] = (Convert.ToDecimal(dr1["actmamt"]) / Convert.ToDecimal(dr1["officerqty"]));
                            }

                            decimal totalHrs = 0;
                            int days = 0;

                            if (!BizFunctions.IsEmpty(dr1["xday1"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday1"].ToString());
                                days = days + isWorkShift(dr1["xday1"].ToString());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday2"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday2"].ToString());
                                days = days + isWorkShift(dr1["xday2"].ToString());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday3"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday3"].ToString());
                                days = days + isWorkShift(dr1["xday3"].ToString());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday4"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday4"].ToString());
                                days = days + isWorkShift(dr1["xday4"].ToString());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday5"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday5"].ToString());
                                days = days + isWorkShift(dr1["xday5"].ToString());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday6"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday6"].ToString());
                                days = days + isWorkShift(dr1["xday6"].ToString());
                            }
                            if (!BizFunctions.IsEmpty(dr1["xday7"]))
                            {
                                totalHrs = totalHrs + GetShiftHrs(dr1["xday7"].ToString());
                                days = days + isWorkShift(dr1["xday7"].ToString());
                            }

                            dr1["hperd"] = totalHrs;
                            dr1["dperw"] = days;





                            dr1["oriamt"] = Convert.ToDecimal(dr1["officerqty"]) * Convert.ToDecimal(dr1["actmamt"]);


                            dr1["hcost"] = Convert.ToDecimal(dr1["oriamt"]) / (Convert.ToDecimal(qcth["wkdaysmth"]) * (totalHrs / Convert.ToDecimal(days)));


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


                qcth["bgtsactsal"] = totalamt;
                qct1_ttlamt.Text = totalamt.ToString();
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
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct7 = this.dbaccess.DataSet.Tables["QCT7"];
            decimal totalamt = 0;

            if (qct7.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct7.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
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
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) == 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) == 0)
                        {
                            dr1["price"] = 0;
                        }
                        else
                        {
                            dr1["price"] = Convert.ToDecimal(dr1["actmamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        }
                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["price"]);

                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
            }
            qcth["bgtsactmach"] = totalamt;
            qct7_ttlamt.Text = totalamt.ToString();
            manpowerFlag = false;
        }

        #endregion

        #region Refresh Equipment

        private void Refresh_Equipment()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct3 = this.dbaccess.DataSet.Tables["QCT3"];
            decimal totalamt = 0;

            if (qct3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct3.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = qcth["totalmonths"];
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }
                        if (BizFunctions.IsEmpty(dr1["qty"]))
                        {
                            dr1["qty"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) == 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) == 0)
                        {
                            dr1["price"] = 0;
                        }
                        else
                        {
                            dr1["price"] = Convert.ToDecimal(dr1["actmamt"]) / Convert.ToDecimal(dr1["mthnum"]);
                        }
                        dr1["oriamt"] = Convert.ToDecimal(dr1["qty"]) * Convert.ToDecimal(dr1["price"]);

                        totalamt = totalamt + (decimal)dr1["oriamt"];
                    }
                }
            }
            qcth["bgtsactequip"] = totalamt;
            qct3_ttlamt.Text = totalamt.ToString();
            equipmentFlag = false;
        }

        #endregion

        #region Refresh Material

        private void Refresh_Material()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct5 = this.dbaccess.DataSet.Tables["QCT5"];
            decimal totalamt = 0;

            if (qct5.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct5.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = qcth["totalmonths"];
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
                    }
                }
            }
            qcth["bgtsactmat"] = totalamt;
            qct5_ttlamt.Text = totalamt.ToString();
            materialFlag = false;
        }

        #endregion

        #region Refresh Toiletries

        private void Refresh_Toiletries()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct8 = this.dbaccess.DataSet.Tables["QCT8"];
            decimal totalamt = 0;

            if (qct8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = qcth["totalmonths"];
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
                    }
                }
            }
            qcth["bgtsactToi"] = totalamt;
            qct8_ttlamt.Text = totalamt.ToString();
            toiletriesFlag = false;
        }

        #endregion

        #region Refresh Chemical

        private void Refresh_Chemical()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct9 = this.dbaccess.DataSet.Tables["QCT9"];
            decimal totalamt = 0;

            if (qct9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = qcth["totalmonths"];
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
                    }
                }
            }
            qcth["bgtsactchem"] = totalamt;
            qct9_ttlamt.Text = totalamt.ToString();
            chemicalFlag = false;
        }

        #endregion

        #region Refresh Periodic Schedule

        private void Refresh_Schedule()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct6 = this.dbaccess.DataSet.Tables["QCT6"];
            decimal totalamt = 0;

            if (qct6.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct6.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = qcth["totalmonths"];
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

                        totalamt = totalamt + Convert.ToDecimal(dr1["oriamt"]);

                        if (Convert.ToDecimal(dr1["qty"]) == 12)
                        {
                            dr1["allmonth"] = true;

                        }
                    }
                   
                }
            }
            qcth["bgtsactPrd"] = totalamt;
            qct6_ttlamt.Text = totalamt.ToString();
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
            DataRow qcth = this.dbaccess.DataSet.Tables["QCTH"].Rows[0];
            DataTable qct1 = this.dbaccess.DataSet.Tables["QCT1"];
            DataTable qct3 = this.dbaccess.DataSet.Tables["QCT3"];
            DataTable qct5 = this.dbaccess.DataSet.Tables["QCT5"];
            DataTable qct6 = this.dbaccess.DataSet.Tables["QCT6"];
            DataTable qct7 = this.dbaccess.DataSet.Tables["QCT7"];
            DataTable qct8 = this.dbaccess.DataSet.Tables["QCT8"];
            DataTable qct9 = this.dbaccess.DataSet.Tables["QCT9"];

            if (qct1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in qct1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(qcth["totalmonths"]))
                        {
                            dr1["mthnum"] = qcth["totalmonths"];
                        }
                    }
                }
            }

            if (qct3.Rows.Count > 0)
            {
                foreach (DataRow dr3 in qct3.Rows)
                {
                    if (dr3.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(qcth["totalmonths"]))
                        {
                            dr3["mthnum"] = qcth["totalmonths"];
                        }
                    }
                }
            }

            if (qct5.Rows.Count > 0)
            {
                foreach (DataRow dr5 in qct5.Rows)
                {
                    if (dr5.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(qcth["totalmonths"]))
                        {
                            dr5["mthnum"] = qcth["totalmonths"];
                        }
                    }
                }
            }

          

            if (qct7.Rows.Count > 0)
            {
                foreach (DataRow dr7 in qct7.Rows)
                {
                    if (dr7.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(qcth["totalmonths"]))
                        {
                            dr7["mthnum"] = qcth["totalmonths"];
                        }
                    }
                }
            }

            if (qct8.Rows.Count > 0)
            {
                foreach (DataRow dr8 in qct8.Rows)
                {
                    if (dr8.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(qcth["totalmonths"]))
                        {
                            dr8["mthnum"] = qcth["totalmonths"];
                        }
                    }
                }
            }

            if (qct9.Rows.Count > 0)
            {
                foreach (DataRow dr9 in qct9.Rows)
                {
                    if (dr9.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(qcth["totalmonths"]))
                        {
                            dr9["mthnum"] = qcth["totalmonths"];
                        }
                    }
                }
            }


        }

        #endregion

        #endregion

        #region Feedback Detail Buttons

        #region Feedback Button Events

        private void qct4_btnAdd_Click(object sender, EventArgs e)
        {
            DataTable qct4 = this.dbaccess.DataSet.Tables["QCT4"];
            DataRow InsertCtr4 = qct4.NewRow();
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
                qct4.Rows.Add(InsertCtr4);
                txt_guid = string.Empty;
            }


        }

        private void qct4_btnUpdate_Click(object sender, EventArgs e)
        {
            DataTable qct4 = this.dbaccess.DataSet.Tables["QCT4"];

            txt_Daterasied = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Daterasied") as DateTimePicker;
            txt_Raisedby = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_Raisedby") as TextBox;
            txt_desc = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_desc") as TextBox;
            txt_followup = BizXmlReader.CurrentInstance.GetControl(feedbackFormName, "txt_followup") as TextBox;

            if (qct4.Rows.Count > 0)
            {
                foreach (DataRow dr4 in qct4.Rows)
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

                    string GetCtr4Row = "Select * from qct4 where [guid]='" + drCur["guid"].ToString() + "'";

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

        protected void qct10_btnBrowseEdu_Click(object sender, System.EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaCompress form = new ATL.BizModules.StaCompressFolders.StaCompress(dbaccess, "QCTH", "QCT10", "refnum");
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

        protected void qct10_btnDownloadEdu_Click(object sender, System.EventArgs e)
        {
            try
            {
                ATL.BizModules.StaCompressFolders.StaDecommpress form = new ATL.BizModules.StaCompressFolders.StaDecommpress(dbaccess, "QCTH", "QCT10", "refnum");
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
            WordForm1 wf = new WordForm1(this.dbaccess,"QCTH","tnc","QCT");
            wf.Show();
            wf.Focus();
        }
        #endregion

        private void getBgtEstTotal()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];
           
            decimal grandTotal = Convert.ToDecimal(qcth["bgtestsal"]) + Convert.ToDecimal(qcth["bgtestmat"]) + Convert.ToDecimal(qcth["bgtestchem"]) +
                                 Convert.ToDecimal(qcth["bgtestmach"]) + Convert.ToDecimal(qcth["bgtestequip"]) + Convert.ToDecimal(qcth["bgtestToi"]) + 
                                 Convert.ToDecimal(qcth["bgtestPrd"]) +  Convert.ToDecimal(qcth["bgtestSub"]) + Convert.ToDecimal(qcth["bgtestOther"]);
            txt_grandTotal1.Text = Convert.ToString(grandTotal);
            if (BizFunctions.IsEmpty(qcth["bgtestctr"]))
            {
                qcth["bgtestctr"] = 0;
            }
            txt_grossProfit1.Text = Convert.ToString(Convert.ToDecimal(qcth["bgtestctr"]) - grandTotal);

            decimal grandTotalLsdp = Convert.ToDecimal(qcth["bgtestLand"]) + Convert.ToDecimal(qcth["bgtestLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(qcth["bgtestctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp1.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp1.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtManualActTotal()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(qcth["bgtActMsal"]) + Convert.ToDecimal(qcth["bgtActMmat"]) + Convert.ToDecimal(qcth["bgtActMchem"]) +
                                 Convert.ToDecimal(qcth["bgtActMmach"]) + Convert.ToDecimal(qcth["bgtActMequip"]) + Convert.ToDecimal(qcth["bgtActMToi"]) +
                                 Convert.ToDecimal(qcth["bgtActMPrd"]) + Convert.ToDecimal(qcth["bgtActMSub"]) + Convert.ToDecimal(qcth["bgtActMOther"]);
            txt_grandTotal2.Text = Convert.ToString(grandTotal);
            if (BizFunctions.IsEmpty(qcth["bgtActMctr"]))
            {
                qcth["bgtActMctr"] = 0;
            }
            txt_grossProfit2.Text = Convert.ToString(Convert.ToDecimal(qcth["bgtActMctr"]) - grandTotal);

            decimal grandTotalLsdp = Convert.ToDecimal(qcth["bgtActMLand"]) + Convert.ToDecimal(qcth["bgtActMLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(qcth["bgtActMctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp2.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp2.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtSysActTotal()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(qcth["bgtsactsal"]) + Convert.ToDecimal(qcth["bgtsactmat"]) + Convert.ToDecimal(qcth["bgtsactchem"]) +
                                 Convert.ToDecimal(qcth["bgtsactmach"]) + Convert.ToDecimal(qcth["bgtsactequip"]) + Convert.ToDecimal(qcth["bgtsactToi"]) +
                                 Convert.ToDecimal(qcth["bgtsactPrd"]) + Convert.ToDecimal(qcth["bgtsactSub"]) + Convert.ToDecimal(qcth["bgtsactOther"]);
            txt_grandTotal3.Text = Convert.ToString(grandTotal);
            if (BizFunctions.IsEmpty(qcth["bgtsactctr"]))
            {
                qcth["bgtsactctr"] = 0;
            }
            txt_grossProfit3.Text = Convert.ToString(Convert.ToDecimal(qcth["bgtsactctr"]) - grandTotal);

            decimal grandTotalLsdp = Convert.ToDecimal(qcth["bgtsactLand"]) + Convert.ToDecimal(qcth["bgtsactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(qcth["bgtsactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp3.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp3.Text = Convert.ToString(grosProfitLsdp);
        
        }

        private void getBgtActualTotal()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(qcth["bgtactsal"]) + Convert.ToDecimal(qcth["bgtactmat"]) + Convert.ToDecimal(qcth["bgtactchem"]) +
                                 Convert.ToDecimal(qcth["bgtactmach"]) + Convert.ToDecimal(qcth["bgtactequip"]) + Convert.ToDecimal(qcth["bgtactToi"]) +
                                 Convert.ToDecimal(qcth["bgtactPrd"]) + Convert.ToDecimal(qcth["bgtactSub"]) + Convert.ToDecimal(qcth["bgtactOther"]);
            txt_grandTotal4.Text = Convert.ToString(grandTotal);
            if (BizFunctions.IsEmpty(qcth["bgtactctr"]))
            {
                qcth["bgtactctr"] = 0;
            }
            txt_grossProfit4.Text = Convert.ToString(Convert.ToDecimal(qcth["bgtactctr"]) - grandTotal);

            decimal grandTotalLsdp = Convert.ToDecimal(qcth["bgtactLand"]) + Convert.ToDecimal(qcth["bgtactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(qcth["bgtactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp4.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp4.Text = Convert.ToString(grosProfitLsdp);
        }

        private void getBgtLatestActTotal()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];

            decimal grandTotal = Convert.ToDecimal(qcth["bgtlactsal"]) + Convert.ToDecimal(qcth["bgtlactmat"]) + Convert.ToDecimal(qcth["bgtlactchem"]) +
                                 Convert.ToDecimal(qcth["bgtlactmach"]) + Convert.ToDecimal(qcth["bgtlactequip"]) + Convert.ToDecimal(qcth["bgtlactToi"]) +
                                 Convert.ToDecimal(qcth["bgtlactPrd"]) + Convert.ToDecimal(qcth["bgtlactSub"]) + Convert.ToDecimal(qcth["bgtlactOther"]);
            txt_grandTotal5.Text = Convert.ToString(grandTotal);
            if (BizFunctions.IsEmpty(qcth["bgtlactctr"]))
            {
                qcth["bgtlactctr"] = 0;
            }
            txt_grossProfit5.Text = Convert.ToString(Convert.ToDecimal(qcth["bgtlactctr"]) - grandTotal);

            decimal grandTotalLsdp = Convert.ToDecimal(qcth["bgtlactLand"]) + Convert.ToDecimal(qcth["bgtlactLandSub"]);
            decimal grosProfitLsdp = Convert.ToDecimal(qcth["bgtlactctrldsp"]) - grandTotalLsdp;

            txt_grandTotalLsdp5.Text = Convert.ToString(grandTotalLsdp);
            txt_grossProfitLsdp5.Text = Convert.ToString(grosProfitLsdp);
        }

        #endregion

        private void initiValues()
        {
            DataRow qcth = this.dbaccess.DataSet.Tables["qcth"].Rows[0];

            if (BizFunctions.IsEmpty(qcth["bgtestctr"]))
            {
                qcth["bgtestctr"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestctrldsp"]))
            {
                qcth["bgtestctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestLandSub"]))
            {
                qcth["bgtestLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestsal"]))
            {
                qcth["bgtestsal"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestmat"]))
            {
                qcth["bgtestmat"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestchem"]))
            {
                qcth["bgtestchem"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestmach"]))
            {
                qcth["bgtestmach"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestequip"]))
            {
                qcth["bgtestequip"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestToi"]))
            {
                qcth["bgtestToi"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestPrd"]))
            {
                qcth["bgtestPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestLand"]))
            {
                qcth["bgtestLand"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestSub"]))
            {
                qcth["bgtestSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtestOther"]))
            {
                qcth["bgtestOther"] = 0;
            }

             ///////

            if (BizFunctions.IsEmpty(qcth["bgtActMctr"]))
            {
                qcth["bgtActMctr"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMctrldsp"]))
            {
                qcth["bgtActMctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMLandSub"]))
            {
                qcth["bgtActMLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMsal"]))
            {
                qcth["bgtActMsal"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMmat"]))
            {
                qcth["bgtActMmat"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMchem"]))
            {
                qcth["bgtActMchem"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMmach"]))
            {
                qcth["bgtActMmach"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMequip"]))
            {
                qcth["bgtActMequip"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMToi"]))
            {
                qcth["bgtActMToi"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMPrd"]))
            {
                qcth["bgtActMPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMLand"]))
            {
                qcth["bgtActMLand"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMSub"]))
            {
                qcth["bgtActMSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtActMOther"]))
            {
                qcth["bgtActMOther"] = 0;
            }

            /////////

            if (BizFunctions.IsEmpty(qcth["bgtsactctr"]))
            {
                qcth["bgtsactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactctrldsp"]))
            {
                qcth["bgtsactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactLandSub"]))
            {
                qcth["bgtsactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactsal"]))
            {
                qcth["bgtsactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactmat"]))
            {
                qcth["bgtsactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactchem"]))
            {
                qcth["bgtsactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactmach"]))
            {
                qcth["bgtsactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactequip"]))
            {
                qcth["bgtsactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactToi"]))
            {
                qcth["bgtsactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactPrd"]))
            {
                qcth["bgtsactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactLand"]))
            {
                qcth["bgtsactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactSub"]))
            {
                qcth["bgtsactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtsactOther"]))
            {
                qcth["bgtsactOther"] = 0;
            }

            ////////

            if (BizFunctions.IsEmpty(qcth["bgtactctr"]))
            {
                qcth["bgtactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactctrldsp"]))
            {
                qcth["bgtactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactLandSub"]))
            {
                qcth["bgtactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactsal"]))
            {
                qcth["bgtactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactmat"]))
            {
                qcth["bgtactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactchem"]))
            {
                qcth["bgtactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactmach"]))
            {
                qcth["bgtactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactequip"]))
            {
                qcth["bgtactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactToi"]))
            {
                qcth["bgtactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactPrd"]))
            {
                qcth["bgtactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactLand"]))
            {
                qcth["bgtactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactSub"]))
            {
                qcth["bgtactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtactOther"]))
            {
                qcth["bgtactOther"] = 0;
            }

            /////  

            if(BizFunctions.IsEmpty(qcth["bgtlactctr"]))
            {
                qcth["bgtlactctr"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactctrldsp"]))
            {
                qcth["bgtlactctrldsp"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactLandSub"]))
            {
                qcth["bgtlactLandSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactsal"]))
            {
                qcth["bgtlactsal"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactmat"]))
            {
                qcth["bgtlactmat"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactchem"]))
            {
                qcth["bgtlactchem"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactmach"]))
            {
                qcth["bgtlactmach"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactequip"]))
            {
                qcth["bgtlactequip"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactToi"]))
            {
                qcth["bgtlactToi"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactPrd"]))
            {
                qcth["bgtlactPrd"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactLand"]))
            {
                qcth["bgtlactLand"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactSub"]))
            {
                qcth["bgtlactSub"] = 0;
            }
            if (BizFunctions.IsEmpty(qcth["bgtlactOther"]))
            {
                qcth["bgtlactOther"] = 0;
            }


        }
    }
}
    

