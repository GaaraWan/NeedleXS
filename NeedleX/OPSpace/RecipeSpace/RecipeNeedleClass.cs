using Common.RecipeSpace;
using Eazy_Project_III;
using JetEazy;
using JzDisplay.OPSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RecipeSpace
{
    //public struct NleXYZ
    //{
    //    public double X;
    //    public double Y;
    //    public double Z;
    //}
    //public class NeedleListPosition
    //{
    //    public string Name { get; set; } = string.Empty;
    //    //public List<NeedleXYZ> NeedleXYZs = new List<NeedleXYZ>();
    //}
    public class NeedleXYZ
    {
        const string _format = "0.000000";

        private double _x = 0;
        private double _y = 0;
        private double _z = 0;

        public NeedleXYZ()
        {

        }
        public NeedleXYZ(string Str)
        {
            FromString(Str);
        }
        public NeedleXYZ(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        public string XStr { get { return _x.ToString(_format); } }
        public string YStr { get { return _y.ToString(_format); } }
        public string ZStr { get { return _z.ToString(_format); } }

        public void FromString(string str)
        {
            string[] parts = str.Split(',');
            _x = double.Parse(parts[0]);
            _y = double.Parse(parts[1]);
            _z = double.Parse(parts[2]);
        }
        public override string ToString()
        {
            string str = "";
            str += _x.ToString(_format) + ",";
            str += _y.ToString(_format) + ",";
            str += _z.ToString(_format);
            return str;
        }
    }
    //public class NeedleCamPara
    //{
    //    const string _format = "0.000000";

    //    private double _resolution = 0.00066;
    //    private LayerMode _layer = LayerMode.White;
    //    private int _thresholdValue = 200;
    //    private int _cropSize = 500;

    //    public NeedleCamPara() { }
    //    public NeedleCamPara(string Str)
    //    {
    //        FromString(Str);
    //    }
    //    [Category("01.基本设置"), Description("")]
    //    [DisplayName("解析度"), Browsable(true)]
    //    public double Resolution
    //    {
    //        get { return _resolution; }
    //        set { _resolution = value; }
    //    }
    //    [Category("01.基本设置"), Description("")]
    //    [DisplayName("寻找方式"), Browsable(true)]
    //    [TypeConverter(typeof(JzEnumConverter))]
    //    public LayerMode Layer
    //    {
    //        get { return _layer; }
    //        set { _layer = value; }
    //    }
    //    [Category("01.基本设置"), Description("")]
    //    [DisplayName("灰度阈值"), Browsable(true)]
    //    [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
    //    public int ThresholdValue
    //    {
    //        get { return _thresholdValue; }
    //        set { _thresholdValue = value; }
    //    }
    //    [Category("01.基本设置"), Description("")]
    //    [DisplayName("截取范围"), Browsable(true)]
    //    public int CropSize
    //    {
    //        get { return _cropSize; }
    //        set { _cropSize = value; }
    //    }
    //    public void FromString(string str)
    //    {
    //        string[] parts = str.Split(',');
    //        if (parts.Length < 4)
    //            return;
    //        _resolution = double.Parse(parts[0]);
    //        _layer = (LayerMode)int.Parse(parts[1]);
    //        _thresholdValue = int.Parse(parts[2]);
    //        _cropSize = int.Parse(parts[3]);
    //    }
    //    public override string ToString()
    //    {
    //        string str = "";
    //        str += _resolution.ToString(_format) + ",";
    //        str += ((int)_layer).ToString() + ",";
    //        str += _thresholdValue.ToString() + ",";
    //        str += _cropSize.ToString();
    //        return str;
    //    }
    //}

    public class RecipeNeedleClass : RecipeBaseClass
    {
        const int ListCount = 8;//集合数量

        protected RecipeNeedleClass()
        {

        }
        private static RecipeNeedleClass _instance = null;
        public static RecipeNeedleClass Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RecipeNeedleClass();
                return _instance;
            }
        }

        //粗定位坐标集合

        const string _Cat1 = "A01.基础设置";
        //[CategoryAttribute(_Cat1), DescriptionAttribute("")]
        ////[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        ////[DisplayName("特征定位坐标集合")]
        //[Browsable(true)]
        //public List<NeedleXYZ> CoarsePositioningList = new List<NeedleXYZ>();


        //精定位坐标集合

        //初始高度 即用来对焦计算距离的初始高度
        [CategoryAttribute(_Cat1), DescriptionAttribute("即用来对焦计算距离的初始高度")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("对焦Z初始高度")]
        [Browsable(true)]
        public double BaseHeightZ { get; set; } = 0;
        //相机曝光和增益
        [CategoryAttribute(_Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("相机曝光集合")]
        [Browsable(true)]
        public string sExposureList { get; set; } = "";
        public string[] ExposureArray = new string[ListCount];
        //public List<string> ExposureList = new List<string>();

        [CategoryAttribute(_Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("相机增益集合")]
        [Browsable(true)]
        public string sGainList { get; set; } = "";
        public string[] GainArray = new string[ListCount];

        [CategoryAttribute(_Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("灯光亮度集合")]
        [Browsable(true)]
        public string sLedValueArray { get; set; } = "";
        public string[] LedValueArray = new string[ListCount];

        [CategoryAttribute(_Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("相机参数集合")]
        [Browsable(true)]
        public string scamparaArray { get; set; } = "";
        public string[] camparaArray = new string[ListCount];
        public ControlClass[] camparaClassArray = new ControlClass[ListCount];

        //public List<string> GainList = new List<string>();


        //对焦范围 分上限和下限
        [CategoryAttribute(_Cat1), DescriptionAttribute("即初始高度正方向的上下限分为正负对称距离")]
        [DisplayName("对焦上下限")]
        [Browsable(true)]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 0.01f, 3)]
        public double FocusUpperAndLower { get; set; } = 0.2;

        //[CategoryAttribute(_Cat1), DescriptionAttribute("即初始高度正方向的上限")]
        //[DisplayName("对焦上限")]
        //[Browsable(true)]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 0.01f, 3)]
        //public double FocusUpper { get; set; } = 0.2;

        const string _Cat2 = "A02.稳定性测试设置";

        [CategoryAttribute(_Cat2), DescriptionAttribute("即抓图使用的相机编号")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("使用相机编号")]
        [Browsable(true)]
        public int CamNumberNo { get; set; } = 0;

        [CategoryAttribute(_Cat2), DescriptionAttribute("即稳定性测试次数")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("测试次数")]
        [Browsable(true)]
        public int TestCount { get; set; } = 30;

        [CategoryAttribute(_Cat2), DescriptionAttribute("即测试抓图停留时间 延时一定时间再抓图 单位ms")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("测试抓图停留时间")]
        [Browsable(true)]
        public int StayTimeMs { get; set; } = 300;

        public override void Load()
        {
            int i = 0;

            CamNumberNo = int.Parse(ReadINIValue("Basic Control", "CamNumberNo", CamNumberNo.ToString(), INIFILE));
            TestCount = int.Parse(ReadINIValue("Basic Control", "TestCount", TestCount.ToString(), INIFILE));
            StayTimeMs = int.Parse(ReadINIValue("Basic Control", "StayTimeMs", StayTimeMs.ToString(), INIFILE));

            BaseHeightZ = double.Parse(ReadINIValue("Basic Control", "BaseHeightZ", BaseHeightZ.ToString(Format), INIFILE));
            FocusUpperAndLower = double.Parse(ReadINIValue("Basic Control", "FocusUpperAndLower", FocusUpperAndLower.ToString(Format), INIFILE));
            sExposureList = ReadINIValue("Cam Control", "sExposureList", sExposureList.ToString(), INIFILE);
            ExposureArray = sExposureList.Split(';').ToArray();
            if (ExposureArray.Length < ListCount)
            {
                ExposureArray = new string[ListCount];
                i = 0;
                while (i < ListCount)
                {
                    ExposureArray[i] = "5000";
                    i++;
                }
            }
            sGainList = ReadINIValue("Cam Control", "sGainList", sGainList.ToString(), INIFILE);
            GainArray = sGainList.Split(';').ToArray();
            if (GainArray.Length < ListCount)
            {
                GainArray = new string[ListCount];
                i = 0;
                while (i < ListCount)
                {
                    GainArray[i] = "0";
                    i++;
                }
            }

            sLedValueArray = ReadINIValue("Cam Control", "sLedValueArray", sLedValueArray.ToString(), INIFILE);
            LedValueArray = sLedValueArray.Split(';').ToArray();
            if (LedValueArray.Length < ListCount)
            {
                LedValueArray = new string[ListCount];
                i = 0;
                while (i < ListCount)
                {
                    LedValueArray[i] = "0";
                    i++;
                }
            }

            scamparaArray = ReadINIValue("Cam Control", "scamparaArray", scamparaArray.ToString(), INIFILE);
            camparaArray = scamparaArray.Split(';').ToArray();
            //if (camparaArray.Length < ListCount)
            {
                camparaClassArray = new ControlClass[ListCount];
                i = 0;
                while (i < ListCount)
                {
                    camparaClassArray[i] = new ControlClass();
                    if (i < camparaArray.Length)
                    {
                        camparaClassArray[i] = new ControlClass(camparaArray[i]);
                    }
                    i++;
                }
            }

            CoarsePositioningClass.Instance.Initial(Path, Index, "CoarsePosition.ini");
            ModelPositioningClass.Instance.Initial(Path, Index, "ModelPosition.ini");


        }
        public override void Save()
        {
            WriteINIValue("Basic Control", "CamNumberNo", CamNumberNo.ToString(), INIFILE);
            WriteINIValue("Basic Control", "TestCount", TestCount.ToString(), INIFILE);
            WriteINIValue("Basic Control", "StayTimeMs", StayTimeMs.ToString(), INIFILE);
            WriteINIValue("Basic Control", "BaseHeightZ", BaseHeightZ.ToString(Format), INIFILE);
            WriteINIValue("Basic Control", "FocusUpperAndLower", FocusUpperAndLower.ToString(Format), INIFILE);
            sExposureList = arrayToString(ExposureArray);
            WriteINIValue("Cam Control", "sExposureList", sExposureList.ToString(), INIFILE);
            sGainList = arrayToString(GainArray);
            WriteINIValue("Cam Control", "sGainList", sGainList.ToString(), INIFILE);
            sLedValueArray = arrayToString(LedValueArray);
            WriteINIValue("Cam Control", "sLedValueArray", sLedValueArray.ToString(), INIFILE);
            scamparaArray = classArrayToString(camparaClassArray);
            WriteINIValue("Cam Control", "scamparaArray", scamparaArray.ToString(), INIFILE);


            CoarsePositioningClass.Instance.Save();
            ModelPositioningClass.Instance.Save();

        }

        public float GetCamExpo(int camid)
        {
            if (camid >= ExposureArray.Length)
                return 1000;

            float camExpo = 1000;
            float.TryParse(ExposureArray[camid].ToString(), out camExpo);

            return camExpo;
        }
        public void SetCamExpo(int camid, float expo)
        {
            if (camid >= ExposureArray.Length)
                return;
            ExposureArray[camid] = expo.ToString();
        }
        public float GetCamGain(int camid)
        {
            if (camid >= GainArray.Length)
                return 0;

            float camGain = 0;
            float.TryParse(GainArray[camid].ToString(), out camGain);

            return camGain;
        }
        public void SetCamGain(int camid, float gain)
        {
            if (camid >= GainArray.Length)
                return;
            GainArray[camid] = gain.ToString();
        }
        public float GetCamLedValue(int camid)
        {
            if (camid >= LedValueArray.Length)
                return 0;

            float camGain = 0;
            float.TryParse(LedValueArray[camid].ToString(), out camGain);

            return camGain;
        }
        public void SetCamLedValue(int camid, float gain)
        {
            if (camid >= LedValueArray.Length)
                return;
            LedValueArray[camid] = gain.ToString();
        }
        private string arrayToString(string[] strArray)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
            {
                if (i > 0)
                    str.Append(";");
                str.Append(strArray[i]);
            }
            return str.ToString();
        }
        private string classArrayToString(ControlClass[] strClassArray)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < strClassArray.Length; i++)
            {
                if (i > 0)
                    str.Append(";");
                str.Append(strClassArray[i].ToString());
            }
            return str.ToString();
        }
    }

    public class CoarsePositioningClass : RecipeBaseClass
    {
        public CoarsePositioningClass()
        {

        }
        private static CoarsePositioningClass _instance = null;
        public static CoarsePositioningClass Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CoarsePositioningClass();
                return _instance;
            }
        }

        const string _Cat1 = "A01.基础设置";
        [CategoryAttribute(_Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("特征定位坐标集合")]
        [Browsable(true)]
        public string sCoarsePosList { get; set; } = "";
        public List<string> CoarsePosList = new List<string>();

        public override void Load()
        {
            string str = string.Empty;
            ReadData(ref str, INIFILE);
            sCoarsePosList = str;
            CoarsePosList = sCoarsePosList.Split(';').ToList();
        }
        public override void Save()
        {
            SaveData(sCoarsePosList, INIFILE);
            CoarsePosList = sCoarsePosList.Split(';').ToList();
        }


    }
    public class ModelPositioningClass : RecipeBaseClass
    {
        public ModelPositioningClass()
        {

        }
        private static ModelPositioningClass _instance = null;
        public static ModelPositioningClass Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ModelPositioningClass();
                return _instance;
            }
        }
        const string _Cat1 = "A01.基础设置";
        [CategoryAttribute(_Cat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("定位坐标集合")]
        [Browsable(true)]
        public string sModelPosList { get; set; } = "";
        public List<string> ModelPosList = new List<string>();

        public override void Load()
        {
            string str = string.Empty;
            ReadData(ref str, INIFILE);
            sModelPosList = str;
            ModelPosList = sModelPosList.Split(';').ToList();
        }
        public override void Save()
        {
            SaveData(sModelPosList, INIFILE);
            ModelPosList = sModelPosList.Split(';').ToList();
        }


    }
}
