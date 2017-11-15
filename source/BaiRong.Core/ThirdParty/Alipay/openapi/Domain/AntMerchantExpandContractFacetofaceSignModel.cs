using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntMerchantExpandContractFacetofaceSignModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntMerchantExpandContractFacetofaceSignModel : AopObject
    {
        /// <summary>
        /// 营业执照授权函图片，个体工商户如果使用总公司或其他公司的营业执照认证需上传该授权函图片，通过ant.merchant.expand.image.upload接口上传营业执照授权函图片
        /// </summary>
        [XmlElement("business_license_auth_pic")]
        public string BusinessLicenseAuthPic { get; set; }

        /// <summary>
        /// 营业执照号码
        /// </summary>
        [XmlElement("business_license_no")]
        public string BusinessLicenseNo { get; set; }

        /// <summary>
        /// 营业执照图片，通过ant.merchant.expand.image.upload接口上传营业执照图片
        /// </summary>
        [XmlElement("business_license_pic")]
        public string BusinessLicensePic { get; set; }

        /// <summary>
        /// 联系人邮箱地址
        /// </summary>
        [XmlElement("contact_email")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        [XmlElement("contact_mobile")]
        public string ContactMobile { get; set; }

        /// <summary>
        /// 企业联系人名称
        /// </summary>
        [XmlElement("contact_name")]
        public string ContactName { get; set; }

        /// <summary>
        /// 所属MCCCode，详情可参考  <a href="https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.59bgD2&treeId=222&articleId=105364&docType=1#s1  ">商家经营类目</a> 中的“经营类目编码”
        /// </summary>
        [XmlElement("mcc_code")]
        public string MccCode { get; set; }

        /// <summary>
        /// 外部入驻申请单据号，由开发者生成，并需保证在开发者端不重复
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 店铺内景图片，个人账户必填，企业账户选填，通过ant.merchant.expand.image.upload接口上传店铺内景图片
        /// </summary>
        [XmlElement("shop_scene_pic")]
        public string ShopScenePic { get; set; }

        /// <summary>
        /// 店铺门头照图片，个人账户必填，企业账户选填，通过ant.merchant.expand.image.upload接口上传店铺门头照图片
        /// </summary>
        [XmlElement("shop_sign_board_pic")]
        public string ShopSignBoardPic { get; set; }

        /// <summary>
        /// 企业特殊资质图片，可参考  <a href="https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.59bgD2&treeId=222&articleId=105364&docType=1#s1  ">商家经营类目</a> 中的“需要的特殊资质证书”   通过ant.merchant.expand.image.upload接口上传企业特殊资质图片
        /// </summary>
        [XmlElement("special_license_pic")]
        public string SpecialLicensePic { get; set; }
    }
}
