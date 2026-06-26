using System.Collections.Generic;
using AKStreamWeb.Attributes;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;
using Microsoft.AspNetCore.Mvc;

namespace AKStreamWeb.Controllers
{
    /// <summary>
    /// 录制计划相关的接口类
    /// 可以针对某个音视频设备实例设置对该音视频设备的开始录制与结束录制计划
    /// 可以设置录制限制（空间和时间配额），在录制受限后通过什么样的方式进行处理
    /// </summary>
    [Log]
    [AuthVerify]
    [ApiController]
    [Route("/RecordPlan")]
    public class RecordPlanController : ControllerBase
    {
        /// <summary>
        /// 删除一个录制计划ById
        /// </summary>
        /// <returns></returns>
        [Route("DeleteRecordPlanByName")]
        [HttpGet]
        public bool DeleteRecordPlanByName([FromHeader(Name = "AccessKey")] string AccessKey, string name)
        {
            ResponseStruct rs;
            var ret = RecordPlanService.DeleteRecordPlanByName(name, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 启用或停用一个录制计划
        /// </summary>
        /// <returns></returns>
        [Route("OnOrOffRecordPlanByName")]
        [HttpGet]
        public bool OnOrOffRecordPlanByName([FromHeader(Name = "AccessKey")] string AccessKey, string name, bool enable)
        {
            ResponseStruct rs;
            var ret = RecordPlanService.OnOrOffRecordPlanByName(name, enable, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 修改录制计划ById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sdp"></param>
        /// <returns></returns>
        ///
        [Route("SetRecordPlanByName")]
        [HttpPost]
        public bool SetRecordPlanByName([FromHeader(Name = "AccessKey")] string AccessKey, string name,
            ReqSetRecordPlan sdp)
        {
            ResponseStruct rs;
            var ret = RecordPlanService.SetRecordPlanByName(name, sdp, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 创建录制计划
        /// </summary>
        /// <returns></returns>
        [Route("CreateRecordPlan")]
        [HttpPost]
        public bool CreateRecordPlan([FromHeader(Name = "AccessKey")] string AccessKey, ReqSetRecordPlan sdp)
        {
            ResponseStruct rs;
            var ret = RecordPlanService.CreateRecordPlan(sdp, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取录制计划
        /// </summary>
        /// <returns></returns>
        ///
        [Route("GetRecordPlanList")]
        [HttpGet]
        public List<RecordPlan> GetRecordPlanList([FromHeader(Name = "AccessKey")] string AccessKey, string? name)
        {
            ResponseStruct rs;
            var ret = RecordPlanService.GetRecordPlanList(name, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}