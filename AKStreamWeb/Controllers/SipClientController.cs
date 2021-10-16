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
    public class SipClientController: ControllerBase
    {
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