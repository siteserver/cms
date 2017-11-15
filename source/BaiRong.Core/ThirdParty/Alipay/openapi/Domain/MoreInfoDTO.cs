using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MoreInfoDTO Data Structure.
    /// </summary>
    [Serializable]
    public class MoreInfoDTO : AopObject
    {
        /// <summary>
        /// 选择opennative的时候必须填写descs的内容
        /// </summary>
        [XmlArray("descs")]
        [XmlArrayItem("string")]
        public List<string> Descs { get; set; }

        /// <summary>
        /// 扩展参数，需要URL地址回带的值，JSON格式(openweb时填)
        /// </summary>
        [XmlElement("params")]
        public string Params { get; set; }

        /// <summary>
        /// 二级页面标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 超链接(选择openweb的时候必须填写url参数内容)
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }
    }
}
