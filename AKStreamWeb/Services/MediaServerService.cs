using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using AKStreamWeb.Misc;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebRequest.AKStreamKeeper;
using LibCommon.Structs.WebResponse;
using LibCommon.Structs.WebResponse.AKStreamKeeper;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebHookRequest;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;
using Newtonsoft.Json;

namespace AKStreamWeb.Services
{
    public static class MediaServerService
    {
        /// <summary>
        /// 获取rtsp鉴权列表
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<UserAuth> GetRtspAuthData(UserAuth? req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            List<UserAuth> list = null;
            if (req != null)
            {
                try
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<UserAuth>().WhereIf(!string.IsNullOrEmpty(req.MediaServerId),
                                x => x.MediaServerId.Equals(req.MediaServerId))
                            .WhereIf(!string.IsNullOrEmpty(req.Username), x => x.Username.Equals(req.Username))
                            .WhereIf(!string.IsNullOrEmpty(req.Password), x => x.Password.Equals(req.Password))
                            .WhereIf(req.Id > 0, x => x.Id.Equals(req.Id)).ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetRtspAuthData->执行SQL:->{sql}");
                    }

                    #endregion


                    list = ORMHelper.Db.Select<UserAuth>().WhereIf(!string.IsNullOrEmpty(req.MediaServerId),
                            x => x.MediaServerId.Equals(req.MediaServerId))
                        .WhereIf(!string.IsNullOrEmpty(req.Username), x => x.Username.Equals(req.Username))
                        .WhereIf(!string.IsNullOrEmpty(req.Password), x => x.Password.Equals(req.Password))
                        .WhereIf(req.Id > 0, x => x.Id.Equals(req.Id)).ToList<UserAuth>();
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace
                    };
                    return null;
                }
            }
            else
            {
                try
                {
                    list = ORMHelper.Db.Select<UserAuth>().ToList<UserAuth>();
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace
                    };
                    return null;
                }
            }

            return list;
        }

        /// <summary>
        /// 删除rtsp鉴权数据
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteRtspAuthData(UserAuth req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            int ret = 0;
            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Delete<UserAuth>().Where(x => x.MediaServerId.Equals(req.MediaServerId))
                        .Where(x => x.Username.Equals(req.Username)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->DeleteRtspAuthData->执行SQL:->{sql}");
                }

                #endregion


                ret = ORMHelper.Db.Delete<UserAuth>().Where(x => x.MediaServerId.Equals(req.MediaServerId))
                    .Where(x => x.Username.Equals(req.Username)).ExecuteAffrows();
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace
                };
                return false;
            }

            if (ret > 0)
            {
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_DB_RecordNotExists,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordNotExists],
            };

            return false;
        }

        /// <summary>
        /// 添加rtsp授权数据
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool AddRtspAuthData(UserAuth req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(req.MediaServerId, out rs);
            if (mediaServer != null)
            {
                int ret = 0;
                try
                {
                    var auth = new UserAuth()
                    {
                        MediaServerId = req.MediaServerId,
                        Username = req.Username,
                        Password = UtilsHelper.Md5New($"{req.Username}:default:{req.Password}"),
                    };

                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Insert<UserAuth>(auth).ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->AddRtspAuthData->执行SQL:->{sql}");
                    }

                    #endregion

                    ret = ORMHelper.Db.Insert<UserAuth>(auth).ExecuteAffrows();
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace
                    };
                    return false;
                }

                if (ret > 0)
                {
                    return true;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_UserAuthAlreadyExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_UserAuthAlreadyExists],
                };
            }

            return false;
        }

        /// <summary>
        /// 获取rtpServer列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<ushort> ListRtpServer(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取RtpServer服务列表失败->{mediaServerId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            var ret = mediaServer.WebApiHelper.ListRtpServer(new ReqZLMediaKitRequestBase(), out rs);
            if (ret != null && rs.Code.Equals(ErrorNumber.None))
            {
                List<ushort> tmpPortList = new List<ushort>();
                if (ret.Data == null || ret.Data.Count <= 0)
                {
                    return null;
                }

                foreach (var d in ret.Data)
                {
                    if (d != null && d.Port > 0)
                    {
                        tmpPortList.Add((ushort)d.Port);
                    }
                }

                return tmpPortList;
            }

            return null;
        }

        /// <summary>
        /// 获取需要裁剪合并的文件列表 
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static List<CutMergeStruct> AnalysisVideoFile(ReqKeeperCutOrMergeVideoFile rcmv, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(rcmv.MediaServerId));
            if (mediaServer == null || mediaServer.KeeperWebApi == null || !mediaServer.IsKeeperRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                };
                return null;
            }

            int startPos = -1;
            int endPos = -1;
            DateTime _start = DateTime.Parse(rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            DateTime _end = DateTime.Parse(rcmv.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var videoList = ORMHelper.Db.Select<RecordFile>()
                 .Where(x => (x.StartTime <= _start && x.EndTime >= _start)
                          || (x.StartTime <= _end && x.EndTime >= _end) ||
                              (x.StartTime >= _start && x.EndTime <= _end))
                .WhereIf(!string.IsNullOrEmpty(rcmv.MediaServerId),
                    x => x.MediaServerId!.Trim().ToLower().Equals(rcmv.MediaServerId!.Trim().ToLower()))
                .WhereIf(!string.IsNullOrEmpty(rcmv.MainId),
                    x => x.Streamid!.Trim().ToLower().Equals(rcmv.MainId!.Trim().ToLower())).OrderBy(x => x.StartTime)
                .ToList(); 

            List<RecordFile> cutMegerList = new List<RecordFile>();
            if (videoList != null && videoList.Count > 0)
            {
                for (int i = videoList.Count - 1; i >= 0; i--)
                {
                    if (!mediaServer.KeeperWebApi.FileExists(out _, videoList[i].VideoPath))
                    {
                        videoList[i] = null;
                        continue;
                    }

                    if (!DateTime.TryParse(((DateTime)videoList[i].StartTime!).ToString("yyyy-MM-dd HH:mm:ss"),
                            out _))
                    {
                        videoList[i] = null;
                        continue;
                    }

                    if (!DateTime.TryParse(((DateTime)videoList[i].EndTime!).ToString("yyyy-MM-dd HH:mm:ss"),
                            out _))
                    {
                        videoList[i] = null;
                    }
                }

                UtilsHelper.RemoveNull(videoList);


                for (int i = 0; i <= videoList.Count - 1; i++)
                {
                    var startInDb = DateTime.Parse(((DateTime)videoList[i].StartTime).ToString("yyyy-MM-dd HH:mm:ss"));
                    var endInDb = DateTime.Parse(((DateTime)videoList[i].EndTime).ToString("yyyy-MM-dd HH:mm:ss"));
                    if (startPos < 0)
                    {
                        if (_start >= startInDb && _start < endInDb) //_start大于等于视频的开始时间，同时_start又小于等于视频结束时间，那么肯定就是节点开始
                        {
                            startPos = i;
                        }
                    }

                    if (endPos < 0)
                    {
                        if (_end <= endInDb && _end >= startInDb) //如果_end小于等于视频结束时间，同时_end又大于等于视频开始时间，那么肯定就是结束位置 
                        {
                            endPos = i;
                        }
                    }

                    if (startPos > -1 && endPos > -1)
                    {
                        break;
                    }
                }

                if (startPos >= 0 && endPos >= 0) //如果开始和结束都找到了，就取这个范围内的视频
                {
                    cutMegerList = videoList.GetRange(startPos, endPos - startPos + 1);
                }
            }

            if (cutMegerList != null && cutMegerList.Count > 0) //取到了要合并文件的列表
            {
                List<CutMergeStruct> cutMergeStructList = new List<CutMergeStruct>();
                if (cutMegerList.Count == 1)
                {
                    CutMergeStruct tmpStruct = new CutMergeStruct();
                    tmpStruct.DbId = cutMegerList[0].Id;
                    tmpStruct.Duration = cutMegerList[0].Duration;
                    tmpStruct.EndTime = cutMegerList[0].EndTime;
                    tmpStruct.FilePath = cutMegerList[0].VideoPath;
                    tmpStruct.FileSize = cutMegerList[0].FileSize;
                    tmpStruct.StartTime = cutMegerList[0].StartTime;
                    var tmpCutMeger = cutMegerList[0];
                    DateTime tmpCutMegerStartTime =
                        DateTime.Parse(((DateTime)tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                    DateTime tmpCutMegerEndTime =
                        DateTime.Parse(((DateTime)tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                    if (tmpCutMegerStartTime <= _start)
                    {
                        TimeSpan ts = -tmpCutMegerStartTime.Subtract(_start); //视频的开始时间减去需要的开始时间，再取反
                        TimeSpan ts2 = tmpCutMegerEndTime.Subtract(_start) + ts; //视频的结束时间减去需要的开始时间，再加上前面的值


                        if (ts2.Hours <= 0 && ts2.Minutes <= 0 && ts2.Seconds <= 0) //如果时间ts2的各项都小于0，说明不需要裁剪
                        {
                            tmpStruct.CutStartPos = "00:00:00";
                        }
                        else //否则做裁剪参数设置
                        {
                            tmpStruct.CutStartPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                    ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                    ts.Seconds.ToString().PadLeft(2, '0');
                        }
                    }

                    if (tmpCutMegerEndTime >= _end)
                    {
                        TimeSpan ts = tmpCutMegerEndTime.Subtract(_end);
                        ts = (tmpCutMegerEndTime - tmpCutMegerStartTime).Subtract(ts);
                        if (ts.Hours <= 0 && ts.Minutes <= 0 && ts.Seconds <= 0)
                        {
                            TimeSpan ts_tmp = new TimeSpan(0, 0, Convert.ToInt32(tmpStruct.Duration));

                            tmpStruct.CutEndPos = ts_tmp.Hours.ToString().PadLeft(2, '0') + ":" +
                                                  ts_tmp.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                  ts_tmp.Seconds.ToString().PadLeft(2, '0');
                        }
                        else
                        {
                            tmpStruct.CutEndPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                  ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                  ts.Seconds.ToString().PadLeft(2, '0');
                        }
                    }


                    //如果只取一秒，ffmpeg执行好像会报错，因此这里碰到只有1秒的时候，则加一秒
                    var tmpStart = "2021-04-07 " + tmpStruct.CutStartPos;
                    var tmpEnd = "2021-04-07 " + tmpStruct.CutEndPos;
                    var startD = DateTime.Parse(tmpStart);
                    var endD = DateTime.Parse(tmpEnd);
                    if (Math.Abs((endD - startD).TotalSeconds) <= 1)
                    {
                        startD = startD.AddSeconds(-1);
                        tmpStruct.CutStartPos = startD.ToString("HH:mm:ss");
                    }

                    cutMergeStructList.Add(tmpStruct); //加入到处理列表中
                }
                else
                {
                    for (int i = 0; i <= cutMegerList.Count - 1; i++)
                    {
                        var tmpCutMeger = cutMegerList[i];
                        if (tmpCutMeger != null && i == 0) //看第一个文件是否需要裁剪
                        {
                            DateTime tmpCutMegerStartTime =
                                DateTime.Parse(((DateTime)tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                            DateTime tmpCutMegerEndTime =
                                DateTime.Parse(((DateTime)tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));

                            if (tmpCutMegerStartTime < _start && tmpCutMegerEndTime > _start
                               ) //如果视频开始时间大于需要的开始时间，而视频结束时间大于需要的开始时间
                            {
                                TimeSpan ts = -tmpCutMegerStartTime.Subtract(_start); //视频的开始时间减去需要的开始时间，再取反
                                TimeSpan ts2 = tmpCutMegerEndTime.Subtract(_start) + ts; //视频的结束时间减去需要的开始时间，再加上前面的值
                                CutMergeStruct tmpStruct = new CutMergeStruct();
                                tmpStruct.DbId = cutMegerList[i].Id;
                                tmpStruct.Duration = cutMegerList[i].Duration;
                                tmpStruct.EndTime = cutMegerList[i].EndTime;
                                tmpStruct.FilePath = cutMegerList[i].VideoPath;
                                tmpStruct.FileSize = cutMegerList[i].FileSize;
                                tmpStruct.StartTime = cutMegerList[i].StartTime;

                                if (ts2.Hours <= 0 && ts2.Minutes <= 0 && ts2.Seconds <= 0) //如果时间ts2的各项都小于0，说明不需要裁剪
                                {
                                    tmpStruct.CutEndPos = "";
                                    tmpStruct.CutStartPos = "";
                                }
                                else //否则做裁剪参数设置
                                {
                                    tmpStruct.CutEndPos = ts2.Hours.ToString().PadLeft(2, '0') + ":" +
                                                          ts2.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                          ts2.Seconds.ToString().PadLeft(2, '0');
                                    tmpStruct.CutStartPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                            ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                            ts.Seconds.ToString().PadLeft(2, '0');
                                    //如果只取一秒，ffmpeg执行好像会报错，因此这里碰到只有1秒的时候，则加一秒
                                    var tmpStart = "2021-04-07 " + tmpStruct.CutStartPos;
                                    var tmpEnd = "2021-04-07 " + tmpStruct.CutEndPos;
                                    var startD = DateTime.Parse(tmpStart);
                                    var endD = DateTime.Parse(tmpEnd);
                                    if (Math.Abs((endD - startD).TotalSeconds) <= 1)
                                    {
                                        startD = startD.AddSeconds(-1);
                                        tmpStruct.CutStartPos = startD.ToString("HH:mm:ss");
                                    }
                                }


                                cutMergeStructList.Add(tmpStruct); //加入到处理列表中
                            }
                            else //如果视频时间大于等于需要的开始时间或者大于等于需要的结束时间，时间刚刚正好，直接加进来
                            {
                                CutMergeStruct tmpStruct = new CutMergeStruct()
                                {
                                    DbId = cutMegerList[i].Id,
                                    CutEndPos = null,
                                    CutStartPos = null,
                                    Duration = cutMegerList[i].Duration,
                                    EndTime = cutMegerList[i].EndTime,
                                    FilePath = cutMegerList[i].VideoPath,
                                    FileSize = cutMegerList[i].FileSize,
                                    StartTime = cutMegerList[i].StartTime,
                                };


                                cutMergeStructList.Add(tmpStruct);
                            }
                        }
                        else if (tmpCutMeger != null && i == cutMegerList.Count - 1) //处理最后一个视频，看是否需要裁剪，后续操作同上
                        {
                            DateTime tmpCutMegerStartTime =
                                DateTime.Parse(((DateTime)tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                            DateTime tmpCutMegerEndTime =
                                DateTime.Parse(((DateTime)tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                            if (tmpCutMegerEndTime > _end)
                            {
                                TimeSpan ts = tmpCutMegerEndTime.Subtract(_end);
                                ts = (tmpCutMegerEndTime - tmpCutMegerStartTime).Subtract(ts);
                                CutMergeStruct tmpStruct = new CutMergeStruct();
                                tmpStruct.DbId = cutMegerList[i].Id;
                                tmpStruct.Duration = cutMegerList[i].Duration;
                                tmpStruct.EndTime = cutMegerList[i].EndTime;
                                tmpStruct.FilePath = cutMegerList[i].VideoPath;
                                tmpStruct.FileSize = cutMegerList[i].FileSize;
                                tmpStruct.StartTime = cutMegerList[i].StartTime;
                                if (ts.Hours <= 0 && ts.Minutes <= 0 && ts.Seconds <= 0)
                                {
                                    tmpStruct.CutEndPos = "";
                                    tmpStruct.CutStartPos = "";
                                }
                                else
                                {
                                    tmpStruct.CutEndPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                          ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                          ts.Seconds.ToString().PadLeft(2, '0');
                                    tmpStruct.CutStartPos = "00:00:00";
                                }

                                //如果只取一秒，ffmpeg执行好像会报错，因此这里碰到只有1秒的时候，则加一秒
                                if (tmpStruct.CutEndPos.Equals("00:00:01"))
                                {
                                    tmpStruct.CutEndPos = "00:00:02";
                                }


                                cutMergeStructList.Add(tmpStruct);
                            }
                            else if (tmpCutMegerEndTime <= _end)
                            {
                                CutMergeStruct tmpStruct = new CutMergeStruct()
                                {
                                    DbId = cutMegerList[i].Id,
                                    CutEndPos = null,
                                    CutStartPos = null,
                                    Duration = cutMegerList[i].Duration,
                                    EndTime = cutMegerList[i].EndTime,
                                    FilePath = cutMegerList[i].VideoPath,
                                    FileSize = cutMegerList[i].FileSize,
                                    StartTime = cutMegerList[i].StartTime,
                                };
                                cutMergeStructList.Add(tmpStruct);
                            }
                        }
                        else //如果不是第一个也不是最后一个，就是中间部分，直接加进列表 
                        {
                            CutMergeStruct tmpStruct = new CutMergeStruct()
                            {
                                DbId = cutMegerList[i].Id,
                                CutEndPos = null,
                                CutStartPos = null,
                                Duration = cutMegerList[i].Duration,
                                EndTime = cutMegerList[i].EndTime,
                                FilePath = cutMegerList[i].VideoPath,
                                FileSize = cutMegerList[i].FileSize,
                                StartTime = cutMegerList[i].StartTime,
                            };
                            cutMergeStructList.Add(tmpStruct);
                        }
                    }
                }

                return cutMergeStructList;
            }

            rs = new ResponseStruct() //报错，视频资源没有找到
            {
                Code = ErrorNumber.Sys_DvrCutMergeFileNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DvrCutMergeFileNotFound],
            };
            return null!;
        }


        /// <summary>
        /// 裁剪或合并视频文件
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResKeeperCutMergeTaskResponse CutOrMergeVideoFile(ReqKeeperCutOrMergeVideoFile rcmv,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (rcmv.StartTime >= rcmv.EndTime)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                return null!;
            }

            if ((rcmv.EndTime - rcmv.StartTime).Minutes > 120) //超过120分钟不允许执行任务
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DvrCutMergeTimeLimit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DvrCutMergeTimeLimit],
                };

                return null!;
            }

            if (string.IsNullOrEmpty(rcmv.CallbackUrl) || !UtilsHelper.IsUrl(rcmv.CallbackUrl!))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };

                return null!;
            }

            //异步回调
            var mergeList = AnalysisVideoFile(rcmv, out rs);
            if (mergeList != null && mergeList.Count > 0)
            {
                ReqKeeperCutMergeTask task = new ReqKeeperCutMergeTask()
                {
                    CutMergeFileList = mergeList,
                    CallbakUrl = rcmv.CallbackUrl,
                    CreateTime = DateTime.Now,
                    TaskId = UtilsHelper.CreateGUID(),
                    TaskStatus = MyTaskStatus.Create,
                    ProcessPercentage = 0,
                    PlayUrl = "",
                };
                try
                {
                    var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(rcmv.MediaServerId));
                    if (mediaServer == null || mediaServer.KeeperWebApi == null || mediaServer.IsKeeperRunning == false)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                        };

                        return null!;
                    }

                    var ret = mediaServer.KeeperWebApi.AddCutOrMergeTask(out rs, task);
                    if (ret == null || rs.Code != ErrorNumber.None)
                    {
                        return null!;
                    }

                    return new ResKeeperCutMergeTaskResponse()
                    {
                        Duration = -1,
                        FilePath = "",
                        FileSize = -1,
                        Status = CutMergeRequestStatus.WaitForCallBack,
                        Task = task,
                        Request = rcmv,
                    };
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct() //报错，队列大于最大值
                    {
                        Code = ErrorNumber.Sys_DvrCutProcessQueueLimit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DvrCutProcessQueueLimit] + "\r\n" +
                                  ex.Message + "\r\n" + ex.StackTrace,
                    };
                    return null!;
                }
            }

            return null!;
        }


        /// <summary>
        /// 获取裁剪合并任务状态
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="taskId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResKeeperCutMergeTaskStatusResponse GetMergeTaskStatus(string mediaServerId, string taskId,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (string.IsNullOrEmpty(mediaServerId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                return null;
            }

            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null || mediaServer.KeeperWebApi == null || mediaServer.IsKeeperRunning == false)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                };
                return null;
            }

            var ret = mediaServer.KeeperWebApi.GetMergeTaskStatus(out rs, taskId);
            if (rs.Code == ErrorNumber.None)
            {
                return ret;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }

        /// <summary>
        /// 获取裁剪合并任务积压列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResKeeperCutMergeTaskStatusResponseList GetBacklogTaskList(string mediaServerId,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (string.IsNullOrEmpty(mediaServerId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                return null;
            }

            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null || mediaServer.KeeperWebApi == null || mediaServer.IsKeeperRunning == false)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                };
                return null;
            }

            var retList = mediaServer.KeeperWebApi.GetBacklogTaskList(out rs);
            if (rs.Code == ErrorNumber.None)
            {
                return retList;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }


        /// <summary>
        /// 获取在线音视频列表信息（支持分页，不支持排序）
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResGetOnlineStreamInfoList GetOnlineStreamInfoList(ReqGetOnlineStreamInfoList req,
            out ResponseStruct rs)
        {
            bool isPageQuery = req.PageIndex != null;
            int count = 0;
            if (isPageQuery)
            {
                if (req.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取在线音视频列表信息失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }

                if (req.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取在线音视频列表信息失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }
            }

            bool foundMediaServerId = !UtilsHelper.StringIsNullEx(req.MediaServerId);
            bool foundMainId = !UtilsHelper.StringIsNullEx(req.MainId);
            bool foundVideoChannelIp = !UtilsHelper.StringIsNullEx(req.VideoChannelIp);
            bool foundStreamSourceType = (req.StreamSourceType == null || req.StreamSourceType == StreamSourceType.ALL)
                ? false
                : true;
            List<VideoChannelMediaInfo> retList = null;

            if (foundMainId && foundMediaServerId && foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                            x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId)).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                           x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                            x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId) &&
                                                                            x.StreamSourceType == req.StreamSourceType)
                                .ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                           x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                            x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId)).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                           x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                            x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId) &&
                                                                            x.StreamSourceType == req.StreamSourceType)
                                .ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId) &&
                                                                           x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            if (foundMainId && foundMediaServerId && !foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.MainId.Equals(req.MainId)).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.MainId.Equals(req.MainId) &&
                                                                            x.StreamSourceType == req.StreamSourceType)
                                .ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.MainId.Equals(req.MainId) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.MainId.Equals(req.MainId)).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.MainId.Equals(req.MainId) &&
                                                                            x.StreamSourceType == req.StreamSourceType)
                                .ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.MainId.Equals(req.MainId) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            if (foundMainId && !foundMediaServerId && foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId)).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId) &&
                                                                            x.StreamSourceType == req.StreamSourceType)
                                .ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId)).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.MainId.Equals(req.MainId) &&
                                                                            x.StreamSourceType == req.StreamSourceType)
                                .ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                           && x.MainId.Equals(req.MainId) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            if (!foundMainId && foundMediaServerId && foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.IpV4Address.Equals(req.VideoChannelIp)
                            ).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.IpV4Address.Equals(req.VideoChannelIp));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.StreamSourceType ==
                                                                            req.StreamSourceType)
                                .ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.IpV4Address.Equals(
                                                                               req.VideoChannelIp) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.IpV4Address.Equals(req.VideoChannelIp)
                                ).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.IpV4Address.Equals(req.VideoChannelIp));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.StreamSourceType ==
                                                                            req.StreamSourceType)
                                .ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                           && x.IpV4Address.Equals(
                                                                               req.VideoChannelIp) &&
                                                                           x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            if (!foundMainId && !foundMediaServerId && foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                            ).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.IpV4Address.Equals(req.VideoChannelIp));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.StreamSourceType ==
                                                                            req.StreamSourceType)
                                .ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x =>
                                x.IpV4Address.Equals(req.VideoChannelIp) &&
                                x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                ).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.IpV4Address.Equals(req.VideoChannelIp));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.IpV4Address.Equals(req.VideoChannelIp)
                                                                            && x.StreamSourceType ==
                                                                            req.StreamSourceType)
                                .ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x =>
                                x.IpV4Address.Equals(req.VideoChannelIp) &&
                                x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            if (foundMainId && !foundMediaServerId && !foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MainId.Equals(req.MainId)).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x =>
                                x.MainId.Equals(req.MainId) && x.StreamSourceType == req.StreamSourceType).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x =>
                                x.MainId.Equals(req.MainId) && x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MainId.Equals(req.MainId)).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MainId.Equals(req.MainId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x =>
                                    x.MainId.Equals(req.MainId) && x.StreamSourceType == req.StreamSourceType).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x =>
                                x.MainId.Equals(req.MainId) && x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            if (!foundMainId && foundMediaServerId && !foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                            ).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.StreamSourceType ==
                                                                            req.StreamSourceType)
                                .ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x =>
                                x.MediaServerId.Equals(req.MediaServerId) &&
                                x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                ).ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.MediaServerId.Equals(req.MediaServerId));
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.MediaServerId.Equals(req.MediaServerId)
                                                                            && x.StreamSourceType ==
                                                                            req.StreamSourceType)
                                .ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x =>
                                x.MediaServerId.Equals(req.MediaServerId) &&
                                x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            if (!foundMainId && !foundMediaServerId && !foundVideoChannelIp)
            {
                if (!isPageQuery)
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.FindAll().ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count();
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.StreamSourceType == req.StreamSourceType)
                                .ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
                else
                {
                    if (!foundStreamSourceType)
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.FindAll().ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count();
                        }
                    }
                    else
                    {
                        lock (GCommon.Ldb.LiteDBLockObj)
                        {
                            retList = GCommon.Ldb.VideoOnlineInfo.Find(x => x.StreamSourceType == req.StreamSourceType)
                                .ToList()
                                .Skip(((int)req.PageIndex - 1) * (int)req.PageSize)
                                .Take((int)req.PageSize).ToList();
                            count = GCommon.Ldb.VideoOnlineInfo.Count(x => x.StreamSourceType == req.StreamSourceType);
                        }
                    }
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var result = new ResGetOnlineStreamInfoList();
            result.Request = req;
            result.Total = count;
            result.VideoChannelMediaInfo = new List<VideoChannelMediaInfo>(retList);
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->获取在线音视频列表信息成功->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(result)}");

            return result;
        }

        /// <summary>
        /// 根据数据库中相关字段自动选择合式的方式结束流
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool StreamStop(string mediaServerId, string mainId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->停止视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }

            VideoChannel videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId))
                .Where(x => x.MediaServerId.Equals(mediaServerId)).First();

            if (videoChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->停止视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }

            if (videoChannel.DeviceStreamType != DeviceStreamType.GB28181)
            {
                VideoChannelMediaInfo obj = null;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    obj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                        x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                }

                if (obj != null && obj.MediaServerStreamInfo != null &&
                    !string.IsNullOrEmpty(obj.MediaServerStreamInfo.NoGb28181Key))
                {
                    switch (videoChannel.MethodByGetStream)
                    {
                        case MethodByGetStream.SelfMethod:

                            ReqZLMediaKitDelStreamProxy reqZlMediaKitDelStreamProxy = new ReqZLMediaKitDelStreamProxy()
                            {
                                Key = obj.MediaServerStreamInfo.NoGb28181Key,
                            };
                            var ret = mediaServer.WebApiHelper.DelStreamProxy(reqZlMediaKitDelStreamProxy, out rs);

                            if (ret == null || ret.Code != 0 || !rs.Code.Equals(ErrorNumber.None))
                            {
                                rs = new ResponseStruct()
                                {
                                    Code = ErrorNumber.MediaServer_WebApiDataExcept,
                                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept] +
                                              JsonHelper.ToJson(rs),
                                };
                                GCommon.Logger.Warn(
                                    $"[{Common.LoggerHead}]->停止视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                                return false;
                            }

                            if (ret != null && ret.Code == 0 && !ret.Data.Flag.Trim().ToLower().Equals("true"))
                            {
                                GCommon.Logger.Warn(
                                    $"[{Common.LoggerHead}]->停止视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(ret, Formatting.Indented)}");

                                return false;
                            }
                            else
                            {
                                GCommon.Logger.Info(
                                    $"[{Common.LoggerHead}]->停止视频流操作成功，等待ZLMediaKit回调后处理->{mediaServerId}->{mainId}");

                                return true;
                            }

                            break;
                        case MethodByGetStream.UseFFmpeg:
                            ReqZLMediaKitDelFFmpegSource reqZLMediaKitDelFFmpegSource =
                                new ReqZLMediaKitDelFFmpegSource()
                                {
                                    Key = obj.MediaServerStreamInfo.NoGb28181Key,
                                };
                            var ret2 = mediaServer.WebApiHelper.DelFFmpegSource(reqZLMediaKitDelFFmpegSource, out rs);

                            if (ret2 == null || ret2.Code != 0 || !rs.Code.Equals(ErrorNumber.None))
                            {
                                rs = new ResponseStruct()
                                {
                                    Code = ErrorNumber.MediaServer_WebApiDataExcept,
                                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept] +
                                              JsonHelper.ToJson(rs),
                                };
                                GCommon.Logger.Warn(
                                    $"[{Common.LoggerHead}]->停止视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                                return false;
                            }

                            if (ret2 != null && ret2.Code == 0 && !ret2.Data.Flag.Trim().ToLower().Equals("true"))
                            {
                                GCommon.Logger.Warn(
                                    $"[{Common.LoggerHead}]->停止视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(ret2, Formatting.Indented)}");

                                return false;
                            }
                            else
                            {
                                GCommon.Logger.Info(
                                    $"[{Common.LoggerHead}]->停止视频流操作成功，等待ZLMediaKit回调后处理->{mediaServerId}->{mainId}");

                                return true;
                            }

                            break;
                    }
                }
                else
                {
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->停止视频流操作失败，在线列表中不存在此流->{mediaServerId}->{mainId}");
                    return false;
                }
            }


            if (videoChannel.DeviceStreamType == DeviceStreamType.GB28181)
            {
                var r = SipServerService.StopLiveVideo(videoChannel.DeviceId, videoChannel.ChannelId, out rs);
                if (r == true)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->停止视频流操作成功->{mediaServerId}->{mainId}");
                }
                else
                {
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->停止视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");
                }

                return r;
            }


            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->停止视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

            return false;
        }

        /// <summary>
        /// 自动选择推拉流方式进行拉流（支持GB28181和非GB28181）
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static MediaServerStreamInfo StreamLive(string mediaServerId, string mainId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            VideoChannel videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId))
                .Where(x => x.Enabled.Equals(true)).Where(x => x.MediaServerId.Equals(mediaServerId)).First();
            if (videoChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (videoChannel.DeviceStreamType != DeviceStreamType.GB28181)
            {
                VideoChannelMediaInfo mediaInfo = null;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    mediaInfo = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                        x.MainId.Equals(videoChannel.MainId) && x.MediaServerId.Equals(videoChannel.MediaServerId));
                }


                if (mediaInfo != null && mediaInfo.MediaServerStreamInfo != null)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->请求内置推流成功(此通道本身就处于推流状态)->{mainId}");

                    return mediaInfo.MediaServerStreamInfo;
                }

                switch (videoChannel.MethodByGetStream)
                {
                    case MethodByGetStream.SelfMethod:
                        var r1 = AddStreamProxy(mediaServerId, mainId, out rs, videoChannel);
                        if (r1 == null || !rs.Code.Equals(ErrorNumber.None))
                        {
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->请求视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");
                        }
                        else
                        {
                            GCommon.Logger.Info(
                                $"[{Common.LoggerHead}]->请求视频流操作成功，等待ZLMediaKit回调处理->{mediaServerId}->{mainId}->{JsonHelper.ToJson(r1)}");
                        }

                        return r1;

                    case MethodByGetStream.UseFFmpeg:
                        var r2 = AddFFmpegStreamProxy(mediaServerId, mainId, out rs, videoChannel);
                        if (r2 == null || !rs.Code.Equals(ErrorNumber.None))
                        {
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->请求视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");
                        }
                        else
                        {
                            GCommon.Logger.Info(
                                $"[{Common.LoggerHead}]->请求视频流操作成功，等待ZLMediaKit回调处理->{mediaServerId}->{mainId}->{JsonHelper.ToJson(r2)}");
                        }

                        return r2;
                    default:

                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.MediaServer_StreamTypeExcept,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_StreamTypeExcept],
                        };
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->请求视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                        return null;
                }
            }

            if (videoChannel.DeviceStreamType == DeviceStreamType.GB28181)
            {
                var r3 = SipServerService.LiveVideo(videoChannel.DeviceId, videoChannel.ChannelId, out rs);
                if (r3 == null || !rs.Code.Equals(ErrorNumber.None))
                {
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->请求视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    var sipDevice =
                        LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId.Equals(videoChannel.DeviceId));
                    if (sipDevice != null && sipDevice.IsReday == true)
                    {
                        try
                        {
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->请求视频流操作失败时重新刷新一次Sip设备->{videoChannel.DeviceId}");

                            SipServerCallBack.OnDeviceReadyReceived(sipDevice);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->请求视频流操作成功，等待ZLMediaKit回调处理->{mediaServerId}->{mainId}->{JsonHelper.ToJson(r3)}");
                }

                return r3;
            }


            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + ",音视频通道实例在数据库中的参数可能有误",
            };
            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->请求视频流操作失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

            return null;
        }


        /// <summary>
        /// 获取流当前一帧图
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string GetStreamSnap(string mediaServerId, ReqZLMediaKitGetSnap req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求获取视频帧失败->{mediaServerId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            var tmpBase64 = mediaServer.WebApiHelper.GetSnap(req, out rs);
            if (!rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求获取视频帧失败->{mediaServerId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            return tmpBase64;
        }

        /// <summary>
        /// 添加一个FFmpeg代理流
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static MediaServerStreamInfo AddFFmpegStreamProxy(string mediaServerId, string mainId,
            out ResponseStruct rs, VideoChannel videoChannel = null)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求FFMPEG代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (videoChannel == null)
            {
                videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId))
                    .Where(x => x.Enabled.Equals(true)).Where(x => x.MediaServerId.Equals(mediaServerId)).First();
                if (videoChannel == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->请求FFMPEG代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (videoChannel.DeviceStreamType == DeviceStreamType.GB28181)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_StreamTypeExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_StreamTypeExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求FFMPEG代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (videoChannel.MethodByGetStream != MethodByGetStream.UseFFmpeg)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_GetStreamTypeExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_GetStreamTypeExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求FFMPEG代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (string.IsNullOrEmpty(videoChannel.VideoSrcUrl))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_VideoSrcExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_VideoSrcExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求FFMPEG代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }


            var req = new ReqZLMediaKitAddFFmpegProxy();
            if (videoChannel.DeviceStreamType.Equals(DeviceStreamType.Rtsp))
            {
                if (mediaServer.Config != null && mediaServer.Config.Data != null &&
                    mediaServer.Config.Data[0] != null &&
                    !UtilsHelper.StringIsNullEx(mediaServer.Config.Data[0].Ffmpeg_Templete_RtspTcp2Flv))
                {
                    req.Dst_Url = $"rtmp://127.0.0.1:{mediaServer.RtmpPort}/{videoChannel.App}/{videoChannel.MainId}";
                    req.Enable_Hls = 1;
                    req.Enable_Mp4 = 0;
                    req.Src_Url = videoChannel.VideoSrcUrl;
                    req.Timeout_Ms = Common.AkStreamWebConfig.WaitEventTimeOutMSec;
                    req.Ffmpeg_Cmd_Key = "ffmpeg_templete.rtsp_tcp2flv"; //采用ffmpeg rtsp with tcp模板
                }
                else
                {
                    req.Dst_Url = $"rtmp://127.0.0.1:{mediaServer.RtmpPort}/{videoChannel.App}/{videoChannel.MainId}";
                    req.Enable_Hls = 1;
                    req.Enable_Mp4 = 0;
                    req.Src_Url = videoChannel.VideoSrcUrl;
                    req.Timeout_Ms = Common.AkStreamWebConfig.WaitEventTimeOutMSec;
                    if (!string.IsNullOrEmpty(videoChannel.FFmpegTemplate))
                    {
                        req.Ffmpeg_Cmd_Key = videoChannel.FFmpegTemplate;
                    }
                }
            }
            else
            {
                req.Dst_Url = $"rtmp://127.0.0.1:{mediaServer.RtmpPort}/{videoChannel.App}/{videoChannel.MainId}";
                req.Enable_Hls = 1;
                req.Enable_Mp4 = 0;
                req.Src_Url = videoChannel.VideoSrcUrl;
                req.Timeout_Ms = Common.AkStreamWebConfig.WaitEventTimeOutMSec;
                if (!string.IsNullOrEmpty(videoChannel.FFmpegTemplate))
                {
                    req.Ffmpeg_Cmd_Key = videoChannel.FFmpegTemplate;
                }
            }

            var ret = mediaServer.WebApiHelper.AddFFmpegSource(req, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None) || ret.Code != 0)
            {
                if (ret != null && ret.Code != 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept] +
                                  JsonHelper.ToJson(ret),
                    };
                }


                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求FFMPEG代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            Common.WebHookNeedReturnTask.TryRemove($"WAITONSTREAMCHANGE_{videoChannel.MainId}",
                out _); //如果存在老的事件等待，就先删除它
            var taskWait = new WebHookNeedReturnTask(Common.WebHookNeedReturnTask);
            AutoResetEvent myWait = new AutoResetEvent(false);
            taskWait.AutoResetEvent = myWait;

            Common.WebHookNeedReturnTask.TryAdd($"WAITONSTREAMCHANGE_{videoChannel.MainId}",
                taskWait);

            var isTimeout = myWait.WaitOne(Common.AkStreamWebConfig.WaitEventTimeOutMSec);
            if (!isTimeout)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WaitWebHookTimeOut,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WaitWebHookTimeOut]
                };
                mediaServer.WebApiHelper.DelFFmpegSource(new ReqZLMediaKitDelFFmpegSource()
                {
                    Key = ret.Data.Key,
                }, out _);
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求FFMPEG代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            ReqForWebHookOnStreamChange onStreamChangeWebhook = (ReqForWebHookOnStreamChange)taskWait.OtherObj;
            var videoChannelMediaInfo = new VideoChannelMediaInfo();
            videoChannelMediaInfo.StreamSourceType = StreamSourceType.Live;
            videoChannelMediaInfo.App = videoChannel.App;
            videoChannelMediaInfo.Enabled = videoChannel.Enabled;
            videoChannelMediaInfo.Vhost = videoChannel.Vhost;
            videoChannelMediaInfo.AutoRecord = videoChannel.AutoRecord;
            videoChannelMediaInfo.AutoVideo = videoChannel.AutoVideo;
            videoChannelMediaInfo.ChannelId = videoChannel.ChannelId;
            videoChannelMediaInfo.ChannelName = videoChannel.ChannelName;
            videoChannelMediaInfo.CreateTime = videoChannel.CreateTime;
            videoChannelMediaInfo.DepartmentId = videoChannel.DepartmentId;
            videoChannelMediaInfo.DepartmentName = videoChannel.DepartmentName;
            videoChannelMediaInfo.DeviceId = videoChannel.DeviceId;
            videoChannelMediaInfo.HasPtz = videoChannel.HasPtz;
            videoChannelMediaInfo.MainId = videoChannel.MainId;
            videoChannelMediaInfo.UpdateTime = videoChannel.UpdateTime;
            videoChannelMediaInfo.DefaultRtpPort = videoChannel.DefaultRtpPort;
            videoChannelMediaInfo.DeviceNetworkType = videoChannel.DeviceNetworkType;
            videoChannelMediaInfo.DeviceStreamType = videoChannel.DeviceStreamType;
            videoChannelMediaInfo.IpV4Address = videoChannel.IpV4Address;
            videoChannelMediaInfo.IpV6Address = videoChannel.IpV6Address;
            videoChannelMediaInfo.MediaServerId = videoChannel.MediaServerId;
            videoChannelMediaInfo.NoPlayerBreak = videoChannel.NoPlayerBreak;
            videoChannelMediaInfo.PDepartmentId = videoChannel.PDepartmentId;
            videoChannelMediaInfo.PDepartmentName = videoChannel.PDepartmentName;
            videoChannelMediaInfo.RtpWithTcp = videoChannel.RtpWithTcp;
            videoChannelMediaInfo.VideoDeviceType = videoChannel.VideoDeviceType;
            videoChannelMediaInfo.VideoSrcUrl = videoChannel.VideoSrcUrl;
            videoChannelMediaInfo.MethodByGetStream = videoChannel.MethodByGetStream;
            videoChannelMediaInfo.MediaServerStreamInfo = new MediaServerStreamInfo();
            videoChannelMediaInfo.MediaServerStreamInfo.App = onStreamChangeWebhook.App;
            videoChannelMediaInfo.MediaServerStreamInfo.Ssrc = null;
            videoChannelMediaInfo.MediaServerStreamInfo.Stream = onStreamChangeWebhook.Stream;
            videoChannelMediaInfo.MediaServerStreamInfo.Vhost = onStreamChangeWebhook.Vhost;
            videoChannelMediaInfo.MediaServerStreamInfo.PlayerList = new List<MediaServerStreamPlayerInfo>();
            videoChannelMediaInfo.MediaServerStreamInfo.StartTime = DateTime.Now;
            videoChannelMediaInfo.MediaServerStreamInfo.RptPort = null;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamPort = (ushort)new Uri(videoChannel.VideoSrcUrl).Port;
            videoChannelMediaInfo.MediaServerStreamInfo.MediaServerId = mediaServerId;
            videoChannelMediaInfo.MediaServerStreamInfo.MediaServerIp = mediaServer.IpV4Address;
            videoChannelMediaInfo.MediaServerStreamInfo.PushSocketType = videoChannel.RtpWithTcp == true
                ? PushStreamSocketType.TCP
                : PushStreamSocketType.UDP;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamIp = new Uri(videoChannel.VideoSrcUrl).Host.Split(':')[0];
            videoChannelMediaInfo.MediaServerStreamInfo.StreamIp = videoChannel.IpV4Address;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamTcpId = null;
            videoChannelMediaInfo.MediaServerStreamInfo.Params = "";
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl = new List<string>();
            videoChannelMediaInfo.MediaServerStreamInfo.NoGb28181Key = ret.Data.Key;
            string exInfo =
                (!string.IsNullOrEmpty(onStreamChangeWebhook.Vhost) &&
                 !onStreamChangeWebhook.Vhost.Trim().ToLower().Equals("__defaultvhost__"))
                    ? $"?vhost={onStreamChangeWebhook.Vhost}"
                    : "";
            var prefix = Common.AkStreamWebConfig.ZlmFlvPrefix;
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = "";
            }

            prefix = prefix.Trim();
            if (!prefix.EndsWith(".") && !string.IsNullOrEmpty(prefix))
            {
                prefix += ".";
            }

            if (mediaServer.UseSsl)
            {
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtsps://{mediaServer.IpV4Address}:{mediaServer.RtspsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtmps://{mediaServer.IpV4Address}:{mediaServer.RtmpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}/hls.m3u8{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");
            }

            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"rtsp://{mediaServer.IpV4Address}:{mediaServer.RtspPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"rtmp://{mediaServer.IpV4Address}:{mediaServer.RtmpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}/hls.m3u8{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");


            VideoChannelMediaInfo retobj = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x => x.MainId.Equals(videoChannelMediaInfo.MainId)
                                                                  && x.MediaServerId.Equals(videoChannelMediaInfo
                                                                      .MediaServerId));
            }

            bool recorded = false;
            if (retobj != null)
            {
                recorded = (bool)retobj.MediaServerStreamInfo.IsRecorded;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x => x.MainId.Equals(videoChannelMediaInfo.MainId)
                                                                && x.MediaServerId.Equals(videoChannelMediaInfo
                                                                    .MediaServerId));
                }
            }

            videoChannelMediaInfo.MediaServerStreamInfo.IsRecorded = recorded;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                GCommon.Ldb.VideoOnlineInfo.Insert(videoChannelMediaInfo);
            }


            Common.WebHookNeedReturnTask.TryRemove($"WAITONSTREAMCHANGE_{videoChannel.MainId}",
                out WebHookNeedReturnTask task);
            if (task != null)
            {
                task.Dispose();
            }

            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->请求FFMPEG代理视频流成功->{mediaServerId}->{mainId}->{JsonHelper.ToJson(videoChannelMediaInfo.MediaServerStreamInfo)}");

            return videoChannelMediaInfo.MediaServerStreamInfo;
        }

        /// <summary>
        /// 添加一个内置的代理流
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static MediaServerStreamInfo AddStreamProxy(string mediaServerId, string mainId,
            out ResponseStruct rs, VideoChannel videoChannel = null)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (videoChannel == null)
            {
                videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId))
                    .Where(x => x.Enabled.Equals(true)).Where(x => x.MediaServerId.Equals(mediaServerId)).First();
                if (videoChannel == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (videoChannel.DeviceStreamType == DeviceStreamType.GB28181)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_StreamTypeExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_StreamTypeExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (videoChannel.MethodByGetStream != MethodByGetStream.SelfMethod)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_GetStreamTypeExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_GetStreamTypeExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (string.IsNullOrEmpty(videoChannel.VideoSrcUrl))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_VideoSrcExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_VideoSrcExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (!UtilsHelper.IsUrl(videoChannel.VideoSrcUrl) && !UtilsHelper.IsRtmpUrl(videoChannel.VideoSrcUrl) &&
                !UtilsHelper.IsRtspUrl(videoChannel.VideoSrcUrl))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_VideoSrcExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_VideoSrcExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }


            var req = new ReqZLMediaKitAddStreamProxy();
            req.App = videoChannel.App;
            req.Vhost = videoChannel.Vhost;
            req.Stream = videoChannel.MainId;
            req.Url = videoChannel.VideoSrcUrl;
            req.Enable_Hls = 1; //转hls要开
            req.Enable_Mp4 = 0; //录制mp4要关
            req.Rtp_Type = videoChannel.RtpWithTcp == true ? 0 : 1; //rtsp拉流时，是否使用tcp,0为tcp,1为udp,2为组播
            req.Timeout_Sec = Common.AkStreamWebConfig.WaitEventTimeOutMSec / 1000; //超时时间
            req.Retry_Count = 0; //不成功则不重试拉流
            var ret = mediaServer.WebApiHelper.AddStreamProxy(req, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None) || ret.Code != 0)
            {
                if (ret != null && ret.Code != 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_WebApiDataExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WebApiDataExcept] +
                                  JsonHelper.ToJson(ret),
                    };
                }

                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            Common.WebHookNeedReturnTask.TryRemove($"WAITONSTREAMCHANGE_{videoChannel.MainId}",
                out _); //如果存在老的事件等待，就先删除它
            var taskWait = new WebHookNeedReturnTask(Common.WebHookNeedReturnTask);
            AutoResetEvent myWait = new AutoResetEvent(false);
            taskWait.AutoResetEvent = myWait;

            Common.WebHookNeedReturnTask.TryAdd($"WAITONSTREAMCHANGE_{videoChannel.MainId}",
                taskWait);

            var isTimeout = myWait.WaitOne(Common.AkStreamWebConfig.WaitEventTimeOutMSec);
            if (!isTimeout)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_WaitWebHookTimeOut,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_WaitWebHookTimeOut]
                };
                mediaServer.WebApiHelper.DelStreamProxy(new ReqZLMediaKitDelStreamProxy()
                {
                    Key = ret.Data.Key,
                }, out _);
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求内置代理视频流失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            ReqForWebHookOnStreamChange onStreamChangeWebhook = (ReqForWebHookOnStreamChange)taskWait.OtherObj;
            var videoChannelMediaInfo = new VideoChannelMediaInfo();
            videoChannelMediaInfo.StreamSourceType = StreamSourceType.Live;
            videoChannelMediaInfo.App = videoChannel.App;
            videoChannelMediaInfo.Enabled = videoChannel.Enabled;
            videoChannelMediaInfo.Vhost = videoChannel.Vhost;
            videoChannelMediaInfo.AutoRecord = videoChannel.AutoRecord;
            videoChannelMediaInfo.AutoVideo = videoChannel.AutoVideo;
            videoChannelMediaInfo.ChannelId = videoChannel.ChannelId;
            videoChannelMediaInfo.ChannelName = videoChannel.ChannelName;
            videoChannelMediaInfo.CreateTime = videoChannel.CreateTime;
            videoChannelMediaInfo.DepartmentId = videoChannel.DepartmentId;
            videoChannelMediaInfo.DepartmentName = videoChannel.DepartmentName;
            videoChannelMediaInfo.DeviceId = videoChannel.DeviceId;
            videoChannelMediaInfo.HasPtz = videoChannel.HasPtz;
            videoChannelMediaInfo.MainId = videoChannel.MainId;
            videoChannelMediaInfo.UpdateTime = videoChannel.UpdateTime;
            videoChannelMediaInfo.DefaultRtpPort = videoChannel.DefaultRtpPort;
            videoChannelMediaInfo.DeviceNetworkType = videoChannel.DeviceNetworkType;
            videoChannelMediaInfo.DeviceStreamType = videoChannel.DeviceStreamType;
            videoChannelMediaInfo.IpV4Address = videoChannel.IpV4Address;
            videoChannelMediaInfo.IpV6Address = videoChannel.IpV6Address;
            videoChannelMediaInfo.MediaServerId = videoChannel.MediaServerId;
            videoChannelMediaInfo.NoPlayerBreak = videoChannel.NoPlayerBreak;
            videoChannelMediaInfo.PDepartmentId = videoChannel.PDepartmentId;
            videoChannelMediaInfo.PDepartmentName = videoChannel.PDepartmentName;
            videoChannelMediaInfo.RtpWithTcp = videoChannel.RtpWithTcp;
            videoChannelMediaInfo.VideoDeviceType = videoChannel.VideoDeviceType;
            videoChannelMediaInfo.VideoSrcUrl = videoChannel.VideoSrcUrl;
            videoChannelMediaInfo.MethodByGetStream = videoChannel.MethodByGetStream;
            videoChannelMediaInfo.MediaServerStreamInfo = new MediaServerStreamInfo();
            videoChannelMediaInfo.MediaServerStreamInfo.App = onStreamChangeWebhook.App;
            videoChannelMediaInfo.MediaServerStreamInfo.Ssrc = null;
            videoChannelMediaInfo.MediaServerStreamInfo.Stream = onStreamChangeWebhook.Stream;
            videoChannelMediaInfo.MediaServerStreamInfo.Vhost = onStreamChangeWebhook.Vhost;
            videoChannelMediaInfo.MediaServerStreamInfo.PlayerList = new List<MediaServerStreamPlayerInfo>();
            videoChannelMediaInfo.MediaServerStreamInfo.StartTime = DateTime.Now;
            videoChannelMediaInfo.MediaServerStreamInfo.RptPort = null;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamPort = (ushort)new Uri(videoChannel.VideoSrcUrl).Port;
            videoChannelMediaInfo.MediaServerStreamInfo.MediaServerId = mediaServerId;
            videoChannelMediaInfo.MediaServerStreamInfo.MediaServerIp = mediaServer.IpV4Address;
            videoChannelMediaInfo.MediaServerStreamInfo.PushSocketType = videoChannel.RtpWithTcp == true
                ? PushStreamSocketType.TCP
                : PushStreamSocketType.UDP;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamIp = new Uri(videoChannel.VideoSrcUrl).Host.Split(':')[0];
            videoChannelMediaInfo.MediaServerStreamInfo.StreamIp = videoChannel.IpV4Address;
            videoChannelMediaInfo.MediaServerStreamInfo.StreamTcpId = null;
            videoChannelMediaInfo.MediaServerStreamInfo.Params = "";
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl = new List<string>();
            videoChannelMediaInfo.MediaServerStreamInfo.NoGb28181Key = ret.Data.Key;
            string exInfo =
                (!string.IsNullOrEmpty(onStreamChangeWebhook.Vhost) &&
                 !onStreamChangeWebhook.Vhost.Trim().ToLower().Equals("__defaultvhost__"))
                    ? $"?vhost={onStreamChangeWebhook.Vhost}"
                    : "";
            var prefix = Common.AkStreamWebConfig.ZlmFlvPrefix;
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = "";
            }

            prefix = prefix.Trim();
            if (!prefix.EndsWith(".") && !string.IsNullOrEmpty(prefix))
            {
                prefix += ".";
            }

            if (mediaServer.UseSsl)
            {
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtsps://{mediaServer.IpV4Address}:{mediaServer.RtspsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"rtmps://{mediaServer.IpV4Address}:{mediaServer.RtmpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}/hls.m3u8{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"https://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");
                videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                    $"wss://{mediaServer.IpV4Address}:{mediaServer.HttpsPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");
            }

            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.{prefix}flv{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"rtsp://{mediaServer.IpV4Address}:{mediaServer.RtspPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"rtmp://{mediaServer.IpV4Address}:{mediaServer.RtmpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}/hls.m3u8{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.ts{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"http://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");
            videoChannelMediaInfo.MediaServerStreamInfo.PlayUrl.Add(
                $"ws://{mediaServer.IpV4Address}:{mediaServer.HttpPort}/{onStreamChangeWebhook.App}/{onStreamChangeWebhook.Stream}.live.mp4{exInfo}");

            VideoChannelMediaInfo retobj = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x => x.MainId.Equals(videoChannelMediaInfo.MainId)
                                                                  && x.MediaServerId.Equals(videoChannelMediaInfo
                                                                      .MediaServerId));
            }

            bool recorded = false;
            if (retobj != null)
            {
                recorded = (bool)retobj.MediaServerStreamInfo.IsRecorded;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x => x.MainId.Equals(videoChannelMediaInfo.MainId)
                                                                && x.MediaServerId.Equals(videoChannelMediaInfo
                                                                    .MediaServerId));
                }
            }

            videoChannelMediaInfo.MediaServerStreamInfo.IsRecorded = recorded;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                GCommon.Ldb.VideoOnlineInfo.Insert(videoChannelMediaInfo);
            }


            Common.WebHookNeedReturnTask.TryRemove($"WAITONSTREAMCHANGE_{videoChannel.MainId}",
                out WebHookNeedReturnTask task);
            if (task != null)
            {
                task.Dispose();
            }

            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->请求内置代理视频流成功->{mediaServerId}->{mainId}->{JsonHelper.ToJson(videoChannelMediaInfo.MediaServerStreamInfo)}");

            return videoChannelMediaInfo.MediaServerStreamInfo;
        }

        public static ResZLMediaKitStopRecord StopRecord(string mediaServerId, string mainId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                return null;
            }

            VideoChannel videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId))
                .Where(x => x.Enabled.Equals(true)).Where(x => x.MediaServerId.Equals(mediaServerId)).First();
            if (videoChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                };
                return null;
            }

            var reqZLMediaKitStopRecord = new ReqZLMediaKitStopRecord();
            reqZLMediaKitStopRecord.Type = 1; //0是hls,1是mp4;
            reqZLMediaKitStopRecord.Stream = videoChannel.MainId;
            reqZLMediaKitStopRecord.App = videoChannel.App;
            reqZLMediaKitStopRecord.Vhost = videoChannel.Vhost;
            var ret = mediaServer.WebApiHelper.StopRecord(reqZLMediaKitStopRecord, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None))
            {
                return null;
            }

            VideoChannelMediaInfo retobj = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x => x.MediaServerId.Equals(videoChannel.MediaServerId)
                                                                  && x.MainId.Equals(videoChannel.MainId));
            }

            if (retobj != null && retobj.MediaServerStreamInfo != null)
            {
                retobj.MediaServerStreamInfo.IsRecorded = false;
                retobj.MediaServerStreamInfo.StopRecordWithAPI = true;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                }
            }

            return ret;
        }

        /// <summary>
        /// 开始录制文件
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="mainId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitStartRecord StartRecord(string mediaServerId, string mainId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求录制文件失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            VideoChannel videoChannel = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId))
                .Where(x => x.Enabled.Equals(true)).Where(x => x.MediaServerId.Equals(mediaServerId)).First();
            if (videoChannel == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求录制文件失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }


            var reqZLMediaKitStartRecord = new ReqZLMediaKitStartRecord();
            reqZLMediaKitStartRecord.Type = 1; //0是hls,1是mp4;
            reqZLMediaKitStartRecord.Stream = videoChannel.MainId;
            reqZLMediaKitStartRecord.App = videoChannel.App;
            reqZLMediaKitStartRecord.Vhost = videoChannel.Vhost;
            reqZLMediaKitStartRecord.Max_Second = (videoChannel.RecordSecs != null && videoChannel.RecordSecs > 0)
                ? videoChannel.RecordSecs
                : null;

            if (mediaServer.RecordPathList != null && mediaServer.RecordPathList.Count > 1)
            {
                mediaServer.RecordPathList.Sort((left, right) => //对相减后的绝对值排序
                {
                    if (left.Key > right.Key)
                        return 0;
                    if ((int)left.Key == (int)right.Key)
                    {
                        return 1;
                    }

                    return -1;
                });


                reqZLMediaKitStartRecord.Customized_Path = mediaServer.RecordPathList[0].Value;
            }
            else if (mediaServer.RecordPathList != null && mediaServer.RecordPathList.Count == 1)
            {
                reqZLMediaKitStartRecord.Customized_Path = mediaServer.RecordPathList[0].Value;
            }

            var ret = mediaServer.WebApiHelper.StartRecord(reqZLMediaKitStartRecord, out rs);
            if (ret == null || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求录制文件失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (ret.Code != 0 || ret.Result != true)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求录制文件失败->{mediaServerId}->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}->{JsonHelper.ToJson(ret)}");

                return null;
            }

            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->请求录制文件成功->{mediaServerId}->{mainId}->{JsonHelper.ToJson(ret)}");

            VideoChannelMediaInfo retobj = null;
            lock (GCommon.Ldb.LiteDBLockObj)
            {
                retobj = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                    x.MediaServerId.Equals(videoChannel.MediaServerId)
                    && x.MainId.Equals(videoChannel.MainId));
            }

            if (retobj != null && retobj.MediaServerStreamInfo != null)
            {
                retobj.MediaServerStreamInfo.IsRecorded = true;
                lock (GCommon.Ldb.LiteDBLockObj)
                {
                    GCommon.Ldb.VideoOnlineInfo.Update(retobj);
                }
            }

            return ret;
        }

        /// <summary>
        /// 开放一个rtp端口
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="streamid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResMediaServerOpenRtpPort MediaServerOpenRtpPort(string mediaServerId, string stream,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = CheckMediaServer(mediaServerId, out rs);
            if (!rs.Code.Equals(ErrorNumber.None) || mediaServer == null)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求开放rtp端口失败->{mediaServerId}->{stream}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            ReqZLMediaKitOpenRtpPort reqZlMediaKitOpenRtpPort = null;
            if (mediaServer.RandomPort == false)
            {
                var rtpPortGuess =
                    mediaServer.KeeperWebApi.GuessAnRtpPort(out rs, mediaServer.RtpPortMin, mediaServer.RtpPortMax);
                if (rtpPortGuess <= 0 || !rs.Code.Equals(ErrorNumber.None))
                {
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->请求开放rtp端口失败->{mediaServerId}->{stream}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }

                reqZlMediaKitOpenRtpPort = new ReqZLMediaKitOpenRtpPort()
                {
                    Enable_Tcp = 1,
                    Port = rtpPortGuess,
                    Stream_Id = stream,
                };
            }
            else //用于支持让zlm自动生成rtp端口
            {
                reqZlMediaKitOpenRtpPort = new ReqZLMediaKitOpenRtpPort()
                {
                    Enable_Tcp = 1,
                    Port = 0,
                    Stream_Id = stream,
                };
            }

            var zlRet = mediaServer.WebApiHelper.OpenRtpPort(reqZlMediaKitOpenRtpPort, out rs);
            if (zlRet == null || !rs.Code.Equals(ErrorNumber.None))
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求开放rtp端口失败->{mediaServerId}->{stream}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (zlRet.Code != 0)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_OpenRtpPortExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_OpenRtpPortExcept],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->请求开放rtp端口失败->{mediaServerId}->{stream}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            var result = new ResMediaServerOpenRtpPort()
            {
                Port = (ushort)zlRet.Port,
                Stream = stream,
            };
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->请求开放rtp端口成功->{mediaServerId}->{stream}->{JsonHelper.ToJson(result)}");

            return result;
        }

        /// <summary>
        /// 批量删除录制文件
        /// </summary>
        /// <param name="dbIdList"></param>
        /// <param name="rs"></param>
        /// <returns>未正确删除列表</returns>
        public static ResDeleteFileList DeleteRecordFileList(List<long> dbIdList, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            ResDeleteFileList result = new ResDeleteFileList();
            result.PathList = new List<KeyValuePair<long, string>>();
            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordFile>().Where(x => dbIdList.Contains(x.Id)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->DeleteRecordFileList->执行SQL:->{sql}");
                }

                #endregion


                var dbRetList = ORMHelper.Db.Select<RecordFile>().Where(x => dbIdList.Contains(x.Id)).ToList();

                foreach (var dbId in dbIdList)
                {
                    var obj = dbRetList.FindLast(x => x.Id.Equals(dbId));
                    if (obj == null)
                    {
                        result.PathList.Add(new KeyValuePair<long, string>(dbId, "未知文件路径"));
                    }
                }

                foreach (var dbRet in dbRetList)
                {
                    if (dbRet == null)
                    {
                        result.PathList.Add(new KeyValuePair<long, string>(dbRet.Id, dbRet.VideoPath));
                        continue;
                    }

                    var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(dbRet.MediaServerId));
                    if (mediaServer == null || !mediaServer.IsKeeperRunning)
                    {
                        result.PathList.Add(new KeyValuePair<long, string>(dbRet.Id, dbRet.VideoPath));
                        continue;
                    }

                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                            .Set(x => x.Undo, false)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .Where(x => x.Id.Equals(dbRet.Id)).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->DeleteRecordFileList->执行SQL:->{sql}");
                    }

                    #endregion

                    var r = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                        .Set(x => x.Undo, false)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .Where(x => x.Id.Equals(dbRet.Id)).ExecuteAffrows();

                    if (r <= 0)
                    {
                        result.PathList.Add(new KeyValuePair<long, string>(dbRet.Id, dbRet.VideoPath));
                        continue;
                    }

                    //var deleted = mediaServer.KeeperWebApi.DeleteFile(out rs, dbRet.VideoPath);
                    var deleted = AKStreamKeeperService.DeleteFile(mediaServer.MediaServerId, dbRet.VideoPath, out rs);
                    if (!rs.Code.Equals(ErrorNumber.None) || !deleted)
                    {
                        result.PathList.Add(new KeyValuePair<long, string>(dbRet.Id, dbRet.VideoPath));
                    }

                    Thread.Sleep(10);
                }

                GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->批量删除(硬删除)录制文件成功->{JsonHelper.ToJson(result, Formatting.Indented)}");

                return result;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->批量删除(硬删除)录制文件失败->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }
        }

        /// <summary>
        /// 软批量删除文件（文件不会立即删除，在24小时后再做清理）
        /// </summary>
        /// <param name="dbIdList"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResDeleteFileList SoftDeleteRecordFileList(List<long> dbIdList, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                ResDeleteFileList result = new ResDeleteFileList();
                result.PathList = new List<KeyValuePair<long, string>>();
                foreach (var dbId in dbIdList)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                            .Set(x => x.Undo, true)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .Where(x => x.Id.Equals(dbId)).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->SoftDeleteRecordFileList->执行SQL:->{sql}");
                    }

                    #endregion

                    var ret = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                        .Set(x => x.Undo, true)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .Where(x => x.Id.Equals(dbId)).ExecuteAffrows();
                    if (ret <= 0)
                    {
                        result.PathList.Add(new KeyValuePair<long, string>(dbId, "未知文件路径"));
                    }

                    Thread.Sleep(10);
                }

                GCommon.Logger.Info(
                    $"[{Common.LoggerHead}]->批量删除(软删除)录制文件成功->{JsonHelper.ToJson(dbIdList, Formatting.Indented)}->{JsonHelper.ToJson(result, Formatting.Indented)}");

                return result;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->批量删除(软删除)录制文件失败->{JsonHelper.ToJson(dbIdList, Formatting.Indented)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }
        }

        /// <summary>
        /// 删除录制文件（立即删除）
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteRecordFile(long dbId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordFile>().Where(x => x.Id.Equals(dbId)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->DeleteRecordFile->执行SQL:->{sql}");
                }

                #endregion

                var row = ORMHelper.Db.Select<RecordFile>().Where(x => x.Id.Equals(dbId)).First();
                if (row != null)
                {
                    var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(row.MediaServerId));
                    if (mediaServer == null || !mediaServer.IsKeeperRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning] +
                                      ",相关流媒体治理程序没有运行，无法继续操作",
                        };
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->删除(硬删除)录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                        return false;
                    }

                    // var deleted = mediaServer.KeeperWebApi.DeleteFile(out rs, row.VideoPath);
                    var deleted = AKStreamKeeperService.DeleteFile(mediaServer.MediaServerId, row.VideoPath, out rs);
                    if (!rs.Code.Equals(ErrorNumber.None) || !deleted)
                    {
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->删除(硬删除)录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");
                    }

                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                            .Set(x => x.Undo, false)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .Where(x => x.Id.Equals(dbId)).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->DeleteRecordFile->执行SQL:->{sql}");
                    }

                    #endregion

                    var ret = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                        .Set(x => x.Undo, false)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .Where(x => x.Id.Equals(dbId)).ExecuteAffrows();
                    if (ret > 0)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.None,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                        };
                        GCommon.Logger.Info($"[{Common.LoggerHead}]->删除(硬删除)录制文件成功->{dbId}");

                        return true;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept] + $",数据库执行未成功，原因不明",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->删除(硬删除)录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return false;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_RecordNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除(硬删除)录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除(硬删除)录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }
        }

        /// <summary>
        /// 恢复被软删除的录制文件
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool RestoreSoftDeleteRecordFile(long dbId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordFile>().Where(x => x.Id.Equals(dbId)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->RestoreSoftDeleteRecordFile->执行SQL:->{sql}");
                }

                #endregion

                var row = ORMHelper.Db.Select<RecordFile>().Where(x => x.Id.Equals(dbId)).First();
                if (row != null)
                {
                    var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(row.MediaServerId));
                    if (mediaServer == null || !mediaServer.IsKeeperRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning] +
                                      ",相关流媒体治理程序没有运行，无法继续操作",
                        };
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->恢复被软件删除的录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                        return false;
                    }

                    var findFile = mediaServer.KeeperWebApi.FileExists(out rs, row.VideoPath);
                    if (!rs.Code.Equals(ErrorNumber.None))
                    {
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->恢复被软件删除的录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                        return false;
                    }

                    if (!findFile)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Sys_SpecifiedFileNotExists,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_SpecifiedFileNotExists] +
                                      $",服务器中不存在此文件->{row.VideoPath}",
                        };
                        GCommon.Logger.Warn(
                            $"[{Common.LoggerHead}]->恢复被软件删除的录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                        return false;
                    }

                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, false)
                            .Set(x => x.Undo, false)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .Where(x => x.Id.Equals(dbId)).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->RestoreSoftDeleteRecordFile->执行SQL:->{sql}");
                    }

                    #endregion

                    var ret = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, false)
                        .Set(x => x.Undo, false)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .Where(x => x.Id.Equals(dbId)).ExecuteAffrows();
                    if (ret > 0)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.None,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                        };
                        GCommon.Logger.Info($"[{Common.LoggerHead}]->恢复被软件删除的录制文件失败->{dbId}");

                        return true;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept] + $",数据库执行未成功，原因不明",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->恢复被软件删除的录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return false;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_RecordNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->恢复被软件删除的录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->恢复被软件删除的录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }
        }

        /// <summary>
        /// 软删除一个录制文件（文件不会被真正删除，真正会在24小后删除）
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SoftDeleteRecordFile(long dbId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                        .Set(x => x.Undo, true)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .Where(x => x.Id.Equals(dbId)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->SoftDeleteRecordFile->执行SQL:->{sql}");
                }

                #endregion

                var ret = ORMHelper.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                    .Set(x => x.Undo, true)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .Where(x => x.Id.Equals(dbId)).ExecuteAffrows();
                if (ret > 0)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->删除(软删除)录制文件成功->{dbId}");

                    return true;
                }

                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除(软删除)录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除(软删除)录制文件失败->{dbId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }
        }

        /// <summary>
        /// 删除一个音视频通道实例
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteVideoChannel(string mainId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (UtilsHelper.StringIsNullEx(mainId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除音视频通道实例失败->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }

            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->DeleteVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var obj = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId)).First();
                if (obj == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->删除音视频通道实例失败->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return false;
                }

                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Delete<VideoChannel>().Where(x => x.MainId.Equals(mainId)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->DeleteVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var ret = ORMHelper.Db.Delete<VideoChannel>().Where(x => x.MainId.Equals(mainId)).ExecuteAffrows() > 0
                    ? true
                    : false;
                if (ret == true)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->删除音视频通道实例成功->{mainId}");
                }
                else
                {
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->删除音视频通道实例失败->{mainId}");
                }

                return ret;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除音视频通道实例失败->{mainId}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return false;
            }
        }

        /// <summary>
        /// 添加音视频通道实例
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static VideoChannel AddVideoChannel(ReqAddVideoChannel req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (req == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (req.DeviceStreamType == DeviceStreamType.GB28181)
            {
                if (UtilsHelper.StringIsNullEx(req.DeviceId) || UtilsHelper.StringIsNullEx(req.ChannelId))
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] +
                                  "DeviceStreamType为GB28181时，DeviceId,ChannelId不能为空",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }

                req.App = "rtp";
            }

            if (req.DeviceStreamType != DeviceStreamType.GB28181)
            {
                if (UtilsHelper.StringIsNullEx(req.VideoSrcUrl))
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] +
                                  "DeviceStreamType不为GB28181时，VideoSrcUrl不能为空",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (req.MethodByGetStream != MethodByGetStream.None)
            {
                if (req.DeviceStreamType == DeviceStreamType.GB28181)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] +
                                  "MethodByGetStream不为空时，DeviceStreamType不能是GB28181",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (!UtilsHelper.StringIsNullEx(req.VideoSrcUrl))
            {
                if (req.MethodByGetStream == MethodByGetStream.None ||
                    !UtilsHelper.StringIsNullEx(req.DeviceId) ||
                    !UtilsHelper.StringIsNullEx(req.ChannelId)
                    || req.DeviceStreamType == DeviceStreamType.GB28181)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] +
                                  "VideoSrcUrl不为空时，MethodByGetStream不能为None,DeviceId,ChannelId必须为空，DeviceStreamType不能为GB28181",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (UtilsHelper.StringIsNullEx(req.IpV4Address) || UtilsHelper.StringIsNullEx(req.MediaServerId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message =
                        ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] + "IPV4Address,MediaServerId不能为空",
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_InstanceIsNull,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            string mainId = "";
            if (req.DeviceStreamType == DeviceStreamType.GB28181)
            {
                var ret = UtilsHelper.GetSSRCInfo(req.DeviceId, req.ChannelId);
                mainId = ret.Value;
            }
            else
            {
                mainId = UtilsHelper.GetDisGB28181VideoChannelMainId(req.VideoSrcUrl);
            }

            if (UtilsHelper.StringIsNullEx(req.App))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsNotEnough,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsNotEnough] + "App不能为空",
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (UtilsHelper.StringIsNullEx(req.Vhost))
            {
                req.Vhost = "__defaultVhost__";
            }

            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->AddVideoChannel->执行SQL:->{sql}");
                }

                #endregion;

                var dbobj = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Equals(mainId)).First();
                if (dbobj != null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_VideoChannelAlRedayExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelAlRedayExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (!UtilsHelper.StringIsNullEx(req.RecordPlanName))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(req.RecordPlanName)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->AddVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var recordPlan = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(req.RecordPlanName))
                    .First();
                if (recordPlan == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_RecordPlanNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordPlanNotExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (!UtilsHelper.StringIsNullEx(req.ShareDeviceId))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>()
                        .Where(x => x.ShareDeviceId.Equals(req.ShareDeviceId.Trim())).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->AddVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var channels = ORMHelper.Db.Select<VideoChannel>()
                    .Where(x => x.ShareDeviceId.Equals(req.ShareDeviceId.Trim())).ToList();
                if (channels != null && channels.Count > 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_SipClient_ShareDeviceIdAlRedayExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SipClient_ShareDeviceIdAlRedayExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            int result = 0;
            try
            {
                var tmpVideoChannel = new VideoChannel();
                tmpVideoChannel.Enabled = req.Enabled;
                tmpVideoChannel.AutoRecord = req.AutoRecord;
                tmpVideoChannel.RecordPlanName =
                    UtilsHelper.StringIsNullEx(req.RecordPlanName) ? null : req.RecordPlanName;
                tmpVideoChannel.AutoVideo = req.AutoVideo;
                tmpVideoChannel.ChannelId = UtilsHelper.StringIsNullEx(req.ChannelId) ? null : req.ChannelId;
                tmpVideoChannel.RecordSecs = req.RecordSecs;
                tmpVideoChannel.App = UtilsHelper.StringIsNullEx(req.App) ? null : req.App;
                tmpVideoChannel.Vhost = UtilsHelper.StringIsNullEx(req.Vhost) ? null : req.Vhost;
                tmpVideoChannel.ChannelName = UtilsHelper.StringIsNullEx(req.ChannelName) ? null : req.ChannelName;
                tmpVideoChannel.CreateTime = DateTime.Now;
                tmpVideoChannel.DepartmentId = UtilsHelper.StringIsNullEx(req.DepartmentId) ? null : req.DepartmentId;
                tmpVideoChannel.DepartmentName =
                    UtilsHelper.StringIsNullEx(req.DepartmentName) ? null : req.DepartmentName;
                tmpVideoChannel.DeviceId = UtilsHelper.StringIsNullEx(req.DeviceId) ? null : req.DeviceId;
                tmpVideoChannel.HasPtz = req.HasPtz;
                tmpVideoChannel.MainId = mainId;
                tmpVideoChannel.UpdateTime = DateTime.Now;
                tmpVideoChannel.DefaultRtpPort = req.DefaultRtpPort;
                tmpVideoChannel.DeviceNetworkType = req.DeviceNetworkType;
                tmpVideoChannel.DeviceStreamType = req.DeviceStreamType;
                tmpVideoChannel.IpV4Address = UtilsHelper.StringIsNullEx(req.IpV4Address) ? null : req.IpV4Address;
                tmpVideoChannel.IpV6Address = UtilsHelper.StringIsNullEx(req.IpV6Address) ? null : req.IpV6Address;
                tmpVideoChannel.MediaServerId =
                    UtilsHelper.StringIsNullEx(req.MediaServerId) ? null : req.MediaServerId;
                tmpVideoChannel.NoPlayerBreak = req.NoPlayerBreak;
                tmpVideoChannel.PDepartmentId =
                    UtilsHelper.StringIsNullEx(req.PDepartmentId) ? null : req.PDepartmentId;
                tmpVideoChannel.PDepartmentName =
                    UtilsHelper.StringIsNullEx(req.PDepartmentName) ? null : req.PDepartmentName;
                tmpVideoChannel.RtpWithTcp = req.RtpWithTcp;
                tmpVideoChannel.VideoDeviceType = req.VideoDeviceType;
                tmpVideoChannel.VideoSrcUrl = UtilsHelper.StringIsNullEx(req.VideoSrcUrl) ? null : req.VideoSrcUrl;
                tmpVideoChannel.MethodByGetStream = req.MethodByGetStream;
                tmpVideoChannel.FFmpegTemplate = req.FFmpegTemplate;
                tmpVideoChannel.IsShareChannel = req.IsShareChannel;
                tmpVideoChannel.ShareUrl = UtilsHelper.StringIsNullEx(req.ShareUrl) ? null : req.ShareUrl;
                tmpVideoChannel.ShareDeviceId =
                    UtilsHelper.StringIsNullEx(req.ShareDeviceId) ? null : req.ShareDeviceId;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && File.Exists("/etc/hostname"))
                {
                    //用于定制gdn的特定端口
                    var text = File.ReadAllText("/etc/hostname").Trim().ToLower();
                    if (text.Contains("gdn") || text.Contains("guardian") || text.Contains("rasp"))
                    {
                        if (mediaServer.RecordSec == null || mediaServer.RecordSec <= 0)
                        {
                            tmpVideoChannel.RecordSecs = 120;
                        }
                        else
                        {
                            tmpVideoChannel.RecordSecs = mediaServer.RecordSec;
                        }
                    }
                }

                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Insert(tmpVideoChannel).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->AddVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                result = ORMHelper.Db.Insert(tmpVideoChannel).ExecuteAffrows();
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (result > 0)
            {
                try
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>()
                            .Where(x => x.MainId.Equals(mainId)).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->AddVideoChannel->执行SQL:->{sql}");
                    }

                    #endregion

                    var r1 = ORMHelper.Db.Select<VideoChannel>()
                        .Where(x => x.MainId.Equals(mainId)).First();
                    if (r1 != null)
                    {
                        GCommon.Logger.Info(
                            $"[{Common.LoggerHead}]->新增音视频通道实例成功->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(r1)}");
                    }
                    else
                    {
                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}");
                    }

                    return r1;
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_DataBaseExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
            };
            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->新增音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

            return null;
        }


        /// <summary>
        /// 获取录像文件（支持分页，全表条件）
        /// 查询所有视频时长落在开始与结束时间内的视频文件
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResGetRecordFileList GetRecordFileListEx(ReqGetRecordFileList req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            bool isPageQuery = req.PageIndex != null;
            bool haveOrderBy = req.OrderBy != null;
            if (isPageQuery)
            {
                if (req.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取录制文件列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }

                if (req.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取录制文件列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }
            }

            string orderBy = "";
            if (haveOrderBy)
            {
                foreach (var order in req.OrderBy!)
                {
                    if (order != null)
                    {
                        orderBy += UtilsHelper.AddQuote(order.FieldName) + " " +
                                   Enum.GetName(typeof(OrderByDir), order.OrderByDir!) + ",";
                    }
                }

                orderBy = orderBy.TrimEnd(',');
            }

            long total = -1;
            List<RecordFile> retList = null;

            try
            {
                if (isPageQuery)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                            .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                            .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                            .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                            .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                            .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                            .WhereIf((req.StartTime != null && req.EndTime != null), x => x.EndTime >= req.StartTime)
                            .WhereIf((req.StartTime != null && req.EndTime != null), x => x.StartTime <= req.EndTime)
                            .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                            .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                                x => x.App.Equals(req.App))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                                x => x.Streamid.Equals(req.Streamid))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                                x => x.Vhost.Equals(req.Vhost))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelId),
                                x => x.ChannelId.Equals(req.ChannelId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelName),
                                x => x.ChannelName.Equals(req.ChannelName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentId),
                                x => x.DepartmentId.Equals(req.DepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentName),
                                x => x.DepartmentName.Equals(req.DepartmentName))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                                x => x.DeviceId.Equals(req.DeviceId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                                x => x.DownloadUrl.Equals(req.DownloadUrl))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                                x => x.MainId.Equals(req.MainId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.RecordDate),
                                x => x.RecordDate.Equals(req.RecordDate))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoPath),
                                x => x.VideoPath.Equals(req.VideoPath))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerId),
                                x => x.MediaServerId.Equals(req.MediaServerId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                                x => x.MediaServerIp.Equals(req.MediaServerIp))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                                x => x.PDepartmentId.Equals(req.PDepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                                x => x.PDepartmentName.Equals(req.PDepartmentName))
                            .OrderBy(orderBy)
                            .Count(out total)
                            .Page((int)req.PageIndex!, (int)req.PageSize!).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetRecordFileListEx->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                        .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                        .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                        .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                        .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                        .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                        .WhereIf((req.StartTime != null && req.EndTime != null), x => x.EndTime >= req.StartTime)
                        .WhereIf((req.StartTime != null && req.EndTime != null), x => x.StartTime <= req.EndTime)
                        .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                        .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                            x => x.App.Equals(req.App))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                            x => x.Streamid.Equals(req.Streamid))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                            x => x.Vhost.Equals(req.Vhost))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelId),
                            x => x.ChannelId.Equals(req.ChannelId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName.Equals(req.ChannelName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId),
                            x => x.DepartmentId.Equals(req.DepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName.Equals(req.DepartmentName))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                            x => x.DeviceId.Equals(req.DeviceId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                            x => x.DownloadUrl.Equals(req.DownloadUrl))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                            x => x.MainId.Equals(req.MainId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.RecordDate),
                            x => x.RecordDate.Equals(req.RecordDate))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoPath),
                            x => x.VideoPath.Equals(req.VideoPath))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerId),
                            x => x.MediaServerId.Equals(req.MediaServerId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                            x => x.MediaServerIp.Equals(req.MediaServerIp))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId.Equals(req.PDepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName.Equals(req.PDepartmentName))
                        .OrderBy(orderBy)
                        .Count(out total)
                        .Page((int)req.PageIndex!, (int)req.PageSize!)
                        .ToList();
                }
                else
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                            .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                            .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                            .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                            .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                            .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                            .WhereIf((req.StartTime != null && req.EndTime != null), x => x.EndTime >= req.StartTime)
                            .WhereIf((req.StartTime != null && req.EndTime != null), x => x.StartTime <= req.EndTime)
                            .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                            .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                                x => x.App.Equals(req.App))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                                x => x.Streamid.Equals(req.Streamid))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                                x => x.Vhost.Equals(req.Vhost))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelId),
                                x => x.ChannelId.Equals(req.ChannelId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelName),
                                x => x.ChannelName.Equals(req.ChannelName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentId),
                                x => x.DepartmentId.Equals(req.DepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentName),
                                x => x.DepartmentName.Equals(req.DepartmentName))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                                x => x.DeviceId.Equals(req.DeviceId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                                x => x.DownloadUrl.Equals(req.DownloadUrl))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                                x => x.MainId.Equals(req.MainId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.RecordDate),
                                x => x.RecordDate.Equals(req.RecordDate))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoPath),
                                x => x.VideoPath.Equals(req.VideoPath))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerId),
                                x => x.MediaServerId.Equals(req.MediaServerId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                                x => x.MediaServerIp.Equals(req.MediaServerIp))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                                x => x.PDepartmentId.Equals(req.PDepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                                x => x.PDepartmentName.Equals(req.PDepartmentName))
                            .OrderBy(orderBy).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetRecordFileListEx->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                        .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                        .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                        .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                        .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                        .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                        .WhereIf((req.StartTime != null && req.EndTime != null), x => x.EndTime >= req.StartTime)
                        .WhereIf((req.StartTime != null && req.EndTime != null), x => x.StartTime <= req.EndTime)
                        .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                        .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                            x => x.App.Equals(req.App))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                            x => x.Streamid.Equals(req.Streamid))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                            x => x.Vhost.Equals(req.Vhost))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelId),
                            x => x.ChannelId.Equals(req.ChannelId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName.Equals(req.ChannelName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId),
                            x => x.DepartmentId.Equals(req.DepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName.Equals(req.DepartmentName))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                            x => x.DeviceId.Equals(req.DeviceId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                            x => x.DownloadUrl.Equals(req.DownloadUrl))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                            x => x.MainId.Equals(req.MainId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.RecordDate),
                            x => x.RecordDate.Equals(req.RecordDate))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoPath),
                            x => x.VideoPath.Equals(req.VideoPath))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerId),
                            x => x.MediaServerId.Equals(req.MediaServerId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                            x => x.MediaServerIp.Equals(req.MediaServerIp))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId.Equals(req.PDepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName.Equals(req.PDepartmentName))
                        .OrderBy(orderBy)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取录制文件列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            ResGetRecordFileList result = new ResGetRecordFileList();
            result.RecordFileList = retList;
            if (!isPageQuery)
            {
                if (retList != null)
                {
                    total = retList.Count;
                }
                else
                {
                    total = 0;
                }
            }

            result.Total = total;
            result.Request = req;
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->获取录制文件列表成功->{JsonHelper.ToJson(req)}->{result.RecordFileList.Count}/{result.Total}");

            return result;
        }

        /// <summary>
        /// 获取录像文件（支持分页，全表条件）
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResGetRecordFileList GetRecordFileList(ReqGetRecordFileList req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            bool isPageQuery = req.PageIndex != null;
            bool haveOrderBy = req.OrderBy != null;
            if (isPageQuery)
            {
                if (req.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取录制文件列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }

                if (req.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取录制文件列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }
            }

            string orderBy = "";
            if (haveOrderBy)
            {
                foreach (var order in req.OrderBy!)
                {
                    if (order != null)
                    {
                        orderBy += UtilsHelper.AddQuote(order.FieldName) + " " +
                                   Enum.GetName(typeof(OrderByDir), order.OrderByDir!) + ",";
                    }
                }

                orderBy = orderBy.TrimEnd(',');
            }

            long total = -1;
            List<RecordFile> retList = null;

            try
            {
                if (isPageQuery)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                            .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                            .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                            .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                            .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                            .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                            .WhereIf(req.StartTime != null, x => x.StartTime >= req.StartTime)
                            .WhereIf(req.EndTime != null, x => x.EndTime <= req.EndTime)
                            .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                            .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                                x => x.App.Equals(req.App))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                                x => x.Streamid.Equals(req.Streamid))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                                x => x.Vhost.Equals(req.Vhost))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelId),
                                x => x.ChannelId.Equals(req.ChannelId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelName),
                                x => x.ChannelName.Equals(req.ChannelName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentId),
                                x => x.DepartmentId.Equals(req.DepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentName),
                                x => x.DepartmentName.Equals(req.DepartmentName))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                                x => x.DeviceId.Equals(req.DeviceId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                                x => x.DownloadUrl.Equals(req.DownloadUrl))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                                x => x.MainId.Equals(req.MainId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.RecordDate),
                                x => x.RecordDate.Equals(req.RecordDate))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoPath),
                                x => x.VideoPath.Equals(req.VideoPath))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerId),
                                x => x.MediaServerId.Equals(req.MediaServerId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                                x => x.MediaServerIp.Equals(req.MediaServerIp))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                                x => x.PDepartmentId.Equals(req.PDepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                                x => x.PDepartmentName.Equals(req.PDepartmentName))
                            .OrderBy(orderBy)
                            .Count(out total)
                            .Page((int)req.PageIndex!, (int)req.PageSize!).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetRecordFileList->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                        .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                        .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                        .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                        .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                        .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                        .WhereIf(req.StartTime != null, x => x.StartTime >= req.StartTime)
                        .WhereIf(req.EndTime != null, x => x.EndTime <= req.EndTime)
                        .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                        .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                            x => x.App.Equals(req.App))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                            x => x.Streamid.Equals(req.Streamid))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                            x => x.Vhost.Equals(req.Vhost))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelId),
                            x => x.ChannelId.Equals(req.ChannelId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName.Equals(req.ChannelName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId),
                            x => x.DepartmentId.Equals(req.DepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName.Equals(req.DepartmentName))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                            x => x.DeviceId.Equals(req.DeviceId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                            x => x.DownloadUrl.Equals(req.DownloadUrl))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                            x => x.MainId.Equals(req.MainId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.RecordDate),
                            x => x.RecordDate.Equals(req.RecordDate))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoPath),
                            x => x.VideoPath.Equals(req.VideoPath))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerId),
                            x => x.MediaServerId.Equals(req.MediaServerId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                            x => x.MediaServerIp.Equals(req.MediaServerIp))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId.Equals(req.PDepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName.Equals(req.PDepartmentName))
                        .OrderBy(orderBy)
                        .Count(out total)
                        .Page((int)req.PageIndex!, (int)req.PageSize!)
                        .ToList();
                }
                else
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                            .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                            .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                            .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                            .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                            .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                            .WhereIf(req.StartTime != null, x => x.StartTime >= req.StartTime)
                            .WhereIf(req.EndTime != null, x => x.EndTime <= req.EndTime)
                            .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                            .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                                x => x.App.Equals(req.App))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                                x => x.Streamid.Equals(req.Streamid))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                                x => x.Vhost.Equals(req.Vhost))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelId),
                                x => x.ChannelId.Equals(req.ChannelId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelName),
                                x => x.ChannelName.Equals(req.ChannelName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentId),
                                x => x.DepartmentId.Equals(req.DepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentName),
                                x => x.DepartmentName.Equals(req.DepartmentName))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                                x => x.DeviceId.Equals(req.DeviceId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                                x => x.DownloadUrl.Equals(req.DownloadUrl))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                                x => x.MainId.Equals(req.MainId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.RecordDate),
                                x => x.RecordDate.Equals(req.RecordDate))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoPath),
                                x => x.VideoPath.Equals(req.VideoPath))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerId),
                                x => x.MediaServerId.Equals(req.MediaServerId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                                x => x.MediaServerIp.Equals(req.MediaServerIp))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                                x => x.PDepartmentId.Equals(req.PDepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                                x => x.PDepartmentName.Equals(req.PDepartmentName))
                            .OrderBy(orderBy).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetRecordFileList->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<RecordFile>().Where("1=1")
                        .WhereIf(req.Deleted != null, x => x.Deleted.Equals(req.Deleted))
                        .WhereIf(req.Duration != null, x => x.Duration.Equals(req.Duration))
                        .WhereIf(req.Id != null, x => x.Id.Equals(req.Id))
                        .WhereIf(req.Undo != null, x => x.Undo.Equals(req.Undo))
                        .WhereIf(req.CreateTime != null, x => x.CreateTime >= req.CreateTime)
                        .WhereIf(req.StartTime != null, x => x.StartTime >= req.StartTime)
                        .WhereIf(req.EndTime != null, x => x.EndTime <= req.EndTime)
                        .WhereIf(req.FileSize != null, x => x.FileSize.Equals(req.FileSize))
                        .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.App),
                            x => x.App.Equals(req.App))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Streamid),
                            x => x.Streamid.Equals(req.Streamid))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                            x => x.Vhost.Equals(req.Vhost))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelId),
                            x => x.ChannelId.Equals(req.ChannelId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName.Equals(req.ChannelName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId),
                            x => x.DepartmentId.Equals(req.DepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName.Equals(req.DepartmentName))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                            x => x.DeviceId.Equals(req.DeviceId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DownloadUrl),
                            x => x.DownloadUrl.Equals(req.DownloadUrl))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                            x => x.MainId.Equals(req.MainId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.RecordDate),
                            x => x.RecordDate.Equals(req.RecordDate))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoPath),
                            x => x.VideoPath.Equals(req.VideoPath))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerId),
                            x => x.MediaServerId.Equals(req.MediaServerId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerIp),
                            x => x.MediaServerIp.Equals(req.MediaServerIp))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId.Equals(req.PDepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName.Equals(req.PDepartmentName))
                        .OrderBy(orderBy)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取录制文件列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            ResGetRecordFileList result = new ResGetRecordFileList();
            result.RecordFileList = retList;
            if (!isPageQuery)
            {
                if (retList != null)
                {
                    total = retList.Count;
                }
                else
                {
                    total = 0;
                }
            }

            result.Total = total;
            result.Request = req;
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->获取录制文件列表成功->{JsonHelper.ToJson(req)}->{result.RecordFileList.Count}/{result.Total}");

            return result;
        }

        /// <summary>
        /// 获取音视频流通道列表（支持分页，全表条件）
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResGetVideoChannelList GetVideoChannelList(ReqGetVideoChannelList req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (!UtilsHelper.StringIsNullEx(req.ChannelIdLike))
            {
                req.ChannelId = null;
            }

            if (!UtilsHelper.StringIsNullEx(req.ChannelNameLike))
            {
                req.ChannelName = null;
            }

            if (!UtilsHelper.StringIsNullEx(req.DepartmentNameLike))
            {
                req.DepartmentName = null;
            }

            if (!UtilsHelper.StringIsNullEx(req.IpV4AddressLike))
            {
                req.IpV4Address = null;
            }

            if (!UtilsHelper.StringIsNullEx(req.IpV6AddressLike))
            {
                req.IpV6Address = null;
            }

            if (!UtilsHelper.StringIsNullEx(req.VideoSrcUrlLike))
            {
                req.VideoSrcUrl = null;
            }

            if (!UtilsHelper.StringIsNullEx(req.DeviceIdLike))
            {
                req.DeviceId = null;
            }

            if (req.IncludeSubDeptartment != null && req.IncludeSubDeptartment == true)
            {
                if (string.IsNullOrEmpty(req.DepartmentId))
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] + ",条件中要求包含子部门,但条件中部门代码为空",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取音视频通道列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }

                req.DepartmentNameLike = "";
                req.DepartmentName = "";
                req.PDepartmentId = "";
                req.PDepartmentName = "";
            }

            bool isPageQuery = req.PageIndex != null;
            bool haveOrderBy = req.OrderBy != null;
            if (isPageQuery)
            {
                if (req.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取音视频通道列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }

                if (req.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取音视频通道列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }
            }

            string orderBy = "";
            if (haveOrderBy)
            {
                foreach (var order in req.OrderBy!)
                {
                    if (order != null)
                    {
                        orderBy += UtilsHelper.AddQuote(order.FieldName) + " " +
                                   Enum.GetName(typeof(OrderByDir), order.OrderByDir!) + ",";
                    }
                }

                orderBy = orderBy.TrimEnd(',');
            }

            long total = -1;
            List<VideoChannel> retList = null;


            try
            {
                if (isPageQuery)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                            .WhereIf(req.Id != null && req.Id > 0, x => x.Id.Equals(req.Id))
                            .WhereIf(req.DeviceNetworkType != null,
                                x => x.DeviceNetworkType.Equals(req.DeviceNetworkType))
                            .WhereIf(req.DeviceStreamType != null, x => x.DeviceStreamType.Equals(req.DeviceStreamType))
                            .WhereIf(req.VideoDeviceType != null, x => x.VideoDeviceType.Equals(req.VideoDeviceType))
                            .WhereIf(req.MethodByGetStream != null,
                                x => x.MethodByGetStream.Equals(req.MethodByGetStream))
                            .WhereIf(req.Enabled != null, x => x.Enabled.Equals(req.Enabled))
                            .WhereIf(req.AutoRecord != null, x => x.AutoRecord.Equals(req.AutoRecord))
                            .WhereIf(req.AutoVideo != null, x => x.AutoVideo.Equals(req.AutoVideo))
                            .WhereIf(req.CreateTime != null, x => x.CreateTime.Equals(req.CreateTime))
                            .WhereIf(req.HasPtz != null, x => x.HasPtz.Equals(req.HasPtz))
                            .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                            .WhereIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort.Equals(req.DefaultRtpPort))
                            .WhereIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak.Equals(req.NoPlayerBreak))
                            .WhereIf(req.RtpWithTcp != null, x => x.RtpWithTcp.Equals(req.RtpWithTcp))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelId),
                                x => x.ChannelId.Equals(req.ChannelId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.RecordPlanName),
                                x => x.RecordPlanName.Equals(req.RecordPlanName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.App),
                                x => x.App.Equals(req.App))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.Vhost),
                                x => x.Vhost.Equals(req.Vhost))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelName),
                                x => x.ChannelName.Equals(req.ChannelName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentId) &&
                                (req.IncludeSubDeptartment == null || req.IncludeSubDeptartment == false),
                                x => x.DepartmentId.Equals(req.DepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentName),
                                x => x.DepartmentName.Equals(req.DepartmentName))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                                x => x.DeviceId.Equals(req.DeviceId))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                                x => x.MainId.Equals(req.MainId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV4Address),
                                x => x.IpV4Address.Equals(req.IpV4Address))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV6Address),
                                x => x.IpV6Address.Equals(req.IpV6Address))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerId),
                                x => x.MediaServerId.Equals(req.MediaServerId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                                x => x.PDepartmentId.Equals(req.PDepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                                x => x.PDepartmentName.Equals(req.PDepartmentName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoSrcUrl),
                                x => x.VideoSrcUrl.Equals(req.VideoSrcUrl))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelNameLike),
                                x => x.ChannelName.Contains(req.ChannelNameLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentNameLike),
                                x => x.DepartmentName.Contains(req.DepartmentNameLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV4AddressLike),
                                x => x.IpV4Address.Contains(req.IpV4AddressLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV6AddressLike),
                                x => x.IpV6Address.Contains(req.IpV6AddressLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoSrcUrlLike),
                                x => x.VideoSrcUrl.Contains(req.VideoSrcUrlLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DeviceIdLike),
                                x => x.DeviceId.Contains(req.DeviceIdLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelIdLike),
                                x => x.ChannelId.Contains(req.ChannelIdLike))
                            .WhereIf(req.IncludeSubDeptartment != null && req.IncludeSubDeptartment == true,
                                x => x.PDepartmentId.Equals(req.DepartmentId))
                            .OrderBy(orderBy)
                            .Count(out total)
                            .Page((int)req.PageIndex!, (int)req.PageSize!).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetVideoChannelList->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                        .WhereIf(req.Id != null && req.Id > 0, x => x.Id.Equals(req.Id))
                        .WhereIf(req.DeviceNetworkType != null, x => x.DeviceNetworkType.Equals(req.DeviceNetworkType))
                        .WhereIf(req.DeviceStreamType != null, x => x.DeviceStreamType.Equals(req.DeviceStreamType))
                        .WhereIf(req.VideoDeviceType != null, x => x.VideoDeviceType.Equals(req.VideoDeviceType))
                        .WhereIf(req.MethodByGetStream != null, x => x.MethodByGetStream.Equals(req.MethodByGetStream))
                        .WhereIf(req.Enabled != null, x => x.Enabled.Equals(req.Enabled))
                        .WhereIf(req.AutoRecord != null, x => x.AutoRecord.Equals(req.AutoRecord))
                        .WhereIf(req.AutoVideo != null, x => x.AutoVideo.Equals(req.AutoVideo))
                        .WhereIf(req.CreateTime != null, x => x.CreateTime.Equals(req.CreateTime))
                        .WhereIf(req.HasPtz != null, x => x.HasPtz.Equals(req.HasPtz))
                        .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                        .WhereIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort.Equals(req.DefaultRtpPort))
                        .WhereIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak.Equals(req.NoPlayerBreak))
                        .WhereIf(req.RtpWithTcp != null, x => x.RtpWithTcp.Equals(req.RtpWithTcp))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelId),
                            x => x.ChannelId.Equals(req.ChannelId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.RecordPlanName),
                            x => x.RecordPlanName.Equals(req.RecordPlanName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.App),
                            x => x.App.Equals(req.App))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.Vhost),
                            x => x.Vhost.Equals(req.Vhost))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName.Equals(req.ChannelName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId) &&
                            (req.IncludeSubDeptartment == null || req.IncludeSubDeptartment == false),
                            x => x.DepartmentId.Equals(req.DepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName.Equals(req.DepartmentName))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                            x => x.DeviceId.Equals(req.DeviceId))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                            x => x.MainId.Equals(req.MainId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV4Address),
                            x => x.IpV4Address.Equals(req.IpV4Address))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV6Address),
                            x => x.IpV6Address.Equals(req.IpV6Address))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerId),
                            x => x.MediaServerId.Equals(req.MediaServerId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId.Equals(req.PDepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName.Equals(req.PDepartmentName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoSrcUrl),
                            x => x.VideoSrcUrl.Equals(req.VideoSrcUrl))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelNameLike),
                            x => x.ChannelName.Contains(req.ChannelNameLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentNameLike),
                            x => x.DepartmentName.Contains(req.DepartmentNameLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV4AddressLike),
                            x => x.IpV4Address.Contains(req.IpV4AddressLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV6AddressLike),
                            x => x.IpV6Address.Contains(req.IpV6AddressLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoSrcUrlLike),
                            x => x.VideoSrcUrl.Contains(req.VideoSrcUrlLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DeviceIdLike),
                            x => x.DeviceId.Contains(req.DeviceIdLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelIdLike),
                            x => x.ChannelId.Contains(req.ChannelIdLike))
                        .WhereIf(req.IncludeSubDeptartment != null && req.IncludeSubDeptartment == true,
                            x => x.PDepartmentId.Equals(req.DepartmentId))
                        .OrderBy(orderBy)
                        .Count(out total)
                        .Page((int)req.PageIndex!, (int)req.PageSize!)
                        .ToList();
                }
                else
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                            .WhereIf(req.Id != null && req.Id > 0, x => x.Id.Equals(req.Id))
                            .WhereIf(req.DeviceNetworkType != null,
                                x => x.DeviceNetworkType.Equals(req.DeviceNetworkType))
                            .WhereIf(req.DeviceStreamType != null, x => x.DeviceStreamType.Equals(req.DeviceStreamType))
                            .WhereIf(req.VideoDeviceType != null, x => x.VideoDeviceType.Equals(req.VideoDeviceType))
                            .WhereIf(req.MethodByGetStream != null,
                                x => x.MethodByGetStream.Equals(req.MethodByGetStream))
                            .WhereIf(req.Enabled != null, x => x.Enabled.Equals(req.Enabled))
                            .WhereIf(req.AutoRecord != null, x => x.AutoRecord.Equals(req.AutoRecord))
                            .WhereIf(req.AutoVideo != null, x => x.AutoVideo.Equals(req.AutoVideo))
                            .WhereIf(req.CreateTime != null, x => x.CreateTime.Equals(req.CreateTime))
                            .WhereIf(req.HasPtz != null, x => x.HasPtz.Equals(req.HasPtz))
                            .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                            .WhereIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort.Equals(req.DefaultRtpPort))
                            .WhereIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak.Equals(req.NoPlayerBreak))
                            .WhereIf(req.RtpWithTcp != null, x => x.RtpWithTcp.Equals(req.RtpWithTcp))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelId),
                                x => x.ChannelId.Equals(req.ChannelId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.RecordPlanName),
                                x => x.RecordPlanName.Equals(req.RecordPlanName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelName),
                                x => x.ChannelName.Equals(req.ChannelName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentId) &&
                                (req.IncludeSubDeptartment == null || req.IncludeSubDeptartment == false),
                                x => x.DepartmentId.Equals(req.DepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentName),
                                x => x.DepartmentName.Equals(req.DepartmentName))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                                x => x.DeviceId.Equals(req.DeviceId))
                            .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                                x => x.MainId.Equals(req.MainId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV4Address),
                                x => x.IpV4Address.Equals(req.IpV4Address))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV6Address),
                                x => x.IpV6Address.Equals(req.IpV6Address))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.MediaServerId),
                                x => x.MediaServerId.Equals(req.MediaServerId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                                x => x.PDepartmentId.Equals(req.PDepartmentId))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                                x => x.PDepartmentName.Equals(req.PDepartmentName))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoSrcUrl),
                                x => x.VideoSrcUrl.Equals(req.VideoSrcUrl))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelNameLike),
                                x => x.ChannelName.Contains(req.ChannelNameLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DepartmentNameLike),
                                x => x.DepartmentName.Contains(req.DepartmentNameLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV4AddressLike),
                                x => x.IpV4Address.Contains(req.IpV4AddressLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.IpV6AddressLike),
                                x => x.IpV6Address.Contains(req.IpV6AddressLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.VideoSrcUrlLike),
                                x => x.VideoSrcUrl.Contains(req.VideoSrcUrlLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.DeviceIdLike),
                                x => x.DeviceId.Contains(req.DeviceIdLike))
                            .WhereIf(
                                !UtilsHelper.StringIsNullEx(req.ChannelIdLike),
                                x => x.ChannelId.Contains(req.ChannelIdLike))
                            .WhereIf(req.IncludeSubDeptartment != null && req.IncludeSubDeptartment == true,
                                x => x.PDepartmentId.Equals(req.DepartmentId))
                            .OrderBy(orderBy).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetVideoChannelList->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                        .WhereIf(req.Id != null && req.Id > 0, x => x.Id.Equals(req.Id))
                        .WhereIf(req.DeviceNetworkType != null, x => x.DeviceNetworkType.Equals(req.DeviceNetworkType))
                        .WhereIf(req.DeviceStreamType != null, x => x.DeviceStreamType.Equals(req.DeviceStreamType))
                        .WhereIf(req.VideoDeviceType != null, x => x.VideoDeviceType.Equals(req.VideoDeviceType))
                        .WhereIf(req.MethodByGetStream != null, x => x.MethodByGetStream.Equals(req.MethodByGetStream))
                        .WhereIf(req.Enabled != null, x => x.Enabled.Equals(req.Enabled))
                        .WhereIf(req.AutoRecord != null, x => x.AutoRecord.Equals(req.AutoRecord))
                        .WhereIf(req.AutoVideo != null, x => x.AutoVideo.Equals(req.AutoVideo))
                        .WhereIf(req.CreateTime != null, x => x.CreateTime.Equals(req.CreateTime))
                        .WhereIf(req.HasPtz != null, x => x.HasPtz.Equals(req.HasPtz))
                        .WhereIf(req.UpdateTime != null, x => x.UpdateTime.Equals(req.UpdateTime))
                        .WhereIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort.Equals(req.DefaultRtpPort))
                        .WhereIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak.Equals(req.NoPlayerBreak))
                        .WhereIf(req.RtpWithTcp != null, x => x.RtpWithTcp.Equals(req.RtpWithTcp))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelId),
                            x => x.ChannelId.Equals(req.ChannelId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.RecordPlanName),
                            x => x.RecordPlanName.Equals(req.RecordPlanName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName.Equals(req.ChannelName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId) &&
                            (req.IncludeSubDeptartment == null || req.IncludeSubDeptartment == false),
                            x => x.DepartmentId.Equals(req.DepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName.Equals(req.DepartmentName))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                            x => x.DeviceId.Equals(req.DeviceId))
                        .WhereIf(!UtilsHelper.StringIsNullEx(req.MainId),
                            x => x.MainId.Equals(req.MainId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV4Address),
                            x => x.IpV4Address.Equals(req.IpV4Address))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV6Address),
                            x => x.IpV6Address.Equals(req.IpV6Address))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerId),
                            x => x.MediaServerId.Equals(req.MediaServerId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId.Equals(req.PDepartmentId))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName.Equals(req.PDepartmentName))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoSrcUrl),
                            x => x.VideoSrcUrl.Equals(req.VideoSrcUrl))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelNameLike),
                            x => x.ChannelName.Contains(req.ChannelNameLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentNameLike),
                            x => x.DepartmentName.Contains(req.DepartmentNameLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV4AddressLike),
                            x => x.IpV4Address.Contains(req.IpV4AddressLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.IpV6AddressLike),
                            x => x.IpV6Address.Contains(req.IpV6AddressLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.VideoSrcUrlLike),
                            x => x.VideoSrcUrl.Contains(req.VideoSrcUrlLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.DeviceIdLike),
                            x => x.DeviceId.Contains(req.DeviceIdLike))
                        .WhereIf(
                            !UtilsHelper.StringIsNullEx(req.ChannelIdLike),
                            x => x.ChannelId.Contains(req.ChannelIdLike))
                        .WhereIf(req.IncludeSubDeptartment != null && req.IncludeSubDeptartment == true,
                            x => x.PDepartmentId.Equals(req.DepartmentId))
                        .OrderBy(orderBy)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取音视频通道列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            ResGetVideoChannelList result = new ResGetVideoChannelList();
            result.VideoChannelList = retList;
            if (!isPageQuery)
            {
                if (retList != null)
                {
                    total = retList.Count;
                }
                else
                {
                    total = 0;
                }
            }

            result.Total = total;
            result.Request = req;
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->获取音视频通道列表成功->{JsonHelper.ToJson(req)}->{result.VideoChannelList.Count}/{result.Total}");

            return result;
        }

        /// <summary>
        /// 修改音视频通道参数
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static VideoChannel ModifyVideoChannel(string mainId, ReqModifyVideoChannel req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (UtilsHelper.StringIsNullEx(mainId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (!UtilsHelper.StringIsNullEx(req.MediaServerId))
            {
                if (Common.MediaServerList.FindLast(x => x.MediaServerId.Trim().Equals(req.MediaServerId.Trim())) ==
                    null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_InstanceIsNull,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (!UtilsHelper.StringIsNullEx(req.VideoSrcUrl))
            {
                if (req.DeviceStreamType != null && req.DeviceStreamType == DeviceStreamType.GB28181)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] +
                                  ",VideoSrcUrl不为空时，DeviceStreamtype不应该是GB28181",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (req.MethodByGetStream != MethodByGetStream.None)
            {
                if (UtilsHelper.StringIsNullEx(req.VideoSrcUrl) ||
                    req.DeviceStreamType == DeviceStreamType.GB28181)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] +
                                  ",MethodByGetStream不为None时VideoSrcUrl不能为空，并且DeviceStreamtype不应该是GB28181",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (!UtilsHelper.StringIsNullEx(req.DeviceId) || !UtilsHelper.StringIsNullEx(req.ChannelId))
            {
                if (req.DeviceStreamType != DeviceStreamType.GB28181)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] +
                                  ",当DeviceId及ChannleId不为空时,表示此通道为GB28181通道，则DeviceStreamType必须为GB28181",
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (req.DeviceStreamType == DeviceStreamType.GB28181)
            {
                req.App = "rtp";
            }

            if (UtilsHelper.StringIsNullEx(req.Vhost))
            {
                req.Vhost = "__defaultVhost__";
            }

            if (!UtilsHelper.StringIsNullEx(req.RecordPlanName))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(req.RecordPlanName)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ModifyVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var recordPlan = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(req.RecordPlanName))
                    .First();
                if (recordPlan == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_RecordPlanNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordPlanNotExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            VideoChannel ret = null;
            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                        .ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ModifyVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                ret = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                    .First();
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (ret == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (!UtilsHelper.StringIsNullEx(req.ShareDeviceId))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>()
                        .Where(x => x.ShareDeviceId.Equals(req.ShareDeviceId.Trim())).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ModifyVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var channels = ORMHelper.Db.Select<VideoChannel>()
                    .Where(x => x.ShareDeviceId.Equals(req.ShareDeviceId.Trim())).ToList();
                if (channels != null && channels.Count > 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_SipClient_ShareDeviceIdAlRedayExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SipClient_ShareDeviceIdAlRedayExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Update<VideoChannel>()
                        .SetIf(!UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName, req.ChannelName)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.RecordPlanName),
                            x => x.RecordPlanName, req.RecordPlanName)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId),
                            x => x.DepartmentId, req.DepartmentId)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName, req.DepartmentName)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId, req.PDepartmentId)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName, req.PDepartmentName)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.IpV4Address),
                            x => x.IpV4Address, req.IpV4Address)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.IpV6Address),
                            x => x.IpV6Address, req.IpV6Address)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.MediaServerId),
                            x => x.MediaServerId, req.MediaServerId)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.VideoSrcUrl),
                            x => x.VideoSrcUrl, req.VideoSrcUrl)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                            x => x.DeviceId, req.DeviceId)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.ChannelId),
                            x => x.ChannelId, req.ChannelId)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.App),
                            x => x.App, req.App)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                            x => x.Vhost, req.Vhost)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.FFmpegTemplate),
                            x => x.FFmpegTemplate, req.FFmpegTemplate)
                        .SetIf(req.AutoVideo != null, x => x.AutoVideo, req.AutoVideo)
                        .SetIf(req.HasPtz != null, x => x.HasPtz, req.HasPtz)
                        .SetIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort, req.DefaultRtpPort)
                        .SetIf(req.DeviceNetworkType != null, x => x.DeviceNetworkType, req.DeviceNetworkType)
                        .SetIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak, req.NoPlayerBreak)
                        .SetIf(req.RtpWithTcp != null, x => x.RtpWithTcp, req.RtpWithTcp)
                        .SetIf(req.VideoDeviceType != null, x => x.VideoDeviceType, req.VideoDeviceType)
                        .SetIf(req.AutoRecord != null, x => x.AutoRecord, req.AutoRecord)
                        .SetIf(req.Enabled != null, x => x.Enabled, req.Enabled)
                        .SetIf(req.DeviceStreamType != null, x => x.DeviceStreamType, req.DeviceStreamType)
                        .SetIf(req.MethodByGetStream != null, x => x.MethodByGetStream, req.MethodByGetStream)
                        .SetIf(req.RecordSecs != null, x => x.RecordSecs, req.RecordSecs)
                        .SetIf(req.IsShareChannel != null, x => x.IsShareChannel, req.IsShareChannel)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.ShareUrl), x => x.ShareUrl, req.ShareUrl)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.ShareDeviceId), x => x.ShareDeviceId, req.ShareDeviceId)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .Where("1=1")
                        .Where(x => x.MainId.Trim().Equals(mainId.Trim())).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ModifyVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var rAffrows = ORMHelper.Db.Update<VideoChannel>()
                    .SetIf(!UtilsHelper.StringIsNullEx(req.ChannelName),
                        x => x.ChannelName, req.ChannelName)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.RecordPlanName),
                        x => x.RecordPlanName, req.RecordPlanName)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.DepartmentId),
                        x => x.DepartmentId, req.DepartmentId)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.DepartmentName),
                        x => x.DepartmentName, req.DepartmentName)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                        x => x.PDepartmentId, req.PDepartmentId)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                        x => x.PDepartmentName, req.PDepartmentName)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.IpV4Address),
                        x => x.IpV4Address, req.IpV4Address)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.IpV6Address),
                        x => x.IpV6Address, req.IpV6Address)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.MediaServerId),
                        x => x.MediaServerId, req.MediaServerId)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.VideoSrcUrl),
                        x => x.VideoSrcUrl, req.VideoSrcUrl)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.DeviceId),
                        x => x.DeviceId, req.DeviceId)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.ChannelId),
                        x => x.ChannelId, req.ChannelId)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.App),
                        x => x.App, req.App)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.Vhost),
                        x => x.Vhost, req.Vhost)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.FFmpegTemplate),
                        x => x.FFmpegTemplate, req.FFmpegTemplate)
                    .SetIf(req.AutoVideo != null, x => x.AutoVideo, req.AutoVideo)
                    .SetIf(req.HasPtz != null, x => x.HasPtz, req.HasPtz)
                    .SetIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort, req.DefaultRtpPort)
                    .SetIf(req.DeviceNetworkType != null, x => x.DeviceNetworkType, req.DeviceNetworkType)
                    .SetIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak, req.NoPlayerBreak)
                    .SetIf(req.RtpWithTcp != null, x => x.RtpWithTcp, req.RtpWithTcp)
                    .SetIf(req.VideoDeviceType != null, x => x.VideoDeviceType, req.VideoDeviceType)
                    .SetIf(req.AutoRecord != null, x => x.AutoRecord, req.AutoRecord)
                    .SetIf(req.Enabled != null, x => x.Enabled, req.Enabled)
                    .SetIf(req.DeviceStreamType != null, x => x.DeviceStreamType, req.DeviceStreamType)
                    .SetIf(req.MethodByGetStream != null, x => x.MethodByGetStream, req.MethodByGetStream)
                    .SetIf(req.RecordSecs != null, x => x.RecordSecs, req.RecordSecs)
                    .SetIf(req.IsShareChannel != null, x => x.IsShareChannel, req.IsShareChannel)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.ShareUrl), x => x.ShareUrl, req.ShareUrl)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.ShareDeviceId), x => x.ShareDeviceId, req.ShareDeviceId)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .Where("1=1")
                    .Where(x => x.MainId.Trim().Equals(mainId.Trim())).ExecuteAffrows();
                if (rAffrows > 0)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                            .ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->ModifyVideoChannel->执行SQL:->{sql}");
                    }

                    #endregion

                    var r = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                        .First();

                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->修改音视频通道实例成功->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(r)}");

                    return r;
                }
                else
                {
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}");
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_DataBaseExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                ExceptMessage = "数据库可能异常，具体原因不明"
            };
            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->修改音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

            return null;
        }

        /// <summary>
        /// 激活音视频通道
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static VideoChannel ActiveVideoChannel(string mainId, ReqActiveVideoChannel req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            req.App = "rtp";
            if (UtilsHelper.StringIsNullEx(mainId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (UtilsHelper.StringIsNullEx(req.MediaServerId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_ParamsIsNotRight,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }


            var MediaServerInstance =
                Common.MediaServerList.FindLast(x => x.MediaServerId.Trim().Equals(req.MediaServerId.Trim()));
            if (MediaServerInstance == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_InstanceIsNull,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            VideoChannel ret = null;
            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                        .Where(x => x.Enabled.Equals(false))
                        .Where(x => x.MediaServerId.Contains("unknown_server")).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ActiveVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                ret = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                    .Where(x => x.Enabled.Equals(false))
                    .Where(x => x.MediaServerId.Contains("unknown_server")).First();
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            if (ret == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DB_VideoChannelNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_VideoChannelNotExists] + ",此设备可能已激活",
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            req.App = "rtp";
            if (!UtilsHelper.StringIsNullEx(req.RecordPlanName))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(req.RecordPlanName)).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ActiveVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var recordPlan = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(req.RecordPlanName))
                    .First();
                if (recordPlan == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DB_RecordPlanNotExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordPlanNotExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            if (!UtilsHelper.StringIsNullEx(req.ShareDeviceId))
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<VideoChannel>()
                        .Where(x => x.ShareDeviceId.Equals(req.ShareDeviceId.Trim())).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ActiveVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var channels = ORMHelper.Db.Select<VideoChannel>()
                    .Where(x => x.ShareDeviceId.Equals(req.ShareDeviceId.Trim())).ToList();
                if (channels != null && channels.Count > 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sip_SipClient_ShareDeviceIdAlRedayExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sip_SipClient_ShareDeviceIdAlRedayExists],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null;
                }
            }

            try
            {
                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Update<VideoChannel>()
                        .SetIf(!UtilsHelper.StringIsNullEx(req.ChannelName),
                            x => x.ChannelName, req.ChannelName)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.RecordPlanName),
                            x => x.RecordPlanName, req.RecordPlanName)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentId),
                            x => x.DepartmentId, req.DepartmentId)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.DepartmentName),
                            x => x.DepartmentName, req.DepartmentName)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                            x => x.PDepartmentId, req.PDepartmentId)
                        .SetIf(
                            !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                            x => x.PDepartmentName, req.PDepartmentName)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.IpV4Address),
                            x => x.IpV4Address, req.IpV4Address)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.IpV6Address),
                            x => x.IpV6Address, req.IpV6Address)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.FFmpegTemplate), x => x.FFmpegTemplate,
                            req.FFmpegTemplate)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.App), x => x.App, req.App)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.Vhost), x => x.Vhost, req.Vhost)
                        .SetIf(req.AutoVideo != null, x => x.AutoVideo, req.AutoVideo)
                        .SetIf(req.HasPtz != null, x => x.HasPtz, req.HasPtz)
                        .SetIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort, req.DefaultRtpPort)
                        .SetIf(req.DeviceNetworkType != null, x => x.DeviceNetworkType, req.DeviceNetworkType)
                        .SetIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak, req.NoPlayerBreak)
                        .SetIf(req.RtpWithTcp != null, x => x.RtpWithTcp, req.RtpWithTcp)
                        .SetIf(req.VideoDeviceType != null, x => x.VideoDeviceType, req.VideoDeviceType)
                        .SetIf(req.AutoRecord != null, x => x.AutoRecord, req.AutoRecord)
                        .SetIf(req.RecordSecs != null, x => x.RecordSecs, req.RecordSecs)
                        .SetIf(
                            (req.RecordSecs == null && MediaServerInstance.RecordSec != null &&
                             MediaServerInstance.RecordSec > 0), x => x.RecordSecs, MediaServerInstance.RecordSec)
                        .SetIf(req.IsShareChannel != null, x => x.IsShareChannel, req.IsShareChannel)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.ShareUrl), x => x.ShareUrl, req.ShareUrl)
                        .SetIf(!UtilsHelper.StringIsNullEx(req.ShareDeviceId), x => x.ShareDeviceId, req.ShareDeviceId)
                        .Set(x => x.MediaServerId, req.MediaServerId)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .Set(x => x.Enabled, true)
                        .Where("1=1")
                        .Where(x => x.MainId.Trim().Equals(mainId.Trim())).ToSql();
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ActiveVideoChannel->执行SQL:->{sql}");
                }

                #endregion

                var rAffrows = ORMHelper.Db.Update<VideoChannel>()
                    .SetIf(!UtilsHelper.StringIsNullEx(req.ChannelName),
                        x => x.ChannelName, req.ChannelName)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.RecordPlanName),
                        x => x.RecordPlanName, req.RecordPlanName)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.DepartmentId),
                        x => x.DepartmentId, req.DepartmentId)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.DepartmentName),
                        x => x.DepartmentName, req.DepartmentName)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.PDepartmentId),
                        x => x.PDepartmentId, req.PDepartmentId)
                    .SetIf(
                        !UtilsHelper.StringIsNullEx(req.PDepartmentName),
                        x => x.PDepartmentName, req.PDepartmentName)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.IpV4Address),
                        x => x.IpV4Address, req.IpV4Address)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.IpV6Address),
                        x => x.IpV6Address, req.IpV6Address)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.FFmpegTemplate), x => x.FFmpegTemplate, req.FFmpegTemplate)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.App), x => x.App, req.App)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.Vhost), x => x.Vhost, req.Vhost)
                    .SetIf(req.AutoVideo != null, x => x.AutoVideo, req.AutoVideo)
                    .SetIf(req.HasPtz != null, x => x.HasPtz, req.HasPtz)
                    .SetIf(req.DefaultRtpPort != null, x => x.DefaultRtpPort, req.DefaultRtpPort)
                    .SetIf(req.DeviceNetworkType != null, x => x.DeviceNetworkType, req.DeviceNetworkType)
                    .SetIf(req.NoPlayerBreak != null, x => x.NoPlayerBreak, req.NoPlayerBreak)
                    .SetIf(req.RtpWithTcp != null, x => x.RtpWithTcp, req.RtpWithTcp)
                    .SetIf(req.VideoDeviceType != null, x => x.VideoDeviceType, req.VideoDeviceType)
                    .SetIf(req.AutoRecord != null, x => x.AutoRecord, req.AutoRecord)
                    .SetIf(req.RecordSecs != null, x => x.RecordSecs, req.RecordSecs)
                    .SetIf(
                        (req.RecordSecs == null && MediaServerInstance.RecordSec != null &&
                         MediaServerInstance.RecordSec > 0), x => x.RecordSecs, MediaServerInstance.RecordSec)
                    .SetIf(req.IsShareChannel != null, x => x.IsShareChannel, req.IsShareChannel)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.ShareUrl), x => x.ShareUrl, req.ShareUrl)
                    .SetIf(!UtilsHelper.StringIsNullEx(req.ShareDeviceId), x => x.ShareDeviceId, req.ShareDeviceId)
                    .Set(x => x.MediaServerId, req.MediaServerId)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .Set(x => x.Enabled, true)
                    .Where("1=1")
                    .Where(x => x.MainId.Trim().Equals(mainId.Trim())).ExecuteAffrows();
                if (rAffrows > 0)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                            .ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->ActiveVideoChannel->执行SQL:->{sql}");
                    }

                    #endregion

                    var r = ORMHelper.Db.Select<VideoChannel>().Where(x => x.MainId.Trim().Equals(mainId.Trim()))
                        .First();
                    GCommon.Logger.Info(
                        $"[{Common.LoggerHead}]->激活音视频通道实例成功->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(r)}");

                    return r;
                }
                else
                {
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}");
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_DataBaseExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                ExceptMessage = "数据库可能异常，具体原因不明"
            };
            GCommon.Logger.Warn(
                $"[{Common.LoggerHead}]->激活音视频通道实例失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

            return null;
        }

        /// <summary>
        /// 获取未激活视频通道列表（支持分页）
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResGetWaitForActiveVideoChannelList GetWaitForActiveVideoChannelList(ReqPaginationBase req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            bool isPageQuery = req.PageIndex != null;
            bool haveOrderBy = req.OrderBy != null;
            if (isPageQuery)
            {
                if (req.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取未激活的音视频通道实例列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }

                if (req.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_DataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseLimited],
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取未激活的音视频通道实例列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                    return null!;
                }
            }

            string orderBy = "";
            if (haveOrderBy)
            {
                foreach (var order in req.OrderBy!)
                {
                    if (order != null)
                    {
                        orderBy += UtilsHelper.AddQuote(order.FieldName) + " " +
                                   Enum.GetName(typeof(OrderByDir), order.OrderByDir!) + ",";
                    }
                }

                orderBy = orderBy.TrimEnd(',');
            }

            long total = -1;
            List<VideoChannel> retList = null!;
            try
            {
                if (!isPageQuery)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                            .Where(x => x.Enabled == false).Where(x => x.MediaServerId.Contains("unknown_server"))
                            .OrderBy(orderBy).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetWaitForActiveVideoChannelList->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                        .Where(x => x.Enabled == false).Where(x => x.MediaServerId.Contains("unknown_server"))
                        .OrderBy(orderBy)
                        .ToList();
                }
                else
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                            .Where(x => x.Enabled == false).Where(x => x.MediaServerId.Contains("unknown_server"))
                            .OrderBy(orderBy)
                            .Count(out total)
                            .Page((int)req.PageIndex!, (int)req.PageSize!).ToSql();
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->GetWaitForActiveVideoChannelList->执行SQL:->{sql}");
                    }

                    #endregion

                    retList = ORMHelper.Db.Select<VideoChannel>().Where("1=1")
                        .Where(x => x.Enabled == false).Where(x => x.MediaServerId.Contains("unknown_server"))
                        .OrderBy(orderBy)
                        .Count(out total)
                        .Page((int)req.PageIndex!, (int)req.PageSize!)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_DataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept],
                    ExceptMessage = ex.Message,
                    ExceptStackTrace = ex.StackTrace,
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->获取未激活的音视频通道实例列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs, Formatting.Indented)}");

                return null;
            }

            ResGetWaitForActiveVideoChannelList result = new ResGetWaitForActiveVideoChannelList();
            result.VideoChannelList = retList;
            if (!isPageQuery)
            {
                if (retList != null)
                {
                    total = retList.Count;
                }
                else
                {
                    total = 0;
                }
            }

            result.Total = total;
            result.Request = req;
            GCommon.Logger.Info(
                $"[{Common.LoggerHead}]->获取未激活的音视频通道实例列表成功->{JsonHelper.ToJson(req)}->{result.VideoChannelList.Count}/{result.Total}");

            return result;
        }

        /// <summary>
        /// 通过MediaServerId获取流媒体服务器实例
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ServerInstance GetMediaServerByMediaServerId(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var r = Common.MediaServerList.FindLast(x => x.MediaServerId.Trim().Equals(mediaServerId.Trim()));
            if (r == null)
            {
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取流媒体服务器失败->{mediaServerId}->结果为空");
            }
            else
            {
                GCommon.Logger.Info($"[{Common.LoggerHead}]->获取流媒体服务器成功->{mediaServerId}->{JsonHelper.ToJson(r)}");
            }

            return r;
        }

        /// <summary>
        /// 获取流媒体服务器列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<ServerInstance> GetMediaServerList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            GCommon.Logger.Info($"[{Common.LoggerHead}]->获取流媒体服务器列表成功->{Common.MediaServerList.Count}");

            return Common.MediaServerList;
        }

        /// <summary>
        /// 检查流媒体服务器情况
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ServerInstance CheckMediaServer(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_InstanceIsNull,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->检查流媒体服务器状态失败->{mediaServerId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            if (!mediaServer.IsKeeperRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_AKStreamKeeperNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_AKStreamKeeperNotRunning],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->检查流媒体服务器状态失败->{mediaServerId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            if (!mediaServer.IsMediaServerRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_NotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_NotRunning],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->检查流媒体服务器状态失败->{mediaServerId}->{JsonHelper.ToJson(rs)}");

                return null;
            }

            GCommon.Logger.Debug(
                $"[{Common.LoggerHead}]->检查流媒体服务器状态成功->{mediaServerId}->{JsonHelper.ToJson(mediaServer)}");
            return mediaServer;
        }
    }
}