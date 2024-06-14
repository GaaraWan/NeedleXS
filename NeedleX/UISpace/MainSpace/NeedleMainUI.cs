using Common.RecipeSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.Interface;
using JzDisplay;
using JzDisplay.UISpace;
using NeedleX.FormSpace;
using NeedleX.ProcessSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.UI.WebControls;
using System.Windows.Forms;
using Traveller106;
using VsCommon.ControlSpace;
using VsCommon.ControlSpace.MachineSpace;

namespace NeedleX.UISpace.MainSpace
{
    public partial class NeedleMainUI : UserControl
    {
        //ICam CAM0
        //{
        //    get { return Universal.CAMERAS[0]; }
        //}

        protected MachineCollectionClass MACHINECollection
        {
            get
            {
                return Traveller106.Universal.MACHINECollection;
            }
        }
        protected NeedleMachineClass MACHINE
        {
            get { return (NeedleMachineClass)Traveller106.Universal.MACHINECollection.MACHINE; }
        }

        Button btnAutoFocus;
        Button btnStart;
        Button btnStop;
        Button btnReset;
        Button btnClearAlarm;
        Button btnMute;
        Button btnManualAuto;
        Button btnStabilize;

        Label lblAlarm;
        Label lblState;

        Label lblx;
        Label lbly;
        Label lblz;
        Button btnToAbs;
        Button btnToMicro;
        Button btnToEyes;
        DataViewUI JzViewer;
        Label lblpercent;
        ProgressBar progressBar;
        Button btnSaveData;

        

        bool[] m_plcCommError;

        int CamSelectIndex
        {
            get
            {
                int ret = -1;

                if (radioButton1.Checked)
                {
                    ret = 0;
                }
                else if (radioButton2.Checked)
                {
                    ret = 1;
                }
                else if (radioButton3.Checked)
                {
                    ret = 2;
                }
                else if (radioButton4.Checked)
                {
                    ret = -1;
                }

                return ret;
            }
        }


        public NeedleMainUI()
        {
            InitializeComponent();
        }
        public void Init()
        {
            init_Display();
            update_Display();
            CommonLogClass.Instance.SetRichTextBox(richTextBox1);

            btnAutoFocus = button2;
            btnAutoFocus.Click += BtnAutoFocus_Click;

            lblAlarm = label5;
            lblState = label11;

            btnManualAuto = button1;

            lblx = label13;
            lbly = label3;
            lblz = label7;
            btnToAbs = button11;
            btnToMicro = button10;
            btnToEyes = button5;
            JzViewer = dataViewUI1;
            lblpercent = label1;
            progressBar = progressBar1;
            btnSaveData = button12;

            btnStart = button6;
            btnStop = button4;
            btnReset = button7;
            btnStabilize = button3;

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            btnReset.Click += BtnReset_Click;
            btnStabilize.Click += BtnStabilize_Click;

            btnToAbs.Click += BtnToAbs_Click;
            btnToMicro.Click += BtnToMicro_Click;
            btnToEyes.Click += BtnToEyes_Click;
            btnSaveData.Click += BtnSaveData_Click;

            InitAllProcesses();
            StopAllProcesses("INIT");

            m_plcCommError = new bool[MACHINE.PLCCollection.Length];
            int i = 0;
            while (i < MACHINE.PLCCollection.Length)
            {
                m_plcCommError[i] = false;
                i++;
            }

            MACHINE.EVENT.Initial(lblAlarm);
            MACHINE.TriggerAction += MACHINE_TriggerAction;
            MACHINE.EVENT.TriggerAlarm += EVENT_TriggerAlarm;
            MACHINE.MachineCommErrorStringAction += MACHINE_MachineCommErrorStringAction;

            this.OnLiveImage += process_OnLiveImage;
            start_scan_thread();
        }
        public void Close()
        {
            stop_scan_thread();
        }

