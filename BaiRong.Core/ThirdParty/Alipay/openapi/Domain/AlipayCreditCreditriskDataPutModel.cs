using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCreditCreditriskDataPutModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCreditCreditriskDataPutModel : AopObject
    {
        /// <summary>
        /// 数据类别,ISV注册成为网商银行的会员，达成数据合作服务，约定数据模型，由网商分配给ISV的数据模型的类别
        /// </summary>
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// 外部机构编码(ISV注册成为网商银行的会员，ISV在网商的会员ID)
        /// </summary>
        [XmlElement("dataorgid")]
        public string Dataorgid { get; set; }

        /// <summary>
        /// 数据提供者,ISV注册成为网商银行的会员，达成数据合作服务，约定数据模型，由网商分配给ISV的机构代号
        /// </summary>
        [XmlElement("dataprovider")]
        public string Dataprovider { get; set; }

        /// <summary>
        /// 实体编码(ISV客户的支付宝数字ID)
        /// </summary>
        [XmlElement("entitycode")]
        public string Entitycode { get; set; }

        /// <summary>
        /// 实体名(ISV客户的支付宝登录号)
        /// </summary>
        [XmlElement("entityname")]
        public string Entityname { get; set; }

        /// <summary>
        /// 实体类型(固定为ALIPAY)
        /// </summary>
        [XmlElement("entitytype")]
        public string Entitytype { get; set; }

        /// <summary>
        /// Json格式,数据内容,ISV注册成为网商银行的会员，达成数据合作服务，约定json串字段和内容,ISV将数据给到网商，网商按照约定解析Json内容
        /// </summary>
        [XmlElement("objectcontent")]
        public string Objectcontent { get; set; }
    }
}
