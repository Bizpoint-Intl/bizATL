/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_PAY.cs
 *	Description:    Purchase Payment Vouchers
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Xinyi			2006-08-17          Edit Form
 * Jer				2006-08-04			Add paste_handle, paste_onclick to enable/disable header columnchanged event
 * Jer				2006-07-10			apd extract, csh posting, gld posting
 * Jer				2006-07-08			converted xml to new core
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
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;
using DEMO.MDT;
using ATL.SupplierSearch;
using ATL.GeneralTools;


using Za.Evaluator;


namespace ATL.PCS
{
	public class Voucher_PCS : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region global variables

		protected string coynum = "SAF";
		protected DBAccess dbaccess = null;

		protected DataGrid dgOthers;
		protected bool opened = false;
		protected decimal piv1_oriamt = 0;
		protected decimal piv1_origstamt = 0;
		protected decimal piv1_postamt = 0;
		protected decimal piv1_gstamt = 0;
		protected decimal piv1_roriamt = 0;
		protected decimal piv1_rpostamt = 0;

		protected Hashtable formsCollection = null;
		protected int TabDetail;

		protected Button btnDelete;
		protected Button btnUp;
		protected Button btnDown;
		protected Button btnMark;
		protected Button btnDuplicate;
		protected Button btnExtract;

		string formdDetailName;
			
		protected Button getIncomeExpense;
		
		GenTools genFunctions = new GenTools();			
		#endregion

		public Voucher_PCS(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_PCS.xml", moduleName, voucherBaseHelpers)
		{
        }

		#region Steph - To stop users from accessing more than one voucher from Purchase Payments at the same time
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
			e.Condition = "pcsh.flag='PCS' AND pcsh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		protected override void AddVoucherDefaultCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherDefaultCondition(e);
			e.Condition = " (pcsh.status= '" + Common.DEFAULT_DOCUMENT_STATUSN + "' or " +
				" pcsh.status = '" + Common.DEFAULT_DOCUMENT_STATUSO + "' or " +
				" pcsh.status = '" + Common.DEFAULT_DOCUMENT_STATUSE + "')  " +
				" AND pcsh.flag='PCS' AND pcsh.systemyear = " + Common.DEFAULT_SYSTEM_YEAR;
		}
		#endregion

    	protected override void Document_Confirm_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Confirm_Handle(sender, e);

			DataRow pcsh = dbaccess.DataSet.Tables["pcsh"].Rows[0];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			DataTable csh = dbaccess.DataSet.Tables["csh"];
			DataTable gld = dbaccess.DataSet.Tables["gld"];

			setDefaults(dbaccess.DataSet, "piv1/csh/gld");

			AutoCalc();

			#region Steph - Error Checking!
			string DataEntryErrors = "Please check the following:";

			dbaccess.ReadSQL("checkAcm", "SELECT * FROM acm");

			DataTable getEmptyAcpcsh = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT accnum1 from [pcsh] WHERE isnull(accnum1,'') not in (SELECT accnum from [checkAcm])");
			if (getEmptyAcpcsh.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid Bank A/C No. in Header";
			}
			
			DataTable getEmptyAcPiv1 = BizFunctions.ExecuteQuery(dbaccess.DataSet, "SELECT refnum from [piv1]  WHERE isnull(accnum,'') not in (SELECT accnum FROM [checkAcm])");
			if (getEmptyAcPiv1.Rows.Count > 0)
			{
				DataEntryErrors = DataEntryErrors + "\n Invalid A/C No. in Detail";
			}

