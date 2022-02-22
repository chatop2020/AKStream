using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.GB28181.Net.SDP;
using LibCommon.Structs.GB28181.Net.SIP;
using LibCommon.Structs.GB28181.Sys;
using LibCommon.Structs.GB28181.XML;
using LibLogger;
using SIPSorcery.SIP;

namespace LibGB28181SipClient
{
    /// <summary>
    /// GB28181客户端类
    /// </summary>
    public class SipClient
    {
        private SIPTransport _sipTransport = null;
        private uint _sn = 0;
        private ushort _cseq = 0;
        private string _registerCallId;
        private string _keepaliveCallId;
        private string _catalogCallId;
        private string _lastCallId;
        private DateTime _keepaliveSendDatetime;
        private bool _isRegister = false;
        private bool _wantAuthorization = false;
        private Thread _registerThread = null;
        private uint _keepaliveLostTimes = 0;
        private DateTime _registerDateTime;
        private DateTime _keeperaliveDateTime;
        private IPEndPoint _localIpEndPoint;
        private IPEndPoint _remoteIpEndPoint;
        private SIPResponse _oldSipResponse;
        private SIPRequest _oldSipRequest;
        private AutoResetEvent _pauseThread = new AutoResetEvent(false);
        private AutoResetEvent _catalogThread = new AutoResetEvent(false);
        public static event GCommon.InviteChannel OnInviteChannel = null!;
        public static event GCommon.DeInviteChannel OnDeInviteChannel = null!;


        /// <summary>
        /// 本地端点
        /// </summary>
        public IPEndPoint LocalIpEndPoint
        {
            get => _localIpEndPoint;
            set => _localIpEndPoint = value;
        }

        /// <summary>
        /// 服务器（远程）端点
        /// </summary>
        public IPEndPoint RemoteIpEndPoint
        {
            get => _remoteIpEndPoint;
            set => _remoteIpEndPoint = value;
        }

        /// <summary>
        /// 自增sn
        /// </summary>
        public uint Sn
        {
            get
            {
                _sn++;
                return _sn;
            }
        }

        /// <summary>
        /// 自增seq
        /// </summary>
        public ushort CSeq
        {
            get
            {
                _cseq++;
                return _cseq;
            }
        }

        /// <summary>
        /// 注册事件专用callid
        /// </summary>
        public string RegisterCallid
        {
            get => _registerCallId;
            set => _registerCallId = value;
        }

        /// <summary>
        /// 信令通道实例
        /// </summary>
        public SIPTransport SipClientInstance
        {
            get => _sipTransport;
            set => _sipTransport = value;
        }


