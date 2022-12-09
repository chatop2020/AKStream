using System;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.WebRequest;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace AKStreamWeb.Misc
{
    public static class SipClientProcess
    {
        /// <summary>
        /// 停止共享流
        /// </summary>
        /// <param name="fromTag"></param>
        /// <param name="toTag"></param>
        /// <param name="callid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeInviteChannel(string fromTag, string toTag, string callid, out ResponseStruct rs,
            out ShareInviteInfo info)
        {
            info = null;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            lock (Common.ShareInviteChannels)
            {
                var obj = Common.ShareInviteChannels.FindLast(x => x.FromTag.Equals(fromTag)
                                                                   && x.ToTag.Equals(toTag) && x.CallId.Equals(callid));
                if (obj != null)
                {
                    var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(obj.MediaServerId));
                    if (mediaServer == null || mediaServer.KeeperWebApi == null || !mediaServer.IsKeeperRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                        };
                        return false;
                    }

                    var req = new ReqZLMediaKitStopSendRtp()
                    {
                        App = obj.App,
                        Ssrc = obj.Ssrc,
                        Stream = obj.Stream,
                        Vhost = obj.Vhost
                    };
                    var ret = mediaServer.WebApiHelper.StopSendRtp(req, out rs);
                    if (ret.Code == 0 && rs.Code.Equals(ErrorNumber.None))
                    {
                        info = JsonHelper.FromJson<ShareInviteInfo>(JsonHelper.ToJson(obj));
                        lock (Common.ShareInviteChannels)
                        {
                            Common.ShareInviteChannels.Remove(obj);
                        }

                        return true;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_NotOnPushStream,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_NotOnPushStream],
                    };
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 共享流
        /// </summary>
        /// <param name="info"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool InviteChannel(ShareInviteInfo info, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            ResZLMediakitStartSendRtp ret = null;
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(info.MediaServerId));
            if (mediaServer == null || mediaServer.KeeperWebApi == null || !mediaServer.IsKeeperRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                };
                return false;
            }

            var con = new ReqGetOnlineStreamInfoList();
            con.MediaServerId = info.MediaServerId;
            con.MainId = info.Stream;
            con.StreamSourceType = StreamSourceType.Live;
            var onlineList = MediaServerService.GetOnlineStreamInfoList(con, out rs);
            if (onlineList != null && rs.Code.Equals(ErrorNumber.None))
            {
                var channel = onlineList.VideoChannelMediaInfo.FindLast(x => x.App.Equals(info.App)
                                                                             && x.MainId.Equals(info.Stream) &&
                                                                             x.Vhost.Equals(info.Vhost));
                if (channel != null)
                {
                    ShareInviteInfo obj = null;
                    lock (Common.ShareInviteChannels)
                    {
                        obj = Common.ShareInviteChannels.FindLast(x => x.App.Equals(info.App) &&
                                                                       x.Stream.Equals(info.Stream) &&
                                                                       x.Vhost.Equals(info.Vhost));
                    }

                    if (obj != null)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sip_AlredayPushStream,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_AlredayPushStream],
                            ExceptMessage = JsonHelper.ToJson(info),
                            ExceptStackTrace = JsonHelper.ToJson(obj),
                        };
                        return false;
                    }

                    var req = new ReqZLMediaKitStartSendRtp()
                    {
                        App = info.App,
                        Stream = info.Stream,
                        Dst_Port = info.RemotePort.ToString(),
                        Dst_Url = info.RemoteIpAddress.Trim(),
                        Is_Udp = info.Is_Udp ? "1" : "0",
                        Src_Port = info.LocalRtpPort,
                        Vhost = info.Vhost,
                        Ssrc = info.Ssrc,
                    };
                    ret = mediaServer.WebApiHelper.StartSendRtp(req, out rs);
                    if (ret.Code == 0 && rs.Code.Equals(ErrorNumber.None))
                    {
                        info.LocalStream = string.Format("{0:X8}", uint.Parse(info.Ssrc));
                        info.PushDateTime = DateTime.Now;
                        lock (Common.ShareInviteChannels)
                        {
                            Common.ShareInviteChannels.Add(info);
                        }

                        return true;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_NotOnPushStream,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_NotOnPushStream],
                    };
                    return false;
                }
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = JsonHelper.ToJson(rs),
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_WebApiExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                ExceptMessage = $"ret.code:{ret.Code},ret.msg:{ret.Msg}",
            };
            return false;
        }
    }
}