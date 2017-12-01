using System.Web;
using BaiRong.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
	public class VisualInfo
	{
	    private VisualInfo()
		{
            PublishmentSystemId = ChannelId = ContentId = FileTemplateId = PageIndex = 0;
            TemplateType = ETemplateType.IndexPageTemplate;
            IsPreview = false;
            IncludeUrl = string.Empty;
            FilePath = string.Empty;
		}

        //public static VisualInfo GetInstance()
        //{
        //    var publishmentSystemId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["s"]); ;
        //    var channelId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["n"]); ;
        //    var contentId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["c"]);
        //    var fileTemplateId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["f"]);
        //    var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["p"]);
        //    var previewId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["previewId"]);//编辑界面预览

        //    return GetInstance(publishmentSystemId, channelId, contentId, fileTemplateId, pageIndex, previewId);
        //}

        public static VisualInfo GetInstance(int publishmentSystemId, int channelId, int contentId, int fileTemplateId, int pageIndex, int previewId)
        {
            var visualInfo = new VisualInfo
            {
                PublishmentSystemId = publishmentSystemId,
                ChannelId = channelId,
                ContentId = contentId,
                FileTemplateId = fileTemplateId,
                PageIndex = pageIndex
            };

            if (visualInfo.PublishmentSystemId == 0)
            {
                visualInfo.PublishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
            }

            if (previewId > 0)
            {
                visualInfo.IsPreview = true;
                visualInfo.ContentId = previewId;
            }

            if (visualInfo.ChannelId > 0)
            {
                visualInfo.TemplateType = ETemplateType.ChannelTemplate;
            }
            if (visualInfo.ContentId > 0 || visualInfo.IsPreview)
            {
                visualInfo.TemplateType = ETemplateType.ContentTemplate;
            }
            if (visualInfo.FileTemplateId > 0)
            {
                visualInfo.TemplateType = ETemplateType.FileTemplate;
            }

            if (visualInfo.ChannelId == 0)
            {
                visualInfo.ChannelId = visualInfo.PublishmentSystemId;
            }

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(visualInfo.PublishmentSystemId);
            if (visualInfo.TemplateType == ETemplateType.IndexPageTemplate)
            {
                var templateInfo = TemplateManager.GetIndexPageTemplateInfo(visualInfo.PublishmentSystemId);
                var isHeadquarters = publishmentSystemInfo.IsHeadquarters;
                visualInfo.FilePath = PathUtility.GetIndexPageFilePath(publishmentSystemInfo, templateInfo.CreatedFileFullName, isHeadquarters, visualInfo.PageIndex);
            }
            else if (visualInfo.TemplateType == ETemplateType.ChannelTemplate)
            {
                visualInfo.FilePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, visualInfo.ChannelId, visualInfo.PageIndex);
            }
            else if (visualInfo.TemplateType == ETemplateType.ContentTemplate)
            {
                visualInfo.FilePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, visualInfo.ChannelId, visualInfo.ContentId, visualInfo.PageIndex);
            }
            else if (visualInfo.TemplateType == ETemplateType.FileTemplate)
            {
                var templateInfo = TemplateManager.GetFileTemplateInfo(visualInfo.PublishmentSystemId, visualInfo.FileTemplateId);
                visualInfo.FilePath = PathUtility.MapPath(publishmentSystemInfo, templateInfo.CreatedFileFullName);
            }

            return visualInfo;
        }

        public int PublishmentSystemId { get; private set; }

	    public int ChannelId { get; private set; }

	    public int ContentId { get; private set; }

	    public int FileTemplateId { get; private set; }

	    public ETemplateType TemplateType { get; private set; }

	    public bool IsPreview { get; private set; }

	    public string IncludeUrl { get; }

	    public string FilePath { get; private set; }

	    public int PageIndex { get; private set; }
	}
}
