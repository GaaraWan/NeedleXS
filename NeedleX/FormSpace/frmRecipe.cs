using AHBlobPro;
using Common.RecipeSpace;
using Eazy_Project_III.OPSpace;
using JetEazy.BasicSpace;
using JetEazy.Interface;
using JzDisplay;
using JzDisplay.OPSpace;
using JzDisplay.UISpace;
using MoveGraphLibrary;
using NeedleX.ProcessSpace;
using NeedleX.UISpace.UIMVC.Controler;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Traveller106;
using Traveller106.FormSpace;
using VsCommon.ControlSpace.MachineSpace;
using WorldOfMoveableObjects;

namespace NeedleX.FormSpace
{
    public partial class frmRecipe : Form
    {
        #region DEFINE WINDOWS MEMBERS

        const string m_format = "0.000000";

        protected NeedleMachineClass MACHINE
        {
            get { return (NeedleMachineClass)Universal.MACHINECollection.MACHINE; }
        }

        CtlPosClass[] ctlPosClasses = new CtlPosClass[2];
        Bitmap[] bmpOperate;
        Timer mTimer = null;
        Mover myStaticMovers = new Mover();

        bool IsNeedToChange = false;
        bool IsAlignRunning = false;
        AlignImageCenterClass alignImageCenterClass = null;

        Button btnOK;
        Button btnCancel;
        Button btnHandAxis;
        ComboBox cboCamList;
        NumericUpDown numExpo;
        NumericUpDown numGain;
        NumericUpDown numLed;
        Button btnSaveImage;
        Button btnCamOneshot;
        Button btnCamContinue;
        Label lblCamName;
        Button btnControlWriteNow;
        Button btnClearData;
        Button btnOutputData;
        ListBox lstCollectData;
        Button btnControlAxis;
        Button btnGoFocusPos;
        Button btnAlignImage;

        Label lblAlignMsg;

        #endregion

        public frmRecipe()
        {
            InitializeComponent();
            this.Load += FrmRecipe_Load;
            this.FormClosed += FrmRecipe_FormClosed;
        }

        private void FrmRecipe_FormClosed(object sender, FormClosedEventArgs e)
        {
            mTimer.Stop();

        }

        #region WINDOWS EVENTS

        private void FrmRecipe_Load(object sender, EventArgs e)
        {
            bmpOperate = new Bitmap[CameraConfig.Instance.COUNT];
            for (int i = 0; i < CameraConfig.Instance.COUNT; i++)
            {
                bmpOperate[i] = new Bitmap(1, 1);
            }

            init_Display();
            update_Display();

            ctlPosClasses[0] = new CtlPosClass(posUI1, dxfViewer1);
            ctlPosClasses[1] = new CtlPosClass(posUI2, dxfViewer1);

            btnOK = button1;
            btnCancel = button2;
            btnHandAxis = button4;
            cboCamList = comboBox1;
            numExpo = numericUpDown1;
            numGain = numericUpDown2;
            numLed = numericUpDown3;
            btnCamOneshot = button5;
            btnCamContinue = button6;
            lblCamName = label4;
            btnSaveImage = button3;
            btnControlWriteNow = button7;
            lstCollectData = listBox1;
            btnClearData = button8;
            btnOutputData = button9;
            btnControlAxis = button10;
            btnGoFocusPos = button11;
            btnAlignImage = button12;
            lblAlignMsg = label7;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            btnHandAxis.Click += BtnHandAxis_Click;
            btnCamOneshot.Click += BtnCamOneshot_Click;
            btnCamContinue.Click += BtnCamContinue_Click;
            btnSaveImage.Click += BtnSaveImage_Click;
            btnControlWriteNow.Click += BtnControlWriteNow_Click;
            btnClearData.Click += BtnClearData_Click;
            btnOutputData.Click += BtnOutputData_Click;
            btnControlAxis.Click += BtnControlAxis_Click;
            btnGoFocusPos.Click += BtnGoFocusPos_Click;
            btnAlignImage.Click += BtnAlignImage_Click;

            numExpo.ValueChanged += NumExpo_ValueChanged;
            numGain.ValueChanged += NumGain_ValueChanged;
            numLed.ValueChanged += NumLed_ValueChanged;

            pgNormal.SelectedObject = RecipeNeedleClass.Instance;

            ctlPosClasses[0].Init(MACHINE, CoarsePositioningClass.Instance.sCoarsePosList, INI.Instance.offsetCAM2CAM1, INI.Instance.offsetCAM2Eyes);
            ctlPosClasses[1].Init(MACHINE, ModelPositioningClass.Instance.sModelPosList, INI.Instance.offsetCAM0CAM1, INI.Instance.offsetCAM2Eyes);
            ctlPosClasses[1].VisableControl(false);
            ctlPosClasses[0].OnMessage += FrmRecipe_OnMessage;

            alignImageCenterClass = new AlignImageCenterClass();
            lblAlignMsg.Text = "";
            init_CboList();
            writeDSControlPara();
            pgCamParaOther.PropertyValueChanged += PgCamParaOther_PropertyValueChanged;
            FillDisplay();
            FillDisplay(true);

            mTimer = new Timer();
            mTimer.Interval = 50;
            mTimer.Enabled = true;
            mTimer.Tick += MTimer_Tick;
        }

