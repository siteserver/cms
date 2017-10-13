using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserInviteAwardReceiveModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserInviteAwardReceiveModel : AopObject
    {
        /// <summary>
        /// 用户扫码抽奖输入的手机号
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 外部业务方代码
        /// </summary>
        [XmlElement("out_biz_code")]
        public string OutBizCode { get; set; }

        /// <summary>
        /// 外部业务号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }
    }
}
