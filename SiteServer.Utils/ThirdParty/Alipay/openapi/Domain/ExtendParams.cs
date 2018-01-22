using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ExtendParams Data Structure.
    /// </summary>
    [Serializable]
    public class ExtendParams : AopObject
    {
        /// <summary>
        /// 使用花呗分期要进行的分期数
        /// </summary>
        [XmlElement("hb_fq_num")]
        public string HbFqNum { get; set; }

        /// <summary>
        /// 使用花呗分期需要卖家承担的手续费比例的百分值，传入100代表100%
        /// </summary>
        [XmlElement("hb_fq_seller_percent")]
        public string HbFqSellerPercent { get; set; }

        /// <summary>
        /// 系统商编号  该参数作为系统商返佣数据提取的依据，请填写系统商签约协议的PID
        /// </summary>
        [XmlElement("sys_service_provider_id")]
        public string SysServiceProviderId { get; set; }
    }
}
