using System;
using System.Collections.Generic;
using System.Threading;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using LibGB28181SipServer;
using LibLogger;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebHookRequest;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;

namespace AKStreamWeb.Services
{
    public static class SipServerService
    {
        /// <summary>
        /// 获取sip设备的历史录制文件列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="queryRecordFile"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GetHistroyRecordFileList(string deviceId, string channelId,
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
                Logger.Warn(
                    $"[{Common.LoggerHead}]->获取Sip设备历史录制文件失败->{deviceId}-{channelId}-{JsonHelper.ToJson(queryRecordFile)}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            SipMethodProxy sipMethodProxy = new SipMethodProxy(20000);
            var got = sipMethodProxy.QueryRecordFileList(sipChannel, queryRecordFile, out rs); //获取历史文件
            if (!rs.Code.Equals(ErrorNumber.None) || got == false)
            {
                Logger.Warn(
                    $"[{Common.LoggerHead}]->获取Sip设备历史录制文件失败->{deviceId}-{channelId}-{JsonHelper.ToJson(queryRecordFile)}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            Logger.Warn(
                $"[{Common.LoggerHead}]->获取Sip设备历史录制文件成功->{deviceId}-{channelId}-{JsonHelper.ToJson(queryRecordFile)}->加载可能较慢，请定时刷新SipChannel下的LastRecordInfos字段");

            return true;
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

            if (videoChannel.Enabled == false || videoChannel.MediaServerId.Contains("unknown_server"))
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
        /// 停止GB28181设备推流
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool StopLiveVideo(string deviceId, string channelId, out ResponseStruct rs)
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
                Logger.Warn($"[{Common.LoggerHead}]->停止Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            VideoChannelMediaInfo mediaInfo = null;
            lock (Common.VideoChannelMediaInfosLock)
            {
                mediaInfo = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));
            }

            if (mediaInfo == null || mediaInfo.MediaServerStreamInfo == null)
            {
                Logger.Info($"[{Common.LoggerHead}]->停止Sip推流成功(此Sip通道本身就处于停止推流状态)->{deviceId}-{channelId}");

                return true;
            }


            try
            {
                SipMethodProxy sipMethodProxy = new SipMethodProxy(20000);
                var retDeInvite = sipMethodProxy.DeInvite(sipChannel, out rs); //通知sip设备停止推流

                ReqZLMediaKitCloseStreams reqZlMediaKitCloseStreams = new ReqZLMediaKitCloseStreams()
                {
                    App = mediaInfo.App,
                    Force = true,
                    Stream = mediaInfo.MediaServerStreamInfo.Stream,
                    Vhost = mediaInfo.Vhost,
                };
                mediaServer.WebApiHelper.CloseStreams(reqZlMediaKitCloseStreams, out rs); //关掉流
                if (videoChannel.DefaultRtpPort == false)
                {
                    ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                    {
                        Stream_Id = sipChannel.Stream,
                    };

                    mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out rs); //关掉rtp端口
                    mediaServer.KeeperWebApi.ReleaseRtpPort(
                        (ushort) mediaInfo.MediaServerStreamInfo.RptPort,
                        out rs); //释放rtp端口
                }


                if (!rs.Code.Equals(ErrorNumber.None))
                {
                    lock (Common.VideoChannelMediaInfosLock)
                    {
                        var obj = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));
                        if (obj != null)
                        {
                            Common.VideoChannelMediaInfos.Remove(obj);
                        }
                    }

                    Logger.Warn(
                        $"[{Common.LoggerHead}]->停止Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                    return false;
                }

