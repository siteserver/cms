using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCampaignDiscountQueryResponse.
    /// </summary>
    public class AlipayMarketingCampaignDiscountQueryResponse : AopResponse
    {
        /// <summary>
        /// 活动唯一标示
        /// </summary>
        [XmlElement("camp_code")]
        public string CampCode { get; set; }

        /// <summary>
        /// 用于账户立减优惠,渠道立减优惠活动时,在收银台页面显示给会员看,最多512个字符，汉字、英文字母、数字都算一个，本输入框支持简单的html符号。
        /// </summary>
        [XmlElement("camp_desc")]
        public string CampDesc { get; set; }

        /// <summary>
        /// camp_id:活动id
        /// </summary>
        [XmlElement("camp_id")]
        public string CampId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [XmlElement("camp_name")]
        public string CampName { get; set; }

        /// <summary>
        /// 活动slogan.活动Slogan会在前台显示，谨慎填写
        /// </summary>
        [XmlElement("camp_slogan")]
        public string CampSlogan { get; set; }

        /// <summary>
        /// 活动状态，已经创建，暂停，上线，结束
        /// </summary>
        [XmlElement("camp_status")]
        public string CampStatus { get; set; }

        /// <summary>
        /// 折扣方式模型 如果类型选了discount,则这个模型必须输入
        /// </summary>
        [XmlElement("discount_dst_camp_prize_model")]
        public DiscountDstCampPrizeModel DiscountDstCampPrizeModel { get; set; }

        /// <summary>
        /// 立减规则模型
        /// </summary>
        [XmlElement("dst_camp_rule_model")]
        public DstCampRuleModel DstCampRuleModel { get; set; }

        /// <summary>
        /// 活动子时间，可以不传
        /// </summary>
        [XmlArray("dst_camp_sub_time_model_list")]
        [XmlArrayItem("date_area_model")]
        public List<DateAreaModel> DstCampSubTimeModelList { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [XmlElement("gmt_end")]
        public string GmtEnd { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 奖品类型. 打折   满减   单笔减   阶梯优惠   抹零优惠    随机立减   订单金额减至       折扣方式       DISCOUNT("discount", "折扣方式"),       REDUCE("reduce", "满立减"),      SINGLE("single", "单笔减"),       STAGED_DISCOUNT("staged_discount", "阶梯优惠"),       RESET_ZERO_DISCOUNT("reset_zero_discount", "抹零优惠"),     RANDOM_DISCOUNT("random", "随机立减");      REDUCE_TO_DISCOUNT("reduce_to_discount","订单金额减至 ")
        /// </summary>
        [XmlElement("prize_type")]
        public string PrizeType { get; set; }

        /// <summary>
        /// 随机立减模型如果类型选了random,则这个模型必须输入
        /// </summary>
        [XmlElement("random_discount_dst_camp_prize_model")]
        public RandomDiscountDstCampPrizeModel RandomDiscountDstCampPrizeModel { get; set; }

        /// <summary>
        /// 满立减奖品模型 如果类型选了reduce,则这个模型必须输入
        /// </summary>
        [XmlElement("reduce_dst_camp_prize_model")]
        public ReduceDstCampPrizeModel ReduceDstCampPrizeModel { get; set; }

        /// <summary>
        /// 订单金额减至模型如果类型选了reduce_to_discount,则这个模型必须输入
        /// </summary>
        [XmlElement("reduce_to_discount_dst_camp_prize_model")]
        public ReduceToDiscountDstCampPrizeModel ReduceToDiscountDstCampPrizeModel { get; set; }

        /// <summary>
        /// 抹零优惠模型如果类型选了reset_zero_discount,则这个模型必须输入
        /// </summary>
        [XmlElement("reset_zero_dst_camp_prize_model")]
        public ResetZeroDstCampPrizeModel ResetZeroDstCampPrizeModel { get; set; }

        /// <summary>
        /// 单笔减奖品模型如果类型选了single,则这个模型必须输入
        /// </summary>
        [XmlElement("single_dst_camp_prize_model")]
        public SingleDstCampPrizeModel SingleDstCampPrizeModel { get; set; }

        /// <summary>
        /// 阶梯优惠如果类型选了staged_discount,则这个模型必须输入
        /// </summary>
        [XmlElement("staged_discount_dst_camp_prize_model")]
        public StagedDiscountDstCampPrizeModel StagedDiscountDstCampPrizeModel { get; set; }
    }
}
