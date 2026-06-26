using System;

namespace LibCommon.Structs
{
    [Serializable]
    public class IPInfo
    {
        private string _ipV4 = null!;
        private string _ipV6 = null!;

        public string IpV4
        {
            get => _ipV4;
            set => _ipV4 = value;
        }

        public string IpV6
        {
            get => _ipV6;
            set => _ipV6 = value;
        }
    }
}