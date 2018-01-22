using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataSmartactivityConfigResponse.
    /// </summary>
    public class KoubeiMarketingDataSmartactivityConfigResponse : AopResponse
    {
        /// <summary>
        /// 活动类型  CONSUME_SEND 消费送  DIRECT_SEND 直发奖  GUESS_SEND 口令送
        /// </summary>
        [XmlElement("activity_type")]
        public string ActivityType { get; set; }

        /// <summary>
        /// 活动有效天数
        /// </summary>
        [XmlElement("activity_valid_days")]
        public string ActivityValidDays { get; set; }

        /// <summary>
        /// 活动配置CODE
        /// </summary>
        [XmlElement("config_code")]
        public string ConfigCode { get; set; }

        /// <summary>
        /// 活动人群对象，包含针对N天未消费的用户/所有用户
        /// </summary>
        [XmlElement("crowd_group")]
        public string CrowdGroup { get; set; }

        /// <summary>
        /// 扩展信息，对于拉新的会返回commission_rate(口碑客分佣比例)，对于方案组的会返回SMART_PROMO_GROUP_ID(方案组ID),SMART_PROMO_PLAN_ID方案ID，多个方案竖线分隔，consume_code表示消费送活动形式，包含RULES/USRLEVEL两个枚举值，分别表示按照用户规则和会员分层来创建活动组
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 商品ID，只有在诊断码为SUPER_ITEM表示菜品营销时此字段才可能有值，多个值之间使用竖线|分隔
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 商品名称，只有在诊断码为SUPER_ITEM表示菜品营销时此字段才可能有值，多个值之间使用竖线|分隔
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// 奖品面额门槛（阶梯状），消费满不同的金额可以使用不同的券,单位：分
        /// </summary>
        [XmlElement("min_consume")]
        public string MinConsume { get; set; }

        /// <summary>
        /// 领券门槛（阶梯状），消费满不同的金额发不同金额的券券  活动类型为消费送且不是消费送礼包时设置  多营销工具之间不允许设置重复值  单位：分
        /// </summary>
        [XmlElement("min_cost")]
        public string MinCost { get; set; }

        /// <summary>
        /// 营销类型，只有在诊断码为SUPER_ITEM表示菜品营销时此字段才可能有值，多个值之间使用竖线|分隔
        /// </summary>
        [XmlElement("pro_type")]
        public string ProType { get; set; }

        /// <summary>
        /// 目前支持以下类型：  EXCHANGE：兑换券  MONEY：代金券  REDUCETO：减至券  RATE：折扣券
        /// </summary>
        [XmlElement("voucher_type")]
        public string VoucherType { get; set; }

        /// <summary>
        /// 券有效天数
        /// </summary>
        [XmlElement("voucher_valid_days")]
        public string VoucherValidDays { get; set; }

        /// <summary>
        /// 券面额，折扣券为折扣比例、立减为金额 单位：分
        /// </summary>
        [XmlElement("worth_value")]
        public string WorthValue { get; set; }
    }
}
