using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiQualityTestCloudacptBatchQueryResponse.
    /// </summary>
    public class KoubeiQualityTestCloudacptBatchQueryResponse : AopResponse
    {
        /// <summary>
        /// 活动id
        /// </summary>
        [XmlElement("activity_id")]
        public string ActivityId { get; set; }

        /// <summary>
        /// 批次列表
        /// </summary>
        [XmlArray("batch_list")]
        [XmlArrayItem("open_batch")]
        public List<OpenBatch> BatchList { get; set; }

        /// <summary>
        /// 单品批次数
        /// </summary>
        [XmlElement("batch_num")]
        public string BatchNum { get; set; }
    }
}
