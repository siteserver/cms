using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace SiteServer.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreateType
    {
        [Display(Name = "栏目页")]
        Channel,
        [Display(Name = "内容页")]
        Content,
        [Display(Name = "文件页")]
        File,
        [Display(Name = "专题页")]
        Special,
        [Display(Name = "栏目下所有内容页")]
        AllContent
    }
}
