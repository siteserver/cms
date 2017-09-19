using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CardTypeVO Data Structure.
    /// </summary>
    [Serializable]
    public class CardTypeVO : AopObject
    {
        /// <summary>
        /// 卡类型标识符，取值范围如下：  DC("借记卡")  CC("贷记卡")  SCC("准贷记卡")  DCC("存贷合一卡")  PC("预付卡")  STPB("标准存折")  STFA("标准对公账户")  NSTFA("非标准对公账户")
        /// </summary>
        [XmlElement("card_type")]
        public string CardType { get; set; }

        /// <summary>
        /// 卡类型描述，参考cardType的描述字段中括号里的值。
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }
    }
}
