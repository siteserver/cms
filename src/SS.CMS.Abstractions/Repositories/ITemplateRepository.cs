using System.Collections.Generic;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface ITemplateRepository : IRepository
    {
        int Insert(TemplateInfo templateInfo, string templateContent, string administratorName);

        void Update(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, string administratorName);

        void SetDefault(int siteId, int id);

        void Delete(int siteId, int id);

        string GetImportTemplateName(int siteId, string templateName);

        Dictionary<TemplateType, int> GetCountDictionary(int siteId);

        IList<TemplateInfo> GetTemplateInfoListByType(int siteId, TemplateType type);

        IList<TemplateInfo> GetTemplateInfoListOfFile(int siteId);

        IList<TemplateInfo> GetTemplateInfoListBySiteId(int siteId);

        IList<string> GetTemplateNameList(int siteId, TemplateType templateType);

        IList<string> GetLowerRelatedFileNameList(int siteId, TemplateType templateType);

        void CreateDefaultTemplateInfo(int siteId, string administratorName);
    }
}
