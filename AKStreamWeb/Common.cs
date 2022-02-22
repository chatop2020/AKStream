using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using AKStreamWeb.AutoTask;
using AKStreamWeb.Misc;
using LibCommon;
using LibCommon.Structs;
using LibGB28181SipClient;
using LibGB28181SipServer;
using LibLogger;
using LibSystemInfo;
using LibZLMediaKitMediaServer;
using Newtonsoft.Json;
using JsonHelper = LibCommon.JsonHelper;

namespace AKStreamWeb
{
    public static class Common
    {
        private static string _loggerHead = "AKStreamWeb";
        private static ORMHelper _ormHelper;
        private static string _configPath = GCommon.ConfigPath + "AKStreamWeb.json";
        public static bool IsDebug = false;
        private static SystemInfo _webSystemInfo = new SystemInfo();
        public static PerformanceInfo WebPerformanceInfo = new PerformanceInfo();

        private static object _performanceInfoLock = new object();

        private static Timer _perFormanceInfoTimer;
        private static AutoLive _autoLive;
        private static AutoRecord _autoRecord;
        private static AutoTaskOther _autoTaskOther;
      

        private static ConcurrentDictionary<string, WebHookNeedReturnTask> _webHookNeedReturnTask =
            new ConcurrentDictionary<string, WebHookNeedReturnTask>();
        public static DateTime StartupDateTime;

    

        /// <summary>
        /// 流媒体服务器列表
        /// </summary>
        public static List<ServerInstance> MediaServerList = new List<ServerInstance>();

        /// <summary>
        /// 操作流媒体服务器列表的锁
        /// </summary>
        public static object MediaServerLockObj = new object();

        /// <summary>
        /// 配置文件实例
        /// </summary>
        public static AKStreamWebConfig AkStreamWebConfig;

        /// <summary>
        /// Sip服务实例
        /// </summary>
        public static SipServer SipServer=null;

        /// <summary>
        /// Sip客户端实例
        /// </summary>
        public static SipClient SipClient=null;
        

        private static List<ShareInviteInfo> _shareInviteChannels = new List<ShareInviteInfo>();
        /// <summary>
        /// 共享流列表
        /// </summary>
        public static List<ShareInviteInfo> ShareInviteChannels
        {
            get => _shareInviteChannels;
            set => _shareInviteChannels = value;
        }

