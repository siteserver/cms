using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayBossCsChannelQueryResponse.
    /// </summary>
    public class AlipayBossCsChannelQueryResponse : AopResponse
    {
        /// <summary>
        /// 平均通话时长
        /// </summary>
        [XmlElement("att")]
        public string Att { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// 接通率
        /// </summary>
        [XmlElement("connection_rate")]
        public string ConnectionRate { get; set; }

        /// <summary>
        /// 通话中人数
        /// </summary>
        [XmlElement("curr_agent_talking")]
        public string CurrAgentTalking { get; set; }

        /// <summary>
        /// 在线小二数
        /// </summary>
        [XmlElement("curr_agents_logged_in")]
        public string CurrAgentsLoggedIn { get; set; }

        /// <summary>
        /// 排队数
        /// </summary>
        [XmlElement("curr_number_waiting_calls")]
        public string CurrNumberWaitingCalls { get; set; }

        /// <summary>
        /// 小休人数
        /// </summary>
        [XmlElement("current_not_ready_agents")]
        public string CurrentNotReadyAgents { get; set; }

        /// <summary>
        /// 等待人数
        /// </summary>
        [XmlElement("current_ready_agents")]
        public string CurrentReadyAgents { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        [XmlElement("row_key")]
        public string RowKey { get; set; }

        /// <summary>
        /// 流入量
        /// </summary>
        [XmlElement("visitor_inflow")]
        public string VisitorInflow { get; set; }

        /// <summary>
        /// 应答量
        /// </summary>
        [XmlElement("visitor_response")]
        public string VisitorResponse { get; set; }

        /// <summary>
        /// 应答量[转接]
        /// </summary>
        [XmlElement("visitor_response_transfer")]
        public string VisitorResponseTransfer { get; set; }
    }
}
