using JetEazy;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JetEazy.BasicSpace
{
    public class EzDataItem
    {
        public EzDataItem() { }
        public float Position { get; set; } = 0;
        public int No { get; set; } = 0;
        public float LineOffset { get; set; } = 0.02f;
        public double[] StdDev = new double[6];
        public int StdDevCount { get; set; } = 1;
    }
    public class EzData
    {
        public EzData() { }
        public string FilePath { get; set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public int CalOffsetCount { get; set; } = 5;
        public EzPolyFit[] ezPolyFits { get; set; } = null;
        public double[] FocusPosition { get; set; } = null;
        public EzData(string filepath)
        {
            this.FilePath = filepath;
        }
        public void Run()
        {
            string fullName = FilePath + "\\DevReport.csv";
            Name = FilePath.Split('\\')[FilePath.Split('\\').Length - 1];
            if (string.IsNullOrEmpty(fullName))
                return;
            if (!File.Exists(fullName))
                return;
            List<EzDataItem> datas = GetFilenameList(fullName);
            int iCount = datas[0].StdDevCount;
            ezPolyFits = new EzPolyFit[iCount];
            FocusPosition = new double[iCount];
            int i = 0;
            while (i < iCount)
            {
                FocusPosition[i] = GetMinMax(datas, i, out ezPolyFits[i]);
                i++;
            }

        }
        public string ToShowString()
        {
            string ret = Name + Environment.NewLine;

            int i = 0;
            while (i < ezPolyFits.Length)
            {
                ret += $"StdDev{i}" + Environment.NewLine;
                ret += $"y= {ezPolyFits[i].FitA:0.000} x^2 + {ezPolyFits[i].FitB:0.000} x + {ezPolyFits[i].FitC:0.000}" + Environment.NewLine;
                ret += $"min_max = {FocusPosition[i]:0.000}" + Environment.NewLine;

                i++;
            }

            return ret;
        }
        public string ToReportString1()
        {
            string ret = Name + ",";

            int i = 0;
            while (i < FocusPosition.Length)
            {
                ret += $"{FocusPosition[i]:0.000},";
                i++;
            }
            return ret;
        }
        double GetMinMax(List<EzDataItem> ezDatas, int index, out EzPolyFit ezPolyFit)
        {
            ezPolyFit = new EzPolyFit();
            double highvalue = -1;
            int i = 0;
            int imax = 0;
            foreach (EzDataItem data in ezDatas)
            {
                if (data.StdDev[index] >= highvalue)
                {
                    highvalue = data.StdDev[index];
                    imax = i;
                }
                i++;
            }

            List<double> yvalues = new List<double>();
            List<double> xvalues = new List<double>();
            i = 0;
            foreach (EzDataItem data in ezDatas)
            {
                if (i >= imax - CalOffsetCount && i <= imax + CalOffsetCount)
                {
                    yvalues.Add(data.Position);
                    xvalues.Add(data.StdDev[index]);
                }

                i++;
            }

            var coefs = ezPolyFit.Fit(yvalues.ToArray(), xvalues.ToArray());
            //a = coefs[2];
            //b = coefs[1];
            //c = coefs[0];
            //Console.WriteLine($"y= {a:0.000} x^2 + {b:0.000} x + {c:0.000}");
            //richTextBox1.Text = $"y= {a:0.000} x^2 + {b:0.000} x + {c:0.000}" + Environment.NewLine;
            // 極值發生在 2a x + b = 0
            // x = -b / 2a
            //var x = -b / 2 / a;
            //var yminmax = a * x * x + b * x + c;
            //Console.WriteLine($"min_max = {yminmax:0.000}");
            //richTextBox1.Text += $"min_max = {yminmax:0.000}" + Environment.NewLine;

            var x = -ezPolyFit.FitB / 2 / ezPolyFit.FitA;
            var yminmax = ezPolyFit.FitA * x * x + ezPolyFit.FitB * x + ezPolyFit.FitC;
            return yminmax;
        }
        List<EzDataItem> GetFilenameList(string epath)
        {

            List<EzDataItem> dataItems = new List<EzDataItem>();
            dataItems.Clear();
            if (string.IsNullOrEmpty(epath))
                return dataItems;

            StreamReader streamReader = new StreamReader(epath);
            string line = streamReader.ReadLine();
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();
                string[] strings = line.Split(',');

                EzDataItem data = new EzDataItem();
                data.Position = float.Parse(strings[1]);
                data.No = int.Parse(strings[2]);
                data.LineOffset = float.Parse(strings[3]);
                data.StdDev[0] = double.Parse(strings[5]);
                data.StdDevCount = 1;
                if (strings.Length > 7)
                {
                    data.StdDev[1] = double.Parse(strings[7]);
                    data.StdDevCount = 2;
                }
                if (strings.Length > 9)
                {
                    data.StdDev[2] = double.Parse(strings[9]);
                    data.StdDevCount = 3;
                }
                if (strings.Length > 11)
                {
                    data.StdDev[3] = double.Parse(strings[11]);
                    data.StdDevCount = 4;
                }
                if (strings.Length > 13)
                {
                    data.StdDev[4] = double.Parse(strings[13]);
                    data.StdDevCount = 5;
                }
                if (strings.Length > 15)
                {
                    data.StdDev[5] = double.Parse(strings[15]);
                    data.StdDevCount = 6;
                }

                dataItems.Add(data);
            }
            streamReader.Close();
            streamReader.Dispose();
            return dataItems;

        }

    }
}
