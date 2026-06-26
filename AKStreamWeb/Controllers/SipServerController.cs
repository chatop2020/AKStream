using System.Collections.Generic;
using AKStreamWeb.Attributes;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.WebRequest;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamWeb.Controllers
{
    /// <summary>
    /// Sip网关相关接口
    /// </summary>
    [Log]
    [AuthVerify]
    [ApiController]
    [Route("/SipGate")]
    [SwaggerTag("Sip网关相关接口")]
    public class SipServerController : ControllerBase
    {
        /// <summary>
        /// 终止回放流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("HistroyStopVideo")]
        [HttpGet]
        public bool HistroyStopVideo(
            [FromHeader(Name = "AccessKey")] string AccessKey, int taskId, string ssrcId)
        {
            ResponseStruct rs;
            var ret = SipServerService.StopLiveVideo(taskId, ssrcId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 请求回放流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("HistroyVideo")]
        [HttpGet]
        public MediaServerStreamInfo HistroyVideo(
            [FromHeader(Name = "AccessKey")] string AccessKey, int taskId, string ssrcId)
        {
            ResponseStruct rs;
            var ret = SipServerService.LiveVideo(taskId, ssrcId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 回放录像时拖动（seek position）
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="taskId"></param>
        /// <param name="ssrcId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("HistroyVideoPosition")]
        [HttpGet]
        public bool HistroyVideoPosition(
            [FromHeader(Name = "AccessKey")] string AccessKey, int taskId, uint ssrcId, long time)
        {
            ResponseStruct rs;
            var ret = SipServerService.RecordVideoSeekPosition(taskId, ssrcId, time, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取回放文件列表状态
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetHistroyRecordFileStatus")]
        [HttpGet]
        public VideoChannelRecordInfo GetHistroyRecordFileStatus(
            [FromHeader(Name = "AccessKey")] string AccessKey, int taskId)
        {
            ResponseStruct rs;
            var ret = SipServerService.GetHistroyRecordFileStatus(taskId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取历史录像列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="queryRecordFile"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetHistroyRecordFileList")]
        [HttpPost]
        public int GetHistroyRecordFileList(
            [FromHeader(Name = "AccessKey")] string AccessKey, string deviceId, string channelId,
            SipQueryRecordFile queryRecordFile)
        {
            ResponseStruct rs;
            var ret = SipServerService.GetHistroyRecordFileList(deviceId, channelId, queryRecordFile, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// ptz控制接口
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="ptzCmd"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("PtzCtrl")]
        [HttpPost]
        public bool PtzCtrl(
            [FromHeader(Name = "AccessKey")] string AccessKey, ReqPtzCtrl ptzCmd)
        {
            ResponseStruct rs;
            var ret = SipServerService.PtzCtrl(ptzCmd, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 请求gb28181直播流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <param name="rtpPort">设置成null(不传),或者0，将自动申请rtp端口，否则需要由应用自行申请rtp端口后填入</param>
        /// <returns>流媒体的相关访问信息</returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("LiveVideo")]
        [HttpGet]
        public MediaServerStreamInfo LiveVideo(
            [FromHeader(Name = "AccessKey")] string AccessKey, string deviceId, string channelId, ushort? rtpPort = 0)
        {
            ResponseStruct rs;
            var ret = SipServerService.LiveVideo(deviceId, channelId, out rs, rtpPort);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 停止GB28181直播流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("StopLiveVideo")]
        [HttpGet]
        public bool StopLiveVideo(
            [FromHeader(Name = "AccessKey")] string AccessKey, string deviceId, string channelId)
        {
            ResponseStruct rs;
            var ret = SipServerService.StopLiveVideo(deviceId, channelId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取指定通道是否正在推流
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("IsLiveVideo")]
        [HttpGet]
        public bool IsLiveVideo(
            [FromHeader(Name = "AccessKey")] string AccessKey, string deviceId, string channelId)
        {
            ResponseStruct rs;
            var ret = SipServerService.IsLiveVideo(deviceId, channelId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 根据ID获取SipChannel
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetSipChannelById")]
        [HttpGet]
        public SipChannel GetSipChannelById(
            [FromHeader(Name = "AccessKey")] string AccessKey, string deviceId, string channelId)
        {
            ResponseStruct rs;
            var ret = SipServerService.GetSipChannelById(deviceId, channelId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 通过deviceId获取sip设备实例
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetSipDeviceListByDeviceId")]
        [HttpGet]
        public SipDevice GetSipDeviceListByDeviceId(
            [FromHeader(Name = "AccessKey")] string AccessKey, string deviceId)
        {
            ResponseStruct rs;
            var ret = SipServerService.GetSipDeviceListByDeviceId(deviceId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 获取Sip设备列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("GetSipDeviceList")]
        [HttpGet]
        public List<SipDevice> GetSipDeviceList(
            [FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = SipServerService.GetSipDeviceList(out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}