using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Office
{
    public class ExcelObject
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IPathManager _pathManager;

        public ExcelObject(IDatabaseManager databaseManager, IOldPluginManager pluginManager, IPathManager pathManager)
        {
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _pathManager = pathManager;
        }

        public async Task CreateExcelFileForContentsAsync(string filePath, Site site,
            Channel channel, IEnumerable<int> contentIdList, List<string> displayAttributes, bool isPeriods, string startDate,
            string endDate, bool? checkedState)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var tableName = _databaseManager.ChannelRepository.GetTableName(site, channel);
            var styleList = ColumnsManager.GetContentListStyles(await _databaseManager.TableStyleRepository.GetContentStylesAsync(channel, tableName));

            foreach (var style in styleList)
            {
                if (displayAttributes.Contains(style.AttributeName))
                {
                    head.Add(style.DisplayName);
                }
            }

            if (contentIdList == null)
            {
                contentIdList = await _databaseManager.ContentRepository.GetContentIdsAsync(site, channel, isPeriods,
                    startDate, endDate, checkedState);
            }

            foreach (var contentId in contentIdList)
            {
                var contentInfo = await _databaseManager.ContentRepository.GetAsync(site, channel, contentId);
                if (contentInfo != null)
                {
                    var row = new List<string>();

                    foreach (var style in styleList)
                    {
                        if (displayAttributes.Contains(style.AttributeName))
                        {
                            var value = contentInfo.Get<string>(style.AttributeName);
                            row.Add(StringUtils.StripTags(value));
                        }
                    }

                    rows.Add(row);
                }
            }

            CsvUtils.Export(filePath, head, rows);
        }

        public async Task CreateExcelFileForContentsAsync(string filePath, Site site,
            Channel channel, List<Content> contentInfoList, List<string> columnNames)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>();
            var rows = new List<List<string>>();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

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

            CsvUtils.Export(filePath, head, rows);
        }

        public async Task CreateExcelFileForUsersAsync(string filePath, bool? checkedState)
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

            List<int> userIdList;
            if (checkedState.HasValue)
            {
                userIdList = (await _databaseManager.UserRepository.GetUserIdsAsync(checkedState.Value)).ToList();
            }
            else
            {
                userIdList = (await _databaseManager.UserRepository.GetUserIdsAsync(true)).ToList();
                userIdList.AddRange(await _databaseManager.UserRepository.GetUserIdsAsync(false));
            }

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

            CsvUtils.Export(filePath, head, rows);
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

            CsvUtils.Export(filePath, head, rows);
        }

        public async Task<List<Content>> GetContentsByCsvFileAsync(string filePath, Site site, Channel channel)
        {
            var contentInfoList = new List<Content>();

            CsvUtils.Import(filePath, out var head, out var rows);

            if (rows.Count <= 0) return contentInfoList;

            var tableName = _databaseManager.ChannelRepository.GetTableName(site, channel);
            var styleList = ColumnsManager.GetContentListStyles(await _databaseManager.TableStyleRepository.GetContentStylesAsync(channel, tableName));
            var nameValueCollection = new NameValueCollection();
            foreach (var style in styleList)
            {
                nameValueCollection[style.DisplayName] = StringUtils.ToLower(style.AttributeName);
            }

            var attributeNames = new List<string>();
            foreach (var columnName in head)
            {
                attributeNames.Add(!string.IsNullOrEmpty(nameValueCollection[columnName])
                    ? nameValueCollection[columnName]
                    : columnName);
            }

            foreach (var row in rows)
            {
                if (row.Count != attributeNames.Count) continue;

                var dict = new Dictionary<string, object>();

                for (var i = 0; i < attributeNames.Count; i++)
                {
                    var attributeName = attributeNames[i];
                    if (!string.IsNullOrEmpty(attributeName))
                    {
                        dict[attributeName] = row[i];
                    }
                }

                var content = new Content();
                content.LoadDict(dict);

                if (!string.IsNullOrEmpty(content.Title))
                {
                    content.SiteId = site.Id;
                    content.ChannelId = channel.Id;

                    contentInfoList.Add(content);
                }
            }

            return contentInfoList;
        }
    }
}
