
//#define HC_Q1_1300D
#define FATEK

using JetEazy.ControlSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using VsCommon.ControlSpace.IOSpace;

namespace VsCommon.ControlSpace.IOSpace
{
#if HC_Q1_1300D
    public class NeedleIOClass : GeoIOClass
    {

        public NeedleIOClass()
        {   

        }
        public void Initial(string path, JetEazy.ControlSpace.PLCSpace.VsCommPLC[] plc)
        {
            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {
           
        }

        public override void SaveData()
        {
            
        }

        public bool ADR_ISEMC
        {
            get { return !GetQXQB("0:IX0.3"); }
        }
        public bool ADR_ISAUTO_AND_MANUAL
        {
            get { return GetQXQB("0:IX0.4"); }
        }
        public bool ADR_ISSTART
        {
            get { return GetQXQB("0:IX0.0"); }
        }
        public bool ADR_ISSTOP
        {
            get { return GetQXQB("0:IX0.1"); }
        }
        public bool ADR_ISVACC
        {
            get { return GetQXQB("0:IX0.2"); }
        }

        public bool ADR_RED
        {
            get
            {
                return GetQXQB("0:QX0.0");
            }
            set
            {
                SetQXQB("0:QX0.0", value);
            }
        }
        public bool ADR_GREEN
        {
            get
            {
                return GetQXQB("0:QX0.1");
            }
            set
            {
                SetQXQB("0:QX0.1", value);
            }
        }
        public bool ADR_YELLOW
        {
            get
            {
                return GetQXQB("0:QX0.2");
            }
            set
            {
                SetQXQB("0:QX0.2", value);
            }
        }
        public bool ADR_BRAKE
        {
            get
            {
                return GetQXQB("0:QX0.3");
            }
            set
            {
                SetQXQB("0:QX0.3", value);
            }
        }


        public bool GetQXQB(string addStr)
        {
            AddressClass address = new AddressClass(addStr);
            return PLC[address.SiteNo].IOData.GetBit(address.Address0);
        }
        public void SetQXQB(int index, bool ison)
        {
            string addr = "0:QB" + index.ToString() + ".0";
            SetQXQB(addr, ison);
        }
        public void SetQXQB(string addStr, bool ison)
        {
            AddressClass address = new AddressClass(addStr);
            PLC[address.SiteNo].SetIO(ison, address.Address0);
        }
        public short GetMW(string addStr)
        {
            AddressClass address = new AddressClass(addStr);
            return PLC[address.SiteNo].IOData.GetMW(address.Address0);
        }

        public void SetMW(string addStr, float value)
        {
            AddressClass address = new AddressClass(addStr);
            PLC[address.SiteNo].SetData(value, address);
        }
        public void SetMW(string addStr, int value)
        {
            AddressClass address = new AddressClass(addStr);
            PLC[address.SiteNo].SetData(value, address);
        }

    }
#endif

#if FATEK
    public class NeedleIOClass : GeoIOClass
    {

