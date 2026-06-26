using System;
using System.Collections.Generic;
using LibCommon;
using LibCommon.Structs.DBModels;

namespace AKStreamWeb.Services
{
    public static class SipClientService
    {
        /// <summary>
        /// 获取可以共享通道的数据列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<VideoChannel> GetShareChannelList(out ResponseStruct rs)
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
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.IsShareChannel.Equals(true))
                        .Where(x => x.Enabled.Equals(true)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->GetShareChannelList->执行SQL:->{sql}");
                }

                #endregion

                return ORMHelper.Db.Select<VideoChannel>().Where(x => x.IsShareChannel.Equals(true))
                    .Where(x => x.Enabled.Equals(true)).ToList();
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
                return null;
            }
        }

        /// <summary>
        /// 获取可共享通道数量
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int ShareChannelSumCount(out ResponseStruct rs)
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
                    var sql = ORMHelper.Db.Select<VideoChannel>().Where(x => x.IsShareChannel.Equals(true))
                        .Where(x => x.Enabled.Equals(true)).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->ShareChannelSumCount->执行SQL:->{sql}");
                }

                #endregion

                return (int)ORMHelper.Db.Select<VideoChannel>().Where(x => x.IsShareChannel.Equals(true))
                    .Where(x => x.Enabled.Equals(true))
                    .Count();
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
                return -1;
            }
        }
    }
}