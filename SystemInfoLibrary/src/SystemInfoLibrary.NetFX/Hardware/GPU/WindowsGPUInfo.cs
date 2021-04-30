using System;
using System.Management;

namespace SystemInfoLibrary.Hardware.GPU
{
    internal class WindowsGPUInfo : GPUInfo
    {
        private readonly ManagementBaseObject _win32_videoController;

        public WindowsGPUInfo(ManagementBaseObject win32_videoController)
        {
            _win32_videoController = win32_videoController;
        }

        public override string Name => (String) _win32_videoController.GetPropertyValue("VideoProcessor");

        public override string Brand => (String) _win32_videoController.GetPropertyValue("Name");

        public override ulong MemoryTotal => (UInt32) _win32_videoController.GetPropertyValue("AdapterRAM");

        protected enum GPUArchitectureType
        {
            Other = 1,
            Unknown = 2,
            CGA = 3,
            EGA = 4,
            VGA = 5,
            SVGA = 6,
            MDA = 7,
            HGC = 8,
            MCGA = 9,
            EightFiveOneFourA = 10,
            XGA = 11,
            LinearFrameBuffer = 12,
            PCEightNine = 160
        };
    }
}