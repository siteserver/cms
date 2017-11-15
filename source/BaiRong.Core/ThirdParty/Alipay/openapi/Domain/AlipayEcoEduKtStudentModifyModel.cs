using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduKtStudentModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduKtStudentModifyModel : AopObject
    {
        /// <summary>
        /// 修改后的学生姓名  本接口调用时，child_name、student_code、student_identify、users这几个参数至少需要填写其中一个，不能同时为空
        /// </summary>
        [XmlElement("child_name")]
        public string ChildName { get; set; }

        /// <summary>
        /// 已经签约教育缴费的isv的支付宝PID
        /// </summary>
        [XmlElement("isv_pid")]
        public string IsvPid { get; set; }

        /// <summary>
        /// 学校编号，调用alipay.eco.edu.kt.schoolinfo.modify接口录入学校信息时，接口返回的编号
        /// </summary>
        [XmlElement("school_no")]
        public string SchoolNo { get; set; }

        /// <summary>
        /// 学校用来签约支付宝教育缴费的支付宝PID
        /// </summary>
        [XmlElement("school_pid")]
        public string SchoolPid { get; set; }

        /// <summary>
        /// 区分ISV操作，“D”表示删除，“U”表示更新，区分大小写。  如果为U，则学生名字，学号，身份证至少填写一项
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 修改后的学号  本接口调用时，child_name、student_code、student_identify、users这几个参数至少需要填写其中一个，不能同时为空
        /// </summary>
        [XmlElement("student_code")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 修改后的身份证号码  本接口调用时，child_name、student_code、student_identify、users这几个参数至少需要填写其中一个，不能同时为空
        /// </summary>
        [XmlElement("student_identify")]
        public string StudentIdentify { get; set; }

        /// <summary>
        /// 支付宝-中小学-教育缴费生成的学生唯一编号
        /// </summary>
        [XmlElement("student_no")]
        public string StudentNo { get; set; }

        /// <summary>
        /// 孩子的家长信息，最多一次输入20个家长。如果输入的家长信息不存在，则该改学生增加家长信息  本接口调用时，child_name、student_code、student_identify、users这几个参数至少需要填写其中一个，不能同时为空
        /// </summary>
        [XmlArray("users")]
        [XmlArrayItem("user_details")]
        public List<UserDetails> Users { get; set; }
    }
}
