using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCreditAutofinanceVidGetModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCreditAutofinanceVidGetModel : AopObject
    {
        /// <summary>
        /// 机构编号
        /// </summary>
        [XmlElement("orgcode")]
        public string Orgcode { get; set; }

        /// <summary>
        /// 支付宝账号数字ID
        /// </summary>
        [XmlElement("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// 当前安装的支付宝钱包版本号
        /// </summary>
        [XmlElement("version")]
        public string Version { get; set; }
    }
}
