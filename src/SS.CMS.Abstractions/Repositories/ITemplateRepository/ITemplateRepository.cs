using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITemplateRepository : IRepository
    {
        Task<int> InsertAsync(TemplateInfo templateInfo, string templateContent, string administratorName);

        Task UpdateAsync(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, string administratorName);

        Task SetDefaultAsync(int siteId, int templateId);

        Task DeleteAsync(int siteId, int templateId);

        string GetImportTemplateName(int siteId, string templateName);

        Dictionary<TemplateType, int> GetCountDictionary(int siteId);

        IList<TemplateInfo> GetTemplateInfoListByType(int siteId, TemplateType type);

        IList<TemplateInfo> GetTemplateInfoListOfFile(int siteId);

        IList<TemplateInfo> GetTemplateInfoListBySiteId(int siteId);

        IList<string> GetTemplateNameList(int siteId, TemplateType templateType);

        IList<string> GetLowerRelatedFileNameList(int siteId, TemplateType templateType);

        Task CreateDefaultTemplateInfoAsync(int siteId, string administratorName);

        Task<string> GetCreatedFileFullNameAsync(int templateId);

        Task<string> GetTemplateNameAsync(int templateId);

        TemplateInfo GetTemplateInfoByTemplateName(int siteId, TemplateType templateType, string templateName);

        TemplateInfo GetDefaultTemplateInfo(int siteId, TemplateType templateType);

        int GetDefaultTemplateId(int siteId, TemplateType templateType);

        int GetTemplateIdByTemplateName(int siteId, TemplateType templateType, string templateName);

        string GetTemplateFilePath(SiteInfo siteInfo, TemplateInfo templateInfo);

        TemplateInfo GetIndexPageTemplateInfo(int siteId);

        Task<TemplateInfo> GetChannelTemplateInfoAsync(int siteId, int channelId);

        Task<TemplateInfo> GetContentTemplateInfoAsync(int siteId, int channelId);

        Task<TemplateInfo> GetFileTemplateInfoAsync(int siteId, int fileTemplateId);

        void WriteContentToTemplateFile(SiteInfo siteInfo, TemplateInfo templateInfo, string content, string administratorName);

        int GetIndexTemplateId(int siteId);

        Task<int> GetChannelTemplateIdAsync(int siteId, int channelId);

        Task<int> GetContentTemplateIdAsync(int siteId, int channelId);

        Task<string> GetTemplateContentAsync(SiteInfo siteInfo, TemplateInfo templateInfo);

        Task<string> GetContentByFilePathAsync(string filePath);

        Task<IEnumerable<int>> GetAllFileTemplateIdListAsync(int siteId);
    }
}
