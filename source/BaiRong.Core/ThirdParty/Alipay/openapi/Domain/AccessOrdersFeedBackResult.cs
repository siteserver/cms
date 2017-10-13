using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AccessOrdersFeedBackResult Data Structure.
    /// </summary>
    [Serializable]
    public class AccessOrdersFeedBackResult : AopObject
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        [XmlElement("error_desc")]
        public string ErrorDesc { get; set; }

        /// <summary>
        /// 反馈主键ID（生产单ID或者采购单ID或者码token）
        /// </summary>
        [XmlElement("feedback_id")]
        public string FeedbackId { get; set; }

        /// <summary>
        /// 生产单：PRODUCE_ORDER  采购单：PURCHASE_ORDER  二维码：QRCODE
        /// </summary>
        [XmlElement("order_type")]
        public string OrderType { get; set; }

        /// <summary>
        /// 外部单据号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 每条记录处理结果
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }
    }
}
