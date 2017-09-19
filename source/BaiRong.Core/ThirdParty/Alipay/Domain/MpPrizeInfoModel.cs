using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MpPrizeInfoModel Data Structure.
    /// </summary>
    [Serializable]
    public class MpPrizeInfoModel : AopObject
    {
        /// <summary>
        /// 凭证id，通过alipay.marketing.campaign.cert.create 接口创建的凭证id，开发者可以根据此凭证处理自己的业务逻辑，如给用户发放自定义优惠券等；
        /// </summary>
        [XmlElement("certlot_number")]
        public string CertlotNumber { get; set; }

        /// <summary>
        /// 奖品频率对应的次数，最大999999，如frequency_type为‘D’，值为2，则表示每日的奖品最多可领取2次
        /// </summary>
        [XmlElement("frequency_count")]
        public string FrequencyCount { get; set; }

        /// <summary>
        /// 奖品中奖频率类型: D，每自然日；W，每自然周（从周一至周日）；M，每自然月
        /// </summary>
        [XmlElement("frequency_type")]
        public string FrequencyType { get; set; }

        /// <summary>
        /// 奖品结束时间，yyyy-mm-dd 00:00:00格式，大于奖品开始时间，必须在活动有效期内
        /// </summary>
        [XmlElement("prize_end_time")]
        public string PrizeEndTime { get; set; }

        /// <summary>
        /// 奖品id，值由支付宝生成；调用alipay.marketing.campaign.drawcamp.create创建活动时不需要传入； 调用alipay.marketing.campaign.drawcamp.query接口查询时会返回；调用alipay.marketing.campaign.drawcamp.update接口修改活动时，如果不填prize_id,则会用参数新增一个奖品，并覆盖之前的奖品，如开发者想保留或修改当前活动奖品信息，则在修改接口中此参数必传。
        /// </summary>
        [XmlElement("prize_id")]
        public string PrizeId { get; set; }

        /// <summary>
        /// 单个用户当前奖品允许领取的最大次数，最大999999，原则上活动领取次数与奖品领取次数保持一致，特殊情况如：中奖次数每人可中2次，但奖品只能每人领取一个，则中奖次数每人只能一次。该属性不支持修改，修改时透传处理
        /// </summary>
        [XmlElement("prize_max_award_limit")]
        public string PrizeMaxAwardLimit { get; set; }

        /// <summary>
        /// 奖品名称，开发者自定义
        /// </summary>
        [XmlElement("prize_name")]
        public string PrizeName { get; set; }

        /// <summary>
        /// 奖品开始时间，yyyy-mm-dd 00:00:00格式，需在活动有效期内，不能晚于奖品结束时间
        /// </summary>
        [XmlElement("prize_start_time")]
        public string PrizeStartTime { get; set; }

        /// <summary>
        /// 奖品总数量，数值，最大999999
        /// </summary>
        [XmlElement("prize_total")]
        public string PrizeTotal { get; set; }

        /// <summary>
        /// 奖品类型，目前支持CAMP_CERT_PRIZE，凭证奖品类型
        /// </summary>
        [XmlElement("prize_type")]
        public string PrizeType { get; set; }
    }
}
