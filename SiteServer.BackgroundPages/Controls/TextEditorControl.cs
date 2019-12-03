using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Controls
{
	public class TextEditorControl : Control
	{
        private string _value;
        private Site _site;
        private string _attributeName;

        public void SetParameters(Site site, string attributeName, string value)
        {
            _site = site;
            _attributeName = attributeName;
            _value = value;
        }

		protected override void Render(HtmlTextWriter output)
		{
		    if (Page.IsPostBack) return;

		    var pageScripts = new NameValueCollection();

		    var attributes = new Dictionary<string, object>();
		    attributes[_attributeName] = _value;

		    var extraBuilder = new StringBuilder();
		    var inputHtml = BackgroundInputTypeParser.ParseTextEditor(attributes, _attributeName, _site, pageScripts, extraBuilder);

		    output.Write(inputHtml + extraBuilder);

		    foreach (string key in pageScripts.Keys)
		    {
		        output.Write(pageScripts[key]);
		    }
		}
    }
}
