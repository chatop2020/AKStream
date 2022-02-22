using System;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;
using LibCommon.Structs.WebResponse;
using LibLogger;

namespace AKStreamWeb.Services
{
    public static class SystemService
    {
       
        /// <summary>
        /// 获取日志级别
        /// </summary>
        /// <param name="level"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string GetLoggerLevel(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            return  GCommon.Logger.GetLogLevel();
        }
        
        /// <summary>
        /// 获取系统性能信息
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static PerformanceInfo GetSystemPerformanceInfo(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            TimeSpan ts = DateTime.Now.Subtract(Common.StartupDateTime);
            Common.WebPerformanceInfo.UpTimeSec = ts.TotalSeconds;
             GCommon.Logger.Info($"[{Common.LoggerHead}]->获取系统性能信息成功->{JsonHelper.ToJson(Common.WebPerformanceInfo)}");

            return Common.WebPerformanceInfo;
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResGetDepartmentInfo GetDeptartmentInfoList(ReqGetDepartmentInfo req, out ResponseStruct rs)
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
                 GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取部门信息列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs)}");
                return null;
            }

            if (req.IncludeSubDepartment != null && req.IncludeSubDepartment == true)
            {
                if (string.IsNullOrEmpty(req.DepartmentId) || !req.DepartmentId.ToLower().Trim().Equals("string"))
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Sys_ParamsIsNotRight,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight] + ",条件要求包含下级部门数据，但部门代码条件为空",
                    };
                     GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->获取部门信息列表失败->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(rs)}");

                    return null;
                }
            }

            var ret = ORMHelper.Db.Select<VideoChannel>().Where("1=1").WhereIf(req.IncludeSubDepartment == true,
                    x => x.PDepartmentId.Equals(req.DepartmentId))
                .WhereIf(
                    (req.IncludeSubDepartment == null || req.IncludeSubDepartment == false) &&
                    !string.IsNullOrEmpty(req.DepartmentId), x => x.DepartmentId.Equals(req.DepartmentId))
                .ToList<DepartmentInfo>();
            if (ret != null)
            {
                 GCommon.Logger.Info($"[{Common.LoggerHead}]->获取部门信息列表成功->{JsonHelper.ToJson(req)}->{JsonHelper.ToJson(ret)}");
            }
            else
            {
                 GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取部门信息列表失败->{JsonHelper.ToJson(req)}->结果为空");
            }

            return new ResGetDepartmentInfo()
            {
                DepartmentInfoList = ret,
            };
        }
    }
}