using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CraftsmanOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class CraftsmanOpenModel : AopObject
    {
        /// <summary>
        /// 手艺人账户名，仅支持小写字母和数字。上限12个小写字母或者数字。举例，若商户在口碑商家的登录账号为 testaop01@alipay.com,手艺人账号名为zhangsan，则手艺人登录口碑商家的账号名为 testaop01@alipay.com#zhangsan，获取登录密码需要扫商户app的员工激活码。从口碑商家app的员工管理进入员工详情页获取登录密码。
        /// </summary>
        [XmlElement("account")]
        public string Account { get; set; }

        /// <summary>
        /// 手艺人评分信息
        /// </summary>
        [XmlElement("assessment")]
        public CraftsmanAssessment Assessment { get; set; }

        /// <summary>
        /// 手艺人头像，在商家端手艺人管理和用户端手艺人个人简介中展示手艺人头像 （通过 alipay.offline.material.image.upload 接口上传图片获取的资源id），上限最大5M,支持bmp,png,jpeg,jpg,gif格式的图片。
        /// </summary>
        [XmlElement("avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// 从业起始年月
        /// </summary>
        [XmlElement("career_begin")]
        public string CareerBegin { get; set; }

        /// <summary>
        /// 职业。目前只能传支持一个。枚举类型目前有19种，发型师、美甲师、美容师、美睫师、纹绣师、纹身师、摄影师、教练、教师、化妆师、司仪、摄像师、按摩技师、足疗技师、灸疗师、理疗师、修脚师、采耳师、其他。
        /// </summary>
        [XmlArray("careers")]
        [XmlArrayItem("string")]
        public List<string> Careers { get; set; }

        /// <summary>
        /// 口碑手艺人id
        /// </summary>
        [XmlElement("craftsman_id")]
        public string CraftsmanId { get; set; }

        /// <summary>
        /// 手艺人简介，上限300字。
        /// </summary>
        [XmlElement("introduction")]
        public string Introduction { get; set; }

        /// <summary>
        /// 手艺人真实姓名，isv生成，由手艺人用户在isv系统填写，展示给商户看。上限40个字。
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 昵称，上限16字,手艺人个人主页名称，展示给消费者看。
        /// </summary>
        [XmlElement("nick_name")]
        public string NickName { get; set; }

        /// <summary>
        /// 手艺人关联的操作员ID
        /// </summary>
        [XmlElement("operator_id")]
        public string OperatorId { get; set; }

        /// <summary>
        /// 外部手艺人id，由ISV生成，isv的appId + 外部手艺人id全局唯一
        /// </summary>
        [XmlElement("out_craftsman_id")]
        public string OutCraftsmanId { get; set; }

        /// <summary>
        /// 收款二维码地址，手艺人收款码，每个手艺人都有一个收款二维码。该二维码收款所得的金额进入商户的账号，如果手艺人所在的门店设置了门店收款账号，则资金进入门店收款账号，如果没有设置门店收款账号，则资金进入商户与口碑开店签约的支付宝账号。
        /// </summary>
        [XmlElement("qr_code")]
        public string QrCode { get; set; }

        /// <summary>
        /// 手艺人所属门店
        /// </summary>
        [XmlArray("shop_relations")]
        [XmlArrayItem("craftsman_shop_relation_open_model")]
        public List<CraftsmanShopRelationOpenModel> ShopRelations { get; set; }

        /// <summary>
        /// 描述手艺人擅长的技术（如烫染、二分式剪法、足疗、中医推拿、刮痧、火疗、拔罐、婚纱、儿童、写真...）。最多6个标签，每个标签字段上限10个字。
        /// </summary>
        [XmlArray("specialities")]
        [XmlArrayItem("string")]
        public List<string> Specialities { get; set; }

        /// <summary>
        /// 手艺人状态，EFFECTIVE和INVALID，生效和失效。失效状态一般用于手艺人已离职 或者 手艺人发布不实信息导致用户投诉被平台处罚。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 手艺人的手机号，在商家端和用户端展示，不支持座机
        /// </summary>
        [XmlElement("tel_num")]
        public string TelNum { get; set; }

        /// <summary>
        /// 头衔，手艺人在店内的职称。上限10个字。
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 支付宝账户uid
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
