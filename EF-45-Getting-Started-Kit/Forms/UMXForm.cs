using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class UMXForm : Form
    {
        public UMXForm()
        {
            string ipFilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\ip";
            if (!System.IO.File.Exists(ipFilePath))
            {
                System.IO.FileStream fileStream = System.IO.File.Create(ipFilePath);
                fileStream.Close();
                System.IO.File.WriteAllText(ipFilePath, "192.168.5.100");
            }

            InitializeComponent(ipFilePath);
        }

        public string IPAddress
        {
            get { return comboBox1.Text; }
            set { comboBox1.Text = value; }
        }

        public string SerialNumber
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        public bool CheckSNEnabled
        {
            get { return checkBoxMatchingImage.Checked; }
            set { checkBoxMatchingImage.Checked = value; }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void UMXForm_Load(object sender, EventArgs e)
        {
            comboBox1.Focus();
        }

        private void checkBoxMatchingImage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.textBox2.Enabled == true)
            {
                this.textBox2.Enabled = false;
            }
            else
            {
                this.textBox2.Enabled = true;
            }
        }
    }
}
