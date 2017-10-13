using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SecurityScene Data Structure.
    /// </summary>
    [Serializable]
    public class SecurityScene : AopObject
    {
        /// <summary>
        /// 接入渠道
        /// </summary>
        [XmlElement("access_channel")]
        public string AccessChannel { get; set; }

        /// <summary>
        /// 事件信息
        /// </summary>
        [XmlElement("ctu_params")]
        public string CtuParams { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [XmlElement("product_name")]
        public string ProductName { get; set; }

        /// <summary>
        /// 产品节点
        /// </summary>
        [XmlElement("product_node")]
        public string ProductNode { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [XmlElement("security_scene_params")]
        public string SecuritySceneParams { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        [XmlElement("system_name")]
        public string SystemName { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        [XmlElement("total_fee")]
        public string TotalFee { get; set; }
    }
}
