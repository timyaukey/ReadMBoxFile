namespace ReadMBoxFile
{
    partial class Form1
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
            this.btnRunBasic = new System.Windows.Forms.Button();
            this.btnRunMultipart = new System.Windows.Forms.Button();
            this.btnRunNested = new System.Windows.Forms.Button();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRunBasic
            // 
            this.btnRunBasic.Location = new System.Drawing.Point(12, 12);
            this.btnRunBasic.Name = "btnRunBasic";
            this.btnRunBasic.Size = new System.Drawing.Size(152, 23);
            this.btnRunBasic.TabIndex = 1;
            this.btnRunBasic.Text = "Basic";
            this.btnRunBasic.UseVisualStyleBackColor = true;
            this.btnRunBasic.Click += new System.EventHandler(this.btnRunBasic_Click);
            // 
            // btnRunMultipart
            // 
            this.btnRunMultipart.Location = new System.Drawing.Point(12, 41);
            this.btnRunMultipart.Name = "btnRunMultipart";
            this.btnRunMultipart.Size = new System.Drawing.Size(152, 23);
            this.btnRunMultipart.TabIndex = 2;
            this.btnRunMultipart.Text = "Multipart";
            this.btnRunMultipart.UseVisualStyleBackColor = true;
            this.btnRunMultipart.Click += new System.EventHandler(this.btnRunMultipart_Click);
            // 
            // btnRunNested
            // 
            this.btnRunNested.Location = new System.Drawing.Point(12, 70);
            this.btnRunNested.Name = "btnRunNested";
            this.btnRunNested.Size = new System.Drawing.Size(152, 23);
            this.btnRunNested.TabIndex = 3;
            this.btnRunNested.Text = "Nested";
            this.btnRunNested.UseVisualStyleBackColor = true;
            this.btnRunNested.Click += new System.EventHandler(this.btnRunNested_Click);
            // 
            // btnRunAll
            // 
            this.btnRunAll.Location = new System.Drawing.Point(12, 99);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(152, 23);
            this.btnRunAll.TabIndex = 4;
            this.btnRunAll.Text = "All";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 233);
            this.Controls.Add(this.btnRunAll);
            this.Controls.Add(this.btnRunNested);
            this.Controls.Add(this.btnRunMultipart);
            this.Controls.Add(this.btnRunBasic);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRunBasic;
        private System.Windows.Forms.Button btnRunMultipart;
        private System.Windows.Forms.Button btnRunNested;
        private System.Windows.Forms.Button btnRunAll;
    }
}

