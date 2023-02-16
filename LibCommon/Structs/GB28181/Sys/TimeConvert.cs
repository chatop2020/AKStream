using System;

namespace LibCommon.Structs.GB28181.Sys
{
    /// <summary>
    /// 日期/时间戳转换
    /// </summary>
    public static class TimeConvert
    {
        /// <summary>
        /// 日期类型转换为时间戳
        /// 返回自1970年以来的秒数
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static uint DateToTimeStamp(DateTime date)
        {
            return (uint)((date.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
        }
    }
}