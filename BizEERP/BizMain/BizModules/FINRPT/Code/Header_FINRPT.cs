/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Header_POR.cs
 *	Description:    Header Page (PORH)
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Wengkin          2006-10-03          New Module creation.
 * 
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;

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
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;

namespace ATL.FINRPT
{
	public class Header_FINRPT
	{
		protected DBAccess	dbAccess		= null;
		protected DataSet	dataSet			= null;
		protected Hashtable	formsCollection	= null;
		protected string    documentKey		= null;
        protected Form      form            = null;

        public Header_FINRPT(DBAccess dbAccess, Hashtable formsCollection, string DocumentKey)
		{
			this.dbAccess			= dbAccess;
			this.dataSet			= this.dbAccess.DataSet;
			this.formsCollection	= formsCollection;
			this.documentKey		= DocumentKey;
        }


        
        /** 
         * Procedure : btn_Openpath_Click
         * ---------------------------------------------------------
         * Author			Time				Description
         * Wengkin          2006-10-04          Created.
         **/
        //#region btn_Openpath_Click
        //protected void btn_Openpath_Click(object sender, System.EventArgs e)
        //{
        //    this.form = (Form)(sender as Button).Parent;
        //    TextBox projectPath = BizXmlReader.CurrentInstance.GetControl(this.form.Name, "porh_projectpath") as TextBox;
        //    FolderBrowserDialog fbd = new FolderBrowserDialog();

        //    if (projectPath.Text == String.Empty)
        //    {
        //        // Initialise Folder Browser Dialog.
        //        fbd.RootFolder = Environment.SpecialFolder.MyComputer;
        //        fbd.ShowNewFolderButton = false;
        //        fbd.Description = "1. Browse for Project Folder.\n2. Select Folder.\n3. Click OK.";

        //        if (fbd.ShowDialog() == DialogResult.OK)
        //        {
        //            projectPath.Text = fbd.SelectedPath;
        //        }
        //    }
        //    else
        //    {
        //        if (!System.IO.Directory.Exists(projectPath.Text))
        //        {
        //            // Warning : Invalid Project Path.
        //            MessageBox.Show("Project Folder does not exist !", "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //        else
        //        {
        //            // Execute the windows explorer.
        //            System.Diagnostics.Process.Start("explorer.exe", projectPath.Text);
        //        }
        //    }
        //}
        //#endregion

        

        //#region btn_attachment1_Click
        //protected void btn_attachment1_Click(object sender, System.EventArgs e)
        //{
        //    this.form = (Form)(sender as Button).Parent;
        //    TextBox attachment1 = BizXmlReader.CurrentInstance.GetControl(this.form.Name, "porh_attachment1") as TextBox;
        //    FolderBrowserDialog fbd = new FolderBrowserDialog();
        //    DataRow porh = this.dbAccess.DataSet.Tables["porh"].Rows[0];
        //    if (attachment1.Text == String.Empty)
        //    {
        //        // Initialise Folder Browser Dialog.
        //        fbd.RootFolder = Environment.SpecialFolder.MyComputer;
        //        fbd.ShowNewFolderButton = false;
        //        fbd.Description = "1. Browse for Project Folder.\n2. Select Folder.\n3. Click OK.";

        //        if (fbd.ShowDialog() == DialogResult.OK)
        //        {
        //            attachment1.Text = fbd.SelectedPath;
        //            porh["attachment1"] = attachment1.Text.ToString().Trim();
        //        }
        //    }
        //    else
        //    {
        //        if (!System.IO.Directory.Exists(attachment1.Text))
        //        {
        //            // Warning : Invalid Project Path.
        //            MessageBox.Show("Project Folder does not exist !", "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //        else
        //        {
        //            // Execute the windows explorer.
        //            System.Diagnostics.Process.Start("explorer.exe", attachment1.Text);
        //        }
        //    }
        //}
        //#endregion
    }
}
