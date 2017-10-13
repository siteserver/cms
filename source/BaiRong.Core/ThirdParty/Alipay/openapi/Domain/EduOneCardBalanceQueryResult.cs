using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// EduOneCardBalanceQueryResult Data Structure.
    /// </summary>
    [Serializable]
    public class EduOneCardBalanceQueryResult : AopObject
    {
        /// <summary>
        /// 校园一卡通机构
        /// </summary>
        [XmlElement("agent_code")]
        public string AgentCode { get; set; }

        /// <summary>
        /// 校园一卡通可用余额
        /// </summary>
        [XmlElement("balance")]
        public string Balance { get; set; }

        /// <summary>
        /// 校园一卡通姓名
        /// </summary>
        [XmlElement("card_name")]
        public string CardName { get; set; }

        /// <summary>
        /// 校园一卡通卡号
        /// </summary>
        [XmlElement("card_no")]
        public string CardNo { get; set; }

        /// <summary>
        /// 余额最后更新时间
        /// </summary>
        [XmlElement("last_update_time")]
        public string LastUpdateTime { get; set; }

        /// <summary>
        /// 校园一卡通待领金额
        /// </summary>
        [XmlElement("pound_amount")]
        public string PoundAmount { get; set; }
    }
}
