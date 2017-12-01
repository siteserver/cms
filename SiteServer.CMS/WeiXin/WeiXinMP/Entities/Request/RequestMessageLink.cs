namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.Request
{
    public class RequestMessageLink : RequestMessageBase, IRequestMessageBase
    {
        public override RequestMsgType MsgType
        {
            get { return RequestMsgType.Link; }
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
