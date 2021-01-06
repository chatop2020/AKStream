using System;
using System.Collections.Generic;

namespace LibCommon.Structs.WebResponse
{
    public class DepartmentInfo
    {
        private string _departmentId;
        private string _departmentName;
        private string _pDepartmentId;
        private string _pDepartmentName;

        /// <summary>
        /// 部门代码
        /// </summary>
        public string DepartmentId
        {
            get => _departmentId;
            set => _departmentId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get => _departmentName;
            set => _departmentName = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 上级部门代码
        /// </summary>
        public string PDepartmentId
        {
            get => _pDepartmentId;
            set => _pDepartmentId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 上级部门名称
        /// </summary>
        public string PDepartmentName
        {
            get => _pDepartmentName;
            set => _pDepartmentName = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    /// <summary>
    /// 部门信息返回结构
    /// </summary>
    public class ResGetDepartmentInfo
    {
        private List<DepartmentInfo> _departmentInfoList = new List<DepartmentInfo>();

        /// <summary>
        /// 部门信息列表
        /// </summary>
        public List<DepartmentInfo> DepartmentInfoList
        {
            get => _departmentInfoList;
            set => _departmentInfoList = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}