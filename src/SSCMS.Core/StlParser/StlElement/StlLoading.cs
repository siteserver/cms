using SSCMS.Core.StlParser.Attributes;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "载入模板", Description = "通过 stl:loading 标签在模板中创建载入中显示的内容")]
    public static class StlLoading
    {
        public const string ElementName = "stl:loading";
    }
}
