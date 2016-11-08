/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_FAVT.cs
 *	Description:   Journal Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Jer				2006-08-04			Add paste_handle, paste_onclick to enable/disable header columnchanged event
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Configuration;
using System.ComponentModel;
using System.Drawing;


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
using DEMO.MDT;
using ATL.GeneralTools;
using ATL.FilterOP;
using ATL.MultiColumnComboBox;

namespace ATL.FAVT
{
    public class Voucher_FAVT : BizRAD.BizApplication.VoucherBaseHelper
    {
        #region global variables
        protected DBAccess dbaccess = null;
        string formdDetailName;

        //FOR DETAILS
        protected GroupBox grpHeaderVoucherInfo;
        protected GroupBox grpHeaderTransInfo;

        protected DataGrid dgDetail;

        protected TextBox txtVouchernum;
        protected TextBox txtDesc;
        protected ColumnComboBox cboApname;
        protected ColumnComboBox cboArname;
        protected ColumnComboBox cboAccname;
        protected DateTimePicker dtTrandate;
        protected DateTimePicker dtInvdate;
        protected TextBox txtAccnum;
        protected TextBox txtAccname;
        protected TextBox txtArname;
        protected TextBox txtApname;
        protected TextBox txtChequeno;
        protected TextBox txtOridebit;
        protected TextBox txtOricredit;
        protected ComboBox cboOricur;
        protected Button btnAdd;
        protected Button btnUpdate;
        protected Button btnInsertTrans;
        protected Button btnNextTrans;
        protected Label txtCurrentAction;
        protected Button btnGetOB;


        protected bool skipValidate;

        protected Button btnInsert;
        protected Button btnDelete;
        protected Button btnUp;
        protected Button btnDown;
        protected Button btnMark;
        protected Button btnDuplicate;
        protected Button btnExtract;
        protected Button btnClose;

        protected bool opened = false;
        protected bool isPaste = false;

        protected int TabDetail;

        public string documentKey = null;
        public string vouchernum = "";
        public string lastYear = "";

        GenTools genFunctions = new GenTools();
        getFilterOP FilterOP = new getFilterOP();

        bool blnNew = true;

        #endregion

        public Voucher_FAVT(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_FAVT.xml", moduleName, voucherBaseHelpers)
        {
        }

        #region Steph - To stop users from accessing more than one voucher from Sales Receipts at the same time
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

        #region Voucher Default/ALL

        protected override void AddVoucherDefaultCondition(BizRAD.BizVoucher.VoucherConditionEventArgs e)
        {
            base.AddVoucherDefaultCondition(e);
            e.Condition = "flag='FAVT' AND status = 'O'";
        }

        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {
            base.AddVoucherAllCondition(e);
            e.Condition = "flag='FAVT'";
        }

        protected override void Voucher_Form_OnLoad(object sender, VoucherEventArgs e)
        {
            base.Voucher_Form_OnLoad(sender, e);
        }
        #endregion


        #region Paste Handle
        protected override void Document_Paste_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Paste_Handle(sender, e);

        }

        protected override void Document_Paste_OnClick(object sender, BizRAD.BizDocument.DocumentEventArgs e)
        {
            base.Document_Paste_OnClick(sender, e);
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];

            isPaste = true;

            //if (favt1.Rows.Count > 0)
            //{
            //    string test = Convert.ToDateTime(favth["trandate"]).ToShortDateString();
            //    foreach (DataRow dr1 in favt1.Rows)
            //    {
            //        if (dr1.RowState != DataRowState.Deleted)
            //        {
            //            dr1["vouchernum"] = dr1["vouchernum"].ToString() + "." + BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(favth["trandate"])).ToString();
            //        }
            //    }
            //}


        }
        #endregion

        protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
        {
            base.Document_TabControl_OnSelectionChanged(sender, e);
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];

            btnInsert = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Insert") as Button;
            btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
            btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
            btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
            btnClose = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Close") as Button;

