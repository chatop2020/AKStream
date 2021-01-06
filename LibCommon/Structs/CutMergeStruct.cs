using System;

namespace LibCommon.Structs
{
    [Serializable]
    public class CutMergeStruct
    {
        private long? _dbId;
        private string? _filePath;
        private DateTime? _startTime;
        private DateTime? _endTime;
        private long? _fileSize;
        private long? _duration;
        private string? _cutStartPos;
        private string? _cutEndPos;

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