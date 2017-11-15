using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMedicalcareHosRegnotifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMedicalcareHosRegnotifyModel : AopObject
    {
        /// <summary>
        /// 业务类型:  挂号成功：REG_SUCCESS  挂号取销：REG_CANCEL
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 撤销说明
        /// </summary>
        [XmlElement("cancel_desc")]
        public string CancelDesc { get; set; }

        /// <summary>
        /// 取消原因  备注:业务类型是  REG_CANCEL，不可空
        /// </summary>
        [XmlElement("cancel_reason")]
        public string CancelReason { get; set; }

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
        /// 排队号
        /// </summary>
        [XmlElement("line_no")]
        public long LineNo { get; set; }

        /// <summary>
        /// 通知日期,如果不传通知时间，逻辑由支付宝内部消化判断   格式为yyyy-MM-dd HH:mm:ss。
        /// </summary>
        [XmlElement("notify_time")]
        public string NotifyTime { get; set; }

        /// <summary>
        /// 操作类型：  明确定义数据是创建还是更新  创建并更新CREATE_UPDATE  删除DELETE
        /// </summary>
        [XmlElement("operate")]
        public string Operate { get; set; }

        /// <summary>
        /// 订单详情链接  链接开头为https或http
        /// </summary>
        [XmlElement("order_link")]
        public string OrderLink { get; set; }

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
        /// 重新预约医生链接  链接开头为https或http  备注:如biz_type的值为REG_CANCEL则不可空
        /// </summary>
        [XmlElement("reg_link")]
        public string RegLink { get; set; }

        /// <summary>
        /// 第三方唯一序列号（可以是订单号确保唯一）
        /// </summary>
        [XmlElement("third_no")]
        public string ThirdNo { get; set; }

        /// <summary>
        /// 就诊日期 格式为yyyy-MM-dd HH:mm:ss。
        /// </summary>
        [XmlElement("treat_date")]
        public string TreatDate { get; set; }

        /// <summary>
        /// 就诊显示日期json格式：  类型：  时间区间类型：range  中文显示类型：cn  备注：  1.range类型HH:mm-HH:mm 中间中横线隔开  {"range":"09:00-10:00"}  2.cn类型  上午  1  下午  2  晚上  3  {"cn":"1"}
        /// </summary>
        [XmlElement("treat_date_ext")]
        public string TreatDateExt { get; set; }

        /// <summary>
        /// 支付宝用户Id，可以通过支付宝钱包用户信息共享接口获取支付宝账户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
