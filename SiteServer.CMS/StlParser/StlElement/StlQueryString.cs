using System.Collections.Generic;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "SQL查询语句", Description = "通过 stl:queryString 标签在模板中定义SQL查询语句")]
    public class StlQueryString
	{
        public const string ElementName = "stl:queryString";

        public static SortedList<string, string> AttributeList => null;
    }
}
