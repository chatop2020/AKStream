using System;
using System.Collections.Generic;
using LibCommon;
using LibCommon.Structs.DBModels;

namespace LibGB28181SipClient
{
    public static class WebApiHelper
    {
        private static string _baseUrl = Common.SipClientConfig.AkstreamWebHttpUrl;
        private static int _httpClientTimeout = 1000;

        /// <summary>
        /// 获取可共享通道
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<VideoChannel> GetShareChannelList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/GetShareChannelList";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
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

                    List<VideoChannel> ret = JsonHelper.FromJson<List<VideoChannel>>(httpRet);
                    if (ret != null)
                    {
                        return ret;
                    }


                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
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
        /// 取可以共享的通道数量
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int ShareChannelSumCount(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ShareChannelSumCount";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
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
                        return -1;
                    }

                    int val = -1;
                    var ret = int.TryParse(httpRet, out val);
                    if (ret && val > -1)
                    {
                        return val;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
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

            return -1;
        }


        /// <summary>
        /// 向流媒体服务器申请一个rtp端口(发送)
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ushort GuessAnRtpPortForSender(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
                           
            string url = $"{_baseUrl}/GuessAnRtpPortForSender?mediaServerId={mediaServerId}";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
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

                    ushort val = 0;
                    var ret = ushort.TryParse(httpRet, out val);
                    if (ret && val > 0)
                    {
                        return val;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
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
        /// 向流媒体服务器释放一个rtp端口(发送)
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool ReleaseRtpPortForSender(string mediaServerId, ushort port, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string url = $"{_baseUrl}/ReleaseRtpPortForSender?mediaServerId={mediaServerId}&port={port}";
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
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

                    bool val = false;
                    var ret = bool.TryParse(httpRet, out val);
                    if (ret && val == true)
                    {
                        return val;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept],
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
    }
}