using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataEnterpriseStaffinfoUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataEnterpriseStaffinfoUploadModel : AopObject
    {
        /// <summary>
        /// 请求流水号，由ISV自定义，在ISV系统内唯一标示一次业务请求。
        /// </summary>
        [XmlElement("batch_id")]
        public string BatchId { get; set; }

        /// <summary>
        /// 企业名称  （参数说明：一个企业名称只能对应一个crowid，重复上传同一个企业名称，返回的crowid是同一个，upload包含创建和修改逻辑，同一个企业名称第一次上传是创建、后面再上传相同的企业名称就走修改逻辑）
        /// </summary>
        [XmlElement("enterprise_name")]
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 操作类型: UPLOAD (上传、修改)                  DEL（删除）  参数说明：DEL删除场景删除的是企业名称对应的用户uid信息
        /// </summary>
        [XmlElement("operator_type")]
        public string OperatorType { get; set; }

        /// <summary>
        /// 上传的企业员工信息列表，单次做多上传500个
        /// </summary>
        [XmlArray("staff_info")]
        [XmlArrayItem("staff_info")]
        public List<StaffInfo> StaffInfo { get; set; }
    }
}
