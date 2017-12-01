using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceDataSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceDataSendModel : AopObject
    {
        /// <summary>
        /// 场景的来源渠道，比如场景  在阿里旅行触发，就用alitrip  接入时和支付宝共同确认
        /// </summary>
        [XmlElement("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// 操作码,由支付宝分配
        /// </summary>
        [XmlElement("op_code")]
        public string OpCode { get; set; }

        /// <summary>
        /// 操作数据，如果只需要支付宝这边利用  数据直接完成某个功能（通知），则使  用此参数传输数据.，根据不同的scene_code,  op_code,channel,version共同确定参数是否  可以为空，接入时由支付宝确定参数格式。
        /// </summary>
        [XmlElement("op_data")]
        public string OpData { get; set; }

        /// <summary>
        /// 场景标识,由支付宝分配
        /// </summary>
        [XmlElement("scene_code")]
        public string SceneCode { get; set; }

        /// <summary>
        /// 场景的数据表示. json 数组  格式，根据不同的scene_code,  op_code,channel,version共同确定  参数是否可以为空，接入时由支付宝确定  参数格式。
        /// </summary>
        [XmlElement("scene_data")]
        public string SceneData { get; set; }

        /// <summary>
        /// 场景覆盖的目标人群标识，  单个用户是支付宝的userId,  多个用户userId 使用英文半  角逗号隔开,最多200个  如果是群组,使用支付宝分配  的群组ID.
        /// </summary>
        [XmlElement("target_id")]
        public string TargetId { get; set; }

        /// <summary>
        /// 取值范围：  IDENTITY_CARD_NO ：身份证  ALIPAY_LOGON_ID:支付宝登录账号  BINDING_MOBILE_NO:支付宝账号绑定的手机号   ALIPAY_USER_ID：支付宝user_id  标明target_id对应的类型，此参数为空时，  默认为支付宝账号的user_id。  注意：类型为身份证、支付宝绑定的手机号时，  可能对应多个支付宝账号，此时只会选择列表  第一个支付宝账号的userId作为targetId使用。
        /// </summary>
        [XmlElement("target_id_type")]
        public string TargetIdType { get; set; }

        /// <summary>
        /// 场景数据的类型的版本,由支付宝分配
        /// </summary>
        [XmlElement("version")]
        public string Version { get; set; }
    }
}
