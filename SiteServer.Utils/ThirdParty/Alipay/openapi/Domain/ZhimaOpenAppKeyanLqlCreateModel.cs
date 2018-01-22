using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaOpenAppKeyanLqlCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaOpenAppKeyanLqlCreateModel : AopObject
    {
        /// <summary>
        /// 参数描述必须通俗易懂、无错别字、完整。描述的内容请按此格式填写：参数名+是1否唯一(如需)+应用场景+枚举值(如有)+如何获取+特殊说明(如有)。如不符合标准终审会驳回，影响上线时间。
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }
    }
}
