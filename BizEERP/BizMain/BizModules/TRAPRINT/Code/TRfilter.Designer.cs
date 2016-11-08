namespace ATL.TR
{
    partial class TRfilter
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
            this.cb_AllLocation = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_SiteTo = new System.Windows.Forms.TextBox();
            this.txt_SiteFrom = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.btn_Preview = new System.Windows.Forms.Button();
            this.btn_Print = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.rptListBox = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_AllLocation
            // 
            this.cb_AllLocation.AutoSize = true;
            this.cb_AllLocation.Location = new System.Drawing.Point(12, 12);
            this.cb_AllLocation.Name = "cb_AllLocation";
            this.cb_AllLocation.Size = new System.Drawing.Size(86, 17);
            this.cb_AllLocation.TabIndex = 0;
            this.cb_AllLocation.Text = "All Locations";
            this.cb_AllLocation.UseVisualStyleBackColor = true;
            this.cb_AllLocation.CheckedChanged += new System.EventHandler(this.cb_AllLocation_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_SiteTo);
            this.groupBox1.Controls.Add(this.txt_SiteFrom);
            this.groupBox1.Controls.Add(this.lblTo);
            this.groupBox1.Controls.Add(this.lblFrom);
            this.groupBox1.Location = new System.Drawing.Point(12, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 82);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Location";
            // 
            // txt_SiteTo
            // 
            this.txt_SiteTo.BackColor = System.Drawing.Color.Yellow;
            this.txt_SiteTo.Location = new System.Drawing.Point(234, 38);
            this.txt_SiteTo.Name = "txt_SiteTo";
            this.txt_SiteTo.Size = new System.Drawing.Size(100, 20);
            this.txt_SiteTo.TabIndex = 3;
            this.txt_SiteTo.TextChanged += new System.EventHandler(this.txt_SiteTo_TextChanged);
            // 
            // txt_SiteFrom
            // 
            this.txt_SiteFrom.BackColor = System.Drawing.Color.Yellow;
            this.txt_SiteFrom.Location = new System.Drawing.Point(66, 38);
            this.txt_SiteFrom.Name = "txt_SiteFrom";
            this.txt_SiteFrom.Size = new System.Drawing.Size(100, 20);
            this.txt_SiteFrom.TabIndex = 2;
            this.txt_SiteFrom.TextChanged += new System.EventHandler(this.txt_SiteFrom_TextChanged);
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(196, 41);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(20, 13);
            this.lblTo.TabIndex = 1;
            this.lblTo.Text = "To";
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(19, 41);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 13);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "From";
            // 
            // btn_Preview
            // 
            this.btn_Preview.Location = new System.Drawing.Point(47, 215);
            this.btn_Preview.Name = "btn_Preview";
            this.btn_Preview.Size = new System.Drawing.Size(75, 23);
            this.btn_Preview.TabIndex = 2;
            this.btn_Preview.Text = "Preview";
            this.btn_Preview.UseVisualStyleBackColor = true;
            this.btn_Preview.Click += new System.EventHandler(this.btn_Preview_Click);
            // 
            // btn_Print
            // 
            this.btn_Print.Location = new System.Drawing.Point(165, 215);
            this.btn_Print.Name = "btn_Print";
            this.btn_Print.Size = new System.Drawing.Size(75, 23);
            this.btn_Print.TabIndex = 3;
            this.btn_Print.Text = "Print";
            this.btn_Print.UseVisualStyleBackColor = true;
            this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(284, 215);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 4;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // rptListBox
            // 
            this.rptListBox.FormattingEnabled = true;
            this.rptListBox.Location = new System.Drawing.Point(13, 134);
            this.rptListBox.Name = "rptListBox";
            this.rptListBox.Size = new System.Drawing.Size(369, 69);
            this.rptListBox.TabIndex = 6;
            this.rptListBox.SelectedIndexChanged += new System.EventHandler(this.rptListBox_SelectedIndexChanged);
            // 
            // TRfilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 250);
            this.Controls.Add(this.rptListBox);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Print);
            this.Controls.Add(this.btn_Preview);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cb_AllLocation);
            this.Name = "TRfilter";
            this.Text = "Filter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_AllLocation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Preview;
        private System.Windows.Forms.Button btn_Print;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.TextBox txt_SiteTo;
        private System.Windows.Forms.TextBox txt_SiteFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.ListBox rptListBox;
    }
}