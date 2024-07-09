using JetEazy.BasicSpace;
using JzDisplay.OPSpace;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using Common.RecipeSpace;

namespace NeedleX.ProcessSpace
{
    public class MainAlignProcess : BaseProcess
    {
        #region PRIVATE_DATA
        int m_UseCameraIndex = 1;
        string m_CmdCurrent = "";
        int m_StayTimeMs = 300;
        #endregion

        #region SINGLETON
        static MainAlignProcess _singleton = null;
        private MainAlignProcess()
        {
        }
        #endregion

        public static MainAlignProcess Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new MainAlignProcess();
                return _singleton;
            }
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

                        Process.NextDuriation = NextDurtimeTmp;
                        Process.ID = 10;

                        m_UseCameraIndex = RecipeNeedle.CamAlignNumberNo;
                        MyAlignImageCenter.SetControlPara(RecipeNeedle.camparaClassArray[m_UseCameraIndex].ToString());
                        MyAlignImageCenter.WholeImage = true;//全域寻找
                        m_StayTimeMs = RecipeNeedleClass.Instance.StayTimeMs;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            using (Bitmap bmp = snapshot_image(GetCamera(m_UseCameraIndex)))
                            {
                                MyAlignImageCenter.bmpInput = bmp;
                                MyAlignImageCenter.Run();

                                string _msg = string.Empty;
                                int iret = MyAlignImageCenter.IsCheckMove();
                                MyAlignResult = (Eazy_Project_III.AlignResult)iret;
                                FireLiveImaging(MyAlignImageCenter.bmpResult);
                                if (iret == -1)
                                {
                                    _msg = $"!未发现定位点";
                                }
                                else
                                {
                                    _msg = $"{(iret != 0 ? "!需要移动" : "对位正确")} 距离误差:{MyAlignImageCenter.Distance.ToString("0.000000")}";
                                    _msg += $" 移动X:{MyAlignImageCenter.MotorOffset.X.ToString("0.000000")}";
                                    _msg += $" 移动Y:{MyAlignImageCenter.MotorOffset.Y.ToString("0.000000")}";
                                }
                                FireMessage(new ProcessEventArgs($"AlignMsg.", _msg));
                                switch (MyAlignResult)
                                {
                                    case Eazy_Project_III.AlignResult.Move:

                                        m_CmdCurrent = MACHINE.GetCurrentPositionOffset(MyAlignImageCenter.MotorOffset.X, MyAlignImageCenter.MotorOffset.Y);
                                        //MACHINECollection.MotorSpeed();
                                        MACHINE.GoPosition(m_CmdCurrent, false);

                                        string[] strings = m_CmdCurrent.Split(',');
                                        AlignPointFResult = new PointF(float.Parse(strings[0]), float.Parse(strings[1]));

                                        Process.NextDuriation = m_StayTimeMs;
                                        Process.ID = 15;
                                        break;
                                    case Eazy_Project_III.AlignResult.NotMove:
                                        m_CmdCurrent = MACHINE.GetCurrentPositionOffset(MyAlignImageCenter.MotorOffset.X, MyAlignImageCenter.MotorOffset.Y);
                                        //MACHINECollection.MotorSpeed();
                                        MACHINE.GoPosition(m_CmdCurrent, false);

                                        strings = m_CmdCurrent.Split(',');
                                        AlignPointFResult = new PointF(float.Parse(strings[0]), float.Parse(strings[1]));

                                        Process.NextDuriation = NextDurtimeTmp;
                                        Process.ID = 9999;
                                        break;
                                    case Eazy_Project_III.AlignResult.NotFound:
                                        Process.NextDuriation = NextDurtimeTmp;
                                        Process.ID = 9999;
                                        break;
                                }
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true) && MACHINE.IsOnSitePosition(m_CmdCurrent))
                            {
                                Process.NextDuriation = m_StayTimeMs;
                                Process.ID = 20;
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true) && MACHINE.IsOnSitePosition(m_CmdCurrent))
                            {
                                Process.NextDuriation = NextDurtimeTmp;
                                Process.ID = 10;
                            }
                        }
                        break;
                    case 9999:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            FireCompleted();
                        }
                        break;
                }
            }
        }

    }
}
