namespace SystemInfoLibrary.Hardware.CPU
{
    public abstract class CPUInfo
    {
        /// <summary>
        /// CPU name. (tm) and (r) are removed.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// CPU vendor.
        /// </summary>
        public abstract string Brand { get; }

        /// <summary>
        /// Could be x86, x64, ARM.
        /// </summary>
        public abstract string Architecture { get; }

        /// <summary>
        /// Number of physical CPU cores.
        /// </summary>
        public abstract int PhysicalCores { get; }

        /// <summary>
        /// Number of logical CPU cores.
        /// </summary>
        public abstract int LogicalCores { get; }

        /// <summary>
        /// Current CPU frequency, average or of the first core.
        /// </summary>
        public abstract double Frequency { get; }
    }
}