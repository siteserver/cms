using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Provider;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteDownloadsChannels : BasePageCms
    {
        public DropDownList DdlSiteId;
        public Repeater RptContents;
        public SqlPager SpContents;

        public string StrArray { get; set; }

        private readonly Hashtable _nodeNameNavigations = new Hashtable();
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _contentIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableDownload = new Hashtable();
        private const string YTypeDownload = "YType_Download";

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteDownloadsChannels), new NameValueCollection
            {
                {"siteID", siteId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;

            SpContents.SelectCommand = DataProvider.ContentDao.GetSqlStringByDownloads(SiteInfo.TableName, SiteId);

            SpContents.SortField = ContentDao.SortFieldName;
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Settings.Chart);

            DdlSiteId.Items.Add(new ListItem("<<全部站点>>", "0"));
            var siteIdList = SiteManager.GetSiteIdListOrderByLevel();
            foreach (var siteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                DdlSiteId.Items.Add(new ListItem(siteInfo.SiteName, siteId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlSiteId, SiteId.ToString());

            SpContents.DataBind();

            foreach (var contentId in _contentIdList)
            {
                var yValueDownload = GetYHashtable(contentId);
                var xValue = GetXHashtable(contentId);
                if (xValue.Length > 10) xValue = xValue.Substring(0, 10);

                StrArray += $@"
xArrayDownload.push('{xValue}');
                yArrayDownload.push('{yValueDownload}');";
            }
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var siteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);
            PageUtils.Redirect(siteId > 0
                ? GetRedirectUrl(siteId)
                : PageAnalysisSiteDownloads.GetRedirectUrl());
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlFileUrl = (Literal)e.Item.FindControl("ltlFileUrl");

            var contentInfo = new ContentInfo(e.Item.DataItem);

            ltlItemTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);

            string nodeNameNavigation;
            if (!_nodeNameNavigations.ContainsKey(contentInfo.ChannelId))
            {
                nodeNameNavigation = ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId);
                _nodeNameNavigations.Add(contentInfo.ChannelId, nodeNameNavigation);
            }
            else
            {
                nodeNameNavigation = _nodeNameNavigations[contentInfo.ChannelId] as string;
            }
            ltlChannel.Text = nodeNameNavigation;

            var fileUrl = contentInfo.GetString(BackgroundContentAttribute.FileUrl);
            if (!string.IsNullOrEmpty(fileUrl))
            {
                ltlFileUrl.Text =
                $@"<a href=""{PageUtility.ParseNavigationUrl(SiteInfo, fileUrl, true)}"" target=""_blank"">{fileUrl}</a>";
            }

            //x轴信息
            SetXHashtable(contentInfo.Id, contentInfo.Title);
            //y轴信息
            SetYHashtable(contentInfo.Id, CountManager.GetCount(SiteInfo.TableName, contentInfo.Id.ToString(), ECountType.Download));
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteDownloadsChannels), new NameValueCollection
                    {
                        {"SiteId", SiteId.ToString()}
                    });
                }
                return _pageUrl;
            }
        }

        private void SetXHashtable(int contentId, string title)
        {
            if (!_xHashtable.ContainsKey(contentId))
            {
                _xHashtable.Add(contentId, title);
            }
            if (!_contentIdList.Contains(contentId))
            {
                _contentIdList.Add(contentId);
            }
        }

        private string GetXHashtable(int contentId)
        {
            return _xHashtable.ContainsKey(contentId) ? _xHashtable[contentId].ToString() : string.Empty;
        }

        private void SetYHashtable(int contentId, int value)
        {

            if (!_yHashtableDownload.ContainsKey(contentId))
            {
                _yHashtableDownload.Add(contentId, value);
            }
            else
            {
                var num = TranslateUtils.ToInt(_yHashtableDownload[contentId].ToString());
                _yHashtableDownload[contentId] = num + value;
            }
            SetVertical(YTypeDownload, value);

            SetHorizental(contentId, value);
        }

        private string GetYHashtable(int contentId)
        {
            if (!_yHashtableDownload.ContainsKey(contentId)) return "0";
            var num = TranslateUtils.ToInt(_yHashtableDownload[contentId].ToString());
            return num.ToString();
        }

        private void SetHorizental(int contentId, int num)
        {
            if (_horizentalHashtable[contentId] == null)
            {
                _horizentalHashtable[contentId] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtable[contentId];
                _horizentalHashtable[contentId] = totalNum + num;
            }
        }

        private void SetVertical(string type, int num)
        {
            if (_verticalHashtable[type] == null)
            {
                _verticalHashtable[type] = num;
            }
            else
            {
                var totalNum = (int)_verticalHashtable[type];
                _verticalHashtable[type] = totalNum + num;
            }
        }
    }
}
