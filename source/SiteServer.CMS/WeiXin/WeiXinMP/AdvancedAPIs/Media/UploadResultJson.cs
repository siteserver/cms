using SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;

namespace SiteServer.CMS.WeiXin.WeiXinMP.AdvancedAPIs.Media
{
    public class UploadResultJson : WxJsonResult
    {
       public UploadMediaFileType type { get; set; }
       public string media_id { get; set; }
       public long created_at { get; set; }
    }
}
