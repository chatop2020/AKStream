using System;
using System.Collections.Generic;
using System.Threading;
using AKStreamWeb.Misc;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.GB28181.XML;
using LibCommon.Structs.Other;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using LibGB28181SipServer;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebHookRequest;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace AKStreamWeb.Services
{
    public static class SipServerService
    {
        private static readonly ConcurrentDictionary<string, object> _gb28181LiveLocks =
            new ConcurrentDictionary<string, object>();

        /// <summary>直播建立和停流共用同一条流的锁，避免旧清理与新 INVITE 同时修改接收服务。</summary>
        internal static object GetLiveLock(string serverId, string stream) =>
            _gb28181LiveLocks.GetOrAdd(serverId + ":" + stream, _ => new object());

        public static bool TryDeInviteSipChannel(SipChannel sipChannel, string reason, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (sipChannel == null)
            {
                return true;
            }

            if (sipChannel.InviteSipRequest == null || sipChannel.InviteSipResponse == null)
            {
                sipChannel.PushStatus = PushStatus.IDLE;
                GCommon.Logger.Info($"[{Common.LoggerHead}]->跳过Sip BYE，当前通道没有有效Invite会话->{reason}");
                return true;
            }

            try
            {
                var sipMethodProxy = new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
                var ret = sipMethodProxy.DeInvite(sipChannel, out rs);

                if (!ret || !rs.Code.Equals(ErrorNumber.None))
                {
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->Sip BYE发送失败->{reason}->{JsonHelper.ToJson(rs)}");
                    return false;
                }

                GCommon.Logger.Info($"[{Common.LoggerHead}]->Sip BYE发送成功->{reason}");
                return true;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeInviteExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeInviteExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->Sip BYE发送异常->{reason}->{JsonHelper.ToJson(rs)}");
                return false;
            }
        }

        /// <summary>
        /// 通过taskId获取回放文件列表状态
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static VideoChannelRecordInfo GetHistroyRecordFileStatus(int taskId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                return GCommon.VideoChannelRecordInfo.FindLast(x => x.TaskId.Equals(taskId));
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                return null;
            }
        }

        /// <summary>
        /// 获取sip设备的历史录制文件列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="queryRecordFile"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int GetHistroyRecordFileList(string deviceId, string channelId,
            SipQueryRecordFile queryRecordFile, out ResponseStruct rs)
        {
            ServerInstance mediaServer;
            VideoChannel videoChannel;
            SipDevice sipDevice;
            SipChannel sipChannel;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = CheckIt(deviceId, channelId, out rs, out mediaServer, out videoChannel, out sipChannel,
                out sipDevice);
            if (ret == false || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取Sip设备历史录制文件失败->{deviceId}-{channelId}-{JsonHelper.ToJson(queryRecordFile)}->{JsonHelper.ToJson(rs)}");

                return -1;
            }

            SipMethodProxy sipMethodProxy = new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
            var got = sipMethodProxy.QueryRecordFileList(sipChannel, queryRecordFile, out rs); //获取历史文件
            if (!rs.Code.Equals(ErrorNumber.None) || got < 0)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取Sip设备历史录制文件失败->{deviceId}-{channelId}-{JsonHelper.ToJson(queryRecordFile)}->{JsonHelper.ToJson(rs)}");

                return -1;
            }

            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->获取Sip设备历史录制文件成功->{deviceId}-{channelId}-{JsonHelper.ToJson(queryRecordFile)}->加载可能较慢，请使用返回值定期调用查询GetHistroyRecordFileStatus接口以获取最终结果");

            return got;
        }

        /// <summary>
        ///  检查livevideo,stopvideo的相关参数
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="rs"></param>
        /// <param name="mediaServer"></param>
        /// <param name="videoChannel"></param>
        /// <param name="sipChannel"></param>
        /// <param name="sipDevice"></param>
        /// <returns></returns>
        private static bool CheckIt(string deviceId, string channelId, out ResponseStruct rs,
            out ServerInstance mediaServer, out VideoChannel videoChannel, out SipChannel sipChannel,
            out SipDevice sipDevice)
        {
            mediaServer = null;
            videoChannel = null;
            sipChannel = null;
            sipDevice = null;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (UtilsHelper.StringIsNullEx(deviceId) || UtilsHelper.StringIsNullEx(channelId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };


                return false;
            }

            sipDevice = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId.Equals(deviceId));

            if (sipDevice == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                return false;
            }

            sipChannel = sipDevice.SipChannels.FindLast(x => x.DeviceId.Equals(channelId));
            if (sipChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_ChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_ChannelNotExists],
                };
                return false;
            }

            if (videoChannel == null)
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.DeviceId.Equals(deviceId))
                        .Where(x => x.ChannelId.Equals(channelId)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->CheckIt->执行SQL:->{sql}");
                }

                #endregion

                videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.DeviceId.Equals(deviceId))
                    .Where(x => x.ChannelId.Equals(channelId))
                    .First();
                if (videoChannel == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                    };
                    return false;
                }
            }

            if (videoChannel.MediaServerId.Contains("unknown_server"))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_VideoChannelNotActived,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_VideoChannelNotActived],
                };
                return false;
            }

            string tmpId = videoChannel.MediaServerId;
            mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(tmpId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_InstanceIsNull,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
                };
                return false;
            }

            if (!mediaServer.IsKeeperRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                };
                return false;
            }

            if (!mediaServer.IsMediaServerRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_NotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_NotRunning],
                };
                return false;
            }

            return true;
        }


        /// <summary>
        /// 停止指定设备录像回放。先持久化接收端口隔离，BYE 和本地关闭均确认后才进入冷却。
        /// </summary>
        /// <param name="taskId">原设备录像查询任务编号。</param>
        /// <param name="ssrcId">该任务内回放流的十进制 SSRC。</param>
        /// <param name="rs">未完全确认时保留失败原因，禁止被本地成功结果覆盖。</param>
        /// <returns>设备侧及本地均确认停止时返回 true；否则端口继续隔离。</returns>
        public static bool StopLiveVideo(int taskId, string ssrcId, out ResponseStruct rs)
        {
            var row = GCommon.VideoChannelRecordInfo.FindLast(x => x.TaskId == taskId);
            var record = row?.RecItems?.FindLast(x => x.SsrcId == ssrcId);
            if (record == null)
            {
                rs = new ResponseStruct { Code = ErrorNumber.Sys_DB_RecordNotExists,
                    Message = ErrorMessage.ErrorDic[ErrorNumber.Sys_DB_RecordNotExists] };
                return false;
            }
            var channel = record.SipChannel;
            var video = ORMHelper.Db.Select<VideoChannel>().Where(x => x.DeviceId == record.SipDevice.DeviceId &&
                x.ChannelId == channel.DeviceId).First();
            var server = Common.MediaServerList.FindLast(x => x.MediaServerId == video?.MediaServerId);
            if (server == null)
            {
                rs = new ResponseStruct { Code = ErrorNumber.MediaServer_InstanceIsNull,
                    Message = "未找到回放接收节点，不能确认停流。" };
                return false;
            }
            var snapshot = new SipChannel { ParentId = record.SipDevice.DeviceId, DeviceId = channel.DeviceId };
            if (record.InviteSipRequest != null) snapshot.InviteSipRequest = record.InviteSipRequest;
            if (record.InviteSipResponse != null) snapshot.InviteSipResponse = record.InviteSipResponse;
            var info = record.MediaServerStreamInfo;
            var port = info?.RptPort ?? 0;
            var result = video.DefaultRtpPort == true
                ? Gb28181StreamStop.StopShared(server, snapshot, record.Stream, record.App, record.Vhost, out rs, Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec)
                : Gb28181StreamStop.Stop(server, snapshot, port, record.Stream, record.App, record.Vhost, true, out rs, Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
            if (result) record.PushStatus = PushStatus.IDLE;
            return result;
        }

        /// <summary>
        /// 停止 GB28181 实时推流。在线媒体记录缺失也检查原 dialog 和接收服务；失败保存待重试任务。
        /// </summary>
        /// <param name="deviceId">设备国标编号。</param>
        /// <param name="channelId">通道国标编号。</param>
        /// <param name="rs">包含 BYE 或接收服务清理失败原因。</param>
        /// <returns>两侧均确认停止时返回 true；假定设备离线不能作为成功依据。</returns>
        public static bool StopLiveVideo(string deviceId, string channelId, out ResponseStruct rs)
        {
            // 设备离线不代表媒体接收已经关闭，停流不能以 SIP 在线列表存在为前提。
            var video = ORMHelper.Db.Select<VideoChannel>().Where(x => x.DeviceId == deviceId && x.ChannelId == channelId).First();
            var server = Common.MediaServerList.FindLast(x => x.MediaServerId == video?.MediaServerId);
            if (video == null || server == null)
            {
                rs = new ResponseStruct { Code = ErrorNumber.MediaServer_InstanceIsNull,
                    Message = "未找到通道或接收节点，不能确认停流。" };
                return false;
            }
            var gate = _gb28181LiveLocks.GetOrAdd($"{server.MediaServerId}:{video.MainId}", _ => new object());
            lock (gate)
            {
                var device = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId == deviceId);
                var channel = device?.SipChannels?.FindLast(x => x.DeviceId == channelId)
                    ?? new SipChannel { ParentId = deviceId, DeviceId = channelId };
                VideoChannelMediaInfo info;
                lock (GCommon.Ldb.LiteDBLockObj)
                    info = GCommon.Ldb.VideoOnlineInfo.FindOne(x => x.MainId == video.MainId && x.MediaServerId == server.MediaServerId);
                var port = info?.MediaServerStreamInfo?.RptPort ?? channel.RtpPort ?? 0;
                var receiverAbsent = false;
                if (port == 0 && video.DefaultRtpPort != true)
                {
                    var list = server.WebApiHelper.ListRtpServer(new ReqZLMediaKitRequestBase(), out var listResult);
                    port = list?.Data?.FindLast(x => x.Stream_Id == video.MainId)?.Port ?? 0;
                    receiverAbsent = listResult.Code == ErrorNumber.None && list?.Code == 0 && list.Data != null && port == 0;
                }
                var result = video.DefaultRtpPort == true
                    ? Gb28181StreamStop.StopShared(server, channel, video.MainId, video.App, video.Vhost, out rs, Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec)
                    : Gb28181StreamStop.Stop(server, channel, port, video.MainId, video.App, video.Vhost, false, out rs,
                        Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec, receiverAbsent);
                if (result) channel.PushStatus = PushStatus.IDLE;
                return result;
            }
        }


        /// <summary>
        /// 请求回放流
        /// </summary>
        /// <param name="record"></param>
        /// <param name="rs"></param>
        /// <param name="rtpPort"></param>
        /// <returns></returns>
        public static MediaServerStreamInfo LiveVideo(int taskId, string ssrcId, out ResponseStruct rs,
            ushort? rtpPort = 0)
        {
            ServerInstance mediaServer;
            VideoChannel videoChannel;
            SipDevice sipDevice;
            SipChannel sipChannel;
            RecordInfo.RecItem record = null;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var row = GCommon.VideoChannelRecordInfo.FindLast(x => x.TaskId.Equals(taskId));
            if (row != null && row.RecItems != null && row.RecItems.Count > 0)
            {
                record = row.RecItems.FindLast(x => x.SsrcId.Equals(ssrcId));
            }

            if (record == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_RecordNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordNotExists],
                };
                return null;
            }

            var ret = CheckIt(record.SipDevice.DeviceId, record.SipChannel.DeviceId, out rs, out mediaServer,
                out videoChannel, out sipChannel,
                out sipDevice);
            if (ret == false || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求Sip推回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(rs)}");

                return null;
            }


            VideoChannelMediaInfo mediaInfo = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                mediaInfo = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                    x.MainId.Equals(record.Stream) && x.MediaServerId.Equals(videoChannel.MediaServerId));
            }

            if (mediaInfo != null && mediaInfo.MediaServerStreamInfo != null)
            {
                GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->请求Sip推回放流成功(此回放流本身就处于推流状态)->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}");

                return mediaInfo.MediaServerStreamInfo;
            }

            ResMediaServerOpenRtpPort openRtpPort;
            if (rtpPort == null || rtpPort == 0) //如果没指定rtp端口，就申请一个
            {
                if (videoChannel.DefaultRtpPort == false) //非使用固定端口，则申请
                {
                    try
                    {
                        openRtpPort =
                            MediaServerService.MediaServerOpenRtpPort(mediaServer.MediaServerId, record.Stream,
                                out rs);
                        if (openRtpPort == null || !rs.Code.Equals(ErrorNumber.None))
                        {
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->请求Sip推回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(rs)}");

                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.MediaServer_OpenRtpPortExcept,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_OpenRtpPortExcept],
                            ExceptMessage = ex.Message,
                            ExceptStackTrace = ex.StackTrace
                        };
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->请求Sip推回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(rs)}");

                        return null;
                    }
                }
                else
                {
                    //使用固定端口就读Config中的固定端口
                    if (mediaServer.Config != null && mediaServer.Config.Data[0] != null)
                    {
                        openRtpPort = new ResMediaServerOpenRtpPort()
                        {
                            Port = (ushort)mediaServer.Config.Data[0].Rtp_Proxy_Port,
                            Stream = record.Stream,
                        };
                    }
                    else //如果Config为空，则默认使用10000
                    {
                        openRtpPort = new ResMediaServerOpenRtpPort()
                        {
                            Port = 10000,
                            Stream = record.Stream,
                        };
                    }
                }
            }
            else
            {
                openRtpPort = new ResMediaServerOpenRtpPort()
                {
                    Port = (ushort)rtpPort,
                    Stream = record.Stream,
                };
            }

            if (!openRtpPort.Stream.Trim().Equals(record.Stream))
            {
                if (videoChannel.DefaultRtpPort == false && openRtpPort != null) //失败时要关掉rtpserver
                {
                    ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                    {
                        Stream_Id = openRtpPort.Stream,
                    };

                    mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                    mediaServer.KeeperWebApi.ReleaseRtpPort(
                        openRtpPort.Port,
                        out _); //释放rtp端口
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_VideoLiveExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_VideoLiveExcept] +
                              ",Record.Stream!=OpenRtpPort.Stream",
                };


                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求Sip推回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            record.MediaServerStreamInfo = new MediaServerStreamInfo
            {
                RptPort = openRtpPort.Port, Stream = record.Stream, App = record.App, Vhost = record.Vhost
            };
            PushMediaInfo pushMediaInfo = new PushMediaInfo();
            pushMediaInfo.StreamPort = openRtpPort.Port;
            if (!string.IsNullOrEmpty(mediaServer.Candidate) && (UtilsHelper.IsIpAddr(mediaServer.Candidate) ||
                                                                 UtilsHelper.IsDomain(mediaServer.Candidate)))
            {
                pushMediaInfo.MediaServerIpAddress = mediaServer.Candidate;
            }
            else
            {
                pushMediaInfo.MediaServerIpAddress = mediaServer.IpV4Address;
            }

            string ip = pushMediaInfo.MediaServerIpAddress;

            if (!string.IsNullOrEmpty(Common.AkStreamWebConfig.PushStreamIpGetUrl) &&
                UtilsHelper.IsUrl(Common.AkStreamWebConfig.PushStreamIpGetUrl))
            {
                string url = Common.AkStreamWebConfig.PushStreamIpGetUrl.Trim();

                try
                {
                    string reqData = JsonHelper.ToJson(new ReqPushStreamGetIP()
                    {
                        DeviceId = sipDevice.DeviceId,
                    });
                    Dictionary<string, string> headers = new Dictionary<string, string>();


                    var httpRet = NetHelper.HttpPostRequest(url, headers, reqData, "utf-8",
                        Common.AkStreamWebConfig.HttpClientTimeoutSec * 1000);
                    if (!string.IsNullOrEmpty(httpRet))
                    {
                        if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_HttpClientTimeout,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                            };
                            GCommon.Logger.Error(
                                $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址失败，将执行默认ip地址->{sipDevice.DeviceId}-{sipChannel.DeviceId}->{JsonHelper.ToJson(rs)}");
                        }
                        else
                        {
                            var resRet = JsonHelper.FromJson<ResPushStreamGetIP>(httpRet);
                            if (resRet != null && !string.IsNullOrEmpty(resRet.DeviceId) &&
                                !string.IsNullOrEmpty(resRet.IpAddress))
                            {
                                ip = resRet.IpAddress.Trim();
                                GCommon.Logger.Debug(
                                    $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址成功，将执行返回中指定的ip地址->{sipDevice.DeviceId}-{sipChannel.DeviceId}->{JsonHelper.ToJson(resRet)}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    GCommon.Logger.Error(
                        $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址失败，将执行默认ip地址->{sipDevice.DeviceId}-{sipChannel.DeviceId}->{JsonHelper.ToJson(rs)}");
                }
            }

            pushMediaInfo.MediaServerIpAddress = ip; //重新设置回去，如果成功获取，将会改写ip变量的值

            pushMediaInfo.PushStreamSocketType =
                videoChannel.RtpWithTcp == true ? PushStreamSocketType.TCP : PushStreamSocketType.UDP;
            try
            {
                SipMethodProxy sipMethodProxy =
                    new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
                record.SipDevice = sipDevice;
                record.SipChannel = sipChannel;
                var liveVideoRet = sipMethodProxy.InviteRecord(record, pushMediaInfo, out rs);
                if (!rs.Code.Equals(ErrorNumber.None) || liveVideoRet == false)
                {
                    if (videoChannel.DefaultRtpPort == false && openRtpPort != null) //失败时要关掉rtpserver
                    {
                        ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                        {
                            Stream_Id = record.Stream,
                        };

                        mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                        mediaServer.KeeperWebApi.ReleaseRtpPort(
                            openRtpPort.Port,
                            out _); //释放rtp端口
                    }

                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->请求Sip推回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(rs)}");

                    return null;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_InviteExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_InviteExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };

                if (videoChannel.DefaultRtpPort == false && openRtpPort != null) //失败时要关掉rtpserver
                {
                    ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                    {
                        Stream_Id = openRtpPort.Stream,
                    };

                    mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                    mediaServer.KeeperWebApi.ReleaseRtpPort(
                        openRtpPort.Port,
                        out _); //释放rtp端口
                }

                return null;
            }

            Common.WebHookNeedReturnTask.TryRemove($"WAITONPUBLISH_{record.Stream}", out _); //如果存在老的事件等待，就先删除它
            var taskWait = new WebHookNeedReturnTask(Common.WebHookNeedReturnTask);
            AutoResetEvent myWait = new AutoResetEvent(false);
            taskWait.AutoResetEvent = myWait;
            Common.WebHookNeedReturnTask.TryAdd($"WAITONPUBLISH_{record.Stream}",
                taskWait);

            var isTimeout = myWait.WaitOne(Common.AkStreamWebConfig.WaitEventTimeOutMSec);
            if (!isTimeout)
            {
                var snapshot = new SipChannel { ParentId = record.SipDevice.DeviceId, DeviceId = record.SipChannel.DeviceId };
                if (record.InviteSipRequest != null) snapshot.InviteSipRequest = record.InviteSipRequest;
                if (record.InviteSipResponse != null) snapshot.InviteSipResponse = record.InviteSipResponse;
                if (videoChannel.DefaultRtpPort == true)
                    Gb28181StreamStop.StopShared(mediaServer, snapshot, record.Stream, record.App, record.Vhost,
                        out _, Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
                else
                    Gb28181StreamStop.Stop(mediaServer, snapshot, openRtpPort.Port, record.Stream, record.App, record.Vhost,
                        true, out _, Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WaitWebHookTimeOut,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WaitWebHookTimeOut]
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求Sip推回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(rs)}");

                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                        x.MainId.Equals(record.Stream) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                }


                return null;
            }


            ReqForWebHookOnPublish onPublishWebhook = (ReqForWebHookOnPublish)taskWait.OtherObj;
            Common.WebHookNeedReturnTask.TryRemove($"WAITONPUBLISH_{videoChannel.MainId}",
                out WebHookNeedReturnTask task);
            if (task != null)
            {
                task.Dispose();
            }


            var videoChannelMediaInfo = new VideoChannelMediaInfo();
            videoChannelMediaInfo.StreamSourceType = StreamSourceType.PlayBack;
            videoChannelMediaInfo.App = record.App;
            videoChannelMediaInfo.Enabled = videoChannel.Enabled;
            videoChannelMediaInfo.Vhost = record.Vhost;
            videoChannelMediaInfo.AutoRecord = false;
            videoChannelMediaInfo.AutoVideo = false;
            videoChannelMediaInfo.ChannelId = videoChannel.ChannelId;
            videoChannelMediaInfo.ChannelName = videoChannel.ChannelName;
            videoChannelMediaInfo.CreateTime = videoChannel.CreateTime;
            videoChannelMediaInfo.DepartmentId = videoChannel.DepartmentId;
            videoChannelMediaInfo.DepartmentName = videoChannel.DepartmentName;
            videoChannelMediaInfo.DeviceId = videoChannel.DeviceId;
            videoChannelMediaInfo.HasPtz = false;
            videoChannelMediaInfo.MainId = record.Stream;
            videoChannelMediaInfo.UpdateTime = DateTime.Now;
            videoChannelMediaInfo.DefaultRtpPort = videoChannel.DefaultRtpPort;
            videoChannelMediaInfo.DeviceNetworkType = videoChannel.DeviceNetworkType;
            videoChannelMediaInfo.DeviceStreamType = videoChannel.DeviceStreamType;
            videoChannelMediaInfo.IpV4Address = videoChannel.IpV4Address;
            videoChannelMediaInfo.IpV6Address = videoChannel.IpV6Address;
            videoChannelMediaInfo.MediaServerId = videoChannel.MediaServerId;
            videoChannelMediaInfo.NoPlayerBreak = videoChannel.NoPlayerBreak;
            videoChannelMediaInfo.PDepartmentId = videoChannel.PDepartmentId;
            videoChannelMediaInfo.PDepartmentName = videoChannel.PDepartmentName;
            videoChannelMediaInfo.RtpWithTcp = videoChannel.RtpWithTcp;
            videoChannelMediaInfo.VideoDeviceType = videoChannel.VideoDeviceType;
            videoChannelMediaInfo.VideoSrcUrl = videoChannel.VideoSrcUrl;
            videoChannelMediaInfo.MethodByGetStream = videoChannel.MethodByGetStream;
            videoChannelMediaInfo.MediaServerStreamInfo = new MediaServerStreamInfo();
            videoChannelMediaInfo.MediaServerStreamInfo.App = onPublishWebhook.App;
            videoChannelMediaInfo.MediaServerStreamInfo.Ssrc = uint.Parse(record.SsrcId);
            videoChannelMediaInfo.MediaServerStreamInfo.Stream = onPublishWebhook.Stream;
            videoChannelMediaInfo.MediaServerStreamInfo.Vhost = onPublishWebhook.Vhost;
            videoChannelMediaInfo.MediaServerStreamInfo.PlayerList = new List<MediaServerStreamPlayerInfo>();
            videoChannelMediaInfo.MediaServerStreamInfo.StartTime = DateTime.Now;
            videoChannelMediaInfo.MediaServerStreamInfo.RptPort = openRtpPort.Port;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamPort = (ushort)onPublishWebhook.Port;
            videoChannelMediaInfo.MediaServerStreamInfo.MediaServerId = onPublishWebhook.MediaServerId;
            videoChannelMediaInfo.MediaServerStreamInfo.MediaServerIp = mediaServer.IpV4Address;
            videoChannelMediaInfo.MediaServerStreamInfo.PushSocketType = pushMediaInfo.PushStreamSocketType;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamIp = onPublishWebhook.Ip;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamTcpId =
                videoChannel.RtpWithTcp == true ? onPublishWebhook.Id : null;
            videoChannelMediaInfo.MediaServerStreamInfo.Params = onPublishWebhook.Params;
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl = new List<string>();
            string exInfo =
                (!string.IsNullOrEmpty(onPublishWebhook.Vhost) &&
                 !onPublishWebhook.Vhost.Trim().ToLower().Equals("__defaultvhost__"))
                    ? $"?vhost={onPublishWebhook.Vhost}"
                    : "";
            var prefix = Common.AkStreamWebConfig.ZlmFlvPrefix;
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = "";
            }

            prefix = prefix.Trim();
            if (!prefix.EndsWith(".") && !string.IsNullOrEmpty(prefix))
            {
                prefix += ".";
            }

            if (mediaServer.UseSsl)
            {
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtsps://{mediaServer.IpV4Address}:{mediaServer.RtspsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtmps://{mediaServer.IpV4Address}:{mediaServer.RtmpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}/hls.m3u8{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");
            }

            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"rtsp://{mediaServer.IpV4Address}:{mediaServer.RtspPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"rtmp://{mediaServer.IpV4Address}:{mediaServer.RtmpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}/hls.m3u8{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");

            lock (GCommon.Ldb.LiteDBLockObj)
            {
                GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                    x.MainId.Equals(record.Stream) && x.MediaServerId.Equals(videoChannel.MediaServerId));

                GCommon.Ldb.VideoOnlineInfo.Insert(videoChannelMediaInfo);
            }


            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->请求Sip推回放流成功->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(videoChannelMediaInfo.MediaServerStreamInfo)}");

            return videoChannelMediaInfo.MediaServerStreamInfo;
        }

        /// <summary>
        /// 请求GB28181直播流
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="rs"></param>
        /// <param name="rtpPort">保持空或者0，将自动申请端口</param>
        /// <returns></returns>
        public static MediaServerStreamInfo LiveVideo(string deviceId, string channelId, out ResponseStruct rs,
            ushort? rtpPort = 0)
        {
            ServerInstance mediaServer;
            VideoChannel videoChannel;
            SipDevice sipDevice;
            SipChannel sipChannel;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = CheckIt(deviceId, channelId, out rs, out mediaServer, out videoChannel, out sipChannel,
                out sipDevice);
            if (ret == false || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            var liveLockKey = $"{videoChannel.MediaServerId}:{videoChannel.MainId}";
            var liveLockObj = _gb28181LiveLocks.GetOrAdd(liveLockKey, _ => new object());

            lock (liveLockObj)
            {
                VideoChannelMediaInfo mediaInfo = null;

                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    mediaInfo = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                        x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                }

                if (mediaInfo != null && mediaInfo.MediaServerStreamInfo != null)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->请求Sip推流成功(此Sip通道本身就处于推流状态)->{deviceId}-{channelId}");

                    mediaInfo.AutoRecord = videoChannel.AutoRecord;
                    mediaInfo.AutoVideo = videoChannel.AutoVideo;
                    mediaInfo.RecordPlanName = videoChannel.RecordPlanName;
                    mediaInfo.RecordSecs = videoChannel.RecordSecs;

                    AutoRecordStartHelper.TryStartAutoRecordAfterStreamReady(mediaInfo, "GB28181LiveVideoExisting");

                    return mediaInfo.MediaServerStreamInfo;

                    // GCommon.Logger.Info($"[{Common.LoggerHead}]->请求Sip推流成功(此Sip通道本身就处于推流状态)->{deviceId}-{channelId}");
                    //
                    // return mediaInfo.MediaServerStreamInfo;
                }


                var pendingStops = mediaServer.KeeperWebApi.GetPendingRtpPortStops(out rs);
                if (pendingStops == null || rs.Code != ErrorNumber.None) return null;
                if (pendingStops.Exists(x => x.MediaServerId == mediaServer.MediaServerId &&
                    x.DeviceId == deviceId && x.ChannelId == channelId && !x.Playback))
                {
                    rs = new ResponseStruct { Code = ErrorNumber.Sip_DeInviteExcept,
                        Message = "该通道上次停流尚未确认，接收端口保持隔离，请重试清理后再拉流。" };
                    return null;
                }
                ResMediaServerOpenRtpPort openRtpPort;
                if (rtpPort == null || rtpPort == 0) //如果没指定rtp端口，就申请一个
                {
                    if (videoChannel.DefaultRtpPort == false) //非使用固定端口，则申请
                    {
                        try
                        {
                            openRtpPort =
                                MediaServerService.MediaServerOpenRtpPort(mediaServer.MediaServerId,
                                    videoChannel.MainId,
                                    out rs);
                            if (openRtpPort == null || !rs.Code.Equals(ErrorNumber.None))
                            {
                                GCommon.Logger.Warn(
                                    $"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.MediaServer_OpenRtpPortExcept,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_OpenRtpPortExcept],
                                ExceptMessage = ex.Message,
                                ExceptStackTrace = ex.StackTrace
                            };
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                            return null;
                        }
                    }
                    else
                    {
                        //使用固定端口就读Config中的固定端口
                        if (mediaServer.Config != null && mediaServer.Config.Data[0] != null)
                        {
                            openRtpPort = new ResMediaServerOpenRtpPort()
                            {
                                Port = (ushort)mediaServer.Config.Data[0].Rtp_Proxy_Port,
                                Stream = videoChannel.MainId,
                            };
                        }
                        else //如果Config为空，则默认使用10000
                        {
                            openRtpPort = new ResMediaServerOpenRtpPort()
                            {
                                Port = 10000,
                                Stream = videoChannel.MainId,
                            };
                        }
                    }
                }
                else
                {
                    openRtpPort = new ResMediaServerOpenRtpPort()
                    {
                        Port = (ushort)rtpPort,
                        Stream = videoChannel.MainId,
                    };
                }

                if (!openRtpPort.Stream.Trim().Equals(sipChannel.Stream))
                {
                    if (videoChannel.DefaultRtpPort == false && openRtpPort != null) //失败时要关掉rtpserver
                    {
                        ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                        {
                            Stream_Id = openRtpPort.Stream,
                        };

                        mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                        mediaServer.KeeperWebApi.ReleaseRtpPort(
                            openRtpPort.Port,
                            out _); //释放rtp端口
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_VideoLiveExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_VideoLiveExcept] +
                                  ",SipChannel.Stream!=OpenRtpPort.Stream",
                    };


                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                    return null;
                }

                // 在线记录可能先被注销回调删除，原 dialog 必须始终保留实际接收端口。
                sipChannel.RtpPort = openRtpPort.Port;
                sipChannel.App = videoChannel.App;
                sipChannel.Vhost = videoChannel.Vhost;
                PushMediaInfo pushMediaInfo = new PushMediaInfo();
                pushMediaInfo.StreamPort = openRtpPort.Port;
                if (!string.IsNullOrEmpty(mediaServer.Candidate) && (UtilsHelper.IsIpAddr(mediaServer.Candidate) ||
                                                                     UtilsHelper.IsDomain(mediaServer.Candidate)))
                {
                    pushMediaInfo.MediaServerIpAddress = mediaServer.Candidate;
                }
                else
                {
                    pushMediaInfo.MediaServerIpAddress = mediaServer.IpV4Address;
                }

                string ip = pushMediaInfo.MediaServerIpAddress;

                if (!string.IsNullOrEmpty(Common.AkStreamWebConfig.PushStreamIpGetUrl) &&
                    UtilsHelper.IsUrl(Common.AkStreamWebConfig.PushStreamIpGetUrl))
                {
                    string url = Common.AkStreamWebConfig.PushStreamIpGetUrl.Trim();

                    try
                    {
                        string reqData = JsonHelper.ToJson(new ReqPushStreamGetIP()
                        {
                            DeviceId = deviceId,
                        });
                        Dictionary<string, string> headers = new Dictionary<string, string>();


                        var httpRet = NetHelper.HttpPostRequest(url, headers, reqData, "utf-8",
                            Common.AkStreamWebConfig.HttpClientTimeoutSec * 1000);
                        if (!string.IsNullOrEmpty(httpRet))
                        {
                            if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                            {
                                rs = new ResponseStruct()
                                {
                                    Code = ErrorNumber.Sys_HttpClientTimeout,
                                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                                };
                                GCommon.Logger.Error(
                                    $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址失败，将执行默认ip地址->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");
                            }
                            else
                            {
                                var resRet = JsonHelper.FromJson<ResPushStreamGetIP>(httpRet);
                                if (resRet != null && !string.IsNullOrEmpty(resRet.DeviceId) &&
                                    !string.IsNullOrEmpty(resRet.IpAddress))
                                {
                                    ip = resRet.IpAddress.Trim();
                                    GCommon.Logger.Debug(
                                        $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址成功，将执行返回中指定的ip地址->{deviceId}-{channelId}->{JsonHelper.ToJson(resRet)}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.MediaServer_WebApiExcept,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                            ExceptMessage = ex.Message,
                            ExceptStackTrace = ex.StackTrace,
                        };
                        GCommon.Logger.Error(
                            $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址失败，将执行默认ip地址->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");
                    }
                }

                pushMediaInfo.MediaServerIpAddress = ip; //重新设置回去，如果成功获取，将会改写ip变量的值

                pushMediaInfo.PushStreamSocketType =
                    videoChannel.RtpWithTcp == true ? PushStreamSocketType.TCP : PushStreamSocketType.UDP;
                try
                {
                    SipMethodProxy sipMethodProxy =
                        new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
                    var liveVideoRet = sipMethodProxy.Invite(sipChannel, pushMediaInfo, out rs);
                    if (!rs.Code.Equals(ErrorNumber.None) || liveVideoRet == false)
                    {
                        if (videoChannel.DefaultRtpPort == false && openRtpPort != null) //失败时要关掉rtpserver
                        {
                            ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                            {
                                Stream_Id = openRtpPort.Stream,
                            };

                            mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                            mediaServer.KeeperWebApi.ReleaseRtpPort(
                                openRtpPort.Port,
                                out _); //释放rtp端口
                        }

                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                        return null;
                    }
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_InviteExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_InviteExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };

                    if (videoChannel.DefaultRtpPort == false && openRtpPort != null) //失败时要关掉rtpserver
                    {
                        ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                        {
                            Stream_Id = openRtpPort.Stream,
                        };

                        mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                        mediaServer.KeeperWebApi.ReleaseRtpPort(
                            openRtpPort.Port,
                            out _); //释放rtp端口
                    }

                    return null;
                }

                Common.WebHookNeedReturnTask.TryRemove($"WAITONPUBLISH_{videoChannel.MainId}",
                    out _); //如果存在老的事件等待，就先删除它
                var taskWait = new WebHookNeedReturnTask(Common.WebHookNeedReturnTask);
                AutoResetEvent myWait = new AutoResetEvent(false);
                taskWait.AutoResetEvent = myWait;
                Common.WebHookNeedReturnTask.TryAdd($"WAITONPUBLISH_{videoChannel.MainId}",
                    taskWait);


                var isTimeout = myWait.WaitOne(Common.AkStreamWebConfig.WaitEventTimeOutMSec);
                if (!isTimeout)
                {
                    Common.WebHookNeedReturnTask.TryRemove($"WAITONPUBLISH_{videoChannel.MainId}",
                        out WebHookNeedReturnTask timeoutTask);
                    if (timeoutTask != null)
                    {
                        timeoutTask.Dispose();
                    }

                    if (videoChannel.DefaultRtpPort == true)
                        Gb28181StreamStop.StopShared(mediaServer, sipChannel, videoChannel.MainId,
                            videoChannel.App, videoChannel.Vhost, out _, Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
                    else
                        Gb28181StreamStop.Stop(mediaServer, sipChannel, openRtpPort.Port, videoChannel.MainId,
                            videoChannel.App, videoChannel.Vhost, false, out _, Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WaitWebHookTimeOut,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WaitWebHookTimeOut]
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                            x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                    }

                    return null;
                }

                //var isTimeout = myWait.WaitOne(Common.AkStreamWebConfig.WaitEventTimeOutMSec);
                // if (!isTimeout)
                // {
                //     try
                //     {
                //         sipChannel.App = videoChannel.App;
                //         sipChannel.RtpPort = openRtpPort.Port;
                //         sipChannel.Vhost = videoChannel.Vhost;
                //         sipChannel.PushStatus = PushStatus.IDLE;
                //         var mediaList =
                //             mediaServer.WebApiHelper.GetMediaList(new ReqZLMediaKitGetMediaList(), out rs);
                //         if (mediaList != null && mediaList.Code == 0 && mediaList.Data != null && mediaList.Data.Count > 0)
                //         {
                //             var media = mediaList.Data.FindLast(x => x.App.Equals(videoChannel.App) &&
                //                                                      x.Stream.Equals(videoChannel.MainId)
                //                                                      && x.Vhost.Equals(videoChannel.Vhost));
                //             if (media != null)
                //             {
                //                 var tmp = new ReqZLMediaKitCloseStreams()
                //                 {
                //                     App = media.App,
                //                     Force = true,
                //                     Stream = videoChannel.MainId,
                //                     Vhost = videoChannel.Vhost,
                //                 };
                //
                //                 mediaServer.WebApiHelper.CloseStreams(tmp, out rs);
                //                 SipMethodProxy sipMethodProxy =
                //                     new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
                //                 sipMethodProxy.DeInvite(sipChannel, out rs);
                //             }
                //         }
                //     }
                //     catch (Exception ex)
                //     {
                //         GCommon.Logger.Error(
                //             $"[{Common.LoggerHead}]->ZLMediaKit On_Publish事件回调超时发现StreamList中已存在此流，实行断开时发生异常->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");
                //     }
                //
                //     if (videoChannel.DefaultRtpPort == false && openRtpPort != null &&
                //         mediaServer.RandomPort == false) //失败时要关掉rtpserver
                //     {
                //         ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                //         {
                //             Stream_Id = openRtpPort.Stream,
                //         };
                //
                //         mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                //         mediaServer.KeeperWebApi.ReleaseRtpPort(
                //             openRtpPort.Port,
                //             out _); //释放rtp端口
                //     }
                //
                //     try
                //     {
                //         MediaServerService.StreamStop(mediaServer.MediaServerId, videoChannel.MainId, out _);
                //     }
                //     catch
                //     {
                //         //
                //     }
                //
                //     rs = new ResponseStruct()
                //     {
                //         Code = ErrorNumber.MediaServer_WaitWebHookTimeOut,
                //         Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WaitWebHookTimeOut]
                //     };
                //     GCommon.Logger.Warn(
                //         $"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");
                //
                //     lock (GCommon.Ldb.LiteDBLockObj)
                //     {
                //         GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                //             x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                //     }
                //
                //     return null;
                // }

                ReqForWebHookOnPublish onPublishWebhook = (ReqForWebHookOnPublish)taskWait.OtherObj;
                Common.WebHookNeedReturnTask.TryRemove($"WAITONPUBLISH_{videoChannel.MainId}",
                    out WebHookNeedReturnTask task);
                if (task != null)
                {
                    task.Dispose();
                }


                var videoChannelMediaInfo = new VideoChannelMediaInfo();
                videoChannelMediaInfo.StreamSourceType = StreamSourceType.Live;
                videoChannelMediaInfo.App = videoChannel.App;
                videoChannelMediaInfo.Enabled = videoChannel.Enabled;
                videoChannelMediaInfo.Vhost = videoChannel.Vhost;
                videoChannelMediaInfo.AutoRecord = videoChannel.AutoRecord;
                videoChannelMediaInfo.AutoVideo = videoChannel.AutoVideo;
                videoChannelMediaInfo.RecordPlanName = videoChannel.RecordPlanName;
                videoChannelMediaInfo.RecordSecs = videoChannel.RecordSecs;
                videoChannelMediaInfo.ChannelId = videoChannel.ChannelId;
                videoChannelMediaInfo.ChannelName = videoChannel.ChannelName;
                videoChannelMediaInfo.CreateTime = videoChannel.CreateTime;
                videoChannelMediaInfo.DepartmentId = videoChannel.DepartmentId;
                videoChannelMediaInfo.DepartmentName = videoChannel.DepartmentName;
                videoChannelMediaInfo.DeviceId = videoChannel.DeviceId;
                videoChannelMediaInfo.HasPtz = videoChannel.HasPtz;
                videoChannelMediaInfo.MainId = videoChannel.MainId;
                videoChannelMediaInfo.UpdateTime = videoChannel.UpdateTime;
                videoChannelMediaInfo.DefaultRtpPort = videoChannel.DefaultRtpPort;
                videoChannelMediaInfo.DeviceNetworkType = videoChannel.DeviceNetworkType;
                videoChannelMediaInfo.DeviceStreamType = videoChannel.DeviceStreamType;
                videoChannelMediaInfo.IpV4Address = videoChannel.IpV4Address;
                videoChannelMediaInfo.IpV6Address = videoChannel.IpV6Address;
                videoChannelMediaInfo.MediaServerId = videoChannel.MediaServerId;
                videoChannelMediaInfo.NoPlayerBreak = videoChannel.NoPlayerBreak;
                videoChannelMediaInfo.PDepartmentId = videoChannel.PDepartmentId;
                videoChannelMediaInfo.PDepartmentName = videoChannel.PDepartmentName;
                videoChannelMediaInfo.RtpWithTcp = videoChannel.RtpWithTcp;
                videoChannelMediaInfo.VideoDeviceType = videoChannel.VideoDeviceType;
                videoChannelMediaInfo.VideoSrcUrl = videoChannel.VideoSrcUrl;
                videoChannelMediaInfo.MethodByGetStream = videoChannel.MethodByGetStream;
                videoChannelMediaInfo.MediaServerStreamInfo = new MediaServerStreamInfo();
                videoChannelMediaInfo.MediaServerStreamInfo.App = onPublishWebhook.App;
                videoChannelMediaInfo.MediaServerStreamInfo.Ssrc = uint.Parse(sipChannel.SsrcId);
                videoChannelMediaInfo.MediaServerStreamInfo.Stream = onPublishWebhook.Stream;
                videoChannelMediaInfo.MediaServerStreamInfo.Vhost = onPublishWebhook.Vhost;
                videoChannelMediaInfo.MediaServerStreamInfo.PlayerList = new List<MediaServerStreamPlayerInfo>();
                videoChannelMediaInfo.MediaServerStreamInfo.StartTime = DateTime.Now;
                videoChannelMediaInfo.MediaServerStreamInfo.RptPort = openRtpPort.Port;
                videoChannelMediaInfo.MediaServerStreamInfo.StreamPort = (ushort)onPublishWebhook.Port;
                videoChannelMediaInfo.MediaServerStreamInfo.MediaServerId = onPublishWebhook.MediaServerId;
                videoChannelMediaInfo.MediaServerStreamInfo.MediaServerIp = mediaServer.IpV4Address;
                videoChannelMediaInfo.MediaServerStreamInfo.PushSocketType = pushMediaInfo.PushStreamSocketType;
                videoChannelMediaInfo.MediaServerStreamInfo.StreamIp = onPublishWebhook.Ip;
                videoChannelMediaInfo.MediaServerStreamInfo.StreamTcpId =
                    videoChannel.RtpWithTcp == true ? onPublishWebhook.Id : null;
                videoChannelMediaInfo.MediaServerStreamInfo.Params = onPublishWebhook.Params;
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl = new List<string>();
                string exInfo =
                    (!string.IsNullOrEmpty(onPublishWebhook.Vhost) &&
                     !onPublishWebhook.Vhost.Trim().ToLower().Equals("__defaultvhost__"))
                        ? $"?vhost={onPublishWebhook.Vhost}"
                        : "";
                var prefix = Common.AkStreamWebConfig.ZlmFlvPrefix;
                if (string.IsNullOrEmpty(prefix))
                {
                    prefix = "";
                }

                prefix = prefix.Trim();
                if (!prefix.EndsWith(".") && !string.IsNullOrEmpty(prefix))
                {
                    prefix += ".";
                }

                if (mediaServer.UseSsl)
                {
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"rtsps://{mediaServer.IpV4Address}:{mediaServer.RtspsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"rtmps://{mediaServer.IpV4Address}:{mediaServer.RtmpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}/hls.m3u8{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");
                    videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                        $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");
                }

                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtsp://{mediaServer.IpV4Address}:{mediaServer.RtspPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtmp://{mediaServer.IpV4Address}:{mediaServer.RtmpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}/hls.m3u8{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.live.mp4{exInfo}");

                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                        x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));

                    GCommon.Ldb.VideoOnlineInfo.Insert(videoChannelMediaInfo);
                }

                GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->请求Sip推流成功->{deviceId}-{channelId}->{JsonHelper.ToJson(videoChannelMediaInfo.MediaServerStreamInfo)}");

                AutoRecordStartHelper.TryStartAutoRecordAfterStreamReady(videoChannelMediaInfo, "GB28181LiveVideo");

                return videoChannelMediaInfo.MediaServerStreamInfo;

                // GCommon.Logger.Debug(
                //     $"[{Common.LoggerHead}]->请求Sip推流成功->{deviceId}-{channelId}->{JsonHelper.ToJson(videoChannelMediaInfo.MediaServerStreamInfo)}");
                //
                // return videoChannelMediaInfo.MediaServerStreamInfo;
            }
        }

        /// <summary>
        /// 获取通道是否正在推流
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool IsLiveVideo(string deviceId, string channelId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (UtilsHelper.StringIsNullEx(deviceId) || UtilsHelper.StringIsNullEx(channelId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->检查Sip推流状态失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            var tmpSipDevice = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId.Equals(deviceId));

            if (tmpSipDevice == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->检查Sip推流状态失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            var tmpSipChannel = tmpSipDevice.SipChannels.FindLast(x => x.DeviceId.Equals(channelId));
            if (tmpSipChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_ChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_ChannelNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->检查Sip推流状态失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->检查Sip推流状态成功->{deviceId}-{channelId}->{JsonHelper.ToJson(tmpSipChannel.PushStatus)}");

            VideoChannelMediaInfo obj = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                obj = GCommon.Ldb.VideoOnlineInfo.FindOne(x => x.MainId.Equals(tmpSipChannel.Stream));
            }

            if (obj != null && obj.MediaServerStreamInfo != null)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// PTZ控制
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool PtzCtrl(ReqPtzCtrl ptzCmd, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (UtilsHelper.StringIsNullEx(ptzCmd.DeviceId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->PTZ控制失败->{ptzCmd.DeviceId}-{ptzCmd.ChannelId}-{JsonHelper.ToJson(ptzCmd)}->{JsonHelper.ToJson(rs)}");

                return false;
            }


            var tmpSipDevice = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId.Equals(ptzCmd.DeviceId));

            if (tmpSipDevice == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->PTZ控制失败->{ptzCmd.DeviceId}-{ptzCmd.ChannelId}-{JsonHelper.ToJson(ptzCmd)}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            SipChannel tmpSipChannel = null;
            if (!UtilsHelper.StringIsNullEx(ptzCmd.ChannelId))
            {
                tmpSipChannel = tmpSipDevice.SipChannels.FindLast(x => x.DeviceId.Equals(ptzCmd.ChannelId));
                if (tmpSipChannel == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_ChannelNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_ChannelNotExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->PTZ控制失败->{ptzCmd.DeviceId}-{ptzCmd.ChannelId}-{JsonHelper.ToJson(ptzCmd)}->{JsonHelper.ToJson(rs)}");

                    return false;
                }
            }

            PtzCtrl ptzCtrl = new PtzCtrl();
            ptzCtrl.Speed = ptzCmd.Speed;
            ptzCtrl.SipChannel = tmpSipChannel == null ? null : tmpSipChannel;
            ptzCtrl.SipDevice = tmpSipDevice;
            ptzCtrl.PtzCommandType = ptzCmd.PtzCommandType;
            SipMethodProxy sipMethodProxy = new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
            var ptz = sipMethodProxy.PtzMove(ptzCtrl, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || ptz == false)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->PTZ控制失败->{ptzCmd.DeviceId}-{ptzCmd.ChannelId}{JsonHelper.ToJson(ptzCmd)}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->PTZ控制成功->{ptzCmd.DeviceId}-{ptzCmd.ChannelId}{JsonHelper.ToJson(ptzCmd)}");

            return true;
        }

        /// <summary>
        /// 根据ID获取Sip通道
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SipChannel GetSipChannelById(string deviceId, string channelId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (UtilsHelper.StringIsNullEx(deviceId) || UtilsHelper.StringIsNullEx(channelId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取Sip通道失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            var tmpSipDevice = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId.Equals(deviceId));

            if (tmpSipDevice == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取Sip通道失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            var tmpSipChannel = tmpSipDevice.SipChannels.FindLast(x => x.DeviceId.Equals(channelId));
            if (tmpSipChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_ChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_ChannelNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取Sip通道失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->获取Sip通道成功->{deviceId}-{channelId}->{JsonHelper.ToJson(tmpSipChannel)}");

            return tmpSipChannel;
        }

        /// <summary>
        /// 通过DeviceId获取Device设备实例
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SipDevice GetSipDeviceListByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (UtilsHelper.StringIsNullEx(deviceId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取Sip设备失败->{deviceId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            var ret = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId.Equals(deviceId));
            if (ret == null)
            {
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取Sip设备失败->{deviceId}->结果为空");
            }
            else
            {
                GCommon.Logger.Debug($"[{Common.LoggerHead}]->获取Sip设备成功->{deviceId}->{JsonHelper.ToJson(ret)}");
            }

            return ret;
        }

        /// <summary>
        /// 获取所有Sip设备列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<SipDevice> GetSipDeviceList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->获取Sip设备列表成功->{JsonHelper.ToJson(LibGB28181SipServer.Common.SipDevices.Count)}");
            return LibGB28181SipServer.Common.SipDevices;
        }

        /// <summary>
        /// 用于回放录像时的拖动
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="ssrcId"></param>
        /// <param name="time"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool RecordVideoSeekPosition(int taskId, uint ssrcId, long time, out ResponseStruct rs)
        {
            ServerInstance mediaServer;
            VideoChannel videoChannel;
            SipDevice sipDevice;
            SipChannel sipChannel;
            bool liveVideoRet = false;
            RecordInfo.RecItem record = null;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var row = GCommon.VideoChannelRecordInfo.FindLast(x => x.TaskId.Equals(taskId));
            if (row != null && row.RecItems != null && row.RecItems.Count > 0)
            {
                record = row.RecItems.FindLast(x => x.SsrcId.Equals(ssrcId.ToString()));
            }

            if (record == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_RecordNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordNotExists],
                };
                return false;
            }

            var ret = CheckIt(record.SipDevice.DeviceId, record.SipChannel.DeviceId, out rs, out mediaServer,
                out videoChannel, out sipChannel,
                out sipDevice);
            if (ret == false || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->拖拽Sip推回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            VideoChannelMediaInfo vBack = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                vBack = GCommon.Ldb.VideoOnlineInfo.FindOne(x => x.MediaServerStreamInfo.Ssrc == ssrcId);
            }

            ResMediaServerOpenRtpPort openRtpPort;
            openRtpPort = new ResMediaServerOpenRtpPort()
            {
                Port = (ushort)vBack.MediaServerStreamInfo.StreamPort,
                Stream = record.Stream,
            };

            PushMediaInfo pushMediaInfo = new PushMediaInfo();
            pushMediaInfo.StreamPort = openRtpPort.Port;
            if (!string.IsNullOrEmpty(mediaServer.Candidate) && (UtilsHelper.IsIpAddr(mediaServer.Candidate) ||
                                                                 UtilsHelper.IsDomain(mediaServer.Candidate)))
            {
                pushMediaInfo.MediaServerIpAddress = mediaServer.Candidate;
            }
            else
            {
                pushMediaInfo.MediaServerIpAddress = mediaServer.IpV4Address;
            }

            string ip = pushMediaInfo.MediaServerIpAddress;

            if (!string.IsNullOrEmpty(Common.AkStreamWebConfig.PushStreamIpGetUrl) &&
                UtilsHelper.IsUrl(Common.AkStreamWebConfig.PushStreamIpGetUrl))
            {
                string url = Common.AkStreamWebConfig.PushStreamIpGetUrl.Trim();

                try
                {
                    string reqData = JsonHelper.ToJson(new ReqPushStreamGetIP()
                    {
                        DeviceId = sipDevice.DeviceId,
                    });
                    Dictionary<string, string> headers = new Dictionary<string, string>();


                    var httpRet = NetHelper.HttpPostRequest(url, headers, reqData, "utf-8",
                        Common.AkStreamWebConfig.HttpClientTimeoutSec * 1000);
                    if (!string.IsNullOrEmpty(httpRet))
                    {
                        if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_HttpClientTimeout,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                            };
                            GCommon.Logger.Error(
                                $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址失败，将执行默认ip地址->{sipDevice.DeviceId}-{sipChannel.DeviceId}->{JsonHelper.ToJson(rs)}");
                        }
                        else
                        {
                            var resRet = JsonHelper.FromJson<ResPushStreamGetIP>(httpRet);
                            if (resRet != null && !string.IsNullOrEmpty(resRet.DeviceId) &&
                                !string.IsNullOrEmpty(resRet.IpAddress))
                            {
                                ip = resRet.IpAddress.Trim();
                                GCommon.Logger.Debug(
                                    $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址成功，将执行返回中指定的ip地址->{sipDevice.DeviceId}-{sipChannel.DeviceId}->{JsonHelper.ToJson(resRet)}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    GCommon.Logger.Error(
                        $"[{Common.LoggerHead}]->请求Sip推流->获取指定流媒体服务器ip地址失败，将执行默认ip地址->{sipDevice.DeviceId}-{sipChannel.DeviceId}->{JsonHelper.ToJson(rs)}");
                }
            }

            pushMediaInfo.MediaServerIpAddress = ip; //重新设置回去，如果成功获取，将会改写ip变量的值

            pushMediaInfo.PushStreamSocketType =
                videoChannel.RtpWithTcp == true ? PushStreamSocketType.TCP : PushStreamSocketType.UDP;
            try
            {
                SipMethodProxy sipMethodProxy = new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
                liveVideoRet = sipMethodProxy.InviteRecordPosition(record, pushMediaInfo, time, out rs);
            }
            catch (Exception ex)
            {
                return false;
            }

            var taskWait = new WebHookNeedReturnTask(Common.WebHookNeedReturnTask);
            AutoResetEvent myWait = new AutoResetEvent(false);
            taskWait.AutoResetEvent = myWait;
            Common.WebHookNeedReturnTask.TryAdd($"WAITONPUBLISH_{record.Stream}",
                taskWait);

            var isTimeout = myWait.WaitOne(Common.AkStreamWebConfig.WaitEventTimeOutMSec);

            ReqForWebHookOnPublish onPublishWebhook = (ReqForWebHookOnPublish)taskWait.OtherObj;
            Common.WebHookNeedReturnTask.TryRemove($"WAITONPUBLISH_{videoChannel.MainId}",
                out WebHookNeedReturnTask task);
            if (task != null)
            {
                task.Dispose();
            }

            if (liveVideoRet)
            {
                GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->拖拽Sip回放流成功->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}");
            }
            else
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->拖拽Sip回放流失败->{record.SipDevice.DeviceId}-{record.SipChannel.DeviceId}-{record.Stream}->liveVideoRet:{liveVideoRet}");
            }

            return liveVideoRet;
        }
    }
}
