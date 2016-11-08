namespace ATL.CRPT
{
	partial class ReportTable
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportTable));
			this.dgv_reportTable = new System.Windows.Forms.DataGridView();
			this.txt_title = new System.Windows.Forms.TextBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btn_Print = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbtn_quickexport = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.dgv_reportTable)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgv_reportTable
			// 
			this.dgv_reportTable.AllowUserToAddRows = false;
			this.dgv_reportTable.AllowUserToDeleteRows = false;
			this.dgv_reportTable.AllowUserToOrderColumns = true;
			this.dgv_reportTable.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dgv_reportTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgv_reportTable.GridColor = System.Drawing.Color.White;
			this.dgv_reportTable.Location = new System.Drawing.Point(0, 28);
			this.dgv_reportTable.Name = "dgv_reportTable";
			this.dgv_reportTable.ReadOnly = true;
			this.dgv_reportTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgv_reportTable.Size = new System.Drawing.Size(880, 394);
			this.dgv_reportTable.TabIndex = 0;
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
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Print,
            this.toolStripSeparator1,
            this.tsbtn_quickexport});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(885, 25);
			this.toolStrip1.TabIndex = 3;
			this.toolStrip1.Text = "Te";
			// 
			// btn_Print
			// 
			this.btn_Print.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btn_Print.Image = ((System.Drawing.Image)(resources.GetObject("btn_Print.Image")));
			this.btn_Print.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_Print.Name = "btn_Print";
			this.btn_Print.Size = new System.Drawing.Size(43, 22);
			this.btn_Print.Text = "Export";
			this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// tsbtn_quickexport
			// 
			this.tsbtn_quickexport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsbtn_quickexport.Image = ((System.Drawing.Image)(resources.GetObject("tsbtn_quickexport.Image")));
			this.tsbtn_quickexport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtn_quickexport.Name = "tsbtn_quickexport";
			this.tsbtn_quickexport.Size = new System.Drawing.Size(72, 22);
			this.tsbtn_quickexport.Text = "Quick Export";
			this.tsbtn_quickexport.Click += new System.EventHandler(this.tsbtn_quickexport_Click);
			// 
			// ReportTable
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(885, 434);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.txt_title);
			this.Controls.Add(this.dgv_reportTable);
			this.Name = "ReportTable";
			this.Text = "ReportTable";
			this.Resize += new System.EventHandler(this.ReportTable_Resize);
			this.Load += new System.EventHandler(this.ReportTable_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgv_reportTable)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgv_reportTable;
		private System.Windows.Forms.TextBox txt_title;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton btn_Print;
		private System.Windows.Forms.ToolStripButton tsbtn_quickexport;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	}
}