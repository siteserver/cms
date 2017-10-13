using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDaoweiSpModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDaoweiSpModifyModel : AopObject
    {
        /// <summary>
        /// 服务者的可用时间表。其中Duration和Unit配合使用，例如duration=30，unit=MIN表示将一天分为以30分钟一小段的时间片段。Unit：目前支持MIN（分钟）。Date：YYYY-MM-DD格式。Bitmap：根据定义的间隔长度跟单位，将date的时间切分，例如将2016-11-29整天按30分钟为一段切分为48段： 111111111111111111111111111111111110000011111111 ， 其中0表示不可用，1表示可用，如果工作日全天可用则每个分段都为1
        /// </summary>
        [XmlElement("calendar_schedule")]
        public CalendarScheduleInfo CalendarSchedule { get; set; }

        /// <summary>
        /// 服务者的身份证号码
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 服务者的证件类型（目前只支持身份证号：IDENTITY_CARD）
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 服务者的描述，会进行安全审核，请勿传包含敏感信息的昵称，如果审核传含有敏感信息，需修改后重新同步服务者的描述信息
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 服务者服务列表信息：包括服务者可提供的类目服务和证书信息等，其中license_id是商家服务者证照的唯一标识，用于确定商家的某个服务者的某个证照，仅支持数字、字母和下划线
        /// </summary>
        [XmlArray("license_list")]
        [XmlArrayItem("license_info")]
        public List<LicenseInfo> LicenseList { get; set; }

        /// <summary>
        /// 服务者的支付宝登录账号
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }

        /// <summary>
        /// 服务者的手机号
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 第三方服务者的姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 服务者昵称，会进行安全审核，请勿传包含敏感信息的昵称，如果审核传含有敏感信息，需修改后重新同步服务者信息
        /// </summary>
        [XmlElement("nick_name")]
        public string NickName { get; set; }

        /// <summary>
        /// 商家服务者id，由商家维护的该商家下某个服务者的唯一标识，仅支持数字、字母和下划线的组合
        /// </summary>
        [XmlElement("out_sp_id")]
        public string OutSpId { get; set; }

        /// <summary>
        /// 服务者的头像url，只支持https，图片大小限制60K以下。请勿发布涉及黄赌毒以及其他违反国家法律法规的图片，如果有安全问题，将会通知商家修改后重新同步服务者头像
        /// </summary>
        [XmlElement("photo_url")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 服务状态，支持以下状态：  ON（上架）  OFF（下架）  DELETE（删除）
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
