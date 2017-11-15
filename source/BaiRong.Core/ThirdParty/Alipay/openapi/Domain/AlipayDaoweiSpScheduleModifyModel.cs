using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDaoweiSpScheduleModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDaoweiSpScheduleModifyModel : AopObject
    {
        /// <summary>
        /// 服务者的可用时间表。其中Duration和Unit配合使用，例如duration=30，unit=MIN表示将一天分为以30分钟一小段的时间片段。Unit：目前支持MIN（分钟）。Date：YYYY-MM-DD格式。Bitmap：根据定义的间隔长度跟单位，将date的时间切分，例如将2016-11-29整天按30分钟为一段切分为48段： 111111111111111111111111111111111110000011111111 ， 其中0表示不可用，1表示可用，如果工作日全天可用则每个分段都为1
        /// </summary>
        [XmlElement("calendar_schedule")]
        public CalendarScheduleInfo CalendarSchedule { get; set; }

        /// <summary>
        /// 商家服务者id，由商家维护的该商家下某个服务者的唯一标识，仅支持数字、字母和下划线的组合
        /// </summary>
        [XmlElement("out_sp_id")]
        public string OutSpId { get; set; }
    }
}
