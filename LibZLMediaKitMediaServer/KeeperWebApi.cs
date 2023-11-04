using System;
using System.Collections.Generic;
using System.Web;
using LibCommon;
using LibCommon.Structs.WebRequest.AKStreamKeeper;
using LibCommon.Structs.WebResponse.AKStreamKeeper;

namespace LibZLMediaKitMediaServer
{
    public class KeeperWebApi
    {
        private string _accessKey;
        private string _baseUrl;
        private int _httpClientTimeout;
        private string _ipAddress;
        private ushort _webApiPort;

        public KeeperWebApi(string ipAddress, ushort webApiPort, string accessKey, int httpClientTimeoutSec = 5)
        {
            _ipAddress = ipAddress;
            _webApiPort = webApiPort;
            _accessKey = accessKey;
            _baseUrl = $"http://{_ipAddress}:{_webApiPort}";
            _httpClientTimeout = httpClientTimeoutSec * 1000;
        }

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        public ushort WebApiPort
        {
            get => _webApiPort;
            set => _webApiPort = value;
        }

        public string AccessKey
        {
            get => _accessKey;
            set => _accessKey = value;
        }

        public int HttpClientTimeout
        {
            get => _httpClientTimeout;
            set => _httpClientTimeout = value;
        }


        /// <summary>
        /// 检查磁盘是否可写
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool CheckDiskWriteable(string dirPath, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/CheckDiskWriteable?dirPath=" + HttpUtility.UrlEncode(dirPath);
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// 修改ffmpeg模板
        /// </summary>
        /// <param name="tmplate"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool ModifyFFmpegTemplate(KeyValuePair<string, string> tmplate, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/ModifyFFmpegTemplate?templateName=" + tmplate.Key + "&templateValue=" +
                         tmplate.Value;
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// 获取ffmpeg模板列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetFFmpegTemplateList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/GetFFmpegTemplateList";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    var reslist = JsonHelper.FromJson<List<KeyValuePair<string, string>>>(httpRet);
                    if (reslist != null)
                    {
                        return reslist;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }

        /// <summary>
        /// 删除ffmpeg模板
        /// </summary>
        /// <param name="tmplate"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool DelFFmpegTemplate(string tmplateName, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/DelFFmpegTemplate?templateName=" + tmplateName;
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }


        /// <summary>
        /// 添加ffmpeg模板
        /// </summary>
        /// <param name="tmplate"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool AddFFmpegTemplate(KeyValuePair<string, string> tmplate, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/AddFFmpegTemplate?templateName=" + tmplate.Key + "&templateValue=" +
                         tmplate.Value;
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// 获取AKStreamKeeper的版本标识
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public string GetAKStreamKeeperVersion(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/GetAKStreamKeeperVersion";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    return httpRet;
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }


        /// <summary>
        /// 获取运行状态
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResKeeperCheckMediaServerRunning CheckMediaServerRunning(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/CheckMediaServerRunning";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    int pid = 0;
                    var ret = int.TryParse(httpRet, out pid);
                    if (ret && pid > 0)
                    {
                        var result = new ResKeeperCheckMediaServerRunning()
                        {
                            IsRunning = true,
                            Pid = pid,
                        };
                        return result;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }


        /// <summary>
        /// 释放被使用过的rtp端口，以防止段时间内同样的端口被重复使用
        /// </summary>
        /// <param name="port"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool ReleaseRtpPort(ushort port, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/ReleaseRtpPort?port={port}";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// 释放被使用过的rtp(发送)端口，以防止段时间内同样的端口被重复使用
        /// </summary>
        /// <param name="port"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool ReleaseRtpPortForSender(ushort port, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/ReleaseRtpPortForSender?port={port}";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// 热加载配置文件
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool ReloadMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/ReloadMediaServer";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }


        /// <summary>
        /// 重启流媒体服务器
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResKeeperRestartMediaServer RestartMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/RestartMediaServer";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    int pid = 0;
                    var ret = int.TryParse(httpRet, out pid);
                    if (ret && pid > 0)
                    {
                        var result = new ResKeeperRestartMediaServer()
                        {
                            IsRunning = true,
                            Pid = pid,
                        };
                        return result;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }


        /// <summary>
        /// 关闭流媒体服务器
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool ShutdownMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/ShutdownMediaServer";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }


        /// <summary>
        /// 启动流媒体服务器
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResKeeperStartMediaServer StartMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/StartMediaServer";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    int pid = 0;
                    var ret = int.TryParse(httpRet, out pid);
                    if (ret && pid > 0)
                    {
                        var result = new ResKeeperStartMediaServer()
                        {
                            IsRunning = true,
                            Pid = pid,
                        };
                        return result;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }

        /// <summary>
        /// 清理空目录
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool CleanUpEmptyDir(out ResponseStruct rs, string? filePath = "")
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/CleanUpEmptyDir";
            url += !string.IsNullOrEmpty(filePath) ? "?filePath=" + filePath : "";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);

                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }


        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="req"></param>
        /// <returns>未能正常删除的文件列表</returns>
        public ResKeeperDeleteFileList DeleteFileList(out ResponseStruct rs, List<string> req)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string url = $"{_baseUrl}/ApiService/DeleteFileList";
            try
            {
                string reqData = JsonHelper.ToJson(req);
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);

