using AKStreamKeeper.Attributes;
using AKStreamKeeper.Services;
using LibCommon;
using LibCommon.Structs.WebRequest.AKStreamKeeper;
using LibCommon.Structs.WebResponse.AKStreamKeeper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AKStreamKeeper.Controllers
{
    /// <summary>
    /// 裁剪与合并视频相关接口
    /// </summary>
    [Log]
    [AuthVerify]
    [ApiController]
    [Route("/CutMergeService")]
    [SwaggerTag("裁剪与合并视频相关接口")]
    public class CutMergeServiceController : ControllerBase
    {
        /// <summary>
        /// 获取合并裁剪任务积压列表
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [HttpGet]
        [Route("GetBacklogTaskList")]
        public ResKeeperCutMergeTaskStatusResponseList GetBacklogTaskList(
            [FromHeader(Name = "AccessKey")] string AccessKey)
        {
            ResponseStruct rs;
            var ret = CutMergeService.GetBacklogTaskList(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }


        /// <summary>
        /// 获取合并剪辑任务的进度信息
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [HttpGet]
        [Route("GetMergeTaskStatus")]
        public ResKeeperCutMergeTaskStatusResponse GetMergeTaskStatus([FromHeader(Name = "AccessKey")] string AccessKey,
            string taskId)
        {
            ResponseStruct rs;
            var ret = CutMergeService.GetMergeTaskStatus(taskId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }

        /// <summary>
        /// 添加一个裁剪合并任务
        /// </summary>
        /// <param name="AccessKey"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="AkStreamException"></exception>
        [Route("AddCutOrMergeTask")]
        [HttpPost]
        public ResKeeperCutMergeTaskResponse AddCutOrMergeTask([FromHeader(Name = "AccessKey")] string AccessKey,
            ReqKeeperCutMergeTask task)
        {
            ResponseStruct rs;
            var ret = CutMergeService.AddCutOrMergeTask(task, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new AkStreamException(rs);
            }

            return ret;
        }
    }
}