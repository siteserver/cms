using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Office
{
    public class ExcelObject
    {
        public const string BelongsChannel1 = nameof(BelongsChannel1);
        public const string BelongsChannel2 = nameof(BelongsChannel2);

        private readonly IDatabaseManager _databaseManager;
        private readonly IPathManager _pathManager;

        public ExcelObject(IDatabaseManager databaseManager, IPathManager pathManager)
        {
            _databaseManager = databaseManager;
            _pathManager = pathManager;
        }

        public async Task CreateExcelFileForContentsAsync(string filePath, Site site,
            Channel channel, List<Content> contentInfoList, List<string> columnNames)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Export);

            foreach (var column in columns)
            {
                if (ListUtils.ContainsIgnoreCase(columnNames, column.AttributeName))
                {
                    head.Add(column.DisplayName);
                }
            }

            foreach (var contentInfo in contentInfoList)
            {
                var row = new List<string>();

                foreach (var column in columns)
                {
                    if (ListUtils.ContainsIgnoreCase(columnNames, column.AttributeName))
                    {
                        var value = contentInfo.Get<string>(column.AttributeName);
                        row.Add(StringUtils.StripTags(value));
                    }
                }

                rows.Add(row);
            }

            ExcelUtils.Write(filePath, head, rows);
        }

        public async Task CreateExcelFileForUsersAsync(string filePath, int departmentId = -1)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>
            {
                "用户名",
                "姓名",
                "邮箱",
                "手机",
                "注册时间",
                "最后一次活动时间"
            };
            var rows = new List<List<string>>();

            var userIdList = await _databaseManager.UserRepository.GetUserIdsAsync(departmentId);

            foreach (var userId in userIdList)
            {
                var userInfo = await _databaseManager.UserRepository.GetByUserIdAsync(userId);

                rows.Add(new List<string>
                {
                    userInfo.UserName,
                    userInfo.DisplayName,
                    userInfo.Email,
                    userInfo.Mobile,
                    DateUtils.GetDateAndTimeString(userInfo.CreatedDate),
                    DateUtils.GetDateAndTimeString(userInfo.LastActivityDate)
                });
            }

            ExcelUtils.Write(filePath, head, rows);
        }

        public async Task CreateExcelFileForAdministratorsAsync(string filePath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>
            {
                "用户名",
                "姓名",
                "邮箱",
                "手机",
                "添加时间",
                "最后一次活动时间"
            };
            var rows = new List<List<string>>();

            var userIdList = await _databaseManager.AdministratorRepository.GetUserIdsAsync();

            foreach (var userId in userIdList)
            {
                var administrator = await _databaseManager.AdministratorRepository.GetByUserIdAsync(userId);

                rows.Add(new List<string>
                {
                    administrator.UserName,
                    administrator.DisplayName,
                    administrator.Email,
                    administrator.Mobile,
                    DateUtils.GetDateAndTimeString(administrator.CreatedDate),
                    DateUtils.GetDateAndTimeString(administrator.LastActivityDate)
                });
            }

            ExcelUtils.Write(filePath, head, rows);
        }
    }
}
