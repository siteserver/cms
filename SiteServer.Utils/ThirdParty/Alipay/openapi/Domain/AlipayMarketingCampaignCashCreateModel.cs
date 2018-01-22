using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignCashCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignCashCreateModel : AopObject
    {
        /// <summary>
        /// 红包名称,商户在查询列表、详情看到的名字,同时也会显示在商户付款页面。
        /// </summary>
        [XmlElement("coupon_name")]
        public string CouponName { get; set; }

        /// <summary>
        /// 活动结束时间,必须大于活动开始时间, 基本格式:yyyy-MM-dd HH:mm:ss,活动有效时间最长为6个月，过期后需要创建新的活动。
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 商户打款后的跳转链接，从支付宝收银台打款成功后的跳转链接。不填时，打款后停留在支付宝支付成功页。商户实际跳转会自动添加crowdNo作为跳转参数。示例: http://www.yourhomepage.com?crowdNo=XXX
        /// </summary>
        [XmlElement("merchant_link")]
        public string MerchantLink { get; set; }

        /// <summary>
        /// 活动文案,用户在账单、红包中看到的账单描述、红包描述
        /// </summary>
        [XmlElement("prize_msg")]
        public string PrizeMsg { get; set; }

        /// <summary>
        /// 现金红包的发放形式, fixed为固定金额,random为随机金额。选择随机金额时，单个红包的金额在平均金额的0.5~1.5倍之间浮动。
        /// </summary>
        [XmlElement("prize_type")]
        public string PrizeType { get; set; }

        /// <summary>
        /// 用户在当前活动参与次数、频率限制。支持日(D)、周(W)、月(M)、终身(L)维度的限制。其中日(D)、周(W)、月(M)最多只能选择一个,终身(L)为必填项。多个配置之间使用"|"进行分隔。终身(L)次数限制最大为100，日(D)、周(W)、月(M)频率设置必须小于等于终身(L)的次数。整个字段不填时默认值为:L1。允许多次领取时，活动触发接口需要传入out_biz_no来配合。
        /// </summary>
        [XmlElement("send_freqency")]
        public string SendFreqency { get; set; }

        /// <summary>
        /// 活动开始时间,必须大于活动创建的时间.   (1) 填固定时间:2016-08-10 22:28:30, 基本格式:yyyy-MM-dd HH:mm:ss  (2) 填字符串NowTime
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 活动发放的现金总金额,最小金额1.00元,最大金额10000000.00元。每个红包的最大金额不允许超过200元,最小金额不得低于0.20元。 实际的金额限制可能会根据业务进行动态调整。
        /// </summary>
        [XmlElement("total_money")]
        public string TotalMoney { get; set; }

        /// <summary>
        /// 红包发放个数，最小1个,最大10000000个。  但不同的发放形式（即prize_type）会使得含义不同：  (1) 若prize_type选择为固定金额，每个用户领取的红包金额为total_money除以total_num得到固定金额。  (2) 若prize_type选择为随机金额，每个用户领取的红包金额为total_money除以total_num得到的平均金额值的0.5~1.5倍。由于金额是随机的，在红包金额全部被领取完时，有可能total_num有所剩余、或者大于设置值的情况。
        /// </summary>
        [XmlElement("total_num")]
        public string TotalNum { get; set; }
    }
}
