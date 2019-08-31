
using App.Utilities;
using System.Drawing;
using System.Windows.Forms;

namespace App
{
    public partial class Master2 : Form
	{

	
	
		public Master2()
		{
			InitializeComponent();

			pnlStage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

			Helpers.InitializeImageList(m_smallImageList);

			LoadControl();
		}



		private void LoadControl()
		{
			pnlStage.Controls.Clear();
			IrisDeviceControl irisDeviceControl = new IrisDeviceControl();
			irisDeviceControl.parentForm = this;
			irisDeviceControl.Dock = DockStyle.Fill;
			pnlStage.Controls.Add(irisDeviceControl);
		}


		private void pnlHeader_Paint(object sender, PaintEventArgs e)
		{ Helpers.DrawLineInFooter(pnlHeader, Color.FromArgb(204, 204, 204), 2); }

		


	}
}
