/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_ACM.cs
 *	Description:    Good Receipt Note Module
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		17/04/07			Start 
***********************************************************/

using System;
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
using DEMO.MDT;

namespace ATL.PON
{
	public class Voucher_PON : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables
        protected bool opened = false;
		protected DBAccess dbaccess = null;
		protected string headerFormName = null;
		protected Hashtable formsCollection = null;
		protected string projectPath = null;
		protected string formName = null;
		protected bool save = false;
		protected string previous = null;

        protected decimal pon1_grosamt = 0;
        protected decimal pon1_discamt = 0;
        protected decimal pon1_oriamt = 0;

		protected int TabDetail;
        protected Button btn_Extract = null;
        protected Button btn_GRN = null;
        protected RadioButton rad_GRNN = null;
        protected RadioButton rad_GRNY = null;
		#endregion

        public Voucher_PON(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_PON.xml", moduleName, voucherBaseHelpers)
		{

		}

		protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_TabControl_Handle(sender, e);

			TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
		}

        #region Voucher Default/ALL
        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherAllCondition(e);

            e.Condition = "ponh.flag='PON' AND ponh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }

        protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
        {

            base.AddVoucherDefaultCondition(e);

            e.Condition = " (ponh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +

                     " ponh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +

                     " ponh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +

                     " AND ponh.flag='PON' AND ponh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;

        }
        #endregion

		#region Document Handle

