namespace ATL.BizModules.Tools
{
    partial class CRForm
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
            this.crViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crViewer1
            // 
            this.crViewer1.ActiveViewIndex = -1;
            this.crViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crViewer1.Location = new System.Drawing.Point(0, 0);
            this.crViewer1.Name = "crViewer1";
            this.crViewer1.SelectionFormula = "";
            this.crViewer1.Size = new System.Drawing.Size(780, 596);
            this.crViewer1.TabIndex = 0;
            this.crViewer1.ViewTimeSelectionFormula = "";
            this.crViewer1.Load += new System.EventHandler(this.crViewer1_Load);
            // 
            // CRForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 596);
            this.Controls.Add(this.crViewer1);
            this.Name = "CRForm";
            this.Text = "Report";
            this.Load += new System.EventHandler(this.CRForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crViewer1;
    }
}