using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedleX.OPSpace
{
    public class AlignCalibration
    {
        JetEazy.BasicSpace.CAoiCalibration cAoiCalibration = new JetEazy.BasicSpace.CAoiCalibration();
        PointF[,] _v = new PointF[2, 2];
        PointF[,] _w = new PointF[2, 2];

        int m_Index = 0;

        public AlignCalibration() { }
        public void Reset()
        {
            m_Index = 0;
        }
        public void Add(PointF pv, PointF pw)
        {
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
        }
        public int Run()
        {
            if (m_Index < 4)
                return -1;
            cAoiCalibration.Dispose();
            cAoiCalibration.SetCalibrationPoints(_v, _w);
            cAoiCalibration.CalculateTransformMatrix();
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
        /// <returns></returns>
        public string OutputStr(string eInputCurrentPosition)
        {
            string[] strs = eInputCurrentPosition.Split(',');
            float x = (float)Math.Round(float.Parse(strs[0]), 3);
            float y = (float)Math.Round(float.Parse(strs[1]), 3);
            float z = (float)Math.Round(float.Parse(strs[2]), 3);

            PointF ret = ViewToWorld(new PointF(x, y));

            return $"{ret.X.ToString("0.000")},{ret.Y.ToString("0.000")},{z.ToString("0.000")}";
        }

    }
}
