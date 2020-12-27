using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Services;

namespace SSCMS.Core.Utils
{
	public class VisualInfo
	{
        public Site Site { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public Template Template { get; private set; }

        public ParseType ContextType { get; private set; }

        public bool IsPreview { get; private set; }

        public string FilePath { get; private set; }

        public int PageIndex { get; private set; }

        public static async Task<VisualInfo> GetInstanceAsync(IPathManager pathManager, IDatabaseManager databaseManager, int siteId, int channelId, int contentId, int fileTemplateId, int pageIndex, bool isPreview = false)
        {
            if (siteId == 0)
            {
                siteId = await pathManager.GetCurrentSiteIdAsync();
            }
            var visualInfo = new VisualInfo
            {
                Site = await databaseManager.SiteRepository.GetAsync(siteId),
                ChannelId = channelId,
                ContentId = contentId,
                Template = null,
                ContextType = ParseType.Undefined,
                IsPreview = false,
                FilePath = string.Empty,
                PageIndex = pageIndex
            };

            if (visualInfo.Site == null) return visualInfo;

            visualInfo.IsPreview = isPreview;

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
                visualInfo.Template = await databaseManager.TemplateRepository.GetIndexPageTemplateAsync(visualInfo.Site.Id);
                visualInfo.ContextType = ParseType.Channel;
                visualInfo.FilePath = await pathManager.GetIndexPageFilePathAsync(visualInfo.Site, visualInfo.Template.CreatedFileFullName, visualInfo.Site.Root, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                var channel = await databaseManager.ChannelRepository.GetAsync(visualInfo.ChannelId);
                visualInfo.Template = await databaseManager.TemplateRepository.GetChannelTemplateAsync(visualInfo.Site.Id, channel);
                visualInfo.ContextType = ParseType.Channel;
                visualInfo.FilePath = await pathManager.GetChannelPageFilePathAsync(visualInfo.Site, visualInfo.ChannelId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                var channel = await databaseManager.ChannelRepository.GetAsync(visualInfo.ChannelId);
                var content =
                    await databaseManager.ContentRepository.GetAsync(visualInfo.Site, channel, visualInfo.ContentId);
                visualInfo.Template = await databaseManager.TemplateRepository.GetContentTemplateAsync(visualInfo.Site.Id, channel, content?.TemplateId ?? 0);
                visualInfo.ContextType = ParseType.Content;
                visualInfo.FilePath = await pathManager.GetContentPageFilePathAsync(visualInfo.Site, visualInfo.ChannelId, visualInfo.ContentId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                visualInfo.Template = await databaseManager.TemplateRepository.GetFileTemplateAsync(visualInfo.Site.Id, fileTemplateId);
                visualInfo.ContextType = ParseType.Undefined;
                visualInfo.FilePath = await pathManager.ParseSitePathAsync(visualInfo.Site, visualInfo.Template.CreatedFileFullName);
            }

            return visualInfo;
        }
	}
}
