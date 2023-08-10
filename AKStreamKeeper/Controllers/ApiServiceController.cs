using System.Collections.Generic;
using System.Web;
using AKStreamKeeper.Attributes;
using AKStreamKeeper.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.WebResponse.AKStreamKeeper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamKeeper.Controllers
{
    /// <summary>
    /// 流媒体服务器相关接口
    /// </summary>
    [Log]
    [AuthVerify]
    [ApiController]
    [Route("/ApiService")]
    [SwaggerTag("流媒体服务器相关接口")]
    public class ApiServiceController : ControllerBase
    {
        /// <summary>
        /// 检查磁盘是否可写
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("CheckDiskWriteable")]
        [HttpGet]
        public bool CheckDiskWriteable([FromHeader(Name = "AccessKey")] string AccessKey, string dirPath)
        {
            ResponseStruct rs;
            var ret = Common.MediaServerInstance.CheckDiskWritable(HttpUtility.UrlDecode(dirPath), out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 删除ffmpeg模板
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("DelFFmpegTemplate")]
        [HttpGet]
        public bool DelFFmpegTemplate([FromHeader(Name = "AccessKey")] string AccessKey, string templateName)
        {
            ResponseStruct rs;
            var ret = Common.MediaServerInstance.DelFFmpegTemplate(templateName, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 修改ffmpeg模板
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="templateName"></param>
        /// <param name="templateValue"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ModifyFFmpegTemplate")]
        [HttpGet]
        public bool ModifyFFmpegTemplate([FromHeader(Name = "AccessKey")] string AccessKey, string templateName,
            string templateValue)
        {
            ResponseStruct rs;
            var ret = Common.MediaServerInstance.ModifyFFmpegTemplate(
                new KeyValuePair<string, string>(templateName, templateValue), out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取ffmpeg模板列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="templateName"></param>
        /// <param name="templateValue"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetFFmpegTemplateList")]
        [HttpGet]
        public List<KeyValuePair<string, string>> GetFFmpegTemplateList(
            [FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = Common.MediaServerInstance.GetFFmpegTempleteList(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 添加一个ffmpeg模板
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="templateName"></param>
        /// <param name="templateValue"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("AddFFmpegTemplate")]
        [HttpGet]
        public bool AddFFmpegTemplate([FromHeader(Name = "AccessKey")] string AccessKey, string templateName,
            string templateValue)
        {
            ResponseStruct rs;
            var ret = Common.MediaServerInstance.AddFFmpegTemplate(
                new KeyValuePair<string, string>(templateName, templateValue), out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

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
            var ret = ApiService.GetLoggerLevel(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取AKStreamKeeper版本标识
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        [Route("GetAKStreamKeeperVersion")]
        [HttpGet]
        public string GetAKStreamKeeperVersion([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            return Common.Version;
        }

        /// <summary>
        /// 获取rtp端口信息列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        [Route("GetRtpPortInfoList")]
        [HttpGet]
        public List<PortInfo> GetRtpPortInfoList([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            return Common.PortInfoList;
        }

        /// <summary>
        /// 获取流媒体服务器运行状态
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns>返回pid,大于0说明正在运行，否则为未运行</returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("CheckMediaServerRunning")]
        [HttpGet]
        public int CheckMediaServerRunning([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = ApiService.CheckMediaServerRunning(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 热加载流媒体服务器配置
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ReloadMediaServer")]
        [HttpGet]
        public bool ReloadMediaServer([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = ApiService.ReloadMediaServer(out rs);
            if (!ret || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 重启流媒体服务器
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("RestartMediaServer")]
        [HttpGet]
        public int RestartMediaServer([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = ApiService.RestartMediaServer(out rs);
            if (ret <= 0 || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 终止流媒体服务器
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ShutdownMediaServer")]
        [HttpGet]
        public bool ShutdownMediaServer([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = ApiService.ShutdownMediaServer(out rs);
            if (!ret || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 启动流媒体服务器
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("StartMediaServer")]
        [HttpGet]
        public int StartMediaServer([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = ApiService.StartMediaServer(out rs);
            if (ret <= 0 || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 清理空目录
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("CleanUpEmptyDir")]
        [HttpGet]
        public bool CleanUpEmptyDir([FromHeader(Name = "AccessKey")] string AccessKey, string? filePath = "")
        {
            ResponseStruct rs;
            var ret = ApiService.CleanUpEmptyDir(out rs);
            if (!ret || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="fileList"></param>
        /// <returns>当有文件未正常删除时返回这些文件列表</returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("DeleteFileList")]
        [HttpPost]
        public ResKeeperDeleteFileList DeleteFileList([FromHeader(Name = "AccessKey")] string AccessKey,
            List<string> fileList)
        {
            ResponseStruct rs;
            var ret = ApiService.DeleteFileList(fileList, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("FileExists")]
        [HttpGet]
        public bool FileExists([FromHeader(Name = "AccessKey")] string AccessKey, string filePath)
        {
            ResponseStruct rs;
            var ret = ApiService.FileExists(filePath, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 健康检测
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        [Route("WebApiHealth")]
        [HttpGet]
        public string WebApiHealth([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            return "OK";
        }

        /// <summary>
        /// 删除一个指定的文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("DeleteFile")]
        [HttpGet]
        public bool DeleteFile([FromHeader(Name = "AccessKey")] string AccessKey, string filePath)
        {
            ResponseStruct rs;
            var ret = ApiService.DeleteFile(filePath, out rs);
            if (!ret || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 释放被用过的rtp端口
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [Route("ReleaseRtpPort")]
        [HttpGet]
        public bool ReleaseRtpPort([FromHeader(Name = "AccessKey")] string AccessKey, ushort port)
        {
            var ret = ApiService.ReleaseRtpPort(port);
            return ret;
        }

        /// <summary>
        /// 释放被用过的rtp(发送)端口
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [Route("ReleaseRtpPortForSender")]
        [HttpGet]
        public bool ReleaseRtpPortForSender([FromHeader(Name = "AccessKey")] string AccessKey, ushort port)
        {
            var ret = ApiService.ReleaseRtpPortForSender(port);
            return ret;
        }

        /// <summary>
        ///  获取一个可用的rtp端口（配置文件中minPort-maxPort的范围内的偶数端口）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GuessAnRtpPort")]
        [HttpGet]
        public ushort GuessAnRtpPort([FromHeader(Name = "AccessKey")] string AccessKey, ushort? min = 0,
            ushort? max = 0)
        {
            ResponseStruct rs;
            var ret = ApiService.GuessAnRtpPort(out rs, min, max);
            if (ret == 0 || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        ///  获取一个可用的rtp(发送)端口（配置文件中minSenderPort-maxSenderPort的范围内的偶数端口）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GuessAnRtpPortForSender")]
        [HttpGet]
        public ushort GuessAnRtpPortForSender([FromHeader(Name = "AccessKey")] string AccessKey, ushort? min = 0,
            ushort? max = 0)
        {
            ResponseStruct rs;
            var ret = ApiService.GuessAnRtpPortForSender(out rs, min, max);
            if (ret == 0 || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}