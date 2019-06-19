using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
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