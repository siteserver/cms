using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineProviderStaffUpdateResponse.
    /// </summary>
    public class AlipayOfflineProviderStaffUpdateResponse : AopResponse
    {
        /// <summary>
        /// 同步失败错误原因信息
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 同步操作完成后的结果
        /// </summary>
        [XmlElement("operate_result")]
        public string OperateResult { get; set; }

        /// <summary>
        /// 同步员工的操作类型
        /// </summary>
        [XmlElement("operate_type")]
        public string OperateType { get; set; }

        /// <summary>
        /// 员工pid
        /// </summary>
        [XmlElement("pid")]
        public string Pid { get; set; }

        /// <summary>
        /// 服务商员工staff_id
        /// </summary>
        [XmlElement("staff_id")]
        public string StaffId { get; set; }
    }
}
