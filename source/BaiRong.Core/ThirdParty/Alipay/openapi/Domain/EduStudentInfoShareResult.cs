using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// EduStudentInfoShareResult Data Structure.
    /// </summary>
    [Serializable]
    public class EduStudentInfoShareResult : AopObject
    {
        /// <summary>
        /// 教育分类
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 学生详细信息
        /// </summary>
        [XmlArray("student_infos")]
        [XmlArrayItem("student_info")]
        public List<StudentInfo> StudentInfos { get; set; }

        /// <summary>
        /// 用户的userid
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
