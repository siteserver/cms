using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ArticlePicture Data Structure.
    /// </summary>
    [Serializable]
    public class ArticlePicture : AopObject
    {
        /// <summary>
        /// 图片的描述
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 图片上传到素材中心后生成的id
        /// </summary>
        [XmlElement("location")]
        public string Location { get; set; }

        /// <summary>
        /// 图片名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// "DISH"："菜品"，"ENVIRONMENT"："环境"，"SHOPHEAD"："门头照"，"OTHER"："其他"
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