                lock (Common.VideoChannelMediaInfosLock)
                {
                    var obj = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));
                    if (obj != null)
                    {
                        Common.VideoChannelMediaInfos.Remove(obj);
                    }
                }

                Logger.Info($"[{Common.LoggerHead}]->停止Sip推流成功->{deviceId}-{channelId}->{retDeInvite}");

                return true;
            }
            catch (Exception ex)
            {
                lock (Common.VideoChannelMediaInfosLock)
                {
                    var obj = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));
                    if (obj != null)
                    {
                        Common.VideoChannelMediaInfos.Remove(obj);
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Other,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace
                };
                throw new AkStreamException(rs);
            }
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
                Logger.Warn($"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            VideoChannelMediaInfo mediaInfo = null;
            mediaInfo = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));

            lock (Common.VideoChannelMediaInfosLock)
            {
                mediaInfo = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));

                if (mediaInfo != null && mediaInfo.MediaServerStreamInfo != null)
                {
                    Logger.Info($"[{Common.LoggerHead}]->请求Sip推流成功(此Sip通道本身就处于推流状态)->{deviceId}-{channelId}");

                    return mediaInfo.MediaServerStreamInfo;
                }
            }


            ResMediaServerOpenRtpPort openRtpPort;
            if (rtpPort == null || rtpPort == 0) //如果没指定rtp端口，就申请一个
            {
                if (videoChannel.DefaultRtpPort == false) //非使用固定端口，则申请
                {
                    try
                    {
                        openRtpPort =
                            MediaServerService.MediaServerOpenRtpPort(mediaServer.MediaServerId, videoChannel.MainId,
                                out rs);
                        if (openRtpPort == null || !rs.Code.Equals(ErrorNumber.None))
                        {
                            Logger.Warn(
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
                        Logger.Warn(
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
                            Port = (ushort) mediaServer.Config.Data[0].Rtp_Proxy_Port,
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
                    Port = (ushort) rtpPort,
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


                Logger.Warn($"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            PushMediaInfo pushMediaInfo = new PushMediaInfo();
            pushMediaInfo.StreamPort = openRtpPort.Port;
            pushMediaInfo.MediaServerIpAddress = mediaServer.IpV4Address;
            pushMediaInfo.PushStreamSocketType =
                videoChannel.RtpWithTcp == true ? PushStreamSocketType.TCP : PushStreamSocketType.UDP;
            SipMethodProxy sipMethodProxy = new SipMethodProxy(20000);
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

                Logger.Warn($"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            var taskWait = new WebHookNeedReturnTask(Common.WebHookNeedReturnTask);
            AutoResetEvent myWait = new AutoResetEvent(false);
            taskWait.AutoResetEvent = myWait;
            Common.WebHookNeedReturnTask.TryAdd($"WAITONPUBLISH_{videoChannel.MainId}",
                taskWait);

            var isTimeout = myWait.WaitOne(20000);
            if (!isTimeout)
            {
                if (videoChannel.DefaultRtpPort == false && openRtpPort != null &&
                    mediaServer.RandomPort == false) //失败时要关掉rtpserver
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

                try
                {
                    MediaServerService.StreamStop(mediaServer.MediaServerId, videoChannel.MainId, out _);
                }
                catch
                {
                    //
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WaitWebHookTimeOut,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WaitWebHookTimeOut]
                };
                Logger.Warn($"[{Common.LoggerHead}]->请求Sip推流失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                lock (Common.VideoChannelMediaInfosLock)
                {
                    var obj = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));
                    if (obj != null)
                    {
                        Common.VideoChannelMediaInfos.Remove(obj);
                    }
                }

                return null;
            }

            ReqForWebHookOnPublish onPublishWebhook = (ReqForWebHookOnPublish) taskWait.OtherObj;
            Common.WebHookNeedReturnTask.TryRemove($"WAITONPUBLISH_{videoChannel.MainId}",
                out WebHookNeedReturnTask task);
            if (task != null)
            {
                task.Dispose();
            }


            var videoChannelMediaInfo = new VideoChannelMediaInfo();
            videoChannelMediaInfo.App = videoChannel.App;
            videoChannelMediaInfo.Enabled = videoChannel.Enabled;
            videoChannelMediaInfo.Id = videoChannel.Id;
            videoChannelMediaInfo.Vhost = videoChannel.Vhost;
            videoChannelMediaInfo.AutoRecord = videoChannel.AutoRecord;
            videoChannelMediaInfo.AutoVideo = videoChannel.AutoVideo;
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
            videoChannelMediaInfo.MediaServerStreamInfo.StreamPort = (ushort) onPublishWebhook.Port;
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
            if (mediaServer.UseSsl)
            {
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.flv{exInfo}");
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
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.flv{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onPublishWebhook.App}/{onPublishWebhook.Stream}.flv{exInfo}");
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


            lock (Common.VideoChannelMediaInfosLock)
            {
                var obj = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(videoChannel.MainId));
                if (obj != null)
                {
                    Common.VideoChannelMediaInfos.Remove(obj);
                }

                Common.VideoChannelMediaInfos.Add(videoChannelMediaInfo);
            }

            Logger.Info(
                $"[{Common.LoggerHead}]->请求Sip推流成功->{deviceId}-{channelId}->{JsonHelper.ToJson(videoChannelMediaInfo.MediaServerStreamInfo)}");

            return videoChannelMediaInfo.MediaServerStreamInfo;
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
                Logger.Warn($"[{Common.LoggerHead}]->检查Sip推流状态失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

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
                Logger.Warn($"[{Common.LoggerHead}]->检查Sip推流状态失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

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
                Logger.Warn($"[{Common.LoggerHead}]->检查Sip推流状态失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            Logger.Info(
                $"[{Common.LoggerHead}]->检查Sip推流状态成功->{deviceId}-{channelId}->{JsonHelper.ToJson(tmpSipChannel.PushStatus)}");

            var obj = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(tmpSipChannel.Stream));
            if (obj != null && obj.MediaServerStreamInfo != null && tmpSipChannel.PushStatus == PushStatus.PUSHON)
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
                Logger.Warn(
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
                Logger.Warn(
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
                    Logger.Warn(
                        $"[{Common.LoggerHead}]->PTZ控制失败->{ptzCmd.DeviceId}-{ptzCmd.ChannelId}-{JsonHelper.ToJson(ptzCmd)}->{JsonHelper.ToJson(rs)}");

                    return false;
                }
            }

            PtzCtrl ptzCtrl = new PtzCtrl();
            ptzCtrl.Speed = ptzCmd.Speed;
            ptzCtrl.SipChannel = tmpSipChannel == null ? null : tmpSipChannel;
            ptzCtrl.SipDevice = tmpSipDevice;
            ptzCtrl.PtzCommandType = ptzCmd.PtzCommandType;
            SipMethodProxy sipMethodProxy = new SipMethodProxy(20000);
            var ptz = sipMethodProxy.PtzMove(ptzCtrl, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || ptz == false)
            {
                Logger.Warn(
                    $"[{Common.LoggerHead}]->PTZ控制失败->{ptzCmd.DeviceId}-{ptzCmd.ChannelId}{JsonHelper.ToJson(ptzCmd)}->{JsonHelper.ToJson(rs)}");

                return false;
            }

            Logger.Info(
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
                Logger.Warn($"[{Common.LoggerHead}]->获取Sip通道失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

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
                Logger.Warn($"[{Common.LoggerHead}]->获取Sip通道失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

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
                Logger.Warn($"[{Common.LoggerHead}]->获取Sip通道失败->{deviceId}-{channelId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            Logger.Debug(
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
                Logger.Warn($"[{Common.LoggerHead}]->获取Sip设备失败->{deviceId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            var ret = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId.Equals(deviceId));
            if (ret == null)
            {
                Logger.Warn($"[{Common.LoggerHead}]->获取Sip设备失败->{deviceId}->结果为空");
            }
            else
            {
                Logger.Debug($"[{Common.LoggerHead}]->获取Sip设备成功->{deviceId}->{JsonHelper.ToJson(ret)}");
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
            Logger.Info(
                $"[{Common.LoggerHead}]->获取Sip设备列表成功->{JsonHelper.ToJson(LibGB28181SipServer.Common.SipDevices.Count)}");
            return LibGB28181SipServer.Common.SipDevices;
        }
    }
}