using System.Collections.Generic;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "添加代码", Description = "通过 stl:code 标签在页面中添加html代码，相同Key的代码将显示最后添加的值")]
    public class StlCode
	{
        private StlCode() { }
		public const string ElementName = "stl:code";

		public const string AttributeType = "type";
        public const string AttributeKey = "key";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("代码放置的位置", TypeList)},
            {AttributeKey, "代码键"}
        };

        public const string TypeHead = "Head";
        public const string TypeBody = "Body";
        public const string TypeFoot = "Foot";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeHead, "头部（</head>之前）"},
            {TypeBody, "主体（<body>之后）"},
            {TypeFoot, "底部（</html>之后）"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var type = string.Empty;
            var key = string.Empty;

		    foreach (var name in contextInfo.Attributes.Keys)
		    {
		        var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeKey))
                {
                    key = value;
                }
            }

		    if (!string.IsNullOrEmpty(key))
		    {
                var innerBuilder = new StringBuilder(contextInfo.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                var code = innerBuilder.ToString();

                if (StringUtils.EqualsIgnoreCase(type, TypeHead))
                {
                    pageInfo.HeadCodes[key] = code;
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeFoot))
                {
                    pageInfo.FootCodes[key] = code;
                }
                else
                {
                    pageInfo.BodyCodes[key] = code;
                }
            }

		    return string.Empty;
		}
	}
}
