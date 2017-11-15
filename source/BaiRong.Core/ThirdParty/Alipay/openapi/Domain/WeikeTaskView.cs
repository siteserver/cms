using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// WeikeTaskView Data Structure.
    /// </summary>
    [Serializable]
    public class WeikeTaskView : AopObject
    {
        /// <summary>
        /// 任务视图描述
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 蚂蚁微客对当前任务视图编号
        /// </summary>
        [XmlElement("id")]
        public long Id { get; set; }

        /// <summary>
        /// 任务视图图片链接
        /// </summary>
        [XmlElement("img")]
        public string Img { get; set; }

        /// <summary>
        /// 任务视图名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 任务视图对应的任务薪资
        /// </summary>
        [XmlElement("salary")]
        public string Salary { get; set; }

        /// <summary>
        /// 当前任务视图剩余任务数
        /// </summary>
        [XmlElement("task_count")]
        public long TaskCount { get; set; }

        /// <summary>
        /// 点击任务视图后唤起微客对应页面的链接
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }
    }
}
