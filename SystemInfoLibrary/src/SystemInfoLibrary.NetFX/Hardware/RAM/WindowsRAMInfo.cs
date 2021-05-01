using System;
using System.Management;

namespace SystemInfoLibrary.Hardware.RAM
{
    internal class WindowsRAMInfo : RAMInfo
    {
        private readonly ManagementBaseObject _win32_OperatingSystem;

        public WindowsRAMInfo(ManagementBaseObject win32_OperatingSystem)
        {
            _win32_OperatingSystem = win32_OperatingSystem;
        }

        public override ulong Free => (UInt64) _win32_OperatingSystem.GetPropertyValue("FreePhysicalMemory");

        public override ulong Total => (UInt64) _win32_OperatingSystem.GetPropertyValue("TotalVisibleMemorySize");
    }
}