        /// <summary>
        /// 创建注册事件的验证信令
        /// </summary>
        /// <returns></returns>
        private SIPRequest CreateAuthRegister()
        {
            var realm = _oldSipResponse.Header.AuthenticationHeaders[0].SIPDigest.Realm;
            var nonce = _oldSipResponse.Header.AuthenticationHeaders[0].SIPDigest.Nonce;
            var username = Common.SipClientConfig.SipUsername;
            var password = Common.SipClientConfig.SipPassword;
            SIPProtocolsEnum protocols = SIPProtocolsEnum.udp;
            var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                new SIPEndPoint(protocols, _localIpEndPoint));
            toSipUri.User = Common.SipClientConfig.SipServerDeviceId;
            SIPToHeader to = new SIPToHeader(null, toSipUri, null);
            var fromSipUri = new SIPURI(SIPSchemesEnum.sip, _localIpEndPoint.Address, _localIpEndPoint.Port);
            fromSipUri.User = Common.SipClientConfig.SipDeviceId;
            SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStreamClient");
            SIPRequest req = SIPRequest.GetRequest(SIPMethodsEnum.REGISTER, toSipUri, to,
                from,
                new SIPEndPoint(protocols,
                    _localIpEndPoint));
            req.Header.Allow = null;
            req.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromSipUri)
            };
            req.Header.Expires = 3600;
            req.Header.UserAgent = Common.SipUserAgent;
            req.Header.CallId = _registerCallId;
            RegisterCallid = req.Header.CallId;
            req.Header.CSeq = CSeq;
            var HA1 = UtilsHelper.Md5($"{username}:{realm}:{password}");
            var HA2 = UtilsHelper.Md5($"REGISTER:{fromSipUri}");
            var response = UtilsHelper.Md5($"{HA1}:{nonce}:{HA2}");
            req.Header.AuthenticationHeaders = new List<SIPAuthenticationHeader>();
            var authHeader = new SIPAuthenticationHeader(
                new SIPAuthorisationDigest(SIPAuthorisationHeadersEnum.WWWAuthenticate)
                {
                    Username = username,
                    Response = response,
                    Realm = realm,
                    Nonce = nonce,
                    URI = fromSipUri.ToString(),
                });
            req.Header.AuthenticationHeaders.Add(authHeader);
            return req;
        }

        /// <summary>
        /// 创建第一次注册信令
        /// </summary>
        /// <returns></returns>
        private SIPRequest CreateFirstRegister()
        {
            SIPProtocolsEnum protocols = SIPProtocolsEnum.udp;
            var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                new SIPEndPoint(protocols, _localIpEndPoint));
            toSipUri.User = Common.SipClientConfig.SipServerDeviceId;
            SIPToHeader to = new SIPToHeader(null, toSipUri, null);

            var fromSipUri = new SIPURI(SIPSchemesEnum.sip, _localIpEndPoint.Address, _localIpEndPoint.Port);
            fromSipUri.User = Common.SipClientConfig.SipDeviceId;
            SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStreamClient");
            SIPRequest req = SIPRequest.GetRequest(SIPMethodsEnum.REGISTER, toSipUri, to,
                from,
                new SIPEndPoint(protocols,
                    _localIpEndPoint));
            req.Header.Allow = null;
            req.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromSipUri)
            };
            req.Header.Expires = 3600;
            req.Header.UserAgent = Common.SipUserAgent;
            req.Header.CallId = CallProperties.CreateNewCallId();
            RegisterCallid = req.Header.CallId;
            req.Header.CSeq = CSeq;
            return req;
        }

        /// <summary>
        /// 创建心跳信令
        /// </summary>
        /// <returns></returns>
        private SIPRequest CreateKeepAlive()
        {
            SIPProtocolsEnum protocols = SIPProtocolsEnum.udp;
            var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                new SIPEndPoint(protocols, _localIpEndPoint));
            toSipUri.User = Common.SipClientConfig.SipServerDeviceId;
            SIPToHeader to = new SIPToHeader(null, toSipUri, null);

            var fromSipUri = new SIPURI(SIPSchemesEnum.sip, _localIpEndPoint.Address, _localIpEndPoint.Port);
            fromSipUri.User = Common.SipClientConfig.SipDeviceId;
            SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStreamClient");
            SIPRequest req = SIPRequest.GetRequest(SIPMethodsEnum.MESSAGE, toSipUri, to,
                from,
                new SIPEndPoint(protocols,
                    _localIpEndPoint));
            req.Header.Allow = null;
            req.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromSipUri)
            };
            req.Header.UserAgent = Common.SipUserAgent;
            req.Header.ContentType = "Application/MANSCDP+xml";
            req.Header.CallId = CallProperties.CreateNewCallId();
            _keepaliveCallId = req.Header.CallId;
            req.Header.CSeq = CSeq;
            var keepaliveBody = new KeepAlive();
            keepaliveBody.Status = "OK";
            keepaliveBody.CmdType = CommandType.Keepalive;
            keepaliveBody.SN = (int)Sn;
            keepaliveBody.DeviceID = Common.SipClientConfig.SipDeviceId;
            req.Body = KeepAlive.Instance.Save<KeepAlive>(keepaliveBody);
            return req;
        }

        /// <summary>
        /// 心跳保持
        /// </summary>
        private void KeeperAlive()
        {
            while ((DateTime.Now - _registerDateTime).TotalSeconds < Common.SipClientConfig.Expiry && _isRegister)
            {
                if ((DateTime.Now - _keeperaliveDateTime).TotalSeconds > Common.SipClientConfig.KeepAliveInterval + 2)
                {
                    _keepaliveLostTimes++;
                }

                if (_keepaliveLostTimes > Common.SipClientConfig.KeepAliveLostNumber)
                {
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->与Sip服务器的心跳丢失超过{Common.SipClientConfig.KeepAliveLostNumber}次->系统重新进入设备注册模式");
                    _isRegister = false;
                    _wantAuthorization = false;
                    _keepaliveCallId = "";
                    _pauseThread.Close();
                    _catalogThread.Close();
                    _catalogCallId = "";
                    _keepaliveLostTimes = 0;
                    _lastCallId = "";
                    _oldSipRequest = null;
                    _oldSipResponse = null;
                    _registerCallId = "";
                    _pauseThread.Reset();
                    _registerThread.Abort();
                    _registerThread = new Thread(Register);
                    _registerThread.Start();
                    break;
                }

                var req = CreateKeepAlive();
                SipClientInstance.SendRequestAsync(
                    new SIPEndPoint(SIPProtocolsEnum.udp, _remoteIpEndPoint.Address, _remoteIpEndPoint.Port),
                    req);
                _oldSipRequest = req;
                GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->发送心跳数据->{req.RemoteSIPEndPoint}->{req}");
                _keepaliveSendDatetime = DateTime.Now;
                Thread.Sleep(Common.SipClientConfig.KeepAliveInterval * 1000);
            }

            if ((DateTime.Now - _registerDateTime).TotalSeconds > Common.SipClientConfig.Expiry)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->超过注册有效期:{Common.SipClientConfig.Expiry}->系统重新进入设备注册模式");

                _isRegister = false;
                _wantAuthorization = false;
                _keepaliveCallId = "";
                _pauseThread.Close();
                _catalogThread.Close();
                _catalogCallId = "";
                _keepaliveLostTimes = 0;
                _lastCallId = "";
                _oldSipRequest = null;
                _oldSipResponse = null;
                _registerCallId = "";
                _pauseThread.Reset();
                _registerThread.Abort();
                _registerThread = new Thread(Register);
                _registerThread.Start();
            }
        }


        /// <summary>
        /// 注册保持 
        /// </summary>
        private void Register()
        {
            while (!_isRegister)
            {
                if (!_isRegister && !_wantAuthorization)
                {
                    var req = CreateFirstRegister();
                    _oldSipRequest = req;
                    SipClientInstance.SendRequestAsync(
                        new SIPEndPoint(SIPProtocolsEnum.udp, _remoteIpEndPoint.Address, _remoteIpEndPoint.Port),
                        req);
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->发送首次注册数据->{req.RemoteSIPEndPoint}->{req}");
                }
                else if (!_isRegister && _wantAuthorization)
                {
                    var req = CreateAuthRegister();
                    _oldSipRequest = req;
                    SipClientInstance.SendRequestAsync(
                        new SIPEndPoint(SIPProtocolsEnum.udp, _remoteIpEndPoint.Address, _remoteIpEndPoint.Port),
                        req);
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->发送验证注册数据->{req.RemoteSIPEndPoint}->{req}");
                }

                _pauseThread.WaitOne(Common.SipClientConfig.KeepAliveInterval * 1000);
            }
        }


        /// <summary>
        /// 发送回复确认
        /// </summary>
        /// <param name="sipRequest"></param>
        private async Task SendOkMessage(SIPRequest sipRequest)
        {
            SIPResponseStatusCodesEnum messaageResponse = SIPResponseStatusCodesEnum.Ok;
            SIPResponse okResponse = SIPResponse.GetResponse(sipRequest, messaageResponse, null);
            await _sipTransport.SendResponseAsync(okResponse);
            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->发送确认数据->{okResponse.RemoteSIPEndPoint}->{okResponse}");
        }


        /// <summary>
        /// 生成设备信息信令
        /// </summary>
        /// <returns></returns>
        private SIPRequest CreateDeviceInfo()
        {
            SIPProtocolsEnum protocols = SIPProtocolsEnum.udp;
            var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                new SIPEndPoint(protocols, _localIpEndPoint));
            toSipUri.User = Common.SipClientConfig.SipServerDeviceId;
            SIPToHeader to = new SIPToHeader(null, toSipUri, null);

            var fromSipUri = new SIPURI(SIPSchemesEnum.sip, _localIpEndPoint.Address, _localIpEndPoint.Port);
            fromSipUri.User = Common.SipClientConfig.SipDeviceId;
            SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStreamClient");
            SIPRequest req = SIPRequest.GetRequest(SIPMethodsEnum.MESSAGE, toSipUri, to,
                from,
                new SIPEndPoint(protocols,
                    _localIpEndPoint));
            req.Header.Allow = null;
            req.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromSipUri)
            };
            req.Header.UserAgent = Common.SipUserAgent;
            req.Header.ContentType = "Application/MANSCDP+xml";
            req.Header.CallId = CallProperties.CreateNewCallId();
            _lastCallId = req.Header.CallId;
            req.Header.CSeq = CSeq;
            var deviceInfoBody = new DeviceInfo();
            deviceInfoBody.CmdType = CommandType.DeviceInfo;
            deviceInfoBody.Manufacturer = "AKStream";
            deviceInfoBody.Firmware = "V1.0";
            deviceInfoBody.Model = "AKStreamWeb V1.0";
            deviceInfoBody.Result = "OK";
            deviceInfoBody.DeviceID = Common.SipClientConfig.SipDeviceId;
            deviceInfoBody.DeviceName = "AKStream SipClient";
            deviceInfoBody.SN = (int)Sn;
            ResponseStruct rs;
            var ret = WebApiHelper.ShareChannelSumCount(out rs);
            if (ret > -1 && rs.Code.Equals(ErrorNumber.None))
            {
                deviceInfoBody.Channel = ret;
            }

            req.Body = DeviceInfo.Instance.Save<DeviceInfo>(deviceInfoBody);
            return req;
        }

        /// <summary>
        /// 处理设备信息
        /// </summary>
        private async Task ProcessDeviceInfo()
        {
            var req = CreateDeviceInfo();
            await SipClientInstance.SendRequestAsync(
                new SIPEndPoint(SIPProtocolsEnum.udp, _remoteIpEndPoint.Address, _remoteIpEndPoint.Port),
                req);
            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->发送设备信息数据->{req.RemoteSIPEndPoint}->{req}");
            _oldSipRequest = req;
        }

        /// <summary>
        /// 生成设备状态信令
        /// </summary>
        /// <returns></returns>
        private SIPRequest CreateDeviceStatus()
        {
            SIPProtocolsEnum protocols = SIPProtocolsEnum.udp;
            var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                new SIPEndPoint(protocols, _localIpEndPoint));
            toSipUri.User = Common.SipClientConfig.SipServerDeviceId;
            SIPToHeader to = new SIPToHeader(null, toSipUri, null);

            var fromSipUri = new SIPURI(SIPSchemesEnum.sip, _localIpEndPoint.Address, _localIpEndPoint.Port);
            fromSipUri.User = Common.SipClientConfig.SipDeviceId;
            SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStreamClient");
            SIPRequest req = SIPRequest.GetRequest(SIPMethodsEnum.MESSAGE, toSipUri, to,
                from,
                new SIPEndPoint(protocols,
                    _localIpEndPoint));
            req.Header.Allow = null;
            req.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromSipUri)
            };
            req.Header.UserAgent = Common.SipUserAgent;
            req.Header.ContentType = "Application/MANSCDP+xml";
            req.Header.CallId = CallProperties.CreateNewCallId();
            _lastCallId = req.Header.CallId;
            req.Header.CSeq = CSeq;
            var deviceStatusBody = new DeviceStatus();
            deviceStatusBody.CmdType = CommandType.DeviceStatus;
            deviceStatusBody.Alarmstatus = new Alarmstatus();
            deviceStatusBody.Online = "ONLINE";
            deviceStatusBody.DeviceID = Common.SipClientConfig.SipDeviceId;
            deviceStatusBody.Result = "OK";
            deviceStatusBody.Status = "OK";
            deviceStatusBody.DeviceTime = DateTime.Now;
            deviceStatusBody.Record = "OFF";
            deviceStatusBody.SN = (int)Sn;
            req.Body = DeviceStatus.Instance.Save<DeviceStatus>(deviceStatusBody);
            return req;
        }

        /// <summary>
        /// 处理设备状态
        /// </summary>
        private async Task ProcessDeviceStatus()
        {
            var req = CreateDeviceStatus();
            await SipClientInstance.SendRequestAsync(
                new SIPEndPoint(SIPProtocolsEnum.udp, _remoteIpEndPoint.Address, _remoteIpEndPoint.Port),
                req);
            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->发送设备状态数据->{req.RemoteSIPEndPoint}->{req}");
            _oldSipRequest = req;
        }


       

        /// <summary>
        /// 生成设备目录信令
        /// </summary>
        /// <param name="tmpList"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private SIPRequest CreateCatalog(List<VideoChannel> tmpList, int total)
        {
            SIPProtocolsEnum protocols = SIPProtocolsEnum.udp;
            var toSipUri = new SIPURI(SIPSchemesEnum.sip,
                new SIPEndPoint(protocols, _localIpEndPoint));
            toSipUri.User = Common.SipClientConfig.SipServerDeviceId;
            SIPToHeader to = new SIPToHeader(null, toSipUri, null);

            var fromSipUri = new SIPURI(SIPSchemesEnum.sip, _localIpEndPoint.Address, _localIpEndPoint.Port);
            fromSipUri.User = Common.SipClientConfig.SipDeviceId;
            SIPFromHeader from = new SIPFromHeader(null, fromSipUri, "AKStreamClient");
            SIPRequest req = SIPRequest.GetRequest(SIPMethodsEnum.MESSAGE, toSipUri, to,
                from,
                new SIPEndPoint(protocols,
                    _localIpEndPoint));
            req.Header.Allow = null;
            req.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromSipUri)
            };
            req.Header.UserAgent = Common.SipUserAgent;
            req.Header.ContentType = "Application/MANSCDP+xml";
            req.Header.CallId =
                string.IsNullOrEmpty(_catalogCallId) ? CallProperties.CreateNewCallId() : _catalogCallId;
            _catalogCallId = req.Header.CallId;
            req.Header.CSeq = CSeq;
            var catalogBody = new Catalog();
            catalogBody.CmdType = CommandType.Catalog;
            catalogBody.SumNum = total;
            catalogBody.SN = (int)Sn;
            catalogBody.DeviceID = Common.SipClientConfig.SipDeviceId;
            catalogBody.DeviceList = new Catalog.DevList();
            foreach (var obj in tmpList)
            {
                var devItem = new Catalog.Item();
                devItem.Manufacturer = "AKStream";
                devItem.Name = obj.ChannelName;
                devItem.Model = "AKStream";
                devItem.Owner = "Owner";
                devItem.CivilCode = "CivilCode";
                devItem.IPAddress = Common.SipClientConfig.LocalIpAddress;
                devItem.Parental = 0;
                devItem.SafetyWay = 0;
                devItem.RegisterWay = 1;
                devItem.Status = DevStatus.ON;
                devItem.Secrecy = 0;
                devItem.DeviceID = obj.ShareDeviceId;
                catalogBody.DeviceList.Items.Add(devItem);
            }

            req.Body = Catalog.Instance.Save<Catalog>(catalogBody);
            return req;
        }

        /// <summary>
        /// 处理设备目录
        /// </summary>
        private async Task ProcessCatalog()
        {
            try
            {
                ResponseStruct rs;
                var shareList = WebApiHelper.GetShareChannelList(out rs);
                if (shareList != null && rs.Code.Equals(ErrorNumber.None))
                {
                    var listGroup = UtilsHelper.GetListGroup(shareList, 2);
                    foreach (var obj in listGroup)
                    {
                        var req = CreateCatalog(obj, shareList.Count);
                        await SipClientInstance.SendRequestAsync(
                            new SIPEndPoint(SIPProtocolsEnum.udp, _remoteIpEndPoint.Address, _remoteIpEndPoint.Port),
                            req);
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->发送目录查询结果数据->{req.RemoteSIPEndPoint}->{req}");
                        _oldSipRequest = req;
                        _catalogThread.WaitOne(5000);
                    }

                    _catalogCallId = ""; //这里要置空
                }
            }
            catch
            {
            }
        }


        private ShareInviteInfo GetDeInviteShareInfo(SIPRequest req)
        {
            return null;
        }

        /// <summary>
        /// 获取流共享信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private bool GetShareInfo(SIPRequest req, out ShareInviteInfo info)
        {
            info = null;
            var sdpBody = req.Body;

            try
            {
                string mediaip = "";
                ushort mediaport = 0;
                string ssrc = "";
                string channelid =
                    req.Header.Subject.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[
                        0];
                channelid = channelid.Substring(0, channelid.IndexOf(':'));
                Console.WriteLine(channelid);

                string[] sdpBodys = sdpBody.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                if (sdpBodys.Length == 0)
                {
                    sdpBodys = sdpBody.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                }

                if (sdpBodys.Length == 0)
                {
                    sdpBodys = sdpBody.Split("\r", StringSplitOptions.RemoveEmptyEntries);
                }

                foreach (var line in sdpBodys)
                {
                    if (line.Trim().ToLower().StartsWith("o="))
                    {
                        var tmp = line.ToLower().Split("ip4", StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length == 2)
                        {
                            mediaip = tmp[1];
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (line.Trim().ToLower().StartsWith("m=video"))
                    {
                        mediaport = ushort.Parse(UtilsHelper.GetValue(line.ToLower(), "m\\=video", "rtp").Trim());
                    }

                    if (line.Trim().ToLower().StartsWith("y="))
                    {
                        var tmp2 = line.Split("=", StringSplitOptions.RemoveEmptyEntries);
                        if (tmp2.Length == 2)
                        {
                            ssrc = tmp2[1];
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                ResponseStruct rs;
                var shareList = WebApiHelper.GetShareChannelList(out rs);
                if (rs.Code.Equals(ErrorNumber.None) && shareList != null)
                {
                    var obj = shareList.FindLast(x =>
                        x.ShareDeviceId.Equals(channelid));
                    if (obj != null)
                    {
                        info = new ShareInviteInfo()
                        {
                            ChannelId = channelid.Trim(),
                            RemoteIpAddress = mediaip.Trim(),
                            RemotePort = mediaport,
                            Ssrc = ssrc.Trim(),
                            CallId = req.Header.CallId.Trim(),
                            Cseq = req.Header.CSeq,
                            FromTag = req.Header.From.FromTag,
                            ToTag = req.Header.To.ToTag,
                            MediaServerId = obj.MediaServerId.Trim(),
                            Stream = obj.MainId.Trim(),
                            App = obj.App.Trim(),
                            Vhost = obj.Vhost.Trim(),
                            Is_Udp = true,
                        };

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->获取sdp协商信息成功->{JsonHelper.ToJson(info)}");
                        return true;
                    }
                }
                else
                {
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取共享流列表失败->{JsonHelper.ToJson(rs)}");
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


        private bool CreateSdp(SIPRequest reqold, ref ShareInviteInfo info, out string sdpout)
        {
            sdpout = "";
            var from = reqold.Header.From;
            var to = reqold.Header.To;
            string callId = reqold.Header.CallId;
            SIPRequest req = SIPRequest.GetRequest(SIPMethodsEnum.INVITE, reqold.Header.To.ToURI,
                new SIPToHeader(to.ToName, to.ToURI, to.ToTag),
                new SIPFromHeader("", from.FromURI, from.FromTag));
            req.Header.Contact = new List<SIPContactHeader>()
                { new SIPContactHeader(reqold.Header.From.FromName, reqold.Header.From.FromURI) };
            req.Header.UserAgent = ConstString.SIP_USERAGENT_STRING;
            req.Header.Allow = null;
            req.Header.Vias = reqold.Header.Vias;
            req.Header.CallId = callId;
            req.Header.CSeq = reqold.Header.CSeq;
            var sdpConn = new SDPConnectionInformation(Common.SipClientConfig.LocalIpAddress);
            var sdp = new SDP()
            {
                Version = 0,
                SessionId = "0",
                Username = Common.SipClientConfig.SipUsername,
                SessionName = CommandType.Play.ToString(),
                Connection = sdpConn,
                Timing = "0 0",
                Address = Common.SipClientConfig.LocalIpAddress,
            };

            var psFormat = new SDPMediaFormat(SDPMediaFormatsEnum.PS)
            {
                IsStandardAttribute = false,
            };
            var h264Format = new SDPMediaFormat(SDPMediaFormatsEnum.H264)
            {
                IsStandardAttribute = false,
            };

            ResponseStruct rs;
            var rtpPort = WebApiHelper.GuessAnRtpPortForSender(info.MediaServerId, out rs);
            if (rs.Code.Equals(ErrorNumber.None) && rtpPort > 0)
            {
                var media = new SDPMediaAnnouncement()
                {
                    Media = SDPMediaTypesEnum.video,
                    Port = rtpPort,
                };
                info.LocalRtpPort = rtpPort;
                media.MediaFormats.Add(psFormat);
                media.MediaFormats.Add(h264Format);
                media.AddExtra("a=sendonly");
                media.Transport = "RTP/AVP";
                media.AddFormatParameterAttribute(psFormat.FormatID, psFormat.Name);
                media.AddFormatParameterAttribute(h264Format.FormatID, h264Format.Name);
                media.AddExtra($"a=username:{Common.SipClientConfig.SipUsername}");
                media.AddExtra($"a=password:{Common.SipClientConfig.SipPassword}");
                media.AddExtra($"y={info.Ssrc}");
                media.AddExtra("f=");
                sdp.Media.Add(media);
                sdpout = sdp.ToString();
                return true;
            }

            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->申请rtp(发送)端口失败->{JsonHelper.ToJson(rs)}");
            return false;
        }

    

        /// <summary>
        /// 创建invite协商结果
        /// </summary>
        /// <param name="oldreq"></param>
        /// <param name="sdp"></param>
        /// <returns></returns>
        private SIPResponse CreateInviteResponse(SIPRequest oldreq, string sdp)
        {
           var res= SIPResponse.GetResponse(oldreq, SIPResponseStatusCodesEnum.Ok, null);
           res.Header.UserAgent = Common.SipUserAgent;
           res.Header.ContentType = "Application/MANSCDP+xml";
           res.Header.CallId = oldreq.Header.CallId;
           res.Header.To.ToTag = UtilsHelper.CreateNewCSeq().ToString();
           _catalogCallId = res.Header.CallId;
           res.Header.CSeq = oldreq.Header.CSeq;
           res.Header.CSeqMethod = SIPMethodsEnum.INVITE;
           res.Body = sdp;

           return res;
        }
       

        /// <summary>
        /// 处理来自远端的请求
        /// </summary>
        /// <param name="localSipChannel"></param>
        /// <param name="localSIPEndPoint"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="sipRequest"></param>
        private async Task RecvSipMessageOfRequest(SIPChannel localSipChannel, SIPEndPoint localSIPEndPoint,
            SIPEndPoint remoteEndPoint,
            SIPRequest sipRequest)
        {
            ResponseStruct rs = null;
            bool? retok = false;
            var method = sipRequest.Header.CSeqMethod;
            switch (method)
            {
                case SIPMethodsEnum.BYE:
                    ShareInviteInfo info = null;
                    var fromTag = sipRequest.Header.From.FromTag.Trim();
                    var toTag = sipRequest.Header.To.ToTag.Trim();
                    var callid = sipRequest.Header.CallId.Trim();
                    SIPResponse byeResponse =
                        SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                    await SipClientInstance.SendResponseAsync(byeResponse);
                    retok = OnDeInviteChannel?.Invoke(fromTag, toTag, callid, out rs, out info);
                    if (retok == true)
                    {
                       
                        GCommon.Logger.Info(
                            $"[{Common.LoggerHead}]->终止共享推流成功->{sipRequest.RemoteSIPEndPoint}->{JsonHelper.ToJson(info)}");
                        
                        var b=WebApiHelper.ReleaseRtpPortForSender(info.MediaServerId, info.LocalRtpPort,out rs);
                       
                        if (!b || !rs.Code.Equals(ErrorNumber.None))
                        {
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->Rtp(发送)端口释放失败->{JsonHelper.ToJson(info.LocalRtpPort)}");

                        }
                        else
                        {
                            GCommon.Logger.Info(
                                $"[{Common.LoggerHead}]->Rtp(发送)端口释放成功->{JsonHelper.ToJson(info.LocalRtpPort)}");
                        }
                    }
                    else
                    {
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->终止共享推流失败->{sipRequest.RemoteSIPEndPoint}->{JsonHelper.ToJson(info)}->{JsonHelper.ToJson(rs)}");
                    }

                    break;
                case SIPMethodsEnum.INVITE:
                
                    ShareInviteInfo shareinfo = null;
                    var shareinfook = GetShareInfo(sipRequest, out shareinfo);
                    if (shareinfook && shareinfo != null)
                    {
                        string sdp = "";
                        var sdpok = CreateSdp(sipRequest, ref shareinfo, out sdp);
                        if (sdpok && !string.IsNullOrEmpty(sdp))
                        {
                            var response = CreateInviteResponse(sipRequest, sdp);
                            shareinfo.ToTag = response.Header.To.ToTag;
                            await SipClientInstance.SendResponseAsync(response);
                            retok = OnInviteChannel?.Invoke(shareinfo, out rs);
                            if (retok == true && rs.Code.Equals(ErrorNumber.None))
                            {
                                GCommon.Logger.Info(
                                    $"[{Common.LoggerHead}]->共享推流成功->{sipRequest.RemoteSIPEndPoint}->{JsonHelper.ToJson(shareinfo)}");
                            }
                            else
                            {
                                GCommon.Logger.Warn(
                                    $"[{Common.LoggerHead}]->共享推流失败->{sipRequest.RemoteSIPEndPoint}->{JsonHelper.ToJson(shareinfo)}->{JsonHelper.ToJson(rs)}");
                            }
                        }
                        else
                        {
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->共享推流失败->{sipRequest.RemoteSIPEndPoint}->{JsonHelper.ToJson(shareinfo)}->{JsonHelper.ToJson(rs)}");
                        }
                    }
                    else
                    {
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->共享推流失败->{sipRequest.RemoteSIPEndPoint}->{JsonHelper.ToJson(shareinfo)}");
                    }


                    break;
                case SIPMethodsEnum.MESSAGE:
                    if (sipRequest.Header.ContentType.Equals(ConstString.Application_MANSCDP))
                    {
                        XElement bodyXml = XElement.Parse(sipRequest.Body);
                        string cmdType = bodyXml.Element("CmdType")?.Value.ToUpper()!;
                        switch (cmdType)
                        {
                            case "DEVICEINFO":
                                //查询设备信息
                                GCommon.Logger.Debug(
                                    $"[{Common.LoggerHead}]->收到设备信息查询信令->{sipRequest.RemoteSIPEndPoint}->{sipRequest}");
                                await SendOkMessage(sipRequest);
                                await ProcessDeviceInfo();
                                break;
                            case "DEVICESTATUS":
                                //查询设备状态
                                GCommon.Logger.Debug(
                                    $"[{Common.LoggerHead}]->收到设备状态查询信令->{sipRequest.RemoteSIPEndPoint}->{sipRequest}");
                                await SendOkMessage(sipRequest);
                                await ProcessDeviceStatus();
                                break;
                            case "CATALOG":
                                GCommon.Logger.Debug(
                                    $"[{Common.LoggerHead}]->收到设备目录查询信令->{sipRequest.RemoteSIPEndPoint}->{sipRequest}");
                                await SendOkMessage(sipRequest);
                                Task.Run(() => { ProcessCatalog(); }); //抛线程出去处理

                                break;
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// 处理来自远端的回复
        /// </summary>
        /// <param name="localSipChannel"></param>
        /// <param name="localSIPEndPoint"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="sipResponse"></param>
        /// <returns></returns>
        private Task RecvSipMessageOfResponse(SIPChannel localSipChannel, SIPEndPoint localSIPEndPoint,
            SIPEndPoint remoteEndPoint,
            SIPResponse sipResponse)
        {
            
            var method = sipResponse.Header.CSeqMethod;
            var status = sipResponse.Status;
            switch (method)
            {
                case SIPMethodsEnum.MESSAGE:

                    if (sipResponse.Header.CallId.Equals(_keepaliveCallId))
                    {
                        if (status == SIPResponseStatusCodesEnum.Ok)
                        {
                            GCommon.Logger.Debug(
                                $"[{Common.LoggerHead}]->收到设备心跳确认回复->{sipResponse.RemoteSIPEndPoint}->{sipResponse}");
                            _keeperaliveDateTime = DateTime.Now;
                        }
                        else if (status == SIPResponseStatusCodesEnum.BadRequest)
                        {
                            GCommon.Logger.Debug(
                                $"[{Common.LoggerHead}]->收到设备心跳异常回复->{sipResponse.RemoteSIPEndPoint}->{sipResponse}");
                            _isRegister = false;
                            _wantAuthorization = false;
                            _keepaliveCallId = "";
                            _pauseThread.Close();
                            _catalogThread.Close();
                            _catalogCallId = "";
                            _keepaliveLostTimes = 0;
                            _lastCallId = "";
                            _oldSipRequest = null;
                            _oldSipResponse = null;
                            _registerCallId = "";

                            _pauseThread.Reset();
                            _registerThread.Abort();
                            _registerThread = new Thread(Register);
                            _registerThread.Start();
                        }
                    }

                    if (sipResponse.Header.CallId.Equals(_lastCallId))
                    {
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->收到设备信息确认回复->{sipResponse.RemoteSIPEndPoint}->{sipResponse}");
                    }

                    if (sipResponse.Header.CallId.Equals(_catalogCallId))
                    {
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->收到设备目录确认回复->{sipResponse.RemoteSIPEndPoint}->{sipResponse}");

                        _catalogThread.Set();
                    }

                    break;
                case SIPMethodsEnum.REGISTER:

                    if (status == SIPResponseStatusCodesEnum.Unauthorised)
                    {
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->收到要求注册验证回复->{sipResponse.RemoteSIPEndPoint}->{sipResponse}");

                        _wantAuthorization = true;
                        _oldSipResponse = sipResponse;
                        _registerCallId = _oldSipResponse.Header.CallId;
                        _pauseThread.Set();
                    }
                    else if (status == SIPResponseStatusCodesEnum.Ok)
                    {
                        if (sipResponse.Header.CallId.Equals(_registerCallId))
                        {
                            GCommon.Logger.Debug(
                                $"[{Common.LoggerHead}]->收到注册完成回复->{sipResponse.RemoteSIPEndPoint}->{sipResponse}");

                            _wantAuthorization = false;
                            _isRegister = true;
                            _oldSipResponse = sipResponse;
                            _registerDateTime = DateTime.Now;
                            Common.SipClientConfig.Expiry = (ushort)sipResponse.Header.Expires;
                            _pauseThread.Close();
                            new Thread(new ThreadStart(delegate
                            {
                                try
                                {
                                    KeeperAlive();
                                }
                                catch
                                {
                                }
                            })).Start();
                        }
                    }

                    break;
            }

            return null;
        }

        /// <summary>
        /// 类构造
        /// </summary>
        /// <exception cref="AkStreamException"></exception>
        public SipClient(string outConfigPath="")
        {
            ResponseStruct rs;
            if (!string.IsNullOrEmpty(outConfigPath))
            {
                Common.SipClientConfigPath = outConfigPath + "SipClientConfig.json";
            }
            var ret = Common.ReadSipClientConfig(out rs);
            if (ret < 0 || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Error($"[{Common.LoggerHead}]->加载配置文件失败->{Common.SipClientConfigPath}");
                throw new AkStreamException(rs);
            }


            Common.SipClient = this;
            GCommon.Logger.Info($"[{Common.LoggerHead}]->加载配置文件成功->{Common.SipClientConfigPath}");

            try
            {
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端本地IP地址->{Common.SipClientConfig.LocalIpAddress}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端本地端口->{Common.SipClientConfig.LocalPort}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->远程Sip服务器IP地址->{Common.SipClientConfig.SipServerIpAddress}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->远程Sip服务器端口->{Common.SipClientConfig.SipServerPort}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->远程Sip服务器设备ID->{Common.SipClientConfig.SipServerDeviceId}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端设备ID->{Common.SipClientConfig.SipDeviceId}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端域(REALM)->{Common.SipClientConfig.Realm}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端注册有效时间->{Common.SipClientConfig.Expiry}秒");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端用户名->{Common.SipClientConfig.SipUsername}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端密码->{Common.SipClientConfig.SipPassword}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端心跳间隔周期->{Common.SipClientConfig.KeepAliveInterval}秒/次");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端心跳丢失多少次后离线->{Common.SipClientConfig.KeepAliveLostNumber}次后设备离线");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端字符集->{Common.SipClientConfig.EncodingType}");
                GCommon.Logger.Info($"[{Common.LoggerHead}]->配置情况->Sip客户端与AKStreamWeb通讯地址->{Common.SipClientConfig.AkstreamWebHttpUrl}");
                _sipTransport = new SIPTransport();
                _sipTransport.SIPTransportResponseReceived += RecvSipMessageOfResponse;
                _sipTransport.SIPTransportRequestReceived += RecvSipMessageOfRequest;
                _localIpEndPoint = new IPEndPoint(IPAddress.Parse(Common.SipClientConfig.LocalIpAddress),
                    Common.SipClientConfig.LocalPort);
                _remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(Common.SipClientConfig.SipServerIpAddress),
                    Common.SipClientConfig.SipServerPort);

                _sipTransport.AddSIPChannel(new SIPUDPChannel(
                    IPAddress.Any, Common.SipClientConfig.LocalPort));
                _registerThread = new Thread(Register);
                _registerThread.Start();
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sip_SipClient_InitExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SipClient_InitExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Error($"[{Common.LoggerHead}]->初始化SipClient异常->{JsonHelper.ToJson(rs)}");
            }
        }
    }
}