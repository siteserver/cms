using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertSpecialAdvContentModifyResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertSpecialAdvContentModifyResponse : AopObject
    {
        /// <summary>
        /// 修改广告内容的结果码；  Success：修改成功；  PASSWORD_RED_EXIST：口令已存在；  ITEM_INVALID：商品无效或者已过期；  CREATE_PASSWORD_MORE_THEN_MAX：口令超过限定最多数量；  ADV_REPEAT_PASSWORD_RED：当前广告已存在口令，不能再次创建；  PASSWORD_RED_INVALID：口令校验失败；  CONTRACT_INVALID：合同已失效，不能创建口令；  NOT_SUPPORT_ERROR：非代金券不支持创建口令；
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 口令红包信息
        /// </summary>
        [XmlElement("content_password")]
        public KbAdvertContentPassword ContentPassword { get; set; }

        /// <summary>
        /// 吱口令结果
        /// </summary>
        [XmlElement("content_share_code")]
        public KbAdvertContentShareCode ContentShareCode { get; set; }

        /// <summary>
        /// 广告内容类型；  当该值是passwordRed时，code的值表示修改口令红包的结果码；
        /// </summary>
        [XmlElement("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// 修改结果描述
        /// </summary>
        [XmlElement("msg")]
        public string Msg { get; set; }
    }
}
