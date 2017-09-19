using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMedicalcareHosReportnotifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMedicalcareHosReportnotifyModel : AopObject
    {
        /// <summary>
        /// 科室信息
        /// </summary>
        [XmlElement("dept_info")]
        public MedicalHospitalDeptInfo DeptInfo { get; set; }

        /// <summary>
        /// 医生信息
        /// </summary>
        [XmlElement("doctor_info")]
        public MedicalHospitalDoctorInfo DoctorInfo { get; set; }

        /// <summary>
        /// 业务扩展参数json格式
        /// </summary>
        [XmlElement("extend_params")]
        public string ExtendParams { get; set; }

        /// <summary>
        /// 医院信息
        /// </summary>
        [XmlElement("hos_info")]
        public MedicalHospitalInfo HosInfo { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        [XmlElement("notify_time")]
        public string NotifyTime { get; set; }

        /// <summary>
        /// 操作类型  明确定义数据是创建还是更新  创建并更新CREATE_UPDATE  删除DELETE
        /// </summary>
        [XmlElement("operate")]
        public string Operate { get; set; }

        /// <summary>
        /// 患者证件号码  获取方式通过支付宝钱包用户信息共享接口中获取证件号或者手工输入证件号
        /// </summary>
        [XmlElement("patient_card_no")]
        public string PatientCardNo { get; set; }

        /// <summary>
        /// 证件类型  01 身份证  02 护照  03 军官证  04 士兵证  05 户口本  06 警官证  07 台湾居民来往大陆通行证（简称“台胞证”）  08 港澳居民来往内地通行证（简称“回乡证”）  09 临时身份证  10 港澳通行证  11 营业执照  12 外国人居留证  13 香港身份证  14 武警证  15 组织机构代码证  16 行政机关  17 社会团体  18 军队  19 武警  20 下属机构(具有主管单位批文号)  21 基金会  99 其它
        /// </summary>
        [XmlElement("patient_card_type")]
        public string PatientCardType { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        [XmlElement("patient_name")]
        public string PatientName { get; set; }

        /// <summary>
        /// 挂号订单号,商户生成
        /// </summary>
        [XmlElement("reg_out_trade_no")]
        public string RegOutTradeNo { get; set; }

        /// <summary>
        /// 报告明细
        /// </summary>
        [XmlArray("report_list")]
        [XmlArrayItem("medical_hospital_report_list")]
        public List<MedicalHospitalReportList> ReportList { get; set; }

        /// <summary>
        /// 第三方唯一序列号（可以是订单号确保唯一）
        /// </summary>
        [XmlElement("third_no")]
        public string ThirdNo { get; set; }

        /// <summary>
        /// 诊疗订单号，商户生成
        /// </summary>
        [XmlElement("treat_out_trade_no")]
        public string TreatOutTradeNo { get; set; }

        /// <summary>
        /// 支付宝用户Id，可以通过支付宝钱包用户信息共享接口获取支付宝账户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
