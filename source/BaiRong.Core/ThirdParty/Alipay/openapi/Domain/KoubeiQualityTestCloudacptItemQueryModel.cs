using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiQualityTestCloudacptItemQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiQualityTestCloudacptItemQueryModel : AopObject
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
        /// partener id
        /// </summary>
        [XmlElement("pid")]
        public string Pid { get; set; }

        /// <summary>
        /// user id
        /// </summary>
        [XmlElement("uid")]
        public string Uid { get; set; }
    }
}
