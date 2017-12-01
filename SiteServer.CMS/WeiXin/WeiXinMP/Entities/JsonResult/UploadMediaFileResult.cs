namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult
{
    /// <summary>
    /// 上传媒体文件返回结果
    /// </summary>
    public class UploadMediaFileResult
    {
        public UploadMediaFileType type { get; set; }
        public string media_id { get; set; }
        public long created_at { get; set; }
    }
}
