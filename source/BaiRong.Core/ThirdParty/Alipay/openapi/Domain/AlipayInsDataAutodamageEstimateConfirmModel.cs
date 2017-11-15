using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsDataAutodamageEstimateConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsDataAutodamageEstimateConfirmModel : AopObject
    {
        /// <summary>
        /// 业务类型，2表示机构核损，3表示机构定损
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

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
        /// 汽车发动机号
        /// </summary>
        [XmlElement("engine_no")]
        public string EngineNo { get; set; }

        /// <summary>
        /// 核损详情对象列表
        /// </summary>
        [XmlArray("estimate_detail_list")]
        [XmlArrayItem("ins_data_autodamage_estimate_confirm_model")]
        public List<InsDataAutodamageEstimateConfirmModel> EstimateDetailList { get; set; }

        /// <summary>
        /// 保险公司定损单号，唯一标识一个定损单的id
        /// </summary>
        [XmlElement("estimate_no")]
        public string EstimateNo { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        [XmlElement("frame_no")]
        public string FrameNo { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [XmlElement("license_no")]
        public string LicenseNo { get; set; }

        /// <summary>
        /// 车型厂牌
        /// </summary>
        [XmlElement("model_brand")]
        public string ModelBrand { get; set; }

        /// <summary>
        /// 维修企业名称
        /// </summary>
        [XmlElement("repair_corp_name")]
        public string RepairCorpName { get; set; }

        /// <summary>
        /// 维修企业类型
        /// </summary>
        [XmlElement("repair_corp_type")]
        public string RepairCorpType { get; set; }

        /// <summary>
        /// 车险报案号
        /// </summary>
        [XmlElement("report_no")]
        public string ReportNo { get; set; }

        /// <summary>
        /// 查勘号
        /// </summary>
        [XmlElement("survey_no")]
        public string SurveyNo { get; set; }

        /// <summary>
        /// 车损总金额，单位：元
        /// </summary>
        [XmlElement("total_damage_amount")]
        public string TotalDamageAmount { get; set; }

        /// <summary>
        /// 残值扣除总金额，单位：元
        /// </summary>
        [XmlElement("total_remain_value")]
        public string TotalRemainValue { get; set; }
    }
}
