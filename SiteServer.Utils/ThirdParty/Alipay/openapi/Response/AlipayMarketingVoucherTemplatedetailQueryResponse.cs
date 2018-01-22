using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingVoucherTemplatedetailQueryResponse.
    /// </summary>
    public class AlipayMarketingVoucherTemplatedetailQueryResponse : AopResponse
    {
        /// <summary>
        /// 面额。每张代金券可以抵扣的金额。币种为人民币，单位为元。该数值不小于0，小数点以后最多两位。
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 最低额度。券的最低使用门槛金额，只有订单金额大于等于最低额度时券才能使用。币种为人民币，单位为元。该数值不小于0，小数点以后最多保留两位。
        /// </summary>
        [XmlElement("floor_amount")]
        public string FloorAmount { get; set; }

        /// <summary>
        /// 已发放总金额。币种为人民币，单位为元。该数值不小于0，小数点以后最多两位。
        /// </summary>
        [XmlElement("publish_amount")]
        public string PublishAmount { get; set; }

        /// <summary>
        /// 已发放张数。单位为张。该数值是大于0的整数。
        /// </summary>
        [XmlElement("publish_count")]
        public long PublishCount { get; set; }

        /// <summary>
        /// 发放结束时间，格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("publish_end_time")]
        public string PublishEndTime { get; set; }

        /// <summary>
        /// 发放开始时间，格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("publish_start_time")]
        public string PublishStartTime { get; set; }

        /// <summary>
        /// 退回金额。币种为人民币，单位为元。该数值不小于0，小数点以后最多两位。
        /// </summary>
        [XmlElement("recycle_amount")]
        public string RecycleAmount { get; set; }

        /// <summary>
        /// 模板状态，可枚举。分别为‘草稿’(I)、‘生效’(S)、‘删除’(D)和‘过期’(E)
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 券模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 总金额面额。币种为人民币，单位为元。该数值不小于0，小数点以后最多两位。仅代金券
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }

        /// <summary>
        /// 已使用总金额。币种为人民币，单位为元。该数值不小于0，小数点以后最多两位。
        /// </summary>
        [XmlElement("used_amount")]
        public string UsedAmount { get; set; }

        /// <summary>
        /// 已使用张数。单位为张。该数值是大于0的整数。
        /// </summary>
        [XmlElement("used_count")]
        public long UsedCount { get; set; }

        /// <summary>
        /// 券使用说明
        /// </summary>
        [XmlElement("voucher_description")]
        public string VoucherDescription { get; set; }

        /// <summary>
        /// 券名称
        /// </summary>
        [XmlElement("voucher_name")]
        public string VoucherName { get; set; }

        /// <summary>
        /// 拟发行券的数量。单位为张。该数值是大于0的整数。
        /// </summary>
        [XmlElement("voucher_quantity")]
        public long VoucherQuantity { get; set; }

        /// <summary>
        /// 券类型。可枚举，暂时只支持"代金券"(FIX_VOUCHER)
        /// </summary>
        [XmlElement("voucher_type")]
        public string VoucherType { get; set; }

        /// <summary>
        /// 券有效期。有两种类型：绝对时间和相对时间。使用JSON字符串表示。绝对时间有3个key：type、start、end，type取值固定为"ABSOLUTE"，start和end分别表示券生效时间和失效时间，格式为yyyy-MM-dd HH:mm:ss。绝对时间示例：{"type": "ABSOLUTE", "start": "2017-01-10 00:00:00", "end": "2017-01-13 23:59:59"}。相对时间有3个key：type、duration、unit，type取值固定为"RELATIVE"，duration表示从发券时间开始到往后推duration个单位时间为止作为券的使用有效期，unit表示有效时间单位，有效时间单位可枚举：MINUTE, HOUR, DAY。示例：{"type": "RELATIVE", "duration": 1 , "unit": "DAY" }，如果此刻发券，那么该券从现在开始生效1(duration)天(unit)后失效。
        /// </summary>
        [XmlElement("voucher_valid_period")]
        public string VoucherValidPeriod { get; set; }
    }
}
