using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AddressInfo Data Structure.
    /// </summary>
    [Serializable]
    public class AddressInfo : AopObject
    {
        /// <summary>
        /// 商户详细经营地址
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 商户所在城市编码，城市编码是与国家统计局一致，请查询:  http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/  国标省市区号下载：http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls?spm=a219a.7629140.0.0.qRW4KQ&file=2016.xls
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 商户所在区县编码，区县编码是与国家统计局一致，请查询: http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/ 国标省市区号下载：http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls?spm=a219a.7629140.0.0.qRW4KQ&file=2016.xls
        /// </summary>
        [XmlElement("district_code")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 纬度，浮点型,小数点后最多保留6位  如需要录入经纬度，请以高德坐标系为准，录入时请确保经纬度参数准确。高德经纬度查询：http://lbs.amap.com/console/show/picker
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度，浮点型, 小数点后最多保留6位。  如需要录入经纬度，请以高德坐标系为准，录入时请确保经纬度参数准确。高德经纬度查询：http://lbs.amap.com/console/show/picker
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 商户所在省份编码，  省份编码是与国家统计局一致，请查询:  http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/  国标省市区号下载：http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls?spm=a219a.7629140.0.0.qRW4KQ&file=2016.xls
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 地址类型。取值范围：BUSINESS_ADDRESS：经营地址（默认）
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
