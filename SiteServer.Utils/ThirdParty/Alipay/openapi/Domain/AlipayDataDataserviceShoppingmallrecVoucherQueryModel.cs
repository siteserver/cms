using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataDataserviceShoppingmallrecVoucherQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataDataserviceShoppingmallrecVoucherQueryModel : AopObject
    {
        /// <summary>
        /// 纬度；注：高德坐标系。经纬度是门店搜索和活动推荐的重要参数，录入时请确保经纬度参数准确。高德经纬度查询：http://lbs.amap.com/console/show/picker
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度；注：高德坐标系。经纬度是门店搜索和活动推荐的重要参数，录入时请确保经纬度参数准确。高德经纬度查询：http://lbs.amap.com/console/show/picker
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 系统内商场的唯一标识id
        /// </summary>
        [XmlElement("mall_id")]
        public string MallId { get; set; }

        /// <summary>
        /// 本次请求的全局唯一标识, 支持英文字母和数字, 由开发者自行定义
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 系统内支付宝用户唯一标识id. 支付宝用户号是以2088开头的纯数字组成
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
