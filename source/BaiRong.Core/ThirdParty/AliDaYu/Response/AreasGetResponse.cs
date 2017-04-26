using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AreasGetResponse.
    /// </summary>
    public class AreasGetResponse : TopResponse
    {
        /// <summary>
        /// 地址区域信息列表.返回的Area包含的具体信息为入参fields请求的字段信息.
        /// </summary>
        [XmlArray("areas")]
        [XmlArrayItem("area")]
        public List<Top.Api.Domain.Area> Areas { get; set; }

    }
}
