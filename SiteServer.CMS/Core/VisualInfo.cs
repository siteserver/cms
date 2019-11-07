using System.Threading.Tasks;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core
{
	public class VisualInfo
	{
        public Site Site { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public TemplateInfo TemplateInfo { get; private set; }

        public EContextType ContextType { get; private set; }

        public bool IsPreview { get; private set; }

        public string FilePath { get; private set; }

        public int PageIndex { get; private set; }

        public static async Task<VisualInfo> GetInstanceAsync(int siteId, int channelId, int contentId, int fileTemplateId, int pageIndex, int previewId)
        {
            if (siteId == 0)
            {
                siteId = await PathUtility.GetCurrentSiteIdAsync();
            }
            var visualInfo = new VisualInfo
            {
                Site = await SiteManager.GetSiteAsync(siteId),
                ChannelId = channelId,
                ContentId = contentId,
                TemplateInfo = null,
                ContextType = EContextType.Undefined,
                IsPreview = false,
                FilePath = string.Empty,
                PageIndex = pageIndex
            };

            if (visualInfo.Site == null) return visualInfo;

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
                visualInfo.ChannelId = visualInfo.Site.Id;
            }

            if (templateType == TemplateType.IndexPageTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetIndexPageTemplateInfo(visualInfo.Site.Id);
                visualInfo.ContextType = EContextType.Channel;
                visualInfo.FilePath = PathUtility.GetIndexPageFilePath(visualInfo.Site, visualInfo.TemplateInfo.CreatedFileFullName, visualInfo.Site.Root, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetChannelTemplateInfo(visualInfo.Site.Id, visualInfo.ChannelId);
                visualInfo.ContextType = EContextType.Channel;
                visualInfo.FilePath = PathUtility.GetChannelPageFilePath(visualInfo.Site, visualInfo.ChannelId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetContentTemplateInfo(visualInfo.Site.Id, visualInfo.ChannelId);
                visualInfo.ContextType = EContextType.Content;
                visualInfo.FilePath = PathUtility.GetContentPageFilePath(visualInfo.Site, visualInfo.ChannelId, visualInfo.ContentId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                visualInfo.TemplateInfo = TemplateManager.GetFileTemplateInfo(visualInfo.Site.Id, fileTemplateId);
                visualInfo.ContextType = EContextType.Undefined;
                visualInfo.FilePath = PathUtility.MapPath(visualInfo.Site, visualInfo.TemplateInfo.CreatedFileFullName);
            }

            return visualInfo;
        }
	}
}
