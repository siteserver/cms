using SiteServer.Plugin;

namespace SiteServer.CMS.Core
{
	public class VisualInfo
	{
        public int SiteId { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public int FileTemplateId { get; private set; }

        public TemplateType TemplateType { get; private set; }

        public bool IsPreview { get; private set; }

        public string IncludeUrl { get; }

        public string FilePath { get; private set; }

        public int PageIndex { get; private set; }

        private VisualInfo()
		{
            SiteId = ChannelId = ContentId = FileTemplateId = PageIndex = 0;
            TemplateType = TemplateType.IndexPageTemplate;
            IsPreview = false;
            IncludeUrl = string.Empty;
            FilePath = string.Empty;
		}

        //public static VisualInfo GetInstance()
        //{
        //    var siteId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["s"]); ;
        //    var channelId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["n"]); ;
        //    var contentId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["c"]);
        //    var fileTemplateId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["f"]);
        //    var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["p"]);
        //    var previewId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["previewId"]);//编辑界面预览

        //    return GetInstance(siteId, channelId, contentId, fileTemplateId, pageIndex, previewId);
        //}

        public static VisualInfo GetInstance(int siteId, int channelId, int contentId, int fileTemplateId, int pageIndex, int previewId)
        {
            var visualInfo = new VisualInfo
            {
                SiteId = siteId,
                ChannelId = channelId,
                ContentId = contentId,
                FileTemplateId = fileTemplateId,
                PageIndex = pageIndex
            };

            if (visualInfo.SiteId == 0)
            {
                visualInfo.SiteId = PathUtility.GetCurrentSiteId();
            }

            if (previewId > 0)
            {
                visualInfo.IsPreview = true;
                visualInfo.ContentId = previewId;
            }

            if (visualInfo.ChannelId > 0)
            {
                visualInfo.TemplateType = TemplateType.ChannelTemplate;
            }
            if (visualInfo.ContentId > 0 || visualInfo.IsPreview)
            {
                visualInfo.TemplateType = TemplateType.ContentTemplate;
            }
            if (visualInfo.FileTemplateId > 0)
            {
                visualInfo.TemplateType = TemplateType.FileTemplate;
            }

            if (visualInfo.ChannelId == 0)
            {
                visualInfo.ChannelId = visualInfo.SiteId;
            }

            var siteInfo = SiteManager.GetSiteInfo(visualInfo.SiteId);
            if (visualInfo.TemplateType == TemplateType.IndexPageTemplate)
            {
                var templateInfo = TemplateManager.GetIndexPageTemplateInfo(visualInfo.SiteId);
                var isHeadquarters = siteInfo.IsRoot;
                visualInfo.FilePath = PathUtility.GetIndexPageFilePath(siteInfo, templateInfo.CreatedFileFullName, isHeadquarters, visualInfo.PageIndex);
            }
            else if (visualInfo.TemplateType == TemplateType.ChannelTemplate)
            {
                visualInfo.FilePath = PathUtility.GetChannelPageFilePath(siteInfo, visualInfo.ChannelId, visualInfo.PageIndex);
            }
            else if (visualInfo.TemplateType == TemplateType.ContentTemplate)
            {
                visualInfo.FilePath = PathUtility.GetContentPageFilePath(siteInfo, visualInfo.ChannelId, visualInfo.ContentId, visualInfo.PageIndex);
            }
            else if (visualInfo.TemplateType == TemplateType.FileTemplate)
            {
                var templateInfo = TemplateManager.GetFileTemplateInfo(visualInfo.SiteId, visualInfo.FileTemplateId);
                visualInfo.FilePath = PathUtility.MapPath(siteInfo, templateInfo.CreatedFileFullName);
            }

            return visualInfo;
        }
	}
}
