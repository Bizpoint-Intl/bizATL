/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_ACM.cs
 *	Description:    Purchase Order Module
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		20/03/07			Start 
***********************************************************/

//using System;
//using System.Drawing;
//using System.Collections;
//using System.Data;
//using System.Windows.Forms;
//using System.Configuration;

//using BizRAD.BizXml;
//using BizRAD.BizCommon;
//using BizRAD.BizBase;
//using BizRAD.BizVoucher;
//using BizRAD.BizDocument;
//using BizRAD.BizDetail;
//using BizRAD.BizApplication;
//using BizRAD.DB.Client;
//using BizRAD.DB.Interface;
//using BizRAD.BizTools;
//using BizRAD.BizControls.OutLookBar;
//using BizRAD.BizControls.DataGridColumns;
//using BizRAD.BizAccounts;
//using Microsoft.Office;
//using System.Data.OleDb;
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

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
using System.Data.OleDb;

using ATL.ExtractMATM;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
//using Outlook = Microsoft.Office.Interop.Outlook;

namespace ATL.POR
{
    public class Voucher_POR : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region global variables
        protected bool opened = false;
        protected DBAccess dbaccess = null;
        protected string headerFormName = null;
        protected Hashtable formsCollection = null;
        protected string formName = null;

        protected decimal por1_grosamt = 0;
        protected decimal por1_discamt = 0;
        protected decimal por1_oriamt = 0;

        protected RadioButton rad_GRNN = null;
        protected RadioButton rad_GRNY = null;
        protected Button btn_GRN = null;
        string filepath = System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository") + "\\POR";
        string projectPath = null;
        protected RadioButton rad_PIVN = null;
        protected RadioButton rad_PIVY = null;
        protected Button btn_PIV = null;
        protected Button btn_ExtractMATM = null;
        protected Button btn_Import = null;
        protected Button btn_ExtractTRQ = null;
        #endregion

