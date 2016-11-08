namespace ATL.SOA
{
    partial class SOASP
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
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_arnum2 = new System.Windows.Forms.TextBox();
            this.cmb_coy = new System.Windows.Forms.ComboBox();
            this.lbl_coy = new System.Windows.Forms.Label();
            this.chk_groupInvoices = new System.Windows.Forms.CheckBox();
            this.chk_showOutstanding = new System.Windows.Forms.CheckBox();
            this.rb_arname = new System.Windows.Forms.RadioButton();
            this.rb_arnum = new System.Windows.Forms.RadioButton();
            this.chk_arnum = new System.Windows.Forms.CheckBox();
            this.lbl_Date = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtp_DateTo = new System.Windows.Forms.DateTimePicker();
            this.lbl_arname = new System.Windows.Forms.Label();
            this.lbl_arnum = new System.Windows.Forms.Label();
            this.dtp_DateFrom = new System.Windows.Forms.DateTimePicker();
            this.tb_arname = new System.Windows.Forms.TextBox();
            this.tb_arnum = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(179, 261);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 0;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(260, 261);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(341, 261);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tb_arnum2);
            this.groupBox1.Controls.Add(this.cmb_coy);
            this.groupBox1.Controls.Add(this.lbl_coy);
            this.groupBox1.Controls.Add(this.chk_groupInvoices);
            this.groupBox1.Controls.Add(this.chk_showOutstanding);
            this.groupBox1.Controls.Add(this.rb_arname);
            this.groupBox1.Controls.Add(this.rb_arnum);
            this.groupBox1.Controls.Add(this.chk_arnum);
            this.groupBox1.Controls.Add(this.lbl_Date);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtp_DateTo);
            this.groupBox1.Controls.Add(this.lbl_arname);
            this.groupBox1.Controls.Add(this.lbl_arnum);
            this.groupBox1.Controls.Add(this.dtp_DateFrom);
            this.groupBox1.Controls.Add(this.tb_arname);
            this.groupBox1.Controls.Add(this.tb_arnum);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 229);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Selection";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 67;
            this.label1.Text = "TO";
            // 
            // tb_arnum2
            // 
            this.tb_arnum2.BackColor = System.Drawing.Color.Yellow;
            this.tb_arnum2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tb_arnum2.Location = new System.Drawing.Point(280, 59);
            this.tb_arnum2.MaxLength = 20;
            this.tb_arnum2.Name = "tb_arnum2";
            this.tb_arnum2.Size = new System.Drawing.Size(101, 20);
            this.tb_arnum2.TabIndex = 66;
            this.tb_arnum2.DoubleClick += new System.EventHandler(this.tb_arnum2_DoubleClick);
            this.tb_arnum2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_arnum2_KeyDown);
            this.tb_arnum2.Enter += new System.EventHandler(this.tb_arnum2_Enter);
            // 
            // cmb_coy
            // 
            this.cmb_coy.FormattingEnabled = true;
            this.cmb_coy.Location = new System.Drawing.Point(137, 159);
            this.cmb_coy.Name = "cmb_coy";
            this.cmb_coy.Size = new System.Drawing.Size(121, 21);
            this.cmb_coy.TabIndex = 17;
            // 
            // lbl_coy
            // 
            this.lbl_coy.AutoSize = true;
            this.lbl_coy.Location = new System.Drawing.Point(36, 162);
            this.lbl_coy.Name = "lbl_coy";
            this.lbl_coy.Size = new System.Drawing.Size(51, 13);
            this.lbl_coy.TabIndex = 65;
            this.lbl_coy.Text = "Company";
            // 
            // chk_groupInvoices
            // 
            this.chk_groupInvoices.AutoSize = true;
            this.chk_groupInvoices.Location = new System.Drawing.Point(218, 206);
            this.chk_groupInvoices.Name = "chk_groupInvoices";
            this.chk_groupInvoices.Size = new System.Drawing.Size(98, 17);
            this.chk_groupInvoices.TabIndex = 19;
            this.chk_groupInvoices.Text = "Group Invoices";
            this.chk_groupInvoices.UseVisualStyleBackColor = true;
            // 
            // chk_showOutstanding
            // 
            this.chk_showOutstanding.AutoSize = true;
            this.chk_showOutstanding.Location = new System.Drawing.Point(13, 206);
            this.chk_showOutstanding.Name = "chk_showOutstanding";
            this.chk_showOutstanding.Size = new System.Drawing.Size(180, 17);
            this.chk_showOutstanding.TabIndex = 18;
            this.chk_showOutstanding.Text = "Show Outstanding Invoices Only";
            this.chk_showOutstanding.UseVisualStyleBackColor = true;
            // 
            // rb_arname
            // 
            this.rb_arname.AutoSize = true;
            this.rb_arname.Location = new System.Drawing.Point(16, 95);
            this.rb_arname.Name = "rb_arname";
            this.rb_arname.Size = new System.Drawing.Size(14, 13);
            this.rb_arname.TabIndex = 13;
            this.rb_arname.UseVisualStyleBackColor = true;
            this.rb_arname.CheckedChanged += new System.EventHandler(this.rb_arname_CheckedChanged);
            // 
            // rb_arnum
            // 
            this.rb_arnum.AutoSize = true;
            this.rb_arnum.Checked = true;
            this.rb_arnum.Location = new System.Drawing.Point(16, 62);
            this.rb_arnum.Name = "rb_arnum";
            this.rb_arnum.Size = new System.Drawing.Size(14, 13);
            this.rb_arnum.TabIndex = 11;
            this.rb_arnum.TabStop = true;
            this.rb_arnum.UseVisualStyleBackColor = true;
            this.rb_arnum.CheckedChanged += new System.EventHandler(this.rb_arnum_CheckedChanged);
            // 
            // chk_arnum
            // 
            this.chk_arnum.AutoSize = true;
            this.chk_arnum.Location = new System.Drawing.Point(137, 31);
            this.chk_arnum.Name = "chk_arnum";
            this.chk_arnum.Size = new System.Drawing.Size(89, 17);
            this.chk_arnum.TabIndex = 10;
            this.chk_arnum.Text = "All Customers";
            this.chk_arnum.UseVisualStyleBackColor = true;
            this.chk_arnum.CheckedChanged += new System.EventHandler(this.chk_arnum_CheckedChanged);
            // 
            // lbl_Date
            // 
            this.lbl_Date.AutoSize = true;
            this.lbl_Date.Location = new System.Drawing.Point(36, 129);
            this.lbl_Date.Name = "lbl_Date";
            this.lbl_Date.Size = new System.Drawing.Size(68, 13);
            this.lbl_Date.TabIndex = 64;
            this.lbl_Date.Text = "Invoice Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(248, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 63;
            this.label3.Text = "to";
            // 
            // dtp_DateTo
            // 
            this.dtp_DateTo.CustomFormat = "dd-MM-yyyy";
            this.dtp_DateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_DateTo.Location = new System.Drawing.Point(280, 125);
            this.dtp_DateTo.Name = "dtp_DateTo";
            this.dtp_DateTo.Size = new System.Drawing.Size(101, 20);
            this.dtp_DateTo.TabIndex = 16;
            // 
            // lbl_arname
            // 
            this.lbl_arname.AutoSize = true;
            this.lbl_arname.Location = new System.Drawing.Point(36, 95);
            this.lbl_arname.Name = "lbl_arname";
            this.lbl_arname.Size = new System.Drawing.Size(82, 13);
            this.lbl_arname.TabIndex = 62;
            this.lbl_arname.Text = "Customer Name";
            // 
            // lbl_arnum
            // 
            this.lbl_arnum.AutoSize = true;
            this.lbl_arnum.Location = new System.Drawing.Point(36, 62);
            this.lbl_arnum.Name = "lbl_arnum";
            this.lbl_arnum.Size = new System.Drawing.Size(79, 13);
            this.lbl_arnum.TabIndex = 61;
            this.lbl_arnum.Text = "Customer Code";
            // 
            // dtp_DateFrom
            // 
            this.dtp_DateFrom.CustomFormat = "dd-MM-yyyy";
            this.dtp_DateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_DateFrom.Location = new System.Drawing.Point(137, 125);
            this.dtp_DateFrom.Name = "dtp_DateFrom";
            this.dtp_DateFrom.Size = new System.Drawing.Size(101, 20);
            this.dtp_DateFrom.TabIndex = 15;
            // 
            // tb_arname
            // 
            this.tb_arname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tb_arname.Enabled = false;
            this.tb_arname.Location = new System.Drawing.Point(137, 92);
            this.tb_arname.MaxLength = 60;
            this.tb_arname.Name = "tb_arname";
            this.tb_arname.Size = new System.Drawing.Size(186, 20);
            this.tb_arname.TabIndex = 14;
            // 
            // tb_arnum
            // 
            this.tb_arnum.BackColor = System.Drawing.Color.Yellow;
            this.tb_arnum.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tb_arnum.Location = new System.Drawing.Point(137, 59);
            this.tb_arnum.MaxLength = 20;
            this.tb_arnum.Name = "tb_arnum";
            this.tb_arnum.Size = new System.Drawing.Size(101, 20);
            this.tb_arnum.TabIndex = 12;
            this.tb_arnum.DoubleClick += new System.EventHandler(this.tb_arnum_DoubleClick);
            this.tb_arnum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_arnum_KeyDown);
            // 
            // SOASP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(428, 296);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SOASP";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Statement Of Accounts";
            this.Load += new System.EventHandler(this.SOASP_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }		

        #endregion

        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbl_arname;
        private System.Windows.Forms.Label lbl_arnum;
        private System.Windows.Forms.DateTimePicker dtp_DateFrom;
        private System.Windows.Forms.TextBox tb_arname;
        private System.Windows.Forms.TextBox tb_arnum;
        private System.Windows.Forms.DateTimePicker dtp_DateTo;
        private System.Windows.Forms.Label lbl_Date;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chk_arnum;
        private System.Windows.Forms.RadioButton rb_arname;
        private System.Windows.Forms.RadioButton rb_arnum;
        private System.Windows.Forms.CheckBox chk_groupInvoices;
        private System.Windows.Forms.CheckBox chk_showOutstanding;
        private System.Windows.Forms.ComboBox cmb_coy;
        private System.Windows.Forms.Label lbl_coy;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tb_arnum2;
    }
}