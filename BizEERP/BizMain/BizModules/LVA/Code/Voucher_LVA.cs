/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_LVA.cs
 *	Description:    Stock Adjustment Note
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 *
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

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

namespace ATL.LVA
{
	public class Voucher_LVA : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Global variables

		protected DBAccess dbaccess = null;
		protected decimal lva1_cosamt = 0;
		protected DialogResult ok = DialogResult.No;
        protected bool opened = false;
		bool reminder = false;

		protected string strcreatedby = null;

        private string formName;
        private string detailFormName = null;

        private Button Btn_Insert;
        private Button Btn_Delete;
        private Button Btn_Mark;
        private Button Btn_Generate;
        private Button Btn_Compute;
        private ComboBox lvah_lvayear;

		#endregion

		public Voucher_LVA(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_LVA.xml", moduleName, voucherBaseHelpers)
		{
		}

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "lvah.flag='LVA' AND lvah.SystemYear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (lvah.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " lvah.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " lvah.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND lvah.flag='LVA' AND lvah.SystemYear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

		#region DocumentPage Event

		protected override void AddDocumentPageEventTarget(object sender, PageEventArgs e)
		{
			base.AddDocumentPageEventTarget (sender, e);
			switch(e.PageName)
			{
                //case "header":
                //    e.EventTarget = new Header_LVA(e.DBAccess, e.FormsCollection, e.DocumentKey);
                //    break;
			}
		}

		#endregion

        #region Voucher New/Edit

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
        #endregion

        #region Document Cancel Click
        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);

