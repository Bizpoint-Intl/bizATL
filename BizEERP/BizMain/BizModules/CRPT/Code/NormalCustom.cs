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

using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;

using BizRAD.BizReport;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;



using BizRAD.BizAccounts;
using BizRAD.BizTools;

// Custom Reports

// Tables Used : SysCustomReportLayout : To Store Different Layouts of Reports
//				 SysCustomReportType   : To Store Different Report Types (Pivot, Normal, Charts)
// Procedures Used : sp_savecustomreportlayout : To save custom report layouts

namespace ATL.CRPT
{
	public partial class NormalCustom : Form
	{
		DBAccess dbaccess = null;
		ReportObject reportObject = null;
		DataTable layout = null;
		DataTable reportType = null;
		DataTable displayTable = null;
		string currentEnvironment = Environment.CurrentDirectory;
		const int Normal = 0;
		const int Pivot = 1;

		public NormalCustom(ReportObject ReportObject)
		{
			InitializeComponent();
			reportObject = ReportObject;
			consumeReportObject(ReportObject);
			dbaccess = new DBAccess();

			customReportInit();

			this.Focus();
			this.Show();

		}

		// Consumes the report object and makes the parameters useful for the report and datagridview.
		private void consumeReportObject(ReportObject ReportObject)
		{
			// Form the proper table with the visible columns.
			this.displayTable = ReportObject.dataSource.Tables["report"].Copy();

            for (int i = 0; i < displayTable.Columns.Count; i++)
            {
                displayTable.Columns[i].ColumnName = displayTable.Columns[i].ColumnName.ToUpper();
            }

            //foreach (DataColumn dc in dt_view.Columns)
            //{

            //    if (dbAccess.DataSet.Tables["wkc1"].Columns.IndexOf(dc.ColumnName) != -1)
            //    {
            //        wkc1tmp[dc.ColumnName] = row[dc.ColumnName];
            //    }
            //}
        

			this.txt_title.Text = ReportObject.title;
		}

        #region PivotGridInit

        private void addPivotField(string fieldName, string dataType, string caption)
		{
			PivotGridField field = new PivotGridField();
			field.Area = PivotArea.FilterArea;
			field.Name = "field" + fieldName;
			field.FieldName = fieldName;
			field.Caption = caption;

			//format datatype
			switch (dataType.ToUpper())
			{
                //case "DECIMAL":
                //    field.CellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                //    field.CellFormat.FormatString = "#,##0.00";
                //    break;
                case "SYSTEM.DECIMAL":
                    //Amended by Spencer
                    //field.CellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //field.CellFormat.FormatString = "#,##0.00";
                    //field.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //field.CellFormat.FormatString = "f2";
                    //pivotGridControl1.BestFit();
                    PivotGridFieldBase.DefaultDecimalFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    PivotGridFieldBase.DefaultDecimalFormat.FormatString = "N2";
                    break;
                case "DECIMAL":
                    //Amended by Spencer
                    //field.CellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //field.CellFormat.FormatString = "#,##0.00";
                    field.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    field.CellFormat.FormatString = "f2";
                    pivotGridControl1.BestFit();
                    break;
				case "DATETIME":
					field.CellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
					field.CellFormat.FormatString = "yyyy/mm/dd";
					break;
			}

			pivotGridControl1.Fields.Add(field);
			field.Options.AllowSort = DevExpress.Utils.DefaultBoolean.True;

			cmb_sumsort.Items.Add(fieldName);
		}

		#endregion

		private void customReportInit()
		{
			refreshReportTypes();

			// Default reporttype is the normal report
			cmb_reporttype.SelectedIndex = 0;
			changeType(cmb_reporttype.SelectedValue.ToString().ToLower());

			refreshLayoutList();
		}

		#region Layout Save/Load/Refresh

