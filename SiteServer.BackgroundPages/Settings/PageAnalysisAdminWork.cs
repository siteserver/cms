using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisAdminWork : BasePageCms
    {
        public DropDownList DdlSiteId;
        public DateTimeTextBox TbStartDate;
        public DateTimeTextBox TbEndDate;
        public PlaceHolder PhAnalysis;
        public Repeater RptContents;
        public SqlPager SpContents;

        public string StrArray { get; set; }

        private readonly List<string> _userNameList = new List<string>();
        private readonly Hashtable _xHashtableUser = new Hashtable();
        private readonly Hashtable _yHashtableUserNew = new Hashtable();
        private readonly Hashtable _yHashtableUserUpdate = new Hashtable();
        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";
        private readonly Hashtable _horizentalHashtableUser = new Hashtable();
        private readonly Hashtable _verticalHashtableUser = new Hashtable();
        private DateTime _begin;
        private DateTime _end;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl(int siteId, string startDate, string endDate)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisAdminWork), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
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
            var siteIdList = SiteManager.GetSiteIdListOrderByLevel();

            if (SiteId == 0 && siteIdList.Count > 0)
            {
                PageUtils.Redirect(GetRedirectUrl(siteIdList[0], DateUtils.GetDateAndTimeString(_begin), DateUtils.GetDateAndTimeString(_end)));
                return;
            }

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Chart);
            
            foreach (var siteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                DdlSiteId.Items.Add(new ListItem(siteInfo.SiteName, siteId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlSiteId, SiteId.ToString());

            TbStartDate.Text = DateUtils.GetDateAndTimeString(_begin);
            TbEndDate.Text = DateUtils.GetDateAndTimeString(_end);

            if (SiteInfo == null)
            {
                PhAnalysis.Visible = false;
                return;
            }

            var ds = DataProvider.ContentDao.GetDataSetOfAdminExcludeRecycle(SiteInfo.TableName, SiteId, _begin, _end);
            if (ds == null || ds.Tables.Count <= 0) return;

            var dt = ds.Tables[0];
            if (dt.Rows.Count <= 0) return;

            foreach (DataRow dr in dt.Rows)
            {
                SetXHashtableUser(dr["userName"].ToString(), dr["userName"].ToString());
                SetYHashtableUser(dr["userName"].ToString(), TranslateUtils.ToInt(dr["addCount"].ToString()), YTypeNew);
                SetYHashtableUser(dr["userName"].ToString(), TranslateUtils.ToInt(dr["updateCount"].ToString()), YTypeUpdate);
            }

            foreach (var key in _userNameList)
            {
                var yValueNew = GetYHashtableUser(key, YTypeNew);
                var yValueUpdate = GetYHashtableUser(key, YTypeUpdate);
                StrArray += $@"
xArrayNew.push('{GetXHashtableUser(key)}');
yArrayNew.push('{yValueNew}');
yArrayUpdate.push('{yValueUpdate}');";
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;
            SpContents.SortField = "UserName";
            SpContents.SortMode = SortMode.DESC;

            SpContents.SelectCommand = DataProvider.ContentDao.GetSqlStringOfAdminExcludeRecycle(SiteInfo.TableName, SiteId, _begin, _end);

            SpContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var userName = SqlUtils.EvalString(e.Item.DataItem, "userName");
            var addCount = SqlUtils.EvalInt(e.Item.DataItem, "addCount");
            var updateCount = SqlUtils.EvalInt(e.Item.DataItem, "updateCount");

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlContentAdd = (Literal)e.Item.FindControl("ltlContentAdd");
            var ltlContentUpdate = (Literal)e.Item.FindControl("ltlContentUpdate");

            ltlUserName.Text = userName;
            ltlDisplayName.Text = AdminManager.GetDisplayName(userName, false);

            ltlContentAdd.Text = addCount == 0 ? "0" : $"<strong>{addCount}</strong>";
            ltlContentUpdate.Text = updateCount == 0 ? "0" : $"<strong>{updateCount}</strong>";
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(GetRedirectUrl(TranslateUtils.ToInt(DdlSiteId.SelectedValue), TbStartDate.Text, TbEndDate.Text));
        }

        private void SetXHashtableUser(string userName, string siteName)
        {
            if (!_xHashtableUser.ContainsKey(userName))
            {
                _xHashtableUser.Add(userName, siteName);
            }
            if (!_userNameList.Contains(userName))
            {
                _userNameList.Add(userName);
            }
            _userNameList.Sort();
            _userNameList.Reverse();
        }

        private string GetXHashtableUser(string userName)
        {
            return _xHashtableUser.ContainsKey(userName) ? _xHashtableUser[userName].ToString() : string.Empty;
        }

        private void SetYHashtableUser(string userName, int value, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (!_yHashtableUserNew.ContainsKey(userName))
                    {
                        _yHashtableUserNew.Add(userName, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserNew[userName].ToString());
                        _yHashtableUserNew[userName] = num + value;
                    }
                    SetVerticalUser(YTypeNew, value);
                    break;
                case YTypeUpdate:
                    if (!_yHashtableUserUpdate.ContainsKey(userName))
                    {
                        _yHashtableUserUpdate.Add(userName, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserUpdate[userName].ToString());
                        _yHashtableUserUpdate[userName] = num + value;
                    }
                    SetVerticalUser(YTypeUpdate, value);
                    break;
            }
            SetHorizentalUser(userName, value);
        }

        private string GetYHashtableUser(string userName, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (_yHashtableUserNew.ContainsKey(userName))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserNew[userName].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeUpdate:
                    if (_yHashtableUserUpdate.ContainsKey(userName))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserUpdate[userName].ToString());
                        return num.ToString();
                    }
                    return "0";

                default:
                    return "0";
            }
        }

        private void SetHorizentalUser(string userName, int num)
        {
            if (_horizentalHashtableUser[userName] == null)
            {
                _horizentalHashtableUser[userName] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtableUser[userName];
                _horizentalHashtableUser[userName] = totalNum + num;
            }
        }

        private void SetVerticalUser(string type, int num)
        {
            if (_verticalHashtableUser[type] == null)
            {
                _verticalHashtableUser[type] = num;
            }
            else
            {
                var totalNum = (int)_verticalHashtableUser[type];
                _verticalHashtableUser[type] = totalNum + num;
            }
        }
    }
}
