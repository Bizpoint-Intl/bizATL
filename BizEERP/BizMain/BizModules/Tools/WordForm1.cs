using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
using ATL.BizModules.FileAcc3;
using ATL.Network;

namespace ATL.BizModules.RichTextEdit
{
    public partial class WordForm1 : Form
    {
        DBAccess dbAccess;
        string tableName, columnName,type,fileLocation = "";
        Byte[] array = null;
        public WordForm1(DBAccess db,string tName, string cName, string Type)
        {
            this.tableName = tName;
            this.columnName = cName;
            this.dbAccess = db;
            this.type = Type;
            InitializeComponent();
           
        }
        private void WordForm1_Load(object sender, EventArgs e)
        {
            LoadDocument();
        }

       
        private void fileSaveItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
                     
        }

        private void richEditControl1_Click(object sender, EventArgs e)
        {

        }
      
        private void LoadDocument()
        {
            DataRow t = this.dbAccess.DataSet.Tables[tableName].Rows[0];

            if (t[columnName].ToString().Length > 0)
            {               
                array = (byte[])t[columnName];

                if (array != null && array.Length > 0)
                {
                    MemoryStream stmBLOBData = new MemoryStream(array);
                    richEditControl1.LoadDocument(stmBLOBData, DevExpress.XtraRichEdit.DocumentFormat.Html);
                }
            }            
        }
       
        public void SaveDocumentHTML()
        {
            DataRow t = this.dbAccess.DataSet.Tables[tableName].Rows[0];
            MemoryStream ms = new MemoryStream();

            richEditControl1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Html);
            array = ms.ToArray();

            t[columnName] = array;
        }
    
        private void WordForm1_Close(object sender, FormClosedEventArgs e)
        {
            
            SaveDocumentHTML(); 
        }

        public bool SaveToFile()
        {
            bool isSaved = false;
            DataRow t = this.dbAccess.DataSet.Tables[tableName].Rows[0];
            MemoryStream ms = new MemoryStream();

            if (t[columnName].ToString().Length > 0)
            {
                array = (byte[])t[columnName];

                if (array != null && array.Length > 0)
                {
                    MemoryStream stmBLOBData = new MemoryStream(array);
                    richEditControl1.LoadDocument(stmBLOBData, DevExpress.XtraRichEdit.DocumentFormat.Html);

                    richEditControl1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Html);
                    array = ms.ToArray();
                }
            }         

             string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

             if (DriveLetter.Trim() != "")
             {


                 FileSendGet3 fsg1 = new FileSendGet3(DriveLetter.Trim(), array, t["arnum"].ToString(), Convert.ToDateTime(t["commencedate"]), type);
                 if (!fsg1.FileUploadSuccess)
                 {
                     try
                     {
                         isSaved = false;
                     }
                     catch (Exception ex)
                     {

                     }
                 }
                 else
                 {
                     fileLocation = fsg1.FileInServerLocation;
                     isSaved = true;
                 }
             }

            return isSaved;
        }

        public string FileInServerLocation
        {
            get
            {
                return fileLocation;
            }
            set
            {
                fileLocation = value;
            }
        }

       
    }
}