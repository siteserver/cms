using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserStepcounterDataBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserStepcounterDataBatchqueryModel : AopObject
    {
        /// <summary>
        /// 步数数据查询的结束日期。此日期不能小于步数查询的开始日期
        /// </summary>
        [XmlElement("end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// 请求方唯一标识。每一个外部商户都会分配一个业务方标识，请使用钉钉联系支付宝小二骁然获取此标识
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 步数数据查询的开始日期
        /// </summary>
        [XmlElement("start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// 用户的计步时区。若此参数为空，则返回所有时区的步数信息。
        /// </summary>
        [XmlElement("time_zone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// 支付宝用户唯一用户id。为2088开头id号，需通过alipay.user.info.share接口获取此值
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
