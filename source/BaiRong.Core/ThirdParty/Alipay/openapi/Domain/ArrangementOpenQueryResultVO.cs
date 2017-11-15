using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ArrangementOpenQueryResultVO Data Structure.
    /// </summary>
    [Serializable]
    public class ArrangementOpenQueryResultVO : AopObject
    {
        /// <summary>
        /// 合约编号
        /// </summary>
        [XmlElement("ar_no")]
        public string ArNo { get; set; }

        /// <summary>
        /// 合约状态  未生效：UN_INVALID  已取消：CANCEL  已生效：VALID  已失效：INVALID
        /// </summary>
        [XmlElement("ar_status")]
        public string ArStatus { get; set; }

        /// <summary>
        /// JSON结构的扩展字段，备用字段
        /// </summary>
        [XmlElement("ext_data")]
        public string ExtData { get; set; }

        /// <summary>
        /// 有效期截止时间
        /// </summary>
        [XmlElement("invalid_date")]
        public string InvalidDate { get; set; }

        /// <summary>
        /// 签约时间
        /// </summary>
        [XmlElement("sign_date")]
        public string SignDate { get; set; }

        /// <summary>
        /// 有效期起始时间
        /// </summary>
        [XmlElement("valid_date")]
        public string ValidDate { get; set; }
    }
}
