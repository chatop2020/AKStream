using System;

namespace LibCommon.Structs.WebResponse.AKStreamKeeper
{
    [Serializable]
    public class ResKeeperCheckMediaServerRunning
    {
        private bool? _isRunning;
        private int _pid;

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