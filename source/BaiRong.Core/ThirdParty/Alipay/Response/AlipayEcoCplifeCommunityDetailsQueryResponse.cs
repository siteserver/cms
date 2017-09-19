using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoCplifeCommunityDetailsQueryResponse.
    /// </summary>
    public class AlipayEcoCplifeCommunityDetailsQueryResponse : AopResponse
    {
        /// <summary>
        /// 开发者关联的高德地图中住宅、住宿或地名地址等小区相关类型的POI（地图兴趣点）ID列表和POI名称，中间用"|"分隔。
        /// </summary>
        [XmlArray("associated_pois")]
        [XmlArrayItem("string")]
        public List<string> AssociatedPois { get; set; }

        /// <summary>
        /// 小区审核状态，小区审核状态关联小区主服务的审核状态：  AUDITING：审核中；  AUDIT_FAILED：审核驳回；  AUDIT_SUCCESS：审核通过。
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

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
        /// 小区所在的经纬度列表（注：需要是高德坐标系），每对经纬度用"|"分隔，经度在前，纬度在后。    注：若新建的小区覆盖多个片区，最多包含5组经纬度，其中第一组作为主经纬度。
        /// </summary>
        [XmlArray("community_locations")]
        [XmlArrayItem("string")]
        public List<string> CommunityLocations { get; set; }

        /// <summary>
        /// 小区名称。
        /// </summary>
        [XmlElement("community_name")]
        public string CommunityName { get; set; }

        /// <summary>
        /// 小区已初始化的服务列表
        /// </summary>
        [XmlArray("community_services")]
        [XmlArrayItem("c_p_comm_services")]
        public List<CPCommServices> CommunityServices { get; set; }

        /// <summary>
        /// 小区当前状态，状态值：  PENDING_ONLINE 待上线  ONLINE - 上线  MAINTAIN - 维护中  OFFLINE - 下线    新创建的小区为PENDING_ONLINE待上线状态，需要尽快初始化基础服务，完成开发和验证，并提交服务上线申请。由支付宝小二审核通过后完成服务和小区上线。
        /// </summary>
        [XmlElement("community_status")]
        public string CommunityStatus { get; set; }

        /// <summary>
        /// 区县编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("district_code")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 小区创建时间
        /// </summary>
        [XmlElement("gmt_created")]
        public string GmtCreated { get; set; }

        /// <summary>
        /// 小区最近修改时间（包括状态变更）。
        /// </summary>
        [XmlElement("gmt_modified")]
        public string GmtModified { get; set; }

        /// <summary>
        /// 物业服务热线或联系电话，便于用户在需要时联系物业。
        /// </summary>
        [XmlElement("hotline")]
        public string Hotline { get; set; }

        /// <summary>
        /// 若开发者和支付宝签署了相关协议，会返回开发者的PID（Partner ID）。
        /// </summary>
        [XmlElement("isv_pid")]
        public string IsvPid { get; set; }

        /// <summary>
        /// 小区关联的物业公司名称
        /// </summary>
        [XmlElement("merchant_firm_name")]
        public string MerchantFirmName { get; set; }

        /// <summary>
        /// 小区对应的物业公司支付宝账号PID（合作伙伴partner id）。
        /// </summary>
        [XmlElement("merchant_pid")]
        public string MerchantPid { get; set; }

        /// <summary>
        /// 若从小区当前状态到下一状态需要完成下一步条件代码，则返回该字段，否则不返回。    操作主体有：  INVOKER - 接口调用方  MERCHANT - 物业公司  AUDITOR - 平台审核方    条件代码包括但不限于：  WAIT_SERVICE_PROVISION - 等待基础服务初始化  WAIT_PROD_VERIFICATION - 等待在生产环境通过自测  WAIT_ONLINE_APPLICATION - 等待提起上线申请  WAIT_OFFLINE_APPLICATION - 等待提起下线申请  WAIT_CONFIRMATION - 等待相关方确认  WAIT_AUDITING - 等待审核
        /// </summary>
        [XmlElement("next_action")]
        public string NextAction { get; set; }

        /// <summary>
        /// 小区在物业系统中的唯一编号，若开发者传入过，则返回。
        /// </summary>
        [XmlElement("out_community_id")]
        public string OutCommunityId { get; set; }

        /// <summary>
        /// 省份编码，国标码，详见国家统计局数据 <a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 若小区上线后，返回小区主页推广二维码图片链接
        /// </summary>
        [XmlElement("qr_code_image")]
        public string QrCodeImage { get; set; }
    }
}
