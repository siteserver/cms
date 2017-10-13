using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DiscountRateModel Data Structure.
    /// </summary>
    [Serializable]
    public class DiscountRateModel : AopObject
    {
        /// <summary>
        /// 折扣方式
        /// </summary>
        [XmlElement("discount_dst_camp_prize_model")]
        public DiscountDstCampPrizeModel DiscountDstCampPrizeModel { get; set; }

        /// <summary>
        /// 交易金额下限必须为数字，大于0，最多2位小数，整数部分不能超过8位
        /// </summary>
        [XmlElement("lower_trade_fee")]
        public string LowerTradeFee { get; set; }

        /// <summary>
        /// 奖品类型. 打折   满减   单笔减   阶梯优惠   抹零优惠    随机立减   订单金额减至        折扣方式     REDUCE_TO_AMT("reduce_to_amt","优惠后价格")     DISCOUNT("discount", "折扣方式"),    REDUCE("reduce", "满立减"),     SINGLE("single", "单笔减"),
        /// </summary>
        [XmlElement("prize_type")]
        public string PrizeType { get; set; }

        /// <summary>
        /// 满立减
        /// </summary>
        [XmlElement("reduce_dst_camp_prize_model")]
        public ReduceDstCampPrizeModel ReduceDstCampPrizeModel { get; set; }

        /// <summary>
        /// 优惠后价格 如果type选了reduce_to_amt 必填
        /// </summary>
        [XmlElement("reduce_to_amt_dst_camp_prize_model")]
        public ReduceToAmtDstCampPrizeModel ReduceToAmtDstCampPrizeModel { get; set; }

        /// <summary>
        /// 单笔减
        /// </summary>
        [XmlElement("single_dst_camp_prize_model")]
        public SingleDstCampPrizeModel SingleDstCampPrizeModel { get; set; }

        /// <summary>
        /// 交易金额上限必须为数字，大于0，最多2位小数，整数部分不能超过8位
        /// </summary>
        [XmlElement("upper_trade_fee")]
        public string UpperTradeFee { get; set; }
    }
}
