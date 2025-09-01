using Common.RecipeSpace;
using JetEazy.BasicSpace;
using JetEazy.Interface;
using NeedleX.OPSpace;
using NeedleX.UISpace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeedleX.ProcessSpace
{
    public class MainProcess : BaseProcess
    {
        #region ACCESS_TO_OTHER_PROCESSES
        System.Diagnostics.Stopwatch m_Stopwatch = new System.Diagnostics.Stopwatch();

        int m_CmdIndex = 0;
        int m_CmdCount = 0;
        string m_CmdCurrent = "";
        int m_StayTimeMs = 300;

        NeedleXYZ m_CurrentXYZ = new NeedleXYZ();
        NeedleXYZ m_AdjustXYZ = new NeedleXYZ();
        XRowEventArgs xRowEventArgs = new XRowEventArgs();

        #endregion

        #region SINGLETON
        static MainProcess _singleton = null;
        private MainProcess()
        {
        }
        #endregion

        public static MainProcess Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new MainProcess();
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

                        //FireMessage(new ProcessEventArgs($"ResetData"));
                        //MACHINE.PLCIO.SetLedValue(0, 150);
                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        m_CmdIndex = 0;
                        m_CmdCount = ModelPositioningClass.Instance.ModelPosList.Count;
                        m_StayTimeMs = RecipeNeedleClass.Instance.StayTimeMs;

                        MACHINE.PLCIO.LedReset();
                        MACHINE.PLCIO.SetLedValue(0, (int)RecipeNeedle.GetCamLedValue(0));
                        FireMessage(new ProcessEventArgs($"ResetData."));

                        MyAlignCalibration.Reset();
                        Traveller106.Universal.DEBUGRESULTPATH = $"D:\\JETEAZY\\{Traveller106.Universal.VEROPT}\\DEBUG\\Main_{JzTimes.DateTimeSerialString}";

                        if (m_CmdCount < 4)
                        {
                            Process.NextDuriation = 500;
                            Process.ID = 9998;
                        }
                        else
                        {
                            switch (alignFuntion)
                            {
                                case Eazy_Project_III.AlignFuntion.Vector:
                                    m_CmdCount = 2;
                                    string k0 = ModelPositioningClass.Instance.ModelPosList[2];
                                    string k1 = ModelPositioningClass.Instance.ModelPosList[3];

                                    MyAlignCalibration.Set(k0, k1);

                                    break;
                                case Eazy_Project_III.AlignFuntion.Calibration:

                                    break;
                            }

                            MyAlignCalibration.xAlignFuntion = alignFuntion;
                        }

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (m_CmdIndex < m_CmdCount)
                            {
                                Process.NextDuriation = 500;
                                Process.ID = 15;

                                m_CmdCurrent = ModelPositioningClass.Instance.ModelPosList[m_CmdIndex];
                                MACHINECollection.MotorSpeed();
                                MACHINE.GoPosition(m_CmdCurrent, true);

                                m_CmdIndex++;
                            }
                            else
                            {
                                int iret = MyAlignCalibration.Run();
                                //FireMessage(new ProcessEventArgs($"CallMsg.", $"{(iret == 0 ? "校正成功" : "校正失败")}"));
                                if (iret == 0)
                                {
                                    //定位完成 开始测试针点位置
                                    Process.NextDuriation = 500;
                                    Process.ID = 20;
                                    FireMessage(new ProcessEventArgs($"CallMsg.", $"校正成功"));
                                }
                                else
                                {
                                    Process.Stop();
                                    FireMessage(new ProcessEventArgs($"CallMsg.", $"校正失败"));
                                    FireCompleted();
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
                                Process.ID = 1510;
                            }
                        }
                        break;
                    case 1510:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true) && MACHINE.IsOnSitePosition(m_CmdCurrent))
                            {
                                MainAlignProcess.Instance.Start();

                                Process.NextDuriation = 500;
                                Process.ID = 1520;
                            }
                        }
                        break;
                    case 1520:
                        if (Process.IsTimeup)
                        {
                            if (!MainAlignProcess.Instance.IsOn)
                            {
                                switch (MyAlignResult)
                                {
                                    case Eazy_Project_III.AlignResult.Move:
                                    case Eazy_Project_III.AlignResult.NotMove:

                                        Process.NextDuriation = 500;
                                        Process.ID = 10;

                                        string[] strings = m_CmdCurrent.Split(',');
                                        PointF pt1 = new PointF(float.Parse(strings[0]), float.Parse(strings[1]));

                                        MyAlignCalibration.Add(pt1, AlignPointFResult);


                                        break;
                                    case Eazy_Project_III.AlignResult.NotFound:

                                        Process.NextDuriation = 500;
                                        Process.ID = 9999;

                                        break;
                                }
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            m_CmdIndex = 0;
                            m_CmdCount = CoarsePositioningClass.Instance.CoarsePosList.Count;
                            MACHINE.PLCIO.LedReset();
                            Process.NextDuriation = 100;
                            Process.ID = 25;
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {
                            if (m_CmdIndex < m_CmdCount)
                            {
                                Process.NextDuriation = 500;
                                Process.ID = 30;

                                m_CurrentXYZ = new NeedleXYZ(CoarsePositioningClass.Instance.CoarsePosList[m_CmdIndex]);
                                //转换校正的点
                                m_CmdCurrent = MyAlignCalibration.OutputStr(CoarsePositioningClass.Instance.CoarsePosList[m_CmdIndex]);
                                m_AdjustXYZ = new NeedleXYZ(m_CmdCurrent);
                                MACHINECollection.MotorSpeed();
                                MACHINE.GoPosition(m_CmdCurrent, true);

                                m_CmdIndex++;
                            }
                            else
                            {
                                //针点完成
                                Process.NextDuriation = 500;
                                Process.ID = 40;
                            }
                        }
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true) && MACHINE.IsOnSitePosition(m_CmdCurrent))
                            {
                                Process.NextDuriation = m_StayTimeMs;
                                Process.ID = 3010;
                            }
                        }
                        break;
                    case 3010:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true) && MACHINE.IsOnSitePosition(m_CmdCurrent))
                            {
                                FocusProcess.Instance.SetFocusRecipe((float)m_CurrentXYZ.Z, eFocusMode: 1);
                                FocusProcess.Instance.Start("MainOnFire");
                                m_Stopwatch.Restart();

                                Process.NextDuriation = 500;
                                Process.ID = 35;
                            }
                        }
                        break;
                    case 35:
                        if (Process.IsTimeup)
                        {
                            if (!FocusProcess.Instance.IsOn)
                            {
                                m_Stopwatch.Stop();

                                Process.NextDuriation = 100;
                                Process.ID = 25;

                                xRowEventArgs.Index = m_CmdIndex;

                                switch (alignFuntion)
                                {
                                    case Eazy_Project_III.AlignFuntion.Vector:

                                        xRowEventArgs.Org = new NeedleXYZ(m_AdjustXYZ.ToString());
                                        xRowEventArgs.Adjust = new NeedleXYZ(m_AdjustXYZ.ToString());
                                        xRowEventArgs.Adjust.X = FocusProcess.Instance.PosCompleteX;
                                        xRowEventArgs.Adjust.Y = FocusProcess.Instance.PosCompleteY;
                                        xRowEventArgs.Adjust.Z = FocusProcess.Instance.PosComplete;
                                        xRowEventArgs.ElapsedTime = m_Stopwatch.ElapsedMilliseconds / 1000.0;

                                        break;
                                    case Eazy_Project_III.AlignFuntion.Calibration:

                                        xRowEventArgs.Org = new NeedleXYZ(m_CurrentXYZ.ToString());
                                        xRowEventArgs.Adjust = new NeedleXYZ(m_AdjustXYZ.ToString());
                                        xRowEventArgs.Adjust.X = FocusProcess.Instance.PosCompleteX;
                                        xRowEventArgs.Adjust.Y = FocusProcess.Instance.PosCompleteY;
                                        xRowEventArgs.Adjust.Z = FocusProcess.Instance.PosComplete;
                                        xRowEventArgs.ElapsedTime = m_Stopwatch.ElapsedMilliseconds / 1000.0;

                                        break;
                                }
                                FireMessage(new ProcessEventArgs($"FocusCompleted.{m_CmdCount.ToString()}", xRowEventArgs));
                            }
                        }
                        break;
                    case 40:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            string report_path = $"D:\\Report\\Needle";
                            if (!Directory.Exists(report_path))
                                Directory.CreateDirectory(report_path);
                            FireMessage(new ProcessEventArgs($"SaveReport.", $"{report_path}\\Single_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv"));
                            FireCompleted();
                        }
                        break;
                    case 9997:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            FireMessage(new ProcessEventArgs($"CallMsg.", $"定位点个数小于2个，流程强制停止。"));
                            FireCompleted();
                        }
                        break;
                    case 9998:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            FireMessage(new ProcessEventArgs($"CallMsg.", $"定位点个数小于4个，流程强制停止。"));
                            FireCompleted();
                        }
                        break;
                    case 9999:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            FireMessage(new ProcessEventArgs($"CallMsg.", $"对位失败，未找到对位点。"));
                            FireCompleted();
                        }
                        break;
                }
            }
        }

    }
}
