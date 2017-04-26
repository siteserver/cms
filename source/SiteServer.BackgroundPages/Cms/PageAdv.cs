using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageAdv : BasePageCms
    {
        public DropDownList AdAreaNameList;
        public DataGrid dgContents;

        public Button AddAdv;
        public Button Delete;

        private int _adAreaId;

        public static string GetRedirectUrl(int publishmentSystemId, int adAreadId)
        {
            return PageUtils.GetCmsUrl(nameof(PageAdv), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"AdAreaID", adAreadId.ToString()}
            });
        }

        public string GetAdAreaName(int adAreaId)
        {
            var adAreaName=string.Empty;
            var adAreaInfo = DataProvider.AdAreaDao.GetAdAreaInfo(adAreaId,PublishmentSystemId);
            if (adAreaInfo != null)
            {
                adAreaName = adAreaInfo.AdAreaName;
            }
            return adAreaName;
         }

        public string GetIsEnabled(string isEnabledStr)
        {
            return StringUtils.GetTrueOrFalseImageHtml(isEnabledStr);
        }

        public string GetAdvInString(int advID)
        {
            var builder = new StringBuilder();
            var adInfo = DataProvider.AdvDao.GetAdvInfo(advID, PublishmentSystemId);
            if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToChannel))
            {
                builder.Append("栏目：");
                var nodeIDArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToChannel);
                foreach (int nodeID in nodeIDArrayList)
                {
                    builder.Append(NodeManager.GetNodeName(PublishmentSystemId, nodeID));
                    builder.Append(",");
                }
                builder.Length--;
            }
            if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToContent))
            {
                if (builder.Length > 0)
                {
                    builder.Append("<br />");
                }
                builder.Append("内容：");
                var nodeIDArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToContent);
                foreach (int nodeID in nodeIDArrayList)
                {
                    builder.Append(NodeManager.GetNodeName(PublishmentSystemId, nodeID));
                    builder.Append(",");
                }
                builder.Length--;
            }
            
            return builder.ToString();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _adAreaId = Body.GetQueryInt("AdAreaID");

            if (Body.IsQueryExists("Delete"))
            {
                var advId = Body.GetQueryInt("AdvID");
                try
                {
                    DataProvider.AdvDao.Delete(advId, PublishmentSystemId);

                    Body.AddSiteLog(PublishmentSystemId, "删除广告");

                     SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
         
            if (!Page.IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdAdvertisement, "固定广告管理", AppManager.Cms.Permission.WebSite.Advertisement);
                var adAreaInfoArrayList = DataProvider.AdAreaDao.GetAdAreaInfoArrayList(PublishmentSystemId);
                AdAreaNameList.Items.Add(new ListItem("<<所有广告位>>",string.Empty));
                if (adAreaInfoArrayList.Count > 0)
                {
                    foreach (AdAreaInfo adAreaInfo in adAreaInfoArrayList)
                    {
                        AdAreaNameList.Items.Add(new ListItem(adAreaInfo.AdAreaName,adAreaInfo.AdAreaID.ToString()));
                    }
                }
                Delete.Attributes.Add("onclick", "return confirm(\"此操作将删除所选广告，确定吗？\");");
               
                if (_adAreaId > 0)
                {
                    AdAreaNameList.SelectedValue = _adAreaId.ToString();
                }
            
            }
             BindGrid();

        }

        public void BindGrid()
        {
            try
            {
                if (string.IsNullOrEmpty(AdAreaNameList.SelectedValue))
                {
                    dgContents.DataSource = DataProvider.AdvDao.GetDataSource(PublishmentSystemId);
                }
                else
                {
                    if (!string.IsNullOrEmpty(AdAreaNameList.SelectedValue))
                    {
                        _adAreaId = TranslateUtils.ToInt(AdAreaNameList.SelectedValue);
                    }

                    dgContents.DataSource = DataProvider.AdvDao.GetDataSourceByAdAreaId(TranslateUtils.ToInt(AdAreaNameList.SelectedValue), PublishmentSystemId);
                }
                dgContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        public void ReFresh(object sender, EventArgs e)
        {
            BindGrid();
        }


        public void AddAdv_OnClick(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(AdAreaNameList.SelectedValue))
            {
                FailMessage("请选择广告位后进行操作！");
                return;
            }
            PageUtils.Redirect(PageAdvAdd.GetRedirectUrl(PublishmentSystemId, _adAreaId));
        }

        public void Delete_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (Request.Form["AdvIDCollection"] != null)
                {
                    var arraylist = TranslateUtils.StringCollectionToStringList(Request.Form["AdvIDCollection"]);
                    try
                    {
                        foreach (string advID in arraylist)
                        {
                            DataProvider.AdvDao.Delete(TranslateUtils.ToInt(advID), PublishmentSystemId);
                        }

                        Body.AddSiteLog(PublishmentSystemId, "删除广告");

                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                    BindGrid();
                }
                else
                {
                    FailMessage("请选择广告后进行操作！");
                }
            }
        }
    }
}
