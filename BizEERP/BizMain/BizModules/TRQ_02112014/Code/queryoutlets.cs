using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;

namespace ATL.TRQ
{
	public partial class queryoutlets : Form
	{
		protected DBAccess dbaccess = null;
		SqlConnection myConnection = null;
		BackgroundWorker bg = null;

		// SQL To retrieve the outlet ip addressses
		string outletsql = "select sitenum,substring(replace(standardsqlstring,'TCP://','') ,0,charindex(':',replace(standardsqlstring,'TCP://',''),0)) address from sitm where standardsqlstring is not null";

		public queryoutlets(DBAccess dbaccess)
		{
			InitializeComponent();
			this.dbaccess = dbaccess;
			this.ShowDialog();
		}

		private void queryoutlets_Load(object sender, EventArgs e)
		{
			
			loadGrid();

			#region Acquire database types and populate combobox

			cmb_database.Items.Add("Outlet");
			cmb_database.Items.Add("SINGAPOREHQ");
			cmb_database.SelectedIndex = 0;

			#endregion


			// Extract only the ip from the typical TCP://192.168.0.1:60233 string in sitm
			populateOutletComboBox(cmb_outlets, outletsql, cmb_database.Items[cmb_database.SelectedIndex].ToString().Trim());

		}

		private void populateOutletComboBox(ComboBox cmb_outlets, String sql, String databaseLocation)
		{
			#region Acquire the outlets and populate combobox
			
			cmb_outlets.Items.Clear();

			DataSet tmpds = dbaccess.ReadSQLTemp("sites", sql);

			DataSet hqds = dbaccess.ReadSQLTemp("SINGAPOREHQ", "select * from SysSqlServers where description ='SINGAPOREHQ'");

			foreach (DataRow dr in tmpds.Tables["sites"].Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					if (databaseLocation == "SINGAPOREHQ")
					{
						cmb_outlets.Items.Add(new comboBoxObject(dr["sitenum"].ToString().Trim(), hqds.Tables["SINGAPOREHQ"].Rows[0]["servername"].ToString().Trim()));

					}
					else
						cmb_outlets.Items.Add(new comboBoxObject(dr["sitenum"].ToString().Trim(), dr["address"].ToString().Trim() + "\\SQLEXPRESS"));
				}
			}

			// Add an option to see all the sites.
			if (databaseLocation == "SINGAPOREHQ")
				cmb_outlets.Items.Add(new comboBoxObject("ALL", hqds.Tables["SINGAPOREHQ"].Rows[0]["servername"].ToString().Trim()));
							

			#endregion
		}

		private void loadGrid()
		{
			if (cmb_outlets.SelectedItem != null)
			{
				bg = new BackgroundWorker();
				bg.WorkerSupportsCancellation = true;
				progbar.Value = progbar.Minimum;
				progbar.Visible = true;
				bg.DoWork += new DoWorkEventHandler(computeRows);
				bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
				bg.RunWorkerAsync(cmb_outlets.SelectedItem);
			}
		}