        private void BtnSaveData_Click(object sender, EventArgs e)
        {
            string str = SaveFilePicker("", DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
            if (string.IsNullOrEmpty(str))
                return;

            JzViewer.Invoke_SaveData(str);
        }

        private void BtnToEyes_Click(object sender, EventArgs e)
        {
            XRowEventArgs xRowEvent = JzViewer.GetCurrentData();
            if (xRowEvent == null)
                return;

            int rowindex = xRowEvent.Index;
            if (rowindex == -1)
                return;
            string msg = "To目镜至序号 " + rowindex;
            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            NeedleXYZ basexyz = new NeedleXYZ(xRowEvent.Adjust.ToString());
            NeedleXYZ relationxyz = new NeedleXYZ(INI.Instance.offsetCAM2CAM1);
            NeedleXYZ resultxyz = new NeedleXYZ(xRowEvent.Adjust.ToString());
            resultxyz.X = basexyz.X - relationxyz.X;
            resultxyz.Y = basexyz.Y - relationxyz.Y;
            resultxyz.Z = basexyz.Z - relationxyz.Z;

            NeedleXYZ relationxyz1 = new NeedleXYZ(INI.Instance.offsetCAM2Eyes);
            NeedleXYZ resultxyz1 = new NeedleXYZ(xRowEvent.Adjust.ToString());
            resultxyz1.X = resultxyz.X - relationxyz1.X;
            resultxyz1.Y = resultxyz.Y - relationxyz1.Y;
            resultxyz1.Z = resultxyz.Z - relationxyz1.Z;

            MACHINE.GoPosition(resultxyz1.ToString(), true);
        }

        private void BtnToMicro_Click(object sender, EventArgs e)
        {
            XRowEventArgs xRowEvent = JzViewer.GetCurrentData();
            if (xRowEvent == null)
                return;

            int rowindex = xRowEvent.Index;
            if (rowindex == -1)
                return;
            string msg = "To显微至序号 " + rowindex;
            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            NeedleXYZ basexyz = new NeedleXYZ(xRowEvent.Adjust.ToString());
            NeedleXYZ relationxyz = new NeedleXYZ(INI.Instance.offsetCAM2CAM1);
            NeedleXYZ resultxyz = new NeedleXYZ(xRowEvent.Adjust.ToString());
            resultxyz.X = basexyz.X - relationxyz.X;
            resultxyz.Y = basexyz.Y - relationxyz.Y;
            resultxyz.Z = basexyz.Z - relationxyz.Z;

            MACHINE.GoPosition(resultxyz.ToString(), true);
        }

        private void BtnToAbs_Click(object sender, EventArgs e)
        {
            XRowEventArgs xRowEvent = JzViewer.GetCurrentData();
            if (xRowEvent == null)
                return;

            int rowindex = xRowEvent.Index;
            if (rowindex == -1)
                return;
            string msg = "定位至序号 " + rowindex;
            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }
            MACHINE.GoPosition(xRowEvent.Adjust.ToString(), true);
        }

        private void BtnStabilize_Click(object sender, EventArgs e)
        {
            //判断是否在手动状态
            if (MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL && !Traveller106.Universal.IsNoUseIO)
            {
                VsMSG.Instance.Warning("手动模式下，无法启动，请检查。");
                return;
            }

            //判斷是否開啓真空
            if (!MACHINE.PLCIO.ADR_ISVACC && !Traveller106.Universal.IsNoUseIO)
            {
                VsMSG.Instance.Warning("真空未打開，無法啓動。");
                return;
            }

            string onStrMsg = "是否要启动稳定性测试？";
            string offStrMsg = "是否要停止稳定性测试？";
            string msg = (m_stablizeprocess.IsOn ? offStrMsg : onStrMsg);
            if (m_stablizeprocess.IsOn)
                if (VsMSG.Instance.Question(msg) != DialogResult.OK)
                    return;

            if (true)
            {
                if (!m_stablizeprocess.IsOn)
                {
                    m_stablizeprocess.Start();
                }
                else
                {
                    StopAllProcesses("USERSTOP");
                }
            }
        }