		private void btn_savelayout_Click(object sender, EventArgs e)
		{
			MemoryStream stream = saveLayoutToStream(cmb_reporttype.SelectedValue.ToString());

			if (BizFunctions.IsEmpty(txt_layout.Text.Trim()))
			{
				MessageBox.Show("Please enter a name to save as");
				txt_layout.Focus();
				return;
			}

			foreach (DataRowView drv in lsb_layoutlist.Items)
			{
				if (drv[lsb_layoutlist.ValueMember].ToString().Trim().ToLower() == txt_layout.Text.ToLower().Trim())
				{
					DialogResult result = MessageBox.Show("Overwrite this existing layout (" + txt_layout.Text.Trim() + "?", "Overwrite", MessageBoxButtons.YesNo);
					if (result == DialogResult.No)
					{
						return;
					}
				}
			}

			Byte[] bytes = stream.ToArray();

			BizRAD.DB.Interface.Parameter[] parameters = new BizRAD.DB.Interface.Parameter[4];
			parameters[0] = new BizRAD.DB.Interface.Parameter("@reportid", reportObject.reportid);
			parameters[1] = new BizRAD.DB.Interface.Parameter("@layoutname", txt_layout.Text.Trim());
			parameters[2] = new BizRAD.DB.Interface.Parameter("@reporttype", cmb_reporttype.SelectedValue.ToString());
			parameters[3] = new BizRAD.DB.Interface.Parameter("@data", bytes);
			parameters[3].SqlDbType = SqlDbType.Image;
			parameters[3].Size = bytes.Length;

			dbaccess.RemoteStandardSQL.GetStoredProcedureResult("sp_savecustomreportlayout", ref parameters);

			refreshLayoutList();
		}

		private void btn_loadlayout_Click(object sender, EventArgs e)
		{

			if (lsb_layoutlist.SelectedIndex == -1)
			{
				MessageBox.Show("Please select a layout from the list to load first!", "Select layout");
				return;
			}
			
			loadLayout(cmb_reporttype.SelectedValue.ToString());
		}

		private MemoryStream saveLayoutToStream(string reporttype)
		{
			MemoryStream stream = new MemoryStream();

			switch (reporttype.ToLower().Trim())
			{
				case "normal":
					gridControl1.MainView.SaveLayoutToStream(stream);
					break;

				case "pivot":
					pivotGridControl1.SaveLayoutToStream(stream);
					break;
			}

			return stream;
		}

		private void loadLayout(string reporttype)
		{
			MemoryStream stream = new MemoryStream();
			String layoutname = lsb_layoutlist.SelectedValue.ToString().Trim();
			DataSet dstmp = dbaccess.ReadSQLTemp("loadlayout", "select reporttype,layout from syscustomreportlayout where layoutname = '" + layoutname + "' and reportid ='" + reportObject.reportid + "' and reporttype='" + cmb_reporttype.SelectedValue.ToString().Trim() + "'");

			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write((Byte[])dstmp.Tables["loadlayout"].Rows[0]["layout"]);

			stream.Position = 0;

			switch (reporttype.ToLower().Trim())
			{
				case "normal":	
					gridControl1.MainView.RestoreLayoutFromStream(stream);
					break;

				case "pivot":
					pivotGridControl1.RestoreLayoutFromStream(stream);
					break;
			}


			writer.Close();

			txt_layout.Text = layoutname;
		}

		private void refreshLayoutList()
		{
			DataSet dstmp = dbaccess.ReadSQLTemp("layout", "select * from syscustomreportlayout where reportid = '" + reportObject.reportid + "' and reporttype ='" + cmb_reporttype.SelectedValue.ToString() + "'");
			this.layout = dstmp.Tables["layout"];

			lsb_layoutlist.DataSource = this.layout;
			lsb_layoutlist.DisplayMember = "layoutname";
			lsb_layoutlist.ValueMember = "layoutname";
		}

		#endregion

		#region Report Type Change

		private void refreshReportTypes()
		{
			DataSet dstmp = dbaccess.ReadSQLTemp("reportType", "select * from syscustomreporttype");
			this.reportType = dstmp.Tables["reportType"];

			cmb_reporttype.DataSource = this.reportType;
			cmb_reporttype.DisplayMember = "reportmode";
			cmb_reporttype.ValueMember = "reportmode";
		}

		private void cmb_reporttype_SelectedIndexChanged(object sender, EventArgs e)
		{
			changeType(cmb_reporttype.SelectedValue.ToString().ToLower());
			refreshLayoutList();
		}

