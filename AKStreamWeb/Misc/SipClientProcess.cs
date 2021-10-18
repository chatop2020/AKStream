using LibCommon;
using LibCommon.Structs;
using LibLogger;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;

namespace AKStreamWeb.Misc
{
    public static class SipClientProcess
    {

        public static bool DeInviteChannel(ShareInviteInfo info)
        {
            return false;
        }
        public static bool InviteChannel(ShareInviteInfo info)
        {

           ResponseStruct rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
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
            Logger.Debug($"提交sender的内容->{req}");
           var ret= mediaServer.WebApiHelper.StartSendRtp(req, out rs);
           if (ret.Code == 0)
           {

               Logger.Debug($"推流成功了->{ret}");
           }
           return true;
            
            
            var obj=Common.ShareInviteChannels.FindLast(x => x.Deviceid.Equals(info.ChannelId) && x.Ssrc.Equals(info.Ssrc));
            if (obj != null)
            {
                return false;
            }
            else
            {
                return true;  
            }
            
        }
    }
}