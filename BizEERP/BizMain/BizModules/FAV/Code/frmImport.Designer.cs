namespace ATL.FAV
{
    partial class frmImport
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
            this.btn_Standard = new System.Windows.Forms.Button();
            this.btn_Opera = new System.Windows.Forms.Button();
            this.btn_txt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Standard
            // 
            this.btn_Standard.Location = new System.Drawing.Point(54, 24);
            this.btn_Standard.Name = "btn_Standard";
            this.btn_Standard.Size = new System.Drawing.Size(135, 31);
            this.btn_Standard.TabIndex = 0;
            this.btn_Standard.Text = "Import Standard";
            this.btn_Standard.UseVisualStyleBackColor = true;
            this.btn_Standard.Click += new System.EventHandler(this.btn_Standard_Click);
            // 
            // btn_Opera
            // 
            this.btn_Opera.Location = new System.Drawing.Point(54, 65);
            this.btn_Opera.Name = "btn_Opera";
            this.btn_Opera.Size = new System.Drawing.Size(135, 31);
            this.btn_Opera.TabIndex = 1;
            this.btn_Opera.Text = "Import Opera MC";
            this.btn_Opera.UseVisualStyleBackColor = true;
            this.btn_Opera.Click += new System.EventHandler(this.btn_Opera_Click);
            // 
            // btn_txt
            // 
            this.btn_txt.Location = new System.Drawing.Point(54, 105);
            this.btn_txt.Name = "btn_txt";
            this.btn_txt.Size = new System.Drawing.Size(135, 31);
            this.btn_txt.TabIndex = 2;
            this.btn_txt.Text = "Import Opera RV";
            this.btn_txt.UseVisualStyleBackColor = true;
            this.btn_txt.Click += new System.EventHandler(this.btn_txt_Click);
            // 
            // frmImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 159);
            this.Controls.Add(this.btn_txt);
            this.Controls.Add(this.btn_Opera);
            this.Controls.Add(this.btn_Standard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmImport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Selection";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmImport_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Standard;
        private System.Windows.Forms.Button btn_Opera;
        private System.Windows.Forms.Button btn_txt;
    }
}