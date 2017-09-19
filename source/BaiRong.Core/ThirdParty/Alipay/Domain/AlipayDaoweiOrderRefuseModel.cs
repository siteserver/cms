using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDaoweiOrderRefuseModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDaoweiOrderRefuseModel : AopObject
    {
        /// <summary>
        /// 到位业务订单号，全局唯一，由32位数字组成，用户在到位下单时系统生成并消息同步给商家，商户只能查自己同步到的订单号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 拒单理由，第三方商户拒绝接单时填写的拒单理由，必填，长度不超过500字符
        /// </summary>
        [XmlElement("reason")]
        public string Reason { get; set; }
    }
}
