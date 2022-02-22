using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using AKStreamKeeper.Misc;
using IniParser;
using IniParser.Model;
using LibCommon;
using LibLogger;

namespace AKStreamKeeper
{
    /// <summary>
    /// 流媒体服务器的进程实例
    /// </summary>
    [Serializable]
    public class MediaServerInstance
    {
        private static int _pid;
        private static bool _isSelfClose = false;

        private static AKStreamKeeperConfig _akStreamKeeperConfig;
        private static ProcessHelper _mediaServerProcessHelper =
            new ProcessHelper(p_StdOutputDataReceived, p_ErrOutputDataReceived, p_Process_Exited!);

        private string _binPath;
        private string _configPath;
        private string _mediaServerId;
        private Process _process;
        private string _secret;
        private string _workPath;
        private string _zlmFFMPEGCmd;
        private ushort _zlmHttpPort;
        private ushort _zlmHttpsPort;
        private uint _zlmRecordFileSec;
        private ushort _zlmRtmpPort;
        private ushort _zlmRtmpsPort;
        private ushort _zlmRtpProxyPort;
        private ushort _zlmRtspPort;
        private ushort _zlmRtspsPort;

        public  AKStreamKeeperConfig AkStreamKeeperConfig
        {
            get => _akStreamKeeperConfig;
            set => _akStreamKeeperConfig = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="binPath"></param>
        /// <param name="configPath"></param>
        public MediaServerInstance(string binPath, AKStreamKeeperConfig keeperConfig, string configPath = "")
        {
            _akStreamKeeperConfig = keeperConfig;
            _binPath = binPath;
            if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            {
                _workPath = Path.GetDirectoryName(binPath);
                _configPath = _workPath + "/config.ini";
            }
            else
            {
                _configPath = configPath;
            }

            ResponseStruct rs;
            try
            {
                var ret = GetConfig(out rs);
                if (!ret || !rs.Code.Equals(ErrorNumber.None))
                {
                    throw new AkStreamException(rs);
                }

                var ret2 = SetConfig(out rs);
                if (!ret2 || !rs.Code.Equals(ErrorNumber.None))
                {
                    throw new AkStreamException(rs);
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Other,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                throw new AkStreamException(rs);
            }
        }

        private bool _isRunning => CheckRunning();

        /// <summary>
        /// 可执行文件路径
        /// </summary>
        public string BinPath
        {
            get => _binPath;
            set => _binPath = value;
        }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string ConfigPath
        {
            get => _configPath;
            set => _configPath = value;
        }

        /// <summary>
        /// 鉴权密钥
        /// </summary>
        public string Secret
        {
            get => _secret;
            set => _secret = value;
        }

        /// <summary>
        /// 流媒体服务器id
        /// </summary>
        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 流媒体进程id
        /// </summary>
        public static int Pid
        {
            get => _pid;
            set => _pid = value;
        }

        public ushort ZlmRtpProxyPort
        {
            get => _zlmRtpProxyPort;
            set => _zlmRtpProxyPort = value;
        }

        /// <summary>
        /// http端口
        /// </summary>
        public ushort ZlmHttpPort
        {
            get => _zlmHttpPort;
            set => _zlmHttpPort = value;
        }

        /// <summary>
        /// https端口
        /// </summary>
        public ushort ZlmHttpsPort
        {
            get => _zlmHttpsPort;
            set => _zlmHttpsPort = value;
        }

        /// <summary>
        /// rtsp端口
        /// </summary>
        public ushort ZlmRtspPort
        {
            get => _zlmRtspPort;
            set => _zlmRtspPort = value;
        }

        /// <summary>
        /// rtmp端口
        /// </summary>
        public ushort ZlmRtmpPort
        {
            get => _zlmRtmpPort;
            set => _zlmRtmpPort = value;
        }

        /// <summary>
        /// rtsps端口
        /// </summary>
        public ushort ZlmRtspsPort
        {
            get => _zlmRtspsPort;
            set => _zlmRtspsPort = value;
        }

        /// <summary>
        /// rtmps端口
        /// </summary>
        public ushort ZlmRtmpsPort
        {
            get => _zlmRtmpsPort;
            set => _zlmRtmpsPort = value;
        }

        /// <summary>
        /// 文件录制时长（秒）
        /// </summary>
        public uint ZlmRecordFileSec
        {
            get => _zlmRecordFileSec;
            set => _zlmRecordFileSec = value;
        }

        /// <summary>
        /// ffmpeg的命令
        /// </summary>
        public string ZlmFFmpegCmd
        {
            get => _zlmFFMPEGCmd;
            set => _zlmFFMPEGCmd = value;
        }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
        }

        public static event Common.MediaServerKilled OnMediaKilled = null!;
        
        
        /// <summary>
        /// 修改一个ffmpeg模板
        /// </summary>
        /// <param name="tmplate"></param>
        /// <returns></returns>
        public  bool ModifyFFmpegTemplate(KeyValuePair<string, string> tmplate,out ResponseStruct rs)
        {
               rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
           
            if (!string.IsNullOrEmpty(_configPath) && File.Exists(_configPath))
            {
                var parser = new FileIniDataParser();
                try
                {
                    IniData data = parser.ReadFile(_configPath, Encoding.UTF8);
                    var ffmpeg_temp = data["ffmpeg_templete"]; //获取ffmpeg模板列表
                    bool found = false;
                    if (ffmpeg_temp != null)
                    {
                        foreach (var temp in ffmpeg_temp)
                        {
                            if (temp.KeyName.Trim().ToLower().Equals(tmplate.Key.Trim().ToLower()))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            data["ffmpeg_templete"][tmplate.Key] =
                               tmplate.Value;
                            parser.WriteFile(_configPath, data);
                            Reload();
                            return true;

                        }

                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.MediaServer_ObjectNotExists,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ObjectNotExists],
                         
                        };
                        return false;

                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_ObjectNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ObjectNotExists],
                         
                    };
                    return false;

                }
                catch(Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ReadIniFileExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ReadIniFileExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rs);
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_ConfigNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ConfigNotFound],
                 
            };
            throw new AkStreamException(rs);
        }
        /// <summary>
        /// 删除一个ffmpeg模板
        /// </summary>
        /// <param name="tmplateName"></param>
        /// <returns></returns>
        public  bool DelFFmpegTemplate(string tmplateName,out ResponseStruct rs)
        {
             rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
           
            if (!string.IsNullOrEmpty(_configPath) && File.Exists(_configPath))
            {
                var parser = new FileIniDataParser();
                try
                {
                    IniData data = parser.ReadFile(_configPath, Encoding.UTF8);
                    var ffmpeg_temp = data["ffmpeg_templete"]; //获取ffmpeg模板列表
                    bool found = false;
                    if (ffmpeg_temp != null)
                    {
                        foreach (var temp in ffmpeg_temp)
                        {
                            if (temp.KeyName.Trim().ToLower().Equals(tmplateName.ToLower()))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            ffmpeg_temp.RemoveKey(tmplateName);
                            parser.WriteFile(_configPath, data);
                            Reload();
                        }
                    }
                    return true;

                }
                catch(Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ReadIniFileExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ReadIniFileExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rs);
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_ConfigNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ConfigNotFound],
                 
            };
            throw new AkStreamException(rs);
        }

        /// <summary>
        /// 添加一个ffmpeg模板
        /// </summary>
        /// <param name="tmplate"></param>
        /// <returns></returns>
        public  bool AddFFmpegTemplate(KeyValuePair<string, string> tmplate,out ResponseStruct rs)
        {
           
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
           
            if (!string.IsNullOrEmpty(_configPath) && File.Exists(_configPath))
            {
                var parser = new FileIniDataParser();
                try
                {
                    IniData data = parser.ReadFile(_configPath, Encoding.UTF8);
                    var ffmpeg_temp = data["ffmpeg_templete"]; //获取ffmpeg模板列表
                    bool found = false;
                    if (ffmpeg_temp != null)
                    {
                        foreach (var temp in ffmpeg_temp)
                        {
                            if (temp.KeyName.Trim().ToLower().Equals(tmplate.Key.Trim().ToLower()))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.MediaServer_InputObjectAlredayExists,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InputObjectAlredayExists],
                            };
                            return false;
                        }
                        SectionData ff = new SectionData("ffmpeg_templete");
                        KeyData ffkey = new KeyData(tmplate.Key);
                        ffkey.Value = tmplate.Value;
                        ff.Keys.AddKey(ffkey);
                        data.Sections.Add(ff);
                        parser.WriteFile(_configPath, data);
                        Reload();
                        return true;
                    }
                    else
                    {
                        SectionData ff = new SectionData("ffmpeg_templete");
                        KeyData ffkey = new KeyData(tmplate.Key);
                        ffkey.Value = tmplate.Value;
                        ff.Keys.AddKey(ffkey);
                        data.Sections.Add(ff);
                        parser.WriteFile(_configPath, data);
                        Reload();
                        return true;
                    }
                   
                }
                catch(Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ReadIniFileExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ReadIniFileExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rs);
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_ConfigNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ConfigNotFound],
                 
            };
            throw new AkStreamException(rs);
        }

        /// <summary>
        /// 获取ffmpeg模板列表
        /// </summary>
        /// <returns></returns>
        public  List<KeyValuePair<string, string>> GetFFmpegTempleteList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(_configPath) && File.Exists(_configPath))
            {
                var parser = new FileIniDataParser();
                try
                {
                    IniData data = parser.ReadFile(_configPath, Encoding.UTF8);
                    var ffmpeg_temp = data["ffmpeg_templete"]; //获取ffmpeg模板列表
                    if (ffmpeg_temp != null && ffmpeg_temp.Count > 0)
                    {
                        foreach (var temp in ffmpeg_temp)
                        {
                            if (temp != null)
                            {
                                result.Add(new KeyValuePair<string, string>(temp.KeyName,temp.Value));
                            }
                        }
                    }
                    return result;
                }
                catch(Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ReadIniFileExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ReadIniFileExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rs);
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_ConfigNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ConfigNotFound],
                 
            };
            throw new AkStreamException(rs);
        }


        public bool SetConfig(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(_configPath) && File.Exists(_configPath))
            {
                var parser = new FileIniDataParser();
                try
                {
                    IniData data = parser.ReadFile(_configPath, Encoding.UTF8);
                    Uri AKStreamWebUri = new Uri(Common.AkStreamKeeperConfig.AkStreamWebRegisterUrl);
                    string h = AKStreamWebUri.Host.ToString();
                    string p = AKStreamWebUri.Port.ToString();

                    var ffmpeg_temp = data["ffmpeg_templete"]; //启用ffmpeg_templete
                    if (ffmpeg_temp == null)
                    {
                        SectionData ff = new SectionData("ffmpeg_templete");
                        KeyData ffkey = new KeyData("rtsp_tcp2flv");
                        ffkey.Value = $"%s -re -rtsp_transport tcp -i %s -vcodec copy -acodec copy -f flv -y  %s";
                        ff.Keys.AddKey(ffkey);
                        data.Sections.Add(ff);
                    }

                    var ffkey_temp = data["ffmpeg_templete"]["rtsp_tcp2flv"];
                    if (UtilsHelper.StringIsNullEx(ffkey_temp))
                    {
                        data["ffmpeg_templete"]["rtsp_tcp2flv"] =
                            $"%s -re -rtsp_transport tcp -i %s -vcodec copy -acodec copy -f flv -y  %s";
                    }

                    ffkey_temp = data["ffmpeg_templete"]["ffmpeg2flv"];
                    if (UtilsHelper.StringIsNullEx(ffkey_temp))
                    {
                        data["ffmpeg_templete"]["ffmpeg2flv"] =
                            $"%s -re  -i %s -vcodec copy -acodec copy -f flv -y  %s";
                    }

                    data["hook"].RemoveAllKeys();
                    if (Common.IsDebug)
                    {
                        data["api"]["apiDebug"] = "1";
                    }
                    else
                    {
                        data["api"]["apiDebug"] = "0";
                    }


                    data["hook"]["enable"] = "1";
                    data["hook"]["on_flow_report"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnFlowReport"; //流量统计
                    data["hook"]["on_http_access"] = "";
                    data["hook"]["on_play"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnPlay"; //有流被客户端播放时
                    data["hook"]["on_publish"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnPublish"; //有流发布时
                    data["hook"]["on_record_mp4"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnRecordMp4"; //当录制mp4完成时
                    data["hook"]["on_record_ts"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnRecordTs"; //当录制ts完成时
                    data["hook"]["on_rtsp_auth"] = "";
                    data["hook"]["on_rtsp_realm"] = "";
                    data["hook"]["on_shell_login"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnShellLogin"; //shell鉴权
                    data["hook"]["on_stream_changed"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnStreamChanged"; //流注册或注销时
                    data["hook"]["on_stream_none_reader"] =
                        $"http://{h}:{p}/MediaServer/WebHook/OnStreamNoneReader"; //流无人观看时
                    data["hook"]["on_stream_not_found"] = "";
                    data["hook"]["on_server_started"] = "";
                    data["hook"]["timeoutSec"] = "20"; //httpclient超时时间20秒
                    data["general"]["flowThreshold"] = "0"; //当用户超过1byte流量时，将触发on_flow_report的webhook(/WebHook/OnStop)
                    data["ffmpeg"]["bin"] = Common.AkStreamKeeperConfig.FFmpegPath;
                    data["ffmpeg"]["cmd"] = "%s -re -i %s -vcodec copy -acodec copy -f flv -y  %s";
                    data["ffmpeg"]["snap"] = "%s -i %s -y -f mjpeg -t 0.001 %s";
                    if (Common.AkStreamKeeperConfig.DisableShell == true)
                    {
                        data["shell"]["port"] = "0";
                    }
                    else
                    {
                        data["shell"]["port"] = "9000";
                    }

                    if (Common.AkStreamKeeperConfig.RecordSec != null && Common.AkStreamKeeperConfig.RecordSec > 0)
                    {
                        data["record"]["fileSecond"] = Common.AkStreamKeeperConfig.RecordSec.ToString();
                    }

                    parser.WriteFile(_configPath, data);
                    return true;
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_WriteIniFileExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_WriteIniFileExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rs);
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_ConfigNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ConfigNotFound],
            };
            return false;
        }

        /// <summary>
        /// 获取和检查流媒体服务器的一些参数
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        public bool GetConfig(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(_configPath) && File.Exists(_configPath))
            {
                var parser = new FileIniDataParser();

                try
                {
                    string[] fileIniStrings = File.ReadAllLines(_configPath);
                    for (int i = 0; i <= fileIniStrings.Length - 1; i++)
                    {
                        if (fileIniStrings[i].Trim().StartsWith('#') || fileIniStrings[i].Trim().StartsWith(';'))
                        {
                            fileIniStrings[i] = fileIniStrings[i].TrimStart('#');
                            fileIniStrings[i] = ";" + fileIniStrings[i];
                        }
                    }

                    File.WriteAllLines(_configPath, fileIniStrings);
                    IniData data = parser.ReadFile(_configPath, Encoding.UTF8);

                    #region 检查MediaServerId

                    var _tmpStr = data["general"]["mediaServerId"];
                    if (string.IsNullOrEmpty(_tmpStr) || _tmpStr.ToUpper().Equals("your_server_id"))
                    {
                        data["general"]["mediaServerId"] = UtilsHelper.generalGuid();
                        try
                        {
                            parser.WriteFile(_configPath, data);
                        }
                        catch (Exception ex)
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_WriteIniFileExcept,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_WriteIniFileExcept],
                                ExceptMessage = ex.Message,
                                ExceptStackTrace = ex.StackTrace,
                            };
                            throw new AkStreamException(rs);
                        }
                    }

                    _mediaServerId = data["general"]["mediaServerId"];
                    _tmpStr = "";
                    _tmpStr = data["api"]["secret"];
                    if (!string.IsNullOrEmpty(_tmpStr))
                    {
                        _secret = _tmpStr;
                    }
                    else
                    {
                        _secret = "";
                    }

                    #endregion

                    #region 检查httpPort

                    _tmpStr = "";
                    _tmpStr = data["http"]["port"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_ConfigNotReady,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                            ExceptMessage = "http.port=null，http端口不能为空",
                        };
                        return false;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmHttpPort = tmpUshort;
                        }
                        else
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_ConfigNotReady,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                                ExceptMessage = "http.port不是可接受端口，http端口设置异常",
                            };
                            return false;
                        }
                    }

                    #endregion

                    #region 检查https端口

                    _tmpStr = "";
                    _tmpStr = data["http"]["sslport"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        _zlmHttpsPort = 0;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmHttpsPort = tmpUshort;
                        }
                        else
                        {
                            _zlmHttpsPort = 0;
                        }
                    }

                    #endregion

                    #region 检查rtsp端口

                    _tmpStr = "";
                    _tmpStr = data["rtsp"]["port"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_ConfigNotReady,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                            ExceptMessage = "rtsp.port=null，rtsp端口不能为空",
                        };
                        return false;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmRtspPort = tmpUshort;
                        }
                        else
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_ConfigNotReady,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                                ExceptMessage = "rtsp.port不是可接受端口，rtsp端口设置异常",
                            };
                            return false;
                        }
                    }

                    #endregion

                    #region rtsps端口检查

                    _tmpStr = "";
                    _tmpStr = data["rtsp"]["sslport"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        _zlmRtspsPort = 0;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmRtspsPort = tmpUshort;
                        }
                        else
                        {
                            _zlmRtspsPort = 0;
                        }
                    }

                    #endregion

                    #region rtmp端口检查

                    _tmpStr = "";
                    _tmpStr = data["rtmp"]["port"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_ConfigNotReady,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                            ExceptMessage = "rtmp.port=null，rtmp端口不能为空",
                        };
                        return false;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmRtmpPort = tmpUshort;
                        }
                        else
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_ConfigNotReady,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                                ExceptMessage = "rtmp.port不是可接受端口，rtmp端口设置异常",
                            };
                            return false;
                        }
                    }

                    #endregion

                    #region rtmps端口检查

                    _tmpStr = "";
                    _tmpStr = data["rtmp"]["sslport"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        _zlmRtmpsPort = 0;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmRtmpsPort = tmpUshort;
                        }
                        else
                        {
                            _zlmRtmpsPort = 0;
                        }
                    }

                    #endregion

                    #region 检查rtpProxy端口

                    _tmpStr = "";
                    _tmpStr = data["rtp_proxy"]["port"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_ConfigNotReady,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                            ExceptMessage = "rtp.port=null，rtp端口不能为空",
                        };
                        return false;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmRtpProxyPort = tmpUshort;
                        }
                        else
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_ConfigNotReady,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                                ExceptMessage = "rtp.port不是可接受端口，rtp端口设置异常",
                            };
                            return false;
                        }
                    }

                    #endregion

                    #region 检查录制文件时长（秒）

                    _tmpStr = "";
                    _tmpStr = data["record"]["fileSecond"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_ConfigNotReady,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                            ExceptMessage = "record.fileSecond=null，fileSecond录制时长不能为空，建议120秒",
                        };
                        return false;
                    }
                    else
                    {
                        ushort tmpUshort;
                        if (ushort.TryParse(_tmpStr, out tmpUshort))
                        {
                            _zlmRecordFileSec = tmpUshort;
                        }
                        else
                        {
                            rs = new ResponseStruct()
                            {
                                Code = ErrorNumber.Sys_ConfigNotReady,
                                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ConfigNotReady],
                                ExceptMessage = "record.fileSecond不可接受，fileSecond录制时长不能为空，建议120秒",
                            };
                            return false;
                        }
                    }

                    #endregion

                    #region 获取ffmpeg命令

                    _tmpStr = "";
                    _tmpStr = data["ffmpeg"]["cmd"];
                    if (string.IsNullOrEmpty(_tmpStr))
                    {
                        _zlmFFMPEGCmd = "";
                    }
                    else
                    {
                        _zlmFFMPEGCmd = _tmpStr.Trim();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ReadIniFileExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ReadIniFileExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    throw new AkStreamException(rs);
                }

                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_ConfigNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ConfigNotFound],
            };
            return false;
        }

        /// <summary>
        /// 流媒体服务器是否正在运行
        /// </summary>
        /// <returns></returns>
        public bool CheckRunning()
        {
            if (_process != null && !_process.HasExited)
            {
                _pid = _process.Id;
                return true;
            }

            return false;
        }


        /// <summary>
        /// 重启流媒体服务
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int Restart()
        {
            try
            {
                Shutdown();
                Thread.Sleep(300);
                return Startup();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 配置热加载
        /// </summary>
        /// <returns></returns>
        public int Reload()
        {
            if (_isRunning)
            {
                var tmpPro = new ProcessHelper();
                tmpPro.RunProcess("/bin/bash",
                    $"-c 'killall -1 {Path.GetFileNameWithoutExtension(_process.StartInfo.FileName)}'", 1000, out _,
                    out _);
                 GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->重新加载流媒体服务器配置文件(热加载)->{_pid}");
                return _process.Id;
            }

            return -1;
        }

        /// <summary>
        /// 启动流媒体服务器
        /// </summary>
        /// <returns></returns>
        public int Startup()
        {
            if (_isRunning)
            {
                 GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->启动流媒体服务器(当前正在运行)->{_pid}");
                return _pid;
            }

            string binDir = Path.GetDirectoryName(_binPath);
            string configDir = Path.GetDirectoryName(_configPath);
            Process ret;
            if (binDir.Trim().Equals(configDir.Trim()))
            {
                if (_akStreamKeeperConfig != null && _akStreamKeeperConfig.UseSsl &&
                    !string.IsNullOrEmpty(_akStreamKeeperConfig.ZLMediakitSSLFilePath))
                {
                    ret = _mediaServerProcessHelper.RunProcess(_binPath, $"-s {_akStreamKeeperConfig.ZLMediakitSSLFilePath}");
                }
                else
                {
                    ret = _mediaServerProcessHelper.RunProcess(_binPath, "");
                }
            }
            else
            {
                if (_akStreamKeeperConfig != null && _akStreamKeeperConfig.UseSsl &&
                    !string.IsNullOrEmpty(_akStreamKeeperConfig.ZLMediakitSSLFilePath))
                {
                    ret = _mediaServerProcessHelper.RunProcess(_binPath, $"-c {_configPath} -s {_akStreamKeeperConfig.ZLMediakitSSLFilePath}");
                }
                else
                {
                    ret = _mediaServerProcessHelper.RunProcess(_binPath, $"-c {_configPath}");
                }
               
            }

            if (ret != null && !ret.HasExited)
            {
                _process = ret;
                _pid = _process.Id;
                _isSelfClose = false;
                 GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->启动流媒体服务器成功->{_pid}");
                return _process.Id;
            }

             GCommon.Logger.Error(
                $"[{Common.LoggerHead}]->启动流媒体服务器失败");
            return -1;
        }

        /// <summary>
        /// 结束流媒体服务器
        /// </summary>
        /// <returns></returns>
        public bool Shutdown()
        {
            _pid = -1;
            _isSelfClose = true;
            var r = _mediaServerProcessHelper.KillProcess(_process);
             GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->终止流媒体服务器运行->{r}");
            return r;
        }

        /// <summary>
        /// 获取流媒体服务器pid
        /// </summary>
        /// <returns></returns>
        public int GetPid()
        {
            if (!_process.HasExited)
            {
                _pid = _process.Id;
                return _process.Id;
            }

            _pid = -1;
            return -1;
        }

        public static void p_Process_Exited(object sender, EventArgs e)
        {
            _pid = -1;
            OnMediaKilled?.Invoke(_isSelfClose);
        }

        public static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                 GCommon.Logger.Debug(
                    $"[{Common.LoggerHead}]->[ZLMediaKit]->{e.Data}");
            }
        }

        public static void p_ErrOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                 GCommon.Logger.Error(
                    $"[{Common.LoggerHead}]->[ZLMediaKit]->{e.Data}");
            }
        }
    }
}