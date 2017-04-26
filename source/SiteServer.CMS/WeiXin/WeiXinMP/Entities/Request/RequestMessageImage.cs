namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.Request
{
    public class RequestMessageImage : RequestMessageBase, IRequestMessageBase
    {
        public override RequestMsgType MsgType
        {
            get { return RequestMsgType.Image; }
        }

        public string MediaId { get; set; }
        public string PicUrl { get; set; }
    }
}
