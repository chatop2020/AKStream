using System.Linq;

namespace SystemInfoLibrary.Hardware.GPU
{
    internal class MacOSXGPUInfo : GPUInfo
    {
        private readonly string[] _info;

        public MacOSXGPUInfo(string[] info)
        {
            _info = info;
        }

        public override string Name => _info.FirstOrDefault()?.Split(' ').FirstOrDefault();

        public override string Brand
        {
            get
            {
                var split = _info.FirstOrDefault()?.Split(' ');
                return split?.Length >= 2 ? string.Join(" ", _info.FirstOrDefault()?.Split(' ').Skip(1)) : string.Empty;
            }
        }

        public override ulong MemoryTotal => _info.Length >= 2
            ? (ulong.TryParse(_info[1].Split(' ').FirstOrDefault(), out var vram) ? vram * 1024 : 0)
            : 0;
    }
}