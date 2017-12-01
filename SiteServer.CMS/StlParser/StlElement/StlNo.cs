using SiteServer.CMS.StlParser.Model;
using System.Collections.Generic;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "失败模板", Description = "通过 stl:no 标签在模板中显示失败模板")]
    public sealed class StlNo
    {
        public const string ElementName = "stl:no";
        public const string ElementName2 = "stl:failuretemplate";

        public static SortedList<string, string> AttributeList => null;
    }
}
