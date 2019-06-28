using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITemplateRepository
    {
        TemplateInfo GetTemplateInfo(int siteId, int templateId);

        string GetCreatedFileFullName(int siteId, int templateId);

        string GetTemplateName(int siteId, int templateId);

        TemplateInfo GetTemplateInfoByTemplateName(int siteId, TemplateType templateType, string templateName);

        TemplateInfo GetDefaultTemplateInfo(int siteId, TemplateType templateType);

        int GetDefaultTemplateId(int siteId, TemplateType templateType);

        int GetTemplateIdByTemplateName(int siteId, TemplateType templateType, string templateName);

        List<int> GetAllFileTemplateIdList(int siteId);

        List<TemplateInfo> GetTemplateInfoList(int siteId, TemplateType type);

        List<TemplateInfo> GetTemplateInfoList(int siteId, string searchText, string templateTypeString);

        void RemoveCache(int siteId);

        string GetTemplateFilePath(SiteInfo siteInfo, TemplateInfo templateInfo);

        TemplateInfo GetIndexPageTemplateInfo(int siteId);

        Task<TemplateInfo> GetChannelTemplateInfoAsync(int siteId, int channelId);

        Task<TemplateInfo> GetContentTemplateInfoAsync(int siteId, int channelId);

        TemplateInfo GetFileTemplateInfo(int siteId, int fileTemplateId);

        void WriteContentToTemplateFile(SiteInfo siteInfo, TemplateInfo templateInfo, string content, string administratorName);

        int GetIndexTemplateId(int siteId);

        Task<int> GetChannelTemplateIdAsync(int siteId, int channelId);

        Task<int> GetContentTemplateIdAsync(int siteId, int channelId);

        Task<string> GetTemplateContentAsync(SiteInfo siteInfo, TemplateInfo templateInfo);

        Task<string> GetContentByFilePathAsync(string filePath);
    }
}
