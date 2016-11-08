namespace ATL.WAC
{
    partial class WAC_Document
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
			this.label2 = new System.Windows.Forms.Label();
			this.txtRemarks = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.dtTrandate = new System.Windows.Forms.DateTimePicker();
			this.dtClosing = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtPeriod = new System.Windows.Forms.TextBox();
			this.dgWAC = new System.Windows.Forms.DataGridView();
			this.cmdCompute = new System.Windows.Forms.Button();
			this.cmdConfirm = new System.Windows.Forms.Button();
			this.txtOpeningQty = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtInQty = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtInAmt = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtOpenAmt = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtClosingAmt = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.cmdClose = new System.Windows.Forms.Button();
			this.matnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.matname = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.openingqty = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.inqty = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.inamt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.openingcost = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.wac = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.openingamt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.closingamt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgWAC)).BeginInit();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Remarks";
			// 
			// txtRemarks
			// 
			this.txtRemarks.Location = new System.Drawing.Point(86, 30);
			this.txtRemarks.Name = "txtRemarks";
			this.txtRemarks.Size = new System.Drawing.Size(316, 21);
			this.txtRemarks.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(625, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(55, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Tran Date";
			// 
			// dtTrandate
			// 
			this.dtTrandate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtTrandate.Location = new System.Drawing.Point(680, 3);
			this.dtTrandate.Name = "dtTrandate";
			this.dtTrandate.Size = new System.Drawing.Size(102, 21);
			this.dtTrandate.TabIndex = 5;
			// 
			// dtClosing
			// 
			this.dtClosing.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtClosing.Location = new System.Drawing.Point(86, 3);
			this.dtClosing.Name = "dtClosing";
			this.dtClosing.Size = new System.Drawing.Size(102, 21);
			this.dtClosing.TabIndex = 7;
			this.dtClosing.ValueChanged += new System.EventHandler(this.dtClosing_ValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 7);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(67, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Closing Date";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(205, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Period";
			// 
			// txtPeriod
			// 
			this.txtPeriod.Location = new System.Drawing.Point(259, 5);
			this.txtPeriod.Name = "txtPeriod";
			this.txtPeriod.ReadOnly = true;
			this.txtPeriod.Size = new System.Drawing.Size(102, 21);
			this.txtPeriod.TabIndex = 9;
			// 
			// dgWAC
			// 
			this.dgWAC.AllowUserToAddRows = false;
			this.dgWAC.AllowUserToDeleteRows = false;
			this.dgWAC.AllowUserToOrderColumns = true;
			this.dgWAC.AllowUserToResizeRows = false;
			this.dgWAC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgWAC.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.matnum,
            this.matname,
            this.openingqty,
            this.inqty,
            this.inamt,
            this.openingcost,
            this.wac,
            this.openingamt,
            this.closingamt});
			this.dgWAC.Location = new System.Drawing.Point(8, 57);
			this.dgWAC.Name = "dgWAC";
			this.dgWAC.Size = new System.Drawing.Size(778, 366);
			this.dgWAC.TabIndex = 10;
			// 
			// cmdCompute
			// 
			this.cmdCompute.Location = new System.Drawing.Point(626, 28);
			this.cmdCompute.Name = "cmdCompute";
			this.cmdCompute.Size = new System.Drawing.Size(75, 23);
			this.cmdCompute.TabIndex = 11;
			this.cmdCompute.Text = "Compute";
			this.cmdCompute.UseVisualStyleBackColor = true;
			this.cmdCompute.Click += new System.EventHandler(this.cmdCompute_Click);
			// 
			// cmdConfirm
			// 
			this.cmdConfirm.Enabled = false;
			this.cmdConfirm.Location = new System.Drawing.Point(707, 28);
			this.cmdConfirm.Name = "cmdConfirm";
			this.cmdConfirm.Size = new System.Drawing.Size(75, 23);
			this.cmdConfirm.TabIndex = 12;
			this.cmdConfirm.Text = "Confirm";
			this.cmdConfirm.UseVisualStyleBackColor = true;
			this.cmdConfirm.Click += new System.EventHandler(this.cmdConfirm_Click);
			// 
			// txtOpeningQty
			// 
			this.txtOpeningQty.Location = new System.Drawing.Point(102, 429);
			this.txtOpeningQty.Name = "txtOpeningQty";
			this.txtOpeningQty.ReadOnly = true;
			this.txtOpeningQty.Size = new System.Drawing.Size(109, 21);
			this.txtOpeningQty.TabIndex = 14;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 432);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(95, 13);
			this.label6.TabIndex = 13;
			this.label6.Text = "Total Opening Qty";
			// 
			// txtInQty
			// 
			this.txtInQty.Location = new System.Drawing.Point(102, 453);
			this.txtInQty.Name = "txtInQty";
			this.txtInQty.ReadOnly = true;
			this.txtInQty.Size = new System.Drawing.Size(109, 21);
			this.txtInQty.TabIndex = 16;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 456);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(65, 13);
			this.label7.TabIndex = 15;
			this.label7.Text = "Total In Qty";
			// 
			// txtInAmt
			// 
			this.txtInAmt.Location = new System.Drawing.Point(352, 453);
			this.txtInAmt.Name = "txtInAmt";
			this.txtInAmt.ReadOnly = true;
			this.txtInAmt.Size = new System.Drawing.Size(109, 21);
			this.txtInAmt.TabIndex = 20;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(255, 456);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(66, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "Total In Amt";
			// 
			// txtOpenAmt
			// 
			this.txtOpenAmt.Location = new System.Drawing.Point(352, 429);
			this.txtOpenAmt.Name = "txtOpenAmt";
			this.txtOpenAmt.ReadOnly = true;
			this.txtOpenAmt.Size = new System.Drawing.Size(109, 21);
			this.txtOpenAmt.TabIndex = 18;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(255, 432);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 13);
			this.label9.TabIndex = 17;
			this.label9.Text = "Total Opening Amt";
			// 
			// txtClosingAmt
			// 
			this.txtClosingAmt.Location = new System.Drawing.Point(571, 429);
			this.txtClosingAmt.Name = "txtClosingAmt";
			this.txtClosingAmt.ReadOnly = true;
			this.txtClosingAmt.Size = new System.Drawing.Size(109, 21);
			this.txtClosingAmt.TabIndex = 22;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(502, 432);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(63, 13);
			this.label10.TabIndex = 21;
			this.label10.Text = "Closing Amt";
			// 
			// cmdClose
			// 
			this.cmdClose.Location = new System.Drawing.Point(711, 432);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(75, 23);
			this.cmdClose.TabIndex = 23;
			this.cmdClose.Text = "Close";
			this.cmdClose.UseVisualStyleBackColor = true;
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// matnum
			// 
			this.matnum.DataPropertyName = "matnum";
			dataGridViewCellStyle19.BackColor = System.Drawing.Color.LightCyan;
			this.matnum.DefaultCellStyle = dataGridViewCellStyle19;
			this.matnum.HeaderText = "Product Code";
			this.matnum.Name = "matnum";
			this.matnum.ReadOnly = true;
			// 
			// matname
			// 
			this.matname.DataPropertyName = "matname";
			dataGridViewCellStyle20.BackColor = System.Drawing.Color.LightCyan;
			this.matname.DefaultCellStyle = dataGridViewCellStyle20;
			this.matname.HeaderText = "Product Name";
			this.matname.Name = "matname";
			this.matname.ReadOnly = true;
			this.matname.Width = 250;
			// 
			// openingqty
			// 
			this.openingqty.DataPropertyName = "openingqty";
			dataGridViewCellStyle21.BackColor = System.Drawing.Color.LightCyan;
			this.openingqty.DefaultCellStyle = dataGridViewCellStyle21;
			this.openingqty.HeaderText = "Opn Qty";
			this.openingqty.Name = "openingqty";
			this.openingqty.ReadOnly = true;
			this.openingqty.Width = 70;
			// 
			// inqty
			// 
			this.inqty.DataPropertyName = "inqty";
			dataGridViewCellStyle22.BackColor = System.Drawing.Color.LightCyan;
			this.inqty.DefaultCellStyle = dataGridViewCellStyle22;
			this.inqty.HeaderText = "In Qty";
			this.inqty.Name = "inqty";
			this.inqty.ReadOnly = true;
			this.inqty.Width = 70;
			// 
			// inamt
			// 
			this.inamt.DataPropertyName = "inamt";
			dataGridViewCellStyle23.BackColor = System.Drawing.Color.LightCyan;
			this.inamt.DefaultCellStyle = dataGridViewCellStyle23;
			this.inamt.HeaderText = "In Amt";
			this.inamt.Name = "inamt";
			this.inamt.ReadOnly = true;
			this.inamt.Width = 70;
			// 
			// openingcost
			// 
			this.openingcost.DataPropertyName = "openingcost";
			dataGridViewCellStyle24.BackColor = System.Drawing.Color.LightCyan;
			this.openingcost.DefaultCellStyle = dataGridViewCellStyle24;
			this.openingcost.HeaderText = "Opn Cost";
			this.openingcost.Name = "openingcost";
			this.openingcost.ReadOnly = true;
			this.openingcost.Width = 70;
			// 
			// wac
			// 
			this.wac.DataPropertyName = "wac";
			dataGridViewCellStyle25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.wac.DefaultCellStyle = dataGridViewCellStyle25;
			this.wac.HeaderText = "WAC";
			this.wac.Name = "wac";
			this.wac.ReadOnly = true;
			this.wac.Width = 70;
			// 
			// openingamt
			// 
			this.openingamt.DataPropertyName = "openingamt";
			dataGridViewCellStyle26.BackColor = System.Drawing.Color.LightCyan;
			this.openingamt.DefaultCellStyle = dataGridViewCellStyle26;
			this.openingamt.HeaderText = "Opn Amt";
			this.openingamt.Name = "openingamt";
			this.openingamt.ReadOnly = true;
			this.openingamt.Width = 70;
			// 
			// closingamt
			// 
			this.closingamt.DataPropertyName = "closingamt";
			dataGridViewCellStyle27.BackColor = System.Drawing.Color.LightCyan;
			this.closingamt.DefaultCellStyle = dataGridViewCellStyle27;
			this.closingamt.HeaderText = "Closing Amt";
			this.closingamt.Name = "closingamt";
			this.closingamt.ReadOnly = true;
			// 
			// WAC_Document
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(794, 478);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.txtClosingAmt);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.txtInAmt);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtOpenAmt);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.txtInQty);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.txtOpeningQty);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.cmdConfirm);
			this.Controls.Add(this.cmdCompute);
			this.Controls.Add(this.dgWAC);
			this.Controls.Add(this.txtPeriod);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.dtClosing);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.dtTrandate);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtRemarks);
			this.Controls.Add(this.label2);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "WAC_Document";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Weighted Average Cost";
			this.Load += new System.EventHandler(this.WAC_Document_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgWAC)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtTrandate;
        private System.Windows.Forms.DateTimePicker dtClosing;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtPeriod;
		private System.Windows.Forms.DataGridView dgWAC;
		private System.Windows.Forms.Button cmdCompute;
		private System.Windows.Forms.Button cmdConfirm;
		private System.Windows.Forms.TextBox txtOpeningQty;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtInQty;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtInAmt;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtOpenAmt;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtClosingAmt;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.DataGridViewTextBoxColumn matnum;
		private System.Windows.Forms.DataGridViewTextBoxColumn matname;
		private System.Windows.Forms.DataGridViewTextBoxColumn openingqty;
		private System.Windows.Forms.DataGridViewTextBoxColumn inqty;
		private System.Windows.Forms.DataGridViewTextBoxColumn inamt;
		private System.Windows.Forms.DataGridViewTextBoxColumn openingcost;
		private System.Windows.Forms.DataGridViewTextBoxColumn wac;
		private System.Windows.Forms.DataGridViewTextBoxColumn openingamt;
		private System.Windows.Forms.DataGridViewTextBoxColumn closingamt;
    }
}