        public Voucher_POR(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_POR.xml", moduleName, voucherBaseHelpers)
        {
        }

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "porh.flag='POR' AND porh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (porh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " porh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " porh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND porh.flag='POR' AND porh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

        #region DocumentPage Event

        protected override void AddDocumentPageEventTarget(object sender, PageEventArgs e)
        {
            base.AddDocumentPageEventTarget(sender, e);

        }
        protected override void Voucher_Edit_Handle(object sender, VoucherHandleEventArgs e)
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

        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);
            opened = false;
        }

        protected override void Voucher_New_Handle(object sender, VoucherHandleEventArgs e)
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

        #region Document Handle

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);

            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = e.DBAccess.DataSet.Tables["por1"];
            try
            {
                Form form = BizXmlReader.CurrentInstance.Load(@"FormPreviewWithCancel.xml", "formPreview", this, null) as Form;
                CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;
                ReportDocument crReportDocument = new ReportDocument();
                crReportDocument.Load(this.projectPath + @"\POR\Report\porRpt.rpt");

                Hashtable selectedCollection = new Hashtable();
                selectedCollection.Add("coy", "SELECT * FROM coy");
                selectedCollection.Add("matm", "SELECT * FROM matm");
                selectedCollection.Add("apm", "SELECT top 1 * FROM apm where apnum='" + porh["apnum"].ToString().Trim() + "'");
                //selectedCollection.Add("ard", "SELECT * FROM ard" + Common.DEFAULT_SYSTEM_YEAR + " where refnum = '" + recp["refnum"].ToString().Trim() + "'");

                e.DBAccess.ReadSQL(selectedCollection);

                crReportDocument.SetDataSource(e.DBAccess.DataSet);
                crystalReportViewer1.ReportSource = crReportDocument;
                crystalReportViewer1.Refresh();

                ExportOptions CrExportOptions;
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                string ServerProjDir = filepath;
                CrDiskFileDestinationOptions.DiskFileName = filepath + @"\" + porh["refnum"].ToString() + ".pdf";
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

        #region Document_Save_Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = e.DBAccess.DataSet.Tables["por1"];
            if (BizFunctions.IsEmpty(porh["type"]))
            {
                porh["type"] = porh["type2"];
            }
            if (porh["type"] == System.DBNull.Value)
            {
                MessageBox.Show("Please select order type");
                e.Handle = false;
            }
            # region Check for empty row and empty mat code

            foreach (DataRow dr in por1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr["matnum"]))
                    {
                        MessageBox.Show("Save Unsuccessful\nProduct Code cannot be empty !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Handle = false;
                        return;
                    }

                    if (!BizFunctions.IsEmpty(dr["matnum"]))
                    {
                        if (BizFunctions.IsEmpty(dr["baseunitprice"]))
                        {
                            dr["baseunitprice"] = BizLogicTools.Tools.GetPeriodCost(dr["matnum"].ToString(), BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(porh["trandate"])), this.dbaccess);
                        }
                        else if (Convert.ToDecimal(dr["baseunitprice"]) == 0)
                        {
                            dr["baseunitprice"] = BizLogicTools.Tools.GetPeriodCost(dr["matnum"].ToString(), BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(porh["trandate"])), this.dbaccess);
                        }
                    }


                    if (BizFunctions.IsEmpty(dr["trqnum"]) && !BizFunctions.IsEmpty(dr["ponum"]))
                    {
                        dr["trqnum"] = dr["ponum"];
                    }
                    else if (BizFunctions.IsEmpty(dr["ponum"]) && !BizFunctions.IsEmpty(dr["trqnum"]))
                    {
                        dr["ponum"] = dr["trqnum"];
                    }
                    else if (BizFunctions.IsEmpty(dr["trqnum"]) && !BizFunctions.IsEmpty(porh["ponum"]))
                    {
                        dr["trqnum"] = porh["ponum"];
                    }
                    else if (BizFunctions.IsEmpty(dr["ponum"]) && !BizFunctions.IsEmpty(porh["ponum"]))
                    {
                        dr["ponum"] = porh["trqnum"];
                    }
                  

                    if (BizFunctions.IsEmpty(dr["whnum"]))
                    {
                        dr["whnum"] = "MWH";
                    }
                }
            }

         
            #endregion
        }

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = e.DBAccess.DataSet.Tables["por1"];
            foreach (DataRow dr in por1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(porh, dr, "type/status");
                }
            }
        }

        #region Save End Event
        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);


            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = e.DBAccess.DataSet.Tables["por1"];

            DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];
            string matnums = String.Empty;

            if (porh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                try
                {
                bool porExists = false;
                int updatePart = 0;
                    // Jason
                    //string update1 = "update trq1 set ponum='" + porh["refnum"].ToString() + "' where refnum='"+porh["ponum"].ToString()+"' and matnum in " +
                    //                    "( "+
                    //                    "select matnum from POR1 where refnum='"+porh["refnum"].ToString()+"'  "+
                    //                    ")";
                    //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(update1);  

                    string Query1 = "Select * from TRQ1 where refnum='"+porh["ponum"].ToString()+"' and matnum in "+
                                    "( "+
                                    "select matnum from POR1 where refnum='"+porh["refnum"].ToString()+"' "+
                                    ")";

                    this.dbaccess.ReadSQL("TempTRQ1TB", Query1);

                    DataTable TempTRQ1TB = this.dbaccess.DataSet.Tables["TempTRQ1TB"];

                   if(TempTRQ1TB.Rows.Count > 0)
                   {
                       foreach(DataRow dr1TQ in TempTRQ1TB.Rows)
                       {
                            foreach (DataRow dr1 in por1.Rows)
                            {
                                if (dr1.RowState != DataRowState.Deleted)
                                {
                                    if(dr1TQ["matnum"].ToString().Trim() == dr1["matnum"].ToString().Trim())
                                    {
                                        if(dr1TQ["ponum"].ToString().Trim() == porh["refnum"].ToString().Trim())
                                        {
                                            porExists = true;
                                        }
                                        if(dr1TQ["ponum2"].ToString().Trim() == porh["refnum"].ToString().Trim())
                                        {
                                            porExists = true;
                                        }
                                        if(dr1TQ["ponum3"].ToString().Trim() == porh["refnum"].ToString().Trim())
                                        {
                                            porExists = true;
                                        }
                                        if (dr1TQ["ponum4"].ToString().Trim() == porh["refnum"].ToString().Trim())
                                        {
                                            porExists = true;
                                        }

                                        if(!porExists)
                                        {
                                            if(BizFunctions.IsEmpty(dr1TQ["ponum"]))
                                            {
                                                dr1TQ["ponum"] = porh["refnum"].ToString().Trim();
                                                updatePart = 1;
                                            }
                                            else if(BizFunctions.IsEmpty(dr1TQ["ponum2"]))
                                            {
                                                dr1TQ["ponum2"] = porh["refnum"].ToString().Trim();
                                                updatePart = 2;
                                            }
                                            else if(BizFunctions.IsEmpty(dr1TQ["ponum3"]))
                                            {
                                                dr1TQ["ponum3"] = porh["refnum"].ToString().Trim();
                                                updatePart = 3;
                                            }
                                            else if (BizFunctions.IsEmpty(dr1TQ["ponum4"]))
                                            {
                                                dr1TQ["ponum4"] = porh["refnum"].ToString().Trim();
                                                updatePart = 4;
                                            }
                                            else if (!BizFunctions.IsEmpty(dr1TQ["ponum"]) && (dr1TQ["ponum"].ToString().Trim() == porh["refnum"].ToString().Trim()))
                                            {
                                            }
                                            else if (!BizFunctions.IsEmpty(dr1TQ["ponum2"]) && (dr1TQ["ponum2"].ToString().Trim() == porh["refnum"].ToString().Trim()))
                                            {
                                            }
                                            else if (!BizFunctions.IsEmpty(dr1TQ["ponum3"]) && (dr1TQ["ponum3"].ToString().Trim() == porh["refnum"].ToString().Trim()))
                                            {
                                            }

                                            porExists = false;

                                            string updateQuery = "";

                                            if (updatePart == 1)
                                            {
                                                updateQuery = "update trq1 set ponum='" + porh["refnum"].ToString() + "' where refnum='" + porh["ponum"].ToString() + "' and matnum='" + dr1["matnum"].ToString().Trim() + "' ";
                                            }
                                            else if (updatePart == 2)
                                            {
                                                updateQuery = "update trq1 set ponum2='" + porh["refnum"].ToString() + "' where refnum='" + porh["ponum"].ToString() + "' and matnum='" + dr1["matnum"].ToString().Trim() + "' ";

                                            }
                                            else if (updatePart == 3)
                                            {
                                                updateQuery = "update trq1 set ponum3='" + porh["refnum"].ToString() + "' where refnum='" + porh["ponum"].ToString() + "' and matnum='" + dr1["matnum"].ToString().Trim() + "' ";

                                            }
                                            else if (updatePart == 4)
                                            {
                                                updateQuery = "update trq1 set ponum4='" + porh["refnum"].ToString() + "' where refnum='" + porh["ponum"].ToString() + "' and matnum='" + dr1["matnum"].ToString().Trim() + "' ";

                                            }


                                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateQuery);  
                                        }
                                       
                                    }

                                }
                            }
                       }
                   }


                 

				
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Confirm unsuccessful! \n\n" + ex.Message + "\n\nPlease try again.", "Transview Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }
        # endregion

        #endregion

        #region Document_Print_Handle
        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];

            if (porh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "porh/por1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];
            Hashtable selectedCollection = new Hashtable();


            selectedCollection.Add("coy", "SELECT * FROM coy");
            selectedCollection.Add("matm", "SELECT * FROM matm");
            selectedCollection.Add("apm", "SELECT top 1 * FROM apm where apnum='" + porh["apnum"].ToString().Trim() + "'");
            //selectedCollection.Add("ard", "SELECT * FROM ard" + Common.DEFAULT_SYSTEM_YEAR + " where refnum = '" + recp["refnum"].ToString().Trim() + "'");
            e.DBAccess.ReadSQL(selectedCollection);
            e.DataSource = e.DBAccess.DataSet;


            //BizLogicTools.Tools.GeneratePdf("porRpt.rpt", porh["flag"].ToString().ToUpper().Trim(), this.dbaccess.DataSet, sender, porh["refnum"].ToString());
        }
        #endregion

        #region Document_Extract_Handle
        protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Extract_Handle(sender, e);

            AutoCalc();
        }
        #endregion

        #region Paste Handle
        protected override void Document_Paste_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Paste_Handle(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];

        }

        protected override void Document_Paste_OnClick(object sender, BizRAD.BizDocument.DocumentEventArgs e)
        {
            base.Document_Paste_OnClick(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];




      
        }
        #endregion

        #region F2/F3
        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];

            switch (e.MappingName)
            {
                case "matnum":
                    if (porh["type2"].ToString() == "Service")
                    {
                        e.DefaultCondition = "status<>'V' and ApplicationType='Service'";
                    }
                    else
                    {
                        e.Condition = BizFunctions.F2Condition("matnum", (sender as TextBox).Text);
                        e.DefaultCondition = " [status]<>'V'";
                    }
                    break;

                case "apmatnum":
                    if (porh["type2"].ToString() == "Service")
                    {
                        e.DefaultCondition = "status<>'V' and ApplicationType='Service'";
                    }
                    else
                    {
                        e.Condition = BizFunctions.F2Condition("matnum", (sender as TextBox).Text);
                        e.DefaultCondition = " flag='MATMSTK' and apnum='"+porh["apnum"].ToString()+"' ";
                    }
                    break;
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = dbaccess.DataSet.Tables["por1"];

            switch (e.MappingName)
            {
               
//select uom,uomqty,uomcode,pqty,qty from por1 where matnum='C006-001'
//select * from vAPMATM where matnum='C006-001'
//price = dpriceConvert

                case "apmatnum":
                    {
                        e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                   


                        e.CurrentRow["uom"] = e.F2CurrentRow["uom"];
                        e.CurrentRow["uomqty"] = e.F2CurrentRow["uomqty"];

                        e.CurrentRow["uomcode"] = e.F2CurrentRow["uomcode"];
                        //e.CurrentRow["pqty"] = e.F2CurrentRow["apmatnum"];

                        //e.CurrentRow["qty"] = e.F2CurrentRow["matnum"];
                        if (BizFunctions.IsEmpty(e.CurrentRow["price"]))
                        {
                            e.CurrentRow["price"] = e.F2CurrentRow["dpriceConvert"];
                        }
                        else if (Convert.ToDecimal(e.CurrentRow["price"]) == 0)
                        {
                            e.CurrentRow["price"] = e.F2CurrentRow["dpriceConvert"];
                        }



                        if (!BizFunctions.IsEmpty(porh["apnum"]))
                        {
                            
                            e.CurrentRow["baseunitprice"] = e.F2CurrentRow["baseunitprice"];
                        
                        }
                        else
                        {
                             e.CurrentRow["baseunitprice"] = e.F2CurrentRow["stdcost"];                  
                        }
                     
                       

                    }
                    break;

                case "matnum":
                    {
                 
                        e.CurrentRow["apmatnum"] = e.F2CurrentRow["apmatnum"];


                        e.CurrentRow["uom"] = e.F2CurrentRow["uom"];
                        e.CurrentRow["uomqty"] = e.F2CurrentRow["uomqty"];

                        e.CurrentRow["uomcode"] = e.F2CurrentRow["uomcode"];

                        if (BizFunctions.IsEmpty(e.CurrentRow["price"]))
                        {
                            e.CurrentRow["price"] = e.F2CurrentRow["dpriceConvert"];
                        }
                        else if (Convert.ToDecimal(e.CurrentRow["price"]) == 0)
                        {
                            e.CurrentRow["price"] = e.F2CurrentRow["dpriceConvert"];
                        }

                        //////////////////////////////
                        if (!BizFunctions.IsEmpty(porh["apnum"]))
                        {

                            e.CurrentRow["baseunitprice"] = e.F2CurrentRow["baseunitprice"];

                        }
                        else
                        {
                            e.CurrentRow["baseunitprice"] = e.F2CurrentRow["stdcost"];
                        }
                        ///////////////////////////////
                         
                    }
                    break;


                case "detail":
                    {

                        e.CurrentRow["apmatnum"] = e.F2CurrentRow["apmatnum"];
                        e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];

                        e.CurrentRow["uom"] = e.F2CurrentRow["uom"];
                        e.CurrentRow["uomqty"] = e.F2CurrentRow["uomqty"];

                        e.CurrentRow["uomcode"] = e.F2CurrentRow["uomcode"];

                        if (BizFunctions.IsEmpty(e.CurrentRow["price"]))
                        {
                            e.CurrentRow["price"] = e.F2CurrentRow["dpriceConvert"];
                        }
                        else if (Convert.ToDecimal(e.CurrentRow["price"]) == 0)
                        {
                            e.CurrentRow["price"] = e.F2CurrentRow["dpriceConvert"];
                        }

                        //////////////////////////////
                        if (!BizFunctions.IsEmpty(porh["apnum"]))
                        {

                            e.CurrentRow["baseunitprice"] = e.F2CurrentRow["baseunitprice"];

                        }
                        else
                        {
                            e.CurrentRow["baseunitprice"] = e.F2CurrentRow["stdcost"];
                        }
                        ///////////////////////////////

                    }
                    break;  

                    
            }
        }

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            DataRow porh = dbaccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = dbaccess.DataSet.Tables["por1"];

            switch (e.ControlName)
            {
                case "porh_ponum":

                    e.DefaultCondition = "(approvestatus ='Yes')   ";
                    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);

            DataRow porh = dbaccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = dbaccess.DataSet.Tables["por1"];

            switch (e.ControlName)
            {
                case "porh_ponum":

                    //BizFunctions.DeleteAllRows(por1);

                    if (!BizFunctions.IsEmpty(porh["ponum"]))
                    {
                        //porh["whnum"] = e.F2CurrentRow["whnum"];
//                        porh["type"] = "SITE";
//                        porh["type2"] = "General";

//                        #region Steph - Import grn1 to piv1

//                        string selectDor1 = "SELECT ISNULL(refnum,'') AS refnum,matnum,sum(qty) AS qty,uom,detail FROM " +
//                    " (SELECT refnum,matnum,approveqty as qty,uomcode uom,detail FROM trq1  UNION ALL " +
//"SELECT ponum as refnum,matnum,-qty as qty,uom,detail FROM por1) GrnVsPiv " +
//                    " WHERE refnum = '" + porh["ponum"].ToString().Trim() + "' " +
//                    " GROUP BY refnum,matnum,uom,detail  " +
//                    " HAVING SUM(qty) > 0";


//                        this.dbaccess.ReadSQL("grn1Tmp", selectDor1);
//                        DataTable grn1Tmp = this.dbaccess.DataSet.Tables["grn1Tmp"];

//                        BizFunctions.DeleteAllRows(por1);
//                        foreach (DataRow dr in grn1Tmp.Select())
//                        {
//                            dr.SetAdded();
//                            por1.ImportRow(dr);
//                        }
//                        foreach (DataRow dr2 in por1.Select())
//                        {
//                            dr2["refnum"] = porh["refnum"].ToString().Trim();
                           
//                            //	dr2["pqty"] = (decimal)dr2["qty"];
//                        }
//                        #endregion

//                        foreach (DataRow drP in por1.Rows)
//                        {
//                            if (drP.RowState != DataRowState.Deleted)
//                            {
//                                if (BizFunctions.IsEmpty(drP["uomcode"]))
//                                {
//                                    drP["uomcode"] = BizLogicTools.Tools.GetMatnumPUOM(drP["matnum"].ToString().Trim(), this.dbaccess);
//                                }
//                            }
//                        }
                    }
                    break;

                case "porh_docunum2":
                    {
                        GetDetailByContract();
                    }
                    break;

                case "porh_ptcnum":
                    {
                        porh["ptc"] = e.F2CurrentRow["ptc"].ToString() + " @" + e.F2CurrentRow["hp"].ToString();
                        porh["saddr1"] = e.F2CurrentRow["saddr1"];
                        porh["saddr2"] = e.F2CurrentRow["saddr2"];
                        porh["saddr3"] = e.F2CurrentRow["saddr3"];
                        porh["saddr4"] = e.F2CurrentRow["saddr4"];
     
                    }
                    break;


            }




        }
        #endregion

        #endregion

        #region Form Load
        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);

            opened = true;
            this.dbaccess = e.DBAccess;
            this.formsCollection = e.FormsCollection;
            this.formName = (sender as Form).Name;
            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            DataRow porh = dbaccess.DataSet.Tables["porh"].Rows[0];

            porh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

            e.DBAccess.DataSet.Tables["porh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PORH_ColumnChanged);
            e.DBAccess.DataSet.Tables["por1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_POR1_ColumnChanged);
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");

            #region initial controls
            rad_GRNN = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_GRNN") as RadioButton;
            rad_GRNY = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_GRNY") as RadioButton;
            btn_GRN = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_GRN") as Button;

            rad_PIVN = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_PIVN") as RadioButton;
            rad_PIVY = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_PIVY") as RadioButton;
            btn_PIV = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_PIV") as Button;

            rad_GRNN.CheckedChanged += new EventHandler(rad_GRNN_CheckedChanged);
            rad_GRNY.CheckedChanged += new EventHandler(rad_GRNY_CheckedChanged);
            rad_PIVN.CheckedChanged += new EventHandler(rad_PIVN_CheckedChanged);
            rad_PIVY.CheckedChanged += new EventHandler(rad_PIVY_CheckedChanged);

            btn_GRN.Click += new EventHandler(btn_GRN_Click);
            btn_PIV.Click += new EventHandler(btn_PIV_Click);

            btn_ExtractMATM = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_ExtractMATM") as Button;
            btn_ExtractMATM.Click += new EventHandler(btn_ExtractMATM_Click);

            btn_Import = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Import") as Button;
            btn_Import.Click += new EventHandler(btn_Import_Click);
            btn_Import.Enabled = true;


            btn_ExtractTRQ = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_ExtractTRQ") as Button;

            btn_ExtractTRQ.Click += new EventHandler(btn_ExtractTRQ_Click);
            //btn_ExtractTRQ = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_ExtractTRQ") as Button;
             
          
        

            #endregion

            #region set auto button
            if (porh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                btn_GRN.Enabled = false;
                btn_PIV.Enabled = false;
            }
            else
            {
                if (rad_GRNY.Checked)
                {
                    btn_GRN.Enabled = true;
                }
                else
                {
                    btn_GRN.Enabled = false;
                }

                if (rad_PIVY.Checked)
                {
                    btn_PIV.Enabled = true;
                }
                else
                {
                    btn_PIV.Enabled = false;
                }
            }
            #endregion

            #region set default deliver address
            string sqlAddr = "select coyaddr1+coyaddr2 as daddress from coy";
            DataSet dsAddr = this.dbaccess.ReadSQLTemp("coyAddr", sqlAddr);
            if (porh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                if (dsAddr.Tables.Count > 0)
                {
                    porh["daddress"] = dsAddr.Tables["coyAddr"].Rows[0]["daddress"];
                }
            }
            #endregion

            if (BizFunctions.IsEmpty(porh["SCNDate"]))
            {
                porh["SCNDate"] = DateTime.Today;
            }


            if (BizFunctions.IsEmpty(porh["ddate"]))
            {
                porh["ddate"] = DateTime.Today.AddDays(1);
            }


            if (BizFunctions.IsEmpty(porh["type"]))
            {
                porh["type"] = "Site";
            }


            if (BizFunctions.IsEmpty(porh["type2"]))
            {
                porh["type2"] = "General";
            }

            
        }

        void btn_ExtractTRQ_Click(object sender, EventArgs e)
        {
            DataTable por1 = this.dbaccess.DataSet.Tables["por1"];
            ATL.POR.ExtractTRQ form = new ATL.POR.ExtractTRQ(this.dbaccess, por1);
            form.Show();
            form.Focus();
        }
        #endregion

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);
            DataRow sivh = this.dbaccess.DataSet.Tables["porh"].Rows[0];

            if (sivh["flag"].ToString().Trim() != "POR")
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

        #region private function
        private void btn_GRN_Click(object sender, EventArgs e)
        {
            POR.AutoGRN GRNFrm = new AutoGRN();
            GRNFrm.ShowDialog();
        }

        //private void btn_ExtractTRQ_Click(object sender, EventArgs e)
        //{
        //    DataTable por1 = this.dbaccess.DataSet.Tables["por1"];
        //    ATL.POR.ExtractTRQ form = new ATL.POR.ExtractTRQ(this.dbaccess, por1);
        //    form.Show();
        //    form.Focus();
        //}

        private void btn_PIV_Click(object sender, EventArgs e)
        {
            POR.AutoPIV PIVFrm = new AutoPIV();
            PIVFrm.ShowDialog();
        }
       
        private void rad_GRNN_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_GRNN.Checked)
            {
                btn_GRN.Enabled = false;
            }
        }
        private void rad_GRNY_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_GRNY.Checked)
            {
                btn_GRN.Enabled = true;
            }
        }
        private void rad_PIVN_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_PIVN.Checked)
            {
                btn_PIV.Enabled = false;
            }
        }
        private void rad_PIVY_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_PIVY.Checked)
            {
                btn_PIV.Enabled = true;
            }
        }
        #endregion

        #region Document_TabControl_Handle

        protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_TabControl_Handle(sender, e);
        }

        #endregion

        #region Refresh
        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow porh = e.DBAccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = e.DBAccess.DataSet.Tables["por1"];

            if (BizFunctions.IsEmpty(porh["senderemail"]))
            {
                porh["senderemail"] = BizLogicTools.Tools.GetEmailByEmpnum(this.dbaccess, Common.DEFAULT_SYSTEM_EMPNUM);
            }

            if (por1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in por1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (dr1["price"] != System.DBNull.Value && Convert.ToDecimal(dr1["pqty"]) > 0)
                        {
                            if(Convert.ToDecimal(dr1["pqty"])>0)
                            {
                                if (Convert.ToDecimal(dr1["uomqty"]) > 0)
                                {
                                    dr1["qty"] = Convert.ToDecimal(dr1["pqty"]) * Convert.ToDecimal(dr1["uomqty"]);
                                }
                            }
                        
                       
               
                            #region set deafult discamt=0
                            if (dr1["discamt"] == System.DBNull.Value)
                            {
                                dr1["discamt"] = 0.00;
                            }
                            #endregion
                            dr1["grosamt"] = (decimal)dr1["price"] * (decimal)dr1["pqty"];
                            dr1["oriamt"] = (decimal)dr1["grosamt"] - (decimal)dr1["discamt"];
                        }
                    }
                }
            }

            setDefaults(dbaccess.DataSet, "porh/por1");

            if (porh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSN)
            {
                if (porh["docunum"].ToString().Trim() == String.Empty || porh["docunum"] == System.DBNull.Value)
                    porh["docunum"] = porh["refnum"];
            }
            AutoCalc();
        }
        #endregion

        #region ColumnChangedEvents
        #region porh
        private void Voucher_PORH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow porh = this.dbaccess.DataSet.Tables["porh"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "apnum":
                    dbaccess.ReadSQL("getApmInfo", "SELECT apnum,apname,ptc,address,phone,hp,fax,ptnum,gstgrpnum,oricur FROM apm where apnum ='" + e.Row["apnum"].ToString().Trim() + "'");

                    if (dbaccess.DataSet.Tables["getApmInfo"].Rows.Count > 0)
                    {
                        DataRow getApmInfo = dbaccess.DataSet.Tables["getApmInfo"].Rows[0];
                        e.Row["apname"] = getApmInfo["apname"];
                        e.Row["contact"] = getApmInfo["ptc"];
                        e.Row["address"] = getApmInfo["address"];
                        e.Row["phone"] = getApmInfo["phone"];
                        e.Row["hp"] = getApmInfo["hp"];
                        e.Row["fax"] = getApmInfo["fax"];

                        if (e.Row["payterms"].ToString().Trim() == "" || e.Row["payterms"] == System.DBNull.Value)
                            e.Row["payterms"] = getApmInfo["ptnum"];
                        if (e.Row["gstgrpnum"].ToString().Trim() == "" || e.Row["gstgrpnum"] == System.DBNull.Value)
                            e.Row["gstgrpnum"] = getApmInfo["gstgrpnum"];
                        if (e.Row["oricur"].ToString().Trim() == "" || e.Row["oricur"] == System.DBNull.Value)
                            e.Row["oricur"] = getApmInfo["oricur"];

                    }
                    else
                    {
                        e.Row["apname"] = "";
                        e.Row["contact"] = "";
                        e.Row["address"] = "";
                        e.Row["phone"] = "";
                        e.Row["hp"] = "";
                        e.Row["fax"] = "";
                        e.Row["payterms"] = "";
                        e.Row["gstgrpnum"] = "";
                        e.Row["oricur"] = "";
                    }
                    break;
                case "oricur":
                    #region set exrate
                    e.Row.BeginEdit();
                    string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
                    this.dbaccess.ReadSQL("exrate", exrStr);
                    if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
                    {
                        decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + ""]);
                        e.Row["exrate"] = exrate;
                    }
                    e.Row.EndEdit();
                    break;
                    #endregion
                case "gstgrpnum":
                    #region set gstper

                    e.Row.BeginEdit();
                    this.dbaccess.ReadSQL("gstm", "SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='" + e.Row[e.Column.ColumnName].ToString() + "'");
                    if (this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
                    {
                        if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
                        {
                            //if ((decimal)e.Row["gstper"] == 0 || e.Row["gstper"] == System.DBNull.Value)
                            //{
                            e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)e.Row["trandate"]);
                            //}
                        }
                        else
                        {
                            e.Row["gstper"] = 0;
                        }
                    }
                    e.Row.EndEdit();
                    break;

                    #endregion
                case "trandate":
                    #region set dorh exrate

                    e.Row.BeginEdit();
                    //e.Row["exrate"] = BizAccounts.GetExRate(this.dbaccess, e.Row["oricur"].ToString(), (DateTime)e.Row[e.Column.ColumnName]);
                    string strexr = "Select rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + " FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
                    this.dbaccess.ReadSQL("exrate", strexr);
                    if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
                    {
                        decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + ""]);
                        e.Row["exrate"] = exrate;
                    }
                    e.Row.EndEdit();

                    e.Row["ddate"] = Convert.ToDateTime(e.Row["trandate"]).AddDays(1);
                    break;
                    #endregion
            }
        }
        #endregion

        #region por1
        private void Voucher_POR1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow porh = this.dbaccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = this.dbaccess.DataSet.Tables["por1"];
            switch (e.Column.ColumnName)
            {
                case "matnum":
                    dbaccess.ReadSQL("getMatm", "SELECT matname, matm.uomcode,puom,ploftcode,dpriceConvert FROM matm left join matm2 on matm.matnum=matm2.matnum where matm.matnum='" + e.Row["matnum"].ToString().Trim() + "'");
                    if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
                    {
                        if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
                            e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
                        if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
                            e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
                        if (e.Row["uomqty"].ToString().Trim() == "" || e.Row["uomqty"] == System.DBNull.Value)
                        {
                            e.Row["uomqty"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["ploftcode"];
                            e.Row["pqty"] = (decimal)e.Row["qty"] / (decimal)e.Row["uomqty"];
                        }
                        if (e.Row["uomcode"].ToString().Trim() == "" || e.Row["uomcode"] == System.DBNull.Value)  //purchase uom
                            e.Row["uomcode"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["puom"];
                        if (e.Row["price"].ToString().Trim() == "" || e.Row["price"] == System.DBNull.Value)
                        {
                            if (!BizFunctions.IsEmpty(dbaccess.DataSet.Tables["getMatm"].Rows[0]["dpriceConvert"]))
                            {
                                e.Row["price"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["dpriceConvert"];
                            }
                            else { }
                        }
                    }
                    break;

                //case "pqty":
                //       if (e.Row["price"] != System.DBNull.Value && Convert.ToDecimal(e.Row["pqty"]) > 0)
                //       {
                //           e.Row["qty"] = Convert.ToDecimal(e.Row["pqty"]) * Convert.ToDecimal(e.Row["uomqty"]);
                //           #region set deafult discamt=0
                //           if (e.Row["discamt"] == System.DBNull.Value)
                //           {
                //               e.Row["discamt"] = 0.00;
                //           }
                //           #endregion
                //           e.Row["grosamt"] = (decimal)e.Row["price"] * (decimal)e.Row["pqty"];
                //           e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                //       }
                //       break;


                //case "pqty":
                //      if (e.Row["price"] != System.DBNull.Value && Convert.ToDecimal(e.Row["pqty"]) > 0)
                //      {
                //          e.Row["qty"] = Convert.ToDecimal(e.Row["pqty"]) * Convert.ToDecimal(e.Row["uomqty"]);
                //          #region set deafult discamt=0
                //          if (e.Row["discamt"] == System.DBNull.Value)
                //          {
                //              e.Row["discamt"] = 0.00;
                //          }
                //          #endregion
                //          e.Row["grosamt"] = (decimal)e.Row["price"] * (decimal)e.Row["pqty"];
                //          e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                //      }
                //      break;
              
                case "price":
                    if (e.Row["pqty"] != System.DBNull.Value && Convert.ToDecimal(e.Row["price"]) > 0)
                    {
                        #region set deafult discamt=0
                        if (e.Row["discamt"] == System.DBNull.Value)
                        {
                            e.Row["discamt"] = 0.00;
                        }
                        #endregion
                        e.Row["grosamt"] = (decimal)e.Row["price"] * (decimal)e.Row["pqty"];
                        e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                    }
                    break;
                case "discamt":
                    #region set deafult discamt=0
                    if (e.Row["discamt"] == System.DBNull.Value)
                    {
                        e.Row["discamt"] = 0.00;
                    }
                    #endregion
                    if (e.Row["grosamt"] != System.DBNull.Value && Convert.ToDecimal(e.Row["discamt"]) > 0)
                    {
                        e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                    }
                    break;
            }
        }
        #endregion
        #endregion

        #region fun fun - To set default values
        public static void setDefaults(DataSet dataSet, string tableNames)
        {
            string[] tables = tableNames.Split(new char[] { '/', '\\' });

            for (int i = 0; i < tables.Length; i++)
            {
                DataTable dt = dataSet.Tables[tables[i]];

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            switch (dc.DataType.ToString())
                            {
                                // All decimals are 0 by default
                                case "System.Decimal":
                                    if (dr[dc.ColumnName] == System.DBNull.Value)
                                        dr[dc.ColumnName] = 0;
                                    break;

                                // All smallints are 0 by default
                                case "System.Int16":
                                    if (dr[dc.ColumnName] == System.DBNull.Value)
                                        dr[dc.ColumnName] = 0;
                                    break;

                                // All ints are 0 by default
                                case "System.Int32":
                                    if (dr[dc.ColumnName] == System.DBNull.Value)
                                        dr[dc.ColumnName] = 0;
                                    break;

                                // All bigints are 0 by default but do not touch ID
                                case "System.Int64":
                                    if (dr[dc.ColumnName] == System.DBNull.Value && dc.ColumnName != "ID")
                                        dr[dc.ColumnName] = 0;
                                    break;

                                // All bits are 0 by default
                                case "System.Bit":
                                    if (dr[dc.ColumnName] == System.DBNull.Value)
                                        dr[dc.ColumnName] = 0;
                                    break;

                                // All booleans are false by default
                                case "System.Boolean":
                                    if (dr[dc.ColumnName] == System.DBNull.Value)
                                        dr[dc.ColumnName] = false;
                                    break;

                                // Trim white spaces due to user entry
                                case "System.String":
                                    if (dr[dc.ColumnName] != System.DBNull.Value)
                                        dr[dc.ColumnName] = (dr[dc.ColumnName] as String).Trim();
                                    break;
                            }
                        }
                    }
                }
            }

        }
        #endregion

        #region Steph - To set the Auto Calculate to be use in various event
        private void AutoCalc()
        {
            DataRow porh = dbaccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = dbaccess.DataSet.Tables["por1"];
            porh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(porh["trandate"]));

            #region por1

            por1_grosamt = 0;
            por1_discamt = 0;
            por1_oriamt = 0;

            foreach (DataRow dr in por1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(porh, dr, "user/status/flag/trandate/expire/created/modified/docunum/apnum");
                    if ((decimal)dr["grosamt"] == 0 || dr["grosamt"] == System.DBNull.Value)
                        dr["grosamt"] = (decimal)dr["pqty"] * (decimal)dr["price"];

                    dr["oriamt"] = (decimal)dr["grosamt"] - (decimal)dr["discamt"];

                    por1_grosamt += (decimal)dr["grosamt"];
                    por1_discamt += (decimal)dr["discamt"];
                    por1_oriamt += (decimal)dr["oriamt"];
                }
            }
            porh["grosamt"] = por1_grosamt;
            porh["discamt"] = por1_discamt;
            porh["oriamt"] = por1_oriamt;
            #endregion
        }
        #endregion

        #region Extract

        private void btn_ExtractMATM_Click(object sender, EventArgs e)
        {
            DataRow porh = dbaccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = dbaccess.DataSet.Tables["por1"];

            Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            DataTable oriTable = por1;
            try
            {
                // Open Extract Form
                ATL.ExtractMATM.ExtractMATM extract = new ATL.ExtractMATM.ExtractMATM(this.dbaccess, oriTable);
                extract.ShowDialog(frm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        #endregion


        #region Import from Excel


        protected void btn_Import_Click(object sender, System.EventArgs e)
        {
            
            //DataRow porh = this.dbaccess.DataSet.Tables["porh"].Rows[0];
     
            //Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            //Outlook._MailItem oMailItem = (Outlook._MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
            //oMailItem.Subject = "Purchase Order From ATL";
            //oMailItem.Body = "Dear \n\nPlease see Attached PO for your immediate attention. Contact the undersigned should you have any queries.\n\nThank You & Regards,\n\n";
            //int iPosition = (int)oMailItem.Body.Length + 1;
            //int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
            //String sDisplayName = "MyAttachment";

            //#region If Marked
            //{
            //    this.dbaccess.ReadSQL("ARM", "select * from apm where apnum='" + porh["apnum"].ToString() + "'");
            //    DataTable dtarm = dbaccess.DataSet.Tables["arm"];
            //    if (File.Exists(filepath + "\\" + porh["refnum"].ToString() + ".pdf"))
            //    {
            //        Outlook.Attachment oAttach = oMailItem.Attachments.Add(filepath + "\\" + porh["refnum"].ToString() + ".pdf", iAttachType, iPosition, sDisplayName);
            //    }
            //    if (dtarm.Rows.Count > 0 && oMailItem.To == null)
            //    {
            //        oMailItem.To = dtarm.Rows[0]["email"].ToString();
            //        oMailItem.CC = dtarm.Rows[0]["cc"].ToString();
            //    }
            //}
            //#endregion
            //oMailItem.Display(true);
        }


        #endregion

        #region openFileDialog
        protected void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DataRow porh = this.dbaccess.DataSet.Tables["porh"].Rows[0];
                DataTable por1 = this.dbaccess.DataSet.Tables["por1"];
                string Path = (sender as OpenFileDialog).FileName;
                //Read data from Excel,and return the dataset
                DataSet ds = ExcelToDS(Path, "XSL", 0);

                //DataTable XSL = null;

                //XSL = ds.Tables["XSL"].Copy();

                //if (this.dbaccess.DataSet.Tables.Contains("XSL"))
                //{
                //    this.dbaccess.DataSet.Tables.Remove("XSL");
                //    XSL.TableName = "XSL";
                //    this.dbaccess.DataSet.Tables.Add(XSL);
                //    //this.dbaccess.DataSet.Tables["XSL"].Dispose();
                //}
                //else
                //{
                //    XSL.TableName = "XSL";
                //    this.dbaccess.DataSet.Tables.Add(XSL);
                //}

                //Delete the old datas in detail1
                #region Commented
                //int iCount = this.dbAccess.DataSet.Tables["por1"].Rows.Count;
                //for (int iPos = iCount - 1; iPos >= 0; iPos--)
                //{
                //    DataRow row = this.dbAccess.DataSet.Tables["por1"].Rows[iPos];
                //    if (row.RowState == DataRowState.Added)
                //    {
                //        this.dbAccess.DataSet.Tables["por1"].Rows.Remove(row);
                //    }
                //    else if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                //    {
                //        row.Delete();
                //    }
                //}
                #endregion

                //copy the data in dataset(from Excel) to dto1
                //foreach (DataRow dr1 in por1.Rows)
                //{
                //    if (dr1.RowState != DataRowState.Deleted)
                //    {

                if (por1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(por1);
                }
                foreach (DataRow dr in ds.Tables["XSL"].Rows)
                {

                    if (dr.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr["PLU"]))
                        {
                            DataRow InsertPor1 = por1.NewRow();
                            InsertPor1["matnum"] = dr["PLU"];


                            if (dr["Description"].ToString().Length > 100)
                            {
                                InsertPor1["detail"] = dr["Description"].ToString().Trim().Replace(" ", "").Substring(0, 99);
                            }
                            else
                            {
                                InsertPor1["detail"] = dr["Description"].ToString();
                            }

                            if (Convert.ToString(dr["Quantity"]).Contains("-") || BizFunctions.IsEmpty(dr["Quantity"]))
                            {
                                InsertPor1["qty"] = System.DBNull.Value;
                            }
                            else
                            {
                                InsertPor1["qty"] = dr["Quantity"];
                            }

                            if (Convert.ToString(dr["uprice"]).Contains("-") || BizFunctions.IsEmpty(dr["uprice"]))
                            {
                                InsertPor1["uprice"] = System.DBNull.Value;
                            }
                            else
                            {
                                InsertPor1["uprice"] = dr["uprice"];
                            }


                            if (dr["UOM"].ToString().Contains("-") || BizFunctions.IsEmpty(dr["UOM"]))
                            {
                                InsertPor1["uom"] = System.DBNull.Value;
                            }
                            else
                            {
                                InsertPor1["uom"] = dr["UOM"];
                            }

                            if (dr["price"].ToString().Contains("-") || BizFunctions.IsEmpty(dr["price"]))
                            {
                                InsertPor1["Price"] = 0;
                            }
                            else
                            {
                                InsertPor1["price"] = dr["price"];
                            }

                            if (dr["Manufacturer"].ToString().Length > 5)
                            {
                                InsertPor1["pbrdcode"] = dr["Manufacturer"].ToString().Trim().Replace(" ", "").Substring(0, 4);
                            }
                            else
                            {
                                InsertPor1["pbrdcode"] = dr["Manufacturer"].ToString();
                            }

                            if (dr["Shaft"].ToString().Length > 5)
                            {
                                InsertPor1["pshfcode"] = dr["Shaft"].ToString().Trim().Replace(" ", "").Substring(0, 4);
                            }
                            else
                            {
                                InsertPor1["pshfcode"] = dr["Shaft"].ToString();
                            }

                            if (dr["Model"].ToString().Length > 10)
                            {
                                InsertPor1["modelcode"] = dr["Model"].ToString().ToString().Trim().Replace(" ", "").Substring(0, 9);
                            }
                            else
                            {
                                InsertPor1["modelcode"] = dr["Model"].ToString();
                            }


                            // Too long.  To check with them(Should give codes only)
                            //InsertPor1["pbrdcode"] = dr["Manufacturer"].ToString().Trim().Replace(" ","").Substring(0, 4);
                            //InsertPor1["pshfcode"] = dr["Shaft"].ToString().Trim().Replace(" ", "").Substring(0, 4);
                            //InsertPor1["modelcode"] = dr["Model"].ToString().ToString().Trim().Replace(" ", "").Substring(0, 4);

                            if (dr["Category"].ToString().Contains("-") || BizFunctions.IsEmpty(dr["Category"]))
                            {
                                InsertPor1["pcatcode"] = System.DBNull.Value;
                            }
                            else
                            {
                                InsertPor1["pcatcode"] = dr["Category"];
                            }

                            if (dr["Loft"].ToString().Contains("-") || BizFunctions.IsEmpty(dr["Loft"]))
                            {
                                InsertPor1["ploftcode"] = System.DBNull.Value;
                            }
                            else
                            {
                                InsertPor1["ploftcode"] = dr["Loft"];
                            }

                            if (dr["Flex"].ToString().Contains("-") || BizFunctions.IsEmpty(dr["Flex"]))
                            {
                                InsertPor1["pflexcode"] = System.DBNull.Value;
                            }
                            else
                            {
                                InsertPor1["pflexcode"] = dr["Flex"];
                            }
                            InsertPor1["boxno"] = dr["boxno"];








                            //InsertPor1["grossamt"] = dr[""];
                            //InsertPor1["discamt"] = dr[""];
                            //InsertPor1["oriamt"] = dr[""];

                            por1.Rows.Add(InsertPor1);
                        }
                    }
                }
                //    }
                //}

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //MessageBox.Show("Error occured! Please make sure Microsoft Office is install correctly in this PC", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region ExcelToDS
        public DataSet ExcelToDS(string Path, string tablename, int sheetIndex)
        {
            string strConn = "Provider = Microsoft.Jet.OLEDB.4.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            OleDbConnection conn = new OleDbConnection(strConn);

            conn.Open();
            System.Data.DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dbSchema == null || dbSchema.Rows.Count < 1)
            {
                throw new Exception("Error: Could not get Excel schema table.");
            }
            string sheetName = "[" + dbSchema.Rows[sheetIndex]["TABLE_NAME"].ToString() + "]";
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from " + sheetName;
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, tablename);
            conn.Close();
            return ds;
        }
        #endregion

        private void GetDetailByContract()
        {
            DataRow porh = this.dbaccess.DataSet.Tables["porh"].Rows[0];
            DataTable por1 = this.dbaccess.DataSet.Tables["por1"];

            if (!BizFunctions.IsEmpty(porh["docunum2"]))
            {
                if (por1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(por1);
                }


                string getCtr = "SELECT D.sitenum,D.sectorcode,D.refnum,D.matnum,D.matname,SUM(D.qty) as qty " +
                                "From " +
                                "( " +
                                "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR3 WHERE refnum='" + porh["docunum2"].ToString() + "' AND [status]<>'V'" +
                                "UNION " +
                                "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR7 WHERE refnum='" + porh["docunum2"].ToString() + "' AND [status]<>'V'" +
                                "UNION " +
                                "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR5 WHERE refnum='" + porh["docunum2"].ToString() + "' AND [status]<>'V'" +
                                "UNION " +
                                "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR8 WHERE refnum='" + porh["docunum2"].ToString() + "' AND [status]<>'V'" +
                                "UNION " +
                                "SELECT refnum,matnum,matname,(qty/mthnum) as qty FROM CTR9 WHERE refnum='" + porh["docunum2"].ToString() + "' AND [status]<>'V' " +
                                ")A1 LEFT JOIN ADH A ON A1.refnum=A.refnum where A.[status]<>'V' " +
                                "GROUP BY A.sitenum,A.sectorcode,A1.refnum,A1.matnum,A1.matname";




                this.dbaccess.ReadSQL("MatTB", getCtr);

                DataTable MatTB = this.dbaccess.DataSet.Tables["MatTB"];

                if (MatTB != null)
                {
                    if (MatTB.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in MatTB.Rows)
                        {
                            DataRow Insertpor1 = por1.NewRow();
                            //Insertpor1["docunum"] = dr1["refnum"];
                            Insertpor1["matnum"] = dr1["matnum"];
                            Insertpor1["apmatnum"] = dr1["apmatnum"];
                            //Insertpor1["detail"] = dr1["matname"];
                            Insertpor1["uom"] = dr1["uomcode"];
                            Insertpor1["stdqty"] = dr1["qty"];
                            Insertpor1["qty"] = dr1["uomqty"];
                            por1.Rows.Add(Insertpor1);
                        }
                    }
                }
            }
        }


       
    }
}

