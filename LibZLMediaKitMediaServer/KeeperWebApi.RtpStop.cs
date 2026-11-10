using System;
using System.Collections.Generic;
using LibCommon;
using LibCommon.Structs;

namespace LibZLMediaKitMediaServer
{
    public partial class KeeperWebApi
    {
        /// <summary>发送 BYE 前，要求 Keeper 持久化端口隔离；失败时调用方不能释放端口。</summary>
        /// <param name="stop">原 SIP dialog 和接收端口。</param>
        /// <param name="rs">Keeper 调用结果。</param>
        /// <returns>Keeper 明确确认持久化成功时返回 true。</returns>
        public bool HoldRtpPortStop(RtpPortStop stop, out ResponseStruct rs)
            => CallRtpStopApi<bool>("HoldRtpPortStop", stop, out rs);

        /// <summary>提交同一 SIP 会话的清理结果，普通 ReleaseRtpPort 不能解除该隔离。</summary>
        /// <param name="stop">本次清理结果。</param>
        /// <param name="rs">Keeper 调用结果。</param>
        /// <returns>Keeper 接受当前 StopId 的结果时返回 true。</returns>
        public bool CompleteRtpPortStop(RtpPortStop stop, out ResponseStruct rs)
            => CallRtpStopApi<bool>("CompleteRtpPortStop", stop, out rs);

        /// <summary>取得 Keeper 持久化的待清理 dialog，支持 Web 或 Keeper 重启后继续清理。</summary>
        /// <param name="rs">Keeper 调用结果。</param>
        /// <returns>成功时返回记录；失败时返回 null，不能解释为没有隔离。</returns>
        public List<RtpPortStop> GetPendingRtpPortStops(out ResponseStruct rs)
            => CallRtpStopApi<List<RtpPortStop>>("GetPendingRtpPortStops", null, out rs);

        /// <summary>调用内部停流接口，不记录包含 SIP dialog 的请求或响应正文。</summary>
        private T CallRtpStopApi<T>(string action, RtpPortStop stop, out ResponseStruct rs)
        {
            rs = new ResponseStruct { Code = ErrorNumber.None, Message = ErrorMessage.ErrorDic[ErrorNumber.None] };
            try
            {
                var headers = new Dictionary<string, string> { ["AccessKey"] = _accessKey };
                var url = $"{_baseUrl}/ApiService/{action}";
                var body = stop == null
                    ? NetHelper.HttpGetRequest(url, headers, "utf-8", _httpClientTimeout)
                    : NetHelper.HttpPostRequest(url, headers, JsonHelper.ToJson(stop), "utf-8", _httpClientTimeout);
                if (string.IsNullOrWhiteSpace(body) || UtilsHelper.HttpClientResponseIsNetWorkError(body))
                    throw new InvalidOperationException($"Keeper {action} 接口未返回有效结果。");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct { Code = ErrorNumber.MediaServer_WebApiExcept,
                    Message = "Keeper 停流隔离调用失败。", ExceptMessage = ex.Message };
                return default(T);
            }
        }
    }
}
