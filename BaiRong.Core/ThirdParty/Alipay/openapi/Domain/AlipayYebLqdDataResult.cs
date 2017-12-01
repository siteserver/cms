using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayYebLqdDataResult Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayYebLqdDataResult : AopObject
    {
        /// <summary>
        /// 申购预测，单位:元
        /// </summary>
        [XmlElement("predict_purchase_amt")]
        public string PredictPurchaseAmt { get; set; }

        /// <summary>
        /// 赎回预测,单位:元
        /// </summary>
        [XmlElement("predict_redeem_amt")]
        public string PredictRedeemAmt { get; set; }

        /// <summary>
        /// 预测日期，格式为yyyymmdd
        /// </summary>
        [XmlElement("target_date")]
        public string TargetDate { get; set; }
    }
}
