using System;
using System.Collections.Generic;

namespace LibCommon.Structs.WebResponse
{
    /// <summary>
    /// 删除批量文件的回复
    /// </summary>
    [Serializable]
    public class ResDeleteFileList
    {
        private List<KeyValuePair<long, string>>? _pathList = new List<KeyValuePair<long, string>>();

        /// <summary>
        /// 未被正常删除的文件
        /// </summary>
        public List<KeyValuePair<long, string>>? PathList
        {
            get => _pathList;
            set => _pathList = value;
        }
    }
}