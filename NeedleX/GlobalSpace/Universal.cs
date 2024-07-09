using Common.RecipeSpace;
using Eazy_Project_III;
using Eazy_Project_III.OPSpace;
using JetEazy;
using JetEazy.CCDSpace;
using JetEazy.CCDSpace.CamLinkDriver;
using JetEazy.ControlSpace;
using JetEazy.DBSpace;
using JetEazy.Interface;
using JetEazy.OPSpace;
using JzDisplay.OPSpace;
using NeedleX.OPSpace;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Traveller106.ControlSpace.MachineSpace;
//using Traveller106.OPSpace;
//using PhotoMachine.ControlSpace.MachineSpace;
using VsCommon.ControlSpace;
using VsCommon.ControlSpace.MachineSpace;

namespace Traveller106
{
    public class Universal : JetEazy.Universal
    {
        public static bool IsNoUseCCD = false;      //<<< 無效改由 ICam.ISim() 判斷
        public static bool IsNoUseIO = false;
        public static bool IsNoUseMotor = IsNoUseIO;
        public static bool IsSilentMode = IsNoUseIO;     //抑制 Buzzer
        public static bool IsAutoLogin = true;     //抑制 Buzzer

        public static string VersionDate = "2024/07/09";

        public static VersionEnum VERSION = VersionEnum.TRAVELLER;
        public static OptionEnum OPTION = OptionEnum.MAIN_NEEDLE;

        public static bool IsNoUseLineScanMode = false;//是否不启用线扫
        //Environment Variables
        public static int MAINTICK = 100;
        //更新主程序画面
        public static int DISPLAYTICK = 500;

        public static bool IsSaveRaw = false;
        public static bool IsMultiThreadUseToRun = true;
        ///// <summary>
        ///// 是否為第二站  第一站和第四站 格局一樣
        ///// </summary>
        //public static bool IsMachineStationS2 = true;

        /// <summary>
        /// 种子功能
        /// </summary>
        public static bool IsUseSeedFuntion = false;

        public static string CODEPATH = @"D:\AUTOMATION";
        public static string VEROPT = VERSION.ToString() + "-" + OPTION.ToString();
        public static string MAINPATH = @"D:\JETEAZY\" + VEROPT;

        public static string DBPATH = MAINPATH + @"\DB";
        public static string RCPPATH = MAINPATH + @"\PIC";
        public static string UIPATH = CODEPATH + @"\" + VERSION.ToString() + "UI";

        public static string LOG_ROOT
        {
            //get {
            //    //return System.IO.Path.Combine("D:", "LOG");
            //    return "";
            //    |}
            get
            {
                switch (OPTION)
                {
                    case OptionEnum.DISPENSING:
                        return @"D:\EVENTLOG\S3";
                    case OptionEnum.DISPENSINGX1:
                        return @"D:\EVENTLOG\S1";
                    case OptionEnum.DISPENSINGX2:
                        return @"D:\EVENTLOG\S2";
                    case OptionEnum.DISPENSINGX4:
                        return @"D:\EVENTLOG\S4";
                    default:
                        return @"D:\EVENTLOG\" + OPTION.ToString();
                }
            }
        }
        public static string LOG_TXT_PATH
        {
            get { return System.IO.Path.Combine(LOG_ROOT, "Logs"); }
        }
        public static string LOG_IMG_PATH
        {
            get { return System.IO.Path.Combine(LOG_ROOT, "Images"); }
        }
        public static string LOG_ALARM_EVENT_PATH
        {
            get { return System.IO.Path.Combine(LOG_ROOT, "Alarms"); }
        }

