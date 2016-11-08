namespace ATL.BizModules.CompressFolders
{
    partial class Decommpress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Decommpress));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.gbxDestination = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveBrowse = new System.Windows.Forms.Button();
            this.txtSaveTo = new System.Windows.Forms.TextBox();
            this.lblSaveTo = new System.Windows.Forms.Label();
            this.gbxExit = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.gbxDestination.SuspendLayout();
            this.gbxExit.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // gbxDestination
            // 
            this.gbxDestination.Controls.Add(this.btnSave);
            this.gbxDestination.Controls.Add(this.btnSaveBrowse);
            this.gbxDestination.Controls.Add(this.txtSaveTo);
            this.gbxDestination.Controls.Add(this.lblSaveTo);
            this.gbxDestination.Location = new System.Drawing.Point(12, 12);
            this.gbxDestination.Name = "gbxDestination";
            this.gbxDestination.Size = new System.Drawing.Size(407, 79);
            this.gbxDestination.TabIndex = 1;
            this.gbxDestination.TabStop = false;
            this.gbxDestination.Text = "Save Compressed Folder";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(314, 46);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveBrowse
            // 
            this.btnSaveBrowse.Location = new System.Drawing.Point(314, 17);
            this.btnSaveBrowse.Name = "btnSaveBrowse";
            this.btnSaveBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnSaveBrowse.TabIndex = 5;
            this.btnSaveBrowse.Text = "Browse...";
            this.btnSaveBrowse.UseVisualStyleBackColor = true;
            this.btnSaveBrowse.Click += new System.EventHandler(this.btnSaveBrowse_Click);
            // 
            // txtSaveTo
            // 
            this.txtSaveTo.Location = new System.Drawing.Point(54, 19);
            this.txtSaveTo.Name = "txtSaveTo";
            this.txtSaveTo.Size = new System.Drawing.Size(254, 20);
            this.txtSaveTo.TabIndex = 4;
            // 
            // lblSaveTo
            // 
            this.lblSaveTo.AutoSize = true;
            this.lblSaveTo.Location = new System.Drawing.Point(2, 22);
            this.lblSaveTo.Name = "lblSaveTo";
            this.lblSaveTo.Size = new System.Drawing.Size(51, 13);
            this.lblSaveTo.TabIndex = 3;
            this.lblSaveTo.Text = "Save To:";
            // 
            // gbxExit
            // 
            this.gbxExit.Controls.Add(this.btnExit);
            this.gbxExit.Location = new System.Drawing.Point(12, 95);
            this.gbxExit.Name = "gbxExit";
            this.gbxExit.Size = new System.Drawing.Size(407, 54);
            this.gbxExit.TabIndex = 2;
            this.gbxExit.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(314, 19);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "E&xit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // Decommpress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 165);
            this.Controls.Add(this.gbxExit);
            this.Controls.Add(this.gbxDestination);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Decommpress";
            this.Text = "Save File";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbxDestination.ResumeLayout(false);
            this.gbxDestination.PerformLayout();
            this.gbxExit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox gbxDestination;
        private System.Windows.Forms.GroupBox gbxExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveBrowse;
        private System.Windows.Forms.TextBox txtSaveTo;
        private System.Windows.Forms.Label lblSaveTo;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

