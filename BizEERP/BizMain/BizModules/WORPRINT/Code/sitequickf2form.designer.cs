namespace ATL.WOR
{
	partial class sitequickf2form
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
            this.dgv_trqf2 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_trqf2)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_trqf2
            // 
            this.dgv_trqf2.AllowUserToAddRows = false;
            this.dgv_trqf2.AllowUserToDeleteRows = false;
            this.dgv_trqf2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_trqf2.Location = new System.Drawing.Point(12, 12);
            this.dgv_trqf2.Name = "dgv_trqf2";
            this.dgv_trqf2.ReadOnly = true;
            this.dgv_trqf2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_trqf2.Size = new System.Drawing.Size(376, 330);
            this.dgv_trqf2.TabIndex = 0;
            // 
            // sitequickf2form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(400, 354);
            this.Controls.Add(this.dgv_trqf2);
            this.Name = "sitequickf2form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select TRA";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_trqf2)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgv_trqf2;
	}
}