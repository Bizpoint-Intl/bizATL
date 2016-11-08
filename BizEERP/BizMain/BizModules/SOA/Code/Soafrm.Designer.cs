namespace ATL.SOA
{
    partial class Soafrm
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
			this.statementpreview = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
			this.btnQuit = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lbl_description = new System.Windows.Forms.Label();
			//this.StatementOfAccounts = new PicoGuards.BizModules.SOA.Report.StatementOfAccounts();
            this.StatementOfAccounts_Monthly = new ATL.BizModules.SOA.Report.StatementOfAccounts_Monthly();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statementpreview
			// 
			this.statementpreview.ActiveViewIndex = 0;
			this.statementpreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.statementpreview.DisplayGroupTree = false;
			this.statementpreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.statementpreview.Location = new System.Drawing.Point(0, 25);
			this.statementpreview.Name = "statementpreview";
			this.statementpreview.ReportSource = this.StatementOfAccounts_Monthly;
			this.statementpreview.ShowGroupTreeButton = false;
			this.statementpreview.Size = new System.Drawing.Size(672, 412);
			this.statementpreview.TabIndex = 0;
			// 
			// btnQuit
			// 
			this.btnQuit.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnQuit.Location = new System.Drawing.Point(597, 0);
			this.btnQuit.Name = "btnQuit";
			this.btnQuit.Size = new System.Drawing.Size(75, 25);
			this.btnQuit.TabIndex = 1;
			this.btnQuit.Text = "Quit";
			this.btnQuit.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.lbl_description);
			this.panel1.Controls.Add(this.btnQuit);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(672, 25);
			this.panel1.TabIndex = 2;
			// 
			// lbl_description
			// 
			this.lbl_description.AutoSize = true;
			this.lbl_description.Location = new System.Drawing.Point(12, 6);
			this.lbl_description.Name = "lbl_description";
			this.lbl_description.Size = new System.Drawing.Size(35, 13);
			this.lbl_description.TabIndex = 2;
			this.lbl_description.Text = "label1";
			// 
			// Soafrm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(672, 437);
			this.Controls.Add(this.statementpreview);
			this.Controls.Add(this.panel1);
			this.Name = "Soafrm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Statement Preview";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer statementpreview;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_description;
        private ATL.BizModules.SOA.Report.StatementOfAccounts_Monthly StatementOfAccounts_Monthly;
		//private PicoGuards.BizModules.SOA.Report.StatementOfAccounts StatementOfAccounts;
    }
}