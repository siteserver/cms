using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InfoSecHitDetectItem Data Structure.
    /// </summary>
    [Serializable]
    public class InfoSecHitDetectItem : AopObject
    {
        /// <summary>
        /// 级别
        /// </summary>
        [XmlElement("detect_resource_level")]
        public string DetectResourceLevel { get; set; }

        /// <summary>
        /// RULEORMODEL("RULEORMODEL", "规则或模型"),    KEYWORDS("KEYWORDS", "关键字检测 "),    REPEAT_MODEL("REPEAT_MODEL", "防重复模型"),    REGEX("regex", "正则表达式"),    URL("url", "URL检测"),    SEXY_PIC("sexyPic", "黄图检测"),    SAMPLE_PIC("samplePic", "样图检测"),    OCR("ocr", "图文识别"),    PICTURE_FACE("picture_face","图片人脸检测"),    QRCODE("QRCode", "二维码检测"),    MDP_MODEL("mdpModel", "mdp检测"),    ANTI_SPAM_MODEL("anti_spam_model", "反垃圾模型");
        /// </summary>
        [XmlElement("detect_type_code")]
        public string DetectTypeCode { get; set; }

        /// <summary>
        /// 保存被命中的内容： 如正则表达式，则保存被正则表达式命中的内容
        /// </summary>
        [XmlElement("hit_content")]
        public string HitContent { get; set; }

        /// <summary>
        /// 命中的检测项的资源： 如命中关键字，则存关键字，如命中正则表达式，则保存正则表达式
        /// </summary>
        [XmlElement("hit_detect_resource")]
        public string HitDetectResource { get; set; }
    }
}
