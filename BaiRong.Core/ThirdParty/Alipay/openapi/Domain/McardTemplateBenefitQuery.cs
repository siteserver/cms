using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// McardTemplateBenefitQuery Data Structure.
    /// </summary>
    [Serializable]
    public class McardTemplateBenefitQuery : AopObject
    {
        /// <summary>
        /// 权益描述信息
        /// </summary>
        [XmlElement("benefit_desc")]
        public string BenefitDesc { get; set; }

        /// <summary>
        /// 权益ID
        /// </summary>
        [XmlElement("benefit_id")]
        public string BenefitId { get; set; }

        /// <summary>
        /// 权益结束时间
        /// </summary>
        [XmlElement("end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// 会员卡模板权益扩展信息：JSON格式; openUrl 说明：跳转到商户的优惠活动页面
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 权益开始时间
        /// </summary>
        [XmlElement("start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// 会员卡模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 权益标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}
