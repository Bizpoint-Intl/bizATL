using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizCommon;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizAccounts;

public partial class ExtractGrid : Form
{
	#region Variables

	protected DBAccess dbaccess = null;
	protected Form frmThis = null;
	protected System.Collections.ArrayList DateTimeColumns = new System.Collections.ArrayList();
	protected DataTable[] extractedTables;
	protected DataTable dataSource;
	protected string selectedSitenum = null;
	protected extractionInfo extractionInfoObject = null;
	protected DataTable finalextraction = null;
	protected Parameter[] parameters;
	protected bool afterExtract = false;
	protected string[] destinationTables;
	protected Hashtable columnMappings;

	#endregion

	#region Constants
	int SQL_PROC = 0;
	int SQL_DIRECT = 1;
	int SQL_ERROR = -1;
	#endregion

	/// <summary>
	/// Construct an extraction grid based on parameters passed in.
	/// </summary>
	/// <param name="dbaccess">Database Connection object which extraction will use</param>
	/// <param name="ExtractionGridName">Title for the extraction grid UI</param>
	/// <param name="DestinationTable">Destination tables to extract to</param>
	/// <param name="ColumnsToDisplay">Columns to Display and their width</param>
	/// <param name="ColumnsToCopy">Columns to extract. (eg. invnum-tablename1@invnum will extract invnum from source to invnum in tablename1)</param>
	/// <param name="SqlDirectForDisplay">This specifies the sql to retrieve the display</param>
	/// <param name="SqlDirectForData">This specifies the sql to retrieve the data</param>
	/// <param name="ExtractionKeyField">Specify columns that will be used to match the display with the data</param>
	/// <param name="IncludeAlreadyExtracted">Option to display already extracted data or not.</param>
	public ExtractGrid( DBAccess dbaccess,
					    string ExtractionGridName,
						string DestinationTable,
						string ColumnsToDisplay,
						string ColumnsToCopy,
						string SqlDirectForDisplay,
						string SqlDirectForData,
						string ExtractionKeyField,
						bool IncludeAlreadyExtracted)
	{
		InitializeComponent();

		// Construct extraction parameters
		extractionInfoObject = new extractionInfo(ExtractionGridName,
													 DestinationTable,
													 ColumnsToDisplay,
													 ColumnsToCopy,
													 SqlDirectForDisplay,
													 SqlDirectForData,
													 ExtractionKeyField,
													 IncludeAlreadyExtracted);
		this.dbaccess = dbaccess;
		// Validate what has been passed in
		if (validateExtractionParameters())
		{
			this.frmThis = this;
			this.DoubleBuffered = true;
			this.Text = extractionInfoObject.ExtractionGridName;
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
	}

	public void showGrid()
	{
		this.ShowDialog();
	}

	/// <summary>
	/// This class stores all the information that is populated from SysExtractGridInfo table in the database.
	/// The user specifies all the necessary parameters in the SysExtractGridInfo table. This class simply 
	/// provides storage for it for easy access.
	/// </summary>
	protected class extractionInfo
	{

		#region Properties
		private string m_ExtractionId = String.Empty;
		private string m_ExtractionGridName = String.Empty;
		private string m_DestinationTable = String.Empty;
		private string m_ColumnsToDisplay = String.Empty;
		private string m_ColumnsToCopy = String.Empty;
		private string m_SqlDirectForDisplay = String.Empty;
		private string m_SqlDirectForData = String.Empty;
		private string m_SqlProcNameForDisplay = String.Empty;
		private string m_SqlProcParametersForDisplay = String.Empty;
		private string m_SqlProcNameForData = String.Empty;
		private string m_SqlProcParametersForData = String.Empty;
		private string m_ExtractionKeyField = String.Empty;
		private bool m_IncludeAlreadyExtracted = false;
		#endregion

		#region Get methods

		public string ExtractionId
		{
			get { return m_ExtractionId; }
			set { m_ExtractionId = value; }
		}
		public string ExtractionGridName
		{
			get { return m_ExtractionGridName; }
			set { m_ExtractionGridName = value; }
		}
		public string DestinationTable
		{
			get { return m_DestinationTable; }
			set { m_DestinationTable = value; }
		}
		public string ColumnsToDisplay
		{
			get { return m_ColumnsToDisplay; }
			set { m_ColumnsToDisplay = value; }
		}
		public string ColumnsToCopy
		{
			get { return m_ColumnsToCopy; }
			set { m_ColumnsToCopy = value; }
		}
		public string SqlDirectForDisplay
		{
			get { return m_SqlDirectForDisplay; }
			set { m_SqlDirectForDisplay = value; }
		}
		public string SqlDirectForData
		{
			get { return m_SqlDirectForData; }
			set { m_SqlDirectForData = value; }
		}
		public string SqlProcNameForDisplay
		{
			get { return m_SqlProcNameForDisplay; }
			set { m_SqlProcNameForDisplay = value; }
		}
		public string SqlProcParametersForDisplay
		{
			get { return m_SqlProcParametersForDisplay; }
			set { m_SqlProcParametersForDisplay = value; }
		}
		public string SqlProcNameForData
		{
			get { return m_SqlProcNameForData; }
			set { m_SqlProcNameForData = value; }
		}
		public string SqlProcParametersForData
		{
			get { return m_SqlProcParametersForData; }
			set { m_SqlProcParametersForData = value; }
		}
		public string ExtractionKeyField
		{
			get { return m_ExtractionKeyField; }
			set { m_ExtractionKeyField = value; }
		}
		public bool IncludeAlreadyExtracted
		{
			get { return m_IncludeAlreadyExtracted; }
			set { m_IncludeAlreadyExtracted = value; }
		}

		#endregion

		#region Constructor
		/// <summary>
		/// Construct an extractionInfo grid based on parameters passed in.
		/// </summary>
		/// <param name="ExtractionGridName">Title for the extraction grid UI</param>
		/// <param name="DestinationTable">Destination tables to extract to</param>
		/// <param name="ColumnsToDisplay">Columns to Display and their width</param>
		/// <param name="ColumnsToCopy">Columns to extract. (eg. invnum-tablename1@invnum will extract invnum from source to invnum in tablename1)</param>
		/// <param name="SqlDirectForDisplay">This specifies the sql to retrieve the display</param>
		/// <param name="SqlDirectForData">This specifies the sql to retrieve the data</param>
		/// <param name="ExtractionKeyField">Specify columns that will be used to match the display with the data</param>
		/// <param name="IncludeAlreadyExtracted">Option to display already extracted data or not.</param>
		public extractionInfo(string ExtractionGridName,
								string DestinationTable,
								string ColumnsToDisplay,
								string ColumnsToCopy,
								string SqlDirectForDisplay,
								string SqlDirectForData,
								string ExtractionKeyField,
								bool IncludeAlreadyExtracted)
		{
			consumeInfo(ExtractionGridName, DestinationTable, ColumnsToDisplay, ColumnsToCopy, SqlDirectForDisplay, SqlDirectForData, String.Empty, String.Empty, String.Empty, String.Empty, ExtractionKeyField, IncludeAlreadyExtracted);
		}


		/// <summary>
		/// This method constructs the object that stores extraction information based on parameters
		/// </summary>

		public void consumeInfo(string ExtractionGridName,
								string DestinationTable,
								string ColumnsToDisplay,
								string ColumnsToCopy,
								string SqlDirectForDisplay,
								string SqlDirectForData,
								string SqlProcNameForDisplay,
								string SqlProcParametersForDisplay,
								string SqlProcNameForData,
								string SqlProcParametersForData,
								string ExtractionKeyField,
								bool IncludeAlreadyExtracted)
		{
			// Title for the extraction grid UI
			this.ExtractionGridName = ExtractionGridName;

			// Destination tables to extract to
			this.DestinationTable = DestinationTable;

			// Columns to Display and their width
			this.ColumnsToDisplay = ColumnsToDisplay;

			// Columns to Copy ... Data Source to destination table mapping
			this.ColumnsToCopy = ColumnsToCopy;
			// This specifies the sql to retrieve the display and the sql to retrieve the data.
			// The extractionkeyfield will be used to match the display with the data.

			this.SqlDirectForDisplay = SqlDirectForDisplay;

			this.SqlDirectForData = SqlDirectForData;

			this.ExtractionKeyField = ExtractionKeyField;

			// If a stored procedure needs to be used, this will be used. This will be ignored if sqldirect is specified.
			this.SqlProcNameForDisplay = SqlProcNameForDisplay;
			this.SqlProcParametersForDisplay = SqlProcParametersForDisplay;

			this.SqlProcNameForData = SqlProcNameForData;
			this.SqlProcParametersForData = SqlProcParametersForData;

			// For cases where user wants extraction to keep showing what has already been extracted.
			this.IncludeAlreadyExtracted = IncludeAlreadyExtracted;
		}
		#endregion
	}

	public void acquireDataForDisplay()
	{
		try
		{
			#region Acquire Data for display
			if (useSQLProc() == SQL_DIRECT)
			{
				#region 1. Execute Direct Sql Statement for Display

				try
				{
					DataSet ds = this.dbaccess.ReadSQLTemp("finalextraction", extractionInfoObject.SqlDirectForDisplay);

					this.finalextraction = ds.Tables[0].Copy();

					this.finalextraction.TableName = "finalextraction";
				}
				catch (Exception directSqlExp)
				{
					MessageBox.Show(directSqlExp.Message, "Direct Sql for Display execution error!");
					return;
				}

				#endregion
			}
			else
				if (useSQLProc() == SQL_PROC)
				{
					#region  2. Construct the sql procedure parameters. Format passed in is "@param1=aaa,@param2=bbb".

					string delimStr = ",/*";
					char[] delimiter = delimStr.ToCharArray();
					string[] split = extractionInfoObject.SqlProcParametersForDisplay.Split(delimiter);

					if (split.Length != 0)
					{
						int i = 0;

						parameters = new Parameter[split.Length];

						try
						{
							// s looks like @param1=paramstring
							foreach (string s in split)
							{
								string delimStrb = "=";
								char[] delimiterb = delimStrb.ToCharArray();

								// Only split on first = sign.
								string[] splitb = s.Split(delimiterb, 2);

								string param, paramvalue;

								param = splitb[0];
								paramvalue = splitb[1];

								parameters[i] = new Parameter(param, paramvalue);

								i++;
							}
						}
						catch
						{
							MessageBox.Show("Sql Procedure Parameters format invalid! (Example: @sitenum ='HQ')", "SqlProcParametersForDisplay");
							return;
						}
					}

					#endregion

					#region 3. Get the result from the storedprocedure

					try
					{
						DataSet ds = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult(extractionInfoObject.SqlProcNameForDisplay, ref parameters);

						this.finalextraction = ds.Tables[0].Copy();

						this.finalextraction.TableName = "finalextraction";
					}
					catch (Exception storedProcExp)
					{
						MessageBox.Show(storedProcExp.Message, "Stored procedure error!");
						return;
					}

					#endregion
				}
				else
					if (useSQLProc() == SQL_ERROR)
					{
						MessageBox.Show("No SQL statements specified!");
						return;
					}
			#endregion
		}
		catch (Exception exp)
		{
			MessageBox.Show(exp.Message, "Acquire data for display error");
			return;
		}
	}

	private void ExtractGrid_Load(object sender, EventArgs e)
	{
		this.dg_extractionGrid.AutoGenerateColumns = false;

		#region Set Colors and Styles

		// Allow colour change next time
		dg_extractionGrid.RowsDefaultCellStyle.BackColor = Color.Bisque;
		dg_extractionGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;
		dg_extractionGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;

		dg_extractionGrid.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
		dg_extractionGrid.DefaultCellStyle.SelectionForeColor = Color.Black;

		txt_lines.BackColor = Color.WhiteSmoke;

		#endregion

		try
		{
			#region Construct Extraction Display
			// If preload was not called.
			if (finalextraction == null)
			{
				acquireDataForDisplay();
			}

			#region 4. Populate destination tables array. Filter the result from the already extracted ones. Set using IncludeAlreadyExtracted column.

			try
			{
				// Split up the multiple destination tables
				// For each destination table, delete what already exists in current dataset.
				string delimStr = ",";
				char[] delimiter = delimStr.ToCharArray();
				string[] split = extractionInfoObject.DestinationTable.Split(delimiter);

				if (split.Length != 0)
				{
					destinationTables = new string[split.Length];
					int i = 0;
					try
					{
						foreach (string s in split)
						{
							destinationTables[i++] = s;
							if (!extractionInfoObject.IncludeAlreadyExtracted)
							{
								string targetColumn, sourceColumn;

								sourceColumn = extractionInfoObject.ExtractionKeyField.Split("-".ToCharArray())[0].Trim();
								targetColumn = extractionInfoObject.ExtractionKeyField.Split("-".ToCharArray())[1].Trim();

								string currentDataSet = "Select " + targetColumn + " from [" + s + "]";
								DataTable currentDataSetTb = BizFunctions.ExecuteQuery(dbaccess.DataSet, currentDataSet);

								foreach (DataRow dr in this.finalextraction.Rows)
								{
									DataRow[] alreadyExtractedRows = currentDataSetTb.Select(targetColumn + "='" + dr[sourceColumn].ToString().Trim() + "'");
									if (alreadyExtractedRows.Length != 0)
									{
										dr.Delete();
									}
								}
							}
						}
					}
					catch
					{
						MessageBox.Show("Destination table format invalid!", "DestinationTable");
						return;
					}
				}
			}
			catch
			{
				MessageBox.Show("Filter from current dataset error! Please check ExtractionKeyField column!", "Filter error");
				return;
			}

			#endregion

			#region 5. Form the DataGrid

			// Assign name
			dg_extractionGrid.Name = extractionInfoObject.ExtractionGridName;

			#region Add Columns to DataGrid

			#region Add Mandatory Mark Column

			DataColumn d_mark = new DataColumn("mark", typeof(Boolean));
			this.finalextraction.Columns.Add(d_mark);

			DataGridViewCheckBoxColumn mark = new DataGridViewCheckBoxColumn();

			mark.DataPropertyName = "mark";
			mark.FalseValue = "false";
			mark.HeaderText = "Mark";
			mark.IndeterminateValue = "false";
			mark.Name = "mark";
			mark.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			mark.TrueValue = "true";
			mark.Width = 40;

			dg_extractionGrid.Columns.Add(mark);

			#endregion

			#region Add Columns to be Displayed

			string delimStrc = ",";
			char[] delimiterc = delimStrc.ToCharArray();
			string[] splitc = extractionInfoObject.ColumnsToDisplay.Split(delimiterc);

			// Format of s = oricur-40
			foreach (String s in splitc)
			{
				string delimStrd = "-";
				char[] delimiterd = delimStrd.ToCharArray();
				string[] splitd = s.Split(delimiterd);
				DataColumn dc = finalextraction.Columns[splitd[0]];
				switch (dc.DataType.Name)
				{
					case "DateTime":
						DataGridViewTextBoxColumn newdatetimecolumn = new DataGridViewTextBoxColumn();

						newdatetimecolumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
						newdatetimecolumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
						newdatetimecolumn.DataPropertyName = dc.ColumnName;
						newdatetimecolumn.HeaderText = dc.ColumnName;
						newdatetimecolumn.Name = dc.ColumnName;
						newdatetimecolumn.Width = int.Parse(splitd[1]);
						dg_extractionGrid.Columns.Add(newdatetimecolumn);
						break;

					case "Decimal":
						DataGridViewTextBoxColumn newdecimalcolumn = new DataGridViewTextBoxColumn();

						newdecimalcolumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

						// Make flexible later.
						if (dc.ColumnName.Trim() == "exrate")
							newdecimalcolumn.DefaultCellStyle.Format = "f6";
						else
							newdecimalcolumn.DefaultCellStyle.Format = "f2";

						newdecimalcolumn.DataPropertyName = dc.ColumnName;
						newdecimalcolumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
						newdecimalcolumn.HeaderText = dc.ColumnName;
						newdecimalcolumn.Name = dc.ColumnName;
						newdecimalcolumn.Width = int.Parse(splitd[1]);
						dg_extractionGrid.Columns.Add(newdecimalcolumn);
						break;

					case "String":
						DataGridViewTextBoxColumn newtextboxcolumn = new DataGridViewTextBoxColumn();

						newtextboxcolumn.DataPropertyName = dc.ColumnName;
						newtextboxcolumn.HeaderText = dc.ColumnName;
						newtextboxcolumn.Name = dc.ColumnName;
						newtextboxcolumn.Width = int.Parse(splitd[1]);
						dg_extractionGrid.Columns.Add(newtextboxcolumn);
						break;

					case "Int32":
						DataGridViewTextBoxColumn newintcolumn = new DataGridViewTextBoxColumn();

						newintcolumn.DataPropertyName = dc.ColumnName;
						newintcolumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
						newintcolumn.HeaderText = dc.ColumnName;
						newintcolumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
						newintcolumn.Name = dc.ColumnName;
						newintcolumn.Width = int.Parse(splitd[1]);
						dg_extractionGrid.Columns.Add(newintcolumn);
						break;

					case "Boolean":

						if (dc.ColumnName.ToLower() != "mark")
						{
							DataGridViewCheckBoxColumn newcheckboxcolumn = new DataGridViewCheckBoxColumn();

							newcheckboxcolumn.DataPropertyName = dc.ColumnName;
							newcheckboxcolumn.FalseValue = "false";
							newcheckboxcolumn.HeaderText = dc.ColumnName;
							newcheckboxcolumn.IndeterminateValue = "false";
							newcheckboxcolumn.Name = dc.ColumnName;
							newcheckboxcolumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
							newcheckboxcolumn.TrueValue = "true";
							newcheckboxcolumn.Width = int.Parse(splitd[1]);
							dg_extractionGrid.Columns.Add(newcheckboxcolumn);
						}
						break;
				}
			}

			#endregion

			#endregion

			#endregion

			#region Format columntypes

			//foreach (DataGridViewColumn dc in dg_extractionGrid.Columns)
			//{
			//    switch (dc.DataPropertyName)
			//    {
			//        case "grosamt":

			//            dc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			//            dc.DefaultCellStyle.Format = "f2";
			//            break;

			//        case "trandate":
			//        case "civdate":

			//            dc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			//            break;
			//    }

			//}
			#endregion

			// Dynamic sorting mechanism. Input in default sorting.
			DataView finalextractdv = new DataView(finalextraction);

			this.dg_extractionGrid.DataSource = finalextractdv;

			AddContextMenu();
			dg_extractionGrid.ClearSelection();

			SizeColumns(dg_extractionGrid);

			// First Row Selected as default
			if (dg_extractionGrid.Rows.Count > 0)
				dg_extractionGrid.Rows[0].Selected = true;

			txt_lines.Text = (dg_extractionGrid.DataSource as DataView).Count + " lines";

			#endregion

			#region Initialize Filters

			// For each column add an item to the combobox.
			foreach (Object ob in dg_extractionGrid.Columns)
			{
				if (ob is DataGridViewTextBoxColumn)
				{
					DataGridViewTextBoxColumn dc = ob as DataGridViewTextBoxColumn;

					if (dc.ValueType != null)
					{
						// If datetime then add to datetime columns list.
						if (dc.ValueType.Name.ToLower() == "datetime")
							DateTimeColumns.Add(dc.DataPropertyName);
						//if (dc.ValueType.Name.ToLower() != "decimal" && dc.ValueType.Name.ToLower() != "datetime")
						cmb_columnFilter.Items.Add(new filterComboBoxObject(dc.HeaderText, dc.DataPropertyName));
					}
				}
			}

			#endregion
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "DataGrid Construction Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

	}

	#region Context menu strip

	private void AddContextMenu()
	{
		ContextMenuStrip strip = new ContextMenuStrip();

		ToolStripMenuItem checkAll = new ToolStripMenuItem();
		checkAll.Text = "Check All";
		checkAll.Click += new EventHandler(checkAll_Click);
		ToolStripMenuItem unCheckAll = new ToolStripMenuItem();
		unCheckAll.Text = "Uncheck All";
		unCheckAll.Click += new EventHandler(unCheckAll_Click);

		DataGridViewColumn markColumn = dg_extractionGrid.Columns["mark"];

		markColumn.ContextMenuStrip = strip;
		markColumn.ContextMenuStrip.Items.Add(checkAll);
		markColumn.ContextMenuStrip.Items.Add(unCheckAll);
	}

	private void checkAll_Click(object sender, EventArgs args)
	{
		dg_extractionGrid.ClearSelection();
		foreach (DataRowView dr in (dg_extractionGrid.DataSource as DataView))
		{
			dr["mark"] = true;
		}

		if ((dg_extractionGrid.DataSource as DataView).Count > 0)
			dg_extractionGrid.Rows[0].Selected = true;

		dg_extractionGrid.RefreshEdit();

	}

	private void unCheckAll_Click(object sender, EventArgs args)
	{
		dg_extractionGrid.ClearSelection();
		foreach (DataRowView dr in (dg_extractionGrid.DataSource as DataView))
		{
			dr["mark"] = false;
		}

		if ((dg_extractionGrid.DataSource as DataView).Count > 0)
			dg_extractionGrid.Rows[0].Selected = true;

		dg_extractionGrid.RefreshEdit();

	}

	#endregion

	private void btn_extract_Click(object sender, EventArgs e)
	{
		#region Obtain the data source

		if ((useSQLProc() == SQL_DIRECT && extractionInfoObject.SqlDirectForDisplay.Trim() == extractionInfoObject.SqlDirectForData.Trim())
			||
			(useSQLProc() == SQL_PROC && extractionInfoObject.SqlProcNameForData.Trim() == extractionInfoObject.SqlProcNameForDisplay.Trim()))
		{
			// DataSource in this case is same as display table.
			dataSource = finalextraction;
		}
		else
			if (useSQLProc() == SQL_DIRECT && extractionInfoObject.SqlDirectForDisplay.Trim() != extractionInfoObject.SqlDirectForData.Trim())
			{
				// Launch the sql to acquire the data source.
				#region 1. Execute Direct Sql Statement for Data

				try
				{
					DataSet ds = this.dbaccess.ReadSQLTemp("datasource", extractionInfoObject.SqlDirectForData);

					this.dataSource = ds.Tables[0].Copy();

					this.dataSource.TableName = "dataSource";
				}
				catch (Exception directSqlExp)
				{
					MessageBox.Show(directSqlExp.Message, "Direct Sql for Data execution error!");
					return;
				}

				#endregion
			}
			else
				if (useSQLProc() == SQL_PROC && extractionInfoObject.SqlProcNameForData.Trim() != extractionInfoObject.SqlProcNameForDisplay.Trim())
				{
					// Launch the sql procedure to acquire the data source.
					#region  2. Construct the sql procedure parameters. Format passed in is "@param1=aaa,@param2=bbb".

					string delimStr = ",/*";
					char[] delimiter = delimStr.ToCharArray();
					string[] split = extractionInfoObject.SqlProcParametersForData.Split(delimiter);

					if (split.Length != 0)
					{
						int i = 0;

						parameters = new Parameter[split.Length];

						try
						{
							// s looks like @param1=paramstring
							foreach (string s in split)
							{
								string delimStrb = "=";
								char[] delimiterb = delimStrb.ToCharArray();

								// Only split on first = sign.
								string[] splitb = s.Split(delimiterb, 2);

								string param, paramvalue;

								param = splitb[0];
								paramvalue = splitb[1];

								parameters[i] = new Parameter(param, paramvalue);

								i++;
							}
						}
						catch
						{
							MessageBox.Show("Sql Procedure Parameters format invalid! (Example: @sitenum ='HQ')", "SqlProcParametersForDisplay");
							return;
						}
					}

					#endregion

					#region 3. Get the result from the storedprocedure

					try
					{
						DataSet ds = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult(extractionInfoObject.SqlProcNameForData, ref parameters);

						this.dataSource = ds.Tables[0].Copy();

						this.dataSource.TableName = "dataSource";
					}
					catch (Exception storedProcExp)
					{
						MessageBox.Show(storedProcExp.Message, "Stored procedure error!");
						return;
					}

					#endregion
				}
				else
					if (useSQLProc() == SQL_ERROR)
					{
						MessageBox.Show("No SQL statements specified!");
						return;
					}

		#endregion

		#region When Data Source same as Display

		#region Extract the data source into the destination tables

		try
		{
			if ((useSQLProc() == SQL_DIRECT && extractionInfoObject.SqlDirectForDisplay.Trim() == extractionInfoObject.SqlDirectForData.Trim())
				||
				(useSQLProc() == SQL_PROC && extractionInfoObject.SqlProcNameForData.Trim() == extractionInfoObject.SqlProcNameForDisplay.Trim()))
			{
				// Form columnMappings hashtable
				createColumnMappings();

				// Form the extractedTables array.
				createExtractedTables(columnMappings);
			}

			// Take the extracted table and merge to the destination tables.
			int i = 0;
			foreach (String s in destinationTables)
			{
				dbaccess.DataSet.Tables[s].Merge(extractedTables[i], (1 == 1), MissingSchemaAction.Ignore);
				i++;
			}
		}
		catch (Exception extractionException)
		{
			MessageBox.Show(extractionException.Message, "Extraction Error!");
			return;
		}


		for (int i = finalextraction.Rows.Count; i > 0; i--)
		{
			if (finalextraction.Rows[i - 1].RowState != DataRowState.Deleted)
			{
				if (Convert.IsDBNull(finalextraction.Rows[i - 1]["mark"]))
					finalextraction.Rows[i - 1]["mark"] = false;

				if ((bool)finalextraction.Rows[i - 1]["mark"])
					finalextraction.Rows[i - 1].Delete();
			}
		}


		txt_lines.Text = (dg_extractionGrid.DataSource as DataView).Count + " lines";

		if (finalextraction.Select().Length == 0)
			btn_cancel_Click(null, null);

		#endregion

		#endregion
	}

	// This function populates the extractedTables array. Specifies a mirror image of the destination table one step before
	// the actual extraction is done. The tables in extractedTables array are intermediary tables.
	// sourcetable -> extractedTables -> destinationTables
	private int createExtractedTables(Hashtable columnMappings)
	{

		// columnMappings will be in the form of eg. refnum-table1@invnum
		#region Validate that each destination table in columnMapping matches an entry in destination tables parameter and vice versa
		IDictionaryEnumerator enumeratorvalidate = columnMappings.GetEnumerator();

		while (enumeratorvalidate.MoveNext())
		{
			string destinationTable = enumeratorvalidate.Value.ToString().Trim().Substring(0, enumeratorvalidate.Value.ToString().Trim().IndexOf('@'));

			bool exists = false;

			for (int i = 0; i < destinationTables.Length; i++)
			{
				if (destinationTables[i] == destinationTable)
					exists = true;
			}

			if (!exists)
			{
				MessageBox.Show("ColumnsToCopy specify more tables than DestinationTable");
				return -1;
			}
		}

		// Make sure each destination table has a columnmapping
		for (int i = 0; i < destinationTables.Length; i++)
		{
			IDictionaryEnumerator enumeratorbvalidate = columnMappings.GetEnumerator();
			bool exists = false;

			while (enumeratorbvalidate.MoveNext())
			{
				string destinationTable = enumeratorbvalidate.Value.ToString().Trim().Substring(0, enumeratorbvalidate.Value.ToString().Trim().IndexOf('@'));
				if (destinationTable == destinationTables[i])
					exists = true;
			}

			if (!exists)
			{
				MessageBox.Show("DestinationTable specify more tables than ColumnsToCopy");
				return -1;
			}
		}

		#endregion

		#region Populate the extractedTables array

		// Each extractedTable will be merged into the corresponding destination table.
		// We use an intermediary table rather than adding straight into destination table 
		// because it is faster for huge extractions.
		// Disadvantage is, more code.
		extractedTables = new DataTable[destinationTables.Length];
		// Assign names to the extractedTables to match destination tables.
		for (int i = 0; i < destinationTables.Length; i++)
		{
			extractedTables[i] = new DataTable(destinationTables[i]);
		}

		// Add the columns to each extractedTable
		IEnumerator destinationColumns = columnMappings.Values.GetEnumerator();
		while (destinationColumns.MoveNext())
		{
			String columnName = destinationColumns.Current.ToString().Trim().Substring(destinationColumns.Current.ToString().Trim().IndexOf('@') + 1);
			String tableName = destinationColumns.Current.ToString().Trim().Substring(0, destinationColumns.Current.ToString().Trim().IndexOf('@'));

			DataColumn col = new DataColumn(columnName);

			for (int i = 0; i < extractedTables.Length; i++)
			{
				if (extractedTables[i].TableName == tableName)
					extractedTables[i].Columns.Add(col);
			}
		}


		foreach (DataRowView dr in (dg_extractionGrid.DataSource as DataView))
		{
			if (Convert.IsDBNull(dr["mark"])) dr["mark"] = false;
			if ((bool)dr["mark"])
			{
				IDictionaryEnumerator enumeratora = columnMappings.GetEnumerator();
				ArrayList addedRow = new ArrayList();
				DataRow[] newrows = new DataRow[destinationTables.Length];

				// Create necessary rows for the certain extractedTables
				while (enumeratora.MoveNext())
				{
					string tablename = enumeratora.Value.ToString().Trim().Substring(0, enumeratora.Value.ToString().Trim().IndexOf('@'));
					int currentTableIndex = 0;

					for (int i = 0; i < extractedTables.Length; i++)
					{
						if (extractedTables[i].TableName == tablename)
						{
							currentTableIndex = i;

							if (!addedRow.Contains(tablename))
							{
								addedRow.Add(tablename);
								newrows[i] = extractedTables[i].NewRow();
							}
						}
					}

					// 1*refnum becomes refnum 
					string columnMappingKey = enumeratora.Key.ToString().Trim().Substring(enumeratora.Key.ToString().Trim().IndexOf('*') + 1);

					// Value is destination table. Key is data source.
					newrows[currentTableIndex][enumeratora.Value.ToString().Trim().Substring(enumeratora.Value.ToString().Trim().IndexOf('@') + 1)] = dr[columnMappingKey];

				}

				for (int i = 0; i < destinationTables.Length; i++)
				{
					extractedTables[i].Rows.Add(newrows[i]);
				}
			}
		}

		#endregion

		return 0;
	}

	// This function populates the columnMappings hashtable. Specifies which source column to map to which destination column
	private void createColumnMappings()
	{

		columnMappings = new Hashtable();

		string delimStr = ",";
		char[] delimiter = delimStr.ToCharArray();
		string[] split = extractionInfoObject.ColumnsToCopy.Trim().Split(delimiter);

		if (split.Length != 0)
		{
			try
			{
				// Foreach mapping
				foreach (string s in split)
				{
					// Split the mappings and store into hashtable
					string delimStrb = "-";
					char[] delimiterb = delimStrb.ToCharArray();
					string[] splitb = s.Trim().Split(delimiterb);

					// 1*refnum = invnum , 2 refnum = refnum   ... making sure each entry has a unique key.
					columnMappings[columnMappings.Count.ToString() + "*" + splitb[0]] = splitb[1];
				}
			}
			catch
			{
				MessageBox.Show("Destination table format invalid!", "DestinationTable");
				return;
			}
		}
	}

	private int useSQLProc()
	{
		// If no direct sql for data, then must use sql proc
		if (extractionInfoObject.SqlDirectForData.Trim() == String.Empty && extractionInfoObject.SqlProcNameForData.Trim() != String.Empty)
		{
			return SQL_PROC;
		}

		// If there is direct sql, then must use direct sql.
		if (extractionInfoObject.SqlDirectForData.Trim() != String.Empty)
		{
			return SQL_DIRECT;
		}

		// If none specified, then error.
		if (extractionInfoObject.SqlDirectForData == String.Empty && extractionInfoObject.SqlProcNameForData == String.Empty)
		{
			return SQL_ERROR;
		}

		return SQL_ERROR;
	}

	protected class filterComboBoxObject
	{
		private string m_Display;
		private string m_Value;
		public filterComboBoxObject(string Display, string Value)
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

	private void btn_addFilter_Click(object sender, EventArgs e)
	{
		// Add the selected filter into the filter box.
		if (cmb_columnFilter.SelectedIndex != -1 && (txt_filterText.Text.Trim().Length > 0 || dtp_datetime.Value != null))
		{
			filterComboBoxObject newObject = null;

			// If like operator, then must use %
			if ((cmb_filterOperator.SelectedItem as filterComboBoxObject).Value.ToLower().Trim() == "like" ||
				 (cmb_filterOperator.SelectedItem as filterComboBoxObject).Value.ToLower().Trim() == "not like")
			{
				if (isDateColumn())
					newObject = new filterComboBoxObject((cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Display + " " + dtp_datetime.Value.ToShortDateString(),
														 (cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Value + " %#" + dtp_datetime.Value.Month + "/" + dtp_datetime.Value.Day + "/" + dtp_datetime.Value.Year + "#%");
				else
					newObject = new filterComboBoxObject((cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Display + " " + txt_filterText.Text,
														 (cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Value + " '%" + txt_filterText.Text + "%'");
			}
			// else don't need
			else
			{
				if (isDateColumn())
					newObject = new filterComboBoxObject((cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Display + " " + dtp_datetime.Value.ToShortDateString(),
														 (cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Value + " #" + dtp_datetime.Value.Month + "/" + dtp_datetime.Value.Day + "/" + dtp_datetime.Value.Year + "#");
				else
					newObject = new filterComboBoxObject((cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Display + " " + txt_filterText.Text,
														 (cmb_columnFilter.SelectedItem as filterComboBoxObject).Value + " " + (cmb_filterOperator.SelectedItem as filterComboBoxObject).Value + " '" + txt_filterText.Text + "'");
			}

			bool exists = false;
			for (int i = 0; i < lbx_filterbox.Items.Count; i++)
				if ((lbx_filterbox.Items[i] as filterComboBoxObject).Value == newObject.Value
					 && (lbx_filterbox.Items[i] as filterComboBoxObject).Display == newObject.Display)
				{
					exists = true;
					break;
				}

			if (!exists)
				lbx_filterbox.Items.Add(newObject);

			//Clear filters
			cmb_columnFilter.SelectedIndex = -1;
			txt_filterText.Text = String.Empty;
			dtp_datetime.Visible = false;
			txt_filterText.Visible = true;
			DataView dv = new DataView(finalextraction);

			try
			{
				// There is always at least one time if ran till here.
				dv.RowFilter = (lbx_filterbox.Items[0] as filterComboBoxObject).Value;

				for (int i = 1; i < lbx_filterbox.Items.Count; i++)
					dv.RowFilter = dv.RowFilter + " and " + (lbx_filterbox.Items[i] as filterComboBoxObject).Value;

			}
			catch
			{
				MessageBox.Show("Invalid value, please try again");
				// Remove the culprit filter.
				lbx_filterbox.Items.Remove(lbx_filterbox.Items[lbx_filterbox.Items.Count - 1]);
				txt_filterText.Text = String.Empty;
			}
			dg_extractionGrid.DataSource = dv;
			txt_lines.Text = (dg_extractionGrid.DataSource as DataView).Count + " lines";
		}
	}

	private void btn_removeFilter_Click(object sender, EventArgs e)
	{
		if (lbx_filterbox.SelectedIndex != -1)
		{
			lbx_filterbox.Items.Remove(lbx_filterbox.Items[lbx_filterbox.SelectedIndex]);

			DataView dv = new DataView(finalextraction);
			if (lbx_filterbox.Items.Count != 0)
			{
				dv.RowFilter = (lbx_filterbox.Items[0] as filterComboBoxObject).Value;

				for (int i = 1; i < lbx_filterbox.Items.Count; i++)
					dv.RowFilter = dv.RowFilter + " and " + (lbx_filterbox.Items[i] as filterComboBoxObject).Value;
			}
			dg_extractionGrid.DataSource = dv;
			txt_lines.Text = (dg_extractionGrid.DataSource as DataView).Count + " lines";
		}
	}

	private void btn_cancel_Click(object sender, EventArgs e)
	{
		if (frmThis != null)
			this.frmThis.Dispose();
		else
			this.Close();
	}

	// Operators, contains - equals - less than - greater than
	private void cmb_columnFilter_SelectionChangeCommitted(object sender, EventArgs e)
	{
		// Upon selection, populate the column filters depending on the type of the column
		filterComboBoxObject obj = (sender as ComboBox).SelectedItem as filterComboBoxObject;
		cmb_filterOperator.Items.Clear();

		switch (this.finalextraction.Columns[obj.Value].DataType.Name.ToLower())
		{
			case "datetime":
				dtp_datetime.Visible = true;
				txt_filterText.Visible = false;
				cmb_filterOperator.Items.Add(new filterComboBoxObject("equals", "="));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("greater than", ">"));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("less than", "<"));
				cmb_filterOperator.SelectedIndex = 0;
				break;

			case "int32":
			case "decimal":
				dtp_datetime.Visible = false;
				txt_filterText.Visible = true;
				cmb_filterOperator.Items.Add(new filterComboBoxObject("equals", "="));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("greater than", ">"));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("less than", "<"));
				cmb_filterOperator.SelectedIndex = 0;
				break;

			default:
				dtp_datetime.Visible = false;
				txt_filterText.Visible = true;
				cmb_filterOperator.Items.Add(new filterComboBoxObject("contains", "like"));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("does not contain", "not like"));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("equals", "="));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("greater than", ">"));
				cmb_filterOperator.Items.Add(new filterComboBoxObject("less than", "<"));
				cmb_filterOperator.SelectedIndex = 0;
				break;
		}
	}

	// This function validates what has been passed in as parameters to the extraction object through the script.
	private bool validateExtractionParameters()
	{

		if (useSQLProc() == SQL_DIRECT && extractionInfoObject.SqlDirectForData.Trim() == String.Empty)
		{
			MessageBox.Show("SqlDirectForData parameter is missing");
			return false;
		}

		if (useSQLProc() == SQL_PROC && extractionInfoObject.SqlProcNameForData.Trim() == String.Empty)
		{
			MessageBox.Show("SqlProcNameForData parameter is missing");
			return false;
		}

		if (extractionInfoObject.ColumnsToDisplay.Trim() == String.Empty)
		{
			MessageBox.Show("ColumnsToDisplay parameter is missing");
			return false;
		}

		if (extractionInfoObject.ColumnsToCopy.Trim() == String.Empty)
		{
			MessageBox.Show("ColumnsToCopy parameter is missing");
			return false;
		}

		if (extractionInfoObject.DestinationTable.Trim() == String.Empty)
		{
			MessageBox.Show("DestinationTable parameter is missing");
			return false;
		}

		if (extractionInfoObject.ExtractionKeyField.Trim() == String.Empty)
		{
			MessageBox.Show("ExtractionKeyField parameter is missing");
			return false;
		}

		return true;
	}

	// Press enter and filter is added
	private void txt_filterText_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Enter)
			btn_addFilter_Click(sender, null);
	}

	private bool isDateColumn()
	{
		bool isDate = false;
		// If date then filters must have #
		for (int i = 0; i < DateTimeColumns.Count; i++)
			if ((cmb_columnFilter.SelectedItem as filterComboBoxObject).Value.Contains(DateTimeColumns[i] as String))
			{
				isDate = true;
				break;
			}

		return isDate;
	}

	protected void SizeColumns(DataGridView grid)
	{
		Graphics g = CreateGraphics();

		DataView dataTable = (DataView)grid.DataSource;

		foreach (DataGridViewColumn dataColumn in grid.Columns)
		{
			int maxSize = 0;

			SizeF size = g.MeasureString(
							dataColumn.HeaderText,
							grid.Font
						 );

			if (size.Width > maxSize)
				maxSize = (int)size.Width;

			foreach (DataRow row in dataTable.Table.Rows)
			{
				if (row.RowState != DataRowState.Deleted)
				{
					size = g.MeasureString(
							  row[dataColumn.Name].ToString().Trim(),
							  grid.Font
						);

					if (size.Width > maxSize)
						maxSize = (int)size.Width;
				}
			}

			//dataColumn.Name = grid[dataColumn.Name].ToString().Trim();
			//dataColumn.HeaderText = grid[dataColumn.Name].ToString().Trim();
			dataColumn.Width = maxSize + 5;
		}

		g.Dispose();
	}
}