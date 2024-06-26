using Common.RecipeSpace;
using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedleX.ProcessSpace
{
    public class StabilizeProcess : BaseProcess
    {
        #region ACCESS_TO_OTHER_PROCESSES
        System.Diagnostics.Stopwatch m_Stopwatch = new System.Diagnostics.Stopwatch();

        int m_CmdIndex = 0;
        int m_CmdCount = 0;
        string m_CmdCurrent = "";

        /// <summary>
        /// 抓图当前序号
        /// </summary>
        int m_StepIndex = 0;
        /// <summary>
        /// 抓图次数
        /// </summary>
        int m_StepCount = 0;
        int m_StayTimeMs = 300;

        NeedleXYZ m_CurrentXYZ = new NeedleXYZ();

        string m_Path = "D:\\STATBILIZE";

        #endregion

        #region SINGLETON
        static StabilizeProcess _singleton = null;
        private StabilizeProcess()
        {
        }
        #endregion

        public static StabilizeProcess Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new StabilizeProcess();
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

                        GetCamera(RecipeNeedleClass.Instance.CamNumberNo).SetExposure(RecipeNeedleClass.Instance.GetCamExpo(RecipeNeedleClass.Instance.CamNumberNo));

                        m_Path = "D:\\STATBILIZE\\" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        if (!Directory.Exists(m_Path))
                            Directory.CreateDirectory(m_Path);

                        m_CmdIndex = 0;
                        m_CmdCount = ModelPositioningClass.Instance.ModelPosList.Count;

                        m_StepIndex = 0;
                        m_StepCount = RecipeNeedleClass.Instance.TestCount;

                        m_StayTimeMs = RecipeNeedleClass.Instance.StayTimeMs;

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (m_CmdIndex < m_CmdCount)
                            {
                                Process.NextDuriation = 300;
                                Process.ID = 1510;

                                m_CmdCurrent = ModelPositioningClass.Instance.ModelPosList[m_CmdIndex];
                                MACHINE.GoPosition(m_CmdCurrent, true);

                                m_CmdIndex++;
                            }
                            else
                            {
                                if (m_StepIndex < m_StepCount)
                                {
                                    //抓图完成 进行下一组
                                    Process.NextDuriation = m_StayTimeMs;
                                    Process.ID = 20;

                                    m_StepIndex++;
                                }
                                else
                                {
                                    //MACHINE.GoPosition("0,0,0", true);
                                    //MACHINE.GoReadyPosition();
                                    Process.NextDuriation = 300;
                                    Process.ID = 25;

                                }
                            }
                        }
                        break;
                    case 1510:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true) && MACHINE.IsOnSitePosition(m_CmdCurrent))
                            {
                                Process.NextDuriation = m_StayTimeMs;
                                Process.ID = 15;
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true) && MACHINE.IsOnSitePosition(m_CmdCurrent))
                            {
                                using (Bitmap bmp = snapshot_image(GetCamera(RecipeNeedleClass.Instance.CamNumberNo)))
                                {
                                    if (m_CmdIndex == 1)
                                        bmp.Save(m_Path + "\\" + m_StepIndex.ToString() + "-" + m_CmdIndex.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                                    FireLiveImaging(bmp);
                                }

                                Process.NextDuriation = 300;
                                Process.ID = 10;
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            m_CmdIndex = 0;

                            Process.NextDuriation = 300;
                            Process.ID = 10;
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnsite(true))
                            {
                                Process.Stop();
                                FireCompleted();
                            }
                        }
                        break;
                }
            }
        }

    }
}
