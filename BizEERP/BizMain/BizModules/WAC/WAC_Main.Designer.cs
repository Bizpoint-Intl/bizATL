namespace ATL.WAC
{
    partial class WAC_Main
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dgWac = new System.Windows.Forms.DataGridView();
			this.matnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.matname = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.std12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.cmdNew = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.cboPeriod = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.View = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtMatnum = new System.Windows.Forms.TextBox();
			this.txtMatname = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dgWac)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgWac
			// 
			this.dgWac.AllowUserToAddRows = false;
			this.dgWac.AllowUserToDeleteRows = false;
			this.dgWac.AllowUserToResizeRows = false;
			this.dgWac.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgWac.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.matnum,
            this.matname,
            this.std0,
            this.std1,
            this.std2,
            this.std3,
            this.std4,
            this.std5,
            this.std6,
            this.std7,
            this.std8,
            this.std9,
            this.std10,
            this.std11,
            this.std12});
			this.dgWac.Location = new System.Drawing.Point(7, 39);
			this.dgWac.Name = "dgWac";
			this.dgWac.Size = new System.Drawing.Size(780, 405);
			this.dgWac.TabIndex = 0;
			// 
			// matnum
			// 
			this.matnum.DataPropertyName = "matnum";
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
			this.matnum.DefaultCellStyle = dataGridViewCellStyle1;
			this.matnum.HeaderText = "Product Code";
			this.matnum.Name = "matnum";
			this.matnum.ReadOnly = true;
			this.matnum.Width = 120;
			// 
			// matname
			// 
			this.matname.DataPropertyName = "matname";
			dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightCyan;
			this.matname.DefaultCellStyle = dataGridViewCellStyle2;
			this.matname.HeaderText = "Product Name";
			this.matname.Name = "matname";
			this.matname.ReadOnly = true;
			this.matname.Width = 250;
			// 
			// std0
			// 
			this.std0.DataPropertyName = "std0";
			dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightCyan;
			this.std0.DefaultCellStyle = dataGridViewCellStyle3;
			this.std0.HeaderText = "P-00";
			this.std0.Name = "std0";
			this.std0.ReadOnly = true;
			this.std0.Width = 70;
			// 
			// std1
			// 
			this.std1.DataPropertyName = "std1";
			dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightCyan;
			this.std1.DefaultCellStyle = dataGridViewCellStyle4;
			this.std1.HeaderText = "P-01";
			this.std1.Name = "std1";
			this.std1.ReadOnly = true;
			this.std1.Width = 70;
			// 
			// std2
			// 
			this.std2.DataPropertyName = "std2";
			dataGridViewCellStyle5.BackColor = System.Drawing.Color.LightCyan;
			this.std2.DefaultCellStyle = dataGridViewCellStyle5;
			this.std2.HeaderText = "P-02";
			this.std2.Name = "std2";
			this.std2.ReadOnly = true;
			this.std2.Width = 70;
			// 
			// std3
			// 
			this.std3.DataPropertyName = "std3";
			dataGridViewCellStyle6.BackColor = System.Drawing.Color.LightCyan;
			this.std3.DefaultCellStyle = dataGridViewCellStyle6;
			this.std3.HeaderText = "P-03";
			this.std3.Name = "std3";
			this.std3.ReadOnly = true;
			this.std3.Width = 70;
			// 
			// std4
			// 
			this.std4.DataPropertyName = "std4";
			dataGridViewCellStyle7.BackColor = System.Drawing.Color.LightCyan;
			this.std4.DefaultCellStyle = dataGridViewCellStyle7;
			this.std4.HeaderText = "P-04";
			this.std4.Name = "std4";
			this.std4.ReadOnly = true;
			this.std4.Width = 70;
			// 
			// std5
			// 
			this.std5.DataPropertyName = "std5";
			dataGridViewCellStyle8.BackColor = System.Drawing.Color.LightCyan;
			this.std5.DefaultCellStyle = dataGridViewCellStyle8;
			this.std5.HeaderText = "P-05";
			this.std5.Name = "std5";
			this.std5.ReadOnly = true;
			this.std5.Width = 70;
			// 
			// std6
			// 
			this.std6.DataPropertyName = "std6";
			dataGridViewCellStyle9.BackColor = System.Drawing.Color.LightCyan;
			this.std6.DefaultCellStyle = dataGridViewCellStyle9;
			this.std6.HeaderText = "P-06";
			this.std6.Name = "std6";
			this.std6.ReadOnly = true;
			this.std6.Width = 70;
			// 
			// std7
			// 
			this.std7.DataPropertyName = "std7";
			dataGridViewCellStyle10.BackColor = System.Drawing.Color.LightCyan;
			this.std7.DefaultCellStyle = dataGridViewCellStyle10;
			this.std7.HeaderText = "P-07";
			this.std7.Name = "std7";
			this.std7.ReadOnly = true;
			this.std7.Width = 70;
			// 
			// std8
			// 
			this.std8.DataPropertyName = "std8";
			dataGridViewCellStyle11.BackColor = System.Drawing.Color.LightCyan;
			this.std8.DefaultCellStyle = dataGridViewCellStyle11;
			this.std8.HeaderText = "P-08";
			this.std8.Name = "std8";
			this.std8.ReadOnly = true;
			this.std8.Width = 70;
			// 
			// std9
			// 
			this.std9.DataPropertyName = "std9";
			dataGridViewCellStyle12.BackColor = System.Drawing.Color.LightCyan;
			this.std9.DefaultCellStyle = dataGridViewCellStyle12;
			this.std9.HeaderText = "P-09";
			this.std9.Name = "std9";
			this.std9.ReadOnly = true;
			this.std9.Width = 70;
			// 
			// std10
			// 
			this.std10.DataPropertyName = "std10";
			dataGridViewCellStyle13.BackColor = System.Drawing.Color.LightCyan;
			this.std10.DefaultCellStyle = dataGridViewCellStyle13;
			this.std10.HeaderText = "P-10";
			this.std10.Name = "std10";
			this.std10.ReadOnly = true;
			this.std10.Width = 70;
			// 
			// std11
			// 
			this.std11.DataPropertyName = "std11";
			dataGridViewCellStyle14.BackColor = System.Drawing.Color.LightCyan;
			this.std11.DefaultCellStyle = dataGridViewCellStyle14;
			this.std11.HeaderText = "P-11";
			this.std11.Name = "std11";
			this.std11.ReadOnly = true;
			this.std11.Width = 70;
			// 
			// std12
			// 
			this.std12.DataPropertyName = "std12";
			dataGridViewCellStyle15.BackColor = System.Drawing.Color.LightCyan;
			this.std12.DefaultCellStyle = dataGridViewCellStyle15;
			this.std12.HeaderText = "P-12";
			this.std12.Name = "std12";
			this.std12.ReadOnly = true;
			this.std12.Width = 70;
			// 
			// cmdNew
			// 
			this.cmdNew.Location = new System.Drawing.Point(571, 6);
			this.cmdNew.Name = "cmdNew";
			this.cmdNew.Size = new System.Drawing.Size(105, 27);
			this.cmdNew.TabIndex = 1;
			this.cmdNew.Text = "New";
			this.cmdNew.UseVisualStyleBackColor = true;
			this.cmdNew.Click += new System.EventHandler(this.cmdNew_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Location = new System.Drawing.Point(682, 6);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(105, 27);
			this.cmdClose.TabIndex = 2;
			this.cmdClose.Text = "Close";
			this.cmdClose.UseVisualStyleBackColor = true;
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// cboPeriod
			// 
			this.cboPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPeriod.FormattingEnabled = true;
			this.cboPeriod.Items.AddRange(new object[] {
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
			this.cboPeriod.Location = new System.Drawing.Point(49, 11);
			this.cboPeriod.Name = "cboPeriod";
			this.cboPeriod.Size = new System.Drawing.Size(67, 21);
			this.cboPeriod.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.View);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.cboPeriod);
			this.groupBox1.Location = new System.Drawing.Point(7, -2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(271, 38);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// View
			// 
			this.View.Location = new System.Drawing.Point(139, 11);
			this.View.Name = "View";
			this.View.Size = new System.Drawing.Size(105, 21);
			this.View.TabIndex = 5;
			this.View.Text = "View";
			this.View.UseVisualStyleBackColor = true;
			this.View.Click += new System.EventHandler(this.View_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Period";
			// 
			// txtMatnum
			// 
			this.txtMatnum.Location = new System.Drawing.Point(47, 450);
			this.txtMatnum.Name = "txtMatnum";
			this.txtMatnum.Size = new System.Drawing.Size(121, 21);
			this.txtMatnum.TabIndex = 5;
			this.txtMatnum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMatnum_KeyDown);
			// 
			// txtMatname
			// 
			this.txtMatname.Location = new System.Drawing.Point(168, 450);
			this.txtMatname.Name = "txtMatname";
			this.txtMatname.Size = new System.Drawing.Size(252, 21);
			this.txtMatname.TabIndex = 6;
			this.txtMatname.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMatnum_KeyDown);
			// 
			// WAC_Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(794, 478);
			this.Controls.Add(this.txtMatname);
			this.Controls.Add(this.txtMatnum);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdNew);
			this.Controls.Add(this.dgWac);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "WAC_Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Weighted Average Cost";
			this.Load += new System.EventHandler(this.WAC_Main_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgWac)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.DataGridView dgWac;
        private System.Windows.Forms.Button cmdNew;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ComboBox cboPeriod;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button View;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMatnum;
        private System.Windows.Forms.TextBox txtMatname;
		private System.Windows.Forms.DataGridViewTextBoxColumn matnum;
		private System.Windows.Forms.DataGridViewTextBoxColumn matname;
		private System.Windows.Forms.DataGridViewTextBoxColumn std0;
		private System.Windows.Forms.DataGridViewTextBoxColumn std1;
		private System.Windows.Forms.DataGridViewTextBoxColumn std2;
		private System.Windows.Forms.DataGridViewTextBoxColumn std3;
		private System.Windows.Forms.DataGridViewTextBoxColumn std4;
		private System.Windows.Forms.DataGridViewTextBoxColumn std5;
		private System.Windows.Forms.DataGridViewTextBoxColumn std6;
		private System.Windows.Forms.DataGridViewTextBoxColumn std7;
		private System.Windows.Forms.DataGridViewTextBoxColumn std8;
		private System.Windows.Forms.DataGridViewTextBoxColumn std9;
		private System.Windows.Forms.DataGridViewTextBoxColumn std10;
		private System.Windows.Forms.DataGridViewTextBoxColumn std11;
		private System.Windows.Forms.DataGridViewTextBoxColumn std12;
    }
}