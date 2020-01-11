using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{

    [RoutePrefix("pages/cms/editor")]
    public partial class PagesEditorController : ApiController
    {
        private const string Route = "";
        private const string RoutePreview = "actions/preview";

        [HttpGet, Route(Route)]
        public async Task<ConfigResult> Get([FromUri]ConfigRequest req)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(req.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(req.SiteId, req.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<ConfigResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(req.SiteId);
            var channel = await ChannelManager.GetChannelAsync(req.SiteId, req.ChannelId);

            if (site == null)
            {
                return Request.BadRequest<ConfigResult>("指定的站点不存在");
            }
            if (channel == null)
            {
                return Request.BadRequest<ConfigResult>("指定的栏目不存在");
            }

            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await DataProvider.ContentTagRepository.GetTagListAsync(site.Id);

            var allStyles = await TableStyleManager.GetContentStyleListAsync(site, channel);
            var styles = allStyles.Where(style =>
                    !StringUtils.ContainsIgnoreCase(ContentAttribute.MetadataAttributes.Value, style.AttributeName));

            var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissionsImpl, site, site.Id);
            var checkedLevels = CheckManager.GetCheckedLevelOptions(site, userIsChecked, userCheckedLevel, true);

            var content = new Content
            {
                Id = 0,
                SiteId = site.Id,
                ChannelId = channel.Id,
                AddDate = DateTime.Now,
                CheckedLevel = site.CheckContentDefaultLevel
            };
            if (req.ContentId != 0)
            {
                content = await DataProvider.ContentRepository.GetAsync(site, channel, req.ContentId);
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            //var siteOptions = await DataProvider.SiteRepository.GetSiteOptionsAsync(0);
            //var channelOptions = await ChannelManager.GetCascadeChildrenAsync(site.Id);

            return new ConfigResult
            {
                User = auth.User,
                Config = config,
                Site = site,
                Channel = channel,
                GroupNames = groupNames,
                TagNames = tagNames,
                Styles = styles,
                CheckedLevels = checkedLevels,
                Content = content,
                //SiteOptions = siteOptions,
                //ChannelOptions = channelOptions
            };
        }

        [HttpPut, Route(Route)]
        public async Task<PreviewResult> Save([FromBody] SaveRequest req)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(req.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(req.SiteId, req.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<PreviewResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(req.SiteId);
            var channel = await ChannelManager.GetChannelAsync(req.SiteId, req.ChannelId);

            if (site == null)
            {
                return Request.BadRequest<PreviewResult>("指定的站点不存在");
            }
            if (channel == null)
            {
                return Request.BadRequest<PreviewResult>("指定的栏目不存在");
            }

            var content = req.Content;
            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.AdminId = auth.AdminId;
            content.Checked = true;

            content.Id = await DataProvider.ContentRepository.InsertPreviewAsync(site, channel, content);

            return new PreviewResult()
            {
                Url = ApiRoutePreview.GetContentPreviewUrl(req.SiteId, req.ChannelId, req.ContentId, content.Id)
            };
        }

        [HttpPost, Route(RoutePreview)]
        public async Task<PreviewResult> Preview([FromBody] SaveRequest req)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(req.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(req.SiteId, req.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<PreviewResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(req.SiteId);
            var channel = await ChannelManager.GetChannelAsync(req.SiteId, req.ChannelId);

            if (site == null)
            {
                return Request.BadRequest<PreviewResult>("指定的站点不存在");
            }
            if (channel == null)
            {
                return Request.BadRequest<PreviewResult>("指定的栏目不存在");
            }

            //var styleList = await TableStyleManager.GetContentStyleListAsync(site, channel);

            //var dict = BackgroundInputTypeParser.SaveAttributesAsync(site, styleList, req.Content, ContentAttribute.AllAttributes.Value);
            //var contentInfo = new Content(dict)
            //{
            //    ChannelId = channelId,
            //    SiteId = siteId,
            //    AddUserName = AuthRequest.AdminName,
            //    LastEditUserName = AuthRequest.AdminName,
            //    LastEditDate = DateTime.Now
            //};

            var content = req.Content;
            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.AdminId = auth.AdminId;
            content.Checked = true;

            content.Id = await DataProvider.ContentRepository.InsertPreviewAsync(site, channel, content);

            return new PreviewResult()
            {
                Url = ApiRoutePreview.GetContentPreviewUrl(req.SiteId, req.ChannelId, req.ContentId, content.Id)
            };

            ////contentInfo.GroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items);
            //var tagCollection = ContentTagUtils.ParseTagsString(form["TbTags"]);

            //contentInfo.Title = form["TbTitle"];
            //var formatString = TranslateUtils.ToBool(form[ContentAttribute.Title + "_formatStrong"]);
            //var formatEm = TranslateUtils.ToBool(form[ContentAttribute.Title + "_formatEM"]);
            //var formatU = TranslateUtils.ToBool(form[ContentAttribute.Title + "_formatU"]);
            //var formatColor = form[ContentAttribute.Title + "_formatColor"];
            //var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);
            //contentInfo.Set(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title), theFormatString);
            ////foreach (ListItem listItem in CblContentAttributes.Items)
            ////{
            ////    var value = listItem.Selected.ToString();
            ////    var attributeName = listItem.Value;
            ////    contentInfo.Set(attributeName, value);
            ////}
            ////contentInfo.LinkUrl = TbLinkUrl.Text;
            //contentInfo.AddDate = TranslateUtils.ToDateTime(form["TbAddDate"]);
            //contentInfo.Checked = false;
            //contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

            //foreach (var service in PluginManager.GetServicesAsync())
            //{
            //    try
            //    {
            //        service.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, contentInfo.Id, TranslateUtils.ToDictionary(form), contentInfo));
            //    }
            //    catch (Exception ex)
            //    {
            //        LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(IService.ContentFormSubmit));
            //    }
            //}

            //contentInfo.Id = DataProvider.ContentRepository.InsertPreviewAsync(site, channelInfo, contentInfo);

            //return new
            //{
            //    previewUrl = ApiRoutePreview.GetContentPreviewUrl(siteId, channelId, contentId, contentInfo.Id)
            //};
        }
    }
}
