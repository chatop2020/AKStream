using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using LibCommon.Structs;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.GB28181.XML;
using LibLogger;
using SIPSorcery.SIP;

namespace LibCommon
{
    public static class GCommon
    {
        private static string _outConfigPath = "";
        private static string _outLogPath = "";
        private static LiteDBHelper _ldb = new LiteDBHelper();
        private static List<VideoChannelRecordInfo> _videoChannelRecordInfo = new List<VideoChannelRecordInfo>();
        public static string BaseStartPath = Environment.CurrentDirectory; //程序启动的目录

        public static string
            BaseStartFullPath = Process.GetCurrentProcess().MainModule.FileName; //程序启动的全路径

        public static string? WorkSpacePath = AppDomain.CurrentDomain.BaseDirectory; //程序运行的目录
        public static string? WorkSpaceFullPath = Environment.GetCommandLineArgs()[0]; //程序运行的全路径
        public static string? CommandLine = Environment.CommandLine; //程序启动命令
        public static string ConfigPath = BaseStartPath + "/Config/";
        public static string TmpPicsPath = BaseStartPath + "/.tmppics/"; //用于截图缓存
        private static Logger _logger = null;
       

        public static Logger Logger
        {
            get => _logger;
            set => _logger = value;
        }


        /// <summary>
        /// 外部传入的配置文件所在目录路径
        /// </summary>
        public static string OutConfigPath
        {
            get => _outConfigPath;
            set => _outConfigPath = value;
        }

        /// <summary>
        /// 外部传入的日志文件所在目录路径
        /// </summary>
        public static string OutLogPath
        {
            get => _outLogPath;
            set => _outLogPath = value;
        }


        public static void InitLogger()
        {
            if (!string.IsNullOrEmpty(OutLogPath))
            {
                Logger.logxmlPath = OutLogPath;
            }

            _logger = new Logger();
        }

        static GCommon()
        {
            if (!string.IsNullOrEmpty(OutLogPath))
            {
                if (!OutLogPath.Trim().EndsWith('/'))
                {
                    OutLogPath += "/";
                }

                Logger.logxmlPath = OutLogPath;
            }

            //使用CodePagesEncodingProvider去注册扩展编码,以支持utf-x以外的字符集
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (!Directory.Exists(ConfigPath)) //如果配置文件目录不存在，则创建目录
            {
                Directory.CreateDirectory(ConfigPath);
            }

            if (!Directory.Exists(TmpPicsPath))
            {
                Directory.CreateDirectory(TmpPicsPath);
            }

            //初始化错误代码
            ErrorMessage.Init();
        }

        public static LiteDBHelper Ldb
        {
            get => _ldb;
            set => _ldb = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static List<VideoChannelRecordInfo> VideoChannelRecordInfo
        {
            get => _videoChannelRecordInfo;
            set => _videoChannelRecordInfo = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region 各类事件委托

        /// <summary>
        /// 踢掉掉线的sip设备
        /// </summary>
        /// <param name="guid"></param>
        public delegate void DoKickSipDevice(SipDevice sipDevice);

        /// <summary>
        /// 当sip设备注册时
        /// </summary>
        /// <param name="sipDeviceJson"></param>
        public delegate void RegisterDelegate(string sipDeviceJson);

        /// <summary>
        /// 当sip设备注销时
        /// </summary>
        /// <param name="sipDeviceJson"></param>
        public delegate void UnRegisterDelegate(string sipDeviceJson);

        /// <summary>
        /// 音视频点播完成（结束）
        /// </summary>
        /// <param name="record"></param>
        public delegate void InviteHistroyVideoFinished(RecordInfo.RecItem record);

        /// <summary>
        /// 当收到心跳数据时
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="keepAliveTime"></param>
        /// <param name="lostTimes"></param>
        public delegate void KeepaliveReceived(string deviceId, DateTime keepAliveTime, int lostTimes);

        /// <summary>
        /// 设备就绪通知
        /// </summary>
        /// <param name="sipDevice"></param>
        public delegate void SipDeviceReadyReceived(SipDevice sipDevice);

        /// <summary>
        /// 当收到设备目录时
        /// </summary>
        /// <param name="sipChannel"></param>
        public delegate void CatalogReceived(SipChannel sipChannel);

        /// <summary>
        /// 当设备报警订阅时
        /// </summary>
        /// <param name="sipTransaction"></param>
        public delegate void DeviceAlarmSubscribeDelegate(SIPTransaction sipTransaction);

        /// <summary>
        /// 获取设备状态时
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="deviceStatus"></param>
        public delegate void DeviceStatusReceived(SipDevice sipDevice, DeviceStatus deviceStatus);

        /// <summary>
        /// 收到设备信息时
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="deivceInfo"></param>
        public delegate void DeviceInfoReceived(SipDevice sipDevice, DeviceInfo deivceInfo);

        /// <summary>
        /// 当设备注册时需要鉴权时，返回为此设备外部指定的鉴权密钥
        /// </summary>
        /// <param name="sipDeviceId"></param>
        public delegate string DeviceAuthentication(string sipDeviceId);

        /// <summary>
        /// Sip客户端收到Sip服务端的消息时
        /// </summary>
        public delegate void RecvSipServerMsg(string msg, DateTime dateTime);

        /// <summary>
        /// 当有实时流推流请求时
        /// </summary>
        public delegate bool InviteChannel(ShareInviteInfo info, out ResponseStruct rs);

        /// <summary>
        /// 当有实时流结束推流请求时
        /// </summary>
        public delegate bool DeInviteChannel(string fromTag, string toTag, string callid, out ResponseStruct rs,
            out ShareInviteInfo info);

        #endregion
    }
}