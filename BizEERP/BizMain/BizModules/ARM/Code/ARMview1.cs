
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizDetail;
using BizRAD.BizApplication;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizTools;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizAccounts;
using BizRAD.BizReport;


using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using ATL.BizLogicTools;


namespace ATL.BizModules.ARM
{
    class ARMview1
    {
        #region Class Variables
        protected string projectPath = null;
        DBAccess dbAccess = null;
        protected Hashtable selectsCollection = null;
        Form form;

        string projectpath = "";
        int rowsAffected = 0;
        int value = 1;
        int TotalPages = 0;
        TextBox txt_selDOnumFrom;
        TextBox txt_selDOnumTo;
        DateTimePicker datefrom;
        DateTimePicker dateto;
        ComboBox cmbGrouptype;

        #endregion

        #region Constructor
        public ARMview1()
        {
            //Set the default value for the number of Copies Once the 'Preview' form shows up
            this.selectsCollection = new Hashtable();
            this.dbAccess = new DBAccess();
            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
            Form DelOrder = BizXmlReader.CurrentInstance.Load(this.projectPath + @"\ARM\UIFile\ARMprev1.xml", "ARMpreview", this, null) as Form;

            IntialiseTextboxes();
            GetDateTime();

            DelOrder.FormBorderStyle = FormBorderStyle.FixedSingle;
            DelOrder.ShowDialog();
        }
        #endregion

        #region Click Print

        protected void btn_ok_preview_Click(object sender, System.EventArgs e)
        {
            RequestDelOrdList(sender, e);
        }

