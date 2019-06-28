using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITemplateRepository : IRepository
    {
        int Insert(TemplateInfo templateInfo, string templateContent, string administratorName);

        void Update(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, string administratorName);

        void SetDefault(int siteId, int id);

        Task DeleteAsync(int siteId, int id);

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
