using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SignedFileInfo Data Structure.
    /// </summary>
    [Serializable]
    public class SignedFileInfo : AopObject
    {
        /// <summary>
        /// 文档过期时间戳
        /// </summary>
        [XmlElement("expired_time")]
        public string ExpiredTime { get; set; }

        /// <summary>
        /// 数据名
        /// </summary>
        [XmlElement("file_name")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型  pdf //pdf文档  p7 //pkcs7签名文档
        /// </summary>
        [XmlElement("file_type")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件读取url地址
        /// </summary>
        [XmlElement("file_url")]
        public string FileUrl { get; set; }

        /// <summary>
        /// 文档创建时间戳
        /// </summary>
        [XmlElement("gmt_time")]
        public string GmtTime { get; set; }

        /// <summary>
        /// 签约数据编号，由平台生成
        /// </summary>
        [XmlElement("inner_data_id")]
        public string InnerDataId { get; set; }

        /// <summary>
        /// 签约数据编号，由外部系统定义，用于数据关联
        /// </summary>
        [XmlElement("out_data_id")]
        public string OutDataId { get; set; }

        /// <summary>
        /// 文档签名结果
        /// </summary>
        [XmlElement("signed_data")]
        public string SignedData { get; set; }

        /// <summary>
        /// 资源文件类型  DATA //文件原文  FILE  //文件OSS索引
        /// </summary>
        [XmlElement("source_type")]
        public string SourceType { get; set; }
    }
}
