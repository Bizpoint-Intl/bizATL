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

namespace ATL.BizModules.RichTextEdit2
{
    public partial class WordForm2 : Form
    {
 
        public Byte[] array = null;
        public WordForm2(Byte[] Array)
        {
            this.array = Array;
            InitializeComponent();
           
        }
        private void WordForm2_Load(object sender, EventArgs e)
        {
            LoadDocument();
        }

       
        private void fileSaveItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
                     
        }

      
        private void LoadDocument()
        {

            if (array != null && array.Length > 0)
            {
                MemoryStream stmBLOBData = new MemoryStream(array);
                richEditControl1.LoadDocument(stmBLOBData, DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
            }
          
        }

        private void SaveDocument()
        {

            MemoryStream ms = new MemoryStream();

            richEditControl1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
            array = ms.ToArray();


        }

        private void richEditControl1_Click(object sender, EventArgs e)
        {

        }

    

        private void WordForm2_Close(object sender, FormClosedEventArgs e)
        {
            SaveDocument();   
        }

       
    }
}