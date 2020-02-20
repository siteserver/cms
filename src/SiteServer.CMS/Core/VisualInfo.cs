using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;


namespace SiteServer.CMS.Core
{
	public class VisualInfo
	{
        public Site Site { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public Template Template { get; private set; }

        public ContextType ContextType { get; private set; }

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
                Site = await DataProvider.SiteRepository.GetAsync(siteId),
                ChannelId = channelId,
                ContentId = contentId,
                Template = null,
                ContextType = ContextType.Undefined,
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
                visualInfo.Template = await DataProvider.TemplateRepository.GetIndexPageTemplateAsync(visualInfo.Site.Id);
                visualInfo.ContextType = ContextType.Channel;
                visualInfo.FilePath = await PathUtility.GetIndexPageFilePathAsync(visualInfo.Site, visualInfo.Template.CreatedFileFullName, visualInfo.Site.Root, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(visualInfo.ChannelId);
                visualInfo.Template = await DataProvider.TemplateRepository.GetChannelTemplateAsync(visualInfo.Site.Id, channel);
                visualInfo.ContextType = ContextType.Channel;
                visualInfo.FilePath = await PathUtility.GetChannelPageFilePathAsync(visualInfo.Site, visualInfo.ChannelId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(visualInfo.ChannelId);
                visualInfo.Template = await DataProvider.TemplateRepository.GetContentTemplateAsync(visualInfo.Site.Id, channel);
                visualInfo.ContextType = ContextType.Content;
                visualInfo.FilePath = await PathUtility.GetContentPageFilePathAsync(visualInfo.Site, visualInfo.ChannelId, visualInfo.ContentId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                visualInfo.Template = await DataProvider.TemplateRepository.GetFileTemplateAsync(visualInfo.Site.Id, fileTemplateId);
                visualInfo.ContextType = ContextType.Undefined;
                visualInfo.FilePath = await PathUtility.MapPathAsync(visualInfo.Site, visualInfo.Template.CreatedFileFullName);
            }

            return visualInfo;
        }
	}
}
