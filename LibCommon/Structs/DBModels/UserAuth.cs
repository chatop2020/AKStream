using System;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace LibCommon.Structs.DBModels;

/// <summary>
/// 用户鉴权表
/// </summary>
[Serializable]
[Table(Name = "UserAuth")]
[Index("idx_uah_msid", "MediaServerId", false)]
[Index("idx_uah_puser", "Username", false)]
/// <summary>
/// 用于每周的记录时间
/// </summary>
public class UserAuth
{
    /// <summary>
    /// 数据库主键
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = true)]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// 流媒体服务器id
    /// </summary>
    public string MediaServerId { get; set; }

    /// <summary>
    /// projectName
    /// 为md5的盐，也是on_rtsp_realm的值
    /// </summary>

    public string Username { get; set; }

    /// <summary>
    /// 密码  MD5（Username:ProjectName:Password）
    /// 其中ProjectName为md5的盐，也是on_rtsp_realm的值
    /// </summary>
    public string Password { get; set; }
}