using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// MybankCreditLoanapplyDataUploadResponse.
    /// </summary>
    public class MybankCreditLoanapplyDataUploadResponse : AopResponse
    {
        /// <summary>
        /// 数据在网商银行内部系统的唯一ID。由网商银行融资平台的系统生成，返回给外部机构。
        /// </summary>
        [XmlElement("data_id")]
        public string DataId { get; set; }
    }
}
