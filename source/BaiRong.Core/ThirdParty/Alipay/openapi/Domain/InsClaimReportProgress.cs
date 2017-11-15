using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsClaimReportProgress Data Structure.
    /// </summary>
    [Serializable]
    public class InsClaimReportProgress : AopObject
    {
        /// <summary>
        /// 案件更新内容
        /// </summary>
        [XmlElement("progress_update_content")]
        public string ProgressUpdateContent { get; set; }

        /// <summary>
        /// 案件更新进度时间
        /// </summary>
        [XmlElement("progress_update_time")]
        public string ProgressUpdateTime { get; set; }

        /// <summary>
        /// 进度状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
