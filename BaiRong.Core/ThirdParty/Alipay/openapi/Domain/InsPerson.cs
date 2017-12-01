using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsPerson Data Structure.
    /// </summary>
    [Serializable]
    public class InsPerson : AopObject
    {
        /// <summary>
        /// 地址
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [XmlElement("birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// 投保参数;标准json格式
        /// </summary>
        [XmlElement("biz_data")]
        public string BizData { get; set; }

        /// <summary>
        /// 证件上名称;如果渠道账号字段没填则必填
        /// </summary>
        [XmlElement("cert_name")]
        public string CertName { get; set; }

        /// <summary>
        /// 证件号码;如果渠道账号字段没填则必填
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型;如果渠道账号字段没填则必填   100:居民身份证;102:护照;103:军官证;104:士兵证;105:港澳居民往来内地通行证;106:台湾同胞往来大陆通行证;109:警官证
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 渠道账号对应的uid;如果证件类型字段没填则必填
        /// </summary>
        [XmlElement("channel_user_id")]
        public string ChannelUserId { get; set; }

        /// <summary>
        /// 渠道账号来源;1:支付宝账号 2:淘宝账号;如果证件类型字段没填则必填
        /// </summary>
        [XmlElement("channel_user_source")]
        public string ChannelUserSource { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// 性别;M:男 F:女
        /// </summary>
        [XmlElement("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        [XmlElement("nationality")]
        public string Nationality { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [XmlElement("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 张三
        /// </summary>
        [XmlElement("pronounce_name")]
        public string PronounceName { get; set; }

        /// <summary>
        /// 支付宝会员ID;如果是投保人则必填
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