        static Common()
        {

          
            if (!string.IsNullOrEmpty(GCommon.OutConfigPath))
            {
                if (!GCommon.OutConfigPath.Trim().EndsWith('/'))
                {
                    GCommon.OutConfigPath +=  "/";
                }
                _configPath= GCommon.OutConfigPath + "AKStreamWeb.json";
            }

            StartupDateTime = DateTime.Now;
            string supportDataBaseList = "MySql\r\n" +
                                         "SqlServer\r\n" +
                                         "PostgreSQL\r\n" +
                                         "Oracle\r\n" +
                                         "Firebird\r\n" +
                                         "Sqlite\r\n" +
                                         "OdbcOracle\r\n" +
                                         "OdbcSqlServer\r\n" +
                                         "OdbcMySql\r\n" +
                                         "OdbcPostgreSQL\r\n" +
                                         "Odbc\r\n" +
                                         "[Odbc说明]\r\n" +
                                         "[通用的 Odbc 实现，只能做基本的 Crud 操作]\r\n" +
                                         "[不支持实体结构迁移、不支持分页（只能 Take 查询]\r\n" +
                                         "[通用实现为了让用户自己适配更多的数据库，比如连接 mssql 2000、db2 等数据库]\r\n" +
                                         "[默认适配 SqlServer，可以继承后重新适配 FreeSql.Odbc.Default.OdbcAdapter，最好去看下代码]\r\n" +
                                         "[适配新的 OdbcAdapter，请在 FreeSqlBuilder.Build 之后调用 IFreeSql.SetOdbcAdapter 方法设置]\r\n" +
                                         "OdbcDameng+\r\n" +
                                         "[OdbcDameng说明]-武汉达梦数据库有限公司，基于 Odbc 的实现\r\n" +
                                         "MsAccess+\r\n" +
                                         "[MsAccess说明]-Microsoft Office Access 是由微软发布的关联式数据库管理系统\r\n" +
                                         "Dameng+\r\n" +
                                         "[Dameng说明]-武汉达梦数据库有限公司，基于 DmProvider.dll 的实现\r\n" +
                                         "OdbcKingbaseES+\r\n" +
                                         "[OdbcKingbaseES说明]-北京人大金仓信息技术股份有限公司，基于 Odbc 的实现\r\n" +
                                         "ShenTong+\r\n" +
                                         "[ShenTong说明]-天津神舟通用数据技术有限公司，基于 System.Data.OscarClient.dll 的实现\r\n" +
                                         "KingbaseES+\r\n" +
                                         "[KingbaseES说明]-Firebird 是一个跨平台的关系数据库，能作为多用户环境下的数据库服务器运行，也提供嵌入式数据库的实现";
            try
            {
                ResponseStruct rs;
                var ret = ReadConfigFile(out rs);
                if (!ret || !rs.Code.Equals(ErrorNumber.None))
                {
                    GCommon.Logger.Error(
                        $"[{LoggerHead}]->获取AKStream配置文件时异常,系统无法运行->\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
                    Environment.Exit(0); //退出程序 
                }

                GCommon.Logger.Info(
                    $"[{LoggerHead}]->AKStreamWeb配置文件加完成");
            }
            catch (AkStreamException ex)
            {
                GCommon.Logger.Error(
                    $"[{LoggerHead}]->获取AKStream配置文件时异常,系统无法运行->\r\n{JsonHelper.ToJson(ex.ResponseStruct, Formatting.Indented)}");
                Environment.Exit(0); //退出程序
            }

#if (DEBUG)
            Console.WriteLine("[Debug]\t当前程序为Debug编译模式");
            Console.WriteLine("[Debug]\t程序启动路径:" + GCommon.BaseStartPath);
            Console.WriteLine("[Debug]\t程序启动全路径:" + GCommon.BaseStartFullPath);
            Console.WriteLine("[Debug]\t程序运行路径:" + GCommon.WorkSpacePath);
            Console.WriteLine("[Debug]\t程序运行全路径:" + GCommon.WorkSpaceFullPath);
            Console.WriteLine("[Debug]\t程序启动命令:" + GCommon.CommandLine);
            Console.WriteLine("[Debug]\t程序版本标识:" + Version);
            IsDebug = true;


#endif
            try
            {
                GCommon.Ldb.VideoOnlineInfo.DeleteAll();
                OrmHelper = new ORMHelper(AkStreamWebConfig.OrmConnStr, AkStreamWebConfig.DbType);
            }
            catch (Exception ex)
            {
                ResponseStruct rsa = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseNotReady,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseNotReady] + ",请检查配置文件中的数据库相关配置信息",
                };
                GCommon.Logger.Error(
                    $"[{LoggerHead}]->数据库连接异常,系统无法运行->\r\n{JsonHelper.ToJson(rsa, Formatting.Indented)}\r\n系统支持以下数据库连接,请根据下表正确设置dBType字段->\r\n{supportDataBaseList}");
                Environment.Exit(0); //退出程序
            }

