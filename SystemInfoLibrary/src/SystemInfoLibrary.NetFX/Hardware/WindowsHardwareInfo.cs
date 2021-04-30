using System.Collections.Generic;
using System.Linq;
using System.Management;
using SystemInfoLibrary.Hardware.CPU;
using SystemInfoLibrary.Hardware.GPU;
using SystemInfoLibrary.Hardware.RAM;

namespace SystemInfoLibrary.Hardware
{
    internal sealed class WindowsHardwareInfo : HardwareInfo
    {
        private IList<CPUInfo> _CPUs;

        private IList<GPUInfo> _GPUs;

        private RAMInfo _RAM;

        public override IList<CPUInfo> CPUs
        {
            get
            {
                if (_CPUs == null)
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
                    {
                        _CPUs = (from ManagementBaseObject processor in searcher.Get()
                            select (CPUInfo) new WindowsCPUInfo(processor)).ToList();
                    }
                }

                return _CPUs;
            }
        }

        public override IList<GPUInfo> GPUs
        {
            get
            {
                if (_GPUs == null)
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                    {
                        _GPUs = (from ManagementBaseObject videoController in searcher.Get()
                            select (GPUInfo) new WindowsGPUInfo(videoController)).ToList();
                    }
                }

                return _GPUs;
            }
        }

        public override RAMInfo RAM
        {
            get
            {
                if (_RAM == null)
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                    {
                        _RAM = (from ManagementBaseObject os in searcher.Get() select (RAMInfo) new WindowsRAMInfo(os))
                            .FirstOrDefault();
                    }
                }

                return _RAM;
            }
        }
    }
}