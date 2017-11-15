using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PaymentAbilityQueryResponse Data Structure.
    /// </summary>
    [Serializable]
    public class PaymentAbilityQueryResponse : AopObject
    {
        /// <summary>
        /// 附加信息，json格式字符串。暂时包含信息：是否是支付宝钱包用户，是否是数字娱乐行业活跃用户。
        /// </summary>
        [XmlElement("extra_infos")]
        public string ExtraInfos { get; set; }

        /// <summary>
        /// 接口返回的支付能力等级
        /// </summary>
        [XmlElement("level")]
        public string Level { get; set; }

        /// <summary>
        /// 返回的单据号
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }
    }
}
