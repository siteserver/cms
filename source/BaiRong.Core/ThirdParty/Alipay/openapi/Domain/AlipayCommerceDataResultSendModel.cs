using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceDataResultSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceDataResultSendModel : AopObject
    {
        /// <summary>
        /// 请求来源
        /// </summary>
        [XmlElement("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// 版本号，由支付宝分配
        /// </summary>
        [XmlElement("interface_version")]
        public string InterfaceVersion { get; set; }

        /// <summary>
        /// 操作code,由支付宝分配
        /// </summary>
        [XmlElement("op_code")]
        public string OpCode { get; set; }

        /// <summary>
        /// 结果码，由支付宝分配，该结果码将对应不同的页面展示
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 场景code，由支付宝分配
        /// </summary>
        [XmlElement("scene_code")]
        public string SceneCode { get; set; }

        /// <summary>
        /// 场景的数据表示. json 数组格式，  根据不同的scene_code,op_code,  channel,version共同确定参数是否  可以为空，接入时由支付宝确定  参数格式。
        /// </summary>
        [XmlElement("scene_data")]
        public string SceneData { get; set; }

        /// <summary>
        /// 通知的目标用户
        /// </summary>
        [XmlElement("target_id")]
        public string TargetId { get; set; }

        /// <summary>
        /// 取值范围：  IDENTITY_CARD_NO ：身份证  ALIPAY_LOGON_ID:支付宝登录账号  BINDING_MOBILE_NO:支付宝账号绑定的手机号   ALIPAY_USER_ID：支付宝user_id  标明target_id对应的类型，此参数为空时，  默认为支付宝账号的user_id。  注意：类型为身份证、支付宝绑定的手机号时，  可能对应多个支付宝账号，此时只会选择列表  第一个支付宝账号的userId作为targetId使用。
        /// </summary>
        [XmlElement("target_id_type")]
        public string TargetIdType { get; set; }
    }
}
