using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataSmartactivityForecastModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataSmartactivityForecastModel : AopObject
    {
        /// <summary>
        /// 活动配置CODE
        /// </summary>
        [XmlElement("config_code")]
        public string ConfigCode { get; set; }

        /// <summary>
        /// 诊断结果CODE，目前有如下四个值  TRADE_RATE 流失会员占比高  USER_COUNT 会员数量少  REPAY_RATE 复购率低  COMPOSED_ACTIVITY 方案组诊断  当入参为TRADE_RATE和USER_COUNT时暂时不支持预测，会返回错误码UNSUPPORT_PARAMETER
        /// </summary>
        [XmlElement("diagnose_code")]
        public string DiagnoseCode { get; set; }

        /// <summary>
        /// 可选参数有如下几个：  worth_value:奖品面额,可以阶梯送数据（示例：10|20|30）单位：分  min_consume:门槛,可以阶梯送数据（示例：100|200|300）单位：分  voucher_valid_days:券有效期天数  activity_valid_days:活动有效期天数  min_cost:领券门槛,可以阶梯送数据（示例：100|200|300）单位：分  unconsume_days:会员流失天数  crowd_group:人群对象  consume_code:消费送活动形式包含  commission_rate:口碑客分佣比例  注意：对于消费送数据，min_consume/min_cost/worth_value是必填的且必须成组出现，对于诊断码为COMPOSED_ACTIVITY的预测，必须传入全量数据，并且各个参数使用竖线分隔多个值的场景
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }
    }
}
