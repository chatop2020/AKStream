using System;
using System.IO;
using LibCommon.Structs;

namespace LibCommon
{
    /// <summary>保留文件检查的错误信息，避免 File.Exists 把权限、I/O 和目录不可达都吞成 false。</summary>
    public static class RecordingFileProbe
    {
        /// <summary>检查文件条目是否存在，只有明确的文件不存在才返回 false 且无错误。</summary>
        /// <param name="path">待检查的文件完整路径。</param>
        /// <param name="result">父目录不可达、权限或 I/O 错误返回磁盘异常，不能解释为已删除。</param>
        /// <param name="readAttributes">默认读取真实文件属性；故障验证可注入同等文件系统异常。</param>
        /// <returns>文件条目存在时返回 true；false 必须结合 result 判断。</returns>
        public static bool Exists(string path, out ResponseStruct result,
            Func<string, FileAttributes>? readAttributes = null)
        {
            result = new ResponseStruct { Code = ErrorNumber.None, Message = "文件检查完成。" };
            try
            {
                var attributes = (readAttributes ?? File.GetAttributes)(path);
                return (attributes & FileAttributes.Directory) == 0;
            }
            catch (FileNotFoundException) { return false; }
            catch (Exception ex)
            {
                result = new ResponseStruct { Code = ErrorNumber.MediaServer_DiskExcept,
                    Message = "无法核实录像文件，保留原索引和文件。", ExceptMessage = ex.Message };
                return false;
            }
        }
    }
}
