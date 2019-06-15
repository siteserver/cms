using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IRelatedFieldRepository : IRepository
    {
        int Insert(RelatedFieldInfo relatedFieldInfo);

        void Update(RelatedFieldInfo relatedFieldInfo);

        void Delete(int id);

        RelatedFieldInfo GetRelatedFieldInfo(int id);

        RelatedFieldInfo GetRelatedFieldInfo(int siteId, string title);

        string GetTitle(int id);

        IList<RelatedFieldInfo> GetRelatedFieldInfoList(int siteId);

        IList<string> GetTitleList(int siteId);

        string GetImportTitle(int siteId, string relatedFieldName);
    }
}