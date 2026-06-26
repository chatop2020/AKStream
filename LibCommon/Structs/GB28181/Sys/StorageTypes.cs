using System;

namespace LibCommon.Structs.GB28181.Sys
{
    public enum StorageTypes
    {
        Unknown,
        MSSQL,
        Postgresql,
        MySQL,
        Oracle,
        XML,
        DBLinqMySQL,
        DBLinqPostgresql,
        SimpleDBLinq,
        SQLLinqMySQL,
        SQLLinqPostgresql,
        SQLLinqMSSQL,
        SQLLinqOracle,
        SQLite
    }

    public class StorageTypesConverter
    {
        public static StorageTypes GetStorageType(string storageType)
        {
            try
            {
                return (StorageTypes)Enum.Parse(typeof(StorageTypes), storageType, true);
            }
            catch
            {
                return StorageTypes.Unknown;
            }
        }
    }
}