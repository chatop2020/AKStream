using System;

namespace LibCommon.Enums
{
    [Serializable]
    /// <summary>
    /// 云台控制命令
    /// </summary>
    public enum PTZCommandType : int
    {
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 0,

        /// <summary>
        /// 上
        /// </summary>
        Up = 1,

        /// <summary>
        /// 左上
        /// </summary>
        UpLeft = 2,

        /// <summary>
        /// 右下
        /// </summary>
        UpRight = 3,

        /// <summary>
        /// 下
        /// </summary>
        Down = 4,

        /// <summary>
        /// 左下
        /// </summary>
        DownLeft = 5,

        /// <summary>
        /// 右下
        /// </summary>
        DownRight = 6,

        /// <summary>
        /// 左
        /// </summary>
        Left = 7,

        /// <summary>
        /// 右
        /// </summary>
        Right = 8,

        /// <summary>
        /// 聚焦+
        /// </summary>
        Focus1 = 9,

        /// <summary>
        /// 聚焦-
        /// </summary>
        Focus2 = 10,

        /// <summary>
        /// 变倍+
        /// </summary>
        Zoom1 = 11,

        /// <summary>
        /// 变倍-
        /// </summary>
        Zoom2 = 12,

        /// <summary>
        /// 光圈开
        /// </summary>
        Iris1 = 13,

        /// <summary>
        /// 光圈关
        /// </summary>
        Iris2 = 14,

        /// <summary>
        /// 设置预置位
        /// </summary>
        SetPreset = 15,

        /// <summary>
        /// 调用预置位
        /// </summary>
        GetPreset = 16,

        /// <summary>
        /// 删除预置位
        /// </summary>
        RemovePreset = 17,

        /// <summary>
        /// 未知
        /// </summary>
        UnKnow = 18,
    }
}