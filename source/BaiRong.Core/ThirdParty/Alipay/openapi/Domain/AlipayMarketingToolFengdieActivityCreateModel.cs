using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingToolFengdieActivityCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingToolFengdieActivityCreateModel : AopObject
    {
        /// <summary>
        /// H5应用初始化数据
        /// </summary>
        [XmlElement("activity")]
        public FengdieActivityCreateData Activity { get; set; }

        /// <summary>
        /// 凤蝶模板包唯一id，从alipay.marketing.tool.fengdie.template.query接口中获取
        /// </summary>
        [XmlElement("template_id")]
        public long TemplateId { get; set; }
    }
}
