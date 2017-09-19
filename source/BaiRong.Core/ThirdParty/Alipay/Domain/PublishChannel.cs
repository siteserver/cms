using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PublishChannel Data Structure.
    /// </summary>
    [Serializable]
    public class PublishChannel : AopObject
    {
        /// <summary>
        /// 当type为MERCHANT_CROWD时，config需填入口令送的密码和图片，样例如下："config":"{\"PASSWORD\":\"口令送密码\",\"BACKGROUND_LOGO\":\"1T8Pp00AT7eo9NoAJkMR3AAAACMAAQEC\"}"
        /// </summary>
        [XmlElement("config")]
        public string Config { get; set; }

        /// <summary>
        /// 扩展信息，无需配置
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 渠道类型，目前支持以下类型  QR_CODE：二维码投放  SHORT_LINK：短连接投放  SHOP_DETAIL：店铺页投放  PAYMENT_RESULT：支付成功页  MERCHANT_CROWD：口令送  URL_WITH_TOKEN：外部发奖活动，只有活动类型为DIRECT_SEND时才支持  EXTERNAL：外部投放，口碑需要感知任何投放内容
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
