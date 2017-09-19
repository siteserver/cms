using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoCplifeCommunityCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoCplifeCommunityCreateModel : AopObject
    {
        /// <summary>
        /// 若开发者录入的物业小区需要精确对应地图上多个小区（比如物业系统中的小区包含类似一期二期、或东区西区的组团结构），以便后续线上推广时覆盖到对应小区的住户，可以指定关联的高德地图中住宅、住宿或地名地址等小区相关类型的POI（地图兴趣点）ID列表。  若传入该参数且参数值合法，则该参数的优先级高于传入的地理位置经纬度。    注：最多包含10组poiid。    高德POI ID的获取接口：  http://lbs.amap.com/api/webservice/guide/api/search/
        /// </summary>
        [XmlArray("associated_pois")]
        [XmlArrayItem("string")]
        public List<string> AssociatedPois { get; set; }

        /// <summary>
        /// 地级市编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 小区主要详细地址，不需要包含省市区名称。
        /// </summary>
        [XmlElement("community_address")]
        public string CommunityAddress { get; set; }

        /// <summary>
        /// 小区所在的经纬度列表（注：需要是高德坐标系），每对经纬度用"|"分隔，经度在前，纬度在后，经纬度小数点后不超过6位。    注：若新建的小区覆盖多个片区，最多包含5组经纬度，其中第一组作为主经纬度。    经纬度是小区搜索和用户推广的重要参数，录入时请确保经纬度参数准确。    高德经纬度查询接口：http://lbs.amap.com/api/webservice/guide/api/search/    高德坐标系转换接口：http://lbs.amap.com/api/webservice/guide/api/convert/
        /// </summary>
        [XmlArray("community_locations")]
        [XmlArrayItem("string")]
        public List<string> CommunityLocations { get; set; }

        /// <summary>
        /// 小区名称，最长不超过32个字符。
        /// </summary>
        [XmlElement("community_name")]
        public string CommunityName { get; set; }

        /// <summary>
        /// 区县编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("district_code")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 需要提供物业服务热线或联系电话，便于用户在需要时联系物业。
        /// </summary>
        [XmlElement("hotline")]
        public string Hotline { get; set; }

        /// <summary>
        /// 小区在物业系统中的唯一编号。
        /// </summary>
        [XmlElement("out_community_id")]
        public string OutCommunityId { get; set; }

        /// <summary>
        /// 省份编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }
    }
}
