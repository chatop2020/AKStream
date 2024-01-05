using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using AKStreamKeeper.AutoTask;
using AKStreamKeeper.Misc;
using AKStreamKeeper.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using LibSystemInfo;
using Newtonsoft.Json;
using JsonHelper = LibCommon.JsonHelper;
using Timer = System.Timers.Timer;

namespace AKStreamKeeper
{
    public static class Common
    {
        public delegate void MediaServerKilled(bool self = false);


        private static string _configPath = GCommon.ConfigPath + "AKStreamKeeper.json";
        private static AKStreamKeeperConfig _akStreamKeeperConfig;
        private static SystemInfo _keeperSystemInfo = new SystemInfo();
        public static PerformanceInfo KeeperPerformanceInfo = new PerformanceInfo();
        private static object _performanceInfoLock = new object();
        private static Timer _perFormanceInfoTimer;
        private static string _loggerHead = "AKStreamKeeper";
        private static DateTime _getDiskSpaceToRecordMapTick = DateTime.Now;
        private static DateTime _sendDataTick = DateTime.Now;
        private static ulong _timerCount = 0;
        private static string _oldMediaServerId = "";
        private static Dictionary<string, int> _disksUseable = new Dictionary<string, int>();
        private static DiskUseableChecker _diskUseableChecker;

        public static string OldMediaServerId
        {
            get => _oldMediaServerId;
            set => _oldMediaServerId = value;
        }

        private static List<KeyValuePair<double, string>> _akStreamDiskInfoOfRecordMap =
            new List<KeyValuePair<double, string>>();

        private static object _akStreamDiskInfoOfRecordMapLock = new object();
        public static object _getRtpPortLock = new object();
        private static long _counter1 = 0;
        private static bool _firstPost = true;
        public static string CutOrMergePath = GCommon.BaseStartPath + "/CutMergeFile/";
        public static string CutOrMergeTempPath = GCommon.BaseStartPath + "/CutMergeTempDir/";
        public static DateTime StartupDateTime;
        public static AutoRtpPortClean AutoRtpPortClean;

        public static Timer PerFormanceInfoTimer
        {
            get => _perFormanceInfoTimer;
            set => _perFormanceInfoTimer = value;
        }

        /// <summary>
        /// 申请的rtp端口放在这里，并记录时间，在一定时间内不允许使用，端口需要要冷却（20秒内）
        /// </summary>
        public static List<PortInfo> PortInfoList = new List<PortInfo>();

        /// <summary>
        /// 申请的rtp端口(发送)放在这里，并记录时间，在一定时间内不允许使用，端口需要要冷却（20秒内）
        /// </summary>
        public static List<PortInfo> PortInfoListForSender = new List<PortInfo>();

        public static int FFmpegThreadCount = 2;

        /// <summary>
        /// 调试模式，不判断accesskey
        /// </summary>
        public static bool IsDebug = false;

        public static MediaServerInstance MediaServerInstance;

        /// <summary>
        /// 挂载的硬盘是否可用
        /// </summary>
        public static Dictionary<string, int> DisksUseable
        {
            get => _disksUseable;
            set => _disksUseable = value;
        }

        static Common()
        {
#if (DEBUG)
            IsDebug = true;
#endif

            StartupDateTime = DateTime.Now;
            CutMergeService.start = true;
            // MediaServerInstance.OnMediaKilled += OnMediaServerKilled;//貌似这里会造成timer中的启动冲突，所以先去掉这里
        }

        public static string Version // 版本号
        {
            get
            {
                var md5 = UtilsHelper.Md5WithFile(GCommon.WorkSpaceFullPath);
                var crc32 = CRC32Helper.GetCRC32(md5);
                return crc32.ToString("x2").ToUpper();
            }
        }


        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string ConfigPath
        {
            get => _configPath;
            set => _configPath = value;
        }

        /// <summary>
        /// 配置实例
        /// </summary>
        public static AKStreamKeeperConfig AkStreamKeeperConfig
        {
            get => _akStreamKeeperConfig;
            set => _akStreamKeeperConfig = value;
        }

        /// <summary>
        /// 日志头
        /// </summary>
        public static string LoggerHead
        {
            get => _loggerHead;
            set => _loggerHead = value;
        }

