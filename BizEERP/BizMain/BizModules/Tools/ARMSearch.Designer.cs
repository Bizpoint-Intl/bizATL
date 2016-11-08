namespace ATL.CustomerSearch
{
	partial class ARMSearch
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
			this.cmdGo = new System.Windows.Forms.Button();
			this.txtTotalPage = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtPage = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.dgCustomer = new System.Windows.Forms.DataGridView();
			this.cmdFirst = new System.Windows.Forms.Button();
			this.cmdLast = new System.Windows.Forms.Button();
			this.txtArName = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cmdNext = new System.Windows.Forms.Button();
			this.cmdPrevious = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtArNum = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.arnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.arname = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.postcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.regioncode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ptc = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fax = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.HP = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.email = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.oricur = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.gstgrpnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ptnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ctnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.creditlimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.salesmanempnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.includegst = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.allowpartial = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgCustomer)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmdGo
			// 
			this.cmdGo.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdGo.Location = new System.Drawing.Point(345, 262);
			this.cmdGo.Name = "cmdGo";
			this.cmdGo.Size = new System.Drawing.Size(36, 23);
			this.cmdGo.TabIndex = 15;
			this.cmdGo.Text = "Go";
			this.cmdGo.UseVisualStyleBackColor = true;
			this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
			// 
			// txtTotalPage
			// 
			this.txtTotalPage.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTotalPage.Location = new System.Drawing.Point(288, 265);
			this.txtTotalPage.Name = "txtTotalPage";
			this.txtTotalPage.ReadOnly = true;
			this.txtTotalPage.Size = new System.Drawing.Size(51, 18);
			this.txtTotalPage.TabIndex = 14;
			this.txtTotalPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(269, 268);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(13, 11);
			this.label5.TabIndex = 13;
			this.label5.Text = "of";
			// 
			// txtPage
			// 
			this.txtPage.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPage.Location = new System.Drawing.Point(212, 265);
			this.txtPage.Name = "txtPage";
			this.txtPage.Size = new System.Drawing.Size(51, 18);
			this.txtPage.TabIndex = 12;
			this.txtPage.Text = "1";
			this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(154, 268);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 11);
			this.label4.TabIndex = 11;
			this.label4.Text = "Go to Page";
			// 
			// dgCustomer
			// 
			this.dgCustomer.AllowUserToAddRows = false;
			this.dgCustomer.AllowUserToDeleteRows = false;
			this.dgCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgCustomer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.arnum,
            this.arname,
            this.Address,
            this.postcode,
            this.regioncode,
            this.ptc,
            this.phone,
            this.fax,
            this.HP,
            this.email,
            this.oricur,
            this.gstgrpnum,
            this.ptnum,
            this.ctnum,
            this.creditlimit,
            this.salesmanempnum,
            this.includegst,
            this.allowpartial});
			this.dgCustomer.Location = new System.Drawing.Point(6, 12);
			this.dgCustomer.Name = "dgCustomer";
			this.dgCustomer.ReadOnly = true;
			this.dgCustomer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgCustomer.Size = new System.Drawing.Size(538, 244);
			this.dgCustomer.TabIndex = 0;
			this.dgCustomer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgCustomer_CellContentClick);
			// 
			// cmdFirst
			// 
			this.cmdFirst.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdFirst.Location = new System.Drawing.Point(400, 262);
			this.cmdFirst.Name = "cmdFirst";
			this.cmdFirst.Size = new System.Drawing.Size(36, 23);
			this.cmdFirst.TabIndex = 16;
			this.cmdFirst.Text = "<<";
			this.cmdFirst.UseVisualStyleBackColor = true;
			this.cmdFirst.Click += new System.EventHandler(this.cmdFirst_Click);
			// 
			// cmdLast
			// 
			this.cmdLast.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdLast.Location = new System.Drawing.Point(508, 262);
			this.cmdLast.Name = "cmdLast";
			this.cmdLast.Size = new System.Drawing.Size(36, 23);
			this.cmdLast.TabIndex = 19;
			this.cmdLast.Text = ">>";
			this.cmdLast.UseVisualStyleBackColor = true;
			this.cmdLast.Click += new System.EventHandler(this.cmdLast_Click);
			// 
			// txtArName
			// 
			this.txtArName.Location = new System.Drawing.Point(202, 31);
			this.txtArName.Name = "txtArName";
			this.txtArName.Size = new System.Drawing.Size(342, 20);
			this.txtArName.TabIndex = 2;
			this.txtArName.TextChanged += new System.EventHandler(this.txtArName_TextChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cmdLast);
			this.groupBox2.Controls.Add(this.cmdNext);
			this.groupBox2.Controls.Add(this.cmdPrevious);
			this.groupBox2.Controls.Add(this.cmdFirst);
			this.groupBox2.Controls.Add(this.cmdGo);
			this.groupBox2.Controls.Add(this.txtTotalPage);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.txtPage);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.dgCustomer);
			this.groupBox2.Location = new System.Drawing.Point(5, 61);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(551, 292);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			// 
			// cmdNext
			// 
			this.cmdNext.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdNext.Location = new System.Drawing.Point(472, 262);
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.Size = new System.Drawing.Size(36, 23);
			this.cmdNext.TabIndex = 18;
			this.cmdNext.Text = ">";
			this.cmdNext.UseVisualStyleBackColor = true;
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// cmdPrevious
			// 
			this.cmdPrevious.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdPrevious.Location = new System.Drawing.Point(436, 262);
			this.cmdPrevious.Name = "cmdPrevious";
			this.cmdPrevious.Size = new System.Drawing.Size(36, 23);
			this.cmdPrevious.TabIndex = 17;
			this.cmdPrevious.Text = "<";
			this.cmdPrevious.UseVisualStyleBackColor = true;
			this.cmdPrevious.Click += new System.EventHandler(this.cmdPrevious_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtArName);
			this.groupBox1.Controls.Add(this.txtArNum);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(5, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(551, 57);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search";
			// 
			// txtArNum
			// 
			this.txtArNum.Location = new System.Drawing.Point(6, 31);
			this.txtArNum.Name = "txtArNum";
			this.txtArNum.Size = new System.Drawing.Size(190, 20);
			this.txtArNum.TabIndex = 1;
			this.txtArNum.TextChanged += new System.EventHandler(this.txtArNum_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(202, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Customer Name";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Customer Code";
			// 
			// arnum
			// 
			this.arnum.DataPropertyName = "arnum";
			this.arnum.HeaderText = "Customer Code";
			this.arnum.Name = "arnum";
			this.arnum.ReadOnly = true;
			this.arnum.Width = 135;
			// 
			// arname
			// 
			this.arname.DataPropertyName = "arname";
			this.arname.HeaderText = "Customer Name";
			this.arname.Name = "arname";
			this.arname.ReadOnly = true;
			this.arname.Width = 360;
			// 
			// Address
			// 
			this.Address.DataPropertyName = "address";
			this.Address.HeaderText = "Address";
			this.Address.Name = "Address";
			this.Address.ReadOnly = true;
			this.Address.Visible = false;
			this.Address.Width = 5;
			// 
			// postcode
			// 
			this.postcode.DataPropertyName = "postcode";
			this.postcode.HeaderText = "Postal Code";
			this.postcode.Name = "postcode";
			this.postcode.ReadOnly = true;
			this.postcode.Visible = false;
			this.postcode.Width = 5;
			// 
			// regioncode
			// 
			this.regioncode.DataPropertyName = "region";
			this.regioncode.HeaderText = "Region";
			this.regioncode.Name = "regioncode";
			this.regioncode.ReadOnly = true;
			this.regioncode.Visible = false;
			this.regioncode.Width = 5;
			// 
			// ptc
			// 
			this.ptc.DataPropertyName = "ptc";
			this.ptc.HeaderText = "Contact";
			this.ptc.Name = "ptc";
			this.ptc.ReadOnly = true;
			this.ptc.Visible = false;
			this.ptc.Width = 5;
			// 
			// phone
			// 
			this.phone.DataPropertyName = "phone";
			this.phone.HeaderText = "phone";
			this.phone.Name = "phone";
			this.phone.ReadOnly = true;
			this.phone.Visible = false;
			this.phone.Width = 5;
			// 
			// fax
			// 
			this.fax.DataPropertyName = "fax";
			this.fax.HeaderText = "fax";
			this.fax.Name = "fax";
			this.fax.ReadOnly = true;
			this.fax.Visible = false;
			this.fax.Width = 5;
			// 
			// HP
			// 
			this.HP.DataPropertyName = "hp";
			this.HP.HeaderText = "HP";
			this.HP.Name = "HP";
			this.HP.ReadOnly = true;
			this.HP.Visible = false;
			this.HP.Width = 5;
			// 
			// email
			// 
			this.email.DataPropertyName = "email";
			this.email.HeaderText = "email";
			this.email.Name = "email";
			this.email.ReadOnly = true;
			this.email.Visible = false;
			this.email.Width = 5;
			// 
			// oricur
			// 
			this.oricur.DataPropertyName = "oricur";
			this.oricur.HeaderText = "Currency";
			this.oricur.Name = "oricur";
			this.oricur.ReadOnly = true;
			this.oricur.Visible = false;
			// 
			// gstgrpnum
			// 
			this.gstgrpnum.DataPropertyName = "gstgrpnum";
			this.gstgrpnum.HeaderText = "GST";
			this.gstgrpnum.Name = "gstgrpnum";
			this.gstgrpnum.ReadOnly = true;
			this.gstgrpnum.Visible = false;
			// 
			// ptnum
			// 
			this.ptnum.DataPropertyName = "ptnum";
			this.ptnum.HeaderText = "Pay Terms";
			this.ptnum.Name = "ptnum";
			this.ptnum.ReadOnly = true;
			this.ptnum.Visible = false;
			// 
			// ctnum
			// 
			this.ctnum.DataPropertyName = "ctnum";
			this.ctnum.HeaderText = "Credit Terms";
			this.ctnum.Name = "ctnum";
			this.ctnum.ReadOnly = true;
			this.ctnum.Visible = false;
			// 
			// creditlimit
			// 
			this.creditlimit.DataPropertyName = "credit";
			this.creditlimit.HeaderText = "Credit Limit";
			this.creditlimit.Name = "creditlimit";
			this.creditlimit.ReadOnly = true;
			this.creditlimit.Visible = false;
			// 
			// salesmanempnum
			// 
			this.salesmanempnum.DataPropertyName = "salesmanempnum";
			this.salesmanempnum.HeaderText = "Salesman";
			this.salesmanempnum.Name = "salesmanempnum";
			this.salesmanempnum.ReadOnly = true;
			this.salesmanempnum.Visible = false;
			// 
			// includegst
			// 
			this.includegst.DataPropertyName = "includegst";
			this.includegst.HeaderText = "Include GST";
			this.includegst.Name = "includegst";
			this.includegst.ReadOnly = true;
			this.includegst.Visible = false;
			// 
			// allowpartial
			// 
			this.allowpartial.DataPropertyName = "allowpartial";
			this.allowpartial.HeaderText = "Allow Partial";
			this.allowpartial.Name = "allowpartial";
			this.allowpartial.ReadOnly = true;
			this.allowpartial.Visible = false;
			// 
			// ARMSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(562, 361);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ARMSearch";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Customer Master Search";
			this.Load += new System.EventHandler(this.ARMSearch_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgCustomer)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cmdGo;
		private System.Windows.Forms.TextBox txtTotalPage;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtPage;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DataGridView dgCustomer;
		private System.Windows.Forms.Button cmdFirst;
		private System.Windows.Forms.Button cmdLast;
		private System.Windows.Forms.TextBox txtArName;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button cmdNext;
		private System.Windows.Forms.Button cmdPrevious;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtArNum;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridViewTextBoxColumn arnum;
		private System.Windows.Forms.DataGridViewTextBoxColumn arname;
		private System.Windows.Forms.DataGridViewTextBoxColumn Address;
		private System.Windows.Forms.DataGridViewTextBoxColumn postcode;
		private System.Windows.Forms.DataGridViewTextBoxColumn regioncode;
		private System.Windows.Forms.DataGridViewTextBoxColumn ptc;
		private System.Windows.Forms.DataGridViewTextBoxColumn phone;
		private System.Windows.Forms.DataGridViewTextBoxColumn fax;
		private System.Windows.Forms.DataGridViewTextBoxColumn HP;
		private System.Windows.Forms.DataGridViewTextBoxColumn email;
		private System.Windows.Forms.DataGridViewTextBoxColumn oricur;
		private System.Windows.Forms.DataGridViewTextBoxColumn gstgrpnum;
		private System.Windows.Forms.DataGridViewTextBoxColumn ptnum;
		private System.Windows.Forms.DataGridViewTextBoxColumn ctnum;
		private System.Windows.Forms.DataGridViewTextBoxColumn creditlimit;
		private System.Windows.Forms.DataGridViewTextBoxColumn salesmanempnum;
		private System.Windows.Forms.DataGridViewTextBoxColumn includegst;
		private System.Windows.Forms.DataGridViewTextBoxColumn allowpartial;
	}
}