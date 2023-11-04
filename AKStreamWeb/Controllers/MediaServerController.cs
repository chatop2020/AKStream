using System.Collections.Generic;
using AKStreamWeb.Attributes;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebRequest.AKStreamKeeper;
using LibCommon.Structs.WebResponse;
using LibCommon.Structs.WebResponse.AKStreamKeeper;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamWeb.Controllers
{
    /// <summary>
    /// 流媒体相关接口
    /// </summary>
    [Log]
    [ApiController]
    [Route("/MediaServer")]
    [SwaggerTag("流媒体相关接口")]
    public class MediaServerController : ControllerBase
    {
        /// <summary>
        /// 添加一个rtsp鉴权记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("AddRtspAuthData")]
        [HttpPost]
        [AuthVerify]
        public bool AddRtspAuthData([FromHeader(Name = "AccessKey")] UserAuth req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.AddRtspAuthData(req, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 删除一个rtsp鉴权记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("DeleteRtspAuthData")]
        [HttpPost]
        [AuthVerify]
        public bool DeleteRtspAuthData([FromHeader(Name = "AccessKey")] UserAuth req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.DeleteRtspAuthData(req, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取rtsp鉴权列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetRtspAuthData")]
        [HttpPost]
        [AuthVerify]
        public List<UserAuth> GetRtspAuthData([FromHeader(Name = "AccessKey")] UserAuth? req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetRtspAuthData(req, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取当前的开放的rtpServer列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("ListRtpServer")]
        [HttpGet]
        public List<ushort> ListRtpServer(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.ListRtpServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            if (ret != null && ret.Count > 0)
            {
                return ret;
            }
            else
            {
                return new List<ushort>();
            }
        }

        /// <summary>
        /// 添加一个裁剪合并任务
        /// </summary>
        /// <returns></returns>
        [Route("CutOrMergeVideoFile")]
        [HttpPost]
        [AuthVerify]
        public ResKeeperCutMergeTaskResponse CutOrMergeVideoFile([FromHeader(Name = "AccessKey")] string AccessKey,
            ReqKeeperCutOrMergeVideoFile rcmv)
        {
            ResponseStruct rs;
            var ret = MediaServerService.CutOrMergeVideoFile(rcmv, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取裁剪合并任务状态
        /// </summary>
        /// <returns></returns>
        [Route("GetMergeTaskStatus")]
        [HttpGet]
        [AuthVerify]
        public ResKeeperCutMergeTaskStatusResponse GetMergeTaskStatus([FromHeader(Name = "AccessKey")] string AccessKey,
            string mediaServerId, string taskId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetMergeTaskStatus(mediaServerId, taskId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取裁剪合并任务积压列表
        /// </summary>
        /// <returns></returns>
        [Route("GetBacklogTaskList")]
        [HttpGet]
        [AuthVerify]
        public ResKeeperCutMergeTaskStatusResponseList GetBacklogTaskList(
            [FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetBacklogTaskList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取在线音视频列表信息（支持分页，不支持排序）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("GetOnlineStreamInfoList")]
        [HttpPost]
        public ResGetOnlineStreamInfoList GetOnlineStreamInfoList([FromHeader(Name = "AccessKey")] string AccessKey,
            ReqGetOnlineStreamInfoList req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetOnlineStreamInfoList(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 根据数据库中的相关字段自动判断使用合适的方法结束流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("StreamStop")]
        [HttpGet]
        public bool StreamStop([FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId, string mainId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.StreamStop(mediaServerId, mainId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 自动选择推拉流方式进行拉流（支持GB28181和非GB28181）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("StreamLive")]
        [HttpGet]
        public MediaServerStreamInfo StreamLive([FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId,
            string mainId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.StreamLive(mediaServerId, mainId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 使用ffmpeg代理拉一个音视频流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("AddFFmpegStreamProxy")]
        [HttpGet]
        public MediaServerStreamInfo AddFFmpegStreamProxy([FromHeader(Name = "AccessKey")] string AccessKey,
            string mediaServerId, string mainId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.AddFFmpegStreamProxy(mediaServerId, mainId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 使用内置流代理器拉一个音视频流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("AddStreamProxy")]
        [HttpGet]
        public MediaServerStreamInfo AddStreamProxy([FromHeader(Name = "AccessKey")] string AccessKey,
            string mediaServerId, string mainId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.AddStreamProxy(mediaServerId, mainId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 停止录制文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("StopRecord")]
        [HttpGet]
        public ResZLMediaKitStopRecord StopRecord([FromHeader(Name = "AccessKey")] string AccessKey,
            string mediaServerId, string mainId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.StopRecord(mediaServerId, mainId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 启动录制文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("StartRecord")]
        [HttpGet]
        public ResZLMediaKitStartRecord StartRecord([FromHeader(Name = "AccessKey")] string AccessKey,
            string mediaServerId, string mainId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.StartRecord(mediaServerId, mainId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 开放一个rtp端口
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId">流媒体服务器id</param>
        /// <param name="stream">流id</param>
        /// <returns>返回结构（ResMediaServerOpenRtpPort）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("MediaServerOpenRtpPort")]
        [HttpGet]
        public ResMediaServerOpenRtpPort MediaServerOpenRtpPort([FromHeader(Name = "AccessKey")] string AccessKey,
            string mediaServerId, string stream)
        {
            ResponseStruct rs;
            var ret = MediaServerService.MediaServerOpenRtpPort(mediaServerId, stream, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 软删除一批录制文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="dbIdList">数据库中id列表</param>
        /// <returns>返回没有正常删掉的列表（ResDeleteFileList）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("SoftDeleteRecordFileList")]
        [HttpPost]
        public ResDeleteFileList SoftDeleteRecordFileList([FromHeader(Name = "AccessKey")] string AccessKey,
            List<long> dbIdList)
        {
            ResponseStruct rs;
            var ret = MediaServerService.SoftDeleteRecordFileList(dbIdList, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 立即删除一批录制文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="dbIdList">数据库中id列表</param>
        /// <returns>返回没有正常删掉的列表（ResDeleteFileList）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("DeleteRecordFileList")]
        [HttpPost]
        public ResDeleteFileList DeleteRecordFileList([FromHeader(Name = "AccessKey")] string AccessKey,
            List<long> dbIdList)
        {
            ResponseStruct rs;
            var ret = MediaServerService.DeleteRecordFileList(dbIdList, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 删除一个录制文件（立即删除）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="dbId">数据库id</param>
        /// <returns>返回true/false</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("DeleteRecordFile")]
        [HttpGet]
        public bool DeleteRecordFile([FromHeader(Name = "AccessKey")] string AccessKey, long dbId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.DeleteRecordFile(dbId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 恢复被软删除的录制文件
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="dbId">数据库id</param>
        /// <returns>返回true/false</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("RestoreSoftDeleteRecordFile")]
        [HttpGet]
        public bool RestoreSoftDeleteRecordFile([FromHeader(Name = "AccessKey")] string AccessKey, long dbId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.RestoreSoftDeleteRecordFile(dbId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 删除一个录制文件（软删除，不会立即删除文件，文件会在24小时后被删除）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="dbId">数据库id</param>
        /// <returns>返回true/false</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("SoftDeleteRecordFile")]
        [HttpGet]
        public bool SoftDeleteRecordFile([FromHeader(Name = "AccessKey")] string AccessKey, long dbId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.SoftDeleteRecordFile(dbId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取录像文件列表（支持分页,全表条件）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="req">结构（ReqGetRecordFileList），除分页结构外，任何属性都可以为空</param>
        /// <returns>结构（ResGetRecordFileList）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("GetRecordFileList")]
        [HttpPost]
        public ResGetRecordFileList GetRecordFileList([FromHeader(Name = "AccessKey")] string AccessKey,
            ReqGetRecordFileList req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetRecordFileList(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取音视频通道实例列表（支持分页，全表条件）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="req">结构（ReqGetVideoChannelList），除分页结构外任何属性都可为空</param>
        /// <returns>返回结构（ReqGetVideoChannelList）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("GetVideoChannelList")]
        [HttpPost]
        public ResGetVideoChannelList GetVideoChannelList([FromHeader(Name = "AccessKey")] string AccessKey,
            ReqGetVideoChannelList req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetVideoChannelList(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 删除一个音视频通道实例
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mianId">音视频通道实例唯一ID</param>
        /// <returns>返回true/false</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("DeleteVideoChannel")]
        [HttpGet]
        public bool DeleteVideoChannel([FromHeader(Name = "AccessKey")] string AccessKey, string mainId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.DeleteVideoChannel(mainId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 添加音视频通道实例
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="req">结构（ReqAddVideoChannel）</param>
        /// <returns>返回音视频通道实例（VideoChannel）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("AddVideoChannel")]
        [HttpPost]
        public VideoChannel AddVideoChannel([FromHeader(Name = "AccessKey")] string AccessKey, ReqAddVideoChannel req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.AddVideoChannel(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 修改音视频通道实例参数
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mainId">音视频通道实例唯一ID</param>
        /// <param name="req">结构（ReqModifyVideoChannel）</param>
        /// <returns>返回音视频通道实例（VideoChannel）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("ModifyVideoChannel")]
        [HttpPost]
        public VideoChannel ModifyVideoChannel([FromHeader(Name = "AccessKey")] string AccessKey, string mainId,
            ReqModifyVideoChannel req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.ModifyVideoChannel(mainId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取一帧视频帧
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns>图片的base64</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("GetStreamSnap")]
        [HttpPost]
        public string GetStreamSnap([FromHeader(Name = "AccessKey")] string AccessKey, string mediaServerId,
            ReqZLMediaKitGetSnap req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetStreamSnap(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 激活音视频通道实例
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mainId">音视频通道实例的唯一ID</param>
        /// <param name="req">结构（ReqActiveVideoChannel）</param>
        /// <returns>返回音视频通道实例（VideoChannel）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("ActiveVideoChannel")]
        [HttpPost]
        public VideoChannel ActiveVideoChannel([FromHeader(Name = "AccessKey")] string AccessKey, string mainId,
            ReqActiveVideoChannel req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.ActiveVideoChannel(mainId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取未激活视频通道实例列表（支持分页）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="req">分页基本信息</param>
        /// <returns>返回带分页的未激活音视频通道实例列表（ResGetWaitForActiveVideoChannelList）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("GetWaitForActiveVideoChannelList")]
        [HttpPost]
        public ResGetWaitForActiveVideoChannelList GetWaitForActiveVideoChannelList(
            [FromHeader(Name = "AccessKey")] string AccessKey, ReqPaginationBase req)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetWaitForActiveVideoChannelList(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取流媒体服务器列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns>返回流媒体服务器实例列表</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("GetMediaServerList")]
        [HttpGet]
        public List<ServerInstance> GetMediaServerList([FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;

            var ret = MediaServerService.GetMediaServerList(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 通过MediaserverId获取流媒体服务器实例
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="mediaServerId">流媒体服务器ID</param>
        /// <returns>返回流媒体服务器实例（ServerInstance）</returns>
        /// <exception cref="AkStreamException"></exception>
        [AuthVerify]
        [Route("GetMediaServerByMediaServerId")]
        [HttpGet]
        public ServerInstance GetMediaServerByMediaServerId([FromHeader(Name = "AccessKey")] string AccessKey,
            string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerService.GetMediaServerByMediaServerId(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}