using System.Collections.Generic;
using AKStreamWeb.Attributes;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs.WebResponse.AKStreamKeeper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamWeb.Controllers
{
    /// <summary>
    /// 流媒体服务器治理的相关接口
    /// </summary>
    [Log]
    [AuthVerify]
    [ApiController]
    [Route("/AKStreamKeeper")]
    [SwaggerTag("流媒体服务器治理的相关接口")]
    public class AKStreamKeeperController : ControllerBase
    {
        /// <summary>
        /// 获取ffmpeg模板列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId">流媒体服务器id</param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetFFmpegTemplateList")]
        [HttpGet]
        public List<KeyValuePair<string, string>> GetFFmpegTemplateList(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.GetFFmpegTemplateList(mediaServerId, out rs);
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
        /// <param name="mediaServerId">流媒体服务器id</param>
        /// <param name="templateName">模板名称</param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("DelFFmpegTemplate")]
        [HttpGet]
        public bool DelFFmpegTemplate([FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId,
            string templateName)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.DelFFmpegTemplate(mediaServerId, templateName, out rs);
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
        /// <param name="mediaServerId">流媒体服务器id</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="templateValue">模板命令（ffmpeg命令）</param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ModifyFFmpegTemplate")]
        [HttpGet]
        public bool ModifyFFmpegTemplate([FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId,
            string templateName,
            string templateValue)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.ModifyFFmpegTemplate(mediaServerId, templateName, templateValue, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 添加ffmpeg模板
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId">流媒体服务器id</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="templateValue">模板命令（ffmpeg命令）</param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("AddFFmpegTemplate")]
        [HttpGet]
        public bool AddFFmpegTemplate([FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId,
            string templateName,
            string templateValue)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.AddFFmpegTemplate(mediaServerId, templateName, templateValue, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取Keeper程序版本
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetVersion")]
        [HttpGet]
        public string GetVersion(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.GetVersion(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取一个可用的rtp端口（偶数端口）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GuessAnRtpPort")]
        [HttpGet]
        public ushort GuessAnRtpPort(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, ushort? min = 0, ushort? max = 0)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.GuessAnRtpPort(mediaServerId, out rs, min, max);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取一个可用的rtp(发送)端口（偶数端口）
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
        /// 释放rtp端口
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ReleaseRtpPort")]
        [HttpGet]
        public bool ReleaseRtpPort(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, ushort port)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.ReleaseRtpPort(mediaServerId, port, out rs);
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
        /// 删除一个指定文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("DeleteFile")]
        [HttpGet]
        public bool DeleteFile(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, string filePath)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.DeleteFile(mediaServerId, filePath, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取流媒体治理程序健康状态
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("KeeperHealth")]
        [HttpGet]
        public bool KeeperHealth(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.KeeperHealth(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 指定文件是否存在
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("FileExists")]
        [HttpGet]
        public bool FileExists(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, string filePath)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.FileExists(mediaServerId, filePath, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("DeleteFileList")]
        [HttpPost]
        public ResKeeperDeleteFileList DeleteFileList(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, List<string> fileList)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.DeleteFileList(mediaServerId, fileList, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 清空没有文件的存储目录
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("CleanUpEmptyDir")]
        [HttpGet]
        public bool CleanUpEmptyDir(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, string? filePath = "")
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.CleanUpEmptyDir(mediaServerId, out rs, filePath);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 启动流媒体服务器
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("StartMediaServer")]
        [HttpGet]
        public ResKeeperStartMediaServer StartMediaServer(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.StartMediaServer(mediaServerId, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 停止流媒体服务器
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ShutdownMediaServer")]
        [HttpGet]
        public bool ShutdownMediaServer(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.ShutdownMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 重新启动流媒体服务器
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("RestartMediaServer")]
        [HttpGet]
        public ResKeeperRestartMediaServer RestartMediaServer(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.RestartMediaServer(mediaServerId, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 流媒体服务器配置文件热加载
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ReloadMediaServer")]
        [HttpGet]
        public bool ReloadMediaServer(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.ReloadMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 检查流媒体服务器的运行状态
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("CheckMediaServerRunning")]
        [HttpGet]
        public ResKeeperCheckMediaServerRunning CheckMediaServerRunning(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = AKStreamKeeperService.CheckMediaServerRunning(mediaServerId, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}