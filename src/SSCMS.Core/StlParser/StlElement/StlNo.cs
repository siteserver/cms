using SSCMS.Core.StlParser.Attributes;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "失败模板", Description = "通过 stl:no 标签在模板中显示失败模板")]
    public static class StlNo
    {
        public const string ElementName = "stl:no";
        public const string ElementName2 = "stl:failuretemplate";
    }
}
