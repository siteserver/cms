using System;
using System.Xml.Serialization;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoMycarParkingConfigQueryResponse.
    /// </summary>
    public class AlipayEcoMycarParkingConfigQueryResponse : AopResponse
    {
        /// <summary>
        /// 签约支付宝账号，开发者通过ISV系统配置信息注册接口(alipay.eco.mycar.parking.config.set)传入的参数值
        /// </summary>
        [XmlElement("account_no")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 业务接口列表数据  Json格式数据
        /// </summary>
        [XmlElement("interface_info_list")]
        public InterfaceInfoList InterfaceInfoList { get; set; }

        /// <summary>
        /// 商户在停车平台首页露出的LOGO链接地址，开发者通过ISV系统配置信息注册接口(alipay.eco.mycar.parking.config.set)调用自动生成该链接
        /// </summary>
        [XmlElement("merchant_logo")]
        public string MerchantLogo { get; set; }

        /// <summary>
        /// 商户简称，开发者通过ISV系统配置信息注册接口(alipay.eco.mycar.parking.config.set)传入的参数值
        /// </summary>
        [XmlElement("merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户客服电话，开发者通过ISV系统配置信息注册接口(alipay.eco.mycar.parking.config.set)传入的参数值
        /// </summary>
        [XmlElement("merchant_service_phone")]
        public string MerchantServicePhone { get; set; }
    }
}
