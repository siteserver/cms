using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayBossCsChannelQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayBossCsChannelQueryModel : AopObject
    {
        /// <summary>
        /// 平均通话时长的qualifier
        /// </summary>
        [XmlElement("att")]
        public string Att { get; set; }

        /// <summary>
        /// 总接通率的qualifier
        /// </summary>
        [XmlElement("connectionrate")]
        public string Connectionrate { get; set; }

        /// <summary>
        /// 在线小二人数的qualifier
        /// </summary>
        [XmlElement("curragentsloggedin")]
        public string Curragentsloggedin { get; set; }

        /// <summary>
        /// 通话中人数的qualifier
        /// </summary>
        [XmlElement("curragenttalking")]
        public string Curragenttalking { get; set; }

        /// <summary>
        /// 小休人数的qualifier
        /// </summary>
        [XmlElement("currentnotreadyagents")]
        public string Currentnotreadyagents { get; set; }

        /// <summary>
        /// 等待人数的qualifier
        /// </summary>
        [XmlElement("currentreadyagents")]
        public string Currentreadyagents { get; set; }

        /// <summary>
        /// 总排队数的Qualifier
        /// </summary>
        [XmlElement("currnumberwaitingcalls")]
        public string Currnumberwaitingcalls { get; set; }

        /// <summary>
        /// 查询hbase的rowkey
        /// </summary>
        [XmlElement("endkey")]
        public string Endkey { get; set; }

        /// <summary>
        /// 查询hbase的rowkey
        /// </summary>
        [XmlElement("startkey")]
        public string Startkey { get; set; }

        /// <summary>
        /// 总流入量的qualifier
        /// </summary>
        [XmlElement("visitorinflow")]
        public string Visitorinflow { get; set; }

        /// <summary>
        /// 总应答量的qualifier
        /// </summary>
        [XmlElement("visitorresponse")]
        public string Visitorresponse { get; set; }

        /// <summary>
        /// 应答量[转接] 的qualifier
        /// </summary>
        [XmlElement("visitorresponsetransfer")]
        public string Visitorresponsetransfer { get; set; }
    }
}
