using Common.RecipeSpace;
using JetEazy.BasicSpace;
using JetEazy.Interface;
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

                        Traveller106.Universal.DEBUGRESULTPATH = $"D:\\JETEAZY\\{Traveller106.Universal.VEROPT}\\DEBUG\\Main_{JzTimes.DateTimeSerialString}";

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
                                //定位完成 开始测试针点位置
                                Process.NextDuriation = 100;
                                Process.ID = 20;
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
                                using (Bitmap bmp = snapshot_image(GetCamera(0)))
                                {
                                    FireLiveImaging(bmp);
                                }

                                Process.NextDuriation = 500;
                                Process.ID = 10;
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

                                m_CmdCurrent = CoarsePositioningClass.Instance.CoarsePosList[m_CmdIndex];
                                MACHINECollection.MotorSpeed();
                                MACHINE.GoPosition(m_CmdCurrent, true);
                                m_CurrentXYZ = new NeedleXYZ(m_CmdCurrent);

                                m_CmdIndex++;
                            }
                            else
                            {
                                //针点完成
                                Process.NextDuriation = 100;
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
                                xRowEventArgs.Org = new NeedleXYZ(m_CurrentXYZ.ToString());
                                xRowEventArgs.Adjust = new NeedleXYZ(m_CurrentXYZ.ToString());
                                xRowEventArgs.Adjust.Z = FocusProcess.Instance.PosComplete;
                                xRowEventArgs.ElapsedTime = m_Stopwatch.ElapsedMilliseconds / 1000.0;

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
                }
            }
        }
        
    }
}
