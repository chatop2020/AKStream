using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using LibLogger;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebHookRequest;
using LibZLMediaKitMediaServer.Structs.WebHookResponse;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;

namespace AKStreamWeb.Services
{
    public static class WebHookService
    {
        /// <summary>
        /// 判断是否为回放流,如果找到返回obj信息
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static bool IsRecordStream(string stream, out VideoChannelRecordInfo outobj)
        {
            foreach (var obj in GCommon.VideoChannelRecordInfo)
            {
                if (obj != null && obj.RecItems != null && obj.RecItems.Count > 0)
                {
                    var o = obj.RecItems.FindLast(x => x.Stream.Trim().ToLower().Equals(stream.Trim().ToLower()));
                    if (o != null)
                    {
                        outobj = obj;
                        return true;
                    }
                }
            }

            outobj = null;
            return false;
        }


        public static ResToWebHookOnRecordMP4 OnRecordMp4(ReqForWebHookOnRecordMP4 req)
        {
             GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnRecordMp4回调->{JsonHelper.ToJson(req)}");

            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnRecordMP4()
                {
                    Code = -1,
                    Msg = "没找到相应的流媒体服务器",
                };
            }

            var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream)).First();

            if (videoChannel == null)
            {
                return new ResToWebHookOnRecordMP4()
                {
                    Code = 0,
                    Msg = "success"
                };
            }

            var st = UtilsHelper.ConvertDateTimeToInt((long) req.Start_Time);
            DateTime currentTime = DateTime.Now;
            RecordFile tmpDvrVideo = new RecordFile();
            tmpDvrVideo.App = req.App;
            tmpDvrVideo.Vhost = req.Vhost;
            tmpDvrVideo.Streamid = req.Stream;
            tmpDvrVideo.FileSize = req.File_Size;
            tmpDvrVideo.DownloadUrl = req.Url;
            tmpDvrVideo.VideoPath = req.File_Path;
            tmpDvrVideo.StartTime = st;
            tmpDvrVideo.EndTime = st.AddSeconds((int) req.Time_Len);
            tmpDvrVideo.Duration = req.Time_Len;
            tmpDvrVideo.Undo = false;
            tmpDvrVideo.Deleted = false;
            tmpDvrVideo.MediaServerId = req.MediaServerId;
            tmpDvrVideo.UpdateTime = currentTime;
            tmpDvrVideo.RecordDate = st.ToString("yyyy-MM-dd");
            tmpDvrVideo.MainId = videoChannel.MainId;
            tmpDvrVideo.MediaServerIp = mediaServer.IpV4Address;
            tmpDvrVideo.ChannelName = videoChannel.ChannelName;
            tmpDvrVideo.DepartmentId = videoChannel.DepartmentId;
            tmpDvrVideo.DepartmentName = videoChannel.DepartmentName;
            tmpDvrVideo.PDepartmentId = videoChannel.PDepartmentId;
            tmpDvrVideo.PDepartmentName = videoChannel.PDepartmentName;
            tmpDvrVideo.DeviceId = videoChannel.DeviceId;
            tmpDvrVideo.ChannelId = videoChannel.ChannelId;
            tmpDvrVideo.VideoSrcUrl = videoChannel.VideoSrcUrl;
            tmpDvrVideo.CreateTime = DateTime.Now;
            string tmp = tmpDvrVideo.DownloadUrl;
            tmp = tmp.Replace("\\", "/", StringComparison.Ordinal); //跨平台兼容
            if (tmp.Contains(":"))
            {
                tmp = tmp.Substring(tmp.IndexOf(':') + 1); //清除掉类似  c: 这样的字符，跨平台兼容
            }

            bool found = false;
            foreach (var recordPath in mediaServer.RecordPathList)
            {
                if (!string.IsNullOrEmpty(recordPath.Value) && req.File_Path.Contains(recordPath.Value))
                {
                    tmp = tmp.Replace(recordPath.Value, "", StringComparison.Ordinal);
                    if (tmp.StartsWith("/"))
                    {
                        tmp = tmp.TrimStart('/');
                    }

                    string str = recordPath.Value.EndsWith('/') == true ? recordPath.Value : recordPath.Value + "/";
                    str = str.StartsWith('/') == true ? str : "/" + str;
                    tmpDvrVideo.DownloadUrl = "http://" + mediaServer.IpV4Address + ":" + mediaServer.KeeperPort +
                                              str.Trim() + tmp;
                    found = true;
                }
            }

            if (!found)
            {
                //如果不包含自定义视频存储目录地址，就认为是默认地址
                if (!string.IsNullOrEmpty(tmp) && tmp.Contains("/record/"))
                {
                    tmp = tmp.Replace("/record/", "", StringComparison.Ordinal);
                }

                tmpDvrVideo.DownloadUrl = "http://" + mediaServer.IpV4Address + ":" + mediaServer.HttpPort +
                                          "/" + tmp;
            }

            try
            {
                var dbRet = ORMHelper.Db.Insert(tmpDvrVideo).ExecuteAffrows();
            }
            catch (Exception ex)
            {
                ResponseStruct rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                 GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->将Mp4录制文件写入数据库时异常->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs)}");
            }

            var retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
            if (retobj != null && retobj.MediaServerStreamInfo != null)
            {
                retobj.MediaServerStreamInfo.IsRecorded = true;
                GCommon.Ldb.VideoOnlineInfo.Update(retobj);
            }


            return new ResToWebHookOnRecordMP4()
            {
                Code = 0,
                Msg = "success"
            };
        }

        /// <summary>
        /// 流被结束时的回调
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static ResToWebHookOnFlowReport OnFlowReport(ReqForWebHookOnFlowReport req)
        {
             GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnFlowReport回调->{JsonHelper.ToJson(req)}");
            if (req.Player == true)
            {
                var retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                    x.MainId.Equals(req.Stream) && x.MediaServerId.Equals(req.MediaServerId));
                if (retobj != null && retobj.MediaServerStreamInfo != null &&
                    retobj.MediaServerStreamInfo.PlayerList != null &&
                    retobj.MediaServerStreamInfo.PlayerList.Count > 0)
                {
                    retobj.MediaServerStreamInfo.PlayerList.Remove(
                        retobj.MediaServerStreamInfo.PlayerList.FindLast(x => x.PlayerId.Equals(req.Id)));
                    GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                }
            }
            else if (req.Player == false)
            {
                if (!IsRecordStream(req.Stream, out _))
                {
                    var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                        .Where(x => x.MediaServerId.Equals(req.MediaServerId)).First();
                    if (videoChannel != null && videoChannel.DeviceStreamType == DeviceStreamType.GB28181)
                    {
                        var sipDevice =
                            LibGB28181SipServer.Common.SipDevices.FindLast(
                                x => x.DeviceId.Equals(videoChannel.DeviceId));
                        if (sipDevice != null && sipDevice.SipChannels != null && sipDevice.SipChannels.Count > 0)
                        {
                            var channel =
                                sipDevice.SipChannels.FindLast(x => x.DeviceId.Equals(videoChannel.ChannelId));
                            if (channel != null)
                            {
                                channel.PushStatus = PushStatus.IDLE;
                            }
                        }

                        try
                        {
                            //gb28181异常断流时，要释放掉原来申请的rtp端口
                            var mediaServer =
                                Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
                            if (mediaServer != null && mediaServer.IsKeeperRunning && mediaServer.IsMediaServerRunning)
                            {
                                var rtpPort = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                                        x.MediaServerId.Equals(videoChannel.MediaServerId) &&
                                        x.MainId.Equals(videoChannel.MainId))
                                    .MediaServerStreamInfo!.RptPort;
                                if (rtpPort != null && rtpPort > 0)
                                {
                                    ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort =
                                        new ReqZLMediaKitCloseRtpPort()
                                        {
                                            Stream_Id = videoChannel.MainId,
                                        };
                                    mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                                    mediaServer.KeeperWebApi.ReleaseRtpPort(
                                        (ushort) rtpPort,
                                        out _); //释放rtp端口
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                        x.MediaServerId.Equals(videoChannel.MediaServerId) && x.MainId.Equals(videoChannel.MainId));
                }
                else
                {
                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                        x.MainId.Equals(req.Stream));
                }
            }


            return new ResToWebHookOnFlowReport()
            {
                Code = 0,
                Msg = "success",
            };
        }

        /// <summary>
        /// 处理自动断流
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResToWebHookOnStreamNoneReader OnStreamNoneReader(ReqForWebHookOnStreamNoneReader req)
        {
            VideoChannelRecordInfo outobj = null;
            var b = IsRecordStream(req.Stream, out outobj);
             GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnStreamNoneReader回调->{JsonHelper.ToJson(req)}");
            if (b == false)
            {
                var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                    .Where(x => x.MediaServerId.Equals(req.MediaServerId)).First();
                if (videoChannel.AutoVideo == false && videoChannel.NoPlayerBreak == true) //或者要求没有人观看时自动断流的，就断流
                {
                    var ret = MediaServerService.StreamStop(videoChannel.MediaServerId, videoChannel.MainId,
                        out ResponseStruct rs);
                    if (!rs.Code.Equals(ErrorNumber.None) || ret == false)
                    {
                         GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->无人观看时断流失败->{videoChannel.MainId}->{JsonHelper.ToJson(rs)}");
                    }
                    else
                    {
                         GCommon.Logger.Info($"[{Common.LoggerHead}]->无人观看时断流成功->{videoChannel.MainId}");
                    }
                }
            }
            else
            {
                if (outobj != null)
                {
                    var record =
                        outobj.RecItems.FindLast(x => x.Stream.Trim().ToLower().Equals(req.Stream.Trim().ToLower()));
                    if (record != null)
                    {
                        var ret = SipServerService.StopLiveVideo(outobj.TaskId, record.SsrcId, out ResponseStruct rs);
                        if (rs.Code == ErrorNumber.None)
                        {
                            if (ret)
                            {
                                 GCommon.Logger.Info($"[{Common.LoggerHead}]->无人观看回调发生时断开回放流成功");
                            }
                            else
                            {
                                 GCommon.Logger.Warn($"[{Common.LoggerHead}]->无人观看回调发生时断开回放流失败");
                            }
                        }
                        else
                        {
                             GCommon.Logger.Error($"[{Common.LoggerHead}]->无人观看回调发生时断开回放流时出现异常->{JsonHelper.ToJson(rs)}");
                        }
                    }
                }
            }

            return new ResToWebHookOnStreamNoneReader()
            {
                Code = 0,
                Close = false,
            };
        }

        /// <summary>
        /// 当有流状态变化时
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResToWebHookOnStreamChange OnStreamChanged(ReqForWebHookOnStreamChange req)
        {
            if (req.Schema.Trim().ToLower().Equals("rtmp") && !IsRecordStream(req.Stream, out _))
            {
                ServerInstance mediaServer = null;
                if (req.Regist == true)
                {
                     GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnStreamChanged回调(流接入)->{JsonHelper.ToJson(req)}");
                    mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
                    if (mediaServer == null)
                    {
                        return new ResToWebHookOnStreamChange()
                        {
                            Code = 0,
                            Msg = "success",
                        };
                    }

                    var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                        .First();
                    if (videoChannel == null)
                    {
                        return new ResToWebHookOnStreamChange()
                        {
                            Code = 0,
                            Msg = "success",
                        };
                    }

                    if (videoChannel.Enabled == false || videoChannel.MediaServerId.Contains("unknown_server"))
                    {
                        return new ResToWebHookOnStreamChange()
                        {
                            Code = 0,
                            Msg = "success",
                        };
                    }

                    if (videoChannel.DeviceStreamType != DeviceStreamType.GB28181)
                    {
                        var taskStr = $"WAITONSTREAMCHANGE_{req.Stream}".Trim();
                        WebHookNeedReturnTask webHookNeedReturnTask;

                        int tick = 0;

                        while (Common.WebHookNeedReturnTask.TryGetValue(taskStr, out webHookNeedReturnTask) == false &&
                               tick <= Common.AkStreamWebConfig.WaitEventTimeOutMSec)
                        {
                            //AutoResetEvent没准备好，OnStreamChanged事件却来了，这里如果发现值为空，就等等
                            tick += 10;
                            Thread.Sleep(10);
                        }

                        var taskFound = Common.WebHookNeedReturnTask.TryGetValue(taskStr, out webHookNeedReturnTask);
                        if (taskFound && webHookNeedReturnTask != null)
                        {
                            webHookNeedReturnTask.OtherObj = req;
                            try
                            {
                                webHookNeedReturnTask.AutoResetEvent.Set(); //让推流业务继续走下去
                            }
                            catch (Exception ex)
                            {
                                ResponseStruct exrs = new ResponseStruct()
                                {
                                    Code = ErrorNumber.Sys_AutoResetEventExcept,
                                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                                    ExceptMessage = ex.Message,
                                    ExceptStackTrace = ex.StackTrace
                                };
                                 GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                            }
                        }
                        else
                        {
                             GCommon.Logger.Error(
                                $"[{Common.LoggerHead}]->WebHookNeedReturnTask异常->没有找到{videoChannel.MainId}的推拉流信息，任务异常");
                        }
                    }
                }
                else
                {
                     GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnStreamChanged回调(流移除)->{JsonHelper.ToJson(req)}");
                    var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                        .First();
                    if (videoChannel.DeviceStreamType == DeviceStreamType.GB28181)
                    {
                        var sipDevice =
                            LibGB28181SipServer.Common.SipDevices.FindLast(
                                x => x.DeviceId.Equals(videoChannel.DeviceId));

                        if (sipDevice != null && sipDevice.SipChannels != null && sipDevice.SipChannels.Count > 0)
                        {
                            var channel =
                                sipDevice.SipChannels.FindLast(x => x.DeviceId.Equals(videoChannel.ChannelId));
                            if (channel != null)
                            {
                                channel.PushStatus = PushStatus.IDLE;
                            }
                        }

                        try
                        {
                            //gb28181异常断流时，要释放掉原来申请的rtp端口
                            var rtpPort = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                                    x.MediaServerId.Equals(videoChannel.MediaServerId) &&
                                    x.MainId.Equals(videoChannel.MainId))
                                .MediaServerStreamInfo!.RptPort;
                            if (rtpPort != null && rtpPort > 0)
                            {
                                ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                                {
                                    Stream_Id = videoChannel.MainId,
                                };
                                mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                                mediaServer.KeeperWebApi.ReleaseRtpPort(
                                    (ushort) rtpPort,
                                    out _); //释放rtp端口
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (videoChannel.DeviceStreamType != DeviceStreamType.GB28181)
                    {
                        MediaServerService.StreamStop(videoChannel.MediaServerId, videoChannel.MainId, out _);
                    }


                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                        x.MediaServerId.Equals(videoChannel.MediaServerId) && x.MainId.Equals(videoChannel.MainId));
                }
            }
            else if (req.Schema.Trim().ToLower().Equals("rtmp") && IsRecordStream(req.Stream, out _))
            {
                if (req.Regist == false)
                {
                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                        x.MediaServerId.Equals(req.MediaServerId) && x.MainId.Equals(req.Stream));
                }
            }

            return new ResToWebHookOnStreamChange()
            {
                Code = 0,
                Msg = "success",
            };
        }

        /// <summary>
        /// 有播放者访问时
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResToWebHookOnPlay OnPlay(ReqForWebHookOnPlay req)
        {
             GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnPlay回调->{JsonHelper.ToJson(req)}");

            if (!IsRecordStream(req.Stream, out _))
            {
                var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                    .Where(x => x.MediaServerId.Equals(req.MediaServerId)).First();

                if (videoChannel == null)
                {
                    return new ResToWebHookOnPlay()
                    {
                        Code = -1,
                        Msg = "feild",
                    };
                }


                var retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                    x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                if (retobj != null && retobj.MediaServerStreamInfo != null)
                {
                    if (retobj.MediaServerStreamInfo.PlayerList == null)
                    {
                        retobj.MediaServerStreamInfo.PlayerList = new List<MediaServerStreamPlayerInfo>();
                    }

                    retobj.MediaServerStreamInfo.PlayerList.Add(new MediaServerStreamPlayerInfo()
                    {
                        IpAddress = req.Ip,
                        PlayerId = req.Id,
                        Params = req.Params,
                        Port = (ushort) req.Port,
                        StartTime = DateTime.Now,
                    });
                    GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                }
            }
            else
            {
                var retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                    x.MainId.Equals(req.Stream));
                if (retobj != null && retobj.MediaServerStreamInfo != null)
                {
                    if (retobj.MediaServerStreamInfo.PlayerList == null)
                    {
                        retobj.MediaServerStreamInfo.PlayerList = new List<MediaServerStreamPlayerInfo>();
                    }

                    retobj.MediaServerStreamInfo.PlayerList.Add(new MediaServerStreamPlayerInfo()
                    {
                        IpAddress = req.Ip,
                        PlayerId = req.Id,
                        Params = req.Params,
                        Port = (ushort) req.Port,
                        StartTime = DateTime.Now,
                    });
                    GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                }
            }


            return new ResToWebHookOnPlay()
            {
                Code = 0,
                Msg = "success",
            };
        }

        /// <summary>
        /// 有rtp流发布时
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResToWebHookOnPublish OnPublish(ReqForWebHookOnPublish req)
        {
             GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnPublish回调->{JsonHelper.ToJson(req)}");

            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnPublish()
                {
                    Code = -1,
                    EnableHls = false,
                    EnableMP4 = false,
                    Msg = "failed",
                };
            }

            VideoChannel videoChannel = null;

            if (!IsRecordStream(req.Stream, out _))
            {
                videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                    .First();
                if (videoChannel == null)
                {
                    return new ResToWebHookOnPublish()
                    {
                        Code = -1,
                        EnableHls = false,
                        EnableMP4 = false,
                        Msg = "failed",
                    };
                }

                if (videoChannel.Enabled == false || videoChannel.MediaServerId.Contains("unknown_server"))
                {
                    return new ResToWebHookOnPublish()
                    {
                        Code = -1,
                        EnableHls = false,
                        EnableMP4 = false,
                        Msg = "failed",
                    };
                }
            }


            if ((videoChannel != null && videoChannel.DeviceStreamType == DeviceStreamType.GB28181) ||
                (IsRecordStream(req.Stream, out _)))
            {
                var taskStr = $"WAITONPUBLISH_{req.Stream}";
                WebHookNeedReturnTask webHookNeedReturnTask;
                int tick = 0;

                while (Common.WebHookNeedReturnTask.TryGetValue(taskStr, out webHookNeedReturnTask) == false &&
                       tick <= Common.AkStreamWebConfig.WaitEventTimeOutMSec)
                {
                    //AutoResetEvent没准备好，onpublish事件却来了，这里如果发现值为空，就等等
                    tick += 10;
                    Thread.Sleep(10);
                }

                var taskFound = Common.WebHookNeedReturnTask.TryGetValue(taskStr, out webHookNeedReturnTask);
                if (taskFound && webHookNeedReturnTask != null)
                {
                    webHookNeedReturnTask.OtherObj = req;
                    try
                    {
                        webHookNeedReturnTask.AutoResetEvent.Set(); //让推流业务继续走下去
                    }
                    catch (Exception ex)
                    {
                        ResponseStruct exrs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AutoResetEventExcept,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                            ExceptMessage = ex.Message,
                            ExceptStackTrace = ex.StackTrace
                        };
                         GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                    }
                }


                ResToWebHookOnPublish result = new ResToWebHookOnPublish();
                result.Code = 0;
                result.EnableHls = true;
                result.Msg = "success";
                result.EnableMP4 = false;
                return result;
            }

            return new ResToWebHookOnPublish()
            {
                Code = -1,
                EnableHls = false,
                EnableMP4 = false,
                Msg = "failed",
            };
        }

        /// <summary>
        /// 保持与流媒体服务器的心跳
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResMediaServerKeepAlive MediaServerKeepAlive(ReqMediaServerKeepAlive req, out ResponseStruct rs)
        {
             GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->收到WebHook-MediaServerKeepAlive回调->{JsonHelper.ToJson(req.MediaServerId)}");

            ResMediaServerKeepAlive result;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (req == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
            }

            if (Math.Abs((DateTime.Now - req.ServerDateTime).TotalSeconds) > 60) //两边服务器时间大于60秒，则回复注册失败
            {
                result = new ResMediaServerKeepAlive()
                {
                    Rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_TimeExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_TimeExcept],
                    },
                    RecommendTimeSynchronization = true,
                    ServerDateTime = DateTime.Now,
                };
                return result;
            }

            lock (Common.MediaServerLockObj)
            {
                var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
                if (mediaServer != null)
                {
                    if (req.FirstPost)
                    {
                        //已经存在的mediaserver被要求重启前要停掉此流媒体所有流信息

                        var removeList = GCommon.Ldb.VideoOnlineInfo
                            .Find(x => x.MediaServerId.Equals(req.MediaServerId))
                            .ToList();

                        if (removeList != null && removeList.Count > 0)
                        {
                            foreach (var obj in removeList)
                            {
                                if (obj != null)
                                {
                                    MediaServerService.StreamStop(obj.MediaServerId, obj.MainId, out _);
                                }
                            }
                        }

                        result = new ResMediaServerKeepAlive()
                        {
                            Rs = rs,
                            RecommendTimeSynchronization = false,
                            ServerDateTime = DateTime.Now,
                            NeedRestartMediaServer = true,
                        };
                        mediaServer.Dispose();
                        Common.MediaServerList.Remove(mediaServer);
                         GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->清理MediaServerList中的的流媒体服务器实例,要求重启流媒体服务器->当前流媒体服务器数量:{Common.MediaServerList.Count}");
                        return result;
                    }

                    //已经存在的
                    if ((DateTime.Now - mediaServer.KeepAliveTime).TotalSeconds < 5) //5秒内多次心跳请求直接回复
                    {
                        mediaServer.KeepAliveTime = DateTime.Now;
                        result = new ResMediaServerKeepAlive()
                        {
                            Rs = rs,
                            RecommendTimeSynchronization = false,
                            ServerDateTime = DateTime.Now,
                        };
                        return result;
                    }

                    mediaServer.Secret = req.Secret;
                    mediaServer.IpV4Address = req.IpV4Address;
                    mediaServer.IpV6Address = req.IpV6Address;
                    mediaServer.IsKeeperRunning = true;
                    mediaServer.IsMediaServerRunning = req.MediaServerIsRunning;
                    mediaServer.KeeperPort = req.KeeperWebApiPort;
                    mediaServer.RecordPathList = req.RecordPathList;
                    mediaServer.ZlmediakitPid = req.MediaServerPid;
                    mediaServer.KeepAliveTime = DateTime.Now;
                    mediaServer.MediaServerId = req.MediaServerId;
                    mediaServer.HttpPort = req.ZlmHttpPort;
                    mediaServer.HttpsPort = req.ZlmHttpsPort;
                    mediaServer.RtmpPort = req.ZlmRtmpPort;
                    mediaServer.RtmpsPort = req.ZlmRtmpsPort;
                    mediaServer.RtspPort = req.ZlmRtspPort;
                    mediaServer.RtspsPort = req.ZlmRtspsPort;
                    mediaServer.RtpPortMax = req.RtpPortMax;
                    mediaServer.RtpPortMin = req.RtpPortMin;
                    mediaServer.RandomPort = req.RandomPort;
                    mediaServer.ServerDateTime = req.ServerDateTime;
                    mediaServer.ZlmRecordFileSec = req.ZlmRecordFileSec;
                    mediaServer.AccessKey = req.AccessKey;
                    if (req.PerformanceInfo != null) //更新性能信息
                    {
                        mediaServer.PerformanceInfo = req.PerformanceInfo;
                    }

                    result = new ResMediaServerKeepAlive()
                    {
                        Rs = rs,
                        RecommendTimeSynchronization = false,
                        ServerDateTime = DateTime.Now,
                        NeedRestartMediaServer = false,
                    };
                }
                else
                {
                    //没有存在的
                    var tmpMediaServer = new ServerInstance();
                    tmpMediaServer.Secret = req.Secret;
                    tmpMediaServer.IpV4Address = req.IpV4Address;
                    tmpMediaServer.IpV6Address = req.IpV6Address;
                    tmpMediaServer.IsKeeperRunning = true;
                    tmpMediaServer.IsMediaServerRunning = req.MediaServerIsRunning;
                    tmpMediaServer.KeeperPort = req.KeeperWebApiPort;
                    tmpMediaServer.RecordPathList = req.RecordPathList;
                    tmpMediaServer.ZlmediakitPid = req.MediaServerPid;
                    tmpMediaServer.KeepAliveTime = DateTime.Now;
                    tmpMediaServer.MediaServerId = req.MediaServerId;
                    tmpMediaServer.HttpPort = req.ZlmHttpPort;
                    tmpMediaServer.HttpsPort = req.ZlmHttpsPort;
                    tmpMediaServer.RtmpPort = req.ZlmRtmpPort;
                    tmpMediaServer.RtmpsPort = req.ZlmRtmpsPort;
                    tmpMediaServer.RtspPort = req.ZlmRtspPort;
                    tmpMediaServer.RtspsPort = req.ZlmRtspsPort;
                    tmpMediaServer.RtpPortMax = req.RtpPortMax;
                    tmpMediaServer.RtpPortMin = req.RtpPortMin;
                    tmpMediaServer.RandomPort = req.RandomPort;
                    tmpMediaServer.ServerDateTime = req.ServerDateTime;
                    tmpMediaServer.ZlmRecordFileSec = req.ZlmRecordFileSec;
                    tmpMediaServer.AccessKey = req.AccessKey;


                    if (req.PerformanceInfo != null) //更新性能信息
                    {
                        tmpMediaServer.PerformanceInfo = req.PerformanceInfo;
                    }

                    tmpMediaServer.WebApiHelper = new WebApiHelper(tmpMediaServer.IpV4Address,
                        tmpMediaServer.UseSsl ? tmpMediaServer.HttpsPort : tmpMediaServer.HttpPort,
                        tmpMediaServer.Secret, Common.AkStreamWebConfig.HttpClientTimeoutSec, "",
                        tmpMediaServer.UseSsl);
                    tmpMediaServer.KeeperWebApi = new KeeperWebApi(tmpMediaServer.IpV4Address,
                        tmpMediaServer.KeeperPort, tmpMediaServer.AccessKey,
                        Common.AkStreamWebConfig.HttpClientTimeoutSec);
                    Common.MediaServerList.Add(tmpMediaServer);
                    result = new ResMediaServerKeepAlive()
                    {
                        Rs = rs,
                        RecommendTimeSynchronization = false,
                        ServerDateTime = DateTime.Now,
                    };
                    if (Common.AkStreamWebConfig.MediaServerFirstToRestart)
                    {
                        result.NeedRestartMediaServer = true;
                    }
                }
            }

            return result;
        }
    }
}