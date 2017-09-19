using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicLifeLabelModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicLifeLabelModifyModel : AopObject
    {
        /// <summary>
        /// 标签id，调用创建标签接口后由支付宝返回 ，只支持生活号自定义标签
        /// </summary>
        [XmlElement("label_id")]
        public string LabelId { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        [XmlElement("label_name")]
        public string LabelName { get; set; }
    }
}
