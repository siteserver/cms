using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ReduceDstCampPrizeModel Data Structure.
    /// </summary>
    [Serializable]
    public class ReduceDstCampPrizeModel : AopObject
    {
        /// <summary>
        /// 折扣预算ID
        /// </summary>
        [XmlElement("budget_id")]
        public string BudgetId { get; set; }

        /// <summary>
        /// 奖品id, 修改必输，新增不输入
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 单次优惠上限(元)
        /// </summary>
        [XmlElement("max_discount_amt")]
        public string MaxDiscountAmt { get; set; }

        /// <summary>
        /// 满的金额只能填写正整数字，大于0，必须小于9位
        /// </summary>
        [XmlElement("reduce_amt_pre")]
        public string ReduceAmtPre { get; set; }

        /// <summary>
        /// 减的金额只能填写数字，大于0，小数点后最多2位，整数部分不能超过8位
        /// </summary>
        [XmlElement("reduce_amt_suf")]
        public string ReduceAmtSuf { get; set; }
    }
}
