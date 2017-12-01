using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCardTemplateQueryResponse.
    /// </summary>
    public class AlipayMarketingCardTemplateQueryResponse : AopResponse
    {
        /// <summary>
        /// 业务卡号前缀，由商户指定  支付宝业务卡号生成规则：biz_no_prefix(商户指定)卡号前缀 + biz_no_suffix(实时生成）卡号后缀
        /// </summary>
        [XmlElement("biz_no_prefix")]
        public string BizNoPrefix { get; set; }

        /// <summary>
        /// 业务卡号后缀的长度  支付宝业务卡号生成规则：biz_no_prefix(商户指定)卡号前缀 + biz_no_suffix(实时生成）卡号后缀
        /// </summary>
        [XmlElement("biz_no_suffix_len")]
        public string BizNoSuffixLen { get; set; }

        /// <summary>
        /// 卡行动点配置；  行动点，即用户可点击跳转的区块，类似按钮控件的交互；  单张卡最多4个行动点。
        /// </summary>
        [XmlArray("card_action_list")]
        [XmlArrayItem("template_action_info_d_t_o")]
        public List<TemplateActionInfoDTO> CardActionList { get; set; }

        /// <summary>
        /// 卡等级配置
        /// </summary>
        [XmlArray("card_level_confs")]
        [XmlArrayItem("template_card_level_conf_d_t_o")]
        public List<TemplateCardLevelConfDTO> CardLevelConfs { get; set; }

        /// <summary>
        /// 会员卡类型：  OUT_MEMBER_CARD：外部权益卡
        /// </summary>
        [XmlElement("card_type")]
        public string CardType { get; set; }

        /// <summary>
        /// 栏位信息（卡包详情页面展现的栏位）
        /// </summary>
        [XmlArray("column_info_list")]
        [XmlArrayItem("template_column_info_d_t_o")]
        public List<TemplateColumnInfoDTO> ColumnInfoList { get; set; }

        /// <summary>
        /// 字段规则列表，会员卡开卡过程中，会员卡信息的生成规则，  例如：卡有效期为开卡后两年内有效，则设置为：DATE_IN_FUTURE
        /// </summary>
        [XmlArray("field_rule_list")]
        [XmlArrayItem("template_field_rule_d_t_o")]
        public List<TemplateFieldRuleDTO> FieldRuleList { get; set; }

        /// <summary>
        /// 商户动态码通知参数配置：  当write_off_type指定为商户动态码mdbarcode或mdqrcode时不为空；  在此字段配置用户打开会员卡时支付宝通知商户生成动态码（发码）的通知参数，如接收通知地址等。
        /// </summary>
        [XmlElement("mdcode_notify_conf")]
        public TemplateMdcodeNotifyConfDTO MdcodeNotifyConf { get; set; }

        /// <summary>
        /// 会员卡用户领卡配置，在门店等渠道露出领卡入口时，需要部署的商户领卡H5页面地址
        /// </summary>
        [XmlElement("open_card_conf")]
        public TemplateOpenCardConfDTO OpenCardConf { get; set; }

        /// <summary>
        /// 卡模板投放渠道
        /// </summary>
        [XmlArray("pub_channels")]
        [XmlArrayItem("pub_channel_d_t_o")]
        public List<PubChannelDTO> PubChannels { get; set; }

        /// <summary>
        /// 服务标签列表
        /// </summary>
        [XmlArray("service_label_list")]
        [XmlArrayItem("string")]
        public List<string> ServiceLabelList { get; set; }

        /// <summary>
        /// 门店ids
        /// </summary>
        [XmlArray("shop_ids")]
        [XmlArrayItem("string")]
        public List<string> ShopIds { get; set; }

        /// <summary>
        /// 权益信息，  1、在卡包的卡详情页面会自动添加权益栏位，展现会员卡特权，  2、如果添加门店渠道，则可在门店页展现会员卡的权益
        /// </summary>
        [XmlArray("template_benefit_info")]
        [XmlArrayItem("template_benefit_info_d_t_o")]
        public List<TemplateBenefitInfoDTO> TemplateBenefitInfo { get; set; }

        /// <summary>
        /// 模板样式信息(钱包展现效果)
        /// </summary>
        [XmlElement("template_style_info")]
        public TemplateStyleInfoDTO TemplateStyleInfo { get; set; }
    }
}
