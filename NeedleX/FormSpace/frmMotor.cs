using Common;
using Common.RecipeSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace.MotionSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Traveller106.ControlSpace.MachineSpace;
using VsCommon.ControlSpace;
using VsCommon.ControlSpace.MachineSpace;

namespace Traveller106.FormSpace
{
    public partial class frmMotor : Form
    {
        const int AXIS_COUNT = 3;
        VsTouchMotorUI[] VSAXISUI = new VsTouchMotorUI[AXIS_COUNT];
        MotionTouchPanelUIClass[] AXISUI = new MotionTouchPanelUIClass[AXIS_COUNT];

        Timer mMotorTimer = null;
        Button btnLineScanTest;
        Button btnLineScanOnce;

        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }

        NeedleMachineClass MACHINE
        {
            get { return (NeedleMachineClass)MACHINECollection.MACHINE; }
        }
        bool IsZToGo
        {
            get { return chkOpenZGo.Checked; }
        }
        public frmMotor()
        {
            InitializeComponent();

            //this.TopMost = true;

            this.Load += FrmMotor_Load;
            this.FormClosed += FrmMotor_FormClosed;
        }

        private void FrmMotor_FormClosed(object sender, FormClosedEventArgs e)
        {
            INI.Instance.IsOpenMotorWindows = false;
        }

        private void FrmMotor_Load(object sender, EventArgs e)
        {
            this.Text = "轴设定视窗";
            Init();
        }

        void Init()
        {
            #region 位置设定控件

            //tabPage1.Text = "AXIS-XYZ";
            //tabPage2.Text = "轨道一 (AXIS 3-4)";
            //tabPage3.Text = "轨道二 (AXIS 5-6)";
            //tabPage4.Text = "轨道三 (AXIS 7-8)";
            //tabPage5.Text = "轨道四 (AXIS 9-10)";
            //tabPage6.Text = "线扫x轴";

            VSAXISUI[0] = vsTouchMotorUI3;
            VSAXISUI[1] = vsTouchMotorUI2;
            VSAXISUI[2] = vsTouchMotorUI1;

            //VSAXISUI[3] = vsTouchMotorUI6;
            //VSAXISUI[4] = vsTouchMotorUI5;

            //VSAXISUI[5] = vsTouchMotorUI8;
            //VSAXISUI[6] = vsTouchMotorUI7;

            //VSAXISUI[7] = vsTouchMotorUI10;
            //VSAXISUI[8] = vsTouchMotorUI9;

            //VSAXISUI[9] = vsTouchMotorUI12;
            //VSAXISUI[10] = vsTouchMotorUI11;

            //VSAXISUI[11] = vsTouchMotorUI4;

            //VSAXISUI[11] = vsTouchMotorUI11;
            //VSAXISUI[12] = vsTouchMotorUI14;
            //VSAXISUI[13] = vsTouchMotorUI13;

            int i = 0;
            while (i < AXIS_COUNT)
            {
                AXISUI[i] = new MotionTouchPanelUIClass(VSAXISUI[i]);

                switch (i)
                {
                    case 0:
                    case 2:
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                    case 11:
                    case 12:
                        AXISUI[i].Initial(MACHINE.PLCMOTIONCollection[i], false);
                        break;
                    default:
                        AXISUI[i].Initial(MACHINE.PLCMOTIONCollection[i]);
                        break;
                }


                i++;
            }

            #endregion

            mMotorTimer = new Timer();
            mMotorTimer.Interval = 50;
            mMotorTimer.Enabled = true;
            mMotorTimer.Tick += MMotorTimer_Tick;

            MACHINE.TriggerAction += MACHINE_TriggerAction;

            btnset1.Click += Btnset1_Click;
            btnset2.Click += Btnset2_Click;
            btnset3.Click += Btnset3_Click;
            btnset1_0.Click += Btnset1_0_Click;
            btnset4.Click += Btnset4_Click;

            btngo1.Click += Btngo1_Click;
            btngo2.Click += Btngo2_Click;
            btngo3.Click += Btngo3_Click;
            btngo1_0.Click += Btngo1_0_Click;
            btngo4.Click += Btngo4_Click;

            btnReSetup.Click += BtnReSetup_Click;

            FillDisplay();
        }

