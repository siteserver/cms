using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardActivateurlApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardActivateurlApplyModel : AopObject
    {
        /// <summary>
        /// 会员卡开卡表单提交后回调地址。  1.该地址不可带参数，如需回传参数，可设置out_string入参。
        /// </summary>
        [XmlElement("callback")]
        public string Callback { get; set; }

        /// <summary>
        /// 需要关注的生活号AppId。若需要在领卡页面展示“关注生活号”提示，可设置此参数为待关注的生活号AppId。生活号快速接入详见：https://doc.open.alipay.com/docs/doc.htm?treeId=193&articleId=105933&docType=1
        /// </summary>
        [XmlElement("follow_app_id")]
        public string FollowAppId { get; set; }

        /// <summary>
        /// 扩展信息，会员领卡完成后将此参数原样带回商户页面。
        /// </summary>
        [XmlElement("out_string")]
        public string OutString { get; set; }

        /// <summary>
        /// 会员卡模板id。使用会员卡模板创建接口(alipay.marketing.card.template.create)返回的结果
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }
    }
}
