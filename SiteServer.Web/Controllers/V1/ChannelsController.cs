using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/channels")]
    public class ChannelsController : ApiController
    {
        private const string RouteSite = "{siteId:int}";
        private const string RouteChannel = "{siteId:int}/{channelId:int}";

        [HttpPost, Route(RouteSite)]
        public async Task<IHttpActionResult> Create(int siteId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var parentId = request.GetPostInt(nameof(Channel.ParentId), siteId);

                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeChannels) ||
                              request.IsAdminLoggin &&
                              await request.AdminPermissions.HasChannelPermissionsAsync(siteId, parentId,
                                  Constants.ChannelPermissions.ChannelAdd);
                if (!isAuth) return Unauthorized();

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var contentModelPluginId = request.GetPostString(nameof(Channel.ContentModelPluginId));
                var contentRelatedPluginIdList = StringUtils.GetStringList(request.GetPostString(nameof(Channel.ContentRelatedPluginIdList)));

                var channelName = request.GetPostString(nameof(Channel.ChannelName));
                var indexName = request.GetPostString(nameof(Channel.IndexName));
                var filePath = request.GetPostString(nameof(Channel.FilePath));
                var channelFilePathRule = request.GetPostString(nameof(Channel.ChannelFilePathRule));
                var contentFilePathRule = request.GetPostString(nameof(Channel.ContentFilePathRule));
                var groupNames = StringUtils.GetStringList(request.GetPostString(nameof(Channel.GroupNames)));
                var imageUrl = request.GetPostString(nameof(Channel.ImageUrl));
                var content = request.GetPostString(nameof(Channel.Content));
                var keywords = request.GetPostString(nameof(Channel.Keywords));
                var description = request.GetPostString(nameof(Channel.Description));
                var linkUrl = request.GetPostString(nameof(Channel.LinkUrl));
                var linkType = TranslateUtils.ToEnum(request.GetPostString(nameof(Channel.LinkType)), LinkType.None);
                var channelTemplateId = request.GetPostInt(nameof(Channel.ChannelTemplateId));
                var contentTemplateId = request.GetPostInt(nameof(Channel.ContentTemplateId));

                var channelInfo = new Channel
                {
                    SiteId = siteId,
                    ParentId = parentId,
                    ContentModelPluginId = contentModelPluginId,
                    ContentRelatedPluginIdList = contentRelatedPluginIdList
                };

                if (!string.IsNullOrEmpty(indexName))
                {
                    var indexNameList = await DataProvider.ChannelRepository.GetIndexNameListAsync(siteId);
                    if (indexNameList.Contains(indexName))
                    {
                        return BadRequest("栏目添加失败，栏目索引已存在！");
                    }
                }

                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(filePath))
                    {
                        return BadRequest("栏目页面路径不符合系统要求！");
                    }

                    if (PathUtils.IsDirectoryPath(filePath))
                    {
                        filePath = PageUtils.Combine(filePath, "index.html");
                    }

                    var filePathList = await DataProvider.ChannelRepository.GetAllFilePathBySiteIdAsync(siteId);
                    if (filePathList.Contains(filePath))
                    {
                        return BadRequest("栏目添加失败，栏目页面路径已存在！");
                    }
                }

                if (!string.IsNullOrEmpty(channelFilePathRule))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(channelFilePathRule))
                    {
                        return BadRequest("栏目页面命名规则不符合系统要求！");
                    }
                    if (PathUtils.IsDirectoryPath(channelFilePathRule))
                    {
                        return BadRequest("栏目页面命名规则必须包含生成文件的后缀！");
                    }
                }

                if (!string.IsNullOrEmpty(contentFilePathRule))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(contentFilePathRule))
                    {
                        return BadRequest("内容页面命名规则不符合系统要求！");
                    }
                    if (PathUtils.IsDirectoryPath(contentFilePathRule))
                    {
                        return BadRequest("内容页面命名规则必须包含生成文件的后缀！");
                    }
                }

                //var parentChannel = await ChannelManager.GetChannelAsync(siteId, parentId);
                //var styleList = TableStyleManager.GetChannelStyleList(parentChannel);
                //var extendedAttributes = BackgroundInputTypeParser.SaveAttributes(site, styleList, Request.Form, null);

                var dict = request.GetPostObject<Dictionary<string, object>>();
                foreach (var o in dict)
                {
                    channelInfo.Set(o.Key, o.Value);
                }
                //foreach (string key in attributes)
                //{
                //    channel.SetExtendedAttribute(key, attributes[key]);
                //}

                channelInfo.ChannelName = channelName;
                channelInfo.IndexName = indexName;
                channelInfo.FilePath = filePath;
                channelInfo.ChannelFilePathRule = channelFilePathRule;
                channelInfo.ContentFilePathRule = contentFilePathRule;

                channelInfo.GroupNames = groupNames;
                channelInfo.ImageUrl = imageUrl;
                channelInfo.Content = content;
                channelInfo.Keywords = keywords;
                channelInfo.Description = description;
                channelInfo.LinkUrl = linkUrl;
                channelInfo.LinkType = linkType;
                channelInfo.ChannelTemplateId = channelTemplateId;
                channelInfo.ContentTemplateId = contentTemplateId;

                channelInfo.AddDate = DateTime.Now;
                channelInfo.Id = await DataProvider.ChannelRepository.InsertAsync(channelInfo);
                //栏目选择投票样式后，内容

                await CreateManager.CreateChannelAsync(siteId, channelInfo.Id);

                await request.AddSiteLogAsync(siteId, "添加栏目", $"栏目:{channelName}");

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteChannel)]
        public async Task<IHttpActionResult> Update(int siteId, int channelId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeChannels) ||
                              request.IsAdminLoggin &&
                              await request.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                                  Constants.ChannelPermissions.ChannelEdit);
                if (!isAuth) return Unauthorized();

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var dict = request.GetPostObject<Dictionary<string, object>>();
                foreach (var o in dict)
                {
                    channelInfo.Set(o.Key, o.Value);
                }

                if (request.IsPostExists(nameof(Channel.ChannelName)))
                {
                    channelInfo.ChannelName = request.GetPostString(nameof(Channel.ChannelName));
                }

                if (request.IsPostExists(nameof(Channel.IndexName)))
                {
                    var indexName = request.GetPostString(nameof(Channel.IndexName));
                    if (!channelInfo.IndexName.Equals(indexName) && !string.IsNullOrEmpty(indexName))
                    {
                        var indexNameList = await DataProvider.ChannelRepository.GetIndexNameListAsync(siteId);
                        if (indexNameList.Contains(indexName))
                        {
                            return BadRequest("栏目属性修改失败，栏目索引已存在！");
                        }
                    }
                    channelInfo.IndexName = indexName;
                }

                if (request.IsPostExists(nameof(Channel.ContentModelPluginId)))
                {
                    var contentModelPluginId = request.GetPostString(nameof(Channel.ContentModelPluginId));
                    if (channelInfo.ContentModelPluginId != contentModelPluginId)
                    {
                        channelInfo.ContentModelPluginId = contentModelPluginId;
                    }
                }

                if (request.IsPostExists(nameof(Channel.ContentRelatedPluginIdList)))
                {
                    channelInfo.ContentRelatedPluginIdList = StringUtils.GetStringList(request.GetPostString(nameof(Channel.ContentRelatedPluginIdList)));
                }

                if (request.IsPostExists(nameof(Channel.FilePath)))
                {
                    var filePath = request.GetPostString(nameof(Channel.FilePath));
                    filePath = filePath.Trim();
                    if (!channelInfo.FilePath.Equals(filePath) && !string.IsNullOrEmpty(filePath))
                    {
                        if (!DirectoryUtils.IsDirectoryNameCompliant(filePath))
                        {
                            return BadRequest("栏目页面路径不符合系统要求！");
                        }

                        if (PathUtils.IsDirectoryPath(filePath))
                        {
                            filePath = PageUtils.Combine(filePath, "index.html");
                        }

                        var filePathList = await DataProvider.ChannelRepository.GetAllFilePathBySiteIdAsync(siteId);
                        if (filePathList.Contains(filePath))
                        {
                            return BadRequest("栏目修改失败，栏目页面路径已存在！");
                        }
                    }
                    channelInfo.FilePath = filePath;
                }

                if (request.IsPostExists(nameof(Channel.ChannelFilePathRule)))
                {
                    var channelFilePathRule = request.GetPostString(nameof(Channel.ChannelFilePathRule));

                    if (!string.IsNullOrEmpty(channelFilePathRule))
                    {
                        var filePathRule = channelFilePathRule.Replace("|", string.Empty);
                        if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                        {
                            return BadRequest("栏目页面命名规则不符合系统要求！");
                        }
                        if (PathUtils.IsDirectoryPath(filePathRule))
                        {
                            return BadRequest("栏目页面命名规则必须包含生成文件的后缀！");
                        }
                    }

                    channelInfo.ChannelFilePathRule = channelFilePathRule;
                }

                if (request.IsPostExists(nameof(Channel.ContentFilePathRule)))
                {
                    var contentFilePathRule = request.GetPostString(nameof(Channel.ContentFilePathRule));

                    if (!string.IsNullOrEmpty(contentFilePathRule))
                    {
                        var filePathRule = contentFilePathRule.Replace("|", string.Empty);
                        if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                        {
                            return BadRequest("内容页面命名规则不符合系统要求！");
                        }
                        if (PathUtils.IsDirectoryPath(filePathRule))
                        {
                            return BadRequest("内容页面命名规则必须包含生成文件的后缀！");
                        }
                    }

                    channelInfo.ContentFilePathRule = contentFilePathRule;
                }

                if (request.IsPostExists(nameof(Channel.GroupNames)))
                {
                    channelInfo.GroupNames = StringUtils.GetStringList(request.GetPostString(nameof(Channel.GroupNames)));
                }

                if (request.IsPostExists(nameof(Channel.ImageUrl)))
                {
                    channelInfo.ImageUrl = request.GetPostString(nameof(Channel.ImageUrl));
                }

                if (request.IsPostExists(nameof(Channel.Content)))
                {
                    channelInfo.Content = request.GetPostString(nameof(Channel.Content));
                }

                if (request.IsPostExists(nameof(Channel.Keywords)))
                {
                    channelInfo.Keywords = request.GetPostString(nameof(Channel.Keywords));
                }

                if (request.IsPostExists(nameof(Channel.Description)))
                {
                    channelInfo.Description = request.GetPostString(nameof(Channel.Description));
                }

                if (request.IsPostExists(nameof(Channel.LinkUrl)))
                {
                    channelInfo.LinkUrl = request.GetPostString(nameof(Channel.LinkUrl));
                }

                if (request.IsPostExists(nameof(Channel.LinkType)))
                {
                    channelInfo.LinkType = TranslateUtils.ToEnum(request.GetPostString(nameof(Channel.LinkType)), LinkType.None);
                }

                if (request.IsPostExists(nameof(Channel.ChannelTemplateId)))
                {
                    channelInfo.ChannelTemplateId = request.GetPostInt(nameof(Channel.ChannelTemplateId));
                }

                if (request.IsPostExists(nameof(Channel.ContentTemplateId)))
                {
                    channelInfo.ContentTemplateId = request.GetPostInt(nameof(Channel.ContentTemplateId));
                }

                await DataProvider.ChannelRepository.UpdateAsync(channelInfo);

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteChannel)]
        public async Task<IHttpActionResult> Delete(int siteId, int channelId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeChannels) ||
                              request.IsAdminLoggin &&
                              await request.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                                  Constants.ChannelPermissions.ChannelDelete);
                if (!isAuth) return Unauthorized();

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
                await DataProvider.ContentRepository.UpdateTrashContentsByChannelIdAsync(siteId, channelId, tableName);
                await DataProvider.ChannelRepository.DeleteAsync(siteId, channelId);

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteChannel)]
        public async Task<IHttpActionResult> Get(int siteId, int channelId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeChannels) ||
                              request.IsAdminLoggin;
                if (!isAuth) return Unauthorized();

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                channelInfo.Children = await ChannelManager.GetChildrenAsync(siteId, channelId);

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteSite)]
        public async Task<IHttpActionResult> GetChannels(int siteId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeChannels) ||
                             request.IsAdminLoggin;
                if (!isAuth) return Unauthorized();

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfoList = await ChannelManager.GetChannelListAsync(siteId);

                var dictInfoList = new List<IDictionary<string, object>>();
                foreach (var channelInfo in channelInfoList)
                {
                    dictInfoList.Add(channelInfo.ToDictionary());
                }

                return Ok(new
                {
                    Value = dictInfoList
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
