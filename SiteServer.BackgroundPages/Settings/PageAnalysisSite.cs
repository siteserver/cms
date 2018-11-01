using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSite : BasePageCms
    {
        public DropDownList DdlSiteId;
        public DateTimeTextBox TbStartDate;
        public DateTimeTextBox TbEndDate;
        public Repeater RptContents;
        public Literal LtlVerticalNew;
        public Literal LtlVerticalUpdate;
        public Literal LtlVerticalTotalNum;

        public string StrArray { get; set; }

        //总数
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        //sort key
        private readonly List<int> _siteIdList = new List<int>();
        //x
        private readonly Hashtable _xHashtable = new Hashtable();
        //y
        private readonly Hashtable _yHashtableNew = new Hashtable();
        private readonly Hashtable _yHashtableUpdate = new Hashtable();
        //y轴类型
        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";
        private DateTime _begin;
        private DateTime _end;

        public static string GetRedirectUrl(string startDate, string endDate)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSite), new NameValueCollection
            {
                {"startDate", startDate},
                {"endDate", endDate}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (string.IsNullOrEmpty(AuthRequest.GetQueryString("startDate")))
            {
                _begin = DateTime.Now.AddMonths(-1);
                _end = DateTime.Now;
            }
            else
            {
                _begin = TranslateUtils.ToDateTime(AuthRequest.GetQueryString("startDate"));
                _end = TranslateUtils.ToDateTime(AuthRequest.GetQueryString("endDate"));
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

            TbStartDate.Text = DateUtils.GetDateAndTimeString(_begin);
            TbEndDate.Text = DateUtils.GetDateAndTimeString(_end);

            BindGrid();

            foreach (var key in _siteIdList)
            {
                var yValueNew = GetYHashtable(key, YTypeNew);
                var yValueUpdate = GetYHashtable(key, YTypeUpdate);

                if (yValueNew != "0")
                {
                    StrArray += $@"
xArrayNew.push('{GetXHashtable(key)}');
yArrayNew.push('{yValueNew}');";
                }
                if (yValueUpdate != "0")
                {
                    StrArray += $@"
xArrayUpdate.push('{GetXHashtable(key)}');
yArrayUpdate.push('{yValueUpdate}');";
                }
            }

            LtlVerticalNew.Text = GetVertical(YTypeNew);
            LtlVerticalUpdate.Text = GetVertical(YTypeUpdate);
            LtlVerticalTotalNum.Text = GetVerticalTotalNum();
        }

        public void BindGrid()
        {
            var siteIdList = SiteManager.GetSiteIdList();
            
            foreach(var siteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);

                var key = siteInfo.Id;
                //x轴信息
                SetXHashtable(key, siteInfo.SiteName);
                //y轴信息
                SetYHashtable(key, DataProvider.ContentDao.GetCountOfContentAdd(siteInfo.TableName, siteInfo.Id, siteInfo.Id, EScopeType.All, _begin, _end, string.Empty, ETriState.All), YTypeNew);
                SetYHashtable(key, DataProvider.ContentDao.GetCountOfContentUpdate(siteInfo.TableName, siteInfo.Id, siteInfo.Id, EScopeType.All, _begin, _end, string.Empty), YTypeUpdate);
            }

            RptContents.DataSource = siteIdList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteId = (int) e.Item.DataItem;
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var ltlSiteName = (Literal)e.Item.FindControl("ltlSiteName");
            var ltlNewContentNum = (Literal)e.Item.FindControl("ltlNewContentNum");
            var ltlUpdateContentNum = (Literal)e.Item.FindControl("ltlUpdateContentNum");
            var ltlTotalNum = (Literal)e.Item.FindControl("ltlTotalNum");

            ltlSiteName.Text = $@"<a href=""{PageAnalysisSiteChannels.GetRedirectUrl(siteId)}"">{siteInfo.SiteName}</a>";
            ltlNewContentNum.Text = GetYHashtable(siteId, YTypeNew);
            ltlUpdateContentNum.Text = GetYHashtable(siteId, YTypeUpdate);
            ltlTotalNum.Text = GetHorizental(siteId);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var siteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);
            if (siteId > 0)
            {
                PageUtils.Redirect(PageAnalysisSiteChannels.GetRedirectUrl(siteId, TbStartDate.Text,
                    TbEndDate.Text));
            }
            else
            {
                PageUtils.Redirect(GetRedirectUrl(TbStartDate.Text, TbEndDate.Text));
            }
        }

        /// <summary>
        /// 设置x轴数据
        /// </summary>
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

        /// <summary>
        /// 获取x轴数据
        /// </summary>
        private string GetXHashtable(int siteId)
        {
            return _xHashtable.ContainsKey(siteId) ? _xHashtable[siteId].ToString() : string.Empty;
        }

        /// <summary>
        /// 设置y轴数据
        /// </summary>
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

        /// <summary>
        /// 获取y轴数据
        /// </summary>
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

        /// <summary>
        /// 设置y总数
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="num"></param>
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

        /// <summary>
        /// 获取y总数
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        private string GetHorizental(int siteId)
        {
            if (_horizentalHashtable[siteId] != null)
            {
                var num = TranslateUtils.ToInt(_horizentalHashtable[siteId].ToString());
                return (num == 0) ? "0" : $"<strong>{num}</strong>";
            }
            return "0";
        }

        /// <summary>
        /// 设置type总数
        /// </summary>
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

        /// <summary>
        /// 获取type总数
        /// </summary>
        private string GetVertical(string type)
        {
            if (_verticalHashtable[type] != null)
            {
                var num = TranslateUtils.ToInt(_verticalHashtable[type].ToString());
                return num == 0 ? "0" : $"<strong>{num}</strong>";
            }
            return "0";
        }

        private string GetVerticalTotalNum()
        {
            var totalNum = 0;
            foreach (int num in _verticalHashtable.Values)
            {
                totalNum += num;
            }
            return (totalNum == 0) ? "0" : $"<strong>{totalNum}</strong>";
        }
    }
}
