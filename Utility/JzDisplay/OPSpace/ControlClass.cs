using JetEazy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JzDisplay.OPSpace
{
    public enum LayerMode : int
    {
        /// <summary>
        /// 找白色 白字黑底
        /// </summary>
        [Description("白字黑底")]
        White = 0,
        /// <summary>
        /// 找黑色 黑字白底
        /// </summary>
        [Description("黑字白底")]
        Black = 1,
    }

    public class ControlClass
    {
        const string _format = "0.000000";

        private double _resolution = 0.00066;
        private LayerMode _layer = LayerMode.White;
        private int _thresholdValue = 200;
        private int _cropSize = 500;
        private ControlMode _controlMode = ControlMode.None;

        public ControlClass() { }
        public ControlClass(string Str)
        {
            FromString(Str);
        }
        [Category("01.基本设置"), Description("")]
        [DisplayName("解析度"), Browsable(true)]
        public double Resolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }
        [Category("01.基本设置"), Description("")]
        [DisplayName("寻找方式"), Browsable(true)]
        [TypeConverter(typeof(JzEnumConverter))]
        public LayerMode Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }
        [Category("01.基本设置"), Description("")]
        [DisplayName("控制方式"), Browsable(true)]
        [TypeConverter(typeof(JzEnumConverter))]
        public ControlMode MyControlMode
        {
            get { return _controlMode; }
            set { _controlMode = value; }
        }
        [Category("01.基本设置"), Description("")]
        [DisplayName("灰度阈值"), Browsable(true)]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public int ThresholdValue
        {
            get { return _thresholdValue; }
            set { _thresholdValue = value; }
        }
        [Category("01.基本设置"), Description("")]
        [DisplayName("截取范围"), Browsable(true)]
        public int CropSize
        {
            get { return _cropSize; }
            set { _cropSize = value; }
        }
        public void FromString(string str)
        {
            string[] parts = str.Split(',');
            //if (parts.Length < 4)
            //    return;
            if (parts.Length > 0)
                _resolution = double.Parse(parts[0]);
            if (parts.Length > 1)
                _layer = (LayerMode)int.Parse(parts[1]);
            if (parts.Length > 2)
                _thresholdValue = int.Parse(parts[2]);
            if (parts.Length > 3)
                _cropSize = int.Parse(parts[3]);
            if (parts.Length > 4)
                _controlMode = (ControlMode)int.Parse(parts[4]);
        }
        public override string ToString()
        {
            string str = "";
            str += _resolution.ToString(_format) + ",";
            str += ((int)_layer).ToString() + ",";
            str += _thresholdValue.ToString() + ",";
            str += _cropSize.ToString() + ",";
            str += ((int)_controlMode).ToString();
            return str;
        }
    }

    public class DisplayEventArgs : EventArgs
    {
        public DisplayEventArgs(string msg = null, object tag = null)
        {
            Message = msg;
            Tag = tag;
        }

        /// <summary>
        /// sender 要通知給 receiver 的訊息.
        /// </summary>
        public string Message = null;
        public object Tag = null;

        /// <summary>
        /// Cancel Flag: 
        /// 必要時可以由 receiver 端 
        /// 來設定來通知 sender 是否要中斷 process
        /// </summary>
        public bool Cancel = false;
        public ManualResetEvent GoControlByClient = null;
    }

}

