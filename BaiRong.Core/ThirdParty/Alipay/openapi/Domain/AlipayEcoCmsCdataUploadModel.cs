using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoCmsCdataUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoCmsCdataUploadModel : AopObject
    {
        /// <summary>
        /// 属性-消息投放的单个行业页面(如教育的某个幼儿园)
        /// </summary>
        [XmlElement("attribute")]
        public string Attribute { get; set; }

        /// <summary>
        /// 类目-消息投放的行业平台（教育、车主、医疗等）
        /// </summary>
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// 消息失效时间,标准时间格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("exp_time")]
        public string ExpTime { get; set; }

        /// <summary>
        /// 商户消息唯一流水,类目范围内唯一,标识此消息唯一ID,若重复上传通过此判断
        /// </summary>
        [XmlElement("merch_id")]
        public string MerchId { get; set; }

        /// <summary>
        /// 操作数据,自定义,使用方需知晓    若需要同步域内时:  如果只需要支付宝这边利用数据直接完成某个功能（通知），则使用此参数传输数据.，根据不同的scene_code,op_code,channel,version共同确定参数是否可以为空，接入时由支付宝确定参数格式。
        /// </summary>
        [XmlElement("op_data")]
        public string OpData { get; set; }

        /// <summary>
        /// 消息数据,json格式,内容由模板参数列表定义.  {"占位符":"参数值","占位符2":"参数值"…}    若需要同步域内时:  场景的数据表示. json 数组  格式，根据不同的scene_code,  op_code,channel,version共同确定  参数是否可以为空，接入时由支付宝确定  参数格式。
        /// </summary>
        [XmlElement("scene_data")]
        public string SceneData { get; set; }

        /// <summary>
        /// 消息生效时间,标准时间格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 是否需要同步域内,设定模板时需支持;  若需要特殊可选均是必填
        /// </summary>
        [XmlElement("syn")]
        public bool Syn { get; set; }

        /// <summary>
        /// 消息模板的版本,由开放生态分配
        /// </summary>
        [XmlElement("t_v")]
        public string TV { get; set; }

        /// <summary>
        /// 消息模板ID,需要预先设定模板才能进行消息投放,由开放生态协商分配
        /// </summary>
        [XmlElement("tamplate_id")]
        public long TamplateId { get; set; }

        /// <summary>
        /// 投放目标对象,自定义.    若需要同步到域内:  场景覆盖的目标人群标识，单个用户是支付宝的userId,多个用户userId 使用英文半角逗号隔开,最多200个如果是群组,使用支付宝分配的群组ID.
        /// </summary>
        [XmlElement("target_id")]
        public string TargetId { get; set; }
    }
}