                var httpRet = NetHelper.HttpPostRequest(url, headers, reqData, "utf-8", _httpClientTimeout * 12);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    var resKeeperDeleteFileList = JsonHelper.FromJson<ResKeeperDeleteFileList>(httpRet);
                    if (resKeeperDeleteFileList != null)
                    {
                        return resKeeperDeleteFileList;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }


        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool FileExists(out ResponseStruct rs, string filePath)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/FileExists?filePath={filePath}";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// Keeper的健康情况
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool KeeperHealth(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/WebApiHealth";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    if (httpRet.Trim().ToUpper().Equals("OK"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool DeleteFile(out ResponseStruct rs, string filePath)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/DeleteFile?filePath={filePath}";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return false;
                    }

                    bool isOk = false;
                    var ret = bool.TryParse(httpRet, out isOk);
                    if (ret)
                    {
                        return isOk;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return false;
        }

        /// <summary>
        /// 获取一个可用的rtp端口（偶数端口）
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public ushort GuessAnRtpPort(out ResponseStruct rs, ushort? min = 0, ushort? max = 0)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ApiService/GuessAnRtpPort";
            url += (min != null && min > 0) ? "?min=" + min : "";
            if (url.Contains('?'))
            {
                url += (max != null && max > 0) ? "&max=" + max : "";
            }
            else
            {
                url += (max != null && max > 0) ? "?max=" + max : "";
            }

            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return 0;
                    }

                    ushort port = 0;
                    var ret = ushort.TryParse(httpRet, out port);
                    if (ret)
                    {
                        return port;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return 0;
        }


        /// <summary>
        /// 获取一个可用的rtp(发送)端口（偶数端口）
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public ushort GuessAnRtpPortForSender(out ResponseStruct rs, ushort? min = 0, ushort? max = 0)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string url = $"{_baseUrl}/ApiService/GuessAnRtpPortForSender";
            url += (min != null && min > 0) ? "?min=" + min : "";
            if (url.Contains('?'))
            {
                url += (max != null && max > 0) ? "&max=" + max : "";
            }
            else
            {
                url += (max != null && max > 0) ? "?max=" + max : "";
            }

            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return 0;
                    }

                    ushort port = 0;
                    var ret = ushort.TryParse(httpRet, out port);
                    if (ret)
                    {
                        return port;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return 0;
        }


        /// <summary>
        /// 获取裁剪合并任务积压列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResKeeperCutMergeTaskStatusResponseList GetBacklogTaskList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/CutMergeService/GetBacklogTaskList";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    var reslist = JsonHelper.FromJson<ResKeeperCutMergeTaskStatusResponseList>(httpRet);
                    if (reslist != null)
                    {
                        return reslist;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }


        /// <summary>
        /// 获取裁剪合并任务状态
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public ResKeeperCutMergeTaskStatusResponse GetMergeTaskStatus(out ResponseStruct rs, string taskId)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/CutMergeService/GetMergeTaskStatus?taskId={taskId}";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);
                var httpRet = NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    var resCutMergeTaskStatusResponse =
                        JsonHelper.FromJson<ResKeeperCutMergeTaskStatusResponse>(httpRet);
                    if (resCutMergeTaskStatusResponse != null)
                    {
                        return resCutMergeTaskStatusResponse;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }


        /// <summary>
        /// 添加一个裁剪合并任务
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="reqKeeper"></param>
        /// <returns></returns>
        public ResKeeperCutMergeTaskResponse AddCutOrMergeTask(out ResponseStruct rs, ReqKeeperCutMergeTask reqKeeper)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string url = $"{_baseUrl}/CutMergeService/AddCutOrMergeTask";
            try
            {
                string reqData = JsonHelper.ToJson(reqKeeper);
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("AccessKey", _accessKey);

                var httpRet = NetHelper.HttpPostRequest(url, headers, reqData, "utf-8", _httpClientTimeout);
                if (!string.IsNullOrEmpty(httpRet))
                {
                    if (UtilsHelper.HttpClientResponseIsNetWorkError(httpRet))
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_HttpClientTimeout,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_HttpClientTimeout],
                        };
                        return null;
                    }

                    var resCutMergeTaskResponse = JsonHelper.FromJson<ResKeeperCutMergeTaskResponse>(httpRet);
                    if (resCutMergeTaskResponse != null)
                    {
                        return resCutMergeTaskResponse;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                        ExceptMessage = httpRet,
                        ExceptStackTrace = "",
                    };
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
            }

            return null;
        }
    }
}