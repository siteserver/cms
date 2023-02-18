using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils.Serialization.Atom.Atom.AdditionalElements;
using SSCMS.Core.Utils.Serialization.Atom.Atom.AdditionalElements.DublinCore;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
    internal class FormIe
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly int _siteId;
        private readonly string _directoryPath;

        public FormIe(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, int siteId, string directoryPath)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _caching = caching;
            _siteId = siteId;
            _directoryPath = directoryPath;
        }

        public async Task ExportFormsAsync()
        {
            DirectoryUtils.DeleteDirectoryIfExists(_directoryPath);
            var forms = await _databaseManager.FormRepository.GetFormsAsync(_siteId);
            foreach (var form in forms)
            {
                var directoryPath = PathUtils.Combine(_directoryPath, form.Id.ToString());
                await ExportFormAsync(_siteId, directoryPath, form.Id);
            }
        }

        public async Task ImportFormsAsync(string guid)
        {
            if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;
            
            var directoryPaths = DirectoryUtils.GetDirectoryPaths(_directoryPath);
            foreach (var directoryPath in directoryPaths)
            {
                _caching.SetProcess(guid, $"导入表单文件: {directoryPath}");
                await ImportFormAsync(_siteId, directoryPath, true);
            }
        }

        private const string VersionFileName = "version.json";
        private const string Prefix = "SiteServer_";

        public async Task ImportFormAsync(int siteId, string directoryPath, bool overwrite)
        {
            if (!Directory.Exists(directoryPath)) return;
            var isHistoric = IsHistoric(directoryPath);

            var filePaths = Directory.GetFiles(directoryPath);

            var form = new Form();

            foreach (var filePath in filePaths)
            {
                if (!StringUtils.EndsWithIgnoreCase(filePath, ".xml")) continue;

                var feed = AtomFeed.Load(new FileStream(filePath, FileMode.Open));

                foreach (var tableColumn in _databaseManager.FormRepository.TableColumns)
                {
                    var value = GetValue(feed.AdditionalElements, tableColumn);
                    if (tableColumn.AttributeName == "ExtendValues")
                    {
                        var extendValues = TranslateUtils.JsonDeserialize<Dictionary<string, string>>(value.ToString());
                        foreach (var key in extendValues.Keys)
                        {
                            form.Set(key, extendValues[key]);
                        }
                    }
                    else
                    {
                        form.Set(tableColumn.AttributeName, value);
                    }
                }

                form.SiteId = siteId;

                var srcForm = await _databaseManager.FormRepository.GetByTitleAsync(siteId, form.Title);
                if (srcForm != null)
                {
                    if (overwrite)
                    {
                        await _databaseManager.FormRepository.DeleteAsync(siteId, srcForm.Id);
                    }
                    else
                    {
                        form.Title = await _databaseManager.FormRepository.GetImportTitleAsync(siteId, form.Title);
                    }
                }

                form.Id = await _databaseManager.FormRepository.InsertAsync(form);

                var directoryName = GetDcElementContent(feed.AdditionalElements, "Id");
                var titleAttributeNameDict = new NameValueCollection();
                if (!string.IsNullOrEmpty(directoryName))
                {
                    var fieldDirectoryPath = PathUtils.Combine(directoryPath, directoryName);
                    titleAttributeNameDict = await ImportFieldsAsync(siteId, form.Id, fieldDirectoryPath, isHistoric);
                }

                var entryList = new List<AtomEntry>();
                foreach (AtomEntry entry in feed.Entries)
                {
                    entryList.Add(entry);
                }

                entryList.Reverse();

                foreach (var entry in entryList)
                {
                    var data = new FormData();

                    foreach (var tableColumn in _databaseManager.FormDataRepository.TableColumns)
                    {
                        var value = GetValue(entry.AdditionalElements, tableColumn);
                        data.Set(tableColumn.AttributeName, value);
                    }

                    var attributes = GetDcElementNameValueCollection(entry.AdditionalElements);
                    foreach (string entryName in attributes.Keys)
                    {
                        data.Set(entryName, attributes[entryName]);
                    }

                    if (isHistoric)
                    {
                        foreach (var title in titleAttributeNameDict.AllKeys)
                        {
                            data.Set(title, data.Get(titleAttributeNameDict[title]));
                        }

                        data.ReplyContent = GetDcElementContent(entry.AdditionalElements, "Reply");
                        if (!string.IsNullOrEmpty(data.ReplyContent))
                        {
                            data.IsReplied = true;
                        }
                        data.CreatedDate = TranslateUtils.ToDateTime(GetDcElementContent(entry.AdditionalElements, "adddate"));
                    }

                    await _databaseManager.FormDataRepository.InsertAsync(form, data);
                }
            }

            if (form.Id > 0)
            {
                var relatedIdentities = _databaseManager.FormRepository.GetRelatedIdentities(form.Id);
                var stylesFilePath = PathUtils.Combine(directoryPath, "tableStyle.zip");
                if (FileUtils.IsFileExists(stylesFilePath))
                {
                    await _pathManager.ImportStylesByZipFileAsync(_databaseManager.FormDataRepository.TableName, relatedIdentities, stylesFilePath);
                    await _databaseManager.TableStyleRepository.DeleteAsync(_databaseManager.FormDataRepository.TableName, form.Id, "SiteId");
                    await _databaseManager.TableStyleRepository.DeleteAsync(_databaseManager.FormDataRepository.TableName, form.Id, "ChannelId");
                    await _databaseManager.TableStyleRepository.DeleteAsync(_databaseManager.FormDataRepository.TableName, form.Id, "ContentId");
                    await _databaseManager.TableStyleRepository.DeleteAsync(_databaseManager.FormDataRepository.TableName, form.Id, "FormId");
                    await _databaseManager.TableStyleRepository.DeleteAsync(_databaseManager.FormDataRepository.TableName, form.Id, "IsReplied");
                    await _databaseManager.TableStyleRepository.DeleteAsync(_databaseManager.FormDataRepository.TableName, form.Id, "ReplyContent");
                    await _databaseManager.TableStyleRepository.DeleteAsync(_databaseManager.FormDataRepository.TableName, form.Id, "ReplyDate");
                }
            }
        }

        public async Task ExportFormAsync(int siteId, string directoryPath, int formId)
        {
            var form = await _databaseManager.FormRepository.GetAsync(siteId, formId);
            var filePath = PathUtils.Combine(directoryPath, form.Id + ".xml");
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            var feed = GetEmptyFeed();

            foreach (var tableColumn in _databaseManager.FormRepository.TableColumns)
            {
                SetValue(feed.AdditionalElements, tableColumn, form);
            }

            var contentList = await _databaseManager.FormDataRepository.GetListAsync(form);
            foreach (var content in contentList)
            {
                var entry = GetAtomEntry(content);
                feed.Entries.Add(entry);
            }
            feed.Save(filePath);

            var relatedIdentities = _databaseManager.FormRepository.GetRelatedIdentities(form.Id);
            var stylesFileName = await _pathManager.ExportStylesAsync(siteId, _databaseManager.FormDataRepository.TableName, relatedIdentities);
            var stylesFilePath = _pathManager.GetTemporaryFilesPath(stylesFileName);
            FileUtils.CopyFile(stylesFilePath, PathUtils.Combine(directoryPath, stylesFileName));
        }

        private static bool IsHistoric(string directoryPath)
        {
            if (!FileUtils.IsFileExists(PathUtils.Combine(directoryPath, VersionFileName))) return true;

            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, VersionFileName));

            return false;
        }
        private async Task<NameValueCollection> ImportFieldsAsync(int siteId, int formId, string styleDirectoryPath, bool isHistoric)
        {
            var titleAttributeNameDict = new NameValueCollection();

            if (!Directory.Exists(styleDirectoryPath)) return titleAttributeNameDict;

            var form = await _databaseManager.FormRepository.GetAsync(siteId, formId);
            var relatedIdentities = _databaseManager.FormRepository.GetRelatedIdentities(form.Id);
            await _pathManager.ImportStylesByDirectoryAsync(_databaseManager.FormDataRepository.TableName, relatedIdentities, styleDirectoryPath);

            //var filePaths = Directory.GetFiles(styleDirectoryPath);
            //foreach (var filePath in filePaths)
            //{
            //    var feed = AtomFeed.Load(new FileStream(filePath, FileMode.Open));

            //    var attributeName = GetDcElementContent(feed.AdditionalElements, "AttributeName");
            //    var title = GetDcElementContent(feed.AdditionalElements, "DisplayName");
            //    if (isHistoric)
            //    {
            //        title = GetDcElementContent(feed.AdditionalElements, "DisplayName");

            //        titleAttributeNameDict[title] = attributeName;
            //    }
            //    var fieldType = GetDcElementContent(feed.AdditionalElements, nameof(TableStyle.InputType));
            //    if (isHistoric)
            //    {
            //        fieldType = GetDcElementContent(feed.AdditionalElements, "InputType");
            //    }
            //    var taxis = FormUtils.ToIntWithNegative(GetDcElementContent(feed.AdditionalElements, "Taxis"), 0);

            //    var style = new TableStyle
            //    {
            //        TableName = tableName,
            //        RelatedIdentity = relatedIdentities[0],
            //        Taxis = taxis,
            //        Title = title,
            //        InputType = TranslateUtils.ToEnum(fieldType, InputType.Text)
            //    };

            //    var fieldItems = new List<FieldItemInfo>();
            //    foreach (AtomEntry entry in feed.Entries)
            //    {
            //        var itemValue = GetDcElementContent(entry.AdditionalElements, "ItemValue");
            //        var isSelected = FormUtils.ToBool(GetDcElementContent(entry.AdditionalElements, "IsSelected"), false);

            //        fieldItems.Add(new FieldItemInfo
            //        {
            //            FormId = formId,
            //            FieldId = 0,
            //            Value = itemValue,
            //            IsSelected = isSelected
            //        });
            //    }

            //    if (fieldItems.Count > 0)
            //    {
            //        style.Items = fieldItems;
            //    }

            //    if (await _fieldRepository.IsTitleExistsAsync(formId, title))
            //    {
            //        await _fieldRepository.DeleteAsync(formId, title);
            //    }
            //    await _fieldRepository.InsertAsync(siteId, style);
            //}

            return titleAttributeNameDict;
        }

        private string ToXmlContent(string inputString)
        {
            var contentBuilder = new StringBuilder(inputString);
            contentBuilder.Replace("<![CDATA[", string.Empty);
            contentBuilder.Replace("]]>", string.Empty);
            contentBuilder.Insert(0, "<![CDATA[");
            contentBuilder.Append("]]>");
            return contentBuilder.ToString();
        }

        private void AddDcElement(ScopedElementCollection collection, string name, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                collection.Add(new DcElement(Prefix + name, ToXmlContent(content)));
            }
        }

        private string GetDcElementContent(ScopedElementCollection additionalElements, string name, string defaultContent = "")
        {
            var localName = Prefix + name;
            var element = additionalElements.FindScopedElementByLocalName(localName);
            return element != null ? element.Content : defaultContent;
        }

        private NameValueCollection GetDcElementNameValueCollection(ScopedElementCollection additionalElements)
        {
            return additionalElements.GetNameValueCollection(Prefix);
        }

        private AtomFeed GetEmptyFeed()
        {
            var feed = new AtomFeed
            {
                Title = new AtomContentConstruct("title", "siteserver channel"),
                Author = new AtomPersonConstruct("author",
                    "siteserver", new Uri("https://sscms.com")),
                Modified = new AtomDateConstruct("modified", DateTime.Now,
                    DateTimeOffset.UtcNow.Offset)
            };

            return feed;
        }

        private AtomEntry GetEmptyEntry()
        {
            var entry = new AtomEntry
            {
                Id = new Uri("https://sscms.com/"),
                Title = new AtomContentConstruct("title", "title"),
                Modified = new AtomDateConstruct("modified", DateTime.Now,
                    DateTimeOffset.UtcNow.Offset),
                Issued = new AtomDateConstruct("issued", DateTime.Now,
                    DateTimeOffset.UtcNow.Offset)
            };

            return entry;
        }

        private AtomEntry GetAtomEntry(Entity entity)
        {
            var entry = GetEmptyEntry();

            foreach (var keyValuePair in entity.ToDictionary())
            {
                if (keyValuePair.Value != null)
                {
                    AddDcElement(entry.AdditionalElements, keyValuePair.Key, keyValuePair.Value.ToString());
                }
            }

            return entry;
        }

        private object GetValue(ScopedElementCollection additionalElements, TableColumn tableColumn)
        {
            if (tableColumn.DataType == DataType.Boolean)
            {
                return TranslateUtils.ToBool(GetDcElementContent(additionalElements, tableColumn.AttributeName), false);
            }
            if (tableColumn.DataType == DataType.DateTime)
            {
                return TranslateUtils.ToDateTime(GetDcElementContent(additionalElements, tableColumn.AttributeName));
            }
            if (tableColumn.DataType == DataType.Decimal)
            {
                return TranslateUtils.ToDecimalWithNegative(GetDcElementContent(additionalElements, tableColumn.AttributeName), 0);
            }
            if (tableColumn.DataType == DataType.Integer)
            {
                return TranslateUtils.ToIntWithNegative(GetDcElementContent(additionalElements, tableColumn.AttributeName), 0);
            }
            if (tableColumn.DataType == DataType.Text)
            {
                return Decrypt(GetDcElementContent(additionalElements, tableColumn.AttributeName));
            }
            return GetDcElementContent(additionalElements, tableColumn.AttributeName);
        }

        private void SetValue(ScopedElementCollection additionalElements, TableColumn tableColumn, Entity entity)
        {
            var value = entity.Get(tableColumn.AttributeName)?.ToString();
            if (tableColumn.DataType == DataType.Text)
            {
                value = Encrypt(value);
            }
            AddDcElement(additionalElements, tableColumn.AttributeName, value);
        }

        private string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString, "TgQQk42O");
        }

        private string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString, "TgQQk42O");
        }
    }
}
