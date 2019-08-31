///
///    Device Detail : CMITECH EF-45
///    Description : Initializers to be places in the class constructor to initialize certain controls
///

using CMITech.UMXClient.Entities;
using System;
using System.Drawing;
using System.Text;

namespace App.Utilities
{
	public class IrisComponentInitializer
	{

		

		public static void InitializeImageList(System.Windows.Forms.ImageList m_smallImageList)
		{
			m_smallImageList.Images.Add(SystemIcons.Information);
			m_smallImageList.Images.Add(SystemIcons.Warning);
			m_smallImageList.Images.Add(SystemIcons.Error);

		}

		public static void InitalizeTrackingImages(ref Bitmap[] faceTrackingOnImg, ref Bitmap[] faceTrackingOffImg, ref Bitmap[] irisTrackingOnImg, ref Bitmap[] irisTrackingOffImg)
		{
			faceTrackingOnImg = new Bitmap[32];
			faceTrackingOnImg[0] = null;
			faceTrackingOnImg[1] = new Bitmap(Properties.Resources.face_tracking_01_on);
			faceTrackingOnImg[2] = new Bitmap(Properties.Resources.face_tracking_02_on);
			faceTrackingOnImg[3] = new Bitmap(Properties.Resources.face_tracking_03_on);
			faceTrackingOnImg[4] = new Bitmap(Properties.Resources.face_tracking_04_on);
			faceTrackingOnImg[5] = new Bitmap(Properties.Resources.face_tracking_05_on);
			faceTrackingOnImg[6] = new Bitmap(Properties.Resources.face_tracking_06_on);
			faceTrackingOnImg[7] = new Bitmap(Properties.Resources.face_tracking_07_on);
			faceTrackingOnImg[8] = new Bitmap(Properties.Resources.face_tracking_08_on);
			faceTrackingOnImg[9] = new Bitmap(Properties.Resources.face_tracking_09_on);
			faceTrackingOnImg[10] = new Bitmap(Properties.Resources.face_tracking_10_on);
			faceTrackingOnImg[11] = new Bitmap(Properties.Resources.face_tracking_11_on);
			faceTrackingOnImg[12] = new Bitmap(Properties.Resources.face_tracking_12_on);
			faceTrackingOnImg[13] = new Bitmap(Properties.Resources.face_tracking_13_on);
			faceTrackingOnImg[14] = new Bitmap(Properties.Resources.face_tracking_14_on);
			faceTrackingOnImg[15] = new Bitmap(Properties.Resources.face_tracking_15_on);
			faceTrackingOnImg[16] = new Bitmap(Properties.Resources.face_tracking_16_on);
			faceTrackingOnImg[17] = new Bitmap(Properties.Resources.face_tracking_17_on);
			faceTrackingOnImg[18] = new Bitmap(Properties.Resources.face_tracking_18_on);
			faceTrackingOnImg[19] = new Bitmap(Properties.Resources.face_tracking_19_on);
			faceTrackingOnImg[20] = new Bitmap(Properties.Resources.face_tracking_20_on);
			faceTrackingOnImg[21] = new Bitmap(Properties.Resources.face_tracking_21_on);
			faceTrackingOnImg[22] = new Bitmap(Properties.Resources.face_tracking_22_on);
			faceTrackingOnImg[23] = new Bitmap(Properties.Resources.face_tracking_23_on);
			faceTrackingOnImg[24] = new Bitmap(Properties.Resources.face_tracking_24_on);
			faceTrackingOnImg[25] = new Bitmap(Properties.Resources.face_tracking_25_on);
			faceTrackingOnImg[26] = new Bitmap(Properties.Resources.face_tracking_26_on);
			faceTrackingOnImg[27] = new Bitmap(Properties.Resources.face_tracking_27_on);
			faceTrackingOnImg[28] = new Bitmap(Properties.Resources.face_tracking_28_on);
			faceTrackingOnImg[29] = new Bitmap(Properties.Resources.face_tracking_29_on);
			faceTrackingOnImg[30] = new Bitmap(Properties.Resources.face_tracking_30_on);
			faceTrackingOnImg[31] = new Bitmap(Properties.Resources.face_tracking_31_on);

			faceTrackingOffImg = new Bitmap[32];
			faceTrackingOffImg[0] = null;
			faceTrackingOffImg[1] = new Bitmap(Properties.Resources.face_tracking_01_off);
			faceTrackingOffImg[2] = new Bitmap(Properties.Resources.face_tracking_02_off);
			faceTrackingOffImg[3] = new Bitmap(Properties.Resources.face_tracking_03_off);
			faceTrackingOffImg[4] = new Bitmap(Properties.Resources.face_tracking_04_off);
			faceTrackingOffImg[5] = new Bitmap(Properties.Resources.face_tracking_05_off);
			faceTrackingOffImg[6] = new Bitmap(Properties.Resources.face_tracking_06_off);
			faceTrackingOffImg[7] = new Bitmap(Properties.Resources.face_tracking_07_off);
			faceTrackingOffImg[8] = new Bitmap(Properties.Resources.face_tracking_08_off);
			faceTrackingOffImg[9] = new Bitmap(Properties.Resources.face_tracking_09_off);
			faceTrackingOffImg[10] = new Bitmap(Properties.Resources.face_tracking_10_off);
			faceTrackingOffImg[11] = new Bitmap(Properties.Resources.face_tracking_11_off);
			faceTrackingOffImg[12] = new Bitmap(Properties.Resources.face_tracking_12_off);
			faceTrackingOffImg[13] = new Bitmap(Properties.Resources.face_tracking_13_off);
			faceTrackingOffImg[14] = new Bitmap(Properties.Resources.face_tracking_14_off);
			faceTrackingOffImg[15] = new Bitmap(Properties.Resources.face_tracking_15_off);
			faceTrackingOffImg[16] = new Bitmap(Properties.Resources.face_tracking_16_off);
			faceTrackingOffImg[17] = new Bitmap(Properties.Resources.face_tracking_17_off);
			faceTrackingOffImg[18] = new Bitmap(Properties.Resources.face_tracking_18_off);
			faceTrackingOffImg[19] = new Bitmap(Properties.Resources.face_tracking_19_off);
			faceTrackingOffImg[20] = new Bitmap(Properties.Resources.face_tracking_20_off);
			faceTrackingOffImg[21] = new Bitmap(Properties.Resources.face_tracking_21_off);
			faceTrackingOffImg[22] = new Bitmap(Properties.Resources.face_tracking_22_off);
			faceTrackingOffImg[23] = new Bitmap(Properties.Resources.face_tracking_23_off);
			faceTrackingOffImg[24] = new Bitmap(Properties.Resources.face_tracking_24_off);
			faceTrackingOffImg[25] = new Bitmap(Properties.Resources.face_tracking_25_off);
			faceTrackingOffImg[26] = new Bitmap(Properties.Resources.face_tracking_26_off);
			faceTrackingOffImg[27] = new Bitmap(Properties.Resources.face_tracking_27_off);
			faceTrackingOffImg[28] = new Bitmap(Properties.Resources.face_tracking_28_off);
			faceTrackingOffImg[29] = new Bitmap(Properties.Resources.face_tracking_29_off);
			faceTrackingOffImg[30] = new Bitmap(Properties.Resources.face_tracking_30_off);
			faceTrackingOffImg[31] = new Bitmap(Properties.Resources.face_tracking_31_off);

			irisTrackingOnImg = new Bitmap[27];
			irisTrackingOnImg[0] = null;
			irisTrackingOnImg[1] = new Bitmap(Properties.Resources.iris_tracking_01_on);
			irisTrackingOnImg[2] = new Bitmap(Properties.Resources.iris_tracking_02_on);
			irisTrackingOnImg[3] = new Bitmap(Properties.Resources.iris_tracking_03_on);
			irisTrackingOnImg[4] = new Bitmap(Properties.Resources.iris_tracking_04_on);
			irisTrackingOnImg[5] = new Bitmap(Properties.Resources.iris_tracking_05_on);
			irisTrackingOnImg[6] = new Bitmap(Properties.Resources.iris_tracking_06_on);
			irisTrackingOnImg[7] = new Bitmap(Properties.Resources.iris_tracking_07_on);
			irisTrackingOnImg[8] = new Bitmap(Properties.Resources.iris_tracking_08_on);
			irisTrackingOnImg[9] = new Bitmap(Properties.Resources.iris_tracking_09_on);
			irisTrackingOnImg[10] = new Bitmap(Properties.Resources.iris_tracking_10_on);
			irisTrackingOnImg[11] = new Bitmap(Properties.Resources.iris_tracking_11_on);
			irisTrackingOnImg[12] = new Bitmap(Properties.Resources.iris_tracking_12_on);
			irisTrackingOnImg[13] = new Bitmap(Properties.Resources.iris_tracking_13_on);
			irisTrackingOnImg[14] = new Bitmap(Properties.Resources.iris_tracking_14_on);
			irisTrackingOnImg[15] = new Bitmap(Properties.Resources.iris_tracking_15_on);
			irisTrackingOnImg[16] = new Bitmap(Properties.Resources.iris_tracking_16_on);
			irisTrackingOnImg[17] = new Bitmap(Properties.Resources.iris_tracking_17_on);
			irisTrackingOnImg[18] = new Bitmap(Properties.Resources.iris_tracking_18_on);
			irisTrackingOnImg[19] = new Bitmap(Properties.Resources.iris_tracking_19_on);
			irisTrackingOnImg[20] = new Bitmap(Properties.Resources.iris_tracking_20_on);
			irisTrackingOnImg[21] = new Bitmap(Properties.Resources.iris_tracking_21_on);
			irisTrackingOnImg[22] = new Bitmap(Properties.Resources.iris_tracking_22_on);
			irisTrackingOnImg[23] = new Bitmap(Properties.Resources.iris_tracking_23_on);
			irisTrackingOnImg[24] = new Bitmap(Properties.Resources.iris_tracking_24_on);
			irisTrackingOnImg[25] = new Bitmap(Properties.Resources.iris_tracking_25_on);
			irisTrackingOnImg[26] = new Bitmap(Properties.Resources.iris_tracking_26_on);

			irisTrackingOffImg = new Bitmap[27];
			irisTrackingOffImg[0] = null;
			irisTrackingOffImg[1] = new Bitmap(Properties.Resources.iris_tracking_01_off);
			irisTrackingOffImg[2] = new Bitmap(Properties.Resources.iris_tracking_02_off);
			irisTrackingOffImg[3] = new Bitmap(Properties.Resources.iris_tracking_03_off);
			irisTrackingOffImg[4] = new Bitmap(Properties.Resources.iris_tracking_04_off);
			irisTrackingOffImg[5] = new Bitmap(Properties.Resources.iris_tracking_05_off);
			irisTrackingOffImg[6] = new Bitmap(Properties.Resources.iris_tracking_06_off);
			irisTrackingOffImg[7] = new Bitmap(Properties.Resources.iris_tracking_07_off);
			irisTrackingOffImg[8] = new Bitmap(Properties.Resources.iris_tracking_08_off);
			irisTrackingOffImg[9] = new Bitmap(Properties.Resources.iris_tracking_09_off);
			irisTrackingOffImg[10] = new Bitmap(Properties.Resources.iris_tracking_10_off);
			irisTrackingOffImg[11] = new Bitmap(Properties.Resources.iris_tracking_11_off);
			irisTrackingOffImg[12] = new Bitmap(Properties.Resources.iris_tracking_12_off);
			irisTrackingOffImg[13] = new Bitmap(Properties.Resources.iris_tracking_13_off);
			irisTrackingOffImg[14] = new Bitmap(Properties.Resources.iris_tracking_14_off);
			irisTrackingOffImg[15] = new Bitmap(Properties.Resources.iris_tracking_15_off);
			irisTrackingOffImg[16] = new Bitmap(Properties.Resources.iris_tracking_16_off);
			irisTrackingOffImg[17] = new Bitmap(Properties.Resources.iris_tracking_17_off);
			irisTrackingOffImg[18] = new Bitmap(Properties.Resources.iris_tracking_18_off);
			irisTrackingOffImg[19] = new Bitmap(Properties.Resources.iris_tracking_19_off);
			irisTrackingOffImg[20] = new Bitmap(Properties.Resources.iris_tracking_20_off);
			irisTrackingOffImg[21] = new Bitmap(Properties.Resources.iris_tracking_21_off);
			irisTrackingOffImg[22] = new Bitmap(Properties.Resources.iris_tracking_22_off);
			irisTrackingOffImg[23] = new Bitmap(Properties.Resources.iris_tracking_23_off);
			irisTrackingOffImg[24] = new Bitmap(Properties.Resources.iris_tracking_24_off);
			irisTrackingOffImg[25] = new Bitmap(Properties.Resources.iris_tracking_25_off);
			irisTrackingOffImg[26] = new Bitmap(Properties.Resources.iris_tracking_26_off);
		}



	}
}
