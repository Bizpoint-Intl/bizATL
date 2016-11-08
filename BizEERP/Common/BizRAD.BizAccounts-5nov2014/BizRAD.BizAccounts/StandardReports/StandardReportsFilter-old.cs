using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices; // For Marshal.Copy

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizBase;
using BizRAD.BizApplication;

using Microsoft.Reporting.WinForms;

namespace BizRAD.BizAccounts
{
    public partial class StandardReportsFilter : Form
    {
        #region variables

        ResXResourceReader resxReader           = null;
        protected string languagePath           = null;
        protected string resourceFileName       = null;
        protected string language               = null;
        protected Hashtable languageBinding     = null;

        protected Form frmThis                  = null; 
        protected string projectPath            = null;
        
        protected DBAccess dbAccess             = null;
        protected string reportType             = null;
        protected DateTime cutOffDate           = System.DateTime.Now;
        protected bool sendToPrinter            = false;

        protected string storedProceudreName    = null;
        protected string rptFileName            = null;
        protected string xsdFileName            = null;
        protected string returnedTableName      = null;
        protected string filterColumnNames      = null;
        protected bool printLandscape           = false;
        protected string safeCutOffDate         = null;
        protected int cutOffPeriod              = 0;

        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        private Metafile pageImage = null;
        private System.Drawing.Graphics.EnumerateMetafileProc m_delegate = null;

        protected string columnsDelimiter = "|";

        protected DataTable reportF2Linkages = null;

        #endregion

        #region constants

        const string FILTER_01 = "contains";
        const string FILTER_02 = "is equal to";
        const string FILTER_03 = "begins with";
        const string FILTER_04 = "is not equal to";
        const string FILTER_05 = "is greater than";
        const string FILTER_06 = "is greater than and equal to";
        const string FILTER_07 = "is less than";
        const string FILTER_08 = "is less than and equal to";
		const string FILTER_09 = "does not contain";

        #endregion

        #region StandardReportsFilter
        public StandardReportsFilter(DBAccess dbAccess, DataRow selectedRow, string reportType, DateTime cutOffDate, bool sendToPrinter)
        {
            InitializeComponent();

            this.frmThis        = this;
            this.projectPath    = ConfigurationManager.AppSettings.Get("ProjectPath");

            #region Set Languages

            switch (Thread.CurrentThread.CurrentUICulture.Name)
            {
                default:
                    this.language = "English";
                    break;
            }
            
            this.resourceFileName = "LanguageBinding" + "_" + this.language + ".resx";
            this.languagePath = ConfigurationManager.AppSettings.Get("LanguagePath");

            this.resxReader = new ResXResourceReader(this.languagePath + this.resourceFileName);
            this.languageBinding = new Hashtable();
            foreach (DictionaryEntry entry in resxReader)
            {
                this.languageBinding.Add(entry.Key, entry.Value);
            }

            #endregion

            this.dbAccess = dbAccess;
            this.reportType = reportType;
            if(!Convert.IsDBNull(cutOffDate)) this.cutOffDate =  cutOffDate;
            this.sendToPrinter = sendToPrinter;

            this.storedProceudreName    = selectedRow["StoredProcedureName"].ToString();
            this.rptFileName            = selectedRow["RptFileName"].ToString();
            this.xsdFileName            = selectedRow["XsdFileName"].ToString();
            this.returnedTableName      = selectedRow["ReturnedTableName"].ToString();
            this.filterColumnNames      = selectedRow["FilterColumnNames"].ToString();
            this.printLandscape         = (Convert.IsDBNull(selectedRow["PrintLandscape"])) ? false : (bool)selectedRow["PrintLandscape"];

            this.frmThis.Text   = this.frmThis.Text + " [ Cut-Off-Date : " + this.cutOffDate.ToShortDateString() + " ]";

            this.safeCutOffDate = BizFunctions.GetSafeDateString(this.cutOffDate);
            this.cutOffPeriod   = BizAccounts.GetPeriod(new DBAccess(), this.cutOffDate);
        }
        #endregion

