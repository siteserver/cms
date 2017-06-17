using System.Collections.Generic;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "载入模板", Description = "通过 stl:loading 标签在模板中创建载入中显示的内容")]
    public sealed class StlLoading
    {
        public const string ElementName = "stl:loading";

        public static SortedList<string, string> AttributeList => null;
    }
}
