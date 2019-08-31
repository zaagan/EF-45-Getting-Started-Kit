using App.Utilities;
using CMITech.UMXClient;
using CMITech.UMXClient.Entities;
using CMITech.UMXClient.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class IrisDeviceControl : UserControl
    {
        public string currentUID = "1";
		public string DeviceType { get; } = "iris";


        public Master2 parentForm = null;
        private Client _client;
        private LogMessages m_logMessages;

        private Bitmap[] faceTrackingOnImg, faceTrackingOffImg, irisTrackingOnImg, irisTrackingOffImg;

        private bool _enrollCompleted = false, _bigOffFaceCanEnroll = true, _startEnrollCaptureCompleted = false, _onMatchingImgae = true;

        private Image _currentEnrolLeftIrisImage = null, _currentEnrolRightIrisImage = null, _currentEnrolIrisOffFaceImage = null;
        private byte[] _currentEnrolLeftIris8bppBmpByteArray = null, _currentEnrolRightIris8bppBmpByteArray = null, _currentEnrolIrisOffFace24bppBmpByteArray = null;

        private EnrolTemplate _currentEnrolTemplate = null;


        #region -------- Initalize --------

        private void InitializeEvents()
        {
            _client.CaptureCompleted += new Client.CaptureCompletedEventHandler(_client_CaptureCompleted);
            _client.CaptureTimedOut += new Client.CaptureTimedOutEventHandler(_client_CaptureTimedOut);
            _client.CaptureError += new Client.CaptureErrorEventHandler(_client_CaptureError);
            _client.MatchLog += new Client.MatchLogEventHandler(_client_MatchLog);
            _client.MatchData += new Client.MatchDataEventHandler(_client_MatchData);
            _client.PreviewImage += new Client.PreviewImageEventHandler(_client_PreviewImage);
        }

        private void DeInitializeEvents()
        {
            try
            {
                _client.CaptureCompleted -= new Client.CaptureCompletedEventHandler(_client_CaptureCompleted);
                _client.CaptureTimedOut -= new Client.CaptureTimedOutEventHandler(_client_CaptureTimedOut);
                _client.CaptureError -= new Client.CaptureErrorEventHandler(_client_CaptureError);
                _client.MatchLog -= new Client.MatchLogEventHandler(_client_MatchLog);
                _client.MatchData -= new Client.MatchDataEventHandler(_client_MatchData);
                _client.PreviewImage -= new Client.PreviewImageEventHandler(_client_PreviewImage);
            }
            catch
            {

            }
         
        }


        #endregion

        public IrisDeviceControl()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();

            tbCtrl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            listViewMessages.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;


            IrisComponentInitializer.InitializeImageList(m_smallImageList);
            IrisComponentInitializer.InitalizeTrackingImages(ref faceTrackingOnImg, ref faceTrackingOffImg, ref irisTrackingOnImg, ref irisTrackingOffImg);

            m_logMessages = new LogMessages(listViewMessages);
            AddMessage(MessageManager.msg_Iris_ClientStarted, LogMessages.Icon.Information);
            tbCtrl.SelectedIndex = 2;
            this.ActiveControl = this.btnConnect;
            LoadIrisServerIP();
        }

        private void LoadIrisServerIP()
        {
            try
            {
                string serverIP = Properties.Settings.Default.IrisServerIP.Trim();
                if (!string.IsNullOrEmpty(serverIP))
                    tbxServiceIP.Text = serverIP.Trim();
            }
            catch
            {
                tbxServiceIP.Text = string.Empty;
            }
        }


        /// <summary>
        /// RESET CONTROLS BEFORE PERFORMING CONNECTION
        /// </summary>
        private void ResetForNewConnection()
        {
            this.panelPrevivew.BackgroundImage = null;
            this.alphaBTextBox_topIrisInfoBack.Hide();
            this.pictureBox_irisGuide.Hide();
            this.pictureBox_faceGuide.Hide();
            this.alphaBTextBox_openEyesInfoTxt.Hide();

            this.pictureBoxLeftEye.Image = null;
            this.pictureBoxIrisFace.Image = null;
            this.pictureBoxRightEye.Image = null;
        }


        private void ToggleControls(bool enable)
        {
            tbxServiceIP.Enabled = enable;
            tbxLogID.Enabled = enable;
            tbxUserID.Enabled = enable;

            gbxInteractionMode.Enabled = enable;
            btnStartCapture.Enabled = enable;
            btnStopCapture.Enabled = enable;
            btnClickToEnroll.Enabled = enable;

            btnGetLog.Enabled = enable;
            btnGetLogs.Enabled = enable;
            btnDeleteAllLogs.Enabled = enable;
            btnClearLogs.Enabled = enable;

            btnListSubjects.Enabled = enable;
            btnDeleteAllUsers.Enabled = enable;

            btnUpdateSettings.Enabled = enable;

            btnConnect.Enabled = enable;
            btnForceConnect.Enabled = enable;
            btnDisconnect.Enabled = enable;

            buttonGetIrisUsableArea.Enabled = enable;
            buttonSetIrisUsableArea.Enabled = enable;
            btnCheckConnection.Enabled = enable;
        }


        /// <summary>
        /// Connect With The Device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnInit_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            ToggleControls(false);
            AddMessage(MessageManager.msg_Iris_EstablishingConnection, LogMessages.Icon.Information, displayMessage: true);

            ResetForNewConnection();
            string ipAddress = tbxServiceIP.Text.Trim();
            string serialNumber = string.Empty;
            bool checkSerialEnabled = false;

            if (ipAddress == string.Empty)
            {
                UMXForm umxForm = new UMXForm();
                if (umxForm.ShowDialog() == DialogResult.OK)
                {
                    ipAddress = umxForm.IPAddress;
                    serialNumber = umxForm.SerialNumber;
                    checkSerialEnabled = umxForm.CheckSNEnabled;
                }
                else
                    return;
            }

            if (_client != null && _client.IsConnected())
            {
                AddMessage(MessageManager.msg_Iris_Connected, LogMessages.Icon.Information, displayMessage: true);
                ToggleControls(true);
                return;
            }

            _client = new Client(ipAddress, serialNumber, checkSerialEnabled);


            InitializeEvents();

            try
            {
                bool isConnected = false;
                Task<bool> connectionTask = Task.Run(() => _client.Connect());

                isConnected = await connectionTask;


                if (isConnected)
                {
                    AddMessage(MessageManager.msg_Iris_ConnectionSuccess + serialNumber, LogMessages.Icon.Information, displayMessage: true);
                    AddMessage(MessageManager.msg_DeviceIsRunningSoftwareVersion + _client.GetVersion(), LogMessages.Icon.Information);
                    Client.UMXMode mode = _client.GetMode();
                    if (mode == Client.UMXMode.Recog || mode == Client.UMXMode.Enrol)
                    {
                        recogRadioButton.Checked = true;
                        enrollRadioButton.Checked = false;
                        this.tbCtrl.SelectedIndex = 0;
                    }
                    else if (mode == Client.UMXMode.Slave)
                    {
                        recogRadioButton.Checked = false;
                        enrollRadioButton.Checked = true;
                        this.tbCtrl.SelectedIndex = 1;
                    }

                    int enrollUsableAreaIndex = _client.GetConfigInt("umx.camera.enrol.MinimumUsableIrisArea");
                    comboBoxIrisUsableArea.SelectedIndex = enrollUsableAreaIndex;
                    comboBoxIrisUsableArea.Enabled = true;

                    if (checkBoxSyncTime.Checked == true) SyncTime();

                }
                else if (_client.IsConnected())
                    AddMessage(MessageManager.msg_AlreadyConnected, LogMessages.Icon.Information, displayMessage: true);
                else
                    AddMessage(MessageManager.msg_ForceConnectToSteal, LogMessages.Icon.Information, displayMessage: true);
            }
            catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Warning, displayMessage: true); }
            catch (InvalidSerialNumberException ex) { AddMessage(ex.Message, LogMessages.Icon.Warning, displayMessage: true); }


            ToggleControls(true);

        }


        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && _client.IsConnected())
            {
                try
                {
                    DeInitializeEvents();
                    ResetForNewConnection();

                    _client.Disconnect();
                    comboBoxIrisUsableArea.Enabled = false;
                    AddMessage(MessageManager.msg_Iris_Disconnected, LogMessages.Icon.Information, displayMessage: true);
                }
                catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }
            }
            else { AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Information, displayMessage: true); }

        }

        internal bool ReleaseResources()
        {
            try
            {
                ReEnrollUser(true, true);
                btnDisconnect_Click(null, null);
                return false;
            }
            catch
            {

                return false;
            }
        }

        private void btnForceConnect_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && !_client.IsConnected())
            {

                DeInitializeEvents();
                try
                {
                    if (_client.StealConnect())
                    {
                        InitializeEvents();
                        AddMessage(MessageManager.msg_Iris_ConnectionSuccess, LogMessages.Icon.Information, displayMessage: true);
                        AddMessage(MessageManager.msg_DeviceIsRunningSoftwareVersion + _client.GetVersion(), LogMessages.Icon.Information);

                        Client.UMXMode mode = _client.GetMode();
                        if (mode == Client.UMXMode.Recog || mode == Client.UMXMode.Enrol)
                        {
                            recogRadioButton.Checked = true;
                            this.tbCtrl.SelectedIndex = 1;
                        }

                        else if (mode == Client.UMXMode.Slave)
                        {
                            enrollRadioButton.Checked = true;
                            this.tbCtrl.SelectedIndex = 0;
                        }

                        int enrollUsableAreaIndex = _client.GetConfigInt("umx.camera.enrol.MinimumUsableIrisArea");
                        comboBoxIrisUsableArea.SelectedIndex = enrollUsableAreaIndex;
                        comboBoxIrisUsableArea.Enabled = true;

                        if (checkBoxSyncTime.Checked == true) SyncTime();

                    }
                    else AddMessage(MessageManager.msg_Iris_ForeConnectionError, LogMessages.Icon.Error, displayMessage: true);
                }
                catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }

            }
        }




        #region -------- EVENTS --------


        private void _client_CaptureCompleted(CaptureCompletedEventArgs captureCompletedEventArgs)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate () { _client_CaptureCompleted(captureCompletedEventArgs); }));
            }
            else
            {
                if (captureCompletedEventArgs.Completed)
                {
                    _currentEnrolLeftIrisImage = captureCompletedEventArgs.LeftEye;
                    _currentEnrolLeftIris8bppBmpByteArray = captureCompletedEventArgs.LeftEye8bitBmpByteArray;
                    pictureBoxLeftEye.Image = captureCompletedEventArgs.LeftEye;

                    _currentEnrolRightIrisImage = captureCompletedEventArgs.RightEye;
                    _currentEnrolRightIris8bppBmpByteArray = captureCompletedEventArgs.RightEye8bitBmpByteArray;
                    pictureBoxRightEye.Image = captureCompletedEventArgs.RightEye;

                    _currentEnrolIrisOffFaceImage = captureCompletedEventArgs.IrisOffFace;
                    _currentEnrolIrisOffFace24bppBmpByteArray = captureCompletedEventArgs.IrisOffFace24bitBmpByteArray;
                    pictureBoxIrisFace.Image = captureCompletedEventArgs.IrisOffFace;



                    // Deduplicate
                    //if ((captureCompletedEventArgs.LeftEyeDuplicateUUID != "" && captureCompletedEventArgs.LeftEyeDuplicateUUID != null)
                    //    || (captureCompletedEventArgs.RightEyeDuplicateUUID != "" && captureCompletedEventArgs.RightEyeDuplicateUUID != null))
                    //{
                    //    if (MessageBox.Show("Duplicate user was found.\n Do you like to continue?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //    {

                    //        // YES
                    //    }
                    //    else
                    //    {
                    //        // NO
                    //        return;
                    //    }
                    //}

                    _currentEnrolTemplate = captureCompletedEventArgs.EnrolTemplate;

                    string leftMetrics = Helpers.GenerateMessageString(true, captureCompletedEventArgs.LeftEyeImageQualityMetrics);
                    if (captureCompletedEventArgs.LeftEyeImageQualityMetrics != null) AddMessage(leftMetrics, LogMessages.Icon.Information);

                    string rightMetrics = Helpers.GenerateMessageString(false, captureCompletedEventArgs.RightEyeImageQualityMetrics);
                    if (captureCompletedEventArgs.RightEyeImageQualityMetrics != null) AddMessage(rightMetrics, LogMessages.Icon.Information);

                    _startEnrollCaptureCompleted = true;

                    AddMessage(MessageManager.msg_Iris_CaptureCompleted, LogMessages.Icon.Information, displayMessage: true);
                }
                else
                    AddMessage(MessageManager.msg_Iris_CaptureFailed, LogMessages.Icon.Error, displayMessage: true);

            }
        }

        void _client_CaptureTimedOut()
        {
            if (this.InvokeRequired)
                BeginInvoke(new MethodInvoker(delegate () { _client_CaptureTimedOut(); }));
            else
                AddMessage("Capture Timed Out", LogMessages.Icon.Warning, displayMessage: true);
        }

        void _client_CaptureError(Exception error)
        {
            if (this.InvokeRequired)
                BeginInvoke(new MethodInvoker(delegate () { _client_CaptureError(error); }));
            else
                AddMessage("Capture Error: " + error.Message, LogMessages.Icon.Error, displayMessage: true);
        }


        void _client_MatchLog(MatchLogEventArgs matchLogEventArgs)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate () { _client_MatchLog(matchLogEventArgs); }));
            }
            else
            {
                DateTime matchTime = matchLogEventArgs.Timestamp.ToLocalTime();
                string message = string.Empty;
                if (matchLogEventArgs.Type == MatchType.Allowed)
                {
                    message = matchLogEventArgs.Type + " " + matchLogEventArgs.SubjectUID + " on " + matchLogEventArgs.SerialNumber;
                    m_logMessages.AddMessage(LogMessages.Icon.Information, matchTime, message);
                    Helpers.ShowStatusBar(message, parentForm.statusBar, true);

                }
                else if (matchLogEventArgs.Type == MatchType.Denied)
                {
                    pictureBoxRightEye.Image = null;
                    pictureBoxLeftEye.Image = null;
                    pictureBoxIrisFace.Image = null;

                    message = matchLogEventArgs.Type + " " + matchLogEventArgs.Info + " on " + matchLogEventArgs.SerialNumber;
                    m_logMessages.AddMessage(LogMessages.Icon.Warning, matchTime, message);
                    Helpers.ShowStatusBar(message, parentForm.statusBar, false);

                }
                else if (matchLogEventArgs.Type == MatchType.Unknown)
                {
                    message = matchLogEventArgs.Type.ToString() + " on " + matchLogEventArgs.SerialNumber;
                    m_logMessages.AddMessage(LogMessages.Icon.Warning, matchTime, message);
                    Helpers.ShowStatusBar(message, parentForm.statusBar, false);
                }

            }
        }


        void _client_MatchData(MatchDataEventArgs matchDataEventArgs)
        {
            this.panelPrevivew.BackgroundImage = null;

            pictureBoxLeftEye.Image = null;
            pictureBoxRightEye.Image = null;
            pictureBoxIrisFace.Image = null;

            if (!_onMatchingImgae) return;

            if (this.InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate () { _client_MatchData(matchDataEventArgs); }));
            }
            else
            {
                if (matchDataEventArgs.LeftEye != null) pictureBoxLeftEye.Image = matchDataEventArgs.LeftEye;
                if (matchDataEventArgs.RightEye != null) pictureBoxRightEye.Image = matchDataEventArgs.RightEye;
                if (matchDataEventArgs.IrisOffFace != null) pictureBoxIrisFace.Image = matchDataEventArgs.IrisOffFace;
            }
        }

        void _client_PreviewImage(PreviewImageEventArgs previewImageEventArgs)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate () { _client_PreviewImage(previewImageEventArgs); }));
            }
            else
            {
                if (previewImageEventArgs.FaceIrisCapture.LeftEye != null) this.pictureBoxLeftEye.Image = previewImageEventArgs.FaceIrisCapture.LeftEye;
                if (previewImageEventArgs.FaceIrisCapture.RightEye != null) this.pictureBoxRightEye.Image = previewImageEventArgs.FaceIrisCapture.RightEye;

                if (previewImageEventArgs.FaceIrisCapture.FaceImage != null)
                {
                    displayGuide(previewImageEventArgs.FaceIrisCapture);
                    this.panelPrevivew.BackgroundImage = scaleDown(previewImageEventArgs.FaceIrisCapture.FaceImage, this.panelPrevivew.Width, this.panelPrevivew.Height);


                }
            }
        }


        #endregion


        private void AddMessage(string message, LogMessages.Icon messageIcon, bool displayMessage = false)
        {
            m_logMessages.AddMessage(messageIcon, DateTime.Now, message);

            if (displayMessage)
                Helpers.ShowStatusBar(message, parentForm.statusBar, messageIcon == LogMessages.Icon.Information);
        }

        private void ClearDispayMessage()
        {
            parentForm.statusBar.Visible = false;
        }

        #region  ------- DEVICE CAMERA CONFIGURATION --------


        bool _gIrisMode = false;
        private int _gFaceTrackingBlink = 0;
        private int _gIrisTrackingBlink = 0;
        private int _gIrisGuideColor = 0;

        void colorTopIndicator(bool face, int color)
        {
            if (face)
            {
                alphaBTextBox_topIrisInfoBack.Hide();
                alphaBTextBox_topFaceInfoBack.Show();
            }
            else
            {
                alphaBTextBox_topIrisInfoBack.Show();
                alphaBTextBox_topFaceInfoBack.Hide();
            }

            switch (color)
            {
                case (int)Client.UMXDistance.CAMERA_ENROLL_GREEN:
                    {
                        if (face)
                        {
                            alphaBTextBox_topFaceInfoBack.BackAlpha = 90;
                            alphaBTextBox_topFaceInfoBack.BackColor = Color.FromArgb(50, 205, 50); //32cd32
                            alphaBTextBox_topFaceInfoBack.Text = "Good"; /*USER_ENROLL_CAMERA_GOOD_TEXT*/
                        }
                        else
                        {
                            alphaBTextBox_topIrisInfoBack.BackAlpha = 90;
                            alphaBTextBox_topIrisInfoBack.BackColor = Color.FromArgb(50, 205, 50); //32cd32
                            alphaBTextBox_topIrisInfoBack.Text = "Good"; /*USER_ENROLL_CAMERA_GOOD_TEXT*/
                        }
                    }
                    break;
                case (int)Client.UMXDistance.CAMERA_ENROLL_BLUE:
                    {
                        if (face)
                        {
                            alphaBTextBox_topFaceInfoBack.BackAlpha = 90;
                            alphaBTextBox_topFaceInfoBack.BackColor = Color.FromArgb(65, 105, 225); //4169e1
                            alphaBTextBox_topFaceInfoBack.Text = "Forward"; /*USER_ENROLL_CAMERA_FORWARD_TEXT*/
                        }
                        else
                        {
                            alphaBTextBox_topIrisInfoBack.BackAlpha = 90;
                            alphaBTextBox_topIrisInfoBack.BackColor = Color.FromArgb(65, 105, 225); //4169e1
                            alphaBTextBox_topIrisInfoBack.Text = "Forward"; /*USER_ENROLL_CAMERA_FORWARD_TEXT*/
                        }
                    }
                    break;
                case (int)Client.UMXDistance.CAMERA_ENROLL_RED:
                    {
                        if (face)
                        {
                            alphaBTextBox_topFaceInfoBack.BackAlpha = 90;
                            alphaBTextBox_topFaceInfoBack.BackColor = Color.FromArgb(255, 0, 0); //ff0000
                            alphaBTextBox_topFaceInfoBack.Text = "Backward"; /*USER_ENROLL_CAMERA_BACKWARD_TEXT*/
                        }
                        else
                        {
                            alphaBTextBox_topIrisInfoBack.BackAlpha = 90;
                            alphaBTextBox_topIrisInfoBack.BackColor = Color.FromArgb(255, 0, 0); //ff0000
                            alphaBTextBox_topIrisInfoBack.Text = "Backward"; /*USER_ENROLL_CAMERA_BACKWARD_TEXT*/
                        }
                    }
                    break;
            }
        }

        private static Image scaleDown(Image imgPhoto, int Width, int Height)
        {
            if (imgPhoto == null) return null;

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                      imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }


        void enrollEyePositionSlot(CameraEyePosition position)
        {
            //qDebug("distance (%d) left X(%d), left (%d), right X(%d) right Y(%d)", position.AvgDistance, position.LeftCenterX, position.LeftCenterY, position.RightCenterX, position.RightCenterY);

            if (position.AvgDistance > 0)
            {
                int positionX = 0;
                int positionY = 0;

                //Found center X, Y (right X - left X, rightY - rightY)
                if (position.LeftCenterX > position.RightCenterX)
                {
                    positionX = position.RightCenterX + ((position.LeftCenterX - position.RightCenterX) / 2);
                }
                else
                {
                    positionX = position.LeftCenterX + ((position.RightCenterX - position.LeftCenterX) / 2);
                }

                if (position.LeftCenterY > position.RightCenterY)
                {
                    positionY = position.RightCenterY + ((position.LeftCenterY - position.RightCenterY) / 2);
                }
                else
                {
                    positionY = position.RightCenterY + ((position.RightCenterY - position.LeftCenterY) / 2);
                }

                //leftX - (centerX - leftX) = leftX,
                int x = position.LeftCenterX - (positionX - position.LeftCenterX);
                int RightX = position.RightCenterX + (position.RightCenterX - positionX);

                int width = RightX - x;
                int y = positionY - ((width / 8) * 2);
                int bottomY = positionY + ((width / 8) * 6);
                int height = bottomY - y;

                setCameraPosition(position.AvgDistance, x, y, width, height);
            }
            else
            {
                setCameraPosition(-1, -1, -1, -1, -1);
            }
        }


        void setCameraPosition(int avgDistance, int x, int y, int width, int height)
        {
            //qDebug("X (%d) Y(%d) width(%d) height(%d)", x, y, width, height);
            //setCameraPositionShow(true);
            //_pRect->setRect(x, y, width, height);

            /// QPixmap pixmap;

            if (!_gIrisMode)
            {
                if (avgDistance > 0)
                {
                    pictureBox_faceTracking.Show();
                    pictureBox_faceTracking.Width = width / 2;
                    pictureBox_faceTracking.Height = height / 2;
                    pictureBox_faceTracking.Location = new Point(x / 2, y / 2);

                    int fileNameIndex = (width - 50) / 10 + 2;
                    if (fileNameIndex <= 0)
                        fileNameIndex = 1;

                    /// QString fileName;
                    Image img;

                    if (_gFaceTrackingBlink < 3)
                    {
                        _gFaceTrackingBlink++;

                        if (fileNameIndex < 10)
                        {
                            img = new Bitmap(faceTrackingOnImg[fileNameIndex], width / 2, height / 2);
                            /// fileName = QString(":/images/camera_d/face_tracking_on/face_tracking_0%1_on.png").arg(fileNameIndex);
                        }
                        else
                        {
                            img = new Bitmap(faceTrackingOnImg[fileNameIndex], width / 2, height / 2);
                            /// fileName = QString(":/images/camera_d/face_tracking_on/face_tracking_%1_on.png").arg(fileNameIndex);
                        }
                    }
                    else
                    {
                        _gFaceTrackingBlink++;

                        if (fileNameIndex < 10)
                        {
                            img = new Bitmap(faceTrackingOffImg[fileNameIndex], width / 2, height / 2);
                            /// fileName = QString(":/images/camera_d/face_tracking_off/face_tracking_0%1_off.png").arg(fileNameIndex);
                        }
                        else
                        {
                            img = new Bitmap(faceTrackingOffImg[fileNameIndex], width / 2, height / 2);
                            /// fileName = QString(":/images/camera_d/face_tracking_off/face_tracking_%1_off.png").arg(fileNameIndex);
                        }

                        if (_gFaceTrackingBlink == 6)
                            _gFaceTrackingBlink = 0;
                    }

                    /// QImage img = QImage(fileName);
                    /// _pFaceTracking->setPixmap(pixmap.fromImage(img).scaled(width, height, Qt::IgnoreAspectRatio, Qt::SmoothTransformation));
                    pictureBox_faceTracking.Image = img;

                    if (avgDistance >= (int)Client.UMXDistance.CAMERA_ENROLL_BLUE_MIN && avgDistance < (int)Client.UMXDistance.CAMERA_ENROLL_BLUE)
                    {
                        //green
                        faceGuide(x, y, (int)Client.UMXDistance.CAMERA_ENROLL_GREEN, fileNameIndex);
                    }
                    else if (avgDistance < (int)Client.UMXDistance.CAMERA_ENROLL_BLUE_MIN)
                    {
                        //red
                        faceGuide(x, y, (int)Client.UMXDistance.CAMERA_ENROLL_RED, fileNameIndex);
                    }
                    else
                    {
                        //blue
                        faceGuide(x, y, (int)Client.UMXDistance.CAMERA_ENROLL_BLUE, fileNameIndex);
                    }
                }
                else
                {
                    //face tracking disapper
                    pictureBox_faceTracking.Hide();
                    //pictureBox_faceTracking.Location = new Point(1000, 1000);
                }
            }
            else
            {
                if (avgDistance > 0)
                {
                    /// _pIrisTracking->setX(x);
                    /// _pIrisTracking->setY(y + 25);
                    pictureBox_irisTracking.Show();
                    pictureBox_irisTracking.Width = width / 2;
                    pictureBox_irisTracking.Height = (height / 2 - 60) / 2 + 15;
                    pictureBox_irisTracking.Location = new Point(x / 2, (y + 25) / 2);

                    int fileNameIndex = (width - 100) / 16 + 2;
                    if (fileNameIndex <= 0)
                        fileNameIndex = 1;

                    /// QString fileName;
                    Image img;

                    if (_gIrisTrackingBlink < 3)
                    {
                        _gIrisTrackingBlink++;

                        fileNameIndex--;
                        if (fileNameIndex < 10)
                        {
                            img = new Bitmap(irisTrackingOnImg[fileNameIndex], width / 2, (height / 2 - 60) / 2 + 15);
                            /// fileName = QString(":/images/camera_d/iris_tracking_on/iris_tracking_0%1_on.png").arg(fileNameIndex);
                        }
                        else if (fileNameIndex <= 26)
                        {
                            ////pictureBox_irisTracking.Width = irisTrackingOnImg[fileNameIndex].Width / 2;
                            ////pictureBox_irisTracking.Height = irisTrackingOnImg[fileNameIndex].Height / 2;
                            ////img = new Bitmap(irisTrackingOnImg[fileNameIndex], irisTrackingOnImg[fileNameIndex].Width / 2, irisTrackingOnImg[fileNameIndex].Height / 2);
                            img = new Bitmap(irisTrackingOnImg[fileNameIndex], width / 2, (height / 2 - 60) / 2 + 15);
                            /// fileName = QString(":/images/camera_d/iris_tracking_on/iris_tracking_%1_on.png").arg(fileNameIndex);
                        }
                        else
                        {
                            img = new Bitmap(irisTrackingOnImg[26], width / 2, (height / 2 - 60) / 2 + 15);
                        }
                    }
                    else
                    {
                        _gIrisTrackingBlink++;

                        if (fileNameIndex < 10)
                        {
                            img = new Bitmap(irisTrackingOffImg[fileNameIndex], width / 2, (height / 2 - 60) / 2 + 15);
                            /// fileName = QString(":/images/camera_d/iris_tracking_off/iris_tracking_0%1_off.png").arg(fileNameIndex);
                        }
                        else if (fileNameIndex <= 26)
                        {
                            img = new Bitmap(irisTrackingOffImg[fileNameIndex], width / 2, (height / 2 - 60) / 2 + 15);
                            /// fileName = QString(":/images/camera_d/iris_tracking_off/iris_tracking_%1_off.png").arg(fileNameIndex);
                        }
                        else
                        {
                            img = new Bitmap(irisTrackingOffImg[26], width / 2, (height / 2 - 60) / 2 + 15);
                        }

                        if (_gIrisTrackingBlink == 6)
                            _gIrisTrackingBlink = 0;
                    }

                    /// QImage img = QImage(fileName);
                    /// _pIrisTracking->setPixmap(pixmap.fromImage(img).scaled(width, height / 2 - 60, Qt::IgnoreAspectRatio, Qt::SmoothTransformation));
                    pictureBox_irisTracking.Image = img;
                }
                else
                {
                    /// _pIrisTracking->setX(1000);
                    /// _pIrisTracking->setY(1000);
                    pictureBox_irisTracking.Hide();
                    /// pictureBox_irisTracking.Location = new Point(1000, 1000);
                }
            }
        }


        void faceGuide(int x, int y, int color, int imageIndex)
        {
            //EF-45(jheekim)-31 : Face guide is move using face position
            if (imageIndex != -1)
            {
                if (imageIndex > 0 && imageIndex < 15)
                {
                    //small
                    //25.25 -> 90
                    x = x - (90 - (20 + (imageIndex * 10) / 2));
                    y = y - (90 - (20 + (imageIndex * 10) / 2));
                }
                else
                {
                    //bigger
                    x = x + (((imageIndex - 14) * 10) / 2);
                    y = y + (((imageIndex - 14) * 10) / 2);
                }

                if (x > 240) pictureBox_faceGuide.Location = new Point(120, pictureBox_faceGuide.Location.Y); /// _pFaceGuide->setX(240);
                else if (x < 60) pictureBox_faceGuide.Location = new Point(30, pictureBox_faceGuide.Location.Y); /// _pFaceGuide->setX(60);
                else pictureBox_faceGuide.Location = new Point(x / 2, pictureBox_faceGuide.Location.Y); /// _pFaceGuide->setX(x);

                if (y > 240) pictureBox_faceGuide.Location = new Point(pictureBox_faceGuide.Location.X, 120); /// _pFaceGuide->setY(240);
                else if (y < 0) pictureBox_faceGuide.Location = new Point(pictureBox_faceGuide.Location.X, 0); /// _pFaceGuide->setY(0);
                else pictureBox_faceGuide.Location = new Point(pictureBox_faceGuide.Location.X, y / 2); /// _pFaceGuide->setY(y);
            }

            /// QPixmap pixmap;

            switch (color)
            {
                case (int)Client.UMXDistance.CAMERA_ENROLL_GREEN:
                    {
                        pictureBox_faceGuide.Image = new Bitmap(Properties.Resources.face_guide_green_02, pictureBox_faceGuide.Width, pictureBox_faceGuide.Height);
                        /// QImage img = QImage(":/images/camera_d/face_guide_green_02.png");
                        /// _pFaceGuide->setPixmap(pixmap.fromImage(img).scaled(180, 180, Qt::IgnoreAspectRatio, Qt::SmoothTransformation));
                    }
                    break;
                case (int)Client.UMXDistance.CAMERA_ENROLL_BLUE:
                    {
                        pictureBox_faceGuide.Image = new Bitmap(Properties.Resources.face_guide_blue, pictureBox_faceGuide.Width, pictureBox_faceGuide.Height);
                        /// QImage img = QImage(":/images/camera_d/face_guide_blue.png");
                        /// _pFaceGuide->setPixmap(pixmap.fromImage(img).scaled(180, 180, Qt::IgnoreAspectRatio, Qt::SmoothTransformation));
                    }
                    break;
                case (int)Client.UMXDistance.CAMERA_ENROLL_RED:
                    {
                        pictureBox_faceGuide.Image = new Bitmap(Properties.Resources.face_guide_red, pictureBox_faceGuide.Width, pictureBox_faceGuide.Height);
                        /// QImage img = QImage(":/images/camera_d/face_guide_red.png");
                        /// _pFaceGuide->setPixmap(pixmap.fromImage(img).scaled(180, 180, Qt::IgnoreAspectRatio, Qt::SmoothTransformation));
                    }
                    break;
            }
        }

        void displayGuide(FaceIrisCapture faceIrisCapture)
        {
            _gIrisMode = faceIrisCapture.SwitchFlag;

            if (faceIrisCapture.AvgDistance > 0 || faceIrisCapture.RightBoundary > 0)
            {
                if (faceIrisCapture.LeftEyeCenterX > 0 &&
                        faceIrisCapture.LeftEyeCenterY > 0 && faceIrisCapture.RightEyeCenterX > 0 &&
                        faceIrisCapture.RightEyeCenterY > 0)
                {
                    CameraEyePosition position = new CameraEyePosition();
                    position.AvgDistance = faceIrisCapture.AvgDistance;
                    position.LeftCenterX = faceIrisCapture.LeftEyeCenterX;
                    position.LeftCenterY = faceIrisCapture.LeftEyeCenterY;
                    position.RightCenterX = faceIrisCapture.RightEyeCenterX;
                    position.RightCenterY = faceIrisCapture.RightEyeCenterY;

                    enrollEyePositionSlot(position);
                }
                else
                {
                    CameraEyePosition position = new CameraEyePosition();
                    position.AvgDistance = -1;

                    enrollEyePositionSlot(position);
                }

                alphaBTextBox_openEyesInfoTxt.Text = "";
                if (faceIrisCapture.SwitchFlag)
                {
                    pictureBox_irisGuide.Show();
                    pictureBox_irisTracking.Show();
                    pictureBox_faceGuide.Hide();
                    pictureBox_faceTracking.Hide();
                    Helpers.ShowStatusBar(MessageManager.msg_Iris_ProcessingData, parentForm.statusBar, true);

                    setUserDistance(faceIrisCapture.AvgDistance);

                    if (faceIrisCapture.AvgDistance < (int)Client.UMXDistance.CAMERA_ENROLL_RED && faceIrisCapture.AvgDistance != -1)
                    {
                        //red
                        //qDebug("RED");
                        colorTopIndicator(false, (int)Client.UMXDistance.CAMERA_ENROLL_RED);
                    }
                    else if ((int)Client.UMXDistance.CAMERA_ENROLL_RED <= faceIrisCapture.AvgDistance && faceIrisCapture.AvgDistance <= (int)Client.UMXDistance.CAMERA_ENROLL_GREEN)
                    {
                        //green
                        //qDebug("Green");
                        colorTopIndicator(false, (int)Client.UMXDistance.CAMERA_ENROLL_GREEN);

                        // Notice : Open your eyes wide
                        if (faceIrisCapture.Message == (int)Client.UMXMessage.UMXCAM_MESSAGE_OPEN_EYES_WIDE)
                            alphaBTextBox_openEyesInfoTxt.Text = "Open your eyes wide" /*USER_ENROLL_CAMERA_OPEN_WIDE_EYES_TEXT*/;
                    }
                    else
                    {
                        //blue
                        //qDebug("Blue");
                        colorTopIndicator(false, (int)Client.UMXDistance.CAMERA_ENROLL_BLUE);
                    }

                    //If cover face using hand, eye value is -1
                }
                else
                {
                    pictureBox_irisGuide.Hide();
                    pictureBox_irisTracking.Hide();
                    pictureBox_faceGuide.Show();
                    pictureBox_faceTracking.Show();

                    if (faceIrisCapture.AvgDistance < (int)Client.UMXDistance.CAMERA_ENROLL_BLUE_MIN && faceIrisCapture.AvgDistance != -1)
                    {
                        //red
                        colorTopIndicator(true, (int)Client.UMXDistance.CAMERA_ENROLL_RED);
                    }
                    else if ((int)Client.UMXDistance.CAMERA_ENROLL_BLUE_MIN <= faceIrisCapture.AvgDistance && faceIrisCapture.AvgDistance <= (int)Client.UMXDistance.CAMERA_ENROLL_BLUE)
                    {
                        //green
                        colorTopIndicator(true, (int)Client.UMXDistance.CAMERA_ENROLL_GREEN);
                    }
                    else
                    {
                        //blue
                        colorTopIndicator(true, (int)Client.UMXDistance.CAMERA_ENROLL_BLUE);
                    }
                }
            }
        }

        void setUserDistance(int avgDistance)
        {
            if (avgDistance >= (int)Client.UMXDistance.CAMERA_ENROLL_GREEN_MIN && avgDistance <= (int)Client.UMXDistance.CAMERA_ENROLL_GREEN)
            {
                //green
                irisGuide((int)Client.UMXDistance.CAMERA_ENROLL_GREEN);
            }
            else if (avgDistance < (int)Client.UMXDistance.CAMERA_ENROLL_GREEN_MIN && avgDistance != -1)
            {
                //red
                irisGuide((int)Client.UMXDistance.CAMERA_ENROLL_RED);
            }
            else
            {
                //blue
                irisGuide((int)Client.UMXDistance.CAMERA_ENROLL_BLUE);
            }
        }

        void irisGuide(int color)
        {
            if (_gIrisGuideColor != color)
            {
                switch (color)
                {
                    case (int)Client.UMXDistance.CAMERA_ENROLL_GREEN:
                        {
                            pictureBox_irisGuide.Image = new Bitmap(Properties.Resources.Iris_guide_green_02, pictureBox_irisGuide.Width, pictureBox_irisGuide.Height);
                        }
                        break;
                    case (int)Client.UMXDistance.CAMERA_ENROLL_BLUE:
                        {
                            pictureBox_irisGuide.Image = new Bitmap(Properties.Resources.Iris_guide_blue, pictureBox_irisGuide.Width, pictureBox_irisGuide.Height);
                        }
                        break;
                    case (int)Client.UMXDistance.CAMERA_ENROLL_RED:
                        {
                            pictureBox_irisGuide.Image = new Bitmap(Properties.Resources.Iris_guide_red, pictureBox_irisGuide.Width, pictureBox_irisGuide.Height);
                        }
                        break;
                }

                _gIrisGuideColor = color;
            }
            else
            {
                //skip
            }
        }

        #endregion

        private void getLogButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && _client.IsConnected())
            {
                if (tbxLogID.Text != "")
                {
                    int logUID = Int32.Parse(tbxLogID.Text);
                    try
                    {
                        Log log = _client.GetLog(logUID);
                        if (log.LogUID == logUID)
                        {

                            string message = log.EventType + " " + log.LogUID + " " + log.Info + " " + log.AdditionalData;
                            m_logMessages.AddMessage(LogMessages.Icon.Information, log.Timestamp.ToLocalTime(), message);
                            Helpers.ShowStatusBar(message, parentForm.statusBar, true);

                            panelPrevivew.BackgroundImage = scaleDown(log.MatchedFaceImage, panelPrevivew.Width, panelPrevivew.Height);
                        }
                    }
                    catch (ArgumentException exception) { AddMessage(exception.Message, LogMessages.Icon.Error, displayMessage: true); }

                }
                else
                {
                    LogListForm logListForm = new LogListForm();
                    List<Log> logs = _client.GetLogs();
                    foreach (Log log in logs)
                    {
                        logListForm.LogUIDs.Add(log.LogUID);
                    }
                    if (logListForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Log log = _client.GetLog(logListForm.LogUID);
                            string message = log.EventType + " " + log.LogUID + " " + log.Info + " " + log.AdditionalData;
                            m_logMessages.AddMessage(LogMessages.Icon.Information, log.Timestamp.ToLocalTime(), message);
                            Helpers.ShowStatusBar(message, parentForm.statusBar, true);

                            panelPrevivew.BackgroundImage = scaleDown(log.MatchedFaceImage, panelPrevivew.Width, panelPrevivew.Height);
                        }
                        catch (ArgumentException exception) { AddMessage(exception.Message, LogMessages.Icon.Error, displayMessage: true); }
                    }
                }
            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            DialogResult rslt = MessageBox.Show(MessageManager.msg_Iris_ClearAutoLogs, MessageManager.msg_Iris_ClearMessageList, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rslt == DialogResult.Yes)
            {
                m_logMessages.ClearMessages();
            }

        }

        private void btnCheckConnection_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && _client.IsConnected())
            {
                AddMessage(MessageManager.msg_Iris_Connected, LogMessages.Icon.Information, displayMessage: true);
                ToggleControls(true);
            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);
        }

        private void btnUpdateSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(MessageManager.msg_Iris_UpdateServerIP, MessageManager.msg_Iris_UpdateSettings, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Properties.Settings.Default.IrisServerIP = tbxServiceIP.Text.Trim();
                Properties.Settings.Default.Save();
                Helpers.ShowStatusBar(MessageManager.msg_Iris_SettingsUpdated, parentForm.statusBar, true);
            }
        }

        private void tpInteraction_Click(object sender, EventArgs e)
        {

        }

        private async void startEnrollButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();
            if (_client != null && _client.IsConnected())
            {
                if (enrollRadioButton.Checked == false)
                {
                    AddMessage(MessageManager.msg_Iris_ChangeModeToEnroll, LogMessages.Icon.Information, displayMessage: true);
                    return;
                }

                if (!_client.IsCaptureStarted())
                {
                    ResetForNewConnection();
                    this.alphaBTextBox_topFaceInfoBack.Hide();
                    this.pictureBox_irisTracking.Hide();
                    this.pictureBox_faceTracking.Hide();

                    _startEnrollCaptureCompleted = false;
                    _enrollCompleted = false;
                    _bigOffFaceCanEnroll = true;

                    bool faceMode = false; ;
                    bool glassesMode = false;
                    bool bothEyeMode = false;
                    bool faceFullResolution = false;
                    bool eitherEyeMode = true;

                    try
                    {
                        AddMessage(MessageManager.msg_Iris_InitCapture, LogMessages.Icon.Information, displayMessage: true);

                        Task captureTask = Task.Run(() => _client.StartCapture(60, faceMode, glassesMode, bothEyeMode, eitherEyeMode, faceFullResolution));
                        await captureTask;

                        AddMessage(MessageManager.msg_Iris_StandBeforeTheDevice, LogMessages.Icon.Information, displayMessage: true);

                    }
                    catch (Exception ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }
                }
                else
                    AddMessage(MessageManager.msg_Iris_DeviceIsStarted, LogMessages.Icon.Information, displayMessage: true);
            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);

        }


        private void stopUmxButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();
            if (_client != null && _client.IsConnected())
            {
                if (enrollRadioButton.Checked == false)
                {
                    AddMessage(MessageManager.msg_Iris_ChangeModeToEnroll, LogMessages.Icon.Information, displayMessage: true);
                    Cursor.Current = Cursors.Default;
                    return;
                }

                if (_client.IsCaptureStarted())
                {
                    ResetForNewConnection();

                    try
                    {
                        _client.StopCapture();
                    }
                    catch (Exception ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }
                }
                else AddMessage(MessageManager.msg_Iris_DeviceIsNotRunning, LogMessages.Icon.Information, displayMessage: true);
            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);


        }


     
        private void deleteAllLogsButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && _client.IsConnected())
            {
                try { _client.DeleteLog(); }
                catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }

                try { _client.VacuumSerivceLog(); }
                catch (Exception exception)
                { AddMessage("Failed to vacuum ServiceLog.db (" + exception.Message + ")", LogMessages.Icon.Error, displayMessage: true); }
            }
            else
            {
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);
            }
        }

        private void getLogsButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && _client.IsConnected())
            {
                try
                {
                    List<Log> logs = _client.GetLogs();
                    foreach (Log log in logs)
                    {
                        m_logMessages.AddMessage(LogMessages.Icon.Information, log.Timestamp.ToLocalTime(), log.EventType + " " + log.LogUID +
                            " " + log.Info + " " + log.AdditionalData);

                        panelPrevivew.BackgroundImage = scaleDown(log.MatchedFaceImage, panelPrevivew.Width, panelPrevivew.Height);
                    }
                }
                catch (DeviceNotFoundException ex)
                {
                    AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true);
                }
            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);

        }

        private void btnClearImages_Click(object sender, EventArgs e) { ResetForNewConnection(); }


        private void deleteAllUsersButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && _client.IsConnected())
            {
                try
                {
                    if (MessageBox.Show(MessageManager.msg_Iris_RemoveAllUserData, "Remove All Users Information", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        List<Subject> subjects = _client.GetSubjects();
                        foreach (Subject subject in subjects)
                        {
                            try
                            {
                                _client.DeleteSubject(subject.SubjectUID);
                            }
                            catch (Exception exception)
                            {
                                AddMessage("Failed to delete subject " + subject.SubjectUID + " (" + exception.Message + ")", LogMessages.Icon.Error);
                            }
                        }
                        AddMessage("Finished deleting subjects.", LogMessages.Icon.Information, displayMessage: true);


                        List<Face> faces = _client.GetFaces();
                        foreach (Face face in faces)
                        {
                            try
                            {
                                _client.DeleteFace(face.FaceUID, face.SubId);
                            }
                            catch (Exception exception)
                            {
                                AddMessage("Failed to delete face " + face.FaceUID + " (" + exception.Message + ")", LogMessages.Icon.Error);
                            }
                        }
                        AddMessage(MessageManager.msg_Iris_FinishedDeletingFaces, LogMessages.Icon.Information, displayMessage: true);



                        List<UserInfo> allUserInfo = _client.GetAllUserInfo();
                        foreach (UserInfo userinfo in allUserInfo)
                        {
                            try
                            {
                                _client.DeleteUserInfoByUUID(userinfo.UUID);
                            }
                            catch (Exception exception)
                            {
                                AddMessage("Failed to delete userinfo " + userinfo.UUID + " (" + exception.Message + ")", LogMessages.Icon.Error);
                            }
                        }
                        AddMessage(MessageManager.msg_Iris_AllUserInfoDelted, LogMessages.Icon.Information, displayMessage: true);

                    }
                    else { return; }
                }
                catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }

                try { _client.VacuumSerivce(); }
                catch (Exception exception)
                { AddMessage("Failed to vacuum Service.db (" + exception.Message + ")", LogMessages.Icon.Error, displayMessage: true); }

            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);

        }

        private void listSubjectsButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client != null && _client.IsConnected())
            {
                try
                {
                    if (tbxUserID.Text != "") // Get subject by uuid
                    {
                        string uuid = tbxUserID.Text;
                        Subject subject = _client.GetSubject(uuid);
                        if (subject.SubjectUID == uuid) AddMessage("Subject: " + subject.SubjectUID + " " + subject.FirstName + " " + subject.LastName, LogMessages.Icon.Information);
                        else AddMessage("Subject: " + uuid + " is not exist.", LogMessages.Icon.Information);

                    }
                    else // Get all
                    {
                        List<Subject> subjects = _client.GetSubjects();
                        if (subjects.Count == 0)
                            AddMessage("No subjects enrolled on device.", LogMessages.Icon.Warning);
                        else
                            foreach (Subject subject in subjects)
                                AddMessage("Subject: " + subject.SubjectUID + " " + subject.FirstName + " " + subject.LastName, LogMessages.Icon.Information);
                    }
                }
                catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }
            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);

        }

        private string GetUserName()
        {
			 return "John doe";
            //return parentForm.GetUserName();
        }


        public void ClearAll()
        {
            ResetForNewConnection();
            if (_client != null)
            {
                try { _client.Close(); }
                catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }
            }
        }


        private void SyncTime()
        {
            string date = Helpers.GetSyncDate();

            try
            {
                _client.SetDeviceTime(date);
                AddMessage(MessageManager.msg_Iris_DeviceTimeSetSuccess + date, LogMessages.Icon.Information, displayMessage: true);
            }
            catch (DeviceNotFoundException ex)
            { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }
            catch (Exception exception)
            { AddMessage(MessageManager.msg_Iris_DeviceTimeSetFailed + date + " (" + exception.Message + ")", LogMessages.Icon.Error, displayMessage: true); }
        }



        private void btnTestEnroll_Click(object sender, EventArgs e) { TestEnroll(); }

       
        public void TestEnroll()
        {

            string userID = "11111111";
            string firstName = "John";
            string lastName = "Doe";
            string enrollData = "<User Template Goes Here>";
            EnrolTemplate template = Helpers.EncodedStringToEnrollTemplate(enrollData);

            ClearDeviceUser();

            Subject subject = new Subject();
            subject.AccessAllowed = true;
            subject.FirstName = firstName;
            subject.LastName = lastName; // Required
            subject.EnrolTemplate = template;
            subject.SubjectUID = userID;

            subject.WiegandSite = -1;
            subject.WiegandFacility = -1;
            subject.WiegandCode = -1;

            _client.AddSubject(subject);

            UserInfo userInfo = new UserInfo();
            userInfo.UUID = userID;
            userInfo.Card = ""; /* Card CSN : 9A 99 1F 3E ...*/
            userInfo.Pin = "";
            userInfo.Admin = 0;
            userInfo.GroupIndex = 0;
            userInfo.ByPassCard = 0;
            userInfo.Indivisual = 0;

            _client.AddUserInfo(userInfo);

        }


        public void ReEnrollUser(bool enableEnroll, bool clearDeviceUser = true)
        {
            ResetForNewConnection();

            if (clearDeviceUser && _client != null && _client.IsConnected()) ClearDeviceUser();

            if (enableEnroll) btnClickToEnroll.Enabled = true;
        }


        private void ClearDeviceUser()
        {
            try { _client.DeleteSubject(currentUID); } catch { }

            try { _client.DeleteUserInfoByUUID(currentUID); } catch { }
        }



        private void enrolButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            string userName = GetUserName();

            if (_client != null && _client.IsConnected())
            {
                if (recogRadioButton.Checked) { Helpers.ShowStatusBar(MessageManager.msg_Iris_ChangeEnrollMode, parentForm.statusBar, false); return; }

                if (_enrollCompleted) { Helpers.ShowStatusBar(MessageManager.msg_Iris_EnrollDataUsed, parentForm.statusBar, false); return; }

                if (!_bigOffFaceCanEnroll) { Helpers.ShowStatusBar(MessageManager.msg_Iris_CantUseBigOff, parentForm.statusBar, false); return; }

                if (!_startEnrollCaptureCompleted) { Helpers.ShowStatusBar(MessageManager.msg_Iris_EnrollFailed, parentForm.statusBar, false); return; }


                try { _client.DeleteSubject(currentUID); }
                catch { }

                try { _client.DeleteUserInfoByUUID(currentUID); }
                catch { }

                try
                {
                    Subject subject = _client.GetSubject(currentUID);
                    if (subject.SubjectUID == currentUID)
                    {
                        AddMessage(MessageManager.msg_Iris_UserExists, LogMessages.Icon.Error, displayMessage: true);
                        return;
                    }
                }
                catch (ArgumentException) { } // Subject "" does not exist


                if (_currentEnrolTemplate != null)
                {
                    Subject subject = new Subject();
                    subject.AccessAllowed = true;
                    //subject.FirstName = enrolForm.FirstName;
                    subject.LastName = userName; // Required
                    subject.EnrolTemplate = _currentEnrolTemplate;
                    //subject.SubjectUID = Guid.NewGuid().ToString();
                    subject.SubjectUID = currentUID;

                    subject.WiegandSite = -1;
                    subject.WiegandFacility = -1;
                    subject.WiegandCode = -1;

                    try
                    {
                        _client.AddSubject(subject);
                        AddMessage("Successfully added the subject " + userName, LogMessages.Icon.Information, displayMessage: true);

                    }
                    catch (DeviceNotFoundException ex)
                    {
                        AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true);
                    }
                    catch (Exception exception)
                    {
                        AddMessage("Unable to add the subject " + userName + " (" + exception.Message + ")", LogMessages.Icon.Error, displayMessage: true);

                        _currentEnrolTemplate = null;
                        _enrollCompleted = true;
                        return;
                    }
                }


                UserInfo userInfo = new UserInfo();
                userInfo.UUID = currentUID;
                userInfo.Card = string.Empty; /* Card CSN : 9A 99 1F 3E ...*/
                userInfo.Pin = string.Empty;
                userInfo.Admin = 0;
                userInfo.GroupIndex = 0;
                userInfo.ByPassCard = 0;
                userInfo.Indivisual = 0;

                try
                {
                    _client.AddUserInfo(userInfo);

                    AddMessage("User information of  " + userName + " was added successfully.", LogMessages.Icon.Information, displayMessage: true);

                }
                catch (DeviceNotFoundException ex) { AddMessage(ex.Message, LogMessages.Icon.Error, displayMessage: true); }
                catch (Exception exception)
                {
                    AddMessage("Unable to add the user information of " + userName + " (" + exception.Message + ")", LogMessages.Icon.Error, displayMessage: true);
                    _currentEnrolTemplate = null;
                    _enrollCompleted = true;
                    return;
                }

                string enrollTemplate = Helpers.EnrollTemplateToEncodedString(_currentEnrolTemplate);



                // parentForm.SetBioMetricTemplate(enrollTemplate, DeviceType);

                _currentEnrolTemplate = null;
                _enrollCompleted = true;
            }
            else
                AddMessage(MessageManager.msg_Iris_NotConnected, LogMessages.Icon.Warning, displayMessage: true);

        }







        #region ********** SWITCH DEVICE MODE **********


        /// <summary>
        /// Enroll Mode Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enrollRadioButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client == null || _client.IsConnected() == false)
            {
                enrollRadioButton.Checked = false;
                return;
            }

            Client.UMXMode mode = _client.GetMode();
            switch (mode)
            {
                // PC Control UI
                case Client.UMXMode.Slave: recogRadioButton.Checked = false; return;

                // Launcher or Recognition UI
                case Client.UMXMode.Recog:
                    try
                    {
                        _client.SetMode(Client.UMXMode.Slave);
                        recogRadioButton.Checked = false;
                        this.tbCtrl.SelectedIndex = 0;
                        AddMessage(MessageManager.msg_Iris_PressStartToCapture, LogMessages.Icon.Information, displayMessage: true);

                    }
                    catch (Exception exception)
                    {
                        AddMessage(exception.Message, LogMessages.Icon.Error, displayMessage: true);
                        enrollRadioButton.Checked = false;
                    }
                    break;

                // User or Settings UI
                case Client.UMXMode.Enrol:
                    AddMessage(MessageManager.msg_Iris_DeviceIsInUse, LogMessages.Icon.Information, displayMessage: true);
                    enrollRadioButton.Checked = false;
                    return;
            }


            ResetForNewConnection();

            _startEnrollCaptureCompleted = false;
        }



        /// <summary>
        /// Recognition Mode Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recogRadioButton_Click(object sender, EventArgs e)
        {
            ClearDispayMessage();

            if (_client == null || _client.IsConnected() == false)
            {
                recogRadioButton.Checked = false;
                return;
            }

            Client.UMXMode mode = _client.GetMode();
            switch (mode)
            {
                case Client.UMXMode.Slave: // PC Control UI
                    try
                    {
                        _client.SetMode(Client.UMXMode.Recog);
                        enrollRadioButton.Checked = false;
                        this.tbCtrl.SelectedIndex = 1;
                    }
                    catch (Exception exception)
                    {
                        AddMessage(exception.Message, LogMessages.Icon.Error);
                        recogRadioButton.Checked = false;
                    }
                    break;
                case Client.UMXMode.Recog: // Launcher or Recognition UI
                    enrollRadioButton.Checked = false;
                    return;
            }

            _bigOffFaceCanEnroll = true;
            _startEnrollCaptureCompleted = false;

            ResetForNewConnection();
        }



        #endregion


    }
}
