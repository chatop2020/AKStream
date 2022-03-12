using System.Diagnostics;
using FreeSql;
using FreeSql.Internal;

namespace LibCommon
{
    public class ORMHelper
    {
        public static IFreeSql Db = null!;

        public ORMHelper(string dbConnStr, string dbType)
        {
            if (Db == null)
            {
                if (DataType.TryParse(dbType, out DataType dt))
                {
                    Db = new FreeSqlBuilder()
                        .UseConnectionString(dt, dbConnStr)
                        .UseMonitorCommand(cmd => Trace.WriteLine($"线程：{cmd.CommandText}\r\n"))
                        .UseAutoSyncStructure(true) //自动创建、迁移实体表结构
                        .UseNoneCommandParameter(true)
                        .UseNameConvert(NameConvertType.ToLower)
                        .Build();
                }
            }
        }
    }
}