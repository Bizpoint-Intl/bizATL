namespace BizRAD.BizAccounts
{
    partial class StandardReports
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
            this.lb_Reports = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.lbl_description = new System.Windows.Forms.Label();
            this.grb_Reports = new System.Windows.Forms.GroupBox();
            this.dtp_CutOffDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.grb_Reports.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_Reports
            // 
            this.lb_Reports.FormattingEnabled = true;
            this.lb_Reports.HorizontalScrollbar = true;
            this.lb_Reports.Location = new System.Drawing.Point(12, 12);
            this.lb_Reports.Name = "lb_Reports";
            this.lb_Reports.Size = new System.Drawing.Size(336, 355);
            this.lb_Reports.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(516, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(354, 12);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 1;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // lbl_description
            // 
            this.lbl_description.Location = new System.Drawing.Point(10, 16);
            this.lbl_description.Name = "lbl_description";
            this.lbl_description.Size = new System.Drawing.Size(221, 191);
            this.lbl_description.TabIndex = 12;
            this.lbl_description.Text = "No reports available for selection. Please contact your software vendor. Have a n" +
                "ice day :-)";
            // 
            // grb_Reports
            // 
            this.grb_Reports.Controls.Add(this.lbl_description);
            this.grb_Reports.Location = new System.Drawing.Point(354, 157);
            this.grb_Reports.Name = "grb_Reports";
            this.grb_Reports.Size = new System.Drawing.Size(237, 210);
            this.grb_Reports.TabIndex = 11;
            this.grb_Reports.TabStop = false;
            this.grb_Reports.Text = "Report Description";
            // 
            // dtp_CutOffDate
            // 
            this.dtp_CutOffDate.CustomFormat = "dd-MM-yyyy";
            this.dtp_CutOffDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_CutOffDate.Location = new System.Drawing.Point(129, 15);
            this.dtp_CutOffDate.Name = "dtp_CutOffDate";
            this.dtp_CutOffDate.Size = new System.Drawing.Size(102, 20);
            this.dtp_CutOffDate.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtp_CutOffDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(354, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(237, 44);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 18);
            this.label1.TabIndex = 9;
            this.label1.Text = "Report Cut Off Date";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(435, 12);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // StandardReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(603, 383);
            this.ControlBox = false;
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.grb_Reports);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lb_Reports);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StandardReports";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reports";
            this.Load += new System.EventHandler(this.StandardReports_Load);
            this.grb_Reports.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lb_Reports;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Label lbl_description;
        private System.Windows.Forms.GroupBox grb_Reports;
        private System.Windows.Forms.DateTimePicker dtp_CutOffDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPrint;
    }
}