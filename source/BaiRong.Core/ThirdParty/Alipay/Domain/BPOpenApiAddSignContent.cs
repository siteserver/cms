using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BPOpenApiAddSignContent Data Structure.
    /// </summary>
    [Serializable]
    public class BPOpenApiAddSignContent : AopObject
    {
        /// <summary>
        /// 自定义的条件跳转。JSON格式
        /// </summary>
        [XmlArray("additional_lines")]
        [XmlArrayItem("string")]
        public List<string> AdditionalLines { get; set; }

        /// <summary>
        /// 任务处理人的域账号列表
        /// </summary>
        [XmlElement("assignee")]
        public string Assignee { get; set; }

        /// <summary>
        /// 自定义操作
        /// </summary>
        [XmlElement("deal_actions")]
        public string DealActions { get; set; }

        /// <summary>
        /// 任务处理链接。如果不填，则使用流程平台默认地址
        /// </summary>
        [XmlElement("deal_url")]
        public string DealUrl { get; set; }

        /// <summary>
        /// 详情查看地址。如果不填写，则使用流程平台默认详情地址
        /// </summary>
        [XmlElement("detail_url")]
        public string DetailUrl { get; set; }

        /// <summary>
        /// 处理节点展示名称
        /// </summary>
        [XmlElement("display_name")]
        public string DisplayName { get; set; }
    }
}
