using System;

namespace LibCommon.Structs.GB28181.XML
{
    [Serializable]
    public class RecordInfoEx
    {
        private string _channelId;
        private string _deviceId;
        private RecordInfo _recordInfo;
        private int _sn;
        private int _tatolNum;

        public RecordInfo RecordInfo
        {
            get => _recordInfo;
            set => _recordInfo = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string ChannelId
        {
            get => _channelId;
            set => _channelId = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int TatolNum
        {
            get => _tatolNum;
            set => _tatolNum = value;
        }

        public int Sn
        {
            get => _sn;
            set => _sn = value;
        }
    }
}