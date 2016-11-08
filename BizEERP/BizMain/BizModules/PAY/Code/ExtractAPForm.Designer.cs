namespace ATL.PAY
{
    partial class ExtractAPForm
    {
        /// <summary>
        /// 必需的设计器变量。



        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。



        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。



        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgView1 = new System.Windows.Forms.DataGridView();
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnMarkAll = new System.Windows.Forms.Button();
            this.btnUnMark = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.Mark = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            //this.refnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.sstylecode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.PP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.P = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.S = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.M = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.L = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.LL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.F = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.mstylecode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.unitprice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //this.color = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgView1
            // 
            this.dgView1.AllowUserToAddRows = false;
            this.dgView1.AllowUserToDeleteRows = false;
            this.dgView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Mark
            //this.refnum,
            //this.sstylecode,
            //this.PP,
            //this.P,
            //this.S,
            //this.M,
            //this.L,
            //this.LL,
            //this.F,
            //this.mstylecode,
            //this.unitprice,
            //this.color
            });
            this.dgView1.Location = new System.Drawing.Point(12, 12);
            this.dgView1.Name = "dgView1";
            this.dgView1.RowHeadersWidth = 20;
            this.dgView1.RowTemplate.Height = 23;
            this.dgView1.Size = new System.Drawing.Size(737, 491);
            this.dgView1.TabIndex = 0;
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Location = new System.Drawing.Point(759, 70);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 23);
            this.btnExtract.TabIndex = 1;
            this.btnExtract.Text = "E&xtract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // btnMarkAll
            // 
            this.btnMarkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMarkAll.Location = new System.Drawing.Point(759, 41);
            this.btnMarkAll.Name = "btnMarkAll";
            this.btnMarkAll.Size = new System.Drawing.Size(75, 23);
            this.btnMarkAll.TabIndex = 2;
            this.btnMarkAll.Text = "Mark All";
            this.btnMarkAll.UseVisualStyleBackColor = true;
            this.btnMarkAll.Click += new System.EventHandler(this.btnMarkAll_Click);
            // 
            // btnUnMark
            // 
            this.btnUnMark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnMark.Location = new System.Drawing.Point(759, 12);
            this.btnUnMark.Name = "btnUnMark";
            this.btnUnMark.Size = new System.Drawing.Size(75, 23);
            this.btnUnMark.TabIndex = 1;
            this.btnUnMark.Text = "Unmark All";
            this.btnUnMark.UseVisualStyleBackColor = true;
            this.btnUnMark.Click += new System.EventHandler(this.btnUnMark_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(759, 99);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "&Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // Mark
            // 
            this.Mark.DataPropertyName = "Mark";
            this.Mark.HeaderText = "Mark";
            this.Mark.Name = "Mark";
            this.Mark.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            //// 
            //// refnum
            //// 
            //this.refnum.DataPropertyName = "refnum";
            //this.refnum.HeaderText = "Por  No";
            //this.refnum.Name = "refnum";
            //this.refnum.ReadOnly = true;
            //// 
            //// sstylecode
            //// 
            //this.sstylecode.DataPropertyName = "sstylecode";
            //this.sstylecode.HeaderText = "Sstylecode";
            //this.sstylecode.Name = "sstylecode";
            //this.sstylecode.ReadOnly = true;
            //// 
            //// PP
            //// 
            //this.PP.DataPropertyName = "pp";
            //this.PP.HeaderText = "PP";
            //this.PP.Name = "PP";
            //this.PP.ReadOnly = true;
            //// 
            //// P
            //// 
            //this.P.DataPropertyName = "p";
            //this.P.HeaderText = "P";
            //this.P.Name = "P";
            //this.P.ReadOnly = true;
            //this.P.Width = 150;
            //// 
            //// S
            //// 
            //this.S.DataPropertyName = "s";
            //this.S.HeaderText = "S";
            //this.S.Name = "S";
            //this.S.ReadOnly = true;
            //// 
            //// M
            //// 
            //this.M.DataPropertyName = "m";
            //this.M.HeaderText = "M";
            //this.M.Name = "M";
            //this.M.ReadOnly = true;
            //// 
            //// L
            //// 
            //this.L.DataPropertyName = "l";
            //this.L.HeaderText = "L";
            //this.L.Name = "L";
            //// 
            //// LL
            //// 
            //this.LL.DataPropertyName = "ll";
            //this.LL.HeaderText = "LL";
            //this.LL.Name = "LL";
            //this.LL.ReadOnly = true;
            //// 
            //// F
            //// 
            //this.F.DataPropertyName = "f";
            //this.F.HeaderText = "F";
            //this.F.Name = "F";
            //// 
            //// mstylecode
            //// 
            //this.mstylecode.DataPropertyName = "mstylecode";
            //this.mstylecode.HeaderText = "Main Stylecode";
            //this.mstylecode.Name = "mstylecode";
            //this.mstylecode.Visible = false;
            //// 
            //// unitprice
            //// 
            //this.unitprice.DataPropertyName = "unitprice";
            //this.unitprice.HeaderText = "Unit Price";
            //this.unitprice.Name = "unitprice";
            //this.unitprice.Visible = false;
            //// 
            //// color
            //// 
            //this.color.DataPropertyName = "color";
            //this.color.HeaderText = "color";
            //this.color.Name = "color";
            //this.color.Visible = false;
            // 
            // ExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 515);
            this.Controls.Add(this.dgView1);
            this.Controls.Add(this.btnMarkAll);
            this.Controls.Add(this.btnUnMark);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnExtract);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ExtractForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Extract ";
            this.Load += new System.EventHandler(this.ExtractForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgView1;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnMarkAll;
        private System.Windows.Forms.Button btnUnMark;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Mark;
    }
}

