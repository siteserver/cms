using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketShopQuerydetailResponse.
    /// </summary>
    public class AlipayOfflineMarketShopQuerydetailResponse : AopResponse
    {
        /// <summary>
        /// 门店详细地址，注：不含省市区
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 门店审核状态描述，如果审核驳回则有相关的驳回理由
        /// </summary>
        [XmlElement("audit_desc")]
        public string AuditDesc { get; set; }

        /// <summary>
        /// 门店审核时需要的图片; 至少包含一张门头照片，两张内景照片，必须反映真实的门店情况，审核才能够通过， 多个以英文逗号分隔
        /// </summary>
        [XmlElement("audit_images")]
        public string AuditImages { get; set; }

        /// <summary>
        /// 门店审核状态，对于系统商而言，只有三个状态，AUDITING：审核中，AUDIT_FAILED：审核驳回，AUDIT_SUCCESS：审核通过。第一次审核通过会触发门店上架。
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 授权函图片
        /// </summary>
        [XmlElement("auth_letter")]
        public string AuthLetter { get; set; }

        /// <summary>
        /// 人均消费价格，最少1元，最大不超过99999元，请按实际情况填写；单位元；
        /// </summary>
        [XmlElement("avg_price")]
        public string AvgPrice { get; set; }

        /// <summary>
        /// 门店是否有包厢，T表示有，F表示没有，不传在客户端不作展示
        /// </summary>
        [XmlElement("box")]
        public string Box { get; set; }

        /// <summary>
        /// 分店名称，比如：万塘路店，与主门店名合并在客户端显示为：肯德基(万塘路店)
        /// </summary>
        [XmlElement("branch_shop_name")]
        public string BranchShopName { get; set; }

        /// <summary>
        /// 品牌LOGO; 图片ID，不填写则默认为门店首图main_image
        /// </summary>
        [XmlElement("brand_logo")]
        public string BrandLogo { get; set; }

        /// <summary>
        /// 品牌名称；不填写则默认为“其它品牌”
        /// </summary>
        [XmlElement("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 经营许可证，只有餐饮类目需要
        /// </summary>
        [XmlElement("business_certificate")]
        public string BusinessCertificate { get; set; }

        /// <summary>
        /// 经营许可证有效期，格式：2020-03-20，只有餐饮类目需要
        /// </summary>
        [XmlElement("business_certificate_expires")]
        public string BusinessCertificateExpires { get; set; }

        /// <summary>
        /// 营业时间;支持分段营业时间，以英文逗号分隔
        /// </summary>
        [XmlElement("business_time")]
        public string BusinessTime { get; set; }

        /// <summary>
        /// 类目ID，类目初始数据由口碑提供
        /// </summary>
        [XmlElement("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 城市编码，国标码，详见国家统计局数据
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 门店电话号码；支持座机和手机，在客户端对用户展现，支持多个电话，以英文逗号分隔
        /// </summary>
        [XmlElement("contact_number")]
        public string ContactNumber { get; set; }

        /// <summary>
        /// 门店创建来源；如：开放平台、支付宝客户端、口碑商家app、商家自主开店、服务商开店、全民开店-支付宝客户端、全民开店-商户app、其它
        /// </summary>
        [XmlElement("create_channel")]
        public string CreateChannel { get; set; }

        /// <summary>
        /// 区县编码，国标码，详见国家统计局数据
        /// </summary>
        [XmlElement("district_code")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 门店创建时间
        /// </summary>
        [XmlElement("gmt_shop_create")]
        public string GmtShopCreate { get; set; }

        /// <summary>
        /// 门店修改时间
        /// </summary>
        [XmlElement("gmt_shop_modified")]
        public string GmtShopModified { get; set; }

        /// <summary>
        /// 店铺使用的机具编号，多个以英文逗号分隔
        /// </summary>
        [XmlElement("implement_id")]
        public string ImplementId { get; set; }

        /// <summary>
        /// 门店是否上架，T表示上架,F表示未上架，第一次门店审核通过后会触发上架
        /// </summary>
        [XmlElement("is_online")]
        public string IsOnline { get; set; }

        /// <summary>
        /// 是否在其他平台开店，T表示有开店，F表示未开店，用于证明其开店资质
        /// </summary>
        [XmlElement("is_operating_online")]
        public string IsOperatingOnline { get; set; }

        /// <summary>
        /// 门店是否在客户端显示，T表示显示，F表示隐藏
        /// </summary>
        [XmlElement("is_show")]
        public string IsShow { get; set; }

        /// <summary>
        /// 开发者返佣ID，重要：如果有口碑签订了返佣协议，则该字段作为返佣数据提取的依据，需要与签约协议的PID保持一致， 该字段只能在创建接口中传入，不能在修改接口中被修改
        /// </summary>
        [XmlElement("isv_uid")]
        public string IsvUid { get; set; }

        /// <summary>
        /// 纬度，最长15位字符（包括小数点）， 注：高德坐标系
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 门店营业执照图片
        /// </summary>
        [XmlElement("licence")]
        public string Licence { get; set; }

        /// <summary>
        /// 门店营业执照编号
        /// </summary>
        [XmlElement("licence_code")]
        public string LicenceCode { get; set; }

        /// <summary>
        /// 营业执照过期时间
        /// </summary>
        [XmlElement("licence_expires")]
        public string LicenceExpires { get; set; }

        /// <summary>
        /// 门店营业执照名称
        /// </summary>
        [XmlElement("licence_name")]
        public string LicenceName { get; set; }

        /// <summary>
        /// 经度，最长15位字符（包括小数点）， 注：高德坐标系
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 门店首图；非常重要，推荐尺寸2000*1500
        /// </summary>
        [XmlElement("main_image")]
        public string MainImage { get; set; }

        /// <summary>
        /// 主店名；比如：肯德基
        /// </summary>
        [XmlElement("main_shop_name")]
        public string MainShopName { get; set; }

        /// <summary>
        /// 是否有无烟区，T表示有无烟区，F表示没有无烟区，不传在客户端不展示
        /// </summary>
        [XmlElement("no_smoking")]
        public string NoSmoking { get; set; }

        /// <summary>
        /// 门店店长电话号码；用于接收门店状态变更通知，收款成功通知等通知消息，不在客户端展示；多个以引文逗号分隔
        /// </summary>
        [XmlElement("notify_mobile")]
        public string NotifyMobile { get; set; }

        /// <summary>
        /// 在其他平台的开店图片，支持多张，逗号分隔
        /// </summary>
        [XmlElement("online_image")]
        public string OnlineImage { get; set; }

        /// <summary>
        /// 通知发送url;当商户的门店审核状态发生变化时，会向该地址推送消息
        /// </summary>
        [XmlElement("operate_notify_url")]
        public string OperateNotifyUrl { get; set; }

        /// <summary>
        /// 其它资质证明图片集；支持多张，逗号分隔
        /// </summary>
        [XmlElement("other_auth_images")]
        public string OtherAuthImages { get; set; }

        /// <summary>
        /// 门店是否支持停车，T表示支持，F表示不支持，不传在客户端不作展示
        /// </summary>
        [XmlElement("parking")]
        public string Parking { get; set; }

        /// <summary>
        /// 门店的签约ID
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 默认付款类型；如：付款码、扫码付、声波支付、在线买单、其它
        /// </summary>
        [XmlElement("pay_type")]
        public string PayType { get; set; }

        /// <summary>
        /// 门店收款账户，门店收款账户只能被查询，不能通过接口修改。如果为空，则表示门店收款账户为商户签约账户
        /// </summary>
        [XmlElement("payment_account")]
        public string PaymentAccount { get; set; }

        /// <summary>
        /// 图片集，是map转化成的json串，key是图片id,value是图片url
        /// </summary>
        [XmlElement("pic_coll")]
        public string PicColl { get; set; }

        /// <summary>
        /// 经过加工后的门店收款二维码
        /// </summary>
        [XmlElement("processed_qr_code")]
        public string ProcessedQrCode { get; set; }

        /// <summary>
        /// 门店运营归属人uid
        /// </summary>
        [XmlElement("provider_xiaoer_uid")]
        public string ProviderXiaoerUid { get; set; }

        /// <summary>
        /// 省份编码，国标码，详见国家统计局数据
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 门店收款二维码裸码
        /// </summary>
        [XmlElement("qr_code")]
        public string QrCode { get; set; }

        /// <summary>
        /// 门店标签；JSON格式。包括：keyMerchant-是否重点商户（T/F）；isHallMeal-堂食（T/F）；注：若标签 key 不存在，则门店无对应的标签
        /// </summary>
        [XmlElement("shop_tags")]
        public string ShopTags { get; set; }

        /// <summary>
        /// 外部门店编号；最长54位字符，该编号将作为收单接口的入参， 请开发者自行确保其唯一性
        /// </summary>
        [XmlElement("store_id")]
        public string StoreId { get; set; }

        /// <summary>
        /// 门店其他的服务，门店与用户线下兑现
        /// </summary>
        [XmlElement("value_added")]
        public string ValueAdded { get; set; }

        /// <summary>
        /// 门店是否支持WIFI，T表示支持，F表示不支持，不传在客户端不作展示
        /// </summary>
        [XmlElement("wifi")]
        public string Wifi { get; set; }
    }
}
