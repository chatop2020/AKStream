using System;

namespace LibCommon.Structs.WebResponse.AKStreamKeeper
{
    [Serializable]
    public class ResKeeperCheckMediaServerRunning
    {
        private int _pid;
        private bool? _isRunning;

        public int Pid
        {
            get => _pid;
            set => _pid = value;
        }

        public bool? IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }
    }
}