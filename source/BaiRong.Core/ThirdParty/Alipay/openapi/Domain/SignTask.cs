using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SignTask Data Structure.
    /// </summary>
    [Serializable]
    public class SignTask : AopObject
    {
        /// <summary>
        /// 业务扩展参数 {"key1":"value2"}
        /// </summary>
        [XmlElement("biz_data")]
        public string BizData { get; set; }

        /// <summary>
        /// 业务流水号，保证唯一性
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 定义签约流程结束后回调方式，必传，枚举值，由签约平台定义。  1. call_back_url。支付宝签约流程结束后，通过http协议回调业务系统页面或外部应用。  2. close。 支付宝签约流程结束后，关闭流程，不做跳转。
        /// </summary>
        [XmlElement("cb_type")]
        public string CbType { get; set; }

        /// <summary>
        /// 回调外部业务系统的地址
        /// </summary>
        [XmlElement("cb_url")]
        public string CbUrl { get; set; }

        /// <summary>
        /// 1 - 云签  2 - 移动签
        /// </summary>
        [XmlElement("cert_sign_type")]
        public long CertSignType { get; set; }

        /// <summary>
        /// 签约跳转方式，格式说明：业务入口形态_2_支付宝签约方式。  'NONEED'  // 无需支付宝认证  'h5_2_alipay'  // 支付宝内部H5页面，通过Url跳转支付宝钱包H5签约流程  'app_2_alipay' // 支付宝外部APP，通过schema协议跳转支付宝钱包H5签约流程  'qrcode_2_alipay' // 请求一个签约二维码入口，通过支付宝钱包二维码扫一扫，跳转H5签约流程  'pc_2_pc'  // PC签约流程，通过Url跳转支付宝PC签约流程
        /// </summary>
        [XmlElement("enter_type")]
        public string EnterType { get; set; }

        /// <summary>
        /// 签约操作者列表
        /// </summary>
        [XmlArray("principal_list")]
        [XmlArrayItem("principal")]
        public List<Principal> PrincipalList { get; set; }

        /// <summary>
        /// 签约主体类型。  individual   //个人认证  entreprise  //企业认证
        /// </summary>
        [XmlElement("signer_type")]
        public string SignerType { get; set; }

        /// <summary>
        /// 签约任务有效时长，单位秒。可有外部系统控制时长，如果不传系统会赋予默认值。
        /// </summary>
        [XmlElement("task_expire")]
        public string TaskExpire { get; set; }
    }
}