        public static string MAPPINGDATA = @"D:\JETEAZY\" + VEROPT + @"\MAPPINGDATA";//存储tray的数据
        public static string HISTORY = @"D:\JETEAZY\" + VEROPT + @"\HISTORYDATA";//存储tray的数据
        public static string COLLECT = @"D:\JETEAZY\" + VEROPT + @"\COLLECT";
        public static string BACKUPDBPATH = @"D:\JETEAZY\" + VEROPT + @"\BACKUPDB";
        public static string WORKPATH = @"D:\JETEAZY\" + VEROPT + @"\WORK";
        public static string DEBUGRAWPATH = @"D:\JETEAZY\" + VEROPT + @"\ORG";              //偵錯儲存的原圖位置
        public static string DEBUGRESULTPATH = @"D:\JETEAZY\" + VEROPT + @"\DEBUG";         //偵錯結果圖位置
        public static string TESTRESULTPATH = @"D:\COPYDATA";                               //偵錯結果圖位置
        public static string DEBUGSRCPATH = @"D:\JETEAZY\" + VEROPT + @"\SRCDEBUG";         //離線測試用的原圖位置
        public static string OCRIMAGEPATH = @"D:\LOA\OCR\";                                 //保存的OCR测试图位置  
        public static string BarcodeIMAGEPATH = @"D:\LOA\Barcode\";                         //保存的OCR测试图位置  

        /// <summary>
        /// 跑线时读到SN.txt里的东西
        /// </summary>
        public static string DATASNTXT = "";
        public static string RELATECOLORSTR = "";
        public static string SHOWBMPSTRING = "view.png";
        public static string PlayerPASSPATH = WORKPATH + @"\TADA.wav";
        public static string PlayerFAILPATH = WORKPATH + @"\RoutingNG.wav";
        public static string PlayerOPPWRATPATH = WORKPATH + @"\OPPWRAP.wav";
        public static string RunDebugOrRelease = "";
        public static string FAILBARCODE = "";

        public static string MainX6_Path = "D:\\CollectPictures\\Inspection\\";

        static string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DBPATH + @"\DATA.mdb;Jet OLEDB:Database Password=12892414;";

        static int LanguageIndex = 0;
        public static string InitialErrorString = "";

        public static System.Drawing.Point MainFormLocation = new System.Drawing.Point(0, 0);


       
        /// <summary>
        /// 离线模式自动登入账户admin
        /// </summary>
        public static bool IsOfflineUserAutoLogin = false;



        public static CAMERAClass[] CAMERAS;
        //public static CCDCollectionClass CCDCollection;
        public static CCDCollectionClass CCDCollection;
        public static MachineCollectionClass MACHINECollection;
        //public static GdxCameraDpiCalibrator[] CAMDpi_Cali;
        public static IxLineScanCam IxLineScan = null;

        public static AlignImageCenterClass MyAlignImageCenter = new AlignImageCenterClass();
        public static AlignResult MyAlignResult { get; set; } = AlignResult.NotMove;
        public static AlignCalibration MyAlignCalibration = new AlignCalibration();
        public static PointF AlignPointFResult { get; set; } = new PointF();


        public static AccDBClass ACCDB;
        public static EsssDBClass ESSDB;
        public static RCPDBClass RCPDB;
        public static RUNDBClass RUNDB;
        public static CalibrationPlateClass CALIBRATIONPLATE;   

        public static bool Initial(int langindex)
        {
            bool ret = true;
            WORKPATH = MAINPATH + @"\WORK";

            //初始化语言
            JetEazy.BasicSpace.LanguageExClass.Instance.Load(WORKPATH);
            JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = 1;

            testProgramme();

            //byte[] data = new byte[4];
            //data[0] = (byte)9;
            //data[1] = (byte)1;
            //Int32 a = BitConverter.ToInt32(data, 0);

            //JetEazy.BasicSpace.JzToolsClass jzToolsClass = new JetEazy.BasicSpace.JzToolsClass();
            //Point pt = new Point(100, 100);
            //PointF ptf = jzToolsClass.ResizeWithLocation2(pt, -1);


            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_X6:

                    break;
            }
            LanguageIndex = langindex;

            CreateDebugDirectories();

            ACCDB = new AccDBClass(DBPATH + @"\ACCDB.jdb");
            ESSDB = new EsssDBClass(DBPATH + @"\ESSDB.jdb");
            RCPDB = new RCPDBClass(DBPATH + @"\RCPDB.jdb", RCPPATH, ESSDB.LastRecipeIndex);
            //RCPDB = new RCPDBClass(DBPATH + @"\RCPDB.jdb", RCPPATH, 1);//總是加載第一個參數 即為正常模式
            RUNDB = new RUNDBClass(DBPATH + @"\RUNDB.jdb");

