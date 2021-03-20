#if UNITY_5
using System.Collections.Generic;

using SystemInfoLibrary.Hardware.CPU;
using SystemInfoLibrary.Hardware.GPU;
using SystemInfoLibrary.Hardware.RAM;

namespace SystemInfoLibrary.Hardware
{
    internal sealed class UnityHardwareInfo : HardwareInfo
    {
        private IList<CPUInfo> _CPUs;
        public override IList<CPUInfo> CPUs => _CPUs ?? (_CPUs =
 new List<CPUInfo> { new UnityCPUInfo() }); // We'll assume only one physical CPU is supported

        private IList<GPUInfo> _GPUs;
        public override IList<GPUInfo> GPUs => _GPUs ?? (_GPUs = new List<GPUInfo> { new UnityGPUInfo() });

        private RAMInfo _RAM;
        public override RAMInfo RAM => _RAM ?? (_RAM = new UnityRAMInfo());
    }
}

#endif