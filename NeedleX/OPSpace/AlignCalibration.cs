using Eazy_Project_III;
using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDemo.LaserDot;

namespace NeedleX.OPSpace
{
    public class AlignCalibration
    {
        JetEazy.BasicSpace.CAoiCalibration cAoiCalibration = new JetEazy.BasicSpace.CAoiCalibration();
        PointF[,] _v = new PointF[2, 2];
        PointF[,] _w = new PointF[2, 2];

        LaserDotCoordinate laserDotCoordinate = new LaserDotCoordinate();

        //向量的方式
        PointF _k0 = new PointF();
        PointF _k1 = new PointF();
        PointF _q0 = new PointF();
        PointF _q1 = new PointF();
        public void Set(string eK0, string eK1)
        {
            string[] strs = eK0.Split(',');
            float x = (float)Math.Round(float.Parse(strs[0]), 3);
            float y = (float)Math.Round(float.Parse(strs[1]), 3);
            float z = (float)Math.Round(float.Parse(strs[2]), 3);
            _k0 = new PointF(x, y);

            strs = eK1.Split(',');
            x = (float)Math.Round(float.Parse(strs[0]), 3);
            y = (float)Math.Round(float.Parse(strs[1]), 3);
            z = (float)Math.Round(float.Parse(strs[2]), 3);
            _k1 = new PointF(x, y);
        }

        int m_Index = 0;
        private AlignFuntion m_Align = AlignFuntion.Vector;

        public AlignFuntion xAlignFuntion
        {
            get { return m_Align; }
            set { m_Align = value; }
        }

        public AlignCalibration() { }
        public void Reset()
        {
            m_Index = 0;
        }
        public void Add(PointF pv, PointF pw)
        {
            switch (m_Align)
            {
                case AlignFuntion.Calibration:

                    switch (m_Index)
                    {
                        case 0:
                            _v[0, 0] = new PointF(pv.X, pv.Y);
                            _w[0, 0] = new PointF(pw.X, pw.Y);
                            break;
                        case 1:
                            _v[0, 1] = new PointF(pv.X, pv.Y);
                            _w[0, 1] = new PointF(pw.X, pw.Y);
                            break;
                        case 2:
                            _v[1, 0] = new PointF(pv.X, pv.Y);
                            _w[1, 0] = new PointF(pw.X, pw.Y);
                            break;
                        case 3:
                            _v[1, 1] = new PointF(pv.X, pv.Y);
                            _w[1, 1] = new PointF(pw.X, pw.Y);
                            break;
                    }
                    m_Index++;

                    break;
                case AlignFuntion.Vector:

                    switch (m_Index)
                    {
                        case 0:
                            //_k0 = new PointF(pv.X, pv.Y);
                            _q0 = new PointF(pw.X, pw.Y);
                            break;
                        case 1:
                            //_k1 = new PointF(pv.X, pv.Y);
                            _q1 = new PointF(pw.X, pw.Y);
                            break;
                    }
                    m_Index++;

                    break;
            }
        }
        public int Run()
        {
            switch (m_Align)
            {
                case AlignFuntion.Calibration:

                    if (m_Index < 4)
                        return -1;
                    cAoiCalibration.Dispose();
                    cAoiCalibration.SetCalibrationPoints(_v, _w);
                    cAoiCalibration.CalculateTransformMatrix();
                    return 0;

                    break;
                case AlignFuntion.Vector:

                    return 0;

                    break;
            }

            return 0;
        }
        public PointF ViewToWorld(PointF ptview)
        {
            PointF ptworld = new PointF(ptview.X, ptview.Y);
            cAoiCalibration.TransformViewToWorld(ptview, out ptworld);
            return ptworld;
        }
        public PointF WorldToView(PointF ptworld)
        {
            PointF ptview = new PointF(ptworld.X, ptworld.Y);
            cAoiCalibration.TransformViewToWorld(ptworld, out ptview);
            return ptview;
        }
        /// <summary>
        /// 输入当前位置 返回校正后的位置
        /// </summary>
        /// <param name="eInputCurrentPosition">输入位置</param>
        /// <param name="fZManual">向量的时候输入固定的Z</param>
        /// <returns></returns>
        public string OutputStr(string eInputCurrentPosition, float fZManual = 0)
        {
            string[] strs = eInputCurrentPosition.Split(',');
            float x = (float)Math.Round(float.Parse(strs[0]), 3);
            float y = (float)Math.Round(float.Parse(strs[1]), 3);
            float z = (float)Math.Round(float.Parse(strs[2]), 3);

            PointF ret = new PointF();// ViewToWorld(new PointF(x, y));
            string retStr = $"0,0,0";

            switch (m_Align)
            {
                case AlignFuntion.Calibration:

                    ret = ViewToWorld(new PointF(x, y));
                    retStr = $"{ret.X.ToString("0.000")},{ret.Y.ToString("0.000")},{z.ToString("0.000")}";

                    break;
                case AlignFuntion.Vector:

                    laserDotCoordinate.CalcPointQ2(_k0, _k1, new PointF(x, y), _q0, _q1, out PointF q2, out double phi);
                    retStr = $"{q2.X.ToString("0.000")},{q2.Y.ToString("0.000")},{fZManual.ToString("0.000")}";

                    break;
            }


            return retStr;
        }

    }
}
