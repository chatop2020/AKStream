namespace SystemInfoLibrary.Hardware.CPU
{
    internal class BSDCPUInfo : CPUInfo
    {
        public override string Name => Utils.FilterCPUName(Utils.GetSysCtlPropertyString("machdep.cpu.brand_string"));

        public override string Brand => Utils.GetSysCtlPropertyString("machdep.cpu.vendor");

        public override string Architecture => Utils.GetSysCtlPropertyInt32("hw.cpu64bit_capable") == 1 ? "x64" : "x86";

        public override int PhysicalCores => Utils.GetSysCtlPropertyInt32("hw.physicalcpu");

        public override int LogicalCores => Utils.GetSysCtlPropertyInt32("hw.logicalcpu");

        public override double Frequency =>
            (double) Utils.GetSysCtlPropertyInt64("hw.cpufrequency") / (double) 1024 / (double) 1024;
    }
}