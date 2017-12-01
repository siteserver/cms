using SiteServer.CMS.StlParser.Model;
using System.Collections.Generic;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "显示模板", Description = "通过 stl:template 标签在模板中定义显示模板")]
    public sealed class StlTemplate
    {
        public const string ElementName = "stl:template";

        public static SortedList<string, string> AttributeList => null;
    }
}
