using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiCraftsmanDataProviderCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiCraftsmanDataProviderCreateModel : AopObject
    {
        /// <summary>
        /// 手艺人账户名，仅支持小写字母和数字。上限12个小写字母或者数字。举例，若商户在口碑商家的登录账号为 testaop01@alipay.com,手艺人账号名为zhangsan，则手艺人登录口碑商家的账号名为 testaop01@alipay.com#zhangsan，获取登录密码需要扫商户app的员工激活码。从口碑商家app的员工管理进入员工详情页获取登录密码。
        /// </summary>
        [XmlElement("account")]
        public string Account { get; set; }

        /// <summary>
        /// 服务商、服务商员工、商户、商户员工等口碑角色操作时必填，对应为《koubei.member.data.oauth.query》中的auth_code，默认有效期24小时；isv自身角色操作的时候，无需传该参数
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 手艺人头像，在商家端手艺人管理和用户端手艺人个人简介中展示手艺人头像 （通过 alipay.offline.material.image.upload 接口上传图片获取的资源id），上限最大5M,支持bmp,png,jpeg,jpg,gif格式的图片。
        /// </summary>
        [XmlElement("avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// 从业起始年月日
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
        /// 外部手艺人id，由ISV生成，isv的appId + 外部手艺人id全局唯一
        /// </summary>
        [XmlElement("out_craftsman_id")]
        public string OutCraftsmanId { get; set; }

        /// <summary>
        /// 手艺人关联门店
        /// </summary>
        [XmlArray("shop_relations")]
        [XmlArrayItem("craftsman_shop_relation_open_model")]
        public List<CraftsmanShopRelationOpenModel> ShopRelations { get; set; }

        /// <summary>
        /// 描述手艺人擅长的技术（如烫染、二分式剪法、足疗、中医推拿、刮痧、火疗、拔罐、婚纱、儿童、写真...）。最多6个标签  每个标签字段上限10个字。
        /// </summary>
        [XmlArray("specialities")]
        [XmlArrayItem("string")]
        public List<string> Specialities { get; set; }

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
    }
}
