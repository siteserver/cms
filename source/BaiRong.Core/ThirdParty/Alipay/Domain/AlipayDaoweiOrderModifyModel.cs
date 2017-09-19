using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDaoweiOrderModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDaoweiOrderModifyModel : AopObject
    {
        /// <summary>
        /// 服务地址，修改物流地址时填写的新服务地址：由第三方确认新的服务地址，最长不超过500字符
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 服务开始时间，修改服务开始时间时传递的开始服务时间，格式：yyyy-MM-dd HH:mm（到分）
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 备注信息，修改服务订单操作填写的备注信息，可以是修改的原因，不超过2000字符
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 订单修改类型，枚举值（AMOUNT、OTHER）如果是改金额的话，就传AMOUNT；如果是改开始时间或者物流地址的话，就传OTHER；
        /// </summary>
        [XmlElement("modify_type")]
        public string ModifyType { get; set; }

        /// <summary>
        /// 到位业务订单号，全局唯一，由32位数字组成，用户在到位下单时系统生成并消息同步给商家，商户只能查自己同步到的订单号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单原始金额，即修改前订单的原始金额，单位元，订单金额小于10w
        /// </summary>
        [XmlElement("origin_amount")]
        public string OriginAmount { get; set; }

        /// <summary>
        /// 实际金额，即修改后的订单应收金额，单位为元，订单金额小于10w
        /// </summary>
        [XmlElement("real_amount")]
        public string RealAmount { get; set; }
    }
}
