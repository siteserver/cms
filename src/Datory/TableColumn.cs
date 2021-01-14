using System;

namespace Datory
{
    [Serializable]
    public class TableColumn
    {
        /// <summary>
        /// 字段名称。
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// 数据类型。
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// 数据长度。
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        /// 是否为表主键。
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否为自增长字段。
        /// </summary>
        public bool IsIdentity { get; set; }
    }
}
