using JetEazy.BasicSpace;
using JetEazy;
using JetEazy.Interface;
using NeedleX.Model.OpencvTools;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Traveller106;
using VsCommon.ControlSpace.MachineSpace;
using System.IO;
using System.Threading;
using System.Diagnostics;
using NeedleX.ProcessSpace;
using VsCommon.ControlSpace;
using NeedleX.Model;
using OpenCvSharp.Flann;
using System.IO.Ports;
using NeedleX.Driver.Keyence;
using AForge.Imaging.Filters;
//using System.Web.UI.WebControls;

namespace NeedleX.FormSpace
{
    public partial class frmAutoFocus : Form
    {
        int xTestCurrentIndex = 0;
        string xRootPath = "D:\\IMAGE";
        Bitmap bmpLiving = new Bitmap(1, 1);

        Button btnGT2OpenClose;
        Button btnGT2Read;
        Button btnGT2ZStable;
        Label lblGT2Value;
        Label lblGT2Value2;
        Label lblGT2Value3;

        //FocusPARA myFocusPara = new FocusPARA();
        Button btnP1;
        Button btnP2;
        Button btnSaveSet;
        PropertyGrid xPG;
        Button btnSaveImage;
        Button btnClear;
        Button btnAutoTest;
        Label lblCount;
        Button btnFlyOpen;
        Button btnFlyAutoFind;

        System.Windows.Forms.Timer xMainTick = null;
       