        private void BtnAlignImage_Click(object sender, EventArgs e)
        {
            IsAlignRunning = !IsAlignRunning;
        }

        private void BtnGoFocusPos_Click(object sender, EventArgs e)
        {
            if (cboCamList.SelectedIndex < 0)
                return;
            int index = cboCamList.SelectedIndex;
            goFocusPosition(index);
        }
        frmMotor mMotorFrom = null;
        private void BtnControlAxis_Click(object sender, EventArgs e)
        {
            if (!INI.Instance.IsOpenMotorWindows)
            {
                INI.Instance.IsOpenMotorWindows = true;
                mMotorFrom = new frmMotor();
                mMotorFrom.TopMost = true;
                mMotorFrom.Show();
            }
        }

        private void FrmRecipe_OnMessage(object sender, ProcessEventArgs e)
        {
            cboCamList.SelectedIndex = int.Parse(e.Message);
        }

        private void BtnOutputData_Click(object sender, EventArgs e)
        {
            if (lstCollectData.Items.Count > 0)
            {
                string str = string.Empty;
                for (int i = 0; i < lstCollectData.Items.Count; i++)
                {
                    str += lstCollectData.Items[i].ToString() + Environment.NewLine;
                }
                SaveData(str, "d:\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
            }
        }

        private void BtnClearData_Click(object sender, EventArgs e)
        {
            lstCollectData.Items.Clear();
        }

        private void BtnControlWriteNow_Click(object sender, EventArgs e)
        {
            writeDSControlPara();
        }

        private void PgCamParaOther_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            mTimer.Enabled = false;
            btnCamContinue.BackColor = Color.FromArgb(192, 255, 192);
        }

