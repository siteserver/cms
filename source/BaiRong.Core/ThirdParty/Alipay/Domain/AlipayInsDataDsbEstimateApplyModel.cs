using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsDataDsbEstimateApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsDataDsbEstimateApplyModel : AopObject
    {
        /// <summary>
        /// 车辆属性，json格式
        /// </summary>
        [XmlElement("car_properties")]
        public string CarProperties { get; set; }

        /// <summary>
        /// 案件属性，json字符串格式，目前key值有is_night_case(是否夜间案件）、is_human_hurt（是否有人伤）、is_only_outlook_damage（是否纯外观损伤）等
        /// </summary>
        [XmlElement("case_properties")]
        public string CaseProperties { get; set; }

        /// <summary>
        /// 车险商业险保单号
        /// </summary>
        [XmlElement("commercial_policy_no")]
        public string CommercialPolicyNo { get; set; }

        /// <summary>
        /// 交强险保单号
        /// </summary>
        [XmlElement("compulsory_policy_no")]
        public string CompulsoryPolicyNo { get; set; }

        /// <summary>
        /// 发动机号
        /// </summary>
        [XmlElement("engine_no")]
        public string EngineNo { get; set; }

        /// <summary>
        /// 保险公司定损单号，唯一标识一个定损单的id
        /// </summary>
        [XmlElement("estimate_no")]
        public string EstimateNo { get; set; }

        /// <summary>
        /// 定损请求uuid，唯一标识一次定损请求，用于做幂等控制
        /// </summary>
        [XmlElement("estimate_request_uuid")]
        public string EstimateRequestUuid { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        [XmlElement("frame_no")]
        public string FrameNo { get; set; }

        /// <summary>
        /// 车损图片信息列表
        /// </summary>
        [XmlArray("image_list")]
        [XmlArrayItem("alipay_ins_data_dsb_request_image_info")]
        public List<AlipayInsDataDsbRequestImageInfo> ImageList { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [XmlElement("license_no")]
        public string LicenseNo { get; set; }

        /// <summary>
        /// 车型厂牌，理赔车型
        /// </summary>
        [XmlElement("model_brand")]
        public string ModelBrand { get; set; }

        /// <summary>
        /// 维修企业属性，json字符串格式，目前key值有：type(企业类型/等级）、name（企业名称）、address（地址）、code（维修企业编码）等
        /// </summary>
        [XmlElement("repair_corp_properties")]
        public string RepairCorpProperties { get; set; }

        /// <summary>
        /// 报案号
        /// </summary>
        [XmlElement("report_no")]
        public string ReportNo { get; set; }

        /// <summary>
        /// 请求发生时的时间戳
        /// </summary>
        [XmlElement("request_timestamp")]
        public string RequestTimestamp { get; set; }

        /// <summary>
        /// 查勘号
        /// </summary>
        [XmlElement("survey_no")]
        public string SurveyNo { get; set; }
    }
}
