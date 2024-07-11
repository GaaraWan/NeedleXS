//using JetEazy;
using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
//using Eazy_Project_III;

namespace Traveller106
{

    public class GetPositionPropertyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
        {
            if (pContext != null && pContext.Instance != null)
            {
                //以「...」按鈕的方式顯示
                //UITypeEditorEditStyle.DropDown    下拉選單
                //UITypeEditorEditStyle.None        預設的輸入欄位
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(pContext);
        }
        public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
        {
            IWindowsFormsEditorService editorService = null;
            if (pContext != null && pContext.Instance != null && pProvider != null)
            {
                editorService = (IWindowsFormsEditorService)pProvider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    //將顯示得視窗放在這邊，並透過ShowDialog方式來呼叫
                    //取得到的值再回傳回去
                    //MessageBox.Show("sfsf");
                    //frmDataGridViewPosition msrdataform = new frmDataGridViewPosition(pContext.PropertyDescriptor.DisplayName,
                    //    pContext.PropertyDescriptor.Name,(string)pValue);
                    ////msrdataform.Show();
                    //if (msrdataform.ShowDialog() == DialogResult.OK)
                    //{
                    //    pValue = JzToolsClass.PassingString;
                    //}

                    //pValue = "FUCK YOU!";
                }
            }
            return pValue;
        }
    }

    public class GetFilePathPropertyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
        {
            if (pContext != null && pContext.Instance != null)
            {
                //以「...」按鈕的方式顯示
                //UITypeEditorEditStyle.DropDown    下拉選單
                //UITypeEditorEditStyle.None        預設的輸入欄位
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(pContext);
        }
        public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
        {
            IWindowsFormsEditorService editorService = null;
            if (pContext != null && pContext.Instance != null && pProvider != null)
            {
                editorService = (IWindowsFormsEditorService)pProvider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {

                    OpenFileDialog fileDlg = new OpenFileDialog();

                    switch(pContext.PropertyDescriptor.Name)
                    {
                        case "CfgPath":
                            fileDlg.Filter = "控制器配置文件|*.dcfg";
                            fileDlg.Title = "请选择匹配的控制器配置文件";
                            break;
                        case "HWCPath":
                            fileDlg.Filter = "控制器位移校准文件|*.hwc";
                            fileDlg.Title = "请选择匹配的控制器位移校准文件";
                            break;
                        default:
                            break;
                    }
                   
                    if (fileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        pValue = fileDlg.FileName;
                    }
                }
            }
            return pValue;
        }
    }

    public class FocusPARA
    {
        private static FocusPARA _instance = null;
        public static FocusPARA Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FocusPARA();
                return _instance;
            }
        }

        const string X1Cat8 = "A08.测试参数";
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A1.测试起点")]
        //[Browsable(false)]
        public float xFoucsPOS1
        {
            get { return INI.Instance.xFoucsPOS1; }
            set { INI.Instance.xFoucsPOS1 = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A2.测试终点")]
        //[Browsable(false)]
        public float xFoucsPOS2
        {
            get { return INI.Instance.xFoucsPOS2; }
            set { INI.Instance.xFoucsPOS2 = value; }
        }
        [CategoryAttribute(X1Cat8), DescriptionAttribute("5=0.002mm 50=0.02mm 500=0.2mm")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A3.测试解析度")]
        //[Browsable(false)]
        public int xFoucsOffset
        {
            get { return INI.Instance.xFoucsOffset; }
            set { INI.Instance.xFoucsOffset = value; }
        }
        [CategoryAttribute(X1Cat8), DescriptionAttribute("单位us")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A4.测试曝光")]
        //[Browsable(false)]
        public float xFoucsExpo
        {
            get { return INI.Instance.xFoucsExpo; }
            set { INI.Instance.xFoucsExpo = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A5-1.对焦测试速度")]
        //[Browsable(false)]
        public double xFoucsSpeed
        {
            get { return INI.Instance.xFoucsSpeed; }
            set { INI.Instance.xFoucsSpeed = value; }
        }
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A5-2.回初始位置速度")]
        //[Browsable(false)]
        public double xReturnSpeed
        {
            get { return INI.Instance.xReturnSpeed; }
            set { INI.Instance.xReturnSpeed = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A6.触发帧数")]
        //[Browsable(false)]
        public int xFramesCount
        {
            get { return INI.Instance.xFramesCount; }
            set { INI.Instance.xFramesCount = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A7.稳定性测试次数")]
        //[Browsable(false)]
        public int xStableTestCount
        {
            get { return INI.Instance.xStableTestCount; }
            set { INI.Instance.xStableTestCount = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A8.稳定性测试保存图片")]
        //[Browsable(false)]
        public bool xStableSaveImage
        {
            get { return INI.Instance.xStableSaveImage; }
            set { INI.Instance.xStableSaveImage = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A9.抓取全域的点")]
        //[Browsable(false)]
        public bool xIsStableWhole
        {
            get { return INI.Instance.xIsStableWhole; }
            set { INI.Instance.xIsStableWhole = value; }
        }
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A9.启用模拟相机")]
        //[Browsable(false)]
        public bool xIsCCDSIM
        {
            get { return INI.Instance.xIsCCDSIM; }
            set { INI.Instance.xIsCCDSIM = value; }
        }
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A9-1.收集数据间隔 单位(分钟)")]
        //[Browsable(false)]
        public int xRecordDataOffsetTime
        {
            get { return INI.Instance.xRecordDataOffsetTime; }
            set { INI.Instance.xRecordDataOffsetTime = value; }
        }
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("A9-2.收集图片间隔 单位(分钟)")]
        //[Browsable(false)]
        public int xRecordImageOffsetTime
        {
            get { return INI.Instance.xRecordImageOffsetTime; }
            set { INI.Instance.xRecordImageOffsetTime = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("B00.外扩大小")]
        //[Browsable(false)]
        public int xStableInflate
        {
            get { return INI.Instance.xStableInflate; }
            set { INI.Instance.xStableInflate = value; }
        }

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("B01.灰阶阈值")]
        //[Browsable(false)]
        public int xStableThresholdValue
        {
            get { return INI.Instance.xStableThresholdValue; }
            set { INI.Instance.xStableThresholdValue = value; }
        }
    }

    public class INI
    {
        private static INI _instance = null;
        public static INI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new INI();
                return _instance;
            }
        }

        #region INI Access Functions
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        void WriteINIValue(string section, string key, string value, string filepath)
        {
            WritePrivateProfileString(section, key, value, filepath);
        }
        string ReadINIValue(string section, string key, string defaultvaluestring, string filepath)
        {
            string retStr = "";

            StringBuilder temp = new StringBuilder(200);
            int Length = GetPrivateProfileString(section, key, "", temp, 200, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;
            //else
            //    retStr = retStr.Split('/')[0]; //把說明排除掉

            return retStr;

        }
        #endregion

        string MAINPATH = "";
        string INIFILE = "";
      
        public bool IsOpenMotorWindows = false;
        public bool IsOpenIOWindows = false;
        public bool IsOpenFocusWindows = false;

        string xFormat = "0.000000";

        //static JzToolsClass JzTools = new JzToolsClass();

        public int LANGUAGE = 0;

        #region 调针机

        const string X1Cat1 = "A01.基础参数";

        [CategoryAttribute(X1Cat1), DescriptionAttribute("即特微相机与定位相机的XYZ偏移")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("01.特微相机与定位相机偏移距离")]
        [Browsable(true),ReadOnly(true)]
        public string offsetCAM0CAM1 { get; set; } = "0,0,0";
        [CategoryAttribute(X1Cat1), DescriptionAttribute("即特微相机与显微相机的XYZ偏移")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("02.特微相机与显微相机偏移距离")]
        [Browsable(true),ReadOnly(true)]
        public string offsetCAM2CAM1 { get; set; } = "0,0,0";
        [CategoryAttribute(X1Cat1), DescriptionAttribute("即特微相机与显微相机的XYZ偏移")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("02.目镜与显微相机偏移距离")]
        [Browsable(true), ReadOnly(true)]
        public string offsetCAM2Eyes { get; set; } = "0,0,0";

        [CategoryAttribute(X1Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("03.开启存储图片功能")]
        [Browsable(true), ReadOnly(false)]
        public bool IsSaveImageOpen { get; set; } = false;
        [CategoryAttribute(X1Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("04.存储对焦完成图片")]
        [Browsable(true), ReadOnly(false)]
        public bool IsSaveFocusImage { get; set; } = false;

        [CategoryAttribute(X1Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("全域相机中心")]
        [Browsable(false)]
        public string Cam0CenterPos { get; set; } = "0,0,0";
        [CategoryAttribute(X1Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("特征相机中心0")]
        [Browsable(false)]
        public string Cam1CenterPos0 { get; set; } = "0,0,0";
        [CategoryAttribute(X1Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("特征相机中心1")]
        [Browsable(false)]
        public string Cam1CenterPos { get; set; } = "0,0,0";
        [CategoryAttribute(X1Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("显微相机中心")]
        [Browsable(false)]
        public string Cam2CenterPos { get; set; } = "0,0,0";
        [CategoryAttribute(X1Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("目镜中心")]
        [Browsable(false)]
        public string EyesCenterPos { get; set; } = "0,0,0";


        const string X1Cat8 = "A08.测试参数";
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public float xFoucsPOS1 { get; set; } = 0;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public float xFoucsPOS2 { get; set; } = 0;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public int xFoucsOffset { get; set; } = 5;//5=0.002mm  50=0.02mm 100=0.04mm

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public float xFoucsExpo { get; set; } = 3000;//3000us=3ms

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public double xFoucsSpeed { get; set; } = 8000;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public int xFramesCount { get; set; } = 1;
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public int xStableTestCount { get; set; } = 1;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public bool xStableSaveImage { get; set; } = false;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public bool xIsStableWhole { get; set; } = false;
        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public int xStableInflate { get; set; } = 10;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public int xStableThresholdValue { get; set; } = 50;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public bool xIsCCDSIM { get; set; } = false;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public int xRecordDataOffsetTime { get; set; } = 10;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public int xRecordImageOffsetTime { get; set; } = 30;

        [CategoryAttribute(X1Cat8), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        //[DisplayName("目镜中心")]
        [Browsable(false)]
        public double xReturnSpeed { get; set; } = 1000;

        #endregion

        public void Initial()
        {
            MAINPATH = Universal.MAINPATH;
            INIFILE = MAINPATH + "\\CONFIG.ini";

            Load();
        }

        public void Load()
        {
            offsetCAM0CAM1 = ReadINIValue("Basic", "offsetCAM0CAM1", offsetCAM0CAM1.ToString(), INIFILE);
            offsetCAM2CAM1 = ReadINIValue("Basic", "offsetCAM2CAM1", offsetCAM2CAM1.ToString(), INIFILE);
            offsetCAM2Eyes = ReadINIValue("Basic", "offsetCAM2Eyes", offsetCAM2Eyes.ToString(), INIFILE);

            Cam0CenterPos = ReadINIValue("Basic", "Cam0CenterPos", Cam0CenterPos.ToString(), INIFILE);
            Cam1CenterPos = ReadINIValue("Basic", "Cam1CenterPos", Cam1CenterPos.ToString(), INIFILE);
            Cam2CenterPos = ReadINIValue("Basic", "Cam2CenterPos", Cam2CenterPos.ToString(), INIFILE);
            Cam1CenterPos0 = ReadINIValue("Basic", "Cam1CenterPos0", Cam1CenterPos0.ToString(), INIFILE);
            EyesCenterPos = ReadINIValue("Basic", "EyesCenterPos", EyesCenterPos.ToString(), INIFILE);

            IsSaveImageOpen = ReadINIValue("Basic", "IsSaveImageOpen", (IsSaveImageOpen ? "1" : "0"), INIFILE) == "1";

            xFoucsPOS1 = float.Parse(ReadINIValue("Basic", "xFoucsPOS1", xFoucsPOS1.ToString(xFormat), INIFILE));
            xFoucsPOS2 = float.Parse(ReadINIValue("Basic", "xFoucsPOS2", xFoucsPOS2.ToString(xFormat), INIFILE));
            xFoucsOffset = int.Parse(ReadINIValue("Basic", "xFoucsOffset", xFoucsOffset.ToString(), INIFILE));
            xFoucsExpo = float.Parse(ReadINIValue("Basic", "xFoucsExpo", xFoucsExpo.ToString(xFormat), INIFILE));
            xFoucsSpeed = float.Parse(ReadINIValue("Basic", "xFoucsSpeed", xFoucsSpeed.ToString(xFormat), INIFILE));
            xFramesCount = int.Parse(ReadINIValue("Basic", "xFramesCount", xFramesCount.ToString(), INIFILE));
            xStableTestCount = int.Parse(ReadINIValue("Basic", "xStableTestCount", xStableTestCount.ToString(), INIFILE));
            xStableSaveImage = ReadINIValue("Basic", "xStableSaveImage", (xStableSaveImage ? "1" : "0"), INIFILE) == "1";
            xIsStableWhole = ReadINIValue("Basic", "xIsStableWhole", (xIsStableWhole ? "1" : "0"), INIFILE) == "1";
            xStableInflate= int.Parse(ReadINIValue("Basic", "xStableInflate", xStableInflate.ToString(), INIFILE));
            xStableThresholdValue = int.Parse(ReadINIValue("Basic", "xStableThresholdValue", xStableThresholdValue.ToString(), INIFILE));
            xRecordDataOffsetTime = int.Parse(ReadINIValue("Basic", "xRecordDataOffsetTime", xRecordDataOffsetTime.ToString(), INIFILE));
            xRecordImageOffsetTime = int.Parse(ReadINIValue("Basic", "xRecordImageOffsetTime", xRecordImageOffsetTime.ToString(), INIFILE));
            xReturnSpeed = int.Parse(ReadINIValue("Basic", "xReturnSpeed", xReturnSpeed.ToString(), INIFILE));

            xIsCCDSIM = ReadINIValue("Basic", "xIsCCDSIM", (xIsCCDSIM ? "1" : "0"), INIFILE) == "1";
            IsSaveFocusImage = ReadINIValue("Basic", "IsSaveFocusImage", (IsSaveFocusImage ? "1" : "0"), INIFILE) == "1";


            //Rectangle rectangle = new Rectangle();rectangle.Inflate
        }
        public void Save()
        {
            WriteINIValue("Basic", "offsetCAM0CAM1", offsetCAM0CAM1.ToString(), INIFILE);
            WriteINIValue("Basic", "offsetCAM2CAM1", offsetCAM2CAM1.ToString(), INIFILE);
            WriteINIValue("Basic", "offsetCAM2Eyes", offsetCAM2Eyes.ToString(), INIFILE);

            WriteINIValue("Basic", "Cam0CenterPos", Cam0CenterPos.ToString(), INIFILE);
            WriteINIValue("Basic", "Cam1CenterPos", Cam1CenterPos.ToString(), INIFILE);
            WriteINIValue("Basic", "Cam2CenterPos", Cam2CenterPos.ToString(), INIFILE);
            WriteINIValue("Basic", "Cam1CenterPos0", Cam1CenterPos0.ToString(), INIFILE);
            WriteINIValue("Basic", "EyesCenterPos", EyesCenterPos.ToString(), INIFILE);

            WriteINIValue("Basic", "IsSaveImageOpen", (IsSaveImageOpen ? "1" : "0"), INIFILE);

            WriteINIValue("Basic", "xFoucsPOS1", xFoucsPOS1.ToString(xFormat), INIFILE);
            WriteINIValue("Basic", "xFoucsPOS2", xFoucsPOS2.ToString(xFormat), INIFILE);
            WriteINIValue("Basic", "xFoucsOffset", xFoucsOffset.ToString(), INIFILE);
            WriteINIValue("Basic", "xFoucsExpo", xFoucsExpo.ToString(xFormat), INIFILE);
            WriteINIValue("Basic", "xFoucsSpeed", xFoucsSpeed.ToString(xFormat), INIFILE);
            WriteINIValue("Basic", "xFramesCount", xFramesCount.ToString(), INIFILE);
            WriteINIValue("Basic", "xStableTestCount", xStableTestCount.ToString(), INIFILE);
            WriteINIValue("Basic", "xStableSaveImage", (xStableSaveImage ? "1" : "0"), INIFILE);
            WriteINIValue("Basic", "xIsStableWhole", (xIsStableWhole ? "1" : "0"), INIFILE);
            WriteINIValue("Basic", "xStableInflate", xStableInflate.ToString(), INIFILE);
            WriteINIValue("Basic", "xStableThresholdValue", xStableThresholdValue.ToString(), INIFILE);
            WriteINIValue("Basic", "xRecordDataOffsetTime", xRecordDataOffsetTime.ToString(), INIFILE);
            WriteINIValue("Basic", "xRecordImageOffsetTime", xRecordImageOffsetTime.ToString(), INIFILE);
            WriteINIValue("Basic", "xReturnSpeed", xReturnSpeed.ToString(), INIFILE);

            WriteINIValue("Basic", "xIsCCDSIM", (xIsCCDSIM ? "1" : "0"), INIFILE);
            WriteINIValue("Basic", "IsSaveFocusImage", (IsSaveFocusImage ? "1" : "0"), INIFILE);


        }


    }
}
