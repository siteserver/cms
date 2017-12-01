using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataDataserviceUserlevelZrankGetModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataDataserviceUserlevelZrankGetModel : AopObject
    {
        /// <summary>
        /// type对应的账号：如手机号-13815869530
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 暂时支持：EMAIL(邮箱),PHONE(手机号),BANKCARD（银行卡）,CERTNO（身份证）,IMEI（设备唯一码）,MAC（mac地址）,TBID（淘宝id）
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
