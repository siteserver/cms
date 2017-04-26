using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageServiceStl : Page
    {
        public const string TypeGetLoadingTemplates = "GetLoadingTemplates";

        public static string GetRedirectUrl(string type)
        {
            return PageUtils.GetCmsUrl(nameof(PageServiceStl), new NameValueCollection
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
                var publishmentSystemId = TranslateUtils.ToInt(Request["publishmentSystemID"]);
                var templateType = Request["templateType"];
                retString = GetLoadingTemplates(publishmentSystemId, templateType);
            }

            Page.Response.Write(retString);
            Page.Response.End();
        }

        public string GetLoadingTemplates(int publishmentSystemId, string templateType)
        {
            var arraylist = new ArrayList();

            var eTemplateType = ETemplateTypeUtils.GetEnumType(templateType);

            var templateInfoArrayList = DataProvider.TemplateDao.GetTemplateInfoArrayListByType(publishmentSystemId, eTemplateType);

            foreach (TemplateInfo templateInfo in templateInfoArrayList)
            {
                var templateAddUrl = PageTemplateAdd.GetRedirectUrl(publishmentSystemId, templateInfo.TemplateId, eTemplateType);
                arraylist.Add($@"
<tr treeitemlevel=""3"">
	<td align=""left"" nowrap="""">
		<img align=""absmiddle"" src=""../assets/icons/tree/empty.gif""><img align=""absmiddle"" src=""../assets/icons/tree/empty.gif""><img align=""absmiddle"" src=""../assets/icons/menu/template.gif"">&nbsp;<a href=""{templateAddUrl}"" onclick=""fontWeightLink(this)"" target=""management"">{templateInfo.TemplateName}</a>
	</td>
</tr>
");
            }

            var builder = new StringBuilder();
            foreach (string html in arraylist)
            {
                builder.Append(html);
            }
            return builder.ToString();
        }
    }
}
