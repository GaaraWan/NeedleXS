﻿using Common.RecipeSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.DBSpace;
using NeedleX.FormSpace;
using System;
using System.Windows.Forms;
using Traveller106.FormSpace;
//using Traveller106.OPSpace;

//using Mist.OPSpace;
//using Mist.DBSpace;

namespace PhotoMachine.UISpace
{
    public partial class RcpUI : UserControl
    {
        enum TagEnum
        {
            ADD,
            MODIFY,

            OK,
            CANCEL,

            DETIAL,
        }

        //VIEWClass VIEW;
        RCPDBClass RCPDB;
        RCPItemClass RCPItemNow
        {
            get
            {
                return RCPDB.RCPItemNow;
            }
        }

        GroupBox grpRcpData;

        TextBox txtName;
        TextBox txtVersion;

        //StpUI STPUI;

        Button btnAdd;
        Button btnModify;

        Button btnOK;
        Button btnCancel;

        Button btnDetial;

        Label lblModifyDateTime;

        RichTextBox rtbComment;
        
        //Language Setup
        JzLanguageClass myLanguage = new JzLanguageClass();

        string UIPath = "";
        int LanguageIndex = 0;

        VersionEnum VER = VersionEnum.STEROPES;
        OptionEnum OPT = OptionEnum.MAIN;


        //IRecipe IxRecipe
        //{
        //    get { return RecipeCHClass.Instance; }
        //}


        public RcpUI()
        {
            InitializeComponent();
            SizeChanged += RcpUI_SizeChanged;
        }

        public void Initial(string uipath,
            int langindex,
            VersionEnum ver,
            OptionEnum opt,
            RCPDBClass rcpdb)
            //VIEWClass view)
        {
            RCPDB = rcpdb;
            //VIEW = view;

            UIPath = uipath;
            LanguageIndex = langindex;
            VER = ver;
            OPT = opt;

            myLanguage.Initial(UIPath + "\\RcpUI.jdb", LanguageIndex, this);

            grpRcpData = groupBox1;
            rtbComment = richTextBox1;

            txtName = textBox1;
            txtVersion = textBox2;

            lblModifyDateTime = label4;

            btnAdd = button1;
            btnModify = button2;
            btnOK = button4;
            btnCancel = button6;
            btnDetial = button3;

            btnAdd.Tag = TagEnum.ADD;
            btnModify.Tag = TagEnum.MODIFY;
            btnOK.Tag = TagEnum.OK;
            btnCancel.Tag = TagEnum.CANCEL;
            btnDetial.Tag = TagEnum.DETIAL;


            btnAdd.Click += new EventHandler(btn_Click);
            btnModify.Click += new EventHandler(btn_Click);
            btnOK.Click += new EventHandler(btn_Click);
            btnCancel.Click += new EventHandler(btn_Click);
            btnDetial.Click += new EventHandler(btn_Click);
            
            //SizeChanged += RcpUI_SizeChanged;

            //STPUI = stpUI1;
            //STPUI.Initial(UIPath,LanguageIndex,VER,OPT,VIEW);
            //STPUI.TriggerAction += new StpUI.TriggerHandler(STPUI_TriggerAction);
            //STPUI.TriggerActionForSetupDetail += new StpUI.TriggerHandlerForSetupDetail(STPUI_TriggerActionForSetupDetail);

            FillDisplay(true);

            DBStatus = DBStatusEnum.NONE;

        }

        void STPUI_TriggerActionForSetupDetail(RCPStatusEnum status, int setupindex)
        {
            switch (status)
            {
                default:
                    OnTrigger(status, setupindex);
                    break;
            }
        }

        void STPUI_TriggerAction(RCPStatusEnum status)
        {
            switch (status)
            {
                default:
                    OnTrigger(status);
                    break;
            }
        }


