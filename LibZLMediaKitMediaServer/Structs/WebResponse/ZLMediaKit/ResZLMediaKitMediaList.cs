using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class originSock
    {
        private string? _identifier;
        private string? _local_ip;
        private ushort? _local_port;
        private string? _peer_ip;
        private ushort? _peer_port;

        public string? Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public string? Local_Ip
        {
            get => _local_ip;
            set => _local_ip = value;
        }

        public ushort? Local_Port
        {
            get => _local_port;
            set => _local_port = value;
        }

        public string? Peer_Ip
        {
            get => _peer_ip;
            set => _peer_ip = value;
        }

        public ushort? Peer_Port
        {
            get => _peer_port;
            set => _peer_port = value;
        }
    }

    [Serializable]
    public enum originType
    {
        unknown = 0,
        rtmp_push = 1,
        rtsp_push = 2,
        rtp_push = 3,
        pull = 4,
        ffmpeg_pull = 5,
        mp4_vod = 6,
        device_chn = 7
    }

    [Serializable]
    public class TracksItem
    {
        private int? _channels;
        private int? _codec_id;
        private string? _codec_id_name;
        private int? _codec_type;
        private string? _ready;
        private int? _sample_bit;
        private int? _sample_rate;
        private int? _fps;
        private int? _width;
        private int? _height;


        [JsonProperty("channels")]
        public int? Channels
        {
            get => _channels;
            set => _channels = value;
        }

        [JsonProperty("codec_id")]
        public int? Codec_Id
        {
            get => _codec_id;
            set => _codec_id = value;
        }

        [JsonProperty("codec_id_name")]
        public string? Codec_Id_Name
        {
            get => _codec_id_name;
            set => _codec_id_name = value;
        }

        [JsonProperty("codec_type")]
        public int? Codec_Type
        {
            get => _codec_type;
            set => _codec_type = value;
        }

        [JsonProperty("ready")]
        public string? Ready
        {
            get => _ready;
            set => _ready = value;
        }

        [JsonProperty("sample_bit")]
        public int? Sample_Bit
        {
            get => _sample_bit;
            set => _sample_bit = value;
        }

        [JsonProperty("sample_rate")]
        public int? Sample_Rate
        {
            get => _sample_rate;
            set => _sample_rate = value;
        }

        public int? Fps
        {
            get => _fps;
            set => _fps = value;
        }

        public int? Width
        {
            get => _width;
            set => _width = value;
        }

        public int? Height
        {
            get => _height;
            set => _height = value;
        }
    }

    [Serializable]
    public class MediaDataItem
    {
        private int? _aliveSecond;
        private int? _bytesSpeed;
        private long? _createStamp;
        private string? _app;
        private int? _readerCount;
        private string? _schema;
        private string? _stream;
        private int? _totalReaderCount;
        private string? _vhost;
        private originSock? _originSock;
        private originType? _originType;
        private string? _originTypeStr;
        private string? _originUrl;
        private List<TracksItem> _tracks;

        [JsonProperty("createStamp")]
        public long? CreateStamp
        {
            get => _createStamp;
            set => _createStamp = value;
        }

        [JsonProperty("bytesSpeed")]
        public int? BytesSpeed
        {
            get => _bytesSpeed;
            set => _bytesSpeed = value;
        }

        [JsonProperty("aliveSecond")]
        public int? AliveSecond
        {
            get => _aliveSecond;
            set => _aliveSecond = value;
        }

        [JsonProperty("app")]
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        [JsonProperty("readerCount")]
        public int? ReaderCount
        {
            get => _readerCount;
            set => _readerCount = value;
        }

        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        [JsonProperty("stream")]
        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        [JsonProperty("totalReaderCount")]
        public int? TotalReaderCount
        {
            get => _totalReaderCount;
            set => _totalReaderCount = value;
        }

        [JsonProperty("vhost")]
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public originSock? OriginSock
        {
            get => _originSock;
            set => _originSock = value;
        }

        public originType? OriginType
        {
            get => _originType;
            set => _originType = value;
        }

        public string? OriginTypeStr
        {
            get => _originTypeStr;
            set => _originTypeStr = value;
        }

        public string? OriginUrl
        {
            get => _originUrl;
            set => _originUrl = value;
        }

        [JsonProperty("tracks")]
        public List<TracksItem> Tracks
        {
            get => _tracks;
            set => _tracks = value;
        }
    }

    [Serializable]
    public class ResZLMediaKitMediaList : ResZLMediaKitResponseBase
    {
        private List<MediaDataItem> _data;

        [JsonProperty("data")]
        public List<MediaDataItem> Data
        {
            get => _data;
            set => _data = value;
        }
    }
}