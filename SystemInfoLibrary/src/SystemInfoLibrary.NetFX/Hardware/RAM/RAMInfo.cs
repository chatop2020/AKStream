namespace SystemInfoLibrary.Hardware.RAM
{
    public abstract class RAMInfo
    {
        /// <summary>
        /// Amount of total RAM memory, in KB.
        /// </summary>
        public abstract ulong Total { get; }

        /// <summary>
        /// Amount of total free RAM memory, in KB.
        /// </summary>
        public abstract ulong Free { get; }
    }
}