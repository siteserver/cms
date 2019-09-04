using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Common
{
    public class VisualInfo
    {
        public Site SiteInfo { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public Template TemplateInfo { get; private set; }

        public EContextType ContextType { get; private set; }

        public bool IsPreview { get; private set; }

        public string FilePath { get; private set; }

        public int PageIndex { get; private set; }

        public static async Task<VisualInfo> GetInstanceAsync(int siteId, int channelId, int contentId, int fileTemplateId, int pageIndex, int previewId, IPathManager pathManager, ISiteRepository siteRepository, ITemplateRepository templateRepository)
        {
            var visualInfo = new VisualInfo
            {
                SiteInfo = await siteRepository.GetSiteAsync(siteId),
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
                visualInfo.TemplateInfo = await templateRepository.GetIndexPageTemplateInfoAsync(visualInfo.SiteInfo.Id);
                visualInfo.ContextType = EContextType.Channel;
                visualInfo.FilePath = pathManager.GetIndexPageFilePath(visualInfo.SiteInfo, visualInfo.TemplateInfo.CreatedFileFullName, visualInfo.SiteInfo.IsRoot, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                visualInfo.TemplateInfo = await templateRepository.GetChannelTemplateInfoAsync(visualInfo.SiteInfo.Id, visualInfo.ChannelId);
                visualInfo.ContextType = EContextType.Channel;
                visualInfo.FilePath = await pathManager.GetChannelPageFilePathAsync(visualInfo.SiteInfo, visualInfo.ChannelId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                visualInfo.TemplateInfo = await templateRepository.GetContentTemplateInfoAsync(visualInfo.SiteInfo.Id, visualInfo.ChannelId);
                visualInfo.ContextType = EContextType.Content;
                visualInfo.FilePath = await pathManager.GetContentPageFilePathAsync(visualInfo.SiteInfo, visualInfo.ChannelId, visualInfo.ContentId, visualInfo.PageIndex);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                visualInfo.TemplateInfo = await templateRepository.GetFileTemplateInfoAsync(visualInfo.SiteInfo.Id, fileTemplateId);
                visualInfo.ContextType = EContextType.Undefined;
                visualInfo.FilePath = pathManager.MapPath(visualInfo.SiteInfo, visualInfo.TemplateInfo.CreatedFileFullName);
            }

            return visualInfo;
        }
    }
}