		private void changeType(string reporttype)
		{
			enableMode(reporttype);

			switch (reporttype)
			{
				case "normal":

                    //displayTable.Columns[0].ColumnName = "TEST";
					gridControl1.DataSource = displayTable;
					gridControl1.Dock = DockStyle.Top;
                   

					break;

				case "pivot":

					pivotGridControl1.DataSource = displayTable;

					//pivotGridControl1.Fields.Clear();
					if (pivotGridControl1.Fields.Count == 0)
					{
						foreach (DataColumn dc in displayTable.Columns)
						{
							addPivotField(dc.ColumnName, dc.DataType.ToString(), dc.ColumnName);
						}
					}

                    //fieldCategoryID.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //fieldCategoryID.CellFormat.FormatString = "f2";

					pivotGridControl1.Dock = DockStyle.Top;
					
					break;


			}
		}

		private void enableMode(string mode)
		{
			// Fill in extra modes here.
			switch (mode)
			{
				case "normal":
					gridControl1.Enabled = true;
					gridControl1.Visible = true;
					pivotGridControl1.Enabled = false;
					pivotGridControl1.Visible = false;
					break;

				case "pivot":
					gridControl1.Enabled = false;
					gridControl1.Visible = false;
					pivotGridControl1.Enabled = true;
					pivotGridControl1.Visible = true;
					break;

				default:
					gridControl1.Enabled = false;
					gridControl1.Visible = false;
					pivotGridControl1.Enabled = false;
					pivotGridControl1.Visible = false;
					break;

			}
		}

		#endregion

		private void pivotGridControl1_CellDoubleClick(object sender, PivotCellEventArgs e)
		{
			Form form = new Form();
			form.Width = (int)((decimal)this.Width * (decimal)0.75);
			form.Height = (int)((decimal)this.Height * (decimal)0.75);
			DevExpress.XtraGrid.GridControl grid = new DevExpress.XtraGrid.GridControl();
			DevExpress.XtraGrid.Views.Grid.GridView gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();

			// Set up gridview
			gridView1.BestFitMaxRowCount = 1000;
			gridView1.GridControl = grid;
			gridView1.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
			gridView1.Name = "gridView1";
			gridView1.OptionsLayout.StoreAllOptions = true;
			gridView1.OptionsLayout.StoreAppearance = true;
			gridView1.OptionsView.ColumnAutoWidth = false;
			gridView1.OptionsView.ShowFooter = true;

			grid.MainView = gridView1;

			form.Controls.Add(grid);
			grid.Dock = DockStyle.Fill;
			grid.DataSource = e.CreateDrillDownDataSource();

			form.Show();
		}