        #region StandardReportsFilter_Load
        private void StandardReportsFilter_Load(object sender, EventArgs e)
        {
            if (this.filterColumnNames == String.Empty)
            {
                this.btnOk_Click(sender, e);
                return;
            }

            DataSet dsTmp = this.dbAccess.ReadSQLTemp("SysStandardReportsLinkage", "select * from SysStandardReportsLinkage");
            this.reportF2Linkages = dsTmp.Tables["SysStandardReportsLinkage"];

            this.cmb_ColumnName1.SelectionChangeCommitted += new EventHandler(cmb_ColumnName_SelectionChangeCommitted);
            this.cmb_ColumnName2.SelectionChangeCommitted += new EventHandler(cmb_ColumnName_SelectionChangeCommitted);
            this.cmb_ColumnName3.SelectionChangeCommitted += new EventHandler(cmb_ColumnName_SelectionChangeCommitted);
            this.cmb_ColumnName4.SelectionChangeCommitted += new EventHandler(cmb_ColumnName_SelectionChangeCommitted);
            this.cmb_ColumnName5.SelectionChangeCommitted += new EventHandler(cmb_ColumnName_SelectionChangeCommitted);

            #region populate the combobox items

            char[] colDelimiter = this.columnsDelimiter.ToCharArray();
            string[] colSplit   = this.filterColumnNames.Split(colDelimiter);

            this.cmb_ColumnName1.Items.Add(new MyComboBoxItem("", String.Empty));
            this.cmb_ColumnName2.Items.Add(new MyComboBoxItem("", String.Empty));
            this.cmb_ColumnName3.Items.Add(new MyComboBoxItem("", String.Empty));
            this.cmb_ColumnName4.Items.Add(new MyComboBoxItem("", String.Empty));
            this.cmb_ColumnName5.Items.Add(new MyComboBoxItem("", String.Empty));

            foreach (string s in colSplit)
            {
                this.cmb_ColumnName1.Items.Add(new MyComboBoxItem(this.languageBinding[s.Trim()].ToString(), s.Trim()));
                this.cmb_ColumnName2.Items.Add(new MyComboBoxItem(this.languageBinding[s.Trim()].ToString(), s.Trim()));
                this.cmb_ColumnName3.Items.Add(new MyComboBoxItem(this.languageBinding[s.Trim()].ToString(), s.Trim()));
                this.cmb_ColumnName4.Items.Add(new MyComboBoxItem(this.languageBinding[s.Trim()].ToString(), s.Trim()));
                this.cmb_ColumnName5.Items.Add(new MyComboBoxItem(this.languageBinding[s.Trim()].ToString(), s.Trim()));
            }

            this.cmb_ColumnName1.SelectedIndex = (colSplit.Length > 0) ? 1 : 0;
            this.cmb_ColumnName2.SelectedIndex = (colSplit.Length > 1) ? 2 : 0;
            this.cmb_ColumnName3.SelectedIndex = (colSplit.Length > 2) ? 3 : 0;
            this.cmb_ColumnName4.SelectedIndex = (colSplit.Length > 3) ? 4 : 0;
            this.cmb_ColumnName5.SelectedIndex = (colSplit.Length > 4) ? 5 : 0;
            cmb_ColumnName_SelectionChangeCommitted(this.cmb_ColumnName1, e); 
            cmb_ColumnName_SelectionChangeCommitted(this.cmb_ColumnName2, e); 
            cmb_ColumnName_SelectionChangeCommitted(this.cmb_ColumnName3, e); 
            cmb_ColumnName_SelectionChangeCommitted(this.cmb_ColumnName4, e);
            cmb_ColumnName_SelectionChangeCommitted(this.cmb_ColumnName5, e);

            #region Add Items to Filter ComobBoxes
            this.cmb_Filter1.Items.Add(String.Empty);
            this.cmb_Filter1.Items.Add(FILTER_01);
            this.cmb_Filter1.Items.Add(FILTER_02);
            this.cmb_Filter1.Items.Add(FILTER_03);
            this.cmb_Filter1.Items.Add(FILTER_04);
            this.cmb_Filter1.Items.Add(FILTER_05);
            this.cmb_Filter1.Items.Add(FILTER_06);
            this.cmb_Filter1.Items.Add(FILTER_07);
            this.cmb_Filter1.Items.Add(FILTER_08);
			this.cmb_Filter1.Items.Add(FILTER_09);

            this.cmb_Filter2.Items.Add(String.Empty);
            this.cmb_Filter2.Items.Add(FILTER_01);
            this.cmb_Filter2.Items.Add(FILTER_02);
            this.cmb_Filter2.Items.Add(FILTER_03);
            this.cmb_Filter2.Items.Add(FILTER_04);
            this.cmb_Filter2.Items.Add(FILTER_05);
            this.cmb_Filter2.Items.Add(FILTER_06);
            this.cmb_Filter2.Items.Add(FILTER_07);
            this.cmb_Filter2.Items.Add(FILTER_08);
			this.cmb_Filter2.Items.Add(FILTER_09);

            this.cmb_Filter3.Items.Add(String.Empty);
            this.cmb_Filter3.Items.Add(FILTER_01);
            this.cmb_Filter3.Items.Add(FILTER_02);
            this.cmb_Filter3.Items.Add(FILTER_03);
            this.cmb_Filter3.Items.Add(FILTER_04);
            this.cmb_Filter3.Items.Add(FILTER_05);
            this.cmb_Filter3.Items.Add(FILTER_06);
            this.cmb_Filter3.Items.Add(FILTER_07);
            this.cmb_Filter3.Items.Add(FILTER_08);
			this.cmb_Filter3.Items.Add(FILTER_09);            
            
            this.cmb_Filter4.Items.Add(String.Empty);
            this.cmb_Filter4.Items.Add(FILTER_01);
            this.cmb_Filter4.Items.Add(FILTER_02);
            this.cmb_Filter4.Items.Add(FILTER_03);
            this.cmb_Filter4.Items.Add(FILTER_04);
            this.cmb_Filter4.Items.Add(FILTER_05);
            this.cmb_Filter4.Items.Add(FILTER_06);
            this.cmb_Filter4.Items.Add(FILTER_07);
            this.cmb_Filter4.Items.Add(FILTER_08);
			this.cmb_Filter4.Items.Add(FILTER_09);  

            this.cmb_Filter5.Items.Add(String.Empty);
            this.cmb_Filter5.Items.Add(FILTER_01);
            this.cmb_Filter5.Items.Add(FILTER_02);
            this.cmb_Filter5.Items.Add(FILTER_03);
            this.cmb_Filter5.Items.Add(FILTER_04);
            this.cmb_Filter5.Items.Add(FILTER_05);
            this.cmb_Filter5.Items.Add(FILTER_06);
            this.cmb_Filter5.Items.Add(FILTER_07);
            this.cmb_Filter5.Items.Add(FILTER_08);
			this.cmb_Filter5.Items.Add(FILTER_09);  
            #endregion

            this.cmb_Filter1.SelectedIndex = (colSplit.Length > 0) ? 2 : 0;
            this.cmb_Filter2.SelectedIndex = (colSplit.Length > 1) ? 2 : 0;
            this.cmb_Filter3.SelectedIndex = (colSplit.Length > 2) ? 2 : 0;
            this.cmb_Filter4.SelectedIndex = (colSplit.Length > 3) ? 2 : 0;
            this.cmb_Filter5.SelectedIndex = (colSplit.Length > 4) ? 2 : 0;

            #endregion

            this.tb_Value1.DoubleClick += new EventHandler(tb_Value_DoubleClick);
            this.tb_Value2.DoubleClick += new EventHandler(tb_Value_DoubleClick);
            this.tb_Value3.DoubleClick += new EventHandler(tb_Value_DoubleClick);
            this.tb_Value4.DoubleClick += new EventHandler(tb_Value_DoubleClick);
            this.tb_Value5.DoubleClick += new EventHandler(tb_Value_DoubleClick);
        }
        #endregion

