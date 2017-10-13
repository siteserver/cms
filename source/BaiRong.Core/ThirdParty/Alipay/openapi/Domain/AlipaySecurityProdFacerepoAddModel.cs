using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityProdFacerepoAddModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityProdFacerepoAddModel : AopObject
    {
        /// <summary>
        /// 商户的业务单据号，用于核对与问题排查
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 人脸图片字节数组进行Base64编码后的字符串
        /// </summary>
        [XmlElement("face_str")]
        public string FaceStr { get; set; }

        /// <summary>
        /// 人脸库分组。每个商户可以使用多个人脸库分组，人脸搜索时会在指定的人脸库分组中搜索
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }
    }
}
