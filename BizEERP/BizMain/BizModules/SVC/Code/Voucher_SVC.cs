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
using ATL.SortTable;
using ATL.TimeUtilites;
using ATL.BizModules.TextValidator;
using NodaTime;
using ATL.DateTimeExt;

using ATL.ExtractATR1Form1;
using ATL.ExtractWORpics;
using ATL.Network;
using System.Diagnostics;
using System.Net.Mail;
//using Outlook = Microsoft.Office.Interop.Outlook;
#endregion

namespace ATL.SVC
{
    public class Voucher_SVC : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region Class Variables
        protected string ImagePath, photofilename = "";
        protected DBAccess dbaccess = null;
        protected string headerFormName,photoFormName, NewTaskForm, OutstandingForm, LocationFormName = null;
        protected TextBox worh_sectorcode, worh_day = null;
        protected Button Btn_Sort, Btn_Extract, btnPrint = null;
        protected ComboBox TableColumn = null;
        protected DateTimePicker worh_worhdate = null;
        protected Button btnExtract1, columnButton,columnButton1,columnButton2, locColumnButton1 = null;
        protected DataGrid Datagrid1, Datagrid2, dg_outstanding, dg_newtask, dg_location, dg_photos = null;
        protected DataGridView DatagridView1 = null;
        protected bool isMouseClicked = false;
        protected string SectorCode = "";
        protected bool opened, enableDocSave = false;
        protected string flag = "";
        string filepath = System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository") + "\\SVC";
        string projectPath = null;

        protected TextBox txt_editEmpno = null;
        protected Button Btn_Show, btnLists, btnListsPic = null,btn_Email=null ;
        #endregion

        #region Construct

        public Voucher_SVC(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_WOR.xml", moduleName, voucherBaseHelpers)
        {
            this.flag = moduleName;
        }
        #endregion Construct

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "flag='" + flag + "' ";
       

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);
            e.Condition = "flag='" + flag + "' and [status]='O'";



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
             DataRow worh = dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = dbaccess.DataSet.Tables["wok1"];

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.photoFormName = (e.FormsCollection["photos"] as Form).Name;
            //this.NewTaskForm = (e.FormsCollection["newtask"] as Form).Name;
            //this.OutstandingForm = (e.FormsCollection["outstanding"] as Form).Name;
            //this.LocationFormName = (e.FormsCollection["location"] as Form).Name;
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");

            // Jason: Now each work order is for 1 single site
            btn_Email = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Email") as Button;
            btn_Email.Enabled = true;
            btn_Email.Click += new EventHandler(btnEmail_Click);
                  
            //e.DBAccess.DataSet.Tables["WOR1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_WOR1_ColumnChanged);
            opened = true;

            Initialise();

            string GetPAYTM = "SELECT * FROM PAYTM WHERE [status]<>'V'";
            this.dbaccess.ReadSQL("PAYTM", GetPAYTM);

            string strEMPTB = "select empnum,empname from hemph";
            this.dbaccess.ReadSQL("EMPTB", strEMPTB);

            string GetvSHLV = "Select * from vSHLV";
            this.dbaccess.ReadSQL("vSHLV", GetvSHLV);

            string GetvHEMPHtmp1 = "select * from vMainHEMPH where [status]<>'V'";
            this.dbaccess.ReadSQL("HEMPHtmp1", GetvHEMPHtmp1);

