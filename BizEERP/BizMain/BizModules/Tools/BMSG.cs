using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.DB.Client;
using BizRAD.DB.Interface;


namespace ATL.BMSG
{
	public partial class BMSG : Form
	{
		DBAccess dbaccess;
		DataTable dtMSGH = null;
		BindingSource bindMSGH = new BindingSource();

		DataTable dtAllSites = null;
		DataView dvAllSites = null;
		DataView dvSelectedSites = null;
		BindingSource bindAllSites = new BindingSource();
		BindingSource bindSelectedSites = new BindingSource();

		int PageSize = 50;		//load 50 messages per page
		int CurrentPage = 1;
		int MaxPage = 0;

		
		public BMSG()
		{
			InitializeComponent();
			this.dgvMsgh.AutoGenerateColumns = false;
			this.dgvMsg1.AutoGenerateColumns = false;
			this.dgvSites.AutoGenerateColumns = false;
			this.dgvSelected.AutoGenerateColumns = false;

			this.dbaccess = new DBAccess();
			this.dgvMsgh.DataSource = this.bindMSGH;
			this.bindMSGH.CurrentChanged += new EventHandler(this.bindMSGH_CurrentChanged);
		}


		private void BMSG_Load(object sender, EventArgs e)
		{
			//Load 1st Page Messages
			this.LoadMSG(1);

			//Load All sites
			this.LoadAllSites();
		}


		private bool LoadMSG(int Page)
		{
			//prepare parameters
			Parameter[] parameters = new Parameter[4];
			parameters[0] = new Parameter("@TableName", "msgh");
			parameters[1] = new Parameter("@SortOrder", "created desc");
			parameters[2] = new Parameter("@Page", Page);
			parameters[3] = new Parameter("@PageSize", this.PageSize);

			DataSet dsMSG = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("sp_FetchPage", ref parameters);
			if (dsMSG == null)
				return false;
			if (dsMSG.Tables.Count < 2)
				return false;

			this.dtMSGH = dsMSG.Tables[0];
			this.bindMSGH.DataSource = this.dtMSGH.DefaultView;

			//Get total rows in this table
			long maxrows = (long)dsMSG.Tables[1].Rows[0][0];
			this.MaxPage = (int)(maxrows / this.PageSize);
			if (maxrows % this.PageSize > 0)
				this.MaxPage++;
			this.CurrentPage = Page;


			return true;
		}


		private void bindMSGH_CurrentChanged(object sender, EventArgs e)
		{
			if (this.bindMSGH.Current == null)
				return;

			DataRowView drvMSGH = this.bindMSGH.Current as DataRowView;
			this.LoadMSG1(drvMSGH["guid"].ToString());
		}


		private bool LoadMSG1(string ID)
		{
			try
			{
				DataSet ds = this.dbaccess.RemoteStandardSQL.GetSQLResult("select sitenum from msg1 where [guid]='" + ID + "'");
				if (ds == null)
					return false;
				if (ds.Tables.Count == 0)
					return false;

				this.dgvMsg1.DataSource = ds.Tables[0].DefaultView;
			}
			catch
			{ return false; }

			return true;
		}


		private bool LoadAllSites()
		{
			try
			{
				DataSet dsSites = this.dbaccess.RemoteStandardSQL.GetSQLResult("select sitenum,sitename, Selected=CAST(0 AS BIT) from sitm where sitenum<>'HQ' and status in ('O','P') order by sitenum");
				if (dsSites == null)
					return false;
				if (dsSites.Tables.Count == 0)
					return false;

				this.dtAllSites = dsSites.Tables[0];

				this.dvAllSites = new DataView(this.dtAllSites, "Selected=0", "", DataViewRowState.CurrentRows);
				this.dvSelectedSites = new DataView(this.dtAllSites, "Selected=1", "", DataViewRowState.CurrentRows);
				this.bindAllSites.DataSource = this.dvAllSites;
				this.bindSelectedSites.DataSource = this.dvSelectedSites;

				this.dgvSites.DataSource = this.bindAllSites;
				this.dgvSelected.DataSource = this.bindSelectedSites;
			}
			catch
			{ return false; }


			return true;
		}



