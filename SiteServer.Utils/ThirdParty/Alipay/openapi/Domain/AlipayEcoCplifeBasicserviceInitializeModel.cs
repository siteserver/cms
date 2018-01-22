using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoCplifeBasicserviceInitializeModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoCplifeBasicserviceInitializeModel : AopObject
    {
        /// <summary>
        /// 若服务类型为物业缴费账单模式，每个小区默认的收款帐号为授权物业的支付宝账号，默认不用传该参数。  但为满足部分物业公司财务要求，允许开发者为每个小区服务传入一个指定的物业收款帐号。根据不同账号类型（account_type），开发者需要向物业或支付宝商务支持接口人获取具体的收款帐号。
        /// </summary>
        [XmlElement("account")]
        public string Account { get; set; }

        /// <summary>
        /// 若服务类型为物业缴费账单模式，每个小区默认的收款账号为授权物业的支付宝账号，默认不用传该参数。用户完成缴费后实时入账至该支付宝账号，后续由物业财务系统根据缴费异步通知和支付宝对账文件进行资金清分。    但为了满足部分物业公司的财务清结算需求，允许在授权物业账号下已设置支付宝收款子账号限制集的前提下，由开发者为指定小区服务传入一个物业公司的支付宝收款子帐号，支持通过以下任一种形式传递该账号：    ALIPAY_LOGON_ID - 支付宝登陆账号。    ALIPAY_PARTNER_ID - 支付宝登陆账号对应的用户ID，2088开头的16位纯数字用户号。    注意：若传递的收款子账号事先未在支付宝配置，开发者在上线前的支付验证环节会提示不支持收款到该账户，请联系物业公司完成配置事宜。
        /// </summary>
        [XmlElement("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        /// 支付宝社区小区统一编号，必须在物业账号名下存在。
        /// </summary>
        [XmlElement("community_id")]
        public string CommunityId { get; set; }

        /// <summary>
        /// 由开发者系统提供的，支付宝根据基础服务类型在特定业务环节调用的外部系统服务地址，开发者需要确保外部地址的准确性。在线上环境需要使用HTTPS协议，会检查ssl证书，要求证书为正规的证书机构签发，不支持自签名证书。  对于PROPERTY_PAY_BILL_MODE服务类型，该地址表示用户缴费支付完成后开发者系统接受支付结果通知的回调地址。
        /// </summary>
        [XmlElement("external_invoke_address")]
        public string ExternalInvokeAddress { get; set; }

        /// <summary>
        /// 若本服务有预期的过期时间（如在物业服务合同中约定），开发者按标准时间格式：yyyy-MM-dd HH:mm:ss传入。
        /// </summary>
        [XmlElement("service_expires")]
        public string ServiceExpires { get; set; }

        /// <summary>
        /// 基础服务类型，目前支持的类型值为：  PROPERTY_PAY_BILL_MODE - 物业缴费账单上传模式
        /// </summary>
        [XmlElement("service_type")]
        public string ServiceType { get; set; }
    }
}
