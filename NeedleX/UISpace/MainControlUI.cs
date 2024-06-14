using JetEazy;
using NeedleX.UISpace.MainSpace;
using System.Drawing;
using System.Windows.Forms;
using VsCommon.ControlSpace.MachineSpace;

namespace Eazy_Project_III.UISpace
{
    public partial class MainControlUI : UserControl
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        //MainGdx3UI mainX3;
        //MainX1UI mainX1;
        NeedleMainUI needleui;

        public MainControlUI()
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

                            //mainLSUI = new MainLSUI();
                            //mainLSUI.Init();
                            //mainLSUI.Location = new Point(0, 0);
                            //this.Controls.Add(mainLSUI);
                            //mainLSUI.Dock = DockStyle.Fill;

                            ////mainX1.OnChangeState += MainX1_OnChangeState;
                            //mainLSUI.OnChangeState += MainLSUI_OnChangeState;
                            break;
                        case OptionEnum.MAIN_NEEDLE:
                            needleui = new NeedleMainUI();
                            needleui.Init();
                            needleui.Location = new Point(0, 0);
                            this.Controls.Add(needleui);
                            needleui.Dock = DockStyle.Fill;

                            break;
                    }
                    break;
            }
        }
        public void Close()
        {
            switch (VERSION)
            {
                case VersionEnum.TRAVELLER:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_NEEDLE:
                            needleui.Close();
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
                            //mainLSUI.SetEnable(isenable);
                            break;
                    }
                    break;
                case VersionEnum.PROJECT:
                    switch (OPTION)
                    {
                        case OptionEnum.DISPENSINGX1:
                            //mainX1.SetEnable(isenable);
                            break;
                        case OptionEnum.DISPENSING:
                            //mainX3.SetEnable(isenable);
                            break;

                    }
                    break;
            }
        }
        public void ChangeRecipe()
        {
            switch (VERSION)
            {
                case VersionEnum.TRAVELLER:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_LS:
                            //mainLSUI.ChangeRecipe();
                            break;
                    }
                    break;
            }
        }

        public void Tick()
        {
            switch (VERSION)
            {
                case VersionEnum.TRAVELLER:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_LS:
                            //mainLSUI.Tick();
                            break;
                        case OptionEnum.MAIN_NEEDLE:
                            needleui.Tick();
                            break;
                    }
                    break;
                case VersionEnum.PROJECT:
                    switch (OPTION)
                    {
                        case OptionEnum.DISPENSING:
                            //mainX3.Tick();
                            break;
                        case OptionEnum.DISPENSINGX1:
                            //mainX1.Tick();
                            break;
                    }
                    break;
            }
        }


        public delegate void ChangeStateHandler(MainS1State status);
        public event ChangeStateHandler OnChangeState;
        protected void FireChangeState(MainS1State status)
        {
            if (OnChangeState != null)
            {
                OnChangeState(status);
            }
        }




    }
}