			if (DataEntryErrors != "Please check the following:")
			{
				MessageBox.Show(DataEntryErrors, "Confirm Unsuccessful");
				e.Handle = false;
			}
			else
			{

				#region Steph - Posting Time!!!

				//add row into csh
				DataRow addcsh = csh.Rows.Add(new object[] { });
				{
					BizFunctions.UpdateDataRow(pcsh,addcsh);
					addcsh["accnum"] = pcsh["accnum1"];
                    addcsh["oricredit"] = Convert.ToDecimal(pcsh["oriamt"]);
					addcsh["oriamt"] = Convert.ToDecimal(pcsh["oriamt"]) *-1;
					addcsh["postamt"] = Convert.ToDecimal(pcsh["oriamt"]) * Convert.ToDecimal(pcsh["exrate"])  *-1;
					addcsh["exramt"] = 0;
				}

				BizAccounts.PostGLD(dbaccess, "csh/piv1", "pcsh");

				#region steph - Need to post the header's remark into GLD.
				foreach (DataRow dr in gld.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						dr["detail"] = pcsh["remark"];
						if (dr["oricur"].ToString().Trim() == String.Empty || dr["oricur"] == System.DBNull.Value)
							dr["oricur"] = "SGD";  //Any rounding diff/GST must be in SGD.
					}
				}
				#endregion

