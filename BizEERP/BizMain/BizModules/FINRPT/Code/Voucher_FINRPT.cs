/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_por.cs
 *	Description:    Purchase Order Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Jer				2006-08-04			Add paste_handle, paste_onclick to enable/disable header columnchanged event
 * Jer				2006-07-11			porfrm - added purchase order form
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Mail;
using System.Threading;


using System.Timers;


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
using ATL.BizLogicTools;

using ATL.FSWRefnum;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;


namespace ATL.FINRPT
{
	public class Voucher_FINRPT : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables

		protected DBAccess dbaccess = null;
        protected string headerFormName = string.Empty;
        protected string ReportFormName = string.Empty;
        protected string detail2FormName = string.Empty;

        protected DataGrid dgRev = null;
        protected DateTimePickerGrid dtpg_revdate = null;
        protected bool opened = false;

        protected string tabName = null;
        public DataGrid dg_Report = null;
        public int uppperline = 0;
        public DataTable dtRemain = null;
        public DataTable dtChange = null;
        public int lastnum = 0;

        protected bool FromFormOnLoad = false;
        protected bool FromSaveHandle = false;
		protected DocumentBaseHelper documentBaseHelper;

		protected Button btnCopyFrOtherFsw;

		#endregion

        #region Voucher
        public Voucher_FINRPT(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_FINRPT.xml", moduleName, voucherBaseHelpers)
        {
        }
        #endregion

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
        protected override void Voucher_Form_OnLoad(object sender, VoucherEventArgs e)
        {
            base.Voucher_Form_OnLoad(sender, e);

            try
            {
                ((Button)BizXmlReader.CurrentInstance.GetControl(this.voucherBase.VoucherForm.Name, Common.DEFAULT_VOUCHER_BTNPRINT)).Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        		              
		#region Form Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad (sender, e);
            
            DataRow finrpth = e.DBAccess.DataSet.Tables["finrpth"].Rows[0];
            DataTable finrpt1 = e.DBAccess.DataSet.Tables["finrpt1mirror"];
            DataTable finrpt2 = e.DBAccess.DataSet.Tables["finrpt2"];
			headerFormName = (e.FormsCollection["header"] as Form).Name;

            ReportFormName = (e.FormsCollection["Report"] as Form).Name;

            this.dbaccess = e.DBAccess;

			this.documentBaseHelper = this.DocumentBaseHelpers[e.DBAccess.DataSet.Tables["finrpth"].Rows[0]["refnum"].ToString()] as DocumentBaseHelper;

			btnCopyFrOtherFsw = BizXmlReader.CurrentInstance.GetControl((e.FormsCollection["header"] as Form).Name, "btn_Copy") as Button;
			btnCopyFrOtherFsw.Click += new EventHandler(btnCopyFrOtherFsw_Click);

            e.DBAccess.DataSet.Tables["finrpth"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_FINRPTH_ColumnChanged);
            e.DBAccess.DataSet.Tables["finrpt1mirror"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_FINRPT1_ColumnChanged);

			if (finrpth["status"].ToString() == "N")
			{
                BizFunctions.GetTrandate(headerFormName, "finrpth_trandate", finrpth);
			}

            opened = true;

			(BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Preview") as Button).Enabled = true;
            (BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Extract") as Button).Enabled = true;
            (BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "btn_Document_Print") as Button).Enabled = false;

            dg_Report = BizXmlReader.CurrentInstance.GetControl(ReportFormName, "dg_Report") as DataGrid;
            TextBoxGrid dgtb_gnum = Tools.DGTextBox(dg_Report, "gnum");

            #region Add DoubleClick & F8 events manually to code column

            foreach (DataGridTableStyle dataGridTableStyle in dg_Report.TableStyles)
            {
                foreach (DataGridColumnStyle dataGridColumnStyle in dataGridTableStyle.GridColumnStyles)
                {
                    if (dataGridColumnStyle is BizDataGridTextBoxColumn)
                    {
                        BizDataGridTextBoxColumn bizDataGridTextBoxColumn = dataGridColumnStyle as BizDataGridTextBoxColumn;

                        if (bizDataGridTextBoxColumn.MappingName == "gnum")
                        {
						    bizDataGridTextBoxColumn.TextBoxGrid.KeyDown += new KeyEventHandler(Code_KeyDown);

                            bizDataGridTextBoxColumn.TextBoxGrid.DoubleClick += new EventHandler(Code_DoubleClick);
                        }
                    }
                }
            }
            #endregion

            LXFClassLibrary.Controls.TabControl tabControl = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "tcl_Document") as LXFClassLibrary.Controls.TabControl;
            tabControl.SelectionChanged += new EventHandler(tabControl_SelectionChanged);

            FromFormOnLoad = true;
        }

		void btnCopyFrOtherFsw_Click(object sender, EventArgs e)
		{
			Refnum getRefnum = new Refnum(dbaccess);
			getRefnum.ShowDialog();
		}

		#endregion


        #region Button Event

        protected void dg_Report_MouseClick(object sender, System.EventArgs e)
        {
            DataTable finrpt2 = this.dbaccess.DataSet.Tables["finrpt2"];
            try
            {
               
                uppperline = Convert.ToInt16(dg_Report[dg_Report.CurrentRowIndex, 1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion
        
        #region Insert Onclick

		protected override void Document_Insert_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Insert_Handle(sender, e);
			DataTable finrpt2 = this.dbaccess.DataSet.Tables["finrpt2"];
            DataRow finrpth = this.dbaccess.DataSet.Tables["finrpth"].Rows[0];

			if (tabName == "Report")
			{
				e.Handle = false;

				if (dg_Report.CurrentRowIndex >= 0)
				{
					if (dg_Report.CurrentRowIndex == 0)
						lastnum = 1;
					else
						lastnum = Convert.ToInt16(dg_Report[dg_Report.CurrentRowIndex, 1]);

					dtRemain = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from [finrpt2] where linenum <= " + lastnum);

                    //dtChange = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, "select * from finrpt2 where refnum='" + finrpth["refnum"].ToString()+ "' and linenum >" + lastnum + " order by linenum");
                    //The changed rows order by linenum.TINGTING 20100812
                    string strChange = "select * from finrpt2 where refnum='" + finrpth["refnum"].ToString() + "' and linenum >" + lastnum + " order by linenum";
                    dtChange = this.dbaccess.ReadSQLTemp("dtChange", strChange).Tables["dtChange"];
					BizFunctions.DeleteRow(finrpt2, "linenum >" + lastnum);

					DataRow findr1 = finrpt2.Rows.Add(new object[] { });
					findr1["linenum"] = lastnum + 1;
					findr1["underline"] = "None";
					findr1["bold"] = 0;
					findr1["header"] = 0;

					if (dtChange.Rows.Count > 0)
					{
						int newnum = lastnum + 1;
						foreach (DataRow dr in dtChange.Rows)
						{
							DataRow findr = finrpt2.Rows.Add(new object[] { });

							findr["linenum"] = newnum + 1;
							newnum = (int)findr["linenum"];
							findr["underline"] = dr["underline"];
							findr["bold"] = dr["bold"];
							findr["header"] = dr["header"];
							findr["hide"] = dr["hide"];
							findr["gnum"] = dr["gnum"];
							findr["gname"] = dr["gname"];
							if (!BizFunctions.IsEmpty(dr["formula"]))
							{
								string sNo = string.Empty;
								string newFormula = string.Empty;
								string oldFormula = dr["formula"].ToString() + '=';

								for (int s = 0; s < oldFormula.Length; s++)
								{
									if (s == 0)
									{
										if (!char.IsNumber(oldFormula[s]))
										{
											MessageBox.Show("Invalid Formula\nUnable to Re-generate", "Invalid Formula", MessageBoxButtons.OK, MessageBoxIcon.Error);
											break;
										}

									}


									if (char.IsNumber(oldFormula[s]))
									{
										sNo = sNo + oldFormula[s];
									}
									else
									{
										if (oldFormula[s] == '=')
										{

											if (Convert.ToInt16(sNo) > lastnum)
												newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) + 1);
											else
												newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo));

											findr["formula"] = newFormula;
										}
										else
										{
											if (Convert.ToInt16(sNo) > lastnum)
												newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) + 1) + oldFormula[s];
											else
												newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo)) + oldFormula[s];

											if (oldFormula[s] == '=')
											{
												findr["formula"] = newFormula + sNo;
											}
											sNo = string.Empty;

										}
									}
								}

							}
							findr["pd0"] = dr["pd0"];
							findr["pd2"] = dr["pd2"];
							findr["pd3"] = dr["pd3"];
							findr["pd4"] = dr["pd4"];
							findr["pd5"] = dr["pd5"];
							findr["pd6"] = dr["pd6"];
							findr["pd7"] = dr["pd7"];
							findr["pd8"] = dr["pd8"];
							findr["pd9"] = dr["pd9"];
							findr["pd10"] = dr["pd10"];
							findr["pd11"] = dr["pd11"];
							findr["pd12"] = dr["pd12"];
						}
					}

				}
				else
				{
					DataRow findr = finrpt2.Rows.Add(new object[] { });
					findr["linenum"] = 1;
					findr["underline"] = "None";
					findr["bold"] = 0;
					findr["header"] = 0;
					findr["pd0"] = 0;
					findr["pd2"] = 0;
					findr["pd3"] = 0;
					findr["pd4"] = 0;
					findr["pd5"] = 0;
					findr["pd6"] = 0;
					findr["pd7"] = 0;
					findr["pd8"] = 0;
					findr["pd9"] = 0;
					findr["pd10"] = 0;
					findr["pd11"] = 0;
					findr["pd12"] = 0;

				}
			}
		}

		protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Insert_OnClick(sender, e);

			DataTable finrpt1 = this.dbaccess.DataSet.Tables["finrpt1"];
			DataTable finrpt2 = this.dbaccess.DataSet.Tables["finrpt2"];

			try
			{
				if (e.CurrentRow.Table.TableName == "finrpt2")
				{
					if (dg_Report.CurrentRowIndex == 0)
					{
						e.CurrentRow["linenum"] = 1;

						if (BizFunctions.IsEmpty(e.CurrentRow["underline"]))
							e.CurrentRow["underline"] = "None";


					}
					else
					{
						BizFunctions.DeleteRow(finrpt2, "linenum >" + lastnum);


						if (BizFunctions.IsEmpty(e.CurrentRow["underline"]))
							e.CurrentRow["underline"] = "None";

						if (dg_Report.CurrentRowIndex == 0)
							e.CurrentRow["linenum"] = 1;
						else
							e.CurrentRow["linenum"] = lastnum + 1;

						if (dtChange.Rows.Count > 0)
						{
							int newnum = lastnum + 1;
							foreach (DataRow dr in dtChange.Rows)
							{
								DataRow findr = finrpt2.Rows.Add(new object[] { });

								findr["linenum"] = newnum + 1;
								newnum = (int)findr["linenum"];
								findr["underline"] = dr["underline"];
								findr["bold"] = dr["bold"];
								findr["header"] = dr["header"];
								findr["gnum"] = dr["gnum"];
								findr["gname"] = dr["gname"];
								if (!BizFunctions.IsEmpty(dr["formula"]))
								{
									int fNo = 0;
									string sNo = string.Empty;
									string newFormula = string.Empty;
									string oldFormula = dr["formula"].ToString() + '=';

									for (int s = 0; s < oldFormula.Length; s++)
									{
										if (s == 0)
										{
											if (!char.IsNumber(oldFormula[s]))
											{
												MessageBox.Show("Invalid Formula\nUnable to Re-generate", "Invalid Formula", MessageBoxButtons.OK, MessageBoxIcon.Error);
												break;
											}

										}

										if (char.IsNumber(oldFormula[s]))
										{
											sNo = sNo + oldFormula[s];
										}
										else
										{
											if (oldFormula[s] == '=')
											{

												if (Convert.ToInt16(sNo) > lastnum)
													newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) + 1);
												else
													newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo));

												findr["formula"] = newFormula;
											}
											else
											{
												if (Convert.ToInt16(sNo) > lastnum)
													newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) + 1) + oldFormula[s];
												else
													newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo)) + oldFormula[s];

												if (oldFormula[s] == '=')
												{
													findr["formula"] = newFormula + sNo;
												}
												sNo = string.Empty;

											}
										}
									}

								}
								findr["pd0"] = dr["pd0"];
								findr["pd2"] = dr["pd2"];
								findr["pd3"] = dr["pd3"];
								findr["pd4"] = dr["pd4"];
								findr["pd5"] = dr["pd5"];
								findr["pd6"] = dr["pd6"];
								findr["pd7"] = dr["pd7"];
								findr["pd8"] = dr["pd8"];
								findr["pd9"] = dr["pd9"];
								findr["pd10"] = dr["pd10"];
								findr["pd11"] = dr["pd11"];
								findr["pd12"] = dr["pd12"];
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}    

        #endregion

        #region Delete Onclick

        protected override void Document_Delete_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Delete_Handle(sender, e);
            DataTable finrpt2 = this.dbaccess.DataSet.Tables["finrpt2"];
			try
			{
				if (tabName == "Report")
				{
					e.Handle = false;

					DataTable dtDelete = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select * from finrpt2 where mark = 1");
					if (dtDelete.Rows.Count > 0)
					{
						for (int a = 0; a < dtDelete.Rows.Count; a++)
						{
							if (a == 0)
							{
								DataTable dtMinus = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select * from [finrpt2] where linenum > '" + dtDelete.Rows[a]["linenum"].ToString() + "'");
								BizFunctions.DeleteRow(finrpt2, "linenum >= '" + dtDelete.Rows[a]["linenum"].ToString() + "'");

								if (dtMinus.Rows.Count > 0)
								{
									int linenum = Convert.ToInt16(dtDelete.Rows[a]["linenum"].ToString());
									int newnum = Convert.ToInt16(dtDelete.Rows[a]["linenum"].ToString());
									foreach (DataRow dr in dtMinus.Rows)
									{
										DataRow findr = finrpt2.Rows.Add(new object[] { });
										findr["mark"] = dr["mark"];
										findr["linenum"] = newnum;
										newnum += 1;
										findr["underline"] = dr["underline"];
										findr["bold"] = dr["bold"];
										findr["header"] = dr["header"];
										findr["gnum"] = dr["gnum"];
										findr["gname"] = dr["gname"];
										if (!BizFunctions.IsEmpty(dr["formula"]))
										{
											string sNo = string.Empty;
											string newFormula = string.Empty;
											string oldFormula = dr["formula"].ToString() + '=';

											for (int s = 0; s < oldFormula.Length; s++)
											{
												if (s == 0)
												{
													if (!char.IsNumber(oldFormula[s]))
													{
														MessageBox.Show("Invalid Formula\nUnable to Re-generate", "Invalid Formula", MessageBoxButtons.OK, MessageBoxIcon.Error);
														break;
													}

												}

												if (char.IsNumber(oldFormula[s]))
												{
													sNo = sNo + oldFormula[s];
												}
												else
												{
													if (oldFormula[s] == '=')
													{

														if (Convert.ToInt16(sNo) > linenum)
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) - 1);
														else
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo));

														findr["formula"] = newFormula;
													}
													else
													{
														if (Convert.ToInt16(sNo) > linenum)
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) - 1) + oldFormula[s];
														else
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo)) + oldFormula[s];

														if (oldFormula[s] == '=')
														{
															findr["formula"] = newFormula + sNo;
														}
														sNo = string.Empty;

													}
												}
											}

										}

										findr["pd0"] = dr["pd0"];
										findr["pd2"] = dr["pd2"];
										findr["pd3"] = dr["pd3"];
										findr["pd4"] = dr["pd4"];
										findr["pd5"] = dr["pd5"];
										findr["pd6"] = dr["pd6"];
										findr["pd7"] = dr["pd7"];
										findr["pd8"] = dr["pd8"];
										findr["pd9"] = dr["pd9"];
										findr["pd10"] = dr["pd10"];
										findr["pd11"] = dr["pd11"];
										findr["pd12"] = dr["pd12"];
									}
								}
							}
							else
							{
								DataTable dtMinus = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select * from [finrpt2] where linenum > '" + (Convert.ToInt16(dtDelete.Rows[a]["linenum"].ToString()) - a) + "'");
								BizFunctions.DeleteRow(finrpt2, "linenum >= '" + (Convert.ToInt16(dtDelete.Rows[a]["linenum"].ToString()) - a) + "'");


								if (dtMinus.Rows.Count > 0)
								{
									int linenum = Convert.ToInt16(dtDelete.Rows[a]["linenum"].ToString()) - a;
									int newnum = Convert.ToInt16(dtDelete.Rows[a]["linenum"].ToString()) - a;
									foreach (DataRow dr in dtMinus.Rows)
									{
										DataRow findr = finrpt2.Rows.Add(new object[] { });
										findr["mark"] = dr["mark"];
										findr["linenum"] = newnum;
										newnum += 1;
										findr["underline"] = dr["underline"];
										findr["bold"] = dr["bold"];
										findr["header"] = dr["header"];
										findr["gnum"] = dr["gnum"];
										findr["gname"] = dr["gname"];
										if (!BizFunctions.IsEmpty(dr["formula"]))
										{
											string sNo = string.Empty;
											string newFormula = string.Empty;
											string oldFormula = dr["formula"].ToString() + '=';

											for (int s = 0; s < oldFormula.Length; s++)
											{
												if (s == 0)
												{
													if (!char.IsNumber(oldFormula[s]))
													{
														MessageBox.Show("Invalid Formula\nUnable to Re-generate", "Invalid Formula", MessageBoxButtons.OK, MessageBoxIcon.Error);
														break;
													}

												}

												if (char.IsNumber(oldFormula[s]))
												{
													sNo = sNo + oldFormula[s];
												}
												else
												{
													if (oldFormula[s] == '=')
													{

														if (Convert.ToInt16(sNo) > linenum)
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) - 1);
														else
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo));

														findr["formula"] = newFormula;
													}
													else
													{
														if (Convert.ToInt16(sNo) > linenum)
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo) - 1) + oldFormula[s];
														else
															newFormula = newFormula + Convert.ToString(Convert.ToInt16(sNo)) + oldFormula[s];

														if (oldFormula[s] == '=')
														{
															findr["formula"] = newFormula + sNo;
														}
														sNo = string.Empty;

													}
												}
											}
										}
										findr["pd0"] = dr["pd0"];
										findr["pd2"] = dr["pd2"];
										findr["pd3"] = dr["pd3"];
										findr["pd4"] = dr["pd4"];
										findr["pd5"] = dr["pd5"];
										findr["pd6"] = dr["pd6"];
										findr["pd7"] = dr["pd7"];
										findr["pd8"] = dr["pd8"];
										findr["pd9"] = dr["pd9"];
										findr["pd10"] = dr["pd10"];
										findr["pd11"] = dr["pd11"];
										findr["pd12"] = dr["pd12"];
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
        }

        #endregion

        #region Confirm OnClick
        protected override void Document_Confirm_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Confirm_OnClick(sender, e);
            DataRow prqh = e.DBAccess.DataSet.Tables["prqh"].Rows[0];
            string mailAddress = string.Empty;

            if (!BizFunctions.IsEmpty(prqh["purchaserid"]))
            {
                this.dbaccess.ReadSQL("tmppurchaser", "Select email from hemph where empnum='" + prqh["purchaserid"].ToString() + "'");
                DataTable tempemail1 = this.dbaccess.DataSet.Tables["tmppurchaser"];

                if (!BizFunctions.IsEmpty(tempemail1.Rows[0][0]))
                {
                    mailAddress = tempemail1.Rows[0][0].ToString();
                }
            }

            if (!BizFunctions.IsEmpty(prqh["empnum"]))
            {
                this.dbaccess.ReadSQL("tmprequestor", "Select email from hemph where empnum='" + prqh["empnum"].ToString() + "'");
                DataTable tempemail2 = this.dbaccess.DataSet.Tables["tmprequestor"];
                if (!BizFunctions.IsEmpty(tempemail2.Rows[0][0].ToString()))
                {
                    if(string.IsNullOrEmpty(mailAddress))
                    {
                        mailAddress = tempemail2.Rows[0][0].ToString();
                    }
                    else
                        mailAddress = mailAddress + ";" + tempemail2.Rows[0][0].ToString();
                }
            }

            if (!BizFunctions.IsEmpty(prqh["approvedby"]))
            {
                this.dbaccess.ReadSQL("tmpapprove", "Select email from hemph where empnum='" + prqh["approvedby"].ToString() + "'");
                DataTable tempemail3 = this.dbaccess.DataSet.Tables["tmpapprove"];
                if (!BizFunctions.IsEmpty(tempemail3.Rows[0][0]))
                {
                    if (string.IsNullOrEmpty(mailAddress))
                    {
                        mailAddress = tempemail3.Rows[0][0].ToString();
                    }
                    else
                        mailAddress = mailAddress + ";" + tempemail3.Rows[0][0].ToString();
                }
            }

            if (!string.IsNullOrEmpty(mailAddress))
            {
				if (Tools.SendEmailWithStatus(" ", mailAddress, "Puchase Request", prqh["refnum"].ToString() + " has been Confirmed"))
				{
					MessageBox.Show("Email Successfully Sent", "Successful Email", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
            }         

        }
        #endregion

		#region Refresh
        
		protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Refresh_OnClick (sender, e);

	        DataTable finrpt1 = this.dbaccess.DataSet.Tables["finrpt1"];
            DataTable finrpt1Mirror = this.dbaccess.DataSet.Tables["finrpt1mirror"];
            DataTable finrpt2 = this.dbaccess.DataSet.Tables["finrpt2"];
            DataRow finrpth = e.DBAccess.DataSet.Tables["finrpth"].Rows[0];

            try
            {
                if (FromFormOnLoad != true)
                {
                    DataSet dsEnviroment = new DataSet("dsEnviroment");
                    TransferFinrpt1Table(finrpt1, finrpt1Mirror);
                    DataTable dtSettings = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select accnum from [finrpt1] where accnum is not null group by accnum");


                    if (dtSettings.Rows.Count > 0)
                    {
                        string sqlcc = " accnum, ccnum, ";

						string sqlG = sqlcc + " ,status ";

                        #region Check Selected Status

                        string SqlStatus = string.Empty;

                        if (!BizFunctions.IsEmpty(finrpth["selectstatus"]))
                        {
                            if (finrpth["selectstatus"].ToString() == "Open Only")
                            {
                                SqlStatus = " status = 'O' ";
                            }
                            else
                                if (finrpth["selectstatus"].ToString() == "Confirmed Only")
                                {
                                    SqlStatus = " status = 'P' ";
                                }
                                else
                                    SqlStatus = " status <> 'V' ";
                        }

						if(SqlStatus == String.Empty)
						{
							SqlStatus = "1=1";
						}
                        #endregion

						string sSql = "select isnull(accnum, '') as accnum , sum(pd0) as pd0, sum(pd1) as pd1, sum(pd2) as pd2, sum(pd3) as pd3, " +
                                      "sum(pd4) as pd4, sum(pd5) as pd5, sum(pd6) as pd6, sum(pd7) as pd7, sum(pd8) as pd8, sum(pd9) as pd9, " +
                                      "sum(pd10) as pd10, sum(pd11) as pd11, sum(pd12) as pd12, isnull(ccnum, '') as ccnum "+
	                                  " from ( " +
                                      "select period, postamt, case when  period =0 then postamt else 0 end pd0, " +
                                      "case when  period =1 then postamt else 0 end pd1, " +
                                      "case when period =2 then postamt else 0 end pd2, " +
                                      "case when period =3 then postamt else 0 end pd3, " +
                                      "case when period =4 then postamt else 0 end pd4, " +
                                      "case when period =5 then postamt else 0 end pd5, " +
                                      "case when period =6 then postamt else 0 end pd6, " +
                                      "case when period =7 then postamt else 0 end pd7, " +
                                      "case when period =8 then postamt else 0 end pd8, " +
                                      "case when period =9 then postamt else 0 end pd9, " +
                                      "case when period =10 then postamt else 0 end pd10, " +
                                      "case when period =11 then postamt else 0 end pd11, " +
                                      "case when period =12 then postamt else 0 end pd12, " +
                                      "accnum, ccnum "+
									  //" , ccnum2, ccnum3, ccnum4, ccnum5 " +
                                      "from gld" + Common.DEFAULT_SYSTEM_YEAR.ToString().Trim() + " where " + SqlStatus + " and period<='" + finrpth["period"].ToString()+ "'" +
                                      ") as result ";
                                   

                        string sqlIn = string.Empty;
                        string sIn = string.Empty;
                        string sqlGroup = " group by isnull(accnum, ''), isnull(ccnum, '') ";

                        for (int b = 0; b < dtSettings.Rows.Count; b++)
                        {
                            if (dtSettings.Rows[b].RowState != DataRowState.Deleted)
                            {
                                if (!BizFunctions.IsEmpty(dtSettings.Rows[b]["accnum"]))
                                {
                                    if (b == dtSettings.Rows.Count - 1)
                                        sIn += dtSettings.Rows[b]["accnum"].ToString();
                                    else
                                        sIn += dtSettings.Rows[b]["accnum"].ToString() + ", ";
                                }
                            }
                        }

                        sqlIn = "where accnum in (" + sIn + ") ";

                        #region CCNUM Filter
                        string sCcnum = string.Empty;

                        if (!BizFunctions.IsEmpty(finrpth["ccnum"]))
                        {
                            if (sCcnum == string.Empty)
                                sCcnum = "ccnum = '" + finrpth["ccnum"].ToString() + "' ";
                            else
                                sCcnum += "or ccnum = '" + finrpth["ccnum"].ToString() + "' ";
                        }					

                        #endregion
                        string sqlCCNum = string.Empty;
                        if (sCcnum != string.Empty)
                        {
                            sqlIn += "and (" + sCcnum + ") ";
                            sqlCCNum = " and " + sCcnum;
                        }


                        #region Put Datatable into dbaccess.dataset Method

                        dbaccess.ReadSQL("dt1", sSql + sqlGroup);
                        DataTable dt1 = dbaccess.DataSet.Tables["dt1"];
                        dtSettings.TableName = "dtSettings";
                        dsEnviroment.Tables.Add(dt1.Copy());
                        dsEnviroment.Tables.Add(dtSettings.Copy());
                        string sqlSelect = "select * from dt1 where accnum in(select accnum from dtSettings where accnum <> '') " + sqlCCNum;
                        DataTable dtAllFigure = BizFunctions.ExecuteQuery(dsEnviroment, sqlSelect);
                        dtAllFigure.TableName = "tempAllFigure";
                        if (dbaccess.DataSet.Tables.Contains("tempAllFigure"))
                        {
                            dbaccess.DataSet.Tables.Remove("tempAllFigure");
                            dbaccess.DataSet.Tables.Add(dtAllFigure.Copy());
                        }
                        else
                            dbaccess.DataSet.Tables.Add(dtAllFigure.Copy());


                        #endregion

                        #region Figure Calculation

                        string dtgnumSql = string.Empty;
                        DataTable dtgnum = new DataTable();
                        if (dtAllFigure.Rows.Count > 0)
                        {
                            foreach (DataRow dr in finrpt2.Rows)
                            {
                                if (dr.RowState != DataRowState.Deleted)
                                {

                                    #region Clear Data B4 Add

                                    //steph - if it's the gnum to pull from other fsw, skip this!
                                    if (dr["gnum"].ToString().Trim().Contains("{"))
                                    {
                                        //skip the clear data! data to follow the figure as user pulled from F8 of the particular row
                                    }
                                    else
                                    {
                                        dr["pd0"] = 0;
                                        dr["pd1"] = 0;
                                        dr["pd2"] = 0;
                                        dr["pd3"] = 0;
                                        dr["pd4"] = 0;
                                        dr["pd5"] = 0;
                                        dr["pd6"] = 0;
                                        dr["pd7"] = 0;
                                        dr["pd8"] = 0;
                                        dr["pd9"] = 0;
                                        dr["pd10"] = 0;
                                        dr["pd11"] = 0;
                                        dr["pd12"] = 0;
                                    }

                                    #endregion

                                    #region Insert Data

                                    DataTable dt2 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select accnum from [finrpt1] where gnum = '" + dr["gnum"].ToString() + "'");
                                    dtgnumSql = "select sum(pd0) as pd0, sum(pd1) as pd1, sum(pd2) as pd2, sum(pd3) as pd3, sum(pd4) as pd4, sum(pd5) as pd5, sum(pd6) as pd6, sum(pd7) as pd7, sum(pd8) as pd8, sum(pd9) as pd9, sum(pd10) as pd10, sum(pd11) as pd11, sum(pd12) as pd12 " +
                                                "from [tempAllFigure] where accnum in (select accnum from [finrpt1] where gnum = '" + dr["gnum"].ToString() + "' and accnum is not null)";
                                    dtgnum = BizFunctions.ExecuteQuery(dbaccess.DataSet, dtgnumSql);


                                    if (dr["gnum"].ToString().Trim() == "{PROFIT&LOSS@87}")
                                    {
                                    }

                                    #region Set 0
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd0"])) dtgnum.Rows[0]["pd0"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd1"])) dtgnum.Rows[0]["pd1"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd2"])) dtgnum.Rows[0]["pd2"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd3"])) dtgnum.Rows[0]["pd3"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd4"])) dtgnum.Rows[0]["pd4"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd5"])) dtgnum.Rows[0]["pd5"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd6"])) dtgnum.Rows[0]["pd6"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd7"])) dtgnum.Rows[0]["pd7"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd8"])) dtgnum.Rows[0]["pd8"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd9"])) dtgnum.Rows[0]["pd9"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd10"])) dtgnum.Rows[0]["pd10"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd11"])) dtgnum.Rows[0]["pd11"] = 0;
                                    if (BizFunctions.IsEmpty(dtgnum.Rows[0]["pd12"])) dtgnum.Rows[0]["pd12"] = 0;

                                    #endregion

                                    //steph - if it's the gnum to pull from other fsw, skip this!
                                    if (dr["gnum"].ToString().Trim().Contains("{"))
                                    {
                                        //skip the assign value from gld. Use the figure from the other fsw as indicated!
                                    }
                                    else
                                    {
                                        if (dtgnum.Rows.Count > 0)
                                        {
                                            dr["pd0"] = dtgnum.Rows[0]["pd0"].ToString();
                                            dr["pd1"] = dtgnum.Rows[0]["pd1"].ToString();
                                            dr["pd2"] = dtgnum.Rows[0]["pd2"].ToString();
                                            dr["pd3"] = dtgnum.Rows[0]["pd3"].ToString();
                                            dr["pd4"] = dtgnum.Rows[0]["pd4"].ToString();
                                            dr["pd5"] = dtgnum.Rows[0]["pd5"].ToString();
                                            dr["pd6"] = dtgnum.Rows[0]["pd6"].ToString();
                                            dr["pd7"] = dtgnum.Rows[0]["pd7"].ToString();
                                            dr["pd8"] = dtgnum.Rows[0]["pd8"].ToString();
                                            dr["pd9"] = dtgnum.Rows[0]["pd9"].ToString();
                                            dr["pd10"] = dtgnum.Rows[0]["pd10"].ToString();
                                            dr["pd11"] = dtgnum.Rows[0]["pd11"].ToString();
                                            dr["pd12"] = dtgnum.Rows[0]["pd12"].ToString();
                                        }
                                    }

                                    #endregion
                                }
                            }

                        #endregion

                            #region Formula Calculation

                            ArrayList aFormula = new ArrayList();
                            int deletedrowcount = 0;
                            foreach (DataRow dr2 in finrpt2.Rows)
                            {
                                if (dr2.RowState != DataRowState.Deleted)
                                {
                                    if (!BizFunctions.IsEmpty(dr2["formula"]))
                                    {

                                        #region split into arraylist

                                        aFormula = SplitFormula(dr2["formula"].ToString());

                                        #endregion

                                        #region Calculate and Insert

                                        decimal pd0 = 0;
                                        decimal pd1 = 0;
                                        decimal pd2 = 0;
                                        decimal pd3 = 0;
                                        decimal pd4 = 0;
                                        decimal pd5 = 0;
                                        decimal pd6 = 0;
                                        decimal pd7 = 0;
                                        decimal pd8 = 0;
                                        decimal pd9 = 0;
                                        decimal pd10 = 0;
                                        decimal pd11 = 0;
                                        decimal pd12 = 0;

                                        int rowindex = GetRowIndex(aFormula[0].ToString());

                                        pd0 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd0"]);
                                        pd1 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd1"]);
                                        pd2 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd2"]);
                                        pd3 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd3"]);
                                        pd4 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd4"]);
                                        pd5 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd5"]);
                                        pd6 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd6"]);
                                        pd7 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd7"]);
                                        pd8 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd8"]);
                                        pd9 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd9"]);
                                        pd10 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd10"]);
                                        pd11 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd11"]);
                                        pd12 = Convert.ToDecimal(finrpt2.Rows[rowindex]["pd12"]);




                                        for (int i = 0; i < aFormula.Count; i++)
                                        {
                                            if (aFormula[i].ToString() == "+")
                                            {
                                                //Third Version
                                                rowindex = GetRowIndex(aFormula[i + 1].ToString());
                                                pd0 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd0"]);
                                                pd1 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd1"]);
                                                pd2 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd2"]);
                                                pd3 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd3"]);
                                                pd4 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd4"]);
                                                pd5 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd5"]);
                                                pd6 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd6"]);
                                                pd7 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd7"]);
                                                pd8 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd8"]);
                                                pd9 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd9"]);
                                                pd10 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd10"]);
                                                pd11 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd11"]);
                                                pd12 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd12"]);


                                            }
                                            else if (aFormula[i].ToString() == "-")
                                            {
                                                rowindex = GetRowIndex(aFormula[i + 1].ToString());
                                                pd0 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd0"]);
                                                pd1 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd1"]);
                                                pd2 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd2"]);
                                                pd3 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd3"]);
                                                pd4 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd4"]);
                                                pd5 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd5"]);
                                                pd6 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd6"]);
                                                pd7 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd7"]);
                                                pd8 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd8"]);
                                                pd9 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd9"]);
                                                pd10 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd10"]);
                                                pd11 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd11"]);
                                                pd12 -= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd12"]);
                                            }
                                            else if (aFormula[i].ToString() == "*")
                                            {
                                                rowindex = GetRowIndex(aFormula[i + 1].ToString());
                                                pd0 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd0"]);
                                                pd1 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd1"]);
                                                pd2 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd2"]);
                                                pd3 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd3"]);
                                                pd4 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd4"]);
                                                pd5 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd5"]);
                                                pd6 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd6"]);
                                                pd7 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd7"]);
                                                pd8 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd8"]);
                                                pd9 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd9"]);
                                                pd10 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd10"]);
                                                pd11 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd11"]);
                                                pd12 *= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd12"]);
                                            }
                                            else if (aFormula[i].ToString() == "/")
                                            {
                                                rowindex = GetRowIndex(aFormula[i + 1].ToString());
                                                pd0 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd0"]);
                                                pd1 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd1"]);
                                                pd2 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd2"]);
                                                pd3 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd3"]);
                                                pd4 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd4"]);
                                                pd5 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd5"]);
                                                pd6 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd6"]);
                                                pd7 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd7"]);
                                                pd8 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd8"]);
                                                pd9 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd9"]);
                                                pd10 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd10"]);
                                                pd11 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd11"]);
                                                pd12 /= Convert.ToDecimal(finrpt2.Rows[rowindex]["pd12"]);

                                            }
                                            else if (aFormula[i].ToString() == ":")
                                            {
                                                for (int a = Convert.ToInt16(aFormula[0].ToString()) + 1; a <= Convert.ToInt16(aFormula[i + 1].ToString()); a++)
                                                {
                                                    rowindex = GetRowIndex(Convert.ToString(a));
                                                    pd0 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd0"]);
                                                    pd1 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd1"]);
                                                    pd2 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd2"]);
                                                    pd3 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd3"]);
                                                    pd4 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd4"]);
                                                    pd5 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd5"]);
                                                    pd6 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd6"]);
                                                    pd7 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd7"]);
                                                    pd8 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd8"]);
                                                    pd9 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd9"]);
                                                    pd10 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd10"]);
                                                    pd11 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd11"]);
                                                    pd12 += Convert.ToDecimal(finrpt2.Rows[rowindex]["pd12"]);
                                                }
                                            }
                                            else if (aFormula[i].ToString() == "=")
                                            {
                                                dr2["pd0"] = pd0;
                                                dr2["pd1"] = pd1;
                                                dr2["pd2"] = pd2;
                                                dr2["pd3"] = pd3;
                                                dr2["pd4"] = pd4;
                                                dr2["pd5"] = pd5;
                                                dr2["pd6"] = pd6;
                                                dr2["pd7"] = pd7;
                                                dr2["pd8"] = pd8;
                                                dr2["pd9"] = pd9;
                                                dr2["pd10"] = pd10;
                                                dr2["pd11"] = pd11;
                                                dr2["pd12"] = pd12;
                                            }

                                        }


                                        #endregion

                                    }
                                }
                                else
                                    deletedrowcount++;
                            }

                            #endregion

                        }
                        else //TINGTING,Only show up to cut-off period
                        {
                            foreach (DataRow dr in finrpt2.Rows)
                            {
                                if (dr.RowState != DataRowState.Deleted)
                                {
                                    dr["pd0"] = 0;
                                    dr["pd1"] = 0;
                                    dr["pd2"] = 0;
                                    dr["pd3"] = 0;
                                    dr["pd4"] = 0;
                                    dr["pd5"] = 0;
                                    dr["pd6"] = 0;
                                    dr["pd7"] = 0;
                                    dr["pd8"] = 0;
                                    dr["pd9"] = 0;
                                    dr["pd10"] = 0;
                                    dr["pd11"] = 0;
                                    dr["pd12"] = 0;

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in finrpt2.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                dr["pd0"] = 0;
                                dr["pd1"] = 0;
                                dr["pd2"] = 0;
                                dr["pd3"] = 0;
                                dr["pd4"] = 0;
                                dr["pd5"] = 0;
                                dr["pd6"] = 0;
                                dr["pd7"] = 0;
                                dr["pd8"] = 0;
                                dr["pd9"] = 0;
                                dr["pd10"] = 0;
                                dr["pd11"] = 0;
                                dr["pd12"] = 0;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

			foreach (DataRow dr in finrpt2.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					dr["line"] = dr["linenum"];
				}
			}
           
        }

        protected override void Document_Refresh_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Refresh_Handle(sender, e);

            try
            {
                if (FromFormOnLoad != true)
                {
                    DataTable finrpt1 = this.dbaccess.DataSet.Tables["finrpt1"];

                    DataTable finrpt2 = this.dbaccess.DataSet.Tables["finrpt2"];
                    DataRow finrpth = e.DBAccess.DataSet.Tables["finrpth"].Rows[0];
                    if (FromSaveHandle == false)
                    {
                        CheckHandle(e);
                    }

                    FromSaveHandle = false;
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
            }

        }

	#endregion

        #region Document Print
        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

            try
            {
                DataRow finrpth = e.DBAccess.DataSet.Tables["finrpth"].Rows[0];
                DataTable finrpt1 = TransferFinrpt1Table(dbaccess.DataSet.Tables["finrpt1"], dbaccess.DataSet.Tables["finrpt1mirror"]);
                string CcnumSelect = "";
                string CcnumGroup = " group by ";

                switch (e.ReportName)
                {
                    case "Financial Report":
                        Hashtable selectedCollection1 = new Hashtable();
                        selectedCollection1.Add("Coy", "SELECT * FROM Coy");

                        string sqlYearly = "select finrpth.title title, finrpt2.gnum gnum, finrpt2.gname gname, finrpt2.pd0 pd0, finrpt2.pd1 pd1, finrpt2.pd2 pd2, " +
                                           "finrpt2.pd3 pd3, finrpt2.pd4 pd4, finrpt2.pd5 pd5, finrpt2.pd6 pd6, finrpt2.pd7 pd7, finrpt2.pd8 pd8, finrpt2.pd9 pd9, " +
                                           "finrpt2.pd10 pd10, finrpt2.pd11 pd11, finrpt2.pd12 pd12, finrpt2.bold bold, finrpt2.underline underline, finrpt2.header header, finrpt2.hide hide from finrpth, finrpt2 " +
                                           "where finrpth.refnum = finrpt2.refnum and finrpth.refnum = '" + finrpth["refnum"].ToString() + "' " +
                                           "and finrpth.status <> 'V' and finrpt2.hide <>1 " +
										   " ORDER BY finrpt2.linenum ";


                        selectedCollection1.Add("dtYearly", sqlYearly);

						  
                        #region Sub Report Table

                        string SubSelect = "select ";
                        string SubItem = "gnum, sum(pd0) as pd0, sum(pd1) as pd1, sum(pd2) as pd2, sum(pd3) as pd3, " +
                                         "sum(pd4) as pd4, sum(pd5) as pd5, sum(pd6) as pd6, sum(pd7) as pd7, " +
                                         "sum(pd8) as pd8, sum(pd9) as pd9, sum(pd10) as pd10, sum(pd11) as pd11, " +
                                         "sum(pd12) as pd12 from [tempAllFigure] " +
                                         "left join finrpt1 on tempAllFigure.accnum = finrpt1.accnum";

                        string sql = SubSelect + CcnumSelect + SubItem + CcnumGroup + " gnum";


                        #endregion

                        DataTable dtSubReport = BizFunctions.ExecuteQuery(e.DBAccess.DataSet, sql);
                        dtSubReport.TableName = "dtSubReport";
                        if (e.DBAccess.DataSet.Tables.Contains("dtSubReport"))
                            e.DBAccess.DataSet.Tables.Remove("dtSubReport");

                        e.DBAccess.DataSet.Tables.Add(dtSubReport);


                        e.DBAccess.ReadSQL(selectedCollection1);
                        e.DataSource = e.DBAccess.DataSet;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        #endregion

        #region Document Preview Handle
        protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Preview_Handle(sender, e);
            try
            {
                DataTable finrpt1 = this.dbaccess.DataSet.Tables["finrpt1mirror"];
                DataTable finrpt2 = this.dbaccess.DataSet.Tables["finrpt2"];
                DataRow finrpth = e.DBAccess.DataSet.Tables["finrpth"].Rows[0];
			

                if (finrpt1.Rows.Count > 0)
                {

                    if (finrpth["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
                    {
                        if (BizValidate.CheckRowState(e.DBAccess.DataSet, "finrpth/finrpt1/finrpt2"))
                        {
                            MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Handle = false;
                            return;
                        }
                    }

                    finrpth["rawreport"] = false;

                    #region Check Setting Fields to be in Report

                    CheckMatching();

                    if (BizValidate.CheckColumnError(this.dbaccess.DataSet, "finrpt1mirror"))
                    {
                        if (MessageBox.Show("Group Num Incomplete\nDo you wish to proceed?", "Incomplete Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            foreach (DataRow dr in dbaccess.DataSet.Tables["finrpt1mirror"].Rows)
                                dr.ClearErrors();

                            finrpth["rawreport"] = true;

                        }
                        else
                            e.Handle = false;
                    }

                    #endregion

                    dbaccess.DataSet.AcceptChanges();
                }
                else
                {
                    e.Handle = false;
                    MessageBox.Show("Please fill in the data before preview report", "Error Preview", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Document Save Handle
       
        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow finrpth = e.DBAccess.DataSet.Tables["finrpth"].Rows[0];
            DataTable finrpt1 = e.DBAccess.DataSet.Tables["finrpt1"];
            DataTable finrpt2 = e.DBAccess.DataSet.Tables["finrpt2"];
            FromSaveHandle = true;
            try
            {
                CheckHandle(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }                       
        }


        protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Confirm_Handle(sender, e);
            try
            {
                CheckHandle(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataTable finrpt2 = e.DBAccess.DataSet.Tables["finrpt2"];

            foreach (DataRow dr in finrpt2.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(dr["header"]))
                        dr["header"] = 0;

                    if (BizFunctions.IsEmpty(dr["hide"]))
                        dr["hide"] = 0;
                }
            }
        }


        #endregion Document Save Handle

        #region Document F2

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            try
            {
                DataRow finrpth = e.DBAccess.DataSet.Tables["finrpth"].Rows[0];
                
                switch (e.ControlName)
                {
                    case "finrpth_ccnum":
                        e.Condition = BizFunctions.F2Condition("ccnum", (sender as TextBox).Text);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            switch (e.ControlName)
            {
                case "prqh_empnum":
                    e.CurrentRow["empname"] = e.F2CurrentRow["empname"];
                    break;

                case "prqh_approvedby":
                    e.CurrentRow["approvedby2"] = e.F2CurrentRow["empname"];
                    break;

                case "prqh_purchasername":
                    e.CurrentRow["purchasername"] = e.F2CurrentRow["empname"];
                    break;
            }
        }
        #endregion Document F2

        #region Detail F2

        protected override void AddDetailF2Condition(object sender, DetailF2EventArgs e)
        {
            base.AddDetailF2Condition(sender, e);
            DataRow finrpth = this.dbaccess.DataSet.Tables["finrpth"].Rows[0];
            try
            {
                switch (e.MappingName)
                {
                   case "accnumfrom":
                        e.Condition = BizFunctions.F2Condition("accnum/accname", (sender as TextBox).Text);
                        break;

                    case "accnumto":
                        e.Condition = BizFunctions.F2Condition("accnum/accname", (sender as TextBox).Text);
                     break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);

            try
            {

                switch (e.MappingName)
                {
                    

                    case "accnum":
                        e.CurrentRow["accname"] = e.F2CurrentRow["accname"];
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ColumnChangedEvents

		#region FINRPTH

        private void Voucher_FINRPTH_ColumnChanged(object sender, DataColumnChangeEventArgs e) 
		{
            DataRow finrpth = this.dbaccess.DataSet.Tables["finrpth"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "trandate":
                    #region Update the period 
                    if (e.Row["trandate"] != System.DBNull.Value)
                    {
                        // Update the period and exchange rate
                        //e.Row["period"] = BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"],true);
                        e.Row["period"] = Tools.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]);
                        
                    }
                    else
                    // Date MUST NEVER be null. No reason to be. Default is always now.
                    {
                        e.Row["trandate"] = System.DateTime.Now.Date.ToShortDateString();
                    }
                    break;

                    #endregion Update the period and  exchange rate
                case "oricur":
                    #region Validate the currency 

                    if (e.Row["oricur"] != System.DBNull.Value && e.Row[e.Column.ColumnName].ToString().Trim() != "")
                    {
                        if (!BizValidate.CheckTableIsValid(this.dbaccess, "exr", "oricur", e.Row["oricur"].ToString().Trim()))
                        {
                            // If invalid oricur, set it back to default
                            MessageBox.Show("Invalid 'Currency Code' selected !");
                            e.Row["oricur"] = BizAccounts.GetDefaultCurrency(dbaccess);
                           
                            return;
                        }
                        else
                        {
                            int arperiod = BizAccounts.GetPeriod(this.dbaccess, (DateTime)e.Row["trandate"]);
                            e.Row["exrate"] = BizAccounts.GetExRate(this.dbaccess, e.Row["oricur"].ToString().Trim(), arperiod);                            
                        }
                    }
                    else
                    {
                        e.Row["oricur"] = BizAccounts.GetDefaultCurrency(dbaccess);  
                    }
                    break;
                    #endregion Validate the currency 

            }

            if (FromFormOnLoad == true)
                FromFormOnLoad = false;
		}

		#endregion

		#region FINRPT1

        private void Voucher_FINRPT1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
            DataTable finrpt1 = dbaccess.DataSet.Tables["finrpt1mirror"];

            switch(e.Column.ColumnName)
			{
                case "accnumfrom":
                    #region clear account name when code is blank

                    if (e.Row[e.Column.ColumnName].ToString().Trim() == "")
                    {
                        e.Row["accnamefrom"] = string.Empty;
                    }
                    else
                    {
                        string s = GetExtraField(getDtCheckAccAvailability(), GetdtSettingAccnum());
                        if (s == string.Empty)
                        {
                            this.dbaccess.ReadSQL("tmpacc", "Select accname from acm where accnum='" + e.Row[e.Column.ColumnName] + "'");
                            DataTable dtacc = this.dbaccess.DataSet.Tables["tmpacc"];

                            if (dtacc.Rows.Count != 0)
                            {
                                e.Row["accnamefrom"] = dtacc.Rows[0]["accname"].ToString().Trim();
                                if (BizFunctions.IsEmpty(e.Row["accnumto"]))
                                    e.Row["accnumto"] = e.Row[e.Column.ColumnName].ToString();
                            }
                            else
                            {
                                e.Row["accnamefrom"] = "";
                                e.Row.SetColumnError(e.Column.ColumnName, "Invalid Account Code!");
                            }
                        }
                        else
                            e.Row.SetColumnError(e.Column.ColumnName, "Account code not in master or not belongs to report type");
                    }

                    break;
                    #endregion

                case "accnumto":
                    #region clear account name when code is blank

                    if (e.Row[e.Column.ColumnName].ToString().Trim() == "")
                    {
                        e.Row["accnameto"] = string.Empty;
                    }
                    else
                    {
                        string s = GetExtraField(getDtCheckAccAvailability(), GetdtSettingAccnum());
                        if (s == string.Empty)
                        {
                            this.dbaccess.ReadSQL("tmpacc", "Select accname from acm where accnum='" + e.Row[e.Column.ColumnName] + "'");
                            DataTable dtacc = this.dbaccess.DataSet.Tables["tmpacc"];

                            if (dtacc.Rows.Count != 0)
                                e.Row["accnameto"] = dtacc.Rows[0]["accname"].ToString().Trim();
                            else
                            {
                                e.Row["accnameto"] = "";
                                e.Row.SetColumnError(e.Column.ColumnName, "Invalid Account Code!");
                            }
                        }
                        else
                            e.Row.SetColumnError(e.Column.ColumnName, "Account code not in master or not belongs to report type");
                    }

                    break;
                    #endregion
             
			}
		}

		#endregion

        #endregion

        #region report get from setting
        #region double click 

        protected void Code_DoubleClick(object sender, System.EventArgs e)
        {
            DataGridEventArgs ev = new DataGridEventArgs();
            ev.MappingName = (sender as TextBoxGrid).MappingName;
            this.Code_DataGridKeyDownF2(sender, ev);
        }
        #endregion

        #region Code_KeyDown

        protected void Code_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                DataGridEventArgs ev = new DataGridEventArgs();
                ev.MappingName = (sender as TextBoxGrid).MappingName;
                this.Code_DataGridKeyDownF2(sender, ev);
            }
        }

        #endregion

        #region Code_DataGridKeyDownF2

        protected void Code_DataGridKeyDownF2(object sender, DataGridEventArgs e)
        {
            DetailKeyEventArgs ev = new DetailKeyEventArgs(this.ReportFormName, e.MappingName);
            this.Detail_KeyDownF2(sender, ev);
        }

        #endregion

        #region Detail_KeyDownF2

        protected void Detail_KeyDownF2(object sender, DetailKeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            DataGrid dataGrid = textBox.Tag as DataGrid;
            DataRow currentRow = (dataGrid.BindingContext[dataGrid.DataSource].Current as DataRowView).Row;

			if (textBox.Text.Contains("{"))
			{
				//steph -  get p&l figure

				string subStr;
				string[] split;
				string sql0;
				string sql1;

				string readGnum = textBox.Text;
			
				try
				{
					subStr = readGnum.ToString().Substring(readGnum.ToString().IndexOf('{'), readGnum.ToString().IndexOf('}') - readGnum.ToString().IndexOf('{') + 1);

					split = subStr.Split('@');

					sql0 = split[0].Substring(1, split[0].Length - 1);
					sql1 = split[1].Substring(0, split[1].Length - 1);
					
					dbaccess.ReadSQL("getFswRow", "SELECT * FROM finrpt2 "+
						" WHERE refnum = '"+sql0+"' and linenum = " + Convert.ToDecimal(sql1));

					DataTable getFswRow = dbaccess.DataSet.Tables["getFswRow"];

					if (getFswRow.Rows.Count>0)
					{
						MessageBox.Show("System is taking the value calculated in Financial Worksheet with Ref. No. " + sql0 +
						" at line " + sql1 + " for this row. Please make sure the value is up to date!");

						currentRow["pd0"] = getFswRow.Rows[0]["pd0"];
						currentRow["pd1"] = getFswRow.Rows[0]["pd1"];
						currentRow["pd2"] = getFswRow.Rows[0]["pd2"];
						currentRow["pd3"] = getFswRow.Rows[0]["pd3"];
						currentRow["pd4"] = getFswRow.Rows[0]["pd4"];
						currentRow["pd5"] = getFswRow.Rows[0]["pd5"];
						currentRow["pd6"] = getFswRow.Rows[0]["pd6"];
						currentRow["pd7"] = getFswRow.Rows[0]["pd7"];
						currentRow["pd8"] = getFswRow.Rows[0]["pd8"];
						currentRow["pd9"] = getFswRow.Rows[0]["pd9"];
						currentRow["pd10"] = getFswRow.Rows[0]["pd10"];
						currentRow["pd11"] = getFswRow.Rows[0]["pd11"];
						currentRow["pd12"] = getFswRow.Rows[0]["pd12"];

					}
					else
					{
						MessageBox.Show("No such record!");
					}


				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				ATL.BizModules.FINRPT.Code.finrptlocal form = new ATL.BizModules.FINRPT.Code.finrptlocal(this.dbaccess, textBox, currentRow);
				form.ShowDialog();
			}
        }

        #endregion
        #endregion

        #region Tab Control

        protected void tabControl_SelectionChanged(object sender, System.EventArgs e)
        {
            Button btnInsert = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Insert") as Button;
            Button btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
            Button btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
            Button btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
            Button btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;
            Button btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;


            switch ((sender as LXFClassLibrary.Controls.TabControl).SelectedIndex)
            {

                case 1:
                    {
                        tabName = "Settings";                        
                        break;
                    }

                case 2:
                    {
                        tabName = "Report";
                        btnDuplicate.Enabled = false;
                        btnDelete.Enabled = true;
                        btnInsert.Enabled = true;
                        btnUp.Enabled = true;
                        btnDown.Enabled = true;                        
                        break;
                    }
                
                default:
                    {

                        break;
                    }

            }
        }


        #endregion


        #region Own Function

        #region Get dtCheckAccAvailability

        private DataTable getDtCheckAccAvailability()
        {
            DataTable dtCheckAccAvailability = new DataTable();

            try
            {
                DataRow finrpth = dbaccess.DataSet.Tables["finrpth"].Rows[0];

                string sqlSelect = "select acm.accnum, acm.acctype, isnull(sum(gld.postamt), 0) as amount from acm left join gld"+ Common.DEFAULT_SYSTEM_YEAR +" gld on acm.accnum = gld.accnum ";
				string sqlLast = " group by acm.accnum, acm.acctype";

                string sqlWhere = string.Empty;

                dbaccess.ReadSQL("tempTable", sqlSelect + sqlWhere + sqlLast);
                dtCheckAccAvailability = dbaccess.DataSet.Tables["tempTable"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dtCheckAccAvailability;
        }

        #endregion

        #region Get dtSettingAccnum

        private DataTable GetdtSettingAccnum()
        {
            DataTable dt2 = new DataTable();
            try
            {
                dt2 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "select accnum from [finrpt1] where accnum is not null group by accnum");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt2;
        }

        #endregion

        #region Get Needed Field

        private string GetNeedField(DataTable dtCheckAccAvailability, DataTable dtSettingAccnum)
        {

            string neccecode = string.Empty;

            try
            {
                #region Check Needed Field
                int counter = 0;

                if (dtCheckAccAvailability.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in dtCheckAccAvailability.Rows)
                    {
                        if (dtSettingAccnum.Rows.Count > 0)
                        {
                            for (int a = 0; a < dtSettingAccnum.Rows.Count; a++)
                            {
                                if (dr1["accnum"].ToString() == dtSettingAccnum.Rows[a]["accnum"].ToString())
                                {
                                    break;
                                }
                                else
                                {
                                    if (a == dtSettingAccnum.Rows.Count - 1) //find until last row of settings detail
                                    {
                                        neccecode += dr1["accnum"].ToString() + "     ";
                                        counter++;
                                        if (counter % 5 == 0)
                                        {
                                            neccecode += "\n";
                                            counter = 0;
                                        }
                                    }
                                }
                            }
                        }
                        else 
                        {
                            neccecode += dr1["accnum"].ToString() + "     ";
                            counter++;
                            if (counter % 5 == 0)
                            {
                                neccecode += "\n";
                                counter = 0;
                            }
                        }
                    }
                }


                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return neccecode;
        }
        #endregion

        #region Get Extra Field

        private string GetExtraField(DataTable dtCheckAccAvailability, DataTable dtSettingAccnum)
        {
            string neccecode2 = string.Empty;

            try
            {
                #region Check Extra Field


                int counter = 0;
                if (dtSettingAccnum.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dtSettingAccnum.Rows)
                    {
                        for (int a = 0; a < dtCheckAccAvailability.Rows.Count; a++)
                        {
                            if (dr2["accnum"].ToString() == dtCheckAccAvailability.Rows[a]["accnum"].ToString())
                                break;
                            else
                            {
                                if (a == dtCheckAccAvailability.Rows.Count - 1)
                                {
                                    if (!BizFunctions.IsEmpty(dr2["accnum"]))
                                    {
                                        neccecode2 += dr2["accnum"].ToString() + "     ";
                                        counter++;
                                        if (counter % 5 == 0)
                                        {
                                            neccecode2 += "\n";
                                            counter = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return neccecode2;
        }


        #endregion

        #region Check Duplicate Data

        void CheckDuplicate(DataTable dt, string FieldToCheck, String ErrorMessage)
        {

            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted)
                    {
                        dt.Rows[i].ClearErrors();
                        if (i != dt.Rows.Count - 1)
                        {
                            if (!BizFunctions.IsEmpty(dt.Rows[i][FieldToCheck]))
                                for (int k = i + 1; k < dt.Rows.Count; k++)
                                    if (dt.Rows[k].RowState != DataRowState.Deleted)
                                    {
                                        if (!BizFunctions.IsEmpty(dt.Rows[k][FieldToCheck]))
                                            if (dt.Rows[i][FieldToCheck].ToString() == dt.Rows[k][FieldToCheck].ToString())
                                            {
                                                dt.Rows[i].SetColumnError(FieldToCheck, ErrorMessage);
                                                break;
                                            }
                                    }
                        }
                        if (dt.TableName == "finrpt1mirror")
                            if (BizFunctions.IsEmpty(dt.Rows[i]["gnum"]))
                                dt.Rows[i].SetColumnError("gnum", "Group num cannot be empty");

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        #endregion

        #region Check Matching on Group and Report

        private void CheckMatching()
        {
            try
            {
                DataTable finrpt1 = dbaccess.DataSet.Tables["finrpt1mirror"];
                //DataTable finrpt1 = TransferFinrpt1Table(dbaccess.DataSet.Tables["finrpt1"], dbaccess.DataSet.Tables["finrpt1mirror"]);
                DataTable finrpt2 = dbaccess.DataSet.Tables["finrpt2"];
                bool match = false;
                
                for (int i = 0; i < finrpt1.Rows.Count; i++)
                {
                    match = false;
                    if (finrpt1.Rows[i].RowState != DataRowState.Deleted)
                    {
                        finrpt1.Rows[i].ClearErrors();

                        for (int k = 0; k < finrpt2.Rows.Count; k++)
                        {
                            if(finrpt2.Rows[k].RowState != DataRowState.Deleted)
                                if (finrpt1.Rows[i]["gnum"].ToString() == finrpt2.Rows[k]["gnum"].ToString())
                                {
                                    match = true;
                                    break;
                                }
                        }

                        if (match == false)
                        {
                            finrpt1.Rows[i].SetColumnError("gnum", "Not in the report");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Check Handle For Save, Print and Refresh

        private void CheckHandle(DocumentHandleEventArgs e)
        {
            try
            {
                //dbaccess.DataSet.Tables["finrpt1"]. = TransferFinrpt1Table(dbaccess.DataSet.Tables["finrpt1"], dbaccess.DataSet.Tables["finrpt1Mirror"]);
                DataTable finrpt1 = dbaccess.DataSet.Tables["finrpt1"];
                DataTable finrpt1Mirror = dbaccess.DataSet.Tables["finrpt1Mirror"];
                DataTable finrpt2 = dbaccess.DataSet.Tables["finrpt2"];
                DataRow finrpth = dbaccess.DataSet.Tables["finrpth"].Rows[0];

                #region Check Finrpt1 Mirror Table

                if (finrpt1Mirror.Rows.Count > 0)
                {
                    foreach (DataRow dr in finrpt1Mirror.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            #region Check accnumfrom and accnumto

                            if (!BizFunctions.IsEmpty(dr["accnumfrom"]) & !BizFunctions.IsEmpty(dr["accnumto"]))
                            {
                                if (string.Compare(dr["accnumto"].ToString(),dr["accnumfrom"].ToString(), true)< 0)
                                {
                                    MessageBox.Show("Account Code To is bigger than Account Code Frm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    e.Handle = false;
                                    return;
                                }
                            }
                            else
                                if (!BizFunctions.IsEmpty(dr["accnumfrom"]))
                                {
                                    if (BizFunctions.IsEmpty(dr["accnumto"]))
                                    {
                                        MessageBox.Show("Account Code To cannot be empty when Account Code Frm is not empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        e.Handle = false;
                                        return;
                                    }
                                }
                                else
                                    if (!BizFunctions.IsEmpty(dr["accnumto"]))
                                    {
                                        if (BizFunctions.IsEmpty(dr["accnumfrom"]))
                                        {
                                            MessageBox.Show("Account Code Frm cannot be empty when Account Code To is not empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            e.Handle = false;
                                            return;
                                        }
                                    }
                                
                            
                            #endregion
                        }
                    }
                }

                #endregion

                finrpt1 = TransferFinrpt1Table(finrpt1, finrpt1Mirror);

				#region Check Formula

                foreach (DataRow dr in finrpt2.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr["formula"]))
                        {
                            ArrayList ary = new ArrayList();
                            ary = SplitFormula(dr["formula"].ToString());

                            if (ValidateFormula(ary) == false)
                            {
                                MessageBox.Show("Error in Formula", "Error Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }

                #endregion

                #region Check Duplicate Field

                #region Group Setting

                CheckDuplicate(finrpt1Mirror, "accnumfrom", "Duplicate Account Number From");
                CheckDuplicate(finrpt1Mirror, "accnumto", "Duplicate Account Number To");
                CheckDuplicate(finrpt1, "accnum", "Duplicate Account Number");

                #endregion

                #region Report Setting

                CheckDuplicate(finrpt2, "gnum", "Duplicate Group Number");

                #endregion

				if (BizValidate.CheckColumnError(this.dbaccess.DataSet, "finrpt1"))
                {
                    string strfinrpt1 = "select accnum,count(accnum) from finrpt1" +
                                          " group by refnum,accnum having count(accnum)>1";
                    DataSet dsfinrpt1=this.dbaccess.ReadSQLTemp("dtfinrpt1",strfinrpt1);
                    if(dsfinrpt1.Tables[0].Rows.Count>1)
                    {
                        foreach(DataRow dr in dsfinrpt1.Tables[0].Rows)
                        {
                            MessageBox.Show("Account Number overlap"+dr["accnum"], "Error Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }                  
                }

                if (BizValidate.CheckColumnError(this.dbaccess.DataSet, "finrpt1Mirror"))
                {
                    MessageBox.Show("Error in Setting Detail", "Error Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (BizValidate.CheckColumnError(this.dbaccess.DataSet, "finrpt2"))
                {
                    MessageBox.Show("Error in Report Detail", "Error Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                #endregion

     
                #region Check Setting Fields to be in Report

                CheckMatching();

                if (BizValidate.CheckColumnError(this.dbaccess.DataSet, "finrpt1mirror"))
                {
                    if (MessageBox.Show("Group Num Incomplete\nDo you wish to proceed?", "Incomplete Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        foreach (DataRow dr in dbaccess.DataSet.Tables["finrpt1mirror"].Rows)
                            if (dr.RowState != DataRowState.Deleted)
                                dr.ClearErrors();

                    }
                    else
                        e.Handle = false;
                }

                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Get Row Index

        private int GetRowIndex(string LineNumToSearch)
        {
            int RowIndex = 1;
            DataTable finrpt2 = dbaccess.DataSet.Tables["finrpt2"];
            try
            {
                foreach (DataRow dr in finrpt2.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                        RowIndex++;
                    else
                    {
                        if (dr["linenum"].ToString() == LineNumToSearch)
                        {
                            RowIndex--;
                            break;
                        }
                        else
                            RowIndex++;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return RowIndex;
        }

        #endregion

        #region Split Formula

        private ArrayList SplitFormula(string drFormula)
        {
            ArrayList aFormula = new ArrayList();
            string sNum = string.Empty;
            try
            {
                string sFormula = drFormula + "=";
                
                aFormula.Clear();
                for (int a = 0; a < sFormula.Length; a++)
                {
                    if (char.IsNumber(Convert.ToChar(sFormula.Substring(a, 1))))
                    {
                        sNum += sFormula.Substring(a, 1);
                    }
                    else
                    {
                        aFormula.Add(sNum);
                        sNum = string.Empty;
                        aFormula.Add(sFormula.Substring(a, 1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return aFormula;
        }
        

        #endregion

        #region Validate Formula

        private bool ValidateFormula(ArrayList ary)
        {
            Boolean noError = true;
            try
            {
                if (ary.Count > 1)
                {
                    for (int a = 0; a < ary.Count; a += 2)
                    {
                        if (!Tools.isNumeric(ary[a].ToString()))
                        {
                            noError = false;
                            break;
                        }
                    }

                    if (noError == true)
                    {
                        for (int a = 1; a < ary.Count; a += 2)
                        {
                            if (ary[a].ToString() != "+" & ary[a].ToString() != "-" & ary[a].ToString() != "*" & ary[a].ToString() != "/" & ary[a].ToString() != ":" & ary[a].ToString() != "=")
                            {
                                noError = false;
                                break;
                            }
                        }
                    }
                }
                else
                    noError = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return noError;
        }

        #endregion

        #region Transfer Finrpt1 Mirror to Finrpt1 

        private DataTable TransferFinrpt1Table(DataTable finrpt1, DataTable finrptMirror)
        {
            string sqlCommand = "";
            try
            {
                sqlCommand = "select gnum, max(isnull(gname, '')) as gname, accnumfrom, accnamefrom, accnumto, accnameto, refnum from [finrpt1mirror] group by gnum, accnumfrom, accnamefrom, accnumto, accnameto, refnum";
                DataTable dtabc = BizFunctions.ExecuteQuery(dbaccess.DataSet, sqlCommand);



                if (dtabc.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(finrpt1);
                    foreach (DataRow dr in dtabc.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            dbaccess.ReadSQL("dtaccnum", "select accnum, accname from acm where accnum >= '" + dr["accnumfrom"].ToString() + "' and accnum <= '" + dr["accnumto"].ToString() + "'");
                            if (dbaccess.DataSet.Tables["dtaccnum"].Rows.Count > 0)
                            {
                                foreach (DataRow drAccnum in dbaccess.DataSet.Tables["dtaccnum"].Rows)
                                {
                                    if (drAccnum.RowState != DataRowState.Deleted)
                                    {
                                        DataRow newfinrpt1 = finrpt1.NewRow();

                                        newfinrpt1["gnum"] = dr["gnum"].ToString();
                                        newfinrpt1["gname"] = dr["gname"].ToString();
                                        newfinrpt1["accnum"] = drAccnum["accnum"].ToString();
                                        newfinrpt1["accname"] = drAccnum["accname"].ToString();
                                        newfinrpt1["refnum"] = dr["refnum"].ToString();
                                        finrpt1.Rows.Add(newfinrpt1);

                                    }
                                }
                            }
                        
                        
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return finrpt1;
        }

        #endregion

        #endregion


    }
}