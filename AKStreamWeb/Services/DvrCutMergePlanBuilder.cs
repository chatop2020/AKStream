using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest.AKStreamKeeper;

namespace AKStreamWeb.Services;

public static class DvrCutMergePlanBuilder
{
    private static readonly TimeSpan GapTolerance = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan MinClipDuration = TimeSpan.FromSeconds(2);
    
    
    private static bool MatchIfProvided(string? actual, string? expected)
    {
        if (string.IsNullOrWhiteSpace(expected))
            return true;

        return Normalize(actual).Equals(Normalize(expected));
    }

    private static bool MatchVhostIfProvided(string? actual, string? expected)
    {
        if (string.IsNullOrWhiteSpace(expected))
            return true;

        var a = Normalize(actual);
        var e = Normalize(expected);

        if (e.Equals("__defaultvhost__"))
            return string.IsNullOrWhiteSpace(a) || a.Equals("__defaultvhost__");

        return a.Equals(e);
    }

    private static string Normalize(string? value)
    {
        return (value ?? "").Trim().ToLower();
    }

    public static List<CutMergeStruct> Build(ReqKeeperCutOrMergeVideoFile req, out ResponseStruct rs)
    {
        rs = new ResponseStruct { Code = ErrorNumber.None, Message = ErrorMessage.ErrorDic![ErrorNumber.None] };

        if (req == null || string.IsNullOrWhiteSpace(req.MediaServerId) || string.IsNullOrWhiteSpace(req.MainId) || req.StartTime >= req.EndTime)
            return Fail(ErrorNumber.Sys_ParamsIsNotRight, out rs);

        if ((req.EndTime - req.StartTime).TotalMinutes > 120)
            return Fail(ErrorNumber.Sys_DvrCutMergeTimeLimit, out rs);

        var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
        if (mediaServer == null || mediaServer.KeeperWebApi == null || !mediaServer.IsKeeperRunning)
            return Fail(ErrorNumber.Sys_AKStreamKeeperNotRunning, out rs);

        var start = TrimToSecond(req.StartTime);
        var end = TrimToSecond(req.EndTime);

        // var files = ORMHelper.Db.Select<RecordFile>()
        //     .Where(x => x.StartTime < end && x.EndTime > start)
        //     .WhereIf(!string.IsNullOrWhiteSpace(req.MediaServerId), x => x.MediaServerId!.Trim().ToLower().Equals(req.MediaServerId!.Trim().ToLower()))
        //     .WhereIf(!string.IsNullOrWhiteSpace(req.MainId), x => x.Streamid!.Trim().ToLower().Equals(req.MainId!.Trim().ToLower()))
        //     .WhereIf(!string.IsNullOrWhiteSpace(req.App), x => x.App != null && x.App.Trim().ToLower().Equals(req.App!.Trim().ToLower()))
        //     .WhereIf(!string.IsNullOrWhiteSpace(req.Vhost), x => x.Vhost != null && x.Vhost.Trim().ToLower().Equals(req.Vhost!.Trim().ToLower()))
        //     .OrderBy(x => x.StartTime)
        //     .ToList();
        
        var files = ORMHelper.Db.Select<RecordFile>()
            .Where(x => x.StartTime < end && x.EndTime > start)
            .Where(x => x.Deleted == false || x.Deleted == null)
            .WhereIf(!string.IsNullOrWhiteSpace(req.MediaServerId),
                x => x.MediaServerId != null && x.MediaServerId.Trim().ToLower().Equals(req.MediaServerId!.Trim().ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(req.MainId),
                x => (x.MainId != null && x.MainId.Trim().ToLower().Equals(req.MainId!.Trim().ToLower())) ||
                     (x.Streamid != null && x.Streamid.Trim().ToLower().Equals(req.MainId!.Trim().ToLower())))
            .OrderBy(x => x.StartTime)
            .ToList();

        files = files
            .Where(x => MatchIfProvided(x.App, req.App))
            .Where(x => MatchVhostIfProvided(x.Vhost, req.Vhost))
            .ToList();

        var result = new List<CutMergeStruct>();
        var coveredTo = start;

        foreach (var file in files)
        {
            if (file.StartTime == null || file.EndTime == null || string.IsNullOrWhiteSpace(file.VideoPath))
                continue;

            if (!mediaServer.KeeperWebApi.FileExists(out _, file.VideoPath))
                continue;

            var fileStart = TrimToSecond(file.StartTime.Value);
            var fileEnd = TrimToSecond(file.EndTime.Value);
            if (fileEnd <= fileStart || fileEnd <= start || fileStart >= end)
                continue;

            if (result.Count == 0 && fileStart > start + GapTolerance)
                return MissingCoverage(start, fileStart, out rs);

            if (result.Count > 0 && fileStart > coveredTo + GapTolerance)
                return MissingCoverage(coveredTo, fileStart, out rs);

            var segmentStart = Max(start, fileStart);
            var segmentEnd = Min(end, fileEnd);
            if (segmentEnd <= segmentStart)
                continue;

            var cutStart = segmentStart - fileStart;
            var cutEnd = segmentEnd - fileStart;
            var sourceDuration = fileEnd - fileStart;

            NormalizeSmallClip(ref cutStart, ref cutEnd, sourceDuration);

            var needCut = cutStart > TimeSpan.Zero || cutEnd < sourceDuration;
            result.Add(new CutMergeStruct
            {
                DbId = file.Id,
                FilePath = file.VideoPath,
                StartTime = file.StartTime,
                EndTime = file.EndTime,
                Duration = file.Duration,
                FileSize = file.FileSize,
                CutStartPos = needCut ? FormatFfmpegTime(cutStart) : null,
                CutEndPos = needCut ? FormatFfmpegTime(cutEnd) : null
            });

            if (segmentEnd > coveredTo)
                coveredTo = segmentEnd;

            if (coveredTo >= end - GapTolerance)
                break;
        }

        if (result.Count == 0)
        {
            //return Fail(ErrorNumber.Sys_DvrCutMergeFileNotFound, out rs);
            rs = new ResponseStruct
            {
                Code = ErrorNumber.Sys_DvrCutMergeFileNotFound,
                Message =
                    $"{ErrorMessage.ErrorDic![ErrorNumber.Sys_DvrCutMergeFileNotFound]}，数据库未找到覆盖录像:{req.MainId},{start:yyyy-MM-dd HH:mm:ss}~{end:yyyy-MM-dd HH:mm:ss}"
            };
            return null;
        }

        if (coveredTo < end - GapTolerance)
            return MissingCoverage(coveredTo, end, out rs);

        return result;
    }

    private static List<CutMergeStruct> MissingCoverage(DateTime from, DateTime to, out ResponseStruct rs)
    {
        rs = new ResponseStruct
        {
            Code = ErrorNumber.Sys_DvrCutMergeFileNotFound,
            Message = $"{ErrorMessage.ErrorDic![ErrorNumber.Sys_DvrCutMergeFileNotFound]}，缺口:{from:yyyy-MM-dd HH:mm:ss}~{to:yyyy-MM-dd HH:mm:ss}"
        };
        return null;
    }

    private static List<CutMergeStruct> Fail(ErrorNumber code, out ResponseStruct rs)
    {
        rs = new ResponseStruct { Code = code, Message = ErrorMessage.ErrorDic![code] };
        return null;
    }

    private static void NormalizeSmallClip(ref TimeSpan cutStart, ref TimeSpan cutEnd, TimeSpan sourceDuration)
    {
        if (cutEnd - cutStart >= MinClipDuration)
            return;

        if (sourceDuration <= MinClipDuration)
        {
            cutStart = TimeSpan.Zero;
            cutEnd = sourceDuration;
            return;
        }

        var expandedEnd = cutStart + MinClipDuration;
        if (expandedEnd <= sourceDuration)
        {
            cutEnd = expandedEnd;
            return;
        }

        cutEnd = sourceDuration;
        cutStart = sourceDuration - MinClipDuration;
    }

    private static string FormatFfmpegTime(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
            value = TimeSpan.Zero;

        return $"{(int)value.TotalHours:00}:{value.Minutes:00}:{value.Seconds:00}";
    }

    private static DateTime TrimToSecond(DateTime value)
    {
        return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Kind);
    }

    private static DateTime Max(DateTime a, DateTime b) => a >= b ? a : b;
    private static DateTime Min(DateTime a, DateTime b) => a <= b ? a : b;
}