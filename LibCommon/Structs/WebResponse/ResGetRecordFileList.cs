using System;
using System.Collections.Generic;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;

namespace LibCommon.Structs.WebResponse
{
    /// <summary>
    /// 获取录像文件返回结构
    /// </summary>
    [Serializable]
    public class ResGetRecordFileList
    {
        private List<RecordFile>? _recordFileList;
        private ReqGetRecordFileList? _request;
        private long? _total;

        public List<RecordFile>? RecordFileList
        {
            get => _recordFileList;
            set => _recordFileList = value;
        }

        public ReqGetRecordFileList? Request
        {
            get => _request;
            set => _request = value;
        }

        public long? Total
        {
            get => _total;
            set => _total = value;
        }
    }
}