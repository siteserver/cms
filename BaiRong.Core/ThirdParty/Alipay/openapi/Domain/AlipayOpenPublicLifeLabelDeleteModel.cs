using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicLifeLabelDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicLifeLabelDeleteModel : AopObject
    {
        /// <summary>
        /// 标签id, 只支持生活号自定义标签
        /// </summary>
        [XmlElement("label_id")]
        public string LabelId { get; set; }
    }
}
