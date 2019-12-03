using System.Web.UI;
using SiteServer.CMS.Context;

namespace SiteServer.BackgroundPages.Controls
{
	public class Style : LiteralControl
	{
        public virtual string Href 
		{
            get
            {
                var href = ViewState["Href"] as string;
                return !string.IsNullOrEmpty(href) ? href : string.Empty;
            }
            set
            {
                ViewState["Href"] = value;
            }
        }

		protected override void Render(HtmlTextWriter writer)
		{
		    if (!string.IsNullOrEmpty(Href))
		    {
                writer.Write($@"<link rel=""stylesheet"" href=""{(Href.StartsWith("~") ? PageUtils.ParseNavigationUrl(Href) : PageUtils.GetAdminUrl(Href))}"" type=""text/css"" />");
            }
		}
	}
}
