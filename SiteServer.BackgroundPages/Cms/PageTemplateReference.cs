using System;
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
        public Literal LtlElements;
        public Literal LtlAttributes;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

            // var elementsDictionary = StlAll.StlElements.GetElementsNameDictionary();
            var elements = StlAll.Elements;

            var liBuilder = new StringBuilder();
            var contentBuilder = new StringBuilder();
            foreach (var elementName in elements.Keys)
            {
                var elementType = elements[elementName];

                var htmlId = elementName.Replace(":", "-");
                liBuilder.Append($@"<li class=""nav-item"">
              <a href=""#tab-content"" data-attr=""{htmlId}"" class=""nav-link"">
                {elementName}
              </a>
            </li>");

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

                var helpUrl = StringUtils.Constants.GetStlUrl(false, elementName);

                var stlAttribute = (StlClassAttribute) Attribute.GetCustomAttribute(elementType, typeof(StlClassAttribute));

                contentBuilder.Append($@"<div class=""tab-pane show"" id=""{htmlId}"">
              <h4 class=""m-t-0 header-title"">
                &lt;{elementName}&gt; {stlAttribute.Usage}
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
                          <th>取值</th>
                          <th>简介</th>
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
");
            }
            LtlElements.Text = liBuilder.ToString();
            LtlAttributes.Text = contentBuilder.ToString();
        }
	}
}
