using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.GB28181.Net.SDP;
using LibCommon.Structs.GB28181.Net.SIP;
using LibCommon.Structs.GB28181.Sys;
using LibCommon.Structs.GB28181.XML;
using SIPSorcery.SIP;

namespace LibGB28181SipServer
{
    /// <summary>
    /// sip网关类
    /// </summary>
    public class SipServer
    {
        /// <summary>
        /// SipTCP通道(IPV4)
        /// </summary>
        private SIPTCPChannel _sipTcpIpV4Channel = null!;

        /// <summary>
        /// SipTCP通道(IPV6)
        /// </summary>
        private SIPTCPChannel _sipTcpIpV6Channel = null!;

        /// <summary>
        /// SIP传输通道
        /// </summary>
        private SIPTransport _sipTransport = null!;

        /// <summary>
        /// SipUDP通道(IPV4)
        /// </summary>
        private SIPUDPChannel _sipUdpIpV4Channel = null!;

        /// <summary>
        /// SipUDP通道(IPV6)
        /// </summary>
        private SIPUDPChannel _sipUdpIpV6Channel = null!;

        public SipServer(string outConfigPath = "")
        {
            ResponseStruct rs;
            GCommon.Logger.Info($"[{Common.LoggerHead}]->加载配置文件->{Common.SipServerConfigPath}");
            if (!string.IsNullOrEmpty(outConfigPath))
            {
                Common.SipServerConfigPath = outConfigPath + "SipServerConfig.json";
            }

            Common.SipServerConfigPath = UtilsHelper.FindPreferredConfigFile(Common.SipServerConfigPath); //查找优先使用的配置文件

            var ret = Common.ReadSipServerConfig(out rs);

            if (ret < 0 || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Error($"[{Common.LoggerHead}]->加载配置文件失败->{Common.SipServerConfigPath}");
                throw new AkStreamException(rs);
            }

            Common.SipServer = this;
            GCommon.Logger.Info($"[{Common.LoggerHead}]->加载配置文件成功->{Common.SipServerConfigPath}");
        }

        /// <summary>
        /// SIP传输通道(公开)
        /// </summary>
        public SIPTransport SipTransport
        {
            get => _sipTransport;
            set => _sipTransport = value;
        }


