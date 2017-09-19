using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserNewbenefitModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserNewbenefitModifyModel : AopObject
    {
        /// <summary>
        /// 权益关联的专区码 ,需要联系蚂蚁会员平台的业务负责人进行专区码分配
        /// </summary>
        [XmlElement("area_code")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 权益Id，用于定位需要编辑的权益，通过权益创建接口获取得到，调用创建接口后，将权益Id妥善存储，编辑时传入
        /// </summary>
        [XmlElement("benefit_id")]
        public long BenefitId { get; set; }

        /// <summary>
        /// 权益的名称，由商户自行定义
        /// </summary>
        [XmlElement("benefit_name")]
        public string BenefitName { get; set; }

        /// <summary>
        /// 权益的副标题，用于补充描述权益
        /// </summary>
        [XmlElement("benefit_sub_title")]
        public string BenefitSubTitle { get; set; }

        /// <summary>
        /// 权益详情描述
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// 当权益为非差异化时，该字段生效，用于控制允许兑换权益的等级
        /// </summary>
        [XmlElement("eligible_grade_desc")]
        public string EligibleGradeDesc { get; set; }

        /// <summary>
        /// 权益展示结束时间，使用Date.getTime()。结束时间必须大于起始时间。
        /// </summary>
        [XmlElement("end_dt")]
        public long EndDt { get; set; }

        /// <summary>
        /// 兑换规则以及不满足该规则后给用户的提示文案，规则id和文案用:分隔；可配置多个，多个之间用,分隔。(分隔符皆是英文半角字符)规则id联系蚂蚁会员pd或运营提供
        /// </summary>
        [XmlElement("exchange_rule_ids")]
        public string ExchangeRuleIds { get; set; }

        /// <summary>
        /// 权益等级配置
        /// </summary>
        [XmlArray("grade_config")]
        [XmlArrayItem("benefit_grade_config")]
        public List<BenefitGradeConfig> GradeConfig { get; set; }

        /// <summary>
        /// 权益展示时的图标地址，由商户上传图片到合适的存储平台，然后将图片的地址传入
        /// </summary>
        [XmlElement("icon_url")]
        public string IconUrl { get; set; }

        /// <summary>
        /// 需要被移除的等级配置
        /// </summary>
        [XmlElement("remove_grades")]
        public string RemoveGrades { get; set; }

        /// <summary>
        /// 权益展示的起始时间戳，使用Date.getTime()获得
        /// </summary>
        [XmlElement("start_dt")]
        public long StartDt { get; set; }
    }
}