		private void btn_print_Click(object sender, EventArgs e)
		{
			PrintableComponentLink printableComponentLink1  = new PrintableComponentLink();

			switch (cmb_reporttype.SelectedValue.ToString().ToLower().Trim())
			{
				case "normal":
				printableComponentLink1.Component = this.gridControl1;
					break;
		
				case "pivot":
				printableComponentLink1.Component = this.pivotGridControl1;
					break;

			}
			
			printableComponentLink1.PaperKind = System.Drawing.Printing.PaperKind.A4;
			printableComponentLink1.Landscape = this.chk_landscape.Checked;
			printableComponentLink1.CreateMarginalHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateMarginalHeaderArea);
			//steph - to add the header's text
			printableComponentLink1.CreateInnerPageHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateInnerPageHeaderArea);
			//printableComponentLink1.CreateReportHeaderArea += new CreateAreaEventHandler(printableComponentLink1_CreateReportHeaderArea);
			printableComponentLink1.PrintingSystem = new PrintingSystem();
			printableComponentLink1.CreateDocument();
			printableComponentLink1.ShowPreview();		
		}

		void printableComponentLink1_CreateInnerPageHeaderArea(object sender, CreateAreaEventArgs e)
		{
			PageInfoBrick brick = null;

			if (txt_layout.Text.Trim() != String.Empty)
			{
				brick = e.Graph.DrawPageInfo(PageInfo.None, txt_layout.Text.Trim() + "   Filters:  " + reportObject.filterList.ToString().Trim(), Color.Black,
				 new RectangleF(0, 0, 100, 20), BorderSide.Top);
			}
			else
			{
				brick = e.Graph.DrawPageInfo(PageInfo.None, this.txt_title.Text + "   Filters: " + reportObject.filterList.ToString().Trim(), Color.Black,
				 new RectangleF(0, 0, 100, 20), BorderSide.Top);
			}

			brick.LineAlignment = BrickAlignment.Near;
			brick.Alignment = BrickAlignment.Near;
			brick.AutoWidth = true;
		}


		void printableComponentLink1_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
		{
			//steph - display date time on the left
			PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.DateTime, "", Color.Black,
			new RectangleF(0, 0, 100, 20), BorderSide.None);
			brick.LineAlignment = BrickAlignment.Near;
			brick.Alignment = BrickAlignment.Near;
			brick.AutoWidth = true;

			//steph - display pages on the right
			PageInfoBrick brick2 = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, "", Color.Black,
			new RectangleF(0, 0, 130, 50), BorderSide.None);
			brick2.LineAlignment = BrickAlignment.Near;
			brick2.Alignment = BrickAlignment.Far;
			brick2.AutoWidth = true;

			//steph - display the report name
			PageInfoBrick brick3 = null;
			if (txt_layout.Text.Trim() != String.Empty)
			{
				brick3 = e.Graph.DrawPageInfo(PageInfo.None, txt_layout.Text.Trim()
					//+
					//"   Filters:  " + reportObject.filterList.ToString().Trim()
					, Color.Black,
				 new RectangleF(0, 0, 100, 20), BorderSide.None);
			}
			else
			{
				brick3 = e.Graph.DrawPageInfo(PageInfo.None, this.txt_title.Text
					//+
					//"   Filters: " + reportObject.filterList.ToString().Trim()  
					, Color.Black,
				 new RectangleF(0, 0, 100, 20), BorderSide.None);
			}
			brick3.LineAlignment = BrickAlignment.Center;
			brick3.Alignment = BrickAlignment.Center;
			brick3.AutoWidth = true;
							
		}

		//steph - function to add the header's text
		void printableComponentLink1_CreateReportHeaderArea(object sender, CreateAreaEventArgs e)
		{
			PageInfoBrick brick = null;

			if (txt_layout.Text.Trim() != String.Empty)
			{
				brick = e.Graph.DrawPageInfo(PageInfo.None, txt_layout.Text.Trim() + "   Filters:  " + reportObject.filterList.ToString().Trim(), Color.Black,
				 new RectangleF(0, 0, 100, 20), BorderSide.Top);
			}
			else
			{
				brick = e.Graph.DrawPageInfo(PageInfo.None, this.txt_title.Text +"   Filters: " + reportObject.filterList.ToString().Trim(), Color.Black,
				 new RectangleF(0, 0, 100, 20), BorderSide.Top);
			}

			brick.LineAlignment = BrickAlignment.Near;
			brick.Alignment = BrickAlignment.Near;
			brick.AutoWidth = true;
		}
	
		private void btn_deletelayout_Click(object sender, EventArgs e)
		{
			//dialog added by ivan 27/3/08
			DialogResult result = DialogResult.None;
			result = MessageBox.Show("Confirm Delete?", "Delete Layout", MessageBoxButtons.YesNo);
			if (result == DialogResult.No)
			{
				// return from the handle and don't continue
				return;
			}
			dbaccess.RemoteStandardSQL.ExecuteNonQuery("delete from syscustomreportlayout where reportid ='" + reportObject.reportid + "' and reporttype ='" + cmb_reporttype.SelectedValue.ToString().Trim() + "' and layoutname ='" + lsb_layoutlist.SelectedValue.ToString().Trim() + "'");
			refreshLayoutList();
		}

		private void cmb_sumsort_SelectedIndexChanged(object sender, EventArgs e)
		{
			//pivotGridControl1.Fields[cmb_sumsort.Items[cmb_sumsort.SelectedIndex].ToString()].SortBySummaryInfo
		}

		private void NormalCustom_FormClosed(object sender, FormClosedEventArgs e)
		{
			Environment.CurrentDirectory = currentEnvironment;
		}
	}

}