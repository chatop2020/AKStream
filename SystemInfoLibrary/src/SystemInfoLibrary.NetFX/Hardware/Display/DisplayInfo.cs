namespace SystemInfoLibrary.Hardware.Display
{
    public abstract class DisplayInfo
    {
        /// <summary>
        /// Display name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Display resolution, in HxW format.
        /// </summary>
        public abstract string Resolution { get; }

        /// <summary>
        /// Display Refrash Rate, in Hz.
        /// </summary>
        public abstract int RefreshRate { get; }
    }
}