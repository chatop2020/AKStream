using System;
using System.IO;
using System.Threading;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.GB28181;
using LibSystemInfo;

namespace LibGB28181SipClient
{
    public static class Common
    {
        private static SipClientConfig _sipClientConfig = null!;
        private static string _sipClientConfigPath = GCommon.ConfigPath + "SipClientConfig.json";
        private static SipClient _sipClient = null;
        private static string _sipUserAgent = "AKStreamSipClient/1.0";

        public static SipClient SipClient
        {
            get => _sipClient;
            set => _sipClient = value;
        }


        public static string SipClientConfigPath
        {
            get => _sipClientConfigPath;
            set => _sipClientConfigPath = value;
        }

        public static string LoggerHead = "SipClient";

        public static SipClientConfig SipClientConfig
        {
            get => _sipClientConfig;
            set => _sipClientConfig = value;
        }

        public static string SipUserAgent
        {
            get => _sipUserAgent;
            set => _sipUserAgent = value;
        }

        /// <summary>
        /// 初始化sip客户端配置文件
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static SipClientConfig InitSipClientConfig(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                SipServerConfig sipServerConfig = null!;
                SystemInfo systemInfo = new SystemInfo();
                string macAddr = "";
                var sys = systemInfo.GetSystemInfoObject();
                int i = 0;
                while ((sys == null || sys.NetWorkStat == null) && i < 50)
                {
                    i++;
                    Thread.Sleep(50);
                }

                if (sys != null && sys.NetWorkStat != null)
                {
                    macAddr = sys.NetWorkStat.Mac;
                    systemInfo.Dispose();
                }

                if (string.IsNullOrEmpty(macAddr))
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_GetMacAddressExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_GetMacAddressExcept],
                    };
                    return null!; //mac 地址没找到了，报错出去
                }

                IPInfo ipInfo = UtilsHelper.GetIpAddressByMacAddress(macAddr, true);
                if (!string.IsNullOrEmpty(ipInfo.IpV4))
                {
                    SipClientConfig sipClientConfig = new SipClientConfig();
                    sipClientConfig.LocalIpAddress = ipInfo.IpV4;
                    sipClientConfig.SipDeviceId = "33020000021190000002";
                    sipClientConfig.SipServerDeviceId = "33020000021180000001";
                    /*SipDeviceID 20位编码规则
                     *1-2省级 33 浙江省
                     *3-4市级 02 宁波市
                     *5-6区级 00 宁波市区
                     *7-8村级 00 宁波市区
                     *9-10行业 02 社会治安内部接入
                     *11-13设备类型 118 NVR
                     *14 网络类型 0 监控专用网
                     *15-20 设备序号 000001 1号设备
                     */
                    sipClientConfig.Realm = sipClientConfig.SipDeviceId.Substring(0, 10);
                    sipClientConfig.LocalPort = 5061;
                    sipClientConfig.SipPassword = "123#@!qwe";
                    sipClientConfig.SipUsername = "admin";
                    sipClientConfig.KeepAliveInterval = 10;
                    sipClientConfig.SipServerPort = 5060;
                    sipClientConfig.KeepAliveLostNumber = 3;
                    sipClientConfig.SipServerIpAddress = "Sip服务器的ip地址";
                    sipClientConfig.Expiry = 3600; //注册有效期 3600秒
                    sipClientConfig.EncodingType = EncodingType.UTF8;
                    //sipClientConfig.Encoding=Encoding.UTF8;
                    sipClientConfig.AkstreamWebHttpUrl = "http://127.0.0.1:5800/SipClient";
                    return sipClientConfig;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_SipClient_InitExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SipClient_InitExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }


        /// <summary>
        /// 读取sip客户端配置文件
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int ReadSipClientConfig(out ResponseStruct rs)
        {
            if (!File.Exists(_sipClientConfigPath))
            {
                var config = InitSipClientConfig(out rs);
                if (config != null && rs.Code.Equals(ErrorNumber.None))
                {
                    _sipClientConfig = config;
                    if (UtilsHelper.WriteJsonConfig(_sipClientConfigPath, _sipClientConfig))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_ConfigNotReady,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                        };
                        return -1;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_JsonWriteExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonWriteExcept],
                    };
                    return -1;
                }
            }
            else
            {
                try
                {
                    _sipClientConfig =
                        (UtilsHelper.ReadJsonConfig<SipClientConfig>(_sipClientConfigPath) as SipClientConfig)!;
                    if (_sipClientConfig != null)
                    {
                        /*
                        switch (_sipClientConfig.EncodingType)
                        {
                           case EncodingType.UTF8:
                               _sipClientConfig.Encoding=Encoding.UTF8;
                               break;
                           case EncodingType.GB2312:
                               _sipClientConfig.Encoding = Encoding.GetEncoding("gb2312");
                               break;
                           case EncodingType.GBK:
                               _sipClientConfig.Encoding=Encoding.GetEncoding("GBK");
                               break;
                           default:
                               _sipClientConfig.Encoding=Encoding.UTF8;
                               break;
                        }*/
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.None,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                        };

                        return 0;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_JsonReadExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonReadExcept],
                    };
                    return -1;
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Other,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    return -1;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return 1;
        }
    }
}