namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.Response
{
    public class ResponseMessageMusic : ResponseMessageBase, IResponseMessageBase
    {
        public override ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Music; }
        }

        public Music Music { get; set; }

        public ResponseMessageMusic()
        {
            Music = new Music();
        }
    }
}
