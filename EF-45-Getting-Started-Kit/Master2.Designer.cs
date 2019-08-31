using System;

namespace App
{
	partial class Master2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Master2));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.m_smallImageList = new System.Windows.Forms.ImageList(this.components);
            this.pnlStage = new System.Windows.Forms.Panel();
            this.statusBar = new App.StatusBar();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(534, 37);
            this.pnlHeader.TabIndex = 713;
            this.pnlHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHeader_Paint);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(116)))), ((int)(((byte)(116)))));
            this.lblHeader.Location = new System.Drawing.Point(12, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(133, 19);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "EF-45 Starter Kit";
            // 
            // m_smallImageList
            // 
            this.m_smallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.m_smallImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.m_smallImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pnlStage
            // 
            this.pnlStage.Location = new System.Drawing.Point(8, 44);
            this.pnlStage.Name = "pnlStage";
            this.pnlStage.Size = new System.Drawing.Size(514, 421);
            this.pnlStage.TabIndex = 715;
            // 
            // statusBar
            // 
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBar.Location = new System.Drawing.Point(0, 471);
            this.statusBar.Message = "Welcome";
            this.statusBar.MessageType = false;
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(534, 23);
            this.statusBar.StatusBarBackColor = System.Drawing.SystemColors.Control;
            this.statusBar.StatusBarForeColor = System.Drawing.SystemColors.ControlText;
            this.statusBar.TabIndex = 714;
            // 
            // Master2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 494);
            this.Controls.Add(this.pnlStage);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(550, 533);
            this.MinimumSize = new System.Drawing.Size(550, 533);
            this.Name = "Master2";
            this.Text = "Master2";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

		}



		#endregion

		private System.Windows.Forms.Panel pnlHeader;
		public System.Windows.Forms.Label lblHeader;
		private System.Windows.Forms.ImageList m_smallImageList;
		public StatusBar statusBar;
		private System.Windows.Forms.Panel pnlStage;
	}
}

