using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class FormDataRepository : IFormDataRepository
    {
        public const string TABLE_NAME = "siteserver_FormData";
        private readonly ISettingsManager _settingsManager;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IFormRepository _formRepository;
        private readonly Repository<FormData> _repository;

        public FormDataRepository(ISettingsManager settingsManager, ITableStyleRepository tableStyleRepository, IFormRepository formRepository)
        {
            _settingsManager = settingsManager;
            _tableStyleRepository = tableStyleRepository;
            _formRepository = formRepository;
            _repository = new Repository<FormData>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(FormData.Id);

            public const string FormId = nameof(FormData.FormId);
            public const string ExtendValues = nameof(ExtendValues);
        }

        public async Task<int> InsertAsync(Form form, FormData data)
        {
            data.FormId = form.Id;
            data.Id = await _repository.InsertAsync(data);

            form.TotalCount += 1;
            await _formRepository.UpdateAsync(form);

            return data.Id;
        }

        public async Task UpdateAsync(FormData data)
        {
            await _repository.UpdateAsync(data);
        }

        public async Task<FormData> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task ReplyAsync(Form form, FormData data)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(FormData.IsReplied), true)
                .Set(nameof(FormData.ReplyDate), DateTime.Now)
                .Set(nameof(FormData.ReplyContent), data.ReplyContent)
                .Where("Id", data.Id)
            );

            if (!data.IsReplied)
            {
                form.RepliedCount += 1;
                await _formRepository.UpdateAsync(form);
            }
        }

        public async Task DeleteByFormIdAsync(int formId)
        {
            if (formId <= 0) return;

            await _repository.DeleteAsync(Q.Where(Attr.FormId, formId));
        }

        public async Task DeleteAsync(Form form, FormData data)
        {
            await _repository.DeleteAsync(data.Id);

            if (data.IsReplied)
            {
                form.RepliedCount -= 1;
            }
            form.TotalCount -= 1;
            await _formRepository.UpdateAsync(form);
        }

        public async Task<int> GetCountAsync(int formId)
        {
            return await _repository.CountAsync(Q.Where(Attr.FormId, formId));
        }

        public async Task<(int Total, List<FormData>)> GetListAsync(Form form, bool isRepliedOnly, DateTime? startDate, DateTime? endDate, string word, int page, int pageSize)
        {
            try
            {
                return await GetListPrivateAsync(form, isRepliedOnly, startDate, endDate, word, page, pageSize);
            }
            catch
            {
                if (!await _settingsManager.Database.IsTableExistsAsync(TableName))
                {
                    await _settingsManager.Database.CreateTableAsync(TableName, TableColumns);
                }

                return await GetListPrivateAsync(form, isRepliedOnly, startDate, endDate, word, page, pageSize);
            }
        }

        private async Task<(int Total, List<FormData>)> GetListPrivateAsync(Form form, bool isRepliedOnly, DateTime? startDate, DateTime? endDate, string word, int page, int pageSize)
        {
            if (form.TotalCount == 0)
            {
                return (0, new List<FormData>());
            }

            if (page == 0) page = 1;

            var q = Q
                .Where(Attr.FormId, form.Id)
                .OrderBy(nameof(FormData.IsReplied))
                .OrderByDesc(Attr.Id)
                ;

            if (isRepliedOnly)
            {
                q.Where(nameof(FormData.IsReplied), true);
            }

            if (startDate.HasValue)
            {
                q.Where(nameof(FormData.CreatedDate), ">=", startDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (endDate.HasValue)
            {
                q.Where(nameof(FormData.CreatedDate), "<=", endDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            if (!string.IsNullOrEmpty(word))
            {
                q.Where(query => query
                    .WhereLike(Attr.ExtendValues, $"%{word}%")
                    .OrWhereLike(nameof(FormData.ReplyContent), $"%{word}%")
                );
            }

            var count = await _repository.CountAsync(q);
            var list = await _repository.GetAllAsync(q.ForPage(page, pageSize));

            return (count, list);
        }

        public async Task<IList<FormData>> GetListAsync(Form form)
        {
            if (form.TotalCount == 0)
            {
                return new List<FormData>();
            }

            var q = Q
                .Where(Attr.FormId, form.Id)
                .OrderBy(nameof(FormData.IsReplied))
                .OrderByDesc(Attr.Id);

            return await _repository.GetAllAsync(q);
        }

        public string GetValue(TableStyle style, FormData data)
        {
            var value = string.Empty;
            if (data.ContainsKey(style.AttributeName))
            {
                var fieldValue = data.Get<string>(style.AttributeName);

                if (style.InputType == InputType.CheckBox || style.InputType == InputType.SelectMultiple)
                {
                    var list = TranslateUtils.JsonDeserialize<List<string>>(fieldValue);
                    if (list != null)
                    {
                        value = string.Join(",", list);
                    }
                }
                else if (style.InputType == InputType.Date)
                {
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        var date = TranslateUtils.ToDateTime(fieldValue, DateTime.MinValue);
                        if (date != DateTime.MinValue)
                        {
                            value = date.ToString("yyyy-MM-dd");
                        }
                    }
                }
                else if (style.InputType == InputType.DateTime)
                {
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        var date = TranslateUtils.ToDateTime(fieldValue, DateTime.MinValue);
                        if (date != DateTime.MinValue)
                        {
                            value = date.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                }
                else
                {
                    value = fieldValue;
                }
            }

            return value;
        }

        public async Task<FormData> GetAsync(int dataId, int formId, List<TableStyle> styles)
        {
            FormData data;
            if (dataId > 0)
            {
                data = await GetAsync(dataId);
            }
            else
            {
                data = new FormData
                {
                    FormId = formId
                };

                foreach (var style in styles)
                {
                    if (style.InputType == InputType.Text || style.InputType == InputType.TextArea || style.InputType == InputType.TextEditor || style.InputType == InputType.Hidden)
                    {
                        if (string.IsNullOrEmpty(style.DefaultValue)) continue;

                        data.Set(style.AttributeName, style.DefaultValue);
                    }
                    else if (style.InputType == InputType.Number)
                    {
                        if (string.IsNullOrEmpty(style.DefaultValue)) continue;

                        data.Set(style.AttributeName, TranslateUtils.ToInt(style.DefaultValue));
                    }
                    else if (style.InputType == InputType.CheckBox || style.InputType == InputType.SelectMultiple)
                    {
                        var value = new List<string>();

                        if (style.Items != null)
                        {
                            foreach (var item in style.Items)
                            {
                                if (item.Selected)
                                {
                                    value.Add(item.Value);
                                }
                            }
                        }

                        data.Set(style.AttributeName, value);
                    }
                    else if (style.InputType == InputType.Radio || style.InputType == InputType.SelectOne)
                    {
                        if (style.Items != null)
                        {
                            foreach (var item in style.Items)
                            {
                                if (item.Selected)
                                {
                                    data.Set(style.AttributeName, item.Value);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(style.DefaultValue))
                        {
                            data.Set(style.AttributeName, style.DefaultValue);
                        }
                    }
                }
            }

            return data;
        }

    }
}
