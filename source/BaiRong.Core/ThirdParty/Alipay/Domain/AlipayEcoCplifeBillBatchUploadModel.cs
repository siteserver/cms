using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoCplifeBillBatchUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoCplifeBillBatchUploadModel : AopObject
    {
        /// <summary>
        /// 每次上传物业费账单，都需要提供一个批次号。对于每一个合作伙伴，传递的每一个批次号都必须保证唯一性，同时对于批次号内的账单明细数据必须保证唯一性；  建议格式为：8位当天日期+流水号（3～24位，流水号可以接受数字或英文字符，建议使用数字）。
        /// </summary>
        [XmlElement("batch_id")]
        public string BatchId { get; set; }

        /// <summary>
        /// 账单应收条目明细集合，同一小区内条目明细不允许重复；一次接口请求最多支持1000条明细。
        /// </summary>
        [XmlArray("bill_set")]
        [XmlArrayItem("c_p_bill_set")]
        public List<CPBillSet> BillSet { get; set; }

        /// <summary>
        /// 支付宝社区小区统一编号，必须在物业账号名下存在。
        /// </summary>
        [XmlElement("community_id")]
        public string CommunityId { get; set; }
    }
}
