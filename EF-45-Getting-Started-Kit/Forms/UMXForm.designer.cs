namespace App
{
    partial class UMXForm
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
        private void InitializeComponent(string ipFilePath)
        {
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.labelFirmware = new System.Windows.Forms.Label();
            this.labelUpload = new System.Windows.Forms.Label();
            this.checkBoxMatchingImage = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(58, 132);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(87, 21);
            this._okButton.TabIndex = 0;
            this._okButton.Text = "&OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(177, 132);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(87, 21);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "&Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // labelFirmware
            // 
            this.labelFirmware.AutoSize = true;
            this.labelFirmware.Location = new System.Drawing.Point(34, 41);
            this.labelFirmware.Name = "labelFirmware";
            this.labelFirmware.Size = new System.Drawing.Size(67, 12);
            this.labelFirmware.TabIndex = 4;
            this.labelFirmware.Text = "IP Address";
            // 
            // labelUpload
            // 
            this.labelUpload.AutoSize = true;
            this.labelUpload.Location = new System.Drawing.Point(34, 83);
            this.labelUpload.Name = "labelUpload";
            this.labelUpload.Size = new System.Drawing.Size(86, 12);
            this.labelUpload.TabIndex = 5;
            this.labelUpload.Text = "Serial Number";
            // 
            // checkBoxMatchingImage
            // 
            this.checkBoxMatchingImage.AutoSize = true;
            this.checkBoxMatchingImage.Location = new System.Drawing.Point(258, 84);
            this.checkBoxMatchingImage.Name = "checkBoxMatchingImage";
            this.checkBoxMatchingImage.Size = new System.Drawing.Size(63, 16);
            this.checkBoxMatchingImage.TabIndex = 6;
            this.checkBoxMatchingImage.Text = "Enable";
            this.checkBoxMatchingImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxMatchingImage.UseVisualStyleBackColor = true;
            this.checkBoxMatchingImage.CheckedChanged += new System.EventHandler(this.checkBoxMatchingImage_CheckedChanged);
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(131, 80);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(116, 21);
            this.textBox2.TabIndex = 3;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(131, 38);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 7;
            string str = System.IO.File.ReadAllText(ipFilePath);
            string[] strList = str.Split('\n');
            for (int i = 0; i < strList.Length; i++)
                this.comboBox1.Items.Add(strList[i]);
            this.comboBox1.SelectedIndex = strList.Length - 1;
            //this.comboBox1.SelectedText = "192.168.5.100";

            // 
            // UMXForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 182);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.checkBoxMatchingImage);
            this.Controls.Add(this.labelUpload);
            this.Controls.Add(this.labelFirmware);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Name = "UMXForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UMXForm";
            this.Load += new System.EventHandler(this.UMXForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label labelFirmware;
        private System.Windows.Forms.Label labelUpload;
        private System.Windows.Forms.CheckBox checkBoxMatchingImage;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}