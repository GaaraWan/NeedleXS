using JetEazy.BasicSpace;
using JetEazy.Interface;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Traveller106;

namespace NeedleX.ProcessSpace
{
    public class FocusProcess : BaseProcess
    {
        #region ACCESS_TO_OTHER_PROCESSES
        System.Diagnostics.Stopwatch m_Stopwatch = new System.Diagnostics.Stopwatch();

        float d1 = 0;
        float d2 = 0;
        float offset = 0.01f;

        int m_AfMode = 1;
        int m_nextTime = 100;
        float m_PosComplete = 0;

        string GetModeName()
        {
            string _namestr = string.Empty;
            switch (m_AfMode)
            {
                case 0:
                    _namestr = "0 - Laplacian";
                    break;
                case 1:
                    _namestr = "1 - MeanStdDev";
                    break;
                case 2:
                    _namestr = "2 - Sobel";
                    break;
            }
            return _namestr;
        }

        class focusItemClass
        {
            public float fCurrentPos = 0f;
            public double dScore = 0d;
        }

        List<focusItemClass> focusList = new List<focusItemClass>();

        string m_filenamepath = "";

        
        #endregion

        #region SINGLETON
        static FocusProcess _singleton = null;
        private FocusProcess()
        {
        }
        #endregion

        public static FocusProcess Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new FocusProcess();
                return _singleton;
            }
        }

        //public List<focusItemClass> xFocusCompleteList
        //{
        //    get { return focusList; }
        //}
        /// <summary>
        /// 对焦完成位置
        /// </summary>
        public float PosComplete
        {
            get { return m_PosComplete; }
        }
        public void SetFocusRecipe(float eCurrentPos, float eOffset = 0.005f, int eFocusMode = 1)
        {
            m_PosComplete = eCurrentPos;
            d1 = eCurrentPos - 0.05f;
            d2 = eCurrentPos + 0.05f;
            offset = eOffset;
            m_AfMode = eFocusMode;
        }
        public void SetFocusRecipe(float ed1,float ed2, float eOffset = 0.005f, int eFocusMode = 1)
        {
            m_PosComplete = ed1;
            d1 = ed1;
            d2 = ed2;
            offset = eOffset;
            m_AfMode = eFocusMode;
        }
        public override void Tick()
        {
            //if (!IsValidPlcScanned())
            //    return;

            var Process = this;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 300;
                        Process.ID = 10;

                        focusList.Clear();

                        //d1 = 112.15f;
                        //d2 = 112.25f;
                        //offset = 0.01f;

                        //float.TryParse(txt1.Text.Trim(), out d1);
                        //float.TryParse(txt2.Text.Trim(), out d2);
                        //float.TryParse(txtoffset.Text.Trim(), out offset);

                        CommonLogClass.Instance.LogMessage("开始位置=" + d1.ToString());
                        CommonLogClass.Instance.LogMessage("结束位置=" + d2.ToString());
                        CommonLogClass.Instance.LogMessage("间隔=" + offset.ToString());

                        if (INI.Instance.IsSaveImageOpen)
                        {
                            m_filenamepath = Traveller106.Universal.DEBUGRESULTPATH + "\\" + JzTimes.DateTimeSerialStringFFF;
                            if (!System.IO.Directory.Exists(m_filenamepath))
                                System.IO.Directory.CreateDirectory(m_filenamepath);
                        }
                     
                        //d1 = float.Parse(txt1.Text.Trim());
                        //d2 = float.Parse(txt2.Text.Trim());
                        //offset = float.Parse(txtoffset.Text.Trim());


                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (d1 < d2)
                            {
                                axisZ.Go(d1, 0);
                                //d1 -= offset;
                                //CommonLogClass.Instance.LogMessage("当前位置=" + d1.ToString());
                                Process.NextDuriation = m_nextTime;
                                Process.ID = 15;
                            }
                            else
                            {
                                Process.NextDuriation = m_nextTime;
                                Process.ID = 20;
                            }

                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            float _posCur = (float)axisZ.GetPos();
                            if (IsInRange(_posCur, d1, 0.005f) && axisZ.IsOK)
                            {
                                Process.NextDuriation = m_nextTime;
                                Process.ID = 10;

                                CamHeight.Snap();
                                Bitmap bmp = new Bitmap(CamHeight.GetSnap());

                                if (INI.Instance.IsSaveImageOpen)
                                    bmp.Save(m_filenamepath + "\\pos_" + d1.ToString("0.0000") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                                Mat source = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
                                double _score = GetImgQualityScore(source, m_AfMode);
                                DrawText(bmp, $"(Method:{GetModeName()}) Score:{_score.ToString("0.000")}");
                                //pictureBox1.Image = bmp;
                                FireLiveImaging(bmp);

                                CommonLogClass.Instance.LogMessage("位置=" + d1.ToString());
                                CommonLogClass.Instance.LogMessage($"(Method:{GetModeName()}) Score:{_score.ToString("0.000")}");
                                focusItemClass focus = new focusItemClass();
                                focus.fCurrentPos = d1;
                                focus.dScore = _score;
                                focusList.Add(focus);

                                bmp.Dispose();
                                source.Dispose();

                                d1 += offset;
                            }

                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (axisZ.IsOK)
                            {
                                //Process.Stop();
                                focusList.Sort((item1, item2) => { return item1.dScore > item2.dScore ? -1 : 1; });
                                if (focusList.Count > 0)
                                {
                                    m_PosComplete = focusList[0].fCurrentPos;
                                    axisZ.Go(focusList[0].fCurrentPos, 0);
                                    CommonLogClass.Instance.LogMessage($"(Method:{GetModeName()}) " +
                                        $"(Current Pos:{focusList[0].fCurrentPos.ToString("0.000")}) " +
                                        $"Max Score:{focusList[0].dScore.ToString("0.000")}");

                                    Process.NextDuriation = m_nextTime;
                                    Process.ID = 25;
                                }
                                else
                                {
                                    Process.Stop();
                                    FireCompleted();
                                }
                            }
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {
                            float _posCur = (float)axisZ.GetPos();
                            if (IsInRange(_posCur, focusList[0].fCurrentPos, 0.005f) && axisZ.IsOK)
                            {
                                Process.Stop();
                                CamHeight.Snap();
                                Bitmap bmp = new Bitmap(CamHeight.GetSnap());
                                FireLiveImaging(bmp);

                                bmp.Dispose();
                                FireCompleted();
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
        double GetImgQualityScore(Mat src, int flag)
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
        void DrawText(Bitmap BMP, string Text)
        {
            SolidBrush B = new SolidBrush(Color.Lime);
            Font MyFont = new Font("Arial", 88);

            Graphics g = Graphics.FromImage(BMP);
            g.DrawString(Text, MyFont, B, new PointF(5, 5));
            g.Dispose();
        }
        //protected void FireLiveImaging(Bitmap bmp)
        //{
        //    try
        //    {
        //        OnLiveImage?.Invoke(this, new ProcessEventArgs("live.image", bmp));
        //    }
        //    catch (Exception ex)
        //    {
        //        _LOG(ex, "FireLiveImaging 異常!");
        //    }
        //}




    }
}
