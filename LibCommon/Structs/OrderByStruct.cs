using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs
{
    /// <summary>
    /// 排序方法
    /// ASC=顺序
    /// DESC=倒序
    /// </summary>
    [Serializable]
    public enum OrderByDir
    {
        ASC,
        DESC
    }

    [Serializable]
    public class OrderByStruct
    {
        private string? _fieldName = "id";
        private OrderByDir? _orderByDir = Structs.OrderByDir.ASC;

        /// <summary>
        /// 排序字段名
        /// </summary>
        public string? FieldName
        {
            get => _fieldName;
            set => _fieldName = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 排序方法
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderByDir? OrderByDir
        {
            get => _orderByDir;
            set => _orderByDir = value;
        }
    }
}