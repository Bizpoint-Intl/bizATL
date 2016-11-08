namespace ATL.POR
{
    partial class AutoPIV
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txt_PIVRef = new System.Windows.Forms.TextBox();
            this.txt_ACNo = new System.Windows.Forms.TextBox();
            this.dtp_PIVDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_exr = new System.Windows.Forms.TextBox();
            this.txt_GST = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "PIV Ref";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "A/C No.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "PIV Date";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 161);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(107, 161);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(201, 161);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // txt_PIVRef
            // 
            this.txt_PIVRef.Location = new System.Drawing.Point(90, 13);
            this.txt_PIVRef.Name = "txt_PIVRef";
            this.txt_PIVRef.Size = new System.Drawing.Size(186, 20);
            this.txt_PIVRef.TabIndex = 6;
            // 
            // txt_ACNo
            // 
            this.txt_ACNo.Location = new System.Drawing.Point(90, 64);
            this.txt_ACNo.Name = "txt_ACNo";
            this.txt_ACNo.Size = new System.Drawing.Size(186, 20);
            this.txt_ACNo.TabIndex = 7;
            // 
            // dtp_PIVDate
            // 
            this.dtp_PIVDate.Location = new System.Drawing.Point(90, 38);
            this.dtp_PIVDate.Name = "dtp_PIVDate";
            this.dtp_PIVDate.Size = new System.Drawing.Size(186, 20);
            this.dtp_PIVDate.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "EX-Rate";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "GST";
            // 
            // txt_exr
            // 
            this.txt_exr.Location = new System.Drawing.Point(90, 95);
            this.txt_exr.Name = "txt_exr";
            this.txt_exr.Size = new System.Drawing.Size(186, 20);
            this.txt_exr.TabIndex = 11;
            // 
            // txt_GST
            // 
            this.txt_GST.Location = new System.Drawing.Point(90, 122);
            this.txt_GST.Name = "txt_GST";
            this.txt_GST.Size = new System.Drawing.Size(186, 20);
            this.txt_GST.TabIndex = 12;
            // 
            // AutoPIV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 205);
            this.Controls.Add(this.txt_GST);
            this.Controls.Add(this.txt_exr);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtp_PIVDate);
            this.Controls.Add(this.txt_ACNo);
            this.Controls.Add(this.txt_PIVRef);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AutoPIV";
            this.Text = "AutoPIV";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txt_PIVRef;
        private System.Windows.Forms.TextBox txt_ACNo;
        private System.Windows.Forms.DateTimePicker dtp_PIVDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_exr;
        private System.Windows.Forms.TextBox txt_GST;
    }
}