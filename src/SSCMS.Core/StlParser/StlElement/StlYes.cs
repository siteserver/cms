using SSCMS.Core.StlParser.Attributes;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "成功模板", Description = "通过 stl:yes 标签在模板中显示成功模板")]
    public static class StlYes
    {
        public const string ElementName = "stl:yes";
        public const string ElementName2 = "stl:successtemplate";
    }
}
