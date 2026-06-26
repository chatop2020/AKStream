using System;
using System.Text.Json.Serialization;
using FreeSql.DataAnnotations;

namespace LibCommon.Structs.DBModels
{
    /// <summary>
    /// 录制计划详细计划的结构，是数据库DvrDayTimeRange表的字段映射
    /// </summary>
    [Serializable]
    [Table(Name = "RecordPlanRange")]
    /// <summary>
    /// 用于每周的记录时间
    /// </summary>
    public class RecordPlanRange
    {
        /// <summary>
        /// 数据库主键
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// 计划任务表的主键
        /// </summary>
        public int RecordPlanId { get; set; }

        /// <summary>
        /// 星期n枚举
        /// </summary>
        [Column(MapType = typeof(string))]
        public DayOfWeek WeekDay { get; set; }

        /// <summary>
        /// 录制开始时间（只取时间部分）
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 录制结束时间（只取时间部分）
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}