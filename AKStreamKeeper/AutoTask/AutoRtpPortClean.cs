using System;
using System.Collections.Generic;
using System.Threading;
using AKStreamKeeper.Services;
using Newtonsoft.Json.Linq;
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
    // AKStreamKeeper/AutoTask/AutoRtpPortClean.cs
// 增加 using Newtonsoft.Json.Linq;
    private List<ushort> GetPortRptList(out ResponseStruct rs)
    {
        rs = new ResponseStruct
        {
            Code = ErrorNumber.None,
            Message = ErrorMessage.ErrorDic![ErrorNumber.None],
        };

        var uri = new Uri(Common.MediaServerInstance.AkStreamKeeperConfig.AkStreamWebRegisterUrl, false);
        var url =
            $"{uri.Scheme}://{uri.Host}:{uri.Port}/MediaServer/ListRtpServer?mediaServerId={Common.MediaServerInstance.MediaServerId}";

        var timeoutMs = Math.Max(
            Common.MediaServerInstance.AkStreamKeeperConfig.HttpClientTimeoutSec * 1000,
            3000);

        var httpRet = NetHelper.HttpGetRequest(url, null, "utf-8", timeoutMs);

        if (string.IsNullOrWhiteSpace(httpRet))
        {
            rs = new ResponseStruct
            {
                Code = ErrorNumber.MediaServer_WebApiDataExcept,
                Message = "获取RtpServer列表失败，AKStreamWeb没有返回任何内容",
                ExceptMessage = $"Url:{url}"
            };
            return null;
        }

        if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
        {
            rs = new ResponseStruct
            {
                Code = ErrorNumber.Sys_HttpClientTimeout,
                Message = "获取RtpServer列表失败，访问AKStreamWeb超时或服务不可达",
                ExceptMessage = $"Url:{url}\r\nBody:{httpRet}"
            };
            return null;
        }

        if (httpRet.Contains(ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull]))
        {
            rs = new ResponseStruct
            {
                Code = ErrorNumber.MediaServer_ObjectNotExists,
                Message = "流媒体服务器还未就绪，无法提供RtpServer列表，下次循环继续尝试",
                ExceptMessage = $"Url:{url}\r\nBody:{httpRet}"
            };

            GCommon.Logger.Warn($"[{Common.LoggerHead}]->流媒体服务器还未就绪,无法提供RtpServer列表,下次循环继续尝试...");
            return null;
        }

        try
        {
            var token = JToken.Parse(httpRet);

            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<ushort>>() ?? new List<ushort>();
            }

            if (token.Type == JTokenType.Object)
            {
                var error = token.ToObject<ResponseStruct>();
                if (error != null && error.Code != ErrorNumber.None)
                {
                    rs = error;
                    if (string.IsNullOrWhiteSpace(rs.Message))
                    {
                        rs.Message = "获取RtpServer列表失败，AKStreamWeb返回了错误对象";
                    }

                    rs.ExceptMessage ??= $"Url:{url}\r\nBody:{httpRet}";

                    if (rs.Code == ErrorNumber.MediaServer_NotRunning)
                    {
                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->流媒体服务器没有运行,无法提供RtpServer列表,下次循环继续尝试...");
                    }
                    else
                    {
                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取RtpServer列表失败->{JsonHelper.ToJson(rs)}");
                    }

                    return null;
                }
            }

            rs = new ResponseStruct
            {
                Code = ErrorNumber.MediaServer_WebApiDataExcept,
                Message = "获取RtpServer列表失败，AKStreamWeb返回的数据既不是端口数组，也不是标准错误对象",
                ExceptMessage = $"Url:{url}\r\nBody:{httpRet}"
            };
            return null;
        }
        catch (Exception ex)
        {
            rs = new ResponseStruct
            {
                Code = ErrorNumber.Sys_JsonReadExcept,
                Message = "获取RtpServer列表失败，解析AKStreamWeb返回内容时发生Json异常",
                ExceptMessage = $"{ex.Message}\r\nUrl:{url}\r\nBody:{httpRet}",
                ExceptStackTrace = ex.StackTrace
            };
            return null;
        }
    }
    // private List<ushort> GetPortRptList(out ResponseStruct rs)
    // {
    //     rs = new ResponseStruct()
    //     {
    //         Code = ErrorNumber.None,
    //         Message = ErrorMessage.ErrorDic![ErrorNumber.None],
    //     };
    //
    //     var uri = new Uri(Common.MediaServerInstance.AkStreamKeeperConfig.AkStreamWebRegisterUrl, false);
    //
    //     var url =
    //         $"{uri.Scheme}://{uri.Host}:{uri.Port}/MediaServer/ListRtpServer?mediaServerId={Common.MediaServerInstance.MediaServerId}";
    //
    //
    //     var httpRet = NetHelper.HttpGetRequest(url, null, "utf-8", 500);
    //
    //     if (!string.IsNullOrEmpty(httpRet))
    //     {
    //         if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
    //         {
    //             rs = new ResponseStruct()
    //             {
    //                 Code = ErrorNumber.Sys_HttpClientTimeout,
    //                 Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
    //             };
    //             return null;
    //         }
    //
    //         if (httpRet.Contains(ErrorMessage.ErrorDic[ErrorNumber.MediaServer_InstanceIsNull]))
    //         {
    //             rs = new ResponseStruct()
    //             {
    //                 Code = ErrorNumber.MediaServer_ObjectNotExists,
    //                 Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ObjectNotExists],
    //             };
    //             GCommon.Logger.Warn($"[{Common.LoggerHead}]->流媒体服务器还未就绪,无法提供RtpServer列表,下次循环继续尝试...");
    //             return null;
    //         }
    //
    //         List<ushort> ports = null;
    //         try
    //         {
    //             ports = JsonHelper.FromJson<List<ushort>>(httpRet);
    //         }
    //         catch (Exception ex)
    //         {
    //             rs = new ResponseStruct()
    //             {
    //                 Code = ErrorNumber.Sys_JsonReadExcept,
    //                 Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_JsonReadExcept],
    //                 ExceptMessage = ex.Message,
    //                 ExceptStackTrace = ex.StackTrace
    //             };
    //             return null;
    //         }
    //
    //         return ports;
    //     }
    //
    //     rs = new ResponseStruct()
    //     {
    //         Code = ErrorNumber.MediaServer_WebApiDataExcept,
    //         Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
    //     };
    //
    //     return null;
    // }

    private void AutoClean()
    {
        GCommon.Logger.Debug($"[{Common.LoggerHead}]->创建Rtp端口清理自动任务");
        while (true)
        {
            ResponseStruct rs = null;

            // 到期状态清理不依赖 Web 在线；失败保留原文件，下轮继续而不是退出维护线程。
            try { Common.RtpPortStops.PruneExpired(DateTime.UtcNow); }
            catch (Exception ex) { GCommon.Logger.Warn("清理停流冷却记录失败：" + ex.Message); }

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
                    try { ApiService.CleanUnusedRtpPorts(ports); }
                    catch (Exception ex) { GCommon.Logger.Warn("自动回收 RTP 端口失败，保留原占用：" + ex.Message); }
                }
            }

            Thread.Sleep(1000 * 10);
        }
    }
}
