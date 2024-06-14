using JetEazy.BasicSpace;
using JetEazy.Interface;
using System.Drawing;
using Traveller106;

namespace NeedleX.ProcessSpace
{
    /// <summary>
    /// INIT流程 即初始化流程 所有轴在手动模式下归位 归位完成后 运动至各轴初始化位置 <br/>
    /// @LETIAN: 20220619 重新包裝
    /// </summary>
    public class ResetProcess : BaseProcess
    {
        #region ACCESS_TO_OTHER_PROCESSES
        BaseProcess m_BuzzerProcess
        {
            get { return BuzzerProcess.Instance; }
        }
        System.Diagnostics.Stopwatch m_Stopwatch = new System.Diagnostics.Stopwatch();
        #endregion

        #region SINGLETON
        static ResetProcess _singleton = null;
        private ResetProcess()
        {
        }
        #endregion

        public static ResetProcess Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new ResetProcess();
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

                        SetRunningLight();

                        axisZ.Home();
                        
                        Process.NextDuriation = 2000;
                        Process.ID = 10;

                        CommonLogClass.Instance.LogMessage("Z轴复位中", Color.Black);

                        m_Stopwatch.Restart();

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (axisZ.IsHome || Universal.IsNoUseIO)
                            {
                                axisX.Home();
                                axisY.Home();

                                axisZ.Go(axisZ.GetInitPosition(), 0);

                                m_Stopwatch.Restart();
                                //m_Stopwatch.Stop();

                                switch (Process.RelateString)
                                {
                                    case "CloseWindows":
                                        break;
                                    default:
                                        m_BuzzerProcess.Start(1);
                                        break;
                                }
                                
                                Process.NextDuriation = 100;
                                Process.ID = 11;
                            }
                            else if (m_Stopwatch.ElapsedMilliseconds >= 5 * 60 * 1000)
                            {
                                m_Stopwatch.Stop();
                                //Time out
                                Process.Stop();
                                switch (Process.RelateString)
                                {
                                    case "CloseWindows":
                                        break;
                                    default:
                                        CommonLogClass.Instance.LogMessage("Z轴复位超時", Color.Red);
                                        m_BuzzerProcess.Start(3);
                                        SetAbnormalLight();
                                        break;
                                }
                            }
                        }
                        break;
                    case 11:
                        if (Process.IsTimeup)
                        {
                            if ((axisZ.IsOK) || Universal.IsNoUseIO)
                            {
                                Process.NextDuriation = 300;
                                Process.ID = 15;
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if ((axisX.IsHome && axisY.IsHome && axisZ.IsOK) || Universal.IsNoUseIO)
                            {
                                axisX.Go(axisX.GetInitPosition(), 0);
                                axisY.Go(axisY.GetInitPosition(), 0);

                                m_Stopwatch.Restart();

                                switch (Process.RelateString)
                                {
                                    case "CloseWindows":
                                        break;
                                    default:
                                        //m_BuzzerProcess.Start(1);
                                        break;
                                }

                                Process.NextDuriation = 100;
                                Process.ID = 20;
                            }
                            else if (m_Stopwatch.ElapsedMilliseconds >= 5 * 60 * 1000)
                            {
                                m_Stopwatch.Stop();
                                //Time out
                                Process.Stop();
                                switch (Process.RelateString)
                                {
                                    case "CloseWindows":
                                        break;
                                    default:
                                        CommonLogClass.Instance.LogMessage("XY轴复位超時1", Color.Red);
                                        m_BuzzerProcess.Start(3);
                                        SetAbnormalLight();
                                        break;
                                }
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if ((axisX.IsOK && axisY.IsOK) || Universal.IsNoUseIO)
                            {
                                m_Stopwatch.Stop();

                                switch (Process.RelateString)
                                {
                                    case "CloseWindows":
                                        break;
                                    default:
                                        m_BuzzerProcess.Start(1);
                                        break;
                                }

                                Process.NextDuriation = 100;
                                Process.ID = 25;
                            }
                            else if (m_Stopwatch.ElapsedMilliseconds >= 5 * 60 * 1000)
                            {
                                m_Stopwatch.Stop();
                                //Time out
                                Process.Stop();
                                switch (Process.RelateString)
                                {
                                    case "CloseWindows":
                                        break;
                                    default:
                                        CommonLogClass.Instance.LogMessage("XY轴复位超時2", Color.Red);
                                        m_BuzzerProcess.Start(3);
                                        SetAbnormalLight();
                                        break;
                                }
                            }
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {
                            //if (!m_BuzzerProcess.IsOn)
                            {
                                Process.Stop();
                                CommonLogClass.Instance.LogMessage("所有轴复位完成", Color.Black);
                                SetNormalLight();
                                FireCompleted();
                            }
                        }
                        break;
                }
            }
        }

    }
}
