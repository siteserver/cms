namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.Request
{
    /// <summary>
    /// 事件之取消订阅
    /// </summary>
    public class RequestMessageEvent_Scan : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.scan; }
        }

        public string Ticket { get; set; }
    }
}
