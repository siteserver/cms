using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayBossBaseProcessTaskProcessModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayBossBaseProcessTaskProcessModel : AopObject
    {
        /// <summary>
        /// 更新的业务上下文。和原有业务上下文同key覆盖，新增key新增。
        /// </summary>
        [XmlElement("context")]
        public string Context { get; set; }

        /// <summary>
        /// 处理幂等值，特别注意这个值的使用，不能依赖于流程中的任何值。
        /// </summary>
        [XmlElement("idempotent_id")]
        public string IdempotentId { get; set; }

        /// <summary>
        /// 处理备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 当前需要推进的节点
        /// </summary>
        [XmlElement("node")]
        public string Node { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        [XmlElement("operate")]
        public string Operate { get; set; }

        /// <summary>
        /// 当前处理人域账号
        /// </summary>
        [XmlElement("operator")]
        public string Operator { get; set; }

        /// <summary>
        /// 更新的优先级。填写0则不更新，使用原值
        /// </summary>
        [XmlElement("priority")]
        public long Priority { get; set; }

        /// <summary>
        /// 流程全局唯一ID
        /// </summary>
        [XmlElement("puid")]
        public string Puid { get; set; }

        /// <summary>
        /// 更新的子流程上下文。完全覆盖原值。若不需要覆盖，则传null
        /// </summary>
        [XmlArray("sub_contexts")]
        [XmlArrayItem("string")]
        public List<string> SubContexts { get; set; }
    }
}
