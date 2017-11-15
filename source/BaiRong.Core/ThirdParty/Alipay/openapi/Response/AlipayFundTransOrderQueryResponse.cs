using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayFundTransOrderQueryResponse.
    /// </summary>
    public class AlipayFundTransOrderQueryResponse : AopResponse
    {
        /// <summary>
        /// 预计到账时间，转账到银行卡专用，格式为yyyy-MM-dd HH:mm:ss，转账受理失败不返回。     注意：  此参数为预计时间，可能与实际到账时间有较大误差，不能作为实际到账时间使用，仅供参考用途。
        /// </summary>
        [XmlElement("arrival_time_end")]
        public string ArrivalTimeEnd { get; set; }

        /// <summary>
        /// 查询失败时，本参数为错误代 码。   查询成功不返回。 对于退票订单，不返回该参数。
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 查询到的订单状态为FAIL失败或REFUND退票时，返回具体的原因。
        /// </summary>
        [XmlElement("fail_reason")]
        public string FailReason { get; set; }

        /// <summary>
        /// 预计收费金额（元），转账到银行卡专用，数字格式，精确到小数点后2位，转账失败或转账受理失败不返回。
        /// </summary>
        [XmlElement("order_fee")]
        public string OrderFee { get; set; }

        /// <summary>
        /// 支付宝转账单据号，查询失败不返回。
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// 发起转账来源方定义的转账单据号。   该参数的赋值均以查询结果中 的 out_biz_no 为准。   如果查询失败，不返回该参数。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 支付时间，格式为yyyy-MM-dd HH:mm:ss，转账失败不返回。
        /// </summary>
        [XmlElement("pay_date")]
        public string PayDate { get; set; }

        /// <summary>
        /// 转账单据状态。   SUCCESS：成功（配合"单笔转账到银行账户接口"产品使用时, 同一笔单据多次查询有可能从成功变成退票状态）；   FAIL：失败（具体失败原因请参见error_code以及fail_reason返回值）；   INIT：等待处理；   DEALING：处理中；   REFUND：退票（仅配合"单笔转账到银行账户接口"产品使用时会涉及, 具体退票原因请参见fail_reason返回值）；   UNKNOWN：状态未知。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
