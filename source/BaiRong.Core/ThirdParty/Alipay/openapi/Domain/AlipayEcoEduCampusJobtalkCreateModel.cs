using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduCampusJobtalkCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduCampusJobtalkCreateModel : AopObject
    {
        /// <summary>
        /// 公司在合作方的唯一标识id
        /// </summary>
        [XmlElement("campany_source")]
        public string CampanySource { get; set; }

        /// <summary>
        /// 工作城市
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 工作城市code
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 宣讲会公司法律名称（唯一）
        /// </summary>
        [XmlElement("company_lawname")]
        public string CompanyLawname { get; set; }

        /// <summary>
        /// 宣讲会公司全称
        /// </summary>
        [XmlElement("company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// 备用字段，json格式
        /// </summary>
        [XmlElement("content_var")]
        public string ContentVar { get; set; }

        /// <summary>
        /// 工作城市地区
        /// </summary>
        [XmlElement("district")]
        public string District { get; set; }

        /// <summary>
        /// 工作地点纬度
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 企业logo的url
        /// </summary>
        [XmlElement("logo_url")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// 工作地点经度
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 工作省份
        /// </summary>
        [XmlElement("province")]
        public string Province { get; set; }

        /// <summary>
        /// 工作省份code
        /// </summary>
        [XmlElement("province_code")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [XmlElement("street")]
        public string Street { get; set; }

        /// <summary>
        /// 宣讲会举办地点
        /// </summary>
        [XmlElement("talk_address")]
        public string TalkAddress { get; set; }

        /// <summary>
        /// 宣讲会正文，过滤要求：表格，字体字号，符号，图片，链接
        /// </summary>
        [XmlElement("talk_content")]
        public string TalkContent { get; set; }

        /// <summary>
        /// 宣讲会结束时间(毫秒数)
        /// </summary>
        [XmlElement("talk_endtime")]
        public string TalkEndtime { get; set; }

        /// <summary>
        /// 宣讲会举办时间(毫秒数)
        /// </summary>
        [XmlElement("talk_holdtime")]
        public string TalkHoldtime { get; set; }

        /// <summary>
        /// 宣讲会举办学校code
        /// </summary>
        [XmlElement("talk_school_code")]
        public string TalkSchoolCode { get; set; }

        /// <summary>
        /// 宣讲会举办学校
        /// </summary>
        [XmlElement("talk_school_name")]
        public string TalkSchoolName { get; set; }

        /// <summary>
        /// 宣讲会来源方id
        /// </summary>
        [XmlElement("talk_source_code")]
        public string TalkSourceCode { get; set; }

        /// <summary>
        /// 宣讲会在合作方的ID
        /// </summary>
        [XmlElement("talk_source_id")]
        public string TalkSourceId { get; set; }

        /// <summary>
        /// 宣讲会标题
        /// </summary>
        [XmlElement("talk_title")]
        public string TalkTitle { get; set; }

        /// <summary>
        /// 宣讲会类型  1=宣讲会  2=空中宣讲会  3=双选会
        /// </summary>
        [XmlElement("talk_type")]
        public long TalkType { get; set; }

        /// <summary>
        /// 宣讲会信息来源
        /// </summary>
        [XmlElement("talksource_source")]
        public string TalksourceSource { get; set; }

        /// <summary>
        /// 企业官网
        /// </summary>
        [XmlElement("web_url")]
        public string WebUrl { get; set; }
    }
}