            opened = false;
        }
        #endregion

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            opened = true;
            this.formName = (sender as Form).Name;
            this.detailFormName = (e.FormsCollection["header"] as Form).Name;

          

            DataTable LVAH = e.DBAccess.DataSet.Tables["lvah"];
            DataRow lvah = e.DBAccess.DataSet.Tables["lvah"].Rows[0];
            DataTable lva1 = e.DBAccess.DataSet.Tables["lva1"];
            //setDefaults(e.DBAccess.DataSet, "LVAH/lva1");

            string headerFormName = (e.FormsCollection["header"] as Form).Name;
            if (lvah["status"].ToString() == "N")
            {
                BizFunctions.GetTrandate(headerFormName, "lvah_trandate", lvah);
            }

            this.dbaccess = e.DBAccess;

            if (BizFunctions.IsEmpty(lvah["trandate"]))
            {
                lvah["trandate"] = DateTime.Now;
            }

            e.DBAccess.DataSet.Tables["lvah"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_LVAH_ColumnChanged);
            e.DBAccess.DataSet.Tables["lva1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_LVA1_ColumnChanged);

            Btn_Insert = BizXmlReader.CurrentInstance.GetControl(this.formName, Common.DEFAULT_DOCUMENT_BTNINSERT) as Button;
            //Btn_Insert.Enabled = true;
            Btn_Delete = BizXmlReader.CurrentInstance.GetControl(this.formName, Common.DEFAULT_DOCUMENT_BTNDELETE) as Button;
            //Btn_Delete.Enabled = true;
            Btn_Mark = BizXmlReader.CurrentInstance.GetControl(this.formName, Common.DEFAULT_DOCUMENT_BTNMARK) as Button;
            //Btn_Mark.Enabled = true;

            Btn_Generate = BizXmlReader.CurrentInstance.GetControl(this.formName, "Btn_Generate") as Button;
            Btn_Generate.Click += new EventHandler(Btn_Generate_Click);

            Btn_Compute = BizXmlReader.CurrentInstance.GetControl(this.formName, "Btn_Compute") as Button;
            Btn_Compute.Click += new EventHandler(Btn_Compute_Click);

            lvah_lvayear = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvah_lvayear") as ComboBox;
            setLvaYear();
        }

        void Btn_Compute_Click(object sender, EventArgs e)
        {
            DataRow lvah = this.dbaccess.DataSet.Tables["lvah"].Rows[0];
            DataTable lva1 = this.dbaccess.DataSet.Tables["lva1"];

     


            foreach (DataRow dr1 in lva1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (!BizFunctions.IsEmpty(dr1["datejoined"]))
                    {

                        if (BizFunctions.IsEmpty(dr1["actualleavecurryear"]))
                        {
                            dr1["actualleavecurryear"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["proposedaddon"]))
                        {
                            dr1["proposedaddon"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["proposedaddon"]))
                        {
                            dr1["proposedaddon"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actualaddon"]))
                        {
                            dr1["actualaddon"] = dr1["proposedaddon"];
                        }
                        if (BizFunctions.IsEmpty(dr1["actualbflyr"]))
                        {
                            dr1["actualbflyr"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["proposedbflyr"]))
                        {
                            dr1["proposedbflyr"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["actualbf"]))
                        {
                            dr1["actualbf"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["actualtotalal"]))
                        {
                            dr1["actualtotalal"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["totalal"]))
                        {
                            dr1["totalal"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["initialentitlement"]))
                        {
                            dr1["initialentitlement"] = 0;
                        }

                        if (Convert.ToDecimal(dr1["initialentitlement"]) > 0 && Convert.ToDecimal(dr1["actualleavecurryear"]) == 0)
                        {
                            dr1["actualleavecurryear"] = dr1["initialentitlement"];
                        }

                        if (Convert.ToDecimal(dr1["actualleavecurryear"]) > 0)
                        {
                            dr1["proposedleavcurryear"] = dr1["actualleavecurryear"];
                        }




                        /////


                        if (Convert.ToDecimal(dr1["actualaddon"]) == 0 && Convert.ToDecimal(dr1["proposedaddon"]) > 0)
                        {
                            dr1["actualaddon"] = dr1["proposedaddon"];
                        }


                        if (Convert.ToDecimal(dr1["actualbf"]) == 0 && Convert.ToDecimal(dr1["actualbflyr"]) > 0)
                        {
                            dr1["actualbf"] = dr1["actualbflyr"];
                        }

                         int yearsOfService = ATL.TimeUtilites.TimeTools.CalculateYears(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));

                         if (yearsOfService > 0)
                         {
                             dr1["monthsofservice"] = ATL.TimeUtilites.TimeTools.MonthDiff(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));
                             dr1["yearsofservice"] = ATL.TimeUtilites.TimeTools.CalculateYears(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));
                         }
                    }

                    string x = dr1["empnum"].ToString();
                    if (Convert.ToDecimal(dr1["yearsofservice"]) > 1)
                    {
                        decimal roundedYearsOfService = Convert.ToDecimal(Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])).Year - Convert.ToDateTime(dr1["datejoined"]).Year)*Convert.ToDecimal(1.00);
                        decimal exactYearsOfService = Convert.ToDecimal(dr1["monthsofservice"]) / 12;

                        if (exactYearsOfService >= roundedYearsOfService)
                        {
                            dr1["initialentitlement"] = GetLeaveEntitlement(dr1["hsgcode"].ToString().Trim(), "AL");
                            dr1["proposedleavcurryear"] = dr1["initialentitlement"];
                            dr1["actualleavecurryear"] = dr1["proposedleavcurryear"];
                            dr1["proposedaddon"] = Convert.ToDecimal(dr1["yearsofservice"]);
                        }
                        else
                        {
                                decimal difference = roundedYearsOfService - exactYearsOfService;
                                decimal finalDifference = 1 - difference;
                                dr1["initialentitlement"] = (GetLeaveEntitlement(dr1["hsgcode"].ToString().Trim(), "AL")) * Convert.ToDecimal(finalDifference);
                                dr1["proposedleavcurryear"] = dr1["initialentitlement"];
                                dr1["actualleavecurryear"] = dr1["proposedleavcurryear"];

                                dr1["proposedaddon"] = Convert.ToDecimal(dr1["yearsofservice"]) * Convert.ToDecimal(0.25);
                          

                            //if(Convert.ToDateTime(lvah["trandate"]).Month>=1 && Convert.ToDateTime(lvah["trandate"]).Month<=3)
                            //{
                            //    dr1["initialentitlement"] = (GetLeaveEntitlement(dr1["hsgcode"].ToString().Trim(), "AL"))*Convert.ToDecimal(0.25);
                            //    dr1["proposedleavcurryear"] = dr1["initialentitlement"];
                            //    dr1["actualleavecurryear"] = dr1["proposedleavcurryear"];

                            //    dr1["proposedaddon"] = Convert.ToDecimal(dr1["yearsofservice"]) * Convert.ToDecimal(0.25);
                            //}
                            // else if(Convert.ToDateTime(lvah["trandate"]).Month>3 && Convert.ToDateTime(lvah["trandate"]).Month<=6)
                            //{
                            //    dr1["initialentitlement"] = (GetLeaveEntitlement(dr1["hsgcode"].ToString().Trim(), "AL")) * Convert.ToDecimal(0.5);
                            //    dr1["proposedleavcurryear"] = dr1["initialentitlement"];
                            //    dr1["actualleavecurryear"] = dr1["proposedleavcurryear"];
                            //    dr1["proposedaddon"] = Convert.ToDecimal(dr1["yearsofservice"]) * Convert.ToDecimal(0.5); ;
                            //}
                            // else if(Convert.ToDateTime(lvah["trandate"]).Month>6 && Convert.ToDateTime(lvah["trandate"]).Month<=9)
                            //{
                            //    dr1["initialentitlement"] = (GetLeaveEntitlement(dr1["hsgcode"].ToString().Trim(), "AL")) * Convert.ToDecimal(0.75);
                            //    dr1["proposedleavcurryear"] = dr1["initialentitlement"];
                            //    dr1["actualleavecurryear"] = dr1["proposedleavcurryear"];
                            //    dr1["proposedaddon"] = Convert.ToDecimal(dr1["yearsofservice"]) * Convert.ToDecimal(0.75);
                            //}
                        
                        }
                      
                    }

                    //if (!BizFunctions.IsEmpty(lvah["lvatype"]))
                    //{
                    //    if (lvah["lvatype"].ToString().Trim().ToUpper() == "YEARLY OPENNING")
                    //    {
                    //        dr1["lvadjtype"] = "BF";
                    //        dr1["actualbflyr"] = GetYearBalance(dr1["empnum"].ToString(), -1);
                    //    }
                    //}
                    //else
                    //{
                    //    dr1["totalal"] = GetYearBalance(dr1["empnum"].ToString(), 0);
                    //}

                    if (dr1["lvadjtype"].ToString().Trim().ToUpper() == "BF")
                    {
                        decimal t = Convert.ToDecimal(dr1["actualbf"]) + Convert.ToDecimal(dr1["actualleavecurryear"]) + Convert.ToDecimal(dr1["actualaddon"]); ;
                        dr1["totalal"] = Decimal.Round(Convert.ToDecimal(dr1["actualbf"]) + Convert.ToDecimal(dr1["actualleavecurryear"]) + Convert.ToDecimal(dr1["actualaddon"]), MidpointRounding.ToEven);

                        if (Convert.ToDecimal(dr1["totalal"]) > 14)
                        {
                            dr1["totalal"] = 14;
                        }
                    }



                    if (Convert.ToDecimal(dr1["actualtotalal"]) == 0 && Convert.ToDecimal(dr1["totalal"]) > 0)
                    {
                        dr1["actualtotalal"] = Decimal.Round(Convert.ToDecimal(dr1["totalal"]),MidpointRounding.ToEven);

                        if (Convert.ToDecimal(dr1["actualtotalal"]) > 14)
                        {
                            dr1["actualtotalal"] = 14;
                        }
                    }

                    decimal totalal = Convert.ToDecimal(dr1["totalal"]); ;
                    decimal actualtotalal = Convert.ToDecimal(dr1["actualtotalal"]);
                    decimal adjbalqty = 0;

                    string test = dr1["lvadjtype"].ToString().Trim().ToUpper();
                    if (dr1["lvadjtype"].ToString().Trim().ToUpper() != "BF")
                    {
                        if (totalal > actualtotalal)
                        {
                            adjbalqty = (totalal - actualtotalal) * -1;
                        }
                        else if (totalal < actualtotalal)
                        {
                            adjbalqty = actualtotalal - totalal;
                        }
                        else
                        {
                            adjbalqty = 0;
                        }
                    }
                    else
                    {
                        adjbalqty = actualtotalal;
                    }


                    if (adjbalqty > 14)
                    {
                        adjbalqty = 14;
                    }
                    dr1["adjbalqty"] = adjbalqty;
                    decimal total = 0;
                    total = total + Convert.ToDecimal(dr1["totalal"]);
                    lvah["totalALqty"] = total;
                }
            }
        }


        private void setLvaYear()
        {
            DataRow lvah = this.dbaccess.DataSet.Tables["lvah"].Rows[0];
            int[] arr1 = new int[100];
            DateTime dt = (DateTime)lvah["trandate"];
            //     dt = (DateTime)lvah["trandate"];

            int Year = dt.AddYears(-5).Year;

            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = Year;
                Year = Year + 1;
            }

            lvah_lvayear.DataSource = arr1;
            if (!BizFunctions.IsEmpty(lvah["trandate"]))
            {              
                lvah["lvayear"] = Convert.ToDateTime(lvah["trandate"]).Year;                            
            }
            else
            {

                if (Convert.ToInt16(lvah["lvayear"]) == 0)
                {
                    lvah["lvayear"] = Convert.ToDateTime(lvah["trandate"]).Year;
                }

            }

        }

        void Btn_Generate_Click(object sender, EventArgs e)
        {
            DataRow lvah = this.dbaccess.DataSet.Tables["lvah"].Rows[0];
            DataTable lva1 = this.dbaccess.DataSet.Tables["LVA1"];

            string GetEmp = "";



            if (BizFunctions.IsEmpty(lvah["toempnum"]) && !BizFunctions.IsEmpty(lvah["fromempnum"]))
            {
                lvah["toempnum"] = lvah["fromempnum"];
            }

            if (BizFunctions.IsEmpty(lvah["daysperweek"]))
            {
                lvah["daysperweek"] = 0;
            }

            if (Convert.ToDecimal(lvah["daysperweek"]) > 0)
            {
                        if (!BizFunctions.IsEmpty(lvah["fromempnum"]) && !BizFunctions.IsEmpty(lvah["toempnum"]) && !BizFunctions.IsEmpty(lvah["sitenum"]) && !BizFunctions.IsEmpty(lvah["daysperweek"]))
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where empnum>='" + lvah["fromempnum"].ToString() + "' and empnum<='" + lvah["toempnum"].ToString() + "' and sitenum='" + lvah["sitenum"].ToString() + "' and daysperweek=" + lvah["daysperweek"].ToString() + " ";

                        }
                        else if (BizFunctions.IsEmpty(lvah["fromempnum"]) && BizFunctions.IsEmpty(lvah["toempnum"]) && !BizFunctions.IsEmpty(lvah["sitenum"]) && !BizFunctions.IsEmpty(lvah["daysperweek"]))
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where sitenum='" + lvah["sitenum"].ToString() + "' and daysperweek=" + lvah["daysperweek"].ToString() + " ";

                        }
                        else if (!BizFunctions.IsEmpty(lvah["fromempnum"]) && !BizFunctions.IsEmpty(lvah["toempnum"]) && BizFunctions.IsEmpty(lvah["sitenum"]) && !BizFunctions.IsEmpty(lvah["daysperweek"]))
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where empnum>='" + lvah["fromempnum"].ToString() + "' and empnum<='" + lvah["toempnum"].ToString() + "' and daysperweek=" + lvah["daysperweek"].ToString() + "  ";

                        }
                        else if (BizFunctions.IsEmpty(lvah["fromempnum"]) && BizFunctions.IsEmpty(lvah["toempnum"]) && BizFunctions.IsEmpty(lvah["sitenum"]) && !BizFunctions.IsEmpty(lvah["daysperweek"]))
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where daysperweek=" + lvah["daysperweek"].ToString() + "  ";

                        }
            }
            else
            {
                        if (!BizFunctions.IsEmpty(lvah["fromempnum"]) && !BizFunctions.IsEmpty(lvah["toempnum"]) && !BizFunctions.IsEmpty(lvah["sitenum"]) )
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where empnum>='" + lvah["fromempnum"].ToString() + "' and empnum<='" + lvah["toempnum"].ToString() + "' and sitenum='" + lvah["sitenum"].ToString() + "'  ";

                        }
                        else if (!BizFunctions.IsEmpty(lvah["fromempnum"]) && !BizFunctions.IsEmpty(lvah["toempnum"]) && BizFunctions.IsEmpty(lvah["sitenum"]) )
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where empnum>='" + lvah["fromempnum"].ToString() + "' and empnum<='" + lvah["toempnum"].ToString() + "'  ";

                        }
                 
                        else if (BizFunctions.IsEmpty(lvah["fromempnum"]) && BizFunctions.IsEmpty(lvah["toempnum"]) && !BizFunctions.IsEmpty(lvah["sitenum"]) )
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where sitenum='" + lvah["sitenum"].ToString() + "'  ";

                        }
                        else if (!BizFunctions.IsEmpty(lvah["fromempnum"]) && !BizFunctions.IsEmpty(lvah["toempnum"]) && BizFunctions.IsEmpty(lvah["sitenum"]) )
                        {
                            GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where empnum>='" + lvah["fromempnum"].ToString() + "' and empnum<='" + lvah["toempnum"].ToString() + "'   ";

                        }
                        else if (BizFunctions.IsEmpty(lvah["fromempnum"]) && BizFunctions.IsEmpty(lvah["toempnum"]) && BizFunctions.IsEmpty(lvah["sitenum"]))
                        {
                            //GetEmp = "Select empnum,empname,matnum,datejoined,hsgcode from HEMPH where [statuscode] like 'ACTI%'   ";
                            GetEmp = "select al.empnum,h.empname,h.matnum,h.datejoined,h.hsgcode from [ALBAL2015] al left join hemph h on al.empnum=h.empnum";

                        }
           
            }

    
           

          

           

           


           

            this.dbaccess.ReadSQL("TempEmpLists",GetEmp);

            DataTable TempEmpLists = this.dbaccess.DataSet.Tables["TempEmpLists"];
            if (lva1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(lva1);
            }
            foreach(DataRow dr1 in TempEmpLists.Rows)
            {
                if (!BizFunctions.IsEmpty(dr1["datejoined"]))
                {
                    int yearsOfService = ATL.TimeUtilites.TimeTools.CalculateYears(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));

                    if (yearsOfService > 0)
                    {
                        DataRow insertLva1 = lva1.NewRow();

                        insertLva1["empnum"] = dr1["empnum"];
                        insertLva1["empname"] = dr1["empname"];
                        insertLva1["matnum"] = dr1["matnum"];
                        insertLva1["hsgcode"] = dr1["hsgcode"];

                        if (!BizFunctions.IsEmpty(dr1["datejoined"]))
                        {
                            insertLva1["monthsofservice"] = ATL.TimeUtilites.TimeTools.MonthDiff(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));
                            insertLva1["yearsofservice"] = ATL.TimeUtilites.TimeTools.CalculateYears(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));
                        }

                        insertLva1["datejoined"] = dr1["datejoined"];

                        lva1.Rows.Add(insertLva1);
                    }
                }
            }


            foreach (DataRow dr1 in lva1.Rows)
            {
                if (dr1.RowState != DataRowState.Deleted)
                {
                    if (Convert.ToDecimal(dr1["yearsofservice"]) > 1)
                    {

                        dr1["initialentitlement"] = GetLeaveEntitlement(dr1["hsgcode"].ToString().Trim(), "AL");
                        //dr1["initialentitlement"] = 7;
                 

                        dr1["proposedaddon"] = Convert.ToDecimal(dr1["yearsofservice"]);
                    }

                    if (!BizFunctions.IsEmpty(lvah["lvatype"]))
                    {
                        if (lvah["lvatype"].ToString().Trim().ToUpper() == "YEARLY OPENNING")
                        {
                            dr1["lvadjtype"] = "BF";
                            dr1["actualbflyr"] = GetYearBalance(dr1["empnum"].ToString(), -1);
                        }
                    }
                    else
                    {
                        dr1["totalal"] = GetYearBalance(dr1["empnum"].ToString(), 0);                        
                    }
                }
            }




        }

        #endregion

        #region Document F2/F3

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);

            DataRow lvah = this.dbaccess.DataSet.Tables["lvah"].Rows[0];

            switch (e.ControlName)
            {
                //case "lvah_whnum":
                //    if (!BizFunctions.IsEmpty(lvah["sitenum"]))
                //    {
                //        e.DefaultCondition = " sitenum='" + lvah["sitenum"].ToString() + "' ";
                //    }
                //    else
                //    {
                //        e.DefaultCondition = "1=1";
                //    }
                //    break;
            }
        }

        #endregion

        #region Detail F2/F3

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);

            switch (e.MappingName)
            {
                case "matnum":
                    e.Condition = BizFunctions.F2Condition("matnum/matname", (sender as TextBox).Text);
                    break;
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);

            switch (e.MappingName)
            {
                case "matnum":
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];

                    e.CurrentRow["uom"] = e.F2CurrentRow["uomcode"];
                    break;
            }
        }

        #endregion

        #region ColumnChangedEvents

        #region lvah

        private void Voucher_LVAH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            DataRow lvah = dbaccess.DataSet.Tables["lvah"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "trandate":
                    #region set period
                    e.Row.BeginEdit();
                    //YushuEdited-23Jul2010-For WAC to group mwt data by period
                    //if ((bool)lvah["opbal"] != true)
                    //if (lvah["lvatype"].ToString() == "Stock Take")
                    //{
                        e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row[e.Column.ColumnName]);
                        //e.Row["pd"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row[e.Column.ColumnName]);
                    //}
                    //else if (lvah["lvatype"].ToString() == "Stock Adjustment")
                    //{
                    //    e.Row["period"] = 0;
                    //    e.Row["pd"] = 0;
                    //}
                    e.Row.EndEdit();
                    break;
                    #endregion

                //case "lvatype":
                //    #region set period

                //    #region Get DataGridView LVA1
                //    //Added By Yushu20100331 - Open Up different columns for different lva type selected

                //    DataGrid dgLVA1 = BizXmlReader.CurrentInstance.GetControl(detailFormName, "dg_Detail") as DataGrid;

                //    foreach (DataGridTableStyle dgts in dgLVA1.TableStyles)
                //    {
                //        foreach (DataGridColumnStyle dgcs in dgts.GridColumnStyles)
                //        {
                //            if (dgcs is BizDataGridTextBoxColumn)
                //            {
                //                BizDataGridTextBoxColumn bizcombo = null;
                //                bizcombo = dgcs as BizDataGridTextBoxColumn;

                //                #region Stock Adjustment
                //               if (lvah["lvatype"].ToString() == "Stock Adjustment")
                //                {
                //                    //e.Row["period"] = 0;
                //                    //e.Row["pd"] = 0;
                //                   //YushuEdited-23Jul2010-For WAC to group mwt data by period
                //                    e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)lvah["trandate"]);
                //                    //e.Row["pd"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)lvah["trandate"]);


                //                    if (bizcombo.MappingName == "phyqty")
                //                    {
                //                        bizcombo.TextBoxGrid.Enabled = false;
                //                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.LightGray;
                //                    }

                //                    if (bizcombo.MappingName == "qty")
                //                    {
                //                        bizcombo.TextBoxGrid.Enabled = true;
                //                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.White;
                //                    }
                //                }
                //                #endregion

                //                #region Stock Take
                //                else if (lvah["lvatype"].ToString() == "Stock Take")
                //                {
                //                    e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)lvah["trandate"]);
                //                    e.Row["pd"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)lvah["trandate"]);

                //                    if (bizcombo.MappingName == "qty")
                //                    {
                //                        bizcombo.TextBoxGrid.Enabled = false;
                //                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.LightGray;
                //                    }

                //                    if (bizcombo.MappingName == "phyqty")
                //                    {
                //                        bizcombo.TextBoxGrid.Enabled = true;
                //                        bizcombo.TextBoxGrid.BackColor = System.Drawing.Color.White;
                //                    }

                //                }
                //                #endregion
                //            }
                //        }
                //    }

                //    #endregion

                //    break;

                //    #endregion

                #region opbal-Commented
                //case "opbal":
                //    #region set period

                //    if ((bool)lvah["opbal"] != true)
                //    {
                //        e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)lvah["trandate"]);
                //    }
                //    else
                //    {
                //        e.Row["period"] = 0;
                //    }
                //    break;
                //    #endregion
                #endregion
            }
        }

        #endregion

        #region lva1

        private void Voucher_LVA1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow lvah = this.dbaccess.DataSet.Tables["lvah"].Rows[0];
            switch (e.Column.ColumnName)
            {
                //case "matnum":
                //    #region uom

                //    if (!BizFunctions.IsEmpty(e.Row["matnum"]))
                //    {
                //        string sql = "Select matname,uomcode from matm where matnum='" + e.Row["matnum"].ToString() + "' and isnull(status,'')<>'V'";
                //        this.dbaccess.ReadSQL("tmpMATM", sql);
                //        DataTable tmpMATM = this.dbaccess.DataSet.Tables["tmpMATM"];
                //        foreach (DataRow drMATM in tmpMATM.Rows)
                //        {
                //            if (drMATM.RowState != DataRowState.Deleted)
                //            {
                //                e.Row["matname"] = drMATM["matname"];
                //                e.Row["uom"] = drMATM["uomcode"];
                //            }
                //        }
                //    }

                //    #endregion
                //    break;
            }
        }

        #endregion

        #endregion

        #region Refresh

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

            #region initialise values

            // Initialise the accounting defaults.
            lva1_cosamt = 0;
            this.ok = DialogResult.OK;
            DataRow lvah = e.DBAccess.DataSet.Tables["lvah"].Rows[0];
            DataTable LVAH = dbaccess.DataSet.Tables["lvah"];
            DataTable lva1 = e.DBAccess.DataSet.Tables["lva1"];

            #endregion

            #region lvah

            lvah["trandate"] = BizFunctions.GetStandardDateString(Convert.ToDateTime(lvah["trandate"]));
            //lvah["stkdate"] = BizFunctions.GetStandardDateString(Convert.ToDateTime(lvah["stkdate"]));
            //if (BizFunctions.IsEmpty(lvah["stkdate"]))
            //{
            //    lvah["stkdate"] = lvah["trandate"];
            //}
            setDefaults(dbaccess.DataSet, "LVAH/lva1");

            #endregion

            if (lvah["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP) return;

            #region lva1

            foreach (DataRow drLVA1 in lva1.Rows)
            {
                if (drLVA1.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(lvah, drLVA1);
                    //#region Update matname & uom if empty
                    //if (BizFunctions.IsEmpty(drLVA1["matname"]) || BizFunctions.IsEmpty(drLVA1["uom"]))
                    //{
                    //    if (!BizFunctions.IsEmpty(drLVA1["matnum"]))
                    //    {
                    //        string sqlUOM = "Select matname,uomcode from matm where matnum='" + drLVA1["matnum"].ToString() + "' and isnull(status,'')<>'V'";
                    //        this.dbaccess.ReadSQL("tmpMATM", sqlUOM);
                    //        DataTable tmpUOM = this.dbaccess.DataSet.Tables["tmpMATM"];
                    //        foreach (DataRow drUOM in tmpUOM.Rows)
                    //        {
                    //            if (drUOM.RowState != DataRowState.Deleted)
                    //            {
                    //                if (BizFunctions.IsEmpty(drLVA1["matname"]))
                    //                {
                    //                    drLVA1["matname"] = drUOM["matname"].ToString();
                    //                }
                    //                if (BizFunctions.IsEmpty(drLVA1["uom"]))
                    //                {
                    //                    drLVA1["uom"] = drUOM["uomcode"].ToString();
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //#endregion
                }
            }

            #endregion

            //#region UpdateDataRow lva1

            //foreach (DataRow dr in lva1.Rows)
            //{
            //    if (dr.RowState != DataRowState.Deleted)
            //    {
            //        BizFunctions.UpdateDataRow(lvah, dr);
            //    }
            //}

            //#endregion

            //foreach (DataRow dr1 in TempEmpLists.Rows)
            //{
            //    if (!BizFunctions.IsEmpty(dr1["datejoined"]))
            //    {
            //        int yearsOfService = ATL.TimeUtilites.TimeTools.CalculateYears(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));

            //        if (yearsOfService > 0)
            //        {
            //            DataRow insertLva1 = lva1.NewRow();

            //            insertLva1["empnum"] = dr1["empnum"];
            //            insertLva1["empname"] = dr1["empname"];
            //            insertLva1["matnum"] = dr1["matnum"];
            //            insertLva1["hsgcode"] = dr1["hsgcode"];

            //            if (!BizFunctions.IsEmpty(dr1["datejoined"]))
            //            {
            //                insertLva1["monthsofservice"] = ATL.TimeUtilites.TimeTools.MonthDiff(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));
            //                insertLva1["yearsofservice"] = ATL.TimeUtilites.TimeTools.CalculateYears(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));
            //            }

            //            insertLva1["datejoined"] = dr1["datejoined"];

            //            lva1.Rows.Add(insertLva1);
            //        }
            //    }
            //}


            //foreach (DataRow dr1 in lva1.Rows)
            //{
            //    if (dr1.RowState != DataRowState.Deleted)
            //    {
            //        dr1["monthsofservice"] = ATL.TimeUtilites.TimeTools.MonthDiff(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));
            //        dr1["yearsofservice"] = ATL.TimeUtilites.TimeTools.CalculateYears(Convert.ToDateTime(dr1["datejoined"]), Convert.ToDateTime(Convert.ToDateTime(lvah["trandate"])));

            //        if (Convert.ToDecimal(dr1["yearsofservice"]) > 1)
            //        {

            //            dr1["initialentitlement"] = GetLeaveEntitlement(dr1["hsgcode"].ToString().Trim(), "AL");
            //            //dr1["initialentitlement"] = 7;


            //            dr1["proposedaddon"] = Convert.ToDecimal(dr1["yearsofservice"]);
            //        }

            //        if (!BizFunctions.IsEmpty(lvah["lvatype"]))
            //        {
            //            if (lvah["lvatype"].ToString().Trim().ToUpper() == "YEARLY OPENNING")
            //            {
            //                dr1["lvadjtype"] = "BF";
            //                dr1["actualbflyr"] = GetYearBalance(dr1["empnum"].ToString(), -1);
            //            }
            //        }
            //        else
            //        {
            //            dr1["totalal"] = GetYearBalance(dr1["empnum"].ToString(), 0);
            //        }
            //    }
            //}

            Btn_Insert.Enabled = true;
            Btn_Delete.Enabled = true;
            Btn_Mark.Enabled = true;
        }

        #endregion

		#region Document Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle (sender, e);

			DataRow lvah = e.DBAccess.DataSet.Tables["lvah"].Rows[0];
            DataTable lva1 = e.DBAccess.DataSet.Tables["lva1"];

            lvah["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

            if (lva1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in lva1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        dr1["aladjdate"] = lvah["trandate"];

                        if (BizFunctions.IsEmpty(dr1["actualleavecurryear"]))
                        {
                            dr1["actualleavecurryear"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["proposedaddon"]))
                        {
                            dr1["proposedaddon"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["proposedaddon"]))
                        {
                            dr1["proposedaddon"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["actualaddon"]))
                        {
                            dr1["actualaddon"] = dr1["proposedaddon"];
                        }
                        if (BizFunctions.IsEmpty(dr1["actualbflyr"]))
                        {
                            dr1["actualbflyr"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["proposedbflyr"]))
                        {
                            dr1["proposedbflyr"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["actualbf"]))
                        {
                            dr1["actualbf"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["actualtotalal"]))
                        {
                            dr1["actualtotalal"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["totalal"]))
                        {
                            dr1["totalal"] = 0;
                        }

                        if (BizFunctions.IsEmpty(dr1["initialentitlement"]))
                        {
                            dr1["initialentitlement"] = 0;
                        }

                        if (Convert.ToDecimal(dr1["initialentitlement"]) > 0 && Convert.ToDecimal(dr1["actualleavecurryear"])==0)
                        {
                            dr1["actualleavecurryear"] = dr1["initialentitlement"];
                        }

                        if (Convert.ToDecimal(dr1["actualleavecurryear"]) > 0)
                        {
                            dr1["proposedleavcurryear"] = dr1["actualleavecurryear"];
                        }
                        



                        /////


                        if (Convert.ToDecimal(dr1["actualaddon"]) == 0 && Convert.ToDecimal(dr1["proposedaddon"]) > 0)
                        {
                            dr1["actualaddon"] = dr1["proposedaddon"];
                        }


                        if (Convert.ToDecimal(dr1["actualbf"]) == 0 && Convert.ToDecimal(dr1["actualbflyr"]) > 0)
                        {
                            dr1["actualbf"] = dr1["actualbflyr"];
                        }

                   

                        if (dr1["lvadjtype"].ToString().Trim().ToUpper() == "BF")
                        {
                            dr1["totalal"] = Convert.ToDecimal(dr1["actualbflyr"]) + Convert.ToDecimal(dr1["actualleavecurryear"]) + Convert.ToDecimal(dr1["actualaddon"]);

                            if (Convert.ToDecimal(dr1["totalal"]) > 14)
                            {
                                dr1["totalal"] = 14;
                            }
                        }
                     
                        

                        if (Convert.ToDecimal(dr1["actualtotalal"]) == 0 && Convert.ToDecimal(dr1["totalal"]) > 0)
                        {
                            dr1["actualtotalal"] = dr1["totalal"];

                            if (Convert.ToDecimal(dr1["actualtotalal"]) > 14)
                            {
                                dr1["actualtotalal"] = 14;
                            }
                        }

                        decimal totalal = Convert.ToDecimal(dr1["totalal"]); ;
                        decimal actualtotalal = Convert.ToDecimal(dr1["actualtotalal"]);
                        decimal adjbalqty = 0;

                        if (dr1["lvadjtype"].ToString().Trim().ToUpper() != "BF")
                        {
                            if (totalal > actualtotalal)
                            {
                                adjbalqty = (totalal - actualtotalal) * -1;
                            }
                            else if (totalal < actualtotalal)
                            {
                                adjbalqty = actualtotalal - totalal;
                            }
                            else
                            {
                                adjbalqty = 0;
                            }
                        }
                        else
                        {
                            adjbalqty = actualtotalal;
                        }
       

                        if (adjbalqty > 14)
                        {
                            adjbalqty = 14;
                        }
                        dr1["adjbalqty"] = adjbalqty;
                        decimal total = 0;
                        total = total + Convert.ToDecimal(dr1["totalal"]);
                        lvah["totalALqty"] = total;

                        
                    }
                }
            }

            //lvah.BeginEdit();
            //lvah["SystemYear"] = Common.DEFAULT_SYSTEM_YEAR.ToString();
            //lvah["period"] = BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(lvah["trandate"]));
            //lvah["pd"] = BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(lvah["trandate"]));

            //if (lvah["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP) return;

            //Document_Refresh_OnClick(sender, new DocumentEventArgs(e.PageName, e.DBAccess, e.DocumentKey, e.FormsCollection, e.CurrentUser, e.TableYear, e.CurrentRow));

            //if (BizValidate.ChkPeriodLocked(e.DBAccess, lvah["period"].ToString()) || BizValidate.ChkPeriodLocked(e.DBAccess, lvah["pd"].ToString()))
            //{
            //    MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //    return;
            //}

            //if (BizFunctions.IsEmpty(lvah["sitenum"]))
            //{
            //    MessageBox.Show("Please select Site No. !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //    return;
            //}

            //if (BizFunctions.IsEmpty(lvah["whnum"]))
            //{
            //    MessageBox.Show("Please select Warehouse !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //    return;
            //}
		}

		#endregion

        #region SaveBegin

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            //DataTable dtTmp = null;
            //string sqlCommand = null;
            DataRow lvah = e.DBAccess.DataSet.Tables["lvah"].Rows[0];
            DataTable lva1 = e.DBAccess.DataSet.Tables["lva1"];
            DataTable lwt = e.DBAccess.DataSet.Tables["lwt"];
            //DataTable mwt = e.DBAccess.DataSet.Tables["mwt"];


            if (lvah["flag"].ToString() == "LVA")
            {
                if (lvah["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
                {
                    #region stock costing adjustment update lwt

                    foreach (DataRow dr in lva1.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            if ((decimal)dr["adjbalqty"] != 0)
                            {
                                DataRow lwt_dr = lwt.Rows.Add(new object[] { });

                 
                                lwt_dr["lvadjtype"] = dr["lvadjtype"];
                                lwt_dr["lvmnum"] = "AL";
                                lwt_dr["empnum"] = dr["empnum"];
                                lwt_dr["empname"] = dr["empname"];
                                lwt_dr["matnum"] = dr["matnum"];
                                lwt_dr["datejoined"] = dr["datejoined"];
                                lwt_dr["yearsofservice"] = dr["yearsofservice"];
                                lwt_dr["monthsofservice"] = dr["monthsofservice"];
                                lwt_dr["albf"] = dr["albf"];
                                lwt_dr["algiven"] = dr["algiven"];
                                lwt_dr["alcurr"] = dr["alcurr"];
                                lwt_dr["ladjust"] = dr["ladjust"];
                                lwt_dr["initialentitlement"] = dr["initialentitlement"];
                                lwt_dr["proposedleavcurryear"] = dr["proposedleavcurryear"];
                                lwt_dr["proposedaddon"] = dr["proposedaddon"];
                                lwt_dr["actualaddon"] = dr["actualaddon"];
                                lwt_dr["actualbflyr"] = dr["actualbflyr"];
                                lwt_dr["proposedbflyr"] = dr["proposedbflyr"];
                                lwt_dr["actualbf"] = dr["actualbf"];
                                lwt_dr["totalal"] = dr["totalal"];
                                lwt_dr["aladjdate"] = dr["aladjdate"];
                                lwt_dr["remark"] = dr["remark"];
                                lwt_dr["refnum"] = dr["refnum"];
                                lwt_dr["totalal"] = dr["totalal"];
                                lwt_dr["adjbalqty"] = dr["adjbalqty"];
                                lwt_dr["actualtotalal"] = dr["actualtotalal"];
                                lwt_dr["currlveqty"] = dr["currlveqty"];

                                lwt_dr["guid"] = BizLogicTools.Tools.getGUID();
                            }
                        }
                    }
                    #endregion
                }
            }
            //else
            //{
                //if (lvah["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
                //{
                //    foreach (DataRow dr in lva1.Rows)
                //    {
                //        if (dr["empnum"] != System.DBNull.Value || dr["empnum"].ToString() != String.Empty)
                //        {
                //            if (dr.RowState != DataRowState.Deleted)
                //            {
                //                if ((decimal)dr["qty"] != 0)
                //                {

                //                    #region assigning rows to local lwt
                //                    for (int i = 0; i <= 1; i++)
                //                    {
                //                        if (i == 0)
                //                        {
                //                            DataRow dr_lwt = lwt.NewRow();
                //                            dr_lwt["refnum"] = dr["refnum"];
                //                            dr_lwt["empnum"] = dr["empnum"];
                //                            dr_lwt["matnum"] = dr["matnum"];
                //                            dr_lwt["empname"] = dr["empname"];
                //                            dr_lwt["docunum"] = dr["refnum"];
                //                            dr_lwt["location"] = lvah["fromsitenum"];
                //                            dr_lwt["qty"] = -(decimal)dr["qty"];
                //                            dr_lwt["uom"] = dr["uom"];
                //                            dr_lwt["whnum"] = lvah["whnumfrm"];//Decrease qty in main warehouse,here is the fromsitenum(local) if is outlets
                //                            dr_lwt["guid"] = BizLogicTools.Tools.getGUID();
                //                            dr_lwt["trandate"] = dr["trandate"];
                //                            dr_lwt["created"] = dr["created"];
                //                            dr_lwt["modified"] = dr["modified"];
                //                            dr_lwt["status"] = dr["status"].ToString().Trim();
                //                            dr_lwt["user"] = dr["user"].ToString().Trim();
                //                            dr_lwt["barcode"] = dr["barcode"].ToString();
                //                            dr_lwt["flag"] = lvah["flag"];
                //                            lwt.Rows.Add(dr_lwt);
                //                        }
                //                        if (i == 1)
                //                        {
                //                            DataRow dr_lwt = lwt.NewRow();
                //                            dr_lwt["refnum"] = dr["refnum"];
                //                            dr_lwt["barcode"] = dr["barcode"];
                //                            dr_lwt["matnum"] = dr["matnum"];
                //                            dr_lwt["detail"] = dr["detail"];
                //                            dr_lwt["docunum"] = dr["refnum"];
                //                            dr_lwt["location"] = lvah["tositenum"];
                //                            dr_lwt["qty"] = (decimal)dr["qty"];
                //                            dr_lwt["uom"] = dr["uom"];
                //                            dr_lwt["whnum"] = lvah["whnumto"];//increase qty in Stock In Transit warehouse(Location HQ)
                //                            dr_lwt["guid"] = BizLogicTools.Tools.getGUID();
                //                            dr_lwt["trandate"] = dr["trandate"];
                //                            dr_lwt["created"] = dr["created"];
                //                            dr_lwt["modified"] = dr["modified"];
                //                            dr_lwt["status"] = dr["status"].ToString().Trim();
                //                            dr_lwt["user"] = dr["user"].ToString().Trim();
                //                            dr_lwt["barcode"] = dr["barcode"].ToString();
                //                            dr_lwt["flag"] = lvah["flag"];
                //                            lwt.Rows.Add(dr_lwt);
                //                        }
                //                    }
                //                    #endregion
                //                }
                //            }
                //        }
                //    }

                //}
            //}

            foreach (DataRow dr in lva1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {

                    BizFunctions.UpdateDataRow(lvah, dr, "refnum/user/flag/status/created/modified/SystemYear");
                }
            }

            //if (lvah["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSP)
            //{
            //    #region stock costing adjustment update MWT

            //    foreach (DataRow dr in lva1.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {
            //            if ((decimal)dr["qty"] != 0)
            //            {
            //                DataRow mwt_dr = mwt.Rows.Add(new object[] { });

            //                mwt_dr["location"] = lvah["sitenum"].ToString();
            //                mwt_dr["whnum"] = lvah["whnum"].ToString();

            //                BizFunctions.UpdateDataRow(lvah, mwt_dr);

            //                if (lvah["lvatype"].ToString() == "Opening Balance")
            //                {
            //                    lvah["opbal"] = 1;
            //                    BizFunctions.UpdateDataRow(lvah, mwt_dr, "opbal");
            //                }
            //                else
            //                {
            //                    lvah["opbal"] = 0;
            //                    BizFunctions.UpdateDataRow(lvah, mwt_dr, "opbal");
            //                }

            //                BizFunctions.UpdateDataRow(dr, mwt_dr, "coy/stkdate");
            //                BizFunctions.UpdateDataRow(dr, mwt_dr, "matnum/uom/stdcost/cosamt");

            //                mwt_dr["detail"] = dr["matname"].ToString();
            //                mwt_dr["year"] = lvah["systemyear"].ToString();
            //                mwt_dr["created"] = lvah["created"].ToString();
            //                mwt_dr["modified"] = lvah["modified"].ToString();
            //                mwt_dr["docunum"] = lvah["refnum"].ToString();
            //                mwt_dr["pd"] = lvah["pd"].ToString();
            //                mwt_dr["qty"] = Convert.ToInt32(dr["qty"]);
            //                mwt_dr["guid"] = HomeFix.BizLogicTools.Tools.getGUID();
            //            }
            //        }
            //    }
            //    #endregion
            //}
        }

        #endregion

        #region Reopen

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);

			DataRow lvah = dbaccess.DataSet.Tables["lvah"].Rows[0];

            //if (BizValidate.ChkPeriodLocked(e.DBAccess, lvah["period"].ToString()) || BizValidate.ChkPeriodLocked(e.DBAccess, lvah["pd"].ToString()))
            //{
            //    MessageBox.Show("You are not allowed to reopen this voucher !", "Period has been Closed !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //}
        }

        #endregion

        #region Preview Handle

        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);

            DataRow lvah = this.dbaccess.DataSet.Tables["lvah"].Rows[0];

            // If allow print even after confirm by checking the status
            if (lvah["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "lvah/lva1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }

        #endregion

        #region Print OnClick

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

            //DataRow lvah=this.dbaccess.DataSet.Tables["lvah"].Rows[0];
            //Hashtable selectedCollection = new Hashtable();

            //selectedCollection.Add("MATM", "SELECT m.matnum,m.matname,m.uomcode FROM MATM m left join LVA1 v on m.matnum=v.matnum where v.refnum='" + lvah["refnum"].ToString() + "'");

            //e.DBAccess.ReadSQL(selectedCollection);
            //e.DataSource = e.DBAccess.DataSet;
        }

        #endregion

        #region Private Functions

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
								case "System.DateTime":
									if (dr[dc.ColumnName] == System.DBNull.Value)
										dr[dc.ColumnName] = DateTime.Now;
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

        //private void calculatePhyQty()
        //{
        //    DataTable lva1 = this.dbaccess.DataSet.Tables["lva1"];

        //    foreach (DataRow drV1 in lva1.Rows)
        //    {
        //        if (drV1.RowState != DataRowState.Deleted)
        //        {
        //            drV1["phyqty"] = BizFunctions.Round(Convert.ToDecimal(drV1["qty"]) + Convert.ToDecimal(drV1["bkqty"]), 4);
        //        }
        //    }
        //}

        //private void calculateAdjQty()
        //{
        //    DataTable lva1 = this.dbaccess.DataSet.Tables["lva1"];

        //    foreach (DataRow drV1 in lva1.Rows)
        //    {
        //        if (drV1.RowState != DataRowState.Deleted)
        //        {
        //            drV1["qty"] = BizFunctions.Round(Convert.ToDecimal(drV1["phyqty"]) - Convert.ToDecimal(drV1["bkqty"]), 4);
        //        }
        //    }
        //}

        //private void calculateTotalQty()
        //{
        //    DataTable lva1 = this.dbaccess.DataSet.Tables["lva1"];
        //    DataRow lvah=this.dbaccess.DataSet.Tables["lvah"].Rows[0];

        //    //decimal totalbkqty = Convert.ToDecimal(lvah["totalbkqty"]);
        //    //decimal totalphyqty = Convert.ToDecimal(lvah["totalphyqty"]);
        //    //decimal totalqty = Convert.ToDecimal(lvah["totalqty"]);
        //    decimal totalbkqty = 0;
        //    decimal totalphyqty = 0;
        //    decimal totalqty = 0;

        //    foreach (DataRow drV1 in lva1.Rows)
        //    {
        //        if (drV1.RowState != DataRowState.Deleted)
        //        {
        //            if (BizFunctions.IsEmpty(drV1["bkqty"]))
        //            {
        //                drV1["bkqty"] = 0;
        //            }
        //            if (BizFunctions.IsEmpty(drV1["phyqty"]))
        //            {
        //                drV1["phyqty"] = 0;
        //            }
        //            if (BizFunctions.IsEmpty(drV1["qty"]))
        //            {
        //                drV1["qty"] = 0;
        //            }
        //            totalbkqty += Convert.ToDecimal(drV1["bkqty"]);
        //            totalphyqty += Convert.ToDecimal(drV1["phyqty"]);
        //            totalqty += Convert.ToDecimal(drV1["qty"]);
        //        }
        //    }

        //    lvah["totalbkqty"] = totalbkqty;
        //    lvah["totalphyqty"] = totalphyqty;
        //    lvah["totalqty"] = totalqty;
        //}

        #endregion

        private decimal GetYearBalance(string empnum,int year)
        {
            decimal BalQty = 0;


            string GetBal = "select ISNULL(SUM(ISNULL(adjbalqty,0)),0) as totaladjbalqty from lwt" + Convert.ToString(Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR) + year) + " where empnum='" + empnum + "' and lvmnum='AL'";

            this.dbaccess.ReadSQL("TempBalQty", GetBal);

            DataTable TempBalQty = this.dbaccess.DataSet.Tables["TempBalQty"];

            if (TempBalQty != null)
            {
                if (TempBalQty.Rows.Count > 0)
                {
                    if (!BizFunctions.IsEmpty(TempBalQty.Rows[0]["totaladjbalqty"]))
                    {
                        BalQty = Convert.ToDecimal(TempBalQty.Rows[0]["totaladjbalqty"]);
                    }
                }
            }

            return BalQty;
        }


        private decimal GetLeaveEntitlement(string hsgcode, string leavecode)
        {
            decimal totaldays = 0;

            string get1 = "Select * from lve1 where lvenum='" + hsgcode + "' and lvmnum='" + leavecode + "'";

            this.dbaccess.ReadSQL("TmpLve1", get1);

            DataTable dt = this.dbaccess.DataSet.Tables["TmpLve1"];

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (BizFunctions.IsEmpty(dt.Rows[0]["noOfdays"]))
                    {
                        dt.Rows[0]["noOfdays"] = 0;
                    }
                    totaldays = Convert.ToDecimal(dt.Rows[0]["noOfdays"]);
                }
            }

            return totaldays;
        }
    }
}
