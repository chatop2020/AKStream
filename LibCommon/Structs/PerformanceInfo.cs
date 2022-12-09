using System;
using System.Collections.Generic;

namespace LibCommon.Structs
{
    [Serializable]
    public class PerformanceInfo
    {
        private string _architecture = null!;
        private int _cpuCores;
        private double _cpuLoad;
        private List<DriveInfoDiy> _driveInfo = null!;
        private string _frameworkVersion = null!;
        private MemoryInfo _memoryInfo = null!;
        private NetWorkStat _netWorkStat = null!;
        private string _osName = null!;
        private string _systemType = null!;
        private DateTime _updateTime;
        private double? _upTimeSec;


        public string SystemType
        {
            get => _systemType;
            set => _systemType = value;
        }

        public string Architecture
        {
            get => _architecture;
            set => _architecture = value;
        }

        public string OsName
        {
            get => _osName;
            set => _osName = value;
        }

        public string FrameworkVersion
        {
            get => _frameworkVersion;
            set => _frameworkVersion = value;
        }

        public int CpuCores
        {
            get => _cpuCores;
            set => _cpuCores = value;
        }

        public MemoryInfo MemoryInfo
        {
            get => _memoryInfo;
            set => _memoryInfo = value;
        }

        public double CpuLoad
        {
            get => _cpuLoad;
            set => _cpuLoad = value;
        }

        public NetWorkStat NetWorkStat
        {
            get => _netWorkStat;
            set => _netWorkStat = value;
        }

        public List<DriveInfoDiy> DriveInfo
        {
            get => _driveInfo;
            set => _driveInfo = value;
        }

        public DateTime UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        /// <summary>
        /// 运行时长（秒）
        /// </summary>
        public double? UpTimeSec
        {
            get => _upTimeSec;
            set => _upTimeSec = value;
        }
    }

    [Serializable]
    public class MemoryInfo
    {
        private ulong _free;
        private double _freePercent;
        private ulong _total;
        private DateTime _updateTime;
        private ulong _used;

        public ulong Total
        {
            get => _total;
            set => _total = value;
        }

        public ulong Used
        {
            get => _used;
            set => _used = value;
        }

        public ulong Free
        {
            get => _free;
            set => _free = value;
        }

        public double FreePercent
        {
            get => _freePercent;
            set => _freePercent = value;
        }

        public DateTime UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }
    }

    [Serializable]
    public class DriveInfoDiy
    {
        private double? _free;
        private double? _freePercent;
        private bool? _isReady;
        private string? _name;
        private double? _total;
        private DateTime? _updateTime;
        private double? _used;


        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        public bool? IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }

        public double? Total
        {
            get => _total;
            set => _total = value;
        }

        public double? Used
        {
            get => _used;
            set => _used = value;
        }

        public double? Free
        {
            get => _free;
            set => _free = value;
        }

        public double? FreePercent
        {
            get => _freePercent;
            set => _freePercent = value;
        }

        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }
    }

    [Serializable]
    public class NetWorkStat
    {
        private ulong _currentRecvBytes;
        private ulong _currentSendBytes;
        private string _mac = null!;
        private ulong _totalRecvBytes;
        private ulong _totalSendBytes;
        private DateTime _updateTime;


        public string Mac
        {
            get => _mac;
            set => _mac = value;
        }

        public ulong TotalSendBytes
        {
            get => _totalSendBytes;
            set => _totalSendBytes = value;
        }

        public ulong TotalRecvBytes
        {
            get => _totalRecvBytes;
            set => _totalRecvBytes = value;
        }

        public ulong CurrentSendBytes
        {
            get => _currentSendBytes;
            set => _currentSendBytes = value;
        }

        public ulong CurrentRecvBytes
        {
            get => _currentRecvBytes;
            set => _currentRecvBytes = value;
        }

        public DateTime UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }
    }
}