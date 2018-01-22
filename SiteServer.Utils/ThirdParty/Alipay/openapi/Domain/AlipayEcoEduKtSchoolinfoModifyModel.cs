using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduKtSchoolinfoModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduKtSchoolinfoModifyModel : AopObject
    {
        /// <summary>
        /// 与浙江网商交易见证平台有交互ISV输入网商交易异步通知回调URL，教育缴费同步账单信息给网商，网商会回调此url，ISV即可获取网商相关的参数，根据教育缴费平台账单发送接口返回的 order_no和网商返回的outer_trade_no来对应账单信息。
        /// </summary>
        [XmlElement("bank_notify_url")]
        public string BankNotifyUrl { get; set; }

        /// <summary>
        /// 与浙江网商交易见证平台有交互的ISV,由交易见证平台分配给合作方业务平台 签约唯一ID ，由网商分配给ISV的渠道参数
        /// </summary>
        [XmlElement("bank_partner_id")]
        public string BankPartnerId { get; set; }

        /// <summary>
        /// 与浙江网商交易见证平台有交互的ISV在创建账户获得的member_id，由网商分配
        /// </summary>
        [XmlElement("bank_uid")]
        public string BankUid { get; set; }

        /// <summary>
        /// 对应集团到卡模式的银行编号.学校与支付宝后台签约时，由学校提交给支付宝的编号
        /// </summary>
        [XmlElement("bankcard_no")]
        public string BankcardNo { get; set; }

        /// <summary>
        /// 城市的国家编码（国家统计局出版的行政区划代码 http://www.stats.gov.cn/tjsj/tjbz/xzqhdm/）
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [XmlElement("city_name")]
        public string CityName { get; set; }

        /// <summary>
        /// 区县的国家编码（国家统计局出版的行政区划代码 http://www.stats.gov.cn/tjsj/tjbz/xzqhdm/）
        /// </summary>
        [XmlElement("district_code")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 区县名称
        /// </summary>
        [XmlElement("district_name")]
        public string DistrictName { get; set; }

        /// <summary>
        /// 商家名称，每个接入教育缴费的ISV商家名称，由ISV自己提供
        /// </summary>
        [XmlElement("isv_name")]
        public string IsvName { get; set; }

        /// <summary>
        /// 注意：本参数从1.3版本开始已经废弃，不再需要传递。    由支付宝提供的给已经签约的isv的编码，业务上一般直接采用isv的支付宝PID。
        /// </summary>
        [XmlElement("isv_no")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 此通知地址是为了保持教育缴费平台与ISV商户支付状态一致性。用户支付成功后，支付宝会根据本isv_notify_url，通过POST请求的形式将支付结果作为参数通知到商户系统，ISV商户可以根据返回的参数更新账单状态。
        /// </summary>
        [XmlElement("isv_notify_url")]
        public string IsvNotifyUrl { get; set; }

        /// <summary>
        /// ISV联系电话,用于账单详情页面显示
        /// </summary>
        [XmlElement("isv_phone")]
        public string IsvPhone { get; set; }

        /// <summary>
        /// 填写已经签约教育缴费的isv的支付宝PID
        /// </summary>
        [XmlElement("isv_pid")]
        public string IsvPid { get; set; }

        /// <summary>
        /// 省份的国家编码（国家统计局出版的行政区划代码 http://www.stats.gov.cn/tjsj/tjbz/xzqhdm/）
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 省名称
        /// </summary>
        [XmlElement("province_name")]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 学校的校徽或logo，在学校展示页面作为学校的标识。该字段为图片的链接地址，只支持png或jpg图片格式，图片高度为108，宽度为108 ，不大于20k。  注意：此链接要求公网可以访问，否则无法正常展示。
        /// </summary>
        [XmlElement("school_icon")]
        public string SchoolIcon { get; set; }

        /// <summary>
        /// 如果填写了school_icon参数，则此字段不能为空。目前只支持png和jpg两种格式
        /// </summary>
        [XmlElement("school_icon_type")]
        public string SchoolIconType { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        [XmlElement("school_name")]
        public string SchoolName { get; set; }

        /// <summary>
        /// 学校用来签约支付宝教育缴费的支付宝PID
        /// </summary>
        [XmlElement("school_pid")]
        public string SchoolPid { get; set; }

        /// <summary>
        /// 学校(机构)标识码（由教育部按照国家标准及编码规则编制，可以在教育局官网查询）
        /// </summary>
        [XmlElement("school_stdcode")]
        public string SchoolStdcode { get; set; }

        /// <summary>
        /// 学校的类型：  1：代表托儿所； 2：代表幼儿园；3：代表小学；4：代表初中；5：代表高中。  如果学校兼有多种属性，可以连写，比如：  45：代表兼有初中部高中部；34：代表兼有小学部初中部
        /// </summary>
        [XmlElement("school_type")]
        public string SchoolType { get; set; }

        /// <summary>
        /// 与浙江网商交易见证平台有交互的ISV,由网商分配给ISV的渠道参数
        /// </summary>
        [XmlElement("white_channel_code")]
        public string WhiteChannelCode { get; set; }
    }
}
