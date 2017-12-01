using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SsdataDataserviceRiskAntifraudlistQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class SsdataDataserviceRiskAntifraudlistQueryModel : AopObject
    {
        /// <summary>
        /// 地址信息。省+市+区/县+详细地址，其中 省+市+区/县可以为空，长度不超过256，不要包含特殊字符，如","，"\"，"|"，"&"，"^"
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 银行卡号。中国大陆银行发布的银行卡:借记卡长度19位；信用卡长度16位；各位的取值位[0,9]的整数；不含虚拟卡
        /// </summary>
        [XmlElement("bank_card")]
        public string BankCard { get; set; }

        /// <summary>
        /// 证件号。证件类型、证件号、姓名三要素均为必填参数
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型。IDENTITY_CARD标识为身份证，目前仅支持身份证类型
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 电子邮箱。合法email，字母小写，特殊符号以半角形式出现
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// 国际移动设备标志。15位长度数字
        /// </summary>
        [XmlElement("imei")]
        public string Imei { get; set; }

        /// <summary>
        /// ip地址。以"."分割的四段Ip，如 x.x.x.x，x为[0,255]之间的整数
        /// </summary>
        [XmlElement("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// 物理地址。支持格式如下：xx:xx:xx:xx:xx:xx，xx-xx-xx-xx-xx-xx，xxxxxxxxxxxx，x取值范围[0,9]之间的整数及A，B，C，D，E，F
        /// </summary>
        [XmlElement("mac")]
        public string Mac { get; set; }

        /// <summary>
        /// 手机号码。中国大陆合法手机号，长度11位，不含国家代码
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 姓名。长度不超过64，姓名中不要包含特殊字符，如","，"\"，"|"，"&"，"^"
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 调用服务的商户id ，如果是第三方服务商，需要将其实际的商户id透传过来，如果是普通商户传入自己的appid即可
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// wifi的物理地址。支持格式如下：xx:xx:xx:xx:xx:xx，xx-xx-xx-xx-xx-xx，xxxxxxxxxxxx，x取值范围[0,9]之间的整数及A，B，C，D，E，F
        /// </summary>
        [XmlElement("wifimac")]
        public string Wifimac { get; set; }
    }
}
