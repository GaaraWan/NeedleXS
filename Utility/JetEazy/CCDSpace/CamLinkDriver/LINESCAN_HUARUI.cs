﻿
using Euresys.MultiCam;
using JetEazy.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JetEazy.CCDSpace.CamLinkDriver
{
    public class LINESCAN_HUARUI : IxLineScanCam
    {

        #region PRIVATE VAR

        // Creation of an event for asynchronous call to paint function
        public delegate void PaintDelegate(Graphics g);
        public delegate void UpdateStatusBarDelegate(String text);

        // The object that will contain the acquired image
        private Bitmap image = null;

        // The object that will contain the palette information for the bitmap
        private ColorPalette imgpal = null;

        // The Mutex object that will protect image objects during processing
        private static Mutex imageMutex = new Mutex();

        // The MultiCam object that controls the acquisition
        UInt32 channel;

        // The MultiCam object that contains the acquired buffer
        private UInt32 currentSurface;

        MC.CALLBACK multiCamCallback;
        bool m_TriggerOK = false;
        bool m_IsDebug = false;
        bool m_TriggerComplete = false;
        string m_TrigCtl = "ISO";

        #endregion

        public void Init(bool debug, string inipara)
        {
            m_IsDebug = debug;
            m_TrigCtl = inipara;
        }
        public bool IsSim()
        {
            return m_IsDebug;
        }
        public bool Open()
        {
            return Open("BASE");
        }
        public bool Open(string configFile)
        {

            if (m_IsDebug)
                return true;

            try
            {
                m_TriggerOK = false;
                // Open MultiCam driver
                MC.OpenDriver();

                // Enable error logging
                MC.SetParam(MC.CONFIGURATION, "ErrorLog", "error.log");

                // In order to support a 10-tap camera on Grablink Full
                // BoardTopology must be set to MONO_DECA
                // In all other cases the default value will work properly 
                // and the parameter doesn't need to be set

                // Set the board topology to support 10 taps mode (only with a Grablink Full)
                // MC.SetParam(MC.BOARD + 0, "BoardTopology", "MONO_DECA");

                // Create a channel and associate it with the first connector on the first board
                MC.Create("CHANNEL", out channel);
                MC.SetParam(channel, "DriverIndex", 0);

                // In order to use single camera on connector A
                // MC_Connector must be set to A for Grablink DualBase
                // For all other Grablink boards the parameter has to be set to M  

                // For all GrabLink boards except Grablink DualBase
                MC.SetParam(channel, "Connector", "M");
                // For Grablink DualBase
                //MC.SetParam(channel, "Connector", "A");

                // Choose the CAM file
                //MC.SetParam(channel, "CamFile", "1000m_P50RG");
                //MC.SetParam(channel, "CamFile", "DAHUA_BASE");
                MC.SetParam(channel, "CamFile", configFile);
                //MC.SetParam(channel, "CamFile", "C:\\Users\\Public\\Documents\\Euresys\\MultiCam\\Cameras\\_TEMPLATES\\Grablink\\MyCameraLink_LxxxxSP.cam");


                #region 单端信号触发 配置

                string outparastr = "";
                MC.SetParam(channel, "TapConfiguration", "FULL_8T8");
                MC.SetParam(channel, "TapGeometry", "1X8");

                MC.GetParam(channel, "TapConfiguration", out outparastr);
                MC.GetParam(channel, "TapGeometry", out outparastr);

                MC.SetParam(channel, "Hactive_Px", "8000");
                MC.SetParam(channel, "LineRate_Hz", "1200000");

                MC.SetParam(channel, "AcquisitionMode", "PAGE");
                MC.SetParam(channel, "NextTrigMode", "SAME");
                MC.SetParam(channel, "TrigMode", "COMBINED");

                MC.SetParam(channel, "SeqLength_Pg", "-1");
                MC.SetParam(channel, "PageLength_Ln", "65535");//这个先后顺序有影响 必须在AcquisitionMode 之后

                MC.GetParam(channel, "AcquisitionMode", out outparastr);
                MC.GetParam(channel, "NextTrigMode", out outparastr);
                MC.GetParam(channel, "TrigMode", out outparastr);

                MC.GetParam(channel, "PageLength_Ln", out outparastr);
                MC.GetParam(channel, "LineCaptureMode", out outparastr);
                MC.GetParam(channel, "LineRateMode", out outparastr);
                Int32 width, height, bufferPitch;

                MC.GetParam(channel, "ImageSizeX", out width);
                MC.GetParam(channel, "ImageSizeY", out height);
                MC.GetParam(channel, "BufferPitch", out bufferPitch);
                //MC.GetParam(currentSurface, "SurfaceAddr", out bufferAddress);

                switch(m_TrigCtl)
                {
                    case "DIFF":
                        //分倍频
                        MC.SetParam(channel, "TrigCtl", "DIFF");
                        MC.SetParam(channel, "TrigEdge", "GOHIGH");
                        MC.SetParam(channel, "TrigLine", "DIN2");
                        break;
                    default:
                        //单端信号触发
                        MC.SetParam(channel, "TrigCtl", "ISO");
                        MC.SetParam(channel, "TrigEdge", "GOHIGH");
                        MC.SetParam(channel, "TrigLine", "IIN1");
                        break;
                }

               

                MC.GetParam(channel, "TrigCtl", out outparastr);
                MC.GetParam(channel, "TrigEdge", out outparastr);
                MC.GetParam(channel, "TrigLine", out outparastr);

                MC.SetParam(channel, "AcqTimeout_ms", "-1");
                MC.GetParam(channel, "AcqTimeout_ms", out outparastr);

                //MC.SetParam(channel, "ImageFlipX", "ON");
                //MC.SetParam(channel, "ImageFlipY", "ON");

                #endregion


                // Register the callback function
                multiCamCallback = new MC.CALLBACK(MultiCamCallback);
                MC.RegisterCallback(channel, multiCamCallback, channel);

                // Enable the signals corresponding to the callback functions
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_SURFACE_PROCESSING, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_ACQUISITION_FAILURE, "ON");

                // Prepare the channel in order to minimize the acquisition sequence startup latency
                MC.SetParam(channel, "ChannelState", "READY");


                // + GrablinkSnapshotTrigger Sample Program


                // Start an acquisition sequence by activating the channel
                String channelState;
                MC.GetParam(channel, "ChannelState", out channelState);
                if (channelState != "ACTIVE")
                    MC.SetParam(channel, "ChannelState", "ACTIVE");

                // Generate a soft trigger event
                //MC.SetParam(channel, "ForceTrig", "TRIG");
                //Refresh();

                // - GrablinkSnapshotTrigger Sample Program
                m_TriggerOK = false;

                return true;
            }
            catch (Euresys.MultiCamException exc)
            {
                // An exception has occurred in the try {...} block. 
                // Retrieve its description and display it in a message box.
                //MessageBox.Show(exc.Message, "MultiCam Exception");
                //Close();
                return false;
            }

            return false;
        }
        public bool Close()
        {
            if (m_IsDebug)
                return true;
            try
            {
                m_TriggerOK = false;
                // + GrablinkSnapshotTrigger Sample Program
                // Stop an acquisition sequence by deactivating the channel
                if (channel != 0)
                    MC.SetParam(channel, "ChannelState", "IDLE");
                //UpdateStatusBar(String.Format("Frame Rate: {0:f2}, Channel State: IDLE", 0));

                // - GrablinkSnapshotTrigger Sample Program
                // Delete the channel
                if (channel != 0)
                {
                    MC.Delete(channel);
                    channel = 0;
                }

                MC.CloseDriver();

                return true;
            }
            catch (Euresys.MultiCamException exc)
            {
                //MessageBox.Show(exc.Message, "MultiCam Exception");

                return false;
            }

            //try
            //{
            //    // Close MultiCam driver
                
            //}
            //catch (Euresys.MultiCamException exc)
            //{
            //    //MessageBox.Show(exc.Message, "MultiCam Exception");
            //}

            return false;
        }
        public void SoftTrigger()
        {
            if (m_IsDebug)
                return;
            // + GrablinkSnapshotTrigger Sample Program

            m_TriggerOK = false;
            // Start an acquisition sequence by activating the channel
            String channelState;
            MC.GetParam(channel, "ChannelState", out channelState);
            if (channelState != "ACTIVE")
                MC.SetParam(channel, "ChannelState", "ACTIVE");

            // Generate a soft trigger event
            MC.SetParam(channel, "ForceTrig", "TRIG");
            //Refresh();

            // - GrablinkSnapshotTrigger Sample Program
        }
        public bool IsGrapImageOK
        {
            get { return m_TriggerOK; }
            set { m_TriggerOK = value; }
        }
        public bool IsGrapImageComplete
        {
            get { return m_TriggerComplete; }
            set { m_TriggerComplete = value; }
        }
        /// <summary>
        /// 获取图像
        /// </summary>
        /// <param name="size">大于0 放大倍数 等于0 不变尺寸 小于0缩小倍数</param>
        /// <returns>返回图像</returns>
        public Bitmap GetPageBitmap(int size)
        {
            if (m_IsDebug)
                return new Bitmap(1, 1);
            if (image == null)
            {
                m_TriggerOK = false;
                return null;
            }

            //缩小一倍

            if (m_TriggerOK)
            {
                Bitmap bmp1 = (Bitmap)image.Clone();
                Bitmap bmpLine = new Bitmap(1, 1);
                if (size > 0)
                    bmpLine = new Bitmap(bmp1, bmp1.Width << size, bmp1.Height << size);
                else if (size == 0)
                    bmpLine = (Bitmap)image.Clone();
                else
                    bmpLine = new Bitmap(bmp1, bmp1.Width >> size, bmp1.Height >> size);
                //bmp1.Dispose();
                return bmpLine;
            }
            //return (Bitmap)image.Clone();

            m_TriggerOK = false;
            return null;
        }
        public void Dispose()
        {
            //base.Equals(null);
            Close();
        }


        #region PRIVATE FUNTION

        private void MultiCamCallback(ref MC.SIGNALINFO signalInfo)
        {
            m_TriggerComplete = false;
            switch (signalInfo.Signal)
            {
                case MC.SIG_SURFACE_PROCESSING:
                    ProcessingCallback(signalInfo);
                    m_TriggerOK = true;
                    break;
                case MC.SIG_ACQUISITION_FAILURE:
                    AcqFailureCallback(signalInfo);
                    m_TriggerOK = false;
                    break;
                default:
                    m_TriggerOK = false;
                    break;
                    //throw new Euresys.MultiCamException("Unknown signal");
            }
            m_TriggerComplete = true;
        }

        private void ProcessingCallback(MC.SIGNALINFO signalInfo)
        {
            UInt32 currentChannel = (UInt32)signalInfo.Context;

            //statusBar.Text = "Processing";
            currentSurface = signalInfo.SignalInfo;

            // + GrablinkSnapshotTrigger Sample Program

            try
            {
                // Update the image with the acquired image buffer data 
                Int32 width, height, bufferPitch;
                IntPtr bufferAddress;
                MC.GetParam(currentChannel, "ImageSizeX", out width);
                MC.GetParam(currentChannel, "ImageSizeY", out height);
                MC.GetParam(currentChannel, "BufferPitch", out bufferPitch);
                MC.GetParam(currentSurface, "SurfaceAddr", out bufferAddress);

                try
                {
                    imageMutex.WaitOne();

                    image = new Bitmap(width, height, bufferPitch, PixelFormat.Format8bppIndexed, bufferAddress);

                    imgpal = image.Palette;

                    // Build bitmap palette Y8
                    for (uint i = 0; i < 256; i++)
                    {
                        imgpal.Entries[i] = Color.FromArgb(
                        (byte)0xFF,
                        (byte)i,
                        (byte)i,
                        (byte)i);
                    }

                    image.Palette = imgpal;

                    //image.Save("D:\\20230629_PULSE\\存图\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png", ImageFormat.Png);

                    /* Insert image analysis and processing code here */
                }
                finally
                {
                    imageMutex.ReleaseMutex();
                }

                // Retrieve the frame rate
                Double frameRate_Hz;
                MC.GetParam(channel, "PerSecond_Fr", out frameRate_Hz);

                // Retrieve the channel state
                String channelState;
                MC.GetParam(channel, "ChannelState", out channelState);

                //// Display frame rate and channel state
                //statusBar.Text = String.Format("Frame Rate: {0:f2}, Channel State: {1}", frameRate_Hz, channelState);

                //// Display the new image
                //this.BeginInvoke(new PaintDelegate(Redraw), new object[1] { CreateGraphics() });
            }
            catch (Euresys.MultiCamException exc)
            {
                //MessageBox.Show(exc.Message, "MultiCam Exception");
            }
            catch (System.Exception exc)
            {
                //MessageBox.Show(exc.Message, "System Exception");
            }
            // - GrablinkSnapshotTrigger Sample Program
        }

        private void AcqFailureCallback(MC.SIGNALINFO signalInfo)
        {
            UInt32 currentChannel = (UInt32)signalInfo.Context;

            // + GrablinkSnapshotTrigger Sample Program

            //try
            //{
            //    // Display frame rate and channel state
            //    statusBar.Text = String.Format("Acquisition Failure, Channel State: IDLE");
            //    this.BeginInvoke(new PaintDelegate(Redraw), new object[1] { CreateGraphics() });
            //}
            //catch (System.Exception exc)
            //{
            //    MessageBox.Show(exc.Message, "System Exception");
            //}

            // - GrablinkSnapshotTrigger Sample Program
        }

        #endregion

    }
}
