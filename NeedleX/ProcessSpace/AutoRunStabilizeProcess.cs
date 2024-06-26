using Common.RecipeSpace;
using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedleX.ProcessSpace
{
    public class AutoRunStabilizeProcess : BaseProcess
    {
        #region ACCESS_TO_OTHER_PROCESSES
        BaseProcess m_MainProcess
        {
            get { return MainProcess.Instance; }
        }
        /// <summary>
        /// 抓图当前序号
        /// </summary>
        int m_StepIndex = 0;
        /// <summary>
        /// 抓图次数
        /// </summary>
        int m_StepCount = 0;

        #endregion

        #region SINGLETON
        static AutoRunStabilizeProcess _singleton = null;
        private AutoRunStabilizeProcess()
        {
        }
        #endregion

        public static AutoRunStabilizeProcess Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new AutoRunStabilizeProcess();
                return _singleton;
            }
        }

        /*
         * 流程注释
         * 设定位置重复抓图测试稳定性
         */

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

                        m_StepIndex = 1;
                        m_StepCount = RecipeNeedleClass.Instance.TestCount;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            m_MainProcess.Start();

                            Process.NextDuriation = 300;
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (!m_MainProcess.IsOn)
                            {
                                if (m_StepIndex < m_StepCount)
                                {
                                    //抓图完成 进行下一组
                                    Process.NextDuriation = 2000;
                                    Process.ID = 20;

                                    FireMessage(new ProcessEventArgs($"AutoRun.", $"自动测试进度 {m_StepIndex}/{m_StepCount} 持续中。。。"));
                                    m_StepIndex++;
                                }
                                else
                                {
                                    Process.Stop();
                                    FireCompleted();
                                }
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            Process.NextDuriation = 300;
                            Process.ID = 10;
                        }
                        break;
                }
            }
        }

    }
}
