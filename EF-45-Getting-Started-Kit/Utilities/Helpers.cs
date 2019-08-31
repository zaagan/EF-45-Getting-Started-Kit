using CMITech.UMXClient.Entities;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace App.Utilities
{
    public class Helpers
    {
		public static string PreText = "UMXEF45";
		public static string Splitter = "#######################";


		public static string GetSyncDate()
		{
			string month, day, hour, minute, year, second = string.Empty;

			DateTime dt = DateTime.Now;

			if (dt.Month < 10) month = "0" + dt.Month.ToString();
			else month = dt.Month.ToString();

			if (dt.Day < 10) day = "0" + dt.Day.ToString();
			else day = dt.Day.ToString();

			if (dt.Hour < 10) hour = "0" + dt.Hour.ToString();
			else hour = dt.Hour.ToString();

			if (dt.Minute < 10) minute = "0" + dt.Minute.ToString();
			else minute = dt.Minute.ToString();

			year = dt.Year.ToString();

			if (dt.Second < 10) second = "0" + dt.Second.ToString();
			else second = dt.Second.ToString();

			string date = month + "" + day + "" + hour + "" + minute + "" + year + "." + second;
			return date;
		}



		public static void ShowStatusBar(string message, StatusBar statusBar, bool type)
		{
			statusBar.Visible = true;
			statusBar.Message = message;
			statusBar.StatusBarForeColor = Color.White;
			if (type)
				statusBar.StatusBarBackColor = Color.FromArgb(79, 208, 154);
			else
				statusBar.StatusBarBackColor = Color.FromArgb(230, 112, 134);

		}



		public static void DisplayMessage(string message, bool isOK, string caption = "")
        {
            if (isOK)
            {
                MessageBox.Show(message, caption);
            }
            else
            {
                MessageBox.Show(message, "Error occured");
            }
        }


        public static void DrawLineInFooter(Control control, Color color, int thickness)
        {
            int y = control.Height;
            DrawLine(control, color, 0, y, control.Width, y, thickness);
        }


        public static void DrawLine(Control control, Color color, int x, int y, int x1, int y1, int thickness)
        {
            Graphics graphicsObj = control.CreateGraphics();
            graphicsObj.DrawLine(new Pen(color, thickness), x, y, x1, y1);
        }


		
		public static void InitializeImageList(ImageList m_smallImageList)
		{
			m_smallImageList.Images.Add(SystemIcons.Information);
			m_smallImageList.Images.Add(SystemIcons.Warning);
			m_smallImageList.Images.Add(SystemIcons.Error);
		}


		public static string GenerateMessageString(bool isLeftMetrics, ImageQualityMetrics imageQualityMetrics)
		{
			StringBuilder sb = new StringBuilder();

			if (imageQualityMetrics != null)
			{
				if (isLeftMetrics)
					sb.Append("Left image metrics: ");
				else
					sb.Append(" Right image metrics:");

				sb.Append("Usable Iris Area - ");
				sb.Append(imageQualityMetrics.UsableIrisArea);
				sb.Append(" Quality Score - ");
				sb.Append(imageQualityMetrics.QualityScore);
				sb.Append(" Quality Ok - ");
				sb.Append(imageQualityMetrics.QualityOk);
			}
			return sb.ToString();
		}





        /// <summary>
        /// Encoding the template
        /// </summary>
        /// <param name="enrollTemplate"></param>
        /// <returns></returns>
        public static string EnrollTemplateToEncodedString(EnrolTemplate enrollTemplate)
        {
            string data1 = Convert.ToBase64String(enrollTemplate.LeftEyeTemplate);
            string data2 = Convert.ToBase64String(enrollTemplate.RightEyeTemplate);

            StringBuilder sb = new StringBuilder();
            sb.Append(PreText);
            sb.Append(Splitter);
            sb.Append(data1);
            sb.Append(Splitter);
            sb.Append(data2);

            return sb.ToString();
        }

        /// <summary>
        /// Decoding the template
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EnrolTemplate EncodedStringToEnrollTemplate(string data)

		{
			try
			{
				string encodedString = data.Trim();
				if (!string.IsNullOrEmpty(encodedString))
				{
					string splitter = Splitter;
					string[] content = data.Split(new string[] { Splitter }, StringSplitOptions.None);

					if (content != null)
					{
						if (content[0] != PreText) throw new Exception("Invalid template.");

						byte[] leftEyeTemplate = Convert.FromBase64String(content[1]);
						byte[] rightEyeTemplate = Convert.FromBase64String(content[2]);


						return new EnrolTemplate(leftEyeTemplate, rightEyeTemplate);
					}
					else return null;
				}
				else
					return null;
			}
			catch
			{
				return null;
			}
		}



	}
}
