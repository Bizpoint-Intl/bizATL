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
using ATL.BizModules.FileAcc3;
using ATL.Network;
using System.Text.RegularExpressions;
#endregion

namespace ATL.ISN
{
    public class Voucher_ISN : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected DBAccess dbaccess = null;
        private string headerFormName = "";
        Button BtnBrowse;
        private TextBox txt_photourl = null;
        #endregion

        #region Construct

        public Voucher_ISN(string moduleName, Hashtable voucherBaseHelpers)

            : base("VoucherGridInfo_ISN.xml", moduleName, voucherBaseHelpers)
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
            txt_photourl = (TextBox)BizXmlReader.CurrentInstance.GetControl(headerFormName, "photourl");
            BtnBrowse = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btnBrowse") as Button;
            BtnBrowse.Click += new EventHandler(BtnBrowse_Click);
           
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
           
                                  
        }
        #endregion
        
        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);
    
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

        protected void BtnBrowse_Click(object sender, System.EventArgs e)
        {
            Form frm = BizXmlReader.CurrentInstance.GetForm(headerFormName) as Form;
            try
            {
                DataRow hemph = this.dbaccess.DataSet.Tables["isn"].Rows[0];
                ////TextBox tb_image = (TextBox)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "hemph_empphoto");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
                //openFileDialog.Filter = "JPEG(*.JPG;*.JPEG;*.JPE;*.JFIF)|*.jpg;*.jpeg;*.jpe;*.jfif|BMP Files|*.bmp|GIF Files|*.gif|TIFF(*.TIF;*.TIFF)|*.tif;*.tiff|PNG Files|*.png|All Picture Files|*.bmp;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png|All Files|*.*";

                //openFileDialog.Filter = "|All Files|*.*";
                openFileDialog.ShowDialog(frm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataRow isn = this.dbaccess.DataSet.Tables["isn"].Rows[0];
 

     
            //try
            //{

            //    string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

            //    if (DriveLetter.Trim() != "")
            //    {

            //        if (BizFunctions.IsEmpty(isn["doclocation"]))
            //        {
            //            FileSendGet fsg1 = new FileSendGet(DriveLetter.Trim(), (sender as OpenFileDialog).FileName, isn["refnum"].ToString(), "PDF");
            //            if (!fsg1.FileUploadSuccess)
            //            {
            //                MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            }
            //            else
            //            {
            //                isn["doclocation"] = fsg1.FileInServerLocation;

            //            }

            //        }
            //        else
            //        {
            //            if (!File.Exists(isn["doclocation"].ToString()))
            //            {
            //                FileSendGet fsg2 = new FileSendGet(DriveLetter.Trim(), (sender as OpenFileDialog).FileName, isn["refnum"].ToString(), "PDF");
            //                if (!fsg2.FileUploadSuccess)
            //                {
            //                    MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                }
            //                else
            //                {
            //                    isn["doclocation"] = fsg2.FileInServerLocation;

            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Upload of Docs unsuccessful!. Please try again.", "Pico Guards Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            //finally
            //{
            //    NetworkDrive.DisconnectNetworkDrive(true);
            //}



            //if (enableDocSave)
            //{
                string DriveLetter1 = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                try
                {
                    string SaveLoc = DriveLetter1 + ":";
                    System.IO.DirectoryInfo fl = new DirectoryInfo(SaveLoc + @"\\ISN\\" + isn["refnum"].ToString() + "\\");

                    if (!fl.Exists)
                    {
                        System.IO.Directory.CreateDirectory(fl.FullName);
                    }


                    string FileName = isn["refnum"].ToString()+".pdf";

                    string ServerLocation = System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository") + "\\ISN\\" + isn["refnum"].ToString() + "";
                    //pb.Image.Save(fl.FullName + "\\" + hemph["empnum"].ToString().Trim() + ".pdf");

                    if (File.Exists(ServerLocation + "\\" + FileName))
                    {
                        File.Delete(ServerLocation + "\\" + FileName);
                    }

                    File.Copy((sender as OpenFileDialog).FileName.ToString(), ServerLocation + "\\" + FileName);

                    ServerLocation = ServerLocation + "\\";

              
                    isn["doclocation"] = ServerLocation + FileName;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "ATL Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    NetworkDrive.DisconnectNetworkDrive(true);
                }
            //}
         
     
        }


    }
}
    

