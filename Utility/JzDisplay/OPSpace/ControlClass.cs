using AHBlobPro;
using JetEazy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
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
        [Category("01.基本设置"), Description("")]
        [DisplayName("过滤小面积"), Browsable(true)]
        public int AreaMin { get; set; } = 0;
        [Category("01.基本设置"), Description("")]
        [DisplayName("过滤大面积"), Browsable(true)]
        public int AreaMax { get; set; } = 1280 * 1024;
        [Category("01.基本设置"), Description("")]
        [DisplayName("对位中心误差"), Browsable(true)]
        public double AlignCenterError { get; set; } = 0.02d;
        [Category("01.基本设置"), Description("")]
        [DisplayName("显示灰阶图"), Browsable(true)]
        public bool IsShowThresholdImage { get; set; } = false;


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
            if (parts.Length > 8)
            {
                AreaMin = int.Parse(parts[5]);
                AreaMax = int.Parse(parts[6]);
                AlignCenterError = double.Parse(parts[7]);
                IsShowThresholdImage = parts[8] == "1";
            }

        }
        public override string ToString()
        {
            string str = "";
            str += _resolution.ToString(_format) + ",";
            str += ((int)_layer).ToString() + ",";
            str += _thresholdValue.ToString() + ",";
            str += _cropSize.ToString() + ",";
            str += ((int)_controlMode).ToString() + ",";
            str += AreaMin.ToString() + ",";
            str += AreaMax.ToString() + ",";
            str += AlignCenterError.ToString(_format) + ",";
            str += (IsShowThresholdImage ? "1" : "0");
            return str;
        }
    }

    public class AlignImageCenterClass
    {
        private Bitmap m_bmpInput = new Bitmap(1, 1);
        private Bitmap m_bmpResult = new Bitmap(1, 1);
        private PointF m_MotorOffset = new PointF(0, 0);
        ControlClass MyControlPara = new ControlClass();
        private double m_Distance = 0;
        public AlignImageCenterClass() { }
        public Bitmap bmpInput
        {
            get { return m_bmpInput; }
            set
            {
                m_bmpInput.Dispose();
                m_bmpInput = new Bitmap(value);
            }
        }
        public Bitmap bmpResult
        {
            get { return m_bmpResult; }
            //set
            //{
            //    m_bmpInput.Dispose();
            //    m_bmpInput = new Bitmap(value);
            //}
        }
        public PointF MotorOffset
        {
            get { return m_MotorOffset; }
        }
        public double Distance
        {
            get { return m_Distance; }
        }
        public void SetControlPara(string eStr)
        {
            if (!string.IsNullOrEmpty(eStr))
                MyControlPara.FromString(eStr);
        }
        public bool IsCheckMove()
        {
            bool ret = true;

            m_Distance = GetPointLength(m_MotorOffset, new PointF(0, 0));
            if (m_Distance >= -Math.Abs(MyControlPara.AlignCenterError) && m_Distance <= Math.Abs(MyControlPara.AlignCenterError))
                ret = false;

            return ret;
        }
        public int Run()
        {
            PointF _imageCenter = new PointF(bmpInput.Width / 2, bmpInput.Height / 2);
            JRotatedRectangleF _jRotatedRectangleF = new JRotatedRectangleF();
            PointF _imageAlignPos = getMoveBlobOffset(_imageCenter, out _jRotatedRectangleF);
            PointF ptOffset = new PointF(-(float)((_jRotatedRectangleF.fCX - _imageCenter.X)),
                                                            -(float)((_jRotatedRectangleF.fCY - _imageCenter.Y)));

            m_MotorOffset = new PointF((float)(ptOffset.X * MyControlPara.Resolution),
                                                                 (float)(ptOffset.Y * MyControlPara.Resolution));


            return 0;
        }
        #region Cal Funtion

        private PointF getMoveBlobOffset(PointF ptLocation, out JRotatedRectangleF jRotatedRectangleF)
        {
            PointF ptret = new PointF(0, 0);
            jRotatedRectangleF = new JRotatedRectangleF();

            m_bmpResult.Dispose();
            m_bmpResult = new Bitmap(bmpInput);

            //点击的位置扩成矩形框
            Rectangle rectLocation = SimpleRect(ptLocation, MyControlPara.CropSize / 2);
            BoundRect(ref rectLocation, m_bmpInput.Size);
            Bitmap bmptemp = m_bmpInput.Clone(rectLocation, PixelFormat.Format24bppRgb);
            JetGrayImg grayimage = new JetGrayImg(bmptemp);
            JetImgproc.Threshold(grayimage, MyControlPara.ThresholdValue, grayimage);
            JetBlob jetBlob = new JetBlob();
            //if (MyControlPara.IsShowThresholdImage)
            //{
            //    m_bmpResult.Dispose();
            //    m_bmpResult = new Bitmap(grayimage.ToBitmap());
            //}
            //grayimage.ToBitmap().Save("d:\\bmpDisplayTemp.png", ImageFormat.Png);
            if (MyControlPara.Layer == LayerMode.Black)
                jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            else
                jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;
            int iMax = -10000000;
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                //if (iArea < MyControlPara.CropSize * MyControlPara.CropSize / 6)
                //    continue;
                if (iArea < MyControlPara.AreaMin || iArea > MyControlPara.AreaMax)
                    continue;
                if (iArea > iMax)
                {
                    iMax = iArea;
                    JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);
                    ptret = new PointF((float)jetrect.fCX, (float)jetrect.fCY);
                    jRotatedRectangleF.fCX = jetrect.fCX;
                    jRotatedRectangleF.fCY = jetrect.fCY;
                    jRotatedRectangleF.fWidth = jetrect.fWidth;
                    jRotatedRectangleF.fHeight = jetrect.fHeight;
                    jRotatedRectangleF.fAngle = jetrect.fAngle;
                }
            }

            ptret.X += rectLocation.X;
            ptret.Y += rectLocation.Y;

            jRotatedRectangleF.fCX += rectLocation.X;
            jRotatedRectangleF.fCY += rectLocation.Y;

            Graphics g = Graphics.FromImage(m_bmpResult);
            if (MyControlPara.IsShowThresholdImage)
                g.DrawImage(grayimage.ToBitmap(), rectLocation);
            RectangleF _rectF = SimpleRectF(ptret, (float)(jRotatedRectangleF.fWidth / 2), (float)(jRotatedRectangleF.fHeight / 2));
            //转换矩形的四个角
            PointF[] myPts = RectToPointF(_rectF, -jRotatedRectangleF.fAngle);
            Pen p = new Pen(Color.Lime, 3);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
            Pen p2 = new Pen(Color.Red, 3);
            //pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawRectangle(p, rectLocation);
            g.DrawLine(p2, myPts[0], myPts[1]);
            g.DrawLine(p2, myPts[0], myPts[2]);
            g.DrawLine(p2, myPts[1], myPts[3]);
            g.DrawLine(p2, myPts[2], myPts[3]);

            g.Dispose();

            return ptret;
        }
        private void BoundRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        private int BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);
        }
        private Rectangle SimpleRect(PointF PtF, int SizeValue)
        {
            Rectangle rect = new Rectangle((int)PtF.X - SizeValue, (int)PtF.Y - SizeValue, SizeValue << 1, SizeValue << 1);
            return rect;
        }
        private PointF[] RectToPointF(RectangleF xRect, double xAngle)
        {
            PointF[] pts = new PointF[4];

            PointF ptCenter = GetRectCenter(xRect);
            pts[0] = xRect.Location;
            pts[1] = new PointF(xRect.Location.X, xRect.Bottom);
            pts[2] = new PointF(xRect.Right, xRect.Location.Y);
            pts[3] = new PointF(xRect.Right, xRect.Bottom);

            pts[0] = PointRotate(ptCenter, pts[0], xAngle);
            pts[1] = PointRotate(ptCenter, pts[1], xAngle);
            pts[2] = PointRotate(ptCenter, pts[2], xAngle);
            pts[3] = PointRotate(ptCenter, pts[3], xAngle);

            return pts;
        }
        private PointF PointRotate(PointF center, PointF p1, double angle)
        {
            PointF tmp = new PointF();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (float)x1;
            tmp.Y = (float)y1;
            return tmp;
        }
        private PointF GetRectCenter(RectangleF Rect)
        {
            return new PointF(Rect.X + (Rect.Width / 2), Rect.Y + (Rect.Height / 2));
        }
        private RectangleF SimpleRectF(PointF Pt, float Width, float Height)
        {
            RectangleF rect = SimpleRectF(Pt);
            rect.Inflate(Width, Height);
            return rect;
        }
        private RectangleF SimpleRectF(PointF Pt)
        {
            return new RectangleF(Pt.X, Pt.Y, 1, 1);
        }
        double GetPointLength(PointF P1, PointF P2)
        {
            return Math.Sqrt((double)Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }
        #endregion
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

