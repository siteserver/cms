using System.Web.UI;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Controls
{
	public class Alerts : Control
	{
	    public bool IsShowImmidiatary { get; set; }

	    public MessageUtils.Message.EMessageType MessageType { get; set; } = MessageUtils.Message.EMessageType.None;

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
		        ? MessageUtils.GetAlertHtml(MessageType, Content, this)
		        : MessageUtils.GetAlertHtml(this, Text));

		    writer.Write(@"<div id=""alert"" class=""alert"" style=""display:none""><button type=""button"" class=""close"" data-dismiss=""alert"">&times;</button><strong>提示!</strong>&nbsp;&nbsp; <span id=""alertMessage""></span></div>");
		}
	}
}
