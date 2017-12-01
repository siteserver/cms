using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduJzPostPublishModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduJzPostPublishModel : AopObject
    {
        /// <summary>
        /// 年龄要求范围
        /// </summary>
        [XmlElement("age_demand")]
        public EduAgeDemand AgeDemand { get; set; }

        /// <summary>
        /// 工资/提成信息备注
        /// </summary>
        [XmlElement("commission")]
        public string Commission { get; set; }

        /// <summary>
        /// 客服联系方式
        /// </summary>
        [XmlElement("company_contact")]
        public string CompanyContact { get; set; }

        /// <summary>
        /// 商户 Logo
        /// </summary>
        [XmlElement("company_logo")]
        public string CompanyLogo { get; set; }

        /// <summary>
        /// 职位所属公司名称
        /// </summary>
        [XmlElement("company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// 联系方式 手机座机
        /// </summary>
        [XmlElement("contact_phone")]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 工作结束的日期
        /// </summary>
        [XmlElement("date_end")]
        public string DateEnd { get; set; }

        /// <summary>
        /// 工作开始的日期
        /// </summary>
        [XmlElement("date_start")]
        public string DateStart { get; set; }

        /// <summary>
        /// 报名截止日期
        /// </summary>
        [XmlElement("deadline")]
        public string Deadline { get; set; }

        /// <summary>
        /// 性别要求 0:不限；1:男；2:女
        /// </summary>
        [XmlElement("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 招聘人数
        /// </summary>
        [XmlElement("hire_number")]
        public string HireNumber { get; set; }

        /// <summary>
        /// 有无提成0:没有1:有
        /// </summary>
        [XmlElement("is_commission")]
        public string IsCommission { get; set; }

        /// <summary>
        /// 职位描述：工作内容、岗位要求
        /// </summary>
        [XmlElement("job_desc")]
        public string JobDesc { get; set; }

        /// <summary>
        /// 工作类型，0:短期兼职，1:长期兼职，2:周末兼职
        /// </summary>
        [XmlElement("job_type")]
        public string JobType { get; set; }

        /// <summary>
        /// 类型代码：兼职类型  1：传单派发  2：促销导购  3：话务客服  4：礼仪模特  5：家教助教  6：服务员  7：问卷调查  8：审核录入  9：地推拉访  10：其他  11：打包分拣  12：展会协助  13：充场  14：实习生  15：安保  16：送餐员  17：演出  18：翻译  19：校园代理  20：义工  21：食品促销  22：临时工
        /// </summary>
        [XmlElement("part_time_type")]
        public string PartTimeType { get; set; }

        /// <summary>
        /// 薪资待遇
        /// </summary>
        [XmlElement("payment")]
        public string Payment { get; set; }

        /// <summary>
        /// 薪资待遇备注
        /// </summary>
        [XmlElement("payment_remark")]
        public string PaymentRemark { get; set; }

        /// <summary>
        /// 结算方式：0日结；1次日结； 2周结；3半月结； 4月结； 5完工结；
        /// </summary>
        [XmlElement("payment_type")]
        public string PaymentType { get; set; }

        /// <summary>
        /// 工资
        /// </summary>
        [XmlElement("salary")]
        public string Salary { get; set; }

        /// <summary>
        /// 薪资单位：元/天；元/周；元/月(与结算方式匹配)
        /// </summary>
        [XmlElement("salary_unit")]
        public string SalaryUnit { get; set; }

        /// <summary>
        /// 兼职服务商职位id
        /// </summary>
        [XmlElement("service_post_id")]
        public string ServicePostId { get; set; }

        /// <summary>
        /// 职位供应商信息
        /// </summary>
        [XmlElement("source_info")]
        public EduSourceInfo SourceInfo { get; set; }

        /// <summary>
        /// 职位特殊要求多选项目：  - 普通话熟练；  - 有健康证；   - 自备正装；  - 携带学生证；  - 沟通能力强；  - 形象好；  - 服从安排；  - 积极主动；  - 认真负责；  - 活泼开朗；  - 吃苦耐劳；
        /// </summary>
        [XmlArray("special_demand")]
        [XmlArrayItem("string")]
        public List<string> SpecialDemand { get; set; }

        /// <summary>
        /// 职位标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 福利，1:包工作餐；2:包住宿；3:交通补助
        /// </summary>
        [XmlArray("welfare")]
        [XmlArrayItem("string")]
        public List<string> Welfare { get; set; }

        /// <summary>
        /// 工作地点
        /// </summary>
        [XmlArray("work_address")]
        [XmlArrayItem("edu_work_address")]
        public List<EduWorkAddress> WorkAddress { get; set; }

        /// <summary>
        /// 工作时间备注
        /// </summary>
        [XmlElement("work_time_remark")]
        public string WorkTimeRemark { get; set; }

        /// <summary>
        /// 每天工作时长 <以小时计>
        /// </summary>
        [XmlElement("working_hours")]
        public string WorkingHours { get; set; }
    }
}
