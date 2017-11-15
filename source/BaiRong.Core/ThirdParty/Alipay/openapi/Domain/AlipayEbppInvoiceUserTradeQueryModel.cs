using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEbppInvoiceUserTradeQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEbppInvoiceUserTradeQueryModel : AopObject
    {
        /// <summary>
        /// 发票管家交易id，来源于用户支付后开票申请跳转开票方的链接中带入参数einv_trade_id
        /// </summary>
        [XmlElement("einv_trade_id")]
        public string EinvTradeId { get; set; }

        /// <summary>
        /// 随机数，从支付宝钱包链接跳转到开票方外部链接中带入的一项参数，调用该方法需将此参数透传回来，参数名：random
        /// </summary>
        [XmlElement("random")]
        public string Random { get; set; }

        /// <summary>
        /// 时间戳，从支付宝钱包链接跳转到开票方外部链接中带入的一项参数，调用该方法需将此参数透传回来，参数名：timestamp
        /// </summary>
        [XmlElement("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// 令牌，从支付宝钱包链接跳转到开票方外部链接中带入的一项参数，调用该方法需将此参数透传回来,传入时请进行URLEncode,采用utf-编码格式，参数名：token
        /// </summary>
        [XmlElement("token")]
        public string Token { get; set; }
    }
}
