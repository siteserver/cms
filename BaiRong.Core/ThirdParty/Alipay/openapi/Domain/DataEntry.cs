using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DataEntry Data Structure.
    /// </summary>
    [Serializable]
    public class DataEntry : AopObject
    {
        /// <summary>
        /// 数据的发生时间
        /// </summary>
        [XmlElement("biz_time")]
        public string BizTime { get; set; }

        /// <summary>
        /// 数据名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 用于指定数据的类型  数值类型：Long（整型）、Double（浮点型）、Average（取平均，浮点型）；  枚举类型（用于分布表格）：Enum；  比率类型（用于比率类平均）：Rate
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 数据值。内容和type参数对应。  支持一组数据的json表达，比如：[“E1”,"E2"]，[“2”,"1"]。  枚举类支持下面简写方式：[“E1*99”,"E2*35"]，即E1出现99次，E2出现35次。中间用“*”分隔。
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
