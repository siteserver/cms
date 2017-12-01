using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BankCardInfo Data Structure.
    /// </summary>
    [Serializable]
    public class BankCardInfo : AopObject
    {
        /// <summary>
        /// 银行卡持卡人姓名
        /// </summary>
        [XmlElement("card_name")]
        public string CardName { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        [XmlElement("card_no")]
        public string CardNo { get; set; }
    }
}
