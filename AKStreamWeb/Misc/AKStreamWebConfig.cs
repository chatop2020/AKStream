using System;

namespace AKStreamWeb.Misc
{
    [Serializable]
    public class AKStreamWebConfig
    {
        private bool _mediaServerFirstToRestart = true;
        private string _ormConnStr;
        private string _dbType;
        private ushort _webApiPort = 5800;
        private string _accessKey;
        private int _httpClientTimeoutSec;

        /// <summary>
        /// 流媒体服务器首次注册是否要求其重新mediaserver
        /// </summary>
        public bool MediaServerFirstToRestart
        {
            get => _mediaServerFirstToRestart;
            set => _mediaServerFirstToRestart = value;
        }


        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType
        {
            get => _dbType;
            set => _dbType = value;
        }


        /// <summary>
        /// 数据库的连接串
        /// </summary>
        public string OrmConnStr
        {
            get => _ormConnStr;
            set => _ormConnStr = value;
        }

        /// <summary>
        /// WebApi端口
        /// </summary>
        public ushort WebApiPort
        {
            get => _webApiPort;
            set => _webApiPort = value;
        }

        /// <summary>
        /// 访问webapi的密钥
        /// </summary>
        public string AccessKey
        {
            get => _accessKey;
            set => _accessKey = value;
        }

        /// <summary>
        /// http客户端超时时间（秒）
        /// </summary>
        public int HttpClientTimeoutSec
        {
            get => _httpClientTimeoutSec;
            set => _httpClientTimeoutSec = value;
        }
    }
}