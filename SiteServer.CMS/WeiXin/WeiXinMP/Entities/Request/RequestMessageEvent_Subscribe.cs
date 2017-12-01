namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.Request
{
    /// <summary>
    /// 事件之订阅
    /// </summary>
    public class RequestMessageEvent_Subscribe : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.subscribe; }
        }
    }
}
