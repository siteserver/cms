using System;
using System.Collections.Generic;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/channels")]
    public class ChannelsController : ApiController
    {
        private const string RouteSite = "{siteId:int}";
        private const string RouteChannel = "{siteId:int}/{channelId:int}";

        [OpenApiOperation("获取栏目列表 API", "https://sscms.com/docs/v6/api/guide/channels/list.html")]
        [HttpGet, Route(RouteSite)]
        public IHttpActionResult List(int siteId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeChannels) ||
                             request.IsAdminLoggin;
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var root = ChannelManager.GetChannelInfo(siteId, siteId);
                var channelIdList = ChannelManager.GetChannelIdList(root, EScopeType.Children);

                var dictInfoList = new List<Dictionary<string, object>>();
                foreach (var channelId in channelIdList)
                {
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    dictInfoList.Add(channelInfo.ToDictionary());
                }

                return Ok(new
                {
                    Value = dictInfoList
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("获取栏目 API", "https://sscms.com/docs/v6/api/guide/channels/get.html")]
        [HttpGet, Route(RouteChannel)]
        public IHttpActionResult Get(int siteId, int channelId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeChannels) ||
                             request.IsAdminLoggin;
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                channelInfo.Children = ChannelManager.GetChildren(siteId, channelId);

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("新增栏目 API", "https://sscms.com/docs/v6/api/guide/channels/create.html")]
        [HttpPost, Route(RouteSite)]
        public IHttpActionResult Create(int siteId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var parentId = request.GetPostInt(ChannelAttribute.ParentId, siteId);

                var isAuth = request.IsApiAuthenticated &&
                              AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeChannels) ||
                              request.IsAdminLoggin &&
                              request.AdminPermissions.HasChannelPermissions(siteId, parentId,
                                  ConfigManager.ChannelPermissions.ChannelAdd);
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var contentModelPluginId = request.GetPostString(ChannelAttribute.ContentModelPluginId);
                var contentRelatedPluginIds = request.GetPostString(ChannelAttribute.ContentRelatedPluginIds);

                var channelName = request.GetPostString(ChannelAttribute.ChannelName);
                var indexName = request.GetPostString(ChannelAttribute.IndexName);
                var filePath = request.GetPostString(ChannelAttribute.FilePath);
                var channelFilePathRule = request.GetPostString(ChannelAttribute.ChannelFilePathRule);
                var contentFilePathRule = request.GetPostString(ChannelAttribute.ContentFilePathRule);
                var groupNameCollection = request.GetPostString(ChannelAttribute.GroupNameCollection);
                var imageUrl = request.GetPostString(ChannelAttribute.ImageUrl);
                var content = request.GetPostString(ChannelAttribute.Content);
                var keywords = request.GetPostString(ChannelAttribute.Keywords);
                var description = request.GetPostString(ChannelAttribute.Description);
                var linkUrl = request.GetPostString(ChannelAttribute.LinkUrl);
                var linkType = request.GetPostString(ChannelAttribute.LinkType);
                var channelTemplateId = request.GetPostInt(ChannelAttribute.ChannelTemplateId);
                var contentTemplateId = request.GetPostInt(ChannelAttribute.ContentTemplateId);

                var channelInfo = new ChannelInfo
                {
                    SiteId = siteId,
                    ParentId = parentId,
                    ContentModelPluginId = contentModelPluginId,
                    ContentRelatedPluginIds = contentRelatedPluginIds
                };

                if (!string.IsNullOrEmpty(indexName))
                {
                    var indexNameList = DataProvider.ChannelDao.GetIndexNameList(siteId);
                    if (indexNameList.IndexOf(indexName) != -1)
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

                    var filePathList = DataProvider.ChannelDao.GetAllFilePathBySiteId(siteId);
                    if (filePathList.IndexOf(filePath) != -1)
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

                //var parentChannelInfo = ChannelManager.GetChannelInfo(siteId, parentId);
                //var styleInfoList = TableStyleManager.GetChannelStyleInfoList(parentChannelInfo);
                //var extendedAttributes = BackgroundInputTypeParser.SaveAttributes(siteInfo, styleInfoList, Request.Form, null);
                channelInfo.Additional.Load(request.GetPostObject<Dictionary<string, object>>());
                //foreach (string key in attributes)
                //{
                //    channelInfo.Additional.SetExtendedAttribute(key, attributes[key]);
                //}

                channelInfo.ChannelName = channelName;
                channelInfo.IndexName = indexName;
                channelInfo.FilePath = filePath;
                channelInfo.ChannelFilePathRule = channelFilePathRule;
                channelInfo.ContentFilePathRule = contentFilePathRule;

                channelInfo.GroupNameCollection = groupNameCollection;
                channelInfo.ImageUrl = imageUrl;
                channelInfo.Content = content;
                channelInfo.Keywords = keywords;
                channelInfo.Description = description;
                channelInfo.LinkUrl = linkUrl;
                channelInfo.LinkType = linkType;
                channelInfo.ChannelTemplateId = channelTemplateId;
                channelInfo.ContentTemplateId = contentTemplateId;

                channelInfo.AddDate = DateTime.Now;
                channelInfo.Id = DataProvider.ChannelDao.Insert(channelInfo);
                //栏目选择投票样式后，内容

                CreateManager.CreateChannel(siteId, channelInfo.Id);

                request.AddSiteLog(siteId, "添加栏目", $"栏目:{channelName}");

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("修改栏目 API", "https://sscms.com/docs/v6/api/guide/channels/update.html")]
        [HttpPut, Route(RouteChannel)]
        public IHttpActionResult Update(int siteId, int channelId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                              AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeChannels) ||
                              request.IsAdminLoggin &&
                              request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                  ConfigManager.ChannelPermissions.ChannelEdit);
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                channelInfo.Additional.Load(request.GetPostObject<Dictionary<string, object>>());

                if (request.IsPostExists(ChannelAttribute.ChannelName))
                {
                    channelInfo.ChannelName = request.GetPostString(ChannelAttribute.ChannelName);
                }

                if (request.IsPostExists(ChannelAttribute.IndexName))
                {
                    var indexName = request.GetPostString(ChannelAttribute.IndexName);
                    if (!channelInfo.IndexName.Equals(indexName) && !string.IsNullOrEmpty(indexName))
                    {
                        var indexNameList = DataProvider.ChannelDao.GetIndexNameList(siteId);
                        if (indexNameList.IndexOf(indexName) != -1)
                        {
                            return BadRequest("栏目属性修改失败，栏目索引已存在！");
                        }
                    }
                    channelInfo.IndexName = indexName;
                }

                if (request.IsPostExists(ChannelAttribute.ContentModelPluginId))
                {
                    var contentModelPluginId = request.GetPostString(ChannelAttribute.ContentModelPluginId);
                    if (channelInfo.ContentModelPluginId != contentModelPluginId)
                    {
                        channelInfo.ContentModelPluginId = contentModelPluginId;
                    }
                }

                if (request.IsPostExists(ChannelAttribute.ContentRelatedPluginIds))
                {
                    channelInfo.ContentRelatedPluginIds = request.GetPostString(ChannelAttribute.ContentRelatedPluginIds);
                }

                if (request.IsPostExists(ChannelAttribute.FilePath))
                {
                    var filePath = request.GetPostString(ChannelAttribute.FilePath);
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

                        var filePathList = DataProvider.ChannelDao.GetAllFilePathBySiteId(siteId);
                        if (filePathList.IndexOf(filePath) != -1)
                        {
                            return BadRequest("栏目修改失败，栏目页面路径已存在！");
                        }
                    }
                    channelInfo.FilePath = filePath;
                }

                if (request.IsPostExists(ChannelAttribute.ChannelFilePathRule))
                {
                    var channelFilePathRule = request.GetPostString(ChannelAttribute.ChannelFilePathRule);

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

                if (request.IsPostExists(ChannelAttribute.ContentFilePathRule))
                {
                    var contentFilePathRule = request.GetPostString(ChannelAttribute.ContentFilePathRule);

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

                if (request.IsPostExists(ChannelAttribute.GroupNameCollection))
                {
                    channelInfo.GroupNameCollection = request.GetPostString(ChannelAttribute.GroupNameCollection);
                }

                if (request.IsPostExists(ChannelAttribute.ImageUrl))
                {
                    channelInfo.ImageUrl = request.GetPostString(ChannelAttribute.ImageUrl);
                }

                if (request.IsPostExists(ChannelAttribute.Content))
                {
                    channelInfo.Content = request.GetPostString(ChannelAttribute.Content);
                }

                if (request.IsPostExists(ChannelAttribute.Keywords))
                {
                    channelInfo.Keywords = request.GetPostString(ChannelAttribute.Keywords);
                }

                if (request.IsPostExists(ChannelAttribute.Description))
                {
                    channelInfo.Description = request.GetPostString(ChannelAttribute.Description);
                }

                if (request.IsPostExists(ChannelAttribute.LinkUrl))
                {
                    channelInfo.LinkUrl = request.GetPostString(ChannelAttribute.LinkUrl);
                }

                if (request.IsPostExists(ChannelAttribute.LinkType))
                {
                    channelInfo.LinkType = request.GetPostString(ChannelAttribute.LinkType);
                }

                if (request.IsPostExists(ChannelAttribute.ChannelTemplateId))
                {
                    channelInfo.ChannelTemplateId = request.GetPostInt(ChannelAttribute.ChannelTemplateId);
                }

                if (request.IsPostExists(ChannelAttribute.ContentTemplateId))
                {
                    channelInfo.ContentTemplateId = request.GetPostInt(ChannelAttribute.ContentTemplateId);
                }

                DataProvider.ChannelDao.Update(channelInfo);

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("删除栏目 API", "https://sscms.com/docs/v6/api/guide/channels/delete.html")]
        [HttpDelete, Route(RouteChannel)]
        public IHttpActionResult Delete(int siteId, int channelId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                              AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeChannels) ||
                              request.IsAdminLoggin &&
                              request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                  ConfigManager.ChannelPermissions.ChannelDelete);
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                DataProvider.ContentDao.UpdateTrashContentsByChannelId(siteId, channelId, tableName);
                DataProvider.ChannelDao.Delete(siteId, channelId);

                return Ok(new
                {
                    Value = channelInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
