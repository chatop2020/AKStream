using System.Configuration;

namespace LibCommon.Structs.GB28181.Sys
{
    public class SIPSorceryConfiguration
    {
        public const string PERSISTENCE_STORAGETYPE_KEY = "DBStorageType";
        public const string PERSISTENCE_STORAGECONNSTR_KEY = "DBConnStr";

        public SIPSorceryConfiguration()
        {
            PersistenceStorageType = (ConfigurationManager.AppSettings[PERSISTENCE_STORAGETYPE_KEY] != null)
                ? StorageTypesConverter.GetStorageType(ConfigurationManager.AppSettings[PERSISTENCE_STORAGETYPE_KEY])
                : StorageTypes.Unknown;
            PersistenceConnStr = ConfigurationManager.AppSettings[PERSISTENCE_STORAGECONNSTR_KEY];
        }

        public StorageTypes PersistenceStorageType { get; private set; }
        public string PersistenceConnStr { get; private set; }

        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}