            btnInsert.Enabled = false;
            btnDelete.Enabled = true;
            btnMark.Enabled = true;
            btnDuplicate.Enabled = false;

        }

        protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_TabControl_Handle(sender, e);

            TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
        }

        #region Document Event

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);

            opened = true;

            dbaccess = e.DBAccess;
            this.formdDetailName = (e.FormsCollection["header"] as Form).Name;
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            documentKey = e.DocumentKey;
            skipValidate = false;



            lastYear = Convert.ToString(GetNumeric(Common.DEFAULT_SYSTEM_YEAR) - 1);

            favth["aropen"] = false;
            favth["apopen"] = false;
            favth["cshopen"] = false;
            favth["glopen"] = false;

            InitializeControls();

            #region Steph - Set the current status of users action
            txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";
            #endregion

            grpHeaderVoucherInfo.Enabled = true;
            grpHeaderTransInfo.Enabled = false;

            //txtAccname.Text = "";
            //cboAccname.SelectedIndex = -1; 
            txtOridebit.Text = "0.00";
            txtOricredit.Text = "0.00";

            MakeEnterEvent();
            MakeLostFocusEvent();

            InitialComboAccnum();
            InitialComboArnum();
            InitialComboApnum();
            //cboAccname["accname"]= getAccName(cboAccname["accnum"].ToString());
            //cboApname.SelectedValue = sivh["apnum"].ToString().Trim();
            //cboAccname.SelectedIndexChanged += new EventHandler(cboAccname_SelectedIndexChanged);

            //LoadComboBox();

            cboOricur.Text = "SGD";

            cboAccname.SelectedIndexChanged += new System.EventHandler(this.AccnumChanged_Combo);
            cboArname.SelectedIndexChanged += new EventHandler(cboArname_SelectedIndexChanged);
            cboApname.SelectedIndexChanged += new EventHandler(cboApname_SelectedIndexChanged);


            //if (cboAccname.SelectedIndex != -1)
            //{
            //    txtAccname.Text = cboAccname.SelectedValue.ToString();
            //}

            MakeF3DblClickEventsOnGrid();
            calcTotalDebitCredit();
            setOricur();
            btnUpdate.Enabled = false;

            btnAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            btnInsertTrans.Click += new System.EventHandler(this.cmdInsert_Click);
            btnNextTrans.Click += new System.EventHandler(this.cmdNextTrans_Click);
            btnUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            btnGetOB.Click += new EventHandler(this.cmdGetOB_Click);

            foreach (DataRow dr in favt1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(favth, dr, "user/flag/status/created/modified");
                }
            }
        }

        void cboApname_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtApname.Text = "";
            if (cboApname.Text != "")
            {
                txtApname.Text = cboApname["apnum"].ToString().Trim();
            }
        }

        void cboArname_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtArname.Text = "";
            if (cboArname.Text != "")
            {
                txtArname.Text = cboArname["arnum"].ToString().Trim();
            }
        }



        void txtOricredit_LostFocus(object sender, EventArgs e)
        {
            try
            {
                decimal getCreditValue = Convert.ToDecimal(txtOricredit.Text);
                if (txtOricredit.Text != "")
                {
                    txtOricredit.Text = string.Format("{0:0.00}", getCreditValue);
                }
            }
            catch
            {
                MessageBox.Show("Invalid value keyed in Credit textfield! Please check");
                txtOricredit.Focus();
                return;
            }

        }

        void txtOridebit_LostFocus(object sender, EventArgs e)
        {
            try
            {
                decimal getDebitValue = Convert.ToDecimal(txtOridebit.Text);
                if (txtOridebit.Text != "")
                {
                    txtOridebit.Text = string.Format("{0:0.00}", getDebitValue);
                }
            }
            catch
            {
                MessageBox.Show("Invalid value keyed in Debit textfield! Please check");
                txtOridebit.Focus();
                return;
            }
        }

        #endregion

        #region Save & Confirm
        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];

            #region Check if each voucher(vouchernum) posting is balance.
            string strVoucherBalance = "select * from (select sum(oridebit) as oridebit,sum(oricredit) as oricredit from favt1 where refnum = '" + favth["refnum"].ToString().Trim() + "') a where oridebit<>oricredit";
            DataTable VoucherBalance = BizFunctions.ExecuteQuery(dbaccess.DataSet, strVoucherBalance);

            string CheckList = "";
            if (VoucherBalance.Rows.Count > 0)
            {
                CheckList = "Please check the entries! This journal is not balance!";
            }

            if (VoucherBalance.Rows.Count > 0)
            {
                MessageBox.Show(CheckList);
            }

            #endregion
        }

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

            #region Steph - Void as Journal Template requires no posting
            //DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            //DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            //DataTable ard = dbaccess.DataSet.Tables["ard"];
            //DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
            //DataTable apd = dbaccess.DataSet.Tables["apd"];
            //DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
            //DataTable csh = dbaccess.DataSet.Tables["csh"];
            //DataTable gld = dbaccess.DataSet.Tables["gld"];

            //#region Steph - to post to individual subledgers and GL
            //if (favth["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            //{
            //    #region Steph - To write in dataset -VOID

            //    //string cmd2 = "SELECT * FROM BFAVT1" + Common.DEFAULT_SYSTEM_YEAR + " WHERE 1=2";
            //    //DataSet dstemp = e.DBAccess.ReadSQLTemp("favt1", cmd2);

            //    //    dbaccess.ReadSQL("bArd", "SELECT * FROM favt1" + Common.DEFAULT_SYSTEM_YEAR + " where refnum = '" + favth["refnum"].ToString().Trim() + "' AND accnum in (select accnum from acm where acctype = '7')");
            //    //    DataTable bArd = dbaccess.DataSet.Tables["bArd"];

            //    #region Loop PIV2 to get data
            //    //for (int j = 0; j < dbaccess.DataSet.Tables["bArd"].Rows.Count; j++)
            //    //{
            //    //    DataRow dr_favt1 = e.DBAccess.DataSet.Tables["ard"].NewRow();

            //    //    for (int i = 0; i < dstemp.Tables["bArd"].Columns.Count; i++)
            //    //    {
            //    //        if (dstemp.Tables["bArd"].Columns[i].ColumnName != "ID")
            //    //        {
            //    //            if (dbaccess.DataSet.Tables["bArd"].Rows[j].RowState != DataRowState.Deleted)
            //    //            {
            //    //                if (dbaccess.DataSet.Tables["ard"].Columns.IndexOf(dstemp.Tables["bArd"].Columns[i].ColumnName) != -1)
            //    //                {
            //    //                    dr_favt1[dstemp.Tables["bArd"].Columns[i].ColumnName] = dbaccess.DataSet.Tables["bArd"].Rows[j][dstemp.Tables["bArd"].Columns[i].ColumnName];
            //    //                }
            //    //            }
            //    //        }
            //    //    }
            //    //    //dr_favt1["tab"] = "Inv";
            //    //    e.DBAccess.DataSet.Tables["ard"].Rows.Add(dr_favt1);
            //    //}
            //    #endregion
            //    #endregion

            //    #region Steph - Saving into ARD
            //    dbaccess.ReadSQL("acm", "select * from acm");
            //    string saveArd = "select refnum,vouchernum,trandate,chknum,accnum,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period from [favt1] where refnum = '" + favth["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '7') GROUP BY refnum,vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,oricur,exrate";
            //    //string dd = "select * from [favt1] where refnum = '" + favth["refnum"].ToString().Trim() + "'";
            //    DataTable getArd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveArd);

            //    foreach (DataRow dr in getArd.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {

            //            DataRow addArd = ard.Rows.Add(new object[] { });
            //            addArd["refnum"] = dr["refnum"];
            //            addArd["docunum"] = dr["vouchernum"];
            //            addArd["trandate"] = dr["trandate"];
            //            addArd["chknum"] = dr["chknum"];
            //            addArd["accnum"] = dr["accnum"];
            //            addArd["detail"] = dr["detail"];
            //            addArd["oridebit"] = dr["oridebit"];
            //            addArd["oricredit"] = dr["oricredit"];
            //            addArd["arnum"] = dr["arnum"];
            //            addArd["invnum"] = dr["invnum"];
            //            addArd["oldref"] = dr["oldref"];
            //            addArd["invdate"] = dr["invdate"];
            //            addArd["lgr"] = "ARD";
            //            addArd["coy"] = "SAF";
            //            addArd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
            //            addArd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
            //            addArd["period"] = dr["period"];
            //            addArd["oricur"] = dr["oricur"];
            //            addArd["exrate"] = dr["exrate"];
            //            addArd["gstamt"] = 0;
            //            addArd["exramt"] = 0;
            //            addArd["oriamt"] = addArd["doriamt"];
            //            addArd["postamt"] = addArd["dpostamt"];
            //        }
            //    }

            //    foreach (DataRow dr2 in ard.Rows)
            //    {
            //        if (dr2.RowState != DataRowState.Deleted)
            //        {
            //            BizFunctions.UpdateDataRow(favth, dr2, "refnum/user/coy/flag/status/created/modified");
            //        }
            //    }

            //    //dbaccess.SetID("ard", "ard" + Common.DEFAULT_SYSTEM_YEAR);
            //    //dbaccess.Update("ard", "ard" + Common.DEFAULT_SYSTEM_YEAR);
            //    #endregion

            //    #region Steph - Saving into SIV
            //    string saveSiv = "select vouchernum,trandate,chknum,accnum,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period from [favt1] where refnum = '" + favth["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '0' OR acctype = '1') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,oricur,exrate";

            //    DataTable getSiv = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveSiv);
            //    //DataTable getSiv = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select vouchernum,trandate,chknum,accnum,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,exrate,MAX(period) as period from favt1Siv GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,oricur,exrate");

            //    foreach (DataRow dr in getSiv.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {

            //            DataRow addSiv = siv1.Rows.Add(new object[] { });
            //            addSiv["docunum"] = dr["vouchernum"];
            //            addSiv["trandate"] = dr["trandate"];
            //            addSiv["accnum"] = dr["accnum"];
            //            addSiv["detail"] = dr["detail"];
            //            addSiv["oridebit"] = dr["oridebit"];
            //            addSiv["oricredit"] = dr["oricredit"];
            //            addSiv["arnum"] = dr["arnum"];
            //            addSiv["invnum"] = dr["invnum"];
            //            addSiv["invdate"] = dr["invdate"];
            //            addSiv["coy"] = "SAF";
            //            addSiv["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
            //            addSiv["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
            //            addSiv["period"] = dr["period"];
            //            addSiv["oricur"] = dr["oricur"];
            //            addSiv["exrate"] = dr["exrate"];
            //            addSiv["gstamt"] = 0;
            //            addSiv["gstper"] = 0;
            //            addSiv["oriamt"] = addSiv["doriamt"];
            //            addSiv["postamt"] = addSiv["dpostamt"];
            //        }
            //    }

            //    foreach (DataRow dr2 in siv1.Rows)
            //    {
            //        if (dr2.RowState != DataRowState.Deleted)
            //        {
            //            BizFunctions.UpdateDataRow(favth, dr2, "refnum/user/flag/status/created/modified");
            //        }
            //    }

            //    #endregion

            //    #region Steph - Saving into APD
            //    string saveApd = "select vouchernum,trandate,chknum,accnum,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period from favt1 where refnum = '" + favth["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '12') GROUP BY vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,oricur,exrate";

            //    DataTable getApd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveApd);

            //    foreach (DataRow dr in getApd.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {

            //            DataRow addApd = apd.Rows.Add(new object[] { });
            //            addApd["docunum"] = dr["vouchernum"];
            //            addApd["trandate"] = dr["trandate"];
            //            addApd["chknum"] = dr["chknum"];
            //            addApd["accnum"] = dr["accnum"];
            //            addApd["detail"] = dr["detail"];
            //            addApd["oridebit"] = dr["oridebit"];
            //            addApd["oricredit"] = dr["oricredit"];
            //            addApd["apnum"] = dr["apnum"];
            //            addApd["invnum"] = dr["invnum"];
            //            addApd["oldref"] = dr["oldref"];
            //            addApd["invdate"] = dr["invdate"];
            //            addApd["lgr"] = "APD";
            //            addApd["coy"] = "SAF";
            //            addApd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
            //            addApd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
            //            addApd["period"] = dr["period"];
            //            addApd["oricur"] = dr["oricur"];
            //            addApd["exrate"] = dr["exrate"];
            //            addApd["gstamt"] = 0;
            //            addApd["exramt"] = 0;
            //            addApd["oriamt"] = addApd["doriamt"];
            //            addApd["postamt"] = addApd["dpostamt"];
            //        }
            //    }

            //    foreach (DataRow dr2 in apd.Rows)
            //    {
            //        if (dr2.RowState != DataRowState.Deleted)
            //        {
            //            BizFunctions.UpdateDataRow(favth, dr2, "refnum/user/coy/flag/status/created/modified");
            //        }
            //    }
            //    #endregion

            //    #region Steph - Saving into PIV
            //    string savePiv = "select vouchernum,trandate,chknum,accnum,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from favt1 where refnum = '" + favth["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '2' OR acctype = '3') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,oricur,exrate";

            //    DataTable getPiv = BizFunctions.ExecuteQuery(dbaccess.DataSet, savePiv);

            //    foreach (DataRow dr in getPiv.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {

            //            DataRow addPiv = piv1.Rows.Add(new object[] { });
            //            addPiv["docunum"] = dr["vouchernum"];
            //            addPiv["trandate"] = dr["trandate"];
            //            addPiv["chknum"] = dr["chknum"];
            //            addPiv["accnum"] = dr["accnum"];
            //            addPiv["detail"] = dr["detail"];
            //            addPiv["oridebit"] = dr["oridebit"];
            //            addPiv["oricredit"] = dr["oricredit"];
            //            addPiv["apnum"] = dr["apnum"];
            //            addPiv["invnum"] = dr["invnum"];
            //            addPiv["invdate"] = dr["invdate"];
            //            addPiv["lgr"] = "PIV";
            //            addPiv["coy"] = "SAF";
            //            addPiv["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
            //            addPiv["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
            //            addPiv["period"] = dr["period"];
            //            addPiv["oricur"] = dr["oricur"];
            //            addPiv["exrate"] = dr["exrate"];
            //            addPiv["gstamt"] = 0;
            //            addPiv["gstper"] = 0;
            //            addPiv["oriamt"] = addPiv["doriamt"];
            //            addPiv["postamt"] = addPiv["dpostamt"];
            //        }
            //    }

            //    foreach (DataRow dr2 in piv1.Rows)
            //    {
            //        if (dr2.RowState != DataRowState.Deleted)
            //        {
            //            BizFunctions.UpdateDataRow(favth, dr2, "refnum/user/flag/status/created/modified");
            //        }
            //    }
            //    #endregion

            //    #region Steph - Saving into Csh
            //    string saveCsh = "select vouchernum,trandate,chknum,accnum,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from [favt1] where refnum = '" + favth["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '5' OR acctype = '6') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,oricur,exrate";

            //    DataTable getCsh = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveCsh);

            //    foreach (DataRow dr in getCsh.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {
            //            DataRow addCsh = csh.Rows.Add(new object[] { });
            //            addCsh["docunum"] = dr["vouchernum"];
            //            addCsh["trandate"] = dr["trandate"];
            //            addCsh["chknum"] = dr["chknum"];
            //            addCsh["accnum"] = dr["accnum"];
            //            addCsh["detail"] = dr["detail"];
            //            addCsh["oridebit"] = dr["oridebit"];
            //            addCsh["oricredit"] = dr["oricredit"];
            //            addCsh["apnum"] = dr["apnum"];
            //            addCsh["lgr"] = "CSH";
            //            addCsh["coy"] = "SAF";
            //            addCsh["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
            //            addCsh["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
            //            addCsh["period"] = dr["period"];
            //            addCsh["oricur"] = dr["oricur"];
            //            addCsh["exrate"] = dr["exrate"];
            //        }
            //    }

            //    foreach (DataRow dr2 in csh.Rows)
            //    {
            //        if (dr2.RowState != DataRowState.Deleted)
            //        {
            //            BizFunctions.UpdateDataRow(favth, dr2, "refnum/user/flag/status/created/modified");
            //        }
            //    }
            //    #endregion

            //    BizAccounts.PostGLD(dbaccess, "csh/apd/piv1/siv1/ard", "favth", "");

            //    #region Steph - Saving into the entries which are not from any of these sub ledgers ARD, APD, PIV, CSH
            //    string saveGld = "select vouchernum,trandate,chknum,accnum,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period from [favt1] where refnum = '" + favth["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype in ('4','8','9','10','11','13','14')) GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,oricur,exrate";

            //    DataTable getGld = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveGld);

            //    foreach (DataRow dr in getGld.Rows)
            //    {
            //        if (dr.RowState != DataRowState.Deleted)
            //        {

            //            DataRow addGld = gld.Rows.Add(new object[] { });
            //            addGld["docunum"] = dr["vouchernum"];
            //            addGld["trandate"] = dr["trandate"];
            //            //addGld["chknum"] = dr["chknum"];
            //            addGld["accnum"] = dr["accnum"];
            //            addGld["detail"] = dr["detail"];
            //            addGld["oridebit"] = dr["oridebit"];
            //            addGld["oricredit"] = dr["oricredit"];
            //            addGld["arnum"] = dr["arnum"];
            //            //addGld["invnum"] = dr["invnum"];
            //            //addGld["invdate"] = dr["invdate"];
            //            addGld["lgr"] = "GLD";
            //            addGld["coy"] = "SAF";
            //            addGld["period"] = dr["period"];
            //            addGld["oricur"] = dr["oricur"];
            //            addGld["exrate"] = dr["exrate"];
            //            addGld["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
            //            addGld["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
            //        }
            //    }

            //    foreach (DataRow dr2 in gld.Rows)
            //    {
            //        if (dr2.RowState != DataRowState.Deleted)
            //        {
            //            BizFunctions.UpdateDataRow(favth, dr2, "refnum/user/coy/flag/status/created/modified");
            //        }
            //    }
            //    #endregion
            //}
            //#endregion
            #endregion
        }

        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            if ((decimal)favth["debit"] != (decimal)favth["credit"])
            {
                MessageBox.Show("The Voucher Posting Is Not Balance!", "Voucher Is Not Confirmed!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handle = false;
            }
        }

        #endregion

        #region Refresh

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            DataTable apd = dbaccess.DataSet.Tables["apd"];
            DataTable ard = dbaccess.DataSet.Tables["ard"];
            DataTable csh = dbaccess.DataSet.Tables["csh"];
            DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
            DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
            DataTable acm = dbaccess.DataSet.Tables["acm"];
            DataTable gld = dbaccess.DataSet.Tables["gld"];

            if (favth["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
            {
                txtCurrentAction.Text = "Voucher is closed!";
                return;
            }

            favth["period"] = BizAccounts.GetPeriod(dbaccess, (DateTime)favth["trandate"]);
            favth["coy"] = "SAF";

            refreshBfavt1();

            if (BizFunctions.IsEmpty(favth["ispaste"]))
            {
                favth["ispaste"] = 0;
            }

            if ((bool)favth["ispaste"])
            {
                if (favt1.Rows.Count > 0)
                {
                   
                    foreach (DataRow dr1 in favt1.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            if(dr1["vouchernum"].ToString().Contains("."))
                            {
                                int position1 = dr1["vouchernum"].ToString().IndexOf('.');

                      

                                dr1["vouchernum"] = dr1["vouchernum"].ToString().Substring(0, position1);
                                dr1["vouchernum"] = dr1["vouchernum"].ToString() + "." + BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(dtTrandate.Text)).ToString();
                            }
                            else
                            {
                                dr1["vouchernum"] = dr1["vouchernum"].ToString() + "." + BizAccounts.GetPeriod(this.dbaccess, Convert.ToDateTime(dtTrandate.Text)).ToString();
                            }
                        }
                    }
                }

              
            }

        }
        #endregion

        #endregion

        #region Steph - Events for Batch Payment Entry into detail

        private void InitializeControls()
        {
            grpHeaderVoucherInfo = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "grp_VoucherInfo") as GroupBox;
            grpHeaderTransInfo = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "grp_TransactionInfo") as GroupBox;

            dgDetail = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "dg_detail") as DataGrid;

            txtVouchernum = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_vouchernum") as TextBox;
            txtDesc = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_desc") as TextBox;
            cboArname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_arname") as ColumnComboBox;
            cboApname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_apname") as ColumnComboBox;
            cboAccname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_accname") as ColumnComboBox;
            txtAccname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_accname") as TextBox;
            txtArname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_arname") as TextBox;
            txtApname = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_apname") as TextBox;
            txtChequeno = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_chequeno") as TextBox;
            dtTrandate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_trandate") as DateTimePicker;
            dtInvdate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_invdate") as DateTimePicker;
            txtAccnum = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_accnum") as TextBox;
            txtOridebit = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_oridebit") as TextBox;
            txtOricredit = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_oricredit") as TextBox;
            cboOricur = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_oricur") as ComboBox;
            txtCurrentAction = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_currentAction") as Label;

            btnAdd = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Add") as Button;
            btnGetOB = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_getOB") as Button;
            btnUpdate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Update") as Button;
            btnInsertTrans = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Insert") as Button;
            btnNextTrans = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_NextTrans") as Button;
        }


        private void setOricur()
        {
            if (cboOricur.Text.ToString().Trim() == "")
            {
                cboOricur.Text = "SGD";
            }
        }

        private void calcTotalDebitCredit()
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];

            setDefaults(dbaccess.DataSet, "favt1");

            decimal countTotalDebit = 0;
            decimal countTotalCredit = 0;
            foreach (DataRow dr in favt1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    countTotalDebit += (decimal)dr["oridebit"];
                    countTotalCredit += (decimal)dr["oricredit"];
                }
            }

            favth["debit"] = countTotalDebit;
            favth["credit"] = countTotalCredit;

        }

        private void MakeEnterEvent()
        {
            foreach (Control crlControl in grpHeaderVoucherInfo.Controls)
            {
                crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
                crlControl.Validating += new CancelEventHandler(crlControl_Validating);
            }

            foreach (Control crlControl in grpHeaderTransInfo.Controls)
            {
                crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
            }

        }

        void crlControl_Validating(object sender, CancelEventArgs e)
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            string strExistVoucher = "select vouchernum from favt1 where refnum = '" + favth["refnum"].ToString().Trim() + "'";
            DataTable existVoucher = BizFunctions.ExecuteQuery(dbaccess.DataSet, strExistVoucher);
            bool checkOnce = true;
            foreach (DataRow dr in existVoucher.Rows)
            {
                if (checkOnce == true)
                {
                    if (skipValidate == false)
                        if (txtVouchernum.Text.Trim() == dr["vouchernum"].ToString().Trim())
                        {
                            MessageBox.Show("This Journal No. exist in this voucher! Please proceed to key in the Transactions Info!");
                            #region Steph - Copied from F11 in order to get the values for all the field in Voucher Info for Existing Data
                            vouchernum = txtVouchernum.Text.ToString();
                            txtVouchernum.Focus();
                            DataTable editHeader = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select vouchernum,apname,arname,detail,trandate,chknum,invdate from favt1 where vouchernum = '" + vouchernum + "'");
                            if (editHeader.Rows.Count > 0)
                            {
                                txtVouchernum.Text = editHeader.Rows[0]["vouchernum"].ToString();
                                cboApname.Text = editHeader.Rows[0]["apname"].ToString();
                                cboArname.Text = editHeader.Rows[0]["arname"].ToString();
                                txtDesc.Text = editHeader.Rows[0]["detail"].ToString();
                                dtTrandate.Text = editHeader.Rows[0]["trandate"].ToString();
                                dtInvdate.Text = editHeader.Rows[0]["invdate"].ToString();
                                txtChequeno.Text = editHeader.Rows[0]["chknum"].ToString();
                            }
                            #endregion
                            add_Click();
                            checkOnce = false;
                        }
                }
            }
            skipValidate = true;

        }

        private void SendTabForEnter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            if (e.KeyCode == Keys.Enter)
            {
                //    dbaccess.ReadSQL("VouchernumExist", "SELECT refnum,vouchernum from favt1 where refnum != '" + favth["refnum"].ToString().Trim() + "' AND vouchernum = '" + txtVouchernum.Text.ToString().Trim() + "'");
                //if (dbaccess.DataSet.Tables["VouchernumExist"].Rows.Count > 0)
                //{
                //    MessageBox.Show("Please check that this Voucher No. has been used in " + dbaccess.DataSet.Tables["VouchernumExist"].Rows[0]["refnum"].ToString().Trim());
                //}

                if (txtVouchernum.Text.ToString().Trim() == String.Empty)
                {
                    MessageBox.Show("Voucher No. Cannot Be Empty!");
                }
                if (txtVouchernum.Text.ToString().Trim() != String.Empty)
                {
                    SendKeys.Send("{Tab}");
                }
            }

            if (e.KeyCode == Keys.F5)
            {
                btnUpdate.Focus();
                SendKeys.Send("{Enter}");
            }

            if (e.KeyCode == Keys.F8)
            {
                btnNextTrans.Focus();
                SendKeys.Send("{Enter}");
            }

            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];

            if (e.KeyCode == Keys.F11)
            {
                F11Event();
            }

            if (e.KeyCode == Keys.F12)
            {
                F12Event();
            }

        }

        private void F12Event()
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];

            if (vouchernum != "")
            {
                foreach (DataRow dr in favt1.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        if (dr["vouchernum"].ToString() == vouchernum)
                        {
                            dr["vouchernum"] = txtVouchernum.Text;
                            dr["trandate"] = dtTrandate.Value;
                            dr["invdate"] = dtInvdate.Value;
                            dr["apnum"] = cboApname["apnum"];
                            dr["apname"] = cboApname.Text;
                            dr["arnum"] = cboArname["arnum"];
                            dr["arname"] = cboArname["arname"];
                            dr["detail"] = txtDesc.Text;
                            dr["chknum"] = txtChequeno.Text;
                        }
                    }
                }
                txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";
                skipValidate = false;
            }

            dgDetail.Enabled = true;
            grpHeaderVoucherInfo.Enabled = true;
            grpHeaderTransInfo.Enabled = false;
            btnAdd.Enabled = true;

            ClearTransaction();
            txtVouchernum.Focus();
        }

        private void F11Event()
        {
            if (txtVouchernum.Text.ToString().Trim() != "")
            {
                vouchernum = txtVouchernum.Text.ToString();
                txtVouchernum.Focus();
                DataTable editHeader = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select vouchernum,apname,arname,detail,trandate,chknum,invdate from favt1 where vouchernum = '" + vouchernum + "'");
                if (editHeader.Rows.Count > 0)
                {
                    txtVouchernum.Text = editHeader.Rows[0]["vouchernum"].ToString();
                    cboApname.Text = editHeader.Rows[0]["apname"].ToString();
                    cboArname.Text = editHeader.Rows[0]["arname"].ToString();
                    txtDesc.Text = editHeader.Rows[0]["detail"].ToString();
                    dtTrandate.Text = editHeader.Rows[0]["trandate"].ToString();
                    dtInvdate.Text = editHeader.Rows[0]["invdate"].ToString();
                    txtChequeno.Text = editHeader.Rows[0]["chknum"].ToString();

                    txtCurrentAction.Text = "Updating Voucher Info, press F12 after updating is done!";

                    //System.Drawing.Color.RoyalBlue
                    dgDetail.Enabled = false;
                    grpHeaderTransInfo.Enabled = false;
                    grpHeaderVoucherInfo.Enabled = true;

                    btnAdd.Enabled = false;
                    btnNextTrans.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnInsertTrans.Enabled = false;
                    blnNew = false;
                    skipValidate = true;
                }
                else
                {
                    MessageBox.Show("The Voucher No. keyed in is not found! Please check!");
                }
            }
        }
        private void cmdNextTrans_Click(object sender, EventArgs e)
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];

            grpHeaderVoucherInfo.Enabled = true;
            grpHeaderTransInfo.Enabled = false;

            txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";

            ClearTransaction();
            txtVouchernum.Focus();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {

            add_Click();
        }

        private void cmdGetOB_Click(object sender, EventArgs e)
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];



            if ((bool)favth["aropen"] == true)
            {
                FilterOP.ShowDialog();
                getAROpening();

            }
            if ((bool)favth["apopen"] == true)
            {
                FilterOP.ShowDialog();
                getAPOpening();

            }
            if ((bool)favth["cshopen"] == true)
            {
                FilterOP.ShowDialog();
                getCSHOpening();
            }
            if ((bool)favth["glopen"] == true)
            {
                FilterOP.ShowDialog();
                getGLOpening();
            }

        }

        private void add_Click()
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];

            //if (txtChequeno.Text.ToString().Trim() == "")
            //{
            //    MessageBox.Show("Cheque No. Cannot Be Empty!");
            //    txtChequeno.Focus();

            //}
            if (txtVouchernum.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Voucher No. Cannot Be Empty!");
                txtVouchernum.Focus();
            }

            if (txtVouchernum.Text.ToString().Trim() != "")
            {
                grpHeaderVoucherInfo.Enabled = false;
                grpHeaderTransInfo.Enabled = true;

                setOricur();

                txtCurrentAction.Text = "OPEN For Transactions Info Data Entry";
                btnInsertTrans.Enabled = true;
                btnNextTrans.Enabled = true;


                calcTotalDebitCredit();

                cboAccname.Focus();
            }

            //if (cboApname.Text != "")
            //{
            //    favth["apnum"] = cboApname.SelectedValue.ToString();
            //}
            //if (cboArname.Text != "")
            //{
            //    favth["arnum"] = cboArname.SelectedValue.ToString();
            //}
        }


        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];

            dgDetail.Enabled = true;


            if (blnNew)
            {
                DataRow addRow = favt1.Rows.Add(new object[] { });
                addRow["line"] = intGetLine();
                addRow["vouchernum"] = txtVouchernum.Text;
                addRow["detail"] = txtDesc.Text;
                addRow["apnum"] = cboApname["apnum"];
                addRow["apname"] = cboApname.Text;
                addRow["arnum"] = cboArname["arnum"];
                addRow["arname"] = cboArname["arname"];
                addRow["trandate"] = dtTrandate.Value;
                addRow["accnum"] = cboAccname["accnum"];
                addRow["accname"] = cboAccname["accname"];
                addRow["chknum"] = txtChequeno.Text;
                addRow["oridebit"] = GetNumeric(txtOridebit.Text);
                addRow["oricredit"] = GetNumeric(txtOricredit.Text);
                addRow["oricur"] = cboOricur.Text;
                addRow["invnum"] = txtVouchernum.Text;
                addRow["invdate"] = dtInvdate.Value;
            }
            else
            {
                int intRow = dgDetail.CurrentRowIndex;

                dgDetail[intRow, 1] = txtVouchernum.Text;
                dgDetail[intRow, 2] = dtTrandate.Value;
                if (cboAccname.Text != "")
                {
                    dgDetail[intRow, 3] = cboAccname["accnum"];
                    dgDetail[intRow, 4] = cboAccname["accname"];
                }
                dgDetail[intRow, 5] = GetNumeric(txtOridebit.Text);
                dgDetail[intRow, 6] = GetNumeric(txtOricredit.Text);
                dgDetail[intRow, 7] = txtDesc.Text;
                if (cboApname.Text != "")
                {
                    dgDetail[intRow, 8] = cboApname["apnum"];
                    dgDetail[intRow, 9] = cboApname["apname"];
                }
                if (cboArname.Text != "")
                {
                    dgDetail[intRow, 10] = cboArname["arnum"];
                    dgDetail[intRow, 11] = cboArname["arname"];
                }
                dgDetail[intRow, 12] = txtVouchernum.Text;
                dgDetail[intRow, 13] = dtInvdate.Value;
                dgDetail[intRow, 14] = txtChequeno.Text;
                dgDetail[intRow, 15] = cboOricur.Text;


            }

            skipValidate = false;

            calcTotalDebitCredit();
            ClearTransaction();
            txtVouchernum.Focus();
            grpHeaderVoucherInfo.Enabled = true;
            grpHeaderTransInfo.Enabled = false;

            txtVouchernum.Enabled = true;
            cboApname.Enabled = true;
            cboArname.Enabled = true;
            txtChequeno.Enabled = true;
            dtTrandate.Enabled = true;
            txtDesc.Enabled = true;
            dtInvdate.Enabled = true;


            btnUpdate.Enabled = false;
            btnAdd.Enabled = true;
            blnNew = true;
            dgDetail.Enabled = true;

            txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";

        }
        private void cmdInsert_Click(object sender, EventArgs e)
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];

            setOricur();

            dgDetail.Enabled = true;


            if (blnNew)
            {
                DataRow addRow = favt1.Rows.Add(new object[] { });
                addRow["line"] = intGetLine();
                addRow["vouchernum"] = txtVouchernum.Text;
                addRow["detail"] = txtDesc.Text;
                addRow["apnum"] = cboApname["apnum"];
                addRow["apname"] = cboApname["apname"];
                addRow["arnum"] = cboArname["arnum"];
                addRow["arname"] = cboArname["arname"];
                addRow["trandate"] = dtTrandate.Value;
                addRow["accnum"] = cboAccname["accnum"];
                addRow["accname"] = cboAccname["accname"];
                addRow["chknum"] = txtChequeno.Text;
                addRow["oridebit"] = GetNumeric(txtOridebit.Text);
                addRow["oricredit"] = GetNumeric(txtOricredit.Text);
                addRow["oricur"] = cboOricur.Text;
                addRow["invnum"] = txtVouchernum.Text;
                addRow["invdate"] = dtInvdate.Value;

            }
            else
            {
                int intRow = dgDetail.CurrentRowIndex;

                dgDetail[intRow, 1] = txtVouchernum.Text;
                dgDetail[intRow, 2] = dtTrandate.Value;
                dgDetail[intRow, 3] = cboAccname["accnum"];
                dgDetail[intRow, 4] = cboAccname["accname"];
                dgDetail[intRow, 5] = GetNumeric(txtOridebit.Text);
                dgDetail[intRow, 6] = GetNumeric(txtOricredit.Text);
                dgDetail[intRow, 7] = txtDesc.Text;
                dgDetail[intRow, 8] = cboApname["apnum"];
                dgDetail[intRow, 9] = cboApname["apname"];
                dgDetail[intRow, 10] = cboArname["arnum"];
                dgDetail[intRow, 11] = cboArname["arname"];
                dgDetail[intRow, 12] = txtVouchernum.Text;
                dgDetail[intRow, 13] = dtInvdate.Value;
                dgDetail[intRow, 14] = txtChequeno.Text;
                dgDetail[intRow, 15] = cboOricur.Text;
            }
            calcTotalDebitCredit();
            ClearEntry();
            cboAccname.Focus();
            blnNew = true;
            dgDetail.Enabled = true;


        }
        #endregion

        private int intGetLine()
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];

            int intValue = 0;

            foreach (DataRow dr in favt1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                    intValue = intValue + 1;
            }

            return intValue;
        }


        private decimal GetNumeric(object Numeric)
        {
            try
            {
                return decimal.Parse(Numeric.ToString());
            }
            catch
            {
                return 0;
            }
        }


        private void ClearTransaction()
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            txtVouchernum.Text = "";
            txtChequeno.Text = "";
            txtDesc.Text = "";
            cboApname.Text = "";
            cboArname.Text = "";
            cboAccname.Text = "";
            txtAccname.Text = "";
            txtArname.Text = "";
            txtApname.Text = "";
            txtOridebit.Text = "0.00";
            txtOricredit.Text = "0.00";

            blnNew = true;
        }

        private void ClearEntry()
        {
            cboAccname["accnum"] = "";
            txtAccname.Text = "";
            txtOridebit.Text = "0.00";
            txtOricredit.Text = "0.00";
            blnNew = true;
        }

        private void Addrow_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int intRow;
                DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
                intRow = dgDetail.CurrentRowIndex + 1;
                if (dgDetail.CurrentRowIndex == getTotalRows() - 1)
                {
                    DataRow addRow = favt1.Rows.Add(new object[] { });
                    addRow["deliverydate"] = dbaccess.DataSet.Tables["favth"].Rows[0]["deliverydate"];
                }
                dgDetail.CurrentCell = new DataGridCell(intRow, 1);
            }
        }

        private int getTotalRows()
        {
            int intRow = 0;
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];
            foreach (DataRow dr in favt1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    intRow = intRow + 1;
                }
            }
            return intRow;
        }

        //private void DetailData_Validating(object sender, CancelEventArgs e)
        //{
        //    if (sender.GetType().Name.ToString() == "ComboBox")
        //    {
        //        ComboBox cboSender = (ComboBox)sender;
        //        if (cboSender.Name.ToString() == "cbo_apname")
        //        {
        //            if (cboSender.SelectedIndex == -1)
        //            {
        //                if (cboSender.Text.Trim() == "") return;
        //                MessageBox.Show("Invalid Selected Item!", "System Message");
        //                e.Cancel = true;
        //                return;
        //            }
        //            else
        //            {
        //                string strSQL = "SELECT * FROM APM WHERE APNUM='" + cboSender.SelectedValue.ToString() + "'";
        //                DataSet dsAPM = dbaccess.ReadSQLTemp("APM", strSQL);
        //                DataTable dtAPM = dsAPM.Tables["APM"];
        //            }
        //        }

        //        if (cboSender.Name.ToString() == "cbo_accname")
        //        {
        //            if (cboSender.SelectedIndex == -1)
        //            {
        //                if (cboSender.Text.Trim() == "") return;
        //                MessageBox.Show("Invalid Selected Item!", "System Message");
        //                e.Cancel = true;
        //                return;
        //            }
        //            else
        //            {
        //                string strSQL = "SELECT * FROM ACM WHERE ACCNUM='" + cboSender.SelectedValue.ToString() + "'";
        //                DataSet dsACM = dbaccess.ReadSQLTemp("ACM", strSQL);
        //                DataTable dtACM = dsACM.Tables["ACM"];
        //            }
        //        }
        //    }
        //}
        //private void LoadComboBox()
        //{
        //    string strSQLARM = "SELECT ARNUM,ARNAME FROM ARM ORDER BY ARNAME";
        //    genFunctions.BindComboBox(cboArname, strSQLARM, "ARNAME", "ARNUM");
        //    string strSQLAPM = "SELECT APNUM,APNAME FROM APM ORDER BY APNAME";
        //    genFunctions.BindComboBox(cboApname, strSQLAPM, "APNAME", "APNUM");
        //    string strSQLACM = "SELECT ACCNUM,ACCNAME FROM ACM";
        //    genFunctions.BindComboBox(cboAccname, strSQLACM, "ACCNUM", "ACCNAME");
        //    string strSQLEXR = "SELECT ORICUR,CURNAME FROM EXR" + Common.DEFAULT_SYSTEM_YEAR + " ORDER BY CURNAME";
        //    genFunctions.BindComboBox(cboOricur, strSQLEXR, "ORICUR", "CURNAME");
        //}

        private void DeleteCheckItemsOnBFAVT1()
        {
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];

            BizFunctions.DeleteRow(favt1, " mark=true");
        }

        private void MakeLostFocusEvent()
        {
            txtOridebit.LostFocus += new EventHandler(txtOridebit_LostFocus);
            txtOricredit.LostFocus += new EventHandler(txtOricredit_LostFocus);
        }

        void txtOridebit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

            }
        }

        protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Delete_OnClick(sender, e);

            calcTotalDebitCredit();
        }

        private void MakeF3DblClickEventsOnGrid()
        {
            foreach (DataGridTableStyle dataGridTableStyle in dgDetail.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
                {
                    if (dataGridColumnStyle is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn = null;

                        bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;
                        bizDataGridTextBoxColumn.TextBoxGrid.MouseDoubleClick += new MouseEventHandler(dgDetail_MouseDoubleClick);

                    }
                }
            }
        }

        private void dgDetail_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int intRow = dgDetail.CurrentRowIndex;

            txtVouchernum.Focus();

            txtVouchernum.Text = dgDetail[intRow, 1].ToString();
            dtTrandate.Text = dgDetail[intRow, 2].ToString();
            txtAccname.Text = dgDetail[intRow, 3].ToString();
            cboAccname.Text = dgDetail[intRow, 3].ToString();
            txtOridebit.Text = dgDetail[intRow, 5].ToString();
            txtOricredit.Text = dgDetail[intRow, 6].ToString();
            txtDesc.Text = dgDetail[intRow, 7].ToString();
            txtApname.Text = dgDetail[intRow, 8].ToString();
            cboApname.Text = dgDetail[intRow, 9].ToString();
            txtArname.Text = dgDetail[intRow, 10].ToString();
            cboArname.Text = dgDetail[intRow, 11].ToString();
            dtInvdate.Text = dgDetail[intRow, 13].ToString();
            txtChequeno.Text = dgDetail[intRow, 14].ToString();
            cboOricur.Text = dgDetail[intRow, 15].ToString();


            dgDetail.Enabled = false;
            grpHeaderTransInfo.Enabled = true;
            grpHeaderVoucherInfo.Enabled = true;

            txtVouchernum.Enabled = false;
            cboApname.Enabled = false;
            cboArname.Enabled = false;
            txtChequeno.Enabled = false;
            dtTrandate.Enabled = false;
            txtDesc.Enabled = true;
            dtInvdate.Enabled = false;

            btnAdd.Enabled = false;
            btnNextTrans.Enabled = false;
            btnUpdate.Enabled = true;
            btnInsertTrans.Enabled = false;
            blnNew = false;
            txtCurrentAction.Text = "Updating Transactions Info, hit the Update button or press F5 once updating is done!";
        }
        private void AccnumChanged_Combo(object sender, EventArgs e)
        {
            txtAccname.Text = "";
            if (cboAccname.Text != "")
            {
                txtAccname.Text = cboAccname["accnum"].ToString().Trim();
            }
        }

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

        //#region Steph - Set Default For Header SIVH
        //private void AutoSetDefaultValueNVCHeader()
        //{
        //    DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];

        //    MDTReader.SetDefaultValueNVC(ref this.dbaccess, favth["flag"].ToString().Trim(), "favth", "oricur");
        //}
        //#endregion


        private void refreshBfavt1()
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];

            #region Refresh for detail transaction BFAVT1
            foreach (DataRow dr in favt1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {

                    #region Steph - To get AP/ AR name in case user brought over the openings from last year
                    dbaccess.ReadSQL("getApname", "SELECT apname from apm where apnum = '" + dr["apnum"].ToString().Trim() + "'");
                    if (dbaccess.DataSet.Tables["getApname"].Rows.Count > 0)
                    {
                        if (dr["apname"].ToString().Trim() == "" || dr["apname"] == System.DBNull.Value)
                        {
                            dr["apname"] = dbaccess.DataSet.Tables["getApname"].Rows[0]["apname"].ToString().Trim();
                        }
                    }

                    dbaccess.ReadSQL("getArname", "SELECT arname from arm where arnum = '" + dr["arnum"].ToString().Trim() + "'");
                    if (dbaccess.DataSet.Tables["getArname"].Rows.Count > 0)
                    {
                        if (dr["arname"].ToString().Trim() == "" || dr["arname"] == System.DBNull.Value)
                        {
                            dr["arname"] = dbaccess.DataSet.Tables["getArname"].Rows[0]["arname"].ToString().Trim();
                        }
                    }
                    dbaccess.ReadSQL("getAccname", "SELECT accname from acm where accnum = '" + dr["accnum"].ToString().Trim() + "'");
                    if (dbaccess.DataSet.Tables["getAccname"].Rows.Count > 0)
                    {
                        if (dr["accname"].ToString().Trim() == "" || dr["accname"] == System.DBNull.Value)
                        {
                            dr["accname"] = dbaccess.DataSet.Tables["getAccname"].Rows[0]["accname"].ToString().Trim();
                        }
                    }
                    #endregion

                    dr["period"] = BizAccounts.GetPeriod(dbaccess, (DateTime)dr["trandate"]);
                    dr["exrate"] = BizAccounts.GetExRate(dbaccess, dr["oricur"].ToString(), (int)BizAccounts.GetPeriod(dbaccess, (DateTime)dr["trandate"]));
                }
            }
            calcTotalDebitCredit();
            #endregion
        }

        public void getAROpening()
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];


            dbaccess.ReadSQL("getAR", "SELECT arnum,invnum,invdate,oricur,accnum,sum(oriamt) as oriamt,sum(postamt) as postamt from ard" + lastYear + " WHERE arnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' and arnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' GROUP BY arnum,invnum,invdate,oricur,accnum");
            DataTable getAR = dbaccess.DataSet.Tables["getAR"];
            foreach (DataRow dr in getAR.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    DataRow addArd = favt1.Rows.Add(new object[] { });
                    addArd["vouchernum"] = dr["invnum"];
                    addArd["trandate"] = favth["trandate"];
                    addArd["coy"] = "SAF";
                    addArd["arnum"] = dr["arnum"];
                    addArd["invnum"] = dr["invnum"];
                    addArd["invdate"] = dr["invdate"];
                    addArd["oricur"] = dr["oricur"];
                    addArd["accnum"] = dr["accnum"];
                    addArd["detail"] = favth["detail"];
                    if ((decimal)dr["oriamt"] > 0)
                    {
                        addArd["oridebit"] = dr["oriamt"];
                    }
                    else
                    {
                        addArd["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
                    }

                }
            }

            refreshBfavt1();

        }

        public void getAPOpening()
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];


            dbaccess.ReadSQL("getAP", "SELECT apnum,invnum,invdate,oricur,accnum,sum(oriamt) as oriamt,sum(postamt) as postamt from apd" + lastYear + " WHERE apnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' and apnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' GROUP BY apnum,invnum,invdate,oricur,accnum");
            DataTable getAP = dbaccess.DataSet.Tables["getAP"];
            foreach (DataRow dr in getAP.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    DataRow addApd = favt1.Rows.Add(new object[] { });
                    addApd["vouchernum"] = dr["invnum"];
                    addApd["trandate"] = favth["trandate"];
                    addApd["coy"] = "SAF";
                    addApd["apnum"] = dr["apnum"];
                    addApd["invnum"] = dr["invnum"];
                    addApd["invdate"] = dr["invdate"];
                    addApd["oricur"] = dr["oricur"];
                    addApd["accnum"] = dr["accnum"];
                    addApd["detail"] = favth["detail"];
                    if ((decimal)dr["oriamt"] > 0)
                    {
                        addApd["oridebit"] = dr["oriamt"];
                    }
                    else
                    {
                        addApd["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
                    }

                }
            }

            refreshBfavt1();

        }

        public void getCSHOpening()
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];


            dbaccess.ReadSQL("getCSH", "SELECT chknum,trandate,oricur,accnum,sum(oriamt) as oriamt,sum(postamt) as postamt from csh" + lastYear + " WHERE " +
                "accnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' and accnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' AND (RECONDATE='' OR RECONDATE IS NULL) GROUP BY refnum,chknum," +
                "trandate,accnum,oricur UNION ALL SELECT max(chknum) as chknum,max(trandate) as trandate,oricur,accnum,sum(oriamt) as oriamt,sum(postamt) as postamt from csh" + lastYear + " WHERE " +
                "accnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' and accnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' AND (RECONDATE <>'' and RECONDATE IS NOT NULL) GROUP BY accnum,oricur");
            DataTable getCSH = dbaccess.DataSet.Tables["getCSH"];
            foreach (DataRow dr in getCSH.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    DataRow addCsh = favt1.Rows.Add(new object[] { });
                    addCsh["vouchernum"] = dr["chknum"];
                    addCsh["trandate"] = favth["trandate"];
                    addCsh["coy"] = "SAF";
                    //addCsh["apnum"] = dr["apnum"];
                    addCsh["chknum"] = dr["chknum"];
                    //addCsh["invdate"] = dr["invdate"];
                    addCsh["oricur"] = dr["oricur"];
                    addCsh["accnum"] = dr["accnum"];
                    addCsh["detail"] = favth["detail"];
                    if ((decimal)dr["oriamt"] > 0)
                    {
                        addCsh["oridebit"] = dr["oriamt"];
                    }
                    else
                    {
                        addCsh["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
                    }

                }
            }

            refreshBfavt1();

        }

        public void getGLOpening()
        {
            DataRow favth = dbaccess.DataSet.Tables["favth"].Rows[0];
            DataTable favt1 = dbaccess.DataSet.Tables["favt1"];


            dbaccess.ReadSQL("getGL", "SELECT trandate,oricur,accnum,acm.acctype as acctype,sum(oriamt) as oriamt,sum(postamt) as postamt from gld" + lastYear + " gld LEFT OUTER JOIN acm ON accnum =  acm.accnum WHERE acm.acctype in ('5','6','7','8','9','10','11','12','13','14') GROUP BY gld.accnum,gld.oricur");
            DataTable getGL = dbaccess.DataSet.Tables["getGL"];
            foreach (DataRow dr in getGL.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    DataRow addGl = favt1.Rows.Add(new object[] { });
                    addGl["vouchernum"] = favth["refnum"];
                    addGl["trandate"] = favth["trandate"];
                    addGl["coy"] = "SAF";
                    //addGl["apnum"] = dr["apnum"];
                    //addGl["invnum"] = dr["invnum"];
                    //addGl["invdate"] = dr["invdate"];
                    addGl["oricur"] = dr["oricur"];
                    addGl["accnum"] = dr["accnum"];
                    addGl["detail"] = favth["detail"];

                    if ((decimal)dr["oriamt"] > 0)
                    {
                        addGl["oridebit"] = dr["oriamt"];
                    }
                    else
                    {
                        addGl["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
                    }

                }
            }

            refreshBfavt1();

        }

        private void InitialComboAccnum()
        {
            this.cboAccname = new ATL.MultiColumnComboBox.ColumnComboBox();

            this.cboAccname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboAccname.DropDownWidth = 17;
            this.cboAccname.Location = new System.Drawing.Point(110, 15);
            this.cboAccname.Name = "cboAccname";
            this.cboAccname.Size = new System.Drawing.Size(240, 21);
            this.cboAccname.TabIndex = 1;
            cboAccname.Visible = true;
            this.cboAccname.KeyDown += new KeyEventHandler(cboAccname_KeyDown);

            this.grpHeaderTransInfo.Controls.Add(cboAccname);

            //this.formName.Controls.Add(cboApname);

            //string strSQL = "SELECT ACCNAME,ACCNUM FROM ACM WHERE ACTIVE='1' ORDER BY ACCNAME";
            string strSQL = "SELECT ACCNUM,ACCNAME FROM ACM WHERE ACTIVE='1' ORDER BY ACCNUM";

            DataSet dsACM = this.dbaccess.ReadSQLTemp("ACM", strSQL);
            DataTable dtACM = dsACM.Tables["ACM"];

            cboAccname.Data = dtACM;
            //Set which row will be displayed in the text box
            //If you set this to a column that isn't displayed then the suggesting functionality won't work.
            cboAccname.ViewColumn = 0;
            //Set a few columns to not be shown
            cboAccname.ColWidthNew(new int[] { 100, 300 });
            cboAccname.UpdateIndex();
            //cboAccname.SelectedIndexChanged += new System.EventHandler(this.cboAccname_SelectedIndexChanged);
            //cboApname.UpdateIndex();
        }

        void cboAccname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }
        }

        private string getAccName(string strAccnum)
        {
            //DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
            string strSQL = "SELECT TOP 1 * FROM ACM WHERE ACCNUM='" + cboAccname["accnum"] + "'";
            DataSet dsACM = this.dbaccess.ReadSQLTemp("ACM", strSQL);
            DataTable dtACM = dsACM.Tables["ACM"];

            if (dtACM.Rows.Count > 0)
                return dtACM.Rows[0]["ACCNAME"].ToString();
            else
                return "";
        }

        private void InitialComboArnum()
        {
            this.cboArname = new ATL.MultiColumnComboBox.ColumnComboBox();

            this.cboArname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboArname.DropDownWidth = 17;
            this.cboArname.Location = new System.Drawing.Point(100, 40);
            this.cboArname.Name = "cboArname";
            this.cboArname.Size = new System.Drawing.Size(200, 21);
            this.cboArname.TabIndex = 1;
            cboArname.Visible = true;
            this.cboArname.KeyDown += new KeyEventHandler(cboArname_KeyDown);

            this.grpHeaderVoucherInfo.Controls.Add(cboArname);

            //this.formName.Controls.Add(cboApname);

            string strSQL = "SELECT ARNAME,ARNUM FROM ARM WHERE ACTIVE='1' ORDER BY ARNAME";
            DataSet dsARM = this.dbaccess.ReadSQLTemp("ARM", strSQL);
            DataTable dtARM = dsARM.Tables["ARM"];

            cboArname.Data = dtARM;
            //Set which row will be displayed in the text box
            //If you set this to a column that isn't displayed then the suggesting functionality won't work.
            cboArname.ViewColumn = 0;
            //Set a few columns to not be shown
            cboArname.ColWidthNew(new int[] { 300, 100 });
            cboArname.UpdateIndex();
            //cboAccname.SelectedIndexChanged += new System.EventHandler(this.cboAccname_SelectedIndexChanged);
            //cboApname.UpdateIndex();
        }

        void cboArname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }
            if (e.KeyCode == Keys.F11)
            {
                F11Event();
            }

            if (e.KeyCode == Keys.F12)
            {
                F12Event();
            }

        }

        private string getArName(string strArnum)
        {
            //DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
            string strSQL = "SELECT TOP 1 * FROM ARM WHERE ARNUM='" + cboArname["arnum"] + "'";
            DataSet dsARM = this.dbaccess.ReadSQLTemp("ARM", strSQL);
            DataTable dtARM = dsARM.Tables["ARM"];

            if (dtARM.Rows.Count > 0)
                return dtARM.Rows[0]["ARNAME"].ToString();
            else
                return "";
        }


        private void InitialComboApnum()
        {
            this.cboApname = new ATL.MultiColumnComboBox.ColumnComboBox();

            this.cboApname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboApname.DropDownWidth = 17;
            this.cboApname.Location = new System.Drawing.Point(100, 65);
            this.cboApname.Name = "cboApname";
            this.cboApname.Size = new System.Drawing.Size(200, 21);
            this.cboApname.TabIndex = 1;
            cboApname.Visible = true;
            this.cboApname.KeyDown += new KeyEventHandler(cboApname_KeyDown);

            this.grpHeaderVoucherInfo.Controls.Add(cboApname);

            //this.formName.Controls.Add(cboApname);

            string strSQL = "SELECT APNAME,APNUM FROM APM WHERE ACTIVE='1' ORDER BY APNAME";
            DataSet dsAPM = this.dbaccess.ReadSQLTemp("APM", strSQL);
            DataTable dtAPM = dsAPM.Tables["APM"];

            cboApname.Data = dtAPM;
            //Set which row will be displayed in the text box
            //If you set this to a column that isn't displayed then the suggesting functionality won't work.
            cboApname.ViewColumn = 0;
            //Set a few columns to not be shown
            cboApname.ColWidthNew(new int[] { 300, 100 });
            cboApname.UpdateIndex();
            //cboAccname.SelectedIndexChanged += new System.EventHandler(this.cboAccname_SelectedIndexChanged);
            //cboApname.UpdateIndex();
        }

        void cboApname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }

            if (e.KeyCode == Keys.F11)
            {
                F11Event();
            }

            if (e.KeyCode == Keys.F12)
            {
                F12Event();
            }
        }

        private string getApName(string strApnum)
        {
            //DataRow pivh = dbaccess.DataSet.Tables["pivh"].Rows[0];
            string strSQL = "SELECT TOP 1 * FROM APM WHERE APNUM='" + cboApname["apnum"] + "'";
            DataSet dsAPM = this.dbaccess.ReadSQLTemp("APM", strSQL);
            DataTable dtAPM = dsAPM.Tables["APM"];

            if (dtAPM.Rows.Count > 0)
                return dtAPM.Rows[0]["APNAME"].ToString();
            else
                return "";
        }
    }
}
