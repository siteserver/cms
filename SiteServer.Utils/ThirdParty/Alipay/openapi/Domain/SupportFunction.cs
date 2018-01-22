using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SupportFunction Data Structure.
    /// </summary>
    [Serializable]
    public class SupportFunction : AopObject
    {
        /// <summary>
        /// 卡名称
        /// </summary>
        [XmlElement("card_name")]
        public string CardName { get; set; }

        /// <summary>
        /// 卡类型编码，为智能卡系统的内部编码规则
        /// </summary>
        [XmlElement("card_type")]
        public string CardType { get; set; }

        /// <summary>
        /// 功能，支持开卡(issue)，圈存(load)，充值转账(recharge)
        /// </summary>
        [XmlArray("function_type")]
        [XmlArrayItem("string")]
        public List<string> FunctionType { get; set; }

        /// <summary>
        /// 智能卡的跳转地址
        /// </summary>
        [XmlElement("goto_url")]
        public string GotoUrl { get; set; }
    }
}
