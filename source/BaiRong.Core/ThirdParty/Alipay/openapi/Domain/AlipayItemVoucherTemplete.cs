using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayItemVoucherTemplete Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayItemVoucherTemplete : AopObject
    {
        /// <summary>
        /// 延迟生效时间（手动领取条件下，可跟valid_period组合使用）
        /// </summary>
        [XmlElement("delay_minute")]
        public long DelayMinute { get; set; }

        /// <summary>
        /// 券使用规则描述，包括描述标题及描述内容列表。子参数只支持title、details；其余参数暂不支持，请不要填写。
        /// </summary>
        [XmlArray("desc_details")]
        [XmlArrayItem("alipay_item_description")]
        public List<AlipayItemDescription> DescDetails { get; set; }

        /// <summary>
        /// 折扣率，填写0-1间的小数且不包括0和1，如八折则传入0.8。（折扣券类型必选字段，代金券不需要）
        /// </summary>
        [XmlElement("discount_rate")]
        public long DiscountRate { get; set; }

        /// <summary>
        /// 外部单品列表
        /// </summary>
        [XmlElement("external_goods_list")]
        public AlipayItemGoodsList ExternalGoodsList { get; set; }

        /// <summary>
        /// 使用时间规则，控制商品的生效时间。  时间单位：MINUTE、HOUR、WEEK_DAY、DAY、WEEK、MONTH  限制规则：INCLUDE、INCLUDE_INTERVAL  例如：每周日、周二的 0点-23点59分  "limit_period_info_list":[{"rule":"INCLUDE","unit":"WEEK_DAY","value":"7,2"},{"rule":"INCLUDE_INTERVAL","unit":"MINUTE","value":"0,1439"}]
        /// </summary>
        [XmlArray("limit_period_info_list")]
        [XmlArrayItem("alipay_item_limit_period_info")]
        public List<AlipayItemLimitPeriodInfo> LimitPeriodInfoList { get; set; }

        /// <summary>
        /// 商品原金额，单位（元），（代金券类型可选字段，折扣券不需要）
        /// </summary>
        [XmlElement("original_amount")]
        public long OriginalAmount { get; set; }

        /// <summary>
        /// 券原折扣,0-1之间，做展示使用（折扣券类型可选字段，代金券不需要）
        /// </summary>
        [XmlElement("original_rate")]
        public long OriginalRate { get; set; }

        /// <summary>
        /// 减至金额，单位（元），代表券可抵扣至多少（代金券类型可选字段，折扣券不需要）
        /// </summary>
        [XmlElement("reduce_to_amount")]
        public long ReduceToAmount { get; set; }

        /// <summary>
        /// 折扣金额取整规则  AUTO_ROUNDING_YUAN：自动抹零到元  AUTO_ROUNDING_JIAO：自动抹零到角  ROUNDING_UP_YUAN：四舍五入到元  ROUNDING_UP_JIAO：四舍五入到角
        /// </summary>
        [XmlElement("rounding_rule")]
        public string RoundingRule { get; set; }

        /// <summary>
        /// 起步金额，单位（元）
        /// </summary>
        [XmlElement("threshold_amount")]
        public long ThresholdAmount { get; set; }

        /// <summary>
        /// 起步数量，用于指定可享受优惠的起步单品购买数量
        /// </summary>
        [XmlElement("threshold_quantity")]
        public long ThresholdQuantity { get; set; }

        /// <summary>
        /// 领券之后多长时间内可以核销，单位：分钟，传入数值需大于1440（一天）
        /// </summary>
        [XmlElement("valid_period")]
        public long ValidPeriod { get; set; }

        /// <summary>
        /// 价值金额，单位（元）  CASH类型为代金券金额  DISCOUNT类型为优惠封顶金额  在代金券类型时，value_amout与reduce_to_amount不能同时为空，不能同时不为空。
        /// </summary>
        [XmlElement("value_amount")]
        public long ValueAmount { get; set; }

        /// <summary>
        /// 券的描述信息，目前客户端将统一展示“折扣须知”
        /// </summary>
        [XmlElement("voucher_desc")]
        public string VoucherDesc { get; set; }

        /// <summary>
        /// 券类型，DISCOUNT（折扣券）、CASH（代金券）
        /// </summary>
        [XmlElement("voucher_type")]
        public string VoucherType { get; set; }
    }
}
