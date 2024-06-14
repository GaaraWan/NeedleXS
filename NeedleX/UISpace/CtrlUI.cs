using JetEazy;
using NeedleX.UISpace.CtrlSpace;
using System.Drawing;
using System.Windows.Forms;
using Traveller106.ControlSpace.MachineSpace;
using VsCommon.ControlSpace.MachineSpace;

namespace PhotoMachine.UISpace
{
    public partial class CtrlUI : UserControl
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        //X3CtrlUI X3Ctrl;
        //X1CtrlUI X1Ctrl;
        NeedleCtrl needleCtrl;

        public CtrlUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {

        }

        public void Initial(VersionEnum version, OptionEnum option, GeoMachineClass machine)
        {
            VERSION = version;
            OPTION = option;

            switch (VERSION)
            {
                case VersionEnum.TRAVELLER:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_LS:

                            //MainLSCtrl = new MainLSCtrlUI();
                            //MainLSCtrl.Initial(VERSION, OPTION, (MainLSMachineClass)machine);
                            //MainLSCtrl.Location = new Point(0, 0);
                            //this.Controls.Add(MainLSCtrl);
                            //MainLSCtrl.Dock = DockStyle.Fill;

                            break;
                        case OptionEnum.MAIN_NEEDLE:
                            needleCtrl = new NeedleCtrl();
                            needleCtrl.Initial(VERSION, OPTION, (NeedleMachineClass)machine);
                            needleCtrl.Location = new Point(0, 0);
                            this.Controls.Add(needleCtrl);
                            needleCtrl.Dock = DockStyle.Fill;
                            break;
                    }
                    break;
                case VersionEnum.PROJECT:

                    break;
                
            }
        }

        //private void AllinoneSDCTRL_TriggerAction(ActionEnum action, string opstr)
        //{
        //    TriggerAction(action, opstr);
        //}

        //private void DFlyCTRL_TriggerAction(ActionEnum action, string opstr)
        //{
        //    TriggerAction(action, opstr);
        //}

        public void Tick()
        {
            switch (VERSION)
            {
                case VersionEnum.TRAVELLER:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_NEEDLE:
                            //MainLSCtrl.Tick();
                            needleCtrl.Tick();
                            break;
                    }
                    break;
                case VersionEnum.PROJECT:
                    switch (OPTION)
                    {
                        case OptionEnum.DISPENSINGX1:
                            //X1Ctrl.Tick();
                            break;
                        case OptionEnum.DISPENSING:
                            //X3Ctrl.Tick();
                            break;
                    }
                    break;
            }
        }
        public void SetEnable(bool isenable)
        {
            switch (VERSION)
            {
                case VersionEnum.TRAVELLER:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_LS:
                            //MainLSCtrl.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_NEEDLE:
                            needleCtrl.SetEnable(isenable);
                            break;
                    }
                    break;
                case VersionEnum.PROJECT:
                    switch (OPTION)
                    {
                        case OptionEnum.DISPENSINGX1:
                            //X1Ctrl.SetEnable(isenable);
                            break;
                        case OptionEnum.DISPENSING:
                            //X3Ctrl.SetEnable(isenable);
                            break;
                        
                    }
                    break;
            }
        }

        public void myDispose()
        {
            switch (VERSION)
            {
                case VersionEnum.PROJECT:
                    switch (OPTION)
                    {
                        case OptionEnum.DISPENSING:
                            break;
                    }
                    break;

            }
        }


        public delegate void TriggerHandler(ActionEnum action, string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(ActionEnum action, string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(action, opstr);
            }
        }

    }
}
