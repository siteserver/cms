using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageVoteLog : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;
        public Button BtnReturn;

        private Dictionary<int, string> _idTitleMap;
        private int _voteId;
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int voteId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageVoteLog), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"voteId", voteId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _voteId = TranslateUtils.ToInt(Request.QueryString["voteID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.VoteLogDao.Delete(list);//删除投票记录
                        var voteLogInfoList = new List<VoteLogInfo>();
                        voteLogInfoList = DataProviderWx.VoteLogDao.GetVoteLogInfoListByVoteId(PublishmentSystemId, _voteId);//根据投票编号获取投票记录表中所有投票记录集合
                        var voteLogCollection = string.Empty;
                        var allCount = 0;
                        if (voteLogInfoList != null && voteLogInfoList.Count > 0)
                        {
                            allCount = voteLogInfoList.Count;
                            foreach (var vlist in voteLogInfoList)
                            {
                                voteLogCollection = voteLogCollection + vlist.ItemIdCollection + ",";//获取该次投票的所有的投票项目并拼接字符串
                            }
                            var strlength = voteLogCollection.Length;
                            voteLogCollection = voteLogCollection.Substring(0, strlength - 1);

                            var dict = new Dictionary<int, int>();
                            var arr = voteLogCollection.Split(',');
                            for (var i = 0; i < arr.Length; i++)
                            {
                                if (dict.ContainsKey(TranslateUtils.ToInt(arr[i])))
                                {
                                    dict[TranslateUtils.ToInt(arr[i])] += 1;//统计该次投票的每个项目的投票次数，重复的投票次数增1
                                }
                                else
                                {
                                    dict[TranslateUtils.ToInt(arr[i])] = 1;
                                }
                            }
                            var otherItemList = new List<int>();
                            foreach (var item in dict)
                            {
                                otherItemList.Add(TranslateUtils.ToInt(item.Key.ToString()));
                                DataProviderWx.VoteItemDao.UpdateVoteNumById(TranslateUtils.ToInt(item.Value.ToString()), TranslateUtils.ToInt(item.Key.ToString()));//修改该次投票的每个项目的投票次数
                            }
                            DataProviderWx.VoteItemDao.UpdateOtherVoteNumByIdList(otherItemList, 0, _voteId);
                            DataProviderWx.VoteDao.UpdateUserCountById(allCount, _voteId);//修改该次投票的总投票次数

                        }
                        else
                        {
                            DataProviderWx.VoteItemDao.UpdateAllVoteNumByVoteId(allCount, _voteId);//修改该次投票的每个项目的投票次数为0
                            DataProviderWx.VoteDao.UpdateUserCountById(allCount, _voteId);//修改该次投票的总投票次数为0
                        }


                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            
            SpContents.SelectCommand = DataProviderWx.VoteLogDao.GetSelectString(_voteId);
            SpContents.SortField = VoteLogAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {

                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdVote, "投票记录设置", AppManager.WeiXin.Permission.WebSite.Vote);
                var itemInfoList = DataProviderWx.VoteItemDao.GetVoteItemInfoList(_voteId);
                _idTitleMap = new Dictionary<int, string>();
                foreach (var itemInfo in itemInfoList)
                {
                    _idTitleMap[itemInfo.Id] = itemInfo.Title;
                }

                SpContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _voteId, _returnUrl), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的投票项", "此操作将删除所选投票项，确认吗？"));
                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var logInfo = new VoteLogInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlItemIdCollection = e.Item.FindControl("ltlItemIDCollection") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlIpAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlWxOpenId = e.Item.FindControl("ltlWXOpenID") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                var builder = new StringBuilder();
                foreach (var itemId in TranslateUtils.StringCollectionToIntList(logInfo.ItemIdCollection))
                {
                    if (_idTitleMap.ContainsKey(itemId))
                    {
                        builder.Append(_idTitleMap[itemId]).Append(",");
                    }
                }
                if (builder.Length > 0) builder.Length -= 1;
                ltlItemIdCollection.Text = builder.ToString();
                ltlUserName.Text = logInfo.UserName;
                ltlIpAddress.Text = logInfo.IpAddress;
                ltlWxOpenId.Text = logInfo.WxOpenId;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(logInfo.AddDate);
            }
        }

    }
}
