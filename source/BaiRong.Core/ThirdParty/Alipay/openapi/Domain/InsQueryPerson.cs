using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsQueryPerson Data Structure.
    /// </summary>
    [Serializable]
    public class InsQueryPerson : AopObject
    {
        /// <summary>
        /// 证件号码;如果渠道账号字段（channel_user_id）没填则本字段为必填
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型;如果渠道账号字段没填则必填 （该字段和cert_no为配对字段）  100:居民身份证;102:护照;103:军官证;104:士兵证;105:港澳居民往来内地通行证;106:台湾同胞往来大陆通行证;109:警官证
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 渠道账号对应的uid;如果证件类型字段没填则本字段为必填
        /// </summary>
        [XmlElement("channel_user_id")]
        public string ChannelUserId { get; set; }

        /// <summary>
        /// 渠道账号来源:  1:支付宝账号;  2:淘宝账号;  如果证件类型字段没填则必填。  和channel_user_id 配对
        /// </summary>
        [XmlElement("channel_user_source")]
        public string ChannelUserSource { get; set; }

        /// <summary>
        /// 保单用户搜索的类型:  1：按照投保人搜索  2：按照受益人搜索   3：按照被保人搜索
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
