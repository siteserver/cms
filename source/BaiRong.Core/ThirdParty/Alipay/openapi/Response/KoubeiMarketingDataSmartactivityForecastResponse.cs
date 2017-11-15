using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataSmartactivityForecastResponse.
    /// </summary>
    public class KoubeiMarketingDataSmartactivityForecastResponse : AopResponse
    {
        /// <summary>
        /// 领取数量 上限值
        /// </summary>
        [XmlElement("apply_cnt_max")]
        public string ApplyCntMax { get; set; }

        /// <summary>
        /// 领取数量 下限值
        /// </summary>
        [XmlElement("apply_cnt_min")]
        public string ApplyCntMin { get; set; }

        /// <summary>
        /// 新增曝光数量 上限值
        /// </summary>
        [XmlElement("increased_exposure_max")]
        public string IncreasedExposureMax { get; set; }

        /// <summary>
        /// 新增曝光数量 下限值
        /// </summary>
        [XmlElement("increased_exposure_min")]
        public string IncreasedExposureMin { get; set; }

        /// <summary>
        /// 提升复购率区间上限
        /// </summary>
        [XmlElement("repay_rate_range_max")]
        public string RepayRateRangeMax { get; set; }

        /// <summary>
        /// 提升复购率区间下限
        /// </summary>
        [XmlElement("repay_rate_range_min")]
        public string RepayRateRangeMin { get; set; }

        /// <summary>
        /// 活动收益金额(分)区间上限值
        /// </summary>
        [XmlElement("trade_amt_range_max")]
        public string TradeAmtRangeMax { get; set; }

        /// <summary>
        /// 活动收益金额(分)区间下限值
        /// </summary>
        [XmlElement("trade_amt_range_min")]
        public string TradeAmtRangeMin { get; set; }

        /// <summary>
        /// 核销数量区间上限
        /// </summary>
        [XmlElement("verify_count_range_max")]
        public string VerifyCountRangeMax { get; set; }

        /// <summary>
        /// 核销数量区间下限
        /// </summary>
        [XmlElement("verify_count_range_min")]
        public string VerifyCountRangeMin { get; set; }
    }
}