		private void computeRows(object sender, DoWorkEventArgs e)
		{
			try
			{
				SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

				if (myConnection != null)
					myConnection.Close();

				if ((e.Argument as comboBoxObject).Value == "localhost")
				{
					builder.ConnectTimeout = 3;
					builder.AsynchronousProcessing = true;
					builder.PacketSize = 32767;
					builder["Data Source"] = (e.Argument as comboBoxObject).Value;
					builder["integrated Security"] = true;
					builder["Initial Catalog"] = "HF2";
				}
				else
				{
					builder.ConnectTimeout = 3;
					builder.AsynchronousProcessing = true;
					builder.PacketSize = 32767;
					builder["Data Source"] = (e.Argument as comboBoxObject).Value;
					builder["integrated Security"] = false;
					builder["Initial Catalog"] = "HF2";
					builder.UserID = "sa";
					builder.Password = "scl8818g";
				}

				myConnection = new SqlConnection(builder.ConnectionString);

				myConnection.Open();

				string sql;

				if ((e.Argument as comboBoxObject).Display != "ALL")
				{
					sql = "select a.matnum,b.matname,a.sitenum,a.qty " +
							 "from " +
							 "((Select sitenum,matnum, sum(qty) qty from mwt2008 group by sitenum,matnum ) a" +
							 " left join " +
							 "( select matname , matnum from matm ) b " +
							 " on a.matnum = b.matnum ) " +
							 " where sitenum ='" + (e.Argument as comboBoxObject).Display + "'";
				}
				else
				{
					sql = "select a.matnum,b.matname,a.sitenum,a.qty " +
							"from " +
							"((Select sitenum,matnum, sum(qty) qty from mwt2008 group by sitenum,matnum ) a" +
							" left join " +
							"( select matname , matnum from matm ) b " +
							" on a.matnum = b.matnum ) ";
				}

				SqlDataAdapter ad = new SqlDataAdapter(sql, myConnection);	

				DataSet ds = new DataSet();

				ad.Fill(ds, "mwt");

				// Return the result
				e.Result = ds.Tables["mwt"];

			}
			catch
			{
				MessageBox.Show("Connection error. Server connection may be down.", "Connection error");
				return;
			}
		}
		

		void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			DataTable result = (DataTable)e.Result;

			fillDataGridView(result);

			progbar.Visible = false;
		}

		void fillDataGridView(DataTable datatable)
		{
			#region Set Colors and Styles
			dgv_outlet.RowsDefaultCellStyle.BackColor = Color.Bisque;
			dgv_outlet.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;
			dgv_outlet.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
			dgv_outlet.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
			dgv_outlet.DefaultCellStyle.SelectionForeColor = Color.Black;
			#endregion

			dgv_outlet.DataSource = datatable;

			SizeColumns(dgv_outlet);

			txt_total.Text = (dgv_outlet.DataSource as DataTable).Compute("sum(qty)","").ToString().Trim();

		}

		protected void SizeColumns(DataGridView grid)
		{
			Graphics g = CreateGraphics();

			DataTable dataTable = (DataTable)grid.DataSource;

			foreach (DataGridViewColumn dataColumn in grid.Columns)
			{
				int maxSize = 0;

				SizeF size = g.MeasureString(
								dataColumn.HeaderText,
								grid.Font
							 );

				if (size.Width > maxSize)
					maxSize = (int)size.Width;

				foreach (DataRow row in dataTable.Rows)
				{
					size = g.MeasureString(
							  row[dataColumn.Name].ToString().Trim(),
							  grid.Font
						);

					if (size.Width > maxSize)
						maxSize = (int)size.Width;
				}

				//dataColumn.Name = grid[dataColumn.Name].ToString().Trim();
				//dataColumn.HeaderText = grid[dataColumn.Name].ToString().Trim();
				dataColumn.Width = maxSize + 5;
			}

			g.Dispose();
		}

		private void cmb_outlets_SelectionChangeCommitted(object sender, EventArgs e)
		{
				loadGrid();
		}


		protected class comboBoxObject
		{
			private string m_Display;
			private string m_Value;
			public comboBoxObject(string Display, string Value)
			{
				m_Display = Display;
				m_Value = Value;
			}
			public string Display
			{
				get { return m_Display; }
			}
			public string Value
			{
				get { return m_Value; }
			}
		}

		private void queryoutlets_FormClosed(object sender, FormClosedEventArgs e)
		{

			if (myConnection != null)
			{
				if (myConnection.State != ConnectionState.Closed)
				{
					myConnection.Close();
					txt_status.Text = myConnection.State.ToString();
				}
				myConnection.Dispose();
				myConnection = null;
			}

			if (bg != null)
			{
				bg.CancelAsync();
				bg.Dispose();
			}

		}

		private void cmb_database_SelectionChangeCommitted(object sender, EventArgs e)
		{
			populateOutletComboBox(cmb_outlets, outletsql, cmb_database.Items[cmb_database.SelectedIndex].ToString().Trim());
		}


	}
}