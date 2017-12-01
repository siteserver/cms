using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarMaintainShopCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarMaintainShopCreateModel : AopObject
    {
        /// <summary>
        /// 门店详细地址，地址字符长度在4-50个字符，注：不含省市区。门店详细地址按规范格式填写地址，以免影响门店搜索及活动报名：例1：道路+门牌号，“人民东路18号”；例2：道路+门牌号+标志性建筑+楼层，“四川北路1552号欢乐广场1楼”。
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 支付宝帐号
        /// </summary>
        [XmlElement("alipay_account")]
        public string AlipayAccount { get; set; }

        /// <summary>
        /// 门店支持的车型品牌，支付宝车型库品牌编号（系统唯一），品牌编号可以通过调用【查询车型信息接口】alipay.eco.mycar.carmodel.query 获取。4S店接入必填
        /// </summary>
        [XmlArray("brand_ids")]
        [XmlArrayItem("string")]
        public List<string> BrandIds { get; set; }

        /// <summary>
        /// 城市编号（国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/AreaCodeList.zip">点此下载</a>）
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 营业结束时间（HH:mm）
        /// </summary>
        [XmlElement("close_time")]
        public string CloseTime { get; set; }

        /// <summary>
        /// 门店店长邮箱
        /// </summary>
        [XmlElement("contact_email")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 门店店长移动电话号码； 不在客户端展示
        /// </summary>
        [XmlElement("contact_mobile_phone")]
        public string ContactMobilePhone { get; set; }

        /// <summary>
        /// 门店店长姓名
        /// </summary>
        [XmlElement("contact_name")]
        public string ContactName { get; set; }

        /// <summary>
        /// 区编号（国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/AreaCodeList.zip">点此下载</a>）
        /// </summary>
        [XmlElement("district_code")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 扩展参数，json格式，可以存放营销信息，以及主营描述等扩展信息
        /// </summary>
        [XmlElement("ext_param")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 行业应用类目编号  15：洗车  16：保养  17：停车  20：4S  （空对象不变更/空集合清空/有数据覆盖）
        /// </summary>
        [XmlArray("industry_app_category_id")]
        [XmlArrayItem("number")]
        public List<long> IndustryAppCategoryId { get; set; }

        /// <summary>
        /// 行业类目编号（<a href="https://doc.open.alipay.com/doc2/detail.htm?treeId=205&articleId=104497&docType=1">点此查看</a> 非口碑类目 – 爱车）  （空对象不变更/空集合清空/有数据覆盖）
        /// </summary>
        [XmlArray("industry_category_id")]
        [XmlArrayItem("number")]
        public List<long> IndustryCategoryId { get; set; }

        /// <summary>
        /// 高德地图纬度（经纬度是门店搜索和活动推荐的重要参数，录入时请确保经纬度参数准确）
        /// </summary>
        [XmlElement("lat")]
        public string Lat { get; set; }

        /// <summary>
        /// 高德地图经度（经纬度是门店搜索和活动推荐的重要参数，录入时请确保经纬度参数准确）
        /// </summary>
        [XmlElement("lon")]
        public string Lon { get; set; }

        /// <summary>
        /// 车主平台接口上传主图片地址，通过alipay.eco.mycar.image.upload接口上传。
        /// </summary>
        [XmlElement("main_image")]
        public string MainImage { get; set; }

        /// <summary>
        /// 分支机构编号，商户在车主平台自己创建的分支机构编码
        /// </summary>
        [XmlElement("merchant_branch_id")]
        public long MerchantBranchId { get; set; }

        /// <summary>
        /// 营业开始时间（HH:mm）
        /// </summary>
        [XmlElement("open_time")]
        public string OpenTime { get; set; }

        /// <summary>
        /// 车主平台接口上传副图片地址，通过alipay.eco.mycar.image.upload接口上传。
        /// </summary>
        [XmlArray("other_images")]
        [XmlArrayItem("string")]
        public List<string> OtherImages { get; set; }

        /// <summary>
        /// 外部门店编号； 请ISV开发者自行确保其唯一性。
        /// </summary>
        [XmlElement("out_shop_id")]
        public string OutShopId { get; set; }

        /// <summary>
        /// 省编号（国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/AreaCodeList.zip">点此下载</a>）
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 分店名称，比如：万塘路店，与主门店名合并在客户端显示为：爱特堡(益乐路店)。
        /// </summary>
        [XmlElement("shop_branch_name")]
        public string ShopBranchName { get; set; }

        /// <summary>
        /// 主门店名，比如：爱特堡；主店名里不要包含分店名，如“益乐路店”。主店名长度不能超过20个字符
        /// </summary>
        [XmlElement("shop_name")]
        public string ShopName { get; set; }

        /// <summary>
        /// 门店电话号码；支持座机和手机，只支持数字和+-号，在客户端对用户展现。
        /// </summary>
        [XmlElement("shop_tel")]
        public string ShopTel { get; set; }

        /// <summary>
        /// 门店类型，（shop_type_beauty：美容店，shop_type_repair：快修店，shop_type_maintenance：维修厂，shop_type_parkinglot：停车场，shop_type_gasstation：加油站，shop_type_4s：4s店）
        /// </summary>
        [XmlElement("shop_type")]
        public string ShopType { get; set; }

        /// <summary>
        /// 门店状态（0：下线；1：上线）。  门店下线后，在门店列表不显示，不能进行下单。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
