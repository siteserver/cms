using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AgreementParams Data Structure.
    /// </summary>
    [Serializable]
    public class AgreementParams : AopObject
    {
        /// <summary>
        /// 支付宝系统中用以唯一标识用户签约记录的编号（用户签约成功后的协议号 ）
        /// </summary>
        [XmlElement("agreement_no")]
        public string AgreementNo { get; set; }

        /// <summary>
        /// 鉴权申请token，其格式和内容，由支付宝定义。在需要做支付鉴权校验时，该参数不能为空。
        /// </summary>
        [XmlElement("apply_token")]
        public string ApplyToken { get; set; }

        /// <summary>
        /// 鉴权确认码，在需要做支付鉴权校验时，该参数不能为空
        /// </summary>
        [XmlElement("auth_confirm_no")]
        public string AuthConfirmNo { get; set; }
    }
}
