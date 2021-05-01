using System;
using System.Collections.Generic;

namespace LibCommon.Structs.WebRequest
{
    /// <summary>
    /// 用于分页查询的结构
    /// </summary>
    [Serializable]
    public class ReqPaginationBase
    {
        private List<OrderByStruct>? _orderBy;
        private int? _pageIndex = 1;
        private int? _pageSize = 10;

        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int? PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int? PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        /// <summary>
        /// 根据什么字段排序
        /// </summary>
        public List<OrderByStruct>? OrderBy
        {
            get => _orderBy;
            set => _orderBy = value;
        }
    }
}