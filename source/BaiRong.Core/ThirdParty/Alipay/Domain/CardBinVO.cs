using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CardBinVO Data Structure.
    /// </summary>
    [Serializable]
    public class CardBinVO : AopObject
    {
        /// <summary>
        /// 卡的别名
        /// </summary>
        [XmlElement("card_alias")]
        public string CardAlias { get; set; }

        /// <summary>
        /// 卡bin值，通常为卡号的前6位
        /// </summary>
        [XmlElement("card_bin_value")]
        public string CardBinValue { get; set; }

        /// <summary>
        /// 卡类型对象定义
        /// </summary>
        [XmlElement("card_type_vo")]
        public CardTypeVO CardTypeVo { get; set; }

        /// <summary>
        /// 卡域模型定义
        /// </summary>
        [XmlElement("domain_vo")]
        public CardDomainVO DomainVo { get; set; }

        /// <summary>
        /// 机构内标
        /// </summary>
        [XmlElement("inst_id")]
        public string InstId { get; set; }

        /// <summary>
        /// 卡号长度
        /// </summary>
        [XmlElement("inst_len")]
        public string InstLen { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        [XmlElement("operator")]
        public string Operator { get; set; }

        /// <summary>
        /// 卡版本信息
        /// </summary>
        [XmlElement("version")]
        public string Version { get; set; }
    }
}
