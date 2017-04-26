using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// Task Data Structure.
    /// </summary>
    [Serializable]
    public class Task : TopObject
    {
        /// <summary>
        /// 下载文件的MD5校验码，通过此校验码可以检查下载的文件是否是完整的。
        /// </summary>
        [XmlElement("check_code")]
        public string CheckCode { get; set; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        [XmlElement("created")]
        public string Created { get; set; }

        /// <summary>
        /// 大任务结果下载地址。当创建的认任务是大数据量的任务时，获取结果会返回此字段，同时subtasks列表会为空。 通过这个地址可以下载到结果的结构体，格式同普通任务下载的一样。 每次获取到的地址只能下载一次。下载的文件加上后缀名.zip打开。
        /// </summary>
        [XmlElement("download_url")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 此任务是由哪个api产生的
        /// </summary>
        [XmlElement("method")]
        public string Method { get; set; }

        /// <summary>
        /// 定时类型任务的执行时间点
        /// </summary>
        [XmlElement("schedule")]
        public string Schedule { get; set; }

        /// <summary>
        /// 异步任务处理状态。new（还未开始处理），doing（处理中），done（处理结束）。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 子任务处理结果，如果任务还没有处理完，返回的结果列表为空。如果任务处理完毕，返回子任务结果列表
        /// </summary>
        [XmlArray("subtasks")]
        [XmlArrayItem("subtask")]
        public List<Top.Api.Domain.Subtask> Subtasks { get; set; }

        /// <summary>
        /// 异步任务id。创建异步任务时返回的任务id号
        /// </summary>
        [XmlElement("task_id")]
        public long TaskId { get; set; }
    }
}
