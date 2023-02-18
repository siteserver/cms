using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class FormRepository : IFormRepository
    {
        public const string DefaultListAttributeNames = "Name,Mobile,Email,Content";
        private readonly ISettingsManager _settingsManager;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly Repository<Form> _repository;

        public FormRepository(ISettingsManager settingsManager, ITableStyleRepository tableStyleRepository)
        {
            _settingsManager = settingsManager;
            _tableStyleRepository = tableStyleRepository;
            _repository = new Repository<Form>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId)
        {
            return CacheUtils.GetListKey(TableName, siteId);
        }

        public async Task<int> InsertAsync(Form form)
        {
            if (form.SiteId == 0) return 0;
            if (string.IsNullOrEmpty(form.Title)) return 0;

            form.Taxis = await GetMaxTaxisAsync(form.SiteId) + 1;

            form.Id = await _repository.InsertAsync(form, Q.CachingRemove(GetCacheKey(form.SiteId)));

            return form.Id;
        }

        public async Task<bool> UpdateAsync(Form form)
        {
            var updated = await _repository.UpdateAsync(form, Q.CachingRemove(GetCacheKey(form.SiteId)));

            return updated;
        }

        public async Task DeleteAsync(int siteId, int formId)
        {
            if (formId <= 0) return;

            await _repository.DeleteAsync(formId, Q.CachingRemove(GetCacheKey(siteId)));
        }

        public async Task UpdateTaxisToDownAsync(int siteId, int formId)
        {
            var taxis = await _repository.GetAsync<int>(Q
                .Select(nameof(Form.Taxis))
                .Where(nameof(Form.Id), formId)
            );

            var data = await _repository.GetAsync(Q
                .Where(nameof(Form.SiteId), siteId)
                .Where(nameof(Form.Taxis), ">", taxis)
                .OrderBy(nameof(Form.Taxis))
            );

            if (data == null) return;

            var higherId = data.Id;
            var higherTaxis = data.Taxis;

            await SetTaxisAsync(siteId, formId, higherTaxis);
            await SetTaxisAsync(siteId, higherId, taxis);
        }

        public async Task UpdateTaxisToUpAsync(int siteId, int formId)
        {
            var taxis = await _repository.GetAsync<int>(Q
                .Select(nameof(Form.Taxis))
                .Where(nameof(Form.Id), formId)
            );

            var data = await _repository.GetAsync(Q
                .Where(nameof(Form.SiteId), siteId)
                .Where(nameof(Form.Taxis), "<", taxis)
                .OrderByDesc(nameof(Form.Taxis))
            );

            if (data == null) return;

            var lowerId = data.Id;
            var lowerTaxis = data.Taxis;

            await SetTaxisAsync(siteId, formId, lowerTaxis);
            await SetTaxisAsync(siteId, lowerId, taxis);
        }

        private async Task<int> GetMaxTaxisAsync(int siteId)
        {
            return await _repository.MaxAsync(nameof(Form.Taxis), Q
                .Where(nameof(Form.SiteId), siteId)
            ) ?? 0;
        }

        private async Task SetTaxisAsync(int siteId, int formId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Form.Taxis), taxis)
                .Where(nameof(Form.Id), formId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task<List<Form>> GetFormsAsync(int siteId)
        {
            try
            {
                return await GetFormsBySiteIdAsync(siteId);
            }
            catch
            {
                if (!await _settingsManager.Database.IsTableExistsAsync(TableName))
                {
                    await _settingsManager.Database.CreateTableAsync(TableName, TableColumns);
                }

                return await GetFormsBySiteIdAsync(siteId);
            }
        }

        private async Task<List<Form>> GetFormsBySiteIdAsync(int siteId)
        {
            var formList = await _repository.GetAllAsync(Q
                .Where(nameof(Form.SiteId), siteId)
                .OrderBy(nameof(Form.Taxis), nameof(Form.Id))
                .CachingGet(GetCacheKey(siteId))
            );

            return formList
                .OrderBy(form => form.Taxis == 0 ? int.MaxValue : form.Taxis)
                .ToList();
        }

        public async Task<string> GetImportTitleAsync(int siteId, string title)
        {
            string importTitle;
            if (title.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var inputNameCount = 0;
                var lastInputName = title.Substring(title.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstInputName = title.Substring(0, title.Length - lastInputName.Length);
                try
                {
                    inputNameCount = int.Parse(lastInputName);
                }
                catch
                {
                    // ignored
                }
                inputNameCount++;
                importTitle = firstInputName + inputNameCount;
            }
            else
            {
                importTitle = title + "_1";
            }

            var form = await GetByTitleAsync(siteId, title);
            if (form != null)
            {
                importTitle = await GetImportTitleAsync(siteId, importTitle);
            }

            return importTitle;
        }

        public async Task<Form> GetAsync(int siteId, int id)
        {
            var formList = await GetFormsAsync(siteId);

            return formList.FirstOrDefault(x => x.Id == id);
        }

        public async Task<Form> GetByTitleAsync(int siteId, string title)
        {
            var formList = await GetFormsAsync(siteId);
            return formList.FirstOrDefault(x => x.Title == title);
        }

        public List<string> GetAllAttributeNames(List<TableStyle> styles)
        {
            var allAttributeNames = new List<string>
            {
                nameof(FormData.Id),
                nameof(FormData.Guid)
            };
            foreach (var style in styles)
            {
                allAttributeNames.Add(style.AttributeName);
            }
            allAttributeNames.Add(nameof(FormData.CreatedDate));
            allAttributeNames.Add(nameof(FormData.LastModifiedDate));

            return allAttributeNames;
        }

        public List<ContentColumn> GetColumns(List<string> listAttributeNames, List<TableStyle> styles, bool isReply)
        {
            var columns = new List<ContentColumn>
            {
                new ContentColumn
                {
                    AttributeName = nameof(FormData.Id),
                    DisplayName = "Id",
                    IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.Id))
                },
                new ContentColumn
                {
                    AttributeName = nameof(FormData.Guid),
                    DisplayName = "编号",
                    IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.Guid))
                }
            };

            foreach (var style in styles)
            {
                if (!string.IsNullOrEmpty(style.DisplayName) && style.InputType != InputType.TextEditor)
                {
                    var column = new ContentColumn
                    {
                        AttributeName = style.AttributeName,
                        DisplayName = style.DisplayName,
                        InputType = style.InputType,
                        IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, style.AttributeName)
                    };

                    columns.Add(column);
                }
            }

            columns.AddRange(new List<ContentColumn>
            {
                new ContentColumn
                {
                    AttributeName = nameof(FormData.CreatedDate),
                    DisplayName = "添加时间",
                    IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.CreatedDate))
                },
                new ContentColumn
                {
                    AttributeName = nameof(FormData.LastModifiedDate),
                    DisplayName = "更新时间",
                    IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.LastModifiedDate))
                },
                new ContentColumn
                {
                    AttributeName = nameof(FormData.ChannelId),
                    DisplayName = "所在栏目页",
                    IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.ChannelId))
                },
                new ContentColumn
                {
                    AttributeName = nameof(FormData.ContentId),
                    DisplayName = "所在内容页",
                    IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.ContentId))
                }
            });

            if (isReply)
            {
                columns.AddRange(new List<ContentColumn>
                {
                    new ContentColumn
                    {
                        AttributeName = nameof(FormData.ReplyDate),
                        DisplayName = "回复时间",
                        IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.ReplyDate))
                    },
                    new ContentColumn
                    {
                        AttributeName = nameof(FormData.ReplyContent),
                        DisplayName = "回复内容",
                        IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, nameof(FormData.ReplyContent))
                    }
                });
            }

            return columns;
        }

        public int GetPageSize(Form form)
        {
            if (form == null || form.PageSize <= 0) return 30;
            return form.PageSize;
        }
    }
}
