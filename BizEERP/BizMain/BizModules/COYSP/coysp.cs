/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		coysp.cs
 *	Description:    Modify Company Details
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Jer              070221              1st version
***********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;

namespace ATL.COYSP
{
    public partial class COYSP : Form
    {
        protected DBAccess dbAccess = null;
        protected SystemDBAccess systemDBAccess = null;
        protected Form frmThis = null;

        protected string rowID = String.Empty;

        public COYSP()
        {
            InitializeComponent();

            this.frmThis = this;
            this.dbAccess = new DBAccess();
            this.systemDBAccess = new SystemDBAccess();

            // lock and prevent other users from using same module
            bool isLocked = systemDBAccess.IsLocked("COYSP", "COMPANY INFORMATION", Common.DEFAULT_SYSTEM_USERNAME);

            if (isLocked == true)
            {
                this.frmThis.Close();
                MessageBox.Show("Company Information Settings is locked/in use !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.frmThis.ShowDialog();
            }
        }

        private void COYSP_Load(object sender, EventArgs e)
        {
            try
            {
                this.dbAccess.ReadSQL("localcoy", "SELECT * FROM coy");

                if (this.dbAccess.DataSet.Tables["localcoy"].Rows.Count != 0)
                {
                    DataRow coy = this.dbAccess.DataSet.Tables["localcoy"].Rows[0];

                    this.rowID = coy["ID"].ToString();
                    this.tb_coyname.Text = coy["coyname"].ToString();
                    this.tb_coyaddr1.Text = coy["coyaddr1"].ToString();
                    this.tb_coyaddr2.Text = coy["coyaddr2"].ToString();
                    this.tb_coyaddr3.Text = coy["coyaddr3"].ToString();
                    this.tb_coyaddr4.Text = coy["coyaddr4"].ToString();
                    this.tb_coytel.Text = coy["coytel"].ToString();
                    this.tb_coyfax.Text = coy["coyfax"].ToString();
                    this.tb_coyemail.Text = coy["coyemail"].ToString();
                    this.tb_coyurl.Text = coy["coyurl"].ToString();
                    this.tb_coyregnum.Text = coy["coyregnum"].ToString();
                    this.tb_coygstnum.Text = coy["coygstnum"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Remove entry from SysLocker
            bool isUnlock = systemDBAccess.UnLock("COYSP", "COMPANY INFORMATION");
            this.frmThis.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.rowID != String.Empty)
                {
                    string updateString = "UPDATE coy SET coyname = '" + this.tb_coyname.Text
                                            + "', coyaddr1 = '" + this.tb_coyaddr1.Text + "', coyaddr2 = '" + this.tb_coyaddr2.Text
                                            + "', coyaddr3 = '" + this.tb_coyaddr3.Text + "', coyaddr4 = '" + this.tb_coyaddr4.Text
                                            + "', coytel = '" + this.tb_coytel.Text + "', coyfax = '" + this.tb_coyfax.Text
                                            + "', coyemail = '" + this.tb_coyemail.Text + "', coyurl = '" + this.tb_coyurl.Text
                                            + "', coyregnum = '" + this.tb_coyregnum.Text + "', coygstnum = '" + this.tb_coygstnum.Text
                                            + "' WHERE [ID] = " + this.rowID;

                    this.dbAccess.RemoteStandardSQL.ExecuteNonQuery(updateString);
                }
                else
                {
                    int maxID = ATL.BizLogicTools.Tools.getMaxID("coy", this.dbAccess);
                    string insertString = "Insert into Coy (ID,coyname,coyaddr1,coyaddr2,coyaddr3,coyaddr4,coytel,coyfax,coyemail,coyurl,coyregnum,coygstnum)" +
                                          "Values ("+maxID+",'"+this.tb_coyname.Text+"','"+this.tb_coyaddr1.Text+"','"+this.tb_coyaddr2.Text+"','"+this.tb_coyaddr3.Text+"', "+
                                          "'"+this.tb_coyaddr4.Text+"','"+this.tb_coytel.Text+"','"+this.tb_coyfax.Text+"','"+this.tb_coyemail.Text+"','"+this.tb_coyurl.Text+"', "+
                                          "'" + this.tb_coyregnum.Text + "','" + this.tb_coygstnum.Text + "')";
                    this.dbAccess.RemoteStandardSQL.ExecuteNonQuery(insertString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}