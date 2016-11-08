namespace ATL.ExtractWORpics1
{
    partial class ExtractWORpics1
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
            this.PhotoDGV1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnPicture = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uploaded = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.photourl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.PhotoDGV1)).BeginInit();
            this.SuspendLayout();
            // 
            // PhotoDGV1
            // 
            this.PhotoDGV1.AllowUserToOrderColumns = true;
            this.PhotoDGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PhotoDGV1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.btnPicture,
            this.Remark,
            this.Uploaded,
            this.photourl,
            this.ID,
            this.Delete});
            this.PhotoDGV1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PhotoDGV1.Location = new System.Drawing.Point(0, 0);
            this.PhotoDGV1.Name = "PhotoDGV1";
            this.PhotoDGV1.Size = new System.Drawing.Size(636, 266);
            this.PhotoDGV1.TabIndex = 0;
            this.PhotoDGV1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PhotoDGV1_CellContentClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Remarks";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 300;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "created";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "photourl";
            this.dataGridViewTextBoxColumn3.MaxInputLength = 300000;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Visible = false;
            this.dataGridViewTextBoxColumn3.Width = 200;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "ID";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // btnPicture
            // 
            this.btnPicture.HeaderText = "Photo";
            this.btnPicture.Name = "btnPicture";
            this.btnPicture.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.btnPicture.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.btnPicture.Text = "View";
            this.btnPicture.ToolTipText = "View Picture";
            this.btnPicture.UseColumnTextForButtonValue = true;
            this.btnPicture.Width = 50;
            // 
            // Remark
            // 
            this.Remark.HeaderText = "Remarks";
            this.Remark.Name = "Remark";
            this.Remark.ReadOnly = true;
            this.Remark.Width = 300;
            // 
            // Uploaded
            // 
            this.Uploaded.HeaderText = "created";
            this.Uploaded.Name = "Uploaded";
            this.Uploaded.ReadOnly = true;
            // 
            // photourl
            // 
            this.photourl.HeaderText = "photourl";
            this.photourl.MaxInputLength = 300000;
            this.photourl.Name = "photourl";
            this.photourl.ReadOnly = true;
            this.photourl.Visible = false;
            this.photourl.Width = 200;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Remove";
            this.Delete.Name = "Delete";
            this.Delete.Text = "Delete";
            this.Delete.ToolTipText = "Delete Phto";
            this.Delete.UseColumnTextForButtonValue = true;
            // 
            // ExtractWORpics1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 266);
            this.Controls.Add(this.PhotoDGV1);
            this.Name = "ExtractWORpics1";
            this.Text = "Pictures";
            this.Load += new System.EventHandler(this.ExtractWORpics1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PhotoDGV1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView PhotoDGV1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewButtonColumn btnPicture;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remark;
        private System.Windows.Forms.DataGridViewTextBoxColumn Uploaded;
        private System.Windows.Forms.DataGridViewTextBoxColumn photourl;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
    }
}