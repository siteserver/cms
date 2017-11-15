using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicInfoModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicInfoModifyModel : AopObject
    {
        /// <summary>
        /// 服务窗名称，2-20个字之间；不得含有违反法律法规和公序良俗的相关信息；不得侵害他人名誉权、知识产权、商业秘密等合法权利；不得以太过广泛的、或产品、行业词组来命名，如：女装、皮革批发；不得以实名认证的媒体资质账号创建服务窗，或媒体相关名称命名服务窗，如：XX电视台、XX杂志等
        /// </summary>
        [XmlElement("app_name")]
        public string AppName { get; set; }

        /// <summary>
        /// 授权运营书，企业商户若为被经营方授权，需上传加盖公章的扫描件，请使用照片上传接口上传图片获得image_url
        /// </summary>
        [XmlElement("auth_pic")]
        public string AuthPic { get; set; }

        /// <summary>
        /// 营业执照地址，建议尺寸 320 x 320px，支持.jpg .jpeg .png 格式，小于3M
        /// </summary>
        [XmlElement("license_url")]
        public string LicenseUrl { get; set; }

        /// <summary>
        /// 服务窗头像地址，建议尺寸 320 x 320px，支持.jpg .jpeg .png 格式，小于3M
        /// </summary>
        [XmlElement("logo_url")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// 服务窗欢迎语，200字以内，首次使用服务窗必须
        /// </summary>
        [XmlElement("public_greeting")]
        public string PublicGreeting { get; set; }

        /// <summary>
        /// 门店照片Url
        /// </summary>
        [XmlArray("shop_pics")]
        [XmlArrayItem("string")]
        public List<string> ShopPics { get; set; }
    }
}
