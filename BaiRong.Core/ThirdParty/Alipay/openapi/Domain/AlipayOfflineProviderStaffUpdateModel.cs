using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineProviderStaffUpdateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineProviderStaffUpdateModel : AopObject
    {
        /// <summary>
        /// 支付宝账号
        /// </summary>
        [XmlElement("alipay_no")]
        public string AlipayNo { get; set; }

        /// <summary>
        /// 行业类型
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 客户端请求IP
        /// </summary>
        [XmlElement("client_ip")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 新增员工的备注信息
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// 要同步员工的邮箱
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// 登录用户的staff_id
        /// </summary>
        [XmlElement("login_staff_id")]
        public string LoginStaffId { get; set; }

        /// <summary>
        /// 服务商pid
        /// </summary>
        [XmlElement("merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// 服务商id的类型
        /// </summary>
        [XmlElement("merchant_id_type")]
        public string MerchantIdType { get; set; }

        /// <summary>
        /// 云纵登录人员pid
        /// </summary>
        [XmlElement("ope_pid")]
        public string OpePid { get; set; }

        /// <summary>
        /// 同步云纵员工操作类型
        /// </summary>
        [XmlElement("operate_type")]
        public string OperateType { get; set; }

        /// <summary>
        /// 流水号参数
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        [XmlElement("role_type")]
        public string RoleType { get; set; }

        /// <summary>
        /// 修改删除员工的主键id
        /// </summary>
        [XmlElement("staff_id")]
        public string StaffId { get; set; }

        /// <summary>
        /// 要同步员工的电话号码
        /// </summary>
        [XmlElement("staff_mobile")]
        public string StaffMobile { get; set; }

        /// <summary>
        /// 新增员工姓名
        /// </summary>
        [XmlElement("staff_name")]
        public string StaffName { get; set; }

        /// <summary>
        /// 员工类型
        /// </summary>
        [XmlElement("staff_type")]
        public string StaffType { get; set; }
    }
}