            //参数加载
            //RecipeCHClass.Instance.Initial(RCPPATH, RCPDB.RCPItemNow.Index);
            //RecipeTrayClass.Instance.Initial(RCPPATH, ESSDB.LastRecipeIndex, "Tray.ini");
            //VisionTrayClass.Instance.Initial(RCPPATH, ESSDB.LastRecipeIndex, "Vision.ini");
            RecipeNeedleClass.Instance.Initial(RCPPATH, ESSDB.LastRecipeIndex, "NeedleX8.ini");

            ret &= InitialMachineCollection();

            if (!ret)
            {
                //InitialErrorString = myLanguage.Messages("msg1", LanguageIndex);
                JetEazy.BasicSpace.VsMSG.Instance.Warning("plc连接错误，请检查。");
                //return false;
            }

            switch(VERSION)
            {
                case VersionEnum.PROJECT:

                    switch(OPTION)
                    {
                        case OptionEnum.DISPENSING:

                            ret &= InitialMeasureHeight();

                            if (!ret)
                            {
                                //InitialErrorString = myLanguage.Messages("msg1", LanguageIndex);
                                JetEazy.BasicSpace.VsMSG.Instance.Warning("LE测量高度连接错误，请检查。");
                                //return false;
                            }

                            break;
                    }

                    break;
            }

            //if (!ret)
            //{
            //    InitialErrorString = myLanguage.Messages("msg1", LanguageIndex);
            //    return false;
            //}

            ret &= InitialCCD();

            if (!ret)
            {
                //InitialErrorString = myLanguage.Messages("msg1", LanguageIndex);
                //return false;
                JetEazy.BasicSpace.VsMSG.Instance.Warning("CCD连接错误，请检查。");
            }

            switch(VERSION)
            {
                case VersionEnum.PROJECT:

                    switch(OPTION)
                    {
                        case OptionEnum.DISPENSINGX1:
                            measureInitial();
                            break;
                    }

                    break;
            }

            return ret;
        }
        public static void SetLanguage(int langindex)
        {
            LanguageIndex = langindex;
        }
        static bool InitialMachineCollection()
        {
            bool ret = true;

            string opstr = "";

            switch (VERSION)
            {

                case VersionEnum.TRAVELLER:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_LS:

                            opstr += "1,";  //1個 PLC  
                            opstr += "14,";   //14個軸
                            opstr += "0,";   //0 Projector
                            opstr += "4,";   //4 barcode sacn

                            MainLSMachineClass machine = new MainLSMachineClass(Machine_EA.MAIN_LS, opstr, WORKPATH, IsNoUseIO);
                            ret = machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, machine);

                            //MACHINECollection.WriteToPlcModulePosition();
                            //MACHINECollection.WriteToPlcRecipe();
                           

                            break;
                        case OptionEnum.MAIN_NEEDLE:

                            opstr += "1,";  //1個 PLC  
                            opstr += "3,";   //14個軸
                            opstr += "0,";   //0 Projector
                            opstr += "0,";   //4 barcode sacn

                            NeedleMachineClass machineneedle = new NeedleMachineClass(Machine_EA.MAIN_NEEDLE, opstr, WORKPATH, IsNoUseIO);
                            ret = machineneedle.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, machineneedle);

