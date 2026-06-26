using System;

namespace LibCommon.Structs.WebRequest
{
    [Serializable]
    public class ReqGetDepartmentInfo
    {
        private string? _departmentId;
        private bool? _includeSubDepartment;

        /// <summary>
        /// 部门代码（可空）
        /// </summary>
        public string? DepartmentId
        {
            get => _departmentId;
            set => _departmentId = value;
        }

        /// <summary>
        /// 包含子部门
        /// </summary>
        public bool? IncludeSubDepartment
        {
            get => _includeSubDepartment;
            set => _includeSubDepartment = value;
        }
    }
}