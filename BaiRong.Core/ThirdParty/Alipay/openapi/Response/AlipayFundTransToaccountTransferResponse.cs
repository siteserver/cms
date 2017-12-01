using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayFundTransToaccountTransferResponse.
    /// </summary>
    public class AlipayFundTransToaccountTransferResponse : AopResponse
    {
        /// <summary>
        /// 支付宝转账单据号，成功一定返回，失败可能不返回也可能返回。
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// 商户转账唯一订单号：发起转账来源方定义的转账单据号。请求时对应的参数，原样返回。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 支付时间：格式为yyyy-MM-dd HH:mm:ss，仅转账成功返回。
        /// </summary>
        [XmlElement("pay_date")]
        public string PayDate { get; set; }
    }
}
