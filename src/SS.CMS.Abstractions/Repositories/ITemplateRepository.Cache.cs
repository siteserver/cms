using System.Collections.Generic;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
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

        TemplateInfo GetChannelTemplateInfo(int siteId, int channelId);

        TemplateInfo GetContentTemplateInfo(int siteId, int channelId);

        TemplateInfo GetFileTemplateInfo(int siteId, int fileTemplateId);

        void WriteContentToTemplateFile(SiteInfo siteInfo, TemplateInfo templateInfo, string content, string administratorName);

        int GetIndexTemplateId(int siteId);

        int GetChannelTemplateId(int siteId, int channelId);

        int GetContentTemplateId(int siteId, int channelId);

        string GetTemplateContent(SiteInfo siteInfo, TemplateInfo templateInfo);

        string GetContentByFilePath(string filePath);
    }
}
