using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFundTransBatchQuerybatchModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFundTransBatchQuerybatchModel : AopObject
    {
        /// <summary>
        /// 批次编号，创建批次时返回的批次编号
        /// </summary>
        [XmlElement("batch_no")]
        public string BatchNo { get; set; }

        /// <summary>
        /// token，创建批次时和批次编号一起返回。注：在使用批次号查询批次信息时需要带上
        /// </summary>
        [XmlElement("token")]
        public string Token { get; set; }
    }
}
