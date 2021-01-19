using System;
using System.Threading;
using AKStreamWeb.Services;
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
        public static void OnRegister(string sipDeviceJson)
        {
            //设备注册时
        }

        public static void OnUnRegister(string sipDeviceJson)
        {
            //设备注销时
            try
            {
                var sipDevice = JsonHelper.FromJson<SipDevice>(sipDeviceJson);
                if (sipDevice != null && sipDevice.SipChannels!=null && sipDevice.SipChannels.Count>0)
                {
                    foreach (var channel in sipDevice.SipChannels)
                    {
                        if (channel != null)
                        {
                            var mediaInfo =
                                Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(channel.Stream));
                            if (mediaInfo != null)
                            {
                               var ret= MediaServerService.StreamStop(mediaInfo.MediaServerId, mediaInfo.MainId,
                                    out ResponseStruct rs);
                               if (ret  && rs.Code.Equals(ErrorNumber.None))
                               {
                                   Logger.Info(
                                       $"[{Common.LoggerHead}]->设备注销->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}->通道-{channel.DeviceId}->注销成功");
                               }
                               else
                               {
                                   Logger.Warn(
                                       $"[{Common.LoggerHead}]->设备注销->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}->通道-{channel.DeviceId}->注销失败->{JsonHelper.ToJson(rs,Formatting.Indented)}");
                               }
                            }
                        }
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
               ResponseStruct rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_CallBackExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_CallBackExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                Logger.Error(
                    $"[{Common.LoggerHead}]->设备注销时异常->{JsonHelper.ToJson(rs,Formatting.Indented)}");
            }

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
            Logger.Debug(
                $"[{Common.LoggerHead}]->设备就绪->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}");
            ResponseStruct rs;
            SipMethodProxy sipMethodProxy2 = new SipMethodProxy(5000);
            if (sipMethodProxy2.GetSipDeviceInfo(sipDevice, out rs))
            {
                Logger.Debug(
                    $"[{Common.LoggerHead}]->获取设备信息成功->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(sipDevice.DeviceInfo, Formatting.Indented)}");
            }
            else
            {
                Logger.Warn(
                    $"[{Common.LoggerHead}]->获取设备信息失败->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
            }

            SipMethodProxy sipMethodProxy3 = new SipMethodProxy(5000);
            if (sipMethodProxy3.GetSipDeviceStatus(sipDevice, out rs))
            {
                Logger.Debug(
                    $"[{Common.LoggerHead}]->获取设备状态信息成功->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(sipDevice.DeviceStatus, Formatting.Indented)}");
            }
            else
            {
                Logger.Warn(
                    $"[{Common.LoggerHead}]->获取设备状态信息失败->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
            }

            SipMethodProxy sipMethodProxy = new SipMethodProxy(5000);
            if (sipMethodProxy.DeviceCatalogQuery(sipDevice, out rs))
            {
                Logger.Debug(
                    $"[{Common.LoggerHead}]->设备目录获取成功->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(sipDevice.SipChannels, Formatting.Indented)}");
            }
            else
            {
                Logger.Error(
                    $"[{Common.LoggerHead}]->设备目录获取失败->{sipDevice.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipDevice.DeviceId}\r\n{JsonHelper.ToJson(rs, Formatting.Indented)}");
            }
        }

        /// <summary>
        /// 收到设备目录时
        /// </summary>
        /// <param name="sipChannel"></param>
        public static void OnCatalogReceived(SipChannel sipChannel)
        {
            Logger.Debug(
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
                videoChannel.MethodByGetStream = MethodByGetStream.None;
                videoChannel.MainId = sipChannel.Stream;
                videoChannel.VideoDeviceType = VideoDeviceType.UNKNOW;
                try
                {
                    var ret = ORMHelper.Db.Insert(videoChannel).ExecuteAffrows();
                    if (ret > 0)
                    {
                        Logger.Debug(
                            $"[{Common.LoggerHead}]->写入一条新的设备目录到数据库，需激活后使用->{sipChannel.RemoteEndPoint.Address.MapToIPv4().ToString()}-{sipChannel.ParentId}:{sipChannel.DeviceId}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{Common.LoggerHead}]->数据库写入异常->{ex.Message}\r\n{ex.StackTrace}");
                }
            }
        }
    }
}