using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteChannels : BasePageCms
    {
        public DropDownList DdlSiteId;
        public DateTimeTextBox TbStartDate;
        public DateTimeTextBox TbEndDate;
        public Repeater RptChannels;

        public string StrArray1 { get; set; }

        private readonly List<int> _siteIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableNew = new Hashtable();
        private readonly Hashtable _yHashtableUpdate = new Hashtable();
        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";
        private const int YTypeOther = -1;
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private NameValueCollection _additional;
        private DateTime _begin;
        private DateTime _end;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteChannels), new NameValueCollection
            {
                {"siteId", siteId.ToString()}
            });
        }

        public static string GetRedirectUrl(int siteId, string startDate, string endDate)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteChannels), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"startDate", startDate},
                {"endDate", endDate}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (string.IsNullOrEmpty(AuthRequest.GetQueryString("StartDate")))
            {
                _begin = DateTime.Now.AddMonths(-1);
                _end = DateTime.Now;
            }
            else
            {
                _begin = TranslateUtils.ToDateTime(AuthRequest.GetQueryString("StartDate"));
                _end = TranslateUtils.ToDateTime(AuthRequest.GetQueryString("EndDate"));
            }

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Chart);

            DdlSiteId.Items.Add(new ListItem("<<全部站点>>", "0"));
            var siteIdList = SiteManager.GetSiteIdListOrderByLevel();
            foreach (var siteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                DdlSiteId.Items.Add(new ListItem(siteInfo.SiteName, siteId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlSiteId, SiteId.ToString());

            TbStartDate.Text = DateUtils.GetDateAndTimeString(_begin);
            TbEndDate.Text = DateUtils.GetDateAndTimeString(_end);

            _additional = new NameValueCollection
            {
                ["StartDate"] = TbStartDate.Text,
                ["EndDate"] = TbEndDate.Text
            };

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, string.Empty, ELoadingType.SiteAnalysis, _additional));

            BindGrid();

            foreach (var key in _siteIdList)
            {
                var yValueNew = GetYHashtable(key, YTypeNew);
                var yValueUpdate = GetYHashtable(key, YTypeUpdate);

                StrArray1 += $@"
xArrayNew.push('{GetXHashtable(key)}');
yArrayNew.push('{yValueNew}');
yArrayUpdate.push('{yValueUpdate}');";
            }
        }

        public void BindGrid()
        {
            var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(SiteId, SiteId), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty);
            foreach (var channelId in channelIdList)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
                var tableName = ChannelManager.GetTableName(SiteInfo, channelId);

                SetXHashtable(channelId, nodeInfo.ChannelName);

                SetYHashtable(channelId, DataProvider.ContentDao.GetCountOfContentAdd(tableName, SiteId, nodeInfo.Id, EScopeType.All, TranslateUtils.ToDateTime(_additional["StartDate"]), TranslateUtils.ToDateTime(_additional["EndDate"]), string.Empty, ETriState.All), YTypeNew);
                SetYHashtable(channelId, DataProvider.ContentDao.GetCountOfContentUpdate(tableName, SiteId, nodeInfo.Id, EScopeType.All, TranslateUtils.ToDateTime(_additional["StartDate"]), TranslateUtils.ToDateTime(_additional["EndDate"]), string.Empty), YTypeUpdate);
            }

            RptChannels.DataSource = channelIdList;
            RptChannels.ItemDataBound += RptChannels_ItemDataBound;
            RptChannels.DataBind();
        }

        private void RptChannels_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsDescendantOwningChannelId(channelId)) e.Item.Visible = false;
            }
            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);

            var ltlRow = (Literal)e.Item.FindControl("ltlRow");

            ltlRow.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.SiteAnalysis, _additional, AuthRequest.AdminPermissionsImpl);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var siteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);
            PageUtils.Redirect(siteId > 0
                ? GetRedirectUrl(siteId, TbStartDate.Text, TbEndDate.Text)
                : PageAnalysisSite.GetRedirectUrl(TbStartDate.Text, TbEndDate.Text));
        }

        private void SetXHashtable(int siteId, string siteName)
        {
            if (siteId == YTypeOther)
            {
                if (!_xHashtable.ContainsKey(YTypeOther))
                {
                    _xHashtable.Add(YTypeOther, "其他");
                }
            }
            else if (!_xHashtable.ContainsKey(siteId))
            {
                _xHashtable.Add(siteId, siteName);
            }
            if (!_siteIdList.Contains(siteId))
            {
                _siteIdList.Add(siteId);
            }
            _siteIdList.Sort();
            _siteIdList.Reverse();
        }

        private string GetXHashtable(int siteId)
        {
            return _xHashtable.ContainsKey(siteId) ? _xHashtable[siteId].ToString() : string.Empty;
        }

        private void SetYHashtable(int publishemtSystemId, int value, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (!_yHashtableNew.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableNew.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableNew[publishemtSystemId].ToString());
                        _yHashtableNew[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeNew, value);
                    break;
                case YTypeUpdate:
                    if (!_yHashtableUpdate.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableUpdate.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUpdate[publishemtSystemId].ToString());
                        _yHashtableUpdate[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeUpdate, value);
                    break;
            }
            SetHorizental(publishemtSystemId, value);
        }

        private string GetYHashtable(int publishemtSystemId, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (_yHashtableNew.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableNew[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeUpdate:
                    if (_yHashtableUpdate.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUpdate[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";

                default:
                    return "0";
            }
        }

        private void SetHorizental(int siteId, int num)
        {
            if (_horizentalHashtable[siteId] == null)
            {
                _horizentalHashtable[siteId] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtable[siteId];
                _horizentalHashtable[siteId] = totalNum + num;
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
