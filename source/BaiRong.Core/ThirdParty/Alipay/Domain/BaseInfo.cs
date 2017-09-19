using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BaseInfo Data Structure.
    /// </summary>
    [Serializable]
    public class BaseInfo : AopObject
    {
        /// <summary>
        /// 支付宝服务窗名称
        /// </summary>
        [XmlElement("alipay_fuwu_name")]
        public string AlipayFuwuName { get; set; }

        /// <summary>
        /// 如果商户的app需要签约使用移动快捷支付产品，需要上传该app的名称
        /// </summary>
        [XmlElement("app_name")]
        public string AppName { get; set; }

        /// <summary>
        /// 企业联系人信息
        /// </summary>
        [XmlArray("contact_info")]
        [XmlArrayItem("contact_person_info")]
        public List<ContactPersonInfo> ContactInfo { get; set; }

        /// <summary>
        /// 企业logo图片
        /// </summary>
        [XmlElement("logo_pic")]
        public string LogoPic { get; set; }

        /// <summary>
        /// 所属MCCCode，详情可参考  <a href="https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.59bgD2&treeId=222&articleId=105364&docType=1#s1  ">商家经营类目</a>
        /// </summary>
        [XmlElement("mcc_code")]
        public string MccCode { get; set; }

        /// <summary>
        /// 企业别称，例如浙江飞猪网络有限公司别称为阿里旅行。需要签约芝麻信用产品必须要传入该字段
        /// </summary>
        [XmlElement("short_name")]
        public string ShortName { get; set; }

        /// <summary>
        /// 企业特殊资质图片，可参考  <a href="https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.59bgD2&treeId=222&articleId=105364&docType=1#s1  ">商家经营类目</a>
        /// </summary>
        [XmlArray("special_license_pic")]
        [XmlArrayItem("string")]
        public List<string> SpecialLicensePic { get; set; }

        /// <summary>
        /// 使用场景，签约芝麻信用产品必须传入该参数，比如用于放贷风险防控、免押金租车等
        /// </summary>
        [XmlElement("usage_scenario")]
        public string UsageScenario { get; set; }

        /// <summary>
        /// 企业网址信息
        /// </summary>
        [XmlArray("web_address")]
        [XmlArrayItem("string")]
        public List<string> WebAddress { get; set; }

        /// <summary>
        /// 网址授权函图片
        /// </summary>
        [XmlElement("web_auth_pic")]
        public string WebAuthPic { get; set; }

        /// <summary>
        /// 微信公众号名称
        /// </summary>
        [XmlElement("weixin_public_name")]
        public string WeixinPublicName { get; set; }
    }
}
