#if UNITY_5
using UnityEngine;

namespace SystemInfoLibrary.Hardware.GPU
{
    internal class UnityGPUInfo : GPUInfo
    {
        public override string Name => SystemInfo.graphicsDeviceName;

        public override string Brand => SystemInfo.graphicsDeviceVendor;

        /*
        public override string Resolution => $"{Screen.currentResolution.width}x{Screen.currentResolution.height}";

        public override int RefreshRate => Screen.currentResolution.refreshRate;
        */

        public override ulong MemoryTotal => (ulong) SystemInfo.graphicsMemorySize*1024;
    }
}

#endif