using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceMedicalInstcardBindModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceMedicalInstcardBindModel : AopObject
    {
        /// <summary>
        /// 区域编码，使用国家行政区划代码，可参看  http://www.stats.gov.cn/tjsj/tjbz/xzqhdm
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 支付宝处理完请求后，如验证成功，当前页面自动跳转到商户网站里指定页面的http路径。
        /// </summary>
        [XmlElement("return_url")]
        public string ReturnUrl { get; set; }
    }
}
