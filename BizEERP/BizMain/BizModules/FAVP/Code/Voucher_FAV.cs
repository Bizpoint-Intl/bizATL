/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_FAV.cs
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
using Microsoft.Office;
using System.Data.OleDb;


namespace ATL.FAVP
{
	public class Voucher_FAVP : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables
		protected DBAccess dbaccess = null;
		string formdDetailName;

		//FOR DETAILS
		protected GroupBox grpHeaderVoucherInfo;
		protected GroupBox grpHeaderTransInfo;
        protected string headerFormName,employeeFormName,summaryAttnFormName,summarySalFromName = "";
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
		protected TextBox txtExrate;
		protected TextBox txtOricredit;
		protected TextBox txtOricur;
		//protected ComboBox cboOricur;
		protected Button btnAdd;
		protected Button btnUpdate;
		protected Button btnInsertTrans;
		protected Button btnNextTrans;

        protected Button btn_Compute1;
        protected Button btn_Compute2;
        protected Button btn_Compute3;
        protected Button btn_Compute4;


      //   <Label		Name="lbl_t1"	  Location="10,  455"		Size="55, 16"	TextAlign="Right" Text="Rate"/>
      //<TextBox	Name="tb_t1"	  Location="10,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t2"	  Location="65,  455"		Size="55, 16"	TextAlign="Right" Text="Dir Fee"/>
      //<TextBox	Name="tb_t2"	  Location="65,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t3"	  Location="120,  455"		Size="55, 16"	TextAlign="Right" Text="Dir Rum"/>
      //<TextBox	Name="tb_t3"	  Location="120,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t4"	  Location="175,  455"		Size="55, 16"	TextAlign="Right" Text="OT"/>
      //<TextBox	Name="tb_t4"	  Location="175,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t5"	  Location="230,  455"		Size="55, 16"	TextAlign="Right" Text="Bonus"/>
      //<TextBox	Name="tb_t5"	  Location="230,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t6"	  Location="285,  455"		Size="55, 16"	TextAlign="Right" Text="CPF"/>
      //<TextBox	Name="tb_t6"	  Location="285,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t7"	  Location="340,  455"		Size="55, 16"	TextAlign="Right" Text="CPF-Skill"/>
      //<TextBox	Name="tb_t7"	  Location="340,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t8"	  Location="395,  455"		Size="55, 16"	TextAlign="Right" Text="CPF3"/>
      //<TextBox	Name="tb_t8"	  Location="395,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t9"	  Location="450,  455"		Size="55, 16"	TextAlign="Right" Text="CPF4"/>
      //<TextBox	Name="tb_t9"	  Location="450,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t10"	  Location="505,  455"		Size="55, 16"	TextAlign="Right" Text="FWL"/>
      //<TextBox	Name="tb_t10"	  Location="505,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t11"	  Location="560,  455"		Size="55, 16"	TextAlign="Right" Text="Allw"/>
      //<TextBox	Name="tb_t11"	  Location="560,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />

      //<Label		Name="lbl_t12"	  Location="615,  455"		Size="55, 16"	TextAlign="Right" Text="Med"/>
      //<TextBox	Name="tb_t12"	  Location="615,474" ReadOnly="True" 		Size="50,21"	TabIndex="51" />


        protected TextBox tb_t1;
        protected TextBox tb_t2;
        protected TextBox tb_t3;
        protected TextBox tb_t4;
        protected TextBox tb_t5;
        protected TextBox tb_t6;
        protected TextBox tb_t7;
        protected TextBox tb_t8;
        protected TextBox tb_t9;
        protected TextBox tb_t10;
        protected TextBox tb_t11;
        protected TextBox tb_t12;

		protected Label txtCurrentAction;
		protected Button btnGetOB;
		protected DialogResult Save = DialogResult.No;
		protected string prevJour = null;		

		protected bool skipValidate;

		protected Button btnInsert;
		protected Button btnDelete;
		protected Button btnUp;
		protected Button btnDown;
		protected Button btnMark;
		protected Button btnDuplicate;
		protected Button btnExtract;
		protected Button btnClose;
        protected Button btn_Extract;
        protected string Flag ="";

		protected int TabDetail;
		
		public string documentKey = null;
		public string vouchernum = "";
		public string lastYear = "";
		
		GenTools genFunctions = new GenTools();
		getFilterOP FilterOP = new getFilterOP();

		bool blnNew = true;

		protected bool opened = false;

		#endregion

		public Voucher_FAVP(string moduleName, Hashtable voucherBaseHelpers,string type) : base("VoucherGridInfo_FAV.xml", moduleName, voucherBaseHelpers)
		{
            this.Flag = type;
		}
		
