using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// FengdieTemplate Data Structure.
    /// </summary>
    [Serializable]
    public class FengdieTemplate : AopObject
    {
        /// <summary>
        /// 模板包唯一id，上传模板时自动生成
        /// </summary>
        [XmlElement("id")]
        public long Id { get; set; }

        /// <summary>
        /// 模板包开发者，由开发者在package.json中指定
        /// </summary>
        [XmlArray("maintainer")]
        [XmlArrayItem("string")]
        public List<string> Maintainer { get; set; }

        /// <summary>
        /// 模板包名称，开发模板时由开发者在package.json里指定
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 模板包预览图，开发者在模板根目录放置的一张命名为snapshot.png的图片
        /// </summary>
        [XmlElement("snapshot")]
        public string Snapshot { get; set; }

        /// <summary>
        /// 模板包描述，开发者在package.json里指定
        /// </summary>
        [XmlElement("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// 模板包中文标题，开发者在fengdie.config.js里指定
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}
