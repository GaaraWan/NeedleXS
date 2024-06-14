using Eazy_Project_III;
using Eazy_Project_III.OPSpace;
using JetEazy;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traveller106;
using Traveller106.ControlSpace.MachineSpace;
//using Traveller106.OPSpace;
using VsCommon.ControlSpace.MachineSpace;

namespace VsCommon.ControlSpace
{


    public class MachineCollectionClass
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        public GeoMachineClass MACHINE;
        public MachineCollectionClass()
        {

        }

        public void Intial(VersionEnum version,OptionEnum option, GeoMachineClass machine)
        {
            VERSION = version;
            OPTION = option;

            MACHINE = machine;

            MotorSpeed();
            SetIniPara();
            //WriteToPlcModulePosition();

            MACHINE.TriggerAction += MACHINE_TriggerAction;
        }

        private void MACHINE_TriggerAction(MachineEventEnum machineevent)
        {
            OnTrigger(machineevent);
        }


        public void MotorSpeed()
        {
            foreach (PLCMotionClass MOTION in MACHINE.PLCMOTIONCollection)
            {
                MOTION.SetSpeed(SpeedTypeEnum.HOMESLOW);
                MOTION.SetSpeed(SpeedTypeEnum.HOMEHIGH);
                MOTION.SetSpeed(SpeedTypeEnum.MANUAL);
                MOTION.SetSpeed(SpeedTypeEnum.GO);
            }
        }

        public void SetIniPara()
        {
           
        }
        public int GetMotorCount()
        {
            return MACHINE.PLCMOTIONCollection.Length;
        }


        public string GetPosition()
        {
            string posstr = "";

            posstr += MACHINE.PLCMOTIONCollection[0].PositionNowString + ",";
            posstr += MACHINE.PLCMOTIONCollection[1].PositionNowString + ",";
            posstr += MACHINE.PLCMOTIONCollection[2].PositionNowString;

            return posstr;

        }
        public void GoPosition(string opstr)
        {
            string[] strs = opstr.Split(',');
            float x = float.Parse(strs[0]);
            float y = float.Parse(strs[1]);
            //float z = float.Parse(strs[2]);
            MACHINE.PLCMOTIONCollection[0].Go(x);
            MACHINE.PLCMOTIONCollection[1].Go(y);
            //MACHINE.PLCMOTIONCollection[2].Go(z);
        }

        public void GoHome()
        {
        }

        public void Close()
        {
            MACHINE.Close();
        }
        public string PLCFps()
        {
            return MACHINE.PLCFps();
        }

        public void Tick()
        {
            MACHINE.Tick();
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
