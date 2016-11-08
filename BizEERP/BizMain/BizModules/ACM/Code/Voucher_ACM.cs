/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_ACM.cs
 *	Description:    Chart of Accounts
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 *
***********************************************************/

using System;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.ACM
{
	public class Voucher_ACM : BizRAD.BizApplication.VoucherBaseHelper
	{
        protected DBAccess dbAccess = null;
        protected Hashtable FormsCollection = null;
        protected string projectPath = null;

        public Voucher_ACM(string moduleName, Hashtable voucherBaseHelpers) : base("VoucherGridInfo_ACM.xml", moduleName, voucherBaseHelpers)
        {
        }

        #region Document Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow acm = e.DBAccess.DataSet.Tables["acm"].Rows[0];
            
            if (BizFunctions.IsEmpty(acm["accname"]))
            {
                MessageBox.Show("Empty Field 'A/C Name' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }
            if (BizFunctions.IsEmpty(acm["acctype"]))
            {
                MessageBox.Show("Empty Field 'A/C Type' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }

            if ((bool)acm["includecashflow"] && BizFunctions.IsEmpty(acm["cashflowtype"]))
            {
                MessageBox.Show("Empty Field Cash Flow Category !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }

            if (BizValidate.CheckColumnError(e.DBAccess.DataSet, "acm"))
            {
                MessageBox.Show("Errors detected in Data Entry !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }
        }
        
        #endregion

        #region Document Event

        #region Document_LoadData

        protected override void Document_LoadData(object sender, DocumentEventArgs e)
        {
            base.Document_LoadData(sender, e);

			//if (BizXmlReader.CurrentInstance.DefCollection.Contains("cbbanknum") == true)
			//{
                DataSet dataSet1 = e.DBAccess.ReadSQLTemp("cbbanknum", "SELECT banknum, bankname FROM bnkm");
                DataView dataView1 = new DataView(dataSet1.Tables["cbbanknum"]);
                BizXmlReader.CurrentInstance.AddReference(dataView1.Table.TableName, dataView1);
			//}
        }

        #endregion

        #region Form_Load

        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            DataRow acm = e.DBAccess.DataSet.Tables["acm"].Rows[0];

            this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
            this.dbAccess = e.DBAccess;
            this.FormsCollection = e.FormsCollection;

            if (BizFunctions.IsEmpty(acm["acctype"]))
            {
                Form form = BizXmlReader.CurrentInstance.Load(this.projectPath + @"\ACM\UIFile\NewVoucher.xml", "newvoucher", this, null) as Form;
                form.ShowDialog();
            }

            if (Convert.IsDBNull(acm["active"])) acm["active"] = 1;
            if (Convert.IsDBNull(acm["includecashflow"])) acm["includecashflow"] = 0;
            if (Convert.IsDBNull(acm["depth"])) acm["depth"] = 0;

            e.DBAccess.DataSet.Tables["acm"].ColumnChanged += new DataColumnChangeEventHandler(Voucher_ACM_ColumnChanged);
        }

        #region Form Load Pop Up (Selection of Sales Type Window)

        #region CheckBoxChanged

        protected void CheckBoxChanged(object sender, System.EventArgs e)
        {
            GroupBox grp_selacctype = (GroupBox)BizXmlReader.CurrentInstance.GetControl("newvoucher", "grp_selacctype");

            CheckBox ckb = sender as CheckBox;

            if (ckb.Checked == true)
            {
                foreach (Control control in grp_selacctype.Controls)
                {
                    if (control is System.Windows.Forms.CheckBox)
                    {
                        if (control.Name != ckb.Name)
                        {
                            (control as System.Windows.Forms.CheckBox).Checked = false;
                            (control as System.Windows.Forms.CheckBox).Refresh();
                        }
                    }
                }
            }
        }

        #endregion

        #region NewVoucher_Closing

        protected void NewVoucher_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataRow acm = this.dbAccess.DataSet.Tables["acm"].Rows[0];
            DataSet dsTmp = this.dbAccess.ReadSQLTemp("acctype", "SELECT * FROM acctype");

            GroupBox grp_selacctype = (GroupBox)BizXmlReader.CurrentInstance.GetControl("newvoucher", "grp_selacctype");

            // Refer to acctype table
            foreach (Control control in grp_selacctype.Controls)
            {
                if (control is System.Windows.Forms.CheckBox)
                {
                    if ((control as CheckBox).Checked)
                    {
                        DataRow [] selectedAccType = dsTmp.Tables["acctype"].Select("acctypename = '" + (control as CheckBox).Text + "'");
                        if (selectedAccType.Length > 0)
                            acm["acctype"] = selectedAccType[0]["acctype"];
                        else
                            acm["acctype"] = 0;
                    }
                }
            }

            BizXmlReader.CurrentInstance.RemoveForm((sender as Form).Name);
        }

        #endregion

        #region btn_NewSave_Click

        protected void btn_NewSave_Click(object sender, System.EventArgs e)
        {
            ((Form)((sender as Button).Parent)).Close();
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region DocumentF2

        protected override void AddDocumentF2Condition(object sender, DocumentF2EventArgs e)
        {
            base.AddDocumentF2Condition(sender, e);
            switch (e.ControlName)
            {
                case "acm_subaccnum":
                    e.Condition = BizFunctions.F2Condition("accnum/accname", (sender as TextBox).Text);
                    e.DefaultCondition = "accnum <> '" + e.CurrentRow["accnum"].ToString() + "' AND acctype = " + e.CurrentRow["acctype"].ToString();
                    break;
            }
        }

        #endregion

        #region ColumnChangedEvents

        #region acm

        private void Voucher_ACM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
                case "subaccnum":
                    if (!BizFunctions.IsEmpty(e.Row[e.Column.ColumnName]))
                    {
                        string filter =  "active=1 AND accnum <> '" + e.Row["accnum"].ToString() + "' AND acctype = " + e.Row["acctype"].ToString();
                        if (BizValidate.CheckTableIsValid(this.dbAccess, "acm", "accnum", e.Row[e.Column.ColumnName].ToString(), filter))
                        {
                            DataSet dsTmp = this.dbAccess.ReadSQLTemp("acm", "SELECT depth FROM acm WHERE accnum = '" + e.Row[e.Column.ColumnName].ToString() + "'");
                            e.Row["depth"] = (int)dsTmp.Tables["acm"].Rows[0]["depth"] + 1;
                            e.Row.SetColumnError(e.Column.ColumnName, "");
                        }
                        else
                            e.Row.SetColumnError(e.Column.ColumnName, "Invalid Account Code");
                    }
                    break;
            }
        }

        #endregion

        #endregion   

        #region Checkbox Change Event
        void Voucher_ACM_CheckedChanged(object sender, EventArgs e)
        {
            ComboBox cashflowtype = BizXmlReader.CurrentInstance.GetControl((this.FormsCollection["header"] as Form).Name, "acm_cashflowtype") as ComboBox;
            
            if ((sender as CheckBox).Checked)
            {
                cashflowtype.Enabled = true;
                cashflowtype.SelectedIndex = 0;
            }
            else
            {
                cashflowtype.Enabled = false;
                cashflowtype.Text = String.Empty;
            }
        }
        #endregion   
    }
}
