using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteHits : BasePageCms
    {
        public DropDownList DdlPublishmentSystemId;
        public Repeater RptContents;
        public Literal LtlVertical;

        public string StrArray { get; set; }

        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _publishmentSystemIdList = new List<int>();
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

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Chart);

            DdlPublishmentSystemId.Items.Add(new ListItem("<<全部站点>>", "0"));
            var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            foreach (var publishmentSystemId in publishmentSystemIdList)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                DdlPublishmentSystemId.Items.Add(new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemId.ToString()));

                var key = publishmentSystemInfo.PublishmentSystemId;
                //x轴信息
                SetXHashtable(key, publishmentSystemInfo.PublishmentSystemName);
                //y轴信息
                SetYHashtable(key, DataProvider.ContentDao.GetTotalHits(publishmentSystemInfo.AuxiliaryTableForContent, publishmentSystemId));
            }

            RptContents.DataSource = publishmentSystemIdList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            foreach (var key in _publishmentSystemIdList)
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

            var publishmentSystemId = (int) e.Item.DataItem;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var ltlPublishmentSystemName = (Literal)e.Item.FindControl("ltlPublishmentSystemName");
            var ltlHitsNum = (Literal)e.Item.FindControl("ltlHitsNum");

            ltlPublishmentSystemName.Text = $@"<a href=""{PageAnalysisSiteHitsChannels.GetRedirectUrl(publishmentSystemId)}"">{publishmentSystemInfo.PublishmentSystemName}</a>";
            ltlHitsNum.Text = GetYHashtable(publishmentSystemId);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var publishmentSystemId = TranslateUtils.ToInt(DdlPublishmentSystemId.SelectedValue);
            PageUtils.Redirect(publishmentSystemId > 0
                ? PageAnalysisSiteHitsChannels.GetRedirectUrl(publishmentSystemId)
                : GetRedirectUrl());
        }

        private void SetXHashtable(int publishmentSystemId, string publishmentSystemName)
        {
            if (!_xHashtable.ContainsKey(publishmentSystemId))
            {
                _xHashtable.Add(publishmentSystemId, publishmentSystemName);
            }
            if (!_publishmentSystemIdList.Contains(publishmentSystemId))
            {
                _publishmentSystemIdList.Add(publishmentSystemId);
            }
            _publishmentSystemIdList.Sort();
            _publishmentSystemIdList.Reverse();
        }

        private string GetXHashtable(int publishmentSystemId)
        {
            return _xHashtable.ContainsKey(publishmentSystemId) ? _xHashtable[publishmentSystemId].ToString() : string.Empty;
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

        private void SetHorizental(int publishmentSystemId, int num)
        {
            if (_horizentalHashtable[publishmentSystemId] == null)
            {
                _horizentalHashtable[publishmentSystemId] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtable[publishmentSystemId];
                _horizentalHashtable[publishmentSystemId] = totalNum + num;
            }
        }

        //private string GetHorizental(int publishmentSystemId)
        //{
        //    if (_horizentalHashtable[publishmentSystemId] == null) return "0";

        //    var num = TranslateUtils.ToInt(_horizentalHashtable[publishmentSystemId].ToString());
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
