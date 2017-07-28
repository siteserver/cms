using System.Web.UI;

namespace SiteServer.BackgroundPages.Controls
{
    public class Help : Control
	{
        public string HelpText
        {
            get
            {
                return ViewState["HelpText"] as string;
            }
            set
            {
                ViewState["HelpText"] = value;
            }
        }

        public string Text
        {
            get
            {
                return ViewState["Text"] as string;
            }
            set
            {
                ViewState["Text"] = value;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(Text);
        }
	}
}
