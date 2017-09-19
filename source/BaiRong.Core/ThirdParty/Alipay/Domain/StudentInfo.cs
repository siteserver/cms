using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// StudentInfo Data Structure.
    /// </summary>
    [Serializable]
    public class StudentInfo : AopObject
    {
        /// <summary>
        /// 学校所在行政区域编码
        /// </summary>
        [XmlElement("city_no")]
        public string CityNo { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [XmlElement("class_name")]
        public string ClassName { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        [XmlElement("college_name")]
        public string CollegeName { get; set; }

        /// <summary>
        /// 学校机构编号
        /// </summary>
        [XmlElement("college_no")]
        public string CollegeNo { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [XmlElement("degree")]
        public string Degree { get; set; }

        /// <summary>
        /// 院系
        /// </summary>
        [XmlElement("departments")]
        public string Departments { get; set; }

        /// <summary>
        /// 入校时间
        /// </summary>
        [XmlElement("gmt_enrollment")]
        public string GmtEnrollment { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        [XmlElement("gmt_graduation")]
        public string GmtGraduation { get; set; }

        /// <summary>
        /// 专业
        /// </summary>
        [XmlElement("major")]
        public string Major { get; set; }

        /// <summary>
        /// 学生证号
        /// </summary>
        [XmlElement("student_id")]
        public string StudentId { get; set; }
    }
}
