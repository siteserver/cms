using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarParkingConfigSetModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarParkingConfigSetModel : AopObject
    {
        /// <summary>
        /// 签约支付宝账号
        /// </summary>
        [XmlElement("account_no")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 接口信息列表，停车业务需要配置的接口列表，该值为JSON数据格式的LIST对象，现阶段只需要配置一个页面接口即可 。每次请将所有的接口配置信息都传入，未传的接口信息将会被置空。
        /// </summary>
        [XmlArray("interface_info_list")]
        [XmlArrayItem("interface_info_list")]
        public List<InterfaceInfoList> InterfaceInfoList { get; set; }

        /// <summary>
        /// 商户在停车平台首页露出的LOGO；注意：该图片为PNG格式内容为BASE64的字符串，若为空则停车平台首页将不露出商户LOGO。建议图片尺寸27px*27px，大小不要超过60K
        /// </summary>
        [XmlElement("merchant_logo")]
        public string MerchantLogo { get; set; }

        /// <summary>
        /// 商户简称，由开发者提供
        /// </summary>
        [XmlElement("merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户客服电话
        /// </summary>
        [XmlElement("merchant_service_phone")]
        public string MerchantServicePhone { get; set; }
    }
}
