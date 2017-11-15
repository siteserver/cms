using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayTradeBatchRefundModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayTradeBatchRefundModel : AopObject
    {
        /// <summary>
        /// 每进行一次即时到账批量退款，都需要提供一个批次号，通过该批次号可以查询这一批次的退款交易记录。对于每一个合作伙伴，传递的每一个批次号都必须保证唯一性。  格式为：退款日期（8位当天日期）+流水号（3～24位，流水号可以接受数字或英文字符，建议使用数字）。
        /// </summary>
        [XmlElement("batch_no")]
        public string BatchNo { get; set; }

        /// <summary>
        /// 退款明细的笔数，即参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔。
        /// </summary>
        [XmlElement("batch_num")]
        public string BatchNum { get; set; }

        /// <summary>
        /// 退款明细列表
        /// </summary>
        [XmlArray("detail_data")]
        [XmlArrayItem("refund_detail")]
        public List<RefundDetail> DetailData { get; set; }

        /// <summary>
        /// 退款请求的当前时间。  格式为：yyyy-MM-dd hh:mm:ss。
        /// </summary>
        [XmlElement("refund_date")]
        public string RefundDate { get; set; }

        /// <summary>
        /// 是否使用冻结金额退款。  Y：可以使用冻结金额退款；  N：不可使用冻结金额退款；  如果不提供，则默认值为N。
        /// </summary>
        [XmlElement("use_freeze_amount")]
        public string UseFreezeAmount { get; set; }
    }
}