		#region Steph - To stop users from open more than 1 voucher from the same module  as this is causing the saving error.
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

		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);
			e.Condition = "bfavh.flag='"+Flag+"' AND bfavh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (bfavh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" bfavh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" bfavh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND bfavh.flag='"+Flag+"' AND bfavh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

		protected override void Voucher_Form_OnLoad(object sender, VoucherEventArgs e)
		{
			base.Voucher_Form_OnLoad(sender, e);
		}


		protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
		{
			base.Document_TabControl_OnSelectionChanged(sender, e);
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			btnInsert = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Insert") as Button;
			btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
			btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
			btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
			btnClose = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Close") as Button;

				btnInsert.Enabled = false;
				btnDelete.Enabled = true;
				btnMark.Enabled = true;
				btnDuplicate.Enabled = false;

                GetDetail1Sum();

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
			base.Document_Form_OnLoad (sender, e);
			opened = true;
			dbaccess = e.DBAccess;
			this.formdDetailName = (e.FormsCollection["header"] as Form).Name;

            this.employeeFormName = (e.FormsCollection["emplist1"] as Form).Name;
            this.summaryAttnFormName = (e.FormsCollection["sumattn"] as Form).Name;
            this.summarySalFromName = (e.FormsCollection["sumsalary"] as Form).Name;

			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			documentKey = e.DocumentKey;
			skipValidate = false;

			bfavh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			bfavh["oricur"] = "SGD";

			e.DBAccess.DataSet.Tables["bfavh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_FAV_ColumnChanged);

			lastYear = Convert.ToString(GetNumeric(Common.DEFAULT_SYSTEM_YEAR) - 1);
			if (bfavh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSN)
			{
				bfavh["aropen"] = false;
				bfavh["apopen"] = false;
				bfavh["cshopen"] = false;
				bfavh["glopen"] = false;
			}

			InitializeControls();

			#region Steph - Set the current status of users action
			txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";
			#endregion
			
			grpHeaderVoucherInfo.Enabled = true; 
			grpHeaderTransInfo.Enabled = false;

			txtOridebit.Text = "0.00";
			txtOricredit.Text = "0.00";
			//txtExrate.Text = "0.00";

			MakeEnterEvent();
			MakeLostFocusEvent();

			InitialComboAccnum();
			InitialComboArnum();
			InitialComboApnum();

//			cboOricur.Text = "SGD";

			cboAccname.SelectedIndexChanged += new System.EventHandler(this.AccnumChanged_Combo);
			cboArname.SelectedIndexChanged += new EventHandler(cboArname_SelectedIndexChanged);
			cboApname.SelectedIndexChanged += new EventHandler(cboApname_SelectedIndexChanged);

//			cboOricur.SelectedIndexChanged += new EventHandler(cboOricur_SelectedIndexChanged);

			MakeF3DblClickEventsOnGrid();
			calcTotalDebitCredit();
//			setOricur();
			btnUpdate.Enabled = false;

			btnAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			btnInsertTrans.Click += new System.EventHandler(this.cmdInsert_Click);
			btnNextTrans.Click += new System.EventHandler(this.cmdNextTrans_Click);
			btnUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
			btnGetOB.Click += new EventHandler(this.cmdGetOB_Click);

			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					BizFunctions.UpdateDataRow(bfavh, dr, "user/flag/status/created/modified");
				}
			}

            btn_Extract = BizXmlReader.CurrentInstance.GetControl(formdDetailName, "btn_Extract") as Button;

            btn_Extract.Click += new EventHandler(btn_Extract_Click);


            btn_Compute1 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "btn_Compute1") as Button;
            btn_Compute1.Click += new EventHandler(btn_Compute1_Click);

            btn_Compute2 = BizXmlReader.CurrentInstance.GetControl(summaryAttnFormName, "btn_Compute2") as Button;
            btn_Compute2.Click += new EventHandler(btn_Compute2_Click);

            btn_Compute3 = BizXmlReader.CurrentInstance.GetControl(summarySalFromName, "btn_Compute3") as Button;
            btn_Compute3.Click += new EventHandler(btn_Compute3_Click);

            btn_Compute4 =  BizXmlReader.CurrentInstance.GetControl(summarySalFromName, "btn_Compute4") as Button;
            btn_Compute4.Click +=new EventHandler(btn_Compute4_Click);

            TextBox tb_t1 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t1") as TextBox;
            TextBox tb_t2 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t2") as TextBox;
            TextBox tb_t3 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t3") as TextBox;
            TextBox tb_t4 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t4") as TextBox;
            TextBox tb_t5 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t5") as TextBox;
            TextBox tb_t6 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t6") as TextBox;
            TextBox tb_t7 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t7") as TextBox;
            TextBox tb_t8 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t8") as TextBox;
            TextBox tb_t9 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t9") as TextBox;
            TextBox tb_t10 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t10") as TextBox;
            TextBox tb_t11 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t11") as TextBox;
            TextBox tb_t12 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t12") as TextBox;

    

		}

      


        void btn_Compute1_Click(object sender, EventArgs e)
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            DataTable bfav1p = dbaccess.DataSet.Tables["bfav1p"];
            DataTable bfav2p = dbaccess.DataSet.Tables["bfav2p"];
            DataTable bfav3p = dbaccess.DataSet.Tables["bfav3p"];

            int month, year = 0;

            month = Convert.ToDateTime(bfavh["trandate"]).Month;
            year = Convert.ToDateTime(bfavh["trandate"]).Year;


            TextBox tb_t1 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t1") as TextBox;
            TextBox tb_t2 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t2") as TextBox;
            TextBox tb_t3 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t3") as TextBox;
            TextBox tb_t4 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t4") as TextBox;
            TextBox tb_t5 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t5") as TextBox;
            TextBox tb_t6 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t6") as TextBox;
            TextBox tb_t7 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t7") as TextBox;
            TextBox tb_t8 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t8") as TextBox;
            TextBox tb_t9 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t9") as TextBox;
            TextBox tb_t10 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t10") as TextBox;
            TextBox tb_t11 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t11") as TextBox;
            TextBox tb_t12 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t12") as TextBox;

            if (bfav1p.Rows.Count > 0)
            {
                DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet,"Select distinct sitenum from bfav1p");

                GetDetail1Sum();


                if (dt1 != null)
                {
                    if (dt1.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(bfav2p);
                        foreach (DataRow dr1 in dt1.Rows)
                        {
                            DataRow insertBfav2p = bfav2p.NewRow();

                            string get1 = "Select sitenum,sum(convert(int,attnmark)) as attnmark, SUM(ActualOTHrs) AS ActualOTHrs from atr1 where  MONTH([date])=" + month.ToString() + " and YEAR([date])=" + year.ToString() + " and sitenum='" + dr1["sitenum"].ToString() + "' GROUP BY sitenum";

                            this.dbaccess.ReadSQL("TmpSumAttn", get1);

                            DataTable tmp1 = this.dbaccess.DataSet.Tables["TmpSumAttn"];

                            if(tmp1.Rows.Count>0)
                            {
                                
                                decimal WorkDays = 0;
                                decimal  OTHrs = 0;
                                foreach (DataRow drTmp1 in tmp1.Rows)
                                {

                                    insertBfav2p["sitenum"] = dr1["sitenum"].ToString();
                                    insertBfav2p["projectid"] = BizLogicTools.Tools.GetProjectIDFromSitenum(dr1["sitenum"].ToString(),this.dbaccess);
                                    insertBfav2p["WorkDays"] = drTmp1["attnmark"];
                                    insertBfav2p["OTHrs"] = drTmp1["ActualOTHrs"];
                                  
                                    bfav2p.Rows.Add(insertBfav2p);

                                    WorkDays = WorkDays + Convert.ToDecimal(drTmp1["attnmark"]);
                                    OTHrs = OTHrs + Convert.ToDecimal(drTmp1["ActualOTHrs"]);
                                }

                                foreach (DataRow dr2 in bfav2p.Rows)
                                {
                                    if (dr2.RowState != DataRowState.Deleted)
                                    {
                                        if (!BizFunctions.IsEmpty(dr2["WorkDays"]) && WorkDays > 0)
                                        {
                                            if (Convert.ToDecimal(dr2["WorkDays"]) > 0)
                                            {
                                                dr2["WorkDaysPc"] = (Convert.ToDecimal(dr2["WorkDays"]) / WorkDays) * 100;
                                            }
                                            else
                                            {
                                                dr2["WorkDaysPc"] = 0;
                                            }

                                            if (!BizFunctions.IsEmpty(dr2["OTHrs"]) && OTHrs > 0)
                                            {
                                                dr2["OTHrsPc"] = (Convert.ToDecimal(dr2["OTHrs"]) / OTHrs) * 100;
                                            }
                                            else
                                            {
                                                dr2["OTHrsPc"] = 0;
                                            }

                                        }
                                    }
                                }
                            }


                        }
                    }
                }
            }

         

        }

        void btn_Compute2_Click(object sender, EventArgs e)
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            DataTable bfav1p = dbaccess.DataSet.Tables["bfav1p"];
            DataTable bfav2p = dbaccess.DataSet.Tables["bfav2p"];
            DataTable bfav3p = dbaccess.DataSet.Tables["bfav3p"];
            DataTable bfav4p = dbaccess.DataSet.Tables["bfav4p"];
            if (bfav2p.Rows.Count > 0)
            {


                string get1 = "Select projectid,SUM(rateamt) as rateamt,SUM(dirFeeAmt) as dirFeeAmt,SUM(DirRemuAmt) as DirRemuAmt,SUM(otamt) as otamt,SUM(bonusamt) as bonusamt,SUM(alencash) as alencash, SUM(accomallow) as accomallow, SUM(adjamt) as adjamt, SUM(cashadv) as cashadv, SUM(cdacamt) as cdacamt, SUM(handphoneamt) as handphoneamt, SUM(loadamt) as loadamt, SUM(medreim) as medreim, SUM(mosqamt) as mosqamt, SUM(otallowamt) as otallowamt, SUM(sindamt) as sindamt, SUM(supallowamt) as supallowamt, SUM(transallowamt) as transallowamt, SUM(transreimamt) as transreimamt, SUM(unionfeeamt) as unionfeeamt, SUM(varamt) as varamt, SUM(otherallowamt) as otherallowamt,SUM(accrualStaffSal) as accrualStaffSal,SUM(accrualsiva) as accrualsiva, SUM(accrualCpfSdl) as accrualCpfSdl, SUM(accrualFwl) as accrualFwl  " +
                              ",SUM(cpfamt) as cpfamt,SUM(cpfskillamt) as cpfskillamt,SUM(cpf3amt) as cpf3amt,SUM(cpf4amt) as cpf4amt,SUM(fwlamt) as fwlamt,SUM(allowamt) as allowamt " +
                              ",SUM(medamt) as medamt from BFAV1P group by projectid";

                string get2 = "Select projectid,matnum,SUM(rateamt) as rateamt,SUM(dirFeeAmt) as dirFeeAmt,SUM(DirRemuAmt) as DirRemuAmt,SUM(otamt) as otamt,SUM(bonusamt) as bonusamt,SUM(alencash) as alencash, SUM(accomallow) as accomallow, SUM(adjamt) as adjamt, SUM(cashadv) as cashadv, SUM(cdacamt) as cdacamt, SUM(handphoneamt) as handphoneamt, SUM(loadamt) as loadamt, SUM(medreim) as medreim, SUM(mosqamt) as mosqamt, SUM(otallowamt) as otallowamt, SUM(sindamt) as sindamt, SUM(supallowamt) as supallowamt, SUM(transallowamt) as transallowamt, SUM(transreimamt) as transreimamt, SUM(unionfeeamt) as unionfeeamt, SUM(varamt) as varamt, SUM(otherallowamt) as otherallowamt,SUM(accrualStaffSal) as accrualStaffSal,SUM(accrualsiva) as accrualsiva, SUM(accrualCpfSdl) as accrualCpfSdl, SUM(accrualFwl) as accrualFwl  " +
                            ",SUM(cpfamt) as cpfamt,SUM(cpfskillamt) as cpfskillamt,SUM(cpf3amt) as cpf3amt,SUM(cpf4amt) as cpf4amt,SUM(fwlamt) as fwlamt,SUM(allowamt) as allowamt " +
                            ",SUM(medamt) as medamt from BFAV1P group by matnum,projectid";

                DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, get1);

                DataTable dt2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, get2);

                //decimal rateamt1, dirFeeAmt1, DirRemuAmt1, otamt1, bonusamt1, cpfamt1, cpfskillamt1, cpf3amt1, cpf4amt1, fwlamt1, allowamt1,
                //          medamt1, alencash1, accomallow1, adjamt1, cashadv1, cdacamt1, handphoneamt1, loadamt1, medreim1, mosqamt1, otallowamt1, sindamt1, supallowamt1, 
                //          transallowamt1, transreimamt1, unionfeeamt1, varamt1, otherallowamt1 = 0;

                decimal rateamt1 = 0;
                decimal dirFeeAmt1 = 0;
                decimal DirRemuAmt1 = 0;
                decimal otamt1 = 0;
                decimal bonusamt1 = 0;
                decimal cpfamt1 = 0;
                decimal cpfskillamt1 = 0;
                decimal cpf3amt1 = 0;
                decimal cpf4amt1 = 0;
                decimal fwlamt1 = 0;
                decimal medamt1 = 0;
                decimal alencash1 = 0;
                decimal accomallow1 = 0;
                decimal adjamt1 = 0;
                decimal cashadv1 = 0;
                decimal cdacamt1 = 0;
                decimal handphoneamt1 = 0;
                decimal loadamt1 = 0;
                decimal medreim1 = 0;
                decimal mosqamt1 = 0;
                decimal otallowamt1 = 0;
                decimal sindamt1 = 0;
                decimal supallowamt1 = 0;
                decimal transallowamt1 = 0;
                decimal transreimamt1 = 0;
                decimal unionfeeamt1 = 0;
                decimal varamt1 = 0;
                decimal otherallowamt1 = 0;
                decimal allowamt1 = 0;
                decimal accrualStaffSal1 = 0;
                decimal accrualsiva1 = 0;
                decimal accrualCpfSdl1 = 0;
                decimal accrualFwl1 = 0;

                decimal rateamt2 = 0;
                decimal dirFeeAmt2 = 0;
                decimal DirRemuAmt2 = 0;
                decimal otamt2 = 0;
                decimal bonusamt2 = 0;
                decimal cpfamt2 = 0;
                decimal cpfskillamt2 = 0;
                decimal cpf3amt2 = 0;
                decimal cpf4amt2 = 0;
                decimal fwlamt2 = 0;
                decimal medamt2 = 0;
                decimal alencash2 = 0;
                decimal accomallow2 = 0;
                decimal adjamt2 = 0;
                decimal cashadv2 = 0;
                decimal cdacamt2 = 0;
                decimal handphoneamt2 = 0;
                decimal loadamt2 = 0;
                decimal medreim2 = 0;
                decimal mosqamt2 = 0;
                decimal otallowamt2 = 0;
                decimal sindamt2 = 0;
                decimal supallowamt2 = 0;
                decimal transallowamt2 = 0;
                decimal transreimamt2 = 0;
                decimal unionfeeamt2 = 0;
                decimal varamt2 = 0;
                decimal otherallowamt2 = 0;
                decimal allowamt2 = 0;
                decimal accrualStaffSal2 = 0;
                decimal accrualsiva2 = 0;
                decimal accrualCpfSdl2 = 0;
                decimal accrualFwl2 = 0;


                //decimal rateamt2, dirFeeAmt2, DirRemuAmt2, otamt2, bonusamt2, cpfamt2, cpfskillamt2, cpf3amt2, cpf4amt2, fwlamt2, allowamt2,
                //      medamt2, alencash2, accomallow2, adjamt2, cashadv2, cdacamt2, handphoneamt2, loadamt2, medreim2, mosqamt2, otallowamt2, sindamt2, supallowamt2, transallowamt2,
                //      transreimamt2, unionfeeamt2, varamt2, otherallowamt2 = 0;

                if (dt1.Rows.Count > 0)
                {
                    if (bfav3p.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(bfav3p);
                    }

                    foreach (DataRow dr1 in dt1.Rows)
                    {
                        DataRow insertbfav3p = bfav3p.NewRow();

                        insertbfav3p["projectid"] = dr1["projectid"];
                        insertbfav3p["rateamt"] = dr1["rateamt"];
                        insertbfav3p["dirFeeAmt"] = dr1["dirFeeAmt"];
                        insertbfav3p["DirRemuAmt"] = dr1["DirRemuAmt"];
                        insertbfav3p["otamt"] = dr1["otamt"];
                        insertbfav3p["bonusamt"] = dr1["bonusamt"];
                        insertbfav3p["cpfamt"] = dr1["cpfamt"];
                        insertbfav3p["cpfskillamt"] = dr1["cpfskillamt"];
                        insertbfav3p["cpf3amt"] = dr1["cpf3amt"];
                        insertbfav3p["cpf4amt"] = dr1["cpf4amt"];
                        insertbfav3p["fwlamt"] = dr1["fwlamt"];
                        insertbfav3p["allowamt"] = dr1["allowamt"];
                        insertbfav3p["medamt"] = dr1["medamt"];


                        insertbfav3p["alencash"] = dr1["alencash"];
                        insertbfav3p["accomallow"] = dr1["accomallow"];
                        insertbfav3p["adjamt"] = dr1["adjamt"];
                        insertbfav3p["cashadv"] = dr1["cashadv"];
                        insertbfav3p["cdacamt"] = dr1["cdacamt"];
                        insertbfav3p["handphoneamt"] = dr1["handphoneamt"];
                        insertbfav3p["loadamt"] = dr1["loadamt"];
                        insertbfav3p["medreim"] = dr1["medreim"];
                        insertbfav3p["mosqamt"] = dr1["mosqamt"];
                        insertbfav3p["otallowamt"] = dr1["otallowamt"];
                        insertbfav3p["sindamt"] = dr1["sindamt"];
                        insertbfav3p["supallowamt"] = dr1["supallowamt"];
                        insertbfav3p["transallowamt"] = dr1["transallowamt"];
                        insertbfav3p["transreimamt"] = dr1["transreimamt"];
                        insertbfav3p["unionfeeamt"] = dr1["unionfeeamt"];
                        insertbfav3p["varamt"] = dr1["varamt"];
                        insertbfav3p["otherallowamt"] = dr1["otherallowamt"];

                        insertbfav3p["accrualStaffSal"] = dr1["accrualStaffSal"];
                        insertbfav3p["accrualsiva"] = dr1["accrualsiva"];
                        insertbfav3p["accrualCpfSdl"] = dr1["accrualCpfSdl"];
                        insertbfav3p["accrualFwl"] = dr1["accrualFwl"];

                        bfav3p.Rows.Add(insertbfav3p);
                    }
                }


                if (dt2.Rows.Count > 0)
                {
                    //1 - CL, 2 - Non-CL
                  

                    if (bfav4p.Rows.Count > 0)
                    {
                        BizFunctions.DeleteAllRows(bfav4p);
                    }

                //    foreach (DataRow dr2 in dt2.Rows)
                //    {

                    
                //        DataRow insertbfav4p = bfav4p.NewRow();                        

                //        insertbfav4p["matnum"] = dr2["matnum"];
                //        insertbfav4p["rateamt"] = dr2["rateamt"];
                //        insertbfav4p["dirFeeAmt"] = dr2["dirFeeAmt"];
                //        insertbfav4p["DirRemuAmt"] = dr2["DirRemuAmt"];
                //        insertbfav4p["otamt"] = dr2["otamt"];
                //        insertbfav4p["bonusamt"] = dr2["bonusamt"];
                //        insertbfav4p["cpfamt"] = dr2["cpfamt"];
                //        insertbfav4p["cpfskillamt"] = dr2["cpfskillamt"];
                //        insertbfav4p["cpf3amt"] = dr2["cpf3amt"];
                //        insertbfav4p["cpf4amt"] = dr2["cpf4amt"];
                //        insertbfav4p["fwlamt"] = dr2["fwlamt"];
                //        insertbfav4p["allowamt"] = dr2["allowamt"];
                //        insertbfav4p["medamt"] = dr2["medamt"];
                //        insertbfav4p["alencash"] = dr2["alencash"];
                //        insertbfav4p["accomallow"] = dr2["accomallow"];
                //        insertbfav4p["adjamt"] = dr2["adjamt"];
                //        insertbfav4p["cashadv"] = dr2["cashadv"];
                //        insertbfav4p["cdacamt"] = dr2["cdacamt"];
                //        insertbfav4p["handphoneamt"] = dr2["handphoneamt"];
                //        insertbfav4p["loadamt"] = dr2["loadamt"];
                //        insertbfav4p["medreim"] = dr2["medreim"];
                //        insertbfav4p["mosqamt"] = dr2["mosqamt"];
                //        insertbfav4p["otallowamt"] = dr2["otallowamt"];
                //        insertbfav4p["sindamt"] = dr2["sindamt"];
                //        insertbfav4p["supallowamt"] = dr2["supallowamt"];
                //        insertbfav4p["transallowamt"] = dr2["transallowamt"];
                //        insertbfav4p["transreimamt"] = dr2["transreimamt"];
                //        insertbfav4p["unionfeeamt"] = dr2["unionfeeamt"];
                //        insertbfav4p["varamt"] = dr2["varamt"];
                //        insertbfav4p["otherallowamt"] = dr2["otherallowamt"];

                //        bfav4p.Rows.Add(insertbfav4p);
                //    }
                //}

                foreach (DataRow dr2 in dt2.Rows)
                {
                    if (BizFunctions.IsEmpty(dr2["rateamt"]))
                    {
                        dr2["rateamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["dirFeeAmt"]))
                    {
                        dr2["dirFeeAmt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["DirRemuAmt"]))
                    {
                        dr2["DirRemuAmt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["otamt"]))
                    {
                        dr2["otamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["bonusamt"]))
                    {
                        dr2["bonusamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["cpfamt"]))
                    {
                        dr2["cpfamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["cpfskillamt"]))
                    {
                        dr2["cpfskillamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["cpf3amt"]))
                    {
                        dr2["cpf3amt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["cpf4amt"]))
                    {
                        dr2["cpf4amt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["fwlamt"]))
                    {
                        dr2["fwlamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["allowamt"]))
                    {
                        dr2["allowamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["medamt"]))
                    {
                        dr2["medamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["alencash"]))
                    {
                        dr2["alencash"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["accomallow"]))
                    {
                        dr2["accomallow"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["accomallow"]))
                    {
                        dr2["accomallow"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["adjamt"]))
                    {
                        dr2["adjamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["cashadv"]))
                    {
                        dr2["cashadv"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["cdacamt"]))
                    {
                        dr2["cdacamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["handphoneamt"]))
                    {
                        dr2["handphoneamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["loadamt"]))
                    {
                        dr2["loadamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["medreim"]))
                    {
                        dr2["medreim"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["mosqamt"]))
                    {
                        dr2["mosqamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["otallowamt"]))
                    {
                        dr2["otallowamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["sindamt"]))
                    {
                        dr2["sindamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["supallowamt"]))
                    {
                        dr2["supallowamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["transallowamt"]))
                    {
                        dr2["transallowamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["unionfeeamt"]))
                    {
                        dr2["unionfeeamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["varamt"]))
                    {
                        dr2["varamt"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["accrualStaffSal"]))
                    {
                        dr2["accrualStaffSal"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["accrualsiva"]))
                    {
                        dr2["accrualsiva"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["accrualCpfSdl"]))
                    {
                        dr2["accrualCpfSdl"] = 0;
                    }
                    if (BizFunctions.IsEmpty(dr2["accrualFwl"]))
                    {
                        dr2["accrualFwl"] = 0;
                    }

                    if (BizFunctions.IsEmpty(dr2["otherallowamt"]))
                    {
                        dr2["otherallowamt"] = 0;
                    }


                        if (dr2["matnum"].ToString().ToUpper().Trim() == "CL")
                        {
                            DataRow insertbfav4p = bfav4p.NewRow();

                            rateamt1 = rateamt1 + Convert.ToDecimal(dr2["rateamt"]);
                            dirFeeAmt1 = dirFeeAmt1 + Convert.ToDecimal(dr2["dirFeeAmt"]);
                            DirRemuAmt1 = DirRemuAmt1 + Convert.ToDecimal(dr2["DirRemuAmt"]);
                            otamt1 = otamt1 + Convert.ToDecimal(dr2["otamt"]);
                            bonusamt1 = bonusamt1 + Convert.ToDecimal(dr2["bonusamt"]);
                            cpfamt1 = cpfamt1 + Convert.ToDecimal(dr2["cpfamt"]);
                            cpfskillamt1 = cpfskillamt1 + Convert.ToDecimal(dr2["cpfskillamt"]);
                            cpf3amt1 = cpf3amt1 + Convert.ToDecimal(dr2["cpf3amt"]);
                            cpf4amt1 = cpf4amt1 + Convert.ToDecimal(dr2["cpf4amt"]);
                            fwlamt1 = fwlamt1 + Convert.ToDecimal(dr2["fwlamt"]);
                            allowamt1 = allowamt1 + Convert.ToDecimal(dr2["allowamt"]);
                            medamt1 = medamt1 + Convert.ToDecimal(dr2["medamt"]);
                            alencash1 = alencash1 + Convert.ToDecimal(dr2["alencash"]);
                            accomallow1 = accomallow1 + Convert.ToDecimal(dr2["accomallow"]);
                            adjamt1 = adjamt1 + Convert.ToDecimal(dr2["adjamt"]);
                            cashadv1 = cashadv1 + Convert.ToDecimal(dr2["cashadv"]);
                            cdacamt1 = cdacamt1 + Convert.ToDecimal(dr2["cdacamt"]);
                            handphoneamt1 = handphoneamt1 + Convert.ToDecimal(dr2["handphoneamt"]);
                            loadamt1 = loadamt1 + Convert.ToDecimal(dr2["loadamt"]);
                            medreim1 = medreim1 + Convert.ToDecimal(dr2["medreim"]);
                            mosqamt1 = mosqamt1 + Convert.ToDecimal(dr2["mosqamt"]);
                            otallowamt1 = otallowamt1 + Convert.ToDecimal(dr2["otallowamt"]);
                            sindamt1 = sindamt1 + Convert.ToDecimal(dr2["sindamt"]);
                            supallowamt1 = supallowamt1 + Convert.ToDecimal(dr2["supallowamt"]);
                            transallowamt1 = transallowamt1 + Convert.ToDecimal(dr2["transallowamt"]);
                            unionfeeamt1 = unionfeeamt1 + Convert.ToDecimal(dr2["unionfeeamt"]);
                            varamt1 = varamt1 + Convert.ToDecimal(dr2["varamt"]);
                            otherallowamt1 = otherallowamt1 + Convert.ToDecimal(dr2["otherallowamt"]);

                            insertbfav4p["projectid"] = dr2["projectid"];
                            insertbfav4p["matnum"] = "CL";
                            insertbfav4p["rateamt"] = rateamt1;
                            insertbfav4p["dirFeeAmt"] = dirFeeAmt1;
                            insertbfav4p["DirRemuAmt"] = DirRemuAmt1;
                            insertbfav4p["otamt"] = otallowamt1;
                            insertbfav4p["bonusamt"] = bonusamt1;
                            insertbfav4p["cpfamt"] = cpfamt1;
                            insertbfav4p["cpfskillamt"] = cpfskillamt1;
                            insertbfav4p["cpf3amt"] = cpf3amt1;
                            insertbfav4p["cpf4amt"] = cpf4amt1;
                            insertbfav4p["fwlamt"] = fwlamt1;
                            insertbfav4p["allowamt"] = allowamt1;
                            insertbfav4p["medamt"] = medamt1;


                            insertbfav4p["alencash"] = alencash1;
                            insertbfav4p["accomallow"] = accomallow1;
                            insertbfav4p["adjamt"] = adjamt1;
                            insertbfav4p["cashadv"] = cashadv1;
                            insertbfav4p["cdacamt"] = cdacamt1;
                            insertbfav4p["handphoneamt"] = handphoneamt1;
                            insertbfav4p["loadamt"] = loadamt1;
                            insertbfav4p["medreim"] = medreim1;
                            insertbfav4p["mosqamt"] = mosqamt1;
                            insertbfav4p["otallowamt"] = otallowamt1;
                            insertbfav4p["sindamt"] = sindamt1;
                            insertbfav4p["supallowamt"] = supallowamt1;
                            insertbfav4p["transallowamt"] = transallowamt1;
                            insertbfav4p["transreimamt"] = transreimamt1;
                            insertbfav4p["unionfeeamt"] = unionfeeamt1;
                            insertbfav4p["varamt"] = varamt1;
                            insertbfav4p["otherallowamt"] = otallowamt1;

                            insertbfav4p["accrualStaffSal"] = dr2["accrualStaffSal"];
                            insertbfav4p["accrualsiva"] = dr2["accrualsiva"];
                            insertbfav4p["accrualCpfSdl"] = dr2["accrualCpfSdl"];
                            insertbfav4p["accrualFwl"] = dr2["accrualFwl"];

                            bfav4p.Rows.Add(insertbfav4p);
                        
                        }
                        else
                        {
                            rateamt2 = rateamt2 + Convert.ToDecimal(dr2["rateamt"]);
                            dirFeeAmt2 = dirFeeAmt2 + Convert.ToDecimal(dr2["dirFeeAmt"]);
                            DirRemuAmt2 = DirRemuAmt2 + Convert.ToDecimal(dr2["DirRemuAmt"]);
                            otamt2 = otamt2 + Convert.ToDecimal(dr2["otamt"]);
                            bonusamt2 = bonusamt2 + Convert.ToDecimal(dr2["bonusamt"]);
                            cpfamt2 = cpfamt2 + Convert.ToDecimal(dr2["cpfamt"]);
                            cpfskillamt2 = cpfskillamt2 + Convert.ToDecimal(dr2["cpfskillamt"]);
                            cpf3amt2 = cpf3amt2 + Convert.ToDecimal(dr2["cpf3amt"]);
                            cpf4amt2 = cpf4amt2 + Convert.ToDecimal(dr2["cpf4amt"]);
                            fwlamt2 = fwlamt2 + Convert.ToDecimal(dr2["fwlamt"]);
                            allowamt2 = allowamt2 + Convert.ToDecimal(dr2["allowamt"]);
                            medamt2 = medamt2 + Convert.ToDecimal(dr2["medamt"]);
                            alencash2 = alencash2 + Convert.ToDecimal(dr2["alencash"]);
                            accomallow2 = accomallow2 + Convert.ToDecimal(dr2["accomallow"]);
                            adjamt2 = adjamt2 + Convert.ToDecimal(dr2["adjamt"]);
                            cashadv2 = cashadv2 + Convert.ToDecimal(dr2["cashadv"]);
                            cdacamt2 = cdacamt2 + Convert.ToDecimal(dr2["cdacamt"]);
                            handphoneamt2 = handphoneamt2 + Convert.ToDecimal(dr2["handphoneamt"]);
                            loadamt2 = loadamt2 + Convert.ToDecimal(dr2["loadamt"]);
                            medreim2 = medreim2 + Convert.ToDecimal(dr2["medreim"]);
                            mosqamt2 = mosqamt2 + Convert.ToDecimal(dr2["mosqamt"]);
                            otallowamt2 = otallowamt2 + Convert.ToDecimal(dr2["otallowamt"]);
                            sindamt2 = sindamt2 + Convert.ToDecimal(dr2["sindamt"]);
                            supallowamt2 = supallowamt2 + Convert.ToDecimal(dr2["supallowamt"]);
                            transallowamt2 = transallowamt2 + Convert.ToDecimal(dr2["transallowamt"]);
                            unionfeeamt2 = unionfeeamt2 + Convert.ToDecimal(dr2["unionfeeamt"]);
                            varamt2 = varamt2 + Convert.ToDecimal(dr2["varamt"]);
                            otherallowamt2 = otherallowamt2 + Convert.ToDecimal(dr2["otherallowamt"]);

                            DataRow insertbfav4p = bfav4p.NewRow();

                            insertbfav4p["projectid"] = dr2["projectid"];
                            insertbfav4p["matnum"] = "HQ";
                            insertbfav4p["rateamt"] = rateamt2;
                            insertbfav4p["dirFeeAmt"] = dirFeeAmt2;
                            insertbfav4p["DirRemuAmt"] = DirRemuAmt2;
                            insertbfav4p["otamt"] = otallowamt2;
                            insertbfav4p["bonusamt"] = bonusamt2;
                            insertbfav4p["cpfamt"] = cpfamt2;
                            insertbfav4p["cpfskillamt"] = cpfskillamt2;
                            insertbfav4p["cpf3amt"] = cpf3amt2;
                            insertbfav4p["cpf4amt"] = cpf4amt2;
                            insertbfav4p["fwlamt"] = fwlamt2;
                            insertbfav4p["allowamt"] = allowamt2;
                            insertbfav4p["medamt"] = medamt2;


                            insertbfav4p["alencash"] = alencash2;
                            insertbfav4p["accomallow"] = accomallow2;
                            insertbfav4p["adjamt"] = adjamt2;
                            insertbfav4p["cashadv"] = cashadv2;
                            insertbfav4p["cdacamt"] = cdacamt2;
                            insertbfav4p["handphoneamt"] = handphoneamt2;
                            insertbfav4p["loadamt"] = loadamt2;
                            insertbfav4p["medreim"] = medreim2;
                            insertbfav4p["mosqamt"] = mosqamt2;
                            insertbfav4p["otallowamt"] = otallowamt2;
                            insertbfav4p["sindamt"] = sindamt2;
                            insertbfav4p["supallowamt"] = supallowamt2;
                            insertbfav4p["transallowamt"] = transallowamt2;
                            insertbfav4p["transreimamt"] = transreimamt2;
                            insertbfav4p["unionfeeamt"] = unionfeeamt2;
                            insertbfav4p["varamt"] = varamt2;
                            insertbfav4p["otherallowamt"] = otallowamt2;

                            insertbfav4p["accrualStaffSal"] = dr2["accrualStaffSal"];
                            insertbfav4p["accrualsiva"] = dr2["accrualsiva"];
                            insertbfav4p["accrualCpfSdl"] = dr2["accrualCpfSdl"];
                            insertbfav4p["accrualFwl"] = dr2["accrualFwl"];

                            bfav4p.Rows.Add(insertbfav4p);
                        }
                    
                    }
                }

                //for (int i = 0; i < bfav3p.Rows.Count; i++)
                //{
                //    DataRow insertbfav4p = bfav4p.NewRow();
                                        
                //    insertbfav4p["matnum"] = "CL";
                //    insertbfav4p["rateamt"] = rateamt1;
                //    insertbfav4p["dirFeeAmt"] = dirFeeAmt1;
                //    insertbfav4p["DirRemuAmt"] = DirRemuAmt1;
                //    insertbfav4p["otamt"] = otallowamt1;
                //    insertbfav4p["bonusamt"] = bonusamt1;
                //    insertbfav4p["cpfamt"] = cpfamt1;
                //    insertbfav4p["cpfskillamt"] = cpfskillamt1;
                //    insertbfav4p["cpf3amt"] = cpf3amt1;
                //    insertbfav4p["cpf4amt"] = cpf4amt1;
                //    insertbfav4p["fwlamt"] = fwlamt1;
                //    insertbfav4p["allowamt"] = allowamt1;
                //    insertbfav4p["medamt"] = medamt1;


                //    insertbfav4p["alencash"] = alencash1;
                //    insertbfav4p["accomallow"] = accomallow1;
                //    insertbfav4p["adjamt"] = adjamt1;
                //    insertbfav4p["cashadv"] = cashadv1;
                //    insertbfav4p["cdacamt"] = cdacamt1;
                //    insertbfav4p["handphoneamt"] = handphoneamt1;
                //    insertbfav4p["loadamt"] = loadamt1;
                //    insertbfav4p["medreim"] = medreim1;
                //    insertbfav4p["mosqamt"] = mosqamt1;
                //    insertbfav4p["otallowamt"] = otallowamt1;
                //    insertbfav4p["sindamt"] = sindamt1;
                //    insertbfav4p["supallowamt"] = supallowamt1;
                //    insertbfav4p["transallowamt"] = transallowamt1;
                //    insertbfav4p["transreimamt"] = transreimamt1;
                //    insertbfav4p["unionfeeamt"] = unionfeeamt1;
                //    insertbfav4p["varamt"] = varamt1;
                //    insertbfav4p["otherallowamt"] = otallowamt1;

                //    bfav4p.Rows.Add(insertbfav4p);
                //}

                //for (int i = 0; i < 1; i++)
                //{
                //    DataRow insertbfav4p = bfav4p.NewRow();

                //    insertbfav4p["matnum"] = "HQ";
                //    insertbfav4p["rateamt"] = rateamt2;
                //    insertbfav4p["dirFeeAmt"] = dirFeeAmt2;
                //    insertbfav4p["DirRemuAmt"] = DirRemuAmt2;
                //    insertbfav4p["otamt"] = otallowamt2;
                //    insertbfav4p["bonusamt"] = bonusamt2;
                //    insertbfav4p["cpfamt"] = cpfamt2;
                //    insertbfav4p["cpfskillamt"] = cpfskillamt2;
                //    insertbfav4p["cpf3amt"] = cpf3amt2;
                //    insertbfav4p["cpf4amt"] = cpf4amt2;
                //    insertbfav4p["fwlamt"] = fwlamt2;
                //    insertbfav4p["allowamt"] = allowamt2;
                //    insertbfav4p["medamt"] = medamt2;


                //    insertbfav4p["alencash"] = alencash2;
                //    insertbfav4p["accomallow"] = accomallow2;
                //    insertbfav4p["adjamt"] = adjamt2;
                //    insertbfav4p["cashadv"] = cashadv2;
                //    insertbfav4p["cdacamt"] = cdacamt2;
                //    insertbfav4p["handphoneamt"] = handphoneamt2;
                //    insertbfav4p["loadamt"] = loadamt2;
                //    insertbfav4p["medreim"] = medreim2;
                //    insertbfav4p["mosqamt"] = mosqamt2;
                //    insertbfav4p["otallowamt"] = otallowamt2;
                //    insertbfav4p["sindamt"] = sindamt2;
                //    insertbfav4p["supallowamt"] = supallowamt2;
                //    insertbfav4p["transallowamt"] = transallowamt2;
                //    insertbfav4p["transreimamt"] = transreimamt2;
                //    insertbfav4p["unionfeeamt"] = unionfeeamt2;
                //    insertbfav4p["varamt"] = varamt2;
                //    insertbfav4p["otherallowamt"] = otallowamt2;

                //    bfav4p.Rows.Add(insertbfav4p);
                //}

            }

            

           

            



        }

        void btn_Compute3_Click(object sender, EventArgs e)
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            DataTable bfav1p = dbaccess.DataSet.Tables["bfav1p"];
            DataTable bfav2p = dbaccess.DataSet.Tables["bfav2p"];
            DataTable bfav3p = dbaccess.DataSet.Tables["bfav3p"];
            DataTable bfav4p = dbaccess.DataSet.Tables["bfav4p"];

            decimal wages1 = 0;
            decimal cpfContribution1 = 0;
            decimal cpfSkillDevLevy1 = 0;
            decimal cpfFwl1 = 0;
            decimal alEncash1 = 0;
            decimal allowances1 = 0;
            decimal medicalReim1 = 0;
           
            decimal wages2 = 0;
            decimal cpfContribution2 = 0;
            decimal cpfSkillDevLevy2 = 0;
            decimal cpfFwl2 = 0;
            decimal alEncash2 = 0;
            decimal allowances2 = 0;
            decimal medicalReim2 = 0;

            decimal directorFee = 0;
            decimal directorRenum = 0;
            decimal directorCpf = 0;

            decimal accrualStaffSal = 0;
            decimal accrualsiva = 0;
            decimal accrualCpfSdl = 0;
            decimal accrualFwl = 0;



            if (bfav4p.Rows.Count > 0)
            {
                if (bfav1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(bfav1);
                }



                string get1 = "Select Projectid,sitenum,ccnum, SUM([58889]) AS [58889],SUM([60058]) AS [60058],SUM([21124_CLNEmpYeeCPF]) AS [21124_CLNEmpYeeCPF],SUM([21124_HQEmpYeeCPF]) AS [21124_HQEmpYeeCPF], SUM([58890]) AS [58890], SUM([60009]) AS [60009],SUM([58887]) AS [58887], SUM([21103]) AS [21103], SUM([60010]) AS [60010],SUM([58886]) AS [58886],SUM([60011]) AS [60011],SUM([58892]) AS [58892],SUM([58884]) AS [58884],SUM([60102]) AS [60102],SUM([58883]) AS [58883],SUM([60103]) AS [60103] " +
                                "From "+
                                "( "+
                                "Select "+
	                            "projectid,  "+
                                "sitenum,  " +
                                "'CLN' as ccnum,  " +
	                            "CASE  "+
		                            "WHEN matnum='CL' then rateamt ELSE 0 END AS [58889], "+
	                            "CASE  "+
		                            "WHEN matnum<>'CL' then rateamt ELSE 0 END AS [60058], "+

                               "CASE  " +
                                    "WHEN matnum='CL' then cpf3amt ELSE 0 END AS [58890], " +
                                "CASE  " +
                                    "WHEN matnum<>'CL' then cpf3amt ELSE 0 END AS [60009], " +

                                 "CASE  " +
                                    "WHEN matnum='CL' then cpf4amt ELSE 0 END AS [21124_CLNEmpYeeCPF], " +
                                "CASE  " +
                                    "WHEN matnum<>'CL' then cpf4amt ELSE 0 END AS [21124_HQEmpYeeCPF], " +

                            		
                                "cpfamt AS [21103], "+                               
                            		
		                            "CASE  "+
		                            "WHEN matnum='CL' then cpfskillamt ELSE 0 END AS [58887], "+
	                            "CASE  "+
		                           " WHEN matnum<>'CL' then cpfskillamt ELSE 0 END AS [60010], "+
                            		
	                            "CASE  "+
		                            "WHEN matnum='CL' then fwlamt ELSE 0 END AS [58886], "+
	                            "CASE  "+
		                            "WHEN matnum<>'CL' then fwlamt ELSE 0 END AS [60011], "+
                            		
	                            "alEncash  AS [58892], "+
                            		
	                            "CASE  "+
		                            "WHEN matnum='CL' then allowamt ELSE 0 END AS [58884], "+
	                            "CASE  "+
		                            "WHEN matnum<>'CL' then allowamt ELSE 0 END AS [60102],	 "+	
                            		
	                            "CASE  "+
		                            "WHEN matnum='CL' then medreim ELSE 0 END AS [58883], "+
	                            "CASE  "+
		                            "WHEN matnum<>'CL' then medreim ELSE 0 END AS [60103] "+
                            		
                            "from bfav1p "+
                            ")A Group by Projectid,sitenum,ccnum ";


                // 14-12-2015@1738
                //string get1 = "Select Projectid,sitenum,ccnum, SUM([58889]) AS [58889],SUM([60058]) AS [60058],SUM([21124_CLNEmpYeeCPF]) AS [21124_CLNEmpYeeCPF],SUM([21124_HQEmpYeeCPF]) AS [21124_HQEmpYeeCPF], SUM([58890]) AS [58890], SUM([60009]) AS [60009],SUM([58887]) AS [58887],SUM([60010]) AS [60010],SUM([58886]) AS [58886],SUM([60011]) AS [60011],SUM([58892]) AS [58892],SUM([58884]) AS [58884],SUM([60102]) AS [60102],SUM([58883]) AS [58883],SUM([60103]) AS [60103] " +
                //                "From " +
                //                "( " +
                //                "Select " +
                //                "projectid,  " +
                //                "sitenum,  " +
                //                "'CLN' as ccnum,  " +
                //                "CASE  " +
                //                    "WHEN matnum='CL' then rateamt ELSE 0 END AS [58889], " +
                //                "CASE  " +
                //                    "WHEN matnum<>'CL' then rateamt ELSE 0 END AS [60058], " +

                //               "CASE  " +
                //                    "WHEN matnum='CL' then cpf3amt ELSE 0 END AS [58890], " +
                //                "CASE  " +
                //                    "WHEN matnum<>'CL' then cp3amt ELSE 0 END AS [60009], " +

                //                 "CASE  " +
                //                    "WHEN matnum='CL' then cpf4amt ELSE 0 END AS [21124_CLNEmpYeeCPF], " +
                //                "CASE  " +
                //                    "WHEN matnum<>'CL' then cpf4amt ELSE 0 END AS [21124_HQEmpYeeCPF], " +


                //                //"CASE  "+
                //    //    "WHEN matnum='CL' then cpfamt ELSE 0 END AS [58890], "+
                //    //"CASE  "+
                //    //    "WHEN matnum<>'CL' then cpfamt ELSE 0 END AS [60016], "+

                //                    "CASE  " +
                //                    "WHEN matnum='CL' then cpfskillamt ELSE 0 END AS [58887], " +
                //                "CASE  " +
                //                   " WHEN matnum<>'CL' then cpfskillamt ELSE 0 END AS [60010], " +

                //                "CASE  " +
                //                    "WHEN matnum='CL' then fwlamt ELSE 0 END AS [58886], " +
                //                "CASE  " +
                //                    "WHEN matnum<>'CL' then fwlamt ELSE 0 END AS [60011], " +

                //                "alEncash  AS [58892], " +

                //                "CASE  " +
                //                    "WHEN matnum='CL' then allowamt ELSE 0 END AS [58884], " +
                //                "CASE  " +
                //                    "WHEN matnum<>'CL' then allowamt ELSE 0 END AS [60102],	 " +

                //                "CASE  " +
                //                    "WHEN matnum='CL' then medreim ELSE 0 END AS [58883], " +
                //                "CASE  " +
                //                    "WHEN matnum<>'CL' then medreim ELSE 0 END AS [60103] " +

                //            "from bfav1p " +
                //            ")A Group by Projectid,sitenum,ccnum ";


                DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, get1);

                dt1.TableName = "TempDt1";

                if (this.dbaccess.DataSet.Tables.Contains("TempDt1"))
                {
                    this.dbaccess.DataSet.Tables["TempDt1"].Dispose();
                    this.dbaccess.DataSet.Tables.Remove("TempDt1");
                    DataTable TempDt1 = dt1.Copy();
                    TempDt1.TableName = "TempDt1";
                    this.dbaccess.DataSet.Tables.Add(TempDt1);
                }
                else
                {
                    DataTable TempDt1 = dt1.Copy();
                    TempDt1.TableName = "TempDt1";
                    this.dbaccess.DataSet.Tables.Add(TempDt1);
                }

                //string get2 = "Select Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as ,Sum() as  from TempDt1";

                //DataTable dt2 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, get2);



                #region Add Accounts
                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in dt1.Rows)
                    {

                        string projectod = dr1["projectid"].ToString().Trim();

                       

                        if (!BizFunctions.IsEmpty(dr1["58889"]))
                        {
                            if (Convert.ToDecimal(dr1["58889"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "58889";
                                insertbfav1_1["oridebit"] = dr1["58889"];
                                //insertbfav1_1["oricredit"] = dr1["58889"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";

                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];

                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }


                        


                        if (!BizFunctions.IsEmpty(dr1["60058"]))
                        {
                            if (Convert.ToDecimal(dr1["60058"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "60058";
                                insertbfav1_1["oridebit"] = dr1["60058"];
                                //insertbfav1_1["oricredit"] = dr1["60058"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["58890"]))
                        {
                            if (Convert.ToDecimal(dr1["58890"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "58890";
                                insertbfav1_1["oridebit"] = dr1["58890"];
                                //insertbfav1_1["oricredit"] = dr1["58890"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["detail"] = "CLN Employer CPF";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }


                        if (!BizFunctions.IsEmpty(dr1["60009"]))
                        {
                            if (Convert.ToDecimal(dr1["60009"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "60009";
                                insertbfav1_1["oridebit"] = dr1["60009"];
                                //insertbfav1_1["oricredit"] = dr1["58889"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["detail"] = "HQ Employer CPF";

                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];

                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        //if (!BizFunctions.IsEmpty(dr1["60016"]))
                        //{
                        //    if (Convert.ToDecimal(dr1["60016"]) > 0)
                        //    {
                        //        DataRow insertbfav1_1 = bfav1.NewRow();
                        //        insertbfav1_1["projectid"] = dr1["projectid"];
                        //        insertbfav1_1["accnum"] = "60016";
                        //        insertbfav1_1["oridebit"] = dr1["60016"];
                        //        //insertbfav1_1["oricredit"] = dr1["60016"];
                        //        insertbfav1_1["trandate"] = bfavh["trandate"];
                        //        insertbfav1_1["oricur"] = "SGD";
                        //        insertbfav1_1["sitenum"] = dr1["sitenum"];
                        //        insertbfav1_1["ccnum"] = dr1["ccnum"];
                        //        bfav1.Rows.Add(insertbfav1_1);
                        //    }
                        //}

                        if (!BizFunctions.IsEmpty(dr1["58887"]))
                        {
                            if (Convert.ToDecimal(dr1["58887"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "58887";
                                insertbfav1_1["oridebit"] = dr1["58887"];
                                //insertbfav1_1["oricredit"] = dr1["58887"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["60010"]))
                        {
                            if (Convert.ToDecimal(dr1["60010"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "60010";
                                insertbfav1_1["oridebit"] = dr1["60010"];
                                //insertbfav1_1["oricredit"] = dr1["60010"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["58886"]))
                        {
                            if (Convert.ToDecimal(dr1["58886"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "58886";
                                insertbfav1_1["oridebit"] = dr1["58886"];
                                //insertbfav1_1["oricredit"] = dr1["58886"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["60011"]))
                        {
                            if (Convert.ToDecimal(dr1["60011"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "60011";
                                insertbfav1_1["oridebit"] = dr1["60011"];
                                //insertbfav1_1["oricredit"] = dr1["60011"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["58892"]))
                        {
                            if (Convert.ToDecimal(dr1["58892"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "58892";
                                insertbfav1_1["oridebit"] = dr1["58892"];
                                //insertbfav1_1["oricredit"] = dr1["58892"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["58884"]))
                        {
                            if (Convert.ToDecimal(dr1["58884"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "58884";
                                insertbfav1_1["oridebit"] = dr1["58884"];
                                //insertbfav1_1["oricredit"] = dr1["58884"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["60102"]))
                        {
                            if (Convert.ToDecimal(dr1["60102"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "60102";
                                insertbfav1_1["oridebit"] = dr1["60102"];
                                //insertbfav1_1["oricredit"] = dr1["60102"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["58883"]))
                        {
                            if (Convert.ToDecimal(dr1["58883"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "58883";
                                insertbfav1_1["oridebit"] = dr1["58883"];
                                //insertbfav1_1["oricredit"] = dr1["58883"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["60103"]))
                        {
                            if (Convert.ToDecimal(dr1["60103"]) > 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "60103";
                                insertbfav1_1["oridebit"] = dr1["60103"];
                                //insertbfav1_1["oricredit"] = dr1["60103"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }


                        //////

                            //"CASE  " +
                            //        "WHEN matnum='CL' then cpf4amt ELSE 0 END AS [58889_CLNEmpYeeCPF], " +
                            //    "CASE  " +
                            //        "WHEN matnum<>'CL' then cpf4amt ELSE 0 END AS [60058_HQEmpYeeCPF], " +
                        //SUM([21124_CLNEmpYeeCPF]) AS [21124_CLNEmpYeeCPF],SUM([21124_HQEmpYeeCPF]) AS [21124_HQEmpYeeCPF]

                        if (!BizFunctions.IsEmpty(dr1["21124_CLNEmpYeeCPF"]))
                        {
                            if (Convert.ToDecimal(dr1["21124_CLNEmpYeeCPF"]) != 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "21124";
                                insertbfav1_1["oridebit"] = Math.Abs(Convert.ToDecimal(dr1["21124_CLNEmpYeeCPF"]));
                                insertbfav1_1["detail"] = "CLN Employee CPF";
                                //insertbfav1_1["oricredit"] = dr1["58883"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }

                        if (!BizFunctions.IsEmpty(dr1["21124_HQEmpYeeCPF"]))
                        {
                            if (Convert.ToDecimal(dr1["21124_HQEmpYeeCPF"]) != 0)
                            {
                                DataRow insertbfav1_1 = bfav1.NewRow();
                                insertbfav1_1["projectid"] = dr1["projectid"];
                                insertbfav1_1["accnum"] = "21124";
                                insertbfav1_1["oridebit"] = Math.Abs(Convert.ToDecimal(dr1["21124_HQEmpYeeCPF"]));
                                insertbfav1_1["detail"] = "HQ Employee CPF";
                                //insertbfav1_1["oricredit"] = dr1["60103"];
                                insertbfav1_1["trandate"] = bfavh["trandate"];
                                insertbfav1_1["oricur"] = "SGD";
                                insertbfav1_1["sitenum"] = dr1["sitenum"];
                                insertbfav1_1["ccnum"] = dr1["ccnum"];
                                bfav1.Rows.Add(insertbfav1_1);
                            }
                        }






                    }

                }
                #endregion
            }


            #region Add Accrual
       

            #endregion

                #region Old Code

            //    foreach (DataRow dr3 in bfav4p.Rows)
            //    {
            //        if (dr3.RowState != DataRowState.Deleted)
            //        {
                                               

            //            if (dr3["matnum"].ToString().Trim().ToUpper() == "CL")
            //            {

            //                wages1 = Convert.ToDecimal(dr3["rateamt"]) + Convert.ToDecimal(dr3["otamt"]) + Convert.ToDecimal(dr3["bonusamt"]); //58889
            //                cpfContribution1 = Convert.ToDecimal(dr3["cpfamt"]); //58890
            //                cpfSkillDevLevy1 = Convert.ToDecimal(dr3["cpfskillamt"]); //58887
            //                cpfFwl1 = Convert.ToDecimal(dr3["fwlamt"]); //58886
            //                alEncash1 = Convert.ToDecimal(dr3["alEncash"]); //58892
            //                allowances1 = Convert.ToDecimal(dr3["allowamt"]); //58884
            //                medicalReim1 = Convert.ToDecimal(dr3["medreim"]); //58883
                           
            //            }
            //            else
            //            {
            //                directorFee = Convert.ToDecimal(dr3["dirFeeAmt"]); //60046
            //                directorRenum = Convert.ToDecimal(dr3["DirRemuAmt"]); //60014
            //                wages2 = Convert.ToDecimal(dr3["rateamt"]) + Convert.ToDecimal(dr3["otamt"]) + Convert.ToDecimal(dr3["bonusamt"]); //60058
            //                cpfContribution2 = Convert.ToDecimal(dr3["cpfamt"]); //60016
            //                cpfSkillDevLevy2 = Convert.ToDecimal(dr3["cpfskillamt"]); //60010
            //                cpfFwl2 = Convert.ToDecimal(dr3["fwlamt"]); //60011
            //                alEncash2 = Convert.ToDecimal(dr3["alEncash"]); //58892
            //                allowances2 = Convert.ToDecimal(dr3["allowamt"]); //60102
            //                medicalReim2 = Convert.ToDecimal(dr3["medreim"]); //60103                        
            //            }

            //            if (BizFunctions.IsEmpty(dr3["accrualStaffSal"]))
            //            {
            //                dr3["accrualStaffSal"] = 0;
            //            }

            //            if (BizFunctions.IsEmpty(dr3["accrualsiva"]))
            //            {
            //                dr3["accrualsiva"] = 0;
            //            }

            //            if (BizFunctions.IsEmpty(dr3["accrualCpfSdl"]))
            //            {
            //                dr3["accrualCpfSdl"] = 0;
            //            }

            //            if (BizFunctions.IsEmpty(dr3["accrualFwl"]))
            //            {
            //                dr3["accrualFwl"] = 0;
            //            }


            //            //decimal accrualStaffSal = 0;
            //            //decimal accrualsiva = 0;
            //            //decimal accrualCpfSdl = 0;
            //            //decimal accrualFwl = 0;


            //            dr3["accrualStaffSal"] = wages1 + alEncash1 + allowances1 + wages2 + alEncash2 + allowances2;
            //            dr3["accrualCpfSdl"] = cpfContribution1 + cpfContribution2;
            //            dr3["accrualFwl"] = cpfFwl1 + cpfFwl2;


            //            accrualStaffSal = accrualStaffSal + Convert.ToDecimal(dr3["accrualStaffSal"]);
            //            accrualsiva = accrualsiva + Convert.ToDecimal(dr3["accrualsiva"]);
            //            accrualCpfSdl = accrualCpfSdl + Convert.ToDecimal(dr3["accrualCpfSdl"]);
            //            accrualFwl = accrualFwl + Convert.ToDecimal(dr3["accrualFwl"]);

            //        }


            //        #region Cleaners
            //        if (wages1 > 0)
            //        {
            //            DataRow insertbfav1_1 = bfav1.NewRow();
            //            insertbfav1_1["projectid"] = dr3["projectid"];
            //            insertbfav1_1["accnum"] = "58889";
            //            insertbfav1_1["oridebit"] = wages1;
            //            insertbfav1_1["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_1);
            //        }
            //        if (cpfContribution1 > 0)
            //        {
            //            DataRow insertbfav1_2 = bfav1.NewRow();
            //            insertbfav1_2["projectid"] = dr3["projectid"];
            //            insertbfav1_2["accnum"] = "58890";
            //            insertbfav1_2["oridebit"] = cpfContribution1;
            //            insertbfav1_2["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_2);
            //        }
            //        if (cpfSkillDevLevy1 > 0)
            //        {
            //            DataRow insertbfav1_3 = bfav1.NewRow();
            //            insertbfav1_3["projectid"] = dr3["projectid"];
            //            insertbfav1_3["accnum"] = "58887";
            //            insertbfav1_3["oridebit"] = cpfSkillDevLevy1;
            //            insertbfav1_3["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_3);
            //        }
            //        if (cpfFwl1 > 0)
            //        {
            //            DataRow insertbfav1_4 = bfav1.NewRow();
            //            insertbfav1_4["projectid"] = dr3["projectid"];
            //            insertbfav1_4["accnum"] = "58886";
            //            insertbfav1_4["oridebit"] = cpfFwl1;
            //            insertbfav1_4["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_4);
            //        }
            //        if (alEncash1 > 0)
            //        {
            //            DataRow insertbfav1_5 = bfav1.NewRow();
            //            insertbfav1_5["projectid"] = dr3["projectid"];
            //            insertbfav1_5["accnum"] = "58892";
            //            insertbfav1_5["oridebit"] = alEncash1;
            //            insertbfav1_5["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_5);
            //        }
            //        if (allowances1 > 0)
            //        {
            //            DataRow insertbfav1_6 = bfav1.NewRow();
            //            insertbfav1_6["projectid"] = dr3["projectid"];
            //            insertbfav1_6["accnum"] = "58884";
            //            insertbfav1_6["oridebit"] = allowances1;
            //            insertbfav1_6["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_6);
            //        }
            //        if (medicalReim1 > 0)
            //        {
            //            DataRow insertbfav1_7 = bfav1.NewRow();
            //            insertbfav1_7["projectid"] = dr3["projectid"];
            //            insertbfav1_7["accnum"] = "58883";
            //            insertbfav1_7["oridebit"] = medicalReim1;
            //            insertbfav1_7["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_7);
            //        }
            //        #endregion

            //        #region HQ
            //        if (wages2 > 0)
            //        {
            //            DataRow insertbfav1_1 = bfav1.NewRow();
            //            insertbfav1_1["projectid"] = dr3["projectid"];
            //            insertbfav1_1["accnum"] = "60058";
            //            insertbfav1_1["oridebit"] = wages2;
            //            insertbfav1_1["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_1);
            //        }
            //        if (cpfContribution2 > 0)
            //        {
            //            DataRow insertbfav1_2 = bfav1.NewRow();
            //            insertbfav1_2["projectid"] = dr3["projectid"];
            //            insertbfav1_2["accnum"] = "60016";
            //            insertbfav1_2["oridebit"] = cpfContribution2;
            //            insertbfav1_2["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_2);
            //        }
            //        if (cpfSkillDevLevy2 > 0)
            //        {
            //            DataRow insertbfav1_3 = bfav1.NewRow();
            //            insertbfav1_3["projectid"] = dr3["projectid"];
            //            insertbfav1_3["accnum"] = "60010";
            //            insertbfav1_3["oridebit"] = cpfSkillDevLevy2;
            //            insertbfav1_3["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_3);
            //        }
            //        if (cpfFwl2 > 0)
            //        {
            //            DataRow insertbfav1_4 = bfav1.NewRow();
            //            insertbfav1_4["projectid"] = dr3["projectid"];
            //            insertbfav1_4["accnum"] = "60011";
            //            insertbfav1_4["oridebit"] = cpfFwl2;
            //            insertbfav1_4["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_4);
            //        }
            //        if (alEncash2 > 0)
            //        {
            //            DataRow insertbfav1_5 = bfav1.NewRow();
            //            insertbfav1_5["projectid"] = dr3["projectid"];
            //            insertbfav1_5["accnum"] = "58892";
            //            insertbfav1_5["oridebit"] = alEncash2;
            //            insertbfav1_5["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_5);
            //        }
            //        if (allowances2 > 0)
            //        {
            //            DataRow insertbfav1_6 = bfav1.NewRow();
            //            insertbfav1_6["projectid"] = dr3["projectid"];
            //            insertbfav1_6["accnum"] = "60102";
            //            insertbfav1_6["oridebit"] = allowances2;
            //            insertbfav1_6["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_6);
            //        }
            //        if (medicalReim2 > 0)
            //        {
            //            DataRow insertbfav1_7 = bfav1.NewRow();
            //            insertbfav1_7["projectid"] = dr3["projectid"];
            //            insertbfav1_7["accnum"] = "60103";
            //            insertbfav1_7["oridebit"] = medicalReim2;
            //            insertbfav1_7["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_7);
            //        }
            //        if (directorFee > 0)
            //        {
            //            DataRow insertbfav1_8 = bfav1.NewRow();
            //            insertbfav1_8["projectid"] = dr3["projectid"];
            //            insertbfav1_8["accnum"] = "60046";
            //            insertbfav1_8["oridebit"] = directorFee;
            //            insertbfav1_8["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_8);
            //        }
            //        if (directorRenum > 0)
            //        {
            //            DataRow insertbfav1_9 = bfav1.NewRow();
            //            insertbfav1_9["projectid"] = dr3["projectid"];
            //            insertbfav1_9["accnum"] = "60014";
            //            insertbfav1_9["oridebit"] = directorRenum;
            //            insertbfav1_9["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_9);
            //        }
            //        #endregion

            //        #region Accrual
            //        if (accrualStaffSal > 0)
            //        {
            //            //58889,58892,58884
            //            DataRow insertbfav1_10 = bfav1.NewRow();
            //            //insertbfav1_6["projectid"] = dr3["projectid"];
            //            insertbfav1_10["accnum"] = "21124";
            //            insertbfav1_10["oricredit"] = accrualStaffSal;
            //            insertbfav1_10["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_10);
            //        }
            //        if (accrualsiva > 0)
            //        {
            //            DataRow insertbfav1_11 = bfav1.NewRow();
            //            //insertbfav1_7["projectid"] = dr3["projectid"];
            //            insertbfav1_11["accnum"] = "21129";
            //            insertbfav1_11["oricredit"] = accrualsiva;
            //            insertbfav1_11["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_11);
            //        }
            //        if (accrualCpfSdl > 0)
            //        {
            //            DataRow insertbfav1_12 = bfav1.NewRow();
            //            //insertbfav1_8["projectid"] = dr3["projectid"];
            //            insertbfav1_12["accnum"] = "21103";
            //            insertbfav1_12["oricredit"] = accrualCpfSdl;
            //            insertbfav1_12["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_12);
            //        }
            //        if (accrualFwl > 0)
            //        {
            //            DataRow insertbfav1_13 = bfav1.NewRow();
            //            //insertbfav1_9["projectid"] = dr3["projectid"];
            //            insertbfav1_13["accnum"] = "21104";
            //            insertbfav1_13["oricredit"] = accrualFwl;
            //            insertbfav1_13["oricur"] = "SGD";
            //            bfav1.Rows.Add(insertbfav1_13);
            //        }
            //    }
            //}

            //        #endregion
                #endregion

            decimal A21124 = 0;
            decimal A21103 = 0;
            decimal A21104 = 0;


            for (int i = 0; i < bfav1.Rows.Count; i++)
            {
                if (bfav1.Rows[i].RowState != DataRowState.Deleted)
                {
                  
                      //"CASE  " +
                      //              "WHEN matnum<>'CL' then cp3amt ELSE 0 END AS [60009], " +

                      //           "CASE  " +
                      //              "WHEN matnum='CL' then cpf4amt ELSE 0 END AS [21124_CLNEmpYeeCPF], " +
                      //          "CASE  " +
                      //              "WHEN matnum<>'CL' then cpf4amt ELSE 0 END AS [21124_HQEmpYeeCPF], " +
                       
                    
                    //21124=58889+58892+58884+60058+60102

                    //21103=58890
                    //21104=58886+60011

                    if (bfav1.Rows[i]["accnum"].ToString().Trim() == "58889" || bfav1.Rows[i]["accnum"].ToString().Trim() == "58892" || bfav1.Rows[i]["accnum"].ToString().Trim() == "58884" || bfav1.Rows[i]["accnum"].ToString().Trim() == "60058" || bfav1.Rows[i]["accnum"].ToString().Trim() == "60102")
                    {
                        A21124 = A21124 + Convert.ToDecimal(bfav1.Rows[i]["oridebit"]);
                    }

                    //if (bfav1.Rows[i]["accnum"].ToString().Trim() == "58890" )
                    //{
                    //    A21103 = A21103 + Convert.ToDecimal(bfav1.Rows[i]["oridebit"]);
                    //}

                    //if (bfav1.Rows[i]["accnum"].ToString().Trim() == "21103")
                    //{
                    //    A21103 = A21103 + Convert.ToDecimal(bfav1.Rows[i]["oridebit"]);
                    //}

                    if (bfav1.Rows[i]["accnum"].ToString().Trim() == "58886" || bfav1.Rows[i]["accnum"].ToString().Trim() == "60011")
                    {
                        A21104 = A21104 + Convert.ToDecimal(bfav1.Rows[i]["oridebit"]);
                    }



                 
                   
                }             
            }


            DataTable TempDt1_1 = this.dbaccess.DataSet.Tables["TempDt1"];


            if (TempDt1_1.Rows.Count > 0)
            {

                foreach (DataRow dr1 in TempDt1_1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        A21103 = A21103 + Convert.ToDecimal(dr1["21103"]);
                    }
                }
            }

            if (A21124 > 0)
            {
                DataRow insertbfav1_1 = bfav1.NewRow();
                insertbfav1_1["accnum"] = "21124";
                insertbfav1_1["oricredit"] = A21124;
                insertbfav1_1["oricur"] = "SGD";
                insertbfav1_1["trandate"] = bfavh["trandate"];
                bfav1.Rows.Add(insertbfav1_1);
            }

            if (A21103 > 0)
            {
                DataRow insertbfav1_1 = bfav1.NewRow();
                insertbfav1_1["accnum"] = "21103";
                insertbfav1_1["oricredit"] = A21103;
                insertbfav1_1["oricur"] = "SGD";
                insertbfav1_1["trandate"] = bfavh["trandate"];
                bfav1.Rows.Add(insertbfav1_1);
            }

            if (A21104 > 0)
            {
                DataRow insertbfav1_1 = bfav1.NewRow();
                insertbfav1_1["accnum"] = "21104";
                insertbfav1_1["oricredit"] = A21104;
                insertbfav1_1["oricur"] = "SGD";
                insertbfav1_1["trandate"] = bfavh["trandate"];
                bfav1.Rows.Add(insertbfav1_1);
            }
        
        }


        void btn_Compute4_Click(object sender, EventArgs e)
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

            decimal A21124 = 0;
            decimal A21103 = 0;
            decimal A21104 = 0;


            for (int i = 0; i < bfav1.Rows.Count; i++)
            {
                if (bfav1.Rows[i].RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(bfav1.Rows[i]["trandate"]))
                    {
                        bfav1.Rows[i]["trandate"] = bfavh["trandate"];
                    }
                    //21124=58889+58892+58884
                    //21103=58890
                    //21104=58886+60011

                    if (bfav1.Rows[i]["accnum"].ToString().Trim() == "58889" || bfav1.Rows[i]["accnum"].ToString().Trim() == "58892" || bfav1.Rows[i]["accnum"].ToString().Trim() == "58884")
                    {
                        A21124 = A21124 + Convert.ToDecimal(bfav1.Rows[i]["oridebit"]);
                    }

                    if (bfav1.Rows[i]["accnum"].ToString().Trim() == "58890")
                    {
                        A21103 = A21103 + Convert.ToDecimal(bfav1.Rows[i]["oridebit"]);
                    }

                    if (bfav1.Rows[i]["accnum"].ToString().Trim() == "58886" || bfav1.Rows[i]["accnum"].ToString().Trim() == "60011")
                    {
                        A21104 = A21104 + Convert.ToDecimal(bfav1.Rows[i]["oridebit"]);
                    }


                    if (A21124 > 0)
                    {
                        DataRow insertbfav1_1 = bfav1.NewRow();
                        insertbfav1_1["accnum"] = "21124";
                        insertbfav1_1["oricredit"] = A21124;
                        insertbfav1_1["oricur"] = "SGD";
                        bfav1.Rows.Add(insertbfav1_1);
                    }

                    if (A21103 > 0)
                    {
                        DataRow insertbfav1_1 = bfav1.NewRow();
                        insertbfav1_1["accnum"] = "21103";
                        insertbfav1_1["oricredit"] = A21103;
                        insertbfav1_1["oricur"] = "SGD";
                        bfav1.Rows.Add(insertbfav1_1);
                    }

                    if (A21104 > 0)
                    {
                        DataRow insertbfav1_1 = bfav1.NewRow();
                        insertbfav1_1["accnum"] = "21104";
                        insertbfav1_1["oricredit"] = A21104;
                        insertbfav1_1["oricur"] = "SGD";
                        bfav1.Rows.Add(insertbfav1_1);
                    }
                }
            }
            //if (bfav1.Rows.Count > 0)
            //{
            //    foreach (DataRow dr1 in bfav1.Rows)
            //    {
            //        if (dr1.RowState != DataRowState.Deleted)
            //        {
            //            if (BizFunctions.IsEmpty(dr1["trandate"]))
            //            {
            //                dr1["trandate"] = bfavh["trandate"];
            //            }
            //            //21124=58889+58892+58884
            //            //21103=58890
            //            //21104=58886+60011

            //            if (dr1["accnum"].ToString().Trim() == "58889" || dr1["accnum"].ToString().Trim() == "58892" || dr1["accnum"].ToString().Trim() == "58884")
            //            {
            //                A21124 = A21124 + Convert.ToDecimal(dr1["oridebit"]);
            //            }

            //            if (dr1["accnum"].ToString().Trim() == "58890")
            //            {
            //                A21103 = A21103 + Convert.ToDecimal(dr1["oridebit"]);
            //            }

            //            if (dr1["accnum"].ToString().Trim() == "58886" || dr1["accnum"].ToString().Trim() == "60011")
            //            {
            //                A21104 = A21104 + Convert.ToDecimal(dr1["oridebit"]);
            //            }


            //            if (A21124 > 0)
            //            {
            //                DataRow insertbfav1_1 = bfav1.NewRow();
            //                insertbfav1_1["accnum"] = "21124";
            //                insertbfav1_1["oricredit"] = A21124;
            //                insertbfav1_1["oricur"] = "SGD";
            //                bfav1.Rows.Add(insertbfav1_1);
            //            }

            //            if (A21103 > 0)
            //            {
            //                DataRow insertbfav1_1 = bfav1.NewRow();
            //                insertbfav1_1["accnum"] = "21103";
            //                insertbfav1_1["oricredit"] = A21103;
            //                insertbfav1_1["oricur"] = "SGD";
            //                bfav1.Rows.Add(insertbfav1_1);
            //            }

            //            if (A21104 > 0)
            //            {
            //                DataRow insertbfav1_1 = bfav1.NewRow();
            //                insertbfav1_1["accnum"] = "21104";
            //                insertbfav1_1["oricredit"] = A21104;
            //                insertbfav1_1["oricur"] = "SGD";
            //                bfav1.Rows.Add(insertbfav1_1);
            //            }

            //        }
            //    }
            //}
        }

        private void GetDetail1Sum()
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            DataTable bfav1p = dbaccess.DataSet.Tables["bfav1p"];
            DataTable bfav2p = dbaccess.DataSet.Tables["bfav2p"];
            DataTable bfav3p = dbaccess.DataSet.Tables["bfav3p"];

            int month, year = 0;

            month = Convert.ToDateTime(bfavh["trandate"]).Month;
            year = Convert.ToDateTime(bfavh["trandate"]).Year;


            TextBox tb_t1 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t1") as TextBox;
            TextBox tb_t2 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t2") as TextBox;
            TextBox tb_t3 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t3") as TextBox;
            TextBox tb_t4 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t4") as TextBox;
            TextBox tb_t5 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t5") as TextBox;
            TextBox tb_t6 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t6") as TextBox;
            TextBox tb_t7 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t7") as TextBox;
            TextBox tb_t8 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t8") as TextBox;
            TextBox tb_t9 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t9") as TextBox;
            TextBox tb_t10 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t10") as TextBox;
            TextBox tb_t11 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t11") as TextBox;
            TextBox tb_t12 = BizXmlReader.CurrentInstance.GetControl(employeeFormName, "tb_t12") as TextBox;

            if (bfav1p.Rows.Count > 0)
            {
                DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "Select distinct sitenum from bfav1p");

                decimal rate = 0;
                decimal dirFee = 0;
                decimal dirRum = 0;
                decimal ot = 0;
                decimal bonus = 0;
                decimal cpf = 0;
                decimal cpfskill = 0;
                decimal cpf3 = 0;
                decimal cpf4 = 0;
                decimal allow = 0;
                decimal fwl = 0;
                decimal med = 0;

                //                    <DetailGridColumnInfo MappingName="rateamt"			HeaderText="Rate"		Width="100"	/>

                //<DetailGridColumnInfo MappingName="dirFeeAmt"			HeaderText="Dir Fee"		Width="100"	/>
                //<DetailGridColumnInfo MappingName="DirRemuAmt"			HeaderText="Dir Rum"		Width="100"	/>
                //<DetailGridColumnInfo MappingName="otamt"			HeaderText="OT"		Width="100"	/>
                //<DetailGridColumnInfo MappingName="bonusamt"			HeaderText="Bonus"		Width="100"	/>
                //    <DetailGridColumnInfo MappingName="cpfamt"			HeaderText="CPF"				Width="100"		TextAlign="Right"	Format="#,##0.00"/>
                //<DetailGridColumnInfo MappingName="cpfskillamt"			HeaderText="CPF-Skill"		Width="100"	/>
                //<DetailGridColumnInfo MappingName="cpf3amt"			HeaderText="CPF3"		Width="100"	/>
                //<DetailGridColumnInfo MappingName="cpf4amt"			HeaderText="CPF4"		Width="100"	/>    
                //    <DetailGridColumnInfo MappingName="fwlamt"				HeaderText="FWL"				Width="100"/>
                //    <DetailGridColumnInfo MappingName="allowamt"			HeaderText="Allowance"			Width="100"		/>
                //    <DetailGridColumnInfo MappingName="medamt"			HeaderText="Medical"			Width="50"	/>	


                foreach (DataRow dr1 in bfav1p.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(dr1["rateamt"]))
                        {
                            dr1["rateamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["dirFeeAmt"]))
                        {
                            dr1["dirFeeAmt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["DirRemuAmt"]))
                        {
                            dr1["DirRemuAmt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["otamt"]))
                        {
                            dr1["otamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["bonusamt"]))
                        {
                            dr1["bonusamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["cpfamt"]))
                        {
                            dr1["cpfamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["cpfskillamt"]))
                        {
                            dr1["cpfskillamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["cpf3amt"]))
                        {
                            dr1["cpf3amt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["cpf3amt"]))
                        {
                            dr1["cpf3amt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["cpf4amt"]))
                        {
                            dr1["cpf4amt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["fwlamt"]))
                        {
                            dr1["fwlamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["allowamt"]))
                        {
                            dr1["allowamt"] = 0;
                        }
                        if (BizFunctions.IsEmpty(dr1["medamt"]))
                        {
                            dr1["medamt"] = 0;
                        }


                        rate = rate + Convert.ToDecimal(dr1["rateamt"]);
                        dirFee = dirFee + Convert.ToDecimal(dr1["dirFeeAmt"]);
                        dirRum = dirRum + Convert.ToDecimal(dr1["DirRemuAmt"]);
                        ot = ot + Convert.ToDecimal(dr1["otamt"]);
                        bonus = bonus + Convert.ToDecimal(dr1["bonusamt"]);
                        cpf = cpf + Convert.ToDecimal(dr1["cpfamt"]);
                        cpfskill = cpfskill + Convert.ToDecimal(dr1["cpfskillamt"]);
                        cpf3 = cpf3 + Convert.ToDecimal(dr1["cpf3amt"]);
                        cpf4 = cpf4 + Convert.ToDecimal(dr1["cpf4amt"]);
                        allow = allow + Convert.ToDecimal(dr1["allowamt"]);
                        fwl = fwl + Convert.ToDecimal(dr1["fwlamt"]);
                        med = med + Convert.ToDecimal(dr1["medamt"]);

                    }
                }

                tb_t1.Text = rate.ToString();
                tb_t2.Text = dirFee.ToString();
                tb_t3.Text = dirRum.ToString();
                tb_t4.Text = ot.ToString();
                tb_t5.Text = bonus.ToString();
                tb_t6.Text = cpf.ToString();
                tb_t7.Text = cpfskill.ToString();
                tb_t8.Text = cpf3.ToString();
                tb_t9.Text = cpf4.ToString();
                tb_t10.Text = fwl.ToString();
                tb_t11.Text = allow.ToString(); 
                tb_t12.Text = med.ToString();


            }
        }

        void btn_Extract_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
                DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
                decimal lineNo = 0;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
                openFileDialog.Filter = "XLS(*.XLS;*.XLSX;)|*.xls;*.xlsx;|All Files|*.*";
                openFileDialog.ShowDialog();



                //foreach (DataRow drPOR1 in por1.Rows)
                //{
                //    if (drPOR1.RowState != DataRowState.Deleted)
                //    {
                //        if (BizFunctions.IsEmpty(drPOR1["line"].ToString()) || (decimal)drPOR1["line"] <= 0)
                //        {
                //            lineNo = lineNo + 100;
                //            drPOR1["line"] = lineNo;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region openFileDialog
        protected void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string getHemphTmpData = "Select empnum,matnum from hemph ";

                this.dbaccess.ReadSQL("HemphTmpData", getHemphTmpData);

                DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
                DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
                 DataTable bfav1p = dbaccess.DataSet.Tables["bfav1p"];
                string Path = (sender as OpenFileDialog).FileName;
                //Read data from Excel,and return the dataset
                DataSet ds = ExcelToDS(Path, "XSL", 0);



                if (bfav1.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(bfav1);
                }

                if (bfav1p.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(bfav1p);
                }
          

                for (int i = 0; i < ds.Tables["XSL"].Rows.Count; i++)
                {
                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][0]))
                    {
                        DataRow insertBfav1p = bfav1p.NewRow();

                        insertBfav1p["empnum"] = ds.Tables["XSL"].Rows[i][0].ToString();
                        insertBfav1p["empname"] = ds.Tables["XSL"].Rows[i][1].ToString();

                        if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][0]))
                        {
                            if(ds.Tables["XSL"].Rows[i][0].ToString().Trim().ToUpper() == "NONE")
                            {
                                insertBfav1p["matnum"] = GetDesignation(ds.Tables["XSL"].Rows[i][0].ToString().Trim());
                            }
                            else
                            {
                                insertBfav1p["matnum"] = GetDesignation(ds.Tables["XSL"].Rows[i][0].ToString());
                            }
                        }
                        
                        insertBfav1p["rateamt"] = ds.Tables["XSL"].Rows[i][14].ToString();


                        insertBfav1p["cpf3amt"] = ds.Tables["XSL"].Rows[i][16].ToString(); //Employer CPF
                        insertBfav1p["cpf4amt"] = ds.Tables["XSL"].Rows[i][17].ToString(); //Employee CPF

                        insertBfav1p["cpfamt"] = ds.Tables["XSL"].Rows[i][18].ToString();

                        insertBfav1p["dirFeeAmt"] = ds.Tables["XSL"].Rows[i][27].ToString();
                        insertBfav1p["otamt"] = ds.Tables["XSL"].Rows[i][7].ToString();
                        insertBfav1p["bonusamt"] = ds.Tables["XSL"].Rows[i][24].ToString();
                        insertBfav1p["alencash"] = ds.Tables["XSL"].Rows[i][19].ToString();
                        insertBfav1p["accomallow"] = ds.Tables["XSL"].Rows[i][20].ToString();
                        insertBfav1p["adjamt"] = ds.Tables["XSL"].Rows[i][21].ToString();
                        insertBfav1p["cashadv"] = ds.Tables["XSL"].Rows[i][25].ToString();
                        insertBfav1p["cdacamt"] = ds.Tables["XSL"].Rows[i][26].ToString();
                        insertBfav1p["handphoneamt"] = ds.Tables["XSL"].Rows[i][28].ToString();
                        insertBfav1p["loadamt"] = ds.Tables["XSL"].Rows[i][29].ToString();
                        insertBfav1p["medamt"] = ds.Tables["XSL"].Rows[i][30].ToString();
                        insertBfav1p["mosqamt"] = ds.Tables["XSL"].Rows[i][31].ToString();
                        insertBfav1p["otallowamt"] = ds.Tables["XSL"].Rows[i][32].ToString();
                        insertBfav1p["sindamt"] = ds.Tables["XSL"].Rows[i][35].ToString();
                        insertBfav1p["supallowamt"] = ds.Tables["XSL"].Rows[i][36].ToString();
                        insertBfav1p["transallowamt"] = ds.Tables["XSL"].Rows[i][37].ToString();
                        insertBfav1p["transreimamt"] = ds.Tables["XSL"].Rows[i][38].ToString();
                        insertBfav1p["unionfeeamt"] = ds.Tables["XSL"].Rows[i][39].ToString();
                        insertBfav1p["varamt"] = ds.Tables["XSL"].Rows[i][40].ToString();
                        insertBfav1p["otherallowamt"] = ds.Tables["XSL"].Rows[i][41].ToString();
                        //insertBfav1p[""] = ds.Tables["XSL"].Rows[i][4].ToString();
                        //insertBfav1p[""] = ds.Tables["XSL"].Rows[i][4].ToString();
                        //insertBfav1p[""] = ds.Tables["XSL"].Rows[i][4].ToString();

                        //insertBfav1p["fwlamt"] = ds.Tables["XSL"].Rows[i][4].ToString();
                        //insertBfav1p["allowamt"] = ds.Tables["XSL"].Rows[i][5].ToString();
                        //insertBfav1p["medamt"] = ds.Tables["XSL"].Rows[i][6].ToString();
                        insertBfav1p["allowamt"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][28].ToString()) + Convert.ToDecimal(ds.Tables["XSL"].Rows[i][32].ToString()) + Convert.ToDecimal(ds.Tables["XSL"].Rows[i][36].ToString()) + +Convert.ToDecimal(ds.Tables["XSL"].Rows[i][37].ToString()) + Convert.ToDecimal(ds.Tables["XSL"].Rows[i][38].ToString()) + Convert.ToDecimal(ds.Tables["XSL"].Rows[i][41].ToString());
                        insertBfav1p["sitenum"] = ds.Tables["XSL"].Rows[i][42].ToString();
                        insertBfav1p["projectid"] = BizLogicTools.Tools.GetProjectIDFromSitenum(ds.Tables["XSL"].Rows[i][42].ToString(), this.dbaccess);


                        bfav1p.Rows.Add(insertBfav1p);
                    }
                }
       
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        private string GetDesignation(string empnum)
        {
            string Designation = "";

            string get1 = "Select matnum from HemphTmpData where empnum='"+empnum+"' ";

            DataTable dt1 = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, get1);

            if (dt1 != null)
            {
                if (dt1.Rows.Count > 0)
                {
                    if (!BizFunctions.IsEmpty(dt1.Rows[0]["matnum"]))
                    {
                        Designation = dt1.Rows[0]["matnum"].ToString().Trim().ToUpper();
                    }
                }
            }

            return Designation;
        }

        #region ExcelToDS
        public DataSet ExcelToDS(string Path, string tablename, int sheetIndex)
        {
            string os_platform = System.Environment.OSVersion.Platform.ToString();
            string strConn = "";

            if (BizLogicTools.Tools.Platform == "x86")
            {
                strConn = "Provider = Microsoft.Jet.OLEDB.4.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            }
            else
            {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            }
     
            
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

		void Voucher_FAV_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			switch (e.Column.ColumnName)
			{
				case "jvtemplate":
					#region Steph - Copy from JV Template
					this.Save = MessageBox.Show("Overwrite existing Journal details?", "Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

					DataSet dstempfavt1 = this.dbaccess.ReadSQLTemp("favt1", "SELECT * FROM favt1 WHERE refnum='" + bfavh["jvtemplate"].ToString().Trim()+ "'");
					DataTable dttempfavt1 = dstempfavt1.Tables["favt1"];

					if (this.Save == DialogResult.Yes)
					{
						DataRow tempbfav1;
						BizFunctions.DeleteAllRows(bfav1);
						//Get bug1 data
						for (int i = 0; i < dttempfavt1.Rows.Count; i++)
						{

							tempbfav1 = bfav1.NewRow();

							if (dttempfavt1.Rows[i].RowState != DataRowState.Deleted)
							{
								tempbfav1["vouchernum"] = dttempfavt1.Rows[i]["vouchernum"];
								tempbfav1["accnum"] = dttempfavt1.Rows[i]["accnum"];
								tempbfav1["desc"] = dttempfavt1.Rows[i]["desc"];
								tempbfav1["trandate"] = bfavh["trandate"];
								tempbfav1["oridebit"] = dttempfavt1.Rows[i]["oricredit"];
								tempbfav1["oricredit"] = dttempfavt1.Rows[i]["oridebit"];
								tempbfav1["detail"] = dttempfavt1.Rows[i]["detail"];
								tempbfav1["apnum"] = dttempfavt1.Rows[i]["apnum"];
								tempbfav1["arnum"] = dttempfavt1.Rows[i]["arnum"];
								tempbfav1["oricur"] = dttempfavt1.Rows[i]["oricur"];
								tempbfav1["exrate"] = dttempfavt1.Rows[i]["exrate"];
								tempbfav1["oricur"] = dttempfavt1.Rows[i]["oricur"];

								bfav1.Rows.Add(tempbfav1.ItemArray);
							}
						}
					}
					else
					{
						bfavh["refnum"] = this.prevJour;
					}
					refreshBfav1();
					#endregion
					break;	
				case "oricur":
					#region Steph - get latest exrate from exr table
					bfavh["exrate"] = BizAccounts.GetExRate(dbaccess, bfavh["oricur"].ToString().Trim(), BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(bfavh["trandate"])));
					#endregion
					break;
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
					txtOricredit.Text= string.Format("{0:0.00}", getCreditValue);
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
		}

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick(sender, e);
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

            GenerateDetail();

		}

		protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			e.Handle = true;

			if ((bool)bfavh["aropen"] != true)
			{
				if ((bool)bfavh["apopen"] != true)
				{
					if ((bool)bfavh["cshopen"] != true)
					{
						DataTable checkBalance = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT SUM(postamt) FROM [bfav1] HAVING SUM(postamt)<>0");
						if (checkBalance.Rows.Count > 0)
						{
							MessageBox.Show("The Voucher Posting Is Not Balance!", "Voucher Is Not Confirmed!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

							e.Handle = false;
						}
					}
				}
			}

			dbaccess.ReadSQL("acm", "select * from acm");

			if(e.Handle != false)
			{
				#region Steph - Posting Time!

				if ((bool)bfavh["aropen"] == true)
				{
					#region Steph - Saving into ARD
					string saveArd = "select refnum,vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,"+
						" sum(oricredit) as oricredit,arnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period "+
						" from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' "+
						" AND accnum in (select accnum from [acm] where acctype = '7') "+
						" GROUP BY refnum,vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate,arnum";
					DataTable getArd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveArd);

					foreach (DataRow dr in getArd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addArd = ard.Rows.Add(new object[] { });
							addArd["refnum"] = dr["refnum"];
							addArd["docunum"] = dr["vouchernum"];
							addArd["trandate"] = dr["trandate"];
							addArd["chknum"] = dr["chknum"];
							addArd["accnum"] = dr["accnum"];
							addArd["locno"] = dr["locno"];
							addArd["deptno"] = dr["deptno"];
							addArd["detail"] = dr["detail"];
							addArd["oridebit"] = dr["oridebit"];
							addArd["oricredit"] = dr["oricredit"];
							addArd["arnum"] = dr["arnum"];
							addArd["invnum"] = dr["invnum"];
							addArd["oldref"] = dr["oldref"];
							addArd["invdate"] = dr["invdate"];
							addArd["lgr"] = "ARD";
							addArd["coy"] = "SAF";
							addArd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addArd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addArd["period"] = dr["period"];
							addArd["oricur"] = dr["oricur"];
							addArd["exrate"] = dr["exrate"];
							addArd["gstamt"] = 0;
							addArd["exramt"] = 0;
							addArd["oriamt"] = addArd["doriamt"];
							addArd["postamt"] = addArd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in ard.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}


					#endregion
				}
				else if ((bool)bfavh["apopen"] == true)
				{
					#region Steph - Saving into APD
					string saveApd = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,"+
						" sum(oricredit) as oricredit,apnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period "+
						" from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "' "+
						" AND accnum in (select accnum from [acm] where acctype = '12') "+
						" GROUP BY vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate,apnum";

					DataTable getApd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveApd);

					foreach (DataRow dr in getApd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addApd = apd.Rows.Add(new object[] { });
							addApd["docunum"] = dr["vouchernum"];
							addApd["trandate"] = dr["trandate"];
							addApd["chknum"] = dr["chknum"];
							addApd["accnum"] = dr["accnum"];
							addApd["locno"] = dr["locno"];
							addApd["deptno"] = dr["deptno"];
							addApd["detail"] = dr["detail"];
							addApd["oridebit"] = dr["oridebit"];
							addApd["oricredit"] = dr["oricredit"];
							addApd["apnum"] = dr["apnum"];
							addApd["invnum"] = dr["invnum"];
							addApd["oldref"] = dr["oldref"];
							addApd["invdate"] = dr["invdate"];
							addApd["lgr"] = "APD";
							addApd["coy"] = "SAF";
							addApd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addApd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addApd["period"] = dr["period"];
							addApd["oricur"] = dr["oricur"];
							addApd["exrate"] = dr["exrate"];
							addApd["gstamt"] = 0;
							addApd["exramt"] = 0;
							addApd["oriamt"] = addApd["doriamt"];
							addApd["postamt"] = addApd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in apd.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion
				}
				else if ((bool)bfavh["cshopen"] == true)
				{
					#region Steph - Saving into Csh
					string saveCsh = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '5' OR acctype = '6') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getCsh = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveCsh);
                    DataTable getCsh = this.dbaccess.ReadSQLTemp("getCsh", saveCsh).Tables[0];
					foreach (DataRow dr in getCsh.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							DataRow addCsh = csh.Rows.Add(new object[] { });
							addCsh["docunum"] = dr["vouchernum"];
							addCsh["trandate"] = dr["trandate"];
							addCsh["chknum"] = dr["chknum"];
							addCsh["accnum"] = dr["accnum"];
							addCsh["locno"] = dr["locno"];
							addCsh["deptno"] = dr["deptno"];
							addCsh["detail"] = dr["detail"];
							addCsh["oridebit"] = dr["oridebit"];
							addCsh["oricredit"] = dr["oricredit"];
							addCsh["apnum"] = dr["apnum"];
							addCsh["lgr"] = "CSH";
							addCsh["coy"] = "SAF";
							addCsh["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addCsh["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addCsh["period"] = dr["period"];
							addCsh["oricur"] = dr["oricur"];
							addCsh["exrate"] = dr["exrate"];
						}
					}

					foreach (DataRow dr2 in csh.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}
					#endregion
				}
				else if ((bool)bfavh["glopen"] == true)
				{
					#region Steph - Saving all into GLD table.
					string saveGld = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getGld = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveGld);
                    DataTable getGld = this.dbaccess.ReadSQLTemp("getGld",saveGld).Tables[0];
					foreach (DataRow dr in getGld.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addGld = gld.Rows.Add(new object[] { });
							addGld["docunum"] = dr["vouchernum"];
							addGld["trandate"] = dr["trandate"];
							addGld["accnum"] = dr["accnum"];
							addGld["locno"] = dr["locno"];
							addGld["deptno"] = dr["deptno"];
							addGld["detail"] = dr["detail"];
							addGld["oridebit"] = dr["oridebit"];
							addGld["oricredit"] = dr["oricredit"];
							addGld["arnum"] = dr["arnum"];
							addGld["lgr"] = "GLD";
							addGld["coy"] = "SAF";
							addGld["period"] = dr["period"];
							addGld["oricur"] = dr["oricur"];
							addGld["exrate"] = dr["exrate"];
							addGld["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addGld["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
						}
					}

					foreach (DataRow dr2 in gld.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion
				}
				else
				{
					#region Steph - Saving into ARD
					dbaccess.ReadSQL("acm", "select * from acm");
					string saveArd = "select refnum,vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '7') GROUP BY refnum,vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate";
                    DataSet ds=this.dbaccess.ReadSQLTemp("getArd",saveArd);
                    DataTable getArd=ds.Tables["getArd"];
                    //DataTable getArd = BizFunctions..ExecuteQuery(dbaccess.DataSet, saveArd);

					foreach (DataRow dr in getArd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addArd = ard.Rows.Add(new object[] { });
							addArd["refnum"] = dr["refnum"];
							addArd["docunum"] = dr["vouchernum"];
							addArd["trandate"] = dr["trandate"];
							addArd["chknum"] = dr["chknum"];
							addArd["accnum"] = dr["accnum"];
							addArd["locno"] = dr["locno"];
							addArd["deptno"] = dr["deptno"];
							addArd["detail"] = dr["detail"];
							addArd["oridebit"] = dr["oridebit"];
							addArd["oricredit"] = dr["oricredit"];
							addArd["arnum"] = dr["arnum"];
							addArd["invnum"] = dr["invnum"];
							addArd["oldref"] = dr["oldref"];
							addArd["invdate"] = dr["invdate"];
							addArd["lgr"] = "ARD";
							addArd["coy"] = "SAF";
							addArd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addArd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addArd["period"] = dr["period"];
							addArd["oricur"] = dr["oricur"];
							addArd["exrate"] = dr["exrate"];
							addArd["gstamt"] = 0;
							addArd["exramt"] = 0;
							addArd["oriamt"] = addArd["doriamt"];
							addArd["postamt"] = addArd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in ard.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}


					#endregion

					#region Steph - Saving into APD
					string saveApd = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,oldref,invdate,oricur,exrate,MAX(period) as period from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '12') GROUP BY vouchernum,trandate,chknum,detail,invnum,oldref,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getApd = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveApd);
                    DataTable getApd = this.dbaccess.ReadSQLTemp("getApd", saveApd).Tables[0];

					foreach (DataRow dr in getApd.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addApd = apd.Rows.Add(new object[] { });
							addApd["docunum"] = dr["vouchernum"];
							addApd["trandate"] = dr["trandate"];
							addApd["chknum"] = dr["chknum"];
							addApd["accnum"] = dr["accnum"];
							addApd["locno"] = dr["locno"];
							addApd["deptno"] = dr["deptno"];
							addApd["detail"] = dr["detail"];
							addApd["oridebit"] = dr["oridebit"];
							addApd["oricredit"] = dr["oricredit"];
							addApd["apnum"] = dr["apnum"];
							addApd["invnum"] = dr["invnum"];
							addApd["oldref"] = dr["oldref"];
							addApd["invdate"] = dr["invdate"];
							addApd["lgr"] = "APD";
							addApd["coy"] = "SAF";
							addApd["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addApd["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addApd["period"] = dr["period"];
							addApd["oricur"] = dr["oricur"];
							addApd["exrate"] = dr["exrate"];
							addApd["gstamt"] = 0;
							addApd["exramt"] = 0;
							addApd["oriamt"] = addApd["doriamt"];
							addApd["postamt"] = addApd["dpostamt"];
						}
					}

					foreach (DataRow dr2 in apd.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion

					#region Steph - Saving into Csh
					string saveCsh = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '5' OR acctype = '6') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getCsh = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveCsh);
                    DataTable getCsh = this.dbaccess.ReadSQLTemp("getCsh", saveCsh).Tables[0];
					foreach (DataRow dr in getCsh.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							DataRow addCsh = csh.Rows.Add(new object[] { });
							addCsh["docunum"] = dr["vouchernum"];
							addCsh["trandate"] = dr["trandate"];
							addCsh["chknum"] = dr["chknum"];
							addCsh["accnum"] = dr["accnum"];
							addCsh["locno"] = dr["locno"];
							addCsh["deptno"] = dr["deptno"];
							addCsh["detail"] = dr["detail"];
							addCsh["oridebit"] = dr["oridebit"];
							addCsh["oricredit"] = dr["oricredit"];
							addCsh["apnum"] = dr["apnum"];
							addCsh["lgr"] = "CSH";
							addCsh["coy"] = "SAF";
							addCsh["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addCsh["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addCsh["period"] = dr["period"];
							addCsh["oricur"] = dr["oricur"];
							addCsh["exrate"] = dr["exrate"];
						}
					}

					foreach (DataRow dr2 in csh.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}
					#endregion

					#region Steph - Saving into SIV
					string saveSiv = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '0' OR acctype = '1') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getSiv = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveSiv);
                    DataTable getSiv = this.dbaccess.ReadSQLTemp("getSiv", saveSiv).Tables[0];

					foreach (DataRow dr in getSiv.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addSiv = siv1.Rows.Add(new object[] { });
							addSiv["docunum"] = dr["vouchernum"];
							addSiv["trandate"] = dr["trandate"];
							addSiv["accnum"] = dr["accnum"];
							addSiv["locno"] = dr["locno"];
							addSiv["deptno"] = dr["deptno"];
							addSiv["detail"] = dr["detail"];
							addSiv["oridebit"] = dr["oridebit"];
							addSiv["oricredit"] = dr["oricredit"];
							addSiv["arnum"] = dr["arnum"];
							addSiv["invnum"] = dr["invnum"];
							addSiv["invdate"] = dr["invdate"];
							addSiv["coy"] = "SAF";
							addSiv["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addSiv["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addSiv["period"] = dr["period"];
							addSiv["oricur"] = dr["oricur"];
							addSiv["exrate"] = dr["exrate"];
							addSiv["gstamt"] = 0;
							addSiv["gstper"] = 0;
							addSiv["oriamt"] = addSiv["doriamt"];
							addSiv["postamt"] = addSiv["dpostamt"];
						}
					}

					foreach (DataRow dr2 in siv1.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}

					#endregion

					#region Steph - Saving into PIV
					string savePiv = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(apnum) as apnum,invnum,invdate,oricur,exrate,MAX(period) as period from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "' AND accnum in (select accnum from [acm] where acctype = '2' OR acctype = '3') GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate";

                    //DataTable getPiv = BizFunctions.ExecuteQuery(dbaccess.DataSet, savePiv);
                    DataTable getPiv =this.dbaccess.ReadSQLTemp("getPiv",savePiv).Tables[0];

					foreach (DataRow dr in getPiv.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addPiv = piv1.Rows.Add(new object[] { });
							addPiv["docunum"] = dr["vouchernum"];
							addPiv["trandate"] = dr["trandate"];
							//addPiv["chknum"] = dr["chknum"];
							addPiv["accnum"] = dr["accnum"];
							addPiv["locno"] = dr["locno"];
							addPiv["deptno"] = dr["deptno"];
							addPiv["detail"] = dr["detail"];
							addPiv["oridebit"] = dr["oridebit"];
							addPiv["oricredit"] = dr["oricredit"];
							addPiv["apnum"] = dr["apnum"];
							addPiv["invnum"] = dr["invnum"];
							addPiv["invdate"] = dr["invdate"];
							addPiv["lgr"] = "PIV";
							addPiv["coy"] = "SAF";
							addPiv["doriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addPiv["dpostamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
							addPiv["period"] = dr["period"];
							addPiv["oricur"] = dr["oricur"];
							addPiv["exrate"] = dr["exrate"];
							addPiv["gstamt"] = 0;
							addPiv["gstper"] = 0;
							addPiv["oriamt"] = addPiv["doriamt"];
							addPiv["postamt"] = addPiv["dpostamt"];
						}
					}

					foreach (DataRow dr2 in piv1.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/flag/status/created/modified");
						}
					}
					#endregion

					#region Steph - Saving all into GLD table.
                    string saveGld = "select vouchernum,trandate,chknum,accnum,locno,deptno,detail,sum(oridebit) as oridebit,sum(oricredit) as oricredit,MAX(arnum) as arnum,invnum,invdate,oricur,exrate,MAX(period) as period,ccnum,projectid,sitenum from [bfav1] where refnum = '" + bfavh["refnum"].ToString().Trim() + "' GROUP BY vouchernum,trandate,chknum,detail,invnum,invdate,accnum,locno,deptno,oricur,exrate,ccnum,projectid,sitenum";

                    //DataTable getGld = BizFunctions.ExecuteQuery(dbaccess.DataSet, saveGld);
                    DataTable getGld = this.dbaccess.ReadSQLTemp("getGld", saveGld).Tables[0];
					foreach (DataRow dr in getGld.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{

							DataRow addGld = gld.Rows.Add(new object[] { });
							addGld["docunum"] = dr["vouchernum"];
							addGld["trandate"] = dr["trandate"];
							addGld["accnum"] = dr["accnum"];
							addGld["locno"] = dr["locno"];
							addGld["deptno"] = dr["deptno"];
							addGld["detail"] = dr["detail"];
							addGld["oridebit"] = dr["oridebit"];
							addGld["oricredit"] = dr["oricredit"];
							addGld["arnum"] = dr["arnum"];
							addGld["lgr"] = "GLD";
							addGld["coy"] = "SAF";
							addGld["period"] = dr["period"];
							addGld["oricur"] = dr["oricur"];
							addGld["exrate"] = dr["exrate"];

                            addGld["ccnum"] = dr["ccnum"];
                            addGld["projectid"] = dr["projectid"];
                            addGld["sitenum"] = dr["sitenum"];

							addGld["oriamt"] = (decimal)dr["oridebit"] - (decimal)dr["oricredit"];
							addGld["postamt"] = ((decimal)dr["oridebit"] - (decimal)dr["oricredit"]) * (decimal)dr["exrate"];
						}
					}

					foreach (DataRow dr2 in gld.Rows)
					{
						if (dr2.RowState != DataRowState.Deleted)
						{
							BizFunctions.UpdateDataRow(bfavh, dr2, "refnum/user/coy/flag/status/created/modified");
						}
					}
					#endregion
				}


				#endregion
			}
		}

		#endregion

		protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick(sender, e);

			DataRow bfavh = e.DBAccess.DataSet.Tables["bfavh"].Rows[0];
			Hashtable selectedCollection = new Hashtable();

			switch (e.ReportName)
			{
				case "Journal":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("arm", "SELECT top 1 * FROM arm where arnum='" + bfavh["arnum"].ToString().Trim() + "'");
					selectedCollection.Add("bfav1det", "SELECT bfav1.accnum AS accnum,acm.accname AS accname,"+
						" bfav1.detail AS detail,bfav1.oricur AS oricur,bfav1.oricredit AS oricredit,bfav1.oridebit AS oridebit FROM bfav1 "+
						" LEFT OUTER JOIN acm on acm.accnum = bfav1.accnum "+
						" where bfav1.refnum='" + bfavh["refnum"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;
			}            
		}

		#region Refresh

		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable ard = dbaccess.DataSet.Tables["ard"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable siv1 = dbaccess.DataSet.Tables["siv1"];
			DataTable acm = dbaccess.DataSet.Tables["acm"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			if (bfavh["status"].ToString().Trim() == Common.DEFAULT_DOCUMENT_STATUSP)
			{
				txtCurrentAction.Text = "Voucher is closed!";
				return;
			}

			bfavh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(bfavh["trandate"]));
			bfavh["coy"] = "SAF";

			refreshBfav1();

		}
		#endregion

		#region Reopen
		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
				
			DataRow bfavh = this.dbaccess.DataSet.Tables["bfavh"].Rows[0];

            if (BizValidate.ChkPeriodLocked(e.DBAccess, bfavh["period"].ToString()))
            {
                MessageBox.Show("Period has been Closed !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
            }


			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM SIV1 WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM ARD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM PIV1 WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM APD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM CSH" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + bfavh["refnum"].ToString().Trim() + "'");

		}
		#endregion



		#endregion


        #region DocumentF2
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            base.AddDocumentF2Condition(sender, e);

            switch (e.ControlName)
            {
      
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            switch (e.ControlName)
            {
              


            }
        }

        #endregion

        #region DetailF2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            switch (e.MappingName)
            {
               
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            switch (e.MappingName)
            {

                case "empnum":
                    e.CurrentRow["matnum"] = e.F2CurrentRow["matnum"];
                    break;
            }
        }

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
			txtOricur = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "bfavh_oricur") as TextBox;
			//txtExrate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_exrate") as TextBox;
			//cboOricur = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "cbo_oricur") as  ComboBox;
			txtCurrentAction = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "txt_currentAction") as Label;

			btnAdd= BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Add") as Button;
			btnGetOB = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_getOB") as Button;
			btnUpdate = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Update") as Button;
			btnInsertTrans = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_Insert") as Button;
			btnNextTrans = BizXmlReader.CurrentInstance.GetControl(this.formdDetailName, "btn_NextTrans") as Button;
		}

		private void calcTotalDebitCredit ()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			setDefaults(dbaccess.DataSet, "bfav1");
	
			decimal countTotalDebit = 0;
			decimal countTotalCredit = 0;
			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					//countTotalDebit += (decimal)dr["oridebit"];
					//countTotalCredit += (decimal)dr["oricredit"];

					if (Convert.ToDecimal(dr["postamt"])> 0)
					{
						countTotalDebit += (decimal)dr["postamt"];
					}
					else
					{
						countTotalCredit += Math.Abs((decimal)dr["postamt"]);
					}

				}
			}

			bfavh["debit"] = countTotalDebit;
			bfavh["credit"] = countTotalCredit;
			bfavh["oriamt"] = countTotalDebit - countTotalCredit;
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
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0]; 
			string strExistVoucher = "select vouchernum from bfav1 where refnum = '" + bfavh["refnum"].ToString().Trim() + "'";
			DataTable existVoucher = BizFunctions.ExecuteQuery(dbaccess.DataSet, strExistVoucher);
			bool checkOnce = true;
			foreach (DataRow dr in existVoucher.Rows)
			{
				if (checkOnce == true)
				{
					if(skipValidate == false)
					if (txtVouchernum.Text.Trim() == dr["vouchernum"].ToString().Trim())
					{
						MessageBox.Show("This Journal No. exist in this voucher! Please proceed to key in the Transactions Info!");
						#region Steph - Copied from F11 in order to get the values for all the field in Voucher Info for Existing Data
						vouchernum = txtVouchernum.Text.ToString();
						txtVouchernum.Focus();
						DataTable editHeader = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select vouchernum,apname,arname,detail,trandate,chknum,invdate from bfav1 where vouchernum = '" + vouchernum + "'");
						if (editHeader.Rows.Count > 0)
						{
							txtVouchernum.Text = editHeader.Rows[0]["vouchernum"].ToString();
							cboApname.Text = editHeader.Rows[0]["apname"].ToString();
							cboArname.Text = editHeader.Rows[0]["arname"].ToString();
							//txtDesc.Text = editHeader.Rows[0]["detail"].ToString();
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
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			if (e.KeyCode == Keys.Enter)
			{
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

			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

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
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			if (vouchernum != "")
			{
				foreach (DataRow dr in bfav1.Rows)
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
							//dr["detail"] = txtDesc.Text;
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
				DataTable editHeader = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select vouchernum,apname,arname,detail,trandate,chknum,invdate from bfav1 where vouchernum = '" + vouchernum + "'");
				if (editHeader.Rows.Count > 0)
				{
					txtVouchernum.Text = editHeader.Rows[0]["vouchernum"].ToString();
					cboApname.Text = editHeader.Rows[0]["apname"].ToString();
					cboArname.Text = editHeader.Rows[0]["arname"].ToString();
					//txtDesc.Text = editHeader.Rows[0]["detail"].ToString();
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
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

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
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			
			
			if ((bool)bfavh["aropen"] == true)
			{
				FilterOP.ShowDialog();
				getAROpening();

			}
			if ((bool)bfavh["apopen"] == true)
			{
				FilterOP.ShowDialog();
				getAPOpening();

			}
			if ((bool)bfavh["cshopen"] == true)
			{
				FilterOP.ShowDialog();
				getCSHOpening();
			}
			if ((bool)bfavh["glopen"] == true)
			{
				FilterOP.ShowDialog();
				getGLOpening();
			}

		}

		private void add_Click()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			if (txtVouchernum.Text.ToString().Trim() == "")
			{
				MessageBox.Show("Voucher No. Cannot Be Empty!");
				txtVouchernum.Focus();
			}

			//steph_29 Mar 2010_1433 - to take from exr table if the exrate is zero

			bfavh["exrate"] = BizAccounts.GetExRate(dbaccess, bfavh["oricur"].ToString().Trim(), BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(bfavh["trandate"])));

			if (txtVouchernum.Text.ToString().Trim() != "")
			{
				grpHeaderVoucherInfo.Enabled = false;
				grpHeaderTransInfo.Enabled = true;

				txtCurrentAction.Text = "OPEN For Transactions Info Data Entry";
				btnInsertTrans.Enabled = true;
				btnNextTrans.Enabled = true;


				calcTotalDebitCredit();

				cboAccname.Focus();
			}
		}

		private DataRow getcurrentrow(DataGrid datagrid)
		{
			CurrencyManager cm = (CurrencyManager)datagrid.BindingContext[datagrid.DataSource, datagrid.DataMember];
			DataRowView drv = cm.Current as DataRowView;
			DataRow dr = drv.Row;

			return dr;
		}

		private void cmdUpdate_Click(object sender, EventArgs e)
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			dgDetail.Enabled = true;
			

			if (blnNew)
			{
				DataRow addRow = bfav1.Rows.Add(new object[] { });
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
				addRow["locno"] = bfavh["locno"];
				addRow["deptno"] = bfavh["deptno"];
				addRow["chknum"] = txtChequeno.Text;
				addRow["oridebit"] = GetNumeric(txtOridebit.Text);
				addRow["oricredit"] = GetNumeric(txtOricredit.Text);
				addRow["exrate"] =bfavh["exrate"];
				addRow["oricur"] = bfavh["oricur"];
				addRow["invnum"] = txtVouchernum.Text;
				addRow["invdate"] = dtInvdate.Value;
			}
			else
			{
                //Jason :18062015: Error after adding some columns
                //int intRow = dgDetail.CurrentRowIndex;
                //DataRow currentRow = getcurrentrow(dgDetail);
                //dgDetail[intRow, 2] = txtVouchernum.Text;
                //dgDetail[intRow, 3] = dtTrandate.Value;
                //if (cboAccname.Text != "")
                //{
                //    dgDetail[intRow, 4] = cboAccname["accnum"];
                //    dgDetail[intRow, 5] = cboAccname["accname"];
                //}
                //dgDetail[intRow, 6] = GetNumeric(txtOridebit.Text);
                //dgDetail[intRow, 7] = GetNumeric(txtOricredit.Text);			
                //dgDetail[intRow, 8] = txtDesc.Text;
                //if (cboApname.Text != "")
                //{
                //    dgDetail[intRow, 9] = cboApname["apnum"];
                //    dgDetail[intRow, 10] = cboApname["apname"];
                //}
                //if (cboArname.Text != "")
                //{
                //    dgDetail[intRow, 11] = cboArname["arnum"];
                //    dgDetail[intRow, 12] = cboArname["arname"];
                //}
                //dgDetail[intRow, 13] = txtVouchernum.Text;
                //dgDetail[intRow, 14] = dtInvdate.Value;
                //dgDetail[intRow, 15] = txtChequeno.Text;
                //dgDetail[intRow, 16] = bfavh["oricur"];
                //dgDetail[intRow, 17] = bfavh["exrate"];
                //dgDetail[intRow, 18] = bfavh["locno"];
                //dgDetail[intRow, 19] = bfavh["deptno"];
				
                //currentRow["vouchernum"] = txtVouchernum.Text;


                int intRow = dgDetail.CurrentRowIndex;
                DataRow currentRow = getcurrentrow(dgDetail);
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
                dgDetail[intRow, 15] = bfavh["oricur"];
                dgDetail[intRow, 16] = bfavh["exrate"];
                dgDetail[intRow, 17] = bfavh["locno"];
                dgDetail[intRow, 18] = bfavh["deptno"];

                currentRow["vouchernum"] = txtVouchernum.Text;


                    //txtVouchernum.Text = dgDetail[intRow, 1].ToString();
                    //dtTrandate.Text = dgDetail[intRow, 2].ToString();
                    //txtAccname.Text = dgDetail[intRow, 3].ToString();
                    //cboAccname.Text = dgDetail[intRow, 4].ToString();
                    //txtOridebit.Text = dgDetail[intRow, 5].ToString();
                    //txtOricredit.Text = dgDetail[intRow, 6].ToString();
                    //txtDesc.Text = dgDetail[intRow, 7].ToString();
                    //txtApname.Text = dgDetail[intRow, 8].ToString();
                    //cboApname.Text = dgDetail[intRow, 9].ToString();
                    //txtArname.Text = dgDetail[intRow, 10].ToString();
                    //cboArname.Text = dgDetail[intRow, 11].ToString();
                    //dtInvdate.Text = dgDetail[intRow, 13].ToString();
                    //txtChequeno.Text = dgDetail[intRow, 14].ToString();
                    //bfavh["oricur"] = dgDetail[intRow, 15].ToString();
                    //bfavh["exrate"] = GetNumeric(dgDetail[intRow, 16].ToString());
                    //bfavh["locno"] = dgDetail[intRow, 17].ToString();
                    //bfavh["deptno"] = dgDetail[intRow, 18].ToString();
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
			//txtDesc.Enabled = true;
			dtInvdate.Enabled = true;


			btnUpdate.Enabled = false;
			btnAdd.Enabled = true;
			blnNew = true;
			dgDetail.Enabled = true;

			refreshBfav1();

			txtCurrentAction.Text = "OPEN For Voucher Info Data Entry";
			
		}
		private void cmdInsert_Click(object sender, EventArgs e)
		{
			DataTable bfav1 =  dbaccess.DataSet.Tables["bfav1"];
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			dgDetail.Enabled = true;
	
			if (blnNew)
			{
				DataRow addRow = bfav1.Rows.Add(new object[] { });
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
				addRow["locno"] = bfavh["locno"];
				addRow["deptno"] = bfavh["deptno"];
				addRow["chknum"] = txtChequeno.Text;
				addRow["oridebit"] = GetNumeric(txtOridebit.Text);
				addRow["oricredit"] = GetNumeric(txtOricredit.Text);
				addRow["exrate"] = bfavh["exrate"];
				addRow["oricur"] = bfavh["oricur"] ;
				addRow["invnum"] = txtVouchernum.Text;
				addRow["invdate"] = dtInvdate.Value;

			}
			else
			{
				int intRow = dgDetail.CurrentRowIndex;

				dgDetail[intRow, 2] = txtVouchernum.Text;
				dgDetail[intRow, 3] = dtTrandate.Value;
				dgDetail[intRow, 4] = cboAccname["accnum"];
				dgDetail[intRow, 5] = cboAccname["accname"];
				dgDetail[intRow, 6] = GetNumeric(txtOridebit.Text);
				dgDetail[intRow, 7] = GetNumeric(txtOricredit.Text);
				dgDetail[intRow, 8] = txtDesc.Text;
				dgDetail[intRow, 9] = cboApname["apnum"];
				dgDetail[intRow, 10] = cboApname["apname"];
				dgDetail[intRow, 11] = cboArname["arnum"];
				dgDetail[intRow, 12] = cboArname["arname"];
				dgDetail[intRow, 13] = txtVouchernum.Text;
				dgDetail[intRow, 14] = dtInvdate.Value;
				dgDetail[intRow, 15] = txtChequeno.Text;
				dgDetail[intRow, 16] = bfavh["oricur"];
				dgDetail[intRow, 17] = bfavh["exrate"];
				dgDetail[intRow, 18] = bfavh["locno"];
				dgDetail[intRow, 19] = bfavh["deptno"];
			}
			calcTotalDebitCredit();
			ClearEntry();
			cboAccname.Focus();
			blnNew = true;
			dgDetail.Enabled = true;

			refreshBfav1();
		}
		#endregion

		private int intGetLine()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			int intValue = 0;

			foreach (DataRow dr in bfav1.Rows)
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
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
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
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			cboAccname.Text= "";
			txtDesc.Text = "";
			txtAccname.Text = "";
			txtOridebit.Text = "0.00";
			txtOricredit.Text = "0.00";
			bfavh["oricur"] = "SGD";
			bfavh["exrate"] = 0;
			blnNew = true;
		}
	
		private void Addrow_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				int intRow;
				DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
				intRow = dgDetail.CurrentRowIndex + 1;
				if (dgDetail.CurrentRowIndex == getTotalRows() - 1)
				{
					DataRow addRow = bfav1.Rows.Add(new object[] { });
					addRow["deliverydate"] = dbaccess.DataSet.Tables["bfavh"].Rows[0]["deliverydate"];
				}
				dgDetail.CurrentCell = new DataGridCell(intRow, 1);
			}
		}

		private int getTotalRows()
		{
			int intRow = 0;
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
			foreach (DataRow dr in bfav1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					intRow = intRow + 1;
				}
			}
			return intRow;
		}

		private void DeleteCheckItemsOnBFAV1()
		{
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

			BizFunctions.DeleteRow(bfav1, " mark=true");
		}

		private void MakeLostFocusEvent()
		{
			txtOridebit.LostFocus+=new EventHandler(txtOridebit_LostFocus);
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
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];

			int intRow = dgDetail.CurrentRowIndex;

			txtVouchernum.Focus();

            //Jason 18062015 : Error after adding new columns
            //txtVouchernum.Text			= dgDetail[intRow, 2].ToString();
            //dtTrandate.Text				= dgDetail[intRow, 3].ToString();
            //txtAccname.Text				= dgDetail[intRow, 4].ToString();
            //cboAccname.Text				= dgDetail[intRow, 5].ToString();
            //txtOridebit.Text				= dgDetail[intRow, 6].ToString();
            //txtOricredit.Text				= dgDetail[intRow, 7].ToString();
            //txtDesc.Text					= dgDetail[intRow, 8].ToString();
            //txtApname.Text				= dgDetail[intRow, 9].ToString();
            //cboApname.Text				= dgDetail[intRow, 10].ToString();
            //txtArname.Text				= dgDetail[intRow, 11].ToString();
            //cboArname.Text				= dgDetail[intRow, 12].ToString();
            //dtInvdate.Text				= dgDetail[intRow, 14].ToString();
            //txtChequeno.Text				= dgDetail[intRow, 15].ToString();
            //bfavh["oricur"]					= dgDetail[intRow, 16].ToString();
            //bfavh["exrate"]				= GetNumeric(dgDetail[intRow, 17].ToString());
            //bfavh["locno"]					= dgDetail[intRow, 18].ToString();
            //bfavh["deptno"]				= dgDetail[intRow, 19].ToString();


            txtVouchernum.Text = dgDetail[intRow, 1].ToString();
            dtTrandate.Text = dgDetail[intRow, 2].ToString();
            txtAccname.Text = dgDetail[intRow, 3].ToString();
            cboAccname.Text = dgDetail[intRow, 4].ToString();
            txtOridebit.Text = dgDetail[intRow, 5].ToString();
            txtOricredit.Text = dgDetail[intRow, 6].ToString();
            txtDesc.Text = dgDetail[intRow, 7].ToString();
            txtApname.Text = dgDetail[intRow, 8].ToString();
            cboApname.Text = dgDetail[intRow, 9].ToString();
            txtArname.Text = dgDetail[intRow, 10].ToString();
            cboArname.Text = dgDetail[intRow, 11].ToString();
            dtInvdate.Text = dgDetail[intRow, 13].ToString();
            txtChequeno.Text = dgDetail[intRow, 14].ToString();
            bfavh["oricur"] = dgDetail[intRow, 15].ToString();
            bfavh["exrate"] = GetNumeric(dgDetail[intRow, 16].ToString());
            bfavh["locno"] = dgDetail[intRow, 17].ToString();
            bfavh["deptno"] = dgDetail[intRow, 18].ToString();


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
		
		private void refreshBfav1()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
					
			#region Refresh for detail transaction BFAV1
			foreach (DataRow dr in bfav1.Rows)
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

					#region Steph -  To get pd from pd (nonYear) table.
					dr["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(dr["trandate"]));
					#endregion
					
					//steph_29 Mar 2010_1433 - to take from exr table if the exrate is zero.
					if (dr["exrate"] == System.DBNull.Value || Convert.ToDecimal(dr["exrate"]) == 0)
					{
						dr["exrate"] = BizAccounts.GetExRate(dbaccess, dr["oricur"].ToString(), BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(dr["trandate"])));
					}
                    if (dr["oridebit"]==System.DBNull.Value)
                    {
                        dr["oridebit"] = 0.00;
                    }
                    if (dr["oricredit"] == System.DBNull.Value)
                    {
                        dr["oricredit"] = 0.00;
                    }

					dr["oriamt"] = Convert.ToDecimal(dr["oridebit"]) - Convert.ToDecimal(dr["oricredit"]);
					dr["postamt"] = BizFunctions.Round(Convert.ToDecimal(dr["oriamt"]) * Convert.ToDecimal(dr["exrate"]));
				}
			}
			calcTotalDebitCredit();
			#endregion
		}

		public void getAROpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
		

			dbaccess.ReadSQL("getAR", "SELECT arnum,invnum,invdate,locno,deptno,oricur,exrate,accnum,sum(oriamt) as oriamt,"+
				" sum(postamt) as postamt "+
				" from ard" + lastYear +
				" WHERE arnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and arnum<= '" + FilterOP.CodeToValue.ToString().Trim() +"' GROUP BY arnum,invnum,invdate,oricur,exrate,accnum,locno,deptno");
			DataTable getAR = dbaccess.DataSet.Tables["getAR"];
			foreach (DataRow dr in getAR.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addArd = bfav1.Rows.Add(new object[] { });
					addArd["vouchernum"] = dr["invnum"];
					addArd["trandate"] = bfavh["trandate"];
					addArd["coy"] = "SAF";
					addArd["arnum"] = dr["arnum"];
					addArd["invnum"] = dr["invnum"];
					addArd["invdate"] = dr["invdate"];
					addArd["locno"] = dr["locno"];
					addArd["deptno"] = dr["deptno"];
					addArd["oricur"] = dr["oricur"];
					addArd["accnum"] = dr["accnum"];
					addArd["detail"] = bfavh["detail"];
					if ((decimal)dr["oriamt"] > 0)
					{
						addArd["oridebit"] = dr["oriamt"];
					}
					else
					{
						addArd["oricredit"] = System.Math.Abs((decimal)dr["oriamt"]);
					}
					addArd["exrate"] = dr["exrate"];

				}
			}

			refreshBfav1();

		}

		public void getAPOpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];


			dbaccess.ReadSQL("getAP", "SELECT apnum,invnum,invdate,locno,deptno,oricur,exrate,accnum,sum(oriamt) as oriamt,"+
				" sum(postamt) as postamt from apd" + lastYear + 
				" WHERE apnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and apnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' "+
				" GROUP BY apnum,invnum,invdate,oricur,exrate,accnum,locno,deptno");
			DataTable getAP = dbaccess.DataSet.Tables["getAP"];
			foreach (DataRow dr in getAP.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addApd = bfav1.Rows.Add(new object[] { });
					addApd["vouchernum"] = dr["invnum"];
					addApd["trandate"] = bfavh["trandate"];
					addApd["coy"] = "SAF";
					addApd["apnum"] = dr["apnum"];
					addApd["invnum"] = dr["invnum"];
					addApd["invdate"] = dr["invdate"];
					addApd["locno"] = dr["locno"];
					addApd["deptno"] = dr["deptno"];
					addApd["oricur"] = dr["oricur"];
					addApd["exrate"] = dr["exrate"];
					addApd["accnum"] = dr["accnum"];
					addApd["detail"] = bfavh["detail"];
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

			refreshBfav1();

		}

		public void getCSHOpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];


			dbaccess.ReadSQL("getCSH", "SELECT chknum,trandate,oricur,exrate,accnum,locno,deptno,sum(oriamt) as oriamt,"+
				" sum(postamt) as postamt from csh" + lastYear + 
				" WHERE accnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and accnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' "+
				" AND (RECONDATE='' OR RECONDATE IS NULL) GROUP BY refnum,chknum," +
				"trandate,accnum,locno,deptno,oricur,exrate "+
				" UNION ALL "+
				" SELECT max(chknum) as chknum,max(trandate) as trandate,oricur,exrate,accnum,locno,deptno,"+
				" sum(oriamt) as oriamt,sum(postamt) as postamt from csh" + lastYear + " WHERE "+
				" accnum >='" + FilterOP.CodeFromValue.ToString().Trim() + "' "+
				" and accnum<= '" + FilterOP.CodeToValue.ToString().Trim() + "' "+
				" AND (RECONDATE <>'' and RECONDATE IS NOT NULL) GROUP BY accnum,locno,deptno,oricur,exrate");
			DataTable getCSH = dbaccess.DataSet.Tables["getCSH"];
			foreach (DataRow dr in getCSH.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addCsh = bfav1.Rows.Add(new object[] { });
					addCsh["vouchernum"] = dr["chknum"];
					addCsh["trandate"] = bfavh["trandate"];
					addCsh["coy"] = "SAF";
					addCsh["chknum"] = dr["chknum"];
					addCsh["oricur"] = dr["oricur"];
					addCsh["exrate"] = dr["exrate"];
					addCsh["accnum"] = dr["accnum"];
					addCsh["locno"] = dr["locno"];
					addCsh["deptno"] = dr["deptno"];
					addCsh["detail"] = bfavh["detail"];
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

			refreshBfav1();

		}

		public void getGLOpening()
		{
			DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
			DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];

            //ori
            //string strlGLD = "SELECT trandate,oricur,gld.exrate,accnum,locno,deptno,acm.acctype as acctype,sum(oriamt) as oriamt,"+
            //    " sum(postamt) as postamt from gld" + lastYear + " gld "+
            //    " LEFT OUTER JOIN acm ON accnum =  acm.accnum "+
            //    " WHERE acm.acctype in ('5','6','7','8','9','10','11','12','13','14') "+
            //    " GROUP BY gld.accnum,gld.locno,gld.deptno,gld.oricur,gld.exrate";
            string strlGLD = "SELECT gld.oricur,gld.exrate,gld.accnum,locno,deptno,sum(oriamt) as oriamt," +
                    " sum(postamt) as postamt from gld" + lastYear + " gld " +
                    " LEFT OUTER JOIN acm ON gld.accnum =  acm.accnum " +
                    " WHERE acm.acctype in ('5','6','7','8','9','10','11','12','13','14') " +
                    " GROUP BY gld.accnum,gld.locno,gld.deptno,gld.oricur,gld.exrate";
            dbaccess.ReadSQL("getGL", strlGLD);
			DataTable getGL = dbaccess.DataSet.Tables["getGL"];
			foreach (DataRow dr in getGL.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					DataRow addGl = bfav1.Rows.Add(new object[] { });
					addGl["vouchernum"] = bfavh["refnum"];
					addGl["trandate"] = bfavh["trandate"];
					addGl["coy"] = "SAF";
					addGl["oricur"] = dr["oricur"];
					addGl["exrate"] = dr["exrate"];
					addGl["accnum"] = dr["accnum"];
					addGl["locno"] = dr["locno"];
					addGl["deptno"] = dr["deptno"];
					addGl["detail"] = bfavh["detail"];

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

			refreshBfav1();

		}

		private void InitialComboAccnum()
		{
			this.cboAccname = new ATL.MultiColumnComboBox.ColumnComboBox();

			this.cboAccname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboAccname.DropDownWidth = 17;
			this.cboAccname.Location = new System.Drawing.Point(60, 15);
			this.cboAccname.Name = "cboAccname";
			this.cboAccname.Size = new System.Drawing.Size(240, 21);
			this.cboAccname.TabIndex = 1;
			cboAccname.Visible = true;
			this.cboAccname.KeyDown += new KeyEventHandler(cboAccname_KeyDown);

			this.grpHeaderTransInfo.Controls.Add(cboAccname);

			string strSQL = "SELECT ACCNAME,ACCNUM FROM ACM WHERE ACTIVE='1' ORDER BY ACCNAME";
			DataSet dsACM = this.dbaccess.ReadSQLTemp("ACM", strSQL);
			DataTable dtACM = dsACM.Tables["ACM"];

			cboAccname.Data = dtACM;
			//Set which row will be displayed in the text box
			//If you set this to a column that isn't displayed then the suggesting functionality won't work.
			cboAccname.ViewColumn = 0;
			//Set a few columns to not be shown
			cboAccname.ColWidthNew(new int[] { 300, 100 });
			cboAccname.UpdateIndex();			
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
			this.cboApname.Location = new System.Drawing.Point(450, 40);
			this.cboApname.Name = "cboApname";
			this.cboApname.Size = new System.Drawing.Size(180, 21);
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
			string strSQL = "SELECT TOP 1 * FROM APM WHERE APNUM='" + cboApname["apnum"] + "'";
			DataSet dsAPM = this.dbaccess.ReadSQLTemp("APM", strSQL);
			DataTable dtAPM = dsAPM.Tables["APM"];

			if (dtAPM.Rows.Count > 0)
				return dtAPM.Rows[0]["APNAME"].ToString();
			else
				return "";
		}

        private void GenerateDetail()
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            DataTable bfav1 = dbaccess.DataSet.Tables["bfav1"];
            DataTable bfav1p = dbaccess.DataSet.Tables["bfav1"];

            decimal totalSalary = 0; //60058
            decimal totalCPF = 0; //60009
            decimal totalFWL = 0;
            decimal totalAllowance = 0; //60102
            decimal totalMedical = 0; //60103

            if (bfav1p.Rows.Count > 0)
            {
                foreach (DataRow dr1 in bfav1p.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        //logic here
                    }
                }
            }

        }

        private string getSitenumFromPrjID(string projectID)
        {
            DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
            string sitenum = "";

            string get = "Select Top 1 sitenum from ctrh where projectid='"+projectID+"' AND commencedate<='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(bfavh["trandate"])) + "' and enddate>='" + BizFunctions.GetSafeDateString(Convert.ToDateTime(bfavh["trandate"])) + "' and [status]<>'V'";

            this.dbaccess.ReadSQL("SiteFromPrjID", get);

            DataTable dt = this.dbaccess.DataSet.Tables["SiteFromPrjID"];

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    sitenum = dt.Rows[0]["sitenum"].ToString();
                }
            }

            return sitenum;
        }
	}
}
