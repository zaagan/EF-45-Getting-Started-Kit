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
    public partial class LogListForm : Form
    {
        public LogListForm()
        {
            InitializeComponent();
        }

        public List<int> LogUIDs
        {
            get { return _logUids; }
            set { _logUids = value;}
        }

        public int LogUID
        {
            get { return _logUid; }
        }

        private void LogListForm_Load(object sender, EventArgs e)
        {
            logUuidCombo.Items.AddRange(_logUids.Cast<object>().ToArray());
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (logUuidCombo.SelectedItem != null)
            {
                _logUid = (int)logUuidCombo.SelectedItem;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private List<int> _logUids = new List<int>();
        private int _logUid;

    }
}
