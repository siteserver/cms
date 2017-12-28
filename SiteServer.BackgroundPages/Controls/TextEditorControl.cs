using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Controls
{
	public class TextEditorControl : Control
	{
        private string _value;
        private PublishmentSystemInfo _publishmentSystemInfo;
        private string _attributeName;

        public void SetParameters(PublishmentSystemInfo publishmentSystemInfo, string attributeName, string value)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            _attributeName = attributeName;
            _value = value;
        }

		protected override void Render(HtmlTextWriter output)
		{
		    if (Page.IsPostBack) return;

		    var pageScripts = new NameValueCollection();

		    var attributes = new ExtendedAttributes();
		    attributes.Set(_attributeName, _value);

		    var extraBuilder = new StringBuilder();
		    var inputHtml = BackgroundInputTypeParser.ParseTextEditor(attributes, _attributeName, _publishmentSystemInfo, pageScripts, extraBuilder);

		    output.Write(inputHtml + extraBuilder);

		    foreach (string key in pageScripts.Keys)
		    {
		        output.Write(pageScripts[key]);
		    }
		}
    }
}