				#endregion
			}
			#endregion

		}

		#region Tab Control
		protected override void Document_TabControl_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_TabControl_Handle(sender, e);

			TabDetail = (sender as LXFClassLibrary.Controls.TabControl).SelectedIndex;
		}
		protected override void Document_TabControl_OnSelectionChanged(object sender, DocumentEventArgs e)
		{
			base.Document_TabControl_OnSelectionChanged(sender, e);
			btnDelete = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Delete") as Button;
			btnUp = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Up") as Button;
			btnDown = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Down") as Button;
			btnMark = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Mark") as Button;
			btnDuplicate = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Duplicate") as Button;
			btnExtract = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "btn_Document_Extract") as Button;

			switch ((sender as LXFClassLibrary.Controls.TabControl).SelectedIndex)
			{
				case 0:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnExtract.Enabled = true;
					btnMark.Enabled = true;
					break;
				case 1:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnExtract.Enabled = false;
					btnMark.Enabled = true;
					break;
				case 2:
					btnDelete.Enabled = true;
					btnUp.Enabled = true;
					btnDown.Enabled = true;
					btnDuplicate.Enabled = true;
					btnExtract.Enabled = false;
					btnMark.Enabled = true;
					break;
			}

		}
		#endregion
		
		#region Document Handle
				
		protected override void Document_Preview_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Preview_Handle (sender, e);
			DataRow pcsh = e.DBAccess.DataSet.Tables["pcsh"].Rows[0];

			if (pcsh["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
			{
				if (BizValidate.CheckRowState(e.DBAccess.DataSet, "pcsh/apd/piv1"))
				{
					MessageBox.Show("Please save before Previewing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					e.Handle = false;
				}
			}
		}

		#endregion

		#region Document Event

		#region Form Load

		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad (sender, e);
			opened = true;

			DataRow pcsh = e.DBAccess.DataSet.Tables["pcsh"].Rows[0];
			DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];

			this.formdDetailName = (e.FormsCollection["header"] as Form).Name;
			string headerFormName = (e.FormsCollection["header"] as Form).Name;
			string ccDistFormName = (e.FormsCollection["CcDist"] as Form).Name;

			if (pcsh["status"].ToString() == "N")
			{
				BizFunctions.GetTrandate(headerFormName, "pcsh_trandate", pcsh);
			}

			pcsh["systemyear"] = Common.DEFAULT_SYSTEM_YEAR;

			this.dbaccess = e.DBAccess;
			getIncomeExpense = BizXmlReader.CurrentInstance.GetControl(ccDistFormName, "btn_getIncomeExpense") as Button;
			getIncomeExpense.Click += new EventHandler(getIncomeExpense_Click);

			e.DBAccess.DataSet.Tables["pcsh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_pcsh_ColumnChanged);
            e.DBAccess.DataSet.Tables["piv1"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_PIV1_ColumnChanged);
		}
		
		void getIncomeExpense_Click(object sender, EventArgs e)
		{
			DataTable pivc = dbaccess.DataSet.Tables["pivc"];

			string strGetIE = "SELECT accnum,SUM(oriamt) AS oriamt,oricur,exrate FROM [piv1] GROUP BY accnum,oricur,exrate HAVING SUM(oriamt)<>0 " +
				" UNION ALL SELECT accnum2,bankchg,oricur,exrate FROM pcsh WHERE ISNULL(bankchg,0) <>0 " +
				" UNION ALL SELECT accnum3,discamt,oricur,exrate FROM pcsh WHERE ISNULL(discamt,0) <>0 " +
				" UNION ALL SELECT accnum4,flexamt,oricur,exrate FROM pcsh WHERE ISNULL(flexamt,0) <>0";

			DataTable getIE = BizFunctions.ExecuteQuery(dbaccess.DataSet,strGetIE);

			DialogResult result = MessageBox.Show("The entries in this page will be reset! Continue?", "Clear Data?", MessageBoxButtons.YesNo);

			if (result == DialogResult.Yes)
			{
				BizFunctions.DeleteAllRows(pivc);

				foreach (DataRow dr in getIE.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						DataRow addPivc = pivc.Rows.Add(new object[] { });
						addPivc["accnum"] = dr["accnum"];
						addPivc["oriamt"] = dr["oriamt"];
						addPivc["oricur"] = dr["oricur"];
						addPivc["exrate"] = dr["exrate"];
					}
				}
			}
			AutoCalc();
		}
		#endregion

		#region Reopen Handle

		protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Reopen_Handle(sender, e);
			DataRow pcsh = this.dbaccess.DataSet.Tables["pcsh"].Rows[0];
			DataTable piv1  =this.dbaccess.DataSet.Tables["piv1"];

			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM CSH" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + pcsh["refnum"].ToString().Trim() + "'");
			e.DBAccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM GLD" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum='" + pcsh["refnum"].ToString().Trim() + "'");
		}
		#endregion

		protected override void Document_Delete_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Delete_OnClick(sender, e);
			AutoCalc();
		}

		#region Refresh
        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow pcsh = e.DBAccess.DataSet.Tables["pcsh"].Rows[0];
            DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
            DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			setDefaults(dbaccess.DataSet, "pcsh/piv1");
			
			AutoCalc();

			#region Steph - To set the correct value for oriamt, postamt using mdt
			MDTReader.SetCorrectValue(ref this.dbaccess, "PIV1", "PAY");
			MDTReader.SetCorrectValue(ref this.dbaccess, "pcsh", "PAY");
			#endregion
		}
		#endregion
		#endregion

		#region SaveBegin

		protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_SaveBegin_OnClick (sender, e);
            DataRow pcsh = e.DBAccess.DataSet.Tables["pcsh"].Rows[0];
			DataTable piv1 = e.DBAccess.DataSet.Tables["piv1"];
            DataTable csh = e.DBAccess.DataSet.Tables["csh"];
			
			#region Steph - This is to update the detail tables
			
			foreach (DataRow dr in piv1.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
					BizFunctions.UpdateDataRow(pcsh, dr, "user/flag/status/created/modified");
			}
			#endregion
		}

		#endregion

		protected override void Document_Paste_OnClick(object sender, DocumentEventArgs e)
		{
			base.Document_Paste_OnClick (sender, e);
			e.DBAccess.DataSet.Tables["pcsh"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_pcsh_ColumnChanged);
		}	

		#region ColumnChangedEvents

		#region column change

		private void Voucher_pcsh_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			DataRow pcsh = dbaccess.DataSet.Tables["pcsh"].Rows[0];

			switch(e.Column.ColumnName)
			{
				case "apnum":
					#region Steph - Pull info from ARM
					dbaccess.ReadSQL("getApmInfo", "SELECT apnum,apname,ptc,address,phone,hp,fax,ptnum,gstgrpnum,oricur,accnum FROM apm where apnum ='" + e.Row["apnum"].ToString().Trim() + "'");

					if (dbaccess.DataSet.Tables["getApmInfo"].Rows.Count > 0)
					{
						DataRow getApmInfo = dbaccess.DataSet.Tables["getApmInfo"].Rows[0];
						e.Row["detail"] = getApmInfo["apname"];

                        #region san* - Save contact info if not empty
                        //San*
                        if (e.Row["contact"].ToString().Trim() != "")
                        {
                            pcsh["ptc"] = e.Row["contact"];
                        }
						//e.Row["contact"] = getApmInfo["ptc"];
                        #endregion - San_End

						e.Row["address"] = getApmInfo["address"];
						e.Row["phone"] = getApmInfo["phone"];
						e.Row["hp"] = getApmInfo["hp"];
						e.Row["fax"] = getApmInfo["fax"];

						if (e.Row["pcsherms"].ToString().Trim() == "" || e.Row["pcsherms"] == System.DBNull.Value)
							e.Row["pcsherms"] = getApmInfo["ptnum"];
						if (e.Row["gstgrpnum"].ToString().Trim() == "" || e.Row["gstgrpnum"] == System.DBNull.Value)
							e.Row["gstgrpnum"] = getApmInfo["gstgrpnum"];
						if (e.Row["oricur"].ToString().Trim() == "" || e.Row["oricur"] == System.DBNull.Value)
							e.Row["oricur"] = getApmInfo["oricur"];
					}
					else
					{
						e.Row["detail"] = "";
						e.Row["contact"] = "";
						e.Row["address"] = "";
						e.Row["phone"] = "";
						e.Row["hp"] = "";
						e.Row["fax"] = "";
						e.Row["pcsherms"] = "";
						e.Row["gstgrpnum"] = "";
						e.Row["oricur"] = "";
					}
				    break;
                    #endregion
				case "oricur":
					#region set exrate
					e.Row.BeginEdit();
					string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					this.dbaccess.ReadSQL("exrate", exrStr);
					if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					{
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
						e.Row["exrate"] = exrate;
					}
					e.Row.EndEdit();
					break;
					#endregion
				case "gstgrpnum":
					#region set gstper
					
					e.Row.BeginEdit();
					this.dbaccess.ReadSQL("gstm","SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='"+e.Row[e.Column.ColumnName].ToString()+"'");
					if(this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
					{
						if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
						{
							e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)pcsh["trandate"]);
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
					string strexr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + e.Row[e.Column.ColumnName].ToString() + "'";
					this.dbaccess.ReadSQL("exrate", strexr);
					if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
					{
						decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(e.Row["trandate"])) + ""]);
						e.Row["exrate"] = exrate;
					}

					pcsh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(pcsh["trandate"]));

					e.Row.EndEdit();
					break;
					#endregion				
			}

		}

        private void Voucher_PIV1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataRow pcsh = this.dbaccess.DataSet.Tables["pcsh"].Rows[0];

            switch (e.Column.ColumnName)
            {
                case "gstgrpnum":
                    string strGSTM = "SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='" + e.Row["gstgrpnum"].ToString() + "'";
                    this.dbaccess.ReadSQL("gstm", strGSTM);
                    if (this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
                    {
                        if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
                        {
                            if (e.Row["gstper"] == System.DBNull.Value)
                            {
                                e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)pcsh["trandate"]);
                            }
                            else
                            {
                                if ((decimal)e.Row["gstper"] == 0)
                                {
                                    e.Row["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)pcsh["trandate"]);
                                }
                            }
                        }
                        else
                        {
                            e.Row["gstper"] = 0;
                        }
                    }
                    break;

                    #endregion
            }
        }
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
				
		#region Auto Calculate
		private void AutoCalc()
		{
			DataRow pcsh = dbaccess.DataSet.Tables["pcsh"].Rows[0];
			DataTable PCSH = dbaccess.DataSet.Tables["pcsh"];
			DataTable apd = dbaccess.DataSet.Tables["apd"];
			DataTable excsh = dbaccess.DataSet.Tables["excsh"];
			DataTable piv1 = dbaccess.DataSet.Tables["piv1"];
			
			setDefaults(dbaccess.DataSet, "PCSH/piv1");

			if (pcsh["refnum"].ToString().Trim().Contains("PCS"))
			{
				if (pcsh["docunum"] == System.DBNull.Value || pcsh["docunum"].ToString().Trim() == String.Empty)
				{
					pcsh["docunum"] = pcsh["refnum"];
				}
			}			
			           
			#region Steph -  To get pd from pd (nonYear) table.

			pcsh["period"] = BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(pcsh["trandate"]));

			#endregion


			#region initialise values
			piv1_oriamt = 0;
			piv1_origstamt = 0;
			piv1_postamt = 0;
			piv1_roriamt = 0;
			piv1_rpostamt = 0;
			piv1_gstamt = 0;
			decimal gstDifference = 0;
			#endregion
				
			#region piv1

			dbaccess.ReadSQL("checkRevExpAccnum", "SELECT accnum,accnum2 FROM apm WHERE apnum = '" + pcsh["apnum"].ToString().Trim() + "'");

			if ((bool)pcsh["inclgst"])
			{
				#region Steph - Inclusive GST calculations

				dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
					" WHERE gstgrpnum ='" + pcsh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in piv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
					{
						BizFunctions.UpdateDataRow(pcsh, dr, "createdby/apnum/docunum/oricur/exrate/gstgrpnum/gstper");

						if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
						{
							if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
							}
						}

						dr["roriamt"] = BizFunctions.Round((Convert.ToDecimal(dr["dqty"]) * Convert.ToDecimal(dr["price"])) - Convert.ToDecimal(dr["discamt"]));
						dr["rpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(dr["exrate"]));
						//dbaccess.ReadSQL("checkGST", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm "+
						//    " WHERE gstgrpnum ='"+dr["gstgrpnum"].ToString().Trim()+"' AND gsttype=3");
						if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
						{
							//steph  - do not have to calculate the gst, allow user to key in manually.
						}
						else
						{
							dr["dorigstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) * Convert.ToDecimal(pcsh["gstper"]) / (100 + Convert.ToDecimal(pcsh["gstper"])));
						}
						dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
						dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
						dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));
						dr["dgrosamt"] = dr["doriamt"];

						piv1_oriamt += (decimal)dr["doriamt"];
						piv1_origstamt += (decimal)dr["dorigstamt"];
						piv1_postamt += (decimal)dr["dpostamt"];
						piv1_gstamt += (decimal)dr["dgstamt"];
					}
				}
				#region Steph - Check the gst amt differences and add/deduct from the first entry of piv1
				if (dbaccess.DataSet.Tables["checkGST"].Rows.Count > 0)
				{
					//steph  - do not have to calculate the gst, allow user to key in manually.
				}
				else
				{
					if (piv1.Rows.Count > 0)
					{
						decimal headerGst = BizFunctions.Round((piv1_oriamt + piv1_origstamt) * Convert.ToDecimal(pcsh["gstper"]) / (100 + Convert.ToDecimal(pcsh["gstper"])));
						if (headerGst != piv1_origstamt)
						{
							piv1.Rows[0]["dorigstamt"] = Convert.ToDecimal(piv1.Rows[0]["dorigstamt"]) + headerGst - piv1_origstamt;
						}
					}

					#region initialise values
					piv1_oriamt = 0;
					piv1_origstamt = 0;
					piv1_postamt = 0;
					piv1_gstamt = 0;
					#endregion

					foreach (DataRow dr in piv1.Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							dr["doriamt"] = BizFunctions.Round(Convert.ToDecimal(dr["roriamt"]) - Convert.ToDecimal(dr["dorigstamt"]));
							dr["dgrosamt"] = dr["doriamt"];
							dr["dgstamt"] = BizFunctions.Round(Convert.ToDecimal(dr["dorigstamt"]) * Convert.ToDecimal(dr["exrate"]));
							dr["dpostamt"] = BizFunctions.Round(Convert.ToDecimal(dr["rpostamt"]) - Convert.ToDecimal(dr["dgstamt"]));

							piv1_oriamt += (decimal)dr["doriamt"];
							piv1_origstamt += (decimal)dr["dorigstamt"];
							piv1_postamt += (decimal)dr["dpostamt"];
							piv1_gstamt += (decimal)dr["dgstamt"];
							piv1_roriamt += (decimal)dr["roriamt"];
							piv1_rpostamt += (decimal)dr["rpostamt"];

						}
					}
				}
				#endregion
				#endregion
			}
			else
			{
				#region Steph - Exclusive GST calculations

				dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm " +
					" WHERE gstgrpnum ='" + pcsh["gstgrpnum"].ToString().Trim() + "' AND gsttype=3");

				foreach (DataRow dr in piv1.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						BizFunctions.UpdateDataRow(pcsh, dr);
                        //BizFunctions.UpdateDataRow(pcsh, dr, "createdby/apnum/docunum/oricur/exrate/gstgrpnum/gstper");
                        BizFunctions.UpdateDataRow(pcsh, dr, "createdby/apnum/docunum/oricur/exrate");

						if (dr["accnum"] == System.DBNull.Value || dr["accnum"].ToString().Trim() == String.Empty)
						{
							if (dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows.Count > 0)
							{
								dr["accnum"] = dbaccess.DataSet.Tables["checkRevExpAccnum"].Rows[0]["accnum2"];
							}
						}

						dr["dgrosamt"] = BizFunctions.Round((decimal)dr["dqty"] * (decimal)dr["price"]);
						dr["doriamt"] = (decimal)dr["dgrosamt"];
						//dbaccess.ReadSQL("checkGST2", "SELECT gstgrpnum, gstgrpname, gsttype FROM gstm "+
						//    " WHERE gstgrpnum ='"+dr["gstgrpnum"].ToString().Trim()+"' AND gsttype=3");
						if (dbaccess.DataSet.Tables["checkGST2"].Rows.Count > 0)
						{
							//steph  - do not have to calculate the gst, allow user to key in manually.
						}
						else
						{
							dr["dorigstamt"] = BizFunctions.Round((decimal)dr["doriamt"] * ((decimal)dr["gstper"] / 100));
						}
						dr["dpostamt"] = BizFunctions.Round((decimal)dr["doriamt"] * (decimal)dr["exrate"]);
						dr["dgstamt"] = BizFunctions.Round((decimal)dr["dorigstamt"] * (decimal)dr["exrate"]);
						dr["roriamt"] = BizFunctions.Round((decimal)dr["doriamt"] + (decimal)dr["dorigstamt"]);
						dr["rpostamt"] = BizFunctions.Round((decimal)dr["dpostamt"] + (decimal)dr["dgstamt"]);

						piv1_oriamt += (decimal)dr["doriamt"];
						piv1_origstamt += (decimal)dr["dorigstamt"];
						piv1_postamt += (decimal)dr["dpostamt"];
						piv1_gstamt += (decimal)dr["dgstamt"];
						piv1_roriamt += (decimal)dr["roriamt"];
						piv1_rpostamt += (decimal)dr["rpostamt"];

					}
				}
				#endregion
			}

			#region set Header Page oriamt/origstamt/oritotalamt/postamt/gstamt/totalamt
			pcsh["pivtotal"] = piv1_oriamt+piv1_origstamt;
			pcsh["oriamt"] = piv1_oriamt + piv1_origstamt;
			pcsh["origstamt"] = piv1_origstamt;
			pcsh["oritotalamt"] = (decimal)pcsh["oriamt"] + (decimal)pcsh["origstamt"];			
			pcsh["gstamt"] = BizFunctions.Round((decimal)pcsh["postamt"] * ((decimal)pcsh["gstper"] / 100));
			pcsh["totalamt"] = (decimal)pcsh["postamt"] + (decimal)pcsh["gstamt"];
			#endregion
			#endregion

            #region update piv1
            foreach (DataRow dr in piv1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted) // Do this step for those row that is not deleted
                {
                    BizFunctions.UpdateDataRow(pcsh, dr, "trandate");
                }
            }
            #endregion
        }
		#endregion
				
		protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
		{
			base.Document_Print_OnClick(sender, e);
			DataRow pcsh = e.DBAccess.DataSet.Tables["pcsh"].Rows[0];
			Hashtable selectedCollection = new Hashtable();
			switch (e.ReportName)
			{
				case "Payment (Creditors)":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("apm", "SELECT top 1 * FROM apm WHERE apnum='" + pcsh["apnum"].ToString().Trim() + "'");
					selectedCollection.Add("apd", "SELECT * FROM apd" + Common.DEFAULT_SYSTEM_YEAR + " WHERE refnum = '" + pcsh["refnum"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;
				case "Payment (Others)":
					selectedCollection.Add("coy", "SELECT * FROM coy");
					selectedCollection.Add("apm", "SELECT top 1 * FROM apm where apnum='" + pcsh["apnum"].ToString().Trim() + "'");
					selectedCollection.Add("piv1", "SELECT * FROM piv1 where refnum = '" + pcsh["refnum"].ToString().Trim() + "'");
					e.DBAccess.ReadSQL(selectedCollection);
					e.DataSource = e.DBAccess.DataSet;
					break;			
			}
		}

		private decimal getExrate(string Currency, object date1)
		{
			string exrStr = "Select * FROM exr" + Common.DEFAULT_SYSTEM_YEAR + " WHERE oricur = '" + Currency + "'";
			this.dbaccess.ReadSQL("exrate", exrStr);
			if (this.dbaccess.DataSet.Tables["exrate"].Rows.Count != 0)
			{
				decimal exrate = Convert.ToDecimal(this.dbaccess.DataSet.Tables["exrate"].Rows[0]["rate" + BizAccounts.GetPeriod(dbaccess, Convert.ToDateTime(date1))]);
				return exrate;
			}
			else
			{
				return 0;
			}
		}

        //protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        //{
        //    base.AddDetailF3Condition(sender, e);

        //    switch (e.MappingName)
        //    {
        //        case "gstgrpnum":
        //            AutoCalc();
        //            string strGSTM = "SELECT gstgrpnum,gstgrpname,gsttype FROM gstm WHERE gstgrpnum='" + e.CurrentRow["gstgrpnum"].ToString() + "'";
        //            this.dbaccess.ReadSQL("gstm", strGSTM);
        //            if (this.dbaccess.DataSet.Tables["gstm"].Rows.Count != 0)
        //            {
        //                if ((int)this.dbaccess.DataSet.Tables["gstm"].Rows[0]["gsttype"] == 1)
        //                {
        //                    if (e.CurrentRow["gstper"] == System.DBNull.Value)
        //                    {
        //                        e.CurrentRow["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)e.CurrentRow["trandate"]);
        //                    }
        //                    else
        //                    {
        //                        if ((decimal)e.CurrentRow["gstper"] == 0)
        //                        {
        //                            e.CurrentRow["gstper"] = BizAccounts.GetGstRate(this.dbaccess, (DateTime)e.CurrentRow["trandate"]);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    e.CurrentRow["gstper"] = 0;
        //                }
        //            }
        //            break;
        //    }
        //}
	}
}