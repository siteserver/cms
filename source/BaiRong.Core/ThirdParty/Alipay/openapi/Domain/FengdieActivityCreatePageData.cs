using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// FengdieActivityCreatePageData Data Structure.
    /// </summary>
    [Serializable]
    public class FengdieActivityCreatePageData : AopObject
    {
        /// <summary>
        /// H5应用中页面名称。指定凤蝶开发工具项目中某个H5应用的页面名称。
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 指定name页面默认展示的数据。由Schema文件中的路径和展示的数据结构组成，默认模板中Schema文件路径：bgImage/bgImage；默认模板中此参数的数据结构请参考：默认模板-project-components-bglmage-bjlmage.json文件，bjlmage.json文件中的内容可以编辑。注意：展示的数据结构需要和Schema文件中的路径一致。
        /// </summary>
        [XmlElement("schema_data")]
        public string SchemaData { get; set; }
    }
}
