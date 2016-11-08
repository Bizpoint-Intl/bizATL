namespace ATL.SupplierSearch
{
    partial class APMSearch
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
            this.txtApName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtApNum = new System.Windows.Forms.TextBox();
            this.cmdNext = new System.Windows.Forms.Button();
            this.cmdPrevious = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdLast = new System.Windows.Forms.Button();
            this.cmdFirst = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdGo = new System.Windows.Forms.Button();
            this.txtTotalPage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dgSupplier = new System.Windows.Forms.DataGridView();
            this.apnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.apname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSupplier)).BeginInit();
            this.SuspendLayout();
            // 
            // txtApName
            // 
            this.txtApName.Location = new System.Drawing.Point(202, 31);
            this.txtApName.Name = "txtApName";
            this.txtApName.Size = new System.Drawing.Size(342, 20);
            this.txtApName.TabIndex = 2;
            this.txtApName.TextChanged += new System.EventHandler(this.txtApName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Supplier Name";
            // 
            // txtApNum
            // 
            this.txtApNum.Location = new System.Drawing.Point(6, 31);
            this.txtApNum.Name = "txtApNum";
            this.txtApNum.Size = new System.Drawing.Size(190, 20);
            this.txtApNum.TabIndex = 1;
            this.txtApNum.TextChanged += new System.EventHandler(this.txtApNum_TextChanged);
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
            this.groupBox1.Controls.Add(this.txtApName);
            this.groupBox1.Controls.Add(this.txtApNum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 57);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Supplier Code";
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
            this.groupBox2.Controls.Add(this.dgSupplier);
            this.groupBox2.Location = new System.Drawing.Point(6, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(551, 292);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
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
            // dgSupplier
            // 
            this.dgSupplier.AllowUserToAddRows = false;
            this.dgSupplier.AllowUserToDeleteRows = false;
            this.dgSupplier.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSupplier.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.apnum,
            this.apname});
            this.dgSupplier.Location = new System.Drawing.Point(6, 12);
            this.dgSupplier.Name = "dgSupplier";
            this.dgSupplier.ReadOnly = true;
            this.dgSupplier.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgSupplier.Size = new System.Drawing.Size(538, 244);
            this.dgSupplier.TabIndex = 0;
			this.dgSupplier.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSupplier_CellContentClick);
            // 
            // apnum
            // 
            this.apnum.DataPropertyName = "apnum";
            this.apnum.HeaderText = "Supplier Code";
            this.apnum.Name = "apnum";
            this.apnum.ReadOnly = true;
            this.apnum.Width = 135;
            // 
            // apname
            // 
            this.apname.DataPropertyName = "apname";
            this.apname.HeaderText = "Supplier Name";
            this.apname.Name = "apname";
            this.apname.ReadOnly = true;
            this.apname.Width = 360;
            // 
            // APMSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 361);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "APMSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Supplier Master Search";
            this.Load += new System.EventHandler(this.APMSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSupplier)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtApName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtApNum;
        private System.Windows.Forms.Button cmdNext;
        private System.Windows.Forms.Button cmdPrevious;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdLast;
        private System.Windows.Forms.Button cmdFirst;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdGo;
        private System.Windows.Forms.TextBox txtTotalPage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgSupplier;
        private System.Windows.Forms.DataGridViewTextBoxColumn apnum;
        private System.Windows.Forms.DataGridViewTextBoxColumn apname;
    }
}