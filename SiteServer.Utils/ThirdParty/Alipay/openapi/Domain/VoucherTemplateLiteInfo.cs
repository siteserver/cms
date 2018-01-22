using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VoucherTemplateLiteInfo Data Structure.
    /// </summary>
    [Serializable]
    public class VoucherTemplateLiteInfo : AopObject
    {
        /// <summary>
        /// 模板激活时间。草稿状态的模板该项为空
        /// </summary>
        [XmlElement("activate_time")]
        public string ActivateTime { get; set; }

        /// <summary>
        /// 模板创建时间。格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("create_time")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 发放结束时间。格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("publish_end_time")]
        public string PublishEndTime { get; set; }

        /// <summary>
        /// 券模板发放开始时间。格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("publish_start_time")]
        public string PublishStartTime { get; set; }

        /// <summary>
        /// 模板状态，可枚举。分别为‘草稿’(I)、‘生效’(S)和‘过期’(E)
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 券模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 券名称
        /// </summary>
        [XmlElement("voucher_name")]
        public string VoucherName { get; set; }
    }
}
