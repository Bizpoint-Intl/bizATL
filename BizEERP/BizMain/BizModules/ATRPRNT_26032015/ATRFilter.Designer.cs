namespace ATL.BizModules.ATRPRNT
{
    partial class ATRFilter
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
            this.button1 = new System.Windows.Forms.Button();
            this.siteTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.yearComboBox1 = new System.Windows.Forms.ComboBox();
            this.monthComboBox1 = new System.Windows.Forms.ComboBox();
            this.sitenameTextBox1 = new System.Windows.Forms.TextBox();
            this.allRD1 = new System.Windows.Forms.RadioButton();
            this.firstRD1 = new System.Windows.Forms.RadioButton();
            this.secondRD1 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Month";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Year";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(70, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Preview";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // siteTextBox
            // 
            this.siteTextBox.Location = new System.Drawing.Point(70, 9);
            this.siteTextBox.Name = "siteTextBox";
            this.siteTextBox.Size = new System.Drawing.Size(293, 20);
            this.siteTextBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Site";
            // 
            // yearComboBox1
            // 
            this.yearComboBox1.FormattingEnabled = true;
            this.yearComboBox1.Location = new System.Drawing.Point(263, 58);
            this.yearComboBox1.Name = "yearComboBox1";
            this.yearComboBox1.Size = new System.Drawing.Size(100, 21);
            this.yearComboBox1.TabIndex = 7;
            // 
            // monthComboBox1
            // 
            this.monthComboBox1.FormattingEnabled = true;
            this.monthComboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.monthComboBox1.Location = new System.Drawing.Point(70, 59);
            this.monthComboBox1.Name = "monthComboBox1";
            this.monthComboBox1.Size = new System.Drawing.Size(100, 21);
            this.monthComboBox1.TabIndex = 8;
            // 
            // sitenameTextBox1
            // 
            this.sitenameTextBox1.Location = new System.Drawing.Point(70, 32);
            this.sitenameTextBox1.Name = "sitenameTextBox1";
            this.sitenameTextBox1.Size = new System.Drawing.Size(293, 20);
            this.sitenameTextBox1.TabIndex = 9;
            // 
            // allRD1
            // 
            this.allRD1.AutoSize = true;
            this.allRD1.Location = new System.Drawing.Point(22, 98);
            this.allRD1.Name = "allRD1";
            this.allRD1.Size = new System.Drawing.Size(36, 17);
            this.allRD1.TabIndex = 10;
            this.allRD1.TabStop = true;
            this.allRD1.Text = "All";
            this.allRD1.UseVisualStyleBackColor = true;
            // 
            // firstRD1
            // 
            this.firstRD1.AutoSize = true;
            this.firstRD1.Location = new System.Drawing.Point(70, 98);
            this.firstRD1.Name = "firstRD1";
            this.firstRD1.Size = new System.Drawing.Size(46, 17);
            this.firstRD1.TabIndex = 11;
            this.firstRD1.TabStop = true;
            this.firstRD1.Text = "1-15";
            this.firstRD1.UseVisualStyleBackColor = true;
            // 
            // secondRD1
            // 
            this.secondRD1.AutoSize = true;
            this.secondRD1.Location = new System.Drawing.Point(122, 98);
            this.secondRD1.Name = "secondRD1";
            this.secondRD1.Size = new System.Drawing.Size(52, 17);
            this.secondRD1.TabIndex = 12;
            this.secondRD1.TabStop = true;
            this.secondRD1.Text = "16-31";
            this.secondRD1.UseVisualStyleBackColor = true;
            // 
            // ATRFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 168);
            this.Controls.Add(this.secondRD1);
            this.Controls.Add(this.firstRD1);
            this.Controls.Add(this.allRD1);
            this.Controls.Add(this.sitenameTextBox1);
            this.Controls.Add(this.monthComboBox1);
            this.Controls.Add(this.yearComboBox1);
            this.Controls.Add(this.siteTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ATRFilter";
            this.Text = "ATRFilter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox siteTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox yearComboBox1;
        private System.Windows.Forms.ComboBox monthComboBox1;
        private System.Windows.Forms.TextBox sitenameTextBox1;
        private System.Windows.Forms.RadioButton allRD1;
        private System.Windows.Forms.RadioButton firstRD1;
        private System.Windows.Forms.RadioButton secondRD1;
    }
}