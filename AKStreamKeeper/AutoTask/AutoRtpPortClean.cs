using System;
using System.Collections.Generic;
using System.Threading;
using AKStreamKeeper.Services;
using LibCommon;
using LibLogger;

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


    /// <summary>
    /// 获取rtpserver列表 
    /// </summary>
    /// <returns></returns>
    private List<ushort> GetPortRptList()
    {
        ResponseStruct rs = new ResponseStruct()
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

            var ports = JsonHelper.FromJson<List<ushort>>(httpRet);
            if (ports != null)
            {
                return ports;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_WebApiDataExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                ExceptMessage = httpRet,
            };
        }
        else
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_WebApiDataExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
            };
        }

        return null;
    }

    private void AutoClean()
    {
        GCommon.Logger.Debug($"[{Common.LoggerHead}]->创建Rtp端口清理自动任务");
        while (true)
        {
            var ports = GetPortRptList();
            GCommon.Logger.Debug($"[{Common.LoggerHead}]->获取在用Rtp端口列表->{JsonHelper.ToJson(ports)}");
            if (ports != null)
            {
                if (ports.Count == 0)
                {
                    foreach (var pi in Common.PortInfoList)
                    {
                        GCommon.Logger.Debug($"[{Common.LoggerHead}]->自动释放Rtp端口->{JsonHelper.ToJson(pi)}");
                        lock (Common._getRtpPortLock)
                        {
                            var portUsed = Common.PortInfoList.FindLast(x => x.Port.Equals(pi.Port));
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
                                        GCommon.Logger.Debug($"[{Common.LoggerHead}]->更新Rtp端口激活状态时间->{JsonHelper.ToJson(portUsed)}");
                                        portUsed.DateTime = DateTime.Now; //更新端口，目前正在使用的时间
                                        
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