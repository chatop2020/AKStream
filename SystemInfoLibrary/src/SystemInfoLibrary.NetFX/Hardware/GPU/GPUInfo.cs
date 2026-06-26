namespace SystemInfoLibrary.Hardware.GPU
{
    public abstract class GPUInfo
    {
        /// <summary>
        /// GPU name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// GPU vendor.
        /// </summary>
        public abstract string Brand { get; }

        /// <summary>
        /// Amount of total VRAM memory, in KB.
        /// </summary>
        public abstract ulong MemoryTotal { get; }
    }
}