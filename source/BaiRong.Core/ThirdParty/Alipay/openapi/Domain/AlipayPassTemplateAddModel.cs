using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayPassTemplateAddModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayPassTemplateAddModel : AopObject
    {
        /// <summary>
        /// 模板内容信息，遵循JSON规范，详情参见tpl_content参数说明：https://doc.open.alipay.com/doc2/detail.htm?treeId=193&articleId=105249&docType=1#tpl_content
        /// </summary>
        [XmlElement("tpl_content")]
        public string TplContent { get; set; }

        /// <summary>
        /// 商户用于控制模版的唯一性。（可以使用时间戳保证唯一性）
        /// </summary>
        [XmlElement("unique_id")]
        public string UniqueId { get; set; }
    }
}