        /// <summary>
        /// 向通道发送Sip指令
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <param name="subject"></param>
        /// <param name="xmlBody"></param>
        /// <param name="needResponse"></param>
        /// <param name="evnt"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private async Task SendRequestViaSipChannel(SipDevice sipDevice, SipChannel sipChannel, SIPMethodsEnum method,
            string contentType,
            string xmlBody, string subject, CommandType commandType, bool needResponse, AutoResetEvent evnt,
            AutoResetEvent evnt2, object obj,
            int timeout)
        {
            try
            {
                IPAddress sipDeviceIpAddr = sipDevice.RemoteEndPoint.Address;
                int sipDevicePort = sipDevice.RemoteEndPoint.Port;
                SIPProtocolsEnum protocols = sipDevice.RemoteEndPoint.Protocol;
                var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                    new SIPEndPoint(protocols, new IPEndPoint(sipDeviceIpAddr, sipDevicePort)));
                toSipUri.User = sipChannel.DeviceId;
                SIPToHeader to = new SIPToHeader(null, toSipUri, null);
                IPAddress sipServerIpAddress = IPAddress.Parse(Common.SipServerConfig.SipIpAddress);
                var fromSipUri = new SIPURI(SIPSchemesEnum.sip, sipServerIpAddress, Common.SipServerConfig.SipPort);
                fromSipUri.User = Common.SipServerConfig.ServerSipDeviceId;

                SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStream");


                SIPRequest req = SIPRequest.GetRequest(method, toSipUri, to,
                    from,
                    new SIPEndPoint(sipDevice.SipChannelLayout.SIPProtocol,
                        new IPEndPoint(
                            IPAddress.Parse(Common.SipServerConfig.SipIpAddress),
                            sipDevice.SipChannelLayout.Port)));


                req.Header.Allow = null;

                req.Header.Contact = new List<SIPContactHeader>()
                {
                    new SIPContactHeader(null, fromSipUri)
                };
                req.Header.UserAgent = ConstString.SIP_USERAGENT_STRING;
                req.Header.ContentType = contentType;
                req.Header.Subject = string.IsNullOrEmpty(subject) ? null : subject;
                req.Header.CallId = CallProperties.CreateNewCallId();
                req.Header.CSeq = UtilsHelper.CreateNewCSeq();
                req.Body = xmlBody;
                if (needResponse)
                {
                    var nrt = new NeedReturnTask(Common.NeedResponseRequests)
                    {
                        AutoResetEvent = evnt,
                        CallId = req.Header.CallId,
                        SipRequest = req,
                        Timeout = timeout,
                        SipDevice = sipDevice,
                        SipChannel = sipChannel,
                        AutoResetEvent2 = evnt2 == null ? null : evnt2,
                        CommandType = commandType,
                        Obj = obj == null ? null : obj, //额外的通用类
                    };
                    Common.NeedResponseRequests.TryAdd(req.Header.CallId, nrt);
                }

                if (commandType == CommandType.Playback && obj != null)
                {
                    ((RecordInfo.RecItem)obj).InviteSipRequest = req;
                    ((RecordInfo.RecItem)obj).CallId = req.Header.CallId;
                    ((RecordInfo.RecItem)obj).CSeq = -1;
                    ((RecordInfo.RecItem)obj).ToTag = "";
                    ((RecordInfo.RecItem)obj).FromTag = req.Header.From.FromTag;
                }
                else if (commandType == CommandType.Play)
                {
                    sipChannel.InviteSipRequest = req;
                }

                sipChannel.LastSipRequest = req;
                GCommon.Logger.Debug($"[{Common.LoggerHead}]->发送Sip请求->{req}");
                await _sipTransport.SendRequestAsync(sipDevice.RemoteEndPoint, req);
            }
            catch (Exception ex)
            {
                ResponseStruct rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_SendMessageExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SendMessageExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                throw new AkStreamException(rs);
            }
        }


        /// <summary>
        /// 向设备发送Sip指令
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <param name="subject"></param>
        /// <param name="xmlBody"></param>
        /// <param name="needResponse"></param>
        /// <param name="evnt"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private async Task SendRequestViaSipDevice(SipDevice sipDevice, SIPMethodsEnum method, string contentType,
            string subject,
            string xmlBody, CommandType commandType, bool needResponse, AutoResetEvent evnt, AutoResetEvent evnt2,
            object obj,
            int timeout)
        {
            try
            {
                IPAddress sipDeviceIpAddr = sipDevice.RemoteEndPoint.Address;
                int sipDevicePort = sipDevice.RemoteEndPoint.Port;
                SIPProtocolsEnum protocols = sipDevice.RemoteEndPoint.Protocol;
                var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                    new SIPEndPoint(protocols, new IPEndPoint(sipDeviceIpAddr, sipDevicePort)));
                toSipUri.User = sipDevice.DeviceId;
                SIPToHeader to = new SIPToHeader(null, toSipUri, null);
                IPAddress sipServerIpAddress = IPAddress.Parse(Common.SipServerConfig.SipIpAddress);
                var fromSipUri = new SIPURI(SIPSchemesEnum.sip, sipServerIpAddress, Common.SipServerConfig.SipPort);
                fromSipUri.User = Common.SipServerConfig.ServerSipDeviceId;
                SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStream");

                SIPRequest req = SIPRequest.GetRequest(method, toSipUri, to,
                    from,
                    new SIPEndPoint(sipDevice.SipChannelLayout.SIPProtocol,
                        new IPEndPoint(
                            IPAddress.Parse(Common.SipServerConfig.SipIpAddress),
                            sipDevice.SipChannelLayout.Port)));

                req.Header.Allow = null;
                req.Header.Contact = new List<SIPContactHeader>()
                {
                    new SIPContactHeader(null, fromSipUri)
                };
                req.Header.UserAgent = ConstString.SIP_USERAGENT_STRING;
                req.Header.ContentType = contentType;
                req.Header.Subject = subject;
                req.Body = xmlBody;
                req.Header.CallId = CallProperties.CreateNewCallId();
                req.Header.CSeq = UtilsHelper.CreateNewCSeq();

                if (needResponse)
                {
                    var nrt = new NeedReturnTask(Common.NeedResponseRequests)
                    {
                        AutoResetEvent = evnt,
                        CallId = req.Header.CallId,
                        SipRequest = req,
                        Timeout = timeout,
                        SipDevice = sipDevice,
                        CommandType = commandType,
                        AutoResetEvent2 = evnt2 == null ? null : evnt2,
                        Obj = obj == null ? null : obj, //额外的通用类
                    };
                    Common.NeedResponseRequests.TryAdd(req.Header.CallId, nrt);
                }

                sipDevice.LastSipRequest = req;
                GCommon.Logger.Debug($"[{Common.LoggerHead}]->发送Sip请求->{req}");

                await _sipTransport.SendRequestAsync(sipDevice.RemoteEndPoint, req);
            }
            catch (Exception ex)
            {
                ResponseStruct rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_SendMessageExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SendMessageExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                throw new AkStreamException(rs);
            }
        }


        private async Task SendRequestForRecordSeekPosition(SipDevice sipDevice, SipChannel sipChannel,
            SIPMethodsEnum method,
            string contentType,
            string xmlBody, string subject, CommandType commandType, bool needResponse, AutoResetEvent evnt,
            AutoResetEvent evnt2, object obj,
            int timeout)
        {
            try
            {
                IPAddress sipDeviceIpAddr = sipDevice.RemoteEndPoint.Address;
                int sipDevicePort = sipDevice.RemoteEndPoint.Port;
                SIPProtocolsEnum protocols = sipDevice.RemoteEndPoint.Protocol;
                var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                    new SIPEndPoint(protocols, new IPEndPoint(sipDeviceIpAddr, sipDevicePort)));
                toSipUri.User = sipChannel.DeviceId;
                SIPToHeader to = new SIPToHeader(null, toSipUri, null);
                IPAddress sipServerIpAddress = IPAddress.Parse(Common.SipServerConfig.SipIpAddress);
                var fromSipUri = new SIPURI(SIPSchemesEnum.sip, sipServerIpAddress, Common.SipServerConfig.SipPort);
                fromSipUri.User = Common.SipServerConfig.ServerSipDeviceId;

                SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStream");


                SIPRequest req = SIPRequest.GetRequest(method, toSipUri, to,
                    from,
                    new SIPEndPoint(sipDevice.SipChannelLayout.SIPProtocol,
                        new IPEndPoint(
                            IPAddress.Parse(Common.SipServerConfig.SipIpAddress),
                            sipDevice.SipChannelLayout.Port)));


                req.Header.Allow = null;

                req.Header.Contact = new List<SIPContactHeader>()
                {
                    new SIPContactHeader(null, fromSipUri)
                };
                req.Header.UserAgent = ConstString.SIP_USERAGENT_STRING;
                req.Header.ContentType = contentType;
                req.Header.Subject = string.IsNullOrEmpty(subject) ? null : subject;
                req.Header.CallId = ((RecordInfo.RecItem)obj).CallId;
                req.Header.CSeq = ((RecordInfo.RecItem)obj).CSeq;
                req.Body = xmlBody;
                if (needResponse)
                {
                    var nrt = new NeedReturnTask(Common.NeedResponseRequests)
                    {
                        AutoResetEvent = evnt,
                        CallId = req.Header.CallId,
                        SipRequest = req,
                        Timeout = timeout,
                        SipDevice = sipDevice,
                        SipChannel = sipChannel,
                        AutoResetEvent2 = evnt2 == null ? null : evnt2,
                        CommandType = commandType,
                        Obj = obj == null ? null : obj, //额外的通用类
                    };
                    Common.NeedResponseRequests.TryAdd(req.Header.CallId, nrt);
                }

                if (commandType == CommandType.Playback && obj != null)
                {
                    ((RecordInfo.RecItem)obj).InviteSipRequest = req;
                    ((RecordInfo.RecItem)obj).CallId = ((RecordInfo.RecItem)obj).CallId;
                    ((RecordInfo.RecItem)obj).CSeq = ((RecordInfo.RecItem)obj).CSeq;
                    ((RecordInfo.RecItem)obj).ToTag = ((RecordInfo.RecItem)obj).ToTag;
                    ((RecordInfo.RecItem)obj).FromTag = ((RecordInfo.RecItem)obj).FromTag;
                }
                else if (commandType == CommandType.Play)
                {
                    sipChannel.InviteSipRequest = req;
                }

                sipChannel.LastSipRequest = req;
                GCommon.Logger.Debug($"[{Common.LoggerHead}]->发送Sip请求->{req}");
                await _sipTransport.SendRequestAsync(sipDevice.RemoteEndPoint, req);
            }
            catch (Exception ex)
            {
                ResponseStruct rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_SendMessageExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SendMessageExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                throw new AkStreamException(rs);
            }
        }

        /// <summary>
        /// 检测请求实时视频流参数是否正确
        /// </summary>
        /// <param name="sipChannel"></param>
        /// <param name="rs"></param>
        private void CheckInviteParam(SipChannel sipChannel, PushStatus pushStatus, out ResponseStruct rs)
        {
            if (sipChannel == null) //传入参数是否正确
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                return;
            }

            var sipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(sipChannel.ParentId));
            if (sipDevice == null) //sip设备是否存在
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                return;
            }

            var tmpSipChannel = sipDevice.SipChannels.FindLast(x => x.DeviceId.Equals(sipChannel.DeviceId));
            if (tmpSipChannel == null) //sip 通道是否存在
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_ChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_ChannelNotExists],
                };
                return;
            }

            if (sipChannel.SipChannelStatus != null && !sipChannel.SipChannelStatus.Equals(DevStatus.OK) &&
                !sipChannel.SipChannelStatus.Equals(DevStatus.ON))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_Channel_StatusExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_Channel_StatusExcept],
                };
                return;
            }

            if (!sipChannel.SipChannelType.Equals(SipChannelType.VideoChannel) &&
                !sipChannel.SipChannelType.Equals(SipChannelType.AudioChannel)) //sip通道类型是否正确
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_OperationNotAllowed,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_OperationNotAllowed],
                };
                return;
            }

            if (pushStatus != PushStatus.IGNORE)
            {
                if (sipChannel.PushStatus == pushStatus)
                {
                    if (pushStatus == PushStatus.PUSHON)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sip_AlredayPushStream,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_AlredayPushStream],
                        };
                    }
                    else
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sip_NotOnPushStream,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_NotOnPushStream],
                        };
                    }

                    return;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
        }


        /// <summary>
        /// 生成回放流的sdp
        /// </summary>
        /// <param name="record"></param>
        /// <param name="pushMediaInfo"></param>
        /// <returns></returns>
        private string MediaSdpCreate(RecordInfo.RecItem record, PushMediaInfo pushMediaInfo)
        {
            var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(record.SipDevice.DeviceId));
            if (tmpSipDevice != null)
            {
                var sdpConn = new SDPConnectionInformation(pushMediaInfo.MediaServerIpAddress);
                var sdp = new SDP()
                {
                    Version = 0,
                    SessionId = "0",
                    Username = Common.SipServerConfig.ServerSipDeviceId,
                    SessionName = CommandType.Playback.ToString(),
                    Connection = sdpConn,
                    Timing = UtilsHelper.ConvertTimestamp(DateTime.Parse(record.StartTime), 1) + " " +
                             UtilsHelper.ConvertTimestamp(DateTime.Parse(record.EndTime), 1),
                    Address = pushMediaInfo.MediaServerIpAddress,
                    URI = record.SipChannel.DeviceId + ":0",
                };

                var psFormat = new SDPMediaFormat(SDPMediaFormatsEnum.PS)
                {
                    IsStandardAttribute = false,
                };
                var h264Format = new SDPMediaFormat(SDPMediaFormatsEnum.H264)
                {
                    IsStandardAttribute = false,
                };
                var media = new SDPMediaAnnouncement()
                {
                    Media = SDPMediaTypesEnum.video,
                    Port = pushMediaInfo.StreamPort,
                };
                media.MediaFormats.Add(psFormat);
                media.MediaFormats.Add(h264Format);
                media.AddExtra("a=recvonly");
                if (Common.SipServerConfig.IsPassive != null && Common.SipServerConfig.IsPassive == false)
                {
                    media.AddExtra("a=setup:active"); //active：主动模式，由摄像头告知服务器监听哪个端口，passive：被动模式，服务器告知摄像头连接端口
                }
                else
                {
                    media.AddExtra("a=setup:passive"); //active：主动模式，由摄像头告知服务器监听哪个端口，passive：被动模式，服务器告知摄像头连接端口
                }

                if (pushMediaInfo.PushStreamSocketType == PushStreamSocketType.TCP)
                {
                    media.Transport = "TCP/RTP/AVP";
                    media.AddExtra("a=connection:new");
                }

                media.AddExtra("y=" + record.SsrcId); //设置ssrc
                media.AddFormatParameterAttribute(psFormat.FormatID, psFormat.Name);
                media.AddFormatParameterAttribute(h264Format.FormatID, h264Format.Name);
                media.Port = pushMediaInfo.StreamPort;
                sdp.Media.Add(media);
                return sdp.ToString();
            }

            return "";
        }

        /// <summary>
        /// 生成实时流的sdp
        /// </summary>
        /// <param name="sipChannel"></param>
        /// <param name="pushMediaInfo"></param>
        /// <returns></returns>
        private string MediaSdpCreate(SipChannel sipChannel, PushMediaInfo pushMediaInfo)
        {
            var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(sipChannel.ParentId));
            if (tmpSipDevice != null)
            {
                var sdpConn = new SDPConnectionInformation(pushMediaInfo.MediaServerIpAddress);
                var sdp = new SDP()
                {
                    Version = 0,
                    SessionId = "0",
                    Username = Common.SipServerConfig.ServerSipDeviceId,
                    SessionName = CommandType.Play.ToString(),
                    Connection = sdpConn,
                    Timing = "0 0",
                    Address = pushMediaInfo.MediaServerIpAddress,
                };

                var psFormat = new SDPMediaFormat(SDPMediaFormatsEnum.PS)
                {
                    IsStandardAttribute = false,
                };
                var h264Format = new SDPMediaFormat(SDPMediaFormatsEnum.H264)
                {
                    IsStandardAttribute = false,
                };
                var media = new SDPMediaAnnouncement()
                {
                    Media = SDPMediaTypesEnum.video,
                    Port = pushMediaInfo.StreamPort,
                };
                media.MediaFormats.Add(psFormat);
                media.MediaFormats.Add(h264Format);
                media.AddExtra("a=recvonly");

                if (Common.SipServerConfig.IsPassive != null && Common.SipServerConfig.IsPassive == false)
                {
                    media.AddExtra("a=setup:active"); //active：主动模式，由摄像头告知服务器监听哪个端口，passive：被动模式，服务器告知摄像头连接端口
                }
                else
                {
                    media.AddExtra("a=setup:passive"); //active：主动模式，由摄像头告知服务器监听哪个端口，passive：被动模式，服务器告知摄像头连接端口
                }

                if (pushMediaInfo.PushStreamSocketType == PushStreamSocketType.TCP)
                {
                    media.Transport = "TCP/RTP/AVP";
                    media.AddExtra("a=connection:new");
                }

                media.AddExtra("y=" + sipChannel.SsrcId); //设置ssrc
                media.AddFormatParameterAttribute(psFormat.FormatID, psFormat.Name);
                media.AddFormatParameterAttribute(h264Format.FormatID, h264Format.Name);
                media.Port = pushMediaInfo.StreamPort;
                sdp.Media.Add(media);
                return sdp.ToString();
            }

            return "";
        }


        /// <summary>
        /// 拼接ptz控制指令
        /// </summary>
        /// <param name="ucommand">控制命令</param>
        /// <param name="dwSpeed">速度</param>
        /// <returns></returns>
        private string GetPtzCmd(PTZCommandType ucommand, int dwSpeed)
        {
            //10进制
            List<int> cmdList = new List<int>(8)
            {
                0xA5,
                0x0F,
                0x01
            };
            switch (ucommand)
            {
                case PTZCommandType.Stop:
                    cmdList.Add(00);
                    cmdList.Add(00);
                    cmdList.Add(00);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Up:
                    cmdList.Add(0x08);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Down:
                    cmdList.Add(0x04);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Left:
                    cmdList.Add(0x02);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Right:
                    cmdList.Add(0x01);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.UpRight:
                    cmdList.Add(0x09);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.DownRight:
                    cmdList.Add(0x05);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.UpLeft:
                    cmdList.Add(0x0A);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.DownLeft:
                    cmdList.Add(0x06);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Zoom1: //镜头放大
                    cmdList.Add(0x10);
                    cmdList.Add(00);
                    cmdList.Add(00);
                    cmdList.Add(dwSpeed << 4);
                    break;
                case PTZCommandType.Zoom2: //镜头缩小
                    cmdList.Add(0x20);
                    cmdList.Add(00);
                    cmdList.Add(00);
                    cmdList.Add(dwSpeed << 4);
                    break;
                case PTZCommandType.Focus1: //聚焦+
                    cmdList.Add(0x42);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Focus2: //聚焦—
                    cmdList.Add(0x41);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Iris1: //光圈open
                    cmdList.Add(0x44);
                    cmdList.Add(00);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.Iris2: //光圈close
                    cmdList.Add(0x48);
                    cmdList.Add(00);
                    cmdList.Add(dwSpeed);
                    cmdList.Add(00);
                    break;
                case PTZCommandType.SetPreset: //设置预置位
                    cmdList.Add(0x81);
                    cmdList.Add(00);
                    cmdList.Add(dwSpeed); //当id使用
                    cmdList.Add(00);
                    break;
                case PTZCommandType.GetPreset: //调用预置位
                    cmdList.Add(0x82);
                    cmdList.Add(00);
                    cmdList.Add(dwSpeed); //当id使用
                    cmdList.Add(0x80); //默认速度128
                    break;
                case PTZCommandType.RemovePreset: //删除预置位
                    cmdList.Add(0x83);
                    cmdList.Add(00);
                    cmdList.Add(dwSpeed); //当id使用
                    cmdList.Add(00);
                    break;
                default:
                    break;
            }

            int checkBit = 0;
            foreach (int cmdItem in cmdList)
            {
                checkBit += cmdItem;
            }

            checkBit = checkBit % 256;
            cmdList.Add(checkBit);

            string cmdStr = string.Empty;
            foreach (var cmdItemStr in cmdList)
            {
                cmdStr += cmdItemStr.ToString("X").PadLeft(2, '0'); //10进制转换为16进制
            }

            return cmdStr;
        }

        public void PtzMove(PtzCtrl ptzCtrl, AutoResetEvent evnt, out ResponseStruct rs, int timeout = 5000)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (Common.SipDevices.FindLast(x => x.DeviceId.Equals(ptzCtrl.SipDevice.DeviceId)) == null) //设备是否在列表中存在
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                try
                {
                    evnt.Set();
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

                return;
            }

            SIPMethodsEnum method = SIPMethodsEnum.MESSAGE;
            string ptzCmdStr = GetPtzCmd(ptzCtrl.PtzCommandType, ptzCtrl.Speed);
            PTZControl ptz = null;
            if (ptzCtrl.SipChannel != null)
            {
                ptz = new PTZControl()
                {
                    CommandType = CommandType.DeviceControl,
                    DeviceID = ptzCtrl.SipChannel.DeviceId,
                    SN = 11,
                    PTZCmd = ptzCmdStr
                };
            }
            else
            {
                ptz = new PTZControl()
                {
                    CommandType = CommandType.DeviceControl,
                    DeviceID = ptzCtrl.SipDevice.DeviceId,
                    SN = 11,
                    PTZCmd = ptzCmdStr
                };
            }

            try
            {
                string xmlBody = PTZControl.Instance.Save<PTZControl>(ptz);
                if (ptzCtrl.SipChannel == null)
                {
                    string subject =
                        $"{Common.SipServerConfig.ServerSipDeviceId}:{0},{ptzCtrl.SipDevice.DeviceId}:{new Random().Next(100, ushort.MaxValue)}";

                    Func<SipDevice, SIPMethodsEnum, string, string, string, CommandType, bool, AutoResetEvent,
                            AutoResetEvent, object, int, Task
                        >
                        request =
                            SendRequestViaSipDevice;
                    request(ptzCtrl.SipDevice, method, ConstString.Application_MANSCDP, subject, xmlBody,
                        ptz.CommandType,
                        true,
                        evnt, null, null, timeout);
                }
                else
                {
                    Func<SipDevice, SipChannel, SIPMethodsEnum, string, string, string, CommandType, bool,
                            AutoResetEvent, AutoResetEvent, object,
                            int, Task>
                        request =
                            SendRequestViaSipChannel;
                    var subject = "";

                    request(ptzCtrl.SipDevice, ptzCtrl.SipChannel, method, ConstString.Application_MANSCDP, xmlBody,
                        subject, ptz.CommandType, true, evnt, null, null,
                        timeout);
                }
            }
            catch (AkStreamException ex)
            {
                rs = ex.ResponseStruct;
                try
                {
                    evnt.Set();
                }
                catch (Exception exex)
                {
                    ResponseStruct exrs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_AutoResetEventExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                        ExceptMessage = exex.Message,
                        ExceptStackTrace = exex.StackTrace
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                }
            }
        }

        /// <summary>
        /// 获取设备状态信息
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="evnt"></param>
        /// <param name="rs"></param>
        /// <param name="timeout"></param>
        public void GetDeviceStatus(SipDevice sipDevice, AutoResetEvent evnt,
            out ResponseStruct rs, int timeout = 5000)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(sipDevice.DeviceId));
            if (tmpSipDevice != null)
            {
                SIPMethodsEnum method = SIPMethodsEnum.MESSAGE;
                string subject =
                    $"{Common.SipServerConfig.ServerSipDeviceId}:{0},{tmpSipDevice.DeviceId}:{new Random().Next(100, ushort.MaxValue)}";
                var body = new CatalogQuery()
                {
                    CommandType = CommandType.DeviceStatus,
                    DeviceID = tmpSipDevice.DeviceId,
                    SN = new Random().Next(1, ushort.MaxValue),
                };
                var xmlBody = CatalogQuery.Instance.Save<CatalogQuery>(body);
                try
                {
                    Func<SipDevice, SIPMethodsEnum, string, string, string, CommandType, bool, AutoResetEvent,
                            AutoResetEvent, object, int, Task
                        >
                        request =
                            SendRequestViaSipDevice;
                    request(tmpSipDevice, method, ConstString.Application_MANSCDP, subject, xmlBody, body.CommandType,
                        true,
                        evnt, null, null, timeout);
                }
                catch (AkStreamException ex)
                {
                    rs = ex.ResponseStruct;
                    try
                    {
                        evnt.Set();
                    }
                    catch (Exception exex)
                    {
                        ResponseStruct exrs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AutoResetEventExcept,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                            ExceptMessage = exex.Message,
                            ExceptStackTrace = exex.StackTrace
                        };
                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                    }
                }
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                try
                {
                    evnt.Set();
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
        }

        /// <summary>
        /// 获取通道的录像文件列表
        /// </summary>
        /// <param name="sipChannel"></param>
        /// <param name="sipQueryRecordFile"></param>
        /// <param name="evnt"></param>
        /// <param name="rs"></param>
        /// <param name="timeout"></param>
        public void GetRecordFileList(SipChannel sipChannel, SipQueryRecordFile sipQueryRecordFile, AutoResetEvent evnt,
            AutoResetEvent evnt2, out ResponseStruct rs, int timeout = 5000)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            CheckInviteParam(sipChannel, PushStatus.IGNORE, out rs); //检测各参数是否正常
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                try
                {
                    evnt.Set();
                    evnt2.Set();
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

                return;
            }

            var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(sipChannel.ParentId));
            SIPMethodsEnum method = SIPMethodsEnum.MESSAGE;
            var body = new RecordQuery()
            {
                DeviceID = sipChannel.DeviceId,
                SN = (int)sipQueryRecordFile.TaskId, //跟进去一个sn
                CmdType = CommandType.RecordInfo,
                Secrecy = 0,
                StartTime = sipQueryRecordFile.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                EndTime = sipQueryRecordFile.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                Type = sipQueryRecordFile.SipRecordFileQueryType.ToString(),
            };
            var subject =
                $"{sipChannel.DeviceId}:{0},{Common.SipServerConfig.ServerSipDeviceId}:{new Random().Next(100, ushort.MaxValue)}";
            var xmlBody = CatalogQuery.Instance.Save<RecordQuery>(body);
            try
            {
                Func<SipDevice, SipChannel, SIPMethodsEnum, string, string, string, CommandType, bool, AutoResetEvent,
                        AutoResetEvent, object,
                        int,
                        Task>
                    request =
                        SendRequestViaSipChannel;
                request(tmpSipDevice, sipChannel, method, ConstString.Application_MANSCDP, xmlBody, subject,
                    body.CmdType,
                    true,
                    evnt, evnt2, sipQueryRecordFile,
                    timeout);
            }
            catch (AkStreamException ex)
            {
                rs = ex.ResponseStruct;
                try
                {
                    evnt.Set();
                    evnt2.Set();
                }
                catch (Exception exex)
                {
                    ResponseStruct exrs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_AutoResetEventExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                        ExceptMessage = exex.Message,
                        ExceptStackTrace = exex.StackTrace
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                }
            }
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="sipDevice"></param>
        /// <param name="evnt"></param>
        /// <param name="rs"></param>
        /// <param name="timeout"></param>
        public void GetDeviceInfo(SipDevice sipDevice, AutoResetEvent evnt,
            out ResponseStruct rs, int timeout = 5000)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(sipDevice.DeviceId));
            if (tmpSipDevice != null)
            {
                SIPMethodsEnum method = SIPMethodsEnum.MESSAGE;
                string subject =
                    $"{Common.SipServerConfig.ServerSipDeviceId}:{0},{tmpSipDevice.DeviceId}:{new Random().Next(100, ushort.MaxValue)}";
                var body = new CatalogQuery()
                {
                    CommandType = CommandType.DeviceInfo,
                    DeviceID = tmpSipDevice.DeviceId,
                    SN = new Random().Next(1, ushort.MaxValue),
                };
                var xmlBody = CatalogQuery.Instance.Save<CatalogQuery>(body);
                try
                {
                    Func<SipDevice, SIPMethodsEnum, string, string, string, CommandType, bool, AutoResetEvent,
                            AutoResetEvent, object, int, Task
                        >
                        request =
                            SendRequestViaSipDevice;
                    request(tmpSipDevice, method, ConstString.Application_MANSCDP, subject, xmlBody, body.CommandType,
                        true,
                        evnt, null, null, timeout);
                }
                catch (AkStreamException ex)
                {
                    rs = ex.ResponseStruct;
                    try
                    {
                        evnt.Set();
                    }
                    catch (Exception exex)
                    {
                        ResponseStruct exrs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AutoResetEventExcept,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                            ExceptMessage = exex.Message,
                            ExceptStackTrace = exex.StackTrace
                        };
                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                    }
                }
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                try
                {
                    evnt.Set();
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
        }

        /// <summary>
        /// 检测回放参数是否正确
        /// </summary>
        /// <param name="record"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private void CheckInviteParam(RecordInfo.RecItem record, PushStatus pushStatus, out ResponseStruct rs)
        {
            if (record.SipDevice == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                return;
            }

            if (record.SipChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_ChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_ChannelNotExists],
                };
                return;
            }

            if (!record.SipChannel.SipChannelStatus.Equals(DevStatus.OK) &&
                !record.SipChannel.SipChannelStatus.Equals(DevStatus.ON))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_Channel_StatusExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_Channel_StatusExcept],
                };
                return;
            }

            if (pushStatus != PushStatus.IGNORE)
            {
                if (record.PushStatus == pushStatus)
                {
                    if (pushStatus == PushStatus.PUSHON)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sip_AlredayPushStream,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_AlredayPushStream],
                        };
                    }
                    else
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sip_NotOnPushStream,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_NotOnPushStream],
                        };
                    }

                    return;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
        }


        private string CreateRecordSeekPositionSdp(RecordInfo.RecItem record, long time)
        {
            string recordSdp =
                "PLAY MANSRTSP/1.0\r\n" +
                "CSeq: " + record.CSeq + "\r\n" +
                "Range: npt=" + time + "-\r\n";
            return recordSdp;
        }

        /// <summary>
        /// 回放视频时的seek position操作
        /// </summary>
        /// <param name="record"></param>
        /// <param name="pushMediaInfo"></param>
        /// <param name="time"></param>
        /// <param name="evnt"></param>
        /// <param name="rs"></param>
        /// <param name="timeout"></param>
        public void InviteRecordPosition(RecordInfo.RecItem record, PushMediaInfo pushMediaInfo, long time,
            AutoResetEvent evnt,
            out ResponseStruct rs, int timeout = 5000)
        {
            try
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };


                string recordSdp = CreateRecordSeekPositionSdp(record, time);

                if (!string.IsNullOrEmpty(recordSdp))
                {
                    SIPMethodsEnum method = SIPMethodsEnum.INFO;
                    var subject = record.InviteSipRequest.Header.Subject;
                    try
                    {
                        Func<SipDevice, SipChannel, SIPMethodsEnum, string, string, string, CommandType, bool,
                            AutoResetEvent, AutoResetEvent, object, int, Task> request =
                            SendRequestForRecordSeekPosition;

                        request(record.SipDevice, record.SipChannel, method, ConstString.Application_SDP, recordSdp,
                            subject, CommandType.Playback, false, evnt, null, record, timeout);
                    }
                    catch (AkStreamException ex)
                    {
                        rs = ex.ResponseStruct;
                        try
                        {
                            evnt.Set();
                        }
                        catch (Exception exex)
                        {
                            ResponseStruct exrs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_AutoResetEventExcept,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                                ExceptMessage = exex.Message,
                                ExceptStackTrace = exex.StackTrace
                            };
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                        }
                    }
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
                try
                {
                    evnt.Set();
                }
                catch (Exception exex)
                {
                    ResponseStruct exrs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_AutoResetEventExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                        ExceptMessage = exex.Message,
                        ExceptStackTrace = exex.StackTrace
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                }
            }
        }

        public void InviteRecord(RecordInfo.RecItem record, PushMediaInfo pushMediaInfo, AutoResetEvent evnt,
            out ResponseStruct rs, int timeout = 5000)
        {
            try
            {
                CheckInviteParam(record, PushStatus.PUSHON, out rs);
                if (!rs.Code.Equals(ErrorNumber.None))
                {
                    try
                    {
                        evnt.Set();
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

                    return;
                }

                string recordSdp = MediaSdpCreate(record, pushMediaInfo);
                if (!string.IsNullOrEmpty(recordSdp))
                {
                    SIPMethodsEnum method = SIPMethodsEnum.INVITE;
                    var subject =
                        $"{record.SipChannel.DeviceId}:{0},{Common.SipServerConfig.ServerSipDeviceId}:{new Random().Next(100, ushort.MaxValue)}";
                    try
                    {
                        Func<SipDevice, SipChannel, SIPMethodsEnum, string, string, string, CommandType, bool,
                            AutoResetEvent, AutoResetEvent, object, int, Task
                        > request = SendRequestViaSipChannel;

                        request(record.SipDevice, record.SipChannel, method, ConstString.Application_SDP, recordSdp,
                            subject,
                            CommandType.Playback,
                            true, evnt, null, record, timeout);
                    }
                    catch (AkStreamException ex)
                    {
                        rs = ex.ResponseStruct;
                        try
                        {
                            evnt.Set();
                        }
                        catch (Exception exex)
                        {
                            ResponseStruct exrs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_AutoResetEventExcept,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                                ExceptMessage = exex.Message,
                                ExceptStackTrace = exex.StackTrace
                            };
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                        }
                    }
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
                try
                {
                    evnt.Set();
                }
                catch (Exception exex)
                {
                    ResponseStruct exrs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_AutoResetEventExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                        ExceptMessage = exex.Message,
                        ExceptStackTrace = exex.StackTrace
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                }
            }
        }

        /// <summary>
        /// 请求实时视频流
        /// </summary>
        /// <param name="sipChannel"></param>
        public void Invite(SipChannel sipChannel, PushMediaInfo pushMediaInfo, AutoResetEvent evnt,
            out ResponseStruct rs, int timeout = 5000)
        {
            try
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                CheckInviteParam(sipChannel, PushStatus.PUSHON, out rs); //检测各参数是否正常
                if (!rs.Code.Equals(ErrorNumber.None))
                {
                    try
                    {
                        evnt.Set();
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

                    return;
                }


                string mediaSdp = MediaSdpCreate(sipChannel, pushMediaInfo);
                var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(sipChannel.ParentId));
                if (!string.IsNullOrEmpty(mediaSdp))
                {
                    SIPMethodsEnum method = SIPMethodsEnum.INVITE;
                    var tmpId = UtilsHelper.CreateGUID();
                    var tmpIntId = CRC32Helper.GetCRC32(tmpId);
                    tmpId = tmpIntId.ToString();
                    if (!tmpId.StartsWith('0'))
                    {
                        tmpId = "0" + tmpId;
                    }

                    var subject =
                        $"{sipChannel.DeviceId}:{tmpId},{Common.SipServerConfig.ServerSipDeviceId}:{new Random().Next(100, ushort.MaxValue)}";
                    try
                    {
                        Func<SipDevice, SipChannel, SIPMethodsEnum, string, string, string, CommandType, bool,
                            AutoResetEvent, AutoResetEvent, object, int, Task
                        > request = SendRequestViaSipChannel;

                        request(tmpSipDevice, sipChannel, method, ConstString.Application_SDP, mediaSdp, subject,
                            CommandType.Play,
                            true, evnt, null, null, timeout);
                    }
                    catch (AkStreamException ex)
                    {
                        rs = ex.ResponseStruct;
                        try
                        {
                            evnt.Set();
                        }
                        catch (Exception exex)
                        {
                            ResponseStruct exrs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_AutoResetEventExcept,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                                ExceptMessage = exex.Message,
                                ExceptStackTrace = exex.StackTrace
                            };
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                        }
                    }
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
                try
                {
                    evnt.Set();
                }
                catch (Exception exex)
                {
                    ResponseStruct exrs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_AutoResetEventExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                        ExceptMessage = exex.Message,
                        ExceptStackTrace = exex.StackTrace
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                }
            }
        }


        public void DeInvite(RecordInfo.RecItem record, AutoResetEvent evnt,
            out ResponseStruct rs, int timeout = 5000)
        {
            try
            {
                //请求终止实时视频流时，callid,from.tag,to.tag都要与invite时一致
                CheckInviteParam(record, PushStatus.IGNORE, out rs);
                if (!rs.Code.Equals(ErrorNumber.None))
                {
                    try
                    {
                        evnt.Set();
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

                    return;
                }


                var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(record.SipChannel.ParentId));
                SIPMethodsEnum method = SIPMethodsEnum.BYE;

                IPAddress sipDeviceIpAddr = tmpSipDevice.RemoteEndPoint.Address;
                int sipDevicePort = tmpSipDevice.RemoteEndPoint.Port;
                SIPProtocolsEnum protocols = tmpSipDevice.RemoteEndPoint.Protocol;


                var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                    new SIPEndPoint(protocols, new IPEndPoint(sipDeviceIpAddr, sipDevicePort)));
                toSipUri.User = record.SipChannel.DeviceId;
                SIPToHeader to = new SIPToHeader(null, toSipUri, null);
                IPAddress sipServerIpAddress = IPAddress.Parse(Common.SipServerConfig.SipIpAddress);
                var fromSipUri = new SIPURI(SIPSchemesEnum.sip, sipServerIpAddress, Common.SipServerConfig.SipPort);
                fromSipUri.User = Common.SipServerConfig.ServerSipDeviceId;

                SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStream");
                var fromUri = tmpSipDevice.LastSipRequest.URI;

                SIPRequest req = SIPRequest.GetRequest(method, toSipUri, to,
                    from);

                req.Header.CallId = record.CallId;
                req.Header.From = new SIPFromHeader(null, fromSipUri, record.FromTag);
                req.Header.To = new SIPToHeader(null, toSipUri, record.ToTag);
                req.Header.Contact = new List<SIPContactHeader>()
                {
                    new SIPContactHeader(null, fromSipUri),
                };
                req.Header.Contact[0].ContactName = null;
                req.Header.Allow = null;
                req.Header.UserAgent = ConstString.SIP_USERAGENT_STRING;
                record.CSeq++;
                req.Header.CSeq = record.CSeq;
                var nrt = new NeedReturnTask(Common.NeedResponseRequests)
                {
                    AutoResetEvent = evnt,
                    CallId = req.Header.CallId,
                    SipRequest = req,
                    Timeout = timeout,
                    SipDevice = tmpSipDevice,
                    SipChannel = record.SipChannel,
                    CommandType = CommandType.Unknown,
                    Obj = record,
                };
                Common.NeedResponseRequests.TryAdd(req.Header.CallId, nrt);
                GCommon.Logger.Debug($"[{Common.LoggerHead}]->发送结束回放流请求->{req}");
                _sipTransport.SendRequestAsync(tmpSipDevice.RemoteEndPoint, req);
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
                try
                {
                    evnt.Set();
                }
                catch (Exception exex)
                {
                    ResponseStruct exrs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_AutoResetEventExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                        ExceptMessage = exex.Message,
                        ExceptStackTrace = exex.StackTrace
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                }
            }
        }


        /// <summary>
        /// 请求终止实时视频流
        /// </summary>
        /// <param name="sipChannel"></param>
        public void DeInvite(SipChannel sipChannel, AutoResetEvent evnt,
            out ResponseStruct rs, int timeout = 5000)
        {
            try
            {
                //请求终止实时视频流时，callid,from.tag,to.tag都要与invite时一致
                CheckInviteParam(sipChannel, sipChannel.PushStatus, out rs);
                if (!rs.Code.Equals(ErrorNumber.None))
                {
                    try
                    {
                        evnt.Set();
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

                    return;
                }

                if (sipChannel.PushStatus != PushStatus.PUSHON && sipChannel.PushStatus != PushStatus.IGNORE)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_NotOnPushStream,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_NotOnPushStream],
                    };
                    try
                    {
                        evnt.Set();
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

                    return;
                }


                var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceId.Equals(sipChannel.ParentId));

                SIPRequest req = null;

                SIPMethodsEnum method = SIPMethodsEnum.BYE;
                IPAddress sipDeviceIpAddr = tmpSipDevice.RemoteEndPoint.Address;
                int sipDevicePort = tmpSipDevice.RemoteEndPoint.Port;
                SIPProtocolsEnum protocols = tmpSipDevice.RemoteEndPoint.Protocol;


                var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                    new SIPEndPoint(protocols, new IPEndPoint(sipDeviceIpAddr, sipDevicePort)));
                toSipUri.User = sipChannel.DeviceId;
                SIPToHeader to = new SIPToHeader(null, toSipUri, null);
                IPAddress sipServerIpAddress = IPAddress.Parse(Common.SipServerConfig.SipIpAddress);
                var fromSipUri = new SIPURI(SIPSchemesEnum.sip, sipServerIpAddress, Common.SipServerConfig.SipPort);
                fromSipUri.User = Common.SipServerConfig.ServerSipDeviceId;
                SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStream");
                var fromUri = tmpSipDevice.LastSipRequest.URI;

                req = SIPRequest.GetRequest(method, toSipUri, to,
                    from);
                req.Header.CallId = sipChannel.InviteSipRequest.Header.CallId;
                req.Header.From =
                    new SIPFromHeader(null, fromSipUri, sipChannel.InviteSipRequest.Header.From.FromTag);
                req.Header.To = new SIPToHeader(null, toSipUri, sipChannel.InviteSipResponse.Header.To.ToTag);
                req.Header.Contact = new List<SIPContactHeader>()
                {
                    new SIPContactHeader(null, fromSipUri),
                };
                req.Header.Contact[0].ContactName = null;
                req.Header.Allow = null;
                req.Header.UserAgent = ConstString.SIP_USERAGENT_STRING;
                sipChannel.InviteSipResponse.Header.CSeq++;
                req.Header.CSeq = sipChannel.InviteSipResponse.Header.CSeq;


                var nrt = new NeedReturnTask(Common.NeedResponseRequests)
                {
                    AutoResetEvent = evnt,
                    CallId = req.Header.CallId,
                    SipRequest = req,
                    Timeout = timeout,
                    SipDevice = tmpSipDevice,
                    SipChannel = sipChannel,
                    CommandType = CommandType.Unknown,
                };

                Common.NeedResponseRequests.TryAdd(req.Header.CallId, nrt);
                GCommon.Logger.Debug($"[{Common.LoggerHead}]->发送终止时实流请求->{req}");
                _sipTransport.SendRequestAsync(tmpSipDevice.RemoteEndPoint, req);
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
                try
                {
                    evnt.Set();
                }
                catch (Exception exex)
                {
                    ResponseStruct exrs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_AutoResetEventExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                        ExceptMessage = exex.Message,
                        ExceptStackTrace = exex.StackTrace
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                }
            }
        }

        /// <summary>
        /// 设备目录查询请求
        /// </summary>
        /// <param name="sipDeviceId"></param>
        public void DeviceCatalogQuery(SipDevice sipDevice, AutoResetEvent evnt, AutoResetEvent evnt2,
            out ResponseStruct rs,
            int timeout = 5000)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var tmpSipDevice = Common.SipDevices.FindLast(x => x.DeviceInfo!.DeviceID.Equals(sipDevice.DeviceId));
            if (tmpSipDevice != null)
            {
                SIPMethodsEnum method = SIPMethodsEnum.MESSAGE;
                string subject =
                    $"{Common.SipServerConfig.ServerSipDeviceId}:{0},{tmpSipDevice.DeviceId}:{new Random().Next(100, ushort.MaxValue)}";
                CatalogQuery catalogQuery = new CatalogQuery()
                {
                    CommandType = CommandType.Catalog,
                    DeviceID = tmpSipDevice.DeviceInfo.DeviceID,
                    SN = new Random().Next(1, ushort.MaxValue),
                };
                string xmlBody = CatalogQuery.Instance.Save<CatalogQuery>(catalogQuery);
                try
                {
                    Func<SipDevice, SIPMethodsEnum, string, string, string, CommandType, bool, AutoResetEvent,
                            AutoResetEvent, object, int, Task
                        >
                        request =
                            SendRequestViaSipDevice;
                    request(tmpSipDevice, method, ConstString.Application_MANSCDP, subject, xmlBody,
                        catalogQuery.CommandType, true, evnt, evnt2, null, timeout);
                }
                catch (AkStreamException ex)
                {
                    rs = ex.ResponseStruct;
                    try
                    {
                        evnt.Set();
                        evnt2.Set();
                    }
                    catch (Exception exex)
                    {
                        ResponseStruct exrs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AutoResetEventExcept,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AutoResetEventExcept],
                            ExceptMessage = exex.Message,
                            ExceptStackTrace = exex.StackTrace
                        };
                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->AutoResetEvent.Set异常->{JsonHelper.ToJson(exrs)}");
                    }
                }
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_DeviceNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_DeviceNotExists],
                };
                try
                {
                    evnt.Set();
                    evnt2.Set();
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
        }


        public void KickSipDevice(SipDevice sipDevice)
        {
            SipMsgProcess.DoKickSipDevice(sipDevice);
        }

        /// <summary>
        /// 停止Sip服务
        /// </summary>
        /// <param name="rs"></param>
        /// <exception cref="AkStreamException"></exception>
        public void Stop(out ResponseStruct rs)
        {
            try
            {
                switch (Common.SipServerConfig.GbVersion.Trim().ToUpper())
                {
                    case "GB-2016":
                        _sipTransport.SIPTransportRequestReceived -= SipMsgProcess.SipTransportRequestReceived;
                        _sipTransport.SIPTransportResponseReceived -= SipMsgProcess.SipTransportResponseReceived;
                        break;
                }

                _sipTransport.Shutdown();
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_StopExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_StopExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                throw new AkStreamException(rs);
            }
        }


        /// <summary>
        /// 启动Sip服务
        /// </summary>
        /// <returns></returns>
        public void Start(out ResponseStruct rs)
        {
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip服务ID->{Common.SipServerConfig.ServerSipDeviceId}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->本机IP地址->{Common.SipServerConfig.SipIpAddress}");
            if (Common.SipServerConfig.IpV6Enable)
            {
                GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->配置情况->本机IPV6地址->{Common.SipServerConfig.SipIpV6Address}");
            }

            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->启用IPV6->{Common.SipServerConfig.IpV6Enable}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip服务端口->{Common.SipServerConfig.SipPort}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip服务协议->{Common.SipServerConfig.MsgProtocol}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->GB28181协议版本->{Common.SipServerConfig.GbVersion}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip服务是否启用鉴权->{Common.SipServerConfig.Authentication}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip服务鉴权用户名->{Common.SipServerConfig.SipUsername}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip服务鉴权密码->{Common.SipServerConfig.SipPassword}");
            GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip服务域ID->{Common.SipServerConfig.Realm}");
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->配置情况->Sip服务心跳周期（秒）->{Common.SipServerConfig.KeepAliveInterval}");
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->配置情况->Sip服务允许心跳丢失次数->{Common.SipServerConfig.KeepAliveLostNumber}");
            try
            {
                GCommon.Logger.Info($"[{Common.LoggerHead}]->启动Sip服务.");

                //创建sip传输层
                _sipTransport =
                    new SIPTransport(false, Common.SipServerConfig.Encoding, Common.SipServerConfig.Encoding);

                // _sipTransport = new SIPTransport();

                // 创建ipv4 udp传输层
                if (string.IsNullOrEmpty(Common.SipServerConfig.ListenIp))
                {
                    _sipUdpIpV4Channel = new SIPUDPChannel(new IPEndPoint(IPAddress.Any,
                        Common.SipServerConfig.SipPort));
                }
                else
                {
                    _sipUdpIpV4Channel = new SIPUDPChannel(new IPEndPoint(
                        IPAddress.Parse(Common.SipServerConfig.ListenIp.Trim()),
                        Common.SipServerConfig.SipPort));
                }

                if (Common.SipServerConfig.MsgProtocol.Trim().ToUpper().Equals("TCP"))
                {
                    if (string.IsNullOrEmpty(Common.SipServerConfig.ListenIp))
                    {
                        _sipTcpIpV4Channel = new SIPTCPChannel(new IPEndPoint(IPAddress.Any,
                            Common.SipServerConfig.SipPort));
                    }
                    else
                    {
                        _sipTcpIpV4Channel = new SIPTCPChannel(new IPEndPoint(
                            IPAddress.Parse(Common.SipServerConfig.ListenIp.Trim()),
                            Common.SipServerConfig.SipPort));
                    }

                    _sipTransport.AddSIPChannel(_sipTcpIpV4Channel);
                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->监听端口成功,监听情况->{_sipTcpIpV4Channel.ListeningEndPoint.Address}:{_sipTcpIpV4Channel.ListeningEndPoint.Port}(TCP via IPV4)");
                }

                _sipTransport.AddSIPChannel(_sipUdpIpV4Channel);

                GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->监听端口成功,监听情况->{_sipUdpIpV4Channel.ListeningEndPoint.Address}:{_sipUdpIpV4Channel.ListeningEndPoint.Port}(UDP via IPV4)");

                // 创建ipv6 udp传输层

                if (Common.SipServerConfig.IpV6Enable)
                {
                    _sipUdpIpV6Channel = new SIPUDPChannel(new IPEndPoint(
                        IPAddress.IPv6Any, Common.SipServerConfig.SipPort));
                    if (Common.SipServerConfig.MsgProtocol.Trim().ToUpper().Equals("TCP"))
                    {
                        _sipTcpIpV6Channel = new SIPTCPChannel(new IPEndPoint(IPAddress.IPv6Any,
                            Common.SipServerConfig.SipPort));
                        _sipTransport.AddSIPChannel(_sipTcpIpV6Channel);
                        GCommon.Logger.Info(
                            $"[{Common.LoggerHead}]->监听端口成功,监听情况->{_sipTcpIpV6Channel.ListeningEndPoint.Address}:{_sipTcpIpV6Channel.ListeningEndPoint.Port}(TCP via IPV6)");
                    }

                    _sipTransport.AddSIPChannel(_sipUdpIpV6Channel);
                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->监听端口成功,监听情况->{_sipUdpIpV6Channel.ListeningEndPoint.Address}:{_sipUdpIpV6Channel.ListeningEndPoint.Port}(UDP via IPV6)");
                }


                switch (Common.SipServerConfig.GbVersion.Trim().ToUpper())
                {
                    case "GB-2016":
                        _sipTransport.SIPTransportRequestReceived += SipMsgProcess.SipTransportRequestReceived;
                        _sipTransport.SIPTransportResponseReceived += SipMsgProcess.SipTransportResponseReceived;
                        Task.Factory.StartNew(() => { SipMsgProcess.ProcessCatalogThread(); });
                        Task.Factory.StartNew(() => { SipMsgProcess.ProcessRecordInfoThread(); });
                        break;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_StartExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_StartExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                throw new AkStreamException(rs);
            }
        }
    }
}