        private void NumLed_ValueChanged(object sender, EventArgs e)
        {
            if (cboCamList.SelectedIndex < 0)
                return;

            int index = cboCamList.SelectedIndex;
            RecipeNeedleClass.Instance.SetCamLedValue(index, (float)numLed.Value);
            MACHINE.PLCIO.SetLedValue(index, (int)numLed.Value);
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            if (cboCamList.SelectedIndex < 0)
                return;
            int index = cboCamList.SelectedIndex;
            string _path = JzToolsClass.SaveFilePicker("BMP Files(*.bmp) | *.BMP | " + "All files(*.*) | *.* ", DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp");

            if (!string.IsNullOrEmpty(_path))
            {
                bmpOperate[index].Save(_path, System.Drawing.Imaging.ImageFormat.Bmp);
            }

        }

        private void MTimer_Tick(object sender, EventArgs e)
        {
            btnAlignImage.BackColor = (IsAlignRunning ? Color.Red : Color.FromArgb(192, 255, 192));
            //lblAlignMsg.Text = (IsAlignRunning ? "" : "未开启对位");
            updateLabelMsg((IsAlignRunning ? "" : "未开启对位"));
            if (cboCamList.SelectedIndex < 0)
                return;
            int index = cboCamList.SelectedIndex;
            //if (IsNeedToChange)
            //    return;
            using (Bitmap bmp = snapshot_image(GetCamera(index)))
            {
                bmpOperate[index].Dispose();
                bmpOperate[index] = new Bitmap(bmp);

                if (IsAlignRunning)
                {
                    alignImageCenterClass.bmpInput = bmp;
                    alignImageCenterClass.Run();

                    string _msg = string.Empty;
                    int iret = alignImageCenterClass.IsCheckMove();
                    if (iret == -1)
                    {
                        _msg = $"!未发现定位点";
                    }
                    else
                    {
                        _msg = $"{(iret != 0 ? "!需要移动" : "对位正确")} 距离误差:{alignImageCenterClass.Distance.ToString("0.000000")}";
                        _msg += $" 移动X:{alignImageCenterClass.MotorOffset.X.ToString("0.000000")} 移动Y:{alignImageCenterClass.MotorOffset.Y.ToString("0.000000")}";

                    }

                    updateLabelMsg(_msg);
                    DS.ReplaceDisplayImage(alignImageCenterClass.bmpResult);
                }
                else
                    DS.ReplaceDisplayImage(bmpOperate[index]);

            }
            btnCamContinue.BackColor = Color.Red;

        }

        private void BtnCamContinue_Click(object sender, EventArgs e)
        {
            mTimer.Enabled = !mTimer.Enabled;
            btnCamContinue.BackColor = (mTimer.Enabled ? Color.Red : Color.FromArgb(192, 255, 192));
        }

        private void BtnCamOneshot_Click(object sender, EventArgs e)
        {
            if (cboCamList.SelectedIndex < 0)
                return;
            int index = cboCamList.SelectedIndex;

            bmpOperate[index].Dispose();
            GetCamera(index).Snap();
            bmpOperate[index] = new Bitmap(GetCamera(index).GetSnap());

            DS.ReplaceDisplayImage(bmpOperate[index]);
        }

        private void NumGain_ValueChanged(object sender, EventArgs e)
        {
            if (cboCamList.SelectedIndex < 0)
                return;

            int index = cboCamList.SelectedIndex;
            //RecipeNeedleClass.Instance.SetCamExpo(index, (float)numExpo.Value);
            RecipeNeedleClass.Instance.SetCamGain(index, (float)numGain.Value);
            GetCamera(index).SetGain((float)numGain.Value);
        }

        private void NumExpo_ValueChanged(object sender, EventArgs e)
        {
            if (cboCamList.SelectedIndex < 0)
                return;

            int index = cboCamList.SelectedIndex;
            RecipeNeedleClass.Instance.SetCamExpo(index, (float)numExpo.Value);
            //RecipeNeedleClass.Instance.SetCamGain(index, (float)numGain.Value);
            GetCamera(index).SetExposure((float)numExpo.Value);
        }

        //frmHandle FrmHandle = null;
        private void BtnHandAxis_Click(object sender, EventArgs e)
        {
            //if (frmHandle.Instance != null)
            frmHandle.Instance.Show();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            //RecipeCHClass.Instance.Load();
            //RecipeTrayClass.Instance.Load();
            //VisionTrayClass.Instance.Load();
            RecipeNeedleClass.Instance.Load();
            //if (frmHandle.Instance != null)
            //    frmHandle.Instance.Close();
            frmHandle.Instance.Hide();
            //pgCamParaOther.SelectedObject = null;
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            CoarsePositioningClass.Instance.sCoarsePosList = ctlPosClasses[0].GetPositionList();
            ModelPositioningClass.Instance.sModelPosList = ctlPosClasses[1].GetPositionList();

            //GraphicalObject grobj = myMover[0].Source;
            //RecipeTrayClass.Instance.rect_start = (grobj as JzRectEAG).GetRect;
            //GraphicalObject grobj1 = myMover[1].Source;
            //RecipeTrayClass.Instance.rect_end = (grobj1 as JzRectEAG).GetRect;
            //if (frmHandle.Instance != null)
            //    frmHandle.Instance.Close();
            frmHandle.Instance.Hide();
            //pgCamParaOther.SelectedObject = null;
            this.DialogResult = DialogResult.OK;
        }

        #endregion

        #region PRIVATE WINDOWS
        void updateLabelMsg(string msg)
        {
            lblAlignMsg.Text = msg;
            if (msg.Length > 0)
            {
                if (msg[0] == '!')
                    lblAlignMsg.ForeColor = Color.Red;
                else
                    lblAlignMsg.ForeColor = Color.Green;
            }
        }
        void FillDisplay()
        {
            //txttr1y.Text = RecipeTrayClass.Instance.Track_PosY_org[0].ToString();
            //txttr2y.Text = RecipeTrayClass.Instance.Track_PosY_org[1].ToString();
            //txttr3y.Text = RecipeTrayClass.Instance.Track_PosY_org[2].ToString();
            //txttr4y.Text = RecipeTrayClass.Instance.Track_PosY_org[3].ToString();
            //txttr1x.Text = RecipeTrayClass.Instance.Track_PosXL_org[0].ToString();
            //txttr2x.Text = RecipeTrayClass.Instance.Track_PosXL_org[1].ToString();
            //txttr2xR.Text = RecipeTrayClass.Instance.Track_PosXR_org[0].ToString();
            //txttr3xR.Text = RecipeTrayClass.Instance.Track_PosXR_org[1].ToString();
            //txttr4xR.Text = RecipeTrayClass.Instance.Track_PosXR_org[2].ToString();

            //txttr2ylastL.Text = RecipeTrayClass.Instance.Track_LenXL_TOP.ToString();
            //txttr2ylastR.Text = RecipeTrayClass.Instance.Track_LenXR_TOP.ToString();
            //txttr2xlast.Text = RecipeTrayClass.Instance.Track_LenXL_LEFT.ToString();
            //txttr2xRlast.Text = RecipeTrayClass.Instance.Track_LenXR_LEFT.ToString();
        }

        void FillDisplay(bool eChangeBaseBmp = false)
        {
            IsNeedToChange = false;

            //_dynamicMover(tabControl1.SelectedIndex, eChangeBaseBmp);
            //_staticMover(tabControl1.SelectedIndex);

            IsNeedToChange = true;
        }

        void init_CboList()
        {
            int i = 0;
            cboCamList.Items.Clear();
            while (i < CameraConfig.Instance.COUNT)
            {
                var cam = Universal.CAMERAS[i];
                if (cam != null)
                {
                    cboCamList.Items.Add(i.ToString() + "-" + cam.CameraCfg.SerialNumber);
                }
                i++;
            }

            if (cboCamList.Items.Count > 0)
            {
                cboCamList.SelectedIndex = 0;
                numExpo.Value = (decimal)RecipeNeedleClass.Instance.GetCamExpo(0);
                numGain.Value = (decimal)RecipeNeedleClass.Instance.GetCamGain(0);
                numLed.Value = (decimal)RecipeNeedleClass.Instance.GetCamLedValue(0);
                _setCamLabelName(0);
            }
            cboCamList.SelectedIndexChanged += CboCamList_SelectedIndexChanged;
        }

        private void CboCamList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MACHINE.PLCIO.LedReset();
            if (cboCamList.SelectedIndex < 0)
                return;
            //IsNeedToChange = true;
            int index = cboCamList.SelectedIndex;
            _setCamLabelName(index);
            numExpo.Value = (decimal)RecipeNeedleClass.Instance.GetCamExpo(index);
            numGain.Value = (decimal)RecipeNeedleClass.Instance.GetCamGain(index);
            numLed.Value = (decimal)RecipeNeedleClass.Instance.GetCamLedValue(index);

            writeDSControlPara();

            update_Display();
            //IsNeedToChange = false;
        }


