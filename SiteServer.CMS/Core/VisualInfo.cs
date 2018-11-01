using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core
{
	public class VisualInfo
	{
        public SiteInfo SiteInfo { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public TemplateInfo TemplateInfo { get; private set; }

        public EContextType ContextType { get; private set; }

        public bool IsPreview { get; private set; }

        public string FilePath { get; private set; }

        public int PageIndex { get; private set; }

        public static VisualInfo GetInstance(int siteId, int channelId, int contentId, int fileTemplateId, int pageIndex, int previewId)
        {
            if (siteId == 0)
            {
                siteId = PathUtility.GetCurrentSiteId();
            }
            var visualInfo = new VisualInfo
            {
                SiteInfo = SiteManager.GetSiteInfo(siteId),
                ChannelId = channelId,
                ContentId = contentId,
                TemplateInfo = null,
                ContextType = EContextType.Undefined,
                IsPreview = false,
                FilePath = string.Empty,
                PageIndex = pageIndex
            };

            if (visualInfo.SiteInfo == null) return visualInfo;

            if (previewId > 0)
            {
                visualInfo.IsPreview = true;
                visualInfo.ContentId = previewId;
            }

            TemplateType templateType;

            if (visualInfo.ContentId > 0 || visualInfo.IsPreview)
            {
                templateType = TemplateType.ContentTemplate;
            }
            else if (fileTemplateId > 0)
            {
                templateType = TemplateType.FileTemplate;
            }
            else if (visualInfo.ChannelId > 0)
            {
                templateType = TemplateType.ChannelTemplate;
            }
            else
            {
                templateType = TemplateType.IndexPageTemplate;
            }

            if (visualInfo.ChannelId == 0)
            {
                visualInfo.ChannelId = visualInfo.SiteInfo.Id;
            }

            if (templateType == TemplateType.IndexPageTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetIndexPageTemplateInfo(visualInfo.SiteInfo.Id);
                visualInfo.ContextType = EContextType.Channel;
                visualInfo.FilePath = PathUtility.GetIndexPageFilePath(visualInfo.SiteInfo, visualInfo.TemplateInfo.CreatedFileFullName, visualInfo.SiteInfo.IsRoot, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetChannelTemplateInfo(visualInfo.SiteInfo.Id, visualInfo.ChannelId);
                visualInfo.ContextType = EContextType.Channel;
                visualInfo.FilePath = PathUtility.GetChannelPageFilePath(visualInfo.SiteInfo, visualInfo.ChannelId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetContentTemplateInfo(visualInfo.SiteInfo.Id, visualInfo.ChannelId);
                visualInfo.ContextType = EContextType.Content;
                visualInfo.FilePath = PathUtility.GetContentPageFilePath(visualInfo.SiteInfo, visualInfo.ChannelId, visualInfo.ContentId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetFileTemplateInfo(visualInfo.SiteInfo.Id, fileTemplateId);
                visualInfo.ContextType = EContextType.Undefined;
                visualInfo.FilePath = PathUtility.MapPath(visualInfo.SiteInfo, visualInfo.TemplateInfo.CreatedFileFullName);
            }

            return visualInfo;
        }
	}
}