        #region btnCancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.m_streams != null)
            {
                foreach (Stream stream in this.m_streams)
                    stream.Close();
                this.m_streams = null;
            }

            this.frmThis.Dispose();
        }
        #endregion

        #region btnOK
        private void btnOk_Click(object sender, EventArgs e)
        {
            string currentDirectory = Environment.CurrentDirectory;

            Cursor currentCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            DataSet spResults = ExecuteReportSP(this.storedProceudreName);
            Cursor.Current = currentCursor;

			if (spResults != null)
	            this.PrintDocument(spResults);

            Environment.CurrentDirectory = currentDirectory;
            this.btnCancel_Click(sender, e);
        }
        #endregion

        #region cmb_ColumnName_SelectionChangeCommitted & tb_Value_DoubleClick

        protected void cmb_ColumnName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string columnToLink                     = String.Empty;
            TextBox correspondingValueTextBox       = null;

            if ((sender as ComboBox).SelectedItem != null)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cmb_ColumnName2":
                        columnToLink                = ((MyComboBoxItem)this.cmb_ColumnName2.SelectedItem).GetValue();
                        correspondingValueTextBox   = this.tb_Value2;
                        break;
                    case "cmb_ColumnName3":
                        columnToLink                = ((MyComboBoxItem)this.cmb_ColumnName3.SelectedItem).GetValue();
                        correspondingValueTextBox   = this.tb_Value3;
                        break;
                    case "cmb_ColumnName4":
                        columnToLink                = ((MyComboBoxItem)this.cmb_ColumnName4.SelectedItem).GetValue();
                        correspondingValueTextBox   = this.tb_Value4;
                        break;
                    case "cmb_ColumnName5":
                        columnToLink                = ((MyComboBoxItem)this.cmb_ColumnName5.SelectedItem).GetValue();
                        correspondingValueTextBox   = this.tb_Value5;
                        break;
                    default:
                        columnToLink                = ((MyComboBoxItem)this.cmb_ColumnName1.SelectedItem).GetValue();
                        correspondingValueTextBox   = this.tb_Value1;
                        break;
                }
            }

            DataRow [] selectedLinkageRow = this.reportF2Linkages.Select("ColumnName = '" + columnToLink + "'");
            if (selectedLinkageRow.Length > 0)
            {
                correspondingValueTextBox.BackColor = Color.Bisque;
            }
            else
            {
                correspondingValueTextBox.BackColor = Color.White;
            }
        }

        protected void tb_Value_DoubleClick(object sender, EventArgs e)
        {
            TextBox currentTextBox = (TextBox)sender;
            string columnToLink = String.Empty;

            if (currentTextBox.BackColor == Color.Bisque)
            {
                switch (currentTextBox.Name)
                {
                    case "tb_Value2":
                        columnToLink = ((MyComboBoxItem)this.cmb_ColumnName2.SelectedItem).GetValue();
                        break;
                    case "tb_Value3":
                        columnToLink = ((MyComboBoxItem)this.cmb_ColumnName3.SelectedItem).GetValue();
                        break;
                    case "tb_Value4":
                        columnToLink = ((MyComboBoxItem)this.cmb_ColumnName4.SelectedItem).GetValue();
                        break;
                    case "tb_Value5":
                        columnToLink = ((MyComboBoxItem)this.cmb_ColumnName5.SelectedItem).GetValue();
                        break;
                    default:
                        columnToLink = ((MyComboBoxItem)this.cmb_ColumnName1.SelectedItem).GetValue();
                        break;
                }

                DataRow [] selectedLinkageRow = this.reportF2Linkages.Select("ColumnName = '" + columnToLink + "'");
                if (selectedLinkageRow.Length > 0)
                {
                    F2BaseHelper f2BaseHelper = new F2BaseHelper(selectedLinkageRow[0]["F2GridInfoFileName"].ToString(), sender, columnToLink, null, null, F2Type.Sort);
                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {
                        currentTextBox.Text = f2BaseHelper.F2Base.CurrentRow[columnToLink].ToString();
                    }
                }
            }
        }

        #endregion

        #region ExecuteReportSP

        protected DataSet ExecuteReportSP(string sp_Name)
        {
            string whereClause = this.GetRowFilter();
            whereClause = (whereClause == String.Empty) ? whereClause : " WHERE " + whereClause;

            Parameter[] parameters = new Parameter[6];
            parameters[0] = new Parameter("LoginSystemYear", Common.DEFAULT_SYSTEM_YEAR);
            parameters[1] = new Parameter("LoginCompany", String.Empty);
            parameters[2] = new Parameter("LoginUserName", Common.DEFAULT_SYSTEM_USERNAME);
            parameters[3] = new Parameter("CutOffDate", this.safeCutOffDate);
            parameters[4] = new Parameter("CutOffPeriod", this.cutOffPeriod); 
            parameters[5] = new Parameter("WhereClause", whereClause);

            try
            {
                DataSet ds_Results = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult(sp_Name, ref parameters);

				if (ds_Results == null)
				{
					MessageBox.Show("Criteria did not return any result set!", "No Data",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return null;
				}

                ds_Results.DataSetName = this.xsdFileName;
                ds_Results.Tables[0].TableName = this.returnedTableName;

                return ds_Results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }

        #endregion

        #region whereClause

        protected string GetRowFilter()
        {
            string rowFilter = String.Empty;

            if (this.cmb_ColumnName1.SelectedItem != null && this.cmb_Filter1.SelectedItem != null && this.tb_Value1.Text != String.Empty)
            {
                rowFilter = this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName1.SelectedItem).GetValue(), this.cmb_Filter1.SelectedItem.ToString(), this.tb_Value1.Text);
            }

            if (this.cmb_ColumnName2.SelectedItem != null && this.cmb_Filter2.SelectedItem != null && this.tb_Value2.Text != String.Empty)
            {
                if(rowFilter != String.Empty) rowFilter += " AND ";
                rowFilter += this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName2.SelectedItem).GetValue(), this.cmb_Filter2.SelectedItem.ToString(), this.tb_Value2.Text);
            }

            if (this.cmb_ColumnName3.SelectedItem != null && this.cmb_Filter3.SelectedItem != null && this.tb_Value3.Text != String.Empty)
            {
                if (rowFilter != String.Empty) rowFilter += " AND ";
                rowFilter += this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName3.SelectedItem).GetValue(), this.cmb_Filter3.SelectedItem.ToString(), this.tb_Value3.Text);
            }

            if (this.cmb_ColumnName4.SelectedItem != null && this.cmb_Filter4.SelectedItem != null && this.tb_Value4.Text != String.Empty)
            {
                if (rowFilter != String.Empty) rowFilter += " AND ";
                rowFilter += this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName4.SelectedItem).GetValue(), this.cmb_Filter4.SelectedItem.ToString(), this.tb_Value4.Text);
            }

            if (this.cmb_ColumnName5.SelectedItem != null && this.cmb_Filter5.SelectedItem != null && this.tb_Value5.Text != String.Empty)
            {
                if (rowFilter != String.Empty) rowFilter += " AND ";
                rowFilter += this.GetFilterString(((MyComboBoxItem)this.cmb_ColumnName5.SelectedItem).GetValue(), this.cmb_Filter5.SelectedItem.ToString(), this.tb_Value5.Text);
            }

            return rowFilter;
        }

        protected string GetFilterString(string columnSelected, string filterSelected, string filterValue)
        {
            string filterClause = String.Empty;

            switch (filterSelected)
            {
                case FILTER_01 :
                    filterClause = columnSelected + " LIKE '%" + filterValue + "%'";
                    break;
                case FILTER_02:
                    filterClause = columnSelected + " = '" + filterValue + "'";
                    break;
                case FILTER_03:
                    filterClause = columnSelected + " LIKE '" + filterValue + "%'";
                    break;
                case FILTER_04:
                    filterClause = columnSelected + " <> '" + filterValue + "'";
                    break;
                case FILTER_05:
                    filterClause = columnSelected + " > '" + filterValue + "'";
                    break;
                case FILTER_06:
                    filterClause = columnSelected + " >='" + filterValue + "'";
                    break;
                case FILTER_07:
                    filterClause = columnSelected + " <'" + filterValue + "'";
                    break;
                case FILTER_08:
                    filterClause = columnSelected + " <='" + filterValue + "'";
                    break;
				case FILTER_09:
					filterClause = columnSelected + " NOT LIKE '%" + filterValue + "%'";
					break;
            }

            return filterClause;
        }

        #endregion

        #region PrintDocument

        protected void PrintDocument(DataSet spResults)
        {
            if (Convert.IsDBNull(this.cutOffDate))
            {
                MessageBox.Show("Please enter a Cut Off Date !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string reportPath = this.projectPath + "\\Standard" + this.reportType + "Reports\\" + this.rptFileName + ".rdlc";
            string reportDataSourceName = this.xsdFileName + "_" + this.returnedTableName;
            ReportDataSource rds = new ReportDataSource(reportDataSourceName, spResults.Tables[0]);

            ReportParameter[] parameters = new ReportParameter[2];
            parameters[0] = new ReportParameter("CutOffPeriod", this.cutOffPeriod.ToString());
            parameters[1] = new ReportParameter("CutOffDate", this.cutOffDate.ToShortDateString());

            try
            {
                if (this.sendToPrinter)
                {
                    LocalReport report = new LocalReport();

                    report.ReportPath = reportPath;
                    report.DataSources.Add(rds);
                    report.SetParameters(parameters);

                    this.Export(report);

                    this.m_currentPageIndex = 0;
                    this.Print();

                    if (this.m_streams != null)
                    {
                        foreach (Stream stream in this.m_streams)
                            stream.Close();
                        this.m_streams = null;
                    }
                }
                else
                {
                    StandardReportsPreview standardReportsPreview = new StandardReportsPreview();

                    standardReportsPreview.reportViewer.LocalReport.ReportPath = reportPath;
                    standardReportsPreview.reportViewer.LocalReport.DataSources.Add(rds);
                    standardReportsPreview.reportViewer.LocalReport.SetParameters(parameters);

                    // Add a handler for drillthrough.
                    //standardReportsPreview.reportViewer.Drillthrough += new DrillthroughEventHandler(DemoDrillthroughEventHandler);

                    standardReportsPreview.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region sendToPrinter Functions

        #region CreateStream
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            //Stream stream = new FileStream(name + "." + fileNameExtension, FileMode.Create);
            Stream stream = new MemoryStream();		//cmh - try out

            this.m_streams.Add(stream);
            return stream;
        }
        #endregion

        #region Export
        private void Export(LocalReport report)
        {
            string deviceInfo = String.Empty;

            if (this.printLandscape)
            {
                //landscape
                deviceInfo =
                  "<DeviceInfo>" +
                  "  <OutputFormat>EMF</OutputFormat>" +
                  "  <PageWidth>11in</PageWidth>" +
                  "  <PageHeight>8.5in</PageHeight>" +
                  "  <MarginTop>0in</MarginTop>" +
                  "  <MarginLeft>0in</MarginLeft>" +
                  "  <MarginRight>0in</MarginRight>" +
                  "  <MarginBottom>0in</MarginBottom>" +
                  "</DeviceInfo>";
            }
            else
            {
                //portrait
                deviceInfo =
                  "<DeviceInfo>" +
                  "  <OutputFormat>EMF</OutputFormat>" + 
                  "  <PageWidth>8.5in</PageWidth>" +
                  "  <PageHeight>11in</PageHeight>" +
                  "  <MarginTop>0.25in</MarginTop>" +
                  "  <MarginLeft>0.25in</MarginLeft>" +
                  "  <MarginRight>0.25in</MarginRight>" +
                  "  <MarginBottom>0.25in</MarginBottom>" +
                  "</DeviceInfo>";
            }

            Warning[] warnings;
            this.m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);

            foreach (Stream stream in this.m_streams)
                stream.Position = 0;
        }
        #endregion

        #region PrintPage
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            //Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);
            //ev.Graphics.DrawImage(pageImage, ev.PageBounds);

            this.pageImage = new Metafile(this.m_streams[this.m_currentPageIndex]);

            lock (this)
            {
                // Set the metafile delegate.
                int width = this.pageImage.Width;
                int height = this.pageImage.Height;
                this.m_delegate = new System.Drawing.Graphics.EnumerateMetafileProc(this.MetafileCallback);
                // Draw in the rectangle
                //System.Drawing.Point destPoint = new System.Drawing.Point(0, 0);
                //ev.Graphics.EnumerateMetafile(pageImage, destPoint, m_delegate);

                //define the printing area
                System.Drawing.Point[] points = new System.Drawing.Point[3];
                if (this.printLandscape)
                {
                    points[0] = new System.Drawing.Point(0, 0);			//top left corner
                    points[1] = new System.Drawing.Point(1050, 0);		//top right corner
                    points[2] = new System.Drawing.Point(0, 800);		//bottom left corner
                }
                else
                {
                    points[0] = new System.Drawing.Point(0, 0);			//top left corner
                    points[1] = new System.Drawing.Point(800, 0);		//top right corner
                    points[2] = new System.Drawing.Point(0, 1050);		//bottom left corner
                }

                ev.Graphics.EnumerateMetafile(this.pageImage, points, this.m_delegate);
                // Clean up
                this.m_delegate = null;
            }

            this.m_currentPageIndex++;
            ev.HasMorePages = (this.m_currentPageIndex < this.m_streams.Count);
        }
        #endregion

        #region MetafileCallBack
        private bool MetafileCallback(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData)
        {
            byte[] dataArray = null;
            // Dance around unmanaged code.
            if (data != IntPtr.Zero)
            {
                // Copy the unmanaged record to a managed byte buffer 
                // that can be used by PlayRecord.
                dataArray = new byte[dataSize];
                Marshal.Copy(data, dataArray, 0, dataSize);
            }
            // play the record.      
            this.pageImage.PlayRecord(recordType, flags, dataSize, dataArray);

            return true;
        }
        #endregion

        #region Print
        private void Print()
        {
            const string printerName = "Microsoft Office Document Image Writer";

            if (m_streams == null || m_streams.Count == 0)
                return;

            PrintDocument printDoc = new PrintDocument();
            //printDoc.PrinterSettings.PrinterName = printerName;		//cmh -commented off to send direct to default printer

            //test landscape printing - cmh
            if(this.printLandscape) printDoc.DefaultPageSettings.Landscape = true;
            //	printDoc.DefaultPageSettings.PaperSize = new PaperSize("Landscape", 1100, 850);
            //	printDoc.DefaultPageSettings.PaperSize = new PaperSize("Landscape", 777, 600);

            //added -cmh
            //printDoc.OriginAtMargins = true;
            //printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            if (!printDoc.PrinterSettings.IsValid)
            {
                string msg = String.Format("Can't find printer \"{0}\".", printerName);
                Console.WriteLine(msg);
                return;
            }
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            printDoc.Print();
        }
        #endregion

        private void cmb_Filter1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion

        //void DemoDrillthroughEventHandler(object sender, DrillthroughEventArgs e)
        //{
        //    LocalReport localReport = (LocalReport)e.Report;
        //    localReport.DataSources.Add(new ReportDataSource("Employees", LoadEmployeesData()));
        //}
    }

    #region MyComboBoxItem
    public class MyComboBoxItem
    {
        private string _name;
        private string _value;

        public MyComboBoxItem(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetValue()
        {
            return _value;
        }
    }
    #endregion
}