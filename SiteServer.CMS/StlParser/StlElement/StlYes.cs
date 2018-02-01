using System.Collections.Generic;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "成功模板", Description = "通过 stl:yes 标签在模板中显示成功模板")]
    public sealed class StlYes
    {
        public const string ElementName = "stl:yes";
        public const string ElementName2 = "stl:successtemplate";

        public static SortedList<string, string> AttributeList => null;
    }
}
