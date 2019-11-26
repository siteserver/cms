using System;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentAddHandler : BaseHandler
    {
        public static string GetRedirectUrl(int siteId, int channelId, int contentId)
        {
            return PageUtils.GetCmsWebHandlerUrl(siteId, nameof(PageContentAddHandler), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()}
            });
        }

        protected override object Process()
        {
            var siteId = AuthRequest.SiteId;
            var channelId = AuthRequest.ChannelId;
            var contentId = AuthRequest.ContentId;

            var site = DataProvider.SiteDao.GetAsync(siteId).GetAwaiter().GetResult();
            var channelInfo = ChannelManager.GetChannelAsync(siteId, channelId).GetAwaiter().GetResult();
            var tableName = ChannelManager.GetTableNameAsync(site, channelInfo).GetAwaiter().GetResult();
            var styleList = TableStyleManager.GetContentStyleListAsync(site, channelInfo).GetAwaiter().GetResult();

            var form = AuthRequest.HttpRequest.Form;

            var dict = BackgroundInputTypeParser.SaveAttributesAsync(site, styleList, form, ContentAttribute.AllAttributes.Value).GetAwaiter().GetResult();
            var contentInfo = new Content(dict)
            {
                ChannelId = channelId,
                SiteId = siteId,
                AddUserName = AuthRequest.AdminName,
                LastEditUserName = AuthRequest.AdminName,
                LastEditDate = DateTime.Now
            };

            //contentInfo.GroupNameCollection = ControlUtils.SelectedItemsValueToStringCollection(CblContentGroups.Items);
            var tagCollection = ContentTagUtils.ParseTagsString(form["TbTags"]);

            contentInfo.Title = form["TbTitle"];
            var formatString = TranslateUtils.ToBool(form[ContentAttribute.Title + "_formatStrong"]);
            var formatEm = TranslateUtils.ToBool(form[ContentAttribute.Title + "_formatEM"]);
            var formatU = TranslateUtils.ToBool(form[ContentAttribute.Title + "_formatU"]);
            var formatColor = form[ContentAttribute.Title + "_formatColor"];
            var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);
            contentInfo.Set(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title), theFormatString);
            //foreach (ListItem listItem in CblContentAttributes.Items)
            //{
            //    var value = listItem.Selected.ToString();
            //    var attributeName = listItem.Value;
            //    contentInfo.Set(attributeName, value);
            //}
            //contentInfo.LinkUrl = TbLinkUrl.Text;
            contentInfo.AddDate = TranslateUtils.ToDateTime(form["TbAddDate"]);
            contentInfo.Checked = false;
            contentInfo.Tags = TranslateUtils.ObjectCollectionToString(tagCollection, " ");

            foreach (var service in PluginManager.GetServicesAsync().GetAwaiter().GetResult())
            {
                try
                {
                    service.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, contentInfo.Id, TranslateUtils.ToDictionary(form), contentInfo));
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(IService.ContentFormSubmit)).GetAwaiter().GetResult();
                }
            }

            contentInfo.Id = DataProvider.ContentDao.InsertPreviewAsync(tableName, site, channelInfo, contentInfo).GetAwaiter().GetResult();

            return new
            {
                previewUrl = ApiRoutePreview.GetContentPreviewUrl(siteId, channelId, contentId, contentInfo.Id)
            };
        }
    }
}
