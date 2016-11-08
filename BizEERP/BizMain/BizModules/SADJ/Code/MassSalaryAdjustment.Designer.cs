namespace ATL.BizModules.SADJ
{
    partial class MassSalaryAdjustment
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
            this.dateAdjustPicker1 = new System.Windows.Forms.DateTimePicker();
            this.importDV1 = new System.Windows.Forms.DataGridView();
            this.btn_Import = new System.Windows.Forms.Button();
            this.btn_Confirm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.importDV1)).BeginInit();
            this.SuspendLayout();
            // 
            // dateAdjustPicker1
            // 
            this.dateAdjustPicker1.Location = new System.Drawing.Point(112, 13);
            this.dateAdjustPicker1.Name = "dateAdjustPicker1";
            this.dateAdjustPicker1.Size = new System.Drawing.Size(200, 20);
            this.dateAdjustPicker1.TabIndex = 0;
            // 
            // importDV1
            // 
            this.importDV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.importDV1.Location = new System.Drawing.Point(34, 76);
            this.importDV1.Name = "importDV1";
            this.importDV1.Size = new System.Drawing.Size(1155, 454);
            this.importDV1.TabIndex = 1;
            // 
            // btn_Import
            // 
            this.btn_Import.Location = new System.Drawing.Point(1059, 550);
            this.btn_Import.Name = "btn_Import";
            this.btn_Import.Size = new System.Drawing.Size(130, 33);
            this.btn_Import.TabIndex = 2;
            this.btn_Import.Text = "Import";
            this.btn_Import.UseVisualStyleBackColor = true;
            this.btn_Import.Click += new System.EventHandler(this.btn_Import_Click);
            // 
            // btn_Confirm
            // 
            this.btn_Confirm.Location = new System.Drawing.Point(871, 550);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(130, 33);
            this.btn_Confirm.TabIndex = 3;
            this.btn_Confirm.Text = "Confirm";
            this.btn_Confirm.UseVisualStyleBackColor = true;
            this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Date Adjust";
            // 
            // MassSalaryAdjustment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 604);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Confirm);
            this.Controls.Add(this.btn_Import);
            this.Controls.Add(this.importDV1);
            this.Controls.Add(this.dateAdjustPicker1);
            this.Name = "MassSalaryAdjustment";
            this.Text = "MassSalaryAdjustment";
            ((System.ComponentModel.ISupportInitialize)(this.importDV1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateAdjustPicker1;
        private System.Windows.Forms.DataGridView importDV1;
        private System.Windows.Forms.Button btn_Import;
        private System.Windows.Forms.Button btn_Confirm;
        private System.Windows.Forms.Label label1;
    }
}