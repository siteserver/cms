using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Controls
{
	public class TextEditorControl : Control
	{
        private NameValueCollection _formCollection;
        private bool _isEdit;
        private bool _isPostBack;
        private PublishmentSystemInfo _publishmentSystemInfo;
        private string _attributeName;

        public void SetParameters(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isEdit, bool isPostBack)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            _attributeName = attributeName;
            _formCollection = formCollection;
            _isEdit = isEdit;
            _isPostBack = isPostBack;
        }

		protected override void Render(HtmlTextWriter output)
		{
            if (!Page.IsPostBack)
            {
                if (_formCollection == null)
                {
                    if (HttpContext.Current.Request.Form.Count > 0)
                    {
                        _formCollection = HttpContext.Current.Request.Form;
                    }
                    else
                    {
                        _formCollection = new NameValueCollection();
                    }
                }

                var pageScripts = new NameValueCollection();

                var isAddAndNotPostBack = !_isEdit && !_isPostBack;

                var inputHtml = BackgroundInputTypeParser.ParseTextEditor(_publishmentSystemInfo, _attributeName, _formCollection, isAddAndNotPostBack, pageScripts, string.Empty, string.Empty, 0);

                output.Write(inputHtml);

                foreach (string key in pageScripts.Keys)
                {
                    output.Write(pageScripts[key]);
                }
            }
		}
    }
}
