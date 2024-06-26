using AForge.Imaging.Filters;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NeedleX.Model
{
    public class CalcModeClass
    {
        public CalcModeClass() { }
        public bool xDebug { get; set; } = false;
        public int Index { get; set; } = 0;
        //public Bitmap xBmpInput { get; set; } = new Bitmap(1, 1);
        public float xPosStart { get; set; } = 0;
        public int xSetReslution { get; set; } = 5;
        public byte[] xbmpInput { get; set; } = null;
        public double xStdDev { get; set; } = -1;
        public Bitmap xbmpResult { get; set; } = new Bitmap(1, 1);
        public List<Rectangle> xRects = new List<Rectangle>();
        public List<double> xStdDevList = new List<double>();
        public void Run()
        {
            if (xbmpInput == null)
                return;

            Bitmap img1 = ConvertFromMONO(xbmpInput, 1280, 1024);

            xbmpResult.Dispose();
            xbmpResult = ConvertFromMONO(xbmpInput, 1280, 1024);

            AForge.Imaging.Filters.ContrastStretch contrastStretch = new ContrastStretch();
            Bitmap img2 = contrastStretch.Apply(img1);
            AForge.Imaging.ImageStatistics imageStatistics = new AForge.Imaging.ImageStatistics(img2);
            xStdDev = imageStatistics.Gray.StdDev;


            img1.Dispose();
            img2.Dispose();

            //Bitmap bmp = new Bitmap(xBmpInput);
            //AForge.Imaging.Filters.ExtractNormalizedRGBChannel extractNormalizedRGBChannel = new ExtractNormalizedRGBChannel(AForge.Imaging.RGB.B);
            //Bitmap bmpApply = extractNormalizedRGBChannel.Apply(bmpMerge);

        }
        public string ToReportStr()
        {
            string ret = string.Empty;

            ret = $"{GetCurrentPosition()},{xStdDev},{xPosStart},{getRealReslution},{Index}";

            return ret;
        }

        public void Run2()
        {
            if (xbmpInput == null)
                return;
            if (xRects.Count <= 0)
                return;

            xStdDevList.Clear();
            Bitmap img1 = ConvertFromMONO(xbmpInput, 1280, 1024);

            //xbmpResult.Dispose();
            //xbmpResult = ConvertFromMONO(xbmpInput, 1280, 1024);
            foreach (Rectangle rect in xRects)
            {
                Bitmap img3 = img1.Clone(rect, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                //AForge.Imaging.Filters.ContrastStretch contrastStretch = new ContrastStretch();
                //Bitmap img2 = contrastStretch.Apply(img3);
                AForge.Imaging.ImageStatistics imageStatistics = new AForge.Imaging.ImageStatistics(img3);
                double xStdDevX = imageStatistics.Gray.StdDev;
                xStdDevList.Add(xStdDevX);
                //img2.Dispose();
                img3.Dispose();
            }

            img1.Dispose();

            //Bitmap bmp = new Bitmap(xBmpInput);
            //AForge.Imaging.Filters.ExtractNormalizedRGBChannel extractNormalizedRGBChannel = new ExtractNormalizedRGBChannel(AForge.Imaging.RGB.B);
            //Bitmap bmpApply = extractNormalizedRGBChannel.Apply(bmpMerge);

        }
        public string ToReportStr2()
        {
            string ret = string.Empty;
            if (xRects.Count <= 0)
                return ret;

            ret = $"{GetCurrentPosition()},{Index},{getRealReslution},";
            int i = 0;
            foreach (Rectangle rect in xRects)
            {
                ret += $"{rect.X};{rect.Y};{rect.Width};{rect.Height},{xStdDevList[i]},";
                i++;
            }

            return ret;
        }

        public float GetCurrentPosition()
        {
            double ret = xPosStart;

            double a = getRealReslution;
            ret = xPosStart + a * (Index + 1);

            return (float)ret;
        }

        private double getRealReslution
        {
            get
            {
                double a = xSetReslution / 5.0 * 0.002;
                return a;
            }
        }
        private Bitmap ConvertFromMONO(byte[] rgbaData, int width, int height)
        {
            if (xDebug)
            {
                return GetImageByBytes(rgbaData);
            }

            var pixelFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
            Bitmap bitmap = new Bitmap(width, height, pixelFormat);

            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                pixelFormat);

            IntPtr intPtr = bitmapData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(rgbaData, 0, intPtr, rgbaData.Length);
            bitmap.UnlockBits(bitmapData);

            System.Drawing.Imaging.ColorPalette tempPalette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                tempPalette.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
            }
            bitmap.Palette = tempPalette;

            return bitmap;
        }
        /// <summary>
        /// 将Byte字节数组转化为Image图像
        /// </summary>
        /// <param name="bytes">Byte字节数组</param>
        /// <returns>返回转化后的图像</returns>
        public Bitmap GetImageByBytes(byte[] bytes)
        {
            Image photo = null;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Write(bytes, 0, bytes.Length);
                photo = Image.FromStream(ms, true);
            }
            //Gray
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply((Bitmap)photo);
            photo.Dispose();
            return bmpgray;
        }

    }
}
