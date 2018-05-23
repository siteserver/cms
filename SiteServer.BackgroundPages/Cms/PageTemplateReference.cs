using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateReference : BasePageCms
    {
        public Literal LtlAll;
        public PlaceHolder PhRefenreces;
        public Literal LtlReferences;

        public static string GetRedirectUrl(int siteId, string elementName)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateReference), new NameValueCollection
            {
                {"elementName", elementName}
            });
        }

        private string _elementName = string.Empty;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _elementName = AuthRequest.GetQueryString("elementName");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

            var elements = StlAll.Elements;
            var allBuilder = new StringBuilder();
            foreach (var elementName in elements.Keys)
            {
                allBuilder.Append($@"<li class=""nav-item {(elementName == _elementName ? "active" : string.Empty)}"">
              <a href=""{GetRedirectUrl(SiteId, elementName)}"" class=""nav-link"">
                {elementName}
              </a>
            </li>");
            }
            LtlAll.Text = allBuilder.ToString();

            if (!string.IsNullOrEmpty(_elementName))
            {
                Type elementType;
                if (elements.TryGetValue(_elementName, out elementType))
                {
                    PhRefenreces.Visible = true;

                    var attrBuilder = new StringBuilder();

                    var fields = elementType.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        var attr = field.GetValue(null) as Attr;
                        if (attr != null)
                        {
                            attrBuilder.Append($@"<tr>
                          <td>{attr.Name}</td>
                          <td>{AttrUtils.GetAttrTypeText(attr.Type, attr.GetEnums(elementType, SiteId))}</td>
                          <td>{attr.Description}</td>
                        </tr>");
                        }
                    }

                    var helpUrl = StringUtils.Constants.GetStlUrl(false, _elementName);

                    var stlAttribute = (StlClassAttribute)Attribute.GetCustomAttribute(elementType, typeof(StlClassAttribute));

                    LtlReferences.Text = $@"
<div class=""tab-pane"" style=""display: block;"">
    <h4 class=""m-t-0 header-title"">
    &lt;{_elementName}&gt; {stlAttribute.Usage}
    </h4>
    <p>
    {stlAttribute.Description}
    <a href=""{helpUrl}"" target=""_blank"">详细使用说明</a>
    </p>
    <div class=""panel panel-default m-t-10"">
    <div class=""panel-body p-0"">
        <div class=""table-responsive"">
        <table class=""table tablesaw table-striped m-0"">
            <thead>
            <tr>
                <th>属性</th>
                <th>类型</th>
                <th>说明</th>
            </tr>
            </thead>
            <tbody>
            {attrBuilder}
            </tbody>
        </table>
        </div>
    </div>
    </div>
</div>
";
                }
            }
        }
	}
}