        private void MACHINE_MachineCommErrorStringAction(string str)
        {
            //輸出那個plc掉綫
            int index = 0;
            string _plcIndex = str.Replace("PLC", "");
            bool bOK = int.TryParse(_plcIndex, out index);
            string _errorStr = "plc通訊中斷!!!\r\n(編號Index=" + index.ToString() + ")\r\n是否重連?";
            //先停掉流程
            StopAllProcesses("PlcCommError");
            if (!m_plcCommError[index])
            {
                m_plcCommError[index] = true;
                if (VsMSG.Instance.Question(_errorStr) == DialogResult.OK)
                {
                    //重連
                    MACHINE.PLCCollection[index].RetryConn();
                    m_plcCommError[index] = false;
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }
        private void EVENT_TriggerAlarm(bool IsBuzzer)
        {
            //if (Universal.IsSilentMode)
            //    MACHINE.PLCIO.ADR_BUZZER = false;
            //else
            //    MACHINE.PLCIO.ADR_BUZZER = IsBuzzer;

            //if (!IsBuzzer)
            //{
            //    SetNormalLight();
            //}
        }

        bool IsEMCTriggered = false;
        bool IsSCREENTriggered = false;

        private void MACHINE_TriggerAction(MachineEventEnum machineevent)
        {
            switch (machineevent)
            {
                case MachineEventEnum.ALARM_SERIOUS:
                    //IsAlarmsSeriousX = true;
                    //SetAbnormalLight();
                    break;
                case MachineEventEnum.ALARM_COMMON:
                    //IsAlarmsCommonX = true;
                    //SetAbnormalLight();
                    break;
                case MachineEventEnum.EMC:
                    IsEMCTriggered = true;
                    break;
                case MachineEventEnum.CURTAIN:
                    IsSCREENTriggered = true;
                    break;
            }
        }

        BaseProcess m_BuzzerProcess
        {
            get { return BuzzerProcess.Instance; }
        }
        BaseProcess m_resetprocess
        {
            get { return ResetProcess.Instance; }
        }
        BaseProcess m_mainprocess
        {
            get { return MainProcess.Instance; }
        }
        BaseProcess m_focusprocess
        {
            get { return FocusProcess.Instance; }
        }
        BaseProcess m_stablizeprocess
        {
            get { return StabilizeProcess.Instance; }
        }

        void StopAllProcesses(string reason = "")
        {
            m_resetprocess.Stop();
            m_mainprocess.Stop();
            m_focusprocess.Stop();
            m_stablizeprocess.Stop();

            switch (reason)
            {
                case "INIT":
                    break;
                case "USERSTOP":
                    m_BuzzerProcess.Stop();
                    break;
                default:
                    break;
            }
        }
        void InitAllProcesses()
        {
            //----------------------------------------------------------------
            // (1) 大部的 Processes 應該可以當成 MainProcess 的 Child Process,
            //      可以集中由 MainProcess 管理, 形成一體 Model.
            // (2) 以下對 Process Event Handler 的掛載.
            //      在 Model-View-Control 的架構規範下, 屬於 Control.
            //      ~ 以後再從 GUI(MainGdx3UI) 抽離出來.
            //----------------------------------------------------------------
            //m_mainprocess.OnCompleted += process_OnCompleted;
            // Buzzer 的結束 用來檢視是否有 NG 發生.
            m_BuzzerProcess.OnCompleted += buzzer_OnCompleted;
            m_resetprocess.OnCompleted += process_OnCompleted;
            m_mainprocess.OnCompleted += process_OnCompleted;
            m_focusprocess.OnCompleted += process_OnCompleted;
            m_stablizeprocess.OnCompleted += process_OnCompleted;

            ((MainProcess)m_mainprocess).OnLiveImage += process_OnLiveImage;
            ((FocusProcess)m_focusprocess).OnLiveImage += process_OnLiveImage;
            m_stablizeprocess.OnLiveImage += process_OnLiveImage;

            m_mainprocess.OnMessage += handle_main_process_completed;
        }

        //private void M_mainprocess_OnMessage(object sender, ProcessEventArgs e)
        //{
        //    if (InvokeRequired)
        //    {
        //        EventHandler<ProcessEventArgs> h = M_mainprocess_OnMessage;
        //        BeginInvoke(h, sender, e);
        //    }
        //    else
        //    {
        //        if (e.Message.IndexOf("FocusCompleted") > -1)
        //        {
        //            if (e.Tag != null)
        //            {
        //                XRowEventArgs xRow = e.Tag as XRowEventArgs;
        //                JzViewer.Invoke_AddRow(xRow);

        //                int myCount = int.Parse(e.Message.Split('.')[1]);
        //                progressBar.Maximum = myCount;
        //                progressBar.Value = xRow.Index;
        //                lblpercent.Text = (xRow.Index * 1.0 / myCount * 100).ToString("0.00") + " %";
        //            }
        //        }
        //        else if (e.Message.IndexOf("ResetData") > -1)
        //        {
        //            JzViewer.Invoke_ClrTable();
        //            progressBar.Value = 0;
        //        }
        //    }
        //}

        void TickAllProcesses()
        {
            m_BuzzerProcess.Tick();
            m_resetprocess.Tick();
            m_mainprocess.Tick();
            m_focusprocess.Tick();
            m_stablizeprocess.Tick();
        }


        private void process_OnCompleted(object sender, ProcessEventArgs e)
        {
            if (sender == m_resetprocess)
            {
                if (m_resetprocess.RelateString == "CloseWindows")
                {
                    //執行的關閉流程 這裏則跳出
                    return;
                }
            }

            try
            {
                //if (sender == m_mainprocess)
                //{
                //    handle_main_process_completed(sender, e);
                //    if (e.Message == "PartialCompleted")
                //        return;
                //}

                // Do whatever message you want to show to the operators.
                string msg = $"程序 {((BaseProcess)sender).Name}, 已完成!\n";
                CommonLogClass.Instance.LogMessage(msg, Color.Black);
            }
            catch
            {
            }
        }
        private void buzzer_OnCompleted(object sender, ProcessEventArgs e)
        {
            if (InvokeRequired)
            {
                EventHandler<ProcessEventArgs> h = buzzer_OnCompleted;
                BeginInvoke(h, sender, e);
            }
            else
            {
                //if (m_mainprocess.LastNG != null)
                //{
                //    //>>> MessageBox.Show(m_mainprocess.LastNG);
                //    var errMsg = m_mainprocess.LastNG + "\n\r\n\r(後續可按 復位 排除NG態)";
                //    VsMSG.Instance.Warning(errMsg);
                //}
            }
        }
        private void handle_main_process_completed(object sender, ProcessEventArgs e)
        {
            if (InvokeRequired)
            {
                EventHandler<ProcessEventArgs> h = handle_main_process_completed;
                BeginInvoke(h, sender, e);
            }
            else
            {
                if (e.Message.IndexOf("FocusCompleted") > -1)
                {
                    if (e.Tag != null)
                    {
                        XRowEventArgs xRow = e.Tag as XRowEventArgs;
                        JzViewer.Invoke_AddRow(xRow);

                        int myCount = int.Parse(e.Message.Split('.')[1]);
                        progressBar.Maximum = myCount;
                        progressBar.Value = xRow.Index;
                        lblpercent.Text = (xRow.Index * 1.0 / myCount * 100).ToString("0.00") + " %";
                    }
                }
                else if (e.Message.IndexOf("ResetData") > -1)
                {
                    JzViewer.Invoke_ClrTable();
                    progressBar.Value = 0;
                }
            }
        }
        private void process_OnLiveImage(object sender, ProcessEventArgs e)
        {
            if (e.Tag != null && e.Tag is Bitmap)
            {
                try
                {
                    if (InvokeRequired)
                    {
                        EventHandler<ProcessEventArgs> h = process_OnLiveImage;
                        this.Invoke(h, sender, e);
                    }
                    else
                    {
                        //@LETIAN: 2022/07/01 改用 GdxDispUI 增加一些 fps
                        // bmp 由 Sender maintains life cycle.
                        // 在此不用 Dispose
                        Bitmap bmp = (Bitmap)e.Tag;
                        DS1.ReplaceDisplayImage(bmp);
                    }
                }
                catch (Exception ex)
                {
                    //>>> 此一層的 try - catch 以後可以省略.
                    //>>> 會由 Event Sender 處理 exception
                    throw ex;
                }
            }
        }


        private void BtnReset_Click(object sender, EventArgs e)
        {
            string onStrMsg = "是否要进行复位？";
            string offStrMsg = "是否要停止复位流程？";
            string msg = (m_resetprocess.IsOn ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) == DialogResult.OK)
            {
                if (!m_resetprocess.IsOn)
                {
                    m_resetprocess.Start();
                }
                else
                {
                    m_resetprocess.Stop();
                }
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            string onStrMsg = "是否停止？";
            string offStrMsg = "是否停止？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) == DialogResult.OK)
            {
                StopAllProcesses("USERSTOP");
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            //判断是否在手动状态
            if (MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL && !Traveller106.Universal.IsNoUseIO)
            {
                VsMSG.Instance.Warning("手动模式下，无法启动，请检查。");
                return;
            }

            //判斷是否開啓真空
            if (!MACHINE.PLCIO.ADR_ISVACC && !Traveller106.Universal.IsNoUseIO)
            {
                VsMSG.Instance.Warning("真空未打開，無法啓動。");
                return;
            }

            string onStrMsg = "是否要启动？";
            string offStrMsg = "是否要停止？";
            string msg = (m_mainprocess.IsOn ? offStrMsg : onStrMsg);
            if (m_mainprocess.IsOn)
                if (VsMSG.Instance.Question(msg) != DialogResult.OK)
                    return;

            if (true)
            {
                if (!m_mainprocess.IsOn)
                {
                    m_mainprocess.Start();
                }
                else
                {
                    StopAllProcesses("USERSTOP");
                }
            }
        }

        frmAutoFocus FrmAutoFocus = null;
        private void BtnAutoFocus_Click(object sender, EventArgs e)
        {
            if (!INI.Instance.IsOpenFocusWindows)
            {
                INI.Instance.IsOpenFocusWindows = true;
                FrmAutoFocus = new frmAutoFocus();
                FrmAutoFocus.Show();
            }
        }

        public void Tick()
        {
            //CAM0.Snap();
            //DS1.SetDisplayImage(CAM0.GetSnap());

            btnReset.BackColor = (m_resetprocess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));
            btnManualAuto.BackColor = (!MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL ? Color.Green : Color.FromArgb(192, 255, 192));
            btnManualAuto.Text = (!MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL ? "自动" : "手动");
            btnStart.BackColor = (m_mainprocess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));
            btnStabilize.BackColor = (m_stablizeprocess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));


            TickAllProcesses();
            AlarmUITick();
        }
        public void SetEnable(bool isendable)
        {
            //pnlButtons.Enabled = isendable;
        }


