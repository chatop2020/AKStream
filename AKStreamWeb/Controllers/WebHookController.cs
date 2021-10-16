using AKStreamWeb.Attributes;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using LibZLMediaKitMediaServer.Structs.WebHookRequest;
using LibZLMediaKitMediaServer.Structs.WebHookResponse;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamWeb.Controllers
{
    [Log]
    [ApiController]
    [Route("/MediaServer/WebHook")]
    [SwaggerTag("WebHook相关接口，第三方应用不需要关心此类接口")]
    public class WebHookController : ControllerBase
    {
        /// <summary>
        /// 当有TS文件录制时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnRecordTs")]
        [HttpPost]
        public ResToWebHookOnRecordMP4 OnRecordTs(ReqForWebHookOnRecordMP4 req)
        {
            return WebHookService.OnRecordMp4(req);
        }

        /// <summary>
        /// 当有mp4录制时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnRecordMp4")]
        [HttpPost]
        public ResToWebHookOnRecordMP4 OnRecordMp4(ReqForWebHookOnRecordMP4 req)
        {
            return WebHookService.OnRecordMp4(req);
        }

        /// <summary>
        /// 有流或播放者断开时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnFlowReport")]
        [HttpPost]
        public ResToWebHookOnFlowReport OnFlowReport(ReqForWebHookOnFlowReport req)
        {
            return WebHookService.OnFlowReport(req);
        }

        /// <summary>
        /// 处理无人观看自动断流问题
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnStreamNoneReader")]
        [HttpPost]
        public ResToWebHookOnStreamNoneReader OnStreamNoneReader(ReqForWebHookOnStreamNoneReader req)
        {
            return WebHookService.OnStreamNoneReader(req);
        }

        /// <summary>
        /// 有流状态改变时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnStreamChanged")]
        [HttpPost]
        public ResToWebHookOnStreamChange OnStreamChanged(ReqForWebHookOnStreamChange req)
        {
            return WebHookService.OnStreamChanged(req);
        }

        /// <summary>
        /// 有播放者的时候
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnPlay")]
        [HttpPost]
        public ResToWebHookOnPlay OnPlay(ReqForWebHookOnPlay req)
        {
            return WebHookService.OnPlay(req);
        }

        /// <summary>
        /// 有流发布的时候，rtp流走这里
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnPublish")]
        [HttpPost]
        public ResToWebHookOnPublish OnPublish(ReqForWebHookOnPublish req)
        {
            return WebHookService.OnPublish(req);
        }

        /// <summary>
        /// 流媒体心跳
        /// </summary>
        /// <returns>返回ResMediaServerKeepAlive（ResMediaServerKeepAlive）</returns>
        [Route("MediaServerKeepAlive")]
        [HttpPost]
        public ResMediaServerKeepAlive MediaServerKeepAlive(ReqMediaServerKeepAlive req)
        {
            ResponseStruct rs;
            var ret = WebHookService.MediaServerKeepAlive(req, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}