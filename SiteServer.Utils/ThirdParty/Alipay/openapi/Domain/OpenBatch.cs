using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OpenBatch Data Structure.
    /// </summary>
    [Serializable]
    public class OpenBatch : AopObject
    {
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
        /// 单品数
        /// </summary>
        [XmlElement("item_num")]
        public string ItemNum { get; set; }
    }
}
