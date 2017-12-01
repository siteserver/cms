using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AssetResult Data Structure.
    /// </summary>
    [Serializable]
    public class AssetResult : AopObject
    {
        /// <summary>
        /// 订单明细ID
        /// </summary>
        [XmlElement("assign_item_id")]
        public string AssignItemId { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        [XmlElement("error_desc")]
        public string ErrorDesc { get; set; }

        /// <summary>
        /// 是否处理成功
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }
    }
}
