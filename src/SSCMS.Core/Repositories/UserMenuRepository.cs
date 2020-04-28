using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class UserMenuRepository : IUserMenuRepository
    {
        private readonly Repository<UserMenu> _repository;

        public UserMenuRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<UserMenu>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey => CacheUtils.GetListKey(_repository.TableName);

        public async Task<int> InsertAsync(UserMenu userMenu)
        {
            return await _repository.InsertAsync(userMenu, Q.CachingRemove(CacheKey));
        }

        public async Task UpdateAsync(UserMenu userMenu)
        {
            await _repository.UpdateAsync(userMenu, Q.CachingRemove(CacheKey));
        }

        public async Task DeleteAsync(int menuId)
        {
            await _repository.DeleteAsync(menuId, Q.CachingRemove(CacheKey));
        }

        public async Task<List<UserMenu>> GetUserMenusAsync()
        {
            var infoList = await _repository.GetAllAsync(Q.CachingGet(CacheKey));
            var list = infoList.ToList();

            return list.OrderBy(userMenu => userMenu.Taxis == 0 ? int.MaxValue : userMenu.Taxis).ToList();
        }

        public async Task<UserMenu> GetAsync(int id)
        {
            var infoList = await _repository.GetAllAsync(Q.CachingGet(CacheKey));
            return infoList.FirstOrDefault(x => x.Id == id);
        }

        public async Task ResetAsync()
        {
            await _repository.DeleteAsync();

            var parentId = await InsertAsync(new UserMenu
            {
                Text = "用户中心"
            });

            await InsertAsync(new UserMenu
            {
                ParentId = parentId,
                Text = "修改资料",
                IconClass = "fa fa-home",
                Link = "/home/profile/"
            });
            await InsertAsync(new UserMenu
            {
                ParentId = parentId,
                Text = "更改密码",
                IconClass = "fa fa-home",
                Link = "/home/password/"
            });
            await InsertAsync(new UserMenu
            {
                ParentId = parentId,
                Text = "退出系统",
                IconClass = "fa fa-home",
                Link = "/home/logout/"
            });

            parentId = await InsertAsync(new UserMenu
            {
                Text = "投稿中心"
            });

            await InsertAsync(new UserMenu
            {
                ParentId = parentId,
                Text = "新增稿件",
                IconClass = "fa fa-plus",
                Link = "/home/contentAdd/"
            });
            await InsertAsync(new UserMenu
            {
                ParentId = parentId,
                Text = "稿件管理",
                IconClass = "fa fa-list",
                Link = "/home/contents/"
            });
        }
    }
}
