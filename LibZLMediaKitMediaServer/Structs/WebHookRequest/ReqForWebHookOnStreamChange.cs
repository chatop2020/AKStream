using System;
using System.Collections.Generic;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest
{
    [Serializable]
    public class ReqForWebHookOnStreamChange
    {
        private int? _aliveSecond;
        private string? _app;
        private long? _bytesSpeed;
        private long? _createStamp;
        private bool? _isRecordingHLS;
        private bool? _isRecordingMP4;
        private string? _mediaServerId;
        private OriginSock? _originSock;
        private OriginType? _originType;
        private string? _originTypeStr;
        private string? _originUrl;
        private int? _readerCount;
        private bool? _regist;
        private string? _schema;
        private string? _stream;
        private int? _totalReaderCount;
        private List<TracksItem>? _tracks;
        private string? _vhost;


        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public bool? Regist
        {
            get => _regist;
            set => _regist = value;
        }

        public int? AliveSecond
        {
            get => _aliveSecond;
            set => _aliveSecond = value;
        }

        public long? BytesSpeed
        {
            get => _bytesSpeed;
            set => _bytesSpeed = value;
        }

        public long? CreateStamp
        {
            get => _createStamp;
            set => _createStamp = value;
        }

        public OriginSock? OriginSock
        {
            get => _originSock;
            set => _originSock = value;
        }

        public OriginType? OriginType
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

        public int? ReaderCount
        {
            get => _readerCount;
            set => _readerCount = value;
        }

        public int? TotalReaderCount
        {
            get => _totalReaderCount;
            set => _totalReaderCount = value;
        }

        public List<TracksItem>? Tracks
        {
            get => _tracks;
            set => _tracks = value;
        }

        public bool? IsRecordingHLS
        {
            get => _isRecordingHLS;
            set => _isRecordingHLS = value;
        }

        public bool? IsRecordingMP4
        {
            get => _isRecordingMP4;
            set => _isRecordingMP4 = value;
        }
    }
}