        JetEazy.CCDSpace.CAMERAClass xCamFocus
        {
            get { return Traveller106.Universal.CAMERAS[1]; }
        }
        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Traveller106.Universal.MACHINECollection;
            }
        }
        JetEazy.ControlSpace.MotionSpace.PLCMotionClass ZAXIS
        {
            get
            {
                return ((NeedleMachineClass)MACHINECollection.MACHINE).PLCMOTIONCollection[2];
            }
        }
        NeedleMachineClass MACHINE
        {
            get { return (NeedleMachineClass)Traveller106.Universal.MACHINECollection.MACHINE; }
        }

        public frmAutoFocus()
        {
            InitializeComponent();

            this.Load += FrmAutoFocus_Load;
            this.FormClosed += FrmAutoFocus_FormClosed;
        }

        private void FrmAutoFocus_FormClosed(object sender, FormClosedEventArgs e)
        {
            xResetData();
            xCamFocus.FlyImage(false);
            INI.Instance.IsOpenFocusWindows = false;
            GT2Dispose();
        }

        private void FrmAutoFocus_Load(object sender, EventArgs e)
        {
            this.Text = "自动对焦稳定性测试";

            btnP1 = button2;
            btnP2 = button3;
            btnSaveSet = button1;
            xPG = propertyGrid1;
            btnSaveImage = button4;
            btnClear = button5;
            btnAutoTest = button6;
            lblCount = label1;
            btnFlyOpen = button7;
            btnFlyAutoFind = button8;

            btnGT2OpenClose = button9;
            btnGT2Read = button10;
            btnGT2ZStable = button11;
            lblGT2Value = label2;
            lblGT2Value2 = label6;
            lblGT2Value3 = label8;

            btnP1.Click += BtnP1_Click;
            btnP2.Click += BtnP2_Click;
            btnSaveSet.Click += BtnSaveSet_Click;
            btnSaveImage.Click += BtnSaveImage_Click;
            btnClear.Click += BtnClear_Click;
            btnAutoTest.Click += BtnAutoTest_Click;
            btnFlyOpen.Click += BtnFlyOpen_Click;
            btnFlyAutoFind.Click += BtnFlyAutoFind_Click;


            btnGT2OpenClose.Click += BtnGT2OpenClose_Click;
            btnGT2Read.Click += BtnGT2Read_Click;
            btnGT2ZStable.Click += BtnGT2ZStable_Click;
            btnGT2ZStable.Text = "收集GT2数据";
            xMainTick = new System.Windows.Forms.Timer();
            xMainTick.Interval = 50;
            xMainTick.Enabled = true;
            xMainTick.Tick += M_MainTick_Tick;

            Task.Run(() =>
            {
                SaveBmp();
            });

            xResetData();
            xCamFocus.FlyImage(true);
            xCamFocus.FlySetPbxHandle(pictureBox1.Handle);
            xFillDisplay();

            GT2Init();
        }

        private void BtnGT2ZStable_Click(object sender, EventArgs e)
        {
            if (!xAutoRecordDataProcess.IsOn)
                xAutoRecordDataProcess.Start();
            else
            {
                xAutoRecordDataProcess.Stop();
                xAutoRecordImageProcess.Stop();
            }
                
        }

        private void BtnGT2Read_Click(object sender, EventArgs e)
        {
            ShowText();
        }

        private void BtnGT2OpenClose_Click(object sender, EventArgs e)
        {
            if (_GT2Opening)
                GT2Close();
            else
                GT2Open();
        }

        private void BtnFlyAutoFind_Click(object sender, EventArgs e)
        {

            string _filename = OpenFilePicker("", "");
            if (string.IsNullOrEmpty(_filename))
                return;


            Bitmap bmp = new Bitmap(_filename);
            Bitmap bmpinput=new Bitmap(bmp);
            bmp.Dispose();


            if (!INI.Instance.xIsStableWhole)
            {
                int w = 60;
                int h = 60;
                Rectangle r = new Rectangle(1280 / 2 - w / 2, 1024 / 2 - h / 2, w, h);
                r.Inflate(INI.Instance.xStableInflate, INI.Instance.xStableInflate);
                //if (list.Count > 0)
                {
                    Graphics g = Graphics.FromImage(bmpinput);
                    g.DrawRectangle(new Pen(Color.Red, 3), r);
                    g.Dispose();

                    StreamWriter streamWriter = new StreamWriter("xStable.txt");
                    streamWriter.WriteLine($"{r.X},{r.Y},{r.Width},{r.Height},{Environment.NewLine}");
                    streamWriter.Close();
                    streamWriter.Dispose();

                }

                pictureBox1.Image = bmpinput;

                return;
            }


            JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
            jzFindObjectClass.AH_SetThreshold(ref bmpinput, INI.Instance.xStableThresholdValue);
            jzFindObjectClass.AH_FindBlob(bmpinput, true);
            if (jzFindObjectClass.FoundList.Count > 0)
            {
                List<Rectangle> list = new List<Rectangle>();
                foreach (FoundClass found in jzFindObjectClass.FoundList)
                {
                    if (found.Area >= 2000 && found.Area <= 8000)
                    {
                        Rectangle recttmp = new Rectangle(found.rect.X, found.rect.Y, found.rect.Width, found.rect.Height);
                        recttmp.Inflate(INI.Instance.xStableInflate, INI.Instance.xStableInflate);

                        list.Add(recttmp);
                    }
                }

                if(list.Count > 0)
                {
                    Graphics g = Graphics.FromImage(bmpinput);
                    g.DrawRectangles(new Pen(Color.Red, 3), list.ToArray());
                    g.Dispose();


                    string str = string.Empty;
                    foreach (Rectangle r in list)
                    {
                        str += $"{r.X},{r.Y},{r.Width},{r.Height},{Environment.NewLine}";
                    }
                    StreamWriter streamWriter = new StreamWriter("xStable.txt");
                    streamWriter.WriteLine(str);
                    streamWriter.Close();
                    streamWriter.Dispose();

                }

                pictureBox1.Image = bmpinput;
            }
            //bmpinput.Dispose();

        }

        private void BtnFlyOpen_Click(object sender, EventArgs e)
        {
            xCamFocus.FlyImage(!xCamFocus.FlyOpen);
        }

        private void BtnAutoTest_Click(object sender, EventArgs e)
        {
            if (!xAutofocusProcess.IsOn)
                xAutofocusProcess.Start();
            else
                xAutofocusProcess.Stop();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            xResetData();
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            IsSaveImage = true;
        }

        private void BtnSaveSet_Click(object sender, EventArgs e)
        {
            INI.Instance.Save();
            //MACHINECollection.SetIniPara();
            xCamFocus.SetFramesPerTrigger(INI.Instance.xFramesCount);
            xCamFocus.SetExposure(INI.Instance.xFoucsExpo);
            ZAXIS.GOSPEED = INI.Instance.xFoucsSpeed;
            ZAXIS.SetSpeed(JetEazy.ControlSpace.MotionSpace.SpeedTypeEnum.GO);
            MACHINE.PLCIO.SetPLCPosReslution(INI.Instance.xFoucsOffset);
        }

        private void BtnP2_Click(object sender, EventArgs e)
        {
            xResetData();
            ZAXIS.SetPos(INI.Instance.xFoucsPOS2);
            MACHINE.PLCIO.ADR_TOFOCUS = true;
        }

        private void BtnP1_Click(object sender, EventArgs e)
        {
            ZAXIS.Go(INI.Instance.xFoucsPOS1);
        }

        private void M_MainTick_Tick(object sender, EventArgs e)
        {
            if (xCamFocus != null)
            {
                lblCount.Text = $"ID:{xAutofocusProcess.ID} 测试进度{xTestCurrentIndex}/{INI.Instance.xStableTestCount} 收集到数目:{xCamFocus.myIndex} plc数目:{MACHINE.PLCIO.GetFlyPLCCount} 曝光:{xCamFocus.GetCurrentExpo().ToString("0.000")}";
                btnFlyOpen.BackColor = (xCamFocus.FlyOpen ? Color.Red : Color.FromArgb(192, 255, 192));

                if (!xCamFocus.FlyOpen)
                {
                    xCamFocus.Snap();
                    using (Bitmap bmp = xCamFocus.GetSnap())
                    {
                        bmpLiving.Dispose();
                        bmpLiving = new Bitmap(bmp);

                        Graphics g = Graphics.FromImage(bmpLiving);
                        g.DrawLine(new Pen(Color.Lime, 2), new PointF(bmpLiving.Width / 2, 0),
                            new PointF(bmpLiving.Width / 2, bmpLiving.Height));
                        g.DrawLine(new Pen(Color.Lime, 2), new PointF(0, bmpLiving.Height / 2),
                           new PointF(bmpLiving.Width, bmpLiving.Height / 2));

                        pictureBox1.Image = bmpLiving;
                    }
                }

            }
            _AutoStableTestTick();
            btnAutoTest.BackColor = (xAutofocusProcess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));
            lblCount.BackColor = (xAutofocusProcess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));

            btnGT2OpenClose.BackColor = (_GT2Opening ? Color.Red : Color.FromArgb(192, 255, 192));

            label7.Text = ZAXIS.PositionNowString;
            label5.Text = ZAXIS.RulerPositionNowString;

            btnGT2ZStable.BackColor = (xAutoRecordDataProcess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));
            _AutoRecordDataTick();
            _AutoRecordImageTick();
        }

        private bool IsSaveImage = false;
        private void SaveBmp()
        {
            while (true)
            {
                if (IsSaveImage && xCamFocus.myIndex > 0)
                {
                    IsSaveImage = false;
                    xCamFocus.FlyAnalyzeImage();
                    int index = 0;
                    string dir = $"D:\\IMAGE\\{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    foreach (Bitmap item in xCamFocus.myImage())
                    {
                        if (index >= xCamFocus.myImage().Count)
                            break;
                        SaveImage(dir, new Bitmap(item), $"{index}");
                        index++;
                    }
                    xResetData();
                }
                Thread.Sleep(20);
            }
        }
        private void SaveImage(string dir, Bitmap bmp, string bmpName)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            bmp.Save($"{dir}\\{bmpName}.png");
        }
        private void xResetData()
        {
            xCamFocus.FlyResetData();
        }
        private void xFillDisplay()
        {
            xPG.SelectedObject = FocusPARA.Instance;
        }


        #region 自动测试流程

        List<CalcModeClass> focusList = new List<CalcModeClass>();
        public ProcessClass xAutofocusProcess = new ProcessClass();

        string m_filenamepath = "";
        List<Rectangle> m_rects = new List<Rectangle>();

        void _AutoStableTestTick()
        {
            ProcessClass Process = xAutofocusProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        xTestCurrentIndex = 1;
                        xRootPath = $"D:\\IMAGE\\{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                        m_rects.Clear();
                        if (File.Exists("xStable.txt"))
                        {
                            StreamReader streamReader = new StreamReader("xStable.txt");
                            while (!streamReader.EndOfStream)
                            {
                                string line = streamReader.ReadLine();
                                if (string.IsNullOrEmpty(line))
                                    continue;
                                string[] strings = line.Split(',');

                                Rectangle rect = new Rectangle(int.Parse(strings[0]),
                                                               int.Parse(strings[1]),
                                                               int.Parse(strings[2]),
                                                               int.Parse(strings[3]));

                                m_rects.Add(rect);

                            }
                            streamReader.Close();
                            streamReader.Dispose();
                        }

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            ZAXIS.GOSPEED = INI.Instance.xReturnSpeed;
                            ZAXIS.SetSpeed(JetEazy.ControlSpace.MotionSpace.SpeedTypeEnum.GO);

                            ZAXIS.Go(INI.Instance.xFoucsPOS1);

                            Process.NextDuriation = 300;
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            float _posCur = ZAXIS.PositionNow;
                            if (IsInRange(_posCur, INI.Instance.xFoucsPOS1, 0.0055f) && ZAXIS.IsOK)
                            {
                                Process.NextDuriation = 300;
                                Process.ID = 16;

                                ZAXIS.GOSPEED = INI.Instance.xFoucsSpeed;
                                ZAXIS.SetSpeed(JetEazy.ControlSpace.MotionSpace.SpeedTypeEnum.GO);

                                xResetData();
                                ZAXIS.SetPos(INI.Instance.xFoucsPOS2);
                                MACHINE.PLCIO.ADR_TOFOCUS = true;
                            }

                        }
                        break;
                    case 16:
                        if (Process.IsTimeup)
                        {
                            float _posCur = ZAXIS.PositionNow;
                            if (IsInRange(_posCur, INI.Instance.xFoucsPOS2, 0.0055f) && ZAXIS.IsOK)
                            {
                                Process.NextDuriation = 2000;
                                Process.ID = 20;

                                if (_GT2Opening)
                                {
                                    if (!Directory.Exists(xRootPath))
                                        Directory.CreateDirectory(xRootPath);
                                    ShowText();
                                    AppendData($"{DateTime.Now.ToString("yyyyMMddHHmmss")},{SaveTextStr()},{ZAXIS.PositionNowString},{ZAXIS.RulerPositionNowString}",
                                                          $"{xRootPath}\\CollectGT2DataDown.csv");
                                }
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            float _posCur = ZAXIS.PositionNow;
                            if (IsInRange(_posCur, INI.Instance.xFoucsPOS2, 0.0055f) && ZAXIS.IsOK)
                            {

                                ZAXIS.GOSPEED = INI.Instance.xReturnSpeed;
                                ZAXIS.SetSpeed(JetEazy.ControlSpace.MotionSpace.SpeedTypeEnum.GO);

                                ZAXIS.Go(INI.Instance.xFoucsPOS1);

                                Process.NextDuriation = 2000;
                                Process.ID = 25;
                            }
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {

                            Process.Stop();

                            #region 读取内存的图片 并计算存入集合
                            focusList.Clear();
                            List<byte[]> bytes = new List<byte[]>();
                            if (xCamFocus != null)
                            {
                                xCamFocus.FlyAnalyzeImage(ref bytes);

                                if (bytes.Count > 0)
                                {
                                    int index = 0;
                                    foreach (byte[] item in bytes)
                                    {

                                        CalcModeClass calcMode = new CalcModeClass();
                                        calcMode.xRects.Clear();
                                        calcMode.Index = index;
                                        calcMode.xbmpInput = new byte[item.Length];
                                        item.CopyTo(calcMode.xbmpInput, 0);
                                        calcMode.xPosStart = INI.Instance.xFoucsPOS1;
                                        calcMode.xSetReslution = INI.Instance.xFoucsOffset;
                                        calcMode.xRects = m_rects;
                                        calcMode.Run();
                                        calcMode.Run2();

                                        focusList.Add(calcMode);

                                        index++;
                                    }
                                }

                            }
                            #endregion

                            #region 数据储存
                            string bmpDir = $"{xRootPath}\\{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                            string bmpDirDev = $"{bmpDir}\\DevReport.csv";
                            string csvDir = xRootPath;
                            if (!Directory.Exists(bmpDir))
                                Directory.CreateDirectory(bmpDir);
                            //if (INI.Instance.xStableSaveImage)
                            {
                                //if (!Directory.Exists(bmpDirDev))
                                //    Directory.CreateDirectory(bmpDirDev);


                                AppendData($"Time,Position,No,LineOffset,Rect1,StdDev1,Rect2,StdDev2,Rect3,StdDev3,Rect4,StdDev4,Rect5,StdDev5,Rect6,StdDev6,", $"{bmpDirDev}");

                                int indexD = 0;
                                foreach (CalcModeClass calc in focusList)
                                {
                                    if (INI.Instance.xStableSaveImage)
                                        SaveImage(bmpDir, new Bitmap(calc.xbmpResult), $"{indexD}");

                                    AppendData($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},{calc.ToReportStr2()}",
                                            $"{bmpDirDev}");

                                    indexD++;
                                }

                            }

                            focusList.Sort((item1, item2) =>
                            { return item1.xStdDev >= item2.xStdDev ? 1 : -1; });

                            if(!INI.Instance.xIsCCDSIM)
                            {
                                CalcModeClass calc1 = focusList[0];
                                AppendData($"{DateTime.Now.ToString("yyyyMMddHHmmss")},{calc1.ToReportStr()}",
                                                      $"{csvDir}\\CollectNeedleData.csv");

                            }

                            if (_GT2Opening)
                            {
                                ShowText();
                                AppendData($"{DateTime.Now.ToString("yyyyMMddHHmmss")},{SaveTextStr()},{ZAXIS.PositionNowString},{ZAXIS.RulerPositionNowString}",
                                                      $"{xRootPath}\\CollectGT2DataUp.csv");
                            }
                                
                            #endregion

                            if (xTestCurrentIndex < INI.Instance.xStableTestCount)
                            {
                                if(INI.Instance.xIsCCDSIM)
                                {
                                    Process.NextDuriation = 300;
                                    Process.ID = 10;
                                    Process.IsOn = true;

                                    xTestCurrentIndex++;
                                }
                                else
                                {
                                    if (xCamFocus.myIndex == MACHINE.PLCIO.GetFlyPLCCount)
                                    {
                                        Process.NextDuriation = 300;
                                        Process.ID = 10;
                                        Process.IsOn = true;

                                        xTestCurrentIndex++;
                                    }
                                    else
                                    {
                                        MessageBox.Show("收集数据与plc触发数据不一致，停止流程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                        break;

                }
            }
        }

        bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        void AppendData(string appendstr, string FileName)
        {
            StreamWriter Sw = File.AppendText(FileName);
            Sw.WriteLine(appendstr);
            Sw.Close();
        }

        string OpenFilePicker(string DefaultPath, string DefaultName)
        {
            string retStr = "";

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "PNG Files (*.png)|*.PNG|" + "All files (*.*)|*.*";
            //dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            dlg.Dispose();
            return retStr;
        }

        #endregion

        #region 记录数据
        ProcessClass xAutoRecordDataProcess = new ProcessClass();
        void _AutoRecordDataTick()
        {
            ProcessClass Process = xAutoRecordDataProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        xRootPath = $"D:\\IMAGE\\{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                        xAutoRecordImageProcess.Start();
                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (_GT2Opening)
                            {
                                if (!Directory.Exists(xRootPath))
                                    Directory.CreateDirectory(xRootPath);
                                ShowText();
                                AppendData($"{DateTime.Now.ToString("yyyy/MM/dd HH-mm-ss")},{SaveTextStr()}", $"{xRootPath}\\CollectGT2RecordData.csv");
                            }

                            Process.NextDuriation = 500;
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            Process.NextDuriation = INI.Instance.xRecordDataOffsetTime * 60 * 1000;
                            Process.ID = 10;
                        }
                        break;
                }
            }
        }
        ProcessClass xAutoRecordImageProcess = new ProcessClass();
        void _AutoRecordImageTick()
        {
            ProcessClass Process = xAutoRecordImageProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        //xRootPath = $"D:\\IMAGE\\{DateTime.Now.ToString("yyyyMMddHHmmss")}";

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (_GT2Opening)
                            {
                                if (!Directory.Exists(xRootPath))
                                    Directory.CreateDirectory(xRootPath);
                                //ShowText();
                                //AppendData($"{DateTime.Now.ToString("yyyy/MM/dd HH-mm-ss")},{SaveTextStr()}", $"{xRootPath}\\CollectGT2RecordData.csv");
                                xCamFocus.Snap();
                                using (Bitmap bmpSaveImage = xCamFocus.GetSnap())
                                {
                                    string _saveImagePath = $"{xRootPath}\\{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.png";
                                    bmpSaveImage.Save(_saveImagePath, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }

                            Process.NextDuriation = 500;
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            Process.NextDuriation = INI.Instance.xRecordImageOffsetTime * 60 * 1000;
                            Process.ID = 10;
                        }
                        break;
                }
            }
        }
        #endregion


        #region 读取基恩士数据 测试Z稳定性

        bool _GT2Opening = false;
        GT2Class _GT2_1;
        //GT2Class _GT2_2;
        public void GT2Init()
        {
            _GT2_1 = new GT2Class("COM14");
            //_GT2_2 = new GT2Class("COM13");
        }
        public void GT2Dispose()
        {
            if(_GT2_1 != null)
            {
                _GT2_1.Dispose();
                _GT2_1 = null;
            }
            //if (_GT2_2 != null)
            //{
            //    _GT2_2.Dispose();
            //    _GT2_2 = null;
            //}
        }
        public int GT2Open()
        {
            int iret = _GT2_1.GT2Open();
            //iret+= _GT2_2.GT2Open();
            if (iret == 0)
                _GT2Opening = true;
            return iret;
        }
        public int GT2Close()
        {
            int iret = _GT2_1.GT2Close();
            //iret += _GT2_2.GT2Close();
            if (iret == 0)
                _GT2Opening = false;
            return iret;
        }
        public void ShowText()
        {
            lblGT2Value.Text = $"读值:{_GT2_1.GT2GetCurrentValue(1).ToString("0.0000")}";
            lblGT2Value2.Text = $"读值:{_GT2_1.GT2GetCurrentValue(2).ToString("0.0000")}";
            lblGT2Value3.Text = $"读值:{_GT2_1.GT2GetCurrentValue(3).ToString("0.0000")}";
        }
        public string SaveTextStr()
        {
            return $"{_GT2_1.GT2GetCurrentValue(1)},{_GT2_1.GT2GetCurrentValue(2)},{_GT2_1.GT2GetCurrentValue(3)}";
        }

        #endregion

        #region 自动对焦测试流程(起点-终点)

