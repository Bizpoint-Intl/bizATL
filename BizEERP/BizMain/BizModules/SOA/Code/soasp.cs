using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;
using BizRAD.BizApplication;
using BizRAD.BizBase;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace ATL.SOA
{
    public partial class SOASP : Form
    {
        protected string projectPath = null;
        protected DBAccess dbAccess = null;
        protected Form frmThis = null;

        protected AutoCompleteStringCollection ArnumList = new AutoCompleteStringCollection();
        protected AutoCompleteStringCollection ArnameList = new AutoCompleteStringCollection();

        protected string footer1_txt = null;
        protected string footer2_txt = null;
        protected DataTable armTmp = null;
        protected bool exitLoop = false;
        protected Soafrm soafrm = null;

        public SOASP()
        {
            InitializeComponent();

            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
            this.frmThis = this;
            this.dbAccess = new DBAccess();

            this.frmThis.ShowDialog();
        }

        private void SOASP_Load(object sender, EventArgs e)
        {
            Hashtable selectCollection = new Hashtable();
            selectCollection.Add("coy", "select coy, coyname from coy");
            // only select Coporate and Non-Trade (ignore VIP and null)
            //selectCollection.Add("arm", "select arnum,arname from arm where (artype='CC' or artype='NT') and arnum in (select distinct arnum FROM ard" + Common.DEFAULT_SYSTEM_YEAR + ")");
			//selectCollection.Add("arm", "select arnum,arname,address from arm where (active = 'true') and arnum in (select distinct arnum FROM ard" + Common.DEFAULT_SYSTEM_YEAR + ")");
			selectCollection.Add("arm", "select arnum,arname,address from arm where arnum in (select distinct arnum FROM ard" + Common.DEFAULT_SYSTEM_YEAR + ")");
			//selectCollection.Add("soa_footer1", "select * from ptxt where ModuleCode='SOA' and ReferenceCode='Footer1'");
			//selectCollection.Add("soa_footer2", "select * from ptxt where ModuleCode='SOA' and ReferenceCode='Footer2'");
            selectCollection.Add("pd", "select * from pd" + Common.DEFAULT_SYSTEM_YEAR + " where period=1");
            DataSet dsTmp = this.dbAccess.ReadSQLTemp(selectCollection);

            this.dtp_DateTo.Value = System.DateTime.Now;
            this.dtp_DateFrom.Value = (DateTime)dsTmp.Tables["pd"].Rows[0]["pdstart"];

            foreach(DataRow dr in dsTmp.Tables["coy"].Rows)
                this.cmb_coy.Items.Add(dr["coy"]);

            this.cmb_coy.SelectedIndex = 0;
            this.cmb_coy.Enabled = false; // hardcoded for homefix since only 1 company

			//this.footer1_txt = dsTmp.Tables["soa_footer1"].Rows[0]["detail"].ToString().Trim();
			//this.footer2_txt = dsTmp.Tables["soa_footer2"].Rows[0]["detail"].ToString().Trim();

            this.armTmp = dsTmp.Tables["arm"];
            
            foreach (DataRow dr in armTmp.Select("", "arnum"))
            {
                this.ArnumList.Add(dr["arnum"].ToString().Trim());
                this.ArnameList.Add(dr["arname"].ToString().Trim());
            }

            this.tb_arnum.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.tb_arnum.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.tb_arnum.AutoCompleteCustomSource = this.ArnumList;

			//added arnum2 for viewing of SOA from arnum to arnum2 as requested by May -24/3/08
			this.tb_arnum2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.tb_arnum2.AutoCompleteSource = AutoCompleteSource.CustomSource;
			this.tb_arnum2.AutoCompleteCustomSource = this.ArnumList;

            this.tb_arname.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.tb_arname.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.tb_arname.AutoCompleteCustomSource = this.ArnameList;
        }

        #region chk_arnum_CheckedChanged
        private void chk_arnum_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                this.tb_arnum.Text = String.Empty;
                this.tb_arnum.Enabled = false;
                this.tb_arname.Text = String.Empty;
                this.tb_arname.Enabled = false;
                this.rb_arnum.Enabled = false;
                this.rb_arname.Enabled = false;
            }
            else
            {
                if(this.rb_arnum.Checked)
                    this.tb_arnum.Enabled = true;
                if(this.rb_arname.Checked)
                    this.tb_arname.Enabled = true;
                this.rb_arnum.Enabled = true;
                this.rb_arname.Enabled = true;
            }
        }
        #endregion

        #region rb_arnum_CheckedChanged & rb_arname_CheckedChanged
        private void rb_arnum_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                this.tb_arnum.Enabled = true;
				this.tb_arnum2.Enabled = true; //added by ivan 24/3/08
                this.rb_arname.Checked = false;
                this.tb_arname.Enabled = false;
                this.tb_arname.Text = String.Empty;
            }
        }

        private void rb_arname_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                this.tb_arname.Enabled = true;
                this.rb_arnum.Checked = false;
                this.tb_arnum.Enabled = false;
                this.tb_arnum.Text = String.Empty;

				//Added by Ivan 24/3/08
				this.tb_arnum2.Enabled = false;
				this.tb_arnum2.Text = String.Empty;
            }
        }
        #endregion

        #region btnCancel_Click, btnPreview_Click & btnPrint_Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.frmThis.Dispose();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (this.checkFilterSelection()) return;
            this.PrintDocument(true);  
            //this.btnCancel_Click(sender, e);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (this.checkFilterSelection()) return; 
            this.PrintDocument(false);
            //this.btnCancel_Click(sender, e);
        }
        #endregion

        #region checkFilterSelection
        private bool checkFilterSelection()
        {
            bool checkedflag = false;
            
            if (Convert.IsDBNull(this.dtp_DateFrom.Value) || Convert.IsDBNull(this.dtp_DateFrom.Value))
            {
                MessageBox.Show("Cut-Off Dates cannot be empty !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkedflag = true;
            }

            if (!this.chk_arnum.Checked)
            {
                if (this.rb_arnum.Checked)
                {
                    if (this.tb_arnum.Text == String.Empty)
                    {
                        MessageBox.Show("Customer Code must be selected !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        checkedflag = true;
                    }
                    else
                    {
                    }
                }
                else
                {
                    if (this.tb_arname.Text == String.Empty)
                    {
                        MessageBox.Show("Customer Name must be selected !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        checkedflag = true;
                    }
                    else
                    {
                    }
                }
            }

            return checkedflag;
        }
        #endregion

        #region PrintDocument

        private void PrintDocument(bool showPreview)
        {
            this.soafrm = new Soafrm(); 
            this.exitLoop = false;
            this.soafrm.buttonQuit.Click += new EventHandler(buttonQuit_Click);

            string safeDateFrom = BizFunctions.GetSafeDateString(this.dtp_DateFrom.Value);
            string safeDateTo = BizFunctions.GetSafeDateString(this.dtp_DateTo.Value);

            string command = null;

			if (this.rb_arnum.Checked && !this.chk_arnum.Checked)
			{
				command = "arnum >='" + tb_arnum.Text.Trim() + "' and arnum <='"+tb_arnum2.Text.Trim()+"'";
			}
			else if (!this.rb_arnum.Checked && !this.chk_arnum.Checked)
				command = "arname ='" + tb_arname.Text.Trim() + "'";

            #region add parameter field for statementdate/footer1/footer2

            ParameterFields[] pf = new ParameterFields[3];

            ParameterFields paramFields = new ParameterFields();

            ParameterField paramField_1 = new ParameterField();
            paramField_1.ParameterFieldName = "statementdate";
            ParameterDiscreteValue discreteVal_1 = new ParameterDiscreteValue();
            discreteVal_1.Value = this.dtp_DateTo.Value;
            ParameterValues val_1 = new ParameterValues();
            val_1.Add(discreteVal_1);
            paramField_1.CurrentValues = val_1;

            paramFields.Add(paramField_1);

            ParameterField paramField_2 = new ParameterField();
            paramField_2.ParameterFieldName = "footer1";
            ParameterDiscreteValue discreteVal_2 = new ParameterDiscreteValue();
            discreteVal_2.Value = this.footer1_txt;
            ParameterValues val_2 = new ParameterValues();
            val_2.Add(discreteVal_2);
            paramField_2.CurrentValues = val_2;

            paramFields.Add(paramField_2);

            ParameterField paramField_3 = new ParameterField();
            paramField_3.ParameterFieldName = "footer2";
            ParameterDiscreteValue discreteVal_3 = new ParameterDiscreteValue();
            discreteVal_3.Value = this.footer2_txt;
            ParameterValues val_3 = new ParameterValues();
            val_3.Add(discreteVal_3);
            paramField_3.CurrentValues = val_3;

            paramFields.Add(paramField_3);

            pf[0] = paramFields;

            #endregion

            DataRow[] armSelected = armTmp.Select(command, "arnum");

            string defaultPath = Environment.CurrentDirectory;

            if (armSelected.Length != 0)
            {
                // put above loop because PicoGuards only 1 entry in coy (i.e. 1 company)
                BizFunctions.SetCoyForPrinting(this.dbAccess, "coy = '" + this.cmb_coy.SelectedItem.ToString() + "'");

                foreach (DataRow dr in armSelected)
                {
                    Environment.CurrentDirectory = defaultPath;

                    if (this.exitLoop) break;

                    string arnum = dr["arnum"].ToString().Trim();
                    
                    soafrm.labelDescription.Text = (this.chk_arnum.Checked) ? " [ All Customers ] " : "";
                    soafrm.labelDescription.Text = soafrm.labelDescription.Text + "[ " + arnum + " : " + dr["arname"].ToString().Trim() + " ]";
                    soafrm.labelDescription.Text = soafrm.labelDescription.Text + " [ " + safeDateFrom + " to " + safeDateTo + " ] ";

                    Parameter[] parameters = new Parameter[7];
                    parameters[0] = new Parameter("@arnum", arnum);
                    parameters[1] = new Parameter("@dateFrom", safeDateFrom);
                    parameters[2] = new Parameter("@dateTo", safeDateTo);
                    parameters[3] = new Parameter("@tableName", "ard" + Common.DEFAULT_SYSTEM_YEAR);
                    parameters[4] = new Parameter("@coyID", this.cmb_coy.SelectedItem.ToString()); // not used for homefix 
                    parameters[5] = new Parameter("@showOutstanding", this.chk_showOutstanding.Checked);
                    parameters[6] = new Parameter("@groupInvoices", this.chk_groupInvoices.Checked);
                    
                    try
                    {
						//DataSet ds_ard = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_Print_StatementOfAccounts_Monthly", ref parameters);
						DataSet ds_ard = this.dbAccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_Print_StatementOfAccounts", ref parameters);
                        ds_ard.Tables[0].TableName = "arm";
                        ds_ard.Tables[1].TableName = "soa_summary";
                        ds_ard.Tables[2].TableName = "soa_details";

						

                        if (ds_ard.Tables[2].Rows.Count != 0)
                        {
                            DataTable cloneCoy = this.dbAccess.DataSet.Tables["coy"].Copy();
                            cloneCoy.TableName = "coy";
                            ds_ard.Tables.Add(cloneCoy);

							ReportDocument crReportDocument = new ReportDocument();
							//crReportDocument.Load(this.projectPath + @"\SOA\Report\StatementOfAccounts_Monthly.rpt");
							crReportDocument.Load(this.projectPath + @"\SOA\Report\StatementOfAccounts.rpt");
                            crReportDocument.SetDataSource(ds_ard);

                            if (showPreview)
                            {
                                soafrm.crystalReportViewer.ReportSource = crReportDocument;
                                soafrm.crystalReportViewer.ParameterFieldInfo = pf[0];
                                soafrm.ShowDialog();
                            }
                            else
                            {
                                PrintDocument printDocument = new PrintDocument();
                                crReportDocument.PrintOptions.PrinterName = printDocument.PrinterSettings.PrinterName;
                                
                                crReportDocument.SetParameterValue("statementdate", this.dtp_DateTo.Value.ToShortDateString());
                                crReportDocument.SetParameterValue("footer1", this.footer1_txt);
                                crReportDocument.SetParameterValue("footer2", this.footer2_txt);
                                
                                crReportDocument.PrintToPrinter(1, true, 0, 0);
								printDocument.Dispose();
                            }

							crReportDocument.Dispose();
							
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
				
                this.soafrm.Dispose();
            }
            else
            {
                MessageBox.Show("No records found !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        protected void buttonQuit_Click(object sender, EventArgs e)
        {
            this.exitLoop = true;
            this.soafrm.Close();
        }

		private void tb_arnum_KeyDown(object sender, KeyEventArgs e)
		{
			F2BaseHelper f2BaseHelper;

			if (e.KeyCode == Keys.F2)
			{
				//if(salesman.Checked)
				//f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", sender, "arnum1", null, "active = '1' and issalesman = '1' and isdriver = '1'", F2Type.Sort);
				//else
				//	f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", sender, "arnum1", null, "active = '1' and isdriver = '1'", F2Type.Sort);

				//Luixia need to see all -ivan 1/3/08
				//f2BaseHelper = new F2BaseHelper("F2GridInfo_ARM.xml", sender, "arnum1", "arnum like '" + (sender as TextBox).Text.Trim() + "%'", "active = '1'", F2Type.Sort);
				f2BaseHelper = new F2BaseHelper("F2GridInfo_ARM.xml", sender, "arnum1", "arnum like '" + (sender as TextBox).Text.Trim() + "%'", "1=1", F2Type.Sort);

				f2BaseHelper.F2_Load();

				if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
				{
					//this.arnum_from = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
					(sender as TextBox).Text = f2BaseHelper.F2Base.CurrentRow["arnum"].ToString();
				}
			}
		}

		private void tb_arnum_DoubleClick(object sender, System.EventArgs e)
		{
			SendKeys.Send("{F2}");
		}

		private void tb_arnum2_KeyDown(object sender, KeyEventArgs e)
		{
			F2BaseHelper f2BaseHelper;

			if (e.KeyCode == Keys.F2)
			{
				//if(salesman.Checked)
				//f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", sender, "arnum1", null, "active = '1' and issalesman = '1' and isdriver = '1'", F2Type.Sort);
				//else
				//	f2BaseHelper = new F2BaseHelper("F2GridInfo_HEMPH.xml", sender, "arnum1", null, "active = '1' and isdriver = '1'", F2Type.Sort);

				//Luixia need to see all -ivan 1/3/08
				//f2BaseHelper = new F2BaseHelper("F2GridInfo_ARM.xml", sender, "arnum1", "arnum like '" + (sender as TextBox).Text.Trim() + "%'", "active = '1'", F2Type.Sort);
				f2BaseHelper = new F2BaseHelper("F2GridInfo_ARM.xml", sender, "arnum1", "arnum like '" + (sender as TextBox).Text.Trim() + "%'", "1=1", F2Type.Sort);

				f2BaseHelper.F2_Load();

				if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
				{
					//this.arnum_from = f2BaseHelper.F2Base.CurrentRow["empnum"].ToString();
					(sender as TextBox).Text = f2BaseHelper.F2Base.CurrentRow["arnum"].ToString();
				}
			}
		}

		void tb_arnum2_DoubleClick(object sender, System.EventArgs e)
		{
			SendKeys.Send("{F2}");
		}		

		private void tb_arnum2_Enter(object sender, EventArgs e)
		{
			tb_arnum2.Text = tb_arnum.Text;
		}

    }
}