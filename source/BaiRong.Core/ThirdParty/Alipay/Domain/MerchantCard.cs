using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MerchantCard Data Structure.
    /// </summary>
    [Serializable]
    public class MerchantCard : AopObject
    {
        /// <summary>
        /// 资金卡余额，单位：元，精确到小数点后两位。
        /// </summary>
        [XmlElement("balance")]
        public string Balance { get; set; }

        /// <summary>
        /// 支付宝业务卡号  说明：  1、开卡成功后返回该参数，需要保存留用；  2、开卡/更新/删卡/查询卡接口请求中不需要传该参数；
        /// </summary>
        [XmlElement("biz_card_no")]
        public string BizCardNo { get; set; }

        /// <summary>
        /// 商户外部会员卡卡号  说明：  1、会员卡开卡接口，如果卡类型为外部会员卡，请求中则必须提供该参数；  2、更新、查询、删除等接口，请求中则不需要提供该参数值；
        /// </summary>
        [XmlElement("external_card_no")]
        public string ExternalCardNo { get; set; }

        /// <summary>
        /// 卡面展示图片的图片ID，通过接口（alipay.offline.material.image.upload）上传图片    这里预期展示的是个人照片；  图片说明：1M以内，格式bmp、png、jpeg、jpg、gif；  图片尺寸为230*295px，可等比放大；
        /// </summary>
        [XmlElement("front_image_id")]
        public string FrontImageId { get; set; }

        /// <summary>
        /// 卡面文案列表，1项对应1行文案，最多只能传入4行文案；  单行文案展现分为左右两部分，左边对应label字段，右边对应value；  形如： 学院    新闻学院
        /// </summary>
        [XmlArray("front_text_list")]
        [XmlArrayItem("card_front_text_d_t_o")]
        public List<CardFrontTextDTO> FrontTextList { get; set; }

        /// <summary>
        /// 会员卡等级（由商户自定义，并可以在卡模板创建时，定义等级信息）
        /// </summary>
        [XmlElement("level")]
        public string Level { get; set; }

        /// <summary>
        /// 商户动态码回传信息：  只用于当write_off_type核销类型为mdbarcode或mdqrcode时，商户调用卡更新接口回传动态码。
        /// </summary>
        [XmlElement("mdcode_info")]
        public MdCodeInfoDTO MdcodeInfo { get; set; }

        /// <summary>
        /// 会员卡开卡时间，格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("open_date")]
        public string OpenDate { get; set; }

        /// <summary>
        /// 会员卡积分，积分必须为数字型（可为浮点型，带2位小数点）
        /// </summary>
        [XmlElement("point")]
        public string Point { get; set; }

        /// <summary>
        /// 会员卡更换不同的卡模板（该参数仅用在会员卡更新接口中）
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 会员卡有效期
        /// </summary>
        [XmlElement("valid_date")]
        public string ValidDate { get; set; }
    }
}
