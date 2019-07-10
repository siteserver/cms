using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Core.Services
{
    public partial class TableManager
    {
        public async Task CreateContentTableAsync(string tableName, IList<TableColumn> tableColumns)
        {
            var isDbExists = await _database.IsTableExistsAsync(tableName);
            if (isDbExists) return;

            await _database.CreateTableAsync(tableName, tableColumns);
            await _database.CreateIndexAsync(tableName, $"IX_{tableName}", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");
            await _database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis", ContentAttribute.Taxis);

            await _cache.RemoveAsync(_cacheKey);
        }

        public void SyncContentTables()
        {
            // var contentDaoList = ContentRepository.GetContentDaoList();
            // foreach (var contentDao in contentDaoList)
            // {
            //     if (!AppContext.Db.IsTableExists(contentDao.TableName))
            //     {
            //         TableColumnManager.CreateTable(contentDao.TableName, contentDao.TableColumns, string.Empty, true, out _);
            //     }
            //     else
            //     {
            //         TableColumnManager.AlterTable(contentDao.TableName, contentDao.TableColumns, string.Empty, ContentAttribute.DropAttributes.Value);
            //     }
            // }
        }

        public List<TableColumn> ContentTableDefaultColumns
        {
            get
            {
                return _database.GetTableColumns<Content>();
            }
        }
    }
}