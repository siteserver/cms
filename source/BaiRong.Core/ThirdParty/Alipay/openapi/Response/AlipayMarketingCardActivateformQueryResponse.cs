using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCardActivateformQueryResponse.
    /// </summary>
    public class AlipayMarketingCardActivateformQueryResponse : AopResponse
    {
        /// <summary>
        /// 表单提交信息各个字段的值JSON数组    默认通用表单字段名称可选值如下：  OPEN_FORM_FIELD_MOBILE – 手机号  OPEN_FORM_FIELD_GENDER – 性别  OPEN_FORM_FIELD_NAME – 姓名  OPEN_FORM_FIELD_BIRTHDAY – 生日  OPEN_FORM_FIELD_IDCARD – 身份证  OPEN_FORM_FIELD_EMAIL – 邮箱  OPEN_FORM_FIELD_ADDRESS – 地址
        /// </summary>
        [XmlElement("infos")]
        public string Infos { get; set; }
    }
}
