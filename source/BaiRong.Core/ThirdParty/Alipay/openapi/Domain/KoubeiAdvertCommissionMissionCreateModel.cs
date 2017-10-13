using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionMissionCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionMissionCreateModel : AopObject
    {
        /// <summary>
        /// 条款分如下三种条款，同时“单品券”只能设置固定金额，“全场券”只能设置  比例的。  PERCENTAGE_CLAUSE：百分比分佣结算条款，和"固定金额分佣结算条款"互斥，条款设置要求，可从koubei.advert.data.conf.query获取  QUOTA_CLAUSE：固定金额分佣结算条款  MISSION_CLAIM_CLAUSE：专属认领人条款，设定专属人条款，则只能条款中指定人员claimers可认领
        /// </summary>
        [XmlArray("commission_clause")]
        [XmlArrayItem("kb_advert_commission_clause")]
        public List<KbAdvertCommissionClause> CommissionClause { get; set; }

        /// <summary>
        /// 主键ID，根据identify_type，设置相应的ID  activity_id:运营活动ID，可以通过koubei.marketing.campaign.activity.create获取  voucher：商品ID，可以在商家中心创建商品获得
        /// </summary>
        [XmlElement("identify")]
        public string Identify { get; set; }

        /// <summary>
        /// 主键类型，需要配置分佣任务的标的ID，现在主要支持如下两种  activity_id：运营活动ID  voucher：商品ID
        /// </summary>
        [XmlElement("identify_type")]
        public string IdentifyType { get; set; }

        /// <summary>
        /// 分佣任务名称，由于不在任何场景不显示，因此可ISV按自己需求自行定义
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 操作人id，必须和operator_type配对出现，不填时默认是商户ID  MERCHANT时填商户ID，同时服务商创建分佣任务，需要审批  PROVIDER时填写服务商ID
        /// </summary>
        [XmlElement("operator_id")]
        public string OperatorId { get; set; }

        /// <summary>
        /// 操作人类型,有以下值可填选项（默认不需要填这个字段，默认为MERCHANT）：  MERCHANT：外部商户  PROVIDER：服务商
        /// </summary>
        [XmlElement("operator_type")]
        public string OperatorType { get; set; }

        /// <summary>
        /// 智能营销信息  通过koubei.marketing.data.smartactivity.config接口ext_info字段获取  plan_id：为ext_info中的SMART_PROMO_PLAN_ID（方案ID）  group_id：为ext_info中的SMART_PROMO_GROUP_ID(方案组ID),  对于和智能营销无关的场景，可以不输入该字段
        /// </summary>
        [XmlElement("smart_promo")]
        public KbadvertSmartPromoRequest SmartPromo { get; set; }
    }
}
