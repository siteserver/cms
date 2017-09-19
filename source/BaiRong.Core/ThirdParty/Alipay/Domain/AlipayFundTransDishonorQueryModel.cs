using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFundTransDishonorQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFundTransDishonorQueryModel : AopObject
    {
        /// <summary>
        /// 查询退票起始时间:（大于等于），格式为yyyyMMdd。  用于查询退票起始日期00:00:00后发生的退票。  与refund_end差距不得大于15天。
        /// </summary>
        [XmlElement("dishonor_begin")]
        public string DishonorBegin { get; set; }

        /// <summary>
        /// 查询退票截止时间：（小于），格式为yyyyMMdd。  用于查询退票截止日期24:00:00前发生的退票。  与refund_begin差距不得大于15天。
        /// </summary>
        [XmlElement("dishonor_end")]
        public string DishonorEnd { get; set; }

        /// <summary>
        /// 查询页号。  必须是正整数。  默认值为1。
        /// </summary>
        [XmlElement("page")]
        public string Page { get; set; }
    }
}
