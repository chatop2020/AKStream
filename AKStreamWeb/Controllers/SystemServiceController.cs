using AKStreamWeb.Attributes;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamWeb.Controllers
{
    /// <summary>
    /// 系统相关API
    /// </summary>
    [Log]
    [AuthVerify]
    [ApiController]
    [Route("/SystemApi")]
    [SwaggerTag("系统相关API")]
    public class SystemServiceController : ControllerBase
    {
        /// <summary>
        /// 获取日志级别
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetLoggerLevel")]
        [HttpGet]
        public string GetLoggerLevel([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = SystemService.GetLoggerLevel(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取版本号,包含
        /// akstreamweb版本
        /// akstreamkeeper版本
        /// zlmediakit编译时间
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        [Route("GetVersions")]
        [HttpPost]
        public AKStreamVersions GetVersions([FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = SystemService.GetVersions(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取AKStreamWeb版本标识
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        [Route("GetVersion")]
        [HttpPost]
        public string GetVersion([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            return Common.Version;
        }

        /// <summary>
        /// 获取系统性能信息
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetSystemPerformanceInfo")]
        [HttpPost]
        public PerformanceInfo GetSystemPerformanceInfo([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = SystemService.GetSystemPerformanceInfo(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetDeptartmentInfoList")]
        [HttpPost]
        public ResGetDepartmentInfo GetDeptartmentInfoList([FromHeader(Name = "AccessKey")] string AccessKey,
            ReqGetDepartmentInfo req)
        {
            ResponseStruct rs;
            var ret = SystemService.GetDeptartmentInfoList(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}