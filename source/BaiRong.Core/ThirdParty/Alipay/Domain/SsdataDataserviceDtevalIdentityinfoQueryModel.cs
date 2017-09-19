using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SsdataDataserviceDtevalIdentityinfoQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class SsdataDataserviceDtevalIdentityinfoQueryModel : AopObject
    {
        /// <summary>
        /// 业务流水号(biz_no)，代表了一笔业务，该参数由上数系统创建，为了唯一确定一笔业务的具体核身查询动作
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 业务类型(biz_type)，代表了查询核身信息的业务具体类型，其中prophet来自于枚举值，目前枚举值只有一个业务枚举，即"先知"业务 ，该参数由上数系统传递
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 手机号(phone)，代表传入的用户手机号码，该参数由上数系统传递
        /// </summary>
        [XmlElement("phone")]
        public string Phone { get; set; }
    }
}
