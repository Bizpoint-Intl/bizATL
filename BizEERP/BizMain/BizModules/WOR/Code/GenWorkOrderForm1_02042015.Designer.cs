namespace ATL.BizModules.WOR
{
    partial class GenWorkOrderForm1_02042015
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.SiteDGV1 = new System.Windows.Forms.DataGridView();
            this.GenerateBtn = new System.Windows.Forms.Button();
            this.ConfirmBtn = new System.Windows.Forms.Button();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.ALLcb = new System.Windows.Forms.CheckBox();
            this.SVCcb = new System.Windows.Forms.CheckBox();
            this.PWORcb = new System.Windows.Forms.CheckBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SiteDGV1)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(60, 14);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // SiteDGV1
            // 
            this.SiteDGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SiteDGV1.Location = new System.Drawing.Point(18, 40);
            this.SiteDGV1.Name = "SiteDGV1";
            this.SiteDGV1.Size = new System.Drawing.Size(1148, 465);
            this.SiteDGV1.TabIndex = 1;
            // 
            // GenerateBtn
            // 
            this.GenerateBtn.Location = new System.Drawing.Point(24, 511);
            this.GenerateBtn.Name = "GenerateBtn";
            this.GenerateBtn.Size = new System.Drawing.Size(75, 23);
            this.GenerateBtn.TabIndex = 3;
            this.GenerateBtn.Text = "Generate";
            this.GenerateBtn.UseVisualStyleBackColor = true;
            this.GenerateBtn.Click += new System.EventHandler(this.GenerateBtn_Click);
            // 
            // ConfirmBtn
            // 
            this.ConfirmBtn.Location = new System.Drawing.Point(230, 511);
            this.ConfirmBtn.Name = "ConfirmBtn";
            this.ConfirmBtn.Size = new System.Drawing.Size(75, 23);
            this.ConfirmBtn.TabIndex = 4;
            this.ConfirmBtn.Text = "Confirm";
            this.ConfirmBtn.UseVisualStyleBackColor = true;
            this.ConfirmBtn.Click += new System.EventHandler(this.ConfirmBtn_Click);
            // 
            // RefreshBtn
            // 
            this.RefreshBtn.Location = new System.Drawing.Point(127, 511);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(75, 23);
            this.RefreshBtn.TabIndex = 5;
            this.RefreshBtn.Text = "Refresh";
            this.RefreshBtn.UseVisualStyleBackColor = true;
            this.RefreshBtn.Click += new System.EventHandler(this.RefreshBtn_Click);
            // 
            // ALLcb
            // 
            this.ALLcb.AutoSize = true;
            this.ALLcb.Location = new System.Drawing.Point(761, 14);
            this.ALLcb.Name = "ALLcb";
            this.ALLcb.Size = new System.Drawing.Size(45, 17);
            this.ALLcb.TabIndex = 6;
            this.ALLcb.Text = "ALL";
            this.ALLcb.UseVisualStyleBackColor = true;
            this.ALLcb.CheckedChanged += new System.EventHandler(this.ALLcb_CheckedChanged);
            // 
            // SVCcb
            // 
            this.SVCcb.AutoSize = true;
            this.SVCcb.Location = new System.Drawing.Point(812, 14);
            this.SVCcb.Name = "SVCcb";
            this.SVCcb.Size = new System.Drawing.Size(47, 17);
            this.SVCcb.TabIndex = 7;
            this.SVCcb.Text = "SVC";
            this.SVCcb.UseVisualStyleBackColor = true;
            this.SVCcb.CheckedChanged += new System.EventHandler(this.SVCcb_CheckedChanged);
            // 
            // PWORcb
            // 
            this.PWORcb.AutoSize = true;
            this.PWORcb.Location = new System.Drawing.Point(865, 14);
            this.PWORcb.Name = "PWORcb";
            this.PWORcb.Size = new System.Drawing.Size(60, 17);
            this.PWORcb.TabIndex = 8;
            this.PWORcb.Text = "PWOR";
            this.PWORcb.UseVisualStyleBackColor = true;
            this.PWORcb.CheckedChanged += new System.EventHandler(this.PWORcb_CheckedChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Mark";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Site";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "SiteName";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Site Template";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(326, 15);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(300, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "To";
            // 
            // GenWorkOrderForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 554);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.PWORcb);
            this.Controls.Add(this.SVCcb);
            this.Controls.Add(this.ALLcb);
            this.Controls.Add(this.RefreshBtn);
            this.Controls.Add(this.ConfirmBtn);
            this.Controls.Add(this.GenerateBtn);
            this.Controls.Add(this.SiteDGV1);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "GenWorkOrderForm1";
            this.Text = "Generate WOR";
            this.Load += new System.EventHandler(this.GenWorkOrderForm1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SiteDGV1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DataGridView SiteDGV1;
        private System.Windows.Forms.Button GenerateBtn;
        private System.Windows.Forms.Button ConfirmBtn;
        private System.Windows.Forms.Button RefreshBtn;
        private System.Windows.Forms.CheckBox ALLcb;
        private System.Windows.Forms.CheckBox SVCcb;
        private System.Windows.Forms.CheckBox PWORcb;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}