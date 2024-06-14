using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace.MotionSpace;
using JetEazy.ControlSpace.PLCSpace;
using JetEazy;

namespace VsCommon.ControlSpace.MachineSpace
{
    public enum Machine_EA : int
    {
        MAIN_LS = 0,
        MAIN_NEEDLE = 1,
    }

    [Serializable]
    public abstract class GeoMachineClass
    {
        protected int PLCCount = 0;
        protected int MotionCount = 0;
        protected int ProjectorCount = 0;
        protected int BarcodeM3DCount = 0;

        public VsCommPLC[] PLCCollection;
        public PLCMotionClass[] PLCMOTIONCollection;
        public ModbusRTUClass[] ModbusRTUClassCollection;
        public EzBarcodeM3DHelper[] EzBarcodeM3DHelperCollection;

        protected string WORKPATH;

        protected Machine_EA myMachineEA;
        protected JzTimes myJzTimes;

        protected ProcessClass MainProcess;

        protected bool IsDirect = false;
        protected bool IsNoUseIO = false;
        protected bool IsNoUseMotor = false;

        public int[] DelayTime = new int[10];
       
        public abstract void GetStart(bool isdirect, bool isnouseplc);
       
        public abstract void Tick();
        public abstract void MainProcessTick();
        public abstract void SetDelayTime();
        public abstract void CheckEvent();
        public abstract void GoHome();
        public abstract void GetOPString(string opstr);
        public abstract bool Initial(bool isnouseio,bool isnousemotor);
        public abstract void Close();

        public abstract string PLCFps();

        public abstract void SetNormalTemp(bool ebTemp);

        protected UInt16 HEX16(string HexStr)
        {
            return System.Convert.ToUInt16(HexStr, 16);
        }
        protected UInt32 HEX32(string HexStr)
        {
            return System.Convert.ToUInt32(HexStr, 16);
        }

        public delegate void TriggerHandler(MachineEventEnum machineevent);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(MachineEventEnum machineevent)
        {
            if (TriggerAction != null)
            {
                TriggerAction(machineevent);
            }
        }
    }
}
