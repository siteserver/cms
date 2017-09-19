using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarMaintainOrderCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarMaintainOrderCreateModel : AopObject
    {
        /// <summary>
        /// 业务订单状态
        /// </summary>
        [XmlElement("biz_status")]
        public string BizStatus { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        [XmlElement("biz_status_txt")]
        public string BizStatusTxt { get; set; }

        /// <summary>
        /// 拓展参数，区分4S厂商  {”remark”:”DFRC”}  日产- DFRC,  北现- BJXD
        /// </summary>
        [XmlElement("ext_param")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 业务订单编号
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 交易主题
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 交易摘要
        /// </summary>
        [XmlElement("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// 交易金额，单位：分
        /// </summary>
        [XmlElement("total_fee")]
        public string TotalFee { get; set; }

        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
