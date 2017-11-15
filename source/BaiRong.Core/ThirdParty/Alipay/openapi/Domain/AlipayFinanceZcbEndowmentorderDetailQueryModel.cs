using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFinanceZcbEndowmentorderDetailQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFinanceZcbEndowmentorderDetailQueryModel : AopObject
    {
        /// <summary>
        /// 查询条件里时间区间的结束时间，格式为：YYYYMMDDHHMISS，采用左开右闭的方式
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// product_id：产品id，可以咨询蚂蚁这边的同学给出需要查询的产品id，查询订单只能按照产品纬度来查
        /// </summary>
        [XmlElement("product_id")]
        public string ProductId { get; set; }

        /// <summary>
        /// 查询条件里时间区间的开始时间，格式：YYYYMMDDHHMISS，查询订单的开始时间，采用左开右闭的方式
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }
    }
}
