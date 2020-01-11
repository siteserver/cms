using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/channels")]
    public partial class PagesChannelsController : ApiController
    {
        private const string Route = "";
        private const string RouteGet = "{siteId:int}/{channelId:int}";
        private const string RouteAppend = "actions/append";
        private const string RouteSort = "actions/sort";
        private const string RouteUpload = "actions/upload";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public async Task<ChannelsResult> List([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<ChannelsResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<ChannelsResult>("无法确定内容对应的站点");

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.SiteId);
            var cascade = await ChannelManager.GetCascadeAsync(site, channel, async (siteInfo, channelInfo) => 
            {
                var dict = new Dictionary<string, object>
                {
                    ["count"] = await DataProvider.ContentRepository.GetCountAsync(siteInfo, channelInfo, 0),
                    [nameof(Channel.IndexName)] = channelInfo.IndexName,
                    [nameof(Channel.GroupNames)] = channelInfo.GroupNames
                };
                return dict;
            });

            var indexNames = await ChannelManager.GetChannelIndexNameListAsync(request.SiteId);
            var groupNames = await DataProvider.ChannelGroupRepository.GetGroupNameListAsync(request.SiteId);

            var channelTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ContentTemplate);
            var contentPlugins = await PluginContentManager.GetContentModelPluginsAsync();
            var relatedPlugins = await PluginContentManager.GetAllContentRelatedPluginsAsync(false);

            return new ChannelsResult
            {
                Channel = cascade,
                IndexNames = indexNames,
                GroupNames = groupNames,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates,
                ContentPlugins = contentPlugins,
                RelatedPlugins = relatedPlugins
            };
        }

        [HttpGet, Route(RouteGet)]
        public async Task<ChannelResult> Get(int siteId, int channelId)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<ChannelResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return Request.BadRequest<ChannelResult>("无法确定内容对应的站点");

            var channel = await ChannelManager.GetChannelAsync(siteId, channelId);

            var linkTypes = ELinkTypeUtilsExtensions.GetAll();
            var taxisTypes = ETaxisTypeUtilsExtensions.GetAllForChannel();

            return new ChannelResult
            {
                Channel = channel,
                LinkTypes = linkTypes,
                TaxisTypes = taxisTypes
            };
        }

        [HttpPost, Route(RouteAppend)]
        public async Task<DefaultResult> Append([FromBody] AppendRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ParentId, Constants.ChannelPermissions.ChannelAdd))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<DefaultResult>("无法确定站点");

            var parent = await ChannelManager.GetChannelAsync(request.SiteId, request.ParentId);
            if (parent == null) return Request.BadRequest<DefaultResult>("无法确定父栏目");

            var insertedChannelIdHashtable = new Hashtable { [1] = request.ParentId }; //key为栏目的级别，1为第一级栏目

            var channelNames = request.Channels.Split('\n');
            IList<string> nodeIndexNameList = null;
            foreach (var item in channelNames)
            {
                if (string.IsNullOrEmpty(item)) continue;

                //count为栏目的级别
                var count = StringUtils.GetStartCount('－', item) == 0 ? StringUtils.GetStartCount('-', item) : StringUtils.GetStartCount('－', item);
                var channelName = item.Substring(count, item.Length - count);
                var indexName = string.Empty;
                count++;

                if (!string.IsNullOrEmpty(channelName) && insertedChannelIdHashtable.Contains(count))
                {
                    if (request.IsIndexName)
                    {
                        indexName = channelName.Trim();
                    }

                    if (StringUtils.Contains(channelName, "(") && StringUtils.Contains(channelName, ")"))
                    {
                        var length = channelName.IndexOf(')') - channelName.IndexOf('(');
                        if (length > 0)
                        {
                            indexName = channelName.Substring(channelName.IndexOf('(') + 1, length);
                            channelName = channelName.Substring(0, channelName.IndexOf('('));
                        }
                    }
                    channelName = channelName.Trim();
                    indexName = indexName.Trim(' ', '(', ')');
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        if (nodeIndexNameList == null)
                        {
                            nodeIndexNameList = (await DataProvider.ChannelRepository.GetIndexNameListAsync(request.SiteId)).ToList();
                        }
                        if (nodeIndexNameList.Contains(indexName))
                        {
                            indexName = string.Empty;
                        }
                        else
                        {
                            nodeIndexNameList.Add(indexName);
                        }
                    }

                    var parentId = (int)insertedChannelIdHashtable[count];

                    var insertedChannelId = await DataProvider.ChannelRepository.InsertAsync(request.SiteId, parentId, channelName, indexName, parent.ContentModelPluginId, parent.ContentRelatedPluginIdList, request.ChannelTemplateId, request.ContentTemplateId);
                    insertedChannelIdHashtable[count + 1] = insertedChannelId;

                    await CreateManager.CreateChannelAsync(request.SiteId, insertedChannelId);
                }
            }

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPut, Route(Route)]
        public async Task<DefaultResult> Edit([FromBody] PutRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.Id, Constants.ChannelPermissions.ChannelEdit))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<DefaultResult>("无法确定站点");

            if (string.IsNullOrEmpty(request.ChannelName))
            {
                return Request.BadRequest<DefaultResult>("栏目修改失败，必须填写栏目名称！");
            }

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.Id);
            if (!channel.IndexName.Equals(request.IndexName) && !string.IsNullOrEmpty(request.IndexName))
            {
                if (await DataProvider.ChannelRepository.IsIndexNameExistsAsync(request.SiteId, request.IndexName))
                {
                    return Request.BadRequest<DefaultResult>("栏目修改失败，栏目索引已存在！");
                }
            }

            if (!channel.FilePath.Equals(request.FilePath) && !string.IsNullOrEmpty(request.FilePath))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                {
                    return Request.BadRequest<DefaultResult>("栏目页面路径不符合系统要求！");
                }

                if (PathUtils.IsDirectoryPath(request.FilePath))
                {
                    request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                }

                if (await DataProvider.ChannelRepository.IsFilePathExistsAsync(request.SiteId, request.FilePath))
                {
                    return Request.BadRequest<DefaultResult>("栏目修改失败，栏目页面路径已存在！");
                }
            }

            if (!string.IsNullOrEmpty(request.ChannelFilePathRule))
            {
                var filePathRule = request.ChannelFilePathRule.Replace("|", string.Empty);
                if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                {
                    return Request.BadRequest<DefaultResult>("栏目页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(filePathRule))
                {
                    return Request.BadRequest<DefaultResult>("栏目页面命名规则必须包含生成文件的后缀！");
                }
            }

            if (!string.IsNullOrEmpty(request.ContentFilePathRule))
            {
                var filePathRule = request.ContentFilePathRule.Replace("|", string.Empty);
                if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                {
                    return Request.BadRequest<DefaultResult>("内容页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(filePathRule))
                {
                    return Request.BadRequest<DefaultResult>("内容页面命名规则必须包含生成文件的后缀！");
                }
            }

            channel.ChannelName = request.ChannelName;
            channel.IndexName = request.IndexName;
            channel.GroupNames = request.GroupNames;
            channel.ImageUrl = request.ImageUrl;
            channel.Content = request.Content;
            channel.ChannelTemplateId = request.ChannelTemplateId;
            channel.ContentTemplateId = request.ContentTemplateId;
            channel.ContentModelPluginId = request.ContentModelPluginId;
            channel.ContentRelatedPluginIdList = request.ContentRelatedPluginIdList;
            channel.LinkUrl = request.LinkUrl;
            channel.LinkType = request.LinkType;
            channel.DefaultTaxisType = request.DefaultTaxisType;
            channel.FilePath = request.FilePath;
            channel.ChannelFilePathRule = request.ChannelFilePathRule;
            channel.ContentFilePathRule = request.ContentFilePathRule;
            channel.Keywords = request.Keywords;
            channel.Description = request.Description;

            await DataProvider.ChannelRepository.UpdateAsync(channel);

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<DefaultResult> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ChannelDelete))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<DefaultResult>("无法确定站点");

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.ChannelId);
            if (channel == null) return Request.BadRequest<DefaultResult>("无法确定父栏目");

            if (channel.ChannelName != request.ChannelName)
            {
                return Request.BadRequest<DefaultResult>("请检查您输入的栏目名称是否正确");
            }

            var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.All);

            if (request.DeleteFiles)
            {
                await DeleteManager.DeleteChannelsAsync(site, channelIdList);
            }

            foreach (var channelId in channelIdList)
            {
                var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
                await DataProvider.ContentRepository.UpdateTrashContentsByChannelIdAsync(request.SiteId, channelId, tableName);
                await DataProvider.ChannelRepository.DeleteAsync(request.SiteId, channelId);
            }

            await auth.AddSiteLogAsync(request.SiteId, "删除栏目", $"栏目:{channel.ChannelName}");

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteSort)]
        public async Task<DefaultResult> Sort([FromBody] ChannelIdsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.Channels);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null)
            {
                return Request.NotFound<DefaultResult>();
            }

            foreach (var channelId in request.ChannelIds)
            {
                await CreateManager.CreateChannelAsync(request.SiteId, channelId);
            }

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<GenericResult<string>> Upload([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.SiteId, Constants.ChannelPermissions.ChannelAdd))
            {
                return Request.Unauthorized<GenericResult<string>>();
            }

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<GenericResult<string>>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return Request.BadRequest<GenericResult<string>>("导入文件为Zip格式，请选择有效的文件上传");
            }

            fileName = $"{StringUtils.GetShortGuid(false)}.zip";
            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            return new GenericResult<string>
            {
                Value = fileName
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<DefaultResult> Import([FromBody] ImportRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ChannelAdd))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            try
            {
                var filePath = PathUtils.GetTemporaryFilesPath(request.FileName);

                var importObject = new ImportObject(request.SiteId, auth.AdminName);
                await importObject.ImportChannelsAndContentsByZipFileAsync(request.ChannelId, filePath,
                    request.IsOverride);

                await auth.AddSiteLogAsync(request.SiteId, "导入栏目");
            }
            catch
            {
                return Request.BadRequest<DefaultResult>("压缩包格式不正确，请上传正确的栏目压缩包");
            }

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<GenericResult<string>> Export([FromBody] ChannelIdsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<GenericResult<string>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<GenericResult<string>>("无法确定站点");

            var exportObject = new ExportObject(request.SiteId, auth.AdminName);
            var fileName = await exportObject.ExportChannelsAsync(request.ChannelIds);
            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            var url = ApiRouteActionsDownload.GetUrl(ApiManager.InnerApiUrl, filePath);

            return new GenericResult<string>
            {
                Value = url
            };
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<DefaultResult> Create([FromBody] ChannelIdsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.Channels);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null)
            {
                return Request.NotFound<DefaultResult>();
            }

            foreach (var channelId in request.ChannelIds)
            {
                await CreateManager.CreateChannelAsync(request.SiteId, channelId);
            }

            return new DefaultResult
            {
                Value = true
            };
        }
    }
}
