using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteDownloads : BasePageCms
    {
        public DropDownList DdlSiteId;
        public Repeater RptContents;
        public Literal LtlTotalNum;

        public string StrArray { get; set; }

        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _siteIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableDownload = new Hashtable();
        private const string YTypeDownload = "YType_Download";

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteDownloads), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Settings.Chart);

            DdlSiteId.Items.Add(new ListItem("<<全部站点>>", "0"));
            var siteIdList = SiteManager.GetSiteIdListOrderByLevel();
            foreach (var siteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                DdlSiteId.Items.Add(new ListItem(siteInfo.SiteName, siteId.ToString()));

                var key = siteInfo.Id;
                //x轴信息
                SetXHashtable(key, siteInfo.SiteName);
                //y轴信息
                SetYHashtable(key, CountManager.GetCount(siteInfo.TableName, siteInfo.Id, ECountType.Download));
            }

            RptContents.DataSource = siteIdList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            foreach (var key in _siteIdList)
            {
                var yValueDownload = GetYHashtable(key);
                if (yValueDownload != "0")
                {
                    StrArray += $@"
xArrayDownload.push('{GetXHashtable(key)}');
                    yArrayDownload.push('{yValueDownload}');";
                }
            }
            LtlTotalNum.Text = GetVerticalTotalNum();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteId = (int) e.Item.DataItem;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var ltlSiteName = (Literal)e.Item.FindControl("ltlSiteName");
            var ltlDownloadNum = (Literal)e.Item.FindControl("ltlDownloadNum");

            ltlSiteName.Text = $@"<a href=""{PageAnalysisSiteDownloadsChannels.GetRedirectUrl(siteId)}"">{siteInfo.SiteName}</a>";
            ltlDownloadNum.Text = GetYHashtable(siteId);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var siteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);
            PageUtils.Redirect(siteId > 0
                ? PageAnalysisSiteDownloadsChannels.GetRedirectUrl(siteId)
                : GetRedirectUrl());
        }

        private void SetXHashtable(int siteId, string siteName)
        {
            if (!_xHashtable.ContainsKey(siteId))
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

        private void SetYHashtable(int publishemtSystemId, int value)
        {

            if (!_yHashtableDownload.ContainsKey(publishemtSystemId))
            {
                _yHashtableDownload.Add(publishemtSystemId, value);
            }
            else
            {
                var num = TranslateUtils.ToInt(_yHashtableDownload[publishemtSystemId].ToString());
                _yHashtableDownload[publishemtSystemId] = num + value;
            }
            SetVertical(YTypeDownload, value);

            SetHorizental(publishemtSystemId, value);
        }

        private string GetYHashtable(int publishemtSystemId)
        {
            if (!_yHashtableDownload.ContainsKey(publishemtSystemId)) return "0";
            var num = TranslateUtils.ToInt(_yHashtableDownload[publishemtSystemId].ToString());
            return num.ToString();
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

        private string GetVerticalTotalNum()
        {
            var totalNum = 0;
            foreach (int num in _verticalHashtable.Values)
            {
                totalNum += num;
            }
            return totalNum == 0 ? "0" : $"<strong>{totalNum}</strong>";
        }
    }
}
