using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Reflection;
using System.Deployment.Application;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizTools;
using BizRAD.BizAccounts;

namespace ATL.DefaultUIFile.DEFAULT_ABOUTBIZERP
{
	public partial class DEFAULT_ABOUTBIZERP : Form
	{
		public string ExeVersion = "";
		public string PubVersion = "";
		
		public DEFAULT_ABOUTBIZERP()
		{
			InitializeComponent();
		}

		private void DEFAULT_ABOUTBIZERP_Load(object sender, EventArgs e)
		{
			ExeVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			if (ApplicationDeployment.IsNetworkDeployed) PubVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

			if (ExeVersion != "") lbxProduct.Items.Add("Build Ver " + ExeVersion);
			if (PubVersion != "") lbxProduct.Items.Add("Publish Ver " + PubVersion);


			string ImagePath = System.Configuration.ConfigurationManager.AppSettings.Get("ImagePath");
			this.pbLogo.Image = Image.FromFile(ImagePath + @"\Bizpoint1.bmp");

			lblLic.Text = "License to: BizERP";
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			this.Close();
		}
	}
}