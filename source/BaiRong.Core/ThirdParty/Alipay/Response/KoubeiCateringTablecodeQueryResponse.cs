using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiCateringTablecodeQueryResponse.
    /// </summary>
    public class KoubeiCateringTablecodeQueryResponse : AopResponse
    {
        /// <summary>
        /// 返回值为shop_code表示只返回了门店码 返回值为table_code表示返回了桌码跟门店码
        /// </summary>
        [XmlElement("code_flag")]
        public string CodeFlag { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 桌码
        /// </summary>
        [XmlElement("table_num")]
        public string TableNum { get; set; }
    }
}
