using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// ZhimaMerchantDataUploadInitializeResponse.
    /// </summary>
    public class ZhimaMerchantDataUploadInitializeResponse : AopResponse
    {
        /// <summary>
        /// 行业模板
        /// </summary>
        [XmlElement("template_url")]
        public string TemplateUrl { get; set; }
    }
}
