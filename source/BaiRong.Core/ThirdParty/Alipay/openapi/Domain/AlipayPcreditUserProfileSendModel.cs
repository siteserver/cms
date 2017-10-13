using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayPcreditUserProfileSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayPcreditUserProfileSendModel : AopObject
    {
        /// <summary>
        /// 委派采集唯一业务流水号，用户标识回执的委派采集任务，业务方在委派数据采集时提供到商户
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 采集的数据类别，用于标识采集数据类型，商户需要和平台约定，数据类别由平台分配给商户，如： 公积金数据 - HOUSING_FUND 运营商数据 - MOBILE_PHONE_CONTACTS 信用卡账单 - CREDIT_CARD_BILL
        /// </summary>
        [XmlElement("item_key")]
        public string ItemKey { get; set; }

        /// <summary>
        /// 采集业务单号，用于在商户系统唯一标识一次采集任务，由商户系统生成
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 扩展参数，json格式，用于商户回执给业务方，每种数据类别的扩展信息可能不同，具体信息由业务方和商户约定，如无约定，默认可不传
        /// </summary>
        [XmlElement("params")]
        public string Params { get; set; }

        /// <summary>
        /// 数据采集状态，用于标记采集结果，状态值和商户约定，目前支持:  SUCCESS-成功  FAIL-失败
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