        public void RequestDelOrdList(object receiver, System.EventArgs e)
        {
            #region Comment off - Previously use store procedure
            //Parameter[] parameters = new Parameter[4];
            //parameters[0] = new Parameter("@Datefrom", this.datefrom.Value.ToString());
            //parameters[1] = new Parameter("@DateTo", this.dateto.Value.ToString());
            //parameters[2] = new Parameter("@DOfrom", this.txt_selDOnumFrom.Text);
            //parameters[3] = new Parameter("@DOTo", this.txt_selDOnumTo.Text);

            //DataSet dorTemp = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult("JAO_FAV_GetARMdetials1", ref parameters);

            //if (dorTemp.Tables[0].Rows.Count > 0)
            //{
            //    DisplayCrystalReport(dorTemp);
            //}
            //else
            //{
            //    MessageBox.Show("No Matching Delivery Report To Extract!", "System Message");
            //}
            #endregion
            //DataSet ds = new DataSet();
            //DataTable dt = new DataTable("dt");
            //dt.Columns.Add("CODE");
            //dt.Columns.Add("ITEM");
            try
            {
                #region Details from XML
                DateTime dtDelDateFr = Convert.ToDateTime(datefrom.Value);
                DateTime dtDelDateTo = Convert.ToDateTime(dateto.Value);

                string DOFrom = txt_selDOnumFrom.Text;
                string DOTO = txt_selDOnumTo.Text;
                #endregion
                #region SQL ARMH Header - Get All the DO within the filter period
                string sql = " SELECT DH.refnum, isnull(DH.orderRefNo, '') as orderRefNo, DH.arnum, "
                + " DH.arname, DH.outlet,isnull(DH.delDate, '') as delDate, DH.vehCode,"
                + " isnull(a.morning, '') as delMode, "
                + " DH.ponum,DH.terms,DH.remark1,DH.remark2,DH.delAddr1,a.addr1,DH.TotalDO, "
                + " DH.GSTDO, DH.GrandTotalDO, DH.cbPriceAdjLbl,DH.DoPrintType,DH.GST,DH.DOnum,"
                + " 0 as isTmp "
                + " from ARMH" + Common.DEFAULT_SYSTEM_YEAR + " as DH "
                + " inner join ARM1" + Common.DEFAULT_SYSTEM_YEAR + " as d1 on DH.refnum = d1.refnum"
                + " left join arm as a on a.arnum = DH.arnum "
                + " WHERE DH.delDate >=convert(DateTime, '" + dtDelDateFr.ToString("dd/MM/yyyy") + "', 103) "
                + " AND DH.delDate <= convert(DateTime, '" + dtDelDateTo.ToString("dd/MM/yyyy") + "', 103) "
                + " AND DH.status = 'O' and DH.flag = 'ARM' and DH.[group]='" + cmbGrouptype.Text + "' ";
                //DO # Filter
                if (DOFrom != "" && DOTO == "")
                {
                    string valueFr = DOFrom.Substring(1);
                    sql += " AND DH.groupID = " + valueFr;
                }
                else if (DOTO != "" && DOFrom == "")
                {
                    string valueTo = DOTO.Substring(1);
                    sql += " AND DH.groupID = " + valueTo;
                }
                else if (DOFrom != "" && DOTO != "")
                {
                    string valueFr = DOFrom.Substring(1);
                    string valueTo = DOTO.Substring(1);
                    sql += " AND DH.groupID >= " + valueFr + " and DH.groupID <=" + valueTo;
                }
                sql += " group by "
                + " DH.refnum, isnull(DH.orderRefNo, ''), DH.arnum,"
                + " DH.arname, DH.outlet,isnull(DH.delDate, ''), DH.vehCode,"
                + " isnull(a.morning, ''),"
                + " DH.ponum,DH.terms,DH.remark1,DH.remark2,DH.delAddr1,a.addr1,DH.TotalDO,"
                + " DH.GSTDO, DH.GrandTotalDO, DH.cbPriceAdjLbl, DH.DoPrintType,"
                + " DH.GST,DH.DOnum ";
                #endregion

                #region SQL ARM1 Detail - Get All Ordered Item of the DO
                sql += " SELECT D1.delDate,D1.refnum,"
                + " D1.shift,  D1.matnum, D1.matname, D1.addItemRmk, D1.totalBarLbl, D1.aTotalBarLbl,"
                + " D1.scanNetWeight, D1.finalNetWeight, D1.adjNetWeight, "
                + " D1.qty, D1.scanQty, D1.adjustQty, D1.finalQty, D1.mPrice, D1.ccmPrice, "
                + " D1.finalPrice, D1.totalCrate, D1.PackSize, D1.pFactorPPM, "
                + " D1.baseuom,D1.sellUOM,D1.pFactor, D1.pQty, D1.wtTo,D1.wtFrom, D1.size, "
                + " D1.prodType, D1.TotalItemPrice,DH.DOnum,"
                + " isnull(ch.matnameChn,'') as matnameCH,";
                if (cmbGrouptype.Text != "N")
                    sql += "Ceiling(D1.finalQty/D1.pFactorPPM) as packing,isnull(sc.isShowWT,1) as isShowWT ";
                else
                    sql += "D1.totalCrate as packing,isShowWT = cast(1 as bit) ";

                sql += " FROM ARM1" + Common.DEFAULT_SYSTEM_YEAR + " AS D1 "
                + " inner join ARMH" + Common.DEFAULT_SYSTEM_YEAR + " AS DH "
                + " on D1.refnum = DH.refnum "
                + " LEFT join ppmch ch on D1.matnum = ch.matnum ";
                if (cmbGrouptype.Text != "N")
                    sql += " LEFT join specialCases sc on DH.arnum = sc.arnum ";
                sql += " WHERE D1.delDate >=convert(DateTime, '" + dtDelDateFr.ToString("dd/MM/yyyy") + "', 103) "
                + " AND D1.delDate <= convert(DateTime, '" + dtDelDateTo.ToString("dd/MM/yyyy") + "', 103) "
                + " and DH.status = 'O' AND DH.flag = 'ARM'and  D1.finalQty > 0"
                + " and DH.[group]='" + cmbGrouptype.Text + "' ";

                if (DOFrom != "" && DOTO == "")
                {
                    string valueFr = DOFrom.Substring(1);
                    sql += " AND DH.groupID = " + valueFr;
                }
                else if (DOTO != "" && DOFrom == "")
                {
                    string valueTo = DOTO.Substring(1);
                    sql += " AND DH.groupID = " + valueTo;
                }
                else if (DOFrom != "" && DOTO != "")
                {
                    string valueFr = DOFrom.Substring(1);
                    string valueTo = DOTO.Substring(1);
                    sql += " AND DH.groupID >= " + valueFr + " and DH.groupID <=" + valueTo;
                }

                //Sql havent finish. Add GROUP BY & ORDER BY
                if (sql != "")
                {
                    sql += " GROUP BY  D1.delDate,D1.refnum,D1.shift,  D1.matnum, D1.matname, "
                       + " D1.addItemRmk, D1.totalBarLbl, D1.aTotalBarLbl,  D1.scanNetWeight, D1.finalNetWeight, "
                       + " D1.adjNetWeight, D1.qty, D1.scanQty, D1.adjustQty, D1.finalQty, D1.mPrice, D1.ccmPrice,"
                       + " D1.finalPrice, D1.totalCrate, D1.PackSize, D1.pFactorPPM, D1.baseuom,D1.sellUOM, D1.pFactor, D1.pQty, "
                       + " D1.wtTo,  D1.wtFrom, D1.size, D1.prodType,D1.TotalItemPrice,DH.DOnum,ch.matnameChn";
                    if (cmbGrouptype.Text != "N")
                        sql += ",sc.isShowWT ";
                    sql += " ORDER BY D1.refnum,D1.matnum ";
                }
                #endregion


                //The date got issue here. But dont know what is it
                //CHEN HAO EDIT ON 8 JUNE 2011
                #region Add in ADJM Table - Price Adjust Label(Havent add in the ShowDateFr/ShowDateTo)
                sql += " select 1 as cbPriceAdjLbl,a2.matname, a2.priceAdjust from ADJM as a "
                   + " left join ADJM2 as a2 on a2.refnum=a.refnum "
                   + " where "
                   + " a.showDateFr >= '" + dtDelDateFr.ToString("yyyyMMdd") + "' "
                   + " and '" + dtDelDateFr.ToString("yyyyMMdd") + "' <= a.showDateTo "
                   + " and a2.matname is not null "
                   + " and a.status = 'P' ";
                //sql += "SELECT 1 as cbPriceAdjLbl, D.REFNUM, A2.MATNAME,A2.PRICEADJUST"
                //    + " FROM ARMH2011 D, ADJM2 A2 "
                //    + " WHERE  D.DELDATE = '" + dtDelDateFr.ToString("yyyyMMdd") + "' "
                //    + " and A2.REFNUM "
                //    + " IN ( SELECT REFNUM FROM ADJM WHERE showDateFr >= '" + dtDelDateFr.ToString("yyyyMMdd") + "' "
                //    + " and  '" + dtDelDateFr.ToString("yyyyMMdd") + "' <= showDateTo)";
                #endregion

                #region NOOfItem In Each DO Order (Need to chg the OR to AND)
                sql += " select D1.refnum,DH.DOnum,count(D1.line) as NoOfItems from ARM1" + Common.DEFAULT_SYSTEM_YEAR + " D1 "
                     + " INNER JOIN ARMH" + Common.DEFAULT_SYSTEM_YEAR + " DH "
                     + " ON D1.REFNUM = DH.REFNUM"
                     + " WHERE DH.delDate >= '" + dtDelDateFr.ToString("yyyyMMdd") + "' "
                     + " and DH.delDate <= '" + dtDelDateTo.ToString("yyyyMMdd") + "' "
                     + " and DH.status = 'O'  and DH.flag = 'ARM' "
                     + " and DH.[group]='" + cmbGrouptype.Text + "' "
                     + " group by D1.refnum,DH.DOnum ";
                #endregion

                #region DataSet for 4 table
                DataSet ds = this.dbAccess.ReadSQLTemp("ds", sql);
                ds.Tables[0].TableName = "ARMHeader";
                ds.Tables[1].TableName = "ARMDetail";
                ds.Tables[2].TableName = "PriceAdjLblList";
                ds.Tables[3].TableName = "ItemsCount";
                #endregion

                #region Display Report
                if (ds.Tables[0].Rows.Count != 0)
                {
                    form = BizXmlReader.CurrentInstance.Load(@"FormPreviewWithCancel.xml", "formPreview", this, null) as Form;
                    CrystalReportViewer crystalReportViewer1 = BizXmlReader.CurrentInstance.GetControl("formPreview", "crystalReportViewer1") as CrystalReportViewer;
                    ReportDocument crReportDocument = new ReportDocument();
                    if (cmbGrouptype.Text != "N")
                        crReportDocument.Load(this.projectPath + @"\ARM\Report\rptDOA4.rpt");
                    else
                        crReportDocument.Load(this.projectPath + @"\ARM\Report\rptDOA4NTUC.rpt");

                    crReportDocument.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = crReportDocument;
                    form.ShowDialog();
                    form.Dispose();
                }
                else
                {
                    MessageBox.Show("No Record Found For The Selection", "Invalid Handle", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                #endregion Display Report
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion

        #region (F2) Key
        #region select Delivery Order From (F2)
        //To do
        #endregion

        #region select Delivery Order To (F2)
        //To do
        #endregion
        #endregion

        #region Get Date and Delivery Order List

        private void GetDateTime()
        {
            DateTime resetDateFrom = datefrom.Value.Date;
            resetDateFrom = resetDateFrom.AddDays(+1);
            datefrom.Value = resetDateFrom;

            DateTime resetDateTo = dateto.Value.Date;
            resetDateTo = resetDateTo.AddDays(+1);
            dateto.Value = resetDateTo;
        }

        private void SetDeliveryOrdList()
        {

            String sql1 = "select top 1 vehNo, vehCode, driver from vhm Order By vehCode asc";//Top
            String sql2 = "select top 1 vehNo, vehCode, driver from vhm Order By vehCode desc";//Bottom
            DataSet ds1 = this.dbAccess.ReadSQLTemp("dt", sql1);

            DataSet ds2 = this.dbAccess.ReadSQLTemp("dt", sql2);

            txt_selDOnumFrom.Text = ds1.Tables[0].Rows[0][0].ToString();
            txt_selDOnumTo.Text = ds2.Tables[0].Rows[0][0].ToString();

        }

        #endregion

        #region Cancel Clicked

        //protected void btn_print_cancel_Click(object sender, System.EventArgs e)
        protected void btn_ok_cancel_Click(object sender, System.EventArgs e)
        {
            Form form_preview = BizXmlReader.CurrentInstance.GetForm("ARMpreview") as Form;
            form_preview.Close();
        }
        #endregion


        #region Intialise TextBoxes
        private void IntialiseTextboxes()
        {
            datefrom = BizXmlReader.CurrentInstance.GetControl("ARMpreview", "DelDateFrom") as DateTimePicker;
            dateto = BizXmlReader.CurrentInstance.GetControl("ARMpreview", "DelDateTo") as DateTimePicker;

            txt_selDOnumFrom = BizXmlReader.CurrentInstance.GetControl("ARMpreview", "selDOnumFrom") as TextBox;
            txt_selDOnumTo = BizXmlReader.CurrentInstance.GetControl("ARMpreview", "selDOnumTo") as TextBox;

            cmbGrouptype = BizXmlReader.CurrentInstance.GetControl("ARMpreview", "grouptype") as ComboBox;
        }
        #endregion


    }
}
