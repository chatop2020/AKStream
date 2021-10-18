using System.Collections.Generic;
using AKStreamWeb.Attributes;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs.DBModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamWeb.Controllers
{
    [Log]
    [ApiController]
    [Route("/SipClient")]
    [SwaggerTag("Sip客户端需要用的接口，第三方应用不需要关心此类接口")]
    public class SipClientController : ControllerBase
    {
        /// <summary>
        /// 获取一个可用的rtp(发送端口（偶数端口）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GuessAnRtpPortForSender")]
        [HttpGet]
        public ushort GuessAnRtpPortForSender(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, ushort? min = 0, ushort? max = 0)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.GuessAnRtpPortForSender(mediaServerId, out rs, min, max);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 释放rtp(发送)端口
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ReleaseRtpPortForSender")]
        [HttpGet]
        public bool ReleaseRtpPortForSender(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, ushort port)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.ReleaseRtpPortForSender(mediaServerId, port, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取可共享列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetShareChannelList")]
        [HttpGet]
        public List<VideoChannel> GetShareChannelList()
        {
            ResponseStruct rs;
            var ret = SipClientService.GetShareChannelList(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取可以共享的通道数量
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ShareChannelSumCount")]
        [HttpGet]
        public int ShareChannelSumCount()
        {
            ResponseStruct rs;
            var ret = SipClientService.ShareChannelSumCount(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}