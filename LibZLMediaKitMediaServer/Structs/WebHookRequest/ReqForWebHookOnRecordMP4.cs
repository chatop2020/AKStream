namespace LibZLMediaKitMediaServer.Structs.WebHookRequest
{
    public class ReqForWebHookOnRecordMP4
    {
        private string? _app;
        private string? _file_Name;
        private string? _file_Path;
        private long? _file_Size;
        private string? _folder;
        private string? _mediaServerId;
        private long? _start_Time;
        private string? _stream;
        private decimal? _time_len;
        private string? _url;
        private string? _vhost;

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? File_Name
        {
            get => _file_Name;
            set => _file_Name = value;
        }

        public string? File_Path
        {
            get => _file_Path;
            set => _file_Path = value;
        }

        public string? Folder
        {
            get => _folder;
            set => _folder = value;
        }

        public long? File_Size
        {
            get => _file_Size;
            set => _file_Size = value;
        }

        public long? Start_Time
        {
            get => _start_Time;
            set => _start_Time = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public string? Url
        {
            get => _url;
            set => _url = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public decimal? Time_Len
        {
            get => _time_len;
            set => _time_len = value;
        }
    }
}