        public NeedleIOClass()
        {

        }
        public void Initial(string path, JetEazy.ControlSpace.PLCSpace.VsCommPLC[] plc)
        {

            ADDRESSARRAY = new AddressClass[10];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {
            //ADDRESSARRAY[0] = new AddressClass(ReadINIValue("Status Address", "ADR_RED", "", INIFILE));
            ADDRESSARRAY[0] = new AddressClass(ReadINIValue("Status Address", "ADR_ISEMC", "", INIFILE));
            ADDRESSARRAY[1] = new AddressClass(ReadINIValue("Status Address", "ADR_ISAUTO_AND_MANUAL", "", INIFILE));
            ADDRESSARRAY[2] = new AddressClass(ReadINIValue("Status Address", "ADR_ISSTART", "", INIFILE));
            ADDRESSARRAY[3] = new AddressClass(ReadINIValue("Status Address", "ADR_ISSTOP", "", INIFILE));
            ADDRESSARRAY[4] = new AddressClass(ReadINIValue("Status Address", "ADR_ISVACC", "", INIFILE));
            

            ADDRESSARRAY[5] = new AddressClass(ReadINIValue("Operation Address", "ADR_RED", "", INIFILE));
            ADDRESSARRAY[6] = new AddressClass(ReadINIValue("Operation Address", "ADR_GREEN", "", INIFILE));
            ADDRESSARRAY[7] = new AddressClass(ReadINIValue("Operation Address", "ADR_YELLOW", "", INIFILE));
            ADDRESSARRAY[8] = new AddressClass(ReadINIValue("Operation Address", "ADR_BRAKE", "", INIFILE));
            ADDRESSARRAY[9] = new AddressClass(ReadINIValue("Operation Address", "ADR_TOFOCUS", "", INIFILE));

            //ADDRESSARRAY[0] = new AddressClass(ReadINIValue("Data Address", "ADR_RED", "", INIFILE));
            //ADDRESSARRAY[0] = new AddressClass(ReadINIValue("Parameters", "ADR_RED", "", INIFILE));

        }

        public override void SaveData()
        {

        }

        public bool ADR_ISEMC
        {
            get
            {
                AddressClass address = ADDRESSARRAY[0];
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_ISAUTO_AND_MANUAL
        {
            get
            {
                AddressClass address = ADDRESSARRAY[1];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_ISSTART
        {
            get
            {
                AddressClass address = ADDRESSARRAY[2];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_ISSTOP
        {
            get
            {
                AddressClass address = ADDRESSARRAY[3];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_ISVACC
        {
            get
            {
                AddressClass address = ADDRESSARRAY[4];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        public bool ADR_RED
        {
            get
            {
                AddressClass address = ADDRESSARRAY[5];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                AddressClass address = ADDRESSARRAY[5];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_GREEN
        {
            get
            {
                AddressClass address = ADDRESSARRAY[6];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                AddressClass address = ADDRESSARRAY[6];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_YELLOW
        {
            get
            {
                AddressClass address = ADDRESSARRAY[7];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                AddressClass address = ADDRESSARRAY[7];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_TOFOCUS
        {
            get
            {
                AddressClass address = ADDRESSARRAY[9];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                AddressClass address = ADDRESSARRAY[9];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool ADR_BRAKE
        {
            get
            {
                AddressClass address = ADDRESSARRAY[8];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                AddressClass address = ADDRESSARRAY[8];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool GetQXQB(string addStr)
        {
            AddressClass address = new AddressClass(addStr);
            return PLC[address.SiteNo].IOData.GetBit(address.Address0);
        }
        public void SetQXQB(int index, bool ison)
        {
            string addr = "0:QB" + index.ToString() + ".0";
            SetQXQB(addr, ison);
        }
        public void SetQXQB(string addStr, bool ison)
        {
            AddressClass address = new AddressClass(addStr);
            PLC[address.SiteNo].SetIO(ison, address.Address0);
        }
        public short GetMW(string addStr)
        {
            AddressClass address = new AddressClass(addStr);
            return PLC[address.SiteNo].IOData.GetMW(address.Address0);
        }
        public void LedReset()
        {
            SetLedValue(0, 0);
            SetLedValue(1, 0);
            SetLedValue(2, 0);
        }
        public void SetLedValue(int ledID,int ivalue)
        {
            switch(ledID)
            {
                case 0:
                    SetData("0:D00607", ivalue);
                    break;
                case 1:
                    SetData("0:D00627", ivalue);
                    break;
                case 2:
                    SetData("0:D00647", ivalue);
                    break;
            }
        }
        public void SetPLCPosReslution(int ivalue)
        {
            SetData("0:R00062", ivalue);
        }
        public int GetFlyPLCCount
        {
            get
            {
                return GetData("0:R00050");
            }
        }
        public void SetData(string addStr, int idata)
        {
            AddressClass address = new AddressClass(addStr);
            PLC[address.SiteNo].SetData(address.Address0, idata);
        }
        public int GetData(string addStr)
        {
            int ret = 0;
            AddressClass address = new AddressClass(addStr);
            if (string.IsNullOrEmpty(address.Address1))
                ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));
            else
                ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address1), 4) +
                                                   ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));
            return ret;
        }
        protected Int32 HEXSigned32(string HexStr)
        {
            return System.Convert.ToInt32(HexStr, 16);
        }
    }
#endif

}
