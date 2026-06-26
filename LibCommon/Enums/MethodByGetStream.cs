using System;

namespace LibCommon.Enums
{
    /// <summary>
    /// 拉流方式
    /// </summary>
    [Serializable]
    public enum MethodByGetStream
    {
        SelfMethod, //内置方法AddStreamProxy
        UseFFmpeg, //引用ffmpeg,AddFFmpegSourceProxy
        None, //不需要
    }
}