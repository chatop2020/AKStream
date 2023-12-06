using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebHookRequest;
using LibZLMediaKitMediaServer.Structs.WebHookResponse;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace AKStreamWeb.Services
{
    public static class WebHookService
    {

        private static void ForwardPush(string postMsg, string url)
        {
            NetHelper.HttpPostRequest(url, null, JsonHelper.ToJson(postMsg));
        }
        /*
        private  delegate void ForwardPushInfo(ReqForWebHookOnStreamChange msg,string url);
        private  delegate void ForwardDestoryInfo(ReqForWebHookOnFlowReport msg,string url);

        private delegate void ForwardOnRecordInfo(ReqForWebHookOnRecordMP4 msg, string url);
        */

        /*private static void ForwardPush(ReqForWebHookOnStreamChange msg, string url)
        {
            NetHelper.HttpPostRequest(url, null, JsonHelper.ToJson(msg));
        }
        private static void ForwardDestory(ReqForWebHookOnFlowReport msg, string url)
        {
            NetHelper.HttpPostRequest(url, null, JsonHelper.ToJson(msg));
        }
        
        private static void ForwardRecord(ReqForWebHookOnRecordMP4 msg, string url)
        {
            NetHelper.HttpPostRequest(url, null, JsonHelper.ToJson(msg));
        }*/

        
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


        /// <summary>
        /// 当需要rtsp鉴权时，返回该rtsp鉴权的专用盐（盐就是项目名称）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static ResToWebHookOnRtspRealm OnRtspRealm(ReqForWebHookOnRtspRealm req)
        {
            GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnRtspRealm回调->{JsonHelper.ToJson(req)}");
            if (req != null && !string.IsNullOrEmpty(req.MediaServerId))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<UserAuth>()
                        .Where(x => x.MediaServerId.Equals(req.MediaServerId.Trim()))
                        .ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->OnRtspRealm->执行SQL:->{sql}");
                }

                #endregion

                var ret = ORMHelper.Db.Select<UserAuth>()
                    .Where(x => x.MediaServerId.Equals(req.MediaServerId.Trim())).First();
                if (ret != null)
                {
                    return new ResToWebHookOnRtspRealm()
                    {
                        Code = 0,
                        Realm = "default"
                    };
                }
            }

            return new ResToWebHookOnRtspRealm()
            {
                Code = -1,
                Realm = "error"
            };
        }

        /// <summary>
        /// rtsp鉴权事件处理
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static ResToWebHookOnRtspAuth OnRtspAuth(ReqForWebHookOnRtspAuth req)
        {
            GCommon.Logger.Info($"[{Common.LoggerHead}]->收到WebHook-OnRtspAuth回调->{JsonHelper.ToJson(req)}");
            if (req != null && !string.IsNullOrEmpty(req.MediaServerId))
            {
                var username = req.User_Name;
                var realm = req.Realm;
                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<UserAuth>()
                        .Where(x => x.MediaServerId.Equals(req.MediaServerId.Trim()))
                        .Where(x => x.Username.Equals(username.Trim()))
                        .ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->OnRtspRealm->执行SQL:->{sql}");
                }

                var ret = ORMHelper.Db.Select<UserAuth>()
                    .Where(x => x.MediaServerId.Equals(req.MediaServerId.Trim()))
                    .Where(x => x.Username.Equals(username.Trim()))
                    .First();
                if (ret != null && !string.IsNullOrEmpty(ret.Password))
                {
                    return new ResToWebHookOnRtspAuth()
                    {
                        Code = 0,
                        Encrypted = true,
                        Passwd = ret.Password,
                        Msg = "success"
                    };
                }
            }

            return new ResToWebHookOnRtspAuth()
            {
                Code = -1,
                Msg = "failed"
            };
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

            #region debug sql output

            if (Common.IsDebug)
            {
                var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream)).ToSql();

                GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->OnRecordMp4->执行SQL:->{sql}");
            }

            #endregion

            var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream)).First();

            if (videoChannel == null)
            {
                if ((!string.IsNullOrEmpty(Common.AkStreamWebConfig.ForwardUrlOut)) &&
                    (UtilsHelper.IsUrl(Common.AkStreamWebConfig.ForwardUrlOut)) &&
                    (Common.AkStreamWebConfig.ForwardUnmanagedRtmpRtspRtcStream) )
                {
                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->转发录制信息->{Common.AkStreamWebConfig.ForwardUrlOnRecord}->{JsonHelper.ToJson(req)}");


                    var postMsg = JsonHelper.ToJson(req);
                    Task.Run(() => { ForwardPush(postMsg,Common.AkStreamWebConfig.ForwardUrlOnRecord); }); //抛线程出去处理

                }
                
                return new ResToWebHookOnRecordMP4()
                {
                    Code = 0,
                    Msg = "success"
                };
            }

            #region debug sql output

            if (Common.IsDebug)
            {
                var sql = ORMHelper.Db.Select<RecordFile>().Where(x => x.Streamid.Equals(req.Stream) &&
                                                                       x.VideoPath.Equals(req.File_Path) &&
                                                                       x.FileSize.Equals(req.File_Size) &&
                                                                       x.Vhost.Equals(req.Vhost) &&
                                                                       x.Deleted.Equals(false) &&
                                                                       x.App.Equals(req.App) &&
                                                                       x.MediaServerId.Equals(req.MediaServerId))
                    .ToSql();

                GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->OnRecordMp4->执行SQL:->{sql}");
            }

            #endregion

            bool diskuseage = true;
            if (mediaServer.DisksUseable != null && mediaServer.DisksUseable.Count > 0)
            {
                foreach (var disk in mediaServer.DisksUseable)
                {
                    if (disk.Value != 0)
                    {
                        diskuseage = false;
                        GCommon.Logger.Error(
                            $"[{Common.LoggerHead}]->磁盘挂载异常->{disk.Key}->异常代码：{disk.Value}->{JsonHelper.ToJson(req)}");
                        break;
                    }
                }
            }

            if (!diskuseage)
            {
                ResponseStruct rs = null;
                try
                {
                    try
                    {
                        AKStreamKeeperService.DeleteFileWithoutCheckDiskUseage(mediaServer.MediaServerId, req.File_Path,
                            out _);
                    }
                    catch
                    {
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_DiskExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_DiskExcept],
                        ExceptMessage = "录制文件异常，磁盘挂载异常",
                        ExceptStackTrace =
                            $"磁盘挂载异常不能实现文件录制功能->{JsonHelper.ToJson(req)}",
                    };
                    return new ResToWebHookOnRecordMP4()
                    {
                        Code = 0,
                        Msg = "success"
                    };
                }

                finally
                {
                    throw new AkStreamException(rs);
                }
            }

            var recordInfo = ORMHelper.Db.Select<RecordFile>().Where(x => x.Streamid.Equals(req.Stream) &&
                                                                          x.VideoPath.Equals(req.File_Path) &&
                                                                          x.FileSize.Equals(req.File_Size) &&
                                                                          x.Vhost.Equals(req.Vhost) &&
                                                                          x.Deleted.Equals(false) &&
                                                                          x.App.Equals(req.App) &&
                                                                          x.MediaServerId.Equals(req.MediaServerId))
                .First();
            if (recordInfo != null) //重复传了
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->ZLMediaKit_OnRecordMp4回调重复传送，被忽略->{JsonHelper.ToJson(req)}");

                return new ResToWebHookOnRecordMP4()
                {
                    Code = 0,
                    Msg = "success"
                };
            }

            var st = UtilsHelper.ConvertDateTimeToInt((long)req.Start_Time);
            DateTime currentTime = DateTime.Now;
            RecordFile tmpDvrVideo = new RecordFile();
            tmpDvrVideo.App = req.App;
            tmpDvrVideo.Vhost = req.Vhost;
            tmpDvrVideo.Streamid = req.Stream;
            tmpDvrVideo.FileSize = req.File_Size;
            tmpDvrVideo.DownloadUrl = req.Url;
            tmpDvrVideo.VideoPath = req.File_Path;
            tmpDvrVideo.StartTime = st;
            decimal _len = (decimal)req.Time_Len;
            int _intLen = (int)Math.Ceiling(_len); //四舍五入后取整
            tmpDvrVideo.EndTime = st.AddSeconds(_intLen);
            tmpDvrVideo.Duration = _intLen;


            if (tmpDvrVideo.Duration <= 0 || req.File_Size > 103881427200) //大概720p下60个小时的录制量，单文件超过这个值，就不再保存
            {
                ResponseStruct rs = null;
                try
                {
                    try
                    {
                        AKStreamKeeperService.DeleteFileWithoutCheckDiskUseage(mediaServer.MediaServerId,
                            tmpDvrVideo.VideoPath, out _);
                        // mediaServer.KeeperWebApi.DeleteFile(out _, tmpDvrVideo.VideoPath); //从磁盘中删除这个文件
                    }
                    catch
                    {
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_RecordFileExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_RecordFileExcept],
                        ExceptMessage = "录制文件异常，视频时长为0或者单文件字节数过大",
                        ExceptStackTrace =
                            $"可能因为磁盘不可写，造成视频时长为0，或者单文件字节数超过103881427200相当于720p下单文件录制60个小时->{JsonHelper.ToJson(req)}",
                    };
                    return new ResToWebHookOnRecordMP4()
                    {
                        Code = 0,
                        Msg = "success"
                    };
                }

                finally
                {
                    throw new AkStreamException(rs);
                }
            }

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
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Insert(tmpDvrVideo).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->OnRecordMp4->执行SQL:->{sql}");
                }

                #endregion

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

            VideoChannelMediaInfo retobj = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                    x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
            }

            if (retobj != null && retobj.MediaServerStreamInfo != null)
            {
                if (retobj.MediaServerStreamInfo.StopRecordWithAPI == false) //判断是否因为手工停止录制，如果手工停止录制就不需要再改变录制状态
                {
                    retobj.MediaServerStreamInfo.IsRecorded = true;
                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                    }
                }
                else //如果是手工停止，则把手工停止属性重置到false
                {
                    retobj.MediaServerStreamInfo.StopRecordWithAPI = false;
                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                    }
                }
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
                VideoChannelMediaInfo retobj = null;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                        x.MainId.Equals(req.Stream) && x.MediaServerId.Equals(req.MediaServerId));
                }

                if (retobj != null && retobj.MediaServerStreamInfo != null &&
                    retobj.MediaServerStreamInfo.PlayerList != null &&
                    retobj.MediaServerStreamInfo.PlayerList.Count > 0)
                {
                    retobj.MediaServerStreamInfo.PlayerList.Remove(
                        retobj.MediaServerStreamInfo.PlayerList.FindLast(x => x.PlayerId.Equals(req.Id)));
                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                    }
                }
            }
            else if (req.Player == false)
            {
                if (!IsRecordStream(req.Stream, out _))
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                            .Where(x => x.MediaServerId.Equals(req.MediaServerId)).ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->OnFlowReport->执行SQL:->{sql}");
                    }

                    #endregion

                    var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                        .Where(x => x.MediaServerId.Equals(req.MediaServerId)).First();

                    if (videoChannel == null)
                    {
                        if ((!string.IsNullOrEmpty(Common.AkStreamWebConfig.ForwardUrlOut)) &&
                            (UtilsHelper.IsUrl(Common.AkStreamWebConfig.ForwardUrlOut)) &&
                            (Common.AkStreamWebConfig.ForwardUnmanagedRtmpRtspRtcStream) )
                        {
                            GCommon.Logger.Info(
                                $"[{Common.LoggerHead}]->转发注销流信息->{Common.AkStreamWebConfig.ForwardUrlOut}->{JsonHelper.ToJson(req)}");

                            var postMsg = JsonHelper.ToJson(req);
                            Task.Run(() => { ForwardPush(postMsg,Common.AkStreamWebConfig.ForwardUrlOut); }); //抛线程出去处理

                        }
                    }
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
                                ushort? rtpPort;
                                lock (GCommon.Ldb.LiteDBLockObj)
                                {
                                    rtpPort = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                                            x.MediaServerId.Equals(videoChannel.MediaServerId) &&
                                            x.MainId.Equals(videoChannel.MainId))
                                        .MediaServerStreamInfo!.RptPort;
                                }

                                if (rtpPort != null && rtpPort > 0)
                                {
                                    ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort =
                                        new ReqZLMediaKitCloseRtpPort()
                                        {
                                            Stream_Id = videoChannel.MainId,
                                        };
                                    mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                                    mediaServer.KeeperWebApi.ReleaseRtpPort(
                                        (ushort)rtpPort,
                                        out _); //释放rtp端口
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                            x.MediaServerId.Equals(videoChannel.MediaServerId) && x.MainId.Equals(videoChannel.MainId));
                    }
                }
                else
                {
                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                            x.MainId.Equals(req.Stream));
                    }
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
                VideoChannelMediaInfo retobj = null;
                lock (GCommon.Ldb.LiteDBLockObj) //当无人观看事件触发时，又不在录制的情况下，清除playerlist
                {
                    retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                        x.MainId.Equals(req.Stream) && x.MediaServerId.Equals(req.MediaServerId));
                    if (retobj != null && retobj.MediaServerStreamInfo != null &&
                        retobj.MediaServerStreamInfo.PlayerList != null &&
                        retobj.MediaServerStreamInfo.PlayerList.Count > 0)
                    {
                        retobj.MediaServerStreamInfo.PlayerList = null;
                        GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                    }
                }

                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                        .Where(x => x.MediaServerId.Equals(req.MediaServerId)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->OnStreamNoneReader->执行SQL:->{sql}");
                }

                #endregion

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
                            GCommon.Logger.Error(
                                $"[{Common.LoggerHead}]->无人观看回调发生时断开回放流时出现异常->{JsonHelper.ToJson(rs)}");
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
                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->收到WebHook-OnStreamChanged回调(流接入)->{JsonHelper.ToJson(req)}");
                    mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
                    if (mediaServer == null)
                    {
                        return new ResToWebHookOnStreamChange()
                        {
                            Code = 0,
                            Msg = "success",
                        };
                    }

                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream)).ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->OnStreamChanged->执行SQL:->{sql}");
                    }

                    #endregion

                    var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                        .First();
                    if (videoChannel == null)
                    {
                        ///对未纳入AKStream管理的流接入进行外部转发
                        if ((!string.IsNullOrEmpty(Common.AkStreamWebConfig.ForwardUrlIn)) &&
                            (UtilsHelper.IsUrl(Common.AkStreamWebConfig.ForwardUrlIn)) &&
                            (req.Regist==true) &&
                            (Common.AkStreamWebConfig.ForwardUnmanagedRtmpRtspRtcStream) && (
                                (req.OriginType.Equals(OriginType.rtmp_push) ||
                                 req.OriginType.Equals(OriginType.rtsp_push) ||
                                 req.OriginType.Equals(OriginType.rtc_push))))
                        {
                            GCommon.Logger.Info(
                                $"[{Common.LoggerHead}]->转发接入流信息->{Common.AkStreamWebConfig.ForwardUrlIn}->{JsonHelper.ToJson(req)}");

                            var postMsg = JsonHelper.ToJson(req);
                            Task.Run(() => { ForwardPush(postMsg,Common.AkStreamWebConfig.ForwardUrlIn); }); //抛线程出去处理

                        }

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
                                GCommon.Logger.Warn(
                                    $"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
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
                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->收到WebHook-OnStreamChanged回调(流移除)->{JsonHelper.ToJson(req)}");

                    mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
                    if (mediaServer == null)
                    {
                        return new ResToWebHookOnStreamChange()
                        {
                            Code = 0,
                            Msg = "success",
                        };
                    }


                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream)).ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->OnStreamChanged->执行SQL:->{sql}");
                    }

                    #endregion

                    try
                    {
                        ///如果是留移除，就要断掉录制
                        mediaServer.WebApiHelper.StopRecord(new ReqZLMediaKitStopRecord()
                        {
                            App = req.App,
                            Stream = req.Stream,
                            Vhost = req.Vhost,
                        }, out _);
                    }
                    catch
                    {
                    }

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
                            ushort? rtpPort;
                            lock (GCommon.Ldb.LiteDBLockObj)
                            {
                                rtpPort = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                                        x.MediaServerId.Equals(videoChannel.MediaServerId) &&
                                        x.MainId.Equals(videoChannel.MainId))
                                    .MediaServerStreamInfo!.RptPort;
                            }

                            if (rtpPort != null && rtpPort > 0)
                            {
                                ReqZLMediaKitCloseRtpPort reqZlMediaKitCloseRtpPort = new ReqZLMediaKitCloseRtpPort()
                                {
                                    Stream_Id = videoChannel.MainId,
                                };
                                mediaServer.WebApiHelper.CloseRtpPort(reqZlMediaKitCloseRtpPort, out _); //关掉rtp端口
                                mediaServer.KeeperWebApi.ReleaseRtpPort(
                                    (ushort)rtpPort,
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

                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                            x.MediaServerId.Equals(videoChannel.MediaServerId) && x.MainId.Equals(videoChannel.MainId));
                    }
                }
            }
            else if (req.Schema.Trim().ToLower().Equals("rtmp") && IsRecordStream(req.Stream, out _))
            {
                if (req.Regist == false)
                {
                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                            x.MediaServerId.Equals(req.MediaServerId) && x.MainId.Equals(req.Stream));
                    }
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
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                        .Where(x => x.MediaServerId.Equals(req.MediaServerId)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->OnPlay->执行SQL:->{sql}");
                }

                #endregion

                var videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                    .Where(x => x.MediaServerId.Equals(req.MediaServerId)).First();

                if (videoChannel == null)
                {
                    if (Common.AkStreamWebConfig.ForwardUnmanagedRtmpRtspRtcStream)
                    {
                        return new ResToWebHookOnPlay()
                        {
                            Code = 0,
                            Msg = "success",
                        };  
                    }
                    else
                    {
                        return new ResToWebHookOnPlay()
                        {
                            Code = -1,
                            Msg = "feild",
                        };
                    }
                }


                VideoChannelMediaInfo retobj = null;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                        x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                }

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
                        Port = (ushort)req.Port,
                        StartTime = DateTime.Now,
                    });
                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                    }
                }
            }
            else
            {
                VideoChannelMediaInfo retobj = null;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                        x.MainId.Equals(req.Stream));
                }

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
                        Port = (ushort)req.Port,
                        StartTime = DateTime.Now,
                    });
                    lock (GCommon.Ldb.LiteDBLockObj)
                    {
                        GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                    }
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
                    Enable_Hls = false,
                    Enable_Mp4 = false,
                    Msg = "failed",
                };
            }

            VideoChannel videoChannel = null;

            if (!IsRecordStream(req.Stream, out _))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->OnPublish->执行SQL:->{sql}");
                }

                #endregion

                videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(req.Stream))
                    .First();
                if (videoChannel == null)
                {
                    if (Common.AkStreamWebConfig.ForwardUnmanagedRtmpRtspRtcStream)
                    {
                        
                        ResToWebHookOnPublish result = new ResToWebHookOnPublish();
                        result.Code = 0;
                        result.Msg = "success";
                        result.Enable_Hls = true;
                        result.Enable_Mp4 = false;
                        result.Enable_Hls_Fmp4 = true;
                        result.Enable_Rtsp = true;
                        result.Enable_Rtmp = true;
                        result.Enable_Ts = true;
                        result.Enable_Fmp4 = true;
                        result.Hls_Demand = true;
                        result.Rtsp_Demand = false;
                        result.Rtmp_Demand = false;
                        result.Ts_Demand = true;
                        result.Fmp4_Demand = true;
                        result.Enable_Audio = true;
                        result.Add_Mute_Audio = true;
                        result.Mp4_Save_Path = "";
                        result.Mp4_As_Player = false;
                        result.Hls_Save_Path = "";
                        result.Auto_Close = false;
                        return result;
                    }
                    else
                    {
                        return new ResToWebHookOnPublish()
                        {
                            Code = -1,
                            Enable_Hls = false,
                            Enable_Mp4 = false,
                            Msg = "failed",
                        };
                    }
                }

                if (videoChannel.Enabled == false || videoChannel.MediaServerId.Contains("unknown_server"))
                {
                    return new ResToWebHookOnPublish()
                    {
                        Code = -1,
                        Enable_Hls = false,
                        Enable_Mp4 = false,
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
                result.Msg = "success";
                result.Enable_Hls = true;
                result.Enable_Mp4 = false;
                result.Enable_Hls_Fmp4 = true;
                result.Enable_Rtsp = true;
                result.Enable_Rtmp = true;
                result.Enable_Ts = true;
                result.Enable_Fmp4 = true;
                result.Hls_Demand = true;
                result.Rtsp_Demand = false;
                result.Rtmp_Demand = false;
                result.Ts_Demand = true;
                result.Fmp4_Demand = true;
                result.Enable_Audio = true;
                result.Add_Mute_Audio = true;
                result.Mp4_Save_Path = "";
                result.Mp4_As_Player = false;
                result.Hls_Save_Path = "";
                result.Auto_Close = false;
                return result;
            }

            return new ResToWebHookOnPublish()
            {
                Code = -1,
                Enable_Hls = false,
                Enable_Mp4 = false,
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

                        List<VideoChannelMediaInfo> removeList = null;
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            removeList = GCommon.Ldb.VideoOnlineInfo
                                .Find(x => x.MediaServerId.Equals(req.MediaServerId))
                                .ToList();
                        }

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


                    mediaServer.Candidate = req.Candidate;
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
                    mediaServer.RecordSec = req.RecordSec;
                    mediaServer.ZlmBuildDateTime = req.ZlmBuildDateTime;
                    mediaServer.AKStreamKeeperVersion = req.Version;
                    mediaServer.CutMergeFilePath = req.CutMergeFilePath;
                    mediaServer.DisksUseable.Clear();
                    if (req.DisksUseable != null && req.DisksUseable.Count > 0)
                    {
                        foreach (var disk in req.DisksUseable)
                        {
                            mediaServer.DisksUseable.Add(disk.Key, disk.Value);
                        }
                    }


                    if (req.PerformanceInfo != null) //更新性能信息
                    {
                        mediaServer.PerformanceInfo = req.PerformanceInfo;
                    }

                    if (mediaServer.IsInitRtspAuthData == false)
                    {
                        #region debug sql output

                        if (Common.IsDebug)
                        {
                            var sql = ORMHelper.Db.Select<UserAuth>()
                                .Where(x => x.MediaServerId.Equals(mediaServer.MediaServerId)).ToSql();

                            GCommon.Logger.Debug(
                                $"[{Common.LoggerHead}]->MediaServerKeepAlive->执行SQL:->{sql}");
                        }

                        #endregion

                        var tmp_list_count = ORMHelper.Db.Select<UserAuth>()
                            .Where(x => x.MediaServerId.Equals(mediaServer.MediaServerId)).Count();
                        if (tmp_list_count <= 0)
                        {
                            UserAuth auth = new UserAuth()
                            {
                                MediaServerId = mediaServer.MediaServerId,
                                Username = "defaultuser",
                                Password = UtilsHelper.Md5New($"defaultuser:default:defaultpasswd"),
                            };
                            var b = ORMHelper.Db.Insert<UserAuth>(auth).ExecuteAffrows();
                            //  var b = MediaServerService.AddRtspAuthData(auth, out _);
                            if (b > 0)
                            {
                                mediaServer.IsInitRtspAuthData = true;
                            }
                        }
                        else
                        {
                            mediaServer.IsInitRtspAuthData = true;
                        }
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
                    tmpMediaServer.RecordSec = req.RecordSec;
                    tmpMediaServer.AKStreamKeeperVersion = req.Version;
                    tmpMediaServer.ZlmBuildDateTime = req.ZlmBuildDateTime;
                    tmpMediaServer.CutMergeFilePath = req.CutMergeFilePath;
                    tmpMediaServer.DisksUseable.Clear();
                    if (req.DisksUseable != null && req.DisksUseable.Count > 0)
                    {
                        foreach (var disk in req.DisksUseable)
                        {
                            tmpMediaServer.DisksUseable.Add(disk.Key, disk.Value);
                        }
                    }

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