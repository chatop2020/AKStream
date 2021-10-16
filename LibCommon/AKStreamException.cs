using System;

namespace LibCommon
{
    [Serializable]
    public class AkStreamException : Exception
    {
        public ResponseStruct ResponseStruct;

        public AkStreamException(ResponseStruct rs)
        {
            ResponseStruct = rs;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(ResponseStruct)}: {ResponseStruct}";
        }
    }
}