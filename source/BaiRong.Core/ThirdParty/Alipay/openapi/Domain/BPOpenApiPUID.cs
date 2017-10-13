using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BPOpenApiPUID Data Structure.
    /// </summary>
    [Serializable]
    public class BPOpenApiPUID : AopObject
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        [XmlElement("app_name")]
        public string AppName { get; set; }

        /// <summary>
        /// 业务ID，对应业务单条记录的ID
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 业务类型。不要填写下划线、点等特殊符号
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 一般留空。如果一个biz_id可发起多个流程实例，则填写此值
        /// </summary>
        [XmlElement("unique_key")]
        public string UniqueKey { get; set; }
    }
}
