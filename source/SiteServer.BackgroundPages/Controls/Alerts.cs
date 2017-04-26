using System.Web.UI;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Controls
{
	public class Alerts : Control
	{
        private bool isShowImmidiatary = false;
        public bool IsShowImmidiatary
        {
            get { return isShowImmidiatary; }
            set { isShowImmidiatary = value; }
        }

        private MessageUtils.Message.EMessageType messageType = MessageUtils.Message.EMessageType.None;
        public MessageUtils.Message.EMessageType MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        private string content = string.Empty;
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

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
            if (isShowImmidiatary)
            {
                writer.Write(MessageUtils.GetAlertHtml(messageType, content, this));
            }
            else
            {
                writer.Write(MessageUtils.GetAlertHtml(this, Text));
            }
            writer.Write(@"<div id=""alert"" class=""alert"" style=""display:none""><button type=""button"" class=""close"" data-dismiss=""alert"">&times;</button><strong>ÌבÊ¾!</strong>&nbsp;&nbsp; <span id=""alertMessage""></span></div>");
		}
	}
}
