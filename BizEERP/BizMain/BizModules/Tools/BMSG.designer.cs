namespace ATL.BMSG
{
	partial class BMSG
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.pnMSG = new System.Windows.Forms.Panel();
			this.pnSelection = new System.Windows.Forms.Panel();
			this.lblAvail = new System.Windows.Forms.Label();
			this.btnUnSelect = new System.Windows.Forms.Button();
			this.dgvSites = new System.Windows.Forms.DataGridView();
			this.colSName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvSelected = new System.Windows.Forms.DataGridView();
			this.colSelectedSite = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnSelect = new System.Windows.Forms.Button();
			this.lblSelected = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnSubmit = new System.Windows.Forms.Button();
			this.chkALL = new System.Windows.Forms.CheckBox();
			this.txtMSG = new System.Windows.Forms.TextBox();
			this.lblMsg = new System.Windows.Forms.Label();
			this.dgvMsgh = new System.Windows.Forms.DataGridView();
			this.pnTop = new System.Windows.Forms.Panel();
			this.btnNext = new System.Windows.Forms.Button();
			this.btnDEL = new System.Windows.Forms.Button();
			this.btnPrev = new System.Windows.Forms.Button();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.btnLast = new System.Windows.Forms.Button();
			this.btnFirst = new System.Windows.Forms.Button();
			this.dgvMsg1 = new System.Windows.Forms.DataGridView();
			this.colTSite = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colHdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colHSite = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colHMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tlpMain.SuspendLayout();
			this.pnMSG.SuspendLayout();
			this.pnSelection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSites)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSelected)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMsgh)).BeginInit();
			this.pnTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMsg1)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 2;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.tlpMain.Controls.Add(this.pnMSG, 0, 2);
			this.tlpMain.Controls.Add(this.dgvMsgh, 0, 1);
			this.tlpMain.Controls.Add(this.pnTop, 0, 0);
			this.tlpMain.Controls.Add(this.dgvMsg1, 1, 1);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 3;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 350F));
			this.tlpMain.Size = new System.Drawing.Size(865, 556);
			this.tlpMain.TabIndex = 0;
			// 
			// pnMSG
			// 
			this.tlpMain.SetColumnSpan(this.pnMSG, 2);
			this.pnMSG.Controls.Add(this.pnSelection);
			this.pnMSG.Controls.Add(this.btnClose);
			this.pnMSG.Controls.Add(this.btnSubmit);
			this.pnMSG.Controls.Add(this.chkALL);
			this.pnMSG.Controls.Add(this.txtMSG);
			this.pnMSG.Controls.Add(this.lblMsg);
			this.pnMSG.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnMSG.Location = new System.Drawing.Point(0, 206);
			this.pnMSG.Margin = new System.Windows.Forms.Padding(0);
			this.pnMSG.Name = "pnMSG";
			this.pnMSG.Size = new System.Drawing.Size(865, 350);
			this.pnMSG.TabIndex = 1;
			// 
			// pnSelection
			// 
			this.pnSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pnSelection.Controls.Add(this.lblAvail);
			this.pnSelection.Controls.Add(this.btnUnSelect);
			this.pnSelection.Controls.Add(this.dgvSites);
			this.pnSelection.Controls.Add(this.dgvSelected);
			this.pnSelection.Controls.Add(this.btnSelect);
			this.pnSelection.Controls.Add(this.lblSelected);
			this.pnSelection.Location = new System.Drawing.Point(14, 132);
			this.pnSelection.Name = "pnSelection";
			this.pnSelection.Size = new System.Drawing.Size(614, 215);
			this.pnSelection.TabIndex = 12;
			// 
			// lblAvail
			// 
			this.lblAvail.AutoSize = true;
			this.lblAvail.Location = new System.Drawing.Point(3, 11);
			this.lblAvail.Name = "lblAvail";
			this.lblAvail.Size = new System.Drawing.Size(79, 13);
			this.lblAvail.TabIndex = 10;
			this.lblAvail.Text = "Available Sites:";
			// 
			// btnUnSelect
			// 
			this.btnUnSelect.Location = new System.Drawing.Point(278, 137);
			this.btnUnSelect.Name = "btnUnSelect";
			this.btnUnSelect.Size = new System.Drawing.Size(38, 38);
			this.btnUnSelect.TabIndex = 6;
			this.btnUnSelect.Text = "<";
			this.btnUnSelect.UseVisualStyleBackColor = true;
			this.btnUnSelect.Click += new System.EventHandler(this.btnUnSelect_Click);
			// 
			// dgvSites
			// 
			this.dgvSites.AllowUserToAddRows = false;
			this.dgvSites.AllowUserToDeleteRows = false;
			this.dgvSites.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSites.ColumnHeadersVisible = false;
			this.dgvSites.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSName});
			this.dgvSites.Location = new System.Drawing.Point(6, 28);
			this.dgvSites.MultiSelect = false;
			this.dgvSites.Name = "dgvSites";
			this.dgvSites.ReadOnly = true;
			this.dgvSites.RowHeadersVisible = false;
			this.dgvSites.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvSites.Size = new System.Drawing.Size(240, 179);
			this.dgvSites.TabIndex = 3;
			// 
			// colSName
			// 
			this.colSName.DataPropertyName = "sitename";
			this.colSName.HeaderText = "SiteName";
			this.colSName.Name = "colSName";
			this.colSName.ReadOnly = true;
			this.colSName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			this.colSName.Width = 200;
			// 
			// dgvSelected
			// 
			this.dgvSelected.AllowUserToAddRows = false;
			this.dgvSelected.AllowUserToDeleteRows = false;
			this.dgvSelected.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSelected.ColumnHeadersVisible = false;
			this.dgvSelected.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelectedSite});
			this.dgvSelected.Location = new System.Drawing.Point(347, 28);
			this.dgvSelected.Name = "dgvSelected";
			this.dgvSelected.ReadOnly = true;
			this.dgvSelected.RowHeadersVisible = false;
			this.dgvSelected.Size = new System.Drawing.Size(240, 179);
			this.dgvSelected.TabIndex = 5;
			// 
			// colSelectedSite
			// 
			this.colSelectedSite.DataPropertyName = "sitename";
			this.colSelectedSite.HeaderText = "SiteName";
			this.colSelectedSite.Name = "colSelectedSite";
			this.colSelectedSite.ReadOnly = true;
			this.colSelectedSite.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			this.colSelectedSite.Width = 200;
			// 
			// btnSelect
			// 
			this.btnSelect.Location = new System.Drawing.Point(278, 77);
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.Size = new System.Drawing.Size(38, 38);
			this.btnSelect.TabIndex = 4;
			this.btnSelect.Text = ">";
			this.btnSelect.UseVisualStyleBackColor = true;
			this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
			// 
			// lblSelected
			// 
			this.lblSelected.AutoSize = true;
			this.lblSelected.Location = new System.Drawing.Point(344, 11);
			this.lblSelected.Name = "lblSelected";
			this.lblSelected.Size = new System.Drawing.Size(78, 13);
			this.lblSelected.TabIndex = 11;
			this.lblSelected.Text = "Selected Sites:";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Location = new System.Drawing.Point(769, 298);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(82, 38);
			this.btnClose.TabIndex = 8;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnSubmit
			// 
			this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubmit.Location = new System.Drawing.Point(769, 157);
			this.btnSubmit.Name = "btnSubmit";
			this.btnSubmit.Size = new System.Drawing.Size(82, 38);
			this.btnSubmit.TabIndex = 7;
			this.btnSubmit.Text = "Submit";
			this.btnSubmit.UseVisualStyleBackColor = true;
			this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
			// 
			// chkALL
			// 
			this.chkALL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkALL.AutoSize = true;
			this.chkALL.Location = new System.Drawing.Point(20, 106);
			this.chkALL.Name = "chkALL";
			this.chkALL.Size = new System.Drawing.Size(115, 17);
			this.chkALL.TabIndex = 2;
			this.chkALL.Text = "Send To ALL Sites";
			this.chkALL.UseVisualStyleBackColor = true;
			this.chkALL.CheckedChanged += new System.EventHandler(this.chkALL_CheckedChanged);
			// 
			// txtMSG
			// 
			this.txtMSG.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMSG.Location = new System.Drawing.Point(20, 32);
			this.txtMSG.Multiline = true;
			this.txtMSG.Name = "txtMSG";
			this.txtMSG.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtMSG.Size = new System.Drawing.Size(831, 66);
			this.txtMSG.TabIndex = 1;
			// 
			// lblMsg
			// 
			this.lblMsg.AutoSize = true;
			this.lblMsg.Location = new System.Drawing.Point(17, 16);
			this.lblMsg.Name = "lblMsg";
			this.lblMsg.Size = new System.Drawing.Size(78, 13);
			this.lblMsg.TabIndex = 0;
			this.lblMsg.Text = "New Message:";
			// 
			// dgvMsgh
			// 
			this.dgvMsgh.AllowUserToAddRows = false;
			this.dgvMsgh.AllowUserToDeleteRows = false;
			this.dgvMsgh.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dgvMsgh.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMsgh.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHdate,
            this.colHSite,
            this.colHMsg});
			this.dgvMsgh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMsgh.Location = new System.Drawing.Point(3, 48);
			this.dgvMsgh.MultiSelect = false;
			this.dgvMsgh.Name = "dgvMsgh";
			this.dgvMsgh.ReadOnly = true;
			this.dgvMsgh.RowHeadersVisible = false;
			this.dgvMsgh.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvMsgh.Size = new System.Drawing.Size(709, 155);
			this.dgvMsgh.TabIndex = 5;
			// 
			// pnTop
			// 
			this.tlpMain.SetColumnSpan(this.pnTop, 2);
			this.pnTop.Controls.Add(this.btnNext);
			this.pnTop.Controls.Add(this.btnDEL);
			this.pnTop.Controls.Add(this.btnPrev);
			this.pnTop.Controls.Add(this.btnClearAll);
			this.pnTop.Controls.Add(this.btnLast);
			this.pnTop.Controls.Add(this.btnFirst);
			this.pnTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnTop.Location = new System.Drawing.Point(0, 0);
			this.pnTop.Margin = new System.Windows.Forms.Padding(0);
			this.pnTop.Name = "pnTop";
			this.pnTop.Size = new System.Drawing.Size(865, 45);
			this.pnTop.TabIndex = 3;
			// 
			// btnNext
			// 
			this.btnNext.Location = new System.Drawing.Point(113, 3);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(38, 38);
			this.btnNext.TabIndex = 3;
			this.btnNext.Text = ">";
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// btnDEL
			// 
			this.btnDEL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDEL.Location = new System.Drawing.Point(692, 3);
			this.btnDEL.Name = "btnDEL";
			this.btnDEL.Size = new System.Drawing.Size(82, 38);
			this.btnDEL.TabIndex = 5;
			this.btnDEL.Text = "Delete Message";
			this.btnDEL.UseVisualStyleBackColor = true;
			this.btnDEL.Click += new System.EventHandler(this.btnDEL_Click);
			// 
			// btnPrev
			// 
			this.btnPrev.Location = new System.Drawing.Point(69, 3);
			this.btnPrev.Name = "btnPrev";
			this.btnPrev.Size = new System.Drawing.Size(38, 38);
			this.btnPrev.TabIndex = 2;
			this.btnPrev.Text = "<";
			this.btnPrev.UseVisualStyleBackColor = true;
			this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
			// 
			// btnClearAll
			// 
			this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearAll.Location = new System.Drawing.Point(780, 3);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Size = new System.Drawing.Size(82, 38);
			this.btnClearAll.TabIndex = 6;
			this.btnClearAll.Text = "Clear All";
			this.btnClearAll.UseVisualStyleBackColor = true;
			this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
			// 
			// btnLast
			// 
			this.btnLast.Location = new System.Drawing.Point(166, 3);
			this.btnLast.Name = "btnLast";
			this.btnLast.Size = new System.Drawing.Size(38, 38);
			this.btnLast.TabIndex = 4;
			this.btnLast.Text = ">|";
			this.btnLast.UseVisualStyleBackColor = true;
			this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
			// 
			// btnFirst
			// 
			this.btnFirst.Location = new System.Drawing.Point(14, 3);
			this.btnFirst.Name = "btnFirst";
			this.btnFirst.Size = new System.Drawing.Size(38, 38);
			this.btnFirst.TabIndex = 1;
			this.btnFirst.Text = "|<";
			this.btnFirst.UseVisualStyleBackColor = true;
			this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
			// 
			// dgvMsg1
			// 
			this.dgvMsg1.AllowUserToAddRows = false;
			this.dgvMsg1.AllowUserToDeleteRows = false;
			this.dgvMsg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMsg1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTSite});
			this.dgvMsg1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMsg1.Location = new System.Drawing.Point(718, 48);
			this.dgvMsg1.Name = "dgvMsg1";
			this.dgvMsg1.ReadOnly = true;
			this.dgvMsg1.RowHeadersVisible = false;
			this.dgvMsg1.Size = new System.Drawing.Size(144, 155);
			this.dgvMsg1.TabIndex = 4;
			// 
			// colTSite
			// 
			this.colTSite.DataPropertyName = "sitenum";
			this.colTSite.HeaderText = "To Site";
			this.colTSite.Name = "colTSite";
			this.colTSite.ReadOnly = true;
			this.colTSite.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			// 
			// colHdate
			// 
			this.colHdate.DataPropertyName = "created";
			this.colHdate.HeaderText = "DateTime";
			this.colHdate.Name = "colHdate";
			this.colHdate.ReadOnly = true;
			this.colHdate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			this.colHdate.Width = 120;
			// 
			// colHSite
			// 
			this.colHSite.DataPropertyName = "SourceSite";
			this.colHSite.HeaderText = "From";
			this.colHSite.Name = "colHSite";
			this.colHSite.ReadOnly = true;
			this.colHSite.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			this.colHSite.Width = 50;
			// 
			// colHMsg
			// 
			this.colHMsg.DataPropertyName = "Msg";
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.colHMsg.DefaultCellStyle = dataGridViewCellStyle2;
			this.colHMsg.HeaderText = "Message";
			this.colHMsg.Name = "colHMsg";
			this.colHMsg.ReadOnly = true;
			this.colHMsg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			this.colHMsg.Width = 500;
			// 
			// BMSG
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(865, 556);
			this.Controls.Add(this.tlpMain);
			this.Name = "BMSG";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Broadcast Message";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.BMSG_Load);
			this.tlpMain.ResumeLayout(false);
			this.pnMSG.ResumeLayout(false);
			this.pnMSG.PerformLayout();
			this.pnSelection.ResumeLayout(false);
			this.pnSelection.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSites)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSelected)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMsgh)).EndInit();
			this.pnTop.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvMsg1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlpMain;
		private System.Windows.Forms.Button btnClearAll;
		private System.Windows.Forms.Button btnDEL;
		private System.Windows.Forms.Panel pnTop;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Button btnPrev;
		private System.Windows.Forms.Button btnLast;
		private System.Windows.Forms.Button btnFirst;
		private System.Windows.Forms.Panel pnMSG;
		private System.Windows.Forms.TextBox txtMSG;
		private System.Windows.Forms.DataGridView dgvSites;
		private System.Windows.Forms.Label lblMsg;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnSubmit;
		private System.Windows.Forms.CheckBox chkALL;
		private System.Windows.Forms.Label lblSelected;
		private System.Windows.Forms.Label lblAvail;
		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.DataGridView dgvSelected;
		private System.Windows.Forms.DataGridView dgvMsg1;
		private System.Windows.Forms.DataGridView dgvMsgh;
		private System.Windows.Forms.Button btnUnSelect;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTSite;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSelectedSite;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSName;
		private System.Windows.Forms.Panel pnSelection;
		private System.Windows.Forms.DataGridViewTextBoxColumn colHdate;
		private System.Windows.Forms.DataGridViewTextBoxColumn colHSite;
		private System.Windows.Forms.DataGridViewTextBoxColumn colHMsg;
	}
}