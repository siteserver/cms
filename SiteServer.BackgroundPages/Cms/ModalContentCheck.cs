using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentCheck : BasePageCms
    {
        protected override bool IsSinglePage => true;
        public Literal LtlTitles;
        public DropDownList DdlCheckType;
        public DropDownList DdlTranslateChannelId;
        public TextBox TbCheckReasons;

        private Dictionary<int, List<int>> _idsDictionary = new Dictionary<int, List<int>>();
        private string _returnUrl;

        public static string GetOpenWindowString(int siteId, int channelId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("审核内容", PageUtils.GetCmsUrl(siteId, nameof(ModalContentCheck), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "contentIdCollection", "请选择需要审核的内容！", 560, 550);
        }

        public static string GetOpenWindowStringForMultiChannels(int siteId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("审核内容", PageUtils.GetCmsUrl(siteId, nameof(ModalContentCheck), new NameValueCollection
            {
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "IDsCollection", "请选择需要审核的内容！", 560, 550);
        }

        public static string GetOpenWindowString(int siteId, int channelId, int contentId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("审核内容", PageUtils.GetCmsUrl(siteId, nameof(ModalContentCheck), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"contentIdCollection", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 560, 550);
        }

        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalContentCheck), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)},
                {"contentIdCollection", contentId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "ReturnUrl");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            _idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);

            if (IsPostBack) return;

            var titles = new StringBuilder();
            foreach (var channelId in _idsDictionary.Keys)
            {
                var tableName = DataProvider.ChannelRepository.GetTableNameAsync(Site, channelId).GetAwaiter().GetResult();
                var contentIdList = _idsDictionary[channelId];
                foreach (var contentId in contentIdList)
                {
                    var title = DataProvider.ContentRepository.GetValueAsync(tableName, contentId, ContentAttribute.Title).GetAwaiter().GetResult();
                    titles.Append(title + "<br />");
                }
            }

            if (!string.IsNullOrEmpty(LtlTitles.Text))
            {
                titles.Length -= 6;
            }
            LtlTitles.Text = titles.ToString();

            var checkedLevel = 5;
            var isChecked = true;

            foreach (var channelId in _idsDictionary.Keys)
            {
                var (isCheckedByChannelId, checkedLevelByChannelId) = CheckManager.GetUserCheckLevelAsync(AuthRequest.AdminPermissionsImpl, Site, channelId).GetAwaiter().GetResult();
                if (checkedLevel > checkedLevelByChannelId)
                {
                    checkedLevel = checkedLevelByChannelId;
                }
                if (!isCheckedByChannelId)
                {
                    isChecked = false;
                }
            }

            CheckManager.LoadContentLevelToCheck(DdlCheckType, Site, isChecked, checkedLevel);

            var listItem = new ListItem("<保持原栏目不变>", "0");
            DdlTranslateChannelId.Items.Add(listItem);

            DataProvider.ChannelRepository.AddListItemsForAddContentAsync(DdlTranslateChannelId.Items, Site, true, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var checkedLevel = TranslateUtils.ToIntWithNegative(DdlCheckType.SelectedValue);

            var isChecked = checkedLevel >= Site.CheckContentLevel;

            var contentInfoListToCheck = new List<Content>();
            var idsDictionaryToCheck = new Dictionary<int, List<int>>();
            foreach (var channelId in _idsDictionary.Keys)
            {
                var channelInfo = DataProvider.ChannelRepository.GetAsync(channelId).GetAwaiter().GetResult();
                var contentIdList = _idsDictionary[channelId];
                var contentIdListToCheck = new List<int>();

                var (isCheckedOfUser, checkedLevelOfUser) = CheckManager.GetUserCheckLevelAsync(AuthRequest.AdminPermissionsImpl, Site, channelId).GetAwaiter().GetResult();

                foreach (var contentId in contentIdList)
                {
                    var contentInfo = DataProvider.ContentRepository.GetAsync(Site, channelInfo, contentId).GetAwaiter().GetResult();
                    if (contentInfo != null)
                    {
                        if (CheckManager.IsCheckable(contentInfo.Checked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                        {
                            contentInfoListToCheck.Add(contentInfo);
                            contentIdListToCheck.Add(contentId);
                        }

                        //DataProvider.ContentRepository.Update(Site, channel, contentInfo);

                        //CreateManager.CreateContent(SiteId, contentInfo.ChannelId, contentId);
                        //CreateManager.TriggerContentChangedEvent(SiteId, contentInfo.ChannelId);
                    }
                }
                if (contentIdListToCheck.Count > 0)
                {
                    idsDictionaryToCheck[channelId] = contentIdListToCheck;
                }
            }

            if (contentInfoListToCheck.Count == 0)
            {
                LayerUtils.CloseWithoutRefresh(Page, "alert('您的审核权限不足，无法审核所选内容！');");
                return;
            }

            var translateChannelId = TranslateUtils.ToInt(DdlTranslateChannelId.SelectedValue);

            foreach (var channelId in idsDictionaryToCheck.Keys)
            {
                var channelInfo = DataProvider.ChannelRepository.GetAsync(channelId).GetAwaiter().GetResult();
                var contentIdList = idsDictionaryToCheck[channelId];
                DataProvider.ContentRepository.UpdateIsCheckedAsync(Site, channelInfo, contentIdList, translateChannelId, AuthRequest.AdminId, isChecked, checkedLevel, TbCheckReasons.Text).GetAwaiter().GetResult();
            }

            AuthRequest.AddSiteLogAsync(SiteId, SiteId, 0, "设置内容状态为" + DdlCheckType.SelectedItem.Text, TbCheckReasons.Text).GetAwaiter().GetResult();

            foreach (var channelId in idsDictionaryToCheck.Keys)
            {
                var contentIdList = _idsDictionary[channelId];
                if (contentIdList != null)
                {
                    foreach (var contentId in contentIdList)
                    {
                        CreateManager.CreateContentAsync(SiteId, channelId, contentId).GetAwaiter().GetResult();
                        CreateManager.TriggerContentChangedEventAsync(SiteId, channelId).GetAwaiter().GetResult();
                    }
                }
            }

            LayerUtils.CloseAndRedirect(Page, _returnUrl);
        }
    }
}
