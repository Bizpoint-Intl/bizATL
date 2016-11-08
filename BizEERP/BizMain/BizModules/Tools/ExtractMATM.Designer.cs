namespace ATL.ExtractMATM
{
    partial class ExtractMATM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractMATM));
            this.lbl_Cat = new System.Windows.Forms.Label();
            this.dgv_Filter = new System.Windows.Forms.DataGridView();
            this.Mark = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pcatname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cnmatname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uomcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_MarkAll = new System.Windows.Forms.Button();
            this.btn_Extract = new System.Windows.Forms.Button();
            this.btn_UnmarkAll = new System.Windows.Forms.Button();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.grp_Filter = new System.Windows.Forms.GroupBox();
            this.cbo_Name = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.lblName = new System.Windows.Forms.Label();
            this.cbo_Code = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.lbl_code = new System.Windows.Forms.Label();
            this.cbo_Cat = new ATL.MultiColumnComboBox.ColumnComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Filter)).BeginInit();
            this.grp_Filter.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Cat
            // 
            this.lbl_Cat.AutoSize = true;
            this.lbl_Cat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Cat.Location = new System.Drawing.Point(11, 16);
            this.lbl_Cat.Name = "lbl_Cat";
            this.lbl_Cat.Size = new System.Drawing.Size(81, 20);
            this.lbl_Cat.TabIndex = 3;
            this.lbl_Cat.Text = "Category";
            // 
            // dgv_Filter
            // 
            this.dgv_Filter.AllowUserToAddRows = false;
            this.dgv_Filter.AllowUserToDeleteRows = false;
            this.dgv_Filter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Filter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Mark,
            this.pcatname,
            this.matnum,
            this.matname,
            this.cnmatname,
            this.uomcode});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Filter.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_Filter.Location = new System.Drawing.Point(3, 86);
            this.dgv_Filter.Name = "dgv_Filter";
            this.dgv_Filter.Size = new System.Drawing.Size(814, 607);
            this.dgv_Filter.TabIndex = 4;
            this.dgv_Filter.Click += new System.EventHandler(this.dgv_Click);
            // 
            // Mark
            // 
            this.Mark.DataPropertyName = "Mark";
            this.Mark.HeaderText = "Mark";
            this.Mark.Name = "Mark";
            this.Mark.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Mark.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Mark.Width = 40;
            // 
            // pcatname
            // 
            this.pcatname.DataPropertyName = "pcatname";
            this.pcatname.HeaderText = "Category";
            this.pcatname.Name = "pcatname";
            this.pcatname.ReadOnly = true;
            // 
            // matnum
            // 
            this.matnum.DataPropertyName = "matnum";
            this.matnum.HeaderText = "MaterialCode";
            this.matnum.Name = "matnum";
            this.matnum.ReadOnly = true;
            this.matnum.Width = 110;
            // 
            // matname
            // 
            this.matname.DataPropertyName = "matname";
            this.matname.HeaderText = "Description";
            this.matname.Name = "matname";
            this.matname.ReadOnly = true;
            this.matname.Width = 250;
            // 
            // cnmatname
            // 
            this.cnmatname.DataPropertyName = "cnmatname";
            this.cnmatname.HeaderText = "Description(CN)";
            this.cnmatname.Name = "cnmatname";
            this.cnmatname.ReadOnly = true;
            this.cnmatname.Width = 200;
            // 
            // uomcode
            // 
            this.uomcode.DataPropertyName = "uomcode";
            this.uomcode.HeaderText = "UOM";
            this.uomcode.Name = "uomcode";
            this.uomcode.ReadOnly = true;
            this.uomcode.Width = 70;
            // 
            // btn_MarkAll
            // 
            this.btn_MarkAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_MarkAll.Location = new System.Drawing.Point(841, 153);
            this.btn_MarkAll.Name = "btn_MarkAll";
            this.btn_MarkAll.Size = new System.Drawing.Size(83, 28);
            this.btn_MarkAll.TabIndex = 6;
            this.btn_MarkAll.Text = "MarkAll";
            this.btn_MarkAll.UseVisualStyleBackColor = true;
            this.btn_MarkAll.Click += new System.EventHandler(this.btn_MarkAll_Click);
            // 
            // btn_Extract
            // 
            this.btn_Extract.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Extract.Location = new System.Drawing.Point(841, 190);
            this.btn_Extract.Name = "btn_Extract";
            this.btn_Extract.Size = new System.Drawing.Size(83, 28);
            this.btn_Extract.TabIndex = 8;
            this.btn_Extract.Text = "Extract";
            this.btn_Extract.UseVisualStyleBackColor = true;
            this.btn_Extract.Click += new System.EventHandler(this.btn_Extract_Click);
            // 
            // btn_UnmarkAll
            // 
            this.btn_UnmarkAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_UnmarkAll.Location = new System.Drawing.Point(841, 122);
            this.btn_UnmarkAll.Name = "btn_UnmarkAll";
            this.btn_UnmarkAll.Size = new System.Drawing.Size(83, 28);
            this.btn_UnmarkAll.TabIndex = 9;
            this.btn_UnmarkAll.Text = "UnmarkAll";
            this.btn_UnmarkAll.UseVisualStyleBackColor = true;
            this.btn_UnmarkAll.Click += new System.EventHandler(this.btn_UnmarkAll_Click);
            // 
            // btn_Exit
            // 
            this.btn_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Exit.Location = new System.Drawing.Point(841, 89);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(83, 28);
            this.btn_Exit.TabIndex = 10;
            this.btn_Exit.Text = "Exit";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // grp_Filter
            // 
            this.grp_Filter.Controls.Add(this.cbo_Name);
            this.grp_Filter.Controls.Add(this.lblName);
            this.grp_Filter.Controls.Add(this.cbo_Code);
            this.grp_Filter.Controls.Add(this.lbl_code);
            this.grp_Filter.Controls.Add(this.cbo_Cat);
            this.grp_Filter.Controls.Add(this.lbl_Cat);
            this.grp_Filter.Location = new System.Drawing.Point(12, 1);
            this.grp_Filter.Name = "grp_Filter";
            this.grp_Filter.Size = new System.Drawing.Size(805, 79);
            this.grp_Filter.TabIndex = 11;
            this.grp_Filter.TabStop = false;
            this.grp_Filter.Text = "Filter";
            // 
            // cbo_Name
            // 
            this.cbo_Name.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbo_Name.DropDownWidth = 17;
            this.cbo_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbo_Name.FormattingEnabled = true;
            this.cbo_Name.Location = new System.Drawing.Point(472, 13);
            this.cbo_Name.Name = "cbo_Name";
            this.cbo_Name.Size = new System.Drawing.Size(311, 23);
            this.cbo_Name.TabIndex = 8;
            this.cbo_Name.ViewColumn = 0;
            this.cbo_Name.SelectedIndexChanged += new System.EventHandler(this.cbo_Name_SelectedIndexChanged);
            this.cbo_Name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbo_Name_KeyDown);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(371, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(96, 20);
            this.lblName.TabIndex = 9;
            this.lblName.Text = "Name (EN)";
            // 
            // cbo_Code
            // 
            this.cbo_Code.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbo_Code.DropDownWidth = 17;
            this.cbo_Code.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbo_Code.FormattingEnabled = true;
            this.cbo_Code.Location = new System.Drawing.Point(98, 46);
            this.cbo_Code.Name = "cbo_Code";
            this.cbo_Code.Size = new System.Drawing.Size(230, 23);
            this.cbo_Code.TabIndex = 4;
            this.cbo_Code.ViewColumn = 0;
            this.cbo_Code.SelectedIndexChanged += new System.EventHandler(this.cbo_Code_SelectedIndexChanged);
            // 
            // lbl_code
            // 
            this.lbl_code.AutoSize = true;
            this.lbl_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_code.Location = new System.Drawing.Point(11, 49);
            this.lbl_code.Name = "lbl_code";
            this.lbl_code.Size = new System.Drawing.Size(51, 20);
            this.lbl_code.TabIndex = 6;
            this.lbl_code.Text = "Code";
            // 
            // cbo_Cat
            // 
            this.cbo_Cat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbo_Cat.DropDownWidth = 17;
            this.cbo_Cat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbo_Cat.FormattingEnabled = true;
            this.cbo_Cat.Location = new System.Drawing.Point(98, 16);
            this.cbo_Cat.Name = "cbo_Cat";
            this.cbo_Cat.Size = new System.Drawing.Size(230, 23);
            this.cbo_Cat.TabIndex = 1;
            this.cbo_Cat.ViewColumn = 0;
            this.cbo_Cat.SelectedIndexChanged += new System.EventHandler(this.cbo_Cat_SelectedIndexChanged);
            // 
            // ExtractMATM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 705);
            this.Controls.Add(this.grp_Filter);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.btn_UnmarkAll);
            this.Controls.Add(this.btn_Extract);
            this.Controls.Add(this.btn_MarkAll);
            this.Controls.Add(this.dgv_Filter);
            this.Name = "ExtractMATM";
            this.Text = "ExtractMATM";
            this.Load += new System.EventHandler(this.ExtractMATM_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Filter)).EndInit();
            this.grp_Filter.ResumeLayout(false);
            this.grp_Filter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ATL.MultiColumnComboBox.ColumnComboBox cbo_Cat;
        private System.Windows.Forms.Label lbl_Cat;
        private System.Windows.Forms.DataGridView dgv_Filter;
        private System.Windows.Forms.Button btn_MarkAll;
        private System.Windows.Forms.Button btn_Extract;
        private System.Windows.Forms.Button btn_UnmarkAll;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.GroupBox grp_Filter;
        private ATL.MultiColumnComboBox.ColumnComboBox cbo_Code;
        private System.Windows.Forms.Label lbl_code;
        private ATL.MultiColumnComboBox.ColumnComboBox cbo_Name;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Mark;
        private System.Windows.Forms.DataGridViewTextBoxColumn pcatname;
        private System.Windows.Forms.DataGridViewTextBoxColumn matnum;
        private System.Windows.Forms.DataGridViewTextBoxColumn matname;
        private System.Windows.Forms.DataGridViewTextBoxColumn cnmatname;
        private System.Windows.Forms.DataGridViewTextBoxColumn uomcode;
    }
}