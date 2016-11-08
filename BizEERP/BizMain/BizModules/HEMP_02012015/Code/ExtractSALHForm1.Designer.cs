namespace ATL.ExtractSALHForm1
{
    partial class ExtractSALHForm1
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
            this.EmpDGV1 = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.TotalTB = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.EmpDGV1)).BeginInit();
            this.SuspendLayout();
            // 
            // EmpDGV1
            // 
            this.EmpDGV1.AllowUserToOrderColumns = true;
            this.EmpDGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EmpDGV1.Location = new System.Drawing.Point(4, 4);
            this.EmpDGV1.Name = "EmpDGV1";
            this.EmpDGV1.Size = new System.Drawing.Size(266, 485);
            this.EmpDGV1.TabIndex = 0;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(47, 546);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(51, 13);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "TOTAL : ";
            // 
            // TotalTB
            // 
            this.TotalTB.Location = new System.Drawing.Point(104, 543);
            this.TotalTB.Name = "TotalTB";
            this.TotalTB.Size = new System.Drawing.Size(157, 20);
            this.TotalTB.TabIndex = 2;
            this.TotalTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ExtractSALHForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 582);
            this.Controls.Add(this.TotalTB);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.EmpDGV1);
            this.Name = "ExtractSALHForm1";
            this.Text = "Salary Lists";
            this.Load += new System.EventHandler(this.ExtractATR1Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.EmpDGV1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView EmpDGV1;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.TextBox TotalTB;
    }
}