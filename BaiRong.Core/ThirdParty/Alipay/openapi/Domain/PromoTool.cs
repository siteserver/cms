using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PromoTool Data Structure.
    /// </summary>
    [Serializable]
    public class PromoTool : AopObject
    {
        /// <summary>
        /// 集点卡工具，仅在活动类型为POINT_SEND时才有效且必填，其他活动类型此字段必须为null
        /// </summary>
        [XmlElement("point_card")]
        public PointCard PointCard { get; set; }

        /// <summary>
        /// 奖品发放的规则
        /// </summary>
        [XmlElement("send_rule")]
        public SendRule SendRule { get; set; }

        /// <summary>
        /// 单个营销工具的生效状态，当在招商部分券失效后会使用这个字段
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 券对象，当活动类型为POINT_SEND时为null，其他活动类型此字段必填
        /// </summary>
        [XmlElement("voucher")]
        public Voucher Voucher { get; set; }

        /// <summary>
        /// 营销工具uid,创建营销活动时无需设置
        /// </summary>
        [XmlElement("voucher_no")]
        public string VoucherNo { get; set; }
    }
}
