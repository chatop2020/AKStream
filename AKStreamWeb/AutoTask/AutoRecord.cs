using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;

namespace AKStreamWeb.AutoTask
{
    public class AutoRecord
    {
        public AutoRecord()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    KeepRecord();
                }
                catch (Exception ex)
                {
                    GCommon.Logger.Error(
                        $"[{Common.LoggerHead}]->启动自动录制线程异常：{ex.Message}\r\n{ex.StackTrace}");
                }
            })).Start();
        }

        /// <summary>
        /// 获取记录日期列表
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        private List<string> GetRecordFileDataList(string mainId)
        {
            List<string?> ret = null!;

            ret = ORMHelper.Db.Select<RecordFile>()
                .Where(x => x.MainId.Equals(mainId))
                .Where(x => x.Deleted == false)
                .GroupBy(x => x.RecordDate)
                .OrderBy(x => x.Value.RecordDate)
                .ToList(a => a.Value.RecordDate);

            if (ret != null && ret.Count > 0)
            {
                return ret!;
            }

            return null!;
        }

        /// <summary>
        /// 获取录制文件总长度
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns></returns>
        private decimal GetRecordFileSize(string mainId)
        {
            try
            {
                var result = ORMHelper.Db.Select<RecordFile>()
                    .Where(x => x.MainId.Equals(mainId))
                    .Where(x => x.Deleted == false)
                    .Sum(x => x.FileSize);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 检查是否在时间范围内
        /// </summary>
        /// <param name="sdp"></param>
        /// <returns></returns>
        private bool CheckTimeRange(RecordPlan sdp)
        {
            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                bool haveFalse = false;
                foreach (var sdpTimeRange in sdp.TimeRangeList)
                {
                    if (sdpTimeRange != null && sdpTimeRange.WeekDay == DateTime.Now.DayOfWeek &&
                        IsTimeRange(sdpTimeRange))
                    {
                        return true; //有当天计划并在时间反问内返回true
                    }

                    if (sdpTimeRange != null && sdpTimeRange.WeekDay == DateTime.Now.DayOfWeek &&
                        !IsTimeRange(sdpTimeRange))
                    {
                        haveFalse = true; //当天计划存在，但不在范围，先做个标记，因为也许会有多个星期n的情况
                    }
                }

                if (haveFalse)
                {
                    return false; //如果循环以外，haveFalse为true,说明真的不在范围内
                }
            }

            return false; //如果是空的，就直接返回否
        }


        /// <summary>
        /// 检查时间范围
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private bool IsTimeRange(RecordPlanRange d)
        {
            TimeSpan nowDt = DateTime.Now.TimeOfDay;
            string start = d.StartTime.ToString("HH:mm:ss");
            string end = d.EndTime.ToString("HH:mm:ss");
            if (start.Trim().Equals("00:00:00") && end.Trim().Equals("23:59:59"))
            {
                return true;
            }

            if (start.Trim().Equals("00:00:00") && end.Trim().Equals("00:00:00"))
            {
                return true;
            }

            TimeSpan workStartDt = DateTime.Parse(start).TimeOfDay;
            TimeSpan workEndDt = DateTime.Parse(end).TimeOfDay;

            if (nowDt > workStartDt && nowDt < workEndDt)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 文件一个一个删除
        /// </summary>
        /// <param name="videoSize"></param>
        /// <param name="sdp"></param>
        private void DeleteFileOneByOne(decimal videoSize, MediaServerStreamInfo mediaInfo, RecordPlan plan)
        {
            ReqGetRecordFileList req = new ReqGetRecordFileList();
            req.MainId = mediaInfo.Stream;
            req.MediaServerId = mediaInfo.MediaServerId;
            req.Deleted = false;
            req.OrderBy = new List<OrderByStruct>();
            req.OrderBy.Add(new OrderByStruct()
            {
                FieldName = "starttime",
                OrderByDir = OrderByDir.ASC,
            });
            req.PageIndex = 1;
            req.PageSize = 100;


            long deleteSize = 0;

            while (videoSize - deleteSize > plan.LimitSpace)
            {
                var recordFileList = MediaServerService.GetRecordFileList(req, out ResponseStruct rs);
                if (!rs.Code.Equals(ErrorNumber.None))
                {
                    break;
                }

                if (recordFileList != null && recordFileList.RecordFileList != null &&
                    recordFileList.RecordFileList.Count > 0)
                {
                    foreach (var ret in recordFileList.RecordFileList)
                    {
                        if (ret != null)
                        {
                            if (MediaServerService.DeleteRecordFile(ret.Id, out rs))
                            {
                                deleteSize += (long)ret.FileSize!;
                                GCommon.Logger.Info(
                                    $"[{Common.LoggerHead}]->删除一个录制文件->{mediaInfo.MediaServerId}->{mediaInfo.Stream}->DBId:{ret.Id}->FilePath:{ret.VideoPath}");
                            }

                            Thread.Sleep(500);
                        }

                        if ((videoSize - deleteSize) < plan.LimitSpace)
                        {
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 一天一天删除文件
        /// </summary>
        /// <param name="days"></param>
        /// <param name="sdp"></param>
        private void DeleteFileByDay(List<string> days, MediaServerStreamInfo mediaInfo)
        {
            foreach (var day in days)
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordFile>()
                        .Where(x => x.RecordDate.Equals(day))
                        .Where(x => x.Deleted.Equals(false))
                        .Where(x => x.MainId.Equals(mediaInfo.Stream)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->DeleteFileByDay->执行SQL:->{sql}");
                }

                #endregion

                var deleteList = ORMHelper.Db.Select<RecordFile>()
                    .Where(x => x.RecordDate.Equals(day))
                    .Where(x => x.Deleted.Equals(false))
                    .Where(x => x.MainId.Equals(mediaInfo.Stream))
                    .ToList();
                if (deleteList != null && deleteList.Count > 0)
                {
                    var deleteFileList = deleteList.Select(x => x.Id).ToList();

                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                            .Set(x => x.Deleted, true)
                            .Where(x => x.RecordDate.Equals(day))
                            .Where(x => x.MainId.Equals(mediaInfo.Stream)).ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->DeleteFileByDay->执行SQL:->{sql}");
                    }

                    #endregion

                    ORMHelper.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                        .Set(x => x.Deleted, true)
                        .Where(x => x.RecordDate.Equals(day))
                        .Where(x => x.MainId.Equals(mediaInfo.Stream)).ExecuteAffrows();
                    MediaServerService.DeleteRecordFileList(deleteFileList, out _);
                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->删除一天录制文件->{mediaInfo.MediaServerId}->{mediaInfo.Stream}->{day}");
                }

                Thread.Sleep(500);
            }
        }

        private void KeepRecord()
        {
            while (true)
            {
                bool diskuseage = true;
                try
                {
                    ResponseStruct rs = null;
                    var recordPlanList = RecordPlanService.GetRecordPlanList("", out rs);

                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.Enabled.Equals(true))
                            .Where(x => !string.IsNullOrEmpty(x.RecordPlanName))
                            .Where(x => x.AutoRecord.Equals(true)).ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->KeepRecord->执行SQL:->{sql}");
                    }

                    #endregion

                    var videoChannelList = ORMHelper.Db.Select<VideoChannel>().Where(x => x.Enabled.Equals(true))
                        .Where(x => !string.IsNullOrEmpty(x.RecordPlanName))
                        .Where(x => x.AutoRecord.Equals(true)).ToList();
                    if (rs.Code.Equals(ErrorNumber.None) && recordPlanList != null && recordPlanList.Count > 0)
                    {
                        try
                        {
                            List<VideoChannelMediaInfo> retlist = null;
                            lock (GCommon.Ldb.LiteDBLockObj)
                            {
                                retlist = GCommon.Ldb.VideoOnlineInfo.FindAll().ToList();
                            }

                            foreach (var obj in retlist)
                            {
                                if (obj != null && obj.MediaServerStreamInfo != null)
                                {
                                    var MediaServerTmp =
                                        Common.MediaServerList.FindLast(x =>
                                            x.MediaServerId.Equals(obj.MediaServerId));
                                    if (MediaServerTmp != null)
                                    {
                                        if (MediaServerTmp.DisksUseable != null &&
                                            MediaServerTmp.DisksUseable.Count > 0)
                                        {
                                            foreach (var disk in MediaServerTmp.DisksUseable)
                                            {
                                                if (disk.Value != 0)
                                                {
                                                    GCommon.Logger.Warn(
                                                        $"[{Common.LoggerHead}]->{disk.Key}所在的磁盘挂载有异常，异常值为：{disk.Value},将中断{obj.MainId}的录制计划，待磁盘挂载正常后恢复录制计划");
                                                    diskuseage = false;
                                                    ResponseStruct rs2 = new ResponseStruct();

                                                    if (obj.MediaServerStreamInfo.IsRecorded != null &&
                                                        obj.MediaServerStreamInfo.IsRecorded == true)
                                                    {
                                                        try
                                                        {
                                                            var ret = MediaServerService.StopRecord(
                                                                obj.MediaServerId,
                                                                obj.MainId,
                                                                out rs2);
                                                            if (!ret.Result || ret.Code != 0 ||
                                                                rs.Code != ErrorNumber.None)
                                                            {
                                                                GCommon.Logger.Warn(
                                                                    $"[{Common.LoggerHead}]->因磁盘挂载异常停止{obj.MainId}的录制计划失败->{JsonHelper.ToJson(rs2)}");
                                                            }
                                                            else
                                                            {
                                                                GCommon.Logger.Info(
                                                                    $"[{Common.LoggerHead}]->因磁盘挂载异常停止{obj.MainId}的录制计划成功->{JsonHelper.ToJson(rs2)}");
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GCommon.Logger.Error(
                                                                $"[{Common.LoggerHead}]->因磁盘挂载异常停止{obj.MainId}的录制计划失败->{ex.Message}->{ex.StackTrace}");
                                                        }
                                                    }

                                                    break;
                                                }

                                                diskuseage = true;
                                            }
                                        }
                                    }

                                    if (!diskuseage)
                                    {
                                        Thread.Sleep(100);
                                        continue;
                                    }

                                    var videoChannel = videoChannelList.FindLast(x => x.MainId.Equals(obj.MainId));
                                    if (videoChannel != null)
                                    {
                                        //启用了自动录制
                                        var recordPlan =
                                            recordPlanList.FindLast(x => x.Name.Equals(videoChannel.RecordPlanName));
                                        if (recordPlan != null && recordPlan.Enable == true)
                                        {
                                            //说明绑定了录制模板
                                            var fileSize = GetRecordFileSize(videoChannel.MainId); //得到文件总长度
                                            var fileDateList = GetRecordFileDataList(videoChannel.MainId); //得到记录天数列表
                                            if (fileDateList == null)
                                            {
                                                fileDateList = new List<string>();
                                            }

                                            var inRange = CheckTimeRange(recordPlan);
                                            bool stopIt = false;
                                            if (!inRange)
                                            {
                                                stopIt = true;
                                            }

                                            if (inRange && recordPlan.LimitDays >= fileDateList.Count &&
                                                recordPlan.LimitSpace >= fileSize)
                                            {
                                                stopIt = false;
                                            }

                                            if (inRange && (recordPlan.LimitDays < fileDateList.Count ||
                                                            recordPlan.LimitSpace < fileSize))
                                            {
                                                stopIt = true;
                                            }


                                            if (stopIt && obj.MediaServerStreamInfo.IsRecorded == true)
                                            {
                                                switch (recordPlan.OverStepPlan)
                                                {
                                                    case OverStepPlan.StopDvr:
                                                        string info =
                                                            $"自动停止录制文件条件被触发->{obj.MediaServerId}->{obj.MainId}->{videoChannel.RecordPlanName}";
                                                        info += (recordPlan.LimitDays < fileDateList.Count)
                                                            ? $"限制录制文件天数:{recordPlan.LimitDays}<实际录制文件天数:{fileDateList.Count}"
                                                            : "";
                                                        info +=
                                                            $"->限制录制空间:{recordPlan.LimitSpace}Bytes<实际录制空间:{fileSize}Bytes";
                                                        info += !inRange ? "->超出录制模板规定的时间区间" : "";
                                                        GCommon.Logger.Info(
                                                            $"[{Common.LoggerHead}]->{info}");
                                                        MediaServerService.StopRecord(videoChannel.MediaServerId,
                                                            videoChannel.MainId, out rs);
                                                        break;
                                                    case OverStepPlan.DeleteFile:
                                                        if (!inRange)
                                                        {
                                                            string info3 = "超出录制模板规定的时间区间";
                                                            GCommon.Logger.Info(
                                                                $"[{Common.LoggerHead}]->自动停止录制文件条件被触发->{info3}->{obj.MediaServerId}->{obj.MainId}->{videoChannel.RecordPlanName}");
                                                            MediaServerService.StopRecord(videoChannel.MediaServerId,
                                                                videoChannel.MainId, out rs);
                                                        }
                                                        else
                                                        {
                                                            string info2 =
                                                                $"自动删除录制文件条件被触发->{obj.MediaServerId}->{obj.MainId}->{videoChannel.RecordPlanName}";
                                                            info2 += (recordPlan.LimitDays < fileDateList.Count)
                                                                ? $"限制录制文件天数:{recordPlan.LimitDays}<实际录制文件天数:{fileDateList.Count}"
                                                                : "";
                                                            info2 +=
                                                                $"->限制录制空间:{recordPlan.LimitSpace}Bytes<实际录制空间:{fileSize}Bytes";
                                                            GCommon.Logger.Info(
                                                                $"[{Common.LoggerHead}]->{info2}");
                                                            bool p = false;
                                                            if (recordPlan.LimitDays < fileDateList.Count) //先一天一天删除
                                                            {
                                                                int? loopCount = fileDateList.Count -
                                                                    recordPlan.LimitDays;

                                                                List<string> willDeleteDays = new List<string>();
                                                                for (int i = 0; i < loopCount; i++)
                                                                {
                                                                    willDeleteDays.Add(fileDateList[i]!);
                                                                }

                                                                DeleteFileByDay(willDeleteDays,
                                                                    obj.MediaServerStreamInfo);
                                                                p = true;
                                                            }

                                                            if (p)
                                                            {
                                                                fileSize = GetRecordFileSize(videoChannel
                                                                    .MainId); //删除完一天以后再取一下文件总长度
                                                            }

                                                            if (recordPlan.LimitSpace < fileSize) //还大，再删除一个文件
                                                            {
                                                                DeleteFileOneByOne(fileSize,
                                                                    obj.MediaServerStreamInfo,
                                                                    recordPlan);
                                                            }
                                                        }

                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                if (obj.MediaServerStreamInfo.IsRecorded == false && inRange &&
                                                    stopIt == false)
                                                {
                                                    GCommon.Logger.Info(
                                                        $"[{Common.LoggerHead}]->自动启动录制文件条件被触发->{obj.MediaServerId}->{obj.MainId}->{videoChannel.RecordPlanName}" +
                                                        $"限制录制文件天数:{recordPlan.LimitDays}>实际录制文件天数:{fileDateList.Count}->限制录制空间:{recordPlan.LimitSpace}Bytes>实际录制空间:{fileSize}Bytes" +
                                                        $"录制计划模板中时间区间被击中，开始录制音视频流");

                                                    MediaServerService.StartRecord(videoChannel.MediaServerId,
                                                        videoChannel.MainId, out rs);
                                                }
                                                else if (stopIt && obj.MediaServerStreamInfo.IsRecorded == false)
                                                {
                                                    //既没启动录制，又不让启动录制，这时要查一下有没有需要删除的文件
                                                    if (recordPlan.OverStepPlan == OverStepPlan.DeleteFile)
                                                    {
                                                        if (recordPlan.LimitDays < fileDateList.Count)
                                                        {
                                                            string info2 =
                                                                $"自动删除录制文件条件被触发->{obj.MediaServerId}->{obj.MainId}->{videoChannel.RecordPlanName}";
                                                            info2 += (recordPlan.LimitDays < fileDateList.Count)
                                                                ? $"限制录制文件天数:{recordPlan.LimitDays}<实际录制文件天数:{fileDateList.Count}"
                                                                : "";
                                                            info2 +=
                                                                $"->限制录制空间:{recordPlan.LimitSpace}Bytes<实际录制空间:{fileSize}Bytes";
                                                            GCommon.Logger.Info(
                                                                $"[{Common.LoggerHead}]->{info2}");
                                                            bool p = false;
                                                            if (recordPlan.LimitDays < fileDateList.Count) //先一天一天删除
                                                            {
                                                                int? loopCount = fileDateList.Count -
                                                                    recordPlan.LimitDays;

                                                                List<string> willDeleteDays = new List<string>();
                                                                for (int i = 0; i < loopCount; i++)
                                                                {
                                                                    willDeleteDays.Add(fileDateList[i]!);
                                                                }

                                                                DeleteFileByDay(willDeleteDays,
                                                                    obj.MediaServerStreamInfo);
                                                                p = true;
                                                            }

                                                            if (p)
                                                            {
                                                                fileSize = GetRecordFileSize(videoChannel
                                                                    .MainId); //删除完一天以后再取一下文件总长度
                                                            }

                                                            if (recordPlan.LimitSpace < fileSize) //还大，再删除一个文件
                                                            {
                                                                DeleteFileOneByOne(fileSize,
                                                                    obj.MediaServerStreamInfo,
                                                                    recordPlan);
                                                            }
                                                        }
                                                        else if (recordPlan.LimitSpace < fileSize)
                                                        {
                                                            //如果文件天数不足，则删除一个文件
                                                            DeleteFileOneByOne(fileSize, obj.MediaServerStreamInfo,
                                                                recordPlan);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (recordPlan != null && recordPlan.Enable == false)
                                        {
                                            if (obj.MediaServerStreamInfo.IsRecorded == true)
                                            {
                                                GCommon.Logger.Info(
                                                    $"[{Common.LoggerHead}]->自动停止录制条件被触发，结束录制音视频流->{obj.MediaServerId}->{obj.MainId}->{videoChannel.RecordPlanName}-录制计划模板已禁用");

                                                MediaServerService.StopRecord(videoChannel.MediaServerId,
                                                    videoChannel.MainId, out rs);
                                            }
                                        }
                                        else if (recordPlan == null)
                                        {
                                            if (obj.MediaServerStreamInfo.IsRecorded == true)
                                            {
                                                GCommon.Logger.Info(
                                                    $"[{Common.LoggerHead}]->自动停止录制条件被触发，结束录制音视频流->{obj.MediaServerId}->{obj.MainId}->未绑定录制计划模板");

                                                MediaServerService.StopRecord(videoChannel.MediaServerId,
                                                    videoChannel.MainId, out rs);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            //
                        }
                    }
                }
                catch
                {
                    //  
                }


                Thread.Sleep(10000);
            }
        }
    }
}