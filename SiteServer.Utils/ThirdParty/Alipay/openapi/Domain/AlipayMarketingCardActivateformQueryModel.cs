using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardActivateformQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardActivateformQueryModel : AopObject
    {
        /// <summary>
        /// 开放表单信息查询业务类型，可选类型如下：  MEMBER_CARD -- 会员卡开卡
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 查询用户表单提交信息的请求id，在用户授权表单确认提交后跳转商户页面url时返回此参数。
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 会员卡模板id。使用会员卡模板创建接口(alipay.marketing.card.template.create)返回的结果
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }
    }
}
