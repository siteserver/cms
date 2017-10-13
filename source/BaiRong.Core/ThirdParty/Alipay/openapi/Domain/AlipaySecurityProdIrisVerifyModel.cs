using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityProdIrisVerifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityProdIrisVerifyModel : AopObject
    {
        /// <summary>
        /// 虹膜校验的关联token，用于二次校验
        /// </summary>
        [XmlElement("biz_token")]
        public string BizToken { get; set; }

        /// <summary>
        /// 虹膜扩展参数，用于后续扩展，格式为json格式，目前传入参数为iris_vendor，虹膜厂商
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 虹膜库分组。每个商户可以使用多个虹膜库分组，虹膜搜索时会在指定的虹膜库分组中搜索
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// 虹膜特征字节数组进行Base64编码后的字符串
        /// </summary>
        [XmlElement("iris_str")]
        public string IrisStr { get; set; }

        /// <summary>
        /// 虹膜校验操作类型，方便后续扩展，目前传入固定irisVerify
        /// </summary>
        [XmlElement("operate_type")]
        public string OperateType { get; set; }

        /// <summary>
        /// 外部应用标识，用于标识使用虹膜的应用来源
        /// </summary>
        [XmlElement("out_app_flag")]
        public string OutAppFlag { get; set; }

        /// <summary>
        /// 外部业务号，商户的业务单据号，用于核对与问题排查
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 用于标识虹膜对应的注册人的id，如支付宝的uid、信用id等等，此处只需要业务方传入唯一可以标识的身份的id即可，虹膜系统不使用此id反查任何内容。用途是在校验的时候返回业务可以识别的唯一id。校验的时候传入此id，则虹膜系统会认为是1:1比对请求。如果是1：N请求，该值需要传空。
        /// </summary>
        [XmlElement("person_id")]
        public string PersonId { get; set; }
    }
}
