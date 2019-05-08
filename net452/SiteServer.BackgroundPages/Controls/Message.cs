using System.Web.UI;
using SiteServer.BackgroundPages.Utils;
using SiteServer.CMS.Fx;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Controls
{
	public class Message : Control
	{
        private bool isShowImmidiatary = false;
        public bool IsShowImmidiatary
        {
            get { return isShowImmidiatary; }
            set { isShowImmidiatary = value; }
        }

        private WebPageUtils.Message.EMessageType messageType = WebPageUtils.Message.EMessageType.None;
        public WebPageUtils.Message.EMessageType MessageType
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

		protected override void Render(HtmlTextWriter writer)
		{
            if (isShowImmidiatary) // 有直接显示的消息
            {
                writer.Write(WebPageUtils.GetMessageHtml(messageType, content, this));
            }
            else // 没有直接显示的消息则去cookies中检查是否有消息需要显示
            {
                writer.Write(WebPageUtils.GetMessageHtml(this));
            }
		}
	}
}
