using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public class UserMenuRepository : IRepository
    {
        private readonly Repository<UserMenu> _repository;

        public UserMenuRepository()
        {
            _repository = new Repository<UserMenu>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(UserMenu userMenu)
        {
            userMenu.Id = await _repository.InsertAsync(userMenu);
            UserMenuManager.ClearCache();

            return userMenu.Id;
        }

        public async Task UpdateAsync(UserMenu userMenu)
        {
            await _repository.UpdateAsync(userMenu);

            UserMenuManager.ClearCache();
        }

        public async Task DeleteAsync(int menuId)
        {
            await _repository.DeleteAsync(menuId);
            UserMenuManager.ClearCache();
        }

        public async Task<List<UserMenu>> GetUserMenuListAsync()
        {
            var infoList = await _repository.GetAllAsync();
            var list = infoList.ToList();

            var systemMenus = UserMenuManager.SystemMenus.Value;
            foreach (var kvp in systemMenus)
            {
                var parent = kvp.Key;
                var children = kvp.Value;

                if (list.All(x => x.SystemId != parent.SystemId))
                {
                    parent.Id = await InsertAsync(parent);
                    list.Add(parent);
                }
                else
                {
                    parent = list.First(x => x.SystemId == parent.SystemId);
                }

                if (children != null)
                {
                    foreach (var child in children)
                    {
                        if (list.All(x => x.SystemId != child.SystemId))
                        {
                            child.ParentId = parent.Id;
                            child.Id = await InsertAsync(child);
                            list.Add(child);
                        }
                    }
                }
            }

            return list.OrderBy(userMenu => userMenu.Taxis == 0 ? int.MaxValue : userMenu.Taxis).ToList();
        }
    }
}