#if NO_USE_
        private void _autoFocusRun()
        {
            //CamHeight.Snap();
            //Bitmap bmp = new Bitmap(CamHeight.GetSnap());

            ////Bitmap bitmap = new Bitmap(bmp);
            //Mat source = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            //double _score = GetImgQualityScore(source, cboMode.SelectedIndex);
            //DrawText(bmp, $"(Method:{cboMode.Text.Trim()}) Score:{_score.ToString("0.000")}");
            //pictureBox1.Image = bmp;
        }


        public double GetImgQualityScore(Mat src, int flag)
        {
            Mat imgLaplance = new Mat();
            Mat imageSobel = new Mat();
            Mat meanValueImage = new Mat();
            Mat meanImage = new Mat();
            Mat BlurMat = new Mat();
            Mat gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            //中值模糊 降低图像噪音
            Cv2.MedianBlur(gray, BlurMat, 5);
            //Cv2.BoxFilter(gray, BlurMat, MatType.CV_8UC3, new OpenCvSharp.Size(11, 11));
            string method = string.Empty;
            double meanValue = 0;
            if (flag == 1)
            {
                //方差法
                Cv2.MeanStdDev(BlurMat, meanValueImage, meanImage);
                meanValue = meanImage.At<double>(0, 0);
                method = "MeanStdDev";
            }
            else if (flag == 0)
            {
                //拉普拉斯
                Cv2.Laplacian(BlurMat, imgLaplance, MatType.CV_16S, 3, 1);
                Cv2.ConvertScaleAbs(imgLaplance, imgLaplance);
                //结果放大两倍,拉开差距
                meanValue = Cv2.Mean(imgLaplance)[0] * 2;
                method = "Laplacian";
            }
            else
            {
                //索贝尔
                Cv2.Sobel(BlurMat, imageSobel, MatType.CV_16S, 3, 3, 5);
                Cv2.ConvertScaleAbs(imageSobel, imageSobel);
                //结果放大两倍,拉开差距
                meanValue = Cv2.Mean(imageSobel)[0] * 2;
                method = "Sobel";

            }

            //释放资源
            imgLaplance.Dispose();
            imageSobel.Dispose();
            meanValueImage.Dispose();
            meanImage.Dispose();
            BlurMat.Dispose();
            gray.Dispose();
            //Cv2.Sobel(gray, imgSobel, MatType.CV_16U, 2, 2);
            //meanValue2 = Cv2.Mean(imgSobel)[0];
            //Cv2.PutText(src, "(" + method + " Method): " + meanValue, new OpenCvSharp.Point(20, 20), HersheyFonts.HersheyPlain, 1, new Scalar(0, 0, 255), 1);
            //Cv2.ImShow(method, src);
            return meanValue;
        }
        public void DrawText(Bitmap BMP, string Text)
        {
            SolidBrush B = new SolidBrush(Color.Lime);
            Font MyFont = new Font("Arial", 88);

            Graphics g = Graphics.FromImage(BMP);
            g.DrawString(Text, MyFont, B, new PointF(5, 5));
            g.Dispose();
        }
        float d1 = 0;
        float d2 = 0;
        float offset = 0.01f;

        class focusItemClass
        {
            public float fCurrentPos = 0f;
            public double dScore = 0d;
        }

        List<focusItemClass> focusList = new List<focusItemClass>();
        public ProcessClass m_autofocusProcess = new ProcessClass();

        string m_filenamepath = "";

        void _AutoFocusTick()
        {
            ProcessClass Process = m_autofocusProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        focusList.Clear();

                        d1 = 112.15f;
                        d2 = 112.25f;
                        offset = 0.01f;

                        float.TryParse(txt1.Text.Trim(), out d1);
                        float.TryParse(txt2.Text.Trim(), out d2);
                        float.TryParse(txtoffset.Text.Trim(), out offset);

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            GetAxis(2).Go(d1, 0);
                            Process.NextDuriation = 300;
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            float _posCur = (float)GetAxis(2).GetPos();
                            if (IsInRange(_posCur, d1, 0.0085f) && GetAxis(2).IsOK)
                            {
                                Process.NextDuriation = 100;
                                Process.ID = 20;

                                GetAxis(2).Go(d2, 0);
                                cam1.StopCapture();
                            }

                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            float _posCur = (float)GetAxis(2).GetPos();
                            if (IsInRange(_posCur, d2, 0.0085f) && GetAxis(2).IsOK)
                            {
                                Process.Stop();
                                cam1.StartCapture();
                            }

                        }
                        break;
                        //case 25:
                        //    if (Process.IsTimeup)
                        //    {
                        //        float _posCur = (float)GetAxis(2).GetPos();
                        //        if (IsInRange(_posCur, d2, 0.0085f) && GetAxis(2).IsOK)
                        //        {
                        //            Process.NextDuriation = 100;
                        //            Process.ID = 15;

                        //            GetAxis(2).Go(d1, 0);
                        //        }

                        //    }
                        //    break;
                }
            }
        }
        void _AutoFocusTickBAK()
        {
            ProcessClass Process = m_autofocusProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        focusList.Clear();

                        d1 = 112.15f;
                        d2 = 112.25f;
                        offset = 0.01f;

                        float.TryParse(txt1.Text.Trim(), out d1);
                        float.TryParse(txt2.Text.Trim(), out d2);
                        float.TryParse(txtoffset.Text.Trim(), out offset);

                        CommonLogClass.Instance.LogMessage("开始位置=" + d1.ToString());
                        CommonLogClass.Instance.LogMessage("结束位置=" + d2.ToString());
                        CommonLogClass.Instance.LogMessage("间隔=" + offset.ToString());

                        m_filenamepath = Traveller106.Universal.DEBUGRESULTPATH + "\\" + JzTimes.DateTimeSerialStringFFF;
                        if (!System.IO.Directory.Exists(m_filenamepath))
                            System.IO.Directory.CreateDirectory(m_filenamepath);
                        //d1 = float.Parse(txt1.Text.Trim());
                        //d2 = float.Parse(txt2.Text.Trim());
                        //offset = float.Parse(txtoffset.Text.Trim());


                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (d1 < d2)
                            {
                                GetAxis(2).Go(d1, 0);
                                //d1 -= offset;
                                //CommonLogClass.Instance.LogMessage("当前位置=" + d1.ToString());
                                Process.NextDuriation = 300;
                                Process.ID = 15;
                            }
                            else
                            {
                                Process.NextDuriation = 300;
                                Process.ID = 20;
                            }

                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            float _posCur = (float)GetAxis(2).GetPos();
                            if (IsInRange(_posCur, d1, 0.0085f) && GetAxis(2).IsOK)
                            {
                                Process.NextDuriation = 100;
                                Process.ID = 10;

                                CamHeight.Snap();
                                Bitmap bmp = new Bitmap(CamHeight.GetSnap());

                                Mat source = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
                                double _score = GetImgQualityScore(source, cboMode.SelectedIndex);
                                DrawText(bmp, $"(Method:{cboMode.Text.Trim()}) Score:{_score.ToString("0.000")}");
                                pictureBox1.Image = bmp;

                                bmp.Save(m_filenamepath + "\\pos_" + d1.ToString("0.0000") + ".png",
                                    System.Drawing.Imaging.ImageFormat.Png);

                                CommonLogClass.Instance.LogMessage("位置=" + d1.ToString());
                                CommonLogClass.Instance.LogMessage($"(Method:{cboMode.Text.Trim()}) Score:{_score.ToString("0.000")}");
                                focusItemClass focus = new focusItemClass();
                                focus.fCurrentPos = d1;
                                focus.dScore = _score;
                                focusList.Add(focus);

                                //bmp.Dispose();
                                source.Dispose();

                                d1 += offset;
                            }

                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (GetAxis(2).IsOK)
                            {
                                //Process.Stop();
                                focusList.Sort((item1, item2) => { return item1.dScore > item2.dScore ? -1 : 1; });
                                if (focusList.Count > 0)
                                {
                                    GetAxis(2).Go(focusList[0].fCurrentPos, 0);
                                    CommonLogClass.Instance.LogMessage($"(Method:{cboMode.Text.Trim()}) " +
                                        $"(Current Pos:{focusList[0].fCurrentPos.ToString("0.000")}) " +
                                        $"Max Score:{focusList[0].dScore.ToString("0.000")}");

                                    Process.NextDuriation = 300;
                                    Process.ID = 25;
                                }
                                else
                                {
                                    Process.Stop();
                                }
                            }
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {
                            float _posCur = (float)GetAxis(2).GetPos();
                            if (IsInRange(_posCur, focusList[0].fCurrentPos, 0.0085f) && GetAxis(2).IsOK)
                            {
                                Process.Stop();

                                m_Running = false;

                                CamHeight.Snap();
                                Bitmap bmp = new Bitmap(CamHeight.GetSnap());
                                //DrawText(bmp, $"(Method:{cboMode.Text.Trim()}) Score:{_score.ToString("0.000")}");
                                pictureBox1.Image = bmp;
                            }
                        }
                        break;
                }
            }
        }

        IAxis GetAxis(int axisID)
        {
            return ((NeedleMachineClass)Traveller106.Universal.MACHINECollection.MACHINE).PLCMOTIONCollection[axisID];
        }
        bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
#endif

        #endregion


    }
}
