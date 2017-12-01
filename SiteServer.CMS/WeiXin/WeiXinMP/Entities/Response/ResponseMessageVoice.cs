namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.Response
{
    /// <summary>
    /// 需要预先上传多媒体文件到微信服务器，只支持认证服务号。
    /// </summary>
    public class ResponseMessageVoice : ResponseMessageBase, IResponseMessageBase
    {
        new public virtual ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Voice; }
        }

        public Voice Voice { get; set; }

        public ResponseMessageVoice()
        {
            Voice = new Voice();
        }
    }
}
