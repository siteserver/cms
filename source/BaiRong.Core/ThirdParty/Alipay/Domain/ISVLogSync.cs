using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ISVLogSync Data Structure.
    /// </summary>
    [Serializable]
    public class ISVLogSync : AopObject
    {
        /// <summary>
        /// 应用名
        /// </summary>
        [XmlElement("application")]
        public string Application { get; set; }

        /// <summary>
        /// isv自定义错误码， 该值传了表示接口调用业务失败或发生异常
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 异常堆栈
        /// </summary>
        [XmlElement("exception_stack_trace")]
        public string ExceptionStackTrace { get; set; }

        /// <summary>
        /// 执行时长，毫秒数。如果能取到尽量传入，涉及到接口耗时的监控
        /// </summary>
        [XmlElement("execution_millis")]
        public string ExecutionMillis { get; set; }

        /// <summary>
        /// 接口全限定名 包含远程rpc和内部调用
        /// </summary>
        [XmlElement("interface_name")]
        public string InterfaceName { get; set; }

        /// <summary>
        /// T 成功  F 失败
        /// </summary>
        [XmlElement("success")]
        public string Success { get; set; }

        /// <summary>
        /// 回流数据类型
        /// </summary>
        [XmlElement("sync_type")]
        public string SyncType { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [XmlElement("timestamp")]
        public string Timestamp { get; set; }
    }
}