        private void BtnReSetup_Click(object sender, EventArgs e)
        {
            NeedleXYZ cam0 = new NeedleXYZ(INI.Instance.Cam0CenterPos);
            NeedleXYZ cam1 = new NeedleXYZ(INI.Instance.Cam1CenterPos);
            NeedleXYZ cam2 = new NeedleXYZ(INI.Instance.Cam2CenterPos);
            NeedleXYZ cam1_0 = new NeedleXYZ(INI.Instance.Cam1CenterPos0);
            NeedleXYZ cam2Eyes = new NeedleXYZ(INI.Instance.EyesCenterPos);

            NeedleXYZ offcam0cam1 = new NeedleXYZ(INI.Instance.offsetCAM0CAM1);
            NeedleXYZ offcam2cam1 = new NeedleXYZ(INI.Instance.offsetCAM2CAM1);
            NeedleXYZ offcam2eyes = new NeedleXYZ(INI.Instance.offsetCAM2Eyes);

            offcam0cam1.X = Math.Round(cam0.X - cam1_0.X, 6);
            offcam0cam1.Y = Math.Round(cam0.Y - cam1_0.Y, 6);
            offcam0cam1.Z = Math.Round(cam0.Z - cam1_0.Z, 6);

            offcam2cam1.X = Math.Round(cam1.X - cam2.X, 6);
            offcam2cam1.Y = Math.Round(cam1.Y - cam2.Y, 6);
            offcam2cam1.Z = Math.Round(cam1.Z - cam2.Z, 6);

            //offcam2cam1.X = Math.Round(cam2.X - cam1.X, 6);
            //offcam2cam1.Y = Math.Round(cam2.Y - cam1.Y, 6);
            //offcam2cam1.Z = Math.Round(cam2.Z - cam1.Z, 6);

            offcam2eyes.X = Math.Round(cam2.X - cam2Eyes.X, 6);
            offcam2eyes.Y = Math.Round(cam2.Y - cam2Eyes.Y, 6);
            offcam2eyes.Z = Math.Round(cam2.Z - cam2Eyes.Z, 6);

            INI.Instance.offsetCAM0CAM1 = offcam0cam1.ToString();
            INI.Instance.offsetCAM2CAM1 = offcam2cam1.ToString();
            INI.Instance.offsetCAM2Eyes = offcam2eyes.ToString();

            INI.Instance.Save();
        }
        private void Btngo4_Click(object sender, EventArgs e)
        {
            string msg = "是否需要定位，请确定安全后点击确定。";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }
            MACHINE.GoPosition(INI.Instance.EyesCenterPos, IsZToGo);
        }

        private void Btngo3_Click(object sender, EventArgs e)
        {
            string msg = "是否需要定位，请确定安全后点击确定。";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }
            MACHINE.GoPosition(INI.Instance.Cam2CenterPos, IsZToGo);
        }

        private void Btngo2_Click(object sender, EventArgs e)
        {
            string msg = "是否需要定位，请确定安全后点击确定。";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }
            MACHINE.GoPosition(INI.Instance.Cam1CenterPos, IsZToGo);
        }

        private void Btngo1_Click(object sender, EventArgs e)
        {
            string msg = "是否需要定位，请确定安全后点击确定。";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }
            MACHINE.GoPosition(INI.Instance.Cam0CenterPos, IsZToGo);
        }
        private void Btngo1_0_Click(object sender, EventArgs e)
        {
            string msg = "是否需要定位，请确定安全后点击确定。";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }
            MACHINE.GoPosition(INI.Instance.Cam1CenterPos0, IsZToGo);
        }


        private void Btnset4_Click(object sender, EventArgs e)
        {
            INI.Instance.EyesCenterPos = MACHINECollection.GetPosition();
            FillDisplay();
        }
        private void Btnset3_Click(object sender, EventArgs e)
        {
            INI.Instance.Cam2CenterPos = MACHINECollection.GetPosition();
            FillDisplay();
        }

        private void Btnset2_Click(object sender, EventArgs e)
        {
            INI.Instance.Cam1CenterPos = MACHINECollection.GetPosition();
            FillDisplay();
        }

        private void Btnset1_Click(object sender, EventArgs e)
        {
            INI.Instance.Cam0CenterPos = MACHINECollection.GetPosition();
            FillDisplay();
        }
        private void Btnset1_0_Click(object sender, EventArgs e)
        {
            INI.Instance.Cam1CenterPos0 = MACHINECollection.GetPosition();
            FillDisplay();
        }

        private void BtnLineScanOnce_Click(object sender, EventArgs e)
        {
            //MACHINE.PLCIO.TestLineScanOnceSpeed = (int)numericUpDown3.Value;
            //MACHINE.PLCIO.TestLineScanOnce = true;
        }

        private void BtnLineScanTest_Click(object sender, EventArgs e)
        {
            if (!m_LineScanTestProcess.IsOn)
                m_LineScanTestProcess.Start();
            else
                m_LineScanTestProcess.Stop();
        }

        bool IsEMCTriggered = false;

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
            }
        }

        private void MMotorTimer_Tick(object sender, EventArgs e)
        {
            if (!Universal.IsNoUseIO)
            {
                if (IsEMCTriggered)
                {
                    //SetAbnormalLight();

                    IsEMCTriggered = false;
                    //StopAllProcess();
                    //OnTrigger(ActionEnum.ACT_ISEMC, "");
                }
            }

            //btnManualAuto.BackColor = (MACHINE.PLCIO.GetMWIndex(IOConstClass.MW1090) == 1 ? Color.Red : Color.Lime);
            //btnManualAuto.Text = (MACHINE.PLCIO.GetMWIndex(IOConstClass.MW1090) == 1 ? "自動模式" : "手動模式");
            //btnChangeValveOneKey.BackColor = (MACHINE.PLCIO.ADR_CHANHEVALVE_ONEKEYING ? Color.Red : Color.Lime);
            //btnChangeValveOneKey.Text = (MACHINE.PLCIO.ADR_CHANHEVALVE_ONEKEYING ? "一鍵換閥中" : "一鍵換閥");

            //btnBypassDoor.BackColor = (MACHINE.PLCIO.ADR_BYPASS_DOOR ? Color.Red : Color.Lime);
            //btnBypassScreen.BackColor = (MACHINE.PLCIO.ADR_BYPASS_SCREEN ? Color.Red : Color.Lime);

            btnToFocus.BackColor = (MACHINE.PLCIO.ADR_TOFOCUS ? Color.Red : Color.Transparent);

            int i = 0;
            while (i < AXIS_COUNT)
            {
                AXISUI[i].Tick();
                i++;
            }

            //_LineScanTestTick();
            //btnLineScanTest.BackColor = (m_LineScanTestProcess.IsOn ? Color.Red : Control.DefaultBackColor);
        }

        float m_distance_1 = 0;
        float m_distance_2 = 0;
        PLCMotionClass AXIS_MODULE2
        {
            get { return MACHINE.PLCMOTIONCollection[5]; }
        }

        ProcessClass m_LineScanTestProcess = new ProcessClass();
        /// <summary>
        /// 线扫测试程序
        /// </summary>
        void _LineScanTestTick()
        {
            ProcessClass Process = m_LineScanTestProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        //m_distance_1 = (float)numericUpDown1.Value;
                        //m_distance_2 = (float)numericUpDown2.Value;
                       
                        //CommonLogClass.Instance.LogMessage("", Color.Black);
                        Process.NextDuriation = 500;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            MACHINE.PLCIO.SetQXQB("0:" + "QX0.6", true);

                            AXIS_MODULE2.Go(m_distance_1);

                            Process.NextDuriation = 500;
                            Process.ID = 20;
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (AXIS_MODULE2.IsOnSite)
                            {
                                AXIS_MODULE2.Go(m_distance_2);

                                Process.NextDuriation = 500;
                                Process.ID = 30;
                            }
                        }
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            if (AXIS_MODULE2.IsOnSite)
                            {
                                MACHINE.PLCIO.SetQXQB("0:" + "QX0.6", false);
                                AXIS_MODULE2.Go(m_distance_1);

                                Process.NextDuriation = 500;
                                Process.ID = 40;
                            }
                        }
                        break;
                    case 40:
                        if (Process.IsTimeup)
                        {
                            if (AXIS_MODULE2.IsOnSite)
                            {
                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        void FillDisplay()
        {
            txtPos1.Text = INI.Instance.Cam0CenterPos;
            txtPos2.Text = INI.Instance.Cam1CenterPos;
            txtPos3.Text = INI.Instance.Cam2CenterPos;
            txtPos1_0.Text = INI.Instance.Cam1CenterPos0;
            txtPos4.Text = INI.Instance.EyesCenterPos;
        }

        private void btnToFocus_Click(object sender, EventArgs e)
        {
            MACHINE.PLCIO.ADR_TOFOCUS = true;
        }
    }
}
