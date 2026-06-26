using System;
using System.Management;

namespace SystemInfoLibrary.Hardware.CPU
{
    internal class WindowsCPUInfo : CPUInfo
    {
        private readonly ManagementBaseObject _win32_processor;

        public WindowsCPUInfo(ManagementBaseObject win32_processor)
        {
            _win32_processor = win32_processor;
        }

        public override string Name => (String)_win32_processor.GetPropertyValue("Name");

        public override string Brand => (String)_win32_processor.GetPropertyValue("Manufacturer");

        public override string Architecture => Enum.GetName(typeof(CPUArchitectureType),
            (UInt16)_win32_processor.GetPropertyValue("Architecture"));

        public override int PhysicalCores => (Int32)(UInt32)_win32_processor.GetPropertyValue("NumberOfCores");

        public override int LogicalCores =>
            (Int32)(UInt32)_win32_processor.GetPropertyValue("NumberOfLogicalProcessors");

        public override double Frequency => (Double)(UInt32)_win32_processor.GetPropertyValue("CurrentClockSpeed");

        protected enum CPUArchitectureType : UInt16
        {
            x86 = 0,
            MIPS = 1,
            Alpha = 2,
            PowerPC = 3,
            ARM = 5,
            ia64 = 6,
            x64 = 9
        };
    }
}