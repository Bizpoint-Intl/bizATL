partial class ExtractGrid
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractGrid));
		this.dg_extractionGrid = new System.Windows.Forms.DataGridView();
		this.btn_extract = new System.Windows.Forms.Button();
		this.btn_cancel = new System.Windows.Forms.Button();
		this.lbx_filterbox = new System.Windows.Forms.ListBox();
		this.txt_filterText = new System.Windows.Forms.TextBox();
		this.btn_addFilter = new System.Windows.Forms.Button();
		this.cmb_columnFilter = new System.Windows.Forms.ComboBox();
		this.btn_removeFilter = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.dtp_datetime = new System.Windows.Forms.DateTimePicker();
		this.cmb_filterOperator = new System.Windows.Forms.ComboBox();
		this.txt_lines = new System.Windows.Forms.TextBox();
		((System.ComponentModel.ISupportInitialize)(this.dg_extractionGrid)).BeginInit();
		this.groupBox1.SuspendLayout();
		this.SuspendLayout();
		// 
		// dg_extractionGrid
		// 
		this.dg_extractionGrid.AllowUserToAddRows = false;
		this.dg_extractionGrid.AllowUserToDeleteRows = false;
		this.dg_extractionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dg_extractionGrid.Location = new System.Drawing.Point(12, 129);
		this.dg_extractionGrid.Name = "dg_extractionGrid";
		this.dg_extractionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dg_extractionGrid.Size = new System.Drawing.Size(903, 386);
		this.dg_extractionGrid.TabIndex = 0;
		// 
		// btn_extract
		// 
		this.btn_extract.Location = new System.Drawing.Point(720, 32);
		this.btn_extract.Name = "btn_extract";
		this.btn_extract.Size = new System.Drawing.Size(95, 30);
		this.btn_extract.TabIndex = 1;
		this.btn_extract.Text = "Extract";
		this.btn_extract.UseVisualStyleBackColor = true;
		this.btn_extract.Click += new System.EventHandler(this.btn_extract_Click);
		// 
		// btn_cancel
		// 
		this.btn_cancel.Location = new System.Drawing.Point(720, 77);
		this.btn_cancel.Name = "btn_cancel";
		this.btn_cancel.Size = new System.Drawing.Size(95, 30);
		this.btn_cancel.TabIndex = 2;
		this.btn_cancel.Text = "Cancel";
		this.btn_cancel.UseVisualStyleBackColor = true;
		this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
		// 
		// lbx_filterbox
		// 
		this.lbx_filterbox.DisplayMember = "Display";
		this.lbx_filterbox.FormattingEnabled = true;
		this.lbx_filterbox.Location = new System.Drawing.Point(370, 20);
		this.lbx_filterbox.Name = "lbx_filterbox";
		this.lbx_filterbox.Size = new System.Drawing.Size(234, 69);
		this.lbx_filterbox.TabIndex = 3;
		this.lbx_filterbox.ValueMember = "Value";
		// 
		// txt_filterText
		// 
		this.txt_filterText.Location = new System.Drawing.Point(34, 74);
		this.txt_filterText.Name = "txt_filterText";
		this.txt_filterText.Size = new System.Drawing.Size(179, 20);
		this.txt_filterText.TabIndex = 4;
		this.txt_filterText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_filterText_KeyDown);
		// 
		// btn_addFilter
		// 
		this.btn_addFilter.Location = new System.Drawing.Point(243, 20);
		this.btn_addFilter.Name = "btn_addFilter";
		this.btn_addFilter.Size = new System.Drawing.Size(75, 23);
		this.btn_addFilter.TabIndex = 5;
		this.btn_addFilter.Text = "Add Filter =>";
		this.btn_addFilter.UseVisualStyleBackColor = true;
		this.btn_addFilter.Click += new System.EventHandler(this.btn_addFilter_Click);
		// 
		// cmb_columnFilter
		// 
		this.cmb_columnFilter.DisplayMember = "Display";
		this.cmb_columnFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmb_columnFilter.FormattingEnabled = true;
		this.cmb_columnFilter.Location = new System.Drawing.Point(34, 20);
		this.cmb_columnFilter.Name = "cmb_columnFilter";
		this.cmb_columnFilter.Size = new System.Drawing.Size(179, 21);
		this.cmb_columnFilter.TabIndex = 6;
		this.cmb_columnFilter.ValueMember = "Value";
		this.cmb_columnFilter.SelectionChangeCommitted += new System.EventHandler(this.cmb_columnFilter_SelectionChangeCommitted);
		// 
		// btn_removeFilter
		// 
		this.btn_removeFilter.Location = new System.Drawing.Point(243, 62);
		this.btn_removeFilter.Name = "btn_removeFilter";
		this.btn_removeFilter.Size = new System.Drawing.Size(75, 23);
		this.btn_removeFilter.TabIndex = 7;
		this.btn_removeFilter.Text = "<= Remove";
		this.btn_removeFilter.UseVisualStyleBackColor = true;
		this.btn_removeFilter.Click += new System.EventHandler(this.btn_removeFilter_Click);
		// 
		// groupBox1
		// 
		this.groupBox1.Controls.Add(this.dtp_datetime);
		this.groupBox1.Controls.Add(this.cmb_filterOperator);
		this.groupBox1.Controls.Add(this.btn_removeFilter);
		this.groupBox1.Controls.Add(this.cmb_columnFilter);
		this.groupBox1.Controls.Add(this.btn_addFilter);
		this.groupBox1.Controls.Add(this.txt_filterText);
		this.groupBox1.Controls.Add(this.lbx_filterbox);
		this.groupBox1.Location = new System.Drawing.Point(12, 12);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(621, 110);
		this.groupBox1.TabIndex = 8;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Filters";
		// 
		// dtp_datetime
		// 
		this.dtp_datetime.CustomFormat = "dd-MMMM-yyyy";
		this.dtp_datetime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
		this.dtp_datetime.Location = new System.Drawing.Point(34, 75);
		this.dtp_datetime.Name = "dtp_datetime";
		this.dtp_datetime.Size = new System.Drawing.Size(179, 20);
		this.dtp_datetime.TabIndex = 9;
		this.dtp_datetime.Visible = false;
		// 
		// cmb_filterOperator
		// 
		this.cmb_filterOperator.DisplayMember = "Display";
		this.cmb_filterOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmb_filterOperator.FormattingEnabled = true;
		this.cmb_filterOperator.Location = new System.Drawing.Point(34, 47);
		this.cmb_filterOperator.Name = "cmb_filterOperator";
		this.cmb_filterOperator.Size = new System.Drawing.Size(179, 21);
		this.cmb_filterOperator.TabIndex = 8;
		this.cmb_filterOperator.ValueMember = "Value";
		// 
		// txt_lines
		// 
		this.txt_lines.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.txt_lines.Location = new System.Drawing.Point(744, 521);
		this.txt_lines.Name = "txt_lines";
		this.txt_lines.ReadOnly = true;
		this.txt_lines.Size = new System.Drawing.Size(154, 13);
		this.txt_lines.TabIndex = 9;
		this.txt_lines.Text = "Lines";
		this.txt_lines.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
		// 
		// ExtractGrid
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.WhiteSmoke;
		this.ClientSize = new System.Drawing.Size(922, 543);
		this.ControlBox = false;
		this.Controls.Add(this.txt_lines);
		this.Controls.Add(this.groupBox1);
		this.Controls.Add(this.btn_cancel);
		this.Controls.Add(this.btn_extract);
		this.Controls.Add(this.dg_extractionGrid);
		this.Cursor = System.Windows.Forms.Cursors.Arrow;
		this.DoubleBuffered = true;
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
		this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "ExtractGrid";
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Load += new System.EventHandler(this.ExtractGrid_Load);
		((System.ComponentModel.ISupportInitialize)(this.dg_extractionGrid)).EndInit();
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.DataGridView dg_extractionGrid;
	private System.Windows.Forms.Button btn_extract;
	private System.Windows.Forms.Button btn_cancel;
	private System.Windows.Forms.ListBox lbx_filterbox;
	private System.Windows.Forms.TextBox txt_filterText;
	private System.Windows.Forms.Button btn_addFilter;
	private System.Windows.Forms.ComboBox cmb_columnFilter;
	private System.Windows.Forms.Button btn_removeFilter;
	private System.Windows.Forms.GroupBox groupBox1;
	private System.Windows.Forms.ComboBox cmb_filterOperator;
	private System.Windows.Forms.DateTimePicker dtp_datetime;
	private System.Windows.Forms.TextBox txt_lines;
}