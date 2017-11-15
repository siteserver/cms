using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CraftsmanWorkOutIdOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class CraftsmanWorkOutIdOpenModel : AopObject
    {
        /// <summary>
        /// 外部作品id，isv生成，isv的appId+out_work_id全局唯一
        /// </summary>
        [XmlElement("out_work_id")]
        public string OutWorkId { get; set; }

        /// <summary>
        /// 支付宝生成的作品id，全局唯一，用于修改删除作品
        /// </summary>
        [XmlElement("work_id")]
        public string WorkId { get; set; }
    }
}
