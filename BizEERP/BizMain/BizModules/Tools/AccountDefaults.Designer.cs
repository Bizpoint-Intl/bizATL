namespace ATL.AccDefaults
{
	partial class AccountDefaults
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dg_accDef = new System.Windows.Forms.DataGridView();
			this.cmdUpdate = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.flag = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.page = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.field = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dg_accDef)).BeginInit();
			this.SuspendLayout();
			// 
			// dg_accDef
			// 
			this.dg_accDef.AllowUserToAddRows = false;
			this.dg_accDef.AllowUserToDeleteRows = false;
			this.dg_accDef.AllowUserToOrderColumns = true;
			this.dg_accDef.AllowUserToResizeColumns = false;
			this.dg_accDef.AllowUserToResizeRows = false;
			this.dg_accDef.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dg_accDef.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.flag,
            this.page,
            this.field,
            this.value});
			this.dg_accDef.Location = new System.Drawing.Point(12, 12);
			this.dg_accDef.Name = "dg_accDef";
			this.dg_accDef.Size = new System.Drawing.Size(570, 323);
			this.dg_accDef.TabIndex = 0;
			// 
			// cmdUpdate
			// 
			this.cmdUpdate.Location = new System.Drawing.Point(426, 341);
			this.cmdUpdate.Name = "cmdUpdate";
			this.cmdUpdate.Size = new System.Drawing.Size(75, 23);
			this.cmdUpdate.TabIndex = 1;
			this.cmdUpdate.Text = "Update";
			this.cmdUpdate.UseVisualStyleBackColor = true;
			this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Location = new System.Drawing.Point(507, 341);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(75, 23);
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// flag
			// 
			this.flag.DataPropertyName = "flag";
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
			this.flag.DefaultCellStyle = dataGridViewCellStyle1;
			this.flag.HeaderText = "Module";
			this.flag.Name = "flag";
			this.flag.ReadOnly = true;
			// 
			// page
			// 
			this.page.DataPropertyName = "page";
			dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightCyan;
			this.page.DefaultCellStyle = dataGridViewCellStyle2;
			this.page.HeaderText = "Page";
			this.page.Name = "page";
			this.page.ReadOnly = true;
			// 
			// field
			// 
			this.field.DataPropertyName = "field";
			dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightCyan;
			this.field.DefaultCellStyle = dataGridViewCellStyle3;
			this.field.HeaderText = "Field";
			this.field.Name = "field";
			this.field.ReadOnly = true;
			this.field.Width = 150;
			// 
			// value
			// 
			this.value.DataPropertyName = "value";
			this.value.HeaderText = "Value";
			this.value.Name = "value";
			this.value.Width = 150;
			// 
			// AccountDefaults
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(594, 376);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdUpdate);
			this.Controls.Add(this.dg_accDef);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AccountDefaults";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Account Defaults";
			this.Load += new System.EventHandler(this.AccountDefaults_Load);
			((System.ComponentModel.ISupportInitialize)(this.dg_accDef)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dg_accDef;
		private System.Windows.Forms.Button cmdUpdate;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.DataGridViewTextBoxColumn flag;
		private System.Windows.Forms.DataGridViewTextBoxColumn page;
		private System.Windows.Forms.DataGridViewTextBoxColumn field;
		private System.Windows.Forms.DataGridViewTextBoxColumn value;
	}
}