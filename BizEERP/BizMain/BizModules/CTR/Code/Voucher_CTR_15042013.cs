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
using ASOMS.BizModules.TextValidator;
using System.Drawing.Imaging;
using ASOMS.BizModules.StaCompressFolders;
using ASOMS.BizModules.FileAcc;
using ASOMS.Network;
using ASOMS.BizModules.RichTextEdit;

#endregion

namespace ASOMS.CTR
{
    public class Voucher_CTR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        protected string headerFormName,budgetFormName, manpowerFormName,machineryFormName, equipmentcostsFormName,materialFormName,toiletriesFormName,
                         chemicalFormName,periodicFormName,archiveFormName,tacFormName,feedbackFormName,txt_guid = null;
        protected CheckBox ctr_daily, ctr_weekldays, ctr_weekend = null;
        protected TextBox ctr1_ttlamt, ctr3_ttlamt, txt_ttlamt, ctr5_ttlamt, ctr6_ttlamt, ctr7_ttlamt, ctr8_ttlamt, ctr9_ttlamt, ctrh_arnum, txt_grossProfit1, txt_grandTotal1,
                          txt_grossProfit2, txt_grandTotal2,
                          txt_grossProfit3, txt_grandTotal3,
                          txt_grossProfit4, txt_grandTotal4,
                          txt_grossProfit5, txt_grandTotal5,
                           txt_Raisedby, txt_desc, txt_followup = null;
        protected Button btn_Voucher_Reports, ctr4_btnUpdate, ctr4_btnAdd, ctr10_btnBrowseEdu, ctr10_btnDownloadEdu;
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

          
        }


        private void btn_Voucher_Reports_Click(object sender, EventArgs e)
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (ctrh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSO)
            {
                if (BizValidate.CheckRowState(dbaccess.DataSet, "adh/ctr1/ctr3"))
                {
                 
                    ReportLists.Reports ReportForm = new ASOMS.ReportLists.Reports(false, "CTR", "CTR", ctrh["refnum"].ToString());

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
            DataTable ctr1 = this.dbaccess.DataSet.Tables["ctr1"];

            //foreach (DataRow dr1 in ctr1.Rows)
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

                case "ctrh_sitenumi":
                    if (!BizFunctions.IsEmpty(ctrh["arnum"]))
                    {
                        e.Condition = BizFunctions.F2Condition("sitenumi,sitename", (sender as TextBox).Text);
                        //e.DefaultCondition = "(sitmi.sitenumi like '" + ctrh["sitenumi"].ToString() + "%' OR sitmi.sitename like '" + ctrh["sitenumi"].ToString() + "%') and sitmi.[status]<>'V'";
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
                case "ctrh_sitenumi":
                    if (!BizFunctions.IsEmpty(ctrh["sitenumi"].ToString()))
                    {
                        string GetSitenumI = "Select * from sitmi where sitenumi='" + ctrh["sitenumi"] + "'";
                        this.dbaccess.ReadSQL("SITMI", GetSitenumI);
                        DataTable sitmi = this.dbaccess.DataSet.Tables["SITMI"];
                        if (sitmi.Rows.Count > 0)
                        {
                            DataRow drSitmi = sitmi.Rows[0];
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
                        ctrh["billadd1"] = e.F2CurrentRow["baddr1"];
                        ctrh["billadd2"] = e.F2CurrentRow["baddr2"];
                        ctrh["billadd3"] = e.F2CurrentRow["baddr3"];
                        ctrh["billadd4"] = e.F2CurrentRow["baddr4"];
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
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];
            DataTable ctr1 = this.dbaccess.DataSet.Tables["ctr1"];
            DataTable ctr10 = this.dbaccess.DataSet.Tables["ctr10"];


            if (BizFunctions.IsEmpty(ctrh["trandate"]))
            {
                ctrh["trandate"] = DateTime.Now;
            }


            #region Personal Files/Docs Record

            if (ctr10.Rows.Count > 0)
            {
                try
                {

                    string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("PicoDocsRepository"));

                    if (DriveLetter.Trim() != "")
                    {

                        foreach (DataRow dr11 in ctr10.Rows)
                        {

                            if (dr11.RowState != DataRowState.Deleted)
                            {
                                BizFunctions.UpdateDataRow(ctrh, dr11, "refnum/user/flag/status/created/modified");

                                if (BizFunctions.IsEmpty(dr11["physicalserverlocation"]))
                                {
                                    FileSendGet fsg1 = new FileSendGet(DriveLetter.Trim(), dr11["templocation"].ToString(), ctrh["refnum"].ToString(), "CTR");
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
                                        FileSendGet fsg2 = new FileSendGet(DriveLetter.Trim(), dr11["templocation"].ToString(), ctrh["refnum"].ToString(), "CTR");
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


        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            DataTable xctr10 = this.dbaccess.DataSet.Tables["xctr10"];

            #region  Make Save Changes in Education Doc Files

            if (xctr10 != null)
            {
                try
                {

                    string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("PicoDocsRepository"));

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

            DataRow ctrh = dbaccess.DataSet.Tables["ctrh"].Rows[0];
            if (ctrh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "adh/ctr1"))
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

            DataRow ctrh = e.DBAccess.DataSet.Tables["adh"].Rows[0];
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
            DataRow ctrh = dbaccess.DataSet.Tables["ctrh"].Rows[0];
            if (ctrh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
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
        private void Voucher_CTRH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];

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
        private void Voucher_CTR1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ctr1 = dbaccess.DataSet.Tables["ctr1"];
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
        #endregion

        #region Equipment
        private void Voucher_CTR3_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable ctr3 = dbaccess.DataSet.Tables["ctr3"];
            switch (e.Column.ColumnName)
            {
                case "qty":
                    {
                        equipmentFlag = true;
                    }
                    break;

                case "rate":
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
            
            }
        }

        #region Machinery
        private void Voucher_CTR7_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
        private void Voucher_CTR8_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
        private void Voucher_CTR9_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
         
        //    DataRow CTRH = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];

        //    if (BizFunctions.IsEmpty(CTRH["subtotal"]))
        //    {
        //        CTRH["subtotal"] = 0;
        //    }
        //    //if (BizFunctions.IsEmpty(CTRH["additionalcosts"]))
        //    //{
        //    //    CTRH["additionalcosts"] = 0;
        //    //}
        //    if (BizFunctions.IsEmpty(CTRH["discamt"]))
        //    {
        //        CTRH["discamt"] = 0;
        //    }
        //    //if (BizFunctions.IsEmpty(CTRH["equipmentcosts"]))
        //    //{
        //    //    CTRH["equipmentcosts"] = 0;
        //    //}

        //    //CTRH["ttlbillingamt"] = (Convert.ToDecimal(CTRH["subtotal"]) + Convert.ToDecimal(CTRH["additionalcosts"]) + Convert.ToDecimal(CTRH["equipmentcosts"])) - Convert.ToDecimal(CTRH["discamt"]);
        //    CTRH["ttlbillingamt"] = Convert.ToDecimal(CTRH["subtotal"]) - Convert.ToDecimal(CTRH["discamt"]);

            
        //}

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
                            totalAmout = totalAmout + GetLatestMatnumCost(dr1[columnname].ToString());
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

        private DataSet adhds1()
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
            ctrh["totalmonths"] = ASOMS.TimeUtilites.TimeTools.TotelMonthDifference(Convert.ToDateTime(ctrh["commencedate"]), Convert.ToDateTime(ctrh["enddate"]));
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


            #region Manual Actual = Estimate
            if (BizFunctions.IsEmpty(ctrh["bgtActMctr"]))
            {
                ctrh["bgtActMctr"] = ctrh["bgtestctr"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMsal"]))
            {
                ctrh["bgtActMsal"] = ctrh["bgtestsal"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMmat"]))
            {
                ctrh["bgtActMmat"] = ctrh["bgtestmat"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMchem"]))
            {
                ctrh["bgtActMchem"] = ctrh["bgtestchem"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMmach"]))
            {
                ctrh["bgtActMmach"] = ctrh["bgtestmach"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMequip"]))
            {
                ctrh["bgtActMequip"] = ctrh["bgtestequip"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMToi"]))
            {
                ctrh["bgtActMToi"] = ctrh["bgtestToi"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMPrd"]))
            {
                ctrh["bgtActMPrd"] = ctrh["bgtestPrd"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMLand"]))
            {
                ctrh["bgtActMLand"] = ctrh["bgtestLand"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMSub"]))
            {
                ctrh["bgtActMSub"] = ctrh["bgtestSub"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMOther"]))
            {
                ctrh["bgtActMOther"] = ctrh["bgtestOther"];
            }
            #endregion

            #region Sys Actual = Manual Actual
            if(BizFunctions.IsEmpty(ctrh["bgtsactctr"]))
            {
                ctrh["bgtsactctr"] = ctrh["bgtActMctr"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactLand"]))
            {
                ctrh["bgtsactLand"] = ctrh["bgtActMLand"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactSub"]))
            {
                ctrh["bgtsactSub"] = ctrh["bgtActMSub"];
            }
            if (BizFunctions.IsEmpty(ctrh["bgtsactOther"]))
            {
                ctrh["bgtsactOther"] = ctrh["bgtActMOther"];
            }
            #endregion

            #region Actual = Sys Actual
            if(BizFunctions.IsEmpty(ctrh["bgtactctr"]))
            {
                ctrh["bgtactctr"] = ctrh["bgtsactctr"];
            }
            #endregion

            #region If empty(manual Actual) then  Sys Actual else Manual Act
            if (BizFunctions.IsEmpty(ctrh["bgtActMctr"]))
            {
                ctrh["bgtactctr"] = ctrh["bgtsactctr"];
            }
            else
            {
                ctrh["bgtactctr"] = ctrh["bgtActMctr"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMsal"]))
            {
                ctrh["bgtactsal"] = ctrh["bgtsactsal"];
            }
            else
            {
                ctrh["bgtactsal"] = ctrh["bgtActMsal"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMmat"]))
            {
                ctrh["bgtactmat"] = ctrh["bgtsactmat"];
            }
            else
            {
                ctrh["bgtactmat"] = ctrh["bgtActMmat"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMchem"]))
            {
                ctrh["bgtactchem"] = ctrh["bgtsactchem"];
            }
            else
            {
                ctrh["bgtactchem"] = ctrh["bgtActMchem"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMmach"]))
            {
                ctrh["bgtactmach"] = ctrh["bgtsactmach"];
            }
            else
            {
                ctrh["bgtactmach"] = ctrh["bgtActMmach"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMequip"]))
            {
                ctrh["bgtactequip"] = ctrh["bgtsactequip"];
            }
            else
            {
                ctrh["bgtactequip"] = ctrh["bgtActMequip"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMToi"]))
            {
                ctrh["bgtactToi"] = ctrh["bgtsactToi"];
            }
            else
            {
                ctrh["bgtactToi"] = ctrh["bgtActMToi"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMPrd"]))
            {
                ctrh["bgtactPrd"] = ctrh["bgtsactPrd"];
            }
            else
            {
                ctrh["bgtactPrd"] = ctrh["bgtActMPrd"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMLand"]))
            {
                ctrh["bgtactLand"] = ctrh["bgtsactLand"];
            }
            else
            {
                ctrh["bgtactLand"] = ctrh["bgtActMLand"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMSub"]))
            {
                ctrh["bgtactSub"] = ctrh["bgtsactSub"];
            }
            else
            {
                ctrh["bgtactSub"] = ctrh["bgtActMSub"];
            }

            if (BizFunctions.IsEmpty(ctrh["bgtActMOther"]))
            {
                ctrh["bgtactOther"] = ctrh["bgtsactOther"];
            }
            else
            {
                ctrh["bgtactOther"] = ctrh["bgtActMOther"];
            }

            #endregion

            #region Latest Actual
            if (!BizFunctions.IsEmpty(ctrh["bgtactctr"]))
            {
                ctrh["bgtlactctr"] = ctrh["bgtactctr"];
            }
            if (!BizFunctions.IsEmpty(ctrh["bgtActMPrd"]))
            {
                ctrh["bgtlactPrd"] = ctrh["bgtActMPrd"];
            }
            if (!BizFunctions.IsEmpty(ctrh["bgtActMLand"]))
            {
                ctrh["bgtlactLand"] = ctrh["bgtActMLand"];
            }
            if (!BizFunctions.IsEmpty(ctrh["bgtActMSub"]))
            {
                ctrh["bgtlactSub"] = ctrh["bgtActMSub"];
            }
            if (!BizFunctions.IsEmpty(ctrh["bgtActMOther"]))
            {
                ctrh["bgtlactOther"] = ctrh["bgtActMOther"];
            }
            #endregion

            #region Take the latest cost from matm for each item from the Detailed Costing

            ctrh["bgtlactmat"] = GetTotalMatnumCost(ctr5, "matnum");
            ctrh["bgtlactchem"] = GetTotalMatnumCost(ctr9, "matnum");
            ctrh["bgtlactmach"] = GetTotalMatnumCost(ctr7, "matnum");
            ctrh["bgtlactequip"] = GetTotalMatnumCost(ctr3, "matnum");
            ctrh["bgtlactToi"] = GetTotalMatnumCost(ctr8, "matnum");

            #endregion

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

            if (ctr1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
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
            ctrh["bgtsactsal"] = totalamt;
            ctr1_ttlamt.Text = totalamt.ToString();
            manpowerFlag = false;
        }

        #endregion

        #region Refresh Machinery

        private void Refresh_Machinery()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["CTRH"].Rows[0];
            DataTable ctr7 = this.dbaccess.DataSet.Tables["CTR7"];
            decimal totalamt = 0;

            if (ctr7.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr7.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
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
            ctrh["bgtsactmach"] = totalamt;
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

            if (ctr3.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr3.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
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
            ctrh["bgtsactequip"] = totalamt;
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

            if (ctr5.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr5.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["esmtamt"];
                        }
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0 || BizFunctions.IsEmpty(dr1["mthnum"]) || Convert.ToDecimal(dr1["mthnum"]) <= 0)
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
            ctrh["bgtsactmat"] = totalamt;
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

            if (ctr8.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr8.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
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
            ctrh["bgtsactToi"] = totalamt;
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

            if (ctr9.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr9.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
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
            ctrh["bgtsactchem"] = totalamt;
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

            if (ctr6.Rows.Count > 0)
            {
                foreach (DataRow dr1 in ctr6.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //dr1["mthnum"] = ctrh["totalmonths"];
                        if (BizFunctions.IsEmpty(dr1["actmamt"]) || Convert.ToDecimal(dr1["actmamt"]) <= 0)
                        {
                            dr1["actmamt"] = dr1["estmamt"];
                        }

                        totalamt = totalamt + Convert.ToDecimal(dr1["actmamt"]);
                    }
                }
            }
            ctrh["bgtsactPrd"] = totalamt;
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
                        if (BizFunctions.IsEmpty(dr1["mthnum"]))
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
                        if (BizFunctions.IsEmpty(dr3["mthnum"]))
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
                        if (BizFunctions.IsEmpty(dr5["mthnum"]))
                        {
                            dr5["mthnum"] = ctrh["totalmonths"];
                        }
                    }
                }
            }

            //if (ctr6.Rows.Count > 0)
            //{
            //    foreach (DataRow dr6 in ctr6.Rows)
            //    {
            //        if (dr6.RowState != DataRowState.Deleted)
            //        {
            //            if (BizFunctions.IsEmpty(dr6["mthnum"]))
            //            {
            //                dr6["mthnum"] = ctrh["totalmonths"];
            //            }
            //        }
            //    }
            //}

            if (ctr7.Rows.Count > 0)
            {
                foreach (DataRow dr7 in ctr7.Rows)
                {
                    if (dr7.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr7["mthnum"]))
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
                        if (BizFunctions.IsEmpty(dr8["mthnum"]))
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
                        if (BizFunctions.IsEmpty(dr9["mthnum"]))
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
                ASOMS.BizModules.StaCompressFolders.StaCompress form = new ASOMS.BizModules.StaCompressFolders.StaCompress(dbaccess, "CTRH", "CTR10", "refnum");
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
                ASOMS.BizModules.StaCompressFolders.StaDecommpress form = new ASOMS.BizModules.StaCompressFolders.StaDecommpress(dbaccess, "CTRH", "CTR10", "refnum");
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
            WordForm1 wf = new WordForm1(this.dbaccess,"CTRH","tnc");
            wf.Show();
            wf.Focus();
        }
        #endregion

        private void getBgtEstTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

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


            decimal grandTotal = Convert.ToDecimal(ctrh["bgtestsal"]) + Convert.ToDecimal(ctrh["bgtestmat"]) + Convert.ToDecimal(ctrh["bgtestchem"]) +
                                 Convert.ToDecimal(ctrh["bgtestmach"]) + Convert.ToDecimal(ctrh["bgtestequip"]) + Convert.ToDecimal(ctrh["bgtestToi"]) + Convert.ToDecimal(ctrh["bgtestPrd"]) +
                                 Convert.ToDecimal(ctrh["bgtestPrd"]) + Convert.ToDecimal(ctrh["bgtestLand"]) + Convert.ToDecimal(ctrh["bgtestSub"]) + Convert.ToDecimal(ctrh["bgtestOther"]);
            txt_grandTotal1.Text = Convert.ToString(grandTotal);
            if (BizFunctions.IsEmpty(ctrh["bgtestctr"]))
            {
                ctrh["bgtestctr"] = 0;
            }
            txt_grossProfit1.Text = Convert.ToString(Convert.ToDecimal(ctrh["bgtestctr"]) - grandTotal);
        }

        private void getBgtManualActTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if(BizFunctions.IsEmpty(ctrh["bgtActMctr"]))
            {
                ctrh["bgtActMctr"] = 0;
            }

            if(BizFunctions.IsEmpty(ctrh["bgtActMsal"]))
            {
                ctrh["bgtActMsal"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMmat"]))
            {
                ctrh["bgtActMmat"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMchem"]))
            {
                ctrh["bgtActMchem"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMmach"]))
            {
                ctrh["bgtActMmach"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMequip"]))
            {
                ctrh["bgtActMequip"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMToi"]))
            {
                ctrh["bgtActMToi"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMPrd"]))
            {
                ctrh["bgtActMPrd"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMLand"]))
            {
                ctrh["bgtActMLand"] = 0;
            }
            if (BizFunctions.IsEmpty(ctrh["bgtActMSub"]))
            {
                ctrh["bgtActMSub"] = 0;
            }
            if(BizFunctions.IsEmpty(ctrh["bgtActMOther"]))
            {
                ctrh["bgtActMOther"] = 0;
            }

            decimal grandTotal = Convert.ToDecimal(ctrh["bgtActMsal"]) + Convert.ToDecimal(ctrh["bgtActMmat"]) + Convert.ToDecimal(ctrh["bgtActMchem"]) +
                                 Convert.ToDecimal(ctrh["bgtActMmach"]) + Convert.ToDecimal(ctrh["bgtActMequip"]) + Convert.ToDecimal(ctrh["bgtActMToi"]) + Convert.ToDecimal(ctrh["bgtActMPrd"]) +
                                 Convert.ToDecimal(ctrh["bgtActMLand"])  + Convert.ToDecimal(ctrh["bgtActMSub"]) + Convert.ToDecimal(ctrh["bgtActMOther"]);
            txt_grandTotal2.Text = Convert.ToString(grandTotal);

            if (BizFunctions.IsEmpty(ctrh["bgtActMctr"]))
            {
                ctrh["bgtActMctr"] = 0;
            }
            txt_grossProfit2.Text = Convert.ToString(Convert.ToDecimal(ctrh["bgtActMctr"]) - grandTotal);
        }

        private void getBgtSysActTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (BizFunctions.IsEmpty(ctrh["bgtsactctr"]))
            {
                ctrh["bgtsactctr"] = 0;
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

            decimal grandTotal = Convert.ToDecimal(ctrh["bgtsactsal"]) + Convert.ToDecimal(ctrh["bgtsactmat"]) + Convert.ToDecimal(ctrh["bgtsactchem"]) +
                                Convert.ToDecimal(ctrh["bgtsactmach"]) + Convert.ToDecimal(ctrh["bgtsactequip"]) + Convert.ToDecimal(ctrh["bgtsactToi"]) + 
                                Convert.ToDecimal(ctrh["bgtsactPrd"]) + Convert.ToDecimal(ctrh["bgtsactLand"]) + Convert.ToDecimal(ctrh["bgtsactSub"]) + Convert.ToDecimal(ctrh["bgtsactOther"]);
            txt_grandTotal3.Text = Convert.ToString(grandTotal);

            if (BizFunctions.IsEmpty(ctrh["bgtsactctr"]))
            {
                ctrh["bgtsactctr"] = 0;
            }
            txt_grossProfit3.Text = Convert.ToString(Convert.ToDecimal(ctrh["bgtsactctr"]) - grandTotal);
        
        }

        private void getBgtActualTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (BizFunctions.IsEmpty(ctrh["bgtactctr"]))
            {
                ctrh["bgtactctr"] = 0;
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
               ctrh["bgtactchem"]  = 0;
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

            decimal grandTotal = Convert.ToDecimal(ctrh["bgtactsal"]) + Convert.ToDecimal(ctrh["bgtactmat"]) + Convert.ToDecimal(ctrh["bgtactchem"]) +
                               Convert.ToDecimal(ctrh["bgtactmach"]) + Convert.ToDecimal(ctrh["bgtactequip"]) + Convert.ToDecimal(ctrh["bgtactToi"]) + 
                               Convert.ToDecimal(ctrh["bgtactPrd"]) + Convert.ToDecimal(ctrh["bgtactLand"]) + Convert.ToDecimal(ctrh["bgtactSub"]) + Convert.ToDecimal(ctrh["bgtactOther"]);
            txt_grandTotal4.Text = Convert.ToString(grandTotal);

            if (BizFunctions.IsEmpty(ctrh["bgtactctr"]))
            {
                ctrh["bgtactctr"] = 0;
            }
            txt_grossProfit4.Text = Convert.ToString(Convert.ToDecimal(ctrh["bgtactctr"]) - grandTotal);
        }

        private void getBgtLatestActTotal()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

            if (BizFunctions.IsEmpty(ctrh["bgtlactctr"]))
            {
               ctrh["bgtlactctr"] = 0;
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
              ctrh["bgtlactToi"]  = 0;
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

             decimal grandTotal = Convert.ToDecimal(ctrh["bgtlactsal"]) + Convert.ToDecimal(ctrh["bgtlactmat"]) + Convert.ToDecimal(ctrh["bgtlactchem"]) +
                                Convert.ToDecimal(ctrh["bgtlactmach"]) + Convert.ToDecimal(ctrh["bgtlactequip"]) + Convert.ToDecimal(ctrh["bgtlactToi"]) + 
                                Convert.ToDecimal(ctrh["bgtlactPrd"]) + Convert.ToDecimal(ctrh["bgtlactLand"]) + Convert.ToDecimal(ctrh["bgtlactSub"]) + Convert.ToDecimal(ctrh["bgtlactOther"]);
            txt_grandTotal5.Text = Convert.ToString(grandTotal);

            if (BizFunctions.IsEmpty(ctrh["bgtlactctr"]))
            {
                ctrh["bgtlactctr"] = 0;
            }
            txt_grossProfit5.Text = Convert.ToString(Convert.ToDecimal(ctrh["bgtlactctr"]) - grandTotal);
        }

        #endregion

        private void initiValues()
        {
            DataRow ctrh = this.dbaccess.DataSet.Tables["ctrh"].Rows[0];

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

            /////  if (BizFunctions.IsEmpty(ctrh["bgtlactctr"]))
            {
                ctrh["bgtlactctr"] = 0;
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
    }
}
    

