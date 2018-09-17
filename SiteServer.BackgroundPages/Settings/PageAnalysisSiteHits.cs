using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteHits : BasePageCms
    {
        public DropDownList DdlSiteId;
        public Repeater RptContents;
        public Literal LtlVertical;

        public string StrArray { get; set; }

        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _siteIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableHits = new Hashtable();
        private const string YTypeHits = "YType_Hits";

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteHits), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Chart);

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
                SetYHashtable(key, DataProvider.ContentDao.GetTotalHits(siteInfo.TableName, siteId));
            }

            RptContents.DataSource = siteIdList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            foreach (var key in _siteIdList)
            {
                var yValueHits = GetYHashtable(key);
                if (yValueHits != "0")
                {
                    StrArray += $@"
xArrayHits.push('{GetXHashtable(key)}');
yArrayHits.push('{yValueHits}');";
                }
            }
            LtlVertical.Text = GetVerticalTotalNum();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteId = (int) e.Item.DataItem;

            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var ltlSiteName = (Literal)e.Item.FindControl("ltlSiteName");
            var ltlHitsNum = (Literal)e.Item.FindControl("ltlHitsNum");

            ltlSiteName.Text = $@"<a href=""{PageAnalysisSiteHitsChannels.GetRedirectUrl(siteId)}"">{siteInfo.SiteName}</a>";
            ltlHitsNum.Text = GetYHashtable(siteId);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var siteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);
            PageUtils.Redirect(siteId > 0
                ? PageAnalysisSiteHitsChannels.GetRedirectUrl(siteId)
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

            if (!_yHashtableHits.ContainsKey(publishemtSystemId))
            {
                _yHashtableHits.Add(publishemtSystemId, value);
            }
            else
            {
                var num = TranslateUtils.ToInt(_yHashtableHits[publishemtSystemId].ToString());
                _yHashtableHits[publishemtSystemId] = num + value;
            }
            SetVertical(YTypeHits, value);

            SetHorizental(publishemtSystemId, value);
        }

        private string GetYHashtable(int publishemtSystemId)
        {
            if (_yHashtableHits.ContainsKey(publishemtSystemId))
            {
                var num = TranslateUtils.ToInt(_yHashtableHits[publishemtSystemId].ToString());
                return num.ToString();
            }
            return "0";
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

        //private string GetHorizental(int siteId)
        //{
        //    if (_horizentalHashtable[siteId] == null) return "0";

        //    var num = TranslateUtils.ToInt(_horizentalHashtable[siteId].ToString());
        //    return (num == 0) ? "0" : $"<strong>{num}</strong>";
        //}

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

        //private string GetVertical(string type)
        //{
        //    if (_verticalHashtable[type] == null) return "0";

        //    var num = TranslateUtils.ToInt(_verticalHashtable[type].ToString());
        //    return (num == 0) ? "0" : $"<strong>{num}</strong>";
        //}

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
