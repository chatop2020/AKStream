using System;
using System.Collections.Generic;
using System.Threading;
using AKStreamKeeper.Services;
using LibCommon;

namespace AKStreamKeeper.AutoTask;

public class AutoRtpPortClean
{
    public AutoRtpPortClean()
    {
        new Thread(new ThreadStart(delegate
        {
            try
            {
                AutoClean();
            }
            catch
            {
            }
        })).Start();
    }


    private void DeleteExpiredLogFiles()
    {
    }

    /// <summary>
    /// 获取rtpserver列表 
    /// </summary>
    /// <returns></returns>
    private List<ushort> GetPortRptList(out ResponseStruct rs)
    {
        rs = new ResponseStruct()
        {
            Code = ErrorNumber.None,
            Message = ErrorMessage.ErrorDic![ErrorNumber.None],
        };

        var uri = new Uri(Common.MediaServerInstance.AkStreamKeeperConfig.AkStreamWebRegisterUrl, false);

        var url =
            $"{uri.Scheme}://{uri.Host}:{uri.Port}/MediaServer/ListRtpServer?mediaServerId={Common.MediaServerInstance.MediaServerId}";


        var httpRet = NetHelper.HttpGetRequest(url, null, "utf-8", 500);

        if (!string.IsNullOrEmpty(httpRet))
        {
            if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_HttpClientTimeout,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                };
                return null;
            }

            if (httpRet.Contains(ErrorMessage.ErrorDic[ErrorNumber.MediaServer_InstanceIsNull]))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_ObjectNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ObjectNotExists],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->流媒体服务器还未就绪,无法提供RtpServer列表,下次循环继续尝试...");
                return null;
            }

            List<ushort> ports = null;
            try
            {
                ports = JsonHelper.FromJson<List<ushort>>(httpRet);
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_JsonReadExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonReadExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace
                };
                return null;
            }

            return ports;
        }

        rs = new ResponseStruct()
        {
            Code = ErrorNumber.MediaServer_WebApiDataExcept,
            Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
        };

        return null;
    }

    private void AutoClean()
    {
        GCommon.Logger.Debug($"[{Common.LoggerHead}]->创建Rtp端口清理自动任务");
        while (true)
        {
            
            ResponseStruct rs = null;

            var ports = GetPortRptList(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取Rtp端口列表异常->{JsonHelper.ToJson(rs)}");
            }
            else
            {
                GCommon.Logger.Debug($"[{Common.LoggerHead}]->获取在用Rtp端口列表->{JsonHelper.ToJson(ports)}");
                if (ports != null)
                {
                    if (ports.Count == 0)
                    {
                        foreach (var pi in Common.PortInfoList)
                        {
                            lock (Common._getRtpPortLock)
                            {
                                var portUsed = Common.PortInfoList.FindLast(x => x.Port.Equals(pi.Port) && x.Useed);
                                if (portUsed != null)
                                {
                                    portUsed.Useed = false;
                                    GCommon.Logger.Info($"[{Common.LoggerHead}]->释放rtp端口成功:{pi.Port}");
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var pi in Common.PortInfoList)
                        {
                            if (pi != null && pi.Useed && DateTime.Now >
                                pi.DateTime.AddSeconds(Common.MediaServerInstance.AkStreamKeeperConfig.RtpPortCdTime))
                            {
                                if (!ports.Contains(pi.Port))
                                {
                                    GCommon.Logger.Debug($"[{Common.LoggerHead}]->自动释放Rtp端口->{JsonHelper.ToJson(pi)}");
                                    ApiService.ReleaseRtpPort(pi.Port);
                                }
                                else
                                {
                                    lock (Common._getRtpPortLock)
                                    {
                                        var portUsed = Common.PortInfoList.FindLast(x => x.Port.Equals(pi.Port));
                                        if (portUsed != null)
                                        {
                                            GCommon.Logger.Debug(
                                                $"[{Common.LoggerHead}]->更新Rtp端口激活状态时间->{JsonHelper.ToJson(portUsed)}");
                                            portUsed.DateTime = DateTime.Now; //更新端口，目前正在使用的时间
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Thread.Sleep(1000 * 10);
        }
    }
}