        /// <summary>
        /// 启动流媒体服务器
        /// </summary>
        /// <returns></returns>
        public static int StartupMediaServer()
        {
            ProcessHelper.KillProcess(_akStreamKeeperConfig.MediaServerPath);

            if (MediaServerInstance == null)
            {
                MediaServerInstance =
                    new MediaServerInstance(_akStreamKeeperConfig.MediaServerPath, AkStreamKeeperConfig);
            }

            return MediaServerInstance.Startup();
        }

        /// <summary>
        /// 流媒体服务器进程被结束时触发
        /// </summary>
        /// <param name="self"></param>
        private static void OnMediaServerKilled(bool self = false)
        {
            if (!self)
            {
                GCommon.Logger.Error(
                    $"[{LoggerHead}]->流媒体服务器进程被意外关闭->1秒后重新尝试启动流媒体服务器");
                Thread.Sleep(1000);
                while (StartupMediaServer() <= 0)
                {
                    GCommon.Logger.Error(
                        $"[{LoggerHead}]->尝试重新启动流媒体服务器启动失败，开始循环尝试，直至启动成功");
                    Thread.Sleep(1000);
                }

                GCommon.Logger.Info(
                    $"[{LoggerHead}]->尝试重新启动流媒体服务器启动成功->进程ID:{MediaServerInstance.GetPid()}");
            }
        }