		#region NEW Message Button/Control Events

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			//Validate message
			string MSG = this.txtMSG.Text.Trim();
			if (MSG == "")
			{
				MessageBox.Show("Please Do Not Broadcast An Empty Message!", "EMPTY Message!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			//Get Selected Sites
			string sites = "";
			if (this.chkALL.Checked)
			{
				sites = "*";
			}
			else
			{
				foreach (DataRowView drvSelected in this.dvSelectedSites)
				{
					if (sites == "")
						sites = drvSelected["sitenum"].ToString();
					else
						sites = sites + "|" + drvSelected["sitenum"].ToString();
				}
			}

			if (sites == "")
			{
				MessageBox.Show("Please Select At Least ONE Site To Broadcast Message!", "No Site Selected!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			//Submit message now
			if (SendMSG(this.dbaccess, "HQ", MSG, sites) == true)
			{
				MessageBox.Show("Message Is Submitted Successfully!", "Message Submit Successful!", MessageBoxButtons.OK, MessageBoxIcon.Stop);

				//Reset All
				this.txtMSG.Text = "";
				this.chkALL.Checked = false;
				this.LoadMSG(1);
				this.LoadAllSites();
			}
			else
			{
				MessageBox.Show("There Is A Problem Submitting The Message Currently!\r\n\r\nPlease Inform The Administrator!", "Message Submit Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}


		public static bool SendMSG(DBAccess curAccess, string SourceSite, string Msg, string Sites)
		{
			//prepare parameters
			Parameter[] parameters = new Parameter[3];
			parameters[0] = new Parameter("@Source", SourceSite);
			parameters[1] = new Parameter("@MSG", Msg);
			parameters[2] = new Parameter("@Sites", Sites);

			try
			{
				curAccess.RemoteStandardSQL.ExecuteNonQuery("sp_SendMSG", ref parameters);
			}
			catch { return false; }


			return true;
		}


		private void btnSelect_Click(object sender, EventArgs e)
		{
			if (this.bindAllSites.Current == null)
			{
				MessageBox.Show("Please Select A Site From The Left Box!", "No Site Selected!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			DataRowView drvSite = this.bindAllSites.Current as DataRowView;
			drvSite.BeginEdit();
			drvSite["Selected"] = true;
			drvSite.EndEdit();

			this.bindAllSites.CurrencyManager.Refresh();
			this.bindSelectedSites.CurrencyManager.Refresh();
		}


		private void btnUnSelect_Click(object sender, EventArgs e)
		{
			if (this.bindSelectedSites.Current == null)
			{
				MessageBox.Show("Please Select A Site From The Right Box To UnSelect A Site!", "No Site Selected!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			DataRowView drvSite = this.bindSelectedSites.Current as DataRowView;
			drvSite.BeginEdit();
			drvSite["Selected"] = false;
			drvSite.EndEdit();

			this.bindAllSites.CurrencyManager.Refresh();
			this.bindSelectedSites.CurrencyManager.Refresh();
		}


		private void chkALL_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chkALL.Checked)
				this.pnSelection.Enabled = false;
			else
				this.pnSelection.Enabled = true;
		}


		#endregion NEW Message Button/Control Events

		#region Top Panel Button Events

		private void btnFirst_Click(object sender, EventArgs e)
		{
			this.LoadMSG(1);
		}


		private void btnPrev_Click(object sender, EventArgs e)
		{
			if ((this.CurrentPage - 1) <= 0)
				MessageBox.Show("You Are Already On The First Page!", "First Page", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else
				this.LoadMSG(this.CurrentPage - 1);
		}


		private void btnNext_Click(object sender, EventArgs e)
		{
			if ((this.CurrentPage + 1) > this.MaxPage)
				MessageBox.Show("You Are Already On The Last Page!", "Last Page", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else
				this.LoadMSG(this.CurrentPage + 1);
		}


		private void btnLast_Click(object sender, EventArgs e)
		{
			this.LoadMSG(this.MaxPage);
		}


		private void btnDEL_Click(object sender, EventArgs e)
		{
			if (this.bindMSGH.Current == null)
			{
				MessageBox.Show("Please Select A Message Row On The Left Table In Order To Delete!", "No Row Selected!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}


			//Delete MSG
			DataRowView drvMSGH = this.bindMSGH.Current as DataRowView;
			if (MessageBox.Show("Are You Sure You Want To Delete The Selected Message?", "Delete Message?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				return;

			string guid = drvMSGH["guid"].ToString();
			try
			{
				this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(string.Format("delete msgh where [guid]='{0}';delete msg1 where [guid]='{1}'", guid, guid));
			}
			catch { }


			this.LoadMSG(this.CurrentPage);			
		}


		private void btnClearAll_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("This Will DELETE ALL Messages In The System!!\r\n\r\nAre You Sure You Want To Proceed?", "Clear All Messages?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				return;

			//CLEAR ALL!!
			try
			{
				this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("delete msgh;delete msg1");
			}
			catch { }

			this.LoadMSG(1);
		}

		#endregion Top Panel Button Events


	}
}