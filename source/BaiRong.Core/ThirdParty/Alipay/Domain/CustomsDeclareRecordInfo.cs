using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CustomsDeclareRecordInfo Data Structure.
    /// </summary>
    [Serializable]
    public class CustomsDeclareRecordInfo : AopObject
    {
        /// <summary>
        /// 支付宝报关流水号。
        /// </summary>
        [XmlElement("alipay_declare_no")]
        public string AlipayDeclareNo { get; set; }

        /// <summary>
        /// 报关金额，单位为人民币“元”，精确到小数点后2位。
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 此记录所报关的海关编号，参见“海关编号”。
        /// </summary>
        [XmlElement("customs_place")]
        public string CustomsPlace { get; set; }

        /// <summary>
        /// 发起报关后，海关返回回执中的结果码。目前只有总署的报关，并且总署回执接收成功的请求才会返回此参数  2：电子口岸申报中  3：发送海关成功  4：发送海关失败  100：海关退单  399：海关审结  小于0的数字：表示处理异常回执     注意：  支付宝原样返回海关返回的数据，参数值以海关的定义为准。
        /// </summary>
        [XmlElement("customs_result_code")]
        public string CustomsResultCode { get; set; }

        /// <summary>
        /// 发起报关后，海关返回回执中的结果描述信息。目前只有总署报关，并且总署成功返回回执的时候会有此值
        /// </summary>
        [XmlElement("customs_result_info")]
        public string CustomsResultInfo { get; set; }

        /// <summary>
        /// 发起报关后，海关返回回执的时间，格式为：yyyyMMddHHmmss。目前只有总署报关，并且总署成功返回回执的时候才会有此参数。
        /// </summary>
        [XmlElement("customs_result_return_time")]
        public string CustomsResultReturnTime { get; set; }

        /// <summary>
        /// T: 拆单；F：非拆单。当请求没有拆单或者请求传入的is_split=F时，不会返回此参数。
        /// </summary>
        [XmlElement("is_split")]
        public string IsSplit { get; set; }

        /// <summary>
        /// 报关记录状态最后更新时间
        /// </summary>
        [XmlElement("last_modified_time")]
        public string LastModifiedTime { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户在海关备案的编号。
        /// </summary>
        [XmlElement("merchant_customs_code")]
        public string MerchantCustomsCode { get; set; }

        /// <summary>
        /// 商户海关备案名称
        /// </summary>
        [XmlElement("merchant_customs_name")]
        public string MerchantCustomsName { get; set; }

        /// <summary>
        /// 报关请求号。商户端报关请求号，对应入参中的某条报关请求号。
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }

        /// <summary>
        /// 该报关单当前的状态：  - ws等待发送海关  - sending已提交发送海关  - succ 海关返回受理成功
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 拆单子订单号。如果报关请求没有请求拆单则不会返回此参数。
        /// </summary>
        [XmlElement("sub_out_biz_no")]
        public string SubOutBizNo { get; set; }

        /// <summary>
        /// 支付宝推送到海关的支付单据号。针对拆单的报关，这个单据号不等于支付宝原始交易号。
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
