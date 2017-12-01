using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ShopSummaryQueryResponse Data Structure.
    /// </summary>
    [Serializable]
    public class ShopSummaryQueryResponse : AopObject
    {
        /// <summary>
        /// 门店地址
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 分店名
        /// </summary>
        [XmlElement("branch_shop_name")]
        public string BranchShopName { get; set; }

        /// <summary>
        /// 品牌名，不填写则默认为其它品牌
        /// </summary>
        [XmlElement("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 经营时间
        /// </summary>
        [XmlElement("business_time")]
        public string BusinessTime { get; set; }

        /// <summary>
        /// 门店类目列表
        /// </summary>
        [XmlArray("category_infos")]
        [XmlArrayItem("shop_category_info")]
        public List<ShopCategoryInfo> CategoryInfos { get; set; }

        /// <summary>
        /// 城市编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/AreaCodeList.zip">点此下载</a>
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 区县编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/AreaCodeList.zip">点此下载</a>
        /// </summary>
        [XmlElement("district_code")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlElement("gmt_create")]
        public string GmtCreate { get; set; }

        /// <summary>
        /// 门店是否在客户端显示，T表示显示，F表示隐藏
        /// </summary>
        [XmlElement("is_show")]
        public string IsShow { get; set; }

        /// <summary>
        /// 纬度，只有在query_type=KB_PROMOTER非空
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度，只有在query_type=KB_PROMOTER非空
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 门店首图
        /// </summary>
        [XmlElement("main_image")]
        public string MainImage { get; set; }

        /// <summary>
        /// 主门店名
        /// </summary>
        [XmlElement("main_shop_name")]
        public string MainShopName { get; set; }

        /// <summary>
        /// 图片集，是map转化成的json串，key是图片id,value是图片url
        /// </summary>
        [XmlElement("pic_coll")]
        public string PicColl { get; set; }

        /// <summary>
        /// 省份编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/AreaCodeList.zip">点此下载</a>
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 门店评论信息
        /// </summary>
        [XmlElement("shop_comment_info")]
        public ShopCommentInfo ShopCommentInfo { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// COMMON（普通门店）、MALL（商圈）
        /// </summary>
        [XmlElement("shop_type")]
        public string ShopType { get; set; }

        /// <summary>
        /// 门店状态，OPEN：营业中、PAUSE：暂停营业、FREEZE：已冻结、CLOSE:门店已关闭
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
