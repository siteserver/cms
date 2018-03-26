using System.Web.UI;
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

		protected override void Render(HtmlTextWriter writer)
		{
            if (isShowImmidiatary) // 有直接显示的消息
            {
                writer.Write(MessageUtils.GetMessageHtml(messageType, content, this));
            }
            else // 没有直接显示的消息则去cookies中检查是否有消息需要显示
            {
                writer.Write(MessageUtils.GetMessageHtml(this));
            }
		}
	}
}
