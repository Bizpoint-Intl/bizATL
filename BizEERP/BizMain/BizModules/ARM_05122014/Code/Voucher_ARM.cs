/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_ARM.cs
 *	Description:    Architect Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Chze Keong		20070122			Start 
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.BizVoucher;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.ARM
{
	public class Voucher_ARM : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Variables
		protected DBAccess dbaccess;
		private DataRow arm;

		private CheckBox chkAsAbove;
		private Panel pnSAddr;
        protected DataGrid dg_branches = null;
        protected string headerFormName,branchesFormName = null;
        protected Button btn_Voucher_Print = null;


		#endregion

		#region Constructor

		public Voucher_ARM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_ARM.xml", moduleName, voucherBaseHelpers)
		{
		}

		#endregion

		#region Voucher Default/ALL

		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);
			e.Condition = "flag='ARM'";
		}

		#endregion

        protected override void Voucher_Form_OnLoad(object sender, VoucherEventArgs e)
        {
            base.Voucher_Form_OnLoad(sender, e);

            Button btn_Voucher_Print = BizXmlReader.CurrentInstance.GetControl(this.voucherBase.VoucherForm.Name, "btn_Voucher_Print") as Button;
            //btn_Voucher_Reports.Text = "Report";
            btn_Voucher_Print.Visible = true;
            btn_Voucher_Print.Enabled = true;
            //btn_Voucher_Print.Click += new EventHandler(btn_Voucher_Print_Click);
        }


		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);
			dbaccess = e.DBAccess;
			this.arm = dbaccess.DataSet.Tables["arm"].Rows[0];

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;
            this.branchesFormName = (e.FormsCollection["branches"] as Form).Name;

			if (Convert.IsDBNull(this.arm["active"])) this.arm["active"] = 1;

			//For Shipping Address
			this.pnSAddr = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "pnSAddr") as Panel;
            //this.chkAsAbove = BizXmlReader.CurrentInstance.GetControl((sender as Form).Name, "arm_ShipEqBill") as CheckBox;
            //this.chkAsAbove.CheckedChanged += new EventHandler(chkAsAbove_CheckedChanged);
            //this.chkAsAbove_CheckedChanged(null, null);
			this.arm.Table.ColumnChanged += new DataColumnChangeEventHandler(ARM_ColumnChanged);

            dg_branches = BizXmlReader.CurrentInstance.GetControl(branchesFormName, "dg_branches") as DataGrid;

            Initialise();


            if (BizFunctions.IsEmpty(arm["oricur"]))
            {
                arm["oricur"] = "SGD";
            }

            if (BizFunctions.IsEmpty(arm["gstgrpnum"]))
            {
                arm["gstgrpnum"] = "SGST";
            }

            if (BizFunctions.IsEmpty(arm["regnum"]))
            {
                arm["regnum"] = "SIN";
            }
		}

        private void Initialise()
        {
            GetSitesInfo();

          

        }


		private void ARM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch (e.Column.ColumnName)
			{
				case "ShipEqBill":
					if ((bool)this.arm["ShipEqBill"] == true)
					{
						//Copy Bill Addr Over
						this.arm.BeginEdit();
						this.arm["saddr1"] = this.arm["addr1"];
						this.arm["saddr2"] = this.arm["addr2"];
						this.arm["saddr3"] = this.arm["addr3"];
						this.arm["saddr4"] = this.arm["addr4"];
						this.arm.EndEdit();
					}
					break;

				default:
					break;
			}
		}


		private void chkAsAbove_CheckedChanged(object sender, EventArgs e)
		{
			//if checked, copy billing address over, disable panelSAddr
			if (this.chkAsAbove.Checked == true)
			{
				//Disallow User Input
				this.pnSAddr.Enabled = false;
			}
			else
			{
				//Enable Panel for User to Input
				this.pnSAddr.Enabled = true;
			}
		}

		#region Document Handle

		//protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
		//{
		//    base.Document_Refresh_OnClick(sender, e);
		//    DataTable armpos = dbaccess.DataSet.Tables["armpos"];

		//    foreach (DataRow dr in armpos.Rows)
		//    {
		//        if (dr.RowState != DataRowState.Deleted)
		//        {
		//            dr["arnum"] = dr["refnum"];
		//        }
		//    }
		//}

		protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
		{
			base.Document_Save_Handle(sender, e);


			if (BizFunctions.IsEmpty(this.arm["arname"]))
			{
                MessageBox.Show("Empty Field 'Customer Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

            //if (!BizFunctions.IsEmpty(this.arm["accnum"]))
            //{
            //    if (!BizValidate.CheckTableIsValid(e.DBAccess, "acm", "accnum", this.arm["accnum"].ToString()))
            //    {
            //        MessageBox.Show("Invalid 'A/C Code' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        e.Handle = false;
            //        return;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Empty 'A/C Code'!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Handle = false;
            //    return;
            //}

			if (!BizFunctions.IsEmpty(this.arm["oricur"]))
			{
				if (!BizValidate.CheckTableIsValid(e.DBAccess, "exr", "oricur", this.arm["oricur"].ToString()))
				{
					MessageBox.Show("Invalid 'Currency Code' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Handle = false;
					return;
				}
			}
			else
			{
				MessageBox.Show("Empty 'Currency Code' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

			if (!BizFunctions.IsEmpty(this.arm["gstgrpnum"]))
			{
				if (!BizValidate.CheckTableIsValid(e.DBAccess, "gstm", "gstgrpnum", this.arm["gstgrpnum"].ToString()))
				{
					MessageBox.Show("Invalid 'Tax Code' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Handle = false;
					return;
				}
			}
			else
			{
				MessageBox.Show("Empty 'Tax Code'!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

			if (!BizFunctions.IsEmpty(this.arm["regnum"]))
			{
				if (!BizValidate.CheckTableIsValid(e.DBAccess, "regm", "regnum", this.arm["regnum"].ToString()))
				{
					MessageBox.Show("Invalid 'Country/Region Code' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Handle = false;
					return;
				}
			}
			else
			{
				MessageBox.Show("Empty 'Country/Region Code'!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

            if (BizFunctions.IsEmpty(arm["barname"]))
            {
                arm["barname"] = arm["arname"];
            }
            if (BizFunctions.IsEmpty(arm["bphone"]))
            {
                arm["bphone"] = arm["phone"];
            }
            if (BizFunctions.IsEmpty(arm["bhp"]))
            {
                arm["bhp"] = arm["hp"];
            }
            if (BizFunctions.IsEmpty(arm["bfax"]))
            {
                arm["bfax"] = arm["fax"];
            }
            if (BizFunctions.IsEmpty(arm["bemail"]))
            {
                arm["bemail"] = arm["email"];
            }
            if (BizFunctions.IsEmpty(arm["bptc"]))
            {
                arm["bptc"] = arm["ptc"];
            }
            if (BizFunctions.IsEmpty(arm["baddr1"]))
            {
                arm["baddr1"] = arm["addr1"];
            }
            if (BizFunctions.IsEmpty(arm["baddr2"]))
            {
                arm["baddr2"] = arm["addr2"];
            }
            if (BizFunctions.IsEmpty(arm["baddr3"]))
            {
                arm["baddr3"] = arm["addr3"];
            }
            if (BizFunctions.IsEmpty(arm["baddr4"]))
            {
                arm["baddr4"] = arm["addr4"];
            }


		}


		#endregion

        #region Save End

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            //InsertSiteMaster();

        }

        private void InsertSiteMaster()
        {
            DataRow arm = this.dbaccess.DataSet.Tables["arm"].Rows[0];

            string checkSiteExist = "Select * from SITM where arnum='"+arm["arnum"].ToString()+"'";

            this.dbaccess.ReadSQL("CheckSiteTB", checkSiteExist);

            DataTable CheckSiteTB = this.dbaccess.DataSet.Tables["CheckSiteTB"];

            if (CheckSiteTB.Rows.Count > 0)
            {
                //string updateSITM = "UPDATE [SITM] "+
                //                   "SET "+
                //                   //[ID] = <ID, bigint,> "+
                //                      "[sectorcode] = '" + arm["sectorcode"].ToString() + "' " +
                //                      ",[sitename] = '" + arm["arname"].ToString() + "' " +
                //                      //",[coy] = <coy, nvarchar(20),> "+
                //                      //",[coyname] = <coyname, nvarchar(50),> "+
                //                      ",[addr1] = '" + arm["addr1"].ToString() + "' " +
                //                      ",[addr2] = '" + arm["addr2"].ToString() + "' " +
                //                      ",[addr3] = '" + arm["addr3"].ToString() + "' " +
                //                      //",[country] = <country, nvarchar(20),> "+
                //                      //",[postalcode] = <postalcode, nvarchar(20),> "+
                //                      //",[officerqty] = <officerqty, int,> "+
                //                      //",[discamt] = <discamt, decimal(16,2),> "+
                //                      ",[billadd1] = '" + arm["baddr1"].ToString() + "' " +
                //                      ",[billadd2] = '" + arm["baddr2"].ToString() + "' " +
                //                      ",[billadd3] = '" + arm["baddr3"].ToString() + "' " +
                //                      ",[rep1] = '" + arm["ptc"].ToString() + "' " +
                //                      ",[email] = '" + arm["email"].ToString() + "' " +
                //                      ",[tel1] = '" + arm["phone"].ToString() + "' " +
                //                      ",[fax] = '" + arm["fax"].ToString() + "' " +
                //                      //",[created] = <created, datetime,> "+
                //                      //",[trandate] = <trandate, datetime,> "+
                //                      ",[modified] = GETDATE() "+
                //                      //",[year] = <year, int,> "+
                //                      ",[status] = '" + arm["status"].ToString() + "' " +
                //                      //",[period] = <period, int,> "+
                //                      //",[flag] = <flag, nvarchar(5),> "+
                //                      ",[user] = '" + arm["user"].ToString() + "'" +
                //                      //",[guid] = <guid, nvarchar(32),> "+
                //                      //",[remark] = <remark, nvarchar(100),> "+
                //                      //",[rep2] = <rep2, nvarchar(200),> "+
                //                      //",[tel2] = <tel2, nvarchar(30),> "+
                //                      //",[rep1tel] = <rep1tel, nvarchar(100),> "+
                //                      //",[rep2tel] = <rep2tel, nvarchar(100),> "+
                //                      //",[createdby] = <createdby, nvarchar(30),> "+
                //                      //",[arnum] = <arnum, nvarchar(30),> "+
                //                      //",[terminalid] = <terminalid, nvarchar(3),> "+
                //                      //",[prmcode] = <prmcode, nvarchar(20),> "+
                //                      //",[qctnum] = <qctnum, nvarchar(20),> "+
                //                      //",[sitenumt] = <sitenumt, nvarchar(20),> "+
                //                      //",[sitenum] = <sitenum, nvarchar(20),> "+
                //                      //,[empnum] = <empnum, nvarchar(20),> "+
                //                      //,[empname] = <empname, nvarchar(100),> "+
                //                 "WHERE arnum='" + arm["arnum"].ToString() + "' ";

                //this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(updateSITM);
            }
            else
            {
                int MaxID = BizLogicTools.Tools.getMaxID("SITM", this.dbaccess);

                string InsertSITM = "INSERT INTO [SITM] "+
                                   "([ID] "+
                                   ",[sectorcode] "+
                                   ",[sitename] "+                                 
                                   ",[addr1] "+
                                   ",[addr2] "+
                                   ",[addr3] "+
                                   //",[country] "+
                                   //",[postalcode] "+
                                   //",[officerqty] "+
                                   //",[discamt] "+
                                   ",[billadd1] "+
                                   ",[billadd2] "+
                                   ",[billadd3] "+
                                   ",[rep1] "+
                                   ",[email] "+
                                   ",[tel1] "+
                                   ",[fax] "+
                                   ",[created] "+
                                   //",[trandate] "+
                                   ",[modified] "+
                                   //",[year] "+
                                   ",[status] "+
                                   //",[period] "+
                                   ",[flag] "+
                                   ",[user] "+
                                   ",[guid] "+
                                   //",[remark] "+
                                   //",[rep2] "+
                                   //",[tel2] "+
                                   //",[rep1tel] "+
                                   //",[rep2tel] "+
                                   ",[createdby] "+
                                   ",[arnum] "+
                                   //",[terminalid] "+
                                   //",[prmcode] "+
                                   //",[qctnum] "+
                                   //",[sitenumt] "+
                                   ",[sitenum] "+
                                   //",[empnum] "+
                                   //",[empname]
                                    ") "+
                                   //" + arm[""].ToString() + "
                             "VALUES "+
                                   "("+Convert.ToString(MaxID+1)+" "+
                                   ",'" + arm["sectorcode"].ToString() + "' " +
                                   ",'" + arm["arname"].ToString() + "' " +
                                   ",'" + arm["addr1"].ToString() + "' " +
                                   ",'" + arm["addr2"].ToString() + "' " +
                                   ",'" + arm["addr3"].ToString() + "' " +
                                   //",<country, nvarchar(20),> "+
                                   //",<postalcode, nvarchar(20),> "+
                                   //",<officerqty, int,> "+
                                   //",<discamt, decimal(16,2),> "+
                                   ",'" + arm["baddr1"].ToString() + "' " +
                                   ",'" + arm["baddr2"].ToString() + "' " +
                                   ",'" + arm["baddr3"].ToString() + "' " +
                                   ",'" + arm["ptc"].ToString() + "' " +
                                   ",'" + arm["email"].ToString() + "' " +
                                   ",'" + arm["phone"].ToString() + "' " +
                                   ",'" + arm["fax"].ToString() + "' " +
                                   ",GETDATE() "+
                                   //"," + arm["trandate"].ToString() + "' " +
                                   ",GETDATE() "+
                                   //",<year, int,> "+
                                   ",'O' "+
                                   //",<period, int,> "+
                                   ",'SITM' "+
                                   ",'" + arm["user"].ToString() + "' " +
                                   ",LOWER(REPLACE(NEWID(),'-','')) "+
                                   //"," + arm["remark"].ToString() + " "+
                                   //",<rep2, nvarchar(200),> "+
                                   //",<tel2, nvarchar(30),> "+
                                   //",<rep1tel, nvarchar(100),> "+
                                   //",<rep2tel, nvarchar(100),> "+
                                   ",'" + arm["createdby"].ToString() + "' " +
                                   ",'" + arm["arnum"].ToString() + "' " +
                                   //",<terminalid, nvarchar(3),> "+
                                   //",<prmcode, nvarchar(20),> "+
                                   //",<qctnum, nvarchar(20),> "+
                                   //",<sitenumt, nvarchar(20),> "+
                                   ",'" + arm["arnum"].ToString() + "1' " +
                                   //",<empnum, nvarchar(20),> "+
                                   //",<empname, nvarchar(100),>
                                   ") ";

                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(InsertSITM);
                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SYSID SET LASTID=(SELECT MAX(ID) FROM SITM) WHERE TABLENAME='SITM'");
            }

        }

        #endregion 

        protected override void AddDetailF3Condition(object sender, DetailF3EventArgs e)
        {
            base.AddDetailF3Condition(sender, e);

            switch (e.MappingName)
            {
                case "arbranch":
                    e.CurrentRow["ardesc"] = e.F2CurrentRow["sitename"];

                    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow arm = dbaccess.DataSet.Tables["arm"].Rows[0];
            switch (e.ControlName)
            {
               
                case "arm_qctnum":
                    {
                        //qcth["arname"] = e.F2CurrentRow["arname"];
                        //qcth["phone"] = e.F2CurrentRow["phone"];
                        //qcth["hp"] = e.F2CurrentRow["hp"];
                        //qcth["fax"] = e.F2CurrentRow["fax"];
                        //qcth["email"] = e.F2CurrentRow["email"];
                        //qcth["ptc"] = e.F2CurrentRow["ptc"];

                        arm["arname"] = e.F2CurrentRow["arname"];
                        arm["phone"] = e.F2CurrentRow["phone"];
                        arm["hp"] = e.F2CurrentRow["hp"];
                        arm["fax"] = e.F2CurrentRow["fax"];
                        arm["email"] = e.F2CurrentRow["email"];
                        arm["ptc"] = e.F2CurrentRow["ptc"];

                        arm["addr1"] = e.F2CurrentRow["billadd1"];
                        arm["addr2"] = e.F2CurrentRow["billadd2"];
                        arm["addr3"] = e.F2CurrentRow["billadd3"];
                        arm["addr4"] = e.F2CurrentRow["billadd4"];
                    }
                    break;



            }
        }

        private void GetSitesInfo()
        {
            DataRow arm = this.dbaccess.DataSet.Tables["ARM"].Rows[0];

            string GetSitmI = "Select sitenum as [Site Info],sitename as [Site Name] from SITM where arnum='" + arm["arnum"].ToString() + "'";

            this.dbaccess.ReadSQL("SITM", GetSitmI);

            DataTable SITM = this.dbaccess.DataSet.Tables["SITM"];

            if (SITM.Rows.Count > 0)
            {
                dg_branches.DataSource = SITM;
            }            
        }

        #region Print on Click

        protected override void Document_Print_OnClick(object sender, DocumentPrintEventArgs e)
        {
            base.Document_Print_OnClick(sender, e);

        }

        #endregion


        #region Print Handle

        protected override void Document_Print_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Print_Handle(sender, e);

        }

        #endregion


        #region Print Handle

        private void btn_Voucher_Print_Click(object sender, EventArgs e)
        {
        

        }

        #endregion
	}
}

