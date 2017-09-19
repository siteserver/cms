using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SubButton Data Structure.
    /// </summary>
    [Serializable]
    public class SubButton : AopObject
    {
        /// <summary>
        /// 当actionType为link时，该参数为url链接；   当actionType为out时，该参数为用户自定义参数；   当actionType为tel时，该参数为电话号码。   当action_type为map时，该参数为查看地图的关键字。   当action_type为consumption时，该参数可不传。   该参数最长255个字符，不允许冒号等特殊字符。
        /// </summary>
        [XmlElement("action_param")]
        public string ActionParam { get; set; }

        /// <summary>
        /// 菜单类型：  out——事件型菜单；  link——链接型菜单；  tel——点击拨打电话；  map——点击查看地图；  consumption——点击查看用户与生活号管理员账号之间的消费记录
        /// </summary>
        [XmlElement("action_type")]
        public string ActionType { get; set; }

        /// <summary>
        /// icon图片url，必须是http协议的url，尺寸为60X60，最大不超过5M，请先调用<a href="https://docs.open.alipay.com/api_3/alipay.offline.material.image.upload"> 图片上传接口</a>获得图片url
        /// </summary>
        [XmlElement("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 菜单名称，icon菜单名称不超过5个汉字，文本菜单名称不超过9个汉字，编码格式为GBK
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
