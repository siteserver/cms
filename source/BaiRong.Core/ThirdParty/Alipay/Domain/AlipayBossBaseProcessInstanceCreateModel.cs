using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayBossBaseProcessInstanceCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayBossBaseProcessInstanceCreateModel : AopObject
    {
        /// <summary>
        /// 加签内容
        /// </summary>
        [XmlArray("add_sign_content")]
        [XmlArrayItem("b_p_open_api_add_sign_content")]
        public List<BPOpenApiAddSignContent> AddSignContent { get; set; }

        /// <summary>
        /// 业务上下文，JSON格式
        /// </summary>
        [XmlElement("context")]
        public string Context { get; set; }

        /// <summary>
        /// 创建人的域账号
        /// </summary>
        [XmlElement("creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// 2088账号
        /// </summary>
        [XmlElement("ip_role_id")]
        public string IpRoleId { get; set; }

        /// <summary>
        /// 流程配置名称。需要先在流程平台配置流程
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 优先级，数字越大优先级越高，最大不超过29
        /// </summary>
        [XmlElement("priority")]
        public long Priority { get; set; }

        /// <summary>
        /// 流程全局唯一ID，和业务一一对应
        /// </summary>
        [XmlElement("puid")]
        public BPOpenApiPUID Puid { get; set; }

        /// <summary>
        /// 前置流程从哪个节点发起的本流程
        /// </summary>
        [XmlElement("source_node_name")]
        public string SourceNodeName { get; set; }

        /// <summary>
        /// 前置流程的PUID。用于串连起两个流程
        /// </summary>
        [XmlElement("source_puid")]
        public string SourcePuid { get; set; }

        /// <summary>
        /// 子流程的上下文。每一个上下文都使用JSON格式
        /// </summary>
        [XmlArray("sub_contexts")]
        [XmlArrayItem("string")]
        public List<string> SubContexts { get; set; }
    }
}
