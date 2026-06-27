using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;

namespace AKStreamWeb.Services;

public static class AutoRecordStartHelper
{
    private static readonly ConcurrentDictionary<string, byte> StartingRecordKeys = new();

    public static void TryStartAutoRecordAfterStreamReady(VideoChannelMediaInfo info, string trigger)
    {
        if (info == null || info.MediaServerStreamInfo == null) return;

        if (info.StreamSourceType != StreamSourceType.Live) return;

        var mediaServerId = info.MediaServerId;
        var mainId = info.MainId;

        var mediaServer = MediaServerService.CheckMediaServer(mediaServerId, out var checkMediaServerRs);
        if (mediaServer == null || !checkMediaServerRs.Code.Equals(ErrorNumber.None))
        {
            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->{trigger}->即时自动录制跳过，流媒体服务器不可用->{mediaServerId}->{mainId}->{checkMediaServerRs.ToJson()}");
            return;
        }

        if (!ShouldStartAutoRecordPolicy(info, mediaServer, out var reason))
        {
            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->{trigger}->即时自动录制条件未触发->{mediaServerId}->{mainId}->{reason}");
            return;
        }

        if (!TryEnterStartRecord(mediaServerId, mainId))
        {
            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->{trigger}->即时自动录制已有并发任务，跳过->{mediaServerId}->{mainId}");
            return;
        }

        try
        {
            if (!TryCheckZlmMp4Recording(mediaServer, info, out var isRecording, out var checkInfo))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->{trigger}->即时自动录制跳过，无法确认ZLM录制状态->{mediaServerId}->{mainId}->{checkInfo}");
                return;
            }

            if (isRecording)
            {
                SyncOnlineRecordStatus(mediaServerId, mainId, true);
                GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->{trigger}->ZLM已处于录制状态，同步IsRecorded=true->{mediaServerId}->{mainId}");
                return;
            }

            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->{trigger}->即时自动录制条件被触发->{mediaServerId}->{mainId}->{reason}");

            var ret = MediaServerService.StartRecord(mediaServerId, mainId, out var rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None) || ret.Code != 0 || !ret.Result)
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->{trigger}->即时自动录制失败->{mediaServerId}->{mainId}->{rs.ToJson()}->{ret.ToJson()}");
        }
        catch (Exception ex)
        {
            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->{trigger}->即时自动录制异常->{mediaServerId}->{mainId}->{ex.Message}->{ex.StackTrace}");
        }
        finally
        {
            ExitStartRecord(mediaServerId, mainId);
        }
    }


    private static bool ShouldStartAutoRecordPolicy(VideoChannel videoChannel, ServerInstance mediaServer,
        out string reason)
    {
        reason = "";

        if (videoChannel == null)
        {
            reason = "通道为空";
            return false;
        }

        if ((bool)!videoChannel.Enabled)
        {
            reason = "通道未启用";
            return false;
        }

        if (!videoChannel.AutoRecord)
        {
            reason = "通道未开启AutoRecord";
            return false;
        }

        if (UtilsHelper.StringIsNullEx(videoChannel.RecordPlanName))
        {
            reason = "通道未绑定录制计划";
            return false;
        }

        if (!IsDiskUsable(mediaServer, out reason)) return false;

        ResponseStruct rs;
        var recordPlanList = RecordPlanService.GetRecordPlanList(videoChannel.RecordPlanName, out rs);
        if (!rs.Code.Equals(ErrorNumber.None))
        {
            reason = $"获取录制计划失败:{rs.ToJson()}";
            return false;
        }

        var recordPlan = recordPlanList?.FindLast(x => x.Name.Equals(videoChannel.RecordPlanName));
        if (recordPlan == null)
        {
            reason = "录制计划不存在";
            return false;
        }

        if (!recordPlan.Enable)
        {
            reason = "录制计划未启用";
            return false;
        }

        if (!CheckTimeRange(recordPlan))
        {
            reason = "当前时间未命中录制计划";
            return false;
        }

        var fileDateList = GetRecordFileDataList(videoChannel.MainId);
        var fileSize = GetRecordFileSize(videoChannel.MainId);

        var limitDays = recordPlan.LimitDays.HasValue ? recordPlan.LimitDays.Value : int.MaxValue;
        var limitSpace = recordPlan.LimitSpace.HasValue ? recordPlan.LimitSpace.Value : decimal.MaxValue;

        if (fileDateList.Count > limitDays)
        {
            reason = $"录制天数超过限制:{fileDateList.Count}>{limitDays}";
            return false;
        }

        if (fileSize > limitSpace)
        {
            reason = $"录制空间超过限制:{fileSize}>{limitSpace}";
            return false;
        }

        reason =
            $"命中录制计划:{recordPlan.Name}, 限制天数:{limitDays}, 实际天数:{fileDateList.Count}, 限制空间:{limitSpace}, 实际空间:{fileSize}";
        return true;
    }

    private static bool IsDiskUsable(ServerInstance mediaServer, out string reason)
    {
        reason = "";

        if (mediaServer.DisksUseable == null || mediaServer.DisksUseable.Count == 0) return true;

        foreach (var disk in mediaServer.DisksUseable)
            if (disk.Value != 0)
            {
                reason = $"{disk.Key}所在的磁盘挂载异常，异常值:{disk.Value}";
                return false;
            }

        return true;
    }

    private static bool TryCheckZlmMp4Recording(ServerInstance mediaServer, VideoChannel info, out bool isRecording,
        out string infoText)
    {
        isRecording = false;
        infoText = "";

        try
        {
            var ret = mediaServer.WebApiHelper.IsRecording(new ReqZLMediaKitIsRecording
            {
                Type = 1,
                Vhost = info.Vhost,
                App = info.App,
                Stream = info.MainId
            }, out var rs);

            if (ret == null || !rs.Code.Equals(ErrorNumber.None) || ret.Code != 0)
            {
                infoText = $"{rs.ToJson()}->{ret.ToJson()}";
                return false;
            }

            isRecording = ret.Status;
            return true;
        }
        catch (Exception ex)
        {
            infoText = $"{ex.Message}->{ex.StackTrace}";
            return false;
        }
    }

    private static void SyncOnlineRecordStatus(string mediaServerId, string mainId, bool isRecorded)
    {
        VideoChannelMediaInfo retobj = null;
        lock (GCommon.Ldb.LiteDBLockObj)
        {
            retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                x.MediaServerId.Equals(mediaServerId) && x.MainId.Equals(mainId));
        }

        if (retobj != null && retobj.MediaServerStreamInfo != null)
        {
            retobj.MediaServerStreamInfo.IsRecorded = isRecorded;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                GCommon.Ldb.VideoOnlineInfo.Update(retobj);
            }
        }
    }

    private static List<string> GetRecordFileDataList(string mainId)
    {
        var ret = ORMHelper.Db.Select<RecordFile>()
            .Where(x => x.MainId.Equals(mainId))
            .Where(x => x.Deleted == false)
            .GroupBy(x => x.RecordDate)
            .OrderBy(x => x.Value.RecordDate)
            .ToList(a => a.Value.RecordDate);

        return ret?.Where(x => !string.IsNullOrEmpty(x)).Select(x => x!).ToList() ?? new List<string>();
    }

    private static decimal GetRecordFileSize(string mainId)
    {
        try
        {
            return ORMHelper.Db.Select<RecordFile>()
                .Where(x => x.MainId.Equals(mainId))
                .Where(x => x.Deleted == false)
                .Sum(x => x.FileSize);
        }
        catch
        {
            return 0;
        }
    }

    private static bool CheckTimeRange(RecordPlan recordPlan)
    {
        if (recordPlan.TimeRangeList == null || recordPlan.TimeRangeList.Count == 0) return false;

        foreach (var range in recordPlan.TimeRangeList)
            if (range != null &&
                range.WeekDay == DateTime.Now.DayOfWeek &&
                IsTimeRange(range))
                return true;

        return false;
    }

    private static bool IsTimeRange(RecordPlanRange range)
    {
        var now = DateTime.Now.TimeOfDay;
        var start = range.StartTime.ToString("HH:mm:ss");
        var end = range.EndTime.ToString("HH:mm:ss");

        if (start.Equals("00:00:00") && end.Equals("23:59:59")) return true;

        if (start.Equals("00:00:00") && end.Equals("00:00:00")) return true;

        var startTime = DateTime.Parse(start).TimeOfDay;
        var endTime = DateTime.Parse(end).TimeOfDay;

        return now > startTime && now < endTime;
    }

    private static bool TryEnterStartRecord(string mediaServerId, string mainId)
    {
        return StartingRecordKeys.TryAdd($"{mediaServerId}_{mainId}", 0);
    }

    private static void ExitStartRecord(string mediaServerId, string mainId)
    {
        StartingRecordKeys.TryRemove($"{mediaServerId}_{mainId}", out _);
    }
}