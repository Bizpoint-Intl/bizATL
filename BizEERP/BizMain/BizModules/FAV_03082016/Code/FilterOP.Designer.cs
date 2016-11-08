namespace ATL.FilterOP
{
	partial class getFilterOP
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
			this.btnExtract = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.CodeFrom = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.CodeTo = new System.Windows.Forms.TextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnExtract
			// 
			this.btnExtract.Location = new System.Drawing.Point(121, 127);
			this.btnExtract.Name = "btnExtract";
			this.btnExtract.Size = new System.Drawing.Size(126, 23);
			this.btnExtract.TabIndex = 0;
			this.btnExtract.Text = "Extract Opening Now!";
			this.btnExtract.UseVisualStyleBackColor = true;
			this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "A/R Code";
			// 
			// CodeFrom
			// 
			this.CodeFrom.Location = new System.Drawing.Point(121, 31);
			this.CodeFrom.Name = "CodeFrom";
			this.CodeFrom.Size = new System.Drawing.Size(157, 20);
			this.CodeFrom.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(85, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "From";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(282, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(20, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "To";
			// 
			// CodeTo
			// 
			this.CodeTo.Location = new System.Drawing.Point(308, 31);
			this.CodeTo.Name = "CodeTo";
			this.CodeTo.Size = new System.Drawing.Size(157, 20);
			this.CodeTo.TabIndex = 5;
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(285, 127);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(106, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// getFilterOP
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(472, 162);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.CodeTo);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.CodeFrom);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnExtract);
			this.Name = "getFilterOP";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FilterOP";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnExtract;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox CodeFrom;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox CodeTo;
		private System.Windows.Forms.Button btnCancel;
	}
}