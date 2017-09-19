using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BizListDataInfo Data Structure.
    /// </summary>
    [Serializable]
    public class BizListDataInfo : AopObject
    {
        /// <summary>
        /// 下拉列表编号
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 下拉列表名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
