using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OldComplextMockModel Data Structure.
    /// </summary>
    [Serializable]
    public class OldComplextMockModel : AopObject
    {
        /// <summary>
        /// biz_num
        /// </summary>
        [XmlElement("biz_num")]
        public long BizNum { get; set; }

        /// <summary>
        /// biz_type
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 简单model
        /// </summary>
        [XmlElement("simple_mock_model")]
        public SimpleMockModel SimpleMockModel { get; set; }
    }
}
