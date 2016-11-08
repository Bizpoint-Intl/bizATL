namespace ATL.GSTSP
{
	partial class GSTSP
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GSTSP));
			this.btn_save = new System.Windows.Forms.Button();
			this.btn_cancel = new System.Windows.Forms.Button();
			this.dg_gstsp = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.dg_gstsp)).BeginInit();
			this.SuspendLayout();
			// 
			// btn_save
			// 
			this.btn_save.Location = new System.Drawing.Point(235, 9);
			this.btn_save.Name = "btn_save";
			this.btn_save.Size = new System.Drawing.Size(75, 23);
			this.btn_save.TabIndex = 0;
			this.btn_save.Text = "Save";
			this.btn_save.UseVisualStyleBackColor = true;
			this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
			// 
			// btn_cancel
			// 
			this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_cancel.Location = new System.Drawing.Point(316, 9);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new System.Drawing.Size(75, 23);
			this.btn_cancel.TabIndex = 1;
			this.btn_cancel.Text = "Cancel";
			this.btn_cancel.UseVisualStyleBackColor = true;
			this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
			// 
			// dg_gstsp
			// 
			this.dg_gstsp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dg_gstsp.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dg_gstsp.Location = new System.Drawing.Point(0, 38);
			this.dg_gstsp.Name = "dg_gstsp";
			this.dg_gstsp.RowTemplate.Height = 24;
			this.dg_gstsp.Size = new System.Drawing.Size(403, 262);
			this.dg_gstsp.TabIndex = 7;
			// 
			// GSTSP
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CancelButton = this.btn_cancel;
			this.ClientSize = new System.Drawing.Size(403, 300);
			this.ControlBox = false;
			this.Controls.Add(this.dg_gstsp);
			this.Controls.Add(this.btn_cancel);
			this.Controls.Add(this.btn_save);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GSTSP";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Modify System Tax Rates Wizard";
			this.Load += new System.EventHandler(this.GSTSP_Load);
			((System.ComponentModel.ISupportInitialize)(this.dg_gstsp)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btn_save;
		private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.DataGridView dg_gstsp;
	}
}