        private void AlarmUITick()
        {

            #region ALARM

            //if (IsAlarmsSeriousX)
            //{
            //    SetAbnormalLight();

            //    IsAlarmsSeriousX = false;
            //    StopAllProcesses("ALM.S");
            //    //SetSeriousAlarms0();
            //    //SetSeriousAlarms1();

            //    //StopAllProcess();
            //}

            //if (IsAlarmsCommonX)
            //{
            //    SetAbnormalLight();

            //    IsAlarmsCommonX = false;
            //    StopAllProcesses("ALM.C");
            //    //SetCommonAlarms();

            //}

            if (!Traveller106.Universal.IsNoUseIO)
            {
                if (IsEMCTriggered)
                {
                    //SetAbnormalLight();

                    IsEMCTriggered = false;
                    StopAllProcesses("EMC");
                    //OnTrigger(ActionEnum.ACT_ISEMC, "");
                }
                if (IsSCREENTriggered)
                {
                    //SetAbnormalLight();

                    //IsSCREENTriggered = false;
                    //if (!MACHINE.PLCIO.ADR_BYPASS_SCREEN)
                    //    StopAllProcesses("SCREEN");
                    //OnTrigger(ActionEnum.ACT_ISEMC, "");
                }
            }

            #endregion

            UpdateStateUI();
            
        }
        private void UpdateStateUI()
        {
            string ngMsg = null;

            lblx.Text = MACHINE.PLCMOTIONCollection[0].PositionNowString;
            lbly.Text = MACHINE.PLCMOTIONCollection[1].PositionNowString;
            lblz.Text = MACHINE.PLCMOTIONCollection[2].PositionNowString;

            //if (m_TestProcess.IsOn)
            //    lblState.Text = "执行-测试区取像中 " + m_TestProcess.ID.ToString();
            //if (MACHINE.PLCIO.IntAlarmsCommon != 0 || MACHINE.PLCIO.IntAlarmsSerious != 0)
            //{
            //    lblState.Text = $"报警中 [{MACHINE.PLCIO.IntAlarmsCommon}]" +
            //        $" [{MACHINE.PLCIO.IntAlarmsSerious}]";
            //    if (MACHINE.IsClearAlarmCache)
            //        return;
            //    if (MACHINE.PLCIO.IntAlarmsCommon != 0)
            //    {
            //        SetCommonAlarms();
            //    }
            //    if (MACHINE.PLCIO.IntAlarmsSerious != 0)
            //    {
            //        //lblState.Text = "严重报警中";
            //        SetSeriousAlarms1();
            //    }
            //}
            if (m_stablizeprocess.IsOn)
                lblState.Text = "稳定性测试中 " + m_stablizeprocess.ID.ToString();
            else if (m_focusprocess.IsOn)
                lblState.Text = "自动对焦中 " + m_focusprocess.ID.ToString();
            else if (m_mainprocess.IsOn)
                lblState.Text = "跑线中 " + m_mainprocess.ID.ToString();
            else if (m_resetprocess.IsOn)
                lblState.Text = "复位中 " + m_resetprocess.ID.ToString();
            else
                lblState.Text = "待机";

            if (MACHINE.PLCIO.ADR_ISEMC)
            {
                lblState.Text = "急停中";
                lblState.BackColor = Color.Red;
            }
            //else if (MACHINE.PLCIO.ADR_ISSCREEN)
            //{
            //    if (!MACHINE.PLCIO.ADR_BYPASS_SCREEN)
            //    {
            //        lblState.Text = "光幕遮挡";
            //        lblState.BackColor = Color.Red;
            //    }
            //}
            else
            {
                if (ngMsg != null)
                {
                    lblState.Text = ngMsg; // "NG: " + lblState.Text;
                    lblState.BackColor = Color.Red;
                }
                else
                {
                    lblState.BackColor = Color.Black;
                }
            }

            lblState.Text = LanguageExClass.Instance.ToTraditionalChinese(lblState.Text);

            //_updateProductionRunTime();
            //_updateButtonsStatus();
        }
        void init_Display()
        {
            DS1.Initial(100, 0.01f);
            DS1.SetDisplayType(DisplayTypeEnum.SHOW);
            //DS2.Initial(100, 0.01f);
            //DS2.SetDisplayType(DisplayTypeEnum.SHOW);
            //DS3.Initial(100, 0.01f);
            //DS3.SetDisplayType(DisplayTypeEnum.SHOW);
        }
        void update_Display()
        {
            DS1.Refresh();
            DS1.DefaultView();
            //DS2.Refresh();
            //DS2.DefaultView();
            //DS3.Refresh();
            //DS3.DefaultView();
        }
        string SaveFilePicker(string DefaultPath, string DefaultName)
        {
            string retStr = "";

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "CSV Files (*.csv)|*.CSV|" + "All files (*.*)|*.*";
            //dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }

        public event EventHandler<ProcessEventArgs> OnLiveImage;
        protected void FireLiveImaging(Bitmap bmp)
        {
            try
            {
                OnLiveImage?.Invoke(this, new ProcessEventArgs("live.image", bmp));
            }
            catch (Exception ex)
            {
                //_LOG(ex, "FireLiveImaging 異常!");
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

                    if (CamSelectIndex < 0)
                    {
                        Thread.Sleep(2);
                        continue;
                    }
                       
                    int index = CamSelectIndex;
                    //if (IsNeedToChange)
                    //    return;
                    using (Bitmap bmp = snapshot_image(GetCamera(index)))
                    {
                        //bmpOperate[index].Dispose();
                        //bmpOperate[index] = new Bitmap(bmp);
                        //DS.ReplaceDisplayImage(bmpOperate[index]);
                        FireLiveImaging(bmp);
                    }

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
                            //_LOG(ex, "live compensating 異常!");
                            //SetNextState(9999);
                            CommonLogClass.Instance.LogError("相机异常");
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
        protected ICam GetCamera(int camID)
        {
            return Traveller106.Universal.CAMERAS[camID];
        }
        protected Bitmap snapshot_image(ICam cam, string tag = null, bool dump = true)
        {
            try
            {
                cam.Snap();
                Bitmap bmp = cam.GetSnap();
                return bmp;
            }
            catch (Exception ex)
            {
                //_LOG(ex, "相機異常!");
                return new Bitmap(1, 1);
            }
        }
    }
}