		#region Document_Save_Handle

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);
            DataRow ponh = e.DBAccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = e.DBAccess.DataSet.Tables["pon1"];

            # region Check for empty row and empty mat code
            if (pon1.Rows.Count < 1)
            {
                //MessageBox.Show("Can't save with empty details", "Bizpoint International");
                //e.Handle = false;
            }
            foreach (DataRow dr in pon1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr["matnum"]))
                    {
                        MessageBox.Show("Save Unsuccessful\nProduct Code cannot be empty !", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Handle = false;
                        return;
                    }
                    if (dr["qty"]  == System.DBNull.Value)
                    {
                        MessageBox.Show("Save Unsuccessful\nInvalid quantity in details!", "ATL Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Handle = false;
                        return;
                    }
                }
            }
            #endregion
		}

		#endregion

		#region Document_SaveBegin_OnClick

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick(sender, e);
            DataRow ponh = e.DBAccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = e.DBAccess.DataSet.Tables["pon1"];

            foreach (DataRow dr in pon1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(ponh, dr, "status");
                }
            }
		}

		#endregion 

		#region Document_Print_Handle
        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            DataRow ponh = e.DBAccess.DataSet.Tables["ponh"].Rows[0];

            if (ponh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                if (BizValidate.CheckRowState(e.DBAccess.DataSet, "ponh/pon1"))
                {
                    MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }
		protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick(sender, e);

			BizFunctions.SetCoyForPrinting(dbaccess);

			e.DataSource = e.DBAccess.DataSet;
		}

		#endregion

		#region Document_Extract_Handle

		protected override void Document_Extract_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Extract_Handle(sender, e);

            DataRow ponh = dbaccess.DataSet.Tables["ponh"].Rows[0];
			#region Steph - MDT Extraction

			Hashtable HsExtract = MDTReader.GetExtraction("grn", "POR-GRN Extract", TabDetail, this.dbaccess);

			if (HsExtract.Count > 0)
			{
				ExtractGrid extract = new ExtractGrid(this.dbaccess, "extract", HsExtract["DestinationTable"].ToString().Trim(), HsExtract["colDisplay"].ToString().Trim(), HsExtract["colCopy"].ToString().Trim(),
									HsExtract["sqlDisplay"].ToString().Trim(), HsExtract["sqlCopy"].ToString().Trim(), HsExtract["extractkey"].ToString().Trim(), Convert.ToBoolean(HsExtract["inclextracted"]));
				extract.showGrid();
			}
			else
			{
				MessageBox.Show("Error in data extraction");
			}
			#endregion

			AutoCalc();

			e.Handle = false;			

		}

		#endregion

		#endregion

		#region F2/F3
        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            DataRow ponh = dbaccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = dbaccess.DataSet.Tables["pon1"];
            switch (e.ControlName)
            {
                case "ponh_ponum":
                    e.Condition = BizFunctions.F2Condition("porh.refnum", (sender as TextBox).Text);
                    //e.DefaultCondition = "porh.type In ('OVERSEA','OVERSEA2H') and porh.status = 'P' and porh.apnum = '" + ponh["apnum"].ToString().Trim() + "' " +
                    e.DefaultCondition = " porh.status = 'P' and porh.apnum = '" + ponh["apnum"].ToString().Trim() + "' " +
                        " AND (porh.refnum IN " +
                        " (SELECT refnum FROM (SELECT ISNULL(refnum,'') AS refnum,matnum,sum(qty) AS qty FROM " +
                        " (SELECT refnum,matnum,qty FROM por1 " +
                        " UNION ALL " +
                        " SELECT ponum as refnum,matnum,-qty FROM pon1) tmp " +
                        " GROUP BY refnum,matnum  " +
                        " HAVING SUM(qty) > 0)result))";
                    break;
            }
        }

		protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
		{
			base.AddDocumentF3Condition(sender, e);

            DataRow ponh = dbaccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = dbaccess.DataSet.Tables["pon1"];
			
			switch (e.ControlName)
			{
                case "ponh_ponum":
					#region Extraction from porh to ponh
                    string ponum = ponh["ponum"].ToString().Trim();
					if (!ponum.Equals(""))
					{
                        string selectPorh = "SELECT * FROM porh WHERE refnum in (select ponum from (SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
                                "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 UNION ALL" +
                                " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price from grn1 WHERE refnum <>'" + ponh["refnum"].ToString().Trim() + "')a" +
                                "  WHERE ponum = '" + ponh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,discamt,detail" +
                                " HAVING SUM(qty) >0)frmSelectDetailBelow)";
						this.dbaccess.ReadSQL("porhTmp", selectPorh);
						DataTable porhTmp = this.dbaccess.DataSet.Tables["porhTmp"];

						if (porhTmp.Select().Length > 0)
						{
                            ponh["apnum"] = porhTmp.Rows[0]["apnum"];
							//grnh["custpo"] = porhTmp.Rows[0]["custpo"];
                            ponh["oricur"] = porhTmp.Rows[0]["oricur"];
                            ponh["remark"] = porhTmp.Rows[0]["remark"];
                            ponh["gstgrpnum"] = porhTmp.Rows[0]["gstgrpnum"];
						}
						else
						{
                            ponh["apnum"] = "";
                            ponh["docunum"] = "";
							//grnh["custpo"] = "";
                            ponh["oricur"] = "";
                            ponh["remark"] = "";
                            ponh["gstgrpnum"] = "";
						}
					#endregion

					#region the detail page
						BizFunctions.DeleteAllRows(pon1);
						if (porhTmp.Rows.Count != 0)
						{
							#region Import por1 to grn1
                            string selectPor1 = "select por1.refnum as ponum,por1.matnum,por1.uom,por1.discamt,por1.detail,por1.price,por1.uprice,"+
                                                      "case when (ISNULL(por1.qty,0) - ISNULL(pon1.qty,0))< 0 then 0 else (ISNULL(por1.qty,0) - ISNULL(pon1.qty,0)) end as qty,modelcode,pcatcode,pbrdcode,ploftcode,pflexcode,pshfcode,pcolcode,pszcode,boxno   " +
                                                      "from (select refnum,matnum,uom,detail,discamt,sum(qty) as qty,price,uprice,modelcode,pcatcode,pbrdcode,ploftcode,pflexcode,pshfcode,pcolcode,pszcode,boxno   from por1 " +
                                                      "where refnum='" + ponh["ponum"].ToString().Trim() + "' and status='P' and type IN ('OVERSEA','OVERSEA2H') group by refnum,matnum,uom,detail,discamt,price,uprice,modelcode,pcatcode,pbrdcode,ploftcode,pflexcode,pshfcode,pcolcode,pszcode,boxno )por1 " +
                                                      "left join (select ponum,matnum,sum(qty) as qty,price,uprice from pon1 where isnull(status,'')<>'V' group by ponum,matnum,price,uprice)pon1 " +
                                                      "on por1.refnum=pon1.ponum and por1.matnum= pon1.matnum where (por1.qty-ISNULL(pon1.qty,0)) > 0";
                      
							this.dbaccess.ReadSQL("por1Tmp", selectPor1);
							DataTable por1Tmp = this.dbaccess.DataSet.Tables["por1Tmp"];

                            BizFunctions.DeleteAllRows(pon1);
							foreach (DataRow dr in por1Tmp.Select())
							{
								dr.SetAdded();
                                pon1.ImportRow(dr);
							}
                            foreach (DataRow dr2 in pon1.Select())
							{
                                dr2["refnum"] = ponh["refnum"].ToString().Trim();
							}
							#endregion
						}
					}

					AutoCalc();
						#endregion
					break;
			}
		}
		#endregion

		#region Document Event

		#region Form Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);

            opened = true;
			this.dbaccess = e.DBAccess;
			this.formsCollection = e.FormsCollection;
			this.formName = (sender as Form).Name;
			this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            DataRow ponh = dbaccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = dbaccess.DataSet.Tables["pon1"];

            ponh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

            #region initial controls
            btn_Extract = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_Extract") as Button;
            btn_GRN = BizXmlReader.CurrentInstance.GetControl(headerFormName, "btn_autoGRN") as Button;
            rad_GRNN = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_GRNN") as RadioButton;
            rad_GRNY = BizXmlReader.CurrentInstance.GetControl(headerFormName, "rad_GRNY") as RadioButton;
            rad_GRNN.CheckedChanged += new EventHandler(rad_GRNN_CheckedChanged);
            rad_GRNY.CheckedChanged += new EventHandler(rad_GRNY_CheckedChanged);
            btn_Extract.Click += new EventHandler(btn_Extract_Click);
            btn_GRN.Click += new EventHandler(btn_GRN_Click);
            #endregion

            e.DBAccess.DataSet.Tables["ponh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PONH_ColumnChanged);
            e.DBAccess.DataSet.Tables["pon1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PON1_ColumnChanged);

            #region set controls
            if (ponh["status"].ToString() == Common.DEFAULT_DOCUMENT_STATUSN)
            {
                btn_GRN.Enabled = false;
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
            }
            #endregion
        }

        private void btn_GRN_Click(object sender, EventArgs e)
        {
            PON.AutoGRN AutoGrn = new AutoGRN();
            AutoGrn.ShowDialog();
        }

        private void btn_Extract_Click(object sender, EventArgs e)
        {
            DataRow ponh = dbaccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = dbaccess.DataSet.Tables["pon1"];

            if (ponh["apnum"].ToString() != string.Empty)
            {
                #region Import por1 to grn1
                /*
                BizFunctions.DeleteAllRows(pon1);
                   string selectPor1 = "SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
                        "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 where apnum='"+ponh["apnum"].ToString()+"' UNION ALL" +
                        " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price from pon1"+
                        " WHERE refnum <>'" + ponh["refnum"].ToString().Trim() + "' and ponum!=pon1.ponum)a" +
                        " GROUP BY ponum,matnum,uom,discamt,detail" +
                        " HAVING SUM(qty) >0";

                    this.dbaccess.ReadSQL("por1Tmp", selectPor1);
                    DataTable por1Tmp = this.dbaccess.DataSet.Tables["por1Tmp"];

                    BizFunctions.DeleteAllRows(pon1);
                    foreach (DataRow dr in por1Tmp.Select())
                    {
                        dr.SetAdded();
                        pon1.ImportRow(dr);
                    }
                    foreach (DataRow dr2 in pon1.Select())
                    {
                        dr2["refnum"] = ponh["refnum"].ToString().Trim();
                        //#region Steph - SOR is not using the dqty
                        //dr2["dqty"] = dr2["qty"];
                        //#endregion
                    }    
                */
                #endregion
                Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
                DataTable oriTable = pon1;

                try
                {
                    // Open Extract Form
                    ExtractPORForm ExtractPOR = new ExtractPORForm(this.dbaccess, oriTable);
                    ExtractPOR.ShowDialog(frm);

                    #region assign line number for pon1
                    int line = 100;
                    foreach(DataRow dr in pon1.Rows)
                    {
                        if(dr.RowState!=DataRowState.Deleted)
                        {
                            dr["line"] = line;
                            line += 100;
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Please selet supplier No. first");
                return;
            }
            AutoCalc();
        }

        private void rad_GRNY_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_GRNY.Checked)
            {
                btn_GRN.Enabled = true;
            }
        }

        private void rad_GRNN_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_GRNN.Checked)
            {
                btn_GRN.Enabled = false;
            }
        }

		#endregion

        #region DocumentPage Event
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

        #region Refresh

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick(sender, e);
            DataRow ponh = e.DBAccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = dbaccess.DataSet.Tables["pon1"];

            setDefaults(dbaccess.DataSet, "ponh/pon1");

            if (ponh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSN)
			{
                if (ponh["docunum"].ToString().Trim() == String.Empty || ponh["docunum"] == System.DBNull.Value)
                    ponh["docunum"] = ponh["refnum"];
			}
			AutoCalc();
		}

		#endregion

		#region ColumnChangedEvents

		#region grnh
		private void Voucher_PONH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
            DataRow ponh = this.dbaccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = this.dbaccess.DataSet.Tables["pon1"];

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

				case "ponum":
                    //#region Extraction from porh to ponh
                    //string ponum = ponh["ponum"].ToString().Trim();
                    //if (!ponum.Equals(""))
                    //{
                    //    string selectPorh = "SELECT * FROM porh WHERE refnum in (select ponum from (SELECT ponum,matnum,uom,discamt,detail,sum(qty) as qty,max(price) as price from " +
                    //            "(SELECT refnum as ponum, matnum,uom,discamt,detail, qty,price from por1 UNION ALL" +
                    //            " SELECT ponum, matnum,uom,discamt,detail,qty,0 as price from grn1 WHERE refnum <>'" + ponh["refnum"].ToString().Trim() + "')a" +
                    //            "  WHERE ponum = '" + ponh["ponum"].ToString().Trim() + "' GROUP BY ponum,matnum,uom,discamt,detail" +
                    //            " HAVING SUM(qty) >0)frmSelectDetailBelow)";
                    //    this.dbaccess.ReadSQL("porhTmp", selectPorh);
                    //    DataTable porhTmp = this.dbaccess.DataSet.Tables["porhTmp"];

                    //    if (porhTmp.Select().Length > 0)
                    //    {
                    //        e.Row["apnum"] = porhTmp.Rows[0]["apnum"];
                    //        e.Row["docunum"] = porhTmp.Rows[0]["docunum"];
                    //        e.Row["oricur"] = porhTmp.Rows[0]["oricur"];
                    //        e.Row["remark"] = porhTmp.Rows[0]["remark"];
                    //        e.Row["gstgrpnum"] = porhTmp.Rows[0]["gstgrpnum"];
                    //    }
                    //    else
                    //    {
                    //        e.Row["apnum"] = "";
                    //        e.Row["docunum"] = "";
                    //        e.Row["oricur"] = "";
                    //        e.Row["remark"] = "";
                    //        e.Row["gstgrpnum"] = "";
                    //    }
                    //#endregion

                    //#region the detail page
                    //    if (porhTmp.Rows.Count != 0)
                    //    {
                    //        #region Steph - Import por1 to grn1
                    //        string selectPor1 = "select por1.refnum as ponum,por1.matnum,por1.uom,por1.discamt,por1.detail,por1.price," +
                    //                                  "case when (ISNULL(por1.qty,0) - ISNULL(pon1.qty,0))< 0 then 0 else (ISNULL(por1.qty,0) - ISNULL(pon1.qty,0)) end as qty " +
                    //                                  "from (select refnum,matnum,uom,detail,discamt,sum(qty) as qty,sum(price)as price from por1 " +
                    //                                  "where refnum='" + ponh["ponum"].ToString().Trim() + "' and status='P' and type='OVERSEA' group by refnum,matnum,uom,detail,discamt)por1 " +
                    //                                  "left join (select ponum,matnum,sum(qty) as qty,sum(price)as price from pon1 where isnull(status,'')<>'V' group by ponum,matnum)pon1 " +
                    //                                  "on por1.refnum=pon1.ponum and por1.matnum= pon1.matnum where (por1.qty-ISNULL(pon1.qty,0)) > 0";

                    //        this.dbaccess.ReadSQL("por1Tmp", selectPor1);
                    //        DataTable por1Tmp = this.dbaccess.DataSet.Tables["por1Tmp"];
                    //        BizFunctions.DeleteAllRows(pon1);

                    //        foreach (DataRow dr in por1Tmp.Select())
                    //        {
                    //            dr.SetAdded();
                    //            pon1.ImportRow(dr);
                    //        }
                    //        foreach (DataRow dr2 in pon1.Select())
                    //        {
                    //            dr2["refnum"] = ponh["refnum"].ToString().Trim();
                    //        }
                    //        #endregion
                    //    }
                    //}
                    //else
                    //{
                    //    e.Row["apnum"] = "";
                    //    e.Row["docunum"] = "";
                    //    e.Row["oricur"] = "";
                    //    e.Row["remark"] = "";
                    //    e.Row["gstgrpnum"] = "";
                    //}

                    //AutoCalc();
                    //    #endregion
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
							e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)e.Row["trandate"]);
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
					#region set grnh exrate

					e.Row.BeginEdit();
					string strexr = "Select rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + " FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					this.dbaccess.ReadSQL("exrate", strexr);
					if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					{
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]) + ""]);
						e.Row["exrate"] = exrate;
					}
					e.Row.EndEdit();
					break;
					#endregion
			}
		}
		#endregion

        #region pon1
        private void Voucher_PON1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow ponh = this.dbaccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = this.dbaccess.DataSet.Tables["pon1"];

          

            switch (e.Column.ColumnName)
            {
                case "matnum":
                    dbaccess.ReadSQL("getMatm", "SELECT matname, uomcode FROM matm where matnum ='" + e.Row["matnum"].ToString().Trim() + "'");
                    if (dbaccess.DataSet.Tables["getMatm"].Rows.Count > 0)
                    {
                        if (e.Row["detail"].ToString().Trim() == "" || e.Row["detail"] == System.DBNull.Value)
                            e.Row["detail"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["matname"];
                        if (e.Row["uom"].ToString().Trim() == "" || e.Row["uom"] == System.DBNull.Value)
                            e.Row["uom"] = dbaccess.DataSet.Tables["getMatm"].Rows[0]["uomcode"];
                    }
                    break;
                case "qty":
                    if (e.Row["price"] != System.DBNull.Value && Convert.ToDecimal(e.Row["qty"]) > 0)
                    {
                        #region set deafult discamt=0
                        if (e.Row["discamt"] == System.DBNull.Value)
                        {
                            e.Row["discamt"] = 0.00;
                        }
                        #endregion
                        e.Row["grosamt"] = (decimal)e.Row["price"] * (decimal)e.Row["qty"];
                        e.Row["oriamt"] = (decimal)e.Row["grosamt"] - (decimal)e.Row["discamt"];
                    }
                    break;
                case "price":
                    if (e.Row["qty"] != System.DBNull.Value && Convert.ToDecimal(e.Row["price"]) > 0)
                    {
                        #region set deafult discamt=0
                        if (e.Row["discamt"] == System.DBNull.Value)
                        {
                            e.Row["discamt"] = 0.00;
                        }
                        #endregion
                        e.Row["grosamt"] = (decimal)e.Row["price"] * (decimal)e.Row["qty"];
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

		#endregion

		#region fun fun - To set default values

		public static void setDefaults(DataSet dataSet, string tableNames)
		{
			string[] tables = tableNames.Split(new char[] { '/', '\\' });

            string ColumName = "";

            //try
            //{

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
                                        ColumName = dc.ColumnName;
                                        if (dr[dc.ColumnName] == System.DBNull.Value)
                                            dr[dc.ColumnName] = 0;
                                        break;

                                    // All smallints are 0 by default
                                    case "System.Int16":
                                        ColumName = dc.ColumnName;
                                        if (dr[dc.ColumnName] == System.DBNull.Value)
                                            dr[dc.ColumnName] = 0;
                                        break;

                                    // All ints are 0 by default
                                    case "System.Int32":
                                        ColumName = dc.ColumnName;
                                        if (dr[dc.ColumnName] == System.DBNull.Value)
                                            dr[dc.ColumnName] = 0;
                                        break;

                                    // All bigints are 0 by default but do not touch ID
                                    case "System.Int64":
                                        ColumName = dc.ColumnName;
                                        if (dr[dc.ColumnName] == System.DBNull.Value && dc.ColumnName != "ID")
                                            dr[dc.ColumnName] = 0;
                                        break;

                                    // All bits are 0 by default
                                    case "System.Bit":
                                        ColumName = dc.ColumnName;
                                        if (dr[dc.ColumnName] == System.DBNull.Value)
                                            dr[dc.ColumnName] = 0;
                                        break;

                                    // All booleans are false by default
                                    case "System.Boolean":
                                        ColumName = dc.ColumnName;
                                        if (dr[dc.ColumnName] == System.DBNull.Value)
                                            dr[dc.ColumnName] = false;
                                        break;

                                    // Trim white spaces due to user entry
                                    case "System.String":
                                        ColumName = dc.ColumnName;
                                        if (dr[dc.ColumnName] != System.DBNull.Value)
                                            dr[dc.ColumnName] = (dr[dc.ColumnName] as String).Trim();
                                        break;
                                }
                            }
                        }
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    string ErrorStr = ColumName+" "+ex.ToString();
            //}

            //////

		}
		#endregion 

		#region Steph - To set the Auto Calculation to be use in various events
		private void AutoCalc()
		{
            DataRow ponh = dbaccess.DataSet.Tables["ponh"].Rows[0];
            DataTable pon1 = dbaccess.DataSet.Tables["pon1"];

            setDefaults(dbaccess.DataSet, "pon1");

            ponh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(ponh["trandate"]));

            #region pon1

            pon1_grosamt = 0;
            pon1_discamt = 0;
            pon1_oriamt = 0;

            foreach (DataRow dr in pon1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
                    if (Convert.ToDecimal(dr["qty"]) > 0)
                    {
                        BizFunctions.UpdateDataRow(ponh, dr, "refnum/apnum/docunum/trandate/period/user/status/flag/expire/created/modified");
                        if ((decimal)dr["grosamt"] == 0 || dr["grosamt"] == System.DBNull.Value)
                            dr["grosamt"] = (decimal)dr["qty"] * (decimal)dr["price"];

                        dr["oriamt"] = (decimal)dr["grosamt"] - (decimal)dr["discamt"];

                        dr["cosamt"] = dr["oriamt"]; // Reason is to get the cosamt after deduct discamt given by supplier.
                        dr["stdcost"] = (decimal)dr["oriamt"] / (decimal)dr["qty"];

                        pon1_grosamt += (decimal)dr["grosamt"];
                        pon1_discamt += (decimal)dr["discamt"];
                        pon1_oriamt += (decimal)dr["oriamt"];
                    }
				}
			}
            ponh["grosamt"] = pon1_grosamt;
            ponh["discamt"] = pon1_discamt;
            ponh["oriamt"] = pon1_oriamt;
			#endregion
		}
		#endregion
	}
}

