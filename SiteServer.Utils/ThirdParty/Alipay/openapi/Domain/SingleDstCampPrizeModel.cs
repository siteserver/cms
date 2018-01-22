using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SingleDstCampPrizeModel Data Structure.
    /// </summary>
    [Serializable]
    public class SingleDstCampPrizeModel : AopObject
    {
        /// <summary>
        /// 预算id
        /// </summary>
        [XmlElement("budget_id")]
        public string BudgetId { get; set; }

        /// <summary>
        /// 奖品id,新增不传，修改传
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 折扣幅度只能填写数字，大于0，小数点后最多2位，整数部分不能超过8位
        /// </summary>
        [XmlElement("reduce_amt")]
        public string ReduceAmt { get; set; }
    }
}
