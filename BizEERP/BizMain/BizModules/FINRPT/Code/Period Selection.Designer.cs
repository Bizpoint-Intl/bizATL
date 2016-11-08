namespace ATL.BizModules.FINRPT.Code
{
    partial class Period_Selection
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmb_pdTo = new System.Windows.Forms.ComboBox();
            this.cmb_pdFrom = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Period From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Period To";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(96, 127);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmb_pdTo);
            this.groupBox1.Controls.Add(this.cmb_pdFrom);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 99);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Period Selection";
            // 
            // cmb_pdTo
            // 
            this.cmb_pdTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_pdTo.FormattingEnabled = true;
            this.cmb_pdTo.Items.AddRange(new object[] {
            "Period 1",
            "Period 2",
            "Period 3",
            "Period 4",
            "Period 5",
            "Period 6",
            "Period 7",
            "Period 8",
            "Period 9",
            "Period 10",
            "Period 11",
            "Period 12"});
            this.cmb_pdTo.Location = new System.Drawing.Point(84, 58);
            this.cmb_pdTo.Name = "cmb_pdTo";
            this.cmb_pdTo.Size = new System.Drawing.Size(121, 21);
            this.cmb_pdTo.TabIndex = 5;
            // 
            // cmb_pdFrom
            // 
            this.cmb_pdFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_pdFrom.FormattingEnabled = true;
            this.cmb_pdFrom.Items.AddRange(new object[] {
            "Period 1",
            "Period 2",
            "Period 3",
            "Period 4",
            "Period 5",
            "Period 6",
            "Period 7",
            "Period 8",
            "Period 9",
            "Period 10",
            "Period 11",
            "Period 12"});
            this.cmb_pdFrom.Location = new System.Drawing.Point(84, 27);
            this.cmb_pdFrom.Name = "cmb_pdFrom";
            this.cmb_pdFrom.Size = new System.Drawing.Size(121, 21);
            this.cmb_pdFrom.TabIndex = 4;
            // 
            // Period_Selection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 160);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Name = "Period_Selection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Period_Selection";
            this.Load += new System.EventHandler(this.Period_Selection_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmb_pdTo;
        private System.Windows.Forms.ComboBox cmb_pdFrom;
    }
}