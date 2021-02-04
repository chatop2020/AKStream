using System;
using System.Collections.Generic;
using LibCommon;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;

namespace AKStreamWeb.Services
{
    /// <summary>
    /// 录制计划的模板
    /// </summary>
    public static class RecordPlanService
    {
        /// <summary>
        /// 通过id删除一个录制计划
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteRecordPlanByName(string name, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (UtilsHelper.StringIsNullEx(name))
            {
                rs.Code = ErrorNumber.Sys_ParamsIsNotRight;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight];
                return false;
            }

            List<RecordPlan> retSelect = null!;
            int retDelete = -1;

            retSelect = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(name)).ToList();
            retDelete = ORMHelper.Db.Delete<RecordPlan>().Where(x => x.Name.Equals(name)).ExecuteAffrows();


            if (retDelete > 0)
            {
                foreach (var select in retSelect)
                {
                    ORMHelper.Db.Delete<RecordPlanRange>().Where(x => x.RecordPlanId == select.Id)
                        .ExecuteAffrows();
                }


                return true;
            }


            rs.Code = ErrorNumber.Sys_DB_RecordPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordPlanNotExists];
            return false;
        }

        /// <summary>
        /// 启用或停止一个录制计划
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnOrOffRecordPlanByName(string name, bool enable, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (UtilsHelper.StringIsNullEx(name))
            {
                rs.Code = ErrorNumber.Sys_ParamsIsNotRight;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight];
                return false;
            }


            var retUpdate = ORMHelper.Db.Update<RecordPlan>().Set(x => x.Enable, enable)
                .Where(x => x.Name.Equals(name))
                .ExecuteAffrows();
            if (retUpdate > 0)
                return true;

            rs.Code = ErrorNumber.Sys_DB_RecordPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordPlanNotExists];
            return false;
        }


        public static List<RecordPlan> GetRecordPlanList(string? name, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            /*联同子类一起查出*/
            return ORMHelper.Db.Select<RecordPlan>().IncludeMany(a => a.TimeRangeList)
                .WhereIf(!UtilsHelper.StringIsNullEx(name),
                    x => x.Name.Equals(name))
                .ToList();
            /*联同子类一起查出*/
        }


        /// <summary>
        /// 修改dvrplan
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SetRecordPlanByName(string name, ReqSetRecordPlan sdp, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.Sys_ParamsIsNotRight;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight];
                        return false;
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.Sys_RecordPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_RecordPlanTimeLimitExcept];

                        return false;
                    }
                }
            }

            try
            {
                RecordPlan retSelect = null!;
                int retDelete = -1;

                retSelect = ORMHelper.Db.Select<RecordPlan>().Where(x => x.Name.Equals(name)).First();
                retDelete = ORMHelper.Db.Delete<RecordPlan>().Where(x => x.Name.Equals(name)).ExecuteAffrows();


                if (retDelete > 0)
                {
                    ORMHelper.Db.Delete<RecordPlanRange>()
                        .Where(x => x.RecordPlanId == retSelect.Id).ExecuteAffrows();


                    var retCreate = CreateRecordPlan(sdp, out rs); //创建新的dvr
                    if (retCreate)
                    {
                        return true;
                    }

                    return false;
                }

                rs.Code = ErrorNumber.Sys_DB_RecordPlanNotExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordPlanNotExists];
                return false;
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.Sys_DataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept];
                rs.ExceptMessage = ex.Message;
                rs.ExceptStackTrace = ex.StackTrace;

                return false;
            }
        }


        /// <summary>
        /// 创建一个录制计划
        /// </summary>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool CreateRecordPlan(ReqSetRecordPlan sdp, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.Sys_ParamsIsNotRight;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight];
                        return false;
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.Sys_RecordPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_RecordPlanTimeLimitExcept];

                        return false;
                    }
                }
            }

            RecordPlan retSelect = null!;

            retSelect = ORMHelper.Db.Select<RecordPlan>().Where(x =>
                    x.Name.Equals(sdp.Name))
                .First();


            if (retSelect != null)
            {
                rs.Code = ErrorNumber.Sys_DB_RecordPlanAlreadyExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DB_RecordPlanAlreadyExists];

                return false;
            }

            try
            {
                RecordPlan tmpStream = new RecordPlan();

                tmpStream.Enable = sdp.Enable;
                tmpStream.Name = sdp.Name;
                tmpStream.LimitDays = sdp.LimitDays;
                tmpStream.LimitSpace = sdp.LimitSpace;
                tmpStream.Describe = sdp.Describe;
                tmpStream.OverStepPlan = sdp.OverStepPlan;
                tmpStream.TimeRangeList = new List<RecordPlanRange>();
                if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
                {
                    foreach (var tmp in sdp.TimeRangeList)
                    {
                        tmpStream.TimeRangeList.Add(new RecordPlanRange()
                        {
                            EndTime = tmp.EndTime,
                            StartTime = tmp.StartTime,
                            WeekDay = tmp.WeekDay,
                        });
                    }
                }

                /*联同子类一起插入*/
                var repo = ORMHelper.Db.GetRepository<RecordPlan>();
                repo.DbContextOptions.EnableAddOrUpdateNavigateList = true; //需要手工开启
                var ret = repo.Insert(tmpStream);
                /*联同子类一起插入*/
                if (ret != null)
                {
                    return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.Sys_DataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_DataBaseExcept];
                rs.ExceptMessage = ex.Message;
                rs.ExceptStackTrace = ex.StackTrace;

                return false;
            }
        }
    }
}