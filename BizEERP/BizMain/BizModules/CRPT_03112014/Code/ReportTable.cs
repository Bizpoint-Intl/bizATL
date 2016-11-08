using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CarlosAg.ExcelXmlWriter;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ATL.CRPT
{
	public partial class ReportTable : Form
	{
		//string columnList;

		public ReportTable(ReportObject reportObject)
		{
			InitializeComponent();
			consumeReportObject(reportObject);

			SizeColumns(dgv_reportTable);

			#region Set Colors and Styles
			dgv_reportTable.RowsDefaultCellStyle.BackColor = Color.Bisque;
			dgv_reportTable.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;
			dgv_reportTable.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
			dgv_reportTable.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
			dgv_reportTable.DefaultCellStyle.SelectionForeColor = Color.Black;
			#endregion

			this.Focus();
			this.Show();


		}

		// Consumes the report object and makes the parameters useful for the report and datagridview.
		private void consumeReportObject(ReportObject reportObject)
		{
			// Form the proper table with the visible columns.
			DataTable displayTable = reportObject.dataSource.Tables["report"].Copy();

			dgv_reportTable.DataSource = displayTable;

			this.txt_title.Text = reportObject.title;
		}


		// Returns true if the column name is in the columnlist else false
		private bool visibleColumn(string columnList, string columnName)
		{
			string delimStrd = ",";
			char[] delimiterd = delimStrd.ToCharArray();
			string[] splitd = columnList.Split(delimiterd);

			for (int i = 0; i < splitd.Length; i++)
			{
				if (splitd[i] == columnName || splitd[i] == "["+columnName+"]")
					return true;
			}

			return false;
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

		private void ReportTable_Resize(object sender, EventArgs e)
		{
			// When window is resized, resize the datagrid too.

			dgv_reportTable.Width = this.Width - 20;
			dgv_reportTable.Height = this.Height - 80;
		}

		private void btn_Print_Click(object sender, EventArgs e)	
		{
			string defaultPath = Environment.CurrentDirectory;

			using (SaveFileDialog saveFileDialog = GetExcelSaveFileDialog())
			{
				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					string fileName = saveFileDialog.FileName;

					Workbook workbook = ExcelGenerator.Generate(dgv_reportTable);
					workbook.Save(fileName);
					//Process.Start(fileName);
				}
			}

			Environment.CurrentDirectory = defaultPath;
		}

		private SaveFileDialog GetExcelSaveFileDialog()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.CheckPathExists = true;
			saveFileDialog.AddExtension = true;
			saveFileDialog.ValidateNames = true;
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			saveFileDialog.DefaultExt = ".xls";
			saveFileDialog.Filter = "Microsoft Excel Workbook (*.xls)|*.xls";
			return saveFileDialog;
		}

		private void ReportTable_Load(object sender, EventArgs e)
		{

			int highestwidth = 0;
			for (int i = 0; i < (dgv_reportTable.DataSource as DataTable).Rows.Count; i++)
			{
				int totalwidth = 0;
				for (int j = 0; j < (dgv_reportTable.DataSource as DataTable).Columns.Count; j++)
				{
					totalwidth += dgv_reportTable.GetCellDisplayRectangle(j, i, true).Width;
					
					
				}
				if (totalwidth > highestwidth)
					highestwidth = totalwidth;
			}


			//this.Width = highestwidth;
			this.Width = dgv_reportTable.Width + 110;
		}

		private void tsbtn_quickexport_Click(object sender, EventArgs e)
		{
			string defaultPath = Environment.CurrentDirectory;

			using (SaveFileDialog saveFileDialog = GetExcelSaveFileDialog())
			{
				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					string fileName = saveFileDialog.FileName;

					// create the DataGrid and perform the databinding
					System.Web.UI.WebControls.DataGrid grid = new System.Web.UI.WebControls.DataGrid();
					grid.HeaderStyle.Font.Bold = true;
					grid.DataSource = dgv_reportTable.DataSource;
					grid.DataMember = (dgv_reportTable.DataSource as DataTable).TableName;

					grid.DataBind();

					// render the DataGrid control to a file
					using (StreamWriter sw = new StreamWriter(fileName))
					{
						using (HtmlTextWriter hw = new HtmlTextWriter(sw))
						{
							grid.RenderControl(hw);
						}
					}
				}
			}

			Environment.CurrentDirectory = defaultPath;
		}
	}


	public class ReportObject
	{
		// Essential report object variables.

		private DataSet _datasource;
		private String _columnList;
		private String _title;
		private String _reportid;
		private String _filterList;

		public DataSet dataSource
		{
			get
			{
				return _datasource;
			}

			set
			{
				_datasource = value;
			}
		}

		public String columnList
		{
			get
			{
				return _columnList;
			}

			set
			{
				_columnList = value;
			}
		}

		public String title
		{
			get
			{
				return _title;
			}

			set
			{
				_title = value;
			}
		}

		public String reportid
		{
			get
			{
				return _reportid;
			}

			set
			{
				_reportid = value;
			}
		}

		public String filterList
		{
			get
			{
				return _filterList;
			}

			set
			{
				_filterList = value;
			}
		}

		public ReportObject(DataSet dataSource, String columnList, String title, String reportid, String filterList)
		{
			_datasource = dataSource;
			_columnList = columnList;
			_title = title;
			_reportid = reportid;
			_filterList = filterList;
		}

	}

	public static class ExcelGenerator
	{
		public static Workbook Generate(DataGridView dataGridView)
		{
			Workbook workbook = new Workbook();
			Worksheet worksheet = workbook.Worksheets.Add("Sheet 1");

			WorksheetRow worksheetRow = new WorksheetRow();
			foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns)
			{
				worksheet.Table.Columns.Add(new WorksheetColumn(dataGridViewColumn.Width));
				worksheetRow.Cells.Add(new WorksheetCell(dataGridViewColumn.HeaderText));
			}
			worksheet.Table.Rows.Insert(0, worksheetRow);

			WorksheetStyle worksheetDefaultStyle = GetWorksheetStyle(dataGridView.DefaultCellStyle, "Default");
			workbook.Styles.Add(worksheetDefaultStyle);

			
			for (int rowIndex = 0; rowIndex < dataGridView.RowCount; ++rowIndex)
			{
				worksheetRow = worksheet.Table.Rows.Add();

				for (int columnIndex = 0; columnIndex < dataGridView.ColumnCount; ++columnIndex)
				{
					DataGridViewCell cell = dataGridView[columnIndex, rowIndex];
					WorksheetStyle cellStyle = GetWorksheetStyle(cell.InheritedStyle, "column" + columnIndex + "row" + rowIndex);

					if (cellStyle != null)
					{
						workbook.Styles.Add(cellStyle);
					}
					else
					{
						cellStyle = worksheetDefaultStyle;
					}

					DataType dataType = GetDataType(cell.ValueType);
					worksheetRow.Cells.Add(cell.FormattedValue.ToString(), dataType, cellStyle.ID);
				}
			}

			return workbook;
		}

		private static WorksheetStyle GetWorksheetStyle(DataGridViewCellStyle dataGridViewCellStyle, string id)
		{
			WorksheetStyle worksheetStyle = null;

			if (dataGridViewCellStyle != null)
			{
				worksheetStyle = new WorksheetStyle(id);
				if (!dataGridViewCellStyle.BackColor.IsEmpty)
				{
					worksheetStyle.Interior.Color = GetColorName(dataGridViewCellStyle.BackColor);
					worksheetStyle.Interior.Pattern = StyleInteriorPattern.Solid;
				}

				if (!dataGridViewCellStyle.ForeColor.IsEmpty)
				{
					worksheetStyle.Font.Color = GetColorName(dataGridViewCellStyle.ForeColor);
				}

				if (dataGridViewCellStyle.Font != null)
				{
					worksheetStyle.Font.Bold = dataGridViewCellStyle.Font.Bold;
					worksheetStyle.Font.FontName = dataGridViewCellStyle.Font.Name;
					worksheetStyle.Font.Italic = dataGridViewCellStyle.Font.Italic;
					worksheetStyle.Font.Size = (int)dataGridViewCellStyle.Font.Size;
					worksheetStyle.Font.Strikethrough = dataGridViewCellStyle.Font.Strikeout;
					worksheetStyle.Font.Underline = dataGridViewCellStyle.Font.Underline ? UnderlineStyle.Single : UnderlineStyle.None;
				}

				worksheetStyle.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "Black");
				worksheetStyle.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "Black");
				worksheetStyle.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "Black");
				worksheetStyle.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "Black");
			}

			return worksheetStyle;
		}

		private static string GetColorName(Color color)
		{
			return "#" + color.ToArgb().ToString("X").Substring(2);
		}

		private static DataType GetDataType(Type valueType)
		{
			if (valueType == typeof(DateTime))
			{
				return DataType.String;
			}
			else if (valueType == typeof(string))
			{
				return DataType.String;
			}
			else if (valueType == typeof(sbyte)
			  || valueType == typeof(byte)
			  || valueType == typeof(short)
			  || valueType == typeof(ushort)
			  || valueType == typeof(int)
			  || valueType == typeof(uint)
			  || valueType == typeof(long)
			  || valueType == typeof(ulong)
			  || valueType == typeof(float)
			  || valueType == typeof(double)
			  || valueType == typeof(decimal))
			{
				return DataType.Number;
			}
			else
			{
				return DataType.String;
			}
		}
	}



}