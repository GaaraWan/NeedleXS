using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eazy_Project_III
{
    public enum ModuleName : int
    {
        [Description("拾取模组")]
        /// <summary>
        /// 前3轴模组 校正平面 拾取玻璃 等操作
        /// </summary>
        MODULE_PICK = 0,
        [Description("点胶模组")]
        /// <summary>
        /// 中间3轴模组 用于点胶
        /// </summary>
        MODULE_DISPENSING = 1,
        [Description("微调模组")]
        /// <summary>
        /// 后3轴模组 用于调整投影对位 角度
        /// </summary>
        MODULE_ADJUST = 2,
    }
    public enum MainS1State : int
    {
        [Description("跑綫中")]
        S1_RUNNING = 0,
        [Description("待機")]
        S1_READY = 1,
        [Description("復位中")]
        S1_RESETING = 2,

        [Description("计时开始")]
        LS_START = 3,
        [Description("计时停止")]
        LS_STOP = 4,
    }
    public enum ProductMode : int
    {
        [Description("預設模式")]
        DEFAULT =0,
        [Description("僅做2模式")]
        ONLY2 =1,
    }

    public enum TrackArea : int
    {
        COUNT=4,

        [Description("轨道PASS区")]
        TrackPASS = 0,
        [Description("轨道检测区")]
        TrackINSPECT = 1,
        [Description("轨道NG1区")]
        TrackNG1 = 2,
        [Description("轨道NG2区")]
        TrackNG2 = 3,
    }

    public enum SwicthOnOff : int
    {

        COUNT=2,

        False = 0,
        True = 1,
    }

    /// <summary>
    /// 轨道模组
    /// </summary>
    public enum TrackModule : int
    {
        COUNT = 24,

        NONE = -1,

        [Description("龙门左取料")]
        ML_TAKE = 0,
        [Description("龙门左放料")]
        ML_PUT = 1,

        [Description("龙门右取料1")]
        MR1_TAKE = 2,
        [Description("龙门右放料1")]
        MR1_PUT = 3,

        [Description("龙门右取料2")]
        MR2_TAKE = 4,
        [Description("龙门右放料2")]
        MR2_PUT = 5,

        /// <summary>
        /// 轨道1供料
        /// </summary>
        [Description("轨道1供料")]
        M1_FEED = 6,
        /// <summary>
        /// 轨道1放料
        /// </summary>
        [Description("轨道1放料")]
        M1_PUT = 7,

        /// <summary>
        /// 轨道2线扫
        /// </summary>
        [Description("轨道2线扫")]
        M2_LINE = 8,
        /// <summary>
        /// 轨道2供料1
        /// </summary>
        [Description("轨道2供料1")]
        M2_FEED1 = 9,
        /// <summary>
        /// 轨道2放料1
        /// </summary>
        [Description("轨道2放料1")]
        M2_PUT1 = 10,
        /// <summary>
        /// 轨道2供料2
        /// </summary>
        [Description("轨道2供料2")]
        M2_FEED2 = 11,
        /// <summary>
        /// 轨道2放料2
        /// </summary>
        [Description("轨道2放料2")]
        M2_PUT2 = 12,

        /// <summary>
        /// 轨道3供料
        /// </summary>
        [Description("轨道3供料")]
        M3_FEED = 13,
        /// <summary>
        /// 轨道3放料
        /// </summary>
        [Description("轨道3放料")]
        M3_PUT = 14,

        /// <summary>
        /// 轨道4供料
        /// </summary>
        [Description("轨道4供料")]
        M4_FEED = 15,
        /// <summary>
        /// 轨道4放料
        /// </summary>
        [Description("轨道4放料")]
        M4_PUT = 16,

        /// <summary>
        /// 龙门左取料多数据
        /// </summary>
        [Description("龙门左取料多数据")]
        HBO_L_TAKE = 17,
        /// <summary>
        /// 龙门左放料多数据
        /// </summary>
        [Description("龙门左放料多数据")]
        HBO_L_PUT = 18,


        /// <summary>
        /// 模组1供收料
        /// </summary>
        [Description("模组1供收料")]
        HBO_XG1 = 19,
        /// <summary>
        /// 模组2-1供收料
        /// </summary>
        [Description("模组2-1供收料")]
        HBO_XG2_1 = 20,
        /// <summary>
        /// 模组2-2供收料
        /// </summary>
        [Description("模组2-2供收料")]
        HBO_XG2_2 = 21,
        /// <summary>
        /// 模组3供收料
        /// </summary>
        [Description("模组3供收料")]
        HBO_XG3 = 22,
        /// <summary>
        /// 模组4供收料
        /// </summary>
        [Description("模组4供收料")]
        HBO_XG4 = 23,

    }
    /// <summary>
    /// 轨道搬运区域
    /// </summary>
    public enum TrackAreaPosition : int
    {
        COUNT = 4,

        /// <summary>
        ///  轨道1
        /// </summary>
        LEFT_T1 = 0,
        /// <summary>
        /// 原始盘 轨道2
        /// </summary>
        LEFT_T2 = 1,
        ///// <summary>
        ///// 右边原始盘 轨道2
        ///// </summary>
        //RIGHT_T2 = 2,
        /// <summary>
        ///  轨道3
        /// </summary>
        RIGHT_T3 = 2,
        /// <summary>
        ///  轨道4
        /// </summary>
        RIGHT_T4 = 3,
    }
    class Enums
    {
    }
    public enum LSTestResult : int
    {
        [Browsable(false)]
        COUNT = 11,

        [Description("PASS")]
        PASS = 0,
        [Description("err上")]
        ERR_TOP = 1,
        [Description("err下")]
        ERR_BOTTOM = 2,
        [Description("err左")]
        ERR_LEFT = 3,
        [Description("err右")]
        ERR_RIGHT = 4,
        [Description("err左上")]
        ERR_LT = 5,
        [Description("err右上")]
        ERR_RT = 6,
        [Description("err左下")]
        ERR_LB = 7,
        [Description("err右下")]
        ERR_RB = 8,
        [Description("方向错误")]
        ERR_BIN9 = 9,
        [Description("未知")]
        ERR_NONE = 10,
    }
    public enum BorderTypeEnum : int
    {
        COUNT = 4,

        LEFT = 0,
        TOP = 1,
        RIGHT = 2,
        BOTTOM = 3,
    }
    public enum CornerEnum : int
    {
        COUNT = 4,

        LT = 0,
        LB = 1,
        RT = 2,
        RB = 3,

        NONE = -1,
    }
    public enum InspectMode : int
    {
        [Description("检测DataMatrix")]
        DATAMATRIX =0,
        [Description("检测芯片")]
        CHIP =1,
    }
    public enum CornerNoMarkEnum : int
    {
        [Browsable(false)]
        COUNT = 4,

        /// <summary>
        /// 左上
        /// </summary>
        [Description("左上")]
        LT = 0,
        /// <summary>
        /// 右上
        /// </summary>
        [Description("右上")]
        RT = 1,
        /// <summary>
        /// 左下
        /// </summary>
        [Description("左下")]
        LB = 2,
        /// <summary>
        /// 右下
        /// </summary>
        [Description("右下")]
        RB = 3,

        //NONE = -1,
    }
    

}
