namespace ATL.ExtractATR1Form1
{
    partial class ExtractATR1Form1
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
            this.EmpDGV1 = new System.Windows.Forms.DataGridView();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.currentBasicTb = new System.Windows.Forms.TextBox();
            this.RevisedAmountTb = new System.Windows.Forms.TextBox();
            this.calculatebtn = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ProrateAmountTb = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.InsertLine = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.EmpDGV1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // EmpDGV1
            // 
            this.EmpDGV1.AllowUserToOrderColumns = true;
            this.EmpDGV1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.EmpDGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EmpDGV1.Location = new System.Drawing.Point(18, 12);
            this.EmpDGV1.Name = "EmpDGV1";
            this.EmpDGV1.Size = new System.Drawing.Size(1416, 502);
            this.EmpDGV1.TabIndex = 0;
            // 
            // CloseBtn
            // 
            this.CloseBtn.Location = new System.Drawing.Point(141, 28);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(103, 36);
            this.CloseBtn.TabIndex = 1;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // currentBasicTb
            // 
            this.currentBasicTb.Location = new System.Drawing.Point(515, 50);
            this.currentBasicTb.Name = "currentBasicTb";
            this.currentBasicTb.Size = new System.Drawing.Size(199, 20);
            this.currentBasicTb.TabIndex = 2;
            // 
            // RevisedAmountTb
            // 
            this.RevisedAmountTb.Location = new System.Drawing.Point(745, 50);
            this.RevisedAmountTb.Name = "RevisedAmountTb";
            this.RevisedAmountTb.Size = new System.Drawing.Size(199, 20);
            this.RevisedAmountTb.TabIndex = 3;
            // 
            // calculatebtn
            // 
            this.calculatebtn.Location = new System.Drawing.Point(385, 28);
            this.calculatebtn.Name = "calculatebtn";
            this.calculatebtn.Size = new System.Drawing.Size(103, 36);
            this.calculatebtn.TabIndex = 4;
            this.calculatebtn.Text = "Calculate";
            this.calculatebtn.UseVisualStyleBackColor = true;
            this.calculatebtn.Click += new System.EventHandler(this.calculatebtn_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(515, 94);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(199, 20);
            this.dateTimePicker1.TabIndex = 5;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(745, 94);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(199, 20);
            this.dateTimePicker2.TabIndex = 6;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(515, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Current Basic";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(742, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Revised Amount";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(515, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Start Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(742, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Effective Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(959, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Pro-rate Amount";
            // 
            // ProrateAmountTb
            // 
            this.ProrateAmountTb.Location = new System.Drawing.Point(962, 50);
            this.ProrateAmountTb.Name = "ProrateAmountTb";
            this.ProrateAmountTb.Size = new System.Drawing.Size(199, 20);
            this.ProrateAmountTb.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.InsertLine);
            this.groupBox1.Controls.Add(this.CloseBtn);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.currentBasicTb);
            this.groupBox1.Controls.Add(this.ProrateAmountTb);
            this.groupBox1.Controls.Add(this.RevisedAmountTb);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.calculatebtn);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dateTimePicker2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 538);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1446, 128);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // InsertLine
            // 
            this.InsertLine.Location = new System.Drawing.Point(267, 28);
            this.InsertLine.Name = "InsertLine";
            this.InsertLine.Size = new System.Drawing.Size(100, 36);
            this.InsertLine.TabIndex = 13;
            this.InsertLine.Text = "Insert";
            this.InsertLine.UseVisualStyleBackColor = true;
            this.InsertLine.Click += new System.EventHandler(this.InsertLine_Click);
            // 
            // ExtractATR1Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1446, 666);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.EmpDGV1);
            this.Name = "ExtractATR1Form1";
            this.Text = "Extract Attendance Record";
            this.Load += new System.EventHandler(this.ExtractATR1Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.EmpDGV1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView EmpDGV1;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.TextBox currentBasicTb;
        private System.Windows.Forms.TextBox RevisedAmountTb;
        private System.Windows.Forms.Button calculatebtn;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ProrateAmountTb;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button InsertLine;
    }
}