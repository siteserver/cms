using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageServiceStl : Page
    {
        public const string TypeGetLoadingTemplates = "GetLoadingTemplates";

        public static string GetRedirectUrl(int siteId, string type)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageServiceStl), new NameValueCollection
            {
                {"type", type}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request["type"];
            var retString = string.Empty;

            if (type == TypeGetLoadingTemplates)
            {
                var siteId = TranslateUtils.ToInt(Request["siteID"]);
                var templateType = Request["templateType"];
                retString = GetLoadingTemplates(siteId, templateType);
            }

            Page.Response.Write(retString);
            Page.Response.End();
        }

        public string GetLoadingTemplates(int siteId, string templateType)
        {
            var list = new List<string>();

            var theTemplateType = TemplateTypeUtils.GetEnumType(templateType);

            var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListByType(siteId, theTemplateType);

            foreach (var templateInfo in templateInfoList)
            {
                var templateAddUrl = PageTemplateAdd.GetRedirectUrl(siteId, templateInfo.Id, theTemplateType);
                list.Add($@"
<tr treeitemlevel=""3"">
	<td align=""left"" nowrap="""">
		<img align=""absmiddle"" src=""../assets/icons/tree/empty.gif""><img align=""absmiddle"" src=""../assets/icons/tree/empty.gif""><img align=""absmiddle"" src=""../assets/icons/tree/empty.gif"">&nbsp;<a href=""{templateAddUrl}"" onclick=""fontWeightLink(this)"" target=""management"">{templateInfo.TemplateName}</a>
	</td>
</tr>
");
            }

            var builder = new StringBuilder();
            foreach (string html in list)
            {
                builder.Append(html);
            }
            return builder.ToString();
        }
    }
}
