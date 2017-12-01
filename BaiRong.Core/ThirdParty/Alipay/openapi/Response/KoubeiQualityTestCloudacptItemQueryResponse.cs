using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiQualityTestCloudacptItemQueryResponse.
    /// </summary>
    public class KoubeiQualityTestCloudacptItemQueryResponse : AopResponse
    {
        /// <summary>
        /// 活动id
        /// </summary>
        [XmlElement("activity_id")]
        public string ActivityId { get; set; }

        /// <summary>
        /// 批次id
        /// </summary>
        [XmlElement("batch_id")]
        public string BatchId { get; set; }

        /// <summary>
        /// 批次状态  0，未检测  1，检测中  2，未通过  3，已通过
        /// </summary>
        [XmlElement("batch_status")]
        public string BatchStatus { get; set; }

        /// <summary>
        /// 失败单品书列表
        /// </summary>
        [XmlArray("fail_list")]
        [XmlArrayItem("open_item")]
        public List<OpenItem> FailList { get; set; }

        /// <summary>
        /// 失败数
        /// </summary>
        [XmlElement("fail_num")]
        public string FailNum { get; set; }

        /// <summary>
        /// 单品列表
        /// </summary>
        [XmlArray("item_list")]
        [XmlArrayItem("open_item")]
        public List<OpenItem> ItemList { get; set; }

        /// <summary>
        /// 单品数
        /// </summary>
        [XmlElement("item_num")]
        public string ItemNum { get; set; }

        /// <summary>
        /// 通过单品列表
        /// </summary>
        [XmlArray("pass_list")]
        [XmlArrayItem("open_item")]
        public List<OpenItem> PassList { get; set; }

        /// <summary>
        /// 通过数
        /// </summary>
        [XmlElement("pass_num")]
        public string PassNum { get; set; }
    }
}
