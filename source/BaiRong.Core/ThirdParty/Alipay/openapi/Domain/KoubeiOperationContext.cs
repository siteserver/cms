using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiOperationContext Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiOperationContext : AopObject
    {
        /// <summary>
        /// 如果是isv代操作，请传入ISV；如果是其他角色（商户MERCHANT、服务商PROVIDER、服务商员工S_STAFF、商户员工M_STAFF）操作，不用填写。
        /// </summary>
        [XmlElement("op_role")]
        public string OpRole { get; set; }
    }
}
