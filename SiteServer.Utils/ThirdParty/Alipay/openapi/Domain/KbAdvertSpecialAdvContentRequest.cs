using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertSpecialAdvContentRequest Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertSpecialAdvContentRequest : AopObject
    {
        /// <summary>
        /// 红包口令模型，创建红包口令时传入该模型
        /// </summary>
        [XmlElement("content_password_modify")]
        public KbAdvertContentPasswordModify ContentPasswordModify { get; set; }

        /// <summary>
        /// 广告内容类型；  当值是passwordRed时，表示修改口令红包，需要传入content_password_modify模型的参数；
        /// </summary>
        [XmlElement("content_type")]
        public string ContentType { get; set; }
    }
}
