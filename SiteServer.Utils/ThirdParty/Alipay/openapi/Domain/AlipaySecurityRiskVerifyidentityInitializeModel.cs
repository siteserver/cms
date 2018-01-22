using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityRiskVerifyidentityInitializeModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityRiskVerifyidentityInitializeModel : AopObject
    {
        /// <summary>
        /// 用户证件号，当前支持身份证号
        /// </summary>
        [XmlElement("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [XmlElement("account_name")]
        public string AccountName { get; set; }

        /// <summary>
        /// 用户证件类型，需传入英文枚举常量，当前支持：CERT(大陆身份证)
        /// </summary>
        [XmlElement("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        /// 核验后回调业务url
        /// </summary>
        [XmlElement("biz_callback_url")]
        public string BizCallbackUrl { get; set; }

        /// <summary>
        /// 接入业务方业务唯一性id
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 附加业务信息，Json结构
        /// </summary>
        [XmlElement("biz_params")]
        public string BizParams { get; set; }

        /// <summary>
        /// 核验服务名称，同时请求多种服务用“|”连接，SMS：短信，FACE：人脸
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        /// <summary>
        /// 身份核验场景CODE，商务谈判基础上，由支付宝来分配。
        /// </summary>
        [XmlElement("scene_code")]
        public string SceneCode { get; set; }
    }
}
