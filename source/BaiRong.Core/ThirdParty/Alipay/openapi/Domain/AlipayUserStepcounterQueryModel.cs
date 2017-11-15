using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserStepcounterQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserStepcounterQueryModel : AopObject
    {
        /// <summary>
        /// 商户要查询步数的日期。如果不传入此参数，则返回用户当日步数。
        /// </summary>
        [XmlElement("count_date")]
        public string CountDate { get; set; }

        /// <summary>
        /// 请求方唯一标识。每一个外部商户都会分配一个业务方标识，请使用钉钉联系支付宝小二骁然获取此标识。
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 商户要查询步数的时区，此参数只在查询当日用户步数时有效。若此参数为空，则以用户当时所在时区返回步数。
        /// </summary>
        [XmlElement("time_zone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// 支付宝用户唯一用户id。为2088开头id号，需通过alipay.user.userinfo.share接口获取此值
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
