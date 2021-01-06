using System;
using System.Collections.Generic;

namespace LibCommon.Structs
{
    [Serializable]
    public class PerformanceInfo
    {
        private string _systemType = null!;
        private string _architecture = null!;
        private string _osName = null!;
        private string _frameworkVersion = null!;
        private int _cpuCores;
        private MemoryInfo _memoryInfo = null!;
        private double _cpuLoad;
        private NetWorkStat _netWorkStat = null!;
        private List<DriveInfoDiy> _driveInfo = null!;
        private DateTime _updateTime;


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
    }

    [Serializable]
    public class MemoryInfo
    {
        private ulong _total;
        private ulong _used;
        private ulong _free;
        private double _freePercent;
        private DateTime _updateTime;

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
        private string? _name;
        private bool? _isReady;
        private double? _total;
        private double? _used;
        private double? _free;
        private double? _freePercent;
        private DateTime? _updateTime;


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
        private string _mac = null!;
        private long _totalSendBytes;
        private long _totalRecvBytes;
        private long _currentSendBytes;
        private long _currentRecvBytes;
        private DateTime _updateTime;


        public string Mac
        {
            get => _mac;
            set => _mac = value;
        }

        public long TotalSendBytes
        {
            get => _totalSendBytes;
            set => _totalSendBytes = value;
        }

        public long TotalRecvBytes
        {
            get => _totalRecvBytes;
            set => _totalRecvBytes = value;
        }

        public long CurrentSendBytes
        {
            get => _currentSendBytes;
            set => _currentSendBytes = value;
        }

        public long CurrentRecvBytes
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