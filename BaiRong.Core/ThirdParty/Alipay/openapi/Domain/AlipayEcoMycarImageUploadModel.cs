using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarImageUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarImageUploadModel : AopObject
    {
        /// <summary>
        /// 文件内容(图片字节数组做Base64转换后的字符串)
        /// </summary>
        [XmlElement("img_content")]
        public string ImgContent { get; set; }

        /// <summary>
        /// 图片格式，枚举：png、jpg、gif
        /// </summary>
        [XmlElement("img_type")]
        public string ImgType { get; set; }

        /// <summary>
        /// 场景类型  枚举：  洗车保养小图 ：MAINTAIN_PIC_S  洗车保养大图 :  MAINTAIN_PIC_L  车型 : CAR_TYPE  加油 : OIL  默认: DEFAULT（scene_type 为空时为默认)  根据类型场景校验大小（BASE64之前的大小），超过返回错误。  洗车保养小图最大60K  洗车保养大图最大100K  车型最大1M  加油最大1M  默认 100K
        /// </summary>
        [XmlElement("scene_type")]
        public string SceneType { get; set; }
    }
}
