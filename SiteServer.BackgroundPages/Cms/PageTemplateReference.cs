using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Utils;

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

            VerifySitePermissions(ConfigManager.SitePermissions.TemplatesReference);

            var elements = StlAll.Elements;
            var allBuilder = new StringBuilder();
            foreach (var elementName in elements.Keys)
            {
                if (!elements.TryGetValue(elementName, out var elementType)) continue;

                var tagName = elementName.Substring(4);
                var stlAttribute = (StlElementAttribute)Attribute.GetCustomAttribute(elementType, typeof(StlElementAttribute));
                var tagNameUrl = CloudUtils.Root.GetDocsStlUrl(tagName);

                allBuilder.Append($@"
<tr class=""{(elementName == _elementName ? "bg-secondary text-white" : string.Empty)}"">
  <td>
    <a href=""{GetRedirectUrl(SiteId, elementName)}"" class=""{(elementName == _elementName ? "text-white" : string.Empty)}"">
     {elementName}
    </a>
  </td>
  <td>{stlAttribute.Title}</td>
  <td><a href=""{tagNameUrl}"" target=""_blank"" class=""{(elementName == _elementName ? "text-white" : string.Empty)}"">{tagNameUrl}</a></td>
</tr>");
            }

            LtlAll.Text = $@"
<div class=""panel panel-default m-t-10"">
    <div class=""panel-body p-0"">
        <div class=""table-responsive"">
        <table class=""table tablesaw table-striped m-0"">
            <thead>
            <tr>
                <th>标签</th>
                <th>说明</th>
                <th>参考</th>
            </tr>
            </thead>
            <tbody>
            {allBuilder}
            </tbody>
        </table>
        </div>
    </div>
</div>
";

            if (!string.IsNullOrEmpty(_elementName))
            {
                if (elements.TryGetValue(_elementName, out var elementType))
                {
                    var tagName = _elementName.Substring(4);
                    PhRefenreces.Visible = true;

                    var attrBuilder = new StringBuilder();

                    var fields = elementType.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        var fieldName = field.Name.ToCamelCase();
                        var attr = (StlAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(StlAttributeAttribute));

                        if (attr != null)
                        {
                            var attrUrl = CloudUtils.Root.GetDocsStlUrl(tagName, fieldName, attr.Title);
                            attrBuilder.Append($@"
<tr>
  <td>{fieldName}</td>
  <td>{attr.Title}</td>
  <td><a href=""{attrUrl}"" target=""_blank"">{attrUrl}</a></td>
</tr>");
                        }
                    }

                    var tagNameUrl = CloudUtils.Root.GetDocsStlUrl(tagName);

                    var stlAttribute = (StlElementAttribute)Attribute.GetCustomAttribute(elementType, typeof(StlElementAttribute));

                    LtlReferences.Text = $@"
<div class=""tab-pane"" style=""display: block;"">
    <h4 class=""m-t-0 header-title"">
    &lt;{_elementName}&gt; {stlAttribute.Title}
    </h4>
    <p>
    {stlAttribute.Description}
    <a href=""{tagNameUrl}"" target=""_blank"">详细使用说明</a>
    </p>
    <div class=""panel panel-default m-t-10"">
        <div class=""panel-body p-0"">
            <div class=""table-responsive"">
            <table class=""table tablesaw table-striped m-0"">
                <thead>
                <tr>
                    <th>属性</th>
                    <th>说明</th>
                    <th>参考</th>
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