namespace ATL.TRA
{
	partial class queryoutlets
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
			this.dgv_outlet = new System.Windows.Forms.DataGridView();
			this.label1 = new System.Windows.Forms.Label();
			this.cmb_outlets = new System.Windows.Forms.ComboBox();
			this.progbar = new System.Windows.Forms.ProgressBar();
			this.txt_status = new System.Windows.Forms.TextBox();
			this.cmb_database = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.txt_total = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dgv_outlet)).BeginInit();
			this.SuspendLayout();
			// 
			// dgv_outlet
			// 
			this.dgv_outlet.AllowUserToAddRows = false;
			this.dgv_outlet.AllowUserToDeleteRows = false;
			this.dgv_outlet.AllowUserToOrderColumns = true;
			this.dgv_outlet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgv_outlet.Location = new System.Drawing.Point(12, 43);
			this.dgv_outlet.Name = "dgv_outlet";
			this.dgv_outlet.ReadOnly = true;
			this.dgv_outlet.Size = new System.Drawing.Size(900, 295);
			this.dgv_outlet.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Outlet";
			// 
			// cmb_outlets
			// 
			this.cmb_outlets.DisplayMember = "Display";
			this.cmb_outlets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmb_outlets.FormattingEnabled = true;
			this.cmb_outlets.Location = new System.Drawing.Point(63, 16);
			this.cmb_outlets.Name = "cmb_outlets";
			this.cmb_outlets.Size = new System.Drawing.Size(121, 21);
			this.cmb_outlets.Sorted = true;
			this.cmb_outlets.TabIndex = 2;
			this.cmb_outlets.ValueMember = "Value";
			this.cmb_outlets.SelectionChangeCommitted += new System.EventHandler(this.cmb_outlets_SelectionChangeCommitted);
			// 
			// progbar
			// 
			this.progbar.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.progbar.ForeColor = System.Drawing.Color.Blue;
			this.progbar.Location = new System.Drawing.Point(12, 344);
			this.progbar.MarqueeAnimationSpeed = 50;
			this.progbar.Name = "progbar";
			this.progbar.Size = new System.Drawing.Size(224, 15);
			this.progbar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progbar.TabIndex = 3;
			this.progbar.Visible = false;
			// 
			// txt_status
			// 
			this.txt_status.BackColor = System.Drawing.Color.WhiteSmoke;
			this.txt_status.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txt_status.Location = new System.Drawing.Point(213, 19);
			this.txt_status.Name = "txt_status";
			this.txt_status.Size = new System.Drawing.Size(156, 13);
			this.txt_status.TabIndex = 4;
			// 
			// cmb_database
			// 
			this.cmb_database.FormattingEnabled = true;
			this.cmb_database.Location = new System.Drawing.Point(323, 16);
			this.cmb_database.Name = "cmb_database";
			this.cmb_database.Size = new System.Drawing.Size(136, 21);
			this.cmb_database.TabIndex = 5;
			this.cmb_database.SelectionChangeCommitted += new System.EventHandler(this.cmb_database_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(264, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Database";
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(744, 344);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(28, 13);
			this.textBox1.TabIndex = 7;
			this.textBox1.Text = "Total";
			// 
			// txt_total
			// 
			this.txt_total.BackColor = System.Drawing.Color.WhiteSmoke;
			this.txt_total.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txt_total.Location = new System.Drawing.Point(778, 344);
			this.txt_total.Name = "txt_total";
			this.txt_total.Size = new System.Drawing.Size(111, 13);
			this.txt_total.TabIndex = 8;
			// 
			// queryoutlets
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(924, 365);
			this.Controls.Add(this.txt_total);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cmb_database);
			this.Controls.Add(this.txt_status);
			this.Controls.Add(this.progbar);
			this.Controls.Add(this.cmb_outlets);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dgv_outlet);
			this.Name = "queryoutlets";
			this.Text = "Query Outlet Stocks";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.queryoutlets_FormClosed);
			this.Load += new System.EventHandler(this.queryoutlets_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgv_outlet)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgv_outlet;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmb_outlets;
		private System.Windows.Forms.ProgressBar progbar;
		private System.Windows.Forms.TextBox txt_status;
		private System.Windows.Forms.ComboBox cmb_database;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox txt_total;
	}
}