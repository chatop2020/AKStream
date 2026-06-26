using System;

namespace LibCommon.Structs
{
    [Serializable]
    public class CutMergeStruct
    {
        private string? _cutEndPos;
        private string? _cutStartPos;
        private long? _dbId;
        private long? _duration;
        private DateTime? _endTime;
        private string? _filePath;
        private long? _fileSize;
        private DateTime? _startTime;

        public long? DbId
        {
            get => _dbId;
            set => _dbId = value;
        }

        public string? FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }


        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        public long? FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public string? CutStartPos
        {
            get => _cutStartPos;
            set => _cutStartPos = value;
        }

        public string? CutEndPos
        {
            get => _cutEndPos;
            set => _cutEndPos = value;
        }
    }
}