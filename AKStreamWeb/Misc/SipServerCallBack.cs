using System;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.GB28181.Sys;
using LibCommon.Structs.GB28181.XML;
using LibGB28181SipServer;
using LibLogger;
using Newtonsoft.Json;

namespace AKStreamWeb.Misc
{
    /// <summary>
    /// sip设备回调类
    /// </summary>
    public static class SipServerCallBack
    {
        /// <summary>
        /// 当设备注册需要鉴权时，用于获取外部的设备密钥
        /// </summary>
        /// <param name="sipDeviceId"></param>
        /// <returns>返回此设备的密钥</returns>
        public static string OnAuthentication(string sipDeviceId)
        {
            return null;
        }

        public static void OnRegister(string sipDeviceJson)
        {
            //设备注册时
        }

        public static void OnUnRegister(string sipDeviceJson)
        {
            //设备注销时，要清掉在线流
            var sipDevice = JsonHelper.FromJson<SipDevice>(sipDeviceJson);

            GCommon.Ldb.VideoOnlineInfo.DeleteMany(x => x.DeviceId.Equals(sipDevice.DeviceId));
             GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->设备注销->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}->所有通道-->注销成功");
        }

        public static void OnKeepalive(string deviceId, DateTime keepAliveTime, int lostTimes)
        {
            //设备有心跳时
        }

        public static void OnDeviceStatusReceived(SipDevice sipDevice, DeviceStatus deviceStatus)
        {
            //获取到设备状态时
        }

        public static void OnInviteHistoryVideoFinished(RecordInfo.RecItem record)
        {
            //收到设备的录像文件列表时
        }

        public static void OnDeviceReadyReceived(SipDevice sipDevice)
        {
             GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->设备就绪->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}");
            ResponseStruct rs;
            SipMethodProxy sipMethodProxy2 = new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
            if (sipMethodProxy2.GetSipDeviceInfo(sipDevice, out rs))
            {
                 GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->获取设备信息成功->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(sipDevice.DeviceInfo, Formatting.Indented)}");
            }
            else
            {
                 GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取设备信息失败->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
            }

            SipMethodProxy sipMethodProxy3 = new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
            if (sipMethodProxy3.GetSipDeviceStatus(sipDevice, out rs))
            {
                 GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->获取设备状态信息成功->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(sipDevice.DeviceStatus, Formatting.Indented)}");
            }
            else
            {
                 GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取设备状态信息失败->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
            }

            SipMethodProxy sipMethodProxy = new SipMethodProxy(Common.AkStreamWebConfig.WaitSipRequestTimeOutMSec);
            if (sipMethodProxy.DeviceCatalogQuery(sipDevice, out rs))
            {
                 GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->设备目录获取成功->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(sipDevice.SipChannels, Formatting.Indented)}");
            }
            else
            {
                 GCommon.Logger.Error(
                    $"[{Common.LoggerHead}]->设备目录获取失败->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
            }
        }

        /// <summary>
        /// 收到设备目录时
        /// </summary>
        /// <param name="sipChannel"></param>
        public static void OnCatalogReceived(SipChannel sipChannel)
        {
             GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->收到一条设备目录通知->{sipChannel.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipChannel.ParentId}:{sipChannel.DeviceId}");

            if (sipChannel.SipChannelType.Equals(SipChannelType.VideoChannel) &&
                sipChannel.SipChannelStatus != DevStatus.OFF) //只有视频设备并且是可用状态的进数据库
            {
                var obj = ORMHelper.Db.Select<VideoChannel>().Where(x =>
                    x.ChannelId.Equals(sipChannel.DeviceId) && x.DeviceId.Equals(sipChannel.ParentId) &&
                    x.DeviceStreamType.Equals(DeviceStreamType.GB28181)).First();
                if (obj != null)
                {
                    return;
                }

                var videoChannel = new VideoChannel();
                videoChannel.Enabled = false;
                videoChannel.AutoRecord = false;
                videoChannel.AutoVideo = true;
                videoChannel.ChannelId = sipChannel.DeviceId;
                if (sipChannel.SipChannelDesc != null && !string.IsNullOrEmpty(sipChannel.SipChannelDesc.Name))
                {
                    videoChannel.ChannelName = sipChannel.SipChannelDesc.Name.Trim();
                }
                else
                {
                    videoChannel.ChannelName = sipChannel.DeviceId;
                }

                videoChannel.CreateTime = DateTime.Now;
                videoChannel.App = "rtp";
                videoChannel.Vhost = "__defaultVhost__";
                videoChannel.DepartmentId = "";
                videoChannel.DepartmentName = "";
                videoChannel.DeviceId = sipChannel.ParentId;
                videoChannel.HasPtz = false;
                videoChannel.UpdateTime = DateTime.Now;
                videoChannel.DeviceNetworkType = DeviceNetworkType.Fixed;
                videoChannel.DeviceStreamType = DeviceStreamType.GB28181;
                videoChannel.DefaultRtpPort = false;
                videoChannel.IpV4Address = sipChannel.RemoteEndPoint.Address.MapToIPv4().ToString();
                videoChannel.IpV6Address = sipChannel.RemoteEndPoint.Address.MapToIPv6().ToString();
                videoChannel.MediaServerId = $"unknown_server_{DateTime.Now.Ticks}";
                videoChannel.NoPlayerBreak = false;
                videoChannel.PDepartmentId = "";
                videoChannel.PDepartmentName = "";
                videoChannel.RtpWithTcp = false;
                videoChannel.VideoSrcUrl = null;
                videoChannel.RecordSecs = 0;
                videoChannel.MethodByGetStream = MethodByGetStream.None;
                videoChannel.MainId = sipChannel.Stream;
                videoChannel.VideoDeviceType = VideoDeviceType.UNKNOW;
                try
                {
                    var ret = ORMHelper.Db.Insert(videoChannel).ExecuteAffrows();
                    if (ret > 0)
                    {
                         GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->写入一条新的设备目录到数据库，需激活后使用->{sipChannel.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipChannel.ParentId}:{sipChannel.DeviceId}");
                    }
                }
                catch (Exception ex)
                {
                     GCommon.Logger.Error($"[{Common.LoggerHead}]->数据库写入异常->{ex.Message}\r\n{ex.StackTrace}");
                }
            }
        }
    }
}