using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    
    [RoutePrefix("pages/cms/channels/channels")]
    public partial class PagesChannelsController : ApiController
    {
        private const string Route = "";
        private const string RouteGet = "{siteId:int}/{channelId:int}";
        private const string RouteAppend = "actions/append";
        private const string RouteUpload = "actions/upload";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";
        private const string RouteOrder = "actions/order";
        private const string RouteSetGroups = "actions/setGroups";
        private const string RouteSetOrders = "actions/setOrders";

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
            if (site == null) return Request.NotFound<ChannelsResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                var groupNames = await DataProvider.ChannelRepository.GetGroupNamesAsync(summary.Id);
                return new
                {
                    Count = count,
                    summary.IndexName,
                    GroupNames = groupNames,
                    summary.Taxis,
                    summary.ParentId
                };
            });

            var indexNames = await DataProvider.ChannelRepository.GetChannelIndexNameListAsync(request.SiteId);
            var groupNameList = await DataProvider.ChannelGroupRepository.GetGroupNameListAsync(request.SiteId);

            var channelTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ContentTemplate);
            var contentPlugins = await PluginContentManager.GetContentModelPluginsAsync();
            var relatedPlugins = await PluginContentManager.GetAllContentRelatedPluginsAsync(false);

            return new ChannelsResult
            {
                Channel = cascade,
                IndexNames = indexNames,
                GroupNames = groupNameList,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates,
                ContentPlugins = contentPlugins,
                RelatedPlugins = relatedPlugins
            };
        }

        [HttpPost, Route(RouteAppend)]
        public async Task<List<int>> Append([FromBody] AppendRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ParentId, Constants.ChannelPermissions.ChannelAdd))
            {
                return Request.Unauthorized<List<int>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<List<int>>();

            var parent = await DataProvider.ChannelRepository.GetAsync(request.ParentId);
            if (parent == null) return Request.BadRequest<List<int>>("无法确定父栏目");

            var insertedChannelIdHashtable = new Hashtable { [1] = request.ParentId }; //key为栏目的级别，1为第一级栏目

            var channelTemplateId = request.ChannelTemplateId;
            var contentTemplateId = request.ContentTemplateId;
            if (request.IsParentTemplates)
            {
                channelTemplateId = parent.ChannelTemplateId;
                contentTemplateId = parent.ContentTemplateId;
            }

            var channelNames = request.Channels.Split('\n');
            IList<string> nodeIndexNameList = null;
            var expandedChannelIds = new List<int>
            {
                request.SiteId
            };
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

                    var insertedChannelId = await DataProvider.ChannelRepository.InsertAsync(request.SiteId, parentId, channelName, indexName, parent.ContentModelPluginId, parent.ContentRelatedPluginIds, channelTemplateId, contentTemplateId);
                    insertedChannelIdHashtable[count + 1] = insertedChannelId;
                    expandedChannelIds.Add(insertedChannelId);

                    await CreateManager.CreateChannelAsync(request.SiteId, insertedChannelId);
                }
            }

            return expandedChannelIds;
        }

        [HttpDelete, Route(Route)]
        public async Task<List<int>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ChannelDelete))
            {
                return Request.Unauthorized<List<int>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<List<int>>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return Request.BadRequest<List<int>>("无法确定父栏目");

            if (channel.ChannelName != request.ChannelName)
            {
                return Request.BadRequest<List<int>>("请检查您输入的栏目名称是否正确");
            }

            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(request.SiteId, request.ChannelId, EScopeType.All);

            if (request.DeleteFiles)
            {
                await DeleteManager.DeleteChannelsAsync(site, channelIdList);
            }

            foreach (var channelId in channelIdList)
            {
                await DataProvider.ChannelRepository.DeleteAsync(request.SiteId, channelId, auth.AdminId);
            }

            await auth.AddSiteLogAsync(request.SiteId, "删除栏目", $"栏目:{channel.ChannelName}");

            return new List<int>
            {
                request.SiteId,
                channel.ParentId
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<StringResult> Upload([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.SiteId, Constants.ChannelPermissions.ChannelAdd))
            {
                return Request.Unauthorized<StringResult>();
            }

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<StringResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return Request.BadRequest<StringResult>("导入文件为Zip格式，请选择有效的文件上传");
            }

            fileName = $"{StringUtils.GetShortGuid(false)}.zip";
            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            return new StringResult
            {
                Value = fileName
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<List<int>> Import([FromBody] ImportRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ChannelAdd))
            {
                return Request.Unauthorized<List<int>>();
            }

            try
            {
                var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
                var filePath = PathUtils.GetTemporaryFilesPath(request.FileName);

                var importObject = new ImportObject(site, auth.AdminId);
                await importObject.ImportChannelsAndContentsByZipFileAsync(request.ChannelId, filePath,
                    request.IsOverride, null);

                await auth.AddSiteLogAsync(request.SiteId, "导入栏目");
            }
            catch
            {
                return Request.BadRequest<List<int>>("压缩包格式不正确，请上传正确的栏目压缩包");
            }

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<StringResult> Export([FromBody] ChannelIdsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<StringResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<StringResult>();

            var exportObject = new ExportObject(site, auth.AdminId);
            var fileName = await exportObject.ExportChannelsAsync(request.ChannelIds);
            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            var url = ApiRouteActionsDownload.GetUrl(ApiManager.InnerApiUrl, filePath);

            return new StringResult
            {
                Value = url
            };
        }

        [HttpPost, Route(RouteOrder)]
        public async Task<List<int>> Order([FromBody] OrderRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<List<int>>();
            }

            if (request.IsUp)
            {
                await DataProvider.ChannelRepository.UpdateTaxisUpAsync(request.SiteId, request.ChannelId, request.ParentId, request.Taxis);
            }
            else
            {
                await DataProvider.ChannelRepository.UpdateTaxisDownAsync(request.SiteId, request.ChannelId, request.ParentId, request.Taxis);
            }

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }

        [HttpPost, Route(RouteSetGroups)]
        public async Task<List<int>> SetGroups([FromBody] SetGroupsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<List<int>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<List<int>>();

            var expendedChannelIds = new List<int>
            {
                request.SiteId
            };
            foreach (var channelId in request.ChannelIds)
            {
                if (!expendedChannelIds.Contains(channelId))
                {
                    expendedChannelIds.Add(channelId);
                }
                await DataProvider.ChannelRepository.SetGroupNamesAsync(request.SiteId, channelId, request.GroupNames);
            }

            await auth.AddSiteLogAsync(request.SiteId, "设置栏目组");

            return expendedChannelIds;
        }

        [HttpPost, Route(RouteSetOrders)]
        public async Task<BoolResult> SetOrders([FromBody] ChannelIdsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            foreach (var channelId in request.ChannelIds)
            {
                await CreateManager.CreateChannelAsync(request.SiteId, channelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
