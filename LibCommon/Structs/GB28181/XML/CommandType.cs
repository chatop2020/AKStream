namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 命令类型
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 心跳
        /// </summary>
        Keepalive = 1,

        /// <summary>
        /// 设备目录
        /// </summary>
        Catalog = 2,

        /// <summary>
        /// 视频直播
        /// </summary>
        Play = 3,

        /// <summary>
        /// 录像点播
        /// </summary>
        Playback = 4,

        /// <summary>
        /// 设备控制
        /// </summary>
        DeviceControl = 5,

        /// <summary>
        /// 警告通知
        /// </summary>
        Alarm = 6,

        /// <summary>
        /// 设备信息
        /// </summary>
        DeviceInfo = 7,

        /// <summary>
        /// 设备状态
        /// </summary>
        DeviceStatus = 8,

        /// <summary>
        /// 文件检索
        /// </summary>
        RecordInfo = 9,

        /// <summary>
        /// 文件下载
        /// </summary>
        Download = 10,

        /// <summary>
        /// 设备配置
        /// </summary>
        ConfigDownload = 11,

        /// <summary>
        /// 语音广播
        /// </summary>
        Broadcast = 12,

        /// <summary>
        /// 预置位查询
        /// </summary>
        PresetQuery = 13,

        MobilePosition = 14,

        /// <summary>
        /// 设备配置
        /// </summary>
        DeviceConfig = 15,

        /// <summary>
        /// 媒体结束通知
        /// </summary>
        MediaStatus = 16
    }
}