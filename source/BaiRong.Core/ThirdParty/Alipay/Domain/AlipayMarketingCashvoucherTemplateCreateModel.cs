using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCashvoucherTemplateCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCashvoucherTemplateCreateModel : AopObject
    {
        /// <summary>
        /// 面额。每张代金券可以抵扣的金额。币种为人民币，单位为元。该数值不能小于0，小数点以后最多保留两位。
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 创建券模板时录入的品牌信息，由商户自定义，在通用模板中展示在券LOGO下方
        /// </summary>
        [XmlElement("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 最低额度。设置券使用门槛，只有订单金额大于等于最低额度时券才能使用。币种为人民币，单位为元。该数值不能小于0，小数点以后最多保留两位。
        /// </summary>
        [XmlElement("floor_amount")]
        public string FloorAmount { get; set; }

        /// <summary>
        /// 出资人登录账号。用于发券的资金会从该账号划拨到发券专用账户上
        /// </summary>
        [XmlElement("fund_account")]
        public string FundAccount { get; set; }

        /// <summary>
        /// 券变动异步通知地址
        /// </summary>
        [XmlElement("notify_uri")]
        public string NotifyUri { get; set; }

        /// <summary>
        /// 外部业务单号。用作幂等控制。同一个pid下相同的外部业务单号作唯一键，参数不变的情况下，再次请求返回同样的模板id。请求成功后，修改参数再次提交，需要更换订单号。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 发放结束时间，晚于该时间不能发券。券的发放结束时间和发放开始时间跨度不能大于90天。发放结束时间必须晚于发放开始时间。格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("publish_end_time")]
        public string PublishEndTime { get; set; }

        /// <summary>
        /// 发放开始时间，早于该时间不能发券。发放开始时间不能大于当前时间15天。格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("publish_start_time")]
        public string PublishStartTime { get; set; }

        /// <summary>
        /// 重定向地址。支付成功后需要重定向的地址，如果为空则不做重定向。
        /// </summary>
        [XmlElement("redirect_uri")]
        public string RedirectUri { get; set; }

        /// <summary>
        /// 规则配置，JSON字符串，{"PID": "2088512417841101,2088512417841102", "STORE": "123456,678901"}，其中PID表示可以核销该券的pid列表，多个值用英文逗号隔开，PID为必传且需与接口调用PID同属一个商家，STORE表示可以核销该券的内部门店ID，多个值用英文逗号隔开
        /// </summary>
        [XmlElement("rule_conf")]
        public string RuleConf { get; set; }

        /// <summary>
        /// 券标语，用于拼接券名称，最多6个字符，券名称=券标语+券面额+’元代金券’,此字段已弃用
        /// </summary>
        [XmlElement("slogan")]
        public string Slogan { get; set; }

        /// <summary>
        /// 拟发行券的数量。单位为张。该数值必须是大于0的整数。
        /// </summary>
        [XmlElement("voucher_quantity")]
        public long VoucherQuantity { get; set; }

        /// <summary>
        /// 券类型。可枚举，暂时只支持"代金券"(FIX_VOUCHER)，使用示例voucher_type=FIX_VOUCHER
        /// </summary>
        [XmlElement("voucher_type")]
        public string VoucherType { get; set; }

        /// <summary>
        /// 券使用场景。可枚举，暂时只支持“支付宝充值中心话费流量通用现金券”(ALIPAY_RECHARGE)，使用示例voucher_use_scene=ALIPAY_RECHARGE
        /// </summary>
        [XmlElement("voucher_use_scene")]
        public string VoucherUseScene { get; set; }

        /// <summary>
        /// 券有效期。有两种类型：绝对时间和相对时间。使用JSON字符串表示。绝对时间有3个key：type、start、end，type取值固定为"ABSOLUTE"，start和end分别表示券生效时间和失效时间，格式为yyyy-MM-dd HH:mm:ss。绝对时间示例：{"type": "ABSOLUTE", "start": "2017-01-10 00:00:00", "end": "2017-01-13 23:59:59"}。相对时间有3个key：type、duration、unit，type取值固定为"RELATIVE"，duration表示从发券时间开始到往后推duration个单位时间为止作为券的使用有效期，unit表示有效时间单位，有效时间单位可枚举：MINUTE, HOUR, DAY。示例：{"type": "RELATIVE", "duration": 1 , "unit": "DAY" }，如果此刻发券，那么该券从现在开始生效1(duration)天(unit)后失效。
        /// </summary>
        [XmlElement("voucher_valid_period")]
        public string VoucherValidPeriod { get; set; }
    }
}
