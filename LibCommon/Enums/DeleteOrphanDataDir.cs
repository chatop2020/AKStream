using System;

namespace LibCommon.Enums;

public class DeleteOrphanDataDir
{
    /// <summary>
    /// 清理方向
    /// </summary>
    [Serializable]
    public enum DataDir
    {
        MySql = 0,
        Disk = 1,
    }
}