            if (ORMHelper.Db == null)
            {
                ResponseStruct rsa = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseNotReady,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseNotReady] + ",请检查配置文件中的数据库相关配置信息" +
                              "\r\n",
                };
                GCommon.Logger.Error(
                    $"[{LoggerHead}]->数据库连接异常,系统无法运行->\r\n{JsonHelper.ToJson(rsa, Formatting.Indented)}\r\n系统支持以下数据库连接,请根据下表正确设置dBType字段->\r\n{supportDataBaseList}");
                Environment.Exit(0); //退出程序
            }


            string outPath = "";
            if (!string.IsNullOrEmpty(GCommon.OutConfigPath))
            {
                outPath = GCommon.OutConfigPath;
            }

            if (AkStreamWebConfig.EnableGB28181Server == null || AkStreamWebConfig.EnableGB28181Server == true)
            {
                SipServer = new SipServer(outPath);
                SipMsgProcess.OnRegisterReceived += SipServerCallBack.OnRegister;
                SipMsgProcess.OnUnRegisterReceived += SipServerCallBack.OnUnRegister;
                SipMsgProcess.OnKeepaliveReceived += SipServerCallBack.OnKeepalive;
                SipMsgProcess.OnDeviceReadyReceived += SipServerCallBack.OnDeviceReadyReceived;
                SipMsgProcess.OnDeviceStatusReceived += SipServerCallBack.OnDeviceStatusReceived;
                SipMsgProcess.OnInviteHistoryVideoFinished += SipServerCallBack.OnInviteHistoryVideoFinished;
                SipMsgProcess.OnCatalogReceived += SipServerCallBack.OnCatalogReceived;
                SipMsgProcess.OnDeviceAuthentication += SipServerCallBack.OnAuthentication;
            }
            GCommon.Logger.Info($"[{LoggerHead}]->配置情况->是否启用Sip服务端->{AkStreamWebConfig.EnableGB28181Server}");
            if (AkStreamWebConfig.EnableGB28181Client)
            {
                outPath = "";
                if (!string.IsNullOrEmpty(GCommon.OutConfigPath))
                {
                    outPath = GCommon.OutConfigPath;
                }
                SipClient = new SipClient(outPath);
                SipClient.OnInviteChannel += SipClientProcess.InviteChannel;
                SipClient.OnDeInviteChannel += SipClientProcess.DeInviteChannel;
            }
            GCommon.Logger.Info($"[{LoggerHead}]->配置情况->是否启用Sip客户端->{AkStreamWebConfig.EnableGB28181Client}");

            if (AkStreamWebConfig.EnableGB28181Server == null || AkStreamWebConfig.EnableGB28181Server == true)
            {
                try
                {

                    ResponseStruct rs;
                    SipServer.Start(out rs);
                    if (!rs.Code.Equals(ErrorNumber.None))
                    {
                        GCommon.Logger.Error(
                            $"[{LoggerHead}]->启动Sip服务时异常,系统无法运行->\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
                        Environment.Exit(0); //退出程序 
                    }
                }
                catch (AkStreamException ex)
                {
                    GCommon.Logger.Error(
                        $"[{LoggerHead}]->启动Sip服务时异常,系统无法运行->\r\n{JsonHelper.ToJson(ex, Formatting.Indented)}");
                    Environment.Exit(0); //退出程序
                }
            }

            _autoLive = new AutoLive();
            _autoRecord = new AutoRecord();
            _autoTaskOther = new AutoTaskOther();
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

        public static ConcurrentDictionary<string, WebHookNeedReturnTask> WebHookNeedReturnTask
        {
            get => _webHookNeedReturnTask;
            set => _webHookNeedReturnTask = value;
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
        /// 数据库对象
        /// </summary>
        public static ORMHelper OrmHelper
        {
            get => _ormHelper;
            set => _ormHelper = value;
        }


        private static void startTimer()
        {
            if (_perFormanceInfoTimer == null)
            {
                _perFormanceInfoTimer = new Timer(1000);
                _perFormanceInfoTimer.Enabled = true; //启动Elapsed事件触发
                _perFormanceInfoTimer.Elapsed += OnTimedEvent; //添加触发事件的函数
                _perFormanceInfoTimer.AutoReset = true; //需要自动reset
                _perFormanceInfoTimer.Start(); //启动计时器
            }
        }


        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            lock (_performanceInfoLock)
            {
                WebPerformanceInfo = _webSystemInfo.GetSystemInfoObject();
            }
        }


        /// <summary>
        /// 所有程序的入口
        /// </summary>
        public static void Init()
        {
            GCommon.Logger.Info(
                $"[{LoggerHead}]->Let's Go...");

            startTimer();
        }

        /// <summary>
        /// 读取AKStream的配置文件akstream.json
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        private static bool ReadConfigFile(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (File.Exists(_configPath))
            {
                try
                {
                    AkStreamWebConfig = JsonHelper.FromJson<AKStreamWebConfig>(File.ReadAllText(_configPath));

                    return true;
                }
                catch (Exception ex)
                {
                    ResponseStruct rsex = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_JsonReadExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonReadExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rsex);
                }
            }
            else
            {
                var config = new AKStreamWebConfig()
                {
                    MediaServerFirstToRestart = true,
                    OrmConnStr = "请配置正确的数据库连接字符串",
                    DbType = "请配置正确的数据库类型如 MySql、Sqlite等",
                    WebApiPort = 5800,
                    HttpClientTimeoutSec = 20,
                    AccessKey = UtilsHelper.generalGuid(),
                    DeletedRecordsExpiredDays = 0,
                    EnableGB28181Client = false
                };
                try
                {
                    string configStr = JsonHelper.ToJson(config, Formatting.Indented);
                    File.WriteAllText(_configPath, configStr);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ConfigNotReady,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                        ExceptMessage = _configPath,
                        ExceptStackTrace = $"{_configPath}文件不存在，已自动创建，请完善此文件的其他属性内容",
                    };
                    return false;
                }
                catch (Exception ex)
                {
                    ResponseStruct rsex = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_JsonWriteExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonWriteExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rsex);
                }
            }
        }
    }
}