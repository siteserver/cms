using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityProdFingerprintVerifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityProdFingerprintVerifyModel : AopObject
    {
        /// <summary>
        /// 业务扩展参数，目前添加指位变更逻辑判断字段，needAuthData标示指位变更敏感，subAction标示当前操作是校验还是更新指位
        /// </summary>
        [XmlElement("extend_param")]
        public string ExtendParam { get; set; }

        /// <summary>
        /// IFAA协议的版本，目前为2.0
        /// </summary>
        [XmlElement("ifaa_version")]
        public string IfaaVersion { get; set; }

        /// <summary>
        /// ifaf_message:校验阶段客户端返回的协议体数据，对应《IFAA本地免密技术规范》中的IFAFMessage，内容中包含客户端的校验数据。
        /// </summary>
        [XmlElement("ifaf_message")]
        public string IfafMessage { get; set; }

        /// <summary>
        /// 外部业务号，商户的业务单据号，用于核对与问题排查,原则上来说需要保持这个参数的唯一性。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// IFAA协议客户端静态信息，调用IFAA客户端SDK接口获取secData，透传至本参数。此参数是为了兼容IFAA1.0而设计的，接入方可根据是否需要接入IFAA1.0来决定是否要传(只接入IFAA2.0不需要传)
        /// </summary>
        [XmlElement("sec_data")]
        public string SecData { get; set; }
    }
}
