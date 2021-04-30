using System;
using SystemInfoLibrary.Hardware;

namespace SystemInfoLibrary.OperatingSystem
{
    internal class OtherOperatingSystemInfo : OperatingSystemInfo
    {
        private Version _javaVersion;
        public override string Architecture => "Unknown";

        public override string Name => "Unknown";
        public override Version JavaVersion => _javaVersion ?? (_javaVersion = new Version());

        public override HardwareInfo Hardware => null;


        public override OperatingSystemInfo Update()
        {
            return this;
        }
    }
}