            enableDocSave = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("EnableDocSave"));


            if (!BizFunctions.IsEmpty(worh["sitenum"]))
            {
                if (BizFunctions.IsEmpty(worh["sitename"]))
                {
                    worh["sitename"] = BizLogicTools.Tools.GetSitenname(worh["sitenum"].ToString(), this.dbaccess);
                }
            }

            dg_photos = BizXmlReader.CurrentInstance.GetControl(photoFormName, "dg_photos") as DataGrid;

            foreach (DataGridTableStyle dataGridTableStyle in dg_photos.TableStyles)
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
                            columnButton1.Text = "Upload";
                            columnButton1.FlatStyle = FlatStyle.Standard;
                            columnButton1.BackColor = Color.FromKnownColor(KnownColor.Control);
                            columnButton1.Size = new Size(75, 14);
                            columnButton1.Click += new EventHandler(columnButton1_Click);

                            bizDataGridTextBoxColumn.TextBoxGrid.Controls.Add(columnButton1);
                        }
                    }
                }
            }

            foreach (DataGridTableStyle dataGridTableStyle in dg_photos.TableStyles)
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
                            columnButton2.Text = "Preview";
                            columnButton2.FlatStyle = FlatStyle.Standard;
                            columnButton2.BackColor = Color.FromKnownColor(KnownColor.Control);
                            columnButton2.Size = new Size(75, 14);
                            columnButton2.Click += new EventHandler(columnButton2_Click);

                            bizDataGridTextBoxColumn.TextBoxGrid.Controls.Add(columnButton2);
                        }
                    }
                }
            }

        }

        void columnButton2_Click(object sender, EventArgs e)
        {
            DataTable wor5 = this.dbaccess.DataSet.Tables["wor5"];

            if (wor5.Rows.Count > 0)
            {
                if (!BizFunctions.IsEmpty(wor5.Rows[dg_photos.CurrentCell.RowNumber]["photourl"]))
                {
                    Process.Start(wor5.Rows[dg_photos.CurrentCell.RowNumber]["photourl"].ToString());
                }
                else
                {
                    MessageBox.Show("This Document has No Signed Picture", "No Pictures", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        void columnButton1_Click(object sender, EventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor5 = this.dbaccess.DataSet.Tables["wor5"];
            string RepositoryLocation = System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository");
            Form frm = BizXmlReader.CurrentInstance.GetForm(headerFormName) as Form;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";

            if (!BizFunctions.IsEmpty(wor5.Rows[dg_photos.CurrentCell.RowNumber]["photourl"]))
            {
                if (MessageBox.Show("There is an existing file, do you want to overwrite it?", "Notification", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    photofilename = openFileDialog.FileName;

                    openFileDialog.ShowDialog(frm);

                    if (wor5.Rows.Count > 0)
                    {

                        int count = openFileDialog.FileName.Length;

                        if (count > 0)
                        {

                            string CompleteLocation = RepositoryLocation + "\\WOR\\SVC\\" + worh["refnum"].ToString().Trim() + "\\";
                            if (!System.IO.Directory.Exists(CompleteLocation))
                            {
                                System.IO.Directory.CreateDirectory(CompleteLocation);
                            }

                            string destifilePath = CompleteLocation + "\\" + openFileDialog.SafeFileName;

                            try
                            {

                                System.IO.File.Copy(openFileDialog.FileName, destifilePath.Trim(), true);

                                wor5.Rows[dg_photos.CurrentCell.RowNumber]["photourl"] = destifilePath.Trim();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not copy files to temp folder.", "File Error");
                                return;
                            }
                        }



                    }
                }
            }
            else
            {
                photofilename = openFileDialog.FileName;

                openFileDialog.ShowDialog(frm);

                if (wor5.Rows.Count > 0)
                {

                    int count = openFileDialog.FileName.Length;

                    if (count > 0)
                    {

                        string CompleteLocation = RepositoryLocation + "\\WOR\\SVC\\" + worh["refnum"].ToString().Trim() + "\\";
                        if (!System.IO.Directory.Exists(CompleteLocation))
                        {
                            System.IO.Directory.CreateDirectory(CompleteLocation);
                        }

                        string destifilePath = CompleteLocation + "\\" + openFileDialog.SafeFileName;

                        try
                        {

                            System.IO.File.Copy(openFileDialog.FileName, destifilePath.Trim(), true);

                            wor5.Rows[dg_photos.CurrentCell.RowNumber]["photourl"] = destifilePath.Trim();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Could not copy files to temp folder.", "File Error");
                            return;
                        }
                    }



                }
            }




        }

        void btnPrint_Click(object sender, EventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = this.dbaccess.DataSet.Tables["wor4"];
            Hashtable selectedCollection = new Hashtable();

            if (!BizValidate.CheckRowState(this.dbaccess.DataSet, "worh"))
            {
                string[] arr1;
                selectedCollection.Add("coy", "SELECT * FROM coy");
                selectedCollection.Add("pdm", "SELECT * FROM PDM");
                selectedCollection.Add("arm", "SELECT * FROM ARM where arnum='" + worh["arnum"].ToString() + "'");

                if (wor1.Rows.Count > 0)
                {
                    DataTable siteDataTable = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select distinct sitenum from WOR1");

                    siteDataTable.Columns.Add("worstatus", typeof(string));

                    if (wor4.Rows.Count > 0)
                    {                       
                            foreach (DataRow dr4 in wor4.Rows)
                            {
                                if (dr4.RowState != DataRowState.Deleted)
                                {
                                    foreach (DataRow drStb in siteDataTable.Rows)
                                    {
                                        if (drStb.RowState != DataRowState.Deleted)
                                        {
                                            if (dr4["sitenum"] == drStb["sitenum"])
                                            {
                                                drStb["worstatus"] = dr4["worstatus"];                                                
                                            }
                                        }
                                    }
                                }
                            }                        
                    }

                    if (siteDataTable.Rows.Count > 0)
                    {
                        arr1 = new string[siteDataTable.Rows.Count];

                        for (int i = 0; i < siteDataTable.Rows.Count; i++)
                        {
                            if (siteDataTable.Rows[i].RowState != DataRowState.Deleted)
                            {

                                if (siteDataTable.Rows.Count > 1 && i < siteDataTable.Rows.Count - 1)
                                {
                                    arr1[i] = "'" + siteDataTable.Rows[i]["sitenum"].ToString() + "',";
                                }
                                else
                                {
                                    arr1[i] = "'" + siteDataTable.Rows[i]["sitenum"].ToString() + "'";
                                }
                            }
                        }
                        string siteList = "";
                        for (int x = 0; x < arr1.Length; x++)
                        {
                            siteList = siteList + arr1[x].ToString();
                        }
                        selectedCollection.Add("sitm", "SELECT * FROM SITM where sitenum in (" + siteList + ")");

                    }

                }



                string NewTaskStr = "SELECT " +
                                    "'" + worh["refnum"].ToString().Trim() + "' as Refnum, " +
                                   "[Zone Sup], " +
                                   "docunum, " +
                                   "[Project Site], " +
                                   "Location, " +
                                   "[Service Code], " +
                                   "[Service Desc], " +
                                   "Frequency, " +
                                   "[Done Date], " +
                                   "[Last Done], " +
                                   "remark " +
                                   "FROM " +
                                   "( " +
                                       "SELECT " +
                                           "zempnum as [Zone Sup], " +
                                           "ctrnum as docunum, " +
                                           "sitenum  as [Project Site], " +
                                           "locnum as Location, " +
                                           "svccode as [Service Code], " +
                                           "svcdesc as [Service Desc], " +
                                           "frequencycode as Frequency, " +
                                           "cdate as [Done Date], " +
                                           "ldone as [Last Done], " +
                                           "remark " +
                                       "FROM WOR1 " +
                                   ")A ";


                string OutstandingStr = "SELECT " +
                                           "'" + worh["refnum"].ToString().Trim() + "' as Refnum, " +
                                           "[Zone Sup], " +
                                           "docunum, " +
                                           "[Project Site], " +
                                           "Location, " +
                                           "[Service Code], " +
                                           "[Service Desc], " +
                                           "Frequency, " +
                                           "[Done Date], " +
                                           "[Last Done], " +
                                           "remark " +
                                         "FROM dtGetOutstanding";

                DataTable NewTaskTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, NewTaskStr);
                DataTable OutstandingTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, OutstandingStr);

                DataTable dtAllPdTask = new DataTable();
                dtAllPdTask = NewTaskTB.Copy();

                dtAllPdTask.Merge(OutstandingTB, true);

                dtAllPdTask.TableName = "dtAllPdTask";

                //if (dtAllPdTask.Rows.Count > 0)
                //{
                //    foreach (DataRow dr1 in dtAllPdTask.Rows)
                //    {
                //        if (dr1.RowState != DataRowState.Deleted)
                //        {
                //            if (!BizFunctions.IsEmpty(dr1["Done Date"]))
                //            {
                //                dr1["Done Date"] = TimeUtilites.TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dr1["Done Date"]));
                //            }
                //            if (!BizFunctions.IsEmpty(dr1["Last Done"]))
                //            {
                //                dr1["Last Done"] = TimeUtilites.TimeTools.GetStandardSafeDateOnly(Convert.ToDateTime(dr1["Last Done"]));
                //            }
                //        }
                //    }
                //}

                if (enableDocSave)
                {
                    GetSignature();
                }

                this.dbaccess.ReadSQL(selectedCollection);

                if (this.dbaccess.DataSet.Tables.Contains("dtAllPdTask"))
                {
                    this.dbaccess.DataSet.Tables["dtAllPdTask"].Dispose();
                    this.dbaccess.DataSet.Tables.Remove("dtAllPdTask");
                    this.dbaccess.DataSet.Tables.Add(dtAllPdTask);

                }
                else
                {
                    this.dbaccess.DataSet.Tables.Add(dtAllPdTask);
                }


                //WORfilter wf = new WORfilter(this.dbaccess);
                //wf.ShowDialog();
                //wf.Focus();
            }
        }

        private void Datagrid2_MouseClick(object sender, MouseEventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
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

                    DataRow drCur = getcurrentrow(Datagrid1);

                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim(), "WOR1");

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

        private void Datagrid2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
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

                    DataRow drCur = getcurrentrow(Datagrid1);

                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim(), "WOR1");

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

        BindingSource bindSrc = null;

        private void Initialise()
        {
            btnLists = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btnLists") as Button;
            btnLists.Click += new EventHandler(btnLists_Click);

            if (moduleName.Trim() == "SVC" || moduleName.Trim() == "PWOR")
            {
                btnListsPic = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btnListsPic") as Button;
                btnListsPic.Click += new EventHandler(btnListsPic_Click);
            }

            this.ImagePath = ConfigurationManager.AppSettings.Get("ImagePath");
        }

        void btnListsPic_Click(object sender, EventArgs e)
        {
            DataRow WORH = this.dbaccess.DataSet.Tables["WORH"].Rows[0];
            DataTable WOR1 = this.dbaccess.DataSet.Tables["WOR1"];

            ATL.ExtractWORpics1.ExtractWORpics1 Ewor = new ATL.ExtractWORpics1.ExtractWORpics1(dbaccess, WORH["refnum"].ToString());

            Ewor.Show();
            Ewor.Focus();
        }

        void locColumnButton1_Click(object sender, EventArgs e)
        {
            DataTable wor4 = this.dbaccess.DataSet.Tables["WOR4"];

            if (!BizFunctions.IsEmpty(wor4.Rows[dg_location.CurrentCell.RowNumber]["servicerptloc"].ToString()))
            {
                Process.Start(wor4.Rows[dg_location.CurrentCell.RowNumber]["servicerptloc"].ToString());
            }
            else
            {
                MessageBox.Show("Not Signed yet", "No Signature", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
          
        }

        void dg_location_CurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        void dg_location_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        void dg_location_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        void columnButton_Click(object sender, EventArgs e)
        {
            DataRow WORH = this.dbaccess.DataSet.Tables["WORH"].Rows[0];
            DataTable WOR1 = this.dbaccess.DataSet.Tables["WOR1"];

            ATL.ExtractWORpics.ExtractWORpics Ewor = new ATL.ExtractWORpics.ExtractWORpics(dbaccess, WORH["refnum"].ToString(),WOR1.Rows[dg_newtask.CurrentCell.RowNumber]["guid"].ToString());

            Ewor.Show();
            Ewor.Focus();
        }

        void dg_newtask_CurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        void dg_newtask_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        void dg_newtask_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void Datagrid1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
              
            try
            {
                isMouseClicked = true;
                #region Get and Set Row

                DataGrid dataGrid = sender as DataGrid;

                int i = dataGrid.CurrentRowIndex;
                System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                System.Windows.Forms.DataGrid.HitTestInfo hti = dataGrid.HitTest(p);

                if (hti.Type == DataGrid.HitTestType.Cell || hti.Type == DataGrid.HitTestType.RowHeader)
                {// if user double click Row Header or Cell, the selected row will be added to CRQ2.
                    dataGrid.Select(i);

                    DataRow drCur = getcurrentrow(Datagrid2);
                                            
                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, drCur["empnum"].ToString().Trim(),"WOR1");

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

        private DataRow getcurrentrow(DataGrid datagrid)
        {
            CurrencyManager cm = (CurrencyManager)datagrid.BindingContext[datagrid.DataSource, datagrid.DataMember];
            DataRowView drv = cm.Current as DataRowView;
            DataRow dr = drv.Row;

            return dr;
        }

        private void Voucher_WOR1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {           

            switch (e.Column.ColumnName)
            {
                case "shiftcode":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["shiftcode"]))
                        {
                            if (BizFunctions.IsEmpty(e.Row["timein"]))
                            {
                                e.Row["timein"] = geTimeIn(e.Row["shiftcode"].ToString());
                            }
                            if (BizFunctions.IsEmpty(e.Row["timeout"]))
                            {
                                e.Row["timeout"] = geTimeOut(e.Row["shiftcode"].ToString());
                            }
                            if (isWorkShift(e.Row["shiftcode"].ToString()) && !BizFunctions.IsEmpty(e.Row["Date"]))
                            {
                                e.Row["Scheddatein"] = Convert.ToDateTime(e.Row["Date"]).ToShortDateString();
                            }
                            else
                            {
                                e.Row["Scheddatein"] = System.DBNull.Value;
                            }
                        }
                    }
                    break;

                case "Date":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["date"]) && BizFunctions.IsEmpty(e.Row["Day"]))
                        {
                            e.Row["Day"] = TimeTools.GetDay(TimeTools.GetDayOfWeekNo(Convert.ToDateTime(e.Row["Date"]).DayOfWeek.ToString()));
                        }
                    }
                    break;
                       
                case "confirmedtimein":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]))
                        {
                            if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                            {
                                e.Row["confirmedtimein"] = System.DBNull.Value;
                            }
                            else
                            {
                                if (!BizFunctions.IsEmpty(e.Row["confirmedtimeout"]))
                                {
                                    if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                                    {
                                        e.Row["confirmedtimeout"] = System.DBNull.Value;
                                    }
                                    else
                                    {

                                        if (Convert.ToDouble(e.Row["confirmedtimein"]) <= Convert.ToDouble(e.Row["confirmedtimeout"]))
                                        {
                                            e.Row["totalhrs"] = Math.Round(Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), e.Row["confirmedtimeout"].ToString())), 1, MidpointRounding.AwayFromZero);
                                            //e.Row["totalhrs"] =Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), e.Row["confirmedtimeout"].ToString()));
                                        }
                                        else
                                        {
                                            decimal tmpTTL1, tmpTTL2 = 0;
                                            tmpTTL1 = (decimal)Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), "2359"));
                                            tmpTTL2 = (decimal)Math.Abs(GetHours("0001", e.Row["confirmedtimeout"].ToString()));
                                            e.Row["totalhrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        }

                                    }

                                    if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]) && !BizFunctions.IsEmpty(e.Row["confirmedtimeout"]) && !BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                    {
                                        decimal tmpTTL1, tmpTTL2, tmpTTL3, tmpTTL4, ftime1, ftime2 = 0;

                                        tmpTTL1 = (decimal)Math.Abs(GetMins(e.Row["confirmedtimein"].ToString(), "2359"));
                                        //tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["confirmedtimeout"].ToString()));
                                        tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime1 = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        tmpTTL3 = (decimal)Math.Abs(GetMins(e.Row["timein"].ToString(), "2359"));
                                        tmpTTL4 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime2 = Math.Round((tmpTTL3 + tmpTTL4), 1, MidpointRounding.AwayFromZero);

                                        if (ftime2 > ftime1)
                                        {
                                            e.Row["LateMins"] = ftime2 - ftime1;
                                        }
                                        else
                                        {
                                            e.Row["LateMins"] = 0;
                                        }

                                    }
                                }
                            }

                        }
                    }
                    break;

                case "confirmedtimeout":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["confirmedtimeout"]))
                        {
                            if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimeout"].ToString()))
                            {
                                e.Row["confirmedtimeout"] = System.DBNull.Value;
                            }
                            else
                            {
                                if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]))
                                {
                                    if (!ATL.BizModules.TextValidator.TextValidator.IsvalidMilitaryTime(e.Row["confirmedtimein"].ToString()))
                                    {
                                        e.Row["confirmedtimein"] = System.DBNull.Value;
                                    }
                                    else
                                    {

                                        if (Convert.ToDouble(e.Row["confirmedtimein"]) <= Convert.ToDouble(e.Row["confirmedtimeout"]))
                                        {
                                            e.Row["totalhrs"] = Math.Round(Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), e.Row["confirmedtimeout"].ToString())), 1, MidpointRounding.AwayFromZero);

                                        }
                                        else
                                        {
                                            decimal tmpTTL1, tmpTTL2 = 0;

                                            tmpTTL1 = (decimal)Math.Abs(GetHours(e.Row["confirmedtimein"].ToString(), "2359"));
                                            tmpTTL2 = (decimal)Math.Abs(GetHours("0001", e.Row["confirmedtimeout"].ToString()));
                                            e.Row["totalhrs"] = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        }

                                    }

                                    if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]) && !BizFunctions.IsEmpty(e.Row["confirmedtimeout"]) && !BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                    {
                                        decimal tmpTTL1, tmpTTL2, tmpTTL3, tmpTTL4, ftime1, ftime2 = 0;

                                        tmpTTL1 = (decimal)Math.Abs(GetMins(e.Row["confirmedtimein"].ToString(), "2359"));
                                        //tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["confirmedtimeout"].ToString()));
                                        tmpTTL2 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime1 = Math.Round((tmpTTL1 + tmpTTL2), 1, MidpointRounding.AwayFromZero);

                                        tmpTTL3 = (decimal)Math.Abs(GetMins(e.Row["timein"].ToString(), "2359"));
                                        tmpTTL4 = (decimal)Math.Abs(GetMins("0001", e.Row["timeout"].ToString()));

                                        ftime2 = Math.Round((tmpTTL3 + tmpTTL4), 1, MidpointRounding.AwayFromZero);

                                        if (ftime2 > ftime1)
                                        {
                                            e.Row["LateMins"] = ftime2 - ftime1;
                                        }
                                        else
                                        {
                                            e.Row["LateMins"] = 0;
                                        }

                                    }

                                    // GET OT MINS
                                    if (!BizFunctions.IsEmpty(e.Row["totalhrs"]))
                                    {
                                        int TotalHrs = Convert.ToInt32(e.Row["totalhrs"]);
                                        if (TotalHrs > 0)
                                        {
                                            if (!BizFunctions.IsEmpty(e.Row["confirmedtimein"]) && !BizFunctions.IsEmpty(e.Row["confirmedtimeout"]) && !BizFunctions.IsEmpty(e.Row["timein"]) && !BizFunctions.IsEmpty(e.Row["timeout"]))
                                            {
                                                decimal tmpTTL1 = 0;

                                                tmpTTL1 = (decimal)Math.Abs(GetMins(e.Row["confirmedtimeout"].ToString(), e.Row["timeout"].ToString()));


                                                if (tmpTTL1 > 0)
                                                {
                                                    e.Row["OtMins"] = tmpTTL1;
                                                }
                                                else
                                                {
                                                    e.Row["OtMins"] = 0;
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "empnum":
                    {
                        e.Row["empnum2"] = e.Row["empnum"].ToString();
                        e.Row["empnum3"] = e.Row["empnum"].ToString();
                        e.Row["empnum4"] = e.Row["empnum"].ToString();
                        if (!BizFunctions.IsEmpty(e.Row["empnum"]))
                        {
                            e.Row["paytypecode"] = GetPayTypeCode(e.Row["empnum"].ToString());
                        }
                    }
                    break;

                case "rempnum":
                    {
                        if (!BizFunctions.IsEmpty(e.Row["rempnum"]))
                        {
                            e.Row["rempname"] = getEmpName(e.Row["rempnum"].ToString());
                        }
                    }
                    break;
            }
        }

        private double GetHours(string start, string end)
        {
            double hourstaken;
            LocalTime dt1 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(start);
            LocalTime dt2 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(end);

            Duration dr1 = new Duration(dt1.TickOfDay);
            Duration dr2 = new Duration(dt2.TickOfDay);

            Duration dr3 = Duration.Subtract(dr1, dr2);

            TimeSpan elapsedSpan1 = new TimeSpan(dr3.Ticks);

            return hourstaken = elapsedSpan1.TotalHours;
        }

        private double GetMins(string start, string end)
        {
            double hourstaken;
            LocalTime dt1 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(start);
            LocalTime dt2 = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(end);

            Duration dr1 = new Duration(dt1.TickOfDay);
            Duration dr2 = new Duration(dt2.TickOfDay);

            Duration dr3 = Duration.Subtract(dr1, dr2);

            TimeSpan elapsedSpan1 = new TimeSpan(dr3.Ticks);

            return hourstaken = elapsedSpan1.TotalMinutes;

        }

        private string getEmpName(string empno)
        {
            string EmpName = "";

            string GetEmpno = "Select empname from HEMPHtmp1 where empnum='" + empno + "'";

            DataTable HemphTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetEmpno);

            if (HemphTmp.Rows.Count > 0)
            {
                EmpName = HemphTmp.Rows[0]["empname"].ToString();
            }

            return EmpName;
        }

        private string geTimeIn(string shiftcode)
        {
            string Timein = "";

            string GetvSHLV = "Select timein from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timein = vSHLVTmp.Rows[0]["timein"].ToString();
            }
            vSHLVTmp.Dispose();
            return Timein;
        }

        private string geTimeOut(string shiftcode)
        {
            string Timeout = "";

            string GetvSHLV = "Select [timeout] from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable vSHLVTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetvSHLV);

            if (vSHLVTmp.Rows.Count > 0)
            {
                Timeout = vSHLVTmp.Rows[0]["timeout"].ToString();
            }
            vSHLVTmp.Dispose();
            return Timeout;
        }

       

        void btnLists_Click(object sender, EventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            if (!BizFunctions.IsEmpty(worh["worDateFrom"]) && !BizFunctions.IsEmpty(worh["worDateTo"]))
            {
                if (!BizFunctions.IsEmpty(worh["docunum"]) && !BizFunctions.IsEmpty(worh["arnum"]) && !BizFunctions.IsEmpty(worh["sitenum"]))
                {
                    GetByCtr();
                }
                if (!BizFunctions.IsEmpty(worh["adhnum"]) && !BizFunctions.IsEmpty(worh["arnum"]) && !BizFunctions.IsEmpty(worh["sitenum"]))
                {
                    GetByAdHoc();
                }
                else if (BizFunctions.IsEmpty(worh["adhnum"]) && !BizFunctions.IsEmpty(worh["arnum"]) && BizFunctions.IsEmpty(worh["sitenum"]))
                {
                    GetByCustomer();
                }
                else if (BizFunctions.IsEmpty(worh["adhnum"]) && !BizFunctions.IsEmpty(worh["arnum"]) && !BizFunctions.IsEmpty(worh["sitenum"]) && BizFunctions.IsEmpty(worh["docunum"]))
                {
                    GetByLocation();
                }
                else if (BizFunctions.IsEmpty(worh["adhnum"]) && BizFunctions.IsEmpty(worh["arnum"]) && BizFunctions.IsEmpty(worh["sitenum"]) && !BizFunctions.IsEmpty(worh["sectorcode"]))
                {
                    GetBySector();
                }
                if (BizFunctions.IsEmpty(worh["docunum"]) && BizFunctions.IsEmpty(worh["arnum"]) && BizFunctions.IsEmpty(worh["sitenum"]) && BizFunctions.IsEmpty(worh["sectorcode"]) && BizFunctions.IsEmpty(worh["adhnum"]))
                {
                    GetByOE();
                }
            }
            else
            {
                MessageBox.Show("Empty Date From / To", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            
        }

        private void Btn_Show_Click(object sender, EventArgs e)
        {
            if (txt_editEmpno.Text != string.Empty)
            {
                try
                {
                    ExtractATR1Form1.ExtractATR1Form1 eATR1 = new ATL.ExtractATR1Form1.ExtractATR1Form1(this.dbaccess, txt_editEmpno.Text, "WOR1");

                    eATR1.Show();
                    eATR1.Focus();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        private void txt_editEmpno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                if (txt_editEmpno.Text != string.Empty)
                {
                    F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + txt_editEmpno.Text + "%' and sectorcode='" + SectorCode + "'", null, F2Type.Sort);

                    f2BaseHelper.F2_Load();

                    if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                    {

                        txt_editEmpno.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                    }
                }
            }
        }

        private void txt_editEmpno_DoubleClick(object sender, EventArgs e)
        {
            if (txt_editEmpno.Text != string.Empty)
            {
                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", e, "empnum", "empnum like '" + txt_editEmpno.Text + "%' and sectorcode='" + SectorCode + "'", null, F2Type.Sort);

                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {

                    txt_editEmpno.Text = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
                }
            }

        }

        private void Btn_Extract_Click(object sender, EventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["WORH"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["WOR1"];
            //if (!BizFunctions.IsEmpty(worh["weekno"]) && !BizFunctions.IsEmpty(worh["weekyear"]))
            //{
            //    DateTime FirstDay = TimeTools.GetFirstDayOfWeek(Convert.ToInt32(worh["weekyear"]), Convert.ToInt32(worh["weekno"]));

            //    DateTime EndDay = FirstDay.AddDays(6);

            //    worh["WeekDateFrom"] = FirstDay;
            //    worh["WeekDateTo"] = EndDay;

            //    Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            //    DataTable oriTable = wor1;
            //    try
            //    {
            //        ExtractATRWForm ExtractATR1 = new ExtractATRWForm(this.dbaccess, oriTable,SectorCode);
            //        ExtractATR1.ShowDialog(frm);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please Fill up both Week and Year", "Empty Week / Year No", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            //if (!BizFunctions.IsEmpty(worh["WeekDateFrom"]) && !BizFunctions.IsEmpty(worh["WeekDateTo"]))
            //{
            //    if (Convert.ToDateTime(worh["WeekDateFrom"]) <= Convert.ToDateTime(worh["WeekDateTo"]))
            //    {
            //        Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            //        DataTable oriTable = wor1;
            //        try
            //        {
            //            ExtractATRWForm ExtractATR1 = new ExtractATRWForm(this.dbaccess, oriTable, SectorCode);
            //            ExtractATR1.ShowDialog(frm);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Date To can't be earlier than Date From", "Wrong Date", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Either Date From / To Can't Be Empty", "Empty Dates", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            
        }

        private string GetPayTypeCode(string empnum)
        {
            string paytype = "";

            string getPayType = "Select paytypecode from HEMPHtmp1 where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getPayType);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                paytype = dr1["paytypecode"].ToString();
            }

            return paytype;
        }

        private decimal GetDaysPerWeek(string empnum)
        {
            decimal daysPerWeek = 0;

            string getDaysPerWK = "Select daysperweek from vMainHEMPH where empnum='" + empnum + "'";
            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, getDaysPerWK);

            if (dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];

                if (!BizFunctions.IsEmpty(dr1["daysperweek"]))
                {
                    daysPerWeek = Convert.ToDecimal(dr1["daysperweek"]);
                }
               
            }

            return daysPerWeek;
        }

        private decimal GetPayTypeValue(string paytypecode)
        {
            decimal value = 0;

            string GetPayTypeValue = "Select ISNULL(value,0) as value from PAYTM where paytypecode='" + paytypecode + "'";

            DataTable dt2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetPayTypeValue);

            if (dt2.Rows.Count > 0)
            {
                DataRow dr2 = dt2.Rows[0];

                value = Convert.ToDecimal(dr2["value"]);
            }



            return value;
        }

        #region Save Handle

        protected override void 
            
        Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            //DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];

            DataTable wor5 = this.dbaccess.DataSet.Tables["wor5"];

            //if (wor1.Rows.Count > 0)
            //{
            //    if (!BizFunctions.IsEmpty(worh["worDateFrom"]))
            //    {
            //        foreach (DataRow dr1 in wor1.Rows)
            //        {
            //            if (dr1.RowState != DataRowState.Deleted)
            //            {
            //                if (BizFunctions.IsEmpty(dr1["eddate"]))
            //                {
            //                    dr1["eddate"] = worh["worDateFrom"];
            //                }
            //            }
            //        }
            //    }
            //}


            if (BizFunctions.IsEmpty(worh["isurgent"]))
            {
                worh["isurgent"] = 0;
            }

            //worh["sectorcode"] = SectorCode;

            //string CheckExists = "Select * from worh where weekno='"+worh["weekno"].ToString()+"' and weekyear='"+worh["weekyear"].ToString()+"' and sectorcode='"+SectorCode+"' and refnum<>'"+worh["refnum"].ToString()+"' and [status]<>'V' ";

            //this.dbaccess.ReadSQL("tmpCheck", CheckExists);

            //DataTable tmpCheck = this.dbaccess.DataSet.Tables["tmpCheck"];

            //if (tmpCheck.Rows.Count > 0)
            //{
            //    MessageBox.Show("There is already a Refnum(" + tmpCheck.Rows[0]["refnum"].ToString() + ") for this Week of the Year ", "Not Saved!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    e.Handle = false;
            //}


            foreach (DataRow dr5 in wor5.Rows)
            {
                if (dr5.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(worh, dr5, "refnum/user/flag/status/trandate/createdby/created/modified");
                }

            }
  
        }


        #region DocumentF2

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {           
            base.AddDocumentF3Condition(sender, e);
            DataRow worh = this.dbaccess.DataSet.Tables["WORH"].Rows[0];
            switch (e.ControlName)
            {
                case "worh_worhnum":
                    {
                        //if (!BizFunctions.IsEmpty(worh["worhnum"].ToString()))
                        //{
                        //    GetWrr();
                        //    GetATMR();
                        //}
                    }
                    break;

                case "worh_adhnum":
                    {
                        e.CurrentRow["arnum"] = e.F2CurrentRow["arnum"];
                        e.CurrentRow["sitenum"] = GetSitenumi(worh["adhnum"].ToString());
                        e.CurrentRow["sectorcode"] = BizLogicTools.Tools.GetSectorCode(e.CurrentRow["sitenum"].ToString(), this.dbaccess);

                    }
                    break;

                case "worh_docunum":
                    {
                        DataRow dr = BizLogicTools.Tools.GetCommonEmpDataRowByDoc(e.CurrentRow["docunum"].ToString());
                        e.CurrentRow["arnum"] = e.F2CurrentRow["arnum"];
                        e.CurrentRow["sitenum"] = GetSitenumCTR(worh["docunum"].ToString());
                        e.CurrentRow["sectorcode"] = BizLogicTools.Tools.GetSectorCode(e.CurrentRow["sitenum"].ToString(), this.dbaccess);
                        e.CurrentRow["empnum"] = dr["empnum"].ToString();
                        e.CurrentRow["empname"] = dr["empname"].ToString();

                    }
                    break;

                case "worh_arnum":
                    {
                        e.CurrentRow["arname"] = e.F2CurrentRow["arname"];
                        e.CurrentRow["serviceaddress"] = e.F2CurrentRow["addr1"].ToString() + ' ' + e.F2CurrentRow["addr2"].ToString() + ' ' + e.F2CurrentRow["addr3"].ToString();
                        //e.CurrentRow["postal"] = e.F2CurrentRow["postal"];
                        e.CurrentRow["ptc"] = e.F2CurrentRow["ptc"];
                        e.CurrentRow["ptccontact"] = e.F2CurrentRow["phone"];
                        e.CurrentRow["adhnum"] = string.Empty;
                        e.CurrentRow["sitenum"] = string.Empty;
                        e.CurrentRow["empnum"] = string.Empty;
                        e.CurrentRow["empname"] = string.Empty;
                    }
                    break;

                case "worh_sitenum":
                    {
                        e.CurrentRow["arnum"] = e.F2CurrentRow["arnum"];
                        e.CurrentRow["sectorcode"] = BizLogicTools.Tools.GetSectorCode(e.CurrentRow["sitenum"].ToString(), this.dbaccess);
                        e.CurrentRow["sitename"] = e.F2CurrentRow["sitename"];
                        if (BizFunctions.IsEmpty(e.CurrentRow["serviceaddress"]))
                        {
                            e.CurrentRow["serviceaddress"] = e.F2CurrentRow["addr1"].ToString() + ' ' + e.F2CurrentRow["addr2"].ToString() + ' ' + e.F2CurrentRow["addr3"].ToString();
                        }
                        e.CurrentRow["sitenumt"] = e.F2CurrentRow["sitenumt"];
                        e.CurrentRow["adhnum"] = GetAhocNo(e.CurrentRow["sitenumt"].ToString());
                    }
                    break;

                case "worh_sectorcode":
                    {
                        e.CurrentRow["arnum"] = string.Empty;
                        e.CurrentRow["arname"] = string.Empty;
                        e.CurrentRow["sitenum"] = string.Empty;
                        e.CurrentRow["sitename"] = string.Empty;
                        e.CurrentRow["sitenumt"] = string.Empty;
                        e.CurrentRow["adhnum"] = string.Empty;
                        e.CurrentRow["empnum"] = string.Empty;
                        e.CurrentRow["empname"] = string.Empty;
                    }
                    break;

                case "worh_empnum":
                    {

                        e.CurrentRow["sectorcode"] = string.Empty;
                        e.CurrentRow["docunum"] = string.Empty;
                        e.CurrentRow["arnum"] = string.Empty;
                        e.CurrentRow["arname"] = string.Empty;
                        e.CurrentRow["sitenum"] = string.Empty;
                        e.CurrentRow["sitename"] = string.Empty;
                        e.CurrentRow["sitenumt"] = string.Empty;
                        e.CurrentRow["adhnum"] = string.Empty;

                        e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    }
                    break;


            }
        }

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
                case "worh_worhnum":
                    {                      
                        e.DefaultCondition = "refnum not in (Select worhnum from worh where status<>'V') and status<>'V'";               
                    }
                    break;

                //case "worh_fromempnum":
                //    {
                //        e.DefaultCondition = "paytypecode='W' and status<>'V' and sectorcode='"+SectorCode+"' ";
                //    }
                //    break;

                //case "worh_toempnum":
                //    {
                //        e.DefaultCondition = "paytypecode='W' and status<>'V' and sectorcode='" + SectorCode + "' ";
                //    }
                //    break;
          
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

        #region  btn_Sort_Click
        protected void Btn_Sort_Click(object sender, System.EventArgs e)
        {
            //DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            //TableColumn = (ComboBox)BizXmlReader.CurrentInstance.GetControl(NewTaskForm, "TableColumn");


            //string cname = TableColumn.Text.ToString();
            //if (cname != "")
            //{
            //    if (wor1.Rows.Count > 0)
            //    {
            //        SortDT sort = new SortDT(wor1, cname + " ASC");
            //        DataTable returnedfinalextraction = sort.SortedTable();

            //        BizFunctions.DeleteAllRows(wor1);

            //        foreach (DataRow dr in returnedfinalextraction.Select())
            //        {
            //            if (dr.RowState != DataRowState.Deleted)
            //            {
            //                wor1.ImportRow(dr);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please Choose Column To Sort !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;

            //}

        }
        #endregion

        #region trq ReOpen/void

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);
            DataRow sivh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            if (sivh["flag"].ToString().Trim() != "SVC")
            {
                e.Handle = false;
            }
            else
            {
                try
                {
                    File.Delete(filepath + @"\" + sivh["refnum"].ToString() + ".pdf");
                }
                catch (Exception ex) { }
            }
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

        private string GetEmpname(string empnum)
        {
            string empName = "";

            string Get = "Select Empname From HEMPHtmp1 where empnum='" + empnum + "'";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, Get);

            if (dt1.Rows.Count > 0)
            {
                empName = dt1.Rows[0]["empname"].ToString();
            }

            dt1.Dispose();

            return empName;

        }

        private void GetSummary()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor2 = this.dbaccess.DataSet.Tables["wor2"];

            string sql1 = "SELECT " +
                                    "b.empnum, " +
                                    "h.empname, " +
                                    "b.wd as totalpayabledays, " +
                                    "b.tothrs as TotalwkHrs, " +
                                    "b.rcount as totalRest, " +
                                    "b.dre as totaldre, " +
                                             "b.ot2 as totalot, " +
                                    "b.ot15hrs, " +
                                    "b.latecount as TotalLatecount, " +
                                    "(b.latemins/60) as TotalLateHrs, " +
                                    "b.pl as totalAleave, " +
                                    "b.mc as totalmc, " +
                                    "b.npl as totalupl, " +
                                    "b.awol totalawol  " +
                                    "from " +
                                    "( " +
                                         "select empnum,SUM(wdcount) as wd,SUM(mccount) as mc, SUM (nplcount) as npl, SUM(awolcount) as awol, SUM (plcount) as pl, SUM (hpcount) as hp, " +
                                              "SUM(phcount) as ph, SUM(workhrs) as tothrs,SUM(latecount) as latecount,sum(latemins) as latemins, sum(standbyhrs) as standbyhrs,sum(ot15hrs) as ot15hrs, " +
                                              "SUM(dre) as dre, SUM(rcount) as rcount,SUM(ot2) as ot2  " +
                                         "from " +
                                         "( " +
                                              "select empnum,[Date] as WorkDate,[DAY],paytypecode,shiftcode,sectorcode,sitenum,timein,confirmedtimein,[timeout],confirmedtimeout, " +
                                                   "case when shiftcode in ('MED','MC') then 1 else 0 end as mccount, " +
                                                   "case when shiftcode in ('HPL','HOS') then 1 else 0 end as hpcount, " +
                                                   "case when shiftcode like '%UPL%' then 1 else 0 end as nplcount, " +
                                                   "case when shiftcode like 'AWO%' then 1 else 0 end as awolcount, " +
                                                   "case when shiftcode in ('AL','A/L') then 1 else 0 end as plcount, " +
                                                   "case when shiftcode like 'RES%' then 1 else 0 end as rcount, " +
                                                   "case when shiftcode in ('PH') then 1 else 0 end as phcount, " +
                                                   "case when ISNULL(TotalHrs,0)>0 then 1 else 0 end as wdcount,  " +
                                                   "case when isnull(isDRE,0)=1 then 1 else 0 end as dre, " +
                                                   "case when isnull(isRD,0)=1 then 1 else 0 end as rd, " +
                                                   "case when isnull(isOT,0)=1 then 1 else 0 end as ot2, " +
                                                   "ISNULL(TotalHrs,0) as workhrs, " +
                                                   "case when ISNULL(LateMins,0)>10 then 1 else 0 end as latecount, " +
                                                   "CASE WHEN ISNULL(latemins,0)<=10 THEN 0 ELSE ISNULL(latemins,0) end as latemins, " +
                                                   "CASE WHEN TotalHrs>7.33 THEN TotalHrs-7.33 else 0 end as ot15hrs, " +
                                                   "CASE WHEN ISNULL(otmins,0)>15 THEN ISNULL(otmins,0)/60 ELSE 0 end as standbyhrs, " +
                                                   "atrnum as reference  " +
                                              "from wor1 " +
                                              "where refnum='" + worh["refnum"].ToString() + "' " +
                                         ") a " +
                                         "group by empnum " +
                                    ") b " +
                                    "left join  " +
                                    "( " +
                                    "select empnum,empname " +
                                    "from hemph " +
                                    ") h on h.empnum = b.empnum";


            this.dbaccess.ReadSQL("tmpWOR2", sql1);

            DataTable tmpWOR2 = this.dbaccess.DataSet.Tables["tmpWOR2"];

            BizFunctions.DeleteAllRows(wor2);

            if (tmpWOR2.Rows.Count > 0)
            {
                foreach (DataRow dr1 in tmpWOR2.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertWOR2 = wor2.NewRow();
                        InsertWOR2["empnum"] = dr1["empnum"];
                        InsertWOR2["empname"] = dr1["empname"];
                        InsertWOR2["totalpayabledays"] = dr1["totalpayabledays"];
                        InsertWOR2["TotalwkHrs"] = dr1["TotalwkHrs"];
                        InsertWOR2["totalRest"] = dr1["totalRest"];
                        InsertWOR2["totaldre"] = dr1["totaldre"];
                        InsertWOR2["totalot"] = dr1["totalot"];
                        InsertWOR2["ot15hrs"] = dr1["ot15hrs"];
                        InsertWOR2["TotalLatecount"] = dr1["TotalLatecount"];
                        InsertWOR2["TotalLateHrs"] = dr1["TotalLateHrs"];
                        InsertWOR2["totalAleave"] = dr1["totalAleave"];
                        InsertWOR2["totalmc"] = dr1["totalmc"];
                        InsertWOR2["totalupl"] = dr1["totalupl"];
                        InsertWOR2["totalawol"] = dr1["totalawol"];
                        wor2.Rows.Add(InsertWOR2);
                    }
                }
            }         
        }    

        #region Save Begin
        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            DataRow worh = this.dbaccess.DataSet.Tables["WORH"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["WOR1"];
            base.Document_SaveBegin_OnClick(sender, e);

            if (BizFunctions.IsEmpty(worh["trandate"]))
            {
                worh["trandate"] = DateTime.Now;
            }

            #region WOR1
            //foreach (DataRow dr1 in wor1.Rows)
            //{
            //    if (dr1.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(worh, dr1, "refnum/user/flag/sectorcode/status/trandate/createdby/created/modified");
            //    }

            //}
            #endregion      
            if (enableDocSave)
            {
                if (worh["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSV)
                {
                    try
                    {
                        string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

                        string SaveLoc = DriveLetter + ":";
                        System.IO.DirectoryInfo fl = new DirectoryInfo(SaveLoc + @"\\WOR\\SVC\\" + worh["refnum"].ToString() + "\\");

                        if (!fl.Exists)
                        {
                            System.IO.Directory.CreateDirectory(fl.FullName);
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

            if (BizFunctions.IsEmpty(worh["createdby"]))
            {
                worh["createdby"] = worh["user"];
            }
                                                
        }
        #endregion

        private bool isWorkShift(string shiftcode)
        {
            bool isWorkShift = false;

            string GetIsWorkShift = "Select isWorkShift from vSHLV where shiftcode='" + shiftcode + "'";

            DataTable GetIsWorkShiftTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, GetIsWorkShift);

            if (GetIsWorkShiftTmp.Rows.Count > 0)
            {
                if (BizFunctions.IsEmpty(GetIsWorkShiftTmp.Rows[0]["isWorkShift"]))
                {
                    GetIsWorkShiftTmp.Rows[0]["isWorkShift"] = 0;
                }
                isWorkShift = (bool)Convert.ToBoolean(GetIsWorkShiftTmp.Rows[0]["isWorkShift"]);
            }

            return isWorkShift;
        }
        
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
            DataRow worh = dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = dbaccess.DataSet.Tables["wor4"];

            #region Refresh Outstanding
            if (!BizFunctions.IsEmpty(worh["searchType"]))
            {
                if (worh["searchType"].ToString().Trim() == "ADH")
                {
                    GetOutstandingByAdh();
                }
                if (worh["searchType"].ToString().Trim() == "ARM")
                {
                    GetOutstandingByArm();
                }
                if (worh["searchType"].ToString().Trim() == "SITM")
                {
                    GetOutstandingBySitmi();
                }
                if (worh["searchType"].ToString().Trim() == "SEM")
                {
                    GetOutstandingBySector();
                }
            }
            #endregion

       

        }
        #endregion

        #region Confirm on Click

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);
            DataRow svc = dbaccess.DataSet.Tables["worh"].Rows[0];
            try
            {
                Form form = BizXmlReader.CurrentInstance.Load(@"FormPreviewWithCancel.xml", "formPreview", this, null) as Form;
                CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;
                ReportDocument crReportDocument = new ReportDocument();
                crReportDocument.Load(this.projectPath + @"\SVC\Report\ServiceReportForm1.rpt");

                Hashtable selectedCollection = new Hashtable();
                selectedCollection.Add("coy", "SELECT * FROM coy");
                //selectedCollection.Add("matm", "SELECT * FROM matm");
                //selectedCollection.Add("apm", "SELECT top 1 * FROM apm where apnum='" + porh["apnum"].ToString().Trim() + "'");
                //selectedCollection.Add("ard", "SELECT * FROM ard" + Common.DEFAULT_SYSTEM_YEAR + " where refnum = '" + recp["refnum"].ToString().Trim() + "'");

                e.DBAccess.ReadSQL(selectedCollection);

                crReportDocument.SetDataSource(e.DBAccess.DataSet);
                crystalReportViewer1.ReportSource = crReportDocument;
                crystalReportViewer1.Refresh();

                ExportOptions CrExportOptions;
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                string ServerProjDir = filepath;
                CrDiskFileDestinationOptions.DiskFileName = filepath + @"\" + svc["refnum"].ToString() + ".pdf";
                if (!Directory.Exists(ServerProjDir))
                {
                    //Create a new subfolder under the current active folder
                    string newFolder = System.IO.Path.Combine(ServerProjDir, "");
                    // Create the subfolder
                    System.IO.Directory.CreateDirectory(newFolder);
                }
                CrExportOptions = crReportDocument.ExportOptions;
                {
                    CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;
                }
                crReportDocument.Export();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region Preview on Click

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow worh = dbaccess.DataSet.Tables["worh"].Rows[0];
            if (worh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "worh/wor1/wor4"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion

        #region Print on Click

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

            DataRow worh = e.DBAccess.DataSet.Tables["worh"].Rows[0];
            Hashtable selectedCollection = new Hashtable();

            //WORfilter wf = new WORfilter(this.dbaccess);
            //wf.Show();
            //wf.Focus();



            switch (e.ReportName)
            {


                case "Cleaning Service Report 1":
                    GetSignature();
                    GetWorkPictures();
                    selectedCollection.Add("Coy", "SELECT top 1 * from coy");
                    selectedCollection.Add("armTmp", "SELECT * from arm where arnum='"+worh["arnum"].ToString()+"'");
                    e.DBAccess.ReadSQL(selectedCollection);
                    e.DataSource = this.dbaccess.DataSet;
                    break;


            }            


        }

        #endregion

        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);

            DataRow worh = dbaccess.DataSet.Tables["worh"].Rows[0];
            if (worh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "worh"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }

        }

        #endregion

        private void GetWrr()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["ATR"].Rows[0];
            string sqlGetWORH = "Select * from worh where refnum='" + worh["worhnum"].ToString().Trim() + "'";
            this.dbaccess.ReadSQL("TBGetWORH", sqlGetWORH);
            DataTable TBGetWORH = this.dbaccess.DataSet.Tables["TBGetWORH"];

            if (TBGetWORH.Rows.Count > 0)
            {
                DataRow drGetWORH = this.dbaccess.DataSet.Tables["TBGetWORH"].Rows[0];
                worh["sectorcode"] = drGetWORH["sectorcode"]; 
                worh["sitenum"] = drGetWORH["sitenum"];
                worh["worhFromDate"] = Convert.ToDateTime(drGetWORH["commencedate"]).ToShortDateString();
                worh["worhToDate"] = Convert.ToDateTime(drGetWORH["enddate"]).ToShortDateString();
           
            }
        }

        private void GetATMR()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["WORH"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["WOR1"];
            if (wor1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor1);
            }
            string sqlGetATR = "Select * from atmr where refnum='" + worh["worhnum"].ToString().Trim() + "'";

            this.dbaccess.ReadSQL("TBGetATMR", sqlGetATR);
            DataTable TBGetATMR = this.dbaccess.DataSet.Tables["TBGetATMR"];
            if (TBGetATMR.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TBGetATMR.Select())
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        DataRow InsertAtr1 = wor1.NewRow();

                        InsertAtr1["nric"] = dr1["nric"];
                        InsertAtr1["empnum"] = dr1["empnum"];                       
                        InsertAtr1["shiftcode"] = dr1["shiftcode"];
                      
                        // timein
                        if (BizFunctions.IsEmpty(dr1["timein"]))
                        {
                            InsertAtr1["timein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["timein"] = dr1["timein"];
                        }
                        //timeout
                        if (BizFunctions.IsEmpty(dr1["timeout"]))
                        {
                            InsertAtr1["timeout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["timeout"] = dr1["timeout"];
                        }
                        //scheddatetiin
                        if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            InsertAtr1["scheddatein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["scheddatein"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimein"]));

                        }
                        //scheddateout



                        if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            InsertAtr1["scheddateout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["scheddateout"] = TimeUtilites.TimeTools.GetSafeDateOnly(Convert.ToDateTime(dr1["actualtimeout"]));
                        }

                        //schedtimein
                        if (BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            InsertAtr1["schedtimein"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                        }

                        ////////////////////////////

                        if (!BizFunctions.IsEmpty(dr1["timein"]) && !BizFunctions.IsEmpty(dr1["actualtimein"]))
                        {
                            LocalTime timein = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timein"].ToString());
                            LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"])));

                            if (timein.TickOfDay < actualtimeout.TickOfDay)
                            {
                                InsertAtr1["confirmedtimein"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimein"]));
                            }
                            else
                            {
                                InsertAtr1["confirmedtimein"] = dr1["timein"].ToString();
                            }
                        }


                        ////////////////////////////

                        //schedtimeout
                        if (BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            InsertAtr1["schedtimeout"] = System.DBNull.Value;
                        }
                        else
                        {
                            InsertAtr1["schedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"]));

                        }

                        if (!BizFunctions.IsEmpty(dr1["timeout"]) && !BizFunctions.IsEmpty(dr1["actualtimeout"]))
                        {
                            LocalTime timeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(dr1["timeout"].ToString());
                            LocalTime actualtimeout = ATL.TimeUtilites.TimeTools.ParseMilitaryTime(TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"])));

                            if (timeout.TickOfDay < actualtimeout.TickOfDay)
                            {
                                InsertAtr1["confirmedtimeout"] = dr1["timeout"].ToString();
                            }
                            else
                            {
                                InsertAtr1["confirmedtimeout"] = TimeUtilites.TimeTools.GetSafeMilitaryTimeOnly1(Convert.ToDateTime(dr1["actualtimeout"]));
                            }
                        }


                        //
                                                                                                    
                        InsertAtr1["replacedby"] = dr1["rempnum"];
                        InsertAtr1["rempname"] = dr1["rempname"];
                        InsertAtr1["rnric"] = dr1["rnric"];
                        InsertAtr1["day"] = dr1["day"];
                        InsertAtr1["date"] = dr1["date"];
                        InsertAtr1["dayofweek"] = TimeTools.GetDayOfWeekNo(dr1["day"].ToString().Trim());
                        InsertAtr1["RepRefKey"] = dr1["RefKey"];
                        wor1.Rows.Add(InsertAtr1);
                    }
                }

            }
        }

        private void GetByOE()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = this.dbaccess.DataSet.Tables["wor4"];

            /////////// Location ////////////

            #region Location

            string strGetByAh = "select refnum as docunum,sectorcode,sitenum,'HQ' as fromsitenum,sitenum as issuedto,addr1,addr2,addr3 from ctrh where empnum='" + worh["empnum"].ToString() + "' and [status]<>'V' ";

            this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

            DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

            if (SITMT_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor4);
                foreach (DataRow dr4 in SITMT_TBtmp.Rows)
                {
                    DataRow InsertWor4 = wor4.NewRow();
                    InsertWor4["ctrnum"] = dr4["docunum"];
                    InsertWor4["sectorcode"] = dr4["sectorcode"];
                    InsertWor4["sitenum"] = dr4["sitenum"];
                    InsertWor4["fromsitenum"] = dr4["fromsitenum"];
                    InsertWor4["tositenum"] = dr4["issuedto"];
                    InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor4.Rows.Add(InsertWor4);
                }
            }

            #endregion

            /////////// New Task //////////// 

            #region New Task

            string getCTR6 = "Select * from CTR6 WHERE refnum in (SELECT refnum from ctrh where empnum='" + worh["empnum"].ToString() + "')  and [status]<>'V' ";
            string getSITMT11 = "Select * from SITMT11 WHERE 1=2";
            this.dbaccess.ReadSQL("SITMT11", getSITMT11);
            this.dbaccess.ReadSQL("CTR6", getCTR6);

            DataTable CTR6 = this.dbaccess.DataSet.Tables["CTR6"];

            if (CTR6.Rows.Count > 0)
            {
                foreach (DataRow dr6 in CTR6.Rows)
                {
                    if (dr6.RowState != DataRowState.Deleted)
                    {
                        dr6["qty"] = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            if (Convert.ToDateTime(worh["worDateFrom"]).Month == i)
                            {
                                if (!BizFunctions.IsEmpty(dr6["xmonth" + i.ToString() + ""]) && !BizFunctions.IsEmpty(dr6["year"]))
                                {
                                    dr6["qty"] = Convert.ToDecimal(dr6["qty"]) + Convert.ToDecimal(CountDaysCTR(i, Convert.ToInt32(dr6["year"]), Convert.ToInt32(Convert.ToBoolean(dr6["xmonth" + i.ToString() + ""])).ToString()));

                                    UpdateScheduleCTR(dr6["svccode"].ToString(), dr6["location"].ToString(), dr6["frequencycode"].ToString(), i.ToString(), dr6["year"].ToString(), Convert.ToInt32(Convert.ToBoolean(dr6["xmonth" + i.ToString() + ""])).ToString());
                                }
                            }
                        }
                    }
                }
            }


            DataTable SITMTPD_TBtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select * from SITMT11");

            if (SITMTPD_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor1);

                foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
                {
                    DataRow InsertWor1 = wor1.NewRow();
                    InsertWor1["ctrnum"] = worh["docunum"];
                    if (!BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                    {
                        InsertWor1["eddate"] = Convert.ToString(dr1["dayNo"]) + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                    }
                    else if (BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                    {
                        InsertWor1["eddate"] = "01" + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                    }

                    InsertWor1["sitenum"] = worh["sitenum"];
                    InsertWor1["locnum"] = dr1["location"];
                    InsertWor1["svccode"] = dr1["svccode"];
                    InsertWor1["frequencycode"] = dr1["frequencycode"];
                    InsertWor1["origuid"] = dr1["guid"];
                    //InsertWor1["guid"] = BizLogicTools.Tools.getGUID(); ;
                    InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                    //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor1.Rows.Add(InsertWor1);
                }
            }


            #endregion

            /////////// Get Outstanding //////////// 

            #region Outstanding

            GetOutstandingByOE();

            #endregion

            worh["searchType"] = "OE";

        }


        private void GetByCtr()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = this.dbaccess.DataSet.Tables["wor4"];

            if (worh["docunum"].ToString().Contains("P"))
            {
                /////////// Location ////////////

                #region Location

                string strGetByAh = "select refnum as docunum,sectorcode,sitenum,'HQ' as fromsitenum,sitenum as issuedto,addr1,addr2,addr3 from ctrh where refnum='" + worh["docunum"].ToString() + "' and sitenum='" + worh["sitenum"].ToString() + "' ";

                this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

                DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

                if (SITMT_TBtmp.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(wor4);
                    foreach (DataRow dr4 in SITMT_TBtmp.Rows)
                    {
                        DataRow InsertWor4 = wor4.NewRow();
                        InsertWor4["ctrnum"] = dr4["docunum"];
                        InsertWor4["sectorcode"] = dr4["sectorcode"];
                        InsertWor4["sitenum"] = dr4["sitenum"];
                        InsertWor4["fromsitenum"] = dr4["fromsitenum"];
                        InsertWor4["tositenum"] = dr4["issuedto"];
                        InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                        wor4.Rows.Add(InsertWor4);
                    }
                }

                #endregion

                /////////// New Task //////////// 

                #region New Task

                string getCTR18 = "Select * from CTR18 WHERE refnum='" + worh["docunum"].ToString() + "' ";
                string getSITMT18 = "Select * from SITMT18 WHERE 1=2";
                this.dbaccess.ReadSQL("SITMT18", getSITMT18);
                this.dbaccess.ReadSQL("CTR18", getCTR18);

                DataTable CTR18 = this.dbaccess.DataSet.Tables["CTR18"];

                if (CTR18.Rows.Count > 0)
                {
                    if (wor1.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(wor1);
                    }

                    foreach (DataRow dr18 in CTR18.Rows)
                    {
                        if (dr18.RowState != DataRowState.Deleted)
                        {
                            DataRow insertWor1 = wor1.NewRow();
                            insertWor1["sitenum"] = worh["sitenum"];
                            insertWor1["ctrnum"] = worh["docunum"];
                            insertWor1["location"] = dr18["location"];
                            insertWor1["svccode"] = dr18["svccode"];
                            insertWor1["svcdesc"] = dr18["svcdesc"];
                            insertWor1["origuid"] = dr18["guid"];
                            insertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                            insertWor1["guid"] = BizLogicTools.Tools.getGUID();
                            wor1.Rows.Add(insertWor1);

                        }
                    }
                }


                DataTable SITMTPD_TBtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select * from SITMT18");

                if (SITMTPD_TBtmp.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(wor1);

                    foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
                    {
                        DataRow InsertWor1 = wor1.NewRow();
                        InsertWor1["ctrnum"] = worh["docunum"];
                        if (!BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                        {
                            InsertWor1["eddate"] = Convert.ToString(dr1["dayNo"]) + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                        }
                        else if (BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                        {
                            InsertWor1["eddate"] = "01" + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                        }

                        InsertWor1["sitenum"] = worh["sitenum"];
                        InsertWor1["locnum"] = dr1["location"];
                        InsertWor1["svccode"] = dr1["svccode"];
                        InsertWor1["frequencycode"] = dr1["frequencycode"];
                        InsertWor1["origuid"] = dr1["guid"];
                        //InsertWor1["guid"] = BizLogicTools.Tools.getGUID(); ;
                        InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                        //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                        wor1.Rows.Add(InsertWor1);
                    }
                }


                #endregion

                /////////// Get Outstanding //////////// 

                #region Outstanding

                GetOutstandingByCtr();

                #endregion
            }
            else
            {
                /////////// Location ////////////

                #region Location

                string strGetByAh = "select refnum as docunum,sectorcode,sitenum,'HQ' as fromsitenum,sitenum as issuedto,addr1,addr2,addr3 from ctrh where refnum='" + worh["docunum"].ToString() + "' and sitenum='" + worh["sitenum"].ToString() + "' ";

                this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

                DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

                if (SITMT_TBtmp.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(wor4);
                    foreach (DataRow dr4 in SITMT_TBtmp.Rows)
                    {
                        DataRow InsertWor4 = wor4.NewRow();
                        InsertWor4["ctrnum"] = dr4["docunum"];
                        InsertWor4["sectorcode"] = dr4["sectorcode"];
                        InsertWor4["sitenum"] = dr4["sitenum"];
                        InsertWor4["fromsitenum"] = dr4["fromsitenum"];
                        InsertWor4["tositenum"] = dr4["issuedto"];
                        InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                        wor4.Rows.Add(InsertWor4);
                    }
                }

                #endregion

                /////////// New Task //////////// 

                #region New Task

                string getCTR6 = "Select * from CTR6 WHERE refnum='" + worh["docunum"].ToString() + "' ";
                string getSITMT11 = "Select * from SITMT11 WHERE 1=2";
                this.dbaccess.ReadSQL("SITMT11", getSITMT11);
                this.dbaccess.ReadSQL("CTR6", getCTR6);

                DataTable CTR6 = this.dbaccess.DataSet.Tables["CTR6"];

                if (CTR6.Rows.Count > 0)
                {
                    foreach (DataRow dr6 in CTR6.Rows)
                    {
                        if (dr6.RowState != DataRowState.Deleted)
                        {
                            dr6["qty"] = 0;
                            for (int i = 1; i <= 12; i++)
                            {
                                if (Convert.ToDateTime(worh["worDateFrom"]).Month == i)
                                {
                                    if (!BizFunctions.IsEmpty(dr6["xmonth" + i.ToString() + ""]) && !BizFunctions.IsEmpty(dr6["year"]))
                                    {
                                        dr6["qty"] = Convert.ToDecimal(dr6["qty"]) + Convert.ToDecimal(CountDaysCTR(i, Convert.ToInt32(dr6["year"]), Convert.ToInt32(Convert.ToBoolean(dr6["xmonth" + i.ToString() + ""])).ToString()));

                                        UpdateScheduleCTR(dr6["svccode"].ToString(), dr6["location"].ToString(), dr6["frequencycode"].ToString(), i.ToString(), dr6["year"].ToString(), Convert.ToInt32(Convert.ToBoolean(dr6["xmonth" + i.ToString() + ""])).ToString());
                                    }
                                }
                            }
                        }
                    }
                }


                DataTable SITMTPD_TBtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select * from SITMT11");

                if (SITMTPD_TBtmp.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(wor1);

                    foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
                    {
                        DataRow InsertWor1 = wor1.NewRow();
                        InsertWor1["ctrnum"] = worh["docunum"];
                        if (!BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                        {
                            InsertWor1["eddate"] = Convert.ToString(dr1["dayNo"]) + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                        }
                        else if (BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                        {
                            InsertWor1["eddate"] = "01" + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                        }

                        InsertWor1["sitenum"] = worh["sitenum"];
                        InsertWor1["locnum"] = dr1["location"];
                        InsertWor1["svccode"] = dr1["svccode"];
                        InsertWor1["frequencycode"] = dr1["frequencycode"];
                        InsertWor1["origuid"] = dr1["guid"];
                        //InsertWor1["guid"] = BizLogicTools.Tools.getGUID(); ;
                        InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                        //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                        wor1.Rows.Add(InsertWor1);
                    }
                }


                #endregion

                /////////// Get Outstanding //////////// 

                #region Outstanding

                GetOutstandingByCtr();

                #endregion
            }

            worh["searchType"] = "CTR";

        }

        private void GetByAdHoc()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = this.dbaccess.DataSet.Tables["wor4"];

            /////////// Location ////////////

            #region Location

            string strGetByAh = "select refnum as docunum,sectorcode,sitenum,'HQ' as fromsitenum,sitenum as issuedto,addr1,addr2,addr3 from adh where refnum='" + worh["adhnum"].ToString() + "' and sitenum='" + worh["sitenum"].ToString() + "' ";

            this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

            DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

            if (SITMT_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor4);
                foreach (DataRow dr4 in SITMT_TBtmp.Rows)
                {
                    DataRow InsertWor4 = wor4.NewRow();
                    InsertWor4["ctrnum"] = dr4["docunum"];
                    InsertWor4["sectorcode"] = dr4["sectorcode"];
                    InsertWor4["sitenum"] = dr4["sitenum"];
                    InsertWor4["fromsitenum"] = dr4["fromsitenum"];
                    InsertWor4["tositenum"] = dr4["issuedto"];
                    InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor4.Rows.Add(InsertWor4);
                }
            }

            #endregion

            /////////// New Task //////////// 

            #region New Task

            string getADH6 = "Select * from ADH6 WHERE refnum='" + worh["adhnum"].ToString() + "' ";
            string getSITMT11 = "Select * from SITMT11 WHERE 1=2";
            this.dbaccess.ReadSQL("SITMT11", getSITMT11);
            this.dbaccess.ReadSQL("ADH6", getADH6);

            DataTable ADH6 = this.dbaccess.DataSet.Tables["ADH6"];

            if (ADH6.Rows.Count > 0)
            {
                foreach (DataRow dr6 in ADH6.Rows)
                {
                    if (dr6.RowState != DataRowState.Deleted)
                    {
                        dr6["qty"] = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            if (Convert.ToDateTime(worh["worDateFrom"]).Month == i)
                            {
                                if (!BizFunctions.IsEmpty(dr6["xmonth" + i.ToString() + ""]) && !BizFunctions.IsEmpty(dr6["year"]))
                                {
                                    dr6["qty"] = Convert.ToDecimal(dr6["qty"]) + Convert.ToDecimal(CountDaysADH(i, Convert.ToInt32(dr6["year"]), Convert.ToInt32(Convert.ToBoolean(dr6["xmonth" + i.ToString() + ""])).ToString()));

                                    UpdateScheduleADH(dr6["svccode"].ToString(), dr6["location"].ToString(), dr6["frequencycode"].ToString(), i.ToString(), dr6["year"].ToString(), Convert.ToInt32(Convert.ToBoolean(dr6["xmonth" + i.ToString() + ""])).ToString());
                                }
                            }
                        }
                    }
                }
            }


            DataTable SITMTPD_TBtmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet,"Select * from SITMT11");

            if (SITMTPD_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor1);

                foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
                {
                    DataRow InsertWor1 = wor1.NewRow();
                    InsertWor1["ctrnum"] = worh["adhnum"];
                    if (!BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                    {
                        InsertWor1["eddate"] = Convert.ToString(dr1["dayNo"]) + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                    }
                    else if (BizFunctions.IsEmpty(dr1["dayNo"]) && !BizFunctions.IsEmpty(dr1["xmonth"]) && !BizFunctions.IsEmpty(dr1["year"]))
                    {
                        InsertWor1["eddate"] = "01" + "-" + Convert.ToString(dr1["xmonth"]) + "-" + Convert.ToString(dr1["year"]);
                    }
                     
                    InsertWor1["sitenum"] = worh["sitenum"];
                    InsertWor1["locnum"] = dr1["location"];
                    InsertWor1["svccode"] = dr1["svccode"];
                    InsertWor1["frequencycode"] = dr1["frequencycode"];
                    InsertWor1["origuid"] = dr1["guid"];
                    //InsertWor1["guid"] = BizLogicTools.Tools.getGUID(); ;
                    InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                    //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor1.Rows.Add(InsertWor1);
                }
            }

         
            #endregion

            /////////// Get Outstanding //////////// 

            #region Outstanding

            GetOutstandingByAdh();

            #endregion


            #region 01082013 - Jason

            ///////////// Location ////////////

            //#region Location

            //string strGetByAh = "Select docunum,sectorcode,sitenum,'HQ' as fromsitenum, sitenum as issuedto, addr1,addr2,addr3 from sitmt where docunum='" + worh["adhnum"].ToString() + "' and sitenum='" + worh["sitenum"].ToString() + "' ";

            //this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

            //DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

            //if (SITMT_TBtmp.Rows.Count > 0)
            //{
            //    BizFunctions.DeleteAllRows(wor4);
            //    foreach (DataRow dr4 in SITMT_TBtmp.Rows)
            //    {
            //        DataRow InsertWor4 = wor4.NewRow();
            //        InsertWor4["ctrnum"] = dr4["docunum"];
            //        InsertWor4["sectorcode"] = dr4["sectorcode"];
            //        InsertWor4["sitenum"] = dr4["sitenum"];
            //        InsertWor4["fromsitenum"] = dr4["fromsitenum"];
            //        InsertWor4["tositenum"] = dr4["issuedto"]; 
            //        InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();
  
            //        wor4.Rows.Add(InsertWor4);
            //    }
            //}

            //#endregion

            ///////////// New Task //////////// 

            //#region New Task

            //string strGetAhPd = "SELECT "+
            //                        "S11.sitenumt, "+
            //                        "S11.frequencycode, " +
            //                        "ST.sitenum, "+ 
            //                        "ST.arnum, "+
            //                        "ST.docunum, "+
            //                        "S11.svccode, "+
            //                        "S11.pdate, "+
            //                        "S11.[GUID] "+
            //                    "FROM "+
            //                    "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
            //                    "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
            //                    "where CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and docunum='" + worh["adhnum"].ToString() + "' "+
            //                    "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')";

            //this.dbaccess.ReadSQL("SITMTPD_TBtmp", strGetAhPd);

            //DataTable SITMTPD_TBtmp = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp"];

            //if (SITMTPD_TBtmp.Rows.Count > 0)
            //{
            //    BizFunctions.DeleteAllRows(wor1);

            //    foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
            //    {
            //        DataRow InsertWor1 = wor1.NewRow();
            //        InsertWor1["ctrnum"] = dr1["docunum"];
            //        InsertWor1["eddate"] = dr1["pdate"];
            //        InsertWor1["sitenum"] = dr1["sitenum"];
            //        InsertWor1["svccode"] = dr1["svccode"];
            //        InsertWor1["frequencycode"] = dr1["frequencycode"];
            //        InsertWor1["origuid"] = dr1["guid"];
            //        InsertWor1["guid"] = BizLogicTools.Tools.getGUID();;
            //        InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
            //        //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

            //        wor1.Rows.Add(InsertWor1);
            //    }
            //}

            //if (vourcherExist(worh["refnum"].ToString()))
            //{

            //    string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
            //                          "( " +
            //                            "SELECT [guid] FROM " +
            //                                "( " +
            //                                  "SELECT " +
            //                                    "S11.sitenumt, " +
            //                                    "S11.frequencycode, " +
            //                                    "ST.sitenum, " +
            //                                    "ST.arnum, " +
            //                                    "ST.docunum, " +
            //                                    "S11.svccode, " +
            //                                    "S11.pdate, " +
            //                                    "S11.[GUID] " +
            //                                    "FROM " +
            //                                    "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
            //                                    "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
            //                                    "where CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and docunum='" + worh["adhnum"].ToString() + "' " +
            //                                    "and S11.[guid] in (Select origuid from wor1 where [status]<>'V') " +
            //                                    "EXCEPT " +
            //                                    "SELECT " +
            //                                        "S11.sitenumt, " +
            //                                        "S11.frequencycode, " +
            //                                        "ST.sitenum, " +
            //                                        "ST.arnum, " +
            //                                        "ST.docunum, " +
            //                                        "S11.svccode, " +
            //                                        "S11.pdate, " +
            //                                        "S11.[GUID] " +
            //                                    "FROM " +
            //                                    "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
            //                                    "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
            //                                    "where CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and docunum='" + worh["adhnum"].ToString() + "' " +
            //                                    "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V') " +
            //                                ")A " +
            //                            ") " +
            //                            "AND [status]<>'V' and refnum<>'" + worh["refnum"].ToString() + "'";

            //    this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

            //    DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

            //    if (SITMTPD_TBtmp2.Rows.Count > 0)
            //    {
            //        string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

            //        for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
            //        {
            //            if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
            //            {
            //                arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();
            //            }
            //        }

            //        string refnum = "";
            //        for (int x = 0; x < arr1.Length; x++)
            //        {
            //            if (arr1[x] != null)
            //            {
            //                refnum = refnum + arr1[x].ToString() + "\n";
            //            }
            //        }
            //        if (refnum != string.Empty)
            //        {
            //            MessageBox.Show("Tasks already issued for this Adhoc from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateto"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");
            //        }

            //    }
            //}
            //else
            //{
            //    string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
            //                         "( " +
            //                           "SELECT [guid] FROM " +
            //                               "( " +
            //                                 "SELECT " +
            //                                   "S11.sitenumt, " +
            //                                   "S11.frequencycode, " +
            //                                   "ST.sitenum, " +
            //                                   "ST.arnum, " +
            //                                   "ST.docunum, " +
            //                                   "S11.svccode, " +
            //                                   "S11.pdate, " +
            //                                   "S11.[GUID] " +
            //                                   "FROM " +
            //                                   "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
            //                                   "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
            //                                   "where CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and docunum='" + worh["adhnum"].ToString() + "' " +
            //                                   "and S11.[guid] in (Select origuid from wor1 where [status]<>'V') " +
            //                                   "EXCEPT " +
            //                                   "SELECT " +
            //                                       "S11.sitenumt, " +
            //                                       "S11.frequencycode, " +
            //                                       "ST.sitenum, " +
            //                                       "ST.arnum, " +
            //                                       "ST.docunum, " +
            //                                       "S11.svccode, " +
            //                                       "S11.pdate, " +
            //                                       "S11.[GUID] " +
            //                                   "FROM " +
            //                                   "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
            //                                   "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
            //                                   "where CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and docunum='" + worh["adhnum"].ToString() + "' " +
            //                                   "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V') " +
            //                               ")A " +
            //                           ") " +
            //                           "AND [status]<>'V' and refnum<>'" + worh["refnum"].ToString() + "'";

            //    this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

            //    DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

            //    if (SITMTPD_TBtmp2.Rows.Count > 0)
            //    {
            //        string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

            //        for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
            //        {
            //            if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
            //            {
            //                arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();
            //            }
            //        }

            //        string refnum = "";
            //        for (int x = 0; x < arr1.Length; x++)
            //        {
            //            if (arr1[x] != null)
            //            {
            //                refnum = refnum + arr1[x].ToString() + "\n";
            //            }
            //        }
            //        if (refnum != string.Empty)
            //        {
            //            MessageBox.Show("Tasks already issued for this Adhoc from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateto"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");
            //        }

            //    }
            //}

            //#endregion

            ///////////// Get Outstanding //////////// 

            //#region Outstanding

            //GetOutstandingByAdh();

            //#endregion

            #endregion

            worh["searchType"] = "ADH";

        }

        private void GetByCustomer()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = this.dbaccess.DataSet.Tables["wor4"];

            /////////// Location ////////////

            #region Location

            string strGetByAh = "Select docunum,sectorcode,sitenum,'HQ' as fromsitenum, sitenum as issuedto, addr1,addr2,addr3 from sitmt where arnum='" + worh["arnum"].ToString() + "' ";

            this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

            DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

            if (SITMT_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor4);
                foreach (DataRow dr4 in SITMT_TBtmp.Rows)
                {
                    DataRow InsertWor4 = wor4.NewRow();
                    InsertWor4["ctrnum"] = dr4["docunum"];
                    InsertWor4["sectorcode"] = dr4["sectorcode"];
                    InsertWor4["sitenum"] = dr4["sitenum"];
                    InsertWor4["fromsitenum"] = dr4["fromsitenum"];
                    InsertWor4["tositenum"] = dr4["issuedto"]; 
                    InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor4.Rows.Add(InsertWor4);
                }
            }

            #endregion

            /////////// New Task //////////// 

            #region New Task

            string strGetAhPd = "SELECT " +
                                    "S11.sitenumt, " +
                                    "S11.frequencycode, " +
                                    "S11.location, " +
                                    "ST.sitenum, " +
                                    "ST.arnum, " +
                                    "ST.docunum, " +
                                    "S11.svccode, " +
                                    "S11.pdate, " +
                                    "S11.[GUID] " +
                                "FROM " +
                                "(select sitenumt,frequencycode,svccode,location,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND  CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and arnum='" + worh["arnum"].ToString() + "' " +
                                "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')";


            this.dbaccess.ReadSQL("SITMTPD_TBtmp", strGetAhPd);

            DataTable SITMTPD_TBtmp = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp"];

            if (SITMTPD_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor1);
                foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
                {
                    DataRow InsertWor1 = wor1.NewRow();
                    InsertWor1["ctrnum"] = dr1["docunum"];
                    InsertWor1["eddate"] = dr1["pdate"];
                    InsertWor1["sitenum"] = dr1["sitenum"];
                    InsertWor1["svccode"] = dr1["svccode"];
                    InsertWor1["locnum"] = dr1["location"];
                    InsertWor1["frequencycode"] = dr1["frequencycode"];
                    InsertWor1["origuid"] = dr1["guid"];
                    InsertWor1["guid"] = BizLogicTools.Tools.getGUID();;
                    InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                    //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor1.Rows.Add(InsertWor1);
                }
            }

            if(vourcherExist(worh["refnum"].ToString()))
            {
                string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
                                    "( " +
                                    "SELECT [guid] FROM " +
                                      "( "+
                                         "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and arnum='" + worh["arnum"].ToString() + "' " +
                                                "and S11.[guid] in (Select origuid from wor1 where [status]<>'V')" +

                                        "EXCEPT " +

                                        "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and arnum='" + worh["arnum"].ToString() + "' " +
                                                "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')" +
   
                                         ")A " +
                                     ") " +
                                    "AND [status]<>'V' and refnum<>'" + worh["refnum"].ToString() + "'";

                                    this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

                                    DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

                                    if (SITMTPD_TBtmp2.Rows.Count > 0)
                                    {
                                        string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

                                        for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
                                        {
                                            if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
                                            {                                               
                                                arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();                                                
                                            }
                                        }

                                        string refnum = "";
                                        for (int x = 0; x < arr1.Length; x++)
                                        {
                                            if (arr1[x] != null)
                                            {
                                                refnum = refnum + arr1[x].ToString() + "\n";
                                            }
                                        }
                                        if (refnum != string.Empty)
                                        {
                                            MessageBox.Show("Tasks already issued for this Customer: " + worh["arname"].ToString() + " from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateTo"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");                                          
                                        }

                                    }
            }
            else
            {
                string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
                                    "( " +
                                    "SELECT [guid] FROM " +
                                      "( "+
                                         "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and arnum='" + worh["arnum"].ToString() + "' " +
                                                "and S11.[guid] in (Select origuid from wor1 where [status]<>'V')" +

                                        "EXCEPT " +

                                        "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and arnum='" + worh["arnum"].ToString() + "' " +
                                                "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')" +
   
                                         ")A " +
                                     ") " +
                                    "AND [status]<>'V'";

                                    this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

                                    DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

                                    if (SITMTPD_TBtmp2.Rows.Count > 0)
                                    {
                                        string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

                                        for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
                                        {
                                            if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
                                            {                                               
                                                arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();                                                
                                            }
                                        }

                                        string refnum = "";
                                        for (int x = 0; x < arr1.Length; x++)
                                        {
                                            if (arr1[x] != null)
                                            {
                                                refnum = refnum + arr1[x].ToString() + "\n";
                                            }
                                        }
                                        if (refnum != string.Empty)
                                        {
                                            MessageBox.Show("Tasks already issued for this Customer: " + worh["arname"].ToString() + " from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateTo"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");                                          
                                        }

                                    }
            }

            #endregion

            /////////// Get Outstanding //////////// 

            #region Outstanding

            GetOutstandingByArm();
            
            #endregion

            worh["searchType"] = "ARM";

        }       

        private void GetByLocation()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = this.dbaccess.DataSet.Tables["wor4"];

            /////////// Location ////////////

            #region Location

            string strGetByAh = "Select docunum,sectorcode,sitenum,'HQ' as fromsitenum, sitenum as issuedto, addr1,addr2,addr3 from sitmt where sitenum='" + worh["sitenum"].ToString() + "' ";

            this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

            DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

            if (SITMT_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor4);
                foreach (DataRow dr4 in SITMT_TBtmp.Rows)
                {
                    DataRow InsertWor4 = wor4.NewRow();
                    InsertWor4["ctrnum"] = dr4["docunum"];
                    InsertWor4["sectorcode"] = dr4["sectorcode"];
                    InsertWor4["sitenum"] = dr4["sitenum"];
                    InsertWor4["fromsitenum"] = dr4["fromsitenum"];
                    InsertWor4["tositenum"] = dr4["issuedto"]; 
                    InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor4.Rows.Add(InsertWor4);
                }
            }

            #endregion

            /////////// New Task //////////// 

            #region New Task

            string strGetAhPd = "SELECT " +
                               "S11.sitenumt, " +
                               "S11.frequencycode, " +
                               "ST.sitenum, " +
                               "ST.arnum, " +
                               "ST.docunum, " +
                               "S11.svccode, " +
                               "S11.location, " +
                               "S11.pdate, " +
                               "S11.[GUID] " +
                           "FROM " +
                           "(select sitenumt,frequencycode,svccode,location, [guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                           "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                           "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sitenum='" + worh["sitenum"].ToString() + "' " +
                           "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')";


            this.dbaccess.ReadSQL("SITMTPD_TBtmp", strGetAhPd);

            DataTable SITMTPD_TBtmp = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp"];

            if (SITMTPD_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor1);
                foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
                {
                    DataRow InsertWor1 = wor1.NewRow();
                    InsertWor1["ctrnum"] = dr1["docunum"];
                    InsertWor1["eddate"] = dr1["pdate"];
                    InsertWor1["sitenum"] = dr1["sitenum"];
                    InsertWor1["locnum"] = dr1["location"];
                    InsertWor1["svccode"] = dr1["svccode"];
                    InsertWor1["frequencycode"] = dr1["frequencycode"];
                    InsertWor1["origuid"] = dr1["guid"];
                    InsertWor1["guid"] = BizLogicTools.Tools.getGUID();;
                    InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                    //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor1.Rows.Add(InsertWor1);
                }
            }

            if (vourcherExist(worh["refnum"].ToString()))
            {

                string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
                                      "( " +
                                        "SELECT [guid] FROM " +
                                            "( " +
                                            "SELECT " +
                                                   "S11.sitenumt, " +
                                                   "S11.frequencycode, " +
                                                   "ST.sitenum, " +
                                                   "ST.arnum, " +
                                                   "ST.docunum, " +
                                                   "S11.svccode, " +
                                                   "S11.pdate, " +
                                                   "S11.[GUID] " +
                                               "FROM " +
                                               "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                               "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                               "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sitenum='" + worh["sitenum"].ToString() + "' " +
                                               "and S11.[guid] in (Select origuid from wor1 where [status]<>'V')" +
                                                "EXCEPT " +
                                              "SELECT " +
                                                   "S11.sitenumt, " +
                                                   "S11.frequencycode, " +
                                                   "ST.sitenum, " +
                                                   "ST.arnum, " +
                                                   "ST.docunum, " +
                                                   "S11.svccode, " +
                                                   "S11.pdate, " +
                                                   "S11.[GUID] " +
                                               "FROM " +
                                               "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                               "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                               "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sitenum='" + worh["sitenum"].ToString() + "' " +
                                               "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')" +
                                               ")A " +
                                        ") " +
                                        "AND [status]<>'V' and refnum<>'" + worh["refnum"].ToString() + "'";

                this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

                DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

                if (SITMTPD_TBtmp2.Rows.Count > 0)
                {
                    string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

                    for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
                    {
                        if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
                        {
                            arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();
                        }
                    }

                    string refnum = "";
                    for (int x = 0; x < arr1.Length; x++)
                    {
                        if (arr1[x] != null)
                        {
                            refnum = refnum + arr1[x].ToString() + "\n";
                        }
                    }
                    if (refnum != string.Empty)
                    {
                        MessageBox.Show("Tasks already issued for this Adhoc from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateto"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");
                    }

                }
            }
            else
            {
                string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
                                     "( " +
                                       "SELECT [guid] FROM " +
                                            "( " +
                                            "SELECT " +
                                                   "S11.sitenumt, " +
                                                   "S11.frequencycode, " +
                                                   "ST.sitenum, " +
                                                   "ST.arnum, " +
                                                   "ST.docunum, " +
                                                   "S11.svccode, " +
                                                   "S11.pdate, " +
                                                   "S11.[GUID] " +
                                               "FROM " +
                                               "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                               "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                               "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sitenum='" + worh["sitenum"].ToString() + "' " +
                                               "and S11.[guid] in (Select origuid from wor1 where [status]<>'V')" +
                                                "EXCEPT " +
                                              "SELECT " +
                                                   "S11.sitenumt, " +
                                                   "S11.frequencycode, " +
                                                   "ST.sitenum, " +
                                                   "ST.arnum, " +
                                                   "ST.docunum, " +
                                                   "S11.svccode, " +
                                                   "S11.pdate, " +
                                                   "S11.[GUID] " +
                                               "FROM " +
                                               "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                               "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                               "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sitenum='" + worh["sitenum"].ToString() + "' " +
                                               "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')" +
                                           ")A " +
                                       ") " +
                                       "AND [status]<>'V' ";

                this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

                DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

                if (SITMTPD_TBtmp2.Rows.Count > 0)
                {
                    string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

                    for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
                    {
                        if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
                        {
                            arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();
                        }
                    }

                    string refnum = "";
                    for (int x = 0; x < arr1.Length; x++)
                    {
                        if (arr1[x] != null)
                        {
                            refnum = refnum + arr1[x].ToString() + "\n";
                        }
                    }
                    if (refnum != string.Empty)
                    {
                        MessageBox.Show("Tasks already issued for this Adhoc from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateto"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");
                    }

                }
            }

            #endregion

            /////////// Get Outstanding //////////// 

            #region Outstanding

            GetOutstandingBySitmi();

            #endregion

            worh["searchType"] = "SITM";
        }

        private void GetBySector()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            DataTable wor1 = this.dbaccess.DataSet.Tables["wor1"];
            DataTable wor4 = this.dbaccess.DataSet.Tables["wor4"];

            /////////// Location ////////////

            #region Location

            string strGetByAh = "Select docunum,sectorcode,sitenum,'HQ' as fromsitenum, sitenum as issuedto, addr1,addr2,addr3 from sitmt where sectorcode='" + worh["sectorcode"].ToString() + "' ";

            this.dbaccess.ReadSQL("SITMT_TBtmp", strGetByAh);

            DataTable SITMT_TBtmp = this.dbaccess.DataSet.Tables["SITMT_TBtmp"];

            if (SITMT_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor4);
                foreach (DataRow dr4 in SITMT_TBtmp.Rows)
                {
                    DataRow InsertWor4 = wor4.NewRow();
                    InsertWor4["ctrnum"] = dr4["docunum"];
                    InsertWor4["sectorcode"] = dr4["sectorcode"];
                    InsertWor4["sitenum"] = dr4["sitenum"];
                    InsertWor4["fromsitenum"] = dr4["fromsitenum"];
                    InsertWor4["tositenum"] = dr4["issuedto"];
                    InsertWor4["address"] = dr4["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor4.Rows.Add(InsertWor4);
                }
            }

            #endregion

            /////////// New Task //////////// 

            #region New Task

            string strGetAhPd = "SELECT " +
                                    "S11.sitenumt, " +
                                    "S11.frequencycode, " +
                                    "ST.sitenum, " +
                                    "ST.arnum, " +
                                    "ST.docunum, " +
                                    "S11.svccode, " +
                                    "S11.location, " +
                                    "S11.pdate, " +
                                    "S11.[GUID] " +
                                "FROM " +
                                "(select sitenumt,frequencycode,svccode,location,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sectorcode='" + worh["sectorcode"].ToString() + "' " +
                                "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')";


            this.dbaccess.ReadSQL("SITMTPD_TBtmp", strGetAhPd);

            DataTable SITMTPD_TBtmp = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp"];

            if (SITMTPD_TBtmp.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(wor1);
                foreach (DataRow dr1 in SITMTPD_TBtmp.Rows)
                {
                    DataRow InsertWor1 = wor1.NewRow();
                    InsertWor1["ctrnum"] = dr1["docunum"];
                    InsertWor1["eddate"] = dr1["pdate"];
                    InsertWor1["sitenum"] = dr1["sitenum"];
                    InsertWor1["svccode"] = dr1["svccode"];
                    InsertWor1["locnum"] = dr1["location"];
                    InsertWor1["frequencycode"] = dr1["frequencycode"];
                    InsertWor1["origuid"] = dr1["guid"];
                    InsertWor1["guid"] = BizLogicTools.Tools.getGUID(); ;
                    InsertWor1["uniqueid"] = BizLogicTools.Tools.getGUID();
                    //InsertWor4["] = dr1["addr1"].ToString() + " " + dr4["addr2"].ToString() + " " + dr4["addr3"].ToString();

                    wor1.Rows.Add(InsertWor1);
                }
            }

            if (vourcherExist(worh["refnum"].ToString()))
            {
                string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
                                    "( " +
                                    "SELECT [guid] FROM " +
                                      "( " +
                                         "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sectorcode='" + worh["sectorcode"].ToString() + "' " +
                                                "and S11.[guid] in (Select origuid from wor1 where [status]<>'V')" +

                                        "EXCEPT " +

                                        "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sectorcode='" + worh["sectorcode"].ToString() + "' " +
                                                "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')" +

                                         ")A " +
                                     ") " +
                                    "AND [status]<>'V' and refnum<>'" + worh["refnum"].ToString() + "'";

                this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

                DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

                if (SITMTPD_TBtmp2.Rows.Count > 0)
                {
                    string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

                    for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
                    {
                        if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
                        {
                            arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();
                        }
                    }

                    string refnum = "";
                    for (int x = 0; x < arr1.Length; x++)
                    {
                        if (arr1[x] != null)
                        {
                            refnum = refnum + arr1[x].ToString() + "\n";
                        }
                    }
                    if (refnum != string.Empty)
                    {
                        MessageBox.Show("Tasks already issued for this Customer: " + worh["arname"].ToString() + " from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateTo"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");
                    }

                }
            }
            else
            {
                string strCheckAhPD = "SELECT DISTINCT refnum FROM WOR1 WHERE origuid IN " +
                                    "( " +
                                    "SELECT [guid] FROM " +
                                      "( " +
                                         "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sectorcode='" + worh["sectorcode"].ToString() + "' " +
                                                "and S11.[guid] in (Select origuid from wor1 where [status]<>'V')" +

                                        "EXCEPT " +

                                        "SELECT " +
                                            "S11.sitenumt, " +
                                            "S11.frequencycode, " +
                                            "ST.sitenum, " +
                                            "ST.arnum, " +
                                            "ST.docunum, " +
                                            "S11.svccode, " +
                                            "S11.pdate, " +
                                            "S11.[GUID] " +
                                                "FROM " +
                                                "(select sitenumt,frequencycode,svccode,[guid],CONVERT(DATETIME,CONVERT(nvarchar,dayno)+'/'+ CONVERT(nvarchar,xmonth)+'/'+CONVERT(nvarchar,[year]),103) as pdate from SITMT11 WHERE [status]<>'V')S11 " +
                                                "LEFT JOIN SITMT ST ON S11.sitenumt = ST.sitenumt " +
                                                "where ST.docunum not like 'ADH%' AND ST.[status]<>'V' AND CONVERT(nvarchar,S11.pdate,112)>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and CONVERT(nvarchar,S11.pdate,112)<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateTo"])) + "' and sectorcode='" + worh["sectorcode"].ToString() + "' " +
                                                "and S11.[guid] not in (Select origuid from wor1 where [status]<>'V')" +

                                         ")A " +
                                     ") " +
                                    "AND [status]<>'V'";

                this.dbaccess.ReadSQL("SITMTPD_TBtmp2", strCheckAhPD);

                DataTable SITMTPD_TBtmp2 = this.dbaccess.DataSet.Tables["SITMTPD_TBtmp2"];

                if (SITMTPD_TBtmp2.Rows.Count > 0)
                {
                    string[] arr1 = new string[SITMTPD_TBtmp2.Rows.Count];

                    for (int i = 0; i < SITMTPD_TBtmp2.Rows.Count; i++)
                    {
                        if (SITMTPD_TBtmp2.Rows[i].RowState != DataRowState.Deleted)
                        {
                            arr1[i] = SITMTPD_TBtmp2.Rows[i]["refnum"].ToString();
                        }
                    }

                    string refnum = "";
                    for (int x = 0; x < arr1.Length; x++)
                    {
                        if (arr1[x] != null)
                        {
                            refnum = refnum + arr1[x].ToString() + "\n";
                        }
                    }
                    if (refnum != string.Empty)
                    {
                        MessageBox.Show("Tasks already issued for this Customer: " + worh["arname"].ToString() + " from " + Convert.ToDateTime(worh["worDateFrom"]).ToShortDateString() + " to " + Convert.ToDateTime(worh["worDateTo"]).ToShortDateString() + "? \n\n" + refnum + " \n", "Some Task may not be included");
                    }

                }
            }

            #endregion

            /////////// Get Outstanding //////////// 

            #region Outstanding

            GetOutstandingBySector();

            #endregion

            worh["searchType"] = "SEM";

        }       

        private string GetSitenumi(string adhoc)
        {
            string getST = "Select sitenum from ADH where refnum='"+adhoc+"'";
            string sitenum = "";
            this.dbaccess.ReadSQL("getSitenumTB", getST);

            DataTable getSitenumTB = this.dbaccess.DataSet.Tables["getSitenumTB"];

            if (getSitenumTB.Rows.Count > 0)
            {
                sitenum = getSitenumTB.Rows[0]["sitenum"].ToString();
            }
            return sitenum;
        }

        private string GetSitenumCTR(string docunum)
        {
            string getST = "Select sitenum from CTRH where refnum='" + docunum + "'";
            string sitenum = "";
            this.dbaccess.ReadSQL("getSitenumTB", getST);

            DataTable getSitenumTB = this.dbaccess.DataSet.Tables["getSitenumTB"];

            if (getSitenumTB.Rows.Count > 0)
            {
                sitenum = getSitenumTB.Rows[0]["sitenum"].ToString();
            }
            return sitenum;
        }

        private bool vourcherExist(string refnum)
        {
            string checkVoucher = "Select * from worh where refnum='" + refnum + "'";

            bool exist = false;

            this.dbaccess.ReadSQL("voucherExist", checkVoucher);

            DataTable voucherExist = this.dbaccess.DataSet.Tables["voucherExist"];

            if (voucherExist != null)
            {
                if (voucherExist.Rows.Count > 0)
                {
                    exist = true;
                }
            }

            return exist;
        }

        private string GetAhocNo(string sitenumt)
        {
            string getST = "Select docunum from SITMT where sitenumt='" + sitenumt + "'";
            string adhocno = "";
            this.dbaccess.ReadSQL("getSitenumTB", getST);

            DataTable getSitenumTB = this.dbaccess.DataSet.Tables["getSitenumTB"];

            if (getSitenumTB.Rows.Count > 0)
            {
                adhocno = getSitenumTB.Rows[0]["docunum"].ToString();
            }
            return adhocno;
        }

        private void GetOutstandingByOE()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            string GetOutstandingStr = "select " +
                                                "W1.zempnum as [Zone Sup], " +
                                                "W1.ctrnum as Docunum, " +
                                                "W1.sitenum as [Project Site], " +
                                                "W1.locnum as Location, " +
                                                "W1.svccode as [Service Code], " +
                                                "W1.svcdesc as [Service Desc], " +
                                                "W1.frequencycode as Frequency, " +
                                                "W1.cdate as [Done Date], " +
                                                "W1.ldone as [Last Done], " +
                                                "DATEDIFF(DD,W1.eddate,GETDATE()) as Dayslapse, " +
                                                "W1.remark, " +
                                                "W1.refnum " +
                                            "from wor1 W1 " +
                                            "LEFT JOIN WORH WH ON W1.refnum=WH.refnum " +
                                            "where W1.ldone is null and CONVERT(nvarchar,W1.eddate,112)<'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' " +
                                            "and W1.refnum<>'" + worh["refnum"].ToString() + "' and WH.empnum='" + worh["empnum"].ToString() + "'  and W1.[status]<>'V' and WH.SearchType='OE'";

            this.dbaccess.ReadSQL("dtGetOutstanding", GetOutstandingStr);
            DataTable dtGetOutstanding = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            if (dtGetOutstanding.Rows.Count > 0)
            {
                dg_outstanding.DataSource = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            }

            dg_outstanding.CaptionText = "Outstanding";
        }

        private void GetOutstandingByCtr()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            string GetOutstandingStr = "select " +
                                                "W1.zempnum as [Zone Sup], " +
                                                "W1.ctrnum as Docunum, " +
                                                "W1.sitenum as [Project Site], " +
                                                "W1.locnum as Location, " +
                                                "W1.svccode as [Service Code], " +
                                                "W1.svcdesc as [Service Desc], " +
                                                "W1.frequencycode as Frequency, " +
                                                "W1.cdate as [Done Date], " +
                                                "W1.ldone as [Last Done], " +
                                                "DATEDIFF(DD,W1.eddate,GETDATE()) as Dayslapse, " +
                                                "W1.remark, " +
                                                "W1.refnum " +
                                            "from wor1 W1 " +
                                            "LEFT JOIN WORH WH ON W1.refnum=WH.refnum " +
                                            "where W1.ldone is null and CONVERT(nvarchar,W1.eddate,112)<'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' " +
                                            "and W1.refnum<>'" + worh["refnum"].ToString() + "' and WH.empnum='" + worh["empnum"].ToString() + "'  and W1.[status]<>'V' and WH.SearchType='OE'";

            this.dbaccess.ReadSQL("dtGetOutstanding", GetOutstandingStr);
            DataTable dtGetOutstanding = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            if (dtGetOutstanding.Rows.Count > 0)
            {
                dg_outstanding.DataSource = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            }

            dg_outstanding.CaptionText = "Outstanding";    
        }

        private void GetOutstandingByAdh()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            string GetOutstandingStr = "select " +
                                                "W1.zempnum as [Zone Sup], " +
                                                "W1.ctrnum as Docunum, " +
                                                "W1.sitenum as [Project Site], " +
                                                "W1.locnum as Location, " +
                                                "W1.svccode as [Service Code], " +
                                                "W1.svcdesc as [Service Desc], " +
                                                "W1.frequencycode as Frequency, " +
                                                "W1.cdate as [Done Date], " +
                                                "W1.ldone as [Last Done], " +
                                                "DATEDIFF(DD,W1.eddate,GETDATE()) as Dayslapse, " +
                                                "W1.remark, " +
                                                "W1.refnum " +
                                            "from wor1 W1 " +
                                            "LEFT JOIN WORH WH ON W1.refnum=WH.refnum " +
                                            "where W1.ldone is null and CONVERT(nvarchar,W1.eddate,112)<'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' " +
                                            "and W1.refnum<>'" + worh["refnum"].ToString() + "' and W1.sitenum='" + worh["sitenum"].ToString() + "' and W1.[status]<>'V' and WH.SearchType='ADH'";

            this.dbaccess.ReadSQL("dtGetOutstanding", GetOutstandingStr);
            DataTable dtGetOutstanding = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            if (dtGetOutstanding.Rows.Count > 0)
            {
                dg_outstanding.DataSource = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            }
            
            dg_outstanding.CaptionText = "Outstanding";
            #region 01082013 - Jason
            //DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            //string GetOutstandingStr = "select " +
            //                                    "zempnum as [Zone Sup], " +
            //                                    "ctrnum as Docunum, " +
            //                                    "sitenum as [Project Site], " +
            //                                    "locnum as Location, " +
            //                                    "svccode as [Service Code], " +
            //                                    "svcdesc as [Service Desc], " +
            //                                    "frequencycode as Frequency, " +
            //                                    "cdate as [Done Date], " +
            //                                    "ldone as [Last Done], " +
            //                                    "DATEDIFF(DD,eddate,GETDATE()) as Dayslapse, " +
            //                                    "remark, " +
            //                                    "refnum " +
            //                                "from wor1 " +
            //                                "where ldone is null and CONVERT(nvarchar,eddate,112)<'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' " +
            //                                "and refnum<>'" + worh["refnum"].ToString() + "' and sitenum='" + worh["sitenum"].ToString() + "' and [status]<>'V'";

            //this.dbaccess.ReadSQL("dtGetOutstanding", GetOutstandingStr);
            //DataTable dtGetOutstanding = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            //if (dtGetOutstanding.Rows.Count > 0)
            //{
            //    dg_outstanding.DataSource = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            //}

            //dg_outstanding.CaptionText = "Outstanding";

            #endregion
        }

        private void GetOutstandingByArm()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            string GetOutstandingStr = "SELECT " +
                                                "A.[Zone Sup], " +
                                                "A.Docunum, " +
                                                "A.[Project Site], " +
                                                "A.Location, " +
                                                "A.[Service Code], " +
                                                "A.[Service Desc], " +
                                                "A.Frequency, " +
                                                "A.[Done Date], " +
                                                "A.[Last Done], " +
                                                "A.Dayslapse, " +
                                                "A.remark, " +
                                                "A.refnum " +
                                            "FROM " +
                                            "( " +
                                            "select " +
                                                "zempnum as [Zone Sup], " +
                                                "ctrnum as Docunum, " +
                                                "sitenum as [Project Site], " +
                                                "locnum as Location, " +
                                                "svccode as [Service Code], " +
                                                "svcdesc as [Service Desc], " +
                                                "frequencycode as Frequency, " +
                                                "ldone as [Last Done], " +
                                                "cdate as [Done Date], " +
                                                "DATEDIFF(DD,eddate,GETDATE()) as Dayslapse, " +
                                                "remark, " +
                                                "refnum " +
                                            "from wor1 " +
                                            "where ldone is null and CONVERT(nvarchar,eddate,112)<'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and [status]<>'V' " +
                                            "and refnum<>'" + worh["refnum"].ToString() + "' " +
                                            ")A " +
                                            "LEFT JOIN WORH WH on A.refnum=WH.refnum " +
                                            "AND WH.arnum='" + worh["arnum"].ToString() + "' ";

            this.dbaccess.ReadSQL("dtGetOutstanding", GetOutstandingStr);
            DataTable dtGetOutstanding = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            if (dtGetOutstanding.Rows.Count > 0)
            {
                dg_outstanding.DataSource = this.dbaccess.DataSet.Tables["dtGetOutstanding"];
            }

            dg_outstanding.CaptionText = "Outstanding";
        }

        private void GetOutstandingBySitmi()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            string GetOutstandingStr = "select " +
                                               "zempnum as [Zone Sup], " +
                                               "ctrnum as Docunum, " +
                                               "sitenum as [Project Site], " +
                                               "locnum as Location, " +
                                               "svccode as [Service Code], " +
                                               "svcdesc as [Service Desc], " +
                                               "frequencycode as Frequency, " +
                                               "cdate as [Done Date], " +
                                               "ldone as [Last Done], " +
                                               "DATEDIFF(DD,eddate,GETDATE()) as Dayslapse, " +
                                               "remark, " +
                                               "refnum " +
                                           "from wor1 " +
                                           "where ldone is null and CONVERT(nvarchar,eddate,112)<'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' " +
                                           "and refnum<>'" + worh["refnum"].ToString() + "' and sitenum='" + worh["sitenum"].ToString() + "' and [status]<>'V' ";

            this.dbaccess.ReadSQL("dtGetOutstanding", GetOutstandingStr);
            DataTable dtGetOutstanding = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            if (dtGetOutstanding.Rows.Count > 0)
            {
                dg_outstanding.DataSource = this.dbaccess.DataSet.Tables["dtGetOutstanding"];


            }

            dg_outstanding.CaptionText = "Outstanding";
        }

        private void GetOutstandingBySector()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            string GetOutstandingStr = "SELECT " +
                                                "A.[Zone Sup], " +
                                                "A.Docunum, " +
                                                "A.[Project Site], " +
                                                "A.Location, " +
                                                "A.[Service Code], " +
                                                "A.[Service Desc], " +
                                                "A.Frequency, " +
                                                "A.[Done Date], " +
                                                "A.[Last Done], " +
                                                "A.Dayslapse, " +
                                                "A.remark, " +
                                                "A.refnum " +
                                            "FROM " +
                                            "( " +
                                            "select " +
                                                "zempnum as [Zone Sup], " +
                                                "ctrnum as Docunum, " +
                                                "sitenum as [Project Site], " +
                                                "locnum as Location, " +
                                                "svccode as [Service Code], " +
                                                "svcdesc as [Service Desc], " +
                                                "frequencycode as Frequency, " +
                                                "ldone as [Last Done], " +
                                                "cdate as [Done Date], " +
                                                "DATEDIFF(DD,eddate,GETDATE()) as Dayslapse, " +
                                                "remark, " +
                                                "refnum " +
                                            "from wor1 " +
                                            "where ldone is null and CONVERT(nvarchar,eddate,112)<'" + BizFunctions.GetSafeDateString(Convert.ToDateTime(worh["worDateFrom"])) + "' and [status]<>'V' " +
                                            "and refnum<>'" + worh["refnum"].ToString() + "' " +
                                            ")A " +
                                            "LEFT JOIN WORH WH on A.refnum=WH.refnum " +
                                            "and WH.sectorcode='" + worh["sectorcode"].ToString() + "' ";

            this.dbaccess.ReadSQL("dtGetOutstanding", GetOutstandingStr);
            DataTable dtGetOutstanding = this.dbaccess.DataSet.Tables["dtGetOutstanding"];

            if (dtGetOutstanding.Rows.Count > 0)
            {
                dg_outstanding.DataSource = this.dbaccess.DataSet.Tables["dtGetOutstanding"];
            }

            dg_outstanding.CaptionText = "Outstanding";
        }

        private string getZoneExec(string sitenum)
        {
            string empnum = "";

            string strGetZoneExec ="select opmgr  from sem where sectorcode in "+
                                    "( "+
                                    "select sectorcode from sitm where sitenum='"+sitenum+"' " +
                                    ")";
            this.dbaccess.ReadSQL("GetZoneExecTB", strGetZoneExec);

            DataTable GetZoneExecTB = this.dbaccess.DataSet.Tables["GetZoneExecTB"];

            if (GetZoneExecTB != null)
            {
                if (GetZoneExecTB.Rows.Count > 0)
                {
                    DataRow dr1 = GetZoneExecTB.Rows[0];

                    empnum = dr1["opmgr"].ToString();
                }
            }
            return empnum;
        }
       
        private int CountDaysADH(int month, int year, string days)
        {
            int totalDays = 0;
            //string[] sTemp = days.Split(',');
            DateTime dt = new DateTime(year, month, 1);

            if (days != string.Empty)
            {
                if (days != "0")
                {
                    if (!days.Contains("99"))
                    {
                        totalDays = Convert.ToInt32(days);
                        //}                   
                    }
                    else
                    {
                        totalDays = totalDays + TimeTools.GetTotalDayMonth(dt);
                    }
                }
            }
            return totalDays;
        }

        private int CountDaysCTR(int month, int year, string days)
        {
            int totalDays = 0;
            //string[] sTemp = days.Split(',');
            DateTime dt = new DateTime(year, month, 1);

            if (days != string.Empty)
            {
                if (days != "0")
                {
                    if (!days.Contains("99"))
                    {
                        //for (int i = 0; i < sTemp.Length; i++)
                        //{
                        totalDays = Convert.ToInt32(days);
                        //}                   
                    }
                    else
                    {
                        totalDays = totalDays + TimeTools.GetTotalDayMonth(dt);
                    }
                }
            }
            return totalDays;
        }

        private void UpdateScheduleCTR(string svcCode, string location, string frequencyCode, string month, string year, string days)
        {
            int totalDays = 0;
            string[] sTemp = days.Split(',');
            DateTime dt = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);

            if (days != string.Empty)
            {
                if (days != "0")
                {
                    if (!days.Contains("99"))
                    {       
                        for (int i = 0; i < Convert.ToInt32(days); i++)
                        {
                            InsertSchedule(svcCode, location, frequencyCode, month, Convert.ToString(1), year);
                        }
                    }
                    else
                    {
                        totalDays = TimeTools.GetTotalDayMonth(dt);

                        for (int i = 0; i < totalDays; i++)
                        {
                            InsertSchedule(svcCode, location, frequencyCode, month, Convert.ToString(1), year);
                        }

                    }
                }
            }
        }

        private void UpdateScheduleADH(string svcCode,string location, string frequencyCode, string month, string year, string days)
        {
            int totalDays = 0;
            string[] sTemp = days.Split(',');
            DateTime dt = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);

            if (days != string.Empty)
            {
                if (days != "0")
                {
                    if (!days.Contains("99"))
                    {    
                        for (int i = 0; i < Convert.ToInt32(days); i++)
                        {
                            InsertSchedule(svcCode,location, frequencyCode, month, Convert.ToString(1), year);
                        }
                    }
                    else
                    {
                        totalDays = TimeTools.GetTotalDayMonth(dt);

                        for (int i = 0; i < totalDays; i++)
                        {
                            InsertSchedule(svcCode,location, frequencyCode, month, Convert.ToString(1), year);
                        }

                    }
                }
            }
        }

        private void InsertSchedule(string svcCode,string location, string frequencyCode, string xMonth, string dayNo, string year)
        {
            DataRow WORH = this.dbaccess.DataSet.Tables["WORH"].Rows[0];
            DataTable SITMT11 = this.dbaccess.DataSet.Tables["SITMT11"];

            DataRow InsertSITMT11 = SITMT11.NewRow();

            //InsertSITMT11["sitenum"] = WORH["sitenum"];
            InsertSITMT11["frequencycode"] = frequencyCode;
            InsertSITMT11["svccode"] = svcCode;
            InsertSITMT11["location"] = location;
            InsertSITMT11["xmonth"] = Convert.ToInt32(xMonth);
            InsertSITMT11["dayno"] = Convert.ToInt32(dayNo);
            InsertSITMT11["year"] = Convert.ToInt32(year);
  

            SITMT11.Rows.Add(InsertSITMT11);
        }

        private void GetSignature()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            DataTable SigTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select '' as empnum,sigloc as signaturepicloc,managersigloc as girosigLoc from worh where refnum='" + worh["refnum"].ToString() + "' ");

            SigTB.TableName = "SigTB1";

            SigTB.Columns.Add("Photo1", typeof(Byte[]));
            SigTB.Columns.Add("Photo2", typeof(Byte[]));

            if (SigTB.Rows.Count > 0)
            {
                foreach (DataRow dr1 in SigTB.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //if (!BizFunctions.IsEmpty(dr1["signaturepicloc"]))
                        //{
                        //    if (dr1["signaturepicloc"].ToString().Trim() != "NULL")
                        //    {
                        //        dr1["photo1"] = System.IO.File.ReadAllBytes(dr1["signaturepicloc"].ToString().Trim());
                        //    }
                        //}

                        //if (!BizFunctions.IsEmpty(dr1["girosigLoc"]))
                        //{
                        //    if (dr1["girosigLoc"].ToString().Trim() != "NULL")
                        //    {
                        //        dr1["photo2"] = System.IO.File.ReadAllBytes(dr1["girosigLoc"].ToString().Trim());
                        //    }
                        //}

                        if (!BizFunctions.IsEmpty(dr1["signaturepicloc"]))
                        {
                            if (dr1["signaturepicloc"].ToString().Trim().ToUpper() != "NULL")
                            {
                                dr1["photo1"] = System.IO.File.ReadAllBytes(dr1["signaturepicloc"].ToString().Trim());
                            }
                            else
                            {

                                dr1["photo1"] = System.IO.File.ReadAllBytes(ImagePath + "BlankImage.JPG");
                            }
                        }
                        else
                        {

                            dr1["photo1"] = System.IO.File.ReadAllBytes(ImagePath + "BlankImage.JPG");
                        }

                        if (!BizFunctions.IsEmpty(dr1["girosigLoc"]))
                        {
                            if (dr1["girosigLoc"].ToString().Trim().ToUpper() != "NULL")
                            {
                                dr1["photo2"] = System.IO.File.ReadAllBytes(dr1["girosigLoc"].ToString().Trim());
                            }
                            else
                            {

                                dr1["photo2"] = System.IO.File.ReadAllBytes(ImagePath + "BlankImage.JPG");
                            }
                        }
                        else
                        {

                            dr1["photo2"] = System.IO.File.ReadAllBytes(ImagePath + "BlankImage.JPG");
                        }
                    }
                }
            }

            if (this.dbaccess.DataSet.Tables.Contains("SigTB1"))
            {
                this.dbaccess.DataSet.Tables["SigTB1"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("SigTB1");
                this.dbaccess.DataSet.Tables.Add(SigTB);
            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(SigTB);
            }
        }

        private void GetWorkPictures()
        {
            DataRow worh = this.dbaccess.DataSet.Tables["worh"].Rows[0];
            string test = "select photourl,remark,created from wor5 where refnum='" + worh["refnum"].ToString() + "' ";
            DataTable WorkTB = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select photourl,remark,created from wor5 where refnum='" + worh["refnum"].ToString() + "' ");


            WorkTB.TableName = "WorkTB1";
            WorkTB.Columns.Add("Photo1", typeof(Byte[]));
            //WorkTB.Columns.Add("Photo2", typeof(Byte[]));

            if (WorkTB.Rows.Count > 0)
            {
                foreach (DataRow dr1 in WorkTB.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["photourl"]))
                        {
                            if (dr1["photourl"].ToString().Trim() != "NULL")
                            {
                                dr1["photo1"] = System.IO.File.ReadAllBytes(dr1["photourl"].ToString().Trim());
                            }
                        }
                    }
                }
            }
            if (this.dbaccess.DataSet.Tables.Contains("WorkTB1"))
            {
                this.dbaccess.DataSet.Tables["WorkTB1"].Dispose();
                this.dbaccess.DataSet.Tables.Remove("WorkTB1");
                this.dbaccess.DataSet.Tables.Add(WorkTB);

            }
            else
            {
                this.dbaccess.DataSet.Tables.Add(WorkTB);
            }
        }


        protected void btnEmail_Click(object sender, System.EventArgs e)
        {
          
            //DataRow svr = this.dbaccess.DataSet.Tables["worh"].Rows[0];

            //Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            //Outlook._MailItem oMailItem = (Outlook._MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
            //oMailItem.Subject = "Cleanning Service Report From ATL";
            //oMailItem.Body = "Dear \n\nPlease see Attached Sevrice Report for your attention.\n\nThank You & Regards,\n\n";
            //int iPosition = (int)oMailItem.Body.Length + 1;
            //int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
            //String sDisplayName = "MyAttachment";

            //#region If Marked
            //{
            //    if (File.Exists(filepath + "\\" + svr["refnum"].ToString() + ".pdf"))
            //    {
            //        //    MessageBox.Show("Invoice is not exist!");                  
            //        Outlook.Attachment oAttach = oMailItem.Attachments.Add(filepath + "\\" + svr["refnum"].ToString() + ".pdf", iAttachType, iPosition, sDisplayName);
            //    }
            //    if (!BizFunctions.IsEmpty( svr["assignmentemail"] ))
            //    {
            //        oMailItem.To = svr["assignmentemail"].ToString();
            //    }
            //}
            //#endregion
            //oMailItem.Display(true);
        }
    }
}
    

