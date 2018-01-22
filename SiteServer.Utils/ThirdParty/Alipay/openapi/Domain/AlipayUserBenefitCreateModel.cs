using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserBenefitCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserBenefitCreateModel : AopObject
    {
        /// <summary>
        /// 权益专区码，在创建权益前应该先向蚂蚁会员平台申请一个合适的专区码。 专区必须存在。
        /// </summary>
        [XmlElement("benefit_area_code")]
        public string BenefitAreaCode { get; set; }

        /// <summary>
        /// 权益图标地址
        /// </summary>
        [XmlElement("benefit_icon_url")]
        public string BenefitIconUrl { get; set; }

        /// <summary>
        /// 权益的名称
        /// </summary>
        [XmlElement("benefit_name")]
        public string BenefitName { get; set; }

        /// <summary>
        /// 是否将权益的名称用作专区的副标题, 若为true,则会使用该权益的名称自动覆盖所属专区的副标题(暂未实现)
        /// </summary>
        [XmlElement("benefit_name_as_area_subtitle")]
        public bool BenefitNameAsAreaSubtitle { get; set; }

        /// <summary>
        /// 权益详情页面地址
        /// </summary>
        [XmlElement("benefit_page_url")]
        public string BenefitPageUrl { get; set; }

        /// <summary>
        /// 权益兑换消耗的积分数
        /// </summary>
        [XmlElement("benefit_point")]
        public long BenefitPoint { get; set; }

        /// <summary>
        /// 权益使用场景索引ID，接入时需要咨询@田豫如何取值
        /// </summary>
        [XmlElement("benefit_rec_biz_id")]
        public string BenefitRecBizId { get; set; }

        /// <summary>
        /// 支付宝商家券 ALIPAY_MERCHANT_COUPON  口碑商家券 KOUBEI_MERCHANT_COUPON  花呗分期免息券 HUABEI_FENQI_FREE_INTEREST_COUP  淘系通用券 TAOBAO_COMMON_COUPON  淘系商家券 TAOBAO_MERCHANT_COUPON  国际线上商家券 INTER_ONLINE_MERCHANT_COUPON  国际线下商家券 INTER_OFFLINE_MERCHANT_COUPON  通用商户权益 COMMON_MERCHANT_GOODS  其它 OTHERS, 接入是需要咨询@田豫如何选值
        /// </summary>
        [XmlElement("benefit_rec_type")]
        public string BenefitRecType { get; set; }

        /// <summary>
        /// 权益的副标题，用于补充描述权益
        /// </summary>
        [XmlElement("benefit_subtitle")]
        public string BenefitSubtitle { get; set; }

        /// <summary>
        /// 支付宝的营销活动id，若不走支付宝活动，则不需要填
        /// </summary>
        [XmlElement("camp_id")]
        public string CampId { get; set; }

        /// <summary>
        /// primary,golden,platinum,diamond分别对应大众、黄金、铂金、钻石会员等级。eligible_grade属性用于限制能够兑换当前权益的用户等级，用户必须不低于配置的等级才能进行兑换。如果没有等级要求，则不要填写该字段。
        /// </summary>
        [XmlElement("eligible_grade")]
        public string EligibleGrade { get; set; }

        /// <summary>
        /// 权益展示结束时间，使用Date.getTime()。结束时间必须大于起始时间。
        /// </summary>
        [XmlElement("end_time")]
        public long EndTime { get; set; }

        /// <summary>
        /// 兑换规则以及不满足该规则后给用户的提示文案，规则id和文案用:分隔；可配置多个，多个之间用,分隔。(分隔符皆是英文半角字符)规则id联系蚂蚁会员pd或运营提供
        /// </summary>
        [XmlElement("exchange_rule_ids")]
        public string ExchangeRuleIds { get; set; }

        /// <summary>
        /// 该权益对应每个等级会员的兑换折扣。等级和折扣用:分隔，多组折扣规则用:分隔。折扣0~1。分隔符皆为英文半角字符
        /// </summary>
        [XmlElement("grade_discount")]
        public string GradeDiscount { get; set; }

        /// <summary>
        /// 权益展示起始时间, 使用Date.getTime()。开始时间必须大于当前时间，且结束时间需要大于开始时间
        /// </summary>
        [XmlElement("start_time")]
        public long StartTime { get; set; }
    }
}