        private static void GetDiskSpaceToRecordMap()
        {
            var tmpList = new List<KeyValuePair<double, string>>();
            foreach (var path in _akStreamKeeperConfig.CustomRecordPathList)
            {
                var obj = KeeperPerformanceInfo.DriveInfo.FindLast(x => x.Name.Trim().Equals(path));
                if (obj != null)
                {
                    tmpList.Add(new KeyValuePair<double, string>((double)obj.Free, path));
                }
                else
                {
                    if (KeeperPerformanceInfo.SystemType.Trim().ToUpper().Equals("WINDOWS"))
                    {
                        var rootPath = Path.GetPathRoot(path).ToUpper();
                        rootPath = rootPath.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
                        rootPath = rootPath.Split('\\', StringSplitOptions.RemoveEmptyEntries)[0];
                        rootPath = rootPath.TrimEnd(':');
                        foreach (var drv in KeeperPerformanceInfo.DriveInfo)
                        {
                            if (drv != null && drv.IsReady == true && !string.IsNullOrEmpty(drv.Name))
                            {
                                var drvPath = Path.GetPathRoot(drv.Name).ToUpper();
                                drvPath = drvPath.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
                                drvPath = drvPath.Split('\\', StringSplitOptions.RemoveEmptyEntries)[0];
                                drvPath = drvPath.TrimEnd(':');
                                if (drvPath.Equals(rootPath))
                                {
                                    tmpList.Add(new KeyValuePair<double, string>((double)drv.Free, path));
                                }
                            }
                        }
                    }
                    else
                    {
                        DriveInfoDiy objUnix = null;
                        if (path.Trim().Equals("/"))
                        {
                            objUnix = KeeperPerformanceInfo.DriveInfo.FindLast(x =>
                                x.Name.Trim().Equals("/"));
                            if (objUnix != null)
                            {
                                tmpList.Add(new KeyValuePair<double, string>((double)objUnix.Free, path));
                            }
                        }
                        else
                        {
                            char[] tmpChars = path.ToCharArray(0, path.Length);
                            List<KeyValuePair<double, string>> subTmpList = new List<KeyValuePair<double, string>>();
                            for (int i = 0; i <= tmpChars.Length - 1; i++)
                            {
                                string tmpStr = "";
                                if (tmpChars[i] == '/')
                                {
                                    if (i == 0)
                                    {
                                        continue;
                                    }

                                    tmpStr = path.Substring(0, i);
                                    objUnix = KeeperPerformanceInfo.DriveInfo.FindLast(x =>
                                        x.Name.Trim().Equals(tmpStr.Trim()));
                                    if (objUnix != null)
                                    {
                                        subTmpList.Add(new KeyValuePair<double, string>((double)objUnix.Free, path));
                                        break;
                                    }
                                }
                            }

                            bool foundDir = false;
                            for (int j = 0; j <= path.Length - 1; j++) //检查多级目录中至少有一层目录是存在的，除根目录以外
                            {
                                if (path[j] == '/')
                                {
                                    string tmpStrPath = path.Substring(0, j);
                                    if (Directory.Exists(tmpStrPath))
                                    {
                                        foundDir = true;
                                        break;
                                    }
                                }
                            }

                            if (subTmpList.Count == 0 && tmpChars[0] == '/' && foundDir)
                            {
                                //如果subTmpList是空的，并且首字符是根目录，同时还满足多级目录中至少有一级目录是存在的情况下，将根目录作为其所在目录获取剩余容量
                                objUnix = KeeperPerformanceInfo.DriveInfo.FindLast(x =>
                                    x.Name.Trim().Equals("/"));
                                if (objUnix != null)
                                {
                                    subTmpList.Add(new KeyValuePair<double, string>((double)objUnix.Free, path));
                                }
                            }

                            tmpList.AddRange(subTmpList);
                        }
                    }
                }
            }

            _akStreamDiskInfoOfRecordMap = tmpList;
            UtilsHelper.RemoveNull(_akStreamDiskInfoOfRecordMap); //去null
            _akStreamDiskInfoOfRecordMap = _akStreamDiskInfoOfRecordMap.Distinct().ToList(); //去重
            for (int i = _akStreamKeeperConfig.CustomRecordPathList.Count - 1;
                 i >= 0;
                 i--)
            {
                //保证去除完全不存在的目录
                string dir = _akStreamKeeperConfig.CustomRecordPathList[i];
                var objExt = _akStreamDiskInfoOfRecordMap.FindLast(x => x.Value.Trim().Equals(dir.Trim()));
                if (objExt.Value == null)
                {
                    _akStreamKeeperConfig.CustomRecordPathList[i] = null;
                }
            }

            UtilsHelper.RemoveNull(_akStreamKeeperConfig.CustomRecordPathList); //去null
            _akStreamKeeperConfig.CustomRecordPathList =
                _akStreamKeeperConfig.CustomRecordPathList.Distinct().ToList(); //去重
        }

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static bool InitConfig(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            _akStreamKeeperConfig = new AKStreamKeeperConfig();


            int i = 0;
            while (KeeperPerformanceInfo == null && KeeperPerformanceInfo.NetWorkStat == null && i < 50)
            {
                i++;
                Thread.Sleep(20);
            }

            string macAddr = "";
            if (KeeperPerformanceInfo != null && KeeperPerformanceInfo.NetWorkStat != null)
            {
                macAddr = KeeperPerformanceInfo.NetWorkStat.Mac;
            }

            if (string.IsNullOrEmpty(macAddr))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_GetMacAddressExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_GetMacAddressExcept],
                };
                return false; //mac 地址没找到了，报错出去
            }

            IPInfo ipInfo = UtilsHelper.GetIpAddressByMacAddress(macAddr, true);
            if (ipInfo != null)
            {
                _akStreamKeeperConfig.IpV4Address = ipInfo.IpV4;
                _akStreamKeeperConfig.IpV6Address = ipInfo.IpV6;
                _akStreamKeeperConfig.WebApiPort = 6880;
                _akStreamKeeperConfig.UseSsl = false;
                _akStreamKeeperConfig.ZLMediakitSSLFilePath = "";
                _akStreamKeeperConfig.MaxRtpPort = 20000;
                _akStreamKeeperConfig.MinRtpPort = 10001;
                _akStreamKeeperConfig.MinSendRtpPort = 20002;
                _akStreamKeeperConfig.MaxSendRtpPort = 20200;
                _akStreamKeeperConfig.RandomPort = false;
                _akStreamKeeperConfig.RecordSec = 120; //时长120秒
                _akStreamKeeperConfig.FFmpegPath = "./ffmpeg";
                _akStreamKeeperConfig.RtpPortCdTime = 3600;
                _akStreamKeeperConfig.HttpClientTimeoutSec = 20;
                _akStreamKeeperConfig.AccessKey = UtilsHelper.GeneralGuid();
                _akStreamKeeperConfig.AkStreamWebRegisterUrl =
                    $"http://127.0.0.1:5800/MediaServer/WebHook/MediaServerKeepAlive";
                _akStreamKeeperConfig.CutMergeFilePath = "/disk1/record";
                _akStreamKeeperConfig.CustomRecordPathList = new List<string>();
                _akStreamKeeperConfig.CustomRecordPathList.Add("请正确配置用于存储录制文件的目录或盘符");
                _akStreamKeeperConfig.CustomRecordPathList.Add("目前可用设备如下:");
                foreach (var disk in KeeperPerformanceInfo.DriveInfo)
                {
                    string tmp = "目标名称:" + disk.Name + "    空闲空间:" + disk.Free + "    总空间:" + disk.Total +
                                 "    可用空间率:" + disk.FreePercent + "%" + "    是否就绪:" +
                                 disk.IsReady;
                    _akStreamKeeperConfig.CustomRecordPathList.Add(tmp);
                }

                _akStreamKeeperConfig.CustomRecordPathList.Add("配置时仅输入目标名称，支持多个，按照Json格式一行一个");
                _akStreamKeeperConfig.CustomRecordPathList.Add("例:");
                _akStreamKeeperConfig.CustomRecordPathList.Add("/disk1/record");
                _akStreamKeeperConfig.CustomRecordPathList.Add("/disk2/record");
                _akStreamKeeperConfig.CustomRecordPathList.Add("/disk3/record");
                _akStreamKeeperConfig.MediaServerPath =
                    "请正确填写流媒体服务器(ZLMediaKit的MediaServer)的绝对路径，如/opt/MediaServer，并确保其已经执行过一次，在MediaServer的同级目录下已经生成了config.ini文件";
                try
                {
                    string configStr = JsonHelper.ToJson(_akStreamKeeperConfig, Formatting.Indented);
                    File.WriteAllText(_configPath, configStr);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ConfigNotReady,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                    };
                    return true;
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_JsonWriteExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonWriteExcept],
                    };
                    return false;
                }
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_GetIpAddressExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_GetIpAddressExcept],
                };
                return false;
            }
        }


        /// <summary>
        /// 检测流媒体服务器的配置文件是否存在
        /// </summary>
        /// <param name="MediaServerBinPath"></param>
        /// <returns></returns>
        private static string CheckMediaServerConfig(string MediaServerBinPath)
        {
            string dir = Path.GetDirectoryName(MediaServerBinPath) + "/";
            if (Directory.Exists(dir))
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                if (di != null)
                {
                    foreach (var file in di.GetFiles())
                    {
                        if (file != null && file.Extension.ToLower().Equals(".ini"))
                        {
                            return file.FullName;
                        }
                    }
                }
            }

            return null!;
        }


        /// <summary>
        /// 检测配置文件是否正常
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static bool CheckConfig(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (_akStreamKeeperConfig == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ConfigNotReady,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                };
                return false;
            }

            if (!UtilsHelper.IsPortOK(_akStreamKeeperConfig.WebApiPort.ToString()))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_NetworkPortExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_NetworkPortExcept],
                };
                return false;
            }


            if (_akStreamKeeperConfig.CustomRecordPathList != null &&
                _akStreamKeeperConfig.CustomRecordPathList.Count > 0)
            {
                foreach (var path in _akStreamKeeperConfig.CustomRecordPathList)
                {
                    if (!Directory.Exists(path))
                    {
                        try
                        {
                            var ret = Directory.CreateDirectory(path);
                            if (ret == null)
                            {
                                rs = new ResponseStruct()
                                {
                                    Code = ErrorNumber.Sys_DiskInfoExcept,
                                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DiskInfoExcept],
                                };
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_DiskInfoExcept,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DiskInfoExcept],
                                ExceptMessage = ex.Message,
                                ExceptStackTrace = ex.StackTrace,
                            };
                            return false;
                        }
                    }
                }
            }

            if (!File.Exists(_akStreamKeeperConfig.MediaServerPath))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_BinNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_BinNotFound],
                };
                return false;
            }

            string MediaServerConfig = CheckMediaServerConfig(_akStreamKeeperConfig.MediaServerPath);
            if (string.IsNullOrEmpty(MediaServerConfig))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_ConfigNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ConfigNotFound],
                };
                return false;
            }

            if (!UtilsHelper.IsUrl(_akStreamKeeperConfig.AkStreamWebRegisterUrl))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_UrlExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_UrlExcept],
                };
                return false;
            }

            return true;
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static bool ReadConfig(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            lock (_performanceInfoLock)
            {
                KeeperPerformanceInfo = _keeperSystemInfo.GetSystemInfoObject();
            }


            if (!File.Exists(_configPath))
            {
                //创建文件 
                var ret = InitConfig(out rs);
                if (ret == false || !rs.Code.Equals(ErrorNumber.None))
                {
                    return false;
                }

                return true;
            }
            else
            {
                //读取文件
                try
                {
                    _akStreamKeeperConfig = JsonHelper.FromJson<AKStreamKeeperConfig>(File.ReadAllText(_configPath));
                    if (_akStreamKeeperConfig.RecordSec == null || _akStreamKeeperConfig.RecordSec == 0) //用于补前面没有的配置项
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && File.Exists("/etc/hostname"))
                        {
                            var text = File.ReadAllText("/etc/hostname").Trim().ToLower();
                            if (text.Contains("gdn") || text.Contains("guardian") || text.Contains("rasp"))
                            {
                                _akStreamKeeperConfig.CheckLinuxDiskMount = true;
                                _akStreamKeeperConfig.RecordSec = 120;
                                var _jsonText = JsonHelper.ToJson(_akStreamKeeperConfig, Formatting.Indented);
                                if (!string.IsNullOrEmpty(_jsonText))
                                {
                                    File.WriteAllText(_configPath, _jsonText);
                                    _akStreamKeeperConfig =
                                        JsonHelper.FromJson<AKStreamKeeperConfig>(File.ReadAllText(_configPath));
                                }
                            }
                        }
                    }

                    _akStreamKeeperConfig.CustomRecordPathList =
                        _akStreamKeeperConfig.CustomRecordPathList.Distinct().ToList(); //去重

                    for (int i = 0; i <= _akStreamKeeperConfig.CustomRecordPathList.Count - 1; i++)
                    {
                        while (_akStreamKeeperConfig.CustomRecordPathList[i].Trim().EndsWith('/') &&
                               _akStreamKeeperConfig.CustomRecordPathList[i].Trim().Length > 1)
                        {
                            _akStreamKeeperConfig.CustomRecordPathList[i] =
                                _akStreamKeeperConfig.CustomRecordPathList[i].TrimEnd('/');
                        }
                    }

                    GetDiskSpaceToRecordMap();
                    var ret = CheckConfig(out rs);
                    if (ret)
                    {
                        File.Delete(_configPath + "_bak");
                        File.Copy(_configPath, _configPath + "_bak");
                    }


                    return ret;
                }
                catch (Exception ex)
                {
                    var tmpStr = "";
                    if (File.Exists(_configPath + "_bak"))
                    {
                        File.Delete(_configPath);
                        File.Copy(_configPath + "_bak", _configPath);
                        tmpStr = "已经将[" + _configPath + "]文件恢复到上一次正常时的状态，请重新执行尝试解决问题";
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_JsonReadExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonReadExcept],
                        ExceptMessage = ex.Message + "\r\n" + tmpStr,
                        ExceptStackTrace = ex.StackTrace,
                    };
                }
            }

            return false;
        }

        private static void InitTimer()
        {
            if (_perFormanceInfoTimer == null)
            {
                _perFormanceInfoTimer = new Timer(1000);
                _perFormanceInfoTimer.Enabled = true; //启动Elapsed事件触发
                _perFormanceInfoTimer.Elapsed += OnTimedEvent; //添加触发事件的函数
                _perFormanceInfoTimer.AutoReset = true; //需要自动reset
                _perFormanceInfoTimer.Stop(); //防止timer重入
            }
        }


        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _timerCount++;
            GCommon.Logger.Debug(
                $"[{LoggerHead}]->Common.OnTimedEvent运行中...({_timerCount})");
            TimeSpan ts = DateTime.Now.Subtract(StartupDateTime);

            if (MediaServerInstance == null || !MediaServerInstance.IsRunning)
            {
                GCommon.Logger.Warn(
                    $"[{LoggerHead}]->流媒体服务器(MediaServer)未启动或流媒体实例不存在，立即创建或启动...");
                _perFormanceInfoTimer.Stop(); //防止timer重入
                var _pid = StartupMediaServer();

                if (_pid > 0)
                {
                    GCommon.Logger.Warn(
                        $"[{LoggerHead}]->流媒体服务器(MediaServer)实例创建并启动成功...{_pid}");
                }
                else
                {
                    GCommon.Logger.Warn(
                        $"[{LoggerHead}]->流媒体服务器(MediaServer)创建或启动失败...下个循环再试");
                }

                _perFormanceInfoTimer.Start();

                return;
            }

            lock (_performanceInfoLock)
            {
                KeeperPerformanceInfo = _keeperSystemInfo.GetSystemInfoObject();
                KeeperPerformanceInfo.UpTimeSec = ts.TotalSeconds;
            }


            if ((DateTime.Now - _getDiskSpaceToRecordMapTick).TotalMilliseconds >= 60000) //每60秒获取一次磁盘占用情况
            {
                lock (_akStreamDiskInfoOfRecordMapLock)
                {
                    GetDiskSpaceToRecordMap();
                }

                _getDiskSpaceToRecordMapTick = DateTime.Now;
            }


            if ((DateTime.Now - _sendDataTick).TotalMilliseconds >= 5000 && MediaServerInstance != null) //发心跳给服务器
            {
                ReqMediaServerKeepAlive tmpKeepAlive = new ReqMediaServerKeepAlive();

                if (_firstPost)
                {
                    tmpKeepAlive.FirstPost = true;
                    _firstPost = false;
                }
                else
                {
                    tmpKeepAlive.FirstPost = false;
                }

                tmpKeepAlive.RecordSec = MediaServerInstance.AkStreamKeeperConfig.RecordSec;
                tmpKeepAlive.Secret = MediaServerInstance.Secret;
                tmpKeepAlive.PerformanceInfo = KeeperPerformanceInfo;
                tmpKeepAlive.UseSsl = _akStreamKeeperConfig.UseSsl;
                if (string.IsNullOrEmpty(_akStreamKeeperConfig.IpV4Address))
                {
                    IPInfo ip = UtilsHelper.GetIpAddressByMacAddress(KeeperPerformanceInfo.NetWorkStat.Mac, true);
                    tmpKeepAlive.IpV4Address = ip.IpV4;
                    tmpKeepAlive.IpV6Address = ip.IpV6 == null ? "" : ip.IpV6;
                }
                else
                {
                    tmpKeepAlive.IpV4Address = _akStreamKeeperConfig.IpV4Address;
                    tmpKeepAlive.IpV6Address = !string.IsNullOrEmpty(_akStreamKeeperConfig.IpV6Address)
                        ? _akStreamKeeperConfig.IpV6Address
                        : "";
                }

                tmpKeepAlive.Candidate = MediaServerInstance.AkStreamKeeperConfig.Candidate;
                tmpKeepAlive.MediaServerId = MediaServerInstance.MediaServerId;
                tmpKeepAlive.MediaServerPid = MediaServerInstance.GetPid();
                tmpKeepAlive.RecordPathList = _akStreamDiskInfoOfRecordMap;
                tmpKeepAlive.RtpPortMax = _akStreamKeeperConfig.MaxRtpPort;
                tmpKeepAlive.RtpPortMin = _akStreamKeeperConfig.MinRtpPort;
                tmpKeepAlive.RandomPort = _akStreamKeeperConfig.RandomPort;
                tmpKeepAlive.ServerDateTime = DateTime.Now;
                tmpKeepAlive.ZlmHttpPort = MediaServerInstance.ZlmHttpPort;
                tmpKeepAlive.ZlmHttpsPort = MediaServerInstance.ZlmHttpsPort;
                tmpKeepAlive.ZlmRtmpPort = MediaServerInstance.ZlmRtmpPort;
                tmpKeepAlive.ZlmRtmpsPort = MediaServerInstance.ZlmRtmpsPort;
                tmpKeepAlive.ZlmRtspPort = MediaServerInstance.ZlmRtspPort;
                tmpKeepAlive.ZlmRtspsPort = MediaServerInstance.ZlmRtspsPort;
                tmpKeepAlive.KeeperWebApiPort = _akStreamKeeperConfig.WebApiPort;
                tmpKeepAlive.ZlmRecordFileSec = MediaServerInstance.ZlmRecordFileSec;
                tmpKeepAlive.AccessKey = _akStreamKeeperConfig.AccessKey;
                tmpKeepAlive.MediaServerIsRunning = MediaServerInstance.IsRunning;
                tmpKeepAlive.Version = Version;
                tmpKeepAlive.ZlmBuildDateTime = MediaServerInstance.ZlmBuildDateTime;
                tmpKeepAlive.CutMergeFilePath = _akStreamKeeperConfig.CutMergeFilePath;
                lock (DisksUseable)
                {
                    if (DisksUseable != null && DisksUseable.Count > 0)
                    {
                        tmpKeepAlive.DisksUseable = new Dictionary<string, int>();
                        foreach (var dic in DisksUseable)
                        {
                            tmpKeepAlive.DisksUseable.Add(dic.Key, dic.Value);
                        }
                    }
                    else
                    {
                        tmpKeepAlive.DisksUseable = null;
                    }
                }

                string reqData = JsonHelper.ToJson(tmpKeepAlive, Formatting.Indented);
#if (DEBUG)
                GCommon.Logger.Debug(
                    $"[{LoggerHead}]->向AKStreamWeb注册本机信息->{reqData}");
#endif
                try
                {
                    var httpRet = NetHelper.HttpPostRequest(_akStreamKeeperConfig.AkStreamWebRegisterUrl, null, reqData,
                        "utf-8", _akStreamKeeperConfig.HttpClientTimeoutSec * 1000);
                    _sendDataTick = DateTime.Now;
#if (DEBUG)
                    GCommon.Logger.Debug(
                        $"[{LoggerHead}]->AKStreamWeb注册本机信息回复内容->{httpRet}");
#endif

                    if (!string.IsNullOrEmpty(httpRet))
                    {
                        if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                        {
                            GCommon.Logger.Error(
                                $"[{LoggerHead}]->访问控制服务器时Http客户端超时或服务不可达");
                            return;
                        }

                        var resMediaServerKeepAlive = JsonHelper.FromJson<ResMediaServerKeepAlive>(httpRet);
                        if (resMediaServerKeepAlive != null)
                        {
                            if (resMediaServerKeepAlive.NeedRestartMediaServer)
                            {
                                GCommon.Logger.Info(
                                    $"[{LoggerHead}]->控制服务器反馈，要求重启流媒体服务器,马上重启");
                                MediaServerInstance.Restart();
                            }

                            if (resMediaServerKeepAlive.RecommendTimeSynchronization)
                            {
                                GCommon.Logger.Warn(
                                    $"[{LoggerHead}]->控制服务器反馈，流媒体服务器与控制服务器时间一致性过差，建议手工同步时间->控制服务器当前时间->{resMediaServerKeepAlive.ServerDateTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                            }
                        }
                        else
                        {
                            GCommon.Logger.Warn(
                                $"[{LoggerHead}]->访问控制服务器异常->\r\n{httpRet}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    GCommon.Logger.Error(
                        $"[{LoggerHead}]->与控制服务器保持心跳时异常->\r\n{ex.Message}\r\n{ex.StackTrace}");
                }
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (!string.IsNullOrEmpty(GCommon.OutConfigPath))
            {
                if (!GCommon.OutConfigPath.Trim().EndsWith('/'))
                {
                    GCommon.OutConfigPath += "/";
                }

                _configPath = GCommon.OutConfigPath + "AKStreamKeeper.json";
            }

            _configPath = UtilsHelper.FindPreferredConfigFile(_configPath); //查找优先使用的配置文件

            GCommon.Logger.Info(
                $"[{LoggerHead}]->Let's Go...");
            GCommon.Logger.Info(
                $"[{LoggerHead}]->程序版本标识:{Version}");
#if (DEBUG)
            Console.WriteLine("[Debug]\t当前程序为Debug编译模式");
            Console.WriteLine("[Debug]\t程序启动路径:" + GCommon.BaseStartPath);
            Console.WriteLine("[Debug]\t程序启动全路径:" + GCommon.BaseStartFullPath);
            Console.WriteLine("[Debug]\t程序运行路径:" + GCommon.WorkSpacePath);
            Console.WriteLine("[Debug]\t程序运行全路径:" + GCommon.WorkSpaceFullPath);
            Console.WriteLine("[Debug]\t程序启动命令:" + GCommon.CommandLine);
            IsDebug = true;
#endif
            ResponseStruct rs;
            InitTimer();
            var ret = ReadConfig(out rs);
            if (!ret || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Error(
                    $"[{LoggerHead}]->获取AKStreamKeeper配置文件时异常,系统无法运行->\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
                Environment.Exit(0); //退出程序 
            }

            ret = UtilsHelper.CheckFFmpegBin(_akStreamKeeperConfig.FFmpegPath);
            if (!ret)
            {
                GCommon.Logger.Error(
                    $"[{LoggerHead}]->检测发现FFmpeg可执行文件{_akStreamKeeperConfig.FFmpegPath}不存在或者无法正常运行,系统无法运行");
                Environment.Exit(0); //退出程序 
            }


            ProcessHelper.KillProcess(_akStreamKeeperConfig.MediaServerPath); //启动前先删除掉所有流媒体进程

            while (StartupMediaServer() <= 0)
            {
                GCommon.Logger.Error(
                    $"[{LoggerHead}]->流媒体服务器启动失败->1秒后重试");
                Thread.Sleep(1000);
            }

            _perFormanceInfoTimer.Start(); //启动计时器

            GCommon.Logger.Info(
                $"[{LoggerHead}]->流媒体服务器启动成功->进程ID:{MediaServerInstance.GetPid()}");

            AutoRtpPortClean = new AutoRtpPortClean(); //启动不使用rtp端口自动清理
            _diskUseableChecker = new DiskUseableChecker(); //启动磁盘挂载监控

            if (!string.IsNullOrEmpty(AkStreamKeeperConfig.CutMergeFilePath))
            {
                if (KeeperPerformanceInfo.SystemType.Trim().ToUpper().Equals("WINDOWS"))
                {
                    var tmp1 = AkStreamKeeperConfig.CutMergeFilePath.TrimEnd('\\') + '\\';
                    if (!Directory.Exists(tmp1))
                    {
                        Directory.CreateDirectory(tmp1);
                    }

                    var tmp2 = tmp1 + "CutMergeFile\\";
                    if (!Directory.Exists(tmp2))
                    {
                        Directory.CreateDirectory(tmp2);
                    }

                    CutOrMergePath = tmp2;
                    tmp2 = tmp1 + "CutMergeTempDir\\";
                    if (!Directory.Exists(tmp2))
                    {
                        Directory.CreateDirectory(tmp2);
                    }

                    CutOrMergeTempPath = tmp2;
                }
                else
                {
                    var tmp1 = AkStreamKeeperConfig.CutMergeFilePath.TrimEnd('/') + '/';
                    if (!Directory.Exists(tmp1))
                    {
                        Directory.CreateDirectory(tmp1);
                    }

                    var tmp2 = tmp1 + "CutMergeFile/";
                    if (!Directory.Exists(tmp2))
                    {
                        Directory.CreateDirectory(tmp2);
                    }

                    CutOrMergePath = tmp2;
                    tmp2 = tmp1 + "CutMergeTempDir/";
                    if (!Directory.Exists(tmp2))
                    {
                        Directory.CreateDirectory(tmp2);
                    }

                    CutOrMergeTempPath = tmp2;
                }
            }
            else
            {
                if (!Directory.Exists(CutOrMergePath))
                {
                    Directory.CreateDirectory(CutOrMergePath);
                }

                if (!Directory.Exists(CutOrMergeTempPath))
                {
                    Directory.CreateDirectory(CutOrMergeTempPath);
                }
            }
        }
    }
}