using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{

    [RoutePrefix("pages/cms/editor/editor")]
    public partial class PagesEditorController : ApiController
    {
        private const string Route = "";
        private const string RoutePreview = "actions/preview";

        [HttpGet, Route(Route)]
        public async Task<ConfigResult> Get([FromUri]ConfigRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<ConfigResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ConfigResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return Request.BadRequest<ConfigResult>("指定的栏目不存在");

            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(site.Id);

            var allStyles = await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channel);
            var styles = allStyles.Where(style =>
                    !string.IsNullOrEmpty(style.DisplayName) && !StringUtils.ContainsIgnoreCase(ContentAttribute.MetadataAttributes.Value, style.AttributeName));

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
            if (request.ContentId != 0)
            {
                content = await DataProvider.ContentRepository.GetAsync(site, channel, request.ContentId);
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

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
                Content = content
            };
        }

        [HttpPut, Route(Route)]
        public async Task<BoolResult> Update([FromBody] SaveRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);

            var content = request.Content;
            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.LastEditAdminId = auth.AdminId;
            content.LastEditDate = DateTime.Now;

            await DataProvider.ContentRepository.UpdateAsync(site, channel, content);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RoutePreview)]
        public async Task<PreviewResult> Preview([FromBody] SaveRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<PreviewResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);

            if (site == null)
            {
                return Request.BadRequest<PreviewResult>("指定的站点不存在");
            }
            if (channel == null)
            {
                return Request.BadRequest<PreviewResult>("指定的栏目不存在");
            }

            //var styleList = await TableStyleManager.GetContentStyleListAsync(site, channel);

            //var dict = BackgroundInputTypeParser.SaveAttributesAsync(site, styleList, request.Content, ContentAttribute.AllAttributes.Value);
            //var contentInfo = new Content(dict)
            //{
            //    ChannelId = channelId,
            //    SiteId = siteId,
            //    AddUserName = AuthRequest.AdminName,
            //    LastEditUserName = AuthRequest.AdminName,
            //    LastEditDate = DateTime.Now
            //};

            var content = request.Content;
            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.AdminId = auth.AdminId;
            content.Checked = true;

            content.Id = await DataProvider.ContentRepository.InsertPreviewAsync(site, channel, content);

            return new PreviewResult()
            {
                Url = ApiRoutePreview.GetContentPreviewUrl(request.SiteId, request.ChannelId, request.ContentId, content.Id)
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
