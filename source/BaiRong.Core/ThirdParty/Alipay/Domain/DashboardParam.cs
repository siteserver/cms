using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DashboardParam Data Structure.
    /// </summary>
    [Serializable]
    public class DashboardParam : AopObject
    {
        /// <summary>
        /// 仪表盘中的字段列名称
        /// </summary>
        [XmlElement("key")]
        public string Key { get; set; }

        /// <summary>
        /// 操作计算符，现支持的包括：EQ(等于),GT(大于),LT(小于),LTE(小于或等于),GTE(大于或等于),NOT_EQ(不等于),LIKE(like),NOT_LIKE(not like)
        /// </summary>
        [XmlElement("operate")]
        public string Operate { get; set; }

        /// <summary>
        /// 过滤条件值
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