                            break;
                    }

                    break;

                case VersionEnum.PROJECT:

                    switch(OPTION)
                    {
                        case OptionEnum.DISPENSING:
                            break;
                        case OptionEnum.DISPENSINGX1:
                            break;
                    }

                    break;

                case VersionEnum.ALLINONE:
                    break;
                case VersionEnum.AUDIX:
                    break;
                default:
                    break;
            }

            return ret;
        }
        static bool InitialMeasureHeight()
        {
            bool ret = true;

            string opstr = "";

            switch (VERSION)
            {
                case VersionEnum.PROJECT:

                    switch (OPTION)
                    {
                        case OptionEnum.DISPENSING:

                            //LEClass.Instance.Init(INI.Instance.CfgPath, INI.Instance.HWCPath, !INI.Instance.IsUseMeasureHeight);
                            //ret = LEClass.Instance.Open() == 0;

                            break;
                    }

                    break;

                case VersionEnum.ALLINONE:
                    break;
                case VersionEnum.AUDIX:
                    break;
                default:
                    break;
            }

            return ret;
        }
        static int measureInitial()
        {
            //CAMDpi_Cali = new GdxCameraDpiCalibrator[CameraConfig.Instance.COUNT];
            //int i = 0;
            //while (i < CameraConfig.Instance.COUNT)
            //{
            //    string _path_name = Universal.WORKPATH + "\\" + "Cam" + i.ToString() + ".cali";
            //    if (File.Exists(_path_name))
            //    {
            //        ArrayList array = new ArrayList();
            //        Read(out array, _path_name);
            //        if (array != null)
            //        {
            //            CAMDpi_Cali[i] = new GdxCameraDpiCalibrator();
            //            CAMDpi_Cali[i].FromString((string)array[0]);
            //            CAMDpi_Cali[i].bmpBase1 = (Bitmap)array[1];
            //            CAMDpi_Cali[i].bmpBase2 = (Bitmap)array[2];
            //        }
            //        else
            //        {
            //            if (CAMDpi_Cali[i] == null)
            //                CAMDpi_Cali[i] = new GdxCameraDpiCalibrator();
            //        }
            //    }
            //    else
            //    {
            //        if (CAMDpi_Cali[i] == null)
            //            CAMDpi_Cali[i] = new GdxCameraDpiCalibrator();

            //        ArrayList array = new ArrayList();
            //        array.Add(CAMDpi_Cali[i].ToString());
            //        array.Add(CAMDpi_Cali[i].bmpBase1);
            //        array.Add(CAMDpi_Cali[i].bmpBase2);
            //        Write(array, _path_name);
            //    }
            //    i++;
            //}

            return 0;
        }
        static bool InitialCCD()
        {
            bool ret = true;

            CameraConfig.Instance.Initial(WORKPATH);
            //CameraConfig.Instance.Save();
            CAMERAS = new CAMERAClass[CameraConfig.Instance.COUNT];
            int i = 0;
            while (i < CameraConfig.Instance.COUNT)
            {
                CAMERAS[i] = new CAMERAClass();
                ret &= CAMERAS[i].Initial(CameraConfig.Instance.cameras[i].ToCameraString()) == 0;

                i++;
            }

            //初始化linescanCam

            //IxLineScan = new LINESCAN_HUARUI();
            //IxLineScan.Init(Universal.IsNoUseCCD, "");//目前没有参数 后续输入必要的参数
            //ret = IxLineScan.Open();

            //CCDCollection = new CCDCollectionClass(WORKPATH, IsNoUseCCD, VERSION, OPTION);

            //ret = CCDCollection.Initial(WORKPATH);

            //if (ret)
            //    CCDCollection.GetBmpAll(-2);


            //CCD = new CCDGroupClass(INI.CCD_TOTALHEAD);

            //int ccdkind = 0;

            //CCDClass.EPIXFMTPath = WORKPATH;

            //while (ccdkind < INI.CCD_KIND)
            //{
            //    CCDClass ccd = new CCDClass();

            //    switch (ccdkind)
            //    {
            //        case 0:
            //            ret &= ccd.Initial(INI.CCD_HEAD.ToString() + "@" + INI.CCD_WIDTH.ToString() + "@" + INI.CCD_HEIGHT + "@" + WORKPATH + "@" + "0" + "@" + INI.CCD_ROTATE, INI.CCD_TYPE, INI.ISNOLIVE);
            //            break;
            //        case 1:
            //            ret &= ccd.Initial(INI.CCD2_HEAD.ToString() + "@" + INI.CCD2_WIDTH.ToString() + "@" + INI.CCD2_HEIGHT + "@" + WORKPATH + "@" + INI.CCD_HEAD.ToString() + "@" + INI.CCD2_ROTATE, INI.CCD2_TYPE, INI.ISNOLIVE);
            //            break;
            //        case 2:
            //            ret &= ccd.Initial(INI.CCD3_HEAD.ToString() + "@" + INI.CCD3_WIDTH.ToString() + "@" + INI.CCD3_HEIGHT + "@" + WORKPATH + "@" + (INI.CCD_HEAD + INI.CCD2_HEAD).ToString() + "@" + INI.CCD3_ROTATE, INI.CCD3_TYPE, INI.ISNOLIVE);
            //            break;
            //        case 3:
            //            ret &= ccd.Initial(INI.CCD4_HEAD.ToString() + "@" + INI.CCD4_WIDTH.ToString() + "@" + INI.CCD4_HEIGHT + "@" + WORKPATH + "@" + (INI.CCD_HEAD + INI.CCD2_HEAD + INI.CCD3_HEAD).ToString() + "@" + INI.CCD4_ROTATE, INI.CCD4_TYPE, INI.ISNOLIVE);
            //            break;
            //    }

            //    if (!ret)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        CCD.Add(ccd);
            //    }
            //    ccdkind++;
            //}


            return ret;
        }
        static void CreateDebugDirectories()
        {
            //if (!Directory.Exists(MAPPINGDATA))
            //    Directory.CreateDirectory(MAPPINGDATA);
            //if (!Directory.Exists(HISTORY))
            //    Directory.CreateDirectory(HISTORY);
            //if (!Directory.Exists(COLLECT))
            //    Directory.CreateDirectory(COLLECT);
            if (!Directory.Exists(DEBUGSRCPATH))
                Directory.CreateDirectory(DEBUGSRCPATH);
            if (!Directory.Exists(DEBUGRESULTPATH))
                Directory.CreateDirectory(DEBUGRESULTPATH);
        }
        static void testProgramme()
        {
            ////测试校正功能 这里只是坐标系平移和旋转
            //JetEazy.BasicSpace.CAoiCalibration cAoiCalibration = new JetEazy.BasicSpace.CAoiCalibration();
            //PointF[,] _w = new PointF[2, 2];
            //PointF[,] _v = new PointF[2, 2];
            //_w[0, 0] = new PointF(0, 1);
            //_w[0, 1] = new PointF(1, 1);
            //_w[1, 0] = new PointF(0, 0);
            //_w[1, 1] = new PointF(1, 0);

            //_v[0, 0] = new PointF(2, 1);
            //_v[0, 1] = new PointF(2, 0);
            //_v[1, 0] = new PointF(1, 1);
            //_v[1, 1] = new PointF(1, 0);

            //cAoiCalibration.SetCalibrationPoints(_w, _v);
            //cAoiCalibration.CalculateTransformMatrix();

            //PointF w1 = new PointF(1, 1);
            //PointF v1 = new PointF(0, 0);
            //cAoiCalibration.TransformViewToWorld(w1, out v1);


        }
        public static void Close()
        {

            if (CAMERAS != null)
            {
                int i = 0;
                while (i < CameraConfig.Instance.COUNT)
                {
                    //CAMERAS[i] = new CAMERAClass();
                    //CAMERAS[i].Initial(CameraConfig.Instance.cameras[i].ToCameraString());
                    CAMERAS[i].Close();
                    i++;
                }
            }

            //CCDCollection.Close();

            //MACHINECollection.MACHINE.PLCCollection[0].Close();
            MACHINECollection.Close();

            switch (VERSION)
            {
                case VersionEnum.TRAVELLER:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_LS:

                            //LEClass.Instance.Close();

                            IxLineScan.Close();

                            break;
                    }

                    break;
            }


            

        }

        /// <summary>
        /// 读出参数
        /// </summary>
        /// <param name="myArray">out 传入的集合</param>
        /// <param name="st_File">读哪个文件</param>
        /// <returns></returns>
        static bool Read(out ArrayList myArray, string st_File)
        {
            try
            {
                System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Stream stream = new FileStream(st_File, FileMode.Open);
                myArray = (ArrayList)formater.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                GC.Collect();//强制进行拉圾回收
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
                myArray = null;
                GC.Collect();//强制进行拉圾回收
                return false;
            }

        }
        /// <summary>
        /// 记录参数
        /// </summary>
        /// <param name="mylist">需记录的集合</param>
        /// <param name="st_File">存放的路径</param>
        /// <returns></returns>
        static bool Write(ArrayList mylist, string st_File)
        {
            try
            {
                FileStream fs = new FileStream(st_File, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, mylist);
                fs.Close();

                fs.Dispose();
                GC.Collect();//强制进行拉圾回收
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
                GC.Collect();//强制进行拉圾回收
                return false;
            }
        }


    }
}