        void _setCamLabelName(int camid)
        {
            switch (camid)
            {
                case 0: lblCamName.Text = "全域特征定位相机"; break;
                case 1: lblCamName.Text = "特微检测高度相机"; break;
                default: lblCamName.Text = "显微相机"; break;

            }
        }
        void init_Display()
        {
            //DS = dispUI1;
            DS.Initial(100, 0.01f);
            DS.SetDisplayType(DisplayTypeEnum.NORMAL);
            //DS.DebugClickPointAction += DS_DebugClickPointAction;
            DS.OnFireMessage += DS_OnFireMessage;
            //m_DispUI.MoverAction += M_DispUI_MoverAction;
            //m_DispUI.AdjustAction += M_DispUI_AdjustAction;
        }

        private void DS_OnFireMessage(object sender, DisplayEventArgs e)
        {
            if (cboCamList.SelectedIndex < 0)
                return;

            int index = cboCamList.SelectedIndex;
            ControlClass needleCam = RecipeNeedleClass.Instance.camparaClassArray[index];

            if (e.Tag != null && e.Tag is Bitmap)
            {
                try
                {
                    if (InvokeRequired)
                    {
                        EventHandler<DisplayEventArgs> h = DS_OnFireMessage;
                        this.Invoke(h, sender, e);
                    }
                    else
                    {
                        Bitmap bmp = (Bitmap)e.Tag;
                        //dispUI1.UpdateLiveImage(bmp);
                    }
                }
                catch (Exception ex)
                {
                    //>>> 此一層的 try - catch 以後可以省略.
                    //>>> 會由 Event Sender 處理 exception
                    throw ex;
                }
            }
            else if (e.Tag != null && e.Tag is PointF)
            {
                try
                {
                    if (InvokeRequired)
                    {
                        EventHandler<DisplayEventArgs> h = DS_OnFireMessage;
                        this.Invoke(h, sender, e);
                    }
                    else
                    {
                        PointF ptLocation = (PointF)e.Tag;
                        PointF ptRealOffset = new PointF((float)(ptLocation.X * needleCam.Resolution),
                                                                      (float)(ptLocation.Y * needleCam.Resolution));
                        MACHINE.GoPositionOffset(ptRealOffset.X, ptRealOffset.Y);
                    }
                }
                catch (Exception ex)
                {
                    //>>> 此一層的 try - catch 以後可以省略.
                    //>>> 會由 Event Sender 處理 exception
                    throw ex;
                }
            }
            else if (e.Tag != null && e.Tag is JRotatedRectangleF)
            {
                try
                {
                    if (InvokeRequired)
                    {
                        EventHandler<DisplayEventArgs> h = DS_OnFireMessage;
                        this.Invoke(h, sender, e);
                    }
                    else
                    {
                        JRotatedRectangleF ptLocation = (JRotatedRectangleF)e.Tag;
                        PointF ptOrgCenter = new PointF(bmpOperate[index].Width / 2, bmpOperate[index].Height / 2);
                        PointF ptOffset = new PointF(-(float)((ptLocation.fCX - ptOrgCenter.X)),
                                                                  -(float)((ptLocation.fCY - ptOrgCenter.Y)));

                        PointF ptRealOffset = new PointF((float)(ptOffset.X * needleCam.Resolution),
                                                                      (float)(ptOffset.Y * needleCam.Resolution));

                        DS.ClearStaticMover();
                        myStaticMovers.Clear();

                        RectangleF rectangleF = new RectangleF((float)(ptLocation.fCX - ptLocation.fWidth / 2),
                                                                                            (float)(ptLocation.fCY - ptLocation.fHeight / 2),
                                                                                            (float)ptLocation.fWidth,
                                                                                            (float)ptLocation.fHeight);
                        JzRectEAG _rect = new JzRectEAG(Color.FromArgb(0, Color.Blue), rectangleF, -ptLocation.fAngle);
                        _rect.RelateLevel = 3;
                        _rect.RelateNo = 0;
                        _rect.RelatePosition = 0;
                        myStaticMovers.Add(_rect);

                        DS.SetStaticMover(myStaticMovers);
                        DS.RefreshDisplayShape();
                        DS.MappingSelect();
                        update_Display(false);

                        switch (e.Message)
                        {
                            case "MoveMaxBlob":
                                MACHINE.GoPositionOffset(ptRealOffset.X, ptRealOffset.Y);
                                break;
                            case "MoveMaxBlobPos":
                                string strData = ptRealOffset.X.ToString(m_format) + "," + ptRealOffset.Y.ToString(m_format);
                                lstCollectData.Items.Add(strData);
                                break;
                        }
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

        private void DS_DebugClickPointAction(PointF ptLocation, MouseButtons eMyMouseButtons)
        {
            //if (cboCamList.SelectedIndex < 0)
            //    return;

            //int index = cboCamList.SelectedIndex;
            //ControlClass needleCam = RecipeNeedleClass.Instance.camparaClassArray[index];

            //PointF ptRealOffset = new PointF((float)(ptLocation.X * needleCam.Resolution),
            //                                                          (float)(ptLocation.Y * needleCam.Resolution));

            //if (eMyMouseButtons == MouseButtons.Left)
            //{
            //    MACHINE.GoPositionOffset(ptRealOffset.X, ptRealOffset.Y);
            //}

        }

        void update_Display(bool eChangeToDefault = true)
        {
            DS.Refresh();
            if (eChangeToDefault)
                DS.DefaultView();
        }
        protected ICam GetCamera(int camID)
        {
            return Universal.CAMERAS[camID];
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
        private void writeDSControlPara()
        {
            if (cboCamList.SelectedIndex < 0)
                return;

            int index = cboCamList.SelectedIndex;
            //ControlClass ctrlTemp = new ControlClass(RecipeNeedleClass.Instance.camparaClassArray[index].ToString());
            pgCamParaOther.SelectedObject = RecipeNeedleClass.Instance.camparaClassArray[index];
            alignImageCenterClass.SetControlPara(RecipeNeedleClass.Instance.camparaClassArray[index].ToString());
            DS.SetControlPara(RecipeNeedleClass.Instance.camparaClassArray[index].ToString());
        }
        private NeedleXYZ goFocusPosition(int icamNo)
        {
            NeedleXYZ needleXYZ = new NeedleXYZ();
            switch (icamNo)
            {
                case 0:
                    needleXYZ = new NeedleXYZ(INI.Instance.Cam0CenterPos);
                    break;
                case 1:
                    needleXYZ = new NeedleXYZ(INI.Instance.Cam1CenterPos);
                    break;
                case 2:
                    needleXYZ = new NeedleXYZ(INI.Instance.Cam2CenterPos);
                    break;
            }
            return needleXYZ;
        }
        void SaveData(string DataStr, string FileName)
        {
            StreamWriter Swr = new StreamWriter(FileName, false, Encoding.Default);

            Swr.Write(DataStr);

            Swr.Flush();
            Swr.Close();
        }
        #endregion

    }
}
