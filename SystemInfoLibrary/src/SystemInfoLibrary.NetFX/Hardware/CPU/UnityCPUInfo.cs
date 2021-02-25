#if UNITY_5
using UnityEngine;

namespace SystemInfoLibrary.Hardware.CPU
{
    internal class UnityCPUInfo : CPUInfo
    {
        public override string Name => SystemInfo.processorType;

        public override string Brand => "Unknown";

        public override string Architecture => "Unknown";

        public override int PhysicalCores => SystemInfo.processorCount;

        public override int LogicalCores => 0;


        public override double Frequency => SystemInfo.processorFrequency;
    }
}
#endif