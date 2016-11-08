/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_LVEM.cs
 *	Description:    Leave Type Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Joshua           20070207            Change to new core
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.LVM
{
    public class Voucher_LVM : BizRAD.BizApplication.VoucherBaseHelper
    {

        #region Class Variables
        protected DBAccess dbaccess = null;
        protected CheckBox lvm_isCalculable, lvm_isCapped, lvm_isCombinable, lvm_AllowBroughtForward, lvm_isincremental = null;
        protected TextBox lvm_noOfDays, lvm_isCappedValue, lvm_isCombinableWith, lvm_BroughtForwardTo, lvm_yearlyincrementalvalue = null;
        protected string headerFormName = "";
        #endregion

        public Voucher_LVM(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_LVM.xml", moduleName, voucherBaseHelpers)
        {
        }

        #region Form Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            this.dbaccess = e.DBAccess;
            DataRow lvm = this.dbaccess.DataSet.Tables["lvm"].Rows[0];

            this.headerFormName = (e.FormsCollection["header"] as Form).Name;

            Initialise();

        }

        #endregion

        private void Initialise()
        {
            DataRow lvm = this.dbaccess.DataSet.Tables["lvm"].Rows[0];

            lvm_isCalculable = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_isCalculable") as CheckBox;
            lvm_isCapped = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_isCapped") as CheckBox;
            lvm_isCombinable = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_isCombinable") as CheckBox;
            lvm_AllowBroughtForward = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_AllowBroughtForward") as CheckBox;


            lvm_isincremental = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_isincremental") as CheckBox;


            lvm_isCappedValue = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_isCappedValue") as TextBox;
            lvm_isCombinableWith = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_isCombinableWith") as TextBox;
            lvm_BroughtForwardTo = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_BroughtForwardTo") as TextBox;


            lvm_yearlyincrementalvalue = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_yearlyincrementalvalue") as TextBox;

            lvm_isCalculable.CheckedChanged += new EventHandler(lvm_isCalculable_CheckedChanged);
            lvm_isCapped.CheckedChanged+=new EventHandler(lvm_isCapped_CheckedChanged);
            lvm_isCombinable.CheckedChanged +=new EventHandler(lvm_isCombinable_CheckedChanged);
            lvm_AllowBroughtForward.CheckedChanged+=new EventHandler(lvm_AllowBroughtForward_CheckedChanged);
            lvm_isincremental.CheckedChanged +=new EventHandler(lvm_isincremental_CheckedChanged);


            //lvm_noOfDays = BizXmlReader.CurrentInstance.GetControl(headerFormName, "lvm_noOfDays") as TextBox;

            if (BizFunctions.IsEmpty(lvm["AllowBroughtForward"]))
            {
                lvm["AllowBroughtForward"] = 0;
            }

            if ((bool)lvm["AllowBroughtForward"])
            {
                lvm_BroughtForwardTo.Enabled = true;
            }
            else
            {
                lvm_BroughtForwardTo.Enabled = false;
            }

            if (BizFunctions.IsEmpty(lvm["isCalculable"]))
            {
                lvm["isCalculable"] = 0;
            }

            if ((bool)lvm["isCalculable"])
            {
                lvm_isCapped.Enabled = true;
            }
            else
            {
                lvm_isCapped.Enabled = false;
            }

            if (BizFunctions.IsEmpty(lvm["isCapped"]))
            {
                lvm["isCapped"] = 0;
            }

            if ((bool)lvm["isCapped"])
            {
                lvm_isCappedValue.Enabled = true;
            }
            else
            {
                lvm_isCappedValue.Enabled = false;
            }



            if (BizFunctions.IsEmpty(lvm["isCombinable"]))
            {
                lvm["isCombinable"] = 0;
            }

            if ((bool)lvm["isCombinable"])
            {
                lvm_isCombinableWith.Enabled = true;
            }
            else
            {
                lvm_isCombinableWith.Enabled = false;
            }


            if (BizFunctions.IsEmpty(lvm["isincremental"]))
            {
                lvm["isincremental"] = 0;
            }

            if ((bool)lvm["isincremental"])
            {
                lvm_yearlyincrementalvalue.Enabled = true;
            }
            else
            {
                lvm_yearlyincrementalvalue.Enabled = false;
            }

        

        }


        private void lvm_isCalculable_CheckedChanged(object sender, EventArgs e)
        {
            if (lvm_isCalculable.Checked)
            {
                lvm_isCapped.Enabled = true;
            }
            else
            {
                lvm_isCapped.Enabled = false;
            }
        }

        private void lvm_isincremental_CheckedChanged(object sender, EventArgs e)
        {
            if (lvm_isincremental.Checked)
            {
                lvm_yearlyincrementalvalue.Enabled = true;
            }
            else
            {
                lvm_yearlyincrementalvalue.Enabled = false;
            }
        }

        private void lvm_isCapped_CheckedChanged(object sender, EventArgs e)
        {
            if (lvm_isCapped.Checked)
            {
                lvm_isCappedValue.Enabled = true;
            }
            else
            {
                lvm_isCappedValue.Enabled = false;
            }
        }

        private void lvm_isCombinable_CheckedChanged(object sender, EventArgs e)
        {
            if (lvm_isCombinable.Checked)
            {
                lvm_yearlyincrementalvalue.Enabled = true;
            }
            else
            {
                lvm_yearlyincrementalvalue.Enabled = false;
            }
        }
        private void lvm_AllowBroughtForward_CheckedChanged(object sender, EventArgs e)
        {
            if (lvm_AllowBroughtForward.Checked)
            {
                lvm_BroughtForwardTo.Enabled = true;
            }
            else
            {
                lvm_BroughtForwardTo.Enabled = false;
            }
        }
      

        #region Document Handle

        protected override void Voucher_Form_OnLoad(object sender, BizRAD.BizVoucher.VoucherEventArgs e)
        {
            base.Voucher_Form_OnLoad(sender, e);

            if (e.DBAccess.DataSet.Tables["lvm"].Rows.Count > 0)
            {
                DataRow lvma = e.DBAccess.DataSet.Tables["lvm"].Rows[0];
            }

            BizLogicTools.Tools.setDefaults(e.DBAccess.DataSet, "lvm");

        }

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow lvm = e.DBAccess.DataSet.Tables["lvm"].Rows[0];

            if (!lvm_isCapped.Checked)
            {
                lvm["isCappedValue"] = System.DBNull.Value;
            }

            if (!lvm_AllowBroughtForward.Checked)
            {
                lvm["BroughtForwardTo"] = System.DBNull.Value;
            }

            if (Convert.IsDBNull(lvm["trandate"])) lvm["trandate"] = System.DateTime.Now.ToShortDateString();

            if (BizFunctions.IsEmpty(lvm["lvmname"]))
            {
                MessageBox.Show("Empty Field 'Leave Description' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }

            if (BizFunctions.IsEmpty(lvm["offorleave"]))
            {
                MessageBox.Show("Empty Field 'Off or Leave' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }


          
        }

        #endregion

        #region Document Event

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);
            DataRow lvm = e.DBAccess.DataSet.Tables["lvm"].Rows[0];

         
            //if (Convert.IsDBNull(lvm["fladjhrs"])) lvm["fladjhrs"] = 0;
            //if (Convert.IsDBNull(lvm["hqadjhrs"])) lvm["hqadjhrs"] = 0;
        }

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);
            DataRow lvm = e.DBAccess.DataSet.Tables["lvm"].Rows[0];

            //if (Convert.IsDBNull(lvm["offorleave"]))
            //{
            //    lvm["offorleave"] = 0;
            //}

            if (lvm_isCombinable.Checked == false)
            {
                lvm["iscombinableWith"] = System.DBNull.Value;
            }
        }

        #endregion
    }
}
