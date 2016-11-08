namespace ATL.CRPT
{
	partial class NormalCustom
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txt_title = new System.Windows.Forms.TextBox();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.txt_layout = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btn_loadlayout = new DevExpress.XtraEditors.SimpleButton();
			this.btn_savelayout = new DevExpress.XtraEditors.SimpleButton();
			this.pivotGridControl1 = new DevExpress.XtraPivotGrid.PivotGridControl();
			this.label2 = new System.Windows.Forms.Label();
			this.cmb_reporttype = new System.Windows.Forms.ComboBox();
			this.lsb_layoutlist = new System.Windows.Forms.ListBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.chk_landscape = new System.Windows.Forms.CheckBox();
			this.lbl_sumsort = new System.Windows.Forms.Label();
			this.cmb_sumsort = new System.Windows.Forms.ComboBox();
			this.btn_deletelayout = new DevExpress.XtraEditors.SimpleButton();
			this.btn_print = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txt_title
			// 
			this.txt_title.BackColor = System.Drawing.Color.WhiteSmoke;
			this.txt_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txt_title.Location = new System.Drawing.Point(13, 12);
			this.txt_title.Name = "txt_title";
			this.txt_title.Size = new System.Drawing.Size(665, 13);
			this.txt_title.TabIndex = 1;
			// 
			// gridControl1
			// 
			// 
			// 
			// 
			this.gridControl1.EmbeddedNavigator.Name = "";
			this.gridControl1.Location = new System.Drawing.Point(0, 0);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(946, 473);
			this.gridControl1.TabIndex = 2;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			// 
			// gridView1
			// 
			this.gridView1.BestFitMaxRowCount = 1000;
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsBehavior.Editable = false;
			this.gridView1.OptionsLayout.StoreAllOptions = true;
			this.gridView1.OptionsLayout.StoreAppearance = true;
			this.gridView1.OptionsView.ColumnAutoWidth = false;
			this.gridView1.OptionsView.ShowFooter = true;
			// 
			// txt_layout
			// 
			this.txt_layout.Location = new System.Drawing.Point(91, 38);
			this.txt_layout.Name = "txt_layout";
			this.txt_layout.Size = new System.Drawing.Size(147, 20);
			this.txt_layout.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Layout Name";
			// 
			// btn_loadlayout
			// 
			this.btn_loadlayout.Location = new System.Drawing.Point(573, 35);
			this.btn_loadlayout.Name = "btn_loadlayout";
			this.btn_loadlayout.Size = new System.Drawing.Size(128, 23);
			this.btn_loadlayout.TabIndex = 8;
			this.btn_loadlayout.Text = "Load Layout";
			this.btn_loadlayout.Click += new System.EventHandler(this.btn_loadlayout_Click);
			// 
			// btn_savelayout
			// 
			this.btn_savelayout.Location = new System.Drawing.Point(253, 35);
			this.btn_savelayout.Name = "btn_savelayout";
			this.btn_savelayout.Size = new System.Drawing.Size(75, 23);
			this.btn_savelayout.TabIndex = 9;
			this.btn_savelayout.Text = "Save Layout";
			this.btn_savelayout.Click += new System.EventHandler(this.btn_savelayout_Click);
			// 
			// pivotGridControl1
			// 
			this.pivotGridControl1.Cursor = System.Windows.Forms.Cursors.Default;
			this.pivotGridControl1.Location = new System.Drawing.Point(0, 0);
			this.pivotGridControl1.Name = "pivotGridControl1";
			this.pivotGridControl1.OptionsLayout.Columns.StoreAllOptions = true;
			this.pivotGridControl1.OptionsLayout.Columns.StoreAppearance = true;
			this.pivotGridControl1.OptionsLayout.StoreAllOptions = true;
			this.pivotGridControl1.OptionsLayout.StoreAppearance = true;
			this.pivotGridControl1.Size = new System.Drawing.Size(946, 473);
			this.pivotGridControl1.TabIndex = 12;
			this.pivotGridControl1.CellDoubleClick += new DevExpress.XtraPivotGrid.PivotCellEventHandler(this.pivotGridControl1_CellDoubleClick);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 13;
			this.label2.Text = "Report Type";
			// 
			// cmb_reporttype
			// 
			this.cmb_reporttype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmb_reporttype.FormattingEnabled = true;
			this.cmb_reporttype.Location = new System.Drawing.Point(88, 7);
			this.cmb_reporttype.Name = "cmb_reporttype";
			this.cmb_reporttype.Size = new System.Drawing.Size(150, 21);
			this.cmb_reporttype.TabIndex = 14;
			this.cmb_reporttype.SelectedIndexChanged += new System.EventHandler(this.cmb_reporttype_SelectedIndexChanged);
			// 
			// lsb_layoutlist
			// 
			this.lsb_layoutlist.FormattingEnabled = true;
			this.lsb_layoutlist.Location = new System.Drawing.Point(361, 6);
			this.lsb_layoutlist.Name = "lsb_layoutlist";
			this.lsb_layoutlist.Size = new System.Drawing.Size(205, 56);
			this.lsb_layoutlist.TabIndex = 15;
			// 
			// panel1
			// 
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.chk_landscape);
			this.panel1.Controls.Add(this.lbl_sumsort);
			this.panel1.Controls.Add(this.cmb_sumsort);
			this.panel1.Controls.Add(this.btn_deletelayout);
			this.panel1.Controls.Add(this.btn_print);
			this.panel1.Controls.Add(this.lsb_layoutlist);
			this.panel1.Controls.Add(this.cmb_reporttype);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.btn_savelayout);
			this.panel1.Controls.Add(this.btn_loadlayout);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.txt_layout);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 479);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(946, 70);
			this.panel1.TabIndex = 16;
			// 
			// chk_landscape
			// 
			this.chk_landscape.AutoSize = true;
			this.chk_landscape.Location = new System.Drawing.Point(736, 14);
			this.chk_landscape.Name = "chk_landscape";
			this.chk_landscape.Size = new System.Drawing.Size(79, 17);
			this.chk_landscape.TabIndex = 20;
			this.chk_landscape.Text = "Landscape";
			this.chk_landscape.UseVisualStyleBackColor = true;
			// 
			// lbl_sumsort
			// 
			this.lbl_sumsort.AutoSize = true;
			this.lbl_sumsort.Location = new System.Drawing.Point(717, 15);
			this.lbl_sumsort.Name = "lbl_sumsort";
			this.lbl_sumsort.Size = new System.Drawing.Size(70, 13);
			this.lbl_sumsort.TabIndex = 19;
			this.lbl_sumsort.Text = "Data Sort By:";
			this.lbl_sumsort.Visible = false;
			// 
			// cmb_sumsort
			// 
			this.cmb_sumsort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmb_sumsort.FormattingEnabled = true;
			this.cmb_sumsort.Location = new System.Drawing.Point(717, 31);
			this.cmb_sumsort.Name = "cmb_sumsort";
			this.cmb_sumsort.Size = new System.Drawing.Size(99, 21);
			this.cmb_sumsort.TabIndex = 18;
			this.cmb_sumsort.Visible = false;
			this.cmb_sumsort.SelectedIndexChanged += new System.EventHandler(this.cmb_sumsort_SelectedIndexChanged);
			// 
			// btn_deletelayout
			// 
			this.btn_deletelayout.Location = new System.Drawing.Point(573, 7);
			this.btn_deletelayout.Name = "btn_deletelayout";
			this.btn_deletelayout.Size = new System.Drawing.Size(128, 25);
			this.btn_deletelayout.TabIndex = 17;
			this.btn_deletelayout.Text = "Delete Layout";
			this.btn_deletelayout.Click += new System.EventHandler(this.btn_deletelayout_Click);
			// 
			// btn_print
			// 
			this.btn_print.Location = new System.Drawing.Point(833, 10);
			this.btn_print.Name = "btn_print";
			this.btn_print.Size = new System.Drawing.Size(101, 49);
			this.btn_print.TabIndex = 16;
			this.btn_print.Text = "Print";
			this.btn_print.Click += new System.EventHandler(this.btn_print_Click);
			// 
			// NormalCustom
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(946, 549);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pivotGridControl1);
			this.Controls.Add(this.gridControl1);
			this.Controls.Add(this.txt_title);
			this.Name = "NormalCustom";
			this.Text = "Custom Report";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NormalCustom_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txt_title;
		private DevExpress.XtraGrid.GridControl gridControl1;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
		private System.Windows.Forms.TextBox txt_layout;
		private System.Windows.Forms.Label label1;
		private DevExpress.XtraEditors.SimpleButton btn_loadlayout;
		private DevExpress.XtraEditors.SimpleButton btn_savelayout;
		private DevExpress.XtraPivotGrid.PivotGridControl pivotGridControl1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cmb_reporttype;
		private System.Windows.Forms.ListBox lsb_layoutlist;
		private System.Windows.Forms.Panel panel1;
		private DevExpress.XtraEditors.SimpleButton btn_print;
		private DevExpress.XtraEditors.SimpleButton btn_deletelayout;
		private System.Windows.Forms.ComboBox cmb_sumsort;
		private System.Windows.Forms.Label lbl_sumsort;
		private System.Windows.Forms.CheckBox chk_landscape;
	}
}