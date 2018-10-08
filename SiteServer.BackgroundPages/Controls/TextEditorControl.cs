using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages.Controls
{
	public class TextEditorControl : Control
	{
        private string _value;
        private SiteInfo _siteInfo;
        private string _attributeName;

        public void SetParameters(SiteInfo siteInfo, string attributeName, string value)
        {
            _siteInfo = siteInfo;
            _attributeName = attributeName;
            _value = value;
        }

		protected override void Render(HtmlTextWriter output)
		{
		    if (Page.IsPostBack) return;

		    var pageScripts = new NameValueCollection();

		    var attributes = new AttributesImpl();
		    attributes.Set(_attributeName, _value);

		    var extraBuilder = new StringBuilder();
		    var inputHtml = BackgroundInputTypeParser.ParseTextEditor(attributes, _attributeName, _siteInfo, pageScripts, extraBuilder);

		    output.Write(inputHtml + extraBuilder);

		    foreach (string key in pageScripts.Keys)
		    {
		        output.Write(pageScripts[key]);
		    }
		}
    }
}
