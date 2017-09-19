using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsApplicationQuery Data Structure.
    /// </summary>
    [Serializable]
    public class InsApplicationQuery : AopObject
    {
        /// <summary>
        /// 投保订单号
        /// </summary>
        [XmlElement("application_no")]
        public string ApplicationNo { get; set; }

        /// <summary>
        /// 投保单状态;此状态用于判断投保流程的推进过程.INIT: 初始,UNDERWROTE:已核保, DECLINED:已拒保,CLOSED:已关闭, PAID:已付款,REFUND:已退款,ISSUED:已出单
        /// </summary>
        [XmlElement("application_status")]
        public string ApplicationStatus { get; set; }

        /// <summary>
        /// 保险机构
        /// </summary>
        [XmlElement("merchant")]
        public InsMerchant Merchant { get; set; }

        /// <summary>
        /// 交易操作流水号;用于跳支付宝收银台付款
        /// </summary>
        [XmlElement("operation_id")]
        public string OperationId { get; set; }

        /// <summary>
        /// 商户生成的外部投保业务号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 产品编码;由蚂蚁保险平台分配,商户通过该产品编码投保特定的保险产品
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }

        /// <summary>
        /// 支付交易订单号;用于跳支付宝收银台付款
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
