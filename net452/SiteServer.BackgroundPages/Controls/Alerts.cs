using System.Web.UI;
using System.Web.UI.HtmlControls;
using SiteServer.BackgroundPages.Utils;
using SiteServer.CMS.Fx;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Controls
{
	public class Alerts : HtmlContainerControl
    {
	    public bool IsShowImmidiatary { get; set; }

	    public WebPageUtils.Message.EMessageType MessageType { get; set; } = WebPageUtils.Message.EMessageType.None;

	    public string Content { get; set; } = string.Empty;

	    public string Text
        {
            get
            {
                var state = ViewState["Text"];
                if (state != null)
                {
                    return (string)state;
                }
                return string.Empty;
            }
            set
            {
                ViewState["Text"] = value;
            }
        }

		protected override void Render(HtmlTextWriter writer)
		{
		    writer.Write(IsShowImmidiatary
		        ? WebPageUtils.GetAlertHtml(MessageType, Content, this)
		        : WebPageUtils.GetAlertHtml(this, string.IsNullOrEmpty(Text) ? InnerHtml : Text));

		    writer.Write(@"<div id=""alert"" class=""alert"" style=""display:none""><button type=""button"" class=""close"" data-dismiss=""alert"">&times;</button><strong>提示!</strong>&nbsp;&nbsp; <span id=""alertMessage""></span></div>");
		}
	}
}
