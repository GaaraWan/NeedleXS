using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeedleX.ProcessSpace
{
    public class MainDSLiveProcess : BaseProcess
    {
        #region PRIVATE_DATA
        ///// <summary>
        ///// 叫的第几次
        ///// </summary>
        //int m_BuzzerIndex = 0;
        ///// <summary>
        ///// 叫几声
        ///// </summary>
        //int m_BuzzerCount = 3;
        #endregion

        #region SINGLETON
        static MainDSLiveProcess _singleton = null;
        private MainDSLiveProcess()
        {
        }
        #endregion

        public static MainDSLiveProcess Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new MainDSLiveProcess();
                return _singleton;
            }
        }

        /// <summary>
        /// 第一個參數可以指定 m_BuzzerCount
        /// </summary>
        /// <param name="args">args[0] 可以指定 m_BuzzerCount</param>
        public override void Start(params object[] args)
        {
            //m_BuzzerIndex = 0;
            //m_BuzzerCount = 3;
            //try
            //{
            //    if (args.Length > 0)
            //        m_BuzzerCount = (int)args[0];
            //}
            //catch
            //{

            //}
            ((ProcessClass)this).Start();
        }

        public override void Tick()
        {
            //if (!IsValidPlcScanned())
            //    return;

            var Process = this;

            //iNextDurtime[3] = 1000;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.NextDuriation = NextDurtimeTmp;
                        Process.ID = 10;

                        //switch (Process.RelateString)
                        //{
                        //    default:
                        //        m_BuzzerIndex = 0;
                        //        //m_BuzzerCount = 3;
                        //        break;
                        //}

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if (m_BuzzerIndex < m_BuzzerCount)
                            //{
                            //    _set_buzzer(true);
                            //    Process.NextDuriation = 500;
                            //    Process.ID = 15;
                            //    m_BuzzerIndex++;
                            //}
                            //else
                            //{
                            //    _set_buzzer(false);
                            //    Process.Stop();
                            //    FireCompleted();
                            //}
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            //_set_buzzer(false);

                            //Process.NextDuriation = 500;
                            //Process.ID = 10;
                        }
                        break;
                }
            }
        }


        #region PRIVATE_THREAD_FUNCTIONS
        private Thread _thread = null;
        private bool _runFlag = false;
        private bool _isThreadStopping = false;

        protected bool is_thread_running()
        {
            return _runFlag || _thread != null;
        }
        protected void start_scan_thread()
        {
            if (!is_thread_running())
            {
                _runFlag = true;
                _thread = new Thread(thread_func);
                _thread.Name = this.Name;
                _thread.Start();
            }
            else
            {
                //GdxGlobal.LOG.Warn("有 Thread 尚未結束");
                CommonLogClass.Instance.LogError("有 Thread 尚未結束");
            }
        }
        protected void stop_scan_thread(int timeout = 3000)
        {
            if (is_thread_running())
            {
                _runFlag = false;
                var stopFunc = new Action<int>((tmout) =>
                {
                    if (!_isThreadStopping)
                    {
                        _isThreadStopping = true;
                        try
                        {
                            var t = _thread;
                            if (t != null)
                            {
                                if (!t.Join(tmout))
                                    t.Abort();
                                _thread = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            //GdxGlobal.LOG.Warn(ex, "Thread 終止異常!");
                            CommonLogClass.Instance.LogError("Thread 終止異常!");
                        }
                        _isThreadStopping = false;
                    }
                });
                stopFunc.BeginInvoke(timeout, null, null);
            }
        }
        private void thread_func(object arg)
        {
            //var phase = (XRunContext)arg;

            while (_runFlag)
            {
                try
                {
                    ////>>> 確保 PLC 有效 scanned 出現 2次 以上
                    //if (!IsValidPlcScanned(2))
                    //{
                    //    Thread.Sleep(2);
                    //    continue;
                    //}

                    //phase.StepFunc(phase);

                    //if (!phase.Go)
                    //    break;

                    //if (phase.IsCompleted)
                    //{
                    //    if (phase.RunCount == 0)
                    //        _LOG(phase.Name, "補償 = 0");
                    //    break;
                    //}
                }
                catch (Exception ex)
                {
                    if (_runFlag)
                    {
                        try
                        {
                            _LOG(ex, "live compensating 異常!");
                            SetNextState(9999);
                        }
                        catch
                        {
                        }
                    }
                    break;
                }
            }

            _runFlag = false;
            _thread = null;

            //int nextState = phase.ExitCode;
            //SetNextState(nextState);
            //base.IsOn = true;
        }
        #endregion

    }
}
