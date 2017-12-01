using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// ZhimaMerchantSingleDataUploadResponse.
    /// </summary>
    public class ZhimaMerchantSingleDataUploadResponse : AopResponse
    {
        /// <summary>
        /// 公用回传参数（非必填），这个字段由商户传入，系统透传给商户。
        /// </summary>
        [XmlElement("biz_ext_params")]
        public string BizExtParams { get; set; }

        /// <summary>
        /// 每次上传都会生成一个任务号 。
        /// </summary>
        [XmlElement("task_id")]
        public string TaskId { get; set; }
    }
}
