using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public class RelatedFieldRepository : IRepository
    {
        private readonly Repository<RelatedField> _repository;

        public RelatedFieldRepository()
        {
            _repository = new Repository<RelatedField>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(RelatedField relatedField)
        {
            return await _repository.InsertAsync(relatedField);
        }

        public async Task<bool> UpdateAsync(RelatedField relatedField)
        {
            return await _repository.UpdateAsync(relatedField);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<RelatedField> GetRelatedFieldAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task<RelatedField> GetRelatedFieldAsync(int siteId, string title)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(RelatedField.SiteId), siteId)
                .Where(nameof(RelatedField.Title), title)
            );
        }

        public async Task<string> GetTitleAsync(int id)
        {
            return await _repository.GetAsync<string>(Q
                .Select(nameof(RelatedField.Title))
                .Where(nameof(RelatedField.Id), id)
            );
        }

        public async Task<IEnumerable<RelatedField>> GetRelatedFieldListAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(RelatedField.SiteId), siteId)
                .OrderBy(nameof(RelatedField.Id)));
        }

        public async Task<IEnumerable<string>> GetTitleListAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(RelatedField.Title))
                .Where(nameof(RelatedField.SiteId), siteId)
                .OrderBy(nameof(RelatedField.Id))
            );
        }

        public async Task<string> GetImportTitleAsync(int siteId, string relatedFieldName)
        {
            string importName;
            if (relatedFieldName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var relatedFieldNameCount = 0;
                var lastName = relatedFieldName.Substring(relatedFieldName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstName = relatedFieldName.Substring(0, relatedFieldName.Length - lastName.Length);
                try
                {
                    relatedFieldNameCount = int.Parse(lastName);
                }
                catch
                {
                    // ignored
                }
                relatedFieldNameCount++;
                importName = firstName + relatedFieldNameCount;
            }
            else
            {
                importName = relatedFieldName + "_1";
            }

            var relatedField = await GetRelatedFieldAsync(siteId, relatedFieldName);
            if (relatedField != null)
            {
                importName = await GetImportTitleAsync(siteId, importName);
            }

            return importName;
        }
	}
}