        void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Button)sender).Tag;

            switch (KEYS)
            {
                case TagEnum.ADD:
                    AddAndCopy(false);
                    break;
                case TagEnum.MODIFY:
                    Modify();
                    break;
                case TagEnum.OK:
                    ModifyComplete();
                    break;
                case TagEnum.CANCEL:
                    ModifyCancel();
                    break;
                case TagEnum.DETIAL:
                    showRecipeDialogWindow();
                    break;
            }
        }
        void AddAndCopy(bool IsCopy)
        {
            OnTrigger(RCPStatusEnum.EDIT);

            RCPDB.AddAndCopy(IsCopy);
            FillDisplay(false || !IsCopy);

            DBStatus = DBStatusEnum.ADD;
        }
        void Modify()
        {
            OnTrigger(RCPStatusEnum.EDIT);

            RCPDB.Backup();
            DBStatus = DBStatusEnum.MODIFY;
        }
        void ModifyComplete()
        {
            if (RCPDB.CheckDuplicate(txtName.Text.Trim() + txtVersion.Text.Trim(), RCPItemNow.Index))
            {

                JetEazy.BasicSpace.VsMSG.Instance.Warning("名称或版本已存在，请检查。");

                //MessageBox.Show(myLanguage.Messages("msg1", INI.LANGUAGE), "SYS", MessageBoxButtons.OK);
                txtName.Focus();
            }
            else
            {
                WriteBack(true);

                //VIEW.Save();

                //STPUI.ModifyComplete();

                //STPUI.ResetcboSetup();
                if (DBStatus == DBStatusEnum.ADD)
                {
                    //RecipeTrayClass.Instance.ChangeIndex(RCPDB.Indicator);
                    RecipeNeedleClass.Instance.ChangeIndex(RCPDB.Indicator);
                }
                    
                //RecipeCHClass.Instance.Save();
                //RecipeTrayClass.Instance.Save();
                //VisionTrayClass.Instance.Save();
                RecipeNeedleClass.Instance.Save();
                FillDisplay(true);

                OnTrigger(RCPStatusEnum.MODIFYCOMPLETE);
                DBStatus = DBStatusEnum.NONE;
            }
        }
        void ModifyCancel()
        {
            if (DBStatus == DBStatusEnum.ADD)
            {
                RCPDB.DeleteLast();
                //RecipeTrayClass.Instance.ChangeIndex(RCPDB.Indicator);
                RecipeNeedleClass.Instance.ChangeIndex(RCPDB.Indicator);
            }
            else
                RCPDB.Restore();

            //VIEW.Load();
            ////VIEW.Train();

            //STPUI.ModifyCancel();

            //RecipeCHClass.Instance.Load();
            //RecipeTrayClass.Instance.Load();
            //VisionTrayClass.Instance.Load();
            RecipeNeedleClass.Instance.ChangeIndex(RCPDB.Indicator);
            RecipeNeedleClass.Instance.Load();
            FillDisplay(true);

            //STPUI.ResetcboSetup();

            OnTrigger(RCPStatusEnum.MODIFYCANCEL);
            DBStatus = DBStatusEnum.NONE;
        }

        void WriteBack(bool IsWithChange)
        {
            if (IsWithChange)
            {
                RCPItemNow.Name = txtName.Text;
                RCPItemNow.Version = txtVersion.Text;
                RCPItemNow.Comment = rtbComment.Text.Trim();

                RCPItemNow.ModifyDateTime = JzTimes.DateTimeString;
            }

            RCPDB.Save();
        }
        public void ChangeRecipe(bool IsLoad) //IsLoad is judge is the image is from file or memory
        {
            FillDisplay(IsLoad);
        }
        void FillDisplay(bool IsLoad)   //IsLoad is judge is the image is from file or memory
        {
            txtName.Text = RCPItemNow.Name;
            txtVersion.Text = RCPItemNow.Version;

            lblModifyDateTime.Text = RCPItemNow.ToModifyString();

            txtName.ReadOnly = RCPItemNow.Index == 0;
            txtVersion.ReadOnly = RCPItemNow.Index == 0;

            rtbComment.Text = RCPItemNow.Comment;

            //if (IsLoad)
            //    STPUI.ResetcboSetup();

        }
        DBStatusEnum myDBStatus = DBStatusEnum.NONE;
        DBStatusEnum DBStatus
        {
            get
            {
                return myDBStatus;
            }
            set
            {
                myDBStatus = value;

                switch (myDBStatus)
                {
                    case DBStatusEnum.ADD:
                    case DBStatusEnum.MODIFY:

                        grpRcpData.Enabled = true;

                        btnAdd.Visible = false;
                        btnModify.Visible = false;

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        break;
                    case DBStatusEnum.NONE:
                        grpRcpData.Enabled = false;

                        btnAdd.Visible = true;
                        btnModify.Visible = true;

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        break;
                }
            }
        }

        public delegate void TriggerHandler(RCPStatusEnum status);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RCPStatusEnum status)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status);
            }
        }

        public delegate void TriggerHandlerForSetupDetail(RCPStatusEnum status, int setupindex);
        public event TriggerHandlerForSetupDetail TriggerActionForSetupDetail;
        public void OnTrigger(RCPStatusEnum status, int setupindex)
        {
            if (TriggerActionForSetupDetail != null)
            {
                TriggerActionForSetupDetail(status, setupindex);
            }
        }

        void showRecipeDialogWindow()
        {
            using (var frm = new frmRecipe())
            {
                frm.ShowDialog();
            }
        }


        #region AUTO_LAYOUT
        void RcpUI_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                _auto_layout();
            }
            catch
            {
            }
        }
        void _auto_layout()
        {
#if OPT_LETIAN_AUTO_LAYOUT

            groupBox1.Dock = DockStyle.Top;
            richTextBox1.Dock = DockStyle.Bottom;

            var rcc = ClientRectangle;
            int pad = 3;
            int w = (rcc.Width - pad * 6) / 3;
            int x = pad * 2;

            //var ctrls = new Control[] { btnAdd, btnModify, btnCancel };
            var ctrls = new Control[] { button1, button2, button6 };
            foreach (var c in ctrls)
            {
                c.Top = rcc.Bottom - c.Height - pad * 2;
                c.Left = x;
                c.Width = w;
                x += w + pad;
            }
            // btnOK
            button4.Location = button2.Location;
            button4.Size = button2.Size;
            // lblModifyDateTime
            label4.Top = button4.Top - label4.Height - pad;
            label4.Left = groupBox1.Left;
            label4.Width = rcc.Width - label4.Left * 2;
            // groupBox1
            groupBox1.Height = (label4.Top - pad * 2) - groupBox1.Top;
            // richbox1
            richTextBox1.Height = richTextBox1.Bottom - button3.Bottom - pad * 2;
            // btnDetail
            rcc = groupBox1.ClientRectangle;
            button3.Left = rcc.Right - button3.Width - pad * 5;
            textBox2.Width = button3.Right - textBox2.Left;
#endif
        }
        #endregion
    }
}
