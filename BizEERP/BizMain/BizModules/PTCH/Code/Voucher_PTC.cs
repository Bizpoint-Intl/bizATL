/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Voucher_PTCH.cs
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

namespace ATL.PTCH
{
	public class Voucher_PTCH : BizRAD.BizApplication.VoucherBaseHelper
	{
		#region Variables
		protected DBAccess dbaccess;
		private DataRow ptch;

		private CheckBox chkAsAbove;
		private Panel pnSAddr;
        protected DataGrid dg_branches = null;
        protected string headerFormName,branchesFormName = null;
        protected Button btn_Voucher_Print = null;


		#endregion

		#region Constructor

		public Voucher_PTCH(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_PTCH.xml", moduleName, voucherBaseHelpers)
		{
		}

		#endregion

		#region Voucher Default/ALL

		protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
		{
			base.AddVoucherAllCondition(e);
			e.Condition = "flag='PTCH'";
		}

		#endregion

        protected override void Voucher_Form_OnLoad(object sender, VoucherEventArgs e)
        {
            base.Voucher_Form_OnLoad(sender, e);

            //Button btn_Voucher_Print = BizXmlReader.CurrentInstance.GetControl(this.voucherBase.VoucherForm.Name, "btn_Voucher_Print") as Button;
            ////btn_Voucher_Reports.Text = "Report";
            //btn_Voucher_Print.Visible = true;
            //btn_Voucher_Print.Enabled = true;
            ////btn_Voucher_Print.Click += new EventHandler(btn_Voucher_Print_Click);
        }


		protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
		{
			base.Document_Form_OnLoad(sender, e);
			dbaccess = e.DBAccess;
			this.ptch = dbaccess.DataSet.Tables["ptch"].Rows[0];

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;

			if (Convert.IsDBNull(this.ptch["active"])) this.ptch["active"] = 1;




            if (BizFunctions.IsEmpty(ptch["oricur"]))
            {
                ptch["oricur"] = "SGD";
            }

            if (BizFunctions.IsEmpty(ptch["gstgrpnum"]))
            {
                ptch["gstgrpnum"] = "SGST";
            }

            if (BizFunctions.IsEmpty(ptch["regnum"]))
            {
                ptch["regnum"] = "SIN";
            }
		}

        private void Initialise()
        {
            //GetSitesInfo();

          

        }


		private void PTCH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			switch (e.Column.ColumnName)
			{
                //case "ShipEqBill":
                //    if ((bool)this.ptch["ShipEqBill"] == true)
                //    {
                //        //Copy Bill Addr Over
                //        this.ptch.BeginEdit();
                //        this.ptch["saddr1"] = this.ptch["addr1"];
                //        this.ptch["saddr2"] = this.ptch["addr2"];
                //        this.ptch["saddr3"] = this.ptch["addr3"];
                //        this.ptch["saddr4"] = this.ptch["addr4"];
                //        this.ptch.EndEdit();
                //    }
                //    break;

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
		//    DataTable ptchpos = dbaccess.DataSet.Tables["ptchpos"];

		//    foreach (DataRow dr in ptchpos.Rows)
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


			if (BizFunctions.IsEmpty(this.ptch["arname"]))
			{
                MessageBox.Show("Empty Field 'Customer Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Handle = false;
				return;
			}

            //if (!BizFunctions.IsEmpty(this.ptch["accnum"]))
            //{
            //    if (!BizValidate.CheckTableIsValid(e.DBAccess, "acm", "accnum", this.ptch["accnum"].ToString()))
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

			if (!BizFunctions.IsEmpty(this.ptch["oricur"]))
			{
				if (!BizValidate.CheckTableIsValid(e.DBAccess, "exr", "oricur", this.ptch["oricur"].ToString()))
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

			if (!BizFunctions.IsEmpty(this.ptch["gstgrpnum"]))
			{
				if (!BizValidate.CheckTableIsValid(e.DBAccess, "gstm", "gstgrpnum", this.ptch["gstgrpnum"].ToString()))
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

			if (!BizFunctions.IsEmpty(this.ptch["regnum"]))
			{
				if (!BizValidate.CheckTableIsValid(e.DBAccess, "regm", "regnum", this.ptch["regnum"].ToString()))
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

            //if (BizFunctions.IsEmpty(ptch["barname"]))
            //{
            //    ptch["barname"] = ptch["arname"];
            //}
            //if (BizFunctions.IsEmpty(ptch["bphone"]))
            //{
            //    ptch["bphone"] = ptch["phone"];
            //}
            //if (BizFunctions.IsEmpty(ptch["bhp"]))
            //{
            //    ptch["bhp"] = ptch["hp"];
            //}
            //if (BizFunctions.IsEmpty(ptch["bfax"]))
            //{
            //    ptch["bfax"] = ptch["fax"];
            //}
            //if (BizFunctions.IsEmpty(ptch["bemail"]))
            //{
            //    ptch["bemail"] = ptch["email"];
            //}
            //if (BizFunctions.IsEmpty(ptch["bptc"]))
            //{
            //    ptch["bptc"] = ptch["ptc"];
            //}
            //if (BizFunctions.IsEmpty(ptch["baddr1"]))
            //{
            //    ptch["baddr1"] = ptch["addr1"];
            //}
            //if (BizFunctions.IsEmpty(ptch["baddr2"]))
            //{
            //    ptch["baddr2"] = ptch["addr2"];
            //}
            //if (BizFunctions.IsEmpty(ptch["baddr3"]))
            //{
            //    ptch["baddr3"] = ptch["addr3"];
            //}
            //if (BizFunctions.IsEmpty(ptch["baddr4"]))
            //{
            //    ptch["baddr4"] = ptch["addr4"];
            //}


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
            DataRow ptch = this.dbaccess.DataSet.Tables["ptch"].Rows[0];

            string checkSiteExist = "Select * from SITM where arnum='"+ptch["arnum"].ToString()+"'";

            this.dbaccess.ReadSQL("CheckSiteTB", checkSiteExist);

            DataTable CheckSiteTB = this.dbaccess.DataSet.Tables["CheckSiteTB"];

            if (CheckSiteTB.Rows.Count > 0)
            {
              
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
                                   //" + ptch[""].ToString() + "
                             "VALUES "+
                                   "("+Convert.ToString(MaxID+1)+" "+
                                   ",'" + ptch["sectorcode"].ToString() + "' " +
                                   ",'" + ptch["arname"].ToString() + "' " +
                                   ",'" + ptch["addr1"].ToString() + "' " +
                                   ",'" + ptch["addr2"].ToString() + "' " +
                                   ",'" + ptch["addr3"].ToString() + "' " +
                                   //",<country, nvarchar(20),> "+
                                   //",<postalcode, nvarchar(20),> "+
                                   //",<officerqty, int,> "+
                                   //",<discamt, decimal(16,2),> "+
                                   ",'" + ptch["baddr1"].ToString() + "' " +
                                   ",'" + ptch["baddr2"].ToString() + "' " +
                                   ",'" + ptch["baddr3"].ToString() + "' " +
                                   ",'" + ptch["ptc"].ToString() + "' " +
                                   ",'" + ptch["email"].ToString() + "' " +
                                   ",'" + ptch["phone"].ToString() + "' " +
                                   ",'" + ptch["fax"].ToString() + "' " +
                                   ",GETDATE() "+
                                   //"," + ptch["trandate"].ToString() + "' " +
                                   ",GETDATE() "+
                                   //",<year, int,> "+
                                   ",'O' "+
                                   //",<period, int,> "+
                                   ",'SITM' "+
                                   ",'" + ptch["user"].ToString() + "' " +
                                   ",LOWER(REPLACE(NEWID(),'-','')) "+
                                   //"," + ptch["remark"].ToString() + " "+
                                   //",<rep2, nvarchar(200),> "+
                                   //",<tel2, nvarchar(30),> "+
                                   //",<rep1tel, nvarchar(100),> "+
                                   //",<rep2tel, nvarchar(100),> "+
                                   ",'" + ptch["createdby"].ToString() + "' " +
                                   ",'" + ptch["arnum"].ToString() + "' " +
                                   //",<terminalid, nvarchar(3),> "+
                                   //",<prmcode, nvarchar(20),> "+
                                   //",<qctnum, nvarchar(20),> "+
                                   //",<sitenumt, nvarchar(20),> "+
                                   ",'" + ptch["arnum"].ToString() + "1' " +
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
                //case "arbranch":
                //    e.CurrentRow["ardesc"] = e.F2CurrentRow["sitename"];

                //    break;
            }
        }

        protected override void AddDocumentF3Condition(object sender, DocumentF3EventArgs e)
        {
            base.AddDocumentF3Condition(sender, e);
            DataRow ptch = dbaccess.DataSet.Tables["ptch"].Rows[0];
            switch (e.ControlName)
            {

                case "ptch_arnum":
                    {
                        //qcth["arname"] = e.F2CurrentRow["arname"];
                        //qcth["phone"] = e.F2CurrentRow["phone"];
                        //qcth["hp"] = e.F2CurrentRow["hp"];
                        //qcth["fax"] = e.F2CurrentRow["fax"];
                        //qcth["email"] = e.F2CurrentRow["email"];
                        //qcth["ptc"] = e.F2CurrentRow["ptc"];

                        ptch["arnum"] = e.F2CurrentRow["arnum"];
                        ptch["arname"] = e.F2CurrentRow["arname"];
                        ptch["phone"] = e.F2CurrentRow["phone"];
                        ptch["hp"] = e.F2CurrentRow["hp"];
                        ptch["fax"] = e.F2CurrentRow["fax"];
                        ptch["email"] = e.F2CurrentRow["email"];
                        ptch["ptc"] = e.F2CurrentRow["ptc"];

                        ptch["saddr1"] = e.F2CurrentRow["baddr1"];
                        ptch["saddr2"] = e.F2CurrentRow["baddr2"];
                        ptch["saddr3"] = e.F2CurrentRow["baddr3"];
                        ptch["saddr4"] = e.F2CurrentRow["baddr4"];
                    }
                    break;



            }
        }

        private void GetSitesInfo()
        {
            DataRow ptch = this.dbaccess.DataSet.Tables["PTCH"].Rows[0];

            string GetSitmI = "Select sitenum as [Site Info],sitename as [Site Name] from SITM where arnum='" + ptch["arnum"].ToString() + "'";

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

