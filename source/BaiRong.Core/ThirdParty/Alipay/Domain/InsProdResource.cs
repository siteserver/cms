using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsProdResource Data Structure.
    /// </summary>
    [Serializable]
    public class InsProdResource : AopObject
    {
        /// <summary>
        /// 资源项标识符;insMustKnow:投保须知,productTerm:产品条款,insAgreement:投保协议,insAgreementH5:投保协议H5,healthMustKnow:健康告知,announcement:重要告知,productFeature:产品特色,productFeatureDetail:产品特色详情,insTermUrl:保险条款链接,relativeFileUrl:相关文件链接,claimFlow:理赔流程,productImage:产品图片,productImageSmall:产品小图片,productIcon:产品图标,insDetail:投保详情,claimDetail:理赔详情,insDetailDigest:投保详情摘要,electronicPolicyUrl:电子保单地址.
        /// </summary>
        [XmlElement("key")]
        public string Key { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 资源类型;text:文本;url:URL;
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
