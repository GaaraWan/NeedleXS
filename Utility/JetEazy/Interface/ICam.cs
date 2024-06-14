using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetEazy.Interface
{
    public interface IxLineScanCam : IDisposable
    {
        void Init(bool debug, string inipara);
        bool IsSim();
        bool Open();
        bool Open(string configFile);
        bool Close();
        bool IsGrapImageOK { get; set; }
        bool IsGrapImageComplete { get; set; }

        void SoftTrigger();
        /// <summary>
        /// 获取图像
        /// 大于0 放大倍数 等于0 不变尺寸 小于0缩小倍数
        /// </summary>
        /// <param name="size">大于0 放大倍数 等于0 不变尺寸 小于0缩小倍数</param>
        /// <returns>返回图像</returns>
        System.Drawing.Bitmap GetPageBitmap(int size = 0);
        //System.Drawing.Bitmap GetPageBitmap(int size);

    }
    public interface ICam
    {
        bool IsSim();
        int Initial(string inipara);
        void SetExposure(int val);
        void SetExposure(float val);
        void SetGain(float val);
        void StartCapture();
        void StopCapture();
        void Snap();
        System.Drawing.Bitmap GetSnap(int msec = 1000);
        int RotateAngle { get; set; }
    }
    public interface IAxis
    {
        bool IsError { get; }
        bool IsOK { get; }
        bool IsHome { get; }

        void Home();
        void Forward();
        void Backward();
        void Stop();
        void Go(double frompos, double offset);
        void SetManualSpeed(int val);
        void SetActionSpeed(int val);
        double GetPos();
        string GetStatus();

        double GetInitPosition();

        void Go(int posindex, float position);
        void SetPos(int posindex, float position);
        void GoPos(int posindex);
        bool IsOnSitePos(int posindex);
        float GetSetPos(int posindex);

    }
}
