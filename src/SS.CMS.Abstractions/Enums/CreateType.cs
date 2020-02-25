using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreateType
    {
        [DataEnum(DisplayName = "��ҳ")]
        Index,
        [DataEnum(DisplayName = "��Ŀҳ")]
        Channel,
        [DataEnum(DisplayName = "����ҳ")]
        Content,
        [DataEnum(DisplayName = "�ļ�ҳ")]
        File,
        [DataEnum(DisplayName = "ר��ҳ")]
        Special,
        [DataEnum(DisplayName = "��Ŀ����������ҳ")]
        AllContent,
        [DataEnum(DisplayName = "����ҳ��")]
        All
    }
}
