using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsMktObjectDTO Data Structure.
    /// </summary>
    [Serializable]
    public class InsMktObjectDTO : AopObject
    {
        /// <summary>
        /// 活动标的id
        /// </summary>
        [XmlElement("obj_id")]
        public string ObjId { get; set; }

        /// <summary>
        /// 标的类型
        /// </summary>
        [XmlElement("type")]
        public long Type { get; set; }
    }
}
