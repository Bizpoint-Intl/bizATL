namespace ATL.InventoryQuery
{
    partial class frmInventoryQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInventoryQuery));
            this.dgQuery = new System.Windows.Forms.DataGridView();
            this.trqnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trqfrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trqto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trqqty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trqtrandate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tranum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trafrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trato = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.traqty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tratrandate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trinum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trifrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trito = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.triqty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tritrandate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpfilter = new System.Windows.Forms.GroupBox();
            this.grpStockBal = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dt_Cutoff = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.lblwarehouse = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.rad_StkBal = new System.Windows.Forms.RadioButton();
            this.rad_Transfer = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnEnquiry = new System.Windows.Forms.Button();
            this.lblSite = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblbarcode = new System.Windows.Forms.Label();
            this.lblmainstyle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgStkBal = new System.Windows.Forms.DataGridView();
            this.matnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.whnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balqty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pbrdname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pcatname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cboCategory = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboBrand = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboMatname = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboWarehouse = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboLocation = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboSiteFrom_TRI = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboSiteTo_TRI = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboSiteFrom_TRA = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboSiteTo_TRA = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboSite = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboSiteFrom_TRQ = new ATL.MultiColumnComboBox.ColumnComboBox();
            this.cboSiteTo_TRQ = new ATL.MultiColumnComboBox.ColumnComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgQuery)).BeginInit();
            this.grpfilter.SuspendLayout();
            this.grpStockBal.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStkBal)).BeginInit();
            this.SuspendLayout();
            // 
            // dgQuery
            // 
            this.dgQuery.AllowUserToAddRows = false;
            this.dgQuery.AllowUserToDeleteRows = false;
            this.dgQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgQuery.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.trqnum,
            this.trqfrom,
            this.trqto,
            this.trqqty,
            this.trqtrandate,
            this.tranum,
            this.trafrom,
            this.trato,
            this.traqty,
            this.tratrandate,
            this.trinum,
            this.trifrom,
            this.trito,
            this.triqty,
            this.tritrandate});
            this.dgQuery.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgQuery.Location = new System.Drawing.Point(0, 195);
            this.dgQuery.Name = "dgQuery";
            this.dgQuery.ReadOnly = true;
            this.dgQuery.Size = new System.Drawing.Size(1161, 549);
            this.dgQuery.TabIndex = 8;
            // 
            // trqnum
            // 
            this.trqnum.DataPropertyName = "trqnum";
            this.trqnum.HeaderText = "TRQ-Ref";
            this.trqnum.Name = "trqnum";
            this.trqnum.ReadOnly = true;
            this.trqnum.Width = 130;
            // 
            // trqfrom
            // 
            this.trqfrom.DataPropertyName = "trqfrom";
            this.trqfrom.HeaderText = "TRQ-From";
            this.trqfrom.Name = "trqfrom";
            this.trqfrom.ReadOnly = true;
            this.trqfrom.Width = 60;
            // 
            // trqto
            // 
            this.trqto.DataPropertyName = "trqto";
            this.trqto.HeaderText = "TRQ-TO";
            this.trqto.Name = "trqto";
            this.trqto.ReadOnly = true;
            this.trqto.Width = 50;
            // 
            // trqqty
            // 
            this.trqqty.DataPropertyName = "trqqty";
            this.trqqty.HeaderText = "TRQ-Qty";
            this.trqqty.Name = "trqqty";
            this.trqqty.ReadOnly = true;
            this.trqqty.Width = 60;
            // 
            // trqtrandate
            // 
            this.trqtrandate.DataPropertyName = "trqtrandate";
            this.trqtrandate.HeaderText = "TRQ-Trandate";
            this.trqtrandate.Name = "trqtrandate";
            this.trqtrandate.ReadOnly = true;
            this.trqtrandate.Width = 90;
            // 
            // tranum
            // 
            this.tranum.DataPropertyName = "tranum";
            this.tranum.HeaderText = "TRA-Ref";
            this.tranum.Name = "tranum";
            this.tranum.ReadOnly = true;
            this.tranum.Width = 130;
            // 
            // trafrom
            // 
            this.trafrom.DataPropertyName = "trafrom";
            this.trafrom.HeaderText = "TRA-From";
            this.trafrom.Name = "trafrom";
            this.trafrom.ReadOnly = true;
            this.trafrom.Width = 60;
            // 
            // trato
            // 
            this.trato.DataPropertyName = "trato";
            this.trato.HeaderText = "TRA-TO";
            this.trato.Name = "trato";
            this.trato.ReadOnly = true;
            this.trato.Width = 50;
            // 
            // traqty
            // 
            this.traqty.DataPropertyName = "traqty";
            this.traqty.HeaderText = "TRA-Qty";
            this.traqty.Name = "traqty";
            this.traqty.ReadOnly = true;
            this.traqty.Width = 60;
            // 
            // tratrandate
            // 
            this.tratrandate.DataPropertyName = "tratrandate";
            this.tratrandate.HeaderText = "TRA-Trandate";
            this.tratrandate.Name = "tratrandate";
            this.tratrandate.ReadOnly = true;
            this.tratrandate.Width = 90;
            // 
            // trinum
            // 
            this.trinum.DataPropertyName = "trinum";
            this.trinum.HeaderText = "TRI-Ref";
            this.trinum.Name = "trinum";
            this.trinum.ReadOnly = true;
            this.trinum.Width = 130;
            // 
            // trifrom
            // 
            this.trifrom.DataPropertyName = "trifrom";
            this.trifrom.HeaderText = "TRI-From";
            this.trifrom.Name = "trifrom";
            this.trifrom.ReadOnly = true;
            this.trifrom.Width = 60;
            // 
            // trito
            // 
            this.trito.DataPropertyName = "trito";
            this.trito.HeaderText = "TRI-TO";
            this.trito.Name = "trito";
            this.trito.ReadOnly = true;
            this.trito.Width = 50;
            // 
            // triqty
            // 
            this.triqty.DataPropertyName = "triqty";
            this.triqty.HeaderText = "TRI-Qty";
            this.triqty.Name = "triqty";
            this.triqty.ReadOnly = true;
            this.triqty.Width = 60;
            // 
            // tritrandate
            // 
            this.tritrandate.DataPropertyName = "tritrandate";
            this.tritrandate.HeaderText = "TRI-Trandate";
            this.tritrandate.Name = "tritrandate";
            this.tritrandate.ReadOnly = true;
            this.tritrandate.Width = 90;
            // 
            // grpfilter
            // 
            this.grpfilter.Controls.Add(this.grpStockBal);
            this.grpfilter.Controls.Add(this.rad_StkBal);
            this.grpfilter.Controls.Add(this.rad_Transfer);
            this.grpfilter.Controls.Add(this.groupBox3);
            this.grpfilter.Controls.Add(this.btnEnquiry);
            this.grpfilter.Controls.Add(this.lblSite);
            this.grpfilter.Controls.Add(this.groupBox2);
            this.grpfilter.Controls.Add(this.cboSite);
            this.grpfilter.Controls.Add(this.groupBox1);
            this.grpfilter.Location = new System.Drawing.Point(3, 0);
            this.grpfilter.Name = "grpfilter";
            this.grpfilter.Size = new System.Drawing.Size(1023, 132);
            this.grpfilter.TabIndex = 0;
            this.grpfilter.TabStop = false;
            this.grpfilter.Text = "Filtering";
            // 
            // grpStockBal
            // 
            this.grpStockBal.Controls.Add(this.cboCategory);
            this.grpStockBal.Controls.Add(this.cboBrand);
            this.grpStockBal.Controls.Add(this.label8);
            this.grpStockBal.Controls.Add(this.label9);
            this.grpStockBal.Controls.Add(this.cboMatname);
            this.grpStockBal.Controls.Add(this.label7);
            this.grpStockBal.Controls.Add(this.dt_Cutoff);
            this.grpStockBal.Controls.Add(this.label6);
            this.grpStockBal.Controls.Add(this.cboWarehouse);
            this.grpStockBal.Controls.Add(this.cboLocation);
            this.grpStockBal.Controls.Add(this.lblwarehouse);
            this.grpStockBal.Controls.Add(this.lblLocation);
            this.grpStockBal.Location = new System.Drawing.Point(213, 20);
            this.grpStockBal.Name = "grpStockBal";
            this.grpStockBal.Size = new System.Drawing.Size(763, 100);
            this.grpStockBal.TabIndex = 39;
            this.grpStockBal.TabStop = false;
            this.grpStockBal.Text = "StockBal";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(511, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "Category";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(511, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 43;
            this.label9.Text = "Brand";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(215, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "MaterialName";
            // 
            // dt_Cutoff
            // 
            this.dt_Cutoff.Location = new System.Drawing.Point(294, 24);
            this.dt_Cutoff.Name = "dt_Cutoff";
            this.dt_Cutoff.Size = new System.Drawing.Size(198, 21);
            this.dt_Cutoff.TabIndex = 40;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(215, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "CutOffDate";
            // 
            // lblwarehouse
            // 
            this.lblwarehouse.AutoSize = true;
            this.lblwarehouse.Location = new System.Drawing.Point(5, 54);
            this.lblwarehouse.Name = "lblwarehouse";
            this.lblwarehouse.Size = new System.Drawing.Size(62, 13);
            this.lblwarehouse.TabIndex = 16;
            this.lblwarehouse.Text = "Warehouse";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(5, 25);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(47, 13);
            this.lblLocation.TabIndex = 37;
            this.lblLocation.Text = "Location";
            // 
            // rad_StkBal
            // 
            this.rad_StkBal.AutoSize = true;
            this.rad_StkBal.Location = new System.Drawing.Point(12, 69);
            this.rad_StkBal.Name = "rad_StkBal";
            this.rad_StkBal.Size = new System.Drawing.Size(88, 17);
            this.rad_StkBal.TabIndex = 36;
            this.rad_StkBal.TabStop = true;
            this.rad_StkBal.Text = "StockBalance";
            this.rad_StkBal.UseVisualStyleBackColor = true;
            this.rad_StkBal.Click += new System.EventHandler(this.rad_StkBal_Click);
            // 
            // rad_Transfer
            // 
            this.rad_Transfer.AutoSize = true;
            this.rad_Transfer.Location = new System.Drawing.Point(12, 45);
            this.rad_Transfer.Name = "rad_Transfer";
            this.rad_Transfer.Size = new System.Drawing.Size(93, 17);
            this.rad_Transfer.TabIndex = 35;
            this.rad_Transfer.TabStop = true;
            this.rad_Transfer.Text = "TransferFlows";
            this.rad_Transfer.UseVisualStyleBackColor = true;
            this.rad_Transfer.Click += new System.EventHandler(this.rad_Transfer_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cboSiteFrom_TRI);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.cboSiteTo_TRI);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(844, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(173, 100);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Transfer In";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "SiteFrom:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "SiteTo:";
            // 
            // btnEnquiry
            // 
            this.btnEnquiry.Location = new System.Drawing.Point(9, 94);
            this.btnEnquiry.Name = "btnEnquiry";
            this.btnEnquiry.Size = new System.Drawing.Size(82, 32);
            this.btnEnquiry.TabIndex = 7;
            this.btnEnquiry.Text = "Enquiry";
            this.btnEnquiry.UseVisualStyleBackColor = true;
            this.btnEnquiry.Click += new System.EventHandler(this.btnEnquiry_Click);
            // 
            // lblSite
            // 
            this.lblSite.AutoSize = true;
            this.lblSite.Location = new System.Drawing.Point(5, 22);
            this.lblSite.Name = "lblSite";
            this.lblSite.Size = new System.Drawing.Size(25, 13);
            this.lblSite.TabIndex = 18;
            this.lblSite.Text = "Site";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboSiteFrom_TRA);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cboSiteTo_TRA);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(624, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Transfer Agreement";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "SiteTo:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "SiteFrom:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboSiteFrom_TRQ);
            this.groupBox1.Controls.Add(this.lblbarcode);
            this.groupBox1.Controls.Add(this.lblmainstyle);
            this.groupBox1.Controls.Add(this.cboSiteTo_TRQ);
            this.groupBox1.Location = new System.Drawing.Point(431, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 100);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transfer Request";
            // 
            // lblbarcode
            // 
            this.lblbarcode.AutoSize = true;
            this.lblbarcode.Location = new System.Drawing.Point(28, 61);
            this.lblbarcode.Name = "lblbarcode";
            this.lblbarcode.Size = new System.Drawing.Size(41, 13);
            this.lblbarcode.TabIndex = 13;
            this.lblbarcode.Text = "SiteTo:";
            // 
            // lblmainstyle
            // 
            this.lblmainstyle.AutoSize = true;
            this.lblmainstyle.Location = new System.Drawing.Point(17, 29);
            this.lblmainstyle.Name = "lblmainstyle";
            this.lblmainstyle.Size = new System.Drawing.Size(53, 13);
            this.lblmainstyle.TabIndex = 6;
            this.lblmainstyle.Text = "SiteFrom:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 16);
            this.label1.TabIndex = 21;
            this.label1.Text = "* Press Escape Key for Clear Filtering";
            // 
            // dgStkBal
            // 
            this.dgStkBal.AllowUserToAddRows = false;
            this.dgStkBal.AllowUserToDeleteRows = false;
            this.dgStkBal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStkBal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.matnum,
            this.detail,
            this.location,
            this.whnum,
            this.balqty,
            this.pbrdname,
            this.pcatname});
            this.dgStkBal.Location = new System.Drawing.Point(0, 165);
            this.dgStkBal.Name = "dgStkBal";
            this.dgStkBal.ReadOnly = true;
            this.dgStkBal.Size = new System.Drawing.Size(866, 549);
            this.dgStkBal.TabIndex = 22;
            this.dgStkBal.Visible = false;
            // 
            // matnum
            // 
            this.matnum.DataPropertyName = "matnum";
            this.matnum.HeaderText = "MaterialCode";
            this.matnum.Name = "matnum";
            this.matnum.ReadOnly = true;
            this.matnum.Width = 120;
            // 
            // detail
            // 
            this.detail.DataPropertyName = "detail";
            this.detail.HeaderText = "MaterialName";
            this.detail.Name = "detail";
            this.detail.ReadOnly = true;
            this.detail.Width = 200;
            // 
            // location
            // 
            this.location.DataPropertyName = "location";
            this.location.HeaderText = "Location";
            this.location.Name = "location";
            this.location.ReadOnly = true;
            // 
            // whnum
            // 
            this.whnum.DataPropertyName = "whnum";
            this.whnum.HeaderText = "Warehouse";
            this.whnum.Name = "whnum";
            this.whnum.ReadOnly = true;
            // 
            // balqty
            // 
            this.balqty.DataPropertyName = "balqty";
            this.balqty.HeaderText = "BalQty";
            this.balqty.Name = "balqty";
            this.balqty.ReadOnly = true;
            // 
            // pbrdname
            // 
            this.pbrdname.DataPropertyName = "pbrdname";
            this.pbrdname.HeaderText = "Brand";
            this.pbrdname.Name = "pbrdname";
            this.pbrdname.ReadOnly = true;
            // 
            // pcatname
            // 
            this.pcatname.DataPropertyName = "pcatname";
            this.pcatname.HeaderText = "Category";
            this.pcatname.Name = "pcatname";
            this.pcatname.ReadOnly = true;
            // 
            // cboCategory
            // 
            this.cboCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboCategory.DropDownWidth = 17;
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Location = new System.Drawing.Point(566, 49);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(121, 22);
            this.cboCategory.TabIndex = 46;
            this.cboCategory.ViewColumn = 0;
            // 
            // cboBrand
            // 
            this.cboBrand.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboBrand.DropDownWidth = 17;
            this.cboBrand.FormattingEnabled = true;
            this.cboBrand.Location = new System.Drawing.Point(566, 21);
            this.cboBrand.Name = "cboBrand";
            this.cboBrand.Size = new System.Drawing.Size(121, 22);
            this.cboBrand.TabIndex = 45;
            this.cboBrand.ViewColumn = 0;
            // 
            // cboMatname
            // 
            this.cboMatname.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboMatname.DropDownWidth = 17;
            this.cboMatname.FormattingEnabled = true;
            this.cboMatname.Location = new System.Drawing.Point(294, 49);
            this.cboMatname.Name = "cboMatname";
            this.cboMatname.Size = new System.Drawing.Size(198, 22);
            this.cboMatname.TabIndex = 42;
            this.cboMatname.ViewColumn = 0;
            // 
            // cboWarehouse
            // 
            this.cboWarehouse.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboWarehouse.DropDownWidth = 17;
            this.cboWarehouse.FormattingEnabled = true;
            this.cboWarehouse.Location = new System.Drawing.Point(68, 50);
            this.cboWarehouse.Name = "cboWarehouse";
            this.cboWarehouse.Size = new System.Drawing.Size(126, 22);
            this.cboWarehouse.TabIndex = 5;
            this.cboWarehouse.ViewColumn = 0;
            // 
            // cboLocation
            // 
            this.cboLocation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboLocation.DropDownWidth = 17;
            this.cboLocation.FormattingEnabled = true;
            this.cboLocation.Location = new System.Drawing.Point(68, 23);
            this.cboLocation.Name = "cboLocation";
            this.cboLocation.Size = new System.Drawing.Size(126, 22);
            this.cboLocation.TabIndex = 38;
            this.cboLocation.ViewColumn = 0;
            // 
            // cboSiteFrom_TRI
            // 
            this.cboSiteFrom_TRI.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboSiteFrom_TRI.DropDownWidth = 17;
            this.cboSiteFrom_TRI.FormattingEnabled = true;
            this.cboSiteFrom_TRI.Location = new System.Drawing.Point(62, 29);
            this.cboSiteFrom_TRI.Name = "cboSiteFrom_TRI";
            this.cboSiteFrom_TRI.Size = new System.Drawing.Size(84, 22);
            this.cboSiteFrom_TRI.TabIndex = 23;
            this.cboSiteFrom_TRI.ViewColumn = 0;
            // 
            // cboSiteTo_TRI
            // 
            this.cboSiteTo_TRI.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboSiteTo_TRI.DropDownWidth = 17;
            this.cboSiteTo_TRI.FormattingEnabled = true;
            this.cboSiteTo_TRI.Location = new System.Drawing.Point(62, 60);
            this.cboSiteTo_TRI.Name = "cboSiteTo_TRI";
            this.cboSiteTo_TRI.Size = new System.Drawing.Size(84, 22);
            this.cboSiteTo_TRI.TabIndex = 24;
            this.cboSiteTo_TRI.ViewColumn = 0;
            // 
            // cboSiteFrom_TRA
            // 
            this.cboSiteFrom_TRA.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboSiteFrom_TRA.DropDownWidth = 17;
            this.cboSiteFrom_TRA.FormattingEnabled = true;
            this.cboSiteFrom_TRA.Location = new System.Drawing.Point(93, 29);
            this.cboSiteFrom_TRA.Name = "cboSiteFrom_TRA";
            this.cboSiteFrom_TRA.Size = new System.Drawing.Size(84, 22);
            this.cboSiteFrom_TRA.TabIndex = 19;
            this.cboSiteFrom_TRA.ViewColumn = 0;
            // 
            // cboSiteTo_TRA
            // 
            this.cboSiteTo_TRA.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboSiteTo_TRA.DropDownWidth = 17;
            this.cboSiteTo_TRA.FormattingEnabled = true;
            this.cboSiteTo_TRA.Location = new System.Drawing.Point(93, 60);
            this.cboSiteTo_TRA.Name = "cboSiteTo_TRA";
            this.cboSiteTo_TRA.Size = new System.Drawing.Size(84, 22);
            this.cboSiteTo_TRA.TabIndex = 20;
            this.cboSiteTo_TRA.ViewColumn = 0;
            // 
            // cboSite
            // 
            this.cboSite.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboSite.DropDownWidth = 17;
            this.cboSite.FormattingEnabled = true;
            this.cboSite.Location = new System.Drawing.Point(58, 17);
            this.cboSite.Name = "cboSite";
            this.cboSite.Size = new System.Drawing.Size(126, 22);
            this.cboSite.TabIndex = 6;
            this.cboSite.ViewColumn = 0;
            // 
            // cboSiteFrom_TRQ
            // 
            this.cboSiteFrom_TRQ.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboSiteFrom_TRQ.DropDownWidth = 17;
            this.cboSiteFrom_TRQ.FormattingEnabled = true;
            this.cboSiteFrom_TRQ.Location = new System.Drawing.Point(71, 25);
            this.cboSiteFrom_TRQ.Name = "cboSiteFrom_TRQ";
            this.cboSiteFrom_TRQ.Size = new System.Drawing.Size(84, 22);
            this.cboSiteFrom_TRQ.TabIndex = 1;
            this.cboSiteFrom_TRQ.ViewColumn = 0;
            // 
            // cboSiteTo_TRQ
            // 
            this.cboSiteTo_TRQ.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cboSiteTo_TRQ.DropDownWidth = 17;
            this.cboSiteTo_TRQ.FormattingEnabled = true;
            this.cboSiteTo_TRQ.Location = new System.Drawing.Point(71, 56);
            this.cboSiteTo_TRQ.Name = "cboSiteTo_TRQ";
            this.cboSiteTo_TRQ.Size = new System.Drawing.Size(84, 22);
            this.cboSiteTo_TRQ.TabIndex = 4;
            this.cboSiteTo_TRQ.ViewColumn = 0;
            // 
            // frmInventoryQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1161, 744);
            this.Controls.Add(this.dgStkBal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpfilter);
            this.Controls.Add(this.dgQuery);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmInventoryQuery";
            this.Text = "Inventory Query Window";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmInventoryQuery_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgQuery)).EndInit();
            this.grpfilter.ResumeLayout(false);
            this.grpfilter.PerformLayout();
            this.grpStockBal.ResumeLayout(false);
            this.grpStockBal.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStkBal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgQuery;
        private System.Windows.Forms.GroupBox grpfilter;
        private System.Windows.Forms.Label lblmainstyle;
        private ATL.MultiColumnComboBox.ColumnComboBox cboSiteFrom_TRQ;
        private System.Windows.Forms.Label lblbarcode;
        private ATL.MultiColumnComboBox.ColumnComboBox cboSiteTo_TRQ;
        private System.Windows.Forms.Label lblwarehouse;
        private ATL.MultiColumnComboBox.ColumnComboBox cboWarehouse;
        private System.Windows.Forms.Button btnEnquiry;
        private System.Windows.Forms.Label lblSite;
        private ATL.MultiColumnComboBox.ColumnComboBox cboSite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private ATL.MultiColumnComboBox.ColumnComboBox cboSiteTo_TRI;
        private System.Windows.Forms.Label label5;
        private ATL.MultiColumnComboBox.ColumnComboBox cboSiteFrom_TRI;
        private System.Windows.Forms.Label label2;
        private ATL.MultiColumnComboBox.ColumnComboBox cboSiteTo_TRA;
        private System.Windows.Forms.Label label3;
        private ATL.MultiColumnComboBox.ColumnComboBox cboSiteFrom_TRA;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rad_StkBal;
        private System.Windows.Forms.RadioButton rad_Transfer;
        private System.Windows.Forms.DataGridView dgStkBal;
        private System.Windows.Forms.DataGridViewTextBoxColumn trqnum;
        private System.Windows.Forms.DataGridViewTextBoxColumn trqfrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn trqto;
        private System.Windows.Forms.DataGridViewTextBoxColumn trqqty;
        private System.Windows.Forms.DataGridViewTextBoxColumn trqtrandate;
        private System.Windows.Forms.DataGridViewTextBoxColumn tranum;
        private System.Windows.Forms.DataGridViewTextBoxColumn trafrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn trato;
        private System.Windows.Forms.DataGridViewTextBoxColumn traqty;
        private System.Windows.Forms.DataGridViewTextBoxColumn tratrandate;
        private System.Windows.Forms.DataGridViewTextBoxColumn trinum;
        private System.Windows.Forms.DataGridViewTextBoxColumn trifrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn trito;
        private System.Windows.Forms.DataGridViewTextBoxColumn triqty;
        private System.Windows.Forms.DataGridViewTextBoxColumn tritrandate;
        private System.Windows.Forms.Label lblLocation;
        private ATL.MultiColumnComboBox.ColumnComboBox cboLocation;
        private System.Windows.Forms.GroupBox grpStockBal;
        private System.Windows.Forms.DateTimePicker dt_Cutoff;
        private System.Windows.Forms.Label label6;
        private ATL.MultiColumnComboBox.ColumnComboBox cboMatname;
        private System.Windows.Forms.Label label7;
        private ATL.MultiColumnComboBox.ColumnComboBox cboCategory;
        private ATL.MultiColumnComboBox.ColumnComboBox cboBrand;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridViewTextBoxColumn matnum;
        private System.Windows.Forms.DataGridViewTextBoxColumn detail;
        private System.Windows.Forms.DataGridViewTextBoxColumn location;
        private System.Windows.Forms.DataGridViewTextBoxColumn whnum;
        private System.Windows.Forms.DataGridViewTextBoxColumn balqty;
        private System.Windows.Forms.DataGridViewTextBoxColumn pbrdname;
        private System.Windows.Forms.DataGridViewTextBoxColumn pcatname;
    }
}